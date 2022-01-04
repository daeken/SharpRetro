// ReSharper disable CheckNamespace

namespace DamageCore;

public class Disassembler {
    public const int InstructionCount = 82 + 0;

    public static string Disassemble(Span<byte> insnBytes, ushort pc) {
        /* LD-rd-rs */
        if((insnBytes[0] & 0xC0) == 0x40) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            if(!((rd != 0x6) & (rs != 0x6)))
                goto insn_1;
            return "ld " +
                   rd switch {
                       0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E",
                       0x4 => "H", 0x5 => "L", 0x7 => "A",
                       _ => "(HL)",
                   } + ", " + rs switch {
                       0x0 => "B", 0x1 => "C", 0x2 => "D",
                       0x3 => "E", 0x4 => "H", 0x5 => "L",
                       0x7 => "A", _ => "(HL)",
                   };
        }

        insn_1:
        /* LD-rd-imm8 */
        if((insnBytes[0] & 0xC7) == 0x6) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            if(!(rd != 0x6))
                goto insn_2;
            return "ld " +
                   rd switch {
                       0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E",
                       0x4 => "H", 0x5 => "L", 0x7 => "A",
                       _ => "(HL)",
                   } + ", " + $"0x{imm:x02}";
        }

        insn_2:
        /* LD-rd-HL */
        if((insnBytes[0] & 0xC7) == 0x46) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            if(!(rd != 0x6))
                goto insn_3;
            return "ld " + rd switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            } + ", (HL)";
        }

        insn_3:
        /* LD-HL-rs */
        if((insnBytes[0] & 0xF8) == 0x70) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            if(!(rs != 0x6))
                goto insn_4;
            return "ld (HL), " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        insn_4:
        /* LD-HL-imm8 */
        if((insnBytes[0] & 0xFF) == 0x36) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "ld (HL), " + $"0x{imm:x02}";
        }

        /* LD-A-BC */
        if((insnBytes[0] & 0xFF) == 0xA) return "ld A, (BC)";
        /* LD-A-DE */
        if((insnBytes[0] & 0xFF) == 0x1A) return "ld A, (DE)";
        /* LD-BC-A */
        if((insnBytes[0] & 0xFF) == 0x2) return "ld (BC), A";
        /* LD-DE-A */
        if((insnBytes[0] & 0xFF) == 0x12) return "ld (DE), A";
        /* LD-A-imm16 */
        if((insnBytes[0] & 0xFF) == 0xFA) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "ld A, (" + $"0x{addr:x04}" + ")";
        }

        /* LD-imm16-A */
        if((insnBytes[0] & 0xFF) == 0xEA) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "ld (" + $"0x{addr:x04}" + "), A";
        }

        /* LDH-A-C */
        if((insnBytes[0] & 0xFF) == 0xF2) return "ldh A, (C)";
        /* LDH-C-A */
        if((insnBytes[0] & 0xFF) == 0xE2) return "ldh (C), A";
        /* LDH-A-imm8 */
        if((insnBytes[0] & 0xFF) == 0xF0) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var addr = (ushort) (0xFF00 | imm);
            return "ldh A, (" + $"0x{addr:x04}" + ")";
        }

        /* LDH-imm8-A */
        if((insnBytes[0] & 0xFF) == 0xE0) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var addr = (ushort) (0xFF00 | imm);
            return "ldh (" + $"0x{addr:x04}" + "), A";
        }

        /* LD-A-HL- */
        if((insnBytes[0] & 0xFF) == 0x3A) return "ld A, (HL-)";
        /* LD-HL--A */
        if((insnBytes[0] & 0xFF) == 0x32) return "ld (HL-), A";
        /* LD-A-HL+ */
        if((insnBytes[0] & 0xFF) == 0x2A) return "ld A, (HL+)";
        /* LD-HL+-A */
        if((insnBytes[0] & 0xFF) == 0x22) return "ld (HL+), A";
        /* LD-rr-imm16 */
        if((insnBytes[0] & 0xCF) == 0x1) {
            var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var imm = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "ld " + r switch { 0x0 => "BC", 0x1 => "DE", 0x2 => "HL", _ => "SP" } +
                   ", " + $"0x{imm:x04}";
        }

        /* LD-imm16-SP */
        if((insnBytes[0] & 0xFF) == 0x8) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "ld (" + $"0x{addr:x04}" + "), SP";
        }

        /* LD-SP-HL */
        if((insnBytes[0] & 0xFF) == 0xF9) return "ld SP, HL";
        /* PUSH-rr */
        if((insnBytes[0] & 0xCF) == 0xC5) {
            var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "push " + (r == 0x3
                ? "AF"
                : r switch {
                    0x0 => "BC", 0x1 => "DE", 0x2 => "HL", _ => "SP",
                });
        }

        /* POP-rr */
        if((insnBytes[0] & 0xCF) == 0xC1) {
            var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "pop " + (r == 0x3
                ? "AF"
                : r switch {
                    0x0 => "BC", 0x1 => "DE", 0x2 => "HL", _ => "SP",
                });
        }

        /* JP-nn */
        if((insnBytes[0] & 0xFF) == 0xC3) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "jp " + $"0x{addr:x04}";
        }

        /* JP-HL */
        if((insnBytes[0] & 0xFF) == 0xE9) return "jp HL";
        /* JP-cc-imm16 */
        if((insnBytes[0] & 0xE7) == 0xC2) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "jp " + cc switch { 0x0 => "NZ", 0x1 => "Z", 0x2 => "NC", _ => "C" } +
                   ", " + $"0x{addr:x04}";
        }

        /* JR-simm8 */
        if((insnBytes[0] & 0xFF) == 0x18) {
            var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var offset = (sbyte) e;
            return "jr " + $"0x{(ushort) (pc + (ushort) offset):x04}";
        }

        /* JR-cc-simm8 */
        if((insnBytes[0] & 0xE7) == 0x20) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var offset = (sbyte) e;
            return "jr " + cc switch { 0x0 => "NZ", 0x1 => "Z", 0x2 => "NC", _ => "C" } +
                   ", " +
                   $"0x{(ushort) (pc + (ushort) offset):x04}";
        }

        /* CALL-imm16 */
        if((insnBytes[0] & 0xFF) == 0xCD) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "call " + $"0x{addr:x04}";
        }

        /* CALL-cc-imm16 */
        if((insnBytes[0] & 0xE7) == 0xC4) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "call " + cc switch { 0x0 => "NZ", 0x1 => "Z", 0x2 => "NC", _ => "C" } +
                   ", " + $"0x{addr:x04}";
        }

        /* RET */
        if((insnBytes[0] & 0xFF) == 0xC9) return "ret";
        /* RET-cc */
        if((insnBytes[0] & 0xE7) == 0xC0) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            return "ret " + cc switch { 0x0 => "NZ", 0x1 => "Z", 0x2 => "NC", _ => "C" };
        }

        /* RETI */
        if((insnBytes[0] & 0xFF) == 0xD9) return "reti";
        /* RST */
        if((insnBytes[0] & 0xC7) == 0xC7) {
            var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            var addr = (byte) (n << 0x3);
            return "rst " + $"0x{addr:x00}";
        }

        /* DI */
        if((insnBytes[0] & 0xFF) == 0xF3) return "di";
        /* EI */
        if((insnBytes[0] & 0xFF) == 0xFB) return "ei";
        /* CCF */
        if((insnBytes[0] & 0xFF) == 0x3F) return "ccf";
        /* SCF */
        if((insnBytes[0] & 0xFF) == 0x37) return "scf";
        /* CPL */
        if((insnBytes[0] & 0xFF) == 0x2F) return "cpl";
        /* NOP */
        if((insnBytes[0] & 0xFF) == 0x0) return "nop";
        /* INC */
        if((insnBytes[0] & 0xC7) == 0x4) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            return "inc " + rd switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* DEC */
        if((insnBytes[0] & 0xC7) == 0x5) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            return "dec " + rd switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* INC-16 */
        if((insnBytes[0] & 0xCF) == 0x3) {
            var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "inc " + rd switch { 0x0 => "BC", 0x1 => "DE", 0x2 => "HL", _ => "SP" };
        }

        /* DEC-16 */
        if((insnBytes[0] & 0xCF) == 0xB) {
            var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "dec " + rd switch { 0x0 => "BC", 0x1 => "DE", 0x2 => "HL", _ => "SP" };
        }

        /* ADD-HL */
        if((insnBytes[0] & 0xCF) == 0x9) {
            var rs = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "add HL, " +
                   rs switch { 0x0 => "BC", 0x1 => "DE", 0x2 => "HL", _ => "SP" };
        }

        /* ADD-SP-r8 */
        if((insnBytes[0] & 0xFF) == 0xE8) {
            var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var imm = (sbyte) rimm;
            return "add SP, " + $"0x{imm:x02}";
        }

        /* LD-HL-SP-r8 */
        if((insnBytes[0] & 0xFF) == 0xF8) {
            var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var imm = (sbyte) rimm;
            return "ld HL, SP+" + $"0x{imm:x02}";
        }

        /* ADD */
        if((insnBytes[0] & 0xF8) == 0x80) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "add A," + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* ADD-imm8 */
        if((insnBytes[0] & 0xFF) == 0xC6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "add A," + " " + $"0x{imm:x02}";
        }

        /* ADC */
        if((insnBytes[0] & 0xF8) == 0x88) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "adc A," + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* ADC-imm8 */
        if((insnBytes[0] & 0xFF) == 0xCE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "adc A," + " " + $"0x{imm:x02}";
        }

        /* SUB */
        if((insnBytes[0] & 0xF8) == 0x90) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "sub" + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SUB-imm8 */
        if((insnBytes[0] & 0xFF) == 0xD6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "sub" + " " + $"0x{imm:x02}";
        }

        /* SBC */
        if((insnBytes[0] & 0xF8) == 0x98) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "sbc A," + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SBC-imm8 */
        if((insnBytes[0] & 0xFF) == 0xDE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "sbc A," + " " + $"0x{imm:x02}";
        }

        /* AND */
        if((insnBytes[0] & 0xF8) == 0xA0) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "and" + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* AND-imm8 */
        if((insnBytes[0] & 0xFF) == 0xE6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "and" + " " + $"0x{imm:x02}";
        }

        /* XOR */
        if((insnBytes[0] & 0xF8) == 0xA8) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "xor" + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* XOR-imm8 */
        if((insnBytes[0] & 0xFF) == 0xEE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "xor" + " " + $"0x{imm:x02}";
        }

        /* OR */
        if((insnBytes[0] & 0xF8) == 0xB0) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "or" + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* OR-imm8 */
        if((insnBytes[0] & 0xFF) == 0xF6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "or" + " " + $"0x{imm:x02}";
        }

        /* CP */
        if((insnBytes[0] & 0xF8) == 0xB8) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "cp" + " " + rs switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* CP-imm8 */
        if((insnBytes[0] & 0xFF) == 0xFE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "cp" + " " + $"0x{imm:x02}";
        }

        /* RLCA */
        if((insnBytes[0] & 0xFF) == 0x7) return "rlca";
        /* RLA */
        if((insnBytes[0] & 0xFF) == 0x17) return "rla";
        /* RLC */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x0) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "rlc " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* RL */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x10) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "rl " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* RRCA */
        if((insnBytes[0] & 0xFF) == 0xF) return "rrca";
        /* RRC */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x8) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "rrc " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* RRA */
        if((insnBytes[0] & 0xFF) == 0x1F) return "rra";
        /* RR */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x18) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "rr " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SLA */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x20) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "sla " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SRA */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x28) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "sra " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SWAP */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x30) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "swap " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SRL */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x38) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "srl " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* BIT */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x40) {
            var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "bit " + bit + ", " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* RES */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x80) {
            var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "res " + bit + ", " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* SET */
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0xC0) {
            var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "set " + bit + ", " + reg switch {
                0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H",
                0x5 => "L", 0x7 => "A", _ => "(HL)",
            };
        }

        /* DAA */
        if((insnBytes[0] & 0xFF) == 0x27) return "daa";
        /* HALT */
        if((insnBytes[0] & 0xFF) == 0x76) return "halt";
        /* STOP */
        if((insnBytes[0] & 0xFF) == 0x10) return "stop";

        return null;
    }

    public static string ClassifyInstruction(Span<byte> insnBytes) {
        if((insnBytes[0] & 0xC0) == 0x40) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            if(!((rd != 0x6) & (rs != 0x6)))
                goto insn_1;
            return "LD-rd-rs";
        }

        insn_1:
        if((insnBytes[0] & 0xC7) == 0x6) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            if(!(rd != 0x6))
                goto insn_2;
            return "LD-rd-imm8";
        }

        insn_2:
        if((insnBytes[0] & 0xC7) == 0x46) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            if(!(rd != 0x6))
                goto insn_3;
            return "LD-rd-HL";
        }

        insn_3:
        if((insnBytes[0] & 0xF8) == 0x70) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            if(!(rs != 0x6))
                goto insn_4;
            return "LD-HL-rs";
        }

        insn_4:
        if((insnBytes[0] & 0xFF) == 0x36) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "LD-HL-imm8";
        }

        if((insnBytes[0] & 0xFF) == 0xA) return "LD-A-BC";
        if((insnBytes[0] & 0xFF) == 0x1A) return "LD-A-DE";
        if((insnBytes[0] & 0xFF) == 0x2) return "LD-BC-A";
        if((insnBytes[0] & 0xFF) == 0x12) return "LD-DE-A";
        if((insnBytes[0] & 0xFF) == 0xFA) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "LD-A-imm16";
        }

        if((insnBytes[0] & 0xFF) == 0xEA) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "LD-imm16-A";
        }

        if((insnBytes[0] & 0xFF) == 0xF2) return "LDH-A-C";
        if((insnBytes[0] & 0xFF) == 0xE2) return "LDH-C-A";
        if((insnBytes[0] & 0xFF) == 0xF0) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var addr = (ushort) (0xFF00 | imm);
            return "LDH-A-imm8";
        }

        if((insnBytes[0] & 0xFF) == 0xE0) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var addr = (ushort) (0xFF00 | imm);
            return "LDH-imm8-A";
        }

        if((insnBytes[0] & 0xFF) == 0x3A) return "LD-A-HL-";
        if((insnBytes[0] & 0xFF) == 0x32) return "LD-HL--A";
        if((insnBytes[0] & 0xFF) == 0x2A) return "LD-A-HL+";
        if((insnBytes[0] & 0xFF) == 0x22) return "LD-HL+-A";
        if((insnBytes[0] & 0xCF) == 0x1) {
            var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var imm = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "LD-rr-imm16";
        }

        if((insnBytes[0] & 0xFF) == 0x8) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "LD-imm16-SP";
        }

        if((insnBytes[0] & 0xFF) == 0xF9) return "LD-SP-HL";
        if((insnBytes[0] & 0xCF) == 0xC5) {
            var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "PUSH-rr";
        }

        if((insnBytes[0] & 0xCF) == 0xC1) {
            var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "POP-rr";
        }

        if((insnBytes[0] & 0xFF) == 0xC3) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "JP-nn";
        }

        if((insnBytes[0] & 0xFF) == 0xE9) return "JP-HL";
        if((insnBytes[0] & 0xE7) == 0xC2) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "JP-cc-imm16";
        }

        if((insnBytes[0] & 0xFF) == 0x18) {
            var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var offset = (sbyte) e;
            return "JR-simm8";
        }

        if((insnBytes[0] & 0xE7) == 0x20) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var offset = (sbyte) e;
            return "JR-cc-simm8";
        }

        if((insnBytes[0] & 0xFF) == 0xCD) {
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "CALL-imm16";
        }

        if((insnBytes[0] & 0xE7) == 0xC4) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
            var addr = (ushort) ((ushort) (msb << 0x8) | lsb);
            return "CALL-cc-imm16";
        }

        if((insnBytes[0] & 0xFF) == 0xC9) return "RET";
        if((insnBytes[0] & 0xE7) == 0xC0) {
            var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
            return "RET-cc";
        }

        if((insnBytes[0] & 0xFF) == 0xD9) return "RETI";
        if((insnBytes[0] & 0xC7) == 0xC7) {
            var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            var addr = (byte) (n << 0x3);
            return "RST";
        }

        if((insnBytes[0] & 0xFF) == 0xF3) return "DI";
        if((insnBytes[0] & 0xFF) == 0xFB) return "EI";
        if((insnBytes[0] & 0xFF) == 0x3F) return "CCF";
        if((insnBytes[0] & 0xFF) == 0x37) return "SCF";
        if((insnBytes[0] & 0xFF) == 0x2F) return "CPL";
        if((insnBytes[0] & 0xFF) == 0x0) return "NOP";
        if((insnBytes[0] & 0xC7) == 0x4) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            return "INC";
        }

        if((insnBytes[0] & 0xC7) == 0x5) {
            var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
            return "DEC";
        }

        if((insnBytes[0] & 0xCF) == 0x3) {
            var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "INC-16";
        }

        if((insnBytes[0] & 0xCF) == 0xB) {
            var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "DEC-16";
        }

        if((insnBytes[0] & 0xCF) == 0x9) {
            var rs = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
            return "ADD-HL";
        }

        if((insnBytes[0] & 0xFF) == 0xE8) {
            var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var imm = (sbyte) rimm;
            return "ADD-SP-r8";
        }

        if((insnBytes[0] & 0xFF) == 0xF8) {
            var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            var imm = (sbyte) rimm;
            return "LD-HL-SP-r8";
        }

        if((insnBytes[0] & 0xF8) == 0x80) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "ADD";
        }

        if((insnBytes[0] & 0xFF) == 0xC6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "ADD-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0x88) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "ADC";
        }

        if((insnBytes[0] & 0xFF) == 0xCE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "ADC-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0x90) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "SUB";
        }

        if((insnBytes[0] & 0xFF) == 0xD6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "SUB-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0x98) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "SBC";
        }

        if((insnBytes[0] & 0xFF) == 0xDE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "SBC-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0xA0) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "AND";
        }

        if((insnBytes[0] & 0xFF) == 0xE6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "AND-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0xA8) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "XOR";
        }

        if((insnBytes[0] & 0xFF) == 0xEE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "XOR-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0xB0) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "OR";
        }

        if((insnBytes[0] & 0xFF) == 0xF6) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "OR-imm8";
        }

        if((insnBytes[0] & 0xF8) == 0xB8) {
            var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
            return "CP";
        }

        if((insnBytes[0] & 0xFF) == 0xFE) {
            var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
            return "CP-imm8";
        }

        if((insnBytes[0] & 0xFF) == 0x7) return "RLCA";
        if((insnBytes[0] & 0xFF) == 0x17) return "RLA";
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x0) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "RLC";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x10) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "RL";
        }

        if((insnBytes[0] & 0xFF) == 0xF) return "RRCA";
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x8) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "RRC";
        }

        if((insnBytes[0] & 0xFF) == 0x1F) return "RRA";
        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x18) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "RR";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x20) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "SLA";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x28) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "SRA";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x30) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "SWAP";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x38) {
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "SRL";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x40) {
            var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "BIT";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x80) {
            var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "RES";
        }

        if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0xC0) {
            var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
            var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
            return "SET";
        }

        if((insnBytes[0] & 0xFF) == 0x27) return "DAA";
        if((insnBytes[0] & 0xFF) == 0x76) return "HALT";
        if((insnBytes[0] & 0xFF) == 0x10) return "STOP";

        return null;
    }
}