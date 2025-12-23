#include <iostream>
#include "nv.h"

void nvnChannelMaskStateSetDefaults(NVNchannelMaskState* channelMask) {
    std::cout << "nvnChannelMaskStateSetDefaults called!" << std::endl;
}

void nvnChannelMaskStateSetChannelMask(NVNchannelMaskState* channelMask, int index, NVNboolean r, NVNboolean g, NVNboolean b, NVNboolean a) {
    std::cout << "nvnChannelMaskStateSetChannelMask called!" << std::endl;
}

void nvnChannelMaskStateGetChannelMask(const NVNchannelMaskState* channelMask, int index, NVNboolean* r, NVNboolean* g, NVNboolean* b, NVNboolean* a) {
    std::cout << "nvnChannelMaskStateGetChannelMask called!" << std::endl;
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}