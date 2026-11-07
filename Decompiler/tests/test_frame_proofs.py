"""Must-alias identities, conservative call effects and native contract recovery.

Synthetic linked-import contracts below are explicit harness inputs; the real
Mach-O import parser and whole CLI round trips are tested independently.
"""
import html
import json
import os
from pathlib import Path
import shutil
import tempfile

import test_pipeline as pipeline
import test_values as values
from test_functions import Code


HARNESS = r'''
using SharpRetro.Decompiler;
using System.Text.Json;
var image=BinaryImage.Open(args[0]);
foreach(string argument in args.Skip(2)) {
    var pair=argument.Split(':'); ulong address=Convert.ToUInt64(pair[0],16);
    ((IDictionary<ulong,string>)image.Imports)[address]=pair[1];
    ((IDictionary<ulong,string>)image.ImportLibraries)[address]="/usr/lib/libSystem.B.dylib";
}
var ranges=image.Regions.Where(r=>r.Executable).Select(r=>new CodeRange(r.Address,r.FileSize)).ToArray();
var program=ControlFlow.Recover(image,image.Entry,ranges,allowCalls:true);
var original=FunctionRecovery.Recover(program);
var frames=NativeMemoryRecovery.Recover(original,program.Entry);
var proof=FrameProofs.Analyze(program,original,frames);
var final=FunctionRecovery.Recover(program,proof.Preserved);
var dead=new HashSet<NativeEffectPoint>();
while(true) {
    var plans=NativeMemoryRecovery.Recover(final,program.Entry);
    var next=NativeDeadEffects.Find(program,final,plans,dead,proof.StdioCalls);
    if(next.Count==0) break;
    dead.UnionWith(next); final=FunctionRecovery.Recover(program,proof.Preserved,dead);
}
int[] Locals(IEnumerable<ValueSlot> values)=>values.Where(s=>s.Kind==SlotKind.Local).Select(s=>s.Index).Order().ToArray();
Console.WriteLine(JsonSerializer.Serialize(final.Functions.Values.ToDictionary(f=>f.Entry.ToString("x"),f=>new {
    Preserved=Locals(proof.Preserved[f.Entry]),Private=proof.PrivateMemory.Contains(f.Entry),
    Results=Locals(f.Results),OriginalResults=Locals(original.Functions[f.Entry].Results),
    Parameters=Locals(f.Parameters),MayWrite=Locals(f.MayWrite),
    Stdio=proof.StdioCalls.Where(p=>f.Blocks.Contains(p.Key) && !p.Value.Factory).ToDictionary(p=>p.Key.ToString("x"),p=>p.Value)
})));
if(args[1]!="-") File.WriteAllText(args[1],new FunctionEmitter().Emit(program,ControlFlowStyle.Auto,"native"));
'''


