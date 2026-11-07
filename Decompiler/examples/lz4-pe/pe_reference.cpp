// Function-level reference runner for the published x64 PE DLL.
// This deliberately does not run DllMain or TLS callbacks, register Windows
// unwind metadata, or emulate a Windows process. Only selected export closures
// which need none of that initialization may be invoked. Unsupported imports
// terminate the subprocess; exceptions never cross unregistered PE stack frames.
#include <algorithm>
#include <cctype>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <fstream>
#include <iostream>
#include <limits>
#include <stdexcept>
#include <string>
#include <string_view>
#include <vector>
#include <sys/mman.h>
#include <unistd.h>

#if !defined(__x86_64__)
#error Build this reference runner for x86_64; its PE calls use the Windows x64 ABI.
#endif
#define MS_ABI __attribute__((ms_abi))

extern "C" MS_ABI void* pe_memcpy(void* dst,const void* src,std::size_t n) { return std::memcpy(dst,src,n); }
extern "C" MS_ABI void* pe_memmove(void* dst,const void* src,std::size_t n) { return std::memmove(dst,src,n); }
extern "C" MS_ABI void* pe_memset(void* dst,int value,std::size_t n) { return std::memset(dst,value,n); }
extern "C" [[noreturn]] MS_ABI void pe_unsupported_import() {
    std::fputs("pe_reference: unsupported PE import executed\n",stderr);
    std::_Exit(126);
}

// SysV host entry: RDI=function, RSI=a0, RDX=a1, RCX=a2, R8=a3.
// Supply a concrete, reproducible machine entry domain to the raw PE export,
// matching the recovered adapter's explicit default preservation parameters.
// Six SysV nonvolatile registers are saved; RDI/RSI are host caller-saved.
// Entry RSP is 8 modulo 16. Six pushes plus 40 bytes make it aligned before
// CALL, with the required 32-byte Win64 shadow store above its return address.
extern "C" __attribute__((naked,noinline)) std::uint64_t pe_invoke4(void*,std::uint64_t,std::uint64_t,std::uint64_t,std::uint64_t) {
    __asm__ volatile(
        "pushq %rbx\n"
        "pushq %rbp\n"
        "pushq %r12\n"
        "pushq %r13\n"
        "pushq %r14\n"
        "pushq %r15\n"
        "subq $40, %rsp\n"
        "movq %rdi, %r11\n"
        "movq %r8, %r9\n"
        "movq %rcx, %r8\n"
        "movq %rsi, %rcx\n"
        "xorl %ebx, %ebx\n"
        "xorl %ebp, %ebp\n"
        "xorl %esi, %esi\n"
        "xorl %edi, %edi\n"
        "xorl %r12d, %r12d\n"
        "xorl %r13d, %r13d\n"
        "xorl %r14d, %r14d\n"
        "xorl %r15d, %r15d\n"
        "callq *%r11\n"
        "addq $40, %rsp\n"
        "popq %r15\n"
        "popq %r14\n"
        "popq %r13\n"
        "popq %r12\n"
        "popq %rbp\n"
        "popq %rbx\n"
        "retq\n");
}

namespace {
[[noreturn]] void fail(const std::string& text) { throw std::runtime_error(text); }

template<class T> T number(const std::vector<std::uint8_t>& bytes,std::size_t offset) {
    if(offset>bytes.size() || sizeof(T)>bytes.size()-offset) fail("truncated PE file");
    T result=0;
    for(std::size_t i=0;i<sizeof(T);++i) result|=T(bytes[offset+i])<<(8*i);
    return result;
}

struct Section { std::uint32_t rva,size,characteristics; };

class Image {
    std::uint8_t* base_=nullptr;
    std::size_t size_=0,mapped_=0,page_=0;
    std::uint32_t headers_=0;
    std::uint64_t preferred_=0;
    std::vector<Section> sections_;
    struct Snapshot { std::uint32_t rva; std::vector<std::uint8_t> bytes; };
    std::vector<Snapshot> snapshots_;
    struct Directory { std::uint32_t rva=0,size=0; } directories_[16];

