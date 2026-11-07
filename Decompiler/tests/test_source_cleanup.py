"""Compile source-tree rewrites against unmodified trees and explicit oracles."""
import html
import os

import test_pipeline as pipeline
import test_values as values


HARNESS = r'''
using SharpRetro.Decompiler;
Console.WriteLine("#include <cstring>");
CppSequence S(params CppStatement[] body) => new(body);
CppLine L(string text,string? declares=null) => new(text,declares);
CppLine T(string text) => new(text,IsEdgeTransfer:true);
CppScope Scope(params CppStatement[] body) => new(S(body));
CppIf If(string condition,CppSequence yes,CppSequence no) => new(new(condition),yes,no);
var cases=new Dictionary<string,(CppSequence Body,string Expected)> {
    ["shadow"]=(S(L("int value=9;","value"),L("int result=0;","result"),
        Scope(L("int value=3;","value"),L("result+=value;")),L("result+=value;"),L("return result;")),"12"),
    ["siblings"]=(S(L("int result=0;","result"),
        Scope(L("int value=1;","value"),L("result+=value;")),
        Scope(L("int value=2;","value"),L("result+=value;")),L("return result;")),"3"),
    ["guard_shadow"]=(S(L("int result=0;","result"),
        If("choose",S(L("int value=5;","value"),L("result+=value;"),L("return result;")),
            S(L("int value=7;","value"),L("result+=value;"))),
        L("int value=9;","value"),L("result+=value;"),L("return result;")),"choose ? 5 : 16"),
    ["effects"]=(S(L("int trace=0;","trace"),
        Scope(L("trace=trace*10+1;"),If("choose",S(),S(L("trace=trace*10+2;")))),
        If("choose",S(L("trace=trace*10+3;"),L("return trace;")),S(L("trace=trace*10+4;"))),
        L("return trace;")),"choose ? 13 : 124"),
    ["loop"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("true",S(Scope(L("++i;")),If("i==2",S(L("continue;")),S()),
            If("i==5",S(L("break;")),S(L("result+=i;"))))),L("return result;")),"8"),
    ["empty"]=(S(L("int result=7;","result"),If("choose",S(),S()),L("return result;")),"7"),
    ["chain"]=(S(L("int result=0;","result"),If("choose",S(L("result=1;")),
        S(If("other",S(L("result=2;")),S(L("result=3;"))))),L("return result;")),"choose ? 1 : other ? 2 : 3"),
    ["shared_effect"]=(S(L("int result=0;","result"),If("choose",
        S(L("result=1;"),L("result=result*10+3;"),L("return result;")),
        S(L("result=2;"),L("result=result*10+3;"),L("return result;")))),"choose ? 13 : 23"),
    ["local_tail"]=(S(If("choose",
        S(L("int value=5;","value"),L("return value;")),
        S(L("int value=7;","value"),L("return value;")))),"choose ? 5 : 7"),
    ["nested_local"]=(S(If("choose",
        S(Scope(L("int value=5;","value"),L("return value;"))),
        S(Scope(L("int value=7;","value"),L("return value;"))))),"choose ? 5 : 7"),
    ["early_tail"]=(S(L("int result=0;","result"),If("choose",
        S(If("other",S(L("return 9;")),S()),L("result=1;"),L("result+=3;"),L("return result;")),
        S(L("result=2;"),L("result+=3;"),L("return result;")))),"choose ? other ? 9 : 4 : 5"),
    ["shared_loop_edges"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("true",S(L("++i;"),If("i==4",
            S(If("choose",S(L("result+=10;"),L("break;")),S(L("result+=20;"),L("break;")))),S()),
            If("choose",S(L("result+=1;"),L("continue;")),S(L("result+=2;"),L("continue;"))))),
        L("return result;")),"choose ? 13 : 26"),
    ["tail_guards"]=(S(L("int result=1;","result"),
        If("choose",S(L("result+=2;"),If("other",S(L("result+=4;")),S())),S()),
        L("return result;")),"choose ? other ? 7 : 3 : 1"),
    ["effect_after_guard"]=(S(L("int result=1;","result"),
        If("choose",S(If("other",S(L("result+=4;")),S())),S()),
        L("result+=8;"),L("return result;")),"choose && other ? 13 : 9"),
    ["ancestor_exit_shadow"]=(S(L("int value=1;","value"),
        If("choose",S(If("other",S(L("[[maybe_unused]] int value=2;","value"),
            If("choose",S(L("++value;")),S())),S()),L("return value;")),S()),
        L("return value;")),"1"),
    ["ancestor_use_shadow"]=(S(L("int value=1;","value"),L("int result=0;","result"),
        If("choose",S(Scope(L("int value=2;","value"),L("result+=value;")),
            L("result+=value;")),S()),L("return result;")),"choose ? 3 : 0"),
    ["nested_loop_targets"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("++i<=3",S(If("choose",S(If("other",S(
            new CppWhile("true",S(L("result+=2;"),L("break;"))),L("result+=4;")),S())),S()),
            L("continue;"))),L("return result;")),"choose && other ? 18 : 0"),
    ["overwritten_condition"]=(S(L("int result=0;","result"),L("bool branch_condition=choose;","branch_condition"),
        If("branch_condition",S(L("branch_condition=false;"),
            If("other",S(L("result=7;")),S())),S()),L("return result;")),"choose && other ? 7 : 0"),
    ["guarded_fault"]=(S(L("int value=3;","value"),L("int* ptr=choose ? &value : nullptr;","ptr"),
        If("choose",S(If("other",S(L("*ptr+=4;")),S())),S()),L("return value;")),"choose && other ? 7 : 3"),
    ["post_test"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("bool again=false;","again"),
        new CppWhile("true",S(L("result+=++i;"),L("again=i<(choose ? 3 : 2);"),
            If("again",S(L("continue;")),S()),L("break;"))),L("return result;")),"choose ? 6 : 3"),
    ["post_test_early_continue"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("bool again=false;","again"),
        new CppWhile("true",S(L("++i;"),If("i==1",S(L("continue;")),S()),
            L("result+=i;"),L("again=i<3;"),If("again",S(L("continue;")),S()),L("break;"))),
        L("return result;")),"5"),
    ["post_test_local_condition"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("true",S(L("result+=++i;"),L("bool again=i<3;","again"),
            If("again",S(L("continue;")),S()),L("break;"))),L("return result;")),"6"),
    ["post_test_nested_continue"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("bool again=false;","again"),
        new CppWhile("true",S(L("int j=0;","j"),
            new CppWhile("++j<=2",S(L("++result;"),L("continue;"))),
            L("again=++i<3;"),If("again",S(L("continue;")),S()),L("break;"))),
        L("return result;")),"6"),
    ["large_else_guard"]=(S(L("int result=0;","result"),If("choose",S(L("result=7;")),
        S(Enumerable.Repeat<CppStatement>(L("++result;"),12).ToArray())),Scope(L("return result;"))),"choose ? 7 : 12"),
    ["large_then_guard"]=(S(L("int result=0;","result"),If("choose",
        S(Enumerable.Repeat<CppStatement>(L("++result;"),12).ToArray()),S(L("result=7;"))),
        L("return result;")),"choose ? 12 : 7"),
    ["large_arm_exit_shadow"]=(S(L("int value=1;","value"),If("choose",S(L("[[maybe_unused]] int value=7;","value")),
        S(Enumerable.Repeat<CppStatement>(L("++value;"),12).ToArray())),Scope(L("return value;"))),"choose ? 1 : 13"),
    ["short_circuit_guard"]=(S(L("int value=3;","value"),L("int* ptr=choose ? &value : nullptr;","ptr"),
        If("choose",S(If("*ptr==3 && other",S(L("value=7;")),S())),S()),L("return value;")),"choose && other ? 7 : 3"),
    ["short_arm_early_return"]=(S(L("int result=0;","result"),
        If("choose",S(If("other",S(L("return 9;")),S()),L("result=7;")),
            S(Enumerable.Repeat<CppStatement>(L("++result;"),12).ToArray())),L("return result;")),"choose ? other ? 9 : 7 : 12"),
    ["long_arm_exit_shadow"]=(S(L("int value=1;","value"),If("choose",S(L("value=3;")),
        S(new CppStatement[] { L("[[maybe_unused]] int value=7;","value") }.Concat(
            Enumerable.Repeat<CppStatement>(L("++value;"),12)).ToArray())),L("return value;")),"choose ? 3 : 1"),
    ["short_arm_continue"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("++i<=3",S(If("choose",S(L("++result;")),
            S(Enumerable.Repeat<CppStatement>(L("++result;"),12).ToArray())),L("continue;"))),
        L("return result;")),"choose ? 3 : 36"),
    ["short_arm_break"]=(S(L("int result=0;","result"),new CppWhile("true",S(
        If("choose",S(L("result=7;")),S(Enumerable.Repeat<CppStatement>(L("++result;"),12).ToArray())),
        L("break;"))),L("return result;")),"choose ? 7 : 12"),
    ["composed_guard_fault"]=(S(L("int value=3;","value"),L("int* ptr=choose ? &value : nullptr;","ptr"),
        If("choose",S(If("*ptr==3 && other",S(L("value=7;")),S())),S()),
        L("value+=10;"),L("return value;")),"choose && other ? 17 : 13"),
    ["tail_transfer_guard"]=(S(L("int result=0;","result"),L("int destination=0;","destination"),
        If("choose",S(L("result+=1;"),If("other",S(L("result+=2;")),S())),S()),
        T("destination = result;"),L("return destination;")),"choose ? other ? 3 : 1 : 0"),
    ["post_test_transfer"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("bool again=false;","again"),
        new CppWhile("true",S(L("++result;"),L("again=i<2;"),
            If("again",S(T("i = i+1;"),L("continue;")),S()),L("break;"))),L("return result;")),"3"),
    ["post_test_transfer_changes_condition"]=(S(L("int i=0;","i"),L("bool again=false;","again"),
        new CppWhile("true",S(L("again=++i<3;"),
            If("again",S(T("again = false;"),L("continue;")),S()),L("break;"))),L("return i;")),"3"),
    ["post_test_transfer_changes_operand"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("true",S(L("++result;"),
            If("i<2",S(T("i = i+1;"),L("continue;")),S()),L("break;"))),L("return result;")),"3"),
    ["dead_loop_copy"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("int next=0;","next"),L("bool again=false;","again"),
        new CppDoWhile(new("again"),S(L("++result;"),L("next=i+1;"),L("again=next<3;"),
            If("again",S(T("i = next;")),S()))),L("return result;")),"3"),
    ["live_loop_copy"]=(S(L("int i=0;","i"),L("bool again=false;","again"),
        new CppDoWhile(new("again"),S(L("again=i<2;"),If("again",S(T("i = i+1;")),S()))),L("return i;")),"2"),
    ["uninitialized_copy_source"]=(S(L("int i=0;","i"),L("[[maybe_unused]] int carrier=0;","carrier"),L("int value;","value"),L("bool again=false;","again"),
        new CppDoWhile(new("again"),S(L("again=choose && i++<1;"),
            If("again",S(L("value=42;")),S()),If("again",S(T("carrier = value;")),S()))),L("return i;")),"choose ? 2 : 0"),
    ["outer_iteration_observes_copy"]=(S(L("int i=0;","i"),L("int result=0;","result"),L("int carrier=3;","carrier"),
        L("int next=0;","next"),L("bool again=false;","again"),new CppWhile("i++<2",S(L("result+=carrier;"),L("next=carrier+1;"),L("again=false;"),
            new CppDoWhile(new("again"),S(If("again",S(T("carrier = next;")),S()))))),L("return result;")),"6"),
    ["indirect_observer_keeps_copy"]=(S(L("int carrier=3;","carrier"),L("int* observer=&carrier;","observer"),L("bool again=false;","again"),
        new CppDoWhile(new("again"),S(If("again",S(T("carrier = 7;")),S()))),L("return *observer;")),"3"),
    ["early_break_skips_copy"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("int next=0;","next"),L("bool again=false;","again"),
        new CppDoWhile(new("again"),S(L("++result;"),If("choose",S(L("break;")),S()),L("next=i+1;"),L("again=next<3;"),
            If("again",S(T("i = next;")),S()))),L("return result;")),"choose ? 1 : 3"),
    ["untagged_tail_stays_guarded"]=(S(L("int carrier=3;","carrier"),L("bool again=false;","again"),
        new CppDoWhile(new("again"),S(If("again",S(L("carrier = 7;")),S()))),L("return carrier;")),"3"),
    ["ordered_loop_swap"]=(S(L("int result=0;","result"),L("int i=0;","i"),L("int a=1;","a"),L("int b=2;","b"),
        L("int saved=0;","saved"),L("bool again=true;","again"),new CppDoWhile(new("again"),S(L("result+=a*10+b;"),L("again=i++<1;"),
            If("again",S(T("saved = a;"),T("a = b;"),T("b = saved;")),S()))),L("return result;")),"33"),
    ["trailing_continue"]=(S(L("int result=0;","result"),L("int i=0;","i"),
        new CppWhile("++i<=3",S(new CppWhile("true",S(L("result+=1;"),L("break;"))),
            If("choose",S(L("result+=10;"),L("continue;")),S()),L("result+=20;"),L("continue;"))),
        L("return result;")),"choose ? 33 : 63")
};
foreach(var (name,item) in cases) {
    foreach(string mode in new[] {"before","after","twice"}) {
        Console.WriteLine($"int {mode}_{name}(bool choose,bool other) {{");
        var carriers=new HashSet<string> {"result","i","j","value","again","destination","carrier","next","a","b","saved"};
        var once=CppLoopCopies.Simplify(CppSourceTree.Clean(item.Body),carriers);
        var twice=CppLoopCopies.Simplify(CppSourceTree.Clean(once),carriers);
        CppSourceTree.Emit(mode=="before" ? item.Body : mode=="after" ? once : twice,(text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
        Console.WriteLine("}");
    }
}
var opaque=S(L("variable_999 = 3;"),L("int replacement=42;","replacement"),
    L("std::memcpy(&variable_999,&replacement,sizeof(variable_999));"),L("return variable_999;"));
Console.WriteLine("int after_opaque_write() {");
CppSourceTree.Emit(CppVariableScopes.Place(opaque,new Dictionary<string,string> {{"variable_999","int"}}),
    (text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
Console.WriteLine("}");
var sharedIf=If("choose",S(L("variable_998 = 3;")),S());
Console.WriteLine("int after_repeated_occurrence(bool choose) {");
CppSourceTree.Emit(CppVariableScopes.Place(S(sharedIf,sharedIf,L("return choose ? variable_998 : 7;")),
    new Dictionary<string,string> {{"variable_998","int"}}),(text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
Console.WriteLine("}");
Console.WriteLine("int after_deferred_declaration(bool choose) {");
CppSourceTree.Emit(CppVariableScopes.Place(S(L("int result=1;","result"),L("result+=2;"),L("variable_997 = 7;"),
    If("choose",S(L("variable_997 = 9;")),S()),L("return result+variable_997;")),new Dictionary<string,string> {{"variable_997","int"}}),
    (text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
Console.WriteLine("}");
Console.WriteLine("int after_mutating_operators() {");
CppSourceTree.Emit(CppVariableScopes.Place(S(L("variable_996 = 5;"),L("++variable_996;"),L("variable_996+=3;"),L("*(&(variable_996)) = 9;"),L("return variable_996;")),
    new Dictionary<string,string> {{"variable_996","int"}}),(text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
Console.WriteLine("}");
Console.WriteLine("int after_parenthesized_address() {");
CppSourceTree.Emit(CppVariableScopes.Place(S(L("variable_994 = 1;"),L("*(&(variable_994)) = 2;"),L("return variable_994;")),
    new Dictionary<string,string> {{"variable_994","int"}}),(text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
Console.WriteLine("}");
Console.WriteLine("int after_reference_alias() {");
CppSourceTree.Emit(CppVariableScopes.Place(S(L("variable_993 = 1;"),L("int& alias=variable_993;","alias"),L("alias=2;"),L("return variable_993;")),
    new Dictionary<string,string> {{"variable_993","int"}}),(text,depth)=>Console.WriteLine(new string(' ',4+depth*4)+text));
Console.WriteLine("}");
// A discarded comparison is a use, never an assignment eligible for folding.
var comparison=CppVariableScopes.Place(S(L("variable_995 == 9;"),L("return 1;")),
    new Dictionary<string,string> {{"variable_995","int"}});
if(comparison.Body.OfType<CppLine>().Any(l=>l.Text.Contains("= =")) ||
   !comparison.Body.OfType<CppLine>().Any(l=>l.Text=="variable_995 == 9;"))
    throw new Exception("Comparison became an initializer");
Console.WriteLine("int main() { if(after_opaque_write()!=42 || after_repeated_occurrence(false)!=7 || after_repeated_occurrence(true)!=3 || after_deferred_declaration(false)!=10 || after_deferred_declaration(true)!=12 || after_mutating_operators()!=9 || after_parenthesized_address()!=2 || after_reference_alias()!=2) return 2; for(int choose=0;choose<2;++choose) for(int other=0;other<2;++other) {");
foreach(var (name,item) in cases)
    Console.WriteLine($"if(before_{name}(choose,other)!=after_{name}(choose,other) || after_{name}(choose,other)!=twice_{name}(choose,other) || after_{name}(choose,other)!=({item.Expected})) return 1;");
Console.WriteLine("} }");
'''


