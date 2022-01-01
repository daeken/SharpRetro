using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using LLVMSharp.Interop;

namespace LlvmJit; 

public static class LlvmExtensions {
	public static LLVMTypeRef ToLLVMType(this Type type) {
		if(type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Vector128<>)) {
			var et = type.GetGenericArguments()[0];
			return LLVMTypeRef.CreateVector(et.ToLLVMType(), 16U / (uint) Marshal.SizeOf(et));
		}
		if(typeof(MulticastDelegate).IsAssignableFrom(type)) {
			var mi = type.GetMethod("Invoke") ?? throw new Exception();
			return LLVMTypeRef.CreateFunction(mi.ReturnType.ToLLVMType(),
				mi.GetParameters().Select(x => x.ParameterType.ToLLVMType()).ToArray(), false);
		}
		if(type == typeof(void))
			return LLVMTypeRef.Void;
		if(type.IsPointer) return LLVMTypeRef.Int64;
		return Activator.CreateInstance(type) switch {
			sbyte => LLVMTypeRef.Int8, 
			byte => LLVMTypeRef.Int8, 
			short => LLVMTypeRef.Int16, 
			ushort => LLVMTypeRef.Int16, 
			int => LLVMTypeRef.Int32, 
			uint => LLVMTypeRef.Int32, 
			long => LLVMTypeRef.Int64, 
			ulong => LLVMTypeRef.Int64, 
			float => LLVMTypeRef.Float, 
			double => LLVMTypeRef.Double, 
			bool => LLVMTypeRef.Int1, 
			_ => throw new NotSupportedException(type.Name)
		};
	}
}