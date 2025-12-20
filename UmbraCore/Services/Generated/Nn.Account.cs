using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account;
public partial class IAccountServiceForAdministrator : _IAccountServiceForAdministrator_Base;
public abstract class _IAccountServiceForAdministrator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetUserCount
				break;
			case 0x1: // GetUserExistence
				break;
			case 0x2: // ListAllUsers
				break;
			case 0x3: // ListOpenUsers
				break;
			case 0x4: // GetLastOpenedUser
				break;
			case 0x5: // GetProfile
				break;
			case 0x6: // GetProfileDigest
				break;
			case 0x32: // IsUserRegistrationRequestPermitted
				break;
			case 0x33: // TrySelectUserWithoutInteraction
				break;
			case 0x3C: // ListOpenContextStoredUsers
				break;
			case 0x64: // GetUserRegistrationNotifier
				break;
			case 0x65: // GetUserStateChangeNotifier
				break;
			case 0x66: // GetBaasAccountManagerForSystemService
				break;
			case 0x67: // GetBaasUserAvailabilityChangeNotifier
				break;
			case 0x68: // GetProfileUpdateNotifier
				break;
			case 0x69: // CheckNetworkServiceAvailabilityAsync
				break;
			case 0x6E: // StoreSaveDataThumbnail
				break;
			case 0x6F: // ClearSaveDataThumbnail
				break;
			case 0x70: // LoadSaveDataThumbnail
				break;
			case 0x71: // GetSaveDataThumbnailExistence
				break;
			case 0xBE: // GetUserLastOpenedApplication
				break;
			case 0xBF: // ActivateOpenContextHolder
				break;
			case 0xC8: // BeginUserRegistration
				break;
			case 0xC9: // CompleteUserRegistration
				break;
			case 0xCA: // CancelUserRegistration
				break;
			case 0xCB: // DeleteUser
				break;
			case 0xCC: // SetUserPosition
				break;
			case 0xCD: // GetProfileEditor
				break;
			case 0xCE: // CompleteUserRegistrationForcibly
				break;
			case 0xD2: // CreateFloatingRegistrationRequest
				break;
			case 0xE6: // AuthenticateServiceAsync
				break;
			case 0xFA: // GetBaasAccountAdministrator
				break;
			case 0x122: // ProxyProcedureForGuestLoginWithNintendoAccount
				break;
			case 0x123: // ProxyProcedureForFloatingRegistrationWithNintendoAccount
				break;
			case 0x12B: // SuspendBackgroundDaemon
				break;
			case 0x3E5: // DebugInvalidateTokenCacheForUser
				break;
			case 0x3E6: // DebugSetUserStateClose
				break;
			case 0x3E7: // DebugSetUserStateOpen
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForAdministrator");
		}
	}
}

public partial class IAccountServiceForApplication : _IAccountServiceForApplication_Base;
public abstract class _IAccountServiceForApplication_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetUserCount
				break;
			case 0x1: // GetUserExistence
				break;
			case 0x2: // ListAllUsers
				break;
			case 0x3: // ListOpenUsers
				break;
			case 0x4: // GetLastOpenedUser
				break;
			case 0x5: // GetProfile
				break;
			case 0x6: // GetProfileDigest
				break;
			case 0x32: // IsUserRegistrationRequestPermitted
				break;
			case 0x33: // TrySelectUserWithoutInteraction
				break;
			case 0x3C: // ListOpenContextStoredUsers
				break;
			case 0x64: // InitializeApplicationInfo
				break;
			case 0x65: // GetBaasAccountManagerForApplication
				break;
			case 0x66: // AuthenticateApplicationAsync
				break;
			case 0x67: // CheckNetworkServiceAvailabilityAsync
				break;
			case 0x6E: // StoreSaveDataThumbnail
				break;
			case 0x6F: // ClearSaveDataThumbnail
				break;
			case 0x78: // CreateGuestLoginRequest
				break;
			case 0x82: // LoadOpenContext
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForApplication");
		}
	}
}

public partial class IAccountServiceForSystemService : _IAccountServiceForSystemService_Base;
public abstract class _IAccountServiceForSystemService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetUserCount
				break;
			case 0x1: // GetUserExistence
				break;
			case 0x2: // ListAllUsers
				break;
			case 0x3: // ListOpenUsers
				break;
			case 0x4: // GetLastOpenedUser
				break;
			case 0x5: // GetProfile
				break;
			case 0x6: // GetProfileDigest
				break;
			case 0x32: // IsUserRegistrationRequestPermitted
				break;
			case 0x33: // TrySelectUserWithoutInteraction
				break;
			case 0x3C: // ListOpenContextStoredUsers
				break;
			case 0x64: // GetUserRegistrationNotifier
				break;
			case 0x65: // GetUserStateChangeNotifier
				break;
			case 0x66: // GetBaasAccountManagerForSystemService
				break;
			case 0x67: // GetBaasUserAvailabilityChangeNotifier
				break;
			case 0x68: // GetProfileUpdateNotifier
				break;
			case 0x69: // CheckNetworkServiceAvailabilityAsync
				break;
			case 0x6E: // StoreSaveDataThumbnail
				break;
			case 0x6F: // ClearSaveDataThumbnail
				break;
			case 0x70: // LoadSaveDataThumbnail
				break;
			case 0x71: // GetSaveDataThumbnailExistence
				break;
			case 0xBE: // GetUserLastOpenedApplication
				break;
			case 0xBF: // ActivateOpenContextHolder
				break;
			case 0x3E5: // DebugInvalidateTokenCacheForUser
				break;
			case 0x3E6: // DebugSetUserStateClose
				break;
			case 0x3E7: // DebugSetUserStateOpen
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForSystemService");
		}
	}
}

public partial class IBaasAccessTokenAccessor : _IBaasAccessTokenAccessor_Base;
public abstract class _IBaasAccessTokenAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // EnsureCacheAsync
				break;
			case 0x1: // LoadCache
				break;
			case 0x2: // GetDeviceAccountId
				break;
			case 0x32: // RegisterNotificationTokenAsync
				break;
			case 0x33: // UnregisterNotificationTokenAsync
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IBaasAccessTokenAccessor");
		}
	}
}

