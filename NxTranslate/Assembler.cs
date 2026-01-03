using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NxTranslate;

public abstract record R {
    public static readonly RSP SP = new();
    public static readonly RX
        X0  = new( 0), X1  = new( 1), X2  = new( 2), X3  = new( 3), 
        X4  = new( 4), X5  = new( 5), X6  = new( 6), X7  = new( 7),
        X8  = new( 8), X9  = new( 9), X10 = new(10), X11 = new(11),
        X12 = new(12), X13 = new(13), X14 = new(14), X15 = new(15), 
        X16 = new(16), X17 = new(17), X18 = new(18), X19 = new(19),
        X20 = new(20), X21 = new(21), X22 = new(22), X23 = new(23),
        X24 = new(24), X25 = new(25), X26 = new(26), X27 = new(27),
        X28 = new(28), X29 = new(29), X30 = new(30), X31 = new(31);
    public static readonly RX XZR = X31;
    public record RX(int Number) : R;
    public record RSP : R;
}

public class Assembler {
    readonly uint[] Instructions;
    public int I = 0;
    public ulong PC {
        get => (ulong) I * 4;
        set => I = (int) (value / 4);
    }

    public ulong Size => (ulong) Instructions.Length * 4;
    public byte[] AsBytes => MemoryMarshal.Cast<uint, byte>(Instructions).ToArray();
    public uint LastInsn => Instructions[I - 1];
    
    public Assembler(int regionSize) {
        Debug.Assert(regionSize % 4 == 0);
        Instructions = new uint[regionSize / 4];
    }

    public void Str(R.RX rs, R rd) {
        var insn = 0b11_111_0_01_00_000000000000_00000_00000U;
        insn |= (uint) rs.Number << 0;
        insn |= rd is R.RX td ? (uint) td.Number << 5 : (uint) 0b11111 << 5;
        Instructions[I++] = insn;
    }

    public void Ldr(R.RX rd, R rn) {
        var insn = 0b11_111_0_01_01_000000000000_00000_00000U;
        insn |= (uint) rd.Number << 0;
        insn |= rn is R.RX tn ? (uint) tn.Number << 5 : (uint) 0b11111 << 5;
        Instructions[I++] = insn;
    }

    public void Stp(R.RX a, R.RX b, R t, int imm = 0) {
        var insn = 0b10_101_0_010_0_0000000_00000_00000_00000U;
        insn |= ((unchecked((uint) imm) >> 3) & 0b1111111) << 15;
        insn |= (uint) a.Number << 0;
        insn |= (uint) b.Number << 10;
        insn |= t is R.RX tr ? (uint) tr.Number << 5 : (uint) 0b11111 << 5;
        Instructions[I++] = insn;
    }

    public void StpPreindex(R.RX a, R.RX b, R t, int imm) {
        var insn = 0b10_101_0_011_0_0000000_00000_00000_00000U;
        insn |= ((unchecked((uint) imm) >> 3) & 0b1111111) << 15;
        insn |= (uint) a.Number << 0;
        insn |= (uint) b.Number << 10;
        insn |= t is R.RX tr ? (uint) tr.Number << 5 : (uint) 0b11111 << 5;
        Instructions[I++] = insn;
    }

    public void Ldp(R.RX a, R.RX b, R t, int imm = 0) {
        var insn = 0b10_101_0_010_1_0000000_00000_00000_00000U;
        insn |= ((unchecked((uint) imm) >> 3) & 0b1111111) << 15;
        insn |= (uint) a.Number << 0;
        insn |= (uint) b.Number << 10;
        insn |= t is R.RX tr ? (uint) tr.Number << 5 : (uint) 0b11111 << 5;
        Instructions[I++] = insn;
    }
    public void LdpPostindex(R.RX a, R.RX b, R t, int imm) {
        var insn = 0b10_101_0_001_1_0000000_00000_00000_00000U;
        insn |= ((unchecked((uint) imm) >> 3) & 0b1111111) << 15;
        insn |= (uint) a.Number << 0;
        insn |= (uint) b.Number << 10;
        insn |= t is R.RX tr ? (uint) tr.Number << 5 : (uint) 0b11111 << 5;
        Instructions[I++] = insn;
    }

    public void B(long imm) {
        var insn = 0b0_00101_00000000000000000000000000U;
        insn |= unchecked((uint) (ulong) (imm >> 2)) & 0b11111111111111111111111111;
        Instructions[I++] = insn;
    }
    
    public void BL(long imm) {
        var insn = 0b1_00101_00000000000000000000000000U;
        insn |= unchecked((uint) (ulong) (imm >> 2)) & 0b11111111111111111111111111;
        Instructions[I++] = insn;
    }
    
    public void BlSelf() => Instructions[I++] = 0b1_00101_00000000000000000000000001U;

    public void Blr(R.RX rn) {
        var insn = 0b1101011_0_0_01_11111_0000_0_0_00000_00000U;
        insn |= (uint) rn.Number << 5;
        Instructions[I++] = insn;
    }

    public void ReadNzcv(R.RX rt) {
        var insn = 0b1101010100_1_1_1_011_0100_0010_000_00000;
        insn |= (uint) rt.Number;
        Instructions[I++] = insn;
    }

