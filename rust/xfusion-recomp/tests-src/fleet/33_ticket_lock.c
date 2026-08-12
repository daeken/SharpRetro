// FLEET 33 — ticket (bakery) lock. Two 32-bit halves in one 64-bit word:
//   ticket @ +0 (low32), serving @ +4 (high32).
// acquire: `lock xadd` the ticket half by 1 -> my_ticket; spin on a PLAIN load
//          of the serving half until serving == my_ticket.
// release: PLAIN increment of the serving half (serving++). This is x86-idiomatic
//          and CORRECT: only the current lock-holder ever writes `serving`, so a
//          plain (non-atomic) RMW is legal — there is no concurrent writer to race.
// Through the JIT the plain `inc serving` is a non-atomic read-modify-write, but
// single-writer ⟹ fine. The test documents single-writer-plain-rmw legality:
// if the JIT somehow made the serving load in another thread observe a torn/
// stale half OR lost the plain increment, protected_ctr would lose updates.
//
// Assert EXACT: protected_ctr @8 == N*ITERS (mutual exclusion held). Also
// serving @4 == ticket @0 == N*ITERS at the end (every ticket taken was served).
//
// Layout (shm, zeroed): tk u32 @0 | sv u32 @4 | protected_ctr u64 @8
//
// CHECK: 0  32 N*ITERS
// CHECK: 4  32 N*ITERS
// CHECK: 8  64 N*ITERS
//
// objdump (clang 15, -O1, x86_64):
//   f0 0f c1 07           lock xadd %eax,(%rdi)         ; grab my ticket
//   f3 90                 pause                         ; spin
//   8b 47 04              mov 0x4(%rdi),%eax            ; plain load of serving
//   ff 47 04              incl 0x4(%rdi)                ; PLAIN rmw release (no lock)
typedef unsigned long long u64;
typedef unsigned int u32;

#define ITERS 20000

typedef struct {
    volatile u32 ticket;         // +0
    volatile u32 serving;        // +4
    volatile u64 protected_ctr;  // +8
} Shm;

static inline void mm_pause(void) { __asm__ volatile("pause" ::: "memory"); }

// lock xadd ticket += 1, returns pre-add value = my ticket number.
static inline u32 take_ticket(volatile u32 *tk) {
    u32 one = 1u;
    __asm__ volatile("lock xaddl %0, %1"
        : "+r"(one), "+m"(*tk)
        :
        : "memory", "cc");
    return one;
}

void _start(Shm *s, u64 tid) {
    (void)tid;
    for (int i = 0; i < ITERS; i++) {
        u32 my = take_ticket(&s->ticket);
        while (s->serving != my) mm_pause();   // plain load spin

        s->protected_ctr++;                    // critical section

        // release: PLAIN increment of serving (single-writer legal on x86).
        __asm__ volatile("" ::: "memory");
        s->serving = s->serving + 1u;
    }
    __asm__ volatile(".byte 0xCC");
}
