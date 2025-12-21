namespace UmbraCore.Core;

public abstract class KObject {
    public readonly uint Handle;
    public bool Closed;
    public KObject() => Handle = Kernel.Add(this);
    public virtual void Close() {}
}