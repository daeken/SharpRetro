using MaxwellGenerator;

namespace Pagentry.Lifter;

// Maxwell SM5x → Pagentry IL lifter (tier-3 M1, day-28).
//
// v0 = hardcoded per-mnemonic lift (switch on MaxwellDef.Name → IL).
// Bypasses the .isa eval-expr → PTree → InferType chain so I can get
// shader-02 → readable IL → SPIR-V end-to-end FAST. The .isa eval-exprs
// remain the SPEC; once Builtins.cs + MaxwellDef:Def exist, this becomes
// the cross-check (per spec §7 hawser oracle: hardcoded vs .isa-driven
// must produce identical IL).
//
// Predication: every insn is gated on (Pred ⊕ PredInv). PT (pred==7,
// inv==0) = unconditional → emit body directly. Otherwise wrap in IlIf.
// RZ (gpr==255) reads as f32 0.0; writes are discarded.
//
// One Maxwell instruction → one IlBlock. The whole shader = a sequence
// of blocks (linear for v0; control flow via Bra/Sync/Ssy = M1.5).

public class MaxwellLift {
    readonly List<MaxwellDef> defs;

    public MaxwellLift(List<MaxwellDef> defs) {
        // Most-fixed-bits-first so overlapping prefixes resolve to the
        // most specific (same as the verify loop's FirstOrDefault).
        this.defs = defs.OrderByDescending(d =>
            System.Numerics.BitOperations.PopCount(d.Mask)).ToList();
    }

    public MaxwellDef Decode(ulong w) =>
        defs.FirstOrDefault(d => (w & d.Mask) == d.Match);

    static readonly IlType F32 = new IlType.F(32);
    static readonly IlType U32 = IlType.U32;

    // Read GPR n (compile-time index). RZ → const 0.
    static Il Gpr(ulong n) => n == 255
        ? new IlConst(F32, 0)
        : new IlReadReg(F32, RegKind.Gpr, (int) n);
    static Il GprU(ulong n) => n == 255
        ? new IlConst(U32, 0)
        : new IlReadReg(U32, RegKind.Gpr, (int) n);
    // Write GPR n. RZ → discard (but RHS effects already evaluated).
    static Il SetGpr(ulong n, Il v) => n == 255
        ? new IlBlock(new Il[0])
        : new IlWriteReg(RegKind.Gpr, (int) n, v);
    // ── H-family fp16 helpers. v0: unpack u32→vec2(f32) via
    //    GLSL.std.450 UnpackHalf2x16, do the op as f32, repack via
    //    PackHalf2x16. Avoids Float16 capability. ──
    // HalfSwizzle: 0=F16(unpack→[lo,hi]) 1=F32(treat as f32, repl)
    //              2=H0H0(repl lo) 3=H1H1(repl hi).
    static (Il, Il) HUnpack(Il src, ulong sw, ulong neg, ulong abs) {
        Il lo, hi;
        if(sw == 1) {
            // Source is a single f32, replicated to both lanes.
            lo = hi = src;
        } else {
            // src is u32-as-f32 (typeless GPR holding 2× packed f16).
            // UnpackHalf2x16 takes uint → vec2. Bitcast first.
            var up = new IlIntrin(new IlType.Vec(64), "UnpackHalf2x16",
                new IlCast(IlType.U32, CastKind.Bitcast, src));
            // ‡ The IlIntrin returns vec2; we need component access.
            // Use IlVecElem (already plumbed for IlSample). But
            // IlVecElem assumes a vec4 result — SpirvEmit needs the
            // vec2 case. v0: emit the unpack ONCE via IlLet, then
            // 2× IlVecElem.
            lo = new IlVecElem(F32, up, C(0ul));
            hi = new IlVecElem(F32, up, C(1ul));
            // ⚠️ This emits the IlIntrin TWICE (once per IlVecElem) =
            // 2× UnpackHalf2x16 calls. Wasteful but correct. v0.
            // Proper = caller IlLet-binds; doing that below.
            if(sw == 2) hi = lo;       // H0H0
            else if(sw == 3) lo = hi;  // H1H1
        }
        if(abs == 1) { lo = new IlUn(F32, UnOp.Abs, lo);
                       hi = new IlUn(F32, UnOp.Abs, hi); }
        if(neg == 1) { lo = new IlUn(F32, UnOp.Neg, lo);
                       hi = new IlUn(F32, UnOp.Neg, hi); }
        return (lo, hi);
    }
    static Il HPack((Il, Il) r, ulong ofmt, Il rdOld) {
        // OFmt: 0=F16(repack) 1=F32(store .lo as f32) 2=MrgH0
        //       (rd.lo16=res.lo, rd.hi16=keep) 3=MrgH1.
        var (lo, hi) = r;
        return ofmt switch {
            0 => new IlCast(F32, CastKind.Bitcast,
                    new IlIntrin(IlType.U32, "PackHalf2x16", lo, hi)),
            1 => lo,   // ‡ F32 → store .lo only (verify: or .hi? or
                       // both into Rd,Rd+1? — v0 = .lo per the
                       // common HSET2.bf pattern)
            // Mrg: pack(res), keep half from rdOld. Bitwise merge
            // on the packed u32: MrgH0 = (pack&0xFFFF)|(old&0xFFFF0000).
            2 => new IlCast(F32, CastKind.Bitcast,
                    new IlBin(IlType.U32, BinOp.Or,
                        new IlBin(IlType.U32, BinOp.And,
                            new IlIntrin(IlType.U32, "PackHalf2x16", lo, hi),
                            C(0xFFFFul)),
                        new IlBin(IlType.U32, BinOp.And,
                            new IlCast(IlType.U32, CastKind.Bitcast, rdOld),
                            C(0xFFFF0000ul)))),
            3 => new IlCast(F32, CastKind.Bitcast,
                    new IlBin(IlType.U32, BinOp.Or,
                        new IlBin(IlType.U32, BinOp.And,
                            new IlIntrin(IlType.U32, "PackHalf2x16", lo, hi),
                            C(0xFFFF0000ul)),
                        new IlBin(IlType.U32, BinOp.And,
                            new IlCast(IlType.U32, CastKind.Bitcast, rdOld),
                            C(0xFFFFul)))),
            _ => lo,
        };
    }

    // SrcPred (the 3-bit pred-select + 1-bit invert that FMNMX/FSET/
    // SEL/FSETP all share): P7 = always-true; inv → !pred.
    static Il SrcPred(ulong n, ulong inv) {
        var p = Pred(n);
        return inv == 1 ? new IlUn(IlType.U1, UnOp.Not, p) : p;
    }
    static Il Pred(ulong n) => n == 7
        ? new IlConst(IlType.U1, 1)
        : new IlReadReg(IlType.U1, RegKind.Pred, (int) n);
    // SSY/PBK push-target stack. v1 instance-state (per-
    // shader; cleared at Lift() entry). Tuples (kind, target).
    readonly Stack<(string, ulong)> _ssyStack = new();
        static Il C(ulong v) => new IlConst(U32, v);
    static Il Cf(uint bits) => new IlConst(F32, bits);
    // Bitcast helpers for the typeless-32-bit GPR ↔ u32-arith bridge.
    static Il Bu32(Il f) => new IlCast(IlType.U32, CastKind.Bitcast, f);
    static Il Bf32(Il u) => new IlCast(F32, CastKind.Bitcast, u);
    static Il Neg(Il x, ulong neg) => neg == 1 ? new IlUn(F32, UnOp.Neg, x) : x;
    static Il Abs(Il x, ulong abs) => abs == 1 ? new IlUn(F32, UnOp.Abs, x) : x;
    static Il Fsrc(Il x, ulong neg, ulong abs) => Neg(Abs(x, abs), neg);

