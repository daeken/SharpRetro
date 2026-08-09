using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy DamageGenerator/Expressions.cs
public static class DmgEmit {
    public static void Register() {

		string RegName(int id) => id switch {
			0b000 => "B", 
			0b001 => "C", 
			0b010 => "D", 
			0b011 => "E", 
			0b100 => "H", 
			0b101 => "L", 
			0b110 => throw new BailoutException(), 
			0b111 => "A", 
			_ => throw new NotSupportedException()
		};
		
        Expression("pc",
            _ => "pc");
        Expression("reg",
            list => $"({GenerateExpression(list[1])}) switch {{ 0b110 => throw new NotSupportedException(), {{}} i => State.Registers[i] }}",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-ime",
            _ => "State.InterruptsEnabled",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-ime-schedule",
            _ => "State.InterruptsEnableScheduled",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-flags",
            _ => "State.Flags",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-bc",
            _ => "((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-de",
            _ => "((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-hl",
            _ => "((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-af",
            _ => "((((ushort) State.Registers[0b111]) << 8) | (ushort) State.Flags)",
            _ => "/*UNIMPLEMENTED*/");
        Expression("reg-sp",
            _ => "State.SP",
            _ => "/*UNIMPLEMENTED*/");
        Statement("=",
            (c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("reg"):
							c += $"State.Registers[(int) {GenerateExpression(sub[1])}] = (byte) ({GenerateExpression(list[2])});";
							return;
						case PName("reg-bc"):
						case PName("reg-de"):
						case PName("reg-hl"):
							var temp = TempName();
							c += $"var {temp} = (ushort) {GenerateExpression(list[2])};";
							var (a, b) = ((PName) sub[0]).Name switch {
								"reg-bc" => ("0b000", "0b001"), 
								"reg-de" => ("0b010", "0b011"), 
								"reg-hl" => ("0b100", "0b101"), 
								_ => throw new NotSupportedException()
							};
							c += $"State.Registers[{a}] = (byte) ({temp} >> 8);";
							c += $"State.Registers[{b}] = (byte) ({temp} & 0xFF);";
							return;
						case PName("reg-af"):
							var aftemp = TempName();
							c += $"var {aftemp} = (ushort) {GenerateExpression(list[2])};";
							c += $"State.Registers[0b111] = (byte) ({aftemp} >> 8);";
							c += $"State.Flags = (byte) ({aftemp} & 0xF0);";
							return;
						case PName("reg-flags"):
							c += $"State.Flags = (byte) ({GenerateExpression(list[2])} & 0xF0);";
							return;
						case PName("reg-sp"):
							c += $"State.SP = (ushort) {GenerateExpression(list[2])};";
							return;
						case PName("reg-ime"):
							c += $"State.InterruptsEnabled = {GenerateExpression(list[2])};";
							return;
						case PName("reg-ime-schedule"):
							c += $"State.InterruptsEnableScheduled = {GenerateExpression(list[2])};";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			},
            (c, list) => {
				c += $"/*UNIMPLEMENTED*/";
			});
        Expression("load",
            list => {
					var type = GenerateType(list.Type);
					return $"ReadMemory<{type}>({GenerateExpression(list[1])})";
				},
            list =>
					$"((RuntimePointer<{GenerateType(list.Type.AsCompiletime())}>) ({GenerateExpression(list[1])})).value()");
        Expression("store",
            list => $"WriteMemory({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list =>
					$"((RuntimePointer<{GenerateType(list[2].Type.AsCompiletime())}>) ({GenerateExpression(list[1])})).value({GenerateExpression(list[2])})");
        BranchExpression("branch-default",
            list => "Branch(pc)");
        BranchExpression("cycles",
            list => $"AddCycles({GenerateExpression(list[1])})");
        Expression("halt",
            _ => "Halt()");
    }
}
