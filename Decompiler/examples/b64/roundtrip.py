#!/usr/bin/env python3
"""Pinned, unchanged upstream b64: x64 executable -> C++ -> standalone ARM64."""
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

ROOT=Path(__file__).resolve().parents[3]
HERE=Path(__file__).resolve().parent
DLL=ROOT/"Decompiler/bin/Release/net10.0/Decompiler.dll"
X64_FLAGS=["-arch","x86_64","-std=c11","-O2","-fno-builtin","-fno-vectorize",
           "-fno-slp-vectorize","-fno-unroll-loops","-fno-stack-protector",
           "-fomit-frame-pointer","-mno-sse","-mno-sse2","-Wl,-no_fixup_chains",
           "-Wno-builtin-macro-redefined",'-D__DATE__="Jan 01 2025"','-D__TIME__="00:00:00"']


def tool(*args,diagnostics=None):
    p=subprocess.run([str(a) for a in args],capture_output=True,text=True,timeout=60)
    if p.returncode: raise RuntimeError(f"Command failed: {args}\n{p.stdout}\n{p.stderr}")
    if diagnostics is not None: diagnostics.append(p.stderr)
    return p.stdout


def digest(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def provenance():
    manifest=json.loads((HERE/"upstream.json").read_text())
    hashes={}
    for name,expected in manifest["files"].items():
        data=(HERE/"upstream"/name).read_bytes()
        actual=hashlib.sha1(b"blob "+str(len(data)).encode()+b"\0"+data).hexdigest()
        if actual!=expected: raise AssertionError(f"Upstream file changed: {name}")
        hashes[name]=hashlib.sha256(data).hexdigest()
    return {**manifest,"sha256":hashes}


def build(directory):
    if platform.system()!="Darwin" or platform.machine()!="arm64":
        raise RuntimeError("Requires Apple Silicon macOS and Rosetta")
    source=provenance()
    directory=Path(directory).resolve(); directory.mkdir(parents=True,exist_ok=True)
    original=directory/"b64-x86_64"
    tool("clang",*X64_FLAGS,HERE/"upstream/src/main.c",HERE/"upstream/src/base64.c","-o",original)
    image=json.loads(tool("dotnet",DLL,"inspect",original))
    artifacts={"directory":directory,"original":original,"image":image,"upstream":source,"diagnostics":[]}
    for name,memory in (("recovered","native"),("checked","checked")):
        cpp=directory/f"{name}.cpp"; exe=directory/f"{name}-arm64"
        tool("dotnet",DLL,"decompile-app",original,"--memory",memory,"--control-flow","structured","--output",cpp,diagnostics=artifacts["diagnostics"])
        tool("clang++","-arch","arm64","-std=c++20","-O2",cpp,"-o",exe)
        artifacts[name]=exe; artifacts[name+"_cpp"]=cpp
    sanitized=directory/"recovered-arm64-sanitized"
    tool("clang++","-arch","arm64","-std=c++20","-O1","-fsanitize=address,undefined",artifacts["recovered_cpp"],"-o",sanitized)
    artifacts["sanitized"]=sanitized
    return artifacts


def cases():
    result=[]
    def add(args=(),data=b"",files=None,expected=None,close_stdin=False,expected_files=None):
        result.append({"id":f"b64-{len(result):03}","argv":list(args),"stdin":data,
                       "files":files or {},"expected_stdout":expected,"close_stdin":close_stdin,"expected_files":expected_files or {}})
    for option in ("-h","--help","-l","--list","-v","--version","--unknown","-o"):
        add([option])
    add(["missing.bin"])
    add(["-o","missing-dir/out.bin"])
    add(["-o","out.bin","missing.bin"])
    add(["-o","directory"],files={"directory/nested":b""})
    add(close_stdin=True)
    def wrapped(data):
        return b"".join(data[i:i+80]+(b"\n" if len(data[i:i+80])==80 else b"") for i in range(0,len(data),80))
    rng=random.Random(0xB64C0DE)
    samples=[b"",b"f",b"fo",b"foo",b"hello",b"\0\xff\x80\n\r",bytes(range(256)),"猫 🦊".encode()]
    samples += [rng.randbytes(n) for n in (58,59,60,61,62,119,120,121,255,256,257,1024,4096,65536)]
    for data in samples:
        encoded=base64.b64encode(data); url=base64.urlsafe_b64encode(data)
        add(data=data,expected=encoded)
        add(["-u"],data,expected=url)
        add(["--wrap"],data,expected=wrapped(encoded))
        add(["-d"],encoded,expected=data)
        add(["--decode","--url-safe"],url,expected=data)
        add(["-d"],b" \t\n"+b"\r\n".join(encoded[i:i+4] for i in range(0,len(encoded),4)),expected=data)
        add(["input.bin"],files={"input.bin":data},expected=encoded)
        add(["-o","out.b64","input.bin"],files={"input.bin":data},expected=b"",expected_files={"out.b64":encoded})
        add(["-d","-o","out.bin","input.b64"],files={"input.b64":encoded},expected=b"",expected_files={"out.bin":data})
    for malformed in (b"!",b"a",b"abc",b"====",b"=AAA",b"A=AA",b"AA=A",b"AA!A",b"AA\xffA",b"Zg==!",b"Zm9vZ"):
        add(["-d"],malformed)
        add(["-d","bad.b64"],files={"bad.b64":malformed})
        add(["-d","--ignore-garbage"],malformed)
    for _ in range(24):
        one=rng.randbytes(rng.randrange(257)); two=rng.randbytes(rng.randrange(257))
        add(["one.bin","two.bin"],files={"one.bin":one,"two.bin":two},expected=base64.b64encode(one)+base64.b64encode(two))
        add(["-d","one.b64","two.b64"],files={"one.b64":base64.b64encode(one),"two.b64":base64.b64encode(two)},expected=one+two)
    return result


def observe(executable,case,directory):
    directory.mkdir(parents=True)
    for name,data in case["files"].items():
        target=directory/name; target.parent.mkdir(parents=True,exist_ok=True); target.write_bytes(data)
    kwargs={"capture_output":True,"timeout":30,"cwd":directory,"env":{**os.environ,"LC_ALL":"C"}}
    if case.get("close_stdin"): kwargs.update(stdin=subprocess.DEVNULL,preexec_fn=lambda:os.close(0))
    else: kwargs["input"]=case["stdin"]
    proc=subprocess.run([str(executable),*case["argv"]],**kwargs)
    files={str(p.relative_to(directory)):p.read_bytes() for p in sorted(directory.rglob("*")) if p.is_file()}
    return proc.returncode,proc.stdout,proc.stderr,files


def encoded(observation):
    return {"exit":observation[0],"stdout":base64.b64encode(observation[1]).decode(),
            "stderr":base64.b64encode(observation[2]).decode(),
            "files":{k:base64.b64encode(v).decode() for k,v in observation[3].items()}}


def mutate_codec_shifts(body):
    # Preserve the original left-to-right shift negative control after native
    # expression rendering; comments, literals and template closers stay intact.
    pattern=r'''("(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|//[^\n]*|/\*[\s\S]*?\*/)|\bshift_left(?=\s*\()|(<<)(?=\s*(?:0x[0-9a-fA-F]+|[0-9]+)ULL\b)'''
    return re.sub(pattern,lambda m:m.group(0) if m.group(1) else '>>' if m.group(2) else 'shift_right',body)


def verify(artifacts):
    directory=artifacts["directory"]; runs=directory/"runs"
    if runs.exists(): shutil.rmtree(runs)
    # The ARM64 process runs with only its generated executable and case inputs.
    isolated=directory/"isolated"; isolated.mkdir(exist_ok=True)
    standalone=isolated/"b64"; shutil.copy2(artifacts["recovered"],standalone)
    corpus=cases(); rows=[]
    for case in corpus:
        root=runs/case["id"]
        reference=observe(artifacts["original"],case,root/"original")
        actual=observe(standalone,case,root/"native")
        sanitized=observe(artifacts["sanitized"],case,root/"sanitized")
        checked=observe(artifacts["checked"],case,root/"checked")
        if not reference==actual==sanitized==checked:
            (directory/"failure.json").write_text(json.dumps({"case":case["id"],"argv":case["argv"],
                "original":encoded(reference),"native":encoded(actual),"sanitized":encoded(sanitized),"checked":encoded(checked)},indent=2))
            raise AssertionError(f"Mismatch in {case['id']}: {case['argv']}; see failure.json")
        if case["expected_stdout"] is not None and reference[1]!=case["expected_stdout"]:
            raise AssertionError(f"Independent Base64 oracle failed for {case['id']}")
        for name,expected in case["expected_files"].items():
            if reference[3].get(name)!=expected: raise AssertionError(f"Independent file oracle failed for {case['id']}: {name}")
        rows.append({"id":case["id"],"argv":case["argv"],"exit":reference[0],"matches":True,
                     "stdout_sha256":hashlib.sha256(reference[1]).hexdigest(),"stderr_sha256":hashlib.sha256(reference[2]).hexdigest(),
                     "files_sha256":{k:hashlib.sha256(v).hexdigest() for k,v in reference[3].items()}})
    source=artifacts["recovered_cpp"].read_text()
    prefix,body=source.split("// Recovered function bodies",1)
    changed=prefix+"// Recovered function bodies"+mutate_codec_shifts(body)
    if changed==source: raise AssertionError("No codec shift available for mutation")
    mutant_cpp=directory/"mutant.cpp"; mutant_cpp.write_text(changed)
    mutant=directory/"mutant-arm64"
    tool("clang++","-arch","arm64","-std=c++20","-O2",mutant_cpp,"-o",mutant)
    control={"argv":[],"stdin":bytes(range(256)),"files":{}}
    rejected=observe(mutant,control,runs/"mutation")!=observe(artifacts["original"],control,runs/"mutation-reference")
    if not rejected: raise AssertionError("Changed codec shift survived the comparison")
    regions=re.findall(r"// Control flow: structured; (\d+) loops, (\d+) conditionals, (\d+) duplicated blocks",source)
    report={"upstream":artifacts["upstream"],"input_sha256":digest(artifacts["original"]),"x64_flags":X64_FLAGS,
            "architectures":{k:tool("lipo","-archs",artifacts[k]).strip() for k in ("original","recovered","sanitized","checked")},
            "generated_cpp_sha256":digest(artifacts["recovered_cpp"]),"source_bytes":len(source.encode()),"source_lines":len(source.splitlines()),
            "imports":artifacts["image"]["Imports"],"bindings":artifacts["image"]["Bindings"],"rebases":artifacts["image"]["Rebases"],
            "recovery_diagnostics":artifacts["diagnostics"],
            "instructions":int(re.search(r"Emitted (\d+) reachable instructions",artifacts["diagnostics"][0])[1]),
            "basic_blocks":int(re.search(r"(\d+) basic blocks",artifacts["diagnostics"][0])[1]),
            "functions":len(regions),"loops":sum(int(r[0]) for r in regions),"conditional_regions":sum(int(r[1]) for r in regions),
            "native_context":("NativeCallContext" in source or "CallFrame" in source),"gotos":len(re.findall(r"\bgoto\s+",source)),
            "case_count":len(rows),"all_match":True,"mutation_rejected":rejected,"cases":rows,
            "scope":"Bounded stdout/stderr/exit/filesystem comparisons; unchanged upstream bugs are retained, not universal equivalence"}
    (directory/"report.json").write_text(json.dumps(report,indent=2)+"\n")
    return report


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--build-dir",type=Path,default=HERE/"build")
    args=parser.parse_args(); report=verify(build(args.build_dir))
    print(f"Matched {report['case_count']} upstream b64 cases (stdout/stderr/exit/files) on native ARM64, ASan/UBSan and checked output; codec mutation rejected.")
    print(f"Generated C++ and report: {args.build_dir.resolve()}")


if __name__=="__main__": main()
