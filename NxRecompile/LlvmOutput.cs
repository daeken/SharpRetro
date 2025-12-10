using System.Diagnostics;
using System.Runtime.InteropServices;
using LibSharpRetro;
using LLVMSharp.Interop;
using System.Runtime.Intrinsics;
using Aarch64Cpu;
using LLVMSharp;
using StaticRecompilerBase;
using static NxRecompile.LlvmExtensions;

namespace NxRecompile;

public unsafe partial class CoreRecompiler {
    public void BuildAndLink(string objectDir, string libPath) {
        var modules = Output(objectDir);
        if(Sh.Run("clang", new[] { "-dynamiclib", "-o", libPath }.Concat(modules).ToArray()) != 0)
            Console.WriteLine($"Linking shared library failed!");
    }

    IEnumerable<string> Output(string objectDir) {
        LLVM.InitializeAllTargetInfos();
        LLVM.InitializeAllTargets();
        LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAllAsmParsers();
        LLVM.InitializeAllAsmPrinters();

        var targetTriple = LLVMTargetRef.DefaultTriple;
        var target = LLVMTargetRef.GetTargetFromTriple(targetTriple);
        var targetMachine = target.CreateTargetMachine(targetTriple, "generic", "",
            LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault, LLVMRelocMode.LLVMRelocDefault,
            LLVMCodeModel.LLVMCodeModelDefault);

        var name = Path.Join(objectDir, "module");
        new LlvmOutput().BuildModule(name, targetTriple, targetMachine,
            WholeBlockGraph.OrderBy(x => x.Key).Select(x => x.Value));
        yield return name + ".o";

        name = Path.Join(objectDir, "glue");
        BuildGlue(name, targetTriple, targetMachine);
        yield return name + ".o";
    }

    ulong ResolvePadding(ulong addr) {
        while(ProbablePadding.TryGetValue(addr, out var taddr))
            addr = taddr;
        return addr;
    }

    void BuildGlue(string path, string targetTriple, LLVMTargetMachineRef targetMachine) {
        var module = LLVMModuleRef.CreateWithName(Path.GetFileName(path));
        module.Target = targetTriple;

        var funcPtrType = LLVMTypeRef.CreatePointer(LlvmType<Func<ulong, ulong>>(), 0);
        var jumpTables = ExeLoader.ExeModules.Select(mod => {
            var funcs = new Dictionary<ulong, LLVMValueRef>();
            LLVMValueRef GetFunc(ulong addr) =>
                funcs.TryGetValue(addr, out var func)
                    ? func
                    : funcs[addr] = module.AddFunction($"f_{addr:X}", LlvmType<Func<ulong, ulong>>());
            var elems = new LLVMValueRef[(mod.TextEnd - mod.TextStart) / 4];
            var i = 0;
            for(var addr = mod.TextStart; addr < mod.TextEnd; addr += 4) {
                if(WholeBlockGraph.ContainsKey(addr))
                    elems[i++] = GetFunc(addr);
                else if(ProbablePadding.ContainsKey(addr))
                    elems[i++] = GetFunc(ResolvePadding(addr));
                else
                    elems[i++] = LLVMValueRef.CreateConstPointerNull(funcPtrType);
            }
            var jumpTable = module.AddGlobal(
                LLVMTypeRef.CreateArray(funcPtrType, (uint) elems.Length),
                $"jumpTable_{mod.LoadBase:X}"
            );
            jumpTable.Linkage = LLVMLinkage.LLVMInternalLinkage;
            jumpTable.Initializer = LLVMValueRef.CreateConstArray(funcPtrType, elems);
            return jumpTable;
        }).ToArray();

        var jumptableType = LLVMTypeRef.CreatePointer(funcPtrType, 0);
        var topTable = module.AddGlobal(
            LLVMTypeRef.CreateArray(jumptableType, (uint) jumpTables.Length),
            "jumpTable"
        );
        topTable.Linkage = LLVMLinkage.LLVMInternalLinkage;
        topTable.Initializer = LLVMValueRef.CreateConstArray(jumptableType, jumpTables);
        
        var function = module.AddFunction("runFrom", LlvmType<Action<ulong, ulong, ulong>>());
        function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        LLVMBuilderRef builder = LLVM.CreateBuilder();
        var passManager = LLVM.CreateFunctionPassManagerForModule(module);
        
        LLVM.AddReassociatePass(passManager);
        LLVM.AddCFGSimplificationPass(passManager);
        LLVM.AddGVNPass(passManager);
        LLVM.AddPromoteMemoryToRegisterPass(passManager);
        LLVM.AddSLPVectorizePass(passManager);
        LLVM.AddDeadStoreEliminationPass(passManager);
        LLVM.AddAggressiveDCEPass(passManager);
        LLVM.AddPartiallyInlineLibCallsPass(passManager);
        LLVM.InitializeFunctionPassManager(passManager);

        var entry = function.AppendBasicBlock("entry");
        var loop = function.AppendBasicBlock("loop");
        var end =  function.AppendBasicBlock("end");
        
        builder.PositionAtEnd(entry);
        var addr = builder.BuildAlloca(LlvmType<ulong>(), "addr");
        builder.BuildStore(function.GetParam(1), addr);
        builder.BuildCondBr(
            builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, builder.BuildLoad2(LlvmType<ulong>(), addr), function.GetParam(2)),
            end, loop
        );
        
