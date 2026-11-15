#pragma once
#include "IntegerRuntime.hpp"
#include <string>
#include <utility>
#include <array>
#include <atomic>
#include <bit>
namespace sharp_retro {
#ifdef SHARP_RETRO_EXTERNAL_UNKNOWN_ORIGIN
// Candidate DLLs use the running process's identity allocator. A private counter
// would falsely correlate unrelated unknown bits after crossing a module boundary.
std::uint64_t external_unknown_origin();
inline std::uint64_t fresh_unknown_origin() { return external_unknown_origin(); }
#else
inline std::uint64_t fresh_unknown_origin() {
    static std::atomic<std::uint64_t> next{1};
    auto result=next.fetch_add(1,std::memory_order_relaxed);
    if(!result) throw std::runtime_error("unknown-value identity exhausted");
    return result;
}
#endif
struct Scalar32 {
    std::uint32_t value=0,known=0;
    // Independent stable identities for unknown bits, optionally XORed by the
    // payload. Zero grants no correlation. Copies retain identities.
    std::array<std::uint64_t,32> origins{};
};
struct GuestCpuFault : std::runtime_error {
    std::string architecture;
    int vector;
    std::uint64_t pc;
    GuestCpuFault(std::string architecture,int vector,std::uint64_t pc)
        : std::runtime_error("guest "+architecture+" fault vector "+std::to_string(vector)+" at PC "+std::to_string(pc)),
          architecture(std::move(architecture)),vector(vector),pc(pc) {}
};
struct State {
    std::uint64_t gpr[16]{};
    std::uint64_t pc = 0;
    std::uint64_t flags = 2;
    std::uint64_t defined_flags = ~std::uint64_t(0);
    std::uint64_t segments[6]{}; // supplied segment bases, not selectors
    std::uint16_t defined_gpr = 0xffff;
    // Used only while the whole-register bit is clear. Known subregister bits
    // never make the untouched portion of a native-clobbered register defined.
    std::uint64_t partial_gpr[16]{};
    std::array<std::uint64_t,32> gpr_origins[16]{},flag_origins{};
    // No process-entry floating-point values are guessed. Explicit guest
    // instructions establish each part before it can be observed.
    std::uint16_t x87_control=0,x87_status=0,x87_tag=0;
    std::uint16_t x87_control_defined=0,x87_status_defined=0,x87_tag_defined=0;
    std::uint32_t x87_ip=0,x87_dp=0;
    std::uint16_t x87_cs=0,x87_ds=0,x87_opcode=0;
    bool x87_environment_defined=false;
    // Raw 80-bit values in logical ST(0)..ST(7) order, matching FSAVE data.
    // Empty tagged registers can still contain unknown physical bits.
    std::array<std::array<std::uint8_t,10>,8> x87_st{};
    std::uint8_t defined_x87_st=0;
    std::uint32_t mxcsr=0,mxcsr_defined=0,mxcsr_mask=0;
    // Physical MMX observations are separate only on a measured host where
    // x87 save/restore does not carry MMX state (Wine/Rosetta). Architectural
    // x86 profiles continue to use the aliased x87 physical payloads.
    std::array<std::uint64_t,8> native_mmx{};
    std::uint8_t defined_native_mmx=0;
    bits xmm[16]{};
    std::uint16_t defined_xmm=0;
};
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
// The native integration supplies native_cpu.cpp; strict validation stays inline.
void native_record_flag_origin(std::uint64_t,unsigned,std::uint64_t);
std::uint64_t native_flag_origin_pc(std::uint64_t,unsigned);
void native_require_observed_alu_flags(const State&);
std::uint32_t native_alu_flag_bits(unsigned,unsigned,std::uint64_t,std::uint64_t,std::uint32_t);
#endif
inline std::uint64_t fresh_unknown_flag_origin(const State& s,unsigned bit) {
    auto origin=fresh_unknown_origin();
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
    native_record_flag_origin(origin,bit,s.pc);
#else
    (void)s;(void)bit;
#endif
    return origin;
}
inline std::uint64_t native_undefined_flag_pc(const State& s,unsigned bit) {
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
    return bit<32 ? native_flag_origin_pc(s.flag_origins[bit],bit):0;
#else
    (void)s;(void)bit;return 0;
#endif
}
inline void native_require_alu_flags(const State& s) {
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
    native_require_observed_alu_flags(s);
#else
    (void)s;
#endif
}
inline bits read_xmm128(const State& s,unsigned index) {
    if(index>=16 || !(s.defined_xmm&(1u<<index))) throw std::runtime_error("unknown XMM register value");
    return s.xmm[index];
}
inline void x87_fnclex(State& s) {
    constexpr std::uint16_t cleared=0x80ff; // IE..PE, SF, ES, B; retain C0..C3 and TOP.
    s.x87_status&=std::uint16_t(~cleared); s.x87_status_defined|=cleared;
}
inline void x87_wait(const State& s,std::uint64_t pc) {
    bool unknown=false;
    for(unsigned bit=0;bit<6;bit++) {
        auto mask=std::uint16_t(1u<<bit);
        if((s.x87_control_defined&mask) && (s.x87_control&mask)) continue;
        if((s.x87_status_defined&mask) && !(s.x87_status&mask)) continue;
        if((s.x87_control_defined&mask) && (s.x87_status_defined&mask)) throw GuestCpuFault("x86",16,pc);
        unknown=true;
    }
    if(unknown) throw std::runtime_error("unknown x87 pending exception state");
}
inline std::uint16_t x87_control(const State& s) {
    if(s.x87_control_defined!=0xffff) throw std::runtime_error("unknown x87 control word");
    return s.x87_control;
}
inline std::uint16_t x87_status(const State& s) {
    if(s.x87_status_defined!=0xffff) throw std::runtime_error("unknown x87 status word");
    return s.x87_status;
}
inline void x87_fldcw(State& s,bits value) {
    s.x87_control=std::uint16_t((std::uint16_t(value)&0x1f3f)|0x40); s.x87_control_defined=0xffff;
}
inline void x87_fninit(State& s) {
    if((s.x87_status_defined&0x3800)==0x3800) {
        auto old=s.x87_st; auto known=s.defined_x87_st; auto top=(s.x87_status>>11)&7;
        s.defined_x87_st=0;
        for(unsigned i=0;i<8;i++) {
            auto source=(i+8-top)&7; s.x87_st[i]=old[source];
            if(known&(1u<<source)) s.defined_x87_st|=std::uint8_t(1u<<i);
        }
    } else s.defined_x87_st=0;
    s.x87_control=0x037f; s.x87_status=0; s.x87_tag=0xffff;
    s.x87_control_defined=s.x87_status_defined=s.x87_tag_defined=0xffff;
    s.x87_ip=s.x87_dp=0; s.x87_cs=s.x87_ds=s.x87_opcode=0; s.x87_environment_defined=true;
}
inline std::uint32_t read_mxcsr(const State& s) {
    if(s.mxcsr_defined!=0xffffffffu) throw std::runtime_error("unknown MXCSR state");
    return s.mxcsr;
}
inline void write_mxcsr(State& s,bits value,std::uint64_t pc) {
    if(!s.mxcsr_mask) throw std::runtime_error("unknown supported MXCSR mask");
    auto word=std::uint32_t(value);
    if(word&~s.mxcsr_mask) throw GuestCpuFault("x86",13,pc);
    s.mxcsr=word; s.mxcsr_defined=0xffffffffu;
}
inline bits read_gpr_mask(const State& s,unsigned index,std::uint64_t mask) {
    if(index>=16 || ((s.defined_gpr & (std::uint16_t(1) << index))==0 && (s.partial_gpr[index]&mask)!=mask))
        throw std::runtime_error("read of undefined guest register after external call; register "+std::to_string(index)+" mask "+std::to_string(mask));
    return s.gpr[index]&mask;
}
inline bits read_gpr(const State& s,unsigned index) { return read_gpr_mask(s,index,~std::uint64_t(0)); }
inline void invalidate_gprs(State& s,std::uint16_t mask) {
    s.defined_gpr&=~mask;
    for(unsigned i=0;i<16;i++) if(mask&(std::uint16_t(1)<<i)) { s.partial_gpr[i]=0; s.gpr_origins[i]={}; }
}
inline void write_gpr_slice(State& s,unsigned index,bits value,std::uint64_t mask) {
    if(index>=16) throw std::runtime_error("invalid partial guest register");
    auto bit=std::uint16_t(1)<<index;
    auto known=(s.defined_gpr&bit) ? ~std::uint64_t(0) : s.partial_gpr[index];
    s.gpr[index]=(s.gpr[index]&~mask)|(std::uint64_t(value)&mask); known|=mask;
    if(known==~std::uint64_t(0)) { s.defined_gpr|=bit; s.partial_gpr[index]=0; }
    else { s.defined_gpr&=~bit; s.partial_gpr[index]=known; }
}
inline bits read_flags(const State& s, int bit) {
    std::uint64_t selected = bit < 0 ? ~std::uint64_t(0) : std::uint64_t(1) << bit;
    if((s.defined_flags & selected) != selected)
        throw std::runtime_error("read of architecturally undefined flags");
    return bit < 0 ? s.flags : (s.flags >> bit) & 1;
}
inline void write_flag(State& s, int bit, bits value) {
    if(bit < 0) { s.flags = std::uint64_t(value) | 2; s.defined_flags = ~std::uint64_t(0); }
    else {
        std::uint64_t selected = std::uint64_t(1) << bit;
        s.flags = (s.flags & ~selected) | (std::uint64_t(value & 1) << bit);
        s.defined_flags |= selected;
    }
}
struct UnknownMemoryRead : std::runtime_error {
    Scalar32 scalar32{};
    UnknownMemoryRead() : std::runtime_error("read of unknown native-clobbered memory") {}
    explicit UnknownMemoryRead(Scalar32 value) : UnknownMemoryRead() { scalar32=value; }
};
struct MemoryValue { bits value; bool defined; };
enum class X87Op { fld1,fldz,fld32,fld64,fstp32,fstp64,fild32,fsub64,fmul64,fdiv64,fadd64,fst32,fstp0,fstp1,fdivr64,
    fxch0,fxch1,fxch2,fxch3,fxch4,fxch5,fxch6,fxch7,fst64,fld80,fsqrt,fmul32,
    fmul_st0_0,fmul_st0_1,fmul_st0_2,fmul_st0_3,fmul_st0_4,fmul_st0_5,fmul_st0_6,fmul_st0_7,
    fmul_sti_0,fmul_sti_1,fmul_sti_2,fmul_sti_3,fmul_sti_4,fmul_sti_5,fmul_sti_6,fmul_sti_7,fsincos,
    fld_st0,fld_st1,fld_st2,fld_st3,fld_st4,fld_st5,fld_st6,fld_st7,
    fmulp_st0,fmulp_st1,fmulp_st2,fmulp_st3,fmulp_st4,fmulp_st5,fmulp_st6,fmulp_st7,
    faddp_st0,faddp_st1,faddp_st2,faddp_st3,faddp_st4,faddp_st5,faddp_st6,faddp_st7,
    fsubp_st0,fsubp_st1,fsubp_st2,fsubp_st3,fsubp_st4,fsubp_st5,fsubp_st6,fsubp_st7,
    fcom_st0,fcom_st1,fcom_st2,fcom_st3,fcom_st4,fcom_st5,fcom_st6,fcom_st7,
    fcomp_st0,fcomp_st1,fcomp_st2,fcomp_st3,fcomp_st4,fcomp_st5,fcomp_st6,fcomp_st7,
    fdiv_sti_0,fdiv_sti_1,fdiv_sti_2,fdiv_sti_3,fdiv_sti_4,fdiv_sti_5,fdiv_sti_6,fdiv_sti_7,fchs,
    fdiv_st0_0,fdiv_st0_1,fdiv_st0_2,fdiv_st0_3,fdiv_st0_4,fdiv_st0_5,fdiv_st0_6,fdiv_st0_7,
    fdivr_st0_0,fdivr_st0_1,fdivr_st0_2,fdivr_st0_3,fdivr_st0_4,fdivr_st0_5,fdivr_st0_6,fdivr_st0_7,
    fdivr_sti_0,fdivr_sti_1,fdivr_sti_2,fdivr_sti_3,fdivr_sti_4,fdivr_sti_5,fdivr_sti_6,fdivr_sti_7,
    fcom32,fcom64,fcomp32,fcomp64 };
struct Memory {
    virtual bits load(std::uint64_t address, unsigned width) = 0;
    virtual void store(std::uint64_t address, bits value, unsigned width) = 0;
    virtual void store_unknown(std::uint64_t, bits, unsigned) {
        throw std::runtime_error("memory backend cannot retain unknown native-clobbered storage");
    }
    virtual MemoryValue load_defined(std::uint64_t address,unsigned width) {
        try { return {load(address,width),true}; }
        // Loads validate the entire address/permissions before reporting unknown
        // bytes. Carry unknownness through register restores; the zero payload
        // is never a defined guest value and cannot be consumed as one.
        catch(const UnknownMemoryRead&) { return {0,false}; }
    }
    virtual bool invoke_import(const char*, State&) { return false; }
    virtual Scalar32 load_scalar8(std::uint64_t address) {
        try { return {std::uint8_t(load(address,8)),0xffffffffu,{}}; }
        catch(const UnknownMemoryRead& error) {
            auto value=error.scalar32; value.value&=0xffu; value.known|=0xffffff00u; return value;
        }
    }
    virtual void store_scalar8(std::uint64_t address,Scalar32 value) {
        if((value.known&0xffu)==0xffu) store(address,value.value,8);
        else store_unknown(address,value.value,8);
    }
    virtual Scalar32 load_scalar16(std::uint64_t address) {
        try { return {std::uint16_t(load(address,16)),0xffffffffu,{}}; }
        catch(const UnknownMemoryRead& error) {
            auto value=error.scalar32; value.value&=0xffffu; value.known|=0xffff0000u; return value;
        }
    }
    virtual void store_scalar16(std::uint64_t address,Scalar32 value) {
        if((value.known&0xffffu)==0xffffu) store(address,value.value,16);
        else store_unknown(address,value.value,16);
    }
    virtual Scalar32 load_scalar32(std::uint64_t address) {
        auto value=load_defined(address,32);
        return {std::uint32_t(value.value),value.defined ? 0xffffffffu : 0u,{}};
    }
    virtual void store_scalar32(std::uint64_t address,Scalar32 value) {
        if(value.known==0xffffffffu) store(address,value.value,32);
        else store_unknown(address,value.value,32);
    }
    virtual std::array<std::uint32_t,4> cpuid(std::uint32_t,std::uint32_t) {
        throw std::runtime_error("CPU identity provider is unimplemented");
    }
    virtual void validate_store(std::uint64_t,unsigned) {
        throw std::runtime_error("memory backend cannot preflight x87 stores");
    }
    virtual void execute_x87(X87Op,State&,std::uint64_t,std::uint32_t,std::uint16_t) {
        throw std::runtime_error("x87 value execution provider is unimplemented");
    }
    virtual ~Memory() = default;
};
inline bits read_for_zero_and32(Memory& memory,std::uint64_t address) {
    // AND still performs the read (including permission/address faults). Its
    // unknown payload cannot influence a result masked entirely to zero.
    try { (void)memory.load(address,32); } catch(const UnknownMemoryRead&) {}
    return 0;
}
inline Scalar32 scalar_gpr32(State& s,unsigned reg) {
    auto known=(s.defined_gpr&(1u<<reg)) ? 0xffffffffu : std::uint32_t(s.partial_gpr[reg]);
    for(unsigned bit=0;bit<32;bit++) if(!(known&(1u<<bit)) && !s.gpr_origins[reg][bit]) s.gpr_origins[reg][bit]=fresh_unknown_origin();
    return {std::uint32_t(s.gpr[reg]),known,s.gpr_origins[reg]};
}
inline void scalar_gpr32(State& s,unsigned reg,Scalar32 value) {
    s.gpr[reg]=value.value; s.gpr_origins[reg]=value.origins;
    if(value.known==0xffffffffu) { s.defined_gpr|=std::uint16_t(1u<<reg); s.partial_gpr[reg]=0; }
    else { s.defined_gpr&=~std::uint16_t(1u<<reg); s.partial_gpr[reg]=0xffffffff00000000ULL|value.known; }
}
inline void push_scalar32(State& s,Memory& memory,Scalar32 value) {
    auto sp=std::uint32_t(read_gpr(s,4))-4u;
    memory.store_scalar32(s.segments[2]+sp,value); s.gpr[4]=sp;
}
inline Scalar32 pop_scalar32(State& s,Memory& memory) {
    auto sp=std::uint32_t(read_gpr(s,4)); auto value=memory.load_scalar32(s.segments[2]+sp);
    s.gpr[4]=std::uint32_t(sp+4u); return value;
}
inline void write_scalar_gpr16(State& s,unsigned reg,const Scalar32& value) {
    auto bit=std::uint16_t(1u<<reg);
    auto known=(s.defined_gpr&bit) ? ~std::uint64_t(0) : s.partial_gpr[reg];
    s.gpr[reg]=(s.gpr[reg]&~std::uint64_t(0xffff))|(value.value&0xffffu);
    known=(known&~std::uint64_t(0xffff))|(value.known&0xffffu);
    for(unsigned i=0;i<16;i++) s.gpr_origins[reg][i]=value.origins[i];
    if(known==~std::uint64_t(0)) { s.defined_gpr|=bit; s.partial_gpr[reg]=0; }
    else { s.defined_gpr&=~bit; s.partial_gpr[reg]=known; }
}
inline void push_scalar16(State& s,Memory& memory,Scalar32 value) {
    auto sp=std::uint32_t(read_gpr_mask(s,4,0xffffffffu))-2u;
    memory.store_scalar16(s.segments[2]+sp,value);
    scalar_gpr32(s,4,{sp,0xffffffffu,{}});
}
inline Scalar32 pop_scalar16(State& s,Memory& memory) {
    auto sp=std::uint32_t(read_gpr_mask(s,4,0xffffffffu)); auto value=memory.load_scalar16(s.segments[2]+sp);
    scalar_gpr32(s,4,{std::uint32_t(sp+2u),0xffffffffu,{}}); return value;
}
inline void pushfw32(State& s,Memory& memory) {
    constexpr std::uint32_t mask=0x7fd5;
    auto known=std::uint32_t(s.defined_flags)|~mask;
    for(unsigned bit=0;bit<16;bit++) if(!(known&(1u<<bit)) && !s.flag_origins[bit]) s.flag_origins[bit]=fresh_unknown_flag_origin(s,bit);
    push_scalar16(s,memory,{(std::uint32_t(s.flags)&mask)|2u,known,s.flag_origins});
}
inline void popfw32(State& s,Memory& memory) {
    auto value=pop_scalar16(s,memory);
    constexpr std::uint32_t writable=0x4dd5; // CPL3; high EFLAGS, including ID, are untouched.
    s.flags=(s.flags&~std::uint64_t(writable|0x10000u))|(value.value&writable)|2u;
    s.defined_flags=(s.defined_flags&~std::uint64_t(writable))|(value.known&writable)|0x10002u;
    for(unsigned bit=0;bit<16;bit++) if(writable&(1u<<bit)) s.flag_origins[bit]=value.origins[bit];
}
inline void pusha16_32(State& s,Memory& memory) {
    (void)read_gpr_mask(s,4,0xffffffffu);
    auto original_sp=scalar_gpr32(s,4);
    // This backend explores mapped, flat protected-mode stacks. Preflight the
    // complete frame; stack-fault delivery/restart is not implemented.
    auto bottom=std::uint32_t(original_sp.value-16u);
    memory.validate_store(s.segments[2]+bottom,128);
    for(unsigned reg:{0u,1u,2u,3u,4u,5u,6u,7u})
        push_scalar16(s,memory,reg==4 ? original_sp : scalar_gpr32(s,reg));
}
inline void popa16_32(State& s,Memory& memory) {
    for(unsigned reg:{7u,6u,5u,4u,3u,2u,1u,0u}) {
        if(reg==4) { // The saved SP slot is skipped, not loaded into SP.
            auto sp=std::uint32_t(read_gpr_mask(s,4,0xffffffffu)); scalar_gpr32(s,4,{std::uint32_t(sp+2u),0xffffffffu,{}});
        } else write_scalar_gpr16(s,reg,pop_scalar16(s,memory));
    }
}
inline void pusha32_32(State& s,Memory& memory) {
    auto sp=std::uint32_t(read_gpr_mask(s,4,0xffffffffu)); auto original_sp=scalar_gpr32(s,4);
    memory.validate_store(s.segments[2]+std::uint32_t(sp-32u),128);
    memory.validate_store(s.segments[2]+std::uint32_t(sp-16u),128);
    for(unsigned reg:{0u,1u,2u,3u,4u,5u,6u,7u}) push_scalar32(s,memory,reg==4 ? original_sp : scalar_gpr32(s,reg));
}
inline void popa32_32(State& s,Memory& memory) {
    for(unsigned reg:{7u,6u,5u,4u,3u,2u,1u,0u}) {
        if(reg==4) {
            auto sp=std::uint32_t(read_gpr_mask(s,4,0xffffffffu)); scalar_gpr32(s,4,{std::uint32_t(sp+4u),0xffffffffu,{}});
        } else scalar_gpr32(s,reg,pop_scalar32(s,memory));
    }
}
inline void pushfd32(State& s,Memory& memory) {
    // PUSHFD clears RF and VM in its saved image. Other undefined bits remain
    // symbolic; no choice for an arithmetic flag becomes an observed value.
    constexpr std::uint32_t mask=0x003c7fd5u;
    auto known=std::uint32_t(s.defined_flags)|~mask;
    for(unsigned bit=0;bit<32;bit++) if(!(known&(1u<<bit)) && !s.flag_origins[bit]) s.flag_origins[bit]=fresh_unknown_flag_origin(s,bit);
    push_scalar32(s,memory,{(std::uint32_t(s.flags)&mask)|2u,known,s.flag_origins});
}
inline void popfd32(State& s,Memory& memory) {
    auto value=pop_scalar32(s,memory);
    // Windows user mode: IF/IOPL/VIP/VIF/VM cannot be changed by POPFD; RF
    // clears. This runtime still doesn't deliver debug/trap exceptions.
    constexpr std::uint32_t writable=0x00244dd5u;
    s.flags=(s.flags&~std::uint64_t(writable|0x10000u))|(value.value&writable)|2u;
    s.defined_flags=(s.defined_flags&~std::uint64_t(writable))|(value.known&writable)|0x10002u;
    for(unsigned bit=0;bit<32;bit++) if(writable&(1u<<bit)) s.flag_origins[bit]=value.origins[bit];
}
inline void scalar_logic_flags32(State& s,Scalar32 value) {
    s.flags&=~std::uint64_t(0x8d5); s.defined_flags=(s.defined_flags&~std::uint64_t(0x8d5))|0x801u;
    if(value.known&0x80000000u) { s.flags|=(value.value>>24)&0x80u; s.defined_flags|=0x80; }
    if(value.value&value.known) { s.defined_flags|=0x40; }
    else if(value.known==0xffffffffu) { s.flags|=0x40; s.defined_flags|=0x40; }
    if((value.known&0xff)==0xff) { s.flags|=(std::popcount(value.value&0xffu)%2==0) ? 4u : 0u; s.defined_flags|=4; }
    for(unsigned bit=0;bit<32;bit++) if((0x8d5u&(1u<<bit)) && !(s.defined_flags&(1u<<bit))) s.flag_origins[bit]=fresh_unknown_flag_origin(s,bit);
}
// In the explicit native x86 execution profile, concrete XOR operands can be
// executed on the CPU to observe its complete arithmetic flags, including AF
// (architecturally undefined). The symbolic validation profile keeps AF unknown.
// Never apply this to symbolic representative bits.
inline void native_logic_flags(State& s,unsigned operation,unsigned width,std::uint64_t left,std::uint64_t right,
                               std::uint64_t input_flags,std::uint64_t input_known) {
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
    if(operation>=4 && operation<=6 && !(right&31u)) return; // A zero shift count preserves all flags.
    if((input_known&0x8d5u)!=0x8d5u) throw std::runtime_error("native ALU input flags have no concrete CPU observation");
    auto seed=std::uint32_t(input_flags&0x8d5u)|0x202u;
    auto flags=native_alu_flag_bits(operation,width,left,right,seed);
    s.flags=(s.flags&~std::uint64_t(0x8d5))|(flags&0x8d5u);
    s.defined_flags|=0x8d5u;
    for(unsigned bit=0;bit<32;++bit) if(0x8d5u&(1u<<bit)) s.flag_origins[bit]=0;
#else
    (void)s;(void)operation;(void)width;(void)left;(void)right;(void)input_flags;(void)input_known;
#endif
}
inline void native_logic_flags(State& s,unsigned operation,unsigned width,std::uint64_t left,std::uint64_t right) {
    native_logic_flags(s,operation,width,left,right,s.flags,s.defined_flags);
}
inline void xor_scalar32(State& s,unsigned reg,std::uint32_t constant) {
    auto input_flags=s.flags,input_known=s.defined_flags;
    auto value=scalar_gpr32(s,reg); auto original=value.value; value.value^=constant;
    if(value.known==0xffffffffu) native_require_alu_flags(s);
    scalar_gpr32(s,reg,value); scalar_logic_flags32(s,value);
    if(value.known==0xffffffffu) native_logic_flags(s,2,32,original,constant,input_flags,input_known);
}
inline void xor_gprs32(State& s,unsigned destination,unsigned source) {
    auto input_flags=s.flags,input_known=s.defined_flags;
    auto a=scalar_gpr32(s,destination),b=scalar_gpr32(s,source);
    if(a.known==0xffffffffu && b.known==0xffffffffu) native_require_alu_flags(s);
    Scalar32 output{a.value^b.value,0,{}};
    for(unsigned bit=0;bit<32;bit++) {
        auto mask=std::uint32_t(1u<<bit); bool ak=(a.known&mask)!=0,bk=(b.known&mask)!=0;
        if((ak && bk) || (!ak && !bk && a.origins[bit] && a.origins[bit]==b.origins[bit])) output.known|=mask;
        else if(ak) output.origins[bit]=b.origins[bit];
        else if(bk) output.origins[bit]=a.origins[bit];
        else output.origins[bit]=fresh_unknown_origin();
    }
    scalar_gpr32(s,destination,output); scalar_logic_flags32(s,output);
    if(a.known==0xffffffffu && b.known==0xffffffffu) native_logic_flags(s,2,32,a.value,b.value,input_flags,input_known);
}
// 30 /r: XOR r/m8,r8. High-byte register encodings select AH/CH/DH/BH.
// Unknown payload bits are representatives, never defined arithmetic inputs.
inline void xor_memory_gpr8(State& s,Memory& memory,std::uint64_t address,unsigned encoded_source) {
    if(encoded_source>=8) throw std::runtime_error("invalid x86 byte register");
    auto input_flags=s.flags,input_known=s.defined_flags;
    auto a=memory.load_scalar8(address),b=scalar_gpr32(s,encoded_source&3u);
    unsigned shift=encoded_source<4 ? 0u:8u;
    Scalar32 output{(a.value^(b.value>>shift))&0xffu,0xffffff00u,{}};
    for(unsigned bit=0;bit<8;bit++) {
        auto mask=1u<<bit; bool ak=(a.known&mask)!=0,bk=(b.known&(mask<<shift))!=0;
        auto ao=a.origins[bit],bo=b.origins[bit+shift];
        if((ak && bk) || (!ak && !bk && ao && ao==bo)) output.known|=mask;
        else if(ak) output.origins[bit]=bo;
        else if(bk) output.origins[bit]=ao;
        else output.origins[bit]=fresh_unknown_origin();
    }
    // A faulting read/modify/write leaves arithmetic flags unchanged.
    memory.store_scalar8(address,output);
    constexpr std::uint32_t affected=0x8d5u;
    s.flags&=~std::uint64_t(affected); s.defined_flags=(s.defined_flags&~std::uint64_t(affected))|0x801u;
    if(output.known&0x80u) { s.flags|=output.value&0x80u; s.defined_flags|=0x80u; }
    if(output.value&output.known&0xffu) s.defined_flags|=0x40u;
    else if((output.known&0xffu)==0xffu) { s.flags|=0x40u; s.defined_flags|=0x40u; }
    if((output.known&0xffu)==0xffu) { s.flags|=(std::popcount(output.value&0xffu)%2==0) ? 4u:0u; s.defined_flags|=4u; }
    for(unsigned bit=0;bit<32;bit++) if(affected&(1u<<bit))
        s.flag_origins[bit]=(s.defined_flags&(1u<<bit)) ? 0:fresh_unknown_flag_origin(s,bit);
    if((a.known&0xffu)==0xffu && ((b.known>>shift)&0xffu)==0xffu)
        native_logic_flags(s,2,8,a.value,b.value>>shift,input_flags,input_known);
}
// x-x-CF does not consume the incoming register, even when its value is unknown.
// The exact 32-bit register encoding is checked by the PE32 emitter.
inline void sbb_self32(State& s,unsigned reg) {
    auto borrow=std::uint32_t(read_flags(s,0));
    s.gpr[reg]=std::uint32_t(0u-borrow);
    s.defined_gpr|=std::uint16_t(1u<<reg); s.partial_gpr[reg]=0;
    write_flag(s,0,borrow); write_flag(s,2,1); write_flag(s,4,borrow);
    write_flag(s,6,borrow^1u); write_flag(s,7,borrow); write_flag(s,11,0);
}
inline bool subtract_correlated32(State& s,unsigned destination,unsigned source) {
    auto a=scalar_gpr32(s,destination),b=scalar_gpr32(s,source);
    if(a.known==0xffffffffu && b.known==0xffffffffu) return false;
    if(a.known!=b.known) return false;
    for(unsigned bit=0;bit<32;bit++) if(!(a.known&(1u<<bit)) && (!a.origins[bit] || a.origins[bit]!=b.origins[bit])) return false;
    auto different=a.value^b.value;
    if(!different || (different&(different-1u))) return false;
    // a and b differ in exactly one bit, with identical symbolic bits
    // elsewhere. Their difference is exactly +bit or -bit; it is never zero.
    auto plus=different,minus=std::uint32_t(0u-different);
    bool sign_known=(a.known&different)!=0;
    auto result=sign_known ? ((a.value&different) ? plus : minus) : plus;
    auto known=sign_known ? 0xffffffffu : ~(plus^minus);
    Scalar32 output{result,known,{}};
    for(unsigned bit=0;bit<32;bit++) if(!(known&(1u<<bit))) output.origins[bit]=fresh_unknown_origin();
    scalar_gpr32(s,destination,output);
    auto flags=[&](bool positive) {
        auto value=positive ? plus : minus;
        return std::uint32_t((positive ? 0u : 1u)|((std::popcount(value&0xffu)%2==0) ? 4u : 0u)|
            ((different^value)&0x10u)|((value>>24)&0x80u)|
            (different==0x80000000u && !positive ? 0x800u : 0u));
    };
    auto positive=flags(true),negative=flags(false),selected=sign_known ? ((a.value&different) ? positive : negative) : positive;
    auto flag_known=sign_known ? 0x8d5u : (0x8d5u&~(positive^negative));
    s.flags=(s.flags&~std::uint64_t(0x8d5))|selected;
    s.defined_flags=(s.defined_flags&~std::uint64_t(0x8d5))|flag_known;
    for(unsigned bit=0;bit<32;bit++) if((0x8d5u&(1u<<bit)) && !(s.defined_flags&(1u<<bit))) s.flag_origins[bit]=fresh_unknown_flag_origin(s,bit); return true;
}
enum class Exit { returned, boundary, budget_exhausted, transfer };
inline void stosd32(State& s,Memory& memory) {
    auto destination=std::uint32_t(read_gpr(s,7)),value=std::uint32_t(read_gpr(s,0));
    auto down=(read_flags(s,10)&1)!=0;
    memory.store(s.segments[0]+destination,value,32);
    s.gpr[7]=std::uint32_t(destination+(down ? std::uint32_t(-4) : std::uint32_t(4)));
}
inline bool rep_stosd32(State& s,Memory& memory,std::uint64_t& budget) {
    while(std::uint32_t(read_gpr(s,1))!=0) {
        if(!budget) return false;
        --budget;
        stosd32(s,memory);
        s.gpr[1]=std::uint32_t(s.gpr[1]-1);
    }
    return true;
}
inline bool rep_stosb32(State& s,Memory& memory,std::uint64_t& budget) {
    while(std::uint32_t(read_gpr_mask(s,1,0xffffffffu))!=0) {
        if(!budget) return false;
        --budget;
        auto destination=std::uint32_t(read_gpr_mask(s,7,0xffffffffu));
        auto value=std::uint32_t(read_gpr_mask(s,0,0xffu));
        auto down=(read_flags(s,10)&1)!=0;
        memory.store(s.segments[0]+destination,value,8);
        scalar_gpr32(s,7,{std::uint32_t(destination+(down ? std::uint32_t(-1) : 1u)),0xffffffffu,{}});
        scalar_gpr32(s,1,{std::uint32_t(s.gpr[1]-1u),0xffffffffu,{}});
    }
    return true;
}
inline bool rep_stosw32(State& s,Memory& memory,std::uint64_t& budget) {
    while(std::uint32_t(read_gpr_mask(s,1,0xffffffffu))!=0) {
        if(!budget) return false;
        --budget;
        auto destination=std::uint32_t(read_gpr_mask(s,7,0xffffffffu));
        auto down=(read_flags(s,10)&1)!=0;
        memory.store_scalar16(s.segments[0]+destination,scalar_gpr32(s,0));
        scalar_gpr32(s,7,{std::uint32_t(destination+(down ? std::uint32_t(-2) : 2u)),0xffffffffu,{}});
        scalar_gpr32(s,1,{std::uint32_t(s.gpr[1]-1u),0xffffffffu,{}});
    }
    return true;
}
// One copy in 32-bit address mode; ECX and all flag facts remain untouched.
inline void movs32(State& s,Memory& memory,unsigned width) {
    if(width!=16 && width!=32) throw std::runtime_error("unsupported plain MOVS width");
    auto source=std::uint32_t(read_gpr_mask(s,6,0xffffffffu));
    auto destination=std::uint32_t(read_gpr_mask(s,7,0xffffffffu));
    auto down=(read_flags(s,10)&1)!=0;
    // Capture the complete source element before any overlapping destination
    // write. A read/write fault must leave both indexes unchanged.
    auto value=width==16 ? memory.load_scalar16(s.segments[3]+source) : memory.load_scalar32(s.segments[3]+source);
    if(width==16) memory.store_scalar16(s.segments[0]+destination,value);
    else memory.store_scalar32(s.segments[0]+destination,value);
    auto step=down ? std::uint32_t(0u-width/8) : std::uint32_t(width/8);
    scalar_gpr32(s,6,{std::uint32_t(source+step),0xffffffffu,{}});
    scalar_gpr32(s,7,{std::uint32_t(destination+step),0xffffffffu,{}});
}
// One budget unit per completed copy iteration in addition to instruction entry.
// A fault leaves EIP at the string instruction and retains completed iterations.
// Sequential element copies deliberately preserve overlap and DF behavior.
inline bool rep_movsd32(State& s,Memory& memory,std::uint64_t& budget) {
    while(std::uint32_t(read_gpr(s,1))!=0) {
        if(!budget) return false;
        --budget;
        auto source=std::uint32_t(read_gpr(s,6)),destination=std::uint32_t(read_gpr(s,7));
        auto down=(read_flags(s,10)&1)!=0;
        auto value=memory.load_scalar32(s.segments[3]+source);
        memory.store_scalar32(s.segments[0]+destination,value);
        auto step=down ? std::uint32_t(-4) : std::uint32_t(4);
        s.gpr[6]=std::uint32_t(source+step); s.gpr[7]=std::uint32_t(destination+step);
        s.gpr[1]=std::uint32_t(s.gpr[1]-1);
    }
    return true;
}
} // namespace sharp_retro
