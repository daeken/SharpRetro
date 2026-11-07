#!/usr/bin/env python3
"""Check a separate agent rewrite using the unchanged published-PE fixture."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path
import sys

HERE = Path(__file__).resolve().parent
EXAMPLE = HERE.parent
BASELINE = HERE / 'recovered-input.cpp'
BASELINE_SHA256 = 'b40eba292c0446559fabb53397e7a2ebc05a3d1d782431154ab60b7028a76099'
MARKER = '// Recovered function bodies'
ADAPTER = '// Win64 register arguments plus explicit recovered entry-state inputs; no DLL initialization.'


def digest(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--candidate', type=Path, default=HERE / 'recovered-idiomatic.cpp')
    parser.add_argument('--build-dir', type=Path, default=EXAMPLE / 'build/idiomatic')
    args = parser.parse_args()
    candidate, directory = args.candidate.resolve(), args.build_dir.resolve()
    # This experiment cannot overwrite the normal generated checkpoint or input.
    if directory in (BASELINE.parent, EXAMPLE / 'build') or directory in candidate.parents or directory == candidate:
        parser.error('Use a separate build directory outside the candidate source directory tree')
    directory.mkdir(parents=True, exist_ok=True)
    report_path = directory / 'report.json'
    report = {'status': 'incomplete', 'experiment': 'Recovered-C++-only agent rewrite',
              'candidate_path': str(candidate), 'baseline_path': str(BASELINE),
              'command': [sys.executable, str(Path(__file__).resolve()), *sys.argv[1:]],
              'comparator': 'Unmodified parent roundtrip.py verify()',
              'scope': 'Published LZ4 export corpus, not universal or whole-game equivalence'}
    try:
        if digest(BASELINE) != BASELINE_SHA256:
            raise AssertionError('The frozen recovered input changed')
        baseline, rewritten = BASELINE.read_bytes(), candidate.read_bytes()
        before = baseline.decode(); after = rewritten.decode()
        if before.count(MARKER) != 1 or after.count(MARKER) != 1:
            raise AssertionError('Expected exactly one recovered-body marker')
        if before.split(MARKER)[0] != after.split(MARKER)[0]:
            raise AssertionError('Runtime prelude or declarations changed')
        if before.count(ADAPTER) != 1 or after.count(ADAPTER) != 1 or before.split(ADAPTER)[1] != after.split(ADAPTER)[1]:
            raise AssertionError('Entry adapter changed')
        report.update(baseline_sha256=digest(BASELINE), candidate_sha256=digest(candidate),
                      validator_sha256=digest(__file__), comparator_sha256=digest(EXAMPLE / 'roundtrip.py'))
        spec = importlib.util.spec_from_file_location('published_pe_roundtrip', EXAMPLE / 'roundtrip.py')
        fixture = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(fixture)
        # Build the existing reference/source runners and retain fresh baseline
        # generation as provenance. Only the candidate compilation input changes.
        artifacts = fixture.build(directory)
        if artifacts['cpp'].read_bytes() != baseline:
            raise AssertionError('Fresh generated source differs from the frozen experiment input')
        (directory / 'generated-baseline.cpp').write_bytes(artifacts['cpp'].read_bytes())
        artifacts['cpp'].write_bytes(rewritten)
        for name, flags in (('native', ['-O2']), ('sanitized', ['-O1', '-fsanitize=address,undefined'])):
            fixture.tool('clang++', '-arch', 'arm64', '-std=c++20', *flags,
                         f'-DRECOVERED_CPP="{artifacts["cpp"]}"', EXAMPLE / 'driver.cpp', '-o', artifacts[name])
        result = fixture.verify(artifacts)
        if digest(candidate) != report['candidate_sha256'] or digest(BASELINE) != report['baseline_sha256']:
            raise AssertionError('Input source changed during validation')
        report.update(result)
        report['baseline_generation_diagnostics'] = report.pop('diagnostics')
        report['status'] = 'corpus_pass'
        report['artifacts_sha256'] = {p.name: digest(p) for p in
            [artifacts['cpp'], artifacts['pe'], artifacts['reference'], artifacts['native'],
             artifacts['sanitized'], artifacts['source'], artifacts['library'],
             directory / 'mutant.cpp', directory / 'mutant-arm64', directory / 'generated-baseline.cpp']}
        print(f"Matched {result['case_count']} original Windows-DLL cases on ARM64 and ASan/UBSan; source oracle agrees; mutation rejected.")
    except Exception as exc:
        report.update(status='failed', error=f'{type(exc).__name__}: {exc}')
        raise
    finally:
        report_path.write_text(json.dumps(report, indent=2) + '\n')
        print(f'Report: {report_path}')


if __name__ == '__main__':
    main()
