using System.Runtime.InteropServices;

namespace UmbraCore.Core;

// NVN → Vulkan backend (lavapipe), v0.
// "let's go the lavapipe route! Time to write a separate vulkan impl to
// kick things off? :3" — parallel to her NativeLib/nvn*.mm Metal backend.
// NvnLinux.cs stays the state-tracking layer (per-handle dict, builder→
// object copy); this does the backend at QueueSubmit/Present, which is
// where her Metal wall is too (per nvnQueue.mm read — no MTL refs there).
// v0 = VkInstance + lavapipe VkPhysicalDevice + VkDevice + VkQueue +
// VkCommandPool. No swapchain (headless). Wired from nvnDeviceInitialize.
// nvnQueuePresentTexture → log frame# + submit an empty cmdbuf (proves
// the wiring). v0.5 = nvnWindow's 3 textures → 3 VkImage HOST_VISIBLE,
// nvnCommandBufferClearColor → vkCmdClearColorImage, readback → PPM dump
// = "I can see something."
// Hand-rolled P/Invoke (no Silk.NET) — matches the segvtrap.c style. ~12
// entry points + ~6 structs for v0. Run with VK_ICD_FILENAMES pointing at
// lvp_icd.aarch64.json to skip the broken PowerVR/etc ICDs on this box.
// ‡ v0: no validation layers; no error recovery (Init fails → log + nvn
// path keeps logging-stub-only). Queue family = first that has GRAPHICS.

public static unsafe class NvnVulkan {
    const string Lib = "libvulkan.so.1";

    // ─── handles (all 64-bit opaque) ───
    static ulong _inst, _phys, _dev, _queue, _cmdPool, _cmdBuf, _fence;
    static uint _qFam;
    static int _frameN;
    public static bool Ready;

    // ─── minimal struct set (sType-tagged, zeroable) ───
    // Layouts match vulkan_core.h on LP64. Only fields we set are named;
    // the rest are padding to keep offsets right.

