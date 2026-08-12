//! Record/replay core — the rr-substitute (DESIGN.md §record-replay).
//!
//! The JIT owns every instruction and — post the atomics work — every
//! cross-thread ordering point routes through Builder nodes
//! (`mem_rmw_atomic` / `mem_cas_atomic` / `fence`). We don't infer
//! nondeterminism sources; we EMIT all of them. So:
//!
//! RECORD: at each atomic op, the emitted code takes a global sequence
//! number (one LDADD) and appends `{seq, guest_pc}` to a per-thread buffer.
//! The interleaving of atomics IS the schedule; everything between them is
//! deterministic (up to plain-access races — measured ~78ppm, and a replay
//! divergence on one = a free race DETECTOR, not a silent wrong).
//!
//! REPLAY: each thread's k-th atomic op waits (spin+yield) until the global
//! replay cursor reaches its k-th recorded seq, executes, then releases the
//! cursor to seq+1. Threads stay genuinely parallel between ordering points.
//!
//! Mode is COMPILE-TIME (env XF_RR=record|replay, like XF_WATCH) — the two
//! paths emit different code; the cross-run block cache must not be shared
//! across modes (callers key the cache by env, or skip CACHE under XF_RR).
//! v1 scope: tier-0 only (BlockCache forces tier-0 + warns, same as
//! XF_WATCH); big fixed buffers + LOUD abort when full (the stop-the-world
//! flush + paused-clock design is banked for v2 — v1 buffers hold 16M
//! events/thread = hours of ordinary boot traffic).
//!
//! Shim results + clock reads are DRIVER-side events (the shim dispatcher
//! records/feeds them host-side — see the harness taps); this module owns
//! the guest-code-visible half: the per-thread handle, the seq counter, the
//! replay cursor, and the buffer contract the emitted code writes/reads.

use std::sync::atomic::{AtomicU64, Ordering};

/// Record-mode global atomic-section lock (one word: 0=free, 1=held). The
/// seq-stamp must be ATOMIC WITH the guest op it stamps — otherwise two
/// threads can take seqs in one order and execute their LSE ops in the
/// other, and the log is not a linearization of the run (replay would then
/// force the WRONG order and diverge on data). Record mode therefore wraps
/// {take-seq, do-op} in this spinlock (CASAL acquire / STLR release in the
/// emitted code). Atomics are rare relative to plain code — the
/// serialization is the record-mode tax, and it buys an exact log.
pub static RR_LOCK: AtomicU64 = AtomicU64::new(0);

/// Global sequence counter (record mode). The emitted stamp does
/// `seq = LDADD(RR_SEQ, 1)`.
pub static RR_SEQ: AtomicU64 = AtomicU64::new(0);
/// Global replay cursor (replay mode). The emitted wait spins until
/// `RR_CURSOR == my_recorded_seq`, executes the atomic, then stores seq+1
/// (release).
pub static RR_CURSOR: AtomicU64 = AtomicU64::new(0);

/// Per-thread rr handle. The emitted code addresses fields BY OFFSET —
/// repr(C) is a contract:
///   +0  cur   (record: next write ptr | replay: next read ptr)
///   +8  end   (record: buffer end — full = loud abort | replay: log end)
///   +16 base  (buffer start; host bookkeeping)
///   +24 tid   (host bookkeeping / event attribution)
#[repr(C)]
pub struct RrThread {
    pub cur: u64,
    pub end: u64,
    pub base: u64,
    pub tid: u64,
}

/// One recorded event = 16 bytes: {seq, guest_pc}. guest_pc is debug
/// context (which atomic site) — replay keys on seq order alone.
pub const RR_EVENT_BYTES: u64 = 16;
/// v1 buffer: 16M events × 16B = 256MB per registered thread.
pub const RR_BUF_EVENTS: usize = 16 * 1024 * 1024;

#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub enum RrMode { Off, Record, Replay }

/// Compile-time mode, read once from XF_RR (record|replay).
pub fn rr_mode() -> RrMode {
    match std::env::var("XF_RR").as_deref() {
        Ok("record") => RrMode::Record,
        Ok("replay") => RrMode::Replay,
        Ok(other) if !other.is_empty() => panic!("XF_RR={other:?} (want record|replay)"),
        _ => RrMode::Off,
    }
}

