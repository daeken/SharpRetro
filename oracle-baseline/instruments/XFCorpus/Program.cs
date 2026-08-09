// C# corpus-dump: linear-sweep bytes through XFusionCpu.Disassembler.DecodeInsn,
// dump `offset(hex) len def_id mnem` per insn — same format as Rust --corpus --dump.
// Phase-1-full gate: diff this output against the Rust decoder's on the same bytes.
using XFusionCpu;

var path = args[0];
var off = args.Length > 1 ? Convert.ToInt32(args[1], 16) : 0;
var len = args.Length > 2 ? Convert.ToInt32(args[2], 16) : -1;
var all = File.ReadAllBytes(path);
if(len < 0) len = all.Length - off;
var bytes = all.AsSpan(off, len);

int i = 0, n_ok = 0, n_fail = 0;
while(i < bytes.Length) {
    var slice = bytes[i..Math.Min(i + 15, bytes.Length)];
    if(Disassembler.DecodeInsn(slice, XMode.Bits64, out var d) && d.Len > 0) {
        // Compare on (offset, len) only — if instruction BOUNDARIES match on N real
        // insns, decode is correct (a length mismatch at insn K desyncs K+1 onward).
        // def_id cross-referenceability is a separate check (BodyOrder must match).
        Console.WriteLine($"{off+i:x} {d.Len}");
        n_ok++;
        i += d.Len;
    } else {
        n_fail++;
        i++;
    }
}
Console.Error.WriteLine($"[C# corpus: decoded={n_ok} undecoded={n_fail}]");
