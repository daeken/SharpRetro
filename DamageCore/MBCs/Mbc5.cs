using LibSharpRetro;

namespace DamageCore.MBCs; 

public class Mbc5 : ICartridge {
	public readonly byte[] Data;
	public readonly byte[] Eram = new byte[0x20000];

	public readonly bool HasRam, HasBattery;
	
	bool EramEnabled;
	int RomBank => RomBankLower | (RomBankUpper << 8);
	byte RomBankLower = 1;
	byte RomBankUpper;
	int RamBank = 0;

	public Mbc5(byte[] data, bool hasRam = false, bool hasBattery = false) {
		HasRam = hasRam;
		HasBattery = hasBattery;
		Data = data;
	}

	public void Write(ushort addr, byte value) {
		switch(addr) {
			case < 0x2000: EramEnabled = HasRam && (value & 0b111) == 0x0A; break;
			case < 0x3000: RomBankLower = value; break;
			case < 0x4000: RomBankUpper = (byte) (value & 1); break;
			case < 0x6000: RamBank = value & 0xF; break;
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