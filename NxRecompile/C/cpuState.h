#pragma once

#include <stdint.h>

typedef uint8_t v16u __attribute__((ext_vector_type(16)));
typedef uint16_t v8u __attribute__((ext_vector_type(8)));
typedef uint32_t v4u __attribute__((ext_vector_type(4)));
typedef uint64_t v2u __attribute__((ext_vector_type(2)));
typedef double v2d __attribute__((ext_vector_type(2)));
typedef float v4f __attribute__((ext_vector_type(4)));

typedef struct CpuState {
	uint64_t PC, SP;

	uint64_t X[32];
	v4f V[32];

	uint64_t TlsBase, BranchTo;

	uint8_t Exclusive8;
	uint16_t Exclusive16;
	uint32_t Exclusive32;
	uint64_t Exclusive64;

	uint64_t NZCV_N, NZCV_Z, NZCV_C, NZCV_V;
} CpuState_t;
