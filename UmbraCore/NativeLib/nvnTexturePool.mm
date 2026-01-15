#include <iostream>
#include "nv.h"

NVNboolean nvnTexturePoolInitialize(NVNtexturePool* _texturePool, NVNmemoryPool* _memoryPool, ptrdiff_t offset, int numDescriptors) {
    std::cout << "nvnTexturePoolInitialize(memoryPool=" << std::hex << reinterpret_cast<uint64_t>(_memoryPool) << ", offset=" << std::dec << offset << ", numDescriptors=" << numDescriptors << ") called!" << std::endl;
    auto texturePool = UNWRAP(_texturePool);
    auto memoryPool = UNWRAP(_memoryPool);
    return 1;
}

void nvnTexturePoolSetDebugLabel(NVNtexturePool* _pool, const char* label) {
    std::cout << "nvnTexturePoolSetDebugLabel called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void nvnTexturePoolFinalize(NVNtexturePool* _pool) {
    std::cout << "nvnTexturePoolFinalize called!" << std::endl;
    auto pool = UNWRAP(_pool);
}

void nvnTexturePoolRegisterTexture(NVNtexturePool* _pool, int id, NVNtexture* _texture, NVNtextureView* _view) {
    std::cout << "nvnTexturePoolRegisterTexture(id=" << id << ", texture=" << std::hex << reinterpret_cast<uint64_t>(_texture) << ", view=" << reinterpret_cast<uint64_t>(_view) << std::dec << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
}

void nvnTexturePoolRegisterImage(NVNtexturePool* _pool, int id, NVNtexture* _texture, NVNtextureView* _view) {
    std::cout << "nvnTexturePoolRegisterImage(id=" << id << ", texture=" << std::hex << reinterpret_cast<uint64_t>(_texture) << ", view=" << reinterpret_cast<uint64_t>(_view) << std::dec << ") called!" << std::endl;
    auto pool = UNWRAP(_pool);
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
}

NVNmemoryPool* nvnTexturePoolGetMemoryPool(NVNtexturePool* _pool) {
    std::cout << "nvnTexturePoolGetMemoryPool() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return nullptr;
}

ptrdiff_t nvnTexturePoolGetMemoryOffset(NVNtexturePool* _pool) {
    std::cout << "nvnTexturePoolGetMemoryOffset() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}

int nvnTexturePoolGetSize(NVNtexturePool* _pool) {
    std::cout << "nvnTexturePoolGetSize() called!" << std::endl;
    auto pool = UNWRAP(_pool);
    return 0;
}
