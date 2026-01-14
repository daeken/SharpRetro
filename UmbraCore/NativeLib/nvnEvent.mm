#include <iostream>
#include "nv.h"

void nvnEventBuilderSetDefaults(NVNeventBuilder* builder) {
    std::cout << "nvnEventBuilderSetDefaults called!" << std::endl;
}

NVNmemoryPool* nvnEventBuilderGetMemoryPool(NVNeventBuilder* builder) {
    std::cout << "nvnEventBuilderGetMemoryPool called!" << std::endl;
    return nullptr;
}

int nvnEventBuilderGetMemoryOffset(NVNeventBuilder* builder) {
    std::cout << "nvnEventBuilderGetMemoryOffset called!" << std::endl;
    return 0;
}

NVNmemoryPool* nvnEventBuilderGetStorage(NVNeventBuilder* builder) {
    std::cout << "nvnEventBuilderGetStorage called!" << std::endl;
    return nullptr;
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

NVNmemoryPool* nvnEventGetMemoryPool(NVNevent* builder) {
    std::cout << "nvnEventGetMemoryPool called!" << std::endl;
    return nullptr;
}

int nvnEventGetMemoryOffset(NVNevent* builder) {
    std::cout << "nvnEventGetMemoryOffset called!" << std::endl;
    return 0;
}

void nvnEventSignal(NVNevent* event, NVNeventSignalMode mode, uint32_t i) {
    std::cout << "nvnEventSignal called!" << std::endl;
}
