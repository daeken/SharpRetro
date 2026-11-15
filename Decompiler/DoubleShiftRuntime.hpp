#ifndef SHARP_RETRO_DOUBLE_SHIFT_RUNTIME
#define SHARP_RETRO_DOUBLE_SHIFT_RUNTIME
namespace sharp_retro {
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
std::uint32_t native_double_shift_bits(bool,unsigned,std::uint32_t,std::uint32_t,unsigned,std::uint32_t,std::uint32_t&);
template<unsigned Immediate> inline std::uint32_t native_double_shift_immediate(bool right,unsigned width,std::uint32_t left,std::uint32_t source,std::uint32_t seed,std::uint32_t& flags) {
    static_assert(Immediate<=255);
#define DOUBLE_SHIFT_IMMEDIATE(OP,SUFFIX,L,S) asm volatile("pushfl; pushl %4; popfl; " OP SUFFIX " $%c3,%2,%0; pushfl; popl %1; popfl" \
    : "+&r"(L),"=r"(flags) : "r"(S),"i"(Immediate),"r"(seed) : "memory","cc")
    if(width==16) {auto a=std::uint16_t(left),b=std::uint16_t(source);if(right){DOUBLE_SHIFT_IMMEDIATE("shrd","w",a,b);}else{DOUBLE_SHIFT_IMMEDIATE("shld","w",a,b);}return a;}
    if(width!=32)throw std::runtime_error("invalid immediate double-shift width");
    if(right){DOUBLE_SHIFT_IMMEDIATE("shrd","l",left,source);}else{DOUBLE_SHIFT_IMMEDIATE("shld","l",left,source);}return left;
#undef DOUBLE_SHIFT_IMMEDIATE
}
#endif
template<int Immediate=-1> inline void double_shift32(State& s,Memory& memory,unsigned descriptor,std::uint64_t address,unsigned count) {
    static_assert(Immediate>=-1 && Immediate<=255);
    if constexpr(Immediate>=0)count=unsigned(Immediate);
    if(descriptor&~0x3f07u)throw std::runtime_error("invalid double-shift descriptor");
    bool right=descriptor&1u,word=descriptor&2u,mem=descriptor&4u;
    unsigned width=word?16u:32u,source=(descriptor>>8)&7u,destination=(descriptor>>11)&7u;
    const std::uint32_t selected=word?0xffffu:0xffffffffu;count&=31u;
    if(!count) {
        // A masked-zero memory shift still reads and performs a write access.
        // Preserve unknown payloads rather than consuming representative bits.
        if(mem) {
            auto value=word?memory.load_scalar16(address):memory.load_scalar32(address);
            memory.validate_store(address,width);
            if(word)memory.store_scalar16(address,value);else memory.store_scalar32(address,value);
        }
        return;
    }
    auto left=mem?std::uint32_t(memory.load(address,width)):std::uint32_t(read_gpr_mask(s,destination,selected));
    if(mem)memory.validate_store(address,width);
    auto input=std::uint32_t(read_gpr_mask(s,source,selected));std::uint32_t result=0;std::uint64_t flags=0,known=0x8d5u;
#if defined(_WIN32) && defined(__i386__) && defined(RECOMP_NATIVE_EXECUTION)
    native_require_observed_alu_flags(s);std::uint32_t observed=0;
    if constexpr(Immediate>=0) result=native_double_shift_immediate<unsigned(Immediate)>(right,width,left,input,std::uint32_t(s.flags&0x8d5u)|0x202u,observed);
    else result=native_double_shift_bits(right,width,left,input,count,std::uint32_t(s.flags&0x8d5u)|0x202u,observed);
    flags=observed&0x8d5u;
#else
    if(count>width)throw std::runtime_error("architecturally undefined double-shift result requires native observation");
    result=std::uint32_t(right?((std::uint64_t(input)<<width)|left)>>count:
        (std::uint64_t(left)<<count)|(std::uint64_t(input)>>(width-count)))&selected;
    auto carry=right?((left>>(count-1))&1u):((left>>(width-count))&1u);
    flags=carry|((std::popcount(result&255u)%2==0)?4u:0u)|(result==0?64u:0u)|((result>>(width-8))&128u);
    known=0xc5u;
    if(count==1) {flags|=std::uint64_t(right?((left^result)>>(width-1))&1u:((result>>(width-1))^carry)&1u)<<11;known|=0x800u;}
#endif
    // Faulting writes retain incoming register/flag facts.
    if(mem)memory.store(address,result,width);
    else if(word)write_gpr_slice(s,destination,result,selected);
    else scalar_gpr32(s,destination,{result,0xffffffffu,{}});
    s.flags=(s.flags&~std::uint64_t(0x8d5u))|flags;s.defined_flags=(s.defined_flags&~std::uint64_t(0x8d5u))|known;
    for(unsigned bit=0;bit<32;++bit)if(0x8d5u&(1u<<bit))s.flag_origins[bit]=(known&(1u<<bit))?0:fresh_unknown_flag_origin(s,bit);
}
}
#endif
