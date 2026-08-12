// C1 acceptance: N JIT'd guest threads hammer shared atomics. Exact final
// counts ⟺ the JIT's lock-prefix lowering is genuinely atomic.
#include <stdatomic.h>
typedef unsigned long long u64;
typedef struct {
    _Atomic u64 xadd_sum;   // lock xadd      +0
    _Atomic u64 incs;       // lock inc       +8
    _Atomic u64 adds;       // lock add       +16
    _Atomic u64 decs;       // lock dec       +24
    _Atomic u64 swap_last;  // xchg-mem churn +32
    _Atomic unsigned lock;  // cmpxchg spinlock +40
    u64 protected_ctr;      // plain ++ under the lock +48
} Shm;
void _start(Shm* s) {
    for (int i = 0; i < 200000; i++) {
        atomic_fetch_add_explicit(&s->xadd_sum, 1, memory_order_seq_cst);
        atomic_fetch_add_explicit(&s->incs, 1, memory_order_relaxed);
        atomic_fetch_add_explicit(&s->adds, 2, memory_order_relaxed);
        atomic_fetch_sub_explicit(&s->decs, 1, memory_order_relaxed);
        atomic_exchange_explicit(&s->swap_last, (u64)i, memory_order_seq_cst);
        unsigned expected = 0;
        while (!atomic_compare_exchange_strong_explicit(&s->lock, &expected, 1,
                    memory_order_acquire, memory_order_relaxed)) expected = 0;
        s->protected_ctr++;
        atomic_store_explicit(&s->lock, 0, memory_order_release);
    }
    __asm__ volatile(".byte 0xCC");
}
