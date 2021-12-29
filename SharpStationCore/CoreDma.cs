using LibSharpRetro;
using static SharpStationCore.Globals;

namespace SharpStationCore; 

enum SyncMode {
	Manual, 
	Request, 
	LinkedList
}

class DmaChannel {
	readonly int Channel;
	
	// Global DMA reg fields
	public uint Priority, Base;
	public bool Enable, IrqEnable, IrqFlag;
	
	// BlockControl fields
	uint TransferSize, Count;

	// ChannelControl fields
	bool FromRam, StepBackward, Chopping;
	SyncMode SyncMode;
	uint DmaWindowSize, CpuWindowSize;
	bool ControlEnable, ManualTrigger;
	
	public uint BlockControl {
		get => TransferSize | (Count << 16);
		set {
			TransferSize = value & 0xFFFF;
			Count = value >> 16;
		}
	}
	
	public uint ChannelControl {
		get =>
			FromRam.ToBit(0) | 
			StepBackward.ToBit(1) | 
			Chopping.ToBit(2) | 
			((uint) SyncMode << 9) | 
			(DmaWindowSize << 16) | 
			(CpuWindowSize << 20) | 
			ControlEnable.ToBit(24) | 
			ManualTrigger.ToBit(28);
		set {
			FromRam = value.HasBit(0);
			StepBackward = value.HasBit(1);
			Chopping = value.HasBit(2);
			SyncMode = (SyncMode) ((value >> 9) & 3);
			DmaWindowSize = (value >> 16) & 7;
			CpuWindowSize = (value >> 20) & 7;
			ControlEnable = value.HasBit(24);
			ManualTrigger = value.HasBit(28);
			TryTransfer();
		}
	}

	public DmaChannel(int channel) => Channel = channel;

	void Assign(Action func) {
		func();
		TryTransfer();
	}

	void TryTransfer() {
		if(!ControlEnable || SyncMode == SyncMode.Manual && !ManualTrigger)
			return;
		//if(Channel == 3) $"Trying transfer on channel {Channel} with base address {Base:X8} -- 0x{TransferSize * 4:X}".Debug();
		
		if(SyncMode == SyncMode.LinkedList) TransferLinkedList();
		else TransferBlocks();

		Done();
	}

	void TransferBlocks() {
		var increment = unchecked((uint) (StepBackward ? -4U : 4));
		var addr = Base;

		var size = TransferSize;
		while(size-- > 0) {
			var maddr = addr & 0x1FFFFC;
			if(FromRam) {
				var src = Memory.Load32(maddr);
				switch(Channel) {
					case 2:
						throw new NotImplementedException();//Gpu.Gp0Incoming(src);
						break;
					default: throw new NotSupportedException($"Transfer from RAM to channel {Channel}");
				}
			} else {
				uint src;
				switch(Channel) {
					case 2:
						throw new NotImplementedException();//src = Gpu.Read;
						break;
					case 3:
						throw new NotImplementedException();
						//src = Cdrom.DataFifo.Dequeue() | ((uint) Cdrom.DataFifo.Dequeue() << 8) |
						//      ((uint) Cdrom.DataFifo.Dequeue() << 16) | ((uint) Cdrom.DataFifo.Dequeue() << 24);
						break;
					case 6:
						src = size == 0 ? 0xFFFFFFU : unchecked(addr - 4) & 0x1FFFFF;
						break;
					default: throw new NotSupportedException($"Transfer to RAM from channel {Channel}");
				}
				Memory.Store32(maddr, src);
			}

			addr = unchecked(addr + increment);
		}
	}

	void TransferLinkedList() {
		var addr = Base & 0x1FFFFC;
		if(!FromRam) throw new NotImplementedException("Linked list transfer to RAM");
		if(Channel != 2) throw new NotSupportedException("Linked list transfer on non-GPU channel");

		while(true) {
			var header = Memory.Load32(addr);
			var size = header >> 24;
			while(size-- > 0) {
				addr = (addr + 4) & 0x1FFFFC;
				throw new NotImplementedException();//Gpu.Gp0Incoming(Memory.Load32(addr));
			}
			if((header & 0x00800000) != 0)  break;
			addr = header & 0x1FFFFC;
		}
	}

	void Done() {
		ControlEnable = ManualTrigger = false;
		//if(IrqEnable && !IrqFlag && Dma.MasterEnable) Irq.Assert(IrqType.DMA, true);
	}
}

public class CoreDma {
	readonly DmaChannel[] Channels = Enumerable.Range(0, 7).Select(i => new DmaChannel(i)).ToArray();

	public bool MasterEnable = true;
	bool ForceIrq;

	public CoreDma() {
		Control = 0x07654321;
	}

	[Port(0x1F8010F0)]
	uint Control {
		get => Channels.Select((x, i) => (x.Enable ? 1U << (i * 4 + 3) : 0) | (x.Priority << (i * 4))).Aggregate((a, b) => a | b);
		set => Channels.ForEach((x, i) => {
			x.Enable = ((value >> (i * 4 + 3)) & 1) == 1;
			x.Priority = (value >> (i * 4)) & 7;
		});
	}

	[Port(0x1F8010F4)]
	uint Interrupt {
		get {
			var value = 0U;
			var masterFlag = ForceIrq;
			for(var i = 0; i < 7; ++i) {
				var channel = Channels[i];
				if(channel.IrqEnable) {
					value |= 1U << (16 + i);
					if(channel.IrqFlag) {
						value |= 1U << (24 + i);
						masterFlag = true;
					}
				}
				
			}
			if(ForceIrq) value |= 1U << 15;
			if(MasterEnable) value |= 1U << 23;
			if(masterFlag) value |= 1U << 31;
			return value;
		}
		set {
			for(var i = 0; i < 7; ++i) {
				var channel = Channels[i];
				channel.IrqEnable = value.HasBit(16 + i);
				if(value.HasBit(24 + i))
					channel.IrqFlag = false;
			}
			ForceIrq = value.HasBit(15);
			MasterEnable = value.HasBit(23);
		}
	}

	[Port(0x1F801080, 7, 0x10)] uint GetBase(int channel) => Channels[channel].Base;
	[Port(0x1F801080, 7, 0x10)] void SetBase(int channel, uint value) => Channels[channel].Base = value;

	[Port(0x1F801084, 7, 0x10)] uint GetBlockControl(int channel) => Channels[channel].BlockControl;
	[Port(0x1F801084, 7, 0x10)] void SetBlockControl(int channel, uint value) => Channels[channel].BlockControl = value;

	[Port(0x1F801088, 7, 0x10)] uint GetChannelControl(int channel) => Channels[channel].ChannelControl;
	[Port(0x1F801088, 7, 0x10)] void SetChannelControl(int channel, uint value) => Channels[channel].ChannelControl = value;
}