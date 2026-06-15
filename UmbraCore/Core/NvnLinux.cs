using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UmbraCore.Core;

// NVN behavioral stubs for Linux Route A — replaces HookManager.Linux.cs's
// universal-return-1 with per-name dispatch + per-handle state.
// : nvnBufferMap return=1 used as memcpy dest in
// NuShaderManagerGen::Init (uploading numtx_identity, 64 bytes). The
// nvn-contract sweep had nvnBufferMap as top-used (13× legoworlds, 311×
// botw), all ret-mov (return → arg to next call). Pointer-returning fns
// are where universal-1 first bites.
// Design (v0, the simplest correct thing per "recklessly forward"):
// - NVN handles are game-allocated opaque memory. We don't allocate them;
// the game passes them as X0.
// - State lives in a Dict<handlePtr, NvnState> keyed by the game's pointer.
// Builder→Object: nvnXInitialize(obj, builder) copies builder's state to
// obj's entry.
// - MemoryPool: game gives us CPU-side storage via SetStorage(builder,
// ptr, size); CPU_CACHED pools = that ptr IS the mapped base. nvnMemory
// PoolMap returns it directly.
// - Buffer: SetStorage(builder, pool, offset, size). Map = pool.cpuPtr +
// offset. GetAddress = a fake GPU addr (we hand out monotonic 4K-aligned
// tokens; game does arithmetic on them, eventually passes them back to
// nvnCommandBuffer* — wall-N when that needs reverse-mapping).
// - Everything else falls through to universal-1 until the next wall.
// Per-name dispatch: nvnDeviceGetProcAddress returns a UCO thunk per name
// from a static table. Each thunk has the C-ABI signature from nvn.h.
// Names not in the table → universal-1 stub (logged once per name).
// Concurrency: game is multithreaded (9 threads at ). State dict =
// ConcurrentDictionary; per-handle state = mutable struct, races on
// individual fields are tolerated (Builder→Initialize is single-threaded
// per the game's own usage; Map reads are pure).

public static unsafe class NvnLinux {

    // ─── per-handle state ───
    // One bag per opaque handle (builder OR object — Initialize copies).
    public class H {
        public ulong CpuPtr, Size;          // MemoryPool: SetStorage ptr/size
        public ulong PoolPtr, Offset;       // Buffer: SetStorage(pool,off,size)
        public ulong GpuAddr;               // Buffer/Pool: assigned at Initialize
        public int Flags;                   // MemoryPool flags
        public ulong Device;                // back-ref
        public int Width, Height, Format;   // Texture
        public int ShIdx, SphType;          // Program: which sh{idx}-t{sph}.bin
        //  decoded RGBA8 bytes (lazy; populated by
        // DecodeForUpload on first EnsureTexBound). null = not
        // decoded (or undecodable format).
        public byte[]? Rgba;
        // (c²³)(a) Gen: bumped on TexInitialize + Copy
        // BufferToTexture's Rgba-stash. NvnVulkan
        // EnsureTexBound caches by texId; Gen-mismatch
        // ⟹ rebuild (game re-init'd / re-uploaded).
        public int Gen;
    }
    //  decode this texture's CpuPtr data → RGBA8 bytes.
    // Lazy; called from EnsureTexBound on first draw using this
    // texId. Caches in H.Rgba. Returns null if Format unhandled
    // or CpuPtr=0. ⚠️ CpuPtr is set at TextureInitialize from
    // (pool.CpuPtr + builder.Offset); the texture data may not
    // be PRESENT yet (= game WriteTexels/CopyBufferToTexture
    // happens later). v2 = decode-at-first-draw assumes data
    // is there by then (= true for menu's atlas; ‡ for streamed
    // textures may capture stale/partial). v3 = re-decode on
    // CopyBufferToTexture targeting this tex.
    public static byte[]? DecodeForUpload(H ts) {
        if(ts.Rgba != null) return ts.Rgba;
        if(ts.CpuPtr == 0 || ts.Width == 0 || ts.Height == 0)
            return null;
        var src = (byte*)ts.CpuPtr;
        var w = ts.Width; var h = ts.Height;
        var fi = Fmt(ts.Format);
        byte[]? rgba = null;
        if(fi.IsBc) {
            rgba = fi.FourCC switch {
                "DXT5" => DecodeBc3(src, w, h),
                "DXT1" => DecodeBc1(src, w, h),
                // BC2/4/5 = ‡ not seen in title yet; add when hit.
                _      => null,
            };
        } else if(fi.Bpp == 4) {
            // RGBA8 (or SRGB) — passthrough copy. ‡ Block-linear
            // tiling NOT handled (= would need swizzle); v2 reads
            // linear. Lavapipe samples LINEAR fine; visually-wrong
            // if game uploaded block-linear. Defer to v3.
            rgba = new byte[w * h * 4];
            new ReadOnlySpan<byte>(src, w*h*4).CopyTo(rgba);
        } else {
            // R8/RG8/depth etc — broadcast to RGBA so the bind at
            // least doesn't crash. ‡ Visually wrong.
            rgba = new byte[w * h * 4];
            for(int i = 0; i < w*h; i++) {
                byte v = src[i * Math.Max(fi.Bpp, 1)];
                rgba[i*4+0]=v; rgba[i*4+1]=v; rgba[i*4+2]=v; rgba[i*4+3]=255;
            }
        }
        ts.Rgba = rgba;
        if(rgba != null && _decN++ < 30)
            $"[nvn] DecodeForUpload {w}×{h} fmt=0x{ts.Format:x}({fi.FourCC ?? $"{fi.Bpp}Bpp"}) cpu=0x{ts.CpuPtr:x} → {rgba.Length}B".Log();
        return rgba;
    }
    static int _decN;
    // BC1 decode (8B/4×4 block: 2× RGB565 endpoints + 16× 2-bit
    // index). When c0>c1: idx 2,3 = 1/3,2/3 lerp; else idx 2 =
    // 1/2 lerp, idx 3 = transparent black.
    static byte[] DecodeBc1(byte* src, int w, int h) {
        var rgba = new byte[w * h * 4];
        var bw = (w + 3) / 4; var bh = (h + 3) / 4;
        Span<(byte r,byte g,byte b,byte a)> c = stackalloc (byte,byte,byte,byte)[4];
        for(var by = 0; by < bh; by++)
        for(var bx = 0; bx < bw; bx++) {
            var blk = src + (by * bw + bx) * 8;
            ushort c0 = (ushort)(blk[0] | blk[1]<<8);
            ushort c1 = (ushort)(blk[2] | blk[3]<<8);
            static (byte,byte,byte) Rgb565(ushort v) => (
                (byte)(((v>>11)&31)*255/31),
                (byte)(((v>>5)&63)*255/63),
                (byte)((v&31)*255/31));
            var (r0,g0,b0)=Rgb565(c0); var (r1,g1,b1)=Rgb565(c1);
            c[0]=(r0,g0,b0,255); c[1]=(r1,g1,b1,255);
            if(c0>c1) {
                c[2]=((byte)((2*r0+r1)/3),(byte)((2*g0+g1)/3),(byte)((2*b0+b1)/3),255);
                c[3]=((byte)((r0+2*r1)/3),(byte)((g0+2*g1)/3),(byte)((b0+2*b1)/3),255);
            } else {
                c[2]=((byte)((r0+r1)/2),(byte)((g0+g1)/2),(byte)((b0+b1)/2),255);
                c[3]=(0,0,0,0);
            }
            uint idx = blk[4] | (uint)blk[5]<<8 | (uint)blk[6]<<16 | (uint)blk[7]<<24;
            for(var py=0; py<4 && by*4+py<h; py++)
            for(var px=0; px<4 && bx*4+px<w; px++) {
                var ci=c[(int)((idx>>((py*4+px)*2))&3)];
                var o=((by*4+py)*w + bx*4+px)*4;
                rgba[o]=ci.r; rgba[o+1]=ci.g; rgba[o+2]=ci.b; rgba[o+3]=ci.a;
            }
        }
        return rgba;
    }
    // GPU-addr → CPU-ptr reverse map (per pool). v0 = linear search; one
    // pool currently. Returns CPU ptr or 0 if no pool contains gpuAddr.
    static ulong GpuToCpu(ulong gpuAddr) {
        foreach(var (_, h) in St)
            if(h.GpuAddr != 0 && h.CpuPtr != 0 &&
               gpuAddr >= h.GpuAddr && gpuAddr < h.GpuAddr + h.Size)
                return h.CpuPtr + (gpuAddr - h.GpuAddr);
        return 0;
    }
    static readonly System.Collections.Concurrent.ConcurrentDictionary<ulong, H> St = new();
    static H S(ulong h) => St.GetOrAdd(h, _ => new());
    // (T1')×3 NvnReplay inject: populate St + TexByPoolIdx
    // from a captured frame's manifest.texPool, so ResolveTex
    // works without a running game. Synthetic ulong-handle =
    // 0xCAFE0000_0000_0000 | texId (won't collide with real
    // game ptrs which are 0xfff5_…).
    public static void InjectTex(int texId, H h) {
        var key = 0xCAFE_0000_0000_0000UL | (uint)texId;
        St[key] = h;
        TexByPoolIdx[texId] = key;
    }

    // ─── per-name table ───
    static readonly bool _vattrHook =
        Environment.GetEnvironmentVariable("UMBRA_VATTR_HOOK") != null;

