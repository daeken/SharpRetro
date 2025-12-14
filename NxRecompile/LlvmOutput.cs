using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using LibSharpRetro;
using LLVMSharp.Interop;
using System.Runtime.Intrinsics;
using Aarch64Cpu;
using DoubleSharp.Linq;
using DoubleSharp.Progress;
using LLVMSharp;
using StaticRecompilerBase;
using static NxRecompile.LlvmExtensions;

namespace NxRecompile;

public unsafe partial class CoreRecompiler {
    public void BuildAndLink(string objectDir, string libPath) {
        var modules = Output(objectDir);
        if(Sh.Run("clang", new[] { "-dynamiclib", "-o", libPath, "-exported_symbols_list", "exports.txt" }.Concat(modules).ToArray()) != 0)
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

        var blocks = WholeBlockGraph.OrderBy(x => x.Key)
            .Select(x => x.Value).ToList();
        var cut = 10_000;
        var iter = (0, blocks.Count, cut).Range().ToList().WithTimedProgress(constantUpdate: true)
            .Select(i => {
                var name = Path.Join(objectDir, $"module_{i / cut}");
                new LlvmOutput().BuildModule(name, targetTriple, targetMachine,
                    blocks.Skip(i).Take(cut)
                        .Select(x => (BlockNames[x.Block.Start], x))
                );
                return name + ".o";
            });
        foreach(var elem in iter) yield return elem;

        var fname = Path.Join(objectDir, "glue");
        BuildGlue(fname, targetTriple, targetMachine);
        yield return fname + ".o";
    }

    ulong ResolvePadding(ulong addr) {
        while(ProbablePadding.TryGetValue(addr, out var taddr))
            addr = taddr;
        return addr;
    }

