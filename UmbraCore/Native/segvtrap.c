// Native SIGSEGV handler — pure C, async-signal-safe (write(2) + _exit only).
// Managed [UnmanagedCallersOnly] handlers can't work: CoreCLR's reverse-
// pinvoke stub (native→cooperative-GC transition) runs BEFORE the method
// body and is NOT signal-safe → secondary SEGV in libcoreclr that masks the
// original. (— found via dmesg pc=libcoreclr+0x49ee80
// after "stripped to bare write(2)" managed handler still didn't fire.)
//
// (ε') 2026-06-12: chain to the PRIOR handler when fault-pc is OUTSIDE the
// game's region. The original install discarded oldact (sigaction(…, NULL)),
// so any managed-side null-deref (= CoreCLR would normally convert to
// NullReferenceException via its own SIGSEGV handler) became fatal here
// instead — even inside a try/catch. (ε) at CondVarKernel.cs:152 was the
// observed case (StackTrace((ulong*)w.Fp) with w.Fp==0); the (ε) point-fix
// guards that one site, but the structural class is "any managed null-deref
// after segvtrap_install()". Fix: save oldact per-signal; in the handler, if
// pc is NOT in the game's mapped range (set via segvtrap_set_game_range()
// after Image.Load), chain to the prior handler — i.e. let CoreCLR's own
// SIGSEGV→NRE machinery run. We still dump+exit for game-region faults
// (= the actual emulation crashes we built this for).
//
// ‡: range-test on pc, not /proc/self/maps lookup (the latter isn't
// async-signal-safe). If a host-native non-CoreCLR .so faults outside the
// game range, we chain to CoreCLR's handler too — which will likely
// re-fault and core; acceptable (segvtrap was masking that anyway).
// ‡: SA_NODEFER is kept so the FP-walk in the dump-path can re-fault
// without deadlocking; chained calls don't depend on it (CoreCLR's own
// sa_flags govern its own re-entry).
#define _GNU_SOURCE
#include <signal.h>
#include <unistd.h>
#include <ucontext.h>
#include <string.h>

static volatile unsigned long g_game_lo = 0, g_game_hi = 0;
static struct sigaction g_prev_segv, g_prev_bus, g_prev_ill;

static void hx(char *p, unsigned long v) {
    for (int i = 60; i >= 0; i -= 4) *p++ = "0123456789abcdef"[(v >> i) & 0xf];
}

static int chain(struct sigaction *prev, int sig, siginfo_t *info, void *uctx) {
    if (prev->sa_flags & SA_SIGINFO) {
        if (prev->sa_sigaction) { prev->sa_sigaction(sig, info, uctx); return 1; }
    } else if (prev->sa_handler && prev->sa_handler != SIG_DFL
            && prev->sa_handler != SIG_IGN) {
        prev->sa_handler(sig); return 1;
    }
    return 0;  // nothing to chain to (SIG_DFL/SIG_IGN/null)
}

static void handler(int sig, siginfo_t *info, void *uctx) {
    ucontext_t *uc = uctx;
    unsigned long pc = uc->uc_mcontext.pc;
    // (ε') chain-out: fault-pc NOT in game region → not an emulation
    // crash; let the prior handler (CoreCLR's hardware-exception
    // handler) take it. We don't dump (would interleave with managed
    // exception output), don't exit.
    if (g_game_hi > g_game_lo && !(pc >= g_game_lo && pc < g_game_hi)) {
        struct sigaction *p = sig == SIGSEGV ? &g_prev_segv
                            : sig == SIGBUS  ? &g_prev_bus
                            :                  &g_prev_ill;
        if (chain(p, sig, info, uctx)) return;
        // No prior handler (= installed before CoreCLR's, or it set
        // SIG_DFL). Fall through to dump — better than silent SIG_DFL
        // core with no context.
    }
    char buf[2048]; int o = 0;
    #define S(s) do { memcpy(buf+o, s, sizeof(s)-1); o += sizeof(s)-1; } while(0)
    #define H(v) do { hx(buf+o, (unsigned long)(v)); o += 16; } while(0)
    S("\n[segv-c] sig="); buf[o++]='0'+sig/10; buf[o++]='0'+sig%10;
    S(" fault=0x"); H(info->si_addr);
    S("\n[segv-c] pc=0x"); H(pc);
    S(" lr=0x"); H(uc->uc_mcontext.regs[30]);
    S(" sp=0x"); H(uc->uc_mcontext.sp);
    S("\n[segv-c] x0=0x"); H(uc->uc_mcontext.regs[0]);
    S(" x1=0x"); H(uc->uc_mcontext.regs[1]);
    S(" x2=0x"); H(uc->uc_mcontext.regs[2]);
    S(" x8=0x"); H(uc->uc_mcontext.regs[8]);
    S("\n[segv-c] x9=0x"); H(uc->uc_mcontext.regs[9]);
    S(" x19=0x"); H(uc->uc_mcontext.regs[19]);
    S(" x20=0x"); H(uc->uc_mcontext.regs[20]);
    S(" x29=0x"); H(uc->uc_mcontext.regs[29]);
    S("\n");
    // Walk a few frames via x29 chain (best-effort; may fault, but we're
    // already in the handler with the signal masked).
    unsigned long fp = uc->uc_mcontext.regs[29];
    for (int k = 0; k < 12 && fp > 0x1000 && (fp & 7) == 0; k++) {
        unsigned long nfp = ((unsigned long*)fp)[0];
        unsigned long ra  = ((unsigned long*)fp)[1];
        S("[segv-c]   #"); buf[o++]='0'+k/10; buf[o++]='0'+k%10;
        S(" ra=0x"); H(ra); S(" fp=0x"); H(fp); S("\n");
        if (nfp <= fp) break;
        fp = nfp;
    }
    write(2, buf, o);
    _exit(139);
}

void segvtrap_set_game_range(unsigned long lo, unsigned long hi) {
    g_game_lo = lo; g_game_hi = hi;
    char b[64]; int o = 0;
    memcpy(b+o, "[segv-c] game-range [0x", 23); o += 23;
    hx(b+o, lo); o += 16;
    memcpy(b+o, ",0x", 3); o += 3;
    hx(b+o, hi); o += 16;
    b[o++] = ')'; b[o++] = '\n';
    write(2, b, o);
}

static volatile int g_installed = 0;

void segvtrap_install(void) {
    // (ε') idempotent: u504 §7 showed install fires twice (line 1
    // = early static-init path; line 302 = RegisterLinuxHooks).
    // Second sigaction(…, &g_prev) would save OUR OWN handler →
    // chain() recurses on itself → hang. Guard with a flag; only
    // the first install captures the real prior (= CoreCLR PAL's).
    if (__sync_lock_test_and_set(&g_installed, 1)) {
        write(2, "[segv-c] (already armed)\n", 25);
        return;
    }
    struct sigaction sa = {0};
    sa.sa_sigaction = handler;
    sa.sa_flags = SA_SIGINFO | SA_ONSTACK | SA_NODEFER;
    sigfillset(&sa.sa_mask);
    sigaction(SIGSEGV, &sa, &g_prev_segv);
    sigaction(SIGBUS,  &sa, &g_prev_bus);
    sigaction(SIGILL,  &sa, &g_prev_ill);
    write(2, "[segv-c] native handler armed\n", 30);
}
