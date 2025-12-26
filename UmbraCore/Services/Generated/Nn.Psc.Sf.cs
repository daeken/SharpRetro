using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Psc.Sf;
public partial class IPmControl : _IPmControl_Base {
	public readonly string ServiceName;
	public IPmControl(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPmControl_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmControl.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0) =>
		"Stub hit for Nn.Psc.Sf.IPmControl.Unknown1".Log();
	protected virtual void Unknown2() =>
		"Stub hit for Nn.Psc.Sf.IPmControl.Unknown2".Log();
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmControl.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		"Stub hit for Nn.Psc.Sf.IPmControl.Unknown4".Log();
	protected virtual void Unknown5() =>
		"Stub hit for Nn.Psc.Sf.IPmControl.Unknown5".Log();
	protected virtual void Unknown6(out byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmControl.Unknown6 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
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
			case 0x6: { // Unknown6
				Unknown6(out var _0, im.GetSpan<byte>(0x6, 0), im.GetSpan<byte>(0x6, 1));
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psc.Sf.IPmControl");
		}
	}
}

public partial class IPmModule : _IPmModule_Base;
public abstract class _IPmModule_Base : IpcInterface {
	protected virtual KObject Initialize(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmModule.Initialize not implemented");
	protected virtual void GetRequest(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmModule.GetRequest not implemented");
	protected virtual void Acknowledge() =>
		"Stub hit for Nn.Psc.Sf.IPmModule.Acknowledge".Log();
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Psc.Sf.IPmModule.Unknown3".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // GetRequest
				GetRequest(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Acknowledge
				Acknowledge();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psc.Sf.IPmModule");
		}
	}
}

public partial class IPmService : _IPmService_Base {
	public readonly string ServiceName;
	public IPmService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPmService_Base : IpcInterface {
	protected virtual Nn.Psc.Sf.IPmModule GetPmModule() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmService.GetPmModule not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetPmModule
				var _return = GetPmModule();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psc.Sf.IPmService");
		}
	}
}

