#include <iostream>
#include "nv.h"

NVNboolean nvnTextureInitialize(NVNtexture* _texture, NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureInitialize called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto builder = UNWRAP(_builder);
    return 1;
}

size_t nvnTextureGetZCullStorageSize(NVNtexture* _texture) {
    std::cout << "nvnTextureGetZCullStorageSize called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

void nvnTextureFinalize(NVNtexture* _texture) {
    std::cout << "nvnTextureFinalize called!" << std::endl;
    auto texture = UNWRAP(_texture);
}

void nvnTextureSetDebugLabel(NVNtexture* _texture, const char* label) {
    std::cout << "nvnTextureSetDebugLabel called!" << std::endl;
    auto texture = UNWRAP(_texture);
}

NVNstorageClass nvnTextureGetStorageClass(NVNtexture* _texture) {
    std::cout << "nvnTextureGetStorageClass called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

ptrdiff_t nvnTextureGetViewOffset(NVNtexture* _texture, NVNtextureView* _view) {
    std::cout << "nvnTextureGetViewOffset called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    return 0;
}

NVNtextureFlags nvnTextureGetFlags(NVNtexture* _texture) {
    std::cout << "nvnTextureGetFlags called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

NVNtextureTarget nvnTextureGetTarget(NVNtexture* _texture) {
    std::cout << "nvnTextureGetTarget called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetWidth(NVNtexture* _texture) {
    std::cout << "nvnTextureGetWidth called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetHeight(NVNtexture* _texture) {
    std::cout << "nvnTextureGetHeight called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetDepth(NVNtexture* _texture) {
    std::cout << "nvnTextureGetDepth called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetLevels(NVNtexture* _texture) {
    std::cout << "nvnTextureGetLevels called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

NVNformat nvnTextureGetFormat(NVNtexture* _texture) {
    std::cout << "nvnTextureGetFormat called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetSamples(NVNtexture* _texture) {
    std::cout << "nvnTextureGetSamples called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

void nvnTextureGetSwizzle(NVNtexture* _texture, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureGetSwizzle called!" << std::endl;
    auto texture = UNWRAP(_texture);
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

NVNtextureDepthStencilMode nvnTextureGetDepthStencilMode(NVNtexture* _texture) {
    std::cout << "nvnTextureGetDepthStencilMode called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

ptrdiff_t nvnTextureGetStride(NVNtexture* _texture) {
    std::cout << "nvnTextureGetStride called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

NVNtextureAddress nvnTextureGetTextureAddress(NVNtexture* _texture) {
    std::cout << "nvnTextureGetTextureAddress called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

void nvnTextureGetSparseTileLayout(NVNtexture* _texture, NVNtextureSparseTileLayout* _layout) {
    std::cout << "nvnTextureGetSparseTileLayout called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto layout = UNWRAP(_layout);
}

void nvnTextureWriteTexels(NVNtexture* _texture, NVNtextureView* _view, NVNcopyRegion* _region, const void* p) {
    std::cout << "nvnTextureWriteTexels called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    auto region = UNWRAP(_region);
}

void nvnTextureWriteTexelsStrided(NVNtexture* _texture, NVNtextureView* _view, NVNcopyRegion* _region, const void* p, ptrdiff_t o1, ptrdiff_t o2) {
    std::cout << "nvnTextureWriteTexelsStrided called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    auto region = UNWRAP(_region);
}

void nvnTextureReadTexels(NVNtexture* _texture, NVNtextureView* _view, NVNcopyRegion* _region, void* p) {
    std::cout << "nvnTextureReadTexels called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    auto region = UNWRAP(_region);
}

void nvnTextureReadTexelsStrided(NVNtexture* _texture, NVNtextureView* _view, NVNcopyRegion* _region, void* p, ptrdiff_t o1, ptrdiff_t o2) {
    std::cout << "nvnTextureReadTexelsStrided called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    auto region = UNWRAP(_region);
}

void nvnTextureFlushTexels(NVNtexture* _texture, NVNtextureView* _view, NVNcopyRegion* _region) {
    std::cout << "nvnTextureFlushTexels called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    auto region = UNWRAP(_region);
}

void nvnTextureInvalidateTexels(NVNtexture* _texture, NVNtextureView* _view, NVNcopyRegion* _region) {
    std::cout << "nvnTextureInvalidateTexels called!" << std::endl;
    auto texture = UNWRAP(_texture);
    auto view = UNWRAP(_view);
    auto region = UNWRAP(_region);
}

NVNmemoryPool nvnTextureGetMemoryPool(NVNtexture* _texture) {
    std::cout << "nvnTextureGetMemoryPool called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return NVNmemoryPool{};
}

ptrdiff_t nvnTextureGetMemoryOffset(NVNtexture* _texture) {
    std::cout << "nvnTextureGetMemoryOffset called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetStorageSize(NVNtexture* _texture) {
    std::cout << "nvnTextureGetStorageSize called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

int nvnTextureGetRawStorageClass(NVNtexture* _texture) {
    std::cout << "nvnTextureGetRawStorageClass called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

NVNboolean nvnTextureCompare(NVNtexture* _texture1, NVNtexture* _texture2) {
    std::cout << "nvnTextureCompare called!" << std::endl;
    auto texture1 = UNWRAP(_texture1);
    auto texture2 = UNWRAP(_texture2);
    return 0;
}

uint64_t nvnTextureGetDebugID(NVNtexture* _texture) {
    std::cout << "nvnTextureGetDebugID called!" << std::endl;
    auto texture = UNWRAP(_texture);
    return 0;
}

NVNdevice* nvnSamplerBuilderGetDevice(NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerBuilderGetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}
