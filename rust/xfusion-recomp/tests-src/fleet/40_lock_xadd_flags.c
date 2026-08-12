// 40_lock_xadd_flags.c — `lock xadd` FULL FLAG CONSUMPTION at 8/16/32/64.
//
// After a lock-RMW the arch flags (OF/SF/ZF/CF/PF) reflect the ADD that xadd
// performs. The JIT must compute them from the pre-image value the CPU
// actually observed, not from a stale/plain read. We drive four sign-flip
// boundary cases per width and read the flags back via SETcc → shm bytes.
//
//   packed flag byte = OF | SF<<1 | ZF<<2 | CF<<3 | PF<<4   (PF over low 8 bits)
//
// FLAG DETERMINISM: flag capture is only well-defined when ONE thread performs
// the RMW on a given word (the pre-image is then known exactly). So the flag
// battery runs on tid==0 ONLY, against DEDICATED per-case words that no other
// thread touches. All other threads spin on a private scratch word so the run
// still exercises N-thread scheduling pressure around the flag thread, but the
// asserted words are single-writer → flag outcomes are EXACT, not racy.
//
// A SECOND section proves lock xadd's RETURN VALUE + accumulation is atomic
// under real contention: every thread does `lock xaddq (tid+1)` ITERS times
// into a shared sum. Final = ITERS * sum(1..N) = ITERS*N*(N+1)/2, order-free.
//
// Boundary cases (seed in memory, addend in reg), per width W:
//   A  SMAX + 1        -> signed overflow: OF=1 SF=1 ZF=0 CF=0
//   B  UMAX + 1        -> unsigned wrap  : OF=0 SF=0 ZF=1 CF=1 PF=1
//   C  SMIN + (-1)=UMAX-> OF=1 CF=1 (borrow-shape via wrap)
//   D  0 + 0           -> ZF=1 PF=1, all else 0
// (values computed offline; see CHECK exprs — width-independent for A packed
//  differs at W=8 because PF of 0x80 low-byte=0x80 has odd popcount.)
//
// objdump (clang-15, -O1) — encodings this file MUST emit:
//   f0 0f c0 ..            lock xadd %r8b, (mem)      [W=8]
//   66 f0 0f c1 ..         lock xadd %r16, (mem)      [W=16]
//   f0 0f c1 ..            lock xadd %r32, (mem)      [W=32]
//   f0 48 0f c1 ..         lock xadd %r64, (mem)      [W=64]
//   0f 90 / 0f 98 / 0f 94 / 0f 92 / 0f 9a   seto/sets/sete(=setz)/setb(=setc)/setp
//   (setz→sete 0f94 and setc→setb 0f92 are opcode aliases; clang prints the
//    canonical sete/setb — same bytes, capturing ZF and CF respectively.)
//
// shm layout (byte offsets; page is zeroed on entry, we pre-seed via tid0):
//   +0   xadd_sum        u64  contended stamped sum   (all threads)
//   +64  scratch         u64  non-tid0 spin target    (keeps others busy)
//   flag battery words (tid0 only), one per (width,case):
//   +128 w8_A  +136 w8_B  +144 w8_C  +152 w8_D
//   +160 w16_A +168 w16_B +176 w16_C +184 w16_D
//   +192 w32_A +200 w32_B +208 w32_C +216 w32_D
//   +224 w64_A +232 w64_B +240 w64_C +248 w64_D
//   flag result bytes (tid0 only): base +256, 5 bytes per case, 16 cases:
//     case k → +256 + k*8  (packed byte at [0]); k = width_idx*4 + case_idx
//     width_idx: 8->0,16->1,32->2,64->3 ; case_idx A->0 B->1 C->2 D->3
//
// CHECK: 0 64 (ITERS*N*(N+1))//2
// packed flag bytes — width 8:  A=3   B=28  C=9   D=20
// CHECK: 256 8 3
// CHECK: 264 8 28
// CHECK: 272 8 9
// CHECK: 280 8 20
// width 16: A=19 B=28 C=25 D=20
// CHECK: 288 8 19
// CHECK: 296 8 28
// CHECK: 304 8 25
// CHECK: 312 8 20
// width 32: A=19 B=28 C=25 D=20
// CHECK: 320 8 19
// CHECK: 328 8 28
// CHECK: 336 8 25
// CHECK: 344 8 20
// width 64: A=19 B=28 C=25 D=20
// CHECK: 352 8 19
// CHECK: 360 8 28
// CHECK: 368 8 25
// CHECK: 376 8 20
#define ITERS 100000
typedef unsigned long long u64; typedef unsigned u32; typedef unsigned short u16; typedef unsigned char u8;

