"""Compiled move-coalescing adversaries with independent output oracles."""
import html
import os

import test_pipeline as pipeline


HARNESS = r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] b)=>new(b);
CppLine L(string s,string? d=null)=>new(s,d);
CppLine E(string s)=>new(s,IsEdgeTransfer:true);
CppIf I(string c,CppSequence yes,CppSequence? no=null)=>new(new(c),yes,no??S());
CppDoWhile D(CppSequence b,string c="again")=>new(new(c),b);
var reusedMove=E("b=a;");
var rows=new Dictionary<string,(CppSequence Body,string Expected,int Merges)> {
 ["simple"]=(S(L("a=5;"),E("b=a;"),L("return b;")),"5",1),
 ["source_mutation"]=(S(L("a=5;"),E("b=a;"),L("a=9;"),L("return b;")),"5",0),
 ["compound_mutation"]=(S(L("a=5;"),E("b=a;"),L("++a;"),L("return b;")),"5",0),
 ["self_definition"]=(S(L("a=5;"),E("b=a;"),L("b=b+1;"),L("return b;")),"6",1),
 ["cross_group"]=(S(L("a=1;"),L("c=2;"),L("int trace=a+c;","trace"),E("b=a;"),L("trace+=b;"),E("c=b;"),L("return trace+c;")),"5",1),
 ["scheduled_swap"]=(S(L("a=1;"),L("b=2;"),L("const auto edge_value=a;","edge_value"),E("a=b;"),E("b=edge_value;"),L("return a*10+b;")),"21",0),
 ["branch_snapshot"]=(S(L("a=1;"),I("choose",S(E("b=a;")),S(L("b=2;"))),L("a=3;"),L("return b;")),"choose ? 1 : 2",0),
 ["branch_good"]=(S(I("choose",S(L("a=1;")),S(L("a=2;"))),E("b=a;"),L("return b;")),"choose ? 1 : 2",1),
 ["outer_reentry"]=(S(L("a=7;"),L("b=0;"),L("int result=0;","result"),L("int i=0;","i"),new CppWhile("i<2",S(L("result+=b;"),L("a=9;"),I("choose",S(E("b=a;"))),L("++i;"))),L("return result;")),"choose ? 9 : 0",0),
 ["address_taken"]=(S(L("a=1;"),L("b=0;"),L("int* ptr=&b;","ptr"),E("b=a;"),L("a=2;"),L("return *ptr;")),"1",0),
 ["template_address"]=(S(L("a=1;"),L("b=0;"),L("int* ptr=std::addressof<int>(b);","ptr"),E("b=a;"),L("return *ptr;")),"1",0),
 ["literal_protection"]=(S(L("a=5;"),E("b=a; /* a b */ // a b"),L("const char* text=\"a b\";","text"),L("const char* raw=R\"q(a /* b */ \\\" b)q\";","raw"),L("char letter='b';","letter"),L("return b + (std::strcmp(text,\"a b\")!=0) + (letter!=98) + (raw[0]!='a');")),"5",1),
 ["multiline_comment"]=(S(L("a=5;"),L("/* a"),L("b */"),E("b=a;"),L("return b;")),"5",0),
 ["loop_coalescing"]=(S(L("a=0;"),new CppWhile("a<3",S(E("b=a;"),L("a=b+1;"))),L("return a;")),"3",1),
 ["bundle_destinations"]=(S(L("a=5;"),E("b=a;"),E("c=a;"),L("return a;")),"5",1),
 ["reused_occurrence"]=(S(L("a=5;"),reusedMove,L("a=9;"),L("int trace=b;","trace"),reusedMove,L("return trace+b;")),"14",0),
 ["width_mismatch"]=(S(L("a=0x100000001ULL;"),E("b=a;"),L("return b;")),"1",0),
 ["early_loop_exit"]=(S(L("a=7;"),L("b=1;"),L("int i=0;","i"),new CppWhile("true",S(L("++i;"),I("choose",S(L("break;"))),E("b=a;"),L("a=9;"),I("i==2",S(L("break;"))))),L("return b;")),"choose ? 1 : 9",0),
 ["goto_refusal"]=(S(L("a=5;"),L("goto done;"),E("b=a;"),L("done:"),L("return a;")),"5",0)
};
Console.WriteLine("#include <cstring>\n#include <memory>\n#include <cstdint>");
foreach(var (name,row) in rows) {
 var types=new Dictionary<string,string>{{"a","int"},{"b","int"},{"c","int"}};
 if(name=="width_mismatch") { types["a"]="std::uint64_t";types["b"]="std::uint32_t"; }
 var reduced=CppCarrierCoalescing.Reduce(row.Body,types);
 if(reduced.Merged!=row.Merges)throw new Exception(name+" merges="+reduced.Merged+" expected="+row.Merges);
 Console.Error.WriteLine(name+" merged="+reduced.Merged+" removed="+reduced.CopiesRemoved);
 foreach(bool clean in new[]{false,true}) {
  Console.WriteLine($"int {(clean?"after":"before")}_{name}(bool choose) {{");
  var body=clean?reduced.Body:row.Body;
  foreach(var (variable,type) in clean?reduced.Types:types)Console.WriteLine($"[[maybe_unused]] {type} {variable};");
  CppSourceTree.Emit(body,(s,d)=>Console.WriteLine(new string(' ',4+d*4)+s));
  Console.WriteLine("}");
 }
}

