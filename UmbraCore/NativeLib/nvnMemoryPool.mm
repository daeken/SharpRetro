#include <iostream>
#include "nv.h"

void nvnMemoryPoolBuilderSetDevice(NVNmemoryPoolBuilder* _builder, NVNdevice* _device) {
    std::cout << "nvnMemoryPoolBuilderSetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto device = UNWRAP(_device);
}

void nvnMemoryPoolBuilderSetDefaults(NVNmemoryPoolBuilder* _builder) {
    std::cout << "nvnMemoryPoolBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnMemoryPoolBuilderSetStorage(NVNmemoryPoolBuilder* _builder, void* memory, size_t size) {
    std::cout << "nvnMemoryPoolBuilderSetStorage called! size: " << size << std::endl;
    auto builder = UNWRAP(_builder);
    builder->pool = memory;
    builder->size = size;
}

void nvnMemoryPoolBuilderSetFlags(NVNmemoryPoolBuilder* _builder, int flags) {
    std::cout << "nvnMemoryPoolBuilderSetFlags (" << flags << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnMemoryPoolBuilderGetMemory(NVNmemoryPoolBuilder* _builder) {
    std::cout << "nvnMemoryPoolBuilderGetMemory called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnMemoryPoolBuilderGetSize(NVNmemoryPoolBuilder* _builder) {
    std::cout << "nvnMemoryPoolBuilderGetSize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNmemoryPoolFlags nvnMemoryPoolBuilderGetFlags(NVNmemoryPoolBuilder* _builder) {
    std::cout << "nvnMemoryPoolBuilderGetFlags() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNboolean nvnMemoryPoolInitialize(NVNmemoryPool* _pool, NVNmemoryPoolBuilder* _builder) {
    std::cout << "nvnMemoryPoolInitialize called!" << std::endl;
    auto pool = UNWRAP(_pool);
    auto builder = UNWRAP(_builder);
    pool->pool = builder->pool;
    pool->size = builder->size;
    return 1;
}

void nvnMemoryPoolSetDebugLabel(NVNmemoryPool* _pool, const char* label) {
    std::cout << "nvnMemoryPoolSetDebugLabel called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void nvnMemoryPoolFinalize(NVNmemoryPool* _pool) {
    std::cout << "nvnMemoryPoolFinalize called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void* nvnMemoryPoolMap(NVNmemoryPool* _pool) {
    std::cout << "nvnMemoryPoolMap() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return pool->pool;
}

void nvnMemoryPoolFlushMappedRange(NVNmemoryPool* _pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnMemoryPoolFlushMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void nvnMemoryPoolInvalidateMappedRange(NVNmemoryPool* _pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnMemoryPoolInvalidateMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

NVNbufferAddress nvnMemoryPoolGetBufferAddress(NVNmemoryPool* _pool) {
    std::cout << "nvnMemoryPoolGetBufferAddress() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}

NVNboolean nvnMemoryPoolMapVirtual(NVNmemoryPool* _pool, int numRequests, NVNmappingRequest* _requests) {
    std::cout << "nvnMemoryPoolMapVirtual(numRequests=" << numRequests << ", requests=" << std::hex << reinterpret_cast<uint64_t>(_requests) << std::dec << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
    auto requests = UNWRAP(_requests);
    return 1;
}

size_t nvnMemoryPoolGetSize(NVNmemoryPool* _pool) {
    std::cout << "nvnMemoryPoolGetSize() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}

NVNmemoryPoolFlags nvnMemoryPoolGetFlags(NVNmemoryPool* _pool) {
    std::cout << "nvnMemoryPoolGetFlags() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}
