using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account;
public partial class IAccountServiceForAdministrator : _IAccountServiceForAdministrator_Base {
	public readonly string ServiceName;
	public IAccountServiceForAdministrator(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAccountServiceForAdministrator_Base : IpcInterface {
	protected virtual uint GetUserCount() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserCount not implemented");
	protected virtual byte GetUserExistence(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserExistence not implemented");
	protected virtual void ListAllUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ListAllUsers not implemented");
	protected virtual void ListOpenUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ListOpenUsers not implemented");
	protected virtual void GetLastOpenedUser(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetLastOpenedUser not implemented");
	protected virtual Nn.Account.Profile.IProfile GetProfile(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfile not implemented");
	protected virtual void GetProfileDigest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileDigest not implemented");
	protected virtual byte IsUserRegistrationRequestPermitted(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.IsUserRegistrationRequestPermitted not implemented");
	protected virtual void TrySelectUserWithoutInteraction(byte _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.TrySelectUserWithoutInteraction not implemented");
	protected virtual void ListOpenContextStoredUsers() =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.ListOpenContextStoredUsers".Log();
	protected virtual Nn.Account.Detail.INotifier GetUserRegistrationNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserRegistrationNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetUserStateChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserStateChangeNotifier not implemented");
	protected virtual Nn.Account.Baas.IManagerForSystemService GetBaasAccountManagerForSystemService(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetBaasAccountManagerForSystemService not implemented");
	protected virtual Nn.Account.Detail.INotifier GetBaasUserAvailabilityChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetBaasUserAvailabilityChangeNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetProfileUpdateNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileUpdateNotifier not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(byte[] _0, ulong _1, Span<byte> _2) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.StoreSaveDataThumbnail".Log();
	protected virtual void ClearSaveDataThumbnail(byte[] _0, ulong _1) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.ClearSaveDataThumbnail".Log();
	protected virtual void LoadSaveDataThumbnail(byte[] _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.LoadSaveDataThumbnail not implemented");
	protected virtual void GetSaveDataThumbnailExistence() =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.GetSaveDataThumbnailExistence".Log();
	protected virtual void GetUserLastOpenedApplication(byte[] _0, out uint _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserLastOpenedApplication not implemented");
	protected virtual void ActivateOpenContextHolder() =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.ActivateOpenContextHolder".Log();
	protected virtual void BeginUserRegistration(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.BeginUserRegistration not implemented");
	protected virtual void CompleteUserRegistration(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.CompleteUserRegistration".Log();
	protected virtual void CancelUserRegistration(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.CancelUserRegistration".Log();
	protected virtual void DeleteUser(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.DeleteUser".Log();
	protected virtual void SetUserPosition(uint _0, byte[] _1) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.SetUserPosition".Log();
	protected virtual Nn.Account.Profile.IProfileEditor GetProfileEditor(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileEditor not implemented");
	protected virtual void CompleteUserRegistrationForcibly(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.CompleteUserRegistrationForcibly".Log();
	protected virtual Nn.Account.Baas.IFloatingRegistrationRequest CreateFloatingRegistrationRequest(uint _0, KObject _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.CreateFloatingRegistrationRequest not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext AuthenticateServiceAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.AuthenticateServiceAsync not implemented");
	protected virtual Nn.Account.Baas.IAdministrator GetBaasAccountAdministrator(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetBaasAccountAdministrator not implemented");
	protected virtual Nn.Account.Nas.IOAuthProcedureForExternalNsa ProxyProcedureForGuestLoginWithNintendoAccount(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ProxyProcedureForGuestLoginWithNintendoAccount not implemented");
	protected virtual Nn.Account.Nas.IOAuthProcedureForExternalNsa ProxyProcedureForFloatingRegistrationWithNintendoAccount(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.ProxyProcedureForFloatingRegistrationWithNintendoAccount not implemented");
	protected virtual Nn.Account.Detail.ISessionObject SuspendBackgroundDaemon() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.SuspendBackgroundDaemon not implemented");
	protected virtual void DebugInvalidateTokenCacheForUser(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugInvalidateTokenCacheForUser".Log();
	protected virtual void DebugSetUserStateClose(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugSetUserStateClose".Log();
	protected virtual void DebugSetUserStateOpen(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugSetUserStateOpen".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				var _return = GetUserCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetUserExistence
				var _return = GetUserExistence(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // ListAllUsers
				ListAllUsers(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ListOpenUsers
				ListOpenUsers(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetLastOpenedUser
				GetLastOpenedUser(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetProfile
				var _return = GetProfile(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // GetProfileDigest
				GetProfileDigest(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				var _return = IsUserRegistrationRequestPermitted(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				TrySelectUserWithoutInteraction(im.GetData<byte>(8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				ListOpenContextStoredUsers();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // GetUserRegistrationNotifier
				var _return = GetUserRegistrationNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetUserStateChangeNotifier
				var _return = GetUserStateChangeNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // GetBaasAccountManagerForSystemService
				var _return = GetBaasAccountManagerForSystemService(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // GetBaasUserAvailabilityChangeNotifier
				var _return = GetBaasUserAvailabilityChangeNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x68: { // GetProfileUpdateNotifier
				var _return = GetProfileUpdateNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x69: { // CheckNetworkServiceAvailabilityAsync
				var _return = CheckNetworkServiceAvailabilityAsync(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				StoreSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				ClearSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x70: { // LoadSaveDataThumbnail
				LoadSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x71: { // GetSaveDataThumbnailExistence
				GetSaveDataThumbnailExistence();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xBE: { // GetUserLastOpenedApplication
				GetUserLastOpenedApplication(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xBF: { // ActivateOpenContextHolder
				ActivateOpenContextHolder();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC8: { // BeginUserRegistration
				BeginUserRegistration(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC9: { // CompleteUserRegistration
				CompleteUserRegistration(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // CancelUserRegistration
				CancelUserRegistration(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCB: { // DeleteUser
				DeleteUser(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCC: { // SetUserPosition
				SetUserPosition(im.GetData<uint>(8), im.GetBytes(16, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCD: { // GetProfileEditor
				var _return = GetProfileEditor(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCE: { // CompleteUserRegistrationForcibly
				CompleteUserRegistrationForcibly(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD2: { // CreateFloatingRegistrationRequest
				var _return = CreateFloatingRegistrationRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xE6: { // AuthenticateServiceAsync
				var _return = AuthenticateServiceAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xFA: { // GetBaasAccountAdministrator
				var _return = GetBaasAccountAdministrator(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x122: { // ProxyProcedureForGuestLoginWithNintendoAccount
				var _return = ProxyProcedureForGuestLoginWithNintendoAccount(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x123: { // ProxyProcedureForFloatingRegistrationWithNintendoAccount
				var _return = ProxyProcedureForFloatingRegistrationWithNintendoAccount(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12B: { // SuspendBackgroundDaemon
				var _return = SuspendBackgroundDaemon();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E5: { // DebugInvalidateTokenCacheForUser
				DebugInvalidateTokenCacheForUser(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E6: { // DebugSetUserStateClose
				DebugSetUserStateClose(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E7: { // DebugSetUserStateOpen
				DebugSetUserStateOpen(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForAdministrator");
		}
	}
}

public partial class IAccountServiceForApplication : _IAccountServiceForApplication_Base {
	public readonly string ServiceName;
	public IAccountServiceForApplication(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAccountServiceForApplication_Base : IpcInterface {
	protected virtual uint GetUserCount() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetUserCount not implemented");
	protected virtual byte GetUserExistence(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetUserExistence not implemented");
	protected virtual void ListAllUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.ListAllUsers not implemented");
	protected virtual void ListOpenUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.ListOpenUsers not implemented");
	protected virtual void GetLastOpenedUser(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetLastOpenedUser not implemented");
	protected virtual Nn.Account.Profile.IProfile GetProfile(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetProfile not implemented");
	protected virtual void GetProfileDigest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetProfileDigest not implemented");
	protected virtual byte IsUserRegistrationRequestPermitted(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.IsUserRegistrationRequestPermitted not implemented");
	protected virtual void TrySelectUserWithoutInteraction(byte _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.TrySelectUserWithoutInteraction not implemented");
	protected virtual void ListOpenContextStoredUsers() =>
		"Stub hit for Nn.Account.IAccountServiceForApplication.ListOpenContextStoredUsers".Log();
	protected virtual void InitializeApplicationInfo(ulong _0, ulong _1) =>
		"Stub hit for Nn.Account.IAccountServiceForApplication.InitializeApplicationInfo".Log();
	protected virtual Nn.Account.Baas.IManagerForApplication GetBaasAccountManagerForApplication(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetBaasAccountManagerForApplication not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext AuthenticateApplicationAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.AuthenticateApplicationAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Account.IAccountServiceForApplication.StoreSaveDataThumbnail".Log();
	protected virtual void ClearSaveDataThumbnail(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForApplication.ClearSaveDataThumbnail".Log();
	protected virtual Nn.Account.Baas.IGuestLoginRequest CreateGuestLoginRequest(uint _0, KObject _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.CreateGuestLoginRequest not implemented");
	protected virtual void LoadOpenContext() =>
		"Stub hit for Nn.Account.IAccountServiceForApplication.LoadOpenContext".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				var _return = GetUserCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetUserExistence
				var _return = GetUserExistence(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // ListAllUsers
				ListAllUsers(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ListOpenUsers
				ListOpenUsers(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetLastOpenedUser
				GetLastOpenedUser(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetProfile
				var _return = GetProfile(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // GetProfileDigest
				GetProfileDigest(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				var _return = IsUserRegistrationRequestPermitted(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				TrySelectUserWithoutInteraction(im.GetData<byte>(8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				ListOpenContextStoredUsers();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // InitializeApplicationInfo
				InitializeApplicationInfo(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // GetBaasAccountManagerForApplication
				var _return = GetBaasAccountManagerForApplication(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // AuthenticateApplicationAsync
				var _return = AuthenticateApplicationAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // CheckNetworkServiceAvailabilityAsync
				var _return = CheckNetworkServiceAvailabilityAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				StoreSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				ClearSaveDataThumbnail(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x78: { // CreateGuestLoginRequest
				var _return = CreateGuestLoginRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x82: { // LoadOpenContext
				LoadOpenContext();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForApplication");
		}
	}
}

public partial class IAccountServiceForSystemService : _IAccountServiceForSystemService_Base {
	public readonly string ServiceName;
	public IAccountServiceForSystemService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAccountServiceForSystemService_Base : IpcInterface {
	protected virtual uint GetUserCount() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserCount not implemented");
	protected virtual byte GetUserExistence(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserExistence not implemented");
	protected virtual void ListAllUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.ListAllUsers not implemented");
	protected virtual void ListOpenUsers(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.ListOpenUsers not implemented");
	protected virtual void GetLastOpenedUser(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetLastOpenedUser not implemented");
	protected virtual Nn.Account.Profile.IProfile GetProfile(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetProfile not implemented");
	protected virtual void GetProfileDigest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetProfileDigest not implemented");
	protected virtual byte IsUserRegistrationRequestPermitted(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.IsUserRegistrationRequestPermitted not implemented");
	protected virtual void TrySelectUserWithoutInteraction(byte _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.TrySelectUserWithoutInteraction not implemented");
	protected virtual void ListOpenContextStoredUsers() =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.ListOpenContextStoredUsers".Log();
	protected virtual Nn.Account.Detail.INotifier GetUserRegistrationNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserRegistrationNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetUserStateChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserStateChangeNotifier not implemented");
	protected virtual Nn.Account.Baas.IManagerForSystemService GetBaasAccountManagerForSystemService(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetBaasAccountManagerForSystemService not implemented");
	protected virtual Nn.Account.Detail.INotifier GetBaasUserAvailabilityChangeNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetBaasUserAvailabilityChangeNotifier not implemented");
	protected virtual Nn.Account.Detail.INotifier GetProfileUpdateNotifier() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetProfileUpdateNotifier not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(byte[] _0, ulong _1, Span<byte> _2) =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.StoreSaveDataThumbnail".Log();
	protected virtual void ClearSaveDataThumbnail(byte[] _0, ulong _1) =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.ClearSaveDataThumbnail".Log();
	protected virtual void LoadSaveDataThumbnail(byte[] _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.LoadSaveDataThumbnail not implemented");
	protected virtual void GetSaveDataThumbnailExistence() =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.GetSaveDataThumbnailExistence".Log();
	protected virtual void GetUserLastOpenedApplication(byte[] _0, out uint _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserLastOpenedApplication not implemented");
	protected virtual void ActivateOpenContextHolder() =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.ActivateOpenContextHolder".Log();
	protected virtual void DebugInvalidateTokenCacheForUser(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.DebugInvalidateTokenCacheForUser".Log();
	protected virtual void DebugSetUserStateClose(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.DebugSetUserStateClose".Log();
	protected virtual void DebugSetUserStateOpen(byte[] _0) =>
		"Stub hit for Nn.Account.IAccountServiceForSystemService.DebugSetUserStateOpen".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				var _return = GetUserCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetUserExistence
				var _return = GetUserExistence(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // ListAllUsers
				ListAllUsers(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ListOpenUsers
				ListOpenUsers(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetLastOpenedUser
				GetLastOpenedUser(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetProfile
				var _return = GetProfile(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // GetProfileDigest
				GetProfileDigest(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				var _return = IsUserRegistrationRequestPermitted(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				TrySelectUserWithoutInteraction(im.GetData<byte>(8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				ListOpenContextStoredUsers();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // GetUserRegistrationNotifier
				var _return = GetUserRegistrationNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetUserStateChangeNotifier
				var _return = GetUserStateChangeNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // GetBaasAccountManagerForSystemService
				var _return = GetBaasAccountManagerForSystemService(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // GetBaasUserAvailabilityChangeNotifier
				var _return = GetBaasUserAvailabilityChangeNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x68: { // GetProfileUpdateNotifier
				var _return = GetProfileUpdateNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x69: { // CheckNetworkServiceAvailabilityAsync
				var _return = CheckNetworkServiceAvailabilityAsync(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				StoreSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				ClearSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x70: { // LoadSaveDataThumbnail
				LoadSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x71: { // GetSaveDataThumbnailExistence
				GetSaveDataThumbnailExistence();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xBE: { // GetUserLastOpenedApplication
				GetUserLastOpenedApplication(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xBF: { // ActivateOpenContextHolder
				ActivateOpenContextHolder();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E5: { // DebugInvalidateTokenCacheForUser
				DebugInvalidateTokenCacheForUser(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E6: { // DebugSetUserStateClose
				DebugSetUserStateClose(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E7: { // DebugSetUserStateOpen
				DebugSetUserStateOpen(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForSystemService");
		}
	}
}

public partial class IBaasAccessTokenAccessor : _IBaasAccessTokenAccessor_Base {
	public readonly string ServiceName;
	public IBaasAccessTokenAccessor(string serviceName) => ServiceName = serviceName;
}
public abstract class _IBaasAccessTokenAccessor_Base : IpcInterface {
	protected virtual Nn.Account.Detail.IAsyncContext EnsureCacheAsync(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.EnsureCacheAsync not implemented");
	protected virtual void LoadCache(byte[] _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.LoadCache not implemented");
	protected virtual ulong GetDeviceAccountId(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.GetDeviceAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterNotificationTokenAsync(byte[] _0, byte[] _1) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.RegisterNotificationTokenAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext UnregisterNotificationTokenAsync(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IBaasAccessTokenAccessor.UnregisterNotificationTokenAsync not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // EnsureCacheAsync
				var _return = EnsureCacheAsync(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // LoadCache
				LoadCache(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // GetDeviceAccountId
				var _return = GetDeviceAccountId(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x32: { // RegisterNotificationTokenAsync
				var _return = RegisterNotificationTokenAsync(im.GetBytes(8, 0x28), im.GetBytes(48, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x33: { // UnregisterNotificationTokenAsync
				var _return = UnregisterNotificationTokenAsync(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IBaasAccessTokenAccessor");
		}
	}
}