    public static readonly Dictionary<string, nint> Table = new() {
        // — Device / DeviceBuilder — most return void or NVNboolean; 1 is fine
        // except DeviceGetInteger writes an out-param the game reads.
        ["nvnDeviceInitialize"] = (nint)(delegate* unmanaged<ulong, ulong, int>)&DeviceInitialize,
        ["nvnDeviceGetInteger"] = (nint)(delegate* unmanaged<ulong, int, int*, void>)&DeviceGetInteger,
        // t=11 worker polls this ~1000×/min; stub returning constant
        // 1 → any (now-start < X) loop is infinite. Sig per nvn.h: takes a
        // counter value (NVNcounterData), returns ns. Game probably calls
        // GetCurrentTimestamp first then converts; for poll loops a monotonic
        // value is enough. ‡ Verify which sig variant later.
        ["nvnDeviceGetTimestampInNanoseconds"] = (nint)(delegate* unmanaged<ulong, ulong*, ulong>)&DeviceGetTimestampNs,
        ["nvnDeviceGetCurrentTimestampInNanoseconds"] = (nint)(delegate* unmanaged<ulong, ulong>)&DeviceGetCurrentTimestampNs,

        // — MemoryPool / MemoryPoolBuilder
        ["nvnMemoryPoolBuilderSetStorage"] = (nint)(delegate* unmanaged<ulong, void*, ulong, void>)&MpbSetStorage,
        ["nvnMemoryPoolBuilderSetFlags"]   = (nint)(delegate* unmanaged<ulong, int, void>)&MpbSetFlags,
        ["nvnMemoryPoolInitialize"]        = (nint)(delegate* unmanaged<ulong, ulong, int>)&MpInitialize,
        ["nvnMemoryPoolMap"]               = (nint)(delegate* unmanaged<ulong, void*>)&MpMap,
        ["nvnMemoryPoolGetBufferAddress"]  = (nint)(delegate* unmanaged<ulong, ulong>)&MpGetBufferAddress,
        ["nvnMemoryPoolGetFlags"]          = (nint)(delegate* unmanaged<ulong, int>)&MpGetFlags,
        ["nvnMemoryPoolGetSize"]           = (nint)(delegate* unmanaged<ulong, ulong>)&MpGetSize,

        // — Buffer / BufferBuilder
        ["nvnBufferBuilderSetStorage"]     = (nint)(delegate* unmanaged<ulong, ulong, ulong, ulong, void>)&BbSetStorage,
        ["nvnBufferInitialize"]            = (nint)(delegate* unmanaged<ulong, ulong, int>)&BufInitialize,
        ["nvnBufferMap"]                   = (nint)(delegate* unmanaged<ulong, void*>)&BufMap,
        ["nvnBufferGetAddress"]            = (nint)(delegate* unmanaged<ulong, ulong>)&BufGetAddress,
        ["nvnBufferGetSize"]               = (nint)(delegate* unmanaged<ulong, ulong>)&BufGetSize,

        // — CommandBuffer — v0.5: capture ClearColor for VkImage clear.
        // The game records into NVNcommandBuffer; we don't replay the full
        // command stream yet (that's the real CommandBuffer→VkCmdBuf chapter),
        // just snapshot the most recent clear color so Present can use it.
        ["nvnCommandBufferClearColor"]     = (nint)(delegate* unmanaged<ulong, int, float*, int, void>)&CbClearColor,
        ["nvnCommandBufferSetRenderTargets"] = (nint)(delegate* unmanaged<ulong, int, ulong*, ulong*, ulong, ulong, void>)&CbSetRenderTargets,
        // T1 ("is there a way I can see that texture? :D"):
        // dump the source buffer of CopyBufferToTexture + record texture
        // builder state so we know the dest format/dims.
        ["nvnCommandBufferCopyBufferToTexture"] = (nint)(delegate* unmanaged<ulong, ulong, ulong, ulong, ulong*, int, void>)&CbCopyBufferToTexture,
        // T2 (sera "fucked up geometry"): capture the draw chain.
        // BindVertexBuffer(cb, index, NVNbufferAddress addr, size_t size)
        // DrawArrays(cb, NVNdrawPrimitive prim, int first, int count)
        // BindTexture(cb, NVNshaderStage stage, int slot, NVNtextureHandle h)
        // GetTextureHandle(dev, int texId, int samplerId) → NVNtextureHandle
        ["nvnCommandBufferBindVertexBuffer"] = (nint)(delegate* unmanaged<ulong, int, ulong, ulong, void>)&CbBindVertexBuffer,
        ["nvnCommandBufferBindVertexAttribState"] = (nint)(delegate* unmanaged<ulong, int, void*, void>)&CbBindVertexAttribState,
        ["nvnCommandBufferBindVertexStreamState"] = (nint)(delegate* unmanaged<ulong, int, void*, void>)&CbBindVertexStreamState,
        // state-builder fns. NVN struct = 4B (CONFIRMED via
        // a test run raw-dump). v2 packs {off:16,fmt:8,streamIdx:8} into
        // p[0]. Gated under UMBRA_VATTR_HOOK while testing whether
        // writing into the game's struct (vs no-op stub) changes
        // splash→load behavior (= a test run/a test run stuck-at-1-draw vs
        // pre-a test run/a test run reached title).
        ["nvnVertexAttribStateSetDefaults"]   = _vattrHook ? (nint)(delegate* unmanaged<int*, void>)&VasSetDefaults : 0,
        ["nvnVertexAttribStateSetFormat"]     = _vattrHook ? (nint)(delegate* unmanaged<int*, int, long, void>)&VasSetFormat : 0,
        ["nvnVertexAttribStateSetStreamIndex"]= _vattrHook ? (nint)(delegate* unmanaged<int*, int, void>)&VasSetStreamIndex : 0,
        ["nvnVertexStreamStateSetDefaults"]   = _vattrHook ? (nint)(delegate* unmanaged<int*, void>)&VssSetDefaults : 0,
        ["nvnVertexStreamStateSetStride"]     = _vattrHook ? (nint)(delegate* unmanaged<int*, int, void>)&VssSetStride : 0,
        ["nvnVertexStreamStateSetDivisor"]    = _vattrHook ? (nint)(delegate* unmanaged<int*, int, void>)&VssSetDivisor : 0,
        ["nvnCommandBufferBindUniformBuffer"]= (nint)(delegate* unmanaged<ulong, int, int, ulong, ulong, void>)&CbBindUniformBuffer,
        ["nvnCommandBufferSetViewport"]      = (nint)(delegate* unmanaged<ulong, int, int, int, int, void>)&CbSetViewport,
        ["nvnCommandBufferSetScissor"]       = (nint)(delegate* unmanaged<ulong, int, int, int, int, void>)&CbSetScissor,
        ["nvnCommandBufferDrawArrays"]       = (nint)(delegate* unmanaged<ulong, int, int, int, void>)&CbDrawArrays,
        // (c²⁹) ReportCounter(cb, NVNcounterType, bufGpuAddr).
        // Type 0 = TIMESTAMP. Game does ~50/frame to a ring-of-9
        // bufs; stub'd ⟹ bufs stay 0 ⟹ Δt=0 ⟹ cutscene frozen
        // (c[3] camera + c[6] model both byte-identical across
        // ~3200 frames per c29 dump). Game reads back via
        // DeviceGetTimestampInNanoseconds (derefs buf+8) AND/OR
        // direct CPU-side deref (the bufs are CPU-mappable).
        ["nvnCommandBufferReportCounter"] = (nint)(delegate* unmanaged<ulong, int, ulong, void>)&CbReportCounter,
        // (c²⁷) DrawElementsBaseVertex(cb, prim, idxType, count,
        // idxGpuAddr, baseVtx). NVN idxType 0=u8 1=u16 2=u32.
        ["nvnCommandBufferDrawElementsBaseVertex"] = (nint)(delegate* unmanaged<ulong, int, int, int, ulong, int, void>)&CbDrawElementsBaseVertex,
        ["nvnCommandBufferBindTexture"]      = (nint)(delegate* unmanaged<ulong, int, int, ulong, void>)&CbBindTexture,
        ["nvnCommandBufferBindProgram"]      = (nint)(delegate* unmanaged<ulong, ulong, int, void>)&CbBindProgram,
        ["nvnDeviceGetTextureHandle"]        = (nint)(delegate* unmanaged<ulong, int, int, ulong>)&DeviceGetTextureHandle,
        // T3 ("real shader compilation 👀"): capture
        // what nvnProgramSetShaders actually receives. NVNshaderData
        // struct body unknown (her nvn.h has it empty); per drop-to-bytes dump
        // the bytes + reverse-map any GPU addrs found, THEN reference
        // yuzu/ryujinx for format understanding (clean-room,
        // reference-only, zero code reuse).
        ["nvnProgramSetShaders"]             = (nint)(delegate* unmanaged<ulong, int, ulong*, int>)&ProgramSetShaders,
        ["nvnTextureBuilderSetSize2D"]     = (nint)(delegate* unmanaged<ulong, int, int, void>)&TbSetSize2D,
        ["nvnTextureBuilderSetWidth"]      = (nint)(delegate* unmanaged<ulong, int, void>)&TbSetWidth,
        ["nvnTextureBuilderSetHeight"]     = (nint)(delegate* unmanaged<ulong, int, void>)&TbSetHeight,
        ["nvnTextureBuilderSetFormat"]     = (nint)(delegate* unmanaged<ulong, int, void>)&TbSetFormat,
        ["nvnTextureBuilderSetStorage"]    = (nint)(delegate* unmanaged<ulong, ulong, ulong, void>)&TbSetStorage,
        // ‡ TbGetStorageSize/Alignment NOT in Table — return-1 stub WORKS
        // (umbra54: 501 game-loop ticks); my "real" w×h×bpp values made
        // the game's pool allocator BRK-assert in NuRenderTargets (
        // regression, reverted at ×3 cycles). The game's allocator
        // tolerates size=1 (likely uses GetPaddedTextureLayout instead, or
        // the engine has its own block-linear size calc). Real values need
        // actual NVN block-linear gob math = future wall; not blocking
        // T1 dump.
        ["nvnTextureInitialize"]           = (nint)(delegate* unmanaged<ulong, ulong, int>)&TexInitialize,
        ["nvnTexturePoolRegisterTexture"]  = (nint)(delegate* unmanaged<ulong, int, ulong, ulong, void>)&TpRegTexture,
        ["nvnTextureGetFormat"]            = (nint)(delegate* unmanaged<ulong, int>)&TexGetFormat,
        ["nvnTextureGetWidth"]             = (nint)(delegate* unmanaged<ulong, int>)&TexGetWidth,
        ["nvnTextureGetHeight"]            = (nint)(delegate* unmanaged<ulong, int>)&TexGetHeight,

        // — Window / Queue — the v0 Vulkan wiring points.
        ["nvnWindowAcquireTexture"]        = (nint)(delegate* unmanaged<ulong, ulong, int*, int>)&WindowAcquireTexture,
        ["nvnQueueAcquireTexture"]         = (nint)(delegate* unmanaged<ulong, ulong, int*, int>)&QueueAcquireTexture,
        ["nvnQueuePresentTexture"]         = (nint)(delegate* unmanaged<ulong, ulong, int, void>)&QueuePresentTexture,
    };

