namespace UmbraCore;

public static class L {
    static readonly object Lock = "lock";

    // UMBRA_QUIET: suppress per-call IPC + per-SVC trace + audio
    // dump (= the ~2200× fps win; the StackTrace symbol-walk was
    // ~660× of that, the rest is IO-bound log spam).
    public static readonly bool Quiet =
        Environment.GetEnvironmentVariable("UMBRA_QUIET") != null;

    public static void Log(this object message) {
        lock(Lock) {
            Console.WriteLine(message);
        }
    }

    public static void Log(Action func) {
        lock(Lock) {
            func();
        }
    }
}