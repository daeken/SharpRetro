using System.Reflection.Emit;
using JitBase;
using Sigil;

namespace CilJit; 

public class CilLocalVar<T, DelegateT> : ILocalVar<T> where T : struct where DelegateT : Delegate {
	readonly Emit<DelegateT> Ilg;
	readonly IRuntimeValue<T> Getter;
	readonly Local Local;
	public IRuntimeValue<T> Value {
		get => Getter;
		set {
			((CilRuntimeValue<T, DelegateT>) value).Emit();
			Ilg.StoreLocal(Local);
		}
	}
	
	internal CilLocalVar(Emit<DelegateT> ilg, TypeBuilder tb) {
		Ilg = ilg;
		Local = Ilg.DeclareLocal<T>();
		Getter = new CilRuntimeValue<T, DelegateT>(ilg, tb, () => Ilg.LoadLocal(Local));
	}
}