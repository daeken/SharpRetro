using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpRetro.Decompiler;

public static class Program {
    static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true, Converters = { new JsonStringEnumConverter() }
    };
    public static int Main(string[] args) {
        try {
            if(args.Length == 1 && args[0] is "--help" or "-h") { Usage(); return 0; }
            if(args.Length < 2) { Usage(); return 2; }
            string command = args[0];
            if(command is not ("inspect" or "decompile" or "decompile-app" or "decompile-function" or "lift")) throw new ArgumentException("Unknown command: " + command);
            var image = BinaryImage.Open(args[1]);
            if(command == "inspect") {
                if(args.Length != 2) throw new ArgumentException("inspect takes only an image path");
                Console.WriteLine(JsonSerializer.Serialize(new { image.Format, image.Architecture, image.Sha256, image.Entry, image.Regions, image.Imports, image.ImportLibraries, image.FunctionStarts, image.HasClassicFixups, image.Rebases, image.Bindings,
                    image.ImageBase,image.PeImageSize,image.Exports,image.PeImportSlots,image.PeRuntimeFunctions,image.PeRelocations,image.PeHasTls }, JsonOptions));
                return 0;
            }
            ulong? entry = null;
            var ranges = new List<CodeRange>();
            string? output = null;
            int limit = 100_000;
            ControlFlowStyle flowStyle = ControlFlowStyle.Auto;
            string? representation = null;
            string memory = "auto";
            string? exportName=null;
            for(int i = 2; i < args.Length; i++) {
                string option = args[i];
                if(++i == args.Length) throw new ArgumentException("Missing value for " + option);
                switch(option) {
                    case "--entry": if(entry != null) throw new ArgumentException("Duplicate entry"); entry = Number(args[i]); break;
                    case "--export": if(exportName!=null) throw new ArgumentException("Duplicate export"); exportName=args[i]; break;
                    case "--range": {
                        var parts = args[i].Split(':');
                        if(parts.Length != 2) throw new ArgumentException("Range must be VA:SIZE");
                        ranges.Add(new(Number(parts[0]), Number(parts[1]))); break;
                    }
                    case "--output": if(output != null) throw new ArgumentException("Duplicate output"); output = args[i]; break;
                    case "--instruction-limit": limit = int.Parse(args[i], System.Globalization.CultureInfo.InvariantCulture); break;
                    case "--representation": representation = args[i] is "machine" or "recovered" or "blocks" ? args[i]
                        : throw new ArgumentException("Representation must be machine, recovered, or blocks"); break;
                    case "--memory": memory = args[i] is "auto" or "native" or "checked" ? args[i]
                        : throw new ArgumentException("Memory must be auto, native, or checked"); break;
                    case "--control-flow": flowStyle = args[i] switch {
                        "auto" => ControlFlowStyle.Auto, "structured" => ControlFlowStyle.Structured, "dispatch" => ControlFlowStyle.Dispatch,
                        _ => throw new ArgumentException("Control flow must be auto, structured, or dispatch")
                    }; break;
                    default: throw new ArgumentException("Unknown option: " + option);
                }
            }
            bool functionOnly=command=="decompile-function";
            bool app = command == "decompile-app" || functionOnly;
            if(exportName!=null) {
                if(!functionOnly || entry!=null) throw new ArgumentException("--export requires decompile-function and no --entry");
                if(!image.Exports.TryGetValue(exportName,out var export)) throw new ArgumentException("Unknown PE export "+exportName);
                if(export.Forwarder!=null) throw new NotSupportedException("Forwarded PE export "+export.Forwarder);
                entry=export.Address;
            }
            if(functionOnly && (entry==null || image.Format!=BinaryFormat.Pe || image.Architecture!=Architecture.X86_64))
                throw new NotSupportedException("decompile-function requires Windows x64 and explicit --export or --entry");
            representation ??= app ? "recovered" : "machine";
            if(memory!="auto" && (!app || representation!="recovered")) throw new NotSupportedException("Explicit memory modes require recovered application representation");
            if(!app && (representation is "recovered" or "blocks")) throw new NotSupportedException("Recovered native storage currently requires decompile-app");
            if(app && representation == "blocks" && flowStyle != ControlFlowStyle.Auto)
                throw new NotSupportedException("Blocks representation emits basic blocks; use recovered or machine representation for explicit control-flow modes");
            if(app) {
                if(!functionOnly && (image.Format != BinaryFormat.MachO || image.Architecture != Architecture.X86_64))
                    throw new NotSupportedException("Application recovery currently requires an x86-64 Mach-O executable");
                if(ranges.Count == 0) ranges.AddRange(image.Regions.Where(r => r.Executable && r.FileSize != 0 && r.MachOType != 8)
                    .Select(r => new CodeRange(r.Address, r.FileSize)));
                entry ??= image.Entry;
            }
            if(functionOnly && representation!="recovered") throw new NotSupportedException("PE functions require recovered representation");
            if(functionOnly && memory=="checked") throw new NotSupportedException("PE function profile currently requires native memory");
            if(ranges.Count == 0) throw new ArgumentException("At least one explicit --range VA:SIZE is required");
            var program = ControlFlow.Recover(image, entry ?? ranges[0].Address, ranges, limit, allowCalls: app);
            string source;
            if(command == "lift") source = string.Join("\n", program.Instructions.Select(i => $"0x{i.Address:x}: {i.Text}\n{i.Body}\n"));
            else if(app && representation == "recovered") {
                var emitter = new FunctionEmitter(); source = emitter.Emit(program,flowStyle,functionOnly ? "native" : memory,functionOnly);
                Console.Error.WriteLine("Recovered values: " + emitter.Summary);
            } else if(app && representation == "blocks") {
                var emitter = new SourceEmitter(); source = emitter.Emit(program);
                Console.Error.WriteLine("Recovered values: " + emitter.Summary);
            } else {
                var emitter = new CppEmitter();
                source = emitter.Emit(program, flowStyle);
                Console.Error.WriteLine("Control flow: " + emitter.ControlFlowSummary);
                if(app) source = MachOApplication.Emit(program, source);
            }
            if(output == null) Console.WriteLine(source);
            else {
                if(Path.GetFullPath(output) == Path.GetFullPath(args[1])) throw new ArgumentException("Output would overwrite input image");
                // Never publish a partial source file when an unsupported node is encountered.
                File.WriteAllText(output, source);
            }
            Console.Error.WriteLine($"Emitted {program.Instructions.Count} reachable instructions; SHA-256 {image.Sha256}. This is translation output, not equivalence evidence.");
            return 0;
        } catch(Exception e) when(e is ArgumentException or FormatException or OverflowException or IOException or InvalidDataException or NotSupportedException or InvalidOperationException or StructuringException) {
            Console.Error.WriteLine("decompiler: " + e.Message); return 1;
        }
    }
    static ulong Number(string value) => value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
        ? Convert.ToUInt64(value[2..], 16) : ulong.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
    static void Usage() => Console.Error.WriteLine("""
        SharpRetro binary-to-C++ development pipeline
          inspect IMAGE
          lift IMAGE --range VA:SIZE [--range VA:SIZE ...] [--entry VA]
          decompile IMAGE --range VA:SIZE [--entry VA] [--output recovered.hpp]
            [--control-flow auto|structured|dispatch]
          decompile-app IMAGE [--entry VA] [--output recovered.cpp]
            [--representation machine|blocks|recovered]
            [--control-flow auto|structured|dispatch]
            [--memory auto|native|checked]
          decompile-function WINDOWS_X64_IMAGE --export NAME [--output recovered.cpp]
            [--entry VA instead of --export] [--control-flow auto|structured|dispatch]
        Numbers are decimal or 0x-prefixed hexadecimal. Addresses are preferred VAs.
        Initial semantic frontend: x86/x86-64 integer code. Whole-program recovery,
        general relocations/platform runtimes and high-level ABI/type recovery are pending.
        decompile-app supports direct calls and explicit Mach-O/libSystem ABI bridges.
        Auto uses structured C++ when supported, otherwise the dispatch baseline.
        """);
}
