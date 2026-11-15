using LiftIl;
using XFusionCpu;
namespace SharpRetro.Decompiler;

// The generic lifter still has legacy placeholders for MXCSR and treats x87
// intrinsic destinations as values. Correct only these explicit PE32 forms;
// every unexpected shape stays unsupported instead of retaining a stub.
public static class FpControlLowering {
    public static IlBlock Rewrite(string mnemonic,IlBlock body) {
        if(mnemonic is "FNSTCW" or "FNSTSW-M" or "FLD-M32" or "FLD-M64" or "FLD-M80" or "FST-M32" or "FST-M64" or "FSTP-M32" or "FSTP-M64" or "FSTP-M80" or "FILD-M32" or "FILD-M64" or
            "FIADD-M16" or "FIADD-M32" or "FIMUL-M16" or "FIMUL-M32" or "FISUB-M16" or "FISUB-M32" or "FISUBR-M16" or "FISUBR-M32" or "FIDIV-M16" or "FIDIV-M32" or "FIDIVR-M16" or "FIDIVR-M32" or
            "FSUB-M32" or "FSUB-M64" or "FSUBR-M32" or "FSUBR-M64" or "FMUL-M32" or "FMUL-M64" or "FDIV-M32" or "FDIV-M64" or "FDIVR-M32" or "FDIVR-M64" or "FADD-M32" or "FADD-M64" or
            "FCOM-M32" or "FCOM-M64" or "FCOMP-M32" or "FCOMP-M64" or "FIST-M16" or "FIST-M32" or "FISTP-M16" or "FISTP-M32" or "FISTP-M64") {
            string name=mnemonic switch { "FNSTCW"=>"fnstcw","FNSTSW-M"=>"fnstsw-m","FMUL-M32"=>"fmul-m",
                "FADD-M32"=>"fadd-m","FSUB-M32"=>"fsub-m","FSUBR-M32"=>"fsubr-m","FDIV-M32"=>"fdiv-m","FDIVR-M32"=>"fdivr-m",
                "FCOM-M32"=>"fcom-m","FCOMP-M32"=>"fcomp-m",_=>mnemonic.ToLowerInvariant() };
            int bits=mnemonic.EndsWith("80") ? 80 : mnemonic.EndsWith("64") ? 64 : mnemonic.EndsWith("32") ? 32 : 16;
            if(body.Body is not [IlLet address,IlIntrin store] || store.Name!=name ||
               store.Args is not [IlLoad operand] || operand.Ty is not IlType.I integer ||
               (integer.Bits!=bits && !(mnemonic is "FLD-M80" or "FSTP-M80" && integer.Bits==32)))
                throw new NotSupportedException("Unexpected x87 store-control operand shape: "+body);
            // Legacy m80 intrinsics carry dummy u32 loads. Only the address
            // survives; explicit providers read/write the actual ten bytes.
            string explicitName=mnemonic.EndsWith("M32") ? mnemonic.ToLowerInvariant() : name;
            return new IlBlock([address,new IlIntrin(IlType.V0,explicitName+"-address",operand.Addr)]);
        }
        if(mnemonic=="LDMXCSR") {
            if(body.Body is not [IlLet address])
                throw new NotSupportedException("Unexpected LDMXCSR operand shape: "+body);
            return new IlBlock([address,new IlIntrin(IlType.V0,"ldmxcsr-address",new IlTmp(address.Val.Ty,address.Id))]);
        }
        if(mnemonic=="STMXCSR") {
            if(body.Body is not [IlLet address,IlStore { Val:IlConst value } store] || value.Bits!=0x1f80 ||
               value.Ty is not IlType.I { Bits:32 })
                throw new NotSupportedException("Unexpected STMXCSR operand shape: "+body);
            return new IlBlock([address,new IlIntrin(IlType.V0,"stmxcsr-address",store.Addr)]);
        }
        if(mnemonic is "VLDMXCSR" or "VSTMXCSR") throw new NotSupportedException("VEX FP-state forms remain unsupported");
        return body;
    }
}
