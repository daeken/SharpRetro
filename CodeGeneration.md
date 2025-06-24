# Code Generation System

This document describes the ISA (Instruction Set Architecture) definition language used in SharpRetro for generating CPU emulation code.

## Overview

SharpRetro uses a Lisp-based DSL to define CPU instruction sets. These definitions are processed by CoreArchCompiler to generate C# code for:

- **Disassemblers** - Convert binary instructions to human-readable assembly
- **Interpreters** - Direct execution of instructions
- **Recompilers** - JIT compilation to native code

## ISA File Structure

ISA files use S-expression syntax with the following top-level constructs:

### Instruction Definitions

The structure of `(def)` varies by architecture, reflecting different instruction encoding schemes:

#### Game Boy (DmgDef) - Variable-Length Instructions
```lisp
(def INSTRUCTION-NAME
    "bit pattern"           ; Variable length (8, 16, 24 bits)
    "disassembly format"
    (names (field var) ...)
    (cycles cycle-count)
    (decode-expression)     ; Optional decode phase
    evaluation-expression)
```

**DmgDef Characteristics:**
- **Variable instruction sizes**: 1-3 bytes based on opcode
- **Byte-wise matching**: Each byte has separate mask/match values
- **Field extraction**: Fields extracted from specific bit positions within bytes
- **Cycle counting**: Often conditional based on addressing modes

#### MIPS/AArch64 (MipsDef/Aarch64Def) - Fixed 32-bit Instructions
```lisp
(def INSTRUCTION-NAME
    "32-bit pattern"        ; Always 32 bits
    "disassembly format"
    (names (field var) ...)
    (decode-expression)     ; Optional decode phase
    evaluation-expression)
```

**MipsDef/Aarch64Def Characteristics:**
- **Fixed 32-bit instructions**: All instructions are exactly 32 bits
- **Single mask/match**: One mask and match value for entire instruction
- **Contiguous fields**: Bit fields can span multiple bits but are contiguous
- **Register rewriting**: MipsDef includes automatic register access optimization

#### Common Components
- `INSTRUCTION-NAME`: Identifier for the instruction
- `"bit pattern"`: Binary pattern with field placeholders (spaces ignored)
  - `0`/`1`: Fixed bits that must match
  - `#`: Don't care bits (MIPS/AArch64 only)
  - Letters: Variable fields mapped through `(names)`
- `"disassembly format"`: Template for disassembly output with `$()` interpolation
- `(names ...)`: Maps bit field characters to variable names
- `evaluation-expression`: Code execution logic

### Macro Definitions

```lisp
(defm MACRO-NAME (param1 param2 ...) body-expression)
```

Macros enable code reuse and abstraction. They are expanded at compile time.

## Core Language Features

### Data Types

The DSL supports strongly-typed expressions with compile-time and runtime variants:

- **Integers**: `u1`, `u8`, `u16`, `u32`, `u64` (unsigned), `i8`, `i16`, `i32`, `i64` (signed)
- **Floats**: `f32`, `f64`
- **Booleans**: Results of comparisons and logical operations
- **Vectors**: SIMD vector types
- **Strings**: For disassembly formatting

### Type System

Each expression has a type that can be:
- **Compile-time**: Values known at code generation
- **Runtime**: Values computed during execution

The system automatically promotes types between compile-time and runtime contexts as needed.

### Built-in Operations

#### Arithmetic
```lisp
(+ lhs rhs)     ; Addition
(- lhs rhs)     ; Subtraction  
(* lhs rhs)     ; Multiplication
(/ lhs rhs)     ; Division
(% lhs rhs)     ; Modulo
```

#### Bitwise Operations
```lisp
(& lhs rhs)     ; Bitwise AND
(| lhs rhs)     ; Bitwise OR
(^ lhs rhs)     ; Bitwise XOR
(~ value)       ; Bitwise NOT
(<< value bits) ; Left shift
(>> value bits) ; Right shift
```

#### Comparisons
```lisp
(== lhs rhs)    ; Equality
(!= lhs rhs)    ; Inequality
(< lhs rhs)     ; Less than
(<= lhs rhs)    ; Less than or equal
(> lhs rhs)     ; Greater than
(>= lhs rhs)    ; Greater than or equal
```

