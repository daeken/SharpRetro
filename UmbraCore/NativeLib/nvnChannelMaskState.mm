#include <iostream>
#include "nv.h"

void nvnChannelMaskStateSetDefaults(NVNchannelMaskState* _channelMask) {
    std::cout << "nvnChannelMaskStateSetDefaults called!" << std::endl;
    auto channelMask = UNWRAP(_channelMask);
}

void nvnChannelMaskStateSetChannelMask(NVNchannelMaskState* _channelMask, int index, NVNboolean r, NVNboolean g, NVNboolean b, NVNboolean a) {
    std::cout << "nvnChannelMaskStateSetChannelMask called!" << std::endl;
    auto channelMask = UNWRAP(_channelMask);
}

void nvnChannelMaskStateGetChannelMask(NVNchannelMaskState* _channelMask, int index, NVNboolean* r, NVNboolean* g, NVNboolean* b, NVNboolean* a) {
    std::cout << "nvnChannelMaskStateGetChannelMask called!" << std::endl;
    auto channelMask = UNWRAP(_channelMask);
    if (r) *r = 0;
    if (g) *g = 0;
    if (b) *b = 0;
    if (a) *a = 0;
}
