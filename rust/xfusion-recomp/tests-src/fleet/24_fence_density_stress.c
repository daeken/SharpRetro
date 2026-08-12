// 24_fence_density_stress.c — MEMORY-ORDERING vertical #5 (fence-tax + dmb path).
//
// Maximum-frequency alternation of PLAIN-write / mfence / PLAIN-read on ONE
// shared cache line across all N threads (>=3 the intent; default N=8). This
// is the dmb-ish-heavy path: every mfence lowers to `dmb ish` in the JIT, so
// this run is dominated by fence execution + the resulting store-buffer drains
// and cross-core line ping-pong on the single hot word.
//
// Two things it establishes:
//  (1) CORRECTNESS of the fence-dense path: the deterministic canary is a
//      lock-xadd accumulator hit once per iter. mfence does NOT make the plain
//      write/read atomic — so we do NOT assert anything about the plain scratch
//      word; we assert only the LOCKED accumulator == ITERS*N. If the dmb-heavy
//      lowering ever mis-serializes the lock op (e.g. a fence reorder drops an
//      xadd's global visibility) the accumulator loses updates. Robust to any
//      plain reorder by construction.
//  (2) FENCE-TAX METRIC (‡ measured at run time, not here — DO NOT RUN policy):
//      the harness prints `[atomics_torture v2] N threads × ITERS, <wall>s`.
//      The fence tax = wall(this test) − wall(a fence-free sibling doing the
//      same lock-xadd loop with the two mfences removed), same N/ITERS. Report
//      it as (Δwall, Δwall/(2*ITERS*N) = per-fence cost). Placeholder below is
//      filled from the actual paired run; left ‡ until measured.
//        // METRIC fence_tax_ns_per_fence: ‡ TBD (paired-run Δwall / (2·ITERS·N))
//
// EXPECT: PASS  (accum @+0 == ITERS*N asserted exactly; N-agnostic, N>=1,
//          intent N>=3 for real one-line contention)
//
// layout (byte offsets):
//   +0   u64 accum   (lock xadd 1 per iter; exact == ITERS*N)     -- canary
//   +8   u64 line    (the ONE hot word: PLAIN write then PLAIN read; NOT checked)
//   +16  u64 spins   (diag: accumulated read-back scratch to defeat DCE)
//
// CHECK: 0 64 ITERS*N
//
// objdump (clang-15 -O1, x86_64):
//   plain write: 48 89 47 08              mov %rax,0x8(%rdi)   (PLAIN store)
//   fence:       0f ae f0                 mfence               (-> dmb ish)
//   plain read:  48 03 47 08              add 0x8(%rdi),%rax   (PLAIN load, folded into accumulate)
//   canary:      f0 48 ff 07              lock incq (%rdi)     (+1 folds to lock inc; LSE ldaddal-class)
//   sink pub:    f0 48 01 47 10           lock add %rax,0x10(%rdi)

typedef unsigned long long u64;
#define ITERS 200000

void _start(u64* shm, u64 tid) {
    u64* accum = shm + 0;                     // +0
    volatile u64* line = (volatile u64*)(shm + 1);  // +8, the hot word
    u64* spins = shm + 2;                     // +16

    u64 sink = 0;
    for (u64 i = 0; i < ITERS; i++) {
        // max-frequency plain-write / dmb / plain-read on the single hot line:
        *line = (tid << 32) ^ i;                  // PLAIN store
        __asm__ volatile("mfence":::"memory");    // -> dmb ish
        u64 v = *line;                            // PLAIN load
        __asm__ volatile("mfence":::"memory");    // -> dmb ish
        sink += v;                                // accumulate to defeat DCE
        // deterministic canary: one atomic accumulate per iteration.
        __atomic_fetch_add(accum, 1, __ATOMIC_SEQ_CST);
    }
    // publish the sink so the plain read cannot be optimized away.
    __atomic_fetch_add(spins, sink | 1ull, __ATOMIC_SEQ_CST);
    __asm__ volatile(".byte 0xCC");
}
