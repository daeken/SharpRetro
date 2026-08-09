using ArchCompilerCore;
using Frontends.Dmg;
using LibSharpRetro;
using static Backends.CSharp.CSharpEmit;

namespace Backends.CSharp;

// Rung-3: port of legacy DamageGenerator/Program.cs Build* → CSharpEmit statics.
// Byte-diff vs oracle-baseline/dmg/{Disassembler,Interpreter}.cs is the acceptance.
// dmg = variable-length byte-matched (per-byte MatchBytes dict + per-byte-anchored fields);
// contrast aarch64's fixed-32bit mask/match. Proves the frontend abstraction generalizes.

public static class DmgScaffold {
    public static void RegisterAll() {
        ScalarMathEmit.Register();
        LogicEmit.Register();
        ControlFlowEmit.Register();
        VectorMathEmit.Register();
        StringManipulationEmit.Register();
        TopLevelProcessingEmit.Register();
        DmgEmit.Register();
    }

    public static void BuildDisassembler(List<DmgDef> defs, string stubDir, string outPath) {
        Context = ContextTypes.Disassembler;

        var c = new CodeBuilder();
        c += 2;
        var ic = new CodeBuilder();
        ic += 2;

        var labelNum = 0;

        foreach(var def in defs) {
            NextLabel = $"insn_{++labelNum}";
            var matcher = string.Join(" && ", def.MatchBytes.Select(x => $"(insnBytes[{x.Key}] & 0x{x.Value.Mask:X}) == 0x{x.Value.Match:X}"));
            c += $"/* {def.Name} */";
            c += $"if({matcher}) {{";
            ic += $"if({matcher}) {{";
            c++;
            ic++;
            GenerateFields(c, def);
            GenerateStatement(c, def.Decode);
            GenerateFields(ic, def);
            GenerateStatement(ic, def.Decode);
            ic += $"return \"{def.Name}\";";
            c += $"return {GenerateExpression(def.Disassembly)};";
            c--;
            ic--;
            c += "}";
            ic += "}";
            c += $"{NextLabel}:";
            ic += $"{NextLabel}:";
        }

        var stub = File.ReadAllText(Path.Combine(stubDir, "DisassemblerStub.cs.skip"));
        File.WriteAllText(outPath, stub
            .Replace("/*%D_CODE%*/", c.Code)
            .Replace("/*%IC_CODE%*/", ic.Code)
            .Replace("/*%IC_COUNT%*/", defs.Count.ToString()));
    }

    public static void BuildInterpreter(List<DmgDef> defs, string stubDir, string outPath) {
        Context = ContextTypes.Interpreter;

        var c = new CodeBuilder();
        c += 2;
        var labelNum = 0;

        foreach(var def in defs) {
            NextLabel = $"insn_{++labelNum}";
            c += $"/* {def.Name} */";
            var matcher = string.Join(" && ", def.MatchBytes.Select(x => $"(insnBytes[{x.Key}] & 0x{x.Value.Mask:X}) == 0x{x.Value.Match:X}"));
            c += $"if({matcher}) {{";
            c++;
            GenerateFields(c, def);
            GenerateStatement(c, def.Decode);
            c += $"pc += {def.Size};";
            GenerateStatement(c, def.Eval);
            c += "return true;";
            c--;
            c += "}";
            c += $"{NextLabel}:";
        }

        var stub = File.ReadAllText(Path.Combine(stubDir, "InterpreterStub.cs.skip"));
        File.WriteAllText(outPath, stub.Replace("/*%CODE%*/", c.Code));
    }

    static void GenerateFields(CodeBuilder c, DmgDef def) {
        foreach(var (fname, (bi, size, shift)) in def.Fields)
            c += $"var {fname} = (byte) ((byte) (insnBytes[{bi}] >> {shift}) & 0x{(1 << size) - 1:X});";
    }
}
