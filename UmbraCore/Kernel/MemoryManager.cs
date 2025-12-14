using System.Runtime.InteropServices;

namespace UmbraCore.Kernel;

public class MemoryManager {
    public ulong Heap;
    public readonly Dictionary<ulong, (ulong Size, ulong Flags)> Regions = [];

    void MapAt(ulong addr, ulong size) => throw new NotImplementedException();
    ulong Alloc(ulong size) => throw new NotImplementedException();
    void Free(ulong addr) => throw new NotImplementedException();

    public static void Mmap(ulong addr, ulong size) {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            if(addr != mmapMac(addr, size, 3, 0x1000 | 0x0010 | 0x0002, -1, 0))
                throw new Exception($"Couldn't allocate memory at 0x{addr:X}-0x{addr + size - 1:X}");
        } else
            throw new NotImplementedException();
    }
    [DllImport("libSystem.dylib", EntryPoint = "mmap")]
    static extern ulong mmapMac(ulong addr, ulong len, int prot, int flags, int fd, ulong offset);
}