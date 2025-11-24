using System.Diagnostics;
using System.Runtime.InteropServices;
using CoreArchCompiler;
using StaticRecompilerBase;
using System.Runtime.Intrinsics;

namespace NxRecompile;

public partial class CoreRecompiler {
    public void Output(CodeBuilder cb) {
        /*cb += "#include <stdint.h>";
        cb += "typedef struct SvcTable {{";
        cb++;
        foreach(var (name, inRegs, outRegs) in Svcs.All.Values) {
            if(outRegs.Length == 0)
                cb += $"void (*svc{name})({string.Join(", ", inRegs.Select(x => $"uint64_t in_{x}"))});";
            else
                cb += $"uint64_t (*svc{name})({string.Join(", ", inRegs.Select(x => $"uint64_t in_{x}").Concat(outRegs.Skip(1).Select(x => $"uint64_t *out_{x}")))});";
        }
        cb--;
        cb += "} SvcTable_t;";*/
        cb += "#include \"C/core.h\"";
        foreach(var blockAddr in WholeBlockGraph.Keys.Order())
            cb += $"uint64_t f_{blockAddr:X}();";
        foreach(var (blockAddr, node) in WholeBlockGraph.OrderBy(x => x.Key)) {
            cb += $"uint64_t f_{blockAddr:X}() {{ /* 0x{blockAddr:X} */";
            cb++;
            cb += $"/**** {(KnownFunctions.Contains(blockAddr) ? "function" : "block")} {node} ****/";
            var assignments = new Dictionary<string, Type>();
            node.Walk(sub => {
                new StaticIRStatement.Body(sub.Block.Body).Walk(stmt => {
                    if(stmt is not StaticIRStatement.Assign(var name, var value)) return;
                    if(assignments.TryGetValue(name, out var type)) {
                        Debug.Assert(type == value.Type);
                        return;
                    }
                    assignments[name] = value.Type;
                    cb += $"{Output(value.Type)} {name};";
                });
            });
            Output(cb, new StaticIRStatement.Body(node.Block.Body));
            cb--;
            cb += "}";
        }

        cb += $"int moduleCount = {ExeLoader.ExeModules.Count};";
        cb += "void ***jumpTable = (void **[]) {";
        cb++;
        foreach(var module in ExeLoader.ExeModules) {
            cb += "(void *[]) {";
            cb++;
            for(var addr = module.TextStart; addr < module.TextEnd; addr += 4)
                if(WholeBlockGraph.ContainsKey(addr))
                    cb += $"f_{addr:X},";
                else
                    cb += "(void*) 0,";
            cb += "(void*) 0";
            cb--;
            cb += "},";
        }
        cb += "(void *[]) { (void*) 0 }";
        cb--;
        cb += "};";

        foreach(var module in ExeLoader.ExeModules) {
            cb += $"uint8_t module_{module.LoadBase:X}[] = {{";
            cb++;
            var size = module.Binary.Length;
            if(module.BssEnd > module.LoadBase + (ulong) size)
                size = (int) (module.BssEnd - module.LoadBase);
            while(size % 16384 != 0) size++;
            var data = new byte[size];
            module.Binary.CopyTo(data);
            cb += string.Join(", ", data.Select(x => $"0x{x:X02}"));
            cb--;
            cb += "};";
            cb += $"uint32_t module_{module.LoadBase:X}_size = 0x{size:X};";
        }

        cb += "void loadModules() {";
        cb++;
        foreach(var module in ExeLoader.ExeModules)
            cb += $"Callbacks->loadModule(0x{module.LoadBase:X}ULL, module_{module.LoadBase:X}, module_{module.LoadBase:X}_size);";
        cb--;
        cb += "}";
    }

