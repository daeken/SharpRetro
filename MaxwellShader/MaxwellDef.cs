namespace MaxwellGenerator;

using System.Diagnostics;
using CoreArchCompiler;

// Maxwell SM5x instruction definition. Direct shape-sibling of
// Aarch64Def (SharpRetro/Aarch64Generator/Aarch64Def.cs:6) adapted
// to 64-bit instruction words. Bitpattern is 64 chars MSB-first
// (matches ryujinx InstTable.cs convention so encodings can be
// transcribed directly). Per kt[14]: this is OUR decoder built from
// our doc; the bitpattern strings are encoding facts, not code.
//
// Maxwell-specific NOT handled here (= outer-loop / stub concerns):
//   - sched word every 4th u64 (decode loop skips position i%4==0)
//   - predication (pred@16-18 + inv@19 is on EVERY insn; could be a
//     wrapper macro or handled in the stub's per-insn dispatch)
//
// v0: NOT inheriting Def yet. Def's ctor runs InferType on dasm/decode/
// eval trees, which throws on every unknown builtin (gpr/cbuf/attr-in/
// etc.). M0 verify only needs Mask/Match/Fields. Once Builtins.cs is
// written (M1), switch to `: Def` and the base ctor type-checks the
// eval-exprs = the gate that catches semantic errors in the .isa.
public class MaxwellDef {
    public readonly string Name;
    public readonly ulong Mask, Match;
    public readonly IReadOnlyDictionary<string, (int Bits, int Shift)> Fields;
    public readonly PTree Dasm; public readonly PList Decode, Eval;

    MaxwellDef(string name, ulong mask, ulong match,
        Dictionary<string, (int Bits, int Shift)> fields,
        PTree dasm, PList decode, PList eval,
        IReadOnlyDictionary<string, EType> locals
    ) {
        Name = name; Mask = mask; Match = match; Fields = fields;
        Dasm = dasm; Decode = decode; Eval = eval;
    }

    public static MaxwellDef Parse(PList def) {
        if(def[0] is not PName("def")) throw new();
        if(def[1] is not PTree _name) throw new();
        if(def[2] is not PTree _bitstr) throw new();
        if(def[3] is not PTree disasm) throw new();
        if(def[4] is not PList names) throw new();
        if(def[5] is not PList decode) throw new();
        if(def[6] is not PList eval) throw new();

        var name = _name switch {
            PName(var x) => x,
            _ => (string) new ExecutionState().Evaluate(_name),
        };

        // (names (rd d) (ra a) ...) → letter → fieldname
        var fieldNames = names.Skip(1).Select(x => (PList) x)
            .Select(x => (((PName) x[1]).Name, ((PName) x[0]).Name)).ToDictionary();

        var fields = new Dictionary<string, (int, int)>();
        var mask = 0UL;
        var match = 0UL;
        var bitstring = ((string) new ExecutionState().Evaluate(_bitstr)).Replace(" ", "");
        if(bitstring.Length != 64)
            throw new($"{name}: bitpattern is {bitstring.Length} chars, must be 64 (Δ={64-bitstring.Length})");
        for(var i = 0; i < 64; ++i) {
            var bit = 63 - i;
            mask <<= 1;
            match <<= 1;
            switch(bitstring[i]) {
                case '0': mask |= 1; break;
                case '1': mask |= 1; match |= 1; break;
                case '#': break;       // don't-care
                case var x:
                    if(!fieldNames.TryGetValue(x.ToString(), out var field))
                        throw new($"{name}: bitpattern char '{x}' not in (names ...)");
                    fields[field] = fields.TryGetValue(field, out var f)
                        ? (f.Item1 + 1, bit) : (1, bit);
                    break;
            }
        }

        var locals = new Dictionary<string, EType>();
        foreach(var (fname, (bits, _)) in fields)
            locals[fname] = new EInt(false, bits).AsCompiletime();

        return new(name, mask, match, fields, disasm, decode, eval, locals);
    }
}
