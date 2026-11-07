"""Optimization/source-memory checks; synthetic domains are separate from app evidence."""
import test_pipeline as pipeline


class ValueRecoveryTests(pipeline.PipelineCase):
    def recover(self, code, memory="auto"):
        self.image.write_bytes(pipeline.macho(bytes.fromhex(code)))
        source = self.root / "values.cpp"
        result = self.cli("decompile-app", self.image, "--representation", "recovered", "--memory", memory, "--output", source)
        self.assertEqual(result.returncode, 0, result.stderr)
        return source

    def execute(self, source, args=()):
        exe = self.root / "values-arm64"
        built = pipeline.run("clang++", "-std=c++20", "-O2", "-fsanitize=address,undefined", source, "-o", exe)
        self.assertEqual(built.returncode, 0, built.stderr)
        return pipeline.run(exe, *args)

    def driver(self, source, body):
        driver = self.root / "driver.cpp"
        driver.write_text('#define main recovered_cli_main\n#include "values.cpp"\n#undef main\n' + body)
        result = self.execute(driver)
        self.assertEqual(result.returncode, 0, result.stderr)

    def test_signed_subtraction_conditions_exhaustive_byte_domain(self):
        # AL = argc.low8; SIL = argc.bits[15:8]; signed comparison, then zero-extend.
        for opcode, predicate in (("9c", "a < b"), ("9d", "a >= b")):
            with self.subTest(predicate=predicate):
                source = self.recover("89f8 89fe c1ee08 4038f0 0f" + opcode + "c0 0fb6c0 c3")
                self.driver(source, f"""
int main() {{
  for(unsigned i=0;i<65536;++i) {{
    auto a=std::bit_cast<std::int8_t>(std::uint8_t(i));
    auto b=std::bit_cast<std::int8_t>(std::uint8_t(i>>8));
    if(sharp_retro::source::recovered_program(int(i),nullptr)!=int({predicate})) return 1;
  }}
}}
""")

    def test_partial_registers_and_32bit_zero_extension(self):
        source = self.recover("48b8ffffffffffffffff 89f8 b4a5 4889c1 48c1e908 48c1e820 31c8 c3")
        self.driver(source, """
int main() {
  for(int argc=0;argc<256;++argc)
    if(sharp_retro::source::recovered_program(argc,nullptr)!=0xa5) return 1;
}
""")

    def test_signed_condition_recovery_at_32_and_64bit_boundaries(self):
        for code, expected in (("beefffffff 39f7 0f9cc0 0fb6c0 c3", "value < -17"),
                               ("4863c7 48c1e020 48be0000000001000000 4839f0 0f9cc0 0fb6c0 c3", "value < 1")):
            with self.subTest(width_code=code):
                source = self.recover(code)
                self.driver(source, f"""
int main() {{
  int cases[]={{INT_MIN,INT_MIN+1,-18,-17,-16,-1,0,1,2,INT_MAX-1,INT_MAX}};
  for(int value:cases)
    if(sharp_retro::source::recovered_program(value,nullptr)!=int({expected})) return 1;
}}
""")

    def test_native_unaligned_access_and_byte_aliases(self):
        source = self.recover("4883ec20 48b88877665544332211 4889442401 c6442402aa 488b442401 48c1e808 4883c420 c3")
        result = self.execute(source)
        self.assertEqual((result.returncode, result.stderr), (0xaa, ""))

    def test_loads_remain_ordered_across_overlapping_store(self):
        source = self.recover("4883ec10 c6042411 0fb60424 c6042422 0fb60c24 31c8 4883c410 c3")
        result = self.execute(source)
        self.assertEqual((result.returncode, result.stderr), (0x33, ""))

    def test_parallel_edge_copies_preserve_swapped_values(self):
        source = self.recover("4889f8 4889f7 4889c6 eb00 4889f8 4831f0 c3")
        self.driver(source, """
int main() {
  char empty[]=""; std::array<char*,9> argv{};
  for(auto& p:argv) p=empty;
  for(int count=1;count<8;++count) {
    auto expected=(reinterpret_cast<std::uintptr_t>(argv.data()) ^ std::uint64_t(count)) & 255;
    if(sharp_retro::source::recovered_program(count,argv.data())!=int(expected)) return 1;
  }
}
""")

    def test_undefined_flag_reads_are_not_optimized_away(self):
        source = self.recover("0fafc79fc3")
        result = self.execute(source)
        self.assertEqual(result.returncode, 125)
        self.assertIn("undefined flags", result.stderr)

    def test_readonly_native_storage_remains_protected(self):
        source = self.recover("31c0c3", memory="checked")
        self.driver(source, """
int main() {
  using namespace sharp_retro::source;
  static const char text[]="immutable";
  accessible.clear(); expose(text,sizeof(text),false);
  try { write_memory<std::uint8_t>(address(text),1); return 1; }
  catch(const std::runtime_error&) {}
  return text[0]=='i' ? 0 : 2;
}
""")

    def test_unsupported_nested_instruction_control_refuses_cleanly(self):
        self.image.write_bytes(pipeline.macho(bytes.fromhex("48d3e0c3")))
        output = self.root / "must-not-exist.cpp"
        result = self.cli("decompile-app", self.image, "--representation", "recovered", "--output", output)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Variable instruction-internal control", result.stderr)
        self.assertFalse(output.exists())
        result = self.cli("decompile-app", self.image, "--representation", "machine", "--output", output)
        self.assertEqual(result.returncode, 0, result.stderr)
        executed = self.execute(output)
        self.assertEqual((executed.returncode, executed.stderr), (0, ""))
