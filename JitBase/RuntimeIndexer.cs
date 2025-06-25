namespace JitBase;

public sealed class RuntimeIndexer<ST, ET>(IStructRef<ST> js, ulong offset) where ST : IJitStruct where ET : struct {
    public IRuntimeValue<ET> this[int index] {
        get => js.GetFieldElement<ET>(offset, index);
        set => js.SetFieldElement(offset, index, value);
    }
    public IRuntimeValue<ET> this[IRuntimeValue<int> index] {
        get => js.GetFieldElement<ET>(offset, index);
        set => js.SetFieldElement(offset, index, value);
    }
}