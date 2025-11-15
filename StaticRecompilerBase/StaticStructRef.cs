using System.Numerics;
using System.Runtime.InteropServices;
using JitBase;
using static JitBase.Helpers;

namespace StaticRecompilerBase; 

public class StaticStructRef<AddrT, T> : IStructRef<T> where AddrT : struct, INumber<AddrT> where T : IJitStruct {
	readonly StaticBuilder<AddrT> Builder;
	readonly IRuntimeValue<AddrT> Pointer;
	public StaticStructRef(StaticBuilder<AddrT> builder, IRuntimeValue<AddrT> ptr) {
		Builder = builder;
		Pointer = ptr;
	}
	
	public override IRuntimeValue<U> GetField<U>(ulong offset) => Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset).Cast<AddrT>()).Value;
	public override void SetField<U>(ulong offset, IRuntimeValue<U> value) => Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset).Cast<AddrT>()).Value = value;
	public override IRuntimeValue<U> GetFieldElement<U>(ulong offset, int index) => Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset + (ulong) (index * SizeOf<U>())).Cast<AddrT>()).Value;
	public override IRuntimeValue<U> GetFieldElement<U>(ulong offset, IRuntimeValue<int> index) => throw new NotImplementedException();
	public override void SetFieldElement<U>(ulong offset, int index, IRuntimeValue<U> value) => Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset + (ulong) (index * SizeOf<U>())).Cast<AddrT>()).Value = value;
	public override void SetFieldElement<U>(ulong offset, IRuntimeValue<int> index, IRuntimeValue<U> value) => throw new NotImplementedException();
}