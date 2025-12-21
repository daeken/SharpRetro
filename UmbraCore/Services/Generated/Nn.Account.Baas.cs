using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Baas;
public partial class IAdministrator : _IAdministrator_Base;
public abstract class _IAdministrator_Base : IpcInterface {
	protected virtual void CheckAvailability() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.CheckAvailability");
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.EnsureIdTokenCacheAsync not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.LoadIdTokenCache not implemented");
	protected virtual void SetSystemProgramIdentification(ulong _0, ulong _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.SetSystemProgramIdentification");
	protected virtual uint GetServiceEntryRequirementCache(ulong _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetServiceEntryRequirementCache not implemented");
	protected virtual void InvalidateServiceEntryRequirementCache(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.InvalidateServiceEntryRequirementCache");
	protected virtual void InvalidateTokenCache(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.InvalidateTokenCache");
	protected virtual ulong GetNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetNintendoAccountId not implemented");
	protected virtual void GetNintendoAccountUserResourceCache(out ulong _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.GetNintendoAccountUserResourceCache not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RefreshNintendoAccountUserResourceCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.RefreshNintendoAccountUserResourceCacheAsync not implemented");
	protected virtual void RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed(uint _0, out byte _1, out Nn.Account.Detail.IAsyncContext _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed not implemented");
	protected virtual void GetNetworkServiceLicenseCache() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.GetNetworkServiceLicenseCache");
	protected virtual void RefreshNetworkServiceLicenseCacheAsync() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.RefreshNetworkServiceLicenseCacheAsync");
	protected virtual void RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed");
	protected virtual Nn.Account.Nas.IAuthorizationRequest CreateAuthorizationRequest(uint _0, KObject _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateAuthorizationRequest not implemented");
	protected virtual byte IsRegistered() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.IsRegistered not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.RegisterAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext UnregisterAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.UnregisterAsync not implemented");
	protected virtual void DeleteRegistrationInfoLocally() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.DeleteRegistrationInfoLocally");
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
	protected virtual Nn.Account.Nas.IOAuthProcedureForNintendoAccountLinkage ResumeProcedureToLinkWithNintendoAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ResumeProcedureToLinkWithNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure CreateProcedureToUpdateLinkageStateOfNintendoAccount() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateProcedureToUpdateLinkageStateOfNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure ResumeProcedureToUpdateLinkageStateOfNintendoAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ResumeProcedureToUpdateLinkageStateOfNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure CreateProcedureToLinkNnidWithNintendoAccount() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.CreateProcedureToLinkNnidWithNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure ResumeProcedureToLinkNnidWithNintendoAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ResumeProcedureToLinkNnidWithNintendoAccount not implemented");
	protected virtual Nn.Account.Http.IOAuthProcedure ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext DebugUnlinkNintendoAccountAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IAdministrator.DebugUnlinkNintendoAccountAsync not implemented");
	protected virtual void DebugSetAvailabilityErrorDetail(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IAdministrator.DebugSetAvailabilityErrorDetail");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CheckAvailability
				break;
			}
			case 0x1: { // GetAccountId
				break;
			}
			case 0x2: { // EnsureIdTokenCacheAsync
				break;
			}
			case 0x3: { // LoadIdTokenCache
				break;
			}
			case 0x64: { // SetSystemProgramIdentification
				break;
			}
			case 0x6E: { // GetServiceEntryRequirementCache
				break;
			}
			case 0x6F: { // InvalidateServiceEntryRequirementCache
				break;
			}
			case 0x70: { // InvalidateTokenCache
				break;
			}
			case 0x78: { // GetNintendoAccountId
				break;
			}
			case 0x82: { // GetNintendoAccountUserResourceCache
				break;
			}
			case 0x83: { // RefreshNintendoAccountUserResourceCacheAsync
				break;
			}
			case 0x84: { // RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed
				break;
			}
			case 0x8C: { // GetNetworkServiceLicenseCache
				break;
			}
			case 0x8D: { // RefreshNetworkServiceLicenseCacheAsync
				break;
			}
			case 0x8E: { // RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed
				break;
			}
			case 0x96: { // CreateAuthorizationRequest
				break;
			}
			case 0xC8: { // IsRegistered
				break;
			}
			case 0xC9: { // RegisterAsync
				break;
			}
			case 0xCA: { // UnregisterAsync
				break;
			}
			case 0xCB: { // DeleteRegistrationInfoLocally
				break;
			}
			case 0xDC: { // SynchronizeProfileAsync
				break;
			}
			case 0xDD: { // UploadProfileAsync
				break;
			}
			case 0xDE: { // SynchronizeProfileAsyncIfSecondsElapsed
				break;
			}
			case 0xFA: { // IsLinkedWithNintendoAccount
				break;
			}
			case 0xFB: { // CreateProcedureToLinkWithNintendoAccount
				break;
			}
			case 0xFC: { // ResumeProcedureToLinkWithNintendoAccount
				break;
			}
			case 0xFF: { // CreateProcedureToUpdateLinkageStateOfNintendoAccount
				break;
			}
			case 0x100: { // ResumeProcedureToUpdateLinkageStateOfNintendoAccount
				break;
			}
			case 0x104: { // CreateProcedureToLinkNnidWithNintendoAccount
				break;
			}
			case 0x105: { // ResumeProcedureToLinkNnidWithNintendoAccount
				break;
			}
			case 0x118: { // ProxyProcedureToAcquireApplicationAuthorizationForNintendoAccount
				break;
			}
			case 0x3E5: { // DebugUnlinkNintendoAccountAsync
				break;
			}
			case 0x3E6: { // DebugSetAvailabilityErrorDetail
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IAdministrator");
		}
	}
}

public partial class IFloatingRegistrationRequest : _IFloatingRegistrationRequest_Base;
public abstract class _IFloatingRegistrationRequest_Base : IpcInterface {
	protected virtual void GetSessionId(Span<byte> _0) =>
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
	protected virtual void RegisterUser(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.RegisterUser not implemented");
	protected virtual void RegisterUserWithUid(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IFloatingRegistrationRequest.RegisterUserWithUid");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterNetworkServiceAccountAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.RegisterNetworkServiceAccountAsync not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RegisterNetworkServiceAccountWithUidAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.RegisterNetworkServiceAccountWithUidAsync not implemented");
	protected virtual void SetSystemProgramIdentification(ulong _0, ulong _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IFloatingRegistrationRequest.SetSystemProgramIdentification");
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IFloatingRegistrationRequest.EnsureIdTokenCacheAsync not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSessionId
				break;
			}
			case 0xC: { // GetAccountId
				break;
			}
			case 0xD: { // GetLinkedNintendoAccountId
				break;
			}
			case 0xE: { // GetNickname
				break;
			}
			case 0xF: { // GetProfileImage
				break;
			}
			case 0x15: { // LoadIdTokenCache
				break;
			}
			case 0x64: { // RegisterUser
				break;
			}
			case 0x65: { // RegisterUserWithUid
				break;
			}
			case 0x66: { // RegisterNetworkServiceAccountAsync
				break;
			}
			case 0x67: { // RegisterNetworkServiceAccountWithUidAsync
				break;
			}
			case 0x6E: { // SetSystemProgramIdentification
				break;
			}
			case 0x6F: { // EnsureIdTokenCacheAsync
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IFloatingRegistrationRequest");
		}
	}
}

public partial class IGuestLoginRequest : _IGuestLoginRequest_Base;
public abstract class _IGuestLoginRequest_Base : IpcInterface {
	protected virtual void GetSessionId(Span<byte> _0) =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSessionId
				break;
			}
			case 0xC: { // GetAccountId
				break;
			}
			case 0xD: { // GetLinkedNintendoAccountId
				break;
			}
			case 0xE: { // GetNickname
				break;
			}
			case 0xF: { // GetProfileImage
				break;
			}
			case 0x15: { // LoadIdTokenCache
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
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForApplication.CheckAvailability");
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
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForApplication.StoreOpenContext");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CheckAvailability
				break;
			}
			case 0x1: { // GetAccountId
				break;
			}
			case 0x2: { // EnsureIdTokenCacheAsync
				break;
			}
			case 0x3: { // LoadIdTokenCache
				break;
			}
			case 0x82: { // GetNintendoAccountUserResourceCacheForApplication
				break;
			}
			case 0x96: { // CreateAuthorizationRequest
				break;
			}
			case 0xA0: { // StoreOpenContext
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
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.CheckAvailability");
	protected virtual ulong GetAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetAccountId not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext EnsureIdTokenCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.EnsureIdTokenCacheAsync not implemented");
	protected virtual void LoadIdTokenCache(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.LoadIdTokenCache not implemented");
	protected virtual void SetSystemProgramIdentification(ulong _0, ulong _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.SetSystemProgramIdentification");
	protected virtual uint GetServiceEntryRequirementCache(ulong _0) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetServiceEntryRequirementCache not implemented");
	protected virtual void InvalidateServiceEntryRequirementCache(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.InvalidateServiceEntryRequirementCache");
	protected virtual void InvalidateTokenCache(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.InvalidateTokenCache");
	protected virtual ulong GetNintendoAccountId() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetNintendoAccountId not implemented");
	protected virtual void GetNintendoAccountUserResourceCache(out ulong _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.GetNintendoAccountUserResourceCache not implemented");
	protected virtual Nn.Account.Detail.IAsyncContext RefreshNintendoAccountUserResourceCacheAsync() =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.RefreshNintendoAccountUserResourceCacheAsync not implemented");
	protected virtual void RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed(uint _0, out byte _1, out Nn.Account.Detail.IAsyncContext _2) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed not implemented");
	protected virtual void GetNetworkServiceLicenseCache() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.GetNetworkServiceLicenseCache");
	protected virtual void RefreshNetworkServiceLicenseCacheAsync() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.RefreshNetworkServiceLicenseCacheAsync");
	protected virtual void RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed() =>
		Console.WriteLine("Stub hit for Nn.Account.Baas.IManagerForSystemService.RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed");
	protected virtual Nn.Account.Nas.IAuthorizationRequest CreateAuthorizationRequest(uint _0, KObject _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Account.Baas.IManagerForSystemService.CreateAuthorizationRequest not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CheckAvailability
				break;
			}
			case 0x1: { // GetAccountId
				break;
			}
			case 0x2: { // EnsureIdTokenCacheAsync
				break;
			}
			case 0x3: { // LoadIdTokenCache
				break;
			}
			case 0x64: { // SetSystemProgramIdentification
				break;
			}
			case 0x6E: { // GetServiceEntryRequirementCache
				break;
			}
			case 0x6F: { // InvalidateServiceEntryRequirementCache
				break;
			}
			case 0x70: { // InvalidateTokenCache
				break;
			}
			case 0x78: { // GetNintendoAccountId
				break;
			}
			case 0x82: { // GetNintendoAccountUserResourceCache
				break;
			}
			case 0x83: { // RefreshNintendoAccountUserResourceCacheAsync
				break;
			}
			case 0x84: { // RefreshNintendoAccountUserResourceCacheAsyncIfSecondsElapsed
				break;
			}
			case 0x8C: { // GetNetworkServiceLicenseCache
				break;
			}
			case 0x8D: { // RefreshNetworkServiceLicenseCacheAsync
				break;
			}
			case 0x8E: { // RefreshNetworkServiceLicenseCacheAsyncIfSecondsElapsed
				break;
			}
			case 0x96: { // CreateAuthorizationRequest
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Baas.IManagerForSystemService");
		}
	}
}

