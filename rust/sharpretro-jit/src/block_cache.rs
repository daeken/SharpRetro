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
    /// Compile insns starting at `pc` into `b` until a block-ending condition
    /// (branch emitted / stop-insn / max-cap). Return (end_pc, why_stopped) —
    /// end_pc = first byte AFTER the block (guest_range for invalidate; was
    /// n_insns ×4 which mis-sized every x64 block's range — variable-length).
    /// Generic over Builder (Val=u32) so the SAME impl serves tier-0 and
    /// tier-1 (lift_one loops are Builder-generic already). Not object-safe;
    /// nothing uses dyn BlockCompiler (checked).
    fn compile_block<B: Builder<Val = u32>>(&self, b: &mut B, pc: u64, mode: u32) -> (u64, StopReason);
    /// Guest byte-range READ BEYOND the block by the most recent compile_block
    /// (exit-liveness successor-peek). (0,0) = none. invalidate() drops blocks
    /// whose peek-window intersects — their stripped flag-computation depended
    /// on those bytes.
    fn last_peek_range(&self) -> (u64, u64) { (0, 0) }
    /// Is this insn a driver-level stop? (BRK/HLT/etc — the "return to host" signal.)
    fn is_stop(&self, insn: u32) -> bool;
    /// Called at each driver-loop iteration BEFORE is_stop / compile-or-lookup.
    /// If `pc` is a native-call target (in the enumerated `native_call_targets` set
    /// under shared-mode / mem_base=0), the impl: calls the native fn (win64→AAPCS
    /// ABI-map: args from state[gpr[rcx/rdx/r8/r9]], return → state[gpr[rax]]), pops
    /// the return-addr from guest stack into state[off_pc], and returns true. The
    /// driver loop then `continue`s (next iter reads the popped return-pc). Default
    /// impl = false (no native crossings; the aarch64 harness case).
    ///
    /// This is the DRIVER-LOOP discrimination point (per DESIGN.md §call_native /
    /// the shared-mode design): every indirect `call [target]` compiles to `push
    /// next_pc; branch(target)` → block ends → driver's next iter sees pc=target
    /// → THIS check fires. Zero tier-0 emit changes; the discrimination is here.
    fn dispatch_native(&self, _pc: u64, _state: &mut [u64]) -> bool { false }
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
    /// Peek-window: guest range read by exit-liveness successor-peek at
    /// compile time (empty if none). invalidate() must ALSO drop blocks whose
    /// peek-window intersects the invalidated range — the stripped flag-
    /// computation depended on those bytes.
    peek_range: (u64, u64),
    /// Predecessor link-slots pointing INTO this block: (pred_key, slot_off,
    /// pred_epilogue_addr). invalidate() resets each to the pred's epilogue
    /// (else: use-after-munmap through a stale chain).
    linked_from: Vec<((u64, u32), usize, u64)>,
    exec_count: u32,          // hotspot promotion counter (v2)
    tier: u8,                 // 0 = tier-0; 1 = tier-1
}

pub struct BlockCache {
    map: HashMap<(u64, u32), Entry>,
    pub max_block_insns: usize,
    pub n_compiles: usize,
    pub n_execs: usize,
    layout: &'static StateLayout,
    /// Shared spill area, sized to max(n_slots) across compiled blocks (grows
    /// on-demand at COMPILE time, not exec time). exec_slice's per-call
    /// vec![0u64; n_slots+1] alloc was 40% of wall on a tight-loop bench
    /// (5M heap allocs). The spill area doesn't need zeroing (each block's
    /// prologue writes before reading — SSA def-before-use guarantees it).
    spill: Vec<u64>,
    /// Block-linking on? (XF_LINK=0 disables; also disables tier-1 emit-side
    /// thunks via the same env in tier1.rs — one switch.)
    link: bool,
    /// Compile tier-1 with tier-0 fallback? (XF_T1=0 → tier-0 only.)
    use_t1: bool,
    /// pcs awaited by already-compiled predecessors: target_key →
    /// [(pred_key, slot_off, pred_epilogue)].
    pending_links: std::collections::HashMap<(u64, u32), Vec<((u64, u32), usize, u64)>>,
    /// IC reverse-index: target_key → blocks whose inline caches may hold the
    /// target's body address. invalidate(target) resets those caches.
    ic_from: std::collections::HashMap<(u64, u32), Vec<(u64, u32)>>,
    /// IC exiter cell: under block-linking, the block whose INDIRECT exit
    /// reached the driver is the chain TAIL, not the dispatched head — each
    /// ic-carrying block's epilogue stores its own guest-pc here (address
    /// baked at compile; Box = stable address). Driver seeds it with the
    /// dispatched pc pre-call and installs into map[*cell] post-call.
    last_exiter: Box<u64>,
}

