using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Mifare.Detail;
public partial class IUser : _IUser_Base;
public abstract class _IUser_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Mifare.Detail.IUser.Initialize");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Mifare.Detail.IUser.Finalize");
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.ListDevices not implemented");
	protected virtual void StartDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Mifare.Detail.IUser.StartDetection");
	protected virtual void StopDetection(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Mifare.Detail.IUser.StopDetection");
	protected virtual void Read(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.Read not implemented");
	protected virtual void Write(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Mifare.Detail.IUser.Write");
	protected virtual void GetTagInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetTagInfo not implemented");
	protected virtual KObject GetActivateEventHandle(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetActivateEventHandle not implemented");
	protected virtual KObject GetDeactivateEventHandle(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetDeactivateEventHandle not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetState not implemented");
	protected virtual uint GetDeviceState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetDeviceState not implemented");
	protected virtual uint GetNpadId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetNpadId not implemented");
	protected virtual KObject GetAvailabilityChangeEventHandle() =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUser.GetAvailabilityChangeEventHandle not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				break;
			}
			case 0x1: { // Finalize
				break;
			}
			case 0x2: { // ListDevices
				break;
			}
			case 0x3: { // StartDetection
				break;
			}
			case 0x4: { // StopDetection
				break;
			}
			case 0x5: { // Read
				break;
			}
			case 0x6: { // Write
				break;
			}
			case 0x7: { // GetTagInfo
				break;
			}
			case 0x8: { // GetActivateEventHandle
				break;
			}
			case 0x9: { // GetDeactivateEventHandle
				break;
			}
			case 0xA: { // GetState
				break;
			}
			case 0xB: { // GetDeviceState
				break;
			}
			case 0xC: { // GetNpadId
				break;
			}
			case 0xD: { // GetAvailabilityChangeEventHandle
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Mifare.Detail.IUser");
		}
	}
}

public partial class IUserManager : _IUserManager_Base;
public abstract class _IUserManager_Base : IpcInterface {
	protected virtual Nn.Nfc.Detail.IUser CreateUserInterface() =>
		throw new NotImplementedException("Nn.Nfc.Mifare.Detail.IUserManager.CreateUserInterface not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserInterface
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Mifare.Detail.IUserManager");
		}
	}
}

