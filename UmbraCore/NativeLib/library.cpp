#include "library.h"

#include <iostream>
#include <ostream>

#include "nv.h"
#include "vi.h"
#include "glslc.h"

#define hook(name, func) regFunc(name, reinterpret_cast<void*>(func))

thread_local uint64_t x18 = 0;

uint64_t getX18() noexcept {
    auto temp = x18;
    //printf("getX18: %llx\n", temp);
    return temp;
}
void setX18(uint64_t value) noexcept {
    //printf("setX18: %llx\n", value);
    x18 = value;
}

void* glslc_Alloc(uint64_t size) {
    std::cout << "glslc_Alloc: 0x" << std::hex << size << std::dec << std::endl;
    if(size == 0) return nullptr;
    auto ret = malloc(size);
    std::cout << "allocated at 0x" << std::hex << reinterpret_cast<uint64_t>(ret) << std::dec << std::endl;
    return ret;
}

void* glslc_Realloc(void* ptr, uint64_t size) {
    std::cout << "glslc_Realloc: 0x" << std::hex << reinterpret_cast<uint64_t>(ptr) << " size 0x" << size << std::dec << std::endl;
    auto ret = realloc(ptr, size);
    std::cout << "allocated at 0x" << std::hex << reinterpret_cast<uint64_t>(ret) << std::dec << std::endl;
    return ret;
}

void* glslc_AllocAlign(uint64_t size, uint64_t align) {
    __builtin_trap();
}

void glslc_Free(void* ptr) {
    if(ptr == nullptr) return;
    std::cout << "glslc_Free: 0x" << std::hex << reinterpret_cast<uint64_t>(ptr) << std::dec << std::endl;
    free(ptr);
}

void* HeapAlloc(void* unk, uint64_t size) {
    std::cout << "HeapAlloc: 0x" << std::hex << size << std::dec << std::endl;
    if(size == 0) return nullptr;
    auto ret = malloc(size);
    std::cout << "allocated at 0x" << std::hex << reinterpret_cast<uint64_t>(ret) << std::dec << std::endl;
    return ret;
}
void* HeapAllocAlign(void* unk, uint64_t size, uint64_t align) {
    __builtin_trap();
}
void* HeapRealloc(void* unk, void* ptr, uint64_t size) {
    std::cout << "HeapRealloc: 0x" << std::hex << reinterpret_cast<uint64_t>(ptr) << " size 0x" << size << std::dec << std::endl;
    auto ret = realloc(ptr, size);
    std::cout << "allocated at 0x" << std::hex << reinterpret_cast<uint64_t>(ret) << std::dec << std::endl;
    return ret;
}
void HeapFree(void* unk, void* ptr) {
    if(ptr == nullptr) return;
    std::cout << "HeapFree: 0x" << std::hex << reinterpret_cast<uint64_t>(ptr) << std::dec << std::endl;
    free(ptr);
}

static struct {
    void* Alloc = reinterpret_cast<void*>(HeapAlloc);
    void* AllocAlign = reinterpret_cast<void*>(HeapAllocAlign);
    void* Realloc = reinterpret_cast<void*>(HeapRealloc);
    void* Free = reinterpret_cast<void*>(HeapFree);
} HeapAllocator;

void* glslc_GetAllocator() {
    std::cout << "glslc_GetAllocator!" << std::endl;
    return &HeapAllocator;
}
void glslc_DebugVprintf() {
    __builtin_trap();
}
void glslc_DebugPrintf() {
    __builtin_trap();
}
void glslc_DebugString() {
    __builtin_trap();
}

void movie_SetAllocator() {
    std::cout << "movie_SetAllocator!" << std::endl;
}

void setupHooks(const hookRegister_t regFunc) {
    hook("$getX18", getX18);
    hook("$setX18", setX18);

    hook("_ZN2nv18InitializeGraphicsEPvm", nv::InitializeGraphics);
    hook("_ZN2nv20SetGraphicsAllocatorEPFPvmmS0_EPFvS0_S0_EPFS0_S0_mS0_ES0_", nv::SetGraphicsAllocator);
    hook("nvnBootstrapLoader", nvnBootstrapLoader);

    hook("_ZN2nn2vi10InitializeEv", nn::vi::Initialize);
    hook("_ZN2nn2vi11OpenDisplayEPPNS0_7DisplayEPKc", nn::vi::OpenDisplay);
    hook("_ZN2nn2vi18OpenDefaultDisplayEPPNS0_7DisplayE", nn::vi::OpenDefaultDisplay);
    hook("_ZN2nn2vi20GetDisplayVsyncEventEPNS_2os15SystemEventTypeEPNS0_7DisplayE", nn::vi::GetDisplayVsyncEvent);
    hook("_ZN2nn2vi11CreateLayerEPPNS0_5LayerEPNS0_7DisplayE", nn::vi::CreateLayer);
    hook("_ZN2nn2vi19SetLayerScalingModeEPNS0_5LayerENS0_11ScalingModeE", nn::vi::SetLayerScalingMode);
    hook("_ZN2nn2vi12DestroyLayerEPNS0_5LayerE", nn::vi::DestroyLayer);
    hook("_ZN2nn2vi15GetNativeWindowEPPvPNS0_5LayerE", nn::vi::GetNativeWindow);

    hook("_ZN5movie12SetAllocatorEPFPvmmS0_EPFvS0_S0_EPFS0_S0_mS0_ES0_", movie_SetAllocator);

    // this is normally provided by nv
    // only needed for native glslc, not replacements
    hook("glslc_Alloc", glslc_Alloc);
    hook("glslc_AllocAlign", glslc_AllocAlign);
    hook("glslc_Realloc", glslc_Realloc);
    hook("glslc_Free", glslc_Free);
    hook("glslc_GetAllocator", glslc_GetAllocator);
    hook("glslc_DebugVprintf", glslc_DebugVprintf);
    hook("glslc_DebugPrintf", glslc_DebugPrintf);
    hook("glslc_DebugString", glslc_DebugString);

    /*hook("glslcCompilePreSpecialized", glslcCompilePreSpecialized);
    hook("glslcCompileSpecialized", glslcCompileSpecialized);
    hook("glslcInitialize", glslcInitialize);
    hook("glslcFinalize", glslcFinalize);
    hook("glslcGetVersion", glslcGetVersion);
    hook("glslcSetAllocator", glslcSetAllocator);
    hook("glslcGetDefaultOptions", glslcGetDefaultOptions);*/
}