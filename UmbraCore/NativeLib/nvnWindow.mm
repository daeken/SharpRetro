#include <iostream>
#include "nv.h"

void nvnWindowBuilderSetDevice(NVNwindowBuilder* builder, NVNdevice* device) {
    std::cout << "nvnWindowBuilderSetDevice called!" << std::endl;
}

void nvnWindowBuilderSetDefaults(NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderSetDefaults called!" << std::endl;
}

void nvnWindowBuilderSetNativeWindow(NVNwindowBuilder* builder, NVNnativeWindow nativeWindow) {
    std::cout << "nvnWindowBuilderSetNativeWindow called!" << std::endl;
}

int nvnWindowBuilderGetNumTextures(NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderGetNumTextures() called!" << std::endl;
    return 0;
}

NVNtexture* nvnWindowBuilderGetTexture(NVNwindowBuilder* builder, int texture) {
    std::cout << "nvnWindowBuilderGetTexture() called!" << std::endl;
    return nullptr;
}

void nvnWindowBuilderSetTextures(NVNwindowBuilder* builder, int numTextures, NVNtexture* const* textures) {
    std::cout << "nvnWindowBuilderSetTextures(numTextures=" << numTextures << ", textures=" << std::hex << reinterpret_cast<uint64_t>(textures) << std::dec << ") called!" << std::endl;
}

int nvnWindowBuilderGetNumActiveTextures(NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderGetNumActiveTextures() called!" << std::endl;
    return 0;
}

void nvnWindowBuilderSetNumActiveTextures(NVNwindowBuilder* builder, int numActiveTextures) {
    std::cout << "nvnWindowBuilderSetNumActiveTextures(numTextures=" << numActiveTextures << ") called!" << std::endl;
}

void nvnWindowBuilderSetPresentInterval(NVNwindowBuilder* builder, int presentInterval) {
    std::cout << "nvnWindowBuilderSetPresentInterval(presentInterval=" << presentInterval << ") called!" << std::endl;
}

NVNnativeWindow nvnWindowBuilderGetNativeWindow(const NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderGetNativeWindow() called!" << std::endl;
    return 0;
}

int nvnWindowBuilderGetPresentInterval(const NVNwindowBuilder* builder) {
    std::cout << "nvnWindowBuilderGetPresentInterval() called!" << std::endl;
    return 0;
}

NVNboolean nvnWindowInitialize(NVNwindow* window, const NVNwindowBuilder* builder) {
    std::cout << "nvnWindowInitialize called!" << std::endl;
    return 1;
}

void nvnWindowFinalize(NVNwindow* window) {
    std::cout << "nvnWindowFinalize called!" << std::endl;
}

void nvnWindowSetDebugLabel(NVNwindow* window, const char* label) {
    std::cout << "nvnWindowSetDebugLabel called!" << std::endl;
}

NVNwindowAcquireTextureResult nvnWindowAcquireTexture(NVNwindow* window, NVNsync* textureAvailableSync, int* textureIndex) {
    std::cout << "nvnWindowAcquireTexture(textureAvailableSync=" << std::hex << reinterpret_cast<uint64_t>(textureAvailableSync) << std::dec << ") called!" << std::endl;
    if (textureIndex) *textureIndex = 0;
    return 0;
}

NVNnativeWindow nvnWindowGetNativeWindow(const NVNwindow* window) {
    std::cout << "nvnWindowGetNativeWindow() called!" << std::endl;
    return 0;
}

int nvnWindowGetPresentInterval(const NVNwindow* window) {
    std::cout << "nvnWindowGetPresentInterval() called!" << std::endl;
    return 0;
}

void nvnWindowSetPresentInterval(NVNwindow* window, int presentInterval) {
    std::cout << "nvnWindowSetPresentInterval(presentInterval=" << presentInterval << ") called!" << std::endl;
}

void nvnWindowSetCrop(NVNwindow* window, int x, int y, int w, int h) {
    std::cout << "nvnWindowSetCrop(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
}

void nvnWindowGetCrop(const NVNwindow* window, NVNrectangle* rectangle) {
    std::cout << "nvnWindowGetCrop called!" << std::endl;
}

int nvnWindowGetNumTextures(NVNwindow* builder) {
    std::cout << "nvnWindowGetNumTextures() called!" << std::endl;
    return 0;
}

int nvnWindowGetNumActiveTextures(NVNwindow* builder) {
    std::cout << "nvnWindowGetNumActiveTextures() called!" << std::endl;
    return 0;
}

void nvnWindowSetNumActiveTextures(NVNwindow* builder, int numActiveTextures) {
    std::cout << "nvnWindowSetNumActiveTextures(numTextures=" << numActiveTextures << ") called!" << std::endl;
}
