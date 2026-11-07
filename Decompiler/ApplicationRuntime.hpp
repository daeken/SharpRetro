#include <algorithm>
#include <climits>
#include <cstdio>
#include <cstring>
#include <string>
#include <vector>
#include <unistd.h>

namespace sharp_retro::application {
class GuestMemory : public Memory {
    struct Region {
        std::uint64_t address;
        std::vector<std::uint8_t> bytes;
        bool writable;
    };
    std::vector<Region> regions;
    Region& region(std::uint64_t address) {
        for(auto& r : regions) if(address >= r.address && address-r.address < r.bytes.size()) return r;
        throw std::runtime_error("unmapped guest memory at " + std::to_string(address));
    }
    static void check_width(std::uint64_t address, unsigned width) {
        if(width == 0 || width > 128 || width % 8 || address > UINT64_MAX - (width / 8 - 1))
            throw std::runtime_error("invalid guest memory width/address");
    }
public:
    void map(std::uint64_t address, std::uint64_t size, bool writable) {
        if(size == 0 || size > 16*1024*1024 || address > UINT64_MAX-size)
            throw std::runtime_error("guest mapping exceeds adapter limits");
        for(const auto& r : regions)
            if(address < r.address+r.bytes.size() && r.address < address+size)
                throw std::runtime_error("overlapping guest mappings");
        regions.push_back({address, std::vector<std::uint8_t>(size), writable});
    }
    void initialize(std::uint64_t address, const std::uint8_t* data, std::size_t size) {
        auto& r=region(address);
        if(size > r.bytes.size()-(address-r.address)) throw std::runtime_error("image initialization bounds");
        std::copy(data,data+size,r.bytes.begin()+(address-r.address));
    }
    bits load(std::uint64_t address, unsigned width) override {
        check_width(address,width); bits value=0;
        for(unsigned i=0;i<width/8;++i) {
            auto& r=region(address+i);
            value|=bits(r.bytes[address+i-r.address])<<(i*8);
        }
        return value;
    }
    void store(std::uint64_t address, bits value, unsigned width) override {
        check_width(address,width);
        for(unsigned i=0;i<width/8;++i) {
            auto& r=region(address+i);
            if(!r.writable) throw std::runtime_error("write to read-only guest memory");
            r.bytes[address+i-r.address]=std::uint8_t(value>>(i*8));
        }
    }
    bool invoke_import(const char* symbol, State& state) override {
        bool reading=std::strcmp(symbol,"_read")==0;
        if(!reading && std::strcmp(symbol,"_write")!=0) return false;
        if(state.gpr[7]>INT_MAX || state.gpr[2]>1024*1024)
            throw std::runtime_error("I/O request exceeds adapter limits");
        int fd=int(state.gpr[7]);
        std::uint64_t address=state.gpr[6], size=state.gpr[2];
        if(address > UINT64_MAX-size) throw std::runtime_error("I/O guest address wraps");
        std::vector<std::uint8_t> buffer(size);
        ssize_t result;
        if(reading) {
            // Validate the full destination before consuming host input.
            for(std::uint64_t i=0;i<size;++i)
                if(!region(address+i).writable) throw std::runtime_error("read destination is read-only");
            result=::read(fd,buffer.data(),buffer.size());
            if(result>0) for(ssize_t i=0;i<result;++i) store(address+std::uint64_t(i),buffer[std::size_t(i)],8);
        } else {
            for(std::uint64_t i=0;i<size;++i) buffer[i]=std::uint8_t(load(address+i,8));
            result=::write(fd,buffer.data(),buffer.size());
        }
        // SysV integer/pointer arguments are guest RDI/RSI/RDX. Return is RAX;
        // import completion pops the return address exactly like the x86 callee.
        state.gpr[0]=std::uint64_t(result);
        state.pc=std::uint64_t(load(state.gpr[4],64));
        state.gpr[4]+=8;
        return true;
    }
};

inline int execute(int argc, char** argv, GuestMemory& memory) {
    constexpr std::uint64_t stack=0x700000000000ULL, arguments=0x600000000000ULL;
    constexpr std::uint64_t arena_size=1024*1024, sentinel=0xfffffffffffffff0ULL;
    memory.map(stack,arena_size,true); memory.map(arguments,arena_size,true);
    std::vector<std::uint64_t> strings;
    std::uint64_t cursor=arguments;
    for(int i=0;i<argc;++i) {
        auto length=std::strlen(argv[i]);
        if(length+1 > arguments+arena_size-cursor) throw std::runtime_error("guest argument storage exhausted");
        strings.push_back(cursor);
        for(std::size_t j=0;j<=length;++j) memory.store(cursor++,std::uint8_t(argv[i][j]),8);
    }
    cursor=(cursor+7)&~std::uint64_t(7);
    std::uint64_t argv_address=cursor;
    if(strings.size()+2 > (arguments+arena_size-cursor)/8) throw std::runtime_error("guest argv table exhausted");
    for(auto address : strings) { memory.store(cursor,address,64); cursor+=8; }
    memory.store(cursor,0,64); cursor+=8;
    std::uint64_t empty_environment=cursor;
    memory.store(cursor,0,64);
    State state;
    state.pc=entry; state.gpr[7]=std::uint64_t(argc); state.gpr[6]=argv_address;
    state.gpr[2]=empty_environment; state.gpr[1]=empty_environment;
    state.gpr[4]=stack+arena_size-8; memory.store(state.gpr[4],sentinel,64);
    auto outcome=run(state,memory,100'000'000);
    if(outcome!=Exit::returned || state.pc!=sentinel || state.gpr[4]!=stack+arena_size)
        throw std::runtime_error("guest main did not return completely within its budget");
    return int(state.gpr[0]&255);
}
} // namespace sharp_retro::application
