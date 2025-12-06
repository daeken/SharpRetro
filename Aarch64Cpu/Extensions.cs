using System.Runtime.Intrinsics;
using JitBase;

namespace Aarch64Cpu;

internal class RuntimeVectorSubIndexer<T>(IStructRef<CpuState> state) where T : struct {
    public IRuntimeValue<T> this[int index] {
        get => state.V[index].Element<T>(0);
        set {
            var builder = state.GetBuilder<ulong>();
            if(typeof(T) == typeof(byte)) {
                var zero = builder.Zero<byte>();
                state.V[index] = builder.CreateVector(
                    (IRuntimeValue<byte>) value, zero, zero, zero, 
                    zero, zero, zero, zero,
                    zero, zero, zero, zero,
                    zero, zero, zero, zero
                );
            } else if(typeof(T) == typeof(ushort)) {
                var zero = builder.Zero<ushort>();
                state.V[index] = builder.CreateVector(
                    (IRuntimeValue<ushort>) value, zero, zero, zero, 
                    zero, zero, zero, zero
                );
            } else if(typeof(T) == typeof(float)) {
                var zero = builder.Zero<float>();
                state.V[index] = builder.CreateVector(
                    (IRuntimeValue<float>) value, zero, zero, zero
                );
            } else if(typeof(T) == typeof(double))
                state.V[index] = builder.CreateVector((IRuntimeValue<double>) value, builder.Zero<double>());
            else
                throw new NotImplementedException($"Unhandled type {typeof(T)}");
        }
    }
}

internal static class Extensions {
    extension(IStructRef<CpuState> state) {
        public RuntimeVectorSubIndexer<byte> VB => new(state);
        public RuntimeVectorSubIndexer<ushort> VH => new(state);
        public RuntimeVectorSubIndexer<uint> VW => new(state);
        public RuntimeVectorSubIndexer<float> VS => new(state);
        public RuntimeVectorSubIndexer<double> VD => new(state);
    }
}