#pragma once
#include "IntegerRuntime.hpp"
namespace sharp_retro {
struct State {
    std::uint64_t gpr[16]{};
    std::uint64_t pc = 0;
    std::uint64_t flags = 2;
    std::uint64_t defined_flags = ~std::uint64_t(0);
    std::uint64_t segments[6]{}; // supplied segment bases, not selectors
};
inline bits read_flags(const State& s, int bit) {
    std::uint64_t selected = bit < 0 ? ~std::uint64_t(0) : std::uint64_t(1) << bit;
    if((s.defined_flags & selected) != selected)
        throw std::runtime_error("read of architecturally undefined flags");
    return bit < 0 ? s.flags : (s.flags >> bit) & 1;
}
inline void write_flag(State& s, int bit, bits value) {
    if(bit < 0) { s.flags = std::uint64_t(value) | 2; s.defined_flags = ~std::uint64_t(0); }
    else {
        std::uint64_t selected = std::uint64_t(1) << bit;
        s.flags = (s.flags & ~selected) | (std::uint64_t(value & 1) << bit);
        s.defined_flags |= selected;
    }
}
struct Memory {
    virtual bits load(std::uint64_t address, unsigned width) = 0;
    virtual void store(std::uint64_t address, bits value, unsigned width) = 0;
    virtual bool invoke_import(const char*, State&) { return false; }
    virtual ~Memory() = default;
};
enum class Exit { returned, boundary, budget_exhausted };
} // namespace sharp_retro
