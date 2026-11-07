"""Presentation cleanup keeps CV-sensitive uses, effects and strict compilation."""
import html
import os
import test_pipeline as pipeline

HARNESS=r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] x)=>new(x);
CppLine L(string text,string? name=null)=>new(text,name);
string Emit(CppSequence b){var lines=new List<string>();CppSourceTree.Emit(b,(s,d)=>lines.Add(new string(' ',4+d)+s));return string.Join("\n",lines);}
var rows=new Dictionary<string,(CppSequence Body,string Expected,string Trace)>();
void Add(string n,CppSequence b,string expected,string trace="0")=>rows[n]=(b,expected,trace);
Add("scalar",S(L("[[maybe_unused]] const std::uint64_t value=n+1;","value"),L("return value;")),"n+1");
Add("pointer",S(L("[[maybe_unused]] const u8* const value=data+3;","value"),L("return address(value)-address(data);")),"3");
Add("by_value",S(L("[[maybe_unused]] const std::uint64_t value=n;","value"),L("return by_value(value);")),"n+1");
Add("opaque_overload",S(L("[[maybe_unused]] const std::uint64_t value=n;","value"),L("return observe(value);")),"1");
Add("generic_reference",S(L("[[maybe_unused]] const std::uint64_t value=n;","value"),L("return generic(value);")),"1");
Add("address",S(L("[[maybe_unused]] const std::uint64_t value=n;","value"),L("return observe_pointer(&value);")),"1");
Add("reference",S(L("[[maybe_unused]] const std::uint64_t value=n;","value"),L("const std::uint64_t& alias=value;","alias"),L("return observe(alias);")),"1");
Add("template",S(L("[[maybe_unused]] const std::uint64_t count=3;","count"),L("std::array<int,count> values{};","values"),L("return values.size();")),"3");
Add("array_extent",S(L("[[maybe_unused]] const std::uint64_t count=3;","count"),L("int values[count]{};","values"),L("return sizeof(values)/sizeof(int);")),"3");
Add("decltype",S(L("[[maybe_unused]] const std::uint64_t value=n;","value"),L("return std::is_const_v<decltype(value)>;")),"1");
Add("unused_effect",S(L("[[maybe_unused]] const std::uint64_t unused=effect();","unused"),L("return 7ULL;")),"7","1");
Add("write_only",S(L("[[maybe_unused]] std::uint64_t unused=0;","unused"),L("unused=effect();"),L("return 7ULL;")),"7","1");
Add("mutation_only",S(L("[[maybe_unused]] std::uint64_t unused=0;","unused"),L("++unused;"),L("unused=unused+1ULL;"),L("return 7ULL;")),"7");
Add("duplicate_bindings",S(new CppScope(S(L("[[maybe_unused]] const std::uint64_t value=1;","value"))),new CppScope(S(L("[[maybe_unused]] const std::uint64_t value=9;","value"),L("return value;")))),"9");
Add("literal_comment",S(L("[[maybe_unused]] const std::uint64_t value=n; // const value","value"),L("return value+literal(\"const value [[maybe_unused]]\");")),"n+1");
Add("numeric_bool",S(L("return (!!n);")),"bool(n)");
Add("bool_arithmetic",S(L("return 1ULL+(!!n);")),"1+bool(n)");
Add("eager_bitwise",S(L("return (n&1ULL)|(n&2ULL);")),"n&3ULL");
Add("right_group",S(L("return n-(n+1ULL);")),"~0ULL");
Add("shift_group",S(L("return (n+1ULL)<<(2ULL+1ULL);")),"(n+1ULL)<<3ULL");
Add("cast_numeric_bool",S(L("return static_cast<std::uint32_t>((!!n)+1ULL);")),"1+bool(n)");
Add("cast_right_group",S(L("return static_cast<std::uint64_t>(n-(n+1ULL));")),"~0ULL");
Add("cast_nan",S(L("double a=nan_value();","a"),L("return static_cast<std::uint32_t>(!(a<=1.0));")),"1");
Add("comma_argument",S(L("return address(byte_offset(data,(++trace,n)))-address(data);")),"n","1");
var byValue=new HashSet<string>{"by_value","literal"};
Console.WriteLine("#include <cstdint>\n#include <cstdio>\n#include <array>\n#include <type_traits>\n#include <limits>\nusing u8=std::uint8_t;std::uint64_t trace;template<class T>std::uint64_t address(T* p){return reinterpret_cast<std::uintptr_t>(p);}template<class T>T* byte_offset(T* p,std::uint64_t n){return p+n;}double nan_value(){return std::numeric_limits<double>::quiet_NaN();}std::uint64_t by_value(std::uint64_t v){return v+1;}std::uint64_t observe(const std::uint64_t&){return 1;}std::uint64_t observe(std::uint64_t&){return 2;}template<class T>std::uint64_t generic(T&){return std::is_const_v<T>?1:2;}std::uint64_t observe_pointer(const std::uint64_t*){return 1;}std::uint64_t observe_pointer(std::uint64_t*){return 2;}std::uint64_t effect(){++trace;return 9;}std::uint64_t literal(const char*){return 1;}");
foreach(var(name,row) in rows){
 var clean=CppReadability.Clean(row.Body,byValue);string after=Emit(clean);
 if(name=="scalar"&&(after.Contains("const")||after.Contains("maybe_unused")))throw new Exception("Scalar declaration still noisy");
 if(name=="pointer"&&(!after.Contains("const u8* value")||after.Contains("* const")))throw new Exception("Wrong pointer qualification");
 if(name is "unused_effect" or "write_only" or "mutation_only" && !after.Contains("[[maybe_unused]]"))throw new Exception("Unused binding warning guard removed");
 foreach(bool rewritten in new[]{false,true})Console.WriteLine("std::uint64_t "+(rewritten?"after_":"before_")+name+"(std::uint64_t n,u8* data){\n"+Emit(rewritten?clean:row.Body)+"\n}");
}
var alias="void f(){\n int frame;\n [[maybe_unused]] auto* frame_bytes=reinterpret_cast<std::byte*>(&frame);\n}\n";
if(CppReadability.FrameAlias(alias).Contains("frame_bytes"))throw new Exception("Unused pure frame alias remains");
if(!CppReadability.FrameAlias(alias.Replace("\n}","\n use(frame_bytes);\n}")).Contains("auto* frame_bytes"))throw new Exception("Used frame alias removed");
string opaque="return opaque<T>((n),(n+1));";
if(CppReadability.Statement(opaque)!=opaque)throw new Exception("Unknown template call rewritten");
Console.WriteLine("int main(){u8 data[64]{};for(std::uint64_t n=0;n<8;++n){");
foreach(var(name,row) in rows)foreach(string mode in new[]{"before_","after_"})Console.WriteLine("trace=0;if("+mode+name+"(n,data)!=std::uint64_t("+row.Expected+")||trace!=std::uint64_t("+row.Trace+")){std::fprintf(stderr,\""+mode+name+"\\n\");return 1;}");
Console.WriteLine("}}");
'''

class ReadabilityTests(pipeline.PipelineCase):
    def test_declaration_cleanup_preserves_cv_observation_and_effects(self):
        project=self.root/'Harness.csproj'
        project.write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env={**os.environ,'DOTNET_NOLOGO':'1'})
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode,0,emitted.stdout+emitted.stderr)
        self.compile_run(emitted.stdout)
