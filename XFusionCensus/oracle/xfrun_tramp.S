// State switch for xfrun.c: load the case's full register state, jump to the
// instruction, and on return through xf_exit store everything back.
// State layout: gpr[16] @0x00, rflags @0x80, ymm[16][32] @0x88, mxcsr @0x288.

	.text
	.globl _xf_enter
	.globl _xf_exit
	.globl _xf_exit_taken

_xf_enter:
	push %rbx
	push %rbp
	push %r12
	push %r13
	push %r14
	push %r15
	mov %rsp, _g_host_rsp(%rip)

	vmovdqu 0x088(%rdi), %ymm0
	vmovdqu 0x0a8(%rdi), %ymm1
	vmovdqu 0x0c8(%rdi), %ymm2
	vmovdqu 0x0e8(%rdi), %ymm3
	vmovdqu 0x108(%rdi), %ymm4
	vmovdqu 0x128(%rdi), %ymm5
	vmovdqu 0x148(%rdi), %ymm6
	vmovdqu 0x168(%rdi), %ymm7
	vmovdqu 0x188(%rdi), %ymm8
	vmovdqu 0x1a8(%rdi), %ymm9
	vmovdqu 0x1c8(%rdi), %ymm10
	vmovdqu 0x1e8(%rdi), %ymm11
	vmovdqu 0x208(%rdi), %ymm12
	vmovdqu 0x228(%rdi), %ymm13
	vmovdqu 0x248(%rdi), %ymm14
	vmovdqu 0x268(%rdi), %ymm15
	ldmxcsr 0x288(%rdi)

	pushq 0x80(%rdi)
	popfq                          // flags live from here on: no flag-writing insns below

	mov 0x00(%rdi), %rax
	mov 0x08(%rdi), %rcx
	mov 0x10(%rdi), %rdx
	mov 0x18(%rdi), %rbx
	mov 0x20(%rdi), %rsp
	mov 0x28(%rdi), %rbp
	mov 0x30(%rdi), %rsi
	mov 0x40(%rdi), %r8
	mov 0x48(%rdi), %r9
	mov 0x50(%rdi), %r10
	mov 0x58(%rdi), %r11
	mov 0x60(%rdi), %r12
	mov 0x68(%rdi), %r13
	mov 0x70(%rdi), %r14
	mov 0x78(%rdi), %r15
	mov 0x38(%rdi), %rdi
	jmp *_g_code(%rip)

_xf_exit_taken:
	movl $1, _g_taken(%rip)        // a mov: flags stay as the instruction left them
_xf_exit:
	mov %rdi, _g_save_rdi(%rip)
	mov _g_state(%rip), %rdi
	mov %rax, 0x00(%rdi)
	mov %rcx, 0x08(%rdi)
	mov %rdx, 0x10(%rdi)
	mov %rbx, 0x18(%rdi)
	mov %rsp, 0x20(%rdi)
	mov %rbp, 0x28(%rdi)
	mov %rsi, 0x30(%rdi)
	mov %r8, 0x40(%rdi)
	mov %r9, 0x48(%rdi)
	mov %r10, 0x50(%rdi)
	mov %r11, 0x58(%rdi)
	mov %r12, 0x60(%rdi)
	mov %r13, 0x68(%rdi)
	mov %r14, 0x70(%rdi)
	mov %r15, 0x78(%rdi)
	mov _g_save_rdi(%rip), %rax
	mov %rax, 0x38(%rdi)
	mov _g_host_rsp(%rip), %rsp
	pushfq
	pop %rax
	mov %rax, 0x80(%rdi)
	cld
	vmovdqu %ymm0, 0x088(%rdi)
	vmovdqu %ymm1, 0x0a8(%rdi)
	vmovdqu %ymm2, 0x0c8(%rdi)
	vmovdqu %ymm3, 0x0e8(%rdi)
	vmovdqu %ymm4, 0x108(%rdi)
	vmovdqu %ymm5, 0x128(%rdi)
	vmovdqu %ymm6, 0x148(%rdi)
	vmovdqu %ymm7, 0x168(%rdi)
	vmovdqu %ymm8, 0x188(%rdi)
	vmovdqu %ymm9, 0x1a8(%rdi)
	vmovdqu %ymm10, 0x1c8(%rdi)
	vmovdqu %ymm11, 0x1e8(%rdi)
	vmovdqu %ymm12, 0x208(%rdi)
	vmovdqu %ymm13, 0x228(%rdi)
	vmovdqu %ymm14, 0x248(%rdi)
	vmovdqu %ymm15, 0x268(%rdi)
	stmxcsr 0x288(%rdi)
	ldmxcsr _g_host_mxcsr(%rip)
	vzeroupper
	pop %r15
	pop %r14
	pop %r13
	pop %r12
	pop %rbp
	pop %rbx
	ret

	.data
	.p2align 2
_g_host_mxcsr:
	.long 0x1f80
