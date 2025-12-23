#include <iostream>
#include "nv.h"

void nvnTextureViewSetDefaults(NVNtextureView* view) {
    std::cout << "nvnTextureViewSetDefaults called!" << std::endl;
}

void nvnTextureViewSetLevels(NVNtextureView* view, int baseLevel, int numLevels) {
    std::cout << "nvnTextureViewSetLevels called!" << std::endl;
}

void nvnTextureViewSetLayers(NVNtextureView* view, int minLayer, int numLayers) {
    std::cout << "nvnTextureViewSetLayers called!" << std::endl;
}

void nvnTextureViewSetFormat(NVNtextureView* view, NVNformat format) {
    std::cout << "nvnTextureViewSetFormat called!" << std::endl;
}

void nvnTextureViewSetSwizzle(NVNtextureView* view, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a) {
    std::cout << "nvnTextureViewSetSwizzle called!" << std::endl;
}

void nvnTextureViewSetDepthStencilMode(NVNtextureView* view, NVNtextureDepthStencilMode mode) {
    std::cout << "nvnTextureViewSetDepthStencilMode called!" << std::endl;
}

void nvnTextureViewSetTarget(NVNtextureView* view, NVNtextureTarget target) {
    std::cout << "nvnTextureViewSetTarget called!" << std::endl;
}

NVNboolean nvnTextureViewGetLevels(const NVNtextureView* view, int* baseLevel, int* numLevels) {
    std::cout << "nvnTextureViewGetLevels called!" << std::endl;
    if (baseLevel) *baseLevel = 0;
    if (numLevels) *numLevels = 0;
    return 0;
}

NVNboolean nvnTextureViewGetLayers(const NVNtextureView* view, int* minLayer, int* numLayers) {
    std::cout << "nvnTextureViewGetLayers called!" << std::endl;
    if (minLayer) *minLayer = 0;
    if (numLayers) *numLayers = 0;
    return 0;
}

NVNboolean nvnTextureViewGetFormat(const NVNtextureView* view, NVNformat* format) {
    std::cout << "nvnTextureViewGetFormat called!" << std::endl;
    if (format) *format = 0;
    return 0;
}

NVNboolean nvnTextureViewGetSwizzle(const NVNtextureView* view, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureViewGetSwizzle called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
    return 0;
}

NVNboolean nvnTextureViewGetDepthStencilMode(const NVNtextureView* view, NVNtextureDepthStencilMode* mode) {
    std::cout << "nvnTextureViewGetDepthStencilMode called!" << std::endl;
    if (mode) *mode = 0;
    return 0;
}

NVNboolean nvnTextureViewGetTarget(const NVNtextureView* view, NVNtextureTarget* target) {
    std::cout << "nvnTextureViewGetTarget called!" << std::endl;
    if (target) *target = 0;
    return 0;
}

NVNboolean nvnTextureViewCompare(const NVNtextureView* view1, const NVNtextureView* view2) {
    std::cout << "nvnTextureViewCompare called!" << std::endl;
    return 0;
}