// ReSharper disable CheckNamespace

namespace DamageCore;

public partial class Interpreter {
    public bool Interpret(Span<byte> insnBytes, ref ushort pc) {
        unchecked {
		/* LD-rd-rs */
		if((insnBytes[0] & 0xC0) == 0x40) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(!((bool) ((((bool) (((byte) (rd)) != ((byte) 0x6))) & ((bool) (((byte) (rs)) != ((byte) 0x6)))))))
				goto insn_1;
			pc += 1;
			State.Registers[(int) rd] = (byte) ((byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x1);
			return true;
		}
		insn_1:
		/* LD-rd-imm8 */
		if((insnBytes[0] & 0xC7) == 0x6) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			if(!((bool) (((byte) (rd)) != ((byte) 0x6))))
				goto insn_2;
			pc += 2;
			State.Registers[(int) rd] = (byte) (imm);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_2:
		/* LD-rd-HL */
		if((insnBytes[0] & 0xC7) == 0x46) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			if(!((bool) (((byte) (rd)) != ((byte) 0x6))))
				goto insn_3;
			pc += 1;
			State.Registers[(int) rd] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])))));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_3:
		/* LD-HL-rs */
		if((insnBytes[0] & 0xF8) == 0x70) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(!((bool) (((byte) (rs)) != ((byte) 0x6))))
				goto insn_4;
			pc += 1;
			WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), (byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_4:
		/* LD-HL-imm8 */
		if((insnBytes[0] & 0xFF) == 0x36) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), imm);
			AddCycles((byte) 0x3);
			return true;
		}
		insn_5:
		/* LD-A-BC */
		if((insnBytes[0] & 0xFF) == 0xA) {
			pc += 1;
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])))));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_6:
		/* LD-A-DE */
		if((insnBytes[0] & 0xFF) == 0x1A) {
			pc += 1;
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])))));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_7:
		/* LD-BC-A */
		if((insnBytes[0] & 0xFF) == 0x2) {
			pc += 1;
			WriteMemory((ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_8:
		/* LD-DE-A */
		if((insnBytes[0] & 0xFF) == 0x12) {
			pc += 1;
			WriteMemory((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])), (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_9:
		/* LD-A-imm16 */
		if((insnBytes[0] & 0xFF) == 0xFA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>(addr)));
			AddCycles((byte) 0x4);
			return true;
		}
		insn_10:
		/* LD-imm16-A */
		if((insnBytes[0] & 0xFF) == 0xEA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			WriteMemory(addr, (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x4);
			return true;
		}
		insn_11:
		/* LDH-A-C */
		if((insnBytes[0] & 0xFF) == 0xF2) {
			pc += 1;
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>((ushort) ((((ushort) ((ushort) ((ushort) 0xFF00))) | ((ushort) ((ushort) ((byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))))))));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_12:
		/* LDH-C-A */
		if((insnBytes[0] & 0xFF) == 0xE2) {
			pc += 1;
			WriteMemory((ushort) ((((ushort) ((ushort) ((ushort) 0xFF00))) | ((ushort) ((ushort) ((byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))))), (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_13:
		/* LDH-A-imm8 */
		if((insnBytes[0] & 0xFF) == 0xF0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) 0xFF00)) | ((ushort) ((ushort) (imm)))));
			pc += 2;
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>(addr)));
			AddCycles((byte) 0x3);
			return true;
		}
		insn_14:
		/* LDH-imm8-A */
		if((insnBytes[0] & 0xFF) == 0xE0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) 0xFF00)) | ((ushort) ((ushort) (imm)))));
			pc += 2;
			WriteMemory(addr, (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			AddCycles((byte) 0x3);
			return true;
		}
		insn_15:
		/* LD-A-HL- */
		if((insnBytes[0] & 0xFF) == 0x3A) {
			pc += 1;
			var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>(hl)));
			var temp_36 = (ushort) (ushort) (((ushort) (ushort) ((ushort) (hl))) - ((ushort) (byte) ((byte) ((byte) 0x1))));
			State.Registers[0b100] = (byte) (temp_36 >> 8);
			State.Registers[0b101] = (byte) (temp_36 & 0xFF);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_16:
		/* LD-HL--A */
		if((insnBytes[0] & 0xFF) == 0x32) {
			pc += 1;
			var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
			WriteMemory(hl, (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			var temp_37 = (ushort) (ushort) (((ushort) (ushort) ((ushort) (hl))) - ((ushort) (byte) ((byte) ((byte) 0x1))));
			State.Registers[0b100] = (byte) (temp_37 >> 8);
			State.Registers[0b101] = (byte) (temp_37 & 0xFF);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_17:
		/* LD-A-HL+ */
		if((insnBytes[0] & 0xFF) == 0x2A) {
			pc += 1;
			var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (ReadMemory<byte>(hl)));
			var temp_38 = (ushort) (ushort) (((ushort) (ushort) ((ushort) (hl))) + ((ushort) (byte) ((byte) ((byte) 0x1))));
			State.Registers[0b100] = (byte) (temp_38 >> 8);
			State.Registers[0b101] = (byte) (temp_38 & 0xFF);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_18:
		/* LD-HL+-A */
		if((insnBytes[0] & 0xFF) == 0x22) {
			pc += 1;
			var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
			WriteMemory(hl, (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
			var temp_39 = (ushort) (ushort) (((ushort) (ushort) ((ushort) (hl))) + ((ushort) (byte) ((byte) ((byte) 0x1))));
			State.Registers[0b100] = (byte) (temp_39 >> 8);
			State.Registers[0b101] = (byte) (temp_39 & 0xFF);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_19:
		/* LD-rr-imm16 */
		if((insnBytes[0] & 0xCF) == 0x1) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var imm = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			switch(r) {
				case (byte) ((byte) 0x0): {
					var temp_40 = (ushort) imm;
					State.Registers[0b000] = (byte) (temp_40 >> 8);
					State.Registers[0b001] = (byte) (temp_40 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x1): {
					var temp_41 = (ushort) imm;
					State.Registers[0b010] = (byte) (temp_41 >> 8);
					State.Registers[0b011] = (byte) (temp_41 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x2): {
					var temp_42 = (ushort) imm;
					State.Registers[0b100] = (byte) (temp_42 >> 8);
					State.Registers[0b101] = (byte) (temp_42 & 0xFF);
					break;
				}
				default: {
					State.SP = (ushort) imm;
					break;
				}
			}
			AddCycles((byte) 0x3);
			return true;
		}
		insn_20:
		/* LD-imm16-SP */
		if((insnBytes[0] & 0xFF) == 0x8) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			WriteMemory(addr, (ushort) (State.SP));
			AddCycles((byte) 0x5);
			return true;
		}
		insn_21:
		/* LD-SP-HL */
		if((insnBytes[0] & 0xFF) == 0xF9) {
			pc += 1;
			State.SP = (ushort) (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_22:
		/* PUSH-rr */
		if((insnBytes[0] & 0xCF) == 0xC5) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			var sp = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (State.SP)))) - ((ushort) (byte) ((byte) ((byte) 0x2))));
			State.SP = (ushort) sp;
			WriteMemory(sp, (ushort) (r switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (((((ushort) State.Registers[0b111]) << 8) | (ushort) State.Flags))) }));
			AddCycles((byte) 0x4);
			return true;
		}
		insn_23:
		/* POP-rr */
		if((insnBytes[0] & 0xCF) == 0xC1) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			var sp = (ushort) (State.SP);
			var v = (ushort) (ReadMemory<ushort>(sp));
			switch(r) {
				case (byte) ((byte) 0x0): {
					var temp_44 = (ushort) v;
					State.Registers[0b000] = (byte) (temp_44 >> 8);
					State.Registers[0b001] = (byte) (temp_44 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x1): {
					var temp_45 = (ushort) v;
					State.Registers[0b010] = (byte) (temp_45 >> 8);
					State.Registers[0b011] = (byte) (temp_45 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x2): {
					var temp_46 = (ushort) v;
					State.Registers[0b100] = (byte) (temp_46 >> 8);
					State.Registers[0b101] = (byte) (temp_46 & 0xFF);
					break;
				}
				default: {
					var temp_47 = (ushort) v;
					State.Registers[0b111] = (byte) (temp_47 >> 8);
					State.Flags = (byte) (temp_47 & 0xF0);
					break;
				}
			}
			State.SP = (ushort) (ushort) (((ushort) (ushort) ((ushort) (sp))) + ((ushort) (byte) ((byte) ((byte) 0x2))));
			AddCycles((byte) 0x4);
			return true;
		}
		insn_24:
		/* JP-nn */
		if((insnBytes[0] & 0xFF) == 0xC3) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			Branch(addr);
			AddCycles((byte) 0x4);
			return true;
		}
		insn_25:
		/* JP-HL */
		if((insnBytes[0] & 0xFF) == 0xE9) {
			pc += 1;
			Branch((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])));
			AddCycles((byte) 0x1);
			return true;
		}
		insn_26:
		/* JP-cc-imm16 */
		if((insnBytes[0] & 0xE7) == 0xC2) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			if((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) {
				Branch(addr);
			} else {
				Branch(pc);
			}
			AddCycles((byte) (((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x3)));
			return true;
		}
		insn_27:
		/* JR-simm8 */
		if((insnBytes[0] & 0xFF) == 0x18) {
			var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var offset = (sbyte) ((sbyte) (e));
			pc += 2;
			Branch((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset))));
			AddCycles((byte) 0x3);
			return true;
		}
		insn_28:
		/* JR-cc-simm8 */
		if((insnBytes[0] & 0xE7) == 0x20) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var offset = (sbyte) ((sbyte) (e));
			pc += 2;
			if((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) {
				Branch((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset))));
			} else {
				Branch(pc);
			}
			AddCycles((byte) (((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) ? (byte) ((byte) 0x3) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_29:
		/* CALL-imm16 */
		if((insnBytes[0] & 0xFF) == 0xCD) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			var sp = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (State.SP)))) - ((ushort) (byte) ((byte) ((byte) 0x2))));
			State.SP = (ushort) sp;
			WriteMemory(sp, (ushort) (pc));
			Branch(addr);
			AddCycles((byte) 0x6);
			return true;
		}
		insn_30:
		/* CALL-cc-imm16 */
		if((insnBytes[0] & 0xE7) == 0xC4) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) (((ushort) ((ushort) (msb))) << (int) ((byte) 0x8))) | ((ushort) ((ushort) (lsb)))));
			pc += 3;
			if((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) {
				var sp = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (State.SP)))) - ((ushort) (byte) ((byte) ((byte) 0x2))));
				State.SP = (ushort) sp;
				WriteMemory(sp, (ushort) (pc));
				Branch(addr);
			} else {
				Branch(pc);
			}
			AddCycles((byte) (((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) ? (byte) ((byte) 0x6) : (byte) ((byte) 0x3)));
			return true;
		}
		insn_31:
		/* RET */
		if((insnBytes[0] & 0xFF) == 0xC9) {
			pc += 1;
			var sp = (ushort) (State.SP);
			var ra = (ushort) (ReadMemory<ushort>(sp));
			State.SP = (ushort) (ushort) (((ushort) (ushort) ((ushort) (sp))) + ((ushort) (byte) ((byte) ((byte) 0x2))));
			Branch(ra);
			AddCycles((byte) 0x4);
			return true;
		}
		insn_32:
		/* RET-cc */
		if((insnBytes[0] & 0xE7) == 0xC0) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			pc += 1;
			if((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) {
				var sp = (ushort) (State.SP);
				var ra = (ushort) (ReadMemory<ushort>(sp));
				State.SP = (ushort) (ushort) (((ushort) (ushort) ((ushort) (sp))) + ((ushort) (byte) ((byte) ((byte) 0x2))));
				Branch(ra);
			} else {
				Branch(pc);
			}
			AddCycles((byte) (((bool) ((byte) ((cc) >> (int) ((byte) 0x1)) switch { (byte) ((byte) 0x0) => (bool) (((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x7)))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))), _ => (bool) (((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1)))))) == ((byte) ((byte) ((byte) (((cc) & ((byte) ((byte) ((byte) 0x1))))))))) })) ? (byte) ((byte) 0x5) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_33:
		/* RETI */
		if((insnBytes[0] & 0xFF) == 0xD9) {
			pc += 1;
			var sp = (ushort) (State.SP);
			var ra = (ushort) (ReadMemory<ushort>(sp));
			State.SP = (ushort) (ushort) (((ushort) (ushort) ((ushort) (sp))) + ((ushort) (byte) ((byte) ((byte) 0x2))));
			State.InterruptsEnabled = true;
			Branch(ra);
			AddCycles((byte) 0x4);
			return true;
		}
		insn_34:
		/* RST */
		if((insnBytes[0] & 0xC7) == 0xC7) {
			var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var addr = (byte) ((n) << (int) ((byte) 0x3));
			pc += 1;
			var sp = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (State.SP)))) - ((ushort) (byte) ((byte) ((byte) 0x2))));
			State.SP = (ushort) sp;
			WriteMemory(sp, (ushort) (pc));
			Branch(addr);
			AddCycles((byte) 0x4);
			return true;
		}
		insn_35:
		/* DI */
		if((insnBytes[0] & 0xFF) == 0xF3) {
			pc += 1;
			State.InterruptsEnabled = false;
			State.InterruptsEnableScheduled = false;
			AddCycles((byte) 0x1);
			return true;
		}
		insn_36:
		/* EI */
		if((insnBytes[0] & 0xFF) == 0xFB) {
			pc += 1;
			State.InterruptsEnableScheduled = true;
			AddCycles((byte) 0x1);
			return true;
		}
		insn_37:
		/* CCF */
		if((insnBytes[0] & 0xFF) == 0x3F) {
			pc += 1;
			var flags = (byte) (State.Flags);
			flags = (byte) ((((byte) (flags)) & ((byte) ((byte) 0x9F))));
			State.Flags = (byte) ((byte) ((((byte) (flags)) ^ ((byte) ((byte) 0x10)))) & 0xF0);
			AddCycles((byte) 0x1);
			return true;
		}
		insn_38:
		/* SCF */
		if((insnBytes[0] & 0xFF) == 0x37) {
			pc += 1;
			var flags = (byte) (State.Flags);
			flags = (byte) ((((byte) (flags)) & ((byte) ((byte) 0x8F))));
			State.Flags = (byte) ((byte) ((((byte) (flags)) | ((byte) ((byte) 0x10)))) & 0xF0);
			AddCycles((byte) 0x1);
			return true;
		}
		insn_39:
		/* CPL */
		if((insnBytes[0] & 0xFF) == 0x2F) {
			pc += 1;
			State.Flags = (byte) ((byte) ((((byte) ((byte) (State.Flags))) | ((byte) ((byte) 0x60)))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (~((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))));
			AddCycles((byte) 0x1);
			return true;
		}
		insn_40:
		/* NOP */
		if((insnBytes[0] & 0xFF) == 0x0) {
			pc += 1;
			AddCycles((byte) 0x1);
			return true;
		}
		insn_41:
		/* INC */
		if((insnBytes[0] & 0xC7) == 0x4) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			pc += 1;
			var lhs = (byte) (((bool) (((byte) (rd)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((rd) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) (((byte) (byte) ((byte) (lhs))) + ((byte) (byte) ((byte) ((byte) 0x1))));
			State.Flags = (byte) ((byte) ((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0x1F)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) ((((byte) 0x1) & ((byte) 0xF))))))))) > ((byte) ((byte) 0xF)))) ? 1U : 0U))) << (int) ((byte) 0x5)))))) & 0xF0);
			if((bool) (((byte) (rd)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) rd] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (rd)) == ((byte) 0x6))) ? (byte) ((byte) 0x3) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_42:
		/* DEC */
		if((insnBytes[0] & 0xC7) == 0x5) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			pc += 1;
			var lhs = (byte) (((bool) (((byte) (rd)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((rd) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) (((byte) (byte) ((byte) (lhs))) - ((byte) (byte) ((byte) ((byte) 0x1))));
			State.Flags = (byte) ((byte) ((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0x1F)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) ((((byte) 0x1) & ((byte) 0xF))))))) ? 1U : 0U))) << (int) ((byte) 0x5)))))) & 0xF0);
			if((bool) (((byte) (rd)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) rd] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (rd)) == ((byte) 0x6))) ? (byte) ((byte) 0x3) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_43:
		/* INC-16 */
		if((insnBytes[0] & 0xCF) == 0x3) {
			var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			switch(rd) {
				case (byte) ((byte) 0x0): {
					var temp_56 = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) + ((ushort) (byte) ((byte) ((byte) 0x1))));
					State.Registers[0b000] = (byte) (temp_56 >> 8);
					State.Registers[0b001] = (byte) (temp_56 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x1): {
					var temp_58 = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) + ((ushort) (byte) ((byte) ((byte) 0x1))));
					State.Registers[0b010] = (byte) (temp_58 >> 8);
					State.Registers[0b011] = (byte) (temp_58 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x2): {
					var temp_60 = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) + ((ushort) (byte) ((byte) ((byte) 0x1))));
					State.Registers[0b100] = (byte) (temp_60 >> 8);
					State.Registers[0b101] = (byte) (temp_60 & 0xFF);
					break;
				}
				default: {
					State.SP = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) + ((ushort) (byte) ((byte) ((byte) 0x1))));
					break;
				}
			}
			AddCycles((byte) 0x2);
			return true;
		}
		insn_44:
		/* DEC-16 */
		if((insnBytes[0] & 0xCF) == 0xB) {
			var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			switch(rd) {
				case (byte) ((byte) 0x0): {
					var temp_63 = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) - ((ushort) (byte) ((byte) ((byte) 0x1))));
					State.Registers[0b000] = (byte) (temp_63 >> 8);
					State.Registers[0b001] = (byte) (temp_63 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x1): {
					var temp_65 = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) - ((ushort) (byte) ((byte) ((byte) 0x1))));
					State.Registers[0b010] = (byte) (temp_65 >> 8);
					State.Registers[0b011] = (byte) (temp_65 & 0xFF);
					break;
				}
				case (byte) ((byte) 0x2): {
					var temp_67 = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) - ((ushort) (byte) ((byte) ((byte) 0x1))));
					State.Registers[0b100] = (byte) (temp_67 >> 8);
					State.Registers[0b101] = (byte) (temp_67 & 0xFF);
					break;
				}
				default: {
					State.SP = (ushort) (ushort) (((ushort) (ushort) ((ushort) ((ushort) (rd switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) })))) - ((ushort) (byte) ((byte) ((byte) 0x1))));
					break;
				}
			}
			AddCycles((byte) 0x2);
			return true;
		}
		insn_45:
		/* ADD-HL */
		if((insnBytes[0] & 0xCF) == 0x9) {
			var rs = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			var lhs = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
			var rhs = (ushort) (rs switch { (byte) ((byte) 0x0) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((byte) 0x1) => (ushort) ((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011]))), (byte) ((byte) 0x2) => (ushort) ((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))), _ => (ushort) ((ushort) (State.SP)) });
			var result = (uint) (((uint) (uint) ((uint) ((uint) ((uint) (lhs))))) + ((uint) (uint) ((uint) ((uint) ((uint) (rhs))))));
			State.Flags = (byte) ((byte) ((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0x8F)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((((ushort) (lhs)) & ((ushort) ((ushort) 0xFFF))))))) + ((ushort) (ushort) ((ushort) ((ushort) ((((ushort) (rhs)) & ((ushort) ((ushort) 0xFFF)))))))))) > ((ushort) ((ushort) 0xFFF)))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((uint) ((uint) ((result) >> (int) ((byte) 0x10)))) != ((uint) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			var temp_71 = (ushort) (ushort) ((ushort) (result));
			State.Registers[0b100] = (byte) (temp_71 >> 8);
			State.Registers[0b101] = (byte) (temp_71 & 0xFF);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_46:
		/* ADD-SP-r8 */
		if((insnBytes[0] & 0xFF) == 0xE8) {
			var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var imm = (sbyte) ((sbyte) (rimm));
			pc += 2;
			var sp = (ushort) (State.SP);
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) ((byte) ((byte) (sp)))) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) (((rimm) & ((byte) 0xF))))))))) > ((byte) ((byte) 0xF)))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((((ushort) (sp)) & ((ushort) ((ushort) ((byte) ((byte) 0xFF))))))))) + ((ushort) (byte) ((byte) (rimm)))))) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.SP = (ushort) (ushort) ((ushort) ((uint) (((uint) (uint) ((uint) ((uint) ((uint) (sp))))) + ((uint) (uint) ((uint) ((uint) ((uint) ((int) ((int) (imm))))))))));
			AddCycles((byte) 0x4);
			return true;
		}
		insn_47:
		/* LD-HL-SP-r8 */
		if((insnBytes[0] & 0xFF) == 0xF8) {
			var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var imm = (sbyte) ((sbyte) (rimm));
			pc += 2;
			var sp = (ushort) (State.SP);
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) ((byte) ((byte) (sp)))) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) (((rimm) & ((byte) 0xF))))))))) > ((byte) ((byte) 0xF)))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((((ushort) (sp)) & ((ushort) ((ushort) ((byte) ((byte) 0xFF))))))))) + ((ushort) (byte) ((byte) (rimm)))))) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			var temp_72 = (ushort) (ushort) ((ushort) ((uint) (((uint) (uint) ((uint) ((uint) ((uint) (sp))))) + ((uint) (uint) ((uint) ((uint) ((uint) ((int) ((int) (imm))))))))));
			State.Registers[0b100] = (byte) (temp_72 >> 8);
			State.Registers[0b101] = (byte) (temp_72 & 0xFF);
			AddCycles((byte) 0x3);
			return true;
		}
		insn_48:
		/* ADD */
		if((insnBytes[0] & 0xF8) == 0x80) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) + ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF)))))))))) > ((byte) ((byte) 0xF)))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_49:
		/* ADD-imm8 */
		if((insnBytes[0] & 0xFF) == 0xC6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) + ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) (((rhs) & ((byte) 0xF))))))))) > ((byte) ((byte) 0xF)))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_50:
		/* ADC */
		if((insnBytes[0] & 0xF8) == 0x88) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var carry = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))));
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) + ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))))))) + ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (carry))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((bool) (((byte) (carry)) != ((byte) ((byte) 0x0)))) ? (bool) ((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF)))))))))) >= ((byte) ((byte) 0xF)))) : (bool) ((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF)))))))))) > ((byte) ((byte) 0xF)))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_51:
		/* ADC-imm8 */
		if((insnBytes[0] & 0xFF) == 0xCE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var carry = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))));
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) + ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))))))) + ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (carry))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((bool) (((byte) (carry)) != ((byte) ((byte) 0x0)))) ? (bool) ((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) (((rhs) & ((byte) 0xF))))))))) >= ((byte) ((byte) 0xF)))) : (bool) ((bool) (((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) (((rhs) & ((byte) 0xF))))))))) > ((byte) ((byte) 0xF)))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_52:
		/* SUB */
		if((insnBytes[0] & 0xF8) == 0x90) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF)))))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_53:
		/* SUB-imm8 */
		if((insnBytes[0] & 0xFF) == 0xD6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) (((rhs) & ((byte) 0xF))))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_54:
		/* SBC */
		if((insnBytes[0] & 0xF8) == 0x98) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var carry = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))));
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (carry))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((bool) (((byte) (carry)) != ((byte) ((byte) 0x0)))) ? (bool) ((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) (((byte) (byte) ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF))))))) + ((byte) (byte) ((byte) ((byte) 0x1)))))))) : (bool) ((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF)))))))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_55:
		/* SBC-imm8 */
		if((insnBytes[0] & 0xFF) == 0xDE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var carry = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))));
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (carry))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((bool) (((byte) (carry)) != ((byte) ((byte) 0x0)))) ? (bool) ((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) (((byte) (byte) ((byte) (((rhs) & ((byte) 0xF))))) + ((byte) (byte) ((byte) 0x1))))))) : (bool) ((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) (((rhs) & ((byte) 0xF))))))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((byte) (result)));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_56:
		/* AND */
		if((insnBytes[0] & 0xF8) == 0xA0) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var result = (byte) ((((byte) (lhs)) & ((byte) (rhs))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (result));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_57:
		/* AND-imm8 */
		if((insnBytes[0] & 0xFF) == 0xE6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var result = (byte) ((((byte) (lhs)) & ((byte) (rhs))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (result));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_58:
		/* XOR */
		if((insnBytes[0] & 0xF8) == 0xA8) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var result = (byte) ((((byte) (lhs)) ^ ((byte) (rhs))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (result));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_59:
		/* XOR-imm8 */
		if((insnBytes[0] & 0xFF) == 0xEE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var result = (byte) ((((byte) (lhs)) ^ ((byte) (rhs))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (result));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_60:
		/* OR */
		if((insnBytes[0] & 0xF8) == 0xB0) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var result = (byte) ((((byte) (lhs)) | ((byte) (rhs))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (result));
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_61:
		/* OR-imm8 */
		if((insnBytes[0] & 0xFF) == 0xF6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var result = (byte) ((((byte) (lhs)) | ((byte) (rhs))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (result));
			AddCycles((byte) 0x2);
			return true;
		}
		insn_62:
		/* CP */
		if((insnBytes[0] & 0xF8) == 0xB8) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = (byte) (rs switch { (byte) ((byte) 0x0) => (byte) (((byte) 0x0) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) ((byte) 0x1) => (byte) ((byte) (((byte) 0x1) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x2) => (byte) ((byte) (((byte) 0x2) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x3) => (byte) ((byte) (((byte) 0x3) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x4) => (byte) ((byte) (((byte) 0x4) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x5) => (byte) ((byte) (((byte) 0x5) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })), (byte) ((byte) 0x6) => (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))), _ => (byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })) });
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) ((((byte) (rhs)) & ((byte) ((byte) 0xF)))))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			AddCycles((byte) (((bool) (((byte) (rs)) == ((byte) 0x6))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x1)));
			return true;
		}
		insn_63:
		/* CP-imm8 */
		if((insnBytes[0] & 0xFF) == 0xFE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			var lhs = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var rhs = imm;
			var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) ((ushort) (lhs))))) - ((ushort) (ushort) ((ushort) ((ushort) ((ushort) (rhs))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (result)))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (lhs)) & ((byte) ((byte) 0xF)))))) < ((byte) ((byte) (((rhs) & ((byte) 0xF))))))) ? 1U : 0U))) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((ushort) (result)) >= ((ushort) ((ushort) 0x100)))) ? 1U : 0U))) << (int) ((byte) 0x4)))))) & 0xF0);
			AddCycles((byte) 0x2);
			return true;
		}
		insn_64:
		/* RLCA */
		if((insnBytes[0] & 0xFF) == 0x7) {
			pc += 1;
			var a = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((a) >> (int) ((byte) 0x7)))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((((byte) ((byte) ((a) << (int) ((byte) 0x1)))) | ((byte) ((byte) ((a) >> (int) ((byte) 0x7)))))));
			AddCycles((byte) 0x1);
			return true;
		}
		insn_65:
		/* RLA */
		if((insnBytes[0] & 0xFF) == 0x17) {
			pc += 1;
			var v = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var result = (byte) ((((byte) ((byte) ((v) << (int) ((byte) 0x1)))) | ((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((v) >> (int) ((byte) 0x7)))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) (result);
			AddCycles((byte) 0x1);
			return true;
		}
		insn_66:
		/* RLC */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x0) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((((byte) ((byte) ((v) << (int) ((byte) 0x1)))) | ((byte) ((byte) ((v) >> (int) ((byte) 0x7))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((v) >> (int) ((byte) 0x7)))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_67:
		/* RL */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x10) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((((byte) ((byte) ((v) << (int) ((byte) 0x1)))) | ((byte) ((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((v) >> (int) ((byte) 0x7)))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_68:
		/* RRCA */
		if((insnBytes[0] & 0xFF) == 0xF) {
			pc += 1;
			var a = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) (a)) & ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) ((((byte) ((byte) ((a) >> (int) ((byte) 0x1)))) | ((byte) ((byte) ((a) << (int) ((byte) 0x7)))))));
			AddCycles((byte) 0x1);
			return true;
		}
		insn_69:
		/* RRC */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x8) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((((byte) ((byte) ((v) >> (int) ((byte) 0x1)))) | ((byte) ((byte) ((v) << (int) ((byte) 0x7))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) (v)) & ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_70:
		/* RRA */
		if((insnBytes[0] & 0xFF) == 0x1F) {
			pc += 1;
			var v = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			var result = (byte) ((((byte) ((byte) ((v) >> (int) ((byte) 0x1)))) | ((byte) ((byte) (((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))))) << (int) ((byte) 0x7))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) (v)) & ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x4)))))) & 0xF0);
			State.Registers[(int) (byte) 0x7] = (byte) (result);
			AddCycles((byte) 0x1);
			return true;
		}
		insn_71:
		/* RR */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x18) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((((byte) ((byte) ((v) >> (int) ((byte) 0x1)))) | ((byte) ((byte) (((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))))) << (int) ((byte) 0x7))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) (v)) & ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_72:
		/* SLA */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x20) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((v) << (int) ((byte) 0x1));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((v) >> (int) ((byte) 0x7)))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_73:
		/* SRA */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x28) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((((byte) ((byte) ((v) >> (int) ((byte) 0x1)))) | ((byte) ((byte) ((((byte) (v)) & ((byte) ((byte) 0x80))))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) (v)) & ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_74:
		/* SWAP */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x30) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((((byte) ((byte) ((v) >> (int) ((byte) 0x4)))) | ((byte) ((byte) ((v) << (int) ((byte) 0x4))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_75:
		/* SRL */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x38) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			var v = (byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
			var result = (byte) ((v) >> (int) ((byte) 0x1));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) (result))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) (v)) & ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x4)))))) & 0xF0);
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
			} else {
				State.Registers[(int) reg] = (byte) (result);
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_76:
		/* BIT */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x40) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			State.Flags = (byte) ((byte) ((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0x1F)))))) | ((byte) ((byte) (((byte) ((byte) ((((byte) ((byte) ((((byte) ((byte) (((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })))) >> (int) (bit)))) & ((byte) ((byte) 0x1)))))) ^ ((byte) ((byte) 0x1)))))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) ((byte) 0x5)))))) & 0xF0);
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_77:
		/* RES */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x80) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), (byte) ((((byte) ((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))) & ((byte) ((byte) (~((byte) (((byte) ((byte) 0x1)) << (int) (bit)))))))));
			} else {
				State.Registers[(int) reg] = (byte) ((byte) ((((byte) ((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))) & ((byte) ((byte) (~((byte) (((byte) ((byte) 0x1)) << (int) (bit)))))))));
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_78:
		/* SET */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0xC0) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			if((bool) (((byte) (reg)) == ((byte) 0x6))) {
				WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), (byte) ((((byte) ((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) (bit)))))));
			} else {
				State.Registers[(int) reg] = (byte) ((byte) ((((byte) ((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : (byte) ((byte) ((reg) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))) | ((byte) ((byte) (((byte) ((byte) 0x1)) << (int) (bit)))))));
			}
			AddCycles((byte) (((bool) (((byte) (reg)) == ((byte) 0x6))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x2)));
			return true;
		}
		insn_79:
		/* DAA */
		if((insnBytes[0] & 0xFF) == 0x27) {
			pc += 1;
			var n = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x6)))) & ((byte) ((byte) 0x1))));
			var h = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x5)))) & ((byte) ((byte) 0x1))));
			var c = (byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) ((byte) 0x4)))) & ((byte) ((byte) 0x1))));
			var a = (byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
			State.Registers[(int) (byte) 0x7] = (byte) ((byte) (((byte) (byte) ((byte) (a))) + ((byte) (sbyte) ((sbyte) ((sbyte) (((bool) (((byte) (n)) != ((byte) ((byte) 0x0)))) ? (byte) ((sbyte) (((sbyte) (sbyte) ((sbyte) ((sbyte) (((bool) (((byte) (c)) != ((byte) ((byte) 0x0)))) ? (byte) ((sbyte) -0x60) : (byte) ((byte) 0x0))))) + ((sbyte) (sbyte) ((sbyte) ((sbyte) (((bool) (((byte) (h)) != ((byte) ((byte) 0x0)))) ? (byte) ((sbyte) -0x6) : (byte) ((byte) 0x0))))))) : (byte) ((byte) (((byte) (byte) ((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (c)) | ((byte) ((byte) (((bool) ((bool) (((byte) (a)) > ((byte) ((byte) 0x99))))) ? 1U : 0U))))))) != ((byte) ((byte) 0x0)))) ? (byte) ((byte) (LibSharpRetro.FunctionalHelpers.Funcify(() => {
					c = (byte) 0x1;
					return (byte) ((byte) 0x60);
				})())) : (byte) ((byte) 0x0))))) + ((byte) (byte) ((byte) ((byte) (((bool) (((byte) ((byte) ((((byte) (h)) | ((byte) ((byte) (((bool) ((bool) (((byte) ((byte) ((((byte) (a)) & ((byte) ((byte) 0xF)))))) > ((byte) ((byte) 0x9))))) ? 1U : 0U))))))) != ((byte) ((byte) 0x0)))) ? (byte) ((byte) 0x6) : (byte) ((byte) 0x0)))))))))))));
			State.Flags = (byte) ((byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) ((byte) 0xF)))))) | ((byte) ((byte) (((byte) ((byte) (((bool) (((byte) ((byte) ((byte) (((byte) 0x7) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })))) == ((byte) ((byte) ((byte) 0x0))))) ? 1U : 0U))) << (int) ((byte) 0x7))))) | ((byte) ((byte) (((byte) (n)) << (int) ((byte) 0x6))))) | ((byte) ((byte) (((byte) ((byte) 0x0)) << (int) ((byte) 0x5))))) | ((byte) ((byte) (((byte) (c)) << (int) ((byte) 0x4)))))) & 0xF0);
			AddCycles((byte) 0x1);
			return true;
		}
		insn_80:
		/* HALT */
		if((insnBytes[0] & 0xFF) == 0x76) {
			pc += 1;
			Halt();
			AddCycles((byte) 0x1);
			return true;
		}
		insn_81:
		/* STOP */
		if((insnBytes[0] & 0xFF) == 0x10) {
			pc += 1;
			AddCycles((byte) 0x1);
			return true;
		}
		insn_82:

        return false;
        }
    }
}