"""Loop header rewrites: independent values, ordered effects and exit ownership."""
import html
import os
import test_pipeline as pipeline

HARNESS=r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] x)=>new(x);
CppLine L(string x,string? n=null)=>new(x,n);
CppIf I(string x,CppSequence b)=>new(new(x),b,S());
CppLine D(string n,string x="0ULL")=>L("std::uint64_t "+n+"="+x+";",n);
string Emit(CppSequence b){var lines=new List<string>();CppSourceTree.Emit(b,(s,d)=>lines.Add(new string(' ',4*d)+s));return string.Join("\n",lines);}
var rows=new Dictionary<string,(CppSequence Body,string Expected,bool For)>();
void Add(string n,CppSequence b,string expected,bool header=true)=>rows[n]=(b,expected,header);
Add("top_guard",S(D("out"),D("i"),new CppWhile("true",S(I("i>=n",S(L("break;"))),L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"n*(n-1)/2");
Add("live_out",S(D("out"),D("i"),new CppWhile("i<n",S(L("out+=i;"),L("++i;"))),L("return out+i;")),"n*(n+1)/2");
Add("owned_continue",S(D("out"),D("i"),new CppWhile("i<6ULL",S(L("++i;"),I("i==1ULL",S(L("continue;"))),L("out+=i;"),L("i=i+1ULL;"))),L("return out+i;")),"19",false);
Add("nested_continue",S(D("out"),D("i"),new CppWhile("i<n",S(D("j"),new CppWhile("j<2ULL",S(L("++j;"),L("++out;"),L("continue;"))),L("i=i+1ULL;"))),L("return out;")),"2*n");
Add("early_break",S(D("out"),D("i"),new CppWhile("i<n",S(I("i==3ULL",S(L("break;"))),L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"n<3?n*(n-1)/2:3");
Add("do_entry_true",S(D("out"),D("i"),new CppDoWhile(new("i<3ULL"),S(L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"3");
Add("do_entry_false",S(D("out"),D("i","3ULL"),new CppDoWhile(new("i<3ULL"),S(L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"3",false);
Add("octal_entry",S(D("out"),D("i","010ULL"),new CppDoWhile(new("i>9ULL"),S(L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"8",false);
Add("do_unknown_bound",S(D("out"),D("i"),new CppDoWhile(new("i<n"),S(L("out+=1ULL;"),L("i=i+1ULL;"))),L("return out;")),"n==0?1:n",false);
Add("guarded_do",S(D("out"),D("i"),I("i<n",S(new CppDoWhile(new("i<n"),S(L("out+=i;"),L("i=i+1ULL;"))))),L("return out;")),"n*(n-1)/2");
Add("escaped_address",S(D("out"),D("i"),new CppWhile("i<n",S(L("seen=&i;"),L("out+=i;"),L("i=i+1ULL;"))),L("return out+(n?*seen:0ULL);")),"n*(n+1)/2");
Add("reference_call",S(D("out"),D("i"),new CppWhile("i<n",S(L("remember(i);"),L("out+=i;"),L("i=i+1ULL;"))),L("return out+(n?*seen:0ULL);")),"n*(n+1)/2");
Add("step_shadow",S(D("out"),D("i"),new CppWhile("i<n",S(L("out+=i;"),L("++i;"),D("i","10ULL"),L("i=i+1ULL;"),L("out+=i;"))),L("return out;")),"n*(n-1)/2+11*n",false);
Add("destructor_observer",S(D("out"),D("i"),new CppWhile("i<n",S(L("Watcher watcher{&out,&i};","watcher"),L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"n*n",false);
Add("ordered_test",S(D("out"),D("i"),new CppWhile("true",S(I("!check(i,n)",S(L("break;"))),L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"n*(n-1)/2");
// Parenthesized and negative conditional metadata must keep exact guard polarity.
Add("negated_guard",S(D("out"),D("i"),new CppWhile("true",S(new CppIf(new("i<n",true),S(L("break;")),S()),L("out+=i;"),L("++i;"))),L("return out;")),"n*(n-1)/2");
Add("placed_attribute",S(D("out"),L("[[maybe_unused]] std::uint64_t i = 0ULL;","i"),new CppWhile("i<n",S(L("out+=i;"),L("i=i+1ULL;"))),L("return out;")),"n*(n-1)/2");
Console.WriteLine("#include <cstdint>\n#include <cstdio>\nstd::uint64_t* seen;std::uint64_t checks;void remember(std::uint64_t& i){seen=&i;}bool check(std::uint64_t i,std::uint64_t n){++checks;return i<n;}struct Watcher{std::uint64_t* out;std::uint64_t* i;~Watcher(){*out+=*i;}};");
foreach(var(name,row) in rows){
 var after=CppLoopHeaders.Rewrite(row.Body);string changed=Emit(after);
 if(changed.Contains("for(")!=row.For)throw new Exception(name+" unexpected header");
 if(name is "live_out" or "escaped_address" or "reference_call" && changed.Contains("for(std::uint64_t i"))throw new Exception(name+" declaration scope shrank");
 if(name=="placed_attribute"&&!changed.Contains("for([[maybe_unused]] std::uint64_t i"))throw new Exception("Placed declaration not folded");
 foreach(bool rewrite in new[]{false,true})Console.WriteLine("std::uint64_t "+(rewrite?"after_":"before_")+name+"(std::uint64_t n){\n"+Emit(rewrite?after:row.Body)+"\n}");
}
Console.WriteLine("int main(){for(std::uint64_t n=0;n<9;++n){");
foreach(var(name,row) in rows)foreach(string mode in new[]{"before_","after_"})Console.WriteLine("seen=nullptr;checks=0;if("+mode+name+"(n)!=std::uint64_t("+row.Expected+")){std::fprintf(stderr,\""+mode+name+" n=%llu\\n\",(unsigned long long)n);return 1;}"+(name=="ordered_test"?"if(checks!=n+1)return 2;":""));
Console.WriteLine("}}");
'''

class LoopHeaderTests(pipeline.PipelineCase):
    def test_loop_headers_preserve_first_iteration_updates_and_continues(self):
        project=self.root/'Harness.csproj'
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>''')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env={**os.environ,'DOTNET_NOLOGO':'1'})
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode,0,emitted.stdout+emitted.stderr)
        self.compile_run(emitted.stdout)
