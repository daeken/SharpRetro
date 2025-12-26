using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Irsensor;
public partial class IIrSensorServer : _IIrSensorServer_Base {
	public readonly string ServiceName;
	public IIrSensorServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IIrSensorServer_Base : IpcInterface {
	protected virtual void ActivateIrsensor(ulong _0, ulong _1) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.ActivateIrsensor".Log();
	protected virtual void DeactivateIrsensor(ulong _0, ulong _1) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.DeactivateIrsensor".Log();
	protected virtual KObject GetIrsensorSharedMemoryHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetIrsensorSharedMemoryHandle not implemented");
	protected virtual void StopImageProcessor(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.StopImageProcessor".Log();
	protected virtual void RunMomentProcessor(uint _0, ulong _1, byte[] _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunMomentProcessor".Log();
	protected virtual void RunClusteringProcessor(uint _0, ulong _1, byte[] _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunClusteringProcessor".Log();
	protected virtual void RunImageTransferProcessor(uint _0, ulong _1, byte[] _2, ulong _3, ulong _4, KObject _5) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunImageTransferProcessor".Log();
	protected virtual void GetImageTransferProcessorState(uint _0, ulong _1, ulong _2, out byte[] _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetImageTransferProcessorState not implemented");
	protected virtual void RunTeraPluginProcessor(uint _0, byte[] _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunTeraPluginProcessor".Log();
	protected virtual uint GetNpadIrCameraHandle(uint _0) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetNpadIrCameraHandle not implemented");
	protected virtual void RunPointingProcessor(uint _0, byte[] _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunPointingProcessor".Log();
	protected virtual void SuspendImageProcessor(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.SuspendImageProcessor".Log();
	protected virtual void CheckFirmwareVersion(uint _0, byte[] _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.CheckFirmwareVersion".Log();
	protected virtual void SetFunctionLevel(uint _0, byte[] _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.SetFunctionLevel".Log();
	protected virtual void RunImageTransferExProcessor(uint _0, ulong _1, byte[] _2, ulong _3, ulong _4, KObject _5) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunImageTransferExProcessor".Log();
	protected virtual void RunIrLedProcessor(uint _0, byte[] _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.RunIrLedProcessor".Log();
	protected virtual void StopImageProcessorAsync(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.StopImageProcessorAsync".Log();
	protected virtual void ActivateIrsensorWithFunctionLevel(byte[] _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Irsensor.IIrSensorServer.ActivateIrsensorWithFunctionLevel".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x12E: { // ActivateIrsensor
				ActivateIrsensor(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12F: { // DeactivateIrsensor
				DeactivateIrsensor(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x130: { // GetIrsensorSharedMemoryHandle
				var _return = GetIrsensorSharedMemoryHandle(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x131: { // StopImageProcessor
				StopImageProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x132: { // RunMomentProcessor
				RunMomentProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x20), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x133: { // RunClusteringProcessor
				RunClusteringProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x28), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x134: { // RunImageTransferProcessor
				RunImageTransferProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x18), im.GetData<ulong>(48), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x135: { // GetImageTransferProcessorState
				GetImageTransferProcessorState(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x136: { // RunTeraPluginProcessor
				RunTeraPluginProcessor(im.GetData<uint>(8), im.GetBytes(12, 0x8), im.GetData<ulong>(24), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x137: { // GetNpadIrCameraHandle
				var _return = GetNpadIrCameraHandle(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x138: { // RunPointingProcessor
				RunPointingProcessor(im.GetData<uint>(8), im.GetBytes(12, 0xC), im.GetData<ulong>(24), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x139: { // SuspendImageProcessor
				SuspendImageProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13A: { // CheckFirmwareVersion
				CheckFirmwareVersion(im.GetData<uint>(8), im.GetBytes(12, 0x4), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13B: { // SetFunctionLevel
				SetFunctionLevel(im.GetData<uint>(8), im.GetBytes(12, 0x4), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13C: { // RunImageTransferExProcessor
				RunImageTransferExProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x20), im.GetData<ulong>(56), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13D: { // RunIrLedProcessor
				RunIrLedProcessor(im.GetData<uint>(8), im.GetBytes(12, 0x8), im.GetData<ulong>(24), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13E: { // StopImageProcessorAsync
				StopImageProcessorAsync(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13F: { // ActivateIrsensorWithFunctionLevel
				ActivateIrsensorWithFunctionLevel(im.GetBytes(8, 0x4), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorServer");
		}
	}
}

public partial class IIrSensorSystemServer : _IIrSensorSystemServer_Base {
	public readonly string ServiceName;
	public IIrSensorSystemServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IIrSensorSystemServer_Base : IpcInterface {
	protected virtual void SetAppletResourceUserId(ulong _0) =>
		"Stub hit for Nn.Irsensor.IIrSensorSystemServer.SetAppletResourceUserId".Log();
	protected virtual void RegisterAppletResourceUserId(byte _0, ulong _1) =>
		"Stub hit for Nn.Irsensor.IIrSensorSystemServer.RegisterAppletResourceUserId".Log();
	protected virtual void UnregisterAppletResourceUserId(ulong _0) =>
		"Stub hit for Nn.Irsensor.IIrSensorSystemServer.UnregisterAppletResourceUserId".Log();
	protected virtual void EnableAppletToGetInput(byte _0, ulong _1) =>
		"Stub hit for Nn.Irsensor.IIrSensorSystemServer.EnableAppletToGetInput".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F4: { // SetAppletResourceUserId
				SetAppletResourceUserId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F5: { // RegisterAppletResourceUserId
				RegisterAppletResourceUserId(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F6: { // UnregisterAppletResourceUserId
				UnregisterAppletResourceUserId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F7: { // EnableAppletToGetInput
				EnableAppletToGetInput(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorSystemServer");
		}
	}
}

