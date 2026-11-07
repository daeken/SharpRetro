# Recovering unrolled byte-copy loops

The published LZ4 decoder contains two 15-byte copy staircases: copy a byte,
test the next destination address, then either leave or copy another byte.
A final positive test guards the last copy. These are bounded forward loops;
a switch with fallthrough would preserve most of the repetition.

`CppByteCopyLoops` runs after value, lifetime, scope and loop-header analyses,
before helper extraction and final presentation. It recognizes 3–64 consecutive
byte copies with matching source/destination bases and literal offsets starting
at zero. Each byte temporary must have only its declaration and store use.
Every stop test must compare the same end address against the next destination
offset. Exits must contain exactly the same terminal continuation.

The generated structure is:

```cpp
for (std::uint64_t copy_index = 0; copy_index < 15; ++copy_index) {
    std::uint8_t byte = *reinterpret_cast<const u8*>(pointer(source + copy_index));
    *reinterpret_cast<u8*>(pointer(destination + copy_index)) = byte;
    if (end <= destination + copy_index + 1)
        break;
}
// Original transfer assignments and return/break/continue stay here.
```

The cap comes from the unrolled copies, not an assumed input length. Arithmetic
stays unsigned 64-bit; no subtraction-derived count assumes a nonwrapping range.
The new final bound comparison is a total expression reading only unescaped,
nonvolatile primitive bindings. It adds no memory access or externally visible
effect. Unknown types, changed bounds/offsets/exits, exposed address roots,
volatile memory and live byte temporaries retain the original structure.
Lexical bindings are tracked separately even when names or immutable AST nodes
repeat in disjoint scopes. New loop identifiers cannot capture existing names.

Each iteration loads before storing. This matters for overlapping LZ4 matches:
later loads can consume bytes written earlier in the same copy. Neither `memcpy`
nor a snapshot-style `memmove` is a substitute for this behavior. Original outer
`break`, `continue` and `return` statements stay outside the inserted loop.

The compiled regression harness compares original/reconstructed trees with an
independent sequential-copy oracle, checking complete buffers, ordered access
addresses, committed exits and return values. It covers both final-guard forms,
negated condition metadata, overlapping buffers, unsigned address wraparound,
native pointer roots, exit ownership, repeated lexical bindings and conservative
refusals. Logical wraparound cases use a mapped byte arena so no invalid host
address is dereferenced. Native-pointer cases use actual in-bounds allocations.
The generated tests compile with strict warnings and ASan/UBSan.

The complete published-DLL output falls from 871 lines / 56,979 bytes to 717 lines
/ 46,458 bytes, replacing both 15-byte staircases. All 181 decompiler test methods
pass with no skips. The same 509 original Windows
DLL comparisons pass on ARM64 and ASan/UBSan; the independent source oracle agrees
and the mutation control remains rejected. This is evidence for the tested export
and cases, not a whole-game or universal equivalence proof.

`examples/lz4-pe/build/copy-loops-summary.json` binds current source, implementation,
regression and raw-binary evidence. The previous source/report checkpoint remains
under `before-copy-loops-*`.
