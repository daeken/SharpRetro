#ifndef NATIVELIB_NVN_H
#define NATIVELIB_NVN_H
#include <cstddef>

#define WRAP_MAGIC 0xCAFEBABEDEADBEEF
#define WRAPPED_DECL(nvnName, realName) struct nvnName { uint64_t magic; realName* wrapped; }
#define UNWRAP(arg) \
    (arg)->magic != WRAP_MAGIC \
        ? ((arg)->magic = WRAP_MAGIC, \
        (arg)->wrapped = new std::remove_pointer_t<std::remove_reference_t<decltype((arg)->wrapped)>>) \
        : (arg)->wrapped \

class HIDeviceBuilder {
public:
};

WRAPPED_DECL(NVNdeviceBuilder, HIDeviceBuilder);

class HIDevice {
public:
};
WRAPPED_DECL(NVNdevice, HIDevice);

class HIQueue {
public:
};
WRAPPED_DECL(NVNqueue, HIQueue);

class HIQueueBuilder {
public:
};
WRAPPED_DECL(NVNqueueBuilder, HIQueueBuilder);

class HIWindow {
public:
};
WRAPPED_DECL(NVNwindow, HIWindow);

class HIWindowBuilder {
public:
};
WRAPPED_DECL(NVNwindowBuilder, HIWindowBuilder);

class HIProgram {
public:
};
WRAPPED_DECL(NVNprogram, HIProgram);

class HIMemoryPool {
public:
    void* pool;
    uint64_t size;
};
WRAPPED_DECL(NVNmemoryPool, HIMemoryPool);

class HIMemoryPoolBuilder {
public:
    void* pool;
    uint64_t size;
};
WRAPPED_DECL(NVNmemoryPoolBuilder, HIMemoryPoolBuilder);

class HITexturePool {
public:
};
WRAPPED_DECL(NVNtexturePool, HITexturePool);

class HISamplerPool {
public:
};
WRAPPED_DECL(NVNsamplerPool, HISamplerPool);

class HIBuffer {
public:
    NVNmemoryPool* pool;
    ptrdiff_t offset;
    uint64_t size;
};
WRAPPED_DECL(NVNbuffer, HIBuffer);

class HIBufferBuilder {
public:
    NVNmemoryPool* pool;
    ptrdiff_t offset;
    uint64_t size;
};
WRAPPED_DECL(NVNbufferBuilder, HIBufferBuilder);

class HITexture {
public:
};
WRAPPED_DECL(NVNtexture, HITexture);

class HITextureBuilder {
public:
    int width, height;
};
WRAPPED_DECL(NVNtextureBuilder, HITextureBuilder);

class HITextureView {
public:
};
WRAPPED_DECL(NVNtextureView, HITextureView);

class HISampler {
public:
};
WRAPPED_DECL(NVNsampler, HISampler);

class HISamplerBuilder {
public:
};
WRAPPED_DECL(NVNsamplerBuilder, HISamplerBuilder);

class HIBlendState {
public:
};
WRAPPED_DECL(NVNblendState, HIBlendState);

class HIColorState {
public:
};
WRAPPED_DECL(NVNcolorState, HIColorState);

class HIChannelMaskState {
public:
};
WRAPPED_DECL(NVNchannelMaskState, HIChannelMaskState);

class HIMultisampleState {
public:
};
WRAPPED_DECL(NVNmultisampleState, HIMultisampleState);

class HIPolygonState {
public:
};
WRAPPED_DECL(NVNpolygonState, HIPolygonState);

class HIDepthStencilState {
public:
};
WRAPPED_DECL(NVNdepthStencilState, HIDepthStencilState);

class HIVertexAttribState {
public:
};
WRAPPED_DECL(NVNvertexAttribState, HIVertexAttribState);

class HIVertexStreamState {
public:
};
WRAPPED_DECL(NVNvertexStreamState, HIVertexStreamState);

class HICommandBuffer {
public:
};
WRAPPED_DECL(NVNcommandBuffer, HICommandBuffer);

class HISync {
public:
};
WRAPPED_DECL(NVNsync, HISync);

class HIEvent {
public:
};
WRAPPED_DECL(NVNevent, HIEvent);

class HIEventBuilder {
public:
};
WRAPPED_DECL(NVNeventBuilder, HIEventBuilder);

class HIShaderData {
public:
};
WRAPPED_DECL(NVNshaderData, HIShaderData);

class HIMappingRequest {
public:
};
WRAPPED_DECL(NVNmappingRequest, HIMappingRequest);

class HICopyRegion {
public:
};
WRAPPED_DECL(NVNcopyRegion, HICopyRegion);

class HIPackagedTextureLayout {
public:
};
WRAPPED_DECL(NVNpackagedTextureLayout, HIPackagedTextureLayout);

class HITextureSparseTileLayout {
public:
};
WRAPPED_DECL(NVNtextureSparseTileLayout, HITextureSparseTileLayout);

class HIBufferRange {
public:
};
WRAPPED_DECL(NVNbufferRange, HIBufferRange);

class HIRectangle {
public:
};
WRAPPED_DECL(NVNrectangle, HIRectangle);

class HICounterData {
public:
};
WRAPPED_DECL(NVNcounterData, HICounterData);

class HIQueueErrorInfo {
public:
};
WRAPPED_DECL(NVNqueueErrorInfo, HIQueueErrorInfo);

class HISubroutineLinkageMapPtr {
public:
};
WRAPPED_DECL(NVNsubroutineLinkageMapPtr, HISubroutineLinkageMapPtr);

class HIDrawTextureRegion {
public:
};
WRAPPED_DECL(NVNdrawTextureRegion, HIDrawTextureRegion);

// Enum types
typedef int NVNboolean;
typedef int NVNwindowOriginMode;
typedef int NVNdepthMode;
typedef int NVNformat;
typedef int NVNdebugObjectType;
typedef int NVNqueueGetErrorResult;
typedef int NVNqueueAcquireTextureResult;
typedef int NVNwindowAcquireTextureResult;
typedef int NVNmemoryPoolFlags;
typedef int NVNstorageClass;
typedef int NVNtextureFlags;
typedef int NVNtextureTarget;
typedef int NVNtextureSwizzle;
typedef int NVNtextureDepthStencilMode;
typedef int NVNminFilter;
typedef int NVNmagFilter;
typedef int NVNwrapMode;
typedef int NVNcompareMode;
typedef int NVNcompareFunc;
typedef int NVNsamplerReduction;
typedef int NVNblendFunc;
typedef int NVNblendEquation;
typedef int NVNblendAdvancedMode;
typedef int NVNblendAdvancedOverlap;
typedef int NVNlogicOp;
typedef int NVNalphaFunc;
typedef int NVNcoverageModulationMode;
typedef int NVNface;
typedef int NVNfrontFace;
typedef int NVNpolygonMode;
typedef int NVNpolygonOffsetEnable;
typedef int NVNdepthFunc;
typedef int NVNstencilFunc;
typedef int NVNstencilOp;
typedef int NVNshaderStage;
typedef int NVNindexType;
typedef int NVNdrawPrimitive;
typedef int NVNtiledCacheAction;
typedef int NVNviewportSwizzle;
typedef int NVNconditionalRenderMode;
typedef int NVNsyncCondition;
typedef int NVNcounterType;
typedef int NVNsyncWaitResult;
typedef int NVNeventSignalMode;
typedef int NVNeventWaitMode;
typedef int NVNeventSignalLocation;
typedef int NVNdebugDomainId;

// Handle types
typedef uint64_t NVNtextureHandle;
typedef uint64_t NVNimageHandle;
typedef uint64_t NVNseparateTextureHandle;
typedef uint64_t NVNseparateSamplerHandle;
typedef uint64_t NVNcommandHandle;
typedef uint64_t NVNbufferAddress;
typedef uint64_t NVNtextureAddress;
typedef uint64_t NVNnativeWindow;

// Callback types
typedef void* PFNNVNGENERICFUNCPTRPROC;
typedef void (*PFNNVNDEBUGCALLBACKPROC)(void*, void*, void*, void*);
typedef void (*PFNNVNWALKDEBUGDATABASECALLBACKPROC)(void*, void*);
typedef void (*PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC)(void*, void*, void*);

enum class NVNshaderStageBits : int {
    VertexBit = 0x1,
    FragmentBit = 0x2,
    GeometryBit = 0x4,
    TessControlBit = 0x8,
    TessEvaluationBit = 0x10,
    ComputeBit = 0x20,
    AllGraphicsBits = 31,
};

enum class NVNdeviceInfo : int {
    ApiMajorVersion                                   = 0x0,
    ApiMinorVersion                                   = 0x1,
    UniformBufferBindingsPerStage                     = 0x2,
    MaxUniformBufferSize                              = 0x3,
    UniformBufferAlignment                            = 0x4,
    ColorBufferBindings                               = 0x5,
    VertexBufferBindings                              = 0x6,
    TransformFeedbackBufferBindings                   = 0x7,
    ShaderStorageBufferBindingsPerStage               = 0x8,
    TextureBindingsPerStage                           = 0x9,
    CounterAlignment                                  = 0xA,
    TransformFeedbackBufferAlignment                  = 0xB,
    TransformFeedbackControlAlignment                 = 0xC,
    IndirectDrawAlignment                             = 0xD,
    VertexAttributes                                  = 0xE,
    TextureDescriptorSize                             = 0xF,
    SamplerDescriptorSize                             = 0x10,
    ReservedTextureDescriptors                        = 0x11,
    ReservedSamplerDescriptors                        = 0x12,
    CommandBufferCommandAlignment                     = 0x13,
    CommandBufferControlAlignment                     = 0x14,
    CommandBufferMinCommandSize                       = 0x15,
    CommandBufferMinControlSize                       = 0x16,
    ShaderScratchMemoryScaleFactorMinimum             = 0x17,
    ShaderScratchMemoryScaleFactorRecommended         = 0x18,
    ShaderScratchMemoryAlignment                      = 0x19,
    ShaderScratchMemoryGranularity                    = 0x1A,
    MaxTextureAnisotropy                              = 0x1B,
    MaxComputeWorkGroupSizeX                          = 0x1C,
    MaxComputeWorkGroupSizeY                          = 0x1D,
    MaxComputeWorkGroupSizeZ                          = 0x1E,
    MaxComputeWorkGroupSizeThreads                    = 0x1F,
    MaxComputeDispatchWorkGroupsX                     = 0x20,
    MaxComputeDispatchWorkGroupsY                     = 0x21,
    MaxComputeDispatchWorkGroupsZ                     = 0x22,
    ImageBindingsPerStage                             = 0x23,
    MaxTexturePoolSize                                = 0x24,
    MaxSamplerPoolSize                                = 0x25,
    MaxViewports                                      = 0x26,
    MempoolTextureObjectPageAlignment                 = 0x27,
    SupportsMinMaxFiltering                           = 0x28,
    SupportsStencil8Format                            = 0x29,
    SupportsAstcFormats                               = 0x2A,
    L2Size                                            = 0x2B,
    MaxTextureLevels                                  = 0x2C,
    MaxTextureLayers                                  = 0x2D,
    GlslcMaxSupportedGpuCodeMajorVersion              = 0x2E,
    GlslcMinSupportedGpuCodeMajorVersion              = 0x2F,
    GlslcMaxSupportedGpuCodeMinorVersion              = 0x30,
    GlslcMinSupportedGpuCodeMinorVersion              = 0x31,
    SupportsConservativeRaster                        = 0x32,
    SubpixelBits                                      = 0x33,
    MaxSubpixelBiasBits                               = 0x34,
    IndirectDispatchAlignment                         = 0x35,
    ZcullSaveRestoreAlignment                         = 0x36,
    ShaderScratchMemoryComputeScaleFactorMinimum      = 0x37,
    LinearTextureStrideAlignment                      = 0x38,
    LinearRenderTargetStrideAlignment                 = 0x39,
    MemoryPoolPageSize                                = 0x3A,
    SupportsZeroFromUnmappedVirtualPoolPages          = 0x3B,
    UniformBufferUpdateAlignment                      = 0x3C,
    MaxTextureSize                                    = 0x3D,
    MaxBufferTextureSize                              = 0x3E,
    Max3DTextureSize                                  = 0x3F,
    MaxCubeMapTextureSize                             = 0x40,
    MaxRectangleTextureSize                           = 0x41,
    SupportsPassthroughGeometryShaders                = 0x42,
    SupportsViewportSwizzle                           = 0x43,
    SupportsSparseTiledPackagedTextures               = 0x44,
    SupportsAdvancedBlendModes                        = 0x45,
    MaxPresentInterval                                = 0x46,
    SupportsDrawTexture                               = 0x47,
    SupportsTargetIndependentRasterization            = 0x48,
    SupportsFragmentCoverageToColor                   = 0x49,
    SupportsPostDepthCoverage                         = 0x4A,
    SupportsImagesUsingTextureHandles                 = 0x4B,
    SupportsSampleLocations                           = 0x4C,
    MaxSampleLocationTableEntries                     = 0x4D,
    ShaderCodeMemoryPoolPaddingSize                   = 0x4E,
    MaxPatchSize                                      = 0x4F,
    QueueCommandMemoryGranularity                     = 0x50,
    QueueCommandMemoryMinSize                         = 0x51,
    QueueCommandMemoryDefaultSize                     = 0x52,
    QueueComputeMemoryGranularity                     = 0x53,
    QueueComputeMemoryMinSize                         = 0x54,
    QueueComputeMemoryDefaultSize                     = 0x55,
    QueueCommandMemoryMinFlushThreshold               = 0x56,
    SupportsFragmentShaderInterlock                   = 0x57,
    MaxTexturesPerWindow                              = 0x58,
    MinTexturesPerWindow                              = 0x59,
    SupportsDebugLayer                                = 0x5A,
    QueueControlMemoryMinSize                         = 0x5B,
    QueueControlMemoryDefaultSize                     = 0x5C,
    QueueControlMemoryGranularity                     = 0x5D,
    SeparateTextureBindingsPerStage                   = 0x5E,
    SeparateSamplerBindingsPerStage                   = 0x5F,
    DebugGroupsMaxDomainId                            = 0x60,
    EventsSupportReductionOperations                  = 0x61,
};