/// Allocate + register a per-thread rr handle (record mode). Returns the
/// Box — caller stores the raw ptr into the thread's state[off_rr] word and
/// keeps the Box alive for the run.
pub fn record_thread(tid: u64) -> Box<RrThread> {
    let buf: Vec<u64> = vec![0u64; RR_BUF_EVENTS * 2]; // 2 words/event
    let base = buf.as_ptr() as u64;
    std::mem::forget(buf); // lives for the run; drained via the handle
    Box::new(RrThread { cur: base, end: base + (RR_BUF_EVENTS as u64) * RR_EVENT_BYTES, base, tid })
}

/// Wrap a recorded per-thread log for replay. `events` = the (seq, pc)
/// pairs recorded for this tid, in order.
pub fn replay_thread(tid: u64, events: &[(u64, u64)]) -> Box<RrThread> {
    let mut buf: Vec<u64> = Vec::with_capacity(events.len() * 2);
    for &(seq, pc) in events { buf.push(seq); buf.push(pc); }
    let base = buf.as_ptr() as u64;
    let end = base + (events.len() as u64) * RR_EVENT_BYTES;
    std::mem::forget(buf);
    Box::new(RrThread { cur: base, end, base, tid })
}

/// Drain a record-mode handle → the (seq, pc) event list.
pub fn drain(h: &RrThread) -> Vec<(u64, u64)> {
    let n = ((h.cur - h.base) / RR_EVENT_BYTES) as usize;
    let p = h.base as *const u64;
    (0..n).map(|i| unsafe { (*p.add(i * 2), *p.add(i * 2 + 1)) }).collect()
}

/// Save a full recording (per-thread logs) to a file. Format XFRR1:
/// magic, n_threads, then per thread {tid, n_events, events…}, fnv1a
/// trailer (same checksum discipline as the block cache).
pub fn save_log(path: &str, threads: &[(u64, Vec<(u64, u64)>)]) -> std::io::Result<()> {
    let mut out: Vec<u8> = Vec::new();
    out.extend_from_slice(b"XFRR1\0\0\0");
    out.extend_from_slice(&(threads.len() as u64).to_le_bytes());
    for (tid, evs) in threads {
        out.extend_from_slice(&tid.to_le_bytes());
        out.extend_from_slice(&(evs.len() as u64).to_le_bytes());
        for (seq, pc) in evs {
            out.extend_from_slice(&seq.to_le_bytes());
            out.extend_from_slice(&pc.to_le_bytes());
        }
    }
    let ck = fnv1a(&out);
    out.extend_from_slice(&ck.to_le_bytes());
    std::fs::write(path, out)
}

pub fn load_log(path: &str) -> std::io::Result<Vec<(u64, Vec<(u64, u64)>)>> {
    let raw = std::fs::read(path)?;
    let bad = |m: &str| std::io::Error::new(std::io::ErrorKind::InvalidData, m.to_string());
    if raw.len() < 24 || &raw[..8] != b"XFRR1\0\0\0" { return Err(bad("bad magic")); }
    let body = &raw[..raw.len() - 8];
    let ck = u64::from_le_bytes(raw[raw.len() - 8..].try_into().unwrap());
    if fnv1a(body) != ck { return Err(bad("checksum mismatch")); }
    let rd = |o: usize| u64::from_le_bytes(body[o..o + 8].try_into().unwrap());
    let n_threads = rd(8) as usize;
    let mut o = 16;
    let mut out = Vec::with_capacity(n_threads);
    for _ in 0..n_threads {
        let tid = rd(o); let n = rd(o + 8) as usize; o += 16;
        let mut evs = Vec::with_capacity(n);
        for _ in 0..n { evs.push((rd(o), rd(o + 8))); o += 16; }
        out.push((tid, evs));
    }
    Ok(out)
}

fn fnv1a(b: &[u8]) -> u64 {
    let mut h: u64 = 0xcbf29ce484222325;
    for &x in b { h ^= x as u64; h = h.wrapping_mul(0x100000001b3); }
    h
}