// These lexical forms cannot be modeled as independent AST fragments. Their
// exact original body/types must survive; no compiler warning/UB is an oracle.
var frozen=new Dictionary<string,string>{{"a","int"},{"b","int"}};
foreach(var fragment in new[]{"// a b "+(char)92+(char)10+" a=9;", "// a b "+(char)92+(char)13+(char)10+" a=9;", "// a b "+(char)92}) {
 var original=S(L("a=5;"),L(fragment),E("b=a;"),L("return b;"));
 var result=CppCarrierCoalescing.Reduce(original,frozen);
 if(!ReferenceEquals(result.Body,original) || !ReferenceEquals(result.Types,frozen) || result.Merged!=0 || result.CopiesRemoved!=0)
  throw new Exception("Line-spliced comment was not refused");
}
var declared=S(L("int a=5;","a"),E("b=a;"),L("return b;"));
if(CppCarrierCoalescing.Reduce(declared,frozen).Merged!=0)throw new Exception("Shadowable declaration admitted");
var unsupported=S(L("a=5;"),L("if(choose) break;"),E("b=a;"),L("return b;"));
if(CppCarrierCoalescing.Reduce(unsupported,frozen).Merged!=0)throw new Exception("Unmodeled control edge admitted");

Console.WriteLine("int main(){for(int choose=0;choose<2;++choose){");
foreach(var (name,row) in rows)Console.WriteLine($"if(before_{name}(choose)!=after_{name}(choose)||after_{name}(choose)!=({row.Expected}))return 1;");
Console.WriteLine("}}");

'''


class CarrierCoalescingTests(pipeline.PipelineCase):
    def test_interference_scopes_lexing_and_ordered_copies(self):
        project=self.root/"Harness.csproj"
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup>
</Project>''')
        (self.root/"Program.cs").write_text(HARNESS)
        (self.root/"NuGet.Config").write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        local_env={**os.environ,"DOTNET_CLI_TELEMETRY_OPTOUT":"1","DOTNET_NOLOGO":"1"}
        built=pipeline.run("dotnet","build",project,"-c","Release","--verbosity","quiet",
                           "-p:UseSharedCompilation=false",env=local_env)
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run("dotnet",self.root/"bin/Release/net10.0/Harness.dll",env=local_env)
        self.assertEqual(emitted.returncode,0,emitted.stderr)
        # Carriers are deliberately declared mutable outside the AST here; this
        # isolates coalescing from the separately tested scope/const pass.
        self.compile_run(emitted.stdout)
        after=emitted.stdout.split("int after_literal_protection(",1)[1].split("int before_multiline_comment(",1)[0]
        self.assertIn("/* a b */ // a b",after)
        self.assertIn('const char* text="a b";',after)
        self.assertIn('R"q(a /* b */',after)
        self.assertIn("char letter='b';",after)
        simple=emitted.stdout.split("int after_simple(",1)[1].split("int before_source_mutation(",1)[0]
        self.assertNotIn("b=a;",simple)
        self.assertIn("return a;",simple)
