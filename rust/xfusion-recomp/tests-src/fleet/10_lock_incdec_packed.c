// 10 — lock inc/dec at ALL widths on adjacent, densely-packed fields of ONE
// cache line. Claim: adjacent-field NON-INTERFERENCE — each width's counter
// lands its exact count even though the fields are byte-adjacent (inc of the
// u8 at +14 must not RMW-widen into the u16 at +12, etc). Fields are ordered
// large→small so every field is naturally aligned AND has no padding gap
// (packed-by-layout; the JIT die-louds on misaligned atomics, so we do NOT
// misalign here — misalignment is 14's job).
//
// CHECK: 0  64 (ITERS*N) % (2**64)
// CHECK: 8  32 (ITERS*N) % (2**32)
// CHECK: 12 16 (ITERS*N) % (2**16)
// CHECK: 14 8  (ITERS*N) % (2**8)
// CHECK: 16 64 (-(ITERS*N)) % (2**64)
// CHECK: 24 32 (-(ITERS*N)) % (2**32)
// CHECK: 28 16 (-(ITERS*N)) % (2**16)
// CHECK: 30 8  (-(ITERS*N)) % (2**8)
//
// objdump (clang -target x86_64 -O1, verified — see header at bottom):
//   f0 48 ff 07             lock inc QWORD PTR [rdi]        ; incq  @0
//   f0 ff 47 08             lock inc DWORD PTR [rdi+0x8]    ; incl  @8
//   f0 66 ff 47 0c          lock inc WORD PTR [rdi+0xc]     ; incw  @12
//   f0 fe 47 0e             lock inc BYTE PTR [rdi+0xe]     ; incb  @14
//   f0 48 ff 4f 10          lock dec QWORD PTR [rdi+0x10]   ; decq  @16
//   f0 ff 4f 18             lock dec DWORD PTR [rdi+0x18]   ; decl  @24
//   f0 66 ff 4f 1c          lock dec WORD PTR [rdi+0x1c]    ; decw  @28
//   f0 fe 4f 1e             lock dec BYTE PTR [rdi+0x1e]    ; decb  @30
typedef unsigned long long u64;
typedef unsigned int u32;
typedef unsigned short u16;
typedef unsigned char u8;
#define ITERS 100000

void _start(u64* shm, u64 tid) {
    (void)tid;
    unsigned char* base = (unsigned char*)shm;
    volatile u64* q_inc = (volatile u64*)(base + 0);
    volatile u32* d_inc = (volatile u32*)(base + 8);
    volatile u16* w_inc = (volatile u16*)(base + 12);
    volatile u8*  b_inc = (volatile u8*) (base + 14);
    volatile u64* q_dec = (volatile u64*)(base + 16);
    volatile u32* d_dec = (volatile u32*)(base + 24);
    volatile u16* w_dec = (volatile u16*)(base + 28);
    volatile u8*  b_dec = (volatile u8*) (base + 30);
    for (int i = 0; i < ITERS; i++) {
        __asm__ volatile("lock incq %0" : "+m"(*q_inc) :: "cc", "memory");
        __asm__ volatile("lock incl %0" : "+m"(*d_inc) :: "cc", "memory");
        __asm__ volatile("lock incw %0" : "+m"(*w_inc) :: "cc", "memory");
        __asm__ volatile("lock incb %0" : "+m"(*b_inc) :: "cc", "memory");
        __asm__ volatile("lock decq %0" : "+m"(*q_dec) :: "cc", "memory");
        __asm__ volatile("lock decl %0" : "+m"(*d_dec) :: "cc", "memory");
        __asm__ volatile("lock decw %0" : "+m"(*w_dec) :: "cc", "memory");
        __asm__ volatile("lock decb %0" : "+m"(*b_dec) :: "cc", "memory");
    }
    __asm__ volatile(".byte 0xCC");
}
