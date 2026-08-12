// 41_cmpxchg_fail_flags.c — `lock cmpxchg` FAILURE-PATH + SUCCESS-PATH flags,
// accumulator-load semantics, at 8/16/32/64, plus exact CAS-sum under
// failure-heavy contention.
//
// CMPXCHG r, m semantics the JIT must reproduce exactly:
//   temp := ACC - MEM   (sets OF/SF/ZF/CF/PF as a CMP would)
//   if ZF:  MEM := r     (success — store the new value)
//   else :  ACC := MEM   (failure — load OLD mem into the accumulator)
// The flags come from the SUBTRACT (ACC-MEM), NOT the store; and on failure
// the accumulator is CLOBBERED with the old memory value. Both are classic
// JIT-lowering traps (emulating cmpxchg with a plain compare+branch loses the
// ACC:=MEM write; computing flags from the wrong operand order flips CF/SF).
//
// packed flag byte = OF | SF<<1 | ZF<<2 | CF<<3 | PF<<4   (PF over low 8 bits)
//
// DETERMINISM: flag + accumulator capture is single-writer (tid0 only) against
// dedicated words → EXACT. Chosen operands (offline-computed):
//   SUCCESS: ACC=MEM=0x40, src=0x99 → temp=0, ZF=1, mem:=0x99, acc stays 0x40
//            flags: OF0 SF0 ZF1 CF0 PF1  => packed = 20
//   FAILURE: ACC=0, MEM=1, src=0x99   → temp=0-1=all-ones, ZF=0, acc:=1(old)
//            flags: OF0 SF1 ZF0 CF1 PF1  => packed = 26
//   (width-independent: temp's low byte is identical at every width here.)
//
// CONTENTION: all N threads run a CAS-increment loop on a shared u64 ITERS
// times. Under contention most CAS attempts FAIL (ZF=0) and retry — so the
// failure path is exercised millions of times — yet the retry loop makes the
// final value EXACT: ITERS*N. A per-thread "failed attempts" counter is stored
// for diagnostics but NOT checked (it is inherently schedule-dependent).
//
// objdump (clang-15, -O1) — encodings this file MUST emit:
//   f0 0f b0 ..          lock cmpxchg %r8b, (mem)     [W=8]
//   66 f0 0f b1 ..       lock cmpxchg %r16, (mem)     [W=16]
//   f0 0f b1 ..          lock cmpxchg %r32, (mem)     [W=32]
//   f0 48 0f b1 ..       lock cmpxchg %r64, (mem)     [W=64]
//   0f 94 (sete=setz) / 0f 92 (setb=setc) / 0f 98 (sets) / 0f 90 / 0f 9a
//
// shm layout (byte offsets; page zeroed on entry):
//   +0    cas_sum u64             contended (all threads)   -> ITERS*N
//   +256  fail_attempts[tid] via +256+tid*8  diagnostic, NOT checked
//         (placed past all checked words to avoid clobbering the battery)
//   tid0 flag battery:
//   success mem words (seed 0x40): +64 w8 +72 w16 +80 w32 +88 w64
//   failure mem words (seed 1)   : +96 w8 +104 w16 +112 w32 +120 w64
//   packed success flags: +128 w8 +136 w16 +144 w32 +152 w64  (all 20)
//   packed failure flags: +160 w8 +168 w16 +176 w32 +184 w64  (all 26)
//   acc-after-success   : +192 w8 +200 w16 +208 w32 +216 w64  (all 0x40)
//   acc-after-failure   : +224 w8 +232 w16 +240 w32 +248 w64  (all 1 = old mem)
//
// CHECK: 0 64 ITERS*N
// success flags (ZF=1,PF=1 => 20):
// CHECK: 128 8 20
// CHECK: 136 8 20
// CHECK: 144 8 20
// CHECK: 152 8 20
// failure flags (SF1 ZF0 CF1 PF1 => 26):
// CHECK: 160 8 26
// CHECK: 168 8 26
// CHECK: 176 8 26
// CHECK: 184 8 26
// accumulator unchanged on success (0x40 = 64):
// CHECK: 192 8 64
// CHECK: 200 16 64
// CHECK: 208 32 64
// CHECK: 216 64 64
// accumulator loaded with OLD mem on failure (=1):
// CHECK: 224 8 1
// CHECK: 232 16 1
// CHECK: 240 32 1
// CHECK: 248 64 1
// success stores new value into mem (0x99 = 153):
// CHECK: 64 8 153
// CHECK: 72 16 153
// CHECK: 80 32 153
// CHECK: 88 64 153
// failure leaves mem untouched (=1):
// CHECK: 96 8 1
// CHECK: 104 16 1
// CHECK: 112 32 1
// CHECK: 120 64 1
#define ITERS 100000
typedef unsigned long long u64; typedef unsigned u32; typedef unsigned short u16; typedef unsigned char u8;

