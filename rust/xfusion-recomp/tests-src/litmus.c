// Axis C: x86-TSO litmus shapes as guest code. Harness drives iterations:
// each _start call runs ONE thread-role of ONE test iteration.
// rdi = shm, rsi = role (0/1), rdx = variant:
//   0 = MP plain      (x86 FORBIDS r1=1,r2=0 — TSO: stores ordered, loads ordered)
//   1 = MP fenced     (mfence between — forbidden under everything)
//   2 = MP lock-rmw   (flag set via lock xadd — forbidden)
//   3 = SB plain      (x86 ALLOWS r1=0,r2=0 — store buffer; the calibration arm)
//   4 = SB mfence     (x86 FORBIDS 0,0 with mfence between store and load)
//   5 = LB plain      (x86 FORBIDS r1=1,r2=1 — loads not reordered w/ later stores)
// Layout: X @0, Y @8, R1 @16, R2 @24.
typedef unsigned long long u64;
typedef struct { volatile u64 x, y, r1, r2; } Shm;
static void mfence(void) { __asm__ volatile("mfence" ::: "memory"); }
void _start(Shm* s, u64 role, u64 variant) {
    switch (variant) {
    case 0: // MP plain
        if (role == 0) { s->x = 1; s->y = 1; }
        else { u64 a = s->y; u64 b = s->x; s->r1 = a; s->r2 = b; }
        break;
    case 1: // MP + mfence
        if (role == 0) { s->x = 1; mfence(); s->y = 1; }
        else { u64 a = s->y; mfence(); u64 b = s->x; s->r1 = a; s->r2 = b; }
        break;
    case 2: // MP via lock-rmw flag
        if (role == 0) { s->x = 1; __asm__ volatile("lock incq %0" : "+m"(s->y) :: "memory"); }
        else { u64 a = s->y; u64 b = s->x; s->r1 = a; s->r2 = b; }
        break;
    case 3: // SB plain
        if (role == 0) { s->x = 1; s->r1 = s->y; }
        else { s->y = 1; s->r2 = s->x; }
        break;
    case 4: // SB + mfence
        if (role == 0) { s->x = 1; mfence(); s->r1 = s->y; }
        else { s->y = 1; mfence(); s->r2 = s->x; }
        break;
    case 5: // LB plain
        if (role == 0) { u64 a = s->x; s->y = 1; s->r1 = a; }
        else { u64 b = s->y; s->x = 1; s->r2 = b; }
        break;
    }
    __asm__ volatile(".byte 0xCC");
}
