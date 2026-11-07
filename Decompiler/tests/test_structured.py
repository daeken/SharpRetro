"""Structured source vs dispatch, including exact interrupted execution states."""
import test_pipeline as pipeline


class Assembler:
    """Tiny explicit short-branch fixups for synthetic topology fixtures."""
    def __init__(self):
        self.code, self.labels, self.fixups = bytearray(), {}, []

    def emit(self, hex_bytes):
        self.code += bytes.fromhex(hex_bytes)
        return self

    def label(self, name):
        assert name not in self.labels
        self.labels[name] = len(self.code)
        return self

    def branch(self, opcode, target):
        self.emit(opcode)
        self.fixups.append((len(self.code), target))
        self.code.append(0)
        return self

    def finish(self):
        for offset, name in self.fixups:
            displacement = self.labels[name] - offset - 1
            assert -128 <= displacement <= 127
            self.code[offset] = displacement & 255
        return bytes(self.code)


def fixtures():
    diamond = (Assembler().emit("89f8 85f6").branch("74", "else").emit("83c007")
               .branch("eb", "join").label("else").emit("83e803").label("join").emit("c3"))
    nested_if = (Assembler().emit("85ff").branch("74", "zero").emit("85f6")
                 .branch("74", "innerelse").emit("b80b000000").branch("eb", "end")
                 .label("innerelse").emit("b816000000").branch("eb", "end")
                 .label("zero").emit("b821000000").label("end").emit("c3"))
    pretest = (Assembler().emit("31c0 89f9").label("head").emit("85c9")
               .branch("74", "end").emit("01c8 ffc9").branch("eb", "head").label("end").emit("c3"))
    posttest = (Assembler().emit("89f8 89f1").label("head").emit("83c003 ffc9")
                .branch("75", "head").emit("c3"))
    loop_edges = (Assembler().emit("31c0 89f9").label("head").emit("85c9").branch("74", "end")
                  .emit("ffc9 83f903").branch("74", "head").emit("83f901").branch("74", "end")
                  .emit("01c8").branch("eb", "head").label("end").emit("c3"))
    nested_loop = (Assembler().emit("31c0 89f9").label("outer").emit("85c9").branch("74", "end")
                   .emit("89f2").label("inner").emit("85d2").branch("74", "innerdone")
                   .emit("01c8 ffca").branch("eb", "inner").label("innerdone").emit("ffc9")
                   .branch("eb", "outer").label("end").emit("c3"))
    early_return = (Assembler().emit("85ff").branch("74", "zero").emit("89f8 c3")
                    .label("zero").emit("31c0 c3"))
    divergent = (Assembler().emit("85f6").branch("74", "done").label("forever")
                 .branch("eb", "forever").label("done").emit("89f8 c3"))
    shared_latch = (Assembler().emit("31c0 89f9 85c9").branch("74", "end")
                    .label("head").emit("ffc9 83f903").branch("74", "latch")
                    .emit("83f901").branch("74", "end").emit("01c8")
                    .label("latch").emit("85c9").branch("75", "head").label("end").emit("c3"))
    return {
        "diamond": (diamond.finish(), "", "std::uint32_t(a + (b ? 7 : -3))", False),
        "nested_if": (nested_if.finish(), "", "a ? (b ? 11 : 22) : 33", False),
        "pretest_loop": (pretest.finish(), "while(true)", "a * (a + 1) / 2", True),
        "posttest_loop": (posttest.finish(), "continue;", "a + b * 3", True),
        "break_continue": (loop_edges.finish(), "break;", "expected_break(a)", True),
        "nested_loops": (nested_loop.finish(), "while(true)", "b * a * (a + 1) / 2", True),
        "early_returns": (early_return.finish(), "", "a", False),
        "divergent_path": (divergent.finish(), "while(true)", "a", True),
        "shared_latch": (shared_latch.finish(), "while(true)", "expected_break(a)", True),
    }


TRACE_MEMORY = r"""
struct TracedArena : Arena {
    std::vector<std::tuple<char, std::uint64_t, unsigned, sharp_retro::bits>> trace;
    sharp_retro::bits load(std::uint64_t address, unsigned width) override {
        auto result = Arena::load(address, width);
        trace.emplace_back('r', address, width, result); return result;
    }
    void store(std::uint64_t address, sharp_retro::bits value, unsigned width) override {
        Arena::store(address, value, width); trace.emplace_back('w', address, width, value);
    }
};
bool same_state(const sharp_retro::State& a, const sharp_retro::State& b) {
    if(a.pc != b.pc || a.flags != b.flags || a.defined_flags != b.defined_flags) return false;
    for(unsigned i=0; i<16; ++i) if(a.gpr[i]!=b.gpr[i]) return false;
    for(unsigned i=0; i<6; ++i) if(a.segments[i]!=b.segments[i]) return false;
    return true;
}
"""