#define PACK(of,sf,zf,cf,pf) ((u8)((of)|((sf)<<1)|((zf)<<2)|((cf)<<3)|((pf)<<4)))

// One cmpxchg with flag capture. acc is +a (IN: expected, OUT: old-or-same).
// operands: %0 mem(+m) %1 of %2 sf %3 zf %4 cf %5 pf %6 acc(+a) %7 src(r)
#define CX_CAP(insn, memptr, accval, srcval, packout, accout, T) do {    \
    T acc=(accval), src=(srcval); u8 of,sf,zf,cf,pf;                      \
    __asm__ volatile(insn " %7,%0\n\t"                                   \
        "seto %1\n\tsets %2\n\tsetz %3\n\tsetc %4\n\tsetp %5"            \
        : "+m"(*(memptr)), "=m"(of), "=m"(sf), "=m"(zf), "=m"(cf),       \
          "=m"(pf), "+a"(acc) : "r"(src) : "cc","memory");               \
    *(packout) = PACK(of,sf,zf,cf,pf); *(accout) = acc;                  \
  } while(0)

void _start(u64* shm, u64 tid) {
    u8* base = (u8*)shm;

    if (tid == 0) {
        // seed success mem = 0x40, failure mem = 1
        *(u8*)(base+64)=0x40; *(u16*)(base+72)=0x40; *(u32*)(base+80)=0x40; *(u64*)(base+88)=0x40;
        *(u8*)(base+96)=1;    *(u16*)(base+104)=1;   *(u32*)(base+112)=1;   *(u64*)(base+120)=1;

        u8 pk; u8 a8; u16 a16; u32 a32; u64 a64;
        // ---- SUCCESS path (acc=0x40==mem, src=0x99) ----
        CX_CAP("lock cmpxchgb", (u8*)(base+64),  (u8)0x40,  (u8)0x99,  (u8*)(base+128), &a8,  u8);  *(u8*)(base+192)=a8;
        CX_CAP("lock cmpxchgw", (u16*)(base+72), (u16)0x40, (u16)0x99, (u8*)(base+136), &a16, u16); *(u16*)(base+200)=a16;
        CX_CAP("lock cmpxchgl", (u32*)(base+80), (u32)0x40, (u32)0x99, (u8*)(base+144), &a32, u32); *(u32*)(base+208)=a32;
        CX_CAP("lock cmpxchgq", (u64*)(base+88), (u64)0x40, (u64)0x99, (u8*)(base+152), &a64, u64); *(u64*)(base+216)=a64;
        // ---- FAILURE path (acc=0 != mem=1, src=0x99) ----
        CX_CAP("lock cmpxchgb", (u8*)(base+96),   (u8)0,  (u8)0x99,  (u8*)(base+160), &a8,  u8);  *(u8*)(base+224)=a8;
        CX_CAP("lock cmpxchgw", (u16*)(base+104), (u16)0, (u16)0x99, (u8*)(base+168), &a16, u16); *(u16*)(base+232)=a16;
        CX_CAP("lock cmpxchgl", (u32*)(base+112), (u32)0, (u32)0x99, (u8*)(base+176), &a32, u32); *(u32*)(base+240)=a32;
        CX_CAP("lock cmpxchgq", (u64*)(base+120), (u64)0, (u64)0x99, (u8*)(base+184), &a64, u64); *(u64*)(base+248)=a64;
        (void)pk;
    }

    // ---- failure-heavy contended CAS-increment: final = ITERS*N ----
    volatile u64* sum = (volatile u64*)(base + 0);
    volatile u64* fa  = (volatile u64*)(base + 256 + tid*8);  // diagnostic
    u64 fails = 0;
    for (u64 i = 0; i < ITERS; i++) {
        u64 old, nw, seen;
        do {
            old = *sum; nw = old + 1;
            u64 acc = old;
            __asm__ volatile("lock cmpxchgq %2,%1"
                : "+a"(acc), "+m"(*sum) : "r"(nw) : "cc","memory");
            seen = acc;               // acc==old on success; ==current on failure
            if (seen != old) fails++;
        } while (seen != old);
    }
    *fa = fails;

    __asm__ volatile(".byte 0xCC");
}
