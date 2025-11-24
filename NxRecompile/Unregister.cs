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
        if(node is not BlockGraph.End) return;
        var regsModified = new HashSet<int>();
        var regsUsed = new HashSet<int>();
        var vecsModified = new HashSet<int>();
        var vecsUsed = new HashSet<int>();
        var spModified = false;
        var spUsed = false;
        var nModified = false;
        var nUsed = false;
        var zModified = false;
        var zUsed = false;
        var cModified = false;
        var cUsed = false;
        var vModified = false;
        var vUsed = false;
        
        node.Walk(sub => {
            new StaticIRStatement.Body(sub.Block.Body).Walk(stmt => {
                if(stmt is StaticIRStatement.SetField { Pointer: StaticIRValue.Named("State", _), Name: var name }) {
                    if(name.StartsWith("X"))
                        regsModified.Add(int.Parse(name[1..]));
                    else if(name.StartsWith("V"))
                        vecsModified.Add(int.Parse(name[1..]));
                    else if(name == "SP")
                        spModified = true;
                    else if(name == "NZCV_N")
                        nModified = true;
                    else if(name == "NZCV_Z")
                        zModified = true;
                    else if(name == "NZCV_C")
                        cModified = true;
                    else if(name == "NZCV_V")
                        vModified = true;
                } else if(stmt is StaticIRStatement.SetFieldIndex { Pointer: StaticIRValue.Named("State", _), Name: "X", Index: var xIndex })
                    regsModified.Add(xIndex);
                else if(stmt is StaticIRStatement.SetFieldIndex { Pointer: StaticIRValue.Named("State", _), Name: "V", Index: var vIndex })
                    vecsModified.Add(vIndex);
                else if(stmt is SvcStmt(_, var inRegs, var outRegs)) {
                    foreach(var reg in inRegs) {
                        if(reg is not StaticIRValue.GetFieldIndex(_, "X", var index, _))
                            throw new NotSupportedException();
                        regsUsed.Add(index);
                    }
                    foreach(var reg in outRegs) {
                        if(reg is not StaticIRValue.GetFieldIndex(_, "X", var index, _))
                            throw new NotSupportedException();
                        regsModified.Add(index);
                    }
                }
            }, value => {
                if(value is StaticIRValue.GetField { Pointer: StaticIRValue.Named("State", _), Name: var name }) {
                    if(name.StartsWith("X"))
                        regsUsed.Add(int.Parse(name[1..]));
                    else if(name.StartsWith("V"))
                        vecsUsed.Add(int.Parse(name[1..]));
                    else if(name == "SP")
                        spUsed = true;
                    else if(name == "NZCV_N")
                        nUsed = true;
                    else if(name == "NZCV_Z")
                        zUsed = true;
                    else if(name == "NZCV_C")
                        cUsed = true;
                    else if(name == "NZCV_V")
                        vUsed = true;
                } else if(value is StaticIRValue.GetFieldIndex { Pointer: StaticIRValue.Named("State", _), Name: "X", Index: var xIndex })
                    regsUsed.Add(xIndex);
                else if(value is StaticIRValue.GetFieldIndex { Pointer: StaticIRValue.Named("State", _), Name: "V", Index: var vIndex })
                    vecsUsed.Add(vIndex);
            });
        });
        var regNames = regsUsed.Union(regsModified).Select(x => (x, new StaticIRValue.Named($"X{x}", typeof(ulong)))).ToDictionary();
        var vecNames = vecsUsed.Union(vecsModified).Select(x => (x, new StaticIRValue.Named($"V{x}", typeof(Vector128<float>)))).ToDictionary();
        var spName = new StaticIRValue.Named("SP", typeof(ulong));
        var nName = new StaticIRValue.Named("N", typeof(bool));
        var zName = new StaticIRValue.Named("Z", typeof(bool));
        var cName = new StaticIRValue.Named("C", typeof(bool));
        var vName = new StaticIRValue.Named("V", typeof(bool));
        var state = new StaticIRValue.Named("State", typeof(CpuState));
        var storeValues = new List<StaticIRStatement>();
        var loadValues = new List<StaticIRStatement>();
        foreach(var (i, named) in regNames) {
            if(regsModified.Contains(i))
                storeValues.Add(new StaticIRStatement.SetFieldIndex(state, "X", i, named));
            if(regsUsed.Contains(i))
                loadValues.Add(new StaticIRStatement.Assign($"X{i}", new StaticIRValue.GetFieldIndex(state, "X", i, typeof(ulong))));
        }
        foreach(var (i, named) in vecNames) {
            if(vecsModified.Contains(i))
                storeValues.Add(new StaticIRStatement.SetFieldIndex(state, "V", i, named));
            if(vecsUsed.Contains(i))
                loadValues.Add(new StaticIRStatement.Assign($"V{i}", new StaticIRValue.GetFieldIndex(state, "V", i, typeof(Vector128<float>))));
        }
        if(spModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "SP", spName));
        if(spUsed)
            loadValues.Add(new StaticIRStatement.Assign("SP", new StaticIRValue.GetField(state, "SP", typeof(ulong))));
        if(nModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_N", nName));
        if(nUsed)
            loadValues.Add(new StaticIRStatement.Assign("N", new StaticIRValue.GetField(state, "NZCV_N", typeof(bool))));
        if(zModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_Z", zName));
        if(zUsed)
            loadValues.Add(new StaticIRStatement.Assign("Z", new StaticIRValue.GetField(state, "NZCV_Z", typeof(bool))));
        if(cModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_C", cName));
        if(cUsed)
            loadValues.Add(new StaticIRStatement.Assign("C", new StaticIRValue.GetField(state, "NZCV_C", typeof(bool))));
        if(vModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_V", vName));
        if(vUsed)
            loadValues.Add(new StaticIRStatement.Assign("V", new StaticIRValue.GetField(state, "NZCV_V", typeof(bool))));
        node.Walk(sub => {
            var body = new StaticIRStatement.Body(sub.Block.Body);
            body = (StaticIRStatement.Body) body.Transform(stmt => {
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
            body = (StaticIRStatement.Body) body.Transform(stmt => {
                if(stmt is LinkedBranch)
                    return new StaticIRStatement.Body(storeValues.Concat([stmt]).Concat(loadValues).ToList());
                if(stmt is StaticIRStatement.Branch)
                    return new StaticIRStatement.Body(storeValues.Concat([stmt]).ToList());
                return null;
            });
            sub.Block = sub.Block with { Body = body.Stmts.ToList() };
        });
        node.Block = node.Block with { Body = loadValues.Concat(node.Block.Body).ToList() };
    }
}