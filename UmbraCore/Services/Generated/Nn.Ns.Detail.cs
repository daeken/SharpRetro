using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ns.Detail;
public partial class IAccountProxyInterface : _IAccountProxyInterface_Base;
public abstract class _IAccountProxyInterface_Base : IpcInterface {
	protected virtual void CreateUserAccount(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IAccountProxyInterface.CreateUserAccount");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserAccount
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAccountProxyInterface");
		}
	}
}

public partial class IApplicationManagerInterface : _IApplicationManagerInterface_Base;
public abstract class _IApplicationManagerInterface_Base : IpcInterface {
	protected virtual void ListApplicationRecord(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecord not implemented");
	protected virtual void GenerateApplicationRecordCount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GenerateApplicationRecordCount not implemented");
	protected virtual KObject GetApplicationRecordUpdateSystemEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecordUpdateSystemEvent not implemented");
	protected virtual void GetApplicationViewDeprecated(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationViewDeprecated not implemented");
	protected virtual void DeleteApplicationEntity(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationEntity");
	protected virtual void DeleteApplicationCompletely(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationCompletely");
	protected virtual void IsAnyApplicationEntityRedundant(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsAnyApplicationEntityRedundant not implemented");
	protected virtual void DeleteRedundantApplicationEntity() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteRedundantApplicationEntity");
	protected virtual void IsApplicationEntityMovable(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationEntityMovable not implemented");
	protected virtual void MoveApplicationEntity(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.MoveApplicationEntity");
	protected virtual void CalculateApplicationOccupiedSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationOccupiedSize not implemented");
	protected virtual void PushApplicationRecord(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushApplicationRecord");
	protected virtual void ListApplicationRecordContentMeta(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecordContentMeta not implemented");
	protected virtual void LaunchApplication(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchApplication not implemented");
	protected virtual void GetApplicationContentPath(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationContentPath not implemented");
	protected virtual void TerminateApplication(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateApplication");
	protected virtual void ResolveApplicationContentPath(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResolveApplicationContentPath");
	protected virtual void BeginInstallApplication(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.BeginInstallApplication");
	protected virtual void DeleteApplicationRecord(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationRecord");
	protected virtual void RequestApplicationUpdateInfo(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestApplicationUpdateInfo not implemented");
	protected virtual void CancelApplicationDownload(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CancelApplicationDownload");
	protected virtual void ResumeApplicationDownload(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeApplicationDownload");
	protected virtual void UpdateVersionList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.UpdateVersionList");
	protected virtual void PushLaunchVersion(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushLaunchVersion");
	protected virtual void ListRequiredVersion(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListRequiredVersion not implemented");
	protected virtual void CheckApplicationLaunchVersion(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationLaunchVersion");
	protected virtual void CheckApplicationLaunchRights(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationLaunchRights");
	protected virtual void GetApplicationLogoData(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationLogoData not implemented");
	protected virtual void CalculateApplicationDownloadRequiredSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationDownloadRequiredSize not implemented");
	protected virtual void CleanupSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupSdCard");
	protected virtual void CheckSdCardMountStatus() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckSdCardMountStatus");
	protected virtual KObject GetSdCardMountStatusChangedEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSdCardMountStatusChangedEvent not implemented");
	protected virtual KObject GetGameCardAttachmentEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardAttachmentEvent not implemented");
	protected virtual void GetGameCardAttachmentInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardAttachmentInfo not implemented");
	protected virtual void GetTotalSpaceSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetTotalSpaceSize not implemented");
	protected virtual void GetFreeSpaceSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetFreeSpaceSize not implemented");
	protected virtual KObject GetSdCardRemovedEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSdCardRemovedEvent not implemented");
	protected virtual KObject GetGameCardUpdateDetectionEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardUpdateDetectionEvent not implemented");
	protected virtual void DisableApplicationAutoDelete(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DisableApplicationAutoDelete");
	protected virtual void EnableApplicationAutoDelete(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationAutoDelete");
	protected virtual void GetApplicationDesiredLanguage(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDesiredLanguage not implemented");
	protected virtual void SetApplicationTerminateResult(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.SetApplicationTerminateResult");
	protected virtual void ClearApplicationTerminateResult(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ClearApplicationTerminateResult");
	protected virtual void GetLastSdCardMountUnexpectedResult() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastSdCardMountUnexpectedResult");
	protected virtual void ConvertApplicationLanguageToLanguageCode(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ConvertApplicationLanguageToLanguageCode not implemented");
	protected virtual void ConvertLanguageCodeToApplicationLanguage(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ConvertLanguageCodeToApplicationLanguage not implemented");
	protected virtual void GetBackgroundDownloadStressTaskInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetBackgroundDownloadStressTaskInfo not implemented");
	protected virtual Nn.Ns.Detail.IGameCardStopper GetGameCardStopper() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardStopper not implemented");
	protected virtual void IsSystemProgramInstalled(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsSystemProgramInstalled not implemented");
	protected virtual void StartApplyDeltaTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.StartApplyDeltaTask");
	protected virtual Nn.Ns.Detail.IRequestServerStopper GetRequestServerStopper() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetRequestServerStopper not implemented");
	protected virtual void GetBackgroundApplyDeltaStressTaskInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetBackgroundApplyDeltaStressTaskInfo not implemented");
	protected virtual void CancelApplicationApplyDelta(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CancelApplicationApplyDelta");
	protected virtual void ResumeApplicationApplyDelta(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeApplicationApplyDelta");
	protected virtual void CalculateApplicationApplyDeltaRequiredSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationApplyDeltaRequiredSize not implemented");
	protected virtual void ResumeAll() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeAll");
	protected virtual void GetStorageSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetStorageSize not implemented");
	protected virtual void RequestDownloadApplication(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplication not implemented");
	protected virtual void RequestDownloadAddOnContent(Span<byte> _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadAddOnContent not implemented");
	protected virtual void DownloadApplication(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DownloadApplication");
	protected virtual void CheckApplicationResumeRights(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationResumeRights");
	protected virtual KObject GetDynamicCommitEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetDynamicCommitEvent not implemented");
	protected virtual void RequestUpdateApplication2(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestUpdateApplication2 not implemented");
	protected virtual void EnableApplicationCrashReport(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationCrashReport");
	protected virtual void IsApplicationCrashReportEnabled(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationCrashReportEnabled not implemented");
	protected virtual void BoostSystemMemoryResourceLimit(Span<byte> _0) =>
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
	protected virtual void CalculateUserSaveDataStatistics(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateUserSaveDataStatistics not implemented");
	protected virtual Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll DeleteUserSaveDataAll(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.DeleteUserSaveDataAll not implemented");
	protected virtual void DeleteUserSystemSaveData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteUserSystemSaveData");
	protected virtual void Unknown211() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown211");
	protected virtual void UnregisterNetworkServiceAccount(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.UnregisterNetworkServiceAccount");
	protected virtual void Unknown221() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown221");
	protected virtual KObject GetApplicationShellEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationShellEvent not implemented");
	protected virtual void PopApplicationShellEventInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.PopApplicationShellEventInfo not implemented");
	protected virtual void LaunchLibraryApplet(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchLibraryApplet not implemented");
	protected virtual void TerminateLibraryApplet(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateLibraryApplet");
	protected virtual void LaunchSystemApplet(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchSystemApplet not implemented");
	protected virtual void TerminateSystemApplet(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateSystemApplet");
	protected virtual void LaunchOverlayApplet(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchOverlayApplet not implemented");
	protected virtual void TerminateOverlayApplet(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateOverlayApplet");
	protected virtual void GetApplicationControlData(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationControlData not implemented");
	protected virtual void InvalidateAllApplicationControlCache() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InvalidateAllApplicationControlCache");
	protected virtual void RequestDownloadApplicationControlData(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplicationControlData not implemented");
	protected virtual void GetMaxApplicationControlCacheCount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetMaxApplicationControlCacheCount not implemented");
	protected virtual void InvalidateApplicationControlCache(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InvalidateApplicationControlCache");
	protected virtual void ListApplicationControlCacheEntryInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationControlCacheEntryInfo not implemented");
	protected virtual void Unknown406() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown406");
	protected virtual void RequestCheckGameCardRegistration(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestCheckGameCardRegistration not implemented");
	protected virtual void RequestGameCardRegistrationGoldPoint(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestGameCardRegistrationGoldPoint not implemented");
	protected virtual void RequestRegisterGameCard(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestRegisterGameCard not implemented");
	protected virtual KObject GetGameCardMountFailureEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetGameCardMountFailureEvent not implemented");
	protected virtual void IsGameCardInserted(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsGameCardInserted not implemented");
	protected virtual void EnsureGameCardAccess() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnsureGameCardAccess");
	protected virtual void GetLastGameCardMountFailureResult() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastGameCardMountFailureResult");
	protected virtual void ListApplicationIdOnGameCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationIdOnGameCard");
	protected virtual void CountApplicationContentMeta(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CountApplicationContentMeta not implemented");
	protected virtual void ListApplicationContentMetaStatus(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationContentMetaStatus not implemented");
	protected virtual void ListAvailableAddOnContent(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListAvailableAddOnContent not implemented");
	protected virtual void GetOwnedApplicationContentMetaStatus(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetOwnedApplicationContentMetaStatus not implemented");
	protected virtual void RegisterContentsExternalKey(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RegisterContentsExternalKey");
	protected virtual void ListApplicationContentMetaStatusWithRightsCheck(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationContentMetaStatusWithRightsCheck not implemented");
	protected virtual void GetContentMetaStorage(Span<byte> _0, Span<byte> _1) =>
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
	protected virtual void ListDownloadTaskStatus(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListDownloadTaskStatus not implemented");
	protected virtual void RequestDownloadTaskListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadTaskListData not implemented");
	protected virtual void RequestVersionList() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestVersionList");
	protected virtual void ListVersionList(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListVersionList not implemented");
	protected virtual void RequestVersionListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVersionListData not implemented");
	protected virtual void GetApplicationRecord(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecord not implemented");
	protected virtual void GetApplicationRecordProperty(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecordProperty not implemented");
	protected virtual void EnableApplicationAutoUpdate(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationAutoUpdate");
	protected virtual void DisableApplicationAutoUpdate(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DisableApplicationAutoUpdate");
	protected virtual void TouchApplication(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TouchApplication");
	protected virtual void RequestApplicationUpdate(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestApplicationUpdate");
	protected virtual void IsApplicationUpdateRequested(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationUpdateRequested not implemented");
	protected virtual void WithdrawApplicationUpdateRequest(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.WithdrawApplicationUpdateRequest");
	protected virtual void ListApplicationRecordInstalledContentMeta(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecordInstalledContentMeta not implemented");
	protected virtual void WithdrawCleanupAddOnContentsWithNoRightsRecommendation(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.WithdrawCleanupAddOnContentsWithNoRightsRecommendation");
	protected virtual void Unknown910() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown910");
	protected virtual void Unknown911() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown911");
	protected virtual void Unknown912() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown912");
	protected virtual void RequestVerifyApplicationDeprecated(Span<byte> _0, KObject _1, out KObject _2, out Nn.Ns.Detail.IProgressAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyApplicationDeprecated not implemented");
	protected virtual void CorruptApplicationForDebug(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CorruptApplicationForDebug");
	protected virtual void RequestVerifyAddOnContentsRights(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IProgressAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyAddOnContentsRights not implemented");
	protected virtual void RequestVerifyApplication() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyApplication");
	protected virtual void CorruptContentForDebug() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CorruptContentForDebug");
	protected virtual void NeedsUpdateVulnerability(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsUpdateVulnerability not implemented");
	protected virtual void IsAnyApplicationEntityInstalled(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsAnyApplicationEntityInstalled not implemented");
	protected virtual void DeleteApplicationContentEntities(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationContentEntities");
	protected virtual void CleanupUnrecordedApplicationEntity(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupUnrecordedApplicationEntity");
	protected virtual void CleanupAddOnContentsWithNoRights(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupAddOnContentsWithNoRights");
	protected virtual void DeleteApplicationContentEntity(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationContentEntity");
	protected virtual void Unknown1308() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown1308");
	protected virtual void Unknown1309() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown1309");
	protected virtual void PrepareShutdown() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PrepareShutdown");
	protected virtual void FormatSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.FormatSdCard");
	protected virtual void NeedsSystemUpdateToFormatSdCard(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsSystemUpdateToFormatSdCard not implemented");
	protected virtual void GetLastSdCardFormatUnexpectedResult() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastSdCardFormatUnexpectedResult");
	protected virtual void InsertSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InsertSdCard");
	protected virtual void RemoveSdCard() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RemoveSdCard");
	protected virtual void GetSystemSeedForPseudoDeviceId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSystemSeedForPseudoDeviceId not implemented");
	protected virtual void ResetSystemSeedForPseudoDeviceId() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetSystemSeedForPseudoDeviceId");
	protected virtual void ListApplicationDownloadingContentMeta(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationDownloadingContentMeta not implemented");
	protected virtual void GetApplicationView(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationView not implemented");
	protected virtual void GetApplicationDownloadTaskStatus(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDownloadTaskStatus not implemented");
	protected virtual void GetApplicationViewDownloadErrorContext(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationViewDownloadErrorContext not implemented");
	protected virtual void IsNotificationSetupCompleted(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsNotificationSetupCompleted not implemented");
	protected virtual void GetLastNotificationInfoCount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetLastNotificationInfoCount not implemented");
	protected virtual void ListLastNotificationInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListLastNotificationInfo not implemented");
	protected virtual void ListNotificationTask(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListNotificationTask not implemented");
	protected virtual void IsActiveAccount(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsActiveAccount not implemented");
	protected virtual void RequestDownloadApplicationPrepurchasedRights(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplicationPrepurchasedRights not implemented");
	protected virtual void GetApplicationTicketInfo() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationTicketInfo");
	protected virtual void GetSystemDeliveryInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSystemDeliveryInfo not implemented");
	protected virtual void SelectLatestSystemDeliveryInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.SelectLatestSystemDeliveryInfo not implemented");
	protected virtual void VerifyDeliveryProtocolVersion(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.VerifyDeliveryProtocolVersion");
	protected virtual void GetApplicationDeliveryInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDeliveryInfo not implemented");
	protected virtual void HasAllContentsToDeliver(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.HasAllContentsToDeliver not implemented");
	protected virtual void CompareApplicationDeliveryInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CompareApplicationDeliveryInfo not implemented");
	protected virtual void CanDeliverApplication(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CanDeliverApplication not implemented");
	protected virtual void ListContentMetaKeyToDeliverApplication(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListContentMetaKeyToDeliverApplication not implemented");
	protected virtual void NeedsSystemUpdateToDeliverApplication(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsSystemUpdateToDeliverApplication not implemented");
	protected virtual void EstimateRequiredSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.EstimateRequiredSize not implemented");
	protected virtual void RequestReceiveApplication(Span<byte> _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestReceiveApplication not implemented");
	protected virtual void CommitReceiveApplication(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CommitReceiveApplication");
	protected virtual void GetReceiveApplicationProgress(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetReceiveApplicationProgress not implemented");
	protected virtual void RequestSendApplication(Span<byte> _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestSendApplication not implemented");
	protected virtual void GetSendApplicationProgress(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSendApplicationProgress not implemented");
	protected virtual void CompareSystemDeliveryInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CompareSystemDeliveryInfo not implemented");
	protected virtual void ListNotCommittedContentMeta(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListNotCommittedContentMeta not implemented");
	protected virtual void CreateDownloadTask(Span<byte> _0, Span<byte> _1) =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListApplicationRecord
				break;
			}
			case 0x1: { // GenerateApplicationRecordCount
				break;
			}
			case 0x2: { // GetApplicationRecordUpdateSystemEvent
				break;
			}
			case 0x3: { // GetApplicationViewDeprecated
				break;
			}
			case 0x4: { // DeleteApplicationEntity
				break;
			}
			case 0x5: { // DeleteApplicationCompletely
				break;
			}
			case 0x6: { // IsAnyApplicationEntityRedundant
				break;
			}
			case 0x7: { // DeleteRedundantApplicationEntity
				break;
			}
			case 0x8: { // IsApplicationEntityMovable
				break;
			}
			case 0x9: { // MoveApplicationEntity
				break;
			}
			case 0xB: { // CalculateApplicationOccupiedSize
				break;
			}
			case 0x10: { // PushApplicationRecord
				break;
			}
			case 0x11: { // ListApplicationRecordContentMeta
				break;
			}
			case 0x13: { // LaunchApplication
				break;
			}
			case 0x15: { // GetApplicationContentPath
				break;
			}
			case 0x16: { // TerminateApplication
				break;
			}
			case 0x17: { // ResolveApplicationContentPath
				break;
			}
			case 0x1A: { // BeginInstallApplication
				break;
			}
			case 0x1B: { // DeleteApplicationRecord
				break;
			}
			case 0x1E: { // RequestApplicationUpdateInfo
				break;
			}
			case 0x20: { // CancelApplicationDownload
				break;
			}
			case 0x21: { // ResumeApplicationDownload
				break;
			}
			case 0x23: { // UpdateVersionList
				break;
			}
			case 0x24: { // PushLaunchVersion
				break;
			}
			case 0x25: { // ListRequiredVersion
				break;
			}
			case 0x26: { // CheckApplicationLaunchVersion
				break;
			}
			case 0x27: { // CheckApplicationLaunchRights
				break;
			}
			case 0x28: { // GetApplicationLogoData
				break;
			}
			case 0x29: { // CalculateApplicationDownloadRequiredSize
				break;
			}
			case 0x2A: { // CleanupSdCard
				break;
			}
			case 0x2B: { // CheckSdCardMountStatus
				break;
			}
			case 0x2C: { // GetSdCardMountStatusChangedEvent
				break;
			}
			case 0x2D: { // GetGameCardAttachmentEvent
				break;
			}
			case 0x2E: { // GetGameCardAttachmentInfo
				break;
			}
			case 0x2F: { // GetTotalSpaceSize
				break;
			}
			case 0x30: { // GetFreeSpaceSize
				break;
			}
			case 0x31: { // GetSdCardRemovedEvent
				break;
			}
			case 0x34: { // GetGameCardUpdateDetectionEvent
				break;
			}
			case 0x35: { // DisableApplicationAutoDelete
				break;
			}
			case 0x36: { // EnableApplicationAutoDelete
				break;
			}
			case 0x37: { // GetApplicationDesiredLanguage
				break;
			}
			case 0x38: { // SetApplicationTerminateResult
				break;
			}
			case 0x39: { // ClearApplicationTerminateResult
				break;
			}
			case 0x3A: { // GetLastSdCardMountUnexpectedResult
				break;
			}
			case 0x3B: { // ConvertApplicationLanguageToLanguageCode
				break;
			}
			case 0x3C: { // ConvertLanguageCodeToApplicationLanguage
				break;
			}
			case 0x3D: { // GetBackgroundDownloadStressTaskInfo
				break;
			}
			case 0x3E: { // GetGameCardStopper
				break;
			}
			case 0x3F: { // IsSystemProgramInstalled
				break;
			}
			case 0x40: { // StartApplyDeltaTask
				break;
			}
			case 0x41: { // GetRequestServerStopper
				break;
			}
			case 0x42: { // GetBackgroundApplyDeltaStressTaskInfo
				break;
			}
			case 0x43: { // CancelApplicationApplyDelta
				break;
			}
			case 0x44: { // ResumeApplicationApplyDelta
				break;
			}
			case 0x45: { // CalculateApplicationApplyDeltaRequiredSize
				break;
			}
			case 0x46: { // ResumeAll
				break;
			}
			case 0x47: { // GetStorageSize
				break;
			}
			case 0x50: { // RequestDownloadApplication
				break;
			}
			case 0x51: { // RequestDownloadAddOnContent
				break;
			}
			case 0x52: { // DownloadApplication
				break;
			}
			case 0x53: { // CheckApplicationResumeRights
				break;
			}
			case 0x54: { // GetDynamicCommitEvent
				break;
			}
			case 0x55: { // RequestUpdateApplication2
				break;
			}
			case 0x56: { // EnableApplicationCrashReport
				break;
			}
			case 0x57: { // IsApplicationCrashReportEnabled
				break;
			}
			case 0x5A: { // BoostSystemMemoryResourceLimit
				break;
			}
			case 0x5B: { // Unknown91
				break;
			}
			case 0x5C: { // Unknown92
				break;
			}
			case 0x5D: { // Unknown93
				break;
			}
			case 0x5E: { // LaunchApplication2
				break;
			}
			case 0x5F: { // Unknown95
				break;
			}
			case 0x60: { // Unknown96
				break;
			}
			case 0x61: { // Unknown97
				break;
			}
			case 0x62: { // Unknown98
				break;
			}
			case 0x64: { // ResetToFactorySettings
				break;
			}
			case 0x65: { // ResetToFactorySettingsWithoutUserSaveData
				break;
			}
			case 0x66: { // ResetToFactorySettingsForRefurbishment
				break;
			}
			case 0xC8: { // CalculateUserSaveDataStatistics
				break;
			}
			case 0xC9: { // DeleteUserSaveDataAll
				break;
			}
			case 0xD2: { // DeleteUserSystemSaveData
				break;
			}
			case 0xD3: { // Unknown211
				break;
			}
			case 0xDC: { // UnregisterNetworkServiceAccount
				break;
			}
			case 0xDD: { // Unknown221
				break;
			}
			case 0x12C: { // GetApplicationShellEvent
				break;
			}
			case 0x12D: { // PopApplicationShellEventInfo
				break;
			}
			case 0x12E: { // LaunchLibraryApplet
				break;
			}
			case 0x12F: { // TerminateLibraryApplet
				break;
			}
			case 0x130: { // LaunchSystemApplet
				break;
			}
			case 0x131: { // TerminateSystemApplet
				break;
			}
			case 0x132: { // LaunchOverlayApplet
				break;
			}
			case 0x133: { // TerminateOverlayApplet
				break;
			}
			case 0x190: { // GetApplicationControlData
				break;
			}
			case 0x191: { // InvalidateAllApplicationControlCache
				break;
			}
			case 0x192: { // RequestDownloadApplicationControlData
				break;
			}
			case 0x193: { // GetMaxApplicationControlCacheCount
				break;
			}
			case 0x194: { // InvalidateApplicationControlCache
				break;
			}
			case 0x195: { // ListApplicationControlCacheEntryInfo
				break;
			}
			case 0x196: { // Unknown406
				break;
			}
			case 0x1F6: { // RequestCheckGameCardRegistration
				break;
			}
			case 0x1F7: { // RequestGameCardRegistrationGoldPoint
				break;
			}
			case 0x1F8: { // RequestRegisterGameCard
				break;
			}
			case 0x1F9: { // GetGameCardMountFailureEvent
				break;
			}
			case 0x1FA: { // IsGameCardInserted
				break;
			}
			case 0x1FB: { // EnsureGameCardAccess
				break;
			}
			case 0x1FC: { // GetLastGameCardMountFailureResult
				break;
			}
			case 0x1FD: { // ListApplicationIdOnGameCard
				break;
			}
			case 0x258: { // CountApplicationContentMeta
				break;
			}
			case 0x259: { // ListApplicationContentMetaStatus
				break;
			}
			case 0x25A: { // ListAvailableAddOnContent
				break;
			}
			case 0x25B: { // GetOwnedApplicationContentMetaStatus
				break;
			}
			case 0x25C: { // RegisterContentsExternalKey
				break;
			}
			case 0x25D: { // ListApplicationContentMetaStatusWithRightsCheck
				break;
			}
			case 0x25E: { // GetContentMetaStorage
				break;
			}
			case 0x25F: { // Unknown607
				break;
			}
			case 0x2BC: { // PushDownloadTaskList
				break;
			}
			case 0x2BD: { // ClearTaskStatusList
				break;
			}
			case 0x2BE: { // RequestDownloadTaskList
				break;
			}
			case 0x2BF: { // RequestEnsureDownloadTask
				break;
			}
			case 0x2C0: { // ListDownloadTaskStatus
				break;
			}
			case 0x2C1: { // RequestDownloadTaskListData
				break;
			}
			case 0x320: { // RequestVersionList
				break;
			}
			case 0x321: { // ListVersionList
				break;
			}
			case 0x322: { // RequestVersionListData
				break;
			}
			case 0x384: { // GetApplicationRecord
				break;
			}
			case 0x385: { // GetApplicationRecordProperty
				break;
			}
			case 0x386: { // EnableApplicationAutoUpdate
				break;
			}
			case 0x387: { // DisableApplicationAutoUpdate
				break;
			}
			case 0x388: { // TouchApplication
				break;
			}
			case 0x389: { // RequestApplicationUpdate
				break;
			}
			case 0x38A: { // IsApplicationUpdateRequested
				break;
			}
			case 0x38B: { // WithdrawApplicationUpdateRequest
				break;
			}
			case 0x38C: { // ListApplicationRecordInstalledContentMeta
				break;
			}
			case 0x38D: { // WithdrawCleanupAddOnContentsWithNoRightsRecommendation
				break;
			}
			case 0x38E: { // Unknown910
				break;
			}
			case 0x38F: { // Unknown911
				break;
			}
			case 0x390: { // Unknown912
				break;
			}
			case 0x3E8: { // RequestVerifyApplicationDeprecated
				break;
			}
			case 0x3E9: { // CorruptApplicationForDebug
				break;
			}
			case 0x3EA: { // RequestVerifyAddOnContentsRights
				break;
			}
			case 0x3EB: { // RequestVerifyApplication
				break;
			}
			case 0x3EC: { // CorruptContentForDebug
				break;
			}
			case 0x4B0: { // NeedsUpdateVulnerability
				break;
			}
			case 0x514: { // IsAnyApplicationEntityInstalled
				break;
			}
			case 0x515: { // DeleteApplicationContentEntities
				break;
			}
			case 0x516: { // CleanupUnrecordedApplicationEntity
				break;
			}
			case 0x517: { // CleanupAddOnContentsWithNoRights
				break;
			}
			case 0x518: { // DeleteApplicationContentEntity
				break;
			}
			case 0x51C: { // Unknown1308
				break;
			}
			case 0x51D: { // Unknown1309
				break;
			}
			case 0x578: { // PrepareShutdown
				break;
			}
			case 0x5DC: { // FormatSdCard
				break;
			}
			case 0x5DD: { // NeedsSystemUpdateToFormatSdCard
				break;
			}
			case 0x5DE: { // GetLastSdCardFormatUnexpectedResult
				break;
			}
			case 0x5E0: { // InsertSdCard
				break;
			}
			case 0x5E1: { // RemoveSdCard
				break;
			}
			case 0x640: { // GetSystemSeedForPseudoDeviceId
				break;
			}
			case 0x641: { // ResetSystemSeedForPseudoDeviceId
				break;
			}
			case 0x6A4: { // ListApplicationDownloadingContentMeta
				break;
			}
			case 0x6A5: { // GetApplicationView
				break;
			}
			case 0x6A6: { // GetApplicationDownloadTaskStatus
				break;
			}
			case 0x6A7: { // GetApplicationViewDownloadErrorContext
				break;
			}
			case 0x708: { // IsNotificationSetupCompleted
				break;
			}
			case 0x709: { // GetLastNotificationInfoCount
				break;
			}
			case 0x70A: { // ListLastNotificationInfo
				break;
			}
			case 0x70B: { // ListNotificationTask
				break;
			}
			case 0x76C: { // IsActiveAccount
				break;
			}
			case 0x76D: { // RequestDownloadApplicationPrepurchasedRights
				break;
			}
			case 0x76E: { // GetApplicationTicketInfo
				break;
			}
			case 0x7D0: { // GetSystemDeliveryInfo
				break;
			}
			case 0x7D1: { // SelectLatestSystemDeliveryInfo
				break;
			}
			case 0x7D2: { // VerifyDeliveryProtocolVersion
				break;
			}
			case 0x7D3: { // GetApplicationDeliveryInfo
				break;
			}
			case 0x7D4: { // HasAllContentsToDeliver
				break;
			}
			case 0x7D5: { // CompareApplicationDeliveryInfo
				break;
			}
			case 0x7D6: { // CanDeliverApplication
				break;
			}
			case 0x7D7: { // ListContentMetaKeyToDeliverApplication
				break;
			}
			case 0x7D8: { // NeedsSystemUpdateToDeliverApplication
				break;
			}
			case 0x7D9: { // EstimateRequiredSize
				break;
			}
			case 0x7DA: { // RequestReceiveApplication
				break;
			}
			case 0x7DB: { // CommitReceiveApplication
				break;
			}
			case 0x7DC: { // GetReceiveApplicationProgress
				break;
			}
			case 0x7DD: { // RequestSendApplication
				break;
			}
			case 0x7DE: { // GetSendApplicationProgress
				break;
			}
			case 0x7DF: { // CompareSystemDeliveryInfo
				break;
			}
			case 0x7E0: { // ListNotCommittedContentMeta
				break;
			}
			case 0x7E1: { // CreateDownloadTask
				break;
			}
			case 0x7E2: { // Unknown2018
				break;
			}
			case 0x802: { // Unknown2050
				break;
			}
			case 0x834: { // Unknown2100
				break;
			}
			case 0x835: { // Unknown2101
				break;
			}
			case 0x866: { // Unknown2150
				break;
			}
			case 0x867: { // Unknown2151
				break;
			}
			case 0x868: { // Unknown2152
				break;
			}
			case 0x869: { // Unknown2153
				break;
			}
			case 0x86A: { // Unknown2154
				break;
			}
			case 0x870: { // Unknown2160
				break;
			}
			case 0x871: { // Unknown2161
				break;
			}
			case 0x87A: { // Unknown2170
				break;
			}
			case 0x87B: { // Unknown2171
				break;
			}
			case 0x884: { // Unknown2180
				break;
			}
			case 0x885: { // Unknown2181
				break;
			}
			case 0x886: { // Unknown2182
				break;
			}
			case 0x88E: { // Unknown2190
				break;
			}
			case 0x897: { // Unknown2199
				break;
			}
			case 0x898: { // Unknown2200
				break;
			}
			case 0x899: { // Unknown2201
				break;
			}
			case 0x8CA: { // Unknown2250
				break;
			}
			case 0x8FC: { // Unknown2300
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IApplicationManagerInterface");
		}
	}
}

public partial class IApplicationVersionInterface : _IApplicationVersionInterface_Base;
public abstract class _IApplicationVersionInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown1");
	protected virtual void Unknown35(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown35");
	protected virtual void Unknown36(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown36");
	protected virtual void Unknown37(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown37 not implemented");
	protected virtual void Unknown800() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown800");
	protected virtual void Unknown801(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown801 not implemented");
	protected virtual void Unknown802(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown802 not implemented");
	protected virtual void Unknown1000() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown1000");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x23: { // Unknown35
				break;
			}
			case 0x24: { // Unknown36
				break;
			}
			case 0x25: { // Unknown37
				break;
			}
			case 0x320: { // Unknown800
				break;
			}
			case 0x321: { // Unknown801
				break;
			}
			case 0x322: { // Unknown802
				break;
			}
			case 0x3E8: { // Unknown1000
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAsyncResult");
		}
	}
}

public partial class IAsyncValue : _IAsyncValue_Base;
public abstract class _IAsyncValue_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IAsyncValue.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown3 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAsyncValue");
		}
	}
}

public partial class IContentManagementInterface : _IContentManagementInterface_Base;
public abstract class _IContentManagementInterface_Base : IpcInterface {
	protected virtual void CalculateApplicationOccupiedSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.CalculateApplicationOccupiedSize not implemented");
	protected virtual void CheckSdCardMountStatus() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IContentManagementInterface.CheckSdCardMountStatus");
	protected virtual void GetTotalSpaceSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.GetTotalSpaceSize not implemented");
	protected virtual void GetFreeSpaceSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.GetFreeSpaceSize not implemented");
	protected virtual void CountApplicationContentMeta(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.CountApplicationContentMeta not implemented");
	protected virtual void ListApplicationContentMetaStatus(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.ListApplicationContentMetaStatus not implemented");
	protected virtual void ListApplicationContentMetaStatusWithRightsCheck(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.ListApplicationContentMetaStatusWithRightsCheck not implemented");
	protected virtual void IsAnyApplicationRunning(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IContentManagementInterface.IsAnyApplicationRunning not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xB: { // CalculateApplicationOccupiedSize
				break;
			}
			case 0x2B: { // CheckSdCardMountStatus
				break;
			}
			case 0x2F: { // GetTotalSpaceSize
				break;
			}
			case 0x30: { // GetFreeSpaceSize
				break;
			}
			case 0x258: { // CountApplicationContentMeta
				break;
			}
			case 0x259: { // ListApplicationContentMetaStatus
				break;
			}
			case 0x25D: { // ListApplicationContentMetaStatusWithRightsCheck
				break;
			}
			case 0x25F: { // IsAnyApplicationRunning
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IContentManagementInterface");
		}
	}
}

public partial class IDevelopInterface : _IDevelopInterface_Base;
public abstract class _IDevelopInterface_Base : IpcInterface {
	protected virtual void LaunchProgram(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchProgram not implemented");
	protected virtual void TerminateProcess(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateProcess");
	protected virtual void TerminateProgram(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateProgram");
	protected virtual KObject GetShellEventHandle() =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.GetShellEventHandle not implemented");
	protected virtual void GetShellEventInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.GetShellEventInfo not implemented");
	protected virtual void TerminateApplication() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateApplication");
	protected virtual void PrepareLaunchProgramFromHost(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.PrepareLaunchProgramFromHost not implemented");
	protected virtual void LaunchApplication(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchApplication not implemented");
	protected virtual void LaunchApplicationWithStorageId(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchApplicationWithStorageId not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // LaunchProgram
				break;
			}
			case 0x1: { // TerminateProcess
				break;
			}
			case 0x2: { // TerminateProgram
				break;
			}
			case 0x4: { // GetShellEventHandle
				break;
			}
			case 0x5: { // GetShellEventInfo
				break;
			}
			case 0x6: { // TerminateApplication
				break;
			}
			case 0x7: { // PrepareLaunchProgramFromHost
				break;
			}
			case 0x8: { // LaunchApplication
				break;
			}
			case 0x9: { // LaunchApplicationWithStorageId
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDevelopInterface");
		}
	}
}

public partial class IDocumentInterface : _IDocumentInterface_Base;
public abstract class _IDocumentInterface_Base : IpcInterface {
	protected virtual void GetApplicationContentPath(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDocumentInterface.GetApplicationContentPath not implemented");
	protected virtual void ResolveApplicationContentPath(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IDocumentInterface.ResolveApplicationContentPath");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x15: { // GetApplicationContentPath
				break;
			}
			case 0x17: { // ResolveApplicationContentPath
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
	protected virtual void ListDownloadTaskStatus(Span<byte> _0, Span<byte> _1) =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2BD: { // ClearTaskStatusList
				break;
			}
			case 0x2BE: { // RequestDownloadTaskList
				break;
			}
			case 0x2BF: { // RequestEnsureDownloadTask
				break;
			}
			case 0x2C0: { // ListDownloadTaskStatus
				break;
			}
			case 0x2C1: { // RequestDownloadTaskListData
				break;
			}
			case 0x2C2: { // TryCommitCurrentApplicationDownloadTask
				break;
			}
			case 0x2C3: { // EnableAutoCommit
				break;
			}
			case 0x2C4: { // DisableAutoCommit
				break;
			}
			case 0x2C5: { // TriggerDynamicCommitEvent
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDownloadTaskInterface");
		}
	}
}

public partial class IECommerceInterface : _IECommerceInterface_Base;
public abstract class _IECommerceInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IECommerceInterface.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: { // ResetToFactorySettings
				break;
			}
			case 0x65: { // ResetToFactorySettingsWithoutUserSaveData
				break;
			}
			case 0x66: { // ResetToFactorySettingsForRefurbishment
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IFactoryResetInterface");
		}
	}
}

public partial class IGameCardStopper : _IGameCardStopper_Base;
public abstract class _IGameCardStopper_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
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
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown2");
	protected virtual void Unknown10(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown10 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll");
		}
	}
}

public partial class IRequestServerStopper : _IRequestServerStopper_Base;
public abstract class _IRequestServerStopper_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F38: { // GetECommerceInterface
				break;
			}
			case 0x1F39: { // GetApplicationVersionInterface
				break;
			}
			case 0x1F3A: { // GetFactoryResetInterface
				break;
			}
			case 0x1F3B: { // GetAccountProxyInterface
				break;
			}
			case 0x1F3C: { // GetApplicationManagerInterface
				break;
			}
			case 0x1F3D: { // GetDownloadTaskInterface
				break;
			}
			case 0x1F3E: { // GetContentManagementInterface
				break;
			}
			case 0x1F3F: { // GetDocumentInterface
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IServiceGetterInterface");
		}
	}
}

public partial class ISystemUpdateControl : _ISystemUpdateControl_Base;
public abstract class _ISystemUpdateControl_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown0 not implemented");
	protected virtual void Unknown1(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown1 not implemented");
	protected virtual void Unknown2(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown4");
	protected virtual void Unknown5(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown7 not implemented");
	protected virtual void Unknown8() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown8");
	protected virtual void Unknown9(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown9 not implemented");
	protected virtual void Unknown10(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown10 not implemented");
	protected virtual void Unknown11(Span<byte> _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown11");
	protected virtual void Unknown12(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown12 not implemented");
	protected virtual void Unknown13(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown13 not implemented");
	protected virtual void Unknown14(Span<byte> _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown14");
	protected virtual void Unknown15(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown15 not implemented");
	protected virtual void Unknown16(Span<byte> _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown16 not implemented");
	protected virtual void Unknown17(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown17 not implemented");
	protected virtual void Unknown18() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown18");
	protected virtual void Unknown19(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown19 not implemented");
	protected virtual void Unknown20(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown20 not implemented");
	protected virtual void Unknown21() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown21");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0x8: { // Unknown8
				break;
			}
			case 0x9: { // Unknown9
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			case 0xC: { // Unknown12
				break;
			}
			case 0xD: { // Unknown13
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // Unknown16
				break;
			}
			case 0x11: { // Unknown17
				break;
			}
			case 0x12: { // Unknown18
				break;
			}
			case 0x13: { // Unknown19
				break;
			}
			case 0x14: { // Unknown20
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateControl");
		}
	}
}

public partial class ISystemUpdateInterface : _ISystemUpdateInterface_Base;
public abstract class _ISystemUpdateInterface_Base : IpcInterface {
	protected virtual void GetBackgroundNetworkUpdateState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetBackgroundNetworkUpdateState not implemented");
	protected virtual Nn.Ns.Detail.ISystemUpdateControl OpenSystemUpdateControl() =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.OpenSystemUpdateControl not implemented");
	protected virtual void NotifyExFatDriverRequired() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyExFatDriverRequired");
	protected virtual void ClearExFatDriverStatusForDebug() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.ClearExFatDriverStatusForDebug");
	protected virtual void RequestBackgroundNetworkUpdate() =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.RequestBackgroundNetworkUpdate");
	protected virtual void NotifyBackgroundNetworkUpdate(Span<byte> _0) =>
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
	protected virtual void RequestSendSystemUpdate(Span<byte> _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.RequestSendSystemUpdate not implemented");
	protected virtual void GetSendSystemUpdateProgress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetSendSystemUpdateProgress not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBackgroundNetworkUpdateState
				break;
			}
			case 0x1: { // OpenSystemUpdateControl
				break;
			}
			case 0x2: { // NotifyExFatDriverRequired
				break;
			}
			case 0x3: { // ClearExFatDriverStatusForDebug
				break;
			}
			case 0x4: { // RequestBackgroundNetworkUpdate
				break;
			}
			case 0x5: { // NotifyBackgroundNetworkUpdate
				break;
			}
			case 0x6: { // NotifyExFatDriverDownloadedForDebug
				break;
			}
			case 0x9: { // GetSystemUpdateNotificationEventForContentDelivery
				break;
			}
			case 0xA: { // NotifySystemUpdateForContentDelivery
				break;
			}
			case 0xB: { // PrepareShutdown
				break;
			}
			case 0x10: { // DestroySystemUpdateTask
				break;
			}
			case 0x11: { // RequestSendSystemUpdate
				break;
			}
			case 0x12: { // GetSendSystemUpdateProgress
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateInterface");
		}
	}
}

public partial class IVulnerabilityManagerInterface : _IVulnerabilityManagerInterface_Base;
public abstract class _IVulnerabilityManagerInterface_Base : IpcInterface {
	protected virtual void NeedsUpdateVulnerability(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IVulnerabilityManagerInterface.NeedsUpdateVulnerability not implemented");
	protected virtual void UpdateSafeSystemVersionForDebug(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ns.Detail.IVulnerabilityManagerInterface.UpdateSafeSystemVersionForDebug");
	protected virtual void GetSafeSystemVersion(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IVulnerabilityManagerInterface.GetSafeSystemVersion not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4B0: { // NeedsUpdateVulnerability
				break;
			}
			case 0x4B1: { // UpdateSafeSystemVersionForDebug
				break;
			}
			case 0x4B2: { // GetSafeSystemVersion
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IVulnerabilityManagerInterface");
		}
	}
}

