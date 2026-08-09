# sharpretro-jit — the Rust runtime crate

Design doc for the tiered-JIT runtime that consumes ArchCompiler's `Backends/Rust`
output. This is the second half of rung-4: the generated `recompiler.rs` (per-guest-ISA,
tier-agnostic) calls into the `Builder` trait defined here; each tier is a `Builder` impl.

## The two pieces

**Generated side (`Backends/Rust` → `recompiler.rs`)**: one function per guest instruction,
emitting IL-node calls into a `&mut dyn Builder`. Same shape as the C# `Recompiler.cs`
(which calls `IBuilder<AddrT>` methods). Guest-ISA-specific, tier-agnostic, host-arch-agnostic.

```rust
// generated per guest-ISA from the .isa via Backends/Rust
pub fn recompile_one(b: &mut impl Builder, state: StateRef, insn: u32, pc: u64) -> bool {
    // ADCS Xd, Xn, Xm  — mask/match dispatch, then the .isa semantics as builder calls:
    if (insn & 0x7FE0FC00) == 0x3A000000 {
        let rd = (insn >> 0) & 0x1F; let rn = (insn >> 5) & 0x1F; let rm = (insn >> 16) & 0x1F;
        let a = b.reg_read(RegFile::Gpr, rn, W64);
        let cin = b.reg_read(RegFile::Nzcv, C, W1);
        let (r, nzcv) = b.add_with_carry(a, b.reg_read(RegFile::Gpr, rm, W64), cin);
        b.reg_write(RegFile::Gpr, rd, r);
        b.reg_write(RegFile::Nzcv, ALL, nzcv);
        return true;
    }
    // ...
}
```

**Runtime crate (this doc)**: `Builder` trait + N tier impls + `BlockCache` + `NativeTable`.
Host-arch-parameterized (aarch64 first; x64 as a second `Emit` impl when wanted).

## The `Builder` trait

Reference shape = the existing C# `JitBase/IBuilder.cs` + `IRuntimeValue.cs` (~50 arithmetic/
logic/compare/cast methods on values; ~15 control-flow/call/local methods on the builder).
The Rust translation:

```rust
pub trait Builder {
    type Val: Copy;   // the SSA-value handle (opaque per-tier: a slot index for tier-0,
                      // a vreg id for tier-1, an LLVMValueRef for tier-2)

    // ── leaves ──
    fn literal(&mut self, ty: IlType, v: u128) -> Self::Val;
    fn reg_read(&mut self, file: RegFile, idx: u32, w: Width) -> Self::Val;
    fn reg_write(&mut self, file: RegFile, idx: u32, v: Self::Val);
    fn mem_read(&mut self, addr: Self::Val, w: Width) -> Self::Val;
    fn mem_write(&mut self, addr: Self::Val, v: Self::Val, w: Width);

    // ── arithmetic / logic (the ~40 ops from IRuntimeValue, width-carried on Val) ──
    fn add(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn sub(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    // ... and/or/xor/shl/shr/asr/mul/div/mod/neg/not/cmp{eq,ne,lt,le,gt,ge}/
    //     cast/bitcast/sext/zext/trunc/element-read/element-write/
    //     abs/sqrt/round/ceil/floor/clz/rbit ...
    // (full set = the C# IRuntimeValue's abstract methods, ~45)

    // ── control flow ──
    fn branch(&mut self, target: Self::Val, link: bool);   // ends the block
    fn cond(&mut self, c: Self::Val, then: impl FnOnce(&mut Self), else_: impl FnOnce(&mut Self));
    fn ternary(&mut self, c: Self::Val, a: Self::Val, b: Self::Val) -> Self::Val;
    fn local(&mut self, ty: IlType) -> LocalId;
    fn local_read(&mut self, l: LocalId) -> Self::Val;
    fn local_write(&mut self, l: LocalId, v: Self::Val);

    // ── the guest→native boundary (§ below) ──
    fn call_native(&mut self, slot: NativeSlot, args: &[Self::Val]) -> Option<Self::Val>;
    fn call_intrinsic(&mut self, id: IntrinsicId, args: &[Self::Val]) -> Option<Self::Val>;
}
```

`IlType` = `{I(signed, width), F(width), V128, Bool}` — matches ArchCompilerCore's `EType`
lattice. `RegFile` = per-guest-ISA enum (Gpr/Vec/Nzcv/Sr for aarch64; Gpr/Eflags/Seg for x86)
declared by the frontend, opaque to the tier — the tier just indexes `state.regs[file][idx]`.

