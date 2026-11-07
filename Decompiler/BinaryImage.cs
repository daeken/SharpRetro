using System.Buffers.Binary;
using System.Security.Cryptography;

namespace SharpRetro.Decompiler;

public enum BinaryFormat { Pe, Elf, MachO }
public enum Architecture { X86, X86_64, Aarch64 }
public sealed record ImageRegion(string Name, ulong Address, ulong Size, ulong FileOffset,
    ulong FileSize, bool Executable, bool Writable, uint MachOType = 0);
public sealed record ImageBinding(string Symbol,string Library);

/// Container parsing and virtual-address mapping, independent of instruction semantics.
/// These are preferred virtual addresses, before dynamic relocations or runtime loading.
public sealed partial class BinaryImage {
    readonly byte[] data;
    public BinaryFormat Format { get; private set; }
    public Architecture Architecture { get; private set; }
    public ulong Entry { get; private set; }
    public string Sha256 { get; }
    public IReadOnlyList<ImageRegion> Regions => regions;
    readonly List<ImageRegion> regions = [];
    readonly Dictionary<ulong, string> imports = [];
    public IReadOnlyDictionary<ulong, string> Imports => imports;
    readonly Dictionary<ulong, string> importLibraries = [];
    public IReadOnlyDictionary<ulong, string> ImportLibraries => importLibraries;
    readonly List<ulong> functionStarts = [];
    public IReadOnlyList<ulong> FunctionStarts => functionStarts;
    public bool HasClassicFixups { get; private set; }
    readonly Dictionary<ulong,ulong> rebases=[];
    readonly Dictionary<ulong,ImageBinding> bindings=[];
    public IReadOnlyDictionary<ulong,ulong> Rebases => rebases;
    public IReadOnlyDictionary<ulong,ImageBinding> Bindings => bindings;

    public static BinaryImage Open(string path) => new(File.ReadAllBytes(path));
    public BinaryImage(byte[] bytes) {
        data = (byte[])bytes.Clone();
        Sha256 = Convert.ToHexStringLower(SHA256.HashData(data));
        if(data.Length >= 2 && data[0] == 'M' && data[1] == 'Z') ParsePe();
        else if(data.Length >= 4 && U32(0) == 0x464c457f) ParseElf();
        else if(data.Length >= 4 && U32(0) == 0xfeedfacf) ParseMachO();
        else throw new NotSupportedException("Expected PE, little-endian ELF, or thin 64-bit little-endian Mach-O");
        var ordered = regions.Where(r => r.Size != 0).OrderBy(r => r.Address).ToArray();
        for(int i = 1; i < ordered.Length; i++)
            if(ordered[i].Address - ordered[i - 1].Address < ordered[i - 1].Size)
                throw new InvalidDataException("Overlapping virtual regions are ambiguous");
    }

    ReadOnlySpan<byte> Slice(ulong offset, ulong size) {
        if(offset > (ulong)data.Length || size > (ulong)data.Length - offset)
            throw new InvalidDataException($"Truncated image at file offset 0x{offset:x}");
        return data.AsSpan((int)offset, (int)size);
    }
    ushort U16(ulong o) => BinaryPrimitives.ReadUInt16LittleEndian(Slice(o, 2));
    uint U32(ulong o) => BinaryPrimitives.ReadUInt32LittleEndian(Slice(o, 4));
    ulong U64(ulong o) => BinaryPrimitives.ReadUInt64LittleEndian(Slice(o, 8));
    string Name(ulong o, int n) => System.Text.Encoding.ASCII.GetString(Slice(o, (ulong)n)).TrimEnd('\0');
    static ulong Add(ulong a, ulong b) => checked(a + b);
    void Region(string name, ulong address, ulong size, ulong offset, ulong fileSize, bool exec, bool write, uint machOType = 0) {
        _ = Add(address, size);
        Slice(offset, fileSize);
        if(fileSize > size) throw new InvalidDataException("File-backed region exceeds virtual size");
        regions.Add(new(name, address, size, offset, fileSize, exec, write, machOType));
    }

