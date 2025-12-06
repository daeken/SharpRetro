using System.Diagnostics;
using System.Runtime.InteropServices;
using CoreArchCompiler;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    public void Output(CodeBuilder cb) {
        cb += "#include \"C/core.hpp\"";
        foreach(var blockAddr in WholeBlockGraph.Keys.Order())
            cb += $"uint64_t f_{blockAddr:X}();";
        foreach(var (blockAddr, node) in WholeBlockGraph.OrderBy(x => x.Key)) {
            cb += $"uint64_t f_{blockAddr:X}() {{ /* 0x{blockAddr:X} */";
            cb++;
            cb += $"/**** {(KnownFunctions.Contains(blockAddr) ? "function" : "block")} {node} ****/";
            var assignments = new Dictionary<string, Type>();
            new StaticIRStatement.Body(node.Block.Body).Walk(stmt => {
                if(stmt is not StaticIRStatement.Assign(var name, var value)) return;
                if(assignments.TryGetValue(name, out var type)) {
                    Debug.Assert(type == value.Type);
                    return;
                }
                assignments[name] = value.Type;
                cb += $"{Output(value.Type)} {name};";
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
                    cb += $"reinterpret_cast<void*>(f_{addr:X}),";
                else if(ProbablePadding.TryGetValue(addr, out var target))
                    cb += $"reinterpret_cast<void*>(f_{ResolvePadding(target):X}),";
                else
                    cb += "nullptr,";
            cb += "nullptr";
            cb--;
            cb += "},";
        }
        cb += "(void *[]) { nullptr }";
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

    ulong ResolvePadding(ulong addr) {
        while(ProbablePadding.TryGetValue(addr, out var taddr))
            addr = taddr;
        return addr;
    }

    void Output(CodeBuilder cb, StaticIRStatement stmt) {
        // if(stmt is not IStaticControlFlowStatement) cb += $"/* {stmt} */";
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
                if(target is StaticIRValue.Literal(var value, _) && IsSaneFunction((ulong) value))
                    cb += $"f_{(ulong) value:X}();";
                else
                    cb += $"runFrom({Output(target)}, State->X[30]);";
                break;
            }
            case StaticIRStatement.Assign(var name, var value) { SsaId: var ssaId }: {
                if(ssaId == -1)
                    cb += $"{name} = {Output(value)};";
                else
                    cb += $"{name}/*{ssaId}*/ = {Output(value)};";
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

    static string BitwiseOp(string op, StaticIRValue left, StaticIRValue right) {
        var _left = POutput(left);
        var _right = POutput(right);
        if(left.Type.IsConstructedGenericType && left.Type.GetGenericTypeDefinition() == typeof(System.Runtime.Intrinsics.Vector128<>))
            return $"__builtin_bit_cast(v4f, __builtin_bit_cast(v16u, {_left}) {op} __builtin_bit_cast(v16u, {_right}))";
        return $"{_left} {op} {_right}";
    }

    static bool NeedsParens(StaticIRValue expr) => expr switch {
        StaticIRValue.Literal or StaticIRValue.Named or StaticIRValue.Negate or StaticIRValue.Not or
        StaticIRValue.Cast or StaticIRValue.Bitcast or StaticIRValue.GetField or StaticIRValue.GetFieldIndex
            => false,
        StaticIRValue.Store(var val) => NeedsParens(val),
        _ => true,
    };
    static string POutput(StaticIRValue expr) =>
        NeedsParens(expr) ? $"({Output(expr)})" : Output(expr);

    static string EnsureSize(Type type, string expr) =>
        !type.IsConstructedGenericType && Marshal.SizeOf(type) < 4
            ? $"({Output(type)}) ({expr})"
            : expr;
    
    static string Output(StaticIRValue expr) {
        switch(expr) {
            case StaticIRValue.Literal(var value, var type): {
                return value switch {
                    bool v => v ? "TRUE" : "FALSE",
                    sbyte v => $"(int8_t) {v}",
                    short v => $"(int16_t) {v}",
                    int v => $"{v}",
                    long v => $"{v}LL",
                    byte v => $"(uint8_t) 0x{v:X}U",
                    ushort v => $"(uint16_t) 0x{v:X}U",
                    uint v => $"0x{v:X}U",
                    ulong v => $"0x{v:X}ULL",
                    float v => $"{v:0.0###############}f",
                    double v => $"{v:0.0###############}",
                    System.Runtime.Intrinsics.Vector128<byte> v => $"(v16u) {{ {string.Join(", ", Enumerable.Range(0, 16).Select(i => v[i]))} }}",
                    UInt128 v => $"(uint128_t) 0x{v:X}ULL", // TODO: Make 128-bit literals. Fakin' it for now
                    Int128 v => $"(int128_t) {v}LL", // TODO: Make 128-bit literals
                    _ => throw new NotImplementedException($"Literal value type {type}")
                };
            }
            case StaticIRValue.Named(var name, _) { SsaId: var ssaId }: {
                return ssaId == -1 ? name : $"{name}/*{ssaId}*/";
            }
            case StaticIRValue.NamedOut(var name, _) { SsaId: var ssaId }: {
                return ssaId == -1 ? name : $"{name}/*{ssaId}*/";
            }
            case StaticIRValue.Add(var left, var right): {
                return EnsureSize(left.Type, $"{POutput(left)} + {POutput(right)}");
            }
            case StaticIRValue.Sub(var left, var right): {
                return EnsureSize(left.Type, $"{POutput(left)} - {POutput(right)}");
            }
            case StaticIRValue.Mul(var left, var right): {
                return EnsureSize(left.Type, $"{POutput(left)} * {POutput(right)}");
            }
            case StaticIRValue.Div(var left, var right): {
                return EnsureSize(left.Type, $"{POutput(left)} / {POutput(right)}");
            }
            case StaticIRValue.Mod(var left, var right): {
                return EnsureSize(left.Type, $"{POutput(left)} % {POutput(right)}");
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
                return $"{POutput(left)} << {POutput(right)}";
            }
            case StaticIRValue.RightShift(var left, var right): {
                return $"{POutput(left)} >> {POutput(right)}";
            }
            case StaticIRValue.Negate(var value): {
                return $"-{POutput(value)}";
            }
            case StaticIRValue.Not(var value): {
                return $"{(value.Type == typeof(bool) ? "!" : "~")}{POutput(value)}";
            }
            case StaticIRValue.EQ(var left, var right): {
                return $"{POutput(left)} == {POutput(right)}";
            }
            case StaticIRValue.NE(var left, var right): {
                return $"{POutput(left)} != {POutput(right)}";
            }
            case StaticIRValue.LT(var left, var right): {
                return $"{POutput(left)} < {POutput(right)}";
            }
            case StaticIRValue.LTE(var left, var right): {
                return $"{POutput(left)} <= {POutput(right)}";
            }
            case StaticIRValue.GT(var left, var right): {
                return $"{POutput(left)} > {POutput(right)}";
            }
            case StaticIRValue.GTE(var left, var right): {
                return $"{POutput(left)} >= {POutput(right)}";
            }
            case StaticIRValue.Dereference(var addr, var type): {
                return $"*({Output(type)} *) {POutput(addr)}";
            }
            case StaticIRValue.GetField(var addr, var field, _): {
                return $"{POutput(addr)}->{field}";
            }
            case StaticIRValue.GetFieldIndex(var addr, var field, var index, _): {
                return $"{POutput(addr)}->{field}[{index}]";
            }
            case StaticIRValue.Store(var value): {
                return Output(value);
            }
            case StaticIRValue.Cast(var value, var type): {
                if(type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(System.Runtime.Intrinsics.Vector128<>))
                    return $"std::bit_cast<{Output(type)}>({Output(value)})";
                return $"static_cast<{Output(type)}>({Output(value)})";
            }
            case StaticIRValue.Bitcast(var value, var type): {
                return $"std::bit_cast<{Output(type)}>({Output(value)})";
            }
            case StaticIRValue.Ternary(var cond, var taken, var not): {
                return $"{POutput(cond)} ? {POutput(taken)} : {POutput(not)}";
            }
            case StaticIRValue.CreateVector(var value): {
                if(value.Type == typeof(float))
                    return $"(v4f) {{ {Output(value)} }}";
                return $"std::bit_cast<v4f>(({Output(typeof(System.Runtime.Intrinsics.Vector128<>).MakeGenericType(value.Type))}) {{ {Output(value)} }})";
            }
            case StaticIRValue.CreateFullVector(var values): {
                var vals = string.Join(", ", values.Select(Output));
                var vec = $"({Output(typeof(System.Runtime.Intrinsics.Vector128<>).MakeGenericType(values[0].Type))}) {{ {vals} }}";
                return values[0].Type == typeof(float) ? vec : $"std::bit_cast<v4f>({vec})";
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
                return $"reinterpret_cast<{Output(typeof(System.Runtime.Intrinsics.Vector128<>).MakeGenericType(etype))}>({Output(vector)})[{Output(index)}]";
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
            case StaticIRValue.Phi(var values): {
                return $"Phi({string.Join(", ", values.Select(Output))})";
            }
            default:
                throw new NotImplementedException($"Unhandled expression {expr}");
        }
    }

    static string Output(Type type) =>
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
            var x when x == typeof(bool) => "bool_t",
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