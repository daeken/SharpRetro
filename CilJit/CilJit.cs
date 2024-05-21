using System.Reflection;
using System.Reflection.Emit;
using JitBase;
using Sigil;

namespace CilJit;

public class CilJit<AddrT> : IJit<AddrT> where AddrT : struct {
	public DelegateT CreateFunction<DelegateT>(string name, Action<IBuilder<AddrT>> body) where DelegateT : Delegate {
		var ab = AssemblyBuilder.DefineDynamicAssembly(new(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
		var mb = ab.DefineDynamicModule($"{name}_Module");
		var tb = mb.DefineType($"{name}_Type");
		var ilg = Emit<DelegateT>.BuildMethod(tb, name, MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard);

		var builder = new CilBuilder<AddrT, DelegateT>(ilg, tb);
		body(builder);

		try {
			ilg.Return();
		} catch(SigilVerificationException) {
		}
		
		ilg.CreateMethod();
		var type = tb.CreateType() ?? throw new();

		foreach(var (obj, fb) in builder.Fields)
			type.GetField(fb.Name)?.SetValue(null, obj);

		return type.GetMethod(name)?.CreateDelegate<DelegateT>() ?? throw new();
	}
}