using System.Text;
using LiftIl;

namespace SharpRetro.Decompiler;

public enum ControlFlowStyle { Auto, Structured, Dispatch }

/// First, conservative source form: architectural state and explicit control flow.
/// Later ABI/SSA/structuring passes can replace this form without replacing the lifter.
public sealed class CppEmitter {
    readonly StringBuilder output = new();
    HashSet<int> defined = [];
    Action<IlBranch>? branchWriter;
    bool allowsCalls;
    bool externalDispatch;
    bool guest32;
    Dictionary<(ulong,int),CppVariable> variables = [];
    IReadOnlyDictionary<string,CppVariableAnnotation>? annotations;
    ulong instructionAddress;
    ushort instructionFpuOpcode;
    int instructionPlainMovsWidth;
    // Stable native operation tag; new opcode-checked forms need no common
    // runtime enum/header edit (and no full translation-bundle rebuild).
    const int NativeEncodedX87=0x7fff;
    bool instructionAlignedVectorStore;
    bool instructionMovapsLoad;
    bool instructionAndMemoryZero;
    bool instructionOrImmediate;
    IlLet? instructionNativeLogicalResult;
    int instructionNativeAluKind;
    Pe32ObjectPlan? objects;
    public string ControlFlowSummary { get; private set; } = "";
    int indent;
    void Line(string text) => output.Append(' ', indent * 4).AppendLine(text);
    static int Width(Il n) => n.Ty is IlType.I { Bits: > 0 and <= 128 } i ? i.Bits
        : throw new NotSupportedException($"C++ integer backend does not support {n.Ty}");
    static NotSupportedException Unsupported(Il n) => new($"Unsupported C++ IL: {n}");
    static string Narrow(string expression, int width) => $"narrow(({expression}), {width})";

