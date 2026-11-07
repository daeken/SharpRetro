"""Same-type snapshots feeding return expressions, including native pointers."""
import html
import os
import test_pipeline as pipeline

HARNESS=r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] x)=>new(x);
CppLine L(string x,string? name=null)=>new(x,name);
string Emit(CppSequence b){var lines=new List<string>();CppSourceTree.Emit(b,(s,d)=>lines.Add(new string(' ',4+d)+s));return string.Join("\n",lines);}
var rows=new Dictionary<string,(CppSequence Body,string Expected,bool Changed,string X,string Y,string P,string Q,string Trace)>();
void Add(string name,CppSequence body,string expected,bool changed=true,string x="std::uint64_t",string y="std::uint64_t",string p="const u8*",string q="const u8*",string trace="0")=>rows[name]=(body,expected,changed,x,y,p,q,trace);
Add("arithmetic",S(L("x=y;"),L("return x+x*2ULL;")),"3*n");
Add("width_conversions",S(L("x=y;"),L("return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(x));")),"std::uint32_t(~std::uint32_t(n))");
Add("pointer_difference",S(L("p=q;"),L("return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(p)-address(data)));")),"std::uint32_t(~3U)");
Add("parenthesized_pointer_return",S(L("p=q;"),L("return (static_cast<std::uint32_t>(~(static_cast<std::uint32_t>((address(p)-address(data))))));")),"std::uint32_t(~3U)");
Add("pointer_bits",S(L("p=q;"),L("return address(p);")),"address(data)+3");
Add("pointer_cv",S(L("p=q;"),L("return address(p);")),"address(data)+3",false,q:"u8*");
Add("narrowing",S(L("x=y;"),L("return x+1ULL;")),"std::uint64_t(std::uint32_t(n))+1",false,x:"std::uint32_t");
Add("volatile_source",S(L("x=y;"),L("return x+1ULL;")),"n+1",false,y:"volatile std::uint64_t");
Add("opaque_mutation",S(L("x=y;"),L("return mutate(y)+x;")),"2*n+1",false,trace:"1");
Add("reference_binding",S(L("std::uint64_t& alias=x;","alias"),L("x=y;"),L("return x+alias;")),"2*n",false);
Add("reference_cast",S(L("x=y;"),L("return static_cast<std::uint64_t&>(x)+1ULL;")),"n+1",false);
Add("increment_source",S(L("x=y;"),L("return ++y+x;")),"2*n+1",false);
Add("introspection",S(L("x=y;"),L("return sizeof(x)+n;")),"8+n",false);
Add("comments",S(L("x=/* snapshot x */y; // copy x"),L("return x+1ULL /* return x */;")),"n+1");
Add("short_circuit",S(L("x=y;"),L("return (x!=0ULL)&&((10ULL/x)>1ULL);")),"n>0&&n<6");
Add("member_binding",S(L("x=y;"),L("return object.x+x;")),"99+n",false);
Add("ordered_earlier_call",S(L("mutate(y);"),L("x=y;"),L("return x+1ULL;")),"n+2",trace:"1");
Console.WriteLine("#include <cstdint>\n#include <cstdio>\nusing u8=std::uint8_t;std::uint64_t trace;template<class T>std::uint64_t address(T* p){return reinterpret_cast<std::uintptr_t>(p);}std::uint64_t mutate(std::uint64_t& n){trace=1;return ++n;}");
foreach(var(name,row) in rows){
 var types=new Dictionary<string,string>{{"x",row.X},{"y",row.Y},{"p",row.P},{"q",row.Q}};
 var reduced=CppReturnForwarding.Reduce(row.Body,types,"std::uint64_t");
 if((Emit(row.Body)!=Emit(reduced))!=row.Changed)throw new Exception(name+" unexpected forwarding");
 if(name=="comments"&&!Emit(reduced).Contains("return x */"))throw new Exception("Comment text replaced");
 foreach(bool after in new[]{false,true}){
  Console.WriteLine("std::uint64_t "+(after?"after_":"before_")+name+"(std::uint64_t n,u8* data){");
  Console.WriteLine("[[maybe_unused]] "+row.X+" x=0;[[maybe_unused]] "+row.Y+" y=n;[[maybe_unused]] "+row.P+" p=data;[[maybe_unused]] "+row.Q+" q=data+3;struct Object{std::uint64_t x;};[[maybe_unused]] Object object{99};");
  Console.WriteLine(Emit(after?reduced:row.Body));Console.WriteLine("}");
 }
}
Console.WriteLine("int main(){u8 data[64]{};for(std::uint64_t n:{0ULL,1ULL,2ULL,5ULL,6ULL,255ULL,0x100000001ULL,0xffffffffffffffffULL}){");
foreach(var(name,row) in rows)foreach(string mode in new[]{"before_","after_"})Console.WriteLine("trace=0;{auto result="+mode+name+"(n,data);if(result!=std::uint64_t("+row.Expected+")||trace!=std::uint64_t("+row.Trace+")){std::fprintf(stderr,\""+mode+name+" n=%llu value=%llu\\n\",(unsigned long long)n,(unsigned long long)result);return 1;}}");
Console.WriteLine("}}");
'''

class CopyReturnExpressionTests(pipeline.PipelineCase):
    def test_typed_copy_forwarding_preserves_snapshots_and_conversions(self):
        project=self.root/'Harness.csproj'
        project.write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env={**os.environ,'DOTNET_NOLOGO':'1'})
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode,0,emitted.stdout+emitted.stderr)
        self.compile_run('#include <initializer_list>\n'+emitted.stdout)
