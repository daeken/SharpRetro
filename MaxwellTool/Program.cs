using System.Diagnostics;
using MaxwellShader;
using Pagentry.Lifter;

// MaxwellTool: Maxwell SM5x shader binary → target by extension.
// Per sera ·11090 (#2): separate CLI; output format dispatched on
// extension so .metal/.glsl/etc. slot in later without arg changes.
//
//   MaxwellTool <in.bin> <out.{spv|dis|il}>
//   MaxwellTool <in.bin>                    → spirv-dis to stdout
//   MaxwellTool --sweep <dir> .spv          → batch <dir>/*.bin → *.bin.spv
//
// .spv  = SPIR-V binary (the runtime path)
// .dis  = SPIR-V disassembly (via spirv-dis if on PATH, else hex)
// .il   = Pagentry IL tree (the M1 intermediate, for debugging)
//
// Input may have NvnCapture's 48B header (magic 0x12345678 @0);
// auto-stripped. Stage (VS/FS) read from SPH @0x30.
//
// Stderr carries the ‡note: lines from SpirvEmit + per-shader
// ‡unimpl census (= sera's fail-fast: visible at the point of
// translation, not buried in a downstream render).

if(args.Length == 0) {
    Console.Error.WriteLine(
        "usage: MaxwellTool <in.bin> [<out.{spv|dis|il}>]\n"
      + "       MaxwellTool --sweep <dir> [.spv]");
    return 1;
}

if(args[0] == "--sweep") {
    var dir = args[1];
    var ext = args.Length > 2 ? args[2] : ".spv";
    var bins = Directory.GetFiles(dir, "sh*-t?.bin")
        .OrderBy(x => x).ToArray();
    int ok = 0, fail = 0;
    foreach(var b in bins) {
        try {
            Emit(b, b + ext);
            ok++;
        } catch(Exception e) {
            Console.Error.WriteLine(
                $"  ✗ {Path.GetFileName(b)}: {e.Message}");
            fail++;
        }
    }
    Console.Error.WriteLine(
        $"[maxwell] sweep {dir}: {ok} ok, {fail} failed ({bins.Length} total)");
    return fail > 0 ? 1 : 0;
}

var inPath = args[0];
var outPath = args.Length > 1 ? args[1] : null;
try {
    Emit(inPath, outPath);
    return 0;
} catch(Exception e) {
    Console.Error.WriteLine($"✗ {Path.GetFileName(inPath)}: {e.Message}");
    if(Environment.GetEnvironmentVariable("MAXWELL_TRACE") != null)
        Console.Error.WriteLine(e.StackTrace);
    return 1;
}

static void Emit(string inPath, string? outPath) {
    var bin = File.ReadAllBytes(inPath);
    var tag = Path.GetFileName(inPath);
    var ext = outPath != null
        ? Path.GetExtension(outPath).ToLowerInvariant()
        : ".dis";  // no out → disassembly to stdout

    switch(ext) {
        case ".spv": {
            var spv = Compiler.Compile(bin, out var notes, tag);
            File.WriteAllBytes(outPath!, spv);
            Console.Error.WriteLine(
                $"  {tag} → {outPath} ({spv.Length}B"
                + (notes.Length > 0 ? $"; {notes.Length} ‡notes" : "")
                + ")");
            break;
        }
        case ".dis": {
            // Compile → spv → spirv-dis (if available) or
            // ‡ fallback to byte-hex. Goes to outPath or stdout.
            var spv = Compiler.Compile(bin, out var notes, tag);
            var tmp = Path.GetTempFileName() + ".spv";
            File.WriteAllBytes(tmp, spv);
            var dis = RunSpirvDis(tmp);
            File.Delete(tmp);
            if(outPath != null)
                File.WriteAllText(outPath, dis);
            else
                Console.Out.Write(dis);
            break;
        }
        case ".il": {
            // M1 intermediate: lift only, dump IL tree.
            // LiftShader takes the FULL .bin (NVN header +
            // SPH + code); do NOT strip.
            var lift = new MaxwellLift(Compiler.Defs);
            var lifted = lift.LiftShader(bin);
            var body = MaxwellLift.Structurize(lifted);
            using var w = outPath != null
                ? (TextWriter) new StreamWriter(outPath)
                : Console.Out;
            w.WriteLine($"// {tag}: {lifted.Count} insns, "
                + $"stage={Compiler.StageFromSph(bin)}");
            foreach(var (pc, def, il) in lifted)
                w.WriteLine($"  +{pc-0x80:x3} {def?.Name,-12}  {il}");
            w.WriteLine($"\n// ── post-Structurize ({body.Count} top-level nodes) ──");
            foreach(var n in body)
                w.WriteLine($"  {n}");
            break;
        }
        // ‡ .metal / .glsl / .hlsl = future backends per sera
        // ·11090; same Compile() → IL → different emitter.
        default:
            throw new($"unknown output extension '{ext}' "
                + "(supported: .spv .dis .il)");
    }
}

static string RunSpirvDis(string spvPath) {
    try {
        var psi = new ProcessStartInfo("spirv-dis", spvPath) {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        using var p = Process.Start(psi)!;
        var s = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        if(p.ExitCode == 0) return s;
        return $"// spirv-dis rc={p.ExitCode}: "
            + p.StandardError.ReadToEnd();
    } catch(Exception e) {
        // spirv-dis not on PATH — fallback to word-hex.
        var b = File.ReadAllBytes(spvPath);
        var sb = new System.Text.StringBuilder(
            $"// spirv-dis unavailable ({e.Message}); "
            + $"raw {b.Length/4} words:\n");
        for(var i = 0; i < b.Length; i += 4)
            sb.AppendLine($"  0x{BitConverter.ToUInt32(b, i):x8}");
        return sb.ToString();
    }
}
