using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using K4os.Compression.LZ4;
using LibSharpRetro;
using NxCommon;

namespace NxRecompile;

public class ExeLoader {
    public const ulong InitialLoadBase = 0x71_0000_0000;
    public readonly List<ExeModule> ExeModules = new();
    public readonly ulong EntryPoint;
    static readonly string[] LoadOrder = [
        //"rtld", 
        "main", "subsdk0", "subsdk1", "subsdk2", "subsdk3",
        "subsdk4", "subsdk5", "subsdk6", "subsdk7", "subsdk8", "subsdk9",
        "sdk",
    ];
    public ExeLoader(string path) {
        if(!Path.Exists(path)) throw new FileNotFoundException(path);
        if(File.GetAttributes(path).HasFlag(FileAttributes.Directory)) {
            var files = Directory.EnumerateFiles(path)
                .Select(Path.GetFileName)
                .Where(x => LoadOrder.Contains(x))
                .OrderBy(x => LoadOrder.IndexOf(x))
                .ToList();
            //EntryPoint = InitialLoadBase;
            ExeModules.AddRange(files.Select((x, i) =>
                ExeModule.LoadNso(File.ReadAllBytes(Path.Join(path, x)), InitialLoadBase + 0x1_0000_0000UL * (ulong) i)));
            return;
        }

        var contents = (Span<byte>) File.ReadAllBytes(path);
        if(contents.Length > 0x80 && BytesToString(contents[0x10..0x14]) == "NRO0") {
            ExeModules.Add(ExeModule.LoadNro(contents, InitialLoadBase));
            EntryPoint = InitialLoadBase;
        } else
            throw new NotSupportedException("Unsupported executable file");
    }

    public T Load<T>(ulong addr, bool textOnly = false) where T : struct {
        if(addr < InitialLoadBase || addr > InitialLoadBase + 0x1_0000_0000UL * (ulong) ExeModules.Count)
            throw new ArgumentOutOfRangeException(nameof(addr));
        var module = ExeModules[(int) ((addr - InitialLoadBase) >> 32)];
        if(textOnly && (addr < module.TextStart || addr > module.TextEnd))
            throw new NotSupportedException($"Attempted text-only read from non-text segment");
        return module.Load<T>(addr);
    }

    static string BytesToString(Span<byte> data) {
        try {
            return Encoding.ASCII.GetString(data);
        } catch {
            return null;
        }
    }
}