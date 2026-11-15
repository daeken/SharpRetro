using LiftIl;
namespace SharpRetro.Decompiler;

// Canonical operand16/32, address32 SHLD/SHRD; no eager value loads.
public static class Pe32DoubleShiftLowering {
    public static bool TryLower(ReadOnlySpan<byte> bytes,out IlBlock body) {
        body=new IlBlock([]);int cursor=0;bool word=false;
        if(bytes.Length>0 && bytes[0]==0x66){word=true;++cursor;}
        if(bytes.Length<cursor+3 || bytes[cursor++]!=0x0f)return false;
        int opcode=bytes[cursor++];if(opcode is not (0xa4 or 0xa5 or 0xac or 0xad))return false;
        int modrm=bytes[cursor++],mod=modrm>>6,reg=(modrm>>3)&7,rm=modrm&7;
        Il address=new IlConst(IlType.U64,0);
        if(mod!=3) {
            Il offset=new IlConst(IlType.U32,0);bool stackSegment=false;
            Il Gpr(int index)=>new IlReadReg(IlType.U32,RegKind.X86,index);
            Il Add(Il a,Il b)=>new IlBin(IlType.U32,BinOp.Add,a,b);
            bool displacementOnly=mod==0 && rm==5;
            if(rm==4) {
                if(cursor>=bytes.Length)throw new NotSupportedException("Truncated double-shift SIB");
                int sib=bytes[cursor++],basis=sib&7,index=(sib>>3)&7,scale=sib>>6;
                displacementOnly=mod==0 && basis==5;
                if(!displacementOnly){offset=Gpr(basis);stackSegment=basis is 4 or 5;}
                if(index!=4)offset=Add(offset,new IlBin(IlType.U32,BinOp.Shl,Gpr(index),new IlConst(IlType.U8,(uint)scale)));
            } else if(!displacementOnly){offset=Gpr(rm);stackSegment=rm==5;}
            int displacementBytes=displacementOnly || mod==2 ? 4:mod==1 ? 1:0;
            if(cursor+displacementBytes>bytes.Length)throw new NotSupportedException("Truncated double-shift displacement");
            uint displacement=displacementBytes==1 ? unchecked((uint)(sbyte)bytes[cursor]):
                displacementBytes==4 ? System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes[cursor..]):0;
            cursor+=displacementBytes;
            if(displacementBytes!=0)offset=Add(offset,new IlConst(IlType.U32,displacement));
            address=new IlBin(IlType.U64,BinOp.Add,new IlReadReg(IlType.U64,RegKind.X86Seg,stackSegment?2:3),new IlCast(IlType.U64,CastKind.Zext,offset));
        }
        Il count;
        if((opcode&1)==0) {
            if(cursor>=bytes.Length)throw new NotSupportedException("Truncated double-shift count");
            count=new IlConst(IlType.U8,bytes[cursor++]);
        } else count=new IlReadReg(IlType.U8,RegKind.X86,1);
        if(cursor!=bytes.Length)throw new NotSupportedException("Double-shift length differs from decoder");
        uint descriptor=(uint)((opcode>=0xac?1:0)|(word?2:0)|(mod!=3?4:0)|(reg<<8)|(rm<<11));
        body=new IlBlock([new IlIntrin(IlType.V0,"pe32-double-shift",new IlConst(IlType.U32,descriptor),address,count)]);
        return true;
    }
}
