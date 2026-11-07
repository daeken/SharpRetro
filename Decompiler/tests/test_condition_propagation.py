"""Predicate snapshot propagation across real structured CFG paths and scopes."""
import html
import os

import test_pipeline as pipeline

HARNESS = r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] b)=>new(b);
CppLine L(string s,string? d=null)=>new(s,d);
CppLine C(string rhs)=>new("branch_condition="+rhs+";",IsConditionDefinition:true);
CppIf I(string c,CppSequence yes,CppSequence? no=null)=>new(new(c),yes,no??S());
CppLine U(string name,string rhs)=>L("std::uint64_t "+name+"="+rhs+";",name);
var shared=C("a==1");
var rows=new Dictionary<string,(CppSequence Body,string Expected,int Removed)> {
 ["adjacent"]=(S(U("a","1"),C("a==1"),I("branch_condition",S(L("return 7;"))),L("return 2;")),"7",1),
 ["comment_parentheses"]=(S(U("a","1"),C("(a/*(*/) || (a)"),I("branch_condition",S(L("return 7;"))),L("return 2;")),"7",1),
 ["unrelated_write"]=(S(U("a","1"),U("b","2"),C("a==1"),L("b=9;"),I("branch_condition",S(L("return b;"))),L("return 0;")),"9",1),
 ["array_index_is_not_escape"]=(S(L("std::array<std::uint64_t,4> data{1,2,3,4};","data"),U("i","1"),C("i<3"),U("a","data[i]"),I("branch_condition",S(L("return a;"))),L("return 0;")),"2",1),
 ["clobbered_operand"]=(S(U("a","1"),C("a==1"),L("a=9;"),I("branch_condition",S(L("return a;"))),L("return 0;")),"9",0),
 ["compound_write"]=(S(U("a","1"),C("a==1"),L("++a;"),I("branch_condition",S(L("return a;"))),L("return 0;")),"2",0),
 ["shift_assignment"]=(S(U("a","1"),U("b","0"),C("a==1"),L("b=3; a<<=1;"),I("branch_condition",S(L("return a+b;"))),L("return 0;")),"5",0),
 ["parenthesized_assignment"]=(S(U("a","1"),U("b","0"),C("a==1"),L("b=3; (a)=9;"),I("branch_condition",S(L("return a+b;"))),L("return 0;")),"12",0),
 ["conditional_write"]=(S(U("a","1"),C("a==1"),I("choose",S(L("a=9;"))),I("branch_condition",S(L("return a;"))),L("return 0;")),"choose ? 9 : 1",0),
 ["join_definitions"]=(S(U("a","1"),I("choose",S(C("a==1")),S(C("a==0"))),I("branch_condition",S(L("return 7;"))),L("return 2;")),"choose ? 7 : 2",0),
 ["safe_posttest"]=(S(U("i","0"),new CppDoWhile(new("branch_condition"),S(L("++i;"),C("i<3"))),L("return i;")),"3",1),
 ["clobbered_posttest"]=(S(U("i","0"),new CppDoWhile(new("branch_condition"),S(L("++i;"),C("i<3"),L("i+=10;"))),L("return i;")),"22",0),
 ["conditional_backedge_write"]=(S(U("i","0"),new CppDoWhile(new("branch_condition"),S(L("++i;"),C("i<3"),I("branch_condition",S(L("i+=10;"))))),L("return i;")),"12",0),
 ["expired_binding"]=(S(new CppScope(S(U("a","1"),C("a==1"))),I("branch_condition",S(L("return 7;"))),L("return 2;")),"7",0),
 ["shadow_binding"]=(S(U("a","1"),U("result","0"),C("a==1"),new CppScope(S(U("a","0"),I("branch_condition",S(L("result=a+3;"))))),L("return result;")),"3",0),
 ["continue_scope_exit"]=(S(U("i","0"),new CppDoWhile(new("branch_condition"),S(U("a","++i"),C("a<3"),L("continue;"))),L("return i;")),"3",0),
 ["nested_break_scope_exit"]=(S(new CppWhile("true",S(U("a","1"),C("a==1"),L("break;"))),I("branch_condition",S(L("return 9;"))),L("return 0;")),"9",0),
 ["escaped_operand"]=(S(U("a","1"),L("auto* ptr=&(a);","ptr"),C("a==1"),L("*ptr=9;"),I("branch_condition",S(L("return a;"))),L("return 0;")),"9",0),
 ["reference_binding"]=(S(U("a","1"),L("std::uint64_t& alias=a;","alias"),C("a==1"),L("alias=9;"),I("branch_condition",S(L("return a;"))),L("return 0;")),"9",0),
 ["borrowed_reference"]=(S(U("a","1"),L("auto* ptr=borrow(a);","ptr"),C("a==1"),L("*ptr+=1;"),I("branch_condition",S(L("return a;"))),L("return 0;")),"2",0),
 ["call_barrier"]=(S(U("a","1"),C("a==1"),L("mutate(a);"),I("branch_condition",S(L("return a;"))),L("return 0;")),"9",0),
 ["raw_load"]=(S(U("a","1"),L("auto* ptr=&a;","ptr"),C("*ptr==1"),L("a=9;"),I("branch_condition",S(L("return a;"))),L("return 0;")),"9",0),
 ["effectful_dead_snapshot"]=(S(U("a","1"),C("mutate(a)"),L("return a;")),"9",0),
 ["embedded_bool_conversion"]=(S(C("2"),I("branch_condition==1",S(L("return 7;"))),L("return 0;")),"7",1),
 ["noncondition_read"]=(S(C("true"),L("bool saved=branch_condition;","saved"),L("return saved ? 7 : 0;")),"7",0),
 ["repeated_occurrence"]=(S(U("a","1"),U("result","0"),shared,I("branch_condition",S(L("++result;"))),L("a=2;"),shared,I("branch_condition",S(L("result+=10;"))),L("return result;")),"1",2),
 ["negated"]=(S(C("false"),new CppIf(new("branch_condition",true),S(L("return 7;")),S()),L("return 0;")),"7",1),
 ["typed_truncation"]=(S(U("a","0x100000000ULL"),C("static_cast<std::uint32_t>(a)==0"),I("branch_condition",S(L("return 7;"))),L("return 0;")),"7",1),
 ["unknown_control"]=(S(C("true"),L("goto done;"),L("done:"),I("branch_condition",S(L("return 7;"))),L("return 0;")),"7",0),
 ["escaped_flag"]=(S(C("true"),L("auto* ptr=&branch_condition;","ptr"),L("*ptr=false;"),I("branch_condition",S(L("return 7;"))),L("return 0;")),"0",0)
};
var member=S(C("true"),I("object.branch_condition",S(L("return 1;"))),L("return 0;"));
if(!ReferenceEquals(CppConditionPropagation.Reduce(member,new Dictionary<string,string>()).Body,member))
 throw new Exception("Member spelling captured as the compiler flag");
