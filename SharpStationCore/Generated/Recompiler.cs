// ReSharper disable CheckNamespace
namespace SharpStationCore;
using JitBase;
using Math = LibSharpRetro.CpuHelpers.Math;

public unsafe partial class Recompiler {
    public bool RecompileOne(IBuilder<uint> builder, IStructRef<CpuState> state, uint insn, uint pc) {
		/* ADD */
		if((insn & 0xFC00003F) == 0x00000020) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			var temp_0 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_1 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var lhs = (temp_0).Store();
			var rhs = (temp_1).Store();
			var r = ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs)))) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rhs))))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (~((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(rhs)))))))))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(r))))))))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x80000000U)))))))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x0U)))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.OV), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			var temp_192 = rd;
			if(temp_192 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_192)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs)))) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rhs))))));
			return true;
		}
		insn_1:
		/* ADDI */
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (Math.SignExt<uint>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_2 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var lhs = (temp_2).Store();
			var r = ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs)))) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(eimm))))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (~((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(eimm)))))))))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(r))))))))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x80000000U)))))))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x0U)))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.OV), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			var temp_193 = rt;
			if(temp_193 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_193)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs)))) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(eimm))))));
			return true;
		}
		insn_2:
		/* ADDIU */
		if((insn & 0xFC000000) == 0x24000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (Math.SignExt<uint>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_3 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_194 = rt;
			if(temp_194 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_194)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_3)))) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(eimm))))));
			return true;
		}
		insn_3:
		/* ADDU */
		if((insn & 0xFC00003F) == 0x00000021) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_4 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_5 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_195 = rd;
			if(temp_195 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_195)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_4)))) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_5))))));
			return true;
		}
		insn_4:
		/* AND */
		if((insn & 0xFC00003F) == 0x00000024) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_6 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_7 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_196 = rd;
			if(temp_196 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_196)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_6))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_7))))));
			return true;
		}
		insn_5:
		/* ANDI */
		if((insn & 0xFC000000) == 0x30000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_8 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_197 = rt;
			if(temp_197 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_197)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_8))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<ushort>) (builder.EnsureRuntime(imm))))))));
			return true;
		}
		insn_6:
		/* BEQ */
		if((insn & 0xFC000000) == 0x10000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_9 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_10 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_9))) == ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_10)))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_7:
		/* BGEZ */
		if((insn & 0xFC110000) == 0x04010000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_11 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_11))))) >= ((IRuntimeValue<int>) ((IRuntimeValue<sbyte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_8:
		/* BGEZAL */
		if((insn & 0xFC110000) == 0x04110000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_12 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			state.Registers[builder.LiteralValue(31)] = (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_12))))) >= ((IRuntimeValue<int>) ((IRuntimeValue<sbyte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_9:
		/* BGTZ */
		if((insn & 0xFC1F0000) == 0x1C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_13 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_13))))) > ((IRuntimeValue<int>) ((IRuntimeValue<sbyte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_10:
		/* BLEZ */
		if((insn & 0xFC1F0000) == 0x18000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_14 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_14))))) <= ((IRuntimeValue<int>) ((IRuntimeValue<sbyte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_11:
		/* BLTZ */
		if((insn & 0xFC110000) == 0x04000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_15 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_15))))) < ((IRuntimeValue<int>) ((IRuntimeValue<sbyte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_12:
		/* BLTZAL */
		if((insn & 0xFC110000) == 0x04100000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_16 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			state.Registers[builder.LiteralValue(31)] = (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_16))))) < ((IRuntimeValue<int>) ((IRuntimeValue<sbyte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_13:
		/* BNE */
		if((insn & 0xFC000000) == 0x14000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (Math.SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_17 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_18 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_17))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_18)))), 
				() => {
					Branch(target);
				}, 
				() => {
					Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
				});
			return true;
		}
		insn_14:
		/* BREAK */
		if((insn & 0xFC00003F) == 0x0000000D) {
			var code = (insn >> 6) & 0xFFFFFU;
			DoLds();
			builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.Break), builder.LiteralValue(pc), builder.LiteralValue(insn));
			return true;
		}
		insn_15:
		/* CFC */
		if((insn & 0xF3E00000) == 0x40400000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			DoLds();
			var temp_198 = rt;
			if(temp_198 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_198)] = (IRuntimeValue<uint>) builder.EnsureRuntime(builder.Call<uint, uint, uint>(Copcreg, (IRuntimeValue<uint>) builder.EnsureRuntime(cop), (IRuntimeValue<uint>) builder.EnsureRuntime(rd)));
			return true;
		}
		insn_16:
		/* COP */
		if((insn & 0xF2000000) == 0x42000000) {
			var cop = (insn >> 26) & 0x3U;
			var command = (insn >> 0) & 0x1FFFFFFU;
			DoLds();
			builder.CallVoid(Copfun, (IRuntimeValue<uint>) builder.EnsureRuntime(cop), (IRuntimeValue<uint>) builder.EnsureRuntime(command));
			return true;
		}
		insn_17:
		/* CTC */
		if((insn & 0xF3E00000) == 0x40C00000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_19 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			builder.CallVoid<uint, uint, uint>(Copcreg, (IRuntimeValue<uint>) builder.EnsureRuntime(cop), (IRuntimeValue<uint>) builder.EnsureRuntime(rd), (IRuntimeValue<uint>) builder.EnsureRuntime(temp_19));
			return true;
		}
		insn_18:
		/* DIV */
		if((insn & 0xFC00003F) == 0x0000001A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_20 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_21 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var rsv = (temp_20).Store();
			var rtv = (temp_21).Store();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(rtv))) == ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0))))), 
				() => {
					state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<byte>) (builder.Ternary((IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(rsv))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x80000000U)))))))) != ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0))))), (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x1), (IRuntimeValue<uint>) builder.EnsureRuntime(0xFFFFFFFFU))));
					state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime(rsv);
				}, 
				() => {
					builder.If(
						(IRuntimeValue<bool>) (((IRuntimeValue<byte>) (builder.EnsureRuntime((IRuntimeValue<byte>) ((((IRuntimeValue<sbyte>) ((IRuntimeValue<sbyte>) builder.Ternary((IRuntimeValue<bool>) (builder.EnsureRuntime((IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(rsv))) == ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x80000000U)))))), builder.LiteralValue(1U), builder.Zero<uint>()))) & ((IRuntimeValue<sbyte>) ((IRuntimeValue<sbyte>) builder.Ternary((IRuntimeValue<bool>) (builder.EnsureRuntime((IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(rtv))) == ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFFFU)))))), builder.LiteralValue(1U), builder.Zero<uint>())))))))) != ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))), 
						() => {
							state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime(0x80000000U);
							state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0);
						}, 
						() => {
							state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<int>) (((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (rsv)))))) / ((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (rtv))))))));
							state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<int>) (((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (rsv)))))) % ((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (rtv))))))));
							DivDelay();
						});
				});
			return true;
		}
		insn_19:
		/* DIVU */
		if((insn & 0xFC00003F) == 0x0000001B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_22 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_23 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var rsv = (temp_22).Store();
			var rtv = (temp_23).Store();
			builder.If(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(rtv))) == ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0))))), 
				() => {
					state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime(0xFFFFFFFFU);
					state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime(rsv);
				}, 
				() => {
					state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rsv)))) / ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rtv))))));
					state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rsv)))) % ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rtv))))));
					DivDelay();
				});
			return true;
		}
		insn_20:
		/* J */
		if((insn & 0xFC000000) == 0x08000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((pc + 4))) & (0xF0000000U))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			DoLds();
			Branch(target);
			return true;
		}
		insn_21:
		/* JAL */
		if((insn & 0xFC000000) == 0x0C000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((pc + 4))) & (0xF0000000U))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F)] = builder.LiteralValue(0U);
			DoLds();
			state.Registers[builder.LiteralValue(31)] = (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			Branch(target);
			return true;
		}
		insn_22:
		/* JALR */
		if((insn & 0xFC00003F) == 0x00000009) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			var temp_24 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var target = (temp_24).Store();
			var temp_199 = rd;
			if(temp_199 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_199)] = (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(target))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			Branch(target);
			return true;
		}
		insn_23:
		/* JR */
		if((insn & 0xFC00003F) == 0x00000008) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_25 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var target = (temp_25).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(target))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			Branch(target);
			return true;
		}
		insn_24:
		/* LB */
		if((insn & 0xFC000000) == 0x80000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_26 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<sbyte>) (builder.Pointer<sbyte>((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_26)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Value));
			return true;
		}
		insn_25:
		/* LBU */
		if((insn & 0xFC000000) == 0x90000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_27 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<byte>) (builder.Pointer<byte>((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_27)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Value));
			return true;
		}
		insn_26:
		/* LH */
		if((insn & 0xFC000000) == 0x84000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_28 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_28)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<short>) (builder.Pointer<short>(addr).Value));
			return true;
		}
		insn_27:
		/* LHU */
		if((insn & 0xFC000000) == 0x94000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_29 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_29)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<ushort>) (builder.Pointer<ushort>(addr).Value));
			return true;
		}
		insn_28:
		/* LUI */
		if((insn & 0xFC000000) == 0x3C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			DoLds();
			var temp_200 = rt;
			if(temp_200 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_200)] = (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x10)));
			return true;
		}
		insn_29:
		/* LW */
		if((insn & 0xFC000000) == 0x8C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_30 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_30)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (builder.Pointer<uint>(addr).Value));
			return true;
		}
		insn_30:
		/* LWL */
		if((insn & 0xFC000000) == 0x88000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_31 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			var temp_32 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLoad(rt, ref temp_31);
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_32)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFFCU)))))).Store();
			var ert = (temp_31).Store();
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (builder.Switch(builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x3)))))))), ((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0), () => (IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(ert))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFU)))))))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.Pointer<byte>(raddr).Value)))).LeftShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x18))))))))), ((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x1), () => (IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(ert))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<ushort>) (builder.EnsureRuntime((ushort) 0xFFFF)))))))))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<ushort>) (builder.Pointer<ushort>(raddr).Value)))).LeftShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x10))))))))), ((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), () => (IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(ert))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0xFF)))))))))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (builder.Pointer<uint>(raddr).Value)).LeftShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x8))))))))), (null, () => (IRuntimeValue<uint>) (builder.Pointer<uint>(raddr).Value)))));
			return true;
		}
		insn_31:
		/* LWR */
		if((insn & 0xFC000000) == 0x98000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_33 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			var temp_34 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLoad(rt, ref temp_33);
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_34)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFFCU)))))).Store();
			var ert = (temp_33).Store();
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (builder.Switch(builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x3)))))))), ((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0), () => (IRuntimeValue<uint>) (builder.Pointer<uint>(raddr).Value)), ((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x1), () => (IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(ert))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFF000000U)))))))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (builder.Pointer<uint>(addr).Value)))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFU))))))))))), ((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), () => (IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(ert))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFF0000U)))))))) | ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<ushort>) (builder.EnsureRuntime((IRuntimeValue<ushort>) (builder.Pointer<ushort>(addr).Value))))))))), (null, () => (IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(ert))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFF00U)))))))) | ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((IRuntimeValue<byte>) (builder.Pointer<byte>(addr).Value))))))))))));
			return true;
		}
		insn_32:
		/* LWC2 */
		if((insn & 0xFC000000) == 0xC8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_35 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_35)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			builder.CallVoid<uint, uint, uint>(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), (IRuntimeValue<uint>) builder.EnsureRuntime(rt), (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (builder.Pointer<uint>(addr).Value)));
			return true;
		}
		insn_33:
		/* MFC */
		if((insn & 0xF3E00000) == 0x40000000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			DoLds();
			state.LdWhich = (IRuntimeValue<uint>) builder.EnsureRuntime(rt);
			state.LdValue = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (builder.Call<uint, uint, uint>(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime(cop), (IRuntimeValue<uint>) builder.EnsureRuntime(rd))));
			return true;
		}
		insn_34:
		/* MFHI */
		if((insn & 0xFC00003F) == 0x00000010) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			DoLds();
			var temp_201 = rd;
			if(temp_201 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_201)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (state.Hi));
			AbsorbMuldivDelay();
			return true;
		}
		insn_35:
		/* MFLO */
		if((insn & 0xFC00003F) == 0x00000012) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			DoLds();
			var temp_202 = rd;
			if(temp_202 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_202)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (state.Lo));
			AbsorbMuldivDelay();
			return true;
		}
		insn_36:
		/* MTC */
		if((insn & 0xF3E00000) == 0x40800000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_36 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			builder.CallVoid<uint, uint, uint>(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime(cop), (IRuntimeValue<uint>) builder.EnsureRuntime(rd), (IRuntimeValue<uint>) builder.EnsureRuntime(temp_36));
			return true;
		}
		insn_37:
		/* MTHI */
		if((insn & 0xFC00003F) == 0x00000011) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_37 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime(temp_37);
			return true;
		}
		insn_38:
		/* MTLO */
		if((insn & 0xFC00003F) == 0x00000013) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_38 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime(temp_38);
			return true;
		}
		insn_39:
		/* MULT */
		if((insn & 0xFC00003F) == 0x00000018) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_39 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_40 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var lhs = (temp_39).Store();
			var rhs = (temp_40).Store();
			var result = ((IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) ((IRuntimeValue<long>) (((IRuntimeValue<long>) (IRuntimeValue<long>) ((IRuntimeValue<long>) (builder.EnsureRuntime((IRuntimeValue<long>) ((IRuntimeValue<long>) (lhs)))))) * ((IRuntimeValue<long>) (IRuntimeValue<long>) ((IRuntimeValue<long>) (builder.EnsureRuntime((IRuntimeValue<long>) ((IRuntimeValue<long>) (rhs)))))))))).Store();
			state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime(result);
			state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<ulong>) ((result).RightShift((IRuntimeValue<ulong>) builder.EnsureRuntime((byte) 0x20))));
			MulDelay(lhs, rhs, (byte) 0x1 != 0);
			return true;
		}
		insn_40:
		/* MULTU */
		if((insn & 0xFC00003F) == 0x00000019) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_41 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_42 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var lhs = (temp_41).Store();
			var rhs = (temp_42).Store();
			var result = ((IRuntimeValue<ulong>) (((IRuntimeValue<ulong>) (IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) (builder.EnsureRuntime((IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) (lhs)))))) * ((IRuntimeValue<ulong>) (IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) (builder.EnsureRuntime((IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) (rhs)))))))).Store();
			state.Lo = (IRuntimeValue<uint>) builder.EnsureRuntime(result);
			state.Hi = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<ulong>) ((result).RightShift((IRuntimeValue<ulong>) builder.EnsureRuntime((byte) 0x20))));
			MulDelay(lhs, rhs, (byte) 0x0 != 0);
			return true;
		}
		insn_41:
		/* NOR */
		if((insn & 0xFC00003F) == 0x00000027) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_43 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_44 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_203 = rd;
			if(temp_203 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_203)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (~((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_43))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_44))))))));
			return true;
		}
		insn_42:
		/* OR */
		if((insn & 0xFC00003F) == 0x00000025) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_45 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_46 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_204 = rd;
			if(temp_204 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_204)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_45))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_46))))));
			return true;
		}
		insn_43:
		/* ORI */
		if((insn & 0xFC000000) == 0x34000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_47 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_205 = rt;
			if(temp_205 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_205)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_47))) | ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) ((uint) (imm))))))));
			return true;
		}
		insn_44:
		/* SB */
		if((insn & 0xFC000000) == 0xA0000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_48 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_49 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			builder.Pointer<byte>((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_48)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Value = (IRuntimeValue<byte>) ((IRuntimeValue<byte>) (temp_49));
			return true;
		}
		insn_45:
		/* SH */
		if((insn & 0xFC000000) == 0xA4000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_50 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_51 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_50)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADES), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			builder.Pointer<ushort>(addr).Value = (IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) (temp_51));
			return true;
		}
		insn_46:
		/* SLL */
		if((insn & 0xFC00003F) == 0x00000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_52 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_206 = rd;
			if(temp_206 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_206)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_52).LeftShift((IRuntimeValue<uint>) builder.EnsureRuntime(shamt))));
			return true;
		}
		insn_47:
		/* SLLV */
		if((insn & 0xFC00003F) == 0x00000004) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_53 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			var temp_54 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_207 = rd;
			if(temp_207 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_207)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_53).LeftShift((IRuntimeValue<uint>) builder.EnsureRuntime(temp_54))));
			return true;
		}
		insn_48:
		/* SLT */
		if((insn & 0xFC00003F) == 0x0000002A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_55 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_56 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_208 = rd;
			if(temp_208 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_208)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_55))))) < ((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_56)))))));
			return true;
		}
		insn_49:
		/* SLTI */
		if((insn & 0xFC000000) == 0x28000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_57 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_209 = rt;
			if(temp_209 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_209)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<bool>) (((IRuntimeValue<int>) (builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_57))))) < ((IRuntimeValue<int>) (builder.EnsureRuntime(eimm)))));
			return true;
		}
		insn_50:
		/* SLTIU */
		if((insn & 0xFC000000) == 0x2C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (Math.SignExt<uint>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_58 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_210 = rt;
			if(temp_210 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_210)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_58))) < ((IRuntimeValue<uint>) (builder.EnsureRuntime(eimm)))));
			return true;
		}
		insn_51:
		/* SLTU */
		if((insn & 0xFC00003F) == 0x0000002B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_59 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_60 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_211 = rd;
			if(temp_211 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_211)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_59))) < ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_60)))));
			return true;
		}
		insn_52:
		/* SRA */
		if((insn & 0xFC00003F) == 0x00000003) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_61 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_212 = rd;
			if(temp_212 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_212)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<int>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_61))).RightShift((IRuntimeValue<int>) builder.EnsureRuntime(shamt))));
			return true;
		}
		insn_53:
		/* SRAV */
		if((insn & 0xFC00003F) == 0x00000007) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_62 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			var temp_63 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_213 = rd;
			if(temp_213 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_213)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<int>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_62))).RightShift((IRuntimeValue<int>) builder.EnsureRuntime((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_63))))));
			return true;
		}
		insn_54:
		/* SRL */
		if((insn & 0xFC00003F) == 0x00000002) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_64 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_214 = rd;
			if(temp_214 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_214)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_64).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime(shamt))));
			return true;
		}
		insn_55:
		/* SRLV */
		if((insn & 0xFC00003F) == 0x00000006) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_65 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			var temp_66 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_215 = rd;
			if(temp_215 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_215)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_65).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime(temp_66))));
			return true;
		}
		insn_56:
		/* SUB */
		if((insn & 0xFC00003F) == 0x00000022) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			var temp_67 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_68 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var lhs = (temp_67).Store();
			var rhs = (temp_68).Store();
			var r = ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs)))) - ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rhs))))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) (((((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(rhs)))))))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(r))))))))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x80000000U)))))))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime(0x0U)))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.OV), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			var temp_216 = rd;
			if(temp_216 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_216)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(lhs)))) - ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(rhs))))));
			return true;
		}
		insn_57:
		/* SUBU */
		if((insn & 0xFC00003F) == 0x00000023) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_69 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_70 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_217 = rd;
			if(temp_217 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_217)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_69)))) - ((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_70))))));
			return true;
		}
		insn_58:
		/* SW */
		if((insn & 0xFC000000) == 0xAC000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_71 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_72 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_71)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADES), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			builder.Pointer<uint>(addr).Value = temp_72;
			return true;
		}
		insn_59:
		/* SWC2 */
		if((insn & 0xFC000000) == 0xE8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_73 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_73)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			builder.When(
				(IRuntimeValue<bool>) (((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x0)))) != ((IRuntimeValue<uint>) (builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))))))), 
				() => {
					builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.ADES), builder.LiteralValue(pc), builder.LiteralValue(insn));
				});
			builder.Pointer<uint>(addr).Value = (IRuntimeValue<uint>) (builder.Call<uint, uint, uint>(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), (IRuntimeValue<uint>) builder.EnsureRuntime(rt)));
			return true;
		}
		insn_60:
		/* SWL */
		if((insn & 0xFC000000) == 0xA8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_74 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_75 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_74)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFFCU)))))).Store();
			var rtv = (temp_75).Store();
			builder.Switch(builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x3)))))))), 
					((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0), () => {
						builder.Pointer<byte>(raddr).Value = (IRuntimeValue<byte>) ((IRuntimeValue<byte>) ((IRuntimeValue<uint>) ((rtv).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x18)))));
					}),
					((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x1), () => {
						builder.Pointer<ushort>(raddr).Value = (IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) ((IRuntimeValue<uint>) ((rtv).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x10)))));
					}),
					((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x3), () => {
						builder.Pointer<uint>(raddr).Value = rtv;
					}),
					(null, () => {
						builder.Pointer<ushort>(raddr).Value = (IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) ((IRuntimeValue<uint>) ((rtv).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x8)))));
						builder.Pointer<byte>((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(raddr)))) + ((IRuntimeValue<uint>) (IRuntimeValue<byte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x2)))))).Value = (IRuntimeValue<byte>) ((IRuntimeValue<byte>) ((IRuntimeValue<uint>) ((rtv).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x18)))));
					})
				);
			return true;
		}
		insn_61:
		/* SWR */
		if((insn & 0xFC000000) == 0xB8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (Math.SignExt<int>(imm, 16));
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_76 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_77 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_76)))) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) ((IRuntimeValue<int>) (builder.EnsureRuntime(offset)))))).Store();
			var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) (builder.EnsureRuntime(0xFFFFFFFCU)))))).Store();
			var rtv = (temp_77).Store();
			builder.Switch(builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(addr))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x3)))))))), 
					((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0), () => {
						builder.Pointer<uint>(raddr).Value = rtv;
					}),
					((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), () => {
						builder.Pointer<ushort>(raddr).Value = (IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) (rtv));
					}),
					((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x3), () => {
						builder.Pointer<byte>(raddr).Value = (IRuntimeValue<byte>) ((IRuntimeValue<byte>) (rtv));
					}),
					(null, () => {
						builder.Pointer<ushort>(raddr).Value = (IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) (rtv));
						builder.Pointer<byte>((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) ((IRuntimeValue<uint>) (builder.EnsureRuntime(raddr)))) + ((IRuntimeValue<uint>) (IRuntimeValue<byte>) ((IRuntimeValue<byte>) (builder.EnsureRuntime((byte) 0x2)))))).Value = (IRuntimeValue<byte>) ((IRuntimeValue<byte>) ((IRuntimeValue<uint>) ((rtv).RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x10)))));
					})
				);
			return true;
		}
		insn_62:
		/* SYSCALL */
		if((insn & 0xFC00003F) == 0x0000000C) {
			var code = (insn >> 6) & 0xFFFFFU;
			DoLds();
			builder.CallVoid(ThrowCpuException, builder.LiteralValue(ExceptionType.Syscall), builder.LiteralValue(pc), builder.LiteralValue(insn));
			return true;
		}
		insn_63:
		/* XOR */
		if((insn & 0xFC00003F) == 0x00000026) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rd)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			var temp_78 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			var temp_79 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rt)])).Store();
			DoLds();
			var temp_218 = rd;
			if(temp_218 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_218)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_78))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_79))))));
			return true;
		}
		insn_64:
		/* XORI */
		if((insn & 0xFC000000) == 0x38000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rt)] = builder.LiteralValue(0U);
			state.ReadAbsorb[(IRuntimeValue<int>) builder.EnsureRuntime(rs)] = builder.LiteralValue(0U);
			var temp_80 = ((IRuntimeValue<uint>) (state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(rs)])).Store();
			DoLds();
			var temp_219 = rt;
			if(temp_219 != 0)
				state.Registers[(IRuntimeValue<int>) builder.EnsureRuntime(temp_219)] = (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (builder.EnsureRuntime(temp_80))) ^ ((IRuntimeValue<uint>) (builder.EnsureRuntime((uint) ((uint) (imm))))))));
			return true;
		}
		insn_65:

        return false;
    }
}
