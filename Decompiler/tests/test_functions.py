"""Function partitions/contracts, real native calls, recursion and tail transfers."""
import struct
import platform
import unittest

import test_pipeline as pipeline
import test_values as values


class Code:
    def __init__(self):
        self.bytes, self.labels, self.fixups = bytearray(), {}, []

    def emit(self, text):
        self.bytes += bytes.fromhex(text)
        return self

    def label(self, name):
        self.labels[name] = len(self.bytes)
        return self

    def branch(self, opcode, label, width=1):
        self.emit(opcode)
        self.fixups.append((len(self.bytes), label, width))
        self.bytes += bytes(width)
        return self

    def call(self, label):
        return self.branch("e8", label, 4)

    def finish(self):
        for offset, label, width in self.fixups:
            value = self.labels[label] - offset - width
            self.bytes[offset:offset+width] = value.to_bytes(width, "little", signed=True)
        return bytes(self.bytes)


def with_starts(code, offsets):
    image = pipeline.macho(code)
    # Text segment includes the headers in this synthetic linked-image mapping.
    struct.pack_into("<QQQQ", image, 56, pipeline.BASE-0x100, 0x100+len(code), 0, len(image))
    struct.pack_into("<II", image, 16, 3, 192)
    payload = bytearray()
    previous = pipeline.BASE-0x100
    for offset in offsets:
        current = pipeline.BASE+offset
        delta = current-previous
        while delta >= 128:
            payload.append((delta & 127)|128)
            delta >>= 7
        payload.append(delta)
        previous = current
    payload.append(0)
    assert len(payload) <= 32
    struct.pack_into("<IIII", image, 208, 0x26, 16, 224, len(payload))
    image[224:224+len(payload)] = payload
    return image


