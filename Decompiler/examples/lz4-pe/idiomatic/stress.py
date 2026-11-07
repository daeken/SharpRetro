#!/usr/bin/env python3
"""Additional deterministic cases; the frozen 509-case comparison stays unchanged."""
import ctypes
import hashlib
import importlib.util
import json
from pathlib import Path
import random

HERE = Path(__file__).resolve().parent
EXAMPLE = HERE.parent
BUILD = EXAMPLE / 'build/idiomatic'


def digest(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def main():
    primary = json.loads((BUILD / 'report.json').read_text())
    if primary['status'] != 'corpus_pass':
        raise AssertionError('Validate the candidate against the frozen corpus first')
    pinned = primary['artifacts_sha256']
    for name, expected in pinned.items():
        if digest(BUILD / name) != expected:
            raise AssertionError(f'Changed validated artifact: {name}')
    spec = importlib.util.spec_from_file_location('published_pe_roundtrip', EXAMPLE / 'roundtrip.py')
    fixture = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(fixture)
    original_cases, library = fixture.cases(BUILD / 'liblz4-source.dylib')
    rng = random.Random(0x1D10A71C)
    cases = []
    def add(data, capacity, expected=None):
        cases.append({'id': f'extra-{len(cases):04}', 'input': data,
                      'capacity': capacity, 'expected': expected})
    # New valid compressed blocks, with both tight and roomy output and a
    # truncated-capacity variant. The compressor supplies bytes, not a candidate.
    for i in range(64):
        size = rng.choice([0,1,4,7,15,16,17,31,32,33,64,127,128,255,256,257,511,512,1024,2048,4096,8192])
        if i % 3:
            period = rng.choice([1,2,3,4,7,8,9,15,16,17,24,31,32,33,64])
            motif = rng.randbytes(period)
            data = (motif * ((size + period - 1) // period))[:size]
        else:
            data = rng.randbytes(size)
        source = ctypes.create_string_buffer(data)
        out = ctypes.create_string_buffer(library.LZ4_compressBound(size))
        length = library.LZ4_compress_default(source, out, size, len(out))
        if length <= 0:
            raise AssertionError('Supplemental input compression failed')
        encoded = out.raw[:length]
        add(encoded, size, data)
        add(encoded, size + 32, data)
        if size:
            add(encoded, size - 1)
    # Mutated/truncated inputs and independent random byte strings exercise
    # rejected paths and their partial writes; no failed case is discarded.
    for i in range(256):
        chosen = rng.choice(original_cases)
        encoded = bytearray(chosen['input'])
        if i % 4 == 0 and encoded:
            encoded[rng.randrange(len(encoded))] ^= 1 << rng.randrange(8)
        elif i % 4 == 1:
            encoded = encoded[:rng.randrange(len(encoded) + 1)]
        elif i % 4 == 2:
            encoded[rng.randrange(len(encoded) + 1):] = rng.randbytes(rng.randrange(1,17))
        else:
            encoded = rng.randbytes(rng.randrange(129))
        add(bytes(encoded), rng.choice([chosen['capacity'], rng.randrange(257)]))
    rows = []
    report = {'status': 'incomplete', 'seed': '0x1d10a71c', 'case_count': len(cases),
              'primary_report_sha256': digest(BUILD / 'report.json'),
              'script_sha256': digest(__file__), 'artifacts_sha256': pinned,
              'comparison': 'Unchanged roundtrip.observe(); exact return and complete output allocation, canaries, repeated original',
              'cases': rows}
    try:
        for case in cases:
            row = {'id': case['id'], 'input_hex': case['input'].hex(), 'capacity': case['capacity']}
            rows.append(row)
            reference = fixture.observe(BUILD / 'pe-reference-x64', case, BUILD / 'liblz4-published-x64.dll')
            repeat = fixture.observe(BUILD / 'pe-reference-x64', case, BUILD / 'liblz4-published-x64.dll')
            native = fixture.observe(BUILD / 'native-arm64', case)
            sanitized = fixture.observe(BUILD / 'sanitized-arm64', case)
            if not reference == repeat == native == sanitized:
                row.update(reference=reference, repeat=repeat, native=native, sanitized=sanitized)
                raise AssertionError(f"Supplemental mismatch: {case['id']}")
            if case['expected'] is not None:
                expected = case['expected']
                if reference[0] != len(expected) or reference[1][:len(expected)*2] != expected.hex():
                    raise AssertionError(f"Independent decoded bytes differ: {case['id']}")
            row.update(matches=True, return_value=reference[0],
                       output_sha256=hashlib.sha256(bytes.fromhex(reference[1])).hexdigest())
            if len(rows) % 64 == 0:
                print(f'Matched {len(rows)}/{len(cases)} supplemental cases', flush=True)
        for name, expected in pinned.items():
            if digest(BUILD / name) != expected:
                raise AssertionError(f'Artifact changed during supplemental run: {name}')
        report['status'] = 'corpus_pass'
    except Exception as exc:
        report.update(status='failed', error=f'{type(exc).__name__}: {exc}')
        raise
    finally:
        (BUILD / 'stress-report.json').write_text(json.dumps(report, indent=2) + '\n')
    print(f'Matched all {len(cases)} additional original-DLL/ARM64/ASan/UBSan cases.')


if __name__ == '__main__':
    main()
