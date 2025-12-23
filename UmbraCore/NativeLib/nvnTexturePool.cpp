#include <iostream>
#include "nv.h"

NVNboolean nvnTexturePoolInitialize(NVNtexturePool* texturePool, const NVNmemoryPool* memoryPool, ptrdiff_t offset, int numDescriptors) {
    std::cout << "nvnTexturePoolInitialize(memoryPool=" << std::hex << reinterpret_cast<uint64_t>(memoryPool) << ", offset=" << std::dec << offset << ", numDescriptors=" << numDescriptors << ") called!" << std::endl;
    return 1;
}

void nvnTexturePoolSetDebugLabel(NVNtexturePool* pool, const char* label) {
    std::cout << "nvnTexturePoolSetDebugLabel called!" << std::endl;
}

void nvnTexturePoolFinalize(NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolFinalize called!" << std::endl;
}

void nvnTexturePoolRegisterTexture(const NVNtexturePool* pool, int id, const NVNtexture* texture, const NVNtextureView* view) {
    std::cout << "nvnTexturePoolRegisterTexture(id=" << id << ", texture=" << std::hex << reinterpret_cast<uint64_t>(texture) << ", view=" << reinterpret_cast<uint64_t>(view) << std::dec << ") called!" << std::endl;
}

void nvnTexturePoolRegisterImage(const NVNtexturePool* pool, int id, const NVNtexture* texture, const NVNtextureView* view) {
    std::cout << "nvnTexturePoolRegisterImage(id=" << id << ", texture=" << std::hex << reinterpret_cast<uint64_t>(texture) << ", view=" << reinterpret_cast<uint64_t>(view) << std::dec << ") called!" << std::endl;
}

const NVNmemoryPool* nvnTexturePoolGetMemoryPool(const NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolGetMemoryPool() called!" << std::endl;
    return nullptr;
}

ptrdiff_t nvnTexturePoolGetMemoryOffset(const NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolGetMemoryOffset() called!" << std::endl;
    return 0;
}

int nvnTexturePoolGetSize(const NVNtexturePool* pool) {
    std::cout << "nvnTexturePoolGetSize() called!" << std::endl;
    return 0;
}