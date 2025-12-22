namespace UmbraCore;

public static class Extensions {
    public static byte[] ToArray(this Action<BinaryWriter> func) {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        func(bw);
        return ms.ToArray();
    }
}