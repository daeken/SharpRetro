using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NxTranslate;

public interface IBinaryWriter {
    void AddSegment(string name, ulong addr, ulong size, byte[] data, MemoryProtection prot);
    void AddSymbol(string name, ulong addr);
    void Write(string fn);
}

// ELF64 ET_DYN writer — Linux sibling of MachoWriter. Same AddSegment/AddSymbol/
// Write interface so Program.cs's Linux arm is a one-line swap.
//
// Layout (vaddr_slide = 0x10000, page = 4K):
//   file [0..H]        ELF header + program headers
//   file [H..0x10000]  .dynamic + .dynsym + .dynstr + .hash  (R PT_LOAD vaddr 0)
//   file [0x10000..]   user segments, p_offset chosen so ≡ p_vaddr (mod 4K)
//   PT_DYNAMIC → .dynamic's vaddr
//
// Only .dynsym is emitted (no static .symtab) — for v0 the only symbols that
// matter are `setup`/`runFrom` (what GameWrapper.cs dlsym's). Game-side debug
// symbols (171K of them in legoworlds) → later, non-loaded .symtab.
//
// glibc dlopen requirements satisfied: PT_LOAD ×N, PT_DYNAMIC, DT_{STRTAB,
// SYMTAB, STRSZ, SYMENT, HASH, NULL}. SysV DT_HASH (not GNU_HASH) for simplicity.

