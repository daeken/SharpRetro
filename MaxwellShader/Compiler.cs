using System.Reflection;
using CoreArchCompiler;
using MaxwellGenerator;
using Pagentry.Lifter;

namespace MaxwellShader;

// Facade over MaxwellLift + SpirvEmit. The single entry point
// callers use (UmbraCore's LoadShader, MaxwellTool CLI). Hides:
//   - maxwell.isa load + parse (lazy, once per process)
//   - own NvnCapture 48B header detection/skip (magic=0x12345678)
//   - SPH stage detection (data[0x30]&0xf: 1=VS-like, 2=FS)
//   - LiftShader → Structurize → SpirvEmit.Compile chain
//
// The IlNote ‡-markers + per-shader unimpl-census go to stderr
// (SpirvEmit @862 already does Console.Error.WriteLine for IlNote;
// this surfaces them in the r-log per sera's fail-fast). Returned
// `notes` carries them too for callers that want structured access.
//
// (T6)×42 per sera ·11090. Source files (Il/MaxwellLift/SpirvEmit/
// MaxwellEval/MaxwellDef + maxwell.isa) brought in verbatim from
// Pagentry @356c; namespaces left as Pagentry.Lifter +
// MaxwellGenerator for v0 (= least churn; ‡ rename post-v0).
public static class Compiler {

    static readonly Lazy<List<MaxwellDef>> _defs = new(() => {
        // maxwell.isa is an EmbeddedResource (compile-time-bound).
        var asm = Assembly.GetExecutingAssembly();
        var name = asm.GetManifestResourceNames()
            .First(n => n.EndsWith("maxwell.isa"));
        using var s = asm.GetManifestResourceStream(name)!;
        using var r = new StreamReader(s);
        // ;;-to-EOL comment strip (ListParser has no comment syntax).
        var src = string.Join("\n", r.ReadToEnd().Split('\n')
            .Select(l => { var ix = l.IndexOf(";;");
                           return ix >= 0 ? l[..ix] : l; }));
        var ptree = ListParser.Parse(src);
        ptree = MacroProcessor.Rewrite(ptree);
        var defs = new List<MaxwellDef>();
        foreach(var tle in ptree)
            if(tle is PList { Count: >= 7 } pl
               && pl[0] is PName("def"))
                defs.Add(MaxwellDef.Parse(pl));
        Console.Error.WriteLine(
            $"[maxwell] loaded {defs.Count} defs from embedded maxwell.isa");
        return defs;
    });

    public static List<MaxwellDef> Defs => _defs.Value;

    // .bin layout (as written by NvnLinux @1117 = the game's
    // raw NVN binary shader format, verbatim):
    //   0x00..0x2F  NVN header (magic 0x12345678 @0)
    //   0x30..0x7F  SPH (Maxwell shader program header, 0x50B;
    //               byte 0 low-nibble = type: 1=VS-like, 2=PS)
    //   0x80..      Maxwell SM5x code (sched word every 4th u64)
    // LiftShader expects the FULL .bin (it reads SPH @0x30 +
    // starts decode @0x80 with sched-skip aligned to that).
    // ⚠ Do NOT strip the 48B header before LiftShader — that
    // shifts decode by 6 u64 ⟹ sched-skip misaligned (verified
    // (T6)×42×2(b): ~70 "no def matched 0x001fc4…" sched-pattern
    // ‡notes per shader + stage misread as Vertex). The strip
    // is for EXTERNAL consumers (ryujinx ShaderTools wants raw
    // SPH+code, no NVN header).
    public static byte[] StripNvnHeader(byte[] bin) {
        if(bin.Length > 52
           && BitConverter.ToUInt32(bin, 0) == 0x12345678)
            return bin[48..];
        return bin;
    }

    public static SpvStage StageFromSph(byte[] bin) =>
        // bin = full .bin (NVN header + SPH + code); SPH @0x30.
        (bin[0x30] & 0xf) == 2
            ? SpvStage.Fragment : SpvStage.Vertex;

    // Maxwell binary → SPIR-V bytes. Throws on lift/emit failure
    // (= sera's fail-fast: an unhandled IL node throws inside
    // SpirvEmit; that exception surfaces here, NOT swallowed).
    // notes[] = the IlNote ‡-markers + unimpl-census this shader
    // produced (also written to stderr by SpirvEmit @862).
    public static byte[] Compile(byte[] bin, out string[] notes,
                                  string tag = "") {
        var stage = StageFromSph(bin);
        var lift = new MaxwellLift(Defs);
        var lifted = lift.LiftShader(bin);

        // ‡unimpl census (= per-shader fail-fast surfacing).
        // The .isa's `(unimplemented)` marker → IlUnimpl in
        // the IL stream; Spirv() in MaxwellGenerator skipped
        // these shaders entirely. Here: COMPILE ANYWAY (most
        // shaders work despite a few unimpl ops; the IlUnimpl
        // node emits a const-0 placeholder per spec §4 lifter
        // self-‡), but record + emit a per-shader summary so
        // it's visible in the r-log.
        var unimpls = lifted
            .Where(x => x.il.ToString().Contains("‡unimpl"))
            .Select(x => x.def?.Name ?? "?").Distinct().ToList();
        var noteList = new List<string>();
        if(unimpls.Count > 0)
            noteList.Add($"‡unimpl: {string.Join(",", unimpls)}");

        // Capture SpirvEmit's stderr ‡note: lines for this shader
        // by hooking Console.Error around the Compile call. ‡ v0
        // = leave them on stderr (they reach the r-log); the
        // structured notes[] return is for MaxwellTool --notes.
        var em = new SpirvEmit(stage);
        var spv = em.Compile(stage, lifted);

        if(noteList.Count > 0) {
            var pfx = tag.Length > 0 ? $"[maxwell {tag}] " : "[maxwell] ";
            Console.Error.WriteLine(pfx + $"{stage} {lifted.Count}insn → "
                + $"{spv.Length}B spv; " + string.Join("; ", noteList));
        }
        notes = noteList.ToArray();
        return spv;
    }

    public static byte[] Compile(byte[] bin, string tag = "")
        => Compile(bin, out _, tag);
}
