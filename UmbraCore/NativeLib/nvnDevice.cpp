#include <iostream>
#include "nv.h"

// Device Builder functions
void nvnDeviceBuilderSetDefaults(NVNdeviceBuilder* builder) {
    std::cout << "nvnDeviceBuilderSetDefaults called!" << std::endl;
}

void nvnDeviceBuilderSetFlags(NVNdeviceBuilder* builder, int flags) {
    std::cout << "nvnDeviceBuilderSetFlags (" << flags << ") called!" << std::endl;
}

// Device functions
NVNboolean nvnDeviceInitialize(NVNdevice* device, const NVNdeviceBuilder* builder) {
    std::cout << "nvnDeviceInitialize called!" << std::endl;
    return 1;
}

void nvnDeviceFinalize(NVNdevice* device) {
    std::cout << "nvnDeviceFinalize called!" << std::endl;
}

void nvnDeviceSetDebugLabel(NVNdevice* device, const char* label) {
    std::cout << "nvnDeviceSetDebugLabel (" << label << ") called!" << std::endl;
}

inline std::string_view ToString(NVNdeviceInfo v) {
    switch (v) {
        case NVNdeviceInfo::ApiMajorVersion: return "ApiMajorVersion";
        case NVNdeviceInfo::ApiMinorVersion: return "ApiMinorVersion";
        case NVNdeviceInfo::UniformBufferBindingsPerStage: return "UniformBufferBindingsPerStage";
        case NVNdeviceInfo::MaxUniformBufferSize: return "MaxUniformBufferSize";
        case NVNdeviceInfo::UniformBufferAlignment: return "UniformBufferAlignment";
        case NVNdeviceInfo::ColorBufferBindings: return "ColorBufferBindings";
        case NVNdeviceInfo::VertexBufferBindings: return "VertexBufferBindings";
        case NVNdeviceInfo::TransformFeedbackBufferBindings: return "TransformFeedbackBufferBindings";
        case NVNdeviceInfo::ShaderStorageBufferBindingsPerStage: return "ShaderStorageBufferBindingsPerStage";
        case NVNdeviceInfo::TextureBindingsPerStage: return "TextureBindingsPerStage";
        case NVNdeviceInfo::CounterAlignment: return "CounterAlignment";
        case NVNdeviceInfo::TransformFeedbackBufferAlignment: return "TransformFeedbackBufferAlignment";
        case NVNdeviceInfo::TransformFeedbackControlAlignment: return "TransformFeedbackControlAlignment";
        case NVNdeviceInfo::IndirectDrawAlignment: return "IndirectDrawAlignment";
        case NVNdeviceInfo::VertexAttributes: return "VertexAttributes";
        case NVNdeviceInfo::TextureDescriptorSize: return "TextureDescriptorSize";
        case NVNdeviceInfo::SamplerDescriptorSize: return "SamplerDescriptorSize";
        case NVNdeviceInfo::ReservedTextureDescriptors: return "ReservedTextureDescriptors";
        case NVNdeviceInfo::ReservedSamplerDescriptors: return "ReservedSamplerDescriptors";
        case NVNdeviceInfo::CommandBufferCommandAlignment: return "CommandBufferCommandAlignment";
        case NVNdeviceInfo::CommandBufferControlAlignment: return "CommandBufferControlAlignment";
        case NVNdeviceInfo::CommandBufferMinCommandSize: return "CommandBufferMinCommandSize";
        case NVNdeviceInfo::CommandBufferMinControlSize: return "CommandBufferMinControlSize";
        case NVNdeviceInfo::ShaderScratchMemoryScaleFactorMinimum: return "ShaderScratchMemoryScaleFactorMinimum";
        case NVNdeviceInfo::ShaderScratchMemoryScaleFactorRecommended: return "ShaderScratchMemoryScaleFactorRecommended";
        case NVNdeviceInfo::ShaderScratchMemoryAlignment: return "ShaderScratchMemoryAlignment";
        case NVNdeviceInfo::ShaderScratchMemoryGranularity: return "ShaderScratchMemoryGranularity";
        case NVNdeviceInfo::MaxTextureAnisotropy: return "MaxTextureAnisotropy";
        case NVNdeviceInfo::MaxComputeWorkGroupSizeX: return "MaxComputeWorkGroupSizeX";
        case NVNdeviceInfo::MaxComputeWorkGroupSizeY: return "MaxComputeWorkGroupSizeY";
        case NVNdeviceInfo::MaxComputeWorkGroupSizeZ: return "MaxComputeWorkGroupSizeZ";
        case NVNdeviceInfo::MaxComputeWorkGroupSizeThreads: return "MaxComputeWorkGroupSizeThreads";
        case NVNdeviceInfo::MaxComputeDispatchWorkGroupsX: return "MaxComputeDispatchWorkGroupsX";
        case NVNdeviceInfo::MaxComputeDispatchWorkGroupsY: return "MaxComputeDispatchWorkGroupsY";
        case NVNdeviceInfo::MaxComputeDispatchWorkGroupsZ: return "MaxComputeDispatchWorkGroupsZ";
        case NVNdeviceInfo::ImageBindingsPerStage: return "ImageBindingsPerStage";
        case NVNdeviceInfo::MaxTexturePoolSize: return "MaxTexturePoolSize";
        case NVNdeviceInfo::MaxSamplerPoolSize: return "MaxSamplerPoolSize";
        case NVNdeviceInfo::MaxViewports: return "MaxViewports";
        case NVNdeviceInfo::MempoolTextureObjectPageAlignment: return "MempoolTextureObjectPageAlignment";
        case NVNdeviceInfo::SupportsMinMaxFiltering: return "SupportsMinMaxFiltering";
        case NVNdeviceInfo::SupportsStencil8Format: return "SupportsStencil8Format";
        case NVNdeviceInfo::SupportsAstcFormats: return "SupportsAstcFormats";
        case NVNdeviceInfo::L2Size: return "L2Size";
        case NVNdeviceInfo::MaxTextureLevels: return "MaxTextureLevels";
        case NVNdeviceInfo::MaxTextureLayers: return "MaxTextureLayers";
        case NVNdeviceInfo::GlslcMaxSupportedGpuCodeMajorVersion: return "GlslcMaxSupportedGpuCodeMajorVersion";
        case NVNdeviceInfo::GlslcMinSupportedGpuCodeMajorVersion: return "GlslcMinSupportedGpuCodeMajorVersion";
        case NVNdeviceInfo::GlslcMaxSupportedGpuCodeMinorVersion: return "GlslcMaxSupportedGpuCodeMinorVersion";
        case NVNdeviceInfo::GlslcMinSupportedGpuCodeMinorVersion: return "GlslcMinSupportedGpuCodeMinorVersion";
        case NVNdeviceInfo::SupportsConservativeRaster: return "SupportsConservativeRaster";
        case NVNdeviceInfo::SubpixelBits: return "SubpixelBits";
        case NVNdeviceInfo::MaxSubpixelBiasBits: return "MaxSubpixelBiasBits";
        case NVNdeviceInfo::IndirectDispatchAlignment: return "IndirectDispatchAlignment";
        case NVNdeviceInfo::ZcullSaveRestoreAlignment: return "ZcullSaveRestoreAlignment";
        case NVNdeviceInfo::ShaderScratchMemoryComputeScaleFactorMinimum: return "ShaderScratchMemoryComputeScaleFactorMinimum";
        case NVNdeviceInfo::LinearTextureStrideAlignment: return "LinearTextureStrideAlignment";
        case NVNdeviceInfo::LinearRenderTargetStrideAlignment: return "LinearRenderTargetStrideAlignment";
        case NVNdeviceInfo::MemoryPoolPageSize: return "MemoryPoolPageSize";
        case NVNdeviceInfo::SupportsZeroFromUnmappedVirtualPoolPages: return "SupportsZeroFromUnmappedVirtualPoolPages";
        case NVNdeviceInfo::UniformBufferUpdateAlignment: return "UniformBufferUpdateAlignment";
        case NVNdeviceInfo::MaxTextureSize: return "MaxTextureSize";
        case NVNdeviceInfo::MaxBufferTextureSize: return "MaxBufferTextureSize";
        case NVNdeviceInfo::Max3DTextureSize: return "Max3DTextureSize";
        case NVNdeviceInfo::MaxCubeMapTextureSize: return "MaxCubeMapTextureSize";
        case NVNdeviceInfo::MaxRectangleTextureSize: return "MaxRectangleTextureSize";
        case NVNdeviceInfo::SupportsPassthroughGeometryShaders: return "SupportsPassthroughGeometryShaders";
        case NVNdeviceInfo::SupportsViewportSwizzle: return "SupportsViewportSwizzle";
        case NVNdeviceInfo::SupportsSparseTiledPackagedTextures: return "SupportsSparseTiledPackagedTextures";
        case NVNdeviceInfo::SupportsAdvancedBlendModes: return "SupportsAdvancedBlendModes";
        case NVNdeviceInfo::MaxPresentInterval: return "MaxPresentInterval";
        case NVNdeviceInfo::SupportsDrawTexture: return "SupportsDrawTexture";
        case NVNdeviceInfo::SupportsTargetIndependentRasterization: return "SupportsTargetIndependentRasterization";
        case NVNdeviceInfo::SupportsFragmentCoverageToColor: return "SupportsFragmentCoverageToColor";
        case NVNdeviceInfo::SupportsPostDepthCoverage: return "SupportsPostDepthCoverage";
        case NVNdeviceInfo::SupportsImagesUsingTextureHandles: return "SupportsImagesUsingTextureHandles";
        case NVNdeviceInfo::SupportsSampleLocations: return "SupportsSampleLocations";
        case NVNdeviceInfo::MaxSampleLocationTableEntries: return "MaxSampleLocationTableEntries";
        case NVNdeviceInfo::ShaderCodeMemoryPoolPaddingSize: return "ShaderCodeMemoryPoolPaddingSize";
        case NVNdeviceInfo::MaxPatchSize: return "MaxPatchSize";
        case NVNdeviceInfo::QueueCommandMemoryGranularity: return "QueueCommandMemoryGranularity";
        case NVNdeviceInfo::QueueCommandMemoryMinSize: return "QueueCommandMemoryMinSize";
        case NVNdeviceInfo::QueueCommandMemoryDefaultSize: return "QueueCommandMemoryDefaultSize";
        case NVNdeviceInfo::QueueComputeMemoryGranularity: return "QueueComputeMemoryGranularity";
        case NVNdeviceInfo::QueueComputeMemoryMinSize: return "QueueComputeMemoryMinSize";
        case NVNdeviceInfo::QueueComputeMemoryDefaultSize: return "QueueComputeMemoryDefaultSize";
        case NVNdeviceInfo::QueueCommandMemoryMinFlushThreshold: return "QueueCommandMemoryMinFlushThreshold";
        case NVNdeviceInfo::SupportsFragmentShaderInterlock: return "SupportsFragmentShaderInterlock";
        case NVNdeviceInfo::MaxTexturesPerWindow: return "MaxTexturesPerWindow";
        case NVNdeviceInfo::MinTexturesPerWindow: return "MinTexturesPerWindow";
        case NVNdeviceInfo::SupportsDebugLayer: return "SupportsDebugLayer";
        case NVNdeviceInfo::QueueControlMemoryMinSize: return "QueueControlMemoryMinSize";
        case NVNdeviceInfo::QueueControlMemoryDefaultSize: return "QueueControlMemoryDefaultSize";
        case NVNdeviceInfo::QueueControlMemoryGranularity: return "QueueControlMemoryGranularity";
        case NVNdeviceInfo::SeparateTextureBindingsPerStage: return "SeparateTextureBindingsPerStage";
        case NVNdeviceInfo::SeparateSamplerBindingsPerStage: return "SeparateSamplerBindingsPerStage";
        case NVNdeviceInfo::DebugGroupsMaxDomainId: return "DebugGroupsMaxDomainId";
        case NVNdeviceInfo::EventsSupportReductionOperations: return "EventsSupportReductionOperations";
        default: return "Unknown";
    }
}

