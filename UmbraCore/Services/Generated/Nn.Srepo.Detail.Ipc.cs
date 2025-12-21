using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Srepo.Detail.Ipc;
public partial class ISrepoService : _ISrepoService_Base;
public abstract class _ISrepoService_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Srepo.Detail.Ipc.ISrepoService.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Srepo.Detail.Ipc.ISrepoService.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Srepo.Detail.Ipc.ISrepoService.Unknown2");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Srepo.Detail.Ipc.ISrepoService");
		}
	}
}

