using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Idle.Detail;
public partial class IPolicyManagerSystem : _IPolicyManagerSystem_Base;
public abstract class _IPolicyManagerSystem_Base : IpcInterface {
	protected virtual KObject GetAutoPowerDownEvent() =>
		throw new NotImplementedException("Nn.Idle.Detail.IPolicyManagerSystem.GetAutoPowerDownEvent not implemented");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown2");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Idle.Detail.IPolicyManagerSystem.Unknown5");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAutoPowerDownEvent
				om.Initialize(0, 1, 0);
				var _return = GetAutoPowerDownEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetBytes(8, 0x38));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4();
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Idle.Detail.IPolicyManagerSystem");
		}
	}
}

