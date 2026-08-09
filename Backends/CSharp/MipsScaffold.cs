using ArchCompilerCore;
using Frontends.Mips;
using LibSharpRetro;
using static Backends.CSharp.CSharpEmit;

namespace Backends.CSharp;

// Rung-3b: port of legacy SharpStationGenerator/Program.cs Build* → CSharpEmit statics.
// mips = fixed-32bit mask/match (identical shape to aarch64), branch-delay-slot semantics
// live in the per-ISA HEADS (defer=/do-lds/pcd), not the scaffold.

public static class MipsScaffold {
    public static void RegisterAll() {
        ScalarMathEmit.Register();
        LogicEmit.Register();
        ControlFlowEmit.Register();
        VectorMathEmit.Register();
        StringManipulationEmit.Register();
        TopLevelProcessingEmit.Register();
        MipsEmit.Register();
    }

    public static void BuildDisassembler(List<MipsDef> defs, string stubDir, string outPath) {
        Context = ContextTypes.Disassembler;

        var c = new CodeBuilder();
        c += 2;
        var ic = new CodeBuilder();
        ic += 2;

        var labelNum = 0;

        foreach(var def in defs) {
            NextLabel = $"insn_{++labelNum}";
            c += $"/* {def.Name} */";
            c += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
            ic += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
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

    public static void BuildInterpreter(List<MipsDef> defs, string stubDir, string outPath) {
        Context = ContextTypes.Interpreter;
        var c = BuildEval(defs);
        var stub = File.ReadAllText(Path.Combine(stubDir, "InterpreterStub.cs.skip"));
        File.WriteAllText(outPath, stub.Replace("/*%CODE%*/", c.Code));
    }

    public static void BuildRecompiler(List<MipsDef> defs, string stubDir, string outPath) {
        Context = ContextTypes.Recompiler;
        var c = BuildEval(defs);
        var stub = File.ReadAllText(Path.Combine(stubDir, "RecompilerStub.cs.skip"));
        File.WriteAllText(outPath, stub.Replace("/*%CODE%*/", c.Code));
    }

    static CodeBuilder BuildEval(List<MipsDef> defs) {
        var c = new CodeBuilder();
        c += 2;
        var labelNum = 0;

        foreach(var def in defs) {
            NextLabel = $"insn_{++labelNum}";
            c += $"/* {def.Name} */";
            c += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
            c++;
            GenerateFields(c, def);
            GenerateStatement(c, def.Decode);
            GenerateStatement(c, def.Eval);
            c += "return true;";
            c--;
            c += "}";
            c += $"{NextLabel}:";
        }
        return c;
    }

    static void GenerateFields(CodeBuilder c, MipsDef def) {
        foreach(var (key, (bits, shift)) in def.Fields)
            c += $"var {key} = (insn >> {shift}) & 0x{(1 << bits) - 1:X}U;";
    }
}
