"""Generated native functions are callable with their recovered inputs alone."""
import test_pipeline as pipeline
import test_values as values
from test_functions import Code


class DirectInputTests(pipeline.PipelineCase):
    driver=values.ValueRecoveryTests.driver
    execute=values.ValueRecoveryTests.execute

    def recover(self, code):
        self.image.write_bytes(pipeline.macho(code))
        source=self.root/"values.cpp"
        result=self.cli("decompile-app",self.image,"--memory","native","--output",source)
        self.assertEqual(result.returncode,0,result.stderr)
        text=source.read_text()
        for removed in ("NativeCallContext","CallFrame","call.depth","call.frame_bytes","thread_local"):
            self.assertNotIn(removed,text)
        self.assertNotRegex(text,r"std::uint64_t\s+local_\d+\s*=\s*argument_\d+")
        return source

    def test_scalar_parameter_is_mutable_direct_input(self):
        code=Code().call("helper").emit("c3").label("helper").emit("83c707 89f8 c3")
        source=self.recover(code.finish())
        helper=f"fn_{pipeline.BASE+code.labels['helper']:x}"
        self.assertIn(f"{helper}(std::uint64_t argument_0)",source.read_text())
        self.driver(source,f"""
int main() {{
  for(unsigned n=0;n<1024;++n)
    if(sharp_retro::source::{helper}(n)!=n+7) return 1;
}}
""")

    def test_no_input_function_is_a_normal_nullary_call(self):
        source=self.recover(bytes.fromhex("b82a000000c3"))
        self.assertIn("fn_1000()",source.read_text())
        self.driver(source,"""
int main() { return sharp_retro::source::fn_1000()==42 ? 0 : 1; }
""")

    def test_void_pointer_input_needs_no_adapter_context(self):
        code=(Code().emit("4883ec08 4889e7").call("store")
              .emit("0fb60424 4883c408 c3").label("store").emit("c6072a c3"))
        source=self.recover(code.finish())
        helper=f"fn_{pipeline.BASE+code.labels['store']:x}"
        self.assertIn(f"static void {helper}(u8* argument_0)",source.read_text())
        self.driver(source,f"""
int main() {{
  unsigned char result=0;
  sharp_retro::source::{helper}(reinterpret_cast<std::uintptr_t>(&result));
  return result==42 ? 0 : 1;
}}
""")

    def test_recursive_inputs_exceed_the_old_context_depth_cap(self):
        code=(Code().call("sum").emit("c3").label("sum").emit("85ff").branch("74","base")
              .emit("4883ec08 893c24 ffcf").call("sum")
              .emit("030424 4883c408 c3").label("base").emit("31c0 c3"))
        source=self.recover(code.finish())
        self.driver(source,"""
int main() {
  for(unsigned n : {513u,1024u,2048u})
    if(sharp_retro::source::recovered_program(n,nullptr)!=int((n*(n+1)/2)&255)) return 1;
}
""")
