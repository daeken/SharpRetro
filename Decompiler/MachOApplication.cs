using System.Text;

namespace SharpRetro.Decompiler;

/// A bounded process adapter for ordinary x86-64 Mach-O main(argc, argv).
/// Application algorithms remain entirely in the binary-derived C++.
public static class MachOApplication {
    internal static ImageRegion[] Validate(LiftedProgram program,bool libraryImports=false) {
        if(program.Image.Format != BinaryFormat.MachO || program.Image.Architecture != Architecture.X86_64 || !program.AllowsCalls)
            throw new NotSupportedException("Application adapter requires x86-64 Mach-O with call recovery");
        foreach(var (address, import) in program.Imports) {
            if(!import.StartsWith('_')) throw new NotSupportedException("No Mach-O application ABI bridge for import "+import);
            if(libraryImports) _=ApplicationImports.Get(import);
            else if(import is not ("_read" or "_write")) throw new NotSupportedException("No application ABI bridge for import " + import);
            if(!program.Image.ImportLibraries.TryGetValue(address, out string? library) || library != "/usr/lib/libSystem.B.dylib")
                throw new NotSupportedException("I/O bridge requires an explicit libSystem import binding");
        }
        // Pointer-bearing data is admitted only with the supported classic
        // loader metadata. Chained payloads must not become raw host pointers;
        // the preceding profiles retain literal/zero-filled data only.
        bool classic=libraryImports && program.Image.HasClassicFixups;
        var data = program.Image.Regions.Where(r => r.Size != 0 && !r.Executable &&
            (r.MachOType is 1 or 2 or 0xc or 0x12 || classic &&
                (r.MachOType is 6 or 7 || r.MachOType==0 && (r.Name.StartsWith("__DATA") || r.Name=="__TEXT,__const")))).ToArray();
        if(!classic && program.Image.Regions.Any(r => r.Size != 0 && !r.Executable && r.Writable && r.MachOType == 0))
            throw new NotSupportedException("Initialized writable data requires explicit relocation support");
        if(data.Any(r => r.Size > 16 * 1024 * 1024)) throw new NotSupportedException("Application image region exceeds 16 MiB adapter limit");
        if(classic) {
            foreach(var (slot,binding) in program.Image.Bindings) {
                if(binding.Library!="/usr/lib/libSystem.B.dylib") throw new NotSupportedException("Data binding requires libSystem");
                _=ApplicationImports.BindingExpression(binding.Symbol);
                if(!data.Any(r=>slot>=r.Address && slot-r.Address<r.Size && 8<=r.Size-(slot-r.Address)))
                    throw new NotSupportedException("Binding outside emitted image data");
            }
            foreach(var (slot,target) in program.Image.Rebases.Where(r=>!program.Image.Bindings.ContainsKey(r.Key)))
                if(!data.Any(r=>target>=r.Address && target-r.Address<r.Size)) throw new NotSupportedException($"Rebase target 0x{target:x} outside emitted data");
        }
        return data;
    }
    public static string Emit(LiftedProgram program, string recovered) {
        var data = Validate(program);
        var source = new StringBuilder(recovered.Replace("#pragma once\n", ""));
        source.AppendLine(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "ApplicationRuntime.hpp")));
        source.AppendLine("static void load_image(sharp_retro::application::GuestMemory& memory) {");
        int index = 0;
        foreach(var region in data) {
            if(region.Size > 16 * 1024 * 1024) throw new NotSupportedException("Application image region exceeds 16 MiB adapter limit");
            source.AppendLine($"    // {region.Name}: preferred VA 0x{region.Address:x}");
            byte[] bytes = program.Image.ReadRegion(region);
            if(bytes.Length != 0) {
                source.AppendLine($"    static constexpr std::uint8_t data_{index}[] = {{");
                foreach(var chunk in bytes.Chunk(24))
                    source.AppendLine("        " + string.Join(",", chunk.Select(b => $"0x{b:x2}")) + ",");
                source.AppendLine("    };");
            }
            source.AppendLine($"    memory.map(0x{region.Address:x}ULL, {region.Size}ULL, {(region.Writable ? "true" : "false")});");
            if(bytes.Length != 0) source.AppendLine($"    memory.initialize(0x{region.Address:x}ULL, data_{index}, sizeof(data_{index}));");
            index++;
        }
        source.AppendLine("}");
        source.AppendLine("""
            int main(int argc, char** argv) {
                try {
                    sharp_retro::application::GuestMemory memory;
                    load_image(memory);
                    return sharp_retro::application::execute(argc, argv, memory);
                } catch(const std::exception& error) {
                    std::fprintf(stderr, "recovered application: %s\n", error.what());
                    return 125;
                }
            }
            """);
        return source.ToString();
    }
}