    // Lift one 64-bit instruction word → IlBlock. Returns (def, il).
    public (MaxwellDef def, Il il) Lift(ulong w, ulong pc) {
        var d = Decode(w);
        if(d == null)
            return (null, new IlUnimpl("decode", $"no def matched 0x{w:x016}"));

        // Field extraction. Missing fields → ~0ul sentinel (matches
        // MaxwellGenerator/Program.cs Disasm convention).
        ulong F(string n) => d.Fields.TryGetValue(n, out var f)
            ? (w >> f.Shift) & ((1ul << f.Bits) - 1) : ~0ul;

        // Universal field positions (same across ~all Maxwell ALU
        // insns). Read RAW per kt[6] — the .isa (names …) maps are
        // for disasm/spec; for the lift, hardcoding the well-known
        // bit positions is more robust against gen-skeleton's letter-
        // collision bugs + my hand-written placement errors. The
        // form-discriminating fields (slot/off/imm/rb-position) ARE
        // taken from F() since those vary by R/I/C/Rc form.
        ulong B(int sh, int n) => (w >> sh) & ((1ul << n) - 1);
        var pred = B(16, 3); var pinv = B(19, 1);
        var rd = B(0, 8); var ra = B(8, 8);
        var rb = F("rb"); var rc = F("rc");          // form-specific
        var slot = F("slot"); var off = F("off"); var imm = F("imm");
        // Integer Imm20: 19-bit value @20 + sign@56. For I-form
        // arith (IADD-I/SHL-I/LOP-I/ISETP-I). ‡ Sign-extend? Most
        // uses are positive shift-amounts/masks; v0 = zero-extend
        // the 20 bits (matches InstIaddI's `(int)((>>37&0x80000)|…)`
        // which DOES set bit 19 for sign but doesn't sign-extend
        // beyond — = a 20-bit signed value).
        ulong Iimm20() {
            var lo = B(20, 19); var sgn = B(56, 1);
            return (ulong)(int)((sgn == 1 ? 0xFFF80000u : 0) | (uint)lo);
        }
        // I-form half2-immediate: BimmH0=bits 20:10 (10b), BimmH1=
        // (bit56<<9)|bits 30:9 (split — bit-9 lives at @56 because
        // bits 39-46 = SrcC, 47-48 = ASwizzle, 49-50 = OFmt. The
        // top-10 of each f16: sign+exp5+mant4; <<6 for full f16).
        // InstHfma2I/InstHadd2I:2406-7/2333-4. Returns (lo,hi) as
        // f32 IlConsts.
        (Il, Il) HImm2() {
            var h0 = (ushort)(B(20, 10) << 6);
            var h1 = (ushort)(((B(56, 1) << 9) | B(30, 9)) << 6);
            static Il H2F(ushort h) => Cf(BitConverter.SingleToUInt32Bits(
                (float)BitConverter.UInt16BitsToHalf(h)));
            return (H2F(h0), H2F(h1));
        }
        // Float modifier bits (per InstFadd*/InstFfma*/InstFmul* — same
        // positions across R/I/C forms). NOT all insns have all of
        // these; reading absent bits is harmless (the case below
        // only applies the ones that exist for that mnemonic).
        // ⚠️ FMUL has DIFFERENT nega position (bit 48, not 49 — InstFmul
        // has no AbsA, NegB at 48). FADD has both AbsA@46 + NegB@45.
        // FFMA has NegA@48 + NegC@49, no abs. Per-mnemonic below.

        // Cbuf operand for C-form. Off is in 4-byte WORD units (per
        // ‡ ryujinx; verify against captured slot-2 mat4 reads = M1
        // oracle work). Convert to byte offset for IlCbufLoad.
        Il Cbuf() => new IlCbufLoad(F32, (int) slot, C(off * 4));

        // Float imm20 (I-form): low 19 bits @imm + sign bit @sgn → top
        // 20 bits of an f32 (low 12 = 0).
        Il Fimm20() {
            var sgn = F("sgn");
            var bits = ((uint)(sgn == 1 ? 1u : 0) << 31) | ((uint) imm << 12);
            return Cf(bits);
        }

        var body = new List<Il>();

        switch(d.Name) {
            case "NOP": break;

            case "EXIT":
                body.Add(new IlExit());
                break;

            case "BRA": {
                // Imm24 sign-extended byte offset from next-pc. ‡ v0
                // emits IlBranch; control-flow graph = M1.5.
                var sext = (long)(imm << 40) >> 40;  // 24-bit sext
                body.Add(new IlBranch(BranchKind.Jmp,
                    new IlConst(IlType.U64, (UInt128)(pc + 8 + (ulong)sext))));
                break;
            }

            case "MOV32I":
                body.Add(SetGpr(rd, new IlCast(F32, CastKind.Bitcast, C(imm))));
                break;

            case "MOV-R":
                // gen-skeleton named SrcB@20:8 as `ra` (only src-reg).
                body.Add(SetGpr(rd, Gpr(F("ra"))));
                break;
            case "MOV-C":
                body.Add(SetGpr(rd, Cbuf()));
                break;

            case "ALD": {
                // Multi-reg load: sz+1 consecutive Rd..Rd+sz from
                // attr[imm], attr[imm+4], ... ‡ ra-indexed not handled.
                var sz = F("sz");
                for(var k = 0ul; k <= sz; k++)
                    body.Add(SetGpr(rd + k,
                        new IlAttrLoad(F32, C(imm + k*4))));
                break;
            }
            case "AST": {
                // Source reg is in the rb position (per InstAst.SrcB
                // at bits[7:0]). Multi-reg store mirrors ALD.
                var sz = F("sz");
                for(var k = 0ul; k <= sz; k++)
                    body.Add(new IlAttrStore(C(imm + k*4), Gpr(rb + k)));
                break;
            }

            case "MUFU":
                // InstMufu: MufuOp@20:4, NegA@48, AbsA@46, Sat@50.
                body.Add(SetGpr(rd, new IlMufu(F32, (int) B(20,4),
                    Fsrc(Gpr(ra), B(48,1), B(46,1)))));
                break;

            case "IPA": {
                // interp(attr[imm], mode) × Rb (Rb=RZ → ×1 effectively
                // since the multiply is identity in pass mode). ‡ sat
                // not applied v0.
                var mode = (int) F("mode");
                body.Add(SetGpr(rd, new IlInterp(F32, C(imm), mode, Gpr(rb))));
                break;
            }

            // ── Float arithmetic — R/I/C/Rc forms ──
            // Modifier bit positions per InstFadd* (verified at-source
            // InstDecoders.cs:1751-1764): NegA@48, NegB@45, AbsA@46,
            // AbsB@49, Sat@50, Ftz@44. Same for R/I/C (only opnd-B
            // source position differs).
            case "FADD-R": case "FADD-C": case "FADD-I": {
                var a = Fsrc(Gpr(ra), B(48,1), B(46,1));
                var b = d.Name switch {
                    "FADD-R" => Gpr(B(20,8)), "FADD-C" => Cbuf(),
                    "FADD-I" => Fimm20(), _ => throw new() };
                b = Fsrc(b, B(45,1), B(49,1));
                body.Add(SetGpr(rd, new IlBin(F32, BinOp.Add, a, b)));
                break;
            }
            case "FMUL-R": case "FMUL-C": case "FMUL-I": case "FMUL32I":
            case "FADD32I": {
                var a = Gpr(ra);
                // 32I forms: imm32 @ bits[51:20] (gen-skeleton'd defs
                // didn't parse it as a named field — InstFmul32i.Imm32
                // is `(int)(_opcode>>20)` no mask = my parser missed the
                // pattern). Read raw per kt[6].
                Il Imm32() => new IlCast(F32, CastKind.Bitcast,
                    C((w >> 20) & 0xFFFFFFFFul));
                var b = d.Name switch {
                    "FMUL-R" => Gpr(B(20,8)), "FMUL-C" => Cbuf(),
                    "FMUL-I" => Fimm20(),
                    "FMUL32I" or "FADD32I" => Imm32(),
                    _ => throw new() };
                if(d.Name == "FADD32I") {
                    // InstFadd32i: NegA@56, AbsA@54, NegB@53, AbsB@57.
                    a = Fsrc(a, B(56,1), B(54,1));
                    b = Fsrc(b, B(53,1), B(57,1));
                    body.Add(SetGpr(rd, new IlBin(F32, BinOp.Add, a, b)));
                    break;
                }
                // InstFmul* (R/I/C): only NegA@48 (no NegB/Abs*).
                // FMUL32I: Sat@55, no neg.
                if(d.Name != "FMUL32I") a = Neg(a, B(48,1));
                body.Add(SetGpr(rd, new IlBin(F32, BinOp.Mul, a, b)));
                break;
            }
            case "FFMA-R": case "FFMA-C": case "FFMA-I": case "FFMA-Rc":
            case "FFMA-RC": {
                // InstFfma* (R/I/C/Rc, verified InstDecoders.cs:1992-):
                // NegA@48, NegC@49, Sat@50. SrcC always @39:8 (named
                // Rc but for Rc-FORM it's actually the B-reg — operand
                // swap; the cbuf is C). SrcB-reg @20:8 for R-form only.
                var a = Neg(Gpr(ra), B(48,1));
                var rcReg = B(39, 8);
                Il srcb, srcc;
                if(d.Name is "FFMA-Rc" or "FFMA-RC") {
                    srcb = Gpr(rcReg);    // Rc-form: reg @39 = B-operand
                    srcc = Cbuf();        // cbuf = C-operand
                } else {
                    srcb = d.Name switch {
                        "FFMA-R" => Gpr(B(20,8)),
                        "FFMA-C" => Cbuf(),
                        "FFMA-I" => Fimm20(), _ => throw new() };
                    srcc = Gpr(rcReg);
                }
                srcc = Neg(srcc, B(49,1));
                // ‡ No native IlFma — emit (a*b)+c. SPIR-V backend can
                // pattern-match it back to OpFma if it wants.
                body.Add(SetGpr(rd, new IlBin(F32, BinOp.Add,
                    new IlBin(F32, BinOp.Mul, a, srcb), srcc)));
                break;
            }
            case "FMNMX-R": case "FMNMX-C": case "FMNMX-I": {
                // Rd = SrcPred ? min(a,b) : max(a,b). Per InstFmnmxC
                // (verified): NegA@48 AbsA@46 NegB@45 AbsB@49 (= same
                // as FADD), SrcPred@39:3 SrcPredInv@42.
                var a = Fsrc(Gpr(ra), B(48,1), B(46,1));
                var b = d.Name switch {
                    "FMNMX-R" => Gpr(B(20,8)), "FMNMX-C" => Cbuf(),
                    "FMNMX-I" => Fimm20(), _ => throw new() };
                b = Fsrc(b, B(45,1), B(49,1));
                var sp = SrcPred(B(39,3), B(42,1));
                var fmin = new IlBin(F32, BinOp.FMin, a, b);
                var fmax = new IlBin(F32, BinOp.FMax, a, b);
                // (T6)×44 vane ·11136 const-fold: SrcPred(7,inv)
                // → IlConst(U1,c) via Pred(7); fold the IlIfV at
                // lift-time so we emit only the chosen arm (drops
                // ~654 FMin + ~108 FMax from the §7-oracle diff;
                // STYLE only — OpSelect-on-const was correct).
                body.Add(SetGpr(rd, sp is IlConst(_, var spc)
                    ? (spc != 0 ? fmin : fmax)
                    : new IlIfV(F32, sp, fmin, fmax)));
                break;
            }

            // ── FSET: float compare → GPR (1.0/0.0 if BF, else
            // bitmask -1/0). Same compare structure as FSETP. ──
            case "FSET-R": case "FSET-C": case "FSET-I": {
                // InstFsetC: NegA@43 AbsA@54 NegB@53 AbsB@44 (DIFFERENT
                // from FADD/FMNMX!). FComp@48:4, BF@52, SrcPred@39:3,
                // SrcPredInv@42, Bop@45:2.
                var a = Fsrc(Gpr(ra), B(43,1), B(54,1));
                var b = d.Name switch {
                    "FSET-R" => Gpr(B(20,8)), "FSET-C" => Cbuf(),
                    "FSET-I" => Fimm20(), _ => throw new() };
                b = Fsrc(b, B(53,1), B(44,1));
                var fcomp = (int) B(48, 4);
                var cop = (fcomp & 7) switch {
                    1 => BinOp.Slt, 2 => BinOp.Eq, 3 => BinOp.Sle,
                    4 => BinOp.Sgt, 5 => BinOp.Ne, 6 => BinOp.Sge,
                    _ => BinOp.Eq };
                Il cmp = new IlBin(IlType.U1, cop, a, b);
                var fsp = B(39,3); var fspi = B(42,1);
                if(!(fsp == 7 && fspi == 0)) {
                    var bo = B(45,2) switch {
                        0 => BinOp.And, 1 => BinOp.Or,
                        2 => BinOp.Xor, _ => BinOp.And };
                    cmp = new IlBin(IlType.U1, bo, cmp, SrcPred(fsp, fspi));
                }
                // BF=1 → 1.0/0.0 (float bool). BF=0 → 0xFFFFFFFF/0
                // (= -NaN as float; the int-bitmask form). v0: emit
                // IlIfV(F32, cmp, 1.0, 0.0) for BF=1; for BF=0 emit
                // IlIfV(F32, cmp, bitcast(-1u), 0.0). The downstream
                // (FMUL on the result for masking) works either way
                // since 1.0×x=x and -NaN×x=NaN — ‡ but BF=0 is the
                // integer-mask form, used with LOP/IADD not FMUL.
                // Most uses are BF=1 (per the game's FSET-I freq).
                var bf = B(52, 1);
                var one = bf == 1 ? Cf(0x3f800000u)   // 1.0f
                    : Cf(0xFFFFFFFFu);                // -NaN = int -1 mask
                body.Add(SetGpr(rd, new IlIfV(F32, cmp, one, Cf(0))));
                break;
            }

            // ── SEL: Rd = SrcPred ? Ra : B. Bitwise select on full
            // 32-bit value (typeless; F32 in IL is fine since SpirvEmit
            // OpSelect on f32 works regardless of the bit pattern). ──
            case "SEL-R": case "SEL-C": case "SEL-I": {
                var b = d.Name switch {
                    "SEL-R" => Gpr(B(20,8)), "SEL-C" => Cbuf(),
                    "SEL-I" => new IlCast(F32, CastKind.Bitcast,
                        C((ulong)(((w>>37)&0x80000)|((w>>20)&0x7FFFF)))),
                    _ => throw new() };
                var sp = SrcPred(B(39,3), B(42,1));
                body.Add(SetGpr(rd, new IlIfV(F32, sp, Gpr(ra), b)));
                break;
            }

            // ── RRO: range-reduction operand prep (sin/cos). On
            // Maxwell hw this preconditions the value for MUFU.sin/
            // cos (which take pre-reduced input). SPIR-V's GLSL.std.
            // 450 sin/cos do their own reduction → RRO is identity
            // at IL level (just apply the abs/neg modifiers). ──
            case "RRO-R": case "RRO-C": case "RRO-I": {
                // InstRroR: SrcB@20:8, NegB@45 AbsB@49.
                var b = d.Name switch {
                    "RRO-R" => Gpr(B(20,8)), "RRO-C" => Cbuf(),
                    "RRO-I" => Fimm20(), _ => throw new() };
                body.Add(SetGpr(rd, Fsrc(b, B(45,1), B(49,1))));
                break;
            }

            // ── DEPBAR: GPU dependency barrier (texture/load
            // scoreboard sync). Scheduling hint, no IL semantics. ──
            case "DEPBAR":
                break;

            // ════════════════════════════════════════════════════
            // ── Conversions: I2F/F2I/I2I/F2F. All share AbsB@49,
            //    NegB@45, SrcB@20:8 (R-form). Format encoding: low
            //    2 bits = size (0=8b 1=16b 2=32b 3=64b), bit 2 =
            //    signed (for I-side). DstFmt for floats: 1=f16
            //    2=f32 3=f64. v0: u32/s32 ↔ f32 only (the dominant
            //    case in 364 shaders); 8/16/64-bit + ByteSel ‡. ──
            // ════════════════════════════════════════════════════
            case "I2F-R": case "I2F-C": case "I2F-I": {
                // Int → float. ISrcFmt = sign@13 | size@10:2.
                // DstFmt@8:2 (1=f16 2=f32 3=f64).
                var srcFmt = (B(13,1) << 2) | B(10,2);
                var signed = (srcFmt & 4) != 0;
                var b = d.Name switch {
                    "I2F-R" => Gpr(B(20,8)), "I2F-C" => Cbuf(),
                    "I2F-I" => C((ulong)(int)(short) B(20,16)),
                    _ => throw new() };
                // Bitcast f32→u32, extract sub-word per ByteSel@41:2
                // (ISrcFmt: 0/4=8b 1/5=16b 2/6=32b), sign-extend if
                // signed, then SToF/UToF. abs/neg AFTER convert.
                var u = new IlCast(IlType.U32, CastKind.Bitcast, b);
                var bsel = (int) B(41, 2);
                var size = (srcFmt & 3) switch
                    { 0 => 8, 1 => 16, 2 => 32, _ => 32 /*‡64*/ };
                Il sw = u;
                if(size < 32) {
                    // (u >> (bsel*size)) & mask, then sign-extend by
                    // (x<<(32-size))>>>(32-size) signed-shr.
                    if(bsel != 0)
                        sw = new IlBin(IlType.U32, BinOp.Shr, sw,
                            C((ulong)(bsel * size)));
                    sw = new IlBin(IlType.U32, BinOp.And, sw,
                        C((1ul << size) - 1));
                    if(signed) {
                        var sh = C((ulong)(32 - size));
                        sw = new IlBin(IlType.U32, BinOp.Sar,
                            new IlBin(IlType.U32, BinOp.Shl, sw, sh), sh);
                    }
                }
                Il r = new IlCast(F32,
                    signed ? CastKind.SToF : CastKind.UToF, sw);
                r = Fsrc(r, B(45,1), B(49,1));
                body.Add(SetGpr(rd, r));
                if((srcFmt & 3) == 3)
                    body.Add(new IlNote($"‡ I2F srcFmt={srcFmt} =64-bit"));
                break;
            }
            case "F2I-R": case "F2I-C": case "F2I-I": {
                // Float → int. SrcFmt@10:2 (DstFmt enum: 2=f32).
                // IDstFmt = sign@12 | size@8:2. RoundMode2@39:2
                // (0=rn 1=rm/floor 2=rp/ceil 3=rz/trunc).
                var dstFmt = (B(12,1) << 2) | B(8,2);
                var signed = (dstFmt & 4) != 0;
                var rm = B(39, 2);
                var b = Fsrc(d.Name switch {
                    "F2I-R" => Gpr(B(20,8)), "F2I-C" => Cbuf(),
                    "F2I-I" => Fimm20(), _ => throw new() },
                    B(45,1), B(49,1));
                // Apply rounding (Floor/Ceil/Round/Trunc) BEFORE the
                // FToI cast — SPIR-V's ConvertFToS/U truncates, so
                // floor(x) → trunc-to-int gets the right rm=1 result.
                if(rm != 3)
                    b = new IlUn(F32, rm switch {
                        0 => UnOp.Round, 1 => UnOp.Floor,
                        2 => UnOp.Ceil, _ => UnOp.Round }, b);
                // Result u32 → bitcast back to f32 for storage in
                // _gpr (typeless). Downstream int-ops bitcast back.
                body.Add(SetGpr(rd, new IlCast(F32, CastKind.Bitcast,
                    new IlCast(IlType.U32, signed ? CastKind.FToSI
                        : CastKind.FToUI, b))));
                break;
            }
            case "F2F-R": case "F2F-C": case "F2F-I": {
                // Float → float (size or rounding change). RoundMode
                // = bit42<<2 | bits[40:39] → IntegerRound enum.
                //
                // (T6)×134 ×2 = 73rd: srcFmt/dstFmt/Sh handling. Own
                // ‡@504 "f16/f64 → ‡unimpl when surface" SURFACED as
                // ‡v0×30th (×133): fs442's HDR-encode does F2F.F16.
                // F16.FLOOR on a packed-half-pair register; pre-73rd
                // floored the f32-bitcast (= NaN when high-half =
                // 0xff80) ⟹ %846=.x=0 ⟹ %881=1/0=∞ ⟹ c[0].rgb=∞-
                // clamp(255) + c[0].α=0 ⟹ #163 fs50×0²=0 ⟹ rt2-base
                // =0 ⟹ 68%-black at [F]. = #153×11th (own deferred-‡
                // in own code) + kt[26] (deferred-with-‡ unstated-
                // bound). Corpus (×140×3(b-3), ×106th-corrected:
                // ×134(d)'s scan was @0x50; insns @0x80 = NVN-
                // prefix 0x00-0x2F + SPH 0x30-0x7F): 212/1451
                // shaders use F2F with non-F32 fmt; ALL single
                // combo (F16,F16,sh=0,neg=1,floor,sat=0) =
                // exactly fs442's @0x0a30. Verified 4-ways (T6)
                // ×141: at-source(×137(b) HUnpack sw=0=F16-
                // unpack docstring) + at-input-bytes(×140(b-2)
                // neg@45=1 INSIDE-F2F) + at-emit-shape(×141(f-3)
                // post73=unpack→.x→neg→floor→pack-merge) +
                // empirical(r206a c[0].α 0%→95.4%).
                //
                // Per kt[14] ryujinx InstEmitConversion.cs:344
                // UnpackReg + InstDecoders.cs:1591 InstF2fR: srcFmt
                // @10:2, dstFmt@8:2, Sh@41. F16-src ⟹ extract Sh-
                // selected ONE half BEFORE abs/neg/rmode (own·8808-
                // mild on ×133×2(p) "floors each half" — Maxwell F2F
                // is single-half via Sh, NOT packed-SIMD). F16-dst ⟹
                // result to Sh-half of rd, other-half preserved (=
                // HPack MrgH0/MrgH1). ‡ F64 → comment-only (0 corpus
                // hits in 1451).
                var rmHi = B(42,1); var rmLo = B(39,2);
                var rmode = (rmHi << 2) | rmLo;
                var srcFmt = B(10,2); var dstFmt = B(8,2);
                var sh = B(41,1);
                var rawSrc = d.Name switch {
                    "F2F-R" => Gpr(B(20,8)), "F2F-C" => Cbuf(),
                    "F2F-I" => Fimm20(), _ => throw new() };
                Il b;
                if(srcFmt == 1) {
                    // F16-src: unpack both, take Sh-selected, THEN
                    // abs/neg on that f32 half.
                    var (lo, hi) = HUnpack(rawSrc, 0, 0, 0);
                    b = Fsrc(sh == 1 ? hi : lo, B(45,1), B(49,1));
                } else {
                    // F32-src (or F64-‡): existing v0 path.
                    b = Fsrc(rawSrc, B(45,1), B(49,1));
                }
                // IntegerRound: 1=Pass, 4=Round, 5=Floor, 6=Ceil,
                // 7=Trunc (per ryujinx InstDecoders.cs:252-258).
                // (T6)×43 mcb (Δ-3+Δ-4): was off-by-one (4=Floor
                // 5=Ceil 6=Round 7=unmapped) — own-comment had
                // IEEE rm/rp/rz names at wrong indices. Verified
                // at-data via histogram conservation: ours pre-fix
                // Ceil=37+Floor=32=69 = ryujinx Floor=69+Ceil=0.
                if(rmode == 4) b = new IlUn(F32, UnOp.Round, b);
                else if(rmode == 5) b = new IlUn(F32, UnOp.Floor, b);
                else if(rmode == 6) b = new IlUn(F32, UnOp.Ceil, b);
                else if(rmode == 7) b = new IlUn(F32, UnOp.Trunc, b);
                // .SAT@50 handled at wrapper-site below (F2F is in
                // the bit-50 allowlist). ‡‡ Sat+F16-dst: wrapper sees
                // the HPack'd-bitcast value not the f32-result ⟹
                // saturating that = wrong. ‡ 0 known corpus hits;
                // ×134×3-census-residual to verify.
                if(dstFmt == 1) {
                    // F16-dst: pack result into Sh-half of rd, merge
                    // other half from rd's prior content. HPack ofmt
                    // =2(MrgH0)=write-lo-keep-hi; =3(MrgH1)=write-hi-
                    // keep-lo. Pass (b,b); ofmt picks the right lane.
                    // ‡ (b,b) emit-wasteful (×141(f-3): post73 shows
                    // dup %836-840 = both halves built then one
                    // masked-away). Semantically correct (verified
                    // at-emit-shape ×141); HPack-MrgH{0,1} only uses
                    // one. (b,Const0) would halve emit but touches
                    // verified-working code ⟹ deferred per kt[13].
                    body.Add(SetGpr(rd,
                        HPack((b, b), sh == 1 ? 3ul : 2ul, Gpr(rd))));
                } else {
                    body.Add(SetGpr(rd, b));
                }
                break;
            }
            case "I2I-R": case "I2I-C": case "I2I-I": {
                // Int → int (size/sign change + sat). For 32→32 it's
                // identity or saturate-clamp. v0 = bitcast pass-through
                // (the common 32→32 case = the game using I2I to apply
                // abs/neg on an int — ‡ how do you abs/neg a typeless
                // GPR? Maxwell does it as int; emit as f32 abs/neg
                // since storage is f32-bitcast). ‡ ByteSel + sat + 8/16
                // → IlNote.
                var srcFmt = (B(13,1) << 2) | B(10,2);
                var dstFmt = (B(12,1) << 2) | B(8,2);
                var b = d.Name switch {
                    "I2I-R" => Gpr(B(20,8)), "I2I-C" => Cbuf(),
                    "I2I-I" => C((ulong)(int)(short) B(20,16)),
                    _ => throw new() };
                // The 280 corpus uses are ALL src=6 dst=6 (= S32→S32
                // identity) with abs=1 (265× plain |x|, 15× -|x|).
                // ⟹ Int-abs/neg on a 32-bit value:
                //   abs: u = (x XOR (x>>>31)) - (x>>>31) [branchless]
                //   neg: 0 - u
                // Then bitcast back. Sub-32-bit + sat + ByteSel ‡.
                Il u = new IlCast(IlType.U32, CastKind.Bitcast, b);
                var abs = B(49,1); var neg = B(45,1); var sat = B(50,1);
                if(abs == 1) {
                    var sgn = new IlBin(IlType.U32, BinOp.Sar, u, C(31ul));
                    u = new IlBin(IlType.U32, BinOp.Sub,
                        new IlBin(IlType.U32, BinOp.Xor, u, sgn), sgn);
                }
                if(neg == 1)
                    u = new IlBin(IlType.U32, BinOp.Sub, C(0ul), u);
                body.Add(SetGpr(rd, new IlCast(F32, CastKind.Bitcast, u)));
                if((srcFmt & 3) != 2 || (dstFmt & 3) != 2 || sat == 1)
                    body.Add(new IlNote($"‡ I2I src={srcFmt} dst={dstFmt} "
                        + $"sat={sat} bsel={B(41,2)} (sub-32 or sat)"));
                break;
            }

            // ════════════════════════════════════════════════════
            // ── Integer arithmetic: IADD/SHL/SHR/LOP/IADD32I.
            //    GPRs are typeless 32-bit; emit IlBin(U32, …) on
            //    bitcast'd operands, bitcast result back to f32. ──
            // ════════════════════════════════════════════════════
            case "IADD-R": case "IADD-C": case "IADD-I": case "IADD32I": {
                // ‡ AvgMode@48:2 (NegA/NegB encoding) + X(carry-in)
                // ignored v0. The common case = plain add.
                Il a = Bu32(Gpr(ra));
                Il b = d.Name switch {
                    "IADD-R" => Bu32(Gpr(B(20,8))), "IADD-C" => Bu32(Cbuf()),
                    "IADD-I" => C(Iimm20()),
                    "IADD32I" => C((w >> 20) & 0xFFFFFFFF),
                    _ => throw new() };
                // AvgMode: 1=NegA, 2=NegB (per InstIaddR @48:2;
                // ‡ verify — assuming standard).
                var avg = B(48, 2);
                if(avg == 2) b = new IlUn(IlType.U32, UnOp.Neg, b);
                if(avg == 1) a = new IlUn(IlType.U32, UnOp.Neg, a);
                body.Add(SetGpr(rd, Bf32(
                    new IlBin(IlType.U32, BinOp.Add, a, b))));
                break;
            }
            case "SHL-R": case "SHL-C": case "SHL-I":
            case "SHR-R": case "SHR-C": case "SHR-I": {
                var isShr = d.Name.StartsWith("SHR");
                var b = d.Name[4..] switch {
                    "R" => Bu32(Gpr(B(20,8))), "C" => Bu32(Cbuf()),
                    "I" => C(Iimm20()), _ => throw new() };
                // ‡ M@39 (mask to 5 bits), X@43, signed (SHR only).
                body.Add(SetGpr(rd, Bf32(
                    new IlBin(IlType.U32, isShr ? BinOp.Shr : BinOp.Shl,
                        Bu32(Gpr(ra)), b))));
                break;
            }
            case "LOP-R": case "LOP-C": case "LOP-I": case "LOP32I": {
                // Lop@41:2 (0=AND 1=OR 2=XOR 3=PASS_B). NegA@39
                // NegB@40 = bitwise NOT, not arith neg.
                var lop = B(41, 2);
                Il a = Bu32(Gpr(ra)); if(B(39,1)==1) a = new IlUn(IlType.U32, UnOp.Not, a);
                Il b = d.Name switch {
                    "LOP-R" => Bu32(Gpr(B(20,8))), "LOP-C" => Bu32(Cbuf()),
                    "LOP-I" => C(Iimm20()),
                    "LOP32I" => C((w >> 20) & 0xFFFFFFFF),
                    _ => throw new() };
                if(B(40,1)==1) b = new IlUn(IlType.U32, UnOp.Not, b);
                Il r = lop switch {
                    0 => new IlBin(IlType.U32, BinOp.And, a, b),
                    1 => new IlBin(IlType.U32, BinOp.Or,  a, b),
                    2 => new IlBin(IlType.U32, BinOp.Xor, a, b),
                    3 => b, _ => throw new() };
                body.Add(SetGpr(rd, Bf32(r)));
                // ‡ DestPred@48:3 + PredicateOp@44:2 (= write a pred
                // from result==0 or sign) ignored v0.
                break;
            }

            // ── ISETP: int compare → predicate. Same shape as
            //    FSETP. IComp@49:3 (1=LT 2=EQ 3=LE 4=GT 5=NE 6=GE),
            //    Signed@48. DestPred@3, DestPredInv@0, SrcPred@39,
            //    SrcPredInv@42, Bop@45:2. ──
            case "ISETP-R": case "ISETP-C": case "ISETP-I": {
                var sgnd = B(48, 1) == 1;
                var icmp = (int) B(49, 3);
                var cop = (icmp & 7) switch {
                    1 => sgnd ? BinOp.Slt : BinOp.Ult,
                    2 => BinOp.Eq, 5 => BinOp.Ne,
                    3 => sgnd ? BinOp.Sle : BinOp.Ule,
                    4 => sgnd ? BinOp.Sgt : BinOp.Ugt,
                    6 => sgnd ? BinOp.Sge : BinOp.Uge,
                    _ => BinOp.Eq };
                var a = Bu32(Gpr(ra));
                var b = d.Name switch {
                    "ISETP-R" => Bu32(Gpr(B(20,8))),
                    "ISETP-C" => Bu32(Cbuf()),
                    "ISETP-I" => C(Iimm20()), _ => throw new() };
                Il cmp = new IlBin(IlType.U1, cop, a, b);
                var fsp = B(39,3); var fspi = B(42,1);
                if(!(fsp == 7 && fspi == 0)) {
                    var bo = B(45,2) switch {
                        0 => BinOp.And, 1 => BinOp.Or,
                        2 => BinOp.Xor, _ => BinOp.And };
                    cmp = new IlBin(IlType.U1, bo, cmp, SrcPred(fsp,fspi));
                }
                var dp = B(3,3); var dpi = B(0,3);
                if(dp != 7) body.Add(new IlWriteReg(RegKind.Pred, (int)dp, cmp));
                if(dpi != 7) body.Add(new IlWriteReg(RegKind.Pred, (int)dpi,
                    new IlUn(IlType.U1, UnOp.Not, cmp)));
                break;
            }
            case "ISET-R": case "ISET-C": case "ISET-I": {
                // Int compare → GPR (= ISETP shape, result to Rd as
                // BF@44 ? 1.0/0.0 : -1mask/0). IComp@49:3, Signed@48,
                // SrcPred@39, Bop@45:2.
                var sgnd = B(48, 1) == 1;
                var icmp = (int) B(49, 3);
                var cop = (icmp & 7) switch {
                    1 => sgnd ? BinOp.Slt : BinOp.Ult,
                    2 => BinOp.Eq, 5 => BinOp.Ne,
                    3 => sgnd ? BinOp.Sle : BinOp.Ule,
                    4 => sgnd ? BinOp.Sgt : BinOp.Ugt,
                    6 => sgnd ? BinOp.Sge : BinOp.Uge,
                    _ => BinOp.Eq };
                var a = Bu32(Gpr(ra));
                var b = d.Name switch {
                    "ISET-R" => Bu32(Gpr(B(20,8))),
                    "ISET-C" => Bu32(Cbuf()),
                    "ISET-I" => C(Iimm20()), _ => throw new() };
                Il cmp = new IlBin(IlType.U1, cop, a, b);
                var fsp = B(39,3); var fspi = B(42,1);
                if(!(fsp == 7 && fspi == 0)) {
                    var bo = B(45,2) switch {
                        0 => BinOp.And, 1 => BinOp.Or,
                        2 => BinOp.Xor, _ => BinOp.And };
                    cmp = new IlBin(IlType.U1, bo, cmp, SrcPred(fsp,fspi));
                }
                var bf = B(44, 1);
                var one = bf == 1 ? Cf(0x3f800000u) : Cf(0xFFFFFFFFu);
                body.Add(SetGpr(rd, new IlIfV(F32, cmp, one, Cf(0))));
                break;
            }

            // ── PSETP: pred bop pred → pred. (P1 bopAB P2) bopC
            //    SrcPred → DestPred / DestPredInv. ──
            case "PSETP": {
                var p1 = SrcPred(B(12,3), B(15,1));
                var p2 = SrcPred(B(29,3), B(32,1));
                var bAB = B(24,2) switch {
                    0 => BinOp.And, 1 => BinOp.Or, 2 => BinOp.Xor, _ => BinOp.And };
                Il r = new IlBin(IlType.U1, bAB, p1, p2);
                var fsp = B(39,3); var fspi = B(42,1);
                if(!(fsp == 7 && fspi == 0)) {
                    var bC = B(45,2) switch {
                        0 => BinOp.And, 1 => BinOp.Or, 2 => BinOp.Xor, _ => BinOp.And };
                    r = new IlBin(IlType.U1, bC, r, SrcPred(fsp,fspi));
                }
                var dp = B(3,3); var dpi = B(0,3);
                if(dp != 7) body.Add(new IlWriteReg(RegKind.Pred, (int)dp, r));
                if(dpi != 7) body.Add(new IlWriteReg(RegKind.Pred, (int)dpi,
                    new IlUn(IlType.U1, UnOp.Not, r)));
                break;
            }

            // ════════════════════════════════════════════════════
            // ── XMAD: 16×16+32 fused multiply-add. The compiler-
            //    emitted 32-bit imul (3× XMAD = lo×lo + (hi×lo+
            //    lo×hi)<<16). Per InstEmitIntegerArithmetic.cs:526+
            //    (read for understanding):
            //      Rd = ext16(A,hiA,sA) × ext16(B,hiB,sB)
            //           [<<16 if PSL]
            //           + cop(C, B_unmod)
            //      if MRG: Rd = (Rd & 0xFFFF) | (B_unmod << 16)
            //    cop: Cfull=C; Clo=C&0xFFFF; Chi=C>>16; Cbcc=C+(B<<16);
            //         Csfu=C ± sign-adjust (‡ v0 = note + Cfull).
            //    ⚠️ Per-form bit layouts DIFFER (R/I share most;
            //    C has Mrg/Psl/X at 56/55/54 + 2-bit cop; Rc has
            //    NO Mrg/Psl). HiloA always @53. ──
            // ════════════════════════════════════════════════════
            case "XMAD-R": case "XMAD-I": case "XMAD-C": case "XMAD-RC": {
                ulong hiA = B(53,1), aS = B(48,1), bS = B(49,1);
                ulong hiB, psl, mrg, cop;
                Il srcB, srcC;
                switch(d.Name) {
                    case "XMAD-R":
                        hiB=B(35,1); psl=B(36,1); mrg=B(37,1); cop=B(50,3);
                        srcB=Bu32(Gpr(B(20,8))); srcC=Bu32(Gpr(B(39,8))); break;
                    case "XMAD-I":
                        hiB=0; psl=B(36,1); mrg=B(37,1); cop=B(50,3);
                        srcB=C(B(20,16)); srcC=Bu32(Gpr(B(39,8))); break;
                    case "XMAD-C":
                        hiB=B(52,1); psl=B(55,1); mrg=B(56,1); cop=B(50,2);
                        srcB=Bu32(Cbuf()); srcC=Bu32(Gpr(B(39,8))); break;
                    case "XMAD-RC":
                        hiB=B(52,1); psl=0; mrg=0; cop=B(50,2);
                        srcB=Bu32(Gpr(B(39,8))); srcC=Bu32(Cbuf()); break;
                    default: throw new();
                }
                var srcBunmod = srcB;
                // Extend16To32: high → >>16; low → &0xFFFF. Signed
                // variants need sign-extend (BitfieldExtractS or
                // shl16-then-sar16); v0 = unsigned only (the imul
                // pattern uses unsigned 16-bit halves; signed XMAD
                // ‡-noted).
                Il Ext16(Il s, ulong hi, ulong sgn) {
                    if(sgn == 1) body.Add(new IlNote("‡ XMAD signed-16 v0=unsigned"));
                    return hi == 1
                        ? new IlBin(IlType.U32, BinOp.Shr, s, C(16ul))
                        : new IlBin(IlType.U32, BinOp.And, s, C(0xFFFFul));
                }
                var a = Ext16(Bu32(Gpr(ra)), hiA, aS);
                srcB = Ext16(srcB, hiB, bS);
                Il res = new IlBin(IlType.U32, BinOp.Mul, a, srcB);
                if(psl == 1)
                    res = new IlBin(IlType.U32, BinOp.Shl, res, C(16ul));
                Il c = cop switch {
                    0 => srcC,                                       // Cfull
                    1 => new IlBin(IlType.U32, BinOp.And, srcC, C(0xFFFFul)),  // Clo
                    2 => new IlBin(IlType.U32, BinOp.Shr, srcC, C(16ul)),       // Chi
                    4 => new IlBin(IlType.U32, BinOp.Add, srcC,        // Cbcc
                            new IlBin(IlType.U32, BinOp.Shl, srcBunmod, C(16ul))),
                    3 => srcC,   // Csfu — ‡ v0 = Cfull + note
                    _ => srcC };
                if(cop == 3) body.Add(new IlNote("‡ XMAD Csfu v0=Cfull"));
                res = new IlBin(IlType.U32, BinOp.Add, res, c);
                if(mrg == 1)
                    res = new IlBin(IlType.U32, BinOp.Or,
                        new IlBin(IlType.U32, BinOp.And, res, C(0xFFFFul)),
                        new IlBin(IlType.U32, BinOp.Shl, srcBunmod, C(16ul)));
                body.Add(SetGpr(rd, Bf32(res)));
                break;
            }

            // ════════════════════════════════════════════════════
            // ── H-family: fp16 packed-vec2 ops. v0: unpack→f32-op
            //    →pack via GLSL.std.450 (avoids Float16 cap).
            //    R-form: ASwizzle@47:2 BSwizzle@28:2 OFmt@49:2.
            //    C-form: B is f32 cbuf (sw=F32). I-form: B is 2×
            //    9-bit half-imm (BimmH0@20:9 BimmH1@29:9, <<6).
            //    HFMA2 has CSwizzle@35:2 + SrcC@39:8 (R-form). ──
            // ════════════════════════════════════════════════════
            case "HMUL2-R": case "HADD2-R":
            case "HMUL2-C": case "HADD2-C":
            case "HMUL2-I": case "HADD2-I": {
                var isAdd = d.Name.StartsWith("HADD");
                var ofmt = B(49, 2);
                // A: always Ra w/ ASwizzle@47, NegA varies (HMUL@31,
                // HADD@43), AbsA@44.
                var negA = isAdd ? B(43,1) : B(31,1);
                var (al, ah) = HUnpack(Gpr(ra), B(47,2), negA, B(44,1));
                // B per form. ⚠️ I-form OFmt is at @49 still? ‡
                // R: SrcB@20, BSwizzle@28, NegB@31(HADD)/none(HMUL), AbsB@30.
                // C: cbuf, sw=F32 fixed, NegB@56, AbsB@54.
                // I: 2× 9-bit half-imm @20+@29, <<6.
                Il bl, bh;
                if(d.Name.EndsWith("-R")) {
                    var negB = isAdd ? B(31,1) : 0ul;
                    (bl, bh) = HUnpack(Gpr(B(20,8)), B(28,2), negB, B(30,1));
                } else if(d.Name.EndsWith("-C")) {
                    // (T6)×16 HMUL2-C negate fix. Per InstHmul2C
                    // (ryujinx Decoders/InstDecoders.cs:2510-27):
                    // NegA = bit 43 (0x800_0000_0000), and
                    // InstEmitFloatArithmetic.cs:372 applies it
                    // to srcB (the CBUF operand): GetHalfSrc(…,
                    // cbuf, op.NegA, op.AbsB). There IS no NegB
                    // field. Verified at-bytes (T6)×14×4 on
                    // sh0418: @0x9a8 bit43=0 (ryu GLSL: no neg);
                    // @0x9d0/@0xa70 bit43=1 (ryu: 0.0−c1[…]).
                    // We had B(56,1) here = always-0 in observed
                    // encodings ⟹ negate silently dropped.
                    // HADD2-C: verified (T6)×142×1(a) vs ryujinx
                    // InstHadd2C (InstDecoders.cs): NegA@43→srcA,
                    // NegB@56→cbuf, ASwz@47:2, OFmt@49:2, AbsA@44,
                    // AbsB@54. My emit (negA=B(43,1) above; negCb
                    // =B(56,1) here) MATCHES. ×142×3(h) InstHadd2I
                    // also matches (BimmH0@20:10, BimmH1=(b56<<9)|
                    // b30:9, <<6, NegA@43). ⟹ ‡@855 closed.
                    //
                    // ‡‡-residual (documented-not-fixed per kt[13]
                    // — observationally =0 in all encodings, NOT
                    // a known wrong-output): srcA's negA above
                    // reads B(31,1) for !isAdd. For HMUL2-C bit31
                    // ∈ CbufOffset(20-33); for HMUL2-I bit31 ∈
                    // BimmH1(30-38). ryujinx passes `false` for
                    // HMUL2-C/I srcA-neg (no field exists). =0 in
                    // all corpus encodings (cbuf-offsets <0x800;
                    // imm-bit1 happens-to-be-0 in fs442's 3 HADD2-I
                    // — but those are isAdd⟹B(43,1) anyway). The
                    // structurally-correct fix = per-form negA
                    // (R:B(31,1), C/I:0 for !isAdd) but touches
                    // pre-form-branch ⟹ deferred until a corpus
                    // hit surfaces. ×142 confirmed: NOT (A)'s root.
                    var negCb = isAdd ? B(56,1) : B(43,1);
                    (bl, bh) = HUnpack(Cbuf(), 1, negCb, B(54,1));
                } else {
                    // I-form: imm halves. BimmH0=bits 20:10 (10b),
                    // BimmH1=(bit56<<9)|bits 30:9 (split). Each <<6
                    // for f16 (= imm encodes top-10 bits of 16-bit
                    // half — sign+exp5+mant4; bottom-6 mant = 0).
                    // InstHadd2I:2333-2334.
                    (bl, bh) = HImm2();
                }
                var op = isAdd ? BinOp.Add : BinOp.Mul;
                var rl = new IlBin(F32, op, al, bl);
                var rh = new IlBin(F32, op, ah, bh);
                body.Add(SetGpr(rd, HPack((rl, rh), ofmt, Gpr(rd))));
                break;
            }
            case "HFMA2-R": case "HFMA2-C": case "HFMA2-I":
            case "HFMA2-RC": {
                // a*b + c. R-form: B@20 BSwizzle@28 NegA@31; C@39
                // CSwizzle@35 NegC@30. ‡ C/I/Rc forms have different
                // layouts (verify per InstHfma2*); v0 = R-form bits
                // for all + ‡note for non-R.
                // ASwizzle@47:2 across all forms. NegA differs (R@31,
                // C/RC@56, I@‡). v0=R's@31 for all (most are R).
                var (al, ah) = HUnpack(Gpr(ra), B(47,2), B(31,1), 0);
                Il bl, bh, cl, ch;
                switch(d.Name) {
                case "HFMA2-R":
                    // B=Gpr@20 swiz@28; C=Gpr@39 swiz@35 negC@30.
                    (bl, bh) = HUnpack(Gpr(B(20,8)), B(28,2), 0, 0);
                    (cl, ch) = HUnpack(Gpr(B(39,8)), B(35,2), B(30,1), 0);
                    break;
                case "HFMA2-C": {
                    // B = c[slot@34:5][off@20:14], halves dup'd
                    // (no swizzle field). C=Gpr@39 swiz@53:2 negC@52.
                    var cb = new IlCbufLoad(F32, (int)B(34,5), C(B(20,14)*4));
                    (bl, bh) = (cb, cb);
                    (cl, ch) = HUnpack(Gpr(B(39,8)), B(53,2), B(52,1), 0);
                    break;
                }
                case "HFMA2-RC": {
                    // B = Gpr@39 (= "Rc"-form swaps roles). C = cbuf.
                    (bl, bh) = HUnpack(Gpr(B(39,8)), B(53,2), 0, 0);
                    var cb = new IlCbufLoad(F32, (int)B(34,5), C(B(20,14)*4));
                    (cl, ch) = (Neg(cb, B(52,1)), Neg(cb, B(52,1)));
                    break;
                }
                case "HFMA2-I":
                default:
                    // B = packed half2-imm (BimmH0@20:10, BimmH1=
                    // (bit56<<9)|bits 30:9, each <<6). C = Gpr@39
                    // CSwizzle@53:2 NegC@51. ASwizzle@47:2 same as
                    // R/C-forms (handled above). InstHfma2I:2406-14.
                    // No NegA in I-form (= bit 31 used by BimmH1).
                    (bl, bh) = HImm2();
                    (cl, ch) = HUnpack(Gpr(B(39,8)), B(53,2), B(51,1), 0);
                    break;
                }
                var rl = new IlBin(F32, BinOp.Add,
                    new IlBin(F32, BinOp.Mul, al, bl), cl);
                var rh = new IlBin(F32, BinOp.Add,
                    new IlBin(F32, BinOp.Mul, ah, bh), ch);
                body.Add(SetGpr(rd, HPack((rl, rh), B(49,2), Gpr(rd))));
                break;
            }
            case "HSET2-R": case "HSET2-I":
            case "HSETP2-R": case "HSETP2-I": {
                // Half-compare → packed bool-pair (HSET2) or pred
                // pair (HSETP2). InstHset2R/I:2543/2566 + InstHsetp2
                // R/I:2609/2633. Both lanes computed; HAnd folds;
                // Bop-with-SrcPred; HSET2 writes packed (hi<<16|lo)
                // where each lane = Bval ? 0xFFFF : 0x3C00 if true
                // else 0; HSETP2 writes DestPred(lo)+DestPredInv(hi).
                var isI = d.Name.EndsWith("-I");
                var isP = d.Name.StartsWith("HSETP2");
                // FComp position: R=@35:4, I=@49:4. NegA@43,AbsA@44
                // (both forms). R: NegB@31,AbsB@30,BSwizzle@28:2.
                // I: B = HImm2().
                var (al, ah) = HUnpack(Gpr(ra), B(47,2), B(43,1), B(44,1));
                Il bl, bh;
                if(isI) (bl, bh) = HImm2();
                else (bl, bh) = HUnpack(Gpr(B(20,8)), B(28,2), B(31,1), B(30,1));
                var fcomp = (int) B(isI ? 49 : 35, 4);
                var cop = (fcomp & 7) switch {
                    1 => BinOp.Slt, 2 => BinOp.Eq, 3 => BinOp.Sle,
                    4 => BinOp.Sgt, 5 => BinOp.Ne, 6 => BinOp.Sge,
                    7 => BinOp.Eq,  // T (= Num if !nan-bit) — handled below
                    _ => BinOp.Eq };
                Il Cmp(Il a, Il b) {
                    // FComp: low-3 = relation (F/Lt/Eq/Le/Gt/Ne/Ge/T);
                    // bit-3 = Nan (= unordered → result OR isnan(a|b)).
                    // 0=F=always-false; 7=Num/T per nan-bit. v1 emits
                    // the relation; nan-bit ‡-noted (no IsNaN intrin
                    // yet — would need IlIntrin "Nan" → SpirvEmit
                    // OpIsNan).
                    if((fcomp & 7) == 0) return new IlConst(IlType.U1, 0);  // F
                    if((fcomp & 7) == 7) return new IlConst(IlType.U1, 1);  // T/Num ‡nan-bit
                    return new IlBin(IlType.U1, cop, a, b);
                }
                if((fcomp & 8) != 0)
                    body.Add(new IlNote($"‡ {d.Name} fcomp.Nan-bit ignored"));
                Il p0 = Cmp(al, bl), p1 = Cmp(ah, bh);
                // HAnd: HSETP2 R@49, I@53. HSET2 doesn't have HAnd.
                if(isP) {
                    var hAnd = B(isI ? 53 : 49, 1);
                    if(hAnd == 1) {
                        p0 = new IlBin(IlType.U1, BinOp.And, p0, p1);
                        p1 = new IlUn(IlType.U1, UnOp.Not, p0);
                    }
                }
                // Bop-with-SrcPred (same shape as ISETP/FSET): @45:2,
                // SrcPred@39:3 SrcPredInv@42.
                var sp = SrcPred(B(39,3), B(42,1));
                BinOp bo = (B(45, 2)) switch {
                    0 => BinOp.And, 1 => BinOp.Or, 2 => BinOp.Xor,
                    _ => BinOp.And };
                // Bop=And + SrcPred=PT (= 7,inv=0) → identity; skip
                // the redundant And to keep the IL readable.
                if(!(B(45,2) == 0 && B(39,3) == 7 && B(42,1) == 0)) {
                    p0 = new IlBin(IlType.U1, bo, p0, sp);
                    p1 = new IlBin(IlType.U1, bo, p1, sp);
                }
                if(isP) {
                    var dp = B(3,3); var dpi = B(0,3);
                    if(dp != 7) body.Add(new IlWriteReg(
                        RegKind.Pred, (int)dp, p0));
                    if(dpi != 7) body.Add(new IlWriteReg(
                        RegKind.Pred, (int)dpi, p1));
                } else {
                    // HSET2: Bval (R@49 I@53): true→0xFFFF (= -1
                    // half) else 0x3C00 (= 1.0 half). False → 0.
                    var bval = B(isI ? 53 : 49, 1);
                    var trueV = bval == 1 ? 0xFFFFu : 0x3C00u;
                    Il Lane(Il p) => new IlIfV(IlType.U32, p,
                        C((ulong)trueV), C(0ul));
                    // Pack: (hi<<16) | lo. Bitcast to F32 for the
                    // typeless GPR write.
                    var packed = new IlBin(IlType.U32, BinOp.Or,
                        new IlBin(IlType.U32, BinOp.Shl, Lane(p1), C(16ul)),
                        Lane(p0));
                    body.Add(SetGpr(rd, new IlCast(F32,
                        CastKind.Bitcast, packed)));
                }
                break;
            }

            // ── TEX: full-form texture sample. Differs from TEXS:
            //    - TidB@36:13 direct (same as TEXS — NOT cbuf-indirect
            //      as I'd guessed; that's the TexB bindless variant).
            //    - Dim@28:3 = TexDim enum (1d/Array1d/2d/Array2d/3d/
            //      Array3d/Cube/ArrayCube — direct, no lookup table).
            //    - WMask@31:4 = direct RGBA bitmask (not TEXS's
            //      wmask×dest2-RZ table).
            //    - One dest reg (Rd, Rd+1, …); no dest2 split.
            //    - Lod@55:3 (0=implicit 1=Lz 2=Lb 3=Ll 6=Lba 7=Lla).
            //    - Dc@50 = depth-compare. Aoffi@54 = packed offsets.
            //    Coord layout: [arrayIdx from Ra++ if Array] ++
            //    N spatial from Ra++ ++ [lod from Rb++ if >Lz] ++
            //    [offsets from Rb++ if Aoffi] ++ [dref from Rb++ if Dc].
            //    Per InstEmitTexture:159+ understood.
            case "TEX": case "TEX-B": {
                if(d.Name == "TEX-B") {
                    body.Add(new IlUnimpl("TEX-B", "bindless ‡v0"));
                    break;
                }
                var dim = (int) B(28, 3);
                var lodm = (int) B(55, 3);
                var dc = B(50, 1) == 1;
                var aoffi = B(54, 1) == 1;
                var wmask = (int) B(31, 4);
                var tidB = B(36, 13);
                // Map TexDim → SampKind dim + Array flag.
                var (skDim, arr, nSpat) = dim switch {
                    0 => (SampKind.D1, false, 1),
                    1 => (SampKind.D1, true,  1),
                    2 => (SampKind.D2, false, 2),
                    3 => (SampKind.D2, true,  2),
                    4 => (SampKind.D3, false, 3),
                    5 => (SampKind.D3, true,  3),
                    6 => (SampKind.Cube, false, 3),
                    7 => (SampKind.Cube, true,  3),
                    _ => (SampKind.D2, false, 2) };
                var sk = skDim
                    | (arr ? SampKind.Array : 0)
                    | (dc ? SampKind.Depth : 0)
                    | (lodm == 1 ? SampKind.LodZero : 0)
                    | (lodm is 3 or 7 ? SampKind.HasLod : 0);
                // ‡ Lb/Lba (lod-bias) → no SampKind flag yet; treat
                // as implicit + ‡note. Aoffi → ‡note (offsets need
                // OpImageSample…WithOffset variant).
                if(lodm is 2 or 6) body.Add(new IlNote("‡ TEX Lb/Lba bias v0=implicit"));
                if(aoffi) body.Add(new IlNote("‡ TEX Aoffi (packed offsets) v0=ignored"));
                // Build coords. Ra++/Rb++ iterators.
                int rai = (int)ra, rbi = (int)B(20,8);
                Il Ra() => Gpr((ulong)(rai > 255 ? 255 : rai++));
                Il Rb() => Gpr((ulong)(rbi > 255 ? 255 : rbi++));
                var co = new List<Il>();
                Il arrIdx = arr ? Ra() : null;
                for(var k = 0; k < nSpat; k++) co.Add(Ra());
                if(arr) co.Add(arrIdx);
                var lodV = (lodm > 1) ? Rb() : Cf(0);  // consume Rb
                if(aoffi) Rb();   // consume + skip (‡v0)
                if(dc) co.Add(Rb());
                if(lodm is 1 or 3 or 7) co.Add(lodV);  // LodLevel/Lz
                // (Lb/Lba would be a separate "bias" arg; ‡v0 dropped.)
                var samp = new IlSample(new IlType.Vec(128),
                    C(tidB), co, sk, wmask);
                var tid = (int)(pc / 8);
                var t = new IlTmp(new IlType.Vec(128), tid);
                body.Add(new IlLet(tid, samp));
                int wn = 0;
                for(var bit = 0; bit < 4; bit++) {
                    if((wmask & (1 << bit)) == 0) continue;
                    body.Add(SetGpr(rd + (ulong)wn,
                        new IlVecElem(F32, t, C((ulong)bit))));
                    wn++;
                }
                break;
            }

            case "IMNMX-R": case "IMNMX-C": case "IMNMX-I": {
                // Int min/max by SrcPred (= FMNMX shape on u32).
                // Signed@48. ‡ v0: emit IlIfV w/ a < b ? a : b for
                // min (and swapped for max) — no IL UMin/UMax yet.
                var sgnd = B(48, 1) == 1;
                var a = Bu32(Gpr(ra));
                var b = d.Name switch {
                    "IMNMX-R" => Bu32(Gpr(B(20,8))),
                    "IMNMX-C" => Bu32(Cbuf()),
                    "IMNMX-I" => C(Iimm20()), _ => throw new() };
                var lt = new IlBin(IlType.U1,
                    sgnd ? BinOp.Slt : BinOp.Ult, a, b);
                var sp = SrcPred(B(39,3), B(42,1));
                // sp ? min : max = sp ? (lt?a:b) : (lt?b:a)
                var min = new IlIfV(IlType.U32, lt, a, b);
                var max = new IlIfV(IlType.U32, lt, b, a);
                body.Add(SetGpr(rd, Bf32(
                    new IlIfV(IlType.U32, sp, min, max))));
                break;
            }

            // ── SSY/SYNC: stack-based control-flow (diverge/
            //    converge). For STRAIGHT-LINE-with-predication
            //    code (= every shader so far) these are no-ops at
            //    IL level — the SSY pushes a converge-point, SYNC
            //    pops to it; with no actual BRA between them, the
            //    fall-through is identical. ‡ v0 = ignore. v1 needs
            //    proper structurization (find SSY-target → wrap
            //    [SSY..target] in a structured block). ──
            case "SSY": case "PBK": {
                // Push a reconverge target. Imm24@20:24 pc-rel
                // (same encoding as BRA). v1: simple stack — the
                // corpus's 30 SSY-shaders are 29/30 non-nested
                // (sh0351 has 5). PBK = same shape (BRK pops it).
                // The SSY itself doesn't branch; the target IS a
                // block-leader though (= Structurize needs it in
                // the leader-set so the merge-block exists).
                var tgt = (ulong)((long)pc + 8 + (long)
                    ((B(20,24) ^ 0x800000) - 0x800000));
                _ssyStack.Push((d.Name, tgt));
                // Emit a no-op marker that Structurize's leader-
                // collection can recognize. v1: IlNote with the
                // target encoded; Structurize parses "@ssy:HEX".
                body.Add(new IlNote($"@ssy:{tgt:x}"));
                break;
            }
            case "SYNC": case "BRK": {
                // Pop + branch to top-of-stack. SYNC matches SSY;
                // BRK matches PBK (= they're independent stacks
                // semantically but share encoding). v1: single
                // stack, pop on uncond (= pred=7), Peek on
                // predicated (= the not-taken arm stays in scope;
                // the later uncond SYNC pops). ‡ Heuristic per
                // sh1012's shape; proper = path-simulation
                // (PropagatePushOp:752 in ref).
                if(_ssyStack.Count == 0) {
                    body.Add(new IlNote(
                        $"‡ {d.Name} empty-stack @{pc:x}"));
                    break;
                }
                // v1.5: SYNC/BRK PEEK only — pop happens at the
                // target-pc in LiftShader's loop (= reconverge
                // closes the scope, not the SYNC). Multiple SYNCs
                // on divergent paths all read the same top.
                var (k, tgt) = _ssyStack.Peek();
                // Emit as IlBranch (= same as BRA). The case-
                // dispatch's outer pred-handling wraps this in
                // IlIf(pred,…) already (per :206-220), so emit
                // an UNCONDITIONAL branch here; the predication
                // is the wrapper's job.
                body.Add(new IlBranch(BranchKind.Jmp,
                    new IlConst(IlType.U64, tgt)));
                break;
            }

            // ── LDC: indexed cbuf load. c[slot][Ra + imm], where Ra
            // is a BYTE-offset register (or RZ for static). LsSize@
            // 48:3 (0=u8 1=s8 2=u16 3=s16 4=B32 5=B64 6=B128). v0:
            // B32 only (= the dominant case; B64/128 = vec2/vec4
            // load → 2/4 IlCbufLoads). AddressMode@44:2 ‡ ignored
            // v0 (default = direct; IL/IS/ISL = bank-select). ──
            // ⚠️ slot@36:5 here (NOT 34 like arith C-forms).
            case "LDC": {
                var lslot = (int) B(36, 5);
                var loff  = (int)(short) B(20, 16);  // signed 16-bit byte off
                var lsz   = B(48, 3);
                // Index expr: if Ra==RZ, static offset; else Ra+imm
                // (both byte-addressed; SpirvEmit divides by 4 for
                // the float[1024] index).
                Il idx = ra == 255 ? C((ulong)(uint) loff)
                    : new IlBin(IlType.U32, BinOp.Add,
                        new IlCast(IlType.U32, CastKind.Bitcast, Gpr(ra)),
                        C((ulong)(uint) loff));
                int n = lsz switch { 5 => 2, 6 => 4, _ => 1 };
                if(lsz < 4) {
                    body.Add(new IlUnimpl(d.Name, $"sub-32-bit lsz={lsz}"));
                    break;
                }
                for(var k = 0; k < n; k++) {
                    Il koff = k == 0 ? idx
                        : new IlBin(IlType.U32, BinOp.Add, idx, C((ulong)(k*4)));
                    body.Add(SetGpr(rd + (ulong)k,
                        new IlCbufLoad(F32, lslot, koff)));
                }
                break;
            }

            // ── FSETP: float compare → predicate. The most common
            // ‡unimpl in the menu FS. Semantics (per understanding from
            // InstEmitFloatComparison.cs): Pdest ← cmp(±|Ra|, ±|B|) bop
            // SrcPred; PdestInv ← (!cmp) bop SrcPred. fcomp 4-bit:
            // 1=LT 2=EQ 3=LE 4=GT 5=NE 6=GE 7=NUM 8=NAN 9-15=ordered
            // variants. bop 2-bit: 0=AND 1=OR 2=XOR. ──
            case "FSETP-R": case "FSETP-C": case "FSETP-I": {
                // Field positions per InstFsetpC (verified at-source).
                // B() now top-level.
                var fdpred = B(3, 3); var fdpinv = B(0, 3);
                var fnega = B(43, 1); var fabsa = B(7, 1);
                var fnegb = B(6, 1); var fabsb = B(44, 1);
                var fcomp = (int) B(48, 4);
                var fspred = B(39, 3); var fspinv = B(42, 1);
                var fbop = B(45, 2);
                var a = Fsrc(Gpr(ra), fnega, fabsa);
                var b = d.Name switch {
                    "FSETP-R" => Gpr(B(20, 8)), "FSETP-C" => Cbuf(),
                    "FSETP-I" => Fimm20(), _ => throw new() };
                b = Fsrc(b, fnegb, fabsb);
                // Map fcomp → IL BinOp comparison. ‡ v0: ordered/NaN
                // cases (8+) collapse to their base for now. Float
                // comparison uses signed-flavor ops (Slt etc.) by
                // convention; SpirvEmit emits OpFOrd* regardless.
                var cop = (fcomp & 7) switch {
                    1 => BinOp.Slt, 2 => BinOp.Eq, 3 => BinOp.Sle,
                    4 => BinOp.Sgt, 5 => BinOp.Ne, 6 => BinOp.Sge,
                    _ => BinOp.Eq,   // 0=F (always-false), 7=T — ‡ v0
                };
                Il cmp = new IlBin(IlType.U1, cop, a, b);
                // bop with SrcPred (spred=PT && !inv means just `cmp`).
                if(!(fspred == 7 && fspinv == 0)) {
                    var srcP = fspinv == 1
                        ? (Il) new IlUn(IlType.U1, UnOp.Not, Pred(fspred))
                        : Pred(fspred);
                    var bo = fbop switch {
                        0 => BinOp.And, 1 => BinOp.Or, 2 => BinOp.Xor,
                        _ => BinOp.And };
                    cmp = new IlBin(IlType.U1, bo, cmp, srcP);
                }
                if(fdpred != 7) body.Add(
                    new IlWriteReg(RegKind.Pred, (int)fdpred, cmp));
                if(fdpinv != 7) body.Add(
                    new IlWriteReg(RegKind.Pred, (int)fdpinv,
                        new IlUn(IlType.U1, UnOp.Not, cmp)));
                break;
            }

            case "KIL":
                // FS discard. Predicated (the wrapper handles cond);
                // here just emit IlDiscard. The when-pred wrapper at
                // the bottom turns it into IlIf(cond, [discard], []).
                body.Add(new IlDiscard());
                break;

            case "TEXS": case "TEXS-F16": case "TEXSF16": {
                // Simplified 2D sample. Hand-written .isa TEXS def has
                // wrong field positions (rd1@41 — should be Dest2@28,
                // SrcB@20, TidB@36 per InstTexs). Per kt[6]: read fields
                // RAW from the word for v0 instead of trusting the
                // hand-written .isa (gen-skeleton'd TEXSF16 has correct
                // positions; the hand-written TEXS def needs re-deriving
                // via gen-skeleton.py — ‡-board'd).
                var dest = B(0, 8); var srcA = B(8, 8); var srcB = B(20, 8);
                var dest2 = B(28, 8); var tidB = B(36, 13);
                var wmask = B(50, 3); var target = B(53, 4);
                // Component-mask encoding (Maxwell hardware fact, learned
                // by reading ryujinx for understanding): WMask 3-bit ×
                // dest2==RZ flag → 4-bit RGBA mask. dest2==RZ → 1-2 comps
                // to dest pair only; dest2≠RZ → 3-4 comps split across
                // both pairs (first two → dest,dest+1; rest → dest2,…).
                // Table values (HW encoding, not copyrightable code):
                var compMask = (dest2 == 255) switch {
                    true  => new[]{0b0001,0b0010,0b0100,0b1000,
                                   0b0011,0b1001,0b1010,0b1100}[wmask],
                    false => new[]{0b0111,0b1011,0b1101,0b1110,
                                   0b1111,0,0,0}[wmask],
                };
                // Build coord list per target. TexsTarget enum (per
                // InstDecoders): 0=1DLodZero 1=2D 2=2DLodZero
                // 3=2DLodLevel 4=2DDepthCmp 5=2DLodLevelDepthCmp
                // 6=2DLodZeroDepthCmp 7=2DArray 8=2DArrayLodZero
                // 9=2DArrayLodZeroDepthCmp 10=3D 11=3DLodZero
                // 12=Cube 13=CubeLodLevel.
                // Source pattern (per InstEmitTexture, understanding):
                //   1,2 → (Ra, Rb)              [Ra=u, Rb=v]
                //   3,4,10,12 → (Ra, Ra+1, Rb)  [Ra,Ra+1=uv/uvw[0:2], Rb=lod/dref/w]
                //   6,11 → (Ra, Ra+1, Rb, 0.0)  [+lod=0]
                //   5,13 → (Ra, Ra+1, Rb, Rb+1) [+lod or dref+lod]
                //   7,8,9 → array forms (Ra,Ra+1=uv; layer=Rb? ‡ unused by game)
                //   0 → 1D (game doesn't use)
                // SampKind: dim + Array/Depth/HasLod/LodZero flags.
                Il Ra(int k) => Gpr(srcA + (ulong)k);
                Il Rb(int k) => Gpr(srcB + (ulong)k);
                List<Il> co; int sk;
                switch(target) {
                    case 1:  co=new(){Ra(0),Rb(0)}; sk=SampKind.D2; break;
                    case 2:  co=new(){Ra(0),Rb(0)}; sk=SampKind.D2|SampKind.LodZero; break;
                    case 3:  // 2D + lod
                        co=new(){Ra(0),Ra(1),Rb(0)};
                        sk=SampKind.D2|SampKind.HasLod; break;
                    case 4:  // 2D depth-compare (the dominant: shadow
                             // sample). 3rd src = depth-ref, NOT lod.
                        co=new(){Ra(0),Ra(1),Rb(0)};
                        sk=SampKind.D2|SampKind.Depth; break;
                    case 5:  // 2D depth-compare + lod
                        co=new(){Ra(0),Ra(1),Rb(0),Rb(1)};
                        sk=SampKind.D2|SampKind.Depth|SampKind.HasLod; break;
                    case 6:  // 2D depth-compare + lod=0
                        co=new(){Ra(0),Ra(1),Rb(0)};
                        sk=SampKind.D2|SampKind.Depth|SampKind.LodZero; break;
                    case 10: co=new(){Ra(0),Ra(1),Rb(0)};
                        sk=SampKind.D3; break;
                    case 11: co=new(){Ra(0),Ra(1),Rb(0)};
                        sk=SampKind.D3|SampKind.LodZero; break;
                    case 12: co=new(){Ra(0),Ra(1),Rb(0)};
                        sk=SampKind.Cube; break;
                    case 13: co=new(){Ra(0),Ra(1),Rb(0),Rb(1)};
                        sk=SampKind.Cube|SampKind.HasLod; break;
                    default:
                        body.Add(new IlUnimpl("TEXS",
                            $"target={target} (array/1D) ‡v0"));
                        goto texs_done;
                }
                var samp = new IlSample(new IlType.Vec(128),
                    C(tidB), co, sk, compMask);
                // Let-bind the vec4 result so each component-write reads
                // one sample, not 4. Then route enabled components to
                // dest/dest+1/dest2/dest2+1 in mask-bit order.
                var tid = (int)(pc / 8);
                var t = new IlTmp(new IlType.Vec(128), tid);
                body.Add(new IlLet(tid, samp));
                // (T6)×7(a) TEXSF16 packs half2-per-GPR. Found via
                // §7 ryujinx-oracle (sera kt[12]×4, day-32): sh0814
                // body = Hfma2-mix(TexsF16(tcb_1E), TexsF16(tcb_20));
                // tracing our SPIR-V showed %48 = f32-component →
                // bitcast → UnpackHalf2x16 = treating f32-mantissa-
                // bits as packed-f16 ≈ 0/garbage. r21 (sh0814+
                // NOBLEND) = 36.5% pure (0,255,0) = the bit-mis-
                // interpret signature.
                //   TEXS:    dest=c0, dest+1=c1, dest2=c2, dest2+1=c3
                //            (one f32-GPR per enabled component)
                //   TEXSF16: dest=pack16(c0,c1), dest2=pack16(c2,c3)
                //            (one u32-as-f32-GPR per enabled PAIR;
                //             downstream HFMA2-R does HUnpack(sw=0)
                //             = bitcast → UnpackHalf2x16)
                // Odd component count: hi-half = 0.0 (‡ HW behavior
                // unverified; ryujinx Hfma2 chain only reads the
                // packed lanes that matter, so zero-hi is safe).
                var isF16 = d.Name != "TEXS";
                var comps = new List<Il>();
                for(var bit = 0; bit < 4; bit++)
                    if((compMask & (1 << bit)) != 0)
                        comps.Add(new IlVecElem(F32, t, C((ulong)bit)));
                if(isF16) {
                    for(var p = 0; p < comps.Count; p += 2) {
                        var lo = comps[p];
                        var hi = p + 1 < comps.Count
                               ? comps[p+1]
                               : new IlConst(F32, 0ul);
                        var dst = p == 0 ? dest : dest2;
                        body.Add(SetGpr(dst,
                            new IlCast(F32, CastKind.Bitcast,
                                new IlIntrin(IlType.U32,
                                    "PackHalf2x16", lo, hi))));
                    }
                } else {
                    int written = 0;
                    foreach(var c in comps) {
                        var dst = written < 2
                            ? dest + (ulong)written
                            : dest2 + (ulong)(written - 2);
                        body.Add(SetGpr(dst, c));
                        written++;
                    }
                }
            texs_done:
                break;
            }

            // Everything else → self-‡ per spec §4. The verify loop
            // tells me which mnemonics surface; each one gets a case
            // here as the M1 chapter fills out.
            default:
                body.Add(new IlUnimpl(d.Name, "M1 v0 hardcoded-lift"));
                break;
        }

        // (T6)×43 .SAT modifier: bit 50 on float-arith ops → clamp
        // every Gpr-write in this insn's body to [0,1]. 526 instances
        // missing across the LEGO corpus per sweep-diff vs ryujinx
        // (= the #1 semantic Δ). Reads F("sat") where the .isa def
        // captures it (FFMA-{R,I,Rc}, FADD-C, FMUL-I, …); falls back
        // to direct B(50,1) for the float-op forms whose .isa def
        // doesn't yet have (sat T) in (names …) — adding those is
        // the ‡ structural follow-up (one-char bitpattern edits per
        // form). Half-prec ops (HFMA2/HMUL2/HADD2) have sat at bit 32
        // (= their HPack call's sat arg) — NOT covered here. Imm32
        // forms (FADD32I/FMUL32I) have sat at bit 55 — also NOT here
        // (those don't have a names-sat capture either; ‡ if surface).
        // (T6)×44 (Δ-1') fix: FMNMX has NO .SAT (verified
        // ryujinx InstFmnmxR — no Sat field; bit50 unused/
        // other). Was over-firing: 18 shaders × +3 FClamp
        // each = the entire +53 aggregate over-count.
        var satBit = d.Fields.ContainsKey("sat") ? F("sat")
            : d.Name.Split('-')[0] is "FFMA" or "FMUL" or "FADD"
                or "F2F" or "F2I" or "I2F"
              ? B(50, 1) : 0;
        if(satBit == 1)
            for(var bi = 0; bi < body.Count; bi++)
                if(body[bi] is IlWriteReg(RegKind.Gpr, var ix, var v))
                    body[bi] = new IlWriteReg(RegKind.Gpr, ix,
                        new IlUn(F32, UnOp.Sat, v));

        // Predication wrapper. PT && !inv = unconditional; else IlIf.
        Il wrapped;
        if((pred == 7 || pred == ~0ul) && (pinv == 0 || pinv == ~0ul)) {
            wrapped = body.Count == 1 ? body[0] : new IlBlock(body);
        } else {
            var cond = pinv == 1
                ? new IlUn(IlType.U1, UnOp.Not, Pred(pred))
                : Pred(pred);
            wrapped = new IlIf(cond, body, new Il[0]);
        }
        return (d, wrapped);
    }

