using System.Runtime.Intrinsics;
using Aarch64Cpu;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void Unregister() {
        foreach(var key in WholeBlockGraph.Keys) {
            if(!KnownFunctions.Contains(key)) continue;
            Unregister(WholeBlockGraph[key]);
        }
    }

    void Unregister(BlockGraph node) {
        // We're only handling a single block here
        if(node is not BlockGraph.End) return;
        
        var body = node.Block.Body;
        var regNames = Enumerable.Range(0, 32).Select(x => (x, new StaticIRValue.Named($"X{x}", typeof(ulong)))).ToDictionary();
        var vecNames = Enumerable.Range(0, 32).Select(x => (x, new StaticIRValue.Named($"V{x}", typeof(Vector128<float>)))).ToDictionary();
        var spName = new StaticIRValue.Named("SP", typeof(ulong));
        var nName = new StaticIRValue.Named("N", typeof(bool));
        var zName = new StaticIRValue.Named("Z", typeof(bool));
        var cName = new StaticIRValue.Named("C", typeof(bool));
        var vName = new StaticIRValue.Named("V", typeof(bool));
        body = body.Transform(stmt => {
            if(stmt is StaticIRStatement.SetFieldIndex { Pointer: StaticIRValue.Named("State", _), Name: var rName, Index: var index, Value: var value })
                return new StaticIRStatement.Assign($"{rName}{index}", value);
            if(stmt is StaticIRStatement.SetField {
                   Pointer: StaticIRValue.Named("State", _), 
                   Name: "SP" or "NZCV_N" or "NZCV_Z" or  "NZCV_C" or "NZCV_V", 
                   Value: var rValue
               } sf)
                return new StaticIRStatement.Assign(sf.Name.Split('_')[^1], rValue);
            if(stmt is SvcStmt(var name, var inRegs, var outRegs)) {
                if(inRegs.Any(x => x is StaticIRValue.Named) || outRegs.Any(x => x is StaticIRValue.Named)) return null;
                return new SvcStmt(
                    name,
                    inRegs.Select(StaticIRValue (x) => regNames[((StaticIRValue.GetFieldIndex) x).Index]).ToArray(),
                    outRegs.Select(StaticIRValue (x) => regNames[((StaticIRValue.GetFieldIndex) x).Index]).ToArray()
                );
            }
            return null;
        }, value => {
            if(value is StaticIRValue.GetFieldIndex { Pointer: StaticIRValue.Named("State", _), Name: var rName, Index: var index })
                return rName == "X" ? regNames[index] : vecNames[index];
            if(value is StaticIRValue.GetField { Pointer: StaticIRValue.Named("State", _), Name: "SP" })
                return spName;
            if(value is StaticIRValue.GetField { Pointer: StaticIRValue.Named("State", _), Name: "NZCV_N" })
                return nName;
            if(value is StaticIRValue.GetField { Pointer: StaticIRValue.Named("State", _), Name: "NZCV_Z" })
                return zName;
            if(value is StaticIRValue.GetField { Pointer: StaticIRValue.Named("State", _), Name: "NZCV_C" })
                return cName;
            if(value is StaticIRValue.GetField { Pointer: StaticIRValue.Named("State", _), Name: "NZCV_V" })
                return vName;
            return null;
        });
        var state = new StaticIRValue.Named("State", typeof(CpuState));
        var storeValues = new List<StaticIRStatement>();
        var loadValues = new List<StaticIRStatement>();
        foreach(var (i, named) in regNames) {
            storeValues.Add(new StaticIRStatement.SetFieldIndex(state, "X", i, named));
            loadValues.Add(new StaticIRStatement.Assign($"X{i}", new StaticIRValue.GetFieldIndex(state, "X", i, typeof(ulong))));
        }
        foreach(var (i, named) in vecNames) {
            storeValues.Add(new StaticIRStatement.SetFieldIndex(state, "V", i, named));
            loadValues.Add(new StaticIRStatement.Assign($"V{i}", new StaticIRValue.GetFieldIndex(state, "V", i, typeof(Vector128<float>))));
        }
        storeValues.Add(new StaticIRStatement.SetField(state, "SP", spName));
        loadValues.Add(new StaticIRStatement.Assign("SP", new StaticIRValue.GetField(state, "SP", typeof(ulong))));
        storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_N", nName));
        loadValues.Add(new StaticIRStatement.Assign("N", new StaticIRValue.GetField(state, "NZCV_N", typeof(bool))));
        storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_Z", zName));
        loadValues.Add(new StaticIRStatement.Assign("Z", new StaticIRValue.GetField(state, "NZCV_Z", typeof(bool))));
        storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_C", cName));
        loadValues.Add(new StaticIRStatement.Assign("C", new StaticIRValue.GetField(state, "NZCV_C", typeof(bool))));
        storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_V", vName));
        loadValues.Add(new StaticIRStatement.Assign("V", new StaticIRValue.GetField(state, "NZCV_V", typeof(bool))));
        body = body.Transform(stmt => {
            if(stmt is LinkedBranch)
                return new StaticIRStatement.Body(storeValues.Concat([stmt]).Concat(loadValues).ToList());
            if(stmt is StaticIRStatement.Branch)
                return new StaticIRStatement.Body(storeValues.Concat([stmt]).ToList());
            return null;
        });
        node.Block = node.Block with { Body = loadValues.Concat(body).ToList() };
    }
}