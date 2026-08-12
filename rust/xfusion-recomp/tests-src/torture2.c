// C1 torture v2 (axis B): all 6 atomic ops × widths {8,16,32,64}, per-thread
// bit-stamped OR/AND/XOR (attributable lost-updates), all counters packed
// into TWO cache lines (maximal same-line contention), + a cmpxchg-16bit
// spinlock protecting a plain counter.
// Thread id arrives in rsi (harness sets it); shm in rdi.
#include <stdatomic.h>
typedef unsigned long long u64;
typedef unsigned int u32;
typedef unsigned short u16;
typedef unsigned char u8;
typedef struct {
    _Atomic u64 add64;        // +0
    _Atomic u32 add32;        // +8
    _Atomic u16 add16;        // +12
    _Atomic u8  add8;         // +14
    _Atomic u64 or_bits;      // +16  thread t sets bit t (OR) — final = mask
    _Atomic u64 and_bits;     // +24  thread t clears bit t (AND) — final = ~mask
    _Atomic u64 xor_bits;     // +32  thread t xors bit t ODD times — final = mask
    _Atomic u64 xadd_mix;     // +40
    _Atomic u16 lock16;       // +48  16-bit cmpxchg spinlock
    u64 protected_ctr;        // +56
} Shm;
void _start(Shm* s, u64 tid) {
    const int N = 100000;
    for (int i = 0; i < N; i++) {
        atomic_fetch_add_explicit(&s->add64, 1, memory_order_seq_cst);
        atomic_fetch_add_explicit(&s->add32, 1, memory_order_seq_cst);
        atomic_fetch_add_explicit(&s->add16, 1, memory_order_seq_cst);
        atomic_fetch_add_explicit(&s->add8, 1, memory_order_seq_cst);
        // stamped or/and/xor: only bit `tid` — a lost update names its thread
        atomic_fetch_or_explicit(&s->or_bits, 1ull << tid, memory_order_seq_cst);
        atomic_fetch_and_explicit(&s->and_bits, ~(1ull << tid), memory_order_seq_cst);
        atomic_fetch_xor_explicit(&s->xor_bits, 1ull << tid, memory_order_seq_cst);
        atomic_fetch_add_explicit(&s->xadd_mix, tid + 1, memory_order_seq_cst);
        u16 exp = 0;
        while (!atomic_compare_exchange_strong_explicit(&s->lock16, &exp, 1,
                    memory_order_acquire, memory_order_relaxed)) exp = 0;
        s->protected_ctr++;
        atomic_store_explicit(&s->lock16, 0, memory_order_release);
    }
    // odd-count xor: one extra flip so total flips = N+1 (odd) → bit ends SET
    atomic_fetch_xor_explicit(&s->xor_bits, 1ull << tid, memory_order_seq_cst);
    __asm__ volatile(".byte 0xCC");
}
