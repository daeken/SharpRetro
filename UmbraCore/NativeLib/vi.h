#ifndef NATIVELIB_VI_H
#define NATIVELIB_VI_H
#include <cstdint>

namespace nn::vi {
    class Display {
    public:
    };

    class Layer {
    public:
    };

    enum ScalingMode {
        // Welp.
    };

    void Initialize();
    int64_t OpenDisplay(Display**, const char*);
    int64_t OpenDefaultDisplay(Display**);
    uint64_t CreateLayer(Layer**, Display*);
    uint64_t SetLayerScalingMode(Layer*, ScalingMode);
}

#endif //NATIVELIB_VI_H