Console.WriteLine("#include <cstdint>\n#include <array>\n#include <initializer_list>\nbool mutate(std::uint64_t& a) { a=9;return true; }\nstd::uint64_t* borrow(std::uint64_t& a) { return &a; }");
foreach(var (name,row) in rows) {
 var result=CppConditionPropagation.Reduce(row.Body,new Dictionary<string,string>{{"choose","bool"}});
 if(result.RemovedDefinitions!=row.Removed)throw new Exception(name+" removed="+result.RemovedDefinitions+" expected="+row.Removed);
 Console.Error.WriteLine(name+" forwarded="+result.ForwardedUses+" removed="+result.RemovedDefinitions);
 foreach(bool clean in new[]{false,true}) {
  Console.WriteLine($"std::uint64_t {(clean?"after":"before")}_{name}(bool choose) {{");
  Console.WriteLine("[[maybe_unused]] bool branch_condition;");
  CppSourceTree.Emit(clean?result.Body:row.Body,(s,d)=>Console.WriteLine(new string(' ',4+d*4)+s));
  Console.WriteLine("}");
 }
}
Console.WriteLine("int main(){for(bool choose : {false,true}) {");
foreach(var (name,row) in rows)Console.WriteLine($"if(before_{name}(choose)!=after_{name}(choose)||after_{name}(choose)!=({row.Expected}))return 1;");
Console.WriteLine("}}");
'''

class ConditionPropagationTests(pipeline.PipelineCase):
    def test_snapshots_preserve_writes_joins_loops_scopes_and_bool_conversion(self):
        project=self.root/'Harness.csproj'
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
</PropertyGroup><ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>''')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        env={**os.environ,'DOTNET_CLI_TELEMETRY_OPTOUT':'1','DOTNET_NOLOGO':'1'}
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env=env)
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll',env=env)
        self.assertEqual(emitted.returncode,0,emitted.stderr)
        self.compile_run(emitted.stdout)
        def after(name): return emitted.stdout.split(f'after_{name}(',1)[1].split('\nstd::uint64_t ',1)[0]
        self.assertNotIn('branch_condition=',after('adjacent'))
        self.assertIn('branch_condition=',after('clobbered_posttest'))
        self.assertIn('branch_condition=',after('expired_binding'))
        self.assertIn('bool(2)',after('embedded_bool_conversion'))