impl BlockCache {
    pub fn new() -> Self { Self::with_layout(&AARCH64_LAYOUT) }
    pub fn with_layout(layout: &'static StateLayout) -> Self {
        // XF_WATCH v1 is TIER-0 ONLY (the recorder doesn't carry per-op guest
        // pc; wiring it = per-op debug-context in IlOp — v2). Force tier-0 +
        // warn so a watch is never silently blind through tier-1 blocks.
        if (std::env::var("XF_WATCH").is_ok() || std::env::var("XF_RR").is_ok())
            && std::env::var("XF_T1").map(|v| v != "0").unwrap_or(true) {
            eprintln!("[cache] XF_WATCH/XF_RR armed → forcing tier-0 (t0-only v1; set XF_T1=0 to silence)");
        }
        Self { map: HashMap::new(), max_block_insns: 32, n_compiles: 0, n_execs: 0, layout,
               spill: vec![0u64; 64],
               link: std::env::var("XF_LINK").map(|v| v != "0").unwrap_or(true),
               use_t1: std::env::var("XF_T1").map(|v| v != "0").unwrap_or(true)
                       && !std::env::var("XF_WATCH").is_ok()
                       && !std::env::var("XF_RR").is_ok(),
               pending_links: Default::default(), ic_from: Default::default(),
               last_exiter: Box::new(0) }
    }

