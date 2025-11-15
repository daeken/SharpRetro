using System.Runtime.InteropServices;
using System.Text;

namespace NxRecompile;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
unsafe struct ModHeader {
    [FieldOffset(0x00)] public fixed byte Magic[4];
    [FieldOffset(0x04)] public uint DynamicOffset;
    [FieldOffset(0x08)] public uint BssStartOffset;
    [FieldOffset(0x0C)] public uint BssEndOffset;
    [FieldOffset(0x10)] public uint ExceptionInfoStartOffset;
    [FieldOffset(0x14)] public uint ExceptionInfoEndOffset;
    [FieldOffset(0x18)] public uint ModuleOffset;
}

enum DynamicKey {
    NULL,
    NEEDED,
    PLTRELSZ,
    PLTGOT,
    HASH,
    STRTAB,
    SYMTAB,
    RELA,
    RELASZ,
    RELAENT,
    STRSZ,
    SYMENT,
    INIT,
    FINI,
    SONAME,
    RPATH,
    SYMBOLIC,
    REL,
    RELSZ,
    RELENT,
    PLTREL,
    DEBUG,
    TEXTREL,
    JMPREL,
    BIND_NOW,
    INIT_ARRAY,
    FINI_ARRAY,
    INIT_ARRAYSZ,
    FINI_ARRAYSZ,
    RUNPATH,
    FLAGS,
}

public class ExeModule {
    public readonly Memory<byte> Binary;
    public readonly ulong LoadBase;
    public readonly ulong TextStart, TextEnd;
    public readonly ulong RoStart, RoEnd;
    public readonly ulong DataStart, DataEnd;
    public readonly ulong BssStart, BssEnd;
    
    unsafe ExeModule(ulong loadBase, uint textOffset, Span<byte> text, uint roOffset, Span<byte> ro, uint dataOffset, Span<byte> data) {
        LoadBase = loadBase;
        TextStart = loadBase + textOffset;
        TextEnd = loadBase + textOffset + (ulong) text.Length;
        RoStart = loadBase + roOffset;
        RoEnd = loadBase + roOffset + (ulong) ro.Length;
        DataStart = loadBase + dataOffset;
        DataEnd = loadBase + dataOffset + (ulong) data.Length;

        Binary = new byte[Math.Max(dataOffset + data.Length, Math.Max(roOffset + ro.Length, textOffset + text.Length))];
        text.CopyTo(Binary[(int) textOffset..].Span);
        ro.CopyTo(Binary[(int) roOffset..].Span);
        data.CopyTo(Binary[(int) dataOffset..].Span);

        var modOff = Read<uint>(4);
        var header = Read<ModHeader>(modOff);
        if(Encoding.ASCII.GetString(new Span<byte>(header.Magic, 4)) != "MOD0")
            throw new NotSupportedException("Missing MOD0 magic");
        
        BssStart = loadBase + header.BssStartOffset;
        BssEnd = loadBase + header.BssEndOffset;

        var dynamic = new Dictionary<DynamicKey, ulong>();
        var dynOff = modOff + header.DynamicOffset;
        while(true) {
            var (tag, val) = ((DynamicKey) Read<ulong>(dynOff), Read<ulong>(dynOff + 8));
            dynOff += 16;
            if(tag == DynamicKey.NULL)
                break;
            if(tag != DynamicKey.NEEDED)
                dynamic[tag] = val;
        }
        
        var dynstr = Encoding.ASCII.GetString(Binary.Slice((int) dynamic[DynamicKey.STRTAB], (int) dynamic[DynamicKey.STRSZ]).Span);
    }

    T Read<T>(uint offset) where T : struct {
        var size = Marshal.SizeOf<T>();
        if(size + offset > Binary.Length)
            throw new IndexOutOfRangeException($"Tried to read {size} bytes from offset {offset}; only {Binary.Length} bytes available");
        return MemoryMarshal.Cast<byte, T>(Binary.Slice((int) offset, size).Span)[0];
    }

    public T Load<T>(ulong addr) where T : struct {
        if(addr < LoadBase) throw new Exception($"Tried to load 0x{addr:X} before 0x{LoadBase:X}");
        return Read<T>((uint) (addr - LoadBase));
    }

    public static ExeModule LoadNro(Span<byte> data, ulong loadBase) {
        //var flags = Read<uint>(data, 0x1C); // TODO: Figure out if NROs are ever compressed, if we care to support that
        var tloc = ReadFrom<uint>(data, 0x20);
        var text = data.Slice((int) tloc, ReadFrom<int>(data, 0x24));
        var rloc = ReadFrom<uint>(data, 0x28);
        var ro = data.Slice((int) rloc, ReadFrom<int>(data, 0x2C));
        var dloc = ReadFrom<uint>(data, 0x30);
        var _data = data.Slice((int) dloc, ReadFrom<int>(data, 0x34));
        return new(loadBase, tloc, text, rloc, ro, dloc, _data);
    }
    
    static T ReadFrom<T>(Span<byte> data, int offset) where T : struct {
        var size = Marshal.SizeOf<T>();
        if(size + offset > data.Length)
            throw new IndexOutOfRangeException($"Tried to read {size} bytes from offset {offset}; only {data.Length} bytes available");
        return MemoryMarshal.Cast<byte, T>(data.Slice(offset, size))[0];
    }
}

public class ExeLoader {
    public const ulong InitialLoadBase = 0x71_0000_0000;
    public readonly List<ExeModule> ExeModules = new();
    public readonly ulong EntryPoint;
    public ExeLoader(string path) {
        if(!Path.Exists(path)) throw new FileNotFoundException(path);
        if(File.GetAttributes(path).HasFlag(FileAttributes.Directory)) {
            throw new NotImplementedException(); // TODO: Implement loading NSOs in a directory
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