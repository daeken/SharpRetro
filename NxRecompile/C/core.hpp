#pragma once

#include <bit>
#include <cstdint>
#include <cmath>
#include "cpuState.hpp"
#include "callbackTable.hpp"

typedef uint64_t bool_t; // OH GOD
#define TRUE ((bool_t) 1ULL)
#define FALSE ((bool_t) 0ULL)
typedef __uint128_t uint128_t;
typedef __int128_t int128_t;

#ifdef BUILD_LIB
CallbackTable_t *Callbacks;

typedef uint64_t (*blockFunc)(CpuState_t *);
extern void ***jumpTable;
extern int moduleCount;
void loadModules();

extern "C" {
	void setup(CallbackTable_t *callbacks);
	void runFrom(CpuState_t *state, uint64_t addr, uint64_t until);
}

void setup(CallbackTable_t *callbacks) {
	Callbacks = callbacks;
	loadModules();
}

void runFrom(CpuState_t *state, uint64_t addr, uint64_t until) {
	//Callbacks->debug(addr, "Attempting run from here");
	while(addr != until) {
		if(addr < 0x7100000000)
			break;
		uint64_t modIndex = (addr - 0x7100000000) >> 32;
		if(modIndex >= moduleCount)
			break;
		addr = ((blockFunc) jumpTable[modIndex][(addr & 0xFFFFFFFF) >> 2])(state);
	}
	//Callbacks->debug(addr, "Finished running");
}

static inline int32_t signext_int32_t(uint8_t v, int32_t width) {
	return (int32_t) ((int32_t) (((int32_t) v) << (32 - width)) >> (32 - width));
}
static inline int32_t signext_int32_t(uint16_t v, int32_t width) {
	return (int32_t) ((int32_t) (((int32_t) v) << (32 - width)) >> (32 - width));
}
static inline int32_t signext_int32_t(uint32_t v, int32_t width) {
	return (int32_t) ((int32_t) (((int32_t) v) << (32 - width)) >> (32 - width));
}
static inline int64_t signext_int64_t(uint8_t v, int32_t width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}
static inline int64_t signext_int64_t(uint16_t v, int32_t width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}
static inline int64_t signext_int64_t(uint32_t v, int32_t width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}
static inline int64_t signext_int64_t(uint64_t v, int32_t width) {
	return (int64_t) ((int64_t) (((int64_t) v) << (64 - width)) >> (64 - width));
}

static inline v4f setElement(v4f vector, int32_t index, uint8_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, uint16_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, uint32_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, uint64_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, int8_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, int16_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, int32_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, int64_t value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, float value) {
	return vector;
}
static inline v4f setElement(v4f vector, int32_t index, double value) {
	return vector;
}

static inline v4f zerotop(v4f vector) {
	return vector;
}

static inline uint32_t floatToFixed_uint32_t(float value, int32_t fbits) {
	return 0;
}
static inline uint64_t floatToFixed_uint64_t(float value, int32_t fbits) {
	return 0;
}
static inline uint32_t floatToFixed_uint32_t(double value, int32_t fbits) {
	return 0;
}
static inline uint64_t floatToFixed_uint64_t(double value, int32_t fbits) {
	return 0;
}

static inline v4f vectorExtract(v4f a, v4f b, uint32_t q, uint32_t index) {
	return a;
}

static inline v4f vectorCountBits(v4f vec, int64_t elems) {
	return vec;
}

static inline uint64_t vectorSumUnsigned(v4f vec, int64_t esize, int64_t count) {
	return 0;
}

static inline v4f vectorFrsqrte(v4f vec, int32_t bits, int32_t elements) {
	return vec;
}

static inline uint8_t compareAndSwap(uint8_t *pointer, uint8_t value, uint8_t comparand) {
	return 0;
}
static inline uint8_t compareAndSwap(uint16_t *pointer, uint16_t value, uint16_t comparand) {
	return 0;
}
static inline uint8_t compareAndSwap(uint32_t *pointer, uint32_t value, uint32_t comparand) {
	return 0;
}
static inline uint8_t compareAndSwap(uint64_t *pointer, uint64_t value, uint64_t comparand) {
	return 0;
}

#endif
