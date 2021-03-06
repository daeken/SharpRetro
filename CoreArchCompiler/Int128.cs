using UltimateOrb;

namespace CoreArchCompiler; 

public class UInt128Wrapper {
    public readonly UInt128 Value;
    protected UInt128Wrapper(UInt128 value) => Value = value;
        
    public static implicit operator UInt128Wrapper(byte value) => new(value);
    public static implicit operator UInt128Wrapper(sbyte value) => new(value);
    public static implicit operator UInt128Wrapper(ushort value) => new(value);
    public static implicit operator UInt128Wrapper(short value) => new(value);
    public static implicit operator UInt128Wrapper(uint value) => new(value);
    public static implicit operator UInt128Wrapper(int value) => new(value);
    public static implicit operator UInt128Wrapper(ulong value) => new(value);
    public static implicit operator UInt128Wrapper(long value) => new(value);

    public static implicit operator byte(UInt128Wrapper value) => unchecked((byte) (uint) value.Value.LoInt64Bits);
    public static implicit operator sbyte(UInt128Wrapper value) => unchecked((sbyte) value.Value.LoInt64Bits);
    public static implicit operator ushort(UInt128Wrapper value) => unchecked((ushort) (ulong) value.Value.LoInt64Bits);
    public static implicit operator short(UInt128Wrapper value) => unchecked((short) value.Value.LoInt64Bits);
    public static implicit operator uint(UInt128Wrapper value) => unchecked((uint) (ulong) value.Value.LoInt64Bits);
    public static implicit operator int(UInt128Wrapper value) => unchecked((int) (uint) (ulong) value.Value.LoInt64Bits);
    public static implicit operator ulong(UInt128Wrapper value) => unchecked((ulong) value.Value.LoInt64Bits);
    public static implicit operator long(UInt128Wrapper value) => value.Value.LoInt64Bits;

    public static UInt128Wrapper operator +(UInt128Wrapper left, UInt128Wrapper right)
        => new(UInt128.op_AdditionUnchecked(left.Value, right.Value));
    public static UInt128Wrapper operator *(UInt128Wrapper left, UInt128Wrapper right)
        => new(UInt128.op_MultiplyUnchecked(left.Value, right.Value));
    public static UInt128Wrapper operator >>(UInt128Wrapper left, int right)
        => new(left.Value >> right);
    public static UInt128Wrapper operator <<(UInt128Wrapper left, int right)
        => new(left.Value << right);
}

public class Int128Wrapper {
    public readonly Int128 Value;
    protected Int128Wrapper(Int128 value) => Value = value;

    public static implicit operator Int128Wrapper(byte value) => new((uint) value);
    public static implicit operator Int128Wrapper(sbyte value) => new(value);
    public static implicit operator Int128Wrapper(ushort value) => new((uint) value);
    public static implicit operator Int128Wrapper(short value) => new(value);
    public static implicit operator Int128Wrapper(uint value) => new(value);
    public static implicit operator Int128Wrapper(int value) => new(value);
    public static implicit operator Int128Wrapper(ulong value) => new(value);
    public static implicit operator Int128Wrapper(long value) => new(value);

    public static implicit operator byte(Int128Wrapper value) => unchecked((byte) (uint) value.Value);
    public static implicit operator sbyte(Int128Wrapper value) => unchecked((sbyte) value.Value);
    public static implicit operator ushort(Int128Wrapper value) => unchecked((ushort) (uint) value.Value);
    public static implicit operator short(Int128Wrapper value) => unchecked((short) value.Value);
    public static implicit operator uint(Int128Wrapper value) => unchecked((uint) value.Value);
    public static implicit operator int(Int128Wrapper value) => unchecked((int) value.Value);
    public static implicit operator ulong(Int128Wrapper value) => unchecked((ulong) value.Value);
    public static implicit operator long(Int128Wrapper value) => unchecked((long) value.Value);

    public static Int128Wrapper operator +(Int128Wrapper left, Int128Wrapper right)
        => new(Int128.op_AdditionUnchecked(left.Value, right.Value));
    public static Int128Wrapper operator *(Int128Wrapper left, Int128Wrapper right)
        => new(Int128.op_MultiplyUnchecked(left.Value, right.Value));
    public static Int128Wrapper operator >>(Int128Wrapper left, int right)
        => new(left.Value >> right);
    public static Int128Wrapper operator <<(Int128Wrapper left, int right)
        => new(left.Value << right);
}