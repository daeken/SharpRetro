#include <iostream>
#include "nv.h"

NVNboolean nvnTextureInitialize(NVNtexture* texture, const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureInitialize called!" << std::endl;
    return 1;
}

size_t nvnTextureGetZCullStorageSize(const NVNtexture* texture) {
    std::cout << "nvnTextureGetZCullStorageSize called!" << std::endl;
    return 0;
}

void nvnTextureFinalize(NVNtexture* texture) {
    std::cout << "nvnTextureFinalize called!" << std::endl;
}

void nvnTextureSetDebugLabel(NVNtexture* texture, const char* label) {
    std::cout << "nvnTextureSetDebugLabel called!" << std::endl;
}

NVNstorageClass nvnTextureGetStorageClass(const NVNtexture* texture) {
    std::cout << "nvnTextureGetStorageClass called!" << std::endl;
    return 0;
}

ptrdiff_t nvnTextureGetViewOffset(const NVNtexture* texture, const NVNtextureView* view) {
    std::cout << "nvnTextureGetViewOffset called!" << std::endl;
    return 0;
}

NVNtextureFlags nvnTextureGetFlags(const NVNtexture* texture) {
    std::cout << "nvnTextureGetFlags called!" << std::endl;
    return 0;
}

NVNtextureTarget nvnTextureGetTarget(const NVNtexture* texture) {
    std::cout << "nvnTextureGetTarget called!" << std::endl;
    return 0;
}

int nvnTextureGetWidth(const NVNtexture* texture) {
    std::cout << "nvnTextureGetWidth called!" << std::endl;
    return 0;
}

int nvnTextureGetHeight(const NVNtexture* texture) {
    std::cout << "nvnTextureGetHeight called!" << std::endl;
    return 0;
}

int nvnTextureGetDepth(const NVNtexture* texture) {
    std::cout << "nvnTextureGetDepth called!" << std::endl;
    return 0;
}

int nvnTextureGetLevels(const NVNtexture* texture) {
    std::cout << "nvnTextureGetLevels called!" << std::endl;
    return 0;
}

NVNformat nvnTextureGetFormat(const NVNtexture* texture) {
    std::cout << "nvnTextureGetFormat called!" << std::endl;
    return 0;
}

int nvnTextureGetSamples(const NVNtexture* texture) {
    std::cout << "nvnTextureGetSamples called!" << std::endl;
    return 0;
}

void nvnTextureGetSwizzle(const NVNtexture* texture, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureGetSwizzle called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

NVNtextureDepthStencilMode nvnTextureGetDepthStencilMode(const NVNtexture* texture) {
    std::cout << "nvnTextureGetDepthStencilMode called!" << std::endl;
    return 0;
}

ptrdiff_t nvnTextureGetStride(const NVNtexture* texture) {
    std::cout << "nvnTextureGetStride called!" << std::endl;
    return 0;
}

NVNtextureAddress nvnTextureGetTextureAddress(const NVNtexture* texture) {
    std::cout << "nvnTextureGetTextureAddress called!" << std::endl;
    return 0;
}

void nvnTextureGetSparseTileLayout(const NVNtexture* texture, NVNtextureSparseTileLayout* layout) {
    std::cout << "nvnTextureGetSparseTileLayout called!" << std::endl;
}

void nvnTextureWriteTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, const void* p) {
    std::cout << "nvnTextureWriteTexels called!" << std::endl;
}

void nvnTextureWriteTexelsStrided(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, const void* p, ptrdiff_t o1, ptrdiff_t o2) {
    std::cout << "nvnTextureWriteTexelsStrided called!" << std::endl;
}

void nvnTextureReadTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, void* p) {
    std::cout << "nvnTextureReadTexels called!" << std::endl;
}

void nvnTextureReadTexelsStrided(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region, void* p, ptrdiff_t o1, ptrdiff_t o2) {
    std::cout << "nvnTextureReadTexelsStrided called!" << std::endl;
}

void nvnTextureFlushTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region) {
    std::cout << "nvnTextureFlushTexels called!" << std::endl;
}

void nvnTextureInvalidateTexels(const NVNtexture* texture, const NVNtextureView* view, const NVNcopyRegion* region) {
    std::cout << "nvnTextureInvalidateTexels called!" << std::endl;
}

NVNmemoryPool nvnTextureGetMemoryPool(const NVNtexture* texture) {
    std::cout << "nvnTextureGetMemoryPool called!" << std::endl;
    return NVNmemoryPool{};
}

ptrdiff_t nvnTextureGetMemoryOffset(const NVNtexture* texture) {
    std::cout << "nvnTextureGetMemoryOffset called!" << std::endl;
    return 0;
}

int nvnTextureGetStorageSize(const NVNtexture* texture) {
    std::cout << "nvnTextureGetStorageSize called!" << std::endl;
    return 0;
}

NVNboolean nvnTextureCompare(const NVNtexture* texture1, const NVNtexture* texture2) {
    std::cout << "nvnTextureCompare called!" << std::endl;
    return 0;
}

uint64_t nvnTextureGetDebugID(const NVNtexture* texture) {
    std::cout << "nvnTextureGetDebugID called!" << std::endl;
    return 0;
}