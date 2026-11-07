namespace SharpRetro.Decompiler;

public sealed record PeExport(ulong Address,uint Ordinal,string? Forwarder);
public sealed record PeRuntimeFunction(ulong Begin,ulong End,ulong Unwind);
public sealed record PeRelocation(ulong Address,int Type);

public sealed partial class BinaryImage {
    public ulong ImageBase { get; private set; }
    public uint PeImageSize { get; private set; }
    public bool PeHasTls { get; private set; }
    public IReadOnlyDictionary<string,PeExport> Exports => exports;
    public IReadOnlyDictionary<ulong,string> PeImportSlots => peImportSlots;
    public IReadOnlyList<PeRuntimeFunction> PeRuntimeFunctions => peRuntimeFunctions;
    public IReadOnlyList<PeRelocation> PeRelocations => peRelocations;
    readonly Dictionary<string,PeExport> exports=[];
    readonly Dictionary<ulong,string> peImportSlots=[];
    readonly List<PeRuntimeFunction> peRuntimeFunctions=[];
    readonly List<PeRelocation> peRelocations=[];
    readonly Dictionary<ulong,uint> peSectionFlags=[];
    public bool IsPeReadable(ImageRegion region) => peSectionFlags.TryGetValue(region.Address,out var flags) && (flags&0x40000000)!=0;
    uint peHeaders;