class SourceCleanupTests(pipeline.PipelineCase):
    execute=values.ValueRecoveryTests.execute

    def test_scope_rewrites_preserve_shadowing_effects_guards_and_loop_edges(self):
        project=self.root/"Harness.csproj"
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup>
</Project>''')
        (self.root/"Program.cs").write_text(HARNESS)
        # This framework-only harness needs neither package feeds nor telemetry.
        (self.root/"NuGet.Config").write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        local_env={**os.environ,"DOTNET_CLI_TELEMETRY_OPTOUT":"1","DOTNET_NOLOGO":"1"}
        built=pipeline.run("dotnet","build",project,"-c","Release","--verbosity","quiet",env=local_env)
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run("dotnet",self.root/"bin/Release/net10.0/Harness.dll")
        self.assertEqual(emitted.returncode,0,emitted.stderr)
        # This compilation exercises both old/new lexical scopes, and executes
        # independent expected results for all input pairs under ASan/UBSan.
        self.compile_run(emitted.stdout)
        self.assertIn("} else if(other)",emitted.stdout)
        clean_empty=emitted.stdout.split("int after_empty(",1)[1].split("int twice_empty(",1)[0]
        self.assertNotIn("if(",clean_empty)
        shared=emitted.stdout.split("int after_shared_effect(",1)[1].split("int twice_shared_effect(",1)[0]
        self.assertEqual(shared.count("result=result*10+3;"),1)
        self.assertEqual(shared.count("return result;"),1)
        local=emitted.stdout.split("int after_local_tail(",1)[1].split("int twice_local_tail(",1)[0]
        self.assertEqual(local.count("return value;"),2)
        loop=emitted.stdout.split("int after_trailing_continue(",1)[1].split("int twice_trailing_continue(",1)[0]
        self.assertEqual(loop.count("continue;"),1)
        self.assertEqual(loop.count("break;"),1)
        def after(name):
            return emitted.stdout.split(f"int after_{name}(",1)[1].split("\nint ",1)[0]
        self.assertIn("if(!(choose))",after("tail_guards"))
        self.assertIn("do {",after("post_test"))
        self.assertNotIn("do {",after("post_test_early_continue"))
        self.assertNotIn("do {",after("post_test_local_condition"))
        self.assertIn("do {",after("post_test_nested_continue"))
        self.assertNotIn("else",after("large_else_guard"))
        self.assertNotIn("else",after("large_then_guard"))
        self.assertIn("else",after("large_arm_exit_shadow"))
        self.assertIn("if((choose) && (*ptr==3 && other))",after("composed_guard_fault"))
        deferred=after("deferred_declaration")
        self.assertLess(deferred.index("result+=2;"),deferred.index("int variable_997"))
        self.assertIn("int variable_997 = 7;",deferred)
        self.assertIn("do {",after("post_test_transfer"))
        self.assertNotIn("do {",after("post_test_transfer_changes_condition"))
        self.assertNotIn("do {",after("post_test_transfer_changes_operand"))
        self.assertNotIn("if(",after("dead_loop_copy"))
        self.assertNotIn("if(",after("ordered_loop_swap"))
        for name in ("live_loop_copy","uninitialized_copy_source","outer_iteration_observes_copy","indirect_observer_keeps_copy","untagged_tail_stays_guarded"):
            self.assertIn("if(",after(name),name)

    def test_recovered_execution_can_exceed_the_previous_instruction_budget(self):
        # Two loop instructions per iteration already exceed the former 100M
        # ceiling. The sum is defined modulo 2^32 and Clang can optimize it.
        code=bytes.fromhex("b901e1f505 31c0 83c001 ffc9 75f9 c3")
        self.image.write_bytes(pipeline.macho(code))
        source=self.root/"values.cpp"
        for representation in ("recovered","blocks"):
            result=self.cli("decompile-app",self.image,"--representation",representation,"--output",source)
            self.assertEqual(result.returncode,0,result.stderr)
            self.assertNotIn("instruction_budget",source.read_text())
            executed=self.execute(source)
            self.assertEqual((executed.returncode,executed.stderr),(1,""))
