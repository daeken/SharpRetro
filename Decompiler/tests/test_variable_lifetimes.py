"""Reaching-definition lifetime webs against compiled value/effect oracles."""
import html
import os

import test_pipeline as pipeline


HARNESS = r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] body)=>new(body);
CppLine L(string text,string? declares=null)=>new(text,declares);
CppIf I(string condition,CppSequence yes,CppSequence? no=null)=>new(new(condition),yes,no??S());
string Emit(CppSequence body){var lines=new List<string>();CppSourceTree.Emit(body,(s,d)=>lines.Add(new string(' ',4+d*4)+s));return string.Join("\n",lines);}
var rows=new Dictionary<string,(CppSequence Body,string Expected,string Trace,bool Split,string Type)>();
void Add(string name,CppSequence body,string expected,bool split=true,string trace="0",string type="std::uint64_t")=>rows[name]=(body,expected,trace,split,type);
Add("straight",S(L("x=seed;"),L("y=x;"),L("x=seed+1ULL;"),L("return y*10ULL+x;")),"seed*11+1");
Add("self_rhs",S(L("x=seed;"),L("x=x+1ULL;"),L("return x;")),"seed+1");
Add("branch_disjoint",S(I("choose",S(L("x=seed+1ULL;"),L("y=x;")),S(L("x=seed+2ULL;"),L("y=x;"))),L("return y;")),"choose?seed+1:seed+2");
Add("branch_join",S(I("choose",S(L("x=seed+1ULL;")),S(L("x=seed+2ULL;"))),L("return x;")),"choose?seed+1:seed+2",false);
Add("memory_snapshot",S(L("x=read64(data);"),L("y=x;"),L("write64(data,7ULL);"),L("x=read64(data);"),L("return y*10ULL+x;")),"(seed+3)*10+7",trace:"121");
Add("while_join",S(L("x=0ULL;"),new CppWhile("x<seed+2ULL",S(L("x=x+1ULL;"))),L("return x;")),"seed+2",false);
Add("do_join",S(L("x=0ULL;"),new CppDoWhile(new("x<seed+2ULL"),S(L("x=x+1ULL;"))),L("return x;")),"seed+2",false);
Add("loop_then_reuse",S(L("x=0ULL;"),new CppWhile("x<2ULL",S(L("x=x+1ULL;"))),L("y=x;"),L("x=seed+10ULL;"),L("return y*10ULL+x;")),"seed+30");
Add("compound_join",S(L("x=seed;"),L("x+=1ULL;"),L("x++;"),L("++x;"),L("x<<=1ULL;"),L("return x;")),"seed*2+6",false);
Add("compound_then_reuse",S(L("x=seed;"),L("x+=1ULL;"),L("y=x;"),L("x=seed+10ULL;"),L("return y*10ULL+x;")),"seed*11+20");
Add("scheduled_swap",S(L("x=1ULL;"),L("y=2ULL;"),L("const std::uint64_t edge_value=x;","edge_value"),L("x=y;"),L("y=edge_value;"),L("return x*10ULL+y;")),"21");
Add("opaque_reference",S(L("x=seed;"),L("x=opaque(x);"),L("y=x;"),L("x=seed+10ULL;"),L("return y*10ULL+x;")),"seed*11+40",false,trace:"1");
Add("address_alias",S(L("x=seed;"),L("std::uint64_t* alias=&((x));","alias"),L("*alias=seed+2ULL;"),L("y=x;"),L("x=seed+10ULL;"),L("return y*10ULL+x;")),"seed*11+30",false);
Add("reference_binding",S(L("x=seed;"),L("std::uint64_t& alias=x;","alias"),L("alias+=2ULL;"),L("y=x;"),L("x=seed+10ULL;"),L("return y*10ULL+x;")),"seed*11+30",false);
var reused=L("y=y+x;");
Add("reused_occurrences",S(L("x=seed;"),L("y=0ULL;"),new CppScope(S(reused)),L("x=seed+10ULL;"),new CppScope(S(reused)),L("return y;")),"2*seed+10");
Add("narrow_type",S(L("x=static_cast<std::uint8_t>(seed+255ULL);"),L("y=x;"),L("x=static_cast<std::uint8_t>(x+2ULL);"),L("return y*10ULL+x;")),"std::uint8_t(seed+255)*10ULL+std::uint8_t(std::uint8_t(seed+255)+2)",type:"std::uint8_t");
Add("mutable_pointer",S(L("x=data;"),L("y=address(x);"),L("x=byte_pointer(address(x)+1ULL);"),L("return y+address(x)-2ULL*address(data);")),"1",type:"std::uint8_t*");
Add("const_pointer",S(L("x=data;"),L("y=address(x);"),L("x=const_byte_pointer(address(x)+1ULL);"),L("return y+address(x)-2ULL*address(data);")),"1",type:"const std::uint8_t*");
Add("literal_and_comment",S(L("x=marked_string(\"x++ &x opaque(x) y\",seed); /* &x */"),L("y=x;"),L("x=marked_string(R\"q(x=y; // &x)q\",seed+1ULL);"),L("return y*10ULL+x;")),"seed*11+1",trace:"11");
Add("unused_effectful_defs",S(L("x=marked(seed);"),L("x=marked(seed+1ULL);"),L("return 5ULL;")),"5",trace:"11");
Add("branch_then_reuse",S(I("choose",S(L("x=seed;"),L("y=x;")),S(L("x=seed+1ULL;"),L("y=x;"))),L("x=seed+10ULL;"),L("return y+x;")),"choose?2*seed+10:2*seed+11");
Add("early_continue",S(L("x=0ULL;"),L("y=0ULL;"),new CppWhile("x<3ULL",S(L("x=x+1ULL;"),I("choose ? x==1ULL : false",S(L("continue;"))),L("y=y+x;"))),L("x=seed;"),L("return y*10ULL+x;")),"(choose?50:60)+seed");
Add("nested_mutation",S(L("x=seed;"),L("y=(++x);"),L("x=seed+10ULL;"),L("return y+x;")),"2*seed+11",false);
Add("volatile_owner",S(L("x=seed;"),L("y=x;"),L("x=seed+1ULL;"),L("return y*10ULL+x;")),"seed*11+1",false,type:"volatile std::uint64_t");
Dictionary<string,string> Types(string type="std::uint64_t")=>new(){{"x",type},{"y","std::uint64_t"},{"z","std::uint64_t"}};
foreach(var original in new[]{
 S(I("choose",S(L("x=seed;"))),L("y=x;"),L("x=seed+1ULL;"),L("return y+x;")),
 S(new CppWhile("choose",S(L("x=seed;"),L("break;"))),L("y=x;"),L("x=seed+1ULL;"),L("return y+x;")),
 S(L("x=seed;"),new CppScope(S(L("std::uint64_t x=3ULL;","x"),L("x=x+1ULL;"))),L("y=x;"),L("x=seed+10ULL;"),L("return y+x;")),
 S(L("x=seed;"),L("return x;"),L("x=seed+1ULL;"))
}){
 var result=CppVariableLifetimes.Split(original,Types());
 if(result.SplitVariables!=0||!ReferenceEquals(result.Body,original))throw new Exception("Undef/shadow/unreachable owner split");
}
foreach(string fragment in new[]{"goto done;","done:","// continuation "+(char)92,"/* open","auto closure=[x]{};"}){
 var original=S(L("x=seed;"),L(fragment),L("y=x;"),L("x=seed+1ULL;"),L("return y+x;"));
 if(!ReferenceEquals(CppVariableLifetimes.Split(original,Types()).Body,original))throw new Exception("Unsupported control/lexical input split");
}
foreach(string type in new[]{"SomeBox","constint","std::uint64_t&"}){
 var original=S(L("x=seed;"),L("y=x;"),L("x=seed+1ULL;"),L("return y+x;"));
 if(CppVariableLifetimes.Split(original,Types(type)).SplitVariables!=0)throw new Exception("Unknown/ref type split");
}
Console.WriteLine("#include <cstdint>\n#include <cstdio>\n#include <cstring>\nstd::uint64_t trace=0;template<class T>std::uint64_t address(T* p){return reinterpret_cast<std::uintptr_t>(p);}\nstd::uint8_t* byte_pointer(std::uint64_t n){return reinterpret_cast<std::uint8_t*>(n);}\nconst std::uint8_t* const_byte_pointer(std::uint64_t n){return reinterpret_cast<const std::uint8_t*>(n);}\nstd::uint64_t marked(std::uint64_t n){trace=trace*10+1;return n;}\nstd::uint64_t marked_string(const char*,std::uint64_t n){return marked(n);}\nstd::uint64_t opaque(std::uint64_t& n){trace=trace*10+1;n+=2;return n+1;}\nstd::uint64_t read64(std::uint8_t* p){trace=trace*10+1;std::uint64_t n;std::memcpy(&n,p,8);return n;}\nvoid write64(std::uint8_t* p,std::uint64_t n){trace=trace*10+2;std::memcpy(p,&n,8);}");
foreach(var(name,row)in rows){
 var types=Types(row.Type);var result=CppVariableLifetimes.Split(row.Body,types);
 if((result.SplitVariables!=0)!=row.Split)throw new Exception(name+" unexpected split "+result.SplitVariables);
 if(result.Types.Count!=types.Count+result.AddedLifetimes)throw new Exception("Incorrect type inventory");
 foreach(var(typeName,type)in result.Types)if(typeName.StartsWith("x_lifetime_",StringComparison.Ordinal)&&type!=row.Type)throw new Exception("Type/CV changed");
 if(name=="self_rhs"&&!Emit(result.Body).Contains("=x+1ULL;"))throw new Exception("Self RHS lost old web");
 if(name=="branch_disjoint"&&result.Types.Keys.Any(n=>n.StartsWith("y_lifetime_",StringComparison.Ordinal)))throw new Exception("Join outputs split");
 foreach(bool after in new[]{false,true}){
  Console.WriteLine($"std::uint64_t {(after?"after":"before")}_{name}(bool choose,std::uint64_t seed,std::uint8_t* data){{");
  Console.WriteLine(Emit(CppVariableScopes.Place(after?result.Body:row.Body,after?result.Types:types)));Console.WriteLine("}");
 }
}
Console.WriteLine("int main(){alignas(16)std::uint8_t data[64]{};for(int choose=0;choose<2;++choose)for(std::uint64_t seed=0;seed<4;++seed){");
foreach(var(name,row)in rows)foreach(string mode in new[]{"before","after"})Console.WriteLine($"trace=0;{{auto initial=seed+3ULL;std::memcpy(data,&initial,8);auto value={mode}_{name}(choose,seed,data);if(value!=std::uint64_t({row.Expected})||trace!=std::uint64_t({row.Trace})){{std::fprintf(stderr,\"{mode}_{name} seed=%llu choose=%d value=%llu trace=%llu\\n\",(unsigned long long)seed,choose,(unsigned long long)value,(unsigned long long)trace);return 1;}}}}");
Console.WriteLine("}}");
'''


class VariableLifetimeTests(pipeline.PipelineCase):
    def test_definition_webs_snapshots_compounds_and_conservative_owners(self):
        project = self.root / "Harness.csproj"
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup>
</Project>''')
        (self.root / "Program.cs").write_text(HARNESS)
        (self.root / "NuGet.Config").write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        env = {**os.environ, "DOTNET_CLI_TELEMETRY_OPTOUT": "1", "DOTNET_NOLOGO": "1"}
        built = pipeline.run("dotnet", "build", project, "-c", "Release", "--verbosity", "quiet",
                             "-p:UseSharedCompilation=false", env=env)
        self.assertEqual(built.returncode, 0, built.stdout + built.stderr)
        emitted = pipeline.run("dotnet", self.root / "bin/Release/net10.0/Harness.dll", env=env)
        self.assertEqual(emitted.returncode, 0, emitted.stdout + emitted.stderr)
        self.compile_run(emitted.stdout)