        builder.PositionAtEnd(loop);
        var modIndex = builder.BuildLShr(builder.BuildSub(
            builder.BuildLoad2(LlvmType<ulong>(), addr),
            LLVMValueRef.CreateConstInt(LlvmType<ulong>(), 0x71_0000_0000UL)
        ), LLVMValueRef.CreateConstInt(LlvmType<ulong>(), 32UL));
        var tableIndex = builder.BuildLShr(builder.BuildAnd(
            builder.BuildLoad2(LlvmType<ulong>(), addr),
            LLVMValueRef.CreateConstInt(LlvmType<ulong>(), 0xFFFF_FFFFUL)
        ), LLVMValueRef.CreateConstInt(LlvmType<ulong>(), 2UL));

        var jumpTable = builder.BuildLoad2(jumptableType, 
            builder.BuildGEP2(
                jumptableType,
                topTable,
                [modIndex]
            ));
        var funcPtr = builder.BuildLoad2(funcPtrType, 
            builder.BuildGEP2(
                funcPtrType,
                jumpTable,
                [tableIndex]
            ));
        var naddr = builder.BuildCall2(LlvmType<Func<ulong, ulong>>(), funcPtr, [function.GetParam(0)]);
        builder.BuildStore(naddr, addr);
        
        builder.BuildCondBr(
            builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, builder.BuildLoad2(LlvmType<ulong>(), addr), function.GetParam(2)),
            end, loop
        );
        
        builder.PositionAtEnd(end);
        builder.BuildRetVoid();
        
        //LLVM.DumpValue(function);
        LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
        if(!function.VerifyFunction(LLVMVerifierFailureAction.LLVMReturnStatusAction))
            throw new("Program verification failed");
        LLVM.RunFunctionPassManager(passManager, function);
        //LLVM.DumpValue(function);
        
        targetMachine.EmitToFile(module, path + ".o", LLVMCodeGenFileType.LLVMObjectFile);
    }
}

unsafe class LlvmOutput {
    LLVMBuilderRef Builder;
    LLVMValueRef Function, RunFrom, CpuStateRef;
    LLVMValueRef BitReverse8, BitReverse16, BitReverse32, BitReverse64;
    LLVMValueRef Clz8, Clz16, Clz32, Clz64;
    LLVMValueRef Abs8, Abs16, Abs32, Abs64, Fabs32, Fabs64;
    LLVMValueRef Trunc32, Trunc64, Round32, Round64, Ceil32, Ceil64, Floor32, Floor64;
    LLVMBasicBlockRef CurrentBlock;
    readonly Dictionary<string, LLVMValueRef> Locals = new();
    LLVMValueRef[] Gprs;
    LLVMValueRef[] Vecs;
    LLVMValueRef SP, N, Z, C, V, TlsBase;
    