    public void WriteNzcv(R.RX rt) {
        var insn = 0b1101010100_0_1_1_011_0100_0010_000_00000;
        insn |= (uint) rt.Number;
        Instructions[I++] = insn;
    }

    public void Mov(R.RX rd, ulong value) {
        Movz(rd, (ushort) (value & 0xFFFF), 0);
        if((value & 0xFFFF_0000) != 0) Movk(rd, (ushort) ((value >> 16)  & 0xFFFF), 16);
        if((value & 0xFFFF_0000_0000) != 0) Movk(rd, (ushort) ((value >> 32)  & 0xFFFF), 32);
        if((value & 0xFFFF_0000_0000_0000) != 0) Movk(rd, (ushort) ((value >> 48)  & 0xFFFF), 48);
    }

    public void Movz(R.RX rd, ushort imm, int shift) {
        Debug.Assert((shift & 0b1111) == 0);
        var insn = 0b1_10_100101_00_0000000000000000_00000U;
        insn |= (uint) rd.Number << 0;
        insn |= (uint) imm << 5;
        insn |= (uint) ((shift >> 4) & 0b11) << 21;
        Instructions[I++] = insn;
    }

    public void Movk(R.RX rd, ushort imm, int shift) {
        Debug.Assert((shift & 0b1111) == 0);
        var insn = 0b1_11_100101_00_0000000000000000_00000U;
        insn |= (uint) rd.Number << 0;
        insn |= (uint) imm << 5;
        insn |= (uint) ((shift >> 4) & 0b11) << 21;
        Instructions[I++] = insn;
    }

    public void Mov(R.RX rd, R.RX rm) => Orr(rd, R.XZR, rm);
    public void Mov(R.RX rd, R.RSP rn) => Add(rd, rn, R.XZR);

    public void Add(R rd, R rn, R.RX rm) {
        var insn = 0b1_0_0_01011_00_1_00000_111_000_00000_00000U;
        insn |= rd is R.RX td ? (uint) td.Number << 0 : (uint) 0b11111 << 0;
        insn |= rn is R.RX tn ? (uint) tn.Number << 5 : (uint) 0b11111 << 5;
        insn |= (uint) rm.Number << 16;
        Instructions[I++] = insn;
    }

    public void Add(R rd, R rn, ushort imm) {
        var insn = 0b1_0_0_10001_00_000000000000_00000_00000U;
        insn |= rd is R.RX td ? (uint) td.Number << 0 : (uint) 0b11111 << 0;
        insn |= rn is R.RX tn ? (uint) tn.Number << 5 : (uint) 0b11111 << 5;
        insn |= (uint) (imm & 0b1111_1111_1111) << 10;
        Instructions[I++] = insn;
    }
    
    public void Sub(R rd, R rn, R.RX rm) {
        var insn = 0b1_1_0_01011_00_1_00000_111_000_00000_00000U;
        insn |= rd is R.RX td ? (uint) td.Number << 0 : (uint) 0b11111 << 0;
        insn |= rn is R.RX tn ? (uint) tn.Number << 5 : (uint) 0b11111 << 5;
        insn |= (uint) rm.Number << 16;
        Instructions[I++] = insn;
    }
    
    public void Sub(R rd, R rn, ushort imm) {
        var insn = 0b1_1_0_10001_00_000000000000_00000_00000U;
        insn |= rd is R.RX td ? (uint) td.Number << 0 : (uint) 0b11111 << 0;
        insn |= rn is R.RX tn ? (uint) tn.Number << 5 : (uint) 0b11111 << 5;
        insn |= (uint) (imm & 0b1111_1111_1111) << 10;
        Instructions[I++] = insn;
    }
    
    public void Orr(R.RX rd, R.RX rn, R.RX rm) {
        var insn = 0b1_01_01010_00_0_00000_000000_00000_00000U;
        insn |= (uint) rd.Number << 0;
        insn |= (uint) rn.Number << 5;
        insn |= (uint) rm.Number << 16;
        Instructions[I++] = insn;
    }

    public void Ret(R.RX rn = null) {
        rn ??= R.X30;
        var insn = 0b1101011_0_0_10_11111_0000_0_0_00000_00000U;
        insn |= (uint) rn.Number << 5;
        Instructions[I++] = insn;
    }

    public void Adrp(R.RX rd, ulong imm) {
        var insn = 0b1_00_10000_0000000000000000000_00000;
        insn |= (uint) (imm & 0b11) << 29;
        insn |= (uint) ((imm >> 2) & 0x7FFFF) << 5;
        insn |= (uint) rd.Number;
        Instructions[I++] = insn;
    }

    public void AddrOf(R.RX rd, ulong current, ulong target) {
        var delta = (long) ((target >> 12) << 12) - (long) ((current >> 12) << 12);
        Adrp(rd, unchecked((ulong) (delta >> 12)));
        var sdelta = target & 0xFFF;
        if(sdelta > 0)
            Add(rd, rd, (ushort) sdelta);
    }

    public void Raw(uint insn) {
        Instructions[I++] = insn;
    }
}