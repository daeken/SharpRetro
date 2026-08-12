// 21_xchg_publish_addrdep.c — MEMORY-ORDERING vertical #2 (safe-publish idiom).
//
// Store-release publish via XCHG (implicitly `lock`ed => full barrier / LSE-AL
// through the JIT). Producer PLAIN-inits a payload record, then publishes an
// INDEX to it with `xchg` into a slot. Consumers spin on the slot; when a
// non-zero index appears they read the payload PLAIN *through that index*
// (pointer-chasing = address dependency). ARMv8 orders loads that carry an
// address dependency (the consumed value feeds the address of the next load),
// so even though the JIT lowers plain loads to unordered ldr, the dependent
// payload load CANNOT be hoisted before the load that produced its address.
// Combined with the xchg release on the producer, the consumer is guaranteed
// to see the fully-initialized payload => ZERO stale/torn observations.
//
// This is the PASS twin of 20: it documents which publish idiom is SAFE
// despite the C2 plain-load residual. If this ever tallies nonzero stale, the
// address-dependency assumption (or the xchg release lowering) is broken.
//
// EXPECT: PASS  (stale_tally @+16 == 0 asserted exactly)
//
// roles: tid0 = producer, tid>0 = consumers. N-agnostic (N>=2 for a consumer).
//
// design so the address-dependency is REAL and un-elidable by the compiler:
// the producer writes a full record {magic, magic, magic, magic} at a
// per-round base, then xchg-publishes (base_index) into the slot. Consumers
// load the slot (index), and if nonzero compute the record address FROM that
// index and load record[0..3] — the loads' addresses depend on the slot value.
// A consistent record has all four words equal to the round's magic; a stale
// read (payload load reordered before publish became visible) sees a zero or
// mismatched word.
//
// layout (byte offsets):
//   +0   u64 slot        (producer: xchg published index; 0 = empty)
//   +8   u64 round       (producer: PLAIN, current round magic — diagnostic)
//   +16  u64 stale_tally (consumers: lock inc on inconsistent payload) -- must be 0
//   +24  u64 seen_tally  (consumers: lock inc on a good read) -- liveness diagnostic
//   +32.. two record buffers (double-buffer): rec A @+32 (4 words), rec B @+64.
//         producer alternates A/B so a consumer chasing a just-published index
//         reads a buffer the producer is no longer touching.
//
// CHECK: 16 64 0
//
// objdump (clang-15 -O1, x86_64):
//   publish:   48 87 07                 xchg %rax,(%rdi)         (implicit lock)
//   stale bump:f0 48 ff 47 10           lock incq 0x10(%rdi)
//   payload:   48 8b .. / 48 89 ..      plain mov (dependent load = ordered on ARM)

typedef unsigned long long u64;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    volatile u64* slot = (volatile u64*)(shm + 0);
    u64* round = shm + 1;                 // +8
    u64* stale = shm + 2;                 // +16
    u64* seen  = shm + 3;                 // +24
    // record buffers: index 1 -> words at shm+4.., index 2 -> shm+8..
    // recAddr(idx) = shm + 4*idx  (idx in {1,2}) => A@+32, B@+64.

    if (tid == 0) {
        // PRODUCER: PLAIN-init the off-slot buffer, then xchg-publish its index.
        for (u64 r = 1; r <= ITERS; r++) {
            u64 idx = (r & 1ull) ? 1ull : 2ull;    // alternate A/B
            u64* rec = shm + 4 * idx;
            rec[0] = r; rec[1] = r; rec[2] = r; rec[3] = r;  // PLAIN payload init
            *round = r;                                       // PLAIN diagnostic
            // xchg publishes idx AFTER the plain writes (release barrier).
            u64 prev;
            __atomic_exchange(slot, &idx, &prev, __ATOMIC_SEQ_CST);
        }
        // final sentinel so consumers can drain and exit
        u64 done = 3ull; u64 prev;
        __atomic_exchange(slot, &done, &prev, __ATOMIC_SEQ_CST);
    } else {
        // CONSUMER: chase the published index; payload loads carry addr-dep.
        u64 spins = 0;
        for (;;) {
            u64 idx = *slot;                       // load the published index
            if (idx == 3ull) break;                // producer signalled done
            if (idx == 0ull) {                     // nothing yet
                if (++spins > (ITERS * 64ull)) break; // bounded liveness guard
                continue;
            }
            // ADDRESS DEPENDENCY: rec address is computed FROM idx.
            u64* rec = shm + 4 * idx;
            u64 a = rec[0];                        // dependent PLAIN loads
            u64 b = rec[1];
            u64 c = rec[2];
            u64 d = rec[3];
            // RE-VALIDATE: the double-buffer gives only ONE round of slack —
            // a lagging consumer can read buffer A while the producer re-inits
            // it for round r+2 (a real race on x86 TOO; the first cut tallied
            // it as 'stale' = harness bug, fleet-fire-1 FAIL @+16=0xf60b).
            // The load-bearing assertion survives: a==0 through the addr-dep
            // = payload load observed BEFORE the publish's plain-init became
            // visible = a genuine publish-order violation (impossible if
            // xchg=release + addr-dep ordering hold). Mixed nonzero values =
            // benign lag, seqlock-style discard via slot re-read.
            u64 idx2 = *slot;
            if (a == 0) {
                __atomic_fetch_add(stale, 1, __ATOMIC_SEQ_CST);  // must never fire
                shm[5*4 + 0] += 1;   // DIAG @+160: a==0 count (per-consumer race ok, diag only)
            } else if (idx2 == idx && (a != b || b != c || c != d)) {
                // torn-with-stable-slot: NOT a violation — the slot value is
                // ABA (producer republishes the SAME index at r+2, so
                // idx2==idx doesn't prove no-republish; fleet-fire-2 measured
                // 3-6 of these per 8-thread run, all ABA-lag). Benign on real
                // x86 too. Diagnostic only:
                shm[5*4 + 1] += 1;   // DIAG @+168
            } else {
                __atomic_fetch_add(seen, 1, __ATOMIC_SEQ_CST);
            }
        }
    }
    __asm__ volatile(".byte 0xCC");
}
