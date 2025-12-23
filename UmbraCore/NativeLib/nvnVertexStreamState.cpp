#include <iostream>
#include "nv.h"

void nvnVertexStreamStateSetDefaults(NVNvertexStreamState* stream) {
    std::cout << "nvnVertexStreamStateSetDefaults called!" << std::endl;
}

void nvnVertexStreamStateSetStride(NVNvertexStreamState* stream, ptrdiff_t stride) {
    std::cout << "nvnVertexStreamStateSetStride called!" << std::endl;
}

void nvnVertexStreamStateSetDivisor(NVNvertexStreamState* stream, int divisor) {
    std::cout << "nvnVertexStreamStateSetDivisor called!" << std::endl;
}

ptrdiff_t nvnVertexStreamStateGetStride(const NVNvertexStreamState* stream) {
    std::cout << "nvnVertexStreamStateGetStride called!" << std::endl;
    return 0;
}

int nvnVertexStreamStateGetDivisor(const NVNvertexStreamState* stream) {
    std::cout << "nvnVertexStreamStateGetDivisor called!" << std::endl;
    return 0;
}