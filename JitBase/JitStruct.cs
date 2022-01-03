using Microsoft.CodeAnalysis;

namespace JitBase; 

public interface IJitStruct {}

public abstract class IStructRef<T> where T : IJitStruct {
	public abstract IRuntimeValue<U> GetField<U>(ulong offset) where U : struct;
	public abstract void SetField<U>(ulong offset, IRuntimeValue<U> value) where U : struct;
}