int getIntegerWrapper(const NVNdevice* device, NVNdeviceInfo pname) {
    switch(pname) {
        case NVNdeviceInfo::ApiMajorVersion:
            return 0x35; // SMO needs this version
        case NVNdeviceInfo::ApiMinorVersion:
            return 0xd; // SMO needs >= 0xD
        case NVNdeviceInfo::SupportsMinMaxFiltering:
        case NVNdeviceInfo::SupportsStencil8Format:
        case NVNdeviceInfo::SupportsAstcFormats:
        case NVNdeviceInfo::SupportsConservativeRaster:
        case NVNdeviceInfo::SupportsZeroFromUnmappedVirtualPoolPages:
        case NVNdeviceInfo::SupportsPassthroughGeometryShaders:
        case NVNdeviceInfo::SupportsViewportSwizzle:
        case NVNdeviceInfo::SupportsSparseTiledPackagedTextures:
        case NVNdeviceInfo::SupportsAdvancedBlendModes:
        case NVNdeviceInfo::SupportsDrawTexture:
        case NVNdeviceInfo::SupportsTargetIndependentRasterization:
        case NVNdeviceInfo::SupportsFragmentCoverageToColor:
        case NVNdeviceInfo::SupportsPostDepthCoverage:
        case NVNdeviceInfo::SupportsImagesUsingTextureHandles:
        case NVNdeviceInfo::SupportsSampleLocations:
        case NVNdeviceInfo::SupportsFragmentShaderInterlock:
        case NVNdeviceInfo::SupportsDebugLayer:
            return 1;
        case NVNdeviceInfo::ReservedSamplerDescriptors:
        case NVNdeviceInfo::ReservedTextureDescriptors:
            return 256;
        case NVNdeviceInfo::TextureDescriptorSize:
        case NVNdeviceInfo::SamplerDescriptorSize:
            return 32;
        case NVNdeviceInfo::QueueComputeMemoryDefaultSize:
            return 262144;
        case NVNdeviceInfo::QueueCommandMemoryDefaultSize:
            return 65536;
        case NVNdeviceInfo::QueueControlMemoryDefaultSize:
            return 16384;
        case NVNdeviceInfo::CommandBufferCommandAlignment:
            return 4;
        case NVNdeviceInfo::CommandBufferControlAlignment:
            return 8;
        case NVNdeviceInfo::ShaderScratchMemoryAlignment:
            return 16384;
        case NVNdeviceInfo::ShaderScratchMemoryGranularity:
            return 131072;
        case NVNdeviceInfo::ShaderScratchMemoryScaleFactorRecommended:
            return 256;
        case NVNdeviceInfo::LinearRenderTargetStrideAlignment:
            return 128;
        case NVNdeviceInfo::LinearTextureStrideAlignment:
            return 32;
        case NVNdeviceInfo::MaxSamplerPoolSize:
            return 4096;
        case NVNdeviceInfo::MaxTexturePoolSize:
            return 1048576;
        case NVNdeviceInfo::GlslcMinSupportedGpuCodeMajorVersion:
        case NVNdeviceInfo::GlslcMaxSupportedGpuCodeMajorVersion:
            return 1;
        case NVNdeviceInfo::GlslcMinSupportedGpuCodeMinorVersion:
            return 5;
        case NVNdeviceInfo::GlslcMaxSupportedGpuCodeMinorVersion:
            return 14;
        case NVNdeviceInfo::UniformBufferAlignment:
            return 256;
        default:
            __builtin_trap();
    }
}

