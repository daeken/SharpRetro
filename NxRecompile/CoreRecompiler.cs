using StaticRecompilerBase;

namespace NxRecompile;

public class CoreRecompiler : StaticRecompilerBase<ulong> {
    public override void Fetch<T>(ulong addr, Span<T> data) {
        throw new NotImplementedException();
    }
}