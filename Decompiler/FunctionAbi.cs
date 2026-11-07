namespace SharpRetro.Decompiler;

public static class FunctionAbi {
    public static bool Windows(BinaryImage image) => image.Format==BinaryFormat.Pe;
    public static int[] Arguments(BinaryImage image) => Windows(image) ? [1,2,8,9] : [7,6,2,1,8,9];
    public static IReadOnlySet<ValueSlot> Preserved(BinaryImage image) =>
        (Windows(image) ? new[] {3,5,6,7,12,13,14,15} : new[] {3,5,12,13,14,15})
        .Select(i=>new ValueSlot(SlotKind.Local,i)).ToHashSet();
    public static IReadOnlySet<ValueSlot> ImportWrites(BinaryImage image) => ImportWrites(Windows(image));
    public static IReadOnlySet<ValueSlot> ImportWrites(bool windows) => !windows ? FunctionRecovery.ImportWrites
        .Concat(Enumerable.Range(0,16).Select(i=>new ValueSlot(SlotKind.Vector,i))).ToHashSet()
        : Enumerable.Range(0,16).Where(i=>i is not (3 or 5 or 6 or 7 or 12 or 13 or 14 or 15))
            .Select(i=>new ValueSlot(SlotKind.Local,i))
            .Concat(Enumerable.Range(0,6).Select(i=>new ValueSlot(SlotKind.Vector,i)))
            .Concat(Enumerable.Range(0,64).SelectMany(i=>new[] {new ValueSlot(SlotKind.Flag,i),new ValueSlot(SlotKind.DefinedFlag,i)})).ToHashSet();
}
