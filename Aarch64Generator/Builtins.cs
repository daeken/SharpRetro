using System.Diagnostics;
using CoreArchCompiler;
using static Aarch64Common.Common;

namespace Aarch64Generator;

public class Builtins : Builtin {
	public override void Define() {
		Statement("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException(),
			(c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("gpr32"):
							c += $"state->X[(int) {GenerateExpression(sub[1])}] = (ulong) (uint) ({GenerateExpression(list[2])});";
							return;
						case PName("gpr-or-sp32"):
							c += $"if({GenerateExpression(sub[1])} == 31)";
							c++;
							c += $"state->SP = (ulong) (uint) ({GenerateExpression(list[2])});";
							c--;
							c += "else";
							c++;
							c += $"state->X[(int) {GenerateExpression(sub[1])}] = (ulong) (uint) ({GenerateExpression(list[2])});";
							c--;
							return;
						case PName("gpr64"):
							c += $"state->X[(int) {GenerateExpression(sub[1])}] = {GenerateExpression(list[2])};";
							return;
						case PName("gpr-or-sp64"):
							c += $"if({GenerateExpression(sub[1])} == 31)";
							c++;
							c += $"state->SP = {GenerateExpression(list[2])};";
							c--;
							c += "else";
							c++;
							c += $"state->X[(int) {GenerateExpression(sub[1])}] = {GenerateExpression(list[2])};";
							c--;
							return;
						
						case PName("vec-b"):
							c += $"state->V[(int) ({GenerateExpression(sub[1])})] = reinterpret_cast<Vector128<float>>((Vector128<uint8_t>) {{ {GenerateExpression(list[2])}, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }});";
							return;
						case PName("vec-h"):
							c += $"state->V[(int) ({GenerateExpression(sub[1])})] = reinterpret_cast<Vector128<float>>((Vector128<uint16_t>) {{ {GenerateExpression(list[2])}, 0, 0, 0, 0, 0, 0, 0 }});";
							return;
						case PName("vec-s"):
							c += $"state->V[(int) ({GenerateExpression(sub[1])})] = (Vector128<float>) {{ {GenerateExpression(list[2])}, 0, 0, 0 }};";
							return;
						case PName("vec-d"):
							c += $"state->V[(int) ({GenerateExpression(sub[1])})] = reinterpret_cast<Vector128<float>>((Vector128<double>) {{ {GenerateExpression(list[2])}, 0 }});";
							return;
						
						case PName("sr"):
							c += $"SR({GenerateExpression(sub[1])}, {GenerateExpression(sub[2])}, {GenerateExpression(sub[3])}, {GenerateExpression(sub[4])}, {GenerateExpression(sub[5])}, {GenerateExpression(list[2])});";
							return;
						
						case PName("nzcv") when sub.Count == 1:
							c += $"NZCV = {GenerateExpression(list[2])};";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			},
			(c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("gpr32"):
							c += $"state.X[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<ulong>) (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("gpr-or-sp32"):
							c += $"if({GenerateExpression(sub[1])} == 31)";
							c++;
							c += $"state.SP = (IRuntimeValue<ulong>) (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							c--;
							c += "else";
							c++;
							c += $"state.X[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<ulong>) (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							c--;
							return;
						case PName("gpr64"):
							c += $"state.X[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<ulong>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("gpr-or-sp64"):
							c += $"if({GenerateExpression(sub[1])} == 31)";
							c++;
							c += $"state.SP = (IRuntimeValue<ulong>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							c--;
							c += "else";
							c++;
							c += $"state.X[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<ulong>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							c--;
							return;
						case PName("vec-b"):
							c += $"state.VB[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<byte>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("vec-h"):
							c += $"state.VH[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<ushort>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("vec-s"):
							c += $"state.VS[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<float>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("vec-d"):
							c += $"state.VD[(int) {GenerateExpression(sub[1])}] = (IRuntimeValue<double>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
						case PName("sr"):
							c += $"SR({GenerateExpression(sub[1])}, {GenerateExpression(sub[2])}, {GenerateExpression(sub[3])}, {GenerateExpression(sub[4])}, {GenerateExpression(sub[5])}, builder.EnsureRuntime({GenerateExpression(list[2])}));";
							return;
						case PName("nzcv") when sub.Count == 1:
							c += $"SetNZCV(state, (IRuntimeValue<ulong>) builder.EnsureRuntime({GenerateExpression(list[2])}));";
							return;
						case PName("nzcv"):
							c += $"{GenerateExpression(list[1], lhs: true)} = (IRuntimeValue<bool>) builder.EnsureRuntime({GenerateExpression(list[2])});";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			}).Interpret((list, state) => {
				var value = state.Evaluate(list[2]);
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("gpr32"):
							var regz32 = state.Evaluate(sub[1]);
							if(regz32 != 31)
								state.Registers[$"X{regz32}"] = (ulong) (uint) value;
							break;
						case PName("gpr-or-sp32"):
							var reg32 = state.Evaluate(sub[1]);
							state.Registers[reg32 == 31 ? "SP" : $"X{reg32}"] = (ulong) (uint) value;
							break;
						case PName("gpr64"):
							var regz64 = state.Evaluate(sub[1]);
							if(regz64 != 31)
								state.Registers[$"X{regz64}"] = (ulong) value;
							break;
						case PName("gpr-or-sp64"):
							var reg64 = state.Evaluate(sub[1]);
							state.Registers[reg64 == 31 ? "SP" : $"X{reg64}"] = (ulong) value;
							break;
						
						case PName("vec"):
							state.Registers[$"V{state.Evaluate(sub[1])}"] = value;
							break;
						case PName("vec-b"):
							state.Registers[$"V{state.Evaluate(sub[1])}"] = new Vector128<byte>((byte) value, single: true);
							break;
						case PName("vec-h"):
							state.Registers[$"V{state.Evaluate(sub[1])}"] = new Vector128<ushort>((ushort) value, single: true);
							break;
						case PName("vec-s"):
							state.Registers[$"V{state.Evaluate(sub[1])}"] = new Vector128<float>((float) value, single: true);
							break;
						case PName("vec-d"):
							state.Registers[$"V{state.Evaluate(sub[1])}"] = new Vector128<double>((double) value, single: true);
							break;
						
						case PName("nzcv"):
							if(sub.Count == 1) {
								state.Registers["NZCV-N"] = (value >> 31) & 1;
								state.Registers["NZCV-Z"] = (value >> 30) & 1;
								state.Registers["NZCV-C"] = (value >> 29) & 1;
								state.Registers["NZCV-V"] = (value >> 28) & 1;
							} else
								switch(sub[1]) {
									case PName("n"):
										state.Registers["NZCV-N"] = Extensions.AsBool(value) ? 1UL : 0UL;
										break;
									case PName("z"):
										state.Registers["NZCV-Z"] = Extensions.AsBool(value) ? 1UL : 0UL;
										break;
									case PName("c"):
										state.Registers["NZCV-C"] = Extensions.AsBool(value) ? 1UL : 0UL;
										break;
									case PName("v"):
										state.Registers["NZCV-V"] = Extensions.AsBool(value) ? 1UL : 0UL;
										break;
									default:
										throw new NotSupportedException();
								}
							break;
						
						case PName("sr"):
							throw new BailoutException();
						
						default:
							throw new NotSupportedException();
					}
				else
					state.Locals[list[1].AsName()] = value;
				return value;
			});
		
