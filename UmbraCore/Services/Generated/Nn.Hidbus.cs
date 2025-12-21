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
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.GetBusHandle");
	protected virtual void IsExternalDeviceConnected() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.IsExternalDeviceConnected");
	protected virtual void Initialize() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.Initialize");
	protected virtual void _Finalize() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer._Finalize");
	protected virtual void EnableExternalDevice() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.EnableExternalDevice");
	protected virtual void GetExternalDeviceId() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.GetExternalDeviceId");
	protected virtual void SendCommandAsync() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.SendCommandAsync");
	protected virtual void GetSendCommandAsynceResult() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.GetSendCommandAsynceResult");
	protected virtual void SetEventForSendCommandAsycResult() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.SetEventForSendCommandAsycResult");
	protected virtual void GetSharedMemoryHandle() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.GetSharedMemoryHandle");
	protected virtual void EnableJoyPollingReceiveMode() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.EnableJoyPollingReceiveMode");
	protected virtual void DisableJoyPollingReceiveMode() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.DisableJoyPollingReceiveMode");
	protected virtual void GetPollingData() =>
		Console.WriteLine("Stub hit for Nn.Hidbus.IHidbusServer.GetPollingData");
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

