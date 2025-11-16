namespace JitBase;

public sealed class RuntimeIndexer<ST, ET>(string name, IStructRef<ST> js, ulong offset) where ST : IJitStruct where ET : struct {
    public IRuntimeValue<ET> this[int index] {
        get => js.GetFieldElement<ET>(name, offset, index);
        set => js.SetFieldElement(name, offset, index, value);
    }
    public IRuntimeValue<ET> this[IRuntimeValue<int> index] {
        get => js.GetFieldElement<ET>(name, offset, index);
        set => js.SetFieldElement(name, offset, index, value);
    }
}