// ── Shim/clock taps (v2 half 1) — HOST-side events. The driver's shim
// dispatcher calls these around every shim/clock crossing. Two classes:
//
// PLAIN shims (file reads, rand, getenv…): thread-local determinism — once
// the atomic interleaving is pinned, each thread makes its shim calls in a
// deterministic per-thread order, so record/replay = per-thread FIFO of
// result blobs. No global ordering needed.
//
// ORDERED shims (the sync primitives whose HOST-side effect orders guest
// threads — WaitForSingleObject/SetEvent/etc implemented as host condvars,
// INVISIBLE to the guest-atomic log): these are ordering points and must
// participate in the same global sequence as the emitted atomics — pass
// ordered=true and the tap takes/gates a seq on RR_SEQ/RR_CURSOR exactly
// like the emitted brackets. (Without this, a host-condvar wake order could
// differ at replay and the guest sees different WaitFor results — the gap
// between "pure-compute rr" and "real-boot rr".)
//
// The driver owns WHICH fns are ordered (its shim table knows); rr just
// provides the two disciplines.
//
// ORDERED-SHIM CONTRACT (the linearization rule, same as the emitted
// brackets): the {host-effect, seq-take} pair must be atomic as a unit —
// the driver holds RR_LOCK (rr_lock_acquire/release below) around
// {run-the-host-shim, ShimLog::record} for ordered shims. Without it, a
// SetEvent's effect and its seq can interleave a racing WaitFor's, and the
// log stops being a linearization (same failure shape the emitted CASAL
// spinlock exists to prevent).

/// One recorded shim event: the result register + any out-param bytes the
/// shim wrote into guest memory (replay re-applies them instead of re-
/// running the host effect).
#[derive(Clone, Debug, PartialEq)]
pub struct ShimEvent {
    pub fn_id: u32,
    pub ordered_seq: Option<u64>,
    pub ret: u64,
    pub out: Vec<(u64, Vec<u8>)>, // (guest_addr, bytes) writes to re-apply
}

/// Per-thread host-side shim log (record: push; replay: pop-front).
#[derive(Default)]
pub struct ShimLog {
    pub events: Vec<ShimEvent>,
    pub cursor: usize,
}

impl ShimLog {
    /// RECORD a shim crossing. Call AFTER the host shim ran, with its result
    /// + the guest-memory writes it performed. ordered=true for sync-shims:
    /// takes a global seq (fetch_add on RR_SEQ — the same counter the
    /// emitted atomics stamp) so replay can gate it.
    pub fn record(&mut self, fn_id: u32, ret: u64, out: Vec<(u64, Vec<u8>)>, ordered: bool) {
        let seq = ordered.then(|| RR_SEQ.fetch_add(1, Ordering::SeqCst));
        self.events.push(ShimEvent { fn_id, ordered_seq: seq, ret, out });
    }

    /// REPLAY a shim crossing: instead of running the host shim, return the
    /// recorded result + re-apply the recorded guest-memory writes. For
    /// ordered events, WAITS until RR_CURSOR reaches the recorded seq (and
    /// advances it after) — the host-side twin of the emitted wait-for-turn.
    /// Panics (loud) on fn_id mismatch = execution diverged from the log.
    pub fn replay(&mut self, fn_id: u32) -> &ShimEvent {
        let ev = &self.events[self.cursor];
        assert_eq!(ev.fn_id, fn_id,
            "rr shim divergence at event {}: recorded fn {} but execution called fn {}",
            self.cursor, ev.fn_id, fn_id);
        if let Some(seq) = ev.ordered_seq {
            while RR_CURSOR.load(Ordering::Acquire) != seq {
                std::thread::yield_now();
            }
        }
        for (addr, bytes) in &ev.out {
            unsafe {
                std::ptr::copy_nonoverlapping(bytes.as_ptr(), *addr as *mut u8, bytes.len());
            }
        }
        if let Some(seq) = ev.ordered_seq {
            RR_CURSOR.store(seq + 1, Ordering::Release);
        }
        self.cursor += 1;
        &self.events[self.cursor - 1]
    }
}

