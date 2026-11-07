"""End-to-end executable recovery; no original source is compiled into ARM64."""
import importlib.util
import json
from pathlib import Path
import platform
import struct
import sys
import tempfile
import unittest

import test_pipeline as pipeline

SPEC = importlib.util.spec_from_file_location("cli_roundtrip", pipeline.ROOT / "Decompiler/examples/cli/roundtrip.py")
DEMO = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = DEMO
SPEC.loader.exec_module(DEMO)


@unittest.skipUnless(platform.system() == "Darwin" and platform.machine() == "arm64",
                     "Executable round-trip requires Apple Silicon macOS/Rosetta")
class CliApplicationTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix="sharpretro-cli-")
        cls.root = Path(cls.temp.name)
        cls.artifacts = DEMO.build(cls.root)

    @classmethod
    def tearDownClass(cls):
        cls.temp.cleanup()

    def test_actual_executable_roundtrip_and_sanitizers(self):
        report = DEMO.verify(self.artifacts)
        self.assertEqual(report["architectures"], {"original": "x86_64", "recovered": "arm64", "sanitized": "arm64", "machine": "arm64", "blocks": "arm64", "checked": "arm64"})
        self.assertTrue(report["all_match"])
        self.assertEqual(report["case_count"], 155)
        self.assertTrue(report["mutation_rejected"])
        self.assertTrue(report["control_flow_mutation_rejected"])
        self.assertEqual(report["structured_control_flow"],
                         {"functions":8,"loops":10,"conditionals":46,"duplicated_blocks":19,"gotos":0})
        source = self.artifacts["cpp"].read_text()
        self.assertNotIn("s.gpr[", source)
        self.assertNotIn("struct State", source)
        self.assertNotIn("memory.load", source)
        self.assertIn("// Recovered source:", source)
        self.assertEqual(len(report["recovered_functions"]),8)
        self.assertNotIn("return_dispatch",source)
        self.assertNotIn("stack_pointer",source)
        self.assertNotIn("read_memory",source)
        self.assertNotIn("write_memory",source)
        self.assertNotIn("instruction_budget",source)
        self.assertNotIn("{ // block",source)
        self.assertNotIn("recovered function fell through",source)
        self.assertEqual(report["source_cleanup"]["empty_branches"],0)
        self.assertNotIn("NativeCallContext",source)
        self.assertFalse(report["native_call_context"])
        self.assertNotIn("CallFrame",source)
        self.assertNotIn("call.depth",source)
        self.assertNotIn("call.frame_bytes",source)
        self.assertNotIn("thread_local",source)
        self.assertNotIn("local_7 = argument_0",source)
        self.assertGreater(report["native_memory"]["typed_stack_fields"],6)
        self.assertNotIn("goto ",source)
        self.assertEqual(source.count("// Control flow: structured;"),8)
        self.assertIn("8 structured functions, 10 loops, 46 conditionals",source)
        self.assertLess(report["source_bytes"]["recovered"], report["source_bytes"]["machine"] // 2)
        for dependency in report["recovered_dependencies"]:
            self.assertNotIn("original", dependency)
            self.assertNotIn("SharpRetro", dependency)
        case = {"argv": ["stats", "17", "5", "99"], "stdin": b""}
        self.assertEqual(DEMO.observe(self.artifacts["recovered"], case), (0, b"count=3 sum=121 min=5 max=99\n", b""))
        self.assertEqual(DEMO.observe(self.artifacts["recovered"], {"argv": ["stats", "-1"], "stdin": b""}),
                         (2, b"", b"invalid arguments\n"))

    def test_function_symbols_are_not_needed_for_recovery(self):
        binary, source, recovered = self.root / "stripped-x86_64", self.root / "stripped.cpp", self.root / "stripped-arm64"
        DEMO.tool("strip", "-x", "-S", "-o", binary, self.artifacts["original"])
        names = DEMO.tool("nm", binary)
        self.assertNotIn("__ZL5parse", names)
        DEMO.tool("dotnet", pipeline.DLL, "decompile-app", binary, "--representation", "recovered", "--output", source)
        DEMO.tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", source, "-o", recovered)
        for case in ({"argv": ["hash", "hello"], "stdin": b""},
                     {"argv": ["seq", str(2**64-1), "32"], "stdin": b""},
                     {"argv": ["hash"], "stdin": bytes(range(256))*3}):
            self.assertEqual(DEMO.observe(recovered, case), DEMO.observe(self.artifacts["original"], case))

    def test_unknown_imports_and_unresolved_targets_refuse(self):
        source, binary = self.root / "unsupported.cpp", self.root / "unsupported-x86_64"
        source.write_text('#include <stdio.h>\nint main() { return puts("not bridged"); }\n')
        DEMO.tool("clang++", *DEMO.X64_FLAGS, source, "-o", binary)
        output = self.root / "must-not-exist.cpp"
        result = pipeline.run("dotnet", pipeline.DLL, "decompile-app", binary, "--output", output)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("No application ABI bridge for import _puts", result.stderr)
        self.assertFalse(output.exists())
        # Explicitly cutting a direct call's callee out of the selected bounds
        # must not produce a successful partial app or a fabricated return value.
        main = self.artifacts["image"]["Entry"]
        result = pipeline.run("dotnet", pipeline.DLL, "decompile-app", self.artifacts["original"],
                              "--range", f"0x{main:x}:848", "--output", output)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Unresolved call/jump target", result.stderr)
        self.assertFalse(output.exists())

    def test_import_stub_bytes_and_binding_are_checked(self):
        image = self.artifacts["image"]
        stub = next(r for r in image["Regions"] if r["MachOType"] == 8)
        raw = bytearray(self.artifacts["original"].read_bytes())
        raw[stub["FileOffset"]] = 0x90
        modified = self.root / "modified-stub"
        modified.write_bytes(raw)
        result = pipeline.run("dotnet", pipeline.DLL, "inspect", modified)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("import stub", result.stderr)
        # Redirect the first stub to the other import's pointer slot; symbol
        # metadata for the first stub must not silently override actual bytes.
        raw = bytearray(self.artifacts["original"].read_bytes())
        displacement = struct.unpack_from("<i", raw, stub["FileOffset"]+2)[0]
        struct.pack_into("<i", raw, stub["FileOffset"]+2, displacement+8)
        modified.write_bytes(raw)
        result = pipeline.run("dotnet", pipeline.DLL, "inspect", modified)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("binding mismatch", result.stderr)

    def test_initialized_writable_data_is_not_treated_as_relocated(self):
        source, binary = self.root / "data.cpp", self.root / "data-x86_64"
        source.write_text('volatile int counter=7; int main() { return ++counter; }\n')
        DEMO.tool("clang++", *DEMO.X64_FLAGS, source, "-o", binary)
        result = pipeline.run("dotnet", pipeline.DLL, "decompile-app", binary)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Initialized writable data", result.stderr)


if __name__ == "__main__":
    unittest.main(verbosity=2)
