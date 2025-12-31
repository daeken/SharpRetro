#include <iostream>
#include "nv.h"

void nvnBufferBuilderSetDevice(NVNbufferBuilder* builder, NVNdevice* device) {
    std::cout << "nvnBufferBuilderSetDevice called!" << std::endl;
}

void nvnBufferBuilderSetDefaults(NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderSetDefaults called!" << std::endl;
}

void nvnBufferBuilderSetStorage(NVNbufferBuilder* builder, NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferBuilderSetStorage(pool=" << std::hex << reinterpret_cast<uint64_t>(pool) << ", offset=" << std::dec << offset << ", size=" << size << ") called!" << std::endl;
}

NVNmemoryPool nvnBufferBuilderGetMemoryPool(const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderGetMemoryPool() called!" << std::endl;
    return NVNmemoryPool{};
}

ptrdiff_t nvnBufferBuilderGetMemoryOffset(const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderGetMemoryOffset() called!" << std::endl;
    return 0;
}

size_t nvnBufferBuilderGetSize(const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferBuilderGetSize() called!" << std::endl;
    return 0;
}

NVNboolean nvnBufferInitialize(NVNbuffer* buffer, const NVNbufferBuilder* builder) {
    std::cout << "nvnBufferInitialize called!" << std::endl;
    return 1;
}

void nvnBufferSetDebugLabel(NVNbuffer* buffer, const char* label) {
    std::cout << "nvnBufferSetDebugLabel called!" << std::endl;
}

void nvnBufferFinalize(NVNbuffer* buffer) {
    std::cout << "nvnBufferFinalize called!" << std::endl;
}

void* nvnBufferMap(const NVNbuffer* buffer) {
    std::cout << "nvnBufferMap() called!" << std::endl;
    return nullptr;
}

NVNbufferAddress nvnBufferGetAddress(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetAddress() called!" << std::endl;
    return 0xDEADBEEF00120000;
}

void nvnBufferFlushMappedRange(const NVNbuffer* buffer, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferFlushMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

void nvnBufferInvalidateMappedRange(const NVNbuffer* buffer, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferInvalidateMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
}

NVNmemoryPool* nvnBufferGetMemoryPool(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetMemoryPool() called!" << std::endl;
    return nullptr;
}

ptrdiff_t nvnBufferGetMemoryOffset(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetMemoryOffset() called!" << std::endl;
    return 0;
}

size_t nvnBufferGetSize(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetSize() called!" << std::endl;
    return 0;
}

uint64_t nvnBufferGetDebugID(const NVNbuffer* buffer) {
    std::cout << "nvnBufferGetDebugID() called!" << std::endl;
    return 0;
}