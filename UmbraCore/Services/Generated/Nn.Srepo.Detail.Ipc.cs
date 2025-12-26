using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Srepo.Detail.Ipc;
public partial class ISrepoService : _ISrepoService_Base {
	public readonly string ServiceName;
	public ISrepoService(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISrepoService_Base : IpcInterface {
	protected virtual void Unknown0() =>
		"Stub hit for Nn.Srepo.Detail.Ipc.ISrepoService.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Srepo.Detail.Ipc.ISrepoService.Unknown1".Log();
	protected virtual void Unknown2() =>
		"Stub hit for Nn.Srepo.Detail.Ipc.ISrepoService.Unknown2".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Srepo.Detail.Ipc.ISrepoService");
		}
	}
}

