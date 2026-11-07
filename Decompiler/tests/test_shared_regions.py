"""Closed and terminal source regions against compiled independent oracles."""
import html
import os

import test_pipeline as pipeline


HARNESS = r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] body)=>new(body);
CppLine L(string text,string? declares=null)=>new(text,declares);
CppLine D(string name,string value="0ULL",string type="std::uint64_t")=>L(type+" "+name+(value==""?"":"="+value)+";",name);
CppIf I(string condition,CppSequence yes,CppSequence? no=null)=>new(new(condition),yes,no??S());
CppDoWhile Do(string condition,CppSequence body)=>new(new(condition),body);
string Emit(CppSequence body){var lines=new List<string>();CppSourceTree.Emit(body,(s,d)=>lines.Add(new string(' ',4+d*4)+s));return string.Join("\n",lines);}
IEnumerable<CppLine> Lines(CppStatement s)=>s switch{CppLine l=>[l],CppSequence q=>q.Body.SelectMany(Lines),CppScope q=>Lines(q.Body),CppIf q=>Lines(q.Then).Concat(Lines(q.Else)),CppWhile q=>Lines(q.Body),CppDoWhile q=>Lines(q.Body),_=>[]};
var rows=new Dictionary<string,(CppSequence Body,string Expected,string Trace,bool Throws,bool? Changed)>();
void Add(string name,CppSequence body,string expected,string trace="0",bool throws=false,bool? changed=true)=>rows[name]=(body,expected,trace,throws,changed);
CppSequence Prefix(string temp)=>S(L(temp+"=seed+1ULL;"),L("out="+temp+";"),L(temp+"="+temp+"+2ULL;"),L("out=out+"+temp+";"),L("trace=trace*10+1ULL;"),L("trace=trace*10+2ULL;"));
Add("latent_uninitialized",S(D("a",""),D("b",""),D("out"),I("choose",Prefix("a"),Prefix("b")),L("return out;")),"2*seed+4","12");
CppSequence Local(string name)=>S(D(name,"seed+1ULL"),L("out="+name+";"),L(name+"="+name+"+2ULL;"),L("out=out+"+name+";"),L("trace=trace*10+1ULL;"),L("trace=trace*10+2ULL;"));
Add("local_binding",S(D("out"),I("choose",Local("a"),Local("b")),L("return out;")),"2*seed+4","12");
CppSequence CopyLoop(string counter)=>S(Do(counter+"<seed+2ULL",S(L("out=out+1ULL;"),L(counter+"="+counter+"+1ULL;"),L("trace=trace*10+"+counter+";"),I(counter+"==1ULL",S(L("continue;"))),I(counter+">=3ULL",S(L("break;"))),L("out=out+4ULL;"))));
Add("owned_loop_exits",S(D("a"),D("b"),D("out"),I("choose",CopyLoop("a"),CopyLoop("b")),L("return out;")),"seed==0?6:7","seed==0?12:123");
CppSequence End(string temp,bool throws=false)=>S(L(temp+"=seed+0x100ULL;"),L("trace=trace*10+1ULL;"),L(temp+"="+temp+"+257ULL;"),L("trace=trace*10+2ULL;"),L(throws?"throw std::runtime_error(\"expected\");":"return static_cast<std::uint8_t>("+temp+");"));
Add("terminal_conversion",S(D("a",""),D("b",""),D("c",""),I("choose",End("a"),S(I("(seed&1ULL)!=0ULL",End("b"),End("c"))))),"seed+1","12");
Add("terminal_throw",S(D("a",""),D("b",""),I("choose",End("a",true),End("b",true))),"0","12",throws:true);
CppSequence ContinuePrefix(string temp,string abrupt)=>S(L(temp+"=seed+1ULL;"),L("out=out+"+temp+";"),L(temp+"="+temp+"+2ULL;"),L("out=out+"+temp+";"),L("trace=trace*10+1ULL;"),L(abrupt));
Add("caller_continue",S(D("a"),D("b"),D("out"),D("iteration"),new CppWhile("iteration<2ULL",S(L("iteration=iteration+1ULL;"),I("choose",ContinuePrefix("a","continue;"),ContinuePrefix("b","continue;")))),L("return out;")),"4*seed+8","11");
Add("caller_break",S(D("a"),D("b"),D("out"),new CppWhile("true",S(I("choose",ContinuePrefix("a","break;"),ContinuePrefix("b","break;")))),L("return out;")),"2*seed+4","1");
CppDoWhile Swap(string a,string b,string counter)=>Do(counter+"<2ULL",S(D("edge_value",a,"const std::uint64_t"),L(a+"="+b+";"),L(b+"=edge_value;"),L("out=out*10+"+a+";"),L(counter+"="+counter+"+1ULL;")));
Add("scheduled_swap",S(D("a","1ULL"),D("b","2ULL"),D("c","3ULL"),D("d","4ULL"),D("first"),D("second"),D("out"),Swap("a","b","first"),Swap("c","d","second"),L("return out;")),"2143");
CppFor For(string name)=>new("std::uint64_t "+name+"=0ULL",name+"<2ULL",name+"="+name+"+1ULL",S(L("out=out+"+name+";"),L("out=out+1ULL;"),L("trace=trace*10+1ULL;"),I(name+"==0ULL",S(L("continue;"))),L("out=out+2ULL;")),name);
Add("closed_for_binding",S(D("out"),For("first"),For("second"),L("return out;")),"10","1111");
CppScope Shadow()=>new(S(D("value","seed+1ULL"),L("out=value;"),L("out=out+value;"),L("trace=trace*10+1ULL;"),L("trace=trace*10+2ULL;"),L("trace=trace*10+3ULL;")));
var reused=Shadow();
Add("shadow_reused_ast",S(D("value","100ULL"),D("out"),I("choose",S(reused),S(reused)),L("return out+value;")),"2*seed+102","123");
CppDoWhile EscapingCounter(string name)=>Do(name+"<2ULL",S(L(name+"="+name+"+1ULL;"),L("out=out+"+name+";"),L("trace=trace*10+1ULL;"),L("out=out+1ULL;")));
Add("declaration_live_out",S(D("out"),D("first"),EscapingCounter("first"),D("second"),EscapingCounter("second"),L("return first+second+out;")),"14","1111");
CppSequence PointerRead(string name)=>S(L("out=address("+name+");"),L("out=out+1ULL;"),L("out=out+2ULL;"),L("out=out+3ULL;"),L("out=out+4ULL;"));
Add("pointer_const_same",S(D("p","data","std::uint8_t* const"),D("q","data","std::uint8_t* const"),D("out"),I("choose",PointerRead("p"),PointerRead("q")),L("return out;")),"address(data)+10");
Add("pointer_const_different",S(D("p","data","std::uint8_t*"),D("q","data","const std::uint8_t*"),D("out"),I("choose",PointerRead("p"),PointerRead("q")),L("return out;")),"address(data)+10",changed:false);
CppSequence Alias(string a,string b)=>S(L(a+"="+a+"+1ULL;"),L(b+"="+b+"+2ULL;"),L("out="+a+"+"+b+";"),L("out=out+"+a+";"),L("out=out+"+b+";"));
Add("alias_pattern",S(D("a"),D("b"),D("c"),D("out"),I("choose",Alias("a","b"),Alias("c","c")),L("return out;")),"choose?6:12",changed:false);
CppSequence Marked(string name)=>S(L(name+"=marked(seed);"),L("out="+name+";"),L(name+"=marked("+name+"+1ULL);"),L("out=out+"+name+";"),L("out=out+2ULL;"),L("trace=trace*10+3ULL;"));
Add("ordered_calls",S(D("a"),D("b"),D("out"),I("choose",Marked("a"),Marked("b")),L("return out;")),"2*seed+3","113");
CppSequence AddressLocal(string name)=>S(D(name,"seed+1ULL"),L("remember("+name+");"),L("out="+name+";"),L("out=out+std::uint64_t(seen==&(("+name+")));"),L("out=out+1ULL;"),L("trace=trace*10+1ULL;"));
Add("interior_address",S(D("out"),I("choose",AddressLocal("a"),AddressLocal("b")),L("return out;")),"seed+4","1",changed:null);
CppSequence Inspect(string name)=>S(L("out=sizeof("+name+");"),L("out=out+sizeof("+name+");"),L("out=out+sizeof("+name+");"),L("out=out+sizeof("+name+");"),L("out=out+sizeof("+name+");"));
Add("introspection",S(D("a"),D("b"),D("out"),I("choose",Inspect("a"),Inspect("b")),L("return out;")),"40",changed:false);
Add("volatile",S(D("a","0ULL","volatile std::uint64_t"),D("b","0ULL","volatile std::uint64_t"),D("out"),I("choose",PointerRead("a"),PointerRead("b")),L("return out;")),"10",changed:false);
CppSequence Literal(string name)=>S(L(name+"=marked_string(\"variable_1 /* value_2 */\",seed);"),L("out="+name+";"),L(name+"=marked_string(R\"q(variable_1 // value_2)q\","+name+"+1ULL);"),L("out=out+"+name+";"),L("out=out+2ULL;"),L("trace=trace*10+3ULL;"));
Add("lexical_literals",S(D("a"),D("b"),D("out"),I("choose",Literal("a"),Literal("b")),L("return out;")),"2*seed+3","113");
var types=new Dictionary<string,string>{{"choose","bool"},{"seed","std::uint64_t"},{"data","std::uint8_t*"},{"trace","std::uint64_t"}};
var captures=new HashSet<string>{"choose","seed","data"};
foreach(string bad in new[]{"goto done;","done:","// continuation "+(char)92,"/* open"}){
 var original=S(L(bad),Prefix("a"),Prefix("b"),L("return out;"));
 if(!ReferenceEquals(CppSharedRegions.Factor(original,types,"std::uint64_t",captures).Body,original))throw new Exception("Unsupported lexical/control input changed");
}
foreach(string badType in new[]{"constint","SomeBox","std::uint64_t&"}){
 var original=S(I("choose",Prefix("a"),Prefix("b")),L("return out;"));
 var unknown=new Dictionary<string,string>(types){{"a",badType},{"b",badType},{"out",badType}};
 if(CppSharedRegions.Factor(original,unknown,"std::uint64_t",captures).Helpers.Count!=0)throw new Exception("Unknown type shared "+badType);
}
// A postfix array declarator must not be mistaken for scalar interior storage.
var arrayBody=S(I("choose",S(L("std::uint64_t a[4]{};","a"),L("out=address(a);"),L("out=out+1ULL;"),L("out=out+2ULL;"),L("out=out+3ULL;"),L("out=out+4ULL;")),S(L("std::uint64_t b[4]{};","b"),L("out=address(b);"),L("out=out+1ULL;"),L("out=out+2ULL;"),L("out=out+3ULL;"),L("out=out+4ULL;"))),L("return out;"));
if(CppSharedRegions.Factor(arrayBody,new Dictionary<string,string>(types){{"out","std::uint64_t"}},"std::uint64_t",captures).Helpers.Count!=0)throw new Exception("Array interior moved");
Console.WriteLine("#include <cstdint>\n#include <stdexcept>\n#include <cstdio>\n#include <cstring>\nstd::uint64_t trace=0;std::uint64_t* seen=nullptr;\ntemplate<class T>std::uint64_t address(T* p){return reinterpret_cast<std::uintptr_t>(p);}\nstd::uint64_t address(std::uint64_t n){return n;}\nstd::uint64_t marked(std::uint64_t n){trace=trace*10+1;return n;}\nstd::uint64_t marked_string(const char*,std::uint64_t n){return marked(n);}\nvoid remember(std::uint64_t& n){seen=&n;++n;}");
foreach(var (name,row) in rows){
 var result=CppSharedRegions.Factor(row.Body,types,"std::uint64_t",captures);
 if(row.Changed is { } expected&&(result.Helpers.Count!=0)!=expected)throw new Exception(name+" unexpected sharing, helpers="+result.Helpers.Count);
 if(result.Helpers.Any(h=>h.Params.Count>8)||result.Helpers.Count!=0&&result.StatementsSaved<=0)throw new Exception("Unprofitable or excessive helper");
 int Printed(CppSequence b){int n=0;CppSourceTree.Emit(b,(s,_)=>n+=1+s.Count(c=>c=='\n'));return n;}
 int actual=Printed(row.Body)-Printed(result.Body)-result.Helpers.Sum(h=>Printed(h.Body)+2);
 if(actual!=result.StatementsSaved)throw new Exception(name+" incorrect savings "+actual+" vs "+result.StatementsSaved);
 if(name=="declaration_live_out"&&!new[]{"first","second"}.All(n=>Lines(result.Body).Any(l=>l.Declares==n)))throw new Exception("Escaping declaration moved");
 if(name=="interior_address"&&result.Helpers.Any(h=>Lines(h.Body).Any(l=>l.Declares!=null)))throw new Exception("Address-sensitive interior moved");
 foreach(bool after in new[]{false,true}){
  Console.WriteLine($"std::uint64_t {(after?"after":"before")}_{name}(bool choose,std::uint64_t seed,std::uint8_t* data){{");
  if(after)foreach(var helper in result.Helpers){
   Console.WriteLine($"const auto {helper.Name}=[{string.Join(",",helper.Captures.Select(c=>"&"+c))}]({string.Join(",",helper.Params.Select(p=>"auto& "+p.Name))})->{helper.ReturnType}{{");
   Console.WriteLine(Emit(helper.Body));Console.WriteLine("};");
  }
  Console.WriteLine(Emit(after?result.Body:row.Body));Console.WriteLine("}");
 }
}
Console.WriteLine("int main(){std::uint8_t data[64]{};for(int choose=0;choose<2;++choose)for(std::uint64_t seed=0;seed<4;++seed){");
foreach(var (name,row) in rows)foreach(string mode in new[]{"before","after"})Console.WriteLine($"trace=0;seen=nullptr;try{{auto value={mode}_{name}(choose,seed,data);if({(row.Throws?"true":"false")}||value!=std::uint64_t({row.Expected})||trace!=std::uint64_t({row.Trace})){{std::fprintf(stderr,\"{mode}_{name} seed=%llu choose=%d value=%llu trace=%llu\\n\",(unsigned long long)seed,choose,(unsigned long long)value,(unsigned long long)trace);return 1;}}}}catch(const std::runtime_error&){{if(!{(row.Throws?"true":"false")}||trace!=std::uint64_t({row.Trace}))return 2;}}");
Console.WriteLine("}}");
'''


class SharedRegionTests(pipeline.PipelineCase):
    def test_closed_and_terminal_regions_preserve_bindings_effects_and_exits(self):
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
