// 20_seqlock_reader_residual.c — MEMORY-ORDERING vertical #1 (C2 residual).
//
// Seqlock (double-buffered engine state, the classic game shape). The WRITER
// is fully ordered: it bumps `version` with `lock inc` (a full barrier —
// LSE-AL through the JIT), writes 4 PLAIN data words, then bumps `version`
// again. So on the writer side every data word is fenced between the two
// version increments. The READER does version/data.../version with PLAIN
// loads and retries on odd-or-changed version. On real x86 (TSO) a reader
// that sees an even, unchanged version around the payload is GUARANTEED a
// consistent snapshot. Through the JIT the reader's PLAIN load-load pairs are
// unordered ARM ldr/str (reader-side reordering measured ~78ppm), so the
// second version read can be hoisted before/among the data loads → a torn
// snapshot slips past the version guard. This test QUANTIFIES that residual
// in the most game-relevant idiom.
//
// EXPECT: C2-RESIDUAL  (nonzero torn_tally @+40 is the residual; NOT asserted
//         to an exact value — it is the measurement. The deterministic
//         invariant we DO assert is version == 2*ITERS: the writer's work is
//         ordered and countable regardless of what readers observe.)
//
// roles: tid0 = writer, tid>0 = readers.  N-agnostic (N>=1). Default N=8 → 7
//         concurrent readers hammering the payload while tid0 flips it.
//
// layout (byte offsets into the zeroed shm page):
//   +0   u64 version      (writer: lock inc ×2 per round)
//   +8   u64 data[0]       } four PLAIN payload words; a consistent snapshot
//   +16  u64 data[1]       } has all four == the round counter. A torn read
//   +24  u64 data[2]       } sees them unequal or version changed underneath.
//   +32  u64 data[3]       }
//   +40  u64 torn_tally    (readers: lock inc on inconsistency)  -- C2-RESIDUAL
//   +48  u64 clean_tally   (readers: lock inc on a good snapshot) -- diagnostic
//
// CHECK: 0 64 2*ITERS
//
// objdump (clang-15 -O1, x86_64):
//   writer version bumps:  f0 48 ff 07              lock incq (%rdi)
//   reader tally bump:     f0 48 ff 44 f7 28        lock incq 0x28(%rdi,%rsi,8)
//                          (clang folds torn@+40 / clean@+48 into an indexed
//                           lock incq; still the atomic we want, just computed offset)
//   payload writes/reads:  48 89 .. / 48 8b ..      plain mov (unordered ldr/str in JIT)

typedef unsigned long long u64;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    volatile u64* version = (volatile u64*)(shm + 0);
    // data is VOLATILE: pins the version/data/version program order at compile
    // time (else -O1 hoists the v1 re-read above the data loads, breaking the
    // seqlock bracket even on native x86 and contaminating the C2 claim).
    // volatile emits NO fence => still plain mov => unordered ldr in the JIT,
    // which is the exact reader-side reorder surface we're measuring.
    volatile u64* data = (volatile u64*)(shm + 1);  // data[0..3] @+8,+16,+24,+32
    u64* torn = shm + 5;                 // +40
    u64* clean = shm + 6;                // +48

    if (tid == 0) {
        // WRITER: fully ordered by the two lock incs.
        for (u64 r = 1; r <= ITERS; r++) {
            __atomic_fetch_add(version, 1, __ATOMIC_SEQ_CST); // -> odd
            data[0] = r;                                      // PLAIN
            data[1] = r;
            data[2] = r;
            data[3] = r;
            __atomic_fetch_add(version, 1, __ATOMIC_SEQ_CST); // -> even
        }
    } else {
        // READERS: seqlock retry loop; PLAIN payload loads are the surface.
        for (u64 i = 0; i < ITERS; i++) {
            u64 v0 = *version;                 // PLAIN load of version
            if (v0 & 1ull) continue;           // writer mid-update, retry slot
            u64 d0 = data[0];                  // PLAIN loads — reorderable in JIT
            u64 d1 = data[1];
            u64 d2 = data[2];
            u64 d3 = data[3];
            u64 v1 = *version;                 // PLAIN re-read
            if (v1 != v0) continue;            // version moved: expected retry
            // Same even version both sides ⟹ x86 guarantees d0==d1==d2==d3.
            if (d0 != d1 || d1 != d2 || d2 != d3) {
                __atomic_fetch_add(torn, 1, __ATOMIC_SEQ_CST);   // C2 residual
            } else {
                __atomic_fetch_add(clean, 1, __ATOMIC_SEQ_CST);
            }
        }
    }
    __asm__ volatile(".byte 0xCC");
}
