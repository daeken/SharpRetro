"""SSA pointer roles from real x64 instructions, independent of generated text."""
import html
import os

import test_pipeline as pipeline
from test_functions import Code


HARNESS = r'''
using SharpRetro.Decompiler;
using System.Reflection;
static ValueSlot S(int index)=>new(SlotKind.Local,index);
void Check(bool condition,string message) { if(!condition)throw new Exception(message); }
(NativePointerPlan Plan,FunctionSsa Ssa,RecoveredFunctions Functions,IReadOnlyDictionary<ulong,NativePointerPlan> All) Read(string name,string? import=null,bool omitHelper=false) {
 var image=BinaryImage.Open(Path.Combine(args[0],name+".bin"));
 if(import!=null)((Dictionary<ulong,string>)typeof(BinaryImage).GetField("imports",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(image)!)[0x2000]=import;
 var range=image.Regions.Single(r=>r.Executable);
 var program=ControlFlow.Recover(image,0x1000,[new(range.Address,range.FileSize)],allowCalls:true);
 var recovered=FunctionRecovery.Recover(program);
 var ssas=recovered.Functions.Where(p=>!omitHelper||p.Key==0x1000).ToDictionary(p=>p.Key,p=>FunctionSsa.Recover(program,recovered,p.Value)!);
 var plans=NativePointerRecovery.Analyze(program,recovered,ssas);
 return (plans[0x1000],ssas[0x1000],recovered,plans);
}
var read=Read("read");
Check(read.Plan.Parameters.TryGetValue(S(7),out var source)&&source.IsReadOnly&&source.Width==64,"direct read pointer/width");
Check(read.Plan.Accesses.Single().Offset==0,"direct address normalization");
var write=Read("write");
Check(write.Plan.Parameters.TryGetValue(S(7),out var destination)&&!destination.IsReadOnly,"direct write mutable");
Check(!write.Plan.Parameters.ContainsKey(S(6)),"stored scalar not inferred pointer");
var offset=Read("offset");
Check(offset.Plan.Parameters.TryGetValue(S(7),out var derived)&&!derived.IsReadOnly,"derived write reaches base");
Check(offset.Plan.Accesses.Any(a=>a.Write&&a.Offset==8&&a.Base==offset.Ssa.Parameter(S(7))),"constant offset retains base");
var mixed=Read("mixed_width");
Check(mixed.Plan.Parameters[S(7)].Width==0,"mixed access widths retain bytes");
var copied=Read("copied_byte");
Check(copied.Plan.Parameters[S(7)].IsReadOnly&&!copied.Plan.Parameters[S(6)].IsReadOnly,"copying bytes does not expose containing source object");
var copiedWord=Read("copied_word");
Check(copiedWord.Plan.Parameters[S(7)].IsReadOnly&&!copiedWord.Plan.Parameters[S(6)].IsReadOnly,"copying words leaves source const");
var unresolvedRead=Read("numeric_read");
Check(unresolvedRead.Plan.Parameters[S(7)].IsReadOnly,"unresolved computed read does not imply a write");
var unknownLoad=Read("loaded_unknown_call",omitHelper:true);
Check(unknownLoad.Plan.Parameters[S(7)].IsReadOnly&&unknownLoad.Plan.Definitions.Any(p=>p.Key.Value?.Operation is LiftIl.IlLoad&&p.Value.Escapes&&!p.Value.IsReadOnly),"unknown callee captures loaded pointee while container stays const");
var escapedLoad=Read("escaped_loaded_pointer");
Check(escapedLoad.Plan.Parameters[S(7)].IsReadOnly&&escapedLoad.Functions.Blocks.Values.SelectMany(b=>b.Effects.Select(e=>(b.Block.Address,e.Value))).Any(p=>p.Value.Operation is LiftIl.IlLoad && escapedLoad.Plan.Role(p.Address,p.Value) is { Escapes:true,IsReadOnly:false }),"escaping loaded pointer retains separate mutable provenance");
var loaded=Read("loaded_pointer");
Check(loaded.Plan.Parameters[S(7)].IsReadOnly,"containing pointer read only");
Check(loaded.Functions.Blocks.Values.SelectMany(b=>b.Effects.Select(e=>(b.Block.Address,e.Value))).Any(p=>p.Value.Operation is LiftIl.IlLoad && loaded.Plan.Role(p.Address,p.Value) is { IsReadOnly:false }),"loaded pointee independent mutable family");
Check(!Read("ambiguous").Plan.Parameters.Any(),"ambiguous two operand sum abstains");
Check(!Read("truncated").Plan.Parameters.Any(),"truncation does not recover original pointer");
var difference=Read("difference");
Check(difference.Plan.Parameters.Count==2&&!difference.Plan.Results.Any(),"pointer difference remains scalar");
var loop=Read("loop");
Check(loop.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"anchored cyclic pointer phi");
var diamonds=Read("diamonds");
Check(diamonds.Plan.Definitions.Keys.Count(d=>d.Kind==SsaDefinitionKind.Phi)>=30,"shared diamond DAG stays bounded and retains pointer joins");
var pureRead=Read("pure_loop_read");
Check(pureRead.Plan.Parameters.TryGetValue(S(7),out var pureSource)&&pureSource.IsReadOnly&&pureRead.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"phi-only source cursor anchored backward");
var pureWrite=Read("pure_loop_write");
Check(pureWrite.Plan.Parameters.TryGetValue(S(7),out var pureDestination)&&!pureDestination.IsReadOnly&&pureWrite.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"phi-only destination cursor anchored backward");
var scalarIndex=Read("scalar_index");
Check(scalarIndex.Plan.Parameters.ContainsKey(S(7))&&!scalarIndex.Plan.Parameters.ContainsKey(S(6)),"numeric induction offset anchored separately");
var sentinel=Read("sentinel_phi");
Check(!sentinel.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"nonnull numeric sentinel blocks pointer phi");
var anchorless=Read("anchorless");
Check(!anchorless.Plan.Parameters.Any()&&!anchorless.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"cycles/null alone cannot anchor pointer");
var nullphi=Read("null_phi");
Check(nullphi.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"pointer/null phi compatible");
var badphi=Read("mixed_phi");
Check(!badphi.Plan.Definitions.Keys.Any(d=>d.Kind==SsaDefinitionKind.Phi),"pointer/scalar phi abstains");
Check(!Read("numeric_constant").Plan.Definitions.Any(),"raw numeric VA is not a pointer literal");
Check(Read("pc_relative").Plan.Results.ContainsKey(S(0)),"PC relative literal retains address role");
var helper=Read("read_helper");
Check(helper.Plan.Parameters.TryGetValue(S(7),out var h)&&h.IsReadOnly,"callee pointer/read signature reaches caller");
var mutable=Read("write_helper");
Check(mutable.Plan.Parameters.TryGetValue(S(7),out var m)&&!m.IsReadOnly,"callee mutation reaches caller");
var omitted=Read("read_helper",omitHelper:true);
Check(omitted.Plan.Parameters.TryGetValue(S(7),out var unknown)&&!unknown.IsReadOnly&&unknown.Escapes,"absent SSA call summary mutable");
var unresolvedArgument=Read("numeric_argument");
Check(!unresolvedArgument.Plan.Parameters[S(7)].IsReadOnly,"unresolved numeric pointer argument exposes base");
var unresolvedReturn=Read("numeric_return");
Check(!unresolvedReturn.Plan.Parameters[S(7)].IsReadOnly,"unresolved numeric pointer return exposes base");
var stored=Read("stored_bits");
Check(stored.Plan.Parameters[S(7)].Escapes&&!stored.Plan.Parameters[S(7)].IsReadOnly,"stored transformed pointer bits escape");
var importRead=Read("import_write","_write");
Check(importRead.Plan.Parameters[S(6)].IsReadOnly,"explicit output bridge reads its buffer");
var importWrite=Read("import_read","_read");
Check(!importWrite.Plan.Parameters[S(6)].IsReadOnly,"explicit input bridge mutates its buffer");
var returnPointer=Read("pointer_return");
Check(returnPointer.Plan.Results.ContainsKey(S(0)),"pointer result inferred");
foreach(var row in new[]{read,write,offset,mixed,loaded,difference,loop,nullphi,badphi,helper,mutable,stored})
 foreach(var (definition,role) in row.Plan.Definitions)Check(definition.Width==64,"only full64 carrier types");
Console.WriteLine("33 pointer role fixtures passed");
'''


