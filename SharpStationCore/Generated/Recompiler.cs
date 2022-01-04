// ReSharper disable CheckNamespace

#region

using JitBase;
using Math = LibSharpRetro.CpuHelpers.Math;

#endregion

namespace SharpStationCore;

    #region

using static Math;

#endregion

public partial class Recompiler {
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
            var temp_0 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            var temp_1 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))
                .Store();
            DoLds();
            var lhs = temp_0.Store();
            var rhs = temp_1.Store();
            var r = (builder.EnsureRuntime(lhs) + builder.EnsureRuntime(rhs)).Store();
            builder.When(
                (builder.EnsureRuntime(~(builder.EnsureRuntime(lhs) ^ builder.EnsureRuntime(rhs))) &
                 builder.EnsureRuntime(
                     builder.EnsureRuntime(lhs) ^ builder.EnsureRuntime(r)) &
                 builder.EnsureRuntime(0x80000000U)) != builder.Zero<uint>(),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.OV), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            var temp_192 = rd;
            if(temp_192 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_192),
                    builder.EnsureRuntime(builder.EnsureRuntime(lhs) +
                                          builder.EnsureRuntime(rhs)));
            return true;
        }

        /* ADDI */
        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_2 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            DoLds();
            var lhs = temp_2.Store();
            var r = (builder.EnsureRuntime(lhs) + builder.EnsureRuntime(eimm)).Store();
            builder.When(
                (builder.EnsureRuntime(~(builder.EnsureRuntime(lhs) ^ builder.EnsureRuntime(eimm))) &
                 builder.EnsureRuntime(
                     builder.EnsureRuntime(lhs) ^ builder.EnsureRuntime(r)) &
                 builder.EnsureRuntime(0x80000000U)) != builder.Zero<uint>(),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.OV), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            var temp_193 = rt;
            if(temp_193 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_193),
                    builder.EnsureRuntime(
                        builder.EnsureRuntime(lhs) + builder.EnsureRuntime(eimm)));
            return true;
        }

        /* ADDIU */
        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_3 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            DoLds();
            var temp_194 = rt;
            if(temp_194 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_194),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_3) +
                                          builder.EnsureRuntime(eimm)));
            return true;
        }

        /* ADDU */
        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_4 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            var temp_5 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))
                .Store();
            DoLds();
            var temp_195 = rd;
            if(temp_195 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_195),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_4) +
                                          builder.EnsureRuntime(temp_5)));
            return true;
        }

        /* AND */
        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_6 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            var temp_7 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt)))
                .Store();
            DoLds();
            var temp_196 = rd;
            if(temp_196 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_196),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_6) & builder.EnsureRuntime(temp_7)));
            return true;
        }

        /* ANDI */
        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_8 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            DoLds();
            var temp_197 = rt;
            if(temp_197 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_197),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_8) &
                                          (IRuntimeValue<uint>) (IRuntimeValue<ushort>) builder.EnsureRuntime(imm)));
            return true;
        }

        /* BEQ */
        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_9 = ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs)))
                .Store();
            var temp_10 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            builder.If(
                builder.EnsureRuntime(temp_9) == builder.EnsureRuntime(temp_10),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BGEZ */
        if((insn & 0xFC110000) == 0x04010000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_11 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            builder.If(
                builder.EnsureRuntime((IRuntimeValue<int>) temp_11) >=
                (IRuntimeValue<int>) (IRuntimeValue<sbyte>) builder.EnsureRuntime((byte) 0x0),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BGEZAL */
        if((insn & 0xFC110000) == 0x04110000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_12 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            state.Registers(builder.LiteralValue(31), builder.EnsureRuntime(pc + 0x8));
            builder.If(
                builder.EnsureRuntime((IRuntimeValue<int>) temp_12) >=
                (IRuntimeValue<int>) (IRuntimeValue<sbyte>) builder.EnsureRuntime((byte) 0x0),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BGTZ */
        if((insn & 0xFC1F0000) == 0x1C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_13 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            builder.If(
                builder.EnsureRuntime((IRuntimeValue<int>) temp_13) >
                (IRuntimeValue<int>) (IRuntimeValue<sbyte>) builder.EnsureRuntime((byte) 0x0),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BLEZ */
        if((insn & 0xFC1F0000) == 0x18000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_14 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            builder.If(
                builder.EnsureRuntime((IRuntimeValue<int>) temp_14) <=
                (IRuntimeValue<int>) (IRuntimeValue<sbyte>) builder.EnsureRuntime((byte) 0x0),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BLTZ */
        if((insn & 0xFC110000) == 0x04000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_15 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            builder.If(
                builder.EnsureRuntime((IRuntimeValue<int>) temp_15) <
                (IRuntimeValue<int>) (IRuntimeValue<sbyte>) builder.EnsureRuntime((byte) 0x0),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BLTZAL */
        if((insn & 0xFC110000) == 0x04100000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_16 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            state.Registers(builder.LiteralValue(31), builder.EnsureRuntime(pc + 0x8));
            builder.If(
                builder.EnsureRuntime((IRuntimeValue<int>) temp_16) <
                (IRuntimeValue<int>) (IRuntimeValue<sbyte>) builder.EnsureRuntime((byte) 0x0),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BNE */
        if((insn & 0xFC000000) == 0x14000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_17 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_18 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            builder.If(
                builder.EnsureRuntime(temp_17) != builder.EnsureRuntime(temp_18),
                () => { Branch(target); },
                () => { Branch(pc + 0x8); });
            return true;
        }

        /* BREAK */
        if((insn & 0xFC00003F) == 0x0000000D) {
            var code = (insn >> 6) & 0xFFFFFU;
            DoLds();
            builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.Break), builder.LiteralValue(pc),
                builder.LiteralValue(insn));
            return true;
        }

        /* CFC */
        if((insn & 0xF3E00000) == 0x40400000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            DoLds();
            var temp_198 = rt;
            if(temp_198 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_198),
                    builder.EnsureRuntime(builder.Call(Copcreg, builder.EnsureRuntime(cop),
                        builder.EnsureRuntime(rd))));
            return true;
        }

        /* COP */
        if((insn & 0xF2000000) == 0x42000000) {
            var cop = (insn >> 26) & 0x3U;
            var command = (insn >> 0) & 0x1FFFFFFU;
            DoLds();
            builder.Call(Copfun, builder.EnsureRuntime(cop), builder.EnsureRuntime(command));
            return true;
        }

        /* CTC */
        if((insn & 0xF3E00000) == 0x40C00000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_19 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            builder.Call(Copcreg, builder.EnsureRuntime(cop), builder.EnsureRuntime(rd),
                builder.EnsureRuntime(temp_19));
            return true;
        }

        /* DIV */
        if((insn & 0xFC00003F) == 0x0000001A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_20 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_21 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var rsv = temp_20.Store();
            var rtv = temp_21.Store();
            builder.If(
                builder.EnsureRuntime(rtv) == (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0),
                () => {
                    state.Lo((IRuntimeValue<uint>) builder.EnsureRuntime((IRuntimeValue<byte>) builder.Ternary(
                        builder.EnsureRuntime(builder.EnsureRuntime(rsv) & builder.EnsureRuntime(0x80000000U)) !=
                        (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0),
                        (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x1), builder.EnsureRuntime(0xFFFFFFFFU))));
                    state.Hi(builder.EnsureRuntime(rsv));
                },
                () => {
                    builder.If(
                        builder.EnsureRuntime(builder.EnsureRuntime(rsv) ==
                                              builder.EnsureRuntime(0x80000000U)) &
                        builder.EnsureRuntime(builder.EnsureRuntime(rtv) ==
                                              builder.EnsureRuntime(0xFFFFFFFFU)),
                        () => {
                            state.Lo(builder.EnsureRuntime(0x80000000U));
                            state.Hi((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0));
                        },
                        () => {
                            state.Lo((IRuntimeValue<uint>) builder.EnsureRuntime(
                                builder.EnsureRuntime((IRuntimeValue<int>) rsv) /
                                builder.EnsureRuntime(
                                    (IRuntimeValue<int>) rtv)));
                            state.Hi((IRuntimeValue<uint>) builder.EnsureRuntime(
                                builder.EnsureRuntime((IRuntimeValue<int>) rsv) %
                                builder.EnsureRuntime(
                                    (IRuntimeValue<int>) rtv)));
                            DivDelay();
                        });
                });
            return true;
        }

        /* DIVU */
        if((insn & 0xFC00003F) == 0x0000001B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_22 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_23 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var rsv = temp_22.Store();
            var rtv = temp_23.Store();
            builder.If(
                builder.EnsureRuntime(rtv) == (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0),
                () => {
                    state.Lo(builder.EnsureRuntime(0xFFFFFFFFU));
                    state.Hi(builder.EnsureRuntime(rsv));
                },
                () => {
                    state.Lo(builder.EnsureRuntime(builder.EnsureRuntime(rsv) /
                                                   builder.EnsureRuntime(rtv)));
                    state.Hi(builder.EnsureRuntime(builder.EnsureRuntime(rsv) %
                                                   builder.EnsureRuntime(rtv)));
                    DivDelay();
                });
            return true;
        }

        /* J */
        if((insn & 0xFC000000) == 0x08000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) + (imm << 0x2);
            DoLds();
            Branch(target);
            return true;
        }

        /* JAL */
        if((insn & 0xFC000000) == 0x0C000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) + (imm << 0x2);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime((byte) 0x1F), builder.LiteralValue(0U));
            DoLds();
            state.Registers(builder.LiteralValue(31), builder.EnsureRuntime(pc + 0x8));
            Branch(target);
            return true;
        }

        /* JALR */
        if((insn & 0xFC00003F) == 0x00000009) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            var temp_24 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var target = temp_24.Store();
            var temp_199 = rd;
            if(temp_199 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_199),
                    builder.EnsureRuntime(pc + 0x8));
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(target) &
                    builder.EnsureRuntime(
                        32 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            Branch(target);
            return true;
        }

        /* JR */
        if((insn & 0xFC00003F) == 0x00000008) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_25 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var target = temp_25.Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(target) &
                    builder.EnsureRuntime(
                        32 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            Branch(target);
            return true;
        }

        /* LB */
        if((insn & 0xFC000000) == 0x80000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_26 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue((IRuntimeValue<uint>) builder.EnsureRuntime(builder
                .Pointer<sbyte>(builder.EnsureRuntime(temp_26) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset))
                .Value));
            return true;
        }

        /* LBU */
        if((insn & 0xFC000000) == 0x90000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_27 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue((IRuntimeValue<uint>) builder.EnsureRuntime(builder
                .Pointer<byte>(builder.EnsureRuntime(temp_27) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset))
                .Value));
            return true;
        }

        /* LH */
        if((insn & 0xFC000000) == 0x84000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_28 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_28) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        16 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue((IRuntimeValue<uint>) builder.EnsureRuntime(builder.Pointer<short>(addr).Value));
            return true;
        }

        /* LHU */
        if((insn & 0xFC000000) == 0x94000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_29 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_29) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        16 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue((IRuntimeValue<uint>) builder.EnsureRuntime(builder.Pointer<ushort>(addr).Value));
            return true;
        }

        /* LUI */
        if((insn & 0xFC000000) == 0x3C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            DoLds();
            var temp_200 = rt;
            if(temp_200 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_200),
                    builder.EnsureRuntime(imm << 0x10));
            return true;
        }

        /* LW */
        if((insn & 0xFC000000) == 0x8C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_30 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_30) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        32 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue(builder.EnsureRuntime(builder.Pointer<uint>(addr).Value));
            return true;
        }

        /* LWL */
        if((insn & 0xFC000000) == 0x88000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_31 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            var temp_32 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLoad(rt, ref temp_31);
            var addr = (builder.EnsureRuntime(temp_32) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            var raddr = (builder.EnsureRuntime(addr) & builder.EnsureRuntime(0xFFFFFFFCU)).Store();
            var ert = temp_31.Store();
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue(builder.EnsureRuntime(
                (builder.EnsureRuntime(addr) & (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x3)) switch {
                    (IRuntimeValue<uint>) (byte) 0x0 => (IRuntimeValue<uint>) (builder.EnsureRuntime(
                                                                                   builder.EnsureRuntime(ert) &
                                                                                   builder.EnsureRuntime(0xFFFFFFU)) |
                                                                               builder.EnsureRuntime(
                                                                                   ((IRuntimeValue<uint>) builder
                                                                                       .Pointer<byte>(raddr).Value)
                                                                                   .LeftShift(
                                                                                       (IRuntimeValue<uint>) builder
                                                                                           .EnsureRuntime(
                                                                                               (byte) 0x18))))(
                        IRuntimeValue<uint>)((byte) 0x1)
                    
                    => (IRuntimeValue<uint>) (builder.EnsureRuntime(
                                                  builder
                                                      .EnsureRuntime(ert) &
                                                  (
                                                      IRuntimeValue<uint>) builder
                                                      .EnsureRuntime(
                                                          (ushort) 0xFFFF)) |
                                              builder.EnsureRuntime(
                                                  ((
                                                      IRuntimeValue<uint>) builder
                                                      .Pointer<ushort>(raddr).Value)
                                                  .LeftShift((IRuntimeValue<uint>) builder
                                                      .EnsureRuntime((byte) 0x10))))(
                        IRuntimeValue<uint>)((byte) 0x2)
                    
                    => builder.EnsureRuntime(
                           builder.EnsureRuntime(ert) &
                           (IRuntimeValue<uint>)
                           builder.EnsureRuntime((byte) 0xFF)) |
                       builder.EnsureRuntime(
                           builder.Pointer<uint>(raddr).Value
                               .LeftShift((IRuntimeValue<uint>) builder.EnsureRuntime(
                                   (byte) 0x8)))
                    _ => builder.Pointer<uint>(raddr).Value,
                }));
            return true;
        }

        /* LWR */
        if((insn & 0xFC000000) == 0x98000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_33 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            var temp_34 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLoad(rt, ref temp_33);
            var addr = (builder.EnsureRuntime(temp_34) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            var raddr = (builder.EnsureRuntime(addr) & builder.EnsureRuntime(0xFFFFFFFCU)).Store();
            var ert = temp_33.Store();
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue(builder.EnsureRuntime(
                (builder.EnsureRuntime(addr) & (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x3)) switch {
                    (IRuntimeValue<uint>) (byte) 0x0 => (IRuntimeValue<uint>) builder.Pointer<uint>(raddr)
                        .Value(IRuntimeValue<uint>)((byte) 0x1)
                    
                    => (IRuntimeValue<uint>) (builder.EnsureRuntime(
                                                  builder.EnsureRuntime(ert) &
                                                  builder.EnsureRuntime(0xFF000000U)) |
                                              builder.EnsureRuntime(
                                                  builder.EnsureRuntime(
                                                      builder.Pointer<uint>(addr).Value) &
                                                  builder.EnsureRuntime(0xFFFFFFU)))(
                        IRuntimeValue<uint>)(
                        (byte) 0x2)
                    
                    => builder.EnsureRuntime(
                           builder
                               .EnsureRuntime(ert) &
                           builder
                               .EnsureRuntime(0xFFFF0000U)) |
                       (IRuntimeValue<uint>) builder
                           .EnsureRuntime(
                               builder
                                   .Pointer<ushort>(addr).Value)
                    _ => builder.EnsureRuntime(
                             builder
                                 .EnsureRuntime(ert) &
                             builder
                                 .EnsureRuntime(0xFFFFFF00U)) |
                         (IRuntimeValue<uint>) builder.EnsureRuntime(
                             builder.Pointer<byte>(addr).Value),
                }));
            return true;
        }

        /* LWC2 */
        if((insn & 0xFC000000) == 0xC8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_35 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_35) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        32 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADEL), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            builder.Call(Copreg, (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), builder.EnsureRuntime(rt),
                builder.EnsureRuntime(builder.Pointer<uint>(addr).Value));
            return true;
        }

        /* MFC */
        if((insn & 0xF3E00000) == 0x40000000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            DoLds();
            state.LdWhich(builder.EnsureRuntime(rt));
            state.LdValue(builder.EnsureRuntime(builder.Call(Copreg, builder.EnsureRuntime(cop),
                builder.EnsureRuntime(rd))));
            return true;
        }

        /* MFHI */
        if((insn & 0xFC00003F) == 0x00000010) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            DoLds();
            var temp_203 = rd;
            if(temp_203 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_203),
                    builder.EnsureRuntime((IRuntimeValue<uint>) state.Hi()));
            AbsorbMuldivDelay();
            return true;
        }

        /* MFLO */
        if((insn & 0xFC00003F) == 0x00000012) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            DoLds();
            var temp_204 = rd;
            if(temp_204 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_204),
                    builder.EnsureRuntime((IRuntimeValue<uint>) state.Lo()));
            AbsorbMuldivDelay();
            return true;
        }

        /* MTC */
        if((insn & 0xF3E00000) == 0x40800000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_36 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            builder.Call(Copreg, builder.EnsureRuntime(cop), builder.EnsureRuntime(rd), builder.EnsureRuntime(temp_36));
            return true;
        }

        /* MTHI */
        if((insn & 0xFC00003F) == 0x00000011) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_37 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            state.Hi(builder.EnsureRuntime(temp_37));
            return true;
        }

        /* MTLO */
        if((insn & 0xFC00003F) == 0x00000013) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_38 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            state.Lo(builder.EnsureRuntime(temp_38));
            return true;
        }

        /* MULT */
        if((insn & 0xFC00003F) == 0x00000018) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_39 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_40 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var lhs = temp_39.Store();
            var rhs = temp_40.Store();
            var result =
                ((IRuntimeValue<ulong>) (builder.EnsureRuntime((IRuntimeValue<long>) lhs) *
                                         builder.EnsureRuntime((IRuntimeValue<long>) rhs))).Store();
            state.Lo((IRuntimeValue<uint>) builder.EnsureRuntime(result));
            state.Hi((IRuntimeValue<uint>) builder.EnsureRuntime(
                result.RightShift((IRuntimeValue<ulong>) builder.EnsureRuntime((byte) 0x20))));
            MulDelay(lhs, rhs, 0x1 != 0);
            return true;
        }

        /* MULTU */
        if((insn & 0xFC00003F) == 0x00000019) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_41 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_42 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var lhs = temp_41.Store();
            var rhs = temp_42.Store();
            var result = (builder.EnsureRuntime((IRuntimeValue<ulong>) lhs) *
                          builder.EnsureRuntime(
                              (IRuntimeValue<ulong>) rhs)).Store();
            state.Lo((IRuntimeValue<uint>) builder.EnsureRuntime(result));
            state.Hi((IRuntimeValue<uint>) builder.EnsureRuntime(
                result.RightShift((IRuntimeValue<ulong>) builder.EnsureRuntime((byte) 0x20))));
            MulDelay(lhs, rhs, 0x0 != 0);
            return true;
        }

        /* NOR */
        if((insn & 0xFC00003F) == 0x00000027) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_43 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_44 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_205 = rd;
            if(temp_205 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_205),
                    builder.EnsureRuntime(~(builder.EnsureRuntime(temp_43) | builder.EnsureRuntime(temp_44))));
            return true;
        }

        /* OR */
        if((insn & 0xFC00003F) == 0x00000025) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_45 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_46 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_206 = rd;
            if(temp_206 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_206),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_45) | builder.EnsureRuntime(temp_46)));
            return true;
        }

        /* ORI */
        if((insn & 0xFC000000) == 0x34000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_47 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_207 = rt;
            if(temp_207 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_207),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_47) | builder.EnsureRuntime(imm)));
            return true;
        }

        /* SB */
        if((insn & 0xFC000000) == 0xA0000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_48 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_49 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            builder.Pointer<byte>(builder.EnsureRuntime(temp_48) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset))
                .Value = (IRuntimeValue<byte>) temp_49;
            return true;
        }

        /* SH */
        if((insn & 0xFC000000) == 0xA4000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_50 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_51 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_50) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        16 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADES), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            builder.Pointer<ushort>(addr).Value = (IRuntimeValue<ushort>) temp_51;
            return true;
        }

        /* SLL */
        if((insn & 0xFC00003F) == 0x00000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_52 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_208 = rd;
            if(temp_208 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_208),
                    builder.EnsureRuntime(temp_52.LeftShift(builder.EnsureRuntime(shamt))));
            return true;
        }

        /* SLLV */
        if((insn & 0xFC00003F) == 0x00000004) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_53 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            var temp_54 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_209 = rd;
            if(temp_209 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_209),
                    builder.EnsureRuntime(temp_53.LeftShift(builder.EnsureRuntime(temp_54))));
            return true;
        }

        /* SLT */
        if((insn & 0xFC00003F) == 0x0000002A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_55 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_56 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_210 = rd;
            if(temp_210 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_210),
                    (IRuntimeValue<uint>) builder.EnsureRuntime(builder.EnsureRuntime((IRuntimeValue<int>) temp_55) <
                                                                builder.EnsureRuntime((IRuntimeValue<int>) temp_56)));
            return true;
        }

        /* SLTI */
        if((insn & 0xFC000000) == 0x28000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_57 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_211 = rt;
            if(temp_211 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_211),
                    (IRuntimeValue<uint>) builder.EnsureRuntime(builder.EnsureRuntime((IRuntimeValue<int>) temp_57) <
                                                                builder.EnsureRuntime(eimm)));
            return true;
        }

        /* SLTIU */
        if((insn & 0xFC000000) == 0x2C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_58 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_212 = rt;
            if(temp_212 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_212),
                    (IRuntimeValue<uint>) builder.EnsureRuntime(builder.EnsureRuntime(temp_58) <
                                                                builder.EnsureRuntime(eimm)));
            return true;
        }

        /* SLTU */
        if((insn & 0xFC00003F) == 0x0000002B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_59 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_60 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_213 = rd;
            if(temp_213 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_213),
                    (IRuntimeValue<uint>) builder.EnsureRuntime(builder.EnsureRuntime(temp_59) <
                                                                builder.EnsureRuntime(temp_60)));
            return true;
        }

        /* SRA */
        if((insn & 0xFC00003F) == 0x00000003) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_61 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_214 = rd;
            if(temp_214 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_214),
                    (IRuntimeValue<uint>) builder.EnsureRuntime(
                        ((IRuntimeValue<int>) temp_61).RightShift((IRuntimeValue<int>) builder.EnsureRuntime(shamt))));
            return true;
        }

        /* SRAV */
        if((insn & 0xFC00003F) == 0x00000007) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_62 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            var temp_63 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_215 = rd;
            if(temp_215 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_215),
                    (IRuntimeValue<uint>) builder.EnsureRuntime(
                        ((IRuntimeValue<int>) temp_62).RightShift(
                            builder.EnsureRuntime((IRuntimeValue<int>) temp_63))));
            return true;
        }

        /* SRL */
        if((insn & 0xFC00003F) == 0x00000002) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_64 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_216 = rd;
            if(temp_216 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_216),
                    builder.EnsureRuntime(temp_64.RightShift(builder.EnsureRuntime(shamt))));
            return true;
        }

        /* SRLV */
        if((insn & 0xFC00003F) == 0x00000006) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_65 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            var temp_66 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_217 = rd;
            if(temp_217 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_217),
                    builder.EnsureRuntime(temp_65.RightShift(builder.EnsureRuntime(temp_66))));
            return true;
        }

        /* SUB */
        if((insn & 0xFC00003F) == 0x00000022) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            var temp_67 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_68 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var lhs = temp_67.Store();
            var rhs = temp_68.Store();
            var r = (builder.EnsureRuntime(lhs) - builder.EnsureRuntime(rhs)).Store();
            builder.When(
                (builder.EnsureRuntime(builder.EnsureRuntime(lhs) ^ builder.EnsureRuntime(rhs)) &
                 builder.EnsureRuntime(builder.EnsureRuntime(lhs) ^ builder.EnsureRuntime(r)) &
                 builder.EnsureRuntime(0x80000000U)) != builder.Zero<uint>(),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.OV), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            var temp_218 = rd;
            if(temp_218 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_218),
                    builder.EnsureRuntime(builder.EnsureRuntime(lhs) -
                                          builder.EnsureRuntime(rhs)));
            return true;
        }

        /* SUBU */
        if((insn & 0xFC00003F) == 0x00000023) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_69 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_70 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_219 = rd;
            if(temp_219 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_219),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_69) -
                                          builder.EnsureRuntime(temp_70)));
            return true;
        }

        /* SW */
        if((insn & 0xFC000000) == 0xAC000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_71 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_72 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_71) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        32 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADES), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            builder.Pointer<uint>(addr).Value = temp_72;
            return true;
        }

        /* SWC2 */
        if((insn & 0xFC000000) == 0xE8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_73 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_73) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            builder.When(
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x0) !=
                builder.EnsureRuntime(
                    builder.EnsureRuntime(addr) &
                    builder.EnsureRuntime(
                        32 / (uint) 0x8 -
                        0x1)),
                () => {
                    builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.ADES), builder.LiteralValue(pc),
                        builder.LiteralValue(insn));
                });
            builder.Pointer<uint>(addr).Value = builder.Call(Copreg,
                (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2), builder.EnsureRuntime(rt));
            return true;
        }

        /* SWL */
        if((insn & 0xFC000000) == 0xA8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_74 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_75 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_74) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            var raddr = (builder.EnsureRuntime(addr) & builder.EnsureRuntime(0xFFFFFFFCU)).Store();
            var rtv = temp_75.Store();
            switch(builder.EnsureRuntime(addr) & (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x3)) {
                case (IRuntimeValue<uint>) (byte) 0x0: {
                    builder.Pointer<byte>(raddr).Value =
                        (IRuntimeValue<byte>) rtv.RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x18));
                    break;
                }
                case (IRuntimeValue<uint>) (byte) 0x1: {
                    builder.Pointer<ushort>(raddr).Value =
                        (IRuntimeValue<ushort>) rtv.RightShift(
                            (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x10));
                    break;
                }
                case (IRuntimeValue<uint>) (byte) 0x3: {
                    builder.Pointer<uint>(raddr).Value = rtv;
                    break;
                }
                default: {
                    builder.Pointer<ushort>(raddr).Value =
                        (IRuntimeValue<ushort>) rtv.RightShift((IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x8));
                    builder.Pointer<byte>(builder.EnsureRuntime(raddr) +
                                          (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2)).Value =
                        (IRuntimeValue<byte>) rtv.RightShift(
                            (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x18));
                    break;
                }
            }

            return true;
        }

        /* SWR */
        if((insn & 0xFC000000) == 0xB8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_76 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_77 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var addr = (builder.EnsureRuntime(temp_76) + (IRuntimeValue<uint>) builder.EnsureRuntime(offset)).Store();
            var raddr = (builder.EnsureRuntime(addr) & builder.EnsureRuntime(0xFFFFFFFCU)).Store();
            var rtv = temp_77.Store();
            switch(builder.EnsureRuntime(addr) & (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x3)) {
                case (IRuntimeValue<uint>) (byte) 0x0: {
                    builder.Pointer<uint>(raddr).Value = rtv;
                    break;
                }
                case (IRuntimeValue<uint>) (byte) 0x2: {
                    builder.Pointer<ushort>(raddr).Value = (IRuntimeValue<ushort>) rtv;
                    break;
                }
                case (IRuntimeValue<uint>) (byte) 0x3: {
                    builder.Pointer<byte>(raddr).Value = (IRuntimeValue<byte>) rtv;
                    break;
                }
                default: {
                    builder.Pointer<ushort>(raddr).Value = (IRuntimeValue<ushort>) rtv;
                    builder.Pointer<byte>(builder.EnsureRuntime(raddr) +
                                          (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x2)).Value =
                        (IRuntimeValue<byte>) rtv.RightShift(
                            (IRuntimeValue<uint>) builder.EnsureRuntime((byte) 0x10));
                    break;
                }
            }

            return true;
        }

        /* SYSCALL */
        if((insn & 0xFC00003F) == 0x0000000C) {
            var code = (insn >> 6) & 0xFFFFFU;
            DoLds();
            builder.Call(ThrowCpuException, builder.LiteralValue(ExceptionType.Syscall), builder.LiteralValue(pc),
                builder.LiteralValue(insn));
            return true;
        }

        /* XOR */
        if((insn & 0xFC00003F) == 0x00000026) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rd), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            var temp_78 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            var temp_79 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rt))).Store();
            DoLds();
            var temp_220 = rd;
            if(temp_220 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_220),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_78) ^ builder.EnsureRuntime(temp_79)));
            return true;
        }

        /* XORI */
        if((insn & 0xFC000000) == 0x38000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rt), builder.LiteralValue(0U));
            state.ReadAbsorb((IRuntimeValue<int>) builder.EnsureRuntime(rs), builder.LiteralValue(0U));
            var temp_80 =
                ((IRuntimeValue<uint>) state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(rs))).Store();
            DoLds();
            var temp_221 = rt;
            if(temp_221 != 0)
                state.Registers((IRuntimeValue<int>) builder.EnsureRuntime(temp_221),
                    builder.EnsureRuntime(builder.EnsureRuntime(temp_80) ^ builder.EnsureRuntime(imm)));
            return true;
        }

        return false;
    }
}