    void span(std::uint64_t rva,std::uint64_t bytes) const {
        if(rva>size_ || bytes>size_-rva) fail("PE RVA outside image");
        if(rva<=headers_ && bytes<=headers_-rva) return;
        for(const auto& section:sections_)
            if(rva>=section.rva && rva-section.rva<=section.size && bytes<=section.size-(rva-section.rva)) return;
        fail("PE RVA outside mapped headers/sections");
    }
    template<class T> T get(std::uint64_t rva) const {
        span(rva,sizeof(T)); T value=0;
        for(std::size_t i=0;i<sizeof(T);++i) value|=T(base_[rva+i])<<(8*i);
        return value;
    }
    template<class T> void put(std::uint64_t rva,T value) {
        span(rva,sizeof(T));
        for(std::size_t i=0;i<sizeof(T);++i) base_[rva+i]=std::uint8_t(value>>(8*i));
    }
    std::string string(std::uint64_t rva) const {
        std::string result;
        for(std::size_t i=0;i<4096;++i) {
            char c=char(get<std::uint8_t>(rva+i));
            if(!c) return result;
            result.push_back(c);
        }
        fail("unterminated PE string");
    }
    void relocations() {
        const auto directory=directories_[5];
        const auto delta=std::uint64_t(reinterpret_cast<std::uintptr_t>(base_))-preferred_;
        if(!directory.rva || !directory.size) {
            if(delta) fail("image cannot be rebased without a relocation directory");
            return;
        }
        span(directory.rva,directory.size);
        std::uint64_t cursor=directory.rva,end=cursor+directory.size;
        while(cursor<end) {
            if(end-cursor<8) fail("truncated relocation block");
            auto page=get<std::uint32_t>(cursor),bytes=get<std::uint32_t>(cursor+4);
            if(bytes<8 || (bytes&1) || bytes>end-cursor) fail("invalid relocation block size");
            for(std::uint64_t pos=cursor+8;pos<cursor+bytes;pos+=2) {
                auto item=get<std::uint16_t>(pos);
                if((item>>12)==0) continue;
                if((item>>12)!=10) fail("unsupported relocation type (requires DIR64)");
                std::uint64_t target=std::uint64_t(page)+(item&4095);
                put<std::uint64_t>(target,get<std::uint64_t>(target)+delta);
            }
            cursor+=bytes;
        }
    }
    void imports() {
        const auto directory=directories_[1];
        if(!directory.rva && !directory.size) return;
        if(!directory.rva || !directory.size) fail("incomplete import directory");
        span(directory.rva,directory.size);
        bool terminated=false;
        for(std::uint64_t offset=0;offset+20<=directory.size;offset+=20) {
            std::uint64_t descriptor=directory.rva+offset;
            auto original=get<std::uint32_t>(descriptor),time=get<std::uint32_t>(descriptor+4);
            auto chain=get<std::uint32_t>(descriptor+8),name=get<std::uint32_t>(descriptor+12);
            auto iat=get<std::uint32_t>(descriptor+16);
            if(!(original|time|chain|name|iat)) { terminated=true; break; }
            if(!name || !iat) fail("invalid import descriptor");
            auto library=string(name);
            std::transform(library.begin(),library.end(),library.begin(),[](unsigned char c){return char(std::tolower(c));});
            std::uint64_t lookup=original ? original : iat;
            bool ended=false;
            for(std::uint64_t index=0;index<65536;++index) {
                auto thunk=get<std::uint64_t>(lookup+index*8);
                if(!thunk) { ended=true; break; }
                std::uintptr_t address=reinterpret_cast<std::uintptr_t>(&pe_unsupported_import);
                if(!(thunk&(std::uint64_t(1)<<63))) {
                    if(thunk>std::numeric_limits<std::uint32_t>::max()) fail("invalid import-name RVA");
                    (void)get<std::uint16_t>(thunk);
                    auto symbol=string(thunk+2);
                    if(library=="msvcrt.dll") {
                        if(symbol=="memcpy") address=reinterpret_cast<std::uintptr_t>(&pe_memcpy);
                        else if(symbol=="memmove") address=reinterpret_cast<std::uintptr_t>(&pe_memmove);
                        else if(symbol=="memset") address=reinterpret_cast<std::uintptr_t>(&pe_memset);
                    }
                }
                put<std::uint64_t>(std::uint64_t(iat)+index*8,address);
            }
            if(!ended) fail("unterminated import thunk table");
        }
        if(!terminated) fail("unterminated import directory");
    }
    void protect() {
        // PE section alignment can be smaller than a host page. Merge access
        // rights for shared pages rather than letting the last section win.
        std::vector<int> rights(mapped_/page_,PROT_NONE);
        std::fill_n(rights.begin(),(headers_+page_-1)/page_,PROT_READ);
        for(const auto& section:sections_) {
            if(!section.size) continue;
            std::size_t first=section.rva/page_;
            std::size_t last=(std::size_t(section.rva)+section.size+page_-1)/page_;
            int flags=0;
            if(section.characteristics&0x40000000u) flags|=PROT_READ;
            if(section.characteristics&0x80000000u) flags|=PROT_WRITE;
            if(section.characteristics&0x20000000u) flags|=PROT_EXEC;
            for(std::size_t page=first;page<last;++page) rights[page]|=flags;
        }
        for(std::size_t first=0;first<rights.size();) {
            std::size_t last=first+1;
            while(last<rights.size() && rights[last]==rights[first]) ++last;
            if(mprotect(base_+first*page_,(last-first)*page_,rights[first])) fail("cannot protect PE pages");
            first=last;
        }
    }
public:
    explicit Image(const char* filename) {
        std::ifstream input(filename,std::ios::binary|std::ios::ate);
        if(!input) fail("cannot open DLL");
        auto length=input.tellg();
        if(length<0 || length>128*1024*1024) fail("invalid DLL file size");
        std::vector<std::uint8_t> bytes(static_cast<std::size_t>(length));
        input.seekg(0); input.read(reinterpret_cast<char*>(bytes.data()),std::streamsize(bytes.size()));
        if(!input) fail("cannot read DLL");
        if(number<std::uint16_t>(bytes,0)!=0x5a4d) fail("not an MZ image");
        std::size_t pe=number<std::uint32_t>(bytes,0x3c);
        if(number<std::uint32_t>(bytes,pe)!=0x4550 || number<std::uint16_t>(bytes,pe+4)!=0x8664) fail("requires x64 PE image");
        auto count=number<std::uint16_t>(bytes,pe+6),optionalSize=number<std::uint16_t>(bytes,pe+20);
        std::size_t optional=pe+24;
        if(count==0 || count>96 || optionalSize<112 || optional>bytes.size() || optionalSize>bytes.size()-optional) fail("invalid PE headers");
        if(number<std::uint16_t>(bytes,optional)!=0x20b) fail("requires PE32+");
        preferred_=number<std::uint64_t>(bytes,optional+24);
        size_=number<std::uint32_t>(bytes,optional+56); headers_=number<std::uint32_t>(bytes,optional+60);
        if(!size_ || size_>256*1024*1024 || !headers_ || headers_>size_ || headers_>bytes.size()) fail("invalid image/header size");
        auto directoryCount=number<std::uint32_t>(bytes,optional+108);
        if(directoryCount>(optionalSize-112)/8) fail("truncated PE directory table");
        for(std::size_t i=0;i<std::min<std::size_t>(directoryCount,16);++i)
            directories_[i]={number<std::uint32_t>(bytes,optional+112+i*8),number<std::uint32_t>(bytes,optional+116+i*8)};
        std::size_t sectionTable=optional+optionalSize;
        if(sectionTable>headers_ || std::size_t(count)*40>headers_-sectionTable) fail("section table outside headers");
        for(std::size_t i=0;i<count;++i) {
            auto at=sectionTable+i*40;
            auto virt=number<std::uint32_t>(bytes,at+8),rva=number<std::uint32_t>(bytes,at+12);
            auto raw=number<std::uint32_t>(bytes,at+16),file=number<std::uint32_t>(bytes,at+20);
            auto extent=std::max(virt,raw);
            if(rva<headers_ || rva>size_ || extent>size_-rva || file>bytes.size() || raw>bytes.size()-file) fail("section outside image/file");
            for(const auto& other:sections_)
                if(std::uint64_t(rva)<std::uint64_t(other.rva)+other.size && std::uint64_t(other.rva)<std::uint64_t(rva)+extent) fail("overlapping PE sections");
            sections_.push_back({rva,extent,number<std::uint32_t>(bytes,at+36)});
        }
        long page=sysconf(_SC_PAGESIZE); if(page<=0) fail("cannot determine page size");
        page_=std::size_t(page); mapped_=(size_+page_-1)/page_*page_;
        auto mapping=mmap(nullptr,mapped_,PROT_READ|PROT_WRITE,MAP_PRIVATE|MAP_ANON,-1,0);
        if(mapping==MAP_FAILED) fail("cannot map PE image");
        base_=static_cast<std::uint8_t*>(mapping);
        try {
            std::memcpy(base_,bytes.data(),headers_);
            for(std::size_t i=0;i<count;++i) {
                auto at=sectionTable+i*40;
                auto raw=number<std::uint32_t>(bytes,at+16),file=number<std::uint32_t>(bytes,at+20);
                if(raw) std::memcpy(base_+sections_[i].rva,bytes.data()+file,raw);
            }
            // Anonymous mapping already zeroed virtual tails and BSS.
            relocations(); imports(); protect();
            snapshots_.push_back({0,std::vector<std::uint8_t>(base_,base_+headers_)});
            for(const auto& section:sections_)
                if(section.characteristics&0x40000000u)
                    snapshots_.push_back({section.rva,std::vector<std::uint8_t>(base_+section.rva,base_+section.rva+section.size)});
        } catch(...) { munmap(base_,mapped_); base_=nullptr; throw; }
    }
    Image(const Image&)=delete;
    Image& operator=(const Image&)=delete;
    ~Image() { if(base_) munmap(base_,mapped_); }
    bool unchanged() const {
        return std::all_of(snapshots_.begin(),snapshots_.end(),[&](const Snapshot& snapshot) {
            return std::equal(snapshot.bytes.begin(),snapshot.bytes.end(),base_+snapshot.rva);
        });
    }
    void* function(std::string_view wanted) const {
        const auto directory=directories_[0];
        if(!directory.rva || directory.size<40) fail("missing export directory");
        span(directory.rva,directory.size);
        auto exports=get<std::uint32_t>(directory.rva+20),names=get<std::uint32_t>(directory.rva+24);
        auto functions=get<std::uint32_t>(directory.rva+28),nameTable=get<std::uint32_t>(directory.rva+32),ordinals=get<std::uint32_t>(directory.rva+36);
        if(exports>65536 || names>65536) fail("export table too large");
        span(functions,std::uint64_t(exports)*4); span(nameTable,std::uint64_t(names)*4); span(ordinals,std::uint64_t(names)*2);
        for(std::uint64_t i=0;i<names;++i) {
            if(string(get<std::uint32_t>(std::uint64_t(nameTable)+i*4))!=wanted) continue;
            auto ordinal=get<std::uint16_t>(std::uint64_t(ordinals)+i*2);
            if(ordinal>=exports) fail("export ordinal outside address table");
            auto rva=get<std::uint32_t>(std::uint64_t(functions)+ordinal*4);
            if(!rva) fail("null export");
            if(rva>=directory.rva && std::uint64_t(rva)-directory.rva<directory.size) fail("forwarded export unsupported");
            span(rva,1);
            for(const auto& section:sections_)
                if(rva>=section.rva && rva-section.rva<section.size && (section.characteristics&0x20000000u)) return base_+rva;
            fail("export is not executable");
        }
        fail("export not found: "+std::string(wanted));
    }
};

std::int32_t signed_argument(const char* text) {
    std::size_t end=0; auto value=std::stoll(text,&end,10);
    if(text[end] || value<std::numeric_limits<std::int32_t>::min() || value>std::numeric_limits<std::int32_t>::max()) fail("expected signed 32-bit integer");
    return std::int32_t(value);
}
int digit(char c) {
    if(c>='0' && c<='9') return c-'0';
    if(c>='a' && c<='f') return c-'a'+10;
    if(c>='A' && c<='F') return c-'A'+10;
    fail("invalid input hex");
}
std::string hex(const std::uint8_t* data,std::size_t length) {
    static constexpr char digits[]="0123456789abcdef";
    std::string result(length*2,'0');
    for(std::size_t i=0;i<length;++i) { result[2*i]=digits[data[i]>>4]; result[2*i+1]=digits[data[i]&15]; }
    return result;
}
}

