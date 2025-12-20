using System.Collections;
using System.Runtime.InteropServices;

namespace UmbraCore;

public class Buffer<T>(ulong address, ulong size) : IEnumerable<T>
    where T : struct {
    public readonly ulong Address = address;
    public readonly int Size = (int) size, ElementSize = Marshal.SizeOf<T>();

    public int Length => Size / ElementSize;
    public Buffer<T> End => this + Length;

    public ref T Value => ref Span[0];
    public ref T this[int index] => ref Span[index];

    public Buffer<OtherT> As<OtherT>() where OtherT : struct => new(Address, (ulong) Size);
    public Buffer<OtherT> As<OtherT>(int size) where OtherT : struct => new(Address, (ulong) size);
    public Buffer<OtherT> As<OtherT>(ulong size) where OtherT : struct => new(Address, size);
    public unsafe Span<T> Span => new((void *) Address, Size / ElementSize);
    public static implicit operator Span<T>(Buffer<T> buffer) => buffer.Span;
    public static implicit operator T(Buffer<T> buffer) => buffer.Value;

    public static Buffer<T> operator +(Buffer<T> buffer, int offset) =>
        new(buffer.Address + (ulong) (buffer.ElementSize * offset),
            (ulong) buffer.Size - (ulong) (buffer.ElementSize * offset));

    public static Buffer<T> operator +(Buffer<T> buffer, uint offset) =>
        new(buffer.Address + (ulong) (buffer.ElementSize * offset),
            (ulong) buffer.Size - (ulong) (buffer.ElementSize * offset));

    public static unsafe void ScopedSpan(Span<T> span, Action<Buffer<T>> func) {
        fixed(byte* ptr = MemoryMarshal.Cast<T, byte>(span))
            func(new Buffer<T>((ulong) ptr, (ulong) span.Length));
    }

    public IEnumerator<T> GetEnumerator() {
        var count = Size / Marshal.SizeOf<T>();
        for(var i = 0; i < count; ++i)
            yield return this[i];
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}