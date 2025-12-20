using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Mifare.Detail;
public partial class IUser : _IUser_Base;
public abstract class _IUser_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // Finalize
				break;
			case 0x2: // ListDevices
				break;
			case 0x3: // StartDetection
				break;
			case 0x4: // StopDetection
				break;
			case 0x5: // Read
				break;
			case 0x6: // Write
				break;
			case 0x7: // GetTagInfo
				break;
			case 0x8: // GetActivateEventHandle
				break;
			case 0x9: // GetDeactivateEventHandle
				break;
			case 0xA: // GetState
				break;
			case 0xB: // GetDeviceState
				break;
			case 0xC: // GetNpadId
				break;
			case 0xD: // GetAvailabilityChangeEventHandle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Mifare.Detail.IUser");
		}
	}
}

public partial class IUserManager : _IUserManager_Base;
public abstract class _IUserManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateUserInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Mifare.Detail.IUserManager");
		}
	}
}

