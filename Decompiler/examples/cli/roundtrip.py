#!/usr/bin/env python3
"""Compile x86-64 CLI -> decompile binary -> compile standalone ARM64 CLI.

The generated programs receive no original source or binary at runtime. Results
are bounded CLI observations (stdout, stderr, exit status), not an equivalence proof.
"""
import argparse
import base64
import hashlib
import json
import os
from pathlib import Path
import platform
import random
import re
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
DLL = ROOT / "Decompiler/bin/Release/net10.0/Decompiler.dll"
ORIGINAL_SOURCE = Path(__file__).with_name("original.cpp")
X64_FLAGS = ["-arch", "x86_64", "-std=c++20", "-O2", "-fno-builtin", "-fno-vectorize",
             "-fno-slp-vectorize", "-fno-unroll-loops", "-fno-stack-protector",
             "-fomit-frame-pointer", "-fno-exceptions", "-fno-rtti", "-mno-sse", "-mno-sse2"]


def tool(*args):
    result = subprocess.run([str(a) for a in args], capture_output=True, text=True, timeout=60)
    if result.returncode:
        raise RuntimeError(f"Command failed ({result.returncode}): {args}\n{result.stdout}\n{result.stderr}")
    return result.stdout


def digest(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def build(directory):
    if platform.system() != "Darwin" or platform.machine() != "arm64":
        raise RuntimeError("This demonstration requires Apple Silicon macOS and Rosetta for the reference")
    if not DLL.exists():
        raise RuntimeError("Build Decompiler/Decompiler.csproj -c Release first")
    directory = Path(directory).resolve()
    directory.mkdir(parents=True, exist_ok=True)
    original, cpp = directory / "original-x86_64", directory / "recovered.cpp"
    recovered, sanitized = directory / "recovered-arm64", directory / "recovered-arm64-sanitized"
    machine_cpp, machine = directory / "machine.cpp", directory / "machine-arm64"
    blocks_cpp, blocks = directory / "blocks.cpp", directory / "blocks-arm64"
    checked_cpp, checked = directory / "checked.cpp", directory / "checked-arm64"
    tool("clang++", *X64_FLAGS, ORIGINAL_SOURCE, "-o", original)
    info = json.loads(tool("dotnet", DLL, "inspect", original))
    if info["Architecture"] != "X86_64" or set(info["Imports"].values()) != {"_read", "_write"}:
        raise RuntimeError("Reference image architecture/imports differ from this fixture's declared boundary")
    tool("dotnet", DLL, "decompile-app", original, "--representation", "recovered", "--control-flow", "structured", "--memory", "native", "--output", cpp)
    tool("dotnet", DLL, "decompile-app", original, "--representation", "recovered", "--control-flow", "structured", "--memory", "checked", "--output", checked_cpp)
    tool("dotnet", DLL, "decompile-app", original, "--representation", "machine", "--output", machine_cpp)
    tool("dotnet", DLL, "decompile-app", original, "--representation", "blocks", "--output", blocks_cpp)
    # This invocation sees ONLY the recovered C++ translation unit, never the
    # original source, object, executable or x86 library.
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", cpp, "-o", recovered)
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O1", "-fsanitize=undefined,address", cpp, "-o", sanitized)
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", machine_cpp, "-o", machine)
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", blocks_cpp, "-o", blocks)
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", checked_cpp, "-o", checked)
    return {"directory": directory, "original": original, "cpp": cpp,
            "recovered": recovered, "sanitized": sanitized, "machine": machine,
            "machine_cpp": machine_cpp, "blocks": blocks, "blocks_cpp": blocks_cpp,
            "checked": checked, "checked_cpp": checked_cpp, "image": info}


def cases():
    result = []
    def add(args, data=b"", close_stdin=False):
        result.append({"id": f"cli-{len(result):03}", "argv": args, "stdin": data, "close_stdin": close_stdin})
    for args in ([], ["help"], ["unknown"], ["hash", "one", "two"], ["stats"], ["seq"],
                 ["seq", "1"], ["seq", "1", "33"], ["seq", "1", "-1"], ["seq", "x", "3"]):
        add(args)
    for value in ("", "hello", "Dragon Age", "猫 🦊", "a\nb\t'\"", "x" * 4096):
        add(["hash", value])
    for data in (b"", b"hello", bytes(range(256)), b"a\0b\xff\n", bytes(range(256)) * 16):
        add(["hash"], data)
    add(["hash"], close_stdin=True)
    for numbers in (("0",), ("17", "5", "99"), (str(2**64-1),),
                    (str(2**64-1), "1"), ("000000", "00012", "0007")):
        add(["stats", *numbers])
    for value in ("", "-1", "+1", "12x", str(2**64), "9" * 100, "１２３"):
        add(["stats", value])
    for seed in ("0", "1", str(2**64-1)):
        for count in ("0", "1", "32"):
            add(["seq", seed, count])
    rng = random.Random(0xDA0C11)
    for _ in range(32):
        add(["hash", "".join(chr(rng.randrange(32, 127)) for _ in range(rng.randrange(128)))])
        add(["stats", *(str(rng.getrandbits(64)) for _ in range(rng.randrange(1, 13)))])
        add(["seq", str(rng.getrandbits(64)), str(rng.randrange(33))])
    for _ in range(16):
        add(["hash"], rng.randbytes(rng.randrange(2049)))
    return result


def observe(executable, case, cwd=None):
    kwargs = {"capture_output": True, "timeout": 30, "cwd": cwd}
    if case.get("close_stdin"):
        kwargs.update(stdin=subprocess.DEVNULL, preexec_fn=lambda: os.close(0))
    else:
        kwargs["input"] = case["stdin"]
    proc = subprocess.run([str(executable), *case["argv"]], **kwargs)
    return proc.returncode, proc.stdout, proc.stderr


def encoded(observation):
    return {"exit": observation[0], "stdout_base64": base64.b64encode(observation[1]).decode(),
            "stderr_base64": base64.b64encode(observation[2]).decode()}


def verify(artifacts):
    directory = artifacts["directory"]
    isolated = directory / "isolated"
    isolated.mkdir(exist_ok=True)
    runnable = isolated / "recovered"
    shutil.copy2(artifacts["recovered"], runnable)
    rows = []
    corpus = cases()
    for case in corpus:
        expected = observe(artifacts["original"], case)
        actual = observe(runnable, case, isolated)
        checked = observe(artifacts["sanitized"], case, isolated)
        baseline = observe(artifacts["machine"], case, isolated)
        flat = observe(artifacts["blocks"], case, isolated)
        checked_memory = observe(artifacts["checked"], case, isolated)
        row = {"id": case["id"], "argv": case["argv"], "stdin_sha256": hashlib.sha256(case["stdin"]).hexdigest(),
               "close_stdin": case["close_stdin"], "matches": all(x==expected for x in (actual,checked,baseline,flat,checked_memory)),
               "exit": expected[0], "stdout_sha256": hashlib.sha256(expected[1]).hexdigest(),
               "stderr_sha256": hashlib.sha256(expected[2]).hexdigest()}
        if not row["matches"]:
            row.update(original=encoded(expected), recovered=encoded(actual), sanitized=encoded(checked), machine=encoded(baseline), blocks=encoded(flat), checked_memory=encoded(checked_memory),
                       stdin_base64=base64.b64encode(case["stdin"]).decode())
            (directory / "failure.json").write_text(json.dumps(row, indent=2))
            raise AssertionError(f"CLI mismatch: {case['id']} {case['argv']}; see {directory / 'failure.json'}")
        rows.append(row)
    # A known semantic change in the translated application must fail comparison.
    source = artifacts["cpp"].read_text()
    prefix, body = source.split("// Recovered function bodies", 1)
    changed = prefix + "// Recovered function bodies" + body.replace("^", "|")
    if changed == source:
        raise AssertionError("No translated XOR operation available for mutation control")
    mutant_source, mutant = directory / "mutant.cpp", directory / "mutant-arm64"
    mutant_source.write_text(changed)
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", mutant_source, "-o", mutant)
    control = {"argv": ["hash", "hello"], "stdin": b""}
    rejected = observe(mutant, control) != observe(artifacts["original"], control)
    if not rejected:
        raise AssertionError("Changed translated arithmetic survived the CLI comparison")
    # Also check a control-flow mutation, independently of arithmetic recovery.
    reversed_source, reversed_exe = directory / "reversed-flow.cpp", directory / "reversed-flow-arm64"
    prefix,body=source.split("// Recovered function bodies",1)
    if "if(" not in body:
        raise AssertionError("No structured conditional available for mutation control")
    # Predicate forwarding removes saved-flag spellings. Negate the actual
    # first recovered test, keeping nested casts/parentheses intact and leaving
    # runtime helper conditions outside the mutation's scope.
    start=body.index("if(")+3
    depth=1;end=start
    while depth:
        if body[end]=='(': depth+=1
        elif body[end]==')': depth-=1
        end+=1
    reversed_text=prefix+"// Recovered function bodies"+body[:start]+"!("+body[start:end-1]+")"+body[end-1:]
    reversed_source.write_text(reversed_text)
    tool("clang++", "-arch", "arm64", "-std=c++20", "-O2", reversed_source, "-o", reversed_exe)
    flow_rejected = observe(reversed_exe, control) != observe(artifacts["original"], control)
    if not flow_rejected:
        raise AssertionError("Reversed structured branch survived the CLI comparison")
    regions = re.findall(r"// Control flow: structured; (\d+) loops, (\d+) conditionals, (\d+) duplicated blocks", source)
    report = {
        "host": {"system": platform.platform(), "machine": platform.machine(),
                 "clang": tool("clang++", "--version").splitlines()[0], "dotnet": tool("dotnet", "--version").strip()},
        "reference_execution": "x86_64 through Rosetta", "recovered_execution": "native arm64",
        "input_sha256": digest(artifacts["original"]), "original_source_sha256": digest(ORIGINAL_SOURCE),
        "generated_cpp_sha256": digest(artifacts["cpp"]), "recovered_sha256": digest(artifacts["recovered"]),
        "sanitized_sha256": digest(artifacts["sanitized"]), "x64_flags": X64_FLAGS,
        "imports": artifacts["image"]["Imports"],
        "architectures": {key: tool("lipo", "-archs", artifacts[key]).strip() for key in ("original", "recovered", "sanitized", "machine", "blocks", "checked")},
        "source_bytes": {"machine": artifacts["machine_cpp"].stat().st_size, "recovered": artifacts["cpp"].stat().st_size, "blocks": artifacts["blocks_cpp"].stat().st_size, "checked": artifacts["checked_cpp"].stat().st_size},
        "source_cleanup": {"lines":len(source.splitlines()),
                           "instruction_budget":"instruction_budget" in source,
                           "block_wrappers":source.count("{ // block"),
                           "empty_branches":len(re.findall(r"(?:if\([^\n]*\)|else)\s*\{\s*\}",source)),
                           "standalone_scopes":len(re.findall(r"^\s*\{\s*$",source,re.MULTILINE))},
        "native_memory": {"frames": len(re.findall(r"struct(?:\s+alignas\(\d+\))?\s+FrameStorage\s*\{",source)),
                          "typed_stack_fields": int(re.search(r"native objects, (\d+) typed stack fields",source)[1]),
                          "emulated_stack_pointer": "stack_pointer" in source,
                          "address_helpers": "read_memory<" in source or "write_memory<" in source},
        "native_call_context": "NativeCallContext" in source or "CallFrame" in source,
        "recovered_functions": sorted(set(re.findall(r"^static \S+ (fn_[0-9a-f]+)\(", source, re.MULTILINE))),
        "structured_control_flow": {"functions": len(regions),
                                    "loops": sum(int(r[0]) for r in regions),
                                    "conditionals": sum(int(r[1]) for r in regions),
                                    "duplicated_blocks": sum(int(r[2]) for r in regions),
                                    "gotos": len(re.findall(r"\bgoto\s+", source))},
        "blocks_source_sha256": digest(artifacts["blocks_cpp"]),
        "machine_source_sha256": digest(artifacts["machine_cpp"]),
        "recovered_dependencies": tool("otool", "-L", artifacts["recovered"]).splitlines()[1:],
        "case_count": len(rows), "all_match": True, "sanitizers": ["address", "undefined"],
        "mutation_rejected": rejected, "control_flow_mutation_rejected": flow_rejected, "cases": rows,
        "scope": "Bounded stdout/stderr/exit comparisons, not universal equivalence or a general macOS runtime"
    }
    (directory / "report.json").write_text(json.dumps(report, indent=2) + "\n")
    return report


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--build-dir", type=Path, default=Path(__file__).with_name("build"))
    args = parser.parse_args()
    artifacts = build(args.build_dir)
    report = verify(artifacts)
    print(f"Matched {report['case_count']} CLI cases on ARM64 and ASan/UBSan; arithmetic and branch mutations rejected.")
    print(f"ARM64 executable: {artifacts['recovered']}")
    print(f"Generated C++: {artifacts['cpp']}")
    print(f"Report: {artifacts['directory'] / 'report.json'}")


if __name__ == "__main__":
    main()
