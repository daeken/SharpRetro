// XADD register-operand writeback widths: `lock xadd [m], r` puts OLD into r
// with x86 partial-write rules — 32-bit ZERO-EXTENDS to the full reg,
// 8/16-bit MERGE (upper bits preserved). Dirty-seed RCX, xadd at each width,
// store full RCX. Same own-#122 class as test 45 but on the XADD writeback
// path (bind_modrm_reg → write_operand Reg-arm), PLUS the multi-thread sum
// stays exact simultaneously (contended arm on separate words).
//
// objdump verified (clang-15 -O1):
//   f0 48 0f c1 0f     lock xadd %rcx,(%rdi)
//   f0 0f c1 4f 08     lock xadd %ecx,0x8(%rdi)
//   66 f0 0f c1 4f 0c  lock xadd %cx,0xc(%rdi)
//   f0 0f c0 4f 0e     lock xadd %cl,0xe(%rdi)
//
// PRESET: 0  64 0x1111111111111111
// PRESET: 8  32 0x22222222
// PRESET: 12 16 0x3333
// PRESET: 14 8  0x44
// CHECK: 16 64 0x1111111111111111
// CHECK: 24 64 0x22222222
// CHECK: 32 64 0xDEADBEEFCAFE3333
// CHECK: 40 64 0xDEADBEEFCAFEF044
// CHECK: 64 64 100000 * N * (N + 1) // 2
#define ITERS 100000
typedef unsigned long long u64;
void _start(u64* shm, u64 tid) {
    if (tid == 0) {
        u64 rcx_out;
        // W=64
        __asm__ volatile(
            "mov $0xDEADBEEFCAFEF00D, %%rcx\n\t"
            "lock xadd %%rcx, (%%rdi)\n\t"
            : "=&c"(rcx_out) : "D"(shm) : "memory", "cc");
        shm[2] = rcx_out;
        // W=32: OLD=0x22222222 → RCX must be ZEXT
        __asm__ volatile(
            "mov $0xDEADBEEFCAFEF00D, %%rcx\n\t"
            "lock xadd %%ecx, 8(%%rdi)\n\t"
            : "=&c"(rcx_out) : "D"(shm) : "memory", "cc");
        shm[3] = rcx_out;
        // W=16: OLD=0x3333 → merge into low16
        __asm__ volatile(
            "mov $0xDEADBEEFCAFEF00D, %%rcx\n\t"
            "lock xadd %%cx, 12(%%rdi)\n\t"
            : "=&c"(rcx_out) : "D"(shm) : "memory", "cc");
        shm[4] = rcx_out;
        // W=8: OLD=0x44 → merge into low8 (0x..F00D → 0x..F044)
        __asm__ volatile(
            "mov $0xDEADBEEFCAFEF00D, %%rcx\n\t"
            "lock xadd %%cl, 14(%%rdi)\n\t"
            : "=&c"(rcx_out) : "D"(shm) : "memory", "cc");
        shm[5] = rcx_out;
    }
    // contended arm (all threads, incl tid0 after its battery): exact sum
    for (int i = 0; i < ITERS; i++)
        __atomic_fetch_add(&shm[8], tid + 1, __ATOMIC_SEQ_CST);
    __asm__ volatile(".byte 0xCC");
}
