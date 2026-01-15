#include <iostream>
#include "nv.h"

NVNboolean nvnSyncInitialize(NVNsync* _sync, NVNdevice* _device) {
    std::cout << "nvnSyncInitialize called!" << std::endl;
    auto sync = UNWRAP(_sync);
    auto device = UNWRAP(_device);
    return 1;
}

NVNboolean nvnSyncInitializeFromFencedGLSync(NVNsync* _sync) {
    std::cout << "nvnSyncInitializeFromFencedGLSync called!" << std::endl;
    auto sync = UNWRAP(_sync);
    return 1;
}

void* nvnSyncCreateGLSync(NVNsync* _sync) {
    std::cout << "nvnSyncCreateGLSync called!" << std::endl;
    auto sync = UNWRAP(_sync);
    return nullptr;
}

void nvnSyncFinalize(NVNsync* _sync) {
    std::cout << "nvnSyncFinalize called!" << std::endl;
    auto sync = UNWRAP(_sync);
}

void nvnSyncSetDebugLabel(NVNsync* _sync, const char* label) {
    std::cout << "nvnSyncSetDebugLabel called!" << std::endl;
    auto sync = UNWRAP(_sync);
}

NVNsyncWaitResult nvnSyncWait(NVNsync* _sync, uint64_t timeoutNs) {
    std::cout << "nvnSyncWait called!" << std::endl;
    auto sync = UNWRAP(_sync);
    return 0;
}
