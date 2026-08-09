using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy SharpStationGenerator/Expressions.cs
public static class MipsEmit {
    public static void Register() {
        Expression("pc",
            _ => "pc");
        Expression("pcd",
            _ => "(pc + 4)");
        Expression("reg",
            list => {
					try {
						var rnum = new ExecutionState().Evaluate(list[1]);
						if(rnum == 0) return "0U";
						return $"State->Registers[{rnum}]";
					} catch(Exception) {}
					var tn = TempName();
					return $"({GenerateExpression(list[1])}) switch {{ 0 => 0U, var {tn} => State->Registers[{tn}] }}";
				},
            list => {
					try {
						var rnum = new ExecutionState().Evaluate(list[1]);
						if(rnum == 0) return "builder.Zero<uint>()";
						return $"state.Registers[builder.LiteralValue<int>({rnum})]";
					} catch(Exception) {}
					return $"state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime({GenerateExpression(list[1])})]";
				});
        Expression("reg-hi",
            _ => "State->Hi",
            _ => "state.Hi");
        Expression("reg-lo",
            _ => "State->Lo",
            _ => "state.Lo");
        Expression("absorb-muldiv-delay",
            _ => "AbsorbMuldivDelay()");
        Expression("copfun",
            list => $"Copfun({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"builder.CallVoid(Copfun, (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[1])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])}))");
        Expression("exception",
            list => $"throw new CpuException(ExceptionType.{list[1]}, pc, insn)",
            list => $"builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.{list[1]}), builder.LiteralValue(pc), builder.LiteralValue(insn))");
        Expression("copreg",
            list => $"Copreg({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"builder.Call<uint, uint, uint>(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[1])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])}))");
        Expression("copcreg",
            list => $"Copcreg({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"builder.Call<uint, uint, uint>(Copcreg, (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[1])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])}))");
        Expression("mul-delay",
            list => $"MulDelay({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])} != 0)");
        Expression("div-delay",
            _ => "DivDelay()");
        Statement("=",
            (c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("reg"):
							try {
								var rnum = new ExecutionState().Evaluate(sub[1]);
								if(rnum != 0)
									c += $"State->Registers[{rnum}] = {GenerateExpression(list[2].Cast<uint>())};";
								return;
							} catch(Exception) {}
							var rtemp = TempName();
							c += $"var {rtemp} = {GenerateExpression(sub[1])};";
							c += $"if({rtemp} != 0)";
							c++;
							c += $"State->Registers[{rtemp}] = {GenerateExpression(list[2].Cast<uint>())};";
							c--;
							return;
						case PName("reg-hi") or PName("reg-lo"):
							c += $"State->{(sub[0] is PName("reg-hi") ? "Hi" : "Lo")} = {GenerateExpression(list[2].Cast<uint>())};";
							return;
						case PName("copreg"):
							c += $"Copreg({GenerateExpression(sub[1])}, {GenerateExpression(sub[2])}, {GenerateExpression(list[2])});";
							return;
						case PName("copcreg"):
							c += $"Copcreg({GenerateExpression(sub[1])}, {GenerateExpression(sub[2])}, {GenerateExpression(list[2])});";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			},
            (c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("reg"):
							try {
								var rnum = new ExecutionState().Evaluate(sub[1]);
								if(rnum != 0)
									c += $"state.Registers[builder.LiteralValue({rnum})] = (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
								return;
							} catch(Exception) {}
							var rtemp = TempName();
							c += $"var {rtemp} = {GenerateExpression(sub[1])};";
							if(sub[1].Type.Runtime) {
								c += $"builder.When((IRuntimeValue<uint>) builder.EnsureRuntime({rtemp}) != builder.LiteralValue(0U),";
								c++;
								c += $"() => state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime({rtemp})] = (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])}));";
								c--;
							} else {
								c += $"if({rtemp} != 0)";
								c++;
								c += $"state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime({rtemp})] = (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
								c--;
							}
							return;
						case PName("reg-hi") or PName("reg-lo"):
							c += $"state.{(sub[0] is PName("reg-hi") ? "Hi" : "Lo")} = (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("copreg"):
							c += $"builder.CallVoid<uint, uint, uint>(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(sub[1])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(sub[2])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])}));";
							return;
						case PName("copcreg"):
							c += $"builder.CallVoid<uint, uint, uint>(Copcreg, (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(sub[1])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(sub[2])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])}));";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			});
        Statement("defer=",
            (c, list) => {
				if(list[1] is not PList sub) throw new NotSupportedException();
				switch(sub[0]) {
					case PName("reg"):
						c += $"State->LdWhich = (uint) ({GenerateExpression(sub[1])});";
						c += $"State->LdValue = (uint) ({GenerateExpression(list[2])});";
						break;
					default:
						throw new NotSupportedException($"Defer= used on non-reg argument {sub}");
				}
			},
            (c, list) => {
				if(list[1] is not PList sub) throw new NotSupportedException();
				switch(sub[0]) {
					case PName("reg"):
						c += $"state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(sub[1])});";
						c += $"state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
						break;
					default:
						throw new NotSupportedException($"Defer= used on non-reg argument {sub}");
				}
			});
        Expression("load",
            list => {
					var type = GenerateType(list.Type);
					return $"ReadMemory<{type}>({GenerateExpression(list[1])})";
				},
            list =>
					$"builder.Pointer<{GenerateType(list.Type.AsCompiletime())}>({GenerateExpression(list[1])}).Value");
        Expression("store",
            list => $"WriteMemory({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list =>
					$"builder.Pointer<{GenerateType(list[2].Type.AsCompiletime())}>({GenerateExpression(list[1])}).Value = {GenerateExpression(list[2])}");
        Expression("do-load",
            list => $"DoLoad({GenerateExpression(list[1])}, ref {GenerateExpression(list[2])})");
        Statement("do-lds",
            (c, _) => {
			c += "DoLds();";
		});
        Statement("read-absorb",
            (c, list) => {
			c += $"State->ReadAbsorb[{GenerateExpression(list[1])}] = 0;";
		},
            (c, list) => {
			c += $"state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime({GenerateExpression(list[1])})] = builder.LiteralValue(0U);";
		});
    }
}