void nvnDeviceBuilderSetDefaults(NVNdeviceBuilder* builder);
int nvnDeviceBuilderGetFlags(NVNdeviceBuilder* builder);
void nvnDeviceBuilderSetFlags(NVNdeviceBuilder* builder, int flags);
NVNboolean nvnDeviceInitialize(NVNdevice* device, NVNdeviceBuilder* builder);
void nvnDeviceFinalize(NVNdevice* device);
void nvnDeviceSetDebugLabel(NVNdevice* device, const char* label);
PFNNVNGENERICFUNCPTRPROC nvnDeviceGetProcAddress(NVNdevice* device, const char* s);
void nvnDeviceGetInteger(NVNdevice* device, NVNdeviceInfo pname, int* v);
uint64_t nvnDeviceGetCurrentTimestampInNanoseconds(NVNdevice* device);
void nvnDeviceSetIntermediateShaderCache(NVNdevice* device, int i);
NVNtextureHandle nvnDeviceGetTextureHandle(NVNdevice* device, int textureID, int samplerID);
NVNtextureHandle nvnDeviceGetTexelFetchHandle(NVNdevice* device, int textureID);
NVNimageHandle nvnDeviceGetImageHandle(NVNdevice* device, int textureID);
void nvnDeviceInstallDebugCallback(NVNdevice* device, const PFNNVNDEBUGCALLBACKPROC callback, void* callbackData, NVNboolean enable);
NVNdebugDomainId nvnDeviceGenerateDebugDomainId(NVNdevice* device, const char* s);
void nvnDeviceSetWindowOriginMode(NVNdevice* device, NVNwindowOriginMode windowOriginMode);
void nvnDeviceSetDepthMode(NVNdevice* device, NVNdepthMode depthMode);
NVNboolean nvnDeviceRegisterFastClearColor(NVNdevice* device, const float* color, NVNformat format);
NVNboolean nvnDeviceRegisterFastClearColori(NVNdevice* device, const int* color, NVNformat format);
NVNboolean nvnDeviceRegisterFastClearColorui(NVNdevice* device, const uint32_t* color, NVNformat format);
NVNboolean nvnDeviceRegisterFastClearDepth(NVNdevice* device, float f);
NVNwindowOriginMode nvnDeviceGetWindowOriginMode(NVNdevice* device);
NVNdepthMode nvnDeviceGetDepthMode(NVNdevice* device);
uint64_t nvnDeviceGetTimestampInNanoseconds(NVNdevice* device, NVNcounterData* counterData);
void nvnDeviceApplyDeferredFinalizes(NVNdevice* device, int i);
void nvnDeviceFinalizeCommandHandle(NVNdevice* device, NVNcommandHandle handles);
void nvnDeviceWalkDebugDatabase(NVNdevice* device, NVNdebugObjectType debugObjectType, PFNNVNWALKDEBUGDATABASECALLBACKPROC callback, void* callbackData);
NVNseparateTextureHandle nvnDeviceGetSeparateTextureHandle(NVNdevice* device, int textureID);
NVNseparateSamplerHandle nvnDeviceGetSeparateSamplerHandle(NVNdevice* device, int textureID);
NVNboolean nvnDeviceIsExternalDebuggerAttached(NVNdevice* device);
void nvnDeviceWaitForError(NVNdevice* device);
NVNqueueGetErrorResult nvnQueueGetError(NVNqueue* queue, NVNqueueErrorInfo* info);
size_t nvnQueueGetTotalCommandMemoryUsed(NVNqueue* queue);
size_t nvnQueueGetTotalControlMemoryUsed(NVNqueue* queue);
size_t nvnQueueGetTotalComputeMemoryUsed(NVNqueue* queue);
void nvnQueueResetMemoryUsageCounts(NVNqueue* queue);
void* nvnQueueBuilderGetMemory(NVNqueueBuilder* builder); // guess
NVNdevice* nvnQueueBuilderGetDevice(NVNqueueBuilder* builder);
void nvnQueueBuilderSetDevice(NVNqueueBuilder* builder, NVNdevice* device);
void nvnQueueBuilderSetDefaults(NVNqueueBuilder* builder);
int nvnQueueBuilderGetFlags(NVNqueueBuilder* builder);
void nvnQueueBuilderSetFlags(NVNqueueBuilder* builder, int flags);
size_t nvnQueueBuilderGetCommandMemorySize(NVNqueueBuilder* builder);
void nvnQueueBuilderSetCommandMemorySize(NVNqueueBuilder* builder, size_t size);
size_t nvnQueueBuilderGetComputeMemorySize(NVNqueueBuilder* builder);
void nvnQueueBuilderSetComputeMemorySize(NVNqueueBuilder* builder, size_t size);
size_t nvnQueueBuilderGetControlMemorySize(NVNqueueBuilder* builder);
void nvnQueueBuilderSetControlMemorySize(NVNqueueBuilder* builder, size_t size);
size_t nvnQueueBuilderGetQueueMemorySize(NVNqueueBuilder* builder);
void nvnQueueBuilderSetQueueMemory(NVNqueueBuilder* builder, void* memory, size_t size);
size_t nvnQueueBuilderGetMemorySize(NVNqueueBuilder* builder);

// guess
int nvnQueueBuilderGetQueuePriority(NVNqueueBuilder* builder);
void nvnQueueBuilderSetQueuePriority(NVNqueueBuilder* builder, int priority);