class StructuredFlowTests(pipeline.PipelineCase):
    def structured(self, code, style="structured"):
        self.image.write_bytes(pipeline.elf(code))
        target = self.root / "recovered.hpp"
        result = self.cli("decompile", self.image, "--range", f"0x1000:{len(code)}",
                          "--control-flow", style, "--output", target)
        self.assertEqual(result.returncode, 0, result.stderr)
        return target.read_text()

    def test_nested_regions_preserve_every_interrupted_state(self):
        for name, (code, syntax, expected, is_loop) in fixtures().items():
            with self.subTest(shape=name):
                source = self.structured(code)
                self.assertIn("// Control flow: structured;", source)
                body = source.split("inline Exit run(", 1)[1]
                self.assertNotIn("goto ", body)
                if syntax:
                    self.assertIn(syntax, body)
                self.assertEqual("while(true)" in body, is_loop)
                if name == "nested_loops":
                    self.assertEqual(body.count("while(true)"), 2)
                if name == "break_continue":
                    self.assertIn("continue;", body)
                if name == "nested_if":
                    self.assertGreaterEqual(body.count("if(condition_"), 2)
                self.compile_run('#include "recovered.hpp"\n#include <vector>\n#include <tuple>\n#include <cstring>\n'
                                 + pipeline.ARENA + TRACE_MEMORY + f"""
unsigned expected_break(unsigned n) {{
    unsigned result=0;
    while(n) {{ --n; if(n==3) continue; if(n==1) break; result+=n; }}
    return result;
}}
int main() {{
  for(unsigned a=0; a<9; ++a) for(unsigned b=0; b<6; ++b) {{
    sharp_retro::State initial; TracedArena memory;
    for(unsigned i=0; i<16; ++i) initial.gpr[i]=0xa5a50000+i;
    initial.pc=sharp_retro::entry; initial.gpr[4]=0x103000; initial.gpr[7]=a; initial.gpr[6]=b;
    memory.store(initial.gpr[4],0xabcdef,64); memory.trace.clear();
    for(unsigned budget=0; budget<97; ++budget) {{
      auto x=initial, y=initial; auto mx=memory, my=memory;
      auto ex=sharp_retro::run(x,mx,budget);
      auto ey=sharp_retro::run_dispatch(y,my,budget);
      if(ex!=ey || !same_state(x,y) || std::memcmp(mx.bytes,my.bytes,sizeof(mx.bytes)) || mx.trace!=my.trace) return 1;
    }}
    if({str(name == "posttest_loop").lower()} && b==0) continue;
    if({str(name == "divergent_path").lower()} && b!=0) continue;
    auto x=initial; auto mx=memory;
    if(sharp_retro::run(x,mx,10000)!=sharp_retro::Exit::returned || x.gpr[0] != ({expected})) return 2;
    // Resume the public API after each instruction, including instructions in
    // the middle of a basic block. Ordered effects must agree with one full run.
    auto y=initial; auto my=memory;
    sharp_retro::Exit exit;
    unsigned steps=0;
    do {{ exit=sharp_retro::run(y,my,1); if(++steps>10000) return 3; }} while(exit==sharp_retro::Exit::budget_exhausted);
    if(exit!=sharp_retro::Exit::returned || !same_state(x,y) || mx.trace!=my.trace) return 4;
  }}
}}
""")

    def test_unsupported_shapes_fallback_without_losing_paths(self):
        multi_exit = (Assembler().label("head").emit("85ff").branch("74", "one")
                      .emit("85f6").branch("74", "two").emit("ffcf").branch("eb", "head")
                      .label("one").emit("b80b000000c3").label("two").emit("b816000000c3").finish())
        irreducible = (Assembler().emit("85ff").branch("74", "b").label("a")
                       .emit("ffce").branch("74", "end").label("b").emit("ffcf")
                       .branch("75", "a").label("end").emit("c3").finish())
        for code in (multi_exit, irreducible):
            with self.subTest(code=code.hex()):
                source = self.structured(code, "auto")
                self.assertIn("// Control flow: dispatch fallback:", source)
                target = self.root / "strict.hpp"
                result = self.cli("decompile", self.image, "--range", f"0x1000:{len(code)}",
                                  "--control-flow", "structured", "--output", target)
                self.assertNotEqual(result.returncode, 0)
                self.assertIn("Cannot structure control flow", result.stderr)
                self.assertFalse(target.exists())
                self.compile_run('#include "recovered.hpp"\n' + pipeline.ARENA + """
int main() {
    Arena m; sharp_retro::State s; s.pc=sharp_retro::entry; s.gpr[4]=0x103000;
    s.gpr[7]=4; s.gpr[6]=3; m.store(s.gpr[4],0xabcdef,64);
    auto x=s; auto n=m;
    return sharp_retro::run(s,m,100)==sharp_retro::run_dispatch(x,n,100) && s.pc==x.pc && s.gpr[0]==x.gpr[0] ? 0 : 1;
}
""")

    def test_structuring_preserves_ordered_memory_effects_inside_loop(self):
        code = (Assembler().emit("89f1").label("head").emit("85c9").branch("74", "end")
                .emit("f6c101").branch("74", "even").emit("480107").branch("eb", "latch")
                .label("even").emit("48314708").label("latch").emit("48ffc0 ffc9")
                .branch("eb", "head").label("end").emit("c3").finish())
        self.structured(code)
        self.compile_run('#include "recovered.hpp"\n#include <vector>\n#include <tuple>\n#include <cstring>\n'
                         + pipeline.ARENA + TRACE_MEMORY + """
int main() {
  for(unsigned count=0; count<8; ++count) for(unsigned budget=0; budget<101; ++budget) {
    TracedArena m; sharp_retro::State s;
    s.pc=sharp_retro::entry; s.gpr[0]=0x123456789; s.gpr[7]=0x100200;
    s.gpr[6]=count; s.gpr[4]=0x103000;
    m.store(0x100200,0xffffffffffffffffULL,64); m.store(0x100208,0x8000000000000000ULL,64);
    m.store(s.gpr[4],0xabcdef,64); m.trace.clear();
    auto x=s; auto n=m;
    auto ex=sharp_retro::run(s,m,budget); auto ey=sharp_retro::run_dispatch(x,n,budget);
    if(ex!=ey || !same_state(s,x) || m.trace!=n.trace || std::memcmp(m.bytes,n.bytes,sizeof(m.bytes))) return 1;
  }
}
""")

    def test_partial_boundary_and_undefined_flags_keep_their_outcomes(self):
        self.structured(bytes.fromhex("85ff74029090"))
        self.compile_run('#include "recovered.hpp"\n' + pipeline.ARENA + """
int main() {
  for(unsigned a=0; a<2; ++a) {
    Arena m; sharp_retro::State s; s.pc=sharp_retro::entry; s.gpr[7]=a;
    if(sharp_retro::run(s,m,20)!=sharp_retro::Exit::boundary || s.pc!=0x1006) return 1;
  }
}
""")
        self.structured(bytes.fromhex("0fafc79fc3"))
        self.compile_run('#include "recovered.hpp"\n#include <string>\n' + pipeline.ARENA + """
int main() {
  Arena m; sharp_retro::State s; s.pc=sharp_retro::entry;
  try { sharp_retro::run(s,m,20); return 1; }
  catch(const std::runtime_error& e) { return std::string(e.what()).find("undefined flags")==std::string::npos ? 2 : 0; }
}
""")

    def test_dispatch_option_and_invalid_style(self):
        source = self.structured(bytes.fromhex("b82a000000c3"), "dispatch")
        self.assertIn("// Control flow: dispatch requested", source)
        result = self.cli("decompile", self.image, "--range", "0x1000:6", "--control-flow", "imaginary")
        self.assertNotEqual(result.returncode, 0)

    def test_analysis_limit_falls_back_with_complete_translation(self):
        code = bytes.fromhex("eb00") * 512 + b"\xc3"
        source = self.structured(code, "auto")
        self.assertIn("reachable instructions: 513", source)
        self.assertIn("dispatch fallback: Structuring analysis limit", source)
        self.assertEqual(source.count("case 0x"), 513)
        result = self.cli("decompile", self.image, "--range", f"0x1000:{len(code)}", "--control-flow", "structured")
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("analysis limit", result.stderr)
