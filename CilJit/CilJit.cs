using System.Reflection;
using System.Reflection.Emit;
using JitBase;
using Sigil;

namespace CilJit;

public class CilJit<AddrT> : IJit<AddrT> where AddrT : struct {
	public DelegateT CreateFunction<DelegateT>(string name, Action<IBuilder<AddrT>> body) where DelegateT : Delegate {
		var ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
		var mb = ab.DefineDynamicModule($"{name}_Module");
		var tb = mb.DefineType($"{name}_Type");
		var ilg = Emit<DelegateT>.BuildMethod(tb, name, MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard);

		body(new CilBuilder<AddrT,DelegateT>(ilg, tb));
		
		ilg.CreateMethod();
		var type = tb.CreateType() ?? throw new Exception();
		return type.GetMethod(name)?.CreateDelegate<DelegateT>() ?? throw new Exception();
	}
}