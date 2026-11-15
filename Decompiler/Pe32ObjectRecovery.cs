using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LiftIl;

namespace SharpRetro.Decompiler;

public sealed record Pe32FieldSite(ulong InstructionRva,string Mode);
public sealed record Pe32Field(string Origin,string Name,int Offset,int Width,string Type,bool Read,bool Write,
    bool Overlapping,IReadOnlyList<Pe32FieldSite> Sites,IReadOnlyList<string> PointerTargets);
public sealed record Pe32Object(string Origin,string Name,string BaseOrigin,int? EntryRegister,
    string? SourceFieldOrigin,IReadOnlyList<Pe32Field> Fields);
public sealed record Pe32StructureAnnotation(string Name,Dictionary<string,CppVariableAnnotation> Fields);
public sealed record Pe32UnrecoveredAccess(ulong InstructionRva,int Width,string Mode,string Reason);
public sealed record Pe32StackAccess(ulong InstructionRva,int Offset,int Width,bool Write,bool StackSegment);

/// Observed sparse guest32 views, never host layouts or allocation bounds.
public sealed class Pe32ObjectPlan {
    public IReadOnlyList<Pe32Object> Objects { get; }
    public IReadOnlyList<Pe32UnrecoveredAccess> UnrecoveredAccesses { get; }
    public IReadOnlyList<Pe32StackAccess> StackAccesses { get; }
    public IReadOnlyList<int> EntryRegisterReads { get; }
    readonly Dictionary<Il,(Pe32Object Object,Pe32Field Field)> accesses;
    internal Pe32ObjectPlan(IReadOnlyList<Pe32Object> objects,List<Pe32UnrecoveredAccess> unrecovered,
        Dictionary<Il,(Pe32Object,Pe32Field)> accesses,IReadOnlyList<Pe32StackAccess> stackAccesses,IReadOnlyList<int> entryRegisterReads) {
        Objects=objects; UnrecoveredAccesses=unrecovered; this.accesses=accesses;
        StackAccesses=stackAccesses; EntryRegisterReads=entryRegisterReads;
    }
    static string Offset(Pe32Field field) => $"0x{unchecked((uint)field.Offset):x8}u";
    static string Type(Pe32Field field) => CppVariableOrigins.Type(field.Type,field.Width);
    static string Instance(Pe32Object obj,Pe32Field field,string address) =>
        $"view_{obj.Name}{{memory,static_cast<std::uint32_t>(std::uint64_t({address}) - {Offset(field)})}}";
    public string? Load(IlLoad load,string address) => accesses.TryGetValue(load,out var a)
        ? $"{Instance(a.Object,a.Field,address)}.read_{a.Field.Name}()" : null;
    public string? Store(IlStore store,string address,string value) => accesses.TryGetValue(store,out var a)
        ? $"{Instance(a.Object,a.Field,address)}.write_{a.Field.Name}({value});" : null;
    public IEnumerable<string> Declarations() {
        foreach(var obj in Objects) {
            yield return $"// Observed guest32 fields; class identity and allocation bounds unknown. Origin: {obj.Origin}";
            yield return $"struct view_{obj.Name} {{";
            yield return "    sharp_retro::Memory& memory;";
            yield return "    std::uint32_t base;";
            foreach(var field in obj.Fields) {
                yield return $"    // {field.Origin}{(field.Overlapping ? "; overlapping observed storage" : "")}";
                yield return $"    static constexpr std::uint32_t {field.Name}_offset = {Offset(field)};";
                if(field.Read) yield return $"    {Type(field)} read_{field.Name}() const {{ return static_cast<{Type(field)}>(memory.load(static_cast<std::uint32_t>(base + {field.Name}_offset),{field.Width})); }}";
                if(field.Write) {
                    yield return $"    void write_{field.Name}(sharp_retro::bits value) const {{";
                    yield return $"        auto field_address = static_cast<std::uint32_t>(base + {field.Name}_offset);";
                    yield return $"        memory.store(field_address, value, {field.Width});";
                    yield return "    }";
                }
            }
            yield return "};";
        }
    }
}

