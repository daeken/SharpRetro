#include "library.h"

#include <iostream>
#include <ostream>

#include "nv.h"
#include "vi.h"
#include "glslc.h"

#define hook(name, func) regFunc(name, reinterpret_cast<void*>(func))

thread_local uint64_t x18 = 0;

uint64_t getX18() {
    std::cout << "getX18: " << std::hex << x18 << std::dec << std::endl;
    return x18;
}
void setX18(uint64_t value) {
    std::cout << "setX18: " << std::hex << value << std::dec << std::endl;
    x18 = value;
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

    hook("glslcCompilePreSpecialized", glslcCompilePreSpecialized);
    hook("glslcCompileSpecialized", glslcCompileSpecialized);
    hook("glslcInitialize", glslcInitialize);
    hook("glslcFinalize", glslcFinalize);
    hook("glslcGetVersion", glslcGetVersion);
    hook("glslcSetAllocator", glslcSetAllocator);
    hook("glslcGetDefaultOptions", glslcGetDefaultOptions);
}