    internal void BuildModule(string path, string targetTriple, LLVMTargetMachineRef targetMachine, IEnumerable<BlockGraph> nodes) {
        var module = LLVMModuleRef.CreateWithName(Path.GetFileName(path));
        module.Target = targetTriple;
        // TODO: How tf do you set the data layout? It should come from targetMachine somehow

        RunFrom = module.AddFunction("runFrom", LlvmType<Action<ulong, ulong, ulong>>());
        RunFrom.Linkage = LLVMLinkage.LLVMExternalLinkage;
        BitReverse8 = module.AddFunction("llvm.bitreverse.i8",  LlvmType<Func<byte, byte>>());
        BitReverse16 = module.AddFunction("llvm.bitreverse.i16",  LlvmType<Func<ushort, ushort>>());
        BitReverse32 = module.AddFunction("llvm.bitreverse.i32",  LlvmType<Func<uint, uint>>());
        BitReverse64 = module.AddFunction("llvm.bitreverse.i64",  LlvmType<Func<ulong, ulong>>());
        Clz8 = module.AddFunction("llvm.ctlz.i8", LlvmType<Func<byte, Bit, byte>>());
        Clz16 = module.AddFunction("llvm.ctlz.i16", LlvmType<Func<ushort, Bit, ushort>>());
        Clz32 = module.AddFunction("llvm.ctlz.i32", LlvmType<Func<uint, Bit, uint>>());
        Clz64 = module.AddFunction("llvm.ctlz.i64", LlvmType<Func<ulong, Bit, ulong>>());
        Abs8 = module.AddFunction("llvm.abs.i8", LlvmType<Func<byte, Bit, byte>>());
        Abs16 = module.AddFunction("llvm.abs.i16", LlvmType<Func<ushort, Bit, ushort>>());
        Abs32 = module.AddFunction("llvm.abs.i32", LlvmType<Func<uint, Bit, uint>>());
        Abs64 = module.AddFunction("llvm.abs.i64", LlvmType<Func<ulong, Bit, ulong>>());
        Fabs32 = module.AddFunction("llvm.fabs.f32", LlvmType<Func<float, float>>());
        Fabs64 = module.AddFunction("llvm.fabs.f64", LlvmType<Func<double, double>>());
        Trunc32 = module.AddFunction("llvm.trunc.f32", LlvmType<Func<float, float>>());
        Trunc64 = module.AddFunction("llvm.trunc.f64", LlvmType<Func<double, double>>());
        Round32 = module.AddFunction("llvm.round.f32", LlvmType<Func<float, float>>());
        Round64 = module.AddFunction("llvm.round.f64", LlvmType<Func<double, double>>());
        Ceil32 = module.AddFunction("llvm.ceil.f32", LlvmType<Func<float, float>>());
        Ceil64 = module.AddFunction("llvm.ceil.f64", LlvmType<Func<double, double>>());
        Floor32 = module.AddFunction("llvm.floor.f32", LlvmType<Func<float, float>>());
        Floor64 = module.AddFunction("llvm.floor.f64", LlvmType<Func<double, double>>());

        var funcs = nodes.Select(node => (node.Block.Start, BuildFunction(module, node))).ToList();
        
        targetMachine.EmitToFile(module, path + ".o", LLVMCodeGenFileType.LLVMObjectFile);
    }

