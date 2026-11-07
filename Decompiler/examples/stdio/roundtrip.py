#!/usr/bin/env python3
"""Local fopen provenance: x64 CLI -> C++ -> native ARM64 with real file effects."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path
import platform
import random
import re
import shutil
import subprocess
import sys

HERE=Path(__file__).resolve().parent
ROOT=HERE.parents[2]
DLL=ROOT/"Decompiler/bin/Release/net10.0/Decompiler.dll"
SPEC=importlib.util.spec_from_file_location("stdio_b64_helpers",HERE.parent/"b64/roundtrip.py")
HELPERS=importlib.util.module_from_spec(SPEC); SPEC.loader.exec_module(HELPERS)
FLAGS=["-arch","x86_64","-std=c++20","-O2","-fno-builtin","-fno-vectorize",
       "-fno-slp-vectorize","-fno-unroll-loops","-fno-stack-protector","-fomit-frame-pointer",
       "-fno-exceptions","-fno-rtti","-mno-sse","-mno-sse2","-Wl,-no_fixup_chains"]


def build(directory):
    if platform.system()!="Darwin" or platform.machine()!="arm64":
        raise RuntimeError("Requires Apple Silicon macOS and Rosetta")
    directory=Path(directory).resolve(); directory.mkdir(parents=True,exist_ok=True)
    original=directory/"stdio-x86_64"
    HELPERS.tool("clang++",*FLAGS,HERE/"original.cpp","-o",original)
    image=json.loads(HELPERS.tool("dotnet",DLL,"inspect",original))
    if set(image["Imports"].values())!={"_fopen","_fclose","_fread","_fwrite"}:
        raise AssertionError("Unexpected reference import boundary")
    result={"directory":directory,"original":original,"image":image,"diagnostics":[]}
    for name,memory in (("recovered","native"),("checked","checked")):
        cpp=directory/f"{name}.cpp"; exe=directory/f"{name}-arm64"
        HELPERS.tool("dotnet",DLL,"decompile-app",original,"--memory",memory,"--control-flow","structured","--output",cpp,diagnostics=result["diagnostics"])
        HELPERS.tool("clang++","-arch","arm64","-std=c++20","-O2",cpp,"-o",exe)
        result[name]=exe; result[name+"_cpp"]=cpp
    result["sanitized"]=directory/"sanitized-arm64"
    HELPERS.tool("clang++","-arch","arm64","-std=c++20","-O1","-fsanitize=address,undefined",result["recovered_cpp"],"-o",result["sanitized"])
    return result


def cases():
    result=[]
    def add(args,files=None,expected=None,expected_files=None):
        result.append({"id":f"stdio-{len(result):03}","argv":args,"files":files or {},
                       "expected":expected,"expected_files":expected_files})
    for args,status in (([],120),(["r"],120),(["unknown","x"],121),(["r","absent"],117),
                        (["w","absent-dir/output.bin"],117)):
        add(args,expected=status,expected_files={})
    block=(3^0x8000ff61).to_bytes(4,"little")*17
    add(["w","output.bin"],expected=0,expected_files={"output.bin":block})
    add(["w","output.bin"],files={"output.bin":b"old contents"},expected=0,expected_files={"output.bin":block})
    rng=random.Random(0x57D10)
    for n in (0,1,2,3,31,32,33,255,256,257,1024,4096,65536):
        data=rng.randbytes(n)
        add(["r","input.bin"],files={"input.bin":data},expected=sum(data)&127,expected_files={"input.bin":data})
    for _ in range(12):
        data=rng.randbytes(rng.randrange(2048))
        add(["r","input.bin"],files={"input.bin":data},expected=sum(data)&127,expected_files={"input.bin":data})
    return result


def verify(artifacts):
    directory=artifacts["directory"]
    isolated=directory/"isolated"; isolated.mkdir(exist_ok=True)
    standalone=isolated/"stdio"; shutil.copy2(artifacts["recovered"],standalone)
    rows=[]
    for case in cases():
        observations=[]
        for name,exe in (("original",artifacts["original"]),("recovered",standalone),
                         ("sanitized",artifacts["sanitized"]),("checked",artifacts["checked"])):
            root=directory/"runs"/case["id"]/name
            if root.exists(): shutil.rmtree(root)
            root.mkdir(parents=True)
            for filename,data in case["files"].items():
                path=root/filename; path.parent.mkdir(parents=True,exist_ok=True); path.write_bytes(data)
            p=subprocess.run([str(exe),*case["argv"]],input=b"",cwd=root,capture_output=True,timeout=30)
            files={str(path.relative_to(root)):path.read_bytes() for path in root.rglob("*") if path.is_file()}
            observations.append((p.returncode,p.stdout,p.stderr,files))
        if not all(item==observations[0] for item in observations):
            raise AssertionError(f"Executable mismatch: {case['id']}")
        expected=(case["expected"],b"",b"",case["expected_files"])
        if observations[0]!=expected: raise AssertionError(f"Independent oracle mismatch: {case['id']}")
        rows.append({"id":case["id"],"exit":case["expected"],"matches":True})
    raw=artifacts["recovered_cpp"].read_bytes(); text=raw.decode()
    prefix,body=text.split("// Recovered function bodies",1)
    # Clang specializes the local seed (argc==3), folding the XOR into this
    # word. Mutate its emitted value, keeping the call/file protocol intact.
    mutated=prefix+"// Recovered function bodies"+body.replace("0x8000ff62ULL","0x8000ff63ULL")
    if mutated==text: raise AssertionError("No pattern arithmetic available for mutation")
    mutant_cpp=directory/"mutant.cpp"; mutant_cpp.write_text(mutated)
    mutant=directory/"mutant-arm64"
    HELPERS.tool("clang++","-arch","arm64","-std=c++20","-O2",mutant_cpp,"-o",mutant)
    mutation_dir=directory/"mutation"; mutation_dir.mkdir(exist_ok=True)
    p=subprocess.run([str(mutant),"w","output.bin"],cwd=mutation_dir,capture_output=True,timeout=30)
    if p.returncode!=0 or (mutation_dir/"output.bin").read_bytes()==(3^0x8000ff61).to_bytes(4,"little")*17:
        raise AssertionError("Mutation control did not produce the intended file mismatch")
    identities=int(re.search(r"(\d+) proven register identities",text)[1])
    stdio=int(re.search(r"(\d+) provenance-checked stdio calls",text)[1])
    if identities==0 or stdio<4: raise AssertionError("Fixture failed to exercise stream-dependent contraction")
    return {"scope":"Synthetic local-stream fixture; bounded comparisons, not a universal proof",
            "host":{"system":platform.platform(),"machine":platform.machine(),
                    "clang":HELPERS.tool("clang++","--version").splitlines()[0],"dotnet":HELPERS.tool("dotnet","--version").strip()},
            "source_sha256":HELPERS.digest(HERE/"original.cpp"),"input_sha256":artifacts["image"]["Sha256"],
            "generated_cpp_sha256":hashlib.sha256(raw).hexdigest(),"x64_flags":FLAGS,
            "case_count":len(rows),"all_match":True,"mutation_rejected":True,"cases":rows,
            "lines":len(text.splitlines()),"proven_register_identities":identities,"provenance_checked_stdio_calls":stdio,
            "architectures":{"original":"x86_64","recovered":"arm64","sanitized":"arm64","checked":"arm64"},
            "diagnostics":artifacts["diagnostics"]}


def main():
    parser=argparse.ArgumentParser();parser.add_argument("--build-dir",type=Path,default=HERE/"build")
    args=parser.parse_args(); artifacts=build(args.build_dir); report=verify(artifacts)
    (args.build_dir/"report.json").write_text(json.dumps(report,indent=2)+"\n")
    print(f"Matched {report['case_count']} stdio CLI cases on ARM64, ASan/UBSan and checked output; mutation rejected.")
    print(f"Proved {report['proven_register_identities']} register identities and {report['provenance_checked_stdio_calls']} stdio calls.")


if __name__=="__main__": main()
