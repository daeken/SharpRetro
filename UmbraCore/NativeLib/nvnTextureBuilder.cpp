#include <iostream>
#include "nv.h"

void nvnTextureBuilderSetDevice(NVNtextureBuilder* builder, NVNdevice* device) {
    std::cout << "nvnTextureBuilderSetDevice called!" << std::endl;
}

void nvnTextureBuilderSetDefaults(NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderSetDefaults called!" << std::endl;
}

void nvnTextureBuilderSetFlags(NVNtextureBuilder* builder, int flags) {
    std::cout << "nvnTextureBuilderSetFlags(flags=" << flags << ") called!" << std::endl;
}

void nvnTextureBuilderSetTarget(NVNtextureBuilder* builder, NVNtextureTarget target) {
    std::cout << "nvnTextureBuilderSetTarget(target=" << target << ") called!" << std::endl;
}

void nvnTextureBuilderSetWidth(NVNtextureBuilder* builder, int width) {
    std::cout << "nvnTextureBuilderSetWidth(width=" << width << ") called!" << std::endl;
}

void nvnTextureBuilderSetHeight(NVNtextureBuilder* builder, int height) {
    std::cout << "nvnTextureBuilderSetHeight(height=" << height << ") called!" << std::endl;
}

void nvnTextureBuilderSetDepth(NVNtextureBuilder* builder, int depth) {
    std::cout << "nvnTextureBuilderSetDepth(depth=" << depth << ") called!" << std::endl;
}

void nvnTextureBuilderSetSize1D(NVNtextureBuilder* builder, int size) {
    std::cout << "nvnTextureBuilderSetSize1D(size=" << size << ") called!" << std::endl;
}

void nvnTextureBuilderSetSize2D(NVNtextureBuilder* builder, int width, int height) {
    std::cout << "nvnTextureBuilderSetSize2D(width=" << width << ", height=" << height << ") called!" << std::endl;
}

void nvnTextureBuilderSetSize3D(NVNtextureBuilder* builder, int width, int height, int depth) {
    std::cout << "nvnTextureBuilderSetSize3D(width=" << width << ", height=" << height << ", depth=" << depth << ") called!" << std::endl;
}

void nvnTextureBuilderSetLevels(NVNtextureBuilder* builder, int numLevels) {
    std::cout << "nvnTextureBuilderSetLevels(numLevels=" << numLevels << ") called!" << std::endl;
}

void nvnTextureBuilderSetFormat(NVNtextureBuilder* builder, NVNformat format) {
    std::cout << "nvnTextureBuilderSetFormat(format=" << format << ") called!" << std::endl;
}

void nvnTextureBuilderSetSamples(NVNtextureBuilder* builder, int samples) {
    std::cout << "nvnTextureBuilderSetSamples(samples=" << samples << ") called!" << std::endl;
}

void nvnTextureBuilderSetSwizzle(NVNtextureBuilder* builder, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a) {
    std::cout << "nvnTextureBuilderSetSwizzle(r=" << r << ", g=" << g << ", b=" << b << ", a=" << a << ") called!" << std::endl;
}

void nvnTextureBuilderSetDepthStencilMode(NVNtextureBuilder* builder, NVNtextureDepthStencilMode mode) {
    std::cout << "nvnTextureBuilderSetDepthStencilMode(mode=" << mode << ") called!" << std::endl;
}

size_t nvnTextureBuilderGetStorageSize(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStorageSize() called!" << std::endl;
    return 0;
}

size_t nvnTextureBuilderGetStorageAlignment(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStorageAlignment() called!" << std::endl;
    return 0;
}

void nvnTextureBuilderSetStorage(NVNtextureBuilder* builder, NVNmemoryPool* pool, ptrdiff_t offset) {
    std::cout << "nvnTextureBuilderSetStorage(pool=" << std::hex << reinterpret_cast<uint64_t>(pool) << ", offset=" << std::dec << offset << ") called!" << std::endl;
}

void nvnTextureBuilderSetPackagedTextureData(NVNtextureBuilder* builder, const void* data) {
    std::cout << "nvnTextureBuilderSetPackagedTextureData called!" << std::endl;
}

void nvnTextureBuilderSetPackagedTextureLayout(NVNtextureBuilder* builder, const NVNpackagedTextureLayout* layout) {
    std::cout << "nvnTextureBuilderSetPackagedTextureLayout called!" << std::endl;
}

void nvnTextureBuilderSetStride(NVNtextureBuilder* builder, ptrdiff_t stride) {
    std::cout << "nvnTextureBuilderSetStride(stride=" << stride << ") called!" << std::endl;
}

void nvnTextureBuilderSetGLTextureName(NVNtextureBuilder* builder, uint32_t name) {
    std::cout << "nvnTextureBuilderSetGLTextureName(name=" << name << ") called!" << std::endl;
}

NVNstorageClass nvnTextureBuilderGetStorageClass(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStorageClass() called!" << std::endl;
    return 0;
}

NVNtextureFlags nvnTextureBuilderGetFlags(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetFlags() called!" << std::endl;
    return 0;
}

NVNtextureTarget nvnTextureBuilderGetTarget(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetTarget() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetWidth(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetWidth() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetHeight(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetHeight() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetDepth(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetDepth() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetLevels(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetLevels() called!" << std::endl;
    return 0;
}

NVNformat nvnTextureBuilderGetFormat(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetFormat() called!" << std::endl;
    return 0;
}

int nvnTextureBuilderGetSamples(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetSamples() called!" << std::endl;
    return 0;
}

void nvnTextureBuilderGetSwizzle(const NVNtextureBuilder* builder, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureBuilderGetSwizzle called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}

NVNtextureDepthStencilMode nvnTextureBuilderGetDepthStencilMode(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetDepthStencilMode called!" << std::endl;
    return 0;
}

const void* nvnTextureBuilderGetPackagedTextureData(const NVNtextureBuilder* builder) {
    return nullptr;
}

ptrdiff_t nvnTextureBuilderGetStride(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetStride called!" << std::endl;
    return 0;
}

void nvnTextureBuilderGetSparseTileLayout(const NVNtextureBuilder* builder, NVNtextureSparseTileLayout* layout) {
    std::cout << "nvnTextureBuilderGetSparseTileLayout called!" << std::endl;
}

uint32_t nvnTextureBuilderGetGLTextureName(const NVNtextureBuilder* builder) {
    return 0;
}

size_t nvnTextureBuilderGetZCullStorageSize(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetZCullStorageSize called!" << std::endl;
    return 0;
}

NVNmemoryPool nvnTextureBuilderGetMemoryPool(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetMemoryPool called!" << std::endl;
    return NVNmemoryPool{};
}

ptrdiff_t nvnTextureBuilderGetMemoryOffset(const NVNtextureBuilder* builder) {
    std::cout << "nvnTextureBuilderGetMemoryOffset called!" << std::endl;
    return 0;
}