using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Detail;
public partial class ISystem : _ISystem_Base;
public abstract class _ISystem_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.Initialize");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.Finalize");
	protected virtual uint GetStateOld() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetStateOld not implemented");
	protected virtual byte IsNfcEnabledOld() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.IsNfcEnabledOld not implemented");
	protected virtual void SetNfcEnabledOld(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.SetNfcEnabledOld");
	protected virtual void InitializeSystem(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.InitializeSystem");
	protected virtual void FinalizeSystem() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.FinalizeSystem");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetState not implemented");
	protected virtual byte IsNfcEnabled() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.IsNfcEnabled not implemented");
	protected virtual void ListDevices() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.ListDevices not implemented");
	protected virtual uint GetDeviceState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetDeviceState not implemented");
	protected virtual uint GetNpadId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetNpadId not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.AttachAvailabilityChangeEvent not implemented");
	protected virtual void StartDetection(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.StartDetection");
	protected virtual void StopDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.StopDetection");
	protected virtual void GetTagInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetTagInfo not implemented");
	protected virtual KObject AttachActivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.AttachDeactivateEvent not implemented");
	protected virtual void SetNfcEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.SetNfcEnabled");
	protected virtual void ReadMifare(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.ReadMifare not implemented");
	protected virtual void WriteMifare(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.WriteMifare");
	protected virtual void SendCommandByPassThrough(Span<byte> _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.SendCommandByPassThrough not implemented");
	protected virtual void KeepPassThroughSession(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.KeepPassThroughSession");
	protected virtual void ReleasePassThroughSession(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.ReleasePassThroughSession");
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
	protected virtual Nn.Nfc.Detail.ISystem CreateSystemInterface() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystemManager.CreateSystemInterface not implemented");
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
	protected virtual void InitializeOld(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.InitializeOld");
	protected virtual void FinalizeOld() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.FinalizeOld");
	protected virtual uint GetStateOld() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetStateOld not implemented");
	protected virtual byte IsNfcEnabledOld() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.IsNfcEnabledOld not implemented");
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.Initialize");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.Finalize");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetState not implemented");
	protected virtual byte IsNfcEnabled() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.IsNfcEnabled not implemented");
	protected virtual void ListDevices() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.ListDevices not implemented");
	protected virtual uint GetDeviceState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetDeviceState not implemented");
	protected virtual uint GetNpadId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetNpadId not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.AttachAvailabilityChangeEvent not implemented");
	protected virtual void StartDetection(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.StartDetection");
	protected virtual void StopDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.StopDetection");
	protected virtual void GetTagInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetTagInfo not implemented");
	protected virtual KObject AttachActivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.AttachDeactivateEvent not implemented");
	protected virtual void ReadMifare(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.ReadMifare not implemented");
	protected virtual void WriteMifare(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.WriteMifare");
	protected virtual void SendCommandByPassThrough(Span<byte> _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.SendCommandByPassThrough not implemented");
	protected virtual void KeepPassThroughSession(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.KeepPassThroughSession");
	protected virtual void ReleasePassThroughSession(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.ReleasePassThroughSession");
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
	protected virtual Nn.Nfc.Mifare.Detail.IUser CreateUserInterface() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUserManager.CreateUserInterface not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateUserInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.IUserManager");
		}
	}
}

