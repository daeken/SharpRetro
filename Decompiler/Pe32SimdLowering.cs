using LiftIl;
namespace SharpRetro.Decompiler;

// Canonical legacy SSE/SSE2 forms. Descriptors preserve the original opcode,
// register indices and immediate; only the effective address is expressed in IL.
// Unsupported prefixes and forms stay with the ordinary (strict) lifter.
public static class Pe32SimdLowering {
    public static int MemoryWidth(uint descriptor) => (descriptor&255) switch {
        1 or 2 or 3 or 4 or 10 or 11 or 12 or 13 or 33 or 34 or 39 or 60 or 61 or 62 or 70 or 71 or 85 or 89 or 90 or 93 or 94 or 95 or 96 or 97 or 98=>64,
        23=>16,24 or 25 or 48 or 49 or 50 or 51 or 56 or 57 or 63 or 72 or 73 or 74 or 75 or 86 or 91 or 92=>32,_=>128 };
    public static bool Store(uint descriptor) => (descriptor&255) is 2 or 4 or 6 or 25 or 41 or 43 or 45 or 47 or 71 or 75 or 96 or 98;
    public static int GprDestination(uint descriptor) => (descriptor&255) switch {
        22 or 32 or 34 or 39 or 48 or 49=>(int)((descriptor>>8)&7),25=>(int)((descriptor>>11)&7),_=>-1 };
    public static bool TryLower(ReadOnlySpan<byte> bytes,out IlBlock body) {
        body=new IlBlock([]);int cursor=0,prefix=0;
        if(bytes.Length>0 && bytes[0] is 0x66 or 0xf2 or 0xf3) prefix=bytes[cursor++];
        if(bytes.Length<cursor+3 || bytes[cursor++]!=0x0f)return false;
        int opcode=bytes[cursor++],modrm=bytes[cursor++],mod=modrm>>6,reg=(modrm>>3)&7,rm=modrm&7;
        int operation=(prefix,opcode) switch {
            (0x66,0x12)=>1,(0x66,0x13)=>2,(0xf2,0x10)=>3,(0xf2,0x11)=>4,
            (0x66,0x28)=>5,(0x66,0x29)=>6,(0x66,0x54)=>7,(0x66,0x56)=>8,(0x66,0x57)=>9,
            (0xf2,0x59)=>10,(0xf2,0x58)=>11,(0xf2,0x5c)=>12,(0xf2,0x5e)=>13,
            (0x66,0x59)=>14,(0x66,0x58)=>15,(0x66,0x5c)=>16,(0x66,0x5e)=>17,
            (0x66,0x73) when reg==2=>18,(0x66,0x73) when reg==6=>19,
            (0x66,0xd3)=>20,(0x66,0xf3)=>21,(0x66,0xc5)=>22,(0x66,0xc4)=>23,
            (0x66,0x6e)=>24,(0x66,0x7e)=>25,(0x66,0x70)=>26,(0x66,0x14)=>27,
            (0x66,0xfb)=>28,(0x66,0xfa)=>29,(0x66,0x76)=>30,(0x66,0xee)=>31,(0x66,0xd7)=>32,
            (0xf3,0xe6)=>33,(0xf2,0x2d)=>34,(0,0x58)=>35,(0,0x5c)=>36,(0,0x59)=>37,(0,0x5e)=>38,
            (0xf2,0x2c)=>39,(0,0x10)=>40,(0,0x11)=>41,
            // Existing MOVAPS forms have an established checked implementation.
            (0xf3,0x6f)=>44,(0xf3,0x7f)=>45,(0x66,0x6f)=>46,(0x66,0x7f)=>47,
            (0xf3,0x2c)=>48,(0xf3,0x2d)=>49,
            (0xf3,0x53)=>50,(0xf3,0x52)=>51,(0,0x53)=>52,(0,0x52)=>53,
            (0,0x5d)=>54,(0,0x5f)=>55,(0xf3,0x5d)=>56,(0xf3,0x5f)=>57,
            (0x66,0x5d)=>58,(0x66,0x5f)=>59,(0xf2,0x5d)=>60,(0xf2,0x5f)=>61,
            (0xf2,0x51)=>62,(0xf3,0x51)=>63,(0x66,0x51)=>64,(0,0x51)=>65,
            (0x66,0x15)=>66,(0,0x14)=>67,(0,0x15)=>68,(0,0xc6)=>69,
            (0xf3,0x7e)=>70,(0x66,0xd6)=>71,(0xf2,0x2a)=>72,(0xf3,0x2a)=>73,
            (0xf3,0x10)=>74,(0xf3,0x11)=>75,(0x66,0x55)=>76,(0,0x55)=>77,
            (0x66,0xdb)=>78,(0x66,0xdf)=>79,(0x66,0xeb)=>80,(0x66,0xef)=>81,
            (0,0x54)=>82,(0,0x56)=>83,(0,0x57)=>84,
            (0xf2,0xc2)=>85,(0xf3,0xc2)=>86,(0x66,0xc2)=>87,(0,0xc2)=>88,
            (0x66,0x2e)=>89,(0x66,0x2f)=>90,(0,0x2e)=>91,(0,0x2f)=>92,
            (0,0x12)=>mod==3?93:95,(0,0x16)=>mod==3?94:97,(0,0x13)=>96,(0,0x17)=>98,
            (0x66,0xd4)=>99,(0x66,0xc6)=>100,
            _=>0 };
        if(operation==0)return false;
        // Preserve the existing full-register copy/unknownness and aligned
        // MOVDQA store paths; they already have strict-backend semantics.
        if(operation==47 || (mod==3 && (operation is 5 or 6 or 40 or 41 or 44 or 45 or 46)))return false;
        // The generic integer backend already preserves unknownness for these
        // zero idioms without requiring a native SIMD provider.
        if(mod==3 && reg==rm && operation is 81 or 84)return false;
        if((operation is 1 or 2 && mod==3) || (operation is 18 or 19 or 22 or 32 && mod!=3))return false;
        if(operation is 96 or 98 && mod==3)return false;
        Il address=new IlConst(IlType.U64,0);
        if(mod!=3) {
            Il offset=new IlConst(IlType.U32,0);bool stackSegment=false;
            Il Gpr(int index)=>new IlReadReg(IlType.U32,RegKind.X86,index);
            Il Add(Il a,Il b)=>new IlBin(IlType.U32,BinOp.Add,a,b);
            bool displacementOnly=mod==0 && rm==5;
            if(rm==4) {
                if(cursor>=bytes.Length)throw new NotSupportedException("Truncated SSE SIB");
                int sib=bytes[cursor++],basis=sib&7,index=(sib>>3)&7,scale=sib>>6;
                displacementOnly=mod==0 && basis==5;
                if(!displacementOnly){offset=Gpr(basis);stackSegment=basis is 4 or 5;}
                if(index!=4)offset=Add(offset,new IlBin(IlType.U32,BinOp.Shl,Gpr(index),new IlConst(IlType.U8,(uint)scale)));
            } else if(!displacementOnly){offset=Gpr(rm);stackSegment=rm==5;}
            int displacementBytes=displacementOnly || mod==2 ? 4:mod==1 ? 1:0;
            if(cursor+displacementBytes>bytes.Length)throw new NotSupportedException("Truncated SSE displacement");
            uint displacement=displacementBytes==1 ? unchecked((uint)(sbyte)bytes[cursor]):
                displacementBytes==4 ? System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes[cursor..]):0;
            cursor+=displacementBytes;
            if(displacementBytes!=0)offset=Add(offset,new IlConst(IlType.U32,displacement));
            address=new IlBin(IlType.U64,BinOp.Add,new IlReadReg(IlType.U64,RegKind.X86Seg,stackSegment?2:3),new IlCast(IlType.U64,CastKind.Zext,offset));
        }
        int immediate=0;
        if(operation is 18 or 19 or 22 or 23 or 26 or 69 or 85 or 86 or 87 or 88 or 100) {
            if(cursor>=bytes.Length)throw new NotSupportedException("Truncated SSE immediate");
            immediate=bytes[cursor++];
        }
        if(operation is >=85 and <=88 && immediate>7)
            throw new NotSupportedException("Reserved legacy SSE comparison predicate requires separate execution evidence");
        if(cursor!=bytes.Length)throw new NotSupportedException("SSE descriptor length differs from decoder");
        uint descriptor=(uint)(operation|(reg<<8)|(rm<<11)|(mod!=3?1<<14:0)|(immediate<<16));
        body=new IlBlock([new IlIntrin(IlType.V0,"pe32-simd",new IlConst(IlType.U32,descriptor),address)]);
        return true;
    }
}