    // ─── Device ───
    // Values from sera's NativeLib/nvnDevice.mm getIntegerWrapper (each
    // was a wall on her macOS run; default __builtin_trap's). Pname enum
    // = NativeLib/nvn.h NVNdeviceInfo.
    // ‡‡ : porting her real values
    // CHANGED behavior — game stalls at NuDeferredFilter_LoadLightAttenuation
    // Texture (8 frames vs 26 with my v0 ‡-guesses; verified at 120s NOT a
    // duration confound). Bisect: 0xf=4096→32 alone didn't move it, so it's
    // some other pname OR the timing-shift exposes a SyncManager race
    // doesn't fully close. STOPPED per (×5 cycles on a regression-from-
    // correctness-fix). KEPT her values (correct > working-by-luck per spec
    // ; the deadlock is real and exposed-by-correctness, like was).
    // The NVN contract values that DIFFER from my v0: 0xf=4096→32 (Texture
    // DescriptorSize), 0x10=1→32 (SamplerDescriptorSize), 0x18=32→256
    // (ShaderScratchMemoryScaleFactorRecommended). Game runtime hits 0xf
    // ×2 + 0x10 ×1, doesn't hit 0x18.
    // ‡ TODO: read Atmosphere's WaitProcessWideKeyAtomic + Signal
    // for the actual condvar protocol (via Atmosphere src); may
    // be wrong-shape fix.
    [UnmanagedCallersOnly]
    static void DeviceGetInteger(ulong dev, int pname, int* v) {
        if(v == null) return;
        *v = pname switch {
            0x00 => 0x35,       // ApiMajorVersion (SMO needs this)
            0x01 => 0x0d,       // ApiMinorVersion (SMO needs ≥0xD)
            0x04 => 256,        // UniformBufferAlignment
            0x0f => 32,         // TextureDescriptorSize
            0x10 => 32,         // SamplerDescriptorSize
            0x11 => 256,        // ReservedTextureDescriptors
            0x12 => 256,        // ReservedSamplerDescriptors
            0x13 => 4,          // CommandBufferCommandAlignment
            0x14 => 8,          // CommandBufferControlAlignment
            0x18 => 256,        // ShaderScratchMemoryScaleFactorRecommended
            0x19 => 16384,      // ShaderScratchMemoryAlignment
            0x1a => 131072,     // ShaderScratchMemoryGranularity
            0x24 => 1048576,    // MaxTexturePoolSize
            0x25 => 4096,       // MaxSamplerPoolSize
            0x2e or 0x2f => 1,  // GlslcMax/MinSupportedGpuCodeMajorVersion
            0x30 => 14,         // GlslcMaxSupportedGpuCodeMinorVersion
            0x31 => 5,          // GlslcMinSupportedGpuCodeMinorVersion
            0x38 => 32,         // LinearTextureStrideAlignment
            0x39 => 128,        // LinearRenderTargetStrideAlignment
            0x52 => 65536,      // QueueCommandMemoryDefaultSize
            0x55 => 262144,     // QueueComputeMemoryDefaultSize
            0x5c => 16384,      // QueueControlMemoryDefaultSize
            // Supports* bools (from her list + the contiguous 0x42-0x4C run):
            0x28 or 0x29 or 0x2a or 0x32 or 0x3b or
            (>= 0x42 and <= 0x4c) or 0x57 or 0x5a => 1,
            // ‡ Not in her list — log once, return a guess by name-shape.
            // (her side traps; here we keep going + log = future-wall list)
            _ => LogUnknownPname(pname),
        };
    }

    [UnmanagedCallersOnly]
    static int DeviceInitialize(ulong dev, ulong builder) {
        S(dev).Device = dev;
        $"[nvn] DeviceInitialize dev=0x{dev:x}".Log();
        // v0 Vulkan wiring: bring up lavapipe here. If it fails, nvn keeps
        // working (state-tracking + log-only); Present becomes a no-op.
        NvnVulkan.Init();
        return 1;
    }

    // ─── CommandBuffer state capture (v0.5) ───
    // Most recent ClearColor — game calls it once per render-target per
    // frame. v0.5 = "show me the loading-screen background color"; v1 =
    // proper per-cmdBuf command stream → vkCmd* replay at QueueSubmit.
    public static readonly float[] LastClearColor = { 0, 0, 0, 1 };
    static int _clearN;

    [UnmanagedCallersOnly]
    static void CbClearColor(ulong cb, int index, float* color, int mask) {
        if(color != null) {
            LastClearColor[0] = color[0]; LastClearColor[1] = color[1];
            LastClearColor[2] = color[2]; LastClearColor[3] = color[3];
        }
        var n = ++_clearN;
        if(n <= 5)
            $"[nvn] ClearColor #{n} idx={index} mask=0x{mask:x} rgba=({LastClearColor[0]:f3},{LastClearColor[1]:f3},{LastClearColor[2]:f3},{LastClearColor[3]:f3})".Log();
    }

    // ─── Texture (T1: track builder state, dump CopyBufferToTexture src) ───
    [UnmanagedCallersOnly] static void TbSetSize2D(ulong b, int w, int h) {
        var s = S(b); s.Width = w; s.Height = h;
    }
    [UnmanagedCallersOnly] static void TbSetWidth(ulong b, int w) => S(b).Width = w;
    [UnmanagedCallersOnly] static void TbSetHeight(ulong b, int h) => S(b).Height = h;
    [UnmanagedCallersOnly] static void TbSetFormat(ulong b, int fmt) => S(b).Format = fmt;
    [UnmanagedCallersOnly] static void TbSetStorage(ulong b, ulong pool, ulong off) {
        var s = S(b); s.PoolPtr = pool; s.Offset = off;
    }
    // ‡ NVNformat → (bytesPerPixel, isBC, FourCC). nvn.h doesn't have the
    // enum body (sera's is `typedef int NVNformat`). Values inferred from
    // observed game usage + pool-offset-delta arithmetic:
    // 0x25: 3× 1920×1080 framebuffers, delta 0x7e9000 ≈ 8MB = 4Bpp = RGBA8
    // 0x44: 2048×2048, delta 0x401000 ≈ 4MB = 1Bpp = BC3/DXT5 (16B/4×4)
    // 0x42: ‡ adjacent to 0x44 in NVN enums → BC1/DXT1 (8B/4×4 = 0.5Bpp)
    // 0x29: ‡ near 0x25 → SRGB RGBA8? 0x11/0x12: ‡ R8/RG8? 0x34/0x35: depth?
    // BC formats: dump as .dds (any viewer opens it) + BC3-decode → PPM.
    record FmtInfo(int Bpp, bool IsBc, int BcBlockBytes, string FourCC);
    static FmtInfo Fmt(int fmt) => fmt switch {
        0x42 => new(0, true, 8,  "DXT1"),   // ‡ BC1
        0x43 => new(0, true, 16, "DXT3"),   // ‡ BC2
        0x44 => new(0, true, 16, "DXT5"),   // BC3 (verified via 1Bpp delta)
        0x45 => new(0, true, 8,  "BC4U"),   // ‡ BC4
        0x46 or 0x47 => new(0, true, 16, "BC5U"),  // ‡
        0x11 => new(1, false, 0, ""),       // ‡ R8
        0x12 => new(2, false, 0, ""),       // ‡ RG8
        0x25 or 0x29 => new(4, false, 0, ""),  // RGBA8 / SRGB
        _ => new(4, false, 0, ""),
    };
    static int Bpp(int fmt) => Fmt(fmt).Bpp;
    [UnmanagedCallersOnly] static ulong TbGetStorageSize(ulong b) {
        var s = S(b);
        // ‡ v0: width*height*bpp, no mips/arrays/blocklinear-padding. Game
        // asked GetStorageSize → my stub returned 1 → game allocated 1 byte;
        // need a real-ish answer. Round up to 4K.
        var sz = (ulong)(Math.Max(s.Width, 1) * Math.Max(s.Height, 1) * Bpp(s.Format));
        return (sz + 0xfff) & ~0xffful;
    }
    [UnmanagedCallersOnly] static ulong TbGetStorageAlign(ulong b) => 4096;
    [UnmanagedCallersOnly] static int TexInitialize(ulong tex, ulong builder) {
        var bs = S(builder); var ts = S(tex);
        ts.Width = bs.Width; ts.Height = bs.Height; ts.Format = bs.Format;
        ts.PoolPtr = bs.PoolPtr; ts.Offset = bs.Offset;
        var pool = St.TryGetValue(bs.PoolPtr, out var ps) ? ps : null;
        ts.CpuPtr = pool != null ? pool.CpuPtr + bs.Offset : 0;
        ts.GpuAddr = pool != null ? pool.GpuAddr + bs.Offset : 0;
        ts.Rgba = null;  // (c²³)(a) re-init ⟹ stale decode
        ts.Gen++;
        $"[nvn] TextureInitialize tex=0x{tex:x} {ts.Width}×{ts.Height} fmt=0x{ts.Format:x} pool=0x{ts.PoolPtr:x}+0x{ts.Offset:x} cpu=0x{ts.CpuPtr:x}".Log();
        return 1;
    }
    [UnmanagedCallersOnly] static int TexGetFormat(ulong t) => S(t).Format;
    [UnmanagedCallersOnly] static int TexGetWidth(ulong t) => S(t).Width;
    [UnmanagedCallersOnly] static int TexGetHeight(ulong t) => S(t).Height;

    // Atlas hand-off to NvnVulkan (T2 v0.5). Set on first BC3
    // CopyBufferToTexture; NvnVulkan reads it lazily on the first
    // RecordDrawPass that has draws (= after the atlas upload happened).
    // ‡ v0.5 = ONE atlas (the font); v1 = per-texture-handle map.
    public static byte[]? AtlasRgba; public static int AtlasW, AtlasH;

    // TexturePool index → NVN texture handle. nvnCommandBuffer
    // BindTexture passes a packed handle = (samplerId<<32)|texId;
    // texId is the index passed to nvnTexturePoolRegisterTexture.
    // This map resolves DrawRecord.TexHandle's texId → the actual
    // NVN texture handle whose St[tex] has W/H/Fmt/CpuPtr.
    // ⚠️ Game re-registers same idx with different tex handles
    // (= virtual texturing / streaming); latest-wins. Snapshot
    // resolved tex-handle at draw-time, not idx.
    public static readonly System.Collections.Concurrent
        .ConcurrentDictionary<int, ulong> TexByPoolIdx = new();
    [UnmanagedCallersOnly]
    static void TpRegTexture(ulong pool, int idx, ulong tex, ulong view) {
        var first = !TexByPoolIdx.ContainsKey(idx);
        TexByPoolIdx[idx] = tex;
        if(first) {  // log first registration per-idx (= the game
                     // re-registers 0x100 every frame; we want the
                     // ONCE-each map for the title's ~30 textures)
            var ts = St.GetValueOrDefault(tex);
            $"[nvn] TexturePoolRegister idx=0x{idx:x} tex=0x{tex:x} view=0x{view:x} → {ts?.Width ?? 0}×{ts?.Height ?? 0} fmt=0x{ts?.Format ?? 0:x} cpu=0x{ts?.CpuPtr ?? 0:x}".Log();
        }
    }
    // Resolve a TexHandle (= packed (samplerId<<32)|texId per
    // nvnDeviceGetTextureHandle convention) to the texture's state
    // for upload. Returns null if texId not registered or tex not
    // in St (= TextureInitialize never fired for it).
    public static H? ResolveTex(ulong texHandle) {
        // TexHandle = (texId << 32) | samplerId. Verified at-data:
        // menu's tx10100000120 → texId=0x101 = the 2048×2048 BC3
        // atlas (= what AtlasRgba already holds + renders 90788nz
        // correctly). Title's tx118-11a…185 = texId 0x118-11a all
        // sampler 0x185 (= different textures, same sampler ✓).
        // ‡ nvnDeviceGetTextureHandle isn't hooked; this layout is
        // inferred from the registered-pool-idx ↔ draw-tx match.
        var texId = (int)(texHandle >> 32);
        if(!TexByPoolIdx.TryGetValue(texId, out var tex)) return null;
        return St.GetValueOrDefault(tex);
    }

