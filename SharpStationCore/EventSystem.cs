using static SharpStationCore.Globals;

namespace SharpStationCore; 

public class Syncer {
	public readonly ISyncable Syncable;
	internal ulong _NextTimestamp = 0xFFFFFFFFFFFFFFFF;
	public ulong NextTimestamp {
		get => _NextTimestamp;
		set {
			var oldNt = _NextTimestamp;
			_NextTimestamp = value;
			if(_NextTimestamp < Events.NextTimestamp)
				Events.NextTimestamp = _NextTimestamp;
			else if(oldNt == Events.NextTimestamp) // We may have just invalidated the next timestamp
				Events.UpdateNext();
		}
	}
	public ulong LastTimestamp;

	public Syncer(ISyncable syncable) {
		Syncable = syncable;
		Events.Syncers.Add(this);
	}

	public void Sync() {
		Syncable.Sync(Timestamp - LastTimestamp);
		LastTimestamp = Timestamp;
	}
}

public interface ISyncable {
	void Sync(ulong delta);
}

public class EventSystem {
	readonly SortedList<ulong, List<Action>> Upcoming = new();
	public readonly List<Syncer> Syncers = [];
	public ulong NextTimestamp = 0xFFFFFFFFFFFFFFFF;

	public void UpdateNext() {
		NextTimestamp = 0xFFFFFFFFFFFFFFFF;
		if(Upcoming.Count > 0)
			NextTimestamp = Upcoming.First().Key;
		foreach(var syncer in Syncers)
			if(NextTimestamp > syncer.NextTimestamp)
				NextTimestamp = syncer.NextTimestamp;
	}

	public void Add(ulong time, Action func) {
		if(Upcoming.TryGetValue(time, out var list))
			list.Add(func);
		else
			Upcoming[time] = [func];
		if(NextTimestamp > time)
			NextTimestamp = time;
	}

	public bool RunEvents() {
		if(NextTimestamp > Timestamp) return Core.Running;
		
		var changed = false;
		var toRemove = new List<ulong>();
		foreach(var (key, value) in Upcoming.ToArray()) {
			if(key > Timestamp) break;
			changed = true;
			Upcoming.Remove(key);
			foreach(var func in value)
				func();
		}
		foreach(var syncer in Syncers)
			if(syncer.NextTimestamp <= Timestamp) {
				syncer._NextTimestamp = 0xFFFFFFFFFFFFFFFF;
				changed = true;
				syncer.Sync();
			}
		if(changed) UpdateNext();
		return Core.Running;
	}
}