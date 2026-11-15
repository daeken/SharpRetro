using LiftIl;
namespace SharpRetro.Decompiler;
// Exact legacy forms omitted by the shared decoder/lifter. Length and address
// are derived from their bytes; prefixes outside these contracts remain errors.
public static class Pe32LegacyLowering {
    public static bool TryLower(ReadOnlySpan<byte> bytes,out IlBlock body,out int length,out string text) {
        body=new IlBlock([]);length=0;text="";
        if(bytes.Length==0)return false;
        if(bytes[0]==0xd7){body=new IlBlock([new IlIntrin(IlType.V0,"pe32-xlat")]);length=1;text="xlat";return true;}
        if(bytes.Length>=2 && bytes[0]==0x0f && bytes[1]==0x31){body=new IlBlock([new IlIntrin(IlType.V0,"pe32-rdtsc")]);length=2;text="rdtsc";return true;}
        if(bytes.Length>=2 && bytes[0]==0x0f && bytes[1]==0x77){body=new IlBlock([new IlIntrin(IlType.V0,"pe32-mmx",new IlConst(IlType.U32,0x77),new IlConst(IlType.U64,0))]);length=2;text="emms";return true;}
        int cursor=0;bool word=bytes[0]==0x66;if(word)++cursor;
        if(bytes.Length<cursor+2)return false;
        int opcode=bytes[cursor++];bool segment=opcode==0x8c;
        bool environment=(opcode is 0xd9 or 0xdd) && !word;
        bool mmx=opcode==0x0f && !word;
        if(!segment && !environment && !mmx)return false;
        if(mmx){opcode=bytes[cursor++];if(opcode is not (0x6e or 0x7e or 0x6f or 0x7f or 0xdb or 0x76 or 0xeb or 0xfa or 0x62 or 0x61 or 0xef or 0x2a))return false;}
        if(cursor>=bytes.Length)throw new NotSupportedException("Truncated legacy ModRM");
        int modrm=bytes[cursor++],mod=modrm>>6,reg=(modrm>>3)&7,rm=modrm&7;
        if(segment && reg>5 || environment && (mod==3 || reg is not (4 or 6)))return false;
        Il address=new IlConst(IlType.U64,0);
        if(mod!=3) {
            Il offset=new IlConst(IlType.U32,0);bool stackSegment=false;
            Il Gpr(int index)=>new IlReadReg(IlType.U32,RegKind.X86,index);
            Il Add(Il a,Il b)=>new IlBin(IlType.U32,BinOp.Add,a,b);
            bool displacementOnly=mod==0 && rm==5;
            if(rm==4) {
                if(cursor>=bytes.Length)throw new NotSupportedException("Truncated legacy operand SIB");
                int sib=bytes[cursor++],basis=sib&7,index=(sib>>3)&7,scale=sib>>6;
                displacementOnly=mod==0 && basis==5;
                if(!displacementOnly){offset=Gpr(basis);stackSegment=basis is 4 or 5;}
                if(index!=4)offset=Add(offset,new IlBin(IlType.U32,BinOp.Shl,Gpr(index),new IlConst(IlType.U8,(uint)scale)));
            } else if(!displacementOnly){offset=Gpr(rm);stackSegment=rm==5;}
            int displacementBytes=displacementOnly || mod==2 ? 4:mod==1 ? 1:0;
            if(cursor+displacementBytes>bytes.Length)throw new NotSupportedException("Truncated legacy operand displacement");
            uint displacement=displacementBytes==1 ? unchecked((uint)(sbyte)bytes[cursor]):
                displacementBytes==4 ? System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes[cursor..]):0;
            cursor+=displacementBytes;
            if(displacementBytes!=0)offset=Add(offset,new IlConst(IlType.U32,displacement));
            address=new IlBin(IlType.U64,BinOp.Add,new IlReadReg(IlType.U64,RegKind.X86Seg,stackSegment?2:3),new IlCast(IlType.U64,CastKind.Zext,offset));
        }

        length=cursor;
        if(segment){text="mov-segment-store";body=new IlBlock([new IlIntrin(IlType.V0,"pe32-segment-store",new IlConst(IlType.U32,(uint)(reg|(rm<<3)|(word?0x40:0)|(mod!=3?0x80:0))),address)]);}
        else if(environment){text="x87-environment";body=new IlBlock([new IlIntrin(IlType.V0,"pe32-x87-environment",new IlConst(IlType.U32,(uint)(((opcode&7)<<8)|modrm)),address)]);}
        else {text="mmx";body=new IlBlock([new IlIntrin(IlType.V0,"pe32-mmx",new IlConst(IlType.U32,(uint)(opcode|(reg<<8)|(rm<<11)|(mod!=3?0x4000:0))),address)]);}
        return true;
    }
}
