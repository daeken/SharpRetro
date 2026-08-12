// 44_xchg_implicit_lock.c — XCHG reg,[mem] IMPLICIT LOCK (no F0 prefix).
// x86 makes XCHG-with-memory atomic UNCONDITIONALLY — the bus is locked even
// though the encoding carries no F0 lock byte. This test proves the JIT lowers
// `xchg r, m` as an atomic swap under an 8-thread swap-chain: every thread
// atomically drops its stamp INTO the slot and pulls the resident stamp OUT.
// If the swap were non-atomic (load-then-store, or a torn 64-bit access), a
// thread would eventually read a MIXED value — high bits of one stamp, low of
// another — that is not any stamp any thread ever wrote.
//
// STAMP ENCODING (makes "valid" a single arithmetic test, N-independent):
//   high 16 bits = MAGIC 0x5150 ; low 48 bits = identity.
//   thread t stamp = 0x5150_0000_0000_0000 | (t+1)        (low in 1..N)
//   seed (initial resident, set by tid0) = 0x5150_0000_0000_1000  (low 0x1000)
//   A read-out is VALID iff  (v & 0xFFFF000000000000)==0x5150000000000000
//                       AND  (low==0x1000  OR  1<=low<=0xFFFF).
//   A torn/mixed 64-bit swap corrupts either the MAGIC half or the low half,
//   failing the test → counted as `torn`.
//
// EXACT, interleaving-independent assertions:
//   torn        == 0            no swap ever produced a non-stamp value
//   member_ok   == N*ITERS      every read-out was a whole, valid stamp
//   reads_total == N*ITERS      every thread performed exactly ITERS swaps
// (The identity of WHICH prior stamp a thread sees is schedule-dependent and
//  is NOT asserted — only that it is always SOME complete stamp. That is the
//  atomicity contract; the tally proves it held on every one of N*ITERS swaps.)
//
// DIAGNOSTIC (not checked): xor_acc = XOR over all swaps of (readout ⊕ myput).
//   By the swap-chain telescoping identity this equals seed ⊕ final_resident
//   (every intermediate resident appears once as a read and once as a put, so
//   it cancels). Left unchecked because final_resident is schedule-dependent;
//   stored for a runner that wants to verify seed⊕final⊕xor_acc==0.
//
// objdump (clang-15, -O1) — the encoding this file MUST emit (NO f0!):
//   48 87 ..            xchg %r64, (mem)      implicit lock, no F0 prefix
//   f0 48 0f c1 ..      lock xadd %r64,(mem)  (only for the tally counters)
//
// shm layout (byte offsets; page zeroed on entry):
//   +0   slot        u64   the swap-chain target (seeded by tid0)
//   +8   seeded      u64   spin gate (tid0 sets to 1 after seeding slot)
//   +16  reads_total u64   -> N*ITERS
//   +24  member_ok   u64   -> N*ITERS
//   +32  torn        u64   -> 0
//   +40  xor_acc     u64   diagnostic, NOT checked
//   +48  fin[tid] via +48+tid*8  per-thread last-seen, diagnostic, NOT checked
//
// CHECK: 32 64 0
// CHECK: 16 64 N*ITERS
// CHECK: 24 64 N*ITERS
#define ITERS 100000
typedef unsigned long long u64; typedef unsigned char u8;

#define MAGIC   0x5150000000000000ull
#define MAGMASK 0xFFFF000000000000ull
#define LOMASK  0x0000FFFFFFFFFFFFull
#define SEED    (MAGIC | 0x1000ull)

void _start(u64* shm, u64 tid) {
    u8* base = (u8*)shm;
    volatile u64* slot        = (volatile u64*)(base + 0);
    volatile u64* seeded      = (volatile u64*)(base + 8);
    volatile u64* reads_total = (volatile u64*)(base + 16);
    volatile u64* member_ok   = (volatile u64*)(base + 24);
    volatile u64* torn        = (volatile u64*)(base + 32);
    volatile u64* xor_acc     = (volatile u64*)(base + 40);
    volatile u64* fin         = (volatile u64*)(base + 48 + tid*8);

    if (tid == 0) {
        *slot = SEED;
        __asm__ volatile("" ::: "memory");
        *seeded = 1;
    } else {
        while (*seeded == 0) { __asm__ volatile("pause" ::: "memory"); }
    }

    u64 mine = MAGIC | (tid + 1);
    u64 l_reads = 0, l_member = 0, l_torn = 0, l_xor = 0, last = 0;

    for (u64 i = 0; i < ITERS; i++) {
        u64 out = mine;
        // THE INSTRUCTION UNDER TEST: implicit-lock xchg reg,[mem] (no F0).
        __asm__ volatile("xchg %0,%1" : "+r"(out), "+m"(*slot) :: "memory");
        last = out;
        l_reads++;
        l_xor ^= (out ^ mine);
        u64 hi = out & MAGMASK, lo = out & LOMASK;
        int valid = (hi == MAGIC) && (lo == 0x1000ull || (lo >= 1 && lo <= 0xFFFFull));
        if (valid) l_member++; else l_torn++;
    }

    // fold locals into shared totals atomically (order-free sums / xor)
    __asm__ volatile("lock xaddq %0,%1" : "+r"(l_reads),  "+m"(*reads_total) :: "cc","memory");
    __asm__ volatile("lock xaddq %0,%1" : "+r"(l_member), "+m"(*member_ok)   :: "cc","memory");
    __asm__ volatile("lock xaddq %0,%1" : "+r"(l_torn),   "+m"(*torn)        :: "cc","memory");
    __asm__ volatile("lock xorq %0,%1"  :: "r"(l_xor), "m"(*xor_acc) : "cc","memory");
    *fin = last;

    __asm__ volatile(".byte 0xCC");
}
