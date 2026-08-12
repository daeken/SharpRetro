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

/// Replay-side deadlock detector: a replaying thread that spins too long on
/// the cursor means the execution diverged from the log (a plain-access race
/// changed a branch → a different atomic order). The emitted spin loops
/// forever; the HOST watchdog (driver-side, checks cursor progress) aborts
/// loud. This is the "divergence = detector" contract.
pub fn cursor() -> u64 { RR_CURSOR.load(Ordering::SeqCst) }
pub fn seq_now() -> u64 { RR_SEQ.load(Ordering::SeqCst) }
