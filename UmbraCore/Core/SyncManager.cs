using LibSharpRetro;

namespace UmbraCore.Core;

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
    
    static TimeSpan ConvertTimeout(ulong timeout) =>
        timeout != ulong.MaxValue
            ? new TimeSpan((long) (timeout / 100))
            : new TimeSpan(0, 0, 0, 0, -1);

    Semaphore EnsureSemaphore(ulong addr) => Semaphores.TryGetValue(addr, out var sema)
        ? sema
        : Semaphores[addr] = new Semaphore(addr);

    Mutex EnsureMutex(ulong addr) => Mutexes.TryGetValue(addr, out var mutex)
        ? mutex
        : Mutexes[addr] = new Mutex();
    
    public unsafe void Setup(GameWrapper game) {
        game.Callbacks.ClearEvent = handle => {
            Kernel.Get<Event>(handle).Triggered = false;
            return 0;
        };
        game.Callbacks.ResetSignal = handle => {
            Kernel.Get<Event>(handle).Triggered = false;
            return 0;
        };
        game.Callbacks.SignalProcessWideKey = (semaAddr, target) => {
            $"SignalProcessWideKey {semaAddr:X}".Log();
            var semaphore = EnsureSemaphore(semaAddr);
            semaphore.Increment();
            if(target == 1)
                semaphore.Signal(true);
            else if(target == 0xFFFFFFFF)
                semaphore.Signal();
            return 0;
        };
        game.Callbacks.WaitSynchronization = (handlesAddr, numHandles, timeout, ref _activated) => {
            "WaitSynchronization".Log();
            var handles = new Buffer<uint>(handlesAddr, numHandles * 4);
            var waitHandle = new AutoResetEvent(false);
            var activated = uint.MaxValue;
            var completed = false;
            Enumerable.Range(0, (int) numHandles).ForEach(i =>
                Kernel.Get<Waitable>(handles[i]).Wait(() => {
                    if(completed) return -1;
                    lock(waitHandle) {
                        activated = (uint) i;
                        waitHandle.Set();
                        return 1;
                    }
                }));
            if(waitHandle.WaitOne(ConvertTimeout(timeout))) {
                completed = true;
                _activated = activated;
                return 0;
            }
            completed = true;
            return 0xea01;
        };
        game.Callbacks.WaitProcessWideKeyAtomic = (mutexAddr, semaAddr, threadHandle, timeout) => {
            $"WaitProcessWideKeyAtomic(0x{mutexAddr:X}, 0x{semaAddr:X}, 0x{threadHandle:X}, {timeout})".Log();
            var mutex = EnsureMutex(mutexAddr);
            var sema = EnsureSemaphore(semaAddr);
            if(sema.Value > 0) {
                $"Early bailout 0x{sema.Value:X}".Log();
                sema.Decrement();
                return 0;
            }
            game.Callbacks.ArbitrateUnlock(mutexAddr);
            sema.Wait();
            game.Callbacks.ArbitrateLock(0, mutexAddr, threadHandle);
            "Waited".Log();
            sema.Decrement();
            return 0;
        };
        game.Callbacks.ArbitrateLock = (curThread, mutexAddr, reqThread) => {
            $"LockMutex(0x{curThread:X}, 0x{mutexAddr:X}, 0x{reqThread:X})".Log();
            EnsureMutex(mutexAddr).WaitOne();
            "Locked mutex".Log();
            *(uint*) mutexAddr = (*(uint*) mutexAddr & 0x40000000) | (uint) reqThread;
            return 0;
        };
        game.Callbacks.ArbitrateUnlock = mutexAddr => {
            (*(uint*) mutexAddr) &= 0x40000000;
            try {
                EnsureMutex(mutexAddr).ReleaseMutex();
            } catch(ApplicationException) {
            }
            return 0;
        };
    }
}