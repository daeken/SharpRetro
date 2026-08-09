Instruments for diffing the new ArchCompiler against the legacy compiler at each rung.

legacy-tree-dump.cs: dumps post-MacroProcessor.Rewrite tree from CoreArchCompiler.
  Rung-1a target — diff against `dotnet run --project ArchCompiler -- <file.isa>`.
  Verified byte-identical @ArchCompiler-scaffold: aarch64(398)/mips(94)/dmg(113).