    void BuildGlue(string path, string targetTriple, LLVMTargetMachineRef targetMachine) {
        var module = LLVMModuleRef.CreateWithName(Path.GetFileName(path));
        module.Target = targetTriple;

        var funcPtrType = LlvmType<Func<ulong, ulong>>().ToPointer();
        var jumpTables = ExeLoader.ExeModules.Select(mod => {
            var funcs = new Dictionary<ulong, LLVMValueRef>();
            LLVMValueRef GetFunc(ulong addr) =>
                funcs.TryGetValue(addr, out var func)
                    ? func
                    : funcs[addr] = module.AddFunction(BlockNames[addr], LlvmType<Func<ulong, ulong>>());
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

        var jumptableType = funcPtrType.ToPointer();
        var topTable = module.AddGlobal(
            LLVMTypeRef.CreateArray(jumptableType, (uint) jumpTables.Length),
            "jumpTable"
        );
        topTable.Linkage = LLVMLinkage.LLVMInternalLinkage;
        topTable.Initializer = LLVMValueRef.CreateConstArray(jumptableType, jumpTables);
        
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

        var function = module.AddFunction("runFrom", LlvmType<Action<ulong, ulong, ulong>>());
        function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        LLVMBuilderRef builder = LLVM.CreateBuilder();

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

        function = module.AddFunction("setup", LlvmType<Action<ulong>>());
        function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        builder = LLVM.CreateBuilder();
        builder.PositionAtEnd(function.AppendBasicBlock("entry"));

        var cbtTy = LlvmType<ulong>().ToPointer().ToPointer();
        var callbackTable = module.AddGlobal(cbtTy, "Callbacks");
        callbackTable.Linkage = LLVMLinkage.LLVMExternalLinkage;
        callbackTable.IsGlobalConstant = false;
        callbackTable.Initializer = LLVMValueRef.CreateConstPointerNull(cbtTy);
        
        builder.BuildStore(function.GetParam(0), callbackTable);

        foreach(var mod in ExeLoader.ExeModules) {
            var size = mod.Binary.Length;
            if(mod.BssEnd > mod.LoadBase + (ulong) size)
                size = (int) (mod.BssEnd - mod.LoadBase);
            while(size % 16384 != 0) size++;
            var data = new byte[size];
            mod.Binary.CopyTo(data);
            var arr = module.AddGlobal(
                LLVMTypeRef.CreateArray(LlvmType<byte>(), (uint) size), 
                $"module_{mod.LoadBase:X}_data"
            );
            arr.Linkage = LLVMLinkage.LLVMInternalLinkage;
            arr.Initializer = LLVMValueRef.CreateConstArray(LlvmType<byte>(), data.Select(x =>
                LLVMValueRef.CreateConstInt(LlvmType<byte>(), x)).ToArray());
            CallbackCaller.CallLoadModule(builder, callbackTable, 
                LLVMValueRef.CreateConstInt(LlvmType<ulong>(), mod.LoadBase),
                arr,
                LLVMValueRef.CreateConstInt(LlvmType<ulong>(), (ulong) size)
            );
        }
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

static unsafe class CallbackCaller {
    public static LLVMValueRef GetFunc(LLVMBuilderRef builder, LLVMTypeRef type, LLVMValueRef callbackTable, int index) {
        var fptr = builder.BuildGEP2(
            type.ToPointer().ToPointer(), 
            builder.BuildLoad2(LlvmType<ulong>().ToPointer(), callbackTable), 
            [
                LLVMValueRef.CreateConstInt(LlvmType<ulong>(), (ulong) index),
            ]
        );
        return builder.BuildLoad2(type.ToPointer(), fptr);
    }
    
    public static LLVMValueRef CallOne(LLVMBuilderRef builder, LLVMTypeRef type, LLVMValueRef callbackTable, int index, params LLVMValueRef[] args) => 
        builder.BuildCall2(type, GetFunc(builder, type, callbackTable, index), args);

    public static void CallLoadModule(
        LLVMBuilderRef builder, LLVMValueRef callbackTable, 
        LLVMValueRef loadBase, LLVMValueRef data, LLVMValueRef size
    ) {
        CallOne(
            builder,
            LLVMTypeRef.CreateFunction(LLVMTypeRef.Void, [
                LlvmType<ulong>(), LlvmType<byte>().ToPointer(), LlvmType<ulong>()]),
            callbackTable, 1, 
            loadBase, data, size
        );
    }

    public static LLVMValueRef ReadSr(
        LLVMBuilderRef builder, LLVMValueRef callbackTable,
        LLVMValueRef op0, LLVMValueRef op1, LLVMValueRef crn, LLVMValueRef crm, LLVMValueRef op2
    ) =>
        CallOne(
            builder, LlvmType<Func<uint, uint, uint, uint, uint, ulong>>(),
            callbackTable, 2,
            op0, op1, crn, crm, op2
        );

    public static void WriteSr(
        LLVMBuilderRef builder, LLVMValueRef callbackTable,
        LLVMValueRef op0, LLVMValueRef op1, LLVMValueRef crn, LLVMValueRef crm, LLVMValueRef op2,
        LLVMValueRef value
    ) =>
        CallOne(
            builder, LlvmType<Action<uint, uint, uint, uint, uint, ulong>>(),
            callbackTable, 3,
            op0, op1, crn, crm, op2, value
        );
}

unsafe class LlvmOutput {
    LLVMBuilderRef Builder;
    LLVMValueRef Function, RunFrom, CpuStateRef, CallbackTable;
    LLVMValueRef BitReverse8, BitReverse16, BitReverse32, BitReverse64;
    LLVMValueRef Clz8, Clz16, Clz32, Clz64;
    LLVMValueRef Abs8, Abs16, Abs32, Abs64, Fabs32, Fabs64;
    LLVMValueRef Trunc32, Trunc64, Round32, Round64, Ceil32, Ceil64, Floor32, Floor64;
    LLVMValueRef Sqrt32, Sqrt64;
    LLVMValueRef PopCntV16;
    LLVMBasicBlockRef CurrentBlock;
    readonly Dictionary<string, LLVMValueRef> Locals = new();
    LLVMValueRef[] Gprs;
    LLVMValueRef[] Vecs;
    LLVMValueRef SP, N, Z, C, V, TlsBase;
    
    internal void BuildModule(string path, string targetTriple, LLVMTargetMachineRef targetMachine, IEnumerable<(string Name, BlockGraph Node)> nodes) {
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
        Sqrt32 = module.AddFunction("llvm.sqrt.f32", LlvmType<Func<float, float>>());
        Sqrt64 = module.AddFunction("llvm.sqrt.f64", LlvmType<Func<double, double>>());
        PopCntV16 = module.AddFunction("llvm.ctpop.v16i8", LlvmType<Func<Vector128<byte>, Vector128<byte>>>());
        CallbackTable = module.AddGlobal(LlvmType<ulong>().ToPointer().ToPointer(), "Callbacks");
        CallbackTable.Linkage = LLVMLinkage.LLVMExternalLinkage;

        var funcs = nodes.Select(node => (node.Node.Block.Start, BuildFunction(module, node.Name, node.Node))).ToList();
        
        targetMachine.EmitToFile(module, path + ".o", LLVMCodeGenFileType.LLVMObjectFile);
        module.Dispose();
    }

    static LLVMValueRef Const(byte v) => LLVMValueRef.CreateConstInt(LlvmType<byte>(), v);
    static LLVMValueRef Const(ushort v) => LLVMValueRef.CreateConstInt(LlvmType<ushort>(), v);
    static LLVMValueRef Const(uint v) => LLVMValueRef.CreateConstInt(LlvmType<uint>(), v);
    static LLVMValueRef Const(ulong v) => LLVMValueRef.CreateConstInt(LlvmType<ulong>(), v);
    static LLVMValueRef Const(sbyte v) => LLVMValueRef.CreateConstInt(LlvmType<sbyte>(), unchecked((byte) v));
    static LLVMValueRef Const(short v) => LLVMValueRef.CreateConstInt(LlvmType<short>(), unchecked((ushort) v));
    static LLVMValueRef Const(int v) => LLVMValueRef.CreateConstInt(LlvmType<int>(), unchecked((uint) v));
    static LLVMValueRef Const(long v) => LLVMValueRef.CreateConstInt(LlvmType<long>(), unchecked((ulong) v));
    static LLVMValueRef Const(float v) => LLVMValueRef.CreateConstReal(LlvmType<float>(), v);
    static LLVMValueRef Const(double v) => LLVMValueRef.CreateConstReal(LlvmType<double>(), v);
    static LLVMValueRef Const(bool v) => LLVMValueRef.CreateConstInt(LlvmType<bool>(), v ? 1UL : 0);
    static LLVMValueRef ConstBit(bool v) => LLVMValueRef.CreateConstInt(LlvmType<Bit>(), v ? 1UL : 0);

    LLVMValueRef BuildFunction(LLVMModuleRef module, string name, BlockGraph node) {
        Locals.Clear();
        Builder = LLVM.CreateBuilder();
        var passManager = (LLVMPassManagerRef) LLVM.CreateFunctionPassManagerForModule(module);
        
        LLVM.AddReassociatePass(passManager);
        LLVM.AddCFGSimplificationPass(passManager);
        LLVM.AddGVNPass(passManager);
        LLVM.AddPromoteMemoryToRegisterPass(passManager);
        LLVM.AddSLPVectorizePass(passManager);
        LLVM.AddDeadStoreEliminationPass(passManager);
        LLVM.AddAggressiveDCEPass(passManager);
        LLVM.AddPartiallyInlineLibCallsPass(passManager);
        LLVM.InitializeFunctionPassManager(passManager);
        
        Function = module.AddFunction(name, LlvmType<Func<ulong, ulong>>());
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
        
        passManager.Dispose();
        Builder.Dispose();

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
                var addr = Builder.BuildAdd(CpuStateRef, Const(offset));
                var ptr = Builder.BuildIntToPtr(addr, LLVM.PointerType(value.Type.ToLLVMType(), 0), $"{name}[{index}]");
                Builder.BuildStore(Compile(value), ptr);
                return false;
            }
            case StaticIRStatement.SetField(StaticIRValue.Named("State", _), var name, var value): {
                var offset = (ulong) Marshal.OffsetOf<CpuState>(name);
                var addr = Builder.BuildAdd(CpuStateRef, Const(offset));
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
            case WriteSrStmt(var op0, var op1, var crn, var crm, var op2, var value):
                CallbackCaller.WriteSr(
                    Builder, CallbackTable,
                    Const(op0),
                    Const(op1),
                    Const(crn),
                    Const(crm),
                    Const(op2),
                    Compile(value)
                );
                return false;
            case SvcStmt(var name, var inRegs, var outRegs): {
                var cbIndex = ((int) Marshal.OffsetOf<CallbackTableOffsets>($"svc{name}")) / 8;
                var funcTy = LLVMTypeRef.CreateFunction(
                    outRegs.Length == 0 ? LLVMTypeRef.Void : LlvmType<ulong>(),
                    inRegs.Select(_ => LlvmType<ulong>())
                        .Concat(outRegs.Skip(1).Select(_ => LlvmType<ulong>())).ToArray()
                );
                var args = inRegs.Select(Compile).ToList();
                foreach(var reg in outRegs.Skip(1))
                    if(reg is StaticIRValue.NamedOut(var rname, _))
                        args.Add(Locals[rname]);
                    else if(reg is StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), "X", var index, _))
                        args.Add(Builder.BuildAdd(
                            CpuStateRef,
                            Const((ulong) (Marshal.OffsetOf<CpuState>("X") + index * 8))
                        ));
                    else
                        throw new NotSupportedException($"Unsupported outReg: {reg}");
                var call = CallbackCaller.CallOne(Builder, funcTy, CallbackTable, cbIndex, args.ToArray());
                if(outRegs.Length == 0) return false;
                if(outRegs[0] is StaticIRValue.NamedOut(var tname, _))
                    Builder.BuildStore(call, Locals[tname]);
                else if(outRegs[0] is StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), "X", var index, _)) {
                    var rptr = Builder.BuildAdd(
                        CpuStateRef,
                        Const((ulong) (Marshal.OffsetOf<CpuState>("X") + index * 8))
                    );
                    rptr = Builder.BuildIntToPtr(rptr, LlvmType<ulong>().ToPointer());
                    Builder.BuildStore(call, rptr);
                } else
                    throw new NotSupportedException($"Unsupported outReg: {outRegs[0]}");
                return false;
            }
            case StaticIRStatement.Sink(var value):
                Compile(value);
                return false;
            default:
                throw new NotSupportedException($"Unknown statement: {stmt}");
        }
    }

    LLVMValueRef CompareFloat(StaticIRValue comp, StaticIRValue left, StaticIRValue right) {
        var op = comp switch {
            StaticIRValue.EQ => LLVMRealPredicate.LLVMRealOEQ,
            StaticIRValue.NE => LLVMRealPredicate.LLVMRealUNE,
            StaticIRValue.GT => LLVMRealPredicate.LLVMRealOGT,
            StaticIRValue.GTE => LLVMRealPredicate.LLVMRealOGE,
            StaticIRValue.LT => LLVMRealPredicate.LLVMRealOLT,
            StaticIRValue.LTE => LLVMRealPredicate.LLVMRealOLE,
            _ => throw new NotImplementedException($"Unknown operator: {comp}"),
        };
        var val = Builder.BuildFCmp(op, Compile(left), Compile(right));
        return Builder.BuildCast(LLVMOpcode.LLVMZExt, val, LlvmType<ulong>());
    }

    LLVMValueRef Compare(StaticIRValue comp, StaticIRValue left, StaticIRValue right) {
        if(left.Type.IsFloat) return CompareFloat(comp, left, right);
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
                        byte v => Const(v),
                        sbyte v => Const(v),
                        ushort v => Const(v),
                        short v => Const(v),
                        uint v => Const(v),
                        int v => Const(v),
                        ulong v => Const(v),
                        long v => Const(v),
                        UInt128 v => LLVMValueRef.CreateConstIntOfArbitraryPrecision(LlvmType<UInt128>(), [
                            (ulong) v,
                            (ulong) (v >> 64),
                        ]),
                        Int128 v => LLVMValueRef.CreateConstIntOfArbitraryPrecision(LlvmType<UInt128>(), [
                            (ulong) (UInt128) v,
                            (ulong) ((UInt128) v >> 64),
                        ]),
                        float v => Const(v),
                        double v => Const(v),
                        bool v => Const(v),
                        _ => throw new NotSupportedException($"Unsupported literal type: {value}"),
                    };
                case StaticIRValue.Named(var name, var type):
                    return Builder.BuildLoad2(type.ToLLVMType(), Locals[name]);
                case StaticIRValue.GetField(StaticIRValue.Named("State", _), var name, var type): {
                    var offset = (ulong) Marshal.OffsetOf<CpuState>(name);
                    var addr = Builder.BuildAdd(CpuStateRef, Const(offset));
                    var ptr = Builder.BuildIntToPtr(addr, LLVM.PointerType(type.ToLLVMType(), 0));
                    return Builder.BuildLoad2(type.ToLLVMType(), ptr, name);
                }
                case StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), var name, var index, var type): {
                    var offset = (ulong) Marshal.OffsetOf<CpuState>(name) + (ulong) (type.ByteCount * index);
                    var addr = Builder.BuildAdd(CpuStateRef, Const(offset));
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
                    if(left.Type.IsVector && !right.Type.IsVector)
                        return Compile(new StaticIRValue.Mul(left, new StaticIRValue.CreateVector(right)));
                    if(!left.Type.IsVector && right.Type.IsVector)
                        return Compile(new StaticIRValue.Mul(new StaticIRValue.CreateVector(left), right));
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
                        1 => Builder.BuildCall2(LlvmType<Func<byte, Bit, byte>>(), Clz8, [Compile(left), ConstBit(false)]),
                        2 => Builder.BuildCall2(LlvmType<Func<ushort, Bit, ushort>>(), Clz16, [Compile(left), ConstBit(false)]),
                        4 => Builder.BuildCall2(LlvmType<Func<uint, Bit, uint>>(), Clz32, [Compile(left), ConstBit(false)]),
                        8 => Builder.BuildCall2(LlvmType<Func<ulong, Bit, ulong>>(), Clz64, [Compile(left), ConstBit(false)]),
                        _ => throw new NotImplementedException($"Can't clz {left.Type}"),
                    };
                case StaticIRValue.Abs(var left):
                    return !left.Type.IsFloat
                        ? left.Type.ByteCount switch {
                            1 => Builder.BuildCall2(LlvmType<Func<byte, Bit, byte>>(), Abs8, [Compile(left), ConstBit(false)]),
                            2 => Builder.BuildCall2(LlvmType<Func<ushort, Bit, ushort>>(), Abs16, [Compile(left), ConstBit(false)]),
                            4 => Builder.BuildCall2(LlvmType<Func<uint, Bit, uint>>(), Abs32, [Compile(left), ConstBit(false)]),
                            8 => Builder.BuildCall2(LlvmType<Func<ulong, Bit, ulong>>(), Abs64, [Compile(left), ConstBit(false)]),
                            _ => throw new NotImplementedException($"Can't abs {left.Type}"),
                        }
                        : left.Type.ByteCount switch {
                            4 => Builder.BuildCall2(LlvmType<Func<float, float>>(), Fabs32, [Compile(left)]),
                            8 => Builder.BuildCall2(LlvmType<Func<double, double>>(), Fabs64, [Compile(left)]),
                            _ => throw new NotImplementedException($"Can't abs {left.Type}"),
                        };
                case StaticIRValue.Sqrt(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Sqrt32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Sqrt64, [Compile(left)]);
                case StaticIRValue.Floor(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Floor32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Floor64, [Compile(left)]);
                case StaticIRValue.Ceil(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Ceil32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Ceil64, [Compile(left)]);
                case StaticIRValue.Round(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Round32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Round64, [Compile(left)]);
                case StaticIRValue.RoundTowardZero(var left):
                    return left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Trunc32, [Compile(left)])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Trunc64, [Compile(left)]);
                case StaticIRValue.RoundHalfUp(var left):
                    return Compile(
                        new StaticIRValue.Floor(
                            new StaticIRValue.Add(
                                left,
                                left.Type == typeof(float)
                                    ? new StaticIRValue.Literal(0.5f, typeof(float))
                                    : new StaticIRValue.Literal(0.5, typeof(double))
                            )
                        )
                    );
                case StaticIRValue.RoundHalfDown(var left):
                    return Compile(
                        new StaticIRValue.Ceil(
                            new StaticIRValue.Sub(
                                left,
                                left.Type == typeof(float)
                                    ? new StaticIRValue.Literal(0.5f, typeof(float))
                                    : new StaticIRValue.Literal(0.5, typeof(double))
                            )
                        )
                    );
                case FloatToFixed(var left, var fbits, var dtype): {
                    var pval = Builder.BuildFMul(
                        Compile(left),
                        left.Type == typeof(float)
                            ? Const((float) (1 << fbits))
                            : Const((double) (1 << fbits))
                    );
                    var rval = left.Type == typeof(float)
                        ? Builder.BuildCall2(LlvmType<Func<float, float>>(), Round32, [pval])
                        : Builder.BuildCall2(LlvmType<Func<double, double>>(), Round64, [pval]);
                    return Builder.BuildFPToSI(rval, dtype.ToLLVMType());
                }
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
                            lvec = Builder.BuildInsertElement(lvec, elem, Const(i));
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
                            lvec = Builder.BuildInsertElement(lvec, lvalues[i], Const(i));
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
                case StaticIRValue.IsNaN(var left): {
                    var val = Compile(left);
                    return Builder.BuildCast(LLVMOpcode.LLVMZExt, 
                        Builder.BuildFCmp(LLVMRealPredicate.LLVMRealUNO, val, val), 
                        LlvmType<ulong>());
                }
                case StaticIRValue.VectorFrsqrte(var vec, var bits, var elems): {
                    LLVMValueRef FastInvsqrtFloat(LLVMValueRef elem) {
                        var i = Builder.BuildBitCast(elem, LlvmType<uint>());
                        i = Builder.BuildSub(Const(0x5f3759dfU), Builder.BuildLShr(i, Const(1U)));
                        var f = Builder.BuildBitCast(i, LlvmType<float>());
                        return Builder.BuildFMul(f,
                            Builder.BuildFSub(
                                Const(1.5f),
                                Builder.BuildFMul(f, Builder.BuildFMul(f, Const(0.5f)))));
                    }
                    LLVMValueRef FastInvsqrtDouble(LLVMValueRef elem) {
                        var i = Builder.BuildBitCast(elem, LlvmType<ulong>());
                        i = Builder.BuildSub(Const(0x5fe6eb50c7b537a9ul), Builder.BuildLShr(i, Const(1UL)));
                        var f = Builder.BuildBitCast(i, LlvmType<double>());
                        return Builder.BuildFMul(f,
                            Builder.BuildFSub(
                                Const(1.5f),
                                Builder.BuildFMul(f, Builder.BuildFMul(f, Builder.BuildFMul(elem, Const(0.5))))));
                    }
                    if(bits == 64) {
                        var ivec = Builder.BuildBitCast(Compile(vec), LlvmType<Vector128<double>>());
                        var ovec = LLVM.GetUndef(LlvmType<Vector128<double>>());
                        ovec = Builder.BuildInsertElement(ovec,
                            FastInvsqrtDouble(Builder.BuildExtractElement(ivec, Const(0))),
                            Const(0)
                        );
                        ovec = Builder.BuildInsertElement(ovec,
                            FastInvsqrtDouble(Builder.BuildExtractElement(ivec, Const(1))),
                            Const(1)
                        );
                        return Builder.BuildBitCast(ovec, LlvmType<Vector128<float>>());
                    } else {
                        var ivec = Compile(vec);
                        var ovec = LLVM.GetUndef(LlvmType<Vector128<float>>());
                        for(var i = 0; i < 4; ++i) {
                            if(i >= elems)
                                ovec = Builder.BuildInsertElement(ovec, Const(i), Const(0f));
                            else
                                ovec = Builder.BuildInsertElement(ovec,
                                    FastInvsqrtFloat(Builder.BuildExtractElement(ivec, Const(i))), 
                                    Const(i)
                                );
                        }
                        return ovec;
                    }
                }
                case StaticIRValue.VectorExtract(var a, var b, var q, var index): {
                    var lvec = LLVM.GetUndef( LlvmType<Vector128<byte>>());
                    var ab = Builder.BuildBitCast(Compile(a), LlvmType<Vector128<byte>>());
                    var bb = Builder.BuildBitCast(Compile(b), LlvmType<Vector128<byte>>());
                    if(q == 0) {
                        for(var i = index; i < 8; ++i)
                            lvec = Builder.BuildInsertElement(
                                lvec, 
                                Builder.BuildExtractElement(ab, Const(i)),
                                Const(i));
                        var offset = 8 - index;
                        for(var i = offset; i < 8; ++i)
                            lvec = Builder.BuildInsertElement(
                                lvec, 
                                Builder.BuildExtractElement(bb, Const(i - offset)),
                                Const(i));
                    } else {
                        for(var i = index; i < 16; ++i)
                            lvec = Builder.BuildInsertElement(
                                lvec, 
                                Builder.BuildExtractElement(ab, Const(i)),
                                Const(i));
                        var offset = 16 - index;
                        for(var i = offset; i < 16; ++i)
                            lvec = Builder.BuildInsertElement(
                                lvec, 
                                Builder.BuildExtractElement(bb, Const(i - offset)),
                                Const(i));
                    }
                    return Builder.BuildBitCast(lvec, LlvmType<Vector128<float>>());
                }
                case StaticIRValue.VectorCountBits(var vec, var elems): {
                    // TODO: Implement doing only the lower half when elems == 8
                    var bvec = Builder.BuildBitCast(Compile(vec), LlvmType<Vector128<byte>>());
                    bvec = Builder.BuildCall2(LlvmType<Func<Vector128<byte>, Vector128<byte>>>(), PopCntV16, [bvec]);
                    return Builder.BuildBitCast(bvec, LlvmType<Vector128<float>>());
                }
                case StaticIRValue.VectorSumUnsigned(var vec, var esize, var elems): {
                    switch(esize) {
                        case 8: {
                            var lvec = Builder.BuildBitCast(Compile(vec), LlvmType<Vector128<byte>>());
                            var sum = Const(0UL);
                            for(var i = 0; i < elems; ++i)
                                sum = Builder.BuildAdd(sum, 
                                    Builder.BuildZExt(Builder.BuildExtractElement(lvec, Const(i)), LlvmType<ulong>()));
                            return sum;
                        }
                        default:
                            throw new NotImplementedException($"Unhandled esize for VectorSumUnsigned: {esize}");
                    }
                }
                case CompareAndSwap(var ptr, var val, var comparand): {
                    var xchg = LLVM.BuildAtomicCmpXchg(
                        Builder,
                        Builder.BuildIntToPtr(Compile(ptr), val.Type.ToLLVMType().ToPointer()), 
                        Compile(comparand), Compile(val),
                        LLVMAtomicOrdering.LLVMAtomicOrderingSequentiallyConsistent,
                        LLVMAtomicOrdering.LLVMAtomicOrderingSequentiallyConsistent,
                        0
                    );
                    return Builder.BuildSelect(
                        Builder.BuildExtractValue(xchg, 1, ""), 
                        Const((byte) 0), Const((byte) 1)
                    );
                }
                case ReadSr(var op0, var op1, var crn, var crm, var op2):
                    return CallbackCaller.ReadSr(
                        Builder, CallbackTable,
                        Const(op0),
                        Const(op1),
                        Const(crn),
                        Const(crm),
                        Const(op2)
                    );
                default:
                    throw new NotSupportedException($"Unknown value: {value}");
            }
        }
    }
}