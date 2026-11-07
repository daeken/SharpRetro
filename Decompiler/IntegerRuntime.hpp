#pragma once
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
