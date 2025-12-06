from capstone import *

buckets = {}
with open('log', 'r') as fp:
	for line in fp:
		if ': Invalid instruction' not in line:
			continue
		line = line.strip()
		addr, insn = line.split(': Invalid instruction ', 1)
		addr = int(addr, 16)
		insn = bytes([int(x, 16) for x in insn.split(' ')])
		md = Cs(CS_ARCH_ARM64, CS_MODE_ARM)
		for i in md.disasm(insn, addr):
			if i.mnemonic != 'udf':
				if i.mnemonic not in buckets:
					buckets[i.mnemonic] = []
				buckets[i.mnemonic].append('0x%x:\t%s\t%s' %(i.address, i.mnemonic, i.op_str))

for mnem, insns in buckets.items():
	print('%s:' % mnem)
	for insn in insns:
		print('\t%s' % insn)