class FunctionRecoveryTests(pipeline.PipelineCase):
    execute = values.ValueRecoveryTests.execute
    driver = values.ValueRecoveryTests.driver

    def recover(self, code, starts=None, memory="auto"):
        self.image.write_bytes(pipeline.macho(code) if starts is None else with_starts(code, starts))
        source = self.root / "values.cpp"
        result = self.cli("decompile-app", self.image, "--representation", "recovered", "--memory", memory, "--output", source)
        self.assertEqual(result.returncode, 0, result.stderr)
        return source

    def test_real_function_calls_and_scalar_return(self):
        code = Code().call("helper").emit("c3").label("helper").emit("8d4707 c3")
        source = self.recover(code.finish())
        text = source.read_text()
        self.assertIn("2 functions", text)
        self.assertNotIn("return_dispatch", text)
        self.assertIn("static std::uint64_t fn_1006", text)
        self.assertRegex(text,r"call_result_0\s*=\s*fn_1006\(")
        self.driver(source, """
int main() {
  for(int n=0;n<32;++n) if(sharp_retro::source::recovered_program(n,nullptr)!=n+7) return 1;
}
""")

    def test_demanded_multiple_register_results(self):
        code = Code().call("helper").emit("31d0 c3").label("helper").emit("b812000000 ba34000000 c3")
        source = self.recover(code.finish())
        self.assertIn("struct result_", source.read_text())
        result = self.execute(source)
        self.assertEqual((result.returncode, result.stderr), (0x26, ""))

    def test_void_function_with_memory_side_effects(self):
        code = (Code().emit("4883ec08 4889e7").call("store").emit("0fb60424 4883c408 c3")
                .label("store").emit("c6072a c3"))
        source = self.recover(code.finish())
        self.assertIn("static void fn_",source.read_text())
        result = self.execute(source)
        self.assertEqual((result.returncode,result.stderr),(42,""))

    def test_flags_cross_function_boundary_explicitly(self):
        code = Code().call("helper").emit("0f92c0 0fb6c0 c3").label("helper").emit("f9 c3")
        source = self.recover(code.finish())
        result = self.execute(source)
        self.assertEqual((result.returncode, result.stderr), (1, ""))

    def test_recursion_and_callee_saved_values(self):
        code = (Code().call("factorial").emit("c3").label("factorial").emit("83ff01")
                .branch("7e", "base").emit("53 4889fb ffcf").call("factorial")
                .emit("480fafc3 5b c3").label("base").emit("b801000000 c3"))
        source = self.recover(code.finish())
        self.driver(source, """
int main() {
  unsigned long long value=1;
  for(int n=0;n<13;++n) {
    if(n) value*=n;
    if(sharp_retro::source::recovered_program(n,nullptr)!=int(value & 255)) return 1;
  }
}
""")

    def test_mutual_recursion_contract_fixed_point(self):
        code = (Code().call("a").emit("c3").label("a").emit("85ff").branch("74", "abase")
                .emit("ffcf").call("b").emit("4883c001 c3").label("abase").emit("b807000000 c3")
                .label("b").emit("85ff").branch("74", "bbase").emit("ffcf").call("a")
                .emit("4883c002 c3").label("bbase").emit("b803000000 c3"))
        source = self.recover(code.finish())
        self.driver(source, """
unsigned b(unsigned);
unsigned a(unsigned n) { return n ? b(n-1)+1 : 7; }
unsigned b(unsigned n) { return n ? a(n-1)+2 : 3; }
int main() {
  for(int n=0;n<24;++n) if(sharp_retro::source::recovered_program(n,nullptr)!=int(a(n))) return 1;
}
""")

    def test_metadata_tail_only_function_and_stripped_name_independence(self):
        code = Code().call("first").emit("c3").label("first").branch("e9", "second", 4).label("second").emit("8d4709 c3")
        source = self.recover(code.finish(), [0, code.labels["first"], code.labels["second"]])
        self.assertIn("3 functions", source.read_text())
        self.assertIn("// Tail call:", source.read_text())
        self.driver(source, """
int main() {
  for(int n=0;n<16;++n) if(sharp_retro::source::recovered_program(n,nullptr)!=n+9) return 1;
}
""")

    def test_ret_immediate_cleanup_is_preserved(self):
        code = Code().emit("4883ec08").call("helper").emit("c3").label("helper").emit("b82a000000 c20800")
        source = self.recover(code.finish())
        result = self.execute(source)
        self.assertEqual((result.returncode, result.stderr), (42, ""))

    def test_noncanonical_return_and_checked_recursion_limit_do_not_pass(self):
        altered = Code().call("helper").emit("c3").label("helper").emit("48c7042400100000 b807000000 c3")
        result = self.execute(self.recover(altered.finish()))
        self.assertEqual(result.returncode, 125)
        self.assertIn("noncanonical recovered call return", result.stderr)
        code = Code().label("self").call("self").emit("c3")
        result = self.execute(self.recover(code.finish(),memory="checked"))
        self.assertEqual(result.returncode, 125)
        self.assertIn("call depth exhausted", result.stderr)

    def test_shared_function_tail_refuses_ambiguous_ownership(self):
        code = (Code().call("a").call("b").emit("c3").label("a").emit("b801000000")
                .branch("eb","shared").label("b").emit("b802000000")
                .branch("eb","shared").label("shared").emit("c3"))
        self.image.write_bytes(pipeline.macho(code.finish()))
        output=self.root/"must-not-exist.cpp"
        result=self.cli("decompile-app",self.image,"--output",output)
        self.assertNotEqual(result.returncode,0)
        self.assertIn("ambiguous ownership",result.stderr)
        self.assertFalse(output.exists())
        result=self.cli("decompile-app",self.image,"--representation","blocks","--output",output)
        self.assertEqual(result.returncode,0,result.stderr)
        executed=self.execute(output)
        self.assertEqual((executed.returncode,executed.stderr),(2,""))

    def test_invalid_function_metadata_refuses_before_source_publication(self):
        # Function start points into the immediate of MOV EAX,imm32.
        image = with_starts(bytes.fromhex("b801000000c3"), [1])
        self.image.write_bytes(image)
        output = self.root / "must-not-exist.cpp"
        result = self.cli("decompile-app", self.image, "--output", output)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("inside a recovered instruction", result.stderr)
        self.assertFalse(output.exists())
        # An unterminated ULEB128 must not become a guessed function entry.
        struct.pack_into("<I", image, 220, 1)
        image[224] = 0x80
        self.image.write_bytes(image)
        result = self.cli("inspect", self.image)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Truncated Mach-O function start", result.stderr)

    @unittest.skipUnless(platform.system()=="Darwin" and platform.machine()=="arm64","Native recursive fixture uses Mach-O/Rosetta")
    def test_clang_recursive_executable_against_its_original(self):
        # Volatile stack use prevents this fixture from becoming a simple loop.
        import test_cli_app as cli
        original_source=self.root/"original.cpp"
        original_source.write_text('''
#include <stdint.h>
__attribute__((noinline)) uint64_t factorial(unsigned n) {
    if(n<2) return 1;
    volatile uint64_t saved=n;
    return saved*factorial(n-1);
}
int main(int argc,char**) { auto x=factorial((unsigned)argc); return int((x^(x>>8))&255); }
''')
        original=self.root/"original-x86_64"
        cli.DEMO.tool("clang++",*cli.DEMO.X64_FLAGS,original_source,"-o",original)
        source=self.root/"values.cpp"
        cli.DEMO.tool("dotnet",pipeline.DLL,"decompile-app",original,"--output",source)
        recovered=self.root/"recursive-arm64"
        cli.DEMO.tool("clang++","-arch","arm64","-std=c++20","-O2","-fsanitize=address,undefined",source,"-o",recovered)
        for count in range(1,13):
            case={"argv":["x"]*(count-1),"stdin":b""}
            self.assertEqual(cli.DEMO.observe(recovered,case),cli.DEMO.observe(original,case))
