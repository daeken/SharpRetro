#include <iostream>
#include "nv.h"

void nvnEventBuilderSetDefaults(NVNeventBuilder* _builder) {
    std::cout << "nvnEventBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

NVNmemoryPool* nvnEventBuilderGetMemoryPool(NVNeventBuilder* _builder) {
    std::cout << "nvnEventBuilderGetMemoryPool called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

int nvnEventBuilderGetMemoryOffset(NVNeventBuilder* _builder) {
    std::cout << "nvnEventBuilderGetMemoryOffset called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNmemoryPool* nvnEventBuilderGetStorage(NVNeventBuilder* _builder) {
    std::cout << "nvnEventBuilderGetStorage called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

void nvnEventBuilderSetStorage(NVNeventBuilder* _builder, NVNmemoryPool* _pool, int64_t size) {
    std::cout << "nvnEventBuilderSetStorage called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto pool = UNWRAP(_pool);
}

NVNboolean nvnEventInitialize(NVNevent* _event, NVNeventBuilder* _builder) {
    std::cout << "nvnEventInitialize called!" << std::endl;
    auto event = UNWRAP(_event);
    auto builder = UNWRAP(_builder);
    return 1;
}

void nvnEventFinalize(NVNevent* _event) {
    std::cout << "nvnEventFinalize called!" << std::endl;
    auto event = UNWRAP(_event);
}

uint32_t nvnEventGetValue(NVNevent* _event) {
    std::cout << "nvnEventGetValue called!" << std::endl;
    auto event = UNWRAP(_event);
    return 0;
}

NVNmemoryPool* nvnEventGetMemoryPool(NVNevent* _event) {
    std::cout << "nvnEventGetMemoryPool called!" << std::endl;
    auto event = UNWRAP(_event);
    return nullptr;
}

int nvnEventGetMemoryOffset(NVNevent* _event) {
    std::cout << "nvnEventGetMemoryOffset called!" << std::endl;
    auto event = UNWRAP(_event);
    return 0;
}

void nvnEventSignal(NVNevent* _event, NVNeventSignalMode mode, uint32_t i) {
    std::cout << "nvnEventSignal called!" << std::endl;
    auto event = UNWRAP(_event);
}
