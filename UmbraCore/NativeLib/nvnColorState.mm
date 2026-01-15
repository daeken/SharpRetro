#include <iostream>
#include "nv.h"

void nvnColorStateSetDefaults(NVNcolorState* _color) {
    std::cout << "nvnColorStateSetDefaults called!" << std::endl;
    auto color = UNWRAP(_color);
}

void nvnColorStateSetBlendEnable(NVNcolorState* _color, int index, NVNboolean enable) {
    std::cout << "nvnColorStateSetBlendEnable called!" << std::endl;
    auto color = UNWRAP(_color);
}

void nvnColorStateSetLogicOp(NVNcolorState* _color, NVNlogicOp logicOp) {
    std::cout << "nvnColorStateSetLogicOp called!" << std::endl;
    auto color = UNWRAP(_color);
}

void nvnColorStateSetAlphaTest(NVNcolorState* _color, NVNalphaFunc alphaFunc) {
    std::cout << "nvnColorStateSetAlphaTest called!" << std::endl;
    auto color = UNWRAP(_color);
}

NVNboolean nvnColorStateGetBlendEnable(NVNcolorState* _color, int index) {
    std::cout << "nvnColorStateGetBlendEnable called!" << std::endl;
    auto color = UNWRAP(_color);
    return 0;
}

NVNlogicOp nvnColorStateGetLogicOp(NVNcolorState* _color) {
    std::cout << "nvnColorStateGetLogicOp called!" << std::endl;
    auto color = UNWRAP(_color);
    return 0;
}

NVNalphaFunc nvnColorStateGetAlphaTest(NVNcolorState* _color) {
    std::cout << "nvnColorStateGetAlphaTest called!" << std::endl;
    auto color = UNWRAP(_color);
    return 0;
}
