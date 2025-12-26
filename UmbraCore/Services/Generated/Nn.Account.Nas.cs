using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Nas;
public partial class IAuthorizationRequest : _IAuthorizationRequest_Base;
public abstract class _IAuthorizationRequest_Base : IpcInterface {
	protected virtual void GetSessionId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IAuthorizationRequest.GetSessionId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext InvokeWithoutInteractionAsync() =>
		throw new NotImplementedException("Nn.Account.Nas.IAuthorizationRequest.InvokeWithoutInteractionAsync not implemented");
	protected virtual byte IsAuthorized() =>
		throw new NotImplementedException("Nn.Account.Nas.IAuthorizationRequest.IsAuthorized not implemented");
	protected virtual void GetAuthorizationCode(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IAuthorizationRequest.GetAuthorizationCode not implemented");
	protected virtual void GetIdToken(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IAuthorizationRequest.GetIdToken not implemented");
	protected virtual void GetState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IAuthorizationRequest.GetState not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSessionId
				GetSessionId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // InvokeWithoutInteractionAsync
				var _return = InvokeWithoutInteractionAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x13: { // IsAuthorized
				var _return = IsAuthorized();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x14: { // GetAuthorizationCode
				GetAuthorizationCode(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x15: { // GetIdToken
				GetIdToken(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x16: { // GetState
				GetState(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IAuthorizationRequest");
		}
	}
}

public partial class IOAuthProcedureForExternalNsa : _IOAuthProcedureForExternalNsa_Base;
public abstract class _IOAuthProcedureForExternalNsa_Base : IpcInterface {
	protected virtual Nn.Account.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.PrepareAsync not implemented");
	protected virtual void GetRequest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.GetRequest not implemented");
	protected virtual void ApplyResponse(Span<byte> _0) =>
		"Stub hit for Nn.Account.Nas.IOAuthProcedureForExternalNsa.ApplyResponse".Log();
	protected virtual Nn.Account.Detail.IAsyncContext ApplyResponseAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.ApplyResponseAsync not implemented");
	protected virtual void Suspend(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.Suspend not implemented");
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.GetAccountId not implemented");
	protected virtual ulong GetLinkedNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.GetLinkedNintendoAccountId not implemented");
	protected virtual void GetNickname(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.GetNickname not implemented");
	protected virtual void GetProfileImage(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForExternalNsa.GetProfileImage not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PrepareAsync
				var _return = PrepareAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetRequest
				GetRequest(im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x1A, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ApplyResponse
				ApplyResponse(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ApplyResponseAsync
				var _return = ApplyResponseAsync(im.GetSpan<byte>(0x9, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // Suspend
				Suspend(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x65: { // GetLinkedNintendoAccountId
				var _return = GetLinkedNintendoAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x66: { // GetNickname
				GetNickname(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // GetProfileImage
				GetProfileImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IOAuthProcedureForExternalNsa");
		}
	}
}

public partial class IOAuthProcedureForGuestLogin : _IOAuthProcedureForGuestLogin_Base;
public abstract class _IOAuthProcedureForGuestLogin_Base : IpcInterface {
	protected virtual Nn.Account.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.PrepareAsync not implemented");
	protected virtual void GetRequest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.GetRequest not implemented");
	protected virtual void ApplyResponse(Span<byte> _0) =>
		"Stub hit for Nn.Account.Nas.IOAuthProcedureForGuestLogin.ApplyResponse".Log();
	protected virtual Nn.Account.Detail.IAsyncContext ApplyResponseAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.ApplyResponseAsync not implemented");
	protected virtual void Suspend(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.Suspend not implemented");
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.GetAccountId not implemented");
	protected virtual ulong GetLinkedNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.GetLinkedNintendoAccountId not implemented");
	protected virtual void GetNickname(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.GetNickname not implemented");
	protected virtual void GetProfileImage(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForGuestLogin.GetProfileImage not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PrepareAsync
				var _return = PrepareAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetRequest
				GetRequest(im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x1A, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ApplyResponse
				ApplyResponse(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ApplyResponseAsync
				var _return = ApplyResponseAsync(im.GetSpan<byte>(0x9, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // Suspend
				Suspend(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x65: { // GetLinkedNintendoAccountId
				var _return = GetLinkedNintendoAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x66: { // GetNickname
				GetNickname(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // GetProfileImage
				GetProfileImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IOAuthProcedureForGuestLogin");
		}
	}
}

public partial class IOAuthProcedureForNintendoAccountLinkage : _IOAuthProcedureForNintendoAccountLinkage_Base;
public abstract class _IOAuthProcedureForNintendoAccountLinkage_Base : IpcInterface {
	protected virtual Nn.Account.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.PrepareAsync not implemented");
	protected virtual void GetRequest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.GetRequest not implemented");
	protected virtual void ApplyResponse(Span<byte> _0) =>
		"Stub hit for Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.ApplyResponse".Log();
	protected virtual Nn.Account.Detail.IAsyncContext ApplyResponseAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.ApplyResponseAsync not implemented");
	protected virtual void Suspend(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.Suspend not implemented");
	protected virtual void GetRequestWithTheme(uint _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.GetRequestWithTheme not implemented");
	protected virtual byte IsNetworkServiceAccountReplaced() =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.IsNetworkServiceAccountReplaced not implemented");
	protected virtual void GetUrlForIntroductionOfExtraMembership(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage.GetUrlForIntroductionOfExtraMembership not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PrepareAsync
				var _return = PrepareAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetRequest
				GetRequest(im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x1A, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ApplyResponse
				ApplyResponse(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ApplyResponseAsync
				var _return = ApplyResponseAsync(im.GetSpan<byte>(0x9, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // Suspend
				Suspend(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // GetRequestWithTheme
				GetRequestWithTheme(im.GetData<uint>(8), im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x1A, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // IsNetworkServiceAccountReplaced
				var _return = IsNetworkServiceAccountReplaced();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xC7: { // GetUrlForIntroductionOfExtraMembership
				GetUrlForIntroductionOfExtraMembership(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage");
		}
	}
}