    ulong PeFile(ulong rva,ulong bytes) {
        if(rva<peHeaders && bytes<=peHeaders-rva) { Slice(rva,bytes); return rva; }
        ulong va=Add(ImageBase,rva);
        var region=regions.SingleOrDefault(r=>va>=r.Address && va-r.Address<=r.FileSize && bytes<=r.FileSize-(va-r.Address))
            ?? throw new InvalidDataException($"PE RVA 0x{rva:x} is not file-backed");
        return Add(region.FileOffset,va-region.Address);
    }
    string PeString(ulong rva,ulong maximum=4096) {
        var text=new System.Text.StringBuilder();
        for(ulong i=0;i<Math.Min(maximum,4096);i++) {
            byte b=Slice(PeFile(Add(rva,(ulong)i),1),1)[0];
            if(b==0) return text.ToString();
            if(b>127) throw new NotSupportedException("Non-ASCII PE metadata name");
            text.Append((char)b);
        }
        throw new InvalidDataException("Unterminated PE metadata string");
    }
    void ParsePeDirectories(ulong opt,int optSize,bool wide,ulong imageBase) {
        ImageBase=imageBase; PeImageSize=U32(opt+56); peHeaders=U32(opt+60);
        uint count=U32(opt+(wide ? 108UL : 92UL));
        if(PeImageSize!=0 || peHeaders!=0 || count!=0) {
            if(PeImageSize==0 || peHeaders==0 || peHeaders>PeImageSize || peHeaders>(ulong)data.Length)
                throw new InvalidDataException("Invalid PE image/header bounds");
            ulong sectionEnd=opt+(ulong)optSize+40UL*(ulong)regions.Count;
            if(sectionEnd>peHeaders || regions.Any(r=>r.Address<imageBase || r.Address-imageBase<peHeaders || r.Address-imageBase>PeImageSize || r.Size>PeImageSize-(r.Address-imageBase)))
                throw new InvalidDataException("PE section outside declared image/header bounds");
        }
        ulong first=wide ? 112UL : 96UL;
        if(count>16 || first+8UL*count>(ulong)optSize) throw new InvalidDataException("PE data directories exceed optional header");
        (uint Rva,uint Size) Directory(int index) {
            if(index>=count) return (0,0);
            uint rva=U32(opt+first+8UL*(ulong)index),size=U32(opt+first+8UL*(ulong)index+4);
            if((rva==0)!=(size==0)) throw new InvalidDataException("Incomplete PE data directory");
            return (rva,size);
        }
        var ex=Directory(0);
        if(ex.Size!=0) {
            if(ex.Size<40) throw new InvalidDataException("Short PE export directory");
            ulong file=PeFile(ex.Rva,40); uint ordinal=U32(file+16),nf=U32(file+20),nn=U32(file+24);
            uint funcs=U32(file+28),names=U32(file+32),ords=U32(file+36);
            if(nf>65536 || nn>65536) throw new NotSupportedException("PE export table limit");
            ulong ff=nf==0 ? 0 : PeFile(funcs,4UL*nf),nnf=nn==0 ? 0 : PeFile(names,4UL*nn),of=nn==0 ? 0 : PeFile(ords,2UL*nn);
            for(uint i=0;i<nn;i++) {
                string name=PeString(U32(nnf+4UL*i)); uint ix=U16(of+2UL*i);
                if(ix>=nf) throw new InvalidDataException("PE export ordinal outside function table");
                uint rva=U32(ff+4UL*ix); if(rva==0) throw new InvalidDataException("Named PE export is empty");
                string? forwarder=rva>=ex.Rva && (ulong)rva-ex.Rva<ex.Size ? PeString(rva,(ulong)ex.Rva+ex.Size-rva) : null;
                if(!exports.TryAdd(name,new(Add(imageBase,rva),checked(ordinal+ix),forwarder))) throw new InvalidDataException("Duplicate PE export name");
            }
        }
        var im=Directory(1);
        if(im.Size!=0) {
            bool terminated=false;
            for(ulong delta=0;delta+20<=im.Size;delta+=20) {
                ulong file=PeFile(im.Rva+delta,20); uint lookup=U32(file),name=U32(file+12),iat=U32(file+16);
                if(lookup==0 && name==0 && iat==0 && U32(file+4)==0 && U32(file+8)==0) { terminated=true; break; }
                if(name==0 || iat==0) throw new InvalidDataException("Incomplete PE import descriptor");
                string library=PeString(name); if(lookup==0) lookup=iat;
                bool ended=false;
                for(uint i=0;i<65536;i++) {
                    ulong step=(wide ? 8UL : 4UL)*i;
                    ulong f=PeFile(lookup+step,wide ? 8UL : 4UL); ulong value=wide ? U64(f) : U32(f);
                    if(value==0) { ended=true; break; }
                    ulong high=wide ? 1UL<<63 : 1UL<<31;
                    string symbol=(value&high)!=0 ? "@ordinal:"+(value&65535) : PeString(Add(value,2));
                    ulong slot=Add(imageBase,iat+step); _=PeFile(iat+step,wide ? 8UL : 4UL);
                    if(!peImportSlots.TryAdd(slot,symbol)) throw new InvalidDataException("Duplicate PE import slot");
                    importLibraries[slot]=library;
                }
                if(!ended) throw new InvalidDataException("Unterminated PE import lookup");
            }
            if(!terminated) throw new InvalidDataException("Unterminated PE import directory");
        }
        var rt=Directory(3);
        if(wide && Architecture==Architecture.X86_64 && rt.Size!=0) {
            if(rt.Size%12!=0) throw new InvalidDataException("Misaligned PE runtime function table");
            for(ulong delta=0;delta<rt.Size;delta+=12) {
                ulong f=PeFile(rt.Rva+delta,12); uint begin=U32(f),end=U32(f+4),unwind=U32(f+8);
                if(begin>=end) throw new InvalidDataException("Invalid PE runtime function range");
                _=PeFile(begin,(ulong)end-begin); _=PeFile(unwind,1);
                peRuntimeFunctions.Add(new(Add(imageBase,begin),Add(imageBase,end),Add(imageBase,unwind)));
            }
        }
        var reloc=Directory(5);
        for(ulong delta=0;delta<reloc.Size;) {
            if(reloc.Size-delta<8) throw new InvalidDataException("Truncated PE relocation block");
            ulong f=PeFile(reloc.Rva+delta,8); uint page=U32(f),bytes=U32(f+4);
            if(bytes<8 || bytes%2!=0 || bytes>reloc.Size-delta) throw new InvalidDataException("Invalid PE relocation block size");
            f=PeFile(reloc.Rva+delta,bytes);
            for(uint k=8;k<bytes;k+=2) {
                ushort item=U16(f+k); int type=item>>12; if(type==0) continue;
                ulong rva=Add(page,(ulong)(item&4095));
                _=PeFile(rva,type==10 ? 8UL : 4UL); peRelocations.Add(new(Add(imageBase,rva),type));
            }
            delta+=bytes;
        }
        PeHasTls=Directory(9).Size!=0;
    }

    public bool ResolvePeImportThunk(ulong address,out string symbol) {
        symbol="";
        if(Format!=BinaryFormat.Pe || Architecture!=Architecture.X86_64) return false;
        var region=regions.SingleOrDefault(r=>r.Executable && address>=r.Address && address-r.Address<r.FileSize && r.FileSize-(address-r.Address)>=6);
        if(region==null) return false;
        var bytes=Slice(region.FileOffset+address-region.Address,6);
        if(bytes[0]!=0xff || bytes[1]!=0x25) return false;
        long displacement=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(bytes[2..]);
        ulong slot=unchecked(address+6+(ulong)displacement);
        if(!peImportSlots.TryGetValue(slot,out var found)) return false;
        symbol=found; imports[address]=found; importLibraries[address]=importLibraries[slot]; return true;
    }
}
