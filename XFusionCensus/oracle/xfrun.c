// xfrun — native x86-64 execution oracle for the XFusion lifter.
//
// Runs ONE instruction against a caller-supplied architectural state and reports the
// state afterwards. Built for x86_64 (on Apple silicon it runs under Rosetta 2, which
// executes AVX/AVX2/FMA/F16C even though its CPUID does not advertise them).
//
// Protocol (stdin -> stdout, little-endian, one record per case, repeated until EOF):
//   in : Case { u64 code_addr; u64 target; u8 code_len; u8 code[15]; State st; u8 mem[MEM_SIZE] }
//   out: u32 signal (0 = ran to completion); u32 taken; State st; u8 mem[MEM_SIZE]
// `target` (0 = none) is a branch destination: a stub there records that control
// went to it ("taken") rather than falling through past the instruction.
// The memory window is mapped at MEM_BASE; the instruction is copied to code_addr
// (page-mapped on demand, so RIP-relative operands can be steered into the window)
// followed by an absolute jump back to the exit stub.
#include <setjmp.h>
#include <signal.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/mman.h>
#include <unistd.h>

#define MEM_BASE 0x200000000ULL
#define MEM_SIZE 0x2000

typedef struct {
	uint64_t gpr[16];      // rax rcx rdx rbx rsp rbp rsi rdi r8..r15
	uint64_t rflags;
	uint8_t ymm[16][32];
	uint32_t mxcsr;
	uint32_t pad;
} State;                   // offsets: gpr 0, rflags 0x80, ymm 0x88, mxcsr 0x288

typedef struct __attribute__((packed)) {
	uint64_t code_addr;
	uint64_t target;
	uint8_t code_len;
	uint8_t code[15];
	State st;
	uint8_t mem[MEM_SIZE];
} Case;

extern void xf_enter(State *st);
extern void xf_exit(void);
extern void xf_exit_taken(void);
uint32_t g_taken;
void *g_code;              // read by the trampoline (jmp *g_code)
State *g_state;
uint64_t g_host_rsp, g_save_rdi;

static sigjmp_buf g_recover;
static volatile int g_signal;

static void on_signal(int sig, siginfo_t *si, void *uc) {
	(void) si; (void) uc;
	g_signal = sig;
	siglongjmp(g_recover, 1);
}

static int read_full(void *p, size_t n) {
	uint8_t *b = p;
	while(n) {
		ssize_t r = read(0, b, n);
		if(r <= 0) return 0;
		b += r; n -= (size_t) r;
	}
	return 1;
}

static void write_full(const void *p, size_t n) {
	const uint8_t *b = p;
	while(n) {
		ssize_t r = write(1, b, n);
		if(r <= 0) exit(2);
		b += r; n -= (size_t) r;
	}
}

int main(void) {
	uint8_t *mem = mmap((void *) MEM_BASE, MEM_SIZE, PROT_READ | PROT_WRITE,
		MAP_PRIVATE | MAP_ANON | MAP_FIXED, -1, 0);
	if(mem == MAP_FAILED) { perror("mmap mem"); return 1; }

	static uint8_t altstack[1 << 16];
	stack_t ss = { .ss_sp = altstack, .ss_size = sizeof altstack, .ss_flags = 0 };
	sigaltstack(&ss, NULL);
	struct sigaction sa;
	memset(&sa, 0, sizeof sa);
	sa.sa_sigaction = on_signal;
	sa.sa_flags = SA_SIGINFO | SA_ONSTACK | SA_NODEFER;
	sigaction(SIGSEGV, &sa, NULL);
	sigaction(SIGBUS, &sa, NULL);
	sigaction(SIGILL, &sa, NULL);
	sigaction(SIGFPE, &sa, NULL);
	sigaction(SIGTRAP, &sa, NULL);

	static Case c;
	static State st;
	uint64_t mapped_page = 0;
	while(read_full(&c, sizeof c)) {
		uint64_t page = c.code_addr & ~0xFFFULL;
		if(page != mapped_page) {
			if(mapped_page) munmap((void *) mapped_page, 0x2000);
			void *p = mmap((void *) page, 0x2000, PROT_READ | PROT_WRITE | PROT_EXEC,
				MAP_PRIVATE | MAP_ANON | MAP_FIXED, -1, 0);
			if(p == MAP_FAILED) { perror("mmap code"); return 1; }
			mapped_page = page;
		}
		uint8_t *code = (uint8_t *) c.code_addr;
		memcpy(code, c.code, c.code_len);
		// jmp qword ptr [rip+0] ; .quad xf_exit
		uint8_t *j = code + c.code_len;
		j[0] = 0xFF; j[1] = 0x25; j[2] = j[3] = j[4] = j[5] = 0;
		uint64_t ex = (uint64_t) &xf_exit;
		memcpy(j + 6, &ex, 8);
		__builtin___clear_cache((char *) code, (char *) j + 14);

		// Pages covering the target stub that the code mapping does not already cover,
		// mapped for this case only (never MAP_FIXED over the code pages).
		uint64_t target_map[2] = { 0, 0 };
		int unmappable = 0;
		if(c.target) {
			for(int k = 0; k < 2; k++) {
				uint64_t tp = ((c.target & ~0xFFFULL) + (uint64_t) k * 0x1000);
				if(k == 1 && tp > ((c.target + 14) & ~0xFFFULL)) break;
				if(tp >= mapped_page && tp < mapped_page + 0x2000) continue;
				// A hint, not MAP_FIXED: the destination may fall on something already
				// mapped (this process, Rosetta's runtime) that must not be replaced.
				void *p = mmap((void *) tp, 0x1000, PROT_READ | PROT_WRITE | PROT_EXEC,
					MAP_PRIVATE | MAP_ANON, -1, 0);
				if(p != (void *) tp) {
					if(p != MAP_FAILED) munmap(p, 0x1000);
					unmappable = 1;
					break;
				}
				target_map[k] = tp;
			}
		}
		if(unmappable) {
			for(int k = 0; k < 2; k++) if(target_map[k]) munmap((void *) target_map[k], 0x1000);
			uint32_t sig = 0xFFFF, zero = 0;
			write_full(&sig, 4);
			write_full(&zero, 4);
			write_full(&c.st, sizeof c.st);
			write_full(c.mem, MEM_SIZE);
			continue;
		}
		if(c.target) {
			uint8_t *t = (uint8_t *) c.target;
			t[0] = 0xFF; t[1] = 0x25; t[2] = t[3] = t[4] = t[5] = 0;
			uint64_t tx = (uint64_t) &xf_exit_taken;
			memcpy(t + 6, &tx, 8);
			__builtin___clear_cache((char *) t, (char *) t + 14);
		}
		memcpy(mem, c.mem, MEM_SIZE);
		st = c.st;
		g_code = code;
		g_state = &st;
		g_signal = 0;
		g_taken = 0;
		if(sigsetjmp(g_recover, 1) == 0)
			xf_enter(&st);
		uint32_t sig = (uint32_t) g_signal;
		write_full(&sig, 4);
		write_full(&g_taken, 4);
		write_full(&st, sizeof st);
		write_full(mem, MEM_SIZE);
		for(int k = 0; k < 2; k++) if(target_map[k]) munmap((void *) target_map[k], 0x1000);
	}
	return 0;
}
