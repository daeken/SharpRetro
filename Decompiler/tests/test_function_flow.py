"""Structured native functions: regions, calls, exit selection and fallback."""
import re
import test_pipeline as pipeline
import test_values as values
from test_functions import Code
from test_structured import fixtures


class FunctionFlowTests(pipeline.PipelineCase):
    execute = values.ValueRecoveryTests.execute
    driver = values.ValueRecoveryTests.driver

    def recover(self, code, style="structured"):
        self.image.write_bytes(pipeline.macho(code))
        source = self.root / "values.cpp"
        result = self.cli("decompile-app", self.image, "--control-flow", style, "--output", source)
        self.assertEqual(result.returncode, 0, result.stderr)
        return source

    def test_nested_regions_and_early_exits_against_source_oracles(self):
        for name, (code, syntax, expected, is_loop) in fixtures().items():
            if name == "divergent_path":
                continue
            with self.subTest(shape=name):
                # Derive both fixture inputs from argc, without native argv use.
                source = self.recover(bytes.fromhex("89fe c1ee04 83e70f") + code)
                text = source.read_text()
                self.assertIn("// Control flow: structured;", text)
                self.assertNotIn("goto ", text)
                bodies=text.split("// Recovered function bodies",1)[1].split("int recovered_program(",1)[0]
                self.assertEqual(bool(re.search(r"\b(?:while|for)\s*\(|\bdo\s*\{",bodies)),is_loop)
                self.driver(source, f"""
unsigned expected_break(unsigned n) {{
  unsigned sum=0;
  while(n) {{ --n; if(n==3) continue; if(n==1) break; sum+=n; }}
  return sum;
}}
int main() {{
  for(unsigned a=0;a<9;++a) for(unsigned b=0;b<6;++b) {{
    if({str(name == 'posttest_loop').lower()} && !b) continue;
    auto expected=({expected}) & 255;
    if(sharp_retro::source::recovered_program(int(a|(b<<4)),nullptr)!=int(expected)) return 1;
  }}
}}
""")

    def test_native_call_and_memory_effects_inside_loop(self):
        code = (Code().emit("4883ec08 c6042400 4889e6").label("head").emit("85ff")
                .branch("74", "done").call("increment").emit("ffcf").branch("eb", "head")
                .label("done").emit("0fb60424 4883c408 c3").label("increment").emit("fe06 c3"))
        source = self.recover(code.finish())
        text = source.read_text()
        self.assertIn("2 structured functions, 1 loops", text)
        self.assertNotIn("goto ", text)
        self.driver(source, """
int main() {
  for(int n=0;n<300;++n)
    if(sharp_retro::source::recovered_program(n,nullptr)!=(n&255)) return 1;
}
""")

    def test_join_search_preserves_bypassed_shared_effects_and_early_return(self):
        code=(Code().emit("31c0 85ff").branch("74","zero").emit("85f6").branch("74","shared")
              .emit("83c001 83ff03").branch("74","done").label("shared").emit("83c007")
              .label("done").emit("c3").label("zero").emit("b809000000 c3").finish())
        source=self.recover(bytes.fromhex("89fe c1ee04 83e70f")+code)
        self.driver(source,"""
int main() {
  for(unsigned a=0;a<9;++a) for(unsigned b=0;b<6;++b) {
    unsigned expected=!a ? 9 : a==3 && b ? 1 : 7+(b!=0);
    if(sharp_retro::source::recovered_program(int(a|(b<<4)),nullptr)!=int(expected)) return 1;
  }
}
""")

    def test_join_search_keeps_nested_loop_exit_destinations(self):
        code=(Code().emit("31c0 89f9").label("outer").emit("85c9").branch("74","done")
              .emit("89f2").label("inner").emit("85d2").branch("74","next_outer")
              .emit("83fa02").branch("74","escaped").emit("01c8 ffca").branch("eb","inner")
              .label("next_outer").emit("ffc9").branch("eb","outer")
              .label("escaped").emit("83c016").label("done").emit("c3").finish())
        source=self.recover(bytes.fromhex("89fe c1ee04 83e70f")+code)
        self.driver(source,"""
int main() {
  for(unsigned a=0;a<9;++a) for(unsigned b=0;b<6;++b) {
    unsigned expected=!a || !b ? 0 : b==1 ? a*(a+1)/2 : 22+a*(b-2);
    if(sharp_retro::source::recovered_program(int(a|(b<<4)),nullptr)!=int(expected)) return 1;
  }
}
""")

    def test_multiple_loop_exits_keep_distinct_effects(self):
        code = (Code().emit("31c0 89f9").label("head").emit("85c9")
                .branch("74", "zero").emit("83f903").branch("74", "three")
                .emit("01c8 ffc9").branch("eb", "head")
                .label("zero").emit("83c00b").branch("eb", "done")
                .label("three").emit("83c016").label("done").emit("c3"))
        source = self.recover(code.finish())
        self.assertNotIn("loop_exit_", source.read_text())
        self.assertNotIn("goto ", source.read_text())
        self.driver(source, """
int main() {
  for(unsigned n=0;n<40;++n) {
    unsigned expected=0, count=n;
    while(count && count!=3) { expected+=count; --count; }
    expected+=count==3 ? 22 : 11;
    if(sharp_retro::source::recovered_program(n,nullptr)!=int(expected&255)) return 1;
  }
}
""")

    def test_irreducible_graph_keeps_complete_fallback_or_refuses_strict(self):
        code = (Code().emit("b82a000000 be03000000 85ff").branch("74", "b")
                .label("a").emit("ffce").branch("74", "done")
                .label("b").emit("ffcf").branch("75", "a").label("done").emit("c3"))
        source = self.recover(code.finish(), "auto")
        self.assertIn("basic-block fallback:", source.read_text())
        self.assertIn("goto block_", source.read_text())
        self.assertEqual(self.execute(source).returncode, 42)
        strict = self.root / "must-not-exist.cpp"
        result = self.cli("decompile-app", self.image, "--control-flow", "structured", "--output", strict)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Cannot structure fn_", result.stderr)
        self.assertFalse(strict.exists())

    def test_explicit_baseline_and_reversed_condition_control(self):
        code = (Code().emit("85ff").branch("74", "zero").emit("b807000000 c3")
                .label("zero").emit("b809000000 c3").finish())
        baseline = self.recover(code, "dispatch")
        self.assertIn("basic blocks requested", baseline.read_text())
        self.assertEqual(self.execute(baseline).returncode, 7)
        source = self.recover(code)
        self.assertEqual(self.execute(source).returncode, 7)
        text = source.read_text()
        # Mutation still reverses the actual recovered branch after predicate
        # forwarding; it must not depend on a saved-flag variable spelling.
        prefix,body=text.split("// Recovered function bodies",1)
        self.assertIn("if(",body)
        self.assertNotIn("branch_condition",body)
        start=body.index("if(")+3
        depth=1
        end=start
        while depth:
            if body[end]=='(': depth+=1
            elif body[end]==')': depth-=1
            end+=1
        body=body[:start]+"!("+body[start:end-1]+")"+body[end-1:]
        source.write_text(prefix+"// Recovered function bodies"+body)
        self.assertEqual(self.execute(source).returncode, 9)
