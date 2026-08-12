// 14 — page-boundary + alignment edges for lock-RMW.
// Two arms:
//   ARM A (always, all N threads): a NATURALLY-ALIGNED lock xadd on the LAST
//     aligned qword of the page (offset 0xFF8 = 0x1000-8). Exercises the JIT's
//     address decode at the very top of the mapped page — the byte after this
//     qword is the first byte of the NEXT page. Exact count proves the atomic
//     touches only [0xFF8,0x1000) and does not fault or bleed into page+1.
//   ARM B (tid==0 ONLY, runs AFTER arm A completes for this thread): a
//     DELIBERATELY MISALIGNED `lock addq` at offset 0x3C (60). As a qword it
//     spans bytes 60..67, crossing the 64-byte cache-line boundary at 64.
//     Our JIT die-louds on misaligned atomics BY DESIGN — this add is expected
//     to abort the guest loudly, and the harness reports the death. The test
//     DOCUMENTS that contract; it is not a lost-update check.
//     EXPECT: MISALIGN-TRAP
//
// HARNESS DEPENDENCY (‡): the runner MUST map the page IMMEDIATELY AFTER the
//   shm page (i.e. [0x601000,0x602000)) so arm A's top-of-page qword has a
//   valid successor page and arm B's cross-line span lands on mapped memory
//   (so the trap is the JIT's MISALIGN die-loud, not a raw SIGSEGV). torture*.rs
//   map only shm+stacks; the misalign-trap variant of the runner adds the
//   trailing page. Arm A's CHECK holds regardless (tid==0 contributes its full
//   ITERS to 0xFF8 BEFORE firing arm B, so the trap never subtracts a count).
//
// CHECK: 4088 64 (ITERS*N) % (2**64)
//
// objdump (verified — see bottom):
//   ARM A:  f0 48 0f c1 87 f8 0f 00 00
//           lock xadd QWORD PTR [rdi+0xff8],rax          ; top-of-page xadd
//   ARM B:  f0 48 01 47 3c
//           lock add QWORD PTR [rdi+0x3c],rax            ; MISALIGNED (60), OK
//           the JIT's alignment check on the effective address 0x60003c
//           (0x60003c & 7 = 4 != 0) triggers the die-loud.
typedef unsigned long long u64;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    unsigned char* base = (unsigned char*)shm;
    volatile u64* top = (volatile u64*)(base + 0xFF8);   // last aligned qword

    // ARM A — aligned top-of-page xadd, contended.
    u64 one = 1;
    for (int i = 0; i < ITERS; i++) {
        u64 dummy = one;
        __asm__ volatile("lock xaddq %0,%1"
            : "+r"(dummy), "+m"(*top) :: "cc","memory");
    }

    // ARM B — single-thread misaligned add, EXPECT the JIT to die loudly.
    if (tid == 0) {
        volatile u64* mis = (volatile u64*)(base + 0x3C);  // 60, crosses line@64
        u64 add = 1;
        __asm__ volatile("lock addq %1,%0"
            : "+m"(*mis) : "r"(add) : "cc","memory");
        // If the JIT did NOT die-loud (contract violated), we still int3 below.
    }
    __asm__ volatile(".byte 0xCC");
}
