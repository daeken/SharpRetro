#ifndef NATIVELIB_LIBRARY_H
#define NATIVELIB_LIBRARY_H

typedef void (*hookRegister_t)(const char*, void*);
extern "C" {
    void setupHooks(hookRegister_t regFunc);
}

#endif // NATIVELIB_LIBRARY_H