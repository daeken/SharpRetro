using NxCommon;
using UmbraCore.Kernel;

namespace UmbraCore;

public class Rtld {
    public Rtld(List<ExeModule> modules) {
        foreach(var module in modules) {
            Console.WriteLine($"Module at {module.LoadBase:X} has {module.Symbols.Count} symbols");
        }
    }

    [Hook("someSymbol")]
    public static Int128 SomeHook() {
        return 0;
    }
}