/// Replay-side deadlock detector: a replaying thread that spins too long on
/// the cursor means the execution diverged from the log (a plain-access race
/// changed a branch → a different atomic order). The emitted spin loops
/// forever; the HOST watchdog (driver-side, checks cursor progress) aborts
/// loud. This is the "divergence = detector" contract.
/// Save shim-logs (per-thread) to a file. Format XFRS1: magic, n_threads,
/// per thread {name_len, name_bytes (the SetThreadDescription name — the
/// STABLEST cross-run thread key; empty = unnamed, falls back to the
/// spawn-order composite), n_events, per event {fn_id, has_seq, seq, ret,
/// n_out, per out {addr, len, bytes}}}, fnv1a trailer.
pub fn save_shim_log(path: &str, threads: &[(String, ShimLog)]) -> std::io::Result<()> {
    let mut out: Vec<u8> = Vec::new();
    out.extend_from_slice(b"XFRS1\0\0\0");
    out.extend_from_slice(&(threads.len() as u64).to_le_bytes());
    for (name, log) in threads {
        out.extend_from_slice(&(name.len() as u32).to_le_bytes());
        out.extend_from_slice(name.as_bytes());
        out.extend_from_slice(&(log.events.len() as u64).to_le_bytes());
        for ev in &log.events {
            out.extend_from_slice(&ev.fn_id.to_le_bytes());
            out.push(ev.ordered_seq.is_some() as u8);
            out.extend_from_slice(&ev.ordered_seq.unwrap_or(0).to_le_bytes());
            out.extend_from_slice(&ev.ret.to_le_bytes());
            out.extend_from_slice(&(ev.out.len() as u32).to_le_bytes());
            for (addr, bytes) in &ev.out {
                out.extend_from_slice(&addr.to_le_bytes());
                out.extend_from_slice(&(bytes.len() as u32).to_le_bytes());
                out.extend_from_slice(bytes);
            }
        }
    }
    let ck = fnv1a(&out);
    out.extend_from_slice(&ck.to_le_bytes());
    std::fs::write(path, out)
}

pub fn load_shim_log(path: &str) -> std::io::Result<Vec<(String, ShimLog)>> {
    let raw = std::fs::read(path)?;
    let bad = |m: &str| std::io::Error::new(std::io::ErrorKind::InvalidData, m.to_string());
    if raw.len() < 24 || &raw[..8] != b"XFRS1\0\0\0" { return Err(bad("bad magic")); }
    let body = &raw[..raw.len() - 8];
    let ck = u64::from_le_bytes(raw[raw.len() - 8..].try_into().unwrap());
    if fnv1a(body) != ck { return Err(bad("checksum mismatch")); }
    let mut o = 8usize;
    let rd_u32 = |raw: &[u8], o: &mut usize| -> u32 {
        let v = u32::from_le_bytes(raw[*o..*o + 4].try_into().unwrap()); *o += 4; v };
    let rd_u64 = |raw: &[u8], o: &mut usize| -> u64 {
        let v = u64::from_le_bytes(raw[*o..*o + 8].try_into().unwrap()); *o += 8; v };
    let n_threads = rd_u64(body, &mut o);
    let mut threads = Vec::new();
    for _ in 0..n_threads {
        let nl = rd_u32(body, &mut o) as usize;
        let name = String::from_utf8_lossy(&body[o..o + nl]).into_owned(); o += nl;
        let n_ev = rd_u64(body, &mut o);
        let mut log = ShimLog::default();
        for _ in 0..n_ev {
            let fn_id = rd_u32(body, &mut o);
            let has_seq = body[o] != 0; o += 1;
            let seq = rd_u64(body, &mut o);
            let ret = rd_u64(body, &mut o);
            let n_out = rd_u32(body, &mut o);
            let mut outv = Vec::new();
            for _ in 0..n_out {
                let addr = rd_u64(body, &mut o);
                let len = rd_u32(body, &mut o) as usize;
                outv.push((addr, body[o..o + len].to_vec())); o += len;
            }
            log.events.push(ShimEvent {
                fn_id, ordered_seq: has_seq.then_some(seq), ret, out: outv });
        }
        threads.push((name, log));
    }
    Ok(threads)
}

/// Host-side RR_LOCK acquire/release — the ordered-shim bracket (see the
/// ORDERED-SHIM CONTRACT above). Same lock the emitted record brackets use,
/// so shim effects and guest atomics serialize into ONE linearization.
pub fn rr_lock_acquire() {
    while RR_LOCK.compare_exchange(0, 1, Ordering::AcqRel, Ordering::Acquire).is_err() {
        std::thread::yield_now();
    }
}
pub fn rr_lock_release() { RR_LOCK.store(0, Ordering::Release); }

