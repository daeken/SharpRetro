using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Mifare.Detail;
public partial class IUser : _IUser_Base;
public abstract class _IUser_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		"Stub hit for Nn.Nfc.Mifare.Detail.IUser.Initialize".Log();
	protected virtual void _Finalize() =>
		"Stub hit for Nn.Nfc.Mifare.Detail.IUser._Finalize".Log();
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.ListDevices not implemented");
	protected virtual void StartDetection(byte[] _0) =>
		"Stub hit for Nn.Nfc.Mifare.Detail.IUser.StartDetection".Log();
	protected virtual void StopDetection(byte[] _0) =>
		"Stub hit for Nn.Nfc.Mifare.Detail.IUser.StopDetection".Log();
	protected virtual void Read(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.Read not implemented");
	protected virtual void Write(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Nfc.Mifare.Detail.IUser.Write".Log();
	protected virtual void GetTagInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetTagInfo not implemented");
	protected virtual KObject GetActivateEventHandle(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetActivateEventHandle not implemented");
	protected virtual KObject GetDeactivateEventHandle(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetDeactivateEventHandle not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetState not implemented");
	protected virtual uint GetDeviceState(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetDeviceState not implemented");
	protected virtual uint GetNpadId(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetNpadId not implemented");
	protected virtual KObject GetAvailabilityChangeEventHandle() =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetAvailabilityChangeEventHandle not implemented");
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
			case 0x2: { // ListDevices
				ListDevices(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // StartDetection
				StartDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // StopDetection
				StopDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Read
				Read(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Write
				Write(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // GetTagInfo
				GetTagInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetActivateEventHandle
				var _return = GetActivateEventHandle(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x9: { // GetDeactivateEventHandle
				var _return = GetDeactivateEventHandle(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // GetDeviceState
				var _return = GetDeviceState(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // GetNpadId
				var _return = GetNpadId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetAvailabilityChangeEventHandle
				var _return = GetAvailabilityChangeEventHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Mifare.Detail.IUser");
		}
	}
}

public partial class IUserManager : _IUserManager_Base {
	public readonly string ServiceName;
	public IUserManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IUserManager_Base : IpcInterface {
	protected virtual Nn.Nfc.Detail.IUser CreateUserInterface() =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUserManager.CreateUserInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserInterface
				var _return = CreateUserInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Mifare.Detail.IUserManager");
		}
	}
}

