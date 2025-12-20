using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Baas;
public partial class IAdministrator : _IAdministrator_Base;
public abstract class _IAdministrator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CheckAvailability
				break;
			case 0x1: // GetAccountId
				break;
			case 0x2: // EnsureIdTokenCacheAsync
				break;
			case 0x3: // LoadIdTokenCache
				break;
			case 0x64: // SetSystemProgramIdentification
				break;
			case 0x6E: // GetServiceEntryRequirementCache
				break;
			case 0x6F: // InvalidateServiceEntryRequirementCache
				break;
			case 0x70: // InvalidateTokenCache
				break;
			case 0x78: // GetNintendoAccountId
				break;
			case 0x82: // GetNintendoAccountUserResourceCache
				break;
			case 0x83: // RefreshNintendoAccountUserResourceCacheAsync
				break;
			case 0x84: // RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed
				break;
			case 0x8C: // GetNetworkServiceLicenseCache
				break;
			case 0x8D: // RefreshNetworkServiceLicenseCacheAsync
				break;
			case 0x8E: // RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed
				break;
			case 0x96: // CreateAuthorizationRequest
				break;
			case 0xC8: // IsRegistered
				break;
			case 0xC9: // RegisterAsync
				break;
			case 0xCA: // UnregisterAsync
				break;
			case 0xCB: // DeleteRegistrationInfoLocally
				break;
			case 0xDC: // SynchronizeProfileAsync
				break;
			case 0xDD: // UploadProfileAsync
				break;
			case 0xDE: // SynchronizeProfileAsyncIfSecondsElapsed
				break;
			case 0xFA: // IsLinkedWithNintendoAccount
				break;
			case 0xFB: // CreateProcedureToLinkWithNintendoAccount
				break;
			case 0xFC: // ResumeProcedureToLinkWithNintendoAccount
				break;
			case 0xFF: // CreateProcedureToUpdateLinkageStateOfNintendoAccount
				break;
			case 0x100: // ResumeProcedureToUpdateLinkageStateOfNintendoAccount
				break;
			case 0x104: // CreateProcedureToLinkNnidWithNintendoAccount
				break;
			case 0x105: // ResumeProcedureToLinkNnidWithNintendoAccount
				break;
			case 0x118: // ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount
				break;
			case 0x3E5: // DebugUnlinkNintendoAccountAsync
				break;
			case 0x3E6: // DebugSetAvailabilityErrorDetail
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IAdministrator");
		}
	}
}

public partial class IFloatingRegistrationRequest : _IFloatingRegistrationRequest_Base;
public abstract class _IFloatingRegistrationRequest_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSessionId
				break;
			case 0xC: // GetAccountId
				break;
			case 0xD: // GetLinkedNintendoAccountId
				break;
			case 0xE: // GetNickname
				break;
			case 0xF: // GetProfileImage
				break;
			case 0x15: // LoadIdTokenCache
				break;
			case 0x64: // RegisterUser
				break;
			case 0x65: // RegisterUserWithUid
				break;
			case 0x66: // RegisterNetworkServiceAccountAsync
				break;
			case 0x67: // RegisterNetworkServiceAccountWithUidAsync
				break;
			case 0x6E: // SetSystemProgramIdentification
				break;
			case 0x6F: // EnsureIdTokenCacheAsync
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IFloatingRegistrationRequest");
		}
	}
}

public partial class IGuestLoginRequest : _IGuestLoginRequest_Base;
public abstract class _IGuestLoginRequest_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSessionId
				break;
			case 0xC: // GetAccountId
				break;
			case 0xD: // GetLinkedNintendoAccountId
				break;
			case 0xE: // GetNickname
				break;
			case 0xF: // GetProfileImage
				break;
			case 0x15: // LoadIdTokenCache
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IGuestLoginRequest");
		}
	}
}

public partial class IManagerForApplication : _IManagerForApplication_Base;
public abstract class _IManagerForApplication_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CheckAvailability
				break;
			case 0x1: // GetAccountId
				break;
			case 0x2: // EnsureIdTokenCacheAsync
				break;
			case 0x3: // LoadIdTokenCache
				break;
			case 0x82: // GetNintendoAccountUserResourceCacheForApplication
				break;
			case 0x96: // CreateAuthorizationRequest
				break;
			case 0xA0: // StoreOpenContext
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IManagerForApplication");
		}
	}
}

public partial class IManagerForSystemService : _IManagerForSystemService_Base;
public abstract class _IManagerForSystemService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CheckAvailability
				break;
			case 0x1: // GetAccountId
				break;
			case 0x2: // EnsureIdTokenCacheAsync
				break;
			case 0x3: // LoadIdTokenCache
				break;
			case 0x64: // SetSystemProgramIdentification
				break;
			case 0x6E: // GetServiceEntryRequirementCache
				break;
			case 0x6F: // InvalidateServiceEntryRequirementCache
				break;
			case 0x70: // InvalidateTokenCache
				break;
			case 0x78: // GetNintendoAccountId
				break;
			case 0x82: // GetNintendoAccountUserResourceCache
				break;
			case 0x83: // RefreshNintendoAccountUserResourceCacheAsync
				break;
			case 0x84: // RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed
				break;
			case 0x8C: // GetNetworkServiceLicenseCache
				break;
			case 0x8D: // RefreshNetworkServiceLicenseCacheAsync
				break;
			case 0x8E: // RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed
				break;
			case 0x96: // CreateAuthorizationRequest
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IManagerForSystemService");
		}
	}
}

