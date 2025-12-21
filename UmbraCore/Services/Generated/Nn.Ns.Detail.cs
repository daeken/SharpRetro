using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ns.Detail;
public partial class IAccountProxyInterface : _IAccountProxyInterface_Base;
public abstract class _IAccountProxyInterface_Base : IpcInterface {
	protected virtual void CreateUserAccount(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IAccountProxyInterface.CreateUserAccount");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserAccount
				om.Initialize(0, 0, 0);
				CreateUserAccount(im.GetBytes(8, 0x21), im.GetSpan<byte>(0x5, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAccountProxyInterface");
		}
	}
}

public partial class IApplicationManagerInterface : _IApplicationManagerInterface_Base;
public abstract class _IApplicationManagerInterface_Base : IpcInterface {
	protected virtual void ListApplicationRecord(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecord not implemented");
	protected virtual void GenerateApplicationRecordCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GenerateApplicationRecordCount not implemented");
	protected virtual KObject GetApplicationRecordUpdateSystemEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecordUpdateSystemEvent not implemented");
	protected virtual void GetApplicationViewDeprecated(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationViewDeprecated not implemented");
	protected virtual void DeleteApplicationEntity(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationEntity");
	protected virtual void DeleteApplicationCompletely(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationCompletely");
	protected virtual void IsAnyApplicationEntityRedundant(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsAnyApplicationEntityRedundant not implemented");
	protected virtual void DeleteRedundantApplicationEntity() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteRedundantApplicationEntity");
	protected virtual void IsApplicationEntityMovable(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationEntityMovable not implemented");
	protected virtual void MoveApplicationEntity(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.MoveApplicationEntity");
	protected virtual void CalculateApplicationOccupiedSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationOccupiedSize not implemented");
	protected virtual void PushApplicationRecord(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushApplicationRecord");
	protected virtual void ListApplicationRecordContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecordContentMeta not implemented");
	protected virtual void LaunchApplication(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchApplication not implemented");
	protected virtual void GetApplicationContentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationContentPath not implemented");
	protected virtual void TerminateApplication(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateApplication");
	protected virtual void ResolveApplicationContentPath(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResolveApplicationContentPath");
	protected virtual void BeginInstallApplication(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.BeginInstallApplication");
	protected virtual void DeleteApplicationRecord(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationRecord");
	protected virtual void RequestApplicationUpdateInfo(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestApplicationUpdateInfo not implemented");
	protected virtual void CancelApplicationDownload(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CancelApplicationDownload");
	protected virtual void ResumeApplicationDownload(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeApplicationDownload");
	protected virtual void UpdateVersionList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.UpdateVersionList");
	protected virtual void PushLaunchVersion(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushLaunchVersion");
	protected virtual void ListRequiredVersion(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListRequiredVersion not implemented");
	protected virtual void CheckApplicationLaunchVersion(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationLaunchVersion");
	protected virtual void CheckApplicationLaunchRights(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationLaunchRights");
	protected virtual void GetApplicationLogoData(byte[] _0, Span<byte> _1, out byte[] _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationLogoData not implemented");
	protected virtual void CalculateApplicationDownloadRequiredSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationDownloadRequiredSize not implemented");
	protected virtual void CleanupSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupSdCard");
	protected virtual void CheckSdCardMountStatus() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckSdCardMountStatus");
	protected virtual KObject GetSdCardMountStatusChangedEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSdCardMountStatusChangedEvent not implemented");
	protected virtual KObject GetGameCardAttachmentEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardAttachmentEvent not implemented");
	protected virtual void GetGameCardAttachmentInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardAttachmentInfo not implemented");
	protected virtual void GetTotalSpaceSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetTotalSpaceSize not implemented");
	protected virtual void GetFreeSpaceSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetFreeSpaceSize not implemented");
	protected virtual KObject GetSdCardRemovedEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSdCardRemovedEvent not implemented");
	protected virtual KObject GetGameCardUpdateDetectionEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardUpdateDetectionEvent not implemented");
	protected virtual void DisableApplicationAutoDelete(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DisableApplicationAutoDelete");
	protected virtual void EnableApplicationAutoDelete(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationAutoDelete");
	protected virtual void GetApplicationDesiredLanguage(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDesiredLanguage not implemented");
	protected virtual void SetApplicationTerminateResult(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.SetApplicationTerminateResult");
	protected virtual void ClearApplicationTerminateResult(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ClearApplicationTerminateResult");
	protected virtual void GetLastSdCardMountUnexpectedResult() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastSdCardMountUnexpectedResult");
	protected virtual void ConvertApplicationLanguageToLanguageCode(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ConvertApplicationLanguageToLanguageCode not implemented");
	protected virtual void ConvertLanguageCodeToApplicationLanguage(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ConvertLanguageCodeToApplicationLanguage not implemented");
	protected virtual void GetBackgroundDownloadStressTaskInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetBackgroundDownloadStressTaskInfo not implemented");
	protected virtual Nn.Ns.Detail.IGameCardStopper GetGameCardStopper() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardStopper not implemented");
	protected virtual void IsSystemProgramInstalled(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsSystemProgramInstalled not implemented");
	protected virtual void StartApplyDeltaTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.StartApplyDeltaTask");
	protected virtual Nn.Ns.Detail.IRequestServerStopper GetRequestServerStopper() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetRequestServerStopper not implemented");
	protected virtual void GetBackgroundApplyDeltaStressTaskInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetBackgroundApplyDeltaStressTaskInfo not implemented");
	protected virtual void CancelApplicationApplyDelta(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CancelApplicationApplyDelta");
	protected virtual void ResumeApplicationApplyDelta(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeApplicationApplyDelta");
	protected virtual void CalculateApplicationApplyDeltaRequiredSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationApplyDeltaRequiredSize not implemented");
	protected virtual void ResumeAll() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeAll");
	protected virtual void GetStorageSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetStorageSize not implemented");
	protected virtual void RequestDownloadApplication(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplication not implemented");
	protected virtual void RequestDownloadAddOnContent(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadAddOnContent not implemented");
	protected virtual void DownloadApplication(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DownloadApplication");
	protected virtual void CheckApplicationResumeRights(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationResumeRights");
	protected virtual KObject GetDynamicCommitEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetDynamicCommitEvent not implemented");
	protected virtual void RequestUpdateApplication2(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestUpdateApplication2 not implemented");
	protected virtual void EnableApplicationCrashReport(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationCrashReport");
	protected virtual void IsApplicationCrashReportEnabled(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationCrashReportEnabled not implemented");
	protected virtual void BoostSystemMemoryResourceLimit(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.BoostSystemMemoryResourceLimit");
	protected virtual void Unknown91() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown91");
	protected virtual void Unknown92() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown92");
	protected virtual void Unknown93() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown93");
	protected virtual void LaunchApplication2() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.LaunchApplication2");
	protected virtual void Unknown95() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown95");
	protected virtual void Unknown96() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown96");
	protected virtual void Unknown97() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown97");
	protected virtual void Unknown98() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown98");
	protected virtual void ResetToFactorySettings() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetToFactorySettings");
	protected virtual void ResetToFactorySettingsWithoutUserSaveData() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetToFactorySettingsWithoutUserSaveData");
	protected virtual void ResetToFactorySettingsForRefurbishment() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetToFactorySettingsForRefurbishment");
	protected virtual void CalculateUserSaveDataStatistics(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateUserSaveDataStatistics not implemented");
	protected virtual Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll DeleteUserSaveDataAll(byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.DeleteUserSaveDataAll not implemented");
	protected virtual void DeleteUserSystemSaveData(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteUserSystemSaveData");
	protected virtual void Unknown211() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown211");
	protected virtual void UnregisterNetworkServiceAccount(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.UnregisterNetworkServiceAccount");
	protected virtual void Unknown221() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown221");
	protected virtual KObject GetApplicationShellEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationShellEvent not implemented");
	protected virtual void PopApplicationShellEventInfo(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.PopApplicationShellEventInfo not implemented");
	protected virtual void LaunchLibraryApplet(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchLibraryApplet not implemented");
	protected virtual void TerminateLibraryApplet(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateLibraryApplet");
	protected virtual void LaunchSystemApplet(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchSystemApplet not implemented");
	protected virtual void TerminateSystemApplet(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateSystemApplet");
	protected virtual void LaunchOverlayApplet(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchOverlayApplet not implemented");
	protected virtual void TerminateOverlayApplet(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateOverlayApplet");
	protected virtual void GetApplicationControlData(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationControlData not implemented");
	protected virtual void InvalidateAllApplicationControlCache() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InvalidateAllApplicationControlCache");
	protected virtual void RequestDownloadApplicationControlData(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplicationControlData not implemented");
	protected virtual void GetMaxApplicationControlCacheCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetMaxApplicationControlCacheCount not implemented");
	protected virtual void InvalidateApplicationControlCache(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InvalidateApplicationControlCache");
	protected virtual void ListApplicationControlCacheEntryInfo(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationControlCacheEntryInfo not implemented");
	protected virtual void Unknown406() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown406");
	protected virtual void RequestCheckGameCardRegistration(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestCheckGameCardRegistration not implemented");
	protected virtual void RequestGameCardRegistrationGoldPoint(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestGameCardRegistrationGoldPoint not implemented");
	protected virtual void RequestRegisterGameCard(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestRegisterGameCard not implemented");
	protected virtual KObject GetGameCardMountFailureEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardMountFailureEvent not implemented");
	protected virtual void IsGameCardInserted(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsGameCardInserted not implemented");
	protected virtual void EnsureGameCardAccess() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnsureGameCardAccess");
	protected virtual void GetLastGameCardMountFailureResult() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastGameCardMountFailureResult");
	protected virtual void ListApplicationIdOnGameCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationIdOnGameCard");
	protected virtual void CountApplicationContentMeta(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CountApplicationContentMeta not implemented");
	protected virtual void ListApplicationContentMetaStatus(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationContentMetaStatus not implemented");
	protected virtual void ListAvailableAddOnContent(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListAvailableAddOnContent not implemented");
	protected virtual void GetOwnedApplicationContentMetaStatus(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetOwnedApplicationContentMetaStatus not implemented");
	protected virtual void RegisterContentsExternalKey(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RegisterContentsExternalKey");
	protected virtual void ListApplicationContentMetaStatusWithRightsCheck(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationContentMetaStatusWithRightsCheck not implemented");
	protected virtual void GetContentMetaStorage(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetContentMetaStorage not implemented");
	protected virtual void Unknown607() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown607");
	protected virtual void PushDownloadTaskList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushDownloadTaskList");
	protected virtual void ClearTaskStatusList() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ClearTaskStatusList");
	protected virtual void RequestDownloadTaskList() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadTaskList");
	protected virtual void RequestEnsureDownloadTask(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestEnsureDownloadTask not implemented");
	protected virtual void ListDownloadTaskStatus(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListDownloadTaskStatus not implemented");
	protected virtual void RequestDownloadTaskListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadTaskListData not implemented");
	protected virtual void RequestVersionList() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestVersionList");
	protected virtual void ListVersionList(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListVersionList not implemented");
	protected virtual void RequestVersionListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVersionListData not implemented");
	protected virtual void GetApplicationRecord(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecord not implemented");
	protected virtual void GetApplicationRecordProperty(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecordProperty not implemented");
	protected virtual void EnableApplicationAutoUpdate(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationAutoUpdate");
	protected virtual void DisableApplicationAutoUpdate(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DisableApplicationAutoUpdate");
	protected virtual void TouchApplication(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TouchApplication");
	protected virtual void RequestApplicationUpdate(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestApplicationUpdate");
	protected virtual void IsApplicationUpdateRequested(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationUpdateRequested not implemented");
	protected virtual void WithdrawApplicationUpdateRequest(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.WithdrawApplicationUpdateRequest");
	protected virtual void ListApplicationRecordInstalledContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecordInstalledContentMeta not implemented");
	protected virtual void WithdrawCleanupAddOnContentsWithNoRightsRecommendation(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.WithdrawCleanupAddOnContentsWithNoRightsRecommendation");
	protected virtual void Unknown910() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown910");
	protected virtual void Unknown911() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown911");
	protected virtual void Unknown912() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown912");
	protected virtual void RequestVerifyApplicationDeprecated(byte[] _0, KObject _1, out KObject _2, out Nn.Ns.Detail.IProgressAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyApplicationDeprecated not implemented");
	protected virtual void CorruptApplicationForDebug(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CorruptApplicationForDebug");
	protected virtual void RequestVerifyAddOnContentsRights(byte[] _0, out KObject _1, out Nn.Ns.Detail.IProgressAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyAddOnContentsRights not implemented");
	protected virtual void RequestVerifyApplication() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyApplication");
	protected virtual void CorruptContentForDebug() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CorruptContentForDebug");
	protected virtual void NeedsUpdateVulnerability(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsUpdateVulnerability not implemented");
	protected virtual void IsAnyApplicationEntityInstalled(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsAnyApplicationEntityInstalled not implemented");
	protected virtual void DeleteApplicationContentEntities(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationContentEntities");
	protected virtual void CleanupUnrecordedApplicationEntity(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupUnrecordedApplicationEntity");
	protected virtual void CleanupAddOnContentsWithNoRights(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupAddOnContentsWithNoRights");
	protected virtual void DeleteApplicationContentEntity(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationContentEntity");
	protected virtual void Unknown1308() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown1308");
	protected virtual void Unknown1309() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown1309");
	protected virtual void PrepareShutdown() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PrepareShutdown");
	protected virtual void FormatSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.FormatSdCard");
	protected virtual void NeedsSystemUpdateToFormatSdCard(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsSystemUpdateToFormatSdCard not implemented");
	protected virtual void GetLastSdCardFormatUnexpectedResult() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastSdCardFormatUnexpectedResult");
	protected virtual void InsertSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InsertSdCard");
	protected virtual void RemoveSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RemoveSdCard");
	protected virtual void GetSystemSeedForPseudoDeviceId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSystemSeedForPseudoDeviceId not implemented");
	protected virtual void ResetSystemSeedForPseudoDeviceId() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetSystemSeedForPseudoDeviceId");
	protected virtual void ListApplicationDownloadingContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationDownloadingContentMeta not implemented");
	protected virtual void GetApplicationView(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationView not implemented");
	protected virtual void GetApplicationDownloadTaskStatus(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDownloadTaskStatus not implemented");
	protected virtual void GetApplicationViewDownloadErrorContext(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationViewDownloadErrorContext not implemented");
	protected virtual void IsNotificationSetupCompleted(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsNotificationSetupCompleted not implemented");
	protected virtual void GetLastNotificationInfoCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetLastNotificationInfoCount not implemented");
	protected virtual void ListLastNotificationInfo(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListLastNotificationInfo not implemented");
	protected virtual void ListNotificationTask(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListNotificationTask not implemented");
	protected virtual void IsActiveAccount(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsActiveAccount not implemented");
	protected virtual void RequestDownloadApplicationPrepurchasedRights(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplicationPrepurchasedRights not implemented");
	protected virtual void GetApplicationTicketInfo() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationTicketInfo");
	protected virtual void GetSystemDeliveryInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSystemDeliveryInfo not implemented");
	protected virtual void SelectLatestSystemDeliveryInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.SelectLatestSystemDeliveryInfo not implemented");
	protected virtual void VerifyDeliveryProtocolVersion(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.VerifyDeliveryProtocolVersion");
	protected virtual void GetApplicationDeliveryInfo(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDeliveryInfo not implemented");
	protected virtual void HasAllContentsToDeliver(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.HasAllContentsToDeliver not implemented");
	protected virtual void CompareApplicationDeliveryInfo(Span<byte> _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CompareApplicationDeliveryInfo not implemented");
	protected virtual void CanDeliverApplication(Span<byte> _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CanDeliverApplication not implemented");
	protected virtual void ListContentMetaKeyToDeliverApplication(byte[] _0, Span<byte> _1, out byte[] _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListContentMetaKeyToDeliverApplication not implemented");
	protected virtual void NeedsSystemUpdateToDeliverApplication(Span<byte> _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsSystemUpdateToDeliverApplication not implemented");
	protected virtual void EstimateRequiredSize(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.EstimateRequiredSize not implemented");
	protected virtual void RequestReceiveApplication(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestReceiveApplication not implemented");
	protected virtual void CommitReceiveApplication(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CommitReceiveApplication");
	protected virtual void GetReceiveApplicationProgress(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetReceiveApplicationProgress not implemented");
	protected virtual void RequestSendApplication(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestSendApplication not implemented");
	protected virtual void GetSendApplicationProgress(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSendApplicationProgress not implemented");
	protected virtual void CompareSystemDeliveryInfo(Span<byte> _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CompareSystemDeliveryInfo not implemented");
	protected virtual void ListNotCommittedContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListNotCommittedContentMeta not implemented");
	protected virtual void CreateDownloadTask(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CreateDownloadTask");
	protected virtual void Unknown2018() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2018");
	protected virtual void Unknown2050() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2050");
	protected virtual void Unknown2100() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2100");
	protected virtual void Unknown2101() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2101");
	protected virtual void Unknown2150() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2150");
	protected virtual void Unknown2151() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2151");
	protected virtual void Unknown2152() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2152");
	protected virtual void Unknown2153() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2153");
	protected virtual void Unknown2154() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2154");
	protected virtual void Unknown2160() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2160");
	protected virtual void Unknown2161() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2161");
	protected virtual void Unknown2170() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2170");
	protected virtual void Unknown2171() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2171");
	protected virtual void Unknown2180() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2180");
	protected virtual void Unknown2181() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2181");
	protected virtual void Unknown2182() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2182");
	protected virtual void Unknown2190() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2190");
	protected virtual void Unknown2199() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2199");
	protected virtual void Unknown2200() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2200");
	protected virtual void Unknown2201() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2201");
	protected virtual void Unknown2250() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2250");
	protected virtual void Unknown2300() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2300");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListApplicationRecord
				om.Initialize(0, 0, 4);
				ListApplicationRecord(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GenerateApplicationRecordCount
				om.Initialize(0, 0, 8);
				GenerateApplicationRecordCount(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // GetApplicationRecordUpdateSystemEvent
				om.Initialize(0, 1, 0);
				var _return = GetApplicationRecordUpdateSystemEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // GetApplicationViewDeprecated
				om.Initialize(0, 0, 0);
				GetApplicationViewDeprecated(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x4: { // DeleteApplicationEntity
				om.Initialize(0, 0, 0);
				DeleteApplicationEntity(im.GetBytes(8, 0x8));
				break;
			}
			case 0x5: { // DeleteApplicationCompletely
				om.Initialize(0, 0, 0);
				DeleteApplicationCompletely(im.GetBytes(8, 0x8));
				break;
			}
			case 0x6: { // IsAnyApplicationEntityRedundant
				om.Initialize(0, 0, 1);
				IsAnyApplicationEntityRedundant(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // DeleteRedundantApplicationEntity
				om.Initialize(0, 0, 0);
				DeleteRedundantApplicationEntity();
				break;
			}
			case 0x8: { // IsApplicationEntityMovable
				om.Initialize(0, 0, 1);
				IsApplicationEntityMovable(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // MoveApplicationEntity
				om.Initialize(0, 0, 0);
				MoveApplicationEntity(im.GetBytes(8, 0x10));
				break;
			}
			case 0xB: { // CalculateApplicationOccupiedSize
				om.Initialize(0, 0, 128);
				CalculateApplicationOccupiedSize(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // PushApplicationRecord
				om.Initialize(0, 0, 0);
				PushApplicationRecord(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x11: { // ListApplicationRecordContentMeta
				om.Initialize(0, 0, 4);
				ListApplicationRecordContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // LaunchApplication
				om.Initialize(0, 0, 8);
				LaunchApplication(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // GetApplicationContentPath
				om.Initialize(0, 0, 0);
				GetApplicationContentPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x16: { // TerminateApplication
				om.Initialize(0, 0, 0);
				TerminateApplication(im.GetBytes(8, 0x8));
				break;
			}
			case 0x17: { // ResolveApplicationContentPath
				om.Initialize(0, 0, 0);
				ResolveApplicationContentPath(im.GetBytes(8, 0x10));
				break;
			}
			case 0x1A: { // BeginInstallApplication
				om.Initialize(0, 0, 0);
				BeginInstallApplication(im.GetBytes(8, 0x10));
				break;
			}
			case 0x1B: { // DeleteApplicationRecord
				om.Initialize(0, 0, 0);
				DeleteApplicationRecord(im.GetBytes(8, 0x8));
				break;
			}
			case 0x1E: { // RequestApplicationUpdateInfo
				om.Initialize(1, 1, 0);
				RequestApplicationUpdateInfo(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x20: { // CancelApplicationDownload
				om.Initialize(0, 0, 0);
				CancelApplicationDownload(im.GetBytes(8, 0x8));
				break;
			}
			case 0x21: { // ResumeApplicationDownload
				om.Initialize(0, 0, 0);
				ResumeApplicationDownload(im.GetBytes(8, 0x8));
				break;
			}
			case 0x23: { // UpdateVersionList
				om.Initialize(0, 0, 0);
				UpdateVersionList(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x24: { // PushLaunchVersion
				om.Initialize(0, 0, 0);
				PushLaunchVersion(im.GetBytes(8, 0x10));
				break;
			}
			case 0x25: { // ListRequiredVersion
				om.Initialize(0, 0, 4);
				ListRequiredVersion(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x26: { // CheckApplicationLaunchVersion
				om.Initialize(0, 0, 0);
				CheckApplicationLaunchVersion(im.GetBytes(8, 0x8));
				break;
			}
			case 0x27: { // CheckApplicationLaunchRights
				om.Initialize(0, 0, 0);
				CheckApplicationLaunchRights(im.GetBytes(8, 0x8));
				break;
			}
			case 0x28: { // GetApplicationLogoData
				om.Initialize(0, 0, 8);
				GetApplicationLogoData(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x29: { // CalculateApplicationDownloadRequiredSize
				om.Initialize(0, 0, 16);
				CalculateApplicationDownloadRequiredSize(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2A: { // CleanupSdCard
				om.Initialize(0, 0, 0);
				CleanupSdCard();
				break;
			}
			case 0x2B: { // CheckSdCardMountStatus
				om.Initialize(0, 0, 0);
				CheckSdCardMountStatus();
				break;
			}
			case 0x2C: { // GetSdCardMountStatusChangedEvent
				om.Initialize(0, 1, 0);
				var _return = GetSdCardMountStatusChangedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2D: { // GetGameCardAttachmentEvent
				om.Initialize(0, 1, 0);
				var _return = GetGameCardAttachmentEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2E: { // GetGameCardAttachmentInfo
				om.Initialize(0, 0, 16);
				GetGameCardAttachmentInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2F: { // GetTotalSpaceSize
				om.Initialize(0, 0, 8);
				GetTotalSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x30: { // GetFreeSpaceSize
				om.Initialize(0, 0, 8);
				GetFreeSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x31: { // GetSdCardRemovedEvent
				om.Initialize(0, 1, 0);
				var _return = GetSdCardRemovedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x34: { // GetGameCardUpdateDetectionEvent
				om.Initialize(0, 1, 0);
				var _return = GetGameCardUpdateDetectionEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x35: { // DisableApplicationAutoDelete
				om.Initialize(0, 0, 0);
				DisableApplicationAutoDelete(im.GetBytes(8, 0x8));
				break;
			}
			case 0x36: { // EnableApplicationAutoDelete
				om.Initialize(0, 0, 0);
				EnableApplicationAutoDelete(im.GetBytes(8, 0x8));
				break;
			}
			case 0x37: { // GetApplicationDesiredLanguage
				om.Initialize(0, 0, 1);
				GetApplicationDesiredLanguage(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x38: { // SetApplicationTerminateResult
				om.Initialize(0, 0, 0);
				SetApplicationTerminateResult(im.GetBytes(8, 0x10));
				break;
			}
			case 0x39: { // ClearApplicationTerminateResult
				om.Initialize(0, 0, 0);
				ClearApplicationTerminateResult(im.GetBytes(8, 0x8));
				break;
			}
			case 0x3A: { // GetLastSdCardMountUnexpectedResult
				om.Initialize(0, 0, 0);
				GetLastSdCardMountUnexpectedResult();
				break;
			}
			case 0x3B: { // ConvertApplicationLanguageToLanguageCode
				om.Initialize(0, 0, 8);
				ConvertApplicationLanguageToLanguageCode(im.GetBytes(8, 0x1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ConvertLanguageCodeToApplicationLanguage
				om.Initialize(0, 0, 1);
				ConvertLanguageCodeToApplicationLanguage(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3D: { // GetBackgroundDownloadStressTaskInfo
				om.Initialize(0, 0, 16);
				GetBackgroundDownloadStressTaskInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3E: { // GetGameCardStopper
				om.Initialize(1, 0, 0);
				var _return = GetGameCardStopper();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3F: { // IsSystemProgramInstalled
				om.Initialize(0, 0, 1);
				IsSystemProgramInstalled(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x40: { // StartApplyDeltaTask
				om.Initialize(0, 0, 0);
				StartApplyDeltaTask(im.GetBytes(8, 0x8));
				break;
			}
			case 0x41: { // GetRequestServerStopper
				om.Initialize(1, 0, 0);
				var _return = GetRequestServerStopper();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x42: { // GetBackgroundApplyDeltaStressTaskInfo
				om.Initialize(0, 0, 16);
				GetBackgroundApplyDeltaStressTaskInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x43: { // CancelApplicationApplyDelta
				om.Initialize(0, 0, 0);
				CancelApplicationApplyDelta(im.GetBytes(8, 0x8));
				break;
			}
			case 0x44: { // ResumeApplicationApplyDelta
				om.Initialize(0, 0, 0);
				ResumeApplicationApplyDelta(im.GetBytes(8, 0x8));
				break;
			}
			case 0x45: { // CalculateApplicationApplyDeltaRequiredSize
				om.Initialize(0, 0, 16);
				CalculateApplicationApplyDeltaRequiredSize(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x46: { // ResumeAll
				om.Initialize(0, 0, 0);
				ResumeAll();
				break;
			}
			case 0x47: { // GetStorageSize
				om.Initialize(0, 0, 16);
				GetStorageSize(im.GetBytes(8, 0x1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x50: { // RequestDownloadApplication
				om.Initialize(1, 1, 0);
				RequestDownloadApplication(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x51: { // RequestDownloadAddOnContent
				om.Initialize(1, 1, 0);
				RequestDownloadAddOnContent(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x52: { // DownloadApplication
				om.Initialize(0, 0, 0);
				DownloadApplication(im.GetBytes(8, 0x10));
				break;
			}
			case 0x53: { // CheckApplicationResumeRights
				om.Initialize(0, 0, 0);
				CheckApplicationResumeRights(im.GetBytes(8, 0x8));
				break;
			}
			case 0x54: { // GetDynamicCommitEvent
				om.Initialize(0, 1, 0);
				var _return = GetDynamicCommitEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x55: { // RequestUpdateApplication2
				om.Initialize(1, 1, 0);
				RequestUpdateApplication2(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x56: { // EnableApplicationCrashReport
				om.Initialize(0, 0, 0);
				EnableApplicationCrashReport(im.GetBytes(8, 0x1));
				break;
			}
			case 0x57: { // IsApplicationCrashReportEnabled
				om.Initialize(0, 0, 1);
				IsApplicationCrashReportEnabled(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5A: { // BoostSystemMemoryResourceLimit
				om.Initialize(0, 0, 0);
				BoostSystemMemoryResourceLimit(im.GetBytes(8, 0x8));
				break;
			}
			case 0x5B: { // Unknown91
				om.Initialize(0, 0, 0);
				Unknown91();
				break;
			}
			case 0x5C: { // Unknown92
				om.Initialize(0, 0, 0);
				Unknown92();
				break;
			}
			case 0x5D: { // Unknown93
				om.Initialize(0, 0, 0);
				Unknown93();
				break;
			}
			case 0x5E: { // LaunchApplication2
				om.Initialize(0, 0, 0);
				LaunchApplication2();
				break;
			}
			case 0x5F: { // Unknown95
				om.Initialize(0, 0, 0);
				Unknown95();
				break;
			}
			case 0x60: { // Unknown96
				om.Initialize(0, 0, 0);
				Unknown96();
				break;
			}
			case 0x61: { // Unknown97
				om.Initialize(0, 0, 0);
				Unknown97();
				break;
			}
			case 0x62: { // Unknown98
				om.Initialize(0, 0, 0);
				Unknown98();
				break;
			}
			case 0x64: { // ResetToFactorySettings
				om.Initialize(0, 0, 0);
				ResetToFactorySettings();
				break;
			}
			case 0x65: { // ResetToFactorySettingsWithoutUserSaveData
				om.Initialize(0, 0, 0);
				ResetToFactorySettingsWithoutUserSaveData();
				break;
			}
			case 0x66: { // ResetToFactorySettingsForRefurbishment
				om.Initialize(0, 0, 0);
				ResetToFactorySettingsForRefurbishment();
				break;
			}
			case 0xC8: { // CalculateUserSaveDataStatistics
				om.Initialize(0, 0, 16);
				CalculateUserSaveDataStatistics(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC9: { // DeleteUserSaveDataAll
				om.Initialize(1, 0, 0);
				var _return = DeleteUserSaveDataAll(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xD2: { // DeleteUserSystemSaveData
				om.Initialize(0, 0, 0);
				DeleteUserSystemSaveData(im.GetBytes(8, 0x18));
				break;
			}
			case 0xD3: { // Unknown211
				om.Initialize(0, 0, 0);
				Unknown211();
				break;
			}
			case 0xDC: { // UnregisterNetworkServiceAccount
				om.Initialize(0, 0, 0);
				UnregisterNetworkServiceAccount(im.GetBytes(8, 0x10));
				break;
			}
			case 0xDD: { // Unknown221
				om.Initialize(0, 0, 0);
				Unknown221();
				break;
			}
			case 0x12C: { // GetApplicationShellEvent
				om.Initialize(0, 1, 0);
				var _return = GetApplicationShellEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12D: { // PopApplicationShellEventInfo
				om.Initialize(0, 0, 4);
				PopApplicationShellEventInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x12E: { // LaunchLibraryApplet
				om.Initialize(0, 0, 8);
				LaunchLibraryApplet(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12F: { // TerminateLibraryApplet
				om.Initialize(0, 0, 0);
				TerminateLibraryApplet(im.GetBytes(8, 0x8));
				break;
			}
			case 0x130: { // LaunchSystemApplet
				om.Initialize(0, 0, 8);
				LaunchSystemApplet(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x131: { // TerminateSystemApplet
				om.Initialize(0, 0, 0);
				TerminateSystemApplet(im.GetBytes(8, 0x8));
				break;
			}
			case 0x132: { // LaunchOverlayApplet
				om.Initialize(0, 0, 8);
				LaunchOverlayApplet(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x133: { // TerminateOverlayApplet
				om.Initialize(0, 0, 0);
				TerminateOverlayApplet(im.GetBytes(8, 0x8));
				break;
			}
			case 0x190: { // GetApplicationControlData
				om.Initialize(0, 0, 4);
				GetApplicationControlData(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x191: { // InvalidateAllApplicationControlCache
				om.Initialize(0, 0, 0);
				InvalidateAllApplicationControlCache();
				break;
			}
			case 0x192: { // RequestDownloadApplicationControlData
				om.Initialize(1, 1, 0);
				RequestDownloadApplicationControlData(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x193: { // GetMaxApplicationControlCacheCount
				om.Initialize(0, 0, 4);
				GetMaxApplicationControlCacheCount(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x194: { // InvalidateApplicationControlCache
				om.Initialize(0, 0, 0);
				InvalidateApplicationControlCache(im.GetBytes(8, 0x8));
				break;
			}
			case 0x195: { // ListApplicationControlCacheEntryInfo
				om.Initialize(0, 0, 4);
				ListApplicationControlCacheEntryInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x196: { // Unknown406
				om.Initialize(0, 0, 0);
				Unknown406();
				break;
			}
			case 0x1F6: { // RequestCheckGameCardRegistration
				om.Initialize(1, 1, 0);
				RequestCheckGameCardRegistration(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F7: { // RequestGameCardRegistrationGoldPoint
				om.Initialize(1, 1, 0);
				RequestGameCardRegistrationGoldPoint(im.GetBytes(8, 0x18), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F8: { // RequestRegisterGameCard
				om.Initialize(1, 1, 0);
				RequestRegisterGameCard(im.GetBytes(8, 0x20), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F9: { // GetGameCardMountFailureEvent
				om.Initialize(0, 1, 0);
				var _return = GetGameCardMountFailureEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1FA: { // IsGameCardInserted
				om.Initialize(0, 0, 1);
				IsGameCardInserted(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1FB: { // EnsureGameCardAccess
				om.Initialize(0, 0, 0);
				EnsureGameCardAccess();
				break;
			}
			case 0x1FC: { // GetLastGameCardMountFailureResult
				om.Initialize(0, 0, 0);
				GetLastGameCardMountFailureResult();
				break;
			}
			case 0x1FD: { // ListApplicationIdOnGameCard
				om.Initialize(0, 0, 0);
				ListApplicationIdOnGameCard();
				break;
			}
			case 0x258: { // CountApplicationContentMeta
				om.Initialize(0, 0, 4);
				CountApplicationContentMeta(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x259: { // ListApplicationContentMetaStatus
				om.Initialize(0, 0, 4);
				ListApplicationContentMetaStatus(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x25A: { // ListAvailableAddOnContent
				om.Initialize(0, 0, 4);
				ListAvailableAddOnContent(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x25B: { // GetOwnedApplicationContentMetaStatus
				om.Initialize(0, 0, 16);
				GetOwnedApplicationContentMetaStatus(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25C: { // RegisterContentsExternalKey
				om.Initialize(0, 0, 0);
				RegisterContentsExternalKey(im.GetBytes(8, 0x10));
				break;
			}
			case 0x25D: { // ListApplicationContentMetaStatusWithRightsCheck
				om.Initialize(0, 0, 4);
				ListApplicationContentMetaStatusWithRightsCheck(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x25E: { // GetContentMetaStorage
				om.Initialize(0, 0, 1);
				GetContentMetaStorage(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25F: { // Unknown607
				om.Initialize(0, 0, 0);
				Unknown607();
				break;
			}
			case 0x2BC: { // PushDownloadTaskList
				om.Initialize(0, 0, 0);
				PushDownloadTaskList(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x2BD: { // ClearTaskStatusList
				om.Initialize(0, 0, 0);
				ClearTaskStatusList();
				break;
			}
			case 0x2BE: { // RequestDownloadTaskList
				om.Initialize(0, 0, 0);
				RequestDownloadTaskList();
				break;
			}
			case 0x2BF: { // RequestEnsureDownloadTask
				om.Initialize(1, 1, 0);
				RequestEnsureDownloadTask(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2C0: { // ListDownloadTaskStatus
				om.Initialize(0, 0, 4);
				ListDownloadTaskStatus(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C1: { // RequestDownloadTaskListData
				om.Initialize(1, 1, 0);
				RequestDownloadTaskListData(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x320: { // RequestVersionList
				om.Initialize(0, 0, 0);
				RequestVersionList();
				break;
			}
			case 0x321: { // ListVersionList
				om.Initialize(0, 0, 4);
				ListVersionList(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x322: { // RequestVersionListData
				om.Initialize(1, 1, 0);
				RequestVersionListData(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x384: { // GetApplicationRecord
				om.Initialize(0, 0, 24);
				GetApplicationRecord(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x385: { // GetApplicationRecordProperty
				om.Initialize(0, 0, 0);
				GetApplicationRecordProperty(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x386: { // EnableApplicationAutoUpdate
				om.Initialize(0, 0, 0);
				EnableApplicationAutoUpdate(im.GetBytes(8, 0x8));
				break;
			}
			case 0x387: { // DisableApplicationAutoUpdate
				om.Initialize(0, 0, 0);
				DisableApplicationAutoUpdate(im.GetBytes(8, 0x8));
				break;
			}
			case 0x388: { // TouchApplication
				om.Initialize(0, 0, 0);
				TouchApplication(im.GetBytes(8, 0x8));
				break;
			}
			case 0x389: { // RequestApplicationUpdate
				om.Initialize(0, 0, 0);
				RequestApplicationUpdate(im.GetBytes(8, 0x10));
				break;
			}
			case 0x38A: { // IsApplicationUpdateRequested
				om.Initialize(0, 0, 8);
				IsApplicationUpdateRequested(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x38B: { // WithdrawApplicationUpdateRequest
				om.Initialize(0, 0, 0);
				WithdrawApplicationUpdateRequest(im.GetBytes(8, 0x8));
				break;
			}
			case 0x38C: { // ListApplicationRecordInstalledContentMeta
				om.Initialize(0, 0, 4);
				ListApplicationRecordInstalledContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x38D: { // WithdrawCleanupAddOnContentsWithNoRightsRecommendation
				om.Initialize(0, 0, 0);
				WithdrawCleanupAddOnContentsWithNoRightsRecommendation(im.GetBytes(8, 0x8));
				break;
			}
			case 0x38E: { // Unknown910
				om.Initialize(0, 0, 0);
				Unknown910();
				break;
			}
			case 0x38F: { // Unknown911
				om.Initialize(0, 0, 0);
				Unknown911();
				break;
			}
			case 0x390: { // Unknown912
				om.Initialize(0, 0, 0);
				Unknown912();
				break;
			}
			case 0x3E8: { // RequestVerifyApplicationDeprecated
				om.Initialize(1, 1, 0);
				RequestVerifyApplicationDeprecated(im.GetBytes(8, 0x10), Kernel.Get<KObject>(im.GetCopy(0)), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3E9: { // CorruptApplicationForDebug
				om.Initialize(0, 0, 0);
				CorruptApplicationForDebug(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3EA: { // RequestVerifyAddOnContentsRights
				om.Initialize(1, 1, 0);
				RequestVerifyAddOnContentsRights(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3EB: { // RequestVerifyApplication
				om.Initialize(0, 0, 0);
				RequestVerifyApplication();
				break;
			}
			case 0x3EC: { // CorruptContentForDebug
				om.Initialize(0, 0, 0);
				CorruptContentForDebug();
				break;
			}
			case 0x4B0: { // NeedsUpdateVulnerability
				om.Initialize(0, 0, 1);
				NeedsUpdateVulnerability(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x514: { // IsAnyApplicationEntityInstalled
				om.Initialize(0, 0, 1);
				IsAnyApplicationEntityInstalled(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x515: { // DeleteApplicationContentEntities
				om.Initialize(0, 0, 0);
				DeleteApplicationContentEntities(im.GetBytes(8, 0x10));
				break;
			}
			case 0x516: { // CleanupUnrecordedApplicationEntity
				om.Initialize(0, 0, 0);
				CleanupUnrecordedApplicationEntity(im.GetBytes(8, 0x8));
				break;
			}
			case 0x517: { // CleanupAddOnContentsWithNoRights
				om.Initialize(0, 0, 0);
				CleanupAddOnContentsWithNoRights(im.GetBytes(8, 0x8));
				break;
			}
			case 0x518: { // DeleteApplicationContentEntity
				om.Initialize(0, 0, 0);
				DeleteApplicationContentEntity(im.GetBytes(8, 0x10));
				break;
			}
			case 0x51C: { // Unknown1308
				om.Initialize(0, 0, 0);
				Unknown1308();
				break;
			}
			case 0x51D: { // Unknown1309
				om.Initialize(0, 0, 0);
				Unknown1309();
				break;
			}
			case 0x578: { // PrepareShutdown
				om.Initialize(0, 0, 0);
				PrepareShutdown();
				break;
			}
			case 0x5DC: { // FormatSdCard
				om.Initialize(0, 0, 0);
				FormatSdCard();
				break;
			}
			case 0x5DD: { // NeedsSystemUpdateToFormatSdCard
				om.Initialize(0, 0, 1);
				NeedsSystemUpdateToFormatSdCard(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5DE: { // GetLastSdCardFormatUnexpectedResult
				om.Initialize(0, 0, 0);
				GetLastSdCardFormatUnexpectedResult();
				break;
			}
			case 0x5E0: { // InsertSdCard
				om.Initialize(0, 0, 0);
				InsertSdCard();
				break;
			}
			case 0x5E1: { // RemoveSdCard
				om.Initialize(0, 0, 0);
				RemoveSdCard();
				break;
			}
			case 0x640: { // GetSystemSeedForPseudoDeviceId
				om.Initialize(0, 0, 32);
				GetSystemSeedForPseudoDeviceId(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x641: { // ResetSystemSeedForPseudoDeviceId
				om.Initialize(0, 0, 0);
				ResetSystemSeedForPseudoDeviceId();
				break;
			}
			case 0x6A4: { // ListApplicationDownloadingContentMeta
				om.Initialize(0, 0, 4);
				ListApplicationDownloadingContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x6A5: { // GetApplicationView
				om.Initialize(0, 0, 0);
				GetApplicationView(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x6A6: { // GetApplicationDownloadTaskStatus
				om.Initialize(0, 0, 1);
				GetApplicationDownloadTaskStatus(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6A7: { // GetApplicationViewDownloadErrorContext
				om.Initialize(0, 0, 0);
				GetApplicationViewDownloadErrorContext(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x708: { // IsNotificationSetupCompleted
				om.Initialize(0, 0, 1);
				IsNotificationSetupCompleted(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x709: { // GetLastNotificationInfoCount
				om.Initialize(0, 0, 8);
				GetLastNotificationInfoCount(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x70A: { // ListLastNotificationInfo
				om.Initialize(0, 0, 4);
				ListLastNotificationInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x70B: { // ListNotificationTask
				om.Initialize(0, 0, 4);
				ListNotificationTask(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x76C: { // IsActiveAccount
				om.Initialize(0, 0, 1);
				IsActiveAccount(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x76D: { // RequestDownloadApplicationPrepurchasedRights
				om.Initialize(1, 1, 0);
				RequestDownloadApplicationPrepurchasedRights(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x76E: { // GetApplicationTicketInfo
				om.Initialize(0, 0, 0);
				GetApplicationTicketInfo();
				break;
			}
			case 0x7D0: { // GetSystemDeliveryInfo
				om.Initialize(0, 0, 0);
				GetSystemDeliveryInfo(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x7D1: { // SelectLatestSystemDeliveryInfo
				om.Initialize(0, 0, 4);
				SelectLatestSystemDeliveryInfo(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D2: { // VerifyDeliveryProtocolVersion
				om.Initialize(0, 0, 0);
				VerifyDeliveryProtocolVersion(im.GetSpan<byte>(0x15, 0));
				break;
			}
			case 0x7D3: { // GetApplicationDeliveryInfo
				om.Initialize(0, 0, 4);
				GetApplicationDeliveryInfo(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D4: { // HasAllContentsToDeliver
				om.Initialize(0, 0, 1);
				HasAllContentsToDeliver(im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D5: { // CompareApplicationDeliveryInfo
				om.Initialize(0, 0, 4);
				CompareApplicationDeliveryInfo(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D6: { // CanDeliverApplication
				om.Initialize(0, 0, 1);
				CanDeliverApplication(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D7: { // ListContentMetaKeyToDeliverApplication
				om.Initialize(0, 0, 4);
				ListContentMetaKeyToDeliverApplication(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D8: { // NeedsSystemUpdateToDeliverApplication
				om.Initialize(0, 0, 1);
				NeedsSystemUpdateToDeliverApplication(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D9: { // EstimateRequiredSize
				om.Initialize(0, 0, 8);
				EstimateRequiredSize(im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DA: { // RequestReceiveApplication
				om.Initialize(1, 1, 0);
				RequestReceiveApplication(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x7DB: { // CommitReceiveApplication
				om.Initialize(0, 0, 0);
				CommitReceiveApplication(im.GetBytes(8, 0x8));
				break;
			}
			case 0x7DC: { // GetReceiveApplicationProgress
				om.Initialize(0, 0, 16);
				GetReceiveApplicationProgress(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DD: { // RequestSendApplication
				om.Initialize(1, 1, 0);
				RequestSendApplication(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x7DE: { // GetSendApplicationProgress
				om.Initialize(0, 0, 16);
				GetSendApplicationProgress(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DF: { // CompareSystemDeliveryInfo
				om.Initialize(0, 0, 4);
				CompareSystemDeliveryInfo(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x15, 1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7E0: { // ListNotCommittedContentMeta
				om.Initialize(0, 0, 4);
				ListNotCommittedContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x7E1: { // CreateDownloadTask
				om.Initialize(0, 0, 0);
				CreateDownloadTask(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x7E2: { // Unknown2018
				om.Initialize(0, 0, 0);
				Unknown2018();
				break;
			}
			case 0x802: { // Unknown2050
				om.Initialize(0, 0, 0);
				Unknown2050();
				break;
			}
			case 0x834: { // Unknown2100
				om.Initialize(0, 0, 0);
				Unknown2100();
				break;
			}
			case 0x835: { // Unknown2101
				om.Initialize(0, 0, 0);
				Unknown2101();
				break;
			}
			case 0x866: { // Unknown2150
				om.Initialize(0, 0, 0);
				Unknown2150();
				break;
			}
			case 0x867: { // Unknown2151
				om.Initialize(0, 0, 0);
				Unknown2151();
				break;
			}
			case 0x868: { // Unknown2152
				om.Initialize(0, 0, 0);
				Unknown2152();
				break;
			}
			case 0x869: { // Unknown2153
				om.Initialize(0, 0, 0);
				Unknown2153();
				break;
			}
			case 0x86A: { // Unknown2154
				om.Initialize(0, 0, 0);
				Unknown2154();
				break;
			}
			case 0x870: { // Unknown2160
				om.Initialize(0, 0, 0);
				Unknown2160();
				break;
			}
			case 0x871: { // Unknown2161
				om.Initialize(0, 0, 0);
				Unknown2161();
				break;
			}
			case 0x87A: { // Unknown2170
				om.Initialize(0, 0, 0);
				Unknown2170();
				break;
			}
			case 0x87B: { // Unknown2171
				om.Initialize(0, 0, 0);
				Unknown2171();
				break;
			}
			case 0x884: { // Unknown2180
				om.Initialize(0, 0, 0);
				Unknown2180();
				break;
			}
			case 0x885: { // Unknown2181
				om.Initialize(0, 0, 0);
				Unknown2181();
				break;
			}
			case 0x886: { // Unknown2182
				om.Initialize(0, 0, 0);
				Unknown2182();
				break;
			}
			case 0x88E: { // Unknown2190
				om.Initialize(0, 0, 0);
				Unknown2190();
				break;
			}
			case 0x897: { // Unknown2199
				om.Initialize(0, 0, 0);
				Unknown2199();
				break;
			}
			case 0x898: { // Unknown2200
				om.Initialize(0, 0, 0);
				Unknown2200();
				break;
			}
			case 0x899: { // Unknown2201
				om.Initialize(0, 0, 0);
				Unknown2201();
				break;
			}
			case 0x8CA: { // Unknown2250
				om.Initialize(0, 0, 0);
				Unknown2250();
				break;
			}
			case 0x8FC: { // Unknown2300
				om.Initialize(0, 0, 0);
				Unknown2300();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IApplicationManagerInterface");
		}
	}
}

public partial class IApplicationVersionInterface : _IApplicationVersionInterface_Base;
public abstract class _IApplicationVersionInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown1");
	protected virtual void Unknown35(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown35");
	protected virtual void Unknown36(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown36");
	protected virtual void Unknown37(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown37 not implemented");
	protected virtual void Unknown800() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown800");
	protected virtual void Unknown801(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown801 not implemented");
	protected virtual void Unknown802(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown802 not implemented");
	protected virtual void Unknown1000() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown1000");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetBytes(8, 0x10));
				break;
			}
			case 0x23: { // Unknown35
				om.Initialize(0, 0, 0);
				Unknown35(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x24: { // Unknown36
				om.Initialize(0, 0, 0);
				Unknown36(im.GetBytes(8, 0x10));
				break;
			}
			case 0x25: { // Unknown37
				om.Initialize(0, 0, 4);
				Unknown37(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x320: { // Unknown800
				om.Initialize(0, 0, 0);
				Unknown800();
				break;
			}
			case 0x321: { // Unknown801
				om.Initialize(0, 0, 4);
				Unknown801(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x322: { // Unknown802
				om.Initialize(1, 1, 0);
				Unknown802(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3E8: { // Unknown1000
				om.Initialize(0, 0, 0);
				Unknown1000();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IApplicationVersionInterface");
		}
	}
}

public partial class IAsyncResult : _IAsyncResult_Base;
public abstract class _IAsyncResult_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IAsyncResult.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IAsyncResult.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncResult.Unknown2 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0();
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAsyncResult");
		}
	}
}

public partial class IAsyncValue : _IAsyncValue_Base;
public abstract class _IAsyncValue_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IAsyncValue.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 8);
				Unknown0(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAsyncValue");
		}
	}
}

public partial class IContentManagementInterface : _IContentManagementInterface_Base;
public abstract class _IContentManagementInterface_Base : IpcInterface {
	protected virtual void CalculateApplicationOccupiedSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.CalculateApplicationOccupiedSize not implemented");
	protected virtual void CheckSdCardMountStatus() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IContentManagementInterface.CheckSdCardMountStatus");
	protected virtual void GetTotalSpaceSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.GetTotalSpaceSize not implemented");
	protected virtual void GetFreeSpaceSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.GetFreeSpaceSize not implemented");
	protected virtual void CountApplicationContentMeta(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.CountApplicationContentMeta not implemented");
	protected virtual void ListApplicationContentMetaStatus(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.ListApplicationContentMetaStatus not implemented");
	protected virtual void ListApplicationContentMetaStatusWithRightsCheck(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.ListApplicationContentMetaStatusWithRightsCheck not implemented");
	protected virtual void IsAnyApplicationRunning(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.IsAnyApplicationRunning not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xB: { // CalculateApplicationOccupiedSize
				om.Initialize(0, 0, 128);
				CalculateApplicationOccupiedSize(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2B: { // CheckSdCardMountStatus
				om.Initialize(0, 0, 0);
				CheckSdCardMountStatus();
				break;
			}
			case 0x2F: { // GetTotalSpaceSize
				om.Initialize(0, 0, 8);
				GetTotalSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x30: { // GetFreeSpaceSize
				om.Initialize(0, 0, 8);
				GetFreeSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x258: { // CountApplicationContentMeta
				om.Initialize(0, 0, 4);
				CountApplicationContentMeta(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x259: { // ListApplicationContentMetaStatus
				om.Initialize(0, 0, 4);
				ListApplicationContentMetaStatus(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x25D: { // ListApplicationContentMetaStatusWithRightsCheck
				om.Initialize(0, 0, 4);
				ListApplicationContentMetaStatusWithRightsCheck(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x25F: { // IsAnyApplicationRunning
				om.Initialize(0, 0, 1);
				IsAnyApplicationRunning(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IContentManagementInterface");
		}
	}
}

public partial class IDevelopInterface : _IDevelopInterface_Base;
public abstract class _IDevelopInterface_Base : IpcInterface {
	protected virtual void LaunchProgram(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchProgram not implemented");
	protected virtual void TerminateProcess(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateProcess");
	protected virtual void TerminateProgram(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateProgram");
	protected virtual KObject GetShellEventHandle() =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.GetShellEventHandle not implemented");
	protected virtual void GetShellEventInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.GetShellEventInfo not implemented");
	protected virtual void TerminateApplication() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateApplication");
	protected virtual void PrepareLaunchProgramFromHost(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.PrepareLaunchProgramFromHost not implemented");
	protected virtual void LaunchApplication(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchApplication not implemented");
	protected virtual void LaunchApplicationWithStorageId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchApplicationWithStorageId not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // LaunchProgram
				om.Initialize(0, 0, 8);
				LaunchProgram(im.GetBytes(8, 0x18), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // TerminateProcess
				om.Initialize(0, 0, 0);
				TerminateProcess(im.GetBytes(8, 0x8));
				break;
			}
			case 0x2: { // TerminateProgram
				om.Initialize(0, 0, 0);
				TerminateProgram(im.GetBytes(8, 0x8));
				break;
			}
			case 0x4: { // GetShellEventHandle
				om.Initialize(0, 1, 0);
				var _return = GetShellEventHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetShellEventInfo
				om.Initialize(0, 0, 16);
				GetShellEventInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // TerminateApplication
				om.Initialize(0, 0, 0);
				TerminateApplication();
				break;
			}
			case 0x7: { // PrepareLaunchProgramFromHost
				om.Initialize(0, 0, 16);
				PrepareLaunchProgramFromHost(im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // LaunchApplication
				om.Initialize(0, 0, 8);
				LaunchApplication(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // LaunchApplicationWithStorageId
				om.Initialize(0, 0, 8);
				LaunchApplicationWithStorageId(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDevelopInterface");
		}
	}
}

public partial class IDocumentInterface : _IDocumentInterface_Base;
public abstract class _IDocumentInterface_Base : IpcInterface {
	protected virtual void GetApplicationContentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDocumentInterface.GetApplicationContentPath not implemented");
	protected virtual void ResolveApplicationContentPath(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDocumentInterface.ResolveApplicationContentPath");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x15: { // GetApplicationContentPath
				om.Initialize(0, 0, 0);
				GetApplicationContentPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x17: { // ResolveApplicationContentPath
				om.Initialize(0, 0, 0);
				ResolveApplicationContentPath(im.GetBytes(8, 0x10));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDocumentInterface");
		}
	}
}

public partial class IDownloadTaskInterface : _IDownloadTaskInterface_Base;
public abstract class _IDownloadTaskInterface_Base : IpcInterface {
	protected virtual void ClearTaskStatusList() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.ClearTaskStatusList");
	protected virtual void RequestDownloadTaskList() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.RequestDownloadTaskList");
	protected virtual void RequestEnsureDownloadTask(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDownloadTaskInterface.RequestEnsureDownloadTask not implemented");
	protected virtual void ListDownloadTaskStatus(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDownloadTaskInterface.ListDownloadTaskStatus not implemented");
	protected virtual void RequestDownloadTaskListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDownloadTaskInterface.RequestDownloadTaskListData not implemented");
	protected virtual void TryCommitCurrentApplicationDownloadTask() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.TryCommitCurrentApplicationDownloadTask");
	protected virtual void EnableAutoCommit() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.EnableAutoCommit");
	protected virtual void DisableAutoCommit() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.DisableAutoCommit");
	protected virtual void TriggerDynamicCommitEvent() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.TriggerDynamicCommitEvent");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2BD: { // ClearTaskStatusList
				om.Initialize(0, 0, 0);
				ClearTaskStatusList();
				break;
			}
			case 0x2BE: { // RequestDownloadTaskList
				om.Initialize(0, 0, 0);
				RequestDownloadTaskList();
				break;
			}
			case 0x2BF: { // RequestEnsureDownloadTask
				om.Initialize(1, 1, 0);
				RequestEnsureDownloadTask(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2C0: { // ListDownloadTaskStatus
				om.Initialize(0, 0, 4);
				ListDownloadTaskStatus(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C1: { // RequestDownloadTaskListData
				om.Initialize(1, 1, 0);
				RequestDownloadTaskListData(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2C2: { // TryCommitCurrentApplicationDownloadTask
				om.Initialize(0, 0, 0);
				TryCommitCurrentApplicationDownloadTask();
				break;
			}
			case 0x2C3: { // EnableAutoCommit
				om.Initialize(0, 0, 0);
				EnableAutoCommit();
				break;
			}
			case 0x2C4: { // DisableAutoCommit
				om.Initialize(0, 0, 0);
				DisableAutoCommit();
				break;
			}
			case 0x2C5: { // TriggerDynamicCommitEvent
				om.Initialize(0, 0, 0);
				TriggerDynamicCommitEvent();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDownloadTaskInterface");
		}
	}
}

public partial class IECommerceInterface : _IECommerceInterface_Base;
public abstract class _IECommerceInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IECommerceInterface.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(1, 1, 0);
				Unknown0(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IECommerceInterface");
		}
	}
}

public partial class IFactoryResetInterface : _IFactoryResetInterface_Base;
public abstract class _IFactoryResetInterface_Base : IpcInterface {
	protected virtual void ResetToFactorySettings() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IFactoryResetInterface.ResetToFactorySettings");
	protected virtual void ResetToFactorySettingsWithoutUserSaveData() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IFactoryResetInterface.ResetToFactorySettingsWithoutUserSaveData");
	protected virtual void ResetToFactorySettingsForRefurbishment() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IFactoryResetInterface.ResetToFactorySettingsForRefurbishment");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: { // ResetToFactorySettings
				om.Initialize(0, 0, 0);
				ResetToFactorySettings();
				break;
			}
			case 0x65: { // ResetToFactorySettingsWithoutUserSaveData
				om.Initialize(0, 0, 0);
				ResetToFactorySettingsWithoutUserSaveData();
				break;
			}
			case 0x66: { // ResetToFactorySettingsForRefurbishment
				om.Initialize(0, 0, 0);
				ResetToFactorySettingsForRefurbishment();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IFactoryResetInterface");
		}
	}
}

public partial class IGameCardStopper : _IGameCardStopper_Base;
public abstract class _IGameCardStopper_Base : IpcInterface {
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IGameCardStopper");
		}
	}
}

public partial class IProgressAsyncResult : _IProgressAsyncResult_Base;
public abstract class _IProgressAsyncResult_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IProgressAsyncResult.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IProgressAsyncResult.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressAsyncResult.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IProgressAsyncResult.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressAsyncResult.Unknown4 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0();
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3();
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IProgressAsyncResult");
		}
	}
}

public partial class IProgressMonitorForDeleteUserSaveDataAll : _IProgressMonitorForDeleteUserSaveDataAll_Base;
public abstract class _IProgressMonitorForDeleteUserSaveDataAll_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown2");
	protected virtual void Unknown10(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown10 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 1, 0);
				var _return = Unknown0();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 1);
				Unknown1(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 40);
				Unknown10(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll");
		}
	}
}

public partial class IRequestServerStopper : _IRequestServerStopper_Base;
public abstract class _IRequestServerStopper_Base : IpcInterface {
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IRequestServerStopper");
		}
	}
}

public partial class IServiceGetterInterface : _IServiceGetterInterface_Base;
public abstract class _IServiceGetterInterface_Base : IpcInterface {
	protected virtual Nn.Ns.Detail.IECommerceInterface GetECommerceInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetECommerceInterface not implemented");
	protected virtual Nn.Ns.Detail.IApplicationVersionInterface GetApplicationVersionInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetApplicationVersionInterface not implemented");
	protected virtual Nn.Ns.Detail.IFactoryResetInterface GetFactoryResetInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetFactoryResetInterface not implemented");
	protected virtual Nn.Ns.Detail.IAccountProxyInterface GetAccountProxyInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetAccountProxyInterface not implemented");
	protected virtual Nn.Ns.Detail.IApplicationManagerInterface GetApplicationManagerInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetApplicationManagerInterface not implemented");
	protected virtual Nn.Ns.Detail.IDownloadTaskInterface GetDownloadTaskInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetDownloadTaskInterface not implemented");
	protected virtual Nn.Ns.Detail.IContentManagementInterface GetContentManagementInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetContentManagementInterface not implemented");
	protected virtual Nn.Ns.Detail.IDocumentInterface GetDocumentInterface() =>
		throw new NotImplementedException("Nn.Ns.Detail.IServiceGetterInterface.GetDocumentInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F38: { // GetECommerceInterface
				om.Initialize(1, 0, 0);
				var _return = GetECommerceInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F39: { // GetApplicationVersionInterface
				om.Initialize(1, 0, 0);
				var _return = GetApplicationVersionInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3A: { // GetFactoryResetInterface
				om.Initialize(1, 0, 0);
				var _return = GetFactoryResetInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3B: { // GetAccountProxyInterface
				om.Initialize(1, 0, 0);
				var _return = GetAccountProxyInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3C: { // GetApplicationManagerInterface
				om.Initialize(1, 0, 0);
				var _return = GetApplicationManagerInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3D: { // GetDownloadTaskInterface
				om.Initialize(1, 0, 0);
				var _return = GetDownloadTaskInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3E: { // GetContentManagementInterface
				om.Initialize(1, 0, 0);
				var _return = GetContentManagementInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3F: { // GetDocumentInterface
				om.Initialize(1, 0, 0);
				var _return = GetDocumentInterface();
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IServiceGetterInterface");
		}
	}
}

public partial class ISystemUpdateControl : _ISystemUpdateControl_Base;
public abstract class _ISystemUpdateControl_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown0 not implemented");
	protected virtual void Unknown1(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown1 not implemented");
	protected virtual void Unknown2(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown2 not implemented");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown4");
	protected virtual void Unknown5(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown5 not implemented");
	protected virtual void Unknown6(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown6 not implemented");
	protected virtual void Unknown7(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown7 not implemented");
	protected virtual void Unknown8() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown8");
	protected virtual void Unknown9(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown9 not implemented");
	protected virtual void Unknown10(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown10 not implemented");
	protected virtual void Unknown11(byte[] _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown11");
	protected virtual void Unknown12(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown12 not implemented");
	protected virtual void Unknown13(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown13 not implemented");
	protected virtual void Unknown14(byte[] _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown14");
	protected virtual void Unknown15(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown15 not implemented");
	protected virtual void Unknown16(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown16 not implemented");
	protected virtual void Unknown17(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown17 not implemented");
	protected virtual void Unknown18() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown18");
	protected virtual void Unknown19(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown19 not implemented");
	protected virtual void Unknown20(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown20 not implemented");
	protected virtual void Unknown21() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown21");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 1);
				Unknown0(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(1, 1, 0);
				Unknown1(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(1, 1, 0);
				Unknown2(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 16);
				Unknown3(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4();
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(1, 1, 0);
				Unknown5(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 16);
				Unknown6(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 1);
				Unknown7(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Unknown8
				om.Initialize(0, 0, 0);
				Unknown8();
				break;
			}
			case 0x9: { // Unknown9
				om.Initialize(0, 0, 8);
				Unknown9(im.GetSpan<byte>(0x15, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 8);
				Unknown10(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 0, 0);
				Unknown11(im.GetBytes(8, 0x8), Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0xC: { // Unknown12
				om.Initialize(0, 0, 8);
				Unknown12(im.GetSpan<byte>(0x15, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				om.Initialize(0, 0, 8);
				Unknown13(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // Unknown14
				om.Initialize(0, 0, 0);
				Unknown14(im.GetBytes(8, 0x8), Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0xF: { // Unknown15
				om.Initialize(0, 0, 1);
				Unknown15(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // Unknown16
				om.Initialize(1, 1, 0);
				Unknown16(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x11: { // Unknown17
				om.Initialize(0, 0, 16);
				Unknown17(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12: { // Unknown18
				om.Initialize(0, 0, 0);
				Unknown18();
				break;
			}
			case 0x13: { // Unknown19
				om.Initialize(0, 0, 8);
				Unknown19(im.GetSpan<byte>(0x15, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // Unknown20
				om.Initialize(0, 0, 8);
				Unknown20(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				om.Initialize(0, 0, 0);
				Unknown21();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateControl");
		}
	}
}

public partial class ISystemUpdateInterface : _ISystemUpdateInterface_Base;
public abstract class _ISystemUpdateInterface_Base : IpcInterface {
	protected virtual void GetBackgroundNetworkUpdateState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetBackgroundNetworkUpdateState not implemented");
	protected virtual Nn.Ns.Detail.ISystemUpdateControl OpenSystemUpdateControl() =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.OpenSystemUpdateControl not implemented");
	protected virtual void NotifyExFatDriverRequired() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyExFatDriverRequired");
	protected virtual void ClearExFatDriverStatusForDebug() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.ClearExFatDriverStatusForDebug");
	protected virtual void RequestBackgroundNetworkUpdate() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.RequestBackgroundNetworkUpdate");
	protected virtual void NotifyBackgroundNetworkUpdate(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyBackgroundNetworkUpdate");
	protected virtual void NotifyExFatDriverDownloadedForDebug() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyExFatDriverDownloadedForDebug");
	protected virtual KObject GetSystemUpdateNotificationEventForContentDelivery() =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetSystemUpdateNotificationEventForContentDelivery not implemented");
	protected virtual void NotifySystemUpdateForContentDelivery() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifySystemUpdateForContentDelivery");
	protected virtual void PrepareShutdown() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.PrepareShutdown");
	protected virtual void DestroySystemUpdateTask() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.DestroySystemUpdateTask");
	protected virtual void RequestSendSystemUpdate(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.RequestSendSystemUpdate not implemented");
	protected virtual void GetSendSystemUpdateProgress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetSendSystemUpdateProgress not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBackgroundNetworkUpdateState
				om.Initialize(0, 0, 1);
				GetBackgroundNetworkUpdateState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // OpenSystemUpdateControl
				om.Initialize(1, 0, 0);
				var _return = OpenSystemUpdateControl();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // NotifyExFatDriverRequired
				om.Initialize(0, 0, 0);
				NotifyExFatDriverRequired();
				break;
			}
			case 0x3: { // ClearExFatDriverStatusForDebug
				om.Initialize(0, 0, 0);
				ClearExFatDriverStatusForDebug();
				break;
			}
			case 0x4: { // RequestBackgroundNetworkUpdate
				om.Initialize(0, 0, 0);
				RequestBackgroundNetworkUpdate();
				break;
			}
			case 0x5: { // NotifyBackgroundNetworkUpdate
				om.Initialize(0, 0, 0);
				NotifyBackgroundNetworkUpdate(im.GetBytes(8, 0x10));
				break;
			}
			case 0x6: { // NotifyExFatDriverDownloadedForDebug
				om.Initialize(0, 0, 0);
				NotifyExFatDriverDownloadedForDebug();
				break;
			}
			case 0x9: { // GetSystemUpdateNotificationEventForContentDelivery
				om.Initialize(0, 1, 0);
				var _return = GetSystemUpdateNotificationEventForContentDelivery();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // NotifySystemUpdateForContentDelivery
				om.Initialize(0, 0, 0);
				NotifySystemUpdateForContentDelivery();
				break;
			}
			case 0xB: { // PrepareShutdown
				om.Initialize(0, 0, 0);
				PrepareShutdown();
				break;
			}
			case 0x10: { // DestroySystemUpdateTask
				om.Initialize(0, 0, 0);
				DestroySystemUpdateTask();
				break;
			}
			case 0x11: { // RequestSendSystemUpdate
				om.Initialize(1, 1, 0);
				RequestSendSystemUpdate(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x12: { // GetSendSystemUpdateProgress
				om.Initialize(0, 0, 16);
				GetSendSystemUpdateProgress(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateInterface");
		}
	}
}

public partial class IVulnerabilityManagerInterface : _IVulnerabilityManagerInterface_Base;
public abstract class _IVulnerabilityManagerInterface_Base : IpcInterface {
	protected virtual void NeedsUpdateVulnerability(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IVulnerabilityManagerInterface.NeedsUpdateVulnerability not implemented");
	protected virtual void UpdateSafeSystemVersionForDebug(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IVulnerabilityManagerInterface.UpdateSafeSystemVersionForDebug");
	protected virtual void GetSafeSystemVersion(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IVulnerabilityManagerInterface.GetSafeSystemVersion not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4B0: { // NeedsUpdateVulnerability
				om.Initialize(0, 0, 1);
				NeedsUpdateVulnerability(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4B1: { // UpdateSafeSystemVersionForDebug
				om.Initialize(0, 0, 0);
				UpdateSafeSystemVersionForDebug(im.GetBytes(8, 0x10));
				break;
			}
			case 0x4B2: { // GetSafeSystemVersion
				om.Initialize(0, 0, 16);
				GetSafeSystemVersion(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IVulnerabilityManagerInterface");
		}
	}
}

