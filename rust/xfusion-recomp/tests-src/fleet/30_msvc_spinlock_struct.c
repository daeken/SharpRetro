// FLEET 30 — MSVC-style spinlock (lock cmpxchg acquire / plain-store release,
// _mm_pause spin) protecting a 4-field struct. The "no torn struct" test: every
// acquirer must observe the whole 4-field publish of the previous holder or none
// of it. On x86-TSO the plain-store release (mov lock,0) publishes the preceding
// plain field writes, and the NEXT acquirer's `lock cmpxchg` is a full-barrier
// acquire — so the release/acquire chain holds even lowered onto ARM, PROVIDED
// the JIT keeps program order for the plain stores + gives the cmpxchg true
// acq semantics. torn_flag != 0 ⟺ the JIT tore the publish.
//
// Layout (shm, zeroed): lock u32 @0 | f0 @8 f1 @16 f2 @24 f3 @32 | torn u64 @40
// Invariant held under the lock: f0==f1==f2==f3 (all equal). Each holder reads
// the snapshot, flags if unequal (= torn publish), then writes all four to base+1.
//
// CHECK: 0  32 0
// CHECK: 8  64 N*ITERS
// CHECK: 16 64 N*ITERS
// CHECK: 24 64 N*ITERS
// CHECK: 32 64 N*ITERS
// CHECK: 40 64 0
//
// objdump (clang 15, -O1 -fno-unroll-loops, x86_64):
//   f0 0f b1 0f            lock cmpxchg %ecx,(%rdi)      ; acquire
//   f3 90                  pause                         ; _mm_pause spin
//   c7 47 00 00 00 00 00   movl $0x0,(%rdi)              ; plain-store release
typedef unsigned long long u64;
typedef unsigned int u32;

#define ITERS 20000

typedef struct {
    volatile u32 lock;   // +0
    volatile u32 _pad;   // +4
    volatile u64 f0;     // +8
    volatile u64 f1;     // +16
    volatile u64 f2;     // +24
    volatile u64 f3;     // +32
    volatile u64 torn;   // +40
} Shm;

static inline void mm_pause(void) {
    __asm__ volatile("pause" ::: "memory");
}

// lock cmpxchg 0 -> 1 acquire; spin with pause on contention.
static inline void acquire(volatile u32 *lk) {
    for (;;) {
        u32 prev;
        __asm__ volatile("lock cmpxchgl %2, %1"
            : "=a"(prev), "+m"(*lk)
            : "r"(1u), "0"(0u)
            : "memory", "cc");
        if (prev == 0u) return;
        mm_pause();
    }
}

// plain-store release (x86: a plain mov is a release store).
static inline void release(volatile u32 *lk) {
    __asm__ volatile("" ::: "memory");   // no reorder of prior stores past this
    *lk = 0u;
}

void _start(Shm *s, u64 tid) {
    (void)tid;
    for (int i = 0; i < ITERS; i++) {
        acquire(&s->lock);
        // snapshot must be internally consistent (all four equal).
        u64 a = s->f0, b = s->f1, c = s->f2, d = s->f3;
        if (!(a == b && b == c && c == d))
            s->torn = 1;
        u64 base = a + 1;
        s->f0 = base;
        s->f1 = base;
        s->f2 = base;
        s->f3 = base;
        release(&s->lock);
    }
    __asm__ volatile(".byte 0xCC");
}
