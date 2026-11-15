#ifndef SHARP_RETRO_EXTRA_MOVS32_RUNTIME
#define SHARP_RETRO_EXTRA_MOVS32_RUNTIME
// Embedded only into units needing byte MOVS or byte/word REP MOVS. The common
// runtime and existing dword path stay unchanged. Addresses are guest offsets;
// the chosen Memory backend owns their actual mapping.
namespace sharp_retro {
inline bool string_compare32(State& s,Memory& memory,unsigned descriptor,std::uint64_t& budget) {
    unsigned width=descriptor&255u;bool scan=descriptor&0x100u,repeat=descriptor&0x200u,not_equal=descriptor&0x400u;
    if((width!=8 && width!=16 && width!=32) || (descriptor&~0x7ffu) || (not_equal&&!repeat))
        throw std::runtime_error("unsupported address32 string comparison");
    const std::uint32_t selected=width==32?0xffffffffu:(1u<<width)-1u,sign=1u<<(width-1u);
    while(!repeat || std::uint32_t(read_gpr_mask(s,1,0xffffffffu))!=0) {
        if(repeat) {if(!budget)return false;--budget;}
        // Resolve/read the complete iteration before flags, count or indexes
        // change. A fault retains exactly the completed prefix of the string.
        auto destination=std::uint32_t(read_gpr_mask(s,7,0xffffffffu));
        auto down=(read_flags(s,10)&1)!=0;
        auto source=scan?0u:std::uint32_t(read_gpr_mask(s,6,0xffffffffu));
        auto left=scan?std::uint32_t(read_gpr_mask(s,0,selected)):std::uint32_t(memory.load(s.segments[3]+source,width));
        auto right=std::uint32_t(memory.load(s.segments[0]+destination,width));
        auto result=std::uint32_t(left-right)&selected;
        std::uint64_t flags=std::uint64_t(left<right)|std::uint64_t(std::popcount(result&255u)%2==0)*4u|
            ((left^right^result)&16u)|std::uint64_t(result==0)*64u|std::uint64_t((result&sign)!=0)*128u|
            std::uint64_t(((left^right)&(left^result)&sign)!=0)*0x800u;
        s.flags=(s.flags&~std::uint64_t(0x8d5u))|flags;s.defined_flags|=0x8d5u;
        for(unsigned bit=0;bit<32;++bit)if(0x8d5u&(1u<<bit))s.flag_origins[bit]=0;
        auto step=down?std::uint32_t(0u-width/8):std::uint32_t(width/8);
        if(!scan)scalar_gpr32(s,6,{std::uint32_t(source+step),0xffffffffu,{}});
        scalar_gpr32(s,7,{std::uint32_t(destination+step),0xffffffffu,{}});
        if(!repeat)return true;
        scalar_gpr32(s,1,{std::uint32_t(s.gpr[1]-1u),0xffffffffu,{}});
        if((result==0)==not_equal)return true;
    }
    return true;
}
inline void movs_extra32(State& s,Memory& memory,unsigned width) {
    if(width!=8 && width!=16) throw std::runtime_error("unsupported extra MOVS width");
    auto source=std::uint32_t(read_gpr_mask(s,6,0xffffffffu));
    auto destination=std::uint32_t(read_gpr_mask(s,7,0xffffffffu));
    auto down=(read_flags(s,10)&1)!=0;
    // One full source element precedes the overlapping write. A failed read or
    // write leaves this iteration's indexes/count unchanged.
    auto value=width==8 ? memory.load_scalar8(s.segments[3]+source) : memory.load_scalar16(s.segments[3]+source);
    if(width==8) memory.store_scalar8(s.segments[0]+destination,value);
    else memory.store_scalar16(s.segments[0]+destination,value);
    auto step=down ? std::uint32_t(0u-width/8) : std::uint32_t(width/8);
    scalar_gpr32(s,6,{std::uint32_t(source+step),0xffffffffu,{}});
    scalar_gpr32(s,7,{std::uint32_t(destination+step),0xffffffffu,{}});
}
inline bool rep_movs_extra32(State& s,Memory& memory,unsigned width,std::uint64_t& budget) {
    if(width!=8 && width!=16) throw std::runtime_error("unsupported extra REP MOVS width");
    while(std::uint32_t(read_gpr_mask(s,1,0xffffffffu))!=0) {
        if(!budget) return false;
        --budget;
        movs_extra32(s,memory,width);
        scalar_gpr32(s,1,{std::uint32_t(s.gpr[1]-1u),0xffffffffu,{}});
    }
    return true;
}
}
#endif
