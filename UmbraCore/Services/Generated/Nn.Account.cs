using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account;
public partial class IAccountServiceForAdministrator : _IAccountServiceForAdministrator_Base;
public abstract class _IAccountServiceForAdministrator_Base : IpcInterface {
	protected virtual uint GetUserCount() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserCount not implemented");
	protected virtual byte GetUserExistence(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserExistence not implemented");
	protected virtual void ListAllUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ListAllUsers not implemented");
	protected virtual void ListOpenUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ListOpenUsers not implemented");
	protected virtual void GetLastOpenedUser(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetLastOpenedUser not implemented");
	protected virtual Nn.Account.Profile.IProfile GetProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfile not implemented");
	protected virtual void GetProfileDigest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileDigest not implemented");
	protected virtual byte IsUserRegistrationRequestPermitted(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.IsUserRegistrationRequestPermitted not implemented");
	protected virtual void TrySelectUserWithoutInteraction(byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.TrySelectUserWithoutInteraction not implemented");
	protected virtual void ListOpenContextStoredUsers() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.ListOpenContextStoredUsers");
	protected virtual Nn.Account.Detail.INotifier GetUserRegistrationNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserRegistrationNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetUserStateChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserStateChangeNotifier not implemented");
	protected virtual Nn.Account.Baas.IManagerForSystemService GetBaasAccountManagerForSystemService(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetBaasAccountManagerForSystemService not implemented");
	protected virtual Nn.Account.Detail.INotifier GetBaasUserAvailabilityChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetBaasUserAvailabilityChangeNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetProfileUpdateNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileUpdateNotifier not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(Span<byte> _0, ulong _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.StoreSaveDataThumbnail");
	protected virtual void ClearSaveDataThumbnail(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.ClearSaveDataThumbnail");
	protected virtual void LoadSaveDataThumbnail(Span<byte> _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.LoadSaveDataThumbnail not implemented");
	protected virtual void GetSaveDataThumbnailExistence() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.GetSaveDataThumbnailExistence");
	protected virtual void GetUserLastOpenedApplication(Span<byte> _0, out uint _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserLastOpenedApplication not implemented");
	protected virtual void ActivateOpenContextHolder() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.ActivateOpenContextHolder");
	protected virtual void BeginUserRegistration(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.BeginUserRegistration not implemented");
	protected virtual void CompleteUserRegistration(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.CompleteUserRegistration");
	protected virtual void CancelUserRegistration(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.CancelUserRegistration");
	protected virtual void DeleteUser(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DeleteUser");
	protected virtual void SetUserPosition(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.SetUserPosition");
	protected virtual Nn.Account.Profile.IProfileEditor GetProfileEditor(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileEditor not implemented");
	protected virtual void CompleteUserRegistrationForcibly(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.CompleteUserRegistrationForcibly");
	protected virtual Nn.Account.Baas.IFloatingRegistrationRequest CreateFloatingRegistrationRequest(uint _0, KObject _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.CreateFloatingRegistrationRequest not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext AuthenticateServiceAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.AuthenticateServiceAsync not implemented");
	protected virtual Nn.Account.Baas.IAdministrator GetBaasAccountAdministrator(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetBaasAccountAdministrator not implemented");
	protected virtual Nn.Account.Nas.IOAuthProcedureForExternalNsa ProxyProcedureForGuestLoginWithNintendoAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ProxyProcedureForGuestLoginWithNintendoAccount not implemented");
	protected virtual Nn.Account.Nas.IOAuthProcedureForExternalNsa ProxyProcedureForFloatingRegistrationWithNintendoAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ProxyProcedureForFloatingRegistrationWithNintendoAccount not implemented");
	protected virtual Nn.Account.Detail.ISessionObject SuspendBackgroundDaemon() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.SuspendBackgroundDaemon not implemented");
	protected virtual void DebugInvalidateTokenCacheForUser(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugInvalidateTokenCacheForUser");
	protected virtual void DebugSetUserStateClose(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugSetUserStateClose");
	protected virtual void DebugSetUserStateOpen(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugSetUserStateOpen");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				break;
			}
			case 0x1: { // GetUserExistence
				break;
			}
			case 0x2: { // ListAllUsers
				break;
			}
			case 0x3: { // ListOpenUsers
				break;
			}
			case 0x4: { // GetLastOpenedUser
				break;
			}
			case 0x5: { // GetProfile
				break;
			}
			case 0x6: { // GetProfileDigest
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				break;
			}
			case 0x64: { // GetUserRegistrationNotifier
				break;
			}
			case 0x65: { // GetUserStateChangeNotifier
				break;
			}
			case 0x66: { // GetBaasAccountManagerForSystemService
				break;
			}
			case 0x67: { // GetBaasUserAvailabilityChangeNotifier
				break;
			}
			case 0x68: { // GetProfileUpdateNotifier
				break;
			}
			case 0x69: { // CheckNetworkServiceAvailabilityAsync
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				break;
			}
			case 0x70: { // LoadSaveDataThumbnail
				break;
			}
			case 0x71: { // GetSaveDataThumbnailExistence
				break;
			}
			case 0xBE: { // GetUserLastOpenedApplication
				break;
			}
			case 0xBF: { // ActivateOpenContextHolder
				break;
			}
			case 0xC8: { // BeginUserRegistration
				break;
			}
			case 0xC9: { // CompleteUserRegistration
				break;
			}
			case 0xCA: { // CancelUserRegistration
				break;
			}
			case 0xCB: { // DeleteUser
				break;
			}
			case 0xCC: { // SetUserPosition
				break;
			}
			case 0xCD: { // GetProfileEditor
				break;
			}
			case 0xCE: { // CompleteUserRegistrationForcibly
				break;
			}
			case 0xD2: { // CreateFloatingRegistrationRequest
				break;
			}
			case 0xE6: { // AuthenticateServiceAsync
				break;
			}
			case 0xFA: { // GetBaasAccountAdministrator
				break;
			}
			case 0x122: { // ProxyProcedureForGuestLoginWithNintendoAccount
				break;
			}
			case 0x123: { // ProxyProcedureForFloatingRegistrationWithNintendoAccount
				break;
			}
			case 0x12B: { // SuspendBackgroundDaemon
				break;
			}
			case 0x3E5: { // DebugInvalidateTokenCacheForUser
				break;
			}
			case 0x3E6: { // DebugSetUserStateClose
				break;
			}
			case 0x3E7: { // DebugSetUserStateOpen
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForAdministrator");
		}
	}
}

public partial class IAccountServiceForApplication : _IAccountServiceForApplication_Base;
public abstract class _IAccountServiceForApplication_Base : IpcInterface {
	protected virtual uint GetUserCount() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetUserCount not implemented");
	protected virtual byte GetUserExistence(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetUserExistence not implemented");
	protected virtual void ListAllUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.ListAllUsers not implemented");
	protected virtual void ListOpenUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.ListOpenUsers not implemented");
	protected virtual void GetLastOpenedUser(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetLastOpenedUser not implemented");
	protected virtual Nn.Account.Profile.IProfile GetProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetProfile not implemented");
	protected virtual void GetProfileDigest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetProfileDigest not implemented");
	protected virtual byte IsUserRegistrationRequestPermitted(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.IsUserRegistrationRequestPermitted not implemented");
	protected virtual void TrySelectUserWithoutInteraction(byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.TrySelectUserWithoutInteraction not implemented");
	protected virtual void ListOpenContextStoredUsers() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.ListOpenContextStoredUsers");
	protected virtual void InitializeApplicationInfo(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.InitializeApplicationInfo");
	protected virtual Nn.Account.Baas.IManagerForApplication GetBaasAccountManagerForApplication(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetBaasAccountManagerForApplication not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext AuthenticateApplicationAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.AuthenticateApplicationAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.StoreSaveDataThumbnail");
	protected virtual void ClearSaveDataThumbnail(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.ClearSaveDataThumbnail");
	protected virtual Nn.Account.Baas.IGuestLoginRequest CreateGuestLoginRequest(uint _0, KObject _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.CreateGuestLoginRequest not implemented");
	protected virtual void LoadOpenContext() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.LoadOpenContext");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				break;
			}
			case 0x1: { // GetUserExistence
				break;
			}
			case 0x2: { // ListAllUsers
				break;
			}
			case 0x3: { // ListOpenUsers
				break;
			}
			case 0x4: { // GetLastOpenedUser
				break;
			}
			case 0x5: { // GetProfile
				break;
			}
			case 0x6: { // GetProfileDigest
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				break;
			}
			case 0x64: { // InitializeApplicationInfo
				break;
			}
			case 0x65: { // GetBaasAccountManagerForApplication
				break;
			}
			case 0x66: { // AuthenticateApplicationAsync
				break;
			}
			case 0x67: { // CheckNetworkServiceAvailabilityAsync
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				break;
			}
			case 0x78: { // CreateGuestLoginRequest
				break;
			}
			case 0x82: { // LoadOpenContext
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForApplication");
		}
	}
}

public partial class IAccountServiceForSystemService : _IAccountServiceForSystemService_Base;
public abstract class _IAccountServiceForSystemService_Base : IpcInterface {
	protected virtual uint GetUserCount() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserCount not implemented");
	protected virtual byte GetUserExistence(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserExistence not implemented");
	protected virtual void ListAllUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.ListAllUsers not implemented");
	protected virtual void ListOpenUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.ListOpenUsers not implemented");
	protected virtual void GetLastOpenedUser(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetLastOpenedUser not implemented");
	protected virtual Nn.Account.Profile.IProfile GetProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetProfile not implemented");
	protected virtual void GetProfileDigest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetProfileDigest not implemented");
	protected virtual byte IsUserRegistrationRequestPermitted(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.IsUserRegistrationRequestPermitted not implemented");
	protected virtual void TrySelectUserWithoutInteraction(byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.TrySelectUserWithoutInteraction not implemented");
	protected virtual void ListOpenContextStoredUsers() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.ListOpenContextStoredUsers");
	protected virtual Nn.Account.Detail.INotifier GetUserRegistrationNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserRegistrationNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetUserStateChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserStateChangeNotifier not implemented");
	protected virtual Nn.Account.Baas.IManagerForSystemService GetBaasAccountManagerForSystemService(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetBaasAccountManagerForSystemService not implemented");
	protected virtual Nn.Account.Detail.INotifier GetBaasUserAvailabilityChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetBaasUserAvailabilityChangeNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetProfileUpdateNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetProfileUpdateNotifier not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(Span<byte> _0, ulong _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.StoreSaveDataThumbnail");
	protected virtual void ClearSaveDataThumbnail(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.ClearSaveDataThumbnail");
	protected virtual void LoadSaveDataThumbnail(Span<byte> _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.LoadSaveDataThumbnail not implemented");
	protected virtual void GetSaveDataThumbnailExistence() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.GetSaveDataThumbnailExistence");
	protected virtual void GetUserLastOpenedApplication(Span<byte> _0, out uint _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserLastOpenedApplication not implemented");
	protected virtual void ActivateOpenContextHolder() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.ActivateOpenContextHolder");
	protected virtual void DebugInvalidateTokenCacheForUser(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.DebugInvalidateTokenCacheForUser");
	protected virtual void DebugSetUserStateClose(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.DebugSetUserStateClose");
	protected virtual void DebugSetUserStateOpen(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.DebugSetUserStateOpen");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				break;
			}
			case 0x1: { // GetUserExistence
				break;
			}
			case 0x2: { // ListAllUsers
				break;
			}
			case 0x3: { // ListOpenUsers
				break;
			}
			case 0x4: { // GetLastOpenedUser
				break;
			}
			case 0x5: { // GetProfile
				break;
			}
			case 0x6: { // GetProfileDigest
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				break;
			}
			case 0x64: { // GetUserRegistrationNotifier
				break;
			}
			case 0x65: { // GetUserStateChangeNotifier
				break;
			}
			case 0x66: { // GetBaasAccountManagerForSystemService
				break;
			}
			case 0x67: { // GetBaasUserAvailabilityChangeNotifier
				break;
			}
			case 0x68: { // GetProfileUpdateNotifier
				break;
			}
			case 0x69: { // CheckNetworkServiceAvailabilityAsync
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				break;
			}
			case 0x70: { // LoadSaveDataThumbnail
				break;
			}
			case 0x71: { // GetSaveDataThumbnailExistence
				break;
			}
			case 0xBE: { // GetUserLastOpenedApplication
				break;
			}
			case 0xBF: { // ActivateOpenContextHolder
				break;
			}
			case 0x3E5: { // DebugInvalidateTokenCacheForUser
				break;
			}
			case 0x3E6: { // DebugSetUserStateClose
				break;
			}
			case 0x3E7: { // DebugSetUserStateOpen
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForSystemService");
		}
	}
}

public partial class IBaasAccessTokenAccessor : _IBaasAccessTokenAccessor_Base;
public abstract class _IBaasAccessTokenAccessor_Base : IpcInterface {
	protected virtual Nn.Account.Detail.IAsyncContext EnsureCacheAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.EnsureCacheAsync not implemented");
	protected virtual void LoadCache(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.LoadCache not implemented");
	protected virtual ulong GetDeviceAccountId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.GetDeviceAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterNotificationTokenAsync(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.RegisterNotificationTokenAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext UnregisterNotificationTokenAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.UnregisterNotificationTokenAsync not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // EnsureCacheAsync
				break;
			}
			case 0x1: { // LoadCache
				break;
			}
			case 0x2: { // GetDeviceAccountId
				break;
			}
			case 0x32: { // RegisterNotificationTokenAsync
				break;
			}
			case 0x33: { // UnregisterNotificationTokenAsync
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IBaasAccessTokenAccessor");
		}
	}
}

