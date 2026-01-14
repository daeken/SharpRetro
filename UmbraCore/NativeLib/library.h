#ifndef NATIVELIB_LIBRARY_H
#define NATIVELIB_LIBRARY_H

#include <SDL2/SDL.h>

struct ManagedCallbacks {
    void (*registerHook)(const char*, void*);
    SDL_Window* (*getSdlWindow)();
    SDL_Renderer* (*getSdlRenderer)();
    const char* (*recompileShader)(void*, uint64_t);
    void (*freeShader)(const char*);
};

extern ManagedCallbacks* Callbacks;

extern "C" {
    void setup(ManagedCallbacks* callbacks);
}

#endif // NATIVELIB_LIBRARY_H