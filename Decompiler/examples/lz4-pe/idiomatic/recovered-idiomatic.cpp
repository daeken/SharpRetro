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
// These copies deliberately retain the recovered access widths and forward
// load/store order. They may write beyond the logical end of a match or literal.
static void copy_eight_bytes(const u8* source, u8* destination) {
    const std::uint64_t word = *reinterpret_cast<const u64*>(source);
    *reinterpret_cast<u64*>(destination) = word;
}

static void copy_sixteen_bytes(const u8* source, u8* destination) {
    const bits word = *reinterpret_cast<const u128*>(source);
    *reinterpret_cast<u128*>(destination) = word;
}

// At least one eight-byte transfer, including when end <= destination.
static void copy_eight_until(const u8* source, u8* destination, const u8* end) {
    do {
        copy_eight_bytes(source, destination);
        destination = byte_offset(destination, 8ULL);
        source = byte_offset(source, 8ULL);
    } while(address(end) > address(destination));
}

// The callers establish that there is at least one sixteen-byte group.
static void copy_wide_groups(const u8* source, u8* destination,
                             std::uint64_t eight_byte_count) {
    const std::uint64_t paired_bytes = (eight_byte_count >> 1ULL) << 4ULL;
    std::uint64_t offset = 0;
    do {
        copy_sixteen_bytes(byte_offset(source, offset), byte_offset(destination, offset));
        offset += 16ULL;
    } while(offset != paired_bytes);

    const std::uint64_t paired_count = eight_byte_count & ~1ULL;
    if(eight_byte_count != paired_count) {
        const std::uint64_t tail_offset = paired_count << 3ULL;
        copy_eight_bytes(byte_offset(source, tail_offset),
                         byte_offset(destination, tail_offset));
    }
}

static void copy_literals(const u8* source, u8* destination, u8* literal_end) {
    const u8* const next_byte = byte_offset(destination, 1ULL);
    const std::uint64_t bytes_to_last = ~address(destination) + address(literal_end);
    const bool separated = address(source) >= address(destination) + 16ULL ||
                           address(destination) >= address(source) + 16ULL;
    if(!separated || address(next_byte) > address(literal_end) || bytes_to_last <= 31ULL) {
        copy_eight_until(source, destination, literal_end);
        return;
    }
    const std::uint64_t count = address(next_byte) > address(literal_end)
        ? 1ULL : (bytes_to_last >> 3ULL) + 1ULL;
    copy_wide_groups(source, destination, count);
}

// Seed the match with exactly the recovered byte/word transfers. Small distances
// depend on the stores becoming visible to subsequent reads in this sequence.
static u8* copy_match_seed(std::uint64_t distance, u8* source, u8* destination,
                          const std::array<std::uint32_t,16>& adjustments) {
    if(distance > 7ULL) {
        copy_eight_bytes(source, destination);
        return byte_offset(source, 8ULL);
    }
    for(std::uint64_t offset = 0; offset < 4ULL; ++offset) {
        const std::uint8_t byte = *byte_offset(source, offset);
        *byte_offset(destination, offset) = byte;
    }
    const std::uint32_t advance = adjustments[distance];
    const std::uint32_t word = *reinterpret_cast<const u32*>(byte_offset(source, advance));
    *reinterpret_cast<u32*>(byte_offset(destination, 4ULL)) = word;
    const std::uint32_t retreat = adjustments[distance + 8ULL];
    return byte_offset(byte_offset(source, advance),
                       -std::uint64_t(sext(retreat, 32, 64)));
}

struct CopyPosition {
    u8* destination;
    const u8* source;
};

static CopyPosition copy_match_to_safe_limit(u8* match_start, u8* destination,
                                            const u8* source, u8* output_end) {
    u8* const limit = byte_offset(output_end, -7ULL);
    if(address(destination) >= address(limit)) return {destination, source};

    const std::uint64_t span = address(byte_offset(output_end, -16ULL)) - address(match_start);
    const bool separated = address(destination) >= address(source) + 16ULL ||
                           address(source) >= address(match_start) + 24ULL;
    if(!separated || span <= 31ULL) {
        copy_eight_until(source, destination, limit);
    } else {
        copy_wide_groups(source, byte_offset(match_start, 8ULL), (span >> 3ULL) + 1ULL);
    }
    return {limit, byte_offset(source, address(limit) - address(destination))};
}

