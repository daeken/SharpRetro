#include <iostream>
#include "nv.h"

void nvnColorStateSetDefaults(NVNcolorState* color) {
    std::cout << "nvnColorStateSetDefaults called!" << std::endl;
}

void nvnColorStateSetBlendEnable(NVNcolorState* color, int index, NVNboolean enable) {
    std::cout << "nvnColorStateSetBlendEnable called!" << std::endl;
}

void nvnColorStateSetLogicOp(NVNcolorState* color, NVNlogicOp logicOp) {
    std::cout << "nvnColorStateSetLogicOp called!" << std::endl;
}

void nvnColorStateSetAlphaTest(NVNcolorState* color, NVNalphaFunc alphaFunc) {
    std::cout << "nvnColorStateSetAlphaTest called!" << std::endl;
}

NVNboolean nvnColorStateGetBlendEnable(const NVNcolorState* color, int index) {
    std::cout << "nvnColorStateGetBlendEnable called!" << std::endl;
    return 0;
}

NVNlogicOp nvnColorStateGetLogicOp(const NVNcolorState* color) {
    std::cout << "nvnColorStateGetLogicOp called!" << std::endl;
    return 0;
}

NVNalphaFunc nvnColorStateGetAlphaTest(const NVNcolorState* color) {
    std::cout << "nvnColorStateGetAlphaTest called!" << std::endl;
    return 0;
}