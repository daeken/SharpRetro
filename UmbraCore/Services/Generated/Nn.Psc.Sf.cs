using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Psc.Sf;
public partial class IPmControl : _IPmControl_Base;
public abstract class _IPmControl_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmControl.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Psc.Sf.IPmControl.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Psc.Sf.IPmControl.Unknown2");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmControl.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Psc.Sf.IPmControl.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Psc.Sf.IPmControl.Unknown5");
	protected virtual void Unknown6() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmControl.Unknown6 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psc.Sf.IPmControl");
		}
	}
}

public partial class IPmModule : _IPmModule_Base;
public abstract class _IPmModule_Base : IpcInterface {
	protected virtual KObject Initialize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmModule.Initialize not implemented");
	protected virtual void GetRequest() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmModule.GetRequest not implemented");
	protected virtual void Acknowledge() =>
		Console.WriteLine("Stub hit for Nn.Psc.Sf.IPmModule.Acknowledge");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Psc.Sf.IPmModule.Unknown3");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // GetRequest
				break;
			case 0x2: // Acknowledge
				break;
			case 0x3: // Unknown3
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psc.Sf.IPmModule");
		}
	}
}

public partial class IPmService : _IPmService_Base;
public abstract class _IPmService_Base : IpcInterface {
	protected virtual Nn.Psc.Sf.IPmModule GetPmModule() =>
		throw new NotImplementedException("Nn.Psc.Sf.IPmService.GetPmModule not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetPmModule
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psc.Sf.IPmService");
		}
	}
}

