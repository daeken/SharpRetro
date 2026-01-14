#include "vi.h"

#include <iostream>
#include <ostream>

void nn::vi::Initialize() {
    std::cout << "nn::vi::Initialize called!" << std::endl;
}

int64_t nn::vi::OpenDisplay(Display** display, const char* name) {
    std::cout << "nn::vi::OpenDisplay called! '" << name << "'" << std::endl;
    return 0;
}

int64_t nn::vi::OpenDefaultDisplay(Display** display) {
    return OpenDisplay(display, "Default");
}

int64_t nn::vi::GetDisplayVsyncEvent(uint32_t* eventHandle, Display* display) {
    std::cout << "nn::GetDisplayVsyncEvent called! " << std::endl;
    return 0;
}

uint64_t nn::vi::CreateLayer(Layer** layer, Display* display) {
    std::cout << "nn::vi::CreateLayer called!" << std::endl;
    return 0;
}

uint64_t nn::vi::CreateLayerExtra(Layer**, Display*, int, int) {
    std::cout << "nn::vi::CreateLayer[Extra] called!" << std::endl;
    return 0;
}

uint64_t nn::vi::SetLayerCrop(Layer*, int, int, int, int) {
    std::cout << "nn::vi::SetLayerCrop called! " << std::endl;
    return 0;
}

uint64_t nn::vi::SetLayerScalingMode(Layer* layer, ScalingMode mode) {
    std::cout << "nn::vi::SetLayerScalingMode called! " << mode << std::endl;
    return 0;
}

uint64_t nn::vi::DestroyLayer(Layer* layer) {
    std::cout << "nn::vi::DestroyLayer called! " << std::endl;
    return 0;
}

uint64_t nn::vi::GetNativeWindow(Window** window, Layer* layer) {
    std::cout << "nn::vi::GetNativeWindow called! " << std::endl;
    return 0;
}

uint64_t nn::vi::native::NativeWindowHolder::GetNativeWindow(void*) {
    std::cout << "nn::vi::native::GetNativeWindow called!" << std::endl;
    return 0;
}
