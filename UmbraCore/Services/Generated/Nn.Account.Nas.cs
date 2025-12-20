using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Nas;
public partial class IAuthorizationRequest : _IAuthorizationRequest_Base;
public abstract class _IAuthorizationRequest_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSessionId
				break;
			case 0xA: // InvokeWithoutInteractionAsync
				break;
			case 0x13: // IsAuthorized
				break;
			case 0x14: // GetAuthorizationCode
				break;
			case 0x15: // GetIdToken
				break;
			case 0x16: // GetState
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IAuthorizationRequest");
		}
	}
}

public partial class IOAuthProcedureForExternalNsa : _IOAuthProcedureForExternalNsa_Base;
public abstract class _IOAuthProcedureForExternalNsa_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PrepareAsync
				break;
			case 0x1: // GetRequest
				break;
			case 0x2: // ApplyResponse
				break;
			case 0x3: // ApplyResponseAsync
				break;
			case 0xA: // Suspend
				break;
			case 0x64: // GetAccountId
				break;
			case 0x65: // GetLinkedNintendoAccountId
				break;
			case 0x66: // GetNickname
				break;
			case 0x67: // GetProfileImage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IOAuthProcedureForExternalNsa");
		}
	}
}

public partial class IOAuthProcedureForGuestLogin : _IOAuthProcedureForGuestLogin_Base;
public abstract class _IOAuthProcedureForGuestLogin_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PrepareAsync
				break;
			case 0x1: // GetRequest
				break;
			case 0x2: // ApplyResponse
				break;
			case 0x3: // ApplyResponseAsync
				break;
			case 0xA: // Suspend
				break;
			case 0x64: // GetAccountId
				break;
			case 0x65: // GetLinkedNintendoAccountId
				break;
			case 0x66: // GetNickname
				break;
			case 0x67: // GetProfileImage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IOAuthProcedureForGuestLogin");
		}
	}
}

public partial class IOAuthProcedureForNintendoAccountLinkage : _IOAuthProcedureForNintendoAccountLinkage_Base;
public abstract class _IOAuthProcedureForNintendoAccountLinkage_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PrepareAsync
				break;
			case 0x1: // GetRequest
				break;
			case 0x2: // ApplyResponse
				break;
			case 0x3: // ApplyResponseAsync
				break;
			case 0xA: // Suspend
				break;
			case 0x64: // GetRequestWithTheme
				break;
			case 0x65: // IsNetworkServiceAccountReplaced
				break;
			case 0xC7: // GetUrlForIntroductionOfExtraMembership
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage");
		}
	}
}