    static int _copyBufN;
    [UnmanagedCallersOnly]
    static void CbCopyBufferToTexture(ulong cb, ulong srcGpuAddr, ulong dstTex,
            ulong dstView, ulong* region, int copyFlags) {
        // Sig per nvn.h: (cmdBuf, NVNbufferAddress src, NVNtexture* dst,
        // NVNtextureView* dstView, NVNcopyRegion* region, NVNcopyFlags flags).
        // ‡ NVNcopyRegion struct body unknown (sera's nvn.h has it empty);
        // assume {x,y,z, w,h,d} 6×u32 from the typical NVN/Vulkan shape.
        var n = ++_copyBufN;
        var srcCpu = GpuToCpu(srcGpuAddr);
        var dst = St.TryGetValue(dstTex, out var ds) ? ds : null;
        int rw = 0, rh = 0;
        if(region != null) {
            var r = (uint*) region;
            // ‡ guess: r[0..2]=xoff/yoff/zoff, r[3..5]=w/h/d
            rw = (int) r[3]; rh = (int) r[4];
        }
        var w = rw > 0 ? rw : (dst?.Width ?? 0);
        var h = rh > 0 ? rh : (dst?.Height ?? 0);
        var fmt = dst?.Format ?? 0;
        $"[nvn] CopyBufferToTexture #{n} srcGpu=0x{srcGpuAddr:x} srcCpu=0x{srcCpu:x} → tex=0x{dstTex:x} ({w}×{h} fmt=0x{fmt:x}) region=({(region!=null?$"{((uint*)region)[0]},{((uint*)region)[1]},{((uint*)region)[2]} {rw}×{rh}×{((uint*)region)[5]}":"null")})".Log();

        //  decode SRC → S(dstTex).Rgba NOW. The texture's
        // own CpuPtr (= pool storage) is the DEST and never
        // written (= we're the stub); src is the staging buffer
        // the game just filled. Decode while src is fresh; stash
        // in the dest-texture's H so EnsureTexBound finds it.
        // ⚠️ region.{x,y,z}off ≠ 0 = partial update (= mip levels:
        // CopyBufferToTexture #3-#10 all → SAME tex 0xfff557aaafa8
        // with shrinking w/h = mip chain). v2 keeps mip 0 only
        // (= region full-size at xoff=0). Also: same-tex re-copy
        // = REPLACE Rgba (= streaming).
        if(srcCpu != 0 && dst != null && w == dst.Width && h == dst.Height
           && (region == null || (((uint*)region)[0]|((uint*)region)[1]|((uint*)region)[2]) == 0)) {
            var fi = Fmt(fmt);
            try {
                dst.Rgba = fi.IsBc
                    ? fi.FourCC switch {
                        "DXT5" => DecodeBc3((byte*)srcCpu, w, h),
                        "DXT1" => DecodeBc1((byte*)srcCpu, w, h),
                        _      => null }
                    : fi.Bpp == 4
                        ? new ReadOnlySpan<byte>((byte*)srcCpu, w*h*4).ToArray()
                        : null;
                if(dst.Rgba != null) dst.Gen++;  // (c²³)(a)
                if(dst.Rgba != null && n <= 30)
                    $"[nvn]   → S(tex).Rgba stashed {w}×{h} {(fi.IsBc?fi.FourCC:$"{fi.Bpp}Bpp")} ({dst.Rgba.Length}B)".Log();
            } catch(Exception e) {
                $"[nvn]   ‡ per-tex stash failed: {e.Message}".Log();
            }
        }

        // T1 dump: write the raw source buffer so sera can SEE it.
        // BC-compressed → .dds (any viewer) + BC3-decode → .ppm here.
        // Uncompressed → .ppm directly. ‡ Linear layout assumed (it's a
        // CPU-side staging buffer the game memcpy'd into via nvnBufferMap;
        // staging data is linear even if dest texture is block-linear).
        if(srcCpu != 0 && w > 0 && h > 0 && n <= 10) {
            try {
                DumpTexture(n, (byte*) srcCpu, w, h, fmt);
                NvnVulkan.StageTexture(srcCpu, w, h, Math.Max(Bpp(fmt), 4));
            } catch(Exception e) {
                $"[nvn]   ‡ dump failed: {e.Message}".Log();
            }
        }
    }

    static void DumpTexture(int n, byte* src, int w, int h, int fmt) {
        var fi = Fmt(fmt);
        if(fi.IsBc) {
            // .dds: 128B header + raw blocks. Viewers handle DXT1/3/5 directly.
            var bw = (w + 3) / 4; var bh = (h + 3) / 4;
            var dataSize = bw * bh * fi.BcBlockBytes;
            var ddsPath = $"/tmp/umbra-copybuf-{n:d2}.dds";
            using(var fs = File.Create(ddsPath)) {
                var hdr = new byte[128];
                void U32(int off, uint v) { hdr[off]=(byte)v; hdr[off+1]=(byte)(v>>8); hdr[off+2]=(byte)(v>>16); hdr[off+3]=(byte)(v>>24); }
                U32(0, 0x20534444);                 // 'DDS '
                U32(4, 124);                        // header size
                U32(8, 0x1007);                     // CAPS|HEIGHT|WIDTH|PIXELFORMAT
                U32(12, (uint) h); U32(16, (uint) w);
                U32(20, (uint)(bw * fi.BcBlockBytes));  // pitch (= one block-row bytes)
                U32(76, 32);                        // pixelformat size
                U32(80, 0x4);                       // DDPF_FOURCC
                var fcc = System.Text.Encoding.ASCII.GetBytes(fi.FourCC);
                Array.Copy(fcc, 0, hdr, 84, 4);
                U32(108, 0x1000);                   // DDSCAPS_TEXTURE
                fs.Write(hdr);
                fs.Write(new ReadOnlySpan<byte>(src, dataSize));
            }
            $"[nvn]   → {ddsPath} ({w}×{h} {fi.FourCC}, {dataSize} bytes raw BC)".Log();
            // + BC3-decode → PPM so I can verify here without a viewer.
            // Also stash as the atlas for NvnVulkan v0.5 (first BC3 only).
            if(fi.FourCC == "DXT5") {
                var rgba = DecodeBc3(src, w, h);
                if(AtlasRgba == null) {
                    AtlasRgba = rgba; AtlasW = w; AtlasH = h;
                    $"[nvn]   → AtlasRgba stashed for vk sampler ({w}×{h})".Log();
                }
                DumpDecodedPpm(n, rgba, w, h, "-bc3");
            }
        } else {
            var path = $"/tmp/umbra-copybuf-{n:d2}.ppm";
            using var fs = File.Create(path);
            fs.Write(System.Text.Encoding.ASCII.GetBytes($"P6\n{w} {h}\n255\n"));
            var bpp = Math.Max(fi.Bpp, 1);
            var row = new byte[w * 3];
            for(var y = 0; y < h; y++) {
                for(var x = 0; x < w; x++) {
                    var p = src + (y*w + x)*bpp;
                    row[x*3+0] = p[0];
                    row[x*3+1] = bpp >= 2 ? p[1] : p[0];
                    row[x*3+2] = bpp >= 3 ? p[2] : p[0];
                }
                fs.Write(row);
            }
            $"[nvn]   → {path} ({w}×{h} fmt=0x{fmt:x} bpp={bpp})".Log();
        }
    }

    // BC3/DXT5 decode → RGBA (one block = 8B alpha + 8B color; 4×4 px).
    // ‡ v0.5: alpha block decoded too (the font glyphs use it for
    // anti-aliased coverage; my prior decode dropped it).
    public static byte[] DecodeBc3(byte* src, int w, int h) {
        var bw = (w + 3) / 4; var bh = (h + 3) / 4;
        var rgba = new byte[w * h * 4];
        Span<byte> ap = stackalloc byte[8];
        for(var by = 0; by < bh; by++) for(var bx = 0; bx < bw; bx++) {
            var blk = src + (by * bw + bx) * 16;
            // Alpha block @0: a0,a1 endpoints + 16× 3-bit idx (48 bits).
            byte a0 = blk[0], a1 = blk[1];
            ulong abits = blk[2] | (ulong)blk[3]<<8 | (ulong)blk[4]<<16 |
                (ulong)blk[5]<<24 | (ulong)blk[6]<<32 | (ulong)blk[7]<<40;
            ap[0]=a0; ap[1]=a1;
            if(a0 > a1) for(var i=0;i<6;i++) ap[2+i]=(byte)(((6-i)*a0+(1+i)*a1)/7);
            else { for(var i=0;i<4;i++) ap[2+i]=(byte)(((4-i)*a0+(1+i)*a1)/5); ap[6]=0; ap[7]=255; }
            // Color block @8: c0,c1 (RGB565 LE), then 16× 2-bit idx.
            ushort c0 = (ushort)(blk[8] | blk[9]<<8), c1 = (ushort)(blk[10] | blk[11]<<8);
            uint cidx = (uint)(blk[12] | blk[13]<<8 | blk[14]<<16 | blk[15]<<24);
            (int,int,int) F(ushort c)=>((c>>11)*255/31,((c>>5)&63)*255/63,(c&31)*255/31);
            var p0=F(c0); var p1=F(c1);
            var pal = new (int r,int g,int b)[]{p0,p1,
                ((2*p0.Item1+p1.Item1)/3,(2*p0.Item2+p1.Item2)/3,(2*p0.Item3+p1.Item3)/3),
                ((p0.Item1+2*p1.Item1)/3,(p0.Item2+2*p1.Item2)/3,(p0.Item3+2*p1.Item3)/3)};
            for(var pi = 0; pi < 16; pi++) {
                int px=pi&3, py=pi>>2, x=bx*4+px, y=by*4+py;
                if(x>=w||y>=h) continue;
                var ci=(int)((cidx>>(pi*2))&3);
                var ai=(int)((abits>>(pi*3))&7);
                var (r,g,b)=pal[ci];
                var o=(y*w+x)*4;
                rgba[o]=(byte)r; rgba[o+1]=(byte)g; rgba[o+2]=(byte)b; rgba[o+3]=ap[ai];
            }
        }
        return rgba;
    }

