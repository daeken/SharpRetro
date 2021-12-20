using System.Diagnostics;
using LibSharpRetro;

namespace DamageCore.MBCs; 

public class Mbc1 : ICartridge {
	public readonly byte[] Data;
	public readonly byte[] Eram = new byte[0x8000];

	bool EramEnabled, RamBanking;
	int RomBank = 1;
	int RamBank = 0;

	public Mbc1(byte[] data) {
		Data = data;
	}

	public void Write(ushort addr, byte value) {
		switch(addr) {
			case < 0x2000: EramEnabled = value == 0x0A; break;
			case < 0x4000:
				RomBank = value & 0x1F;
				if(RomBank is 0x00 or 0x20 or 0x40 or 0x60) RomBank++;
				break;
			case < 0x6000:
				if(RamBanking)
					RamBank = value & 3;
				else {
					RomBank |= value & 3;
					if(RomBank is 0x00 or 0x20 or 0x40 or 0x60) RomBank++;
				}
				break;
			case < 0x8000:
				RamBanking = value.HasBit(0);
				break;
			case >= 0xA000 and <= 0xBFFF when EramEnabled:
				Eram[addr - 0xA000] = value;
				break;
		}
	}

	public byte Read(ushort addr) => addr switch {
		<= 0x3FFF           => Data[addr], 
		<= 0x7FFF           => Data[0x4000 * RomBank + (addr & 0x3FFF)],
		_ when !EramEnabled => 0xFF,
		_                   => Eram[0x2000 * RamBank + (addr & 0x1FFF)]
	};
}