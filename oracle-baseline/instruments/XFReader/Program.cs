// XFReader -- read an X64D sweep corpus and drive XFusionCpu's C# lift+lower+exec
// (X86Machine) over each row, comparing against the corpus's post-state.
//
// WHY THIS EXISTS -- the link that nothing verified:
//   link-1  .isa formula -> interp.rs      graded by SILICON (the sweep corpus).
//   link-2  interp.rs    -> IlLower.cs     graded by NOTHING. Hand-transcribed.
// A C# evaluator written alone would be CO-BLIND for link-1 (both arms derive from the
// one .isa, so a wrong formula agrees with itself). The independence here comes from the
// ROWS: each carries a post-state that real silicon produced, so a transcription error
// in IlLower shows as a diff against a number no C# code computed.
//
// WHY THE def_id IS IGNORED. A row's stored def_id indexes the def table AS IT WAS WHEN
// THE CORPUS WAS GENERATED. The corpus is frozen (2026-08-11); today's table has defs
// inserted at several points, so a def_id lookup mislabels ~74% of rows and every
// mislabel is a PLAUSIBLE mnemonic -- no error state to notice. The insn bytes are in
// the stub at SLOT_OFF, so we read those and let the decoder name it. See
// rust/xfusion-recomp/SWEEP-VECTOR-COVERAGE.md for how that was measured.
//
// ROW FORMAT (sweep.rs:641-651), all little-endian:
//   u32 def_id | u32 flags_mask | u32 stub_len | u8[stub_len] stub
//   u64[90] pre  | u64[90] post          (X64E adds [pre_mem:64][post_mem:64])
// Header: 4-byte magic "X64D"/"X64E" then u32 count.
//
// FLAGS_MASK IS LOAD-BEARING and is why a naive compare fails: only the bits the def
// DECLARES it writes are comparable. SDM-undefined flags differ legitimately between
// any two correct implementations. Bit 31 marks a FAULT row -- skipped here (a fault
// is silicon's signal delivery, which X86Machine does not model).
//
// USAGE (worked, with real output -- a usage line names the SHAPE of a subject, a worked
// invocation names a REAL one):
//   dotnet run --project oracle-baseline/instruments/XFReader -- \
//       /tmp/sweep_p2_GOLDEN.x64d 64 200
//     -> [XFReader] rows=200 ok=196 skip=4 diff=0
//   A nonzero `diff` prints the first 20 with the mnemonic, the differing field, and
//   both values. `skip` counts rows this reader cannot grade and SAYS WHY per class --
//   never silently, because a high skip rate with diff=0 reads exactly like success.

using XFusionCpu;

if(args.Length < 1) {
	Console.Error.WriteLine("usage: XFReader <corpus.x64d> [bits:64|32] [max_rows]");
	return 2;
}
var path = args[0];
var bits = args.Length > 1 ? int.Parse(args[1]) : 64;
var maxRows = args.Length > 2 ? int.Parse(args[2]) : int.MaxValue;
// A DIFF LISTING IS NOT A DIFF CENSUS. This capped prints at 20 and I read the 20 as the
// population -- twice on an 800-row diff, concluding "all PSRLDQ" from a truncated listing
// whose cap I wrote myself. Then I "fixed" two width helpers against that reading and the
// count did not move, because I was measuring the cap. XF_DIFFCAP raises it so a
// distribution comes OUT of the output instead of being inferred from its head.
var mode = bits == 32 ? XMode.Bits32 : XMode.Bits64;

// STATE LAYOUT -- must match rust/xfusion-recomp/src/state.rs. Read from there, not
// composed: OFF_GPR=0(16) OFF_EFLAGS=16 OFF_RIP=17 OFF_SEG=18(6) OFF_XMM=24(64) = 90.
const int STATE_WORDS = 90, OFF_GPR = 0, OFF_EFLAGS = 16, OFF_RIP = 17, OFF_SEG = 18, OFF_XMM = 24;

using var fs = File.OpenRead(path);
using var br = new BinaryReader(fs);
var magic = new string(br.ReadChars(4));
if(magic != "X64D" && magic != "X64E") {
	Console.Error.WriteLine($"  x bad magic '{magic}' -- expected X64D or X64E. NOT a corpus.");
	return 2;
}
var hasMem = magic == "X64E";
var count = br.ReadUInt32();
Console.Error.WriteLine($"[XFReader] {path}: magic={magic} count={count} mode={mode}");

