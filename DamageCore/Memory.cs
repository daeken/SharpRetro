namespace DamageCore; 

public class Memory {
	public readonly Core Core;
	public readonly ICartridge Cartridge;

	public readonly byte[] VRam = new byte[0x2000];
	public readonly byte[] WRam = new byte[0x2000];
	public readonly byte[] HRam = new byte[0x7F];
	public readonly byte[] Oam = new byte[0xA0];

	public Memory(Core core, ICartridge cartridge) {
		Core = core;
		Cartridge = cartridge;
	}

	public void Write(ushort addr, byte value) {
		switch(addr) {
			case <= 0x7FFF or >= 0xA000 and <= 0xBFFF:
				Cartridge.Write(addr, value);
				break;
			case <= 0x9FFF:
				VRam[addr - 0x8000] = value;
				break;
			case <= 0xDFFF: // TODO: Add bank switching for CGB mode
				WRam[addr - 0xC000] = value;
				break;
			case <= 0xFDFF:
				Write((ushort) (addr - 0x2000), value);
				break;
			case <= 0xFE9F:
				Oam[addr - 0xFE00] = value;
				break;
			case <= 0xFEFF:
				break;
			case <= 0xFF7F or 0xFFFF:
				Core.IoWrite(addr, value);
				break;
			case <= 0xFFFE:
				HRam[addr - 0xFF80] = value;
				break;
		}
	}

	public byte Read(ushort addr) => addr switch {
		<= 0x7FFF or >= 0xA000 and <= 0xBFFF => Cartridge.Read(addr), 
		<= 0x9FFF => VRam[addr - 0x8000],
		<= 0xDFFF => WRam[addr - 0xC000],
		<= 0xFDFF => Read((ushort) (addr - 0x2000)),
		<= 0xFE9F => Oam[addr - 0xFE00],
		<= 0xFEFF => 0,
		<= 0xFF7F or 0xFFFF => Core.IoRead(addr),
		<= 0xFFFE => HRam[addr - 0xFF80]
	};

	public byte[] ReadBlock(ushort addr, int length) =>
		Enumerable.Range(0, length).Select(i => Read((ushort) (addr + i))).ToArray();

	public void Write16(ushort addr, ushort value) {
		Write(addr, (byte) (value & 0xFF));
		Write((ushort) (addr + 1), (byte) (value >> 8));
	}

	public ushort Read16(ushort addr) =>
		(ushort) (Read(addr) | (Read((ushort) (addr + 1)) << 8));
}