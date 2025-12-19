using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NxTranslate;

[Flags]
public enum MemoryProtection {
    Read = 1,
    Write = 2,
    Execute = 4,
}

public unsafe class MachoWriter {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachHeader {
        public uint Magic, CpuType, CpuSubtype, FileType, NumLcmds, SizeLcmds, Flags, Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachLoadSegment {
        public uint Command, Size;
        public fixed byte Name[16];
        public ulong Address, AddressSize, FileOffset, FileSize;
        public uint MaxProt, InitProt, NumSect, Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachLoadSegmentPlusSection {
        public uint Command, Size;
        public fixed byte Name[16];
        public ulong Address, AddressSize, FileOffset, FileSize;
        public uint MaxProt, InitProt, NumSect, Flags;
        
        public fixed byte SectName[16], SegName[16];
        public ulong SectAddress, SectAddressSize;
        public uint SectFileOffset, SectAlignment, SectRelocOffset, SectNumRelocs, SectFlag;
        public uint SectReserved1, SectReserved2, SectReserved3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachLinkEditSymtab {
        public uint Command, Size, SymbolOff, NumSymbol, StringOff, StringSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachDysymtab {
        public uint Command, Size;
        public uint LocalIndex, LocalCount;
        public uint ExternalIndex, ExternalCount;
        public uint UndefIndex, UndefCount;
        public uint TocOff, TocCount;
        public uint ModTabOff, ModTabCount;
        public uint ExtRefSymOff, ExtRefSymCount;
        public uint IndirectSymOff, IndirectSymCount;
        public uint ExtRelOff, ExtRelCount;
        public uint LocalRelOff, LocalRelCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachSymbol {
        public uint NameOff;
        public byte Type, SectNum;
        public ushort DataInfo;
        public ulong Address;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachDylibCommand {
        public uint Command, Size, NameOff, Timestamp, CurrentVersion, CompatibilityVersion;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachCodeSignature {
        public uint Command, Size, DataOff, DataSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MachUuid {
        public uint Command, Size;
        public ulong A, B;
    }

    public readonly List<(string Name, ulong Start, ulong Size, byte[] Data, MemoryProtection Protection)>
        Segments = [];
    public readonly List<(byte[] Name, ulong Addr)> Symbols = [];
    
    public void AddSegment(string name, ulong addr, ulong size, byte[] data, MemoryProtection protection) =>
        Segments.Add((name, addr, size, data, protection));
    public void AddSymbol(string name, ulong addr) =>
        Symbols.Add((Encoding.ASCII.GetBytes(name), addr));

    public void Write(string fn) {
        using var fp = File.Open(fn, FileMode.Create);
        void Write<T>(T data) where T : unmanaged {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(buffer, data);
            fp.Write(buffer);
        }

        fp.Seek(Marshal.SizeOf<MachHeader>(), SeekOrigin.Begin); // Skip over header to start
        
        var linkEditSize = 
            Marshal.SizeOf<MachSymbol>() * Symbols.Count + 
            Symbols.Select(x => x.Name.Length + 1).Sum() + 1 + 
            0x200000; // space for code signature
        while(linkEditSize % 16 != 0)
            linkEditSize++;

        var binStart = 0x100_000;
        var fileOff = (uint) binStart;
        var lcmd = new MachLoadSegment {
            Command = 0x00000019,
            Size = (uint) Marshal.SizeOf<MachLoadSegment>(),
            Address = 0, AddressSize = (uint) binStart, FileOffset = 0, FileSize = (uint) binStart,
            MaxProt = 5, InitProt = 5, NumSect = 0, Flags = 0,
        };
        "__TEXT"u8.CopyTo(new Span<byte>(lcmd.Name, 16));
        Write(lcmd);
        var binSlide = 0x1_0000_0000UL;
        foreach(var (name, start, size, _, prot) in Segments) {
            var cmd = new MachLoadSegmentPlusSection {
                Command = 0x00000019,
                Size = (uint) Marshal.SizeOf<MachLoadSegmentPlusSection>(),
                Address = start + binSlide, AddressSize = size, FileOffset = fileOff, FileSize = size,
                MaxProt = (uint) prot, InitProt = (uint) prot, NumSect = 1, Flags = 0, 
                
                SectAddress = start + binSlide, SectAddressSize = size, SectFileOffset = fileOff, 
                SectAlignment = 14, // 16kb-aligned 
                SectRelocOffset = 0, SectNumRelocs = 0,
                SectFlag = prot.HasFlag(MemoryProtection.Execute) ? 0x80000400 : 0, 
                SectReserved1 = 0, SectReserved2 = 0, SectReserved3 = 0,
            };
            var nameBytes = prot .HasFlag(MemoryProtection.Execute)
                ? "__STEXT"u8.ToArray()
                : "__SDATA"u8.ToArray();
            nameBytes.CopyTo(new Span<byte>(cmd.Name, 16));
            nameBytes.CopyTo(new Span<byte>(cmd.SegName, 16));
            nameBytes = Encoding.ASCII.GetBytes(name);
            Debug.Assert(nameBytes.Length <= 16);
            nameBytes.CopyTo(new Span<byte>(cmd.SectName, 16));
            Write(cmd);
            fileOff += (uint) size;
        }
        var binEnd = fileOff;
        lcmd = new MachLoadSegment {
            Command = 0x00000019,
            Size = (uint) Marshal.SizeOf<MachLoadSegment>(),
            Address = 0x2_0000_0000, AddressSize = (uint) linkEditSize, FileOffset = binEnd, FileSize = (uint) linkEditSize,
            MaxProt = 1, InitProt = 1, NumSect = 0, Flags = 0,
        };
        "__LINKEDIT"u8.CopyTo(new Span<byte>(lcmd.Name, 16));
        Write(lcmd);
        
        Write(new MachDylibCommand {
            Command = 0xD,
            Size = 24 + 16,
            NameOff = 24,
        });
        fp.Write("libfake.dylib\0\0\0"u8);
        Write(new MachDylibCommand {
            Command = 0xC,
            Size = 24 + 32,
            NameOff = 24,
        });
        fp.Write("/usr/lib/libSystem.B.dylib\0\0\0\0\0\0"u8);

        var symOff = binEnd;
        var strOff = symOff + 16 * (uint) Symbols.Count;
        Write(new MachLinkEditSymtab {
            Command = 2,
            Size = 24,
            SymbolOff = symOff,
            NumSymbol = (uint) Symbols.Count,
            StringOff = strOff,
            StringSize = (uint) Symbols.Select(x => x.Name.Length + 1).Sum() + 1,
        });
        
        Write(new MachDysymtab {
            Command = 0xB,
            Size = (uint) Marshal.SizeOf<MachDysymtab>(),
            ExternalIndex = 0,
            ExternalCount = (uint) Symbols.Count,
        });
        
        Write(new MachUuid {
            Command = 0x1B,
            Size = 24,
            A = 0xDEADBEEF,
            B = 0xCAFEBABE,
        });
        
        Write(new MachCodeSignature {
            Command = 0x1D,
            Size = 16,
            DataOff = (uint) (binEnd + linkEditSize - 0x200000),
            DataSize = 0x200000,
        });
        
        var header = new MachHeader {
            Magic = 0xFEEDFACF,
            CpuType = 0x100000C,
            CpuSubtype = 0,
            FileType = 6,
            NumLcmds = (uint) (8 + Segments.Count),
            SizeLcmds = (uint) (
                Marshal.SizeOf<MachLoadSegment>() * 2 +
                Marshal.SizeOf<MachLoadSegmentPlusSection>() * Segments.Count + 
                Marshal.SizeOf<MachLinkEditSymtab>() + 
                Marshal.SizeOf<MachDysymtab>() +
                Marshal.SizeOf<MachDylibCommand>() + 16 + 
                Marshal.SizeOf<MachDylibCommand>() + 26 + 6 + 
                Marshal.SizeOf<MachCodeSignature>() + 
                Marshal.SizeOf<MachUuid>()
            ),
            Flags = 0x00100085,
            Reserved = 0,
        };
        fp.Seek(0, SeekOrigin.Begin);
        Write(header);
        
        fp.Seek(binStart,  SeekOrigin.Begin);
        foreach(var (_, _, size, data, _) in Segments) {
            var tdata = data;
            if(data.Length < (int) size) {
                var ndata = new byte[size];
                data.CopyTo(ndata, 0);
                tdata = ndata;
            }
            fp.Write(tdata);
        }
        
        fp.Seek(binEnd, SeekOrigin.Begin); // We should already be right here...
        strOff = 1;
        foreach(var (name, addr) in Symbols) {
            Write(new MachSymbol {
                NameOff = strOff,
                Type = 0b000_0_111_1, // defined external
                SectNum = (byte) Segments.Index().First(x => x.Item.Start <= addr && addr <= x.Item.Start + x.Item.Size).Index,
                DataInfo = 0b0000_0000_0001_0000,
                Address = addr + binSlide,
            });
            strOff += (uint) name.Length + 1;
        }
        fp.WriteByte(0);
        foreach(var (name, _) in Symbols) {
            fp.Write(name);
            fp.WriteByte(0);
        }
        fp.Seek(binEnd + linkEditSize, SeekOrigin.Begin);
        fp.WriteByte(0);
    }
}