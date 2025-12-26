namespace UmbraCore;

public static class L {
    static readonly object Lock = "lock";

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