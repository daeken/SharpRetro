#include <cstdint>
#include <stdexcept>

// Exact-width arithmetic shared by the machine and source backends.
// unsigned __int128 is a Clang/GCC extension, required by this initial backend.
namespace sharp_retro {
using bits = unsigned __int128;
inline bits mask(unsigned width) { return width == 128 ? ~bits(0) : (bits(1) << width) - 1; }
inline bits narrow(bits value, unsigned width) { return value & mask(width); }
inline bits sext(bits value, unsigned from, unsigned to) {
    value &= mask(from);
    return narrow((value & (bits(1) << (from - 1))) ? value | ~mask(from) : value, to);
}
inline bits shift_left(bits value, bits count, unsigned width) {
    return count >= width ? 0 : narrow(value << unsigned(count), width);
}
inline bits shift_right(bits value, bits count, unsigned width) {
    return count >= width ? 0 : narrow(value, width) >> unsigned(count);
}
inline bits shift_arithmetic(bits value, bits count, unsigned width) {
    value &= mask(width);
    bool negative = (value & (bits(1) << (width - 1))) != 0;
    if(count >= width) return negative ? mask(width) : 0;
    unsigned n = unsigned(count);
    if(n == 0) return value;
    return (value >> n) | (negative ? mask(width) ^ mask(width - n) : 0);
}
inline bits rotate_right(bits value, bits count, unsigned width) {
    unsigned n = unsigned(count % width);
    value &= mask(width);
    return n == 0 ? value : narrow((value >> n) | (value << (width - n)), width);
}
inline bits signed_key(bits value, unsigned width) { return narrow(value, width) ^ (bits(1) << (width - 1)); }
inline bits unsigned_divide(bits a, bits b, bool remainder) {
    if(b == 0) throw std::runtime_error("division by zero");
    return remainder ? a % b : a / b;
}
inline bits signed_divide(bits a, bits b, unsigned width, bool remainder) {
    a &= mask(width); b &= mask(width);
    bits sign = bits(1) << (width - 1);
    bool na = (a & sign) != 0, nb = (b & sign) != 0;
    if(b == 0) throw std::runtime_error("division by zero");
    if(a == sign && b == mask(width)) throw std::runtime_error("signed division overflow");
    bits ma = na ? narrow(bits(0) - a, width) : a;
    bits mb = nb ? narrow(bits(0) - b, width) : b;
    bits v = remainder ? ma % mb : ma / mb;
    return narrow((remainder ? na : na != nb) ? bits(0) - v : v, width);
}
inline bits popcount(bits value, unsigned width) {
    value &= mask(width); bits result = 0;
    while(value) { result += value & 1; value >>= 1; }
    return result;
}
inline bits leading_zeroes(bits value, unsigned width) {
    value &= mask(width); unsigned n = 0;
    while(n < width && (value & (bits(1) << (width - 1 - n))) == 0) ++n;
    return n;
}
inline bits reverse_bits(bits value, unsigned width) {
    bits result = 0;
    for(unsigned i = 0; i < width; ++i) { result = (result << 1) | (value & 1); value >>= 1; }
    return result;
}
} // namespace sharp_retro

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
    return address(result);
}
inline void host_release(std::uint64_t value) {
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
    return std::fread(pointer(data),std::size_t(size),std::size_t(count),file(stream));
}
inline std::uint64_t host_fwrite(std::uint64_t data,std::uint64_t size,std::uint64_t count,std::uint64_t stream) {
    [[maybe_unused]] auto bytes=transfer_size(size,count);
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
namespace sharp_retro::source {
// Input SHA-256: 1731d54f5ec77effa24beda1075105395aa9b9ff0a042a618a17204253c8a960
// Recovered source: 1 functions, 94 basic blocks; 1 structured functions, 21 loops, 108 conditionals; native objects, 21 typed stack fields analyzed; 0 proven register identities; 8 private dead effects removed; 0 provenance-checked stdio calls; 17 private stack cells promoted; 1 function SSA graphs; 60 pointer roles; explicit inputs/results, direct calls and tail calls
// .rdata, original VA 0x614ad000
static const char image_0[] =
    "1.8.3\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\x01""\0""\0""\0""\x02""\0""\0""\0""\x01""\0""\0""\0""\0""\0""\0""\0""\x04""\0""\0""\0""\x04""\0""\0""\0""\x04""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\xff""\xff""\xff""\xff"""
    "\xfc""\xff""\xff""\xff""\x01""\0""\0""\0""\x02""\0""\0""\0""\x03""\0""\0""\0""\0""\0""\0""\0""\x01""\xff""\xff""\xff""\x02""\xfe""\xff""\xff""\x03""\xfd""\xff""\xff""\x10""\xf0""\xff""\xff""\x10""\xf0""\xff""\xff""\x10""\xf0""\xff""\xff""\x10""\xf0""\xff""\xff""\x10""\0""\0""\0""\0""\0""\0""\0""\x10""\0""\0""\0""\0""\0""\0""\0"""
    "\x0f""\0""\0""\0""\0""\0""\0""\0""\x0f""\0""\0""\0""\0""\0""\0""\0""\r\xf3""\xff""\xff""\r\xf3""\xff""\xff""\r\xf3""\xff""\xff""\r\xf3""\xff""\xff""\xf1""\xff""\xff""\xff""\xf5""\xfb""\xff""\xff""\xf9""\xf7""\xff""\xff""\xfd""\xf3""\xff""\xff""\x0c""\0""\0""\0""\0""\0""\0""\0""\x0c""\0""\0""\0""\0""\0""\0""\0"""
    "\x04""\xfc""\xff""\xff""\x04""\xfc""\xff""\xff""\x04""\xfc""\xff""\xff""\x04""\xfc""\xff""\xff""\x10""\xf0""\xff""\xff""\xff""\xff""\xff""\xff""\x10""\xf0""\xff""\xff""\xff""\xff""\xff""\xff""\x0f""\xf1""\xff""\xff""\xff""\xff""\xff""\xff""\x0f""\xf1""\xff""\xff""\xff""\xff""\xff""\xff""\0""\0""\0""\x80""\0""\0""\0""\x80""\0""\0""\0""\x80""\0""\0""\0""\x80"""
    "Unspecified error code\0""\0""\xc9""\xc7""\xfe""\xff""\xcc""\xc8""\xfe""\xff""x\xc6""\xfe""\xff""x\xc8""\xfe""\xff""(\xc7""\xfe""\xff""=\xc7""\xfe""\xff""\xa8""\xc5""\xfe""\xff""x\xc5""\xfe""\xff""(\xc5""\xfe""\xff""\x18""\xc6""\xfe""\xff"""
    "\xe8""\xc4""\xfe""\xff""\x98""\xc4""\xfe""\xff""H\xc4""\xfe""\xff""X\xc3""\xfe""\xff""=\xc8""\xfe""\xff""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\x01""\0""\0""\0""\0""\0""\0""\0""\x04""\0""\0""\0""\0""\0""\0""\0""\x10""\0""\0""\0""\0""\0""\0""\0""@\0""\0""\0""\0""\0"""
    "OK_NoError\0""ERROR_GENERIC\0""ERROR_maxBlockSize_invalid\0""ERROR_blockM"
    "ode_invalid\0""\0""\0""\0""\0""ERROR_contentChecksumFlag_invalid\0""\0""\0""\0""\0""\0""\0""ERROR_co"
    "mpressionLevel_invalid\0""ERROR_headerVersion_wrong\0""ERROR_blockChec"
    "ksum_invalid\0""ERROR_reservedFlag_set\0""ERROR_allocation_failed\0""ERRO"
    "R_srcSize_tooLarge\0""ERROR_dstMaxSize_tooSmall\0""ERROR_frameHeader_i"
    "ncomplete\0""ERROR_frameType_unknown\0""ERROR_frameSize_wrong\0""ERROR_sr"
    "cPtr_wrong\0""ERROR_decompressionFailed\0""ERROR_headerChecksum_invali"
    "d\0""ERROR_contentChecksum_invalid\0""ERROR_frameDecoding_alreadyStart"
    "ed\0""ERROR_maxCode\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0""\0"""
;
static std::uint64_t fn_61493c20(const u8* argument_0,u8* argument_1,std::uint64_t argument_2,std::uint64_t argument_3);
template<class T0,class T1>
requires (std::is_integral_v<T0> && std::is_integral_v<T1>)
static std::uint64_t fn_61493c20(T0 bits_0,T1 bits_1,std::uint64_t bits_2,std::uint64_t bits_3) {
    return fn_61493c20(const_byte_pointer(std::uint64_t(bits_0)),byte_pointer(std::uint64_t(bits_1)),bits_2,bits_3);
}
static std::uint64_t import_614ab520(std::uint64_t a_1,std::uint64_t a_2,std::uint64_t a_8) {
    auto value=address(std::memcpy(pointer(a_1),pointer(a_2),std::size_t(a_8)));
    return value;
}
// Recovered function bodies
static std::uint64_t fn_61493c20(const u8* argument_0,u8* argument_1,std::uint64_t argument_2,std::uint64_t argument_3) {
    // Control flow: structured; 21 loops, 108 conditionals, 72 duplicated blocks; edges validated
    struct alignas(16) FrameStorage {
        std::array<std::byte,128> bytes_0;
        std::array<std::uint32_t,16> stack_array_88;
        std::array<std::byte,72> bytes_c0;
    };
    static_assert(offsetof(FrameStorage,stack_array_88)==128);
    static_assert(sizeof(std::array<std::uint32_t,16>)==64);
    static_assert(sizeof(FrameStorage)>=264);
    alignas(16) FrameStorage frame{};
    // Function SSA: 112 value carriers, 34 joins; 17 private cells promoted; selected-edge parallel transfers.
    // Carriers: 112 -> 74; 118 redundant copies removed.
    // Predicates: 118 uses forwarded; 110 snapshots removed.
    // Lifetimes: 41 reused carriers separated into 52 additional locals.
    bits value_54 = *reinterpret_cast<const u128*>(image_0 + 16);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.stack_array_88.data()) + 0) = value_54;
    bits value_68 = *reinterpret_cast<const u128*>(image_0 + 32);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.stack_array_88.data()) + 16) = value_68;
    bits value_128 = *reinterpret_cast<const u128*>(image_0 + 48);
    std::uint32_t value_58 = argument_3;
    u8* variable_34 = byte_offset(byte_offset(argument_1, static_cast<std::uint64_t>(sext(value_58, 32, 64))), -0x20ULL);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.stack_array_88.data()) + 32) = value_128;
    bits value_139 = *reinterpret_cast<const u128*>(image_0 + 64);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.stack_array_88.data()) + 48) = value_139;
    u8* variable_44 = byte_offset(argument_1, static_cast<std::uint64_t>(sext(value_58, 32, 64)));
    const u8* variable_45 = byte_offset(argument_0, static_cast<std::uint64_t>(sext(static_cast<std::uint32_t>(argument_2), 32, 64)));
    const u8* variable_46 = byte_offset(variable_45, -0x10ULL);
    if(value_58 == 0ULL) {
        if(static_cast<std::uint32_t>(argument_2) != 1ULL) {
            return 0xffffffffULL;
        }
        std::uint8_t value_2566 = *argument_0;
        return static_cast<std::uint32_t>(static_cast<std::uint64_t>(0ULL) - static_cast<std::uint32_t>(value_2566 != 0ULL));
    }
    if(static_cast<std::uint32_t>(argument_2) == 0ULL) {
        return 0xffffffffULL;
    }
    u8* const variable_35 = byte_offset(variable_44, -7ULL);
    u8* variable_36 = byte_offset(variable_44, -5ULL);
    u8* const variable_37 = byte_offset(variable_44, -0x10ULL);
    const u8* variable_38 = byte_offset(variable_45, -5ULL);
    const u8* variable_39 = byte_offset(variable_45, -8ULL);
    const u8* variable_40 = byte_offset(variable_45, -0xfULL);
    u8* variable_47 = byte_offset(variable_44, -0xcULL);
    const u8* variable_0 = argument_0;
    u8* variable_1 = argument_1;
    
    auto shared_region_0 = [](auto& shared_input_0, auto& shared_input_1, auto& shared_input_2, auto& shared_input_3, auto& shared_input_4, auto& shared_input_5) -> void {
        if(address(shared_input_0) < address(shared_input_1)) {
            std::uint64_t shared_local_0 = address(shared_input_2) - address(shared_input_3);
            if(static_cast<std::uint8_t>((static_cast<std::uint8_t>((address(shared_input_0)>=(address(shared_input_4)+0x10ULL))))|(static_cast<std::uint8_t>((address(shared_input_4)>=(address(shared_input_3)+0x18ULL))))) == 0ULL) {
                u8* shared_local_1 = shared_input_0;
                u8* shared_local_2 = shared_input_4;
                u8* shared_local_3;
                do {
                    std::uint64_t shared_local_4 = *reinterpret_cast<const u64*>(shared_local_2);
                    *reinterpret_cast<u64*>(shared_local_1) = shared_local_4;
                    shared_local_3 = byte_offset(shared_local_1, 8ULL);
                    u8* shared_local_5 = byte_offset(shared_local_2, 8ULL);
                    if(address(shared_input_1) > address(shared_local_3)) {
                        shared_local_1 = shared_local_3;
                        shared_local_2 = shared_local_5;
                    }
                } while(address(shared_input_1) > address(shared_local_3));
            } else if(shared_local_0 <= 0x1fULL) {
                u8* shared_local_6 = shared_input_0;
                u8* shared_local_7 = shared_input_4;
                u8* shared_local_8;
                do {
                    std::uint64_t shared_local_9 = *reinterpret_cast<const u64*>(shared_local_7);
                    *reinterpret_cast<u64*>(shared_local_6) = shared_local_9;
                    shared_local_8 = byte_offset(shared_local_6, 8ULL);
                    u8* shared_local_10 = byte_offset(shared_local_7, 8ULL);
                    if(address(shared_input_1) > address(shared_local_8)) {
                        shared_local_6 = shared_local_8;
                        shared_local_7 = shared_local_10;
                    }
                } while(address(shared_input_1) > address(shared_local_8));
            } else {
                std::uint64_t shared_local_11 = (shared_local_0 >> 3ULL) + 1ULL;
                std::uint64_t shared_local_12 = shared_local_11 >> 1ULL << 4ULL;
                std::uint64_t shared_local_13 = 0ULL;
                do {
                    bits shared_local_14 = *reinterpret_cast<const u128*>(byte_offset(shared_input_4, shared_local_13));
                    *reinterpret_cast<u128*>(byte_offset(byte_offset(shared_input_3, shared_local_13), 8ULL)) = shared_local_14;
                    shared_local_13 = shared_local_13 + 0x10ULL;
                } while(shared_local_12 != shared_local_13);
                std::uint64_t shared_local_15 = shared_local_11 & ~1ULL;
                std::uint64_t shared_local_16 = shared_local_15 << 3ULL;
                std::uint64_t shared_local_17 = shared_local_16 + address(shared_input_4);
                std::uint64_t shared_local_18 = address(shared_input_0) + shared_local_16;
                if(shared_local_11 != shared_local_15) {
                    std::uint64_t shared_local_19 = *reinterpret_cast<const u64*>(pointer(shared_local_17));
                    *reinterpret_cast<u64*>(pointer(shared_local_18)) = shared_local_19;
                }
            }
            shared_input_5 = address(shared_input_4) + (address(shared_input_1) - address(shared_input_0));
            shared_input_0 = shared_input_1;
        } else {
            shared_input_5 = address(shared_input_4);
        }
    };
    
    auto shared_region_1 = [&frame](auto& shared_input_6, auto& shared_input_7, auto& shared_input_8, auto& shared_input_9) -> void {
        if(shared_input_6 <= 7ULL) {
            std::uint8_t shared_local_20 = *shared_input_7;
            *shared_input_8 = shared_local_20;
            std::uint8_t shared_local_21 = *byte_offset(shared_input_7, 1ULL);
            *byte_offset(shared_input_8, 1ULL) = shared_local_21;
            std::uint8_t shared_local_22 = *byte_offset(shared_input_7, 2ULL);
            *byte_offset(shared_input_8, 2ULL) = shared_local_22;
            std::uint8_t shared_local_23 = *byte_offset(shared_input_7, 3ULL);
            *byte_offset(shared_input_8, 3ULL) = shared_local_23;
            std::uint32_t shared_local_24 = frame.stack_array_88[shared_input_6];
            std::uint32_t shared_local_25 = *reinterpret_cast<const u32*>(byte_offset(shared_input_7, static_cast<std::uint64_t>(shared_local_24)));
            *reinterpret_cast<u32*>(byte_offset(shared_input_8, 4ULL)) = shared_local_25;
            std::uint32_t shared_local_26 = frame.stack_array_88[(shared_input_6)+8ULL];
            shared_input_9 = byte_offset(byte_offset(shared_input_7, static_cast<std::uint64_t>(shared_local_24)), -std::uint64_t(static_cast<std::uint64_t>(sext(shared_local_26, 32, 64))));
        } else {
            std::uint64_t shared_local_27 = *reinterpret_cast<const u64*>(shared_input_7);
            *reinterpret_cast<u64*>(shared_input_8) = shared_local_27;
            shared_input_9 = byte_offset(shared_input_7, 8ULL);
        }
    };
    
    auto shared_region_2 = [](auto& shared_input_10, auto& shared_input_11, auto& shared_input_12, auto& shared_input_13, auto& shared_input_14, auto& shared_input_15, auto& shared_input_16) -> void {
        std::uint64_t shared_local_28 = shared_input_10 >> 1ULL << 4ULL;
        std::uint64_t shared_local_29 = 0ULL;
        do {
            bits shared_local_30 = *reinterpret_cast<const u128*>(byte_offset(byte_offset(shared_input_11, shared_local_29), 8ULL));
            *reinterpret_cast<u128*>(byte_offset(byte_offset(shared_input_12, shared_local_29), 0x10ULL)) = shared_local_30;
            shared_local_29 = shared_local_29 + 0x10ULL;
        } while(shared_local_28 != shared_local_29);
        std::uint64_t shared_local_31 = shared_input_10 & ~1ULL;
        std::uint64_t shared_local_32 = shared_local_31 << 3ULL;
        std::uint64_t shared_local_33 = address(shared_input_13) + shared_local_32;
        std::uint64_t shared_local_34 = address(shared_input_14) + shared_local_32;
        if(shared_input_10 != shared_local_31) {
            std::uint64_t shared_local_35 = *reinterpret_cast<const u64*>(pointer(shared_local_33));
            *reinterpret_cast<u64*>(pointer(shared_local_34)) = shared_local_35;
        }
        shared_input_15 = shared_input_16;
    };
    
    auto shared_region_3 = [](auto& shared_input_17, auto& shared_input_18, auto& shared_input_19) -> void {
        const u8* shared_local_36 = shared_input_17;
        u8* shared_local_37;
        do {
            std::uint64_t shared_local_38 = *reinterpret_cast<const u64*>(shared_local_36);
            *reinterpret_cast<u64*>(shared_input_19) = shared_local_38;
            shared_local_37 = byte_offset(shared_input_19, 8ULL);
            const u8* shared_local_39 = byte_offset(shared_local_36, 8ULL);
            if(address(shared_input_18) > address(shared_local_37)) {
                shared_local_36 = shared_local_39;
                shared_input_19 = shared_local_37;
            }
        } while(address(shared_input_18) > address(shared_local_37));
    };
    
    auto shared_region_4 = [](auto& shared_input_20, auto& shared_input_21, auto& shared_input_22) -> void {
        u8* shared_local_40;
        do {
            std::uint64_t shared_local_41 = *reinterpret_cast<const u64*>(shared_input_21);
            *reinterpret_cast<u64*>(shared_input_22) = shared_local_41;
            shared_local_40 = byte_offset(shared_input_22, 8ULL);
            u8* shared_local_42 = byte_offset(shared_input_21, 8ULL);
            if(address(shared_input_20) > address(shared_local_40)) {
                shared_input_21 = shared_local_42;
                shared_input_22 = shared_local_40;
            }
        } while(address(shared_input_20) > address(shared_local_40));
    };
    
    while(true) {
        std::uint8_t value_208 = *variable_0;
        std::uint32_t value_209 = value_208;
        std::uint32_t value_214 = value_209 >> 4ULL;
        const u8* variable_19 = byte_offset(variable_0, 1ULL);
        std::uint64_t variable_49 = value_209;
        std::uint64_t variable_50 = value_214;
        const u8* variable_4;
        u8* variable_5;
        std::uint64_t variable_6;
        const u8* variable_55;
        u8* variable_12;
        const u8* variable_15;
        std::uint64_t variable_16;
        if(value_214 != 0xfULL) {
            variable_12 = byte_offset(variable_1, variable_50);
            const u8* variable_58 = byte_offset(variable_19, variable_50);
            if(address(variable_46) > address(variable_19) && address(variable_34) >= address(variable_1)) {
                bits value_754 = *reinterpret_cast<const u128*>(byte_offset(variable_0, 1ULL));
                *reinterpret_cast<u128*>(variable_1) = value_754;
                std::uint16_t value_784 = *reinterpret_cast<const u16*>(variable_58);
                std::uint64_t variable_13 = static_cast<std::uint32_t>(static_cast<std::uint32_t>(variable_49) & 0xfULL);
                variable_0 = byte_offset(variable_58, 2ULL);
                const std::uint64_t variable_14 = static_cast<std::uint32_t>(value_784);
                u8* const variable_11 = byte_offset(variable_12, -std::uint64_t(variable_14));
                if(variable_13 != 0xfULL && variable_14 > 7ULL) {
                    if(address(argument_1) > address(variable_11)) {
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                    std::uint64_t value_906 = *reinterpret_cast<const u64*>(variable_11);
                    *reinterpret_cast<u64*>(variable_12) = value_906;
                    std::uint64_t value_910 = *reinterpret_cast<const u64*>(byte_offset(variable_11, 8ULL));
                    *reinterpret_cast<u64*>(byte_offset(variable_12, 8ULL)) = value_910;
                    std::uint16_t value_914 = *reinterpret_cast<const u16*>(byte_offset(variable_11, 0x10ULL));
                    *reinterpret_cast<u16*>(byte_offset(variable_12, 0x10ULL)) = value_914;
                    variable_1 = byte_offset(byte_offset(variable_12, variable_13), 4ULL);
                    continue;
                }
                if(address(argument_1) > address(variable_11)) {
                    return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                }
                *reinterpret_cast<u32*>(variable_12) = 0ULL;
                if(variable_13 == 0xfULL) {
                    while(true) {
                        const std::uint8_t value_2714 = *variable_0;
                        variable_0 = byte_offset(variable_0, 1ULL);
                        std::uint64_t variable_94 = static_cast<std::uint32_t>(value_2714);
                        if(address(variable_0) > address(variable_38)) {
                            return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                        }
                        const std::uint8_t value_2611 = variable_94;
                        variable_13 = variable_13 + static_cast<std::uint64_t>(static_cast<std::uint32_t>(value_2611));
                        if(value_2611 == 0xffULL) {
                            continue;
                        }
                        u8* const value_4382 = byte_offset(variable_12, variable_13);
                        if((address(value_4382) < address(variable_12)) | (address(value_4382) < variable_13)) {
                            return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                        }
                        variable_15 = variable_0;
                        variable_16 = variable_13;
                        break;
                    }
                } else {
                    variable_15 = variable_0;
                    variable_16 = variable_13;
                }
                std::uint64_t variable_75 = variable_16 + 4ULL;
                variable_1 = byte_offset(variable_12, variable_75);
                u8* variable_17;
                shared_region_1(variable_14, variable_11, variable_12, variable_17);
                if(address(variable_1) > address(variable_47)) {
                    if(address(variable_1) > address(variable_36)) {
                        variable_0 = variable_15;
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                    u8* variable_28 = byte_offset(variable_12, 8ULL);
                    std::uint64_t variable_108;
                    shared_region_0(variable_28, variable_35, variable_37, variable_12, variable_17, variable_108);
                    if(address(variable_1) <= address(variable_28)) {
                        variable_0 = variable_15;
                        continue;
                    }
                    std::uint64_t variable_97 = address(variable_1) - address(variable_28);
                    if(static_cast<std::uint8_t>((static_cast<std::uint8_t>((address(variable_28)>=(variable_108+0x10ULL))))|(static_cast<std::uint8_t>((variable_108>=(address(variable_28)+0x10ULL))))) != 0ULL && address(variable_1) - 1ULL - address(variable_28) > 0xeULL) {
                        std::uint64_t variable_98 = variable_97 & ~0xfULL;
                        std::uint64_t variable_29 = 0ULL;
                        do {
                            const bits value_3149 = *reinterpret_cast<const u128*>(pointer(variable_108 + variable_29));
                            *reinterpret_cast<u128*>(byte_offset(variable_28, variable_29)) = value_3149;
                            variable_29 = variable_29 + 0x10ULL;
                        } while(variable_98 != variable_29);
                        const std::uint64_t value_3214 = variable_97 & ~0xfULL;
                        std::uint64_t variable_100 = variable_108 + value_3214;
                        std::uint64_t variable_101 = address(variable_28) + value_3214;
                        if(value_3214 == variable_97) {
                            variable_0 = variable_15;
                            continue;
                        }
                        for(std::uint64_t copy_index_0 = 0ULL; copy_index_0 < 15ULL; ++copy_index_0) {
                            std::uint8_t copied_byte_0 = *reinterpret_cast<const u8*>(pointer(variable_100 + copy_index_0));
                            *reinterpret_cast<u8*>(pointer(variable_101 + copy_index_0)) = copied_byte_0;
                            if(address(variable_1) <= variable_101 + copy_index_0 + 1ULL) {
                                break;
                            }
                        }
                        variable_0 = variable_15;
                        continue;
                    }
                    std::uint64_t variable_109 = 0ULL;
                    do {
                        const std::uint8_t value_4435 = *reinterpret_cast<const u8*>(pointer(variable_108 + variable_109));
                        *byte_offset(variable_28, variable_109) = value_4435;
                        variable_109 = variable_109 + 1ULL;
                    } while(variable_97 != variable_109);
                } else {
                    const std::uint64_t value_1719 = *reinterpret_cast<const u64*>(variable_17);
                    *reinterpret_cast<u64*>(byte_offset(variable_12, 8ULL)) = value_1719;
                    if(variable_75 <= 0x10ULL) {
                        variable_0 = variable_15;
                        continue;
                    }
                    std::uint64_t variable_41 = 0xffffffffffffffefULL - address(variable_12) + address(variable_1);
                    u8* variable_24 = byte_offset(variable_12, 0x10ULL);
                    std::uint8_t variable_42 = address(variable_24) >= address(variable_17) + 0x18ULL;
                    u8* variable_23 = byte_offset(variable_17, 8ULL);
                    u8* variable_80 = byte_offset(variable_12, 0x11ULL);
                    if(static_cast<std::uint8_t>(variable_42|(static_cast<std::uint8_t>((address(variable_23)>=(address(variable_12)+0x20ULL))))) != 0ULL) {
                        std::uint8_t variable_43 = address(variable_80) <= address(variable_1);
                        if(static_cast<std::uint8_t>(variable_43&(static_cast<std::uint8_t>((variable_41>0x1fULL)))) != 0ULL) {
                            const std::uint64_t variable_81 = (address(variable_80)>address(variable_1)) ? 1ULL : ((variable_41>>3ULL)+1ULL);
                            shared_region_2(variable_81, variable_17, variable_12, variable_23, variable_24, variable_0, variable_15);
                            continue;
                        }
                    }
                    shared_region_4(variable_1, variable_23, variable_24);
                }
                variable_0 = variable_15;
                continue;
            }
            variable_4 = variable_19;
            variable_5 = variable_12;
            variable_6 = variable_50;
            variable_55 = variable_58;
        } else {
            if(address(variable_19) >= address(variable_40)) {
                variable_0 = variable_19;
                return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
            }
            std::uint64_t variable_3 = 0xfULL;
            while(true) {
                std::uint8_t value_344 = *variable_19;
                variable_19 = byte_offset(variable_19, 1ULL);
                std::uint64_t variable_52 = static_cast<std::uint32_t>(value_344);
                variable_3 = variable_3 + variable_52;
                if(address(variable_40) != address(variable_19)) {
                    if(static_cast<std::uint8_t>(variable_52) == 0xffULL) {
                        continue;
                    }
                    variable_5 = byte_offset(variable_1, variable_3);
                    if(address(variable_1) > address(variable_5)) {
                        variable_0 = variable_19;
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                    variable_55 = byte_offset(variable_19, variable_3);
                    if(address(variable_19) > address(variable_55)) {
                        variable_0 = variable_19;
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                } else {
                    variable_5 = byte_offset(variable_1, variable_3);
                    if(address(variable_1) > address(variable_5)) {
                        variable_0 = variable_19;
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                    variable_55 = byte_offset(variable_19, variable_3);
                    if(address(variable_19) > address(variable_55)) {
                        variable_0 = variable_19;
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                }
                variable_4 = variable_19;
                variable_6 = variable_3;
                break;
            }
        }
        if(address(variable_5) <= address(variable_47)) {
            if(address(variable_55) > address(variable_39)) {
                if(address(variable_45) != address(variable_55)) {
                    const u8* variable_19_lifetime_0 = variable_4;
                    variable_0 = variable_19_lifetime_0;
                    return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                }
                if(address(variable_44) < address(variable_5)) {
                    const u8* variable_19_lifetime_1 = variable_4;
                    variable_0 = variable_19_lifetime_1;
                    return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                }
                import_614ab520(address(variable_1), address(variable_4), variable_6);
                return static_cast<std::uint32_t>(static_cast<std::uint64_t>(static_cast<std::uint32_t>(address(variable_5))) - static_cast<std::uint32_t>(address(argument_1)));
            }
            u8* variable_64 = byte_offset(variable_1, 1ULL);
            std::uint64_t variable_65 = ~address(variable_1) + address(variable_5);
            if(static_cast<std::uint8_t>((static_cast<std::uint8_t>((address(variable_4)>=(address(variable_1)+0x10ULL))))|(static_cast<std::uint8_t>((address(variable_1)>=(address(variable_4)+0x10ULL))))) == 0ULL) {
                shared_region_3(variable_4, variable_5, variable_1);
            } else if(static_cast<std::uint8_t>((static_cast<std::uint8_t>((address(variable_64)<=address(variable_5))))&(static_cast<std::uint8_t>((variable_65>0x1fULL)))) == 0ULL) {
                shared_region_3(variable_4, variable_5, variable_1);
            } else {
                std::uint64_t variable_66 = (address(variable_64)>address(variable_5)) ? 1ULL : ((variable_65>>3ULL)+1ULL);
                std::uint64_t variable_67 = variable_66 >> 1ULL << 4ULL;
                std::uint64_t variable_68 = 0ULL;
                do {
                    bits value_1285 = *reinterpret_cast<const u128*>(byte_offset(variable_4, variable_68));
                    *reinterpret_cast<u128*>(byte_offset(variable_1, variable_68)) = value_1285;
                    variable_68 = variable_68 + 0x10ULL;
                } while(variable_68 != variable_67);
                std::uint64_t value_1350 = variable_66 & ~1ULL;
                std::uint64_t value_1372 = value_1350 << 3ULL;
                std::uint64_t variable_69 = address(variable_4) + value_1372;
                std::uint64_t variable_70 = address(variable_1) + value_1372;
                if(variable_66 != value_1350) {
                    std::uint64_t value_1449 = *reinterpret_cast<const u64*>(pointer(variable_69));
                    *reinterpret_cast<u64*>(pointer(variable_70)) = value_1449;
                }
            }
            std::uint16_t value_1452 = *reinterpret_cast<const u16*>(variable_55);
            variable_0 = byte_offset(variable_55, 2ULL);
            const std::uint64_t variable_14_lifetime_0 = static_cast<std::uint32_t>(value_1452);
            u8* const variable_11_lifetime_0 = byte_offset(variable_5, -std::uint64_t(variable_14_lifetime_0));
            std::uint64_t variable_13_lifetime_0 = static_cast<std::uint32_t>(static_cast<std::uint32_t>(variable_49) & 0xfULL);
            variable_12 = variable_5;
            if(address(argument_1) > address(variable_11_lifetime_0)) {
                return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
            }
            *reinterpret_cast<u32*>(variable_12) = 0ULL;
            if(variable_13_lifetime_0 == 0xfULL) {
                while(true) {
                    const std::uint8_t value_2714 = *variable_0;
                    variable_0 = byte_offset(variable_0, 1ULL);
                    std::uint64_t variable_94_lifetime_0 = static_cast<std::uint32_t>(value_2714);
                    if(address(variable_0) > address(variable_38)) {
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                    const std::uint8_t value_2611 = variable_94_lifetime_0;
                    variable_13_lifetime_0 = variable_13_lifetime_0 + static_cast<std::uint64_t>(static_cast<std::uint32_t>(value_2611));
                    if(value_2611 == 0xffULL) {
                        continue;
                    }
                    u8* const value_4382 = byte_offset(variable_12, variable_13_lifetime_0);
                    if((address(value_4382) < address(variable_12)) | (address(value_4382) < variable_13_lifetime_0)) {
                        return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                    }
                    variable_15 = variable_0;
                    variable_16 = variable_13_lifetime_0;
                    break;
                }
            } else {
                variable_15 = variable_0;
                variable_16 = variable_13_lifetime_0;
            }
            std::uint64_t variable_75_lifetime_0 = variable_16 + 4ULL;
            variable_1 = byte_offset(variable_12, variable_75_lifetime_0);
            u8* variable_17_lifetime_0;
            shared_region_1(variable_14_lifetime_0, variable_11_lifetime_0, variable_12, variable_17_lifetime_0);
            if(address(variable_1) > address(variable_47)) {
                if(address(variable_1) > address(variable_36)) {
                    variable_0 = variable_15;
                    return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
                }
                u8* variable_28_lifetime_0 = byte_offset(variable_12, 8ULL);
                std::uint64_t variable_108_lifetime_0;
                shared_region_0(variable_28_lifetime_0, variable_35, variable_37, variable_12, variable_17_lifetime_0, variable_108_lifetime_0);
                if(address(variable_1) <= address(variable_28_lifetime_0)) {
                    variable_0 = variable_15;
                    continue;
                }
                std::uint64_t variable_97_lifetime_0 = address(variable_1) - address(variable_28_lifetime_0);
                if(static_cast<std::uint8_t>((static_cast<std::uint8_t>((address(variable_28_lifetime_0)>=(variable_108_lifetime_0+0x10ULL))))|(static_cast<std::uint8_t>((variable_108_lifetime_0>=(address(variable_28_lifetime_0)+0x10ULL))))) != 0ULL && address(variable_1) - 1ULL - address(variable_28_lifetime_0) > 0xeULL) {
                    std::uint64_t variable_98_lifetime_0 = variable_97_lifetime_0 & ~0xfULL;
                    std::uint64_t variable_29_lifetime_0 = 0ULL;
                    do {
                        const bits value_3149 = *reinterpret_cast<const u128*>(pointer(variable_108_lifetime_0 + variable_29_lifetime_0));
                        *reinterpret_cast<u128*>(byte_offset(variable_28_lifetime_0, variable_29_lifetime_0)) = value_3149;
                        variable_29_lifetime_0 = variable_29_lifetime_0 + 0x10ULL;
                    } while(variable_98_lifetime_0 != variable_29_lifetime_0);
                    const std::uint64_t value_3214 = variable_97_lifetime_0 & ~0xfULL;
                    std::uint64_t variable_100_lifetime_0 = variable_108_lifetime_0 + value_3214;
                    std::uint64_t variable_101_lifetime_0 = address(variable_28_lifetime_0) + value_3214;
                    if(value_3214 == variable_97_lifetime_0) {
                        variable_0 = variable_15;
                        continue;
                    }
                    for(std::uint64_t copy_index_1 = 0ULL; copy_index_1 < 15ULL; ++copy_index_1) {
                        std::uint8_t copied_byte_1 = *reinterpret_cast<const u8*>(pointer(variable_100_lifetime_0 + copy_index_1));
                        *reinterpret_cast<u8*>(pointer(variable_101_lifetime_0 + copy_index_1)) = copied_byte_1;
                        if(address(variable_1) <= variable_101_lifetime_0 + copy_index_1 + 1ULL) {
                            break;
                        }
                    }
                    variable_0 = variable_15;
                    continue;
                }
                std::uint64_t variable_109_lifetime_0 = 0ULL;
                do {
                    const std::uint8_t value_4435 = *reinterpret_cast<const u8*>(pointer(variable_108_lifetime_0 + variable_109_lifetime_0));
                    *byte_offset(variable_28_lifetime_0, variable_109_lifetime_0) = value_4435;
                    variable_109_lifetime_0 = variable_109_lifetime_0 + 1ULL;
                } while(variable_97_lifetime_0 != variable_109_lifetime_0);
            } else {
                const std::uint64_t value_1719 = *reinterpret_cast<const u64*>(variable_17_lifetime_0);
                *reinterpret_cast<u64*>(byte_offset(variable_12, 8ULL)) = value_1719;
                if(variable_75_lifetime_0 <= 0x10ULL) {
                    variable_0 = variable_15;
                    continue;
                }
                std::uint64_t variable_41_lifetime_0 = 0xffffffffffffffefULL - address(variable_12) + address(variable_1);
                u8* variable_24_lifetime_0 = byte_offset(variable_12, 0x10ULL);
                std::uint8_t variable_42_lifetime_0 = address(variable_24_lifetime_0) >= address(variable_17_lifetime_0) + 0x18ULL;
                u8* variable_23_lifetime_0 = byte_offset(variable_17_lifetime_0, 8ULL);
                u8* variable_80_lifetime_0 = byte_offset(variable_12, 0x11ULL);
                if(static_cast<std::uint8_t>(variable_42_lifetime_0|(static_cast<std::uint8_t>((address(variable_23_lifetime_0)>=(address(variable_12)+0x20ULL))))) != 0ULL) {
                    std::uint8_t variable_43_lifetime_0 = address(variable_80_lifetime_0) <= address(variable_1);
                    if(static_cast<std::uint8_t>(variable_43_lifetime_0&(static_cast<std::uint8_t>((variable_41_lifetime_0>0x1fULL)))) != 0ULL) {
                        const std::uint64_t variable_81_lifetime_0 = (address(variable_80_lifetime_0)>address(variable_1)) ? 1ULL : ((variable_41_lifetime_0>>3ULL)+1ULL);
                        shared_region_2(variable_81_lifetime_0, variable_17_lifetime_0, variable_12, variable_23_lifetime_0, variable_24_lifetime_0, variable_0, variable_15);
                        continue;
                    }
                }
                shared_region_4(variable_1, variable_23_lifetime_0, variable_24_lifetime_0);
            }
            variable_0 = variable_15;
            continue;
        }
        if(address(variable_45) != address(variable_55)) {
            const u8* variable_19_lifetime_2 = variable_4;
            variable_0 = variable_19_lifetime_2;
            return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
        }
        if(address(variable_44) < address(variable_5)) {
            const u8* variable_19_lifetime_3 = variable_4;
            variable_0 = variable_19_lifetime_3;
            return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(variable_0) - address(argument_0)));
        }
        import_614ab520(address(variable_1), address(variable_4), variable_6);
        return static_cast<std::uint32_t>(static_cast<std::uint64_t>(static_cast<std::uint32_t>(address(variable_5))) - static_cast<std::uint32_t>(address(argument_1)));
    }
}
// Win64 register arguments plus explicit recovered entry-state inputs; no DLL initialization.
std::uint64_t recovered_export(std::uint64_t a0=0,std::uint64_t a1=0,std::uint64_t a2=0,std::uint64_t a3=0) {
    return fn_61493c20(a0,a1,a2,a3);
}
} // namespace sharp_retro::source
