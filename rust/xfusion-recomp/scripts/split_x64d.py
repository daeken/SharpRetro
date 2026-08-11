#!/usr/bin/env python3
# Split an X64D/X64E corpus into N shards (each valid, own header).
# Rows are variable-length (12 + slen + 90*8*2 [+ 2*4096 mem windows for
# X64E/v4]) so must walk sequentially.
import sys, struct, os
inp, N, outbase = sys.argv[1], int(sys.argv[2]), sys.argv[3]
with open(inp, "rb") as f:
    magic, n = struct.unpack("<II", f.read(8))
    assert magic in (0x44343658, 0x45343658), f"bad magic {magic:x}"
    mem_extra = 2*4096 if magic == 0x45343658 else 0
    per = (n + N - 1) // N
    print(f"  {inp}: {n} rows → {N} shards × ~{per}")
    outs = []
    for k in range(N):
        w = open(f"{outbase}.{k:02d}", "wb")
        w.write(struct.pack("<II", magic, 0))  # count patched at end
        outs.append([w, 0])
    for i in range(n):
        hdr = f.read(12)
        did, fmask, slen = struct.unpack("<III", hdr)
        body = f.read(slen + 90*8*2 + mem_extra)
        k = i // per
        if k >= N: k = N-1
        outs[k][0].write(hdr); outs[k][0].write(body)
        outs[k][1] += 1
    for k, (w, cnt) in enumerate(outs):
        w.seek(4); w.write(struct.pack("<I", cnt)); w.close()
        sz = os.path.getsize(f"{outbase}.{k:02d}")
        print(f"    shard {k:02d}: {cnt} rows, {sz//1024//1024}MB")
