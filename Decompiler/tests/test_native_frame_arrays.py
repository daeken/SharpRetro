"""Byte-exact native frame arrays and conservative direct-index bounds."""
import html
import os

import test_pipeline as pipeline
from test_functions import Code


HARNESS = r'''
using SharpRetro.Decompiler;
using LiftIl;
void Check(bool condition,string message){if(!condition)throw new Exception(message);}
(NativeFrameArrayPlan Plan,RecoveredFunctions Functions,Dictionary<ulong,NativeFunctionMemory> Frames,Dictionary<ulong,FunctionSsa> Ssas) Read(string name){
 var image=BinaryImage.Open(Path.Combine(args[0],name+".bin"));
 var region=image.Regions.Single(r=>r.Executable);
 var program=ControlFlow.Recover(image,0x1000,[new(region.Address,region.FileSize)],allowCalls:true);
 var recovered=FunctionRecovery.Recover(program);
 var frames=NativeMemoryRecovery.Recover(recovered,0x1000);
 var ssas=recovered.Functions.ToDictionary(p=>p.Key,p=>FunctionSsa.Recover(program,recovered,p.Value,frames[p.Key])!);
 return(NativeFrameArrays.Analyze(recovered,frames,ssas)[0x1000],recovered,frames,ssas);
}
IEnumerable<(ulong Block,RecoveredValue Address,int Width)> Loads(RecoveredFunctions functions)=>
 functions.Blocks.Values.SelectMany(b=>b.Effects.Where(e=>e.Kind=="evaluate"&&e.Value.Operation is IlLoad&&e.Value.Width<=128)
 .Select(e=>(b.Block.Address,e.Value.Inputs[0],e.Value.Width)));
NativeFrameElement? Dynamic(NativeFrameArrayPlan plan,RecoveredFunctions functions)=>Loads(functions)
 .Select(p=>plan.Element(p.Block,p.Address,p.Width)).FirstOrDefault(e=>e?.Index!=null);
var mask=Read("mask");
Check(mask.Plan.Arrays.Count==1&&mask.Plan.Arrays[0].Bytes==32&&mask.Plan.Arrays[0].ElementWidth==64,"four qword cells grouped");
Check(Dynamic(mask.Plan,mask.Functions)!=null,"mask permits bounded indexing");
var array=mask.Plan.Arrays.Single();
Check(mask.Plan.Find(array.Offset,128)==array,"whole vector view can alias grouped array");
Check(mask.Plan.Find(array.Offset+array.Bytes-4,64)==null,"view must fit entire group");
var first=mask.Functions.Blocks.Values.SelectMany(b=>b.Effects.Where(e=>e.Kind=="store").Select(e=>(b.Block.Address,e.Address))).First();
Check(mask.Plan.Element(first.Item1,first.Item2!,64) is { Index:null,BiasElements:0 },"constant element");
Check(mask.Plan.Element(first.Item1,first.Item2!,32)==null,"different width uses alias view");
var unbounded=Read("unbounded");Check(Dynamic(unbounded.Plan,unbounded.Functions)==null,"unbounded index refused");
var guarded=Read("guarded");Check(Dynamic(guarded.Plan,guarded.Functions)!=null,"same SSA unsigned dominating guard");
foreach(string name in new[]{"stale_guard","truncated_guard","signed_guard","bypass_guard","same_block_guard"}){
 var row=Read(name);Check(Dynamic(row.Plan,row.Functions)==null,name+" cannot prove direct indexing");
}
var vectors=Read("vectors");
Check(vectors.Plan.Arrays.Single().Bytes==64&&vectors.Plan.Arrays.Single().ElementWidth==32,"four vector fields become sixteen word elements");
Check(Dynamic(vectors.Plan,vectors.Functions)!=null,"vector-backed word index bounded");
var vectorsUnknown=Read("vectors_unknown");
Check(vectorsUnknown.Plan.Arrays.Single().ElementWidth==32&&Dynamic(vectorsUnknown.Plan,vectorsUnknown.Functions)==null,"storage element recovery independent of index proof");
var promotedCell=mask.Frames[0x1000].Scalars.Keys.OrderBy(c=>c.Offset).Skip(1).First();
var promotion=new NativePromotionPlan(new HashSet<(long,int)>{promotedCell},new Dictionary<int,(long,int)>(),new Dictionary<ValueEffect,(long,int)>(),new Dictionary<(long,int),string>());
var split=NativeFrameArrays.Analyze(mask.Functions,mask.Frames,mask.Ssas,new Dictionary<ulong,NativePromotionPlan>{{0x1000,promotion}})[0x1000];
Check(split.Arrays.Count==1&&split.Arrays[0].Bytes==16&&split.Arrays[0].Offset==array.Offset+16,"promoted physical gap splits grouping");
// Adversarial wide arithmetic: saturating 128-bit x to uint64 before >>63
// would invent maximum1; actual Trunc64((1<<66)>>63) is8 and out of bounds.
int id=100000;
RecoveredValue V(int width,Il? operation,params RecoveredValue[] inputs)=>new(id++,width,operation,inputs);
var dummy=new IlConst(IlType.U64,0);
var wide=V(128,null);
var shift=new RecoveredValue(id++,128,null,[],63);
var shifted=V(128,new IlBin(new IlType.I(false,128),BinOp.Shr,dummy,dummy),wide,shift);
var index=V(64,new IlCast(IlType.U64,CastKind.Trunc,dummy),shifted);
var stride=new RecoveredValue(id++,64,null,[],8);
var scaled=V(64,new IlBin(IlType.U64,BinOp.Mul,dummy,dummy),index,stride);
var address=V(64,new IlBin(IlType.U64,BinOp.Add,dummy,dummy),first.Item2!,scaled);
Check(mask.Plan.Element(first.Item1,address,64)==null,"wide shift then truncation stays unknown");
Console.WriteLine("14 frame array scenarios passed");
'''


class NativeFrameArrayTests(pipeline.PipelineCase):
    def test_groups_and_exact_value_bounds(self):
        prefix = "4883ec28 48c7042401000000 48c744240802000000 48c744241003000000 48c744241804000000"
        suffix = "4883c428 c3"
        fixtures = {
            "mask": bytes.fromhex(prefix + "83e703 488b04fc" + suffix),
            "unbounded": bytes.fromhex(prefix + "488b04fc" + suffix),
            "same_block_guard": bytes.fromhex(prefix + "488b04fc 4883ff03" + suffix),
        }
        for name, compare, update, branch in (
            ("guarded", "4883ff03", "", "77"),
            ("stale_guard", "4883ff03", "488d7f01", "77"),
            ("truncated_guard", "83ff03", "", "77"),
            ("signed_guard", "4883ff03", "", "7f"),
        ):
            fixtures[name] = (Code().emit(prefix + "31c0" + compare + update).branch(branch, "done")
                              .emit("488b04fc").label("done").emit(suffix).finish())
        fixtures["bypass_guard"] = (Code().emit(prefix + "31c0 85f6").branch("75", "access")
                                    .emit("4883ff03").branch("77", "done").label("access")
                                    .emit("488b04fc").label("done").emit(suffix).finish())
        vector_prefix = "4883ec48 0f110424 0f11442410 0f11442420 0f11442430"
        fixtures["vectors"] = bytes.fromhex(vector_prefix + "83e707 8b04bc 4883c448 c3")
        fixtures["vectors_unknown"] = bytes.fromhex(vector_prefix + "8b04bc 4883c448 c3")
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
        self.assertIn("frame array scenarios passed", result.stdout)
