// FLEET 34 — Chase-Lev work-stealing deque (simplified), owner + 3+ thieves.
//   tid 0 = OWNER: push/pop at `bottom` with PLAIN index ops, one `lock cmpxchg`
//                  on `top` only for the contended last-element case.
//   tid 1..N-1 = THIEVES: steal at `top` via `lock cmpxchg` (t -> t+1).
// The essential Chase-Lev fence is the mfence in pop() between the bottom-store
// and the top-load (an SB shape: without it, owner and a thief can both take the
// last item). The `lock cmpxchg` on top is the arbitration primitive under test.
//
// Correctness property = EXACTLY-ONCE consumption. Every pushed item carries a
// unique stamp (owner-generated monotonic 1..PUSHED). On consume (owner-pop OR
// thief-steal) we tally two atomic checksums:
//   consumed_count += 1     (lock inc)
//   consumed_sum   += stamp (lock xadd)
// PUSHED = ITERS*CAP (owner-only, so N-independent). Exactly-once ⟺
//   consumed_count == PUSHED  AND  consumed_sum == PUSHED*(PUSHED+1)/2.
// A double-consume inflates both; a lost item deflates both; a stamp-swap that
// preserved count would still break the sum — the count+sum pair pins exactly-once.
//
// Owner pushes CAP items per epoch then drains until pop returns EMPTY; thieves
// steal until `done`. A dropped cmpxchg / torn index shows as count/sum mismatch.
//
// shm layout (zeroed): top u64 @0 | bottom u64 @8 | done u64 @16
//   | consumed_count u64 @24 | consumed_sum u64 @32 | (pad) @40..@64
//   | buf[CAP] u64 @64 ...  (CAP=64 -> buf spans @64..@576, within the 4K page)
// #define CAP 64
//
// CHECK: 16 64 1
// CHECK: 24 64 ITERS*64
// CHECK: 32 64 (ITERS*64)*(ITERS*64+1)//2
//
// objdump (clang 15, -O1 -fno-unroll-loops, x86_64):
//   0f ae f0              mfence                        ; pop SB-fence
//   f0 48 0f b1 0e        lock cmpxchg %rcx,(%rsi)      ; top arbitration
//   f0 48 ff 47 18        lock incq 0x18(%rdi)          ; consumed_count
//   f0 48 0f c1 47 20     lock xadd %rax,0x20(%rdi)     ; consumed_sum
typedef unsigned long long u64;

#define ITERS 2000     // owner epochs
#define CAP   64        // deque capacity (must be power of two for the mask)
#define MASK  (CAP - 1)
#define EMPTY 0xFFFFFFFFFFFFFFFFull   // sentinel (stamps are >= 1, never all-ones)

typedef struct {
    volatile u64 top;             // +0
    volatile u64 bottom;          // +8
    volatile u64 done;            // +16
    volatile u64 consumed_count;  // +24
    volatile u64 consumed_sum;    // +32
    volatile u64 _pad[3];         // +40..+64
    volatile u64 buf[CAP];        // +64 ...
} Shm;

static inline void mfence(void) { __asm__ volatile("mfence" ::: "memory"); }
static inline void mm_pause(void) { __asm__ volatile("pause" ::: "memory"); }
static inline void barrier(void) { __asm__ volatile("" ::: "memory"); }

// lock cmpxchg on `top`: if *top==old, set to old+1, return 1 (won); else 0.
static inline int cas_top(volatile u64 *top, u64 old) {
    u64 prev;
    u64 neu = old + 1;
    __asm__ volatile("lock cmpxchgq %2, %1"
        : "=a"(prev), "+m"(*top)
        : "r"(neu), "0"(old)
        : "memory", "cc");
    return prev == old;
}

static inline void tally(Shm *s, u64 stamp) {
    __asm__ volatile("lock incq %0" : "+m"(s->consumed_count) :: "memory", "cc");
    __asm__ volatile("lock xaddq %0, %1"
        : "+r"(stamp), "+m"(s->consumed_sum) :: "memory", "cc");
}

// OWNER push: plain buf write, release barrier, plain bottom bump.
static inline void push(Shm *s, u64 v) {
    u64 b = s->bottom;
    s->buf[b & MASK] = v;
    barrier();                 // buf store visible before bottom publish
    s->bottom = b + 1;
}

// OWNER pop: returns stamp or EMPTY.
static inline u64 pop(Shm *s) {
    u64 b = s->bottom - 1;
    s->bottom = b;             // plain store
    mfence();                  // Chase-Lev SB-fence: bottom-store before top-load
    u64 t = s->top;
    if ((long long)t > (long long)b) {   // deque was empty
        s->bottom = b + 1;               // restore
        return EMPTY;
    }
    u64 v = s->buf[b & MASK];
    if (t == b) {
        // last element: race with thieves for it via cmpxchg on top.
        if (!cas_top(&s->top, t)) v = EMPTY;   // a thief won
        s->bottom = b + 1;                     // reset to empty
    }
    return v;                  // t < b : uncontended, no top write
}

// THIEF steal: returns stamp, EMPTY (deque empty), or EMPTY on lost race.
static inline u64 steal(Shm *s) {
    u64 t = s->top;
    barrier();
    u64 b = s->bottom;
    if ((long long)t >= (long long)b) return EMPTY;   // empty
    u64 v = s->buf[t & MASK];
    if (!cas_top(&s->top, t)) return EMPTY;           // lost race -> retry later
    return v;
}

void _start(Shm *s, u64 tid) {
    if (tid == 0) {
        // OWNER
        u64 stamp = 1;
        for (int e = 0; e < ITERS; e++) {
            for (int k = 0; k < CAP; k++)
                push(s, stamp++);
            // drain: pop until the deque reports empty.
            for (;;) {
                u64 v = pop(s);
                if (v == EMPTY) break;
                tally(s, v);
            }
        }
        barrier();
        s->done = 1;           // publish termination
    } else {
        // THIEF — steal until owner signals done; bounded for harness liveness.
        u64 guard = (u64)ITERS * (u64)CAP * 64ull + 1024ull;
        while (!s->done) {
            u64 v = steal(s);
            if (v != EMPTY) tally(s, v);
            else mm_pause();
            if (guard-- == 0) break;   // safety: never hang the runner
        }
        // final sweep after done, drain any straggler the owner left contended.
        for (int k = 0; k < CAP + 4; k++) {
            u64 v = steal(s);
            if (v != EMPTY) tally(s, v);
        }
    }
    __asm__ volatile(".byte 0xCC");
}
