using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bluetooth;
public partial class IBluetoothDriver : _IBluetoothDriver_Base;
public abstract class _IBluetoothDriver_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Init
				break;
			case 0x2: // Enable
				break;
			case 0x3: // Disable
				break;
			case 0x4: // CleanupAndShutdown
				break;
			case 0x5: // GetAdapterProperties
				break;
			case 0x6: // GetAdapterProperty
				break;
			case 0x7: // SetAdapterProperty
				break;
			case 0x8: // StartDiscovery
				break;
			case 0x9: // CancelDiscovery
				break;
			case 0xA: // CreateBond
				break;
			case 0xB: // RemoveBond
				break;
			case 0xC: // CancelBond
				break;
			case 0xD: // PinReply
				break;
			case 0xE: // SspReply
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // InitInterfaces
				break;
			case 0x11: // HidHostInterface_Connect
				break;
			case 0x12: // HidHostInterface_Disconnect
				break;
			case 0x13: // HidHostInterface_SendData
				break;
			case 0x14: // HidHostInterface_SendData2
				break;
			case 0x15: // HidHostInterface_SetReport
				break;
			case 0x16: // HidHostInterface_GetReport
				break;
			case 0x17: // HidHostInterface_WakeController
				break;
			case 0x18: // HidHostInterface_AddPairedDevice
				break;
			case 0x19: // HidHostInterface_GetPairedDevice
				break;
			case 0x1A: // HidHostInterface_CleanupAndShutdown
				break;
			case 0x1B: // Unknown27
				break;
			case 0x1C: // ExtInterface_SetTSI
				break;
			case 0x1D: // ExtInterface_SetBurstMode
				break;
			case 0x1E: // ExtInterface_SetZeroRetran
				break;
			case 0x1F: // ExtInterface_SetMcMode
				break;
			case 0x20: // ExtInterface_StartLlrMode
				break;
			case 0x21: // ExtInterface_ExitLlrMode
				break;
			case 0x22: // ExtInterface_SetRadio
				break;
			case 0x23: // ExtInterface_SetVisibility
				break;
			case 0x24: // Unknown36
				break;
			case 0x25: // Unknown37
				break;
			case 0x26: // HidHostInterface_GetLatestPlr
				break;
			case 0x27: // ExtInterface_GetPendingConnections
				break;
			case 0x28: // HidHostInterface_GetChannelMap
				break;
			case 0x29: // SetIsBluetoothBoostEnabled
				break;
			case 0x2A: // GetIsBluetoothBoostEnabled
				break;
			case 0x2B: // SetIsBluetoothAfhEnabled
				break;
			case 0x2C: // GetIsBluetoothAfhEnabled
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bluetooth.IBluetoothDriver");
		}
	}
}

