# oracle-baseline

Frozen output of the legacy CoreArchCompiler generators @fa46496, captured 2026-08-09
BEFORE the ArchCompiler rewrite. This is the byte-diffable answer-key: the rewrite's
C# backend must reproduce these files byte-identical (sha256 match) at rung-2.

The freeze-law: "we know how it behaves RIGHT NOW" is the value — the legacy generator's
output IS the acceptance harness for every step of the rewrite. Rung-graded:
  rung-1a: parse+macro tree-dump byte-diff
  rung-1b: type-inferred tree byte-diff
  rung-2:  Aarch64Cpu/Generated/*.cs byte-diff vs oracle-baseline/aarch64/
  rung-3:  same for mips/dmg (frontend-generality)
  rung-4:  Rust backend (new capability, born on settled core)

Regenerate the legacy compiler's output to compare:
  (cd Aarch64Generator && dotnet run) -> Aarch64Cpu/Generated/
  (cd SharpStationGenerator && dotnet run) -> SharpStationCore/Generated/
  (cd DamageGenerator && dotnet run) -> DamageCore/Generated/
  (cd XFusionGenerator && dotnet run) -> XFusionCpu/Generated/

The .isa files in isa/ are the survive-verbatim spec — they DO NOT change across
the rewrite (modulo mlet-normalization lint).

sha256s at capture-time: see git log for this commit.
