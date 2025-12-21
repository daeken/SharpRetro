using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Friends.Detail.Ipc;
public partial class IDaemonSuspendSessionService : _IDaemonSuspendSessionService_Base;
public abstract class _IDaemonSuspendSessionService_Base : IpcInterface {
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
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
	protected virtual void GetFriendListIds(uint _0, byte[] _1, byte[] _2, ulong _3, ulong _4, out uint _5, Span<ulong> _6) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendListIds not implemented");
	protected virtual void GetFriendList(uint _0, byte[] _1, byte[] _2, ulong _3, ulong _4, out uint _5, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendList not implemented");
	protected virtual void UpdateFriendInfo(byte[] _0, ulong _1, ulong _2, Span<ulong> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.UpdateFriendInfo not implemented");
	protected virtual void GetFriendProfileImage(byte[] _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendProfileImage not implemented");
	protected virtual void SendFriendRequestForApplication(byte[] _0, ulong _1, ulong _2, ulong _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestForApplication");
	protected virtual void AddFacedFriendRequestForApplication(byte[] _0, byte[] _1, byte[] _2, ulong _3, ulong _4, Span<byte> _5, Span<byte> _6, Span<byte> _7) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AddFacedFriendRequestForApplication");
	protected virtual void GetBlockedUserListIds(uint _0, byte[] _1, out uint _2, Span<ulong> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetBlockedUserListIds not implemented");
	protected virtual void GetProfileList(byte[] _0, Span<ulong> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileList not implemented");
	protected virtual void DeclareOpenOnlinePlaySession(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeclareOpenOnlinePlaySession");
	protected virtual void DeclareCloseOnlinePlaySession(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeclareCloseOnlinePlaySession");
	protected virtual void UpdateUserPresence(byte[] _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.UpdateUserPresence");
	protected virtual void GetPlayHistoryRegistrationKey(byte _0, byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryRegistrationKey not implemented");
	protected virtual void GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId(byte _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId not implemented");
	protected virtual void AddPlayHistory(byte[] _0, ulong _1, ulong _2, Span<byte> _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AddPlayHistory");
	protected virtual void GetProfileImageUrl(byte[] _0, uint _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileImageUrl not implemented");
	protected virtual uint GetFriendCount(byte[] _0, byte[] _1, ulong _2, ulong _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendCount not implemented");
	protected virtual uint GetNewlyFriendCount(byte[] _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetNewlyFriendCount not implemented");
	protected virtual void GetFriendDetailedInfo(byte[] _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendDetailedInfo not implemented");
	protected virtual void SyncFriendList(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SyncFriendList");
	protected virtual void RequestSyncFriendList(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.RequestSyncFriendList");
	protected virtual void LoadFriendSetting(byte[] _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.LoadFriendSetting not implemented");
	protected virtual void GetReceivedFriendRequestCount(byte[] _0, out uint _1, out uint _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetReceivedFriendRequestCount not implemented");
	protected virtual void GetFriendRequestList(uint _0, uint _1, byte[] _2, out uint _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendRequestList not implemented");
	protected virtual void GetFriendCandidateList(uint _0, byte[] _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFriendCandidateList not implemented");
	protected virtual void GetNintendoNetworkIdInfo(uint _0, byte[] _1, out uint _2, Span<byte> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetNintendoNetworkIdInfo not implemented");
	protected virtual void GetSnsAccountLinkage() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountLinkage");
	protected virtual void GetSnsAccountProfile() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountProfile");
	protected virtual void GetSnsAccountFriendList() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountFriendList");
	protected virtual void GetBlockedUserList(uint _0, byte[] _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetBlockedUserList not implemented");
	protected virtual void SyncBlockedUserList(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SyncBlockedUserList");
	protected virtual void GetProfileExtraList(byte[] _0, Span<ulong> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileExtraList not implemented");
	protected virtual void GetRelationship(byte[] _0, ulong _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetRelationship not implemented");
	protected virtual void GetUserPresenceView(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetUserPresenceView not implemented");
	protected virtual void GetPlayHistoryList(uint _0, byte[] _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryList not implemented");
	protected virtual void GetPlayHistoryStatistics(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetPlayHistoryStatistics not implemented");
	protected virtual void LoadUserSetting(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.LoadUserSetting not implemented");
	protected virtual void SyncUserSetting(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SyncUserSetting");
	protected virtual void RequestListSummaryOverlayNotification() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.RequestListSummaryOverlayNotification");
	protected virtual void GetExternalApplicationCatalog(byte[] _0, byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetExternalApplicationCatalog not implemented");
	protected virtual void DropFriendNewlyFlags(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DropFriendNewlyFlags");
	protected virtual void DeleteFriend(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeleteFriend");
	protected virtual void DropFriendNewlyFlag(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DropFriendNewlyFlag");
	protected virtual void ChangeFriendFavoriteFlag(byte _0, byte[] _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangeFriendFavoriteFlag");
	protected virtual void ChangeFriendOnlineNotificationFlag(byte _0, byte[] _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangeFriendOnlineNotificationFlag");
	protected virtual void SendFriendRequest(uint _0, byte[] _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequest");
	protected virtual void SendFriendRequestWithApplicationInfo(uint _0, byte[] _1, ulong _2, byte[] _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestWithApplicationInfo");
	protected virtual void CancelFriendRequest(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.CancelFriendRequest");
	protected virtual void AcceptFriendRequest(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AcceptFriendRequest");
	protected virtual void RejectFriendRequest(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.RejectFriendRequest");
	protected virtual void ReadFriendRequest(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ReadFriendRequest");
	protected virtual void GetFacedFriendRequestRegistrationKey(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFacedFriendRequestRegistrationKey not implemented");
	protected virtual void AddFacedFriendRequest(byte[] _0, byte[] _1, byte[] _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.AddFacedFriendRequest");
	protected virtual void CancelFacedFriendRequest(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.CancelFacedFriendRequest");
	protected virtual void GetFacedFriendRequestProfileImage(byte[] _0, ulong _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFacedFriendRequestProfileImage not implemented");
	protected virtual void GetFacedFriendRequestProfileImageFromPath(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetFacedFriendRequestProfileImageFromPath not implemented");
	protected virtual void SendFriendRequestWithExternalApplicationCatalogId(uint _0, byte[] _1, ulong _2, byte[] _3, Span<byte> _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestWithExternalApplicationCatalogId");
	protected virtual void ResendFacedFriendRequest(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ResendFacedFriendRequest");
	protected virtual void SendFriendRequestWithNintendoNetworkIdInfo(byte[] _0, byte[] _1, byte[] _2, byte[] _3, uint _4, byte[] _5, ulong _6) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.SendFriendRequestWithNintendoNetworkIdInfo");
	protected virtual void GetSnsAccountLinkPageUrl() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.GetSnsAccountLinkPageUrl");
	protected virtual void UnlinkSnsAccount() =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.UnlinkSnsAccount");
	protected virtual void BlockUser(uint _0, byte[] _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.BlockUser");
	protected virtual void BlockUserWithApplicationInfo(uint _0, byte[] _1, ulong _2, byte[] _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.BlockUserWithApplicationInfo");
	protected virtual void UnblockUser(byte[] _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.UnblockUser");
	protected virtual void GetProfileExtraFromFriendCode(byte[] _0, byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendService.GetProfileExtraFromFriendCode not implemented");
	protected virtual void DeletePlayHistory(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeletePlayHistory");
	protected virtual void ChangePresencePermission(uint _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangePresencePermission");
	protected virtual void ChangeFriendRequestReception(byte _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangeFriendRequestReception");
	protected virtual void ChangePlayLogPermission(uint _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ChangePlayLogPermission");
	protected virtual void IssueFriendCode(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.IssueFriendCode");
	protected virtual void ClearPlayLog(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.ClearPlayLog");
	protected virtual void DeleteNetworkServiceAccountCache(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Friends.Detail.Ipc.IFriendService.DeleteNetworkServiceAccountCache");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCompletionEvent
				om.Initialize(0, 1, 0);
				var _return = GetCompletionEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Cancel
				om.Initialize(0, 0, 0);
				Cancel();
				break;
			}
			case 0x2774: { // GetFriendListIds
				om.Initialize(0, 0, 4);
				GetFriendListIds(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetBytes(32, 0x10), im.GetData<ulong>(48), im.Pid, out var _0, im.GetSpan<ulong>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2775: { // GetFriendList
				om.Initialize(0, 0, 4);
				GetFriendList(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetBytes(32, 0x10), im.GetData<ulong>(48), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2776: { // UpdateFriendInfo
				om.Initialize(0, 0, 0);
				UpdateFriendInfo(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.Pid, im.GetSpan<ulong>(0x9, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x277E: { // GetFriendProfileImage
				om.Initialize(0, 0, 4);
				GetFriendProfileImage(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x27D8: { // SendFriendRequestForApplication
				om.Initialize(0, 0, 0);
				SendFriendRequestForApplication(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetData<ulong>(32), im.Pid, im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				break;
			}
			case 0x27E3: { // AddFacedFriendRequestForApplication
				om.Initialize(0, 0, 0);
				AddFacedFriendRequestForApplication(im.GetBytes(8, 0x40), im.GetBytes(72, 0x21), im.GetBytes(112, 0x10), im.GetData<ulong>(128), im.Pid, im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x28A0: { // GetBlockedUserListIds
				om.Initialize(0, 0, 4);
				GetBlockedUserListIds(im.GetData<uint>(8), im.GetBytes(16, 0x10), out var _0, im.GetSpan<ulong>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2904: { // GetProfileList
				om.Initialize(0, 0, 0);
				GetProfileList(im.GetBytes(8, 0x10), im.GetSpan<ulong>(0x9, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x2968: { // DeclareOpenOnlinePlaySession
				om.Initialize(0, 0, 0);
				DeclareOpenOnlinePlaySession(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2969: { // DeclareCloseOnlinePlaySession
				om.Initialize(0, 0, 0);
				DeclareCloseOnlinePlaySession(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2972: { // UpdateUserPresence
				om.Initialize(0, 0, 0);
				UpdateUserPresence(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.Pid, im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x29CC: { // GetPlayHistoryRegistrationKey
				om.Initialize(0, 0, 0);
				GetPlayHistoryRegistrationKey(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x29CD: { // GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId
				om.Initialize(0, 0, 0);
				GetPlayHistoryRegistrationKeyWithNetworkServiceAccountId(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x29CE: { // AddPlayHistory
				om.Initialize(0, 0, 0);
				AddPlayHistory(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.Pid, im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1), im.GetSpan<byte>(0x19, 2));
				break;
			}
			case 0x2AF8: { // GetProfileImageUrl
				om.Initialize(0, 0, 160);
				GetProfileImageUrl(im.GetBytes(8, 0xA0), im.GetData<uint>(168), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4E84: { // GetFriendCount
				om.Initialize(0, 0, 4);
				var _return = GetFriendCount(im.GetBytes(8, 0x10), im.GetBytes(24, 0x10), im.GetData<ulong>(40), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x4E85: { // GetNewlyFriendCount
				om.Initialize(0, 0, 4);
				var _return = GetNewlyFriendCount(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x4E86: { // GetFriendDetailedInfo
				om.Initialize(0, 0, 0);
				GetFriendDetailedInfo(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x4E87: { // SyncFriendList
				om.Initialize(0, 0, 0);
				SyncFriendList(im.GetBytes(8, 0x10));
				break;
			}
			case 0x4E88: { // RequestSyncFriendList
				om.Initialize(0, 0, 0);
				RequestSyncFriendList(im.GetBytes(8, 0x10));
				break;
			}
			case 0x4E8E: { // LoadFriendSetting
				om.Initialize(0, 0, 0);
				LoadFriendSetting(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x4EE8: { // GetReceivedFriendRequestCount
				om.Initialize(0, 0, 8);
				GetReceivedFriendRequestCount(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4EE9: { // GetFriendRequestList
				om.Initialize(0, 0, 4);
				GetFriendRequestList(im.GetData<uint>(8), im.GetData<uint>(12), im.GetBytes(16, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x4F4C: { // GetFriendCandidateList
				om.Initialize(0, 0, 4);
				GetFriendCandidateList(im.GetData<uint>(8), im.GetBytes(16, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x4F4D: { // GetNintendoNetworkIdInfo
				om.Initialize(0, 0, 4);
				GetNintendoNetworkIdInfo(im.GetData<uint>(8), im.GetBytes(16, 0x10), out var _0, im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x4F4E: { // GetSnsAccountLinkage
				om.Initialize(0, 0, 0);
				GetSnsAccountLinkage();
				break;
			}
			case 0x4F4F: { // GetSnsAccountProfile
				om.Initialize(0, 0, 0);
				GetSnsAccountProfile();
				break;
			}
			case 0x4F50: { // GetSnsAccountFriendList
				om.Initialize(0, 0, 0);
				GetSnsAccountFriendList();
				break;
			}
			case 0x4FB0: { // GetBlockedUserList
				om.Initialize(0, 0, 4);
				GetBlockedUserList(im.GetData<uint>(8), im.GetBytes(16, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x4FB1: { // SyncBlockedUserList
				om.Initialize(0, 0, 0);
				SyncBlockedUserList(im.GetBytes(8, 0x10));
				break;
			}
			case 0x5014: { // GetProfileExtraList
				om.Initialize(0, 0, 0);
				GetProfileExtraList(im.GetBytes(8, 0x10), im.GetSpan<ulong>(0x9, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x5015: { // GetRelationship
				om.Initialize(0, 0, 8);
				GetRelationship(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5078: { // GetUserPresenceView
				om.Initialize(0, 0, 0);
				GetUserPresenceView(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x50DC: { // GetPlayHistoryList
				om.Initialize(0, 0, 4);
				GetPlayHistoryList(im.GetData<uint>(8), im.GetBytes(16, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x50DD: { // GetPlayHistoryStatistics
				om.Initialize(0, 0, 16);
				GetPlayHistoryStatistics(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5140: { // LoadUserSetting
				om.Initialize(0, 0, 0);
				LoadUserSetting(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x5141: { // SyncUserSetting
				om.Initialize(0, 0, 0);
				SyncUserSetting(im.GetBytes(8, 0x10));
				break;
			}
			case 0x51A4: { // RequestListSummaryOverlayNotification
				om.Initialize(0, 0, 0);
				RequestListSummaryOverlayNotification();
				break;
			}
			case 0x5208: { // GetExternalApplicationCatalog
				om.Initialize(0, 0, 0);
				GetExternalApplicationCatalog(im.GetBytes(8, 0x8), im.GetBytes(16, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x7594: { // DropFriendNewlyFlags
				om.Initialize(0, 0, 0);
				DropFriendNewlyFlags(im.GetBytes(8, 0x10));
				break;
			}
			case 0x7595: { // DeleteFriend
				om.Initialize(0, 0, 0);
				DeleteFriend(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x759E: { // DropFriendNewlyFlag
				om.Initialize(0, 0, 0);
				DropFriendNewlyFlag(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x75A8: { // ChangeFriendFavoriteFlag
				om.Initialize(0, 0, 0);
				ChangeFriendFavoriteFlag(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32));
				break;
			}
			case 0x75A9: { // ChangeFriendOnlineNotificationFlag
				om.Initialize(0, 0, 0);
				ChangeFriendOnlineNotificationFlag(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32));
				break;
			}
			case 0x75F8: { // SendFriendRequest
				om.Initialize(0, 0, 0);
				SendFriendRequest(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32));
				break;
			}
			case 0x75F9: { // SendFriendRequestWithApplicationInfo
				om.Initialize(0, 0, 0);
				SendFriendRequestWithApplicationInfo(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32), im.GetBytes(40, 0x10), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				break;
			}
			case 0x75FA: { // CancelFriendRequest
				om.Initialize(0, 0, 0);
				CancelFriendRequest(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x75FB: { // AcceptFriendRequest
				om.Initialize(0, 0, 0);
				AcceptFriendRequest(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x75FC: { // RejectFriendRequest
				om.Initialize(0, 0, 0);
				RejectFriendRequest(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x75FD: { // ReadFriendRequest
				om.Initialize(0, 0, 0);
				ReadFriendRequest(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x7602: { // GetFacedFriendRequestRegistrationKey
				om.Initialize(0, 0, 64);
				GetFacedFriendRequestRegistrationKey(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7603: { // AddFacedFriendRequest
				om.Initialize(0, 0, 0);
				AddFacedFriendRequest(im.GetBytes(8, 0x40), im.GetBytes(72, 0x21), im.GetBytes(112, 0x10), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x7604: { // CancelFacedFriendRequest
				om.Initialize(0, 0, 0);
				CancelFacedFriendRequest(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x7605: { // GetFacedFriendRequestProfileImage
				om.Initialize(0, 0, 4);
				GetFacedFriendRequestProfileImage(im.GetBytes(8, 0x10), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7606: { // GetFacedFriendRequestProfileImageFromPath
				om.Initialize(0, 0, 4);
				GetFacedFriendRequestProfileImageFromPath(im.GetSpan<byte>(0x9, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7607: { // SendFriendRequestWithExternalApplicationCatalogId
				om.Initialize(0, 0, 0);
				SendFriendRequestWithExternalApplicationCatalogId(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32), im.GetBytes(40, 0x10), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				break;
			}
			case 0x7608: { // ResendFacedFriendRequest
				om.Initialize(0, 0, 0);
				ResendFacedFriendRequest(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x7609: { // SendFriendRequestWithNintendoNetworkIdInfo
				om.Initialize(0, 0, 0);
				SendFriendRequestWithNintendoNetworkIdInfo(im.GetBytes(8, 0x20), im.GetBytes(40, 0x10), im.GetBytes(56, 0x20), im.GetBytes(88, 0x10), im.GetData<uint>(104), im.GetBytes(112, 0x10), im.GetData<ulong>(128));
				break;
			}
			case 0x765C: { // GetSnsAccountLinkPageUrl
				om.Initialize(0, 0, 0);
				GetSnsAccountLinkPageUrl();
				break;
			}
			case 0x765D: { // UnlinkSnsAccount
				om.Initialize(0, 0, 0);
				UnlinkSnsAccount();
				break;
			}
			case 0x76C0: { // BlockUser
				om.Initialize(0, 0, 0);
				BlockUser(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32));
				break;
			}
			case 0x76C1: { // BlockUserWithApplicationInfo
				om.Initialize(0, 0, 0);
				BlockUserWithApplicationInfo(im.GetData<uint>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32), im.GetBytes(40, 0x10), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x76C2: { // UnblockUser
				om.Initialize(0, 0, 0);
				UnblockUser(im.GetBytes(8, 0x10), im.GetData<ulong>(24));
				break;
			}
			case 0x7724: { // GetProfileExtraFromFriendCode
				om.Initialize(0, 0, 0);
				GetProfileExtraFromFriendCode(im.GetBytes(8, 0x20), im.GetBytes(40, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x77EC: { // DeletePlayHistory
				om.Initialize(0, 0, 0);
				DeletePlayHistory(im.GetBytes(8, 0x10));
				break;
			}
			case 0x785A: { // ChangePresencePermission
				om.Initialize(0, 0, 0);
				ChangePresencePermission(im.GetData<uint>(8), im.GetBytes(16, 0x10));
				break;
			}
			case 0x785B: { // ChangeFriendRequestReception
				om.Initialize(0, 0, 0);
				ChangeFriendRequestReception(im.GetData<byte>(8), im.GetBytes(16, 0x10));
				break;
			}
			case 0x785C: { // ChangePlayLogPermission
				om.Initialize(0, 0, 0);
				ChangePlayLogPermission(im.GetData<uint>(8), im.GetBytes(16, 0x10));
				break;
			}
			case 0x7864: { // IssueFriendCode
				om.Initialize(0, 0, 0);
				IssueFriendCode(im.GetBytes(8, 0x10));
				break;
			}
			case 0x786E: { // ClearPlayLog
				om.Initialize(0, 0, 0);
				ClearPlayLog(im.GetBytes(8, 0x10));
				break;
			}
			case 0xC2EC: { // DeleteNetworkServiceAccountCache
				om.Initialize(0, 0, 0);
				DeleteNetworkServiceAccountCache(im.GetBytes(8, 0x10));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.IFriendService");
		}
	}
}

public partial class IFriendServiceCreator : _IFriendServiceCreator_Base;
public abstract class _IFriendServiceCreator_Base : IpcInterface {
	protected virtual Nn.Friends.Detail.Ipc.IFriendService Create() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IFriendServiceCreator.Create not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Create
				om.Initialize(1, 0, 0);
				var _return = Create();
				om.Move(0, CreateHandle(_return));
				break;
			}
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
	protected virtual void Pop(out byte[] _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.INotificationService.Pop not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetEvent
				om.Initialize(0, 1, 0);
				var _return = GetEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Clear
				om.Initialize(0, 0, 0);
				Clear();
				break;
			}
			case 0x2: { // Pop
				om.Initialize(0, 0, 16);
				Pop(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.INotificationService");
		}
	}
}

public partial class IServiceCreator : _IServiceCreator_Base;
public abstract class _IServiceCreator_Base : IpcInterface {
	protected virtual Nn.Friends.Detail.Ipc.IFriendService CreateFriendService() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IServiceCreator.CreateFriendService not implemented");
	protected virtual Nn.Friends.Detail.Ipc.INotificationService CreateNotificationService(byte[] _0) =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IServiceCreator.CreateNotificationService not implemented");
	protected virtual Nn.Friends.Detail.Ipc.IDaemonSuspendSessionService CreateDaemonSuspendSessionService() =>
		throw new NotImplementedException("Nn.Friends.Detail.Ipc.IServiceCreator.CreateDaemonSuspendSessionService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateFriendService
				om.Initialize(1, 0, 0);
				var _return = CreateFriendService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateNotificationService
				om.Initialize(1, 0, 0);
				var _return = CreateNotificationService(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // CreateDaemonSuspendSessionService
				om.Initialize(1, 0, 0);
				var _return = CreateDaemonSuspendSessionService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Friends.Detail.Ipc.IServiceCreator");
		}
	}
}

