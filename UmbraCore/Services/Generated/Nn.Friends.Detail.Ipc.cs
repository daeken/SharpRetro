using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Friends.Detail.Ipc;
public partial class IDaemonSuspendSessionService : _IDaemonSuspendSessionService_Base;
public abstract class _IDaemonSuspendSessionService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.IDaemonSuspendSessionService");
		}
	}
}

public partial class IFriendService : _IFriendService_Base;
public abstract class _IFriendService_Base : IpcInterface {
	protected virtual KObject GetCompletionEvent() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetCompletionEvent not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.Cancel");
	protected virtual void GetFriendListIds(uint _0, Span<byte> _1, Span<byte> _2, ulong _3, ulong _4) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendListIds not implemented");
	protected virtual void GetFriendList(uint _0, Span<byte> _1, Span<byte> _2, ulong _3, ulong _4) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendList not implemented");
	protected virtual void UpdateFriendInfo(Span<byte> _0, ulong _1, ulong _2, Span<ulong> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.UpdateFriendInfo not implemented");
	protected virtual void GetFriendProfileImage(Span<byte> _0, ulong _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendProfileImage not implemented");
	protected virtual void SendFriendRequestForApplication(Span<byte> _0, ulong _1, ulong _2, ulong _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestForApplication");
	protected virtual void AddFacedFriendRequestForApplication(Span<byte> _0, Span<byte> _1, Span<byte> _2, ulong _3, ulong _4, Span<byte> _5, Span<byte> _6, Span<byte> _7) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AddFacedFriendRequestForApplication");
	protected virtual void GetBlockedUserListIds(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetBlockedUserListIds not implemented");
	protected virtual void GetProfileList(Span<byte> _0, Span<ulong> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileList not implemented");
	protected virtual void DeclareOpenOnlinePlaySession(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeclareOpenOnlinePlaySession");
	protected virtual void DeclareCloseOnlinePlaySession(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeclareCloseOnlinePlaySession");
	protected virtual void UpdateUserPresence(Span<byte> _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.UpdateUserPresence");
	protected virtual void GetPlayHistoryRegistrationKey(byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryRegistrationKey not implemented");
	protected virtual void GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId not implemented");
	protected virtual void AddPlayHistory(Span<byte> _0, ulong _1, ulong _2, Span<byte> _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AddPlayHistory");
	protected virtual void GetProfileImageUrl(Span<byte> _0, uint _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileImageUrl not implemented");
	protected virtual uint GetFriendCount(Span<byte> _0, Span<byte> _1, ulong _2, ulong _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendCount not implemented");
	protected virtual uint GetNewlyFriendCount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetNewlyFriendCount not implemented");
	protected virtual void GetFriendDetailedInfo(Span<byte> _0, ulong _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendDetailedInfo not implemented");
	protected virtual void SyncFriendList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SyncFriendList");
	protected virtual void RequestSyncFriendList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.RequestSyncFriendList");
	protected virtual void LoadFriendSetting(Span<byte> _0, ulong _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.LoadFriendSetting not implemented");
	protected virtual void GetReceivedFriendRequestCount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetReceivedFriendRequestCount not implemented");
	protected virtual void GetFriendRequestList(uint _0, uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendRequestList not implemented");
	protected virtual void GetFriendCandidateList(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendCandidateList not implemented");
	protected virtual void GetNintendoNetworkIdInfo(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetNintendoNetworkIdInfo not implemented");
	protected virtual void GetSnsAccountLinkage() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountLinkage");
	protected virtual void GetSnsAccountProfile() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountProfile");
	protected virtual void GetSnsAccountFriendList() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountFriendList");
	protected virtual void GetBlockedUserList(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetBlockedUserList not implemented");
	protected virtual void SyncBlockedUserList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SyncBlockedUserList");
	protected virtual void GetProfileExtraList(Span<byte> _0, Span<ulong> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileExtraList not implemented");
	protected virtual void GetRelationship(Span<byte> _0, ulong _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetRelationship not implemented");
	protected virtual void GetUserPresenceView(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetUserPresenceView not implemented");
	protected virtual void GetPlayHistoryList(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryList not implemented");
	protected virtual void GetPlayHistoryStatistics(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryStatistics not implemented");
	protected virtual void LoadUserSetting(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.LoadUserSetting not implemented");
	protected virtual void SyncUserSetting(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SyncUserSetting");
	protected virtual void RequestListSummaryOverlayNotification() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.RequestListSummaryOverlayNotification");
	protected virtual void GetExternalApplicationCatalog(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetExternalApplicationCatalog not implemented");
	protected virtual void DropFriendNewlyFlags(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DropFriendNewlyFlags");
	protected virtual void DeleteFriend(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeleteFriend");
	protected virtual void DropFriendNewlyFlag(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DropFriendNewlyFlag");
	protected virtual void ChangeFriendFavoriteFlag(byte _0, Span<byte> _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangeFriendFavoriteFlag");
	protected virtual void ChangeFriendOnlineNotificationFlag(byte _0, Span<byte> _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangeFriendOnlineNotificationFlag");
	protected virtual void SendFriendRequest(uint _0, Span<byte> _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequest");
	protected virtual void SendFriendRequestWithApplicationInfo(uint _0, Span<byte> _1, ulong _2, Span<byte> _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestWithApplicationInfo");
	protected virtual void CancelFriendRequest(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.CancelFriendRequest");
	protected virtual void AcceptFriendRequest(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AcceptFriendRequest");
	protected virtual void RejectFriendRequest(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.RejectFriendRequest");
	protected virtual void ReadFriendRequest(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ReadFriendRequest");
	protected virtual void GetFacedFriendRequestRegistrationKey(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFacedFriendRequestRegistrationKey not implemented");
	protected virtual void AddFacedFriendRequest(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AddFacedFriendRequest");
	protected virtual void CancelFacedFriendRequest(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.CancelFacedFriendRequest");
	protected virtual void GetFacedFriendRequestProfileImage(Span<byte> _0, ulong _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFacedFriendRequestProfileImage not implemented");
	protected virtual void GetFacedFriendRequestProfileImageFromPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFacedFriendRequestProfileImageFromPath not implemented");
	protected virtual void SendFriendRequestWithExternalApplicationCatalogId(uint _0, Span<byte> _1, ulong _2, Span<byte> _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestWithExternalApplicationCatalogId");
	protected virtual void ResendFacedFriendRequest(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ResendFacedFriendRequest");
	protected virtual void SendFriendRequestWithNintendoNetworkIdInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, uint _4, Span<byte> _5, ulong _6) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestWithNintendoNetworkIdInfo");
	protected virtual void GetSnsAccountLinkPageUrl() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountLinkPageUrl");
	protected virtual void UnlinkSnsAccount() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.UnlinkSnsAccount");
	protected virtual void BlockUser(uint _0, Span<byte> _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.BlockUser");
	protected virtual void BlockUserWithApplicationInfo(uint _0, Span<byte> _1, ulong _2, Span<byte> _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.BlockUserWithApplicationInfo");
	protected virtual void UnblockUser(Span<byte> _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.UnblockUser");
	protected virtual void GetProfileExtraFromFriendCode(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileExtraFromFriendCode not implemented");
	protected virtual void DeletePlayHistory(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeletePlayHistory");
	protected virtual void ChangePresencePermission(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangePresencePermission");
	protected virtual void ChangeFriendRequestReception(byte _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangeFriendRequestReception");
	protected virtual void ChangePlayLogPermission(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangePlayLogPermission");
	protected virtual void IssueFriendCode(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.IssueFriendCode");
	protected virtual void ClearPlayLog(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ClearPlayLog");
	protected virtual void DeleteNetworkServiceAccountCache(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeleteNetworkServiceAccountCache");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCompletionEvent
				break;
			case 0x1: // Cancel
				break;
			case 0x2774: // GetFriendListIds
				break;
			case 0x2775: // GetFriendList
				break;
			case 0x2776: // UpdateFriendInfo
				break;
			case 0x277E: // GetFriendProfileImage
				break;
			case 0x27D8: // SendFriendRequestForApplication
				break;
			case 0x27E3: // AddFacedFriendRequestForApplication
				break;
			case 0x28A0: // GetBlockedUserListIds
				break;
			case 0x2904: // GetProfileList
				break;
			case 0x2968: // DeclareOpenOnlinePlaySession
				break;
			case 0x2969: // DeclareCloseOnlinePlaySession
				break;
			case 0x2972: // UpdateUserPresence
				break;
			case 0x29CC: // GetPlayHistoryRegistrationKey
				break;
			case 0x29CD: // GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId
				break;
			case 0x29CE: // AddPlayHistory
				break;
			case 0x2AF8: // GetProfileImageUrl
				break;
			case 0x4E84: // GetFriendCount
				break;
			case 0x4E85: // GetNewlyFriendCount
				break;
			case 0x4E86: // GetFriendDetailedInfo
				break;
			case 0x4E87: // SyncFriendList
				break;
			case 0x4E88: // RequestSyncFriendList
				break;
			case 0x4E8E: // LoadFriendSetting
				break;
			case 0x4EE8: // GetReceivedFriendRequestCount
				break;
			case 0x4EE9: // GetFriendRequestList
				break;
			case 0x4F4C: // GetFriendCandidateList
				break;
			case 0x4F4D: // GetNintendoNetworkIdInfo
				break;
			case 0x4F4E: // GetSnsAccountLinkage
				break;
			case 0x4F4F: // GetSnsAccountProfile
				break;
			case 0x4F50: // GetSnsAccountFriendList
				break;
			case 0x4FB0: // GetBlockedUserList
				break;
			case 0x4FB1: // SyncBlockedUserList
				break;
			case 0x5014: // GetProfileExtraList
				break;
			case 0x5015: // GetRelationship
				break;
			case 0x5078: // GetUserPresenceView
				break;
			case 0x50DC: // GetPlayHistoryList
				break;
			case 0x50DD: // GetPlayHistoryStatistics
				break;
			case 0x5140: // LoadUserSetting
				break;
			case 0x5141: // SyncUserSetting
				break;
			case 0x51A4: // RequestListSummaryOverlayNotification
				break;
			case 0x5208: // GetExternalApplicationCatalog
				break;
			case 0x7594: // DropFriendNewlyFlags
				break;
			case 0x7595: // DeleteFriend
				break;
			case 0x759E: // DropFriendNewlyFlag
				break;
			case 0x75A8: // ChangeFriendFavoriteFlag
				break;
			case 0x75A9: // ChangeFriendOnlineNotificationFlag
				break;
			case 0x75F8: // SendFriendRequest
				break;
			case 0x75F9: // SendFriendRequestWithApplicationInfo
				break;
			case 0x75FA: // CancelFriendRequest
				break;
			case 0x75FB: // AcceptFriendRequest
				break;
			case 0x75FC: // RejectFriendRequest
				break;
			case 0x75FD: // ReadFriendRequest
				break;
			case 0x7602: // GetFacedFriendRequestRegistrationKey
				break;
			case 0x7603: // AddFacedFriendRequest
				break;
			case 0x7604: // CancelFacedFriendRequest
				break;
			case 0x7605: // GetFacedFriendRequestProfileImage
				break;
			case 0x7606: // GetFacedFriendRequestProfileImageFromPath
				break;
			case 0x7607: // SendFriendRequestWithExternalApplicationCatalogId
				break;
			case 0x7608: // ResendFacedFriendRequest
				break;
			case 0x7609: // SendFriendRequestWithNintendoNetworkIdInfo
				break;
			case 0x765C: // GetSnsAccountLinkPageUrl
				break;
			case 0x765D: // UnlinkSnsAccount
				break;
			case 0x76C0: // BlockUser
				break;
			case 0x76C1: // BlockUserWithApplicationInfo
				break;
			case 0x76C2: // UnblockUser
				break;
			case 0x7724: // GetProfileExtraFromFriendCode
				break;
			case 0x77EC: // DeletePlayHistory
				break;
			case 0x785A: // ChangePresencePermission
				break;
			case 0x785B: // ChangeFriendRequestReception
				break;
			case 0x785C: // ChangePlayLogPermission
				break;
			case 0x7864: // IssueFriendCode
				break;
			case 0x786E: // ClearPlayLog
				break;
			case 0xC2EC: // DeleteNetworkServiceAccountCache
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.IFriendService");
		}
	}
}

public partial class IFriendServiceCreator : _IFriendServiceCreator_Base;
public abstract class _IFriendServiceCreator_Base : IpcInterface {
	protected virtual Nn.Friends.Detail.Ipc.IFriendService Create() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendServiceCreator.Create not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Create
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.IFriendServiceCreator");
		}
	}
}

public partial class INotificationService : _INotificationService_Base;
public abstract class _INotificationService_Base : IpcInterface {
	protected virtual KObject GetEvent() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.INotificationService.GetEvent not implemented");
	protected virtual void Clear() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.INotificationService.Clear");
	protected virtual void Pop() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.INotificationService.Pop not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetEvent
				break;
			case 0x1: // Clear
				break;
			case 0x2: // Pop
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.INotificationService");
		}
	}
}

public partial class IServiceCreator : _IServiceCreator_Base;
public abstract class _IServiceCreator_Base : IpcInterface {
	protected virtual Nn.Friends.Detail.Ipc.IFriendService CreateFriendService() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IServiceCreator.CreateFriendService not implemented");
	protected virtual Nn.Friends.Detail.Ipc.INotificationService CreateNotificationService(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IServiceCreator.CreateNotificationService not implemented");
	protected virtual Nn.Friends.Detail.Ipc.IDaemonSuspendSessionService CreateDaemonSuspendSessionService() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IServiceCreator.CreateDaemonSuspendSessionService not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateFriendService
				break;
			case 0x1: // CreateNotificationService
				break;
			case 0x2: // CreateDaemonSuspendSessionService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.IServiceCreator");
		}
	}
}

