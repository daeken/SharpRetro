using System.Diagnostics;
using System.Runtime.InteropServices;
using LibSharpRetro;
using LLVMSharp.Interop;
using System.Runtime.Intrinsics;
using Aarch64Cpu;
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
    }
}

unsafe class LlvmOutput {
    LLVMBuilderRef Builder;
    LLVMValueRef Function, RunFrom, CpuStateRef;
    LLVMBasicBlockRef CurrentBlock;
    readonly Dictionary<string, LLVMValueRef> Locals = new();
    LLVMValueRef[] Gprs;
    LLVMValueRef[] Vecs;
    LLVMValueRef SP, N, Z, C, V, TlsBase;
    
    internal void BuildModule(string path, string targetTriple, LLVMTargetMachineRef targetMachine, IEnumerable<BlockGraph> nodes) {
        var module = LLVMModuleRef.CreateWithName(Path.GetFileName(path));
        module.Target = targetTriple;
        // TODO: How tf do you set the data layout? It should come from targetMachine somehow

        RunFrom = module.AddFunction("runFrom", LlvmType<Action<ulong, ulong>>());
        RunFrom.Linkage = LLVMLinkage.LLVMExternalLinkage;

        foreach(var node in nodes)
            BuildFunction(module, node);
    }

    void BuildFunction(LLVMModuleRef module, BlockGraph node) {
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
        LLVM.SetLinkage(Function, LLVMLinkage.LLVMLinkerPrivateLinkage);
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
        
        Compile(new StaticIRStatement.Body(node.Block.Body));

        LLVM.DumpValue(Function);
        LLVM.VerifyFunction(Function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
        if(!Function.VerifyFunction(LLVMVerifierFailureAction.LLVMReturnStatusAction))
            throw new("Program verification failed");
        LLVM.RunFunctionPassManager(passManager, Function);
        LLVM.DumpValue(Function);
    }

    bool Compile(StaticIRStatement stmt) {
        switch(stmt) {
            case StaticIRStatement.Body(var stmts):
                stmts.Take(stmts.Count - 1).ForEach(x => Compile(x));
                return stmts.Count > 0 && Compile(stmts.Last());
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
                Builder.BuildCall2(LlvmType<Action<ulong, ulong>>(), RunFrom, [CpuStateRef, Compile(target)]);
                return false;
            case WriteSrStmt:
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
                case StaticIRValue.And(var left, var right):
                    return Builder.BuildAnd(Compile(left), Compile(right));
                case StaticIRValue.Or(var left, var right):
                    return Builder.BuildOr(Compile(left), Compile(right));
                case StaticIRValue.Xor(var left, var right):
                    return Builder.BuildXor(Compile(left), Compile(right));
                case StaticIRValue.LeftShift(var left, var right):
                    return Builder.BuildShl(Compile(left), Compile(right));
                case StaticIRValue.RightShift(var left, var right):
                    return left.Type.IsSigned
                        ? Builder.BuildAShr(Compile(left), Compile(right))
                        : Builder.BuildLShr(Compile(left), Compile(right));
                case StaticIRValue.EQ(var left, var right):
                    return Compare(value, left, right);
                case StaticIRValue.NE(var left, var right):
                    return Compare(value, left, right);
                case StaticIRValue.Not(var left):
                    return Builder.BuildNot(Compile(left));
                case StaticIRValue.Cast(var left, var type): {
                    var fw = left.Type.ByteCount;
                    var sw = type.ByteCount;
                    var fs = left.Type.IsSigned;
                    var ss = type.IsSigned;

                    var opcode = LLVMOpcode.LLVMTrunc;
                    if(fw == sw)
                        opcode = LLVMOpcode.LLVMBitCast;
                    else if(fw < sw)
                        opcode = (fs && !ss) || (fs && ss) ? LLVMOpcode.LLVMSExt : LLVMOpcode.LLVMZExt;

                    return Builder.BuildCast(opcode, Compile(left), type.ToLLVMType());
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
                default:
                    throw new NotSupportedException($"Unknown value: {value}");
            }
        }
    }
}