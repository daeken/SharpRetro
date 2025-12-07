using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using K4os.Compression.LZ4;

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
    ENCODING,
    PREINIT_ARRAY,
    PREINIT_ARRAYSZ,
    SYMTAB_SHNDX,
    RELRSZ,
    RELR,
    RELRENT,
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

    [Flags]
    enum NsoFlags {
        TextCompress = 1 << 0,
        RoCompress = 1 << 1,
        DataCompress = 1 << 2,
        TextHash = 1 << 3,
        RoHash = 1 << 4,
        DataHash = 1 << 5,
        ExecuteOnlyMemory =  1 << 6,
    }

    public static ExeModule LoadNso(Memory<byte> data, ulong loadBase) {
        if(ReadFrom<uint>(data, 0) != 0x304f534e) // NSO0
            throw new NotSupportedException();
        var flags = (NsoFlags) ReadFrom<uint>(data, 0xC);
        Memory<byte> ReadSegment(int offset, int coffset, NsoFlags compressionFlag, out uint segOffset) {
            var fileOffset = ReadFrom<int>(data, offset);
            var memoryOffset = ReadFrom<uint>(data, offset + 4);
            var size = ReadFrom<int>(data, offset + 8);
            segOffset = memoryOffset;
            if(!flags.HasFlag(compressionFlag))
                return data.Slice(fileOffset, size);
            var csize = ReadFrom<int>(data, coffset);
            var seg = data.Slice(fileOffset, csize);
            var odata = new byte[size];
            LZ4Codec.Decode(seg.ToArray(), 0, csize, odata, 0, size);
            return odata;
        }
        var textData = ReadSegment(0x10, 0x60, NsoFlags.TextCompress, out var textOffset);
        var roData = ReadSegment(0x20, 0x64, NsoFlags.RoCompress, out var roOffset);
        var dataData = ReadSegment(0x30, 0x68, NsoFlags.DataCompress, out var dataOffset);
        return new ExeModule(loadBase, textOffset, textData.Span, roOffset, roData.Span, dataOffset, dataData.Span);
    }

    static T ReadFrom<T>(Memory<byte> data, int offset) where T : struct =>
        ReadFrom<T>(data.Span, offset);
    
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