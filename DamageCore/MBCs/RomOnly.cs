using System.Diagnostics;

namespace DamageCore.MBCs; 

public class RomOnly : ICartridge {
	public readonly byte[] Data;

	public RomOnly(byte[] data) {
		Debug.Assert(data.Length == 0x8000);
		Data = data;
	}

	public void Write(ushort addr, byte value) =>
		Console.WriteLine($"Write to ROM-only cartridge! Address 0x{addr:X04}, value 0x{value:X02}");

	public byte Read(ushort addr) => Data[addr & 0x7FFF];
}