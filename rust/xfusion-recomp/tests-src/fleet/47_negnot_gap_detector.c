// NEG/NOT lock forms — the KNOWN-UNROUTED v1 gap (0 occurrences in the
// CP2077 census; atomic_pre does not intercept unary-store RMWs). This test
// DETECTS their non-atomicity under contention so the gap has a standing
// alarm: if a future title uses lock neg/not, the XFAIL flips visible.
//
// Invariant design: `lock not` is an involution — 2×ITERS×N NOTs total
// (even count per thread) must return the word to its seed IF each NOT is
// atomic. Interleaved `lock neg` pairs likewise (neg;neg = identity when
// atomic). Torn read-modify-write breaks parity → word ≠ seed.
// A separate lock-add word proves the test itself ran.
//
// EXPECT: FAIL-UNTIL NEG
// objdump verified (clang-15 inline asm):
//   f0 48 f7 17    lock notq (%rdi)
//   f0 48 f7 1f 08 lock negq 0x8(%rdi)   [f0 48 f7 5f 08]
//
// PRESET: 0 64 0x5A5A5A5A5A5A5A5A
// PRESET: 8 64 0x1234567812345678
// CHECK: 0  64 0x5A5A5A5A5A5A5A5A
// CHECK: 8  64 0x1234567812345678
// CHECK: 16 64 ITERS * N
#define ITERS 100000
typedef unsigned long long u64;
void _start(u64* shm, u64 tid) {
    (void)tid;
    for (int i = 0; i < ITERS; i++) {
        __asm__ volatile("lock notq (%0)\n\tlock notq (%0)" :: "r"(shm) : "memory", "cc");
        __asm__ volatile("lock negq 8(%0)\n\tlock negq 8(%0)" :: "r"(shm) : "memory", "cc");
        __atomic_fetch_add(&shm[2], 1, __ATOMIC_SEQ_CST);
    }
    __asm__ volatile(".byte 0xCC");
}
