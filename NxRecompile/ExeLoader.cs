using System.Runtime.InteropServices;
using System.Text;

namespace NxRecompile;

public class ExeModule {
    ExeModule(ulong loadBase, uint textOffset, Span<byte> text, uint roOffset, Span<byte> ro, uint dataOffset, Span<byte> data, uint bssSize) {
        Console.WriteLine($"Got module at 0x{loadBase:X} -- text at 0x{textOffset:X} ro at 0x{roOffset:X} data at 0x{dataOffset:X} bss size 0x{bssSize:X}");
    }

    public static ExeModule LoadNro(Span<byte> data, ulong loadBase) {
        //var flags = Read<uint>(data, 0x1C); // TODO: Figure out if NROs are ever compressed, if we care
        var tloc = Read<uint>(data, 0x20);
        var text = data.Slice((int) tloc, Read<int>(data, 0x24));
        var rloc = Read<uint>(data, 0x28);
        var ro = data.Slice((int) rloc, Read<int>(data, 0x2C));
        var dloc = Read<uint>(data, 0x30);
        var _data = data.Slice((int) dloc, Read<int>(data, 0x34));
        var bssSize = Read<uint>(data, 0x38);
        return new(loadBase, tloc, text, rloc, ro, dloc, _data, bssSize);
    }
    
    static T Read<T>(Span<byte> data, int offset) where T : struct {
        var size = Marshal.SizeOf<T>();
        if(size + offset > data.Length)
            throw new IndexOutOfRangeException($"Tried to read {size} bytes from offset {offset}; only {data.Length} bytes available");
        return MemoryMarshal.Cast<byte, T>(data.Slice(offset, size))[0];
    }
}

public class ExeLoader {
    const ulong InitialLoadBase = 0x1_0000_0000;
    public readonly List<ExeModule> ExeModules = new();
    public ExeLoader(string path) {
        if(!Path.Exists(path)) throw new FileNotFoundException(path);
        if(File.GetAttributes(path).HasFlag(FileAttributes.Directory)) {
            throw new NotImplementedException(); // TODO: Implement loading NSOs in a directory
        }

        var contents = (Span<byte>) File.ReadAllBytes(path);
        if(contents.Length > 0x80 && BytesToString(contents[0x10..0x14]) == "NRO0")
            ExeModules.Add(ExeModule.LoadNro(contents, InitialLoadBase));
        else
            throw new NotSupportedException("Unsupported executable file");
    }

    static string BytesToString(Span<byte> data) {
        try {
            return Encoding.ASCII.GetString(data);
        } catch {
            return null;
        }
    }
}