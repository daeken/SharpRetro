namespace JitBase; 

public interface IJitStruct {}

public abstract class IStructRef<T> where T : IJitStruct {
	public abstract IRuntimeValue<U> GetField<U>(ulong offset) where U : struct;
	public abstract void SetField<U>(ulong offset, IRuntimeValue<U> value) where U : struct;
	public abstract IRuntimeValue<U> GetFieldElement<U>(ulong offset, IRuntimeValue<int> index) where U : struct;
	public abstract void SetFieldElement<U>(ulong offset, IRuntimeValue<int> index, IRuntimeValue<U> value) where U : struct;
}