class NativePointerRecoveryTests(pipeline.PipelineCase):
    def test_pointer_phi_constness_and_call_boundaries(self):
        fixtures = {
            "read": bytes.fromhex("488b07 c3"),
            "write": bytes.fromhex("488937 31c0 c3"),
            "offset": bytes.fromhex("488d4708 488930 31c0 c3"),
            "mixed_width": bytes.fromhex("0fb607 668b4f02 0307 01c8 c3"),
            "loaded_pointer": bytes.fromhex("488b07 48c7002a000000 31c0 c3"),
            "copied_byte": bytes.fromhex("0fb607 8806 31c0 c3"),
            "copied_word": bytes.fromhex("488b07 488906 31c0 c3"),
            "numeric_read": bytes.fromhex("488b07 488d0437 488b00 c3"),
            "escaped_loaded_pointer": bytes.fromhex("488b07 488906 48c7002a000000 31c0 c3"),
            "ambiguous": bytes.fromhex("488d0437 488b00 c3"),
            "truncated": bytes.fromhex("89f8 488b00 c3"),
            "difference": bytes.fromhex("488b07 488b0e 4889f8 4829f0 c3"),
            "numeric_constant": bytes.fromhex("48b80010000000000000 488b00 c3"),
            "pc_relative": bytes.fromhex("488d0500000000 c3"),
            "stored_bits": bytes.fromhex("488b07 4889f8 48d1e8 488906 31c0 c3"),
            "pointer_return": bytes.fromhex("488b07 4889f8 c3"),
            "import_write": bytes.fromhex("488b06 e8f80f0000 31c0 c3"),
            "import_read": bytes.fromhex("488b06 e8f80f0000 31c0 c3"),
        }
        fixtures["loop"] = (Code().emit("488b17 4889f8").label("head")
                            .emit("488b08 4883c008 ffce").branch("75", "head")
                            .emit("4889c8 c3").finish())
        for name, scalar in (("null_phi", "31c0"), ("mixed_phi", "4889d0")):
            fixtures[name] = (Code().emit("488b07 85f6").branch("74", "other")
                              .emit("4889f8").branch("eb", "join")
                              .label("other").emit(scalar).label("join")
                              .emit("488b00 c3").finish())
        fixtures["read_helper"] = (Code().emit("488b07").call("helper").emit("c3")
                                   .label("helper").emit("488b07 c3").finish())
        fixtures["write_helper"] = (Code().call("helper").emit("31c0 c3")
                                    .label("helper").emit("48c7072a000000 c3").finish())
        fixtures["numeric_argument"] = (Code().emit("488b07 488d3c37").call("helper").emit("31c0 c3")
                                        .label("helper").emit("48c7072a000000 c3").finish())
        fixtures["numeric_return"] = (Code().call("helper").emit("48c7002a000000 31c0 c3")
                                      .label("helper").emit("488b07 488d0437 c3").finish())
        fixtures["pure_loop_read"] = (Code().emit("4889f8").label("head").emit("488b10 4883c008 ffce")
                                      .branch("75", "head").emit("4889d0 c3").finish())
        fixtures["pure_loop_write"] = (Code().emit("4889f8").label("head").emit("488910 4883c008 ffce")
                                       .branch("75", "head").emit("31c0 c3").finish())
        fixtures["scalar_index"] = (Code().emit("31c0").label("head").emit("488b0cc7 48ffc0 4839f0")
                                    .branch("72", "head").emit("4889c8 c3").finish())
        fixtures["sentinel_phi"] = (Code().emit("488b07 85f6").branch("74", "other").emit("4889f8")
                                    .branch("eb", "join").label("other").emit("b807000000")
                                    .label("join").emit("488b00 c3").finish())
        fixtures["anchorless"] = (Code().emit("31c0").label("head").emit("488b08 4883c008 85f6")
                                  .branch("75", "head").emit("4889c8 c3").finish())
        fixtures["loaded_unknown_call"] = (Code().emit("488b07 488b08 4889c7").call("helper").emit("31c0 c3")
                                           .label("helper").emit("488b07 c3").finish())
        diamonds = Code().emit("488b07")
        for i in range(35):
            diamonds.emit("85f6").branch("75", f"right{i}").emit("4883c701").branch("eb", f"join{i}")
            diamonds.label(f"right{i}").emit("4883c702").label(f"join{i}")
        fixtures["diamonds"] = diamonds.emit("488b07 c3").finish()
        for name, code in fixtures.items():
            (self.root / (name + ".bin")).write_bytes(pipeline.macho(code))
        project = self.root / "Harness.csproj"
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference>
<Reference Include="LiftIl"><HintPath>{html.escape(str(pipeline.DLL.parent / 'LiftIl.dll'))}</HintPath></Reference></ItemGroup>
</Project>''')
        (self.root / "Program.cs").write_text(HARNESS)
        (self.root / "NuGet.Config").write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        env = {**os.environ, "DOTNET_CLI_TELEMETRY_OPTOUT": "1", "DOTNET_NOLOGO": "1"}
        built = pipeline.run("dotnet", "build", project, "-c", "Release", "--verbosity", "quiet",
                             "-p:UseSharedCompilation=false", env=env)
        self.assertEqual(built.returncode, 0, built.stdout + built.stderr)
        result = pipeline.run("dotnet", self.root / "bin/Release/net10.0/Harness.dll", self.root, env=env)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        self.assertIn("pointer role fixtures passed", result.stdout)