int main(int argc,char** argv) {
    try {
        if(argc<3) fail("usage: pe_reference DLL version | bound SIZE | decompress HEXINPUT CAPACITY");
        Image image(argv[1]); std::string command=argv[2];
        if(command=="version" && argc==3) {
            using Function=std::uint32_t(MS_ABI*)();
            auto function=reinterpret_cast<Function>(image.function("LZ4_versionNumber"));
            auto result=std::uint32_t(pe_invoke4(reinterpret_cast<void*>(function),0,0,0,0));
            std::cout<<"{\"return\":"<<result<<",\"image_unchanged\":"<<(image.unchanged() ? "true" : "false")<<",\"initial_preserved_gprs_zero\":true}\n";
        } else if(command=="bound" && argc==4) {
            using Function=std::int32_t(MS_ABI*)(std::int32_t);
            auto size=signed_argument(argv[3]);
            auto function=reinterpret_cast<Function>(image.function("LZ4_compressBound"));
            auto result=std::int32_t(pe_invoke4(reinterpret_cast<void*>(function),std::uint32_t(size),0,0,0));
            std::cout<<"{\"return\":"<<result<<",\"image_unchanged\":"<<(image.unchanged() ? "true" : "false")<<",\"initial_preserved_gprs_zero\":true}\n";
        } else if(command=="decompress" && argc==5) {
            std::string_view input=argv[3]; auto capacity=signed_argument(argv[4]);
            if(input.size()%2 || input.size()>2*1024*1024 || capacity<0 || capacity>1024*1024) fail("input/output limit is one MiB");
            std::size_t length=input.size()/2;
            std::vector<std::uint8_t> source(length+64,0xee),destination(std::size_t(capacity)+64,0xcd);
            for(std::size_t i=0;i<length;++i) source[i+32]=std::uint8_t((digit(input[2*i])<<4)|digit(input[2*i+1]));
            const auto originalSource=source;
            std::fill(destination.begin()+32,destination.end()-32,0xa5);
            using Function=std::int32_t(MS_ABI*)(const void*,void*,std::int32_t,std::int32_t);
            auto function=reinterpret_cast<Function>(image.function("LZ4_decompress_safe"));
            auto result=std::int32_t(pe_invoke4(reinterpret_cast<void*>(function),reinterpret_cast<std::uintptr_t>(source.data()+32),
                reinterpret_cast<std::uintptr_t>(destination.data()+32),length,std::uint32_t(capacity)));
            bool canaries=source==originalSource && std::all_of(destination.begin(),destination.begin()+32,[](auto c){return c==0xcd;})
                && std::all_of(destination.end()-32,destination.end(),[](auto c){return c==0xcd;});
            std::cout<<"{\"return\":"<<result<<",\"output\":\""<<hex(destination.data()+32,std::size_t(capacity))<<"\",\"canaries_ok\":"<<(canaries ? "true" : "false")
                <<",\"image_unchanged\":"<<(image.unchanged() ? "true" : "false")<<",\"initial_preserved_gprs_zero\":true}\n";
        } else fail("invalid command or argument count");
        return 0;
    } catch(const std::exception& error) {
        std::cerr<<"pe_reference: "<<error.what()<<'\n'; return 125;
    }
}