		Expression("pc", _ => new EInt(false, 64), _ => "pc").Interpret((_, state) => state.GetRegister("PC"));
			
			Expression("gpr32", _ => new EInt(false, 32).AsRuntime(),
				list => $"({GenerateExpression(list[1])}) == 31 ? 0U : (uint) state->X[(int) {GenerateExpression(list[1])}]",
				list => $"({GenerateExpression(list[1])}) == 31 ? builder.Zero<uint>() : (IRuntimeValue<uint>) (state.X[(int) {GenerateExpression(list[1])}])")
				.Interpret((list, state) => {
					var reg = state.Evaluate(list[1]);
					if(reg == 31)
						return 0U;
					return (uint) state.GetRegister($"X{reg}");
				});
			Expression("gpr-or-sp32", _ => new EInt(false, 32).AsRuntime(),
				list => $"({GenerateExpression(list[1])}) == 31 ? state->SP : (state->X[(int) {GenerateExpression(list[1])}] & 0xFFFFFFFFUL)",
				list => $"({GenerateExpression(list[1])}) == 31 ? state.SP : state.X[(int) {GenerateExpression(list[1])}]")
				.Interpret((list, state) => {
					var reg = state.Evaluate(list[1]);
					return (uint) state.GetRegister(reg == 31 ? "SP" : $"X{reg}");
				});
			Expression("gpr64", _ => new EInt(false, 64).AsRuntime(),
				list => $"({GenerateExpression(list[1])}) == 31 ? 0UL : state->X[(int) {GenerateExpression(list[1])}]",
				list => $"({GenerateExpression(list[1])}) == 31 ? builder.Zero<ulong>() : state.X[(int) {GenerateExpression(list[1])}]")
				.Interpret((list, state) => {
					var reg = state.Evaluate(list[1]);
					if(reg == 31)
						return 0UL;
					return (ulong) state.GetRegister($"X{reg}");
				});
			Expression("gpr-or-sp64", _ => new EInt(false, 64).AsRuntime(),
				list => $"({GenerateExpression(list[1])}) == 31 ? state->SP : state->X[(int) {GenerateExpression(list[1])}]",
				list => $"({GenerateExpression(list[1])}) == 31 ? state.SP : state.X[(int) {GenerateExpression(list[1])}]")
				.Interpret((list, state) => {
					var reg = state.Evaluate(list[1]);
					return (ulong) state.GetRegister(reg == 31 ? "SP" : $"X{reg}");
				});
			