    /// Compile one block: tier-1 first (catch_unwind — wide-ty/loop_n/
    /// intrinsic blocks panic at RECORD or EMIT time, compile-time only),
    /// tier-0 on fallback. Returns (block, end_pc, tier).
    fn compile_one<C: BlockCompiler>(&self, compiler: &C, pc: u64, mode: u32)
        -> (CompiledBlock, u64, u8)
    {
        if self.use_t1 {
            let layout = self.layout;
            let cell_addr = &*self.last_exiter as *const u64 as u64;
            // Suppress the default panic-print for this EXPECTED bail class
            // (wide-ty/loop_n/intrinsic blocks panic → tier-0 serves them; a
            // 100K-block boot with ~1K bails must not spam stderr). The hook
            // is process-global: swap, run, restore. Drivers are single-
            // threaded today (cp2077 worker threads each own a BlockCache but
            // compile rarely; a race here just mis-suppresses one message).
            let prev_hook = std::panic::take_hook();
            std::panic::set_hook(Box::new(|_| {}));
            let r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                let mut t1 = crate::tier1::Tier1::with_layout(layout);
                t1.exit_ident = Some((cell_addr, pc));
                let (end_pc, _stop) = compiler.compile_block(&mut t1, pc, mode);
                if !t1.rec.branched() {
                    debug_assert_eq!(layout.flag_file, 2,
                        "non-aarch64 BlockCompiler must emit branch (variable-length)");
                    let t = <crate::tier1::Tier1 as Builder>::literal(&mut t1, IlType::U64, end_pc as u128);
                    <crate::tier1::Tier1 as Builder>::branch(&mut t1, t, false);
                }
                (t1.compile(), end_pc)
            }));
            std::panic::set_hook(prev_hook);
            if let Ok((block, end_pc)) = r { return (block, end_pc, 1); }
            if std::env::var("XF_BAIL_LOG").is_ok() {
                let msg = match &r { Err(e) => e.downcast_ref::<String>().cloned()
                    .or_else(|| e.downcast_ref::<&str>().map(|s| s.to_string()))
                    .unwrap_or_default(), _ => unreachable!() };
                eprintln!("[t1-bail] pc={pc:#x}: {msg}");
            }
            // fall through to tier-0 (bail-class: wide/intrinsic/loop_n).
        }
        let mut t0 = Tier0::with_layout(self.layout);
        let (end_pc, _stop) = compiler.compile_block(&mut t0, pc, mode);
        if !t0.branched() {
            debug_assert_eq!(self.layout.flag_file, 2,
                "non-aarch64 BlockCompiler must emit branch (variable-length)");
            let t = t0.literal(IlType::U64, end_pc as u128);
            t0.branch(t, false);
        }
        (t0.finalize(), end_pc, 0)
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
            // Native-call discrimination: if pc is a native-call target (shared-mode:
            // in the enumerated seam-vtable/IAT-shim set), the compiler calls it +
            // pops return-addr into state[pc_idx]; loop continues at the return-pc.
            if compiler.dispatch_native(pc, state) {
                self.n_execs += 1;
                continue;
            }
            // Stop-check BEFORE compile (a block that ended AT a BRK branch()es to it,
            // so the next iteration's fetch sees it here).
            if compiler.is_stop(compiler.fetch(pc)) {
                return RunResult::Stop { pc };
            }
            if !self.map.contains_key(&(pc, mode)) {
                let (block, end_pc, tier) = self.compile_one(compiler, pc, mode);
                let peek = compiler.last_peek_range();
                // Grow the shared spill area if this block needs more.
                if block.n_slots as usize + 1 > self.spill.len() {
                    self.spill.resize(block.n_slots as usize + 1, 0);
                }
                self.n_compiles += 1;
                if std::env::var("XF_DBG").is_ok() { eprintln!("[cache] compile pc={:#x} tier={} sites={:?} ic={}", pc, tier, block.link_sites, block.ic_sites.len()); }
                // ── BLOCK-LINKING cross-patch ─────────────────────────────
                // (a) NEW block's link-sites aimed at already-compiled pcs
                //     (incl. itself) → patch to their bodies now.
                // (b) Register the rest in pending_links so future compiles
                //     patch them lazily.
                // (c) Serve pending_links aimed at THIS pc: patch each
                //     registered predecessor slot to our body.
                // Reverse-index (linked_from) records every live edge for
                // invalidate()'s unlink pass.
                if self.link {
                    let new_key = (pc, mode);
                    let mut fwd: Vec<((u64, u32), usize, u64)> = vec![];   // edges FROM new
                    // (link_sites only exist on tier-1 blocks; body_off!=0 for them.)
                    for &(off, tgt) in &block.link_sites {
                        if tgt == pc {
                            block.patch_link(off, block.body_addr());
                            fwd.push((new_key, off, block.epilogue_addr));
                        } else if let Some(e) = self.map.get(&(tgt, mode)) {
                            // CHAINABILITY GUARD: only tier-1 blocks (body_off
                            // != 0) can be chain TARGETS — a tier-0 block has
                            // no uniform frame; jumping into it with a tier-1
                            // frame live corrupts the stack (found the hard
                            // way: branchbench's imul block bailed to tier-0,
                            // got linked into, core-dumped @garbage pc).
                            if e.block.body_off != 0 {
                                block.patch_link(off, e.block.body_addr());
                                self.map.get_mut(&(tgt, mode)).unwrap().linked_from
                                    .push((new_key, off, block.epilogue_addr));
                            }
                        } else {
                            self.pending_links.entry((tgt, mode)).or_default()
                                .push((new_key, off, block.epilogue_addr));
                        }
                    }
                    let mut entry = Entry {
                        block, guest_range: (pc, end_pc), peek_range: peek,
                        linked_from: fwd, exec_count: 0, tier,
                    };
                    // (c) predecessors waiting on this pc.
                    if entry.block.body_off != 0 {
                        if let Some(waiters) = self.pending_links.remove(&new_key) {
                            for (pred_key, off, pred_epi) in waiters {
                                if let Some(pe) = self.map.get(&pred_key) {
                                    pe.block.patch_link(off, entry.block.body_addr());
                                    entry.linked_from.push((pred_key, off, pred_epi));
                                }
                            }
                        }
                    }
                    // (tier-0 new block: waiters stay pending forever — their
                    // slots still point at their own epilogues = correct
                    // unlinked behavior; a future invalidate+recompile at this
                    // pc may serve them as tier-1.)
                    self.map.insert(new_key, entry);
                } else {
                    self.map.insert((pc, mode), Entry {
                        block, guest_range: (pc, end_pc), peek_range: peek,
                        linked_from: vec![], exec_count: 0, tier,
                    });
                }
            }
            let entry = self.map.get_mut(&(pc, mode)).unwrap();
            // Direct entry-call with the shared spill area (was exec_slice →
            // vec![0u64; n_slots+1] per call = 40% of wall on tight-loop bench).
            let ef = entry.block.entry_fn();
            let sp = state.as_mut_ptr(); let spp = self.spill.as_mut_ptr();
            *self.last_exiter = pc;   // seed: ic-less chains attribute to head
            ef(sp, spp);
            entry.exec_count = entry.exec_count.saturating_add(1);
            // SAME-PC FAST-LOOP: if the block back-branched to its own entry
            // (a loop-body block), tight-loop calling its entry_fn directly —
            // skip dispatch_native + is_stop + HashMap for the self-loop hot
            // path. Safe: a compiled-block pc is never a native-target nor a
            // stop-insn (both would've been caught on the FIRST iteration).
            // ‡ v1: self-loop only. General block-linking (A→B chain) needs
            //   a 2-entry cache or in-code patching.
            let mut hot = 0u64;
            while unsafe { *sp.add(pc_idx) } == pc {
                ef(sp, spp);
                hot += 1;
            }
            entry.exec_count = entry.exec_count.saturating_add(hot as u32);
            self.n_execs += hot as usize;
            self.n_execs += 1;
            // ── INLINE-CACHE PATCH ────────────────────────────────────────
            // The block at `pc` exited toward state[pc_idx] through the
            // epilogue (indirect-miss or unlinked const). If it HAS IC sites
            // and the destination is a compiled CHAINABLE block, install
            // (guest→body) into its caches: next time that indirect exit
            // resolves the same target, it br's straight to the body — no
            // driver round-trip. guest→body is universal truth, so blindly
            // patching every site of the exiting block is sound (we don't
            // know which site missed; a site that never sees this target
            // just keeps a never-matching pair). Only tier-1 dest (body_off
            // != 0) — same chainability rule as linking. The reverse-index
            // (ic_from) lets invalidate() scrub stale bodies.
            let next = unsafe { *sp.add(pc_idx) };
            // CHAIN-TAIL ATTRIBUTION (the never-hits root): under block-
            // linking the dispatched block br's through linked successors,
            // so the block whose indirect exit reached the driver is the
            // chain TAIL — installing into the dispatched head's ic-sites
            // never arms the cache that missed. Each ic-carrying block's
            // epilogue stores its own guest-pc into last_exiter (seeded
            // with the dispatched pc pre-call, so ic-less exits attribute
            // to the head as before — head-without-ic ⇒ no install, same
            // net behavior, correct attribution when it matters).
            let exiter = *self.last_exiter;
            let has_ic = self.map.get(&(exiter, mode))
                .map_or(false, |xe| !xe.block.ic_sites.is_empty());
            if has_ic && std::env::var("XF_DBG").is_ok() {
                eprintln!("[ic] exiter={exiter:#x} (dispatched {pc:#x}) → next={next:#x} (installing={})",
                    self.map.get(&(next, mode)).map_or(false, |te| te.block.body_off != 0));
            }
            if has_ic {
                if let Some(te) = self.map.get(&(next, mode)) {
                    if te.block.body_off != 0 {
                        let body = te.block.body_addr();
                        self.map[&(exiter, mode)].block.ic_install(next, body);
                        self.ic_from.entry((next, mode)).or_default().push((exiter, mode));
                    }
                }
            }
        }
        RunResult::MaxExecs
    }

    // ── Cross-run persistence (JIT-perf item ④) ────────────────────────────
    //
    // Format v1 (little-endian, no compression):
    //   magic "XFC1" | u32 n_records
    //   per record: u64 pc | u64 end_pc | u64 peek_lo | u64 peek_hi |
    //               u32 mode | u32 n_slots | u32 body_off | u32 epi_off |
    //               u32 n_sites | n_sites × (u32 slot_off, u64 guest_tgt) |
    //               u32 code_len | code bytes
    // Only TIER-1 blocks persist (tier-0 = the rare bail class, recompiles in
    // µs; and tier-0 blocks are frameless/unchainable anyway). Code is
    // position-independent except link slots, which from_code_bytes re-
    // defaults to the fresh epilogue. CALLER owns the key discipline: the
    // file must be keyed by (guest-image identity, compiler version, codegen
    // env) — a stale cache against changed guest bytes is silently wrong.
    // What's in the map at save time is exactly the never-invalidated set
    // (invalidate removes entries), so save() can't persist stale ranges.

    fn fnv1a(data: &[u8]) -> u64 {
        let mut h = 0xcbf29ce484222325u64;
        for &b in data { h ^= b as u64; h = h.wrapping_mul(0x100000001b3); }
        h
    }

    /// Serialize every tier-1 block. Returns bytes (caller writes the file).
    /// Trailer = fnv1a of everything after the magic (bit-rot → loud load-fail
    /// instead of executing corrupted machine code).
    pub fn save(&self) -> Vec<u8> {
        let mut out = Vec::with_capacity(1 << 20);
        out.extend_from_slice(b"XFC1");
        // ic-carrying blocks bake THIS run's last_exiter cell address into
        // their epilogue → not position-independent → excluded from persist
        // (they recompile next run; ICs are hot-path caches, cheap to re-earn).
        let recs: Vec<_> = self.map.iter()
            .filter(|(_, e)| e.tier == 1 && e.block.ic_sites.is_empty()).collect();
        out.extend_from_slice(&(recs.len() as u32).to_le_bytes());
        for (&(pc, mode), e) in recs {
            let b = &e.block;
            out.extend_from_slice(&pc.to_le_bytes());
            out.extend_from_slice(&e.guest_range.1.to_le_bytes());
            out.extend_from_slice(&e.peek_range.0.to_le_bytes());
            out.extend_from_slice(&e.peek_range.1.to_le_bytes());
            out.extend_from_slice(&mode.to_le_bytes());
            out.extend_from_slice(&b.n_slots.to_le_bytes());
            out.extend_from_slice(&(b.body_off as u32).to_le_bytes());
            out.extend_from_slice(&((b.epilogue_addr - b.page_addr()) as u32).to_le_bytes());
            out.extend_from_slice(&(b.link_sites.len() as u32).to_le_bytes());
            for &(off, tgt) in &b.link_sites {
                out.extend_from_slice(&(off as u32).to_le_bytes());
                out.extend_from_slice(&tgt.to_le_bytes());
            }
            let code = b.code_bytes();
            out.extend_from_slice(&(code.len() as u32).to_le_bytes());
            out.extend_from_slice(code);
        }
        let h = Self::fnv1a(&out[4..]);
        out.extend_from_slice(&h.to_le_bytes());
        out
    }

    /// Load a save() image: reconstruct blocks, insert, then eagerly cross-
    /// link (all members present → one pass; edges to absent pcs go to
    /// pending_links as usual, same chainability rules). Returns blocks
    /// loaded. (pc,mode) collisions: first wins. Corrupt input → Err (caller
    /// falls back to cold compile).
    pub fn load(&mut self, data: &[u8]) -> Result<usize, &'static str> {
        fn rd_u32(d: &[u8], o: &mut usize) -> Option<u32> {
            let v = d.get(*o..*o+4)?; *o += 4; Some(u32::from_le_bytes(v.try_into().unwrap())) }
        fn rd_u64(d: &[u8], o: &mut usize) -> Option<u64> {
            let v = d.get(*o..*o+8)?; *o += 8; Some(u64::from_le_bytes(v.try_into().unwrap())) }
        if data.len() < 16 || &data[..4] != b"XFC1" { return Err("bad magic"); }
        let (body, trailer) = data.split_at(data.len() - 8);
        let want = u64::from_le_bytes(trailer.try_into().unwrap());
        if Self::fnv1a(&body[4..]) != want { return Err("checksum mismatch"); }
        let data = body;
        let mut o = 4usize;
        let n = rd_u32(data, &mut o).ok_or("truncated")? as usize;
        let mut loaded = 0usize;
        for _ in 0..n {
            let pc = rd_u64(data, &mut o).ok_or("truncated")?;
            let end_pc = rd_u64(data, &mut o).ok_or("truncated")?;
            let peek = (rd_u64(data, &mut o).ok_or("truncated")?,
                        rd_u64(data, &mut o).ok_or("truncated")?);
            let mode = rd_u32(data, &mut o).ok_or("truncated")?;
            let n_slots = rd_u32(data, &mut o).ok_or("truncated")?;
            let body_off = rd_u32(data, &mut o).ok_or("truncated")? as usize;
            let epi_off = rd_u32(data, &mut o).ok_or("truncated")? as usize;
            let n_sites = rd_u32(data, &mut o).ok_or("truncated")? as usize;
            let mut sites = Vec::with_capacity(n_sites);
            for _ in 0..n_sites {
                let off = rd_u32(data, &mut o).ok_or("truncated")? as usize;
                let tgt = rd_u64(data, &mut o).ok_or("truncated")?;
                sites.push((off, tgt));
            }
            let code_len = rd_u32(data, &mut o).ok_or("truncated")? as usize;
            let code = data.get(o..o + code_len).ok_or("truncated code")?; o += code_len;
            if self.map.contains_key(&(pc, mode)) { continue; }
            let block = CompiledBlock::from_code_bytes(code, n_slots, body_off, epi_off, sites);
            if block.n_slots as usize + 1 > self.spill.len() {
                self.spill.resize(block.n_slots as usize + 1, 0);
            }
            self.map.insert((pc, mode), Entry {
                block, guest_range: (pc, end_pc), peek_range: peek,
                linked_from: vec![], exec_count: 0, tier: 1,
            });
            loaded += 1;
        }
        // Eager cross-link pass (same chainability rules as compile path).
        if self.link {
            let keys: Vec<(u64, u32)> = self.map.keys().copied().collect();
            for k in keys {
                let (sites, epi) = {
                    let e = &self.map[&k];
                    (e.block.link_sites.clone(), e.block.epilogue_addr)
                };
                for (off, tgt) in sites {
                    let tkey = (tgt, k.1);
                    if let Some(te) = self.map.get(&tkey) {
                        if te.block.body_off != 0 {
                            let addr = te.block.body_addr();
                            self.map[&k].block.patch_link(off, addr);
                            self.map.get_mut(&tkey).unwrap().linked_from.push((k, off, epi));
                        }
                    } else {
                        self.pending_links.entry(tkey).or_default().push((k, off, epi));
                    }
                }
            }
        }
        Ok(loaded)
    }

    /// Drop every cached block whose guest_range intersects `[start, end)`.
    /// The loader↔JIT contract seam (per DESIGN.md §invalidate): callers are
    /// loader-side (bulk-patches ride free pre-first-compile; runtime-patches call
    /// this explicitly; guest-SMC = write-protect+fault → this). No callers yet;
    /// the contract lands here so the loader integration has the seam to build against.
    pub fn invalidate(&mut self, start: u64, end: u64) -> usize {
        let before = self.map.len();
        // Drop blocks whose CODE range or exit-liveness PEEK-window intersects
        // (peek: the stripped flag computation depended on successor bytes —
        // if those changed, the strip may now be unsound → recompile).
        let doomed: Vec<(u64, u32)> = self.map.iter()
            .filter(|(_, e)| (e.guest_range.0 < end && start < e.guest_range.1)
                          || (e.peek_range.0 < end && start < e.peek_range.1))
            .map(|(k, _)| *k).collect();
        for k in &doomed {
            let e = self.map.remove(k).unwrap();
            // UNLINK: every predecessor slot pointing into this block's page
            // gets reset to that predecessor's OWN epilogue (else the chain
            // jumps into a munmap'd page). Predecessor may itself be doomed —
            // then it's already/soon removed and its slots die with it.
            for (pred_key, slot_off, pred_epi) in e.linked_from {
                if let Some(pe) = self.map.get(&pred_key) {
                    pe.block.patch_link(slot_off, pred_epi);
                }
            }
            // Drop any pending-link registrations FROM this block (its slots
            // are about to be freed; a later compile must not patch them).
            for v in self.pending_links.values_mut() {
                v.retain(|(pk, _, _)| *pk != *k);
            }
            // IC SCRUB: any surviving block whose inline caches may hold THIS
            // block's body address gets its caches reset (else a stale IC
            // hit br's into a munmap'd page). Coarse (resets all pairs of
            // those blocks, not just the matching entry) — invalidate is
            // rare; correctness over precision.
            if let Some(holders) = self.ic_from.remove(k) {
                for hk in holders {
                    if let Some(he) = self.map.get(&hk) { he.block.ic_reset(); }
                }
            }
            // And forget waiters ON this pc? No — waiters are OTHER blocks'
            // slots aimed at this guest pc; they stay pending and re-serve
            // when the pc recompiles. (Their slots still point at their own
            // epilogues if never patched, or... if they WERE patched to us,
            // they're in linked_from and were just reset above.)
        }
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
        fn compile_block<B: Builder<Val = u32>>(&self, b: &mut B, pc: u64, _: u32) -> (u64, StopReason) {
            let t = b.literal(IlType::U64, (pc + 4) as u128);
            b.branch(t, false);
            (pc + 4, StopReason::Branched)
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

    // ── INLINE-CACHE discriminator (the never-hits mystery's instrument) ──
    // Two blocks in an INDIRECT loop: A @0x1000 reads state[40] (the "return
    // address register") and branches to it INDIRECTLY (no Literal trace →
    // const_tgt=None → IC arm emits when XF_IC=1). B @0x2000 decrements a
    // counter state[41] and branches CONST back to 0x1000. state[40]=0x2000
    // always — so A's indirect target is the SAME every iteration = the IC's
    // designed case. With IC working: after B compiles, A's exit installs
    // (0x2000, B.body) and every later A-exit br's straight to B's body —
    // driver dispatches collapse to ~the compile handful. IC broken: every
    // A→B transition round-trips the driver (n_execs ≈ 2×iterations).
    struct IndirectLoop;
    impl BlockCompiler for IndirectLoop {
        fn fetch(&self, pc: u64) -> u32 { if pc == 0x3000 { 0xD4200000 } else { 0 } }
        fn is_stop(&self, insn: u32) -> bool { insn == 0xD4200000 }
        fn compile_block<B: Builder<Val = u32>>(&self, b: &mut B, pc: u64, _: u32) -> (u64, StopReason) {
            if pc == 0x1000 {
                // A: indirect branch to state[40] (idx 40, file 0 = X-regs in
                // the aarch64 layout — any u64 slot works for the test).
                let t = b.reg_read(crate::RegFile(0), 40, IlType::U64);
                b.branch(t, false);
            } else {
                // B @0x2000: ctr -= 1; branch to (ctr==0 ? 0x3000 : 0x1000) —
                // CONST-traced? No: ternary isn't Literal-traced, so this is
                // ALSO indirect. Fine — the discriminator only needs A's.
                let c = b.reg_read(crate::RegFile(0), 41, IlType::U64);
                let one = b.literal(IlType::U64, 1);
                let c2 = b.sub(c, one);
                b.reg_write(crate::RegFile(0), 41, c2);
                let done = b.literal(IlType::U64, 0x3000);
                let more = b.literal(IlType::U64, 0x1000);
                let t = b.ternary(c2, more, done);   // c2!=0 → loop
                b.branch(t, false);
            }
            (pc + 4, StopReason::Branched)
        }
    }

    #[test]
    fn ic_collapses_indirect_dispatch() {
        if !cfg!(target_arch = "aarch64") { return; }
        unsafe { std::env::set_var("XF_IC", "1"); }
        let mut c = BlockCache::new();
        c.use_t1 = true;
        let mut st = [0u64; crate::tier0::STATE_WORDS];
        st[33] = 0x1000;        // pc
        st[40] = 0x2000;        // A's indirect target — constant
        st[41] = 1000;          // loop count
        let r = c.run(&IndirectLoop, &mut st[..], 0, 5000);
        assert!(matches!(r, RunResult::Stop { pc: 0x3000 }), "r={r:?} pc={:#x}", st[33]);
        assert_eq!(st[41], 0);
        eprintln!("[ic-test] n_execs={} n_compiles={}", c.n_execs, c.n_compiles);
        // 1000 iterations × 2 blocks. Without IC every transition is a
        // driver dispatch: n_execs ≈ 2000. With A's IC hitting (A→B goes
        // br-direct) the A→B leg vanishes from the driver: n_execs ≈ 1000.
        // (B→A stays driver-side: B's ternary target is also indirect and
        // ALSO gets an IC — if both hit, execs collapse toward ~single-digit
        // because the whole loop lives in JIT'd code.)
        assert!(c.n_execs < 1500,
            "IC never hit: n_execs={} (≈2/iter = every transition driver-dispatched)", c.n_execs);
        unsafe { std::env::remove_var("XF_IC"); }
    }
}
