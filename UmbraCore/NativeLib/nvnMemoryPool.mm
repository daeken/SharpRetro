#include <iostream>
#include "nv.h"

void nvnMemoryPoolBuilderSetDevice(NVNmemoryPoolBuilder* builder, NVNdevice* device) {
    std::cout << "nvnMemoryPoolBuilderSetDevice called!" << std::endl;
}

void nvnMemoryPoolBuilderSetDefaults(NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderSetDefaults called!" << std::endl;
}

void nvnMemoryPoolBuilderSetStorage(NVNmemoryPoolBuilder* builder, void* memory, size_t size) {
    std::cout << "nvnMemoryPoolBuilderSetStorage called! size: " << size << std::endl;
    builder->pool = memory;
    builder->size = size;
}

void nvnMemoryPoolBuilderSetFlags(NVNmemoryPoolBuilder* builder, int flags) {
    std::cout << "nvnMemoryPoolBuilderSetFlags (" << flags << ") called!" << std::endl;
}

void nvnMemoryPoolBuilderGetMemory(const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderGetMemory called!" << std::endl;
}

size_t nvnMemoryPoolBuilderGetSize(const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderGetSize() called!" << std::endl;
    return 0;
}

NVNmemoryPoolFlags nvnMemoryPoolBuilderGetFlags(const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolBuilderGetFlags() called!" << std::endl;
    return 0;
}

NVNboolean nvnMemoryPoolInitialize(NVNmemoryPool* pool, const NVNmemoryPoolBuilder* builder) {
    std::cout << "nvnMemoryPoolInitialize called!" << std::endl;
    pool->pool = builder->pool;
    pool->size = builder->size;
    return 1;
}

void nvnMemoryPoolSetDebugLabel(NVNmemoryPool* pool, const char* label) {
    std::cout << "nvnMemoryPoolSetDebugLabel called!" << std::endl;
}

void nvnMemoryPoolFinalize(NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolFinalize called!" << std::endl;
}

void* nvnMemoryPoolMap(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolMap() called!" << std::endl;
    return pool->pool;
}

void nvnMemoryPoolFlushMappedRange(const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnMemoryPoolFlushMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

void nvnMemoryPoolInvalidateMappedRange(const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnMemoryPoolInvalidateMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

NVNbufferAddress nvnMemoryPoolGetBufferAddress(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolGetBufferAddress() called!" << std::endl;
    return 0;
}

NVNboolean nvnMemoryPoolMapVirtual(NVNmemoryPool* pool, int numRequests, const NVNmappingRequest* requests) {
    std::cout << "nvnMemoryPoolMapVirtual(numRequests=" << numRequests << ", requests=" << std::hex << reinterpret_cast<uint64_t>(requests) << std::dec << ") called!" << std::endl;
    return 1;
}

size_t nvnMemoryPoolGetSize(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolGetSize() called!" << std::endl;
    return 0;
}

NVNmemoryPoolFlags nvnMemoryPoolGetFlags(const NVNmemoryPool* pool) {
    std::cout << "nvnMemoryPoolGetFlags() called!" << std::endl;
    return 0;
}