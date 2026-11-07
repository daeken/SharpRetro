"""Adjacent integral return forwarding against compiled, independent oracles."""
import html
import os

import test_pipeline as pipeline


HARNESS = r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] body)=>new(body);
CppLine L(string text,string? declares=null)=>new(text,declares);
CppIf I(string condition,CppSequence yes,CppSequence no)=>new(new(condition),yes,no);
string Emit(CppSequence body) { var lines=new List<string>();CppSourceTree.Emit(body,(s,d)=>lines.Add(new string(' ',4+d*4)+s));return string.Join("\n",lines); }
IEnumerable<CppLine> Lines(CppStatement s)=>s switch {
 CppLine l=>new[]{l},CppSequence q=>q.Body.SelectMany(Lines),CppScope q=>Lines(q.Body),
 CppIf q=>Lines(q.Then).Concat(Lines(q.Else)),CppWhile q=>Lines(q.Body),CppDoWhile q=>Lines(q.Body),_=>Array.Empty<CppLine>()
};
var rows=new Dictionary<string,(CppSequence Body,string Type,string Return,string Expected,string Trace,string Throws,bool Changed)>();
void Add(string name,CppSequence body,string expected,bool changed,string type="std::uint64_t",string? result=null,string trace="0",string throws="false")
 =>rows[name]=(body,type,result??type,expected,trace,throws,changed);
