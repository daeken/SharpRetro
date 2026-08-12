// Litmus v2: K striped slots per JIT run — both roles blast the stripe with
// NO in-guest sync; overlap is statistical (dense: ~5 insns/slot × 512 slots
// per barrier round-trip). Host resets + tallies at barriers.
// Layout: x[K] @0, y[K] @+8K... flat arrays: x_i @ i*40, y_i @ i*40+8,
// r1_i @ i*40+16, r2_i @ i*40+24 (40B stride keeps pairs off shared lines a
// bit; contention still high = good).
typedef unsigned long long u64;
#define K 512
void _start(u64* shm, u64 role, u64 variant) {
    for (u64 i = 0; i < K; i++) {
        u64* x  = shm + i*5 + 0;
        u64* y  = shm + i*5 + 1;
        u64* r1 = shm + i*5 + 2;
        u64* r2 = shm + i*5 + 3;
        volatile u64* vx = x; volatile u64* vy = y;
        switch (variant) {
        case 0: if (role==0) { *vx=1; *vy=1; } else { u64 a=*vy; u64 b=*vx; *r1=a; *r2=b; } break;
        case 1: if (role==0) { *vx=1; __asm__ volatile("mfence":::"memory"); *vy=1; }
                else { u64 a=*vy; __asm__ volatile("mfence":::"memory"); u64 b=*vx; *r1=a; *r2=b; } break;
        case 2: if (role==0) { *vx=1; __asm__ volatile("lock incq %0":"+m"(*y)::"memory"); }
                else { u64 a=*vy; u64 b=*vx; *r1=a; *r2=b; } break;
        case 3: if (role==0) { *vx=1; *r1=*vy; } else { *vy=1; *r2=*vx; } break;
        case 4: if (role==0) { *vx=1; __asm__ volatile("mfence":::"memory"); *r1=*vy; }
                else { *vy=1; __asm__ volatile("mfence":::"memory"); *r2=*vx; } break;
        case 5: if (role==0) { u64 a=*vx; *vy=1; *r1=a; } else { u64 b=*vy; *vx=1; *r2=b; } break;
        }
    }
    __asm__ volatile(".byte 0xCC");
}
