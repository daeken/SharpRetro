// ReSharper disable CheckNamespace

#region

using LibSharpRetro;

#endregion

namespace DamageCore;

public partial class Interpreter {
    public bool Interpret(Span<byte> insnBytes, ref ushort pc) {
        unchecked {
            /* LD-rd-rs */
            if((insnBytes[0] & 0xC0) == 0x40) {
                var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                if(!((rd != 0x6) & (rs != 0x6)))
                    goto insn_1;
                pc += 1;
                State.Registers[rd] =
                    rs switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                AddCycles(0x1);
                return true;
            }

            insn_1:
            /* LD-rd-imm8 */
            if((insnBytes[0] & 0xC7) == 0x6) {
                var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                if(!(rd != 0x6))
                    goto insn_2;
                pc += 2;
                State.Registers[rd] = imm;
                AddCycles(0x2);
                return true;
            }

            insn_2:
            /* LD-rd-HL */
            if((insnBytes[0] & 0xC7) == 0x46) {
                var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
                if(!(rd != 0x6))
                    goto insn_3;
                pc += 1;
                State.Registers[rd] =
                    ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]));
                AddCycles(0x2);
                return true;
            }

            insn_3:
            /* LD-HL-rs */
            if((insnBytes[0] & 0xF8) == 0x70) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                if(!(rs != 0x6))
                    goto insn_4;
                pc += 1;
                WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                    rs switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                AddCycles(0x2);
                return true;
            }

            insn_4:
            /* LD-HL-imm8 */
            if((insnBytes[0] & 0xFF) == 0x36) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), imm);
                AddCycles(0x3);
                return true;
            }

            /* LD-A-BC */
            if((insnBytes[0] & 0xFF) == 0xA) {
                pc += 1;
                State.Registers[0x7] =
                    ReadMemory<byte>((ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]));
                AddCycles(0x2);
                return true;
            }

            /* LD-A-DE */
            if((insnBytes[0] & 0xFF) == 0x1A) {
                pc += 1;
                State.Registers[0x7] =
                    ReadMemory<byte>((ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]));
                AddCycles(0x2);
                return true;
            }

            /* LD-BC-A */
            if((insnBytes[0] & 0xFF) == 0x2) {
                pc += 1;
                WriteMemory((ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                    0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                AddCycles(0x2);
                return true;
            }

            /* LD-DE-A */
            if((insnBytes[0] & 0xFF) == 0x12) {
                pc += 1;
                WriteMemory((ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                    0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                AddCycles(0x2);
                return true;
            }

            /* LD-A-imm16 */
            if((insnBytes[0] & 0xFF) == 0xFA) {
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                State.Registers[0x7] = ReadMemory<byte>(addr);
                AddCycles(0x4);
                return true;
            }

            /* LD-imm16-A */
            if((insnBytes[0] & 0xFF) == 0xEA) {
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                WriteMemory(addr,
                    0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                AddCycles(0x4);
                return true;
            }

            /* LDH-A-C */
            if((insnBytes[0] & 0xFF) == 0xF2) {
                pc += 1;
                State.Registers[0x7] = ReadMemory<byte>((ushort) (0xFF00 | 0x1 switch {
                    0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                }));
                AddCycles(0x2);
                return true;
            }

            /* LDH-C-A */
            if((insnBytes[0] & 0xFF) == 0xE2) {
                pc += 1;
                WriteMemory(
                    (ushort) (0xFF00 | 0x1 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    }), 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                AddCycles(0x2);
                return true;
            }

            /* LDH-A-imm8 */
            if((insnBytes[0] & 0xFF) == 0xF0) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var addr = (ushort) (0xFF00 | imm);
                pc += 2;
                State.Registers[0x7] = ReadMemory<byte>(addr);
                AddCycles(0x3);
                return true;
            }

            /* LDH-imm8-A */
            if((insnBytes[0] & 0xFF) == 0xE0) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var addr = (ushort) (0xFF00 | imm);
                pc += 2;
                WriteMemory(addr,
                    0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                AddCycles(0x3);
                return true;
            }

            /* LD-A-HL- */
            if((insnBytes[0] & 0xFF) == 0x3A) {
                pc += 1;
                var hl = (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]);
                State.Registers[0x7] = ReadMemory<byte>(hl);
                var temp_36 = (ushort) (hl - 0x1);
                State.Registers[0b100] = (byte) (temp_36 >> 8);
                State.Registers[0b101] = (byte) (temp_36 & 0xFF);
                AddCycles(0x2);
                return true;
            }

            /* LD-HL--A */
            if((insnBytes[0] & 0xFF) == 0x32) {
                pc += 1;
                var hl = (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]);
                WriteMemory(hl, 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                var temp_37 = (ushort) (hl - 0x1);
                State.Registers[0b100] = (byte) (temp_37 >> 8);
                State.Registers[0b101] = (byte) (temp_37 & 0xFF);
                AddCycles(0x2);
                return true;
            }

            /* LD-A-HL+ */
            if((insnBytes[0] & 0xFF) == 0x2A) {
                pc += 1;
                var hl = (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]);
                State.Registers[0x7] = ReadMemory<byte>(hl);
                var temp_38 = (ushort) (hl + 0x1);
                State.Registers[0b100] = (byte) (temp_38 >> 8);
                State.Registers[0b101] = (byte) (temp_38 & 0xFF);
                AddCycles(0x2);
                return true;
            }

            /* LD-HL+-A */
            if((insnBytes[0] & 0xFF) == 0x22) {
                pc += 1;
                var hl = (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]);
                WriteMemory(hl, 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] });
                var temp_39 = (ushort) (hl + 0x1);
                State.Registers[0b100] = (byte) (temp_39 >> 8);
                State.Registers[0b101] = (byte) (temp_39 & 0xFF);
                AddCycles(0x2);
                return true;
            }

            /* LD-rr-imm16 */
            if((insnBytes[0] & 0xCF) == 0x1) {
                var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var imm = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                switch(r) {
                    case 0x0: {
                        var temp_40 = imm;
                        State.Registers[0b000] = (byte) (temp_40 >> 8);
                        State.Registers[0b001] = (byte) (temp_40 & 0xFF);
                        break;
                    }
                    case 0x1: {
                        var temp_41 = imm;
                        State.Registers[0b010] = (byte) (temp_41 >> 8);
                        State.Registers[0b011] = (byte) (temp_41 & 0xFF);
                        break;
                    }
                    case 0x2: {
                        var temp_42 = imm;
                        State.Registers[0b100] = (byte) (temp_42 >> 8);
                        State.Registers[0b101] = (byte) (temp_42 & 0xFF);
                        break;
                    }
                    default: {
                        State.SP = imm;
                        break;
                    }
                }

                AddCycles(0x3);
                return true;
            }

            /* LD-imm16-SP */
            if((insnBytes[0] & 0xFF) == 0x8) {
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                WriteMemory(addr, State.SP);
                AddCycles(0x5);
                return true;
            }

            /* LD-SP-HL */
            if((insnBytes[0] & 0xFF) == 0xF9) {
                pc += 1;
                State.SP = (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]);
                AddCycles(0x2);
                return true;
            }

            /* PUSH-rr */
            if((insnBytes[0] & 0xCF) == 0xC5) {
                var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
                pc += 1;
                var sp = (ushort) (State.SP - 0x2);
                State.SP = sp;
                WriteMemory(sp,
                    r switch {
                        0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                        0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                        0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                        _ => (ushort) ((State.Registers[0b111] << 8) | State.Flags),
                    });
                AddCycles(0x4);
                return true;
            }

            /* POP-rr */
            if((insnBytes[0] & 0xCF) == 0xC1) {
                var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
                pc += 1;
                var sp = State.SP;
                var v = ReadMemory<ushort>(sp);
                switch(r) {
                    case 0x0: {
                        var temp_44 = v;
                        State.Registers[0b000] = (byte) (temp_44 >> 8);
                        State.Registers[0b001] = (byte) (temp_44 & 0xFF);
                        break;
                    }
                    case 0x1: {
                        var temp_45 = v;
                        State.Registers[0b010] = (byte) (temp_45 >> 8);
                        State.Registers[0b011] = (byte) (temp_45 & 0xFF);
                        break;
                    }
                    case 0x2: {
                        var temp_46 = v;
                        State.Registers[0b100] = (byte) (temp_46 >> 8);
                        State.Registers[0b101] = (byte) (temp_46 & 0xFF);
                        break;
                    }
                    default: {
                        var temp_47 = v;
                        State.Registers[0b111] = (byte) (temp_47 >> 8);
                        State.Flags = (byte) (temp_47 & 0xF0);
                        break;
                    }
                }

                State.SP = (ushort) (sp + 0x2);
                AddCycles(0x4);
                return true;
            }

            /* JP-nn */
            if((insnBytes[0] & 0xFF) == 0xC3) {
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                Branch(addr);
                AddCycles(0x4);
                return true;
            }

            /* JP-HL */
            if((insnBytes[0] & 0xFF) == 0xE9) {
                pc += 1;
                Branch((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]));
                AddCycles(0x1);
                return true;
            }

            /* JP-cc-imm16 */
            if((insnBytes[0] & 0xE7) == 0xC2) {
                var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                if((byte) (cc >> 0x1) switch {
                       0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                       _ => (byte) ((byte) (State.Flags >> 0x4) &
                                    0x1) ==
                            (byte) (cc & 0x1),
                   })
                    Branch(addr);
                else
                    Branch(pc);
                AddCycles((byte) (cc >> 0x1) switch {
                    0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                    _ => (byte) ((byte) (State.Flags >> 0x4) &
                                 0x1) ==
                         (byte) (cc & 0x1),
                }
                    ? (byte) 0x4
                    : (byte) 0x3);
                return true;
            }

            /* JR-simm8 */
            if((insnBytes[0] & 0xFF) == 0x18) {
                var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var offset = (sbyte) e;
                pc += 2;
                Branch((ushort) (pc + (ushort) offset));
                AddCycles(0x3);
                return true;
            }

            /* JR-cc-simm8 */
            if((insnBytes[0] & 0xE7) == 0x20) {
                var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
                var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var offset = (sbyte) e;
                pc += 2;
                if((byte) (cc >> 0x1) switch {
                       0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                       _ => (byte) ((byte) (State.Flags >> 0x4) &
                                    0x1) ==
                            (byte) (cc & 0x1),
                   })
                    Branch((ushort) (pc + (ushort) offset));
                else
                    Branch(pc);
                AddCycles((byte) (cc >> 0x1) switch {
                    0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                    _ => (byte) ((byte) (State.Flags >> 0x4) &
                                 0x1) ==
                         (byte) (cc & 0x1),
                }
                    ? (byte) 0x3
                    : (byte) 0x2);
                return true;
            }

            /* CALL-imm16 */
            if((insnBytes[0] & 0xFF) == 0xCD) {
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                var sp = (ushort) (State.SP - 0x2);
                State.SP = sp;
                WriteMemory(sp, pc);
                Branch(addr);
                AddCycles(0x6);
                return true;
            }

            /* CALL-cc-imm16 */
            if((insnBytes[0] & 0xE7) == 0xC4) {
                var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
                var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
                var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
                pc += 3;
                if((byte) (cc >> 0x1) switch {
                       0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                       _ => (byte) ((byte) (State.Flags >> 0x4) &
                                    0x1) ==
                            (byte) (cc & 0x1),
                   }) {
                    var sp = (ushort) (State.SP - 0x2);
                    State.SP = sp;
                    WriteMemory(sp, pc);
                    Branch(addr);
                }
                else {
                    Branch(pc);
                }

                AddCycles((byte) (cc >> 0x1) switch {
                    0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                    _ => (byte) ((byte) (State.Flags >> 0x4) &
                                 0x1) ==
                         (byte) (cc & 0x1),
                }
                    ? (byte) 0x6
                    : (byte) 0x3);
                return true;
            }

            /* RET */
            if((insnBytes[0] & 0xFF) == 0xC9) {
                pc += 1;
                var sp = State.SP;
                var ra = ReadMemory<ushort>(sp);
                State.SP = (ushort) (sp + 0x2);
                Branch(ra);
                AddCycles(0x4);
                return true;
            }

            /* RET-cc */
            if((insnBytes[0] & 0xE7) == 0xC0) {
                var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
                pc += 1;
                if((byte) (cc >> 0x1) switch {
                       0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                       _ => (byte) ((byte) (State.Flags >> 0x4) &
                                    0x1) ==
                            (byte) (cc & 0x1),
                   }) {
                    var sp = State.SP;
                    var ra = ReadMemory<ushort>(sp);
                    State.SP = (ushort) (sp + 0x2);
                    Branch(ra);
                }
                else {
                    Branch(pc);
                }

                AddCycles((byte) (cc >> 0x1) switch {
                    0x0 => (byte) (State.Flags >> 0x7) == (byte) (cc & 0x1),
                    _ => (byte) ((byte) (State.Flags >> 0x4) &
                                 0x1) ==
                         (byte) (cc & 0x1),
                }
                    ? (byte) 0x5
                    : (byte) 0x2);
                return true;
            }

            /* RETI */
            if((insnBytes[0] & 0xFF) == 0xD9) {
                pc += 1;
                var sp = State.SP;
                var ra = ReadMemory<ushort>(sp);
                State.SP = (ushort) (sp + 0x2);
                State.InterruptsEnabled = true;
                Branch(ra);
                AddCycles(0x4);
                return true;
            }

            /* RST */
            if((insnBytes[0] & 0xC7) == 0xC7) {
                var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
                var addr = (byte) (n << 0x3);
                pc += 1;
                var sp = (ushort) (State.SP - 0x2);
                State.SP = sp;
                WriteMemory(sp, pc);
                Branch(addr);
                AddCycles(0x4);
                return true;
            }

            /* DI */
            if((insnBytes[0] & 0xFF) == 0xF3) {
                pc += 1;
                State.InterruptsEnabled = false;
                State.InterruptsEnableScheduled = false;
                AddCycles(0x1);
                return true;
            }

            /* EI */
            if((insnBytes[0] & 0xFF) == 0xFB) {
                pc += 1;
                State.InterruptsEnableScheduled = true;
                AddCycles(0x1);
                return true;
            }

            /* CCF */
            if((insnBytes[0] & 0xFF) == 0x3F) {
                pc += 1;
                var flags = State.Flags;
                flags = (byte) (flags & 0x9F);
                State.Flags = (byte) ((byte) (flags ^ 0x10) & 0xF0);
                AddCycles(0x1);
                return true;
            }

            /* SCF */
            if((insnBytes[0] & 0xFF) == 0x37) {
                pc += 1;
                var flags = State.Flags;
                flags = (byte) (flags & 0x8F);
                State.Flags = (byte) ((byte) (flags | 0x10) & 0xF0);
                AddCycles(0x1);
                return true;
            }

            /* CPL */
            if((insnBytes[0] & 0xFF) == 0x2F) {
                pc += 1;
                State.Flags = (byte) ((byte) (State.Flags | 0x60) & 0xF0);
                State.Registers[0x7] = (byte) ~(0x7 switch {
                    0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                });
                AddCycles(0x1);
                return true;
            }

            /* NOP */
            if((insnBytes[0] & 0xFF) == 0x0) {
                pc += 1;
                AddCycles(0x1);
                return true;
            }

            /* INC */
            if((insnBytes[0] & 0xC7) == 0x4) {
                var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
                pc += 1;
                var lhs = rd == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : rd switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) (lhs + 0x1);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0x1F) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (byte) (
                                                  (byte)
                                                  ((byte) ((byte) (lhs &
                                                                   0xF) +
                                                           (0x1 &
                                                            0xF)) >
                                                   0xF
                                                      ? 1U
                                                      : 0U) << 0x5)) & 0xF0);
                if(rd == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[rd] = result;
                AddCycles(rd == 0x6 ? (byte) 0x3 : (byte) 0x1);
                return true;
            }

            /* DEC */
            if((insnBytes[0] & 0xC7) == 0x5) {
                var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
                pc += 1;
                var lhs = rd == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : rd switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) (lhs - 0x1);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0x1F) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((byte) (lhs &
                                                                      0xF) < (0x1 & 0xF)
                                                  ? 1U
                                                  : 0U) << 0x5)) & 0xF0);
                if(rd == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[rd] = result;
                AddCycles(rd == 0x6 ? (byte) 0x3 : (byte) 0x1);
                return true;
            }

            /* INC-16 */
            if((insnBytes[0] & 0xCF) == 0x3) {
                var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
                pc += 1;
                switch(rd) {
                    case 0x0: {
                        var temp_56 = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } + 0x1);
                        State.Registers[0b000] = (byte) (temp_56 >> 8);
                        State.Registers[0b001] = (byte) (temp_56 & 0xFF);
                        break;
                    }
                    case 0x1: {
                        var temp_58 = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } + 0x1);
                        State.Registers[0b010] = (byte) (temp_58 >> 8);
                        State.Registers[0b011] = (byte) (temp_58 & 0xFF);
                        break;
                    }
                    case 0x2: {
                        var temp_60 = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } + 0x1);
                        State.Registers[0b100] = (byte) (temp_60 >> 8);
                        State.Registers[0b101] = (byte) (temp_60 & 0xFF);
                        break;
                    }
                    default: {
                        State.SP = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } + 0x1);
                        break;
                    }
                }

                AddCycles(0x2);
                return true;
            }

            /* DEC-16 */
            if((insnBytes[0] & 0xCF) == 0xB) {
                var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
                pc += 1;
                switch(rd) {
                    case 0x0: {
                        var temp_63 = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } - 0x1);
                        State.Registers[0b000] = (byte) (temp_63 >> 8);
                        State.Registers[0b001] = (byte) (temp_63 & 0xFF);
                        break;
                    }
                    case 0x1: {
                        var temp_65 = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } - 0x1);
                        State.Registers[0b010] = (byte) (temp_65 >> 8);
                        State.Registers[0b011] = (byte) (temp_65 & 0xFF);
                        break;
                    }
                    case 0x2: {
                        var temp_67 = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } - 0x1);
                        State.Registers[0b100] = (byte) (temp_67 >> 8);
                        State.Registers[0b101] = (byte) (temp_67 & 0xFF);
                        break;
                    }
                    default: {
                        State.SP = (ushort) (rd switch {
                            0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                            0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                            0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                            _ => State.SP,
                        } - 0x1);
                        break;
                    }
                }

                AddCycles(0x2);
                return true;
            }

            /* ADD-HL */
            if((insnBytes[0] & 0xCF) == 0x9) {
                var rs = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
                pc += 1;
                var lhs = (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]);
                var rhs = rs switch {
                    0x0 => (ushort) ((State.Registers[0b000] << 8) | State.Registers[0b001]),
                    0x1 => (ushort) ((State.Registers[0b010] << 8) | State.Registers[0b011]),
                    0x2 => (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                    _ => State.SP,
                };
                var result = lhs + (uint) rhs;
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0x8F) | (0x0 << 0x6) |
                                              (byte) ((byte) ((ushort) ((ushort) (lhs &
                                                                            0xFFF) +
                                                                        (ushort) (rhs &
                                                                            0xFFF)) >
                                                              0xFFF
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >>
                                                  0x10 != 0x0
                                                      ? 1U
                                                      : 0U) << 0x4)) & 0xF0);
                var temp_71 = (ushort) result;
                State.Registers[0b100] = (byte) (temp_71 >> 8);
                State.Registers[0b101] = (byte) (temp_71 & 0xFF);
                AddCycles(0x2);
                return true;
            }

            /* ADD-SP-r8 */
            if((insnBytes[0] & 0xFF) == 0xE8) {
                var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var imm = (sbyte) rimm;
                pc += 2;
                var sp = State.SP;
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) | (0x0 << 0x7) |
                                              (0x0 << 0x6) |
                                              (byte) (
                                                  (byte)
                                                  ((byte)
                                                   ((byte) ((byte) sp &
                                                            0xF) +
                                                    (byte) (rimm & 0xF)) >
                                                   0xF
                                                      ? 1U
                                                      : 0U) << 0x5) |
                                              (byte) ((byte)
                                                  ((ushort)
                                                      ((ushort) (sp &
                                                                 0xFF) +
                                                       rimm) >= 0x100
                                                          ? 1U
                                                          : 0U) << 0x4)) & 0xF0);
                State.SP = (ushort) (sp + (uint) imm);
                AddCycles(0x4);
                return true;
            }

            /* LD-HL-SP-r8 */
            if((insnBytes[0] & 0xFF) == 0xF8) {
                var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                var imm = (sbyte) rimm;
                pc += 2;
                var sp = State.SP;
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) | (0x0 << 0x7) |
                                              (0x0 << 0x6) |
                                              (byte) (
                                                  (byte)
                                                  ((byte)
                                                   ((byte) ((byte) sp &
                                                            0xF) +
                                                    (byte) (rimm & 0xF)) >
                                                   0xF
                                                      ? 1U
                                                      : 0U) << 0x5) |
                                              (byte) ((byte)
                                                  ((ushort)
                                                      ((ushort) (sp &
                                                                 0xFF) +
                                                       rimm) >= 0x100
                                                          ? 1U
                                                          : 0U) << 0x4)) & 0xF0);
                var temp_72 = (ushort) (sp + (uint) imm);
                State.Registers[0b100] = (byte) (temp_72 >> 8);
                State.Registers[0b101] = (byte) (temp_72 & 0xFF);
                AddCycles(0x3);
                return true;
            }

            /* ADD */
            if((insnBytes[0] & 0xF8) == 0x80) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var result = (ushort) (lhs + rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (byte) (
                                                  (byte)
                                                  ((byte) ((byte) (lhs &
                                                                   0xF) +
                                                           (byte) (rhs &
                                                                   0xF)) >
                                                   0xF
                                                      ? 1U
                                                      : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* ADD-imm8 */
            if((insnBytes[0] & 0xFF) == 0xC6) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var result = (ushort) (lhs + rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (byte) (
                                                  (byte)
                                                  ((byte) ((byte) (lhs &
                                                                   0xF) +
                                                           (byte) (rhs &
                                                                   0xF)) > 0xF
                                                      ? 1U
                                                      : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(0x2);
                return true;
            }

            /* ADC */
            if((insnBytes[0] & 0xF8) == 0x88) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var carry = (byte) ((byte) (State.Flags >> 0x4) & 0x1);
                var result = (ushort) ((ushort) (lhs + rhs) +
                                       carry);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (byte) ((byte) ((carry != 0
                                                  ? (byte) ((byte) (lhs &
                                                                    0xF) +
                                                            (byte) (rhs &
                                                                    0xF)) >=
                                                    0xF
                                                  : (byte) ((byte) (lhs &
                                                                    0xF) +
                                                            (byte) (rhs &
                                                                    0xF)) >
                                                    0xF)
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* ADC-imm8 */
            if((insnBytes[0] & 0xFF) == 0xCE) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var carry = (byte) ((byte) (State.Flags >> 0x4) & 0x1);
                var result = (ushort) ((ushort) (lhs + rhs) +
                                       carry);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (byte) ((byte) ((carry != 0
                                                  ? (byte) ((byte) (lhs &
                                                                    0xF) +
                                                            (byte) (rhs & 0xF)) >=
                                                    0xF
                                                  : (byte) ((byte) (lhs &
                                                                    0xF) +
                                                            (byte) (rhs & 0xF)) >
                                                    0xF)
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(0x2);
                return true;
            }

            /* SUB */
            if((insnBytes[0] & 0xF8) == 0x90) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var result = (ushort) (lhs - rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((byte) (lhs &
                                                                      0xF) < (byte) (rhs & 0xF)
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* SUB-imm8 */
            if((insnBytes[0] & 0xFF) == 0xD6) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var result = (ushort) (lhs - rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((byte) (lhs &
                                                                      0xF) < (byte) (rhs & 0xF)
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(0x2);
                return true;
            }

            /* SBC */
            if((insnBytes[0] & 0xF8) == 0x98) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var carry = (byte) ((byte) (State.Flags >> 0x4) & 0x1);
                var result = (ushort) ((ushort) (lhs - rhs) -
                                       carry);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((carry != 0
                                                  ? (byte) (lhs & 0xF) <
                                                    (byte)
                                                    ((byte) (rhs &
                                                             0xF) +
                                                     0x1)
                                                  : (byte) (lhs & 0xF) <
                                                    (byte) (rhs & 0xF))
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* SBC-imm8 */
            if((insnBytes[0] & 0xFF) == 0xDE) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var carry = (byte) ((byte) (State.Flags >> 0x4) & 0x1);
                var result = (ushort) ((ushort) (lhs - rhs) -
                                       carry);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((carry != 0
                                                  ? (byte) (lhs & 0xF) <
                                                    (byte)
                                                    ((byte) (rhs & 0xF) +
                                                     0x1)
                                                  : (byte) (lhs & 0xF) <
                                                    (byte) (rhs & 0xF))
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                State.Registers[0x7] = (byte) result;
                AddCycles(0x2);
                return true;
            }

            /* AND */
            if((insnBytes[0] & 0xF8) == 0xA0) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var result = (byte) (lhs & rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x1 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* AND-imm8 */
            if((insnBytes[0] & 0xFF) == 0xE6) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var result = (byte) (lhs & rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x1 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(0x2);
                return true;
            }

            /* XOR */
            if((insnBytes[0] & 0xF8) == 0xA8) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var result = (byte) (lhs ^ rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* XOR-imm8 */
            if((insnBytes[0] & 0xFF) == 0xEE) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var result = (byte) (lhs ^ rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(0x2);
                return true;
            }

            /* OR */
            if((insnBytes[0] & 0xF8) == 0xB0) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var result = (byte) (lhs | rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* OR-imm8 */
            if((insnBytes[0] & 0xFF) == 0xF6) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var result = (byte) (lhs | rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(0x2);
                return true;
            }

            /* CP */
            if((insnBytes[0] & 0xF8) == 0xB8) {
                var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
                pc += 1;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = rs switch {
                    0x0 => 0x0 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x1 => 0x1 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] },
                    0x2 => 0x2 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x3 => 0x3 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x4 => 0x4 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x5 => 0x5 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                    0x6 => ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                      State.Registers[0b101])),
                    _ => 0x7 switch {
                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                    },
                };
                var result = (ushort) (lhs - rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((byte) (lhs &
                                                                      0xF) < (byte) (rhs & 0xF)
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                AddCycles(rs == 0x6 ? (byte) 0x2 : (byte) 0x1);
                return true;
            }

            /* CP-imm8 */
            if((insnBytes[0] & 0xFF) == 0xFE) {
                var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
                pc += 2;
                var lhs = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var rhs = imm;
                var result = (ushort) (lhs - rhs);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) ((byte) result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x1 << 0x6) |
                                              (byte) ((byte) ((byte) (lhs &
                                                                      0xF) < (byte) (rhs & 0xF)
                                                  ? 1U
                                                  : 0U) << 0x5) |
                                              (byte) ((byte) (result >=
                                                              0x100
                                                  ? 1U
                                                  : 0U) << 0x4)) & 0xF0);
                AddCycles(0x2);
                return true;
            }

            /* RLCA */
            if((insnBytes[0] & 0xFF) == 0x7) {
                pc += 1;
                var a = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) | (0x0 << 0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (a >> 0x7) <<
                                                      0x4)) & 0xF0);
                State.Registers[0x7] = (byte) ((byte) (a << 0x1) | (byte) (a >> 0x7));
                AddCycles(0x1);
                return true;
            }

            /* RLA */
            if((insnBytes[0] & 0xFF) == 0x17) {
                pc += 1;
                var v = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v << 0x1) | (byte) ((byte) (State.Flags >> 0x4) & 0x1));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) | (0x0 << 0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v >> 0x7) <<
                                                      0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(0x1);
                return true;
            }

            /* RLC */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x0) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v << 0x1) | (byte) (v >> 0x7));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v >> 0x7) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* RL */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x10) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v << 0x1) | (byte) ((byte) (State.Flags >> 0x4) & 0x1));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v >> 0x7) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* RRCA */
            if((insnBytes[0] & 0xFF) == 0xF) {
                pc += 1;
                var a = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) | (0x0 << 0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (a & 0x1) <<
                                                      0x4)) & 0xF0);
                State.Registers[0x7] = (byte) ((byte) (a >> 0x1) | (byte) (a << 0x7));
                AddCycles(0x1);
                return true;
            }

            /* RRC */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x8) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v >> 0x1) | (byte) (v << 0x7));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v & 0x1) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* RRA */
            if((insnBytes[0] & 0xFF) == 0x1F) {
                pc += 1;
                var v = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v >> 0x1) |
                                     (byte) ((byte) ((byte) (State.Flags >> 0x4) & 0x1) << 0x7));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) | (0x0 << 0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v & 0x1) <<
                                                      0x4)) & 0xF0);
                State.Registers[0x7] = result;
                AddCycles(0x1);
                return true;
            }

            /* RR */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x18) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v >> 0x1) |
                                     (byte) ((byte) ((byte) (State.Flags >> 0x4) & 0x1) << 0x7));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v & 0x1) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* SLA */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x20) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) (v << 0x1);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v >> 0x7) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* SRA */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x28) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v >> 0x1) | (byte) (v & 0x80));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v & 0x1) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* SWAP */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x30) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) ((byte) (v >> 0x4) | (byte) (v << 0x4));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (0x0 << 0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* SRL */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x38) {
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                var v = reg == 0x6
                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                    : reg switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                var result = (byte) (v >> 0x1);
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (result == 0x0 ? 1U : 0U) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) ((byte) (v & 0x1) <<
                                                      0x4)) & 0xF0);
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]), result);
                else
                    State.Registers[reg] = result;
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* BIT */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x40) {
                var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0x1F) |
                                              (byte) ((byte) ((byte)
                                                              ((byte) ((reg == 0x6
                                                                  ? ReadMemory<byte>(
                                                                      (ushort) ((State.Registers[0b100] << 8) |
                                                                          State.Registers[0b101]))
                                                                  : reg switch {
                                                                      0b110 => throw new NotSupportedException(),
                                                                      { } i => State.Registers[i],
                                                                  }) >> bit) & 0x1) ^
                                                              0x1) <<
                                                      0x7) |
                                              (0x0 << 0x6) |
                                              (0x1 << 0x5)) & 0xF0);
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* RES */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x80) {
                var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                        (byte) ((reg == 0x6
                                    ? ReadMemory<byte>(
                                        (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                                    : reg switch {
                                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                                    }) &
                                (byte) ~(byte) (0x1 << bit)));
                else
                    State.Registers[reg] =
                        (byte) ((reg == 0x6
                                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                                 State.Registers[0b101]))
                                    : reg switch {
                                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                                    }) &
                                (byte) ~(byte) (0x1 << bit));
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* SET */
            if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0xC0) {
                var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
                var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
                pc += 2;
                if(reg == 0x6)
                    WriteMemory((ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]),
                        (byte) ((reg == 0x6
                                    ? ReadMemory<byte>(
                                        (ushort) ((State.Registers[0b100] << 8) | State.Registers[0b101]))
                                    : reg switch {
                                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                                    }) |
                                (byte) (0x1 << bit)));
                else
                    State.Registers[reg] =
                        (byte) ((reg == 0x6
                                    ? ReadMemory<byte>((ushort) ((State.Registers[0b100] << 8) |
                                                                 State.Registers[0b101]))
                                    : reg switch {
                                        0b110 => throw new NotSupportedException(), { } i => State.Registers[i],
                                    }) |
                                (byte) (0x1 << bit));
                AddCycles(reg == 0x6 ? (byte) 0x4 : (byte) 0x2);
                return true;
            }

            /* DAA */
            if((insnBytes[0] & 0xFF) == 0x27) {
                pc += 1;
                var n = (byte) ((byte) (State.Flags >> 0x6) & 0x1);
                var h = (byte) ((byte) (State.Flags >> 0x5) & 0x1);
                var c = (byte) ((byte) (State.Flags >> 0x4) & 0x1);
                var a = 0x7 switch { 0b110 => throw new NotSupportedException(), { } i => State.Registers[i] };
                State.Registers[0x7] = (byte) (a + (byte) (sbyte) (n != 0
                    ? (byte) (sbyte) ((sbyte) (c != 0 ? (byte) -0x60 : (byte) 0x0) +
                                      (sbyte) (h != 0
                                          ? (byte) -0x6
                                          : (byte) 0x0))
                    : (byte) (((byte) (c |
                                       (byte) (a > 0x99 ? 1U : 0U)) != 0
                                  ? FunctionalHelpers.Funcify(() => {
                                      c = 0x1;
                                      return (byte) 0x60;
                                  })()
                                  : 0x0) +
                              ((byte) (h |
                                       (byte) ((byte) (a &
                                                       0xF) > 0x9
                                           ? 1U
                                           : 0U)) != 0
                                  ? 0x6
                                  : 0x0))));
                State.Flags = (byte) ((byte) ((byte) (State.Flags & 0xF) |
                                              (byte) ((byte) (0x7 switch {
                                                  0b110 => throw new NotSupportedException(),
                                                  { } i => State.Registers[i],
                                              } == 0x0
                                                  ? 1U
                                                  : 0U) << 0x7) |
                                              (byte) (n << 0x6) |
                                              (0x0 << 0x5) |
                                              (byte) (c << 0x4)) & 0xF0);
                AddCycles(0x1);
                return true;
            }

            /* HALT */
            if((insnBytes[0] & 0xFF) == 0x76) {
                pc += 1;
                Halt();
                AddCycles(0x1);
                return true;
            }

            /* STOP */
            if((insnBytes[0] & 0xFF) == 0x10) {
                pc += 1;
                AddCycles(0x1);
                return true;
            }

            return false;
        }
    }
}