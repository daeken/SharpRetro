#!/usr/bin/env bash
# (T6)×72: canonical legoworlds runner. Per sera kt[12]×27
# (·11349 "make a runner script, instead of writing the
# command line each time") — the structural fix for kt[40]
# (a): (W') was ×62-×71 ~7.5h n=9 stuck on a wrong args[1]
# reconstructed from filesystem-find. The script IS the
# canonical invocation; calling it = no reconstruction.
#
# Usage:
#   ./run-lego.sh u791                    # default: live, QUIET, setarch -R, 420s
#   ./run-lego.sh u791 --cap nvncap6k     # + capture to /tmp/nvncap6k
#   ./run-lego.sh u791 --no-quiet         # full per-SVC log (1M+ lines)
#   ./run-lego.sh u791 --no-aslr-pin      # don't setarch -R
#   ./run-lego.sh u791 --bin /tmp/umbra-X # alternate UmbraCli build
#   ./run-lego.sh u791 --fg               # foreground (don't nohup)
#   ./run-lego.sh u791 --timeout 600
#   UMBRA_T3=1 ./run-lego.sh u791         # extra env passes through
#
# Logs to /tmp/<label>.log. Echoes the FULL command before
# firing (= the [boot]-log discipline applied at the script
# layer too).
set -euo pipefail

LABEL="${1:?usage: run-lego.sh <label> [opts]}"
shift

# ── canonical invocation (= the thing kt[40] says to encode) ──
SO=/tmp/legoworlds.so
ROMFS=/tmp/legoworlds-romfs/romfs   # ⚠ NOT the parent dir (= (W'))
BIN="UmbraCli/bin/Debug/net10.0/UmbraCli.dll"
TIMEOUT=420
QUIET=1
ASLR_PIN=1
CAP=""
ON3D="12,200,50"
FG=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    --cap)         CAP="/tmp/$2"; shift 2 ;;
    --on3d)        ON3D="$2"; shift 2 ;;
    --no-quiet)    QUIET=0; shift ;;
    --no-aslr-pin) ASLR_PIN=0; shift ;;
    --bin)         BIN="$2"; shift 2 ;;
    --timeout)     TIMEOUT="$2"; shift 2 ;;
    --fg)          FG=1; shift ;;
    *) echo "unknown opt: $1" >&2; exit 1 ;;
  esac
done

# ── canonical env (= the FULL u779 invocation per kt[40](a)) ──
# Per (T6)×17×5(E) freq-table (own NOTES@10673 — past-me at
# ×17 had this exact reconstruction problem; the runner-script
# is the structural fix for that recursion = own ·8808-PROPER
# ×17th @ ×72): u761→u779 chain had QUIET + SHIM_PAD + VATTR_HOOK
# + HID + LEGO_DIAG + C33_PATCH_W21 + C33_FORCE_SPEED + T3.
# (W') @×71 was the missing romfs-path; ×72×1(b) found the
# missing UMBRA_HID (= 1-press → past title-screen → cutscene
# spawns → [c33] fires → 3D → ON3D triggers → capture).
ENV=(
  UMBRA_LEGO_DIAG=1
  UMBRA_C33_PATCH_W21=1
  UMBRA_C33_FORCE_SPEED=1
  UMBRA_SHIM_PAD=1
  UMBRA_VATTR_HOOK=1
  UMBRA_T3=1
  "UMBRA_HID=${UMBRA_HID:-A@d480-d490}"
)
[[ $QUIET -eq 1 ]] && ENV+=(UMBRA_QUIET=1)
if [[ -n "$CAP" ]]; then
  mkdir -p "$CAP"
  ENV+=(UMBRA_NVNCAP="$CAP" UMBRA_NVNCAP_ON3D="$ON3D")
fi

PRE=()
[[ $ASLR_PIN -eq 1 ]] && PRE=(setarch aarch64 -R)

LOG="/tmp/${LABEL}.log"
cd "$(dirname "$0")"

# ── echo the FULL invocation (= kt[40](a) at script-layer) ──
echo "═══ ${LABEL}: $(date -u +%H:%M:%SZ) ═══" | tee -a "$LOG"
echo "  env: ${ENV[*]}" | tee -a "$LOG"
echo "  cmd: ${PRE[*]} dotnet $BIN $SO $ROMFS" | tee -a "$LOG"
echo "  log: $LOG  timeout: ${TIMEOUT}s" | tee -a "$LOG"

CMD=(env "${ENV[@]}" timeout -k 5 -s TERM "$TIMEOUT" "${PRE[@]}" dotnet "$BIN" "$SO" "$ROMFS")

if [[ $FG -eq 1 ]]; then
  exec "${CMD[@]}" >> "$LOG" 2>&1
else
  nohup "${CMD[@]}" >> "$LOG" 2>&1 &
  echo "  pid: $!"
fi
