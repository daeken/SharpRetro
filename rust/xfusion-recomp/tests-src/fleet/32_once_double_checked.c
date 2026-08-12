// FLEET 32 — MSVC magic-static / std::call_once double-checked init.
//   guard: 0=uninit, 1=initializing (claimed), 2=done.
//   fast path: plain-load guard; if ==2, plain-load payload and USE it.
//   slow path: lock cmpxchg guard 0->1; winner writes payload (plain) then
//              stores guard=2 (release). losers spin until guard==2.
// The C2-sensitive part is the fast-path pair of PLAIN loads: read guard,
// then read payload. On x86-TSO the payload write (mov) happens-before the
// guard=2 store (program order, both plain), and a reader that sees guard==2
// is guaranteed to see the payload (loads are ordered, stores are ordered).
// On ARM there is NO control/address dependency from the guard-load to the
// payload-load, so the payload read can be satisfied by a STALE value even
// though guard==2 was observed — UNLESS the JIT inserts the acquire barrier
// that x86 semantics imply. bad_payload != 0 ⟺ a thread saw guard==2 with an
// unwritten (0) or wrong payload  →  C2-RESIDUAL.
//
// Also: init_runs @16 must be EXACTLY 1 (cmpxchg 0->1 admits one winner only).
//
// EXPECT: bad_payload==0 on a correct JIT. A nonzero final documents the ARM
//         load-load reordering gap and is marked  // EXPECT: C2-RESIDUAL.
//
// The published payload is a fixed magic; every guard==2 reader checks it.
//
// Layout (shm, zeroed): guard u32 @0 | payload u64 @8 | init_runs u64 @16
//                     | bad_payload u64 @24 | seen_done u64 @32
// #define MAGIC 0xC0FFEE1234567890
//
// CHECK: 0  32 2
// CHECK: 8  64 0xC0FFEE1234567890
// CHECK: 16 64 1
// CHECK: 24 64 0
//
// objdump (clang 15, -O1, x86_64):
//   8b 07                 mov (%rdi),%eax               ; plain guard load
//   f0 0f b1 0f           lock cmpxchg %ecx,(%rdi)      ; claim 0->1
//   48 89 47 08           mov %rax,0x8(%rdi)            ; plain payload store
//   c7 07 02 00 00 00     movl $0x2,(%rdi)              ; publish guard=2
typedef unsigned long long u64;
typedef unsigned int u32;

#define ITERS 20000
#define MAGIC 0xC0FFEE1234567890ull

typedef struct {
    volatile u32 guard;        // +0
    volatile u32 _pad;         // +4
    volatile u64 payload;      // +8
    volatile u64 init_runs;    // +16
    volatile u64 bad_payload;  // +24
    volatile u64 seen_done;    // +32
} Shm;

static inline void mm_pause(void) { __asm__ volatile("pause" ::: "memory"); }

// try to claim: lock cmpxchg guard 0->1, return 1 if we won.
static inline int claim(volatile u32 *g) {
    u32 prev;
    __asm__ volatile("lock cmpxchgl %2, %1"
        : "=a"(prev), "+m"(*g)
        : "r"(1u), "0"(0u)
        : "memory", "cc");
    return prev == 0u;
}

void _start(Shm *s, u64 tid) {
    (void)tid;
    for (int i = 0; i < ITERS; i++) {
        // FAST PATH: plain guard read, then plain payload read.
        u32 g = s->guard;
        if (g == 2u) {
            u64 p = s->payload;                 // C2-sensitive plain load
            if (p != MAGIC)
                __asm__ volatile("lock incq %0" : "+m"(s->bad_payload) :: "memory","cc");
            __asm__ volatile("lock incq %0" : "+m"(s->seen_done) :: "memory","cc");
            continue;
        }
        // SLOW PATH
        if (claim(&s->guard)) {
            // we are the sole initializer.
            __asm__ volatile("lock incq %0" : "+m"(s->init_runs) :: "memory","cc");
            s->payload = MAGIC;                 // plain init write
            __asm__ volatile("" ::: "memory");  // keep payload store before publish
            s->guard = 2u;                      // publish (plain release store)
        } else {
            // loser: spin until published, then validate payload.
            while (s->guard != 2u) mm_pause();
            u64 p = s->payload;                 // C2-sensitive plain load
            if (p != MAGIC)
                __asm__ volatile("lock incq %0" : "+m"(s->bad_payload) :: "memory","cc");
            __asm__ volatile("lock incq %0" : "+m"(s->seen_done) :: "memory","cc");
        }
    }
    __asm__ volatile(".byte 0xCC");
}