/// Abstract interpretation tracks low-32 affine base+offset facts. Only proven
/// zero-extended guest32 effective addresses are lowered. Unsupported/segmented,
/// indexed and conflicting join facts stay raw. No load, branch or effect is removed.
public static class Pe32ObjectRecovery {
    sealed record Affine(string? Root,uint Offset,bool Address32,bool StackSegment=false);
    sealed record RootInfo(string Origin,int? EntryRegister,IlLoad? Source,bool Stack);
    sealed record Access(Il Node,ulong InstructionRva,Affine Address,int Width,bool Write);
    sealed record Temporary(Affine? Value,int Width);
    sealed class Flow(Affine?[] registers) {
        public Affine?[] Registers=registers;
        public Dictionary<int,Temporary> Temps=[];
        public Flow Copy() => new((Affine?[])Registers.Clone()) { Temps=new(Temps) };
    }
    static int Width(Il node) => node.Ty is IlType.I type ? type.Bits : 0;
    static bool Identifier(string name) => Regex.IsMatch(name,@"\A[a-zA-Z][a-zA-Z0-9_]*\z") && !name.Contains("__") &&
        !Regex.IsMatch("alignas alignof and and_eq asm auto bitand bitor bool break case catch char char8_t char16_t char32_t class compl concept const consteval constexpr constinit const_cast continue co_await co_return co_yield decltype default delete do double dynamic_cast else enum explicit export extern false float for friend goto if inline int long mutable namespace new noexcept not not_eq nullptr operator or or_eq private protected public register reinterpret_cast requires return short signed sizeof static static_assert static_cast struct switch template this thread_local throw true try typedef typeid typename union unsigned using virtual void volatile wchar_t while xor xor_eq",
            @"\b"+Regex.Escape(name)+@"\b");

