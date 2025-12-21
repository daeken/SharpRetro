using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfp.Detail;
public partial class IDebug : _IDebug_Base;
public abstract class _IDebug_Base : IpcInterface {
	protected virtual void InitializeDebug(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.InitializeDebug");
	protected virtual void FinalizeDebug() =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.FinalizeDebug");
	protected virtual void ListDevices() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.ListDevices not implemented");
	protected virtual void StartDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.StartDetection");
	protected virtual void StopDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.StopDetection");
	protected virtual void Mount(Span<byte> _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Mount");
	protected virtual void Unmount(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unmount");
	protected virtual void OpenApplicationArea(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.OpenApplicationArea");
	protected virtual void GetApplicationArea(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetApplicationArea not implemented");
	protected virtual void SetApplicationArea(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.SetApplicationArea");
	protected virtual void Flush(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Flush");
	protected virtual void Restore(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Restore");
	protected virtual void CreateApplicationArea(Span<byte> _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.CreateApplicationArea");
	protected virtual void GetTagInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetTagInfo not implemented");
	protected virtual void GetRegisterInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetRegisterInfo not implemented");
	protected virtual void GetCommonInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetCommonInfo not implemented");
	protected virtual void GetModelInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetModelInfo not implemented");
	protected virtual KObject AttachActivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.AttachDeactivateEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetState not implemented");
	protected virtual uint GetDeviceState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetDeviceState not implemented");
	protected virtual uint GetNpadId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetNpadId not implemented");
	protected virtual uint GetApplicationArea2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetApplicationArea2 not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.AttachAvailabilityChangeEvent not implemented");
	protected virtual void RecreateApplicationArea(Span<byte> _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.RecreateApplicationArea");
	protected virtual void Format(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Format");
	protected virtual void GetAdminInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetAdminInfo not implemented");
	protected virtual void GetRegisterInfo2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetRegisterInfo2 not implemented");
	protected virtual void SetRegisterInfo(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.SetRegisterInfo");
	protected virtual void DeleteRegisterInfo(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.DeleteRegisterInfo");
	protected virtual void DeleteApplicationArea(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.DeleteApplicationArea");
	protected virtual byte ExistsApplicationArea(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.ExistsApplicationArea not implemented");
	protected virtual void GetAll(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetAll not implemented");
	protected virtual void SetAll(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.SetAll");
	protected virtual void FlushDebug(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.FlushDebug");
	protected virtual void BreakTag(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.BreakTag");
	protected virtual void ReadBackupData() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.ReadBackupData not implemented");
	protected virtual void WriteBackupData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.WriteBackupData");
	protected virtual void WriteNtf(Span<byte> _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.WriteNtf");
	protected virtual void Unknown300(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unknown300");
	protected virtual void Unknown301() =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unknown301");
	protected virtual void Unknown302() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown302 not implemented");
	protected virtual void Unknown303(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unknown303");
	protected virtual void Unknown304(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unknown304");
	protected virtual void Unknown305(Span<byte> _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown305 not implemented");
	protected virtual void Unknown306(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown306 not implemented");
	protected virtual KObject Unknown307(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown307 not implemented");
	protected virtual KObject Unknown308(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown308 not implemented");
	protected virtual uint Unknown309() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown309 not implemented");
	protected virtual uint Unknown310(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown310 not implemented");
	protected virtual uint Unknown311(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown311 not implemented");
	protected virtual void Unknown312(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unknown312");
	protected virtual void Unknown313(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IDebug.Unknown313");
	protected virtual KObject Unknown314() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown314 not implemented");
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
	protected virtual Nn.Nfp.Detail.IDebug CreateDebugInterface() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebugManager.CreateDebugInterface not implemented");
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
	protected virtual void InitializeSystem(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.InitializeSystem");
	protected virtual void FinalizeSystem() =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.FinalizeSystem");
	protected virtual void ListDevices() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.ListDevices not implemented");
	protected virtual void StartDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.StartDetection");
	protected virtual void StopDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.StopDetection");
	protected virtual void Mount(Span<byte> _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.Mount");
	protected virtual void Unmount(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.Unmount");
	protected virtual void Flush(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.Flush");
	protected virtual void Restore(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.Restore");
	protected virtual void GetTagInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetTagInfo not implemented");
	protected virtual void GetRegisterInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetRegisterInfo not implemented");
	protected virtual void GetCommonInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetCommonInfo not implemented");
	protected virtual void GetModelInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetModelInfo not implemented");
	protected virtual KObject AttachActivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.AttachDeactivateEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetState not implemented");
	protected virtual uint GetDeviceState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetDeviceState not implemented");
	protected virtual uint GetNpadId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetNpadId not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.AttachAvailabilityChangeEvent not implemented");
	protected virtual void Format(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.Format");
	protected virtual void GetAdminInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetAdminInfo not implemented");
	protected virtual void GetRegisterInfo2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetRegisterInfo2 not implemented");
	protected virtual void SetRegisterInfo(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.SetRegisterInfo");
	protected virtual void DeleteRegisterInfo(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.DeleteRegisterInfo");
	protected virtual void DeleteApplicationArea(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.ISystem.DeleteApplicationArea");
	protected virtual byte ExistsApplicationArea(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.ExistsApplicationArea not implemented");
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
	protected virtual Nn.Nfp.Detail.ISystem CreateSystemInterface() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystemManager.CreateSystemInterface not implemented");
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
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.Initialize");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.Finalize");
	protected virtual void ListDevices() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.ListDevices not implemented");
	protected virtual void StartDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.StartDetection");
	protected virtual void StopDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.StopDetection");
	protected virtual void Mount(Span<byte> _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.Mount");
	protected virtual void Unmount(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.Unmount");
	protected virtual void OpenApplicationArea(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.OpenApplicationArea");
	protected virtual void GetApplicationArea(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetApplicationArea not implemented");
	protected virtual void SetApplicationArea(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.SetApplicationArea");
	protected virtual void Flush(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.Flush");
	protected virtual void Restore(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.Restore");
	protected virtual void CreateApplicationArea(Span<byte> _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.CreateApplicationArea");
	protected virtual void GetTagInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetTagInfo not implemented");
	protected virtual void GetRegisterInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetRegisterInfo not implemented");
	protected virtual void GetCommonInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetCommonInfo not implemented");
	protected virtual void GetModelInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetModelInfo not implemented");
	protected virtual KObject AttachActivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.AttachDeactivateEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetState not implemented");
	protected virtual uint GetDeviceState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetDeviceState not implemented");
	protected virtual uint GetNpadId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetNpadId not implemented");
	protected virtual uint GetApplicationArea2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetApplicationArea2 not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.AttachAvailabilityChangeEvent not implemented");
	protected virtual void RecreateApplicationArea(Span<byte> _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Nfp.Detail.IUser.RecreateApplicationArea");
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
	protected virtual Nn.Nfp.Detail.IUser CreateUserInterface() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUserManager.CreateUserInterface not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateUserInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IUserManager");
		}
	}
}

