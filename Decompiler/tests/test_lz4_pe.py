"""Published Windows x64 DLL bytes compared directly with recovered ARM64."""
import importlib.util
import json
from pathlib import Path
import platform
import struct
import tempfile
import unittest

import test_pipeline as pipeline

SPEC=importlib.util.spec_from_file_location("lz4_pe_roundtrip",pipeline.ROOT/"Decompiler/examples/lz4-pe/roundtrip.py")
DEMO=importlib.util.module_from_spec(SPEC);SPEC.loader.exec_module(DEMO)


class MutationSpellingTests(unittest.TestCase):
    def test_mutation_changes_only_recovered_shift_operations(self):
        source='x=shift_right(a,4ULL,32); y=a>>4ULL; z=box<box<int>>(); "a>>4ULL shift_right(x)"; /* a>>4ULL */ // shift_right(x)\n'
        expected='x=shift_left(a,4ULL,32); y=a<<4ULL; z=box<box<int>>(); "a>>4ULL shift_right(x)"; /* a>>4ULL */ // shift_right(x)\n'
        self.assertEqual(DEMO.mutate_decoder_shifts(source),expected)


@unittest.skipUnless(platform.system()=="Darwin" and platform.machine()=="arm64" and
                    (DEMO.CACHE/"lz4_v1_8_3_win64.zip").exists(),"Requires Apple Silicon/Rosetta and pinned archive; run lz4-pe/roundtrip.py")
class PublishedPeTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp=tempfile.TemporaryDirectory(prefix="sharpretro-published-pe-")
        cls.root=Path(cls.temp.name);cls.artifacts=DEMO.build(cls.root)

    @classmethod
    def tearDownClass(cls): cls.temp.cleanup()

    def test_actual_published_decoder_matches_complete_outputs_and_errors(self):
        report=DEMO.verify(self.artifacts)
        self.assertEqual(report["case_count"],509)
        self.assertTrue(report["all_match"]);self.assertTrue(report["mutation_rejected"])
        self.assertEqual(report["input_sha256"],"1731d54f5ec77effa24beda1075105395aa9b9ff0a042a618a17204253c8a960")
        self.assertEqual(report["export_va"],0x61493c20)
        self.assertIn("451 reachable instructions",report["diagnostics"])
        self.assertTrue(self.artifacts["image"]["PeHasTls"]) # Deliberately not initialized by this isolated closure runner.
        body=self.artifacts["cpp"].read_text().split("// Recovered function bodies",1)[1]
        self.assertNotIn("[[maybe_unused]]",body)
        self.assertIn("const u8* argument_0",body)
        self.assertGreater(body.index("auto shared_region_"),body.index("return "))

    def test_reference_loader_refuses_bad_images_and_unsupported_calls(self):
        original=self.artifacts["pe"].read_bytes()
        pe=struct.unpack_from("<I",original,60)[0];opt=pe+24
        n=struct.unpack_from("<H",original,pe+6)[0];size=struct.unpack_from("<H",original,pe+20)[0]
        sections=[struct.unpack_from("<IIII",original,opt+size+i*40+8) for i in range(n)]
        def offset(rva):
            for virtual,address,raw,file in sections:
                if address<=rva<address+min(virtual,raw): return file+rva-address
            raise AssertionError("unmapped test RVA")
        export,size=struct.unpack_from("<II",original,opt+112)
        e=offset(export);ordinal_base=struct.unpack_from("<I",original,e+16)[0]
        table=struct.unpack_from("<I",original,e+28)[0]
        ordinal=self.artifacts["image"]["Exports"]["LZ4_decompress_safe"]["Ordinal"]-ordinal_base
        variants=[(bytearray(original[:100]),125)]
        forwarded=bytearray(original);struct.pack_into("<I",forwarded,offset(table)+ordinal*4,export+0x100);variants.append((forwarded,125))
        data_export=bytearray(original);struct.pack_into("<I",data_export,offset(table)+ordinal*4,0x2d000);variants.append((data_export,125))
        small=bytearray(original);struct.pack_into("<I",small,opt+56,0x1000);variants.append((small,125))
        unknown=bytearray(original);where=unknown.index(b"memcpy\0");unknown[where:where+7]=b"memfoo\0";variants.append((unknown,126))
        for i,(raw,status) in enumerate(variants):
            with self.subTest(variant=i):
                path=self.root/f"bad-{i}.dll";path.write_bytes(raw)
                p=pipeline.run(self.artifacts["reference"],path,"decompress","5068656c6c6f",5)
                self.assertEqual(p.returncode,status,p.stderr)
                self.assertNotIn('"return"',p.stdout)
        fallback=bytearray(original);imports,imsize=struct.unpack_from("<II",original,opt+120)
        for delta in range(0,imsize-19,20):
            at=offset(imports+delta)
            if not any(fallback[at:at+20]): break
            struct.pack_into("<I",fallback,at,0)
        path=self.root/"fallback.dll";path.write_bytes(fallback)
        p=pipeline.run(self.artifacts["reference"],path,"decompress","5068656c6c6f",5)
        self.assertEqual(p.returncode,0,p.stderr)
        result=json.loads(p.stdout);self.assertEqual((result["return"],result["output"]),(5,"68656c6c6f"))
        self.assertTrue(result["image_unchanged"]);self.assertTrue(result["canaries_ok"])
