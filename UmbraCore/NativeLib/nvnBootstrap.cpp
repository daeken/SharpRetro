#include <iostream>
#include "nv.h"

#define NVN_FUNC(fname) if(strcmp(name, #fname) == 0) return reinterpret_cast<void*>((void*)fname);

PFNNVNGENERICFUNCPTRPROC nvnDeviceGetProcAddress(const NVNdevice* device, const char* name) {
    std::cout << "nvnDeviceGetProcAddress (from device) '" << name << "'" << std::endl;
    NVN_ALL(NVN_FUNC);
    if(strcmp(name, "nvnDeviceGetProcAddress") == 0) return reinterpret_cast<PFNNVNGENERICFUNCPTRPROC>(nvnDeviceGetProcAddress);
    std::cout << "Could not find nvn function " << name << std::endl;
    __builtin_trap();
    return nullptr;
}

PFNNVNGENERICFUNCPTRPROC nvnBootstrapLoader(const char* name) {
    std::cout << "nvnBootstrapLoader called! '" << name << "'" << std::endl;
    if(strcmp(name, "nvnDeviceGetProcAddress") == 0)return reinterpret_cast<void*>(nvnDeviceGetProcAddress);
    NVN_ALL(NVN_FUNC);
    __builtin_trap();
    return nullptr;
}