    public static Pe32ObjectPlan Analyze(LiftedProgram program,
        IReadOnlyDictionary<string,Pe32StructureAnnotation>? annotations=null) {
        if(program.Image.Format!=BinaryFormat.Pe || program.Image.Architecture!=Architecture.X86)
            throw new NotSupportedException("Guest32 structure recovery requires PE32");
        var graph=BasicBlocks.Build(program,includeCalls:true,allowDynamicTargets:true);
        var roots=new Dictionary<string,RootInfo>();
        var entry=new Affine?[16];
        for(int i=0;i<8;i++) {
            string origin=$"entry-register-v1:{i}";
            roots[origin]=new(origin,i,null,i==4);
            entry[i]=new(origin,0,false);
        }
        roots["stack-segment-v1"]=new("stack-segment-v1",null,null,false);
        var stackAccesses=new List<Pe32StackAccess>();
        var entryReads=new HashSet<int>();
        var inputs=new Dictionary<ulong,Affine?[]> { [program.Entry]=entry };
        var queue=new Queue<ulong>(); var queued=new HashSet<ulong> { program.Entry }; queue.Enqueue(program.Entry);
        // Indirectly entered case blocks have no propagated affine contract.
        // Seed unknown facts so joins cannot retain primary-path assumptions.
        foreach(var extra in program.ExtraEntries.Where(address=>address!=program.Entry)) {
            inputs[extra]=new Affine?[16];
            if(queued.Add(extra)) queue.Enqueue(extra);
        }
        int budget=checked(graph.Blocks.Count*128+1);
        List<Access>? recorded=null;
        List<Pe32UnrecoveredAccess>? unrecorded=null;
        ulong instructionRva=0;
        string instructionOrigin="";
        var accessFacts=new Dictionary<Il,Affine?>(ReferenceEqualityComparer.Instance);

        Affine? Cast(Affine? value,int from,int to,CastKind kind) {
            if(value?.StackSegment==true && to<=32) return null;
            if(value==null || to<32 && value.Root!=null || kind==CastKind.Sext && from<32) return null;
            bool address32=to<=32 || kind!=CastKind.Sext && value.Address32;
            return value with { Address32=address32,Offset=to<32 ? value.Offset & ((1u<<to)-1) : value.Offset };
        }
        void Observe(Il node,Affine? address,int width,bool write) {
            if(recorded==null || unrecorded==null) return;
            if(address?.Root is { } stackRoot && roots[stackRoot].Stack)
                stackAccesses.Add(new(instructionRva,unchecked((int)address.Offset),width,write,address.StackSegment));
            string? reason=width is not (8 or 16 or 32 or 64 or 128) ? "unsupported field width"
                : address?.Root==null ? "unknown, constant, segmented or indexed address"
                : roots[address.Root].Stack ? "entry-stack-derived storage"
                : !address.Address32 ? "effective address not proven zero-extended guest32" : null;
            var fact=reason==null ? address : null;
            if(accessFacts.TryGetValue(node,out var previous) && previous!=fact) accessFacts[node]=null;
            else accessFacts.TryAdd(node,fact);
            if(reason!=null) unrecorded.Add(new(instructionRva,width,write ? "write" : "read",reason));
            else recorded.Add(new(node,instructionRva,address!,width,write));
        }
        Affine? Eval(Il node,Flow state,string path) {
            int width=Width(node);
            switch(node) {
                case IlConst constant:
                    return new(null,unchecked((uint)constant.Bits) & (width is >0 and <32 ? (1u<<width)-1 : uint.MaxValue),
                        width<=32 || constant.Bits<=uint.MaxValue);
                case IlReadReg { Kind:RegKind.X86,Idx:>=0 and <16 } reg:
                    if(recorded!=null && state.Registers[reg.Idx]?.Root is { } root && roots[root].EntryRegister is { } entryRegister)
                        entryReads.Add(entryRegister);
                    return Cast(state.Registers[reg.Idx],64,width,CastKind.Trunc);
                case IlReadReg { Kind:RegKind.X86Seg,Idx:2 }: return new("stack-segment-v1",0,false);
                case IlTmp temporary:
                    return state.Temps.TryGetValue(temporary.Id,out var saved) ? Cast(saved.Value,saved.Width,width,CastKind.Trunc) : null;
                case IlCast cast: return Cast(Eval(cast.X,state,path+"/x"),Width(cast.X),width,cast.Kind);
                case IlLoad load: {
                    var address=Eval(load.Addr,state,path+"/address"); Observe(load,address,width,false);
                    string origin=$"load-v1:{instructionRva:x8}:{instructionOrigin}:{path}";
                    roots.TryAdd(origin,new(origin,null,load,false));
                    return new(origin,0,width<=32);
                }
                case IlBin binary: {
                    var left=Eval(binary.L,state,path+"/left"); var right=Eval(binary.R,state,path+"/right");
                    if(left==null || right==null || width<32) return null;
                    if(binary.Op==BinOp.Add && width==64) {
                        var stack=left.Root=="stack-segment-v1" ? right : right.Root=="stack-segment-v1" ? left : null;
                        if(stack?.Root is { } sr && roots[sr].Stack && stack.Address32)
                            return stack with { Address32=false,StackSegment=true };
                    }
                    if(binary.Op==BinOp.Add && (left.Root==null || right.Root==null))
                        return new(left.Root ?? right.Root,unchecked(left.Offset+right.Offset),width==32);
                    if(binary.Op==BinOp.Sub && right.Root==null)
                        return new(left.Root,unchecked(left.Offset-right.Offset),width==32);
                    return null;
                }
                case IlUn unary: _=Eval(unary.X,state,path+"/x"); return null;
                case IlIfV conditional: {
                    _=Eval(conditional.Cond,state,path+"/condition");
                    var left=Eval(conditional.Then,state,path+"/then"); var right=Eval(conditional.Else,state,path+"/else");
                    return left==right ? left : null;
                }
                default: return null;
            }
        }
        void Statements(IReadOnlyList<Il> statements,Flow state,string path) {
            for(int index=0;index<statements.Count;index++) {
                string position=path+"/"+index;
                switch(statements[index]) {
                    case IlBlock block: Statements(block.Body,state,position); break;
                    case IlLet let: state.Temps[let.Id]=new(Eval(let.Val,state,position+"/value"),Width(let.Val)); break;
                    case IlWriteReg reg: {
                        var value=Eval(reg.Val,state,position+"/value");
                        if(reg.Kind==RegKind.X86 && reg.Idx is >=0 and <16) state.Registers[reg.Idx]=value;
                        break;
                    }
                    case IlStore store: {
                        var address=Eval(store.Addr,state,position+"/address");
                        _=Eval(store.Val,state,position+"/value"); Observe(store,address,Width(store.Val),true); break;
                    }
                    case IlIf conditional: {
                        _=Eval(conditional.Cond,state,position+"/condition");
                        var left=state.Copy(); var right=state.Copy();
                        Statements(conditional.Then,left,position+"/then"); Statements(conditional.Else,right,position+"/else");
                        for(int i=0;i<16;i++) state.Registers[i]=left.Registers[i]==right.Registers[i] ? left.Registers[i] : null;
                        state.Temps=left.Temps.Where(p=>right.Temps.GetValueOrDefault(p.Key)==p.Value).ToDictionary();
                        break;
                    }
                    case IlBranch branch:
                        _=Eval(branch.Target,state,position+"/target");
                        if(branch.Cond!=null) _=Eval(branch.Cond,state,position+"/condition"); break;
                    case IlIntrin { Name:"cpuid",Args:[] }:
                        for(int i=0;i<4;i++) state.Registers[i]=null;
                        break;
                    case IlIntrin { Name:"pusha16" or "pusha32",Args:[] } intrinsic:
                        state.Registers[4]=null;
                        unrecorded?.Add(new(instructionRva,intrinsic.Name.EndsWith("32") ? 32 : 16,"write","eight legacy register stack stores")); break;
                    case IlIntrin { Name:"popa16" or "popa32",Args:[] } intrinsic:
                        for(int i=0;i<8;i++) state.Registers[i]=null;
                        unrecorded?.Add(new(instructionRva,intrinsic.Name.EndsWith("32") ? 32 : 16,"read","seven legacy register stack loads; saved SP skipped")); break;
                    case IlIntrin { Name:"pe32-xlat",Args:[] }:
                        _=Eval(new IlReadReg(IlType.U32,RegKind.X86,3),state,position+"/xlat-base");
                        _=Eval(new IlReadReg(IlType.U8,RegKind.X86,0),state,position+"/xlat-index");
                        state.Registers[0]=null;unrecorded?.Add(new(instructionRva,8,"read","DS:(EBX+AL) modulo32"));break;
                    case IlIntrin { Name:"pe32-rdtsc",Args:[] }:
                        state.Registers[0]=state.Registers[2]=null;break;
                    case IlIntrin { Name:"pe32-segment-store",Args:[IlConst descriptor,var address] }:
                        _=Eval(address,state,position+"/selector-address");
                        if(((uint)descriptor.Bits&0x80)!=0)unrecorded?.Add(new(instructionRva,16,"write","native segment selector"));
                        else state.Registers[(int)((descriptor.Bits>>3)&7)]=null;break;
                    case IlIntrin { Name:"pe32-x87-environment",Args:[IlConst descriptor,var address] }:
                        _=Eval(address,state,position+"/x87-environment-address");
                        unrecorded?.Add(new(instructionRva,((uint)descriptor.Bits&0x400)!=0?864:224,((uint)descriptor.Bits&0x30)==0x30?"write":"read","32-bit x87 environment image"));break;
                    case IlIntrin { Name:"pe32-mmx",Args:[IlConst descriptor,var address] }:
                        _=Eval(address,state,position+"/mmx-address");uint legacyDescriptor=(uint)descriptor.Bits;
                        if((legacyDescriptor&0x4000)!=0)unrecorded?.Add(new(instructionRva,(legacyDescriptor&255) is 0x6e or 0x7e?32:64,(legacyDescriptor&255) is 0x7e or 0x7f?"write":"read","MMX operand aliased to physical x87 registers"));
                        else if((legacyDescriptor&255)==0x6e)_=Eval(new IlReadReg(IlType.U32,RegKind.X86,(int)((legacyDescriptor>>11)&7)),state,position+"/mmx-gpr-source");
                        else if((legacyDescriptor&255)==0x7e)state.Registers[(int)((legacyDescriptor>>11)&7)]=null;break;
                    case IlIntrin { Name:"pe32-x87-register",Args:[IlConst] }:
                        break; // Integer flags and x87 payloads only.
                    case IlIntrin { Name:"fnclex" or "fninit" or "fwait",Args:[] }:
                        // Only x87 exception/status bits change; no GPR or guest
                        // memory facts arise from clearing pending exceptions.
                        break;
                    case IlIntrin { Name:"fst-st" or "fstp-st",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                        break; // Register-stack destination, no affine GPR/memory fact.
                    case IlIntrin { Name:"fxch",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                        break; // Raw x87 register exchange, no affine GPR/memory fact.
                    case IlIntrin { Name:"fmul-sti" or "fadd-sti" or "fsub-sti" or "fsubr-sti",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                    case IlIntrin { Name:"fdiv-sti" or "fdivr-sti",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                    case IlIntrin { Name:"fld-st" or "fmulp" or "faddp" or "fsubp" or "fsubrp" or "fdivp" or "fdivrp",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                    case IlIntrin { Name:"fmul" or "fadd" or "fsub" or "fsubr",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                    case IlIntrin { Name:"fcom" or "fcomp",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                    case IlIntrin { Name:"fdiv" or "fdivr",Args:[IlReadReg { Kind:RegKind.St,Idx:0 },IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                    case IlIntrin { Name:"fucom" or "fucomp",Args:[IlReadReg { Kind:RegKind.St,Idx:>=0 and <8 }] }:
                        break; // Register arithmetic; the opcode determines its destination.
                    case IlIntrin { Name:"fcompp" or "fucompp",Args:[] }:
                        break; // x87 condition bits and two pops; no affine GPR fact.
                    case IlIntrin { Name:"fld1" or "fldz" or "fldl2t" or "fldl2e" or "fldpi" or "fldlg2" or "fldln2" or "fsqrt" or "fsincos" or "fchs" or "fabs" or "f2xm1" or "fyl2x" or "fptan" or "fpatan" or "fxam" or "ftst" or "fprem1" or "fprem" or "fyl2xp1" or "frndint" or "fscale" or "fsin" or "fcos",Args:[] }:
                        break; // Explicit x87 value provider, no affine GPR facts.
                    case IlIntrin { Name:"fld-m32-address" or "fld-m64-address" or "fld-m80-address" or "fst-m32-address" or "fst-m64-address" or "fstp-m32-address" or "fstp-m64-address" or "fstp-m80-address" or "fild-m32-address" or "fild-m64-address" or
                        "fiadd-m16-address" or "fiadd-m32-address" or "fimul-m16-address" or "fimul-m32-address" or "fisub-m16-address" or "fisub-m32-address" or "fisubr-m16-address" or "fisubr-m32-address" or "fidiv-m16-address" or "fidiv-m32-address" or "fidivr-m16-address" or "fidivr-m32-address" or
                        "fsub-m32-address" or "fsub-m64-address" or "fsubr-m32-address" or "fsubr-m64-address" or "fmul-m32-address" or "fmul-m64-address" or "fdiv-m32-address" or "fdiv-m64-address" or "fdivr-m32-address" or "fdivr-m64-address" or "fadd-m32-address" or "fadd-m64-address" or
                        "fcom-m32-address" or "fcom-m64-address" or "fcomp-m32-address" or "fcomp-m64-address" or "fist-m16-address" or "fist-m32-address" or "fistp-m16-address" or "fistp-m32-address" or "fistp-m64-address",Args:[var address] } value:
                        _=Eval(address,state,position+"/fp-address");
                        unrecorded?.Add(new(instructionRva,value.Name.Contains("80") ? 80 : value.Name.Contains("64") ? 64 : value.Name.Contains("16") ? 16 : 32,value.Name.StartsWith("fst") || value.Name.StartsWith("fist") ? "write" : "read","explicit x87 value access")); break;
                    case IlIntrin { Name:"pe32-simd",Args:[IlConst descriptor,var address] }:
                        _=Eval(address,state,position+"/simd-address");
                        uint simdDescriptor=(uint)descriptor.Bits;
                        if((simdDescriptor&0x4000)!=0)unrecorded?.Add(new(instructionRva,Pe32SimdLowering.MemoryWidth(simdDescriptor),Pe32SimdLowering.Store(simdDescriptor)?"write":"read","explicit legacy SSE operand"));
                        int destination=Pe32SimdLowering.GprDestination(simdDescriptor);
                        if(destination>=0 && ((simdDescriptor&255)!=25 || (simdDescriptor&0x4000)==0))state.Registers[destination]=null;
                        break;
                    case IlIntrin { Name:"fldcw",Args:[var value] }:
                        _=Eval(value,state,position+"/control"); break;
                    case IlIntrin { Name:"fnstcw-address" or "fnstsw-m-address" or "ldmxcsr-address" or "stmxcsr-address" } intrinsic:
                        foreach(var address in intrinsic.Args) _=Eval(address,state,position+"/fp-address");
                        unrecorded?.Add(new(instructionRva,intrinsic.Name.Contains("mxcsr") ? 32 : 16,
                            intrinsic.Name=="ldmxcsr-address" ? "read" : "write","explicit floating-point environment access")); break;
                    case IlIntrin { Name:"fnstsw-ax",Args:[] }:
                        state.Registers[0]=null; break;
                    case IlFault { Architecture:"x86",Vector:>=0 and <=255 }:
                        // This path terminates at runtime. Keeping its incoming
                        // facts in a join can only discard facts conservatively;
                        // it must not produce invented memory/register effects.
                        break;
                    case IlIntrin { Name:"pe32-double-shift",Args:[IlConst descriptor,var address,var count] }:
                        _=Eval(address,state,position+"/double-shift-address");_=Eval(count,state,position+"/double-shift-count");
                        uint shiftDescriptor=(uint)descriptor.Bits;int shiftWidth=(shiftDescriptor&2)!=0?16:32;
                        _=Eval(new IlReadReg(IlType.U32,RegKind.X86,(int)((shiftDescriptor>>8)&7)),state,position+"/conditional-source");
                        if((shiftDescriptor&4)!=0) {
                            unrecorded?.Add(new(instructionRva,shiftWidth,"read","double-shift read/modify/write operand"));
                            unrecorded?.Add(new(instructionRva,shiftWidth,"write","double-shift read/modify/write operand"));
                        } else {
                            int target=(int)((shiftDescriptor>>11)&7);
                            _=Eval(new IlReadReg(IlType.U32,RegKind.X86,target),state,position+"/conditional-destination");
                            state.Registers[target]=null;
                        }
                        break;
                    case IlIntrin { Name:"pe32-string-compare",Args:[IlConst descriptor] }:
                        bool scan=((uint)descriptor.Bits&0x100)!=0,repeat=((uint)descriptor.Bits&0x200)!=0;
                        int compareWidth=(int)((uint)descriptor.Bits&255);
                        foreach(int register in new[] {scan?0:6,7})
                            _=Eval(new IlReadReg(IlType.U64,RegKind.X86,register),state,position+"/conditional-input");
                        if(repeat)_=Eval(new IlReadReg(IlType.U64,RegKind.X86,1),state,position+"/conditional-count");
                        unrecorded?.Add(new(instructionRva,compareWidth,"read","conditional CMPS/SCAS destination ES:EDI"));
                        state.Registers[7]=null;
                        if(!scan) {unrecorded?.Add(new(instructionRva,compareWidth,"read","conditional CMPS source DS:ESI"));state.Registers[6]=null;}
                        if(repeat)state.Registers[1]=null;
                        break;
                    case IlIntrin { Name:"movs",Args:[IlConst width] } when width.Bits==8 || width.Bits==16 || width.Bits==32:
                        foreach(int register in new[] {6,7})
                            _=Eval(new IlReadReg(IlType.U64,RegKind.X86,register),state,position+"/input");
                        unrecorded?.Add(new(instructionRva,(int)width.Bits,"read","one-element MOVS source DS:ESI"));
                        unrecorded?.Add(new(instructionRva,(int)width.Bits,"write","one-element MOVS destination ES:EDI"));
                        state.Registers[6]=state.Registers[7]=null;
                        break;
                    case IlIntrin { Name:"rep_movs",Args:[IlConst width] } when width.Bits==8 || width.Bits==16 || width.Bits==32:
                        // Dynamic copies invalidate these affine roots. No
                        // inferred layout or copied-field facts are invented.
                        foreach(int register in new[] {1,6,7})
                            _=Eval(new IlReadReg(IlType.U64,RegKind.X86,register),state,position+"/conditional-input");
                        unrecorded?.Add(new(instructionRva,(int)width.Bits,"read","dynamic REP copy; conditional and repeated DS:ESI accesses"));
                        unrecorded?.Add(new(instructionRva,(int)width.Bits,"write","dynamic REP copy; conditional and repeated ES:EDI accesses"));
                        state.Registers[1]=state.Registers[6]=state.Registers[7]=null;
                        break;
                    case IlIntrin { Name:"stos",Args:[IlConst width] } when width.Bits==32:
                        foreach(int register in new[] {0,7})
                            _=Eval(new IlReadReg(IlType.U64,RegKind.X86,register),state,position+"/input");
                        unrecorded?.Add(new(instructionRva,32,"write","implicit STOSD destination ES:EDI"));
                        state.Registers[7]=null;
                        break;
                    case IlIntrin { Name:"rep_stos",Args:[IlConst width] } when width.Bits==8 || width.Bits==16 || width.Bits==32:
                        foreach(int register in new[] {0,1,7})
                            _=Eval(new IlReadReg(IlType.U64,RegKind.X86,register),state,position+"/conditional-input");
                        unrecorded?.Add(new(instructionRva,(int)width.Bits,"write","dynamic REP store; conditional and repeated ES:EDI accesses"));
                        state.Registers[1]=state.Registers[7]=null;
                        break;
                    default: throw new NotSupportedException("Unsupported statement in guest32 structure analysis: "+statements[index]);
                }
            }
        }
        Flow Transfer(ulong block,Affine?[] initial) {
            var state=new Flow((Affine?[])initial.Clone());
            foreach(var instruction in graph.Blocks[block].Instructions) {
                instructionRva=checked(instruction.Address-program.Image.ImageBase);
                var ids=new Dictionary<string,int>();
                string normalized=Regex.Replace(instruction.Body.ToString(),@"%([0-9]+)",m=> {
                    string key=m.Groups[1].Value;
                    if(!ids.ContainsKey(key)) ids[key]=ids.Count;
                    return "%"+ids[key];
                });
                instructionOrigin=Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
                state.Temps.Clear(); Statements(instruction.Body.Body,state,"");
            }
            // Until callee preservation/cleanup contracts are established, a
            // return continuation gets no invented surviving register facts.
            if(graph.Blocks[block].Kind==TerminatorKind.Call) Array.Clear(state.Registers);
            return state;
        }
        while(queue.TryDequeue(out var block)) {
            if(--budget<0) throw new NotSupportedException("Guest32 structure dataflow budget exhausted");
            queued.Remove(block);
            var output=Transfer(block,inputs[block]).Registers;
            var terminal=graph.Blocks[block];
            var successors=terminal.Kind==TerminatorKind.Jump && terminal.Branch!=null &&
                ControlFlow.ConstantTarget(terminal.Instructions[^1].Body,terminal.Branch)==null ? [] : terminal.FlowSuccessors;
            foreach(ulong target in successors) {
                if(!graph.Blocks.ContainsKey(target)) continue; // Explicit runtime boundary, no local facts beyond it.
                bool changed=false;
                if(!inputs.TryGetValue(target,out var previous)) {
                    inputs[target]=(Affine?[])output.Clone(); changed=true;
                } else for(int i=0;i<16;i++) {
                    var next=previous[i]==output[i] ? previous[i] : null;
                    if(next!=previous[i]) { previous[i]=next; changed=true; }
                }
                if(changed && queued.Add(target)) queue.Enqueue(target);
            }
        }
        recorded=[]; unrecorded=[];
        foreach(var (address,input) in inputs.OrderBy(p=>p.Key)) _=Transfer(address,input);
        foreach(var access in recorded.Where(a=>accessFacts.GetValueOrDefault(a.Node)!=a.Address))
            unrecorded.Add(new(access.InstructionRva,access.Width,access.Write ? "write" : "read","conflicting shared-node address facts"));
        recorded=recorded.Where(a=>accessFacts.GetValueOrDefault(a.Node)==a.Address).ToList();
        string function=$"{program.Entry-program.Image.ImageBase:x8}";
        string ObjectOrigin(string root) => $"guest32-object-v1:{function}:{root}";
        string FieldOrigin(Access access) => $"guest32-field-v1:{function}:{access.Address.Root}:{access.Address.Offset:x8}:{access.Width}";
        var objects=new List<Pe32Object>();
        var mapping=new Dictionary<Il,(Pe32Object,Pe32Field)>(ReferenceEqualityComparer.Instance);
        foreach(var group in recorded.GroupBy(a=>a.Address.Root!).OrderBy(g=>g.Key,StringComparer.Ordinal)) {
            string origin=ObjectOrigin(group.Key);
            var annotation=annotations?.GetValueOrDefault(origin);
            string name=annotation?.Name ?? $"object_{objects.Count}";
            if(!Identifier(name) || objects.Any(o=>o.Name==name)) throw new ArgumentException("Invalid or duplicate structure name: "+name);
            var fields=new List<Pe32Field>();
            var spans=group.GroupBy(a=>(a.Address.Offset,a.Width)).OrderBy(g=>g.Key.Offset).ThenBy(g=>g.Key.Width).ToArray();
            foreach(var span in spans) {
                string fieldOrigin=FieldOrigin(span.First());
                var fieldAnnotation=annotation?.Fields.GetValueOrDefault(fieldOrigin);
                string fieldName=fieldAnnotation?.Name ?? $"field_{span.Key.Offset:x8}_u{span.Key.Width}";
                string type=fieldAnnotation?.Type ?? (span.Key.Width==128 ? "bits" : $"u{span.Key.Width}");
                if(!Identifier(fieldName) || fields.Any(f=>f.Name==fieldName)) throw new ArgumentException("Invalid or duplicate field name: "+fieldName);
                if(type!="bits" && type!=$"u{span.Key.Width}") throw new NotSupportedException("Field types must preserve the observed storage width");
                bool overlaps=spans.Any(other=>other.Key!=span.Key &&
                    (unchecked(other.Key.Offset-span.Key.Offset)<span.Key.Width/8 || unchecked(span.Key.Offset-other.Key.Offset)<other.Key.Width/8));
                var targets=roots.Values.Where(r=>r.Source!=null && span.Any(a=>ReferenceEquals(a.Node,r.Source)))
                    .Select(r=>ObjectOrigin(r.Origin)).Where(o=>recorded.Any(a=>ObjectOrigin(a.Address.Root!)==o)).Distinct().Order().ToArray();
                fields.Add(new(fieldOrigin,fieldName,unchecked((int)span.Key.Offset),span.Key.Width,type,
                    span.Any(a=>!a.Write),span.Any(a=>a.Write),overlaps,
                    span.Select(a=>new Pe32FieldSite(a.InstructionRva,a.Write ? "write" : "read")).Distinct().OrderBy(s=>s.InstructionRva).ThenBy(s=>s.Mode).ToArray(),targets));
            }
            if(annotation!=null && annotation.Fields.Keys.Any(k=>!fields.Any(f=>f.Origin==k)))
                throw new NotSupportedException("Unmatched field annotations; review changed field origins");
            var root=roots[group.Key];
            string? parent=root.Source!=null ? recorded.Where(a=>ReferenceEquals(a.Node,root.Source)).Select(FieldOrigin).Distinct().SingleOrDefault() : null;
            var obj=new Pe32Object(origin,name,group.Key,root.EntryRegister,parent,fields);
            objects.Add(obj);
            foreach(var access in group)
                if(accessFacts.GetValueOrDefault(access.Node)==access.Address)
                    mapping.TryAdd(access.Node,(obj,fields.Single(f=>f.Origin==FieldOrigin(access))));
        }
        if(annotations!=null && annotations.Keys.Any(k=>!objects.Any(o=>o.Origin==k)))
            throw new NotSupportedException("Unmatched structure annotations; review changed base origins");
        return new(objects,unrecorded,mapping,stackAccesses.Distinct().OrderBy(a=>a.InstructionRva).ThenBy(a=>a.Offset).ToArray(),entryReads.Order().ToArray());
    }
}
