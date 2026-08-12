// 11 — lock add/sub with per-thread SIGNED operands crossing zero repeatedly.
// Two claims:
//   (a) TELESCOPE to a known value — a contended accumulator that every thread
//       both adds to and subtracts from nets to 0 (tele64), while pure-add /
//       pure-sub accumulators land on ITERS*Σ(tid+1) and its two's-complement
//       negation (sub64 spends most of its life deeply negative = crossing
//       zero into the high half of the u64 range).
//   (b) FLAG behavior of lock add/sub read back via SETcc — a SINGLE-THREAD
//       arm (tid==0 only, private offsets → no contention → deterministic):
//       sub 1 from 0 (borrow: CF=1,SF=1,ZF=0), add 1 back to 0 (carry-out
//       CF=1,ZF=1,SF=0), then signed-overflow 0x7fffffff+1 (OF=1,SF=1).
//   Σ(tid+1 for tid in 0..N) = N*(N+1)/2.
//
// CHECK: 0  64 0
// CHECK: 8  64 (ITERS*N*(N+1)//2) % (2**64)
// CHECK: 16 64 (-(ITERS*N*(N+1)//2)) % (2**64)
// CHECK: 24 32 0x80000000
// CHECK: 32 8  1
// CHECK: 33 8  1
// CHECK: 34 8  0
// CHECK: 35 8  1
// CHECK: 36 8  1
// CHECK: 37 8  1
// CHECK: 38 8  1
//
// objdump (verified — see bottom):
//   f0 48 01 07             lock add QWORD PTR [rdi],rax       ; tele64 add
//   f0 48 29 07             lock sub QWORD PTR [rdi],rax       ; tele64 sub
//   f0 48 01 47 08          lock add QWORD PTR [rdi+0x8],rax   ; sum64
//   f0 48 29 47 10          lock sub QWORD PTR [rdi+0x10],rax  ; sub64
//   f0 83 6f 18 01          lock sub DWORD PTR [rdi+0x18],0x1  ; flag arm
//   0f 92 / 0f 98 / 0f 94 / 0f 90   setb/sets/sete/seto
typedef unsigned long long u64;
typedef unsigned int u32;
typedef unsigned char u8;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    unsigned char* base = (unsigned char*)shm;
    volatile u64* tele = (volatile u64*)(base + 0);
    volatile u64* sum  = (volatile u64*)(base + 8);
    volatile u64* sub  = (volatile u64*)(base + 16);
    u64 op = tid + 1;                       // per-thread signed operand
    for (int i = 0; i < ITERS; i++) {
        __asm__ volatile("lock addq %1,%0" : "+m"(*tele) : "r"(op) : "cc","memory");
        __asm__ volatile("lock subq %1,%0" : "+m"(*tele) : "r"(op) : "cc","memory");
        __asm__ volatile("lock addq %1,%0" : "+m"(*sum)  : "r"(op) : "cc","memory");
        __asm__ volatile("lock subq %1,%0" : "+m"(*sub)  : "r"(op) : "cc","memory");
    }
    if (tid == 0) {
        volatile u32* acc = (volatile u32*)(base + 24);
        volatile u8*  s32 = (volatile u8*)(base + 32); // setc  after sub 1 (from 0)
        volatile u8*  s33 = (volatile u8*)(base + 33); // sets  after sub 1
        volatile u8*  s34 = (volatile u8*)(base + 34); // setz  after sub 1
        volatile u8*  s35 = (volatile u8*)(base + 35); // setz  after add 1 -> 0
        volatile u8*  s36 = (volatile u8*)(base + 36); // setc  after add 1 -> 0
        volatile u8*  s37 = (volatile u8*)(base + 37); // seto  after overflow add
        volatile u8*  s38 = (volatile u8*)(base + 38); // sets  after overflow add
        // acc = 0 (page is zeroed). sub 1 -> 0xFFFFFFFF: borrow CF=1, SF=1, ZF=0.
        __asm__ volatile(
            "lock subl $1,%0\n\t setc %1\n\t sets %2\n\t setz %3"
            : "+m"(*acc), "=m"(*s32), "=m"(*s33), "=m"(*s34) :: "cc","memory");
        // add 1 -> 0: carry-out CF=1, ZF=1, SF=0.
        __asm__ volatile(
            "lock addl $1,%0\n\t setz %1\n\t setc %2"
            : "+m"(*acc), "=m"(*s35), "=m"(*s36) :: "cc","memory");
        // 0x7fffffff, then +1 -> 0x80000000: signed overflow OF=1, SF=1.
        __asm__ volatile(
            "lock addl $0x7fffffff,%0\n\t"
            "lock addl $1,%0\n\t seto %1\n\t sets %2"
            : "+m"(*acc), "=m"(*s37), "=m"(*s38) :: "cc","memory");
    }
    __asm__ volatile(".byte 0xCC");
}
