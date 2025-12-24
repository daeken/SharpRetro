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

    class Window {
    public:
    };

    enum ScalingMode {
        // Welp.
    };

    void Initialize();
    int64_t OpenDisplay(Display**, const char*);
    int64_t OpenDefaultDisplay(Display**);
    int64_t GetDisplayVsyncEvent(uint32_t*, Display*);
    uint64_t CreateLayer(Layer**, Display*);
    uint64_t SetLayerScalingMode(Layer*, ScalingMode);
    uint64_t DestroyLayer(Layer*);
    uint64_t GetNativeWindow(Window**, Layer*);
}

#endif //NATIVELIB_VI_H