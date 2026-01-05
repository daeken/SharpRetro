using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibSharpRetro;

namespace UmbraCore.Core;

public class KSharedMemory : KObject {
    readonly int Size;
    byte[] InitialBackingMemory;
    ulong _Address;
    public ulong Address { set => SetAddress(value); }

    unsafe void SetAddress(ulong value) {
        var data = _Address != 0 ? new Span<byte>((byte*) _Address, Size).ToArray() : InitialBackingMemory;
        _Address = value;
        if(_Address == 0) {
            InitialBackingMemory = new byte[Size];
            return;
        }
        if(data != null) {
            var span = new Span<byte>((byte*) _Address, Size);
            for(var i = 0; i < Size; ++i)
                span[i] = data[i];
        }
    }

    public KSharedMemory(int size) {
        Size = size;
        InitialBackingMemory = new byte[size];
    }

    public KSharedMemory(byte[] data) {
        Size = data.Length;
        InitialBackingMemory = data;
    }
}

public class MemoryManager {
    public ulong HeapAddress, HeapSize;
    public readonly Dictionary<ulong, (ulong Size, ulong Flags)> Regions = [];

    public bool IsKnownPointer(ulong addr) =>
        Regions.Any(x => x.Key <= addr && addr < x.Key + x.Value.Size);

    IEnumerable<(bool Resident, ulong Start, ulong Size, ulong Flags)> AllRegions() {
        var cur = 0UL;
        foreach(var (start, (size, flags)) in Regions.OrderBy(x => x.Key)) {
            if(cur < start)
                yield return (false, cur, start - cur, 0);
            yield return (true, start, size, flags);
            cur = start + size;
        }
        yield return (false, cur, 0x0FFF_FFFF_FFFF_FFFF - cur + 1, 0);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MemoryInfo {
        public ulong Begin, Size;
        public uint MemoryType, MemoryAttribute;
        public uint Permission, IpcRefCount, DeviceRefCount;
        public uint __padding;
    }
    
    public unsafe void Setup(GameWrapper game) {
        game.Callbacks.QueryMemory = (memoryInfoPtr, addr, ref pageInfo) => {
            $"Querying memory for {addr:X} -- pointer is {memoryInfoPtr:X}".Log();
            var memoryInfo = (MemoryInfo*) memoryInfoPtr;
            /*foreach(var (start, size, exists, mapped, prot) in MemoryHelpers.GetAllRegions()) {
                if(start <= addr && addr < start + size) {
                    memoryInfo->Begin = start;
                    memoryInfo->Size = size;
                    memoryInfo->MemoryType = exists || start < 0x1_0000_0000 ? 3U : 0;
                    memoryInfo->MemoryAttribute = 0;
                    memoryInfo->Permission = exists ? (uint) prot | 1 : 0;
                    memoryInfo->DeviceRefCount = memoryInfo->IpcRefCount = memoryInfo->__padding = 0;
                    *(uint*) pageInfo = 0;
                    $"Found? {start:X}-{start+size:X} {exists} {prot}".Log();
                    break;
                }
            }*/
            foreach(var (resident, start, size, flags) in AllRegions()) {
                if(start <= addr && addr < start + size) {
                    memoryInfo->Begin = start;
                    memoryInfo->Size = size;
                    memoryInfo->MemoryType = resident ? 3U : 0;
                    memoryInfo->MemoryAttribute = 0;
                    memoryInfo->Permission = resident ? (flags == 1 ? 3U : 5U) : 0; // RX for code, RW for heap
                    memoryInfo->DeviceRefCount = memoryInfo->IpcRefCount = memoryInfo->__padding = 0;
                    *(uint*) pageInfo = 0;
                    break;
                }
            }
            return 0;
        };
        game.Callbacks.SetHeapSize = (size, ref ptr) => {
            Debug.Assert(HeapAddress == 0);
            HeapSize = size;
            $"Heap size: {HeapSize:X}".Log();
            HeapAddress = (ulong) NativeMemory.AlignedAlloc((UIntPtr) HeapSize, 2 * 1024 * 1024);
            $"Allocated heap at 0x{HeapAddress:X}".Log();
            Regions[HeapAddress] = (HeapSize, 1);
            ptr = HeapAddress;
            Unsafe.InitBlockUnaligned((void*) ptr, 0, (uint) HeapSize);
            return 0;
        };
        game.Callbacks.CreateTransferMemory = (addr, size, perm, ref handle) => {
            "Transfer memory 'created'".Log();
            handle = 0xDEADBEEF;
            return 0;
        };
        game.Callbacks.MapSharedMemory = (handle, addr, size, perm) => {
            $"Mapping shared memory handle 0x{handle:X} at 0x{addr:X} (size 0x{size:X})".Log();
            MemoryHelpers.Mmap(addr, size, requirePosition: true);
            Regions[addr] = (size, 1);
            return 0;
        };
        game.Callbacks.UnmapSharedMemory = (handle, addr, size) => {
            $"Unmapping shared memory from handle 0x{handle:X} at 0x{addr:X} with size 0x{size:X}".Log();
            return 0;
        };
        game.Callbacks.MapMemory = (dest, src, size) => {
            $"Attempting to map memory from 0x{src:X} to 0x{dest:X} (size 0x{size:X})".Log();
            var adest = dest;
            while(adest % 16384 != 0) adest--;
            var asize = size + (dest - adest);
            while(asize % 16384 != 0) asize++;
            MemoryHelpers.Mmap(adest, asize, requirePosition: true);
            Unsafe.CopyBlockUnaligned((void*) dest, (void*) src, (uint) size);
            Regions[dest] = (size, 1);
            return 0;
        };
        game.Callbacks.UnmapMemory = (dest, src, size) => {
            $"Unmapping memory from 0x{src:X} to 0x{dest:X} (size 0x{size:X})".Log();
            return 0;
        };
    }
    
    void MapAt(ulong addr, ulong size) => throw new NotImplementedException();
    ulong Alloc(ulong size) => throw new NotImplementedException();
    void Free(ulong addr) => throw new NotImplementedException();
}