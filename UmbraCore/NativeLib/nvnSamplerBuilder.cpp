#include <iostream>
#include "nv.h"

void nvnSamplerBuilderSetDevice(NVNsamplerBuilder* builder, NVNdevice* device) {
    std::cout << "nvnSamplerBuilderSetDevice called!" << std::endl;
}

void nvnSamplerBuilderSetDefaults(NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderSetDefaults called!" << std::endl;
}

void nvnSamplerBuilderSetMinMagFilter(NVNsamplerBuilder* builder, NVNminFilter min, NVNmagFilter mag) {
    std::cout << "nvnSamplerBuilderSetMinMagFilter called!" << std::endl;
}

void nvnSamplerBuilderSetWrapMode(NVNsamplerBuilder* builder, NVNwrapMode s, NVNwrapMode t, NVNwrapMode r) {
    std::cout << "nvnSamplerBuilderSetWrapMode called!" << std::endl;
}

void nvnSamplerBuilderSetLodClamp(NVNsamplerBuilder* builder, float min, float max) {
    std::cout << "nvnSamplerBuilderSetLodClamp called!" << std::endl;
}

void nvnSamplerBuilderSetLodBias(NVNsamplerBuilder* builder, float bias) {
    std::cout << "nvnSamplerBuilderSetLodBias called!" << std::endl;
}

void nvnSamplerBuilderSetCompare(NVNsamplerBuilder* builder, NVNcompareMode mode, NVNcompareFunc func) {
    std::cout << "nvnSamplerBuilderSetCompare called!" << std::endl;
}

void nvnSamplerBuilderSetBorderColor(NVNsamplerBuilder* builder, const float* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColor called!" << std::endl;
}

void nvnSamplerBuilderSetBorderColori(NVNsamplerBuilder* builder, const int* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColori called!" << std::endl;
}

void nvnSamplerBuilderSetBorderColorui(NVNsamplerBuilder* builder, const uint32_t* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColorui called!" << std::endl;
}

void nvnSamplerBuilderSetMaxAnisotropy(NVNsamplerBuilder* builder, float maxAniso) {
    std::cout << "nvnSamplerBuilderSetMaxAnisotropy called!" << std::endl;
}

void nvnSamplerBuilderSetReductionFilter(NVNsamplerBuilder* builder, NVNsamplerReduction filter) {
    std::cout << "nvnSamplerBuilderSetReductionFilter called!" << std::endl;
}

void nvnSamplerBuilderSetLodSnap(NVNsamplerBuilder* builder, float f) {
    std::cout << "nvnSamplerBuilderSetLodSnap called!" << std::endl;
}

void nvnSamplerBuilderGetMinMagFilter(const NVNsamplerBuilder* builder, NVNminFilter* min, NVNmagFilter* mag) {
    std::cout << "nvnSamplerBuilderGetMinMagFilter called!" << std::endl;
    if (min) *min = 0;
    if (mag) *mag = 0;
}

void nvnSamplerBuilderGetWrapMode(const NVNsamplerBuilder* builder, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r) {
    std::cout << "nvnSamplerBuilderGetWrapMode called!" << std::endl;
    if (s) *s = 0;
    if (t) *t = 0;
    if (r) *r = 0;
}

void nvnSamplerBuilderGetLodClamp(const NVNsamplerBuilder* builder, float* min, float* max) {
    std::cout << "nvnSamplerBuilderGetLodClamp called!" << std::endl;
    if (min) *min = 0.0f;
    if (max) *max = 0.0f;
}

float nvnSamplerBuilderGetLodBias(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetLodBias called!" << std::endl;
    return 0.0f;
}

void nvnSamplerBuilderGetCompare(const NVNsamplerBuilder* builder, NVNcompareMode* mode, NVNcompareFunc* func) {
    std::cout << "nvnSamplerBuilderGetCompare called!" << std::endl;
    if (mode) *mode = 0;
    if (func) *func = 0;
}

void nvnSamplerBuilderGetBorderColor(const NVNsamplerBuilder* builder, float* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColor called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0.0f;
        borderColor[1] = 0.0f;
        borderColor[2] = 0.0f;
        borderColor[3] = 0.0f;
    }
}

void nvnSamplerBuilderGetBorderColori(const NVNsamplerBuilder* builder, int* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColori called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

void nvnSamplerBuilderGetBorderColorui(const NVNsamplerBuilder* builder, uint32_t* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColorui called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

float nvnSamplerBuilderGetMaxAnisotropy(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetMaxAnisotropy called!" << std::endl;
    return 0.0f;
}

NVNsamplerReduction nvnSamplerBuilderGetReductionFilter(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetReductionFilter called!" << std::endl;
    return 0;
}

float nvnSamplerBuilderGetLodSnap(const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerBuilderGetLodSnap called!" << std::endl;
    return 0.0f;
}