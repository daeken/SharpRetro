using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Detail;
public partial class ISystem : _ISystem_Base;
public abstract class _ISystem_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.Initialize");
	protected virtual void _Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem._Finalize");
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
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.ListDevices not implemented");
	protected virtual uint GetDeviceState(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetDeviceState not implemented");
	protected virtual uint GetNpadId(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetNpadId not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.AttachAvailabilityChangeEvent not implemented");
	protected virtual void StartDetection(byte[] _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.StartDetection");
	protected virtual void StopDetection(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.StopDetection");
	protected virtual void GetTagInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.GetTagInfo not implemented");
	protected virtual KObject AttachActivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.AttachDeactivateEvent not implemented");
	protected virtual void SetNfcEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.SetNfcEnabled");
	protected virtual void ReadMifare(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.ReadMifare not implemented");
	protected virtual void WriteMifare(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.WriteMifare");
	protected virtual void SendCommandByPassThrough(byte[] _0, ulong _1, Span<byte> _2, out uint _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystem.SendCommandByPassThrough not implemented");
	protected virtual void KeepPassThroughSession(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.KeepPassThroughSession");
	protected virtual void ReleasePassThroughSession(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.ISystem.ReleasePassThroughSession");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				Initialize(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetStateOld
				var _return = GetStateOld();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // IsNfcEnabledOld
				var _return = IsNfcEnabledOld();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x64: { // SetNfcEnabledOld
				SetNfcEnabledOld(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x190: { // InitializeSystem
				InitializeSystem(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x191: { // FinalizeSystem
				FinalizeSystem();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x192: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x193: { // IsNfcEnabled
				var _return = IsNfcEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x194: { // ListDevices
				ListDevices(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x195: { // GetDeviceState
				var _return = GetDeviceState(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x196: { // GetNpadId
				var _return = GetNpadId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x197: { // AttachAvailabilityChangeEvent
				var _return = AttachAvailabilityChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x198: { // StartDetection
				StartDetection(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x199: { // StopDetection
				StopDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19A: { // GetTagInfo
				GetTagInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19B: { // AttachActivateEvent
				var _return = AttachActivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x19C: { // AttachDeactivateEvent
				var _return = AttachDeactivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1F4: { // SetNfcEnabled
				SetNfcEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E8: { // ReadMifare
				ReadMifare(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // WriteMifare
				WriteMifare(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x514: { // SendCommandByPassThrough
				SendCommandByPassThrough(im.GetBytes(8, 0x8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x515: { // KeepPassThroughSession
				KeepPassThroughSession(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x516: { // ReleasePassThroughSession
				ReleasePassThroughSession(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.ISystem");
		}
	}
}

public partial class ISystemManager : _ISystemManager_Base {
	public readonly string ServiceName;
	public ISystemManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemManager_Base : IpcInterface {
	protected virtual Nn.Nfc.Detail.ISystem CreateSystemInterface() =>
		throw new NotImplementedException("Nn.Nfc.Detail.ISystemManager.CreateSystemInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateSystemInterface
				var _return = CreateSystemInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
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
	protected virtual void _Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser._Finalize");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetState not implemented");
	protected virtual byte IsNfcEnabled() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.IsNfcEnabled not implemented");
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.ListDevices not implemented");
	protected virtual uint GetDeviceState(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetDeviceState not implemented");
	protected virtual uint GetNpadId(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetNpadId not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.AttachAvailabilityChangeEvent not implemented");
	protected virtual void StartDetection(byte[] _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.StartDetection");
	protected virtual void StopDetection(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.StopDetection");
	protected virtual void GetTagInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.GetTagInfo not implemented");
	protected virtual KObject AttachActivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.AttachDeactivateEvent not implemented");
	protected virtual void ReadMifare(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.ReadMifare not implemented");
	protected virtual void WriteMifare(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.WriteMifare");
	protected virtual void SendCommandByPassThrough(byte[] _0, ulong _1, Span<byte> _2, out uint _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUser.SendCommandByPassThrough not implemented");
	protected virtual void KeepPassThroughSession(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.KeepPassThroughSession");
	protected virtual void ReleasePassThroughSession(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Detail.IUser.ReleasePassThroughSession");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeOld
				InitializeOld(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // FinalizeOld
				FinalizeOld();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetStateOld
				var _return = GetStateOld();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // IsNfcEnabledOld
				var _return = IsNfcEnabledOld();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x190: { // Initialize
				Initialize(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x191: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x192: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x193: { // IsNfcEnabled
				var _return = IsNfcEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x194: { // ListDevices
				ListDevices(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x195: { // GetDeviceState
				var _return = GetDeviceState(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x196: { // GetNpadId
				var _return = GetNpadId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x197: { // AttachAvailabilityChangeEvent
				var _return = AttachAvailabilityChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x198: { // StartDetection
				StartDetection(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x199: { // StopDetection
				StopDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19A: { // GetTagInfo
				GetTagInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19B: { // AttachActivateEvent
				var _return = AttachActivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x19C: { // AttachDeactivateEvent
				var _return = AttachDeactivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3E8: { // ReadMifare
				ReadMifare(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // WriteMifare
				WriteMifare(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x514: { // SendCommandByPassThrough
				SendCommandByPassThrough(im.GetBytes(8, 0x8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x515: { // KeepPassThroughSession
				KeepPassThroughSession(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x516: { // ReleasePassThroughSession
				ReleasePassThroughSession(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.IUser");
		}
	}
}

public partial class IUserManager : _IUserManager_Base {
	public readonly string ServiceName;
	public IUserManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IUserManager_Base : IpcInterface {
	protected virtual Nn.Nfc.Mifare.Detail.IUser CreateUserInterface() =>
		throw new NotImplementedException("Nn.Nfc.Detail.IUserManager.CreateUserInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserInterface
				var _return = CreateUserInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Detail.IUserManager");
		}
	}
}