    public byte[] ReadRegion(ImageRegion region) {
        if(!regions.Any(r=>region.Address>=r.Address && region.Address-r.Address<=r.Size &&
            region.Size<=r.Size-(region.Address-r.Address) && region.FileSize<=region.Size &&
            region.Address-r.Address<=r.FileSize && region.FileSize<=r.FileSize-(region.Address-r.Address) &&
            region.FileOffset==r.FileOffset+region.Address-r.Address) && !regions.Contains(region))
            throw new ArgumentException("Region does not belong to image");
        return Slice(region.FileOffset, region.FileSize).ToArray();
    }

    public ReadOnlySpan<byte> ReadExecutable(ulong address, int maximum) {
        if(maximum <= 0) throw new ArgumentOutOfRangeException(nameof(maximum));
        var r = regions.SingleOrDefault(r => r.Executable && address >= r.Address && address - r.Address < r.FileSize)
            ?? throw new InvalidDataException($"No file-backed executable bytes at VA 0x{address:x}");
        ulong delta = address - r.Address;
        return Slice(Add(r.FileOffset, delta), Math.Min((ulong)maximum, r.FileSize - delta));
    }

    void ParsePe() {
        Format = BinaryFormat.Pe;
        ulong pe = U32(0x3c);
        if(U32(pe) != 0x4550) throw new InvalidDataException("Invalid PE signature");
        Architecture = U16(pe + 4) switch {
            0x14c => Architecture.X86, 0x8664 => Architecture.X86_64, 0xaa64 => Architecture.Aarch64,
            _ => throw new NotSupportedException("Unsupported PE machine")
        };
        int count = U16(pe + 6), optSize = U16(pe + 20);
        ulong opt = pe + 24;
        Slice(opt, (ulong)optSize);
        ushort magic = U16(opt);
        bool wide = magic == 0x20b;
        if(magic != 0x10b && !wide) throw new NotSupportedException("Unsupported PE optional header");
        if(optSize < (wide ? 112 : 96)) throw new InvalidDataException("Short PE optional header");
        if(wide != (Architecture != Architecture.X86)) throw new InvalidDataException("PE machine/header width mismatch");
        ulong imageBase = wide ? U64(opt + 24) : U32(opt + 28);
        Entry = Add(imageBase, U32(opt + 16));
        ulong table = opt + (ulong)optSize;
        Slice(table, (ulong)count * 40);
        for(int i = 0; i < count; i++) {
            ulong s = table + (ulong)i * 40;
            ulong virtualSize = U32(s + 8), rawSize = U32(s + 16);
            ulong size = virtualSize == 0 ? rawSize : virtualSize;
            uint flags = U32(s + 36);
            // Raw alignment padding beyond VirtualSize is not part of the mapped section.
            Region(Name(s, 8), Add(imageBase, U32(s + 12)), size, U32(s + 20), Math.Min(size, rawSize),
                (flags & 0x20000000) != 0, (flags & 0x80000000) != 0);
            peSectionFlags[Add(imageBase,U32(s+12))]=flags;
        }
        ParsePeDirectories(opt,optSize,wide,imageBase);
    }

    void ParseElf() {
        Format = BinaryFormat.Elf;
        Slice(0, 16);
        if(data[5] != 1 || data[6] != 1) throw new NotSupportedException("Only little-endian ELF version 1 is supported");
        bool wide = data[4] == 2;
        if(data[4] is not (1 or 2)) throw new InvalidDataException("Invalid ELF class");
        Slice(0, wide ? 64UL : 52UL);
        if(U16(16) is not (2 or 3)) throw new NotSupportedException("ELF requires an executable/shared image, not ET_REL");
        Architecture = U16(18) switch {
            3 => Architecture.X86, 62 => Architecture.X86_64, 183 => Architecture.Aarch64,
            _ => throw new NotSupportedException("Unsupported ELF machine")
        };
        if(wide != (Architecture != Architecture.X86)) throw new InvalidDataException("ELF machine/class mismatch");
        Entry = wide ? U64(24) : U32(24);
        ulong table = wide ? U64(32) : U32(28);
        int stride = U16(wide ? 54UL : 42UL), count = U16(wide ? 56UL : 44UL);
        if(count == 0xffff) throw new NotSupportedException("ELF extended program-header count");
        if(stride < (wide ? 56 : 32)) throw new InvalidDataException("Short ELF program header");
        Slice(table, (ulong)stride * (ulong)count);
        for(int i = 0; i < count; i++) {
            ulong h = Add(table, (ulong)i * (ulong)stride);
            if(U32(h) != 1) continue;
            uint flags = U32(h + (wide ? 4UL : 24UL));
            Region($"LOAD{i}", wide ? U64(h + 16) : U32(h + 8), wide ? U64(h + 40) : U32(h + 20),
                wide ? U64(h + 8) : U32(h + 4), wide ? U64(h + 32) : U32(h + 16),
                (flags & 1) != 0, (flags & 2) != 0);
        }
    }

