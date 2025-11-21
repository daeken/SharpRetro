#pragma once

#include <stdint.h>
#include <math.h>
#include "cpuState.h"
#include "callbackTable.h"

typedef uint64_t bool; // OH GOD
typedef __uint128_t uint128_t;
typedef __int128_t int128_t;

#ifdef BUILD_LIB
CpuState_t *State;
CallbackTable_t *Callbacks;

typedef uint64_t (*blockFunc)();
void ***jumpTable;
int moduleCount;
void loadModules();

void setup(CpuState_t *state, CallbackTable_t *callbacks) {
	State = state;
	Callbacks = callbacks;
	loadModules();
}

void runFrom(uint64_t addr, uint64_t until) {
	//Callbacks->debug(addr, "Attempting run from here");
	while(addr != until) {
		if(addr < 0x7100000000)
			break;
		uint64_t modIndex = (addr - 0x7100000000) >> 32;
		if(modIndex >= moduleCount)
			break;
		addr = ((blockFunc) jumpTable[modIndex][(addr & 0xFFFFFFFF) >> 2])();
	}
	//Callbacks->debug(addr, "Finished running");
}

static inline int32_t signext_uint8_t_int32_t(uint8_t v, int width) {
	return (int32_t) ((int32_t) (((int32_t) v) << (32 - width)) >> (32 - width));
}
static inline int64_t signext_uint8_t_int64_t(uint8_t v, int width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}
static inline int32_t signext_uint16_t_int32_t(uint16_t v, int width) {
	return (int32_t) ((int32_t) (((int32_t) v) << (32 - width)) >> (32 - width));
}
static inline int64_t signext_uint16_t_int64_t(uint16_t v, int width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}
static inline int64_t signext_uint32_t_int64_t(uint32_t v, int width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}

static inline v4f setElement_v16u(v4f vector, int index, uint8_t value) {
	return vector;
}
static inline v4f setElement_v2u(v4f vector, int index, uint64_t value) {
	return vector;
}
static inline v4f setElement_v4f(v4f vector, int index, float value) {
	return vector;
}
static inline v4f setElement_v2d(v4f vector, int index, double value) {
	return vector;
}

static inline v4f zerotop_v4f(v4f vector) {
	return vector;
}

#endif
