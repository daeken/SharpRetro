# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SharpRetro is a multi-architecture emulation framework written in C# (.NET 10.0) that supports multiple CPU architectures and emulation approaches. The project implements:

- **Game Boy emulation** (DamageCore) - Complete Game Boy core with CPU, PPU, APU, and memory systems
- **PlayStation emulation** (SharpStationCore) - PlayStation core with MIPS R3051 CPU emulation  
- **AArch64 support** (Aarch64*) - ARM64 CPU emulation capabilities
- **Multiple JIT backends** - CIL-based JIT, LLVM-based JIT, and static recompilation
- **Code generation framework** (CoreArchCompiler) - ISA-driven code generation for interpreters, disassemblers, and recompilers

## Architecture

### Core Components

- **CoreArchCompiler**: Domain-specific language and compiler for generating CPU emulation code from ISA definitions
- **JitBase/CilJit/LlvmJit**: Abstract JIT interface with CIL and LLVM concrete implementations  
- **StaticRecompilerBase**: Static recompilation framework for ahead-of-time compilation
- **LibSharpRetro**: Common interfaces and utilities shared across all cores

### Emulation Cores

- **DamageCore**: Game Boy emulator with interpreter and recompiler support
- **SharpStationCore**: PlayStation emulator with MIPS CPU core
- **Aarch64Cpu**: ARM64 CPU emulation (in development)

### Code Generation

The project uses `.isa` files (Instruction Set Architecture definitions) to automatically generate:
- Disassemblers (`*Generator` projects generate disassembly code)
- Interpreters (direct instruction execution)  
- Recompilers (JIT compilation to native code)

ISA files are processed by generator projects to produce C# code in `Generated/` directories.

## Build Commands

```bash
# Build entire solution
dotnet build SharpRetro.sln

# Build specific project
dotnet build <ProjectName>/<ProjectName>.csproj

# Build in Release mode
dotnet build SharpRetro.sln -c Release
```

## Test Commands

```bash
# Run JIT tests
dotnet test JitTests/JitTests.csproj

# Run XFusion tests  
dotnet test XFusionTests/XFusionTests.csproj

# Run all tests
dotnet test SharpRetro.sln
```

## Code Generation Workflow

When modifying ISA definitions:

1. Edit the `.isa` file in the appropriate `*Generator` project
2. Run the generator: `dotnet run --project <GeneratorProject>`
3. Generated code appears in the corresponding core's `Generated/` directory
4. Build the core project to compile the generated code

Example:
```bash
# After editing DamageGenerator/sm83.isa
dotnet run --project DamageGenerator
dotnet build DamageCore
```

## Project Dependencies

- **Target Framework**: .NET 10.0 with preview language features
- **Key Dependencies**: 
  - DoubleSharp (LINQ extensions)
  - PrettyPrinter (code formatting)  
  - UltimateOrb.Int128 (128-bit integer support)
  - NUnit (testing framework)
  - Microsoft.CodeAnalysis (for source generators)

## Development Notes

- All projects use `AllowUnsafeBlocks=true` for performance-critical emulation code
- Source generators are used extensively for code generation (SourceGenerators project)
- Generated code should not be manually edited - modify ISA definitions instead
- JIT implementations provide runtime code generation capabilities
- Static recompiler enables ahead-of-time compilation for better performance