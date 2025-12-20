using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Detail;
public partial class ISystem : _ISystem_Base;
public abstract class _ISystem_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // Finalize
				break;
			case 0x2: // GetStateOld
				break;
			case 0x3: // IsNfcEnabledOld
				break;
			case 0x64: // SetNfcEnabledOld
				break;
			case 0x190: // InitializeSystem
				break;
			case 0x191: // FinalizeSystem
				break;
			case 0x192: // GetState
				break;
			case 0x193: // IsNfcEnabled
				break;
			case 0x194: // ListDevices
				break;
			case 0x195: // GetDeviceState
				break;
			case 0x196: // GetNpadId
				break;
			case 0x197: // AttachAvailabilityChangeEvent
				break;
			case 0x198: // StartDetection
				break;
			case 0x199: // StopDetection
				break;
			case 0x19A: // GetTagInfo
				break;
			case 0x19B: // AttachActivateEvent
				break;
			case 0x19C: // AttachDeactivateEvent
				break;
			case 0x1F4: // SetNfcEnabled
				break;
			case 0x3E8: // ReadMifare
				break;
			case 0x3E9: // WriteMifare
				break;
			case 0x514: // SendCommandByPassThrough
				break;
			case 0x515: // KeepPassThroughSession
				break;
			case 0x516: // ReleasePassThroughSession
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.ISystem");
		}
	}
}

public partial class ISystemManager : _ISystemManager_Base;
public abstract class _ISystemManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateSystemInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.ISystemManager");
		}
	}
}

public partial class IUser : _IUser_Base;
public abstract class _IUser_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // InitializeOld
				break;
			case 0x1: // FinalizeOld
				break;
			case 0x2: // GetStateOld
				break;
			case 0x3: // IsNfcEnabledOld
				break;
			case 0x190: // Initialize
				break;
			case 0x191: // Finalize
				break;
			case 0x192: // GetState
				break;
			case 0x193: // IsNfcEnabled
				break;
			case 0x194: // ListDevices
				break;
			case 0x195: // GetDeviceState
				break;
			case 0x196: // GetNpadId
				break;
			case 0x197: // AttachAvailabilityChangeEvent
				break;
			case 0x198: // StartDetection
				break;
			case 0x199: // StopDetection
				break;
			case 0x19A: // GetTagInfo
				break;
			case 0x19B: // AttachActivateEvent
				break;
			case 0x19C: // AttachDeactivateEvent
				break;
			case 0x3E8: // ReadMifare
				break;
			case 0x3E9: // WriteMifare
				break;
			case 0x514: // SendCommandByPassThrough
				break;
			case 0x515: // KeepPassThroughSession
				break;
			case 0x516: // ReleasePassThroughSession
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.IUser");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.IUserManager");
		}
	}
}

