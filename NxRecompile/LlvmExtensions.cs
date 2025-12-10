using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using LLVMSharp.Interop;

namespace NxRecompile;

// This is literally just a way to represent i1
struct Bit {
}

public static unsafe class LlvmExtensions {
    public static readonly sbyte* EmptyString;

    static LlvmExtensions() {
        EmptyString = (sbyte*) Marshal.AllocHGlobal(1);
        EmptyString[0] = 0;
    }
	
    extension(Type type) {
        public int ByteCount =>
            type == typeof(bool)
                ? 8
                : type.IsConstructedGenericType
                    ? type.GetGenericTypeDefinition() == typeof(Vector128<>)
                        ? 16
                        : throw new NotSupportedException()
                    : Marshal.SizeOf(type);
        public bool IsFloat =>
            type == typeof(float) || type == typeof(double) ||
            type == typeof(Vector128<float>) || type == typeof(Vector128<double>);
        public bool IsSigned =>
            type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(Int128);
        public bool IsVector =>
            type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Vector128<>);
        public LLVMTypeRef ToLLVMType() {
            if(type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Vector128<>)) {
                var et = type.GetGenericArguments()[0];
                return LLVMTypeRef.CreateVector(et.ToLLVMType(), 16U / (uint) Marshal.SizeOf(et));
            }
            if(typeof(MulticastDelegate).IsAssignableFrom(type)) {
                var mi = type.GetMethod("Invoke") ?? throw new();
                return LLVMTypeRef.CreateFunction(mi.ReturnType.ToLLVMType(),
                    mi.GetParameters().Select(x => x.ParameterType.ToLLVMType()).ToArray(), false);
            }
            if(type == typeof(void))
                return LLVMTypeRef.Void;
            if(type.IsPointer || type.IsByRef) return LLVMTypeRef.Int64;
            return Activator.CreateInstance(type) switch {
                sbyte or byte => LLVMTypeRef.Int8, 
                short or ushort => LLVMTypeRef.Int16, 
                int or uint => LLVMTypeRef.Int32, 
                long or ulong => LLVMTypeRef.Int64, 
                UInt128 or Int128 => LLVMTypeRef.CreateInt(128),
                float => LLVMTypeRef.Float, 
                double => LLVMTypeRef.Double, 
                bool => LLVMTypeRef.Int64, 
                Bit => LLVMTypeRef.Int1,
                _ => throw new NotSupportedException(type.Name)
            };
        }
    }
	
    public static LLVMTypeRef LlvmType<T>() => typeof(T).ToLLVMType();
}