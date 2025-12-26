using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Hidbus;
public partial class IHidbusServer : _IHidbusServer_Base {
	public readonly string ServiceName;
	public IHidbusServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IHidbusServer_Base : IpcInterface {
	protected virtual void GetBusHandle() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.GetBusHandle".Log();
	protected virtual void IsExternalDeviceConnected() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.IsExternalDeviceConnected".Log();
	protected virtual void Initialize() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.Initialize".Log();
	protected virtual void _Finalize() =>
		"Stub hit for Nn.Hidbus.IHidbusServer._Finalize".Log();
	protected virtual void EnableExternalDevice() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.EnableExternalDevice".Log();
	protected virtual void GetExternalDeviceId() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.GetExternalDeviceId".Log();
	protected virtual void SendCommandAsync() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.SendCommandAsync".Log();
	protected virtual void GetSendCommandAsynceResult() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.GetSendCommandAsynceResult".Log();
	protected virtual void SetEventForSendCommandAsycResult() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.SetEventForSendCommandAsycResult".Log();
	protected virtual void GetSharedMemoryHandle() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.GetSharedMemoryHandle".Log();
	protected virtual void EnableJoyPollingReceiveMode() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.EnableJoyPollingReceiveMode".Log();
	protected virtual void DisableJoyPollingReceiveMode() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.DisableJoyPollingReceiveMode".Log();
	protected virtual void GetPollingData() =>
		"Stub hit for Nn.Hidbus.IHidbusServer.GetPollingData".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // GetBusHandle
				GetBusHandle();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // IsExternalDeviceConnected
				IsExternalDeviceConnected();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Initialize
				Initialize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // EnableExternalDevice
				EnableExternalDevice();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetExternalDeviceId
				GetExternalDeviceId();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // SendCommandAsync
				SendCommandAsync();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetSendCommandAsynceResult
				GetSendCommandAsynceResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // SetEventForSendCommandAsycResult
				SetEventForSendCommandAsycResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // GetSharedMemoryHandle
				GetSharedMemoryHandle();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // EnableJoyPollingReceiveMode
				EnableJoyPollingReceiveMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // DisableJoyPollingReceiveMode
				DisableJoyPollingReceiveMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // GetPollingData
				GetPollingData();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hidbus.IHidbusServer");
		}
	}
}