void nvnDeviceGetInteger(const NVNdevice* device, NVNdeviceInfo pname, int* v) {
    std::cout << "nvnDeviceGetInteger(" << ToString(pname) << ") called!" << std::endl;
    if(!v) return;
    *v = getIntegerWrapper(device, pname);
}

uint64_t nvnDeviceGetCurrentTimestampInNanoseconds(const NVNdevice* device) {
    std::cout << "nvnDeviceGetCurrentTimestampInNanoseconds() called!" << std::endl;
    return 0;
}

void nvnDeviceSetIntermediateShaderCache(NVNdevice* device, int i) {
    std::cout << "nvnDeviceSetIntermediateShaderCache(i=" << i << ") called!" << std::endl;
}

NVNtextureHandle nvnDeviceGetTextureHandle(const NVNdevice* device, int textureID, int samplerID) {
    std::cout << "nvnDeviceGetTextureHandle(textureID=" << textureID << ", samplerID=" << samplerID << ") called!" << std::endl;
    return 0xDEADBEEFCAFEBA00;
}

NVNtextureHandle nvnDeviceGetTexelFetchHandle(const NVNdevice* device, int textureID) {
    std::cout << "nvnDeviceGetTexelFetchHandle(textureID=" << textureID << ") called!" << std::endl;
    return 0;
}

