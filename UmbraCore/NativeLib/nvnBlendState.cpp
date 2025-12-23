#include <iostream>
#include "nv.h"

void nvnBlendStateSetDefaults(NVNblendState* blend) {
    std::cout << "nvnBlendStateSetDefaults called!" << std::endl;
}

void nvnBlendStateSetBlendTarget(NVNblendState* blend, int target) {
    std::cout << "nvnBlendStateSetBlendTarget(target=" << target << ") called!" << std::endl;
}

void nvnBlendStateSetBlendFunc(NVNblendState* blend, NVNblendFunc srcFunc, NVNblendFunc dstFunc, NVNblendFunc srcFuncAlpha, NVNblendFunc dstFuncAlpha) {
    std::cout << "nvnBlendStateSetBlendFunc(srcFunc=" << srcFunc << ", dstFunc=" << dstFunc << ", srcFuncAlpha=" << srcFuncAlpha << ", dstFuncAlpha=" << dstFuncAlpha << ") called!" << std::endl;
}

void nvnBlendStateSetBlendEquation(NVNblendState* blend, NVNblendEquation modeRGB, NVNblendEquation modeAlpha) {
    std::cout << "nvnBlendStateSetBlendEquation(modeRGB=" << modeRGB << ", modeAlpha=" << modeAlpha << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedMode(NVNblendState* blend, NVNblendAdvancedMode mode) {
    std::cout << "nvnBlendStateSetAdvancedMode(mode=" << mode << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedOverlap(NVNblendState* blend, NVNblendAdvancedOverlap overlap) {
    std::cout << "nvnBlendStateSetAdvancedOverlap(overlap=" << overlap << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedPremultipliedSrc(NVNblendState* blend, NVNboolean b) {
    std::cout << "nvnBlendStateSetAdvancedPremultipliedSrc(b=" << b << ") called!" << std::endl;
}

void nvnBlendStateSetAdvancedNormalizedDst(NVNblendState* blend, NVNboolean b) {
    std::cout << "nvnBlendStateSetAdvancedNormalizedDst(b=" << b << ") called!" << std::endl;
}

int nvnBlendStateGetBlendTarget(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetBlendTarget called!" << std::endl;
    return 0;
}

void nvnBlendStateGetBlendFunc(const NVNblendState* blend, NVNblendFunc* srcFunc, NVNblendFunc* dstFunc, NVNblendFunc* srcFuncAlpha, NVNblendFunc* dstFuncAlpha) {
    std::cout << "nvnBlendStateGetBlendFunc called!" << std::endl;
    if (srcFunc) *srcFunc = 0;
    if (dstFunc) *dstFunc = 0;
    if (srcFuncAlpha) *srcFuncAlpha = 0;
    if (dstFuncAlpha) *dstFuncAlpha = 0;
}

void nvnBlendStateGetBlendEquation(const NVNblendState* blend, NVNblendEquation* modeRGB, NVNblendEquation* modeAlpha) {
    std::cout << "nvnBlendStateGetBlendEquation called!" << std::endl;
    if (modeRGB) *modeRGB = 0;
    if (modeAlpha) *modeAlpha = 0;
}

NVNblendAdvancedMode nvnBlendStateGetAdvancedMode(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedMode called!" << std::endl;
    return 0;
}

NVNblendAdvancedOverlap nvnBlendStateGetAdvancedOverlap(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedOverlap called!" << std::endl;
    return 0;
}

NVNboolean nvnBlendStateGetAdvancedPremultipliedSrc(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedPremultipliedSrc called!" << std::endl;
    return 0;
}

NVNboolean nvnBlendStateGetAdvancedNormalizedDst(const NVNblendState* blend) {
    std::cout << "nvnBlendStateGetAdvancedNormalizedDst called!" << std::endl;
    return 0;
}