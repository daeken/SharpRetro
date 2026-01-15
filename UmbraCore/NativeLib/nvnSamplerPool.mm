#include <iostream>
#include "nv.h"

NVNboolean nvnSamplerPoolInitialize(NVNsamplerPool* _samplerPool, NVNmemoryPool* _memoryPool, ptrdiff_t offset, int numDescriptors) {
    std::cout << "nvnSamplerPoolInitialize(memoryPool=" << std::hex << reinterpret_cast<uint64_t>(_memoryPool) << ", offset=" << std::dec << offset << ", numDescriptors=" << numDescriptors << ") called!" << std::endl;
    auto samplerPool = UNWRAP(_samplerPool);
    auto memoryPool = UNWRAP(_memoryPool);
    return 1;
}

void nvnSamplerPoolSetDebugLabel(NVNsamplerPool* _pool, const char* label) {
    std::cout << "nvnSamplerPoolSetDebugLabel called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void nvnSamplerPoolFinalize(NVNsamplerPool* _pool) {
    std::cout << "nvnSamplerPoolFinalize called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void nvnSamplerPoolRegisterSampler(NVNsamplerPool* _pool, int id, NVNsampler* _sampler) {
    std::cout << "nvnSamplerPoolRegisterSampler(id=" << id << ", sampler=" << std::hex << reinterpret_cast<uint64_t>(_sampler) << std::dec << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
    auto sampler = UNWRAP(_sampler);
}

void nvnSamplerPoolRegisterSamplerBuilder(NVNsamplerPool* _pool, int id, NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerPoolRegisterSamplerBuilder(id=" << id << ", builder=" << std::hex << reinterpret_cast<uint64_t>(_builder) << std::dec << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
    auto builder = UNWRAP(_builder);
}

NVNmemoryPool* nvnSamplerPoolGetMemoryPool(NVNsamplerPool* _pool) {
    std::cout << "nvnSamplerPoolGetMemoryPool() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return nullptr;
}

ptrdiff_t nvnSamplerPoolGetMemoryOffset(NVNsamplerPool* _pool) {
    std::cout << "nvnSamplerPoolGetMemoryOffset() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}

int nvnSamplerPoolGetSize(NVNsamplerPool* _pool) {
    std::cout << "nvnSamplerPoolGetSize() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}

NVNdevice* nvnBufferBuilderGetDevice(NVNbufferBuilder* _builder) {
    std::cout << "nvnBufferBuilderGetDevice() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}