    void ParseMachO() {
        Format = BinaryFormat.MachO;
        Slice(0, 32);
        Architecture = U32(4) switch {
            0x1000007 => Architecture.X86_64, 0x100000c => Architecture.Aarch64,
            _ => throw new NotSupportedException("Unsupported Mach-O CPU")
        };
        if(U32(12) is not (2 or 6 or 8)) throw new NotSupportedException("Mach-O requires an executable/dylib/bundle, not MH_OBJECT");
        uint count = U32(16), commandBytes = U32(20);
        Slice(32, commandBytes);
        ulong cursor = 32, end = 32UL + commandBytes;
        ulong? entryOffset = null;
        var segments = new List<(ulong Address, ulong Offset, ulong FileSize)>();
        var stubs = new List<(ulong Address, ulong Size, uint First, uint Stride)>();
        var pointers = new List<(ulong Address, ulong Size, uint First)>();
        var libraries = new List<string>();
        ulong? symbols = null, indirect = null;
        ulong? textBase = null, startsCommand = null, dyldInfo = null;
        bool chainedFixups=false;
        for(uint i = 0; i < count; i++) {
            if(cursor > end || end - cursor < 8) throw new InvalidDataException("Truncated Mach-O command header");
            uint command = U32(cursor), size = U32(cursor + 4);
            if(size < 8 || size > end - cursor) throw new InvalidDataException("Invalid Mach-O command size");
            if(command == 0x19) { // LC_SEGMENT_64
                if(size < 72) throw new InvalidDataException("Short Mach-O segment command");
                ulong address = U64(cursor + 24), vmSize = U64(cursor + 32);
                ulong offset = U64(cursor + 40), fileSize = U64(cursor + 48);
                Slice(offset, fileSize);
                if(fileSize > vmSize) throw new InvalidDataException("Mach-O segment file size exceeds VM size");
                _ = Add(address, vmSize);
                segments.Add((address, offset, fileSize));
                if(Name(cursor + 8,16)=="__TEXT") {
                    if(textBase!=null) throw new InvalidDataException("Duplicate Mach-O text segment");
                    textBase=address;
                }
                uint protection = U32(cursor + 60), sections = U32(cursor + 64);
                if((ulong)sections * 80 > size - 72UL) throw new InvalidDataException("Short Mach-O section table");
                for(uint j = 0; j < sections; j++) {
                    ulong s = cursor + 72 + j * 80UL;
                    ulong va = U64(s + 32), length = U64(s + 40), raw = U32(s + 48);
                    uint flags = U32(s + 64), type = flags & 0xff;
                    bool zeroFill = type is 1 or 0xc or 0x12;
                    if(va < address || va - address > vmSize || length > vmSize - (va - address))
                        throw new InvalidDataException("Mach-O section outside segment VM range");
                    if(!zeroFill && (raw < offset || raw - offset > fileSize || length > fileSize - (raw - offset)))
                        throw new InvalidDataException("Mach-O section outside segment file range");
                    Region(Name(s + 16, 16) + "," + Name(s, 16), va, length, zeroFill ? 0 : raw,
                        zeroFill ? 0 : length, (protection & 4) != 0 && (flags & 0x80000400) != 0, (protection & 2) != 0, type);
                    if(type == 8) stubs.Add((va, length, U32(s + 68), U32(s + 72)));
                    if(type is 6 or 7) pointers.Add((va, length, U32(s + 68)));
                }
            } else if(command == 0x80000028) { // LC_MAIN
                if(size < 24) throw new InvalidDataException("Short LC_MAIN");
                entryOffset = U64(cursor + 8);
            } else if(command == 2) { // LC_SYMTAB
                if(size != 24 || symbols != null) throw new InvalidDataException("Invalid/duplicate LC_SYMTAB");
                symbols = cursor;
            } else if(command == 0xb) { // LC_DYSYMTAB
                if(size != 80 || indirect != null) throw new InvalidDataException("Invalid/duplicate LC_DYSYMTAB");
                indirect = cursor;
            } else if(command==0x26) { // LC_FUNCTION_STARTS
                if(size!=16 || startsCommand!=null) throw new InvalidDataException("Invalid/duplicate LC_FUNCTION_STARTS");
                startsCommand=cursor;
            } else if(command is 0x22 or 0x80000022) {
                if(size!=48 || dyldInfo!=null) throw new InvalidDataException("Invalid/duplicate LC_DYLD_INFO");
                dyldInfo=cursor;
            } else if(command==0x80000034) {
                if(size!=16) throw new InvalidDataException("Invalid LC_DYLD_CHAINED_FIXUPS");
                Slice(U32(cursor+8),U32(cursor+12));
                chainedFixups|=U32(cursor+12)!=0;
            } else if(command is 0xc or 0x80000018 or 0x8000001f or 0x20 or 0x80000023) {
                if(size < 24) throw new InvalidDataException("Short Mach-O dylib command");
                uint nameOffset = U32(cursor + 8);
                if(nameOffset < 24 || nameOffset >= size) throw new InvalidDataException("Invalid Mach-O dylib name offset");
                var name = Slice(cursor + nameOffset, size - nameOffset);
                int nameSize = name.IndexOf((byte)0);
                if(nameSize <= 0) throw new InvalidDataException("Invalid Mach-O dylib name");
                libraries.Add(System.Text.Encoding.UTF8.GetString(name[..nameSize]));
            }
            cursor += size;
        }
        if(cursor != end) throw new InvalidDataException("Mach-O load command size/count mismatch");
        if(dyldInfo is { } info) {
            if(chainedFixups) throw new NotSupportedException("Mixed classic and chained Mach-O fixups");
            HasClassicFixups=true;
            DecodeFixups(Slice(U32(info+8),U32(info+12)).ToArray(),false,false);
            DecodeFixups(Slice(U32(info+16),U32(info+20)).ToArray(),true,false);
            if(U32(info+28)!=0) throw new NotSupportedException("Mach-O weak bindings require symbol coalescing");
            DecodeFixups(Slice(U32(info+32),U32(info+36)).ToArray(),true,true);
        }
        if(entryOffset is { } fileEntry) {
            var matches = segments.Where(s => fileEntry >= s.Offset && fileEntry - s.Offset < s.FileSize).ToArray();
            if(matches.Length != 1) throw new InvalidDataException("Ambiguous/unmapped Mach-O entry point");
            Entry = Add(matches[0].Address, fileEntry - matches[0].Offset);
        }
        if(startsCommand is { } starts) {
            if(textBase==null) throw new InvalidDataException("Mach-O function starts have no text base");
            var payload=Slice(U32(starts+8),U32(starts+12));
            ulong address=textBase.Value; int offset=0;
            bool terminated=payload.Length==0;
            while(offset<payload.Length) {
                ulong delta=0; int shift=0; byte item;
                do {
                    if(offset==payload.Length) throw new InvalidDataException("Truncated Mach-O function start delta");
                    item=payload[offset++];
                    if(shift>63 || (shift==63 && (item&0x7f)>1)) throw new InvalidDataException("Mach-O function start delta overflow");
                    delta|=(ulong)(item&0x7f)<<shift; shift+=7;
                } while((item&0x80)!=0);
                if(delta==0) {
                    terminated=true;
                    foreach(byte padding in payload[offset..]) if(padding!=0) throw new InvalidDataException("Nonzero Mach-O function start padding");
                    break;
                }
                address=Add(address,delta);
                ReadExecutable(address,1);
                functionStarts.Add(address);
            }
            if(!terminated) throw new InvalidDataException("Unterminated Mach-O function starts");
        }
        if(stubs.Count != 0) {
            if(symbols == null || indirect == null) throw new NotSupportedException("Mach-O stubs require symbol and indirect-symbol tables");
            ulong symOffset = U32(symbols.Value + 8), symCount = U32(symbols.Value + 12);
            ulong strOffset = U32(symbols.Value + 16), strSize = U32(symbols.Value + 20);
            ulong indOffset = U32(indirect.Value + 56), indCount = U32(indirect.Value + 60);
            Slice(symOffset, symCount * 16); var strings = Slice(strOffset, strSize);
            Slice(indOffset, indCount * 4);
            foreach(var stub in stubs) {
                if(stub.Stride == 0 || stub.Size % stub.Stride != 0 || stub.First > indCount ||
                   stub.Size / stub.Stride > indCount - stub.First) throw new InvalidDataException("Invalid Mach-O stub table extent");
                for(ulong i = 0; i < stub.Size / stub.Stride; i++) {
                    uint symbolIndex = U32(indOffset + (stub.First + i) * 4);
                    if(symbolIndex >= symCount) throw new NotSupportedException("Local/absolute or invalid Mach-O stub symbol");
                    ulong symbol = symOffset + symbolIndex * 16UL;
                    uint strIndex = U32(symbol);
                    if(strIndex >= strSize) throw new InvalidDataException("Mach-O symbol string offset");
                    // N_UNDF|N_EXT: an actual external import, not a guessed name.
                    if(Slice(symbol + 4, 1)[0] != 1 || U64(symbol + 8) != 0)
                        throw new NotSupportedException("Mach-O stub is not an undefined external symbol");
                    var nameBytes = strings[(int)strIndex..];
                    int length = nameBytes.IndexOf((byte)0);
                    if(length <= 0) throw new InvalidDataException("Unterminated/empty Mach-O symbol name");
                    string name = System.Text.Encoding.UTF8.GetString(nameBytes[..length]);
                    ulong address = Add(stub.Address, i * stub.Stride);
                    ulong? pointerSlot=null;
                    if(Architecture == Architecture.X86_64) {
                        var code = ReadExecutable(address, checked((int)stub.Stride));
                        if(stub.Stride != 6 || code.Length != 6 || code[0] != 0xff || code[1] != 0x25)
                            throw new NotSupportedException("Mach-O import stub is not the supported RIP-relative indirect jump");
                        int displacement = BinaryPrimitives.ReadInt32LittleEndian(code[2..]);
                        ulong slot = unchecked(address + 6 + (ulong)(long)displacement);
                        pointerSlot=slot;
                        var table = pointers.SingleOrDefault(p => slot >= p.Address && slot - p.Address < p.Size);
                        if(table.Size == 0 || (slot - table.Address) % 8 != 0 || table.Size % 8 != 0)
                            throw new NotSupportedException("Mach-O stub does not target a known import pointer slot");
                        ulong pointerIndex = table.First + (slot - table.Address) / 8;
                        if(pointerIndex >= indCount || U32(indOffset + pointerIndex * 4) != symbolIndex)
                            throw new InvalidDataException("Mach-O stub/pointer symbol binding mismatch");
                    }
                    if(!imports.TryAdd(address, name))
                        throw new InvalidDataException("Duplicate Mach-O import address");
                    int ordinal = U16(symbol + 6) >> 8;
                    importLibraries[address] = ordinal > 0 && ordinal <= libraries.Count ? libraries[ordinal - 1] : "";
                    if(HasClassicFixups && pointerSlot is { } boundSlot &&
                       (!bindings.TryGetValue(boundSlot,out var binding) || binding.Symbol!=name || binding.Library!=importLibraries[address]))
                        throw new InvalidDataException("Mach-O stub/dyld binding mismatch");
                }
            }
        }

        void DecodeFixups(byte[] bytes,bool binding,bool lazy) {
            int cursor=0,segment=-1,ordinal=0,type=binding ? 1 : 0;
            ulong offset=0; string? symbol=null;
            bool terminated=bytes.Length==0;
            ulong Uleb() {
                ulong result=0; int shift=0;
                while(true) {
                    if(cursor>=bytes.Length) throw new InvalidDataException("Truncated Mach-O fixup ULEB128");
                    byte item=bytes[cursor++];
                    if(shift>63 || shift==63 && (item&127)>1) throw new InvalidDataException("Mach-O fixup ULEB128 overflow");
                    result|=(ulong)(item&127)<<shift;
                    if((item&128)==0) return result;
                    shift+=7;
                }
            }
            void Fix() {
                if(type!=1 || segment<0 || segment>=segments.Count) throw new NotSupportedException("Unsupported Mach-O fixup type/segment");
                var seg=segments[segment];
                if(offset>seg.FileSize || 8>seg.FileSize-offset) throw new InvalidDataException("Mach-O fixup outside file-backed segment");
                ulong address=Add(seg.Address,offset);
                var region=regions.SingleOrDefault(r=>address>=r.Address && address-r.Address<r.Size && 8<=r.Size-(address-r.Address));
                if(region==null || region.Executable) throw new NotSupportedException("Mach-O fixup outside a data section");
                if(binding) {
                    if(symbol==null || ordinal<=0 || ordinal>libraries.Count) throw new NotSupportedException("Mach-O binding requires a named dylib symbol");
                    if(!bindings.TryAdd(address,new(symbol,libraries[ordinal-1]))) throw new InvalidDataException("Duplicate Mach-O binding");
                } else if(!rebases.TryAdd(address,U64(Add(seg.Offset,offset)))) throw new InvalidDataException("Duplicate Mach-O rebase");
            }
            while(cursor<bytes.Length) {
                byte code=bytes[cursor++]; int op=code&0xf0,imm=code&15;
                terminated=op==0;
                if(op==0) {
                    if(lazy) { segment=-1; ordinal=0; symbol=null; type=1; offset=0; continue; }
                    if(bytes.AsSpan(cursor).IndexOfAnyExcept((byte)0)>=0) throw new InvalidDataException("Nonzero Mach-O fixup padding");
                    return;
                }
                if(!binding) switch(op) {
                    case 0x10: type=imm; break;
                    case 0x20: segment=imm; offset=Uleb(); break;
                    case 0x30: offset=Add(offset,Uleb()); break;
                    case 0x40: offset=Add(offset,(ulong)imm*8); break;
                    case 0x50:
                    case 0x60: {
                        ulong count=op==0x50 ? (ulong)imm : Uleb();
                        if(count>1_000_000) throw new NotSupportedException("Mach-O fixup count limit");
                        for(ulong i=0;i<count;i++) { Fix(); offset=Add(offset,8); }
                        break;
                    }
                    case 0x70: Fix(); offset=Add(offset,Add(8,Uleb())); break;
                    case 0x80: {
                        ulong count=Uleb(),skip=Uleb();
                        if(count>1_000_000) throw new NotSupportedException("Mach-O fixup count limit");
                        for(ulong i=0;i<count;i++) { Fix(); offset=Add(offset,Add(8,skip)); }
                        break;
                    }
                    default: throw new NotSupportedException("Unsupported Mach-O rebase opcode");
                } else switch(op) {
                    case 0x10: ordinal=imm; break;
                    case 0x20: ordinal=checked((int)Uleb()); break;
                    case 0x30: throw new NotSupportedException("Mach-O special dylib ordinals");
                    case 0x40: {
                        if(imm!=0) throw new NotSupportedException("Mach-O weak/nonstandard binding flags");
                        int start=cursor;
                        while(cursor<bytes.Length && bytes[cursor]!=0) cursor++;
                        if(cursor==bytes.Length || cursor==start) throw new InvalidDataException("Invalid Mach-O binding symbol");
                        symbol=System.Text.Encoding.UTF8.GetString(bytes,start,cursor-start); cursor++; break;
                    }
                    case 0x50: type=imm; break;
                    case 0x60:
                        // Zero has the same one-byte representation in SLEB128.
                        if(cursor>=bytes.Length || bytes[cursor++]!=0) throw new NotSupportedException("Mach-O nonzero binding addend");
                        break;
                    case 0x70: segment=imm; offset=Uleb(); break;
                    case 0x80: offset=Add(offset,Uleb()); break;
                    case 0x90: Fix(); offset=Add(offset,8); break;
                    case 0xa0: Fix(); offset=Add(offset,Add(8,Uleb())); break;
                    case 0xb0: Fix(); offset=Add(offset,8+(ulong)imm*8); break;
                    case 0xc0: {
                        ulong count=Uleb(),skip=Uleb();
                        if(count>1_000_000) throw new NotSupportedException("Mach-O fixup count limit");
                        for(ulong i=0;i<count;i++) { Fix(); offset=Add(offset,Add(8,skip)); }
                        break;
                    }
                    default: throw new NotSupportedException("Unsupported Mach-O binding opcode");
                }
            }
            if(!terminated) throw new InvalidDataException("Unterminated Mach-O fixup stream");
        }
    }
}
