using System.Text;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sm.Detail;

public partial class IUserInterface {
    protected override IpcInterface GetService(byte[] name) {
        var sname = Encoding.ASCII.GetString(name).Trim('\0');
        Console.WriteLine($"Trying to get service '{sname}'!");
        return Kernel.IpcManager.Services[sname]();
    }
}