NVNimageHandle nvnDeviceGetImageHandle(const NVNdevice* device, int textureID) {
    std::cout << "nvnDeviceGetImageHandle(textureID=" << textureID << ") called!" << std::endl;
    return 0;
}

void nvnDeviceInstallDebugCallback(NVNdevice* device, const PFNNVNDEBUGCALLBACKPROC callback, void* callbackData, NVNboolean enable) {
    std::cout << "nvnDeviceInstallDebugCallback(callback=" << std::hex << reinterpret_cast<uint64_t>(callback)
              << ", callbackData=" << reinterpret_cast<uint64_t>(callbackData)
              << ", enable=" << std::dec << enable << ") called!" << std::endl;
}

NVNdebugDomainId nvnDeviceGenerateDebugDomainId(const NVNdevice* device, const char* s) {
    std::cout << "nvnDeviceGenerateDebugDomainId(s=\"" << (s ? s : "null") << "\") called!" << std::endl;
    return 0;
}

void nvnDeviceSetWindowOriginMode(NVNdevice* device, NVNwindowOriginMode windowOriginMode) {
    std::cout << "nvnDeviceSetWindowOriginMode(windowOriginMode=" << windowOriginMode << ") called!" << std::endl;
}

void nvnDeviceSetDepthMode(NVNdevice* device, NVNdepthMode depthMode) {
    std::cout << "nvnDeviceSetDepthMode(depthMode=" << depthMode << ") called!" << std::endl;
}

