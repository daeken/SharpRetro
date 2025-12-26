using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Profile;
public partial class IProfile : _IProfile_Base;
public abstract class _IProfile_Base : IpcInterface {
	protected virtual void Get(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.Get not implemented");
	protected virtual void GetBase(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.GetBase not implemented");
	protected virtual uint GetImageSize() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.GetImageSize not implemented");
	protected virtual void LoadImage(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Profile.IProfile.LoadImage not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Get
				Get(out var _0, im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 56);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetBase
				GetBase(out var _0);
				om.Initialize(0, 0, 56);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // GetImageSize
				var _return = GetImageSize();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // LoadImage
				LoadImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Profile.IProfile");
		}
	}
}

public partial class IProfileEditor : _IProfileEditor_Base;
public abstract class _IProfileEditor_Base : IpcInterface {
	protected virtual void Get(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.Get not implemented");
	protected virtual void GetBase(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.GetBase not implemented");
	protected virtual uint GetImageSize() =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.GetImageSize not implemented");
	protected virtual void LoadImage(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Profile.IProfileEditor.LoadImage not implemented");
	protected virtual void Store(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Account.Profile.IProfileEditor.Store".Log();
	protected virtual void StoreWithImage(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		"Stub hit for Nn.Account.Profile.IProfileEditor.StoreWithImage".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Get
				Get(out var _0, im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 56);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetBase
				GetBase(out var _0);
				om.Initialize(0, 0, 56);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // GetImageSize
				var _return = GetImageSize();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // LoadImage
				LoadImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x64: { // Store
				Store(im.GetBytes(8, 0x38), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // StoreWithImage
				StoreWithImage(im.GetBytes(8, 0x38), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Profile.IProfileEditor");
		}
	}
}

