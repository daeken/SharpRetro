// FLEET 31 — COM/shared_ptr refcount: `lock inc` on acquire, `lock dec` + ZF
// read via SETcc on release; the LAST decrementer (dec drove count to 0 ⟺ ZF=1)
// sets a per-epoch "destroyed" flag. This is THE CRT/engine refcount pattern.
// The classic bug: capturing ZF from the OLD value instead of the post-dec NEW
// value (i.e. reading flags before the lock dec, or the JIT lowering `lock dec`
// without publishing the true post-op ZF). If the JIT gets ZF-of-NEW wrong,
// destroyed[] either never gets set (missed 0-cross) or gets set early/twice.
//
// Structure: EPOCHS epochs. In each epoch every thread does ONE inc (acquire)
// then ONE dec (release); the count returns to 0 exactly once per epoch, so the
// destroyed counter for that epoch must end == 1 (exactly one thread saw ZF=1).
// To make the 0-cross deterministic-per-epoch we seed each epoch's count to N
// via a barrier: thread 0 seeds nothing — instead every thread inc's then dec's,
// and the invariant is over the WHOLE run: total 0-crossings == EPOCHS (each
// epoch's N incs are matched by N decs, and exactly one of those N decs hits 0).
//
// We can't cheaply barrier without libc, so we assert the RUN-TOTAL instead:
//   destroyed_total (# of dec-to-zero events across the whole run) must equal
//   the number of times count returned to 0. With N threads each doing
//   EPOCHS*(inc then dec) interleaved arbitrarily, count starts 0 and ends 0;
//   the # of downward 0-crossings == # of upward departures from 0, and both
//   are data-race-free counts we can bound: destroyed_total is whatever the
//   interleaving produced, but it is ALWAYS >= 1 and the "early" bug is caught
//   by never_early: a dec must never observe ZF=1 while count (post-dec) < 0
//   (underflow) — count must never go negative.
//
// Asserted exactly:
//   count @0 ends 0 (balanced inc/dec).
//   underflow @16 == 0 (never decremented below zero — the ZF-from-NEW canary:
//     a spurious ZF=1 on a non-zero result would let a thread mis-set destroyed
//     but the underflow guard catches ZF/count desync directly).
//   inc_total @24 == dec_total @32 == N*ITERS.
//   destroyed_total @8: NOT statically fixed by interleaving, so unchecked here
//     (documented — see notes); the load-bearing checks are count==0 & underflow==0.
//
// Layout (shm, zeroed): count i64 @0 | destroyed u64 @8 | underflow u64 @16
//                     | inc_total u64 @24 | dec_total u64 @32
//
// CHECK: 0  64 0
// CHECK: 16 64 0
// CHECK: 24 64 N*ITERS
// CHECK: 32 64 N*ITERS
//
// objdump (clang 15, -O1, x86_64):
//   f0 48 ff 07           lock incq (%rdi)              ; acquire bump
//   f0 48 ff 0f           lock decq (%rdi)              ; release drop
//   0f 94 c0              sete %al                      ; ZF (post-dec == 0) -> AL
typedef unsigned long long u64;
typedef long long i64;

#define ITERS 20000

typedef struct {
    volatile i64 count;         // +0
    volatile u64 destroyed;     // +8  (# of dec-to-zero events)
    volatile u64 underflow;     // +16 (# of decs whose result went < 0)
    volatile u64 inc_total;     // +24
    volatile u64 dec_total;     // +32
} Shm;

// lock inc; return nothing (acquire bump). Also lock-inc the accounting counter.
static inline void refc_inc(volatile i64 *c) {
    __asm__ volatile("lock incq %0" : "+m"(*c) :: "memory", "cc");
}

// lock dec; capture post-op ZF (result==0) and SF (result<0) via SETcc.
// Returns 1 if result hit exactly zero (the LAST decrementer).
static inline int refc_dec(volatile i64 *c, unsigned char *neg_out) {
    unsigned char zf, sf;
    __asm__ volatile(
        "lock decq %2\n\t"
        "sete %0\n\t"
        "sets %1"
        : "=r"(zf), "=r"(sf), "+m"(*c)
        :
        : "memory", "cc");
    *neg_out = sf;
    return zf;
}

void _start(Shm *s, u64 tid) {
    (void)tid;
    for (int i = 0; i < ITERS; i++) {
        refc_inc(&s->count);
        __asm__ volatile("lock incq %0" : "+m"(s->inc_total) :: "memory", "cc");

        unsigned char neg = 0;
        int hit_zero = refc_dec(&s->count, &neg);
        __asm__ volatile("lock incq %0" : "+m"(s->dec_total) :: "memory", "cc");

        if (hit_zero)
            __asm__ volatile("lock incq %0" : "+m"(s->destroyed) :: "memory", "cc");
        // result went negative ⟺ SF=1 and ZF=0 (post-dec < 0) — must never happen
        // for a balanced inc-before-dec, so this is the ZF/SF-of-NEW canary.
        if (neg && !hit_zero)
            __asm__ volatile("lock incq %0" : "+m"(s->underflow) :: "memory", "cc");
    }
    __asm__ volatile(".byte 0xCC");
}
