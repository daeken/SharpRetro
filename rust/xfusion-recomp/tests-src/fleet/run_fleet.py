#!/usr/bin/env python3
"""Fleet runner: compile each fleet *.c, run through atomics_torture's --fleet
arm (generic: load elf, N threads, _start(shm, tid), read CHECK offsets),
eval CHECK exprs, report. EXPECT: markers gate expected-fail semantics.

CHECK grammar (header comments):
    // CHECK: <offset> <width 8|16|32|64> <python-expr over N, ITERS>
    // PRESET: <offset> <width> <value-expr>       (harness writes before run)
    // EXPECT: FAIL-UNTIL <mnemonic>               (test detects a known gap)
    // EXPECT: MISALIGN-TRAP                       (die-loud contract test)
    // EXPECT: C2-RESIDUAL                         (nonzero = quantified residual, not fail)
    // THREADS: <n>                                (default 8)
Usage: run_fleet.py [file.c ...] (default: all in this dir)
"""
import os, re, subprocess, sys, glob

HERE = os.path.dirname(os.path.abspath(__file__))
XR = os.path.abspath(os.path.join(HERE, "../.."))
RUNNER = os.path.join(XR, "target/release/examples/atomics_torture")
CLANG = ["clang", "-target", "x86_64-linux-gnu", "-fuse-ld=lld",
         "-nostdlib", "-static", "-O1", "-fno-unroll-loops"]

def parse(src):
    text = open(src).read()
    m = re.search(r"#define\s+ITERS\s+(\d+)", text)
    iters = int(m.group(1)) if m else 100000
    checks = [(int(a, 0), int(w), e.strip()) for a, w, e in
              re.findall(r"//\s*CHECK:\s*(\S+)\s+(8|16|32|64)\s+(.+)", text)]
    presets = [(int(a, 0), int(w), e.strip()) for a, w, e in
               re.findall(r"//\s*PRESET:\s*(\S+)\s+(8|16|32|64)\s+(.+)", text)]
    expect = re.findall(r"//\s*EXPECT:\s*(\S+)", text)
    m = re.search(r"//\s*THREADS:\s*(\d+)", text)
    threads = int(m.group(1)) if m else 8
    return iters, checks, presets, expect, threads

def main():
    files = sys.argv[1:] or sorted(glob.glob(os.path.join(HERE, "*.c")))
    results = []
    for src in files:
        name = os.path.basename(src)
        iters, checks, presets, expect, threads = parse(src)
        if not checks and "MISALIGN-TRAP" not in expect:
            results.append((name, "SKIP", "no CHECK lines")); continue
        exe = f"/tmp/fleet_{name[:-2]}"
        r = subprocess.run(CLANG + ["-o", exe, src], capture_output=True, text=True)
        if r.returncode != 0:
            results.append((name, "COMPILE-FAIL", r.stderr.strip()[:200])); continue
        # runner protocol: --fleet <exe> <threads> [--preset off:w:val ...] → dumps
        #   FLEET <offset> <value-hex> per CHECK offset requested via --read off:w
        args = [RUNNER, exe, str(threads), "--fleet"]
        for off, w, expr in presets:
            v = eval(expr, {"N": threads, "ITERS": iters})
            args += ["--preset", f"{off}:{w}:{v & (2**w - 1):#x}"]
        for off, w, _ in checks:
            args += ["--read", f"{off}:{w}"]
        r = subprocess.run(args, capture_output=True, text=True, timeout=600)
        if "MISALIGN-TRAP" in expect:
            ok = ("misaligned" in r.stderr) or ("misaligned" in r.stdout) or r.returncode not in (0,)
            results.append((name, "PASS" if ok else "FAIL",
                            "die-loud contract " + ("held" if ok else "MISSING")))
            continue
        got = {}
        for line in r.stdout.splitlines():
            mm = re.match(r"FLEET (\d+) (0x[0-9a-fA-F]+)", line)
            if mm: got[int(mm.group(1))] = int(mm.group(2), 16)
        fails = []
        for off, w, expr in checks:
            want = eval(expr, {"N": threads, "ITERS": iters}) & (2**w - 1)
            g = got.get(off)
            if g is None: fails.append(f"@{off}: no output")
            elif g != want: fails.append(f"@{off}: got {g:#x} want {want:#x}")
        gap = next((e for e in expect if e == "FAIL-UNTIL"), None)
        if fails and any(e.startswith("FAIL-UNTIL") or e == "FAIL-UNTIL" for e in expect):
            results.append((name, "XFAIL", f"gap-detector firing: {fails[0]}"))
        elif fails and "C2-RESIDUAL" in expect:
            results.append((name, "RESIDUAL", "; ".join(fails[:3])))
        elif fails:
            results.append((name, "FAIL", "; ".join(fails[:3])))
        else:
            note = "XPASS?! gap closed?" if any("FAIL-UNTIL" in e for e in expect) else "ok"
            results.append((name, "PASS", note))
    w = max(len(n) for n, _, _ in results) if results else 10
    hard_fail = False
    for n, st, note in results:
        if st == "FAIL" or st == "COMPILE-FAIL": hard_fail = True
        print(f"{st:>12}  {n:<{w}}  {note}")
    print(f"[fleet] {sum(1 for _,s,_ in results if s=='PASS')} pass, "
          f"{sum(1 for _,s,_ in results if s=='XFAIL')} xfail(gap-detectors), "
          f"{sum(1 for _,s,_ in results if s in ('FAIL','COMPILE-FAIL'))} FAIL "
          f"of {len(results)}")
    sys.exit(1 if hard_fail else 0)

if __name__ == "__main__":
    main()
