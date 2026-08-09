//! `BlockCache` — the driver loop that turns "compile one insn" into "run a program".
//! Owns the (pc, mode) → CompiledBlock map + the compile-on-miss path + the invalidate
//! seam. Per DESIGN.md step ④ + the loader↔JIT contract (§invalidate).
//!
//! v1 = single-thread, tier-0 only. Hotspot promotion (exec_count → tier-1 recompile) +
//! the atomic-swap-under-execution + DashMap-concurrent = when tier-1 lands. `mode` is
//! carried in the key but always 0 for aarch64 (matters for x86 CS.D etc).

#![cfg(target_arch = "aarch64")]

use crate::tier0::{Tier0, CompiledBlock, StateLayout, AARCH64_LAYOUT};
use crate::{Builder, IlType};
use std::collections::HashMap;

/// The per-guest-ISA hook: given (pc, mode), read guest-mem and compile ONE block into
/// the given `Tier0`. Returns (n_insns_compiled, block_end_pc). The generated
/// `recompiler.rs` supplies this via `recompile_one` in a loop; this trait keeps
/// BlockCache arch-neutral.
pub trait BlockCompiler {
    /// Read one insn word at `pc` from guest memory (for the driver's stop-check).
    fn fetch(&self, pc: u64) -> u32;
    /// Compile insns starting at `pc` into `t0` until a block-ending condition
    /// (branch emitted / stop-insn / max-cap). Return (n_insns, why_stopped).
    fn compile_block(&self, t0: &mut Tier0, pc: u64, mode: u32) -> (usize, StopReason);
    /// Is this insn a driver-level stop? (BRK/HLT/etc — the "return to host" signal.)
    fn is_stop(&self, insn: u32) -> bool;
}

#[derive(Debug, Clone, Copy)]
pub enum StopReason {
    Branched,       // block emitted a branch (pc set by the block itself)
    StopInsn,       // hit is_stop() — block emitted branch-to-that-pc, driver returns
    MaxInsns,       // block-size cap — block emitted fallthrough-branch
}

struct Entry {
    block: CompiledBlock,
    guest_range: (u64, u64),  // [start, end) in guest bytes — for invalidate() intersection
    exec_count: u32,          // hotspot promotion counter (v2)
    tier: u8,                 // 0 for now
}

pub struct BlockCache {
    map: HashMap<(u64, u32), Entry>,
    pub max_block_insns: usize,
    pub n_compiles: usize,
    pub n_execs: usize,
    layout: &'static StateLayout,
}

impl BlockCache {
    pub fn new() -> Self { Self::with_layout(&AARCH64_LAYOUT) }
    pub fn with_layout(layout: &'static StateLayout) -> Self {
        Self { map: HashMap::new(), max_block_insns: 32, n_compiles: 0, n_execs: 0, layout }
    }

