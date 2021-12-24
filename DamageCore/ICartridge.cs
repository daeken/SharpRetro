using DamageCore.MBCs;

namespace DamageCore; 

public interface ICartridge {
	void Write(ushort addr, byte value);
	byte Read(ushort addr);

	public static ICartridge Load(byte[] data) => data[0x0147] switch {
		0x00 => new RomOnly(data),
		0x01 or 0x02 or 0x03 => new Mbc1(data),
		0x13 => new Mbc3(data), 
		0x19 => new Mbc5(data), 
		0x1A => new Mbc5(data, hasRam: true), 
		0x1B => new Mbc5(data, hasRam: true, hasBattery: true), 
		var mbc => throw new NotSupportedException($"MBC 0x{mbc:X02} unsupported")
	};
}