    static void DumpDecodedPpm(int n, byte[] rgba, int w, int h, string suffix) {
        var path = $"/tmp/umbra-copybuf-{n:d2}{suffix}.ppm";
        using var fs = File.Create(path);
        fs.Write(System.Text.Encoding.ASCII.GetBytes($"P6\n{w} {h}\n255\n"));
        var rgb = new byte[w*3];
        for(var y=0;y<h;y++){
            for(var x=0;x<w;x++){rgb[x*3]=rgba[(y*w+x)*4];rgb[x*3+1]=rgba[(y*w+x)*4+1];rgb[x*3+2]=rgba[(y*w+x)*4+2];}
            fs.Write(rgb);
        }
        $"[nvn]   → {path} (BC3-decoded RGBA→PPM)".Log();
    }

    // ─── T2 draw chain capture ───
    // Per-cmdbuf "current bind state" + a flat draw log. Each DrawArrays
    // snapshots the bound state at that moment. v0 = enough to dump the
    // vertex data + know prim/count; v1 = replay as vkCmd* at QueueSubmit.
    public class DrawRecord {
        public int N;                    // draw#
        public int Prim, First, Count;
        public ulong VbGpu, VbCpu, VbSize;
        // (T6)×17 stream-1 vbuf (binding=1). 0 if single-
        // stream. NvnVulkan binds both to _t3Vbuf at
        // separate offsets; T3Pipe declares 2 bindings
        // when d.Streams.Length>1.
        public ulong VbGpu1, VbCpu1, VbSize1;
        public int VbIndex;
        public ulong Program;
        public ulong TexHandle; public int TexStage, TexSlot;
        // (c⁴⁰)(F) FS-stage per-slot tex handles snapshot at
        // draw-time. [40] = NVN tex slots 0..39 for stage=1
        // (FRAGMENT). Null for non-indexed draws (= 2D-screen
        // path doesn't need it; menu binds 1 atlas).
        public ulong[]? TexHandles;
        // v1 disc: per-draw UBO snapshot (game may CPU-write the same
        // bound buffer between draws via mapped ptr; snapshotting AT
        // DrawArrays catches the value the game intended for THIS draw).
        public byte[]? Ubo;              // slot-2 only (T2 fixed path)
        // T3: per-(stage, slot) UBO snapshots at draw time.
        // Ubos[stage*8 + slot]. NvnVulkan binds [0*8+K] → set0/K+3
        // (VS), [1*8+K] → set2/K+3 (FS). NVN cbufs are per-stage
        // CONFIRMED at umbra72 (stage=0 slot=2 = 64B model mat4 vs
        // stage=1 slot=2 = 16B α-threshold, different gpuAddrs).
        public byte[][] Ubos;            // [16] = 2 stages × 8 slots
        // Which sh{idx}-t{sph} the bound VS+FS programs map to (via
        // _stagePr[0/1] → S(prog).ShIdx). For per-program pipelines.
        public int VsShIdx, FsShIdx;
        // (c²⁷) Indexed draw (DrawElementsBaseVertex). When
        // IdxCount>0: use vkCmdDrawIndexed with IdxCpu/IdxType/
        // BaseVtx; Count/First unused. NVN idxType 0=u8 1=u16
        // 2=u32. The game's 3D-cutscene geometry is ALL indexed
        // (~36K calls/cutscene, prim=4 idxType=1 count~810-2280).
        public ulong IdxCpu; public int IdxType, IdxCount, BaseVtx;
        public int VpX, VpY, VpW, VpH;   // viewport at draw time
        public int ScX, ScY, ScW, ScH;   // scissor at draw time
        // per-draw vertex layout. Snapshot of the game's
        // NvnVertexAttribState[] / NvnVertexStreamState[] at
        // DrawArrays time (= the format/stride/offset per-attr
        // that T3Pipe needs to build VkVertexInputState).
        public NvnAttrib[] Attribs;
        public NvnStream[] Streams;
        // resolved texture state (W/H/Fmt/CpuPtr) at draw
        // time. Null if TexHandle's texId not registered.
        public H? Tex;
    }

    // NVN VertexAttribState: built by game via SetDefaults/
    // SetFormat(p,fmt,off)/SetStreamIndex(p,idx). Layout per
    // observed call args (SetFormat x1=fmt x2=offset; SetStream
    // Index x1=idx) + NVN convention = packed {fmt:u32,off:u32,
    // streamIdx:u32} or similar. ‡ Reading the struct DIRECTLY
    // at BindVertexAttribState time (= game has already filled
    // it via the Set* fns we don't hook).
    // ‡‡ Layout is INFERRED from arg-positions; verify by dumping
    // raw bytes at first BindVertexAttribState (drop-to-bytes).
    public record struct NvnAttrib(int StreamIdx, int Format, int Offset);
    public record struct NvnStream(int Stride, int Divisor);
    public static readonly List<DrawRecord> Draws = new();
    static int _drawN;

    // Current bind state per cmdbuf (flat for v0 — only one cmdbuf in use).
    static ulong _curVbGpu, _curVbSize; static int _curVbIdx;
    // (T6)×17 multi-stream vbuf: ~83/165 cutscene draws use
    // 2 vertex streams (binding 0 = positions f16×4 stride
    // 8; binding 1 = normals/colors/UV stride 16). Game
    // calls BindVB(1,…) then BindVB(0,…); the scalar above
    // kept only stream-0 ⟹ stream-1 never captured ⟹
    // vs417 reads attr1-4=0 ⟹ constant out_attr (r61
    // verified: every pixel of a 21K-vtx mesh = same
    // (0,255,147)) ⟹ fs418 lighting=0 = the 50% black in
    // r44. = the vbuf-binding-1-NULL ×36K validation
    // finding traced to root. v0 tracks index 0+1; >1
    // unobserved (NVN supports 16).
    static ulong _curVbGpu1, _curVbSize1;
    static NvnAttrib[] _curAttribs = [];
    static NvnStream[] _curStreams = [];
    static ulong _curProgram, _curTexHandle; static int _curTexStage, _curTexSlot;
    // (c⁴⁰)(F) per-slot tex tracking. _curTexHandle (scalar)
    // = last-bound-wins; for sh813 the LAST bind before draw
    // is an aux slot (16/30/32) with texId=0x100 (= 4×4 black
    // default), so EnsureTexBound writes black to all 40 ⟹
    // any tex-sample → 0 ⟹ RGB=0. _curTexHandles[stage,slot]
    // captures the full table; SnapDraw snapshots FS-row.
    static readonly ulong[,] _curTexHandles = new ulong[2, 40];
    static long _bindTexN;
    // UBO per (stage, slot). umbra64: stage=0 slot=2 (64B = one mat4) is
    // the per-draw model matrix (rebinds 300+× — once per draw, ty steps
    // -0.05/row). slot=0 (224B) = ‡ projection+. slot=1/3 + stage=1 slot=0
    // bound once. v1: snapshot slot 2 per-draw → push-constant mat4.
    static readonly (ulong cpu, ulong size)[,] _ubo = new (ulong, ulong)[2, 8];
    static int _curUboBindN;
    static int _vpX, _vpY, _vpW, _vpH, _scX, _scY, _scW, _scH;

    [UnmanagedCallersOnly]
    static void CbBindUniformBuffer(ulong cb, int stage, int slot, ulong gpuAddr, ulong size) {
        var cpu = GpuToCpu(gpuAddr);
        if((uint) stage < 2 && (uint) slot < 8) _ubo[stage, slot] = (cpu, size);
        var n = ++_curUboBindN;
        if(n <= 8 || n % 200 == 0)
            $"[nvn] BindUniformBuffer #{n} stage={stage} slot={slot} gpu=0x{gpuAddr:x} cpu=0x{cpu:x} size=0x{size:x}".Log();
    }

    // Slot-0 (proj×view, 224B) snapshot — bound once, doesn't change
    // per-draw. NvnVulkan reads [0..15] = the proj mat4.
    public static byte[]? ProjUbo;

    // One-shot dump of all bound UBO slots at first draw (= the static
    // ones: projection in slot 0, etc.). Per drop-to-bytes — see what's there.
    static bool _uboDumped;
    static void DumpUboSlots() {
        if(_uboDumped) return; _uboDumped = true;
        for(var st = 0; st < 2; st++) for(var sl = 0; sl < 8; sl++) {
            var (cpu, size) = _ubo[st, sl];
            if(cpu == 0) continue;
            var n = (int) Math.Min(size, 256);
            var f = (float*) cpu;
            $"[nvn] UBO[stage={st},slot={sl}] size=0x{size:x} cpu=0x{cpu:x}:".Log();
            for(var i = 0; i < n/4; i += 4)
                $"[nvn]   [{i,2}]: {f[i],9:f4} {f[i+1],9:f4} {f[i+2],9:f4} {f[i+3],9:f4}".Log();
        }
    }
    [UnmanagedCallersOnly]
    static void CbSetViewport(ulong cb, int x, int y, int w, int h) {
        _vpX=x; _vpY=y; _vpW=w; _vpH=h;
    }
    [UnmanagedCallersOnly]
    static void CbSetScissor(ulong cb, int x, int y, int w, int h) {
        _scX=x; _scY=y; _scW=w; _scH=h;
    }

    [UnmanagedCallersOnly]
    static void CbBindVertexBuffer(ulong cb, int index, ulong gpuAddr, ulong size) {
        // (T6)×17: per-index. Was last-call-only scalar.
        if(index == 0) { _curVbGpu  = gpuAddr; _curVbSize  = size; }
        else           { _curVbGpu1 = gpuAddr; _curVbSize1 = size; }
        _curVbIdx = index;
        // Bounded log: first 20 of any + first 20 of idx>0
        // (= settles "do streams 0/1 alias same buffer?"
        // — if gpuAddr matches across indices, NvnReplay's
        // bind-b1=b0 fallback would work without recapture).
        if(_vbBindLogN++ < 20 || (index > 0 && _vbBind1LogN++ < 20))
            $"[nvn] BindVB[{index}] gpu=0x{gpuAddr:x} sz=0x{size:x}".Log();
    }
    static long _vbBindLogN, _vbBind1LogN;

    // NvnVertex*State setters. The struct is opaque-sized;
    // my v0 wrote 12B/8B and CORRUPTED HEAP (a test run SEGV in
    // BinUnlink, a test run 0-draws). v1 = TRACK in side-dict keyed
    // by ptr (= no writes into game memory). BindVertexAttrib
    // State reads from _attribByPtr instead of the array.
    // ⚠️ This means BindVertexAttribState's array-walk needs
    // the stride to compute &attribs[i] — STILL UNKNOWN.
    // ⟹ v1.5 = stride-from-Δ: at SetDefaults, if the prev
    // SetDefaults ptr was within ±64B and the delta divides
    // count, that's the stride. Recorded per-base.
    // ⟹ ACTUALLY: write SAFELY (≤4B = single u32). Pack
    // {fmt:8, off:16, streamIdx:4} into ONE u32 at p[0]. The
    // game's struct is ≥4B (= NVN's smallest opaque type).
    // ‡‡ If game's struct stride ≠ 4 (likely 8 per NVN convention),
    // BindVertexAttribState reads p[i*2] = every-other u32.
    // ⟹ Try BOTH 4B and 8B stride at Bind-time, log which has
    // sane fmt values.
    [UnmanagedCallersOnly] static void VasSetDefaults(int* p)
        { *p = 0; }
    [UnmanagedCallersOnly] static void VasSetFormat(int* p, int fmt, long off)
        { *p = (*p & ~0x00ffffff) | ((fmt & 0xff) << 16) | ((int)off & 0xffff); }
    [UnmanagedCallersOnly] static void VasSetStreamIndex(int* p, int idx)
        { *p = (*p & 0x00ffffff) | ((idx & 0xff) << 24); }
    [UnmanagedCallersOnly] static void VssSetDefaults(int* p)
        { *p = 0; }
    [UnmanagedCallersOnly] static void VssSetStride(int* p, int s)
        { *p = (*p & ~0xffff) | (s & 0xffff); }
    [UnmanagedCallersOnly] static void VssSetDivisor(int* p, int d)
        { *p = (*p & 0xffff) | (d << 16); }

