// ReSharper disable CheckNamespace
namespace SharpStationCore;
using JitBase;
using static LibSharpRetro.CpuHelpers.Math;

public unsafe partial class Recompiler {
    public bool RecompileOne(IBuilder<uint> builder, IStructRef<CpuState> state, uint insn, uint pc) {
		/* ADD */
		if((insn & 0xFC00003F) == 0x00000020) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
			state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
			state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
			var temp_0 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
			var temp_1 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
			DoLds();
			var lhs = (temp_0).Store();
			var rhs = (temp_1).Store();
			var r = ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (lhs)) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rhs)))))).Store();
			builder.When(
				(IRuntimeValue<uint>) (((((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (~((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (lhs)) ^ ((IRuntimeValue<uint>) (rhs)))))))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (lhs)) ^ ((IRuntimeValue<uint>) (r))))))) & ((IRuntimeValue<uint>) (0x80000000U)))), 
				() => {
					throw new CpuException(ExceptionType.OV, pc, insn);
				});
				var temp_192 = rd;
				builder.When(temp_192 != builder.LiteralValue(0),
					() => state.Registers(temp_192, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (lhs)) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rhs))))));
				return true;
			}
			insn_1:
			/* ADDI */
			if((insn & 0xFC000000) == 0x20000000) {
				var rs = (insn >> 21) & 0x1FU;
				var rt = (insn >> 16) & 0x1FU;
				var imm = (insn >> 0) & 0xFFFFU;
				var eimm = (uint) (SignExt<uint>(imm, 16));
				state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
				state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
				var temp_2 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
				DoLds();
				var lhs = (temp_2).Store();
				var r = ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (lhs)) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (eimm)))))).Store();
				builder.When(
					(IRuntimeValue<uint>) (((((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (~((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (lhs)) ^ ((IRuntimeValue<uint>) (eimm)))))))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (lhs)) ^ ((IRuntimeValue<uint>) (r))))))) & ((IRuntimeValue<uint>) (0x80000000U)))), 
					() => {
						throw new CpuException(ExceptionType.OV, pc, insn);
					});
					var temp_193 = rt;
					builder.When(temp_193 != builder.LiteralValue(0),
						() => state.Registers(temp_193, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (lhs)) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (eimm))))));
					return true;
				}
				insn_2:
				/* ADDIU */
				if((insn & 0xFC000000) == 0x24000000) {
					var rs = (insn >> 21) & 0x1FU;
					var rt = (insn >> 16) & 0x1FU;
					var imm = (insn >> 0) & 0xFFFFU;
					var eimm = (uint) (SignExt<uint>(imm, 16));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
					var temp_3 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
					DoLds();
					var temp_194 = rt;
					builder.When(temp_194 != builder.LiteralValue(0),
						() => state.Registers(temp_194, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_3)) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (eimm))))));
					return true;
				}
				insn_3:
				/* ADDU */
				if((insn & 0xFC00003F) == 0x00000021) {
					var rs = (insn >> 21) & 0x1FU;
					var rt = (insn >> 16) & 0x1FU;
					var rd = (insn >> 11) & 0x1FU;
					var shamt = (insn >> 6) & 0x1FU;
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
					var temp_4 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
					var temp_5 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
					DoLds();
					var temp_195 = rd;
					builder.When(temp_195 != builder.LiteralValue(0),
						() => state.Registers(temp_195, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_4)) + ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_5))))));
					return true;
				}
				insn_4:
				/* AND */
				if((insn & 0xFC00003F) == 0x00000024) {
					var rs = (insn >> 21) & 0x1FU;
					var rt = (insn >> 16) & 0x1FU;
					var rd = (insn >> 11) & 0x1FU;
					var shamt = (insn >> 6) & 0x1FU;
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
					var temp_6 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
					var temp_7 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
					DoLds();
					var temp_196 = rd;
					builder.When(temp_196 != builder.LiteralValue(0),
						() => state.Registers(temp_196, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_6)) & ((IRuntimeValue<uint>) (temp_7)))))));
					return true;
				}
				insn_5:
				/* ANDI */
				if((insn & 0xFC000000) == 0x30000000) {
					var rs = (insn >> 21) & 0x1FU;
					var rt = (insn >> 16) & 0x1FU;
					var imm = (insn >> 0) & 0xFFFFU;
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
					var temp_8 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
					DoLds();
					var temp_197 = rt;
					builder.When(temp_197 != builder.LiteralValue(0),
						() => state.Registers(temp_197, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_8)) & ((IRuntimeValue<uint>) (imm)))))));
					return true;
				}
				insn_6:
				/* BEQ */
				if((insn & 0xFC000000) == 0x10000000) {
					var rs = (insn >> 21) & 0x1FU;
					var rt = (insn >> 16) & 0x1FU;
					var imm = (insn >> 0) & 0xFFFFU;
					var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
					var temp_9 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
					var temp_10 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
					DoLds();
					builder.If(
						(IRuntimeValue<uint>) ((temp_9) == (temp_10)), 
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
						var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
						var temp_11 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
						DoLds();
						builder.If(
							(IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_11))) >= ((byte) 0x0)), 
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
							var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F), builder.LiteralValue(0U));
							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
							var temp_12 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
							DoLds();
							state.Registers(builder.LiteralValue(31), (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8)))));
							builder.If(
								(IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_12))) >= ((byte) 0x0)), 
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
								var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
								state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
								var temp_13 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
								DoLds();
								builder.If(
									(IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_13))) > ((byte) 0x0)), 
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
									var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
									state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
									var temp_14 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
									DoLds();
									builder.If(
										(IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_14))) <= ((byte) 0x0)), 
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
										var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
										var temp_15 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
										DoLds();
										builder.If(
											(IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_15))) < ((byte) 0x0)), 
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
											var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
											state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F), builder.LiteralValue(0U));
											state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
											var temp_16 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
											DoLds();
											state.Registers(builder.LiteralValue(31), (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8)))));
											builder.If(
												(IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_16))) < ((byte) 0x0)), 
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
												var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
												state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
												state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
												var temp_17 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
												var temp_18 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
												DoLds();
												builder.If(
													(IRuntimeValue<uint>) ((temp_17) != (temp_18)), 
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
													throw new CpuException(ExceptionType.Break, pc, insn);
													return true;
												}
												insn_15:
												/* CFC */
												if((insn & 0xF3E00000) == 0x40400000) {
													var cop = (insn >> 26) & 0x3U;
													var rt = (insn >> 16) & 0x1FU;
													var rd = (insn >> 11) & 0x1FU;
													var cofun = (insn >> 0) & 0x7FFU;
													state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
													DoLds();
													var temp_198 = rt;
													builder.When(temp_198 != builder.LiteralValue(0),
														() => state.Registers(temp_198, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (Copcreg(cop, rd)))));
													return true;
												}
												insn_16:
												/* COP */
												if((insn & 0xF2000000) == 0x42000000) {
													var cop = (insn >> 26) & 0x3U;
													var command = (insn >> 0) & 0x1FFFFFFU;
													DoLds();
													Copfun(cop, command);
													return true;
												}
												insn_17:
												/* CTC */
												if((insn & 0xF3E00000) == 0x40C00000) {
													var cop = (insn >> 26) & 0x3U;
													var rt = (insn >> 16) & 0x1FU;
													var rd = (insn >> 11) & 0x1FU;
													var cofun = (insn >> 0) & 0x7FFU;
													state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
													var temp_19 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
													DoLds();
													Copcreg(cop, rd, temp_19);
													return true;
												}
												insn_18:
												/* DIV */
												if((insn & 0xFC00003F) == 0x0000001A) {
													var rs = (insn >> 21) & 0x1FU;
													var rt = (insn >> 16) & 0x1FU;
													var rd = (insn >> 11) & 0x1FU;
													var shamt = (insn >> 6) & 0x1FU;
													state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
													state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
													var temp_20 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
													var temp_21 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
													DoLds();
													var rsv = (temp_20).Store();
													var rtv = (temp_21).Store();
													builder.If(
														(IRuntimeValue<uint>) ((rtv) == ((byte) 0x0)), 
														() => {
															State->Lo = (uint) ((IRuntimeValue<byte>) (Ternary<uint, byte>((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (rsv)) & ((IRuntimeValue<uint>) (0x80000000U))))) != ((byte) 0x0))), (byte) 0x1, 0xFFFFFFFFU)));
															State->Hi = (uint) (rsv);
														}, 
														() => {
															builder.If(
																(IRuntimeValue<uint>) ((((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((rsv) == (0x80000000U)))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((rtv) == (0xFFFFFFFFU)))))), 
																() => {
																	State->Lo = (uint) (0x80000000U);
																	State->Hi = (uint) ((byte) 0x0);
																}, 
																() => {
																	State->Lo = (uint) ((IRuntimeValue<int>) (((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) ((IRuntimeValue<int>) (rsv)))) / ((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) ((IRuntimeValue<int>) (rtv))))));
																	State->Hi = (uint) ((IRuntimeValue<int>) (((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) ((IRuntimeValue<int>) (rsv)))) % ((IRuntimeValue<int>) (IRuntimeValue<int>) ((IRuntimeValue<int>) ((IRuntimeValue<int>) (rtv))))));
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
															state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
															state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
															var temp_22 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
															var temp_23 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
															DoLds();
															var rsv = (temp_22).Store();
															var rtv = (temp_23).Store();
															builder.If(
																(IRuntimeValue<uint>) ((rtv) == ((byte) 0x0)), 
																() => {
																	State->Lo = (uint) (0xFFFFFFFFU);
																	State->Hi = (uint) (rsv);
																}, 
																() => {
																	State->Lo = (uint) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rsv)) / ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rtv))));
																	State->Hi = (uint) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rsv)) % ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rtv))));
																	DivDelay();
																});
																return true;
															}
															insn_20:
															/* J */
															if((insn & 0xFC000000) == 0x08000000) {
																var imm = (insn >> 0) & 0x3FFFFFFU;
																var target = (uint) (((uint) (uint) ((uint) ((((uint) ((uint) ((pc + 4)))) & ((uint) (0xF0000000U)))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
																DoLds();
																Branch(target);
																return true;
															}
															insn_21:
															/* JAL */
															if((insn & 0xFC000000) == 0x0C000000) {
																var imm = (insn >> 0) & 0x3FFFFFFU;
																var target = (uint) (((uint) (uint) ((uint) ((((uint) ((uint) ((pc + 4)))) & ((uint) (0xF0000000U)))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
																state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F), builder.LiteralValue(0U));
																DoLds();
																state.Registers(builder.LiteralValue(31), (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8)))));
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
																state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																var temp_24 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																DoLds();
																var target = (temp_24).Store();
																var temp_199 = rd;
																builder.When(temp_199 != builder.LiteralValue(0),
																	() => state.Registers(temp_199, (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))))));
																builder.When(
																	(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (target)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																	() => {
																		throw new CpuException(ExceptionType.ADEL, pc, insn);
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
																	state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																	var temp_25 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																	DoLds();
																	var target = (temp_25).Store();
																	builder.When(
																		(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (target)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																		() => {
																			throw new CpuException(ExceptionType.ADEL, pc, insn);
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
																		var offset = (int) (SignExt<int>(imm, 16));
																		state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																		state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																		var temp_26 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																		DoLds();
																		/*UNIMPLEMENTED*/
																		return true;
																	}
																	insn_25:
																	/* LBU */
																	if((insn & 0xFC000000) == 0x90000000) {
																		var rs = (insn >> 21) & 0x1FU;
																		var rt = (insn >> 16) & 0x1FU;
																		var imm = (insn >> 0) & 0xFFFFU;
																		var offset = (int) (SignExt<int>(imm, 16));
																		state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																		state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																		var temp_27 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																		DoLds();
																		/*UNIMPLEMENTED*/
																		return true;
																	}
																	insn_26:
																	/* LH */
																	if((insn & 0xFC000000) == 0x84000000) {
																		var rs = (insn >> 21) & 0x1FU;
																		var rt = (insn >> 16) & 0x1FU;
																		var imm = (insn >> 0) & 0xFFFFU;
																		var offset = (int) (SignExt<int>(imm, 16));
																		state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																		state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																		var temp_28 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																		DoLds();
																		var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_28)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																		builder.When(
																			(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																			() => {
																				throw new CpuException(ExceptionType.ADEL, pc, insn);
																			});
																			/*UNIMPLEMENTED*/
																			return true;
																		}
																		insn_27:
																		/* LHU */
																		if((insn & 0xFC000000) == 0x94000000) {
																			var rs = (insn >> 21) & 0x1FU;
																			var rt = (insn >> 16) & 0x1FU;
																			var imm = (insn >> 0) & 0xFFFFU;
																			var offset = (int) (SignExt<int>(imm, 16));
																			state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																			state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																			var temp_29 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																			DoLds();
																			var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_29)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																			builder.When(
																				(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																				() => {
																					throw new CpuException(ExceptionType.ADEL, pc, insn);
																				});
																				/*UNIMPLEMENTED*/
																				return true;
																			}
																			insn_28:
																			/* LUI */
																			if((insn & 0xFC000000) == 0x3C000000) {
																				var rs = (insn >> 21) & 0x1FU;
																				var rt = (insn >> 16) & 0x1FU;
																				var imm = (insn >> 0) & 0xFFFFU;
																				state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																				DoLds();
																				var temp_200 = rt;
																				builder.When(temp_200 != builder.LiteralValue(0),
																					() => state.Registers(temp_200, (IRuntimeValue<uint>) builder.EnsureRuntime((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x10)))));
																				return true;
																			}
																			insn_29:
																			/* LW */
																			if((insn & 0xFC000000) == 0x8C000000) {
																				var rs = (insn >> 21) & 0x1FU;
																				var rt = (insn >> 16) & 0x1FU;
																				var imm = (insn >> 0) & 0xFFFFU;
																				var offset = (int) (SignExt<int>(imm, 16));
																				state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																				state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																				var temp_30 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																				DoLds();
																				var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_30)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																				builder.When(
																					(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																					() => {
																						throw new CpuException(ExceptionType.ADEL, pc, insn);
																					});
																					/*UNIMPLEMENTED*/
																					return true;
																				}
																				insn_30:
																				/* LWL */
																				if((insn & 0xFC000000) == 0x88000000) {
																					var rs = (insn >> 21) & 0x1FU;
																					var rt = (insn >> 16) & 0x1FU;
																					var imm = (insn >> 0) & 0xFFFFU;
																					var offset = (int) (SignExt<int>(imm, 16));
																					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																					var temp_31 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																					var temp_32 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																					DoLoad(rt, ref temp_31);
																					var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_32)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																					var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) (0xFFFFFFFCU))))).Store();
																					var ert = (temp_31).Store();
																					/*UNIMPLEMENTED*/
																					return true;
																				}
																				insn_31:
																				/* LWR */
																				if((insn & 0xFC000000) == 0x98000000) {
																					var rs = (insn >> 21) & 0x1FU;
																					var rt = (insn >> 16) & 0x1FU;
																					var imm = (insn >> 0) & 0xFFFFU;
																					var offset = (int) (SignExt<int>(imm, 16));
																					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																					var temp_33 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																					var temp_34 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																					DoLoad(rt, ref temp_33);
																					var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_34)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																					var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) (0xFFFFFFFCU))))).Store();
																					var ert = (temp_33).Store();
																					/*UNIMPLEMENTED*/
																					return true;
																				}
																				insn_32:
																				/* LWC2 */
																				if((insn & 0xFC000000) == 0xC8000000) {
																					var rs = (insn >> 21) & 0x1FU;
																					var rt = (insn >> 16) & 0x1FU;
																					var imm = (insn >> 0) & 0xFFFFU;
																					var offset = (int) (SignExt<int>(imm, 16));
																					state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																					var temp_35 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																					DoLds();
																					var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_35)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																					builder.When(
																						(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																						() => {
																							throw new CpuException(ExceptionType.ADEL, pc, insn);
																						});
																						Copreg((byte) 0x2, rt, (IRuntimeValue<uint>) (((RuntimePointer<uint>) (addr)).value()));
																						return true;
																					}
																					insn_33:
																					/* MFC */
																					if((insn & 0xF3E00000) == 0x40000000) {
																						var cop = (insn >> 26) & 0x3U;
																						var rt = (insn >> 16) & 0x1FU;
																						var rd = (insn >> 11) & 0x1FU;
																						var cofun = (insn >> 0) & 0x7FFU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						DoLds();
																						/*UNIMPLEMENTED*/
																						return true;
																					}
																					insn_34:
																					/* MFHI */
																					if((insn & 0xFC00003F) == 0x00000010) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var rd = (insn >> 11) & 0x1FU;
																						var shamt = (insn >> 6) & 0x1FU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																						DoLds();
																						var temp_201 = rd;
																						builder.When(temp_201 != builder.LiteralValue(0),
																							() => state.Registers(temp_201, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (state.Hi()))));
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
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																						DoLds();
																						var temp_202 = rd;
																						builder.When(temp_202 != builder.LiteralValue(0),
																							() => state.Registers(temp_202, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (state.Lo()))));
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
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_36 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						Copreg(cop, rd, temp_36);
																						return true;
																					}
																					insn_37:
																					/* MTHI */
																					if((insn & 0xFC00003F) == 0x00000011) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var rd = (insn >> 11) & 0x1FU;
																						var shamt = (insn >> 6) & 0x1FU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						var temp_37 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						DoLds();
																						State->Hi = (uint) (temp_37);
																						return true;
																					}
																					insn_38:
																					/* MTLO */
																					if((insn & 0xFC00003F) == 0x00000013) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var rd = (insn >> 11) & 0x1FU;
																						var shamt = (insn >> 6) & 0x1FU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						var temp_38 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						DoLds();
																						State->Lo = (uint) (temp_38);
																						return true;
																					}
																					insn_39:
																					/* MULT */
																					if((insn & 0xFC00003F) == 0x00000018) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var rd = (insn >> 11) & 0x1FU;
																						var shamt = (insn >> 6) & 0x1FU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_39 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						var temp_40 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						var lhs = (temp_39).Store();
																						var rhs = (temp_40).Store();
																						var result = ((IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) ((IRuntimeValue<long>) (((IRuntimeValue<long>) (IRuntimeValue<long>) ((IRuntimeValue<long>) ((IRuntimeValue<long>) (lhs)))) * ((IRuntimeValue<long>) (IRuntimeValue<long>) ((IRuntimeValue<long>) ((IRuntimeValue<long>) (rhs)))))))).Store();
																						State->Lo = (uint) (result);
																						State->Hi = (uint) ((IRuntimeValue<ulong>) ((result) >> ((byte) 0x20)));
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
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_41 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						var temp_42 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						var lhs = (temp_41).Store();
																						var rhs = (temp_42).Store();
																						var result = ((IRuntimeValue<ulong>) (((IRuntimeValue<ulong>) (IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) (lhs)))) * ((IRuntimeValue<ulong>) (IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) ((IRuntimeValue<ulong>) (rhs)))))).Store();
																						State->Lo = (uint) (result);
																						State->Hi = (uint) ((IRuntimeValue<ulong>) ((result) >> ((byte) 0x20)));
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
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_43 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						var temp_44 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						var temp_203 = rd;
																						builder.When(temp_203 != builder.LiteralValue(0),
																							() => state.Registers(temp_203, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (~((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_43)) | ((IRuntimeValue<uint>) (temp_44)))))))));
																						return true;
																					}
																					insn_42:
																					/* OR */
																					if((insn & 0xFC00003F) == 0x00000025) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var rd = (insn >> 11) & 0x1FU;
																						var shamt = (insn >> 6) & 0x1FU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_45 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						var temp_46 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						var temp_204 = rd;
																						builder.When(temp_204 != builder.LiteralValue(0),
																							() => state.Registers(temp_204, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_45)) | ((IRuntimeValue<uint>) (temp_46)))))));
																						return true;
																					}
																					insn_43:
																					/* ORI */
																					if((insn & 0xFC000000) == 0x34000000) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var imm = (insn >> 0) & 0xFFFFU;
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						var temp_47 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						DoLds();
																						var temp_205 = rt;
																						builder.When(temp_205 != builder.LiteralValue(0),
																							() => state.Registers(temp_205, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_47)) | ((IRuntimeValue<uint>) ((uint) ((uint) (imm)))))))));
																						return true;
																					}
																					insn_44:
																					/* SB */
																					if((insn & 0xFC000000) == 0xA0000000) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var imm = (insn >> 0) & 0xFFFFU;
																						var offset = (int) (SignExt<int>(imm, 16));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_48 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						var temp_49 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						((RuntimePointer<byte>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_48)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset))))).value((IRuntimeValue<byte>) ((IRuntimeValue<byte>) (temp_49)));
																						return true;
																					}
																					insn_45:
																					/* SH */
																					if((insn & 0xFC000000) == 0xA4000000) {
																						var rs = (insn >> 21) & 0x1FU;
																						var rt = (insn >> 16) & 0x1FU;
																						var imm = (insn >> 0) & 0xFFFFU;
																						var offset = (int) (SignExt<int>(imm, 16));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																						state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																						var temp_50 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																						var temp_51 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																						DoLds();
																						var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_50)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																						builder.When(
																							(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																							() => {
																								throw new CpuException(ExceptionType.ADES, pc, insn);
																							});
																							((RuntimePointer<ushort>) (addr)).value((IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) (temp_51)));
																							return true;
																						}
																						insn_46:
																						/* SLL */
																						if((insn & 0xFC00003F) == 0x00000000) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							var temp_52 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							DoLds();
																							var temp_206 = rd;
																							builder.When(temp_206 != builder.LiteralValue(0),
																								() => state.Registers(temp_206, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_52) << (shamt)))));
																							return true;
																						}
																						insn_47:
																						/* SLLV */
																						if((insn & 0xFC00003F) == 0x00000004) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							var temp_53 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							var temp_54 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							DoLds();
																							var temp_207 = rd;
																							builder.When(temp_207 != builder.LiteralValue(0),
																								() => state.Registers(temp_207, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_53) << (temp_54)))));
																							return true;
																						}
																						insn_48:
																						/* SLT */
																						if((insn & 0xFC00003F) == 0x0000002A) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							var temp_55 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							var temp_56 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							DoLds();
																							var temp_208 = rd;
																							builder.When(temp_208 != builder.LiteralValue(0),
																								() => state.Registers(temp_208, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_55))) < ((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_56)))))));
																							return true;
																						}
																						insn_49:
																						/* SLTI */
																						if((insn & 0xFC000000) == 0x28000000) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var imm = (insn >> 0) & 0xFFFFU;
																							var eimm = (int) (SignExt<int>(imm, 16));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							var temp_57 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							DoLds();
																							var temp_209 = rt;
																							builder.When(temp_209 != builder.LiteralValue(0),
																								() => state.Registers(temp_209, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_57))) < (eimm)))));
																							return true;
																						}
																						insn_50:
																						/* SLTIU */
																						if((insn & 0xFC000000) == 0x2C000000) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var imm = (insn >> 0) & 0xFFFFU;
																							var eimm = (uint) (SignExt<uint>(imm, 16));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							var temp_58 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							DoLds();
																							var temp_210 = rt;
																							builder.When(temp_210 != builder.LiteralValue(0),
																								() => state.Registers(temp_210, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_58) < (eimm)))));
																							return true;
																						}
																						insn_51:
																						/* SLTU */
																						if((insn & 0xFC00003F) == 0x0000002B) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							var temp_59 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							var temp_60 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							DoLds();
																							var temp_211 = rd;
																							builder.When(temp_211 != builder.LiteralValue(0),
																								() => state.Registers(temp_211, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_59) < (temp_60)))));
																							return true;
																						}
																						insn_52:
																						/* SRA */
																						if((insn & 0xFC00003F) == 0x00000003) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							var temp_61 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							DoLds();
																							var temp_212 = rd;
																							builder.When(temp_212 != builder.LiteralValue(0),
																								() => state.Registers(temp_212, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<int>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_61))) >> (shamt)))));
																							return true;
																						}
																						insn_53:
																						/* SRAV */
																						if((insn & 0xFC00003F) == 0x00000007) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							var temp_62 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							var temp_63 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							DoLds();
																							var temp_213 = rd;
																							builder.When(temp_213 != builder.LiteralValue(0),
																								() => state.Registers(temp_213, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<int>) (((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_62))) >> ((IRuntimeValue<int>) ((IRuntimeValue<int>) (temp_63)))))));
																							return true;
																						}
																						insn_54:
																						/* SRL */
																						if((insn & 0xFC00003F) == 0x00000002) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							var temp_64 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							DoLds();
																							var temp_214 = rd;
																							builder.When(temp_214 != builder.LiteralValue(0),
																								() => state.Registers(temp_214, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_64) >> (shamt)))));
																							return true;
																						}
																						insn_55:
																						/* SRLV */
																						if((insn & 0xFC00003F) == 0x00000006) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							var temp_65 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							var temp_66 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							DoLds();
																							var temp_215 = rd;
																							builder.When(temp_215 != builder.LiteralValue(0),
																								() => state.Registers(temp_215, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((temp_65) >> (temp_66)))));
																							return true;
																						}
																						insn_56:
																						/* SUB */
																						if((insn & 0xFC00003F) == 0x00000022) {
																							var rs = (insn >> 21) & 0x1FU;
																							var rt = (insn >> 16) & 0x1FU;
																							var rd = (insn >> 11) & 0x1FU;
																							var shamt = (insn >> 6) & 0x1FU;
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																							state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																							var temp_67 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																							var temp_68 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																							DoLds();
																							var lhs = (temp_67).Store();
																							var rhs = (temp_68).Store();
																							var r = ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (lhs)) - ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rhs)))))).Store();
																							builder.When(
																								(IRuntimeValue<uint>) (((((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (lhs)) ^ ((IRuntimeValue<uint>) (rhs)))))) & ((IRuntimeValue<uint>) ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (lhs)) ^ ((IRuntimeValue<uint>) (r))))))) & ((IRuntimeValue<uint>) (0x80000000U)))), 
																								() => {
																									throw new CpuException(ExceptionType.OV, pc, insn);
																								});
																								var temp_216 = rd;
																								builder.When(temp_216 != builder.LiteralValue(0),
																									() => state.Registers(temp_216, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (lhs)) - ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (rhs))))));
																								return true;
																							}
																							insn_57:
																							/* SUBU */
																							if((insn & 0xFC00003F) == 0x00000023) {
																								var rs = (insn >> 21) & 0x1FU;
																								var rt = (insn >> 16) & 0x1FU;
																								var rd = (insn >> 11) & 0x1FU;
																								var shamt = (insn >> 6) & 0x1FU;
																								state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																								state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																								state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																								var temp_69 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																								var temp_70 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																								DoLds();
																								var temp_217 = rd;
																								builder.When(temp_217 != builder.LiteralValue(0),
																									() => state.Registers(temp_217, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_69)) - ((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_70))))));
																								return true;
																							}
																							insn_58:
																							/* SW */
																							if((insn & 0xFC000000) == 0xAC000000) {
																								var rs = (insn >> 21) & 0x1FU;
																								var rt = (insn >> 16) & 0x1FU;
																								var imm = (insn >> 0) & 0xFFFFU;
																								var offset = (int) (SignExt<int>(imm, 16));
																								state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																								state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																								var temp_71 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																								var temp_72 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																								DoLds();
																								var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_71)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																								builder.When(
																									(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																									() => {
																										throw new CpuException(ExceptionType.ADES, pc, insn);
																									});
																									((RuntimePointer<uint>) (addr)).value(temp_72);
																									return true;
																								}
																								insn_59:
																								/* SWC2 */
																								if((insn & 0xFC000000) == 0xE8000000) {
																									var rs = (insn >> 21) & 0x1FU;
																									var rt = (insn >> 16) & 0x1FU;
																									var imm = (insn >> 0) & 0xFFFFU;
																									var offset = (int) (SignExt<int>(imm, 16));
																									state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																									var temp_73 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																									DoLds();
																									var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_73)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																									builder.When(
																										(IRuntimeValue<uint>) (((byte) 0x0) != ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))), 
																										() => {
																											throw new CpuException(ExceptionType.ADES, pc, insn);
																										});
																										((RuntimePointer<uint>) (addr)).value((IRuntimeValue<uint>) (Copreg((byte) 0x2, rt)));
																										return true;
																									}
																									insn_60:
																									/* SWL */
																									if((insn & 0xFC000000) == 0xA8000000) {
																										var rs = (insn >> 21) & 0x1FU;
																										var rt = (insn >> 16) & 0x1FU;
																										var imm = (insn >> 0) & 0xFFFFU;
																										var offset = (int) (SignExt<int>(imm, 16));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																										var temp_74 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																										var temp_75 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																										DoLds();
																										var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_74)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																										var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) (0xFFFFFFFCU))))).Store();
																										var rtv = (temp_75).Store();
																										switch((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((byte) 0x3))))) {
																											case (IRuntimeValue<uint>) ((byte) 0x0): {
																												((RuntimePointer<byte>) (raddr)).value((IRuntimeValue<byte>) ((IRuntimeValue<byte>) ((IRuntimeValue<uint>) ((rtv) >> ((byte) 0x18)))));
																												break;
																											}
																											case (IRuntimeValue<uint>) ((byte) 0x1): {
																												((RuntimePointer<ushort>) (raddr)).value((IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) ((IRuntimeValue<uint>) ((rtv) >> ((byte) 0x10)))));
																												break;
																											}
																											case (IRuntimeValue<uint>) ((byte) 0x3): {
																												((RuntimePointer<uint>) (raddr)).value(rtv);
																												break;
																											}
																											default: {
																												((RuntimePointer<ushort>) (raddr)).value((IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) ((IRuntimeValue<uint>) ((rtv) >> ((byte) 0x8)))));
																												((RuntimePointer<byte>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (raddr)) + ((IRuntimeValue<uint>) (IRuntimeValue<byte>) ((byte) 0x2))))).value((IRuntimeValue<byte>) ((IRuntimeValue<byte>) ((IRuntimeValue<uint>) ((rtv) >> ((byte) 0x18)))));
																												break;
																											}
																										}
																										return true;
																									}
																									insn_61:
																									/* SWR */
																									if((insn & 0xFC000000) == 0xB8000000) {
																										var rs = (insn >> 21) & 0x1FU;
																										var rt = (insn >> 16) & 0x1FU;
																										var imm = (insn >> 0) & 0xFFFFU;
																										var offset = (int) (SignExt<int>(imm, 16));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																										var temp_76 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																										var temp_77 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																										DoLds();
																										var addr = ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (temp_76)) + ((IRuntimeValue<uint>) (IRuntimeValue<int>) (offset)))).Store();
																										var raddr = ((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) (0xFFFFFFFCU))))).Store();
																										var rtv = (temp_77).Store();
																										switch((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (addr)) & ((IRuntimeValue<uint>) ((byte) 0x3))))) {
																											case (IRuntimeValue<uint>) ((byte) 0x0): {
																												((RuntimePointer<uint>) (raddr)).value(rtv);
																												break;
																											}
																											case (IRuntimeValue<uint>) ((byte) 0x2): {
																												((RuntimePointer<ushort>) (raddr)).value((IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) (rtv)));
																												break;
																											}
																											case (IRuntimeValue<uint>) ((byte) 0x3): {
																												((RuntimePointer<byte>) (raddr)).value((IRuntimeValue<byte>) ((IRuntimeValue<byte>) (rtv)));
																												break;
																											}
																											default: {
																												((RuntimePointer<ushort>) (raddr)).value((IRuntimeValue<ushort>) ((IRuntimeValue<ushort>) (rtv)));
																												((RuntimePointer<byte>) ((IRuntimeValue<uint>) (((IRuntimeValue<uint>) (IRuntimeValue<uint>) (raddr)) + ((IRuntimeValue<uint>) (IRuntimeValue<byte>) ((byte) 0x2))))).value((IRuntimeValue<byte>) ((IRuntimeValue<byte>) ((IRuntimeValue<uint>) ((rtv) >> ((byte) 0x10)))));
																												break;
																											}
																										}
																										return true;
																									}
																									insn_62:
																									/* SYSCALL */
																									if((insn & 0xFC00003F) == 0x0000000C) {
																										var code = (insn >> 6) & 0xFFFFFU;
																										DoLds();
																										throw new CpuException(ExceptionType.Syscall, pc, insn);
																										return true;
																									}
																									insn_63:
																									/* XOR */
																									if((insn & 0xFC00003F) == 0x00000026) {
																										var rs = (insn >> 21) & 0x1FU;
																										var rt = (insn >> 16) & 0x1FU;
																										var rd = (insn >> 11) & 0x1FU;
																										var shamt = (insn >> 6) & 0x1FU;
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																										var temp_78 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																										var temp_79 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))).Store();
																										DoLds();
																										var temp_218 = rd;
																										builder.When(temp_218 != builder.LiteralValue(0),
																											() => state.Registers(temp_218, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_78)) ^ ((IRuntimeValue<uint>) (temp_79)))))));
																										return true;
																									}
																									insn_64:
																									/* XORI */
																									if((insn & 0xFC000000) == 0x38000000) {
																										var rs = (insn >> 21) & 0x1FU;
																										var rt = (insn >> 16) & 0x1FU;
																										var imm = (insn >> 0) & 0xFFFFU;
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
																										state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
																										var temp_80 = ((IRuntimeValue<uint>) (state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))).Store();
																										DoLds();
																										var temp_219 = rt;
																										builder.When(temp_219 != builder.LiteralValue(0),
																											() => state.Registers(temp_219, (IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<uint>) ((((IRuntimeValue<uint>) (temp_80)) ^ ((IRuntimeValue<uint>) ((uint) ((uint) (imm)))))))));
																										return true;
																									}
																									insn_65:

        return false;
    }
}
