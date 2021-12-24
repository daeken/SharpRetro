using System.Diagnostics;
using LibSharpRetro;

namespace DamageCore.MBCs; 

public class Mbc3 : ICartridge {
	public readonly byte[] Data;
	public readonly byte[] Eram = new byte[0x8000];

	bool EramEnabled;
	int RomBank = 1;
	int RamBank = 0;

	public Mbc3(byte[] data) {
		Data = data;
	}

	public void Write(ushort addr, byte value) {
		switch(addr) {
			case < 0x2000: EramEnabled = value == 0x0A; break;
			case < 0x4000:
				RomBank = value & 0x1F;
				if(RomBank == 0x00) RomBank++;
				break;
			case < 0x6000:
				RamBank = value;
				break;
			case >= 0xA000 and <= 0xBFFF when EramEnabled && RamBank < 4:
				Eram[addr - 0xA000] = value;
				break;
		}
	}

	public byte Read(ushort addr) => addr switch {
		<= 0x3FFF           => Data[addr], 
		<= 0x7FFF           => Data[0x4000 * RomBank + (addr & 0x3FFF)],
		_ when !EramEnabled => 0xFF,
		_ when RamBank < 4  => Eram[0x2000 * RamBank + (addr & 0x1FFF)], 
		_ => 0xFF
	};
}