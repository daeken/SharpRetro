namespace UmbraCore.Kernel;

[AttributeUsage(AttributeTargets.Method)]
public class Hook(string Symbol) : Attribute;

public class HookManager {
    public readonly Dictionary<string, Action> Hooks = [];
    
    public void Register(string symbol, Action hook) => Hooks[symbol] = hook;
}