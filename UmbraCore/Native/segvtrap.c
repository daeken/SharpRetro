// Native SIGSEGV handler — pure C, async-signal-safe (write(2) + _exit only).
// Managed [UnmanagedCallersOnly] handlers can't work: CoreCLR's reverse-
// pinvoke stub (native→cooperative-GC transition) runs BEFORE the method
// body and is NOT signal-safe → secondary SEGV in libcoreclr that masks the
// original. (— found via dmesg pc=libcoreclr+0x49ee80
// after "stripped to bare write(2)" managed handler still didn't fire.)
#define _GNU_SOURCE
#include <signal.h>
#include <unistd.h>
#include <ucontext.h>
#include <string.h>

static void hx(char *p, unsigned long v) {
    for (int i = 60; i >= 0; i -= 4) *p++ = "0123456789abcdef"[(v >> i) & 0xf];
}

static void handler(int sig, siginfo_t *info, void *uctx) {
    ucontext_t *uc = uctx;
    char buf[2048]; int o = 0;
    #define S(s) do { memcpy(buf+o, s, sizeof(s)-1); o += sizeof(s)-1; } while(0)
    #define H(v) do { hx(buf+o, (unsigned long)(v)); o += 16; } while(0)
    S("\n[segv-c] sig="); buf[o++]='0'+sig/10; buf[o++]='0'+sig%10;
    S(" fault=0x"); H(info->si_addr);
    S("\n[segv-c] pc=0x"); H(uc->uc_mcontext.pc);
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

void segvtrap_install(void) {
    struct sigaction sa = {0};
    sa.sa_sigaction = handler;
    sa.sa_flags = SA_SIGINFO | SA_ONSTACK | SA_NODEFER;
    sigfillset(&sa.sa_mask);
    sigaction(SIGSEGV, &sa, NULL);
    sigaction(SIGBUS, &sa, NULL);
    sigaction(SIGILL, &sa, NULL);
    write(2, "[segv-c] native handler armed\n", 30);
}
