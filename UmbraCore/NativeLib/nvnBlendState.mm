#include <iostream>
#include "nv.h"

void nvnBlendStateSetDefaults(NVNblendState* _blend) {
    std::cout << "nvnBlendStateSetDefaults called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetBlendTarget(NVNblendState* _blend, int target) {
    std::cout << "nvnBlendStateSetBlendTarget(target=" << target << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetBlendFunc(NVNblendState* _blend, NVNblendFunc srcFunc, NVNblendFunc dstFunc, NVNblendFunc srcFuncAlpha, NVNblendFunc dstFuncAlpha) {
    std::cout << "nvnBlendStateSetBlendFunc(srcFunc=" << srcFunc << ", dstFunc=" << dstFunc << ", srcFuncAlpha=" << srcFuncAlpha << ", dstFuncAlpha=" << dstFuncAlpha << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetBlendEquation(NVNblendState* _blend, NVNblendEquation modeRGB, NVNblendEquation modeAlpha) {
    std::cout << "nvnBlendStateSetBlendEquation(modeRGB=" << modeRGB << ", modeAlpha=" << modeAlpha << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetAdvancedMode(NVNblendState* _blend, NVNblendAdvancedMode mode) {
    std::cout << "nvnBlendStateSetAdvancedMode(mode=" << mode << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetAdvancedOverlap(NVNblendState* _blend, NVNblendAdvancedOverlap overlap) {
    std::cout << "nvnBlendStateSetAdvancedOverlap(overlap=" << overlap << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetAdvancedPremultipliedSrc(NVNblendState* _blend, NVNboolean b) {
    std::cout << "nvnBlendStateSetAdvancedPremultipliedSrc(b=" << b << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

void nvnBlendStateSetAdvancedNormalizedDst(NVNblendState* _blend, NVNboolean b) {
    std::cout << "nvnBlendStateSetAdvancedNormalizedDst(b=" << b << ") called!" << std::endl;
    auto blend = UNWRAP(_blend);
}

int nvnBlendStateGetBlendTarget(NVNblendState* _blend) {
    std::cout << "nvnBlendStateGetBlendTarget called!" << std::endl;
    auto blend = UNWRAP(_blend);
    return 0;
}

void nvnBlendStateGetBlendFunc(NVNblendState* _blend, NVNblendFunc* srcFunc, NVNblendFunc* dstFunc, NVNblendFunc* srcFuncAlpha, NVNblendFunc* dstFuncAlpha) {
    std::cout << "nvnBlendStateGetBlendFunc called!" << std::endl;
    auto blend = UNWRAP(_blend);
    if (srcFunc) *srcFunc = 0;
    if (dstFunc) *dstFunc = 0;
    if (srcFuncAlpha) *srcFuncAlpha = 0;
    if (dstFuncAlpha) *dstFuncAlpha = 0;
}

void nvnBlendStateGetBlendEquation(NVNblendState* _blend, NVNblendEquation* modeRGB, NVNblendEquation* modeAlpha) {
    std::cout << "nvnBlendStateGetBlendEquation called!" << std::endl;
    auto blend = UNWRAP(_blend);
    if (modeRGB) *modeRGB = 0;
    if (modeAlpha) *modeAlpha = 0;
}

NVNblendAdvancedMode nvnBlendStateGetAdvancedMode(NVNblendState* _blend) {
    std::cout << "nvnBlendStateGetAdvancedMode called!" << std::endl;
    auto blend = UNWRAP(_blend);
    return 0;
}

NVNblendAdvancedOverlap nvnBlendStateGetAdvancedOverlap(NVNblendState* _blend) {
    std::cout << "nvnBlendStateGetAdvancedOverlap called!" << std::endl;
    auto blend = UNWRAP(_blend);
    return 0;
}

NVNboolean nvnBlendStateGetAdvancedPremultipliedSrc(NVNblendState* _blend) {
    std::cout << "nvnBlendStateGetAdvancedPremultipliedSrc called!" << std::endl;
    auto blend = UNWRAP(_blend);
    return 0;
}

NVNboolean nvnBlendStateGetAdvancedNormalizedDst(NVNblendState* _blend) {
    std::cout << "nvnBlendStateGetAdvancedNormalizedDst called!" << std::endl;
    auto blend = UNWRAP(_blend);
    return 0;
}