size_t nvnQueueBuilderGetCommandFlushThreshold(NVNqueueBuilder* builder);
void nvnQueueBuilderSetCommandFlushThreshold(NVNqueueBuilder* builder, size_t size);
NVNboolean nvnQueueInitialize(NVNqueue* queue, NVNqueueBuilder* builder);
void nvnQueueFinalize(NVNqueue* queue);
void nvnQueueSetDebugLabel(NVNqueue* queue, const char* label);
void nvnQueueSubmitCommands(NVNqueue* queue, int numCommands, const NVNcommandHandle* handles);
void nvnQueueFlush(NVNqueue* queue);
void nvnQueueFinish(NVNqueue* queue);
void nvnQueuePresentTexture(NVNqueue* queue, NVNwindow* window, int textureIndex);
NVNqueueAcquireTextureResult nvnQueueAcquireTexture(NVNqueue* queue, NVNwindow* window, int* textureIndex);
NVNdevice* nvnWindowBuilderGetDevice(NVNwindowBuilder* builder);
void nvnWindowBuilderSetDevice(NVNwindowBuilder* builder, NVNdevice* device);
void nvnWindowBuilderSetDefaults(NVNwindowBuilder* builder);
void nvnWindowBuilderSetNativeWindow(NVNwindowBuilder* builder, NVNnativeWindow nativeWindow);
int nvnWindowBuilderGetNumTextures(NVNwindowBuilder* builder);
NVNtexture* nvnWindowBuilderGetTexture(NVNwindowBuilder* builder, int texture);
void nvnWindowBuilderSetTextures(NVNwindowBuilder* builder, int numTextures, NVNtexture* const* textures);
int nvnWindowBuilderGetNumActiveTextures(NVNwindowBuilder* builder); // guess
void nvnWindowBuilderSetNumActiveTextures(NVNwindowBuilder* builder, int numActiveTextures); // guess
void nvnWindowBuilderSetPresentInterval(NVNwindowBuilder* builder, int presentInterval);
NVNnativeWindow nvnWindowBuilderGetNativeWindow(NVNwindowBuilder* builder);
int nvnWindowBuilderGetPresentInterval(NVNwindowBuilder* builder);
NVNboolean nvnWindowInitialize(NVNwindow* window, NVNwindowBuilder* builder);
void nvnWindowFinalize(NVNwindow* window);
void nvnWindowSetDebugLabel(NVNwindow* window, const char* label);
NVNwindowAcquireTextureResult nvnWindowAcquireTexture(NVNwindow* window, NVNsync* textureAvailableSync, int* textureIndex);
NVNnativeWindow nvnWindowGetNativeWindow(NVNwindow* window);
int nvnWindowGetPresentInterval(NVNwindow* window);
void nvnWindowSetPresentInterval(NVNwindow* window, int presentInterval);
void nvnWindowSetCrop(NVNwindow* window, int x, int y, int w, int h);
void nvnWindowGetCrop(NVNwindow* window, NVNrectangle* rectangle);
int nvnWindowGetNumTextures(NVNwindow* builder);
int nvnWindowGetNumActiveTextures(NVNwindow* builder); // guess
void nvnWindowSetNumActiveTextures(NVNwindow* builder, int numActiveTextures); // guess
NVNboolean nvnProgramInitialize(NVNprogram* program, NVNdevice* device);
void nvnProgramFinalize(NVNprogram* program);
void nvnProgramSetDebugLabel(NVNprogram* program, const char* label);
void nvnProgramSetSampleShading(NVNprogram* program); // absolutely no idea
NVNboolean nvnProgramSetShaders(NVNprogram* program, int count, NVNshaderData* stageData);
NVNboolean nvnProgramSetShadersExt(NVNprogram* program, int count, NVNshaderData* stageData); // guess -- why is this different
NVNdevice* nvnMemoryPoolBuilderGetDevice(NVNmemoryPoolBuilder* builder);
void nvnMemoryPoolBuilderSetDevice(NVNmemoryPoolBuilder* builder, NVNdevice* device);
void nvnMemoryPoolBuilderSetDefaults(NVNmemoryPoolBuilder* builder);
void nvnMemoryPoolBuilderSetStorage(NVNmemoryPoolBuilder* builder, void* memory, size_t size);
void nvnMemoryPoolBuilderSetFlags(NVNmemoryPoolBuilder* builder, int flags);
void nvnMemoryPoolBuilderGetMemory(NVNmemoryPoolBuilder* builder);
size_t nvnMemoryPoolBuilderGetSize(NVNmemoryPoolBuilder* builder);
NVNmemoryPoolFlags nvnMemoryPoolBuilderGetFlags(NVNmemoryPoolBuilder* builder);
NVNboolean nvnMemoryPoolInitialize(NVNmemoryPool* pool, NVNmemoryPoolBuilder* builder);
void nvnMemoryPoolSetDebugLabel(NVNmemoryPool* pool, const char* label);
void nvnMemoryPoolFinalize(NVNmemoryPool* pool);
void* nvnMemoryPoolMap(NVNmemoryPool* pool);
void nvnMemoryPoolFlushMappedRange(NVNmemoryPool* pool, ptrdiff_t offset, size_t size);
void nvnMemoryPoolInvalidateMappedRange(NVNmemoryPool* pool, ptrdiff_t offset, size_t size);
NVNbufferAddress nvnMemoryPoolGetBufferAddress(NVNmemoryPool* pool);
NVNboolean nvnMemoryPoolMapVirtual(NVNmemoryPool* pool, int numRequests, NVNmappingRequest* requests);
size_t nvnMemoryPoolGetSize(NVNmemoryPool* pool);
NVNmemoryPoolFlags nvnMemoryPoolGetFlags(NVNmemoryPool* pool);
NVNboolean nvnTexturePoolInitialize(NVNtexturePool* texturePool, NVNmemoryPool* memoryPool, ptrdiff_t offset, int numDescriptors);
void nvnTexturePoolSetDebugLabel(NVNtexturePool* pool, const char* label);
void nvnTexturePoolFinalize(NVNtexturePool* pool);
void nvnTexturePoolRegisterTexture(NVNtexturePool* pool, int id, NVNtexture* texture, NVNtextureView* view);
void nvnTexturePoolRegisterImage(NVNtexturePool* pool, int id, NVNtexture* texture, NVNtextureView* view);
NVNmemoryPool* nvnTexturePoolGetMemoryPool(NVNtexturePool* pool);
ptrdiff_t nvnTexturePoolGetMemoryOffset(NVNtexturePool* pool);
int nvnTexturePoolGetSize(NVNtexturePool* pool);
NVNboolean nvnSamplerPoolInitialize(NVNsamplerPool* samplerPool, NVNmemoryPool* memoryPool, ptrdiff_t offset, int numDescriptors);
void nvnSamplerPoolSetDebugLabel(NVNsamplerPool* pool, const char* label);
void nvnSamplerPoolFinalize(NVNsamplerPool* pool);
void nvnSamplerPoolRegisterSampler(NVNsamplerPool* pool, int id, NVNsampler* sampler);
void nvnSamplerPoolRegisterSamplerBuilder(NVNsamplerPool* pool, int id, NVNsamplerBuilder* builder);
NVNmemoryPool* nvnSamplerPoolGetMemoryPool(NVNsamplerPool* pool);
ptrdiff_t nvnSamplerPoolGetMemoryOffset(NVNsamplerPool* pool);
int nvnSamplerPoolGetSize(NVNsamplerPool* pool);
NVNdevice* nvnBufferBuilderGetDevice(NVNbufferBuilder* builder);
void nvnBufferBuilderSetDevice(NVNbufferBuilder* builder, NVNdevice* device);
void nvnBufferBuilderSetDefaults(NVNbufferBuilder* builder);
void nvnBufferBuilderSetStorage(NVNbufferBuilder* builder, NVNmemoryPool* pool, ptrdiff_t offset, size_t size);
NVNmemoryPool nvnBufferBuilderGetMemoryPool(NVNbufferBuilder* builder);
ptrdiff_t nvnBufferBuilderGetMemoryOffset(NVNbufferBuilder* builder);
size_t nvnBufferBuilderGetSize(NVNbufferBuilder* builder);
NVNboolean nvnBufferInitialize(NVNbuffer* buffer, NVNbufferBuilder* builder);
void nvnBufferSetDebugLabel(NVNbuffer* buffer, const char* label);
void nvnBufferFinalize(NVNbuffer* buffer);
void* nvnBufferMap(NVNbuffer* buffer);
NVNbufferAddress nvnBufferGetAddress(NVNbuffer* buffer);
void nvnBufferFlushMappedRange(NVNbuffer* buffer, ptrdiff_t offset, size_t size);
void nvnBufferInvalidateMappedRange(NVNbuffer* buffer, ptrdiff_t offset, size_t size);
NVNmemoryPool* nvnBufferGetMemoryPool(NVNbuffer* buffer);
ptrdiff_t nvnBufferGetMemoryOffset(NVNbuffer* buffer);
size_t nvnBufferGetSize(NVNbuffer* buffer);
uint64_t nvnBufferGetDebugID(NVNbuffer* buffer);
NVNdevice* nvnTextureBuilderGetDevice(NVNtextureBuilder* builder);
void nvnTextureBuilderSetDevice(NVNtextureBuilder* builder, NVNdevice* device);
void nvnTextureBuilderSetDefaults(NVNtextureBuilder* builder);
void nvnTextureBuilderSetFlags(NVNtextureBuilder* builder, int flags);
void nvnTextureBuilderSetTarget(NVNtextureBuilder* builder, NVNtextureTarget target);
void nvnTextureBuilderSetWidth(NVNtextureBuilder* builder, int width);
void nvnTextureBuilderSetHeight(NVNtextureBuilder* builder, int height);
void nvnTextureBuilderSetDepth(NVNtextureBuilder* builder, int depth);
void nvnTextureBuilderSetSize1D(NVNtextureBuilder* builder, int size);
void nvnTextureBuilderSetSize2D(NVNtextureBuilder* builder, int width, int height);
void nvnTextureBuilderSetSize3D(NVNtextureBuilder* builder, int width, int height, int depth);
void nvnTextureBuilderSetLevels(NVNtextureBuilder* builder, int numLevels);
void nvnTextureBuilderSetFormat(NVNtextureBuilder* builder, NVNformat format);
void nvnTextureBuilderSetSamples(NVNtextureBuilder* builder, int samples);
void nvnTextureBuilderSetSwizzle(NVNtextureBuilder* builder, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a);
void nvnTextureBuilderSetDepthStencilMode(NVNtextureBuilder* builder, NVNtextureDepthStencilMode mode);
size_t nvnTextureBuilderGetStorageSize(NVNtextureBuilder* builder);
size_t nvnTextureBuilderGetStorageAlignment(NVNtextureBuilder* builder);
void nvnTextureBuilderSetStorage(NVNtextureBuilder* builder, NVNmemoryPool* pool, ptrdiff_t offset);
void nvnTextureBuilderSetPackagedTextureData(NVNtextureBuilder* builder, const void* data);
void nvnTextureBuilderSetPackagedTextureLayout(NVNtextureBuilder* builder, NVNpackagedTextureLayout* layout);
void nvnTextureBuilderSetStride(NVNtextureBuilder* builder, ptrdiff_t stride);
void nvnTextureBuilderSetGLTextureName(NVNtextureBuilder* builder, uint32_t name);
NVNstorageClass nvnTextureBuilderGetStorageClass(NVNtextureBuilder* builder);
NVNtextureFlags nvnTextureBuilderGetFlags(NVNtextureBuilder* builder);
NVNtextureTarget nvnTextureBuilderGetTarget(NVNtextureBuilder* builder);
int nvnTextureBuilderGetWidth(NVNtextureBuilder* builder);
int nvnTextureBuilderGetHeight(NVNtextureBuilder* builder);
int nvnTextureBuilderGetDepth(NVNtextureBuilder* builder);
int nvnTextureBuilderGetLevels(NVNtextureBuilder* builder);
NVNformat nvnTextureBuilderGetFormat(NVNtextureBuilder* builder);
int nvnTextureBuilderGetSamples(NVNtextureBuilder* builder);
void nvnTextureBuilderGetSwizzle(NVNtextureBuilder* builder, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a);
NVNtextureDepthStencilMode nvnTextureBuilderGetDepthStencilMode(NVNtextureBuilder* builder);
const void* nvnTextureBuilderGetPackagedTextureData(NVNtextureBuilder* builder);
const void* nvnTextureBuilderGetPackagedTextureLayout(NVNtextureBuilder* builder); // Literally no idea
ptrdiff_t nvnTextureBuilderGetStride(NVNtextureBuilder* builder);
void nvnTextureBuilderGetSparseTileLayout(NVNtextureBuilder* builder, NVNtextureSparseTileLayout* layout);
uint32_t nvnTextureBuilderGetGLTextureName(NVNtextureBuilder* builder);
size_t nvnTextureBuilderGetZCullStorageSize(NVNtextureBuilder* builder);
NVNmemoryPool nvnTextureBuilderGetMemoryPool(NVNtextureBuilder* builder);
ptrdiff_t nvnTextureBuilderGetMemoryOffset(NVNtextureBuilder* builder);
int nvnTextureBuilderGetRawStorageClass(NVNtextureBuilder* builder); // guess
void nvnTextureViewSetDefaults(NVNtextureView* view);
void nvnTextureViewSetLevels(NVNtextureView* view, int baseLevel, int numLevels);
void nvnTextureViewSetLayers(NVNtextureView* view, int minLayer, int numLayers);
void nvnTextureViewSetFormat(NVNtextureView* view, NVNformat format);
void nvnTextureViewSetSwizzle(NVNtextureView* view, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a);
void nvnTextureViewSetDepthStencilMode(NVNtextureView* view, NVNtextureDepthStencilMode mode);
void nvnTextureViewSetTarget(NVNtextureView* view, NVNtextureTarget target);
NVNboolean nvnTextureViewGetLevels(NVNtextureView* view, int* baseLevel, int* numLevels);
NVNboolean nvnTextureViewGetLayers(NVNtextureView* view, int* minLayer, int* numLayers);
NVNboolean nvnTextureViewGetFormat(NVNtextureView* view, NVNformat* format);
NVNboolean nvnTextureViewGetSwizzle(NVNtextureView* view, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a);
NVNboolean nvnTextureViewGetDepthStencilMode(NVNtextureView* view, NVNtextureDepthStencilMode* mode);
NVNboolean nvnTextureViewGetTarget(NVNtextureView* view, NVNtextureTarget* target);
NVNboolean nvnTextureViewCompare(NVNtextureView* view1, NVNtextureView* view2);
NVNboolean nvnTextureInitialize(NVNtexture* texture, NVNtextureBuilder* builder);
size_t nvnTextureGetZCullStorageSize(NVNtexture* texture);
void nvnTextureFinalize(NVNtexture* texture);
void nvnTextureSetDebugLabel(NVNtexture* texture, const char* label);
NVNstorageClass nvnTextureGetStorageClass(NVNtexture* texture);
ptrdiff_t nvnTextureGetViewOffset(NVNtexture* texture, NVNtextureView* view);
NVNtextureFlags nvnTextureGetFlags(NVNtexture* texture);
NVNtextureTarget nvnTextureGetTarget(NVNtexture* texture);
int nvnTextureGetWidth(NVNtexture* texture);
int nvnTextureGetHeight(NVNtexture* texture);
int nvnTextureGetDepth(NVNtexture* texture);
int nvnTextureGetLevels(NVNtexture* texture);
NVNformat nvnTextureGetFormat(NVNtexture* texture);
int nvnTextureGetSamples(NVNtexture* texture);
void nvnTextureGetSwizzle(NVNtexture* texture, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a);
NVNtextureDepthStencilMode nvnTextureGetDepthStencilMode(NVNtexture* texture);
ptrdiff_t nvnTextureGetStride(NVNtexture* texture);
NVNtextureAddress nvnTextureGetTextureAddress(NVNtexture* texture);
void nvnTextureGetSparseTileLayout(NVNtexture* texture, NVNtextureSparseTileLayout* layout);
void nvnTextureWriteTexels(NVNtexture* texture, NVNtextureView* view, NVNcopyRegion* region, const void* p);
void nvnTextureWriteTexelsStrided(NVNtexture* texture, NVNtextureView* view, NVNcopyRegion* region, const void* p, ptrdiff_t o1, ptrdiff_t o2);
void nvnTextureReadTexels(NVNtexture* texture, NVNtextureView* view, NVNcopyRegion* region, void* p);
void nvnTextureReadTexelsStrided(NVNtexture* texture, NVNtextureView* view, NVNcopyRegion* region, void* p, ptrdiff_t o1, ptrdiff_t o2);
void nvnTextureFlushTexels(NVNtexture* texture, NVNtextureView* view, NVNcopyRegion* region);
void nvnTextureInvalidateTexels(NVNtexture* texture, NVNtextureView* view, NVNcopyRegion* region);
NVNmemoryPool nvnTextureGetMemoryPool(NVNtexture* texture);
ptrdiff_t nvnTextureGetMemoryOffset(NVNtexture* texture);
int nvnTextureGetStorageSize(NVNtexture* texture);
int nvnTextureGetRawStorageClass(NVNtexture* texture); // guess
NVNboolean nvnTextureCompare(NVNtexture* texture1, NVNtexture* texture2);
uint64_t nvnTextureGetDebugID(NVNtexture* texture);
NVNdevice* nvnSamplerBuilderGetDevice(NVNsamplerBuilder* builder);
void nvnSamplerBuilderSetDevice(NVNsamplerBuilder* builder, NVNdevice* device);
void nvnSamplerBuilderSetDefaults(NVNsamplerBuilder* builder);
void nvnSamplerBuilderSetMinMagFilter(NVNsamplerBuilder* builder, NVNminFilter min, NVNmagFilter mag);
void nvnSamplerBuilderSetWrapMode(NVNsamplerBuilder* builder, NVNwrapMode s, NVNwrapMode t, NVNwrapMode r);
void nvnSamplerBuilderSetLodClamp(NVNsamplerBuilder* builder, float min, float max);
void nvnSamplerBuilderSetLodBias(NVNsamplerBuilder* builder, float bias);
void nvnSamplerBuilderSetCompare(NVNsamplerBuilder* builder, NVNcompareMode mode, NVNcompareFunc func);
void nvnSamplerBuilderSetBorderColor(NVNsamplerBuilder* builder, const float* borderColor);
void nvnSamplerBuilderSetBorderColori(NVNsamplerBuilder* builder, const int* borderColor);
void nvnSamplerBuilderSetBorderColorui(NVNsamplerBuilder* builder, const uint32_t* borderColor);
void nvnSamplerBuilderSetMaxAnisotropy(NVNsamplerBuilder* builder, float maxAniso);
void nvnSamplerBuilderSetReductionFilter(NVNsamplerBuilder* builder, NVNsamplerReduction filter);
void nvnSamplerBuilderSetLodSnap(NVNsamplerBuilder* builder, float f);
void nvnSamplerBuilderGetMinMagFilter(NVNsamplerBuilder* builder, NVNminFilter* min, NVNmagFilter* mag);
void nvnSamplerBuilderGetWrapMode(NVNsamplerBuilder* builder, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r);
void nvnSamplerBuilderGetLodClamp(NVNsamplerBuilder* builder, float* min, float* max);
float nvnSamplerBuilderGetLodBias(NVNsamplerBuilder* builder);
void nvnSamplerBuilderGetCompare(NVNsamplerBuilder* builder, NVNcompareMode* mode, NVNcompareFunc* func);
void nvnSamplerBuilderGetBorderColor(NVNsamplerBuilder* builder, float* borderColor);
void nvnSamplerBuilderGetBorderColori(NVNsamplerBuilder* builder, int* borderColor);
void nvnSamplerBuilderGetBorderColorui(NVNsamplerBuilder* builder, uint32_t* borderColor);
float nvnSamplerBuilderGetMaxAnisotropy(NVNsamplerBuilder* builder);
NVNsamplerReduction nvnSamplerBuilderGetReductionFilter(NVNsamplerBuilder* builder);
float nvnSamplerBuilderGetLodSnap(NVNsamplerBuilder* builder);
NVNboolean nvnSamplerInitialize(NVNsampler* sampler, NVNsamplerBuilder* builder);
void nvnSamplerFinalize(NVNsampler* sampler);
void nvnSamplerSetDebugLabel(NVNsampler* sampler, const char* label);
void nvnSamplerGetMinMagFilter(NVNsampler* sampler, NVNminFilter* min, NVNmagFilter* mag);
void nvnSamplerGetWrapMode(NVNsampler* sampler, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r);
void nvnSamplerGetLodClamp(NVNsampler* sampler, float* min, float* max);
float nvnSamplerGetLodBias(NVNsampler* sampler);
void nvnSamplerGetCompare(NVNsampler* sampler, NVNcompareMode* mode, NVNcompareFunc* func);
void nvnSamplerGetBorderColor(NVNsampler* sampler, float* borderColor);
void nvnSamplerGetBorderColori(NVNsampler* sampler, int* borderColor);
void nvnSamplerGetBorderColorui(NVNsampler* sampler, uint32_t* borderColor);
float nvnSamplerGetMaxAnisotropy(NVNsampler* sampler);
NVNsamplerReduction nvnSamplerGetReductionFilter(NVNsampler* sampler);
NVNboolean nvnSamplerCompare(NVNsampler* sampler1, NVNsampler* sampler2);
uint64_t nvnSamplerGetDebugID(NVNsampler* sampler);
void nvnBlendStateSetDefaults(NVNblendState* blend);
void nvnBlendStateSetBlendTarget(NVNblendState* blend, int target);
void nvnBlendStateSetBlendFunc(NVNblendState* blend, NVNblendFunc srcFunc, NVNblendFunc dstFunc, NVNblendFunc srcFuncAlpha, NVNblendFunc dstFuncAlpha);
void nvnBlendStateSetBlendEquation(NVNblendState* blend, NVNblendEquation modeRGB, NVNblendEquation modeAlpha);
void nvnBlendStateSetAdvancedMode(NVNblendState* blend, NVNblendAdvancedMode mode);
void nvnBlendStateSetAdvancedOverlap(NVNblendState* blend, NVNblendAdvancedOverlap overlap);
void nvnBlendStateSetAdvancedPremultipliedSrc(NVNblendState* blend, NVNboolean b);
void nvnBlendStateSetAdvancedNormalizedDst(NVNblendState* blend, NVNboolean b);
int nvnBlendStateGetBlendTarget(NVNblendState* blend);
void nvnBlendStateGetBlendFunc(NVNblendState* blend, NVNblendFunc* srcFunc, NVNblendFunc* dstFunc, NVNblendFunc* srcFuncAlpha, NVNblendFunc* dstFuncAlpha);
void nvnBlendStateGetBlendEquation(NVNblendState* blend, NVNblendEquation* modeRGB, NVNblendEquation* modeAlpha);
NVNblendAdvancedMode nvnBlendStateGetAdvancedMode(NVNblendState* blend);
NVNblendAdvancedOverlap nvnBlendStateGetAdvancedOverlap(NVNblendState* blend);
NVNboolean nvnBlendStateGetAdvancedPremultipliedSrc(NVNblendState* blend);
NVNboolean nvnBlendStateGetAdvancedNormalizedDst(NVNblendState* blend);
void nvnColorStateSetDefaults(NVNcolorState* color);
void nvnColorStateSetBlendEnable(NVNcolorState* color, int index, NVNboolean enable);
void nvnColorStateSetLogicOp(NVNcolorState* color, NVNlogicOp logicOp);
void nvnColorStateSetAlphaTest(NVNcolorState* color, NVNalphaFunc alphaFunc);
NVNboolean nvnColorStateGetBlendEnable(NVNcolorState* color, int index);
NVNlogicOp nvnColorStateGetLogicOp(NVNcolorState* color);
NVNalphaFunc nvnColorStateGetAlphaTest(NVNcolorState* color);
void nvnChannelMaskStateSetDefaults(NVNchannelMaskState* channelMask);
void nvnChannelMaskStateSetChannelMask(NVNchannelMaskState* channelMask, int index, NVNboolean r, NVNboolean g, NVNboolean b, NVNboolean a);
void nvnChannelMaskStateGetChannelMask(NVNchannelMaskState* channelMask, int index, NVNboolean* r, NVNboolean* g, NVNboolean* b, NVNboolean* a);
void nvnMultisampleStateSetDefaults(NVNmultisampleState* multisample);
void nvnMultisampleStateSetMultisampleEnable(NVNmultisampleState* multisample, NVNboolean enable);
void nvnMultisampleStateSetSamples(NVNmultisampleState* multisample, int samples);
void nvnMultisampleStateSetAlphaToCoverageEnable(NVNmultisampleState* multisample, NVNboolean enable);
void nvnMultisampleStateSetAlphaToCoverageDither(NVNmultisampleState* multisample, NVNboolean enable);
NVNboolean nvnMultisampleStateGetMultisampleEnable(NVNmultisampleState* multisample);
int nvnMultisampleStateGetSamples(NVNmultisampleState* multisample);
NVNboolean nvnMultisampleStateGetAlphaToCoverageEnable(NVNmultisampleState* multisample);
NVNboolean nvnMultisampleStateGetAlphaToCoverageDither(NVNmultisampleState* multisample);
void nvnMultisampleStateSetRasterSamples(NVNmultisampleState* multisample, int samples);
int nvnMultisampleStateGetRasterSamples(NVNmultisampleState* multisample);
void nvnMultisampleStateSetCoverageModulationMode(NVNmultisampleState* multisample, NVNcoverageModulationMode mode);
NVNcoverageModulationMode nvnMultisampleStateGetCoverageModulationMode(NVNmultisampleState* multisample);
void nvnMultisampleStateSetCoverageToColorEnable(NVNmultisampleState* multisample, NVNboolean enable);
NVNboolean nvnMultisampleStateGetCoverageToColorEnable(NVNmultisampleState* multisample);
void nvnMultisampleStateSetCoverageToColorOutput(NVNmultisampleState* multisample, int i);
int nvnMultisampleStateGetCoverageToColorOutput(NVNmultisampleState* multisample);
void nvnMultisampleStateSetSampleLocationsEnable(NVNmultisampleState* multisample, NVNboolean enable);
NVNboolean nvnMultisampleStateGetSampleLocationsEnable(NVNmultisampleState* multisample);
void nvnMultisampleStateGetSampleLocationsGrid(NVNmultisampleState* multisample, int* w, int* h);
void nvnMultisampleStateSetSampleLocationsGridEnable(NVNmultisampleState* multisample, NVNboolean enable);
NVNboolean nvnMultisampleStateGetSampleLocationsGridEnable(NVNmultisampleState* multisample);
void nvnMultisampleStateSetSampleLocations(NVNmultisampleState* multisample, int i1, int i2, const float* f);
void nvnPolygonStateSetDefaults(NVNpolygonState* polygon);
void nvnPolygonStateSetCullFace(NVNpolygonState* polygon, NVNface face);
void nvnPolygonStateSetFrontFace(NVNpolygonState* polygon, NVNfrontFace face);
void nvnPolygonStateSetPolygonMode(NVNpolygonState* polygon, NVNpolygonMode polygonMode);
void nvnPolygonStateSetPolygonOffsetEnables(NVNpolygonState* polygon, int enables);
NVNface nvnPolygonStateGetCullFace(NVNpolygonState* polygon);
NVNfrontFace nvnPolygonStateGetFrontFace(NVNpolygonState* polygon);
NVNpolygonMode nvnPolygonStateGetPolygonMode(NVNpolygonState* polygon);
NVNpolygonOffsetEnable nvnPolygonStateGetPolygonOffsetEnables(NVNpolygonState* polygon);
void nvnDepthStencilStateSetDefaults(NVNdepthStencilState* depthStencil);
void nvnDepthStencilStateSetDepthTestEnable(NVNdepthStencilState* depthStencil, NVNboolean enable);
void nvnDepthStencilStateSetDepthWriteEnable(NVNdepthStencilState* depthStencil, NVNboolean enable);
void nvnDepthStencilStateSetDepthFunc(NVNdepthStencilState* depthStencil, NVNdepthFunc func);
void nvnDepthStencilStateSetStencilTestEnable(NVNdepthStencilState* depthStencil, NVNboolean enable);
void nvnDepthStencilStateSetStencilFunc(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilFunc func);
void nvnDepthStencilStateSetStencilOp(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilOp fail, NVNstencilOp depthFail, NVNstencilOp depthPass);
NVNboolean nvnDepthStencilStateGetDepthTestEnable(NVNdepthStencilState* depthStencil);
NVNboolean nvnDepthStencilStateGetDepthWriteEnable(NVNdepthStencilState* depthStencil);
NVNdepthFunc nvnDepthStencilStateGetDepthFunc(NVNdepthStencilState* depthStencil);
NVNboolean nvnDepthStencilStateGetStencilTestEnable(NVNdepthStencilState* depthStencil);
NVNstencilFunc nvnDepthStencilStateGetStencilFunc(NVNdepthStencilState* depthStencil, NVNface faces);
void nvnDepthStencilStateGetStencilOp(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilOp* fail, NVNstencilOp* depthFail, NVNstencilOp* depthPass);
void nvnVertexAttribStateSetDefaults(NVNvertexAttribState* attrib);
void nvnVertexAttribStateSetFormat(NVNvertexAttribState* attrib, NVNformat format, ptrdiff_t relativeOffset);
void nvnVertexAttribStateSetStreamIndex(NVNvertexAttribState* attrib, int streamIndex);
void nvnVertexAttribStateGetFormat(NVNvertexAttribState* attrib, NVNformat* format, ptrdiff_t* relativeOffset);
int nvnVertexAttribStateGetStreamIndex(NVNvertexAttribState* attrib);
void nvnVertexStreamStateSetDefaults(NVNvertexStreamState* stream);
void nvnVertexStreamStateSetStride(NVNvertexStreamState* stream, ptrdiff_t stride);
void nvnVertexStreamStateSetDivisor(NVNvertexStreamState* stream, int divisor);
ptrdiff_t nvnVertexStreamStateGetStride(NVNvertexStreamState* stream);
int nvnVertexStreamStateGetDivisor(NVNvertexStreamState* stream);
NVNboolean nvnCommandBufferInitialize(NVNcommandBuffer* cmdBuf, NVNdevice* device);
void nvnCommandBufferFinalize(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferSetDebugLabel(NVNcommandBuffer* cmdBuf, const char* label);
void nvnCommandBufferSetMemoryCallback(NVNcommandBuffer* cmdBuf, PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC callback);
void nvnCommandBufferSetMemoryCallbackData(NVNcommandBuffer* cmdBuf, void* callbackData);
void nvnCommandBufferAddCommandMemory(NVNcommandBuffer* cmdBuf, NVNmemoryPool* pool, ptrdiff_t offset, size_t size);
void nvnCommandBufferAddControlMemory(NVNcommandBuffer* cmdBuf, void* memory, size_t size);
size_t nvnCommandBufferGetCommandMemorySize(NVNcommandBuffer* cmdBuf);
size_t nvnCommandBufferGetCommandMemoryUsed(NVNcommandBuffer* cmdBuf);
size_t nvnCommandBufferGetCommandMemoryFree(NVNcommandBuffer* cmdBuf);
size_t nvnCommandBufferGetControlMemorySize(NVNcommandBuffer* cmdBuf);
size_t nvnCommandBufferGetControlMemoryUsed(NVNcommandBuffer* cmdBuf);
size_t nvnCommandBufferGetControlMemoryFree(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferBeginRecording(NVNcommandBuffer* cmdBuf);
NVNcommandHandle nvnCommandBufferEndRecording(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferCallCommands(NVNcommandBuffer* cmdBuf, int numCommands, const NVNcommandHandle* handles);
void nvnCommandBufferCopyCommands(NVNcommandBuffer* cmdBuf, int numCommands, const NVNcommandHandle* handles);
void nvnCommandBufferBindBlendState(NVNcommandBuffer* cmdBuf, NVNblendState* blend);
void nvnCommandBufferBindChannelMaskState(NVNcommandBuffer* cmdBuf, NVNchannelMaskState* channelMask);
void nvnCommandBufferBindColorState(NVNcommandBuffer* cmdBuf, NVNcolorState* color);
void nvnCommandBufferBindMultisampleState(NVNcommandBuffer* cmdBuf, NVNmultisampleState* multisample);
void nvnCommandBufferBindPolygonState(NVNcommandBuffer* cmdBuf, NVNpolygonState* polygon);
void nvnCommandBufferBindDepthStencilState(NVNcommandBuffer* cmdBuf, NVNdepthStencilState* depthStencil);
void nvnCommandBufferBindVertexAttribState(NVNcommandBuffer* cmdBuf, int numAttribs, NVNvertexAttribState* attribs);
void nvnCommandBufferBindVertexStreamState(NVNcommandBuffer* cmdBuf, int numStreams, NVNvertexStreamState* streams);
void nvnCommandBufferBindProgram(NVNcommandBuffer* cmdBuf, NVNprogram* program, int stages);
void nvnCommandBufferBindVertexBuffer(NVNcommandBuffer* cmdBuf, int index, NVNbufferAddress buffer, size_t size);
void nvnCommandBufferBindVertexBuffers(NVNcommandBuffer* cmdBuf, int first, int count, const NVNbufferRange* buffers);
void nvnCommandBufferBindUniformBuffer(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size);
void nvnCommandBufferBindUniformBuffers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers);
void nvnCommandBufferBindTransformFeedbackBuffer(NVNcommandBuffer* cmdBuf, int index, NVNbufferAddress buffer, size_t size);
void nvnCommandBufferBindTransformFeedbackBuffers(NVNcommandBuffer* cmdBuf, int first, int count, const NVNbufferRange* buffers);
void nvnCommandBufferBindStorageBuffer(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size);
void nvnCommandBufferBindStorageBuffers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers);
void nvnCommandBufferBindTexture(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNtextureHandle texture);
void nvnCommandBufferBindTextures(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNtextureHandle* textures);
void nvnCommandBufferBindImage(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNimageHandle image);
void nvnCommandBufferBindImages(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNimageHandle* images);
void nvnCommandBufferSetPatchSize(NVNcommandBuffer* cmdBuf, int i);
void nvnCommandBufferSetInnerTessellationLevels(NVNcommandBuffer* cmdBuf, const float* f);
void nvnCommandBufferSetOuterTessellationLevels(NVNcommandBuffer* cmdBuf, const float* f);
void nvnCommandBufferSetPrimitiveRestart(NVNcommandBuffer* cmdBuf, NVNboolean b, int i);
void nvnCommandBufferBeginTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer);
void nvnCommandBufferEndTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer);
void nvnCommandBufferPauseTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer);
void nvnCommandBufferResumeTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer);
void nvnCommandBufferDrawTransformFeedback(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer);
void nvnCommandBufferDrawArrays(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, int first, int count);
void nvnCommandBufferDrawElements(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer);
void nvnCommandBufferDrawElementsBaseVertex(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex);
void nvnCommandBufferDrawArraysInstanced(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, int first, int count, int baseInstance, int instanceCount);
void nvnCommandBufferDrawElementsInstanced(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex, int baseInstance, int instanceCount);
void nvnCommandBufferDrawArraysIndirect(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer);
void nvnCommandBufferDrawElementsIndirect(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2);
void nvnCommandBufferMultiDrawArraysIndirectCount(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer1, NVNbufferAddress buffer2, int i, ptrdiff_t o);
void nvnCommandBufferMultiDrawElementsIndirectCount(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2, NVNbufferAddress buffer3, int i, ptrdiff_t o);
void nvnCommandBufferClearColor(NVNcommandBuffer* cmdBuf, int index, const float* color, int mask);
void nvnCommandBufferClearColori(NVNcommandBuffer* cmdBuf, int index, const int* color, int mask);
void nvnCommandBufferClearColorui(NVNcommandBuffer* cmdBuf, int index, const uint32_t* color, int mask);
void nvnCommandBufferClearDepthStencil(NVNcommandBuffer* cmdBuf, float depthValue, NVNboolean depthMask, int stencilValue, int stencilMask);
void nvnCommandBufferDispatchCompute(NVNcommandBuffer* cmdBuf, int groupsX, int groupsY, int groupsZ);
void nvnCommandBufferDispatchComputeIndirect(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer);
void nvnCommandBufferSetViewport(NVNcommandBuffer* cmdBuf, int x, int y, int w, int h);
void nvnCommandBufferSetViewports(NVNcommandBuffer* cmdBuf, int first, int count, const float* ranges);
void nvnCommandBufferSetViewportSwizzles(NVNcommandBuffer* cmdBuf, int first, int count, const NVNviewportSwizzle* swizzles);
void nvnCommandBufferSetScissor(NVNcommandBuffer* cmdBuf, int x, int y, int w, int h);
void nvnCommandBufferSetScissors(NVNcommandBuffer* cmdBuf, int first, int count, const int* rects);
void nvnCommandBufferSetDepthRange(NVNcommandBuffer* cmdBuf, float n, float f);
void nvnCommandBufferSetDepthBounds(NVNcommandBuffer* cmdBuf, NVNboolean enable, float n, float f);
void nvnCommandBufferSetDepthRanges(NVNcommandBuffer* cmdBuf, int first, int count, const float* ranges);
void nvnCommandBufferSetTiledCacheAction(NVNcommandBuffer* cmdBuf, NVNtiledCacheAction action);
void nvnCommandBufferSetTiledCacheTileSize(NVNcommandBuffer* cmdBuf, int w, int h);
void nvnCommandBufferBindSeparateTexture(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i, NVNseparateTextureHandle handle);
void nvnCommandBufferBindSeparateSampler(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i, NVNseparateSamplerHandle handle);
void nvnCommandBufferBindSeparateTextures(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateTextureHandle* handle);
void nvnCommandBufferBindSeparateSamplers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateSamplerHandle* handle);
void nvnCommandBufferSetStencilValueMask(NVNcommandBuffer* cmdBuf, NVNface faces, int mask);
void nvnCommandBufferSetStencilMask(NVNcommandBuffer* cmdBuf, NVNface faces, int mask);
void nvnCommandBufferSetStencilRef(NVNcommandBuffer* cmdBuf, NVNface faces, int ref);
void nvnCommandBufferSetStencilCullCriteria(NVNcommandBuffer* cmdBuf); // no idea
void nvnCommandBufferSetBlendColor(NVNcommandBuffer* cmdBuf, const float* blendColor);
void nvnCommandBufferSetColorReductionEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable);
void nvnCommandBufferSetColorReductionThresholds(NVNcommandBuffer* cmdBuf, float f); // pure guess
void nvnCommandBufferSetPointSize(NVNcommandBuffer* cmdBuf, float pointSize);
void nvnCommandBufferSetLineWidth(NVNcommandBuffer* cmdBuf, float lineWidth);
void nvnCommandBufferSetPolygonOffsetClamp(NVNcommandBuffer* cmdBuf, float factor, float units, float clamp);
void nvnCommandBufferSetAlphaRef(NVNcommandBuffer* cmdBuf, float ref);
void nvnCommandBufferSetSampleMask(NVNcommandBuffer* cmdBuf, int mask);
void nvnCommandBufferSetRasterizerDiscard(NVNcommandBuffer* cmdBuf, NVNboolean discard);
void nvnCommandBufferSetDepthClamp(NVNcommandBuffer* cmdBuf, NVNboolean clamp);
void nvnCommandBufferSetConservativeRasterEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable);
void nvnCommandBufferSetConservativeRasterDilate(NVNcommandBuffer* cmdBuf, float f);
void nvnCommandBufferSetSubpixelPrecisionBias(NVNcommandBuffer* cmdBuf, int i1, int i2);
void nvnCommandBufferSetCommandMemoryCallbackEnabled(NVNcommandBuffer* cmdBuf, NVNboolean enable); // guess
void nvnCommandBufferCopyBufferToTexture(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, NVNtexture* dstTexture, NVNtextureView* dstView, NVNcopyRegion* dstRegion, int flags);
void nvnCommandBufferCopyTextureToBuffer(NVNcommandBuffer* cmdBuf, NVNtexture* srcTexture, NVNtextureView* srcView, NVNcopyRegion* srcRegion, NVNbufferAddress buffer, int flags);
void nvnCommandBufferCopyTextureToTexture(NVNcommandBuffer* cmdBuf, NVNtexture* srcTexture, NVNtextureView* srcView, NVNcopyRegion* srcRegion, NVNtexture* dstTexture, NVNtextureView* dstView, NVNcopyRegion* dstRegion, int flags);
void nvnCommandBufferCopyBufferToBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress src, NVNbufferAddress dst, size_t size, int flags);
void nvnCommandBufferClearBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, size_t size, uint32_t i);
void nvnCommandBufferClearTexture(NVNcommandBuffer* cmdBuf, NVNtexture* dstTexture, NVNtextureView* dstView, NVNcopyRegion* dstRegion, const float* color, int mask);
void nvnCommandBufferClearTexturei(NVNcommandBuffer* cmdBuf, NVNtexture* dstTexture, NVNtextureView* dstView, NVNcopyRegion* dstRegion, const int* color, int mask);
void nvnCommandBufferClearTextureui(NVNcommandBuffer* cmdBuf, NVNtexture* dstTexture, NVNtextureView* dstView, NVNcopyRegion* dstRegion, const uint32_t* color, int mask);
void nvnCommandBufferUpdateUniformBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, size_t size, ptrdiff_t o, size_t s, const void* p);
void nvnCommandBufferReportCounter(NVNcommandBuffer* cmdBuf, NVNcounterType counter, NVNbufferAddress buffer);
void nvnCommandBufferResetCounter(NVNcommandBuffer* cmdBuf, NVNcounterType counter);
void nvnCommandBufferReportValue(NVNcommandBuffer* cmdBuf, uint32_t value, NVNbufferAddress buffer);
void nvnCommandBufferSetRenderEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable);
void nvnCommandBufferSetRenderEnableConditional(NVNcommandBuffer* cmdBuf, NVNconditionalRenderMode mode, NVNbufferAddress addr);
void nvnCommandBufferSetRenderTargets(NVNcommandBuffer* cmdBuf, int numColors, NVNtexture* const* colors, NVNtextureView* const* colorViews, NVNtexture* depthStencil, NVNtextureView* depthStencilView);
void nvnCommandBufferDiscardColor(NVNcommandBuffer* cmdBuf, int i);
void nvnCommandBufferDiscardDepthStencil(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferDownsample(NVNcommandBuffer* cmdBuf, NVNtexture* src, NVNtexture* dst);
void nvnCommandBufferTiledDownsample(NVNcommandBuffer* cmdBuf, NVNtexture* texture1, NVNtexture* texture2);
void nvnCommandBufferDownsampleTextureView(NVNcommandBuffer* cmdBuf, NVNtexture* texture1, NVNtextureView* view1, NVNtexture* texture2, NVNtextureView* view2);
void nvnCommandBufferTiledDownsampleTextureView(NVNcommandBuffer* cmdBuf, NVNtexture* texture1, NVNtextureView* view1, NVNtexture* texture2, NVNtextureView* view2);
void nvnCommandBufferBarrier(NVNcommandBuffer* cmdBuf, int barrier);
void nvnCommandBufferWaitSync(NVNcommandBuffer* cmdBuf, NVNsync* sync);
void nvnCommandBufferFenceSync(NVNcommandBuffer* cmdBuf, NVNsync* sync, NVNsyncCondition condition, int fence);
void nvnCommandBufferSetTexturePool(NVNcommandBuffer* cmdBuf, NVNtexturePool* pool);
void nvnCommandBufferSetSamplerPool(NVNcommandBuffer* cmdBuf, NVNsamplerPool* pool);
void nvnCommandBufferSetShaderScratchMemory(NVNcommandBuffer* cmdBuf, NVNmemoryPool* pool, ptrdiff_t offset, size_t size);
void nvnCommandBufferSaveZCullData(NVNcommandBuffer* cmdBuf, NVNbufferAddress addr, size_t size);
void nvnCommandBufferRestoreZCullData(NVNcommandBuffer* cmdBuf, NVNbufferAddress addr, size_t size);
void nvnCommandBufferSetCopyRowStride(NVNcommandBuffer* cmdBuf, ptrdiff_t stride);
void nvnCommandBufferSetCopyImageStride(NVNcommandBuffer* cmdBuf, ptrdiff_t stride);
ptrdiff_t nvnCommandBufferGetCopyRowStride(NVNcommandBuffer* cmdBuf);
ptrdiff_t nvnCommandBufferGetCopyImageStride(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferDrawTexture(NVNcommandBuffer* cmdBuf, NVNtextureHandle handle, const NVNdrawTextureRegion* region1, const NVNdrawTextureRegion* region2);
NVNboolean nvnProgramSetSubroutineLinkage(NVNprogram* program, int i, const NVNsubroutineLinkageMapPtr* ptr);
void nvnCommandBufferSetProgramSubroutines(NVNcommandBuffer* cmdBuf, NVNprogram* program, NVNshaderStage stage, const int i1, const int i2, const int* i3);
void nvnCommandBufferBindCoverageModulationTable(NVNcommandBuffer* cmdBuf, const float* f);
void nvnCommandBufferResolveDepthBuffer(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferPushDebugGroupStatic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description);
void nvnCommandBufferPushDebugGroupDynamic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description);
void nvnCommandBufferPushDebugGroup(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description);
void nvnCommandBufferPopDebugGroup(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferPopDebugGroupId(NVNcommandBuffer* cmdBuf, uint32_t i);
void nvnCommandBufferInsertDebugMarkerStatic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description);
void nvnCommandBufferInsertDebugMarkerDynamic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description);
void nvnCommandBufferInsertDebugMarker(NVNcommandBuffer* cmdBuf, const char* description);
PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC nvnCommandBufferGetMemoryCallback(NVNcommandBuffer* cmdBuf);
void nvnCommandBufferGetMemoryCallbackData(NVNcommandBuffer* cmdBuf);
NVNboolean nvnCommandBufferIsRecording(NVNcommandBuffer* cmdBuf);
NVNboolean nvnSyncInitialize(NVNsync* sync, NVNdevice* device);
NVNboolean nvnSyncInitializeFromFencedGLSync(NVNsync* sync); // pure guess
void* nvnSyncCreateGLSync(NVNsync* sync); // pure guess
void nvnSyncFinalize(NVNsync* sync);
void nvnSyncSetDebugLabel(NVNsync* sync, const char* label);
void nvnQueueFenceSync(NVNqueue* queue, NVNsync* sync, NVNsyncCondition condition, int flags);
NVNsyncWaitResult nvnSyncWait(NVNsync* sync, uint64_t timeoutNs);
NVNboolean nvnQueueWaitSync(NVNqueue* queue, NVNsync* sync);
void nvnEventBuilderSetDefaults(NVNeventBuilder* builder);
NVNmemoryPool* nvnEventBuilderGetMemoryPool(NVNeventBuilder* builder); // guess
int nvnEventBuilderGetMemoryOffset(NVNeventBuilder* builder); // guess
NVNmemoryPool* nvnEventBuilderGetStorage(NVNeventBuilder* builder); // guess
void nvnEventBuilderSetStorage(NVNeventBuilder* builder, NVNmemoryPool* pool, int64_t size);
NVNboolean nvnEventInitialize(NVNevent* event, NVNeventBuilder* builder);
void nvnEventFinalize(NVNevent* event);
uint32_t nvnEventGetValue(NVNevent* event);
NVNmemoryPool* nvnEventGetMemoryPool(NVNevent* builder); // guess
int nvnEventGetMemoryOffset(NVNevent* builder); // guess
void nvnEventSignal(NVNevent* event, NVNeventSignalMode mode, uint32_t i);
void nvnCommandBufferWaitEvent(NVNcommandBuffer* cmdBuf, NVNevent* event, NVNeventWaitMode mode, uint32_t i);
void nvnCommandBufferSignalEvent(NVNcommandBuffer* cmdBuf, NVNevent* event, NVNeventSignalMode mode, NVNeventSignalLocation location, int flags, uint32_t i);

