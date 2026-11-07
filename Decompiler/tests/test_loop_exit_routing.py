"""Exact exit ownership, terminal continuations, bindings and scope effects."""
import html
import os
import test_pipeline as pipeline

HARNESS=r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] x)=>new(x);
CppLine L(string x,string? n=null)=>new(x,n);
CppIf I(string x,CppSequence y,CppSequence? no=null)=>new(new(x),y,no??S());
CppExitLoop E(ulong h,ulong t)=>new(h,t);
CppLine D(string n,string v="0ULL")=>L("[[maybe_unused]] std::uint64_t "+n+"="+v+";",n);
CppMultiExitLoop Loop(CppSequence body,CppSequence a,CppSequence b,ulong h=1)=>new(h,body,[new(10,a),new(20,b)]);
string Emit(CppSequence body){var lines=new List<string>();CppSourceTree.Emit(body,(s,d)=>lines.Add(new string(' ',4+d)+s));return string.Join("\n",lines);}
var rows=new Dictionary<string,(CppSequence Body,string Expected,string Trace,bool Selector,int Budget)>();
void Add(string name,CppSequence body,string expected,string trace="0",bool selector=false,int budget=4096)=>rows[name]=(body,expected,trace,selector,budget);
CppSequence Select(ulong h=1)=>S(I("n==0",S(E(h,10)),S(E(h,20))));
Add("terminal",S(Loop(Select(),S(L("return 11ULL;")),S(L("return 22ULL;")))),"n==0?11:22");
Add("common_return",S(D("out"),Loop(Select(),S(L("out=11ULL;")),S(L("out=22ULL;"))),L("return out;")),"n==0?11:22");
var grouped=Loop(Select(),S(L("out=11ULL;")),S(L("out=22ULL;")));
Add("grouped_common_return",S(S(D("out"),S(grouped)),new CppScope(S(L("return out;")))),"n==0?11:22");
Add("conditional_common_return",S(D("out"),I("n<8",S(new CppScope(S(grouped))),S(L("out=77;"))),L("return out;")),"n<8?(n==0?11:22):77");
Add("normal_exits",S(D("out"),D("i","n"),Loop(S(I("i==0ULL",S(E(1,10))),I("i==3ULL",S(E(1,20))),L("out+=i;"),L("--i;")),S(L("out+=11ULL;")),S(L("out+=22ULL;"))),L("return out;")),"n<=2?n*(n+1)/2+11:n*(n+1)/2-6+22");
var inner=Loop(Select(2),S(E(1,10)),S(E(1,20)),2);
Add("nested_terminal",S(Loop(S(inner),S(L("return 11ULL;")),S(L("return 22ULL;")))),"n==0?11:22");
Add("nested_common_return",S(D("out"),Loop(S(inner),S(L("out=11ULL;")),S(L("out=22ULL;"))),L("return out;")),"n==0?11:22");
// The effectful common suffix must run exactly once after the chosen outer exit,
// even when it becomes a terminal continuation inside the nested loop.
Add("nested_outer_break",S(D("out"),Loop(S(inner),S(L("out=11ULL;")),S(L("out=22ULL;"))),L("trace=trace*10+out;"),I("n==99",S(L("return 99ULL;"))),L("return out;")),"n==0?11:22","n==0?11:22");
var continuing=Loop(S(I("i!=0ULL",S(E(2,10)),S(E(2,20)))),S(L("out+=3ULL;"),L("--i;"),L("continue;")),S(E(1,10)),2);
Add("outer_continue",S(D("out"),D("i","n"),new CppMultiExitLoop(1,S(continuing),[new CppExitArm(10,S(L("return out;")))])),"3*n");
var external=Loop(Select(),S(L("out=11;"),L("break;")),S(L("out=22;"),L("break;")));
Add("outer_break_dispatch",S(D("out"),new CppWhile("true",S(external)),L("return out;")),"n==0?11:22",selector:true);
var externalContinue=Loop(Select(),S(L("out=11;"),L("i=0;"),L("continue;")),S(L("out=22;"),L("i=0;"),L("continue;")));
Add("outer_continue_dispatch",S(D("out"),D("i","1ULL"),new CppWhile("i!=0",S(externalContinue)),L("return out;")),"n==0?11:22",selector:true);
var owned=S(new CppWhile("true",S(L("out+=3ULL;"),L("break;"))),L("out+=2ULL;"));
Add("owned_nested_break",S(D("out"),Loop(Select(),owned,S(L("out=19ULL;"))),L("return out;")),"n==0?5:19");
Add("shadow_barrier",S(D("out"),D("value","4ULL"),Loop(S(D("value","9ULL"),Select()),S(L("out=value;")),S(L("out=value;"))),L("return out;")),"4",selector:true);
Add("interior_shadow",S(Loop(S(D("value","9ULL"),Select()),S(D("value","5ULL"),L("return value;")),S(D("value","6ULL"),L("return value;")))),"n==0?5:6");
Add("destructor_barrier",S(Loop(S(L("Watcher watcher;","watcher"),Select()),S(L("return trace;")),S(L("return trace;")))),"1","1",selector:true);
Add("conditional_destructor",S(I("n==0",S(L("Watcher watcher;","watcher")),S(L("Watcher watcher;","watcher"))),L("return trace;")),"1","1");
Add("scope_binding_common_return",S(D("out"),D("value","4ULL"),new CppScope(S(D("value","9ULL"),grouped)),L("return value+out;")),"4+(n==0?11:22)");
Add("reference_barrier",S(D("out"),Loop(S(L("[[maybe_unused]] std::uint64_t& alias=out;","alias"),Select()),S(L("out=11ULL;")),S(L("out=22ULL;"))),L("return out;")),"n==0?11:22",selector:true);
Add("scheduled_copies",S(D("a","1ULL"),D("b","2ULL"),Loop(S(L("const std::uint64_t saved=a;","saved"),L("a=b;"),L("b=saved;"),Select()),S(L("return a*10+b;")),S(L("return b*10+a;")))),"n==0?21:12");
Add("call_order",S(D("out"),Loop(S(L("trace=trace*10+1;"),Select()),S(L("out=marked(7);")),S(L("out=marked(9);"))),L("trace=trace*10+3;"),L("return out;")),"n==0?7:9","123");
Add("budget_fallback",S(Loop(Select(),S(L("return 11ULL;")),S(L("return 22ULL;")))),"n==0?11:22",selector:true,budget:0);
// A malformed untagged break cannot silently become a valid normal exit.
var malformed=S(new CppMultiExitLoop(1,S(L("break;")),[new CppExitArm(10,S(L("return 9ULL;")))]));
if(!Emit(CppLoopExits.Lower(malformed)).Contains("invalid recovered loop exit"))throw new Exception("Untargeted break optimized");
Console.WriteLine("#include <cstdint>\n#include <stdexcept>\n#include <cstdio>\nstd::uint64_t trace;struct Watcher{~Watcher(){++trace;}};std::uint64_t marked(std::uint64_t v){trace=trace*10+2;return v;}");
foreach(var(name,row) in rows){
 var rewritten=CppLoopExits.Lower(row.Body,row.Budget);string emitted=Emit(rewritten);
 if(emitted.Contains("loop_exit_")!=row.Selector)throw new Exception(name+" unexpected selector state");
 foreach(bool after in new[]{false,true})Console.WriteLine("std::uint64_t "+(after?"after_":"before_")+name+"(std::uint64_t n){\n"+Emit(after?rewritten:CppLoopExits.Lower(row.Body,0))+"\n}");
}
Console.WriteLine("int main(){for(std::uint64_t n=0;n<9;++n){");
foreach(var(name,row) in rows)foreach(string mode in new[]{"before_","after_"})Console.WriteLine("trace=0;{auto value="+mode+name+"(n);if(value!=std::uint64_t("+row.Expected+")||trace!=std::uint64_t("+row.Trace+")){std::fprintf(stderr,\""+mode+name+" n=%llu value=%llu trace=%llu\\n\",(unsigned long long)n,(unsigned long long)value,(unsigned long long)trace);return 1;}}");
Console.WriteLine("}}");
'''

class LoopExitRoutingTests(pipeline.PipelineCase):
    def test_exit_continuations_preserve_owners_bindings_and_effects(self):
        project=self.root/'Harness.csproj'
        project.write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env={**os.environ,'DOTNET_NOLOGO':'1'})
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode,0,emitted.stdout+emitted.stderr)
        self.compile_run(emitted.stdout)