static void copy_match_tail(const u8* source, u8* destination, u8* match_end) {
    if(address(match_end) <= address(destination)) return;
    const std::uint64_t remaining = address(match_end) - address(destination);
    const bool separated = address(destination) >= address(source) + 16ULL ||
                           address(source) >= address(destination) + 16ULL;
    if(separated && address(match_end) - 1ULL - address(destination) > 14ULL) {
        const std::uint64_t vector_bytes = remaining & ~15ULL;
        std::uint64_t offset = 0;
        do {
            copy_sixteen_bytes(byte_offset(source, offset), byte_offset(destination, offset));
            offset += 16ULL;
        } while(offset != vector_bytes);
        if(vector_bytes == remaining) return;

        const u8* const tail_source = byte_offset(source, vector_bytes);
        u8* const tail_destination = byte_offset(destination, vector_bytes);
        for(std::uint64_t index = 0; index < 15ULL; ++index) {
            const std::uint8_t byte = *byte_offset(tail_source, index);
            *byte_offset(tail_destination, index) = byte;
            if(address(match_end) <= address(tail_destination) + index + 1ULL) break;
        }
        return;
    }
    std::uint64_t offset = 0;
    do {
        const std::uint8_t byte = *byte_offset(source, offset);
        *byte_offset(destination, offset) = byte;
        ++offset;
    } while(offset != remaining);
}

