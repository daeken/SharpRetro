"""Native frame objects and aliasing, with checked-memory fallback retained."""
import platform
import unittest

import test_pipeline as pipeline
import test_values as values
from test_functions import Code, with_starts


class NativeMemoryTests(pipeline.PipelineCase):
    execute = values.ValueRecoveryTests.execute
    driver = values.ValueRecoveryTests.driver

    def recover(self, code, memory="native", starts=None):
        self.image.write_bytes(pipeline.macho(code) if starts is None else with_starts(code, starts))
        source = self.root / "values.cpp"
        result = self.cli("decompile-app", self.image, "--memory", memory, "--output", source)
        self.assertEqual(result.returncode,0,result.stderr)
        text=source.read_text()
        if memory=="native":
            for old in ("stack_pointer", "return_address", "read_memory", "write_memory", "accessible", "expose(","NativeCallContext","CallFrame"):
                self.assertNotIn(old,text)
        return source

    def test_typed_field_and_byte_pointer_alias_across_native_call(self):
        code=(Code().emit("4883ec10 48b88877665544332211 48890424 4889e6").call("change")
              .emit("488b0424 48c1e808 4883c410 c3").label("change").emit("c64601aa c3"))
        source=self.recover(code.finish())
        self.assertIn("std::uint64_t stack_local_10;",source.read_text())
        self.assertRegex(source.read_text(),r"=\s*frame\.stack_local_10;")
        self.assertEqual(self.execute(source).returncode,0xaa)

    def test_overlapping_unaligned_views_keep_ordered_bytes(self):
        code=bytes.fromhex("4883ec20 48b88877665544332211 4889442401 66c7442402aa99 488b442401 48c1e810 4883c420 c3")
        source=self.recover(code)
        self.assertIn("reinterpret_cast<const u64*>",source.read_text())
        self.assertEqual(self.execute(source).returncode,0x99)

    def test_direct_alias_views_at_every_offset_under_strict_optimization(self):
        driver=self.root/"direct-alias.cpp"
        driver.write_text(f'#include "{pipeline.ROOT}/Decompiler/IntegerRuntime.hpp"\n'
                          f'#include "{pipeline.ROOT}/Decompiler/NativeObjectsRuntime.hpp"\n'+r'''
using namespace sharp_retro::source;
using sharp_retro::bits;
struct Pair { bits before,after; };
#define DEFINE_VIEW(T) \
__attribute__((noinline)) Pair mutate_##T(T* wide,u32* narrow,bits value) { \
    *wide=static_cast<T>(value); auto before=*wide; \
    *narrow=0x89abcdefU; return {before,*wide}; \
}
DEFINE_VIEW(u8)
DEFINE_VIEW(u16)
DEFINE_VIEW(u32)
DEFINE_VIEW(u64)
DEFINE_VIEW(u128)
int main() {
    const bits pattern=(bits(0x1122334455667788ULL)<<64)|0xfedcba9876543210ULL;
#define CHECK_VIEW(T,N) \
    for(unsigned offset=0;offset<32;++offset) { \
        alignas(16) std::array<unsigned char,64> data{},expected{}; \
        data.fill(0xa5); expected=data; \
        for(unsigned i=0;i<N;++i) expected[offset+i]=unsigned(pattern>>(8*i)); \
        for(unsigned i=0;i<4;++i) expected[offset+1+i]=unsigned(0x89abcdefU>>(8*i)); \
        auto pair=mutate_##T(reinterpret_cast<T*>(data.data()+offset),reinterpret_cast<u32*>(data.data()+offset+1),pattern); \
        bits wanted=0; for(unsigned i=0;i<N;++i) wanted|=bits(expected[offset+i])<<(8*i); \
        if(pair.before!=(pattern&sharp_retro::mask(N*8)) || pair.after!=wanted || data!=expected) return 1; \
    }
    CHECK_VIEW(u8,1)
    CHECK_VIEW(u16,2)
    CHECK_VIEW(u32,4)
    CHECK_VIEW(u64,8)
    CHECK_VIEW(u128,16)
}
''')
        exe=self.root/"direct-alias"
        built=pipeline.run("clang++","-std=c++20","-O3","-fstrict-aliasing","-Wall","-Wextra","-Werror",
                           "-fsanitize=address,undefined",driver,"-o",exe)
        self.assertEqual(built.returncode,0,built.stderr)
        result=pipeline.run(exe)
        self.assertEqual((result.returncode,result.stderr),(0,""))

    def test_recursive_frames_are_independent_native_objects(self):
        code=(Code().call("sum").emit("c3").label("sum").emit("85ff").branch("74","base")
              .emit("4883ec08 893c24 ffcf").call("sum")
              .emit("030424 4883c408 c3").label("base").emit("31c0 c3"))
        source=self.recover(code.finish())
        self.driver(source,"""
int main() {
  for(int n=0;n<80;++n)
    if(sharp_retro::source::recovered_program(n,nullptr)!=((n*(n+1)/2)&255)) return 1;
}
""")

    def test_ret_cleanup_through_metadata_tail_function(self):
        code=(Code().emit("4883ec08").call("first").emit("c3").label("first")
              .branch("e9","second",4).label("second").emit("b82a000000 c20800"))
        source=self.recover(code.finish(),starts=[0,code.labels["first"],code.labels["second"]])
        self.assertEqual(self.execute(source).returncode,42)

    def test_noncanonical_layouts_fallback_or_refuse_before_publication(self):
        differing=(Code().emit("85ff").branch("74","join").emit("4883ec08")
                   .label("join").emit("31c0 c3").finish())
        fixtures={
            "return slot read": bytes.fromhex("488b0424 c3"),
            "incoming argument": bytes.fromhex("488b442408 c3"),
            "dynamic allocation": bytes.fromhex("4829fc 4801fc 31c0 c3"),
            "frame escape": bytes.fromhex("488d4424f8 c3"),
            "unrestored stack": bytes.fromhex("4883ec08 31c0 c3"),
            "different join heights": differing,
            "return slot address": Code().emit("4889e7").call("helper").emit("c3").label("helper").emit("0fb607c3").finish(),
        }
        for name,code in fixtures.items():
            with self.subTest(shape=name):
                source=self.recover(code,"auto")
                self.assertIn("checked memory fallback:",source.read_text())
                target=self.root/"must-not-exist.cpp"
                result=self.cli("decompile-app",self.image,"--memory","native","--output",target)
                self.assertNotEqual(result.returncode,0)
                self.assertIn("Cannot reconstruct native memory",result.stderr)
                self.assertFalse(target.exists())

    def test_explicit_checked_mode_keeps_return_slot_checks(self):
        code=(Code().call("helper").emit("c3").label("helper")
              .emit("48c7042400100000 b807000000 c3"))
        source=self.recover(code.finish(),"checked")
        self.assertIn("checked address layer",source.read_text())
        executed=self.execute(source)
        self.assertEqual(executed.returncode,125)
        self.assertIn("noncanonical recovered call return",executed.stderr)

    @unittest.skipUnless(platform.system()=="Darwin" and platform.machine()=="arm64","Mach-O/Rosetta reference")
    def test_clang_native_aliasing_fixture_against_x64(self):
        import test_cli_app as cli
        original_source=self.root/"original.cpp"
        original_source.write_text('''
#include <stdint.h>
__attribute__((noinline)) void mutate(unsigned char* p,unsigned n) {
    for(unsigned i=0;i<n;++i) p[i]=(unsigned char)(p[i]+i*7+3);
}
__attribute__((noinline)) uint64_t mix(uint64_t* p,unsigned n) {
    mutate((unsigned char*)p,n&7);
    return *p;
}
int main(int argc,char**) {
    uint64_t value=0x1122334455667788ULL;
    auto changed=mix(&value,(unsigned)argc);
    return int((changed ^ (changed>>8) ^ (value>>32))&255);
}
''')
        original=self.root/"alias-x86_64"
        cli.DEMO.tool("clang++",*cli.DEMO.X64_FLAGS,original_source,"-o",original)
        source=self.root/"values.cpp"
        cli.DEMO.tool("dotnet",pipeline.DLL,"decompile-app",original,"--memory","native","--output",source)
        recovered=self.root/"alias-arm64"
        cli.DEMO.tool("clang++","-arch","arm64","-std=c++20","-O2","-fsanitize=address,undefined",source,"-o",recovered)
        self.assertNotIn("stack_pointer",source.read_text())
        for count in range(1,25):
            case={"argv":["x"]*(count-1),"stdin":b""}
            self.assertEqual(cli.DEMO.observe(recovered,case),cli.DEMO.observe(original,case))
