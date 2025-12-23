#include <iostream>
#include "nv.h"

NVNboolean nvnProgramInitialize(NVNprogram* program, NVNdevice* device) {
    std::cout << "nvnProgramInitialize called!" << std::endl;
    return 1;
}

void nvnProgramFinalize(NVNprogram* program) {
    std::cout << "nvnProgramFinalize called!" << std::endl;
}

void nvnProgramSetDebugLabel(NVNprogram* program, const char* label) {
    std::cout << "nvnProgramSetDebugLabel called!" << std::endl;
}

NVNboolean nvnProgramSetShaders(NVNprogram* program, int count, const NVNshaderData* stageData) {
    std::cout << "nvnProgramSetShaders called!" << std::endl;
    return 1;
}