NVNboolean nvnDeviceRegisterFastClearColor(NVNdevice* device, const float* color, NVNformat format) {
    std::cout << "nvnDeviceRegisterFastClearColor(color=[" << (color ? std::to_string(color[0]) + "," + std::to_string(color[1]) + "," + std::to_string(color[2]) + "," + std::to_string(color[3]) : "null") << "], format=" << format << ") called!" << std::endl;
    return 1;
}

NVNboolean nvnDeviceRegisterFastClearColori(NVNdevice* device, const int* color, NVNformat format) {
    std::cout << "nvnDeviceRegisterFastClearColori(color=[" << (color ? std::to_string(color[0]) + "," + std::to_string(color[1]) + "," + std::to_string(color[2]) + "," + std::to_string(color[3]) : "null") << "], format=" << format << ") called!" << std::endl;
    return 1;
}

NVNboolean nvnDeviceRegisterFastClearColorui(NVNdevice* device, const uint32_t* color, NVNformat format) {
    std::cout << "nvnDeviceRegisterFastClearColorui(color=[" << (color ? std::to_string(color[0]) + "," + std::to_string(color[1]) + "," + std::to_string(color[2]) + "," + std::to_string(color[3]) : "null") << "], format=" << format << ") called!" << std::endl;
    return 1;
}

NVNboolean nvnDeviceRegisterFastClearDepth(NVNdevice* device, float f) {
    std::cout << "nvnDeviceRegisterFastClearDepth(f=" << f << ") called!" << std::endl;
    return 1;
}

NVNwindowOriginMode nvnDeviceGetWindowOriginMode(const NVNdevice* device) {
    std::cout << "nvnDeviceGetWindowOriginMode() called!" << std::endl;
    return 0;
}

NVNdepthMode nvnDeviceGetDepthMode(const NVNdevice* device) {
    std::cout << "nvnDeviceGetDepthMode() called!" << std::endl;
    return 0;
}

uint64_t nvnDeviceGetTimestampInNanoseconds(const NVNdevice* device, const NVNcounterData* counterData) {
    std::cout << "nvnDeviceGetTimestampInNanoseconds(counterData=" << std::hex << reinterpret_cast<uint64_t>(counterData) << std::dec << ") called!" << std::endl;
    return 0;
}

void nvnDeviceApplyDeferredFinalizes(NVNdevice* device, int i) {
    std::cout << "nvnDeviceApplyDeferredFinalizes(i=" << i << ") called!" << std::endl;
}

void nvnDeviceFinalizeCommandHandle(NVNdevice* device, NVNcommandHandle handles) {
    std::cout << "nvnDeviceFinalizeCommandHandle(handles=" << std::hex << handles << std::dec << ") called!" << std::endl;
}

void nvnDeviceWalkDebugDatabase(const NVNdevice* device, NVNdebugObjectType debugObjectType, PFNNVNWALKDEBUGDATABASECALLBACKPROC callback, void* callbackData) {
    std::cout << "nvnDeviceWalkDebugDatabase(debugObjectType=" << debugObjectType << ", callback=" << std::hex << reinterpret_cast<uint64_t>(callback) << std::dec << ") called!" << std::endl;
}

NVNseparateTextureHandle nvnDeviceGetSeparateTextureHandle(const NVNdevice* device, int textureID) {
    std::cout << "nvnDeviceGetSeparateTextureHandle(textureID=" << textureID << ") called!" << std::endl;
    return 0;
}

NVNseparateSamplerHandle nvnDeviceGetSeparateSamplerHandle(const NVNdevice* device, int textureID) {
    std::cout << "nvnDeviceGetSeparateSamplerHandle(textureID=" << textureID << ") called!" << std::endl;
    return 0;
}

NVNboolean nvnDeviceIsExternalDebuggerAttached(const NVNdevice* device) {
    std::cout << "nvnDeviceIsExternalDebuggerAttached() called!" << std::endl;
    return 0;
}

void nvnDeviceWaitForError(NVNdevice* device) {
    std::cout << "nvnDeviceWaitForError called!" << std::endl;
}