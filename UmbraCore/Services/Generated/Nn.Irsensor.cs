using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Irsensor;
public partial class IIrSensorServer : _IIrSensorServer_Base;
public abstract class _IIrSensorServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x12E: // ActivateIrsensor
				break;
			case 0x12F: // DeactivateIrsensor
				break;
			case 0x130: // GetIrsensorSharedMemoryHandle
				break;
			case 0x131: // StopImageProcessor
				break;
			case 0x132: // RunMomentProcessor
				break;
			case 0x133: // RunClusteringProcessor
				break;
			case 0x134: // RunImageTransferProcessor
				break;
			case 0x135: // GetImageTransferProcessorState
				break;
			case 0x136: // RunTeraPluginProcessor
				break;
			case 0x137: // GetNpadIrCameraHandle
				break;
			case 0x138: // RunPointingProcessor
				break;
			case 0x139: // SuspendImageProcessor
				break;
			case 0x13A: // CheckFirmwareVersion
				break;
			case 0x13B: // SetFunctionLevel
				break;
			case 0x13C: // RunImageTransferExProcessor
				break;
			case 0x13D: // RunIrLedProcessor
				break;
			case 0x13E: // StopImageProcessorAsync
				break;
			case 0x13F: // ActivateIrsensorWithFunctionLevel
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorServer");
		}
	}
}

public partial class IIrSensorSystemServer : _IIrSensorSystemServer_Base;
public abstract class _IIrSensorSystemServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F4: // SetAppletResourceUserId
				break;
			case 0x1F5: // RegisterAppletResourceUserId
				break;
			case 0x1F6: // UnregisterAppletResourceUserId
				break;
			case 0x1F7: // EnableAppletToGetInput
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Irsensor.IIrSensorSystemServer");
		}
	}
}

