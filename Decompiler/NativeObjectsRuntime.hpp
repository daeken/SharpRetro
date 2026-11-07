#include <array>
#include <bit>
#include <climits>
#include <cstddef>
#include <cstdio>
#include <cstring>
#include <type_traits>
#include <unistd.h>

namespace sharp_retro::source {
static_assert(sizeof(void*) == 8 && std::endian::native == std::endian::little);
// Native Clang/GCC profile: direct accesses may be unaligned and may alias
// other widths/types. These typedef attributes apply to the memory view, not
// the alignment or type of the value carried by recovered variables.
using u8 __attribute__((aligned(1),may_alias)) = std::uint8_t;
using u16 __attribute__((aligned(1),may_alias)) = std::uint16_t;
using u32 __attribute__((aligned(1),may_alias)) = std::uint32_t;
using u64 __attribute__((aligned(1),may_alias)) = std::uint64_t;
using u128 __attribute__((aligned(1),may_alias)) = bits;
static_assert(alignof(u8)==1 && alignof(u16)==1 && alignof(u32)==1 && alignof(u64)==1 && alignof(u128)==1);
static_assert(sizeof(u8)==1 && sizeof(u16)==2 && sizeof(u32)==4 && sizeof(u64)==8 && sizeof(u128)==16);
inline std::uint64_t address(const void* pointer) { return reinterpret_cast<std::uintptr_t>(pointer); }
inline void* pointer(std::uint64_t value) { return reinterpret_cast<void*>(std::uintptr_t(value)); }
inline u8* byte_pointer(std::uint64_t value) { return reinterpret_cast<u8*>(std::uintptr_t(value)); }
inline const u8* const_byte_pointer(std::uint64_t value) { return reinterpret_cast<const u8*>(std::uintptr_t(value)); }
// Address roles do not prove C++ array bounds. Retain the original unsigned
// machine arithmetic when producing a derived native pointer.
template<class T> inline T* byte_offset(T* base,std::uint64_t offset) {
    return reinterpret_cast<T*>(std::uintptr_t(address(base)+offset));
}
inline bool checked_flag(bool value, bool defined) {
    if(!defined) throw std::runtime_error("read of architecturally undefined flags");
    return value;
}
inline std::uint64_t input_output(bool reading, std::uint64_t fd, std::uint64_t buffer, std::uint64_t size) {
    if(fd>INT_MAX || size>1024*1024) throw std::runtime_error("I/O request exceeds adapter limits");
    return std::uint64_t(reading ? ::read(int(fd),pointer(buffer),std::size_t(size))
                                 : ::write(int(fd),pointer(buffer),std::size_t(size)));
}
} // namespace sharp_retro::source
