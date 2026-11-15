using LiftIl;
namespace SharpRetro.Decompiler;
// Keep conditional/flag-setting x87 forms indivisible: integer flags are part
// of their architectural input/output and are not floating-point operands.
public static class Pe32X87RegisterLowering {
    public static bool TryLower(ReadOnlySpan<byte> bytes,out IlBlock body) {
        body=new IlBlock([]);
        if(bytes.Length!=2)return false;
        int first=bytes[0],second=bytes[1];
        bool conditional=(first is 0xda or 0xdb) && second>=0xc0 && second<=0xdf;
        bool compare=(first is 0xdb or 0xdf) && second>=0xe8 && second<=0xf7;
        if(!conditional && !compare)return false;
        body=new IlBlock([new IlIntrin(IlType.V0,"pe32-x87-register",new IlConst(IlType.U32,(uint)(((first&7)<<8)|second)))]);
        return true;
    }
}
