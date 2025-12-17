using System.Runtime.InteropServices;

namespace UmbraCore.Kernel;

public class MemoryManager {
    public ulong Heap;
    public readonly Dictionary<ulong, (ulong Size, ulong Flags)> Regions = [];

    IEnumerable<(bool Resident, ulong Start, ulong Size, ulong Flags)> AllRegions() {
        var cur = 0UL;
        foreach(var (start, (size, flags)) in Regions.OrderBy(x => x.Key)) {
            if(cur < start)
                yield return (false, cur, start - cur, 0);
            yield return (true, start, size, flags);
            cur = start + size;
        }
        yield return (false, cur, 0xFFFF_FFFF_FFFF_FFFF - cur + 1, 0);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MemoryInfo {
        public ulong Begin, Size;
        public uint MemoryType, MemoryAttribute;
        public uint Permission, DeviceRefCount, IpcRefCount;
        public uint __padding;
    }
    
    public unsafe void Setup(GameWrapper game) {
        game.Callbacks.QueryMemory = (memoryInfoPtr, addr, ref pageInfo) => {
            Console.WriteLine($"Querying memory for {addr:X}");
            var memoryInfo = (MemoryInfo*) memoryInfoPtr;
            foreach(var (resident, start, size, flags) in AllRegions()) {
                if(start <= addr && addr < start + size) {
                    memoryInfo->Begin = start;
                    memoryInfo->Size = size;
                    memoryInfo->MemoryType = resident ? 3U : 0;
                    memoryInfo->MemoryAttribute = 0;
                    memoryInfo->Permission = resident ? 5U : 0; // RX
                    memoryInfo->DeviceRefCount = memoryInfo->IpcRefCount = memoryInfo->__padding = 0;
                    pageInfo = 0;
                    Console.WriteLine($"Found?? {start:X} - {start + size:X}");
                    break;
                }
            }
            return 0;
        };
    }
    
    void MapAt(ulong addr, ulong size) => throw new NotImplementedException();
    ulong Alloc(ulong size) => throw new NotImplementedException();
    void Free(ulong addr) => throw new NotImplementedException();
    
    public void Mmap(ulong addr, ulong size) {
        Regions[addr] = (size, 0);
        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            if(addr != mmapMac(addr, size, 3, 0x1000 | 0x0010 | 0x0002, -1, 0))
                throw new Exception($"Couldn't allocate memory at 0x{addr:X}-0x{addr + size - 1:X}");
        } else
            throw new NotImplementedException();
    }
    [DllImport("libSystem.dylib", EntryPoint = "mmap")]
    static extern ulong mmapMac(ulong addr, ulong len, int prot, int flags, int fd, ulong offset);
}