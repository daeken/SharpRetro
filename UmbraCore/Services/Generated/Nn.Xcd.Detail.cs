using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Xcd.Detail;
public partial class ISystemServer : _ISystemServer_Base;
public abstract class _ISystemServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDataFormat
				break;
			case 0x1: // SetDataFormat
				break;
			case 0x2: // GetMcuState
				break;
			case 0x3: // SetMcuState
				break;
			case 0x4: // GetMcuVersionForNfc
				break;
			case 0x5: // CheckNfcDevicePower
				break;
			case 0xA: // SetNfcEvent
				break;
			case 0xB: // GetNfcInfo
				break;
			case 0xC: // StartNfcDiscovery
				break;
			case 0xD: // StopNfcDiscovery
				break;
			case 0xE: // StartNtagRead
				break;
			case 0xF: // StartNtagWrite
				break;
			case 0x10: // SendNfcRawData
				break;
			case 0x11: // RegisterMifareKey
				break;
			case 0x12: // ClearMifareKey
				break;
			case 0x13: // StartMifareRead
				break;
			case 0x14: // StartMifareWrite
				break;
			case 0x65: // GetAwakeTriggerReasonForLeftRail
				break;
			case 0x66: // GetAwakeTriggerReasonForRightRail
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Xcd.Detail.ISystemServer");
		}
	}
}

