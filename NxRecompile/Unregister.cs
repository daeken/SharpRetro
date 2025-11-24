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
        
        var regsModified = new HashSet<int>();
        var vecsModified = new HashSet<int>();
        var spModified = false;
        var nModified = false;
        var zModified = false;
        var cModified = false;
        var vModified = false;
        
        var body = new StaticIRStatement.Body(node.Block.Body);
        body.Walk(stmt => {
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
                foreach(var reg in outRegs) {
                    if(reg is not StaticIRValue.GetFieldIndex(_, "X", var index, _))
                        throw new NotSupportedException();
                    regsModified.Add(index);
                }
            }
        });
        var regNames = Enumerable.Range(0, 32).Select(x => (x, new StaticIRValue.Named($"X{x}", typeof(ulong)))).ToDictionary();
        var vecNames = Enumerable.Range(0, 32).Select(x => (x, new StaticIRValue.Named($"V{x}", typeof(Vector128<float>)))).ToDictionary();
        var spName = new StaticIRValue.Named("SP", typeof(ulong));
        var nName = new StaticIRValue.Named("N", typeof(bool));
        var zName = new StaticIRValue.Named("Z", typeof(bool));
        var cName = new StaticIRValue.Named("C", typeof(bool));
        var vName = new StaticIRValue.Named("V", typeof(bool));
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
        body = new Ssaify().Transform(body);
        var seenXr = Enumerable.Range(0, 32).Select(_ => false).ToArray();
        var needXr = Enumerable.Range(0, 32).Select(_ => false).ToArray();
        var seenVr = Enumerable.Range(0, 32).Select(_ => false).ToArray();
        var needVr = Enumerable.Range(0, 32).Select(_ => false).ToArray();
        var seenSp = false;
        var needSp = false;
        var seenN = false;
        var needN = false;
        var seenZ = false;
        var needZ = false;
        var seenC = false;
        var needC = false;
        var seenV = false;
        var needV = false;
        body.Walk(stmt => {
            if(stmt is not StaticIRStatement.Assign(var name, _)) return;
            if(name[0] == 'X') seenXr[int.Parse(name[1..])] = true;
            else if(name[0] == 'V' && name.Length > 1) seenVr[int.Parse(name[1..])] = true;
            else if(name == "SP" && !seenSp) seenSp = true;
            else if(name == "N" && !seenSp) seenN = true;
            else if(name == "Z" && !seenSp) seenZ = true;
            else if(name == "C" && !seenSp) seenC = true;
            else if(name == "V" && !seenSp) seenV = true;
        }, value => {
            if(value is not StaticIRValue.Named(var name, _)) return;
            if(name[0] == 'X' && !seenXr[int.Parse(name[1..])]) {
                var reg = int.Parse(name[1..]);
                needXr[reg] = seenXr[reg] = true;
            } else if(name[0] == 'V' && name.Length > 1 && !seenVr[int.Parse(name[1..])]) {
                var reg = int.Parse(name[1..]);
                needVr[reg] = seenVr[reg] = true;
            } else if(name == "SP" && !seenSp) needSp = seenSp = true;
            else if(name == "N" && !seenN) needN = seenN = true;
            else if(name == "Z" && !seenZ) needZ = seenZ = true;
            else if(name == "C" && !seenC) needC = seenC = true;
            else if(name == "V" && !seenV) needV = seenV = true;
        });
        var state = new StaticIRValue.Named("State", typeof(CpuState));
        var storeValues = new List<StaticIRStatement>();
        var loadValues = new List<StaticIRStatement>();
        foreach(var (i, named) in regNames) {
            if(regsModified.Contains(i))
                storeValues.Add(new StaticIRStatement.SetFieldIndex(state, "X", i, named));
            if(needXr[i])
                loadValues.Add(new StaticIRStatement.Assign($"X{i}", new StaticIRValue.GetFieldIndex(state, "X", i, typeof(ulong))));
        }
        foreach(var (i, named) in vecNames) {
            if(vecsModified.Contains(i))
                storeValues.Add(new StaticIRStatement.SetFieldIndex(state, "V", i, named));
            if(needVr[i])
                loadValues.Add(new StaticIRStatement.Assign($"V{i}", new StaticIRValue.GetFieldIndex(state, "V", i, typeof(Vector128<float>))));
        }
        if(spModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "SP", spName));
        if(needSp)
            loadValues.Add(new StaticIRStatement.Assign("SP", new StaticIRValue.GetField(state, "SP", typeof(ulong))));
        if(nModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_N", nName));
        if(needN)
            loadValues.Add(new StaticIRStatement.Assign("N", new StaticIRValue.GetField(state, "NZCV_N", typeof(bool))));
        if(zModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_Z", zName));
        if(needZ)
            loadValues.Add(new StaticIRStatement.Assign("Z", new StaticIRValue.GetField(state, "NZCV_Z", typeof(bool))));
        if(cModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_C", cName));
        if(needC)
            loadValues.Add(new StaticIRStatement.Assign("C", new StaticIRValue.GetField(state, "NZCV_C", typeof(bool))));
        if(vModified)
            storeValues.Add(new StaticIRStatement.SetField(state, "NZCV_V", vName));
        if(needV)
            loadValues.Add(new StaticIRStatement.Assign("V", new StaticIRValue.GetField(state, "NZCV_V", typeof(bool))));
        body = (StaticIRStatement.Body) body.Transform(stmt => {
            if(stmt is LinkedBranch)
                return new StaticIRStatement.Body(storeValues.Concat([stmt]).Concat(loadValues).ToList());
            if(stmt is StaticIRStatement.Branch)
                return new StaticIRStatement.Body(storeValues.Concat([stmt]).ToList());
            return null;
        });
        node.Block = node.Block with { Body = loadValues.Concat(body.Stmts).ToList() };
    }
}