namespace JitBase; 

public interface IRuntimePointer<AddrT, ValueT> where AddrT : struct where ValueT : struct {
	IRuntimeValue<AddrT> Address { get; }
	IRuntimeValue<ValueT> Value { get; set; }
	IRuntimeValue<ValueT> this[long index] { get; set; }
	IRuntimeValue<ValueT> this[IRuntimeValue<long> index] { get; set; }
}