#include <iostream>
#include "nv.h"

NVNboolean nvnSamplerInitialize(NVNsampler* sampler, const NVNsamplerBuilder* builder) {
    std::cout << "nvnSamplerInitialize called!" << std::endl;
    return 1;
}

void nvnSamplerFinalize(NVNsampler* sampler) {
    std::cout << "nvnSamplerFinalize called!" << std::endl;
}

void nvnSamplerSetDebugLabel(NVNsampler* sampler, const char* label) {
    std::cout << "nvnSamplerSetDebugLabel called!" << std::endl;
}

void nvnSamplerGetMinMagFilter(const NVNsampler* sampler, NVNminFilter* min, NVNmagFilter* mag) {
    std::cout << "nvnSamplerGetMinMagFilter called!" << std::endl;
    if (min) *min = 0;
    if (mag) *mag = 0;
}

void nvnSamplerGetWrapMode(const NVNsampler* sampler, NVNwrapMode* s, NVNwrapMode* t, NVNwrapMode* r) {
    std::cout << "nvnSamplerGetWrapMode called!" << std::endl;
    if (s) *s = 0;
    if (t) *t = 0;
    if (r) *r = 0;
}

void nvnSamplerGetLodClamp(const NVNsampler* sampler, float* min, float* max) {
    std::cout << "nvnSamplerGetLodClamp called!" << std::endl;
    if (min) *min = 0.0f;
    if (max) *max = 0.0f;
}

float nvnSamplerGetLodBias(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetLodBias called!" << std::endl;
    return 0.0f;
}

void nvnSamplerGetCompare(const NVNsampler* sampler, NVNcompareMode* mode, NVNcompareFunc* func) {
    std::cout << "nvnSamplerGetCompare called!" << std::endl;
    if (mode) *mode = 0;
    if (func) *func = 0;
}

void nvnSamplerGetBorderColor(const NVNsampler* sampler, float* borderColor) {
    std::cout << "nvnSamplerGetBorderColor called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0.0f;
        borderColor[1] = 0.0f;
        borderColor[2] = 0.0f;
        borderColor[3] = 0.0f;
    }
}

void nvnSamplerGetBorderColori(const NVNsampler* sampler, int* borderColor) {
    std::cout << "nvnSamplerGetBorderColori called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

void nvnSamplerGetBorderColorui(const NVNsampler* sampler, uint32_t* borderColor) {
    std::cout << "nvnSamplerGetBorderColorui called!" << std::endl;
    if (borderColor) {
        borderColor[0] = 0;
        borderColor[1] = 0;
        borderColor[2] = 0;
        borderColor[3] = 0;
    }
}

float nvnSamplerGetMaxAnisotropy(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetMaxAnisotropy called!" << std::endl;
    return 0.0f;
}

NVNsamplerReduction nvnSamplerGetReductionFilter(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetReductionFilter called!" << std::endl;
    return 0;
}

NVNboolean nvnSamplerCompare(const NVNsampler* sampler1, const NVNsampler* sampler2) {
    std::cout << "nvnSamplerCompare called!" << std::endl;
    return 0;
}

uint64_t nvnSamplerGetDebugID(const NVNsampler* sampler) {
    std::cout << "nvnSamplerGetDebugID called!" << std::endl;
    return 0;
}