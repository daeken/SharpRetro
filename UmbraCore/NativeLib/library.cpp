#include "library.h"
#include "nv.h"
#include "vi.h"

#define hook(name, func) regFunc(name, reinterpret_cast<void*>(func))

void setupHooks(const hookRegister_t regFunc) {
    hook("_ZN2nv18InitializeGraphicsEPvm", nv::InitializeGraphics);
    hook("_ZN2nv20SetGraphicsAllocatorEPFPvmmS0_EPFvS0_S0_EPFS0_S0_mS0_ES0_", nv::SetGraphicsAllocator);
    hook("nvnBootstrapLoader", nvnBootstrapLoader);
    //hook("_ZN2nn2vi10InitializeEv", nn::vi::Initialize);
}