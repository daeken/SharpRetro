using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Irsensor;
public partial class IIrSensorServer : _IIrSensorServer_Base;
public abstract class _IIrSensorServer_Base : IpcInterface {
	protected virtual void ActivateIrsensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.ActivateIrsensor");
	protected virtual void DeactivateIrsensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.DeactivateIrsensor");
	protected virtual KObject GetIrsensorSharedMemoryHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetIrsensorSharedMemoryHandle not implemented");
	protected virtual void StopImageProcessor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.StopImageProcessor");
	protected virtual void RunMomentProcessor(uint _0, ulong _1, byte[] _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunMomentProcessor");
	protected virtual void RunClusteringProcessor(uint _0, ulong _1, byte[] _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunClusteringProcessor");
	protected virtual void RunImageTransferProcessor(uint _0, ulong _1, byte[] _2, ulong _3, ulong _4, KObject _5) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunImageTransferProcessor");
	protected virtual void GetImageTransferProcessorState(uint _0, ulong _1, ulong _2, out byte[] _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetImageTransferProcessorState not implemented");
	protected virtual void RunTeraPluginProcessor(uint _0, byte[] _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunTeraPluginProcessor");
	protected virtual uint GetNpadIrCameraHandle(uint _0) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetNpadIrCameraHandle not implemented");
	protected virtual void RunPointingProcessor(uint _0, byte[] _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunPointingProcessor");
	protected virtual void SuspendImageProcessor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.SuspendImageProcessor");
	protected virtual void CheckFirmwareVersion(uint _0, byte[] _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.CheckFirmwareVersion");
	protected virtual void SetFunctionLevel(uint _0, byte[] _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.SetFunctionLevel");
	protected virtual void RunImageTransferExProcessor(uint _0, ulong _1, byte[] _2, ulong _3, ulong _4, KObject _5) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunImageTransferExProcessor");
	protected virtual void RunIrLedProcessor(uint _0, byte[] _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunIrLedProcessor");
	protected virtual void StopImageProcessorAsync(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.StopImageProcessorAsync");
	protected virtual void ActivateIrsensorWithFunctionLevel(byte[] _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.ActivateIrsensorWithFunctionLevel");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x12E: { // ActivateIrsensor
				om.Initialize(0, 0, 0);
				ActivateIrsensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x12F: { // DeactivateIrsensor
				om.Initialize(0, 0, 0);
				DeactivateIrsensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x130: { // GetIrsensorSharedMemoryHandle
				om.Initialize(0, 1, 0);
				var _return = GetIrsensorSharedMemoryHandle(im.GetData<ulong>(8), im.Pid);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x131: { // StopImageProcessor
				om.Initialize(0, 0, 0);
				StopImageProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x132: { // RunMomentProcessor
				om.Initialize(0, 0, 0);
				RunMomentProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x20), im.Pid);
				break;
			}
			case 0x133: { // RunClusteringProcessor
				om.Initialize(0, 0, 0);
				RunClusteringProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x28), im.Pid);
				break;
			}
			case 0x134: { // RunImageTransferProcessor
				om.Initialize(0, 0, 0);
				RunImageTransferProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x18), im.GetData<ulong>(48), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0x135: { // GetImageTransferProcessorState
				om.Initialize(0, 0, 16);
				GetImageTransferProcessorState(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x136: { // RunTeraPluginProcessor
				om.Initialize(0, 0, 0);
				RunTeraPluginProcessor(im.GetData<uint>(8), im.GetBytes(12, 0x8), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x137: { // GetNpadIrCameraHandle
				om.Initialize(0, 0, 4);
				var _return = GetNpadIrCameraHandle(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x138: { // RunPointingProcessor
				om.Initialize(0, 0, 0);
				RunPointingProcessor(im.GetData<uint>(8), im.GetBytes(12, 0xC), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x139: { // SuspendImageProcessor
				om.Initialize(0, 0, 0);
				SuspendImageProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x13A: { // CheckFirmwareVersion
				om.Initialize(0, 0, 0);
				CheckFirmwareVersion(im.GetData<uint>(8), im.GetBytes(12, 0x4), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x13B: { // SetFunctionLevel
				om.Initialize(0, 0, 0);
				SetFunctionLevel(im.GetData<uint>(8), im.GetBytes(12, 0x4), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x13C: { // RunImageTransferExProcessor
				om.Initialize(0, 0, 0);
				RunImageTransferExProcessor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetBytes(24, 0x20), im.GetData<ulong>(56), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0x13D: { // RunIrLedProcessor
				om.Initialize(0, 0, 0);
				RunIrLedProcessor(im.GetData<uint>(8), im.GetBytes(12, 0x8), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x13E: { // StopImageProcessorAsync
				om.Initialize(0, 0, 0);
				StopImageProcessorAsync(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x13F: { // ActivateIrsensorWithFunctionLevel
				om.Initialize(0, 0, 0);
				ActivateIrsensorWithFunctionLevel(im.GetBytes(8, 0x4), im.GetData<ulong>(16), im.Pid);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorServer");
		}
	}
}

public partial class IIrSensorSystemServer : _IIrSensorSystemServer_Base;
public abstract class _IIrSensorSystemServer_Base : IpcInterface {
	protected virtual void SetAppletResourceUserId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorSystemServer.SetAppletResourceUserId");
	protected virtual void RegisterAppletResourceUserId(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorSystemServer.RegisterAppletResourceUserId");
	protected virtual void UnregisterAppletResourceUserId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorSystemServer.UnregisterAppletResourceUserId");
	protected virtual void EnableAppletToGetInput(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorSystemServer.EnableAppletToGetInput");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F4: { // SetAppletResourceUserId
				om.Initialize(0, 0, 0);
				SetAppletResourceUserId(im.GetData<ulong>(8));
				break;
			}
			case 0x1F5: { // RegisterAppletResourceUserId
				om.Initialize(0, 0, 0);
				RegisterAppletResourceUserId(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1F6: { // UnregisterAppletResourceUserId
				om.Initialize(0, 0, 0);
				UnregisterAppletResourceUserId(im.GetData<ulong>(8));
				break;
			}
			case 0x1F7: { // EnableAppletToGetInput
				om.Initialize(0, 0, 0);
				EnableAppletToGetInput(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorSystemServer");
		}
	}
}

