// 43_width_mixing_overlap.c — byte/word/dword/qword `lock or` all targeting
// OVERLAPPING bytes of ONE qword. Proves the JIT keeps mixed-width locked RMWs
// atomic against each other on the same cache line — a wider locked-or must not
// tear a narrower one's contribution, and vice-versa.
//
// DESIGN (interleaving- AND width-independent by construction):
//   thread t owns a unique bit at byte (t & 7):  BIT(t) = 1ull << ((t & 7)*8).
//   thread t's WIDTH class = t & 3:
//     0 → lock orb  at byte (t&7)                     [1 byte]
//     1 → lock orw  at word  containing byte (t&7)    [2 bytes]
//     2 → lock orl  at dword containing byte (t&7)    [4 bytes]
//     3 → lock orq  at the whole qword                [8 bytes]
//   Each thread ORs its single bit (immediate positioned within its sub-word).
//   OR is commutative + idempotent, so no matter the interleaving or the
//   width overlap (orq spans all bytes; orl@0 spans bytes0-3 overlapping the
//   orb@byte0..3 threads; etc.), the qword ends as the UNION of every thread's
//   bit:  final = OR over t in [0,N) of (1<<((t&7)*8)).
//   For N≤8 that is sum(1<<(t*8) for t in range(N)) — one 0x01 byte per active
//   thread, low→high. Repeating ITERS times is idempotent (still the union),
//   which is the point: hammering exposes any non-atomic read-modify-write as
//   a DROPPED or TORN bit.
//
// The overlap is genuine, verified offline (each width's addressed sub-word
// contains that thread's bit): tid1 orw@[0..1] holds bit@byte1; tid2 orl@[0..3]
// holds bit@byte2; tid3/tid7 orq@[0..7]; tid5 orw@[4..5]; tid6 orl@[4..7].
//
// N>8 wraps bytes (t&7) so two threads share a byte-bit — still correct (OR
// idempotent) but the union saturates at 0x0101010101010101; CHECK below uses
// the general union expr so it holds for any N.
//
// objdump (clang-15, -O1) — encodings this file MUST emit:
//   f0 08 ..            lock or %r8b,  (mem)     [orb]
//   66 f0 09 ..         lock or %r16,  (mem)     [orw]
//   f0 09 ..            lock or %r32,  (mem)     [orl]
//   f0 48 09 ..         lock or %r64,  (mem)     [orq]
//
// shm layout (byte offsets; page zeroed on entry):
//   +0   target   u64   the shared overlapped qword
//   +8   done[tid] via +8+tid*8   diagnostic (per-thread iters), NOT checked
//
// final target = union of all threads' bits:
// CHECK: 0 64 sum(1<<((t&7)*8) for t in range(N))
#define ITERS 200000
typedef unsigned long long u64; typedef unsigned u32; typedef unsigned short u16; typedef unsigned char u8;

void _start(u64* shm, u64 tid) {
    u8* base = (u8*)shm;
    unsigned bb = (unsigned)(tid & 7);          // byte index of this thread's bit
    unsigned c  = (unsigned)(tid & 3);          // width class

    for (u64 i = 0; i < ITERS; i++) {
        if (c == 0) {
            u8 m = 1u;                          // bit 0 of byte `bb`
            __asm__ volatile("lock orb %1,%0"
                : "+m"(*(u8*)(base + bb)) : "r"(m) : "cc","memory");
        } else if (c == 1) {
            unsigned wbase = bb & ~1u;          // word start (2-aligned)
            u16 m = (u16)(1u << ((bb - wbase) * 8));
            __asm__ volatile("lock orw %1,%0"
                : "+m"(*(u16*)(base + wbase)) : "r"(m) : "cc","memory");
        } else if (c == 2) {
            unsigned dbase = bb & ~3u;          // dword start (4-aligned)
            u32 m = (u32)(1u << ((bb - dbase) * 8));
            __asm__ volatile("lock orl %1,%0"
                : "+m"(*(u32*)(base + dbase)) : "r"(m) : "cc","memory");
        } else {
            u64 m = (u64)1 << (bb * 8);         // bit anywhere in the qword
            __asm__ volatile("lock orq %1,%0"
                : "+m"(*(u64*)base) : "r"(m) : "cc","memory");
        }
    }
    // diagnostic (not checked): record that this thread completed ITERS
    *(volatile u64*)(base + 8 + tid*8) = ITERS;

    __asm__ volatile(".byte 0xCC");
}
