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

