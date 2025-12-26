using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcv;
public partial class IArbitrationManager : _IArbitrationManager_Base {
	public readonly string ServiceName;
	public IArbitrationManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IArbitrationManager_Base : IpcInterface {
	protected virtual void ReleaseControl(uint _0) =>
		"Stub hit for Nn.Pcv.IArbitrationManager.ReleaseControl".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ReleaseControl
				ReleaseControl(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.IArbitrationManager");
		}
	}
}

public partial class IImmediateManager : _IImmediateManager_Base {
	public readonly string ServiceName;
	public IImmediateManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IImmediateManager_Base : IpcInterface {
	protected virtual void SetClockRate(uint _0, uint _1) =>
		"Stub hit for Nn.Pcv.IImmediateManager.SetClockRate".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetClockRate
				SetClockRate(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.IImmediateManager");
		}
	}
}

