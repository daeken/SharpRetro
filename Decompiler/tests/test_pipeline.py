"""Integration checks for actual LiftIl -> C++ translation, not a game oracle.

Build Decompiler in Release first. Tests need dotnet and clang++; native Mach-O
checks additionally need macOS with x86-64 execution (Rosetta on Apple Silicon).
"""
import json
from pathlib import Path
import platform
import random
import shutil
import struct
import subprocess
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
DLL = ROOT / "Decompiler/bin/Release/net10.0/Decompiler.dll"
BASE = 0x1000


def run(*args, **kwargs):
    return subprocess.run([str(a) for a in args], text=True, capture_output=True,
                          timeout=45, **kwargs)


def pe(code, wide=True):
    b = bytearray(0x200 + len(code))
    b[:2] = b"MZ"
    struct.pack_into("<I", b, 0x3c, 0x80)
    b[0x80:0x84] = b"PE\0\0"
    opt_size = 0xf0 if wide else 0xe0
    struct.pack_into("<HHIIIHH", b, 0x84, 0x8664 if wide else 0x14c, 1, 0, 0, 0, opt_size, 2)
    opt = 0x98
    struct.pack_into("<H", b, opt, 0x20b if wide else 0x10b)
    struct.pack_into("<I", b, opt + 16, BASE)
    if wide:
        struct.pack_into("<Q", b, opt + 24, 0)
    else:
        struct.pack_into("<I", b, opt + 28, 0)
    s = opt + opt_size
    b[s:s+8] = b".text\0\0\0"
    struct.pack_into("<IIII", b, s+8, len(code), BASE, len(code), 0x200)
    struct.pack_into("<I", b, s+36, 0x60000020)
    b[0x200:] = code
    return b


def elf(code, wide=True):
    b = bytearray(0x100 + len(code))
    b[:7] = b"\x7fELF" + bytes([2 if wide else 1, 1, 1])
    struct.pack_into("<HHI", b, 16, 2, 62 if wide else 3, 1)
    if wide:
        struct.pack_into("<QQ", b, 24, BASE, 64)
        struct.pack_into("<HHH", b, 52, 64, 56, 1)
        struct.pack_into("<IIQQQQQQ", b, 64, 1, 5, 0x100, BASE, BASE, len(code), len(code)+16, 0x1000)
    else:
        struct.pack_into("<II", b, 24, BASE, 52)
        struct.pack_into("<HHH", b, 40, 52, 32, 1)
        struct.pack_into("<IIIIIIII", b, 52, 1, 0x100, BASE, BASE, len(code), len(code)+16, 5, 0x1000)
    b[0x100:] = code
    return b


def macho(code):
    b = bytearray(0x100 + len(code))
    struct.pack_into("<IIIIIIII", b, 0, 0xfeedfacf, 0x1000007, 3, 2, 2, 176, 0, 0)
    struct.pack_into("<II16sQQQQIIII", b, 32, 0x19, 152, b"__TEXT", BASE,
                     len(code), 0x100, len(code), 7, 5, 1, 0)
    struct.pack_into("<16s16sQQIIIIIIII", b, 104, b"__text", b"__TEXT", BASE,
                     len(code), 0x100, 0, 0, 0, 0x80000400, 0, 0, 0)
    struct.pack_into("<IIQQ", b, 184, 0x80000028, 24, 0x100, 0)
    b[0x100:] = code
    return b


ARENA = r"""
struct Arena : sharp_retro::Memory {
    std::uint8_t bytes[16384]{};
    static constexpr std::uint64_t base = 0x100000;
    void check(std::uint64_t address, unsigned width) {
        if(width == 0 || width > 128 || width % 8 || address < base ||
           address - base > sizeof(bytes) || width / 8 > sizeof(bytes) - (address - base))
            throw std::runtime_error("guest memory bounds");
    }
    sharp_retro::bits load(std::uint64_t address, unsigned width) override {
        check(address, width); sharp_retro::bits v = 0;
        for(unsigned i = 0; i < width / 8; ++i) v |= sharp_retro::bits(bytes[address - base + i]) << (i * 8);
        return v;
    }
    void store(std::uint64_t address, sharp_retro::bits v, unsigned width) override {
        check(address, width);
        for(unsigned i = 0; i < width / 8; ++i) bytes[address - base + i] = std::uint8_t(v >> (i * 8));
    }
};
"""


