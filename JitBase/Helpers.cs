using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace JitBase; 

public static class Helpers {
	public static bool IsSigned<U>() => default(U) is sbyte or short or int or long;
	public static void IsSigned<U>(Action if_, Action else_) {
		if(default(U) is sbyte or short or int or long)
			if_();
		else
			else_();
	}

	static readonly Dictionary<Type, (Type DelegateType, Func<Delegate, object> Converter)> DelegateTypeMap = new();
	public static DelegateT GetAnyDelegateForFunctionPointer<DelegateT>(IntPtr ptr) {
		if(!typeof(DelegateT).IsGenericType) return Marshal.GetDelegateForFunctionPointer<DelegateT>(ptr);
		if(!DelegateTypeMap.TryGetValue(typeof(DelegateT), out var dt)) {
			var method = typeof(DelegateT).GetMethod("Invoke") ?? throw new Exception();
			
			var ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
			var mb = ab.DefineDynamicModule("DelegateModule");
			var tb = mb.DefineType("DelegateType", TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));
			
			var ctor = tb.DefineConstructor(
				MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
				CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
			ctor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

			var parameters = method.GetParameters();

			var invokeMethod = tb.DefineMethod(
				"Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
				method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
			invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

			for(var i = 0; i < parameters.Length; i++) {
				var parameter = parameters[i];
				invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
			}

			var bt = tb.CreateType() ?? throw new Exception();
			var imi = bt.GetMethod("Invoke") ?? throw new Exception();

			var del = Expression.Parameter(typeof(Delegate));

			var subparams = parameters.Select(x => Expression.Parameter(x.ParameterType)).ToArray();
			
			var et = Expression.Lambda<Func<Delegate, object>>(
					Expression.Lambda(typeof(DelegateT), 
						Expression.Call(Expression.Convert(del, bt), imi, subparams.Select(x => (Expression) x)), 
						subparams
					),
					del
				);
			dt = DelegateTypeMap[typeof(DelegateT)] = (bt, et.Compile());
		}
		var (ttype, conv) = dt;
		return (DelegateT) conv(Marshal.GetDelegateForFunctionPointer(ptr, ttype));
	}
}