Add("plain",S(L("x=7ULL;"),L("return x;")),"7",true);
Add("chain",S(L("x=marked(7ULL);"),L("y=x;"),L("return y;")),"7",true,trace:"1");
Add("chain_narrow",S(L("x=wide32();"),L("y=x;"),L("return y;")),"1",true,"std::uint32_t","std::uint64_t");
Add("self_read",S(L("x=7ULL;"),L("x=x+1ULL;"),L("return x;")),"8",true);
Add("wrap",S(L("x=0xffffffffffffffffULL;"),L("x=x+1ULL;"),L("return x;")),"0",true);
Add("narrow_cast",S(L("x=static_cast<std::uint32_t>(0ULL-static_cast<std::uint32_t>(choose));"),L("return x;")),"choose?0xffffffffULL:0ULL",true);
Add("implicit_narrow",S(L("x=wide(choose);"),L("return x;")),"choose?1:255",true,"std::uint8_t");
Add("mixed_width",S(L("x=wide32();"),L("return x;")),"1",false,"std::uint32_t","std::uint64_t");
Add("character",S(L("x='x';"),L("return x;")),"120",true);
Add("call_once",S(L("x=marked(7ULL);"),L("return x;")),"7",true,trace:"1");
Add("opaque_reference",S(L("x=3ULL;"),L("x=opaque(x);"),L("return x;")),"4",false,trace:"1");
Add("nested_opaque_reference",S(L("x=3ULL;"),L("x=static_cast<std::uint64_t>(opaque(x))>(1ULL);"),L("return x;")),"1",false,trace:"1");
Add("reference_binding",S(L("x=3ULL;"),L("std::uint64_t& alias=x;","alias"),L("alias=5ULL;"),L("x=9ULL;"),L("return x;")),"9",false);
Add("quoted_semicolons",S(L("x=marked_string(\"x; &x; // x\",7ULL);"),L("return x;")),"7",true,trace:"1");
Add("raw_semicolons",S(L("x=marked_string(R\"tag(x; &x; /* x */)tag\",7ULL);"),L("return x;")),"7",true,trace:"1");
Add("effect_between",S(L("x=marked(7ULL);"),L("trace=trace*10+2;"),L("return x;")),"7",false,trace:"12");
Add("comma_sequence",S(L("x=marked(7ULL),marked(9ULL);"),L("return x;")),"7",false,trace:"11");
Add("address_taken",S(L("x=3ULL;"),L("[[maybe_unused]] auto* alias=&((x));","alias"),L("x=9ULL;"),L("return x;")),"9",false);
Add("template_reference",S(L("x=3ULL;"),L("[[maybe_unused]] auto* alias=std::addressof<std::uint64_t>(x);","alias"),L("x=9ULL;"),L("return x;")),"9",false);
Add("nested_branches",S(I("choose",S(L("x=7ULL;"),L("return x;")),S(L("x=9ULL;"),L("return x;")))),"choose?7:9",true);
Add("shared_return",S(I("choose",S(L("trace=1;"),L("x=7ULL;")),S(L("trace=2;"),L("x=9ULL;"))),L("return x;")),"choose?7:9",true,trace:"choose?1:2");
Add("nested_shared",S(I("choose",S(I("other",S(L("trace=1;"),L("x=11ULL;")),S(L("trace=2;"),L("x=12ULL;")))),S(L("x=13ULL;"))),L("return x;")),"choose?(other?11:12):13",true,trace:"choose?(other?1:2):0");
Add("early_return",S(I("choose",S(L("return 17ULL;")),S(L("x=19ULL;"))),L("return x;")),"choose?17:19",true);
Add("loop_break",S(L("x=3ULL;"),new CppWhile("true",S(I("choose",S(L("break;")),S(L("x=7ULL;"))),L("return x;"))),L("return x;")),"choose?3:7",true);
Add("loop_continue",S(L("x=3ULL;"),L("int count=0;","count"),new CppWhile("++count<=2",S(I("choose && count==1",S(L("continue;")),S(L("x=7ULL;"))),L("return x;"))),L("return x;")),"7",true);
Add("zero_trip",S(L("x=3ULL;"),new CppWhile("choose",S(L("x=7ULL;"),L("return x;"))),L("return x;")),"choose?7:3",true);
Add("post_test",S(new CppDoWhile(new("false"),S(L("x=7ULL;"),L("return x;"))),L("return 9ULL;")),"7",true);
Add("throw_rhs",S(L("x=throwing();"),L("return x;")),"0",true,trace:"1",throws:"true");
Add("throw_arm",S(L("x=3ULL;"),I("choose",S(L("throwing();")),S(L("x=5ULL;"))),L("return x;")),"5",true,trace:"choose?1:0",throws:"choose");
Add("initialized",S(L("const std::uint64_t x=marked(7ULL);","x"),L("return x;")),"7",true,trace:"1");
Add("initialized_narrow",S(L("const std::uint8_t x=static_cast<std::uint8_t>(wide(choose));","x"),L("return x;")),"choose?1:255",true,"std::uint8_t");
Add("declared_width",S(L("std::uint32_t x=wide32();","x"),L("return x;")),"1",false);
Add("shadow",S(L("[[maybe_unused]] std::uint64_t x=3ULL;","x"),I("choose",S(L("std::uint64_t x=7ULL;","x"),L("x=9ULL;"),L("return x;")),S()),L("return x;")),"choose?9:3",false);
Add("comments",S(L("/* assignment x */ x=7ULL; // assignment end x"),L("/* return x */ return x; // return end x")),"7",true);
Add("rhs_line_comment",S(L("x=7ULL // RHS comment x\n;"),L("return x;")),"7",false);

// Unknown/qualified types and unsupported lexical fragments stay untouched.
foreach(string type in new[]{"volatile std::uint64_t","SomeBox","std::uint64_t&"}) {
 var original=S(L("x=7;"),L("return x;"));
 if(!ReferenceEquals(CppReturnForwarding.Reduce(original,new Dictionary<string,string>{{"x",type}},type),original))throw new Exception("Unknown type forwarded");
}
foreach(string fragment in new[]{"// x "+(char)92,"// x "+(char)92+(char)10+"x=9;","/* open x"}) {
 var original=S(L(fragment),L("x=7;"),L("return x;"));
 if(!ReferenceEquals(CppReturnForwarding.Reduce(original,new Dictionary<string,string>{{"x","std::uint64_t"}},"std::uint64_t"),original))throw new Exception("Unsupported lexical fragment forwarded");
}