#### Type Conversions
```lisp
(cast value type)     ; Type cast with value conversion
(bitcast value type)  ; Bitcast (reinterpret bits)
(signext value type)  ; Sign extend to larger type
```

#### Control Flow
```lisp
(if condition then-expr else-expr)  ; Conditional expression
(match value                        ; Pattern matching
  pattern1 result1
  pattern2 result2
  default-result)
```

#### Variable Binding
```lisp
(let var-name value body...)        ; Single variable binding
(mlet (var1 val1 var2 val2) body...) ; Multiple variable binding
```

### Architecture-Specific Functions

These functions are implemented by the target architecture:

#### Register Access
```lisp
(reg index)           ; Read register
(= (reg index) value) ; Write register
```

#### Memory Operations
```lisp
(load address type)        ; Load from memory
(store address value)      ; Store to memory
```

#### Control Flow
```lisp
(branch target)           ; Unconditional branch
(branch-default)          ; Continue to next instruction
```

#### Special Operations
```lisp
(pc)                      ; Current program counter
(exception type)          ; Raise exception
(halt)                    ; Halt execution
```

## Code Generation Process

1. **Parsing**: ISA files are parsed into abstract syntax trees
2. **Type Checking**: Expressions are type-checked and types inferred
3. **Code Generation**: AST nodes are translated to target C# code
4. **Template Substitution**: Generated code is inserted into stub templates

### Generation Contexts

Code generation behavior changes based on context:

- **Disassembler**: Focuses on formatting and field extraction
- **Interpreter**: Generates direct execution code
- **Recompiler**: Generates JIT-compatible builder calls

## Architecture Examples

### Game Boy (SM83) - Variable Length with Cycles
```lisp
(def LD-rd-rs
    "01 ddd sss"                                    ; 8-bit instruction
    "ld $(reg-name rd), $(reg-name rs)"
    (names (rd d) (rs s))                           ; Maps 'd' bits to 'rd', 's' bits to 'rs'
    (cycles 1)                                      ; Game Boy includes cycle count
    (requires (& (!= rd 0b110) (!= rs 0b110)))     ; Decode phase constraint
    (= (reg rd) (reg rs)))                          ; Evaluation: register-to-register copy
```

### MIPS (R3051) - Fixed 32-bit with Register Rewriting
```lisp
(rtype ADD "100000" "add %$rd, %$rs, %$rt"
    (mlet (lhs (reg rs)                             ; MipsDef automatically handles
           rhs (reg rt))                            ; register access optimization
        (check-overflow-add lhs rhs)               ; Architecture-specific overflow check
        (= (reg rd) (+ lhs rhs))))                  ; Evaluation: addition with overflow
```

### Architecture-Specific Differences

#### Bit Pattern Encoding
- **Game Boy**: Variable length, space-separated bytes: `"01 ddd sss"` (8 bits), `"11001011 00000rrr"` (16 bits)
- **MIPS/AArch64**: Fixed 32-bit patterns: `"000000 sssss ttttt ddddd aaaaa 100000"`

#### Field Extraction
- **Game Boy**: Fields can span bit boundaries within bytes, tracked per-byte with shift/size
- **MIPS/AArch64**: Contiguous bit fields extracted as single units from 32-bit word

#### Instruction Matching
- **Game Boy**: Multiple byte-wise mask/match pairs for variable-length instructions
- **MIPS/AArch64**: Single 32-bit mask/match pair for entire instruction

#### Special Processing
- **Game Boy**: No special register handling
- **MIPS**: Automatic register read/write optimization and load delay slot handling
- **AArch64**: Minimal processing (inherits MIPS structure)

## Extension Points

The system supports extension through:

1. **Builtin Classes**: Add new operations by extending `Builtin`
2. **Architecture Definitions**: Implement arch-specific parsing and helpers
3. **Code Templates**: Customize generated code structure
4. **Type System**: Add new type kinds and conversions

## Limitations

**NOT part of core language:**
- Dynamic memory allocation
- Function definitions (use macros instead)
- Object-oriented features
- Exception handling beyond basic exceptions
- File I/O operations
- Complex data structures (arrays, maps)
- Recursive definitions
- First-class functions
- Closures or lexical scoping beyond local bindings

The DSL is intentionally minimal and focused on instruction-level operations for efficient code generation.