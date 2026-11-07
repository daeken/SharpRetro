#include <array>
#include <bit>
#include <climits>
#include <cstdio>
#include <cstring>
#include <vector>
#include <unistd.h>

namespace sharp_retro::source {
static_assert(sizeof(void*) == 8 && std::endian::native == std::endian::little);
constexpr std::uint64_t main_return = 0xfffffffffffffff0ULL;
struct CallFrame {
    std::uint64_t stack_pointer;
    std::uint64_t return_address;
    unsigned depth;
};
inline std::uint64_t address(const void* pointer) { return reinterpret_cast<std::uintptr_t>(pointer); }
inline void* pointer(std::uint64_t value) { return reinterpret_cast<void*>(std::uintptr_t(value)); }
struct MemorySpan { std::uint64_t begin, size; bool writable; };
inline thread_local std::vector<MemorySpan> accessible;
inline void expose(const void* data, std::size_t size, bool writable) {
    accessible.push_back({address(data),size,writable});
}
inline void check_memory(std::uint64_t where, std::uint64_t size, bool writing) {
    if(size == 0) return;
    for(const auto& span : accessible)
        if(where>=span.begin && where-span.begin<span.size && size<=span.size-(where-span.begin)) {
            if(writing && !span.writable) throw std::runtime_error("write to read-only native storage");
            return;
        }
    throw std::runtime_error("native access outside recovered objects");
}
template<class T> inline T read_memory(std::uint64_t where) {
    check_memory(where,sizeof(T),false);
    T value; std::memcpy(&value,pointer(where),sizeof(value)); return value;
}
template<class T> inline void write_memory(std::uint64_t where, T value) {
    check_memory(where,sizeof(T),true);
    std::memcpy(pointer(where),&value,sizeof(value));
}
inline bool checked_flag(bool value, bool defined) {
    if(!defined) throw std::runtime_error("read of architecturally undefined flags");
    return value;
}
inline std::uint64_t input_output(bool reading, std::uint64_t fd, std::uint64_t buffer, std::uint64_t size) {
    if(fd>INT_MAX || size>1024*1024) throw std::runtime_error("I/O request exceeds adapter limits");
    check_memory(buffer,size,reading);
    return std::uint64_t(reading ? ::read(int(fd),pointer(buffer),std::size_t(size))
                                 : ::write(int(fd),pointer(buffer),std::size_t(size)));
}
} // namespace sharp_retro::source
