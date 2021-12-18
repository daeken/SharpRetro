using DamageCore.MBCs;

namespace DamageCore; 

public interface ICartridge {
	void Write(ushort addr, byte value);
	byte Read(ushort addr);

	public static ICartridge Load(byte[] data) => data[0x0147] switch {
		0x00 => new RomOnly(data),
		var mbc => throw new NotSupportedException($"MBC 0x{mbc:X02} unsupported")
	};
}