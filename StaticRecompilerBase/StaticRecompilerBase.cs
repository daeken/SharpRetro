using JitBase;

namespace StaticRecompilerBase;

public abstract class StaticRecompilerBase<AddrT> where AddrT : struct {
    public void Recompile(AddrT entry) {
    }
}