## Tiers

### tier-0: dumb-fast template

Every `Builder` method emits a **fixed** aarch64 instruction sequence via `Emit`. No regalloc:
guest state lives in the `GuestState` struct (host x28 = state ptr), values live in scratch
regs x9-x15 in a stack-machine discipline (each `Val` = a scratch-reg slot). `add(a,b)` = one
`add x9, x10, x11`; `reg_read(Gpr, rn)` = one `ldr x9, [x28, #gpr_off + rn*8]`.

Compiles a block in μs. Every guest instruction ≈ 5-15 host instructions. This is what
executes on first-touch.

### tier-1: lightly-optimizing

Records IL-nodes into an `IlBlock` (the same tree the interpreter walks — one semantics
source), then a ~200L walker: linear-scan regalloc over the block, const-fold, dead-store
elim, redundant-load elim (two `reg_read(Gpr, rn)` with no intervening write → one load).
Emits via the same `Emit` as tier-0. ≈2-5× tier-0's code quality, ~ms compile.

### tier-2: LLVM

Lower `IlBlock` → LLVM IR (via `inkwell`), let LLVM optimize + codegen. Slow (~10-100ms/block),
best code. Only for genuinely hot blocks. Reference impl = the existing C# `LlvmJit/`.

### Host-arch dimension

Tier-0/1 are `Tier0<E: Emit>` where `Emit` supplies the machine-code encoding. `Aarch64Emit`
first (this box + Apple Silicon). `X64Emit` = a second impl when a consumer wants it — no
crate redesign, just another ~500L of encoding tables. Tier-2 (LLVM) is host-arch-free.

## `BlockCache` + hotspot promotion

```rust
struct BlockEntry {
    native: AtomicPtr<()>,   // the compiled entry point (atomic for background-swap)
    exec_count: AtomicU32,   // hotspot counter
    tier: AtomicU8,          // which tier compiled `native`
    guest_range: (u64, u64), // for invalidate()
}
struct BlockCache {
    map: DashMap<(u64 /*pc*/, u32 /*mode*/), BlockEntry>,
    // ...
}
```

Lookup path: `(pc, mode)` → entry → `native.load()` → jump. Cache miss = compile at tier-0
inline (μs) → store → jump. Every N executions bump `exec_count`; a background thread scans
for `exec_count > T1` (tier-0→1) and `> T2` (tier-1→2), recompiles, atomic-swaps `native`.
The old block stays valid until quiescent (a simple epoch counter).

`mode` in the key = guest CPU mode where relevant (x86: CS.D + prefix state that changes
decode; aarch64: EL if it matters). Blocks compiled under different modes are different blocks.

## The invalidate contract (loader ↔ JIT)

The consumer (a loader/host) patches guest memory at three times:

1. **post-map, pre-first-execute** — the bulk (import-table shims, vtable overwrites, guard
   patches). All before any block is JIT'd → the JIT reads guest memory as-laid-out and
   these ride for free. No invalidate needed.
2. **runtime host-side patches** — a finite, host-known set (auth/deadline fixups etc). Each
   patch site calls `block_cache.invalidate(addr..addr+len)` explicitly.
3. **guest self-modifying code** — the JIT write-protects any guest page containing a
   compiled block; a guest write to it faults → the fault handler invalidates every block
   whose `guest_range` intersects the page → un-write-protects → resumes (guest write
   completes, next execute recompiles at tier-0).

`invalidate(range)` = drop every `BlockEntry` whose `guest_range` intersects, plus (for
tier-2 blocks that inlined a `NativeTable` slot in that range) drop those too.

The JIT **always** reads guest memory as-currently-laid-out (the loader's mapped+patched
image), never a file image. Block discovery starts at the loader-supplied entrypoint and
follows branches; indirect targets are resolved at execute-time via the cache lookup.

## `call_native` — the guest→native boundary

The primitive that lets emulated code call directly into native library code. Two flavors:

- **`call_intrinsic(id, args)`** — the .isa's contract-intrinsics (svc, load-exclusive,
  paged-memory-fault). `id` indexes an `IntrinsicTable` populated by the arch's runtime
  helper impls. Signature is compile-time-known (declared in the frontend).
