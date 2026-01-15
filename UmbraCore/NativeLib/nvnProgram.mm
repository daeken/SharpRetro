#include <iostream>
#include "nv.h"

NVNboolean nvnProgramInitialize(NVNprogram* _program, NVNdevice* _device) {
    std::cout << "nvnProgramInitialize called!" << std::endl;
    auto program = UNWRAP(_program);
    auto device = UNWRAP(_device);
    return 1;
}

void nvnProgramFinalize(NVNprogram* _program) {
    std::cout << "nvnProgramFinalize called!" << std::endl;
    auto program = UNWRAP(_program);
}

void nvnProgramSetDebugLabel(NVNprogram* _program, const char* label) {
    std::cout << "nvnProgramSetDebugLabel called!" << std::endl;
    auto program = UNWRAP(_program);
}

void nvnProgramSetSampleShading(NVNprogram* _program) {
    std::cout << "nvnProgramSetSampleShading called!" << std::endl;
    auto program = UNWRAP(_program);
}

NVNboolean nvnProgramSetShaders(NVNprogram* _program, int count, NVNshaderData* _stageData) {
    std::cout << "nvnProgramSetShaders called!" << std::endl;
    auto program = UNWRAP(_program);
    auto stageData = UNWRAP(_stageData);
    return 1;
}

NVNboolean nvnProgramSetShadersExt(NVNprogram* _program, int count, NVNshaderData* _stageData) {
    std::cout << "nvnProgramSetShadersExt called!" << std::endl;
    auto program = UNWRAP(_program);
    auto stageData = UNWRAP(_stageData);
    return 1;
}

NVNdevice* nvnMemoryPoolBuilderGetDevice(NVNmemoryPoolBuilder* _builder) {
    std::cout << "nvnMemoryPoolBuilderGetDevice called!" << std::endl;
    auto builder = UNWRAP(_builder);
    return nullptr;
}
