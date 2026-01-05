using System.Buffers;
using System.Runtime.InteropServices;

namespace LibSharpRetro;

public static class MemoryHelpers {
    const string Printable = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-[]{}`~!@#$%^&*()-=\\|;:'\",./<>?";
    public static void Hexdump(this Span<byte> _buffer) {
        var buffer = _buffer.ToArray();
        for(var i = 0; i < buffer.Length; i += 16) {
            Console.Write($"{i:X4} | ");
            for(var j = 0; j < 16; ++j) {
                Console.Write(i + j >= buffer.Length ? $"   " : $"{buffer[i + j]:X2} ");
                if(j == 7) Console.Write(" ");
            }
            Console.Write("| ");
            for(var j = 0; j < 16; ++j) {
                if(i + j >= buffer.Length) break;
                Console.Write(Printable.Contains((char) buffer[i + j]) ? new string((char) buffer[i + j], 1) : ".");
                if(j == 7) Console.Write(" ");
            }
            Console.WriteLine();
        }
        Console.WriteLine($"{buffer.Length:X4}");
    }
    
    public static ulong Mmap(ulong addr, ulong size, bool requirePosition = false) {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            var maddr = mmapMac(addr, size, 3, 
                0x1000 | (requirePosition ? 0x0010 : 0) | 0x0002, 
                -1, 0);
            if(requirePosition && addr != maddr)
                throw new Exception($"Couldn't allocate memory at 0x{addr:X}-0x{addr + size - 1:X}");
            return maddr;
        }
        throw new NotImplementedException();
    }
    [DllImport("libSystem.dylib", EntryPoint = "mmap")]
    static extern ulong mmapMac(ulong addr, ulong len, int prot, int flags, int fd, ulong offset);
    
    [StructLayout(LayoutKind.Sequential)]
    struct vm_region_basic_info_data_64_t {
        public int protection;
        public int max_protection;
        public int inheritance;
        public int shared;
        public int reserved;
        public ulong offset;
        public int behavior;
        public ushort user_wired_count;
    }
    
    [DllImport("libSystem.B.dylib")]
    static extern int mach_vm_region(
        uint target_task,
        ref ulong address,
        out ulong size,
        int flavor,
        out vm_region_basic_info_data_64_t info,
        ref uint infoCnt,
        ref uint object_name);

    static bool TryQueryRegion(uint mach_task_self, ulong addr, out (ulong Start, ulong Size, int Protection) res) {
        var infoCnt = 10U;
        var objName = 0U;
        var kr = mach_vm_region(mach_task_self, ref addr, out var size, 9, out var info, ref infoCnt, ref objName);
        res = (addr, size, info.protection);
        return kr == 0;
    }

    public static IEnumerable<(ulong Start, ulong Size, bool Exists, bool Mapped, int Protection)> GetAllRegions() {
        var lib = NativeLibrary.Load("libSystem.B.dylib");
        uint mach_task_self;
        unsafe { mach_task_self = *(uint*) NativeLibrary.GetExport(lib, "mach_task_self_"); }
        var addr = 0UL;
        while(addr < 0x8000_0000_0000_0000) {
            if(!TryQueryRegion(mach_task_self, addr, out var res)) break;
            var (start, size, prot) = res;
            if(start > addr) yield return (addr, start - addr, false, false, 0);
            var mprot = 0;
            if((prot & 1) != 0) mprot |= 1;
            if((prot & 2) != 0) mprot |= 4;
            if((prot & 4) != 0) mprot |= 2;
            yield return (start, size, true, true, mprot);
            addr = start + size;
        }
        yield return (addr, 0xFFFF_FFFF_FFFF_FFFFUL - addr + 1, false, false, 0);
    }
    
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