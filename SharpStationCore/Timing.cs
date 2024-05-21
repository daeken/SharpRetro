using LibSharpRetro;
using static SharpStationCore.Globals;

namespace SharpStationCore; 

public enum Sync {
	Pause, 
	Reset, 
	ResetAndPause, 
	WaitForSync
}

public enum Clock {
	SysClock, 
	SysClockDiv8, 
	GpuDotClock, 
	GpuHSync
}

public struct FracCycles {
	const int FracBits = 16;
	public static FracCycles FromCycles(ulong cycles) => new() { FP = cycles << FracBits };
	public static FracCycles FromDouble(double cycles) => new() { FP = (ulong) (cycles * (1UL << FracBits)) };
	
	public ulong FP;
	
	public static FracCycles operator +(FracCycles a, FracCycles b) => new() { FP = a.FP + b.FP };
	public static FracCycles operator /(FracCycles a, FracCycles b) => new() { FP = (a.FP << FracBits) / b.FP };

	public uint Ceil() => (uint) ((FP + ((1UL << FracBits) - 1)) >> FracBits);
}

public class Timer {
	readonly int Index;
	public Timer(int index) => Index = index;

	Sync Sync;
	bool UseSync, TargetWrap, TargetIrq, WrapIrq, RepeatIrq, NegateIrq;
	uint ClockSource;
	Clock Clock;
	FracCycles Period, Phase;

	public uint Counter, Target;
	public bool TargetReached, OverflowReached, Interrupt;

	static readonly Clock[,] Lookup = {
		{ Clock.SysClock, Clock.GpuDotClock, Clock.SysClock, Clock.GpuDotClock }, 
		{ Clock.SysClock, Clock.GpuHSync, Clock.SysClock, Clock.GpuHSync }, 
		{ Clock.SysClock, Clock.SysClock, Clock.SysClockDiv8, Clock.SysClockDiv8 }
	};
	public uint CounterMode {
		get {
			var v = UseSync.ToBit(0) |
			        ((uint) Sync << 1) |
			        TargetWrap.ToBit(3) |
			        TargetIrq.ToBit(4) |
			        WrapIrq.ToBit(5) |
			        RepeatIrq.ToBit(6) |
			        NegateIrq.ToBit(7) |
			        (ClockSource << 8) |
			        Interrupt.ToBit(10) |
			        TargetReached.ToBit(11) |
			        OverflowReached.ToBit(12);
			TargetReached = OverflowReached = false;
			return v;
		}
		set {
			UseSync = value.HasBit(0);
			Sync = (Sync) ((value >> 1) & 3);
			TargetWrap = value.HasBit(3);
			TargetIrq = value.HasBit(4);
			WrapIrq = value.HasBit(5);
			RepeatIrq = value.HasBit(6);
			NegateIrq = value.HasBit(7);
			ClockSource = (value >> 8) & 3;
			Clock = Lookup[Index, ClockSource];
			if(Interrupt) Irq.Assert(IrqType.Timer0 + Index, false);
			Interrupt = false;
			Counter = 0;

			Phase = FracCycles.FromCycles(0);
			switch(Clock) {
				case Clock.SysClock:
					Period = FracCycles.FromCycles(1);
					break;
				case Clock.SysClockDiv8:
					Period = FracCycles.FromCycles(8);
					break;
				case Clock.GpuDotClock:
					//Period = Gpu.DotclockPeriod();
					// TODO: Phase
					break;
				case Clock.GpuHSync:
					//Period = Gpu.HSyncPeriod();
					// TODO: Phase
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			SetupEvent();
		}
	}

	ulong LastTimestamp;
	void SetupEvent() {
		if(!TargetIrq) return;

		var countdown = Counter <= Target ? Target - Counter : 0xFFFF - Counter + Target;

		var delta = Period.FP * (countdown + 1);
		delta -= Phase.FP;
		var deltac = new FracCycles { FP = delta }.Ceil();
		LastTimestamp = Timestamp;
		Events.Add(Timestamp + deltac, Update);
	}

	void Update() {
		if(LastTimestamp == Timestamp) return;
		var delta = FracCycles.FromCycles(Timestamp - LastTimestamp);
		var ticks = delta + Phase;
		var count = ticks.FP / Period.FP;
		Phase = new() { FP = ticks.FP % Period.FP };
		count += Counter;

		var targetPassed = false;
		if(Counter <= Target && count > Target)
			targetPassed = TargetReached = true;

		var wrap = TargetWrap ? Target + 1UL : 0x10000;
		var overflow = false;
		if(count >= wrap) {
			count %= wrap;
			if(wrap == 0x10000)
				overflow = OverflowReached = true;
		}

		Counter = (uint) count;
		if(WrapIrq && overflow || TargetIrq && targetPassed) {
			if(NegateIrq) throw new NotImplementedException();
			Irq.Assert(IrqType.Timer0 + Index, true);
			Interrupt = true;
		} else if(!NegateIrq)
			Interrupt = false;

		LastTimestamp = Timestamp;

		SetupEvent();
	}

	public T UpdateThen<T>(Func<Timer, T> func) {
		Update();
		return func(this);
	}
}

public static class Timing {
	static readonly Timer[] Timers = [new(0), new(1), new(2)];
	
	[Port(0x1F801100, 3, 0x10)]
	static uint CurrentValue(int timer) => Timers[timer].UpdateThen(x => x.Counter);
	[Port(0x1F801100, 3, 0x10, debug: true)]
	static void CurrentValue(int timer, uint value) => Timers[timer].Counter = value;

	[Port(0x1F801104, 3, 0x10)]
	static uint CounterMode(int timer) => Timers[timer].UpdateThen(x => x.CounterMode);
	[Port(0x1F801104, 3, 0x10, debug: true)]
	static void CounterMode(int timer, uint value) => Timers[timer].CounterMode = value;

	[Port(0x1F801108, 3, 0x10)]
	static uint CounterTargetValue(int timer) => Timers[timer].UpdateThen(x => x.Target);
	[Port(0x1F801108, 3, 0x10, debug: true)]
	static void CounterTargetValue(int timer, uint value) => Timers[timer].Target = value;
}