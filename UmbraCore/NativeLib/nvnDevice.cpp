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

void nvnDeviceWaitForError(NVNdevice* device) {
    std::cout << "nvnDeviceWaitForError called!" << std::endl;
}