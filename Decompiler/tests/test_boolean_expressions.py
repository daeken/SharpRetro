"""Boolean rendering preserves integer widths, grouping, and short-circuit order."""
import html
import os
import test_pipeline as pipeline

HARNESS=r'''
using SharpRetro.Decompiler;
var rows=new Dictionary<string,string> {
 ["hint"]="(!(a==0xfULL)) && (!(b<=7ULL))",
 ["eq"]="!(a==b)",["ne"]="!(a!=b)",["lt"]="!(a<b)",["le"]="!(a<=b)",["gt"]="!(a>b)",["ge"]="!(a>=b)",
 ["demorgan_and"]="!((a<b) && (a!=b))",
 ["demorgan_or"]="!((a<b) || (a==b))",
 ["precedence"]="((a==0) || (b==0)) && (!(a<=b))",
 ["logic_groups"]="(a==0 && b==0) || a>b",
 ["bool_conversion"]="!!a==1",
 ["root_double_not"]="!!a",
 ["nested_bool_conversion"]="(!!a)==b",
 ["double_compare"]="!!(a==b)",
 ["bitwise_compare"]="!(a | (b==0))",
 ["bitwise_arithmetic"]="!((a+b) & (b^3ULL))",
 ["mixed_comparison"]="!((a==b)<(b==0))",
 ["nested_comparison"]="!(a<(b<a))",
 ["shift"]="!((a>>3)<=b)",
 ["wrap"]="!((a+1ULL)<b)",
 ["arithmetic_associativity"]="!(a-(b-1ULL)==b)",
 ["minus_minus"]="-(-a)==b",
 ["plus_plus"]="+(+a)==b",
 ["cast"]="!(static_cast<std::uint32_t>(a)<=b)",
 ["signed_cast"]="!(static_cast<std::int64_t>(a)<=static_cast<std::int64_t>(b))",
 ["nested_cast"]="!(static_cast<std::uint64_t>(static_cast<std::uint32_t>(a))<=b)",
 ["template_call"]="!(templated<std::uint64_t>(a)<=b)",
 ["character_add"]="a + 'c' + b",
 ["comment"]="(!(a/*note*/==0xfULL)) && (!(b<=7ULL))",
 ["opaque_ternary"]="a ? b : 0ULL"
};
Console.WriteLine("#include <cstdint>\n#include <initializer_list>\n#include <limits>\ntemplate<typename T>T templated(std::uint64_t a){return T(a);}\nint trace=0;bool first(bool a){trace=trace*10+1;return a;}bool second(bool a){trace=trace*10+2;return a;}");
foreach(var (name,expression) in rows) {
 string clean=CppBooleanExpressions.Format(expression,false,true);
 if(CppBooleanExpressions.Format(clean,false,true)!=clean)throw new Exception("Not idempotent: "+name);
 Console.Error.WriteLine(name+": "+clean);
 if(name=="hint" && clean!="a != 0xfULL && b > 7ULL")throw new Exception("Hint not simplified");
 if(name=="root_double_not" && clean!="a")throw new Exception("Boolean context kept double negation");
 if(name=="template_call" && clean!=expression)throw new Exception("Ambiguous template call changed");
 foreach(bool after in new[]{false,true})Console.WriteLine($"bool {(after?"after":"before")}_{name}(std::uint64_t a,std::uint64_t b){{return bool({(after?clean:expression)});}}");
}
foreach(var (name,expression,integers) in new[]{("float_le","!(a<=b)",false),("float_lt","!(a<b)",false),("float_ge","!(a>=b)",false),("float_gt","!(a>b)",false)}) {
 string clean=CppBooleanExpressions.Format(expression,false,integers);
 foreach(bool after in new[]{false,true})Console.WriteLine($"bool {(after?"after":"before")}_{name}(double a,double b){{return {(after?clean:expression)};}}");
}
string side="!((first(a)) && (second(b)))";
Console.WriteLine($"bool before_effects(bool a,bool b){{return {side};}}");
Console.WriteLine($"bool after_effects(bool a,bool b){{return {CppBooleanExpressions.Format(side,false,true)};}}");
if(CppBooleanExpressions.Format("a<=b",true,true)!="a > b")throw new Exception("Negated condition metadata ignored");
if(CppBooleanExpressions.Format("a<=b",true,false)!="!(a <= b)")throw new Exception("Floating comparison complemented");
Console.WriteLine("int main(){std::uint64_t values[]={0,1,2,3,7,8,15,16,0xffffffffULL,0x100000000ULL,0x8000000000000000ULL,0xffffffffffffffffULL};for(auto a:values){for(auto b:values){");
foreach(var name in rows.Keys)Console.WriteLine($"if(before_{name}(a,b)!=after_{name}(a,b))return 1;");
Console.WriteLine("} }\ndouble floats[]={0,-0.0,1,-1,std::numeric_limits<double>::infinity(),-std::numeric_limits<double>::infinity(),std::numeric_limits<double>::quiet_NaN()};for(double a:floats){for(double b:floats){");
foreach(string name in new[]{"float_le","float_lt","float_ge","float_gt"})Console.WriteLine($"if(before_{name}(a,b)!=after_{name}(a,b))return 2;");
Console.WriteLine("} }\nfor(bool a:{false,true}){for(bool b:{false,true}){trace=0;bool before=before_effects(a,b);int saved=trace;trace=0;if(before!=after_effects(a,b)||saved!=trace)return 3;}}}");
'''

class BooleanExpressionTests(pipeline.PipelineCase):
    def test_complements_grouping_conversions_and_unordered_float_comparisons(self):
        project=self.root/'Harness.csproj'
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType>
<TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>''')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        env={**os.environ,'DOTNET_CLI_TELEMETRY_OPTOUT':'1','DOTNET_NOLOGO':'1'}
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env=env)
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll',env=env)
        self.assertEqual(emitted.returncode,0,emitted.stderr)
        self.compile_run(emitted.stdout)