static std::uint64_t fn_61493c20(const u8* input, u8* output,
                               std::uint64_t input_size, std::uint64_t output_capacity) {
    struct alignas(16) FrameStorage {
        std::array<std::byte,128> bytes_0;
        std::array<std::uint32_t,16> adjustments;
        std::array<std::byte,72> bytes_c0;
    };
    static_assert(offsetof(FrameStorage,adjustments) == 128);
    static_assert(sizeof(std::array<std::uint32_t,16>) == 64);
    static_assert(sizeof(FrameStorage) >= 264);
    alignas(16) FrameStorage frame{};
    bits table_word = *reinterpret_cast<const u128*>(image_0 + 16);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.adjustments.data())) = table_word;
    table_word = *reinterpret_cast<const u128*>(image_0 + 32);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.adjustments.data()) + 16) = table_word;
    table_word = *reinterpret_cast<const u128*>(image_0 + 48);
    const std::uint32_t capacity_bits = output_capacity;
    u8* const fast_output_limit = byte_offset(
        byte_offset(output, std::uint64_t(sext(capacity_bits, 32, 64))), -32ULL);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.adjustments.data()) + 32) = table_word;
    table_word = *reinterpret_cast<const u128*>(image_0 + 64);
    *reinterpret_cast<u128*>(reinterpret_cast<u8*>(frame.adjustments.data()) + 48) = table_word;

    u8* const output_end = byte_offset(output, std::uint64_t(sext(capacity_bits, 32, 64)));
    const u8* const input_end = byte_offset(input, std::uint64_t(sext(std::uint32_t(input_size), 32, 64)));
    const u8* const fast_input_limit = byte_offset(input_end, -16ULL);
    if(capacity_bits == 0) {
        if(std::uint32_t(input_size) != 1) return 0xffffffffULL;
        const std::uint8_t byte = *input;
        return std::uint32_t(0ULL - std::uint32_t(byte != 0));
    }
    if(std::uint32_t(input_size) == 0) return 0xffffffffULL;

    u8* const match_copy_limit = byte_offset(output_end, -12ULL);
    u8* const last_match_end = byte_offset(output_end, -5ULL);
    const u8* const match_extension_limit = byte_offset(input_end, -5ULL);
    const u8* const literal_read_limit = byte_offset(input_end, -8ULL);
    const u8* const literal_extension_limit = byte_offset(input_end, -15ULL);
    const auto error_at = [input](const u8* cursor) -> std::uint32_t {
        return ~std::uint32_t(address(cursor) - address(input));
    };

    const u8* input_cursor = input;
    u8* output_cursor = output;
    while(true) {
        const std::uint8_t token = *input_cursor;
        std::uint64_t literal_length = std::uint32_t(token) >> 4ULL;
        const u8* literal_source = byte_offset(input_cursor, 1ULL);
        u8* literal_end;
        const u8* offset_field;
        const bool extended_literals = literal_length == 15ULL;
        if(extended_literals) {
            if(address(literal_source) >= address(literal_extension_limit)) return error_at(literal_source);
            while(true) {
                const std::uint8_t extension = *literal_source;
                literal_source = byte_offset(literal_source, 1ULL);
                literal_length += extension;
                if(address(literal_source) != address(literal_extension_limit) && extension == 255) continue;
                literal_end = byte_offset(output_cursor, literal_length);
                if(address(output_cursor) > address(literal_end)) return error_at(literal_source);
                offset_field = byte_offset(literal_source, literal_length);
                if(address(literal_source) > address(offset_field)) return error_at(literal_source);
                break;
            }
        } else {
            literal_end = byte_offset(output_cursor, literal_length);
            offset_field = byte_offset(literal_source, literal_length);
        }

        const bool fast_literals = !extended_literals &&
            address(fast_input_limit) > address(literal_source) &&
            address(fast_output_limit) >= address(output_cursor);
        if(fast_literals) {
            copy_sixteen_bytes(literal_source, output_cursor);
        } else {
            if(address(literal_end) > address(match_copy_limit) ||
               address(offset_field) > address(literal_read_limit)) {
                if(address(input_end) != address(offset_field)) return error_at(literal_source);
                if(address(output_end) < address(literal_end)) return error_at(literal_source);
                import_614ab520(address(output_cursor), address(literal_source), literal_length);
                return std::uint32_t(std::uint64_t(std::uint32_t(address(literal_end))) -
                                     std::uint32_t(address(output)));
            }
            copy_literals(literal_source, output_cursor, literal_end);
        }

        const std::uint16_t distance_word = *reinterpret_cast<const u16*>(offset_field);
        input_cursor = byte_offset(offset_field, 2ULL);
        const std::uint64_t distance = distance_word;
        u8* const match_source = byte_offset(literal_end, -distance);
        std::uint64_t match_length = std::uint32_t(token) & 15ULL;
        if(fast_literals && match_length != 15ULL && distance > 7ULL) {
            if(address(output) > address(match_source)) return error_at(input_cursor);
            copy_eight_bytes(match_source, literal_end);
            copy_eight_bytes(byte_offset(match_source, 8ULL), byte_offset(literal_end, 8ULL));
            const std::uint16_t word = *reinterpret_cast<const u16*>(byte_offset(match_source, 16ULL));
            *reinterpret_cast<u16*>(byte_offset(literal_end, 16ULL)) = word;
            output_cursor = byte_offset(byte_offset(literal_end, match_length), 4ULL);
            continue;
        }
        if(address(output) > address(match_source)) return error_at(input_cursor);

        // The zero store precedes extension validation, including error returns.
        *reinterpret_cast<u32*>(literal_end) = 0;
        if(match_length == 15ULL) {
            while(true) {
                const std::uint8_t extension = *input_cursor;
                input_cursor = byte_offset(input_cursor, 1ULL);
                if(address(input_cursor) > address(match_extension_limit)) return error_at(input_cursor);
                match_length += extension;
                if(extension == 255) continue;
                u8* const extended_end = byte_offset(literal_end, match_length);
                if((address(extended_end) < address(literal_end)) ||
                   (address(extended_end) < match_length)) return error_at(input_cursor);
                break;
            }
        }
        match_length += 4ULL;
        output_cursor = byte_offset(literal_end, match_length);
        u8* const next_match_source = copy_match_seed(distance, match_source, literal_end, frame.adjustments);
        if(address(output_cursor) > address(match_copy_limit)) {
            // The seed writes happen even if the match end is rejected here.
            if(address(output_cursor) > address(last_match_end)) return error_at(input_cursor);
            const CopyPosition tail = copy_match_to_safe_limit(
                literal_end, byte_offset(literal_end, 8ULL), next_match_source, output_end);
            copy_match_tail(tail.source, tail.destination, output_cursor);
            continue;
        }

        copy_eight_bytes(next_match_source, byte_offset(literal_end, 8ULL));
        if(match_length <= 16ULL) continue;
        const std::uint64_t bytes_to_last = 0xffffffffffffffefULL - address(literal_end) + address(output_cursor);
        u8* const destination = byte_offset(literal_end, 16ULL);
        u8* const source = byte_offset(next_match_source, 8ULL);
        const u8* const next_byte = byte_offset(literal_end, 17ULL);
        const bool separated = address(destination) >= address(next_match_source) + 24ULL ||
                               address(source) >= address(literal_end) + 32ULL;
        if(separated && address(next_byte) <= address(output_cursor) && bytes_to_last > 31ULL) {
            const std::uint64_t count = address(next_byte) > address(output_cursor)
                ? 1ULL : (bytes_to_last >> 3ULL) + 1ULL;
            copy_wide_groups(source, destination, count);
        } else {
            copy_eight_until(source, destination, output_cursor);
        }
    }
}
// Win64 register arguments plus explicit recovered entry-state inputs; no DLL initialization.
std::uint64_t recovered_export(std::uint64_t a0=0,std::uint64_t a1=0,std::uint64_t a2=0,std::uint64_t a3=0) {
    return fn_61493c20(a0,a1,a2,a3);
}
} // namespace sharp_retro::source
