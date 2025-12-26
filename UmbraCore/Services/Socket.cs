using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Sf;

public partial class IClient {
    protected override uint RegisterClient(BsdBufferConfig config, ulong pid,
        ulong transferMemorySize, KObject transferMemory, ulong _4
    ) {
        $"Nn.Socket.Sf.IClient.RegisterClient pid=0x{pid:X} transferMemorySize=0x{transferMemorySize:X} transferMemory=0x{transferMemory?.Handle} _4=0x{_4:X}".Log();
        return 0;
    }
}
