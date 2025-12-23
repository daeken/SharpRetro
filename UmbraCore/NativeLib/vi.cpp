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
}

uint64_t nn::vi::CreateLayer(Layer** layer, Display* display) {
    std::cout << "nn::vi::CreateLayer called!" << std::endl;
    return 0;
}

uint64_t nn::vi::SetLayerScalingMode(Layer* layer, ScalingMode mode) {
    std::cout << "nn::vi::SetLayerScalingMode called! " << mode << std::endl;
    return 0;
}
