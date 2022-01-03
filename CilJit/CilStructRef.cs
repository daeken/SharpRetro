using JitBase;
using Sigil;

namespace CilJit; 

public class CilStructRef<AddrT, DelegateT, T> : IStructRef<T> where AddrT : struct where DelegateT : Delegate where T : IJitStruct {
	readonly CilBuilder<AddrT, DelegateT> JBuilder;
	readonly Emit<DelegateT> Ilg;
	readonly IRuntimeValue<ulong> Pointer;
	internal CilStructRef(CilBuilder<AddrT, DelegateT> jBuilder, Emit<DelegateT> ilg, IRuntimeValue<ulong> ptr) {
		JBuilder = jBuilder;
		Ilg = ilg;
		Pointer = ptr;
	}
	
	static CilRuntimeValue<U, DelegateT> TT<U>(IRuntimeValue<U> v) where U : struct => v as CilRuntimeValue<U, DelegateT>;

	public override IRuntimeValue<U> GetField<U>(ulong offset) => JBuilder.C<U>(() => {
		TT(Pointer + JBuilder.LiteralValue(offset)).Emit();
		Ilg.Convert<IntPtr>();
		Ilg.LoadIndirect<U>();
	});
	public override void SetField<U>(ulong offset, IRuntimeValue<U> value) {
		TT(Pointer + JBuilder.LiteralValue(offset)).Emit();
		Ilg.Convert<IntPtr>();
		TT(value).Emit();
		Ilg.StoreIndirect<U>();
	}
}