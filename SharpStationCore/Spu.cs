namespace SharpStationCore; 

public class SPU {
	[Port(0x1F801C00, 24, 0x10)] static ushort[] VoiceVolumeLeft;
	[Port(0x1F801C02, 24, 0x10)] static ushort[] VoiceVolumeRight;
	[Port(0x1F801C04, 24, 0x10)] static ushort[] VoiceSampleRate;
	[Port(0x1F801C06, 24, 0x10)] static ushort[] VoiceStartAddress;
	[Port(0x1F801C08, 24, 0x10)] static ushort[] VoiceAdsrLo;
	[Port(0x1F801C0A, 24, 0x10)] static ushort[] VoiceAdsrHi;
	[Port(0x1F801C0C, 24, 0x10)] static ushort[] VoiceAdsrCurrentVolume;
	[Port(0x1F801C0E, 24, 0x10)] static ushort[] VoiceAdpcmRepeatAddress;
	
	[Port(0x1F801D80)] static ushort MainVolumeLeft;
	[Port(0x1F801D82)] static ushort MainVolumeRight;
	[Port(0x1F801D84)] static ushort ReverbOutputVolumeLeft;
	[Port(0x1F801D86)] static ushort ReverbOutputVolumeRight;

	[Port(0x1F801D88)] static uint KeyOn; // Supposed to be write-only but read by BIOS??
	
	[Port(0x1F801D88)] static ushort VoiceKeyOnLo; // Supposed to be write-only but read by BIOS??
	[Port(0x1F801D8A)] static ushort VoiceKeyOnHi; // Supposed to be write-only but read by BIOS??
	[Port(0x1F801D8C)] static ushort KeyOffLo; // Supposed to be write-only but read by BIOS??
	[Port(0x1F801D8E)] static ushort KeyOffHi;

	[Port(0x1F801D90)] static void ChannelFMModeLo(ushort v) {}
	[Port(0x1F801D92)] static void ChannelFMModeHi(ushort v) {}

	[Port(0x1F801D94)] static void ChannelNoiseModeLo(ushort v) {}
	[Port(0x1F801D96)] static void ChannelNoiseModeHi(ushort v) {}

	[Port(0x1F801D98)] static void ChannelReverbModeLo(ushort v) {}
	[Port(0x1F801D9A)] static void ChannelReverbModeHi(ushort v) {}

	[Port(0x1F801DA2)] static ushort RamReverbWorkAreaStartAddress;
	[Port(0x1F801DA4)] static ushort RamIrqAddress;
	[Port(0x1F801DA6)] static ushort RamDataTransferAddress;
	[Port(0x1F801DA8)] static ushort RamDataTransferFifo;
	[Port(0x1F801DAA)] static ushort Control;
	[Port(0x1F801DAC)] static ushort RamDataTransferControl;
	[Port(0x1F801DAE)] static ushort Status => 0;
	
	[Port(0x1F801DB0)] static ushort CdVolumeLeft;
	[Port(0x1F801DB2)] static ushort CdVolumeRight;
	[Port(0x1F801DB4)] static ushort ExternVolumeLeft;
	[Port(0x1F801DB6)] static ushort ExternVolumeRight;
	
	[Port(0x1F801DC0)] static ushort ReverbApfOffset1;
	[Port(0x1F801DC2)] static ushort ReverbApfOffset2;
	[Port(0x1F801DC4)] static ushort ReverbReflectionVolume1;
	[Port(0x1F801DC6)] static ushort ReverbCombVolume1;
	[Port(0x1F801DC8)] static ushort ReverbCombVolume2;
	[Port(0x1F801DCA)] static ushort ReverbCombVolume3;
	[Port(0x1F801DCC)] static ushort ReverbCombVolume4;
	[Port(0x1F801DCE)] static ushort ReverbReflectionVolume2;
	[Port(0x1F801DD0)] static ushort ReverbApfVolume1;
	[Port(0x1F801DD2)] static ushort ReverbApfVolume2;
	[Port(0x1F801DD4)] static ushort ReverbSameSideReflectionAddress1Left;
	[Port(0x1F801DD6)] static ushort ReverbSameSideReflectionAddress1Right;
	[Port(0x1F801DD8)] static ushort ReverbCombAddress1Left;
	[Port(0x1F801DDA)] static ushort ReverbCombAddress1Right;
	[Port(0x1F801DDC)] static ushort ReverbCombAddress2Left;
	[Port(0x1F801DDE)] static ushort ReverbCombAddress2Right;
	[Port(0x1F801DE0)] static ushort ReverbSameSideReflectionAddress2Left;
	[Port(0x1F801DE2)] static ushort ReverbSameSideReflectionAddress2Right;
	[Port(0x1F801DE4)] static ushort ReverbDifferentSideReflectionAddress1Left;
	[Port(0x1F801DE6)] static ushort ReverbDifferentSideReflectionAddress1Right;
	[Port(0x1F801DE8)] static ushort ReverbCombAddress3Left;
	[Port(0x1F801DEA)] static ushort ReverbCombAddress3Right;
	[Port(0x1F801DEC)] static ushort ReverbCombAddress4Left;
	[Port(0x1F801DEE)] static ushort ReverbCombAddress4Right;
	[Port(0x1F801DF0)] static ushort ReverbDifferentSideReflectionAddress2Left;
	[Port(0x1F801DF2)] static ushort ReverbDifferentSideReflectionAddress2Right;
	[Port(0x1F801DF4)] static ushort ReverbApfAddress1Left;
	[Port(0x1F801DF6)] static ushort ReverbApfAddress1Right;
	[Port(0x1F801DF8)] static ushort ReverbApfAddress2Left;
	[Port(0x1F801DFA)] static ushort ReverbApfAddress2Right;
	[Port(0x1F801DFC)] static ushort ReverbInputVolumeLeft;
	[Port(0x1F801DFE)] static ushort ReverbInputVolumeRight;
}