    void Output(CodeBuilder cb, StaticIRStatement stmt) {
        switch(stmt) {
            case StaticIRStatement.Body(var stmts): {
                foreach(var sub in stmts)
                    Output(cb, sub);
                break;
            }
            case StaticIRStatement.If(var cond, var then, var @else): {
                cb += $"if({Output(cond)}) {{";
                cb++;
                Output(cb, then);
                cb--;
                cb += "} else {";
                cb++;
                Output(cb, @else);
                cb--;
                cb += "}";
                break;
            }
            case StaticIRStatement.When(var cond, var then): {
                cb += $"if({Output(cond)}) {{";
                cb++;
                Output(cb, then);
                cb--;
                cb += "}";
                break;
            }
            case StaticIRStatement.Unless(var cond, var then): {
                cb += $"if(!({Output(cond)})) {{";
                cb++;
                Output(cb, then);
                cb--;
                cb += "}";
                break;
            }
            case StaticIRStatement.While(var cond, var loop): {
                cb += $"while({Output(cond)}) {{";
                cb++;
                Output(cb, loop);
                cb--;
                cb += "}";
                break;
            }
            case StaticIRStatement.DoWhile(var loop, var cond): {
                cb += "do {";
                cb++;
                Output(cb, loop);
                cb--;
                cb += $"}} while({Output(cond)});";
                break;
            }
            case StaticIRStatement.Branch(var target): {
                cb += $"return {Output(target)};";
                break;
            }
            case LinkedBranch(var target): {
                /*if(target is StaticIRValue.Literal(var value, _))
                    cb += $"f_{(ulong) value:X}();";
                else
                    cb += $"CALL({Output(target)});";*/
                cb += $"runFrom({Output(target)}, State->X[30]);";
                break;
            }
            case StaticIRStatement.Assign(var name, var value): {
                cb += $"{name} = {Output(value)};";
                break;
            }
            case StaticIRStatement.Dereference(var addr, var value): {
                cb += $"*({Output(value.Type)} *) ({Output(addr)}) = {Output(value)};";
                break;
            }
            case StaticIRStatement.SetField(var addr, var field, var value): {
                cb += $"({Output(addr)})->{field} = {Output(value)};";
                break;
            }
            case StaticIRStatement.SetFieldIndex(var addr, var field, var index, var value): {
                cb += $"({Output(addr)})->{field}[{index}] = {Output(value)};";
                break;
            }
            case StaticIRStatement.Sink(var value): {
                cb += $"{Output(value)};";
                break;
            }
            case SvcStmt(var name, var inRegs, var outRegs): {
                if(outRegs.Length == 0)
                    cb += $"Callbacks->svc{name}({string.Join(", ", inRegs.Select(Output))});";
                else if(outRegs.Length == 1)
                    cb += $"{Output(outRegs[0])} = Callbacks->svc{name}({string.Join(", ", inRegs.Select(Output))});";
                else
                    cb += $"{Output(outRegs[0])} = Callbacks->svc{name}({string.Join(", ", inRegs.Select(Output))}{(inRegs.Length != 0 ? ", " : "")}{string.Join(", ", outRegs.Skip(1).Select(v => $"&({Output(v)})"))});";
                break;
            }
            case WriteSrStmt(var op0, var op1, var crn, var crm, var op2, var value): {
                cb += $"Callbacks->writeSr({op0}, {op1}, {crn}, {crm}, {op2}, {Output(value)});";
                break;
            }
            case DebugStmt(var pc, var dasm): {
                cb += $"Callbacks->debug(0x{pc:X}ULL, \"{dasm}\");";
                break;
            }
            default:
                cb += $"/* Unhandled stmt {stmt} */";
                break;
        }
    }

    string BitwiseOp(string op, StaticIRValue left, StaticIRValue right) {
        var _left = Output(left);
        var _right = Output(right);
        if(left.Type.IsConstructedGenericType && left.Type.GetGenericTypeDefinition() == typeof(System.Runtime.Intrinsics.Vector128<>))
            return $"__builtin_bit_cast(v4f, __builtin_bit_cast(v16u, {_left}) {op} __builtin_bit_cast(v16u, {_right}))";
        return $"({_left}) {op} ({_right})";
    }

