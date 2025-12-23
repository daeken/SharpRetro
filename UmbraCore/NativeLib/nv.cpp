#include "nv.h"
#include <iostream>
#include <cstring>

#define NVN_FUNC(fname) if(strcmp(name, #fname) == 0) return reinterpret_cast<void*>((void*)fname);

// Forward declarations for NVN types (opaque types)
struct NVNdeviceBuilder {};
struct NVNdevice {};
struct NVNqueue {};
struct NVNqueueBuilder {};
struct NVNwindow {};
struct NVNwindowBuilder {};
struct NVNprogram {};
struct NVNmemoryPool {};
struct NVNmemoryPoolBuilder {};
struct NVNtexturePool {};
struct NVNsamplerPool {};
struct NVNbuffer {};
struct NVNbufferBuilder {};
struct NVNtexture {};
struct NVNtextureBuilder {};
struct NVNtextureView {};
struct NVNsampler {};
struct NVNsamplerBuilder {};
struct NVNblendState {};
struct NVNcolorState {};
struct NVNchannelMaskState {};
struct NVNmultisampleState {};
struct NVNpolygonState {};
struct NVNdepthStencilState {};
struct NVNvertexAttribState {};
struct NVNvertexStreamState {};
struct NVNcommandBuffer {};
struct NVNsync {};
struct NVNevent {};
struct NVNeventBuilder {};
struct NVNshaderData {};
struct NVNmappingRequest {};
struct NVNcopyRegion {};
struct NVNpackagedTextureLayout {};
struct NVNtextureSparseTileLayout {};
struct NVNbufferRange {};
struct NVNrectangle {};
struct NVNcounterData {};
struct NVNqueueErrorInfo {};
struct NVNsubroutineLinkageMapPtr {};
struct NVNdrawTextureRegion {};

// Enum types
typedef int NVNboolean;
typedef int NVNdeviceInfo;
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

void nv::InitializeGraphics(void*, uint64_t) {
    std::cout << "nv::InitializeGraphics called!" << std::endl;
}

void nv::SetGraphicsAllocator(void*, void*, void*, void*) {
    std::cout << "nv::SetGraphicsAllocator called!" << std::endl;
}

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