public unsafe class ElfWriter : IBinaryWriter {
    const ulong Slide = 0x10000;     // user segments' vaddrs offset by this
    const ulong Page = 0x1000;
    const ulong DynSegSize = Slide;  // header + dynamic-data segment

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Ehdr {
        public fixed byte Ident[16];
        public ushort Type, Machine;
        public uint Version;
        public ulong Entry, Phoff, Shoff;
        public uint Flags;
        public ushort Ehsize, Phentsize, Phnum, Shentsize, Shnum, Shstrndx;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Phdr {
        public uint Type, Flags;
        public ulong Offset, Vaddr, Paddr, Filesz, Memsz, Align;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Sym {
        public uint Name;
        public byte Info, Other;
        public ushort Shndx;
        public ulong Value, Size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Dyn {
        public long Tag;
        public ulong Val;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Shdr {
        public uint Name, Type;
        public ulong Flags, Addr, Offset, Size;
        public uint Link, Info;
        public ulong Addralign, Entsize;
    }

    public readonly List<(string Name, ulong Start, ulong Size, byte[] Data, MemoryProtection Prot)> Segments = [];
    public readonly List<(string Name, ulong Addr)> Symbols = [];

    public void AddSegment(string name, ulong addr, ulong size, byte[] data, MemoryProtection prot) =>
        Segments.Add((name, addr, size, data, prot));
    public void AddSymbol(string name, ulong addr) =>
        Symbols.Add((name, addr));

    static uint ElfHash(string s) {
        uint h = 0;
        foreach(var c in s) {
            h = (h << 4) + (byte) c;
            var g = h & 0xf0000000;
            if(g != 0) h ^= g >> 24;
            h &= ~g;
        }
        return h;
    }

    static uint PFlags(MemoryProtection p) =>
        (p.HasFlag(MemoryProtection.Read) ? 4u : 0) |
        (p.HasFlag(MemoryProtection.Write) ? 2u : 0) |
        (p.HasFlag(MemoryProtection.Execute) ? 1u : 0);

    public void Write(string fn) {
        using var fp = File.Open(fn, FileMode.Create);
        void W<T>(T v) where T : unmanaged {
            Span<byte> b = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(b, v);
            fp.Write(b);
        }
        ulong Align(ulong x, ulong a) => (x + a - 1) & ~(a - 1);

        // — Build dynamic-data blob (will live at vaddr 0 + dynDataOff, file same) —
        // Order: .hash, .dynsym, .dynstr, .dynamic. Sizes computed first.

        // .dynsym: index 0 = STN_UNDEF (null sym), then our symbols.
        var nsym = Symbols.Count + 1;
        var symBytes = nsym * Marshal.SizeOf<Sym>();

        // .dynstr: \0 + each name + \0
        var strtab = new List<byte> { 0 };
        var nameOff = new uint[Symbols.Count];
        for(var i = 0; i < Symbols.Count; i++) {
            nameOff[i] = (uint) strtab.Count;
            strtab.AddRange(Encoding.ASCII.GetBytes(Symbols[i].Name));
            strtab.Add(0);
        }
        while(strtab.Count % 8 != 0) strtab.Add(0);

        // .hash: nbucket=1, nchain=nsym, bucket[1], chain[nsym]. All u32.
        var hashWords = 2 + 1 + nsym;
        var hashBytes = hashWords * 4;

        // .dynamic: 7 entries (STRTAB, SYMTAB, STRSZ, SYMENT, HASH, SONAME?, NULL)
        var ndyn = 7;
        var dynBytes = ndyn * Marshal.SizeOf<Dyn>();

        // Lay them out after the program headers, all within [0, DynSegSize).
        var ehdrSz = (ulong) Marshal.SizeOf<Ehdr>();
        var phdrSz = (ulong) Marshal.SizeOf<Phdr>();
        // PT_LOADs: 1 (dyn-data) + Segments.Count; + PT_DYNAMIC + PT_GNU_STACK
        var nphdr = (ulong)(1 + Segments.Count + 2);
        var phOff = ehdrSz;
        var dynDataOff = Align(phOff + nphdr * phdrSz, 16);

        var hashOff = dynDataOff;
        var symOff = hashOff + (ulong) hashBytes;
        var strOff = symOff + (ulong) symBytes;
        var dynOff = strOff + (ulong) strtab.Count;
        var dynDataEnd = dynOff + (ulong) dynBytes;
        Debug.Assert(dynDataEnd <= DynSegSize,
            $"dynamic data ({dynDataEnd}) overflows header segment ({DynSegSize}); too many .dynsym entries?");

        // — Assign file offsets to user segments —
        // Constraint: p_offset ≡ p_vaddr (mod Page). vaddrs come in 16K-aligned
        // (per NxTranslate's own asserts) so low 12 bits = 0; we just keep the
        // running file cursor 4K-aligned.
        var segOff = new ulong[Segments.Count];
        var cursor = DynSegSize;
        for(var i = 0; i < Segments.Count; i++) {
            var (_, start, size, data, _) = Segments[i];
            var va = start + Slide;
            // bump cursor until cursor ≡ va (mod Page)
            cursor = Align(cursor, Page);
            var miss = (va - cursor) & (Page - 1);
            cursor += miss;
            segOff[i] = cursor;
            cursor += (ulong) data.Length;       // filesz only; memsz may be larger (bss)
        }

        // — ELF header —
        var eh = new Ehdr {
            Type = 3,            // ET_DYN
            Machine = 0xb7,      // EM_AARCH64
            Version = 1,
            Entry = 0,
            Phoff = phOff,
            Shoff = 0,
            Flags = 0,
            Ehsize = (ushort) ehdrSz,
            Phentsize = (ushort) phdrSz,
            Phnum = (ushort) nphdr,
            Shentsize = 0, Shnum = 0, Shstrndx = 0,
        };
        // e_ident
        eh.Ident[0] = 0x7f; eh.Ident[1] = (byte) 'E'; eh.Ident[2] = (byte) 'L'; eh.Ident[3] = (byte) 'F';
        eh.Ident[4] = 2;     // ELFCLASS64
        eh.Ident[5] = 1;     // ELFDATA2LSB
        eh.Ident[6] = 1;     // EV_CURRENT
        eh.Ident[7] = 0;     // ELFOSABI_NONE
        W(eh);

        // — Program headers —
        // PT_LOAD #0: header + dynamic data, R, vaddr 0..DynSegSize, file 0..DynSegSize
        W(new Phdr {
            Type = 1, Flags = 4, // PT_LOAD, PF_R
            Offset = 0, Vaddr = 0, Paddr = 0,
            Filesz = dynDataEnd, Memsz = DynSegSize, Align = Page,
        });
        // PT_LOAD per user segment
        for(var i = 0; i < Segments.Count; i++) {
            var (_, start, size, data, prot) = Segments[i];
            W(new Phdr {
                Type = 1, Flags = PFlags(prot),
                Offset = segOff[i], Vaddr = start + Slide, Paddr = start + Slide,
                Filesz = (ulong) data.Length, Memsz = size, Align = Page,
            });
        }
        // PT_DYNAMIC
        W(new Phdr {
            Type = 2, Flags = 4,
            Offset = dynOff, Vaddr = dynOff, Paddr = dynOff,
            Filesz = (ulong) dynBytes, Memsz = (ulong) dynBytes, Align = 8,
        });
        // PT_GNU_STACK (RW, no-exec) — glibc wants this or it marks stack exec.
        W(new Phdr { Type = 0x6474e551, Flags = 6, Align = 0x10 });

        // — .hash (SysV) —
        fp.Seek((long) hashOff, SeekOrigin.Begin);
        W(1u);                 // nbucket
        W((uint) nsym);        // nchain
        // bucket[0]: head of the single chain. We chain syms 1..nsym-1; bucket
        // points at the FIRST defined sym (index 1) if any, else 0.
        // Build chain so dlsym walks all defined syms regardless of hash:
        // bucket[0] = 1; chain[i] = i+1 (i in 1..nsym-2); chain[nsym-1] = 0; chain[0]=0.
        // With nbucket=1, every name hashes to bucket 0 → walks the whole chain.
        W(nsym > 1 ? 1u : 0u);
        for(var i = 0; i < nsym; i++)
            W(i == 0 ? 0u : (i < nsym - 1 ? (uint)(i + 1) : 0u));

        // — .dynsym —
        // Shndx must reference a REAL section index. SHN_ABS makes glibc skip
        // the load-base relocation on dlsym (verified at-bytes: deref on the
        // returned addr SIGSEGVs). Section indices: 0=null, 1=.hash, 2=.dynsym,
        // 3=.dynstr, 4=.dynamic, 5..=user segments (.text/.rodata/.data per
        // module), N-1=.shstrtab.
        ushort SecIdx(ulong vaddr) {
            for(var s = 0; s < Segments.Count; s++) {
                var (_, st, sz, _, _) = Segments[s];
                if(vaddr >= st + Slide && vaddr < st + Slide + sz)
                    return (ushort)(5 + s);
            }
            // Should not happen for setup/runFrom (both in glue segment).
            return 0xfff1;
        }
        fp.Seek((long) symOff, SeekOrigin.Begin);
        W(new Sym()); // STN_UNDEF
        for(var i = 0; i < Symbols.Count; i++) {
            var va = Symbols[i].Addr + Slide;
            W(new Sym {
                Name = nameOff[i],
                Info = 0x12,                       // STB_GLOBAL<<4 | STT_FUNC
                Other = 0,
                Shndx = SecIdx(va),
                Value = va,
                Size = 0,
            });
        }

        // — .dynstr —
        fp.Seek((long) strOff, SeekOrigin.Begin);
        fp.Write(strtab.ToArray());

        // — .dynamic —
        fp.Seek((long) dynOff, SeekOrigin.Begin);
        W(new Dyn { Tag = 4,  Val = hashOff });          // DT_HASH
        W(new Dyn { Tag = 5,  Val = strOff });           // DT_STRTAB
        W(new Dyn { Tag = 6,  Val = symOff });           // DT_SYMTAB
        W(new Dyn { Tag = 10, Val = (ulong) strtab.Count }); // DT_STRSZ
        W(new Dyn { Tag = 11, Val = (ulong) Marshal.SizeOf<Sym>() }); // DT_SYMENT
        W(new Dyn { Tag = 14, Val = nameOff.Length > 0 ? nameOff[0] : 0 }); // DT_SONAME → first sym name (harmless)
        W(new Dyn { Tag = 0,  Val = 0 });                // DT_NULL

        // — User segment data —
        for(var i = 0; i < Segments.Count; i++) {
            fp.Seek((long) segOff[i], SeekOrigin.Begin);
            fp.Write(Segments[i].Data);
        }
        // — Section headers (at end of file, non-loaded) —
        // glibc dlsym needs Shndx to point at a real section so it relocates
        // st_value by the load base. Minimal table: null + .hash/.dynsym/
        // .dynstr/.dynamic + one per user segment + .shstrtab.
        var shstrtab = new List<byte> { 0 };
        uint ShStr(string s) {
            var o = (uint) shstrtab.Count;
            shstrtab.AddRange(Encoding.ASCII.GetBytes(s));
            shstrtab.Add(0);
            return o;
        }
        var shdrs = new List<Shdr> {
            new(), // SHN_UNDEF
            new() { Name = ShStr(".hash"), Type = 5, Flags = 2, Addr = hashOff,
                    Offset = hashOff, Size = (ulong) hashBytes, Link = 2, Addralign = 4, Entsize = 4 },
            new() { Name = ShStr(".dynsym"), Type = 11, Flags = 2, Addr = symOff,
                    Offset = symOff, Size = (ulong) symBytes, Link = 3, Info = 1,
                    Addralign = 8, Entsize = (ulong) Marshal.SizeOf<Sym>() },
            new() { Name = ShStr(".dynstr"), Type = 3, Flags = 2, Addr = strOff,
                    Offset = strOff, Size = (ulong) strtab.Count, Addralign = 1 },
            new() { Name = ShStr(".dynamic"), Type = 6, Flags = 3, Addr = dynOff,
                    Offset = dynOff, Size = (ulong) dynBytes, Link = 3,
                    Addralign = 8, Entsize = (ulong) Marshal.SizeOf<Dyn>() },
        };
        for(var i = 0; i < Segments.Count; i++) {
            var (name, start, size, data, prot) = Segments[i];
            var sname = $".seg{i}_{name.TrimStart('_').ToLowerInvariant().Replace(" ", "")}";
            shdrs.Add(new() {
                Name = ShStr(sname),
                Type = 1, // PROGBITS
                Flags = 2ul | (prot.HasFlag(MemoryProtection.Execute) ? 4ul : 0)
                            | (prot.HasFlag(MemoryProtection.Write) ? 1ul : 0),
                Addr = start + Slide, Offset = segOff[i],
                Size = (ulong) data.Length, Addralign = Page,
            });
        }
        // .shstrtab itself (after we know its name's offset)
        var shstrNameOff = ShStr(".shstrtab");
        var shstrtabFileOff = Align(cursor, 8);
        shdrs.Add(new() {
            Name = shstrNameOff, Type = 3,
            Offset = shstrtabFileOff, Size = (ulong) shstrtab.Count, Addralign = 1,
        });
        var shoff = Align(shstrtabFileOff + (ulong) shstrtab.Count, 8);

        fp.Seek((long) shstrtabFileOff, SeekOrigin.Begin);
        fp.Write(shstrtab.ToArray());
        fp.Seek((long) shoff, SeekOrigin.Begin);
        foreach(var s in shdrs) W(s);

        // — Patch ELF header with section info —
        fp.Seek(0, SeekOrigin.Begin);
        eh.Shoff = shoff;
        eh.Shentsize = (ushort) Marshal.SizeOf<Shdr>();
        eh.Shnum = (ushort) shdrs.Count;
        eh.Shstrndx = (ushort)(shdrs.Count - 1);
        W(eh);
    }
}
