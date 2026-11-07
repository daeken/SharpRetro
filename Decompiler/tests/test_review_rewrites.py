"""Review-driven source simplifications retain arithmetic widths and effects."""
import re
import test_pipeline as pipeline
import test_values as values
from test_functions import Code


class ReviewRewriteTests(pipeline.PipelineCase):
    recover=values.ValueRecoveryTests.recover
    execute=values.ValueRecoveryTests.execute
    driver=values.ValueRecoveryTests.driver

    def test_boolean_comparison_inversion_and_double_complement(self):
        for complements,predicate,syntax in (("f5","n>=7",">=7ULL"),("f5f5","n<7","<7ULL")):
            with self.subTest(complements=complements):
                source=self.recover("83ff07 "+complements+" 0f92c0 0fb6c0 c3",memory="native")
                self.assertRegex(source.read_text(),re.escape(syntax).replace("7ULL",r"\s*7ULL"))
                self.driver(source,f"""
int main() {{
  for(std::uint32_t n : {{0u,1u,2u,6u,7u,8u,0x7fffffffu,0xffffffffu}})
    if(sharp_retro::source::fn_1000(n)!=unsigned({predicate})) return 1;
}}
""")

    def test_low_bit_boolean_truncation_is_not_a_truthiness_cast(self):
        source=self.recover("89f8 d1e8 0f92c0 0fb6c0 c3",memory="native")
        self.driver(source,"""
int main() {
  for(unsigned n=0;n<1024;++n)
    if(sharp_retro::source::fn_1000(n)!=(n&1)) return 1;
}
""")

    def test_boolean_bitwise_operators_remain_eager_and_exact(self):
        # SETLE combines SF xor OF with ZF; SETG complements that result.
        # Every signed byte pair exercises both outcomes of each flag.
        for opcode,predicate in (("9e","a<=b"),("9f","a>b")):
            with self.subTest(opcode=opcode):
                source=self.recover("89f8 89fe c1ee08 4038f0 0f"+opcode+"c0 0fb6c0 c3",memory="native")
                self.driver(source,f"""
int main() {{
  for(unsigned n=0;n<65536;++n) {{
    auto a=std::bit_cast<std::int8_t>(std::uint8_t(n));
    auto b=std::bit_cast<std::int8_t>(std::uint8_t(n>>8));
    if(sharp_retro::source::fn_1000(n)!=unsigned({predicate})) return 1;
  }}
}}
""")

    def test_inclusive_comparisons_use_identical_typed_operands(self):
        for opcode,signed in (("96",False),("9e",True)):
            with self.subTest(signed=signed):
                source=self.recover("89f8 89fe c1ee08 4038f0 0f"+opcode+"c0 0fb6c0 c3",memory="native")
                self.assertIn("<=",source.read_text().split("// Recovered function bodies",1)[1])
                cast="std::bit_cast<std::int8_t>" if signed else "std::uint8_t"
                self.driver(source,f"""
int main() {{
  for(unsigned n=0;n<65536;++n) {{
    auto a={cast}(std::uint8_t(n));
    auto b={cast}(std::uint8_t(n>>8));
    if(sharp_retro::source::fn_1000(n)!=unsigned(a<=b)) return 1;
  }}
}}
""")

    def test_different_comparison_operands_do_not_merge(self):
        source=self.recover("4038f7 0f92c0 4038d7 0f94c1 08c8 0fb6c0 c3",memory="native")
        self.driver(source,"""
int main() {
  for(unsigned a=0;a<16;++a) for(unsigned b=0;b<16;++b) for(unsigned c=0;c<16;++c)
    if(sharp_retro::source::fn_1000(a,b,c)!=unsigned((a<b)|(a==c))) return 1;
}
""")

    def test_ignored_call_result_keeps_memory_effects_and_checked_return(self):
        code=(Code().emit("4883ec08 48c7042400000000 4889e7").call("change").call("change")
              .emit("030424 4883c408 c3").label("change").emit("48ff07 8b07 c3"))
        for memory in ("native","checked"):
            with self.subTest(memory=memory):
                source=self.recover(code.finish().hex(),memory=memory)
                body=source.read_text().split("// Recovered function bodies",1)[1]
                self.assertEqual(body.count("const auto call_result_"),1)
                self.assertEqual(self.execute(source).returncode,4)
                if memory=="checked":
                    self.assertEqual(body.count("noncanonical recovered call return"),2)

    def test_only_observed_members_of_multiple_call_results_are_assigned(self):
        code=(Code().emit("4883ec08 48c7042400000000 4889e7").call("change").emit("89d6")
              .call("change").emit("01f0 030424 4883c408 c3")
              .label("change").emit("48ff07 8b07 8d500a c3"))
        for memory in ("native","checked"):
            with self.subTest(memory=memory):
                source=self.recover(code.finish().hex(),memory=memory)
                body=source.read_text().split("// Recovered function bodies",1)[1]
                self.assertEqual(len(re.findall(r"=\s*call_result_0\.local_2;",body)),1)
                self.assertNotRegex(body,r"=\s*call_result_0\.value;")
                self.assertEqual(len(re.findall(r"=\s*call_result_1\.value;",body)),1)
                self.assertNotRegex(body,r"=\s*call_result_1\.local_2;")
                self.assertEqual(self.execute(source).returncode,15)

    def test_unused_native_field_read_disappears_but_checked_read_remains(self):
        code="4883ec08 48c704242a000000 488b1c24 b807000000 4883c408 c3"
        native=self.recover(code,memory="native")
        body=native.read_text().split("// Recovered function bodies",1)[1]
        self.assertNotRegex(body,r"=\s*frame\.stack_local_8;")
        self.assertEqual(self.execute(native).returncode,7)
        checked=self.recover(code,memory="checked")
        body=checked.read_text().split("// Recovered function bodies",1)[1]
        self.assertGreaterEqual(body.count("read_memory<std::uint64_t>"),2)
        self.assertEqual(self.execute(checked).returncode,7)

    def test_live_native_field_snapshot_is_retained(self):
        source=self.recover("4883ec08 48893c24 488b0424 4883c408 c3",memory="native")
        self.assertIn("1 private stack cells promoted",source.read_text())
        self.assertNotIn("frame.stack_local_8",source.read_text())
        self.driver(source,"""
int main() {
  for(std::uint64_t n : {0ULL,42ULL,0xffffffffffffffffULL})
    if(sharp_retro::source::fn_1000(n)!=n) return 1;
}
""")

    def test_field_read_feeding_dead_division_fault_is_retained(self):
        source=self.recover("4883ec08 893c24 8b0c24 31d2 31c0 f7f1 b807000000 4883c408 c3",memory="native")
        self.assertIn("1 private stack cells promoted",source.read_text())
        self.assertIn("x86 fault vector 0",source.read_text())
        self.driver(source,"""
int main() {
  if(sharp_retro::source::fn_1000(1)!=7) return 1;
  try { sharp_retro::source::fn_1000(0); return 2; }
  catch(const std::runtime_error&) { return 0; }
}
""")

    def test_unused_generic_load_is_not_treated_as_a_native_field(self):
        code="31c0 488b18 b807000000 c3"
        native=self.recover(code,memory="native")
        body=native.read_text().split("// Recovered function bodies",1)[1]
        self.assertIn("reinterpret_cast<const u64*>",body)
        checked=self.recover(code,memory="checked")
        result=self.execute(checked)
        self.assertEqual(result.returncode,125)
        self.assertIn("outside recovered objects",result.stderr)

    def test_live_snapshot_before_overlapping_byte_write_is_retained(self):
        source=self.recover("4883ec08 48b88877665544332211 48890424 488b0424 c6442401aa 488b0c24 4831c8 4883c408 c3",memory="native")
        self.assertGreaterEqual(len(re.findall(r"=\s*frame\.stack_local_8;",source.read_text())),2)
        self.driver(source,"""
int main() { return sharp_retro::source::fn_1000()==0xdd00 ? 0 : 1; }
""")

    def test_post_call_field_read_observes_escaped_pointer_write(self):
        code=(Code().emit("4883ec10 48893c24 4889e7").call("change")
              .emit("488b0424 4883c410 c3").label("change").emit("48ff07 c3"))
        source=self.recover(code.finish().hex(),memory="native")
        # Exact-type return forwarding can load the escaped field directly in
        # the return. The field read must still follow the mutating call.
        body=source.read_text().split("// Recovered function bodies",1)[1]
        self.assertRegex(body,r"(?:=\s*|return\s+)frame\.stack_local_10;")
        self.assertLess(body.index("fn_1019("),body.index("frame.stack_local_10;"))
        self.driver(source,"""
int main() {
  for(std::uint64_t n : {0ULL,42ULL,0xffffffffffffffffULL})
    if(sharp_retro::source::fn_1000(n)!=n+1) return 1;
}
""")

    def test_clean_64bit_arithmetic_wraps_at_64_bits(self):
        source=self.recover("4889f8 48b9ffffffffffffffff 480fafc1 4883c007 c3",memory="native")
        self.driver(source,"""
int main() {
  std::uint64_t cases[]={0,1,7,0x8000000000000000ULL,0xffffffffffffffffULL};
  for(auto n:cases)
    if(sharp_retro::source::fn_1000(n)!=std::uint64_t(0)-n+7) return 1;
}
""")

    def test_negative_offsets_use_unsigned_subtraction_with_wraparound(self):
        source=self.recover("4889f8 4883c0e0 c3",memory="native")
        body=source.read_text().split("// Recovered function bodies",1)[1]
        self.assertRegex(body,r"-\s*0x20ULL")
        self.assertNotRegex(body,r"\+\s*0xffffffffffffffe0ULL")
        self.driver(source,"""
int main() {
  for(std::uint64_t n : {0ULL,1ULL,31ULL,32ULL,33ULL,0x8000000000000000ULL,0xffffffffffffffffULL})
    if(sharp_retro::source::fn_1000(n)!=std::uint64_t(n-32)) return 1;
}
""")

    def test_assignment_cleanup_preserves_nested_32bit_wrap_before_widening(self):
        source=self.recover("31c0 85ff 0f95c0 f7d8 c3",memory="native")
        self.driver(source,"""
int main() {
  for(std::uint64_t n : {0ULL,1ULL,0x80000000ULL,0xffffffffULL,0x100000000ULL}) {
    std::uint64_t expected=std::uint32_t(n) ? 0xffffffffULL : 0;
    if(sharp_retro::source::fn_1000(n)!=expected) return 1;
  }
}
""")

        source=self.recover("89f8 0501000080 c3",memory="native")
        self.driver(source,"""
int main() {
  for(std::uint64_t n : {0ULL,1ULL,0x7fffffffULL,0x80000000ULL,0xffffffffULL}) {
    std::uint64_t expected=std::uint32_t(std::uint32_t(n)+0x80000001u);
    if(sharp_retro::source::fn_1000(n)!=expected) return 1;
  }
}
""")

    def test_full_width_bit_clear_mask_keeps_high_bits(self):
        source=self.recover("4889f8 4883e0f0 c3",memory="native")
        body=source.read_text().split("// Recovered function bodies",1)[1]
        self.assertRegex(body,r"&\s*~0xfULL")
        self.driver(source,"""
int main() {
  for(std::uint64_t n : {0ULL,1ULL,15ULL,16ULL,0x800000000000000fULL,0xffffffffffffffffULL})
    if(sharp_retro::source::fn_1000(n)!=(n&~15ULL)) return 1;
}
""")
    def test_narrow_arithmetic_keeps_promotion_and_truncation(self):
        source=self.recover("89f8 0fafc0 83c007 c3",memory="native")
        self.driver(source,"""
int main() {
  std::uint32_t cases[]={0,1,0xffff,0x10000,0x7fffffff,0x80000000,0xffffffff};
  for(auto n:cases) {
    auto expected=std::uint32_t(std::uint64_t(n)*n+7);
    if(sharp_retro::source::fn_1000(n)!=expected) return 1;
  }
}
""")

    def test_wide_multiply_keeps_high_half(self):
        source=self.recover("4889f8 48f7e6 4831d0 c3",memory="native")
        self.driver(source,"""
int main() {
  std::uint64_t cases[]={0,1,0xffffffff,0x8000000000000000ULL,0xffffffffffffffffULL};
  for(auto a:cases) for(auto b:cases) {
    auto product=static_cast<unsigned __int128>(a)*b;
    auto expected=std::uint64_t(product)^std::uint64_t(product>>64);
    if(sharp_retro::source::fn_1000(a,b)!=expected) return 1;
  }
}
""")