    // Lift a whole shader binary (data @ container, insns from 0x80,
    // sched-skip every 4th word). Returns the sequence of (pc, def, il).
    // pc here = FILE OFFSET (incl the 0x80 header). IlBranch targets
    // (from BRA case) are computed in the same space.
    public List<(ulong pc, MaxwellDef def, Il il)> LiftShader(byte[] data) {
        _ssyStack.Clear();  // per-shader (instance reused across corpus)
        var result = new List<(ulong, MaxwellDef, Il)>();
        for(int off = 0x80, i = 0; off + 8 <= data.Length; off += 8, i++) {
            if(i % 4 == 0) continue;     // sched word
            // ── _ssyStack v1.5: pop-at-target. SSY/PBK push (kind,
            //    target). SYNC/BRK PEEK (never pop). When linear-pc
            //    reaches the top's target, that SSY's reconverge
            //    scope is closed → pop here. Handles the sh0083
            //    shape (one SSY → divergent → 2 SYNCs both peek
            //    same top → pop ONCE at target). Nested SSY: inner
            //    target reached first (= linear-walk encounters
            //    inner-T before outer-T iff properly-nested) → pops
            //    correctly. ‡ Limit: out-of-order targets (= SSY-A
            //    @x→T1, SSY-B @y→T2, T2<T1 = improper nesting). v1.5
            //    pops in target-pc-order, which is correct for the
            //    properly-nested case the corpus has (per (π-b3)'s
            //    sh0351 5×SSY check). The proper fix = per-block CFG
            //    DFS simulating stack-state (= ref PropagatePushOp).
            while(_ssyStack.Count > 0 && _ssyStack.Peek().Item2 <= (ulong)off)
                _ssyStack.Pop();
            var w = BitConverter.ToUInt64(data, off);
            if(w == 0) break;
            var (def, il) = Lift(w, (ulong) off);
            result.Add(((ulong) off, def, il));
            // ‡ v0: stop at first unconditional EXIT. Multi-EXIT/
            // post-EXIT branches = M1.5 control flow.
            if(def?.Name == "EXIT") break;
        }
        return result;
    }

