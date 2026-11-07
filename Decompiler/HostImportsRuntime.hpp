#include <algorithm>
#include <cstdlib>
#include <initializer_list>
#include <limits>
#include <string>

namespace sharp_retro::source {
[[noreturn]] inline void unsupported_loader_stub() { throw std::runtime_error("dyld loader entry requires loader execution"); }
inline const char* c_string(std::uint64_t value) { return static_cast<const char*>(pointer(value)); }
inline FILE* file(std::uint64_t value) { return static_cast<FILE*>(pointer(value)); }
inline std::uint64_t host_allocate(std::uint64_t size) {
    if(size>16*1024*1024) throw std::runtime_error("allocation exceeds adapter limit");
    void* result=std::malloc(std::size_t(size));
#ifdef SHARP_RETRO_CHECKED_MEMORY
    if(result) expose(result,std::size_t(size),true);
#endif
    return address(result);
}
inline void host_release(std::uint64_t value) {
#ifdef SHARP_RETRO_CHECKED_MEMORY
    std::erase_if(accessible,[&](const auto& span) { return span.begin==value; });
#endif
    std::free(pointer(value));
}
inline std::size_t transfer_size(std::uint64_t size,std::uint64_t count) {
    if(size && count>std::numeric_limits<std::size_t>::max()/size) throw std::runtime_error("stdio transfer size overflow");
    auto bytes=std::size_t(size*count);
    if(bytes>16*1024*1024) throw std::runtime_error("stdio transfer exceeds adapter limit");
    return bytes;
}
inline std::uint64_t host_fread(std::uint64_t data,std::uint64_t size,std::uint64_t count,std::uint64_t stream) {
    [[maybe_unused]] auto bytes=transfer_size(size,count);
#ifdef SHARP_RETRO_CHECKED_MEMORY
    check_memory(data,bytes,true);
#endif
    return std::fread(pointer(data),std::size_t(size),std::size_t(count),file(stream));
}
inline std::uint64_t host_fwrite(std::uint64_t data,std::uint64_t size,std::uint64_t count,std::uint64_t stream) {
    [[maybe_unused]] auto bytes=transfer_size(size,count);
#ifdef SHARP_RETRO_CHECKED_MEMORY
    check_memory(data,bytes,false);
#endif
    return std::fwrite(pointer(data),std::size_t(size),std::size_t(count),file(stream));
}
// Decode supported C formats instead of forwarding untyped x86 varargs into
// AArch64's different variadic ABI. Unsupported formats/vector arguments fail.
inline std::uint64_t format_output(FILE* stream,std::uint64_t format,std::initializer_list<std::uint64_t> values,std::uint64_t vector_count) {
    if((vector_count&255)!=0) throw std::runtime_error("floating-point varargs are unsupported");
    std::string output;
    const char* text=c_string(format); auto next=values.begin();
    for(std::size_t i=0;text[i];++i) {
        if(text[i]!='%') { output+=text[i]; continue; }
        char type=text[++i];
        if(type=='%') { output+='%'; continue; }
        if(next==values.end()) throw std::runtime_error("format requires stack-passed arguments");
        auto value=*next++;
        if(type=='s') output+=c_string(value);
        else if(type=='c') output+=char(static_cast<unsigned char>(value));
        else throw std::runtime_error("unsupported formatted I/O conversion");
        if(output.size()>16*1024*1024) throw std::runtime_error("formatted output exceeds adapter limit");
    }
    auto count=std::fwrite(output.data(),1,output.size(),stream);
    return count==output.size() ? std::uint64_t(count) : std::uint64_t(std::uint32_t(-1));
}
} // namespace sharp_retro::source
