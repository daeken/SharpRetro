using SwipcParser;

namespace SwipcGenerator;

public enum HandleStyle {
    Unknown,
    Copy,
    Move,
}

public abstract record IpcType(int Alignment = -1, string Name = null) {
    public static readonly UnknownType Unknown = new();
    public static readonly BoolType Bool = new();
    public static readonly IntType
        U8 = new(8, false), U16 = new(16, false),
        U32 = new(32, false), U64 = new(64, false),
        U128 = new(128, false),
        I8 = new(8, true), I16 = new(16, true),
        I32 = new(32, true), I64 = new(64, true),
        I128 = new(128, true);
    public static readonly FloatType F32 = new(32), F64 = new(64);
    
    public record UnknownType : IpcType;
    public record BoolType : IpcType;
    public record IntType(int Bits, bool Signed) : IpcType;
    public record FloatType(int bits) : IpcType;
    public record HandleType(HandleStyle Style) : IpcType;
    public record ObjectType(string Interface) : IpcType;
    public record PidType : IpcType;
    public record BufferType(IpcType DataType, int TransferType) : IpcType;
    public record BytesType(int Size = -1, int Alignment = -1) : IpcType(Alignment: Alignment);
    public record EnumType(IpcType UnderlyingType, IReadOnlyList<(string Name, ulong Value)> Options) : IpcType;
    public record StructType(IReadOnlyList<(string Name, IpcType Type)> Fields) : IpcType;
    public record ArrayType(IpcType ElementType, int Length) : IpcType;
}