- **`call_native(slot, args)`** — guest imports (IAT slots). `slot` indexes a `NativeTable`
  populated by the loader (`native_table.set(slot, fn_ptr, sig)`). Signature comes from the
  loader (which knows the import's declared type).

Both lower the same way per tier:

- **tier-0**: spill all live guest state → `state`, load args → x0-x7 per AAPCS, `bl fn_ptr`,
  return value → the result Val's slot, reload guest state. ~20 host insns overhead.
- **tier-1**: the signature says which guest regs the callee reads/writes → spill only those
  + caller-saved host regs the block was using → `bl` → reload only what's needed. ~5-8 insns
  for a typical 2-arg call.
- **tier-2**: LLVM sees a normal call. If the native fn is LTO-visible (same Rust crate),
  LLVM can inline it — zero boundary cost for hot leaf calls (memcpy, math).

Signature declarations carry an ABI hint for cross-ABI cases:
```rust
struct Signature {
    args: Vec<IlType>,
    ret: Option<IlType>,
    guest_abi: GuestAbi,   // MS-x64, SysV, AAPCS — how the GUEST placed args
                           // (matters when the call site is a guest `call [IAT+N]` and
                           //  args are already in guest rcx/rdx/r8/r9; the tier reads
                           //  those from state and remaps to host x0-x3)
}
```

**Native → emulated (callbacks)**: a guest passes a fn-ptr to a native lib which later calls
it. The loader wraps outgoing guest fn-ptrs in a **reverse thunk** — a native aarch64 stub
that loads the `GuestState` ptr, sets up guest args from host x0-x7 → guest-ABI slots, calls
`block_cache.enter(guest_fn_ptr, mode)`, returns guest-rax → host-x0. Re-entrancy: `enter()`
saves/restores the JIT's host-reg allocation around the nested execution.

**Table indirection vs inlining**: tier-0/1 always call through the table (`ldr x16, [table, #slot*8]; blr x16`)
so a `native_table.set(slot, new_fn)` takes effect immediately with no invalidate. tier-2 MAY
inline (for LTO-visible hot leaves) — those blocks are tagged `inlined_slots: SmallVec<NativeSlot>`
and `native_table.set()` invalidates any tier-2 block that inlined that slot. This is the
"optimize the library independent of recompilation" property: swap a native impl, tier-0/1
callers pick it up instantly, tier-2 callers pay one recompile.

## Guest state layout

```rust
#[repr(C)]
pub struct GuestState<Regs> {
    pub regs: Regs,          // per-arch reg file struct (x86: gpr[16], eflags, seg[6], ...;
                             //                            aarch64: x[32], v[32], nzcv, sp, pc)
    pub pc: u64,
    pub mode: u32,
    pub gs_base: u64,        // guest TEB ptr — a struct field, NOT a real host register.
                             // guest `gs:[N]` compiles to `mem_read(state.gs_base + N)`.
    pub tls: *mut (),        // host-side per-guest-thread scratch
    // ...
}
```

Under emulator-only, the guest never touches real host CPU state (gs, TPIDR, etc) — everything
is a `GuestState` field. Native library code runs with normal host TLS untouched. There is no
context-swap at the boundary; only ABI-remap.

## Oracles

- **tier-0 vs interpreter**: same `IlBlock` → interpret vs tier-0-execute → state diff = 0.
  (The C# side already runs this exact test: `X86Machine` vs `X86Recompiler` on `CilJit`.)
- **tier-N vs tier-0**: any block, compile at both, execute both from same start-state,
  final-state diff = 0. Every tier oracles the one below.
- **generated recompiler.rs vs C# Recompiler.cs**: same .isa → both should decode the same
  insns to the same IL-node sequences. A tree-dump diff (rung-1b's form) at IL grain.

## Ordering

1. `Builder` trait + `IlType`/`Val` + `GuestState<Regs>` skeleton (this doc → code)
2. `Backends/Rust` in ArchCompiler → generate `recompiler.rs` for aarch64 (rung-4 proper;
   oracle = IL-tree-dump diff vs C# Recompiler.cs's builder-call sequence)
3. `Aarch64Emit` + tier-0 → first executing block (oracle = interpreter state-diff)
4. `BlockCache` + hotspot → tier-1
5. tier-2 (LLVM) + `NativeTable` + the reverse-thunk

x64-guest (via XFusion frontend) rides the same generated-recompiler shape once aarch64-guest
proves the pipeline; x64-host `Emit` when a consumer needs it.
