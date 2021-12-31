namespace JitBase; 

public interface IJit<AddrT> where AddrT : struct {
	DelegateT CreateFunction<DelegateT>(string name, Action<IBuilder<AddrT>> body) where DelegateT : Delegate;
}