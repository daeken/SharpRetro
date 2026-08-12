// CMPXCHG accumulator-writeback on FAILURE: RAX/EAX/AX/AL receives the
// LOADED value with x86 partial-write rules — 32-bit ZERO-EXTENDS to RAX,
// 8/16-bit MERGE (upper bits preserved). Pre-seed RAX with dirty upper bits;
// after a failed CAS at each width, store the FULL RAX for inspection.
// (The own-#122 class: a missing width-truncate in the writeback path shows
// as dirty bits where zext should be, or zeroed bits where merge should be.)
//
// tid0 only (single-writer slots); others spin on scratch for scheduling
// pressure. Memory seeds chosen so the CAS always FAILS (mem != expected).
//
// objdump verified (clang-15 -O1):
//   f0 48 0f b1 0f     lock cmpxchg %rcx,(%rdi)     [W=64]
//   f0 0f b1 4f 08     lock cmpxchg %ecx,0x8(%rdi)  [W=32]
//   66 f0 0f b1 4f 0c  lock cmpxchg %cx,0xc(%rdi)   [W=16]
//   f0 0f b0 4f 0e     lock cmpxchg %cl,0xe(%rdi)   [W=8]
//
// Layout: mem seeds @0(u64)=0x1111111111111111 @8(u32)=0x22222222
//         @12(u16)=0x3333 @14(u8)=0x44   (PRESET lines below)
// Results (full RAX after each failed CAS):
//   @16 u64: after W=64 fail → RAX = loaded = 0x1111111111111111
//   @24 u64: after W=32 fail → RAX = ZEXT(0x22222222) = 0x0000000022222222
//   @32 u64: after W=16 fail → RAX = merge: 0xDEADBEEFCAFE3333
//   @40 u64: after W=8  fail → RAX = merge: 0xDEADBEEFCAFEF044
//   @48 u64: ZF-capture pack (all four must be 0 = fail): 0
//
// PRESET: 0  64 0x1111111111111111
// PRESET: 8  32 0x22222222
// PRESET: 12 16 0x3333
// PRESET: 14 8  0x44
// CHECK: 16 64 0x1111111111111111
// CHECK: 24 64 0x22222222
// CHECK: 32 64 0xDEADBEEFCAFE3333
// CHECK: 40 64 0xDEADBEEFCAFEF044
// CHECK: 48 64 0
#define ITERS 1
typedef unsigned long long u64;
void _start(u64* shm, u64 tid) {
    if (tid != 0) {
        // scheduling pressure on private scratch
        volatile u64* sc = shm + 32 + tid;
        for (int i = 0; i < 100000; i++) *sc += 1;
        __asm__ volatile(".byte 0xCC");
    }
    u64 rax_out, zf_pack = 0, zf;

    // W=64: expected=0xDEADBEEFCAFEF00D (≠ mem 0x1111...) → fail, rax=loaded
    __asm__ volatile(
        "mov $0xDEADBEEFCAFEF00D, %%rax\n\t"
        "lock cmpxchg %%rcx, (%%rdi)\n\t"
        "setz %b1\n\t"
        : "=&a"(rax_out), "=&r"(zf) : "D"(shm), "c"(0x5555555555555555ull) : "memory", "cc");
    shm[2] = rax_out; zf_pack |= (zf & 1);

    // W=32: dirty upper RAX; failed CAS must ZERO-EXTEND eax-load
    __asm__ volatile(
        "mov $0xDEADBEEFCAFEF00D, %%rax\n\t"
        "lock cmpxchg %%ecx, 8(%%rdi)\n\t"
        "setz %b1\n\t"
        : "=&a"(rax_out), "=&r"(zf) : "D"(shm), "c"(0x66666666u) : "memory", "cc");
    shm[3] = rax_out; zf_pack |= (zf & 1) << 1;

    // W=16: dirty upper RAX; failed CAS must MERGE ax only
    __asm__ volatile(
        "mov $0xDEADBEEFCAFEF00D, %%rax\n\t"
        "lock cmpxchgw %%cx, 12(%%rdi)\n\t"
        "setz %b1\n\t"
        : "=&a"(rax_out), "=&r"(zf) : "D"(shm), "c"((unsigned short)0x7777) : "memory", "cc");
    shm[4] = rax_out; zf_pack |= (zf & 1) << 2;

    // W=8: dirty RAX; failed CAS must MERGE al only (note 0xF00D low byte = 0x0D
    // replaced by loaded 0x44 → 0x...F044)
    __asm__ volatile(
        "mov $0xDEADBEEFCAFEF00D, %%rax\n\t"
        "lock cmpxchgb %%cl, 14(%%rdi)\n\t"
        "setz %b1\n\t"
        : "=&a"(rax_out), "=&r"(zf) : "D"(shm), "c"((unsigned char)0x88) : "memory", "cc");
    shm[5] = rax_out; zf_pack |= (zf & 1) << 3;

    shm[6] = zf_pack;
    __asm__ volatile(".byte 0xCC");
}
