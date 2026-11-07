# Readability review

Three implementation/reviewer rounds focused on clarity of the emitted native
C++, with the existing raw-binary comparisons and strict compiler checks.

## Round 1: declarations

`CppReadability` runs after the value, scope and shared-region analyses. It drops
`maybe_unused` from used single bindings, and removes top-level primitive scalar
or pointer-object `const` when no CV-sensitive use is present. Pointee qualifiers
remain. References, opaque/generic reference calls, address exposure, constant
expressions, type introspection, arrays and ambiguous repeated bindings constrain
the cleanup. Unused loads, calls and initialization conversions remain evaluated
as declarations; genuinely unused bindings retain their diagnostic attribute.

The exact pure `frame_bytes` prelude alias is omitted when unused. Native entry
adapters allocate the empty environment only when a recovered input needs it.

## Round 2: helper locality

Native output now asks the shared-region pass to repay the cost of a helper's
interface: its body must have at least `max(8, 2*parameter_count+2)` printed
statements/structure lines, in addition to the existing positive savings proof.
Tiny snippets stay inline. Larger copying regions remain shared. Helper definitions
appear just before the first containing function-level sibling that can call them,
after the input early returns where possible. Captures already exist at that point;
definitions are not moved into loops or new lifetime scopes.

## Round 3: expressions

Assignments, returns and understood operators use consistent spacing and C++
precedence. Known primitive casts and runtime value calls can format their argument
expressions recursively, preserving type text, argument order and comma arity.
Unknown calls/templates, literals and comments retain opaque expression syntax.
The recursion depth is bounded. Value formatting keeps numeric `!!x`, exact inner
conversions, right-hand grouping and unordered floating comparisons. Diagnostic
parentheses around additions inside shifts remain explicit. Dynamic negative byte
offsets convert to `uint64_t` before negation, preserving modular address arithmetic.

For example:

```cpp
// Before
[[maybe_unused]] const std::uint64_t length = value+4ULL;
return (static_cast<std::uint32_t>(~(static_cast<std::uint32_t>((address(cursor)-address(input))))));
// After
std::uint64_t length = value + 4ULL;
return static_cast<std::uint32_t>(~static_cast<std::uint32_t>(address(cursor) - address(input)));
```

## Measured output

| Complete published-LZ4 output | Before | After |
| --- | ---: | ---: |
| Lines | 859 | 871 |
| Bytes | 58,547 | 56,979 |
| `maybe_unused` markers in recovered bodies | 103 | 0 |
| `const` tokens in recovered bodies | 218 | 128 |
| Shared helpers | 8 | 5 |

The line count grows because small fragments are inline and helper definitions
have visual separation. Source length is not the clarity criterion. Input checks
now precede helper definitions. Read-only source types and typed load casts remain.

All three rounds were independently reviewed. All 180 decompiler test methods pass
with no skips. The same 509 original published-DLL
cases pass ARM64 and ASan/UBSan, along with the source oracle and mutation control.
`examples/lz4-pe/build/readability-summary.json` binds the final regression outcome,
baseline/stage/source identities, implementation, compiler outputs and logs.

Twenty-four compiled presentation scenarios include overloaded/generic references,
address exposure, type/template/array uses, repeated bindings, unused effects,
write-only locals, numeric bool conversion, NaNs, shift/right grouping and comma
expressions. Existing source-shape assertions accept the new whitespace and
pointer-object qualification; runtime expectations and comparison policy stay fixed.

The reviewer recommends deferring eager boolean/cast idioms to explicit zero-or-one
provenance analysis. Domain names and higher-level algorithm recovery also remain
separate from this formatting pass.
