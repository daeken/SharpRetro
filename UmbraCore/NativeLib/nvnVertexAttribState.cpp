#include <iostream>
#include "nv.h"

void nvnVertexAttribStateSetDefaults(NVNvertexAttribState* attrib) {
    std::cout << "nvnVertexAttribStateSetDefaults called!" << std::endl;
}

void nvnVertexAttribStateSetFormat(NVNvertexAttribState* attrib, NVNformat format, ptrdiff_t relativeOffset) {
    std::cout << "nvnVertexAttribStateSetFormat called!" << std::endl;
}

void nvnVertexAttribStateSetStreamIndex(NVNvertexAttribState* attrib, int streamIndex) {
    std::cout << "nvnVertexAttribStateSetStreamIndex called!" << std::endl;
}

void nvnVertexAttribStateGetFormat(const NVNvertexAttribState* attrib, NVNformat* format, ptrdiff_t* relativeOffset) {
    std::cout << "nvnVertexAttribStateGetFormat called!" << std::endl;
    if (format) *format = 0;
    if (relativeOffset) *relativeOffset = 0;
}

int nvnVertexAttribStateGetStreamIndex(const NVNvertexAttribState* attrib) {
    std::cout << "nvnVertexAttribStateGetStreamIndex called!" << std::endl;
    return 0;
}