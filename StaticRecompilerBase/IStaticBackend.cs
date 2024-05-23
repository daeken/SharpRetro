namespace StaticRecompilerBase;

public interface IStaticBackend<AddrT> where AddrT : struct {
    void Emit(AddrT address, StaticIRStatement stmt);
}