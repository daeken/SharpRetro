#include <iostream>
#include "nv.h"

void nvnWindowBuilderSetDevice(NVNwindowBuilder* _builder, NVNdevice* _device) {
    std::cout << "nvnWindowBuilderSetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto device = UNWRAP(_device);
}

void nvnWindowBuilderSetDefaults(NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnWindowBuilderSetNativeWindow(NVNwindowBuilder* _builder, NVNnativeWindow nativeWindow) {
    std::cout << "nvnWindowBuilderSetNativeWindow called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

int nvnWindowBuilderGetNumTextures(NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowBuilderGetNumTextures() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNtexture* nvnWindowBuilderGetTexture(NVNwindowBuilder* _builder, int texture) {
    std::cout << "nvnWindowBuilderGetTexture() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

void nvnWindowBuilderSetTextures(NVNwindowBuilder* _builder, int numTextures, NVNtexture* const* textures) {
    std::cout << "nvnWindowBuilderSetTextures(numTextures=" << numTextures << ", textures=" << std::hex << reinterpret_cast<uint64_t>(textures) << std::dec << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

int nvnWindowBuilderGetNumActiveTextures(NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowBuilderGetNumActiveTextures() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnWindowBuilderSetNumActiveTextures(NVNwindowBuilder* _builder, int numActiveTextures) {
    std::cout << "nvnWindowBuilderSetNumActiveTextures(numTextures=" << numActiveTextures << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

void nvnWindowBuilderSetPresentInterval(NVNwindowBuilder* _builder, int presentInterval) {
    std::cout << "nvnWindowBuilderSetPresentInterval(presentInterval=" << presentInterval << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

NVNnativeWindow nvnWindowBuilderGetNativeWindow(NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowBuilderGetNativeWindow() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnWindowBuilderGetPresentInterval(NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowBuilderGetPresentInterval() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

NVNboolean nvnWindowInitialize(NVNwindow* _window, NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowInitialize called!" << std::endl;
    auto window = UNWRAP(_window);
    auto builder = UNWRAP(_builder);
    return 1;
}

void nvnWindowFinalize(NVNwindow* _window) {
    std::cout << "nvnWindowFinalize called!" << std::endl;
    auto window = UNWRAP(_window);
}

void nvnWindowSetDebugLabel(NVNwindow* _window, const char* label) {
    std::cout << "nvnWindowSetDebugLabel called!" << std::endl;
    auto window = UNWRAP(_window);
}

NVNwindowAcquireTextureResult nvnWindowAcquireTexture(NVNwindow* _window, NVNsync* _textureAvailableSync, int* textureIndex) {
    std::cout << "nvnWindowAcquireTexture(textureAvailableSync=" << std::hex << reinterpret_cast<uint64_t>(_textureAvailableSync) << std::dec << ") called!" << std::endl;
    auto window = UNWRAP(_window);
    auto textureAvailableSync = UNWRAP(_textureAvailableSync);
    if (textureIndex) *textureIndex = 0;
    return 0;
}

NVNnativeWindow nvnWindowGetNativeWindow(NVNwindow* _window) {
    std::cout << "nvnWindowGetNativeWindow() called!" << std::endl;
    auto window = UNWRAP(_window);
    return 0;
}

int nvnWindowGetPresentInterval(NVNwindow* _window) {
    std::cout << "nvnWindowGetPresentInterval() called!" << std::endl;
    auto window = UNWRAP(_window);
    return 0;
}

void nvnWindowSetPresentInterval(NVNwindow* _window, int presentInterval) {
    std::cout << "nvnWindowSetPresentInterval(presentInterval=" << presentInterval << ") called!" << std::endl;
    auto window = UNWRAP(_window);
}

void nvnWindowSetCrop(NVNwindow* _window, int x, int y, int w, int h) {
    std::cout << "nvnWindowSetCrop(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
    auto window = UNWRAP(_window);
}

void nvnWindowGetCrop(NVNwindow* _window, NVNrectangle* _rectangle) {
    std::cout << "nvnWindowGetCrop called!" << std::endl;
    auto window = UNWRAP(_window);
    auto rectangle = UNWRAP(_rectangle);
}

int nvnWindowGetNumTextures(NVNwindow* _window) {
    std::cout << "nvnWindowGetNumTextures() called!" << std::endl;
    auto window = UNWRAP(_window);
    return 0;
}

int nvnWindowGetNumActiveTextures(NVNwindow* _window) {
    std::cout << "nvnWindowGetNumActiveTextures() called!" << std::endl;
    auto window = UNWRAP(_window);
    return 0;
}

void nvnWindowSetNumActiveTextures(NVNwindow* _window, int numActiveTextures) {
    std::cout << "nvnWindowSetNumActiveTextures(numTextures=" << numActiveTextures << ") called!" << std::endl;
    auto window = UNWRAP(_window);
}
