#include <iostream>
#include "nv.h"

void nvnMultisampleStateSetDefaults(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateSetDefaults called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

void nvnMultisampleStateSetMultisampleEnable(NVNmultisampleState* _multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetMultisampleEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

void nvnMultisampleStateSetSamples(NVNmultisampleState* _multisample, int samples) {
    std::cout << "nvnMultisampleStateSetSamples called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

void nvnMultisampleStateSetAlphaToCoverageEnable(NVNmultisampleState* _multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetAlphaToCoverageEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

void nvnMultisampleStateSetAlphaToCoverageDither(NVNmultisampleState* _multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetAlphaToCoverageDither called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

NVNboolean nvnMultisampleStateGetMultisampleEnable(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetMultisampleEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

int nvnMultisampleStateGetSamples(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetSamples called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

NVNboolean nvnMultisampleStateGetAlphaToCoverageEnable(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetAlphaToCoverageEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

NVNboolean nvnMultisampleStateGetAlphaToCoverageDither(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetAlphaToCoverageDither called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateSetRasterSamples(NVNmultisampleState* _multisample, int samples) {
    std::cout << "nvnMultisampleStateSetRasterSamples called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

int nvnMultisampleStateGetRasterSamples(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetRasterSamples called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateSetCoverageModulationMode(NVNmultisampleState* _multisample, NVNcoverageModulationMode mode) {
    std::cout << "nvnMultisampleStateSetCoverageModulationMode called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

NVNcoverageModulationMode nvnMultisampleStateGetCoverageModulationMode(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetCoverageModulationMode called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateSetCoverageToColorEnable(NVNmultisampleState* _multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetCoverageToColorEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

NVNboolean nvnMultisampleStateGetCoverageToColorEnable(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetCoverageToColorEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateSetCoverageToColorOutput(NVNmultisampleState* _multisample, int i) {
    std::cout << "nvnMultisampleStateSetCoverageToColorOutput called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

int nvnMultisampleStateGetCoverageToColorOutput(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetCoverageToColorOutput called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateSetSampleLocationsEnable(NVNmultisampleState* _multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetSampleLocationsEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

NVNboolean nvnMultisampleStateGetSampleLocationsEnable(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetSampleLocationsEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateGetSampleLocationsGrid(NVNmultisampleState* _multisample, int* w, int* h) {
    std::cout << "nvnMultisampleStateGetSampleLocationsGrid called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    if (w) *w = 0;
    if (h) *h = 0;
}

void nvnMultisampleStateSetSampleLocationsGridEnable(NVNmultisampleState* _multisample, NVNboolean enable) {
    std::cout << "nvnMultisampleStateSetSampleLocationsGridEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}

NVNboolean nvnMultisampleStateGetSampleLocationsGridEnable(NVNmultisampleState* _multisample) {
    std::cout << "nvnMultisampleStateGetSampleLocationsGridEnable called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
    return 0;
}

void nvnMultisampleStateSetSampleLocations(NVNmultisampleState* _multisample, int i1, int i2, const float* f) {
    std::cout << "nvnMultisampleStateSetSampleLocations called!" << std::endl;
    auto multisample = UNWRAP(_multisample);
}
