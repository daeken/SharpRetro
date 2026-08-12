// 12 — lock or/and/xor bit-stamp torture at 8/16/32-bit widths SPECIFICALLY
// (the 64-bit version lives in torture2.c). Stresses:
//   (a) narrow-width W-form LSE paths — the ARM lowering of a sub-word
//       lock-RMW (ldsetb/ldclrb/ldeorb + h/w variants, or the cmpxchg-loop
//       fallback the narrow x86 __atomic ops emit).
//   (b) bit-stamp correctness: contended, N<=8 threads, bit `tid` only — a
//       lost update names its thread. or/xor final == mask ; and final == ~mask.
//   (c) ZERO-EXTENSION of the fetched OLD value (offset 24): a hand-rolled
//       byte-cmpxchg loop keeps the OLD in a FULL 64-bit GPR (movzbl load,
//       cmpxchgb, store rax), then OR-accumulates (old >> 8). A correct
//       narrow atomic leaves bits>=8 zero → zx_garbage stays EXACTLY 0.
//       A JIT that forgets to zero-extend AL→RAX leaks garbage into >=8. ‡
//
// HARNESS DEPENDENCY (‡): runner must pre-set the AND targets to all-ones,
//   mirroring torture2.rs's `*(shm+24)=u64::MAX` for and_bits:
//     *(u32*)(shm+8)=0xFFFFFFFF; *(u16*)(shm+12)=0xFFFF; *(u8*)(shm+14)=0xFF;
//   (page is otherwise zeroed.)
//
// CHECK: 0  32 ((1<<N)-1)
// CHECK: 4  16 ((1<<N)-1)
// CHECK: 6  8  ((1<<N)-1)
// CHECK: 7  8  ((1<<N)-1)
// PRESET: 8  32 0xFFFFFFFF
// PRESET: 12 16 0xFFFF
// PRESET: 14 8  0xFF
// CHECK: 8  32 ((~((1<<N)-1)) & 0xFFFFFFFF)
// CHECK: 12 16 ((~((1<<N)-1)) & 0xFFFF)
// CHECK: 14 8  ((~((1<<N)-1)) & 0xFF)
// CHECK: 16 32 ((1<<N)-1)
// CHECK: 20 16 ((1<<N)-1)
// CHECK: 24 64 0
//
// objdump (VERIFIED, clang-15 -O1): return-value-unused fetch_{or,and,xor}
// lower to SINGLE narrow lock-RMW insns (the W-form LSE path directly) —
//   f0 44 09 1f           lock or  DWORD PTR [rdi],r11d
//   f0 66 44 09 5f 04     lock or  WORD PTR [rdi+0x4],r11w
//   f0 44 08 5f 06        lock or  BYTE PTR [rdi+0x6],r11b
//   f0 44 21 47 08        lock and DWORD PTR [rdi+0x8],r8d      (+ w/b variants)
//   f0 44 31 5f 10        lock xor DWORD PTR [rdi+0x10],r11d    (+ w/b variants)
// the +24 ZERO-EXTENSION arm is the explicit CAS loop keeping OLD in a GPR:
//   0f b6 01              movzx  eax,BYTE PTR [r9]         ; ZX AL->EAX(->RAX)
//   .. or dl,.. ; f0 0f b0 11   lock cmpxchg BYTE PTR [r9],dl ; jne 1b
//   -> zxg (old>>8) MUST stay 0 iff the JIT zero-extends the sub-word old value.
#include <stdatomic.h>
typedef unsigned long long u64;
typedef unsigned int u32;
typedef unsigned short u16;
typedef unsigned char u8;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    unsigned char* base = (unsigned char*)shm;
    _Atomic u32* or32  = (_Atomic u32*)(base + 0);
    _Atomic u16* or16  = (_Atomic u16*)(base + 4);
    _Atomic u8*  or8   = (_Atomic u8*) (base + 6);
    _Atomic u8*  xor8  = (_Atomic u8*) (base + 7);
    _Atomic u32* and32 = (_Atomic u32*)(base + 8);
    _Atomic u16* and16 = (_Atomic u16*)(base + 12);
    _Atomic u8*  and8  = (_Atomic u8*) (base + 14);
    _Atomic u32* xor32 = (_Atomic u32*)(base + 16);
    _Atomic u16* xor16 = (_Atomic u16*)(base + 20);
    _Atomic u64* zxg   = (_Atomic u64*)(base + 24);

    u32 bit  = 1u << tid;
    u8  cbit = (u8)(1u << tid);   // bit `tid` as a byte (N<=8)

    for (int i = 0; i < ITERS; i++) {
        atomic_fetch_or_explicit (or32,  bit,  memory_order_seq_cst);
        atomic_fetch_or_explicit (or16,  (u16)bit, memory_order_seq_cst);
        atomic_fetch_or_explicit (or8,   cbit, memory_order_seq_cst);
        atomic_fetch_and_explicit(and32, ~bit, memory_order_seq_cst);
        atomic_fetch_and_explicit(and16, (u16)~bit, memory_order_seq_cst);
        atomic_fetch_and_explicit(and8,  (u8)~cbit, memory_order_seq_cst);
        atomic_fetch_xor_explicit(xor32, bit,  memory_order_seq_cst);
        atomic_fetch_xor_explicit(xor16, (u16)bit, memory_order_seq_cst);
        atomic_fetch_xor_explicit(xor8,  cbit, memory_order_seq_cst);

        // (c) byte fetch-or keeping OLD in a full 64-bit register.
        u64 old64;
        __asm__ volatile(
            "1:\n\t"
            "movzbl (%1), %%eax\n\t"       // rax = zx(current byte)
            "movl %%eax, %%edx\n\t"
            "orb %2, %%dl\n\t"             // dl = al | bit
            "lock cmpxchgb %%dl, (%1)\n\t" // [p]==al ? [p]=dl : al=[p]
            "jne 1b\n\t"
            "movq %%rax, %0"               // store full rax (old byte, zx)
            : "=r"(old64) : "r"(or8), "r"(cbit)
            : "rax","rdx","cc","memory");
        if (old64 >> 8)
            atomic_fetch_or_explicit(zxg, old64 >> 8, memory_order_seq_cst);
    }
    // one extra xor flip so total flips = ITERS+1 (odd) → bit ends SET.
    atomic_fetch_xor_explicit(xor32, bit,  memory_order_seq_cst);
    atomic_fetch_xor_explicit(xor16, (u16)bit, memory_order_seq_cst);
    atomic_fetch_xor_explicit(xor8,  cbit, memory_order_seq_cst);
    __asm__ volatile(".byte 0xCC");
}
