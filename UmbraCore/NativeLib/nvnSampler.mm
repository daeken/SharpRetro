#include <iostream>
#include "nv.h"

NVNboolean nvnSamplerInitialize(NVNsampler* _sampler, NVNsamplerBuilder* _builder) {
    std::cout << "nvnSamplerInitialize called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    auto builder = UNWRAP(_builder);
    return 1;
}

void nvnSamplerFinalize(NVNsampler* _sampler) {
    std::cout << "nvnSamplerFinalize called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
}

void nvnSamplerSetDebugLabel(NVNsampler* _sampler, const char* label) {
    std::cout << "nvnSamplerSetDebugLabel called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
}

void nvnSamplerGetMinMagFilter(NVNsampler* _sampler, NVNminFilter* min, NVNmagFilter* mag) {
    std::cout << "nvnSamplerGetMinMagFilter called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (min) *min = 0;
    if (mag) *mag = 0;
}

void nvnSamplerGetWrapMode(NVNsampler* _sampler, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r) {
    std::cout << "nvnSamplerGetWrapMode called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (s) *s = 0;
    if (t) *t = 0;
    if (r) *r = 0;
}

void nvnSamplerGetLodClamp(NVNsampler* _sampler, float* min, float* max) {
    std::cout << "nvnSamplerGetLodClamp called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (min) *min = 0.0f;
    if (max) *max = 0.0f;
}

float nvnSamplerGetLodBias(NVNsampler* _sampler) {
    std::cout << "nvnSamplerGetLodBias called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    return 0.0f;
}

void nvnSamplerGetCompare(NVNsampler* _sampler, NVNcompareMode* mode, NVNcompareFunc* func) {
    std::cout << "nvnSamplerGetCompare called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (mode) *mode = 0;
    if (func) *func = 0;
}

void nvnSamplerGetBorderColor(NVNsampler* _sampler, float* borderColor) {
    std::cout << "nvnSamplerGetBorderColor called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (borderColor) {
        borderColor[0] = 0.0f;
        borderColor[1] = 0.0f;
        borderColor[2] = 0.0f;
        borderColor[3] = 0.0f;
    }
}

void nvnSamplerGetBorderColori(NVNsampler* _sampler, int* borderColor) {
    std::cout << "nvnSamplerGetBorderColori called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

void nvnSamplerGetBorderColorui(NVNsampler* _sampler, uint32_t* borderColor) {
    std::cout << "nvnSamplerGetBorderColorui called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

float nvnSamplerGetMaxAnisotropy(NVNsampler* _sampler) {
    std::cout << "nvnSamplerGetMaxAnisotropy called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    return 0.0f;
}

NVNsamplerReduction nvnSamplerGetReductionFilter(NVNsampler* _sampler) {
    std::cout << "nvnSamplerGetReductionFilter called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    return 0;
}

NVNboolean nvnSamplerCompare(NVNsampler* _sampler1, NVNsampler* _sampler2) {
    std::cout << "nvnSamplerCompare called!" << std::endl;
    auto sampler1 = UNWRAP(_sampler1);
    auto sampler2 = UNWRAP(_sampler2);
    return 0;
}

uint64_t nvnSamplerGetDebugID(NVNsampler* _sampler) {
    std::cout << "nvnSamplerGetDebugID called!" << std::endl;
    auto sampler = UNWRAP(_sampler);
    return 0;
}
