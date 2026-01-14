#include <iostream>
#include "nv.h"

NVNboolean nvnSyncInitialize(NVNsync* sync, NVNdevice* device) {
    std::cout << "nvnSyncInitialize called!" << std::endl;
    return 1;
}

NVNboolean nvnSyncInitializeFromFencedGLSync(NVNsync* sync) {
    std::cout << "nvnSyncInitializeFromFencedGLSync called!" << std::endl;
    return 1;
}

void* nvnSyncCreateGLSync(NVNsync* sync) {
    std::cout << "nvnSyncCreateGLSync called!" << std::endl;
    return nullptr;
}

void nvnSyncFinalize(NVNsync* sync) {
    std::cout << "nvnSyncFinalize called!" << std::endl;
}

void nvnSyncSetDebugLabel(NVNsync* sync, const char* label) {
    std::cout << "nvnSyncSetDebugLabel called!" << std::endl;
}

NVNsyncWaitResult nvnSyncWait(const NVNsync* sync, uint64_t timeoutNs) {
    std::cout << "nvnSyncWait called!" << std::endl;
    return 0;
}
