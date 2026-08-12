// 13 — lock bts/btr/btc on memory: STANDING NON-ATOMICITY DETECTOR.
// EXPECT: FAIL-UNTIL BTS
// The JIT does not (v1) route lock bt{s,r,c} through an atomic RMW — it lowers
// the bit-test-and-mutate as a plain read-modify-write of the containing word.
// Under 8-thread contention on ONE shared word, that loses updates: two threads
// read the same word, each mutates its own bit, and the second store clobbers
// the first thread's bit. This test makes that loss OBSERVABLE and STANDING —
// once the JIT routes bt{s,r,c} atomically (LSE ldset/ldclr on the word, or a
// cmpxchg loop), the three CHECKs flip green with no test edit.
//
// Stamps (contended on ONE word each; N<=8 → bit `tid`):
//   bts_word @0  : thread t sets   bit t once      → ATOMIC final = mask
//   btr_word @8  : starts all-ones, thread t clears bit t → ATOMIC final = ~mask
//   btc_word @16 : thread t toggles bit t ITERS+1 (odd) times → ATOMIC = mask
// Non-atomic routing → any of these lands < the full stamp = LOST-UPDATES.
//
// HARNESS DEPENDENCY (‡): runner pre-sets btr_word to all-ones, as torture2.rs
// does for and_bits:  *(u64*)(shm+8) = u64::MAX;
//
// CHECK: 0  64 ((1<<N)-1)
// CHECK: 8  64 ((~((1<<N)-1)) & ((1<<64)-1))
// CHECK: 16 64 ((1<<N)-1)
//
// objdump (clang -O1 with `lock` in the asm template — the encodings we assert
// the JIT sees; whether it ROUTES them atomically is exactly what fails v1):
//   f0 48 0f ab 07         lock bts QWORD PTR [rdi],rax      ; bts, reg bit index
//   f0 48 0f b3 47 08      lock btr QWORD PTR [rdi+0x8],rax  ; btr
//   f0 48 0f bb 47 10      lock btc QWORD PTR [rdi+0x10],rax ; btc
typedef unsigned long long u64;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    unsigned char* base = (unsigned char*)shm;
    volatile u64* bts_word = (volatile u64*)(base + 0);
    volatile u64* btr_word = (volatile u64*)(base + 8);
    volatile u64* btc_word = (volatile u64*)(base + 16);
    u64 b = tid;                          // bit index = tid

    // set our bit once (idempotent per-bit; contention loses it non-atomically)
    __asm__ volatile("lock btsq %1,%0" : "+m"(*bts_word) : "r"(b) : "cc","memory");
    // clear our bit once (word starts all-ones)
    __asm__ volatile("lock btrq %1,%0" : "+m"(*btr_word) : "r"(b) : "cc","memory");

    for (int i = 0; i < ITERS; i++) {
        // toggle our bit; odd total (ITERS+1) → ends SET when atomic
        __asm__ volatile("lock btcq %1,%0" : "+m"(*btc_word) : "r"(b) : "cc","memory");
    }
    __asm__ volatile("lock btcq %1,%0" : "+m"(*btc_word) : "r"(b) : "cc","memory");
    __asm__ volatile(".byte 0xCC");
}
