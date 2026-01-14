#include "glslc.h"

#include <iostream>

bool glslcCompilePreSpecialized(GLSLCcompileObject*) {
    std::cout << "glslcCompilePreSpecialized() called!" << std::endl;
    __builtin_trap();
    return false;
}

void** glslcCompileSpecialized(GLSLCcompileObject*, GLSLCspecializationBatch*) {
    std::cout << "glslcCompileSpecialized() called!" << std::endl;
    __builtin_trap();
    return nullptr;
}

uint8_t glslcInitialize(GLSLCcompileObject*) {
    std::cout << "glslcInitialize() called!" << std::endl;
    __builtin_trap();
    return 0;
}

void glslcFinalize(GLSLCcompileObject*) {
    std::cout << "glslcFinalize() called!" << std::endl;
    __builtin_trap();
}

GLSLCversion glslcGetVersion() {
    std::cout << "glslcGetVersion() called!" << std::endl;
    return {
        0x35,
        0xd,
        1,
        0xFF, // Fake minor version
    };
}
void glslcSetAllocator(GLSLCallocateFunction allocator, GLSLCfreeFunction freer, GLSLCreallocateFunction reallocator, void*) {
    std::cout << "glslcSetAllocator() called!" << std::endl;
}

GLSLCoptions glslcGetDefaultOptions() {
    std::cout << "glslcGetDefaultOptions() called!" << std::endl;
    __builtin_trap();
    return {};
}
