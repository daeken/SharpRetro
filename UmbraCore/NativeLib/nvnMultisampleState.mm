#include <iostream>
#include "nv.h"

void nvnMultisampleStateSetDefaults(NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateSetDefaults called!" << std::endl;
}

void nvnMultisampleStateSetMultisampleEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetMultisampleEnable called!" << std::endl;
}

void nvnMultisampleStateSetSamples(NVNmultisampleState* multisample, int samples) {
    std::cout << "nvnMultisampleStateSetSamples called!" << std::endl;
}

void nvnMultisampleStateSetAlphaToCoverageEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetAlphaToCoverageEnable called!" << std::endl;
}

void nvnMultisampleStateSetAlphaToCoverageDither(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetAlphaToCoverageDither called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetMultisampleEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetMultisampleEnable called!" << std::endl;
    return 0;
}

int nvnMultisampleStateGetSamples(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetSamples called!" << std::endl;
    return 0;
}

NVNboolean nvnMultisampleStateGetAlphaToCoverageEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetAlphaToCoverageEnable called!" << std::endl;
    return 0;
}

NVNboolean nvnMultisampleStateGetAlphaToCoverageDither(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetAlphaToCoverageDither called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetRasterSamples(NVNmultisampleState* multisample, int samples) {
    std::cout << "nvnMultisampleStateSetRasterSamples called!" << std::endl;
}

int nvnMultisampleStateGetRasterSamples(NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetRasterSamples called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetCoverageModulationMode(NVNmultisampleState* multisample, NVNcoverageModulationMode mode) {
    std::cout << "nvnMultisampleStateSetCoverageModulationMode called!" << std::endl;
}

NVNcoverageModulationMode nvnMultisampleStateGetCoverageModulationMode(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetCoverageModulationMode called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetCoverageToColorEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetCoverageToColorEnable called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetCoverageToColorEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetCoverageToColorEnable called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetCoverageToColorOutput(NVNmultisampleState* multisample, int i) {
    std::cout << "nvnMultisampleStateSetCoverageToColorOutput called!" << std::endl;
}

int nvnMultisampleStateGetCoverageToColorOutput(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetCoverageToColorOutput called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetSampleLocationsEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetSampleLocationsEnable called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetSampleLocationsEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetSampleLocationsEnable called!" << std::endl;
    return 0;
}

void nvnMultisampleStateGetSampleLocationsGrid(NVNmultisampleState* multisample, int* w, int* h) {
    std::cout << "nvnMultisampleStateGetSampleLocationsGrid called!" << std::endl;
    if (w) *w = 0;
    if (h) *h = 0;
}

void nvnMultisampleStateSetSampleLocationsGridEnable(NVNmultisampleState* multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetSampleLocationsGridEnable called!" << std::endl;
}

NVNboolean nvnMultisampleStateGetSampleLocationsGridEnable(const NVNmultisampleState* multisample) {
    std::cout << "nvnMultisampleStateGetSampleLocationsGridEnable called!" << std::endl;
    return 0;
}

void nvnMultisampleStateSetSampleLocations(NVNmultisampleState* multisample, int i1, int i2, const float* f) {
    std::cout << "nvnMultisampleStateSetSampleLocations called!" << std::endl;
}