using System.Runtime.InteropServices;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void NaiveOpt() {
        Parallel.ForEach(WholeBlockGraph.Values, node => {
            while(FoldConstants(node) || RemoveRedundancies(node) || ResolveRoData(node)) { }
        });
    }
    
    static StaticIRValue CombineMasks(StaticIRValue a, StaticIRValue b) =>
        FoldBinary(new StaticIRValue.And(a, b), a, b);

    StaticIRValue RemoveRedundancy(StaticIRValue value) => value switch {
        StaticIRValue.Add(var left, var right) when IsZero(right) => left,
        StaticIRValue.Add(var left, var right) when IsZero(left) => right,
        StaticIRValue.Sub(var left, var right) when IsZero(right) => left,
        StaticIRValue.And(var left, _) when IsZero(left) => left,
        StaticIRValue.And(_, var right) when IsZero(right) => right,
        StaticIRValue.And(var left, var right) when IsAllOnes(right) => left,
        StaticIRValue.And(var left, var right) when IsAllOnes(left) => right,
        StaticIRValue.And(StaticIRValue.And(var left, var middle), var right)
            when IsConstant(middle) && IsConstant(right)
            => new StaticIRValue.And(left, CombineMasks(middle, right)),
        StaticIRValue.And(StaticIRValue.And(var left, var middle), var right)
            when IsConstant(left) && IsConstant(right)
            => new StaticIRValue.And(middle, CombineMasks(left, right)),
        StaticIRValue.And(StaticIRValue.And(var left, var middle), var right)
                when IsConstant(left) && IsConstant(middle)
            => new StaticIRValue.And(right, CombineMasks(left, middle)),
        StaticIRValue.And(var left, StaticIRValue.And(var middle, var right))
                when IsConstant(middle) && IsConstant(right)
            => new StaticIRValue.And(left, CombineMasks(middle, right)),
        StaticIRValue.And(var left, StaticIRValue.And(var middle, var right))
                when IsConstant(left) && IsConstant(right)
            => new StaticIRValue.And(middle, CombineMasks(left, right)),
        StaticIRValue.And(var left, StaticIRValue.And(var middle, var right))
                when IsConstant(left) && IsConstant(middle)
            => new StaticIRValue.And(right, CombineMasks(left, middle)),
        StaticIRValue.Or(var left, var right) when IsZero(right) => left,
        StaticIRValue.Or(var left, var right) when IsZero(left) => right,
        StaticIRValue.Xor(var left, var right) when IsZero(right) => left,
        StaticIRValue.Xor(var left, var right) when IsZero(left) => right,
        StaticIRValue.LeftShift(var left, var right) when IsZero(right) => left,
        StaticIRValue.LeftShift(var left, var right)
                when GetInt(right) is {} lshift && lshift >= Marshal.SizeOf(left.Type) * 8 =>
            new StaticIRValue.Literal(Activator.CreateInstance(left.Type), left.Type),
        StaticIRValue.RightShift(var left, var right)
                when GetInt(right) is {} lshift && lshift >= Marshal.SizeOf(left.Type) * 8 =>
            new StaticIRValue.Literal(Activator.CreateInstance(left.Type), left.Type),
        StaticIRValue.RightShift(var left, var right) when IsZero(right) => left,
        StaticIRValue.Cast(var val, var type) when !type.IsConstructedGenericType && GetConstant(val) is var (_, cval) =>
            new StaticIRValue.Literal(Cast(cval, type), type),
        StaticIRValue.Not(StaticIRValue.Not(var val)) => val,
        _ => null,
    };

    bool RemoveRedundancies(BlockGraph node) {
        var block = node.Block;
        var body = block.Body;
        var did = false;
        var didSome = false;
        do {
            did = false;
            body = body.TransformValues(value => {
                try {
                    var rep = RemoveRedundancy(value);
                    if(rep != null && !ReferenceEquals(value, rep))
                        did = didSome = true;
                    return rep;
                } catch(Exception e) {
                    Console.WriteLine($"Removing redundancies failed for block 0x{block.Start:X}");
                    Console.WriteLine(value);
                    Console.WriteLine(e);
                    return null;
                }
            });
        } while(did);
        if(!didSome) return false;
        node.Block = node.Block with { Body = body };
        return true;
    }

    bool ResolveRoData(BlockGraph node) {
        var block = node.Block;
        var body = block.Body;
        var foundData = false;
        for(var i = 0; i < body.Count; ++i) {
            var stmt = body[i];
            var nstmt = stmt.TransformValues(x => {
                if(x is StaticIRValue.Dereference(var pointer, var type) && GetConstant(pointer) is var (ptrType, _ptrValue)) {
                    if(ptrType != typeof(ulong)) {
                        Console.WriteLine($"Weird -- pointer has non-ulong type...? {x}");
                        return null;
                    }
                    var ptrValue = (ulong) _ptrValue;
                    if(!IsRoDataAddr(ptrValue)) return null;
                    var nval = type switch {
                        _ when type == typeof(byte) => (object) ExeLoader.Load<byte>(ptrValue),
                        _ when type == typeof(ushort) => ExeLoader.Load<ushort>(ptrValue),
                        _ when type == typeof(uint) => ExeLoader.Load<uint>(ptrValue),
                        _ when type == typeof(ulong) => ExeLoader.Load<ulong>(ptrValue),
                        _ when type == typeof(sbyte) => ExeLoader.Load<sbyte>(ptrValue),
                        _ when type == typeof(short) => ExeLoader.Load<short>(ptrValue),
                        _ when type == typeof(int) => ExeLoader.Load<int>(ptrValue),
                        _ when type == typeof(long) => ExeLoader.Load<long>(ptrValue),
                        _ => null
                    };
                    if(nval != null) return new StaticIRValue.Literal(nval, type);
                }
                return null;
            });
            if(nstmt != null && stmt != nstmt) {
                foundData = true;
                body[i] = nstmt;
            }
        }
        return foundData;
    }
}