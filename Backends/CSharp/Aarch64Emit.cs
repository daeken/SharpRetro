using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Aarch64Common.Common;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy Aarch64Generator/Builtins.cs
public static class Aarch64Emit {
    public static void Register() {
        Statement("=",
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
			});
        Expression("pc",
            _ => "pc");
        Expression("gpr32",
            list => $"({GenerateExpression(list[1])}) == 31 ? 0U : (uint) state->X[(int) {GenerateExpression(list[1])}]",
            list => $"({GenerateExpression(list[1])}) == 31 ? builder.Zero<uint>() : (IRuntimeValue<uint>) (state.X[(int) {GenerateExpression(list[1])}])");
        Expression("gpr-or-sp32",
            list => $"({GenerateExpression(list[1])}) == 31 ? state->SP : (state->X[(int) {GenerateExpression(list[1])}] & 0xFFFFFFFFUL)",
            list => $"({GenerateExpression(list[1])}) == 31 ? state.SP : state.X[(int) {GenerateExpression(list[1])}]");
        Expression("gpr64",
            list => $"({GenerateExpression(list[1])}) == 31 ? 0UL : state->X[(int) {GenerateExpression(list[1])}]",
            list => $"({GenerateExpression(list[1])}) == 31 ? builder.Zero<ulong>() : state.X[(int) {GenerateExpression(list[1])}]");
        Expression("gpr-or-sp64",
            list => $"({GenerateExpression(list[1])}) == 31 ? state->SP : state->X[(int) {GenerateExpression(list[1])}]",
            list => $"({GenerateExpression(list[1])}) == 31 ? state.SP : state.X[(int) {GenerateExpression(list[1])}]");
        Expression("vec",
            list => $"state->V[{GenerateExpression(list[1])}]",
            list => $"state.V[(int) ({GenerateExpression(list[1])})]");
        Expression("vec-b",
            list => $"reinterpret_cast<Vector128<uint8_t>>(state->V[{GenerateExpression(list[1])}])[0]",
            list => $"state.VB[(int) ({GenerateExpression(list[1])})]");
        Expression("vec-h",
            list => $"reinterpret_cast<Vector128<uint16_t>>(state->V[{GenerateExpression(list[1])}])[0]",
            list => $"state.VH[(int) ({GenerateExpression(list[1])})]");
        Expression("vec-s",
            list => $"state->V[{GenerateExpression(list[1])}][0]",
            list => $"state.VS[(int) ({GenerateExpression(list[1])})]");
        Expression("vec-d",
            list => $"reinterpret_cast<Vector128<double>>(state->V[{GenerateExpression(list[1])}])[0]",
            list => $"state.VD[(int) ({GenerateExpression(list[1])})]");
        Expression("nzcv",
            list => {
					if(list.Count == 1) throw new NotSupportedException();
					switch(list[1]) {
						case PName("n"): return "state->NZCV_N";
						case PName("z"): return "state->NZCV_Z";
						case PName("c"): return "state->NZCV_C";
						case PName("v"): return "state->NZCV_V";
						default: throw new NotSupportedException($"Unknown field of NZCV: {list[1]}");
					}
				},
            list => {
					if(list.Count == 1) throw new NotSupportedException();
					switch(list[1]) {
						case PName("n"): return "state.NZCV_N";
						case PName("z"): return "state.NZCV_Z";
						case PName("c"): return "state.NZCV_C";
						case PName("v"): return "state.NZCV_V";
						default: throw new NotSupportedException($"Unknown field of NZCV: {list[1]}");
					}
				});
        Expression("vector-insert",
            list => $"reinterpret_cast<Vector128<{GenerateType(list[3].Type)}>*>(&(state->V[(int) ({GenerateExpression(list[1])})]))[0][{GenerateExpression(list[2])}] = {GenerateExpression(list[3])}",
            list => $"state.V[(int) ({GenerateExpression(list[1])})] = state.V[(int) ({GenerateExpression(list[1])})].Element({GenerateExpression(list[2].Cast<int>())}, {GenerateExpression(list[3])})");
        Expression("sr",
            list => $"SR({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])}, {GenerateExpression(list[5])})",
            list => $"SR({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])}, {GenerateExpression(list[5])})");
        Expression("float-to-fixed-point",
            list => $"FloatToFixed{((EInt) list.Type).Width}({GenerateExpression(list[1])}, (int) ({GenerateExpression(list[3])}))",
            list => $"FloatToFixed{((EInt) list.Type).Width}({GenerateExpression(list[1])}, {GenerateExpression(list[3])})");
        Expression("make-wmask",
            list => $"MakeWMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})",
            list => $"MakeWMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})");
        Expression("make-tmask",
            list => $"MakeTMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})",
            list => $"MakeTMask({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[5])}, {GenerateExpression(list[4])})");
        Expression("svc",
            list => $"Svc({GenerateExpression(list[1])})",
            list => $"CallSvc({GenerateExpression(list[1])})");
        BranchExpression("branch-linked",
            list => $"BranchLinked({GenerateExpression(list[1])})");
        BranchExpression("branch-default",
            list => "Branch(pc + 4)");
        Expression("load",
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
					$"builder.Pointer<{GenerateType(list.Type.AsCompiletime())}>((IRuntimeValue<ulong>) builder.EnsureRuntime({GenerateExpression(list[1])})).Value");
        Expression("load-exclusive",
            list =>
					$"state->Exclusive{(list.Type is EInt(_, var ewidth) ? ewidth : throw new NotSupportedException())} = *({GenerateType(list.Type)}*) ({GenerateExpression(list[1])})",
            list =>
					$"state.Exclusive{(list.Type is EInt(_, var width) ? width : throw new NotSupportedException())} = builder.Pointer<{GenerateType(list.Type.AsCompiletime())}>({GenerateExpression(list[1])}).Value");
 // TODO: Implement
			
        Expression("store",
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
					$"builder.Pointer<{GenerateType(list[2].Type.AsCompiletime())}>({GenerateExpression(list[1])}).Value = {GenerateExpression(list[2])}");
        Expression("store-exclusive",
            list => $"CompareAndSwap(({GenerateType(list[2].Type)}*) ({GenerateExpression(list[1])}), {GenerateExpression(list[2])}, state->Exclusive{(list[2].Type is EInt(_, var sewidth) ? sewidth : throw new NotSupportedException())})",
            list => $"CompareAndSwap<{GenerateType(list[2].Type.AsCompiletime())}>(builder.Pointer<{GenerateType(list[2].Type.AsCompiletime())}>({GenerateExpression(list[1])}), {GenerateExpression(list[2])}, state.Exclusive{(list[2].Type is EInt(_, var sewidth) ? sewidth : throw new NotSupportedException())})");
 // TODO: Implement
			
        Statement("breakpoint",
            (cb, list) => cb += $"Breakpoint({GenerateExpression(list[1])});",
            (cb, list) => cb += $"Breakpoint({GenerateExpression(list[1])});");
 // TODO: Implement
	    }
}