class FrameProofTests(pipeline.PipelineCase):
    execute=values.ValueRecoveryTests.execute
    driver=values.ValueRecoveryTests.driver

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.harness_temp=tempfile.TemporaryDirectory(prefix="sharpretro-frame-proofs-")
        root=Path(cls.harness_temp.name)
        references="".join(f'<Reference Include="{p.stem}"><HintPath>{html.escape(str(p))}</HintPath></Reference>'
                           for p in pipeline.DLL.parent.glob("*.dll"))
        (root/"Harness.csproj").write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup>{references}</ItemGroup></Project>''')
        (root/"Program.cs").write_text(HARNESS)
        (root/"NuGet.Config").write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        env={**os.environ,"DOTNET_CLI_TELEMETRY_OPTOUT":"1","DOTNET_NOLOGO":"1"}
        built=pipeline.run("dotnet","build",root/"Harness.csproj","-c","Release","-p:UseSharedCompilation=false","--verbosity","quiet",env=env)
        if built.returncode: raise RuntimeError(built.stdout+built.stderr)
        cls.harness=root/"bin/Release/net10.0/Harness.dll"
        for header in (pipeline.ROOT/"Decompiler").glob("*.hpp"): shutil.copy2(header,cls.harness.parent/header.name)

    @classmethod
    def tearDownClass(cls): cls.harness_temp.cleanup()

    def proof(self,code,imports=None,emit=False):
        self.image.write_bytes(pipeline.macho(code.finish() if isinstance(code,Code) else bytes.fromhex(code)))
        self.source=self.root/"values.cpp"
        linked=[f"{pipeline.BASE+code.labels[label]:x}:{symbol}" for label,symbol in (imports or {}).items()]
        result=pipeline.run("dotnet",self.harness,self.image,self.source if emit else "-",*linked)
        self.assertEqual(result.returncode,0,result.stdout+result.stderr)
        return json.loads(result.stdout)

    def test_saved_identity_contract_is_removed_and_native_behavior_matches(self):
        code=(Code().emit("4889fb").call("helper").emit("4889d8 c3")
              .label("helper").emit("53 bb11000000 5b c3"))
        proof=self.proof(code,emit=True)[f"{pipeline.BASE+code.labels['helper']:x}"]
        self.assertIn(3,proof["Preserved"])
        self.assertIn(3,proof["OriginalResults"])
        self.assertNotIn(3,proof["Results"])
        self.assertNotIn(3,proof["Parameters"])
        self.driver(self.source,"""
int main() {
  for(std::uint64_t n : {0ULL,42ULL,0x1122334455667788ULL,0xffffffffffffffffULL})
    if(sharp_retro::source::fn_1000(n)!=n) return 1;
}
""")

    def test_byte_overlap_kills_identity_but_disjoint_store_does_not(self):
        for offset,expected in (("01",False),("08",True)):
            with self.subTest(offset=offset):
                facts=self.proof("4883ec10 48891c24 c64424"+offset+"aa 488b1c24 4883c410 31c0 c3")["1000"]
                self.assertEqual(3 in facts["Preserved"],expected)

    def test_load_snapshot_survives_later_overlapping_store(self):
        facts=self.proof("53 488b0424 c6042400 4889c3 4883c408 31c0 c3")["1000"]
        self.assertIn(3,facts["Preserved"])

    def test_all_return_paths_and_parallel_outputs_are_required(self):
        code=(Code().emit("53 5b 85ff").branch("74","changed").emit("31c0 c3")
              .label("changed").emit("31db 31c0 c3"))
        self.assertNotIn(3,self.proof(code)["1000"]["Preserved"])
        facts=self.proof("4889d8 4889eb 4889c5 31c0 c3")["1000"]
        self.assertNotIn(3,facts["Preserved"]); self.assertNotIn(5,facts["Preserved"])

    def test_narrow_restore_does_not_preserve_high_bits(self):
        self.assertNotIn(3,self.proof("53 8b1c24 4883c408 31c0 c3")["1000"]["Preserved"])

    def test_unknown_address_write_invalidates_frame(self):
        facts=self.proof("53 c60700 5b 31c0 c3")["1000"]
        self.assertNotIn(3,facts["Preserved"])
        self.assertFalse(facts["Private"])

    def test_interprocedural_private_frame_summary_preserves_caller(self):
        code=(Code().emit("53").call("helper").emit("5b 31c0 c3")
              .label("helper").emit("53 bb11000000 5b c3"))
        facts=self.proof(code)
        self.assertIn(3,facts["1000"]["Preserved"])
        self.assertTrue(facts[f"{pipeline.BASE+code.labels['helper']:x}"]["Private"])

    def test_entry_pointer_write_is_not_a_private_callee_effect(self):
        code=(Code().emit("53 4889e7").call("helper").emit("5b 4889d8 c3")
              .label("helper").emit("48c7072a000000 c3"))
        facts=self.proof(code,emit=True)
        self.assertNotIn(3,facts["1000"]["Preserved"])
        self.assertFalse(facts[f"{pipeline.BASE+code.labels['helper']:x}"]["Private"])
        self.assertEqual(self.execute(self.source).returncode,42)

    def test_recursive_calls_do_not_bootstrap_a_preservation_proof(self):
        code=(Code().emit("53 85ff").branch("74","done").emit("ffcf").call("helper")
              .label("done").emit("5b 31c0 c3").label("helper").call("root").emit("c3"))
        code.labels['root']=0
        self.assertNotIn(3,self.proof(code)["1000"]["Preserved"])

    def test_syscall_read_write_bounds_and_unknown_count(self):
        for symbol,offset,count,expected in (("_read","08","ba08000000",True),
                                            ("_read","00","ba08000000",False),
                                            ("_read","08","",False),
                                            ("_write","00","ba08000000",True)):
            with self.subTest(symbol=symbol,offset=offset,count=count):
                code=(Code().emit("4883ec10 48891c24 488d7424"+offset+" 31ff "+count)
                      .call("io").emit("488b1c24 4883c410 31c0 c3").label("io"))
                self.assertEqual(3 in self.proof(code,{"io":symbol})["1000"]["Preserved"],expected)

    def test_opaque_stdio_does_not_gain_syscall_effect_contract(self):
        code=(Code().emit("4883ec10 48891c24 488d7c2408 be01000000 ba03000000 31c9")
              .call("io").emit("488b1c24 4883c410 31c0 c3").label("io"))
        self.assertNotIn(3,self.proof(code,{"io":"_fread"})["1000"]["Preserved"])

    def test_frame_exposure_is_sticky_across_later_opaque_call(self):
        # First call may retain the pointer. Re-save RBX afterward, then call
        # again with a nonframe argument: the earlier escape still matters.
        code=(Code().emit("53 4889e7").call("io").emit("48891c24 31ff")
              .call("io").emit("5b 31c0 c3").label("io"))
        self.assertNotIn(3,self.proof(code,{"io":"_fgetc"})["1000"]["Preserved"])

    def test_tail_only_function_does_not_claim_identity(self):
        # Both targets are independently established by direct calls.
        code=Code().call("helper").call("tail").emit("c3").label("tail").branch("e9","helper",4).label("helper").emit("53 5b 31c0 c3")
        facts=self.proof(code)
        self.assertEqual(facts[f"{pipeline.BASE+code.labels['tail']:x}"]["Preserved"],[])

    def test_may_write_propagation_handles_equal_size_contractions(self):
        code=(Code().emit("bb07000000").call("middle").emit("4889e8 c3")
              .label("middle").emit("53").call("leaf").emit("5b c3")
              .label("leaf").emit("bd2a000000 c3"))
        facts=self.proof(code,emit=True)
        self.assertIn(3,facts[f"{pipeline.BASE+code.labels['middle']:x}"]["Preserved"])
        self.assertIn(5,facts["1000"]["MayWrite"])
        self.assertEqual(self.execute(self.source).returncode,42)

    def test_native_spill_inputs_disappear_while_checked_contract_stays(self):
        code=(Code().emit("4889fb").call("helper").emit("4889d8 c3")
              .label("helper").emit("53 bb11000000 5b c3"))
        self.proof(code,emit=True)
        helper=f"fn_{pipeline.BASE+code.labels['helper']:x}"
        self.assertIn(helper+"()",self.source.read_text())
        checked=self.root/"checked.cpp"
        result=self.cli("decompile-app",self.image,"--memory","checked","--output",checked)
        self.assertEqual(result.returncode,0,result.stderr)
        self.assertIn("incoming_local_3",checked.read_text())

    def test_dead_store_keeps_division_fault(self):
        self.proof("4883ec08 31c0 31d2 89f9 f7f1 890424 b807000000 4883c408 c3",emit=True)
        self.driver(self.source,"""
int main() {
  if(sharp_retro::source::fn_1000(1)!=7) return 1;
  try { sharp_retro::source::fn_1000(0); return 2; }
  catch(const std::runtime_error&) { return 0; }
}
""")

    def test_branch_read_and_overwrite_union_keep_initial_value(self):
        code=(Code().emit("4883ec08 c6042415 85ff").branch("74","read")
              .emit("c604242a").label("read").emit("0fb60424 4883c408 c3"))
        self.proof(code,emit=True)
        self.driver(self.source,"""
int main() {
  return sharp_retro::source::fn_1000(0)==21 && sharp_retro::source::fn_1000(1)==42 ? 0 : 1;
}
""")

    def test_narrow_observation_keeps_overlapping_wide_store(self):
        self.proof("4883ec08 48c7042444332211 c6442402aa 0fb6442401 4883c408 c3",emit=True)
        self.assertEqual(self.execute(self.source).returncode,0x33)

    def test_snapshot_and_original_effect_ordinals_survive_store_removal(self):
        # The unknown pointer (RSI=0 in the driver) reads the first store; an
        # identical dead store later in the SAME block must not remove both.
        self.proof("4883ec08 48c704242a000000 4889e7 4809f7 488b07 48c704242a000000 4883c408 c3",emit=True)
        self.driver(self.source,"""
int main() { return sharp_retro::source::fn_1000(0)==42 ? 0 : 1; }
""")

    def test_write_observes_frame_and_read_preserves_short_input_bytes(self):
        for symbol in ("_write","_read"):
            with self.subTest(symbol=symbol):
                fd="bf01000000" if symbol=="_write" else "31ff"
                code=(Code().emit("4883ec08 c6042441 4889e6 ba01000000 "+fd).call("io")
                      .emit("0fb60424 4883c408 c3").label("io"))
                self.proof(code,{"io":symbol},emit=True)
                exe=self.root/"io-arm64"
                built=pipeline.run("clang++","-std=c++20","-O2","-fsanitize=address,undefined",self.source,"-o",exe)
                self.assertEqual(built.returncode,0,built.stderr)
                empty=pipeline.run(exe,input="")
                self.assertEqual((empty.returncode,empty.stdout,empty.stderr),(65,"A" if symbol=="_write" else "",""))
                if symbol=="_read":
                    filled=pipeline.run(exe,input="B")
                    self.assertEqual((filled.returncode,filled.stdout,filled.stderr),(66,"",""))

    def link_stdio(self,code,imports):
        # Distinct synthetic import addresses; these stub bytes are never lifted.
        for name in imports: code.label(name).emit("c3")
        return self.proof(code,imports)["1000"]

    def test_locally_opened_stream_allows_bounded_frame_read(self):
        code=(Code().emit("53 4883ec10").call("open").label("read_call")
              .emit("4889c1 4889e7 be01000000 ba08000000").call("read")
              .emit("4883c410 5b 31c0 c3"))
        facts=self.link_stdio(code,{"open":"_fopen","read":"_fread"})
        self.assertIn(3,facts["Preserved"])
        self.assertIn(f"{pipeline.BASE+code.labels['read_call']:x}",facts["Stdio"])
        self.assertNotIn(3,facts["Parameters"])

    def test_stream_proof_contracts_a_saved_result_observed_by_its_caller(self):
        code=(Code().emit("4889fb").call("helper").emit("4889d8 c3")
              .label("helper").emit("53 4883ec10").call("open")
              .emit("4889c1 4889e7 be01000000 ba08000000").call("read")
              .emit("4883c410 5b c3"))
        imports={"open":"_fopen","read":"_fread"}
        for name in imports: code.label(name).emit("c3")
        facts=self.proof(code,imports)[f"{pipeline.BASE+code.labels['helper']:x}"]
        self.assertIn(3,facts["OriginalResults"])
        self.assertIn(3,facts["Preserved"])
        self.assertNotIn(3,facts["Results"])
        self.assertNotIn(3,facts["Parameters"])

    def test_stream_spill_partial_corruption_does_not_corrupt_other_alias(self):
        for source,expected in (("488b0c24",False),("4c89e1",True)):
            with self.subTest(source=source):
                code=(Code().emit("53 4883ec20").call("open")
                      .emit("4989c4 48890424 c644240100 "+source+" 488d7c2410 be01000000 ba08000000")
                      .call("read").emit("4883c420 5b 31c0 c3"))
                facts=self.link_stdio(code,{"open":"_fopen","read":"_fread"})
                self.assertEqual(bool(facts["Stdio"]),expected)

    def test_unknown_store_revokes_cached_and_entry_stream_values(self):
        for capture in ("", "48890424 4c8b2424"):
            with self.subTest(capture=capture):
                code=Code().emit("53 4883ec20").call("open")
                if not capture: code.branch("eb","next").label("next")
                code.emit(capture+" c60700 "+("4c89e1" if capture else "4889c1")+
                          " 488d7c2410 be01000000 ba08000000").call("read").emit("4883c420 5b 31c0 c3")
                self.assertFalse(self.link_stdio(code,{"open":"_fopen","read":"_fread"})["Stdio"])

    def test_closing_stream_invalidates_register_and_spill_aliases(self):
        code=(Code().emit("53 4883ec10").call("open").emit("4989c4 48890424 4889c7")
              .call("close").label("read_call").emit("488b0c24 4889e7 be01000000 ba08000000")
              .call("read").emit("4883c410 5b 31c0 c3"))
        facts=self.link_stdio(code,{"open":"_fopen","close":"_fclose","read":"_fread"})
        self.assertEqual(len(facts["Stdio"]),1)
        self.assertNotIn(f"{pipeline.BASE+code.labels['read_call']:x}",facts["Stdio"])

    def test_reopen_at_same_site_does_not_revive_closed_alias(self):
        code=(Code().emit("53 4883ec10 4531ed").label("loop").call("open")
              .emit("4585ed").branch("75","old").emit("4989c4 4889c7").call("close")
              .emit("41ffc5").branch("eb","loop").label("old")
              .emit("4c89e1 4889e7 be01000000 ba08000000").call("read")
              .emit("4883c410 5b 31c0 c3"))
        facts=self.link_stdio(code,{"open":"_fopen","close":"_fclose","read":"_fread"})
        self.assertEqual(len(facts["Stdio"]),1) # Only the fresh first fclose.
        self.assertNotIn(3,facts["Preserved"])

    def test_read_into_another_stream_revokes_all_stream_facts(self):
        code=(Code().emit("53").call("open").emit("4989c4").call("open")
              .emit("4989c5 4889c1 4c89e7 be01000000 ba08000000").call("read")
              .label("getc_call").emit("4c89ef").call("getc").emit("5b 31c0 c3"))
        facts=self.link_stdio(code,{"open":"_fopen","read":"_fread","getc":"_fgetc"})
        self.assertEqual(len(facts["Stdio"]),1)
        self.assertNotIn(f"{pipeline.BASE+code.labels['getc_call']:x}",facts["Stdio"])

    def test_trusted_stream_counts_bound_write_without_assuming_exact_return(self):
        for arithmetic,expected in (("4889c2 48d1e2",None),
                                    ("4889c2 4801c2",16),
                                    ("4889c2 48b80000000000000080 480fafd0",None),
                                    ("4889c2 48b8ffffffffffffffff 4801c2",None),
                                    ("89c2 83ea09",None)):
            # SHL is outside this first range proof; addition by itself is
            # bounded, while overflowing arithmetic and SUB stay unknown.
            with self.subTest(arithmetic=arithmetic):
                code=(Code().emit("53 4883ec20").call("open").emit("4989c4 4889c1 4889e7 be01000000 ba08000000")
                      .call("read").label("write_call").emit(arithmetic+" 4c89e1 4889e7 be01000000")
                      .call("write").emit("4883c420 5b 31c0 c3"))
                facts=self.link_stdio(code,{"open":"_fopen","read":"_fread","write":"_fwrite"})
                span=facts["Stdio"][f"{pipeline.BASE+code.labels['write_call']:x}"]["Read"]
                self.assertEqual(None if span is None else span["Bytes"],expected)

    def test_zero_read_size_does_not_modify_unknown_destination(self):
        code=(Code().emit("53").call("open").emit("4889c1 31f6 ba03000000")
              .call("read").emit("5b 31c0 c3"))
        facts=self.link_stdio(code,{"open":"_fopen","read":"_fread"})
        self.assertIn(3,facts["Preserved"])
        self.assertTrue(facts["Private"])

    def test_returned_stream_does_not_automatically_trust_a_caller(self):
        code=(Code().emit("53 4883ec10").call("factory").emit("4889c1 4889e7 be01000000 ba08000000")
              .call("read").emit("4883c410 5b 31c0 c3").label("factory").call("open").emit("c3"))
        self.assertFalse(self.link_stdio(code,{"open":"_fopen","read":"_fread"})["Stdio"])

    def test_stream_validity_must_survive_every_incoming_path(self):
        code=(Code().emit("53 4883ec10").call("open").emit("4989c4 4585ed").branch("74","join")
              .emit("4889c7").call("close").label("join")
              .emit("4c89e1 4889e7 be01000000 ba08000000").call("read")
              .emit("4883c410 5b 31c0 c3"))
        facts=self.link_stdio(code,{"open":"_fopen","close":"_fclose","read":"_fread"})
        self.assertEqual(len(facts["Stdio"]),1) # Close only; join lost validity.
