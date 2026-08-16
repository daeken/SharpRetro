using ArchCompilerCore;
using static Backends.Rust.RustEmit;

namespace Backends.Rust;

// Vector heads → Builder trait vector methods. The Builder trait's V128 ops are
// deliberately MINIMAL for rung-4a (velement_read/write + vzero_top + a few via
// call_intrinsic escape) — the full lane-wise vector-op set expands the trait once
// the emitted-set is known (per lib.rs's own comment: "don't over-declare").
// For now: vector heads that don't map to the minimal set → call_intrinsic (id 100+),
// which is honest (the arch runtime supplies them) and unblocks the census.

public static class VectorMathEmit {
    public static void Register() {
        // (vector e0 e1 ... eN) — construct a V128 from lanes. LINEARIZED: each velement_write
        // + its literal-index emit as own-line Rt() calls; the fold accumulates over temps,
        // no nesting. (The prior Aggregate form nested bd.velement_write(bd.literal(...)) —
        // 7767× E0499 double-borrow, the entire remaining error class.)
        Expression("vector", list => {
            var v = Rt("bd.literal(IlType::V128, 0)");
            var elems = list.Skip(1).ToList();
            for(var i = 0; i < elems.Count; i++) {
                var idx = Rt($"bd.literal(IlType::U32, {i})");
                var e = Lift(elems[i]);
                v = Rt($"bd.velement_write({v}, {idx}, {e})");
            }
            return v;
        });
        // (vector-all x) — broadcast scalar x to all lanes.
        Expression("vector-all", list =>
            $"bd.call_intrinsic(IntrinsicId(100/*vec_broadcast*/), &[{Lift(list[1])}]).unwrap()");
        // (vector-element v i ty) → velement_read
        Expression("vector-element", list =>
            $"bd.velement_read({Lift(list[1])}, {Lift(list[2])}, {TypeShort(list.Type)})");
        Expression("vector-zero-top", list => $"bd.vzero_top({Lift(list[1])})");
        // ALL operands, generically — the v1 hand-listed arms dropped vector-extract's
        // 4th arg (the INDEX) and vector-sum-unsigned's 3rd (the COUNT): a call-site
        // arity silently truncated at the emitter (the CALLDROP class). Join over
        // list.Skip(1) can't drift when the .isa's contract grows.
        Expression("vector-extract", list =>
            $"bd.call_intrinsic(IntrinsicId(101/*vec_extract*/), "
            + $"&[{string.Join(", ", list.Skip(1).Select(Lift))}]).unwrap()");
        Expression("vector-count-bits", list =>
            $"bd.call_intrinsic(IntrinsicId(102/*vec_popcnt*/), "
            + $"&[{string.Join(", ", list.Skip(1).Select(Lift))}]).unwrap()");
        Expression("vector-sum-unsigned", list =>
            $"bd.call_intrinsic(IntrinsicId(103/*vec_sum_u*/), "
            + $"&[{string.Join(", ", list.Skip(1).Select(Lift))}]).unwrap()");

        // Lane-wise arith/logic — the vec+/vec-/vec&/etc family. All → call_intrinsic
        // for rung-4a (the trait gains proper vector-op methods once the census settles).
        VecBinop("vec+", 110); VecBinop("vec-", 111);
        VecBinop("vec*", 112); VecBinop("vec/", 113);
        VecBinop("vec&", 114); VecBinop("vec|", 115);
        VecBinop("vec^", 116); VecBinop("vec&~", 117);
        VecBinop("vec>", 120); VecBinop("vec<", 121);
        VecBinop("vec>=", 122); VecBinop("vec<=", 123);
        VecBinop("vec==", 124); VecBinop("vec!=", 125);
        VecBinop("vec-uint+", 130); VecBinop("vec-uint*", 131);
        VecBinop("vec-uint>", 132); VecBinop("vec-uint<", 133);
        VecBinop("vec>>", 134); VecBinop("vec<<", 135);
        VecUnop("vec~", 140); VecUnop("vec-abs", 141);
        VecUnop("vec-frsqrte", 142); VecUnop("vec-neg", 143);

        // (for (var [start] end [step]) body...) — 2/3/4-elem tuple. C# backend UNROLLS at
        // emit-time (evaluates PInt bounds); for rung-4a emit a Rust range-loop (bounds are
        // ct-Rust-vars, rustc const-props/unrolls). ‡ Divergence: C# unroll means the loop
        // body sees a LITERAL `i` per iteration (so type-widths derived from `i` are ct too);
        // a Rust loop keeps `i` as a runtime var. Fine for gate-(a); IL-seq-diff at gate-(b)
        // will surface if any body actually needs the unroll.
        Statement("for", (c, list) => {
            var d = (PList) list[1];
            var name = SafeIdent(((PName) d[0]).Name);
            string start, end, step;
            switch(d.Count) {
                case 2: (start, end, step) = ("0", GenerateExpression(d[1]), "1"); break;
                case 3: (start, end, step) = (GenerateExpression(d[1]), GenerateExpression(d[2]), "1"); break;
                case 4: (start, end, step) = (GenerateExpression(d[1]), GenerateExpression(d[2]), GenerateExpression(d[3])); break;
                default: throw new NotSupportedException($"for-tuple {d.Count}");
            }
            c += $"let mut {name}: u32 = {start};";
            c += $"while ({name} as u64) < ({end} as u64) {{";
            c++;
            foreach(var e in list.Skip(2))
                if(e is PList pl) RustEmit.GenerateStatement(c, pl);
            c += $"{name} = {name}.wrapping_add({step});";
            c--;
            c += "}";
        });
    }

    static void VecBinop(string head, int intrinsicId) =>
        Expression(head, list =>
            $"bd.call_intrinsic(IntrinsicId({intrinsicId}), "
            + $"&[{Lift(list[1])}, {Lift(list[2])}]).unwrap()");
    static void VecUnop(string head, int intrinsicId) =>
        Expression(head, list =>
            $"bd.call_intrinsic(IntrinsicId({intrinsicId}), &[{Lift(list[1])}]).unwrap()");
}