#define PACK(of,sf,zf,cf,pf) ((u8)((of)|((sf)<<1)|((zf)<<2)|((cf)<<3)|((pf)<<4)))

// capture SETcc results into the 5 packed bits and store the packed byte.
#define XADD_CAP(insn, seedptr, addend, outbyteptr, TREG) do {           \
    u8 of,sf,zf,cf,pf; TREG a=(addend);                                  \
    __asm__ volatile(insn " %5,%0\n\t"                                   \
        "seto %1\n\tsets %2\n\tsetz %3\n\tsetc %4\n\tsetp %6"            \
        : "+m"(*(seedptr)), "=m"(of), "=m"(sf), "=m"(zf), "=m"(cf),      \
          "+r"(a), "=m"(pf) :: "cc","memory");                           \
    *(outbyteptr) = PACK(of,sf,zf,cf,pf);                                \
  } while(0)

void _start(u64* shm, u64 tid) {
    u8* base = (u8*)shm;

    if (tid == 0) {
        // ---- seed the flag-battery words (each case gets its own 8-byte slot) ----
        *(u8*)(base+128)=0x7F; *(u8*)(base+136)=0xFF; *(u8*)(base+144)=0x80; *(u8*)(base+152)=0x00;
        *(u16*)(base+160)=0x7FFF; *(u16*)(base+168)=0xFFFF; *(u16*)(base+176)=0x8000; *(u16*)(base+184)=0x0000;
        *(u32*)(base+192)=0x7FFFFFFFu; *(u32*)(base+200)=0xFFFFFFFFu; *(u32*)(base+208)=0x80000000u; *(u32*)(base+216)=0u;
        *(u64*)(base+224)=0x7FFFFFFFFFFFFFFFull; *(u64*)(base+232)=0xFFFFFFFFFFFFFFFFull; *(u64*)(base+240)=0x8000000000000000ull; *(u64*)(base+248)=0ull;

        u8* out = base + 256;
        // width 8: addends 1,1,0xFF,0
        XADD_CAP("lock xaddb", (u8*)(base+128), (u8)1,    out+0*8, u8);
        XADD_CAP("lock xaddb", (u8*)(base+136), (u8)1,    out+1*8, u8);
        XADD_CAP("lock xaddb", (u8*)(base+144), (u8)0xFF, out+2*8, u8);
        XADD_CAP("lock xaddb", (u8*)(base+152), (u8)0,    out+3*8, u8);
        // width 16
        XADD_CAP("lock xaddw", (u16*)(base+160), (u16)1,      out+4*8, u16);
        XADD_CAP("lock xaddw", (u16*)(base+168), (u16)1,      out+5*8, u16);
        XADD_CAP("lock xaddw", (u16*)(base+176), (u16)0xFFFF, out+6*8, u16);
        XADD_CAP("lock xaddw", (u16*)(base+184), (u16)0,      out+7*8, u16);
        // width 32
        XADD_CAP("lock xaddl", (u32*)(base+192), (u32)1,          out+8*8,  u32);
        XADD_CAP("lock xaddl", (u32*)(base+200), (u32)1,          out+9*8,  u32);
        XADD_CAP("lock xaddl", (u32*)(base+208), (u32)0xFFFFFFFFu, out+10*8, u32);
        XADD_CAP("lock xaddl", (u32*)(base+216), (u32)0,          out+11*8, u32);
        // width 64
        XADD_CAP("lock xaddq", (u64*)(base+224), (u64)1,                     out+12*8, u64);
        XADD_CAP("lock xaddq", (u64*)(base+232), (u64)1,                     out+13*8, u64);
        XADD_CAP("lock xaddq", (u64*)(base+240), (u64)0xFFFFFFFFFFFFFFFFull, out+14*8, u64);
        XADD_CAP("lock xaddq", (u64*)(base+248), (u64)0,                     out+15*8, u64);
    }

    // ---- contended stamped sum (ALL threads incl tid0) ----
    u64 stamp = tid + 1;
    volatile u64* sum = (volatile u64*)(base + 0);
    volatile u64* scratch = (volatile u64*)(base + 64);
    for (u64 i = 0; i < ITERS; i++) {
        u64 s = stamp;
        __asm__ volatile("lock xaddq %0,%1" : "+r"(s), "+m"(*sum) :: "cc","memory");
        // keep non-tid0 threads churning near tid0's flag words w/o touching them
        u64 z = 1; __asm__ volatile("lock xaddq %0,%1" : "+r"(z), "+m"(*scratch) :: "cc","memory");
    }

    __asm__ volatile(".byte 0xCC");
}
