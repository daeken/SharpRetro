//! xfusion-recomp — the x64-guest recompiler crate. Hand-ported decode primitives
//! (`decode.rs`) + generated dispatch table (`disassembler.rs`, from XFusionScaffold)
//! + generated per-insn lift bodies (`lift.rs`) that emit into the `Builder` trait.
//!
//! The x86 architecture differs from aarch64: variable-length insns, semantics-templates
//! separate from encoding-rows, hand-written prefix/ModRM decode. See DESIGN.md.

pub mod decode;
pub mod operand;
pub mod state;
pub mod hand_lift;
pub mod x64_stub;
pub mod disassembler;
pub mod lift;
// pub mod state;         // X86State: RegState impl — next
