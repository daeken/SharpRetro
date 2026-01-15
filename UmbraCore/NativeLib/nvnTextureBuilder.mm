#include <iostream>
#include "nv.h"

void nvnTextureBuilderSetDevice(NVNtextureBuilder* _builder, NVNdevice* _device) {
    std::cout << "nvnTextureBuilderSetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto device = UNWRAP(_device);
}

void nvnTextureBuilderSetDefaults(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetFlags(NVNtextureBuilder* _builder, int flags) {
    std::cout << "nvnTextureBuilderSetFlags(flags=" << flags << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetTarget(NVNtextureBuilder* _builder, NVNtextureTarget target) {
    std::cout << "nvnTextureBuilderSetTarget(target=" << target << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetWidth(NVNtextureBuilder* _builder, int width) {
    std::cout << "nvnTextureBuilderSetWidth(width=" << width << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
    builder->width = width;
}

void nvnTextureBuilderSetHeight(NVNtextureBuilder* _builder, int height) {
    std::cout << "nvnTextureBuilderSetHeight(height=" << height << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
    builder->height = height;
}

void nvnTextureBuilderSetDepth(NVNtextureBuilder* _builder, int depth) {
    std::cout << "nvnTextureBuilderSetDepth(depth=" << depth << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetSize1D(NVNtextureBuilder* _builder, int size) {
    std::cout << "nvnTextureBuilderSetSize1D(size=" << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetSize2D(NVNtextureBuilder* _builder, int width, int height) {
    std::cout << "nvnTextureBuilderSetSize2D(width=" << width << ", height=" << height << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetSize3D(NVNtextureBuilder* _builder, int width, int height, int depth) {
    std::cout << "nvnTextureBuilderSetSize3D(width=" << width << ", height=" << height << ", depth=" << depth << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetLevels(NVNtextureBuilder* _builder, int numLevels) {
    std::cout << "nvnTextureBuilderSetLevels(numLevels=" << numLevels << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetFormat(NVNtextureBuilder* _builder, NVNformat format) {
    std::cout << "nvnTextureBuilderSetFormat(format=" << format << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetSamples(NVNtextureBuilder* _builder, int samples) {
    std::cout << "nvnTextureBuilderSetSamples(samples=" << samples << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetSwizzle(NVNtextureBuilder* _builder, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a) {
    std::cout << "nvnTextureBuilderSetSwizzle(r=" << r << ", g=" << g << ", b=" << b << ", a=" << a << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetDepthStencilMode(NVNtextureBuilder* _builder, NVNtextureDepthStencilMode mode) {
    std::cout << "nvnTextureBuilderSetDepthStencilMode(mode=" << mode << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnTextureBuilderGetStorageSize(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetStorageSize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0x80; // Just BS
}

size_t nvnTextureBuilderGetStorageAlignment(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetStorageAlignment() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0x10;
}

void nvnTextureBuilderSetStorage(NVNtextureBuilder* _builder, NVNmemoryPool* _pool, ptrdiff_t offset) {
    std::cout << "nvnTextureBuilderSetStorage(pool=" << std::hex << reinterpret_cast<uint64_t>(_pool) << ", offset=" << std::dec << offset << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto pool = UNWRAP(_pool);
}

void nvnTextureBuilderSetPackagedTextureData(NVNtextureBuilder* _builder, const void* data) {
    std::cout << "nvnTextureBuilderSetPackagedTextureData called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetPackagedTextureLayout(NVNtextureBuilder* _builder, NVNpackagedTextureLayout* _layout) {
    std::cout << "nvnTextureBuilderSetPackagedTextureLayout called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto layout = UNWRAP(_layout);
}

void nvnTextureBuilderSetStride(NVNtextureBuilder* _builder, ptrdiff_t stride) {
    std::cout << "nvnTextureBuilderSetStride(stride=" << stride << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnTextureBuilderSetGLTextureName(NVNtextureBuilder* _builder, uint32_t name) {
    std::cout << "nvnTextureBuilderSetGLTextureName(name=" << name << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

NVNstorageClass nvnTextureBuilderGetStorageClass(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetStorageClass() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNtextureFlags nvnTextureBuilderGetFlags(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetFlags() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNtextureTarget nvnTextureBuilderGetTarget(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetTarget() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnTextureBuilderGetWidth(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetWidth() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnTextureBuilderGetHeight(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetHeight() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnTextureBuilderGetDepth(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetDepth() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnTextureBuilderGetLevels(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetLevels() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNformat nvnTextureBuilderGetFormat(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetFormat() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnTextureBuilderGetSamples(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetSamples() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnTextureBuilderGetSwizzle(NVNtextureBuilder* _builder, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureBuilderGetSwizzle called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

NVNtextureDepthStencilMode nvnTextureBuilderGetDepthStencilMode(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetDepthStencilMode called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

const void* nvnTextureBuilderGetPackagedTextureData(NVNtextureBuilder* _builder) {
    auto builder = UNWRAP(_builder);
    return nullptr;
}

const void* nvnTextureBuilderGetPackagedTextureLayout(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetPackagedTextureLayout called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

ptrdiff_t nvnTextureBuilderGetStride(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetStride called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnTextureBuilderGetSparseTileLayout(NVNtextureBuilder* _builder, NVNtextureSparseTileLayout* _layout) {
    std::cout << "nvnTextureBuilderGetSparseTileLayout called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto layout = UNWRAP(_layout);
}

uint32_t nvnTextureBuilderGetGLTextureName(NVNtextureBuilder* _builder) {
    auto builder = UNWRAP(_builder);
    return 0;
}

size_t nvnTextureBuilderGetZCullStorageSize(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetZCullStorageSize called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNmemoryPool nvnTextureBuilderGetMemoryPool(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetMemoryPool called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return NVNmemoryPool{};
}

ptrdiff_t nvnTextureBuilderGetMemoryOffset(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetMemoryOffset called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnTextureBuilderGetRawStorageClass(NVNtextureBuilder* _builder) {
    std::cout << "nvnTextureBuilderGetRawStorageClass called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}
