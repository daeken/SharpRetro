using System;
using System.Diagnostics;
using System.Linq;
using DoubleSharp.Linq;

namespace CoreArchCompiler; 

public class Logic : Builtin {
	public override void Define() {
		Statement("let", 
			list => list.Last().Type.AsRuntime(list[2].Type.Runtime),
			(c, list) => {
				c += $"var {list[1]} = {GenerateExpression(list[2])};";
				list.Skip(3).ForEach(x => GenerateStatement(c, (PList) x));
			}, (c, list) => {
				if(list[2].Type.Runtime)
					c += $"var {list[1]} = ({GenerateExpression(list[2])}).Store();";
				else
					c += $"var {list[1]} = {GenerateExpression(list[2])};";
				list.Skip(3).ForEach(x => GenerateStatement(c, (PList) x));
			}).Interpret((list, state) => {
			state.Locals[list[1].AsName()] = state.Evaluate(list[2]);
			return state.Evaluate(list.Skip(3));
		});

		Statement("mlet", 
			list => list.Last().Type.AsRuntime(list.AnyRuntime),
			(c, list) => {
				if(list[1] is not PList dlist) throw new NotSupportedException();
				Debug.Assert(dlist.Count % 2 == 0);
				for(var i = 0; i < dlist.Count; i += 2)
					c += $"var {dlist[i]} = {GenerateExpression(dlist[i + 1])};";
				list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
			}, (c, list) => {
				if(list[1] is not PList dlist) throw new NotSupportedException();
				Debug.Assert(dlist.Count % 2 == 0);
				for(var i = 0; i < dlist.Count; i += 2)
					if(dlist[i + 1].Type.Runtime)
						c += $"var {dlist[i]} = ({GenerateExpression(dlist[i + 1])}).Store();";
					else
						c += $"var {dlist[i]} = {GenerateExpression(dlist[i + 1])};";
				list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
			}).Interpret((list, state) => {
			var assigns = (PList) list[1];
			Debug.Assert(assigns.Count % 2 == 0);
			for(var i = 0; i < assigns.Count; i += 2)
				state.Locals[assigns[i].AsName()] = state.Evaluate(assigns[i + 1]);
			return state.Evaluate(list.Skip(2));
		});

		Expression("ensure-runtime", list => list[1].Type.AsRuntime(),
			list => GenerateExpression(list[1]),
			list => $"builder.EnsureRuntime({GenerateExpression(list[1])})")
			.NoInterpret();

		Expression("cast", list => TypeFromName(list[2]).AsRuntime(list.AnyRuntime), 
				list => {
					if(list[1].Type.ToString() == list.Type.ToString()) return GenerateExpression(list[1]);
					if(Core.Context == ContextTypes.Recompiler && list[1].Type.Runtime) {
						if(list.Type is EBool)
							return $"({GenerateExpression(list[1])}) != builder.Zero<{GenerateType(list[1].Type.AsCompiletime())}>()";
						if(list[1].Type is EBool)
							return $"({GenerateType(list.Type)}) builder.Ternary({GenerateExpression(list[1])}, builder.LiteralValue(1U), builder.Zero<uint>())";
					} else {
						if(list.Type is EBool)
							return $"({GenerateExpression(list[1])}) != 0";
						if(list[1].Type is EBool)
							return $"({GenerateType(list.Type)}) (({GenerateExpression(list[1])}) ? 1U : 0U)";
					}
					return $"({GenerateType(list.Type)}) ({GenerateExpression(list[1])})";
				})
			.Interpret((list, state) => TypeFromName(list[2]) switch {
				EInt(true, <= 8) => (dynamic) (sbyte) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(true, <= 16) => (short) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(true, <= 32) => (int) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(true, <= 64) => (long) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(true, <= 128) => (Int128Wrapper) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(false, <= 8) => (byte) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(false, <= 16) => (ushort) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(false, <= 32) => (uint) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(false, <= 64) => (ulong) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EInt(false, <= 128) => (UInt128Wrapper) Extensions.AsNonBool(state.Evaluate(list[1])), 
				EFloat(32) => (float) state.Evaluate(list[1]), 
				EFloat(64) => (double) state.Evaluate(list[1]), 
				{} type => throw new NotSupportedException($"Cannot cast to type {type}")
			});
		Expression("bitcast", list => TypeFromName(list[2]).AsRuntime(list.AnyRuntime), 
				list => $"Math.Bitcast<{GenerateType(list[1].Type)}, {GenerateType(list.Type)}>({GenerateExpression(list[1])})", 
				list => $"({GenerateExpression(list[1])}).Bitcast<{GenerateType(list.Type.AsCompiletime())}>()")
			.Interpret((list, state) => {
				var bytes = ((byte[]) BitConverter.GetBytes(state.Evaluate(list[1]))).Concat(new byte[8]).ToArray();
				return TypeFromName(list[2]) switch {
					EInt(true, 8) => (sbyte) bytes[0], 
					EInt(false, 8) => bytes[0], 
					EInt(true, 16) => BitConverter.ToInt16(bytes), 
					EInt(false, 16) => BitConverter.ToUInt16(bytes), 
					EInt(true, 32) => BitConverter.ToInt32(bytes), 
					EInt(false, 32) => BitConverter.ToUInt32(bytes), 
					EInt(true, 64) => BitConverter.ToInt64(bytes), 
					EInt(false, 64) => BitConverter.ToUInt64(bytes),
					EFloat(32) => BitConverter.ToSingle(bytes), 
					EFloat(64) => BitConverter.ToDouble(bytes), 
					EVector => Vector128<float>.FromBytes(bytes), 
					{} type => throw new NotSupportedException($"Cannot bitcast to type {type}")
				};
			});
			
		T SignExt<T>(ulong value, int size) {
			if(typeof(T) == typeof(long))
				return (T) (object) ((value & (1UL << (size - 1))) != 0 ? (long) value - (1L << size) : (long) value);
			if(typeof(T) == typeof(int))
				return (T) (object) ((value & (1UL << (size - 1))) != 0 ? (int) value - (1 << size) : (int) value);
			throw new NotImplementedException($"Unknown return for SignExt: {typeof(T)}");
		}

		Expression("signext", list => TypeFromName(list[2]).AsRuntime(list.AnyRuntime), 
				list => $"Math.SignExt<{GenerateType(list.Type)}>({GenerateExpression(list[1])}, {((EInt) list[1].Type).Width})", 
				list => $"({GenerateExpression(list[1])}).SignExt<{GenerateType(list.Type.AsCompiletime())}>({((EInt) list[1].Type).Width})")
			.Interpret((list, state) =>
				TypeFromName(list[2]) switch {
					EInt(_, 32) => SignExt<int>((ulong) state.Evaluate(list[1]), ((EInt) list[1].Type).Width), 
					EInt(_, 64) => SignExt<long>((ulong) state.Evaluate(list[1]), ((EInt) list[1].Type).Width),
					{} type => throw new NotSupportedException($"SignExt on unsupported type {type}")
				});

		Expression(new[] { "==", "!=", ">", ">=", "<=", "<" },
				list => new EBool().AsRuntime(list.AnyRuntime),
				list => {
					list = list.HomogeneousRuntime();
					var runtime = list.AnyRuntime;
					var lhs = list[1];
					var rhs = list[2];
					var lhe = GenerateExpression(lhs);
					var rhe = GenerateExpression(rhs);
					if(lhs.Type is EInt(var lsigned, var lsize) && rhs.Type is EInt(var rsigned, var rsize)) {
						if(!lsigned && rsigned) lhe = $"({GenerateType(new EInt(true, lsize).AsRuntime(runtime))}) ({lhe})";
						if(lsigned && !rsigned) rhe = $"({GenerateType(new EInt(true, rsize).AsRuntime(runtime))}) ({rhe})";
						var signed = lsigned || rsigned;
						if(lsize < rsize) lhe = $"({GenerateType(new EInt(signed, rsize).AsRuntime(runtime))}) ({lhe})";
						if(rsize < lsize) rhe = $"({GenerateType(new EInt(signed, lsize).AsRuntime(runtime))}) ({rhe})";
					}
					return $"({lhe}) {list[0]} ({rhe})";
				})
			.Interpret(
				(list, state) =>
					(state.Evaluate(list[1]), state.Evaluate(list[2])).WithCommonType((a, b) =>
						list[0].AsName() switch {
							"==" => a == b, "!=" => a != b, 
							">" => a > b, ">=" => a >= b, 
							"<" => a < b, "<=" => a <= b, 
							_ => throw new BailoutException()
						}));
	}
}