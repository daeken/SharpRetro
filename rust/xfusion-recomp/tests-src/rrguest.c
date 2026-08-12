// rr acceptance guest: ITERS × { slot = lock xadd(cursor,1); array[slot]=tid }
// The array's tid-sequence = the atomic interleaving (the thing rr pins).
// shm layout: +0 cursor(u64), +0x100.. array (u8 per slot).
#define ITERS 2000
typedef unsigned long long u64;
void _start(u64* shm, u64 tid) {
    unsigned char* arr = (unsigned char*)shm + 0x100;
    for (int i = 0; i < ITERS; i++) {
        u64 slot = __atomic_fetch_add(shm, 1, __ATOMIC_SEQ_CST);
        arr[slot] = (unsigned char)tid;
    }
    __asm__ volatile(".byte 0xCC");
}
