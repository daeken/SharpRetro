#pragma once

#include <stdint.h>
#include <math.h>
#include "cpuState.h"
#include "svcTable.h"

typedef uint64_t bool; // OH GOD
typedef __uint128_t uint128_t;
typedef __int128_t int128_t;

#ifdef BUILD_LIB
CpuState_t *State;
SvcTable_t *SvcTable;

static void setup(CpuState_t *state, SvcTable_t *svcTable) {
	State = state;
	SvcTable = svcTable;
}

static void CALL(uint64_t addr) {
}

static inline int32_t signext_uint8_t_int32_t(uint8_t v, int width) {
	return 0;
}
static inline int64_t signext_uint8_t_int64_t(uint8_t v, int width) {
	return 0;
}
static inline int64_t signext_uint16_t_int32_t(uint16_t v, int width) {
	return 0;
}
static inline int64_t signext_uint16_t_int64_t(uint16_t v, int width) {
	return 0;
}
static inline int64_t signext_uint32_t_int64_t(uint32_t v, int width) {
	return 0;
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