class PipelineCase(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if not DLL.exists():
            raise RuntimeError("Build Decompiler/Decompiler.csproj -c Release before running tests")
        if not shutil.which("clang++"):
            raise RuntimeError("clang++ is required for compiled integration tests")

    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix="sharpretro-decompiler-")
        self.root = Path(self.temp.name)
        self.image = self.root / "image.bin"

    def tearDown(self):
        self.temp.cleanup()

    def cli(self, *args):
        return run("dotnet", DLL, *args)

    def inspect(self, data):
        self.image.write_bytes(data)
        result = self.cli("inspect", self.image)
        self.assertEqual(result.returncode, 0, result.stderr)
        return json.loads(result.stdout)

    def translate(self, data, entry=BASE, size=None):
        self.image.write_bytes(data)
        if size is None:
            info = self.inspect(data)
            size = next(r["FileSize"] for r in info["Regions"] if r["Executable"])
        target = self.root / "recovered.hpp"
        result = self.cli("decompile", self.image, "--range", f"0x{entry:x}:{size}", "--output", target)
        self.assertEqual(result.returncode, 0, result.stderr)
        return target

    def compile_run(self, source):
        cpp, exe = self.root / "driver.cpp", self.root / "driver"
        cpp.write_text(source)
        result = run("clang++", "-std=c++20", "-O2", "-Wall", "-Wextra", "-Werror",
                     "-Wno-unused-parameter", "-fsanitize=undefined,address", cpp, "-o", exe)
        self.assertEqual(result.returncode, 0, result.stderr)
        result = run(exe)
        self.assertEqual(result.returncode, 0, result.stderr)
        return result.stdout