    [UnmanagedCallersOnly]
    static void CbBindVertexAttribState(ulong cb, int count, void* states) {
        // v1.5: setters write a packed u32 at *p.
        // Stride UNKNOWN (= NVN opaque). Try 4B AND 8B; log
        // both interpretations. The one with non-zero fmt
        // values for >1 attrib = correct stride.
        // v2: NVN VertexAttribState struct = 4 BYTES.
        // CONFIRMED at-bytes via raw-dump (a test run): 4 packed
        // u32s contiguous, all 4 attribs populated. My v0
        // 12B-stride writes were stomping 8B/call → heap
        // corruption (a test run/a test run BinUnlink segv). v1.5 8B-
        // stride read skipped every other. v2 = stride 4.
        var p = (uint*) states;
        var arr = new NvnAttrib[count];
        for(var i = 0; i < count; i++) {
            var v = p[i];  // stride=4B = NVN's actual size
            arr[i] = new((int)(v >> 24), (int)((v >> 16) & 0xff), (int)(v & 0xffff));
        }
        _curAttribs = arr;
        if(_attribLogN++ < 3) {
            $"[nvn] BindVertexAttribState count={count}".Log();
            // Drop-to-bytes: dump raw first 64B + interpreted.
            var b = (byte*) states;
            $"[nvn]   raw: {string.Join(" ", Enumerable.Range(0, Math.Min(count*16,64)).Select(j => $"{b[j]:x2}"))}".Log();
            for(var i = 0; i < Math.Min(count, 8); i++)
                $"[nvn]   [{i}] streamIdx={arr[i].StreamIdx} fmt=0x{arr[i].Format:x} off={arr[i].Offset}".Log();
        }
    }
    static int _attribLogN, _streamLogN, _streamLog2N;

    [UnmanagedCallersOnly]
    static void CbBindVertexStreamState(ulong cb, int count, void* states) {
        // v2: NVN VertexStreamState = 4B packed (‡ count
        // always 1 in observed runs so stride untested; using
        // 4B per AttribState's confirmed-4B convention).
        var p = (uint*) states;
        var arr = new NvnStream[count];
        for(var i = 0; i < count; i++) {
            var v = p[i];
            arr[i] = new((int)(v & 0xffff), (int)(v >> 16));
        }
        // (T6)×17 struct-stride auto-detect: NVN's opaque
        // VertexStreamState may be 8B not 4B (the comment
        // above guessed 4B from AttribState, but count>1
        // was never observed at the time). Census shows
        // 83/165 draws have stride[1]=0 with max_streamIdx
        // =1 ⟹ p[1] read junk. If 4B-stride gave [1]=0 for
        // count>1, retry p[2] (= bytes 8-11 = struct[1] at
        // 8B). The recapture's raw-bytes log settles it
        // definitively; this is the safe-both-ways read.
        if(count > 1 && arr[1].Stride == 0) {
            var v8 = p[2];
            if((v8 & 0xffff) != 0) {
                for(var i = 1; i < count; i++) {
                    var v = p[i*2];
                    arr[i] = new((int)(v & 0xffff), (int)(v >> 16));
                }
            }
        }
        _curStreams = arr;
        // Log first-3-of-any + first-5 count>1 (= the
        // discriminator: raw bytes show real struct size).
        if(_streamLogN++ < 3 || (count > 1 && _streamLog2N++ < 5)) {
            $"[nvn] BindVertexStreamState count={count}".Log();
            var b = (byte*) states;
            $"[nvn]   raw: {string.Join(" ", Enumerable.Range(0, Math.Min(count*16,32)).Select(j => $"{b[j]:x2}"))}".Log();
            for(var i = 0; i < Math.Min(count, 4); i++)
                $"[nvn]   [{i}] stride={arr[i].Stride} divisor={arr[i].Divisor}".Log();
        }
    }
    // T3 instrument: track per-stage bound program. NVN
    // BindProgram's `stages` arg = bitmask of NVN_SHADER_STAGE_*
    // (‡ VERTEX_BIT=1, FRAGMENT_BIT=16). Each NVNprogram = 1 shader
    // (count=1 always at SetShaders), so BindProgram with stages=0x11
    // means "bind THIS program to BOTH vs and fs slots" (= it's a
    // linked pair? unlikely). More likely: 2× BindProgram per draw,
    // or 1× with stages=appropriate-bit. Discriminator = log it.
    static int _bindProgN;
    static readonly ulong[] _stagePr = new ulong[6];  // 0=vs..5=cs ‡

    [UnmanagedCallersOnly]
    static void CbBindProgram(ulong cb, ulong program, int stages) {
        _curProgram = program;
        // ‡ stages bitmask layout unknown — track which bits ever set.
        for(var b = 0; b < 6; b++)
            if((stages & (1 << b)) != 0) _stagePr[b] = program;
        var n = ++_bindProgN;
        if(n <= 10 || (n >= 100 && n <= 110)) {
            var ps = St.TryGetValue(program, out var s) ? s : null;
            $"[nvn] BindProgram #{n} prog=0x{program:x} stages=0x{stages:x} sh={ps?.ShIdx ?? -1} sph={ps?.SphType ?? -1}".Log();
        }
    }
    [UnmanagedCallersOnly]
    static void CbBindTexture(ulong cb, int stage, int slot, ulong handle) {
        _curTexHandle = handle; _curTexStage = stage; _curTexSlot = slot;
        // (c⁴⁰)(F) per-slot capture. NVN binds tex per
        // (stage, slot); shader reads tex_<2*slot+8> (=
        // SpirvEmit binding=2*slot+8 at set=1; sh0814 reads
        // bindings 16,30,32 = slots 4,11,12). Game's pattern
        // for sh813: bind diffuse→slot N (real BC1/3 tex),
        // then normal/shadow/etc → slots 4/11/12 with 0x100
        // defaults. Last-bound captured in _curTexHandle =
        // 0x100 (= black). Per-slot table fixes that.
        if((uint)stage < 2 && (uint)slot < 40)
            _curTexHandles[stage, slot] = handle;
        var n = ++_bindTexN;
        if(n <= 20 || n % 2000 == 0)
            $"[nvn] BindTexture #{n} stage={stage} slot={slot} handle=0x{handle:x} (texId=0x{handle>>32:x})".Log();
    }
    // ─── T3: shader bytecode capture ───
    // Wide capture: dedupe by data-binary hash, dump only distinct ones,
    // census top-14 prefixes across ALL shaders so MaxwellGenerator knows
    // which of the 177 mnemonics this game actually uses (= real priority
    // order for filling maxwell.isa). v0 dumped n≤5; this dumps every
    // distinct shader + writes /tmp/umbra-shader-census.txt at exit.
    static int _setShadersN;
    static readonly Dictionary<ulong, (int Idx, int RefN)> _shaderHash = new();
    static readonly Dictionary<ushort, int> _prefixCensus = new();
    static int _distinctShaderN;

    static ulong Fnv1a(byte* p, int len) {
        var h = 0xcbf29ce484222325ul;
        for(var i = 0; i < len; i++) { h ^= p[i]; h *= 0x100000001b3ul; }
        return h;
    }

    // Find the end of the instruction stream: scan from 0x80, stop at
    // first 0 word that ISN'T a sched-position (i%4==0). Then round up
    // to next sched group. ‡ Heuristic; control section has real size.
    static int FindShaderEnd(byte* p, int max) {
        for(int off = 0x80, i = 0; off + 8 <= max; off += 8, i++) {
            var w = *(ulong*)(p + off);
            if(i % 4 != 0 && w == 0) return off;
            // Also census this insn's top-14 prefix.
            if(i % 4 != 0) {
                var top14 = (ushort)((w >> 50) & 0x3fff);
                lock(_prefixCensus)
                    _prefixCensus[top14] = _prefixCensus.GetValueOrDefault(top14) + 1;
            }
        }
        return max;
    }

    public static void DumpShaderCensus() {
        // Called once at process exit (or manually). Writes the prefix
        // census + distinct-shader summary so MaxwellGenerator can
        // prioritize which mnemonics to fill next.
        try {
            using var sw = new StreamWriter("/tmp/umbra-shader-census.txt");
            sw.WriteLine($"# {_setShadersN} ProgramSetShaders calls, {_distinctShaderN} distinct shaders");
            sw.WriteLine($"# {_prefixCensus.Count} distinct top-14 prefixes:");
            foreach(var (p, n) in _prefixCensus.OrderByDescending(kv => kv.Value))
                sw.WriteLine($"{Convert.ToString(p, 2).PadLeft(14, '0')}  {n}");
            $"[nvn] DumpShaderCensus → /tmp/umbra-shader-census.txt ({_distinctShaderN} distinct, {_prefixCensus.Count} prefixes)".Log();
        } catch(Exception e) { $"[nvn] census dump failed: {e.Message}".Log(); }
    }

