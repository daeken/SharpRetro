# Atomics test-fleet contract

Each test = ONE freestanding C file exercising ONE x64 atomic instruction /
edge-class under multi-thread fire, runnable through the atomics_torture
harness shape:

- `void _start(u64* shm, u64 tid)` — shm = shared page (identity-mapped,
  zeroed except where your header-comment says otherwise), tid = 0..N-1.
- End with `__asm__ volatile(".byte 0xCC");` (int3 = stop).
- NO libc, NO syscalls. `-nostdlib -static -O1 -fno-unroll-loops`,
  clang -target x86_64-linux-gnu.
- First comment block MUST machine-readably declare:
    // CHECK: <offset> <width:8|16|32|64> <expected-value-expr over N,ITERS>
  (one per line; the runner evals with python eval, N=threads ITERS=your
  per-thread loop count as a `#define ITERS`).
- Use `_Atomic` types / __atomic builtins / inline asm with lock prefixes —
  whatever emits the REAL instruction you're testing (verify with objdump
  which encodings you actually got; put the objdump line in a comment).
- Push a boundary: width-mixing on one cache line, cross-cache-line UNALIGNED
  atomics (note: our JIT die-louds on misaligned — a test that EXPECTS the
  loud death documents the contract), flag-consumption after lock-RMW
  (lock xadd's OF/SF/ZF read back via SETcc into shm), AH/high-8 operands,
  CMPXCHG failure-path flags, contended ABA, same-address mixed-width
  aliasing, lock-op on a PAGE-BOUNDARY-adjacent address, XCHG implicit lock,
  NEG/NOT/ADC/SBB lock forms (currently NOT routed atomically — a test that
  DETECTS their non-atomicity under contention documents the v1 gap
  honestly; expected-fail tests mark `// EXPECT: FAIL-UNTIL <mnemonic>`).
