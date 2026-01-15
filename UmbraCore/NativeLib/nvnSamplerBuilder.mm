#include <iostream>
#include "nv.h"

void nvnSamplerBuilderSetDevice(NVNsamplerBuilder* _builder, NVNdevice* _device) {
    std::cout << "nvnSamplerBuilderSetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto device = UNWRAP(_device);
}

void nvnSamplerBuilderSetDefaults(NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetMinMagFilter(NVNsamplerBuilder* _builder, NVNminFilter min, NVNmagFilter mag) {
    std::cout << "nvnSamplerBuilderSetMinMagFilter called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetWrapMode(NVNsamplerBuilder* _builder, NVNwrapMode s, NVNwrapMode t, NVNwrapMode r) {
    std::cout << "nvnSamplerBuilderSetWrapMode called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetLodClamp(NVNsamplerBuilder* _builder, float min, float max) {
    std::cout << "nvnSamplerBuilderSetLodClamp called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetLodBias(NVNsamplerBuilder* _builder, float bias) {
    std::cout << "nvnSamplerBuilderSetLodBias called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetCompare(NVNsamplerBuilder* _builder, NVNcompareMode mode, NVNcompareFunc func) {
    std::cout << "nvnSamplerBuilderSetCompare called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetBorderColor(NVNsamplerBuilder* _builder, const float* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColor called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetBorderColori(NVNsamplerBuilder* _builder, const int* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColori called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetBorderColorui(NVNsamplerBuilder* _builder, const uint32_t* borderColor) {
    std::cout << "nvnSamplerBuilderSetBorderColorui called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetMaxAnisotropy(NVNsamplerBuilder* _builder, float maxAniso) {
    std::cout << "nvnSamplerBuilderSetMaxAnisotropy called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetReductionFilter(NVNsamplerBuilder* _builder, NVNsamplerReduction filter) {
    std::cout << "nvnSamplerBuilderSetReductionFilter called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderSetLodSnap(NVNsamplerBuilder* _builder, float f) {
    std::cout << "nvnSamplerBuilderSetLodSnap called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnSamplerBuilderGetMinMagFilter(NVNsamplerBuilder* _builder, NVNminFilter* min, NVNmagFilter* mag) {
    std::cout << "nvnSamplerBuilderGetMinMagFilter called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (min) *min = 0;
    if (mag) *mag = 0;
}

void nvnSamplerBuilderGetWrapMode(NVNsamplerBuilder* _builder, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r) {
    std::cout << "nvnSamplerBuilderGetWrapMode called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (s) *s = 0;
    if (t) *t = 0;
    if (r) *r = 0;
}

void nvnSamplerBuilderGetLodClamp(NVNsamplerBuilder* _builder, float* min, float* max) {
    std::cout << "nvnSamplerBuilderGetLodClamp called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (min) *min = 0.0f;
    if (max) *max = 0.0f;
}

float nvnSamplerBuilderGetLodBias(NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerBuilderGetLodBias called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0.0f;
}

void nvnSamplerBuilderGetCompare(NVNsamplerBuilder* _builder, NVNcompareMode* mode, NVNcompareFunc* func) {
    std::cout << "nvnSamplerBuilderGetCompare called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (mode) *mode = 0;
    if (func) *func = 0;
}

void nvnSamplerBuilderGetBorderColor(NVNsamplerBuilder* _builder, float* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColor called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (borderColor) {
        borderColor[0] = 0.0f;
        borderColor[1] = 0.0f;
        borderColor[2] = 0.0f;
        borderColor[3] = 0.0f;
    }
}

void nvnSamplerBuilderGetBorderColori(NVNsamplerBuilder* _builder, int* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColori called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

void nvnSamplerBuilderGetBorderColorui(NVNsamplerBuilder* _builder, uint32_t* borderColor) {
    std::cout << "nvnSamplerBuilderGetBorderColorui called!" << std::endl;
    auto builder = UNWRAP(_builder);
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

float nvnSamplerBuilderGetMaxAnisotropy(NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerBuilderGetMaxAnisotropy called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0.0f;
}

NVNsamplerReduction nvnSamplerBuilderGetReductionFilter(NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerBuilderGetReductionFilter called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

float nvnSamplerBuilderGetLodSnap(NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerBuilderGetLodSnap called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0.0f;
}
