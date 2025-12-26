using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Baas;
public partial class IAdministrator : _IAdministrator_Base;
public abstract class _IAdministrator_Base : IpcInterface {
	protected virtual void CheckAvailability() =>
		"Stub hit for Nn.Account.Baas.IAdministrator.CheckAvailability".Log();
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.EnsureIdTokenCacheAsync not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.LoadIdTokenCache not implemented");
	protected virtual void SetSystemProgramIdentification(ulong _0, ulong _1, Span<byte> _2) =>
		"Stub hit for Nn.Account.Baas.IAdministrator.SetSystemProgramIdentification".Log();
	protected virtual uint GetServiceEntryRequirementCache(ulong _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetServiceEntryRequirementCache not implemented");
	protected virtual void InvalidateServiceEntryRequirementCache(ulong _0) =>
		"Stub hit for Nn.Account.Baas.IAdministrator.InvalidateServiceEntryRequirementCache".Log();
	protected virtual void InvalidateTokenCache(ulong _0) =>
		"Stub hit for Nn.Account.Baas.IAdministrator.InvalidateTokenCache".Log();
	protected virtual ulong GetNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetNintendoAccountId not implemented");
	protected virtual void GetNintendoAccountUserResourceCache(out ulong _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetNintendoAccountUserResourceCache not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RefreshNintendoAccountUserResourceCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.RefreshNintendoAccountUserResourceCacheAsync not implemented");
	protected virtual void RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed(uint _0, out byte _1, out Nn.Account.Detail.IAsyncContext _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed not implemented");
	protected virtual void GetNetworkServiceLicenseCache() =>
		"Stub hit for Nn.Account.Baas.IAdministrator.GetNetworkServiceLicenseCache".Log();
	protected virtual void RefreshNetworkServiceLicenseCacheAsync() =>
		"Stub hit for Nn.Account.Baas.IAdministrator.RefreshNetworkServiceLicenseCacheAsync".Log();
	protected virtual void RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed() =>
		"Stub hit for Nn.Account.Baas.IAdministrator.RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed".Log();
	protected virtual Nn.Account.Nas.IAuthorizationRequest CreateAuthorizationRequest(uint _0, KObject _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateAuthorizationRequest not implemented");
	protected virtual byte IsRegistered() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.IsRegistered not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.RegisterAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext UnregisterAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.UnregisterAsync not implemented");
	protected virtual void DeleteRegistrationInfoLocally() =>
		"Stub hit for Nn.Account.Baas.IAdministrator.DeleteRegistrationInfoLocally".Log();
	protected virtual Nn.Account.Detail.IAsyncContext SynchronizeProfileAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.SynchronizeProfileAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext UploadProfileAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.UploadProfileAsync not implemented");
	protected virtual void SynchronizeProfileAsyncIfSecondsElapsed(uint _0, out byte _1, out Nn.Account.Detail.IAsyncContext _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.SynchronizeProfileAsyncIfSecondsElapsed not implemented");
	protected virtual byte IsLinkedWithNintendoAccount() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.IsLinkedWithNintendoAccount not implemented");
	protected virtual Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage CreateProcedureToLinkWithNintendoAccount() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateProcedureToLinkWithNintendoAccount not implemented");
	protected virtual Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage ResumeProcedureToLinkWithNintendoAccount(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ResumeProcedureToLinkWithNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure CreateProcedureToUpdateLinkageStateOfNintendoAccount() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateProcedureToUpdateLinkageStateOfNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure ResumeProcedureToUpdateLinkageStateOfNintendoAccount(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ResumeProcedureToUpdateLinkageStateOfNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure CreateProcedureToLinkNnidWithNintendoAccount() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateProcedureToLinkNnidWithNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure ResumeProcedureToLinkNnidWithNintendoAccount(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ResumeProcedureToLinkNnidWithNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext DebugUnlinkNintendoAccountAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.DebugUnlinkNintendoAccountAsync not implemented");
	protected virtual void DebugSetAvailabilityErrorDetail(uint _0) =>
		"Stub hit for Nn.Account.Baas.IAdministrator.DebugSetAvailabilityErrorDetail".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CheckAvailability
				CheckAvailability();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // EnsureIdTokenCacheAsync
				var _return = EnsureIdTokenCacheAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // LoadIdTokenCache
				LoadIdTokenCache(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x64: { // SetSystemProgramIdentification
				SetSystemProgramIdentification(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6E: { // GetServiceEntryRequirementCache
				var _return = GetServiceEntryRequirementCache(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x6F: { // InvalidateServiceEntryRequirementCache
				InvalidateServiceEntryRequirementCache(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x70: { // InvalidateTokenCache
				InvalidateTokenCache(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x78: { // GetNintendoAccountId
				var _return = GetNintendoAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x82: { // GetNintendoAccountUserResourceCache
				GetNintendoAccountUserResourceCache(out var _0, im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x83: { // RefreshNintendoAccountUserResourceCacheAsync
				var _return = RefreshNintendoAccountUserResourceCacheAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x84: { // RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed
				RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(1, 0, 1);
				om.SetData(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x8C: { // GetNetworkServiceLicenseCache
				GetNetworkServiceLicenseCache();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8D: { // RefreshNetworkServiceLicenseCacheAsync
				RefreshNetworkServiceLicenseCacheAsync();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8E: { // RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed
				RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x96: { // CreateAuthorizationRequest
				var _return = CreateAuthorizationRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // IsRegistered
				var _return = IsRegistered();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xC9: { // RegisterAsync
				var _return = RegisterAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCA: { // UnregisterAsync
				var _return = UnregisterAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCB: { // DeleteRegistrationInfoLocally
				DeleteRegistrationInfoLocally();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xDC: { // SynchronizeProfileAsync
				var _return = SynchronizeProfileAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xDD: { // UploadProfileAsync
				var _return = UploadProfileAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xDE: { // SynchronizeProfileAsyncIfSecondsElapsed
				SynchronizeProfileAsyncIfSecondsElapsed(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(1, 0, 1);
				om.SetData(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0xFA: { // IsLinkedWithNintendoAccount
				var _return = IsLinkedWithNintendoAccount();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xFB: { // CreateProcedureToLinkWithNintendoAccount
				var _return = CreateProcedureToLinkWithNintendoAccount();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xFC: { // ResumeProcedureToLinkWithNintendoAccount
				var _return = ResumeProcedureToLinkWithNintendoAccount(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xFF: { // CreateProcedureToUpdateLinkageStateOfNintendoAccount
				var _return = CreateProcedureToUpdateLinkageStateOfNintendoAccount();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x100: { // ResumeProcedureToUpdateLinkageStateOfNintendoAccount
				var _return = ResumeProcedureToUpdateLinkageStateOfNintendoAccount(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x104: { // CreateProcedureToLinkNnidWithNintendoAccount
				var _return = CreateProcedureToLinkNnidWithNintendoAccount();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x105: { // ResumeProcedureToLinkNnidWithNintendoAccount
				var _return = ResumeProcedureToLinkNnidWithNintendoAccount(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x118: { // ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount
				var _return = ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E5: { // DebugUnlinkNintendoAccountAsync
				var _return = DebugUnlinkNintendoAccountAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E6: { // DebugSetAvailabilityErrorDetail
				DebugSetAvailabilityErrorDetail(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IAdministrator");
		}
	}
}

public partial class IFloatingRegistrationRequest : _IFloatingRegistrationRequest_Base;
public abstract class _IFloatingRegistrationRequest_Base : IpcInterface {
	protected virtual void GetSessionId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.GetSessionId not implemented");
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.GetAccountId not implemented");
	protected virtual ulong GetLinkedNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.GetLinkedNintendoAccountId not implemented");
	protected virtual void GetNickname(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.GetNickname not implemented");
	protected virtual void GetProfileImage(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.GetProfileImage not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.LoadIdTokenCache not implemented");
	protected virtual void RegisterUser(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.RegisterUser not implemented");
	protected virtual void RegisterUserWithUid(byte[] _0) =>
		"Stub hit for Nn.Account.Baas.IFloatingRegistrationRequest.RegisterUserWithUid".Log();
	protected virtual Nn.Account.Detail.IAsyncContext RegisterNetworkServiceAccountAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.RegisterNetworkServiceAccountAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterNetworkServiceAccountWithUidAsync(byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.RegisterNetworkServiceAccountWithUidAsync not implemented");
	protected virtual void SetSystemProgramIdentification(ulong _0, ulong _1, Span<byte> _2) =>
		"Stub hit for Nn.Account.Baas.IFloatingRegistrationRequest.SetSystemProgramIdentification".Log();
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.EnsureIdTokenCacheAsync not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSessionId
				GetSessionId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetLinkedNintendoAccountId
				var _return = GetLinkedNintendoAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // GetNickname
				GetNickname(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetProfileImage
				GetProfileImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x15: { // LoadIdTokenCache
				LoadIdTokenCache(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x64: { // RegisterUser
				RegisterUser(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x65: { // RegisterUserWithUid
				RegisterUserWithUid(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // RegisterNetworkServiceAccountAsync
				var _return = RegisterNetworkServiceAccountAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // RegisterNetworkServiceAccountWithUidAsync
				var _return = RegisterNetworkServiceAccountWithUidAsync(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // SetSystemProgramIdentification
				SetSystemProgramIdentification(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // EnsureIdTokenCacheAsync
				var _return = EnsureIdTokenCacheAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IFloatingRegistrationRequest");
		}
	}
}

public partial class IGuestLoginRequest : _IGuestLoginRequest_Base;
public abstract class _IGuestLoginRequest_Base : IpcInterface {
	protected virtual void GetSessionId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IGuestLoginRequest.GetSessionId not implemented");
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IGuestLoginRequest.GetAccountId not implemented");
	protected virtual ulong GetLinkedNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IGuestLoginRequest.GetLinkedNintendoAccountId not implemented");
	protected virtual void GetNickname(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IGuestLoginRequest.GetNickname not implemented");
	protected virtual void GetProfileImage(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IGuestLoginRequest.GetProfileImage not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IGuestLoginRequest.LoadIdTokenCache not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSessionId
				GetSessionId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetLinkedNintendoAccountId
				var _return = GetLinkedNintendoAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // GetNickname
				GetNickname(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetProfileImage
				GetProfileImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x15: { // LoadIdTokenCache
				LoadIdTokenCache(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IGuestLoginRequest");
		}
	}
}

public partial class IManagerForApplication : _IManagerForApplication_Base;
public abstract class _IManagerForApplication_Base : IpcInterface {
	protected virtual void CheckAvailability() =>
		"Stub hit for Nn.Account.Baas.IManagerForApplication.CheckAvailability".Log();
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForApplication.GetAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForApplication.EnsureIdTokenCacheAsync not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForApplication.LoadIdTokenCache not implemented");
	protected virtual void GetNintendoAccountUserResourceCacheForApplication(out ulong _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForApplication.GetNintendoAccountUserResourceCacheForApplication not implemented");
	protected virtual Nn.Account.Nas.IAuthorizationRequest CreateAuthorizationRequest(uint _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForApplication.CreateAuthorizationRequest not implemented");
	protected virtual void StoreOpenContext() =>
		"Stub hit for Nn.Account.Baas.IManagerForApplication.StoreOpenContext".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CheckAvailability
				CheckAvailability();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // EnsureIdTokenCacheAsync
				var _return = EnsureIdTokenCacheAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // LoadIdTokenCache
				LoadIdTokenCache(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x82: { // GetNintendoAccountUserResourceCacheForApplication
				GetNintendoAccountUserResourceCacheForApplication(out var _0, im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x96: { // CreateAuthorizationRequest
				var _return = CreateAuthorizationRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA0: { // StoreOpenContext
				StoreOpenContext();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IManagerForApplication");
		}
	}
}

public partial class IManagerForSystemService : _IManagerForSystemService_Base;
public abstract class _IManagerForSystemService_Base : IpcInterface {
	protected virtual void CheckAvailability() =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.CheckAvailability".Log();
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.EnsureIdTokenCacheAsync not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.LoadIdTokenCache not implemented");
	protected virtual void SetSystemProgramIdentification(ulong _0, ulong _1, Span<byte> _2) =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.SetSystemProgramIdentification".Log();
	protected virtual uint GetServiceEntryRequirementCache(ulong _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetServiceEntryRequirementCache not implemented");
	protected virtual void InvalidateServiceEntryRequirementCache(ulong _0) =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.InvalidateServiceEntryRequirementCache".Log();
	protected virtual void InvalidateTokenCache(ulong _0) =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.InvalidateTokenCache".Log();
	protected virtual ulong GetNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetNintendoAccountId not implemented");
	protected virtual void GetNintendoAccountUserResourceCache(out ulong _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetNintendoAccountUserResourceCache not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RefreshNintendoAccountUserResourceCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.RefreshNintendoAccountUserResourceCacheAsync not implemented");
	protected virtual void RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed(uint _0, out byte _1, out Nn.Account.Detail.IAsyncContext _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed not implemented");
	protected virtual void GetNetworkServiceLicenseCache() =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.GetNetworkServiceLicenseCache".Log();
	protected virtual void RefreshNetworkServiceLicenseCacheAsync() =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.RefreshNetworkServiceLicenseCacheAsync".Log();
	protected virtual void RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed() =>
		"Stub hit for Nn.Account.Baas.IManagerForSystemService.RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed".Log();
	protected virtual Nn.Account.Nas.IAuthorizationRequest CreateAuthorizationRequest(uint _0, KObject _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.CreateAuthorizationRequest not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CheckAvailability
				CheckAvailability();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetAccountId
				var _return = GetAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // EnsureIdTokenCacheAsync
				var _return = EnsureIdTokenCacheAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // LoadIdTokenCache
				LoadIdTokenCache(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x64: { // SetSystemProgramIdentification
				SetSystemProgramIdentification(im.GetData<ulong>(8), im.Pid, im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6E: { // GetServiceEntryRequirementCache
				var _return = GetServiceEntryRequirementCache(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x6F: { // InvalidateServiceEntryRequirementCache
				InvalidateServiceEntryRequirementCache(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x70: { // InvalidateTokenCache
				InvalidateTokenCache(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x78: { // GetNintendoAccountId
				var _return = GetNintendoAccountId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x82: { // GetNintendoAccountUserResourceCache
				GetNintendoAccountUserResourceCache(out var _0, im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x83: { // RefreshNintendoAccountUserResourceCacheAsync
				var _return = RefreshNintendoAccountUserResourceCacheAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x84: { // RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed
				RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(1, 0, 1);
				om.SetData(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x8C: { // GetNetworkServiceLicenseCache
				GetNetworkServiceLicenseCache();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8D: { // RefreshNetworkServiceLicenseCacheAsync
				RefreshNetworkServiceLicenseCacheAsync();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8E: { // RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed
				RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x96: { // CreateAuthorizationRequest
				var _return = CreateAuthorizationRequest(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IManagerForSystemService");
		}
	}
}

