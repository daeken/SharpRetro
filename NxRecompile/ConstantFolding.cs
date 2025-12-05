using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    static bool IsSigned(Type type) =>
        type switch {
            _ when type == typeof(sbyte) => true,
            _ when type == typeof(short) => true,
            _ when type == typeof(int) => true,
            _ when type == typeof(long) => true,
            _ when type == typeof(Int128) => true,
            _ => false
        };
    static object ToSigned(object obj) =>
        IsSigned(obj.GetType()) ? obj : obj switch {
            byte v => (object) unchecked((sbyte) v),
            ushort v => unchecked((short) v),
            uint v => unchecked((int) v),
            ulong v => unchecked((long) v),
            UInt128 v => unchecked((Int128) v),
            _ => throw new NotImplementedException($"Can't make signed value for {obj.GetType()}")
        };
    static object ToUnsigned(object obj) =>
        !IsSigned(obj.GetType()) ? obj : obj switch {
            sbyte v => (object) unchecked((byte) v),
            short v => unchecked((ushort) v),
            int v => unchecked((uint) v),
            long v => unchecked((ulong) v),
            Int128 v => unchecked((UInt128) v),
            _ => throw new NotImplementedException($"Can't make unsigned value for {obj.GetType()}")
        };
    static object Cast(object value, Type castTo) {
        if(IsSigned(value.GetType()) != IsSigned(castTo))
            value = IsSigned(castTo)
                ? ToSigned(value)
                : ToUnsigned(value);
        if(value.GetType() == castTo) return value;
        if(Marshal.SizeOf(castTo) > Marshal.SizeOf(value.GetType())) {
            if(castTo == typeof(UInt128))
                return new UInt128(0, Convert.ToUInt64(value));
            if(castTo == typeof(Int128)) {
                var big = unchecked((ulong) Convert.ToInt64(value));
                return new Int128((big >> 63) == 0 ? 0 : ulong.MaxValue, big);
            }
            return Convert.ChangeType(value, castTo);
        }
        var temp = value switch {
            UInt128 uv => (ulong) uv,
            Int128 iv => unchecked((ulong) (long) iv),
            _ => (ulong) Convert.ChangeType(value, typeof(ulong))
        };
        var mask = (1UL << (Marshal.SizeOf(castTo) * 8)) - 1;
        return Convert.ChangeType(temp & mask, castTo);
    }

    static (Type Type, object Value)? GetConstant(StaticIRValue value) =>
        value switch {
            StaticIRValue.Literal(var lit, var type) => (type, lit),
            StaticIRValue.Cast(var lval, var type) when GetConstant(lval) is var (_, oval) => (type, Cast(oval, type)),
            _ => null
        };

    static T DoBinaryOp<T>(StaticIRValue bin, T left, T right) where T : IBinaryNumber<T> =>
        unchecked(bin switch {
            StaticIRValue.Add => left + right,
            StaticIRValue.Sub => left - right,
            StaticIRValue.And => left & right,
            StaticIRValue.Or => left | right,
            StaticIRValue.Xor => left ^ right,
            StaticIRValue.LeftShift when typeof(T) == typeof(UInt128) =>
                (T) (object) ((UInt128) (object) left << (int) (UInt128) (object) right),
            StaticIRValue.RightShift when typeof(T) == typeof(UInt128) =>
                (T) (object) ((UInt128) (object) left >> (int) (UInt128) (object) right),
            StaticIRValue.LeftShift when Convert.ToInt32(right) >= 0 => left switch {
                byte v => (T) (object) (byte) (v << Convert.ToInt32(right)),
                ushort v => (T) (object) (ushort) (v << Convert.ToInt32(right)),
                uint v => (T) (object) (v << Convert.ToInt32(right)),
                ulong v => (T) (object) (v << Convert.ToInt32(right)),
                _ => throw new NotImplementedException($"Attempted left shift on {bin} -- {left} and {right}")
            },
            StaticIRValue.RightShift when Convert.ToInt32(right) >= 0 => left switch {
                byte v => (T) (object) (byte) (v >> Convert.ToInt32(right)),
                ushort v => (T) (object) (ushort) (v >> Convert.ToInt32(right)),
                uint v => (T) (object) (v >> Convert.ToInt32(right)),
                ulong v => (T) (object) (v >> Convert.ToInt32(right)),
                _ => throw new NotImplementedException($"Attempted right shift on {bin} -- {left} and {right}")
            },
            _ => throw new NotImplementedException($"Attempted {bin} on {left} and {right}"),
        });

    static bool DoCompare<T>(StaticIRValue op, T left, T right) where T : IBinaryNumber<T> =>
        op switch {
            StaticIRValue.EQ => left == right,
            StaticIRValue.NE => left != right,
            StaticIRValue.LT => left < right,
            StaticIRValue.LTE => left <= right,
            StaticIRValue.GT => left > right,
            StaticIRValue.GTE => left >= right,
            _ => throw new NotImplementedException($"Attempted {op} on {left} and {right}"),
        };

    static StaticIRValue FoldBinary(StaticIRValue bin, StaticIRValue left, StaticIRValue right) {
        if(GetConstant(left) is not var (leftType, leftValue) ||
           GetConstant(right) is not var (rightType, rightValue)) return null;

        if(leftType != rightType) return null;
        var nlit = leftType switch {
            { } x when x == typeof(byte) => DoBinaryOp(bin, (byte) leftValue,
                (byte) rightValue),
            { } x when x == typeof(ushort) => DoBinaryOp(bin, (ushort) leftValue,
                (ushort) rightValue),
            { } x when x == typeof(uint) => DoBinaryOp(bin, (uint) leftValue,
                (uint) rightValue),
            { } x when x == typeof(ulong) => DoBinaryOp(bin, (ulong) leftValue,
                (ulong) rightValue),
            { } x when x == typeof(UInt128) => DoBinaryOp(bin, (UInt128) leftValue,
                (UInt128) rightValue),
            _ => (object) null,
        };
        if(nlit != null) return new StaticIRValue.Literal(nlit, leftType);
        return null;
    }

    static StaticIRValue FoldComp(StaticIRValue op, StaticIRValue left, StaticIRValue right) {
        if(GetConstant(left) is not var (leftType, leftValue) ||
           GetConstant(right) is not var (rightType, rightValue)) return null;

        if(leftType != rightType) return null;
        var nlit = leftType switch {
            { } x when x == typeof(byte) => DoCompare(op, (byte) leftValue,
                (byte) rightValue),
            { } x when x == typeof(ushort) => DoCompare(op, (ushort) leftValue,
                (ushort) rightValue),
            { } x when x == typeof(uint) => DoCompare(op, (uint) leftValue,
                (uint) rightValue),
            { } x when x == typeof(ulong) => DoCompare(op, (ulong) leftValue,
                (ulong) rightValue),
            { } x when x == typeof(UInt128) => DoCompare(op, (UInt128) leftValue,
                (UInt128) rightValue),
            _ => (object) null,
        };
        if(nlit != null) return new StaticIRValue.Literal(nlit, typeof(bool));
        return null;
    }
    bool FoldConstants() {
        var foldedAny = false;
        foreach(var node in WholeBlockGraph.Values) {
            var block = node.Block;
            var body = block.Body;
            try {
                var folded = false;
                do {
                    folded = false;
                    var regs = new StaticIRValue[32];
                    StaticIRValue N = null;
                    StaticIRValue Z = null;
                    StaticIRValue C = null;
                    StaticIRValue V = null;
                    var vars = new Dictionary<string, StaticIRValue>();
                    var stack = new Stack<(
                        StaticIRValue[] Regs, StaticIRValue N, StaticIRValue Z, StaticIRValue C, StaticIRValue V, 
                        Dictionary<string, StaticIRValue> Vars
                    )>();
                    FoldList(body);

                    void Push() {
                        stack.Push((regs, N, Z, C, V, vars));
                        regs = regs.ToArray();
                        vars = vars.ToDictionary();
                    }
                    (
                        StaticIRValue[] Regs, StaticIRValue N, StaticIRValue Z, StaticIRValue C, StaticIRValue V, 
                        Dictionary<string, StaticIRValue> Vars
                    ) Pop() {
                        var ret = (regs, N, Z, C, V, vars);
                        (regs, N, Z, C, V, vars) = stack.Pop();
                        return ret;
                    }

                    void MergeScopes(params (
                        StaticIRValue[] Regs, StaticIRValue N, StaticIRValue Z, StaticIRValue C, StaticIRValue V,
                        Dictionary<string, StaticIRValue> Vars
                    )[] scopes) {
                        Debug.Assert(scopes.Length <= 2);
                        if(scopes.Length == 1) {
                            var (tregs, tn, tz, tc, tv, _) = scopes[0];
                            regs = regs.Zip(tregs).Select(x => x.First == x.Second ? x.First : null).ToArray();
                            if(tn != N) N = null;
                            if(tz != Z) Z = null;
                            if(tc != C) C = null;
                            if(tv != V) V = null;
                        } else {
                            var (aregs, an, az, ac, av, _) = scopes[0];
                            var (bregs, bn, bz, bc, bv, _) = scopes[1];
                            regs = aregs.Zip(bregs).Select(x => x.First == x.Second ? x.First : null).ToArray();
                            N = an == bn ? an : null;
                            Z = az == bz ? az : null;
                            C = ac == bc ? ac : null;
                            V = av == bv ? av : null;
                        }
                    }
                    void FoldList(List<StaticIRStatement> body) {
                        for(var i = 0; i < body.Count; ++i) {
                            var stmt = body[i];
                            switch(stmt) {
                                case StaticIRStatement.If(var cond, var left, var right): {
                                    cond = Fold(cond) ?? cond;
                                    Push();
                                    var stmts = left is StaticIRStatement.Body lbody ? lbody.Stmts : [left];
                                    FoldList(stmts);
                                    left = new StaticIRStatement.Body(stmts);
                                    var lscope = Pop();
                                    Push();
                                    stmts = right is StaticIRStatement.Body rbody ? rbody.Stmts : [right];
                                    FoldList(stmts);
                                    right = new StaticIRStatement.Body(stmts);
                                    var rscope = Pop();
                                    MergeScopes(lscope, rscope);
                                    body[i] = new StaticIRStatement.If(cond, left, right);
                                    break;
                                }
                                case StaticIRStatement.When(var cond, var then): {
                                    cond = Fold(cond) ?? cond;
                                    Push();
                                    var stmts = then is StaticIRStatement.Body tbody ? tbody.Stmts : [then];
                                    FoldList(stmts);
                                    then = new StaticIRStatement.Body(stmts);
                                    var scope = Pop();
                                    MergeScopes(scope);
                                    body[i] = new StaticIRStatement.When(cond, then);
                                    break;
                                }
                                case StaticIRStatement.Unless(var cond, var then): {
                                    cond = Fold(cond) ?? cond;
                                    Push();
                                    var stmts = then is StaticIRStatement.Body tbody ? tbody.Stmts : [then];
                                    FoldList(stmts);
                                    then = new StaticIRStatement.Body(stmts);
                                    var scope = Pop();
                                    MergeScopes(scope);
                                    body[i] = new StaticIRStatement.Unless(cond, then);
                                    break;
                                }
                                case StaticIRStatement.While(var cond, var then): {
                                    cond = Fold(cond) ?? cond;
                                    Push();
                                    var stmts = then is StaticIRStatement.Body tbody ? tbody.Stmts : [then];
                                    FoldList(stmts);
                                    then = new StaticIRStatement.Body(stmts);
                                    var scope = Pop();
                                    MergeScopes(scope);
                                    body[i] = new StaticIRStatement.While(cond, then);
                                    break;
                                }
                                case StaticIRStatement.DoWhile(var then, var cond): {
                                    Push();
                                    var stmts = then is StaticIRStatement.Body tbody ? tbody.Stmts : [then];
                                    FoldList(stmts);
                                    then = new StaticIRStatement.Body(stmts);
                                    var scope = Pop();
                                    MergeScopes(scope);
                                    cond = Fold(cond) ?? cond;
                                    body[i] = new StaticIRStatement.DoWhile(then, cond);
                                    break;
                                }
                                case StaticIRStatement.Body(var stmts):
                                    FoldList(stmts);
                                    break;
                                case SvcStmt(_, _, var outRegs): {
                                    foreach(var outReg in outRegs)
                                        if(outReg is StaticIRValue.GetFieldIndex(_, _, var regIndex, _))
                                            regs[regIndex] = null; // Can't know the results of svc calls
                                    break;
                                }
                                case LinkedBranch: {
                                    for(var j = 0; j < 32; ++j)
                                        regs[j] = null;
                                    N = Z = C = V = null;
                                    break;
                                }
                                case StaticIRStatement.Assign(var name, var value): {
                                    var foldedVal = Fold(value);
                                    if(foldedVal != null)
                                        body[i] = new StaticIRStatement.Assign(name, foldedVal);
                                    vars[name] = foldedVal ?? value;
                                    break;
                                }
                                case StaticIRStatement.SetFieldIndex(
                                    StaticIRValue.Named("State", _) ptr, "X", var reg, var val): {
                                    var foldedVal = Fold(val);
                                    if(foldedVal != null)
                                        body[i] = new StaticIRStatement.SetFieldIndex(ptr, "X", reg, foldedVal);
                                    regs[reg] = foldedVal ?? val;
                                    break;
                                }
                                case StaticIRStatement.SetField(
                                    StaticIRValue.Named("State", _) fptr, var fname, var fval) when
                                    fname is "NZCV_N" or "NZCV_Z" or "NZCV_C" or "NZCV_V": {
                                    var foldedVal = Fold(fval);
                                    if(foldedVal != null)
                                        body[i] = new StaticIRStatement.SetField(fptr, fname, foldedVal);
                                    foldedVal ??= fval;
                                    switch(fname) {
                                        case "NZCV_N": N = foldedVal; break;
                                        case "NZCV_Z": Z = foldedVal; break;
                                        case "NZCV_C": C = foldedVal; break;
                                        case "NZCV_V": V = foldedVal; break;
                                    }
                                    break;
                                }
                            }
                            StaticIRValue Fold(StaticIRValue val) {
                                var foldedVal = val.Transform(x => {
                                    return x switch {
                                        StaticIRValue.Add(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.Sub(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.And(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.Or(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.Xor(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.LeftShift(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.RightShift(var left, var right) => FoldBinary(x, left, right),
                                        StaticIRValue.EQ(var left, var right) => FoldComp(x, left, right),
                                        StaticIRValue.NE(var left, var right) => FoldComp(x, left, right),
                                        StaticIRValue.LT(var left, var right) => FoldComp(x, left, right),
                                        StaticIRValue.LTE(var left, var right) => FoldComp(x, left, right),
                                        StaticIRValue.GT(var left, var right) => FoldComp(x, left, right),
                                        StaticIRValue.GTE(var left, var right) => FoldComp(x, left, right),
                                        StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), "X", var regIndex, _) =>
                                            regs[regIndex] switch {
                                                StaticIRValue.Literal lit => lit,
                                                _ => null,
                                            },
                                        StaticIRValue.Named(var name, _) when vars.TryGetValue(name, out var val) =>
                                            val switch {
                                                StaticIRValue.Literal lit => lit,
                                                _ => null,
                                            },
                                        StaticIRValue.GetField(StaticIRValue.Named("State", _), "NZCV_N", _) => N,
                                        StaticIRValue.GetField(StaticIRValue.Named("State", _), "NZCV_Z", _) => Z,
                                        StaticIRValue.GetField(StaticIRValue.Named("State", _), "NZCV_C", _) => C,
                                        StaticIRValue.GetField(StaticIRValue.Named("State", _), "NZCV_V", _) => V,
                                        _ => null,
                                    };
                                });
                                if(foldedVal == null || ReferenceEquals(foldedVal, val)) return null;
                                folded = true;
                                foldedAny = true;
                                return foldedVal;
                            }
                        }
                    }
                } while(folded);
            } catch(Exception e) {
                Console.WriteLine($"Constant folding failed for block 0x{block.Start:X}");
                Console.WriteLine(e);
            }
        }
        return foldedAny;
    }

    static bool IsZero(StaticIRValue value) =>
        GetConstant(value) is var (type, cvalue) && Equals(cvalue, Activator.CreateInstance(type));

    static int? GetInt(StaticIRValue value) =>
        GetConstant(value) is var (_, cvalue) ? (int) Cast(cvalue, typeof(int)) : null;

    static bool IsAllOnes(StaticIRValue value) {
        if(GetConstant(value) is not var (type, cvalue)) return false;
        var bits = Marshal.SizeOf(type) * 8;
        if(bits > 64) return false;
        var mask = bits == 64 ? 0xFFFF_FFFF_FFFF_FFFFUL : (1UL << bits) - 1;
        return mask == (ulong) Cast(cvalue, typeof(ulong));
    }
    
    static bool IsConstant(StaticIRValue value) => GetConstant(value) is not null;
}