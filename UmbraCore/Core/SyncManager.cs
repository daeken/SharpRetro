namespace UmbraCore.Kernel;

public abstract class Waitable : KObject {
    public readonly Queue<Func<bool, int>> Waiters = [];
    public bool Presignaled, Canceled;
		
    protected virtual bool Presignalable => true;

    public bool Wait() {
        var wait = new AutoResetEvent(false);
        var canceled = false;
        Wait(_canceled => {
            canceled = _canceled;
            wait.Set();
            return 1;
        });
        wait.WaitOne();
        return canceled;
    }
    public void Wait(Func<int> cb) => Wait(_ => cb());
    public virtual void Wait(Func<bool, int> cb) {
        lock(this) {
            if(!Presignaled || (Presignaled && cb(Canceled) == 0))
                Waiters.Enqueue(cb);
            Presignaled = false;
            Canceled = false;
        }
    }

    public void Signal(bool one = false) {
        lock(this) {
            if(Waiters.Count == 0 && Presignalable)
                Presignaled = true;
            else {
                var realHit = false;
                while(Waiters.TryDequeue(out var waiter)) {
                    var res = waiter(Canceled);
                    if(res == 0)
                        Waiters.Enqueue(waiter);
                    if(res != -1) {
                        realHit = true;
                        if(one)
                            break;
                    }
                }
                if(!realHit && Presignalable)
                    Presignaled = true;
            }
        }
    }

    public void Cancel() {
        lock(this) {
            Canceled = true;
            Signal();
        }
    }
}
	
public unsafe class Semaphore : Waitable {
    public readonly uint* Addr;
    public uint Value {
        get {
            lock(this)
                return *Addr;
        }
    }

    public Semaphore(ulong addr) => Addr = (uint*) addr;
    protected override bool Presignalable => false;

    public void Increment() {
        lock(this)
            (*Addr)++;
    }
		
    public void Decrement() {
        lock(this)
            (*Addr)--;
    }
}

public class Event : Waitable {
    bool _Triggered;
    public bool Triggered {
        get => _Triggered;
        set {
            var doSignal = !_Triggered && value; 
            Presignaled = _Triggered = value;
            if(doSignal) Signal();
        }
    }

    public Event(bool? triggered = null) {
        if(triggered != null)
            Triggered = triggered.Value;
    }
}

public class SyncManager {
    readonly Dictionary<ulong, Semaphore> Semaphores = [];
    readonly Dictionary<ulong, Mutex> Mutexes = [];
    
    static TimeSpan? ConvertTimeout(ulong timeout) =>
        timeout != uint.MaxValue ? (TimeSpan?) new TimeSpan((long) (timeout / 100)) : null;

    Semaphore EnsureSemaphore(ulong addr) => Semaphores.TryGetValue(addr, out var sema)
        ? sema
        : Semaphores[addr] = new Semaphore(addr);

    Mutex EnsureMutex(ulong addr) => Mutexes.TryGetValue(addr, out var mutex)
        ? mutex
        : Mutexes[addr] = new Mutex();
    
    public void Setup(GameWrapper game) {
        game.Callbacks.ClearEvent = handle => {
            Kernel.Get<Event>(handle).Triggered = false;
            return 0;
        };
        game.Callbacks.ResetSignal = handle => {
            Kernel.Get<Event>(handle).Triggered = false;
            return 0;
        };
        game.Callbacks.SignalProcessWideKey = (semaAddr, target) => {
            var semaphore = EnsureSemaphore(semaAddr);
            semaphore.Increment();
            if(target == 1)
                semaphore.Signal(true);
            else if(target == 0xFFFFFFFF)
                semaphore.Signal();
            return 0;
        };
    }
}