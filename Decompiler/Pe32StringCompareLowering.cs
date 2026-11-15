using LiftIl;
namespace SharpRetro.Decompiler;

// Canonical address32 CMPS/SCAS. Segment overrides and duplicate prefixes keep
// their existing explicit failure; no eager operand loads for a zero count.
public static class Pe32StringCompareLowering {
    public static bool TryLower(ReadOnlySpan<byte> bytes,out IlBlock body) {
        body=new IlBlock([]);int cursor=0,repeat=0;bool word=false;
        while(cursor<bytes.Length && bytes[cursor] is 0x66 or 0xf2 or 0xf3) {
            int prefix=bytes[cursor++];
            if(prefix==0x66) {if(word)return false;word=true;}
            else {if(repeat!=0)return false;repeat=prefix;}
        }
        if(cursor+1!=bytes.Length || bytes[cursor] is not (0xa6 or 0xa7 or 0xae or 0xaf))return false;
        int opcode=bytes[cursor];bool scan=opcode>=0xae;
        if(word && (opcode&1)==0)return false;
        int width=(opcode&1)==0 ? 8 : word ? 16 : 32;
        uint descriptor=(uint)(width|(scan?0x100:0)|(repeat!=0?0x200:0)|(repeat==0xf2?0x400:0));
        body=new IlBlock([new IlIntrin(IlType.V0,"pe32-string-compare",new IlConst(IlType.U32,descriptor))]);
        return true;
    }
}