Console.WriteLine("#include <cstdint>\n#include <memory>\n#include <stdexcept>\n#include <cstdio>");
Console.WriteLine("std::uint64_t trace=0; std::uint64_t marked(std::uint64_t n){trace=trace*10+1;return n;} std::uint64_t marked_string(const char*,std::uint64_t n){return marked(n);} std::uint64_t opaque(std::uint64_t& n){trace=trace*10+1;return n+1;} std::uint64_t wide(bool c){return c?257ULL:511ULL;} std::uint64_t wide32(){return 0x100000001ULL;} std::uint64_t throwing(){trace=trace*10+1;throw std::runtime_error(\"expected\");}");
foreach(var (name,row) in rows) {
 var types=new Dictionary<string,string>{{"x",row.Type}};
 if(name is "chain" or "chain_narrow")types["y"]="std::uint64_t";
 var reduced=CppReturnForwarding.Reduce(row.Body,types,row.Return);
 if((Emit(reduced)!=Emit(row.Body))!=row.Changed)throw new Exception(name+" unexpected forwarding decision");
 bool hasDeclaration=Lines(row.Body).Any(l=>l.Declares=="x");
 bool explicitStorage=name is "opaque_reference" or "nested_opaque_reference" or "reference_binding";
 foreach(bool clean in new[]{false,true}) {
  Console.WriteLine($"{row.Return} {(clean?"after":"before")}_{name}(bool choose,bool other) {{");
  var source=clean?reduced:row.Body;
  // Explicit declaration cases exercise that form directly; ordinary cases
  // also validate subsequent declaration placement/removal. The opaque T&
  // fixture lies outside that pass's emitter-owned by-value call contract.
  if(explicitStorage)foreach(var (variable,type) in types)Console.WriteLine($"[[maybe_unused]] {type} {variable};");
  else if(!hasDeclaration)source=CppVariableScopes.Place(source,types);
  Console.WriteLine(Emit(source)); Console.WriteLine("}");
 }
}
Console.WriteLine("int main(){for(int choose=0;choose<2;++choose)for(int other=0;other<2;++other){");
foreach(var (name,row) in rows)foreach(string mode in new[]{"before","after"})
 Console.WriteLine($"trace=0;try{{auto value={mode}_{name}(choose,other);if(({row.Throws})||value!=std::uint64_t({row.Expected})||trace!=std::uint64_t({row.Trace})){{std::fprintf(stderr,\"{mode}_{name} choose=%d other=%d value=%llu trace=%llu\\n\",choose,other,(unsigned long long)value,(unsigned long long)trace);return 1;}}}}catch(const std::runtime_error&){{if(!({row.Throws})||trace!=std::uint64_t({row.Trace})){{std::fprintf(stderr,\"{mode}_{name} unexpected exception\\n\");return 2;}}}}");
Console.WriteLine("}}");
'''


class ReturnForwardingTests(pipeline.PipelineCase):
    def test_exact_conversions_effects_scopes_and_branch_returns(self):
        project=self.root/"Harness.csproj"
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup>
</Project>''')
        (self.root/"Program.cs").write_text(HARNESS)
        (self.root/"NuGet.Config").write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        env={**os.environ,"DOTNET_CLI_TELEMETRY_OPTOUT":"1","DOTNET_NOLOGO":"1"}
        built=pipeline.run("dotnet","build",project,"-c","Release","--verbosity","quiet",
                           "-p:UseSharedCompilation=false",env=env)
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run("dotnet",self.root/"bin/Release/net10.0/Harness.dll",env=env)
        self.assertEqual(emitted.returncode,0,emitted.stderr)
        self.compile_run(emitted.stdout)
        def after(name):
            return emitted.stdout.split(f" after_{name}(",1)[1].split("\n}",1)[0]
        self.assertNotIn(" x",after("plain"))
        self.assertIn("return 7ULL;",after("plain"))
        self.assertNotIn(" x",after("chain"))
        self.assertNotIn(" y",after("chain"))
        self.assertIn("return marked(7ULL);",after("chain"))
        self.assertIn("std::uint32_t x",after("chain_narrow"))
        self.assertNotIn(" x",after("shared_return"))
        self.assertNotIn(" x",after("nested_shared"))
        self.assertIn("static_cast<std::uint32_t>",after("narrow_cast"))
        self.assertIn("/* assignment x */",after("comments"))
        self.assertIn("// return end x",after("comments"))
