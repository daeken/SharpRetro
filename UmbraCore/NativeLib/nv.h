#ifndef NATIVELIB_NV_H
#define NATIVELIB_NV_H
#include <cstdint>

class nv {
public:
    static void InitializeGraphics(void*, uint64_t);
    static void SetGraphicsAllocator(void*, void*, void*, void*);
};

void* nvnBootstrapLoader(const char*);

#endif //NATIVELIB_NV_H