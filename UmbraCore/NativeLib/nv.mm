#include "nv.h"
#include <iostream>
#include <cstring>

void nv::InitializeGraphics(void*, uint64_t) {
    std::cout << "nv::InitializeGraphics called!" << std::endl;
}

void nv::SetGraphicsAllocator(void*, void*, void*, void*) {
    std::cout << "nv::SetGraphicsAllocator called!" << std::endl;
}