    public string Emit(LiftedProgram program, ControlFlowStyle style = ControlFlowStyle.Auto, CppEmissionOptions? options = null) {
        output.Clear(); indent = 0;
        variables=[]; annotations=options?.Annotations;
        objects=options?.Objects;
        externalDispatch=options?.ExternalDispatch==true;
        guest32=program.Image.Architecture==Architecture.X86;
        if(options!=null) {
            if(!System.Text.RegularExpressions.Regex.IsMatch(options.Namespace,@"\A[a-z][a-z0-9_]*(::[a-z][a-z0-9_]*)*\z") ||
               !System.Text.RegularExpressions.Regex.IsMatch(options.RuntimeHeader,@"\A[a-z][a-z0-9_.]*\z"))
                throw new ArgumentException("Invalid source namespace/runtime header");
            var origins=CppVariableOrigins.Collect(program);
            variables=origins.ToDictionary(v=>(v.InstructionRva+program.Image.ImageBase,v.TemporaryId));
            if(annotations!=null && annotations.Keys.Any(k=>!origins.Any(v=>v.Origin==k)))
                throw new NotSupportedException("Unmatched variable annotations; review changed origins before emitting");
            var names=new HashSet<string>();
            foreach(var variable in origins) {
                var annotation=annotations?.GetValueOrDefault(variable.Origin);
                string name=annotation?.Name ?? variable.Name;
                // Reserve every identifier already used by the backend/runtime,
                // plus C++ keywords. Reject collisions instead of rewriting text.
                string reserved=File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"IntegerRuntime.hpp"))+" "+
                    File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"CppRuntime.hpp"))+" "+
                    File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"StringRuntime.hpp"))+" "+
                    "alignas alignof and and_eq asm atomic_cancel atomic_commit atomic_noexcept auto bitand bitor bool break case catch char char8_t char16_t char32_t class compl concept const consteval constexpr constinit const_cast continue co_await co_return co_yield decltype default delete do double dynamic_cast else enum explicit export extern false float for friend goto if inline int long mutable namespace new noexcept not not_eq nullptr operator or or_eq private protected public reflexpr register reinterpret_cast requires return short signed sizeof static static_assert static_cast struct switch synchronized template this thread_local throw true try typedef typeid typename union unsigned using virtual void volatile wchar_t while xor xor_eq s memory instruction_budget returning dispatch address value run run_dispatch entry bits native_simd native_legacy double_shift32 push_sp push_value";
                if(!System.Text.RegularExpressions.Regex.IsMatch(name,@"\A[a-z][a-zA-Z0-9_]*\z") || name.Contains("__") ||
                   System.Text.RegularExpressions.Regex.IsMatch(reserved,@"\b"+System.Text.RegularExpressions.Regex.Escape(name)+@"\b") ||
                   name.StartsWith("condition_") || name.StartsWith("L_") ||
                   objects?.Objects.Any(o=>name=="view_"+o.Name)==true || !names.Add(name))
                    throw new ArgumentException("Invalid, reserved or duplicate variable name: "+name);
                _=CppVariableOrigins.Type(annotation?.Type,variable.Width);
            }
        }
        branchWriter = null;
        allowsCalls = program.AllowsCalls;
        StructuredProgram? structured = null;
        string flow = "dispatch requested";
        if(style != ControlFlowStyle.Dispatch) {
            try {
                if(allowsCalls) throw new StructuringException("Interprocedural call/return execution currently uses dispatch");
                structured = Structuring.Recover(program);
                flow = $"structured; {structured.Graph.Blocks.Count} blocks, {structured.Loops} loops, {structured.Conditionals} conditionals";
            } catch(StructuringException e) {
                if(style == ControlFlowStyle.Structured) throw new NotSupportedException("Cannot structure control flow: " + e.Message, e);
                flow = "dispatch fallback: " + e.Message;
            }
        }
        ControlFlowSummary = flow;
        if(options==null) output.AppendLine(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "CppRuntime.hpp"))
            .Replace("#include \"IntegerRuntime.hpp\"", File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "IntegerRuntime.hpp"))));
        else Line($"#include \"{options.RuntimeHeader}\"");
        if(program.Instructions.Any(i=>i.Body.Body.Any(n=>n is IlIntrin {Name:"pe32-string-compare"} || n is IlIntrin {Name:"rep_movs",Args:[IlConst width]} && (width.Bits==8 || width.Bits==16) || n is IlIntrin {Name:"movs",Args:[IlConst plainWidth]} && plainWidth.Bits==8)))
            output.AppendLine(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"StringRuntime.hpp")));
        if(program.Instructions.Any(i=>i.Body.Body.Any(n=>n is IlIntrin {Name:"pe32-simd" or "pe32-mmx" or "pe32-segment-store" or "pe32-rdtsc"}))) {
            Line("namespace sharp_retro {");
            Line("#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)");
            Line("void native_legacy(Memory&,State&,std::uint64_t,unsigned,std::uint32_t,std::uint32_t); ");
            Line("void native_simd(Memory&,State&,std::uint64_t,std::uint32_t,std::uint32_t);");
            Line("#else");
            Line("inline void native_legacy(Memory&,State&,std::uint64_t,unsigned,std::uint32_t,std::uint32_t) { throw std::runtime_error(\"Legacy operation requires native Windows x86 execution\"); }");
            Line("inline void native_simd(Memory&,State&,std::uint64_t,std::uint32_t,std::uint32_t) { throw std::runtime_error(\"SIMD operation requires native Windows x86 execution\"); }");
            Line("#endif");Line("}");
        }
        if(program.Instructions.Any(i=>i.Body.Body.Any(n=>n is IlIntrin {Name:"pe32-double-shift"})))
            output.AppendLine(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"DoubleShiftRuntime.hpp")));
        Line($"namespace {options?.Namespace ?? "sharp_retro"} {{");
        if(options!=null) Line("using namespace sharp_retro;");
        if(objects!=null) foreach(string declaration in objects.Declarations()) Line(declaration);
        Line($"// Input SHA-256: {program.Image.Sha256}");
        Line($"// {program.Image.Format} / {program.Image.Architecture}; reachable instructions: {program.Instructions.Count}");
        Line($"// Control flow: {flow}");
        Line($"inline constexpr std::uint64_t entry = 0x{program.Entry:x}ULL;");
        string budgetParameter=externalDispatch ? "std::uint64_t& instruction_budget" : "std::uint64_t instruction_budget";
        Line($"inline Exit run_dispatch(State& s, Memory& memory, {budgetParameter}) {{"); indent++;
        if(allowsCalls && !externalDispatch) Line("bool returning = false;");
        Line("goto dispatch;");
        foreach(var insn in program.Instructions) {
            Instruction(insn, $"L_{insn.Address:x}: ");
        }
        Line("dispatch:");
        Line("switch(s.pc) {"); indent++;
        foreach(var insn in program.Instructions) Line($"case 0x{insn.Address:x}ULL: goto L_{insn.Address:x};");
        foreach(var (address, symbol) in program.Imports.OrderBy(i => i.Key)) {
            Line($"case 0x{address:x}ULL:"); indent++;
            Line($"if(!memory.invoke_import({CppString(symbol)}, s)) throw std::runtime_error(\"unbound guest import\");");
            if(allowsCalls && !externalDispatch) Line("returning = true;");
            Line("goto dispatch;"); indent--;
        }
        Line(allowsCalls && !externalDispatch ? "default: return returning ? Exit::returned : Exit::boundary;" : "default: return Exit::boundary;");
        indent--; Line("}");
        indent--; Line("}");
        Line($"inline Exit run(State& s, Memory& memory, {budgetParameter}) {{"); indent++;
        if(structured == null) Line("return run_dispatch(s, memory, instruction_budget);");
        else {
            // Keep the existing API's arbitrary-PC entry and budget-resume behavior.
            // A resumed middle instruction uses the baseline, without replaying a
            // structured region's earlier instructions or effects.
            Line("if(s.pc != entry) return run_dispatch(s, memory, instruction_budget);");
            EmitRegion(structured.Body);
            Line("throw std::logic_error(\"structured control flow fell through\");");
        }
        indent--; Line("}"); Line("} // namespace sharp_retro");
        return output.ToString();
    }

    void Instruction(LiftedInstruction insn, string label = "") {
        try {
            instructionAddress=insn.Address;
            instructionNativeLogicalResult=null;
            instructionFpuOpcode=insn.Bytes.Length>=2 && insn.Bytes[0] is >=0xd8 and <=0xdf
                ? (ushort)(((insn.Bytes[0]&7)<<8)|insn.Bytes[1]) : (ushort)0;
            instructionAlignedVectorStore=(insn.Bytes is [0x66,0x0f,0x7f,var modrm,..] && (modrm&0xc0)!=0xc0) ||
                (insn.Bytes is [0x0f,0x29,var apsStore,..] && (apsStore&0xc0)!=0xc0);
            instructionMovapsLoad=insn.Bytes is [0x0f,0x28,var apsLoad,..] && (apsLoad&0xc0)!=0xc0;
            instructionOrImmediate=externalDispatch && guest32 &&
                ((insn.Bytes is [0x83,var orShort,_] && (orShort&0xf8)==0xc8) ||
                 (insn.Bytes is [0x81,var orLong,_,_,_,_] && (orLong&0xf8)==0xc8) ||
                 insn.Bytes is [0x0d,_,_,_,_]);
            instructionAndMemoryZero=externalDispatch && guest32 && insn.Bytes.Length>=3 && insn.Bytes[0]==0x83 &&
                (insn.Bytes[1]&0x38)==0x20 && (insn.Bytes[1]&0xc0)!=0xc0 && insn.Bytes[^1]==0;
            defined = [];
            Line($"{label}{{ // 0x{insn.Address:x}: {Convert.ToHexStringLower(insn.Bytes)}  {insn.Text}"); indent++;
            if(allowsCalls && !externalDispatch) Line("returning = false;");
            Line("if(instruction_budget == 0) return Exit::budget_exhausted;");
            Line("--instruction_budget;");
            // MSVC allocates scratch locals with PUSH of a volatile register.
            // Spill unknownness, rather than inventing a preserved native value.
            bool unknownPush=externalDispatch && guest32 && insn.Bytes.Length==1 && insn.Bytes[0] is >=0x50 and <=0x57;
            bool registerPop=externalDispatch && guest32 && insn.Bytes.Length==1 && insn.Bytes[0] is >=0x58 and <=0x5f;
            bool memoryPush=externalDispatch && guest32 && insn.Bytes.Length>=2 && insn.Bytes[0]==0xff &&
                (insn.Bytes[1]&0x38)==0x30 && (insn.Bytes[1]&0xc0)!=0xc0;
            bool zeroIdiom=externalDispatch && guest32 && insn.Bytes.Length==2 && insn.Bytes[0] is 0x31 or 0x33 &&
                (insn.Bytes[1]&0xc0)==0xc0 && ((insn.Bytes[1]>>3)&7)==(insn.Bytes[1]&7);
            instructionPlainMovsWidth=insn.Bytes switch { [0xa4]=>8,[0xa5]=>32,[0x66,0xa5]=>16,_=>0 };
            bool flagPush=externalDispatch && guest32 && insn.Bytes is [0x9c];
            bool flagPop=externalDispatch && guest32 && insn.Bytes is [0x9d];
            bool wordFlagPush=externalDispatch && guest32 && insn.Bytes is [0x66,0x9c];
            bool wordFlagPop=externalDispatch && guest32 && insn.Bytes is [0x66,0x9d];
            bool frameLeave=externalDispatch && guest32 && insn.Bytes is [0xc9];
            bool registerMove=externalDispatch && guest32 && insn.Bytes.Length==2 && insn.Bytes[0] is 0x89 or 0x8b && (insn.Bytes[1]&0xc0)==0xc0;
            // Base-only LEA with zero displacement is a bit copy, including
            // alignment NOPs on volatile registers whose values are unknown.
            bool addressCopy=externalDispatch && guest32 && insn.Bytes.Length is 2 or 3 && insn.Bytes[0]==0x8d &&
                (insn.Bytes[1]&7)!=4 && ((insn.Bytes.Length==2 && (insn.Bytes[1]&0xc0)==0 && (insn.Bytes[1]&7)!=5) ||
                (insn.Bytes.Length==3 && (insn.Bytes[1]&0xc0)==0x40 && insn.Bytes[2]==0));
            bool immediateXor=externalDispatch && guest32 &&
                (insn.Bytes is [0x35,_,_,_,_] || insn.Bytes.Length==6 && insn.Bytes[0]==0x81 && (insn.Bytes[1]&0xf8)==0xf0);
            bool selfSbb=externalDispatch && guest32 && insn.Bytes.Length==2 && insn.Bytes[0] is 0x19 or 0x1b &&
                (insn.Bytes[1]&0xc0)==0xc0 && ((insn.Bytes[1]>>3)&7)==(insn.Bytes[1]&7);
            bool registerSubtract=externalDispatch && guest32 && insn.Bytes.Length==2 && insn.Bytes[0] is 0x29 or 0x2b && (insn.Bytes[1]&0xc0)==0xc0;
            bool registerXor=externalDispatch && guest32 && insn.Bytes.Length==2 && insn.Bytes[0] is 0x31 or 0x33 && (insn.Bytes[1]&0xc0)==0xc0;
            bool byteMemoryXor=externalDispatch && guest32 && insn.Bytes.Length>=2 && insn.Bytes[0]==0x30 && (insn.Bytes[1]&0xc0)!=0xc0;
            if(unknownPush) {
                int reg=insn.Bytes[0]-0x50;
                Line($"if((s.defined_gpr & (1u<<{reg}))==0) {{"); indent++;
                Line("auto push_sp=std::uint32_t(read_gpr(s,4))-std::uint32_t(4);");
                Line($"auto push_value=scalar_gpr32(s,{reg});");
                Line("memory.store_scalar32(s.segments[2]+push_sp,push_value); s.gpr[4]=push_sp;");
                indent--; Line("} else {"); indent++;
            } else if(zeroIdiom) {
                int reg=insn.Bytes[1]&7;
                Line($"if((s.defined_gpr & (1u<<{reg}))==0) {{"); indent++;
                Line($"s.gpr[{reg}]=0; s.defined_gpr|=std::uint16_t(1u<<{reg});");
                Line($"s.partial_gpr[{reg}]=0;");
                Line("s.flags=(s.flags & ~std::uint64_t(0x8d5)) | 0x44;");
                Line("s.defined_flags=(s.defined_flags | 0x8c5) & ~std::uint64_t(0x10);");
                Line("s.flag_origins[4]=fresh_unknown_flag_origin(s,4);");
                indent--; Line("} else {"); indent++;
            } else if(registerSubtract) {
                int left=(insn.Bytes[1]>>3)&7,right=insn.Bytes[1]&7;
                if(insn.Bytes[0]==0x29) (left,right)=(right,left);
                Line($"if(!subtract_correlated32(s,{left},{right})) {{"); indent++;
            }
            if(memoryPush) {
                if(insn.Body.Body is not [IlLet address,IlLet { Val:IlLoad { Ty:IlType.I { Bits:32 } } load } value,
                    IlWriteReg { Kind:RegKind.X86,Idx:4 },IlStore store] ||
                    load.Addr is not IlTmp addressTemp || addressTemp.Id!=address.Id ||
                    store.Val is not IlTmp valueTemp || valueTemp.Id!=value.Id)
                    throw new NotSupportedException("Unexpected memory PUSH operand shape: "+insn.Body);
                // ESP-relative effective addresses use the incoming ESP. Load
                // before the push helper commits SP after a successful store.
                Line($"auto push_value=memory.load_scalar32(std::uint64_t({Expr(address.Val)}));");
                Line("push_scalar32(s,memory,push_value);");
            }
            else if(flagPush) Line("pushfd32(s,memory);");
            else if(flagPop) Line("popfd32(s,memory);");
            else if(wordFlagPush) Line("pushfw32(s,memory);");
            else if(wordFlagPop) Line("popfw32(s,memory);");
            else if(frameLeave) {
                Line("auto frame_sp=std::uint32_t(read_gpr(s,5)); auto frame_value=memory.load_scalar32(s.segments[2]+frame_sp);");
                Line("s.gpr[4]=std::uint32_t(frame_sp+4u); s.defined_gpr|=std::uint16_t(1u<<4); scalar_gpr32(s,5,frame_value);");
            }
            else if(selfSbb) Line($"sbb_self32(s,{insn.Bytes[1]&7});");
            else if(addressCopy) {
                Line($"scalar_gpr32(s,{(insn.Bytes[1]>>3)&7},scalar_gpr32(s,{insn.Bytes[1]&7}));");
            }
            else if(registerMove) {
                int source=(insn.Bytes[1]>>3)&7,destination=insn.Bytes[1]&7;
                if(insn.Bytes[0]==0x8b) (source,destination)=(destination,source);
                Line($"scalar_gpr32(s,{destination},scalar_gpr32(s,{source}));");
            } else if(byteMemoryXor) {
                if(insn.Body.Body is not [IlLet address,IlLet { Val:IlBin { Op:BinOp.Xor,Ty:IlType.I { Bits:8 },L:IlLoad load } } value,IlStore store,..] ||
                    store.Addr is not IlTmp addressTemp || addressTemp.Id!=address.Id || load.Addr!=store.Addr ||
                    store.Val is not IlTmp valueTemp || valueTemp.Id!=value.Id ||
                    insn.Body.Body.Skip(3).Any(n=>n is not IlLet && n is not IlWriteReg { Kind:RegKind.Eflags }))
                    throw new NotSupportedException("Unexpected byte memory XOR operand shape: "+insn.Body);
                Line($"{{ auto address=std::uint64_t({Expr(address.Val)}); xor_memory_gpr8(s,memory,address,{(insn.Bytes[1]>>3)&7}); }}");
            } else if(registerXor) {
                int destination=(insn.Bytes[1]>>3)&7,source=insn.Bytes[1]&7;
                if(insn.Bytes[0]==0x31) (source,destination)=(destination,source);
                Line($"xor_gprs32(s,{destination},{source});");
            } else if(immediateXor) {
                int reg=insn.Bytes[0]==0x35 ? 0 : insn.Bytes[1]&7;
                uint constant=System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(insn.Bytes.AsSpan(insn.Bytes.Length-4));
                Line($"xor_scalar32(s,{reg},0x{constant:x}u);");
            } else if(registerPop) {
                int reg=insn.Bytes[0]-0x58;
                Line("auto pop_sp=std::uint32_t(read_gpr(s,4));");
                Line("auto pop_value=memory.load_scalar32(s.segments[2]+pop_sp);");
                Line("s.gpr[4]=std::uint32_t(pop_sp+4u); s.defined_gpr|=std::uint16_t(1u<<4);");
                Line($"scalar_gpr32(s,{reg},pop_value);");
            } else {
            foreach(int id in Temps(insn.Body).Distinct().Order()) {
                string type=variables.TryGetValue((instructionAddress,id),out var variable)
                    ? CppVariableOrigins.Type(annotations?.GetValueOrDefault(variable.Origin)?.Type,variable.Width) : "bits";
                Line($"[[maybe_unused]] {type} {Temporary(id)};");
            }
            EmitNativeLogicalFlags(insn,true);
            Statements(insn.Body.Body);
            if(instructionAndMemoryZero || instructionOrImmediate) Line("s.defined_flags&=~std::uint64_t(0x10); s.flag_origins[4]=fresh_unknown_flag_origin(s,4);");
            EmitNativeLogicalFlags(insn,false);
            }
            if(unknownPush || zeroIdiom || registerSubtract) { indent--; Line("}"); }
            if(branchWriter == null || insn.Body.Body.LastOrDefault() is not IlBranch) {
                Line($"s.pc = 0x{insn.Next:x}ULL;");
                if(branchWriter == null) Line("goto dispatch;");
            }
            indent--; Line("}");
        } catch(NotSupportedException e) {
            throw new NotSupportedException($"C++ emission failed at 0x{insn.Address:x} ({insn.Text}): {e.Message}", e);
        }
    }

    void EmitNativeLogicalFlags(LiftedInstruction insn,bool prepare) {
        if(!externalDispatch || !guest32) return;
        int cursor=0,width=32;
        while(cursor<insn.Bytes.Length && insn.Bytes[cursor] is 0x66 or 0x67 or 0x26 or 0x2e or 0x36 or 0x3e or 0x64 or 0x65) {
            if(insn.Bytes[cursor]==0x66) width=16;
            cursor++;
        }
        if(cursor>=insn.Bytes.Length) return;
        byte opcode=insn.Bytes[cursor]; BinOp? operation=null; int native=-1;
        if(opcode is >=0x08 and <=0x0d) {operation=BinOp.Or;native=1;}
        else if(opcode is >=0x20 and <=0x25) {operation=BinOp.And;native=0;}
        else if(opcode is >=0x30 and <=0x35) {operation=BinOp.Xor;native=2;}
        else if(opcode is 0x84 or 0x85 or 0xa8 or 0xa9) {operation=BinOp.And;native=3;}
        else if(opcode is 0x69 or 0x6b || opcode==0x0f && cursor+1<insn.Bytes.Length && insn.Bytes[cursor+1]==0xaf) {
            operation=BinOp.Mul;native=7;
        }
        else if(opcode is 0x80 or 0x81 or 0x83 && cursor+1<insn.Bytes.Length) {
            int group=(insn.Bytes[cursor+1]>>3)&7;
            if(group==1) {operation=BinOp.Or;native=1;}
            else if(group==4) {operation=BinOp.And;native=0;}
            else if(group==6) {operation=BinOp.Xor;native=2;}
        } else if(opcode is 0xf6 or 0xf7 && cursor+1<insn.Bytes.Length && (insn.Bytes[cursor+1]&0x38)==0) {
            operation=BinOp.And;native=3;
        } else if(opcode is 0xf6 or 0xf7 && cursor+1<insn.Bytes.Length && ((insn.Bytes[cursor+1]>>3)&7) is 4 or 5) {
            operation=BinOp.Mul;native=((insn.Bytes[cursor+1]>>3)&7)==4 ? 8:9;
        } else if(opcode is 0xf6 or 0xf7 && cursor+1<insn.Bytes.Length && ((insn.Bytes[cursor+1]>>3)&7) is 6 or 7) {
            bool signed=((insn.Bytes[cursor+1]>>3)&7)==7;
            operation=signed ? BinOp.SDiv:BinOp.UDiv;native=signed ? 11:10;
        } else if(opcode is 0xc0 or 0xc1 or 0xd0 or 0xd1 or 0xd2 or 0xd3 && cursor+1<insn.Bytes.Length) {
            int group=(insn.Bytes[cursor+1]>>3)&7;
            if(group is 4 or 6) {operation=BinOp.Shl;native=4;}
            else if(group==5) {operation=BinOp.Shr;native=5;}
            else if(group==7) {operation=BinOp.Sar;native=6;}
        }
        if(operation==null) return;
        if((opcode&1)==0 && opcode!=0x83 && native!=7) width=8;
        if(native==3 && opcode is 0x84 or 0x85 && cursor+1<insn.Bytes.Length && (insn.Bytes[cursor+1]&0xc0)==0xc0) {
            if(prepare) {
                Line("#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)");
                Line("native_require_alu_flags(s);");
                Line("auto native_logical_flags=s.flags,native_logical_known=s.defined_flags;");
                Line("#endif");
                return;
            }
            int a=insn.Bytes[cursor+1]&7,b=(insn.Bytes[cursor+1]>>3)&7;
            string Operand(int encoded) => width==8
                ? $"(read_gpr_mask(s,{encoded&3},0x{(encoded<4 ? 0xff : 0xff00):x}u)>>{(encoded<4 ? 0 : 8)})"
                : $"read_gpr_mask(s,{encoded},0x{(width==16 ? 0xffffu : 0xffffffffu):x}u)";
            // TEST leaves both source registers untouched, including when the
            // IL optimizer folds x&x into a direct source value.
            Line("#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)");
            Line($"native_logic_flags(s,3,{width},std::uint32_t({Operand(a)}),std::uint32_t({Operand(b)}),native_logical_flags,native_logical_known);");
            Line("#endif");
            return;
        }
        // Match the data operation before the flag equations. Capture operands
        // at its original evaluation point, before any destination changes.
        var result=insn.Body.Body.OfType<IlLet>().FirstOrDefault(let=>let.Val is IlBin b &&
            b.Op==operation && b.Ty is IlType.I {Bits:var bits} && bits==(native is >=7 and <=11 ? width*2 : width));
        if(result?.Val is not IlBin)
            return; // Retain unresolved flags; native consumers must refuse them.
        if(prepare) {instructionNativeLogicalResult=result;instructionNativeAluKind=native;return;}
        Line("#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)");
        Line($"native_logic_flags(s,{native},{width},native_logical_left,native_logical_right,native_logical_flags,native_logical_known);");
        Line("#endif");
    }

    string Temporary(int id) => variables.TryGetValue((instructionAddress,id),out var variable)
        ? annotations?.GetValueOrDefault(variable.Origin)?.Name ?? variable.Name : $"t{id}";

    static string CppString(string value) => string.Join("", Encoding.UTF8.GetBytes(value).Select(b => $"\"\\x{b:x2}\""));

    void EmitBlock(BasicBlock block, string? condition = null) {
        var previous = branchWriter;
        branchWriter = branch => {
            if(branch.Kind == BranchKind.Ret) {
                Line($"s.pc = std::uint64_t({Expr(branch.Target)});");
                Line("return Exit::returned;");
            } else if(branch.Cond == null) Line($"s.pc = std::uint64_t({Expr(branch.Target)});");
            else {
                if(condition == null) throw new InvalidOperationException("Missing structured branch condition");
                Line($"{condition} = ({Expr(branch.Cond)} & 1) != 0;");
                Line($"s.pc = {condition} ? std::uint64_t({Expr(branch.Target)}) : 0x{block.Alternative:x}ULL;");
            }
        };
        foreach(var insn in block.Instructions) Instruction(insn);
        branchWriter = previous;
    }

    void EmitRegion(Region region) {
        switch(region) {
            case SequenceRegion s: foreach(var child in s.Body) EmitRegion(child); break;
            case BlockRegion b: EmitBlock(b.Block); break;
            case IfRegion i: {
                string condition = $"condition_{i.Test.Address:x}";
                Line($"bool {condition};");
                EmitBlock(i.Test, condition);
                Line($"if({condition}) {{"); indent++;
                EmitRegion(i.Then); indent--; Line("} else {"); indent++;
                EmitRegion(i.Else); indent--; Line("}"); break;
            }
            case LoopRegion l:
                Line($"while(true) {{ // loop at 0x{l.Header:x}"); indent++;
                EmitRegion(l.Body); indent--; Line("}"); break;
            case BreakRegion: Line("break;"); break;
            case ContinueRegion: Line("continue;"); break;
            case BoundaryRegion: Line("return Exit::boundary;"); break;
            default: throw new InvalidOperationException("Unknown structured region");
        }
    }

    static IEnumerable<int> Temps(Il n) => n switch {
        IlLet l => new[] { l.Id }, IlBlock b => b.Body.SelectMany(Temps),
        IlIf i => i.Then.Concat(i.Else).SelectMany(Temps), _ => []
    };

    void Statements(IEnumerable<Il> nodes) {
        foreach(var n in nodes) switch(n) {
            case IlBlock b: Statements(b.Body); break;
            case IlIntrin { Name:"pusha16",Args:[] } when guest32 && externalDispatch:
                Line("pusha16_32(s,memory);"); break;
            case IlIntrin { Name:"popa16",Args:[] } when guest32 && externalDispatch:
                Line("popa16_32(s,memory);"); break;
            case IlIntrin { Name:"pusha32",Args:[] } when guest32 && externalDispatch:
                Line("pusha32_32(s,memory);"); break;
            case IlIntrin { Name:"popa32",Args:[] } when guest32 && externalDispatch:
                Line("popa32_32(s,memory);"); break;
            case IlIntrin { Name:"fstp-st",Args:[IlReadReg { Kind:RegKind.St,Idx:0 or 1 } target] } when guest32 && externalDispatch:
                if(instructionFpuOpcode!=0x5d8+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::fstp{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fst-st" or "fstp-st",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fst-st" ? 0x5d0 : 0x5d8)+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"pe32-xlat",Args:[] } when guest32 && externalDispatch:
                Line("{ auto offset=std::uint32_t(read_gpr_mask(s,3,0xffffffffu)+read_gpr_mask(s,0,255)); auto value=memory.load_scalar8(s.segments[3]+offset); auto old=scalar_gpr32(s,0); old.value=(old.value&0xffffff00u)|(value.value&255u); old.known=(old.known&0xffffff00u)|(value.known&255u); for(unsigned bit=0;bit<8;++bit)old.origins[bit]=value.origins[bit]; auto high=s.gpr[0]&0xffffffff00000000ULL; auto highKnown=(s.defined_gpr&1u)?0xffffffff00000000ULL:(s.partial_gpr[0]&0xffffffff00000000ULL); scalar_gpr32(s,0,old); s.gpr[0]|=high; if(highKnown!=0xffffffff00000000ULL){ s.defined_gpr&=~1u; s.partial_gpr[0]=highKnown|old.known; } }");
                break;
            case IlIntrin { Name:"pe32-rdtsc",Args:[] } when guest32 && externalDispatch:
                Line($"native_legacy(memory,s,0x{instructionAddress:x}ULL,2,0,0);");break;
            case IlIntrin { Name:"pe32-mmx" or "pe32-segment-store",Args:[IlConst legacyDescriptor,var legacyAddress] } legacy when guest32 && externalDispatch:
                Line($"native_legacy(memory,s,0x{instructionAddress:x}ULL,{(legacy.Name=="pe32-mmx"?0:1)},0x{legacyDescriptor.Bits:x}u,std::uint32_t({Expr(legacyAddress)}));");break;
            case IlIntrin { Name:"pe32-x87-environment",Args:[IlConst environmentOpcode,var environmentAddress] } when guest32 && externalDispatch:
                if(environmentOpcode.Bits!=instructionFpuOpcode)throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(environmentAddress)}),0x{instructionFpuOpcode:x});");break;
            case IlIntrin { Name:"pe32-x87-register",Args:[IlConst encodedOpcode] } when guest32 && externalDispatch:
                if(encodedOpcode.Bits!=instructionFpuOpcode)throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fabs",Args:[] } when guest32 && externalDispatch:
                if(instructionFpuOpcode!=0x1e1) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"f2xm1" or "fyl2x" or "fptan" or "fpatan" or "fxam" or "ftst" or "fprem1" or "fprem" or "fyl2xp1" or "frndint" or "fscale" or "fsin" or "fcos",Args:[] } value when guest32 && externalDispatch:
                int mathOpcode=value.Name switch {"f2xm1"=>0x1f0,"fyl2x"=>0x1f1,"fptan"=>0x1f2,"fpatan"=>0x1f3,"fxam"=>0x1e5,"ftst"=>0x1e4,"fprem1"=>0x1f5,"fprem"=>0x1f8,"fyl2xp1"=>0x1f9,"frndint"=>0x1fc,"fscale"=>0x1fd,"fsin"=>0x1fe,_=>0x1ff};
                if(instructionFpuOpcode!=mathOpcode)throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fxch",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } when guest32 && externalDispatch:
                if(instructionFpuOpcode!=0x1c8+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::fxch{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fmul-sti",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } when guest32 && externalDispatch:
                if(instructionFpuOpcode!=0x4c8+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::fmul_sti_{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fdiv-sti" or "fdivr-sti",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fdiv-sti" ? 0x4f8 : 0x4f0)+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::{(value.Name=="fdiv-sti" ? "fdiv" : "fdivr")}_sti_{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fdiv" or "fdivr",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fdiv" ? 0xf0 : 0xf8)+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::{value.Name}_st0_{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fcom" or "fcomp",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fcom" ? 0xd0 : 0xd8)+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::{value.Name}_st{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fucom" or "fucomp",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fucom" ? 0x5e0 : 0x5e8)+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fdivp" or "fdivrp",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fdivp" ? 0x6f8 : 0x6f0)+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fcompp" or "fucompp",Args:[] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode!=(value.Name=="fcompp" ? 0x6d9 : 0x2e9)) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fchs",Args:[] } when guest32 && externalDispatch:
                if(instructionFpuOpcode!=0x1e0) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::fchs,s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fadd" or "fsub" or "fsubr",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                int addSubSt0Opcode=value.Name switch { "fadd"=>0xc0,"fsub"=>0xe0,_=>0xe8 };
                if(instructionFpuOpcode!=addSubSt0Opcode+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fadd-sti" or "fsub-sti" or "fsubr-sti" or "fsubrp",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                int addSubStiOpcode=value.Name switch { "fadd-sti"=>0x4c0,"fsub-sti"=>0x4e8,"fsubr-sti"=>0x4e0,_=>0x6e0 };
                if(instructionFpuOpcode!=addSubStiOpcode+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fmul",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } when guest32 && externalDispatch:
                if(instructionFpuOpcode!=0xc8+target.Idx) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::fmul_st0_{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fld-st" or "fmulp" or "faddp" or "fsubp",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 } target] } value when guest32 && externalDispatch:
                int stackOpcode=value.Name switch { "fld-st"=>0x1c0,"fmulp"=>0x6c8,"faddp"=>0x6c0,_=>0x6e8 };
                if(instructionFpuOpcode!=stackOpcode+target.Idx) throw Unsupported(n);
                string stackOperation=value.Name=="fld-st" ? "fld_st" : value.Name+"_st";
                Line($"memory.execute_x87(X87Op::{stackOperation}{target.Idx},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fldl2t" or "fldl2e" or "fldpi" or "fldlg2" or "fldln2",Args:[] } value when guest32 && externalDispatch:
                int constantOpcode=value.Name switch {
                    "fldl2t"=>0x1e9,"fldl2e"=>0x1ea,"fldpi"=>0x1eb,"fldlg2"=>0x1ec,_=>0x1ed };
                if(instructionFpuOpcode!=constantOpcode) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fld1" or "fldz" or "fsqrt" or "fsincos",Args:[] } value when guest32 && externalDispatch:
                int unaryOpcode=value.Name switch { "fld1"=>0x1e8,"fldz"=>0x1ee,"fsqrt"=>0x1fa,_=>0x1fb };
                if(instructionFpuOpcode!=unaryOpcode) throw Unsupported(n);
                Line($"memory.execute_x87(X87Op::{value.Name},s,0x{instructionAddress:x}ULL,0,0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fild-m64-address",Args:[var address] } when guest32 && externalDispatch:
                if((instructionFpuOpcode&0x738)!=0x728 || (instructionFpuOpcode&0xc0)==0xc0) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(address)}),0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fadd-m32-address" or "fsub-m32-address" or "fsubr-m32-address" or "fsubr-m64-address" or "fdiv-m32-address" or "fdivr-m32-address",Args:[var address] } value when guest32 && externalDispatch:
                int memoryArithmeticOpcode=value.Name switch {
                    "fadd-m32-address"=>0,"fsub-m32-address"=>0x20,"fsubr-m32-address"=>0x28,
                    "fsubr-m64-address"=>0x428,"fdiv-m32-address"=>0x30,_=>0x38 };
                if((instructionFpuOpcode&0x738)!=memoryArithmeticOpcode || (instructionFpuOpcode&0xc0)==0xc0) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(address)}),0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fiadd-m16-address" or "fiadd-m32-address" or "fimul-m16-address" or "fimul-m32-address" or "fisub-m16-address" or "fisub-m32-address" or "fisubr-m16-address" or "fisubr-m32-address" or "fidiv-m16-address" or "fidiv-m32-address" or "fidivr-m16-address" or "fidivr-m32-address",Args:[var address] } value when guest32 && externalDispatch:
                int integerArithmeticOpcode=(value.Name.Contains("16") ? 0x600 : 0x200) | (value.Name.Split('-')[0] switch {
                    "fiadd"=>0,"fimul"=>8,"fisub"=>0x20,"fisubr"=>0x28,"fidiv"=>0x30,"fidivr"=>0x38,
                    _=>throw Unsupported(n) });
                if((instructionFpuOpcode&0x738)!=integerArithmeticOpcode || (instructionFpuOpcode&0xc0)==0xc0) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(address)}),0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fstp-m80-address",Args:[var address] } when guest32 && externalDispatch:
                if((instructionFpuOpcode&0x738)!=0x338 || (instructionFpuOpcode&0xc0)==0xc0) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(address)}),0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fist-m16-address" or "fist-m32-address" or "fistp-m16-address" or "fistp-m32-address" or "fistp-m64-address",Args:[var address] } value when guest32 && externalDispatch:
                int integerStoreOpcode=value.Name switch {
                    "fist-m16-address"=>0x710,"fist-m32-address"=>0x310,"fistp-m16-address"=>0x718,
                    "fistp-m32-address"=>0x318,_=>0x738 };
                if((instructionFpuOpcode&0x738)!=integerStoreOpcode || (instructionFpuOpcode&0xc0)==0xc0) throw Unsupported(n);
                Line($"memory.execute_x87(static_cast<X87Op>(0x{NativeEncodedX87:x}),s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(address)}),0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"fld-m32-address" or "fld-m64-address" or "fld-m80-address" or "fst-m32-address" or "fst-m64-address" or "fstp-m32-address" or "fstp-m64-address" or "fild-m32-address" or
                "fsub-m64-address" or "fmul-m32-address" or "fmul-m64-address" or "fdiv-m64-address" or "fdivr-m64-address" or "fadd-m64-address" or
                "fcom-m32-address" or "fcom-m64-address" or "fcomp-m32-address" or "fcomp-m64-address",Args:[var address] } value when guest32 && externalDispatch:
                if(instructionFpuOpcode==0) throw Unsupported(n);
                if(value.Name.StartsWith("fcom")) {
                    int compareOpcode=(value.Name.Contains("64") ? 0x400 : 0)|(value.Name.StartsWith("fcomp") ? 0x18 : 0x10);
                    if((instructionFpuOpcode&0x738)!=compareOpcode || (instructionFpuOpcode&0xc0)==0xc0) throw Unsupported(n);
                }
                string fpOperation=value.Name[..value.Name.IndexOf('-')];
                fpOperation+=value.Name.Contains("80") ? "80" : value.Name.Contains("64") ? "64" : "32";
                Line($"memory.execute_x87(X87Op::{fpOperation},s,0x{instructionAddress:x}ULL,std::uint32_t({Expr(address)}),0x{instructionFpuOpcode:x});");
                break;
            case IlIntrin { Name:"pe32-simd",Args:[IlConst descriptor,var address] } when guest32 && externalDispatch:
                Line($"native_simd(memory,s,0x{instructionAddress:x}ULL,0x{descriptor.Bits:x}u,std::uint32_t({Expr(address)}));");
                break;
            case IlIntrin { Name:"cpuid",Args:[] } when guest32 && externalDispatch:
                Line("{ auto leaf=std::uint32_t(read_gpr_mask(s,0,0xffffffff)); auto values=memory.cpuid(leaf,(leaf==0 || leaf==1) ? 0u : std::uint32_t(read_gpr_mask(s,1,0xffffffff)));");
                Line("s.gpr[0]=values[0]; s.gpr[3]=values[1]; s.gpr[1]=values[2]; s.gpr[2]=values[3]; s.defined_gpr|=0xf; }");
                break;
            case IlIntrin { Name:"fnclex",Args:[] } when guest32 && externalDispatch:
                Line("x87_fnclex(s);");
                break;
            case IlIntrin { Name:"fninit",Args:[] } when guest32 && externalDispatch:
                Line("x87_fninit(s);"); break;
            case IlIntrin { Name:"fwait",Args:[] } when guest32 && externalDispatch:
                Line($"x87_wait(s,0x{instructionAddress:x}ULL);"); break;
            case IlIntrin { Name:"fldcw",Args:[var value] } when guest32 && externalDispatch:
                Line($"x87_wait(s,0x{instructionAddress:x}ULL);");
                Line($"x87_fldcw(s,{Expr(value)});"); break;
            case IlIntrin { Name:"fnstcw-address",Args:[var address] } when guest32 && externalDispatch:
                Line($"{{ auto address=std::uint64_t({Expr(address)}); auto value=x87_control(s); memory.store(address,value,16); }}"); break;
            case IlIntrin { Name:"fnstsw-m-address",Args:[var address] } when guest32 && externalDispatch:
                Line($"{{ auto address=std::uint64_t({Expr(address)}); auto value=x87_status(s); memory.store(address,value,16); }}"); break;
            case IlIntrin { Name:"fnstsw-ax",Args:[] } when guest32 && externalDispatch:
                Line("write_gpr_slice(s,0,x87_status(s),0xffff);"); break;
            case IlIntrin { Name:"ldmxcsr-address",Args:[var address] } when guest32 && externalDispatch:
                Line($"write_mxcsr(s,memory.load(std::uint64_t({Expr(address)}),32),0x{instructionAddress:x}ULL);"); break;
            case IlIntrin { Name:"stmxcsr-address",Args:[var address] } when guest32 && externalDispatch:
                Line($"{{ auto address=std::uint64_t({Expr(address)}); auto value=read_mxcsr(s); memory.store(address,value,32); }}"); break;
            case IlIntrin { Name:"pe32-double-shift",Args:[IlConst descriptor,var address,var count] } when guest32 && externalDispatch:
                string shiftTemplate=count is IlConst shiftCount ? $"<{shiftCount.Bits}>" : "";
                Line($"double_shift32{shiftTemplate}(s,memory,0x{descriptor.Bits:x}u,std::uint64_t({Expr(address)}),std::uint32_t({Expr(count)}));");
                break;
            case IlIntrin { Name:"pe32-string-compare",Args:[IlConst descriptor] } when guest32 && externalDispatch:
                Line($"if(!string_compare32(s,memory,0x{descriptor.Bits:x}u,instruction_budget)) return Exit::budget_exhausted;");
                break;
            case IlIntrin { Name:"rep_movs",Args:[IlConst width] } when (width.Bits==8 || width.Bits==16) && guest32 && externalDispatch:
                Line($"if(!rep_movs_extra32(s,memory,{width.Bits},instruction_budget)) return Exit::budget_exhausted;");
                break;
            case IlIntrin { Name:"movs",Args:[IlConst width] } when width.Bits==8 && guest32 && externalDispatch:
                if(instructionPlainMovsWidth!=8) throw Unsupported(n);
                Line("movs_extra32(s,memory,8);");
                break;
            case IlIntrin { Name:"rep_movs",Args:[IlConst width] } when width.Bits==32 && guest32 && externalDispatch:
                Line("if(!rep_movsd32(s,memory,instruction_budget)) return Exit::budget_exhausted;");
                break;
            case IlIntrin { Name:"movs",Args:[IlConst width] } when (width.Bits==16 || width.Bits==32) && guest32 && externalDispatch:
                // Other segment/address-size prefix forms need their own
                // checked address contract; do not silently treat them as DS32.
                if(instructionPlainMovsWidth!=(int)width.Bits) throw Unsupported(n);
                Line($"movs32(s,memory,{width.Bits});");
                break;
            case IlIntrin { Name:"stos",Args:[IlConst width] } when width.Bits==32 && guest32 && externalDispatch:
                Line("stosd32(s,memory);");
                break;
            case IlIntrin { Name:"rep_stos",Args:[IlConst width] } when width.Bits==32 && guest32 && externalDispatch:
                Line("if(!rep_stosd32(s,memory,instruction_budget)) return Exit::budget_exhausted;");
                break;
            case IlIntrin { Name:"rep_stos",Args:[IlConst width] } when width.Bits==8 && guest32 && externalDispatch:
                Line("if(!rep_stosb32(s,memory,instruction_budget)) return Exit::budget_exhausted;");
                break;
            case IlIntrin { Name:"rep_stos",Args:[IlConst width] } when width.Bits==16 && guest32 && externalDispatch:
                Line("if(!rep_stosw32(s,memory,instruction_budget)) return Exit::budget_exhausted;");
                break;
            case IlFault { Architecture:"x86",Vector:>=0 and <=255 } fault:
                Line($"throw GuestCpuFault(\"x86\",{fault.Vector},0x{instructionAddress:x}ULL);");
                break;
            case IlLet l:
                if(ReferenceEquals(l,instructionNativeLogicalResult) && l.Val is IlBin logical && logical.Ty is IlType.I logicalType) {
                    bool oldAnd=instructionAndMemoryZero,oldOr=instructionOrImmediate;
                    instructionAndMemoryZero=false;instructionOrImmediate=false;
                    string left=Expr(logical.L),right=Expr(logical.R);
                    instructionAndMemoryZero=oldAnd;instructionOrImmediate=oldOr;
                    string calculation=logical.Op switch {
                        BinOp.And=>"bits(native_logical_left & native_logical_right)",
                        BinOp.Or=>"bits(native_logical_left | native_logical_right)",
                        BinOp.Xor=>"bits(native_logical_left ^ native_logical_right)",
                        BinOp.Shl=>$"shift_left(bits(native_logical_left),bits(native_logical_right),{logicalType.Bits})",
                        BinOp.Shr=>$"shift_right(bits(native_logical_left),bits(native_logical_right),{logicalType.Bits})",
                        BinOp.Sar=>$"shift_arithmetic(sext(bits(native_logical_left),{logicalType.Bits},{logicalType.Bits}),bits(native_logical_right),{logicalType.Bits})",
                        BinOp.Mul when instructionNativeAluKind==8=>"(bits(native_logical_left) * bits(native_logical_right))",
                        BinOp.Mul=>$"(sext(bits(native_logical_left),{logicalType.Bits/2},{logicalType.Bits}) * sext(bits(native_logical_right),{logicalType.Bits/2},{logicalType.Bits}))",
                        BinOp.UDiv=>"unsigned_divide(bits(native_logical_left),bits(native_logical_right),false)",
                        BinOp.SDiv=>$"signed_divide(bits(native_logical_left),bits(native_logical_right),{logicalType.Bits},false)",
                        _=>throw Unsupported(l)};
                    Line("#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)");
                    string nativeOperandType=instructionNativeAluKind is 10 or 11 ? "std::uint64_t":"std::uint32_t";
                    Line($"auto native_logical_left={nativeOperandType}({left}),native_logical_right={nativeOperandType}({right});");
                    Line(logical.Op is BinOp.Shl or BinOp.Shr or BinOp.Sar
                        ? "if(native_logical_right&31u) native_require_alu_flags(s);" : "native_require_alu_flags(s);");
                    Line("auto native_logical_flags=s.flags,native_logical_known=s.defined_flags;");
                    Line($"{Temporary(l.Id)}=narrow({calculation},{logicalType.Bits});");
                    Line("#else");
                    Line($"{Temporary(l.Id)} = {Expr(l.Val)};");
                    Line("#endif");
                } else Line($"{Temporary(l.Id)} = {Expr(l.Val)};");
                defined.Add(l.Id); break;
            case IlWriteReg { Kind: RegKind.X86, Idx: >= 0 and < 16 } r:
                if(externalDispatch && PartialGprWrite(r,out var inserted,out ulong mask)) {
                    Line($"write_gpr_slice(s,{r.Idx},{Expr(inserted)},0x{mask:x}ULL);");
                    break;
                }
                bool carryUnknownLoad=externalDispatch && guest32 && ScalarLoad32(r.Val);
                if(carryUnknownLoad) { Line("try {"); indent++; }
                Line($"s.gpr[{r.Idx}] = std::uint64_t({Expr(r.Val)});");
                if(externalDispatch) { Line($"s.defined_gpr |= std::uint16_t(1) << {r.Idx};"); Line($"s.partial_gpr[{r.Idx}]=0;"); }
                if(carryUnknownLoad) { indent--; Line($"}} catch(const UnknownMemoryRead& error) {{ scalar_gpr32(s,{r.Idx},error.scalar32); }}"); }
                break;
            case IlWriteReg { Kind:RegKind.Xmm,Idx:>=0 and <8,Val:IlLoad { Ty:IlType.I { Bits:128 } } load } destination when externalDispatch && guest32 && instructionMovapsLoad:
                Line($"{{ auto address=std::uint64_t({Expr(load.Addr)}); if(address&15u) throw GuestCpuFault(\"x86\",13,0x{instructionAddress:x}ULL);");
                Line($"auto value=memory.load_defined(address,128); s.xmm[{destination.Idx}]=value.value;");
                Line($"s.defined_xmm=std::uint16_t((s.defined_xmm & ~(1u<<{destination.Idx})) | (unsigned(value.defined)<<{destination.Idx})); }}");
                break;
            case IlWriteReg { Kind:RegKind.Xmm,Idx:>=0 and <8,Val:IlReadReg { Kind:RegKind.Xmm,Idx:>=0 and <8,Ty:IlType.I { Bits:128 } } source } destination when externalDispatch && guest32:
                Line($"s.xmm[{destination.Idx}]=s.xmm[{source.Idx}];");
                Line($"s.defined_xmm=std::uint16_t((s.defined_xmm & ~(1u<<{destination.Idx})) | (((s.defined_xmm>>{source.Idx})&1u)<<{destination.Idx}));");
                break;
            case IlWriteReg { Kind:RegKind.Xmm,Idx:>=0 and <8,Val:IlBin { Op:BinOp.Xor,Ty:IlType.I { Bits:128 },L:IlReadReg { Kind:RegKind.Xmm } left,R:IlReadReg { Kind:RegKind.Xmm } right } } destination when externalDispatch && guest32 && left.Idx==right.Idx && left.Idx==destination.Idx:
                Line($"s.xmm[{destination.Idx}]=0; s.defined_xmm|=std::uint16_t(1u<<{destination.Idx});");
                break;
            case IlWriteReg { Kind: RegKind.Eflags, Idx: >= 0 and < 64, Val: IlUndefined } r:
                Line($"s.defined_flags &= ~(std::uint64_t(1) << {r.Idx}); s.flag_origins[{r.Idx}]=fresh_unknown_flag_origin(s,{r.Idx});"); break;
            case IlWriteReg { Kind: RegKind.Eflags, Idx: >= -1 and < 64 } r:
                Line($"write_flag(s, {r.Idx}, {Expr(r.Val)});"); break;
            case IlStore store: {
                // Sequence address before value, as in LiftIl execution. Do not
                // rely on unspecified C++ argument evaluation order for loads.
                string address = Expr(store.Addr);
                int width = Width(store.Val);
                if(width % 8 != 0) throw Unsupported(n);
                if(externalDispatch && guest32 && store.Val is IlReadReg { Kind:RegKind.Xmm,Idx:>=0 and <8 } vector) {
                    if(width!=128 || !instructionAlignedVectorStore) throw Unsupported(n);
                    Line($"{{ auto address=std::uint64_t({address}); if(address&15u) throw GuestCpuFault(\"x86\",13,0x{instructionAddress:x}ULL);");
                    Line($"if(s.defined_xmm&(1u<<{vector.Idx})) memory.store(address,s.xmm[{vector.Idx}],128); else memory.store_unknown(address,s.xmm[{vector.Idx}],128); }}");
                    break;
                }
                string value=Expr(store.Val);
                string operation=objects?.Store(store,"address","value") ?? $"memory.store(address, value, {width});";
                if(externalDispatch && guest32 && width==32 && ScalarRegister32(store.Val) is int source) {
                    Line($"{{ auto address=std::uint64_t({address});"); indent++;
                    Line($"if((s.defined_gpr&(1u<<{source})) || (s.partial_gpr[{source}]&0xffffffffULL)==0xffffffffULL) {{ auto value={value}; {operation} }}");
                    Line($"else memory.store_scalar32(address,scalar_gpr32(s,{source}));");
                    indent--; Line("}");
                } else Line($"{{ auto address = std::uint64_t({address}); auto value = {value}; {operation} }}");
                break;
            }
            case IlBranch b: {
                if(branchWriter != null) { branchWriter(b); break; }
                if(b.Kind == BranchKind.Call && !allowsCalls) throw Unsupported(n);
                if(b.Cond != null) { Line($"if(({Expr(b.Cond)} & 1) != 0) {{"); indent++; }
                Line($"s.pc = std::uint64_t({Expr(b.Target)});");
                if(b.Kind == BranchKind.Ret && allowsCalls && !externalDispatch) Line("returning = true;");
                Line(externalDispatch && b.Kind==BranchKind.Call ? "return Exit::transfer;"
                    : b.Kind==BranchKind.Ret && (externalDispatch || !allowsCalls) ? "return Exit::returned;" : "goto dispatch;");
                if(b.Cond != null) { indent--; Line("}"); }
                break;
            }
            case IlIf conditional: {
                Line($"if(({Expr(conditional.Cond)} & 1) != 0) {{"); indent++;
                var before = new HashSet<int>(defined);
                Statements(conditional.Then); var afterThen = new HashSet<int>(defined);
                indent--; Line("} else {"); indent++;
                defined = before; Statements(conditional.Else);
                defined.IntersectWith(afterThen);
                indent--; Line("}"); break;
            }
            default: throw Unsupported(n);
        }
    }

    static bool ScalarLoad32(Il value) => value switch {
        IlLoad { Ty:IlType.I { Bits:32 } } => true,
        IlCast { Kind:CastKind.Zext or CastKind.Trunc,Ty:IlType.I { Bits:>=32 },X:var inner } => ScalarLoad32(inner),
        _ => false
    };
    static int? ScalarRegister32(Il value) => value switch {
        IlReadReg { Kind:RegKind.X86,Idx:>=0 and <16 } register => register.Idx,
        IlCast { Kind:CastKind.Zext or CastKind.Trunc,Ty:IlType.I { Bits:>=32 },X:var inner } => ScalarRegister32(inner),
        _ => null
    };
    static bool PartialGprWrite(IlWriteReg write,out Il inserted,out ulong mask) {
        inserted=write.Val; mask=0;
        if(write.Val is not IlBin { Op:BinOp.Or,L:IlBin { Op:BinOp.And,L:IlReadReg { Kind:RegKind.X86 } old,R:IlConst keep },R:var value } ||
           old.Idx!=write.Idx || Width(write.Val)!=64 || keep.Bits>ulong.MaxValue) return false;
        mask=~(ulong)keep.Bits;
        if(mask is not (0xff or 0xff00 or 0xffff)) return false;
        inserted=value; return true;
    }
    static ulong LowMask(int width) => width>=64 ? ulong.MaxValue : (1UL<<width)-1;
    string Expr(Il n,ulong? demanded=null) {
        int width = Width(n);
        ulong required=(demanded ?? LowMask(width))&LowMask(width);
        string value = n switch {
            IlConst c => $"((bits(0x{(ulong)(c.Bits >> 64):x}ULL) << 64) | bits(0x{(ulong)c.Bits:x}ULL))",
            IlReadReg { Kind: RegKind.X86, Idx: >= 0 and < 16 } r => externalDispatch ? $"read_gpr_mask(s,{r.Idx},0x{required:x}ULL)" : $"bits(s.gpr[{r.Idx}])",
            IlReadReg { Kind: RegKind.Eflags, Idx: >= -1 and < 64 } r => $"read_flags(s, {r.Idx})",
            IlReadReg { Kind: RegKind.X86Seg, Idx: >= 0 and < 6 } r => $"bits(s.segments[{r.Idx}])",
            IlReadPc => "bits(s.pc)",
            IlTmp t when defined.Contains(t.Id) => Temporary(t.Id),
            IlBin b => Binary(b,required),
            IlUn u => Unary(u),
            IlCast c when c.Kind is CastKind.Zext or CastKind.Trunc or CastKind.Bitcast => Expr(c.X,required&LowMask(Width(c.X))),
            IlCast { Kind: CastKind.Sext } c => $"sext({Expr(c.X)}, {Width(c.X)}, {width})",
            IlLoad l when width % 8 == 0 => Load(l,width),
            IlIfV c => $"(({Expr(c.Cond)} & 1) != 0 ? {Expr(c.Then)} : {Expr(c.Else)})",
            _ => throw Unsupported(n)
        };
        return Narrow(value, width);
    }

    string Load(IlLoad load,int width) {
        string address=$"std::uint64_t({Expr(load.Addr)})";
        return objects?.Load(load,address) ?? $"memory.load({address}, {width})";
    }

    string Binary(IlBin b,ulong required) {
        int width = Width(b), leftWidth = Width(b.L), rightWidth = Width(b.R);
        if(instructionAndMemoryZero && b.Op==BinOp.And && width==32 && b.L is IlLoad load && b.R is IlConst zero && zero.Bits==0)
            return $"read_for_zero_and32(memory,std::uint64_t({Expr(load.Addr)}))";
        bool comparison = b.Op is BinOp.Eq or BinOp.Ne or BinOp.Ult or BinOp.Ule or BinOp.Ugt or BinOp.Uge
            or BinOp.Slt or BinOp.Sle or BinOp.Sgt or BinOp.Sge;
        if(comparison && leftWidth != rightWidth) throw Unsupported(b);
        ulong? leftDemand=null;
        if(width<=64 && b.R is IlConst shift && shift.Bits<(uint)width) {
            int amount=(int)shift.Bits;
            if(b.Op==BinOp.Shr) leftDemand=(required<<amount)&LowMask(width);
            else if(b.Op==BinOp.Shl) leftDemand=required>>amount;
        }
        if(instructionOrImmediate && width==32 && b.Op==BinOp.Or && b.R is IlConst constant)
            leftDemand=required & ~((ulong)constant.Bits);
        string a = Expr(b.L,leftDemand), c = Expr(b.R);
        // Capture left then right: nested loads must keep LiftIl effect order.
        string expression = b.Op switch {
            BinOp.Add => "a + b", BinOp.Sub => "a - b", BinOp.Mul => "a * b",
            BinOp.And => "a & b", BinOp.Or => "a | b", BinOp.Xor => "a ^ b",
            BinOp.Shl => $"shift_left(a, b, {width})", BinOp.Shr => $"shift_right(a, b, {width})",
            BinOp.Sar => $"shift_arithmetic(sext(a, {leftWidth}, {width}), b, {width})",
            BinOp.Ror => $"rotate_right(a, b, {width})",
            BinOp.UDiv => "unsigned_divide(a, b, false)", BinOp.URem => "unsigned_divide(a, b, true)",
            BinOp.SDiv => $"signed_divide(a, b, {width}, false)", BinOp.SRem => $"signed_divide(a, b, {width}, true)",
            BinOp.Eq => "a == b", BinOp.Ne => "a != b",
            BinOp.Ult => "a < b", BinOp.Ule => "a <= b", BinOp.Ugt => "a > b", BinOp.Uge => "a >= b",
            BinOp.Slt => $"signed_key(a, {leftWidth}) < signed_key(b, {rightWidth})",
            BinOp.Sle => $"signed_key(a, {leftWidth}) <= signed_key(b, {rightWidth})",
            BinOp.Sgt => $"signed_key(a, {leftWidth}) > signed_key(b, {rightWidth})",
            BinOp.Sge => $"signed_key(a, {leftWidth}) >= signed_key(b, {rightWidth})",
            _ => throw Unsupported(b)
        };
        return $"([&]() -> bits {{ auto a = {a}; auto b = {c}; return {expression}; }}())";
    }

    string Unary(IlUn u) {
        string x = Expr(u.X);
        return u.Op switch {
            UnOp.Neg => $"bits(0) - ({x})", UnOp.Not => $"~({x})",
            UnOp.Popcnt => $"popcount({x}, {Width(u.X)})",
            UnOp.Clz => $"leading_zeroes({x}, {Width(u.X)})",
            UnOp.Rbit => $"reverse_bits({x}, {Width(u.X)})", _ => throw Unsupported(u)
        };
    }
}
