using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfp.Detail;
public partial class IDebug : _IDebug_Base;
public abstract class _IDebug_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // InitializeDebug
				break;
			case 0x1: // FinalizeDebug
				break;
			case 0x2: // ListDevices
				break;
			case 0x3: // StartDetection
				break;
			case 0x4: // StopDetection
				break;
			case 0x5: // Mount
				break;
			case 0x6: // Unmount
				break;
			case 0x7: // OpenApplicationArea
				break;
			case 0x8: // GetApplicationArea
				break;
			case 0x9: // SetApplicationArea
				break;
			case 0xA: // Flush
				break;
			case 0xB: // Restore
				break;
			case 0xC: // CreateApplicationArea
				break;
			case 0xD: // GetTagInfo
				break;
			case 0xE: // GetRegisterInfo
				break;
			case 0xF: // GetCommonInfo
				break;
			case 0x10: // GetModelInfo
				break;
			case 0x11: // AttachActivateEvent
				break;
			case 0x12: // AttachDeactivateEvent
				break;
			case 0x13: // GetState
				break;
			case 0x14: // GetDeviceState
				break;
			case 0x15: // GetNpadId
				break;
			case 0x16: // GetApplicationArea2
				break;
			case 0x17: // AttachAvailabilityChangeEvent
				break;
			case 0x18: // RecreateApplicationArea
				break;
			case 0x64: // Format
				break;
			case 0x65: // GetAdminInfo
				break;
			case 0x66: // GetRegisterInfo2
				break;
			case 0x67: // SetRegisterInfo
				break;
			case 0x68: // DeleteRegisterInfo
				break;
			case 0x69: // DeleteApplicationArea
				break;
			case 0x6A: // ExistsApplicationArea
				break;
			case 0xC8: // GetAll
				break;
			case 0xC9: // SetAll
				break;
			case 0xCA: // FlushDebug
				break;
			case 0xCB: // BreakTag
				break;
			case 0xCC: // ReadBackupData
				break;
			case 0xCD: // WriteBackupData
				break;
			case 0xCE: // WriteNtf
				break;
			case 0x12C: // Unknown300
				break;
			case 0x12D: // Unknown301
				break;
			case 0x12E: // Unknown302
				break;
			case 0x12F: // Unknown303
				break;
			case 0x130: // Unknown304
				break;
			case 0x131: // Unknown305
				break;
			case 0x132: // Unknown306
				break;
			case 0x133: // Unknown307
				break;
			case 0x134: // Unknown308
				break;
			case 0x135: // Unknown309
				break;
			case 0x136: // Unknown310
				break;
			case 0x137: // Unknown311
				break;
			case 0x138: // Unknown312
				break;
			case 0x139: // Unknown313
				break;
			case 0x13A: // Unknown314
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IDebug");
		}
	}
}

public partial class IDebugManager : _IDebugManager_Base;
public abstract class _IDebugManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateDebugInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IDebugManager");
		}
	}
}

public partial class ISystem : _ISystem_Base;
public abstract class _ISystem_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // InitializeSystem
				break;
			case 0x1: // FinalizeSystem
				break;
			case 0x2: // ListDevices
				break;
			case 0x3: // StartDetection
				break;
			case 0x4: // StopDetection
				break;
			case 0x5: // Mount
				break;
			case 0x6: // Unmount
				break;
			case 0xA: // Flush
				break;
			case 0xB: // Restore
				break;
			case 0xD: // GetTagInfo
				break;
			case 0xE: // GetRegisterInfo
				break;
			case 0xF: // GetCommonInfo
				break;
			case 0x10: // GetModelInfo
				break;
			case 0x11: // AttachActivateEvent
				break;
			case 0x12: // AttachDeactivateEvent
				break;
			case 0x13: // GetState
				break;
			case 0x14: // GetDeviceState
				break;
			case 0x15: // GetNpadId
				break;
			case 0x17: // AttachAvailabilityChangeEvent
				break;
			case 0x64: // Format
				break;
			case 0x65: // GetAdminInfo
				break;
			case 0x66: // GetRegisterInfo2
				break;
			case 0x67: // SetRegisterInfo
				break;
			case 0x68: // DeleteRegisterInfo
				break;
			case 0x69: // DeleteApplicationArea
				break;
			case 0x6A: // ExistsApplicationArea
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.ISystem");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.ISystemManager");
		}
	}
}

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
			case 0x5: // Mount
				break;
			case 0x6: // Unmount
				break;
			case 0x7: // OpenApplicationArea
				break;
			case 0x8: // GetApplicationArea
				break;
			case 0x9: // SetApplicationArea
				break;
			case 0xA: // Flush
				break;
			case 0xB: // Restore
				break;
			case 0xC: // CreateApplicationArea
				break;
			case 0xD: // GetTagInfo
				break;
			case 0xE: // GetRegisterInfo
				break;
			case 0xF: // GetCommonInfo
				break;
			case 0x10: // GetModelInfo
				break;
			case 0x11: // AttachActivateEvent
				break;
			case 0x12: // AttachDeactivateEvent
				break;
			case 0x13: // GetState
				break;
			case 0x14: // GetDeviceState
				break;
			case 0x15: // GetNpadId
				break;
			case 0x16: // GetApplicationArea2
				break;
			case 0x17: // AttachAvailabilityChangeEvent
				break;
			case 0x18: // RecreateApplicationArea
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IUser");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IUserManager");
		}
	}
}

