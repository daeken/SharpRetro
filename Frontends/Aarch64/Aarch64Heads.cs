using ArchCompilerCore;
namespace Frontends.Aarch64;

// Per-ISA heads {sig, exec} extracted from legacy Aarch64Generator/Builtins.cs.
// Rung-1b: registered as-is so InferType matches legacy. Rung-2+: these become
// macros-over-primitives (reg/mem/state/branch class) or contract-intrinsics (svc/exclusive)
// per the head-classification census. That transformation happens against
// the rung-1b typed-tree oracle, so any semantic drift is caught at diff-time.
public class Aarch64Heads : Builtin {
    public override void Define() {
        Stmt("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException(),
            (list, state) => {
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
        Expr("pc", _ => new EInt(false, 64),
            (_, state) => state.GetRegister("PC"));
        Expr("gpr32", _ => new EInt(false, 32).AsRuntime(),
            (list, state) => {
					var reg = state.Evaluate(list[1]);
					if(reg == 31)
						return 0U;
					return (uint) state.GetRegister($"X{reg}");
				});
        Expr("gpr-or-sp32", _ => new EInt(false, 32).AsRuntime(),
            (list, state) => {
					var reg = state.Evaluate(list[1]);
					return (uint) state.GetRegister(reg == 31 ? "SP" : $"X{reg}");
				});
        Expr("gpr64", _ => new EInt(false, 64).AsRuntime(),
            (list, state) => {
					var reg = state.Evaluate(list[1]);
					if(reg == 31)
						return 0UL;
					return (ulong) state.GetRegister($"X{reg}");
				});
        Expr("gpr-or-sp64", _ => new EInt(false, 64).AsRuntime(),
            (list, state) => {
					var reg = state.Evaluate(list[1]);
					return (ulong) state.GetRegister(reg == 31 ? "SP" : $"X{reg}");
				});
        Expr("vec", _ => EType.Vector.AsRuntime(),
            (list, state) => state.GetRegister($"V{state.Evaluate(list[1])}"));
        Expr("vec-b", _ => new EFloat(8).AsRuntime(),
            (list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<byte>()[0]);
        Expr("vec-h", _ => new EInt(false, 16).AsRuntime(),
            (list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<ushort>()[0]);
        Expr("vec-s", _ => new EFloat(32).AsRuntime(),
            (list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<float>()[0]);
        Expr("vec-d", _ => new EFloat(64).AsRuntime(),
            (list, state) => state.GetRegister($"V{state.Evaluate(list[1])}").As<double>()[0]);
        Expr("nzcv", _ => new EBool().AsRuntime(),
            (list, state) => {
					if(list.Count == 1) throw new NotSupportedException();
					return list[1] switch {
						PName("n") => Extensions.AsBool(state.GetRegister("NZCV-N")) ? 1 : 0, 
						PName("z") => Extensions.AsBool(state.GetRegister("NZCV-Z")) ? 1 : 0, 
						PName("c") => Extensions.AsBool(state.GetRegister("NZCV-C")) ? 1 : 0, 
						PName("v") => Extensions.AsBool(state.GetRegister("NZCV-V")) ? 1 : 0, 
						_ => throw new NotSupportedException()
					};
				});
        Expr("vector-insert", _ => EType.Unit,
            (list, state) => {
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
        Expr("sr", _ => new EInt(false, 64).AsRuntime());
        Expr("float-to-fixed-point", list => TypeFromName(list[2]).AsRuntime(list[1].Type.Runtime || list[3].Type.Runtime),
            (list, state) => {
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
        // make-wmask/tmask: compiletime-fold-out heads. Exec bodies (MakeWMask/MakeTMask)
        // are runtime helpers in Aarch64Cpu — for rung-1b (InferType only) sig suffices.
        // Rung-2+: these fold out in ArchCompilerCore's compiletime-eval leg (backends never see them).
        Expr("make-wmask", _ => new EInt(false, 64));
        Expr("make-tmask", _ => new EInt(false, 64));
        Expr("svc", _ => EType.Unit.AsRuntime());
        Expr("load", list => TypeFromName(list[2]).AsRuntime(),
            (list, state) => state.GetMemory(state.Evaluate(list[1]), list.Type));
        Expr("load-exclusive", list => TypeFromName(list[2]).AsRuntime());
        Expr("store", _ => EType.Unit.AsRuntime(),
            (list, state) => {
					state.SetMemory(state.Evaluate(list[1]), state.Evaluate(list[2]));
					return null;
				});
        Expr("store-exclusive", _ => new EInt(false, 1).AsRuntime());
        Stmt("breakpoint", _ => EUnit.RuntimeType);
        // BranchExpression variants (extraction regex missed — same as core's `branch`):
        Stmt("branch-linked", _ => EType.Unit.AsRuntime());
        Stmt("branch-default", _ => EType.Unit.AsRuntime());
    }
}