    // ── Structurize: post-process the flat per-insn IL list into
    //    structured control-flow. v0 handles ONE pattern: forward
    //    predicated branch, properly-nested-or-sequential.
    //
    //      @cond BRA tgt    →    if(!cond) {
    //      …body…                  …body…
    //      tgt: …                }
    //                            tgt: …
    //
    //    Recursive for nesting (if a BRA inside body targets within
    //    body, it becomes a nested if). On unhandled shapes (backward
    //    = loop; overlapping = irreducible; unpredicated forward =
    //    if/else split), v0 emits IlNote + leaves the IlBranch in
    //    place (= SpirvEmit will throw → these shaders still SKIP,
    //    but FEWER). pass-7 = the harder cases. ──
    public static List<Il> Structurize(
            List<(ulong pc, MaxwellDef def, Il il)> insns) {
        // Index pc → list-index for target lookup. ALSO register
        // sched-word positions (= every 4th 8-byte slot) → next
        // real insn's index, since BRA targets land ON sched-words
        // (compiler emits target = byte-offset, sched-pc included).
        var pcIdx = new Dictionary<ulong, int>();
        for(var k = 0; k < insns.Count; k++) {
            var p = insns[k].pc;
            pcIdx[p] = k;
            // Slot before this insn = sched? Register it → k.
            // (sched at file-off = 0x80 + 32*N, i.e. p - 8 if
            // (p - 0x80) % 32 == 8.)
            if(((p - 0x80) % 32) == 8) pcIdx[p - 8] = k;
        }
        // Targets can also land PAST the last insn (= after EXIT,
        // = the final sched-word). Snap those to insns.Count.
        int Snap(ulong tgt) {
            if(pcIdx.TryGetValue(tgt, out var idx)) return idx;
            // Past-end = converge at exit. ‡ Or genuinely OOB.
            return tgt > insns[^1].pc ? insns.Count : -1;
        }

        // Recursive descent over [lo,hi). Returns the structured
        // body for that range; on overlap/backward, falls back to
        // emitting the raw IL with an IlNote (= will throw later).
        List<Il> Walk(int lo, int hi) {
            var result = new List<Il>();
            var k = lo;
            while(k < hi) {
                var (pc, def, il) = insns[k];
                // Pattern: IlIf(cond, [IlBranch(Jmp, IlConst tgt)], [])
                // → forward predicated branch.
                if(il is IlIf(var cond, var th, var el)
                        && th.Count == 1 && el.Count == 0
                        && th[0] is IlBranch(BranchKind.Jmp,
                            IlConst(_, var tgtV), _)) {
                    var tgt = (ulong)tgtV;
                    if(tgt <= pc) {
                        // Backward = loop. Pattern: `tgt: …body…;
                        // @cond BRA tgt` → IlLoop(cond, [body]).
                        // body = everything ALREADY EMITTED into
                        // result since tgt's index. Find where in
                        // result tgt landed (= the first stmt at
                        // pc≥tgt; but result doesn't carry pc…).
                        // ⟹ Need to know HOW MANY result-entries
                        // back the loop-header is. = walk insns
                        // [tgtIdx, k) and count what they emitted.
                        // ⚠️ With nested ifs, the count ≠ k-tgtIdx.
                        // v0: assume the loop-header is at the
                        // CURRENT result.Count - (k - tgtIdx) — i.e.
                        // each insn in [tgtIdx,k) emitted exactly 1
                        // result entry. ‡ Wrong if any of them was
                        // a forward-BRA that consumed multiple insns
                        // into one IlIf.
                        // Cleaner: re-Walk [tgtIdx, k) to get the
                        // body fresh, replace result's tail.
                        var hdrIdx = Snap(tgt);
                        if(hdrIdx < lo || hdrIdx > k) {
                            result.Add(new IlNote(
                                $"‡ Structurize: backward BRA @{pc:x}→{tgt:x} hdr-OOB v0=throw"));
                            result.Add(il); k++; continue;
                        }
                        // Re-walk [hdrIdx, k) to get body. result
                        // already has those stmts emitted (from when
                        // we walked them linearly above) — we need
                        // to REWIND result and re-emit as IlLoop.
                        // Track where result was at hdrIdx. Since
                        // we don't have that, simplest = re-walk
                        // and TRUNCATE result by body.Count.
                        var loopBody = Walk(hdrIdx, k);
                        // result currently has loopBody's stmts at
                        // its tail (emitted before we hit this BRA).
                        // Truncate them.
                        if(result.Count >= loopBody.Count) {
                            result.RemoveRange(
                                result.Count - loopBody.Count,
                                loopBody.Count);
                        }
                        // ⚠️ This double-walks [hdrIdx,k) — O(n²)
                        // for nested loops. sh0158 has 1 loop, fine.
                        // Cond: @cond BRA tgt continues IF cond.
                        // do{body}while(cond). IlLoop semantics =
                        // "execute body; if cond, repeat."
                        result.Add(new IlLoop(cond, loopBody));
                        k++;
                        continue;
                    }
                    var tgtIdx = Snap(tgt);
                    if(tgtIdx < 0) {
                        result.Add(new IlNote(
                            $"‡ Structurize: BRA @{pc:x}→{tgt:x} no-snap v0=throw"));
                        result.Add(il); k++; continue;
                    }
                    if(tgtIdx > hi) {
                        // Inner BRA targets PAST the enclosing
                        // range = "diamond" (inner if converges
                        // at/past outer's merge). Under OpSelect-
                        // flatten, both arms execute regardless;
                        // the predicates compose (effC = outer &&
                        // !inner). Clamping tgtIdx=hi means the
                        // inner if's body = [k+1,hi) and the
                        // ‡-skipped tail [hi,tgtIdx) belongs to
                        // the OUTER's else (which runs under
                        // outer-cond, NOT inner-cond — but flatten
                        // makes that moot). ‡ Semantically: under
                        // flatten this is correct; under a real
                        // branching backend it'd need the diamond
                        // properly. v0=clamp+note.
                        result.Add(new IlNote(
                            $"‡ Structurize: BRA @{pc:x}→{tgt:x} idx={tgtIdx}>{hi} (diamond, clamp) v0"));
                        tgtIdx = hi;
                    }
                    // Forward, in-range. body = [k+1, tgtIdx),
                    // recursively structurized.
                    //
                    // if/else recognition: if the body's LAST stmt
                    // is an unpredicated BRA → tgtIdx2 (= the if-arm
                    // jumping over the else), then:
                    //   then-arm = [k+1, tgtIdx-1)  (drop the BRA)
                    //   else-arm = [tgtIdx, tgtIdx2)
                    //   k        = tgtIdx2
                    // and IlIf(!cond, then, else). Doesn't fit the
                    // current SpirvEmit (els.Count==0 only) so for
                    // v0 emit BOTH arms predicated separately:
                    //   if(!cond){then}; if(cond){else}
                    // = OpSelect-flatten works on each independently.
                    // ‡ Side-effects in else (texture-deriv) ‡-noted.
                    var bodyEnd = tgtIdx;
                    var elseEnd = -1;
                    if(bodyEnd > k + 1
                            && insns[bodyEnd - 1].il is IlBranch(
                                BranchKind.Jmp, IlConst(_, var t2v), _)) {
                        var t2 = Snap((ulong)t2v);
                        if(t2 >= bodyEnd && t2 <= hi) {
                            elseEnd = t2;
                            bodyEnd--;   // drop the BRA from then-arm
                        }
                    }
                    var inner = Walk(k + 1, bodyEnd);
                    var notCond = new IlUn(IlType.U1, UnOp.Not, cond);
                    result.Add(new IlIf(notCond, inner, new Il[0]));
                    if(elseEnd >= 0) {
                        var elseB = Walk(tgtIdx, elseEnd);
                        result.Add(new IlIf(cond, elseB, new Il[0]));
                        k = elseEnd;
                    } else {
                        k = tgtIdx;
                    }
                    continue;
                }
                // Unpredicated BRA at this level (= NOT the last-
                // stmt-of-an-if-arm, since that's recognized above)
                // = goto-style. v0: if forward + in-range, treat
                // as `if(true){body}` — semantically correct (the
                // body between here and tgt is dead code from the
                // BRA's perspective; but the unpredicated BRA at
                // this nesting means we ARRIVED here uncond, so
                // it just jumps → emit nothing for the body,
                // continue at tgt). = SKIP [k+1, tgtIdx).
                if(il is IlBranch(BranchKind.Jmp, IlConst(_, var ut), _)) {
                    var ti = Snap((ulong)ut);
                    if(ti == k + 1) {
                        // BRA-to-next-insn = nop-fallthrough (=
                        // SYNC at reconverge-point; the IlBranch
                        // it emits is semantically a nop here).
                        // Skip range [k+1,k+1) = empty. Silent.
                        k = ti; continue;
                    }
                    if(ti > k && ti <= hi) {
                        result.Add(new IlNote(
                            $"‡ Structurize: top-level uncond BRA @{pc:x}→{(ulong)ut:x} (skip [{k+1},{ti})) v0"));
                        // ‡ The skipped body MAY be reached via a
                        // backward BRA from later (= loop) — would
                        // need full CFG. v0 = skip + note.
                        k = ti;
                        continue;
                    }
                    result.Add(new IlNote(
                        $"‡ Structurize: uncond BRA @{pc:x}→{(ulong)ut:x} OOB v0=throw"));
                    result.Add(il);
                    k++;
                    continue;
                }
                result.Add(il);
                k++;
            }
            return result;
        }
        return Walk(0, insns.Count);
    }
}
