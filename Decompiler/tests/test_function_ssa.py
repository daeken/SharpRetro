"""Native function SSA and private scalar promotion against explicit oracles."""
import test_pipeline as pipeline
import test_values as values
from test_functions import Code


class FunctionSsaTests(pipeline.PipelineCase):
    driver=values.ValueRecoveryTests.driver
    execute=values.ValueRecoveryTests.execute

    def recover(self,code):
        self.image.write_bytes(pipeline.macho(code))
        source=self.root/"values.cpp"
        result=self.cli("decompile-app",self.image,"--memory","native","--output",source)
        self.assertEqual(result.returncode,0,result.stderr)
        self.assertIn("Function SSA:",source.read_text())
        return source

    def test_loop_phi_swap_uses_parallel_selected_edge_copies(self):
        code=(Code().emit("ba0b000000 be1d000000 89f9").label("head").emit("85c9")
              .branch("74","done").emit("4889d0 4889f2 4889c6 ffc9").branch("eb","head")
              .label("done").emit("4889d0 c3").finish())
        source=self.recover(code)
        self.assertIn("edge_value_",source.read_text())
        self.driver(source,"""
int main() {
  for(unsigned n=0;n<1024;++n)
    if(sharp_retro::source::fn_1000(n)!=(n&1 ? 29u : 11u)) return 1;
}
""")

    def test_entry_backedge_keeps_external_entry_distinct(self):
        code=(Code().label("entry").emit("89f8 85ff").branch("74","done")
              .emit("ffcf").branch("eb","entry").label("done").emit("c3").finish())
        source=self.recover(code)
        self.driver(source,"""
int main() { for(unsigned n=0;n<64;++n) if(sharp_retro::source::fn_1000(n)!=0) return 1; }
""")

    def test_reused_argument_register_has_distinct_value_lifetimes(self):
        code=(Code().emit("89f8 83c007 85ff").branch("74","done")
              .emit("bf03000000 01f8").label("done").emit("c3").finish())
        source=self.recover(code)
        body=source.read_text().split("// Recovered function bodies",1)[1].split("int recovered_program",1)[0]
        self.assertNotRegex(body,r"argument_0\s*=(?!=)")
        # Nonoverlapping internal versions may now coalesce into one mutable
        # local; the entry parameter must still retain its original identity.
        self.assertIn("std::uint64_t variable_",body)
        self.driver(source,"""
int main() {
  for(unsigned n : {0u,1u,127u,0xfffffff8u,0xffffffffu}) {
    unsigned expected=n+(n ? 10u : 7u);
    if(sharp_retro::source::fn_1000(n)!=expected) return 1;
  }
}
""")

    def test_private_load_is_snapshot_before_later_store(self):
        source=self.recover(bytes.fromhex("4883ec08 893c24 8b0424 c704242a000000 030424 4883c408 c3"))
        self.assertNotIn("stack_local_",source.read_text())
        self.driver(source,"""
int main() {
  for(unsigned n : {0u,1u,127u,0xfffffff8u,0xffffffffu})
    if(sharp_retro::source::fn_1000(n)!=unsigned(n+42u)) return 1;
}
""")

    def test_private_cell_phi_has_both_definite_branch_definitions(self):
        code=(Code().emit("4883ec08 85ff").branch("74","zero").emit("c7042411000000")
              .branch("eb","join").label("zero").emit("c704241d000000")
              .label("join").emit("8b0424 4883c408 c3").finish())
        source=self.recover(code)
        self.assertNotIn("stack_local_",source.read_text())
        self.driver(source,"""
int main() { for(unsigned n=0;n<32;++n) if(sharp_retro::source::fn_1000(n)!=(n ? 17u : 29u)) return 1; }
""")

    def test_partial_alias_and_uninitialized_path_keep_native_storage(self):
        # A high-half view is not an independent initialized scalar.
        source=self.recover(bytes.fromhex("4883ec08 48b81122334455667788 48890424 8b442404 4883c408 c3"))
        self.assertIn("stack_local_",source.read_text())
        self.driver(source,"""
int main() { return sharp_retro::source::fn_1000()==0x88776655u ? 0 : 1; }
""")
        code=(Code().emit("4883ec08 85ff").branch("74","join").emit("c7042411000000")
              .label("join").emit("8b0424 4883c408 c3").finish())
        source=self.recover(code)
        self.assertIn("stack_local_",source.read_text())
        # Execute only the initialized path; the other path is a refusal of
        # promotion, not a claim about original uninitialized stack contents.
        self.driver(source,"int main() { return sharp_retro::source::fn_1000(1)==17 ? 0 : 1; }")

    def test_helper_observation_keeps_private_buffer_in_memory(self):
        code=(Code().emit("4883ec08 c704240b000000 4889e7").call("change")
              .emit("8b0424 4883c408 c3").label("change").emit("c7071d000000 c3").finish())
        source=self.recover(code)
        self.assertIn("stack_local_",source.read_text())
        self.driver(source,"int main() { return sharp_retro::source::fn_1000()==29 ? 0 : 1; }")

    def test_frame_coordinate_copy_across_blocks_is_not_a_missing_ssa_input(self):
        source=self.recover(bytes.fromhex("55 4883ec10 eb00 4889e5 eb00 48c745002a000000 488b4500 4883c410 5d c3"))
        self.driver(source,"int main() { return sharp_retro::source::fn_1000()==42 ? 0 : 1; }")

    def test_index_guard_cannot_bound_a_value_updated_after_comparison(self):
        prefix="4883ec20 48c7042401000000 48c744240802000000 48c744241003000000 b963000000 4883fe01"
        for stale in (False,True):
            with self.subTest(stale=stale):
                code=bytes.fromhex(prefix+("488d7601" if stale else "")+"7708 488d0424 488b0cf0 4889c8 4883c420 c3")
                source=self.recover(code)
                if stale:
                    # Physical storage may now be an array; the stale bound
                    # still cannot authorize direct indexing or promotion.
                    body=source.read_text().split("// Recovered function bodies",1)[1]
                    self.assertIn("FrameStorage",body)
                    self.assertIn("*reinterpret_cast<const u64*>(pointer(",body)
                else: self.assertNotIn("stack_local_10",source.read_text())
                self.driver(source,f"""
int main() {{
  for(std::uint64_t n : {{0ULL,1ULL,2ULL,3ULL,0xffffffffffffffffULL}}) {{
    auto expected=n<=1 ? n+{2 if stale else 1} : 99;
    if(sharp_retro::source::fn_1000(n)!=expected) return 1;
  }}
}}
""")

    def test_frame_bits_and_implicit_control_keep_stack_storage(self):
        # Refusal/classification fixtures: observable raw frame-address values
        # are outside the native profile, so no numeric address oracle is used.
        cases=[
            "4883ec08 48c7042407000000 488d0424 48d1e8 48d1e0 488907 488b0424 4883c408 c3",
            "4883ec18 48c744241007000000 488d0424 83e001 ba00000000 b901000000 85c0 480f45d1 488917 488b442410 4883c418 c3",
            "4883ec18 48c744241007000000 488d0424 83e001 85c0 7407 ba01000000 eb05 ba00000000 488917 488b442410 4883c418 c3"
        ]
        for code in cases:
            with self.subTest(code=code):
                source=self.recover(bytes.fromhex(code))
                self.assertIn("0 private stack cells promoted",source.read_text())
                self.assertIn("stack_local_",source.read_text())