int nOk = 0, nDiff = 0, nPrinted = 0;
var DiffCap = int.TryParse(Environment.GetEnvironmentVariable("XF_DIFFCAP"), out var _dc) ? _dc : 20;
var SkipNames = Environment.GetEnvironmentVariable("XF_SKIPNAMES") == "1";
var skByDef = new Dictionary<int,int>();
void NoteSkip(int id) { if(SkipNames) skByDef[id] = skByDef.GetValueOrDefault(id) + 1; }
// Skip reasons kept SEPARATE. A single skip count hides which population went ungraded.
int skDecode = 0, skFault = 0, skLift = 0, skStep = 0, skNoSlot = 0;

for(uint r = 0; r < count && r < maxRows; r++) {
	var defId = br.ReadUInt32();
	var fmask = br.ReadUInt32();
	var stubLen = br.ReadUInt32();
	var stub = br.ReadBytes((int) stubLen);
	var pre = new ulong[STATE_WORDS];
	for(var i = 0; i < STATE_WORDS; i++) pre[i] = br.ReadUInt64();
	var post = new ulong[STATE_WORDS];
	for(var i = 0; i < STATE_WORDS; i++) post[i] = br.ReadUInt64();
	if(hasMem) { br.ReadBytes(64); br.ReadBytes(64); }

	if((fmask & 0x8000_0000u) != 0) { skFault++; continue; }

	// SLOT_OFF from stub_len, per sweep.rs:95 (64-bit v1) / :421 (XMM v2) / 32-bit.
	// Derived from the artifact's own dispatch, not from a remembered constant.
	int slot = stubLen switch { 85 => 29, 191 => 82, 213 => 93, 479 => 226, _ => -1 };
	if(slot < 0 || slot + 15 > stub.Length) { skNoSlot++; continue; }

	if(!Disassembler.DecodeInsn(stub.AsSpan(slot, 15), mode, out var d) || d.Len == 0) { skDecode++; continue; }

	var m = new X86Machine { Mode = mode, Mem = new byte[0x20000] };
	for(var i = 0; i < 16; i++) m.Gpr[i] = pre[OFF_GPR + i];
	m.Flags = pre[OFF_EFLAGS];
	for(var i = 0; i < 6; i++) m.SegBase[i] = pre[OFF_SEG + i];
	// Xmm is ulong[32] here where a row carries u128 per register: LO WORD ONLY.
	// That is the CARRIER LIMIT, and it is why lanes 2-3 are not graded -- see
	// rust/xfusion-recomp/DIFFERENTIAL-SCOPING.md step 2 (path corrected: this cite
	// said "DIFFERENTIAL-SCOPING.md" with no directory, and the file lives two trees
	// over -- a named artifact whose reader has to hunt for it).
	//
	// AND THE SCOPE IS NOW SIZED, which it wasn't when I wrote this line. Walking p2's
	// bytes and asking per row whether any xmm HI word moves pre-vs-post:
	//   rows 4,088,162 | LO changed 1,296,848 (graded) | HI changed 921,360 = 22.5% (not)
	// So this is not a corner. Nearly a quarter of the corpus mutates a lane that the
	// loop below LOADS (pre[OFF_XMM + i*2] reads only even offsets) and the compare at
	// :124 never reads. The hi words are already on disk, so widening the carrier grades
	// them with no fresh sweep. Counted as a scope, not silently dropped.
	// SEED THE FULL 128, both words. This read pre[OFF_XMM + i*2] alone -- the LO word --
	// so every xmm register entered the machine with a ZERO high half, and the corpus's own
	// hi words sat on disk unread. The rows were still GRADED, which is what made it
	// invisible: a lo-only compare against a lo-only seed agrees.
	//
	// PSRLDQ is what surfaced it. A byte-shift-right pulls the top byte DOWN from the hi
	// half, so with hi=0 the answer is want>>8 -- and I read that as a lowering bug and
	// "fixed" two width helpers against it before reading the seed.
	for(var i = 0; i < 32; i++)
		m.Xmm[i] = ((UInt128) pre[OFF_XMM + i*2 + 1] << 64) | pre[OFF_XMM + i*2];
	// Execute the stub's insn in place: copy it to a known IP.
	var ip = 0x1000UL;
	stub.AsSpan(slot, Math.Min(15, stub.Length - slot)).CopyTo(m.Mem.AsSpan((int) ip));
	m.Ip = ip;

	// XFR_DUMP=<row> prints the LOWERED IL for one row. A diff tells you a value is
	// wrong; the dump tells you whether the statement that computes it EXISTS. Those are
	// different questions and the second one is where a transcription error lives.
	if(Environment.GetEnvironmentVariable("XFR_DUMP") == r.ToString()) {
		var blk = X86Lifter.Lift(in d, ip, mode);
		Console.Error.WriteLine($"  [dump row={r}] {Disassembler.DefNames[d.DefId]} -> {blk?.Body.Count.ToString() ?? "LIFT NULL"} stmts");
		if(blk != null) foreach(var s in blk.Body) Console.Error.WriteLine($"      {s}");
	}

	bool stepped;
	try { stepped = m.Step(); }
	// NAME THE DEF, don't just count it. A skip TOTAL says how much wasn't graded and
	// cannot say WHAT -- so "3 unlowered" and "99,198 unlowered" read the same way, and the
	// residual after a widening is exactly the population you want named. XF_SKIPNAMES=1
	// prints the per-def tally at the end.
	catch(NotSupportedException) { skLift++; NoteSkip(d.DefId); continue; }   // unlowered: LOUD by design
	catch(NotImplementedException) { skLift++; NoteSkip(d.DefId); continue; }
	if(!stepped && !m.Halted) { skStep++; continue; }

	// COMPARE. GPRs and the DECLARED flag bits. Xmm lo-word only (carrier limit above).
	var bad = new List<string>();
	for(var i = 0; i < 16; i++)
		if(m.Gpr[i] != post[OFF_GPR + i]) bad.Add($"gpr{i} got={m.Gpr[i]:x} want={post[OFF_GPR + i]:x}");
	var declared = fmask & 0x7FFF_FFFFu;
	if(((m.Flags ^ post[OFF_EFLAGS]) & declared) != 0)
		bad.Add($"eflags&{declared:x} got={m.Flags & declared:x} want={post[OFF_EFLAGS] & declared:x}");
	// COMPARE THE FULL 128. The hi word was on disk the whole time and never read: 22.5%
	// of p2's 4,088,162 rows mutate an xmm hi word (measured at c243eaf), so a lo-only
	// compare graded 3.17M rows and silently exempted the high half of every one.
	for(var i = 0; i < 32; i++) {
		var wantX = ((UInt128) post[OFF_XMM + i*2 + 1] << 64) | post[OFF_XMM + i*2];
		if(m.Xmm[i] != wantX) bad.Add($"xmm{i} got={m.Xmm[i]:x} want={wantX:x}");
	}

	if(bad.Count == 0) { nOk++; continue; }
	nDiff++;
	if(nPrinted++ < DiffCap)
		// TWO DIFFERENT def-ids on this line, and the distinction is load-bearing:
		//   defId    (:78)  the STORED one, read from the corpus row. Joined against a
		//                   LIVE table it mislabels ~74% of rows in a frozen corpus, and
		//                   every mislabel is a plausible mnemonic -- hence the hedge.
		//   d.DefId  (:95)  DECODED FRESH from this row's own stub bytes. Sound by
		//                   construction, which is why the mnemonic reads off THIS one.
		// I mis-read my own line once (thought the mnemonic rode the stale id) -- the
		// two variables differ by one character of case, so the check is to trace each
		// to its assignment rather than to read the expression.
		Console.Error.WriteLine($"  DIFF row={r} def_id={defId}(stale-index, informational) mnem={Disassembler.DefNames[d.DefId]} len={d.Len}: {string.Join(" | ", bad)}");
}

var skTotal = skDecode + skFault + skLift + skStep + skNoSlot;
Console.Error.WriteLine($"[XFReader] rows_read={Math.Min(count, maxRows)} ok={nOk} diff={nDiff} skip={skTotal}");
Console.Error.WriteLine($"[XFReader]   skip breakdown: decode={skDecode} fault={skFault} unlowered={skLift} step={skStep} no-slot={skNoSlot}");
if(SkipNames && skByDef.Count > 0)
	foreach(var kv in skByDef.OrderByDescending(kv => kv.Value))
		Console.Error.WriteLine($"[XFReader]     unlowered {kv.Value,8}  def_id={kv.Key} {(kv.Key < Disassembler.DefNames.Length ? Disassembler.DefNames[kv.Key] : "?")}");
if(nOk == 0) {
	// A zero-graded run with diff=0 reads exactly like success. It is not.
	Console.Error.WriteLine("  x NOTHING GRADED (ok=0) -- read this as BLIND, not clean.");
	return 2;
}
return nDiff == 0 ? 0 : 1;
