// 42_cmpxchg_aba.c — contended ABA on `lock cmpxchg`. Documents CAS's
// VALUE-semantics: a CAS(expect A) succeeds iff mem==A at the RMW instant,
// even if the slot went A→B→A arbitrarily many times in between — the actor
// cannot observe the intervening churn. That "spurious" success is LEGAL.
//
// ROLES by tid (no barrier; tid0 seeds the slot=A at entry, others spin-wait):
//   even tid  = CAS-EXPECT-A ACTOR: do { } while(!CAS(slot, A -> markerT));
//               then CAS(slot, markerT -> A) to release. ITERS full pairs.
//               markerT = 0xC000000000000000 | (tid+1)  — unique per thread,
//               so once slot==markerT only THIS thread can restore it (every
//               other actor's CAS(expect A) and every swapper's CAS see a
//               non-A / non-B value and retry). Each ACTOR completes EXACTLY
//               ITERS successful expect-A CASes.
//   odd  tid  = A↔B SWAPPER: per cycle, CAS(slot, A -> B) [retry], then
//               CAS(slot, B -> A) [retry]. ITERS full cycles. This is the
//               engine that opens ABA windows for the actors.
//
// WHAT IS EXACT (interleaving-INDEPENDENT), and WHAT IS NOT:
//   EXACT  cas_total  = ITERS * (#even tids) = ITERS*((N+1)//2)   — retry loops
//   EXACT  swap_total = ITERS * (#odd  tids) = ITERS*(N//2)
//   EXACT  slot final = A. Every actor's last write is a restore→A and every
//          swapper's last write is a restore→A; markers are unique so a marker
//          can only be cleared by its own writer. Whichever thread writes last
//          globally, its last write is →A. Hence slot==0xAAAAAAAA00000000.
//   NOT EXACT (deliberately NOT checked): the number of ABA/"spurious"
//          successes — i.e. successes where the slot had cycled A→B→A since the
//          actor's prior observation. This count is fundamentally unobservable
//          from inside a value-based CAS; asserting it would be asserting a
//          schedule. We store a diagnostic "spurious_seen" tally (successes
//          that immediately followed observing a non-A value) but do NOT CHECK
//          it — it merely proves ABA windows fired at all when nonzero.
//
// DEGENERATE N: N==1 → only tid0 (actor), no swappers, no ABA; still valid,
//   cas_total==ITERS, swap_total==0, slot==A. N==2 → 1 actor + 1 swapper.
//
// objdump (clang-15, -O1) — MUST emit the 64-bit locked CAS:
//   f0 48 0f b1 ..        lock cmpxchg %r64, (mem)
//
// shm layout (byte offsets; page zeroed on entry):
//   +0   slot        u64  the ABA target        -> final A = 0xAAAAAAAA00000000
//   +8   cas_total   u64  actor successes        -> ITERS*((N+1)//2)
//   +16  swap_total  u64  swapper cycles         -> ITERS*(N//2)
//   +24  seeded      u64  tid0 sets to 1 after seeding slot (spin gate)
//   +32  spurious[tid] via +32+tid*8  diagnostic, NOT checked
//
// CHECK: 8 64 ITERS*((N+1)//2)
// CHECK: 16 64 ITERS*(N//2)
// CHECK: 0 64 0xAAAAAAAA00000000
// CHECK: 24 64 1
#define ITERS 50000
typedef unsigned long long u64; typedef unsigned char u8;

#define VAL_A 0xAAAAAAAA00000000ull
#define VAL_B 0xBBBBBBBB00000000ull

// CAS wrapper: returns 1 on success (slot was `expect`), else 0; on failure
// `*got` receives the observed value (the accumulator-load on ZF=0).
static inline int cas64(volatile u64* p, u64 expect, u64 newv, u64* got) {
    u64 acc = expect;
    __asm__ volatile("lock cmpxchgq %2,%0"
        : "+m"(*p), "+a"(acc) : "r"(newv) : "cc","memory");
    *got = acc;
    return acc == expect;
}

void _start(u64* shm, u64 tid) {
    u8* base = (u8*)shm;
    volatile u64* slot     = (volatile u64*)(base + 0);
    volatile u64* cas_total = (volatile u64*)(base + 8);
    volatile u64* swap_total= (volatile u64*)(base + 16);
    volatile u64* seeded   = (volatile u64*)(base + 24);
    volatile u64* spurious = (volatile u64*)(base + 32 + tid*8);

    if (tid == 0) {
        *slot = VAL_A;
        __asm__ volatile("" ::: "memory");
        *seeded = 1;                  // release the spin gate
    } else {
        while (*seeded == 0) { __asm__ volatile("pause" ::: "memory"); }
    }

    u64 got;
    if ((tid & 1) == 0) {
        // ---- ACTOR: expect A, stamp unique marker, restore A ----
        u64 marker = 0xC000000000000000ull | (tid + 1);
        u64 local_ok = 0, local_spur = 0;
        for (u64 i = 0; i < ITERS; i++) {
            // acquire: CAS A -> marker, retrying past any B/other-marker
            int aba_window = 0;
            while (!cas64(slot, VAL_A, marker, &got)) {
                if (got != VAL_A) aba_window = 1;  // saw non-A: an ABA window
                __asm__ volatile("pause" ::: "memory");
            }
            local_ok++;
            if (aba_window) local_spur++;          // this success followed churn
            // release: CAS marker -> A (only we can; marker is unique)
            while (!cas64(slot, marker, VAL_A, &got)) {
                __asm__ volatile("pause" ::: "memory");
            }
        }
        u64 one = local_ok; // add exactly ITERS to the global actor tally
        __asm__ volatile("lock xaddq %0,%1" : "+r"(one), "+m"(*cas_total) :: "cc","memory");
        *spurious = local_spur;
    } else {
        // ---- SWAPPER: A->B then B->A, ITERS cycles ----
        u64 cycles = 0;
        for (u64 i = 0; i < ITERS; i++) {
            while (!cas64(slot, VAL_A, VAL_B, &got)) { __asm__ volatile("pause":::"memory"); }
            while (!cas64(slot, VAL_B, VAL_A, &got)) { __asm__ volatile("pause":::"memory"); }
            cycles++;
        }
        __asm__ volatile("lock xaddq %0,%1" : "+r"(cycles), "+m"(*swap_total) :: "cc","memory");
    }

    __asm__ volatile(".byte 0xCC");
}
