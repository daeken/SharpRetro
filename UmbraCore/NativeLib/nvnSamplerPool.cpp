#include <iostream>
#include "nv.h"

NVNboolean nvnSamplerPoolInitialize(NVNsamplerPool* samplerPool, const NVNmemoryPool* memoryPool, ptrdiff_t offset, int numDescriptors) {
    std::cout << "nvnSamplerPoolInitialize(memoryPool=" << std::hex << reinterpret_cast<uint64_t>(memoryPool) << ", offset=" << std::dec << offset << ", numDescriptors=" << numDescriptors << ") called!" << std::endl;
    return 1;
}

void nvnSamplerPoolSetDebugLabel(NVNsamplerPool* pool, const char* label) {
    std::cout << "nvnSamplerPoolSetDebugLabel called!" << std::endl;
}

void nvnSamplerPoolFinalize(NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolFinalize called!" << std::endl;
}

void nvnSamplerPoolRegisterSampler(const NVNsamplerPool* pool, int id, const NVNsampler* sampler) {
    std::cout << "nvnSamplerPoolRegisterSampler(id=" << id << ", sampler=" << std::hex << reinterpret_cast<uint64_t>(sampler) << std::dec << ") called!" << std::endl;
}

void nvnSamplerPoolRegisterSamplerBuilder(const NVNsamplerPool* pool, int id, const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerPoolRegisterSamplerBuilder(id=" << id << ", builder=" << std::hex << reinterpret_cast<uint64_t>(builder) << std::dec << ") called!" << std::endl;
}

const NVNmemoryPool* nvnSamplerPoolGetMemoryPool(const NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolGetMemoryPool() called!" << std::endl;
    return nullptr;
}

ptrdiff_t nvnSamplerPoolGetMemoryOffset(const NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolGetMemoryOffset() called!" << std::endl;
    return 0;
}

int nvnSamplerPoolGetSize(const NVNsamplerPool* pool) {
    std::cout << "nvnSamplerPoolGetSize() called!" << std::endl;
    return 0;
}