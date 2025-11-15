using JitBase;

namespace Aarch64Cpu;

public partial class Recompiler {
    protected virtual void Branch(ulong addr) {
        throw new NotImplementedException();
    }

    protected virtual void Branch(IRuntimeValue<ulong> addr) {
        throw new NotImplementedException();
    }

    protected virtual void BranchLinked(ulong addr) {
        throw new NotImplementedException();
    }

    protected virtual void BranchLinked(IRuntimeValue<ulong> addr) {
        throw new NotImplementedException();
    }

    protected virtual void CallSvc(ulong svc) {
        throw new NotImplementedException();
    }
    
    protected virtual void SR(uint op0, uint op1, uint crn, uint crm, uint op2, IRuntimeValue<ulong> value) {
        throw new NotImplementedException();
    }

    protected virtual IRuntimeValue<ulong> SR(uint op0, uint op1, uint crn, uint crm, uint op2) {
        throw new NotImplementedException();
    }

    protected virtual void Breakpoint(uint imm) {
        throw new NotImplementedException();
    }

    protected virtual IRuntimeValue<byte> CompareAndSwap<T>(IRuntimePointer<ulong, T> pointer, IRuntimeValue<T> value, IRuntimeValue<T> comparand) where T : struct {
        throw new NotImplementedException();
    }

    protected virtual void SetNZCV(IStructRef<CpuState> state, IRuntimeValue<ulong> nzcv) {
        throw new NotImplementedException();
    }
}