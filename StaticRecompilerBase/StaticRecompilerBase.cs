using JitBase;

namespace StaticRecompilerBase;

public abstract class StaticRecompilerBase<AddrT> where AddrT : struct {
    public void RecompileFrom(AddrT entry) {
    }

    public abstract bool IsValidCodeAt(AddrT addr);
    public abstract bool Recompile(AddrT addr);
}