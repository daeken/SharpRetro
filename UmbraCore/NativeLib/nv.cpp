#include "nv.h"

#include <iostream>

void nv::InitializeGraphics(void *, uint64_t) {
    std::cout << "nv::InitializeGraphics called!" << std::endl;
}

// nv::SetGraphicsAllocator(
//  void* (*)(unsigned long, unsigned long, void*),
//  void (*)(void*, void*),
//  void* (*)(void*, unsigned long, void*),
//  void*)
void nv::SetGraphicsAllocator(void *, void *, void *, void *) {
    std::cout << "nv::SetGraphicsAllocator called!" << std::endl;
}

void* nvnBootstrapLoader(const char *name) {
    std::cout << "nvnBootstrapLoader called! '" << name << "'" << std::endl;
    return nullptr;
}
