"""Real local-fopen executable and file effects; separate from upstream b64."""
import importlib.util
from pathlib import Path
import platform
import re
import tempfile
import unittest

import test_pipeline as pipeline

SPEC=importlib.util.spec_from_file_location("stdio_roundtrip",pipeline.ROOT/"Decompiler/examples/stdio/roundtrip.py")
DEMO=importlib.util.module_from_spec(SPEC);SPEC.loader.exec_module(DEMO)


@unittest.skipUnless(platform.system()=="Darwin" and platform.machine()=="arm64","Requires macOS/Apple Silicon/Rosetta")
class StdioApplicationTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp=tempfile.TemporaryDirectory(prefix="sharpretro-stdio-app-")
        cls.artifacts=DEMO.build(Path(cls.temp.name))

    @classmethod
    def tearDownClass(cls): cls.temp.cleanup()

    def test_real_stream_cli_matches_files_and_independent_oracles(self):
        report=DEMO.verify(self.artifacts)
        self.assertEqual(report["case_count"],32)
        self.assertTrue(report["all_match"])
        self.assertTrue(report["mutation_rejected"])
        self.assertGreaterEqual(report["proven_register_identities"],6)
        self.assertGreaterEqual(report["provenance_checked_stdio_calls"],4)

    def test_helpers_have_direct_inputs_without_saved_register_carriers(self):
        text=self.artifacts["recovered_cpp"].read_text()
        entry=self.artifacts["image"]["Entry"]
        declarations=re.findall(r"^static \S+ (fn_[a-f0-9]+)\((.*?)\);$",text,re.M)
        helpers=[arguments for name,arguments in declarations if name!=f"fn_{entry:x}"]
        self.assertEqual(len(helpers),2)
        for arguments in helpers:
            self.assertNotIn("incoming_local_",arguments)
        self.assertNotIn("incoming_local_",text)
        self.assertNotIn("struct result_",text)
        # This caller observes only RAX, so the baseline already has scalar
        # results; its conservative saved-register INPUTS still distinguish it.
        checked=self.artifacts["checked_cpp"].read_text()
        self.assertIn("incoming_local_",checked)
        self.assertIn("CallFrame& call",checked)
