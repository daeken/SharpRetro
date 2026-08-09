using System.Diagnostics;
namespace ArchCompilerCore;

// Extracted {sig, exec} from legacy: CoreArchCompiler/Logic.cs
// Emit-lambdas → Backends/CSharp (rung-2). Local helpers lifted VERBATIM from legacy.
public class Logic : Builtin {
    public override void Define() {
        // local fn from legacy Logic.cs:108 (used only by signext's exec lambda)
        // legacy Logic.cs:108-113 VERBATIM (the freeze-law is byte-identical, not
        // "equivalent under my reasoning" — a composed-from-memory mask-OR form differs
        // at the size==width edge).
        T SignExt<T>(ulong value, int size) {
            if(typeof(T) == typeof(long))
                return (T) (object) ((value & (1UL << (size - 1))) != 0 ? (long) value - (1L << size) : (long) value);
            if(typeof(T) == typeof(int))
                return (T) (object) ((value & (1UL << (size - 1))) != 0 ? (int) value - (1 << size) : (int) value);
            throw new NotImplementedException($"Unknown return for SignExt: {typeof(T)}");
        }
        Stmt("let", list => list.Last().Type.AsRuntime(list[2].Type.Runtime),
            (list, state) => {
			state.Locals[list[1].AsName()] = state.Evaluate(list[2]);
			return state.Evaluate(list.Skip(3));
		});
        Stmt("mlet", list => list.Last().Type.AsRuntime(list.AnyRuntime),
            (list, state) => {
			var assigns = (PList) list[1];
			Debug.Assert(assigns.Count % 2 == 0);
			for(var i = 0; i < assigns.Count; i += 2)
				state.Locals[assigns[i].AsName()] = state.Evaluate(assigns[i + 1]);
			return state.Evaluate(list.Skip(2));
		});
        Expr("ensure-runtime", list => list[1].Type.AsRuntime());
        Expr("cast", list => TypeFromName(list[2]).AsRuntime(list.AnyRuntime),
            (list, state) => TypeFromName(list[2]) switch {
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
        Expr("bitcast", list => TypeFromName(list[2]).AsRuntime(list.AnyRuntime),
            (list, state) => {
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
        Expr("signext", list => TypeFromName(list[2]).AsRuntime(list.AnyRuntime),
            (list, state) =>
				TypeFromName(list[2]) switch {
					EInt(_, 32) => SignExt<int>((ulong) state.Evaluate(list[1]), ((EInt) list[1].Type).Width), 
					EInt(_, 64) => SignExt<long>((ulong) state.Evaluate(list[1]), ((EInt) list[1].Type).Width),
					{} type => throw new NotSupportedException($"SignExt on unsupported type {type}")
				});
        Expr(new[] { "==", "!=", ">", ">=", "<=", "<" }, list => new EBool().AsRuntime(list.AnyRuntime),
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
