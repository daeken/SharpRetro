#include <iostream>
#include "nv.h"

void nvnVertexAttribStateSetDefaults(NVNvertexAttribState* _attrib) {
    std::cout << "nvnVertexAttribStateSetDefaults called!" << std::endl;
    auto attrib = UNWRAP(_attrib);
}

void nvnVertexAttribStateSetFormat(NVNvertexAttribState* _attrib, NVNformat format, ptrdiff_t relativeOffset) {
    std::cout << "nvnVertexAttribStateSetFormat called!" << std::endl;
    auto attrib = UNWRAP(_attrib);
}

void nvnVertexAttribStateSetStreamIndex(NVNvertexAttribState* _attrib, int streamIndex) {
    std::cout << "nvnVertexAttribStateSetStreamIndex called!" << std::endl;
    auto attrib = UNWRAP(_attrib);
}

void nvnVertexAttribStateGetFormat(NVNvertexAttribState* _attrib, NVNformat* format, ptrdiff_t* relativeOffset) {
    std::cout << "nvnVertexAttribStateGetFormat called!" << std::endl;
    auto attrib = UNWRAP(_attrib);
    if (format) *format = 0;
    if (relativeOffset) *relativeOffset = 0;
}

int nvnVertexAttribStateGetStreamIndex(NVNvertexAttribState* _attrib) {
    std::cout << "nvnVertexAttribStateGetStreamIndex called!" << std::endl;
    auto attrib = UNWRAP(_attrib);
    return 0;
}
