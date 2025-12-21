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
	protected virtual void RunMomentProcessor(uint _0, ulong _1, Span<byte> _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunMomentProcessor");
	protected virtual void RunClusteringProcessor(uint _0, ulong _1, Span<byte> _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunClusteringProcessor");
	protected virtual void RunImageTransferProcessor(uint _0, ulong _1, Span<byte> _2, ulong _3, ulong _4, KObject _5) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunImageTransferProcessor");
	protected virtual void GetImageTransferProcessorState(uint _0, ulong _1, ulong _2, Span<byte> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetImageTransferProcessorState not implemented");
	protected virtual void RunTeraPluginProcessor(uint _0, Span<byte> _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunTeraPluginProcessor");
	protected virtual uint GetNpadIrCameraHandle(uint _0) =>
		throw new NotImplementedException("Nn.Irsensor.IIrSensorServer.GetNpadIrCameraHandle not implemented");
	protected virtual void RunPointingProcessor(uint _0, Span<byte> _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunPointingProcessor");
	protected virtual void SuspendImageProcessor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.SuspendImageProcessor");
	protected virtual void CheckFirmwareVersion(uint _0, Span<byte> _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.CheckFirmwareVersion");
	protected virtual void SetFunctionLevel(uint _0, Span<byte> _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.SetFunctionLevel");
	protected virtual void RunImageTransferExProcessor(uint _0, ulong _1, Span<byte> _2, ulong _3, ulong _4, KObject _5) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunImageTransferExProcessor");
	protected virtual void RunIrLedProcessor(uint _0, Span<byte> _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.RunIrLedProcessor");
	protected virtual void StopImageProcessorAsync(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.StopImageProcessorAsync");
	protected virtual void ActivateIrsensorWithFunctionLevel(Span<byte> _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Irsensor.IIrSensorServer.ActivateIrsensorWithFunctionLevel");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x12E: { // ActivateIrsensor
				break;
			}
			case 0x12F: { // DeactivateIrsensor
				break;
			}
			case 0x130: { // GetIrsensorSharedMemoryHandle
				break;
			}
			case 0x131: { // StopImageProcessor
				break;
			}
			case 0x132: { // RunMomentProcessor
				break;
			}
			case 0x133: { // RunClusteringProcessor
				break;
			}
			case 0x134: { // RunImageTransferProcessor
				break;
			}
			case 0x135: { // GetImageTransferProcessorState
				break;
			}
			case 0x136: { // RunTeraPluginProcessor
				break;
			}
			case 0x137: { // GetNpadIrCameraHandle
				break;
			}
			case 0x138: { // RunPointingProcessor
				break;
			}
			case 0x139: { // SuspendImageProcessor
				break;
			}
			case 0x13A: { // CheckFirmwareVersion
				break;
			}
			case 0x13B: { // SetFunctionLevel
				break;
			}
			case 0x13C: { // RunImageTransferExProcessor
				break;
			}
			case 0x13D: { // RunIrLedProcessor
				break;
			}
			case 0x13E: { // StopImageProcessorAsync
				break;
			}
			case 0x13F: { // ActivateIrsensorWithFunctionLevel
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F4: { // SetAppletResourceUserId
				break;
			}
			case 0x1F5: { // RegisterAppletResourceUserId
				break;
			}
			case 0x1F6: { // UnregisterAppletResourceUserId
				break;
			}
			case 0x1F7: { // EnableAppletToGetInput
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorSystemServer");
		}
	}
}

