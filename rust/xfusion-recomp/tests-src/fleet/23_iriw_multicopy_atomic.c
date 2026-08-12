// 23_iriw_multicopy_atomic.c — MEMORY-ORDERING vertical #4 (IRIW / MCA).
//
// Independent Reads of Independent Writes. Two writers publish 0->1 to two
// distinct locations with `lock xchg` (LSE store, AL, through the JIT); two
// readers observe the pair in OPPOSITE orders:
//   writer P (tid0): xchg x_i <- 1
//   writer Q (tid1): xchg y_i <- 1
//   reader R1 (tid2): a = x_i ; b = y_i     (x-before-y) — records obs
//   reader R2 (tid3): c = y_i ; d = x_i     (y-before-x) — adjudicates
// The forbidden IRIW outcome, in one slot: R1 sees (x=1,y=0) AND R2 sees
// (y=1,x=0). That requires the two independent stores to become visible in
// DIFFERENT orders to different readers = a multi-copy-atomicity (MCA)
// violation. x86-TSO forbids it; ARMv8.1 LSE acquire+release atomics are MCA,
// so a correct `lock xchg` lowering makes it airtight. Any nonzero tally is a
// real JIT correctness bug (e.g. lock-store lowered to plain-store + local
// barrier that isn't globally ordered).
//
// SHM IS ONLY 0x1000 (512 u64) and the harness free-runs once with NO host
// barrier/reset — so this is litmus2-style: a fixed ring of SLOTS one-shot
// slots, statistical overlap across the ring (‡ detection power is bounded by
// SLOTS per run; a MCA break shows on SOME slot, but absence-of-tally in one
// run is weaker evidence than a barriered litmus loop — this is the
// single-run harness's honest ceiling). Adjudication never false-positives:
// R2 only scores a slot R1 has already validly recorded.
//
// EXPECT: PASS  (iriw_tally @+0 == 0 asserted exactly)
//
// roles: tid0=writer-x, tid1=writer-y, tid2=reader R1, tid3=reader R2.
//        Needs N>=4; tid>=4 idle-exit.
//
// layout (byte offsets). header (8 words) then SLOTS slots of 3 words each:
//   +0   u64 iriw_tally  (R2: lock inc on a forbidden co-observation) == 0
//   +8   u64 r1_pos      (diag: R1 saw x=1,y=0)
//   +16  u64 r2_pos      (diag: R2 saw y=1,x=0)
//   +24  u64 coobs       (diag: slots R2 adjudicated with R1 recorded)
//   slot i base word = 8 + 3*i :  [0]=x_i (xchg<-1)  [1]=y_i (xchg<-1)
//                                  [2]=r1_obs (PLAIN: (a?1)|(b?2)|4valid)
//   SLOTS = ITERS (per-thread loop count). 8 + 3*ITERS <= 512 => ITERS<=168.
//
// CHECK: 0 64 0
//
// objdump (clang-15 -O1, x86_64):
//   publish x/y:  48 87 ..                 xchg r,(mem)   (implicit lock => LSE-AL)
//   iriw bump:    f0 48 ff 07              lock incq (%rdi)

typedef unsigned long long u64;
#define ITERS 160          // = SLOTS; 8 + 3*160 = 488 words <= 512

void _start(u64* shm, u64 tid) {
    if (tid > 3) { __asm__ volatile(".byte 0xCC"); return; }

    u64* iriw = shm + 0;     // +0
    u64* r1p  = shm + 1;     // +8
    u64* r2p  = shm + 2;     // +16
    u64* coob = shm + 3;     // +24
    #define SLOT(i)  (shm + 8 + 3*(i))

    if (tid == 0) {
        for (u64 i = 0; i < ITERS; i++) {
            volatile u64* x = (volatile u64*)(SLOT(i) + 0);
            u64 one = 1ull, prev;
            __atomic_exchange(x, &one, &prev, __ATOMIC_SEQ_CST);
        }
    } else if (tid == 1) {
        for (u64 i = 0; i < ITERS; i++) {
            volatile u64* y = (volatile u64*)(SLOT(i) + 1);
            u64 one = 1ull, prev;
            __atomic_exchange(y, &one, &prev, __ATOMIC_SEQ_CST);
        }
    } else if (tid == 2) {
        // R1: load x then y; record obs = (x?1)|(y?2)|4valid PLAIN in word[2].
        for (u64 i = 0; i < ITERS; i++) {
            volatile u64* x = (volatile u64*)(SLOT(i) + 0);
            volatile u64* y = (volatile u64*)(SLOT(i) + 1);
            u64 a = *x;                       // x first
            u64 b = *y;                       // then y
            SLOT(i)[2] = (a ? 1ull : 0) | (b ? 2ull : 0) | 4ull;
            if (a && !b) __atomic_fetch_add(r1p, 1, __ATOMIC_SEQ_CST);
        }
    } else { // tid == 3
        // R2: load y then x; adjudicate against R1's recorded obs.
        for (u64 i = 0; i < ITERS; i++) {
            volatile u64* x = (volatile u64*)(SLOT(i) + 0);
            volatile u64* y = (volatile u64*)(SLOT(i) + 1);
            u64 c = *y;                       // y first
            u64 d = *x;                       // then x
            if (c && !d) __atomic_fetch_add(r2p, 1, __ATOMIC_SEQ_CST);
            u64 o1 = SLOT(i)[2];
            if (o1 & 4ull) {                              // R1 recorded this slot
                __atomic_fetch_add(coob, 1, __ATOMIC_SEQ_CST);
                u64 r1_x_not_y = ((o1 & 3ull) == 1ull);   // R1 saw x=1,y=0
                u64 r2_y_not_x = (c && !d);               // R2 saw y=1,x=0
                if (r1_x_not_y && r2_y_not_x)
                    __atomic_fetch_add(iriw, 1, __ATOMIC_SEQ_CST);  // FORBIDDEN
            }
        }
    }
    __asm__ volatile(".byte 0xCC");
}
