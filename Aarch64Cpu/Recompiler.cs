using JitBase;

namespace Aarch64Cpu;

public partial class Recompiler {
    void Branch(ulong addr) {
        throw new NotImplementedException();
    }

    void Branch(IRuntimeValue<ulong> addr) {
        throw new NotImplementedException();
    }

    void BranchLinked(ulong addr) {
        throw new NotImplementedException();
    }

    void BranchLinked(IRuntimeValue<ulong> addr) {
        throw new NotImplementedException();
    }

    void CallSvc(ulong svc) {
        throw new NotImplementedException();
    }
    
    void SR(uint op0, uint op1, uint crn, uint crm, uint op2, ulong value) {
        throw new NotImplementedException();
    }

    ulong SR(uint op0, uint op1, uint crn, uint crm, uint op2) {
        throw new NotImplementedException();
    }

    void Breakpoint(uint imm) {
        throw new NotImplementedException();
    }

    IRuntimeValue<byte> CompareAndSwap<T>(IRuntimePointer<ulong, T> pointer, IRuntimeValue<T> value, IRuntimeValue<T> comparand) where T : struct {
        throw new NotImplementedException();
    }

    void SetNZCV(IStructRef<CpuState> state, IRuntimeValue<ulong> nzcv) {
        throw new NotImplementedException();
    }
}