namespace StaticRecompilerBase;
using JitBase;

public class StaticRuntimePointer<AddrT, ValueT>(StaticBuilder<AddrT> Builder, IRuntimeValue<AddrT> Address) : IRuntimePointer<AddrT, ValueT> where AddrT : struct where ValueT : struct {
    public IRuntimeValue<AddrT> Address { get; } = Address;

    public IRuntimeValue<ValueT> Value {
        get => Builder.Dereference<ValueT>(Address);
        set => Builder.Dereference(Address, value);
    }

    public IRuntimeValue<ValueT> this[long index] {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IRuntimeValue<ValueT> this[IRuntimeValue<long> index] {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
}