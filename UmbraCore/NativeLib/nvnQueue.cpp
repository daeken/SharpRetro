#include <iostream>
#include "nv.h"

NVNqueueGetErrorResult nvnQueueGetError(NVNqueue* queue, NVNqueueErrorInfo* info) {
    std::cout << "nvnQueueGetError(info=" << std::hex << reinterpret_cast<uint64_t>(info) << std::dec << ") called!" << std::endl;
    return 0;
}

size_t nvnQueueGetTotalCommandMemoryUsed(NVNqueue* queue) {
    std::cout << "nvnQueueGetTotalCommandMemoryUsed() called!" << std::endl;
    return 0;
}

size_t nvnQueueGetTotalControlMemoryUsed(NVNqueue* queue) {
    std::cout << "nvnQueueGetTotalControlMemoryUsed() called!" << std::endl;
    return 0;
}

size_t nvnQueueGetTotalComputeMemoryUsed(NVNqueue* queue) {
    std::cout << "nvnQueueGetTotalComputeMemoryUsed() called!" << std::endl;
    return 0;
}

void nvnQueueResetMemoryUsageCounts(NVNqueue* queue) {
    std::cout << "nvnQueueResetMemoryUsageCounts called!" << std::endl;
}

void nvnQueueBuilderSetDevice(NVNqueueBuilder* builder, NVNdevice* device) {
    std::cout << "nvnQueueBuilderSetDevice called!" << std::endl;
}

void nvnQueueBuilderSetDefaults(NVNqueueBuilder* builder) {
    std::cout << "nvnQueueBuilderSetDefaults called!" << std::endl;
}

void nvnQueueBuilderSetFlags(NVNqueueBuilder* builder, int flags) {
    std::cout << "nvnQueueBuilderSetFlags(flags=" << flags << ") called!" << std::endl;
}

void nvnQueueBuilderSetCommandMemorySize(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetCommandMemorySize(size=" << size << ") called!" << std::endl;
}

void nvnQueueBuilderSetComputeMemorySize(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetComputeMemorySize(size=" << size << ") called!" << std::endl;
}

void nvnQueueBuilderSetControlMemorySize(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetControlMemorySize(size=" << size << ") called!" << std::endl;
}

size_t nvnQueueBuilderGetQueueMemorySize(const NVNqueueBuilder* builder) {
    std::cout << "nvnQueueBuilderGetQueueMemorySize() called!" << std::endl;
    return 0;
}

void nvnQueueBuilderSetQueueMemory(NVNqueueBuilder* builder, void* memory, size_t size) {
    std::cout << "nvnQueueBuilderSetQueueMemory(memory=" << std::hex << reinterpret_cast<uint64_t>(memory) << ", size=" << std::dec << size << ") called!" << std::endl;
}

void nvnQueueBuilderSetCommandFlushThreshold(NVNqueueBuilder* builder, size_t size) {
    std::cout << "nvnQueueBuilderSetCommandFlushThreshold(size=" << size << ") called!" << std::endl;
}

NVNboolean nvnQueueInitialize(NVNqueue* queue, const NVNqueueBuilder* builder) {
    std::cout << "nvnQueueInitialize called!" << std::endl;
    return 1;
}

void nvnQueueFinalize(NVNqueue* queue) {
    std::cout << "nvnQueueFinalize called!" << std::endl;
}

void nvnQueueSetDebugLabel(NVNqueue* queue, const char* label) {
    std::cout << "nvnQueueSetDebugLabel called!" << std::endl;
}

void nvnQueueSubmitCommands(NVNqueue* queue, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnQueueSubmitCommands(numCommands=" << numCommands << ", handles=" << std::hex << reinterpret_cast<uint64_t>(handles) << std::dec << ") called!" << std::endl;
}

void nvnQueueFlush(NVNqueue* queue) {
    std::cout << "nvnQueueFlush called!" << std::endl;
}

void nvnQueueFinish(NVNqueue* queue) {
    std::cout << "nvnQueueFinish called!" << std::endl;
}

void nvnQueuePresentTexture(NVNqueue* queue, NVNwindow* window, int textureIndex) {
    std::cout << "nvnQueuePresentTexture(window=" << std::hex << reinterpret_cast<uint64_t>(window) << ", textureIndex=" << std::dec << textureIndex << ") called!" << std::endl;
}

NVNqueueAcquireTextureResult nvnQueueAcquireTexture(NVNqueue* queue, NVNwindow* window, int* textureIndex) {
    std::cout << "nvnQueueAcquireTexture(window=" << std::hex << reinterpret_cast<uint64_t>(window) << std::dec << ") called!" << std::endl;
    if (textureIndex) *textureIndex = 0;
    return 0;
}

void nvnQueueFenceSync(NVNqueue* queue, NVNsync* sync, NVNsyncCondition condition, int flags) {
    std::cout << "nvnQueueFenceSync called!" << std::endl;
}

NVNboolean nvnQueueWaitSync(NVNqueue* queue, const NVNsync* sync) {
    std::cout << "nvnQueueWaitSync called!" << std::endl;
    return 1;
}