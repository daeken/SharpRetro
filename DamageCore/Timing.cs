namespace DamageCore;

public class AsyncTimer {
	readonly IEnumerator<ulong> Runner;
	ulong WaitingFor = ulong.MaxValue;
	ulong Current;
	
	public AsyncTimer(IEnumerable<ulong> runner) {
		Runner = runner.GetEnumerator();
		Runner.MoveNext();
		WaitingFor = Runner.Current;
	}

	public void Update(ulong cycles) {
		while(WaitingFor <= cycles) {
			Current = WaitingFor;
			Runner.MoveNext();
			WaitingFor = Current + Runner.Current;
		}
	}
}

public class Timing {
	ulong Cycles;
	readonly List<AsyncTimer> Timers = new();
	
	public void AddCycles(ulong count) {
		Cycles += count;
		foreach(var timer in Timers)
			timer.Update(Cycles);
	}

	public void TimeSync(IEnumerable<ulong> runner) => Timers.Add(new AsyncTimer(runner));
}