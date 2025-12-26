using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LibSharpRetro;

public class Blob {
    const int AdditionSize = 0x400000;
    Memory<byte> Data;
    int FurthestTouched = 0;
    public Blob(int initialSize = AdditionSize) => Data = new byte[initialSize];
    
    public Memory<byte> ToMemory() => Data[..FurthestTouched];
    public Span<byte> ToSpan() => ToMemory().Span;
    public byte[] ToArray() => ToMemory().ToArray();

    public Blob Write<T>(int offset, T value) where T : struct {
        var size = Marshal.SizeOf<T>();
        var end = offset + size;
        if(Data.Length < end) {
            var ndata = new byte[end + AdditionSize];
            Data.CopyTo(ndata);
            Data = ndata;
        }

        MemoryMarshal.Cast<byte, T>(Data.Span[offset..end])[0] = value;

        FurthestTouched = Math.Max(end, FurthestTouched);
        return this;
    }

    public Blob Write<T>(int offset, IEnumerable<T> values) where T : struct {
        foreach(var value in values) {
            Write(offset, value);
            offset += Marshal.SizeOf<T>();
        }
        return this;
    }

    public Blob WriteBytes(int offset, string values) {
        Write(offset, Encoding.ASCII.GetBytes(values));
        return this;
    }
}