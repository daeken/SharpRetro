#include <iostream>
#include "nv.h"

NVNqueueGetErrorResult nvnQueueGetError(NVNqueue* _queue, NVNqueueErrorInfo* _info) {
    std::cout << "nvnQueueGetError(info=" << std::hex << reinterpret_cast<uint64_t>(_info) << std::dec << ") called!" << std::endl;
    auto queue = UNWRAP(_queue);
    auto info = UNWRAP(_info);
    return 0;
}

size_t nvnQueueGetTotalCommandMemoryUsed(NVNqueue* _queue) {
    std::cout << "nvnQueueGetTotalCommandMemoryUsed() called!" << std::endl;
    auto queue = UNWRAP(_queue);
    return 0;
}

size_t nvnQueueGetTotalControlMemoryUsed(NVNqueue* _queue) {
    std::cout << "nvnQueueGetTotalControlMemoryUsed() called!" << std::endl;
    auto queue = UNWRAP(_queue);
    return 0;
}

size_t nvnQueueGetTotalComputeMemoryUsed(NVNqueue* _queue) {
    std::cout << "nvnQueueGetTotalComputeMemoryUsed() called!" << std::endl;
    auto queue = UNWRAP(_queue);
    return 0;
}

void nvnQueueResetMemoryUsageCounts(NVNqueue* _queue) {
    std::cout << "nvnQueueResetMemoryUsageCounts called!" << std::endl;
    auto queue = UNWRAP(_queue);
}

void* nvnQueueBuilderGetMemory(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetMemory() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

NVNdevice* nvnQueueBuilderGetDevice(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetDevice() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

void nvnQueueBuilderSetDevice(NVNqueueBuilder* _builder, NVNdevice* _device) {
    std::cout << "nvnQueueBuilderSetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    auto device = UNWRAP(_device);
}

void nvnQueueBuilderSetDefaults(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderSetDefaults called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

int nvnQueueBuilderGetFlags(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetFlags called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetFlags(NVNqueueBuilder* _builder, int flags) {
    std::cout << "nvnQueueBuilderSetFlags(flags=" << flags << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnQueueBuilderGetCommandMemorySize(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetCommandMemorySize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetCommandMemorySize(NVNqueueBuilder* _builder, size_t size) {
    std::cout << "nvnQueueBuilderSetCommandMemorySize(size=" << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnQueueBuilderGetComputeMemorySize(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetComputeMemorySize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetComputeMemorySize(NVNqueueBuilder* _builder, size_t size) {
    std::cout << "nvnQueueBuilderSetComputeMemorySize(size=" << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnQueueBuilderGetControlMemorySize(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetControlMemorySize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetControlMemorySize(NVNqueueBuilder* _builder, size_t size) {
    std::cout << "nvnQueueBuilderSetControlMemorySize(size=" << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnQueueBuilderGetQueueMemorySize(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetQueueMemorySize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetQueueMemory(NVNqueueBuilder* _builder, void* memory, size_t size) {
    std::cout << "nvnQueueBuilderSetQueueMemory(memory=" << std::hex << reinterpret_cast<uint64_t>(memory) << ", size=" << std::dec << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnQueueBuilderGetMemorySize(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetMemorySize() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetQueuePriority(NVNqueueBuilder* _builder, int priority) {
    std::cout << "nvnQueueBuilderSetQueuePriority(priority=" << priority << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

size_t nvnQueueBuilderGetCommandFlushThreshold(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetCommandFlushThreshold() called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

int nvnQueueBuilderGetQueuePriority(NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueBuilderGetQueuePriority called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return 0;
}

void nvnQueueBuilderSetCommandFlushThreshold(NVNqueueBuilder* _builder, size_t size) {
    std::cout << "nvnQueueBuilderSetCommandFlushThreshold(size=" << size << ") called!" << std::endl;
    auto builder = UNWRAP(_builder);
}

NVNboolean nvnQueueInitialize(NVNqueue* _queue, NVNqueueBuilder* _builder) {
    std::cout << "nvnQueueInitialize called!" << std::endl;
    auto queue = UNWRAP(_queue);
    auto builder = UNWRAP(_builder);
    return 1;
}

void nvnQueueFinalize(NVNqueue* _queue) {
    std::cout << "nvnQueueFinalize called!" << std::endl;
    auto queue = UNWRAP(_queue);
}

void nvnQueueSetDebugLabel(NVNqueue* _queue, const char* label) {
    std::cout << "nvnQueueSetDebugLabel called!" << std::endl;
    auto queue = UNWRAP(_queue);
}

void nvnQueueSubmitCommands(NVNqueue* _queue, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnQueueSubmitCommands(numCommands=" << numCommands << ", handles=" << std::hex << reinterpret_cast<uint64_t>(handles) << std::dec << ") called!" << std::endl;
    auto queue = UNWRAP(_queue);
}

void nvnQueueFlush(NVNqueue* _queue) {
    std::cout << "nvnQueueFlush called!" << std::endl;
    auto queue = UNWRAP(_queue);
}

void nvnQueueFinish(NVNqueue* _queue) {
    std::cout << "nvnQueueFinish called!" << std::endl;
    auto queue = UNWRAP(_queue);
}

void nvnQueuePresentTexture(NVNqueue* _queue, NVNwindow* _window, int textureIndex) {
    std::cout << "nvnQueuePresentTexture(window=" << std::hex << reinterpret_cast<uint64_t>(_window) << ", textureIndex=" << std::dec << textureIndex << ") called!" << std::endl;
    auto queue = UNWRAP(_queue);
    auto window = UNWRAP(_window);
}

NVNqueueAcquireTextureResult nvnQueueAcquireTexture(NVNqueue* _queue, NVNwindow* _window, int* textureIndex) {
    std::cout << "nvnQueueAcquireTexture(window=" << std::hex << reinterpret_cast<uint64_t>(_window) << std::dec << ") called!" << std::endl;
    auto queue = UNWRAP(_queue);
    auto window = UNWRAP(_window);
    if (textureIndex) *textureIndex = 0;
    return 0;
}

NVNdevice* nvnWindowBuilderGetDevice(NVNwindowBuilder* _builder) {
    std::cout << "nvnWindowBuilderGetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}

void nvnQueueFenceSync(NVNqueue* _queue, NVNsync* _sync, NVNsyncCondition condition, int flags) {
    std::cout << "nvnQueueFenceSync called!" << std::endl;
    auto queue = UNWRAP(_queue);
    auto sync = UNWRAP(_sync);
}

NVNboolean nvnQueueWaitSync(NVNqueue* _queue, NVNsync* _sync) {
    std::cout << "nvnQueueWaitSync called!" << std::endl;
    auto queue = UNWRAP(_queue);
    auto sync = UNWRAP(_sync);
    return 1;
}