    /// Run guest code from `state[OFF_PC]` until a stop-insn or `max_execs` blocks.
    /// Returns the reason.
    pub fn run<C: BlockCompiler>(&mut self, compiler: &C, state: &mut [u64],
                                 mode: u32, max_execs: usize) -> RunResult
    {
        debug_assert_eq!(state.len(), self.layout.state_words);
        let pc_idx = (self.layout.off_pc / 8) as usize;
        for _ in 0..max_execs {
            let pc = state[pc_idx];
            // Stop-check BEFORE compile (a block that ended AT a BRK branch()es to it,
            // so the next iteration's fetch sees it here).
            if compiler.is_stop(compiler.fetch(pc)) {
                return RunResult::Stop { pc };
            }
            let entry = match self.map.get(&(pc, mode)) {
                Some(_) => self.map.get_mut(&(pc, mode)).unwrap(),
                None => {
                    let mut t0 = Tier0::with_layout(self.layout);
                    let (n, _stop) = compiler.compile_block(&mut t0, pc, mode);
                    // Ensure the block terminates: if compile_block didn't emit a branch
                    // (e.g. hit max-cap on a non-branch), append a fallthrough-branch.
                    // ‡ x64: `n * 4` is aarch64-specific (fixed 4-byte insns). For x64 the
                    //   BlockCompiler must ALWAYS emit a branch (compile_block returns the
                    //   fallthrough-pc via a branch it emits itself), so this arm is aarch64-only.
                    //   Assert instead of miscomputing.
                    if !t0.branched() {
                        debug_assert_eq!(self.layout.flag_file, 2,
                            "non-aarch64 BlockCompiler must emit branch (variable-length)");
                        let next = pc + (n as u64 * 4);
                        let t = t0.literal(IlType::U64, next as u128);
                        t0.branch(t, false);
                    }
                    let block = t0.finalize();
                    self.n_compiles += 1;
                    self.map.insert((pc, mode), Entry {
                        block,
                        guest_range: (pc, pc + n as u64 * 4),
                        exec_count: 0,
                        tier: 0,
                    });
                    self.map.get_mut(&(pc, mode)).unwrap()
                }
            };
            entry.block.exec_slice(state);
            entry.exec_count = entry.exec_count.saturating_add(1);
            self.n_execs += 1;
        }
        RunResult::MaxExecs
    }

    /// Drop every cached block whose guest_range intersects `[start, end)`.
    /// The loader↔JIT contract seam (per DESIGN.md §invalidate): callers are
    /// loader-side (bulk-patches ride free pre-first-compile; runtime-patches call
    /// this explicitly; guest-SMC = write-protect+fault → this). No callers yet;
    /// the contract lands here so the loader integration has the seam to build against.
    pub fn invalidate(&mut self, start: u64, end: u64) -> usize {
        let before = self.map.len();
        self.map.retain(|_, e| !(e.guest_range.0 < end && start < e.guest_range.1));
        before - self.map.len()
    }

    pub fn len(&self) -> usize { self.map.len() }
    pub fn clear(&mut self) { self.map.clear(); self.n_compiles = 0; self.n_execs = 0; }
}

#[derive(Debug)]
pub enum RunResult {
    Stop { pc: u64 },     // hit is_stop() at this pc
    MaxExecs,             // ran max_execs blocks without stopping
}

#[cfg(test)]
mod tests {
    use super::*;
    // A stub compiler that counts compiles + always emits a branch-to-pc+4.
    struct StubCompiler;
    impl BlockCompiler for StubCompiler {
        fn fetch(&self, pc: u64) -> u32 { if pc >= 0x1010 { 0xD4200000 } else { 0xD503201F } }
        fn is_stop(&self, insn: u32) -> bool { insn == 0xD4200000 }
        fn compile_block(&self, t0: &mut Tier0, pc: u64, _: u32) -> (usize, StopReason) {
            let t = t0.literal(IlType::U64, (pc + 4) as u128);
            t0.branch(t, false);
            (1, StopReason::Branched)
        }
    }
    #[test]
    fn invalidate_intersecting() {
        let mut c = BlockCache::new();
        let mut st = [0u64; crate::tier0::STATE_WORDS];
        st[33] = 0x1000;
        c.run(&StubCompiler, &mut st[..], 0, 100);
        assert_eq!(c.n_compiles, 4);  // 4 blocks: 0x1000, 0x1004, 0x1008, 0x100C
        assert_eq!(c.len(), 4);
        // Invalidate [0x1004, 0x100C) → drops 2 blocks (@1004, @1008).
        let dropped = c.invalidate(0x1004, 0x100C);
        assert_eq!(dropped, 2);
        assert_eq!(c.len(), 2);
        // Re-run: recompiles the 2 dropped.
        st[33] = 0x1000;
        let before = c.n_compiles;
        c.run(&StubCompiler, &mut st[..], 0, 100);
        assert_eq!(c.n_compiles - before, 2);
    }
}
