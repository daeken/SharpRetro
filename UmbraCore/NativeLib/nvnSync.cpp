#include <iostream>
#include "nv.h"

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

NVNsyncWaitResult nvnSyncWait(const NVNsync* sync, uint64_t timeoutNs) {
    std::cout << "nvnSyncWait called!" << std::endl;
    return 0;
}