class PipelineTests(PipelineCase):
    def test_all_container_formats_and_x86_widths(self):
        for builder, expected, arch in [(pe, "Pe", "X86_64"), (elf, "Elf", "X86_64"),
                                         (macho, "MachO", "X86_64"),
                                         (lambda c: pe(c, False), "Pe", "X86"),
                                         (lambda c: elf(c, False), "Elf", "X86")]:
            with self.subTest(format=expected, arch=arch):
                info = self.inspect(builder(bytes.fromhex("b82a000000c3")))
                self.assertEqual((info["Format"], info["Architecture"], info["Entry"]), (expected, arch, BASE))
                self.assertEqual(len(info["Sha256"]), 64)

    def test_truncated_images_are_rejected(self):
        for builder in (pe, elf, macho):
            full = builder(b"\xc3")
            for size in (0, 1, 16, 31, len(full)-1):
                with self.subTest(builder=builder.__name__, size=size):
                    self.image.write_bytes(full[:size])
                    self.assertNotEqual(self.cli("inspect", self.image).returncode, 0)

    def test_unmapped_and_crossing_ranges_are_rejected(self):
        self.image.write_bytes(pe(b"\xb8\x01\x02"))
        for bounds in ("0x1000:3", "0x9999:2", "0xffffffffffffffff:2", "0x1000:0"):
            with self.subTest(bounds=bounds):
                result = self.cli("decompile", self.image, "--range", bounds)
                self.assertNotEqual(result.returncode, 0, result.stderr)

    def test_refuses_unsupported_semantics_without_publishing_output(self):
        for code in ("0f05", "0f0b", "e800000000c3", "ffe0", "f0480107", "0f57c0c3"):
            with self.subTest(code=code):
                self.image.write_bytes(elf(bytes.fromhex(code)))
                target = self.root / "must-not-exist.hpp"
                result = self.cli("decompile", self.image, "--range", f"0x1000:{len(code)//2}", "--output", target)
                self.assertNotEqual(result.returncode, 0)
                self.assertFalse(target.exists())

    def test_discovery_budget_and_overlapping_instructions(self):
        self.image.write_bytes(elf(bytes.fromhex("90c3")))
        result = self.cli("decompile", self.image, "--range", "0x1000:2", "--instruction-limit", "1")
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("budget", result.stderr)
        # Conditional jump into the middle of the following MOV immediate.
        self.image.write_bytes(elf(bytes.fromhex("7401b801000000c3")))
        result = self.cli("decompile", self.image, "--range", "0x1000:8")
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Overlapping", result.stderr)

    def test_compiles_integer_return_for_every_container_and_x86_mode(self):
        for builder in (pe, elf, macho, lambda c: pe(c, False), lambda c: elf(c, False)):
            with self.subTest(builder=str(builder)):
                self.translate(builder(bytes.fromhex("b82a000000c3")))
                stack_width = 32 if self.inspect(builder(b"\xc3"))["Architecture"] == "X86" else 64
                out = self.compile_run('#include "recovered.hpp"\n#include <cstdio>\n' + ARENA + f"""
int main() {{
    Arena m; sharp_retro::State s; s.pc = sharp_retro::entry; s.gpr[4] = 0x103000;
    m.store(s.gpr[4], 0xabcdef, {stack_width});
    auto exit = sharp_retro::run(s, m, 10);
    if(exit != sharp_retro::Exit::returned || s.pc != 0xabcdef || s.gpr[4] != 0x103000 + {stack_width//8}) return 1;
    std::printf("%llu\\n", (unsigned long long)s.gpr[0]);
}}
""")
                self.assertEqual(out, "42\n")

    def test_partial_exit_and_execution_budget_are_distinct(self):
        for code, expected in (("90", "boundary"), ("ebfe", "budget_exhausted")):
            with self.subTest(code=code):
                self.translate(elf(bytes.fromhex(code)))
                self.compile_run('#include "recovered.hpp"\n' + ARENA + f"""
int main() {{ Arena m; sharp_retro::State s; s.pc=sharp_retro::entry;
    return sharp_retro::run(s, m, 7) == sharp_retro::Exit::{expected} ? 0 : 1; }}
""")

    def test_undefined_flag_is_not_a_silent_zero(self):
        # imul eax, edi; lahf reads AF/ZF/SF/PF, architecturally undefined by IMUL.
        self.translate(elf(bytes.fromhex("0fafc79fc3")))
        self.compile_run('#include "recovered.hpp"\n#include <string>\n' + ARENA + r"""
int main() {
    Arena m; sharp_retro::State s; s.pc=sharp_retro::entry; s.gpr[4]=0x103000;
    m.store(s.gpr[4], 0, 64);
    try { sharp_retro::run(s, m, 20); return 1; }
    catch(const std::runtime_error& e) {
        return std::string(e.what()).find("undefined flags") == std::string::npos ? 2 : 0;
    }
}
""")

    @unittest.skipUnless(platform.system() == "Darwin", "Native fixture requires Mach-O execution")
    def test_clang_macho_roundtrip_against_translated_x86_execution(self):
        fixtures = {
            "arithmetic": r"""
extern "C" unsigned long long fixture(unsigned long long a, unsigned long long b) {
    for(unsigned long long i = 0; i < (b & 15); ++i) a = (a * 33) ^ (b + i);
    unsigned n = (b >> 4) & 63;
    return ((a >> n) | (a << ((64 - n) & 63))) + b;
}
""",
            "memory": r"""
extern "C" unsigned long long fixture(unsigned long long* p, unsigned long long a, unsigned long long b) {
    for(unsigned long long i=0; i<(b & 7); ++i) { p[1] += p[0]; p[0] ^= a + i; }
    return p[0] + p[1];
}
""",
            "nested": r"""
extern "C" unsigned long long fixture(volatile unsigned long long* p, unsigned long long a, unsigned long long b) {
    for(unsigned long long i=0; i<(a & 7); ++i) {
        for(unsigned long long j=0; j<(b & 7); ++j) {
            if((i+j)&1) p[0] = p[0] + i; else p[1] = p[1] ^ j;
        }
    }
    return p[0] + p[1];
}
""",
            "break_continue": r"""
extern "C" unsigned long long fixture(volatile unsigned long long* p, unsigned long long a, unsigned long long b) {
    unsigned long long n=b&15;
    while(n) {
        --n;
        if(n==3) continue;
        if(n==1) break;
        if(n&1) p[0]=p[0]+a; else p[1]=p[1]^a;
    }
    return p[0] + p[1];
}
""",
        }
        randomizer = random.Random(0x5a17)
        cases = [(0, 0), (0xffffffffffffffff, 1), (0x8000000000000000, 0xffffffffffffffff)]
        cases += [(randomizer.getrandbits(64), randomizer.getrandbits(64)) for _ in range(253)]
        rows = ",\n".join(f"{{0x{a:x}ULL,0x{b:x}ULL}}" for a, b in cases)
        for name, source in fixtures.items():
            with self.subTest(fixture=name):
                fixture, dylib = self.root / "fixture.cpp", self.root / "fixture.dylib"
                fixture.write_text(source)
                result = run("clang++", "-arch", "x86_64", "-dynamiclib", "-O2", "-fno-vectorize",
                             "-fno-slp-vectorize", "-fno-unroll-loops", "-fomit-frame-pointer",
                             "-fno-stack-protector", fixture, "-o", dylib)
                self.assertEqual(result.returncode, 0, result.stderr)
                self.image.write_bytes(dylib.read_bytes())
                info_result = self.cli("inspect", self.image)
                self.assertEqual(info_result.returncode, 0, info_result.stderr)
                info = json.loads(info_result.stdout)
                text = next(r for r in info["Regions"] if r["Name"] == "__TEXT,__text")
                result = run("nm", "-n", dylib)
                self.assertEqual(result.returncode, 0, result.stderr)
                symbol = next(line.split()[0] for line in result.stdout.splitlines() if line.endswith(" _fixture"))
                entry = int(symbol, 16)
                header = self.root / "recovered.hpp"
                result = self.cli("decompile", self.image, "--range", f"0x{text['Address']:x}:{text['FileSize']}",
                                  "--entry", f"0x{entry:x}", "--output", header)
                self.assertEqual(result.returncode, 0, result.stderr)
                native = self.root / "native.cpp"
                call = "fixture(a,b)" if name == "arithmetic" else "fixture(p,a,b)"
                # Invoke the very dylib whose bytes were decompiled. Recompiling
                # the source into the harness would create a different reference.
                declaration = source.split('{', 1)[0] + ';\n'
                native.write_text('#include <cstdio>\n' + declaration + f"""
unsigned long long cases[][2] = {{ {rows} }};
int main() {{ for(auto& row : cases) {{
    auto a=row[0], b=row[1]; unsigned long long p[2]={{a,b}};
    auto result={call};
    std::printf("%016llx %016llx %016llx\\n",result,p[0],p[1]);
}} }}
""")
                result = run("clang++", "-arch", "x86_64", "-O2", "-fno-vectorize", "-fno-slp-vectorize",
                             "-fno-unroll-loops", "-fomit-frame-pointer", "-fno-stack-protector", native, dylib,
                             "-o", self.root / "native")
                self.assertEqual(result.returncode, 0, result.stderr)
                original = run(self.root / "native")
                self.assertEqual(original.returncode, 0, original.stderr)
                args = "s.gpr[7]=a; s.gpr[6]=b;" if name == "arithmetic" else "s.gpr[7]=0x100200; s.gpr[6]=a; s.gpr[2]=b;"
                generated = self.compile_run('#include "recovered.hpp"\n#include <cstdio>\n#include <initializer_list>\n' + ARENA + f"""
unsigned long long cases[][2] = {{ {rows} }};
int main() {{ for(auto& row : cases) {{
    Arena m; sharp_retro::State s; auto a=row[0], b=row[1];
    for(unsigned i=0; i<16; ++i) s.gpr[i]=0xfeed0000+i;
    s.pc=sharp_retro::entry; s.gpr[4]=0x103000; {args}
    m.store(0x100200,a,64); m.store(0x100208,b,64); m.store(s.gpr[4],0xabcdef,64);
    if(sharp_retro::run(s,m,10000)!=sharp_retro::Exit::returned || s.pc!=0xabcdef || s.gpr[4]!=0x103008) return 1;
    for(unsigned i : {{3,5,12,13,14,15}}) if(s.gpr[i]!=0xfeed0000+i) return 2;
    auto p0 = {"a" if name == "arithmetic" else "(unsigned long long)m.load(0x100200,64)"};
    auto p1 = {"b" if name == "arithmetic" else "(unsigned long long)m.load(0x100208,64)"};
    std::printf("%016llx %016llx %016llx\\n",(unsigned long long)s.gpr[0],p0,p1);
}} }}
""")
                self.assertEqual(generated, original.stdout)
                # A changed emitted operation must not survive the native check.
                baseline = header.read_text()
                prefix, executable = baseline.split("inline Exit run(", 1)
                flow = next(line for line in prefix.splitlines() if line.startswith("// Control flow:"))
                self.assertTrue(flow.startswith("// Control flow: structured;"), flow)
                self.assertIn("while(true)", executable)
                mutant = prefix + "inline Exit run(" + executable.replace("return a + b;", "return a - b;")
                self.assertNotEqual(mutant, baseline, "Fixture did not exercise emitted addition")
                header.write_text(mutant)
                result = run("clang++", "-std=c++20", "-O2", "-fsanitize=undefined,address",
                             self.root / "driver.cpp", "-o", self.root / "mutant")
                self.assertEqual(result.returncode, 0, result.stderr)
                wrong = run(self.root / "mutant")
                self.assertTrue(wrong.returncode != 0 or wrong.stdout != original.stdout,
                                "Known incorrect generated operation passed the native comparison")
                # Also check the new transformation's conditional edge selection.
                mutant = prefix + "inline Exit run(" + executable.replace("if(condition_", "if(!condition_", 1)
                self.assertNotEqual(mutant, baseline, "Fixture has no emitted conditional")
                header.write_text(mutant)
                result = run("clang++", "-std=c++20", "-O2", "-fsanitize=undefined,address",
                             self.root / "driver.cpp", "-o", self.root / "mutant")
                self.assertEqual(result.returncode, 0, result.stderr)
                wrong = run(self.root / "mutant")
                self.assertTrue(wrong.returncode != 0 or wrong.stdout != original.stdout,
                                "Reversed structured edge passed the native comparison")


if __name__ == "__main__":
    unittest.main(verbosity=2)
