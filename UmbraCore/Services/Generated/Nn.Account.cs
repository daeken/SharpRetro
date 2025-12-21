using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account;
public partial class IAccountServiceForAdministrator : _IAccountServiceForAdministrator_Base;
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
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.ListOpenContextStoredUsers");
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
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.StoreSaveDataThumbnail");
	protected virtual void ClearSaveDataThumbnail(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.ClearSaveDataThumbnail");
	protected virtual void LoadSaveDataThumbnail(byte[] _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.LoadSaveDataThumbnail not implemented");
	protected virtual void GetSaveDataThumbnailExistence() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.GetSaveDataThumbnailExistence");
	protected virtual void GetUserLastOpenedApplication(byte[] _0, out uint _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetUserLastOpenedApplication not implemented");
	protected virtual void ActivateOpenContextHolder() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.ActivateOpenContextHolder");
	protected virtual void BeginUserRegistration(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.BeginUserRegistration not implemented");
	protected virtual void CompleteUserRegistration(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.CompleteUserRegistration");
	protected virtual void CancelUserRegistration(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.CancelUserRegistration");
	protected virtual void DeleteUser(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DeleteUser");
	protected virtual void SetUserPosition(uint _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.SetUserPosition");
	protected virtual Nn.Account.Profile.IProfileEditor GetProfileEditor(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForAdministrator.GetProfileEditor not implemented");
	protected virtual void CompleteUserRegistrationForcibly(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.CompleteUserRegistrationForcibly");
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
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugInvalidateTokenCacheForUser");
	protected virtual void DebugSetUserStateClose(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugSetUserStateClose");
	protected virtual void DebugSetUserStateOpen(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForAdministrator.DebugSetUserStateOpen");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				om.Initialize(0, 0, 4);
				var _return = GetUserCount();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetUserExistence
				om.Initialize(0, 0, 1);
				var _return = GetUserExistence(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // ListAllUsers
				om.Initialize(0, 0, 0);
				ListAllUsers(im.GetSpan<byte>(0xA, 0));
				break;
			}
			case 0x3: { // ListOpenUsers
				om.Initialize(0, 0, 0);
				ListOpenUsers(im.GetSpan<byte>(0xA, 0));
				break;
			}
			case 0x4: { // GetLastOpenedUser
				om.Initialize(0, 0, 16);
				GetLastOpenedUser(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetProfile
				om.Initialize(1, 0, 0);
				var _return = GetProfile(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // GetProfileDigest
				om.Initialize(0, 0, 16);
				GetProfileDigest(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				om.Initialize(0, 0, 1);
				var _return = IsUserRegistrationRequestPermitted(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				om.Initialize(0, 0, 16);
				TrySelectUserWithoutInteraction(im.GetData<byte>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				om.Initialize(0, 0, 0);
				ListOpenContextStoredUsers();
				break;
			}
			case 0x64: { // GetUserRegistrationNotifier
				om.Initialize(1, 0, 0);
				var _return = GetUserRegistrationNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetUserStateChangeNotifier
				om.Initialize(1, 0, 0);
				var _return = GetUserStateChangeNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // GetBaasAccountManagerForSystemService
				om.Initialize(1, 0, 0);
				var _return = GetBaasAccountManagerForSystemService(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // GetBaasUserAvailabilityChangeNotifier
				om.Initialize(1, 0, 0);
				var _return = GetBaasUserAvailabilityChangeNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x68: { // GetProfileUpdateNotifier
				om.Initialize(1, 0, 0);
				var _return = GetProfileUpdateNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x69: { // CheckNetworkServiceAvailabilityAsync
				om.Initialize(1, 0, 0);
				var _return = CheckNetworkServiceAvailabilityAsync(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				om.Initialize(0, 0, 0);
				StoreSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				om.Initialize(0, 0, 0);
				ClearSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x70: { // LoadSaveDataThumbnail
				om.Initialize(0, 0, 4);
				LoadSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x71: { // GetSaveDataThumbnailExistence
				om.Initialize(0, 0, 0);
				GetSaveDataThumbnailExistence();
				break;
			}
			case 0xBE: { // GetUserLastOpenedApplication
				om.Initialize(0, 0, 16);
				GetUserLastOpenedApplication(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xBF: { // ActivateOpenContextHolder
				om.Initialize(0, 0, 0);
				ActivateOpenContextHolder();
				break;
			}
			case 0xC8: { // BeginUserRegistration
				om.Initialize(0, 0, 16);
				BeginUserRegistration(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC9: { // CompleteUserRegistration
				om.Initialize(0, 0, 0);
				CompleteUserRegistration(im.GetBytes(8, 0x10));
				break;
			}
			case 0xCA: { // CancelUserRegistration
				om.Initialize(0, 0, 0);
				CancelUserRegistration(im.GetBytes(8, 0x10));
				break;
			}
			case 0xCB: { // DeleteUser
				om.Initialize(0, 0, 0);
				DeleteUser(im.GetBytes(8, 0x10));
				break;
			}
			case 0xCC: { // SetUserPosition
				om.Initialize(0, 0, 0);
				SetUserPosition(im.GetData<uint>(8), im.GetBytes(16, 0x10));
				break;
			}
			case 0xCD: { // GetProfileEditor
				om.Initialize(1, 0, 0);
				var _return = GetProfileEditor(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCE: { // CompleteUserRegistrationForcibly
				om.Initialize(0, 0, 0);
				CompleteUserRegistrationForcibly(im.GetBytes(8, 0x10));
				break;
			}
			case 0xD2: { // CreateFloatingRegistrationRequest
				om.Initialize(1, 0, 0);
				var _return = CreateFloatingRegistrationRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xE6: { // AuthenticateServiceAsync
				om.Initialize(1, 0, 0);
				var _return = AuthenticateServiceAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xFA: { // GetBaasAccountAdministrator
				om.Initialize(1, 0, 0);
				var _return = GetBaasAccountAdministrator(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x122: { // ProxyProcedureForGuestLoginWithNintendoAccount
				om.Initialize(1, 0, 0);
				var _return = ProxyProcedureForGuestLoginWithNintendoAccount(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x123: { // ProxyProcedureForFloatingRegistrationWithNintendoAccount
				om.Initialize(1, 0, 0);
				var _return = ProxyProcedureForFloatingRegistrationWithNintendoAccount(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12B: { // SuspendBackgroundDaemon
				om.Initialize(1, 0, 0);
				var _return = SuspendBackgroundDaemon();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E5: { // DebugInvalidateTokenCacheForUser
				om.Initialize(0, 0, 0);
				DebugInvalidateTokenCacheForUser(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3E6: { // DebugSetUserStateClose
				om.Initialize(0, 0, 0);
				DebugSetUserStateClose(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3E7: { // DebugSetUserStateOpen
				om.Initialize(0, 0, 0);
				DebugSetUserStateOpen(im.GetBytes(8, 0x10));
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
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.ListOpenContextStoredUsers");
	protected virtual void InitializeApplicationInfo(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.InitializeApplicationInfo");
	protected virtual Nn.Account.Baas.IManagerForApplication GetBaasAccountManagerForApplication(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.GetBaasAccountManagerForApplication not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext AuthenticateApplicationAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.AuthenticateApplicationAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext CheckNetworkServiceAvailabilityAsync() =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.CheckNetworkServiceAvailabilityAsync not implemented");
	protected virtual void StoreSaveDataThumbnail(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.StoreSaveDataThumbnail");
	protected virtual void ClearSaveDataThumbnail(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.ClearSaveDataThumbnail");
	protected virtual Nn.Account.Baas.IGuestLoginRequest CreateGuestLoginRequest(uint _0, KObject _1) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForApplication.CreateGuestLoginRequest not implemented");
	protected virtual void LoadOpenContext() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForApplication.LoadOpenContext");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				om.Initialize(0, 0, 4);
				var _return = GetUserCount();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetUserExistence
				om.Initialize(0, 0, 1);
				var _return = GetUserExistence(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // ListAllUsers
				om.Initialize(0, 0, 0);
				ListAllUsers(im.GetSpan<byte>(0xA, 0));
				break;
			}
			case 0x3: { // ListOpenUsers
				om.Initialize(0, 0, 0);
				ListOpenUsers(im.GetSpan<byte>(0xA, 0));
				break;
			}
			case 0x4: { // GetLastOpenedUser
				om.Initialize(0, 0, 16);
				GetLastOpenedUser(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetProfile
				om.Initialize(1, 0, 0);
				var _return = GetProfile(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // GetProfileDigest
				om.Initialize(0, 0, 16);
				GetProfileDigest(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				om.Initialize(0, 0, 1);
				var _return = IsUserRegistrationRequestPermitted(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				om.Initialize(0, 0, 16);
				TrySelectUserWithoutInteraction(im.GetData<byte>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				om.Initialize(0, 0, 0);
				ListOpenContextStoredUsers();
				break;
			}
			case 0x64: { // InitializeApplicationInfo
				om.Initialize(0, 0, 0);
				InitializeApplicationInfo(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x65: { // GetBaasAccountManagerForApplication
				om.Initialize(1, 0, 0);
				var _return = GetBaasAccountManagerForApplication(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // AuthenticateApplicationAsync
				om.Initialize(1, 0, 0);
				var _return = AuthenticateApplicationAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // CheckNetworkServiceAvailabilityAsync
				om.Initialize(1, 0, 0);
				var _return = CheckNetworkServiceAvailabilityAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				om.Initialize(0, 0, 0);
				StoreSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				om.Initialize(0, 0, 0);
				ClearSaveDataThumbnail(im.GetBytes(8, 0x10));
				break;
			}
			case 0x78: { // CreateGuestLoginRequest
				om.Initialize(1, 0, 0);
				var _return = CreateGuestLoginRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x82: { // LoadOpenContext
				om.Initialize(0, 0, 0);
				LoadOpenContext();
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
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.ListOpenContextStoredUsers");
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
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.StoreSaveDataThumbnail");
	protected virtual void ClearSaveDataThumbnail(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.ClearSaveDataThumbnail");
	protected virtual void LoadSaveDataThumbnail(byte[] _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.LoadSaveDataThumbnail not implemented");
	protected virtual void GetSaveDataThumbnailExistence() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.GetSaveDataThumbnailExistence");
	protected virtual void GetUserLastOpenedApplication(byte[] _0, out uint _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Account.IAccountServiceForSystemService.GetUserLastOpenedApplication not implemented");
	protected virtual void ActivateOpenContextHolder() =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.ActivateOpenContextHolder");
	protected virtual void DebugInvalidateTokenCacheForUser(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.DebugInvalidateTokenCacheForUser");
	protected virtual void DebugSetUserStateClose(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.DebugSetUserStateClose");
	protected virtual void DebugSetUserStateOpen(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Account.IAccountServiceForSystemService.DebugSetUserStateOpen");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUserCount
				om.Initialize(0, 0, 4);
				var _return = GetUserCount();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetUserExistence
				om.Initialize(0, 0, 1);
				var _return = GetUserExistence(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // ListAllUsers
				om.Initialize(0, 0, 0);
				ListAllUsers(im.GetSpan<byte>(0xA, 0));
				break;
			}
			case 0x3: { // ListOpenUsers
				om.Initialize(0, 0, 0);
				ListOpenUsers(im.GetSpan<byte>(0xA, 0));
				break;
			}
			case 0x4: { // GetLastOpenedUser
				om.Initialize(0, 0, 16);
				GetLastOpenedUser(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetProfile
				om.Initialize(1, 0, 0);
				var _return = GetProfile(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // GetProfileDigest
				om.Initialize(0, 0, 16);
				GetProfileDigest(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // IsUserRegistrationRequestPermitted
				om.Initialize(0, 0, 1);
				var _return = IsUserRegistrationRequestPermitted(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // TrySelectUserWithoutInteraction
				om.Initialize(0, 0, 16);
				TrySelectUserWithoutInteraction(im.GetData<byte>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ListOpenContextStoredUsers
				om.Initialize(0, 0, 0);
				ListOpenContextStoredUsers();
				break;
			}
			case 0x64: { // GetUserRegistrationNotifier
				om.Initialize(1, 0, 0);
				var _return = GetUserRegistrationNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetUserStateChangeNotifier
				om.Initialize(1, 0, 0);
				var _return = GetUserStateChangeNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // GetBaasAccountManagerForSystemService
				om.Initialize(1, 0, 0);
				var _return = GetBaasAccountManagerForSystemService(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // GetBaasUserAvailabilityChangeNotifier
				om.Initialize(1, 0, 0);
				var _return = GetBaasUserAvailabilityChangeNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x68: { // GetProfileUpdateNotifier
				om.Initialize(1, 0, 0);
				var _return = GetProfileUpdateNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x69: { // CheckNetworkServiceAvailabilityAsync
				om.Initialize(1, 0, 0);
				var _return = CheckNetworkServiceAvailabilityAsync(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // StoreSaveDataThumbnail
				om.Initialize(0, 0, 0);
				StoreSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x6F: { // ClearSaveDataThumbnail
				om.Initialize(0, 0, 0);
				ClearSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x70: { // LoadSaveDataThumbnail
				om.Initialize(0, 0, 4);
				LoadSaveDataThumbnail(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x71: { // GetSaveDataThumbnailExistence
				om.Initialize(0, 0, 0);
				GetSaveDataThumbnailExistence();
				break;
			}
			case 0xBE: { // GetUserLastOpenedApplication
				om.Initialize(0, 0, 16);
				GetUserLastOpenedApplication(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xBF: { // ActivateOpenContextHolder
				om.Initialize(0, 0, 0);
				ActivateOpenContextHolder();
				break;
			}
			case 0x3E5: { // DebugInvalidateTokenCacheForUser
				om.Initialize(0, 0, 0);
				DebugInvalidateTokenCacheForUser(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3E6: { // DebugSetUserStateClose
				om.Initialize(0, 0, 0);
				DebugSetUserStateClose(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3E7: { // DebugSetUserStateOpen
				om.Initialize(0, 0, 0);
				DebugSetUserStateOpen(im.GetBytes(8, 0x10));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IAccountServiceForSystemService");
		}
	}
}

public partial class IBaasAccessTokenAccessor : _IBaasAccessTokenAccessor_Base;
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
				om.Initialize(1, 0, 0);
				var _return = EnsureCacheAsync(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // LoadCache
				om.Initialize(0, 0, 4);
				LoadCache(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // GetDeviceAccountId
				om.Initialize(0, 0, 8);
				var _return = GetDeviceAccountId(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x32: { // RegisterNotificationTokenAsync
				om.Initialize(1, 0, 0);
				var _return = RegisterNotificationTokenAsync(im.GetBytes(8, 0x28), im.GetBytes(48, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x33: { // UnregisterNotificationTokenAsync
				om.Initialize(1, 0, 0);
				var _return = UnregisterNotificationTokenAsync(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.IBaasAccessTokenAccessor");
		}
	}
}

