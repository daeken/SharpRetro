using JitBase;

namespace StaticRecompilerBase;

public abstract class StaticRecompilerBase<AddrT> where AddrT : struct {
    public void Recompile(AddrT entry) {
    }

    public abstract void Fetch<T>(AddrT addr, Span<T> data) where T : struct;
}