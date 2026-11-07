#!/usr/bin/env python3
"""Published Windows PE export -> recovered C++ -> native ARM64.

The primary reference executes the original DLL bytes in an isolated x64 Mach-O
process under Rosetta, with explicit Win64 ABI/import bridges and no DllMain/TLS.
The same-tag source is an independent oracle/compression generator, not the PE.
"""
import argparse
import ctypes
import hashlib
import json
from pathlib import Path
import platform
import random
import re
import subprocess
import urllib.request
import zipfile

HERE=Path(__file__).resolve().parent
ROOT=HERE.parents[2]
DLL=ROOT/"Decompiler/bin/Release/net10.0/Decompiler.dll"
CACHE=HERE/"build/download"


def digest(path): return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def tool(*args):
    result=subprocess.run([str(a) for a in args],capture_output=True,text=True,timeout=60)
    if result.returncode: raise RuntimeError(f"Command failed: {args}\n{result.stdout}\n{result.stderr}")
    return result


def fixture(download=False):
    manifest=json.loads((HERE/"upstream.json").read_text())
    for name,expected in manifest["source_sha256"].items():
        if digest(HERE/"upstream"/name)!=expected: raise AssertionError(f"Changed upstream source: {name}")
    archive=CACHE/"lz4_v1_8_3_win64.zip"
    if not archive.exists():
        if not download: raise FileNotFoundError("Run this fixture's roundtrip.py to download the pinned official archive")
        CACHE.mkdir(parents=True,exist_ok=True)
        request=urllib.request.Request(manifest["archive_url"],headers={"User-Agent":"SharpRetro-fixture"})
        with urllib.request.urlopen(request,timeout=60) as remote: data=remote.read(2*1024*1024)
        if hashlib.sha256(data).hexdigest()!=manifest["archive_sha256"]: raise AssertionError("Release archive hash mismatch")
        archive.write_bytes(data)
    if digest(archive)!=manifest["archive_sha256"]: raise AssertionError("Cached release archive hash mismatch")
    with zipfile.ZipFile(archive) as zipped: raw=zipped.read(manifest["dll_member"])
    if hashlib.sha256(raw).hexdigest()!=manifest["dll_sha256"]: raise AssertionError("Published DLL hash mismatch")
    return manifest,raw


def build(directory,download=False):
    if platform.system()!="Darwin" or platform.machine()!="arm64": raise RuntimeError("Requires Apple Silicon macOS and Rosetta")
    manifest,raw=fixture(download)
    directory=Path(directory).resolve();directory.mkdir(parents=True,exist_ok=True)
    pe=directory/"liblz4-published-x64.dll";pe.write_bytes(raw)
    info=json.loads(tool("dotnet",DLL,"inspect",pe).stdout)
    cpp=directory/"recovered.cpp"
    recovered=tool("dotnet",DLL,"decompile-function",pe,"--export","LZ4_decompress_safe","--control-flow","structured","--output",cpp)
    artifacts={"directory":directory,"pe":pe,"image":info,"manifest":manifest,"cpp":cpp,"diagnostics":recovered.stderr}
    reference=directory/"pe-reference-x64"
    tool("clang++","-arch","x86_64","-std=c++20","-O2","-fno-builtin",HERE/"pe_reference.cpp","-o",reference)
    artifacts["reference"]=reference
    for name,flags in (("native",["-O2"]),("sanitized",["-O1","-fsanitize=address,undefined"])):
        exe=directory/f"{name}-arm64"
        tool("clang++","-arch","arm64","-std=c++20",*flags,f'-DRECOVERED_CPP="{cpp}"',HERE/"driver.cpp","-o",exe)
        artifacts[name]=exe
    source_object=directory/"source.o"
    tool("clang","-arch","arm64","-std=c11","-O2","-c",HERE/"upstream/lz4.c","-o",source_object)
    source=directory/"source-arm64"
    tool("clang++","-arch","arm64","-std=c++20","-O2","-DSOURCE_REFERENCE",HERE/"driver.cpp",source_object,"-o",source)
    library=directory/"liblz4-source.dylib"
    tool("clang","-arch","arm64","-std=c11","-O2","-dynamiclib",HERE/"upstream/lz4.c","-o",library)
    artifacts.update(source=source,library=library)
    return artifacts