    string Output(StaticIRValue expr) {
        switch(expr) {
            case StaticIRValue.Literal(var value, var type): {
                return value switch {
                    sbyte v => $"(int8_t) {v}",
                    short v => $"(int16_t) {v}",
                    int v => $"(int32_t) {v}",
                    long v => $"(int64_t) {v}LL",
                    byte v => $"(uint8_t) 0x{v:X}U",
                    ushort v => $"(uint16_t) 0x{v:X}U",
                    uint v => $"(uint32_t) 0x{v:X}U",
                    ulong v => $"(uint64_t) 0x{v:X}ULL",
                    float v => $"{v:0.0###############}f",
                    double v => $"{v:0.0###############}",
                    System.Runtime.Intrinsics.Vector128<byte> v => $"(v16u) {{ {string.Join(", ", Enumerable.Range(0, 16).Select(i => v[i]))} }}",
                    UInt128 v => $"0x{v:X}ULL", // TODO: Make 128-bit literals. Fakin' it for now
                    _ => throw new NotImplementedException($"Literal value type {type}")
                };
            }
            case StaticIRValue.Named(var name, _): {
                return name;
            }
            case StaticIRValue.Add(var left, var right): {
                return $"({Output(left)}) + ({Output(right)})";
            }
            case StaticIRValue.Sub(var left, var right): {
                return $"({Output(left)}) - ({Output(right)})";
            }
            case StaticIRValue.Mul(var left, var right): {
                return $"({Output(left)}) * ({Output(right)})";
            }
            case StaticIRValue.Div(var left, var right): {
                return $"({Output(left)}) / ({Output(right)})";
            }
            case StaticIRValue.Mod(var left, var right): {
                return $"({Output(left)}) % ({Output(right)})";
            }
            case StaticIRValue.And(var left, var right): {
                return BitwiseOp("&", left, right);
            }
            case StaticIRValue.Or(var left, var right): {
                return BitwiseOp("|", left, right);
            }
            case StaticIRValue.Xor(var left, var right): {
                return BitwiseOp("^", left, right);
            }
            case StaticIRValue.LeftShift(var left, var right): {
                return $"({Output(left)}) << ({Output(right)})";
            }
            case StaticIRValue.RightShift(var left, var right): {
                return $"({Output(left)}) >> ({Output(right)})";
            }
            case StaticIRValue.Negate(var value): {
                return $"!({Output(value)})";
            }
            case StaticIRValue.Not(var value): {
                return $"~({Output(value)})";
            }
            case StaticIRValue.EQ(var left, var right): {
                return $"({Output(left)}) == {Output(right)}";
            }
            case StaticIRValue.NE(var left, var right): {
                return $"({Output(left)}) != {Output(right)}";
            }
            case StaticIRValue.LT(var left, var right): {
                return $"({Output(left)}) < {Output(right)}";
            }
            case StaticIRValue.LTE(var left, var right): {
                return $"({Output(left)}) <= {Output(right)}";
            }
            case StaticIRValue.GT(var left, var right): {
                return $"({Output(left)}) > {Output(right)}";
            }
            case StaticIRValue.GTE(var left, var right): {
                return $"({Output(left)}) >= {Output(right)}";
            }
            case StaticIRValue.Dereference(var addr, var type): {
                return $"*({Output(type)} *) ({Output(addr)})";
            }
            case StaticIRValue.GetField(var addr, var field, _): {
                return $"({Output(addr)})->{field}";
            }
            case StaticIRValue.GetFieldIndex(var addr, var field, var index, _): {
                return $"({Output(addr)})->{field}[{index}]";
            }
            case StaticIRValue.Store(var value): {
                return Output(value);
            }
            case StaticIRValue.Cast(var value, var type): {
                if(type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(System.Runtime.Intrinsics.Vector128<>))
                    return $"__builtin_bit_cast({Output(type)}, {Output(value)})";
                return $"({Output(type)}) ({Output(value)})";
            }
            case StaticIRValue.Bitcast(var value, var type): {
                return $"__builtin_bit_cast({Output(type)}, {Output(value)})";
            }
            case StaticIRValue.Ternary(var cond, var taken, var not): {
                return $"({Output(cond)}) ? ({Output(taken)}) : ({Output(not)})";
            }
            case StaticIRValue.CreateVector(var value): {
                return $"(v4f) {{ {Output(value)} }}";
            }
            case StaticIRValue.SignExt(var value, var width, var type): {
                return $"signext_{Output(value.Type)}_{Output(type)}({Output(value)}, {width})";
            }
            case StaticIRValue.ReverseBits(var value): {
                return $"__builtin_bitreverse{Marshal.SizeOf(value.Type) * 8}({Output(value)})";
            }
            case StaticIRValue.CountLeadingZeros(var value): {
                return $"__builtin_clzg({Output(value)})";
            }
            case StaticIRValue.SetElement(var vector, var index, var element): {
                return $"setElement_{Output(typeof(System.Runtime.Intrinsics.Vector128<>).MakeGenericType(element.Type))}({Output(vector)}, {Output(index)}, {Output(element)})";
            }
            case StaticIRValue.GetElement(var vector, var index, var etype): {
                return $"__builtin_bit_cast({Output(typeof(System.Runtime.Intrinsics.Vector128<>).MakeGenericType(etype))}, {Output(vector)})[{Output(index)}]";
            }
            case StaticIRValue.IsNaN(var value): {
                return $"isnan({Output(value)})";
            }
            case StaticIRValue.ZeroTop(var value): {
                return $"zerotop_{Output(value.Type)}({Output(value)})";
            }
            case StaticIRValue.Abs(var value): {
                if(value.Type == typeof(float))
                    return $"fabsf({Output(value)})";
                if(value.Type == typeof(double))
                    return $"fabs({Output(value)})";
                throw new NotImplementedException($"Unsupported type for abs: {value}");
            }
            case ReadSr(var op0, var op1, var crn, var crm, var op2): {
                return $"Callbacks->readSr({op0}, {op1}, {crn}, {crm}, {op2})";
            }
            default:
                throw new NotImplementedException($"Unhandled expression {expr}");
        }
    }

    string Output(Type type) =>
        type switch {
            var x when x == typeof(byte) => "uint8_t",
            var x when x == typeof(ushort) => "uint16_t",
            var x when x == typeof(uint) => "uint32_t",
            var x when x == typeof(ulong) => "uint64_t",
            var x when x == typeof(UInt128) => "uint128_t",
            var x when x == typeof(sbyte) => "int8_t",
            var x when x == typeof(short) => "int16_t",
            var x when x == typeof(int) => "int32_t",
            var x when x == typeof(long) => "int64_t",
            var x when x == typeof(Int128) => "int128_t",
            var x when x == typeof(bool) => "bool",
            var x when x == typeof(float) => "float",
            var x when x == typeof(double) => "double",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<sbyte>) => "v16i",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<short>) => "v8i",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<int>) => "v4i",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<long>) => "v2i",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<byte>) => "v16u",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<ushort>) => "v8u",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<uint>) => "v4u",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<ulong>) => "v2u",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<float>) => "v4f",
            var x when x == typeof(System.Runtime.Intrinsics.Vector128<double>) => "v2d",
            _ => throw new NotImplementedException($"Unhandled type for output: {type}")
        };
}