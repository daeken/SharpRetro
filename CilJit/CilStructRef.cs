using System.Runtime.InteropServices;
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
	
	public override IRuntimeValue<U> GetField<U>(string name, ulong offset) => JBuilder.C<U>(() => {
		JBuilder.Emit(Pointer + JBuilder.LiteralValue(offset));
		Ilg.Convert<IntPtr>();
		Ilg.LoadIndirect<U>();
	});
	public override void SetField<U>(string name, ulong offset, IRuntimeValue<U> value) {
		JBuilder.Emit(Pointer + JBuilder.LiteralValue(offset));
		Ilg.Convert<IntPtr>();
		JBuilder.Emit(value);
		Ilg.StoreIndirect<U>();
	}
	public override IRuntimeValue<U> GetFieldElement<U>(string name, ulong offset, int index) => JBuilder.C<U>(() => {
		JBuilder.Emit(Pointer + JBuilder.LiteralValue(offset + (ulong) index * (ulong) Marshal.SizeOf<U>()));
		Ilg.Convert<IntPtr>();
		Ilg.LoadIndirect<U>();
	});
	public override IRuntimeValue<U> GetFieldElement<U>(string name, ulong offset, IRuntimeValue<int> index) => JBuilder.C<U>(() => {
		JBuilder.Emit(Pointer + JBuilder.LiteralValue(offset) + (IRuntimeValue<ulong>) index * JBuilder.LiteralValue((ulong) Marshal.SizeOf<U>()));
		Ilg.Convert<IntPtr>();
		Ilg.LoadIndirect<U>();
	});
	public override void SetFieldElement<U>(string name, ulong offset, int index, IRuntimeValue<U> value) {
		JBuilder.Emit(Pointer + JBuilder.LiteralValue(offset + (ulong) index * (ulong) Marshal.SizeOf<U>()));
		Ilg.Convert<IntPtr>();
		JBuilder.Emit(value);
		Ilg.StoreIndirect<U>();
	}
	public override void SetFieldElement<U>(string name, ulong offset, IRuntimeValue<int> index, IRuntimeValue<U> value) {
		JBuilder.Emit(Pointer + JBuilder.LiteralValue(offset) + (IRuntimeValue<ulong>) index * JBuilder.LiteralValue((ulong) Marshal.SizeOf<U>()));
		Ilg.Convert<IntPtr>();
		JBuilder.Emit(value);
		Ilg.StoreIndirect<U>();
	}
}