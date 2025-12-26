namespace UmbraCore.Core;

public class MiscManager {
    public void Setup(GameWrapper game) {
        game.Callbacks.GetInfo = (id1, handle, id2, ref value) => {
            var ret = 0UL;
            $"GetInfo? {id1} {id2}".Log();
            value = (id1, id2) switch {
                (0, 0) => 0xF,
                (1, 0) => 0xFFFFFFFF00000000UL,
                (2, 0) => 0xbb0000000,
                (3, 0) => 0x1000000000,
                (4, 0) => Kernel.MemoryManager.HeapAddress,
                (5, 0) => Kernel.MemoryManager.HeapSize,
                (6, 0) => 0x1_00000000, // Total memory
                (7, 0) => 0x40000000, // Used memory
                (8, 0) => 0,
                (11, 2) => 0xDEADBEEF,
                (11, 3) => 0xCAFEBABE,
                (12, 0) => 0,
                (13, 0) => 1UL << 40,
                (14, 0) => 0xcafeb00000, // stack base
                (15, 0) => 0x100000000, // stack size
                (18, 0) => 0x10000,
                (11, _) => 0,
                _ => ret = 0xF001,
            };
            if(ret == 0xF001)
                $"Unsupported GetInfo: {id1} {id2} 0x{handle:X}".Log();
            return ret;
        };
    }
}