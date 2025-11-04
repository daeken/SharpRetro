using System.Runtime.Intrinsics;
using JitBase;

namespace Aarch64Cpu;

internal class RuntimeVectorSubIndexer<T>(IStructRef<CpuState> state) where T : struct {
    public IRuntimeValue<T> this[int index] {
        get => state.V[index].Element<T>(0);
        set {
            var nvec = state.V[index].Cast<Vector128<byte>>().ToZero().Cast<Vector128<float>>();
            state.V[index] = nvec.Element(0, value);
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