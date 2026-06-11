using UmbraCore.Core;
namespace UmbraCore.Services.Nns.Nvdrv;

// : nnMain → nv::InitializeGraphics →
// sm::GetServiceHandle("nvdrv") → Initialize. On macOS the design intercepts
// nvn* at the fnptr level via HookManager+NativeLib, so SDK's nvn never talks
// to nvdrv. On Linux v0 with NativeLib skipped, SDK's real nvn runs and hits
// this. Stubbing forward to find the next wall; the proper Route A (intercept
// nvnBootstrapLoader/nvnDeviceGetProcAddress to return managed Vulkan impls)
// makes this whole service unreachable. ‡

public partial class INvDrvServices {
    protected override uint Initialize(uint transferMemorySize, KObject proc, KObject tmem) {
        $"[nvdrv] Initialize(tmemSize={transferMemorySize}) ‡ stubbed → 0".Log();
        return 0;
    }

    protected override uint SetClientPID(ulong _0, ulong _1) {
        $"[nvdrv] SetClientPID({_0}, {_1}) ‡ stubbed → 0".Log();
        return 0;
    }

    readonly Dictionary<uint, string> Fds = new();
    uint NextFd = 1;

    protected override void Open(Span<byte> path, out uint fd, out uint err) {
        var p = System.Text.Encoding.ASCII.GetString(path).TrimEnd('\0');
        fd = NextFd++;
        Fds[fd] = p;
        err = 0;
        $"[nvdrv] Open('{p}') → fd={fd}".Log();
    }

    uint _nvmapHandle = 0x10000;

    protected override void Ioctl(uint fd, uint rq, Span<byte> inb, out uint err, Span<byte> outb) {
        var dev = Fds.GetValueOrDefault(fd, "?");
        // copy-through default
        if(inb.Length > 0 && outb.Length >= inb.Length)
            inb.CopyTo(outb);
        err = 0;
        // discriminator: minimal real-output for nvmap so handle≠0.
        // NVMAP_IOC_CREATE 0xc0080101: in {u32 size} out {u32 size; u32 handle}
        // NVMAP_IOC_ALLOC 0xc0200104: {u32 handle; u32 heapmask; u32 flags;
        // u32 align; u8 kind; u8[7] pad; u64 addr}
        // NVMAP_IOC_GET_ID 0xc008010e: {u32 id_out; u32 handle_in}
        switch(rq) {
            case 0xc0080101 when outb.Length >= 8: // CREATE
                var h = _nvmapHandle++;
                BitConverter.TryWriteBytes(outb[4..], h);
                $"[nvdrv] Ioctl {dev} CREATE size={BitConverter.ToUInt32(inb)} → handle=0x{h:x}".Log();
                return;
            case 0xc0200104: // ALLOC — copy-through; addr in, addr out (game gave us a heap ptr)
                $"[nvdrv] Ioctl {dev} ALLOC handle=0x{BitConverter.ToUInt32(inb):x} addr=0x{BitConverter.ToUInt64(inb[24..]):x}".Log();
                return;
            case 0xc008010e when outb.Length >= 4: // GET_ID
                BitConverter.TryWriteBytes(outb, BitConverter.ToUInt32(inb[4..]));
                return;
        }
        $"[nvdrv] Ioctl(fd={fd} '{dev}', rq=0x{rq:x8}, in={inb.Length}, out={outb.Length}) ‡ copy-through".Log();
    }

    protected override void QueryEvent(uint fd, uint eventId, out uint err, out KObject ev) {
        $"[nvdrv] QueryEvent(fd={fd}, event={eventId}) ‡ → fake event".Log();
        ev = new Event();
        err = 0;
    }

    protected override uint _Close(uint fd) {
        Fds.Remove(fd);
        return 0;
    }
}
