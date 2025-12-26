using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Idle.Detail;
public partial class IPolicyManagerSystem : _IPolicyManagerSystem_Base {
	public readonly string ServiceName;
	public IPolicyManagerSystem(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPolicyManagerSystem_Base : IpcInterface {
	protected virtual KObject GetAutoPowerDownEvent() =>
		throw new NotImplementedException("Nn.Idle.Detail.IPolicyManagerSystem.GetAutoPowerDownEvent not implemented");
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown1".Log();
	protected virtual void Unknown2() =>
		"Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown2".Log();
	protected virtual void Unknown3(byte[] _0) =>
		"Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown3".Log();
	protected virtual void Unknown4() =>
		"Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown4".Log();
	protected virtual void Unknown5() =>
		"Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown5".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAutoPowerDownEvent
				var _return = GetAutoPowerDownEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
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
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x38));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Idle.Detail.IPolicyManagerSystem");
		}
	}
}

