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
	
	public override IRuntimeValue<U> GetField<U>(string name, ulong offset) =>
		name == null
			? Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset).Cast<AddrT>()).Value
			: Builder.GetField<U>(Pointer, name);
	public override void SetField<U>(string name, ulong offset, IRuntimeValue<U> value) {
		if(name == null)
			Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset).Cast<AddrT>()).Value = value;
		else
			Builder.SetField(Pointer, name, value);
	}

	public override IRuntimeValue<U> GetFieldElement<U>(string name, ulong offset, int index) =>
		name == null
			? Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset + (ulong) (index * SizeOf<U>())).Cast<AddrT>()).Value
			: Builder.GetFieldIndex<U>(Pointer, name, index);
	public override IRuntimeValue<U> GetFieldElement<U>(string name, ulong offset, IRuntimeValue<int> index) => throw new NotImplementedException();

	public override void SetFieldElement<U>(string name, ulong offset, int index, IRuntimeValue<U> value) {
		if(name == null)
			Builder.Pointer<U>(Pointer + Builder.LiteralValue(offset + (ulong) (index * SizeOf<U>())).Cast<AddrT>()).Value = value;
		else
			Builder.SetFieldIndex(Pointer, name, index, value);
	}
	public override void SetFieldElement<U>(string name, ulong offset, IRuntimeValue<int> index, IRuntimeValue<U> value) => throw new NotImplementedException();
}