void nvnDeviceGetInteger(const NVNdevice* device, NVNdeviceInfo pname, int* v) {
    std::cout << "nvnDeviceGetInteger(" << pname << ") called!" << std::endl;
    if (v) *v = 0;
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
    return 0;
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

// Queue functions
NVNqueueGetErrorResult nvnQueueGetError(NVNqueue* queue, NVNqueueErrorInfo* info) {
    std::cout << "nvnQueueGetError(info=" << std::hex << reinterpret_cast<uint64_t>(info) << std::dec << ") called!" << std::endl;
    return 0;
}

size_t nvnQueueGetTotalCommandMemoryUsed(NVNqueue* queue) {
    std::cout << "nvnQueueGetTotalCommandMemoryUsed() called!" << std::endl;
    return 0;
}

size_t nvnQueueGetTotalControlMemoryUsed(NVNqueue* queue) {
    std::cout << "nvnQueueGetTotalControlMemoryUsed() called!" << std::endl;
    return 0;
}

size_t nvnQueueGetTotalComputeMemoryUsed(NVNqueue* queue) {
    std::cout << "nvnQueueGetTotalComputeMemoryUsed() called!" << std::endl;
    return 0;
}

void nvnQueueResetMemoryUsageCounts(NVNqueue* queue) {
    std::cout << "nvnQueueResetMemoryUsageCounts called!" << std::endl;
}

void nvnQueueBuilderSetDevice(NVNqueueBuilder* builder, NVNdevice* device) {
    std::cout << "nvnQueueBuilderSetDevice called!" << std::endl;
}

void nvnQueueBuilderSetDefaults(NVNqueueBuilder* builder) {
    std::cout << "nvnQueueBuilderSetDefaults called!" << std::endl;
}

void nvnQueueBuilderSetFlags(NVNqueueBuilder* builder, int flags) {
    std::cout << "nvnQueueBuilderSetFlags(flags=" << flags << ") called!" << std::endl;
}

void nvnQueueBuilderSetCommandMemorySize(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetCommandMemorySize(size=" << size << ") called!" << std::endl;
}

void nvnQueueBuilderSetComputeMemorySize(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetComputeMemorySize(size=" << size << ") called!" << std::endl;
}

void nvnQueueBuilderSetControlMemorySize(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetControlMemorySize(size=" << size << ") called!" << std::endl;
}

size_t nvnQueueBuilderGetQueueMemorySize(const NVNqueueBuilder* builder) {
    std::cout << "nvnQueueBuilderGetQueueMemorySize() called!" << std::endl;
    return 0;
}

void nvnQueueBuilderSetQueueMemory(NVNqueueBuilder* builder, void* memory, size_t size) {
    std::cout << "nvnQueueBuilderSetQueueMemory(memory=" << std::hex << reinterpret_cast<uint64_t>(memory) << ", size=" << std::dec << size << ") called!" << std::endl;
}

void nvnQueueBuilderSetCommandFlushThreshold(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetCommandFlushThreshold(size=" << size << ") called!" << std::endl;
}

NVNboolean nvnQueueInitialize(NVNqueue* queue, const NVNqueueBuilder* builder) {
    std::cout << "nvnQueueInitialize called!" << std::endl;
    return 1;
}

void nvnQueueFinalize(NVNqueue* queue) {
    std::cout << "nvnQueueFinalize called!" << std::endl;
}

void nvnQueueSetDebugLabel(NVNqueue* queue, const char* label) {
    std::cout << "nvnQueueSetDebugLabel called!" << std::endl;
}

void nvnQueueSubmitCommands(NVNqueue* queue, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnQueueSubmitCommands(numCommands=" << numCommands << ", handles=" << std::hex << reinterpret_cast<uint64_t>(handles) << std::dec << ") called!" << std::endl;
}

void nvnQueueFlush(NVNqueue* queue) {
    std::cout << "nvnQueueFlush called!" << std::endl;
}

void nvnQueueFinish(NVNqueue* queue) {
    std::cout << "nvnQueueFinish called!" << std::endl;
}

void nvnQueuePresentTexture(NVNqueue* queue, NVNwindow* window, int textureIndex) {
    std::cout << "nvnQueuePresentTexture(window=" << std::hex << reinterpret_cast<uint64_t>(window) << ", textureIndex=" << std::dec << textureIndex << ") called!" << std::endl;
}

NVNqueueAcquireTextureResult nvnQueueAcquireTexture(NVNqueue* queue, NVNwindow* window, int* textureIndex) {
    std::cout << "nvnQueueAcquireTexture(window=" << std::hex << reinterpret_cast<uint64_t>(window) << std::dec << ") called!" << std::endl;
    if (textureIndex) *textureIndex = 0;
    return 0;
}

// Window functions
void nvnWindowBuilderSetDevice(NVNwindowBuilder* builder, NVNdevice* device) {
    std::cout << "nvnWindowBuilderSetDevice called!" << std::endl;
}

void nvnWindowBuilderSetDefaults(NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderSetDefaults called!" << std::endl;
}

void nvnWindowBuilderSetNativeWindow(NVNwindowBuilder* builder, NVNnativeWindow nativeWindow) {
    std::cout << "nvnWindowBuilderSetNativeWindow called!" << std::endl;
}

void nvnWindowBuilderSetTextures(NVNwindowBuilder* builder, int numTextures, NVNtexture* const* textures) {
    std::cout << "nvnWindowBuilderSetTextures(numTextures=" << numTextures << ", textures=" << std::hex << reinterpret_cast<uint64_t>(textures) << std::dec << ") called!" << std::endl;
}

void nvnWindowBuilderSetPresentInterval(NVNwindowBuilder* builder, int presentInterval) {
    std::cout << "nvnWindowBuilderSetPresentInterval(presentInterval=" << presentInterval << ") called!" << std::endl;
}

NVNnativeWindow nvnWindowBuilderGetNativeWindow(const NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderGetNativeWindow() called!" << std::endl;
    return 0;
}

int nvnWindowBuilderGetPresentInterval(const NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderGetPresentInterval() called!" << std::endl;
    return 0;
}

NVNboolean nvnWindowInitialize(NVNwindow* window, const NVNwindowBuilder* builder) {
    std::cout << "nvnWindowInitialize called!" << std::endl;
    return 1;
}

void nvnWindowFinalize(NVNwindow* window) {
    std::cout << "nvnWindowFinalize called!" << std::endl;
}

void nvnWindowSetDebugLabel(NVNwindow* window, const char* label) {
    std::cout << "nvnWindowSetDebugLabel called!" << std::endl;
}

NVNwindowAcquireTextureResult nvnWindowAcquireTexture(NVNwindow* window, NVNsync* textureAvailableSync, int* textureIndex) {
    std::cout << "nvnWindowAcquireTexture(textureAvailableSync=" << std::hex << reinterpret_cast<uint64_t>(textureAvailableSync) << std::dec << ") called!" << std::endl;
    if (textureIndex) *textureIndex = 0;
    return 0;
}

NVNnativeWindow nvnWindowGetNativeWindow(const NVNwindow* window) {
    std::cout << "nvnWindowGetNativeWindow() called!" << std::endl;
    return 0;
}

int nvnWindowGetPresentInterval(const NVNwindow* window) {
    std::cout << "nvnWindowGetPresentInterval() called!" << std::endl;
    return 0;
}

void nvnWindowSetPresentInterval(NVNwindow* window, int presentInterval) {
    std::cout << "nvnWindowSetPresentInterval(presentInterval=" << presentInterval << ") called!" << std::endl;
}

void nvnWindowSetCrop(NVNwindow* window, int x, int y, int w, int h) {
    std::cout << "nvnWindowSetCrop(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
}

void nvnWindowGetCrop(const NVNwindow* window, NVNrectangle* rectangle) {
    std::cout << "nvnWindowGetCrop called!" << std::endl;
}

// Program functions
NVNboolean nvnProgramInitialize(NVNprogram* program, NVNdevice* device) {
    std::cout << "nvnProgramInitialize called!" << std::endl;
    return 1;
}

void nvnProgramFinalize(NVNprogram* program) {
    std::cout << "nvnProgramFinalize called!" << std::endl;
}

void nvnProgramSetDebugLabel(NVNprogram* program, const char* label) {
    std::cout << "nvnProgramSetDebugLabel called!" << std::endl;
}

NVNboolean nvnProgramSetShaders(NVNprogram* program, int count, const NVNshaderData* stageData) {
    std::cout << "nvnProgramSetShaders called!" << std::endl;
    return 1;
}

// Memory Pool functions
void nvnMemoryPoolBuilderSetDevice(NVNmemoryPoolBuilder* builder, NVNdevice* device) {
    std::cout << "nvnMemoryPoolBuilderSetDevice called!" << std::endl;
}

void nvnMemoryPoolBuilderSetDefaults(NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderSetDefaults called!" << std::endl;
}

void nvnMemoryPoolBuilderSetStorage(NVNmemoryPoolBuilder* builder, void* memory, size_t size) {
    std::cout << "nvnMemoryPoolBuilderSetStorage called! size: " << size << std::endl;
}

void nvnMemoryPoolBuilderSetFlags(NVNmemoryPoolBuilder* builder, int flags) {
    std::cout << "nvnMemoryPoolBuilderSetFlags (" << flags << ") called!" << std::endl;
}

void nvnMemoryPoolBuilderGetMemory(const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderGetMemory called!" << std::endl;
}

size_t nvnMemoryPoolBuilderGetSize(const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderGetSize() called!" << std::endl;
    return 0;
}

NVNmemoryPoolFlags nvnMemoryPoolBuilderGetFlags(const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderGetFlags() called!" << std::endl;
    return 0;
}

NVNboolean nvnMemoryPoolInitialize(NVNmemoryPool* pool, const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolInitialize called!" << std::endl;
    return 1;
}

void nvnMemoryPoolSetDebugLabel(NVNmemoryPool* pool, const char* label) {
    std::cout << "nvnMemoryPoolSetDebugLabel called!" << std::endl;
}

void nvnMemoryPoolFinalize(NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolFinalize called!" << std::endl;
}

void* nvnMemoryPoolMap(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolMap() called!" << std::endl;
    return nullptr;
}

void nvnMemoryPoolFlushMappedRange(const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnMemoryPoolFlushMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

void nvnMemoryPoolInvalidateMappedRange(const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnMemoryPoolInvalidateMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

NVNbufferAddress nvnMemoryPoolGetBufferAddress(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolGetBufferAddress() called!" << std::endl;
    return 0;
}

NVNboolean nvnMemoryPoolMapVirtual(NVNmemoryPool* pool, int numRequests, const NVNmappingRequest* requests) {
    std::cout << "nvnMemoryPoolMapVirtual(numRequests=" << numRequests << ", requests=" << std::hex << reinterpret_cast<uint64_t>(requests) << std::dec << ") called!" << std::endl;
    return 1;
}

size_t nvnMemoryPoolGetSize(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolGetSize() called!" << std::endl;
    return 0;
}

NVNmemoryPoolFlags nvnMemoryPoolGetFlags(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolGetFlags() called!" << std::endl;
    return 0;
}

// Texture Pool functions
NVNboolean nvnTexturePoolInitialize(NVNtexturePool* texturePool, const NVNmemoryPool* memoryPool, ptrdiff_t offset, int numDescriptors) {
    std::cout << "nvnTexturePoolInitialize(memoryPool=" << std::hex << reinterpret_cast<uint64_t>(memoryPool) << ", offset=" << std::dec << offset << ", numDescriptors=" << numDescriptors << ") called!" << std::endl;
    return 1;
}

void nvnTexturePoolSetDebugLabel(NVNtexturePool* pool, const char* label) {
    std::cout << "nvnTexturePoolSetDebugLabel called!" << std::endl;
}

void nvnTexturePoolFinalize(NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolFinalize called!" << std::endl;
}

void nvnTexturePoolRegisterTexture(const NVNtexturePool* pool, int id, const NVNtexture* texture, const NVNtextureView* view) {
    std::cout << "nvnTexturePoolRegisterTexture(id=" << id << ", texture=" << std::hex << reinterpret_cast<uint64_t>(texture) << ", view=" << reinterpret_cast<uint64_t>(view) << std::dec << ") called!" << std::endl;
}

void nvnTexturePoolRegisterImage(const NVNtexturePool* pool, int id, const NVNtexture* texture, const NVNtextureView* view) {
    std::cout << "nvnTexturePoolRegisterImage(id=" << id << ", texture=" << std::hex << reinterpret_cast<uint64_t>(texture) << ", view=" << reinterpret_cast<uint64_t>(view) << std::dec << ") called!" << std::endl;
}

const NVNmemoryPool* nvnTexturePoolGetMemoryPool(const NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolGetMemoryPool() called!" << std::endl;
    return nullptr;
}

ptrdiff_t nvnTexturePoolGetMemoryOffset(const NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolGetMemoryOffset() called!" << std::endl;
    return 0;
}

int nvnTexturePoolGetSize(const NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolGetSize() called!" << std::endl;
    return 0;
}

// Sampler Pool functions
NVNboolean nvnSamplerPoolInitialize(NVNsamplerPool* samplerPool, const NVNmemoryPool* memoryPool, ptrdiff_t offset, int numDescriptors) {
    std::cout << "nvnSamplerPoolInitialize(memoryPool=" << std::hex << reinterpret_cast<uint64_t>(memoryPool) << ", offset=" << std::dec << offset << ", numDescriptors=" << numDescriptors << ") called!" << std::endl;
    return 1;
}

void nvnSamplerPoolSetDebugLabel(NVNsamplerPool* pool, const char* label) {
    std::cout << "nvnSamplerPoolSetDebugLabel called!" << std::endl;
}

void nvnSamplerPoolFinalize(NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolFinalize called!" << std::endl;
}

void nvnSamplerPoolRegisterSampler(const NVNsamplerPool* pool, int id, const NVNsampler* sampler) {
    std::cout << "nvnSamplerPoolRegisterSampler(id=" << id << ", sampler=" << std::hex << reinterpret_cast<uint64_t>(sampler) << std::dec << ") called!" << std::endl;
}

void nvnSamplerPoolRegisterSamplerBuilder(const NVNsamplerPool* pool, int id, const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerPoolRegisterSamplerBuilder(id=" << id << ", builder=" << std::hex << reinterpret_cast<uint64_t>(builder) << std::dec << ") called!" << std::endl;
}

const NVNmemoryPool* nvnSamplerPoolGetMemoryPool(const NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolGetMemoryPool() called!" << std::endl;
    return nullptr;
}

ptrdiff_t nvnSamplerPoolGetMemoryOffset(const NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolGetMemoryOffset() called!" << std::endl;
    return 0;
}

int nvnSamplerPoolGetSize(const NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolGetSize() called!" << std::endl;
    return 0;
}

// Buffer functions
void nvnBufferBuilderSetDevice(NVNbufferBuilder* builder, NVNdevice* device) {
    std::cout << "nvnBufferBuilderSetDevice called!" << std::endl;
}

void nvnBufferBuilderSetDefaults(NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderSetDefaults called!" << std::endl;
}

void nvnBufferBuilderSetStorage(NVNbufferBuilder* builder, NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferBuilderSetStorage(pool=" << std::hex << reinterpret_cast<uint64_t>(pool) << ", offset=" << std::dec << offset << ", size=" << size << ") called!" << std::endl;
}

NVNmemoryPool nvnBufferBuilderGetMemoryPool(const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderGetMemoryPool() called!" << std::endl;
    return NVNmemoryPool{};
}

ptrdiff_t nvnBufferBuilderGetMemoryOffset(const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderGetMemoryOffset() called!" << std::endl;
    return 0;
}

size_t nvnBufferBuilderGetSize(const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderGetSize() called!" << std::endl;
    return 0;
}

NVNboolean nvnBufferInitialize(NVNbuffer* buffer, const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferInitialize called!" << std::endl;
    return 1;
}

void nvnBufferSetDebugLabel(NVNbuffer* buffer, const char* label) {
    std::cout << "nvnBufferSetDebugLabel called!" << std::endl;
}

void nvnBufferFinalize(NVNbuffer* buffer) {
    std::cout << "nvnBufferFinalize called!" << std::endl;
}

void* nvnBufferMap(const NVNbuffer* buffer) {
    std::cout << "nvnBufferMap() called!" << std::endl;
    return nullptr;
}

NVNbufferAddress nvnBufferGetAddress(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetAddress() called!" << std::endl;
    return 0;
}

void nvnBufferFlushMappedRange(const NVNbuffer* buffer, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferFlushMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

void nvnBufferInvalidateMappedRange(const NVNbuffer* buffer, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferInvalidateMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

NVNmemoryPool* nvnBufferGetMemoryPool(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetMemoryPool() called!" << std::endl;
    return nullptr;
}

ptrdiff_t nvnBufferGetMemoryOffset(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetMemoryOffset() called!" << std::endl;
    return 0;
}

size_t nvnBufferGetSize(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetSize() called!" << std::endl;
    return 0;
}

uint64_t nvnBufferGetDebugID(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetDebugID() called!" << std::endl;
    return 0;
}

// Texture Builder functions
void nvnTextureBuilderSetDevice(NVNtextureBuilder* builder, NVNdevice* device) {
    std::cout << "nvnTextureBuilderSetDevice called!" << std::endl;
}

void nvnTextureBuilderSetDefaults(NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderSetDefaults called!" << std::endl;
}

void nvnTextureBuilderSetFlags(NVNtextureBuilder* builder, int flags) {
    std::cout << "nvnTextureBuilderSetFlags(flags=" << flags << ") called!" << std::endl;
}

void nvnTextureBuilderSetTarget(NVNtextureBuilder* builder, NVNtextureTarget target) {
    std::cout << "nvnTextureBuilderSetTarget(target=" << target << ") called!" << std::endl;
}

void nvnTextureBuilderSetWidth(NVNtextureBuilder* builder, int width) {
    std::cout << "nvnTextureBuilderSetWidth(width=" << width << ") called!" << std::endl;
}

void nvnTextureBuilderSetHeight(NVNtextureBuilder* builder, int height) {
    std::cout << "nvnTextureBuilderSetHeight(height=" << height << ") called!" << std::endl;
}

void nvnTextureBuilderSetDepth(NVNtextureBuilder* builder, int depth) {
    std::cout << "nvnTextureBuilderSetDepth(depth=" << depth << ") called!" << std::endl;
}

void nvnTextureBuilderSetSize1D(NVNtextureBuilder* builder, int size) {
    std::cout << "nvnTextureBuilderSetSize1D(size=" << size << ") called!" << std::endl;
}

void nvnTextureBuilderSetSize2D(NVNtextureBuilder* builder, int width, int height) {
    std::cout << "nvnTextureBuilderSetSize2D(width=" << width << ", height=" << height << ") called!" << std::endl;
}

void nvnTextureBuilderSetSize3D(NVNtextureBuilder* builder, int width, int height, int depth) {
    std::cout << "nvnTextureBuilderSetSize3D(width=" << width << ", height=" << height << ", depth=" << depth << ") called!" << std::endl;
}

void nvnTextureBuilderSetLevels(NVNtextureBuilder* builder, int numLevels) {
    std::cout << "nvnTextureBuilderSetLevels(numLevels=" << numLevels << ") called!" << std::endl;
}

void nvnTextureBuilderSetFormat(NVNtextureBuilder* builder, NVNformat format) {
    std::cout << "nvnTextureBuilderSetFormat(format=" << format << ") called!" << std::endl;
}

void nvnTextureBuilderSetSamples(NVNtextureBuilder* builder, int samples) {
    std::cout << "nvnTextureBuilderSetSamples(samples=" << samples << ") called!" << std::endl;
}

void nvnTextureBuilderSetSwizzle(NVNtextureBuilder* builder, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a) {
    std::cout << "nvnTextureBuilderSetSwizzle(r=" << r << ", g=" << g << ", b=" << b << ", a=" << a << ") called!" << std::endl;
}

void nvnTextureBuilderSetDepthStencilMode(NVNtextureBuilder* builder, NVNtextureDepthStencilMode mode) {
    std::cout << "nvnTextureBuilderSetDepthStencilMode(mode=" << mode << ") called!" << std::endl;
}

size_t nvnTextureBuilderGetStorageSize(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStorageSize() called!" << std::endl;
    return 0;
}

size_t nvnTextureBuilderGetStorageAlignment(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStorageAlignment() called!" << std::endl;
    return 0;
}

void nvnTextureBuilderSetStorage(NVNtextureBuilder* builder, NVNmemoryPool* pool, ptrdiff_t offset) {
    std::cout << "nvnTextureBuilderSetStorage(pool=" << std::hex << reinterpret_cast<uint64_t>(pool) << ", offset=" << std::dec << offset << ") called!" << std::endl;
}

void nvnTextureBuilderSetPackagedTextureData(NVNtextureBuilder* builder, const void* data) {
    std::cout << "nvnTextureBuilderSetPackagedTextureData called!" << std::endl;
}

void nvnTextureBuilderSetPackagedTextureLayout(NVNtextureBuilder* builder, const NVNpackagedTextureLayout* layout) {
    std::cout << "nvnTextureBuilderSetPackagedTextureLayout called!" << std::endl;
}

void nvnTextureBuilderSetStride(NVNtextureBuilder* builder, ptrdiff_t stride) {
    std::cout << "nvnTextureBuilderSetStride(stride=" << stride << ") called!" << std::endl;
}

void nvnTextureBuilderSetGLTextureName(NVNtextureBuilder* builder, uint32_t name) {
    std::cout << "nvnTextureBuilderSetGLTextureName(name=" << name << ") called!" << std::endl;
}

NVNstorageClass nvnTextureBuilderGetStorageClass(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStorageClass() called!" << std::endl;
    return 0;
}

NVNtextureFlags nvnTextureBuilderGetFlags(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetFlags() called!" << std::endl;
    return 0;
}

NVNtextureTarget nvnTextureBuilderGetTarget(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetTarget() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetWidth(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetWidth() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetHeight(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetHeight() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetDepth(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetDepth() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetLevels(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetLevels() called!" << std::endl;
    return 0;
}

NVNformat nvnTextureBuilderGetFormat(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetFormat() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetSamples(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetSamples() called!" << std::endl;
    return 0;
}

void nvnTextureBuilderGetSwizzle(const NVNtextureBuilder* builder, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureBuilderGetSwizzle called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

NVNtextureDepthStencilMode nvnTextureBuilderGetDepthStencilMode(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetDepthStencilMode called!" << std::endl;
    return 0;
}

const void* nvnTextureBuilderGetPackagedTextureData(const NVNtextureBuilder* builder) {
    return nullptr;
}

ptrdiff_t nvnTextureBuilderGetStride(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStride called!" << std::endl;
    return 0;
}

void nvnTextureBuilderGetSparseTileLayout(const NVNtextureBuilder* builder, NVNtextureSparseTileLayout* layout) {
    std::cout << "nvnTextureBuilderGetSparseTileLayout called!" << std::endl;
}

uint32_t nvnTextureBuilderGetGLTextureName(const NVNtextureBuilder* builder) {
    return 0;
}

size_t nvnTextureBuilderGetZCullStorageSize(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetZCullStorageSize called!" << std::endl;
    return 0;
}

NVNmemoryPool nvnTextureBuilderGetMemoryPool(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetMemoryPool called!" << std::endl;
    return NVNmemoryPool{};
}

ptrdiff_t nvnTextureBuilderGetMemoryOffset(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetMemoryOffset called!" << std::endl;
    return 0;
}

// Texture View functions
void nvnTextureViewSetDefaults(NVNtextureView* view) {
    std::cout << "nvnTextureViewSetDefaults called!" << std::endl;
}

void nvnTextureViewSetLevels(NVNtextureView* view, int baseLevel, int numLevels) {
    std::cout << "nvnTextureViewSetLevels called!" << std::endl;
}

void nvnTextureViewSetLayers(NVNtextureView* view, int minLayer, int numLayers) {
    std::cout << "nvnTextureViewSetLayers called!" << std::endl;
}

void nvnTextureViewSetFormat(NVNtextureView* view, NVNformat format) {
    std::cout << "nvnTextureViewSetFormat called!" << std::endl;
}

void nvnTextureViewSetSwizzle(NVNtextureView* view, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a) {
    std::cout << "nvnTextureViewSetSwizzle called!" << std::endl;
}

void nvnTextureViewSetDepthStencilMode(NVNtextureView* view, NVNtextureDepthStencilMode mode) {
    std::cout << "nvnTextureViewSetDepthStencilMode called!" << std::endl;
}

void nvnTextureViewSetTarget(NVNtextureView* view, NVNtextureTarget target) {
    std::cout << "nvnTextureViewSetTarget called!" << std::endl;
}

NVNboolean nvnTextureViewGetLevels(const NVNtextureView* view, int* baseLevel, int* numLevels) {
    std::cout << "nvnTextureViewGetLevels called!" << std::endl;
    if (baseLevel) *baseLevel = 0;
    if (numLevels) *numLevels = 0;
    return 0;
}

NVNboolean nvnTextureViewGetLayers(const NVNtextureView* view, int* minLayer, int* numLayers) {
    std::cout << "nvnTextureViewGetLayers called!" << std::endl;
    if (minLayer) *minLayer = 0;
    if (numLayers) *numLayers = 0;
    return 0;
}

NVNboolean nvnTextureViewGetFormat(const NVNtextureView* view, NVNformat* format) {
    std::cout << "nvnTextureViewGetFormat called!" << std::endl;
    if (format) *format = 0;
    return 0;
}

NVNboolean nvnTextureViewGetSwizzle(const NVNtextureView* view, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureViewGetSwizzle called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
    return 0;
}

NVNboolean nvnTextureViewGetDepthStencilMode(const NVNtextureView* view, NVNtextureDepthStencilMode* mode) {
    std::cout << "nvnTextureViewGetDepthStencilMode called!" << std::endl;
    if (mode) *mode = 0;
    return 0;
}

NVNboolean nvnTextureViewGetTarget(const NVNtextureView* view, NVNtextureTarget* target) {
    std::cout << "nvnTextureViewGetTarget called!" << std::endl;
    if (target) *target = 0;
    return 0;
}

NVNboolean nvnTextureViewCompare(const NVNtextureView* view1, const NVNtextureView* view2) {
    std::cout << "nvnTextureViewCompare called!" << std::endl;
    return 0;
}

// Texture functions
NVNboolean nvnTextureInitialize(NVNtexture* texture, const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureInitialize called!" << std::endl;
    return 1;
}

size_t nvnTextureGetZCullStorageSize(const NVNtexture* texture) {
    std::cout << "nvnTextureGetZCullStorageSize called!" << std::endl;
    return 0;
}

void nvnTextureFinalize(NVNtexture* texture) {
    std::cout << "nvnTextureFinalize called!" << std::endl;
}

void nvnTextureSetDebugLabel(NVNtexture* texture, const char* label) {
    std::cout << "nvnTextureSetDebugLabel called!" << std::endl;
}

NVNstorageClass nvnTextureGetStorageClass(const NVNtexture* texture) {
    std::cout << "nvnTextureGetStorageClass called!" << std::endl;
    return 0;
}

ptrdiff_t nvnTextureGetViewOffset(const NVNtexture* texture, const NVNtextureView* view) {
    std::cout << "nvnTextureGetViewOffset called!" << std::endl;
    return 0;
}

NVNtextureFlags nvnTextureGetFlags(const NVNtexture* texture) {
    std::cout << "nvnTextureGetFlags called!" << std::endl;
    return 0;
}

NVNtextureTarget nvnTextureGetTarget(const NVNtexture* texture) {
    std::cout << "nvnTextureGetTarget called!" << std::endl;
    return 0;
}

int nvnTextureGetWidth(const NVNtexture* texture) {
    std::cout << "nvnTextureGetWidth called!" << std::endl;
    return 0;
}

int nvnTextureGetHeight(const NVNtexture* texture) {
    std::cout << "nvnTextureGetHeight called!" << std::endl;
    return 0;
}

int nvnTextureGetDepth(const NVNtexture* texture) {
    std::cout << "nvnTextureGetDepth called!" << std::endl;
    return 0;
}

int nvnTextureGetLevels(const NVNtexture* texture) {
    std::cout << "nvnTextureGetLevels called!" << std::endl;
    return 0;
}

NVNformat nvnTextureGetFormat(const NVNtexture* texture) {
    std::cout << "nvnTextureGetFormat called!" << std::endl;
    return 0;
}

int nvnTextureGetSamples(const NVNtexture* texture) {
    std::cout << "nvnTextureGetSamples called!" << std::endl;
    return 0;
}

void nvnTextureGetSwizzle(const NVNtexture* texture, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureGetSwizzle called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

NVNtextureDepthStencilMode nvnTextureGetDepthStencilMode(const NVNtexture* texture) {
    std::cout << "nvnTextureGetDepthStencilMode called!" << std::endl;
    return 0;
}

ptrdiff_t nvnTextureGetStride(const NVNtexture* texture) {
    std::cout << "nvnTextureGetStride called!" << std::endl;
    return 0;
}

NVNtextureAddress nvnTextureGetTextureAddress(const NVNtexture* texture) {
    std::cout << "nvnTextureGetTextureAddress called!" << std::endl;
    return 0;
}

void nvnTextureGetSparseTileLayout(const NVNtexture* texture, NVNtextureSparseTileLayout* layout) {
    std::cout << "nvnTextureGetSparseTileLayout called!" << std::endl;
}

void nvnTextureWriteTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, const void* p) {
    std::cout << "nvnTextureWriteTexels called!" << std::endl;
}

void nvnTextureWriteTexelsStrided(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, const void* p, ptrdiff_t o1, ptrdiff_t o2) {
    std::cout << "nvnTextureWriteTexelsStrided called!" << std::endl;
}

void nvnTextureReadTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, void* p) {
    std::cout << "nvnTextureReadTexels called!" << std::endl;
}

void nvnTextureReadTexelsStrided(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, void* p, ptrdiff_t o1, ptrdiff_t o2) {
    std::cout << "nvnTextureReadTexelsStrided called!" << std::endl;
}

void nvnTextureFlushTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region) {
    std::cout << "nvnTextureFlushTexels called!" << std::endl;
}

void nvnTextureInvalidateTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region) {
    std::cout << "nvnTextureInvalidateTexels called!" << std::endl;
}

NVNmemoryPool nvnTextureGetMemoryPool(const NVNtexture* texture) {
    std::cout << "nvnTextureGetMemoryPool called!" << std::endl;
    return NVNmemoryPool{};
}

ptrdiff_t nvnTextureGetMemoryOffset(const NVNtexture* texture) {
    std::cout << "nvnTextureGetMemoryOffset called!" << std::endl;
    return 0;
}

int nvnTextureGetStorageSize(const NVNtexture* texture) {
    std::cout << "nvnTextureGetStorageSize called!" << std::endl;
    return 0;
}

NVNboolean nvnTextureCompare(const NVNtexture* texture1, const NVNtexture* texture2) {
    std::cout << "nvnTextureCompare called!" << std::endl;
    return 0;
}

uint64_t nvnTextureGetDebugID(const NVNtexture* texture) {
    std::cout << "nvnTextureGetDebugID called!" << std::endl;
    return 0;
}

// Sampler Builder functions
void nvnSamplerBuilderSetDevice(NVNsamplerBuilder* builder, NVNdevice* device) {
    std::cout << "nvnSamplerBuilderSetDevice called!" << std::endl;
}

void nvnSamplerBuilderSetDefaults(NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderSetDefaults called!" << std::endl;
}

void nvnSamplerBuilderSetMinMagFilter(NVNsamplerBuilder* builder, NVNminFilter min, NVNmagFilter mag) {
    std::cout << "nvnSamplerBuilderSetMinMagFilter called!" << std::endl;
}

void nvnSamplerBuilderSetWrapMode(NVNsamplerBuilder* builder, NVNwrapMode s, NVNwrapMode t, NVNwrapMode r) {
    std::cout << "nvnSamplerBuilderSetWrapMode called!" << std::endl;
}

void nvnSamplerBuilderSetLodClamp(NVNsamplerBuilder* builder, float min, float max) {
    std::cout << "nvnSamplerBuilderSetLodClamp called!" << std::endl;
}

void nvnSamplerBuilderSetLodBias(NVNsamplerBuilder* builder, float bias) {
    std::cout << "nvnSamplerBuilderSetLodBias called!" << std::endl;
}

void nvnSamplerBuilderSetCompare(NVNsamplerBuilder* builder, NVNcompareMode mode, NVNcompareFunc func) {
    std::cout << "nvnSamplerBuilderSetCompare called!" << std::endl;
}

void nvnSamplerBuilderSetBorderColor(NVNsamplerBuilder* builder, const float* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColor called!" << std::endl;
}

void nvnSamplerBuilderSetBorderColori(NVNsamplerBuilder* builder, const int* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColori called!" << std::endl;
}

void nvnSamplerBuilderSetBorderColorui(NVNsamplerBuilder* builder, const uint32_t* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColorui called!" << std::endl;
}

void nvnSamplerBuilderSetMaxAnisotropy(NVNsamplerBuilder* builder, float maxAniso) {
    std::cout << "nvnSamplerBuilderSetMaxAnisotropy called!" << std::endl;
}

void nvnSamplerBuilderSetReductionFilter(NVNsamplerBuilder* builder, NVNsamplerReduction filter) {
    std::cout << "nvnSamplerBuilderSetReductionFilter called!" << std::endl;
}

void nvnSamplerBuilderSetLodSnap(NVNsamplerBuilder* builder, float f) {
    std::cout << "nvnSamplerBuilderSetLodSnap called!" << std::endl;
}

void nvnSamplerBuilderGetMinMagFilter(const NVNsamplerBuilder* builder, NVNminFilter* min, NVNmagFilter* mag) {
    std::cout << "nvnSamplerBuilderGetMinMagFilter called!" << std::endl;
    if (min) *min = 0;
    if (mag) *mag = 0;
}

void nvnSamplerBuilderGetWrapMode(const NVNsamplerBuilder* builder, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r) {
    std::cout << "nvnSamplerBuilderGetWrapMode called!" << std::endl;
    if (s) *s = 0;
    if (t) *t = 0;
    if (r) *r = 0;
}

void nvnSamplerBuilderGetLodClamp(const NVNsamplerBuilder* builder, float* min, float* max) {
    std::cout << "nvnSamplerBuilderGetLodClamp called!" << std::endl;
    if (min) *min = 0.0f;
    if (max) *max = 0.0f;
}

float nvnSamplerBuilderGetLodBias(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetLodBias called!" << std::endl;
    return 0.0f;
}

void nvnSamplerBuilderGetCompare(const NVNsamplerBuilder* builder, NVNcompareMode* mode, NVNcompareFunc* func) {
    std::cout << "nvnSamplerBuilderGetCompare called!" << std::endl;
    if (mode) *mode = 0;
    if (func) *func = 0;
}

void nvnSamplerBuilderGetBorderColor(const NVNsamplerBuilder* builder, float* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColor called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0.0f;
        borderColor[1] = 0.0f;
        borderColor[2] = 0.0f;
        borderColor[3] = 0.0f;
    }
}

void nvnSamplerBuilderGetBorderColori(const NVNsamplerBuilder* builder, int* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColori called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

void nvnSamplerBuilderGetBorderColorui(const NVNsamplerBuilder* builder, uint32_t* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColorui called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

float nvnSamplerBuilderGetMaxAnisotropy(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetMaxAnisotropy called!" << std::endl;
    return 0.0f;
}

NVNsamplerReduction nvnSamplerBuilderGetReductionFilter(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetReductionFilter called!" << std::endl;
    return 0;
}

float nvnSamplerBuilderGetLodSnap(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetLodSnap called!" << std::endl;
    return 0.0f;
}

// Sampler functions
NVNboolean nvnSamplerInitialize(NVNsampler* sampler, const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerInitialize called!" << std::endl;
    return 1;
}

void nvnSamplerFinalize(NVNsampler* sampler) {
    std::cout << "nvnSamplerFinalize called!" << std::endl;
}

void nvnSamplerSetDebugLabel(NVNsampler* sampler, const char* label) {
    std::cout << "nvnSamplerSetDebugLabel called!" << std::endl;
}

void nvnSamplerGetMinMagFilter(const NVNsampler* sampler, NVNminFilter* min, NVNmagFilter* mag) {
    std::cout << "nvnSamplerGetMinMagFilter called!" << std::endl;
    if (min) *min = 0;
    if (mag) *mag = 0;
}

void nvnSamplerGetWrapMode(const NVNsampler* sampler, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r) {
    std::cout << "nvnSamplerGetWrapMode called!" << std::endl;
    if (s) *s = 0;
    if (t) *t = 0;
    if (r) *r = 0;
}

void nvnSamplerGetLodClamp(const NVNsampler* sampler, float* min, float* max) {
    std::cout << "nvnSamplerGetLodClamp called!" << std::endl;
    if (min) *min = 0.0f;
    if (max) *max = 0.0f;
}

float nvnSamplerGetLodBias(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetLodBias called!" << std::endl;
    return 0.0f;
}

void nvnSamplerGetCompare(const NVNsampler* sampler, NVNcompareMode* mode, NVNcompareFunc* func) {
    std::cout << "nvnSamplerGetCompare called!" << std::endl;
    if (mode) *mode = 0;
    if (func) *func = 0;
}

void nvnSamplerGetBorderColor(const NVNsampler* sampler, float* borderColor) {
    std::cout << "nvnSamplerGetBorderColor called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0.0f;
        borderColor[1] = 0.0f;
        borderColor[2] = 0.0f;
        borderColor[3] = 0.0f;
    }
}

void nvnSamplerGetBorderColori(const NVNsampler* sampler, int* borderColor) {
    std::cout << "nvnSamplerGetBorderColori called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

void nvnSamplerGetBorderColorui(const NVNsampler* sampler, uint32_t* borderColor) {
    std::cout << "nvnSamplerGetBorderColorui called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

float nvnSamplerGetMaxAnisotropy(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetMaxAnisotropy called!" << std::endl;
    return 0.0f;
}

NVNsamplerReduction nvnSamplerGetReductionFilter(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetReductionFilter called!" << std::endl;
    return 0;
}

NVNboolean nvnSamplerCompare(const NVNsampler* sampler1, const NVNsampler* sampler2) {
    std::cout << "nvnSamplerCompare called!" << std::endl;
    return 0;
}

uint64_t nvnSamplerGetDebugID(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetDebugID called!" << std::endl;
    return 0;
}

// Blend State functions
void nvnBlendStateSetDefaults(NVNblendState* blend) {
    std::cout << "nvnBlendStateSetDefaults called!" << std::endl;
}

void nvnBlendStateSetBlendTarget(NVNblendState* blend, int target) {
    std::cout << "nvnBlendStateSetBlendTarget(target=" << target << ") called!" << std::endl;
}

void nvnBlendStateSetBlendFunc(NVNblendState* blend, NVNblendFunc srcFunc, NVNblendFunc dstFunc, NVNblendFunc srcFuncAlpha, NVNblendFunc dstFuncAlpha) {
    std::cout << "nvnBlendStateSetBlendFunc(srcFunc=" << srcFunc << ", dstFunc=" << dstFunc << ", srcFuncAlpha=" << srcFuncAlpha << ", dstFuncAlpha=" << dstFuncAlpha << ") called!" << std::endl;
}

void nvnBlendStateSetBlendEquation(NVNblendState* blend, NVNblendEquation modeRGB, NVNblendEquation modeAlpha) {
    std::cout << "nvnBlendStateSetBlendEquation(modeRGB=" << modeRGB << ", modeAlpha=" << modeAlpha << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedMode(NVNblendState* blend, NVNblendAdvancedMode mode) {
    std::cout << "nvnBlendStateSetAdvancedMode(mode=" << mode << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedOverlap(NVNblendState* blend, NVNblendAdvancedOverlap overlap) {
    std::cout << "nvnBlendStateSetAdvancedOverlap(overlap=" << overlap << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedPremultipliedSrc(NVNblendState* blend, NVNboolean b) {
    std::cout << "nvnBlendStateSetAdvancedPremultipliedSrc(b=" << b << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedNormalizedDst(NVNblendState* blend, NVNboolean b) {
    std::cout << "nvnBlendStateSetAdvancedNormalizedDst(b=" << b << ") called!" << std::endl;
}

int nvnBlendStateGetBlendTarget(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetBlendTarget called!" << std::endl;
    return 0;
}

void nvnBlendStateGetBlendFunc(const NVNblendState* blend, NVNblendFunc* srcFunc, NVNblendFunc* dstFunc, NVNblendFunc* srcFuncAlpha, NVNblendFunc* dstFuncAlpha) {
    std::cout << "nvnBlendStateGetBlendFunc called!" << std::endl;
    if (srcFunc) *srcFunc = 0;
    if (dstFunc) *dstFunc = 0;
    if (srcFuncAlpha) *srcFuncAlpha = 0;
    if (dstFuncAlpha) *dstFuncAlpha = 0;
}

void nvnBlendStateGetBlendEquation(const NVNblendState* blend, NVNblendEquation* modeRGB, NVNblendEquation* modeAlpha) {
    std::cout << "nvnBlendStateGetBlendEquation called!" << std::endl;
    if (modeRGB) *modeRGB = 0;
    if (modeAlpha) *modeAlpha = 0;
}

NVNblendAdvancedMode nvnBlendStateGetAdvancedMode(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedMode called!" << std::endl;
    return 0;
}

NVNblendAdvancedOverlap nvnBlendStateGetAdvancedOverlap(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedOverlap called!" << std::endl;
    return 0;
}

NVNboolean nvnBlendStateGetAdvancedPremultipliedSrc(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedPremultipliedSrc called!" << std::endl;
    return 0;
}

NVNboolean nvnBlendStateGetAdvancedNormalizedDst(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedNormalizedDst called!" << std::endl;
    return 0;
}

// Color State functions
void nvnColorStateSetDefaults(NVNcolorState* color) {
    std::cout << "nvnColorStateSetDefaults called!" << std::endl;
}

void nvnColorStateSetBlendEnable(NVNcolorState* color, int index, NVNboolean enable) {
    std::cout << "nvnColorStateSetBlendEnable called!" << std::endl;
}

void nvnColorStateSetLogicOp(NVNcolorState* color, NVNlogicOp logicOp) {
    std::cout << "nvnColorStateSetLogicOp called!" << std::endl;
}

void nvnColorStateSetAlphaTest(NVNcolorState* color, NVNalphaFunc alphaFunc) {
    std::cout << "nvnColorStateSetAlphaTest called!" << std::endl;
}

NVNboolean nvnColorStateGetBlendEnable(const NVNcolorState* color, int index) {
    std::cout << "nvnColorStateGetBlendEnable called!" << std::endl;
    return 0;
}

NVNlogicOp nvnColorStateGetLogicOp(const NVNcolorState* color) {
    std::cout << "nvnColorStateGetLogicOp called!" << std::endl;
    return 0;
}

NVNalphaFunc nvnColorStateGetAlphaTest(const NVNcolorState* color) {
    std::cout << "nvnColorStateGetAlphaTest called!" << std::endl;
    return 0;
}

// Channel Mask State functions
void nvnChannelMaskStateSetDefaults(NVNchannelMaskState* channelMask) {
    std::cout << "nvnChannelMaskStateSetDefaults called!" << std::endl;
}

void nvnChannelMaskStateSetChannelMask(NVNchannelMaskState* channelMask, int index, NVNboolean r, NVNboolean g, NVNboolean b, NVNboolean a) {
    std::cout << "nvnChannelMaskStateSetChannelMask called!" << std::endl;
}

void nvnChannelMaskStateGetChannelMask(const NVNchannelMaskState* channelMask, int index, NVNboolean* r, NVNboolean* g, NVNboolean* b, NVNboolean* a) {
    std::cout << "nvnChannelMaskStateGetChannelMask called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

// Multisample State functions
void nvnMultisampleStateSetDefaults(NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateSetDefaults called!" << std::endl;
}

void nvnMultisampleStateSetMultisampleEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetMultisampleEnable called!" << std::endl;
}

void nvnMultisampleStateSetSamples(NVNmultisampleState* multisample, int samples) {
    std::cout << "nvnMultisampleStateSetSamples called!" << std::endl;
}

void nvnMultisampleStateSetAlphaToCoverageEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetAlphaToCoverageEnable called!" << std::endl;
}

void nvnMultisampleStateSetAlphaToCoverageDither(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetAlphaToCoverageDither called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetMultisampleEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetMultisampleEnable called!" << std::endl;
    return 0;
}

int nvnMultisampleStateGetSamples(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetSamples called!" << std::endl;
    return 0;
}

NVNboolean nvnMultisampleStateGetAlphaToCoverageEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetAlphaToCoverageEnable called!" << std::endl;
    return 0;
}

NVNboolean nvnMultisampleStateGetAlphaToCoverageDither(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetAlphaToCoverageDither called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetRasterSamples(NVNmultisampleState* multisample, int samples) {
    std::cout << "nvnMultisampleStateSetRasterSamples called!" << std::endl;
}

int nvnMultisampleStateGetRasterSamples(NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetRasterSamples called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetCoverageModulationMode(NVNmultisampleState* multisample, NVNcoverageModulationMode mode) {
    std::cout << "nvnMultisampleStateSetCoverageModulationMode called!" << std::endl;
}

NVNcoverageModulationMode nvnMultisampleStateGetCoverageModulationMode(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetCoverageModulationMode called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetCoverageToColorEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetCoverageToColorEnable called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetCoverageToColorEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetCoverageToColorEnable called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetCoverageToColorOutput(NVNmultisampleState* multisample, int i) {
    std::cout << "nvnMultisampleStateSetCoverageToColorOutput called!" << std::endl;
}

int nvnMultisampleStateGetCoverageToColorOutput(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetCoverageToColorOutput called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetSampleLocationsEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetSampleLocationsEnable called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetSampleLocationsEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetSampleLocationsEnable called!" << std::endl;
    return 0;
}

void nvnMultisampleStateGetSampleLocationsGrid(NVNmultisampleState* multisample, int* w, int* h) {
    std::cout << "nvnMultisampleStateGetSampleLocationsGrid called!" << std::endl;
    if (w) *w = 0;
    if (h) *h = 0;
}

void nvnMultisampleStateSetSampleLocationsGridEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetSampleLocationsGridEnable called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetSampleLocationsGridEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetSampleLocationsGridEnable called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetSampleLocations(NVNmultisampleState* multisample, int i1, int i2, const float* f) {
    std::cout << "nvnMultisampleStateSetSampleLocations called!" << std::endl;
}

// Polygon State functions
void nvnPolygonStateSetDefaults(NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateSetDefaults called!" << std::endl;
}

void nvnPolygonStateSetCullFace(NVNpolygonState* polygon, NVNface face) {
    std::cout << "nvnPolygonStateSetCullFace called!" << std::endl;
}

void nvnPolygonStateSetFrontFace(NVNpolygonState* polygon, NVNfrontFace face) {
    std::cout << "nvnPolygonStateSetFrontFace called!" << std::endl;
}

void nvnPolygonStateSetPolygonMode(NVNpolygonState* polygon, NVNpolygonMode polygonMode) {
    std::cout << "nvnPolygonStateSetPolygonMode called!" << std::endl;
}

void nvnPolygonStateSetPolygonOffsetEnables(NVNpolygonState* polygon, int enables) {
    std::cout << "nvnPolygonStateSetPolygonOffsetEnables called!" << std::endl;
}

NVNface nvnPolygonStateGetCullFace(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetCullFace called!" << std::endl;
    return 0;
}

NVNfrontFace nvnPolygonStateGetFrontFace(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetFrontFace called!" << std::endl;
    return 0;
}

NVNpolygonMode nvnPolygonStateGetPolygonMode(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetPolygonMode called!" << std::endl;
    return 0;
}

NVNpolygonOffsetEnable nvnPolygonStateGetPolygonOffsetEnables(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetPolygonOffsetEnables called!" << std::endl;
    return 0;
}

// Depth Stencil State functions
void nvnDepthStencilStateSetDefaults(NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateSetDefaults called!" << std::endl;
}

void nvnDepthStencilStateSetDepthTestEnable(NVNdepthStencilState* depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetDepthTestEnable called!" << std::endl;
}

void nvnDepthStencilStateSetDepthWriteEnable(NVNdepthStencilState* depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetDepthWriteEnable called!" << std::endl;
}

void nvnDepthStencilStateSetDepthFunc(NVNdepthStencilState* depthStencil, NVNdepthFunc func) {
    std::cout << "nvnDepthStencilStateSetDepthFunc called!" << std::endl;
}

void nvnDepthStencilStateSetStencilTestEnable(NVNdepthStencilState* depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetStencilTestEnable called!" << std::endl;
}

void nvnDepthStencilStateSetStencilFunc(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilFunc func) {
    std::cout << "nvnDepthStencilStateSetStencilFunc called!" << std::endl;
}

void nvnDepthStencilStateSetStencilOp(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilOp fail, NVNstencilOp depthFail, NVNstencilOp depthPass) {
    std::cout << "nvnDepthStencilStateSetStencilOp called!" << std::endl;
}

NVNboolean nvnDepthStencilStateGetDepthTestEnable(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthTestEnable called!" << std::endl;
    return 0;
}

NVNboolean nvnDepthStencilStateGetDepthWriteEnable(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthWriteEnable called!" << std::endl;
    return 0;
}

NVNdepthFunc nvnDepthStencilStateGetDepthFunc(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthFunc called!" << std::endl;
    return 0;
}

NVNboolean nvnDepthStencilStateGetStencilTestEnable(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetStencilTestEnable called!" << std::endl;
    return 0;
}

NVNstencilFunc nvnDepthStencilStateGetStencilFunc(const NVNdepthStencilState* depthStencil, NVNface faces) {
    std::cout << "nvnDepthStencilStateGetStencilFunc called!" << std::endl;
    return 0;
}

void nvnDepthStencilStateGetStencilOp(const NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilOp* fail, NVNstencilOp* depthFail, NVNstencilOp* depthPass) {
    std::cout << "nvnDepthStencilStateGetStencilOp called!" << std::endl;
    if (fail) *fail = 0;
    if (depthFail) *depthFail = 0;
    if (depthPass) *depthPass = 0;
}

// Vertex Attrib State functions
void nvnVertexAttribStateSetDefaults(NVNvertexAttribState* attrib) {
    std::cout << "nvnVertexAttribStateSetDefaults called!" << std::endl;
}

void nvnVertexAttribStateSetFormat(NVNvertexAttribState* attrib, NVNformat format, ptrdiff_t relativeOffset) {
    std::cout << "nvnVertexAttribStateSetFormat called!" << std::endl;
}

void nvnVertexAttribStateSetStreamIndex(NVNvertexAttribState* attrib, int streamIndex) {
    std::cout << "nvnVertexAttribStateSetStreamIndex called!" << std::endl;
}

void nvnVertexAttribStateGetFormat(const NVNvertexAttribState* attrib, NVNformat* format, ptrdiff_t* relativeOffset) {
    std::cout << "nvnVertexAttribStateGetFormat called!" << std::endl;
    if (format) *format = 0;
    if (relativeOffset) *relativeOffset = 0;
}

int nvnVertexAttribStateGetStreamIndex(const NVNvertexAttribState* attrib) {
    std::cout << "nvnVertexAttribStateGetStreamIndex called!" << std::endl;
    return 0;
}

// Vertex Stream State functions
void nvnVertexStreamStateSetDefaults(NVNvertexStreamState* stream) {
    std::cout << "nvnVertexStreamStateSetDefaults called!" << std::endl;
}

void nvnVertexStreamStateSetStride(NVNvertexStreamState* stream, ptrdiff_t stride) {
    std::cout << "nvnVertexStreamStateSetStride called!" << std::endl;
}

void nvnVertexStreamStateSetDivisor(NVNvertexStreamState* stream, int divisor) {
    std::cout << "nvnVertexStreamStateSetDivisor called!" << std::endl;
}

ptrdiff_t nvnVertexStreamStateGetStride(const NVNvertexStreamState* stream) {
    std::cout << "nvnVertexStreamStateGetStride called!" << std::endl;
    return 0;
}

int nvnVertexStreamStateGetDivisor(const NVNvertexStreamState* stream) {
    std::cout << "nvnVertexStreamStateGetDivisor called!" << std::endl;
    return 0;
}

// Command Buffer functions
NVNboolean nvnCommandBufferInitialize(NVNcommandBuffer* cmdBuf, NVNdevice* device) {
    std::cout << "nvnCommandBufferInitialize called!" << std::endl;
    return 1;
}

void nvnCommandBufferFinalize(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferFinalize called!" << std::endl;
}

void nvnCommandBufferSetDebugLabel(NVNcommandBuffer* cmdBuf, const char* label) {
    std::cout << "nvnCommandBufferSetDebugLabel called!" << std::endl;
}

void nvnCommandBufferSetMemoryCallback(NVNcommandBuffer* cmdBuf, PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC callback) {
    std::cout << "nvnCommandBufferSetMemoryCallback called!" << std::endl;
}

void nvnCommandBufferSetMemoryCallbackData(NVNcommandBuffer* cmdBuf, void* callbackData) {
    std::cout << "nvnCommandBufferSetMemoryCallbackData called!" << std::endl;
}

void nvnCommandBufferAddCommandMemory(NVNcommandBuffer* cmdBuf, const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnCommandBufferAddCommandMemory called!" << std::endl;
}

void nvnCommandBufferAddControlMemory(NVNcommandBuffer* cmdBuf, void* memory, size_t size) {
    std::cout << "nvnCommandBufferAddControlMemory called!" << std::endl;
}

size_t nvnCommandBufferGetCommandMemorySize(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemorySize called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetCommandMemoryUsed(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemoryUsed called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetCommandMemoryFree(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemoryFree called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetControlMemorySize(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemorySize called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetControlMemoryUsed(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemoryUsed called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetControlMemoryFree(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemoryFree called!" << std::endl;
    return 0;
}

void nvnCommandBufferBeginRecording(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferBeginRecording called!" << std::endl;
}

NVNcommandHandle nvnCommandBufferEndRecording(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferEndRecording called!" << std::endl;
    return 0;
}

void nvnCommandBufferCallCommands(NVNcommandBuffer* cmdBuf, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnCommandBufferCallCommands called!" << std::endl;
}

void nvnCommandBufferCopyCommands(NVNcommandBuffer* cmdBuf, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnCommandBufferCopyCommands called!" << std::endl;
}

void nvnCommandBufferBindBlendState(NVNcommandBuffer* cmdBuf, const NVNblendState* blend) {
    std::cout << "nvnCommandBufferBindBlendState called!" << std::endl;
}

void nvnCommandBufferBindChannelMaskState(NVNcommandBuffer* cmdBuf, const NVNchannelMaskState* channelMask) {
    std::cout << "nvnCommandBufferBindChannelMaskState called!" << std::endl;
}

void nvnCommandBufferBindColorState(NVNcommandBuffer* cmdBuf, const NVNcolorState* color) {
    std::cout << "nvnCommandBufferBindColorState called!" << std::endl;
}

void nvnCommandBufferBindMultisampleState(NVNcommandBuffer* cmdBuf, const NVNmultisampleState* multisample) {
    std::cout << "nvnCommandBufferBindMultisampleState called!" << std::endl;
}

void nvnCommandBufferBindPolygonState(NVNcommandBuffer* cmdBuf, const NVNpolygonState* polygon) {
    std::cout << "nvnCommandBufferBindPolygonState called!" << std::endl;
}

void nvnCommandBufferBindDepthStencilState(NVNcommandBuffer* cmdBuf, const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnCommandBufferBindDepthStencilState called!" << std::endl;
}

void nvnCommandBufferBindVertexAttribState(NVNcommandBuffer* cmdBuf, int numAttribs, const NVNvertexAttribState* attribs) {
    std::cout << "nvnCommandBufferBindVertexAttribState called!" << std::endl;
}

void nvnCommandBufferBindVertexStreamState(NVNcommandBuffer* cmdBuf, int numStreams, const NVNvertexStreamState* streams) {
    std::cout << "nvnCommandBufferBindVertexStreamState called!" << std::endl;
}

void nvnCommandBufferBindProgram(NVNcommandBuffer* cmdBuf, const NVNprogram* program, int stages) {
    std::cout << "nvnCommandBufferBindProgram(program=" << std::hex << reinterpret_cast<uint64_t>(program) << ", stages=" << std::dec << stages << ") called!" << std::endl;
}

void nvnCommandBufferBindVertexBuffer(NVNcommandBuffer* cmdBuf, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindVertexBuffer(index=" << index << ", buffer=" << std::hex << buffer << ", size=" << std::dec << size << ") called!" << std::endl;
}

void nvnCommandBufferBindVertexBuffers(NVNcommandBuffer* cmdBuf, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindVertexBuffers(first=" << first << ", count=" << count << ", buffers=" << std::hex << reinterpret_cast<uint64_t>(buffers) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindUniformBuffer(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindUniformBuffer(stage=" << stage << ", index=" << index << ", buffer=" << std::hex << buffer << ", size=" << std::dec << size << ") called!" << std::endl;
}

void nvnCommandBufferBindUniformBuffers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindUniformBuffers(stage=" << stage << ", first=" << first << ", count=" << count << ", buffers=" << std::hex << reinterpret_cast<uint64_t>(buffers) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindTransformFeedbackBuffer(NVNcommandBuffer* cmdBuf, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindTransformFeedbackBuffer called!" << std::endl;
}

void nvnCommandBufferBindTransformFeedbackBuffers(NVNcommandBuffer* cmdBuf, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindTransformFeedbackBuffers called!" << std::endl;
}

void nvnCommandBufferBindStorageBuffer(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindStorageBuffer called!" << std::endl;
}

void nvnCommandBufferBindStorageBuffers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindStorageBuffers called!" << std::endl;
}

void nvnCommandBufferBindTexture(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNtextureHandle texture) {
    std::cout << "nvnCommandBufferBindTexture(stage=" << stage << ", index=" << index << ", texture=" << std::hex << texture << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindTextures(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNtextureHandle* textures) {
    std::cout << "nvnCommandBufferBindTextures(stage=" << stage << ", first=" << first << ", count=" << count << ", textures=" << std::hex << reinterpret_cast<uint64_t>(textures) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindImage(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNimageHandle image) {
    std::cout << "nvnCommandBufferBindImage(stage=" << stage << ", index=" << index << ", image=" << std::hex << image << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindImages(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNimageHandle* images) {
    std::cout << "nvnCommandBufferBindImages(stage=" << stage << ", first=" << first << ", count=" << count << ", images=" << std::hex << reinterpret_cast<uint64_t>(images) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferSetPatchSize(NVNcommandBuffer* cmdBuf, int i) {
    std::cout << "nvnCommandBufferSetPatchSize called!" << std::endl;
}

void nvnCommandBufferSetInnerTessellationLevels(NVNcommandBuffer* cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferSetInnerTessellationLevels called!" << std::endl;
}

void nvnCommandBufferSetOuterTessellationLevels(NVNcommandBuffer* cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferSetOuterTessellationLevels called!" << std::endl;
}

void nvnCommandBufferSetPrimitiveRestart(NVNcommandBuffer* cmdBuf, NVNboolean b, int i) {
    std::cout << "nvnCommandBufferSetPrimitiveRestart called!" << std::endl;
}

void nvnCommandBufferBeginTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferBeginTransformFeedback called!" << std::endl;
}

void nvnCommandBufferEndTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferEndTransformFeedback called!" << std::endl;
}

void nvnCommandBufferPauseTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferPauseTransformFeedback called!" << std::endl;
}

void nvnCommandBufferResumeTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferResumeTransformFeedback called!" << std::endl;
}

void nvnCommandBufferDrawTransformFeedback(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDrawTransformFeedback called!" << std::endl;
}

void nvnCommandBufferDrawArrays(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, int first, int count) {
    std::cout << "nvnCommandBufferDrawArrays(mode=" << mode << ", first=" << first << ", count=" << count << ") called!" << std::endl;
}

void nvnCommandBufferDrawElements(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer) {
    std::cout << "nvnCommandBufferDrawElements(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferDrawElementsBaseVertex(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex) {
    std::cout << "nvnCommandBufferDrawElementsBaseVertex(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << ", baseVertex=" << std::dec << baseVertex << ") called!" << std::endl;
}

void nvnCommandBufferDrawArraysInstanced(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, int first, int count, int baseInstance, int instanceCount) {
    std::cout << "nvnCommandBufferDrawArraysInstanced(mode=" << mode << ", first=" << first << ", count=" << count << ", baseInstance=" << baseInstance << ", instanceCount=" << instanceCount << ") called!" << std::endl;
}

void nvnCommandBufferDrawElementsInstanced(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex, int baseInstance, int instanceCount) {
    std::cout << "nvnCommandBufferDrawElementsInstanced(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << ", baseVertex=" << std::dec << baseVertex << ", baseInstance=" << baseInstance << ", instanceCount=" << instanceCount << ") called!" << std::endl;
}

void nvnCommandBufferDrawArraysIndirect(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDrawArraysIndirect called!" << std::endl;
}

void nvnCommandBufferDrawElementsIndirect(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2) {
    std::cout << "nvnCommandBufferDrawElementsIndirect called!" << std::endl;
}

void nvnCommandBufferMultiDrawArraysIndirectCount(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer1, NVNbufferAddress buffer2, int i, ptrdiff_t o) {
    std::cout << "nvnCommandBufferMultiDrawArraysIndirectCount called!" << std::endl;
}

void nvnCommandBufferMultiDrawElementsIndirectCount(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2, NVNbufferAddress buffer3, int i, ptrdiff_t o) {
    std::cout << "nvnCommandBufferMultiDrawElementsIndirectCount called!" << std::endl;
}

void nvnCommandBufferClearColor(NVNcommandBuffer* cmdBuf, int index, const float* color, int mask) {
    std::cout << "nvnCommandBufferClearColor called!" << std::endl;
}

void nvnCommandBufferClearColori(NVNcommandBuffer* cmdBuf, int index, const int* color, int mask) {
    std::cout << "nvnCommandBufferClearColori called!" << std::endl;
}

void nvnCommandBufferClearColorui(NVNcommandBuffer* cmdBuf, int index, const uint32_t* color, int mask) {
    std::cout << "nvnCommandBufferClearColorui called!" << std::endl;
}

void nvnCommandBufferClearDepthStencil(NVNcommandBuffer* cmdBuf, float depthValue, NVNboolean depthMask, int stencilValue, int stencilMask) {
    std::cout << "nvnCommandBufferClearDepthStencil called!" << std::endl;
}

void nvnCommandBufferDispatchCompute(NVNcommandBuffer* cmdBuf, int groupsX, int groupsY, int groupsZ) {
    std::cout << "nvnCommandBufferDispatchCompute(groupsX=" << groupsX << ", groupsY=" << groupsY << ", groupsZ=" << groupsZ << ") called!" << std::endl;
}

void nvnCommandBufferDispatchComputeIndirect(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDispatchComputeIndirect called!" << std::endl;
}

void nvnCommandBufferSetViewport(NVNcommandBuffer* cmdBuf, int x, int y, int w, int h) {
    std::cout << "nvnCommandBufferSetViewport(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
}

void nvnCommandBufferSetViewports(NVNcommandBuffer* cmdBuf, int first, int count, const float* ranges) {
    std::cout << "nvnCommandBufferSetViewports(first=" << first << ", count=" << count << ", ranges=" << std::hex << reinterpret_cast<uint64_t>(ranges) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferSetViewportSwizzles(NVNcommandBuffer* cmdBuf, int first, int count, const NVNviewportSwizzle* swizzles) {
    std::cout << "nvnCommandBufferSetViewportSwizzles called!" << std::endl;
}

void nvnCommandBufferSetScissor(NVNcommandBuffer* cmdBuf, int x, int y, int w, int h) {
    std::cout << "nvnCommandBufferSetScissor(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
}

void nvnCommandBufferSetScissors(NVNcommandBuffer* cmdBuf, int first, int count, const int* rects) {
    std::cout << "nvnCommandBufferSetScissors(first=" << first << ", count=" << count << ", rects=" << std::hex << reinterpret_cast<uint64_t>(rects) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferSetDepthRange(NVNcommandBuffer* cmdBuf, float n, float f) {
    std::cout << "nvnCommandBufferSetDepthRange called!" << std::endl;
}

void nvnCommandBufferSetDepthBounds(NVNcommandBuffer* cmdBuf, NVNboolean enable, float n, float f) {
    std::cout << "nvnCommandBufferSetDepthBounds called!" << std::endl;
}

void nvnCommandBufferSetDepthRanges(NVNcommandBuffer* cmdBuf, int first, int count, const float* ranges) {
    std::cout << "nvnCommandBufferSetDepthRanges called!" << std::endl;
}

void nvnCommandBufferSetTiledCacheAction(NVNcommandBuffer* cmdBuf, NVNtiledCacheAction action) {
    std::cout << "nvnCommandBufferSetTiledCacheAction called!" << std::endl;
}

void nvnCommandBufferSetTiledCacheTileSize(NVNcommandBuffer* cmdBuf, int w, int h) {
    std::cout << "nvnCommandBufferSetTiledCacheTileSize called!" << std::endl;
}

void nvnCommandBufferBindSeparateTexture(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i, NVNseparateTextureHandle handle) {
    std::cout << "nvnCommandBufferBindSeparateTexture called!" << std::endl;
}

void nvnCommandBufferBindSeparateSampler(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i, NVNseparateSamplerHandle handle) {
    std::cout << "nvnCommandBufferBindSeparateSampler called!" << std::endl;
}

void nvnCommandBufferBindSeparateTextures(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateTextureHandle* handle) {
    std::cout << "nvnCommandBufferBindSeparateTextures called!" << std::endl;
}

void nvnCommandBufferBindSeparateSamplers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateSamplerHandle* handle) {
    std::cout << "nvnCommandBufferBindSeparateSamplers called!" << std::endl;
}

void nvnCommandBufferSetStencilValueMask(NVNcommandBuffer* cmdBuf, NVNface faces, int mask) {
    std::cout << "nvnCommandBufferSetStencilValueMask called!" << std::endl;
}

void nvnCommandBufferSetStencilMask(NVNcommandBuffer* cmdBuf, NVNface faces, int mask) {
    std::cout << "nvnCommandBufferSetStencilMask called!" << std::endl;
}

void nvnCommandBufferSetStencilRef(NVNcommandBuffer* cmdBuf, NVNface faces, int ref) {
    std::cout << "nvnCommandBufferSetStencilRef called!" << std::endl;
}

void nvnCommandBufferSetBlendColor(NVNcommandBuffer* cmdBuf, const float* blendColor) {
    std::cout << "nvnCommandBufferSetBlendColor called!" << std::endl;
}

void nvnCommandBufferSetPointSize(NVNcommandBuffer* cmdBuf, float pointSize) {
    std::cout << "nvnCommandBufferSetPointSize called!" << std::endl;
}

void nvnCommandBufferSetLineWidth(NVNcommandBuffer* cmdBuf, float lineWidth) {
    std::cout << "nvnCommandBufferSetLineWidth called!" << std::endl;
}

void nvnCommandBufferSetPolygonOffsetClamp(NVNcommandBuffer* cmdBuf, float factor, float units, float clamp) {
    std::cout << "nvnCommandBufferSetPolygonOffsetClamp called!" << std::endl;
}

void nvnCommandBufferSetAlphaRef(NVNcommandBuffer* cmdBuf, float ref) {
    std::cout << "nvnCommandBufferSetAlphaRef called!" << std::endl;
}

void nvnCommandBufferSetSampleMask(NVNcommandBuffer* cmdBuf, int mask) {
    std::cout << "nvnCommandBufferSetSampleMask called!" << std::endl;
}

void nvnCommandBufferSetRasterizerDiscard(NVNcommandBuffer* cmdBuf, NVNboolean discard) {
    std::cout << "nvnCommandBufferSetRasterizerDiscard called!" << std::endl;
}

void nvnCommandBufferSetDepthClamp(NVNcommandBuffer* cmdBuf, NVNboolean clamp) {
    std::cout << "nvnCommandBufferSetDepthClamp called!" << std::endl;
}

void nvnCommandBufferSetConservativeRasterEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetConservativeRasterEnable called!" << std::endl;
}

void nvnCommandBufferSetConservativeRasterDilate(NVNcommandBuffer* cmdBuf, float f) {
    std::cout << "nvnCommandBufferSetConservativeRasterDilate called!" << std::endl;
}

void nvnCommandBufferSetSubpixelPrecisionBias(NVNcommandBuffer* cmdBuf, int i1, int i2) {
    std::cout << "nvnCommandBufferSetSubpixelPrecisionBias called!" << std::endl;
}

void nvnCommandBufferCopyBufferToTexture(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, int flags) {
    std::cout << "nvnCommandBufferCopyBufferToTexture called!" << std::endl;
}

void nvnCommandBufferCopyTextureToBuffer(NVNcommandBuffer* cmdBuf, const NVNtexture* srcTexture, const NVNtextureView* srcView, const NVNcopyRegion* srcRegion, NVNbufferAddress buffer, int flags) {
    std::cout << "nvnCommandBufferCopyTextureToBuffer called!" << std::endl;
}

void nvnCommandBufferCopyTextureToTexture(NVNcommandBuffer* cmdBuf, const NVNtexture* srcTexture, const NVNtextureView* srcView, const NVNcopyRegion* srcRegion, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, int flags) {
    std::cout << "nvnCommandBufferCopyTextureToTexture called!" << std::endl;
}

void nvnCommandBufferCopyBufferToBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress src, NVNbufferAddress dst, size_t size, int flags) {
    std::cout << "nvnCommandBufferCopyBufferToBuffer called!" << std::endl;
}

void nvnCommandBufferClearBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, size_t size, uint32_t i) {
    std::cout << "nvnCommandBufferClearBuffer called!" << std::endl;
}

void nvnCommandBufferClearTexture(NVNcommandBuffer* cmdBuf, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, const float* color, int mask) {
    std::cout << "nvnCommandBufferClearTexture called!" << std::endl;
}

void nvnCommandBufferClearTexturei(NVNcommandBuffer* cmdBuf, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, const int* color, int mask) {
    std::cout << "nvnCommandBufferClearTexturei called!" << std::endl;
}

void nvnCommandBufferClearTextureui(NVNcommandBuffer* cmdBuf, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, const uint32_t* color, int mask) {
    std::cout << "nvnCommandBufferClearTextureui called!" << std::endl;
}

void nvnCommandBufferUpdateUniformBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, size_t size, ptrdiff_t o, size_t s, const void* p) {
    std::cout << "nvnCommandBufferUpdateUniformBuffer called!" << std::endl;
}

void nvnCommandBufferReportCounter(NVNcommandBuffer* cmdBuf, NVNcounterType counter, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferReportCounter called!" << std::endl;
}

void nvnCommandBufferResetCounter(NVNcommandBuffer* cmdBuf, NVNcounterType counter) {
    std::cout << "nvnCommandBufferResetCounter called!" << std::endl;
}

void nvnCommandBufferReportValue(NVNcommandBuffer* cmdBuf, uint32_t value, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferReportValue called!" << std::endl;
}

void nvnCommandBufferSetRenderEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetRenderEnable called!" << std::endl;
}

void nvnCommandBufferSetRenderEnableConditional(NVNcommandBuffer* cmdBuf, NVNconditionalRenderMode mode, NVNbufferAddress addr) {
    std::cout << "nvnCommandBufferSetRenderEnableConditional called!" << std::endl;
}

void nvnCommandBufferSetRenderTargets(NVNcommandBuffer* cmdBuf, int numColors, const NVNtexture* const* colors, const NVNtextureView* const* colorViews, const NVNtexture* depthStencil, const NVNtextureView* depthStencilView) {
    std::cout << "nvnCommandBufferSetRenderTargets(numColors=" << numColors << ", colors=" << std::hex << reinterpret_cast<uint64_t>(colors) << ", colorViews=" << reinterpret_cast<uint64_t>(colorViews) << ", depthStencil=" << reinterpret_cast<uint64_t>(depthStencil) << ", depthStencilView=" << reinterpret_cast<uint64_t>(depthStencilView) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferDiscardColor(NVNcommandBuffer* cmdBuf, int i) {
    std::cout << "nvnCommandBufferDiscardColor called!" << std::endl;
}

void nvnCommandBufferDiscardDepthStencil(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferDiscardDepthStencil called!" << std::endl;
}

void nvnCommandBufferDownsample(NVNcommandBuffer* cmdBuf, const NVNtexture* src, const NVNtexture* dst) {
    std::cout << "nvnCommandBufferDownsample called!" << std::endl;
}

void nvnCommandBufferTiledDownsample(NVNcommandBuffer* cmdBuf, const NVNtexture* texture1, const NVNtexture* texture2) {
    std::cout << "nvnCommandBufferTiledDownsample called!" << std::endl;
}

void nvnCommandBufferDownsampleTextureView(NVNcommandBuffer* cmdBuf, const NVNtexture* texture1, const NVNtextureView* view1, const NVNtexture* texture2, const NVNtextureView* view2) {
    std::cout << "nvnCommandBufferDownsampleTextureView called!" << std::endl;
}

void nvnCommandBufferTiledDownsampleTextureView(NVNcommandBuffer* cmdBuf, const NVNtexture* texture1, const NVNtextureView* view1, const NVNtexture* texture2, const NVNtextureView* view2) {
    std::cout << "nvnCommandBufferTiledDownsampleTextureView called!" << std::endl;
}

void nvnCommandBufferBarrier(NVNcommandBuffer* cmdBuf, int barrier) {
    std::cout << "nvnCommandBufferBarrier called!" << std::endl;
}

void nvnCommandBufferWaitSync(NVNcommandBuffer* cmdBuf, const NVNsync* sync) {
    std::cout << "nvnCommandBufferWaitSync called!" << std::endl;
}

void nvnCommandBufferFenceSync(NVNcommandBuffer* cmdBuf, NVNsync* sync, NVNsyncCondition condition, int fence) {
    std::cout << "nvnCommandBufferFenceSync called!" << std::endl;
}

void nvnCommandBufferSetTexturePool(NVNcommandBuffer* cmdBuf, const NVNtexturePool* pool) {
    std::cout << "nvnCommandBufferSetTexturePool called!" << std::endl;
}

void nvnCommandBufferSetSamplerPool(NVNcommandBuffer* cmdBuf, const NVNsamplerPool* pool) {
    std::cout << "nvnCommandBufferSetSamplerPool called!" << std::endl;
}

void nvnCommandBufferSetShaderScratchMemory(NVNcommandBuffer* cmdBuf, const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnCommandBufferSetShaderScratchMemory called!" << std::endl;
}

void nvnCommandBufferSaveZCullData(NVNcommandBuffer* cmdBuf, NVNbufferAddress addr, size_t size) {
    std::cout << "nvnCommandBufferSaveZCullData called!" << std::endl;
}

void nvnCommandBufferRestoreZCullData(NVNcommandBuffer* cmdBuf, NVNbufferAddress addr, size_t size) {
    std::cout << "nvnCommandBufferRestoreZCullData called!" << std::endl;
}

void nvnCommandBufferSetCopyRowStride(NVNcommandBuffer* cmdBuf, ptrdiff_t stride) {
    std::cout << "nvnCommandBufferSetCopyRowStride called!" << std::endl;
}

void nvnCommandBufferSetCopyImageStride(NVNcommandBuffer* cmdBuf, ptrdiff_t stride) {
    std::cout << "nvnCommandBufferSetCopyImageStride called!" << std::endl;
}

ptrdiff_t nvnCommandBufferGetCopyRowStride(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCopyRowStride called!" << std::endl;
    return 0;
}

ptrdiff_t nvnCommandBufferGetCopyImageStride(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCopyImageStride called!" << std::endl;
    return 0;
}

void nvnCommandBufferDrawTexture(NVNcommandBuffer* cmdBuf, NVNtextureHandle handle, const NVNdrawTextureRegion* region1, const NVNdrawTextureRegion* region2) {
    std::cout << "nvnCommandBufferDrawTexture called!" << std::endl;
}

NVNboolean nvnProgramSetSubroutineLinkage(NVNprogram* program, int i, const NVNsubroutineLinkageMapPtr* ptr) {
    std::cout << "nvnProgramSetSubroutineLinkage called!" << std::endl;
    return 1;
}

void nvnCommandBufferSetProgramSubroutines(NVNcommandBuffer* cmdBuf, NVNprogram* program, NVNshaderStage stage, const int i1, const int i2, const int* i3) {
    std::cout << "nvnCommandBufferSetProgramSubroutines called!" << std::endl;
}

void nvnCommandBufferBindCoverageModulationTable(NVNcommandBuffer* cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferBindCoverageModulationTable called!" << std::endl;
}

void nvnCommandBufferResolveDepthBuffer(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferResolveDepthBuffer called!" << std::endl;
}

void nvnCommandBufferPushDebugGroupStatic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroupStatic called!" << std::endl;
}

void nvnCommandBufferPushDebugGroupDynamic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroupDynamic called!" << std::endl;
}

void nvnCommandBufferPushDebugGroup(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroup called!" << std::endl;
}

void nvnCommandBufferPopDebugGroup(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferPopDebugGroup called!" << std::endl;
}

void nvnCommandBufferPopDebugGroupId(NVNcommandBuffer* cmdBuf, uint32_t i) {
    std::cout << "nvnCommandBufferPopDebugGroupId called!" << std::endl;
}

void nvnCommandBufferInsertDebugMarkerStatic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarkerStatic called!" << std::endl;
}

void nvnCommandBufferInsertDebugMarkerDynamic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarkerDynamic called!" << std::endl;
}

void nvnCommandBufferInsertDebugMarker(NVNcommandBuffer* cmdBuf, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarker called!" << std::endl;
}

PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC nvnCommandBufferGetMemoryCallback(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetMemoryCallback called!" << std::endl;
    return nullptr;
}

void nvnCommandBufferGetMemoryCallbackData(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetMemoryCallbackData called!" << std::endl;
}

NVNboolean nvnCommandBufferIsRecording(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferIsRecording called!" << std::endl;
    return 0;
}

// Sync functions
NVNboolean nvnSyncInitialize(NVNsync* sync, NVNdevice* device) {
    std::cout << "nvnSyncInitialize called!" << std::endl;
    return 1;
}

void nvnSyncFinalize(NVNsync* sync) {
    std::cout << "nvnSyncFinalize called!" << std::endl;
}

void nvnSyncSetDebugLabel(NVNsync* sync, const char* label) {
    std::cout << "nvnSyncSetDebugLabel called!" << std::endl;
}

void nvnQueueFenceSync(NVNqueue* queue, NVNsync* sync, NVNsyncCondition condition, int flags) {
    std::cout << "nvnQueueFenceSync called!" << std::endl;
}

NVNsyncWaitResult nvnSyncWait(const NVNsync* sync, uint64_t timeoutNs) {
    std::cout << "nvnSyncWait called!" << std::endl;
    return 0;
}

NVNboolean nvnQueueWaitSync(NVNqueue* queue, const NVNsync* sync) {
    std::cout << "nvnQueueWaitSync called!" << std::endl;
    return 1;
}

// Event functions
void nvnEventBuilderSetDefaults(NVNeventBuilder* builder) {
    std::cout << "nvnEventBuilderSetDefaults called!" << std::endl;
}

void nvnEventBuilderSetStorage(NVNeventBuilder* builder, const NVNmemoryPool* pool, int64_t size) {
    std::cout << "nvnEventBuilderSetStorage called!" << std::endl;
}

NVNboolean nvnEventInitialize(NVNevent* event, const NVNeventBuilder* builder) {
    std::cout << "nvnEventInitialize called!" << std::endl;
    return 1;
}

void nvnEventFinalize(NVNevent* event) {
    std::cout << "nvnEventFinalize called!" << std::endl;
}

uint32_t nvnEventGetValue(const NVNevent* event) {
    std::cout << "nvnEventGetValue called!" << std::endl;
    return 0;
}

void nvnEventSignal(NVNevent* event, NVNeventSignalMode mode, uint32_t i) {
    std::cout << "nvnEventSignal called!" << std::endl;
}

void nvnCommandBufferWaitEvent(NVNcommandBuffer* cmdBuf, const NVNevent* event, NVNeventWaitMode mode, uint32_t i) {
    std::cout << "nvnCommandBufferWaitEvent called!" << std::endl;
}

void nvnCommandBufferSignalEvent(NVNcommandBuffer* cmdBuf, const NVNevent* event, NVNeventSignalMode mode, NVNeventSignalLocation location, int flags, uint32_t i) {
    std::cout << "nvnCommandBufferSignalEvent called!" << std::endl;
}

// Missing functions that are in nvn.h but not in reference
void nvnDeviceWaitForError(NVNdevice* device) {
    std::cout << "nvnDeviceWaitForError called!" << std::endl;
}

// Function lookup functions (must be at the end after all functions are defined)
PFNNVNGENERICFUNCPTRPROC nvnDeviceGetProcAddress(const NVNdevice* device, const char* name) {
    std::cout << "nvnDeviceGetProcAddress (from device) '" << name << "'" << std::endl;
    NVN_ALL(NVN_FUNC);
    if(strcmp(name, "nvnDeviceGetProcAddress") == 0) return reinterpret_cast<PFNNVNGENERICFUNCPTRPROC>((void*)nvnDeviceGetProcAddress);
    std::cout << "Could not find nvn function " << name << std::endl;
    __builtin_trap();
    return nullptr;
}

void* nvnBootstrapLoader(const char* name) {
    std::cout << "nvnBootstrapLoader called! '" << name << "'" << std::endl;
    if(strcmp(name, "nvnDeviceGetProcAddress") == 0)
        return reinterpret_cast<void*>(nvnDeviceGetProcAddress);
    return nullptr;
}