#define NVN_ALL(with) \
with(nvnBlendStateGetAdvancedMode) \
with(nvnBlendStateGetAdvancedNormalizedDst) \
with(nvnBlendStateGetAdvancedOverlap) \
with(nvnBlendStateGetAdvancedPremultipliedSrc) \
with(nvnBlendStateGetBlendEquation) \
with(nvnBlendStateGetBlendFunc) \
with(nvnBlendStateGetBlendTarget) \
with(nvnBlendStateSetAdvancedMode) \
with(nvnBlendStateSetAdvancedNormalizedDst) \
with(nvnBlendStateSetAdvancedOverlap) \
with(nvnBlendStateSetAdvancedPremultipliedSrc) \
with(nvnBlendStateSetBlendEquation) \
with(nvnBlendStateSetBlendFunc) \
with(nvnBlendStateSetBlendTarget) \
with(nvnBlendStateSetDefaults) \
with(nvnBufferBuilderGetMemoryOffset) \
with(nvnBufferBuilderGetMemoryPool) \
with(nvnBufferBuilderGetSize) \
with(nvnBufferBuilderSetDefaults) \
with(nvnBufferBuilderGetDevice) \
with(nvnBufferBuilderSetDevice) \
with(nvnBufferBuilderSetStorage) \
with(nvnBufferFinalize) \
with(nvnBufferFlushMappedRange) \
with(nvnBufferGetAddress) \
with(nvnBufferGetDebugID) \
with(nvnBufferGetMemoryOffset) \
with(nvnBufferGetMemoryPool) \
with(nvnBufferGetSize) \
with(nvnBufferInitialize) \
with(nvnBufferInvalidateMappedRange) \
with(nvnBufferMap) \
with(nvnBufferSetDebugLabel) \
with(nvnChannelMaskStateGetChannelMask) \
with(nvnChannelMaskStateSetChannelMask) \
with(nvnChannelMaskStateSetDefaults) \
with(nvnColorStateGetAlphaTest) \
with(nvnColorStateGetBlendEnable) \
with(nvnColorStateGetLogicOp) \
with(nvnColorStateSetAlphaTest) \
with(nvnColorStateSetBlendEnable) \
with(nvnColorStateSetDefaults) \
with(nvnColorStateSetLogicOp) \
with(nvnCommandBufferAddCommandMemory) \
with(nvnCommandBufferAddControlMemory) \
with(nvnCommandBufferBarrier) \
with(nvnCommandBufferBeginRecording) \
with(nvnCommandBufferBeginTransformFeedback) \
with(nvnCommandBufferBindBlendState) \
with(nvnCommandBufferBindChannelMaskState) \
with(nvnCommandBufferBindColorState) \
with(nvnCommandBufferBindCoverageModulationTable) \
with(nvnCommandBufferBindDepthStencilState) \
with(nvnCommandBufferBindImage) \
with(nvnCommandBufferBindImages) \
with(nvnCommandBufferBindMultisampleState) \
with(nvnCommandBufferBindPolygonState) \
with(nvnCommandBufferBindProgram) \
with(nvnCommandBufferBindStorageBuffer) \
with(nvnCommandBufferBindStorageBuffers) \
with(nvnCommandBufferBindTexture) \
with(nvnCommandBufferBindTextures) \
with(nvnCommandBufferBindTransformFeedbackBuffer) \
with(nvnCommandBufferBindTransformFeedbackBuffers) \
with(nvnCommandBufferBindUniformBuffer) \
with(nvnCommandBufferBindUniformBuffers) \
with(nvnCommandBufferBindVertexAttribState) \
with(nvnCommandBufferBindVertexBuffer) \
with(nvnCommandBufferBindVertexBuffers) \
with(nvnCommandBufferBindVertexStreamState) \
with(nvnCommandBufferCallCommands) \
with(nvnCommandBufferClearBuffer) \
with(nvnCommandBufferClearColor) \
with(nvnCommandBufferClearColori) \
with(nvnCommandBufferClearColorui) \
with(nvnCommandBufferClearDepthStencil) \
with(nvnCommandBufferClearTexture) \
with(nvnCommandBufferClearTexturei) \
with(nvnCommandBufferClearTextureui) \
with(nvnCommandBufferCopyBufferToBuffer) \
with(nvnCommandBufferCopyBufferToTexture) \
with(nvnCommandBufferCopyCommands) \
with(nvnCommandBufferCopyTextureToBuffer) \
with(nvnCommandBufferCopyTextureToTexture) \
with(nvnCommandBufferDiscardColor) \
with(nvnCommandBufferDiscardDepthStencil) \
with(nvnCommandBufferDispatchCompute) \
with(nvnCommandBufferDispatchComputeIndirect) \
with(nvnCommandBufferDownsample) \
with(nvnCommandBufferDrawArrays) \
with(nvnCommandBufferDrawArraysIndirect) \
with(nvnCommandBufferDrawArraysInstanced) \
with(nvnCommandBufferDrawElements) \
with(nvnCommandBufferDrawElementsBaseVertex) \
with(nvnCommandBufferDrawElementsIndirect) \
with(nvnCommandBufferDrawElementsInstanced) \
with(nvnCommandBufferDrawTexture) \
with(nvnCommandBufferDrawTransformFeedback) \
with(nvnCommandBufferEndRecording) \
with(nvnCommandBufferEndTransformFeedback) \
with(nvnCommandBufferFenceSync) \
with(nvnCommandBufferFinalize) \
with(nvnCommandBufferGetCommandMemoryFree) \
with(nvnCommandBufferGetCommandMemorySize) \
with(nvnCommandBufferGetCommandMemoryUsed) \
with(nvnCommandBufferGetControlMemoryFree) \
with(nvnCommandBufferGetControlMemorySize) \
with(nvnCommandBufferGetControlMemoryUsed) \
with(nvnCommandBufferGetCopyImageStride) \
with(nvnCommandBufferGetCopyRowStride) \
with(nvnCommandBufferGetMemoryCallback) \
with(nvnCommandBufferGetMemoryCallbackData) \
with(nvnCommandBufferInitialize) \
with(nvnCommandBufferInsertDebugMarker) \
with(nvnCommandBufferIsRecording) \
with(nvnCommandBufferMultiDrawArraysIndirectCount) \
with(nvnCommandBufferMultiDrawElementsIndirectCount) \
with(nvnCommandBufferPauseTransformFeedback) \
with(nvnCommandBufferPopDebugGroup) \
with(nvnCommandBufferPopDebugGroupId) \
with(nvnCommandBufferInsertDebugMarkerStatic) \
with(nvnCommandBufferInsertDebugMarkerDynamic) \
with(nvnCommandBufferPushDebugGroup) \
with(nvnCommandBufferReportCounter) \
with(nvnCommandBufferReportValue) \
with(nvnCommandBufferResetCounter) \
with(nvnCommandBufferResolveDepthBuffer) \
with(nvnCommandBufferPushDebugGroupStatic) \
with(nvnCommandBufferPushDebugGroupDynamic) \
with(nvnCommandBufferRestoreZCullData) \
with(nvnCommandBufferResumeTransformFeedback) \
with(nvnCommandBufferSaveZCullData) \
with(nvnCommandBufferSetAlphaRef) \
with(nvnCommandBufferSetBlendColor) \
with(nvnCommandBufferSetColorReductionEnable) \
with(nvnCommandBufferSetColorReductionThresholds) \
with(nvnCommandBufferSetConservativeRasterDilate) \
with(nvnCommandBufferSetConservativeRasterEnable) \
with(nvnCommandBufferSetCopyImageStride) \
with(nvnCommandBufferSetCopyRowStride) \
with(nvnCommandBufferSetDebugLabel) \
with(nvnCommandBufferSetDepthBounds) \
with(nvnCommandBufferSetDepthClamp) \
with(nvnCommandBufferSetDepthRange) \
with(nvnCommandBufferSetDepthRanges) \
with(nvnCommandBufferSetInnerTessellationLevels) \
with(nvnCommandBufferSetLineWidth) \
with(nvnCommandBufferSetMemoryCallback) \
with(nvnCommandBufferSetMemoryCallbackData) \
with(nvnCommandBufferSetOuterTessellationLevels) \
with(nvnCommandBufferSetPatchSize) \
with(nvnCommandBufferSetPointSize) \
with(nvnCommandBufferSetPolygonOffsetClamp) \
with(nvnCommandBufferSetPrimitiveRestart) \
with(nvnCommandBufferSetProgramSubroutines) \
with(nvnCommandBufferSetRasterizerDiscard) \
with(nvnCommandBufferSetRenderEnable) \
with(nvnCommandBufferSetRenderEnableConditional) \
with(nvnCommandBufferSetRenderTargets) \
with(nvnCommandBufferSetSampleMask) \
with(nvnCommandBufferSetSamplerPool) \
with(nvnCommandBufferSetScissor) \
with(nvnCommandBufferSetScissors) \
with(nvnCommandBufferSetShaderScratchMemory) \
with(nvnCommandBufferSetStencilMask) \
with(nvnCommandBufferSetStencilRef) \
with(nvnCommandBufferSetStencilValueMask) \
with(nvnCommandBufferSetStencilCullCriteria) \
with(nvnCommandBufferSetSubpixelPrecisionBias) \
with(nvnCommandBufferSetCommandMemoryCallbackEnabled) \
with(nvnCommandBufferSetTexturePool) \
with(nvnCommandBufferSetTiledCacheAction) \
with(nvnCommandBufferSetTiledCacheTileSize) \
with(nvnCommandBufferBindSeparateTexture) \
with(nvnCommandBufferBindSeparateSampler) \
with(nvnCommandBufferBindSeparateTextures) \
with(nvnCommandBufferBindSeparateSamplers) \
with(nvnCommandBufferSetViewport) \
with(nvnCommandBufferSetViewportSwizzles) \
with(nvnCommandBufferSetViewports) \
with(nvnCommandBufferTiledDownsample) \
with(nvnCommandBufferDownsampleTextureView) \
with(nvnCommandBufferTiledDownsampleTextureView) \
with(nvnCommandBufferUpdateUniformBuffer) \
with(nvnCommandBufferWaitSync) \
with(nvnDepthStencilStateGetDepthFunc) \
with(nvnDepthStencilStateGetDepthTestEnable) \
with(nvnDepthStencilStateGetDepthWriteEnable) \
with(nvnDepthStencilStateGetStencilFunc) \
with(nvnDepthStencilStateGetStencilOp) \
with(nvnDepthStencilStateGetStencilTestEnable) \
with(nvnDepthStencilStateSetDefaults) \
with(nvnDepthStencilStateSetDepthFunc) \
with(nvnDepthStencilStateSetDepthTestEnable) \
with(nvnDepthStencilStateSetDepthWriteEnable) \
with(nvnDepthStencilStateSetStencilFunc) \
with(nvnDepthStencilStateSetStencilOp) \
with(nvnDepthStencilStateSetStencilTestEnable) \
with(nvnDeviceApplyDeferredFinalizes) \
with(nvnDeviceBuilderSetDefaults) \
with(nvnDeviceBuilderGetFlags) \
with(nvnDeviceBuilderSetFlags) \
with(nvnDeviceFinalize) \
with(nvnDeviceFinalizeCommandHandle) \
with(nvnDeviceWalkDebugDatabase) \
with(nvnDeviceGetSeparateTextureHandle) \
with(nvnDeviceGetSeparateSamplerHandle) \
with(nvnDeviceIsExternalDebuggerAttached) \
with(nvnDeviceGetCurrentTimestampInNanoseconds) \
with(nvnDeviceGetDepthMode) \
with(nvnDeviceGetImageHandle) \
with(nvnDeviceInstallDebugCallback) \
with(nvnDeviceGenerateDebugDomainId) \
with(nvnDeviceGetInteger) \
with(nvnDeviceGetTexelFetchHandle) \
with(nvnDeviceGetTextureHandle) \
with(nvnDeviceGetTimestampInNanoseconds) \
with(nvnDeviceGetWindowOriginMode) \
with(nvnDeviceInitialize) \
with(nvnDeviceInstallDebugCallback) \
with(nvnDeviceRegisterFastClearColor) \
with(nvnDeviceRegisterFastClearColori) \
with(nvnDeviceRegisterFastClearColorui) \
with(nvnDeviceRegisterFastClearDepth) \
with(nvnDeviceSetDebugLabel) \
with(nvnDeviceSetDepthMode) \
with(nvnDeviceSetIntermediateShaderCache) \
with(nvnDeviceSetWindowOriginMode) \
with(nvnDeviceWaitForError) \
with(nvnMemoryPoolBuilderGetFlags) \
with(nvnMemoryPoolBuilderGetMemory) \
with(nvnMemoryPoolBuilderGetSize) \
with(nvnMemoryPoolBuilderSetDefaults) \
with(nvnMemoryPoolBuilderGetDevice) \
with(nvnMemoryPoolBuilderSetDevice) \
with(nvnMemoryPoolBuilderSetFlags) \
with(nvnMemoryPoolBuilderSetStorage) \
with(nvnMemoryPoolFinalize) \
with(nvnMemoryPoolFlushMappedRange) \
with(nvnMemoryPoolGetBufferAddress) \
with(nvnMemoryPoolGetFlags) \
with(nvnMemoryPoolGetSize) \
with(nvnMemoryPoolInitialize) \
with(nvnMemoryPoolInvalidateMappedRange) \
with(nvnMemoryPoolMap) \
with(nvnMemoryPoolMapVirtual) \
with(nvnMemoryPoolSetDebugLabel) \
with(nvnMultisampleStateGetAlphaToCoverageDither) \
with(nvnMultisampleStateGetAlphaToCoverageEnable) \
with(nvnMultisampleStateGetCoverageModulationMode) \
with(nvnMultisampleStateGetCoverageToColorEnable) \
with(nvnMultisampleStateGetCoverageToColorOutput) \
with(nvnMultisampleStateGetMultisampleEnable) \
with(nvnMultisampleStateGetRasterSamples) \
with(nvnMultisampleStateGetSampleLocationsEnable) \
with(nvnMultisampleStateGetSampleLocationsGrid) \
with(nvnMultisampleStateGetSampleLocationsGridEnable) \
with(nvnMultisampleStateGetSamples) \
with(nvnMultisampleStateSetAlphaToCoverageDither) \
with(nvnMultisampleStateSetAlphaToCoverageEnable) \
with(nvnMultisampleStateSetCoverageModulationMode) \
with(nvnMultisampleStateSetCoverageToColorEnable) \
with(nvnMultisampleStateSetCoverageToColorOutput) \
with(nvnMultisampleStateSetDefaults) \
with(nvnMultisampleStateSetMultisampleEnable) \
with(nvnMultisampleStateSetRasterSamples) \
with(nvnMultisampleStateSetSampleLocations) \
with(nvnMultisampleStateSetSampleLocationsEnable) \
with(nvnMultisampleStateSetSampleLocationsGridEnable) \
with(nvnMultisampleStateSetSamples) \
with(nvnPolygonStateGetCullFace) \
with(nvnPolygonStateGetFrontFace) \
with(nvnPolygonStateGetPolygonMode) \
with(nvnPolygonStateGetPolygonOffsetEnables) \
with(nvnPolygonStateSetCullFace) \
with(nvnPolygonStateSetDefaults) \
with(nvnPolygonStateSetFrontFace) \
with(nvnPolygonStateSetPolygonMode) \
with(nvnPolygonStateSetPolygonOffsetEnables) \
with(nvnProgramFinalize) \
with(nvnProgramInitialize) \
with(nvnProgramSetDebugLabel) \
with(nvnProgramSetSampleShading) \
with(nvnProgramSetShaders) \
with(nvnProgramSetShadersExt) \
with(nvnProgramSetSubroutineLinkage) \
with(nvnQueueAcquireTexture) \
with(nvnQueueBuilderGetQueueMemorySize) \
with(nvnQueueBuilderGetCommandFlushThreshold) \
with(nvnQueueBuilderSetCommandFlushThreshold) \
with(nvnQueueBuilderGetCommandMemorySize) \
with(nvnQueueBuilderSetCommandMemorySize) \
with(nvnQueueBuilderGetComputeMemorySize) \
with(nvnQueueBuilderSetComputeMemorySize) \
with(nvnQueueBuilderGetControlMemorySize) \
with(nvnQueueBuilderSetControlMemorySize) \
with(nvnQueueBuilderSetDefaults) \
with(nvnQueueBuilderGetMemory) \
with(nvnQueueBuilderGetDevice) \
with(nvnQueueBuilderSetDevice) \
with(nvnQueueBuilderGetFlags) \
with(nvnQueueBuilderSetFlags) \
with(nvnQueueBuilderSetQueueMemory) \
with(nvnQueueBuilderGetMemorySize) \
with(nvnQueueBuilderGetQueuePriority) \
with(nvnQueueBuilderSetQueuePriority) \
with(nvnQueueFenceSync) \
with(nvnQueueFinalize) \
with(nvnQueueFinish) \
with(nvnQueueFlush) \
with(nvnQueueGetError) \
with(nvnQueueGetTotalCommandMemoryUsed) \
with(nvnQueueGetTotalComputeMemoryUsed) \
with(nvnQueueGetTotalControlMemoryUsed) \
with(nvnQueueInitialize) \
with(nvnQueuePresentTexture) \
with(nvnQueueResetMemoryUsageCounts) \
with(nvnQueueSetDebugLabel) \
with(nvnQueueSubmitCommands) \
with(nvnQueueWaitSync) \
with(nvnSamplerBuilderGetBorderColor) \
with(nvnSamplerBuilderGetBorderColori) \
with(nvnSamplerBuilderGetBorderColorui) \
with(nvnSamplerBuilderGetCompare) \
with(nvnSamplerBuilderGetLodBias) \
with(nvnSamplerBuilderGetLodClamp) \
with(nvnSamplerBuilderGetMaxAnisotropy) \
with(nvnSamplerBuilderGetMinMagFilter) \
with(nvnSamplerBuilderGetReductionFilter) \
with(nvnSamplerBuilderGetWrapMode) \
with(nvnSamplerBuilderSetBorderColor) \
with(nvnSamplerBuilderSetBorderColori) \
with(nvnSamplerBuilderSetBorderColorui) \
with(nvnSamplerBuilderSetCompare) \
with(nvnSamplerBuilderSetDefaults) \
with(nvnSamplerBuilderGetDevice) \
with(nvnSamplerBuilderSetDevice) \
with(nvnSamplerBuilderSetLodBias) \
with(nvnSamplerBuilderSetLodClamp) \
with(nvnSamplerBuilderSetMaxAnisotropy) \
with(nvnSamplerBuilderSetMinMagFilter) \
with(nvnSamplerBuilderSetReductionFilter) \
with(nvnSamplerBuilderGetLodSnap) \
with(nvnSamplerBuilderSetLodSnap) \
with(nvnSamplerBuilderSetWrapMode) \
with(nvnSamplerCompare) \
with(nvnSamplerFinalize) \
with(nvnSamplerGetBorderColor) \
with(nvnSamplerGetBorderColori) \
with(nvnSamplerGetBorderColorui) \
with(nvnSamplerGetCompare) \
with(nvnSamplerGetDebugID) \
with(nvnSamplerGetLodBias) \
with(nvnSamplerGetLodClamp) \
with(nvnSamplerGetMaxAnisotropy) \
with(nvnSamplerGetMinMagFilter) \
with(nvnSamplerGetReductionFilter) \
with(nvnSamplerGetWrapMode) \
with(nvnSamplerInitialize) \
with(nvnSamplerPoolFinalize) \
with(nvnSamplerPoolGetMemoryOffset) \
with(nvnSamplerPoolGetMemoryPool) \
with(nvnSamplerPoolGetSize) \
with(nvnSamplerPoolInitialize) \
with(nvnSamplerPoolRegisterSampler) \
with(nvnSamplerPoolRegisterSamplerBuilder) \
with(nvnSamplerPoolSetDebugLabel) \
with(nvnSamplerSetDebugLabel) \
with(nvnSyncFinalize) \
with(nvnSyncInitialize) \
with(nvnSyncInitializeFromFencedGLSync) \
with(nvnSyncCreateGLSync) \
with(nvnSyncSetDebugLabel) \
with(nvnSyncWait) \
with(nvnTextureBuilderGetDepth) \
with(nvnTextureBuilderGetDepthStencilMode) \
with(nvnTextureBuilderGetFlags) \
with(nvnTextureBuilderGetFormat) \
with(nvnTextureBuilderGetGLTextureName) \
with(nvnTextureBuilderGetHeight) \
with(nvnTextureBuilderGetLevels) \
with(nvnTextureBuilderGetMemoryOffset) \
with(nvnTextureBuilderGetRawStorageClass) \
with(nvnTextureBuilderGetMemoryPool) \
with(nvnTextureBuilderGetPackagedTextureData) \
with(nvnTextureBuilderGetPackagedTextureLayout) \
with(nvnTextureBuilderGetSamples) \
with(nvnTextureBuilderGetSparseTileLayout) \
with(nvnTextureBuilderGetStorageAlignment) \
with(nvnTextureBuilderGetStorageClass) \
with(nvnTextureBuilderGetStorageSize) \
with(nvnTextureBuilderGetStride) \
with(nvnTextureBuilderGetSwizzle) \
with(nvnTextureBuilderGetTarget) \
with(nvnTextureBuilderGetWidth) \
with(nvnTextureBuilderGetZCullStorageSize) \
with(nvnTextureBuilderSetDefaults) \
with(nvnTextureBuilderSetDepth) \
with(nvnTextureBuilderSetDepthStencilMode) \
with(nvnTextureBuilderGetDevice) \
with(nvnTextureBuilderSetDevice) \
with(nvnTextureBuilderSetFlags) \
with(nvnTextureBuilderSetFormat) \
with(nvnTextureBuilderSetGLTextureName) \
with(nvnTextureBuilderSetHeight) \
with(nvnTextureBuilderSetLevels) \
with(nvnTextureBuilderSetPackagedTextureData) \
with(nvnTextureBuilderSetPackagedTextureLayout) \
with(nvnTextureBuilderSetSamples) \
with(nvnTextureBuilderSetSize1D) \
with(nvnTextureBuilderSetSize2D) \
with(nvnTextureBuilderSetSize3D) \
with(nvnTextureBuilderSetStorage) \
with(nvnTextureBuilderSetStride) \
with(nvnTextureBuilderSetSwizzle) \
with(nvnTextureBuilderSetTarget) \
with(nvnTextureBuilderSetWidth) \
with(nvnTextureCompare) \
with(nvnTextureFinalize) \
with(nvnTextureFlushTexels) \
with(nvnTextureGetDebugID) \
with(nvnTextureGetDepth) \
with(nvnTextureGetDepthStencilMode) \
with(nvnTextureGetFlags) \
with(nvnTextureGetFormat) \
with(nvnTextureGetHeight) \
with(nvnTextureGetLevels) \
with(nvnTextureGetMemoryOffset) \
with(nvnTextureGetMemoryPool) \
with(nvnTextureGetSamples) \
with(nvnTextureGetSparseTileLayout) \
with(nvnTextureGetStorageClass) \
with(nvnTextureGetStorageSize) \
with(nvnTextureGetRawStorageClass) \
with(nvnTextureGetStride) \
with(nvnTextureGetSwizzle) \
with(nvnTextureGetTarget) \
with(nvnTextureGetTextureAddress) \
with(nvnTextureGetViewOffset) \
with(nvnTextureGetWidth) \
with(nvnTextureGetZCullStorageSize) \
with(nvnTextureInitialize) \
with(nvnTextureInvalidateTexels) \
with(nvnTexturePoolFinalize) \
with(nvnTexturePoolGetMemoryOffset) \
with(nvnTexturePoolGetMemoryPool) \
with(nvnTexturePoolGetSize) \
with(nvnTexturePoolInitialize) \
with(nvnTexturePoolRegisterImage) \
with(nvnTexturePoolRegisterTexture) \
with(nvnTexturePoolSetDebugLabel) \
with(nvnTextureReadTexels) \
with(nvnTextureReadTexelsStrided) \
with(nvnTextureSetDebugLabel) \
with(nvnTextureViewCompare) \
with(nvnTextureViewGetDepthStencilMode) \
with(nvnTextureViewGetFormat) \
with(nvnTextureViewGetLayers) \
with(nvnTextureViewGetLevels) \
with(nvnTextureViewGetSwizzle) \
with(nvnTextureViewGetTarget) \
with(nvnTextureViewSetDefaults) \
with(nvnTextureViewSetDepthStencilMode) \
with(nvnTextureViewSetFormat) \
with(nvnTextureViewSetLayers) \
with(nvnTextureViewSetLevels) \
with(nvnTextureViewSetSwizzle) \
with(nvnTextureViewSetTarget) \
with(nvnTextureWriteTexels) \
with(nvnTextureWriteTexelsStrided) \
with(nvnVertexAttribStateGetFormat) \
with(nvnVertexAttribStateGetStreamIndex) \
with(nvnVertexAttribStateSetDefaults) \
with(nvnVertexAttribStateSetFormat) \
with(nvnVertexAttribStateSetStreamIndex) \
with(nvnVertexStreamStateGetDivisor) \
with(nvnVertexStreamStateGetStride) \
with(nvnVertexStreamStateSetDefaults) \
with(nvnVertexStreamStateSetDivisor) \
with(nvnVertexStreamStateSetStride) \
with(nvnWindowAcquireTexture) \
with(nvnWindowBuilderGetNativeWindow) \
with(nvnWindowBuilderGetPresentInterval) \
with(nvnWindowBuilderSetDefaults) \
with(nvnWindowBuilderGetDevice) \
with(nvnWindowBuilderSetDevice) \
with(nvnWindowBuilderSetNativeWindow) \
with(nvnWindowBuilderSetPresentInterval) \
with(nvnWindowBuilderGetNumTextures) \
with(nvnWindowBuilderGetTexture) \
with(nvnWindowBuilderSetTextures) \
with(nvnWindowBuilderGetNumActiveTextures) \
with(nvnWindowBuilderSetNumActiveTextures) \
with(nvnWindowFinalize) \
with(nvnWindowGetNativeWindow) \
with(nvnWindowGetPresentInterval) \
with(nvnWindowInitialize) \
with(nvnWindowSetCrop) \
with(nvnWindowGetCrop) \
with(nvnWindowGetNumTextures) \
with(nvnWindowGetNumActiveTextures) \
with(nvnWindowSetNumActiveTextures) \
with(nvnWindowSetDebugLabel) \
with(nvnWindowSetPresentInterval) \
with(nvnEventBuilderSetDefaults) \
with(nvnEventBuilderGetMemoryPool) \
with(nvnEventBuilderGetMemoryOffset) \
with(nvnEventBuilderGetStorage) \
with(nvnEventBuilderSetStorage) \
with(nvnEventInitialize) \
with(nvnEventGetMemoryPool) \
with(nvnEventGetMemoryOffset) \
with(nvnEventFinalize) \
with(nvnEventGetValue) \
with(nvnEventSignal) \
with(nvnCommandBufferWaitEvent) \
with(nvnCommandBufferSignalEvent)

#endif //NATIVELIB_NVN_H