    [UnmanagedCallersOnly]
    static int ProgramSetShaders(ulong program, int count, ulong* stageData) {
        var n = ++_setShadersN;
        // ‡ NVNshaderData layout unknown. Stub-log showed count=1 always
        // (~1000+ programs). Per public NVN RE the struct is ~16B:
        // {NVNbufferAddress data; void* control;} — data = GPU addr to
        // the uploaded SASS binary in a SHADER_CODE memory pool; control
        // = CPU ptr to a glslc control section (stage, entry, sizes).
        // But: dump first, verify second (drop-to-bytes).
        if(stageData != null) {
            for(var i = 0; i < count; i++) {
                var dataGpu = stageData[i*2 + 0];
                var ctrlCpu = stageData[i*2 + 1];
                var dataCpu = GpuToCpu(dataGpu);
                if(dataCpu == 0) continue;
                var p = (byte*) dataCpu;
                // Sanity: magic 0x12345678 @0.
                if(*(uint*) p != 0x12345678) {
                    if(n <= 3) $"[nvn] ProgramSetShaders #{n}: ‡ no magic at data (0x{*(uint*)p:x8})".Log();
                    continue;
                }
                // Find instruction-stream end (also feeds _prefixCensus).
                var end = FindShaderEnd(p, 16384);
                // Hash the SPH+insn region (skip 0x00-0x2f padding) for dedupe.
                var hash = Fnv1a(p + 0x30, end - 0x30);
                int shIdx;
                lock(_shaderHash) {
                    if(_shaderHash.TryGetValue(hash, out var seen)) {
                        _shaderHash[hash] = (seen.Idx, seen.RefN + 1);
                        shIdx = seen.Idx;
                        // Track on the program handle for T3.
                        var ps2 = S(program);
                        ps2.ShIdx = seen.Idx; ps2.SphType = p[0x30] & 0xf;
                        continue;  // already dumped
                    }
                    var idx = ++_distinctShaderN;
                    _shaderHash[hash] = (idx, 1); shIdx = idx;
                    // Dump distinct shader to disk. Sized to actual end.
                    var sphType = p[0x30] & 0xf;  // 1=vtx-like, 2=ps
                    // Track program-handle → shader-idx + sph-type for
                    // T3 (so DrawArrays can name which sh*.bin
                    // its bound program corresponds to).
                    var ps = S(program); ps.ShIdx = idx; ps.SphType = sphType;
                    var path = $"/tmp/umbra-shaders/sh{idx:d4}-t{sphType}.bin";
                    Directory.CreateDirectory("/tmp/umbra-shaders");
                    File.WriteAllBytes(path, new ReadOnlySpan<byte>(p, end).ToArray());
                    if(idx <= 10 || idx % 50 == 0)
                        $"[nvn] shader #{idx} (call #{n}) sph={sphType} {end-0x80}B insns hash={hash:x16} → {path}".Log();
                }
            }
        }
        // Periodic census dump (so a SIGKILL'd run still leaves data).
        if(n == 100 || n % 500 == 0) DumpShaderCensus();
        return 1;
    }

    [UnmanagedCallersOnly]
    static ulong DeviceGetTextureHandle(ulong dev, int texId, int samplerId) {
        // Real NVN packs (texId, samplerId) into a handle the GPU descriptor
        // hardware reads. We just need it to round-trip through BindTexture
        // → recorded → reverse-mapped to the texture. v0 = pack hi/lo.
        return ((ulong)(uint) texId << 32) | (uint) samplerId;
    }

    static ulong[] SnapTexRow(int stage) {
        var r = new ulong[40];
        for(var i = 0; i < 40; i++) r[i] = _curTexHandles[stage, i];
        return r;
    }
    // (c²⁷) Shared state-snapshot for both DrawArrays and
    // DrawElementsBaseVertex. Returns the populated DrawRecord
    // with all current pipeline/UBO/tex/viewport state captured.
    static DrawRecord SnapDraw(int prim) {
        var vbCpu = GpuToCpu(_curVbGpu);
        var ubos = new byte[16][];
        for(var st = 0; st < 2; st++)
            for(var sl = 0; sl < 8; sl++) {
                var (cpu, sz) = _ubo[st, sl];
                if(cpu == 0) continue;
                var cap = (int) Math.Min(sz, 4096);
                ubos[st*8 + sl] = new ReadOnlySpan<byte>((void*)cpu, cap).ToArray();
            }
        return new DrawRecord {
            Prim = prim,
            VbGpu = _curVbGpu, VbCpu = vbCpu, VbSize = _curVbSize, VbIndex = _curVbIdx,
            VbGpu1 = _curVbGpu1,
            // (T6)×17×4: GpuToCpu(0) per-draw cost suspect
            // (u762 hit ~1.1fps vs u761 ~52fps; menu phase
            // never calls BindVB(1,…) so _curVbGpu1 stays 0).
            // Guard regardless of whether GpuToCpu(0) is the
            // actual cost — semantically correct either way.
            VbCpu1 = _curVbGpu1 != 0 ? GpuToCpu(_curVbGpu1) : 0,
            VbSize1 = _curVbSize1,
            Program = _curProgram,
            TexHandle = _curTexHandle, TexStage = _curTexStage, TexSlot = _curTexSlot,
            // (c⁴⁰)(F) FS per-slot tex snapshot. 40×8B = 320B
            // /draw — at ~200 idx-draws/frame ≈ 64KB/frame,
            // acceptable. Only FS row (stage=1); VS textures
            // are rare (vertex-fetch, none in this game).
            TexHandles = SnapTexRow(1),
            Ubo = ubos[2], Ubos = ubos,
            VsShIdx = St.TryGetValue(_stagePr[0], out var pvs) ? pvs.ShIdx : 0,
            FsShIdx = St.TryGetValue(_stagePr[1], out var pfs) ? pfs.ShIdx : 0,
            Tex = ResolveTex(_curTexHandle),
            VpX = _vpX, VpY = _vpY, VpW = _vpW, VpH = _vpH,
            ScX = _scX, ScY = _scY, ScW = _scW, ScH = _scH,
            Attribs = _curAttribs, Streams = _curStreams,
        };
    }

    static int _drawElN;
    [UnmanagedCallersOnly]
    static void CbDrawElementsBaseVertex(ulong cb, int prim,
            int idxType, int count, ulong idxGpuAddr, int baseVtx) {
        var n = ++_drawElN;
        var idxCpu = GpuToCpu(idxGpuAddr);
        var dr = SnapDraw(prim);
        dr.N = -n;  // negative = indexed (visual disc in logs)
        dr.IdxCpu = idxCpu; dr.IdxType = idxType;
        dr.IdxCount = count; dr.BaseVtx = baseVtx;
        lock(Draws) Draws.Add(dr);
        if(n <= 25 || n % 1000 == 0)
            $"[nvn] DrawElementsBV #{n} prim={prim} idxT={idxType} count={count} idx=gpu:0x{idxGpuAddr:x}/cpu:0x{idxCpu:x} baseVtx={baseVtx} vb=gpu:0x{_curVbGpu:x}/sz=0x{_curVbSize:x} sh{dr.VsShIdx}/{dr.FsShIdx} tex=0x{_curTexHandle:x} vp=({_vpX},{_vpY},{_vpW},{_vpH}) str={(_curStreams.Length>0?_curStreams[0].Stride:0)} attr=[{string.Join(",",_curAttribs.Select(a=>$"0x{a.Format:x}@{a.Offset}"))}]".Log();
    }

    [UnmanagedCallersOnly]
    static void CbDrawArrays(ulong cb, int prim, int first, int count) {
        var n = ++_drawN;
        var vbCpu = GpuToCpu(_curVbGpu);
        if(n == 1) DumpUboSlots();
        // Snapshot slot-0 once for the proj×view push-constant. 128B =
        // proj mat4 @0 + view mat4 @64 (per umbra65 DumpUboSlots: [0..15]
        // = sx=1.429 sy=2.5405 perspective z/w; [16..31] = identity+tz=-7).
        if(ProjUbo == null && _ubo[0,0].cpu != 0 && _ubo[0,0].size >= 128)
            ProjUbo = new ReadOnlySpan<byte>((void*)_ubo[0,0].cpu, 128).ToArray();
        // T3: snapshot ALL bound NVN UBO slots (stage 0 = vertex,
        // stage 1 = fragment) at THIS moment. NVN slot K → hw cbuf
        // c[K+3] (CONFIRMED at sh0349). c[1]/c[2] are driver-managed
        // (NvnVulkan synthesizes those). Cap each at 4KB (= the SPIR-V
        // float[1024] declared size). Slot 2 (model mat4) is the
        // per-draw one (rebinds every draw); slots 0/1/3 are static
        // per-frame. Captured per-draw regardless so RecordDrawPass
        // doesn't need to reason about which-changes-when.
        var ubos = new byte[16][];
        for(var st = 0; st < 2; st++)
            for(var sl = 0; sl < 8; sl++) {
                var (cpu, sz) = _ubo[st, sl];
                if(cpu == 0) continue;
                var cap = (int) Math.Min(sz, 4096);
                ubos[st*8 + sl] = new ReadOnlySpan<byte>((void*) cpu, cap).ToArray();
            }
        // Keep T2 v1's slot-2-only snapshot for the fixed-shader
        // path (DrawRecord.Ubo = the model mat4 push-constant source).
        byte[]? ubo = ubos[2];
        var dr = new DrawRecord {
            N = n, Prim = prim, First = first, Count = count,
            VbGpu = _curVbGpu, VbCpu = vbCpu, VbSize = _curVbSize, VbIndex = _curVbIdx,
            VbGpu1 = _curVbGpu1,
            // (T6)×17×4: GpuToCpu(0) per-draw cost suspect
            // (u762 hit ~1.1fps vs u761 ~52fps; menu phase
            // never calls BindVB(1,…) so _curVbGpu1 stays 0).
            // Guard regardless of whether GpuToCpu(0) is the
            // actual cost — semantically correct either way.
            VbCpu1 = _curVbGpu1 != 0 ? GpuToCpu(_curVbGpu1) : 0,
            VbSize1 = _curVbSize1,
            Program = _curProgram,
            TexHandle = _curTexHandle, TexStage = _curTexStage, TexSlot = _curTexSlot,
            Ubo = ubo, Ubos = ubos,
            // T3: which sh{idx} the bound VS/FS programs map to.
            // _stagePr[0]=VS, [1]=FS per umbra71 (stages bit 0/1).
            VsShIdx = St.TryGetValue(_stagePr[0], out var pvs) ? pvs.ShIdx : 0,
            FsShIdx = St.TryGetValue(_stagePr[1], out var pfs) ? pfs.ShIdx : 0,
            // resolve TexHandle → tex state at DRAW time (=
            // re-registration-safe; latest pool-idx mapping wins).
            Tex = ResolveTex(_curTexHandle),
            VpX = _vpX, VpY = _vpY, VpW = _vpW, VpH = _vpH,
            ScX = _scX, ScY = _scY, ScW = _scW, ScH = _scH,
            Attribs = _curAttribs, Streams = _curStreams,
        };
        lock(Draws) Draws.Add(dr);
        if(n <= 25 || n % 100 == 0) {
            // T3 instrument: which sh{idx} files do the bound
            // programs correspond to? _stagePr[b] = program handle
            // bound at NVN_SHADER_STAGE bit b (‡ bit-layout TBD; log
            // all 6). For each, look up its ShIdx via S(prog).
            var stPr = string.Join(" ", Enumerable.Range(0, 6)
                .Where(b => _stagePr[b] != 0)
                .Select(b => {
                    var ps = St.TryGetValue(_stagePr[b], out var s) ? s : null;
                    return $"st[{b}]=sh{ps?.ShIdx ?? -1}/t{ps?.SphType ?? -1}";
                }));
            // The program passed to BindProgram itself:
            var cps = St.TryGetValue(_curProgram, out var cp) ? cp : null;
            $"[nvn] DrawArrays #{n} prim={prim} first={first} count={count} vb=gpu:0x{_curVbGpu:x}/cpu:0x{vbCpu:x}/size=0x{_curVbSize:x} prog=0x{_curProgram:x}(sh{cps?.ShIdx ?? -1}/t{cps?.SphType ?? -1}) tex=0x{_curTexHandle:x} vp=({_vpX},{_vpY},{_vpW},{_vpH}) {stPr}".Log();
            // T2 dump: write the vertex data to disk. ‡ Don't know
            // stride yet (set via VertexStreamState which I'm not tracking);
            // dump as raw bytes + a float[4]-per-vertex guess. Per drop-to-bytes:
            // see what's there before reasoning about format.
            if(vbCpu != 0 && n <= 5) DumpVertexBuf(n, (byte*) vbCpu, _curVbSize, count);
        }
    }

