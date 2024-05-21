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

	public static Type ToSigned<T>() => default(T) switch {
		byte => typeof(sbyte),
		ushort => typeof(short),
		uint => typeof(int),
		ulong => typeof(long),
		_ => typeof(T)
	};

	public static Type ToUnsigned<T>() => default(T) switch {
		sbyte => typeof(byte),
		short => typeof(ushort),
		int => typeof(uint),
		long => typeof(ulong),
		_ => typeof(T)
	};

	public static int BitWidth<T>() => Marshal.SizeOf<T>() * 8;

	static readonly Dictionary<Type, (Type DelegateType, Func<Delegate, object> Converter)> DelegateTypeMap = new();
	public static DelegateT GetAnyDelegateForFunctionPointer<DelegateT>(IntPtr ptr) {
		if(!typeof(DelegateT).IsGenericType) return Marshal.GetDelegateForFunctionPointer<DelegateT>(ptr);
		if(!DelegateTypeMap.TryGetValue(typeof(DelegateT), out var dt)) {
			var method = typeof(DelegateT).GetMethod("Invoke") ?? throw new();
			
			var ab = AssemblyBuilder.DefineDynamicAssembly(new(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
			var mb = ab.DefineDynamicModule("DelegateModule");
			var tb = mb.DefineType("DelegateType", TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));
			
			var ctor = tb.DefineConstructor(
				MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
				CallingConventions.Standard, [typeof(object), typeof(IntPtr)]);
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

			var bt = tb.CreateType() ?? throw new();
			var imi = bt.GetMethod("Invoke") ?? throw new();

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
	static readonly Dictionary<Type, object> InvDelegateTypeMap = new();
	public static IntPtr GetFunctionPointerForAnyDelegate<DelegateT>(DelegateT func) {
		if(!typeof(DelegateT).IsGenericType) return Marshal.GetFunctionPointerForDelegate(func);
		if(!InvDelegateTypeMap.TryGetValue(typeof(DelegateT), out var conv)) {
			var method = typeof(DelegateT).GetMethod("Invoke") ?? throw new();
			
			var ab = AssemblyBuilder.DefineDynamicAssembly(new(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
			var mb = ab.DefineDynamicModule("DelegateModule");
			var tb = mb.DefineType("DelegateType", TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));
			
			var ctor = tb.DefineConstructor(
				MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
				CallingConventions.Standard, [typeof(object), typeof(IntPtr)]);
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

			var bt = tb.CreateType() ?? throw new();
			var imi = bt.GetMethod("Invoke") ?? throw new();

			var del = Expression.Parameter(typeof(DelegateT));

			var subparams = parameters.Select(x => Expression.Parameter(x.ParameterType)).ToArray();
			
			var et = Expression.Lambda<Func<DelegateT, Delegate>>(
				Expression.Lambda(bt, 
					Expression.Call(del, method, subparams.Select(x => (Expression) x)), 
					subparams
				),
				del
			);
			InvDelegateTypeMap[typeof(DelegateT)] = conv = et.Compile();
		}
		return Marshal.GetFunctionPointerForDelegate(((Func<DelegateT, Delegate>) conv)(func));
	}
}