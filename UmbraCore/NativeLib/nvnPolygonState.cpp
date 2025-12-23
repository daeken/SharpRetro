#include <iostream>
#include "nv.h"

void nvnPolygonStateSetDefaults(NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateSetDefaults called!" << std::endl;
}

void nvnPolygonStateSetCullFace(NVNpolygonState* polygon, NVNface face) {
    std::cout << "nvnPolygonStateSetCullFace called!" << std::endl;
}

void nvnPolygonStateSetFrontFace(NVNpolygonState* polygon, NVNfrontFace face) {
    std::cout << "nvnPolygonStateSetFrontFace called!" << std::endl;
}

void nvnPolygonStateSetPolygonMode(NVNpolygonState* polygon, NVNpolygonMode polygonMode) {
    std::cout << "nvnPolygonStateSetPolygonMode called!" << std::endl;
}

void nvnPolygonStateSetPolygonOffsetEnables(NVNpolygonState* polygon, int enables) {
    std::cout << "nvnPolygonStateSetPolygonOffsetEnables called!" << std::endl;
}

NVNface nvnPolygonStateGetCullFace(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetCullFace called!" << std::endl;
    return 0;
}

NVNfrontFace nvnPolygonStateGetFrontFace(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetFrontFace called!" << std::endl;
    return 0;
}

NVNpolygonMode nvnPolygonStateGetPolygonMode(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetPolygonMode called!" << std::endl;
    return 0;
}

NVNpolygonOffsetEnable nvnPolygonStateGetPolygonOffsetEnables(const NVNpolygonState* polygon) {
    std::cout << "nvnPolygonStateGetPolygonOffsetEnables called!" << std::endl;
    return 0;
}