def cases(library):
    lib=ctypes.CDLL(str(library))
    lib.LZ4_compressBound.argtypes=[ctypes.c_int];lib.LZ4_compressBound.restype=ctypes.c_int
    lib.LZ4_compress_default.argtypes=[ctypes.c_void_p,ctypes.c_void_p,ctypes.c_int,ctypes.c_int]
    lib.LZ4_compress_default.restype=ctypes.c_int
    result=[];seen=set()
    def add(encoded,capacity,expected=None):
        key=(encoded,capacity)
        if key in seen: return
        seen.add(key)
        result.append({"id":f"pe-{len(result):04}","input":encoded,"capacity":capacity,"expected":expected})
    rng=random.Random(0x1835045)
    for size in (0,1,2,3,4,5,7,8,9,12,15,16,17,31,32,33,64,127,255,256,257,511,1024,4096,16384):
        for data in (b"A"*size,(bytes(range(256))*((size+255)//256))[:size],rng.randbytes(size)):
            source=ctypes.create_string_buffer(data);capacity=lib.LZ4_compressBound(size);out=ctypes.create_string_buffer(capacity)
            length=lib.LZ4_compress_default(source,out,size,capacity)
            if length<=0: raise AssertionError("Source compression failed")
            encoded=out.raw[:length]
            add(encoded,size,data);add(encoded,size+16,data)
            if size: add(encoded,size-1)
            for cut in (0,1,2,len(encoded)-1):
                if 0<=cut<len(encoded): add(encoded[:cut],size+16)
    for offset in range(1,9):
        prefix=bytes(range(1,offset+1));match=bytes(prefix[i%offset] for i in range(20));tail=b"END!!"
        encoded=bytes([(offset<<4)|15])+prefix+offset.to_bytes(2,"little")+b"\x01\x50"+tail
        expected=prefix+match+tail
        for extra in (0,7,16,32): add(encoded,len(expected)+extra,expected)
    for raw in (b"",b"\xf0",b"\xf0\xff",b"\x10",b"\x10A",b"\x00\x01\x00",b"\x0f\x00\x00\xff",b"\xff"*16):
        for capacity in (0,1,8,16,32): add(raw,capacity)
    for _ in range(32): add(rng.randbytes(rng.randrange(1,65)),rng.randrange(65))
    return result,lib


def observe(executable,case,pe=None):
    args=[str(executable)]
    if pe is not None: args.extend([str(pe),"decompress"])
    args.extend([case["input"].hex(),str(case["capacity"])])
    p=subprocess.run(args,capture_output=True,text=True,timeout=15)
    if p.returncode!=0: raise AssertionError(f"Runner failure on {case['id']}: exit{p.returncode}: {p.stderr[:1000]}")
    value=json.loads(p.stdout)
    if not value["canaries_ok"]: raise AssertionError(f"Canary failure on {case['id']}")
    if pe is not None and (not value["image_unchanged"] or not value["initial_preserved_gprs_zero"]):
        raise AssertionError("Reference image mutated or entry-state contract absent")
    return value["return"],value["output"]


def mutate_decoder_shifts(body):
    # Both spellings represent the same negative control after typed rendering.
    # Skip comments/literals; a template closer has no following ULL count.
    pattern=r'''("(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|//[^\n]*|/\*[\s\S]*?\*/)|\bshift_right(?=\s*\()|(>>)(?=\s*(?:0x[0-9a-fA-F]+|[0-9]+)ULL\b)'''
    return re.sub(pattern,lambda m:m.group(0) if m.group(1) else '<<' if m.group(2) else 'shift_left',body)


def verify(artifacts):
    corpus,lib=cases(artifacts["library"]);rows=[]
    version=json.loads(tool(artifacts["reference"],artifacts["pe"],"version").stdout)
    if version["return"]!=10803 or not version["image_unchanged"]: raise AssertionError("Wrong published DLL version")
    for size in (-2147483648,-1,0,1,254,255,256,4096,0x7e000000,0x7e000001,2147483647):
        bound=json.loads(tool(artifacts["reference"],artifacts["pe"],"bound",size).stdout)
        if bound["return"]!=lib.LZ4_compressBound(size) or not bound["image_unchanged"]: raise AssertionError("Published/source bound mismatch")
    for case in corpus:
        reference=observe(artifacts["reference"],case,artifacts["pe"])
        repeat=observe(artifacts["reference"],case,artifacts["pe"])
        native=observe(artifacts["native"],case);sanitized=observe(artifacts["sanitized"],case)
        if not reference==repeat==native==sanitized:
            failure={"id":case["id"],"input_hex":case["input"].hex(),"capacity":case["capacity"],
                     "reference":reference,"repeat":repeat,"native":native,"sanitized":sanitized}
            (artifacts["directory"]/"failure.json").write_text(json.dumps(failure,indent=2))
            raise AssertionError(f"PE/recovered mismatch on {case['id']}; see failure.json")
        source=observe(artifacts["source"],case)
        # Same-tag source confirms API success/failure and returned output; its
        # compiler's extra wild-copy bytes/partial error writes are not the PE.
        if (source[0]<0)!=(reference[0]<0): raise AssertionError(f"Source status mismatch: {case['id']}")
        if reference[0]>=0:
            if source[0]!=reference[0] or source[1][:reference[0]*2]!=reference[1][:reference[0]*2]:
                raise AssertionError(f"Source decoded bytes mismatch: {case['id']}")
        if case["expected"] is not None and (reference[0]!=len(case["expected"]) or reference[1][:reference[0]*2]!=case["expected"].hex()):
            raise AssertionError(f"Independent bytes mismatch: {case['id']}")
        rows.append({"id":case["id"],"input_sha256":hashlib.sha256(case["input"]).hexdigest(),"input_size":len(case["input"]),
                     "capacity":case["capacity"],"return":reference[0],"output_sha256":hashlib.sha256(bytes.fromhex(reference[1])).hexdigest(),"matches":True})
    prefix,body=artifacts["cpp"].read_text().split("// Recovered function bodies",1)
    changed=mutate_decoder_shifts(body)
    if changed==body: raise AssertionError("No decoder shift for mutation")
    mutant_cpp=artifacts["directory"]/"mutant.cpp";mutant_cpp.write_text(prefix+"// Recovered function bodies"+changed)
    mutant=artifacts["directory"]/"mutant-arm64"
    tool("clang++","-arch","arm64","-std=c++20","-O2",f'-DRECOVERED_CPP="{mutant_cpp}"',HERE/"driver.cpp","-o",mutant)
    control={"id":"mutation","input":b"\x50hello","capacity":5}
    if observe(mutant,control)==observe(artifacts["reference"],control,artifacts["pe"]): raise AssertionError("Decoder mutation escaped comparisons")
    return {"scope":"Selected prebuilt PE export closure; no Windows process/DllMain/TLS/unwind emulation",
            "reference":"Original published x64 DLL machine code via Win64 ABI/import gate under Rosetta",
            "candidate":"Recovered C++ compiled separately for ARM64, with ASan/UBSan repeat",
            "entry_state":"Four Win64 arguments; explicit nonargument preserved GPR inputs zero in both runners",
            "upstream":artifacts["manifest"],"input_sha256":digest(artifacts["pe"]),"generated_cpp_sha256":digest(artifacts["cpp"]),
            "host":{"system":platform.platform(),"clang":tool("clang++","--version").stdout.splitlines()[0]},
            "export":"LZ4_decompress_safe","export_va":artifacts["image"]["Exports"]["LZ4_decompress_safe"]["Address"],
            "case_count":len(rows),"all_match":True,"mutation_rejected":True,"cases":rows,"diagnostics":artifacts["diagnostics"]}


def main():
    parser=argparse.ArgumentParser();parser.add_argument("--build-dir",type=Path,default=HERE/"build")
    args=parser.parse_args();artifacts=build(args.build_dir,download=True);report=verify(artifacts)
    (artifacts["directory"]/"report.json").write_text(json.dumps(report,indent=2)+"\n")
    print(f"Matched {report['case_count']} published Windows PE decoder cases on ARM64 and ASan/UBSan; source oracle agrees; mutation rejected.")


if __name__=="__main__": main()
