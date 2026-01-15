#include <iostream>
#include "nv.h"

void nvnBufferBuilderSetDevice(NVNbufferBuilder* _builder, NVNdevice* _device) {
    std::cout << "nvnBufferBuilderSetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto device = UNWRAP(_device);
}

void nvnBufferBuilderSetDefaults(NVNbufferBuilder* _builder) {
    std::cout << "nvnBufferBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnBufferBuilderSetStorage(NVNbufferBuilder* _builder, NVNmemoryPool* _pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferBuilderSetStorage(pool=" << std::hex << reinterpret_cast<uint64_t>(_pool) << ", offset=" << std::dec << offset << ", size=" << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto pool = UNWRAP(_pool);
    builder->pool = _pool;
    builder->offset = offset;
    builder->size = size;
}

NVNmemoryPool nvnBufferBuilderGetMemoryPool(NVNbufferBuilder* _builder) {
    std::cout << "nvnBufferBuilderGetMemoryPool() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return NVNmemoryPool{};
}

ptrdiff_t nvnBufferBuilderGetMemoryOffset(NVNbufferBuilder* _builder) {
    std::cout << "nvnBufferBuilderGetMemoryOffset() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

size_t nvnBufferBuilderGetSize(NVNbufferBuilder* _builder) {
    std::cout << "nvnBufferBuilderGetSize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNboolean nvnBufferInitialize(NVNbuffer* _buffer, NVNbufferBuilder* _builder) {
    std::cout << "nvnBufferInitialize called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    auto builder = UNWRAP(_builder);
    buffer->pool = builder->pool;
    buffer->offset = builder->offset;
    buffer->size = builder->size;
    return 1;
}

void nvnBufferSetDebugLabel(NVNbuffer* _buffer, const char* label) {
    std::cout << "nvnBufferSetDebugLabel called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
}

void nvnBufferFinalize(NVNbuffer* _buffer) {
    std::cout << "nvnBufferFinalize called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
}

void* nvnBufferMap(NVNbuffer* _buffer) {
    std::cout << "nvnBufferMap() called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    auto pool = UNWRAP(buffer->pool);
    return static_cast<uint8_t*>(pool->pool) + buffer->offset;
}

NVNbufferAddress nvnBufferGetAddress(NVNbuffer* _buffer) {
    std::cout << "nvnBufferGetAddress() called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    auto pool = UNWRAP(buffer->pool);
    return reinterpret_cast<NVNbufferAddress>(pool->pool) + buffer->offset;
}

void nvnBufferFlushMappedRange(NVNbuffer* _buffer, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferFlushMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
}

void nvnBufferInvalidateMappedRange(NVNbuffer* _buffer, ptrdiff_t offset, size_t size) {
    std::cout << "nvnBufferInvalidateMappedRange(offset=" << offset << ", size=" << size << ") called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
}

NVNmemoryPool* nvnBufferGetMemoryPool(NVNbuffer* _buffer) {
    std::cout << "nvnBufferGetMemoryPool() called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    return nullptr;
}

ptrdiff_t nvnBufferGetMemoryOffset(NVNbuffer* _buffer) {
    std::cout << "nvnBufferGetMemoryOffset() called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    return 0;
}

size_t nvnBufferGetSize(NVNbuffer* _buffer) {
    std::cout << "nvnBufferGetSize() called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    return 0;
}

uint64_t nvnBufferGetDebugID(NVNbuffer* _buffer) {
    std::cout << "nvnBufferGetDebugID() called!" << std::endl;
    auto buffer = UNWRAP(_buffer);
    return 0;
}

NVNdevice* nvnTextureBuilderGetDevice(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetDevice() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}