    [StructLayout(LayoutKind.Sequential)] struct VkApplicationInfo {
        public uint sType; public void* pNext;
        public byte* pAppName; public uint appVer;
        public byte* pEngineName; public uint engineVer;
        public uint apiVersion;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkInstanceCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public VkApplicationInfo* pAppInfo;
        public uint layerCount; public byte** ppLayers;
        public uint extCount; public byte** ppExts;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDeviceQueueCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint queueFamilyIndex; public uint queueCount;
        public float* pQueuePriorities;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDeviceCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint queueCreateInfoCount; public VkDeviceQueueCreateInfo* pQueueCreateInfos;
        public uint layerCount; public byte** ppLayers;
        public uint extCount; public byte** ppExts;
        public void* pEnabledFeatures;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkCommandPoolCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint queueFamilyIndex;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkCommandBufferAllocateInfo {
        public uint sType; public void* pNext;
        public ulong commandPool; public uint level; public uint count;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkCommandBufferBeginInfo {
        public uint sType; public void* pNext; public uint flags;
        public void* pInheritanceInfo;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkSubmitInfo {
        public uint sType; public void* pNext;
        public uint waitSemCount; public ulong* pWaitSems; public uint* pWaitStages;
        public uint cmdBufCount; public ulong* pCmdBufs;
        public uint signalSemCount; public ulong* pSignalSems;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkFenceCreateInfo {
        public uint sType; public void* pNext; public uint flags;
    }
    // VkPhysicalDeviceProperties — only need deviceName @ offset 20
    // (apiVersion u32, driverVersion u32, vendorID u32, deviceID u32,
    // deviceType u32, deviceName char[256], …). 824 bytes total.
    [StructLayout(LayoutKind.Sequential)] struct VkPhysDevProps {
        public uint apiVersion, driverVersion, vendorID, deviceID, deviceType;
        public fixed byte deviceName[256];
        fixed byte _rest[824 - 20 - 256];
    }
    [StructLayout(LayoutKind.Sequential)] struct VkQueueFamilyProps {
        public uint queueFlags, queueCount, timestampValidBits;
        public uint w, h, d;  // minImageTransferGranularity
    }

    // sType values
    const uint ST_APP_INFO = 0, ST_INST_CI = 1, ST_DEV_QUEUE_CI = 2,
               ST_DEV_CI = 3, ST_SUBMIT = 4, ST_FENCE_CI = 8,
               ST_CMDPOOL_CI = 39, ST_CMDBUF_AI = 40, ST_CMDBUF_BI = 42;

    // ─── P/Invoke ───
    [DllImport(Lib)] static extern int vkCreateInstance(
        VkInstanceCreateInfo* ci, void* alloc, ulong* inst);
    [DllImport(Lib)] static extern int vkEnumeratePhysicalDevices(
        ulong inst, uint* count, ulong* devs);
    [DllImport(Lib)] static extern void vkGetPhysicalDeviceProperties(
        ulong phys, VkPhysDevProps* props);
    [DllImport(Lib)] static extern void vkGetPhysicalDeviceQueueFamilyProperties(
        ulong phys, uint* count, VkQueueFamilyProps* props);
    [DllImport(Lib)] static extern int vkCreateDevice(
        ulong phys, VkDeviceCreateInfo* ci, void* alloc, ulong* dev);
    [DllImport(Lib)] static extern void vkGetDeviceQueue(
        ulong dev, uint qFam, uint qIdx, ulong* queue);
    [DllImport(Lib)] static extern int vkCreateCommandPool(
        ulong dev, VkCommandPoolCreateInfo* ci, void* alloc, ulong* pool);
    [DllImport(Lib)] static extern int vkAllocateCommandBuffers(
        ulong dev, VkCommandBufferAllocateInfo* ai, ulong* bufs);
    [DllImport(Lib)] static extern int vkBeginCommandBuffer(
        ulong cb, VkCommandBufferBeginInfo* bi);
    [DllImport(Lib)] static extern int vkEndCommandBuffer(ulong cb);
    [DllImport(Lib)] static extern int vkCreateFence(
        ulong dev, VkFenceCreateInfo* ci, void* alloc, ulong* fence);
    [DllImport(Lib)] static extern int vkResetFences(ulong dev, uint n, ulong* f);
    [DllImport(Lib)] static extern int vkQueueSubmit(
        ulong queue, uint n, VkSubmitInfo* submits, ulong fence);
    [DllImport(Lib)] static extern int vkWaitForFences(
        ulong dev, uint n, ulong* fences, uint waitAll, ulong timeout);

    // ─── v0.5: image + memory + clear + readback ───
    [StructLayout(LayoutKind.Sequential)] struct VkImageCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint imageType;          // VK_IMAGE_TYPE_2D = 1
        public uint format;             // R8G8B8A8_UNORM = 37
        public uint width, height, depth;
        public uint mipLevels, arrayLayers;
        public uint samples;            // 1
        public uint tiling;             // LINEAR = 1 (for direct readback)
        public uint usage;              // TRANSFER_DST | TRANSFER_SRC = 3
        public uint sharingMode;        // EXCLUSIVE = 0
        public uint queueFamilyIndexCount; public uint* pQueueFamilyIndices;
        public uint initialLayout;      // UNDEFINED = 0
    }
    [StructLayout(LayoutKind.Sequential)] struct VkMemoryRequirements {
        public ulong size, alignment; public uint memoryTypeBits;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkMemoryAllocateInfo {
        public uint sType; public void* pNext;
        public ulong allocationSize; public uint memoryTypeIndex;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPhysicalDeviceMemoryProperties {
        public uint memoryTypeCount;
        public fixed ulong memoryTypes[32 * 2];  // {propertyFlags u32, heapIndex u32} × 32, packed as u64s
        public uint memoryHeapCount;
        public fixed ulong memoryHeaps[16 * 2];  // {size u64, flags u32+pad}
    }
    [StructLayout(LayoutKind.Sequential)] struct VkImageSubresourceRange {
        public uint aspectMask, baseMipLevel, levelCount, baseArrayLayer, layerCount;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkClearColorValue {
        public float r, g, b, a;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkImageMemoryBarrier {
        public uint sType; public void* pNext;
        public uint srcAccessMask, dstAccessMask;
        public uint oldLayout, newLayout;
        public uint srcQueueFamilyIndex, dstQueueFamilyIndex;
        public ulong image;
        public VkImageSubresourceRange subresourceRange;
    }

    const uint ST_IMAGE_CI = 14, ST_MEM_AI = 5, ST_IMG_BARRIER = 45;

    [DllImport(Lib)] static extern int vkCreateImage(
        ulong dev, VkImageCreateInfo* ci, void* alloc, ulong* img);
    [DllImport(Lib)] static extern void vkGetImageMemoryRequirements(
        ulong dev, ulong img, VkMemoryRequirements* req);
    [DllImport(Lib)] static extern void vkGetPhysicalDeviceMemoryProperties(
        ulong phys, VkPhysicalDeviceMemoryProperties* props);
    [DllImport(Lib)] static extern int vkAllocateMemory(
        ulong dev, VkMemoryAllocateInfo* ai, void* alloc, ulong* mem);
    [DllImport(Lib)] static extern int vkBindImageMemory(
        ulong dev, ulong img, ulong mem, ulong offset);
    [DllImport(Lib)] static extern int vkMapMemory(
        ulong dev, ulong mem, ulong offset, ulong size, uint flags, void** pp);
    [DllImport(Lib)] static extern void vkCmdPipelineBarrier(
        ulong cb, uint srcStage, uint dstStage, uint depFlags,
        uint nMem, void* memBarriers,
        uint nBuf, void* bufBarriers,
        uint nImg, VkImageMemoryBarrier* imgBarriers);
    [DllImport(Lib)] static extern void vkCmdClearColorImage(
        ulong cb, ulong img, uint layout, VkClearColorValue* color,
        uint nRanges, VkImageSubresourceRange* ranges);

    // ─── T2: render pass + pipeline + draw ───
    // Geometry-only v0 (no descriptor sets — FS is solid vColor). Adds
    // ~15 entry points + ~12 structs. Most CreateInfo structs are
    // mostly-zero; only the fields we set are named, rest are explicit
    // padding to keep offsets right (verified against vulkan_core.h).

    [StructLayout(LayoutKind.Sequential)] struct VkAttachmentDescription {
        public uint flags, format, samples;
        public uint loadOp, storeOp, stencilLoadOp, stencilStoreOp;
        public uint initialLayout, finalLayout;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkAttachmentReference {
        public uint attachment, layout;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkSubpassDescription {
        public uint flags, pipelineBindPoint;
        public uint inputCount; public void* pInputs;
        public uint colorCount; public VkAttachmentReference* pColors;
        public void* pResolves; public void* pDepthStencil;
        public uint preserveCount; public void* pPreserves;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkRenderPassCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint attachmentCount; public VkAttachmentDescription* pAttachments;
        public uint subpassCount; public VkSubpassDescription* pSubpasses;
        public uint dependencyCount; public void* pDependencies;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkImageViewCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public ulong image; public uint viewType, format;
        public uint compR, compG, compB, compA;     // VkComponentMapping
        public VkImageSubresourceRange subresourceRange;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkFramebufferCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public ulong renderPass;
        public uint attachmentCount; public ulong* pAttachments;
        public uint width, height, layers;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkShaderModuleCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public nuint codeSize; public uint* pCode;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPushConstantRange {
        public uint stageFlags, offset, size;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineLayoutCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint setLayoutCount; public void* pSetLayouts;
        public uint pushConstantRangeCount; public VkPushConstantRange* pPushConstantRanges;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineShaderStageCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint stage; public ulong module; public byte* pName;
        public void* pSpecializationInfo;
    }
    // ── (x) NVNformat → VkFormat (vertex-attrib formats) ────────────
    // NVN's format enum is undocumented; values inferred per-observed
    // (= known stride+offset of menu's 4-attr layout → which
    // VkFormat fits the slot). The texture-format space (0x25/0x42/
    // 0x44) is in NvnLinux.Fmt(); vertex formats are a different sub-
    // range (0x2N = 32-bit-component family per spacing).
    //
    // GROUND TRUTH (menu, stride=36, renders 90788nz correct):
    //   loc0 @0  pos     12B → R32G32B32_SFLOAT  (= NVN ‡0x2f? not seen)
    //   loc1 @12 color    4B → R8G8B8A8_UNORM    (= NVN ‡0x25? = RGBA8)
    //   loc2 @20 uv       8B → R32G32_SFLOAT     (= NVN 0x2e CONFIRMED)
    //   loc3 @28 extra    8B → R32G32_SFLOAT     (= NVN ‡)
    //
    // ⟹ 0x2e = R32G32_SFLOAT (8B). Inferred adjacents:
    //   0x2d = R32_SFLOAT? (4B)
    //   0x2f = R32G32B32_SFLOAT? (12B)
    //   0x30 = R32G32B32A32_SFLOAT? (16B)
    // ‡ Pattern matches NVN's "components-grow-by-1 = enum+1" convention
    // observed in tex formats (0x42-0x47 = BC1-BC6).
    //
    // 0x22 (seen in SetFormat#1, NOT in menu's bound array) — ‡‡.
    // 0x29 (21 instances) — adjacent to 0x25=RGBA8, ‡ = RGBA8_SRGB
    // (texture fmt; not vertex). 0x11/0x12 = R8/RG8.
    //
    // ⚠️ THROW on unknown (silent-fallthrough hides type-misses
    // as wrong-render not error). The exception-text IS the format
    // discriminator for the next iteration.
    // Values verified via menu's 4-attr ground-truth (a test run raw-
    // dump; stride=36, render 90788nz correct with hardcoded
    // VkFormats below = the same fmts now derived from this
    // table = §5-self-check):
    //   0x22 @0  =12B → R32G32B32_SFLOAT (pos)        ✓
    //   0x25 @12 = 4B → R8G8B8A8_UNORM   (color)       ✓
    //   0x25 @16 = 4B → R8G8B8A8_UNORM   (color2)      ✓
    //   0x2e @20 = 8B → R32G32_SFLOAT    (uv)          ✓
    static (uint VkFmt, int Bytes) NvnVtxFmt(int nvn) => nvn switch {
        // ── (c²⁷) byte-arithmetic CORRECTED from sh800/813
        // attr layouts (gap-to-next-attr = true byte-size).
        // sh800 str=16 [0x29@0,0x25@8,0x11@12]: 0x29=8B 0x11=4B.
        // sh813 str=20 [0x22@0,0x25@12,0x11@16]: 0x11=4B again.
        // sh794 str=36 […,0x29@20,0x16@28]: 0x29=8B 0x16=8B.
        // Semantics ‡‡ — size from arithmetic, type guessed
        // (kt[19]: bytes-first, reference-check pending).
        // (T6)×9 sera-steer: k=164 LEGO-logo quad's UV
        // attr (fmt 0x11 @12) raw bytes = 0x3c00 0x3c00 =
        // f16(1.0,1.0) — perfect quad corners. But 89 =
        // VK_FORMAT_R16G16B16_SINT (3-comp signed-INT, the
        // comment "SNORM" was wrong too). Vertex-fetch read
        // f16 UVs as ints → VS got garbage → fs880's
        // diffuse-UV in_attr4 = garbage → flat-blue square
        // instead of LEGO logo. = the VUID-08733 ×49 from
        // (F') validation (which I'd mis-attributed to 0x16
        // = (H2); 89 IS R16G16B16_SINT, exactly what
        // validation named). 83 = R16G16_SFLOAT.
        0x11 => ( 83,  4),  // R16G16_SFLOAT
        0x12 => ( 16,  2),  // R8G8_UNORM                     ‡
        0x16 => ( 91,  8),  // R16G16B16A16_SNORM  ‡‡ (sh794 tangent? 8B by gap)
        // ── float×N (R32) family ───────────────────────────────
        // 0x22 = float×3 CONFIRMED (menu pos@0, 12B). Adjacents
        // ‡-inferred by enum±1 = component±1 pattern.
        0x20 => (100,  4),  // R32_SFLOAT                     ‡
        0x21 => (103,  8),  // R32G32_SFLOAT                  ‡
        0x22 => (106, 12),  // R32G32B32_SFLOAT       ✓ menu pos
        0x23 => (109, 16),  // R32G32B32A32_SFLOAT            ‡
        // ── RGBA8 family ───────────────────────────────────────
        0x25 => ( 37,  4),  // R8G8B8A8_UNORM         ✓ menu color
        // (T6)×33 sera ·10966 botw nvn.h: 0x27=RGBA8UI.
        // vs1023 bone-idx attr: shader does int(floor(
        // in_attr.x)) ⟹ wants float-with-integer-VALUE
        // ⟹ VK_FORMAT_R8G8B8A8_USCALED (33; uint8→float
        // 0..255 verbatim). UINT (41) would need ivec
        // input + the .x-as-float wouldn't work.
        0x27 => ( 33,  4),  // R8G8B8A8_USCALED (NVN: RGBA8UI; bone-idx)
        // (c²⁷) 0x29 in VERTEX context = 8B (sh800 @0, gap-to
        // -next=8). Was R8G8B8A8_SRGB (4B, tex-format guess).
        // sh800 reads 3 floats → quantized pos. ‡‡ SNORM vs
        // SFLOAT undetermined; SFLOAT first (positions can
        // exceed [-1,1]).
        0x29 => ( 97,  8),  // R16G16B16A16_SFLOAT ‡‡ (was RGBA8_SRGB 4B, WRONG by stride-math)
        // ── R32G32 (= 8B; 0x2e was the SAME as 0x21? — no:
        // both seen distinctly; 0x2e@off=20 is the menu's uv,
        // 8B confirmed. ⟹ NVN may have 0x21=R32G32_UINT and
        // 0x2e=R32G32_SFLOAT, OR a stride-N gap in the enum.
        // Treating both as RG32_SFLOAT until disambiguated.) ──
        0x2e => (103,  8),  // R32G32_SFLOAT          ✓ menu uv
        // ── R16 half-float family ──────────────────────────────
        0x34 => ( 97,  8),  // R16G16B16A16_SFLOAT            ‡‡
        // 0 = unset (no SetFormat called for this slot). Caller
        // SKIPS — fmt 0 isn't a real attr.
        0x00 => (  0,  0),
        // (c²⁷) collect-mode: log unknown + default RGBA8_UNORM
        // (= renders wrong-but-doesn't-crash). The set of distinct
        // unknowns from one run = the fill-table input. kt[2]
        // tradeoff: throw stops at first; this collects all.
        _ => NvnVtxFmtUnknown(nvn),
    };
    static readonly HashSet<int> _unkVtxFmt = new();
    static (uint, int) NvnVtxFmtUnknown(int nvn) {
        if(_unkVtxFmt.Add(nvn))
            $"[vk] ‡ NvnVtxFmt UNKNOWN 0x{nvn:x} — defaulting RGBA8_UNORM(4B); add to table".Log();
        return (37, 4);
    }

    [StructLayout(LayoutKind.Sequential)] struct VkVertexInputBindingDescription {
        public uint binding, stride, inputRate;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkVertexInputAttributeDescription {
        public uint location, binding, format, offset;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineVertexInputStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint bindingCount; public VkVertexInputBindingDescription* pBindings;
        public uint attributeCount; public VkVertexInputAttributeDescription* pAttributes;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineInputAssemblyStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint topology, primitiveRestartEnable;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkViewport {
        public float x, y, width, height, minDepth, maxDepth;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkRect2D {
        public int x, y; public uint width, height;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineViewportStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint viewportCount; public VkViewport* pViewports;
        public uint scissorCount; public VkRect2D* pScissors;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineRasterizationStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint depthClampEnable, rasterizerDiscardEnable;
        public uint polygonMode, cullMode, frontFace;
        public uint depthBiasEnable; public float depthBiasConstant, depthBiasClamp, depthBiasSlope;
        public float lineWidth;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineMultisampleStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint rasterizationSamples;
        public uint sampleShadingEnable; public float minSampleShading;
        public void* pSampleMask;
        public uint alphaToCoverageEnable, alphaToOneEnable;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineColorBlendAttachmentState {
        public uint blendEnable;
        public uint srcColorBF, dstColorBF, colorBlendOp;
        public uint srcAlphaBF, dstAlphaBF, alphaBlendOp;
        public uint colorWriteMask;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineColorBlendStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint logicOpEnable, logicOp;
        public uint attachmentCount; public VkPipelineColorBlendAttachmentState* pAttachments;
        public float blendConst0, blendConst1, blendConst2, blendConst3;
    }
    // (c³⁷)(G) Depth-stencil state. depthCompareOp 1=LESS.
    // Without depth, draw-order = visibility ⟹ 100+ indexed
    // meshes painting over each other in submission order =
    // unintelligible. With depth-test, foreground occludes
    // background regardless of draw-order = visible structure
    // even with wrong textures.
    [StructLayout(LayoutKind.Sequential)] struct VkPipelineDepthStencilStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint depthTestEnable, depthWriteEnable, depthCompareOp;
        public uint depthBoundsTestEnable, stencilTestEnable;
        // VkStencilOpState front, back (7×u32 each) — zeroed.
        public fixed uint stencilFront[7], stencilBack[7];
        public float minDepthBounds, maxDepthBounds;
    }
    const uint ST_PIPELINE_DEPTH_STENCIL_CI = 25;
    // (T6)×35: dynamic viewport+scissor (RT-targets
    // range 30×16..1920×1080; static vp baked at
    // pipe-build won't work). VK_DYNAMIC_STATE_VIEWPORT
    // =0, _SCISSOR=1.
    [StructLayout(LayoutKind.Sequential)]
    struct VkPipelineDynamicStateCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint dynamicStateCount; public uint* pDynamicStates;
    }
    const uint ST_PIPELINE_DYNAMIC_STATE_CI = 27;
    [StructLayout(LayoutKind.Sequential)] struct VkGraphicsPipelineCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint stageCount; public VkPipelineShaderStageCreateInfo* pStages;
        public VkPipelineVertexInputStateCreateInfo* pVertexInputState;
        public VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;
        public void* pTessellationState;
        public VkPipelineViewportStateCreateInfo* pViewportState;
        public VkPipelineRasterizationStateCreateInfo* pRasterizationState;
        public VkPipelineMultisampleStateCreateInfo* pMultisampleState;
        public void* pDepthStencilState;
        public VkPipelineColorBlendStateCreateInfo* pColorBlendState;
        public void* pDynamicState;
        public ulong layout, renderPass; public uint subpass;
        public ulong basePipelineHandle; public int basePipelineIndex;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkRenderPassBeginInfo {
        public uint sType; public void* pNext;
        public ulong renderPass, framebuffer;
        public VkRect2D renderArea;
        public uint clearValueCount; public VkClearColorValue* pClearValues;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkBufferCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public ulong size; public uint usage, sharingMode;
        public uint queueFamilyIndexCount; public void* pQueueFamilyIndices;
    }

    const uint ST_SHADER_MODULE_CI = 16, ST_PIPELINE_SHADER_STAGE_CI = 18,
        ST_PIPELINE_VERTEX_INPUT_CI = 19, ST_PIPELINE_INPUT_ASM_CI = 20,
        ST_PIPELINE_VIEWPORT_CI = 22, ST_PIPELINE_RASTER_CI = 23,
        ST_PIPELINE_MULTISAMPLE_CI = 24, ST_PIPELINE_COLOR_BLEND_CI = 26,
        ST_GRAPHICS_PIPELINE_CI = 28, ST_PIPELINE_LAYOUT_CI = 30,
        ST_RENDER_PASS_CI = 38, ST_FRAMEBUFFER_CI = 37, ST_IMAGE_VIEW_CI = 15,
        ST_RENDER_PASS_BI = 43, ST_BUFFER_CI = 12;

    [DllImport(Lib)] static extern int vkCreateRenderPass(
        ulong dev, VkRenderPassCreateInfo* ci, void* alloc, ulong* rp);
    [DllImport(Lib)] static extern int vkCreateImageView(
        ulong dev, VkImageViewCreateInfo* ci, void* alloc, ulong* view);
    [DllImport(Lib)] static extern int vkCreateFramebuffer(
        ulong dev, VkFramebufferCreateInfo* ci, void* alloc, ulong* fb);
    [DllImport(Lib)] static extern int vkCreateShaderModule(
        ulong dev, VkShaderModuleCreateInfo* ci, void* alloc, ulong* sm);
    [DllImport(Lib)] static extern int vkCreatePipelineLayout(
        ulong dev, VkPipelineLayoutCreateInfo* ci, void* alloc, ulong* pl);
    [DllImport(Lib)] static extern int vkCreateGraphicsPipelines(
        ulong dev, ulong cache, uint count, VkGraphicsPipelineCreateInfo* ci,
        void* alloc, ulong* pipelines);
    [DllImport(Lib)] static extern int vkCreateBuffer(
        ulong dev, VkBufferCreateInfo* ci, void* alloc, ulong* buf);
    [DllImport(Lib)] static extern void vkGetBufferMemoryRequirements(
        ulong dev, ulong buf, VkMemoryRequirements* req);
    [DllImport(Lib)] static extern int vkBindBufferMemory(
        ulong dev, ulong buf, ulong mem, ulong off);
    [DllImport(Lib)] static extern void vkCmdBeginRenderPass(
        ulong cb, VkRenderPassBeginInfo* bi, uint contents);
    [DllImport(Lib)] static extern void vkCmdEndRenderPass(ulong cb);
    // (T6)×36: RT-readback. RT images are tiling=OPTIMAL
    // ⟹ vkCmdCopyImageToBuffer → host-mapped staging.
    [StructLayout(LayoutKind.Sequential)]
    struct VkBufferImageCopy {
        public ulong bufferOffset;
        public uint bufferRowLength, bufferImageHeight;
        // VkImageSubresourceLayers (4× u32):
        public uint aspectMask, mipLevel, baseArrayLayer, layerCount;
        // VkOffset3D + VkExtent3D (3× i32 + 3× u32):
        public int ox, oy, oz;
        public uint ew, eh, ed;
    }
    [DllImport(Lib)] static extern void vkCmdCopyImageToBuffer(
        ulong cb, ulong srcImage, uint srcLayout,
        ulong dstBuffer, uint regionCount, VkBufferImageCopy* r);
    [DllImport(Lib)] static extern void vkCmdFillBuffer(
        ulong cb, ulong dstBuffer, ulong dstOffset,
        ulong size, uint data);
    // (T6)×37 ×3: explicit per-attachment clear (loadOp
    // is baked at RP-create; this fires on first-visit
    // -this-frame only).
    [StructLayout(LayoutKind.Sequential)]
    struct VkClearAttachment {
        public uint aspectMask, colorAttachment;
        public VkClearColorValue clearValue;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct VkClearRect {
        public VkRect2D rect;
        public uint baseArrayLayer, layerCount;
    }
    [DllImport(Lib)] static extern void vkCmdClearAttachments(
        ulong cb, uint attachmentCount, VkClearAttachment* a,
        uint rectCount, VkClearRect* r);
    [DllImport(Lib)] static extern void vkCmdSetViewport(
        ulong cb, uint first, uint count, VkViewport* vp);
    [DllImport(Lib)] static extern void vkCmdSetScissor(
        ulong cb, uint first, uint count, VkRect2D* sc);
    [DllImport(Lib)] static extern void vkCmdBindPipeline(
        ulong cb, uint bindPoint, ulong pipeline);
    [DllImport(Lib)] static extern void vkCmdBindVertexBuffers(
        ulong cb, uint firstBinding, uint count, ulong* bufs, ulong* offsets);
    [DllImport(Lib)] static extern void vkCmdPushConstants(
        ulong cb, ulong layout, uint stageFlags, uint offset, uint size, void* values);
    [DllImport(Lib)] static extern void vkCmdDraw(
        ulong cb, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

    // ─── T2 v0.5: descriptor set + sampler + atlas image ───
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorSetLayoutBinding {
        public uint binding, descriptorType, descriptorCount, stageFlags;
        public void* pImmutableSamplers;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorSetLayoutCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint bindingCount; public VkDescriptorSetLayoutBinding* pBindings;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorPoolSize {
        public uint type, descriptorCount;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorPoolCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint maxSets, poolSizeCount; public VkDescriptorPoolSize* pPoolSizes;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorSetAllocateInfo {
        public uint sType; public void* pNext;
        public ulong descriptorPool; public uint descriptorSetCount;
        public ulong* pSetLayouts;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorImageInfo {
        public ulong sampler, imageView; public uint imageLayout;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkDescriptorBufferInfo {
        public ulong buffer, offset, range;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkWriteDescriptorSet {
        public uint sType; public void* pNext;
        public ulong dstSet; public uint dstBinding, dstArrayElement;
        public uint descriptorCount, descriptorType;
        public VkDescriptorImageInfo* pImageInfo;
        public VkDescriptorBufferInfo* pBufferInfo;
        public void* pTexelBufferView;
    }
    [StructLayout(LayoutKind.Sequential)] struct VkSamplerCreateInfo {
        public uint sType; public void* pNext; public uint flags;
        public uint magFilter, minFilter, mipmapMode;
        public uint addressU, addressV, addressW;
        public float mipLodBias; public uint anisotropyEnable; public float maxAnisotropy;
        public uint compareEnable, compareOp;
        public float minLod, maxLod;
        public uint borderColor, unnormalizedCoordinates;
    }

    const uint ST_DESCRIPTOR_SET_LAYOUT_CI = 32, ST_DESCRIPTOR_POOL_CI = 33,
        ST_DESCRIPTOR_SET_AI = 34, ST_WRITE_DESCRIPTOR_SET = 35,
        ST_SAMPLER_CI = 31;
    const uint DESC_TYPE_COMBINED_IMAGE_SAMPLER = 1, DESC_TYPE_UNIFORM_BUFFER = 6,
               DESC_TYPE_UNIFORM_BUFFER_DYNAMIC = 8;

    [DllImport(Lib)] static extern int vkCreateDescriptorSetLayout(
        ulong dev, VkDescriptorSetLayoutCreateInfo* ci, void* alloc, ulong* dsl);
    [DllImport(Lib)] static extern int vkCreateDescriptorPool(
        ulong dev, VkDescriptorPoolCreateInfo* ci, void* alloc, ulong* pool);
    [DllImport(Lib)] static extern int vkAllocateDescriptorSets(
        ulong dev, VkDescriptorSetAllocateInfo* ai, ulong* sets);
    [DllImport(Lib)] static extern void vkUpdateDescriptorSets(
        ulong dev, uint writeCount, VkWriteDescriptorSet* writes,
        uint copyCount, void* copies);
    [DllImport(Lib)] static extern int vkCreateSampler(
        ulong dev, VkSamplerCreateInfo* ci, void* alloc, ulong* sampler);
    [DllImport(Lib)] static extern void vkCmdBindDescriptorSets(
        ulong cb, uint bindPoint, ulong layout, uint firstSet, uint count,
        ulong* sets, uint dynOffCount, void* dynOffs);

    static ulong _renderPass, _pipelineLayout, _pipeline;
    static ulong _dsLayout, _dsPool, _dsAtlas, _sampler;
    static ulong _atlasImg, _atlasMem, _atlasView;

    //  per-texId Vk state. Lazy: first draw with this
    // texId triggers upload+DS-alloc. Keyed by texId (= the
    // pool-idx, NOT TexHandle — same texture w/ different
    // sampler shares the upload).
    record TexVk(ulong Img, ulong Mem, ulong View, ulong Ds,
                 int W, int H, bool BarrierDone,
                 // (c²³)(a) source H + its Gen at upload
                 // time. EnsureTexBound rebuilds if either
                 // changed (= re-register or re-init/upload).
                 NvnLinux.H? Src = null, int Gen = 0);
    static readonly Dictionary<int, TexVk> _texVk = new();
    static readonly List<int> _texPendingBarrier = new();
    // ── T3 T3 state ──
    static ulong _t3Pipeline, _t3PipelineLayout;
    static string _t3ShDir;
    // -2: cache key = (vs, fs, attrib-layout-hash).
    // Same vs/fs pair with different vertex-layout = different
    // pipeline (= the format/offset is baked into VkPipeline).
    // (T6)×35 ×3: key += rtId. rtId=255 = pre-RTT
    // sentinel (renderPass=_renderPass, nC=1, static
    // viewport — = the !_t3Rtt path, identical to
    // pre-×35). rtId∈[0,15) = use _rtFb[rtId].{Rp,NC}
    // + dynamic viewport+scissor (RT dims vary 30×16
    // ..1920×1080). Including rtId in the key means
    // a (vs,fs) pair drawing to both [A]nC=3 and [SHADOW]
    // nC=0 (e.g. vs45/fs287 per nvncap5 census: rt1×55
    // + rt9×423) gets two distinct pipelines, each
    // RP-compatible with its target.
    // (T6)×40 ×1: + noDepth flag. Non-indexed draws
    // under RTT (= all fullscreen postproc: #163 fs50,
    // #164-165, #631 fs111, #636-668 bloom/composite)
    // never depth-test on the real engine (game sets
    // nvnCommandBufferSetDepthStencilState with test=
    // off before each — uncaptured). Verified r105≡
    // r104: #631 (vs63 fullscreen) wrote 0 pixels with
    // depthTest=LEQUAL. Indexed draws (geometry, light-
    // volumes #633-635) DO depth-test. The idxN>0 split
    // that bit me at ×38+×39 is the CORRECT discriminator
    // here (engine convention, not own-code-gap).
    static readonly Dictionary<(int vs, int fs, int vh, byte rt,
        bool nd), ulong> _t3Pipes = new();
    static int VtxHash(NvnLinux.NvnAttrib[] a, NvnLinux.NvnStream[] s) {
        // Cheap structural hash. Order matters (= location N).
        int h = (a?.Length ?? 0) | ((s?.Length ?? 0) << 8);
        if(a != null) foreach(var x in a) h = h*31 + (x.Format << 16 | x.Offset);
        if(s != null) foreach(var x in s) h = h*31 + (x.Stride | x.Divisor << 16);
        return h;
    }

    static ulong LoadShader(string path) {
        if(!File.Exists(path)) return 0;
        var bytes = File.ReadAllBytes(path);
        fixed(byte* p = bytes) {
            var ci = new VkShaderModuleCreateInfo {
                sType = ST_SHADER_MODULE_CI,
                codeSize = (nuint) bytes.Length, pCode = (uint*) p,
            };
            ulong sm; Chk(vkCreateShaderModule(_dev, &ci, null, &sm), $"vkCreateShaderModule({path})");
            return sm;
        }
    }

    // T3 v1.0: build (or fetch cached) pipeline for a (vs#, fs#)
    // pair. The DSLs/layout/cbuf-buffers/desc-sets are SHARED
    // (built once in InitPipeline; SpirvEmit's interface contract
    // is identical across all shaders). Only the shader modules
    // + fixed-state differ. ‡ v1: vertex-layout = the menu's
    // 36B 4-attr (most title-screen draws use different stride
    // → wrong vbuf decode → garbage geometry; per-pair vertex-
    // layout = ). Returns 0 if .spv missing.
    static ulong T3Pipe(int vsIdx, int fsIdx,
                        NvnLinux.NvnAttrib[] nA, NvnLinux.NvnStream[] nS,
                        byte rtId = 255, bool noDepth = false) {
        var vh = VtxHash(nA, nS);
        if(_t3Pipes.TryGetValue((vsIdx, fsIdx, vh, rtId, noDepth),
                out var p))
            return p;
        // (T6)×35 ×3: rtId=255 ⟹ pre-RTT (swap RP, 1
        // color, static vp). Else resolve rf for (rp,
        // nC, dyn-vp). EnsureRtFb is idempotent (cached);
        // calling it here AND at RP-switch is fine.
        var rf = rtId == 255 ? null : EnsureRtFb(rtId);
        var rp_ = rf?.Rp ?? _renderPass;
        var nCb = rf?.NC ?? 1;
        var vsm = LoadShader($"{_t3ShDir}/sh{vsIdx:d4}-t1.bin.spv");
        // (c²⁸)×4 UMBRA_T3_WHITE_FS=1 → substitute const-white
        // FS for ALL pipelines. The kt[33]-correct discriminator
        // for "rasterizing-vs-not" (wireframe was wrong instrument
        // — FS still runs on line-frags, black-on-black). White
        // silhouettes visible ⟹ rasterizing, gap is FS-side (tex/
        // lighting/blend). Still 0px ⟹ VS-side (off-screen/MVP).
        // (c³⁷)×4 UMBRA_T3_DEPTHVIS_FS=1 → gl_FragCoord.z as
        // grayscale (near=bright, far=dark). Proof-of-depth +
        // shows scene structure without per-slot-tex/lighting.
        // (c⁴⁵)(M') UMBRA_T3_FS_OVERRIDE=<path> → use that
        // .spv as FS for ALL pipelines. Generic; precedes
        // the named overrides. The (M') bisect: sh0346 is
        // KNOWN-WORKING-for-menu (executed-sample, 3.21%
        // u742) — if it ALSO 0% for 3D draws ⟹ ALL executed
        // -sample-FS fail for 3D = DS-state-side (per-tex
        // image not sampleable). If >0% ⟹ glslang-SPIR-V-
        // specific (= very strange given (I') identical
        // OpTypeImage/OpImageSample).
        // (c⁴⁰)×4 UMBRA_T3_TEXFETCH_FS=1 → output texelFetch
        // (tex_8, (2,2), 0) directly. Settles tex-path-vs-FS-
        // compute: color ⟹ tex bound+uploaded OK; black ⟹
        // upload/binding broken (ResolveTex/DecodeForUpload/
        // EnsureTexBound).
        var fsOverride = Environment.GetEnvironmentVariable("UMBRA_T3_FS_OVERRIDE")
            ?? (Environment.GetEnvironmentVariable("UMBRA_T3_TEXFETCH_FS") != null
                ? "/tmp/texfetch.frag.spv"
            : Environment.GetEnvironmentVariable("UMBRA_T3_DEPTHVIS_FS") != null
                ? "/tmp/depthvis.frag.spv"
            : Environment.GetEnvironmentVariable("UMBRA_T3_WHITE_FS") != null
                ? "/tmp/white.frag.spv"
            : null);
        var fsm = LoadShader(fsOverride ?? $"{_t3ShDir}/sh{fsIdx:d4}-t2.bin.spv");
        if(vsm == 0 || fsm == 0) {
            if(!L.Quiet) $"[vk] T3Pipe({vsIdx},{fsIdx},vh{vh:x}): .spv missing → skip".Log();
            _t3Pipes[(vsIdx, fsIdx, vh, rtId, noDepth)] = 0;
            return 0;
        }
        var entryName = stackalloc byte[8]; "main\0"u8.CopyTo(new Span<byte>(entryName, 8));
        var stages = stackalloc VkPipelineShaderStageCreateInfo[2];
        stages[0] = new() { sType = ST_PIPELINE_SHADER_STAGE_CI, stage = 0x1, module = vsm, pName = entryName };
        stages[1] = new() { sType = ST_PIPELINE_SHADER_STAGE_CI, stage = 0x10, module = fsm, pName = entryName };
        // -2: build VkVertexInputState from the per-draw
        // captured Attribs/Streams. Falls back to v1's hardcoded
        // 36B/4-attr if nA/nS empty (= UMBRA_VATTR_HOOK off OR
        // game hasn't bound vertex state yet).
        // SpirvEmit puts attr-space[0x80+N*0x10] → in_N (Loc N),
        // so location = NVN attrib index = array position. ‡
        // Multi-stream (binding>0) untested (game uses 1 stream
        // for menu; title may differ).
        var nAttr = nA?.Count(a => a.Format != 0) ?? 0;
        var nStr  = nS?.Length ?? 0;
        VkVertexInputBindingDescription* bindP;
        VkVertexInputAttributeDescription* attrP;
        int bindN, attrN;
        if(nAttr > 0 && nStr > 0) {
            var binds = stackalloc VkVertexInputBindingDescription[nStr];
            for(var i = 0; i < nStr; i++)
                binds[i] = new() {
                    binding = (uint)i, stride = (uint)nS[i].Stride,
                    inputRate = nS[i].Divisor > 0 ? 1u : 0u,
                };
            var attrs = stackalloc VkVertexInputAttributeDescription[nA.Length];
            int j = 0;
            for(var i = 0; i < nA.Length; i++) {
                var (vf, _) = NvnVtxFmt(nA[i].Format);
                if(vf == 0) continue;  // unset slot
                attrs[j++] = new() {
                    location = (uint)i, binding = (uint)nA[i].StreamIdx,
                    format = vf, offset = (uint)nA[i].Offset,
                };
            }
            bindP = binds; bindN = nStr;
            attrP = attrs; attrN = j;
            $"[vk] T3Pipe({vsIdx},{fsIdx},vh{vh:x}): {attrN}attr/{bindN}bind str={nS[0].Stride} [{string.Join(",", nA.Take(attrN).Select(a => $"L{Array.IndexOf(nA,a)}:0x{a.Format:x}@{a.Offset}"))}]".Log();
        } else {
            // v1 fallback: hardcoded menu layout.
            var binding = stackalloc VkVertexInputBindingDescription[1];
            binding[0] = new() { binding = 0, stride = 36, inputRate = 0 };
            var attrs = stackalloc VkVertexInputAttributeDescription[4];
            attrs[0] = new() { location = 0, binding = 0, format = 106, offset = 0 };
            attrs[1] = new() { location = 1, binding = 0, format = 37,  offset = 12 };
            attrs[2] = new() { location = 2, binding = 0, format = 37,  offset = 16 };
            attrs[3] = new() { location = 3, binding = 0, format = 103, offset = 20 };
            bindP = binding; bindN = 1; attrP = attrs; attrN = 4;
        }
        var vi = new VkPipelineVertexInputStateCreateInfo {
            sType = ST_PIPELINE_VERTEX_INPUT_CI,
            bindingCount = (uint)bindN, pBindings = bindP,
            attributeCount = (uint)attrN, pAttributes = attrP,
        };
        var ia = new VkPipelineInputAssemblyStateCreateInfo {
            sType = ST_PIPELINE_INPUT_ASM_CI, topology = 3,
        };
        // y-flipped (NVN y-up → Vulkan y-down).
        var vp = new VkViewport { x = 0, y = H, width = W, height = -H, minDepth = 0, maxDepth = 1 };
        var sc = new VkRect2D { x = 0, y = 0, width = W, height = H };
        var vps = new VkPipelineViewportStateCreateInfo {
            sType = ST_PIPELINE_VIEWPORT_CI,
            viewportCount = 1, pViewports = &vp,
            scissorCount = 1, pScissors = &sc,
        };
        var rs = new VkPipelineRasterizationStateCreateInfo {
            sType = ST_PIPELINE_RASTER_CI,
            polygonMode = 0, cullMode = 0, frontFace = 0, lineWidth = 1.0f,
        };
        var ms = new VkPipelineMultisampleStateCreateInfo {
            sType = ST_PIPELINE_MULTISAMPLE_CI, rasterizationSamples = 1,
        };
        // (c³⁹)(M) blendEnable env-gated. With blend on
        // (SRC_ALPHA), FS α=0 ⟹ invisible. sh0442 outputs
        // α = (…)×cbuf1[0][1].y — cbuf1 is driver-managed
        // and v0-zero-filled ⟹ α=0 for ALL geometry FS that
        // multiply by a c[1] component. NOBLEND surfaces RGB
        // directly (= settles α-vs-RGB).
        // (T6)×35 ×3: nCb attachments (1 pre-RTT; 0 for
        // shadow nC=0; 3 for [A]G-buf). All identical
        // blend-state for now (‡ per-attachment blend
        // from game's nvnBlendState = ×37+).
        var cbAtt = stackalloc
            VkPipelineColorBlendAttachmentState[Math.Max(nCb, 1)];
        for(var ci = 0; ci < Math.Max(nCb, 1); ci++)
            cbAtt[ci] = new() {
                blendEnable = _t3NoBlend ? 0u : 1u,
                colorWriteMask = 0xf,
                srcColorBF = 6, dstColorBF = 7, colorBlendOp = 0,
                srcAlphaBF = 1, dstAlphaBF = 7, alphaBlendOp = 0,
            };
        var cbs = new VkPipelineColorBlendStateCreateInfo {
            sType = ST_PIPELINE_COLOR_BLEND_CI,
            attachmentCount = (uint)nCb, pAttachments = cbAtt,
        };
        // (T6)×35 ×3: dynamic viewport+scissor when RTT
        // (RT dims vary 30×16..1920×1080..1024×4096; the
        // static @vp/sc above are ignored, count must
        // still =1). vkCmdSetViewport/Scissor at RP-switch.
        var dynStates = stackalloc uint[2] { 0, 1 };  // VIEWPORT, SCISSOR
        var dynS = new VkPipelineDynamicStateCreateInfo {
            sType = ST_PIPELINE_DYNAMIC_STATE_CI,
            dynamicStateCount = 2, pDynamicStates = dynStates,
        };
        // (c³⁷)(G) depth-stencil state: test+write enabled,
        // compareOp=LESS. ‡ NVN convention may be GREATER
        // (reversed-Z); start with LESS, flip if everything
        // depth-fails. UMBRA_T3_DEPTH=0 disables (= revert to
        // pre-(G) painters-order behavior).
        // (T6)×40 ×1(ii): noDepth ⟹ disable test+write
        // (fullscreen postproc draws under RTT). Pre-RTT
        // path (rtId=255) never sets noDepth ⟹ unchanged.
        var depthEn = _t3DepthEnable && !noDepth;
        var ds = new VkPipelineDepthStencilStateCreateInfo {
            sType = ST_PIPELINE_DEPTH_STENCIL_CI,
            depthTestEnable = depthEn ? 1u : 0u,
            depthWriteEnable = _t3DepthEnable ? 1u : 0u,
            depthCompareOp = _t3DepthOp,
            maxDepthBounds = 1.0f,
        };
        var gpci = new VkGraphicsPipelineCreateInfo {
            sType = ST_GRAPHICS_PIPELINE_CI,
            stageCount = 2, pStages = stages,
            pVertexInputState = &vi, pInputAssemblyState = &ia,
            pViewportState = &vps, pRasterizationState = &rs,
            pMultisampleState = &ms, pDepthStencilState = &ds,
            pColorBlendState = &cbs,
            // (T6)×35: rp_ = _rtFb[rtId].Rp under RTT,
            // else _renderPass (= pre-RTT). pDynamicState
            // only when RTT (= rtId!=255); pre-RTT keeps
            // static vp ⟹ behavior identical to ×34.
            pDynamicState = rtId == 255 ? null : &dynS,
            layout = _t3PipelineLayout, renderPass = rp_,
            subpass = 0, basePipelineIndex = -1,
        };
        ulong pipe = 0;
        var rc = vkCreateGraphicsPipelines(_dev, 0, 1, &gpci, null, &pipe);
        if(rc != 0) {
            $"[vk] T3Pipe({vsIdx},{fsIdx},rt{rtId}): vkCreateGraphicsPipelines={rc} → 0".Log();
            pipe = 0;
        } else if(rtId != 255) {
            $"[vk] T3Pipe({vsIdx},{fsIdx},rt{rtId}) nC={nCb} rp=0x{rp_:x} dyn-vp → 0x{pipe:x}".Log();
        } else {
            $"[vk] T3Pipe({vsIdx},{fsIdx}) built → 0x{pipe:x}".Log();
        }
        _t3Pipes[(vsIdx, fsIdx, vh, rtId, noDepth)] = pipe;
        return pipe;
    }
    static ulong _t3DslCbuf, _t3DslTex, _t3DsPool;
    // (c²⁶) set=1 binding count (max FS-corpus binding=38)
    // + per-texId DS budget. Each DS = T3TexBindN samplers.
    const int T3TexBindN = 40, T3TexDsMax = 128;
    // (c²⁹)-(c³⁶) master gate — see comment at the sh813
    // block in RecordDrawPass.
    static readonly bool _legoDiag =
        Environment.GetEnvironmentVariable("UMBRA_LEGO_DIAG") != null;
    static bool _c28Dumped;
    static int _c29LastBucket = -1, _c29First;
    static uint _c29PrevC3, _c29PrevC6;
    static int _c30PrevFC, _c30PrevF;
    static float[]? _c32PrevBand;
    static readonly bool _whiteFs =
        Environment.GetEnvironmentVariable("UMBRA_T3_WHITE_FS") != null;
    // (c³⁹)(M) UMBRA_T3_NOBLEND=1 → blendEnable=0 in T3Pipe.
    // Discriminator: real-FS outputs α=0 (⟹ invisible under
    // SRC_ALPHA blend) vs RGB=0 (⟹ black regardless). With
    // blend off, FS RGB writes directly: if color appears,
    // α was the gate; if still black, RGB=0 = (F) per-slot
    // tex (sh0814 samples bindings 16/30/32; current code
    // writes d.TexHandle's image to all 40 ⟹ 16/30/32 get
    // whatever was last-bound, ‡ a 4×4 placeholder or
    // wrong-format → samples 0).
    static readonly bool _t3NoBlend =
        Environment.GetEnvironmentVariable("UMBRA_T3_NOBLEND") != null;
    static readonly bool _t3C1Ones =
        Environment.GetEnvironmentVariable("UMBRA_T3_C1_ONES") != null;
    // (T6)×24 VS-stage c[1][0] = normal-decode scale.
    // vs417 @306/320/324: temp_15/22/24 = fma(in_attr1
    // .{y,x,z}, vp_c1[0].x, −1.0) = the [0,1]→[−1,1]
    // UNORM-normal unpack; K should be 2.0. VS c[1]
    // zero-filled ⟹ all normals = (−1,−1,−1) const ⟹
    // out_attr1 (= SH-lit normal via vp_c5[0-6]) const
    // ≈0.008 (verified r72ip1) ⟹ fs418 lighting≈0.
    // (c⁴⁰) scoped C1_ONES to FS-only bc VS c[1]=all-
    // 1.0 broke menu position; this sets ONLY c1[0].
    // UMBRA_T3_VS_C1_0=V → VS c1[0]=(V,V,V,V).
    static readonly float? _t3VsC10 =
        float.TryParse(Environment.GetEnvironmentVariable(
            "UMBRA_T3_VS_C1_0"), out var v10) ? v10 : null;
    // (T6)×28 fp_c1 heuristic: comma-separated floats
    // → fp_c1[0].x,y,z,w,c1[1].x,y,z. Default per-slot
    // = 1.0 (= C1_ONES). For back-solving the t280
    // auto-normalize divisor (c1[0].w / c1[1].x).
    static readonly float[]? _t3FsC1 =
        Environment.GetEnvironmentVariable("UMBRA_T3_FS_C1")
            is {} fc1
            ? fc1.Split(',').Select(float.Parse).ToArray()
            : null;
    // (T6)×35 RTT render-side master gate. When ON:
    // per-draw d.RtId routes to _rtFb[rtId] (lazy-
    // alloc'd from NvnLinux.RtSigs); RP-switch in
    // per-draw loop; T3Pipe gets per-rtId renderPass
    // + nC colorBlend attachments; viewport dynamic.
    // OFF = pre-RTT single-_renderPass-to-swap.
    static readonly bool _t3Rtt =
        Environment.GetEnvironmentVariable("UMBRA_T3_RTT") != null;
    // (T6)×36 RT-readback: UMBRA_T3_RTT_DUMP=N → after
    // the per-draw loop, vkCmdCopyImageToBuffer
    // _rtFb[N].{Img[0] | DepthImg} → host-staging →
    // DumpRtPpm decodes per-fmt (RGBA8 direct / RGBA16F
    // Reinhard tonemap / D32F depth→gray / RG16 r,g,128)
    // → /tmp/umbra-frame-{N}.ppm (= same path DumpPpm
    // writes, so NvnReplay's rename → replay-f*-i*.ppm
    // works unchanged). r87 DUMP=1 = G-buffer slot-0;
    // r88 DUMP=9 = 1024×4096 cascaded shadow-map.
    static readonly int _t3RttDump =
        int.TryParse(Environment.GetEnvironmentVariable(
            "UMBRA_T3_RTT_DUMP"), out var rd) ? rd : -1;
    static ulong _rtDumpBuf, _rtDumpMem;
    static byte* _rtDumpPtr;
    const int RtDumpBufSz = 32 << 20;  // 32MB ≥ max(1920×1080×8, 1024×4096×4)
    static readonly bool _depthvisFs =
        Environment.GetEnvironmentVariable("UMBRA_T3_DEPTHVIS_FS") != null;
    static readonly HashSet<int>? _skipVs =
        Environment.GetEnvironmentVariable("UMBRA_T3_SKIP_VS") is {} sv
            ? sv.Split(',').Select(int.Parse).ToHashSet() : null;
    static readonly bool _dump3d =
        Environment.GetEnvironmentVariable("UMBRA_DUMP_3D") != null;
    static int _dump3dFirst, _dump3dWant;
    // (c³⁷)(G) depth: default ON; UMBRA_T3_DEPTH=0 reverts to
    // pre-(G) painters-order. UMBRA_T3_DEPTH_OP=N overrides
    // — N is VkCompareOp DIRECTLY: 0=NEVER 1=LESS 2=EQUAL
    // 3=LESS_OR_EQUAL 4=GREATER 5=NOT_EQUAL 6=GREATER_OR_
    // EQUAL 7=ALWAYS. ⚠ kt[36]: 2≠LEQUAL (cost ~25 cycles
    // u744-u760 attributing depth-EQUAL-fail to FS-abort).
    // compareOp (1=LESS default; 4=GREATER for reversed-Z).
    static readonly bool _t3DepthEnable =
        Environment.GetEnvironmentVariable("UMBRA_T3_DEPTH") != "0";
    static readonly uint _t3DepthOp =
        uint.TryParse(Environment.GetEnvironmentVariable(
            "UMBRA_T3_DEPTH_OP"), out var dop) ? dop : 1u;
    static readonly bool _c33ForceSpeed =
        Environment.GetEnvironmentVariable("UMBRA_C33_FORCE_SPEED") != null;
    static int _c33ForceN, _c40LogN, _etbLogN, _txDs0LogN;
    static readonly HashSet<int> _c42SeenVs = new();
    static readonly HashSet<int> _c43Dumped = new();
    static readonly bool _dumpTex =
        Environment.GetEnvironmentVariable("UMBRA_DUMP_TEX") != null;
    // (c³³)×4 patch-test: NOP the +0x348 `cbnz w21` gate
    // (and +0x260 sibling). If c[3] unfreezes ⟹ w21 (=
    // arg-w1 = mpd+40 via Sys::Update) IS the gate; trace
    // mpd+40 source. If still frozen ⟹ another gate.
    static readonly bool _c33Patch =
        Environment.GetEnvironmentVariable("UMBRA_C33_PATCH_W21") != null;
    static readonly bool _c35PatchBit1 =
        Environment.GetEnvironmentVariable("UMBRA_C35_PATCH_BIT1") != null;
    static readonly bool _c36ForceDev =
        Environment.GetEnvironmentVariable("UMBRA_C36_FORCE_DEV") != null;
    static int _c36ForceN;
    static bool _c33Patched;
    [DllImport("libc", SetLastError=true)]
    static extern int mprotect(nint addr, nuint len, int prot);
    [DllImport("libgcc_s.so.1", EntryPoint="__clear_cache")]
    static extern void __clear_cache(nint begin, nint end);
    static uint Hash16(byte[] b) {
        // FNV-1a over first 256B (= the matrix region).
        uint h = 2166136261;
        var n = Math.Min(b.Length, 256);
        for(var i=0;i<n;i++) { h ^= b[i]; h *= 16777619; }
        return h;
    }
    static string FloatHex(byte[] b, int off, int n) {
        var s=""; for(var i=0;i<n && off+i*4+4<=b.Length;i++){
            var f=BitConverter.ToSingle(b,off+i*4);
            s+=$"{f:g6} "; } return s;
    }
    static ulong _t3DsVs, _t3DsTex, _t3DsFs;   // sets 0/1/2
    static ulong[] _t3CbufBuf;        // [1..24] = stage*12 + binding
    static byte*[] _t3CbufPtr;        // mapped ptrs
    // (T6)×7(b) T3MaxDraws 64→256: 796ca500 cold-read HM
    // found the @1988 clamp `slot = recN<64?recN:63` ⟹
    // with 118 idx draws/frame, draws 64-117 ALL write+
    // read slot-63's cbufs (= last-write-wins reintro'd
    // for the back half). Depthvis (no cbuf reads) was
    // unaffected = why r5 looked fine. 256 covers the
    // 198/445-draw cutscene frames. Alloc scales @1304
    // (T3CbufStride × T3MaxDraws × 24 buffers = 24MB).
    const int T3CbufStride = 4096, T3MaxDraws = 256;

    // Allocate + write t3 descriptor sets. Lazy (after _atlasView
    // exists). Set0=VS-cbufs (12 UBO → _t3CbufBuf[1..12]), set1=tex,
    // set2=FS-cbufs (12 UBO → _t3CbufBuf[13..24]).
    static void EnsureT3Sets() {
        if(_t3DsVs != 0 || _t3Pipeline == 0 || _atlasView == 0) return;
        var layouts = stackalloc ulong[3];
        layouts[0] = _t3DslCbuf; layouts[1] = _t3DslTex; layouts[2] = _t3DslCbuf;
        var dsai = new VkDescriptorSetAllocateInfo {
            sType = ST_DESCRIPTOR_SET_AI, descriptorPool = _t3DsPool,
            descriptorSetCount = 3, pSetLayouts = layouts,
        };
        var sets = stackalloc ulong[3];
        if(Chk(vkAllocateDescriptorSets(_dev, &dsai, sets),
               "vkAllocateDescriptorSets(t3)") != 0) return;
        _t3DsVs = sets[0]; _t3DsTex = sets[1]; _t3DsFs = sets[2];

        // Write set0+set2: 12× UNIFORM_BUFFER_DYNAMIC each.
        // range=T3CbufStride (the per-draw WINDOW; the buffer is
        // T3CbufStride×T3MaxDraws total). dynamicOffsets supplied
        // per-draw at vkCmdBindDescriptorSets time = recN*T3CbufStride.
        var bufInfos = stackalloc VkDescriptorBufferInfo[24];
        var writes = stackalloc VkWriteDescriptorSet[25];
        for(uint stage = 0; stage < 2; stage++)
            for(uint b = 1; b <= 12; b++) {
                var idx = stage * 12 + (b - 1);   // 0..23
                bufInfos[idx] = new() {
                    buffer = _t3CbufBuf[stage*12 + b], offset = 0,
                    range = T3CbufStride,
                };
                writes[idx] = new() {
                    sType = ST_WRITE_DESCRIPTOR_SET,
                    dstSet = stage == 0 ? _t3DsVs : _t3DsFs,
                    dstBinding = b, descriptorCount = 1,
                    descriptorType = DESC_TYPE_UNIFORM_BUFFER_DYNAMIC,
                    pBufferInfo = &bufInfos[idx],
                };
            }
        // Write set1: T3TexBindN× COMBINED_IMAGE_SAMPLER →
        // atlas. (c²⁶) was binding=8 only; layout now has
        // 0..39, all must be written.
        var imgInfo = new VkDescriptorImageInfo {
            sampler = _sampler, imageView = _atlasView,
            imageLayout = 1,  // GENERAL (= matches actual; the atlas
                              // is HOST_VISIBLE LINEAR, never
                              // transitioned past GENERAL.)
        };
        var twrites = stackalloc VkWriteDescriptorSet[T3TexBindN];
        for(var b = 0; b < T3TexBindN; b++)
            twrites[b] = new() {
                sType = ST_WRITE_DESCRIPTOR_SET, dstSet = _t3DsTex,
                dstBinding = (uint)b, descriptorCount = 1,
                descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
                pImageInfo = &imgInfo,
            };
        vkUpdateDescriptorSets(_dev, 24, writes, 0, null);
        vkUpdateDescriptorSets(_dev, T3TexBindN, twrites, 0, null);
        $"[vk] EnsureT3Sets OK — vs={_t3DsVs:x} tex={_t3DsTex:x} fs={_t3DsFs:x}".Log();
    }
    static readonly ulong[] _swapView = new ulong[3];
    static readonly ulong[] _swapFb = new ulong[3];
    // One big HOST_VISIBLE vertex buffer; per-draw data memcpy'd in at
    // an advancing offset. ‡ v0 = 4MB fixed, no overflow check.
    static ulong _vbuf, _vbufMem; static byte* _vbufPtr;
    // (c²⁷) bumped 4MB→32MB: indexed draws copy whole bound
    // vbuf chunks (deduped per-frame by VbGpu).
    const ulong VbufSize = 32 << 20;
    static ulong _ibuf, _ibufMem; static byte* _ibufPtr;
    const ulong IbufSize = 16 << 20;
    [DllImport(Lib)] static extern void vkCmdBindIndexBuffer(
        ulong cb, ulong buf, ulong off, uint indexType);
    [DllImport(Lib)] static extern void vkCmdDrawIndexed(
        ulong cb, uint idxCount, uint instCount,
        uint firstIdx, int vtxOff, uint firstInst);
    public static bool PipelineReady;

    // 3 swap images, HOST_VISIBLE-mapped for direct readback.
    static readonly ulong[] _swapImg = new ulong[3];
    static readonly ulong[] _swapMem = new ulong[3];
    static readonly byte*[] _swapPtr = new byte*[3];
    const uint W = 1280, H = 720;
    static uint _hostMemType;

    static bool InitImages() {
        // Find a HOST_VISIBLE | HOST_COHERENT memory type.
        VkPhysicalDeviceMemoryProperties mp;
        vkGetPhysicalDeviceMemoryProperties(_phys, &mp);
        _hostMemType = 0;
        for(var i = 0u; i < mp.memoryTypeCount; i++) {
            // memoryTypes packed: low u32 = propertyFlags, high u32 = heapIndex
            var flags = (uint)(mp.memoryTypes[i] & 0xffffffff);
            if((flags & 0x6) == 0x6) { _hostMemType = i; break; }  // HOST_VISIBLE|HOST_COHERENT
        }

        for(var i = 0; i < 3; i++) {
            var ici = new VkImageCreateInfo {
                sType = ST_IMAGE_CI, imageType = 1, format = 37,  // 2D, R8G8B8A8_UNORM
                width = W, height = H, depth = 1,
                mipLevels = 1, arrayLayers = 1, samples = 1,
                tiling = 1,            // LINEAR for direct host readback
                usage = 0x3 | 0x10,    // TRANSFER_SRC|DST + COLOR_ATTACHMENT
                initialLayout = 0,
            };
            ulong img;
            if(Chk(vkCreateImage(_dev, &ici, null, &img), $"vkCreateImage[{i}]") != 0)
                return false;
            _swapImg[i] = img;

            VkMemoryRequirements req;
            vkGetImageMemoryRequirements(_dev, img, &req);
            // ‡ v0.5: assume _hostMemType is in req.memoryTypeBits (lavapipe = one heap, all types).
            var mai = new VkMemoryAllocateInfo {
                sType = ST_MEM_AI, allocationSize = req.size, memoryTypeIndex = _hostMemType,
            };
            ulong mem;
            if(Chk(vkAllocateMemory(_dev, &mai, null, &mem), $"vkAllocateMemory[{i}]") != 0)
                return false;
            _swapMem[i] = mem;
            Chk(vkBindImageMemory(_dev, img, mem, 0), $"vkBindImageMemory[{i}]");

            void* p;
            Chk(vkMapMemory(_dev, mem, 0, req.size, 0, &p), $"vkMapMemory[{i}]");
            _swapPtr[i] = (byte*) p;
        }
        $"[vk] InitImages OK — 3× {W}×{H} R8G8B8A8 LINEAR HOST_VISIBLE (memType={_hostMemType})".Log();
        return true;
    }

    // T2 v0: render pass + framebuffers + fixed-shader pipeline + a
    // host-visible vertex buffer. Renders directly into the LINEAR swap
    // images (lavapipe doesn't care about tiling for color attachments;
    // ‡ verified-by-trying — if vkCreateFramebuffer or the draw fails,
    // fallback = OPTIMAL render target + blit, wall-N).
    // (c³⁷)(G) depth image (1× shared across swap-fbs; clears
    // each frame via loadOp). D32_SFLOAT=126, OPTIMAL tiling,
    // usage=DEPTH_STENCIL_ATTACHMENT.
    static ulong _depthImg, _depthMem, _depthView;
    static bool InitPipeline() {
        // ── (c³⁷)(G) depth image: D32_SFLOAT W×H OPTIMAL ──
        {
            var dci = new VkImageCreateInfo {
                sType = ST_IMAGE_CI, imageType = 1, format = 126,  // D32_SFLOAT
                width = W, height = H, depth = 1,
                mipLevels = 1, arrayLayers = 1, samples = 1,
                tiling = 0,      // OPTIMAL (depth needs it)
                usage = 0x20,    // DEPTH_STENCIL_ATTACHMENT_BIT
            };
            ulong di; Chk(vkCreateImage(_dev, &dci, null, &di), "vkCreateImage(depth)");
            _depthImg = di;
            VkMemoryRequirements dr; vkGetImageMemoryRequirements(_dev, di, &dr);
            var dai = new VkMemoryAllocateInfo {
                sType = ST_MEM_AI, allocationSize = dr.size,
                memoryTypeIndex = _hostMemType,
            };
            ulong dm; Chk(vkAllocateMemory(_dev, &dai, null, &dm), "vkAllocateMemory(depth)");
            _depthMem = dm;
            Chk(vkBindImageMemory(_dev, di, dm, 0), "vkBindImageMemory(depth)");
            var dvci = new VkImageViewCreateInfo {
                sType = ST_IMAGE_VIEW_CI, image = di,
                viewType = 1, format = 126,
                subresourceRange = new() {
                    aspectMask = 2,  // DEPTH_BIT
                    levelCount = 1, layerCount = 1,
                },
            };
            ulong dv; Chk(vkCreateImageView(_dev, &dvci, null, &dv), "vkCreateImageView(depth)");
            _depthView = dv;
        }
        // ── render pass: color + depth, both LOAD_CLEAR ──
        var atts = stackalloc VkAttachmentDescription[2];
        atts[0] = new() {
            format = 37, samples = 1,           // R8G8B8A8_UNORM
            loadOp = 1, storeOp = 0,            // CLEAR, STORE
            stencilLoadOp = 2, stencilStoreOp = 1,
            initialLayout = 0, finalLayout = 1, // UNDEFINED → GENERAL
        };
        atts[1] = new() {                       // (c³⁷)(G) depth
            format = 126, samples = 1,          // D32_SFLOAT
            loadOp = 1, storeOp = 1,            // CLEAR, DONT_CARE
            stencilLoadOp = 2, stencilStoreOp = 1,
            initialLayout = 0, finalLayout = 3, // → DEPTH_STENCIL_ATTACHMENT_OPTIMAL
        };
        var colorRef = new VkAttachmentReference { attachment = 0, layout = 2 };  // COLOR_ATTACHMENT_OPTIMAL
        var depthRef = new VkAttachmentReference { attachment = 1, layout = 3 };  // DEPTH_STENCIL_ATTACHMENT_OPTIMAL
        var subpass = new VkSubpassDescription {
            pipelineBindPoint = 0,              // GRAPHICS
            colorCount = 1, pColors = &colorRef,
            pDepthStencil = &depthRef,
        };
        var rpci = new VkRenderPassCreateInfo {
            sType = ST_RENDER_PASS_CI,
            attachmentCount = 2, pAttachments = atts,
            subpassCount = 1, pSubpasses = &subpass,
        };
        ulong rp;
        if(Chk(vkCreateRenderPass(_dev, &rpci, null, &rp), "vkCreateRenderPass") != 0)
            return false;
        _renderPass = rp;

        // ── image views + framebuffers over the 3 swap images ──
        // (c³⁷)(G) framebuffers now 2-att: [swapView[i], depthView].
        // Single shared depth image (= all 3 fbs share one depth;
        // OK because we render+wait synchronously per-frame, no
        // overlap).
        for(var i = 0; i < 3; i++) {
            var ivci = new VkImageViewCreateInfo {
                sType = ST_IMAGE_VIEW_CI, image = _swapImg[i],
                viewType = 1, format = 37,      // 2D, R8G8B8A8_UNORM
                subresourceRange = new() { aspectMask = 1, levelCount = 1, layerCount = 1 },
            };
            ulong v; Chk(vkCreateImageView(_dev, &ivci, null, &v), $"vkCreateImageView[{i}]");
            _swapView[i] = v;
            var fbAtts = stackalloc ulong[2] { v, _depthView };
            var fbci = new VkFramebufferCreateInfo {
                sType = ST_FRAMEBUFFER_CI, renderPass = _renderPass,
                attachmentCount = 2, pAttachments = fbAtts,
                width = W, height = H, layers = 1,
            };
            ulong fb; Chk(vkCreateFramebuffer(_dev, &fbci, null, &fb), $"vkCreateFramebuffer[{i}]");
            _swapFb[i] = fb;
        }

        // ── shader modules ──
        var vsm = LoadShader("/tmp/fixed.vert.spv");
        var fsm = LoadShader("/tmp/fixed.frag.spv");
        if(vsm == 0 || fsm == 0) return false;

        // ── descriptor set layout: 1 combined image sampler @ binding 0 ──
        // (v0.5 textured FS samples uAtlas at vUV).
        var dslb = new VkDescriptorSetLayoutBinding {
            binding = 0, descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
            descriptorCount = 1, stageFlags = 0x10,  // FRAGMENT
        };
        var dslci = new VkDescriptorSetLayoutCreateInfo {
            sType = ST_DESCRIPTOR_SET_LAYOUT_CI, bindingCount = 1, pBindings = &dslb,
        };
        ulong dsl; Chk(vkCreateDescriptorSetLayout(_dev, &dslci, null, &dsl), "vkCreateDescriptorSetLayout");
        _dsLayout = dsl;

        var dps = new VkDescriptorPoolSize { type = DESC_TYPE_COMBINED_IMAGE_SAMPLER, descriptorCount = 4 };
        var dpci = new VkDescriptorPoolCreateInfo {
            sType = ST_DESCRIPTOR_POOL_CI, maxSets = 4, poolSizeCount = 1, pPoolSizes = &dps,
        };
        ulong dpool; Chk(vkCreateDescriptorPool(_dev, &dpci, null, &dpool), "vkCreateDescriptorPool");
        _dsPool = dpool;

        // ── sampler: linear filter, clamp ──
        var sci = new VkSamplerCreateInfo {
            sType = ST_SAMPLER_CI, magFilter = 1, minFilter = 1,  // LINEAR
            addressU = 2, addressV = 2, addressW = 2,             // CLAMP_TO_EDGE
            maxLod = 0,
        };
        ulong samp; Chk(vkCreateSampler(_dev, &sci, null, &samp), "vkCreateSampler");
        _sampler = samp;

        // ── pipeline layout: 1 set + push constants (mat4 proj + mat4 model = 128B) ──
        var pcr = new VkPushConstantRange { stageFlags = 0x1, offset = 0, size = 128 };  // VERTEX
        var plci = new VkPipelineLayoutCreateInfo {
            sType = ST_PIPELINE_LAYOUT_CI,
            setLayoutCount = 1, pSetLayouts = &dsl,
            pushConstantRangeCount = 1, pPushConstantRanges = &pcr,
        };
        ulong pl; Chk(vkCreatePipelineLayout(_dev, &plci, null, &pl), "vkCreatePipelineLayout");
        _pipelineLayout = pl;

        // ── graphics pipeline ──
        var entryName = stackalloc byte[8]; "main\0"u8.CopyTo(new Span<byte>(entryName, 8));
        var stages = stackalloc VkPipelineShaderStageCreateInfo[2];
        stages[0] = new() { sType = ST_PIPELINE_SHADER_STAGE_CI, stage = 0x1, module = vsm, pName = entryName };  // VERTEX
        stages[1] = new() { sType = ST_PIPELINE_SHADER_STAGE_CI, stage = 0x10, module = fsm, pName = entryName }; // FRAGMENT

        // Vertex input: binding 0 stride 36, attrs match game layout
        // (pos f3 @0, c0 unorm8×4 @12, c1 unorm8×4 @16, uv f2 @20).
        var binding = new VkVertexInputBindingDescription { binding = 0, stride = 36, inputRate = 0 };
        var attrs = stackalloc VkVertexInputAttributeDescription[4];
        attrs[0] = new() { location = 0, binding = 0, format = 106, offset = 0 };   // R32G32B32_SFLOAT
        attrs[1] = new() { location = 1, binding = 0, format = 37,  offset = 12 };  // R8G8B8A8_UNORM
        attrs[2] = new() { location = 2, binding = 0, format = 37,  offset = 16 };
        attrs[3] = new() { location = 3, binding = 0, format = 103, offset = 20 };  // R32G32_SFLOAT
        var vi = new VkPipelineVertexInputStateCreateInfo {
            sType = ST_PIPELINE_VERTEX_INPUT_CI,
            bindingCount = 1, pBindings = &binding,
            attributeCount = 4, pAttributes = attrs,
        };
        var ia = new VkPipelineInputAssemblyStateCreateInfo {
            sType = ST_PIPELINE_INPUT_ASM_CI, topology = 3,  // TRIANGLE_LIST
        };
        // T2 viewport = unflipped (its fixed-shader does -p.y).
        // T3 gets a separate flipped viewport (NVN y-up → Vulkan
        // y-down) at t3gpci-build time below.
        var vp = new VkViewport { x = 0, y = 0, width = W, height = H, minDepth = 0, maxDepth = 1 };
        var sc = new VkRect2D { x = 0, y = 0, width = W, height = H };
        var vps = new VkPipelineViewportStateCreateInfo {
            sType = ST_PIPELINE_VIEWPORT_CI,
            viewportCount = 1, pViewports = &vp,
            scissorCount = 1, pScissors = &sc,
        };
        var rs = new VkPipelineRasterizationStateCreateInfo {
            sType = ST_PIPELINE_RASTER_CI,
            // (c²⁷)×6 UMBRA_T3_WIREFRAME=1 → polygonMode=LINE.
            // Discriminator: do the big indexed meshes (sh800/
            // 813) RASTERIZE (= edges visible ⟹ FS-side issue:
            // tex/lighting/blend) or NOT (= VS-side: vtx-fmt/
            // cbuf/transform)? ‡ Needs fillModeNonSolid feature;
            // lavapipe has it.
            polygonMode = Environment.GetEnvironmentVariable(
                "UMBRA_T3_WIREFRAME") != null ? 1u : 0u,
            cullMode = 0, frontFace = 0,  // NONE, CCW
            lineWidth = 1.0f,
        };
        var ms = new VkPipelineMultisampleStateCreateInfo {
            sType = ST_PIPELINE_MULTISAMPLE_CI, rasterizationSamples = 1,
        };
        // v0.5: alpha blend (src_α, 1−src_α) so the font atlas's BC3
        // alpha channel anti-aliases glyph edges over the clear color.
        var cbAtt = new VkPipelineColorBlendAttachmentState {
            blendEnable = 1, colorWriteMask = 0xf,
            srcColorBF = 6, dstColorBF = 7, colorBlendOp = 0,  // SRC_ALPHA, ONE_MINUS_SRC_ALPHA, ADD
            srcAlphaBF = 1, dstAlphaBF = 7, alphaBlendOp = 0,  // ONE, ONE_MINUS_SRC_ALPHA
        };
        var cbs = new VkPipelineColorBlendStateCreateInfo {
            sType = ST_PIPELINE_COLOR_BLEND_CI,
            attachmentCount = 1, pAttachments = &cbAtt,
        };
        // (c³⁷)(G) legacy fixed-pipeline = 2D blit (menu-atlas
        // path); no depth wanted. But renderpass now has depth
        // attachment ⟹ MUST supply pDepthStencilState (vk1.0
        // §9.2: required when subpass has depth attachment).
        // depthTestEnable=0 = pass-through.
        var ds0 = new VkPipelineDepthStencilStateCreateInfo {
            sType = ST_PIPELINE_DEPTH_STENCIL_CI,
            maxDepthBounds = 1.0f,
        };
        var gpci = new VkGraphicsPipelineCreateInfo {
            sType = ST_GRAPHICS_PIPELINE_CI,
            stageCount = 2, pStages = stages,
            pVertexInputState = &vi, pInputAssemblyState = &ia,
            pViewportState = &vps, pRasterizationState = &rs,
            pMultisampleState = &ms, pDepthStencilState = &ds0,
            pColorBlendState = &cbs,
            layout = _pipelineLayout, renderPass = _renderPass, subpass = 0,
            basePipelineIndex = -1,
        };
        ulong pipe;
        if(Chk(vkCreateGraphicsPipelines(_dev, 0, 1, &gpci, null, &pipe),
               "vkCreateGraphicsPipelines") != 0) return false;
        _pipeline = pipe;

        // ════════════════════════════════════════════════════════════
        // T3 T3: second pipeline using the GAME'S compiled
        // shaders (sh0349 VS + sh0346 FS = the menu pair, compiled by
        // MaxwellGenerator from captured SASS bytes). Gated on
        // UMBRA_T3=1 so T2 fixed-shader path stays working for A/B.
        // Interface (per sh0349/sh0346 SPIR-V dis @ b9cc5a6):
        // VS in: Loc 0 (pos.xyz) + Loc 1 (c0.a only) + Loc 2 (c1.xyz)
        // + Loc 3 (uv.xy) — same 36B vertex layout, REUSE attrs.
        // VS out / FS in: Loc 0 (uv+, comp 0-3) + Loc 8 (color, comp 0-3)
        // gl_Position / gl_FragCoord
        // cbuf: set=0 binding={1,3,4,5,6} ← NVN slots {?,0,1,2,3}.
        // c[1] = driver-managed (vertex-base/RT-scale); v0
        // feeds zeros. c[3..6] = NVN slot 0..3 snapshots.
        // tex: set=1 binding=8 ← the atlas (existing _atlasView).
        // FS out: Loc 0 oColor.
        // ════════════════════════════════════════════════════════════
        // T3 = use the game's actual compiled Maxwell shaders,
        // recompiled to SPIR-V by an external Maxwell→SPIR-V
        // compiler. Contract: NvnLinux dumps each game shader to
        // {UMBRA_SHADER_DIR}/sh{N:04}-t{stage}.bin; the external
        // compiler writes {same}.spv; T3 loads the .spv. v0:
        // hardcoded to the menu's VS+FS pair (sh0349/sh0346); the
        // generalization (per-draw shader-pair lookup) follows
        // once more pairs are exercised.
        var t3mode = Environment.GetEnvironmentVariable("UMBRA_T3");
        var shDir = Environment.GetEnvironmentVariable("UMBRA_SHADER_DIR")
                    ?? "/tmp/umbra-shaders";
        if(t3mode == "1") {
            $"[vk] T3: building shared layouts (pipeline-per-pair built lazily)".Log();
            _t3ShDir = shDir;
            // v1.0: DSLs + layout + cbuf-buffers + pool built ONCE
            // (shader-independent: SpirvEmit's interface contract
            // is identical across all shaders — set0/2 = 12 cbuf
            // bindings each, set1 = tex binding-8). The PIPELINE
            // (= shader modules + fixed state) is built lazily
            // per (vs#,fs#) pair on first use in RecordDrawPass.
            {
                // ── Desc set layouts: 0=VS-cbufs, 1=tex, 2=FS-cbufs ──
                // NVN cbufs are per-(stage, slot) — VS c[5] (model mat4)
                // and FS c[5] (α-threshold) are DIFFERENT data
                // (CONFIRMED at umbra72). SpirvEmit puts VS cbufs at
                // set=0, FS cbufs at set=2. Both layouts identical
                // (binding 1..12 = uniform-buffer); same DSL reused.
                // UNIFORM_BUFFER_DYNAMIC for all 12 cbuf bindings:
                // each underlying buffer holds N draws' worth of
                // data (4096-aligned per draw; 4096 = max draws ×
                // cbuf-size, but really nb≤256 typical). Per-draw,
                // vkCmdBindDescriptorSets passes pDynamicOffsets[12]
                // = recN*4096 for each binding. = the per-draw
                // cbuf isolation fix (last-write-wins). (All 12
                // dynamic even though only c[5] varies, since the
                // dynamic-offset count must match the layout's
                // dynamic-binding count exactly.)
                var t3dslCbufB = stackalloc VkDescriptorSetLayoutBinding[12];
                for(uint k = 0; k < 12; k++)
                    t3dslCbufB[k] = new() {
                        binding = k + 1, descriptorType = DESC_TYPE_UNIFORM_BUFFER_DYNAMIC,
                        descriptorCount = 1, stageFlags = 0x11,
                    };
                var t3dslCbufCi = new VkDescriptorSetLayoutCreateInfo {
                    sType = ST_DESCRIPTOR_SET_LAYOUT_CI,
                    bindingCount = 12, pBindings = t3dslCbufB,
                };
                ulong t3dslCbuf;
                Chk(vkCreateDescriptorSetLayout(_dev, &t3dslCbufCi, null, &t3dslCbuf),
                    "vkCreateDescriptorSetLayout(t3 cbuf)");

                // set=1: combined samplers. (c²⁶) was single
                // binding=8 (sh0346's only tex). sh0140 (first
                // 3D shader) declares 7 set=1 bindings {8,12,
                // 14,18,20,22,30}; max across full 1036-FS
                // corpus = 38 (sh0357). vkCreateGraphicsPipelines
                // validates shader-interface against layout ⟹
                // missing bindings → lavapipe walks unbound
                // descriptors → segv/hang at T3Pipe(63,140).
                // v1: declare 0..39 (T3TexBindN), all FRAGMENT
                // combined-image-sampler. EnsureTexBound writes
                // the SAME image to all 40 (= won't crash; non-8
                // slots sample wrong texture; per-slot resolution
                // = (c²⁷) when DrawRecord captures the full
                // texHandle table per-draw, not just slot-0).
                var t3dsl1b = stackalloc VkDescriptorSetLayoutBinding[T3TexBindN];
                for(var b = 0; b < T3TexBindN; b++)
                    t3dsl1b[b] = new() {
                        binding = (uint)b,
                        descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
                        descriptorCount = 1, stageFlags = 0x10,
                    };
                var t3dsl1ci = new VkDescriptorSetLayoutCreateInfo {
                    sType = ST_DESCRIPTOR_SET_LAYOUT_CI,
                    bindingCount = T3TexBindN, pBindings = t3dsl1b,
                };
                ulong t3dsl1;
                Chk(vkCreateDescriptorSetLayout(_dev, &t3dsl1ci, null, &t3dsl1),
                    "vkCreateDescriptorSetLayout(t3 tex)");

                // ── Pipeline layout: 3 sets {VS-cbuf, tex, FS-cbuf} ──
                var t3sets = stackalloc ulong[3];
                t3sets[0] = t3dslCbuf; t3sets[1] = t3dsl1; t3sets[2] = t3dslCbuf;
                var t3plci = new VkPipelineLayoutCreateInfo {
                    sType = ST_PIPELINE_LAYOUT_CI,
                    setLayoutCount = 3, pSetLayouts = t3sets,
                };
                ulong t3pl; Chk(vkCreatePipelineLayout(_dev, &t3plci, null, &t3pl),
                    "vkCreatePipelineLayout(t3)");
                _t3PipelineLayout = t3pl;
                _t3DslCbuf = t3dslCbuf; _t3DslTex = t3dsl1;

                // (Pipeline built lazily per-pair; see T3Pipe().)
                _t3Pipeline = 1;  // = "T3 enabled" sentinel

                // ── 24× host-mapped cbuf VkBuffers, each
                // T3CbufStride × T3MaxDraws bytes. Per-draw, data
                // for draw#k goes at byte k*T3CbufStride in EVERY
                // buffer; vkCmdBindDescriptorSets passes
                // pDynamicOffsets[12]={k*T3CbufStride,…} so each
                // draw sees its own slice (= per-draw cbuf
                // isolation; was last-write-wins). Index =
                // stage*12 + binding. [1..12]=VS set0, [13..24]=FS
                // set2. ──
                _t3CbufBuf = new ulong[25];
                _t3CbufPtr = new byte*[25];
                for(var n = 1; n <= 24; n++) {
                    var sz = (ulong)(T3CbufStride * T3MaxDraws);
                    var cbci = new VkBufferCreateInfo {
                        sType = ST_BUFFER_CI, size = sz,
                        usage = 0x10,  // UNIFORM_BUFFER_BIT
                    };
                    ulong cb; Chk(vkCreateBuffer(_dev, &cbci, null, &cb), $"vkCreateBuffer(cbuf{n})");
                    VkMemoryRequirements cr; vkGetBufferMemoryRequirements(_dev, cb, &cr);
                    var cmai = new VkMemoryAllocateInfo {
                        sType = ST_MEM_AI, allocationSize = cr.size,
                        memoryTypeIndex = _hostMemType,
                    };
                    ulong cm; Chk(vkAllocateMemory(_dev, &cmai, null, &cm), $"vkAlloc(cbuf{n})");
                    Chk(vkBindBufferMemory(_dev, cb, cm, 0), $"vkBind(cbuf{n})");
                    void* cp; Chk(vkMapMemory(_dev, cm, 0, sz, 0, &cp), $"vkMap(cbuf{n})");
                    _t3CbufBuf[n] = cb; _t3CbufPtr[n] = (byte*) cp;
                    new Span<byte>(cp, (int)sz).Clear();
                }

                // ── Descriptor pool: 24 UBO_DYNAMIC + samplers ──
                // (c²⁶) Each set=1 DS now has T3TexBindN bindings
                // (was 1). 1 shared atlas DS + up to T3TexDsMax
                // per-texId DSs from EnsureTexBound, each
                // consuming T3TexBindN sampler descriptors.
                var t3ps = stackalloc VkDescriptorPoolSize[2];
                t3ps[0] = new() { type = DESC_TYPE_UNIFORM_BUFFER_DYNAMIC, descriptorCount = 24 };
                t3ps[1] = new() { type = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
                                  descriptorCount = (uint)((1 + T3TexDsMax) * T3TexBindN) };
                var t3dpci = new VkDescriptorPoolCreateInfo {
                    sType = ST_DESCRIPTOR_POOL_CI,
                    maxSets = (uint)(3 + T3TexDsMax),
                    poolSizeCount = 2, pPoolSizes = t3ps,
                };
                ulong t3dp; Chk(vkCreateDescriptorPool(_dev, &t3dpci, null, &t3dp),
                    "vkCreateDescriptorPool(t3)");
                _t3DsPool = t3dp;
                // (Sets allocated + written lazily in EnsureT3Sets.)
            }
        }

        // ── one big host-visible vertex buffer ──
        var bci = new VkBufferCreateInfo {
            sType = ST_BUFFER_CI, size = VbufSize,
            usage = 0x80,  // VERTEX_BUFFER_BIT
        };
        ulong buf; Chk(vkCreateBuffer(_dev, &bci, null, &buf), "vkCreateBuffer(vbuf)");
        _vbuf = buf;
        VkMemoryRequirements req; vkGetBufferMemoryRequirements(_dev, buf, &req);
        var mai = new VkMemoryAllocateInfo {
            sType = ST_MEM_AI, allocationSize = req.size, memoryTypeIndex = _hostMemType,
        };
        ulong mem; Chk(vkAllocateMemory(_dev, &mai, null, &mem), "vkAllocateMemory(vbuf)");
        _vbufMem = mem;
        Chk(vkBindBufferMemory(_dev, buf, mem, 0), "vkBindBufferMemory(vbuf)");
        void* p; Chk(vkMapMemory(_dev, mem, 0, req.size, 0, &p), "vkMapMemory(vbuf)");
        _vbufPtr = (byte*) p;

        // ── (c²⁷) one big host-visible index buffer ──
        var ibci = new VkBufferCreateInfo {
            sType = ST_BUFFER_CI, size = IbufSize,
            usage = 0x40,  // INDEX_BUFFER_BIT
        };
        ulong ibuf; Chk(vkCreateBuffer(_dev, &ibci, null, &ibuf), "vkCreateBuffer(ibuf)");
        _ibuf = ibuf;
        VkMemoryRequirements ireq; vkGetBufferMemoryRequirements(_dev, ibuf, &ireq);
        var imai = new VkMemoryAllocateInfo {
            sType = ST_MEM_AI, allocationSize = ireq.size, memoryTypeIndex = _hostMemType,
        };
        ulong imem; Chk(vkAllocateMemory(_dev, &imai, null, &imem), "vkAllocateMemory(ibuf)");
        _ibufMem = imem;
        Chk(vkBindBufferMemory(_dev, ibuf, imem, 0), "vkBindBufferMemory(ibuf)");
        void* ip; Chk(vkMapMemory(_dev, imem, 0, ireq.size, 0, &ip), "vkMapMemory(ibuf)");
        _ibufPtr = (byte*) ip;

        $"[vk] InitPipeline OK — rp={_renderPass:x} pipeline={_pipeline:x} vbuf={_vbuf:x} ({VbufSize>>10}KB host-mapped) dsl={_dsLayout:x} sampler={_sampler:x}".Log();
        PipelineReady = true;

        // ── Present-watchdog ──────────────────────────────────
        // Polls every 1s; if _frameN (= Present count) hasn't
        // moved for >5s, dump CondVarKernel state + per-thread
        // managed stacks. = the instrument for the cv-race
        // hang (A/B runs/A/B runs all-threads-futex_wait; can't reason
        // about it from outside, dump from inside).
        if(Environment.GetEnvironmentVariable("UMBRA_WATCHDOG") != null)
            new Thread(Watchdog) { IsBackground = true, Name = "umbra-wd" }.Start();
        CondVarKernel.StartWatchpoint();  // UMBRA_WATCHPOINT_M1
        return true;
    }

    // Lazy atlas upload + descriptor set allocation. Called from
    // RecordDrawPass once NvnLinux.AtlasRgba is populated (= after the
    // game's CopyBufferToTexture #2 fires, ~frame 30). v0.5 = HOST_VISIBLE
    // LINEAR image, memcpy directly (lavapipe samples LINEAR fine; saves
    // a staging-buffer + copy-cmd). Records its own one-shot transition.
    static bool _atlasReady;
    static void EnsureAtlasBound() {
        if(_atlasReady || NvnLinux.AtlasRgba == null) return;
        var aw = (uint) NvnLinux.AtlasW; var ah = (uint) NvnLinux.AtlasH;

        var ici = new VkImageCreateInfo {
            sType = ST_IMAGE_CI, imageType = 1, format = 37,
            width = aw, height = ah, depth = 1,
            mipLevels = 1, arrayLayers = 1, samples = 1,
            tiling = 1,             // LINEAR — direct memcpy
            usage = 0x4 | 0x2,      // SAMPLED | TRANSFER_DST
            initialLayout = 0,
        };
        ulong img; if(Chk(vkCreateImage(_dev, &ici, null, &img), "vkCreateImage(atlas)") != 0) return;
        _atlasImg = img;
        VkMemoryRequirements req; vkGetImageMemoryRequirements(_dev, img, &req);
        var mai = new VkMemoryAllocateInfo {
            sType = ST_MEM_AI, allocationSize = req.size, memoryTypeIndex = _hostMemType,
        };
        ulong mem; if(Chk(vkAllocateMemory(_dev, &mai, null, &mem), "vkAllocateMemory(atlas)") != 0) return;
        _atlasMem = mem;
        Chk(vkBindImageMemory(_dev, img, mem, 0), "vkBindImageMemory(atlas)");
        // memcpy decoded RGBA in. ‡ v0.5: assume rowPitch == aw*4 for
        // LINEAR R8G8B8A8 on lavapipe (verified for swap images already).
        void* p; Chk(vkMapMemory(_dev, mem, 0, req.size, 0, &p), "vkMapMemory(atlas)");
        NvnLinux.AtlasRgba.AsSpan().CopyTo(new Span<byte>(p, (int)(aw*ah*4)));

        // Image view.
        var ivci = new VkImageViewCreateInfo {
            sType = ST_IMAGE_VIEW_CI, image = img, viewType = 1, format = 37,
            subresourceRange = new() { aspectMask = 1, levelCount = 1, layerCount = 1 },
        };
        ulong v; Chk(vkCreateImageView(_dev, &ivci, null, &v), "vkCreateImageView(atlas)");
        _atlasView = v;

        // Allocate + write descriptor set.
        ulong dsl = _dsLayout;
        var dsai = new VkDescriptorSetAllocateInfo {
            sType = ST_DESCRIPTOR_SET_AI, descriptorPool = _dsPool,
            descriptorSetCount = 1, pSetLayouts = &dsl,
        };
        ulong ds; if(Chk(vkAllocateDescriptorSets(_dev, &dsai, &ds), "vkAllocateDescriptorSets(atlas)") != 0) return;
        _dsAtlas = ds;
        var dii = new VkDescriptorImageInfo {
            sampler = _sampler, imageView = _atlasView, imageLayout = 1,  // GENERAL
        };
        var wds = new VkWriteDescriptorSet {
            sType = ST_WRITE_DESCRIPTOR_SET, dstSet = ds, dstBinding = 0,
            descriptorCount = 1, descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
            pImageInfo = &dii,
        };
        vkUpdateDescriptorSets(_dev, 1, &wds, 0, null);

        // One-shot layout transition UNDEFINED→GENERAL on the atlas image,
        // recorded into the SAME cmdbuf RecordDrawPass is building (we're
        // called from inside it, before BeginRenderPass — barriers can't
        // go inside a render pass). ‡ Caller must call this BEFORE
        // vkCmdBeginRenderPass.
        var range = new VkImageSubresourceRange { aspectMask = 1, levelCount = 1, layerCount = 1 };
        var bar = new VkImageMemoryBarrier {
            sType = ST_IMG_BARRIER, image = img,
            srcAccessMask = 0, dstAccessMask = 0x20,    // SHADER_READ
            oldLayout = 0, newLayout = 1,               // UNDEFINED → GENERAL
            srcQueueFamilyIndex = ~0u, dstQueueFamilyIndex = ~0u,
            subresourceRange = range,
        };
        vkCmdPipelineBarrier(_cmdBuf, 0x1, 0x80, 0, 0, null, 0, null, 1, &bar);  // TOP→FRAGMENT_SHADER

        _atlasReady = true;
        $"[vk] EnsureAtlasBound OK — atlas {aw}×{ah} img={img:x} view={v:x} ds={ds:x}".Log();
    }

    //  ensure texId has a VkImage+View+DS. Lazy upload
    // on first draw. v1 = RGBA8 LINEAR HOST_VISIBLE only (= the
    // atlas path, generalized). Decode (BC3 etc) is the caller's
    // job — pass already-decoded RGBA8 bytes. Returns the DS for
    // per-draw bind, or 0 if upload failed/pending.
    // ⚠️ Must be called BEFORE vkCmdBeginRenderPass (= barriers
    // can't go inside a render pass). The barrier for newly-
    // uploaded images is queued in _texPendingBarrier and emitted
    // by FlushTexBarriers().
    static ulong EnsureTexBound(int texId, NvnLinux.H tex) {
        if(_texVk.TryGetValue(texId, out var t)) {
            // (c²³)(a) Gen-check: game may re-register the
            // same texId with a different NVN tex (= different
            // H instance), OR re-init/re-upload the same one
            // (= H.Gen bumped). Either ⟹ stale vk-image.
            // u702: idx 0x101 re-registered with new 2048² f42
            // (BC1) tex post-menu; cache returned the f44 atlas
            // → font-glyph-wedge instead of title screen.
            if(ReferenceEquals(t.Src, tex)
                    && t.Gen == (tex?.Gen ?? 0))
                return t.Ds;
            // ‡ v1: leak old vk resources (few textures total;
            // v2 = vkDestroyImage/View + vkFreeMemory + free DS).
            $"[vk] EnsureTexBound texId=0x{texId:x} STALE (gen {t.Gen}→{tex?.Gen ?? 0}, src{(ReferenceEquals(t.Src,tex)?"=":"≠")}) — rebuild".Log();
            _texVk.Remove(texId);
        }
        // v1: only handle textures that already have decoded
        // RGBA8 available. The atlas (= texId 0x101) does (via
        // NvnLinux.AtlasRgba). Other textures need per-tex
        // decode (= v2). Fall back to atlas DS (= _dsAtlas)
        // for textures without decode yet — wrong-image but
        // doesn't crash.
        // ‡ v1: hardwire texId 0x101 → AtlasRgba. v2 = generic.
        byte[]? rgba;
        int w, h;
        // v2 generic: decode-on-demand from tex.CpuPtr. Falls
        // back to AtlasRgba for 0x101 if decode fails (= keeps
        // §5 stable while v2's decode is shaken out).
        if(tex != null
           && NvnLinux.DecodeForUpload(tex) is { } dec) {
            rgba = dec; w = tex.Width; h = tex.Height;
        } else if(texId == 0x101 && NvnLinux.AtlasRgba != null) {
            rgba = NvnLinux.AtlasRgba;
            w = NvnLinux.AtlasW; h = NvnLinux.AtlasH;
        } else {
            // No decode available → cache as 0 (= caller falls
            // back to atlas DS or skips texture bind).
            _texVk[texId] = new(0, 0, 0, 0, 0, 0, true);
            return 0;
        }

        var ici = new VkImageCreateInfo {
            sType = ST_IMAGE_CI, imageType = 1, format = 37,
            width = (uint)w, height = (uint)h, depth = 1,
            mipLevels = 1, arrayLayers = 1, samples = 1,
            tiling = 1, usage = 0x4 | 0x2, initialLayout = 0,
        };
        ulong img; if(Chk(vkCreateImage(_dev, &ici, null, &img),
                $"vkCreateImage(tex{texId:x})") != 0) return 0;
        VkMemoryRequirements req;
        vkGetImageMemoryRequirements(_dev, img, &req);
        var mai = new VkMemoryAllocateInfo {
            sType = ST_MEM_AI, allocationSize = req.size,
            memoryTypeIndex = _hostMemType,
        };
        ulong mem; if(Chk(vkAllocateMemory(_dev, &mai, null, &mem),
                $"vkAllocMem(tex{texId:x})") != 0) return 0;
        Chk(vkBindImageMemory(_dev, img, mem, 0), "vkBindImageMem");
        void* p; Chk(vkMapMemory(_dev, mem, 0, req.size, 0, &p),
                "vkMapMem");
        rgba.AsSpan().CopyTo(new Span<byte>(p, (int)(w*h*4)));
        // (c⁴¹)×3(S) verify-log: post-memcpy first-4-bytes at
        // mapped *p (= what the GPU will sample at texel(0,0)).
        // If ≠ rgba[0..3] ⟹ memcpy/mapping broken. Plus the
        // returned DS handle (= 0 ⟹ caller falls to atlas).
        if(_etbLogN++ < 30 || _etbLogN % 200 == 0) {
            var bp = (byte*)p;
            $"[c41] EnsureTexBound texId=0x{texId:x} {w}×{h}: rgba[0]={rgba[0]:x2}{rgba[1]:x2}{rgba[2]:x2}{rgba[3]:x2} → *p[0]={bp[0]:x2}{bp[1]:x2}{bp[2]:x2}{bp[3]:x2} req.size={req.size} (vs w*h*4={w*h*4})".Log();
        }

        var ivci = new VkImageViewCreateInfo {
            sType = ST_IMAGE_VIEW_CI, image = img, viewType = 1,
            format = 37,
            subresourceRange = new() { aspectMask=1, levelCount=1, layerCount=1 },
        };
        ulong vw; Chk(vkCreateImageView(_dev, &ivci, null, &vw),
                "vkCreateImageView");

        // 5: alloc against _t3DslTex (= set=1's layout:
        // binding=8 COMBINED_IMAGE_SAMPLER) + _t3DsPool, so the
        // resulting DS is binding-compatible with _t3PipelineLayout
        // and can be swapped into t3sets[1] per-draw.
        ulong dsl = _t3DslTex;
        var dsai = new VkDescriptorSetAllocateInfo {
            sType = ST_DESCRIPTOR_SET_AI, descriptorPool = _t3DsPool,
            descriptorSetCount = 1, pSetLayouts = &dsl,
        };
        ulong ds; if(Chk(vkAllocateDescriptorSets(_dev, &dsai, &ds),
                $"vkAllocDS(tex{texId:x})") != 0) return 0;
        // (c²⁶) Write the SAME image to all T3TexBindN
        // bindings. v1 = wrong-texture-but-doesn't-crash for
        // bindings ≠8; per-slot resolution = (c²⁷). Vulkan
        // requires ALL bindings written before bind (can't
        // leave 1..39 dangling — that's the segv we just hit
        // from the layout side).
        var dii = new VkDescriptorImageInfo {
            sampler = _sampler, imageView = vw, imageLayout = 1,
        };
        var wds = stackalloc VkWriteDescriptorSet[T3TexBindN];
        for(var b = 0; b < T3TexBindN; b++)
            wds[b] = new() {
                sType = ST_WRITE_DESCRIPTOR_SET, dstSet = ds,
                dstBinding = (uint)b, descriptorCount = 1,
                descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
                pImageInfo = &dii,
            };
        vkUpdateDescriptorSets(_dev, T3TexBindN, wds, 0, null);

        _texVk[texId] = new(img, mem, vw, ds, w, h, false,
                            tex, tex?.Gen ?? 0);
        _texPendingBarrier.Add(texId);
        $"[vk] EnsureTexBound texId=0x{texId:x} {w}×{h} img={img:x} view={vw:x} ds={ds:x}".Log();
        return ds;
    }

    // (T6)×31 (α)-fix v2-PROPER — per-slot texture bind.
    // The (c⁴⁰)(F)/(c²⁶)/(c²⁷) deferred-‡ at ~30-chapter
    // scale (kt[26]). Per own (c⁴⁰) docstring @NvnLinux
    // CbBindTexture: shader binding = 2×slot+8 (sh0814
    // bindings 16,30,32 = slots 4,11,12). fs442 reads
    // tcb_10 (b16=sl4 albedo) + tcb_12 (b18=sl5 normal-
    // map) + tcb_E (b14=sl3 env-cube). With (c⁴⁰)(F)'s
    // one-tex-for-all-40, tcb_12 reads the slot[4]-pick
    // (= white albedo-atlas) → t3-5≈(0.98,0.98,0.98) →
    // lighting-on-color-values → G-bias (verified r79p7
    // = trees BRIGHT-GREEN; r80tex b16=b18 at-pixel).
    //
    // DS keyed on hash(TexHandles[0..7]) — distinct per-
    // material tuples ≈ 50-100 across 669 draws ⟹ fits
    // T3TexDsMax=128. For each binding b: if b∈[8,22]
    // even ⟹ slot=(b−8)/2 ⟹ that slot's texId's view;
    // else fallback (= the (c⁴⁰)(F)-pick's view, keeps
    // non-load-bearing bindings non-dangling per (c²⁶)).
    // ‡ slots 8-15 (= bindings 24-38) need needExt path
    // (capture stores [0..7] only); ‡ slot 3 = cube
    // (writing 2D view; lavapipe tolerates per (T6)×14).
    static readonly bool _t3TexPerSlot = Environment
        .GetEnvironmentVariable("UMBRA_T3_TEX_PERSLOT")
            != null;
    static readonly Dictionary<ulong, ulong> _t3SlotDs
        = new();
    static long _slotDsLogN;

    static ulong EnsureSlotDs(NvnLinux.DrawRecord d,
            ulong fbView) {
        ulong key = 0xcbf29ce484222325;
        for(var sl = 0; sl < 8; sl++)
            key = (key ^ d.TexHandles[sl]) * 0x100000001b3;
        if(_t3SlotDs.TryGetValue(key, out var ds))
            return ds;
        // Ensure each slot's texId is uploaded; collect
        // views. EnsureTexBound also creates a redundant
        // per-texId DS (= pool-waste, ‡ tolerable v1).
        var views = stackalloc ulong[8];
        var nReal = 0;
        var nRt = 0;
        for(var sl = 0; sl < 8; sl++) {
            var h = d.TexHandles[sl];
            var ti = (int)(h >> 32);
            if(ti == 0) { views[sl] = fbView; continue; }
            // (T6)×37 ×2 RT-as-sampler: if this texId is
            // a render-target, bind the rendered view.
            // Producing RT's EnsureRtFb must have fired
            // first (= RT was drawn-to before being
            // sampled, which the game's render-graph
            // guarantees: rt1[A] drawn before rt2[B]
            // samples 0x134 etc per ×33×3(b) sequence).
            if(_t3Rtt && _rtTexView.TryGetValue(ti, out var rv)
               && rv != 0) {
                views[sl] = rv; nReal++; nRt++; continue;
            }
            var tx = NvnLinux.ResolveTex(h);
            EnsureTexBound(ti, tx);
            views[sl] = _texVk.TryGetValue(ti, out var tv)
                && tv.View != 0 ? tv.View : fbView;
            if(views[sl] != fbView) nReal++;
        }
        ulong dsl = _t3DslTex;
        var dsai = new VkDescriptorSetAllocateInfo {
            sType = ST_DESCRIPTOR_SET_AI,
            descriptorPool = _t3DsPool,
            descriptorSetCount = 1, pSetLayouts = &dsl,
        };
        if(Chk(vkAllocateDescriptorSets(_dev, &dsai, &ds),
                "vkAllocDS(slotDs)") != 0) {
            // Pool exhausted ⟹ cache 0 (= caller falls
            // back to per-texId DS = old behavior).
            _t3SlotDs[key] = 0; return 0;
        }
        var dii = stackalloc VkDescriptorImageInfo[T3TexBindN];
        var wds = stackalloc VkWriteDescriptorSet[T3TexBindN];
        for(var b = 0; b < T3TexBindN; b++) {
            var sl = (b - 8) >> 1;
            var vw = (b >= 8 && b < 24 && (b & 1) == 0
                      && views[sl] != 0)
                ? views[sl] : fbView;
            dii[b] = new() { sampler = _sampler,
                imageView = vw, imageLayout = 1 };
            wds[b] = new() {
                sType = ST_WRITE_DESCRIPTOR_SET,
                dstSet = ds, dstBinding = (uint)b,
                descriptorCount = 1,
                descriptorType
                    = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
                pImageInfo = &dii[b],
            };
        }
        vkUpdateDescriptorSets(_dev, T3TexBindN, wds, 0, null);
        _t3SlotDs[key] = ds;
        if(_slotDsLogN++ < 20 || nRt > 0)
            $"[vk] EnsureSlotDs key=0x{key:x} ds={ds:x} nReal={nReal}/8 nRt={nRt} sl4=0x{d.TexHandles[4]>>32:x} sl5=0x{d.TexHandles[5]>>32:x}".Log();
        return ds;
    }

    // Emit UNDEFINED→GENERAL barriers for newly-uploaded textures.
    // Called from RecordDrawPass BEFORE vkCmdBeginRenderPass.
    static void FlushTexBarriers() {
        if(_texPendingBarrier.Count == 0) return;
        var range = new VkImageSubresourceRange {
            aspectMask = 1, levelCount = 1, layerCount = 1 };
        foreach(var texId in _texPendingBarrier) {
            var t = _texVk[texId];
            var bar = new VkImageMemoryBarrier {
                sType = ST_IMG_BARRIER, image = t.Img,
                srcAccessMask = 0, dstAccessMask = 0x20,
                oldLayout = 0, newLayout = 1,
                srcQueueFamilyIndex = ~0u, dstQueueFamilyIndex = ~0u,
                subresourceRange = range,
            };
            vkCmdPipelineBarrier(_cmdBuf, 0x1, 0x80, 0,
                0, null, 0, null, 1, &bar);
            _texVk[texId] = t with { BarrierDone = true };
        }
        _texPendingBarrier.Clear();
    }

    // ════════════════════════════════════════════════
    // (T6)×35 RTT render-side: per-RtSig framebuffer.
    // ════════════════════════════════════════════════
    // EnsureRtFb(rtId): lazy-alloc {nC color VkImage +
    // optional depth + VkRenderPass + VkFramebuffer}
    // from NvnLinux.RtSigs[rtId]. Cached _rtFb[rtId].
    // u778/nvncap5 census = 15 distinct sigs:
    //   id=0  [F]    1920×1080 RGBA8  +D24S8  composite
    //   id=1  [A]    1920×1080 RGBA8×3+D24S8  G-buffer (3-MRT)
    //   id=2  [B]    1920×1080 RGBA16F+D24S8  HDR-light
    //   id=3  [C]     960×540  RGBA16F        half-res
    //   id=4-8        480..30  RGBA8          bloom mips
    //   id=9  SHADOW   nC=0    1024×4096 D32F cascaded
    //   id=10/11      480/240  RGBA16F        HDR bloom-down
    //   id=12-14     1024×512/64²/256² misc
    // All images usage=ATTACH|SAMPLED so ×36 RT-as-
    // sampler can bind same view. ‡ device-local would
    // be correct; lavapipe doesn't care, _hostMemType
    // works. ‡ loadOp=CLEAR every RP-begin (= wrong for
    // [B]×9 accumulate; v1 = LOAD after first; ×36).
    // ‡ no readback yet (OPTIMAL needs vkCmdCopyImage
    // ToBuffer; ×35×4 verifies via RP-switch-count +
    // log only).
    public class RtFb {
        public ulong[] Img = [], View = [], Mem = [];
        public ulong DepthImg, DepthView, DepthMem;
        public ulong Rp, Fb;
        public int W, H, NC;
        public bool HasDepth;
        // (T6)×37 ×3: loadOp=LOAD-after-first. RP-begin
        // uses CLEAR on first visit this frame, LOAD
        // thereafter (via vkCmdClear* inside the pass
        // — RP loadOp is baked at create-time, so we
        // build the RP with loadOp=LOAD and explicitly
        // vkCmdClearAttachments on first-visit). Verified
        // (T6)×37×2(a): rt2[B] visited ×2 (×3 lit-passes
        // sampling shadow temporally + ×5 post-shadow);
        // CLEAR on revisit wiped the ×3.
        public int SeenThisFrame;
    }
    static readonly RtFb?[] _rtFb = new RtFb?[32];
    // (T6)×37 ×2 RT-as-sampler: texId → the RT-view that
    // produces it. Populated by EnsureRtFb as RTs alloc.
    // EnsureSlotDs checks this BEFORE _texVk so a draw
    // sampling texId=0x134 (= rt1.c[0] G-buffer per
    // nvncap5 manifest) gets the rendered _rtFb[1].View
    // [0] instead of an asset-texture upload. Verified
    // (T6)×37×1(b): rt2[B] deferred-light draws sample
    // slot[1]=0x134 slot[2]=0x136 slot[6]=0x139 slot[4]
    // =0x138(shadow-D32F) = the G-buf+shadow read-set.
    // ‡ depth-as-sampler: D24S8 view has aspectMask=
    // DEPTH|STENCIL (=6) which some drivers reject for
    // combined-sampler; D32F (=2) is fine. ×38 if hit.
    static readonly Dictionary<int, ulong> _rtTexView = new();

    // NVN RT format → (VkFormat, aspectMask, usage).
    // Per botw nvn.h (sera ·10966).
    static (uint vf, uint asp, uint usg) NvnRtFmt(int nvnFmt)
        => nvnFmt switch {
        0x25 => ( 37, 1, 0x10|0x4|0x1), // RGBA8       COLOR_ATTACH|SAMPLED|TRANSFER_SRC
        0x29 => ( 97, 1, 0x10|0x4|0x1), // RGBA16F
        0x12 => ( 77, 1, 0x10|0x4|0x1), // RG16_UNORM (velocity)
        0x34 => (126, 2, 0x20|0x4),     // D32_SFLOAT  DEPTH_STENCIL_ATTACH|SAMPLED
        0x35 => (129, 6, 0x20|0x4),     // D24_UNORM_S8 DEPTH|STENCIL
        0x33 => (125, 2, 0x20|0x4),     // D24X8 → X8_D24_UNORM_PACK32 ‡
        _    => ( 37, 1, 0x10|0x4|0x1), // ‡ default RGBA8
    };

    static RtFb EnsureRtFb(byte rtId) {
        if(_rtFb[rtId] is {} cached) return cached;
        if(rtId >= NvnLinux.RtSigs.Count) {
            // ‡ rtId out-of-range (capVer<3 d.RtId=0
            // with no RtSigs). Synthesize swap-equiv.
            $"[vk] ⚠ EnsureRtFb({rtId}) no sig; using swap".Log();
            return _rtFb[rtId] = new() {
                Rp = _renderPass, Fb = _swapFb[0],
                W = (int)W, H = (int)H, NC = 1, HasDepth = true,
            };
        }
        var sig = NvnLinux.RtSigs[rtId];
        var nC = sig.NC;
        var w = nC > 0 ? sig.Colors[0].W : sig.Depth!.W;
        var h = nC > 0 ? sig.Colors[0].H : sig.Depth!.H;
        var hasD = sig.Depth != null;
        var rf = new RtFb {
            Img = new ulong[nC], View = new ulong[nC],
            Mem = new ulong[nC],
            W = w, H = h, NC = nC, HasDepth = hasD,
        };
        // Local: alloc one image+mem+view per attachment.
        (ulong, ulong, ulong) MkImg(int aw, int ah, int nf,
                                    string tag) {
            var (vf, asp, usg) = NvnRtFmt(nf);
            var ici = new VkImageCreateInfo {
                sType = ST_IMAGE_CI, imageType = 1, format = vf,
                width = (uint)aw, height = (uint)ah, depth = 1,
                mipLevels = 1, arrayLayers = 1, samples = 1,
                tiling = 0, usage = usg,
            };
            ulong im; Chk(vkCreateImage(_dev, &ici, null, &im),
                $"vkCreateImage({tag})");
            VkMemoryRequirements mr;
            vkGetImageMemoryRequirements(_dev, im, &mr);
            var mai = new VkMemoryAllocateInfo {
                sType = ST_MEM_AI, allocationSize = mr.size,
                memoryTypeIndex = _hostMemType,
            };
            ulong mm; Chk(vkAllocateMemory(_dev, &mai, null, &mm),
                $"vkAllocateMemory({tag})");
            Chk(vkBindImageMemory(_dev, im, mm, 0),
                $"vkBindImageMemory({tag})");
            var vci = new VkImageViewCreateInfo {
                sType = ST_IMAGE_VIEW_CI, image = im,
                viewType = 1, format = vf,
                subresourceRange = new() {
                    aspectMask = asp, levelCount = 1, layerCount = 1,
                },
            };
            ulong vw; Chk(vkCreateImageView(_dev, &vci, null, &vw),
                $"vkCreateImageView({tag})");
            return (im, mm, vw);
        }
        for(var i = 0; i < nC; i++)
            (rf.Img[i], rf.Mem[i], rf.View[i]) =
                MkImg(w, h, sig.Colors[i].Fmt, $"rt{rtId}.c[{i}]");
        if(hasD)
            (rf.DepthImg, rf.DepthMem, rf.DepthView) =
                MkImg(sig.Depth!.W, sig.Depth.H, sig.Depth.Fmt,
                      $"rt{rtId}.d");
        // RenderPass: nC color + optional depth.
        var nAtt = nC + (hasD ? 1 : 0);
        var atts = stackalloc VkAttachmentDescription[Math.Max(nAtt, 1)];
        var crefs = stackalloc VkAttachmentReference[Math.Max(nC, 1)];
        // (T6)×37 ×3: loadOp=LOAD (=0); explicit clear via
        // vkCmdClearAttachments on first-visit-this-frame
        // (rt2 visited ×2 per rts.bin seq; CLEAR-on-revisit
        // wiped first-3 lit-passes). initialLayout=GENERAL
        // so LOAD on first-ever-visit reads alloc-garbage —
        // ‡ harmless (first-visit always clears explicitly).
        for(var i = 0; i < nC; i++) {
            var (vf, _, _) = NvnRtFmt(sig.Colors[i].Fmt);
            atts[i] = new() {
                format = vf, samples = 1,
                loadOp = 0, storeOp = 0,        // LOAD, STORE
                stencilLoadOp = 2, stencilStoreOp = 1,
                initialLayout = 1, finalLayout = 1,  // GENERAL → GENERAL
            };
            crefs[i] = new() { attachment = (uint)i, layout = 2 };
        }
        VkAttachmentReference dref = default;
        if(hasD) {
            var (vf, _, _) = NvnRtFmt(sig.Depth!.Fmt);
            atts[nC] = new() {
                format = vf, samples = 1,
                loadOp = 0, storeOp = 0,
                stencilLoadOp = 2, stencilStoreOp = 1,
                initialLayout = 1, finalLayout = 1,
            };
            dref = new() { attachment = (uint)nC, layout = 3 };
        }
        var sub = new VkSubpassDescription {
            pipelineBindPoint = 0,
            colorCount = (uint)nC,
            pColors = nC > 0 ? crefs : null,
            pDepthStencil = hasD ? &dref : null,
        };
        var rpci = new VkRenderPassCreateInfo {
            sType = ST_RENDER_PASS_CI,
            attachmentCount = (uint)nAtt, pAttachments = atts,
            subpassCount = 1, pSubpasses = &sub,
        };
        ulong rp; Chk(vkCreateRenderPass(_dev, &rpci, null, &rp),
            $"vkCreateRenderPass(rt{rtId})");
        rf.Rp = rp;
        // Framebuffer
        var fbAtts = stackalloc ulong[Math.Max(nAtt, 1)];
        for(var i = 0; i < nC; i++) fbAtts[i] = rf.View[i];
        if(hasD) fbAtts[nC] = rf.DepthView;
        var fbci = new VkFramebufferCreateInfo {
            sType = ST_FRAMEBUFFER_CI, renderPass = rp,
            attachmentCount = (uint)nAtt, pAttachments = fbAtts,
            width = (uint)w, height = (uint)h, layers = 1,
        };
        ulong fb; Chk(vkCreateFramebuffer(_dev, &fbci, null, &fb),
            $"vkCreateFramebuffer(rt{rtId})");
        rf.Fb = fb;
        _rtFb[rtId] = rf;
        // (T6)×37 ×2: register this RT's texIds → views
        // for RT-as-sampler. texId=0 ⟹ never sampled.
        // (T6)×40 ×1: TryAdd — rt0/1/2 SHARE depth texId
        // 0x135 (game binds same NVNtexture as depth for
        // all three). First-RT-to-alloc wins = rt1 (the
        // G-buffer scene-depth, which is what samplers
        // want). Was: rt2.DepthView overwrote rt1's ⟹
        // fs111 sl0=0x135 read rt2's depth (only #163's
        // fullscreen-z) instead of scene depth.
        // Depth: D24S8 (asp=6) can't be combined-image-
        // sampler (VUID-01976) ⟹ make a SEPARATE asp=2
        // (depth-only) view for sampling. D32F (asp=2)
        // is fine as-is.
        for(var i = 0; i < nC; i++)
            if(sig.Colors[i].TexId is var tid && tid != 0)
                _rtTexView.TryAdd(tid, rf.View[i]);
        if(hasD && sig.Depth!.TexId is var dtid && dtid != 0) {
            var dv = rf.DepthView;
            var (dvf, dasp, _) = NvnRtFmt(sig.Depth.Fmt);
            if(dasp == 6) {
                // D24S8: separate depth-only sample-view.
                var vci = new VkImageViewCreateInfo {
                    sType = ST_IMAGE_VIEW_CI, image = rf.DepthImg,
                    viewType = 1, format = dvf,
                    subresourceRange = new() {
                        aspectMask = 2, levelCount = 1,
                        layerCount = 1,
                    },
                };
                Chk(vkCreateImageView(_dev, &vci, null, &dv),
                    $"vkCreateImageView(rt{rtId}.d-sample)");
            }
            _rtTexView.TryAdd(dtid, dv);
        }
        $"[vk] EnsureRtFb id={rtId} {w}×{h} nC={nC} d={hasD} rp=0x{rp:x} fb=0x{fb:x} rtTexView+={nC+(hasD?1:0)}".Log();
        return rf;
    }

    // T1 v0: copy game's CopyBufferToTexture source directly into our
    // host-mapped swap image. Bypasses Vulkan entirely for the COPY (the
    // image is HOST_VISIBLE LINEAR so memcpy works); the next Present's
    // submit still goes through Vulkan, and the PPM dump reads back from
    // the same memory. This shows the texture; v1 = real VkBuffer +
    // vkCmdCopyBufferToImage so it composes with future render passes.
    static byte* _stagedSrc; static int _stagedW, _stagedH, _stagedBpp;
    public static void StageTexture(ulong srcCpu, int w, int h, int bpp) {
        _stagedSrc = (byte*) srcCpu; _stagedW = w; _stagedH = h; _stagedBpp = bpp;
        $"[vk] StageTexture src=0x{srcCpu:x} {w}×{h} bpp={bpp} (will blit on next Present)".Log();
    }

    static void BlitStaged(int idx) {
        if(_stagedSrc == null || _swapPtr[idx] == null) return;
        // Center-letterbox the staged image into our W×H (1280×720) swap.
        // ‡ Game's texture is 1920×1080; v0 = nearest-downsample.
        var dst = _swapPtr[idx];
        var sw = _stagedW; var sh = _stagedH; var sbpp = _stagedBpp;
        for(var y = 0u; y < H; y++) {
            var sy = (int)((ulong) y * (ulong) sh / H);
            for(var x = 0u; x < W; x++) {
                var sx = (int)((ulong) x * (ulong) sw / W);
                var sp = _stagedSrc + (sy * sw + sx) * sbpp;
                var dp = dst + (y * W + x) * 4;
                dp[0] = sp[0]; dp[1] = sp[1]; dp[2] = sp[2]; dp[3] = sbpp >= 4 ? sp[3] : (byte)255;
            }
        }
    }

    static void RecordDrawPass(int idx) {
        // Snapshot accumulated draws (NvnLinux records on the render
        // thread; we consume on Present). v0 = drain everything; ‡ v1 =
        // per-cmdbuf association so QueueSubmit knows which draws are
        // its. With one cmdbuf in use, drain-all is correct.
        NvnLinux.DrawRecord[] draws;
        lock(NvnLinux.Draws) {
            draws = NvnLinux.Draws.ToArray();
            NvnLinux.Draws.Clear();
        }
        // (T1') NVN-boundary frame capture. After Draws
        // snapshot, before any Vulkan recording — pure
        // NVN-state at this point. See Pagentry/NVNCAP.md.
        NvnCapture.Maybe(_frameN, draws);
        {  // (re-open the brace the str_replace closed)
        }

        // Lazy atlas upload — must be BEFORE BeginRenderPass (the layout
        // transition barrier can't go inside a render pass).
        if(draws.Length > 0) EnsureAtlasBound();
        // T3 desc sets need atlas → also lazy.
        if(_t3Pipeline != 0 && _atlasReady) EnsureT3Sets();

        // Begin render pass — clears to game's clear color via loadOp.
        // (c³⁷)(G) 2 clear values: [0]=color, [1]=depth (1.0).
        // VkClearValue is a union {VkClearColorValue color;
        // VkClearDepthStencilValue {float depth; u32 stencil}};
        // setting .r=1.0 on the depth slot puts the right bytes.
        var c = NvnLinux.LastClearColor;
        var clears = stackalloc VkClearColorValue[2];
        clears[0] = new() { r = c[0], g = c[1], b = c[2], a = c[3] };
        clears[1] = new() { r = 1.0f };  // depth=1.0, stencil=0
        var rpbi = new VkRenderPassBeginInfo {
            sType = ST_RENDER_PASS_BI,
            renderPass = _renderPass, framebuffer = _swapFb[idx],
            renderArea = new() { width = W, height = H },
            clearValueCount = 2, pClearValues = clears,
        };
        //  emit barriers for textures uploaded LAST frame
        // (= EnsureTexBound queued them; barriers must be outside
        // render-pass). 1-frame delay before texture is sampleable.
        FlushTexBarriers();
        vkCmdBeginRenderPass(_cmdBuf, &rpbi, 0);  // INLINE

        // ── T3 PATH: game's compiled shaders ──
        // ⚠️ v0 KNOWN-WRONG: per-draw cbuf data memcpy'd into the SAME
        // host-mapped VkBuffers each iteration → at vkQueueSubmit time
        // ALL draws see the LAST draw's c[5] (model matrix). Result =
        // 21 rows stacked at one position. Correct = per-draw offset
        // into a big buffer + dynamic-offset descriptor (= v0.5). v0
        // exists to prove the pipeline + shader-interface CREATE
        // correctly + produce ANY pixels with game's shaders. The
        // pixel-correctness is the v0.5 follow.
        if(_t3Pipeline != 0 && _t3DsVs != 0) {
            ulong curPipe = 0;
            var t3sets = stackalloc ulong[3];
            t3sets[0] = _t3DsVs; t3sets[1] = _t3DsTex; t3sets[2] = _t3DsFs;
            // 24 dynamic offsets (12 per cbuf-set × 2 sets;
            // ordering = set-major then binding-ascending per
            // Vulkan spec). All same value per draw (= recN*
            // T3CbufStride) since every cbuf has its draw#k slot
            // at the same offset.
            var t3dyn = stackalloc uint[24];
            ulong vbo = 0, ibo = 0;
            ulong vb = _vbuf;
            int recN = 0, nIdx = 0, nIdxSkip = 0, nPostSkip = 0;
            // (c²⁷) Per-frame vbuf-dedup: many indexed draws
            // share one big mesh vbuf. Copy each (VbGpu,VbSize)
            // once, record its vbo-offset, reuse.
            var vbCache = new Dictionary<(ulong,ulong), ulong>(64);
            // (c²⁷)(A) filter v2: when 3D scene is active
            // (= ANY indexed draws this frame), keep ONLY
            // indexed + UI (vs343/345/349 = the 2D overlay
            // path). All other non-indexed = post-proc chain
            // (bloom downsample vs31/32, upsample vs33/34,
            // composite vs1/2, tonemap vs42/198) which sample
            // RTs we don't write to ⟹ output black ⟹ over-
            // write the geometry. v1's Count==3 filter missed
            // the strip-quad ones (count=4 → expanded 6).
            // When NO indexed draws (= 2D screens), keep all
            // (= existing menu/legal/title behavior).
            var skipPostproc = Environment.GetEnvironmentVariable(
                "UMBRA_T3_SKIP_POSTPROC") != null;
            var hasIndexed = draws.Any(x => x.IdxCount > 0);
            // (T6)×35 ×3: per-rtId draw routing instrument.
            int curRtId = -1, nRpSwitch = 0, rtDrawNAt = 0;
            var rtDrawN = new int[32];
            // (T6)×37 ×3: reset SeenThisFrame at frame-start.
            if(_t3Rtt) foreach(var rf_ in _rtFb)
                if(rf_ != null) rf_.SeenThisFrame = 0;
            foreach(var d in draws) {
                if(d.VbCpu == 0) continue;
                if(d.IdxCount == 0 && d.Count <= 0) continue;
                if(skipPostproc && hasIndexed
                        && d.IdxCount == 0
                        && d.VsShIdx != 343 && d.VsShIdx != 345
                        && d.VsShIdx != 349) {
                    nPostSkip++; continue;
                }
                // (c³⁴)×4(e') skybox-skip: when FS-override
                // active AND 3D, skip the sh794 skybox draw
                // (= screen-covering quad). With WHITE_FS it
                // paints over geometry; with DEPTHVIS_FS at
                // z≈far it'd be near-black anyway, but skip
                // for consistent comparison. (c³⁷)×4: also
                // skip when SKIP_VS contains 794 regardless.
                if((_whiteFs || _depthvisFs) && hasIndexed
                        && d.VsShIdx == 794) {
                    nPostSkip++; continue;
                }
                // (c³⁷)×1(H) generic per-VS skip: UMBRA_T3
                // _SKIP_VS=45,287,… skips draws with VsShIdx
                // in the set. Iterative discriminator for
                // "which indexed mesh screen-fills under
                // WHITE_FS?". sh794 already handled above
                // (skybox); next candidates: sh45 (idxN=
                // 2280, ‡skydome/depth-prepass), sh813
                // (idxN=34944, the big mesh).
                if(_skipVs != null && _skipVs.Contains(d.VsShIdx)) {
                    nPostSkip++; continue;
                }
                // (T6)×35 ×3: RP-switch on rtId-change. The
                // swap-RP (@1822→FlushTexBarriers→BeginRP)
                // is already open at loop-entry; first
                // switch ends it + begins _rtFb[d.RtId].
                // @2701's vkCmdEndRenderPass closes the
                // last RT-RP. ‡ No barrier yet (= [B]
                // sampling [A]'s color before transition;
                // lavapipe sequential ⟹ tolerates per ×14;
                // proper barrier = ×36). curPipe=0 forces
                // rebind (pipe is per-rtId via cache-key).
                var rtKey = _t3Rtt ? d.RtId : (byte)255;
                if(_t3Rtt && d.RtId != curRtId) {
                    vkCmdEndRenderPass(_cmdBuf);
                    // (T6)×39 ×1(c) disc: barrier between
                    // prev-RT-write and next-RT-samples-it.
                    // ×35's "‡ lavapipe sequential tolerates"
                    // may be wrong (lavapipe IS multi-thread
                    // tile-raster). r96/r98 diagonal-stripe
                    // signature = wrong-stride read = tiles
                    // not flushed at sample-time. Full memory
                    // barrier (srcStage=COLOR_ATTACH_OUT|LATE_
                    // FRAG=0x400|0x200, dstStage=FRAG_SHADER=
                    // 0x80; srcAccess=COLOR_ATTACH_WRITE|DS_
                    // WRITE=0x100|0x400, dstAccess=SHADER_READ
                    // =0x20). Coarse — per-image barrier =
                    // ×39+ once root confirmed.
                    // Execution-only barrier (0 mem-barriers
                    // = valid; src→dst stage dependency
                    // forces COLOR_ATTACH_OUTPUT|LATE_FRAG
                    // complete before FRAG_SHADER starts).
                    vkCmdPipelineBarrier(_cmdBuf,
                        0x400 | 0x200, 0x80, 0,
                        0, null, 0, null, 0, null);
                    var rf = EnsureRtFb(d.RtId);
                    var clr = stackalloc VkClearColorValue[rf.NC + 1];
                    for(var ci = 0; ci <= rf.NC; ci++)
                        clr[ci] = new() { r = ci == rf.NC ? 1.0f : 0 };
                    var rb = new VkRenderPassBeginInfo {
                        sType = ST_RENDER_PASS_BI,
                        renderPass = rf.Rp, framebuffer = rf.Fb,
                        renderArea = new() {
                            width = (uint)rf.W, height = (uint)rf.H },
                        clearValueCount = (uint)(rf.NC
                            + (rf.HasDepth ? 1 : 0)),
                        pClearValues = clr,
                    };
                    vkCmdBeginRenderPass(_cmdBuf, &rb, 0);
                    // (T6)×37 ×3: explicit clear on FIRST
                    // visit this frame only. Subsequent
                    // visits LOAD (= rt2's ×3+×5 both keep).
                    // ‡ Real engine clears via game's
                    // nvnCommandBufferClearColor/Depth
                    // calls (stubbed); this approximates
                    // "clear at frame-start per RT" until
                    // those are hooked + captured.
                    if(rf.SeenThisFrame++ == 0) {
                        var nClr = rf.NC + (rf.HasDepth ? 1 : 0);
                        var ca = stackalloc VkClearAttachment[nClr];
                        for(var ci = 0; ci < rf.NC; ci++)
                            ca[ci] = new() { aspectMask = 1,
                                colorAttachment = (uint)ci };
                        if(rf.HasDepth)
                            ca[rf.NC] = new() { aspectMask = 6,
                                clearValue = new() { r = 1.0f } };
                        var cr = new VkClearRect {
                            rect = new() { width = (uint)rf.W,
                                height = (uint)rf.H },
                            layerCount = 1,
                        };
                        vkCmdClearAttachments(_cmdBuf,
                            (uint)nClr, ca, 1, &cr);
                    }
                    // y-flipped (NVN y-up → Vk y-down), per-RT dims.
                    var rvp = new VkViewport {
                        x = 0, y = rf.H, width = rf.W, height = -rf.H,
                        minDepth = 0, maxDepth = 1 };
                    var rsc = new VkRect2D {
                        width = (uint)rf.W, height = (uint)rf.H };
                    vkCmdSetViewport(_cmdBuf, 0, 1, &rvp);
                    vkCmdSetScissor(_cmdBuf, 0, 1, &rsc);
                    rtDrawN[curRtId >= 0 ? curRtId : 31]
                        += recN - rtDrawNAt;
                    rtDrawNAt = recN; nRpSwitch++;
                    curRtId = d.RtId; curPipe = 0;
                }
                // v1.0: per-draw pipeline lookup (build-on-miss).
                // Returns 0 if .spv missing → skip this draw
                // (rather than render through wrong shaders).
                // (T6)×40 ×1(ii): non-indexed under RTT =
                // fullscreen postproc ⟹ no depth-test.
                // ‡ Real fix = capture per-draw depth-
                // state from nvnCommandBufferSetDepth
                // StencilState; this is the engine-
                // convention approximation.
                var noDepth = _t3Rtt && d.IdxCount == 0;
                var pipe = T3Pipe(d.VsShIdx, d.FsShIdx,
                                  d.Attribs, d.Streams,
                                  rtKey, noDepth);
                if(pipe == 0) continue;
                if(pipe != curPipe) {
                    vkCmdBindPipeline(_cmdBuf, 0, pipe);
                    curPipe = pipe;
                }
                // 5: per-texId DS bind. EnsureTexBound returns
                // a DS (alloc'd against _t3DslTex = binding-compatible
                // with set=1) for textures with decoded RGBA8, or 0
                // for not-yet-decoded → fall back to _t3DsTex (= the
                // atlas; wrong-image but doesn't crash). ‡ Barriers
                // queue inside-RP → emit NEXT frame's pre-RP (1-frame
                // delay; the first frame using a new texId samples
                // UNDEFINED layout = lavapipe tolerates).
                // (c⁴⁰)(F)v1 pick-first-real: d.TexHandle (=
                // last-bound-wins) is texId=0x100 (4×4 black
                // default) for most 3D draws — game binds
                // diffuse→slot-N then aux→slots-K with 0x100
                // last. Scan d.TexHandles[] for first slot
                // with a NON-0x100, NON-zero texId; bind THAT
                // to all 40 (= still wrong-per-slot but a real
                // texture instead of black). v2-PROPER (per-
                // binding image from full TexHandles[]) =
                // (c⁴⁰)×4 once v1 confirms color.
                var texId = (int)(d.TexHandle >> 32);
                var tex = d.Tex;
                // (c⁴²)×2 (F)v1-v3 UNCONDITIONAL: u752 showed
                // ×21 draws have d.TexHandle=0x12d (RT, black)
                // and ×17=0x128, ×8=0x12a — all bypass the
                // texId∈{0,0x100} gate ⟹ bind RT directly ⟹
                // texfetch→black for the FOREGROUND draws.
                // Always scan; prefer BC. If d.TexHandle was
                // already a good BC tex, the scan re-picks it
                // (= no regression). isIndexed-only (= 2D
                // legacy unaffected).
                // (c⁴²)×3 (F)v1-v4: u753 [c42] showed slot[0]
                // = 0x103/0x105 (256² BC AO/shadow-receiver,
                // rgba[0]=000000ff BLACK) for ~30/43 VS — the
                // first-BC-break heuristic picks BLACK. Per
                // the 43-VS table, **slot[4] = diffuse**
                // consistently (fcfbfcff/f9c70aff/e6e6eeff
                // etc). Score: BC=2, +slot[4]=4, +non-black-
                // rgba[0]=8. Scan-all (no early-break).
                if(d.TexHandles != null && d.IdxCount > 0) {
                    int bestSl = -1, bestScore = 0;
                    NvnLinux.H? bestTex = null; int bestTi = 0;
                    for(var sl = 0; sl < 40; sl++) {
                        var h = d.TexHandles[sl];
                        var ti = (int)(h >> 32);
                        if(ti == 0 || ti == 0x100) continue;
                        var t = NvnLinux.ResolveTex(h);
                        if(t == null) continue;
                        var isRT = t.Width == 1920
                                || t.Format == 0x29
                                || t.Format == 0x35;
                        var isBC = t.Format >= 0x42
                                && t.Format <= 0x49;
                        var nonBlk = t.Rgba != null
                                  && t.Rgba.Length >= 3
                                  && (t.Rgba[0]|t.Rgba[1]|t.Rgba[2]) != 0;
                        // (T6)×9 sera-steer: tid146 LEGO-logo
                        // has rgba[0]=(0,0,0) (black border) ⟹
                        // nonBlk=0 ⟹ score=6; tid106 normal-
                        // map at slot[5] rgba[0]=(0,123,255) ⟹
                        // score=10 ⟹ picks normal-map over
                        // diffuse. slot[4]-weight > nonBlk so
                        // slot[4] always wins when present.
                        var score = (isBC ? 2 : isRT ? 0 : 1)
                                  + (sl == 4 ? 16 : 0)
                                  + (nonBlk ? 8 : 0);
                        if(score > bestScore) {
                            bestScore = score; bestSl = sl;
                            bestTex = t; bestTi = ti;
                        }
                    }
                    if(bestSl >= 0) {
                        texId = bestTi; tex = bestTex;
                    }
                    // (c⁴³)×2 (Z') one-shot dump: write the
                    // picked tex's full Rgba to /tmp/texdump-
                    // {texId}.ppm for the first 6 distinct
                    // texIds. = LOOK at what we're binding.
                    // ·7827 at the texture-content layer.
                    if(_dumpTex && bestTex?.Rgba != null
                            && _c43Dumped.Add(bestTi)
                            && _c43Dumped.Count <= 6) {
                        var path = $"/tmp/texdump-{bestTi:x3}.ppm";
                        try {
                            using var fs = File.Create(path);
                            var hdr = System.Text.Encoding.ASCII
                                .GetBytes($"P6\n{bestTex.Width} {bestTex.Height}\n255\n");
                            fs.Write(hdr);
                            // RGBA→RGB strip
                            var rb = bestTex.Rgba;
                            var rgb = new byte[bestTex.Width*bestTex.Height*3];
                            for(int i=0,j=0; i<rb.Length; i+=4,j+=3) {
                                rgb[j]=rb[i]; rgb[j+1]=rb[i+1]; rgb[j+2]=rb[i+2];
                            }
                            fs.Write(rgb);
                            $"[c43] dumped texId=0x{bestTi:x} {bestTex.Width}×{bestTex.Height} → {path}".Log();
                        } catch(Exception ex) {
                            $"[c43] dump fail 0x{bestTi:x}: {ex.Message}".Log();
                        }
                    }
                    // (c⁴²)×2 per-VS-first-occurrence: dump
                    // full slot-table for first draw of each
                    // distinct VsShIdx (= settles whether
                    // sh33/sh31/sh1/sh390/sh1096 — the f17590
                    // dominant draws — even HAVE a BC slot,
                    // OR are pure-RT-sampling overlays).
                    if(_c42SeenVs.Add(d.VsShIdx)) {
                        var slots = "";
                        for(var sl=0; sl<40; sl++) {
                            var h = d.TexHandles[sl];
                            if(h == 0) continue;
                            var ti = (int)(h>>32);
                            var t = NvnLinux.ResolveTex(h);
                            slots += $" [{sl}]=0x{ti:x}"
                                + (t==null?"":$"({t.Width}×{t.Height},f{t.Format:x}{(t.Format>=0x42&&t.Format<=0x49?",BC":"")}{(t.Width==1920||t.Width==1024?",RT":"")},{(t.Rgba==null?"null":$"{t.Rgba[0]:x2}{t.Rgba[1]:x2}{t.Rgba[2]:x2}{t.Rgba[3]:x2}")})");
                        }
                        $"[c42] sh{d.VsShIdx}/{d.FsShIdx} d.TexH=0x{d.TexHandle>>32:x}→pick[{bestSl}]=0x{(bestSl>=0?bestTi:texId):x}(sc{bestScore}) slots:{slots}".Log();
                    }
                }
                // (c²³)(a) Always go through EnsureTexBound
                // (it Gen-checks + returns t.Ds fast on hit).
                // The prior _texVk.TryGetValue short-circuit
                // bypassed the Gen-check ⟹ stale forever.
                ulong txDs = texId == 0 ? _t3DsTex
                    : EnsureTexBound(texId, tex);
                // (T6)×31 (α)-fix: replace per-texId DS
                // with per-slot-tuple DS. Falls back to
                // txDs on pool-exhaust / no-TexHandles.
                // (T6)×39 ×3 ROOT: was gated on d.IdxCount
                // >0 — added at ×31 because non-indexed
                // draws had TexHandles=null (= the (δ)
                // CbDrawArrays gap fixed at ×38 d2c731a).
                // With (δ) fixed, the gate excluded ALL
                // postproc draws (#163 fs50, #631 fs111,
                // #663 fs244, bloom) ⟹ they fell through
                // to (c⁴⁰) single-tex heuristic (verified
                // r102: textureSize.x=1024 ≠ rt1.c[0]'s
                // 1920). Also dropped: txDs!=0 + _texVk
                // [texId] precondition (texId may be an
                // RT-texId not in _texVk; the fbView
                // fallback for empty slots can be any
                // valid view — _atlasView always exists).
                if(_t3TexPerSlot && d.TexHandles != null) {
                    var fbV = (_texVk.TryGetValue(texId,
                            out var ptv) && ptv.View != 0)
                        ? ptv.View : _atlasView;
                    var sds = EnsureSlotDs(d, fbV);
                    if(sds != 0) txDs = sds;
                }
                // (c⁴¹)×3(S2) log txDs=0 (= cached-as-0 ⟹
                // atlas fallback). If picked BC texIds show
                // txDs=0 ⟹ THAT's the gap (DecodeForUpload
                // failed at first encounter, cached forever).
                if(txDs == 0 && texId != 0
                        && (_txDs0LogN++ < 10 || _txDs0LogN%200==0))
                    $"[c41] ‡ txDs=0 for texId=0x{texId:x} (cached-as-0; sh{d.VsShIdx}/{d.FsShIdx}) → atlas fallback".Log();
                t3sets[1] = txDs != 0 ? txDs : _t3DsTex;
                // (c²⁴) NVN prim 5 = TRIANGLE_STRIP. T3Pipe is
                // hardcoded topology=TRIANGLE_LIST (@888). Game
                // uses prim=5 count=4 for fullscreen quads (legal
                // splash, title key-art) → only verts {0,1,2}
                // drew = bottom-left triangle = the half-quad.
                // Expand strip → list at copy time: strip vert i
                // (i≥2) emits triangle (i-2,i-1,i) with winding
                // flip on odd i. Count N → 3(N-2) list verts.
                // ‡ Doesn't honor primitive-restart (game doesn't
                // use it for sh349/346). NVN prim 6 (TRI_FAN) /
                // 9 (QUADS) / 10 (QUAD_STRIP) not seen in u705;
                // when they show, same expansion approach.
                // (c²⁷) Actual stride from captured Streams
                // (was hardcoded 36 — wrong for str=28/40/etc;
                // worked-by-luck on count=3 reads past 84B buf).
                var stride = d.Streams?.Length > 0
                    ? d.Streams[0].Stride : 36;
                int drawCount, drawFirst;
                ulong bytes, voff;
                bool isIndexed = d.IdxCount > 0;
                if(isIndexed) {
                    // ── (c²⁷) Indexed draw ──────────────────
                    // vbuf: copy whole bound buffer ONCE per
                    // (VbGpu,VbSize) per-frame; reuse offset.
                    // Indices reference into it via baseVtx +
                    // idx[i], so we need it whole.
                    var vk2 = (d.VbGpu, d.VbSize);
                    if(!vbCache.TryGetValue(vk2, out voff)) {
                        var vbBytes = Math.Min(d.VbSize,
                            VbufSize - vbo);
                        if(vbBytes < d.VbSize) {
                            nIdxSkip++; continue;  // out of vbuf
                        }
                        Buffer.MemoryCopy((void*)d.VbCpu,
                            _vbufPtr + vbo, vbBytes, vbBytes);
                        voff = vbo;
                        vbCache[vk2] = voff;
                        vbo += (vbBytes + 255) & ~255ul;
                    }
                    // ibuf: copy idx data (count × idxSz).
                    var idxSz = d.IdxType switch
                        { 0 => 1, 1 => 2, 2 => 4, _ => 2 };
                    var iBytes = (ulong)d.IdxCount * (ulong)idxSz;
                    if(d.IdxCpu == 0 || ibo + iBytes > IbufSize) {
                        nIdxSkip++; continue;
                    }
                    Buffer.MemoryCopy((void*)d.IdxCpu,
                        _ibufPtr + ibo, iBytes, iBytes);
                    var ioff = ibo;
                    ibo += (iBytes + 3) & ~3ul;
                    // VK_INDEX_TYPE: 0=u16 1=u32 1000265000=u8.
                    // ‡ u8 (NVN idxType=0) needs EXT_index_type
                    // _uint8; game uses idxType=1 throughout.
                    var vkIdxT = d.IdxType switch
                        { 1 => 0u, 2 => 1u, _ => 0u };
                    vkCmdBindIndexBuffer(_cmdBuf, _ibuf, ioff, vkIdxT);
                    // (T6)×17 multi-stream: bind binding-1
                    // too. If d.VbCpu1==0 (single-stream draw
                    // OR pre-(T6)×17 capture), alias b1=b0 so
                    // T3Pipe's 2-binding pipeline still has a
                    // valid buffer (closes vbuf-b1-NULL ×36K
                    // VUID-04008 regardless; reads garbage but
                    // doesn't crash). With recaptured data,
                    // d.VbCpu1 = stream-1's actual storage.
                    var voff1 = voff;
                    if(d.VbCpu1 != 0 && d.VbSize1 > 0) {
                        var vk1 = (d.VbGpu1, d.VbSize1);
                        if(!vbCache.TryGetValue(vk1, out voff1)) {
                            var b1 = Math.Min(d.VbSize1,
                                VbufSize - vbo);
                            if(b1 >= d.VbSize1) {
                                Buffer.MemoryCopy(
                                    (void*)d.VbCpu1,
                                    _vbufPtr + vbo, b1, b1);
                                voff1 = vbo;
                                vbCache[vk1] = voff1;
                                vbo += (b1 + 255) & ~255ul;
                            } else voff1 = voff;
                        }
                    }
                    var vbA = stackalloc ulong[2] { vb, vb };
                    var voA = stackalloc ulong[2] { voff, voff1 };
                    vkCmdBindVertexBuffers(_cmdBuf, 0, 2, vbA, voA);
                    drawCount = d.IdxCount;
                    drawFirst = 0;  // baked into ibuf offset
                    bytes = 0;      // vbo already advanced
                    nIdx++;
                } else if(d.Prim == 5 && d.Count >= 3) {
                    drawCount = (d.Count - 2) * 3;
                    drawFirst = 0;  // First baked into copy.
                    bytes = (ulong) drawCount * (ulong) stride;
                    if(vbo + bytes > VbufSize) break;
                    var src = (byte*) d.VbCpu;
                    var dst = (byte*)(_vbufPtr + vbo);
                    for(var i = 2; i < d.Count; i++) {
                        // odd-i flip: (i-1,i-2,i) instead of
                        // (i-2,i-1,i) to preserve winding.
                        var a = (i & 1) == 0 ? i-2 : i-1;
                        var b = (i & 1) == 0 ? i-1 : i-2;
                        Buffer.MemoryCopy(src + (d.First+a)*stride, dst, stride, stride); dst += stride;
                        Buffer.MemoryCopy(src + (d.First+b)*stride, dst, stride, stride); dst += stride;
                        Buffer.MemoryCopy(src + (d.First+i)*stride, dst, stride, stride); dst += stride;
                    }
                    voff = vbo;
                    vkCmdBindVertexBuffers(_cmdBuf, 0, 1, &vb, &voff);
                } else {
                    drawCount = d.Count;
                    drawFirst = d.First;
                    bytes = (ulong) drawCount * (ulong) stride;
                    if(vbo + bytes > VbufSize) break;
                    Buffer.MemoryCopy((void*) d.VbCpu, _vbufPtr + vbo, bytes, bytes);
                    voff = vbo;
                    vkCmdBindVertexBuffers(_cmdBuf, 0, 1, &vb, &voff);
                }
                // Cbuf: NVN(stage,slot K) → c[K+3] at set=stage.
                // _t3CbufPtr[stage*12 + (K+3)]. c[1]/c[2] driver-
                // managed; v0 zero-filled. ⚠️ Last-write-wins on VS
                // c[5] (model mat4) — all 21 rows at last position;
                // v0.5b = dynamic-offset descriptor.
                // Per-draw cbuf data → recN's slice in each buffer.
                // (per-draw cbuf isolation; was last-write-wins.)
                var slot = recN < T3MaxDraws ? recN : T3MaxDraws - 1;
                if(d.Ubos != null)
                    for(var st = 0; st < 2; st++)
                        for(var sl = 0; sl < 8; sl++) {
                            var snap = d.Ubos[st*8 + sl];
                            if(snap == null) continue;
                            var hw = sl + 3;
                            if(hw > 12) continue;
                            var cb = _t3CbufPtr[st*12 + hw]
                                   + slot * T3CbufStride;
                            var nb = Math.Min(snap.Length, T3CbufStride);
                            new ReadOnlySpan<byte>(snap, 0, nb)
                                .CopyTo(new Span<byte>(cb, T3CbufStride));
                        }
                // (c³⁹)×4(R') c[1]/c[2] driver-managed: real
                // NVN driver fills these (RT-dims/Y-flip/
                // viewport-scale/exposure-ish per Maxwell
                // convention). v0 zero-fills ⟹ any FS that
                // multiplies by a c[1]/c[2] component → 0.
                // UMBRA_T3_C1_ONES = fill with 1.0 (= neutral
                // for multiplies). ‡‡ Wrong values for
                // anything that ADDs c[1] components, but
                // most uses are scale-mults. Discriminator:
                // if color appears ⟹ c[1]/c[2]-zero was the
                // gate; then trace what real values should be
                // (kt[14] reference: ryujinx GpuAccessor
                // ConstantBuffer1 / ScreenScale).
                // (c⁴⁰)(R'-fix) scope to FS-stage only (st=1).
                // u747 showed VS c[1] is position-relevant
                // (menu→0% with C1_ONES on both stages); FS
                // c[1] is the scale-mult target.
                if(_t3C1Ones)
                    for(var hw = 1; hw <= 2; hw++) {
                        var cb = (float*)(_t3CbufPtr[1*12 + hw]
                                        + slot * T3CbufStride);
                        for(var k=0; k<64; k++) cb[k] = 1.0f;
                    }
                // (T6)×28: FS c[1] heuristic override
                // (applied AFTER C1_ONES so it wins).
                if(_t3FsC1 != null) {
                    var cb = (float*)(_t3CbufPtr[1*12 + 1]
                                    + slot * T3CbufStride);
                    for(var k=0; k<_t3FsC1.Length && k<64; k++)
                        cb[k] = _t3FsC1[k];
                }
                // (T6)×24: VS c1[0].x = normal-decode K.
                // (T6)×25 ×2: .x-only (was .xyzw=2.0 ⟹
                // r73rich floor regressed primary→b/w;
                // vs45 may use c1[0].{y|z|w} differently).
                if(_t3VsC10 is {} vc10) {
                    var cb = (float*)(_t3CbufPtr[0*12 + 1]
                                    + slot * T3CbufStride);
                    cb[0] = vc10;
                    // cb[1..3] left at 0 (zero-filled
                    // default). ‡ If vs45 needs c1[0].y
                    // etc, this still breaks it; ×2(i)
                    // decompile settles at-source.
                }
                // Bind sets WITH this draw's dynamic offsets.
                for(var k = 0; k < 24; k++)
                    t3dyn[k] = (uint)(slot * T3CbufStride);
                // (c²⁸) one-shot dump: c[3]+c[4]+first-3-verts
                // for FIRST sh813 (or sh45) indexed draw. The
                // kt[22] §7-eval-with-real-data settler. sh794
                // (stars, WORKS) reads c[3] only; sh45/800/813
                // (0px) read c[3]+c[4]. ⟹ c[4] is the disc.
                // (c²⁹)×4 multi-frame dump, RELATIVE to first
                // sh813 (3D-onset moves with timeline; was
                // gated f≥32000 absolute = missed when Report
                // Counter compressed loading→3D@f~12000).
                // (c²⁹)-(c³⁶) diagnostics — LEGO Worlds-specific
                // (hardcoded game-image addrs at 0xfff5b…, valid
                // ONLY under setarch -R with this .so). All gated
                // on UMBRA_LEGO_DIAG; without it, none of this
                // fires and the file is generic NVN→Vulkan. The
                // PATCH/FORCE bypasses (UMBRA_C33_*) are also
                // sub-gated on this. Removable post-(N)-audio-fix.
                if(_legoDiag && isIndexed && d.VsShIdx == 813) {
                    if(_c29First == 0) _c29First = _frameN;
                    // (c³³)×4 one-shot patch: NOP cbnz w21
                    // gates at Inst::Update +0x260/+0x348.
                    // (c³⁵)×3(J) PATCH CI::OnProcess+0x34
                    // `tbz w9,#1 → +0x80` → NOP. Forces the
                    // bit-1=1 path = Timer::Update(LegoGet
                    // FrameTime()) at +0x7c regardless of
                    // SeedInfo bit-1. CLEAN discriminator: if
                    // cutscene plays WITHOUT +0x26c-NOP or
                    // FORCE, bit-1 IS the lever (Timer gets
                    // real dt → target advances → vel≠0 →
                    // playhead advances). If still frozen ⟹
                    // bit-1 wasn't it; +0x1a4 path's Timer::
                    // Update(0,…,w3=1) overrides regardless.
                    // Env: UMBRA_C35_PATCH_BIT1.
                    // (c³⁴)'s +0x26c-NOP kept under separate
                    // env (UMBRA_C33_PATCH_W21) for A/B.
                    if(_c35PatchBit1 && !_c33Patched) {
                        _c33Patched = true;
                        var ci = 0xfff5b195eb2cUL;
                        var pg = (nint)(ci & ~0xfffUL);
                        var rc = mprotect(pg, 0x2000, 7);
                        $"[c35] mprotect({pg:x},0x2000,RWX) rc={rc}".Log();
                        if(rc == 0) {
                            var p34 = (uint*)(ci+0x34);
                            $"[c35]   pre: CI+0x34=0x{*p34:x8}".Log();
                            *p34 = 0xd503201f; // NOP
                            __clear_cache((nint)(ci+0x34),
                                          (nint)(ci+0x38));
                            $"[c35]   PATCHED CI::OnProcess+0x34 (tbz bit-1) → NOP + icache".Log();
                        }
                    } else if(_c33Patch && !_c33Patched) {
                        _c33Patched = true;
                        var ci = 0xfff5b195eb2cUL;
                        var pg = (nint)(ci & ~0xfffUL);
                        var rc = mprotect(pg, 0x2000, 7);
                        $"[c34] mprotect({pg:x},0x2000,RWX) rc={rc}".Log();
                        if(rc == 0) {
                            var p26c = (uint*)(ci+0x26c);
                            $"[c34]   pre: CI+0x26c=0x{*p26c:x8}".Log();
                            *p26c = 0xd503201f; // NOP
                            __clear_cache((nint)(ci+0x26c),
                                          (nint)(ci+0x270));
                            $"[c34]   PATCHED CI::OnProcess+0x26c (vel←target−playhead) → NOP + icache".Log();
                        }
                    }
                    // (c³³)×2 (v) per-PRESENT FORCE (was per-
                    // 1500f inside c29 bucket-gate; +0x98 was
                    // back to 0 by next sample ⟹ something
                    // re-zeros it within ~225 Inst::Update
                    // calls). If THIS unfreezes c[3] ⟹ re-
                    // zero is per-call; trace it. If still
                    // frozen ⟹ +0x388 gate-side. Constant
                    // 1.0 (= 1 EDL-frame/Inst::Update-call ≈
                    // 30fps at sysFC≈225/1500f rate).
                    if(_c33ForceSpeed) {
                        var ih = *(ulong*)0xfff5b4c180f8UL;
                        if(ih != 0) {
                            *(float*)(ih+0x98) = 1.0f;
                            if(_c33ForceN++ < 3 || _c33ForceN%500==0)
                                $"[c33] f={_frameN} FORCE inst@{ih:x}+0x98 ← 1.0 (#{_c33ForceN})".Log();
                        }
                    }
                    // (c³⁶)×2 (P): FORCE NuSound device-count
                    // global ≥1. GetNumAvailableOutputDevices()
                    // = *[*[b444d000+2072]]; <1 → Timer::Update
                    // bypasses to +0xdc (no-advance). NuSound
                    // init left it 0 (‡ which IPC-stub?). Write
                    // 1 → see if gate-2 (TrackManager::IsEnabled)
                    // OR streamer-null is the next wall. Cheap
                    // operational test, NOT clean-fix.
                    // (c³⁶)×3 (R) discriminator: which arm
                    // freezes Timer::Update? devCount=1 ✓
                    // (gate-1 passes). Read: gate-2 byte +
                    // walk instHead→data→sequence→handles
                    // (= the timer's *[timer+0]+848/864).
                    // NuGCutSceneInst likely holds a NuCut
                    // Scene* (= the data, per UpdateCameras
                    // sig). instHead+0x70 = ‡ that ptr (per
                    // (c³²)×2 +0x90 ldr x23,[+0x108] was
                    // PARENT-inst; data is elsewhere).
                    if(_c29First != 0 && _c36ForceN++ < 8) {
                        var devGp = *(ulong*)(0xfff5b444d000UL+2072);
                        var devC = *(int*)devGp;
                        var tmByte = *(byte*)(*(ulong*)(0xfff5b4458000UL+3960));
                        var ih = *(ulong*)0xfff5b4c180f8UL;
                        // Walk inst+0x10..0x78 for ptrs (= find
                        // NuCutScene* data; it'll have +848/864
                        // SoundEventHandle slots).
                        var ptrs = "";
                        if(ih != 0) {
                            for(var po=0x10; po<=0x78; po+=8) {
                                var pv = *(ulong*)(ih+(ulong)po);
                                if(pv>0xfff500000000UL && pv<0xfff600000000UL)
                                    ptrs += $" +{po:x2}=0x{pv:x}";
                            }
                        }
                        $"[c36] f={_frameN} devC={devC} tmByte=0x{tmByte:x2}(gate-2 {(tmByte==0?"PASS":"FIRES")})  inst-ptrs:[{ptrs}]".Log();
                        // For each candidate ptr, probe +848/
                        // +864 (= SoundEventHandle slots if it's
                        // the sequence-data the Timer derefs).
                        if(ih != 0) {
                            for(var po=0x10; po<=0x78; po+=8) {
                                var pv = *(ulong*)(ih+(ulong)po);
                                if(pv<0xfff500000000UL || pv>=0xfff600000000UL) continue;
                                try {
                                    var h848 = *(ulong*)(pv+848);
                                    var h864 = *(ulong*)(pv+864);
                                    if(h848!=0 || h864!=0)
                                        $"[c36]   inst+{po:x2}→0x{pv:x}: [+848]=0x{h848:x} [+864]=0x{h864:x}  ◀━━ SoundEventHandle cand".Log();
                                } catch {}
                            }
                        }
                    }
                }
                if(_legoDiag && isIndexed && d.VsShIdx == 813
                        && (_frameN-_c29First) / 200 != _c29LastBucket) {
                    _c29LastBucket = (_frameN-_c29First) / 200;
                    var c3 = d.Ubos?[0]; var c6 = d.Ubos?[3];
                    var c3h = c3==null?0u:Hash16(c3);
                    var c6h = c6==null?0u:Hash16(c6);
                    var c3d = c3h != _c29PrevC3 ? "🔥CHANGED" : "frozen";
                    var c6d = c6h != _c29PrevC6 ? "🔥CHANGED" : "frozen";
                    $"[c29] f={_frameN} sh{d.VsShIdx} c[3]({c3d} h={c3h:x8}): {(c3==null?"NULL":FloatHex(c3,0,4))} cam@d0={(c3==null||c3.Length<0xdc?"—":FloatHex(c3,0xd0,3))} | c[6]({c6d} h={c6h:x8}): {(c6==null?"NULL":FloatHex(c6,0,4))}".Log();
                    _c29PrevC3 = c3h; _c29PrevC6 = c6h;
                    // (c³⁰)×4 3-way disc: read game-globals
                    // directly (same addr-space; setarch -R ⟹
                    // .so always at fff5b…). Settles:
                    //  (g) sysFC low ⟹ Sys::Update NOT called
                    //      (= LegoCutscenesManager::OnProcess
                    //      not dispatched OR early-outs before
                    //      Sys::Update bl).
                    //  (h) sysFC≈frameN, flags bit31|bit8 set ⟹
                    //      Inst::Update early-outs to +0xb20.
                    //  (i) sysFC≈frameN, flags clean ⟹ dt=0
                    //      (= mpd+36=0; trace LCM::Process's
                    //      caller).
                    try {
                        var instHead = *(ulong*)0xfff5b4c180f8UL;
                        var sysFC    = *(int*) (*(ulong*)(0xfff5b4463000UL+1544));
                        var x24flag  = *(sbyte*)(*(ulong*)(0xfff5b4456000UL+3368));
                        // (c³¹) NuFrameEnd's dt-global @ container
                        // (= *[0xfff5b442ff98] = 0xfff5b4accbb0)
                        // +1200/+1204. If THIS is nonzero ⟹
                        // NuFrameEnd computes dt fine; gap is in
                        // GameFramework::Process's mpd-build.
                        // If 0 ⟹ NuTime/AsSeconds path broken
                        // (= our CNTPCT/CNTFRQ).
                        var ctnr = *(ulong*)0xfff5b442ff98UL;
                        var dt1200 = *(float*)(ctnr+1200);
                        var dt1204 = *(float*)(ctnr+1204);
                        // + container+1208..1216 (= ‡ neighbors;
                        // GFP might read a DIFFERENT field).
                        var dt1208 = *(float*)(ctnr+1208);
                        var dt1212 = *(float*)(ctnr+1212);
                        // (c³⁴)(C) inst-list walk: instHead is
                        // a linked-list head (Sys::Update walks
                        // *[inst+0x0] per +0xd4/+0xd8). If >1
                        // inst, FORCE+c30 only touch head; the
                        // RENDERED inst (= feeds c[3]/c[6]) may
                        // be a different one.
                        var ilist = ""; var icnt = 0;
                        var cur = instHead;
                        while(cur != 0 && icnt < 8) {
                            var ph = *(float*)(cur+0x84);
                            var vl = *(float*)(cur+0x98);
                            var fl2 = *(uint*)(cur+0x7c);
                            ilist += $" [{icnt}]@{cur:x}:ph={ph:g4},vel={vl:g4},fl={fl2:x}";
                            cur = *(ulong*)cur; icnt++;
                        }
                        $"[c30] f={_frameN}  sysFC={sysFC} (Δf={sysFC-_c30PrevFC};{_frameN-_c30PrevF}f)  instHead=0x{instHead:x}  x24flag={x24flag}".Log();
                        $"[c34]   inst-list (n={icnt}):{ilist}".Log();
                        // (c³⁵)×3 (K): read CutsceneSeedInfo+904
                        // flags directly. Path: LegoCutscenes
                        // singleton = *[*[b443b000+1832]] (per
                        // CI+0x1a8). [LC+336] = ‡ active CI.
                        // Then SeedInfo = *[*[CI+0xe0]+104]
                        // (per GetSeedInfo body). [+904] = the
                        // bit-flags word. ALSO read the ctor-
                        // config global *[*[b4431000+1480]]
                        // (= cutscene_start's bit-{0,4} source).
                        try {
                            var lc = *(ulong*)(*(ulong*)(0xfff5b443b000UL+1832));
                            var ciP = *(ulong*)(lc+336);
                            var cfg = *(byte*)(*(ulong*)(0xfff5b4431000UL+1480));
                            // (c³⁵)×4 (K-fix): [LC+336] isn't
                            // the active CI (it's the master-
                            // ref per +0x180). Try BOTH: dump
                            // ciP's struct + ALSO scan for
                            // which heap obj has [+0x58]==
                            // instHead (= the CI that owns
                            // our NuGCutSceneInst). For now:
                            // just probe ciP deeper + dump
                            // raw [+0xe0]/[+0x58] to see what
                            // it actually is.
                            string seedS = "";
                            ulong realCI = 0;
                            if(ciP != 0) {
                                var ci58 = *(ulong*)(ciP+0x58);
                                var cie0 = *(ulong*)(ciP+0xe0);
                                var civt = *(ulong*)ciP;
                                seedS += $" ciP[vt=0x{civt:x},+58=0x{ci58:x},+e0=0x{cie0:x}]";
                                // Is ciP the active CI? check
                                // [+0x58]==instHead.
                                if(ci58 == instHead) realCI = ciP;
                            }
                            // Also: instHead has back-ref?
                            // Try common offsets for owner-ptr.
                            if(realCI == 0 && instHead != 0) {
                                // ‡ guess: NuGCutSceneInst may
                                // have owner-CI ptr somewhere
                                // in 0x100..0x140 (per (c³²)
                                // band — +0x108=parent-cutscene
                                // =0; check +0x118/+0x120).
                                for(var po=0x100; po<=0x148; po+=8) {
                                    var pv = *(ulong*)(instHead+(ulong)po);
                                    if(pv>0xfff500000000UL && pv<0xfff600000000UL) {
                                        var pv58 = *(ulong*)(pv+0x58);
                                        if(pv58 == instHead) {
                                            realCI = pv;
                                            seedS += $" [back-ref@inst+{po:#x}]";
                                            break;
                                        }
                                    }
                                }
                            }
                            if(realCI != 0) {
                                var pkt = *(ulong*)(realCI+0xe0);
                                var seed = pkt!=0 ? *(ulong*)(pkt+104) : 0;
                                var tmr  = pkt!=0 ? *(ulong*)(pkt+0x60) : 0;
                                seedS += $" CI=0x{realCI:x} pkt=0x{pkt:x}";
                                if(seed != 0) {
                                    var f904 = *(uint*)(seed+904);
                                    var f908 = *(byte*)(seed+908);
                                    var bits = "";
                                    for(var b=0;b<40;b++) {
                                        var bv = b<32 ? (f904>>b&1) : (uint)(f908>>(b-32)&1);
                                        if(bv!=0) bits+=$"{b},";
                                    }
                                    seedS += $" 🔥seed[+904]=0x{f904:x8}[+908]=0x{f908:x2} bits=[{bits}]";
                                }
                                if(tmr != 0) {
                                    var t8 = *(float*)(tmr+8);
                                    var t12 = *(float*)(tmr+12);
                                    var t0 = *(uint*)(tmr+0);
                                    var t4 = *(uint*)(tmr+4);
                                    var t16 = *(float*)(tmr+16);
                                    var t20 = *(float*)(tmr+20);
                                    seedS += $" tmr=0x{tmr:x}[0]=0x{t0:x}[4]=0x{t4:x}[8]={t8:g6}[12]={t12:g6}[16]={t16:g6}[20]={t20:g6}";
                                }
                            } else {
                                seedS += " [no CI found]";
                            }
                            $"[c35]   LC=0x{lc:x} cfg=0x{cfg:x2}{seedS}".Log();
                        } catch(Exception ex) {
                            $"[c35]   ✗ seedInfo-read fault: {ex.Message}".Log();
                        }
                        $"[c31]   NuFrameEnd dt-global ctnr=0x{ctnr:x}: [+1200]={dt1200:g6} [+1204]={dt1204:g6} [+1208]={dt1208:g6} [+1212]={dt1212:g6}".Log();
                        // (c³¹)×4 GF_this fields directly (=
                        // the kt[34](a) verify-not-infer step).
                        // GF_this = **[0xfff5b44571f8] per
                        // GameFrameworkAPI::GetFrameTime().
                        // [+13900]=raw s0×s1; [+13904]=smoothed
                        // dt-cache; [+14812]=mpd+36-source;
                        // [+13912]=accum; [+13920]=tickN(int).
                        // + [ctnr+1236] (= the if-zero gate at
                        // GFP+0x8f0). + [+13924] (= w8 read at
                        // +0x8c8, ‡ paused-flag?).
                        var gfP = *(ulong*)(*(ulong*)0xfff5b44571f8UL);
                        var raw  = *(float*)(gfP+13900);
                        var sm   = *(float*)(gfP+13904);
                        var src  = *(float*)(gfP+14812);
                        var acc  = *(float*)(gfP+13912);
                        var tkN  = *(int*)  (gfP+13920);
                        var w8c8 = *(int*)  (gfP+13924);
                        var c1236= *(float*)(ctnr+1236);
                        // (c³³)×5 [GF+13944] = mpd+40 source
                        // (= Inst::Update's w21 gate). If ≠0
                        // ⟹ w21-gate IS the freeze. + ctor
                        // confirmed +0x98←1.0; if zeroed,
                        // sysFC≈225/1500 says ~225 calls ran
                        // — read +0x98's neighbor +0xa0 (=
                        // CreateFixPtrs also writes, ‡ same
                        // fate?) for cross-check.
                        var m40 = *(int*)(gfP+13944);
                        var ihh = *(ulong*)0xfff5b4c180f8UL;
                        var fa0 = ihh!=0 ? *(float*)(ihh+0xa0) : -1f;
                        $"[c31]   GF_this=0x{gfP:x}: raw[+13900]={raw:g6} sm[+13904]={sm:g6} 🔥src[+14812]={src:g6} acc={acc:g6} tkN={tkN} w8={w8c8} ctnr+1236={c1236:g6}".Log();
                        $"[c33]   🔥 GF[+13944](=mpd+40=w21)={m40}  inst+0xa0(ctor-sibling)={fa0:g6}".Log();
                        var v31 =
                            src != 0 ? "🔥 src≠0 ⟹ mpd+36 NONZERO ⟹ (d')-dt=0 inference WRONG; freeze is DOWNSTREAM (Inst::Update has 2nd gate, OR LegoCutscenesManager::OnProcess early-outs before bl Sys::Update)"
                          : sm  != 0 ? "sm≠0 src=0 ⟹ +0x988 not reached (gate +0x95c/+0x960?)"
                          : raw != 0 ? "raw≠0 sm=0 ⟹ smoothing zeroes (lerp const @b3733f3c=0?)"
                          : "raw=0 ⟹ s0×s1=0 at +0x8cc (s0 or s1 = 0; need +0x8b8..+0x8cc disasm)";
                        $"[c31]   ⟹ {v31}".Log();
                        if(instHead != 0) {
                            var fl  = *(uint*)(instHead+0x7c);
                            var b81 = *(byte*)(instHead+0x81);
                            var f84 = *(float*)(instHead+0x84);
                            var f98 = *(float*)(instHead+0x98);
                            // ‡ inst+0x88..0x94 region — what's
                            // the cutscene's own time/frame?
                            var f88 = *(float*)(instHead+0x88);
                            var f8c = *(float*)(instHead+0x8c);
                            var f90 = *(float*)(instHead+0x90);
                            var f94 = *(float*)(instHead+0x94);
                            $"[c30]   inst+0x7c flags=0x{fl:x8} b31={(fl>>31)&1} b8={(fl>>8)&1}  +0x81={b81}(vs sysFC%256={sysFC&0xff})  +0x84(rate)={f84:g6} +0x98={f98:g6}".Log();
                            $"[c30]   inst+0x88..94: {f88:g6} {f8c:g6} {f90:g6} {f94:g6}".Log();
                            // (c³²)×2 wide-band: read inst+
                            // {0xa0..0x12c} as float[36] +
                            // diff against prev. Per (n)
                            // census, accumulators at {0xac,
                            // 0x110, 0x11c}; +0x90=MayaFrame.
                            // CHANGED-set = the live fields;
                            // STUCK-set ∩ {accumulators} =
                            // the freeze.
                            var band = new float[36];
                            for(var k=0;k<36;k++)
                                band[k] = *(float*)(instHead+(ulong)(0xa0+k*4));
                            var chg = "";
                            if(_c32PrevBand != null)
                                for(var k=0;k<36;k++)
                                    if(band[k] != _c32PrevBand[k])
                                        chg += $" +{0xa0+k*4:x}={_c32PrevBand[k]:g4}→{band[k]:g4}";
                            $"[c32]   inst+0xa0..0x12c CHANGED:{(chg==""?" (none — all 36 fields stuck)":chg)}".Log();
                            $"[c32]   key: +0xac={band[3]:g6} +0x108={band[26]:g6} +0x110={band[28]:g6} +0x11c={band[31]:g6} +0x90(MayaFrame)={f90:g4}".Log();
                            // + deref candidate ptr fields
                            // (= x23 source? inst+0x?? →
                            // NuCutScene* data). Try the
                            // ptr-looking ones (8-aligned,
                            // value in 0xfff5… range).
                            for(var po=0x10; po<=0x70; po+=8) {
                                var pv = *(ulong*)(instHead+(ulong)po);
                                if(pv>=0xfff500000000UL && pv<0xfff600000000UL) {
                                    var p152 = *(float*)(pv+152);
                                    $"[c32]   inst+{po:#x}=0x{pv:x} → [+152]={p152:g6}  (= x23 cand; if 0 ⟹ +0x110 stuck-source)".Log();
                                }
                            }
                            _c32PrevBand = band;
                            // (c³²)×3 OPERATIONAL TEST: force
                            // inst+0x98 (playback-velocity) =
                            // src[+14812] × 30 (= dt×30fps).
                            // Per +0x388: playhead += [+0x98]
                            // × d9(=1.0). With +0x98=0 ⟹
                            // frozen. d8(=dt) NOT used for
                            // playhead (only sub-frame interp
                            // to UpdateCameras). If THIS makes
                            // c[3] change ⟹ +0x98 IS the
                            // lever; ×4 = trace who should
                            // set it (Start? external Play?).
                            // Env-gated; write-once-per-
                            // sample (Inst::Update only fneg's
                            // it, never zeros, so persists).
                            if(Environment.GetEnvironmentVariable(
                                    "UMBRA_C32_FORCE_SPEED") != null) {
                                var spd = src * 30f;
                                *(float*)(instHead+0x98) = spd;
                                $"[c32]   FORCE inst+0x98 ← {spd:g6} (= src×30)".Log();
                            }
                            var verdict =
                                sysFC <= 1 ? "🔥(g) Sys::Update NOT CALLED — LegoCutscenesManager::OnProcess not dispatched/early-outs"
                              : ((fl>>31)&1)!=0 || ((fl>>8)&1)!=0 ? $"🔥(h) Inst::Update EARLY-OUTS on flags bit{(((fl>>31)&1)!=0?31:8)} → +0xb20 (Start/End state-machine)"
                              : "🔥(i) Sys::Update called + flags clean ⟹ dt path. Check inst+0x88..94 for time-not-advancing.";
                            $"[c30]   ⟹ {verdict}".Log();
                        } else {
                            $"[c30]   ⟹ instHead=0 — no cutscene inst registered (= Sys::Update list empty)".Log();
                        }
                        _c30PrevFC = sysFC; _c30PrevF = _frameN;
                    } catch(Exception ex) {
                        $"[c30]   ✗ global-read fault: {ex.Message}".Log();
                    }
                }
                // (c²⁸)×4 one-shot full dump (kept):
                // (c³⁹)×3 gate-fix: relative to _c29First
                // (was _frameN>=35000, stale from pre-Report
                // Counter timeline). Fire ~100f after first
                // sh813 (= cutscene running, c[3] settled).
                if(_legoDiag && isIndexed && !_c28Dumped
                        && _c29First > 0
                        && _frameN >= _c29First + 100
                        && d.VsShIdx == 813) {
                    _c28Dumped = true;
                    var c3 = d.Ubos?[0]; var c4 = d.Ubos?[1];
                    var c5 = d.Ubos?[2]; var c6 = d.Ubos?[3];
                    var c7 = d.Ubos?[4];
                    $"[c28] f={_frameN} sh{d.VsShIdx}/{d.FsShIdx} idxN={d.IdxCount} baseVtx={d.BaseVtx} str={stride} vbSz=0x{d.VbSize:x}".Log();
                    $"[c28]   c[3] (slot0, {c3?.Length ?? -1}B): {(c3==null?"NULL":FloatHex(c3,0,16))}".Log();
                    $"[c28]   c[3] @0xd0..: {(c3==null||c3.Length<0xdc?"—":FloatHex(c3,0xd0,3))}".Log();
                    $"[c28]   c[4] (slot1, {c4?.Length ?? -1}B): {(c4==null?"NULL":FloatHex(c4,0,16))}".Log();
                    $"[c28]   c[5] (slot2, {c5?.Length ?? -1}B): {(c5==null?"NULL":FloatHex(c5,0,8))}".Log();
                    $"[c28]   c[6] (slot3, {c6?.Length ?? -1}B): {(c6==null?"NULL":FloatHex(c6,0,8))}".Log();
                    $"[c28]   c[7] (slot4, {c7?.Length ?? -1}B): {(c7==null?"NULL":FloatHex(c7,0,4))}".Log();
                    // (c³⁹)×3 (P') FS-stage cbufs: sh0814 RGB
                    // = (…tex-derived…) × cbuf4[5].y; α =
                    // (…) × cbuf3[0].w. cbuf4 = FS c[4] = NVN
                    // FS slot 1 = d.Ubos[8+1]. If [5].y = 0
                    // ⟹ RGB=0 regardless of tex. sh0442 RGB
                    // × %875 (untraced); α × cbuf1[1].y (=
                    // driver-managed, KNOWN-zero-by-us).
                    var fc3 = d.Ubos?[8+0]; var fc4 = d.Ubos?[8+1];
                    var fc5 = d.Ubos?[8+2]; var fc1 = (byte[]?)null;
                    $"[c39]   FS c[3] (FSslot0,{fc3?.Length??-1}B) [0]={(fc3==null?"NULL":FloatHex(fc3,0,8))}".Log();
                    $"[c39]   FS c[4] (FSslot1,{fc4?.Length??-1}B) [0..7]={(fc4==null?"NULL":FloatHex(fc4,0,8))}".Log();
                    $"[c39]   🔥 FS c[4][5].y (= sh0814 RGB-mult @off 84): {(fc4==null||fc4.Length<88?"—":FloatHex(fc4,80,4))}".Log();
                    $"[c39]   FS c[5] (FSslot2,{fc5?.Length??-1}B) [0..3]={(fc5==null?"NULL":FloatHex(fc5,0,4))}".Log();
                    // + d.Tex resolved (= what texture is
                    // bound to all 40 slots for THIS draw?)
                    var tx = d.Tex;
                    $"[c39]   d.TexHandle=0x{d.TexHandle:x} → tex={(tx==null?"NULL":$"{tx.Width}×{tx.Height} fmt=0x{tx.Format:x} rgba={(tx.Rgba==null?"null":$"{tx.Rgba.Length}B")}")}".Log();
                    // (c⁴⁰)×5: dump full FS TexHandles[] WITH
                    // per-slot fmt+dims (= which slots have
                    // content textures vs RTs?). The c28-trigger
                    // sh813 draw at ph≈100.
                    if(d.TexHandles != null) {
                        var ts = "";
                        for(var sl=0; sl<40; sl++) {
                            var h = d.TexHandles[sl];
                            if(h == 0) continue;
                            var ti = (int)(h>>32);
                            var t = NvnLinux.ResolveTex(h);
                            ts += $"\n[c40]     [{sl}]=0x{ti:x}"
                               + (t==null ? " NULL"
                                 : $" {t.Width}×{t.Height} fmt=0x{t.Format:x}"
                                 + (t.Format>=0x42&&t.Format<=0x49?" BC":"")
                                 + (t.Width==1920?" RT":"")
                                 + (t.Rgba==null?" rgba=null"
                                   :$" rgba[0..3]={t.Rgba[0]:x2}{t.Rgba[1]:x2}{t.Rgba[2]:x2}{t.Rgba[3]:x2}"));
                        }
                        $"[c40]   FS TexHandles per-slot for sh{d.VsShIdx}/{d.FsShIdx}:{ts}".Log();
                    }
                    // first 3 verts at baseVtx (raw + as f32)
                    var vp = (byte*)d.VbCpu + (long)d.BaseVtx*stride;
                    for(var v=0; v<3; v++) {
                        var s = $"[c28]   vtx[{d.BaseVtx+v}] raw=";
                        for(var k=0;k<stride;k++) s+=$"{vp[v*stride+k]:x2}";
                        if(stride>=12) {
                            var f=(float*)(vp+v*stride);
                            s+=$"  asF32=({f[0]:g6},{f[1]:g6},{f[2]:g6})";
                        }
                        s.Log();
                    }
                    // first 3 indices
                    if(d.IdxCpu!=0 && d.IdxType==1) {
                        var ip=(ushort*)d.IdxCpu;
                        $"[c28]   idx[0..2]={ip[0]},{ip[1]},{ip[2]}".Log();
                    }
                }
                vkCmdBindDescriptorSets(_cmdBuf, 0, _t3PipelineLayout,
                    0, 3, t3sets, 24, t3dyn);
                if(isIndexed)
                    vkCmdDrawIndexed(_cmdBuf, (uint)drawCount, 1,
                        0, d.BaseVtx, 0);
                else
                    vkCmdDraw(_cmdBuf, (uint) drawCount, 1, (uint) drawFirst, 0);
                vbo += (bytes + 255) & ~255ul;
                recN++;
            }
            if(_t3Rtt && curRtId >= 0)
                rtDrawN[curRtId] += recN - rtDrawNAt;
            vkCmdEndRenderPass(_cmdBuf);
            // (T6)×36: RT-readback. After all RPs closed,
            // copy the requested RT → host-staging. RT
            // finalLayout=GENERAL per EnsureRtFb ⟹ no
            // barrier needed (srcLayout=GENERAL=1).
            if(_t3Rtt && _t3RttDump >= 0
               && _rtFb[_t3RttDump % 100] is {} drf
               && drf.Rp != _renderPass) {  // = real RT, not swap-fallback
                if(_rtDumpBuf == 0) {
                    // Lazy-alloc staging (host-mapped,
                    // usage=TRANSFER_DST=2).
                    var bci = new VkBufferCreateInfo {
                        sType = ST_BUFFER_CI, size = RtDumpBufSz,
                        usage = 2,
                    };
                    ulong b; Chk(vkCreateBuffer(_dev, &bci, null, &b),
                        "vkCreateBuffer(rtDump)");
                    _rtDumpBuf = b;
                    VkMemoryRequirements mr;
                    vkGetBufferMemoryRequirements(_dev, b, &mr);
                    var mai = new VkMemoryAllocateInfo {
                        sType = ST_MEM_AI, allocationSize = mr.size,
                        memoryTypeIndex = _hostMemType,
                    };
                    ulong mm; Chk(vkAllocateMemory(_dev, &mai, null,
                        &mm), "vkAllocateMemory(rtDump)");
                    _rtDumpMem = mm;
                    Chk(vkBindBufferMemory(_dev, b, mm, 0),
                        "vkBindBufferMemory(rtDump)");
                    void* p; Chk(vkMapMemory(_dev, mm, 0,
                        RtDumpBufSz, 0, &p), "vkMapMemory(rtDump)");
                    _rtDumpPtr = (byte*)p;
                    $"[vk] rtDump staging: 32MB host-mapped @0x{(ulong)p:x}".Log();
                }
                // (T6)×36 ×2-cont disc: prefill staging
                // with 0xAB so DumpRtPpm can detect "copy
                // didn't write" (= 0xAB survives) vs "RT
                // genuinely clear-value" (= 0x00/0xFF).
                for(var k = 0; k < RtDumpBufSz; k += 64)
                    _rtDumpPtr[k] = 0xAB;
                // DUMP=N+100 ⟹ rtId=N's DEPTH attachment
                // (= kt[28] different-observable: if rt1
                // color is empty but depth has geometry,
                // it's FS-output not rasterization).
                var dumpDepth = _t3RttDump >= 100;
                var srcImg = (drf.NC > 0 && !dumpDepth)
                    ? drf.Img[0] : drf.DepthImg;
                var asp = (drf.NC > 0 && !dumpDepth)
                    ? 1u : 2u;  // COLOR : DEPTH
                var bic = new VkBufferImageCopy {
                    aspectMask = asp, layerCount = 1,
                    ew = (uint)drf.W, eh = (uint)drf.H, ed = 1,
                };
                vkCmdCopyImageToBuffer(_cmdBuf, srcImg, 1,
                    _rtDumpBuf, 1, &bic);
                $"[vk] rtDump record: srcImg=0x{srcImg:x} buf=0x{_rtDumpBuf:x} {drf.W}×{drf.H} asp={asp} cb=0x{_cmdBuf:x}".Log();
            }
            if(_t3Rtt && nRpSwitch > 0) {
                var hist = string.Join(" ", rtDrawN
                    .Select((n,i) => (n,i)).Where(x => x.n > 0)
                    .Select(x => $"rt{x.i}×{x.n}"));
                $"[vk] RTT frame={_frameN}: {nRpSwitch} RP-switches, {recN} drawn → {hist}".Log();
            }
            _lastDrawN = draws.Length;
            if(_frameN <= 5 || (recN > 0 && _frameN % 10 == 0))
                {
                    var pairs = string.Join(" ", draws
                        .Select(d => (d.VsShIdx, d.FsShIdx, d.TexHandle))
                        .Distinct()
                        .Select(p => $"vs{p.VsShIdx}/fs{p.FsShIdx}/tx{p.TexHandle:x}"));
                    {
                        // instrument: per-distinct-tex resolve log.
                        var txs = string.Join(",", draws.Take(recN)
                            .GroupBy(d => d.TexHandle).Select(g => {
                                var t = g.First().Tex;
                                return $"tx{g.Key:x}→{(t==null?"∅":$"{t.Width}×{t.Height}f{t.Format:x}")}";
                            }));
                        $"[vk] RecordDrawPass(T3) frame={_frameN} draws={recN} idx={nIdx}/{nIdx+nIdxSkip} skipPP={nPostSkip} vbufUsed={vbo} ibufUsed={ibo} [{pairs}] tex:[{txs}]".Log();
                    // (c³⁴)×4-redo: 3D-onset-RELATIVE auto-
                    // dump (kt[32]-shape: onset varies ~1200f
                    // run-to-run; absolute-frame-N capture
                    // structurally wrong). When idx>50 first
                    // appears, dump every 40f for 12 frames.
                    // Robust to onset-variance + doesn't slow
                    // loading-phase with pre-3D readbacks.
                    if(_dump3d && nIdx > 50) {
                        if(_dump3dFirst == 0) {
                            _dump3dFirst = _frameN;
                            $"[c34] 3D-onset @ f{_frameN} (idx={nIdx}) — auto-dump every 40f ×12".Log();
                        }
                        var rel = _frameN - _dump3dFirst;
                        if(rel % 40 == 0 && rel/40 < 12)
                            _dump3dWant = _frameN;  // Present reads this
                    }
                    }
                }
            return;
        }

        // ── T2 PATH (fixed shaders + push-constants) ──
        vkCmdBindPipeline(_cmdBuf, 0, _pipeline); // GRAPHICS
        if(_atlasReady) {
            ulong ds = _dsAtlas;
            vkCmdBindDescriptorSets(_cmdBuf, 0, _pipelineLayout, 0, 1, &ds, 0, null);
        }

        // For each draw: copy game's vbuf into our host-mapped buffer at
        // advancing offset, bind, push per-draw constants, draw.
        ulong off = 0;
        int rowI = 0;
        ulong vbuf = _vbuf;
        // pcBuf[0..15] = proj×view (CPU-composed once from slot-0 [0..15]
        // and [16..31]); [16..31] = model (slot-2 per-draw). v1.5's
        // proj-only depth-clipped because view (tz=-7) wasn't applied;
        // v2(a) composes them CPU-side so it fits in 128B. ‡ Assumes
        // slot-0 layout = {proj mat4 @0, view mat4 @64} per umbra65 dump.
        var pcBuf = stackalloc float[32];
        if(NvnLinux.ProjUbo is { Length: >= 128 } pj) {
            // Column-major mat4 multiply: pv = proj × view.
            float P(int c, int r) => BitConverter.ToSingle(pj, (c*4+r)*4);
            float V(int c, int r) => BitConverter.ToSingle(pj, 64 + (c*4+r)*4);
            for(var col = 0; col < 4; col++)
                for(var row = 0; row < 4; row++)
                    pcBuf[col*4+row] =
                        P(0,row)*V(col,0) + P(1,row)*V(col,1) +
                        P(2,row)*V(col,2) + P(3,row)*V(col,3);
        } else { pcBuf[0]=pcBuf[5]=pcBuf[10]=pcBuf[15]=1f; }
        foreach(var d in draws) {
            if(d.VbCpu == 0 || d.Count <= 0) continue;
            var bytes = (ulong) d.Count * 36;
            if(off + bytes > VbufSize) break;  // ‡ v0: drop overflow
            Buffer.MemoryCopy((void*) d.VbCpu, _vbufPtr + off, bytes, bytes);
            ulong vbOff = off;
            vkCmdBindVertexBuffers(_cmdBuf, 0, 1, &vbuf, &vbOff);
            // v1.5: proj (slot-0, set above once) + model (slot-2 per-draw).
            // Game's VS does proj × model × pos; we do the same. ‡ slot-0
            // [16..31] (view, tz=-7) not composed — proj[0..15] may already
            // be proj×view (the -7 in both suggests precomputed). Per drop-to-bytes:
            // let the GPU do the math, see what comes out.
            if(d.Ubo is { Length: >= 64 }) {
                for(var k = 0; k < 16; k++)
                    pcBuf[16+k] = BitConverter.ToSingle(d.Ubo, k*4);
            } else {
                for(var k = 0; k < 16; k++) pcBuf[16+k] = 0;
                pcBuf[16]=pcBuf[21]=pcBuf[26]=pcBuf[31]=1f;
            }
            vkCmdPushConstants(_cmdBuf, _pipelineLayout, 0x1, 0, 128, pcBuf);
            vkCmdDraw(_cmdBuf, (uint) d.Count, 1, (uint) d.First, 0);
            off += (bytes + 255) & ~255ul;
            rowI++;
        }
        vkCmdEndRenderPass(_cmdBuf);
        _lastDrawN = draws.Length;
        if(_frameN <= 5 || (draws.Length > 0 && _frameN % 10 == 0))
            $"[vk] RecordDrawPass frame={_frameN} idx={idx} draws={draws.Length} vbufUsed={off}".Log();
    }
    static int _lastDrawN, _drawFrameN;
    static int _dumpedWithDraws;

    // (r) Present-rate throttle. UMBRA_QUIET removed the per-SVC
    // backtrace cost → Present-spin at ~1500fps → bg-load thread
    // (NuBgProcManager, h1f) starves → splash never finishes load
    // (a test run: 201/573 GAME.DAT reads vs a test run @335fps). UMBRA_FPS_CAP
    // = N → spin-wait until ≥1000/N ms since last Present. Spin
    // (not Sleep) so wall-clock-gated game logic still sees real
    // time pass; the spin yields CPU via Thread.Yield() so bg-
    // threads run.
    static readonly int _fpsCapUs = int.TryParse(
        Environment.GetEnvironmentVariable("UMBRA_FPS_CAP"), out var fc) && fc > 0
            ? 1_000_000 / fc : 0;
    static long _lastPresentTick;

    // Present-watchdog: polls every 1s; if _frameN hasn't moved
    // for >5s, dump CondVarKernel state. = the instrument
    // for the cv-race hang (A/B runs/A/B runs all-threads-futex_wait).
    static void Watchdog() {
        // UMBRA_WD_TIMEOUT = seconds before force-break (default 4).
        // Lower = faster recovery from re-deadlocking runs; too low
        // risks force-breaking a slow-but-progressing load (the 16MB
        // BC3 decode at copy-time can take >1s on first hit).
        var fbAt = int.TryParse(
            Environment.GetEnvironmentVariable("UMBRA_WD_TIMEOUT"),
            out var fbT) ? Math.Max(1, fbT) : 4;
        int last = -1, stuck = 0;
        while(true) {
            Thread.Sleep(1000);
            if(_frameN == last) {
                if(stuck == fbAt && CondVarKernel.ForceBreak()) {
                    $"[wd] force-broke @ frame {_frameN}; resuming".Log();
                    stuck = 0; continue;
                }
                if(++stuck == fbAt + 1 || (stuck > fbAt && stuck % 30 == 0)) {
                    $"[wd] STALL: Present stuck {stuck}s @ frame {_frameN}".Log();
                    CondVarKernel.DumpState($"wd-{stuck}s");
                    foreach(var t in Kernel.ThreadManager.Threads)
                        $"[wd]   kthread h=0x{t.Handle:X} core={t.IdealCore}".Log();
                }
            } else {
                last = _frameN; stuck = 0;
                // ── bg-watchdog: when Present IS moving, check each
                //    game-thread's reentry-N for staleness. A thread
                //    whose N hasn't changed in >5s while Present
                //    advances = blocked at SVC-level invisible to the
                //    Present-stall watchdog above. Dump CondVarKernel
                //    state ONCE per stalled tid.
                if(Environment.GetEnvironmentVariable("UMBRA_BG_WD") != null) {
                    var now = System.Diagnostics.Stopwatch.GetTimestamp();
                    foreach(var (tid, s) in CondVarKernel.ThreadSnap) {
                        if(_bgLastN.TryGetValue(tid, out var prev)) {
                            if(s.N == prev.N) {
                                var stuckS = (now - prev.Ts) / (double)System.Diagnostics.Stopwatch.Frequency;
                                if(stuckS > 5 && !_bgDumped.Contains(tid)) {
                                    _bgDumped.Add(tid);
                                    $"[wd-bg] tid={tid} stuck {stuckS:F1}s @ N={s.N} op={s.OpA:x} lr={s.Lr:x} fp={s.Fp:x} (Present @{_frameN})".Log();
                                    CondVarKernel.DumpState($"bg-stall-t{tid}");
                                }
                            } else {
                                _bgLastN[tid] = (s.N, now);
                                _bgDumped.Remove(tid);
                            }
                        } else _bgLastN[tid] = (s.N, now);
                    }
                }
            }
        }
    }
    static readonly Dictionary<int,(ulong N,long Ts)> _bgLastN = new();
    static readonly HashSet<int> _bgDumped = new();
    static Func<int, bool>? _dumpFrames;
    static Func<int, bool> ParseDumpFrames() {
        var env = Environment.GetEnvironmentVariable("UMBRA_DUMP_FRAMES");
        if(env == null) return _ => false;
        // "onchange" — capture whenever _lastDrawN differs from
        // prior frame (= state transitions; the title appears at
        // wall-clock-relative load-completion, so frame# varies
        // 5K-30K depending on Present-spin-rate during load).
        // + 5 frames after each change (= settle).
        if(env == "onchange") {
            int prev = -1, since = 0;
            return _ => {
                if(_lastDrawN != prev) { prev = _lastDrawN; since = 0; return true; }
                return since++ < 5;
            };
        }
        if(env.StartsWith("every:")) {
            var k = int.Parse(env[6..]);
            return n => n % k == 0;
        }
        var set = env.Split(',').Select(int.Parse).ToHashSet();
        return n => set.Contains(n);
    }

    static void TransitionAndClear(int idx, float r, float g, float b, float a) {
        var range = new VkImageSubresourceRange { aspectMask = 1, levelCount = 1, layerCount = 1 };
        // UNDEFINED → GENERAL (clear needs GENERAL or TRANSFER_DST_OPTIMAL).
        var bar = new VkImageMemoryBarrier {
            sType = ST_IMG_BARRIER,
            srcAccessMask = 0, dstAccessMask = 0x1000,  // TRANSFER_WRITE
            oldLayout = 0, newLayout = 1,               // UNDEFINED → GENERAL
            srcQueueFamilyIndex = ~0u, dstQueueFamilyIndex = ~0u,
            image = _swapImg[idx], subresourceRange = range,
        };
        vkCmdPipelineBarrier(_cmdBuf, 0x1, 0x1000, 0,   // TOP_OF_PIPE → TRANSFER
            0, null, 0, null, 1, &bar);
        var col = new VkClearColorValue { r = r, g = g, b = b, a = a };
        vkCmdClearColorImage(_cmdBuf, _swapImg[idx], 1, &col, 1, &range);  // layout=GENERAL
    }

    // (T6)×36: per-fmt decode of _rtDumpPtr → ppm. Same
    // output path as DumpPpm so NvnReplay's rename
    // (umbra-frame-K → replay-fN-iK) works unchanged.
    static void DumpRtPpm(int frameN) {
        var rtId = _t3RttDump % 100;
        var dumpDepth = _t3RttDump >= 100;
        if(_rtDumpPtr == null || _rtFb[rtId] is not {} rf)
            return;
        // (T6)×36 ×2-cont disc: did the copy WRITE? Check
        // 0xAB sentinel survival across first 64KB.
        var nAB = 0;
        for(var k = 0; k < (1<<16); k += 64)
            if(_rtDumpPtr[k] == 0xAB) nAB++;
        if(nAB > 512)
            $"[vk] ⚠ DumpRtPpm: 0xAB-sentinel {nAB}/1024 — copy may not have executed (fence-timeout?)".Log();
        var path = $"/tmp/umbra-frame-{frameN:d3}.ppm";
        var sig = rtId < NvnLinux.RtSigs.Count
            ? NvnLinux.RtSigs[rtId] : null;
        var nf = (rf.NC > 0 && !dumpDepth)
            ? (sig?.Colors[0].Fmt ?? 0x25)
            : (sig?.Depth?.Fmt ?? 0x34);
        var (vf, _, _) = NvnRtFmt(nf);
        var w = rf.W; var h = rf.H;
        try {
            using var fs = File.Create(path);
            fs.Write(System.Text.Encoding.ASCII
                .GetBytes($"P6\n{w} {h}\n255\n"));
            var s = _rtDumpPtr;
            var row = new byte[w * 3];
            for(var y = 0; y < h; y++) {
                for(var x = 0; x < w; x++) {
                    byte r, g, b;
                    switch(vf) {
                    case 37: {  // RGBA8: direct
                        var p = s + (y*w+x)*4;
                        r = p[0]; g = p[1]; b = p[2];
                        break; }
                    case 97: {  // RGBA16F: Reinhard tonemap
                        var p = (ushort*)(s + (y*w+x)*8);
                        static byte tm(ushort h) {
                            var f = (float)BitConverter
                                .UInt16BitsToHalf(h);
                            f = MathF.Max(0, f);
                            return (byte)(255 * f / (f + 1));
                        }
                        r = tm(p[0]); g = tm(p[1]); b = tm(p[2]);
                        break; }
                    case 126: { // D32F: depth→gray
                        var d = *(float*)(s + (y*w+x)*4);
                        // ‡ depth often clusters near 1.0;
                        // invert+stretch for visibility:
                        // gray = (1−d)^0.4 × 255.
                        var v = (byte)(255 * MathF.Pow(
                            MathF.Max(0, 1 - d), 0.4f));
                        r = g = b = v;
                        break; }
                    case 77: {  // RG16_UNORM: r,g,128
                        var p = (ushort*)(s + (y*w+x)*4);
                        r = (byte)(p[0] >> 8);
                        g = (byte)(p[1] >> 8);
                        b = 128;
                        break; }
                    default: r = g = b = 0; break;
                    }
                    row[x*3] = r; row[x*3+1] = g; row[x*3+2] = b;
                }
                fs.Write(row);
            }
            $"[vk] DumpRtPpm rt{_t3RttDump} {w}×{h} nf=0x{nf:x}(vf{vf}) → {path}".Log();
        } catch(Exception e) {
            $"[vk] DumpRtPpm failed: {e.Message}".Log();
        }
    }

    static void DumpPpm(int idx, int frameN) {
        // (T6)×36: when RT-dump active, write the RT
        // staging instead of swap (same path).
        if(_t3Rtt && _t3RttDump >= 0 && _rtDumpPtr != null) {
            DumpRtPpm(frameN); return;
        }
        var path = $"/tmp/umbra-frame-{frameN:d3}.ppm";
        // ‡ v0.5: assume row-pitch == W*4 (LINEAR + lavapipe usually does).
        // Proper = vkGetImageSubresourceLayout for rowPitch. wall-N if not.
        try {
            using var fs = File.Create(path);
            var hdr = System.Text.Encoding.ASCII.GetBytes($"P6\n{W} {H}\n255\n");
            fs.Write(hdr);
            var src = _swapPtr[idx];
            var row = stackalloc byte[(int)(W * 3)];
            for(var y = 0u; y < H; y++) {
                var s = src + y * W * 4;
                for(var x = 0u; x < W; x++) {
                    row[x*3+0] = s[x*4+0];
                    row[x*3+1] = s[x*4+1];
                    row[x*3+2] = s[x*4+2];
                }
                fs.Write(new ReadOnlySpan<byte>(row, (int)(W*3)));
            }
            $"[vk] frame {frameN} → {path}".Log();
        } catch(Exception e) {
            $"[vk] DumpPpm failed: {e.Message}".Log();
        }
    }

    static int Chk(int r, string what) {
        if(r != 0) $"[vk] {what} → VkResult {r}".Log();
        return r;
    }

    // Called from nvnDeviceInitialize. Idempotent.
    public static bool Init() {
        if(Ready) return true;
        try {
            // Instance.
            var appName = stackalloc byte[16];
            "UmbraCore\0"u8.CopyTo(new Span<byte>(appName, 16));
            var ai = new VkApplicationInfo {
                sType = ST_APP_INFO, pAppName = appName,
                apiVersion = (1u << 22) | (3u << 12),  // VK_API_VERSION_1_3
            };
            var ici = new VkInstanceCreateInfo { sType = ST_INST_CI, pAppInfo = &ai };
            ulong inst;
            if(Chk(vkCreateInstance(&ici, null, &inst), "vkCreateInstance") != 0)
                return false;
            _inst = inst;

            // Physical device — pick lavapipe (deviceType=CPU=4, or name
            // contains "llvmpipe"). With VK_ICD_FILENAMES=lvp it's the only one.
            uint nDev = 0;
            vkEnumeratePhysicalDevices(_inst, &nDev, null);
            var devs = stackalloc ulong[(int) nDev];
            vkEnumeratePhysicalDevices(_inst, &nDev, devs);
            for(var i = 0; i < nDev; i++) {
                VkPhysDevProps p;
                vkGetPhysicalDeviceProperties(devs[i], &p);
                var name = Marshal.PtrToStringAnsi((nint) p.deviceName);
                $"[vk]   phys[{i}] type={p.deviceType} '{name}'".Log();
                if(_phys == 0 && (p.deviceType == 4 || (name?.Contains("llvmpipe") ?? false)))
                    _phys = devs[i];
            }
            if(_phys == 0) { _phys = devs[0]; "[vk] ‡ no CPU device; using phys[0]".Log(); }

            // Queue family with GRAPHICS bit.
            uint nQf = 0;
            vkGetPhysicalDeviceQueueFamilyProperties(_phys, &nQf, null);
            var qfs = stackalloc VkQueueFamilyProps[(int) nQf];
            vkGetPhysicalDeviceQueueFamilyProperties(_phys, &nQf, qfs);
            _qFam = 0;
            for(var i = 0u; i < nQf; i++)
                if((qfs[i].queueFlags & 1) != 0) { _qFam = i; break; }  // GRAPHICS_BIT

            // Device + queue.
            float prio = 1.0f;
            var qci = new VkDeviceQueueCreateInfo {
                sType = ST_DEV_QUEUE_CI, queueFamilyIndex = _qFam,
                queueCount = 1, pQueuePriorities = &prio,
            };
            var dci = new VkDeviceCreateInfo {
                sType = ST_DEV_CI, queueCreateInfoCount = 1, pQueueCreateInfos = &qci,
            };
            ulong dev;
            if(Chk(vkCreateDevice(_phys, &dci, null, &dev), "vkCreateDevice") != 0)
                return false;
            _dev = dev;
            ulong q; vkGetDeviceQueue(_dev, _qFam, 0, &q); _queue = q;

            // Command pool + one reusable buffer (RESET bit so we can re-record
            // per frame). + a fence for the v0 submit-and-wait.
            var cpci = new VkCommandPoolCreateInfo {
                sType = ST_CMDPOOL_CI, queueFamilyIndex = _qFam,
                flags = 0x2,  // RESET_COMMAND_BUFFER_BIT
            };
            ulong pool; Chk(vkCreateCommandPool(_dev, &cpci, null, &pool), "vkCreateCommandPool");
            _cmdPool = pool;
            var cbai = new VkCommandBufferAllocateInfo {
                sType = ST_CMDBUF_AI, commandPool = _cmdPool, level = 0, count = 1,
            };
            ulong cb; Chk(vkAllocateCommandBuffers(_dev, &cbai, &cb), "vkAllocateCommandBuffers");
            _cmdBuf = cb;
            var fci = new VkFenceCreateInfo { sType = ST_FENCE_CI };
            ulong f; Chk(vkCreateFence(_dev, &fci, null, &f), "vkCreateFence"); _fence = f;

            $"[vk] Init OK — instance={_inst:x} dev={_dev:x} queue={_queue:x} qFam={_qFam} cmdPool={_cmdPool:x}".Log();
            if(!InitImages()) { "[vk] InitImages failed; Present will be log-only".Log(); }
            else if(!InitPipeline()) { "[vk] InitPipeline failed; Present = clear-only".Log(); }
            Ready = true;
            return true;
        } catch(Exception e) {
            $"[vk] Init failed: {e}".Log();
            return false;
        }
    }

    // Called from nvnQueuePresentTexture. v0 = record an empty cmdbuf,
    // submit, wait on the fence. Proves the wiring round-trips through
    // lavapipe. v0.5 = vkCmdClearColorImage with the game's clear color
    // (recorded by NvnLinux at nvnCommandBufferClearColor) → readback.
    public static void Present(ulong window, int texIdx) {
        // (r) fps-cap: yield-spin until min frame interval. The
        // yield is the load-bearing part (= bg-load thread runs
        // while nnMain idles here, instead of nnMain spinning
        // Present at 1500fps and starving the loader). See the
        // _fpsCapUs comment for the a test run-vs-a test run mechanism.
        if(_fpsCapUs > 0) {
            // v1 spin+Yield was too expensive (a test run=7fps; ~3300
            // Yield()s/frame × 10+ runnable threads = 3300 ctx
            // switches). v2 = compute ms-to-sleep, Sleep once.
            var freq = System.Diagnostics.Stopwatch.Frequency;
            var now = System.Diagnostics.Stopwatch.GetTimestamp();
            var elapsedUs = (now - _lastPresentTick) * 1_000_000 / freq;
            var waitUs = _fpsCapUs - elapsedUs;
            if(waitUs > 0) Thread.Sleep((int)(waitUs / 1000));
            _lastPresentTick = System.Diagnostics.Stopwatch.GetTimestamp();
        }
        var n = ++_frameN;
        // Per-frame HID pump (writes Npad ring entries into the
        // shared-memory the game's nn::hid SDK reads). Driven by
        // UMBRA_HID env script. Tick before Ready-check so the
        // controller is connected from frame 1 (some games gate
        // on connected-controller before first draw). drawFrameN
        // = frames-since-first-draw (= "menu is up" clock; the
        // menu's appearance is wall-clock gated by async load,
        // so absolute frameN varies by host speed — drawFrameN
        // is the stable reference for the input script).
        if(_lastDrawN > 0) _drawFrameN++;
        HidPump.Tick(n, _drawFrameN);
        if(!Ready) {
            if(n <= 3) $"[vk] Present #{n} (not ready, log-only)".Log();
            return;
        }
        var bi = new VkCommandBufferBeginInfo {
            sType = ST_CMDBUF_BI, flags = 0x1,  // ONE_TIME_SUBMIT
        };
        Chk(vkBeginCommandBuffer(_cmdBuf, &bi), "vkBeginCommandBuffer");
        var idx = ((uint) texIdx) % 3;
        if(PipelineReady && _swapFb[idx] != 0) {
            // T2 v0: render pass + per-DrawRecord vkCmdDraw. Pull
            // accumulated draws from NvnLinux.Draws (snapshot under lock,
            // then clear so next frame starts fresh). Each draw's vbuf
            // memcpy'd into the host-mapped _vbuf at advancing offset.
            RecordDrawPass((int) idx);
        } else if(_swapImg[idx] != 0) {
            var c = NvnLinux.LastClearColor;
            TransitionAndClear((int) idx, c[0], c[1], c[2], c[3]);
        }
        Chk(vkEndCommandBuffer(_cmdBuf), "vkEndCommandBuffer");

        ulong cb = _cmdBuf, fence = _fence;
        vkResetFences(_dev, 1, &fence);
        var si = new VkSubmitInfo { sType = ST_SUBMIT, cmdBufCount = 1, pCmdBufs = &cb };
        var r = vkQueueSubmit(_queue, 1, &si, _fence);
        if(r == 0) {
            // (T6)×36 ×3 ROOT: was 1s. Pre-RTT (~140 draws
            // post-SKIP_VS to 1280×720) <<1s on lavapipe.
            // RTT = 669 draws incl 465→1024×4096 shadow +
            // 163→1920×1080 nC=3 G-buf ⟹ ~3-5s software-
            // raster. Wait timed out → DumpRtPpm read
            // staging while GPU hadn't reached the copy
            // (sentinel 0xAB survived = the disc); iter2's
            // vkBeginCommandBuffer hit a PENDING buffer
            // (VUID-00049) = the iter2-segv-flaky root.
            // own kt[26]: the "// 1s" comment was the
            // bound; RTT broke it. 60s + log VK_TIMEOUT.
            var wr = vkWaitForFences(_dev, 1, &fence, 1,
                60_000_000_000);
            if(wr != 0)
                $"[vk] ⚠ vkWaitForFences → {wr} (2=TIMEOUT) — GPU work >60s; readback STALE".Log();
            // Dump: first 2 frames (clear-only baseline) + first 3
            // frames-with-draws + any in UMBRA_DUMP_FRAMES env
            // (comma-sep frame numbers OR "every:N").
            _dumpFrames ??= ParseDumpFrames();
            var hasDraws = PipelineReady && _lastDrawN > 0;
            if(_swapPtr[idx] != null &&
               (n <= 2 || (hasDraws && _dumpedWithDraws++ < 3)
                       || _dumpFrames(n)
                       || (_dump3dWant != 0 && n >= _dump3dWant))) {
                DumpPpm((int) idx, n);
                _dump3dWant = 0;
            }
            if(n <= 5 || n % 30 == 0)
                $"[vk] Present #{n} tex={texIdx} → submit+wait OK".Log();
        } else {
            $"[vk] Present #{n} vkQueueSubmit → {r}".Log();
        }
    }
}
