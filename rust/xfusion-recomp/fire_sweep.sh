#!/bin/bash
# fire_sweep.sh — the deterministic sweep fire-loop (corpus → box → split →
# 16-way fire → aggregate). Replaces the manual sequence that cost 3 re-fires
# tonight alone (splitter-args, dot-names, stale-parts) + the pgrep-self-match
# class (pgrep/pkill -f self-matching a heredoc cmdline: this IS the scp-a-script-file form).
#
# Usage: ./fire_sweep.sh <corpus.x64d> [runner=oracle_runner_v8]
# Judgement (WHICH corpus, WHEN the box is free) = the caller's.
# Execution (ship/split/fire/aggregate order + who-file etiquette) = here.
set -e
CORPUS="${1:?usage: fire_sweep.sh <corpus.x64d> [runner]}"
RUNNER="${2:-oracle_runner_v8}"
KEY=~/.ssh/x64-oracle.pem
BOX=ec2-user@ec2-3-230-167-219.compute-1.amazonaws.com
B="$(basename "$CORPUS" .x64d)"

echo "== 1/5 box-idle check (who-file + runner census) =="
BUSY=$(ssh -i $KEY -o StrictHostKeyChecking=no $BOX \
  "cat /tmp/whofile 2>/dev/null; pgrep -x oracle_runner_v8 2>/dev/null | head -1" </dev/null)
if [ -n "$BUSY" ]; then echo "BOX BUSY: $BUSY"; exit 1; fi

echo "== 2/5 gzip + ship =="
gzip -kf1 "$CORPUS"
scp -i $KEY -o StrictHostKeyChecking=no "$CORPUS.gz" $BOX:/tmp/

echo "== 3/5 remote fire-script (file-form; no heredoc self-match) =="
cat > /tmp/${B}_fire.sh <<EOF
#!/bin/bash
set -e
cd /tmp
echo "sweep-$USER $B \$(date -u +%H:%M:%S)" > /tmp/whofile
gunzip -f $B.x64d.gz
python3 split_x64d.py $B.x64d 16 ${B}part
for k in \$(seq -w 0 15); do
  nohup ./$RUNNER ${B}part.\$k > ${B}_\$k.log 2>&1 &
done
wait
grep -h "RESULT" ${B}_*.log | awk '{ok+=\$3; diff+=\$6; rej+=\$9} END {print "$B TOTAL: ok="ok" diff="diff" rej="rej}' > /tmp/${B}_TOTAL.txt
cat /tmp/${B}_TOTAL.txt
rm -f /tmp/whofile /tmp/${B}part.*
EOF
scp -i $KEY -o StrictHostKeyChecking=no /tmp/${B}_fire.sh $BOX:/tmp/

echo "== 4/5 fire (blocking; sweep shards run ~2-8min) =="
ssh -i $KEY -o StrictHostKeyChecking=no $BOX "chmod +x /tmp/${B}_fire.sh && /tmp/${B}_fire.sh" </dev/null

echo "== 5/5 result =="
ssh -i $KEY -o StrictHostKeyChecking=no $BOX "cat /tmp/${B}_TOTAL.txt" </dev/null
