using System.Runtime.InteropServices;

namespace UmbraCore.Core;

public class IpcManager {
    public void Setup(GameWrapper game) {
        game.Callbacks.ConnectToNamedPort = (name, ref handle) => {
            Console.WriteLine($"Attempting to connect to '{Marshal.PtrToStringAnsi((IntPtr) name)}'");
            handle = 0xdaedae00;
            return 0;
        };
    }
}