			Expression("vec", _ => EType.Vector.AsRuntime(), 
				list => $"state->V[{GenerateExpression(list[1])}]", 
				list => $"state.V[(int) ({GenerateExpression(list[1])})]")
				.Interpret((list, state) => state.GetRegister($"V{state.Evaluate(list[1])}"));
			Expression("vec-b", _ => new EFloat(8).AsRuntime(),
				list => $"reinterpret_cast<Vector128<uint8_t>>(state->V[{GenerateExpression(list[1])}])[0]",
				list => $"state.VB[(int) ({GenerateExpression(list[1])})]")
				.Interpret((list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<byte>()[0]);
			Expression("vec-h", _ => new EInt(false, 16).AsRuntime(),
				list => $"reinterpret_cast<Vector128<uint16_t>>(state->V[{GenerateExpression(list[1])}])[0]",
				list => $"state.VH[(int) ({GenerateExpression(list[1])})]")
				.Interpret((list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<ushort>()[0]);
			Expression("vec-s", _ => new EFloat(32).AsRuntime(),
				list => $"state->V[{GenerateExpression(list[1])}][0]",
				list => $"state.VS[(int) ({GenerateExpression(list[1])})]")
				.Interpret((list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<float>()[0]);
			Expression("vec-d", _ => new EFloat(64).AsRuntime(),
				list => $"reinterpret_cast<Vector128<double>>(state->V[{GenerateExpression(list[1])}])[0]",
				list => $"state.VD[(int) ({GenerateExpression(list[1])})]")
				.Interpret((list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<double>()[0]);
			
			Expression("nzcv", _ => new EBool().AsRuntime(),
				list => {
					if(list.Count == 1) throw new NotSupportedException();
					switch(list[1]) {
						case PName("n"): return "state->NZCV_N";
						case PName("z"): return "state->NZCV_Z";
						case PName("c"): return "state->NZCV_C";
						case PName("v"): return "state->NZCV_V";
						default: throw new NotSupportedException($"Unknown field of NZCV: {list[1]}");
					}
				}, list => {
					if(list.Count == 1) throw new NotSupportedException();
					switch(list[1]) {
						case PName("n"): return "state.NZCV_N";
						case PName("z"): return "state.NZCV_Z";
						case PName("c"): return "state.NZCV_C";
						case PName("v"): return "state.NZCV_V";
						default: throw new NotSupportedException($"Unknown field of NZCV: {list[1]}");
					}
				}).Interpret((list, state) => {
					if(list.Count == 1) throw new NotSupportedException();
					return list[1] switch {
						PName("n") => Extensions.AsBool(state.GetRegister("NZCV-N")) ? 1 : 0, 
						PName("z") => Extensions.AsBool(state.GetRegister("NZCV-Z")) ? 1 : 0, 
						PName("c") => Extensions.AsBool(state.GetRegister("NZCV-C")) ? 1 : 0, 
						PName("v") => Extensions.AsBool(state.GetRegister("NZCV-V")) ? 1 : 0, 
						_ => throw new NotSupportedException()
					};
				});

				Expression("vector-insert", _ => EType.Unit,
						list => $"reinterpret_cast<Vector128<{GenerateType(list[3].Type)}>*>(&(state->V[(int) ({GenerateExpression(list[1])})]))[0][{GenerateExpression(list[2])}] = {GenerateExpression(list[3])}",
						list => $"state.V[(int) ({GenerateExpression(list[1])})] = state.V[(int) ({GenerateExpression(list[1])})].Element({GenerateExpression(list[2].Cast<int>())}, {GenerateExpression(list[3])})")
					.Interpret((list, state) => {
						var name = $"V{state.Evaluate(list[1])}";
						var vector = state.GetRegister(name).As(list[3].Type).Copy();
						var value = state.Evaluate(list[3]);
						value = list[3].Type switch {
							EInt(false, 8) => (byte) value, 
							EInt(true, 8) => (sbyte) value, 
							EInt(false, 16) => (ushort) value, 
							EInt(true, 16) => (short) value, 
							EInt(false, 32) => (uint) value, 
							EInt(true, 32) => (int) value, 
							EInt(false, 64) => (ulong) value, 
							EInt(true, 64) => (long) value,
							EFloat(32) => (float) value, 
							EFloat(64) => (double) value, 
							_ => throw new NotSupportedException()
						};
						vector[(int) state.Evaluate(list[2])] = value;
						state.Registers[name] = vector;
						return null;
					});
		
			Expression("sr", _ => new EInt(false, 64).AsRuntime(), 
				list => $"SR({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])}, {GenerateExpression(list[5])})", 
				list => $"SR({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])}, {GenerateExpression(list[5])})")
				.NoInterpret();
			
			Expression("float-to-fixed-point", list => TypeFromName(list[2]).AsRuntime(list[1].Type.Runtime || list[3].Type.Runtime), 
				list => $"FloatToFixed{((EInt) list.Type).Width}({GenerateExpression(list[1])}, (int) ({GenerateExpression(list[3])}))", 
				list => $"builder.Call<{GenerateType(list[1].Type.AsCompiletime())}, int, {(((EInt) list.Type).Width == 64 ? "ulong" : "uint")}>(FloatToFixed{((EInt) list.Type).Width}, {GenerateExpression(list[1])}, (IRuntimeValue<int>) builder.EnsureRuntime({GenerateExpression(list[3])}))")
				.Interpret((list, state) => {
					var width = ((EInt) list.Type).Width;
					var swidth = ((EFloat) list[1].Type).Width;
					var fvalue = state.Evaluate(list[1]);
					var fbits = (int) state.Evaluate(list[3]);
					return (width, swidth) switch {
						(32, 32) => (dynamic) unchecked((uint) (int) MathF.Round(fvalue * (1 << fbits))), 
						(64, 32) => (dynamic) unchecked((ulong) (long) MathF.Round(fvalue * (1 << fbits))), 
						(32, 64) => (dynamic) unchecked((uint) (int) Math.Round(fvalue * (1 << fbits))), 
						(64, 64) => (dynamic) unchecked((ulong) (long) Math.Round(fvalue * (1 << fbits))), 
						_ => throw new NotSupportedException()
					};
				});
			
			Expression("make-wmask", _ => new EInt(false, 64),
				list => $"MakeWMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})",
				list => $"MakeWMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})")
				.Interpret((list, state) => MakeWMask((uint) state.Evaluate(list[1]), (uint) state.Evaluate(list[2]), (uint) state.Evaluate(list[3]), (long) state.Evaluate(list[5]), (int) state.Evaluate(list[4])));

			Expression("make-tmask", _ => new EInt(false, 64),
				list => $"MakeTMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})",
				list => $"MakeTMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})")
				.Interpret((list, state) => MakeTMask((uint) state.Evaluate(list[1]), (uint) state.Evaluate(list[2]), (uint) state.Evaluate(list[3]), (long) state.Evaluate(list[5]), (int) state.Evaluate(list[4])));

			Expression("svc", _ => EType.Unit.AsRuntime(),
				list => $"Svc({GenerateExpression(list[1])})",
				list => $"CallSvc({GenerateExpression(list[1])})")
				.NoInterpret();
			
			BranchExpression("branch-linked", _ => EType.Unit.AsRuntime(), list => $"BranchLinked({GenerateExpression(list[1])})")
				.Interpret((list, state) => {
					state.Registers["X30"] = state.GetRegister("PC") + 4;
					return state.Registers["PC"] = state.Evaluate(list[1]);
				});
			BranchExpression("branch-default", _ => EType.Unit.AsRuntime(), list => "Branch(pc + 4)")
				.Interpret((list, state) => state.Registers["PC"] = state.GetRegister("PC") + 4);
			
			Expression("load", list => TypeFromName(list[2]).AsRuntime(),
				list => {
					var type = GenerateType(list.Type);
#if USE_SYSTEM_MEMORY
					if(type == "Vector128<float>")
						return $"LoadVector({GenerateExpression(list[1])})";
					return $"*({type}*) ({GenerateExpression(list[1])})";
#else
					return $"ReadMemory<{type}>({GenerateExpression(list[1])})";
#endif
				},
				list =>
					$"builder.Pointer<{GenerateType(list.Type.AsCompiletime())}>((IRuntimeValue<ulong>) builder.EnsureRuntime({GenerateExpression(list[1])})).Value")
				.Interpret((list, state) => state.GetMemory(state.Evaluate(list[1]), list.Type));

			Expression("load-exclusive", list => TypeFromName(list[2]).AsRuntime(),
				list =>
					$"state->Exclusive{(list.Type is EInt(_, var ewidth) ? ewidth : throw new NotSupportedException())} = *({GenerateType(list.Type)}*) ({GenerateExpression(list[1])})",
				list =>
					$"state.Exclusive{(list.Type is EInt(_, var width) ? width : throw new NotSupportedException())} = builder.Pointer<{GenerateType(list.Type.AsCompiletime())}>({GenerateExpression(list[1])}).Value")
				.NoInterpret(); // TODO: Implement
			
			Expression("store", _ => EType.Unit.AsRuntime(),
				list => {
					var type = GenerateType(list[2].Type);
#if USE_SYSTEM_MEMORY
					if(type == "Vector128<float>")
						return $"StoreVector({GenerateExpression(list[1])}, {GenerateExpression(list[2])})";
					return $"*({GenerateType(list[2].Type)}*) ({GenerateExpression(list[1])}) = {GenerateExpression(list[2])}";
#else
					return $"WriteMemory<{type}>({GenerateExpression(list[1])}, {GenerateExpression(list[2])})";
#endif
				},
				list =>
					$"builder.Pointer<{GenerateType(list[2].Type.AsCompiletime())}>({GenerateExpression(list[1])}).Value = {GenerateExpression(list[2])}")
				.Interpret((list, state) => {
					state.SetMemory(state.Evaluate(list[1]), state.Evaluate(list[2]));
					return null;
				});
			
			Expression("store-exclusive", _ => new EInt(false, 1).AsRuntime(), 
				list => $"CompareAndSwap(({GenerateType(list[2].Type)}*) ({GenerateExpression(list[1])}), {GenerateExpression(list[2])}, state->Exclusive{(list[2].Type is EInt(_, var sewidth) ? sewidth : throw new NotSupportedException())})", 
				list => $"CompareAndSwap<{GenerateType(list[2].Type.AsCompiletime())}>((IRuntimePointer<ulong, {GenerateType(list[2].Type.AsCompiletime())}>) ({GenerateExpression(list[1])}), {GenerateExpression(list[2])}, state.Exclusive{(list[2].Type is EInt(_, var sewidth) ? sewidth : throw new NotSupportedException())})")
				.NoInterpret(); // TODO: Implement
			
			Statement("breakpoint", _ => EUnit.RuntimeType,
				(cb, list) => cb += $"Breakpoint({GenerateExpression(list[1])});",
				(cb, list) => cb += $"builder.CallVoid<uint>(Breakpoint, builder.LiteralValue({GenerateExpression(list[1])}));"
				).NoInterpret(); // TODO: Implement
	}
}