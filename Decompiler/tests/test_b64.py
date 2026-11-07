"""Unmodified upstream CLI, classic dyld metadata, stdio and real file effects."""
import importlib.util
from pathlib import Path
import platform
import struct
import sys
import tempfile
import unittest

import test_pipeline as pipeline

SPEC=importlib.util.spec_from_file_location("b64_roundtrip",pipeline.ROOT/"Decompiler/examples/b64/roundtrip.py")
DEMO=importlib.util.module_from_spec(SPEC); sys.modules[SPEC.name]=DEMO; SPEC.loader.exec_module(DEMO)


class MutationSpellingTests(unittest.TestCase):
    def test_native_and_helper_shift_mutations_preserve_literals(self):
        source='a=shift_left(x,2ULL,64); b=x<<2ULL; "x<<2ULL shift_left(x)"; // x<<2ULL\n'
        expected='a=shift_right(x,2ULL,64); b=x>>2ULL; "x<<2ULL shift_left(x)"; // x<<2ULL\n'
        self.assertEqual(DEMO.mutate_codec_shifts(source),expected)


@unittest.skipUnless(platform.system()=="Darwin" and platform.machine()=="arm64","Requires macOS/Apple Silicon/Rosetta")
class B64IntegrationTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp=tempfile.TemporaryDirectory(prefix="sharpretro-upstream-b64-")
        cls.root=Path(cls.temp.name)
        cls.artifacts=DEMO.build(cls.root)

    @classmethod
    def tearDownClass(cls): cls.temp.cleanup()

    def test_whole_upstream_tool_matches_binary_files_options_and_failures(self):
        report=DEMO.verify(self.artifacts)
        self.assertEqual(report["case_count"],292)
        self.assertTrue(report["all_match"])
        self.assertTrue(report["mutation_rejected"])
        self.assertEqual(report["architectures"],{"original":"x86_64","recovered":"arm64","sanitized":"arm64","checked":"arm64"})
        self.assertEqual(report["upstream"]["commit"],"20b0bf244e785443ae27bea06dc6f67b4639d880")
        self.assertGreater(report["instructions"],500)
        self.assertEqual(report["gotos"],0)
        self.assertFalse(report["native_context"])
        self.assertGreaterEqual(report["functions"],4)

    def test_malformed_fixups_and_conflicting_stub_binding_refuse(self):
        raw=self.artifacts["original"].read_bytes()
        cursor=32; info=None
        for _ in range(struct.unpack_from("<I",raw,16)[0]):
            command,size=struct.unpack_from("<II",raw,cursor)
            if command in (0x22,0x80000022): info=cursor
            cursor+=size
        self.assertIsNotNone(info)
        variants=[]
        truncated=bytearray(raw)
        stream=struct.unpack_from("<I",truncated,info+8)[0]
        struct.pack_into("<I",truncated,info+12,1); truncated[stream]=0x20
        variants.append((truncated,"Truncated Mach-O fixup ULEB128"))
        conflict=bytearray(raw)
        start,size=struct.unpack_from("<II",conflict,info+32)
        index=conflict[start:start+size].index(b"_strcmp\0")+start
        conflict[index:index+8]=b"_malloc\0"
        variants.append((conflict,"stub/dyld binding mismatch"))
        outside=bytearray(raw)
        slot=next(int(a) for a in self.artifacts["image"]["Rebases"] if a not in self.artifacts["image"]["Bindings"])
        region=next(r for r in self.artifacts["image"]["Regions"] if r["Address"]<=slot<r["Address"]+r["Size"])
        struct.pack_into("<Q",outside,region["FileOffset"]+slot-region["Address"],0xdeadbeef000)
        variants.append((outside,"outside emitted data"))
        mixed=bytearray(raw); cursor=32
        for _ in range(struct.unpack_from("<I",mixed,16)[0]):
            command,size=struct.unpack_from("<II",mixed,cursor)
            if command==0x26:
                struct.pack_into("<I",mixed,cursor,0x80000034); break
            cursor+=size
        variants.append((mixed,"Mixed classic and chained"))
        for i,(data,message) in enumerate(variants):
            with self.subTest(error=message):
                image=self.root/f"bad-{i}"; image.write_bytes(data)
                target=self.root/f"must-not-exist-{i}.cpp"
                result=pipeline.run("dotnet",pipeline.DLL,"decompile-app",image,"--memory","native","--output",target)
                self.assertEqual(result.returncode,1,result.stderr)
                self.assertIn(message,result.stderr)
                self.assertFalse(target.exists())

    def test_upstream_bad_stdin_exit_quirk_is_retained(self):
        case={"argv":["-d"],"stdin":b"!","files":{}}
        reference=DEMO.observe(self.artifacts["original"],case,self.root/"stdin-reference")
        recovered=DEMO.observe(self.artifacts["recovered"],case,self.root/"stdin-native")
        self.assertEqual(reference,recovered)
        self.assertEqual(reference[0],0)
        self.assertIn(b"Invalid base64 character",reference[2])
