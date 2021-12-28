using static SharpStationCore.Globals;

namespace SharpStationCore; 

public static class MemoryControl {
	[Port(0x1F801000)] static uint Expansion1BaseAddress; // usually 1F000000h
	[Port(0x1F801004)] static uint Expansion2BaseAddress; // usually 1F802000h
	[Port(0x1F801008)] static uint Expansion1DelaySize;   // usually 0013243Fh; 512Kbytes 8bit-bus
	[Port(0x1F80100C)] static uint Expansion3DelaySize;   // usually 00003022h; 1 byte
	[Port(0x1F801010)] static uint BiosRomDelaySize;      // usually 0013243Fh; 512Kbytes 8bit-bus
	[Port(0x1F801014)] static uint SpuDelaySize;          // usually 200931E1h
	[Port(0x1F801018)] static uint CdromDelaySize;        // usually 00020843h or 00020943h
	[Port(0x1F80101C)] static uint Expansion2DelaySize;   // usually 00070777h; 128-bytes 8bit-bus
	[Port(0x1F801020)] static uint ComDelay;              // usually 00031125h or 0000132Ch or 00001325h
	[Port(0x1F801060)] static uint RamSize;               // usually 00000B88h; 2MB RAM mirrored in first 8MB

	[Port(0xFFFE0130)]
	static uint BIU {
		get => Cpu.BIU;
		set => Cpu.BIU = value;
	}
}