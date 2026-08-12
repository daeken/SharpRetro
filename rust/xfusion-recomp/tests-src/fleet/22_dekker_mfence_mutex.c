// 22_dekker_mfence_mutex.c — MEMORY-ORDERING vertical #3 (mfence-makes-Dekker).
//
// Dekker's mutual-exclusion via PLAIN stores + `mfence` — the store-buffer (SB)
// shape real spinlock fallbacks use. Two threads (tid 0 and 1) each: set their
// flag, MFENCE, then check the other's flag; on conflict, defer by `turn`.
// The critical section increments a PLAIN counter. Without a fence, the flag
// store sits in the store buffer while the peer's flag load reads stale 0 =>
// both enter the CS => lost updates on the plain counter (the SB anti-pattern).
// With `mfence` after each flag store (lowered to `dmb ish` in the JIT), the
// store is drained before the peer-flag load => real mutual exclusion on BOTH
// x86 (TSO+mfence) and ARM (dmb). So the PLAIN counter lands EXACTLY 2*ITERS.
//
// This is the positive proof: mfence closes the SB hole that C2 would other-
// wise leave open. It is the deterministic mutex counterpart to litmus2 case1
// (which only observes the SB reorder rather than protecting against it).
//
// EXPECT: PASS  (counter @+40 == 2*ITERS asserted exactly; requires N>=2)
//
// roles: EXACTLY tid 0 and tid 1 participate; tid>=2 idle-exit. The check is
//        independent of N as long as N>=2 (the two participants each do ITERS
//        critical sections => 2*ITERS).
//
// layout (byte offsets):
//   +0   u64 flag0   (tid0 wants-in; PLAIN store, then mfence)
//   +8   u64 flag1   (tid1 wants-in; PLAIN store, then mfence)
//   +16  u64 turn    (whose turn to defer; xchg-published to be safe)
//   +40  u64 counter (PLAIN ++ inside CS; exact iff mutual exclusion holds)
//
// CHECK: 40 64 2*ITERS
//
// objdump (clang-15 -O1, x86_64):
//   flag store:  48 c7 07 01 00 00 00     movq $1,(%rdi)     (PLAIN)
//   drain:       0f ae f0                 mfence             (-> dmb ish)
//   peer load:   48 83 3f 00              cmpq $0,(%rdi)     (PLAIN)
//   CS inc:      48 ff 47 28              incq 0x28(%rdi)    (PLAIN, non-locked)

typedef unsigned long long u64;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    if (tid > 1) { __asm__ volatile(".byte 0xCC"); return; }

    volatile u64* flag0 = (volatile u64*)(shm + 0);
    volatile u64* flag1 = (volatile u64*)(shm + 1);
    volatile u64* turn  = (volatile u64*)(shm + 2);   // +16
    u64* counter = shm + 5;                            // +40

    volatile u64* my   = (tid == 0) ? flag0 : flag1;
    volatile u64* other= (tid == 0) ? flag1 : flag0;
    u64 me = tid;
    u64 him = 1ull - tid;

    for (u64 i = 0; i < ITERS; i++) {
        // announce intent, then DRAIN the store buffer before peeking peer.
        *my = 1;
        __asm__ volatile("mfence":::"memory");     // -> dmb ish
        while (*other) {                            // peer also wants in?
            if (*turn != me) {                      // not our turn: back off
                *my = 0;
                __asm__ volatile("mfence":::"memory");
                while (*turn != me) { /* spin until ceded */ }
                *my = 1;
                __asm__ volatile("mfence":::"memory");
            }
        }
        // ---- critical section: PLAIN read-modify-write of the shared counter.
        u64 c = *counter;
        *counter = c + 1;
        // ---- exit: cede turn to peer, drop flag.
        u64 h = him; u64 prev;
        __atomic_exchange(turn, &h, &prev, __ATOMIC_SEQ_CST);
        *my = 0;
        __asm__ volatile("mfence":::"memory");
    }
    __asm__ volatile(".byte 0xCC");
}
