using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Profile;
public partial class IProfile : _IProfile_Base;
public abstract class _IProfile_Base : IpcInterface {
	protected virtual void Get() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.Get not implemented");
	protected virtual void GetBase() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.GetBase not implemented");
	protected virtual uint GetImageSize() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.GetImageSize not implemented");
	protected virtual void LoadImage() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.LoadImage not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Get
				break;
			case 0x1: // GetBase
				break;
			case 0xA: // GetImageSize
				break;
			case 0xB: // LoadImage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Profile.IProfile");
		}
	}
}

public partial class IProfileEditor : _IProfileEditor_Base;
public abstract class _IProfileEditor_Base : IpcInterface {
	protected virtual void Get() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.Get not implemented");
	protected virtual void GetBase() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.GetBase not implemented");
	protected virtual uint GetImageSize() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.GetImageSize not implemented");
	protected virtual void LoadImage() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.LoadImage not implemented");
	protected virtual void Store(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Account.Profile.IProfileEditor.Store");
	protected virtual void StoreWithImage(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Account.Profile.IProfileEditor.StoreWithImage");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Get
				break;
			case 0x1: // GetBase
				break;
			case 0xA: // GetImageSize
				break;
			case 0xB: // LoadImage
				break;
			case 0x64: // Store
				break;
			case 0x65: // StoreWithImage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Profile.IProfileEditor");
		}
	}
}