    static void DumpVertexBuf(int n, byte* src, ulong size, int count) {
        try {
            var path = $"/tmp/umbra-vbuf-{n:d2}.bin";
            var sz = (int) Math.Min(size, 4096);
            File.WriteAllBytes(path, new ReadOnlySpan<byte>(src, sz).ToArray());
            // Guess: stride = size/count if it divides cleanly.
            var stride = count > 0 && size % (ulong) count == 0 ? (int)(size / (ulong) count) : 0;
            $"[nvn]   → {path} ({sz} bytes; ‡ stride guess = {stride})".Log();
            // Hex-dump first few "vertices" as floats per drop-to-bytes.
            var f = (float*) src;
            var nf = stride > 0 ? stride/4 : 8;
            for(var v = 0; v < Math.Min(count, 4); v++) {
                var s = string.Join(" ", Enumerable.Range(0, Math.Min(nf, 12))
                    .Select(i => $"{f[v*nf + i]:f3}"));
                $"[nvn]     v{v}: {s}".Log();
            }
        } catch(Exception e) { $"[nvn]   ‡ vbuf dump failed: {e.Message}".Log(); }
    }

    [UnmanagedCallersOnly]
    static void CbSetRenderTargets(ulong cb, int numColors, ulong* colors,
            ulong* colorViews, ulong depthStencil, ulong depthView) {
        // ‡ v0.5: just log; v1 records which textures are bound so Present
        // knows which VkImage to clear (currently using texIdx from Acquire).
        if(numColors > 0 && colors != null)
            S(cb).PoolPtr = colors[0];  // borrow PoolPtr field as "current RT"
    }

    // ─── Window / Queue — v0 Vulkan present path ───
    static int _acqIdx;
    [UnmanagedCallersOnly]
    static int WindowAcquireTexture(ulong win, ulong sync, int* texIdx) {
        // Rotate through 0..2 (game set up 3 textures via WindowBuilderSetTextures).
        // ‡ v0: ignore sync; real impl waits on a fence/semaphore.
        if(texIdx != null) *texIdx = _acqIdx;
        _acqIdx = (_acqIdx + 1) % 3;
        return 0;  // NVN_WINDOW_ACQUIRE_TEXTURE_RESULT_SUCCESS
    }
    [UnmanagedCallersOnly]
    static int QueueAcquireTexture(ulong queue, ulong win, int* texIdx) {
        // Older API name for the same thing (legoworlds calls this one).
        if(texIdx != null) *texIdx = _acqIdx;
        _acqIdx = (_acqIdx + 1) % 3;
        return 0;
    }
    [UnmanagedCallersOnly]
    static void QueuePresentTexture(ulong queue, ulong win, int texIdx) {
        NvnVulkan.Present(win, texIdx);
    }

    static readonly HashSet<int> _seenPname = new();
    static int LogUnknownPname(int p) {
        lock(_seenPname)
            if(_seenPname.Add(p))
                $"[nvn] DeviceGetInteger pname=0x{p:x} ‡ NOT in sera's list (her side traps)".Log();
        return 1;  // ‡
    }

    static readonly System.Diagnostics.Stopwatch _clock = System.Diagnostics.Stopwatch.StartNew();
    static ulong NowNs() =>
        (ulong) (_clock.ElapsedTicks * (1_000_000_000.0
                 / System.Diagnostics.Stopwatch.Frequency));
    [UnmanagedCallersOnly]
    static ulong DeviceGetCurrentTimestampNs(ulong dev) => NowNs();
    static long _getTsN;
    [UnmanagedCallersOnly]
    static ulong DeviceGetTimestampNs(ulong dev, ulong* counterData) {
        // (c²⁹) NVNcounterData = { u64 counter; u64 timestamp }.
        // Real impl converts timestamp (raw GPU ticks) → ns.
        // CbReportCounter writes ns directly at pool-buf+8.
        // u720: 9.5M calls, cd=fff34e7ebe50 fixed (stack-local,
        // NOT pool), cd[0]=cd[1]=0 always ⟹ game's local copy
        // never gets the ReportCounter data (‡ via stub'd nvn
        // SyncWait?). Game spin-polls → throughput halved. v1
        // hack: if cd reads all-0, return NowNs() (= unblock
        // the spin; the value is monotone-ns which is what the
        // conversion would yield anyway). Honest path = find
        // what writes cd (·7827 the call site).
        var n = ++_getTsN;
        if(counterData == null) return NowNs();
        var c0 = counterData[0]; var c1 = counterData[1];
        if(n <= 3 || n % 50000 == 0)
            $"[nvn] DeviceGetTimestampNs #{n} cd=0x{(ulong)counterData:x} [0]={c0} [1]={c1}".Log();
        // Layout uncertainty (kt[19]): try +8, then +0, then
        // fall back to wall-clock. Whichever is nonzero is
        // probably the timestamp.
        return c1 != 0 ? c1 : c0 != 0 ? c0 : NowNs();
    }
    static long _rcN;
    [UnmanagedCallersOnly]
    static void CbReportCounter(ulong cb, int counterType, ulong bufGpuAddr) {
        // (c²⁹) Write NVNcounterData { counter, timestamp } at
        // bufGpuAddr. counterType 0 = TIMESTAMP (the only type
        // legoworlds uses, ~50/frame). Real GPU writes raw GPU
        // ticks; we write wall-ns directly (= what GetTimestampNs
        // would convert TO). counterType≥1 (SAMPLES_PASSED etc.)
        // ⟹ counter field would carry the query value; ‡ stub 0.
        var cpu = GpuToCpu(bufGpuAddr);
        var n = ++_rcN;
        if(cpu == 0) {
            if(n <= 3) $"[nvn] ReportCounter #{n} type={counterType} buf=gpu:0x{bufGpuAddr:x} → cpu=0 (unmapped, skip)".Log();
            return;
        }
        var ts = NowNs();
        *(ulong*) cpu       = 0;   // counter value (unused for TIMESTAMP)
        *(ulong*)(cpu + 8)  = ts;
        if(n <= 5 || n % 50000 == 0)
            $"[nvn] ReportCounter #{n} type={counterType} buf=gpu:0x{bufGpuAddr:x}/cpu:0x{cpu:x} → ts={ts}".Log();
    }

    // ─── MemoryPool ───
    [UnmanagedCallersOnly]
    static void MpbSetStorage(ulong b, void* ptr, ulong size) {
        var s = S(b); s.CpuPtr = (ulong) ptr; s.Size = size;
    }
    [UnmanagedCallersOnly]
    static void MpbSetFlags(ulong b, int flags) => S(b).Flags = flags;
    [UnmanagedCallersOnly]
    static int MpInitialize(ulong pool, ulong builder) {
        var bs = S(builder); var ps = S(pool);
        ps.CpuPtr = bs.CpuPtr; ps.Size = bs.Size; ps.Flags = bs.Flags;
        ps.GpuAddr = AllocGpu(bs.Size);
        $"[nvn] MemoryPoolInitialize pool=0x{pool:x} cpu=0x{ps.CpuPtr:x} size=0x{ps.Size:x} flags=0x{ps.Flags:x} gpu=0x{ps.GpuAddr:x}".Log();
        return 1;
    }
    [UnmanagedCallersOnly] static void* MpMap(ulong p) => (void*) S(p).CpuPtr;
    [UnmanagedCallersOnly] static ulong MpGetBufferAddress(ulong p) => S(p).GpuAddr;
    [UnmanagedCallersOnly] static int MpGetFlags(ulong p) => S(p).Flags;
    [UnmanagedCallersOnly] static ulong MpGetSize(ulong p) => S(p).Size;

    // ─── Buffer ───
    [UnmanagedCallersOnly]
    static void BbSetStorage(ulong b, ulong pool, ulong off, ulong size) {
        var s = S(b); s.PoolPtr = pool; s.Offset = off; s.Size = size;
    }
    [UnmanagedCallersOnly]
    static int BufInitialize(ulong buf, ulong builder) {
        var bs = S(builder); var s = S(buf);
        s.PoolPtr = bs.PoolPtr; s.Offset = bs.Offset; s.Size = bs.Size;
        var pool = St.TryGetValue(bs.PoolPtr, out var ps) ? ps : null;
        s.CpuPtr = pool != null ? pool.CpuPtr + bs.Offset : 0;
        s.GpuAddr = pool != null ? pool.GpuAddr + bs.Offset : AllocGpu(bs.Size);
        return 1;
    }
    [UnmanagedCallersOnly] static void* BufMap(ulong b) {
        var s = S(b);
        if(s.CpuPtr == 0) {
            // Pool wasn't CPU-mapped (or we missed SetStorage). Allocate
            // scratch so the game's memcpy doesn't crash; data goes nowhere
            // useful but the wall moves. ‡ fallback.
            s.CpuPtr = (ulong) NativeMemory.AlignedAlloc((nuint) Math.Max(s.Size, 4096), 4096);
            $"[nvn] BufferMap(0x{b:x}) ‡ no pool storage; scratch 0x{s.CpuPtr:x} size=0x{s.Size:x}".Log();
        }
        return (void*) s.CpuPtr;
    }
    [UnmanagedCallersOnly] static ulong BufGetAddress(ulong b) => S(b).GpuAddr;
    [UnmanagedCallersOnly] static ulong BufGetSize(ulong b) => S(b).Size;

    static long _gpu = 0x6_0000_0000;
    static ulong AllocGpu(ulong size) =>
        (ulong) System.Threading.Interlocked.Add(ref _gpu, (long)((size + 0xfff) & ~0xffful))
        - ((size + 0xfff) & ~0xffful);

    // ─── unknown-name fallback (logged once per name) ───
    static readonly HashSet<string> _seenUnknown = new();
    public static void NoteUnknown(string name) {
        lock(_seenUnknown)
            if(_seenUnknown.Add(name))
                $"[nvn] '{name}' → universal-stub (no impl yet)".Log();
    }
}
