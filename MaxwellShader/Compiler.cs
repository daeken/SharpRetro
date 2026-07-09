using System.Reflection;
using CoreArchCompiler;
using MaxwellGenerator;
// (types now in this namespace)

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
// Namespaces unified under MaxwellShader.
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

    // (T6)×44 ×3: SPH OmapTarget — per-RT component write-mask
    // for FS. SphType=2 (PS) places this at SPH+0x48 = file
    // offset 0x78. 32 bits = 8 RTs × 4-bit per-RT (bit 4N+k =
    // RT N writes component k). Verified at-bytes on sh0442:
    // @0x78 = ff 0f 00 00 = 0x00000fff = RT0/1/2 all-rgba ⟹
    // fs442 is 3-MRT. sh0050 (single-RT) = 0x0000000f. Used by
    // SpirvEmit to declare N output vars + emit R[4N..4N+3]
    // → oColor{N} per RT instead of RT0-only.
    public static uint OmapTargetsFromSph(byte[] bin) =>
        StageFromSph(bin) == SpvStage.Fragment
            ? BitConverter.ToUInt32(bin, 0x78) : 0;

    // Maxwell binary → SPIR-V bytes. Throws on lift/emit failure
    // (= sera's fail-fast: an unhandled IL node throws inside
    // SpirvEmit; that exception surfaces here, NOT swallowed).
    // notes[] = the IlNote ‡-markers + unimpl-census this shader
    // produced (also written to stderr by SpirvEmit @862).
    // (T6)×47 in-shader-printf knob: UMBRA_LIFT_DUMP="shIdx:pcOff:rA,rB,rC"
    // — splice R252←R[A], R253←R[B], R254←R[C], R251←1.0 into lifted[]
    // just BEFORE the insn at file-offset (0x80 + pcOff). Structurize
    // wraps the splice in whatever IlIf governs that pc ⟹ at FS-exit
    // _gpr[252-254] = Select(governingP, R[A,B,C]-at-that-point, 0).
    // R251 = Select(governingP, 1.0, 0) = the predicate-marker. SpirvEmit
    // FS-exit (when _dumpRs != null) writes (R252,R253,R254,R251) to
    // oColor[0] instead of the omap-driven R0-3. = read OUR ACTUAL emit's
    // intermediate GPR values, not a hand-recreated GLSL approximation.
    // pcOff is hex, matches the +XXX shown in MaxwellTool .il output.
    static readonly (int sh, ulong pc, int[] rs)? _dump =
        Environment.GetEnvironmentVariable("UMBRA_LIFT_DUMP")
            ?.Split(':') is [var s, var p, var r]
        ? (int.Parse(s),
           0x80 + Convert.ToUInt64(p, 16),
           r.Split(',').Select(int.Parse).ToArray())
        : null;

    // (T6)×66: out texKinds (binding → SampKind type-bits)
    // so the host can match imageView type to the shader's
    // declared OpTypeImage Dim. Empty for VS (no tex binds in
    // this corpus) and for shaders with no texture ops.
    public static byte[] Compile(byte[] bin, out string[] notes,
                                  out Dictionary<int,int> texKinds,
                                  string tag = "") {
        var stage = StageFromSph(bin);
        var lift = new MaxwellLift(Defs);
        var lifted = lift.LiftShader(bin);

        int[]? dumpRs = null;
        if(_dump is var (dsh, dpc, drs)
                && tag.Contains($"{dsh:d4}")) {
            var ix = lifted.FindIndex(e => e.pc == dpc);
            if(ix < 0) {
                Console.Error.WriteLine(
                    $"[maxwell {tag}] ⚠ LIFT_DUMP pc=+{dpc-0x80:x} not in lifted[] "
                    + $"(nearest: +{lifted.MinBy(e => Math.Abs((long)e.pc-(long)dpc)).pc-0x80:x})");
            } else {
                var F32 = new IlType.F(32);
                // R251 ← 1.0 (predicate-marker; Select(P,1,0) at exit)
                lifted.Insert(ix, (dpc, null!,
                    new IlWriteReg(RegKind.Gpr, 251,
                        new IlConst(F32, 0x3f800000ul))));
                // R252+k ← R[drs[k]] (up to 3)
                for(var k = 0; k < Math.Min(drs.Length, 3); k++)
                    lifted.Insert(ix + 1 + k, (dpc, null!,
                        new IlWriteReg(RegKind.Gpr, 252 + k,
                            new IlReadReg(F32, RegKind.Gpr, drs[k]))));
                dumpRs = drs;
                Console.Error.WriteLine(
                    $"[maxwell {tag}] LIFT_DUMP @+{dpc-0x80:x} "
                    + $"R[{string.Join(",", drs)}]→R252+ (P-marker→R251)");
            }
        }

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
        var omap = OmapTargetsFromSph(bin);
        var em = new SpirvEmit(stage, omap, dumpRs != null);
        var spv = em.Compile(stage, lifted);
        texKinds = em.TexKinds;
        // (T6)×66 fail-fast: surface non-2D tex bindings in
        // notes[] (= visible in r-log; the host MUST match
        // these or the descriptor-bind is UB).
        var nonStd = texKinds.Where(kv => (kv.Value & 0xf) != SampKind.D2)
                             .ToList();
        if(nonStd.Count > 0)
            noteList.Add("TexKinds: " + string.Join(",", nonStd
                .Select(kv => $"b{kv.Key}=k{kv.Value:x}")));
        // (T6)×44 fail-fast for ‡v0-deferrals: surface MRT-count
        // so multi-RT shaders are visible in r-log even before
        // the MRT path is exercised.
        var nMrt = 0;
        for(var n = 0; n < 8; n++)
            if((omap & (0xfu << (4*n))) != 0) nMrt++;
        if(nMrt > 1)
            noteList.Add($"MRT={nMrt} (omap=0x{omap:x})");

        if(noteList.Count > 0) {
            var pfx = tag.Length > 0 ? $"[maxwell {tag}] " : "[maxwell] ";
            Console.Error.WriteLine(pfx + $"{stage} {lifted.Count}insn → "
                + $"{spv.Length}B spv; " + string.Join("; ", noteList));
        }
        notes = noteList.ToArray();
        return spv;
    }

    public static byte[] Compile(byte[] bin, out string[] notes,
                                  string tag = "")
        => Compile(bin, out notes, out _, tag);
    public static byte[] Compile(byte[] bin, string tag = "")
        => Compile(bin, out _, out _, tag);
}
