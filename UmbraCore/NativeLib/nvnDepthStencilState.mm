#include <iostream>
#include "nv.h"

void nvnDepthStencilStateSetDefaults(NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateSetDefaults called!" << std::endl;
}

void nvnDepthStencilStateSetDepthTestEnable(NVNdepthStencilState* depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetDepthTestEnable called!" << std::endl;
}

void nvnDepthStencilStateSetDepthWriteEnable(NVNdepthStencilState* depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetDepthWriteEnable called!" << std::endl;
}

void nvnDepthStencilStateSetDepthFunc(NVNdepthStencilState* depthStencil, NVNdepthFunc func) {
    std::cout << "nvnDepthStencilStateSetDepthFunc called!" << std::endl;
}

void nvnDepthStencilStateSetStencilTestEnable(NVNdepthStencilState* depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetStencilTestEnable called!" << std::endl;
}

void nvnDepthStencilStateSetStencilFunc(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilFunc func) {
    std::cout << "nvnDepthStencilStateSetStencilFunc called!" << std::endl;
}

void nvnDepthStencilStateSetStencilOp(NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilOp fail, NVNstencilOp depthFail, NVNstencilOp depthPass) {
    std::cout << "nvnDepthStencilStateSetStencilOp called!" << std::endl;
}

NVNboolean nvnDepthStencilStateGetDepthTestEnable(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthTestEnable called!" << std::endl;
    return 0;
}

NVNboolean nvnDepthStencilStateGetDepthWriteEnable(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthWriteEnable called!" << std::endl;
    return 0;
}

NVNdepthFunc nvnDepthStencilStateGetDepthFunc(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthFunc called!" << std::endl;
    return 0;
}

NVNboolean nvnDepthStencilStateGetStencilTestEnable(const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnDepthStencilStateGetStencilTestEnable called!" << std::endl;
    return 0;
}

NVNstencilFunc nvnDepthStencilStateGetStencilFunc(const NVNdepthStencilState* depthStencil, NVNface faces) {
    std::cout << "nvnDepthStencilStateGetStencilFunc called!" << std::endl;
    return 0;
}

void nvnDepthStencilStateGetStencilOp(const NVNdepthStencilState* depthStencil, NVNface faces, NVNstencilOp* fail, NVNstencilOp* depthFail, NVNstencilOp* depthPass) {
    std::cout << "nvnDepthStencilStateGetStencilOp called!" << std::endl;
    if (fail) *fail = 0;
    if (depthFail) *depthFail = 0;
    if (depthPass) *depthPass = 0;
}