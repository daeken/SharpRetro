#include <iostream>
#include "nv.h"

void nvnTextureViewSetDefaults(NVNtextureView* _view) {
    std::cout << "nvnTextureViewSetDefaults called!" << std::endl;
    auto view = UNWRAP(_view);
}

void nvnTextureViewSetLevels(NVNtextureView* _view, int baseLevel, int numLevels) {
    std::cout << "nvnTextureViewSetLevels called!" << std::endl;
    auto view = UNWRAP(_view);
}

void nvnTextureViewSetLayers(NVNtextureView* _view, int minLayer, int numLayers) {
    std::cout << "nvnTextureViewSetLayers called!" << std::endl;
    auto view = UNWRAP(_view);
}

void nvnTextureViewSetFormat(NVNtextureView* _view, NVNformat format) {
    std::cout << "nvnTextureViewSetFormat called!" << std::endl;
    auto view = UNWRAP(_view);
}

void nvnTextureViewSetSwizzle(NVNtextureView* _view, NVNtextureSwizzle r, NVNtextureSwizzle g, NVNtextureSwizzle b, NVNtextureSwizzle a) {
    std::cout << "nvnTextureViewSetSwizzle called!" << std::endl;
    auto view = UNWRAP(_view);
}

void nvnTextureViewSetDepthStencilMode(NVNtextureView* _view, NVNtextureDepthStencilMode mode) {
    std::cout << "nvnTextureViewSetDepthStencilMode called!" << std::endl;
    auto view = UNWRAP(_view);
}

void nvnTextureViewSetTarget(NVNtextureView* _view, NVNtextureTarget target) {
    std::cout << "nvnTextureViewSetTarget called!" << std::endl;
    auto view = UNWRAP(_view);
}

NVNboolean nvnTextureViewGetLevels(NVNtextureView* _view, int* baseLevel, int* numLevels) {
    std::cout << "nvnTextureViewGetLevels called!" << std::endl;
    auto view = UNWRAP(_view);
    if (baseLevel) *baseLevel = 0;
    if (numLevels) *numLevels = 0;
    return 0;
}

NVNboolean nvnTextureViewGetLayers(NVNtextureView* _view, int* minLayer, int* numLayers) {
    std::cout << "nvnTextureViewGetLayers called!" << std::endl;
    auto view = UNWRAP(_view);
    if (minLayer) *minLayer = 0;
    if (numLayers) *numLayers = 0;
    return 0;
}

NVNboolean nvnTextureViewGetFormat(NVNtextureView* _view, NVNformat* format) {
    std::cout << "nvnTextureViewGetFormat called!" << std::endl;
    auto view = UNWRAP(_view);
    if (format) *format = 0;
    return 0;
}

NVNboolean nvnTextureViewGetSwizzle(NVNtextureView* _view, NVNtextureSwizzle* r, NVNtextureSwizzle* g, NVNtextureSwizzle* b, NVNtextureSwizzle* a) {
    std::cout << "nvnTextureViewGetSwizzle called!" << std::endl;
    auto view = UNWRAP(_view);
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
    return 0;
}

NVNboolean nvnTextureViewGetDepthStencilMode(NVNtextureView* _view, NVNtextureDepthStencilMode* mode) {
    std::cout << "nvnTextureViewGetDepthStencilMode called!" << std::endl;
    auto view = UNWRAP(_view);
    if (mode) *mode = 0;
    return 0;
}

NVNboolean nvnTextureViewGetTarget(NVNtextureView* _view, NVNtextureTarget* target) {
    std::cout << "nvnTextureViewGetTarget called!" << std::endl;
    auto view = UNWRAP(_view);
    if (target) *target = 0;
    return 0;
}

NVNboolean nvnTextureViewCompare(NVNtextureView* _view1, NVNtextureView* _view2) {
    std::cout << "nvnTextureViewCompare called!" << std::endl;
    auto view1 = UNWRAP(_view1);
    auto view2 = UNWRAP(_view2);
    return 0;
}
