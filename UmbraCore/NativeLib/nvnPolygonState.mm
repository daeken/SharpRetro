#include <iostream>
#include "nv.h"

void nvnPolygonStateSetDefaults(NVNpolygonState* _polygon) {
    std::cout << "nvnPolygonStateSetDefaults called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
}

void nvnPolygonStateSetCullFace(NVNpolygonState* _polygon, NVNface face) {
    std::cout << "nvnPolygonStateSetCullFace called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
}

void nvnPolygonStateSetFrontFace(NVNpolygonState* _polygon, NVNfrontFace face) {
    std::cout << "nvnPolygonStateSetFrontFace called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
}

void nvnPolygonStateSetPolygonMode(NVNpolygonState* _polygon, NVNpolygonMode polygonMode) {
    std::cout << "nvnPolygonStateSetPolygonMode called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
}

void nvnPolygonStateSetPolygonOffsetEnables(NVNpolygonState* _polygon, int enables) {
    std::cout << "nvnPolygonStateSetPolygonOffsetEnables called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
}

NVNface nvnPolygonStateGetCullFace(NVNpolygonState* _polygon) {
    std::cout << "nvnPolygonStateGetCullFace called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
    return 0;
}

NVNfrontFace nvnPolygonStateGetFrontFace(NVNpolygonState* _polygon) {
    std::cout << "nvnPolygonStateGetFrontFace called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
    return 0;
}

NVNpolygonMode nvnPolygonStateGetPolygonMode(NVNpolygonState* _polygon) {
    std::cout << "nvnPolygonStateGetPolygonMode called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
    return 0;
}

NVNpolygonOffsetEnable nvnPolygonStateGetPolygonOffsetEnables(NVNpolygonState* _polygon) {
    std::cout << "nvnPolygonStateGetPolygonOffsetEnables called!" << std::endl;
    auto polygon = UNWRAP(_polygon);
    return 0;
}
