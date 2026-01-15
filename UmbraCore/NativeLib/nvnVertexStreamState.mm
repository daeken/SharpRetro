#include <iostream>
#include "nv.h"

void nvnVertexStreamStateSetDefaults(NVNvertexStreamState* _stream) {
    std::cout << "nvnVertexStreamStateSetDefaults called!" << std::endl;
    auto stream = UNWRAP(_stream);
}

void nvnVertexStreamStateSetStride(NVNvertexStreamState* _stream, ptrdiff_t stride) {
    std::cout << "nvnVertexStreamStateSetStride called!" << std::endl;
    auto stream = UNWRAP(_stream);
}

void nvnVertexStreamStateSetDivisor(NVNvertexStreamState* _stream, int divisor) {
    std::cout << "nvnVertexStreamStateSetDivisor called!" << std::endl;
    auto stream = UNWRAP(_stream);
}

ptrdiff_t nvnVertexStreamStateGetStride(NVNvertexStreamState* _stream) {
    std::cout << "nvnVertexStreamStateGetStride called!" << std::endl;
    auto stream = UNWRAP(_stream);
    return 0;
}

int nvnVertexStreamStateGetDivisor(NVNvertexStreamState* _stream) {
    std::cout << "nvnVertexStreamStateGetDivisor called!" << std::endl;
    auto stream = UNWRAP(_stream);
    return 0;
}
