#include <iostream>
#include "nv.h"

void nvnDepthStencilStateSetDefaults(NVNdepthStencilState* _depthStencil) {
    std::cout << "nvnDepthStencilStateSetDefaults called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnDepthStencilStateSetDepthTestEnable(NVNdepthStencilState* _depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetDepthTestEnable called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnDepthStencilStateSetDepthWriteEnable(NVNdepthStencilState* _depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetDepthWriteEnable called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnDepthStencilStateSetDepthFunc(NVNdepthStencilState* _depthStencil, NVNdepthFunc func) {
    std::cout << "nvnDepthStencilStateSetDepthFunc called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnDepthStencilStateSetStencilTestEnable(NVNdepthStencilState* _depthStencil, NVNboolean enable) {
    std::cout << "nvnDepthStencilStateSetStencilTestEnable called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnDepthStencilStateSetStencilFunc(NVNdepthStencilState* _depthStencil, NVNface faces, NVNstencilFunc func) {
    std::cout << "nvnDepthStencilStateSetStencilFunc called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnDepthStencilStateSetStencilOp(NVNdepthStencilState* _depthStencil, NVNface faces, NVNstencilOp fail, NVNstencilOp depthFail, NVNstencilOp depthPass) {
    std::cout << "nvnDepthStencilStateSetStencilOp called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
}

NVNboolean nvnDepthStencilStateGetDepthTestEnable(NVNdepthStencilState* _depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthTestEnable called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
    return 0;
}

NVNboolean nvnDepthStencilStateGetDepthWriteEnable(NVNdepthStencilState* _depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthWriteEnable called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
    return 0;
}

NVNdepthFunc nvnDepthStencilStateGetDepthFunc(NVNdepthStencilState* _depthStencil) {
    std::cout << "nvnDepthStencilStateGetDepthFunc called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
    return 0;
}

NVNboolean nvnDepthStencilStateGetStencilTestEnable(NVNdepthStencilState* _depthStencil) {
    std::cout << "nvnDepthStencilStateGetStencilTestEnable called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
    return 0;
}

NVNstencilFunc nvnDepthStencilStateGetStencilFunc(NVNdepthStencilState* _depthStencil, NVNface faces) {
    std::cout << "nvnDepthStencilStateGetStencilFunc called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
    return 0;
}

void nvnDepthStencilStateGetStencilOp(NVNdepthStencilState* _depthStencil, NVNface faces, NVNstencilOp* fail, NVNstencilOp* depthFail, NVNstencilOp* depthPass) {
    std::cout << "nvnDepthStencilStateGetStencilOp called!" << std::endl;
    auto depthStencil = UNWRAP(_depthStencil);
    if (fail) *fail = 0;
    if (depthFail) *depthFail = 0;
    if (depthPass) *depthPass = 0;
}