pub fn cursor() -> u64 { RR_CURSOR.load(Ordering::SeqCst) }
pub fn seq_now() -> u64 { RR_SEQ.load(Ordering::SeqCst) }

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn shimlog_fifo_and_divergence() {
        let mut log = ShimLog::default();
        log.record(7, 42, vec![(0, vec![])], false);
        log.record(9, 43, vec![], false);
        // replay in order works (skip the out-write: addr 0 w/ 0 bytes = no-op)
        assert_eq!(log.replay(7).ret, 42);
        assert_eq!(log.replay(9).ret, 43);
        // divergence = panic (loud)
        let mut log2 = ShimLog::default();
        log2.record(7, 1, vec![], false);
        log2.cursor = 0;
        let r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| { log2.replay(8); }));
        assert!(r.is_err(), "fn_id mismatch must panic");
    }

    #[test]
    fn ordered_shims_replay_in_recorded_order() {
        // record: two "threads" (simulated serially) take ordered seqs A then B
        RR_SEQ.store(100, Ordering::SeqCst);
        let mut t0 = ShimLog::default();
        let mut t1 = ShimLog::default();
        t0.record(1, 10, vec![], true);   // seq 100
        t1.record(2, 20, vec![], true);   // seq 101
        // replay: t1 must WAIT until cursor passes 100 even if it arrives first.
        RR_CURSOR.store(100, Ordering::SeqCst);
        let t1h = std::thread::spawn(move || {
            let ret = t1.replay(2).ret;   // blocks until cursor==101
            (ret, std::time::Instant::now())
        });
        std::thread::sleep(std::time::Duration::from_millis(50));
        let ret0 = t0.replay(1).ret;      // seq 100 → advances cursor to 101
        let at0 = std::time::Instant::now();
        let (ret1, at1) = t1h.join().unwrap();
        assert_eq!((ret0, ret1), (10, 20));
        assert!(at1 >= at0, "t1's ordered replay must complete after t0's");
        assert_eq!(RR_CURSOR.load(Ordering::SeqCst), 102);
    }

    #[test]
    fn out_writes_reapply() {
        let mut buf = [0u8; 8];
        let mut log = ShimLog::default();
        log.record(3, 0, vec![(buf.as_mut_ptr() as u64, vec![0xAA, 0xBB])], false);
        buf = [0; 8];
        log.replay(3);
        assert_eq!(&buf[..2], &[0xAA, 0xBB]);
    }
}

#[cfg(test)]
mod shim_log_tests {
    use super::*;
    #[test]
    fn xfrs1_roundtrip_and_negative_controls() {
        let mut l1 = ShimLog::default();
        l1.events.push(ShimEvent { fn_id: 7, ordered_seq: None, ret: 42,
            out: vec![(0x1000, vec![1, 2, 3])] });
        l1.events.push(ShimEvent { fn_id: 9, ordered_seq: Some(5), ret: 0, out: vec![] });
        let mut l2 = ShimLog::default();
        l2.events.push(ShimEvent { fn_id: 1, ordered_seq: Some(6), ret: 1,
            out: vec![(0x2000, vec![0xFF; 32]), (0x3000, vec![])] });
        let threads = vec![("GameThread".to_string(), l1), ("redIOWorker0".to_string(), l2)];
        let p = "/tmp/xfrs1_test.bin";
        save_shim_log(p, &threads).unwrap();
        let back = load_shim_log(p).unwrap();
        assert_eq!(back.len(), 2);
        assert_eq!(back[0].0, "GameThread");
        assert_eq!(back[0].1.events, threads[0].1.events);
        assert_eq!(back[1].1.events, threads[1].1.events);
        // negative controls: bit-flip → checksum; truncation; bad magic.
        let mut raw = std::fs::read(p).unwrap();
        raw[20] ^= 1;
        std::fs::write(p, &raw).unwrap();
        assert!(load_shim_log(p).is_err(), "bit-flip must fail checksum");
        std::fs::write(p, &raw[..raw.len() / 2]).unwrap();
        assert!(load_shim_log(p).is_err(), "truncation must fail");
        std::fs::write(p, b"NOTMAGIC________________").unwrap();
        assert!(load_shim_log(p).is_err(), "bad magic must fail");
        let _ = std::fs::remove_file(p);
    }
}
