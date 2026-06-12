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
    // ── T3 T3 state ──
    static ulong _t3Pipeline, _t3PipelineLayout;
    static string _t3ShDir;
    static readonly Dictionary<(int vs, int fs), ulong> _t3Pipes = new();

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
    // layout = (k2)). Returns 0 if .spv missing.
    static ulong T3Pipe(int vsIdx, int fsIdx) {
        if(_t3Pipes.TryGetValue((vsIdx, fsIdx), out var p)) return p;
        var vsm = LoadShader($"{_t3ShDir}/sh{vsIdx:d4}-t1.bin.spv");
        var fsm = LoadShader($"{_t3ShDir}/sh{fsIdx:d4}-t2.bin.spv");
        if(vsm == 0 || fsm == 0) {
            if(!L.Quiet) $"[vk] T3Pipe({vsIdx},{fsIdx}): .spv missing → skip".Log();
            _t3Pipes[(vsIdx, fsIdx)] = 0;
            return 0;
        }
        var entryName = stackalloc byte[8]; "main\0"u8.CopyTo(new Span<byte>(entryName, 8));
        var stages = stackalloc VkPipelineShaderStageCreateInfo[2];
        stages[0] = new() { sType = ST_PIPELINE_SHADER_STAGE_CI, stage = 0x1, module = vsm, pName = entryName };
        stages[1] = new() { sType = ST_PIPELINE_SHADER_STAGE_CI, stage = 0x10, module = fsm, pName = entryName };
        // ‡ v1: hardcoded 36B 4-attr layout (= the menu's). Most
        // title-screen pairs use different stride → wrong vbuf
        // decode. Per-pair = (k2). The shaders themselves declare
        // their inputs at fixed Locations 0-3 so the FORMATS at
        // least match (SpirvEmit puts attr@N at Loc N consistently).
        var binding = new VkVertexInputBindingDescription { binding = 0, stride = 36, inputRate = 0 };
        var attrs = stackalloc VkVertexInputAttributeDescription[4];
        attrs[0] = new() { location = 0, binding = 0, format = 106, offset = 0 };
        attrs[1] = new() { location = 1, binding = 0, format = 37,  offset = 12 };
        attrs[2] = new() { location = 2, binding = 0, format = 37,  offset = 16 };
        attrs[3] = new() { location = 3, binding = 0, format = 103, offset = 20 };
        var vi = new VkPipelineVertexInputStateCreateInfo {
            sType = ST_PIPELINE_VERTEX_INPUT_CI,
            bindingCount = 1, pBindings = &binding,
            attributeCount = 4, pAttributes = attrs,
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
        var cbAtt = new VkPipelineColorBlendAttachmentState {
            blendEnable = 1, colorWriteMask = 0xf,
            srcColorBF = 6, dstColorBF = 7, colorBlendOp = 0,
            srcAlphaBF = 1, dstAlphaBF = 7, alphaBlendOp = 0,
        };
        var cbs = new VkPipelineColorBlendStateCreateInfo {
            sType = ST_PIPELINE_COLOR_BLEND_CI,
            attachmentCount = 1, pAttachments = &cbAtt,
        };
        var gpci = new VkGraphicsPipelineCreateInfo {
            sType = ST_GRAPHICS_PIPELINE_CI,
            stageCount = 2, pStages = stages,
            pVertexInputState = &vi, pInputAssemblyState = &ia,
            pViewportState = &vps, pRasterizationState = &rs,
            pMultisampleState = &ms, pColorBlendState = &cbs,
            layout = _t3PipelineLayout, renderPass = _renderPass,
            subpass = 0, basePipelineIndex = -1,
        };
        ulong pipe = 0;
        var rc = vkCreateGraphicsPipelines(_dev, 0, 1, &gpci, null, &pipe);
        if(rc != 0) {
            $"[vk] T3Pipe({vsIdx},{fsIdx}): vkCreateGraphicsPipelines={rc} → 0".Log();
            pipe = 0;
        } else {
            $"[vk] T3Pipe({vsIdx},{fsIdx}) built → 0x{pipe:x}".Log();
        }
        _t3Pipes[(vsIdx, fsIdx)] = pipe;
        return pipe;
    }
    static ulong _t3DslCbuf, _t3DslTex, _t3DsPool;
    static ulong _t3DsVs, _t3DsTex, _t3DsFs;   // sets 0/1/2
    static ulong[] _t3CbufBuf;        // [1..24] = stage*12 + binding
    static byte*[] _t3CbufPtr;        // mapped ptrs
    const int T3CbufStride = 4096, T3MaxDraws = 64;

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
        // Write set1: 1× COMBINED_IMAGE_SAMPLER, binding 8 → atlas.
        var imgInfo = new VkDescriptorImageInfo {
            sampler = _sampler, imageView = _atlasView,
            imageLayout = 1,  // GENERAL (= matches actual; the atlas
                              // is HOST_VISIBLE LINEAR, never
                              // transitioned past GENERAL.)
        };
        writes[24] = new() {
            sType = ST_WRITE_DESCRIPTOR_SET, dstSet = _t3DsTex,
            dstBinding = 8, descriptorCount = 1,
            descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
            pImageInfo = &imgInfo,
        };
        vkUpdateDescriptorSets(_dev, 25, writes, 0, null);
        $"[vk] EnsureT3Sets OK — vs={_t3DsVs:x} tex={_t3DsTex:x} fs={_t3DsFs:x}".Log();
    }
    static readonly ulong[] _swapView = new ulong[3];
    static readonly ulong[] _swapFb = new ulong[3];
    // One big HOST_VISIBLE vertex buffer; per-draw data memcpy'd in at
    // an advancing offset. ‡ v0 = 4MB fixed, no overflow check.
    static ulong _vbuf, _vbufMem; static byte* _vbufPtr;
    const ulong VbufSize = 4 << 20;
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
    static bool InitPipeline() {
        // ── render pass: one color attachment, LOAD_CLEAR → STORE ──
        var att = new VkAttachmentDescription {
            format = 37, samples = 1,           // R8G8B8A8_UNORM, 1×MSAA
            loadOp = 1, storeOp = 0,            // CLEAR, STORE
            stencilLoadOp = 2, stencilStoreOp = 1,  // DONT_CARE both
            initialLayout = 0, finalLayout = 1, // UNDEFINED → GENERAL
        };
        var colorRef = new VkAttachmentReference { attachment = 0, layout = 2 };  // COLOR_ATTACHMENT_OPTIMAL
        var subpass = new VkSubpassDescription {
            pipelineBindPoint = 0,              // GRAPHICS
            colorCount = 1, pColors = &colorRef,
        };
        var rpci = new VkRenderPassCreateInfo {
            sType = ST_RENDER_PASS_CI,
            attachmentCount = 1, pAttachments = &att,
            subpassCount = 1, pSubpasses = &subpass,
        };
        ulong rp;
        if(Chk(vkCreateRenderPass(_dev, &rpci, null, &rp), "vkCreateRenderPass") != 0)
            return false;
        _renderPass = rp;

        // ── image views + framebuffers over the 3 swap images ──
        for(var i = 0; i < 3; i++) {
            var ivci = new VkImageViewCreateInfo {
                sType = ST_IMAGE_VIEW_CI, image = _swapImg[i],
                viewType = 1, format = 37,      // 2D, R8G8B8A8_UNORM
                subresourceRange = new() { aspectMask = 1, levelCount = 1, layerCount = 1 },
            };
            ulong v; Chk(vkCreateImageView(_dev, &ivci, null, &v), $"vkCreateImageView[{i}]");
            _swapView[i] = v;
            var fbci = new VkFramebufferCreateInfo {
                sType = ST_FRAMEBUFFER_CI, renderPass = _renderPass,
                attachmentCount = 1, pAttachments = &v,
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
            polygonMode = 0, cullMode = 0, frontFace = 0,  // FILL, NONE, CCW
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
        var gpci = new VkGraphicsPipelineCreateInfo {
            sType = ST_GRAPHICS_PIPELINE_CI,
            stageCount = 2, pStages = stages,
            pVertexInputState = &vi, pInputAssemblyState = &ia,
            pViewportState = &vps, pRasterizationState = &rs,
            pMultisampleState = &ms, pColorBlendState = &cbs,
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

                // set=1: tex_8 combined sampler (sh0346 TidB=8). ‡ v0
                // = single binding=8; other tex handles → M2.5.
                var t3dsl1b = new VkDescriptorSetLayoutBinding {
                    binding = 8, descriptorType = DESC_TYPE_COMBINED_IMAGE_SAMPLER,
                    descriptorCount = 1, stageFlags = 0x10,
                };
                var t3dsl1ci = new VkDescriptorSetLayoutCreateInfo {
                    sType = ST_DESCRIPTOR_SET_LAYOUT_CI,
                    bindingCount = 1, pBindings = &t3dsl1b,
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

                // ── Descriptor pool: 24 UBO_DYNAMIC + 1 sampler ──
                var t3ps = stackalloc VkDescriptorPoolSize[2];
                t3ps[0] = new() { type = DESC_TYPE_UNIFORM_BUFFER_DYNAMIC, descriptorCount = 24 };
                t3ps[1] = new() { type = DESC_TYPE_COMBINED_IMAGE_SAMPLER, descriptorCount = 1 };
                var t3dpci = new VkDescriptorPoolCreateInfo {
                    sType = ST_DESCRIPTOR_POOL_CI, maxSets = 3,
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

        // Lazy atlas upload — must be BEFORE BeginRenderPass (the layout
        // transition barrier can't go inside a render pass).
        if(draws.Length > 0) EnsureAtlasBound();
        // T3 desc sets need atlas → also lazy.
        if(_t3Pipeline != 0 && _atlasReady) EnsureT3Sets();

        // Begin render pass — clears to game's clear color via loadOp.
        var c = NvnLinux.LastClearColor;
        var clear = new VkClearColorValue { r = c[0], g = c[1], b = c[2], a = c[3] };
        var rpbi = new VkRenderPassBeginInfo {
            sType = ST_RENDER_PASS_BI,
            renderPass = _renderPass, framebuffer = _swapFb[idx],
            renderArea = new() { width = W, height = H },
            clearValueCount = 1, pClearValues = &clear,
        };
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
            ulong vbo = 0;
            ulong vb = _vbuf;
            int recN = 0;
            foreach(var d in draws) {
                if(d.VbCpu == 0 || d.Count <= 0) continue;
                // v1.0: per-draw pipeline lookup (build-on-miss).
                // Returns 0 if .spv missing → skip this draw
                // (rather than render through wrong shaders).
                var pipe = T3Pipe(d.VsShIdx, d.FsShIdx);
                if(pipe == 0) continue;
                if(pipe != curPipe) {
                    vkCmdBindPipeline(_cmdBuf, 0, pipe);
                    curPipe = pipe;
                }
                var bytes = (ulong) d.Count * 36;
                if(vbo + bytes > VbufSize) break;
                Buffer.MemoryCopy((void*) d.VbCpu, _vbufPtr + vbo, bytes, bytes);
                ulong voff = vbo;
                vkCmdBindVertexBuffers(_cmdBuf, 0, 1, &vb, &voff);
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
                // Bind sets WITH this draw's dynamic offsets.
                for(var k = 0; k < 24; k++)
                    t3dyn[k] = (uint)(slot * T3CbufStride);
                vkCmdBindDescriptorSets(_cmdBuf, 0, _t3PipelineLayout,
                    0, 3, t3sets, 24, t3dyn);
                vkCmdDraw(_cmdBuf, (uint) d.Count, 1, (uint) d.First, 0);
                vbo += (bytes + 255) & ~255ul;
                recN++;
            }
            vkCmdEndRenderPass(_cmdBuf);
            _lastDrawN = draws.Length;
            if(_frameN <= 5 || (recN > 0 && _frameN % 10 == 0))
                {
                    var pairs = string.Join(" ", draws
                        .Select(d => (d.VsShIdx, d.FsShIdx, d.TexHandle))
                        .Distinct()
                        .Select(p => $"vs{p.VsShIdx}/fs{p.FsShIdx}/tx{p.TexHandle:x}"));
                    $"[vk] RecordDrawPass(T3) frame={_frameN} draws={recN} vbufUsed={vbo} [{pairs}]".Log();
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

    // Present-watchdog: polls every 1s; if _frameN hasn't moved
    // for >5s, dump CondVarKernel state. = the instrument
    // for the cv-race hang (A/B runs/A/B runs all-threads-futex_wait).
    static void Watchdog() {
        int last = -1, stuck = 0;
        while(true) {
            Thread.Sleep(1000);
            if(_frameN == last) {
                if(++stuck == 5 || (stuck > 5 && stuck % 30 == 0)) {
                    $"[wd] STALL: Present stuck {stuck}s @ frame {_frameN}".Log();
                    CondVarKernel.DumpState($"wd-{stuck}s");
                    foreach(var t in Kernel.ThreadManager.Threads)
                        $"[wd]   kthread h=0x{t.Handle:X} core={t.IdealCore}".Log();
                }
            } else { last = _frameN; stuck = 0; }
        }
    }
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

    static void DumpPpm(int idx, int frameN) {
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
            vkWaitForFences(_dev, 1, &fence, 1, 1_000_000_000);  // 1s
            // Dump: first 2 frames (clear-only baseline) + first 3
            // frames-with-draws + any in UMBRA_DUMP_FRAMES env
            // (comma-sep frame numbers OR "every:N").
            _dumpFrames ??= ParseDumpFrames();
            var hasDraws = PipelineReady && _lastDrawN > 0;
            if(_swapPtr[idx] != null &&
               (n <= 2 || (hasDraws && _dumpedWithDraws++ < 3)
                       || _dumpFrames(n)))
                DumpPpm((int) idx, n);
            if(n <= 5 || n % 30 == 0)
                $"[vk] Present #{n} tex={texIdx} → submit+wait OK".Log();
        } else {
            $"[vk] Present #{n} vkQueueSubmit → {r}".Log();
        }
    }
}
