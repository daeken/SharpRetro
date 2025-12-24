#include "library.h"
#include "nv.h"
#include "vi.h"

#define hook(name, func) regFunc(name, reinterpret_cast<void*>(func))

void setupHooks(const hookRegister_t regFunc) {
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
}