    LLVMValueRef BuildFunction(LLVMModuleRef module, BlockGraph node) {
        Locals.Clear();
        Builder = LLVM.CreateBuilder();
        var passManager = LLVM.CreateFunctionPassManagerForModule(module);
        
        LLVM.AddReassociatePass(passManager);
        LLVM.AddCFGSimplificationPass(passManager);
        LLVM.AddGVNPass(passManager);
        LLVM.AddPromoteMemoryToRegisterPass(passManager);
        LLVM.AddSLPVectorizePass(passManager);
        LLVM.AddDeadStoreEliminationPass(passManager);
        LLVM.AddAggressiveDCEPass(passManager);
        LLVM.AddPartiallyInlineLibCallsPass(passManager);
        LLVM.InitializeFunctionPassManager(passManager);
        
        Function = module.AddFunction($"f_{node.Block.Start:X}", LlvmType<Func<ulong, ulong>>());
        CpuStateRef = LLVM.GetParam(Function, 0);
        Function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        Function.Visibility = LLVMVisibility.LLVMHiddenVisibility;
        LLVM.PositionBuilderAtEnd(Builder, CurrentBlock = Function.AppendBasicBlock("entrypoint"));

        /*Gprs = Enumerable.Range(0, 31)
            .Select(LLVMValueRef (_) => LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString)).ToArray();
        Vecs = Enumerable.Range(0, 32)
            .Select(LLVMValueRef (_) => LLVM.BuildAlloca(Builder, LlvmType<Vector128<float>>(), EmptyString)).ToArray();
        SP = LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString);
        N = LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString);
        Z = LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString);
        C = LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString);
        V = LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString);
        TlsBase = LLVM.BuildAlloca(Builder, LlvmType<ulong>(), EmptyString);*/

        node.Block.Body.Walk(stmt => {
            if(stmt is not StaticIRStatement.Assign(var name, var value)) return;
            if(!Locals.ContainsKey(name))
                Locals[name] = Builder.BuildAlloca(value.Type.ToLLVMType(), name);
        });

        var body = new StaticIRStatement.Body(node.Block.Body);
        Compile(body);

        //LLVM.DumpValue(Function);
        LLVM.VerifyFunction(Function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
        if(!Function.VerifyFunction(LLVMVerifierFailureAction.LLVMReturnStatusAction))
            throw new("Program verification failed");
        LLVM.RunFunctionPassManager(passManager, Function);
        //LLVM.DumpValue(Function);

        return Function;
    }

    bool Compile(StaticIRStatement stmt) {
        switch(stmt) {
            case StaticIRStatement.Body(var stmts):
                foreach(var sub in stmts)
                    if(Compile(sub))
                        return true;
                return false;
            case StaticIRStatement.If(var cond, var taken, var not): {
                var ifLabel = Function.AppendBasicBlock("");
                var elseLabel = Function.AppendBasicBlock("");
                LLVMBasicBlockRef endLabel = null;
                var asBool = Builder.BuildCast(LLVMOpcode.LLVMTrunc, Compile(cond), LLVMTypeRef.Int1);
                Builder.BuildCondBr(asBool, ifLabel, elseLabel);
                
                Builder.PositionAtEnd(CurrentBlock = ifLabel);
                var takenBranches = Compile(taken);
                if(!takenBranches) {
                    if(endLabel == null)
                        endLabel = Function.AppendBasicBlock("");
                    Builder.BuildBr(endLabel);
                }
                Builder.PositionAtEnd(CurrentBlock = elseLabel);
                var notBranches = Compile(not);
                if(!notBranches) {
                    if(endLabel == null)
                        endLabel = Function.AppendBasicBlock("");
                    Builder.BuildBr(endLabel);
                }
                if(!takenBranches || !notBranches)
                    Builder.PositionAtEnd(CurrentBlock = endLabel);
                return takenBranches && notBranches;
            }
            case StaticIRStatement.When(var cond, var taken): {
                var ifLabel = Function.AppendBasicBlock("");
                var endLabel = Function.AppendBasicBlock("");
                var asBool = Builder.BuildCast(LLVMOpcode.LLVMTrunc, Compile(cond), LLVMTypeRef.Int1);
                Builder.BuildCondBr(asBool, ifLabel, endLabel);
                
                Builder.PositionAtEnd(CurrentBlock = ifLabel);
                var takenBranches = Compile(taken);
                if(!takenBranches)
                    Builder.BuildBr(endLabel);
                Builder.PositionAtEnd(CurrentBlock = endLabel);
                return false;
            }
            case StaticIRStatement.Unless(var cond, var taken): {
                var ifLabel = Function.AppendBasicBlock("");
                var endLabel = Function.AppendBasicBlock("");
                var asBool = Builder.BuildCast(LLVMOpcode.LLVMTrunc, Compile(cond), LLVMTypeRef.Int1);
                Builder.BuildCondBr(Builder.BuildNot(asBool), ifLabel, endLabel);
                
                Builder.PositionAtEnd(CurrentBlock = ifLabel);
                var takenBranches = Compile(taken);
                if(!takenBranches)
                    Builder.BuildBr(endLabel);
                Builder.PositionAtEnd(CurrentBlock = endLabel);
                return false;
            }
            case StaticIRStatement.DoWhile(var taken, var cond): {
                var loopLabel = Function.AppendBasicBlock("");
                var endLabel = Function.AppendBasicBlock("");
                Builder.BuildBr(loopLabel);
                
                Builder.PositionAtEnd(CurrentBlock = loopLabel);
                var takenBranches = Compile(taken);
                if(!takenBranches) { // TODO: Figure out why this would ever not happen
                    var asBool = Builder.BuildCast(LLVMOpcode.LLVMTrunc, Compile(cond), LLVMTypeRef.Int1);
                    Builder.BuildCondBr(asBool, loopLabel, endLabel);
                }
                Builder.PositionAtEnd(CurrentBlock = endLabel);
                return false;
            }
            case StaticIRStatement.Assign(var name, var value):
                Builder.BuildStore(Compile(value), Locals[name]);
                return false;
            case StaticIRStatement.SetFieldIndex(StaticIRValue.Named("State", _), var name, var index, var value): {
                var offset = (ulong) Marshal.OffsetOf<CpuState>(name) + (ulong) (value.Type.ByteCount * index);
                var addr = Builder.BuildAdd(CpuStateRef, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset));
                var ptr = Builder.BuildIntToPtr(addr, LLVM.PointerType(value.Type.ToLLVMType(), 0), $"{name}[{index}]");
                Builder.BuildStore(Compile(value), ptr);
                return false;
            }
            case StaticIRStatement.SetField(StaticIRValue.Named("State", _), var name, var value): {
                var offset = (ulong) Marshal.OffsetOf<CpuState>(name);
                var addr = Builder.BuildAdd(CpuStateRef, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset));
                var ptr = Builder.BuildIntToPtr(addr, LLVM.PointerType(value.Type.ToLLVMType(), 0), name);
                Builder.BuildStore(Compile(value), ptr);
                return false;
            }
            case StaticIRStatement.Dereference(var addr, var value): {
                var ptr = Builder.BuildIntToPtr(Compile(addr), LLVM.PointerType(value.Type.ToLLVMType(), 0));
                Builder.BuildStore(Compile(value), ptr);
                return false;
            }
            case StaticIRStatement.Branch(var addr):
                Builder.BuildRet(Compile(addr));
                return true;
            case LinkedBranch(var target):
                Builder.BuildCall2(LlvmType<Action<ulong, ulong, ulong>>(), RunFrom, [
                    CpuStateRef, 
                    Compile(target), 
                    Compile(new StaticIRValue.GetFieldIndex(
                        new StaticIRValue.Named("State", typeof(void)), 
                        "X", 30, typeof(ulong))
                    ),
                ]);
                return false;
            case WriteSrStmt:
                // TODO: Implement
                return false;
            case SvcStmt:
                // TODO: Implement
                return false;
            default:
                throw new NotSupportedException($"Unknown statement: {stmt}");
        }
    }

    LLVMValueRef Compare(StaticIRValue comp, StaticIRValue left, StaticIRValue right) {
        var op = comp switch {
            StaticIRValue.EQ => LLVMIntPredicate.LLVMIntEQ, 
            StaticIRValue.NE => LLVMIntPredicate.LLVMIntNE,
            StaticIRValue.GT => left.Type.IsSigned ? LLVMIntPredicate.LLVMIntSGT : LLVMIntPredicate.LLVMIntUGT,
            StaticIRValue.GTE => left.Type.IsSigned ? LLVMIntPredicate.LLVMIntSGE : LLVMIntPredicate.LLVMIntUGE,
            StaticIRValue.LT => left.Type.IsSigned ? LLVMIntPredicate.LLVMIntSLT : LLVMIntPredicate.LLVMIntULT,
            StaticIRValue.LTE => left.Type.IsSigned ? LLVMIntPredicate.LLVMIntSLE : LLVMIntPredicate.LLVMIntULE,
            _ => throw new NotImplementedException($"Unknown operator: {comp}"),
        };
        var val = Builder.BuildICmp(op, Compile(left), Compile(right));
        return Builder.BuildCast(LLVMOpcode.LLVMZExt, val, LlvmType<ulong>());
    }

    LLVMValueRef Compile(StaticIRValue value) {
        var llvmValue = SubCompile(value);
        if(llvmValue.TypeOf != value.Type.ToLLVMType())
            throw new NotSupportedException($"{value} goes from {value.Type} ({value.Type.ToLLVMType().PrintToString()}) to {llvmValue.TypeOf.PrintToString()}");
        return llvmValue;
    }

    LLVMValueRef SubCompile(StaticIRValue value) {
        unchecked {
            switch(value) {
                case StaticIRValue.Store(var left):
                    return Compile(left);
                case StaticIRValue.Literal(var lval, var type):
                    return lval switch {
                        byte v => LLVMValueRef.CreateConstInt(LlvmType<byte>(), v),
                        sbyte v => LLVMValueRef.CreateConstInt(LlvmType<byte>(), (byte) v),
                        ushort v => LLVMValueRef.CreateConstInt(LlvmType<ushort>(), v),
                        short v => LLVMValueRef.CreateConstInt(LlvmType<short>(), (ushort) v),
                        uint v => LLVMValueRef.CreateConstInt(LlvmType<uint>(), v),
                        int v => LLVMValueRef.CreateConstInt(LlvmType<int>(), (uint) v),
                        ulong v => LLVMValueRef.CreateConstInt(LlvmType<ulong>(), v),
                        long v => LLVMValueRef.CreateConstInt(LlvmType<long>(), (ulong) v),
                        UInt128 v => LLVMValueRef.CreateConstIntOfArbitraryPrecision(LlvmType<UInt128>(), [
                            (ulong) v,
                            (ulong) (v >> 64),
                        ]),
                        Int128 v => LLVMValueRef.CreateConstIntOfArbitraryPrecision(LlvmType<UInt128>(), [
                            (ulong) (UInt128) v,
                            (ulong) ((UInt128) v >> 64),
                        ]),
                        float v => LLVMValueRef.CreateConstReal(LlvmType<float>(), v),
                        double v => LLVMValueRef.CreateConstReal(LlvmType<double>(), v),
                        bool v => LLVMValueRef.CreateConstInt(LlvmType<ulong>(), v ? 1UL : 0UL),
                        _ => throw new NotSupportedException($"Unsupported literal type: {value}"),
                    };
                case StaticIRValue.Named(var name, var type):
                    return Builder.BuildLoad2(type.ToLLVMType(), Locals[name]);
                case StaticIRValue.GetField(StaticIRValue.Named("State", _), var name, var type): {
                    var offset = (ulong) Marshal.OffsetOf<CpuState>(name);
                    var addr = Builder.BuildAdd(CpuStateRef, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset));
                    var ptr = Builder.BuildIntToPtr(addr, LLVM.PointerType(type.ToLLVMType(), 0));
                    return Builder.BuildLoad2(type.ToLLVMType(), ptr, name);
                }
                case StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), var name, var index, var type): {
                    var offset = (ulong) Marshal.OffsetOf<CpuState>(name) + (ulong) (type.ByteCount * index);
                    var addr = Builder.BuildAdd(CpuStateRef, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset));
                    var ptr = Builder.BuildIntToPtr(addr, LLVM.PointerType(type.ToLLVMType(), 0));
                    return Builder.BuildLoad2(type.ToLLVMType(), ptr, $"{name}[{index}]");
                }
                case StaticIRValue.Dereference(var left, var type): {
                    var ptr = Builder.BuildIntToPtr(Compile(left), LLVM.PointerType(type.ToLLVMType(), 0));
                    return Builder.BuildLoad2(type.ToLLVMType(), ptr);
                }
                case StaticIRValue.Add(var left, var right):
                    return left.Type.IsFloat
                        ? Builder.BuildFAdd(Compile(left), Compile(right))
                        : Builder.BuildAdd(Compile(left), Compile(right));
                case StaticIRValue.Sub(var left, var right):
                    return left.Type.IsFloat
                        ? Builder.BuildFSub(Compile(left), Compile(right))
                        : Builder.BuildSub(Compile(left), Compile(right));
                case StaticIRValue.Mul(var left, var right):
                    return left.Type.IsFloat
                        ? Builder.BuildFMul(Compile(left), Compile(right))
                        : Builder.BuildMul(Compile(left), Compile(right));
                case StaticIRValue.Div(var left, var right):
                    return left.Type.IsFloat
                        ? Builder.BuildFDiv(Compile(left), Compile(right))
                        : left.Type.IsSigned
                            ? Builder.BuildSDiv(Compile(left), Compile(right))
                            : Builder.BuildUDiv(Compile(left), Compile(right));
                case StaticIRValue.Mod(var left, var right):
                    return left.Type.IsFloat
                        ? Builder.BuildFRem(Compile(left), Compile(right))
                        : left.Type.IsSigned
                            ? Builder.BuildSRem(Compile(left), Compile(right))
                            : Builder.BuildURem(Compile(left), Compile(right));
                case StaticIRValue.And(var left, var right):
                    return left.Type.IsVector
                        ? Builder.BuildBitCast(Builder.BuildAnd(
                            Builder.BuildBitCast(Compile(left), LlvmType<Vector128<ulong>>()), 
                            Builder.BuildBitCast(Compile(right), LlvmType<Vector128<ulong>>()) 
                        ), left.Type.ToLLVMType())
                        : Builder.BuildAnd(Compile(left), Compile(right));
                case StaticIRValue.Or(var left, var right):
                    return left.Type.IsVector
                        ? Builder.BuildBitCast(Builder.BuildOr(
                            Builder.BuildBitCast(Compile(left), LlvmType<Vector128<ulong>>()), 
                            Builder.BuildBitCast(Compile(right), LlvmType<Vector128<ulong>>()) 
                        ), left.Type.ToLLVMType())
                        : Builder.BuildOr(Compile(left), Compile(right));
                case StaticIRValue.Xor(var left, var right):
                    return left.Type.IsVector
                        ? Builder.BuildBitCast(Builder.BuildXor(
                                Builder.BuildBitCast(Compile(left), LlvmType<Vector128<ulong>>()), 
                                Builder.BuildBitCast(Compile(right), LlvmType<Vector128<ulong>>()) 
                            ), left.Type.ToLLVMType())
                        : Builder.BuildXor(Compile(left), Compile(right));
                case StaticIRValue.LeftShift(var left, var right):
                    return Builder.BuildShl(Compile(left), Compile(right));
                case StaticIRValue.RightShift(var left, var right):
                    return left.Type.IsSigned
                        ? Builder.BuildAShr(Compile(left), Compile(right))
                        : Builder.BuildLShr(Compile(left), Compile(right));
                case StaticIRValue.Negate(var left):
                    return left.Type.IsFloat
                        ? Builder.BuildFNeg(Compile(left))
                        : Builder.BuildNeg(Compile(left));
                case StaticIRValue.EQ(var left, var right): return Compare(value, left, right);
                case StaticIRValue.NE(var left, var right): return Compare(value, left, right);
                case StaticIRValue.GT(var left, var right): return Compare(value, left, right);
                case StaticIRValue.GTE(var left, var right): return Compare(value, left, right);
                case StaticIRValue.LT(var left, var right): return Compare(value, left, right);
                case StaticIRValue.LTE(var left, var right): return Compare(value, left, right);
                case StaticIRValue.Not(var left):
                    return left.Type.IsVector
                        ? Builder.BuildBitCast(Builder.BuildNot(
                            Builder.BuildBitCast(Compile(left), LlvmType<Vector128<ulong>>()) 
                        ), left.Type.ToLLVMType())
                        : Builder.BuildNot(Compile(left));
                case StaticIRValue.Cast(var left, var type): {
                    if(left.Type == type) return Compile(left);
                    var fw = left.Type.ByteCount;
                    var sw = type.ByteCount;
                    var fs = left.Type.IsSigned;
                    var ss = type.IsSigned;
                    if(left.Type.IsFloat && !left.Type.IsVector) {
                        if(type.IsFloat)
                            return Builder.BuildFPCast(Compile(left), type.ToLLVMType());
                        return type.IsSigned
                            ? Builder.BuildFPToSI(Compile(left), type.ToLLVMType())
                            : Builder.BuildFPToUI(Compile(left), type.ToLLVMType());
                    }
                    if(type.IsFloat && !type.IsVector) {
                        return type.IsSigned
                            ? Builder.BuildSIToFP(Compile(left), type.ToLLVMType())
                            : Builder.BuildUIToFP(Compile(left), type.ToLLVMType());
                    }

                    var opcode = LLVMOpcode.LLVMTrunc;
                    if(fw == sw)
                        opcode = LLVMOpcode.LLVMBitCast;
                    else if(fw < sw)
                        opcode = (fs && !ss) || (fs && ss) ? LLVMOpcode.LLVMSExt : LLVMOpcode.LLVMZExt;

                    return Builder.BuildCast(opcode, Compile(left), type.ToLLVMType());
                }
                case StaticIRValue.Bitcast(var left, var type):
                    return Builder.BuildBitCast(Compile(left), type.ToLLVMType());
                case StaticIRValue.SignExt(var left, var width, var type): {
                    var totalBits = type.ByteCount * 8;
                    var shift = LLVMValueRef.CreateConstInt(type.ToLLVMType(), (ulong) (totalBits - width));
                    var val = Builder.BuildSExtOrBitCast(Compile(left), type.ToLLVMType());
                    return Builder.BuildAShr(Builder.BuildShl(val, shift), shift);
                }
                case StaticIRValue.ReverseBits(var left):
                    return left.Type.ByteCount switch {
                        1 => Builder.BuildCall2(LlvmType<Func<byte, byte>>(), BitReverse8, [Compile(left)]),
                        2 => Builder.BuildCall2(LlvmType<Func<ushort, ushort>>(), BitReverse16, [Compile(left)]),
                        4 => Builder.BuildCall2(LlvmType<Func<uint, uint>>(), BitReverse32, [Compile(left)]),
                        8 => Builder.BuildCall2(LlvmType<Func<ulong, ulong>>(), BitReverse64, [Compile(left)]),
                        _ => throw new NotImplementedException($"Can't bitreverse {left.Type}"),
                    };
                case StaticIRValue.CountLeadingZeros(var left):
                    return left.Type.ByteCount switch {
                        1 => Builder.BuildCall2(LlvmType<Func<byte, Bit, byte>>(), Clz8, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                        2 => Builder.BuildCall2(LlvmType<Func<ushort, Bit, ushort>>(), Clz16, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                        4 => Builder.BuildCall2(LlvmType<Func<uint, Bit, uint>>(), Clz32, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                        8 => Builder.BuildCall2(LlvmType<Func<ulong, Bit, ulong>>(), Clz64, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                        _ => throw new NotImplementedException($"Can't clz {left.Type}"),
                    };
                case StaticIRValue.Abs(var left):
                    return !left.Type.IsFloat
                        ? left.Type.ByteCount switch {
                            1 => Builder.BuildCall2(LlvmType<Func<byte, Bit, byte>>(), Abs8, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                            2 => Builder.BuildCall2(LlvmType<Func<ushort, Bit, ushort>>(), Abs16, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                            4 => Builder.BuildCall2(LlvmType<Func<uint, Bit, uint>>(), Abs32, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                            8 => Builder.BuildCall2(LlvmType<Func<ulong, Bit, ulong>>(), Abs64, [Compile(left), LLVMValueRef.CreateConstInt(LlvmType<Bit>(), 0)]),
                            _ => throw new NotImplementedException($"Can't abs {left.Type}"),
                        }
                        : left.Type.ByteCount switch {
                            4 => Builder.BuildCall2(LlvmType<Func<float, float>>(), Fabs32, [Compile(left)]),
                            8 => Builder.BuildCall2(LlvmType<Func<double, double>>(), Fabs64, [Compile(left)]),
                            _ => throw new NotImplementedException($"Can't abs {left.Type}"),
                        };
                case StaticIRValue.Round(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Round32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Round64, [Compile(left)]);
                case StaticIRValue.RoundTowardZero(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Trunc32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Trunc64, [Compile(left)]);
                case StaticIRValue.Ternary(var cond, var a, var b): {
                    var ifLabel = Function.AppendBasicBlock("");
                    var elseLabel = Function.AppendBasicBlock("");
                    var endLabel = Function.AppendBasicBlock("");

                    var asBool = Builder.BuildCast(LLVMOpcode.LLVMTrunc, Compile(cond), LLVMTypeRef.Int1);
                    LLVM.BuildCondBr(Builder, asBool, ifLabel, elseLabel);
		
                    Builder.PositionAtEnd(CurrentBlock = ifLabel);
                    var left = Compile(a);
                    var leftBlockEnd = CurrentBlock;
                    LLVM.BuildBr(Builder, endLabel);

                    Builder.PositionAtEnd(CurrentBlock = elseLabel);
                    var right = Compile(b);
                    var rightBlockEnd = CurrentBlock;
                    LLVM.BuildBr(Builder, endLabel);
		
                    Builder.PositionAtEnd(CurrentBlock = endLabel);
                    var phi = Builder.BuildPhi(a.Type.ToLLVMType());
                    phi.AddIncoming(new[] { left, right }, new[] { leftBlockEnd, rightBlockEnd }, 2);
                    return phi;
                }
                case StaticIRValue.CreateVector(var val): {
                    var elem = Compile(val);
                    var count = 16 / val.Type.ByteCount;
                    LLVMValueRef lvec;
                    if(elem.IsConstant)
                        lvec = LLVMValueRef.CreateConstVector(Enumerable.Range(0, count).Select(_ => elem).ToArray());
                    else {
                        lvec = LLVM.GetUndef(
                            typeof(Vector128<>).MakeGenericType(val.Type).ToLLVMType());
                        for(var i = 0; i < count; ++i)
                            lvec = Builder.BuildInsertElement(lvec, elem,
                                LLVMValueRef.CreateConstInt(LlvmType<int>(), (ulong) i));
                    }
                    return lvec;
                }
                case StaticIRValue.CreateFullVector(var values): {
                    var lvalues = values.Select(Compile).ToArray();
                    LLVMValueRef lvec;
                    if(lvalues.All(x => x.IsConstant))
                        lvec = LLVMValueRef.CreateConstVector(lvalues);
                    else {
                        lvec = LLVM.GetUndef(
                            typeof(Vector128<>).MakeGenericType(values[0].Type).ToLLVMType());
                        for(var i = 0; i < lvalues.Length; ++i)
                            lvec = Builder.BuildInsertElement(lvec, lvalues[i],
                                LLVMValueRef.CreateConstInt(LlvmType<int>(), (ulong) i));
                    }
                    return values[0].Type == typeof(float)
                        ? lvec
                        : Builder.BuildBitCast(lvec, LlvmType<Vector128<float>>());
                }
                case StaticIRValue.ZeroTop(var left):
                    return Compile(left); // TODO: Implement this. Or don't; it's probably fine.
                case StaticIRValue.SetElement(var vec, var index, var elem): {
                    if(elem.Type == vec.Type.GetGenericArguments()[0])
                        return Builder.BuildInsertElement(Compile(vec), Compile(elem), Compile(index));
                    var bvec = Builder.BuildBitCast(Compile(vec), typeof(Vector128<>).MakeGenericType(elem.Type).ToLLVMType());
                    bvec = Builder.BuildInsertElement(bvec, Compile(elem), Compile(index));
                    return Builder.BuildBitCast(bvec, LlvmType<Vector128<float>>());
                }
                case StaticIRValue.GetElement(var vec, var index, var type): {
                    var lvec = type == typeof(float)
                        ? Compile(vec)
                        : Builder.BuildBitCast(Compile(vec), typeof(Vector128<>).MakeGenericType(type).ToLLVMType());
                    return Builder.BuildExtractElement(lvec, Compile(index));
                }
                case ReadSr(var op0, var op1, var crn, var crm, var op2):
                    return LLVMValueRef.CreateConstInt(LlvmType<ulong>(), 0); // TODO: Implement
                default:
                    throw new NotSupportedException($"Unknown value: {value}");
            }
        }
    }
}