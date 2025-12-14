using System.Buffers;
using System.Runtime.InteropServices;

namespace LibSharpRetro;

public static class MemoryHelpers {
    unsafe class UnsafeMemoryManager<T>(void* Pointer, int Length) : MemoryManager<T> {
        protected override void Dispose(bool disposing) {}
        public override Span<T> GetSpan() => new(Pointer, Length);
        public override MemoryHandle Pin(int elementIndex = 0) => throw new NotImplementedException();
        public override void Unpin() {}
    }

    public static unsafe Memory<T> AsMemory<T>(void* pointer, int length) =>
        new UnsafeMemoryManager<T>(pointer, length).Memory;
    
    extension(Memory<byte> memory) {
        public T[] Read<T>(uint offset, int count) where T : unmanaged {
            var size = Marshal.SizeOf<T>();
            var ret = new T[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2)[] Read<T1, T2>(uint offset, int count) where T1 : unmanaged where T2 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>();
            var ret = new (T1, T2)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2, T3)[] Read<T1, T2, T3>(uint offset, int count)
            where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>() + Marshal.SizeOf<T3>();
            var ret = new (T1, T2, T3)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2, T3>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2, T3, T4)[] Read<T1, T2, T3, T4>(uint offset, int count) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>() + Marshal.SizeOf<T3>() + Marshal.SizeOf<T4>();
            var ret = new (T1, T2, T3, T4)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2, T3, T4>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2, T3, T4, T5)[] Read<T1, T2, T3, T4, T5>(uint offset, int count) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>() + Marshal.SizeOf<T3>() + Marshal.SizeOf<T4>() +
                       Marshal.SizeOf<T5>();
            var ret = new (T1, T2, T3, T4, T5)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2, T3, T4, T5>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2, T3, T4, T5, T6)[] Read<T1, T2, T3, T4, T5, T6>(uint offset, int count) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>() + Marshal.SizeOf<T3>() + Marshal.SizeOf<T4>() +
                       Marshal.SizeOf<T5>() + Marshal.SizeOf<T6>();
            var ret = new (T1, T2, T3, T4, T5, T6)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2, T3, T4, T5, T6>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2, T3, T4, T5, T6, T7)[] Read<T1, T2, T3, T4, T5, T6, T7>(uint offset, int count)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>() + Marshal.SizeOf<T3>() + Marshal.SizeOf<T4>() +
                       Marshal.SizeOf<T5>() + Marshal.SizeOf<T6>() + Marshal.SizeOf<T7>();
            var ret = new (T1, T2, T3, T4, T5, T6, T7)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2, T3, T4, T5, T6, T7>(offset + (uint) (size * i));
            return ret;
        }

        public (T1, T2, T3, T4, T5, T6, T7, T8)[] Read<T1, T2, T3, T4, T5, T6, T7, T8>(uint offset, int count)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged {
            var size = Marshal.SizeOf<T1>() + Marshal.SizeOf<T2>() + Marshal.SizeOf<T3>() + Marshal.SizeOf<T4>() +
                       Marshal.SizeOf<T5>() + Marshal.SizeOf<T6>() + Marshal.SizeOf<T7>() + Marshal.SizeOf<T8>();
            var ret = new (T1, T2, T3, T4, T5, T6, T7, T8)[count];
            for(var i = 0; i < count; i++)
                ret[i] = memory.Read<T1, T2, T3, T4, T5, T6, T7, T8>(offset + (uint) (size * i));
            return ret;
        }

        public T Read<T>(uint offset) where T : unmanaged {
            var size = Marshal.SizeOf<T>();
            if(size + offset > memory.Length)
                throw new IndexOutOfRangeException(
                    $"Tried to read {size} bytes from offset {offset}; only {memory.Length} bytes available");
            return MemoryMarshal.Cast<byte, T>(memory.Slice((int) offset, size).Span)[0];
        }

        public (T1, T2) Read<T1, T2>(uint offset) where T1 : unmanaged where T2 : unmanaged =>
            (memory.Read<T1>(offset), memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()));

        public (T1, T2, T3) Read<T1, T2, T3>(uint offset)
            where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged =>
        (
            memory.Read<T1>(offset),
            memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()),
            memory.Read<T3>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>())
        );

        public (T1, T2, T3, T4) Read<T1, T2, T3, T4>(uint offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged =>
        (
            memory.Read<T1>(offset),
            memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()),
            memory.Read<T3>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>()),
            memory.Read<T4>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>())
        );

        public (T1, T2, T3, T4, T5) Read<T1, T2, T3, T4, T5>(uint offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged =>
        (
            memory.Read<T1>(offset),
            memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()),
            memory.Read<T3>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>()),
            memory.Read<T4>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>()),
            memory.Read<T5>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>())
        );

        public (T1, T2, T3, T4, T5, T6) Read<T1, T2, T3, T4, T5, T6>(uint offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged =>
        (
            memory.Read<T1>(offset),
            memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()),
            memory.Read<T3>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>()),
            memory.Read<T4>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>()),
            memory.Read<T5>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>()),
            memory.Read<T6>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>() + (uint) Marshal.SizeOf<T5>())
        );

        public (T1, T2, T3, T4, T5, T6, T7) Read<T1, T2, T3, T4, T5, T6, T7>(uint offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged =>
        (
            memory.Read<T1>(offset),
            memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()),
            memory.Read<T3>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>()),
            memory.Read<T4>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>()),
            memory.Read<T5>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>()),
            memory.Read<T6>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>() + (uint) Marshal.SizeOf<T5>()),
            memory.Read<T7>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>() + (uint) Marshal.SizeOf<T5>() +
                            (uint) Marshal.SizeOf<T6>())
        );

        public (T1, T2, T3, T4, T5, T6, T7, T8) Read<T1, T2, T3, T4, T5, T6, T7, T8>(uint offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged =>
        (
            memory.Read<T1>(offset),
            memory.Read<T2>(offset + (uint) Marshal.SizeOf<T1>()),
            memory.Read<T3>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>()),
            memory.Read<T4>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>()),
            memory.Read<T5>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>()),
            memory.Read<T6>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>() + (uint) Marshal.SizeOf<T5>()),
            memory.Read<T7>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>() + (uint) Marshal.SizeOf<T5>() +
                            (uint) Marshal.SizeOf<T6>()),
            memory.Read<T8>(offset + (uint) Marshal.SizeOf<T1>() + (uint) Marshal.SizeOf<T2>() +
                            (uint) Marshal.SizeOf<T3>() + (uint) Marshal.SizeOf<T4>() + (uint) Marshal.SizeOf<T5>() +
                            (uint) Marshal.SizeOf<T6>() + (uint) Marshal.SizeOf<T7>())
        );

        public T Read<T>(ulong offset) where T : unmanaged => memory.Read<T>((uint) offset);

        public (T1, T2) Read<T1, T2>(ulong offset) where T1 : unmanaged where T2 : unmanaged =>
            memory.Read<T1, T2>((uint) offset);

        public (T1, T2, T3) Read<T1, T2, T3>(ulong offset)
            where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged => memory.Read<T1, T2, T3>((uint) offset);

        public (T1, T2, T3, T4) Read<T1, T2, T3, T4>(ulong offset)
            where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged =>
            memory.Read<T1, T2, T3, T4>((uint) offset);

        public (T1, T2, T3, T4, T5) Read<T1, T2, T3, T4, T5>(ulong offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged => memory.Read<T1, T2, T3, T4, T5>((uint) offset);

        public (T1, T2, T3, T4, T5, T6) Read<T1, T2, T3, T4, T5, T6>(ulong offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged => memory.Read<T1, T2, T3, T4, T5, T6>((uint) offset);

        public (T1, T2, T3, T4, T5, T6, T7) Read<T1, T2, T3, T4, T5, T6, T7>(ulong offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged => memory.Read<T1, T2, T3, T4, T5, T6, T7>((uint) offset);

        public (T1, T2, T3, T4, T5, T6, T7, T8) Read<T1, T2, T3, T4, T5, T6, T7, T8>(ulong offset) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged => memory.Read<T1, T2, T3, T4, T5, T6, T7, T8>((uint) offset);

        public T[] Read<T>(ulong offset, int count) where T : unmanaged => memory.Read<T>((uint) offset, count);

        public (T1, T2)[] Read<T1, T2>(ulong offset, int count) where T1 : unmanaged where T2 : unmanaged =>
            memory.Read<T1, T2>((uint) offset, count);

        public (T1, T2, T3)[] Read<T1, T2, T3>(ulong offset, int count)
            where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged =>
            memory.Read<T1, T2, T3>((uint) offset, count);

        public (T1, T2, T3, T4)[] Read<T1, T2, T3, T4>(ulong offset, int count)
            where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged =>
            memory.Read<T1, T2, T3, T4>((uint) offset, count);

        public (T1, T2, T3, T4, T5)[] Read<T1, T2, T3, T4, T5>(ulong offset, int count) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged => memory.Read<T1, T2, T3, T4, T5>((uint) offset, count);

        public (T1, T2, T3, T4, T5, T6)[] Read<T1, T2, T3, T4, T5, T6>(ulong offset, int count) where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged => memory.Read<T1, T2, T3, T4, T5, T6>((uint) offset, count);

        public (T1, T2, T3, T4, T5, T6, T7)[] Read<T1, T2, T3, T4, T5, T6, T7>(ulong offset, int count)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged => memory.Read<T1, T2, T3, T4, T5, T6, T7>((uint) offset, count);

        public (T1, T2, T3, T4, T5, T6, T7, T8)[] Read<T1, T2, T3, T4, T5, T6, T7, T8>(ulong offset, int count)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
            where T8 : unmanaged => memory.Read<T1, T2, T3, T4, T5, T6, T7, T8>((uint) offset, count);
    }
}