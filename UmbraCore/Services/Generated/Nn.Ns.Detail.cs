using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ns.Detail;
public partial class IAccountProxyInterface : _IAccountProxyInterface_Base;
public abstract class _IAccountProxyInterface_Base : IpcInterface {
	protected virtual void CreateUserAccount(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Ns.Detail.IAccountProxyInterface.CreateUserAccount".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserAccount
				CreateUserAccount(im.GetBytes(8, 0x21), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAccountProxyInterface");
		}
	}
}

public partial class IApplicationManagerInterface : _IApplicationManagerInterface_Base {
	public readonly string ServiceName;
	public IApplicationManagerInterface(string serviceName) => ServiceName = serviceName;
}
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationEntity".Log();
	protected virtual void DeleteApplicationCompletely(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationCompletely".Log();
	protected virtual void IsAnyApplicationEntityRedundant(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsAnyApplicationEntityRedundant not implemented");
	protected virtual void DeleteRedundantApplicationEntity() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteRedundantApplicationEntity".Log();
	protected virtual void IsApplicationEntityMovable(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationEntityMovable not implemented");
	protected virtual void MoveApplicationEntity(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.MoveApplicationEntity".Log();
	protected virtual void CalculateApplicationOccupiedSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationOccupiedSize not implemented");
	protected virtual void PushApplicationRecord(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushApplicationRecord".Log();
	protected virtual void ListApplicationRecordContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecordContentMeta not implemented");
	protected virtual void LaunchApplication(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchApplication not implemented");
	protected virtual void GetApplicationContentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationContentPath not implemented");
	protected virtual void TerminateApplication(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateApplication".Log();
	protected virtual void ResolveApplicationContentPath(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResolveApplicationContentPath".Log();
	protected virtual void BeginInstallApplication(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.BeginInstallApplication".Log();
	protected virtual void DeleteApplicationRecord(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationRecord".Log();
	protected virtual void RequestApplicationUpdateInfo(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestApplicationUpdateInfo not implemented");
	protected virtual void CancelApplicationDownload(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CancelApplicationDownload".Log();
	protected virtual void ResumeApplicationDownload(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeApplicationDownload".Log();
	protected virtual void UpdateVersionList(Span<byte> _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.UpdateVersionList".Log();
	protected virtual void PushLaunchVersion(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushLaunchVersion".Log();
	protected virtual void ListRequiredVersion(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListRequiredVersion not implemented");
	protected virtual void CheckApplicationLaunchVersion(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationLaunchVersion".Log();
	protected virtual void CheckApplicationLaunchRights(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationLaunchRights".Log();
	protected virtual void GetApplicationLogoData(byte[] _0, Span<byte> _1, out byte[] _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationLogoData not implemented");
	protected virtual void CalculateApplicationDownloadRequiredSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationDownloadRequiredSize not implemented");
	protected virtual void CleanupSdCard() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupSdCard".Log();
	protected virtual void CheckSdCardMountStatus() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckSdCardMountStatus".Log();
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DisableApplicationAutoDelete".Log();
	protected virtual void EnableApplicationAutoDelete(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationAutoDelete".Log();
	protected virtual void GetApplicationDesiredLanguage(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationDesiredLanguage not implemented");
	protected virtual void SetApplicationTerminateResult(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.SetApplicationTerminateResult".Log();
	protected virtual void ClearApplicationTerminateResult(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ClearApplicationTerminateResult".Log();
	protected virtual void GetLastSdCardMountUnexpectedResult() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastSdCardMountUnexpectedResult".Log();
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.StartApplyDeltaTask".Log();
	protected virtual Nn.Ns.Detail.IRequestServerStopper GetRequestServerStopper() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetRequestServerStopper not implemented");
	protected virtual void GetBackgroundApplyDeltaStressTaskInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetBackgroundApplyDeltaStressTaskInfo not implemented");
	protected virtual void CancelApplicationApplyDelta(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CancelApplicationApplyDelta".Log();
	protected virtual void ResumeApplicationApplyDelta(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeApplicationApplyDelta".Log();
	protected virtual void CalculateApplicationApplyDeltaRequiredSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateApplicationApplyDeltaRequiredSize not implemented");
	protected virtual void ResumeAll() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResumeAll".Log();
	protected virtual void GetStorageSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetStorageSize not implemented");
	protected virtual void RequestDownloadApplication(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplication not implemented");
	protected virtual void RequestDownloadAddOnContent(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadAddOnContent not implemented");
	protected virtual void DownloadApplication(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DownloadApplication".Log();
	protected virtual void CheckApplicationResumeRights(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CheckApplicationResumeRights".Log();
	protected virtual KObject GetDynamicCommitEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetDynamicCommitEvent not implemented");
	protected virtual void RequestUpdateApplication2(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestUpdateApplication2 not implemented");
	protected virtual void EnableApplicationCrashReport(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationCrashReport".Log();
	protected virtual void IsApplicationCrashReportEnabled(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationCrashReportEnabled not implemented");
	protected virtual void BoostSystemMemoryResourceLimit(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.BoostSystemMemoryResourceLimit".Log();
	protected virtual void Unknown91() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown91".Log();
	protected virtual void Unknown92() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown92".Log();
	protected virtual void Unknown93() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown93".Log();
	protected virtual void LaunchApplication2() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.LaunchApplication2".Log();
	protected virtual void Unknown95() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown95".Log();
	protected virtual void Unknown96() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown96".Log();
	protected virtual void Unknown97() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown97".Log();
	protected virtual void Unknown98() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown98".Log();
	protected virtual void ResetToFactorySettings() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetToFactorySettings".Log();
	protected virtual void ResetToFactorySettingsWithoutUserSaveData() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetToFactorySettingsWithoutUserSaveData".Log();
	protected virtual void ResetToFactorySettingsForRefurbishment() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetToFactorySettingsForRefurbishment".Log();
	protected virtual void CalculateUserSaveDataStatistics(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CalculateUserSaveDataStatistics not implemented");
	protected virtual Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll DeleteUserSaveDataAll(byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.DeleteUserSaveDataAll not implemented");
	protected virtual void DeleteUserSystemSaveData(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteUserSystemSaveData".Log();
	protected virtual void Unknown211() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown211".Log();
	protected virtual void UnregisterNetworkServiceAccount(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.UnregisterNetworkServiceAccount".Log();
	protected virtual void Unknown221() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown221".Log();
	protected virtual KObject GetApplicationShellEvent() =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationShellEvent not implemented");
	protected virtual void PopApplicationShellEventInfo(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.PopApplicationShellEventInfo not implemented");
	protected virtual void LaunchLibraryApplet(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchLibraryApplet not implemented");
	protected virtual void TerminateLibraryApplet(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateLibraryApplet".Log();
	protected virtual void LaunchSystemApplet(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchSystemApplet not implemented");
	protected virtual void TerminateSystemApplet(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateSystemApplet".Log();
	protected virtual void LaunchOverlayApplet(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.LaunchOverlayApplet not implemented");
	protected virtual void TerminateOverlayApplet(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TerminateOverlayApplet".Log();
	protected virtual void GetApplicationControlData(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationControlData not implemented");
	protected virtual void InvalidateAllApplicationControlCache() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InvalidateAllApplicationControlCache".Log();
	protected virtual void RequestDownloadApplicationControlData(byte[] _0, out KObject _1, out Nn.Ns.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadApplicationControlData not implemented");
	protected virtual void GetMaxApplicationControlCacheCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetMaxApplicationControlCacheCount not implemented");
	protected virtual void InvalidateApplicationControlCache(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InvalidateApplicationControlCache".Log();
	protected virtual void ListApplicationControlCacheEntryInfo(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationControlCacheEntryInfo not implemented");
	protected virtual void Unknown406() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown406".Log();
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnsureGameCardAccess".Log();
	protected virtual void GetLastGameCardMountFailureResult() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastGameCardMountFailureResult".Log();
	protected virtual void ListApplicationIdOnGameCard() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationIdOnGameCard".Log();
	protected virtual void CountApplicationContentMeta(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.CountApplicationContentMeta not implemented");
	protected virtual void ListApplicationContentMetaStatus(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationContentMetaStatus not implemented");
	protected virtual void ListAvailableAddOnContent(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListAvailableAddOnContent not implemented");
	protected virtual void GetOwnedApplicationContentMetaStatus(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetOwnedApplicationContentMetaStatus not implemented");
	protected virtual void RegisterContentsExternalKey(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RegisterContentsExternalKey".Log();
	protected virtual void ListApplicationContentMetaStatusWithRightsCheck(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationContentMetaStatusWithRightsCheck not implemented");
	protected virtual void GetContentMetaStorage(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetContentMetaStorage not implemented");
	protected virtual void Unknown607() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown607".Log();
	protected virtual void PushDownloadTaskList(Span<byte> _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PushDownloadTaskList".Log();
	protected virtual void ClearTaskStatusList() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ClearTaskStatusList".Log();
	protected virtual void RequestDownloadTaskList() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadTaskList".Log();
	protected virtual void RequestEnsureDownloadTask(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestEnsureDownloadTask not implemented");
	protected virtual void ListDownloadTaskStatus(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListDownloadTaskStatus not implemented");
	protected virtual void RequestDownloadTaskListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestDownloadTaskListData not implemented");
	protected virtual void RequestVersionList() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestVersionList".Log();
	protected virtual void ListVersionList(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListVersionList not implemented");
	protected virtual void RequestVersionListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVersionListData not implemented");
	protected virtual void GetApplicationRecord(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecord not implemented");
	protected virtual void GetApplicationRecordProperty(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationRecordProperty not implemented");
	protected virtual void EnableApplicationAutoUpdate(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.EnableApplicationAutoUpdate".Log();
	protected virtual void DisableApplicationAutoUpdate(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DisableApplicationAutoUpdate".Log();
	protected virtual void TouchApplication(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.TouchApplication".Log();
	protected virtual void RequestApplicationUpdate(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestApplicationUpdate".Log();
	protected virtual void IsApplicationUpdateRequested(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsApplicationUpdateRequested not implemented");
	protected virtual void WithdrawApplicationUpdateRequest(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.WithdrawApplicationUpdateRequest".Log();
	protected virtual void ListApplicationRecordInstalledContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.ListApplicationRecordInstalledContentMeta not implemented");
	protected virtual void WithdrawCleanupAddOnContentsWithNoRightsRecommendation(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.WithdrawCleanupAddOnContentsWithNoRightsRecommendation".Log();
	protected virtual void Unknown910() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown910".Log();
	protected virtual void Unknown911() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown911".Log();
	protected virtual void Unknown912() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown912".Log();
	protected virtual void RequestVerifyApplicationDeprecated(byte[] _0, KObject _1, out KObject _2, out Nn.Ns.Detail.IProgressAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyApplicationDeprecated not implemented");
	protected virtual void CorruptApplicationForDebug(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CorruptApplicationForDebug".Log();
	protected virtual void RequestVerifyAddOnContentsRights(byte[] _0, out KObject _1, out Nn.Ns.Detail.IProgressAsyncResult _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyAddOnContentsRights not implemented");
	protected virtual void RequestVerifyApplication() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RequestVerifyApplication".Log();
	protected virtual void CorruptContentForDebug() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CorruptContentForDebug".Log();
	protected virtual void NeedsUpdateVulnerability(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsUpdateVulnerability not implemented");
	protected virtual void IsAnyApplicationEntityInstalled(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.IsAnyApplicationEntityInstalled not implemented");
	protected virtual void DeleteApplicationContentEntities(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationContentEntities".Log();
	protected virtual void CleanupUnrecordedApplicationEntity(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupUnrecordedApplicationEntity".Log();
	protected virtual void CleanupAddOnContentsWithNoRights(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CleanupAddOnContentsWithNoRights".Log();
	protected virtual void DeleteApplicationContentEntity(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.DeleteApplicationContentEntity".Log();
	protected virtual void Unknown1308() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown1308".Log();
	protected virtual void Unknown1309() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown1309".Log();
	protected virtual void PrepareShutdown() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.PrepareShutdown".Log();
	protected virtual void FormatSdCard() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.FormatSdCard".Log();
	protected virtual void NeedsSystemUpdateToFormatSdCard(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.NeedsSystemUpdateToFormatSdCard not implemented");
	protected virtual void GetLastSdCardFormatUnexpectedResult() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetLastSdCardFormatUnexpectedResult".Log();
	protected virtual void InsertSdCard() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.InsertSdCard".Log();
	protected virtual void RemoveSdCard() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.RemoveSdCard".Log();
	protected virtual void GetSystemSeedForPseudoDeviceId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSystemSeedForPseudoDeviceId not implemented");
	protected virtual void ResetSystemSeedForPseudoDeviceId() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.ResetSystemSeedForPseudoDeviceId".Log();
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.GetApplicationTicketInfo".Log();
	protected virtual void GetSystemDeliveryInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.GetSystemDeliveryInfo not implemented");
	protected virtual void SelectLatestSystemDeliveryInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationManagerInterface.SelectLatestSystemDeliveryInfo not implemented");
	protected virtual void VerifyDeliveryProtocolVersion(Span<byte> _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.VerifyDeliveryProtocolVersion".Log();
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CommitReceiveApplication".Log();
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
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.CreateDownloadTask".Log();
	protected virtual void Unknown2018() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2018".Log();
	protected virtual void Unknown2050() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2050".Log();
	protected virtual void Unknown2100() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2100".Log();
	protected virtual void Unknown2101() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2101".Log();
	protected virtual void Unknown2150() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2150".Log();
	protected virtual void Unknown2151() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2151".Log();
	protected virtual void Unknown2152() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2152".Log();
	protected virtual void Unknown2153() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2153".Log();
	protected virtual void Unknown2154() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2154".Log();
	protected virtual void Unknown2160() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2160".Log();
	protected virtual void Unknown2161() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2161".Log();
	protected virtual void Unknown2170() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2170".Log();
	protected virtual void Unknown2171() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2171".Log();
	protected virtual void Unknown2180() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2180".Log();
	protected virtual void Unknown2181() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2181".Log();
	protected virtual void Unknown2182() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2182".Log();
	protected virtual void Unknown2190() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2190".Log();
	protected virtual void Unknown2199() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2199".Log();
	protected virtual void Unknown2200() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2200".Log();
	protected virtual void Unknown2201() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2201".Log();
	protected virtual void Unknown2250() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2250".Log();
	protected virtual void Unknown2300() =>
		"Stub hit for Nn.Ns.Detail.IApplicationManagerInterface.Unknown2300".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListApplicationRecord
				ListApplicationRecord(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GenerateApplicationRecordCount
				GenerateApplicationRecordCount(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // GetApplicationRecordUpdateSystemEvent
				var _return = GetApplicationRecordUpdateSystemEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // GetApplicationViewDeprecated
				GetApplicationViewDeprecated(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // DeleteApplicationEntity
				DeleteApplicationEntity(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // DeleteApplicationCompletely
				DeleteApplicationCompletely(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // IsAnyApplicationEntityRedundant
				IsAnyApplicationEntityRedundant(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // DeleteRedundantApplicationEntity
				DeleteRedundantApplicationEntity();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // IsApplicationEntityMovable
				IsApplicationEntityMovable(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // MoveApplicationEntity
				MoveApplicationEntity(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // CalculateApplicationOccupiedSize
				CalculateApplicationOccupiedSize(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 128);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // PushApplicationRecord
				PushApplicationRecord(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // ListApplicationRecordContentMeta
				ListApplicationRecordContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // LaunchApplication
				LaunchApplication(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // GetApplicationContentPath
				GetApplicationContentPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // TerminateApplication
				TerminateApplication(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // ResolveApplicationContentPath
				ResolveApplicationContentPath(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // BeginInstallApplication
				BeginInstallApplication(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // DeleteApplicationRecord
				DeleteApplicationRecord(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // RequestApplicationUpdateInfo
				RequestApplicationUpdateInfo(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x20: { // CancelApplicationDownload
				CancelApplicationDownload(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // ResumeApplicationDownload
				ResumeApplicationDownload(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // UpdateVersionList
				UpdateVersionList(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x24: { // PushLaunchVersion
				PushLaunchVersion(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // ListRequiredVersion
				ListRequiredVersion(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x26: { // CheckApplicationLaunchVersion
				CheckApplicationLaunchVersion(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x27: { // CheckApplicationLaunchRights
				CheckApplicationLaunchRights(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // GetApplicationLogoData
				GetApplicationLogoData(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x29: { // CalculateApplicationDownloadRequiredSize
				CalculateApplicationDownloadRequiredSize(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2A: { // CleanupSdCard
				CleanupSdCard();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2B: { // CheckSdCardMountStatus
				CheckSdCardMountStatus();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2C: { // GetSdCardMountStatusChangedEvent
				var _return = GetSdCardMountStatusChangedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2D: { // GetGameCardAttachmentEvent
				var _return = GetGameCardAttachmentEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2E: { // GetGameCardAttachmentInfo
				GetGameCardAttachmentInfo(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2F: { // GetTotalSpaceSize
				GetTotalSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x30: { // GetFreeSpaceSize
				GetFreeSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x31: { // GetSdCardRemovedEvent
				var _return = GetSdCardRemovedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x34: { // GetGameCardUpdateDetectionEvent
				var _return = GetGameCardUpdateDetectionEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x35: { // DisableApplicationAutoDelete
				DisableApplicationAutoDelete(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x36: { // EnableApplicationAutoDelete
				EnableApplicationAutoDelete(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x37: { // GetApplicationDesiredLanguage
				GetApplicationDesiredLanguage(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x38: { // SetApplicationTerminateResult
				SetApplicationTerminateResult(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x39: { // ClearApplicationTerminateResult
				ClearApplicationTerminateResult(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3A: { // GetLastSdCardMountUnexpectedResult
				GetLastSdCardMountUnexpectedResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3B: { // ConvertApplicationLanguageToLanguageCode
				ConvertApplicationLanguageToLanguageCode(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // ConvertLanguageCodeToApplicationLanguage
				ConvertLanguageCodeToApplicationLanguage(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3D: { // GetBackgroundDownloadStressTaskInfo
				GetBackgroundDownloadStressTaskInfo(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3E: { // GetGameCardStopper
				var _return = GetGameCardStopper();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3F: { // IsSystemProgramInstalled
				IsSystemProgramInstalled(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x40: { // StartApplyDeltaTask
				StartApplyDeltaTask(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x41: { // GetRequestServerStopper
				var _return = GetRequestServerStopper();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x42: { // GetBackgroundApplyDeltaStressTaskInfo
				GetBackgroundApplyDeltaStressTaskInfo(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x43: { // CancelApplicationApplyDelta
				CancelApplicationApplyDelta(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x44: { // ResumeApplicationApplyDelta
				ResumeApplicationApplyDelta(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x45: { // CalculateApplicationApplyDeltaRequiredSize
				CalculateApplicationApplyDeltaRequiredSize(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x46: { // ResumeAll
				ResumeAll();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x47: { // GetStorageSize
				GetStorageSize(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x50: { // RequestDownloadApplication
				RequestDownloadApplication(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x51: { // RequestDownloadAddOnContent
				RequestDownloadAddOnContent(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x52: { // DownloadApplication
				DownloadApplication(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x53: { // CheckApplicationResumeRights
				CheckApplicationResumeRights(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x54: { // GetDynamicCommitEvent
				var _return = GetDynamicCommitEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x55: { // RequestUpdateApplication2
				RequestUpdateApplication2(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x56: { // EnableApplicationCrashReport
				EnableApplicationCrashReport(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x57: { // IsApplicationCrashReportEnabled
				IsApplicationCrashReportEnabled(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5A: { // BoostSystemMemoryResourceLimit
				BoostSystemMemoryResourceLimit(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5B: { // Unknown91
				Unknown91();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5C: { // Unknown92
				Unknown92();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5D: { // Unknown93
				Unknown93();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5E: { // LaunchApplication2
				LaunchApplication2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5F: { // Unknown95
				Unknown95();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x60: { // Unknown96
				Unknown96();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x61: { // Unknown97
				Unknown97();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x62: { // Unknown98
				Unknown98();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // ResetToFactorySettings
				ResetToFactorySettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // ResetToFactorySettingsWithoutUserSaveData
				ResetToFactorySettingsWithoutUserSaveData();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // ResetToFactorySettingsForRefurbishment
				ResetToFactorySettingsForRefurbishment();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC8: { // CalculateUserSaveDataStatistics
				CalculateUserSaveDataStatistics(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC9: { // DeleteUserSaveDataAll
				var _return = DeleteUserSaveDataAll(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xD2: { // DeleteUserSystemSaveData
				DeleteUserSystemSaveData(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD3: { // Unknown211
				Unknown211();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xDC: { // UnregisterNetworkServiceAccount
				UnregisterNetworkServiceAccount(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xDD: { // Unknown221
				Unknown221();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12C: { // GetApplicationShellEvent
				var _return = GetApplicationShellEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12D: { // PopApplicationShellEventInfo
				PopApplicationShellEventInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12E: { // LaunchLibraryApplet
				LaunchLibraryApplet(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12F: { // TerminateLibraryApplet
				TerminateLibraryApplet(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x130: { // LaunchSystemApplet
				LaunchSystemApplet(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x131: { // TerminateSystemApplet
				TerminateSystemApplet(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x132: { // LaunchOverlayApplet
				LaunchOverlayApplet(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x133: { // TerminateOverlayApplet
				TerminateOverlayApplet(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x190: { // GetApplicationControlData
				GetApplicationControlData(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x191: { // InvalidateAllApplicationControlCache
				InvalidateAllApplicationControlCache();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x192: { // RequestDownloadApplicationControlData
				RequestDownloadApplicationControlData(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x193: { // GetMaxApplicationControlCacheCount
				GetMaxApplicationControlCacheCount(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x194: { // InvalidateApplicationControlCache
				InvalidateApplicationControlCache(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x195: { // ListApplicationControlCacheEntryInfo
				ListApplicationControlCacheEntryInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x196: { // Unknown406
				Unknown406();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F6: { // RequestCheckGameCardRegistration
				RequestCheckGameCardRegistration(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F7: { // RequestGameCardRegistrationGoldPoint
				RequestGameCardRegistrationGoldPoint(im.GetBytes(8, 0x18), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F8: { // RequestRegisterGameCard
				RequestRegisterGameCard(im.GetBytes(8, 0x20), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F9: { // GetGameCardMountFailureEvent
				var _return = GetGameCardMountFailureEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1FA: { // IsGameCardInserted
				IsGameCardInserted(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1FB: { // EnsureGameCardAccess
				EnsureGameCardAccess();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1FC: { // GetLastGameCardMountFailureResult
				GetLastGameCardMountFailureResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1FD: { // ListApplicationIdOnGameCard
				ListApplicationIdOnGameCard();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x258: { // CountApplicationContentMeta
				CountApplicationContentMeta(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x259: { // ListApplicationContentMetaStatus
				ListApplicationContentMetaStatus(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25A: { // ListAvailableAddOnContent
				ListAvailableAddOnContent(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25B: { // GetOwnedApplicationContentMetaStatus
				GetOwnedApplicationContentMetaStatus(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25C: { // RegisterContentsExternalKey
				RegisterContentsExternalKey(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25D: { // ListApplicationContentMetaStatusWithRightsCheck
				ListApplicationContentMetaStatusWithRightsCheck(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25E: { // GetContentMetaStorage
				GetContentMetaStorage(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25F: { // Unknown607
				Unknown607();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BC: { // PushDownloadTaskList
				PushDownloadTaskList(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BD: { // ClearTaskStatusList
				ClearTaskStatusList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BE: { // RequestDownloadTaskList
				RequestDownloadTaskList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BF: { // RequestEnsureDownloadTask
				RequestEnsureDownloadTask(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2C0: { // ListDownloadTaskStatus
				ListDownloadTaskStatus(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C1: { // RequestDownloadTaskListData
				RequestDownloadTaskListData(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x320: { // RequestVersionList
				RequestVersionList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x321: { // ListVersionList
				ListVersionList(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x322: { // RequestVersionListData
				RequestVersionListData(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x384: { // GetApplicationRecord
				GetApplicationRecord(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x385: { // GetApplicationRecordProperty
				GetApplicationRecordProperty(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x386: { // EnableApplicationAutoUpdate
				EnableApplicationAutoUpdate(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x387: { // DisableApplicationAutoUpdate
				DisableApplicationAutoUpdate(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x388: { // TouchApplication
				TouchApplication(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x389: { // RequestApplicationUpdate
				RequestApplicationUpdate(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x38A: { // IsApplicationUpdateRequested
				IsApplicationUpdateRequested(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x38B: { // WithdrawApplicationUpdateRequest
				WithdrawApplicationUpdateRequest(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x38C: { // ListApplicationRecordInstalledContentMeta
				ListApplicationRecordInstalledContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x38D: { // WithdrawCleanupAddOnContentsWithNoRightsRecommendation
				WithdrawCleanupAddOnContentsWithNoRightsRecommendation(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x38E: { // Unknown910
				Unknown910();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x38F: { // Unknown911
				Unknown911();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x390: { // Unknown912
				Unknown912();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E8: { // RequestVerifyApplicationDeprecated
				RequestVerifyApplicationDeprecated(im.GetBytes(8, 0x10), Kernel.Get<KObject>(im.GetCopy(0)), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3E9: { // CorruptApplicationForDebug
				CorruptApplicationForDebug(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EA: { // RequestVerifyAddOnContentsRights
				RequestVerifyAddOnContentsRights(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3EB: { // RequestVerifyApplication
				RequestVerifyApplication();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EC: { // CorruptContentForDebug
				CorruptContentForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B0: { // NeedsUpdateVulnerability
				NeedsUpdateVulnerability(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x514: { // IsAnyApplicationEntityInstalled
				IsAnyApplicationEntityInstalled(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x515: { // DeleteApplicationContentEntities
				DeleteApplicationContentEntities(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x516: { // CleanupUnrecordedApplicationEntity
				CleanupUnrecordedApplicationEntity(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x517: { // CleanupAddOnContentsWithNoRights
				CleanupAddOnContentsWithNoRights(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x518: { // DeleteApplicationContentEntity
				DeleteApplicationContentEntity(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x51C: { // Unknown1308
				Unknown1308();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x51D: { // Unknown1309
				Unknown1309();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x578: { // PrepareShutdown
				PrepareShutdown();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5DC: { // FormatSdCard
				FormatSdCard();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5DD: { // NeedsSystemUpdateToFormatSdCard
				NeedsSystemUpdateToFormatSdCard(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5DE: { // GetLastSdCardFormatUnexpectedResult
				GetLastSdCardFormatUnexpectedResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5E0: { // InsertSdCard
				InsertSdCard();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5E1: { // RemoveSdCard
				RemoveSdCard();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x640: { // GetSystemSeedForPseudoDeviceId
				GetSystemSeedForPseudoDeviceId(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x641: { // ResetSystemSeedForPseudoDeviceId
				ResetSystemSeedForPseudoDeviceId();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6A4: { // ListApplicationDownloadingContentMeta
				ListApplicationDownloadingContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6A5: { // GetApplicationView
				GetApplicationView(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6A6: { // GetApplicationDownloadTaskStatus
				GetApplicationDownloadTaskStatus(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6A7: { // GetApplicationViewDownloadErrorContext
				GetApplicationViewDownloadErrorContext(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x708: { // IsNotificationSetupCompleted
				IsNotificationSetupCompleted(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x709: { // GetLastNotificationInfoCount
				GetLastNotificationInfoCount(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x70A: { // ListLastNotificationInfo
				ListLastNotificationInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x70B: { // ListNotificationTask
				ListNotificationTask(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x76C: { // IsActiveAccount
				IsActiveAccount(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x76D: { // RequestDownloadApplicationPrepurchasedRights
				RequestDownloadApplicationPrepurchasedRights(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x76E: { // GetApplicationTicketInfo
				GetApplicationTicketInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D0: { // GetSystemDeliveryInfo
				GetSystemDeliveryInfo(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D1: { // SelectLatestSystemDeliveryInfo
				SelectLatestSystemDeliveryInfo(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D2: { // VerifyDeliveryProtocolVersion
				VerifyDeliveryProtocolVersion(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D3: { // GetApplicationDeliveryInfo
				GetApplicationDeliveryInfo(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D4: { // HasAllContentsToDeliver
				HasAllContentsToDeliver(im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D5: { // CompareApplicationDeliveryInfo
				CompareApplicationDeliveryInfo(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D6: { // CanDeliverApplication
				CanDeliverApplication(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D7: { // ListContentMetaKeyToDeliverApplication
				ListContentMetaKeyToDeliverApplication(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D8: { // NeedsSystemUpdateToDeliverApplication
				NeedsSystemUpdateToDeliverApplication(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D9: { // EstimateRequiredSize
				EstimateRequiredSize(im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DA: { // RequestReceiveApplication
				RequestReceiveApplication(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x7DB: { // CommitReceiveApplication
				CommitReceiveApplication(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7DC: { // GetReceiveApplicationProgress
				GetReceiveApplicationProgress(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DD: { // RequestSendApplication
				RequestSendApplication(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x7DE: { // GetSendApplicationProgress
				GetSendApplicationProgress(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DF: { // CompareSystemDeliveryInfo
				CompareSystemDeliveryInfo(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x15, 1), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7E0: { // ListNotCommittedContentMeta
				ListNotCommittedContentMeta(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7E1: { // CreateDownloadTask
				CreateDownloadTask(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7E2: { // Unknown2018
				Unknown2018();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x802: { // Unknown2050
				Unknown2050();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x834: { // Unknown2100
				Unknown2100();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x835: { // Unknown2101
				Unknown2101();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x866: { // Unknown2150
				Unknown2150();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x867: { // Unknown2151
				Unknown2151();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x868: { // Unknown2152
				Unknown2152();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x869: { // Unknown2153
				Unknown2153();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x86A: { // Unknown2154
				Unknown2154();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x870: { // Unknown2160
				Unknown2160();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x871: { // Unknown2161
				Unknown2161();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x87A: { // Unknown2170
				Unknown2170();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x87B: { // Unknown2171
				Unknown2171();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x884: { // Unknown2180
				Unknown2180();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x885: { // Unknown2181
				Unknown2181();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x886: { // Unknown2182
				Unknown2182();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x88E: { // Unknown2190
				Unknown2190();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x897: { // Unknown2199
				Unknown2199();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x898: { // Unknown2200
				Unknown2200();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x899: { // Unknown2201
				Unknown2201();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8CA: { // Unknown2250
				Unknown2250();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8FC: { // Unknown2300
				Unknown2300();
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown1".Log();
	protected virtual void Unknown35(Span<byte> _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown35".Log();
	protected virtual void Unknown36(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown36".Log();
	protected virtual void Unknown37(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown37 not implemented");
	protected virtual void Unknown800() =>
		"Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown800".Log();
	protected virtual void Unknown801(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown801 not implemented");
	protected virtual void Unknown802(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IApplicationVersionInterface.Unknown802 not implemented");
	protected virtual void Unknown1000() =>
		"Stub hit for Nn.Ns.Detail.IApplicationVersionInterface.Unknown1000".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // Unknown35
				Unknown35(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x24: { // Unknown36
				Unknown36(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // Unknown37
				Unknown37(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x320: { // Unknown800
				Unknown800();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x321: { // Unknown801
				Unknown801(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x322: { // Unknown802
				Unknown802(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3E8: { // Unknown1000
				Unknown1000();
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IAsyncResult.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Ns.Detail.IAsyncResult.Unknown1".Log();
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncResult.Unknown2 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IAsyncValue.Unknown2".Log();
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IAsyncValue.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IContentManagementInterface.CheckSdCardMountStatus".Log();
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
				CalculateApplicationOccupiedSize(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 128);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2B: { // CheckSdCardMountStatus
				CheckSdCardMountStatus();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2F: { // GetTotalSpaceSize
				GetTotalSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x30: { // GetFreeSpaceSize
				GetFreeSpaceSize(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x258: { // CountApplicationContentMeta
				CountApplicationContentMeta(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x259: { // ListApplicationContentMetaStatus
				ListApplicationContentMetaStatus(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25D: { // ListApplicationContentMetaStatusWithRightsCheck
				ListApplicationContentMetaStatusWithRightsCheck(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25F: { // IsAnyApplicationRunning
				IsAnyApplicationRunning(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IContentManagementInterface");
		}
	}
}

public partial class IDevelopInterface : _IDevelopInterface_Base {
	public readonly string ServiceName;
	public IDevelopInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IDevelopInterface_Base : IpcInterface {
	protected virtual void LaunchProgram(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchProgram not implemented");
	protected virtual void TerminateProcess(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateProcess".Log();
	protected virtual void TerminateProgram(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateProgram".Log();
	protected virtual KObject GetShellEventHandle() =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.GetShellEventHandle not implemented");
	protected virtual void GetShellEventInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.GetShellEventInfo not implemented");
	protected virtual void TerminateApplication() =>
		"Stub hit for Nn.Ns.Detail.IDevelopInterface.TerminateApplication".Log();
	protected virtual void PrepareLaunchProgramFromHost(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.PrepareLaunchProgramFromHost not implemented");
	protected virtual void LaunchApplication(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchApplication not implemented");
	protected virtual void LaunchApplicationWithStorageId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDevelopInterface.LaunchApplicationWithStorageId not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // LaunchProgram
				LaunchProgram(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // TerminateProcess
				TerminateProcess(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // TerminateProgram
				TerminateProgram(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetShellEventHandle
				var _return = GetShellEventHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetShellEventInfo
				GetShellEventInfo(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // TerminateApplication
				TerminateApplication();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // PrepareLaunchProgramFromHost
				PrepareLaunchProgramFromHost(im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // LaunchApplication
				LaunchApplication(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // LaunchApplicationWithStorageId
				LaunchApplicationWithStorageId(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
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
		"Stub hit for Nn.Ns.Detail.IDocumentInterface.ResolveApplicationContentPath".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x15: { // GetApplicationContentPath
				GetApplicationContentPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // ResolveApplicationContentPath
				ResolveApplicationContentPath(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.ClearTaskStatusList".Log();
	protected virtual void RequestDownloadTaskList() =>
		"Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.RequestDownloadTaskList".Log();
	protected virtual void RequestEnsureDownloadTask(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDownloadTaskInterface.RequestEnsureDownloadTask not implemented");
	protected virtual void ListDownloadTaskStatus(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDownloadTaskInterface.ListDownloadTaskStatus not implemented");
	protected virtual void RequestDownloadTaskListData(out KObject _0, out Nn.Ns.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.IDownloadTaskInterface.RequestDownloadTaskListData not implemented");
	protected virtual void TryCommitCurrentApplicationDownloadTask() =>
		"Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.TryCommitCurrentApplicationDownloadTask".Log();
	protected virtual void EnableAutoCommit() =>
		"Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.EnableAutoCommit".Log();
	protected virtual void DisableAutoCommit() =>
		"Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.DisableAutoCommit".Log();
	protected virtual void TriggerDynamicCommitEvent() =>
		"Stub hit for Nn.Ns.Detail.IDownloadTaskInterface.TriggerDynamicCommitEvent".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2BD: { // ClearTaskStatusList
				ClearTaskStatusList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BE: { // RequestDownloadTaskList
				RequestDownloadTaskList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BF: { // RequestEnsureDownloadTask
				RequestEnsureDownloadTask(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2C0: { // ListDownloadTaskStatus
				ListDownloadTaskStatus(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C1: { // RequestDownloadTaskListData
				RequestDownloadTaskListData(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2C2: { // TryCommitCurrentApplicationDownloadTask
				TryCommitCurrentApplicationDownloadTask();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2C3: { // EnableAutoCommit
				EnableAutoCommit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2C4: { // DisableAutoCommit
				DisableAutoCommit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2C5: { // TriggerDynamicCommitEvent
				TriggerDynamicCommitEvent();
				om.Initialize(0, 0, 0);
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
				Unknown0(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Initialize(1, 1, 0);
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
		"Stub hit for Nn.Ns.Detail.IFactoryResetInterface.ResetToFactorySettings".Log();
	protected virtual void ResetToFactorySettingsWithoutUserSaveData() =>
		"Stub hit for Nn.Ns.Detail.IFactoryResetInterface.ResetToFactorySettingsWithoutUserSaveData".Log();
	protected virtual void ResetToFactorySettingsForRefurbishment() =>
		"Stub hit for Nn.Ns.Detail.IFactoryResetInterface.ResetToFactorySettingsForRefurbishment".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: { // ResetToFactorySettings
				ResetToFactorySettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // ResetToFactorySettingsWithoutUserSaveData
				ResetToFactorySettingsWithoutUserSaveData();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // ResetToFactorySettingsForRefurbishment
				ResetToFactorySettingsForRefurbishment();
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IProgressAsyncResult.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Ns.Detail.IProgressAsyncResult.Unknown1".Log();
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressAsyncResult.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Ns.Detail.IProgressAsyncResult.Unknown3".Log();
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressAsyncResult.Unknown4 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown2".Log();
	protected virtual void Unknown10(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IProgressMonitorForDeleteUserSaveDataAll.Unknown10 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10(out var _0);
				om.Initialize(0, 0, 40);
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

public partial class IServiceGetterInterface : _IServiceGetterInterface_Base {
	public readonly string ServiceName;
	public IServiceGetterInterface(string serviceName) => ServiceName = serviceName;
}
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
				var _return = GetECommerceInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F39: { // GetApplicationVersionInterface
				var _return = GetApplicationVersionInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3A: { // GetFactoryResetInterface
				var _return = GetFactoryResetInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3B: { // GetAccountProxyInterface
				var _return = GetAccountProxyInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3C: { // GetApplicationManagerInterface
				var _return = GetApplicationManagerInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3D: { // GetDownloadTaskInterface
				var _return = GetDownloadTaskInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3E: { // GetContentManagementInterface
				var _return = GetContentManagementInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F3F: { // GetDocumentInterface
				var _return = GetDocumentInterface();
				om.Initialize(1, 0, 0);
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
		"Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown4".Log();
	protected virtual void Unknown5(out KObject _0, out Nn.Ns.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown5 not implemented");
	protected virtual void Unknown6(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown6 not implemented");
	protected virtual void Unknown7(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown7 not implemented");
	protected virtual void Unknown8() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown8".Log();
	protected virtual void Unknown9(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown9 not implemented");
	protected virtual void Unknown10(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown10 not implemented");
	protected virtual void Unknown11(byte[] _0, KObject _1) =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown11".Log();
	protected virtual void Unknown12(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown12 not implemented");
	protected virtual void Unknown13(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown13 not implemented");
	protected virtual void Unknown14(byte[] _0, KObject _1) =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown14".Log();
	protected virtual void Unknown15(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown15 not implemented");
	protected virtual void Unknown16(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown16 not implemented");
	protected virtual void Unknown17(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown17 not implemented");
	protected virtual void Unknown18() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown18".Log();
	protected virtual void Unknown19(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown19 not implemented");
	protected virtual void Unknown20(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateControl.Unknown20 not implemented");
	protected virtual void Unknown21() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateControl.Unknown21".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(im.GetSpan<byte>(0x15, 0), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(im.GetBytes(8, 0x8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12(im.GetSpan<byte>(0x15, 0), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				Unknown13(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetBytes(8, 0x8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x11: { // Unknown17
				Unknown17(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12: { // Unknown18
				Unknown18();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // Unknown19
				Unknown19(im.GetSpan<byte>(0x15, 0), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // Unknown20
				Unknown20(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateControl");
		}
	}
}

public partial class ISystemUpdateInterface : _ISystemUpdateInterface_Base {
	public readonly string ServiceName;
	public ISystemUpdateInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemUpdateInterface_Base : IpcInterface {
	protected virtual void GetBackgroundNetworkUpdateState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetBackgroundNetworkUpdateState not implemented");
	protected virtual Nn.Ns.Detail.ISystemUpdateControl OpenSystemUpdateControl() =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.OpenSystemUpdateControl not implemented");
	protected virtual void NotifyExFatDriverRequired() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyExFatDriverRequired".Log();
	protected virtual void ClearExFatDriverStatusForDebug() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.ClearExFatDriverStatusForDebug".Log();
	protected virtual void RequestBackgroundNetworkUpdate() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.RequestBackgroundNetworkUpdate".Log();
	protected virtual void NotifyBackgroundNetworkUpdate(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyBackgroundNetworkUpdate".Log();
	protected virtual void NotifyExFatDriverDownloadedForDebug() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifyExFatDriverDownloadedForDebug".Log();
	protected virtual KObject GetSystemUpdateNotificationEventForContentDelivery() =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetSystemUpdateNotificationEventForContentDelivery not implemented");
	protected virtual void NotifySystemUpdateForContentDelivery() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.NotifySystemUpdateForContentDelivery".Log();
	protected virtual void PrepareShutdown() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.PrepareShutdown".Log();
	protected virtual void DestroySystemUpdateTask() =>
		"Stub hit for Nn.Ns.Detail.ISystemUpdateInterface.DestroySystemUpdateTask".Log();
	protected virtual void RequestSendSystemUpdate(byte[] _0, Span<byte> _1, out KObject _2, out Nn.Ns.Detail.IAsyncResult _3) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.RequestSendSystemUpdate not implemented");
	protected virtual void GetSendSystemUpdateProgress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.ISystemUpdateInterface.GetSendSystemUpdateProgress not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBackgroundNetworkUpdateState
				GetBackgroundNetworkUpdateState(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // OpenSystemUpdateControl
				var _return = OpenSystemUpdateControl();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // NotifyExFatDriverRequired
				NotifyExFatDriverRequired();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ClearExFatDriverStatusForDebug
				ClearExFatDriverStatusForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // RequestBackgroundNetworkUpdate
				RequestBackgroundNetworkUpdate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // NotifyBackgroundNetworkUpdate
				NotifyBackgroundNetworkUpdate(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // NotifyExFatDriverDownloadedForDebug
				NotifyExFatDriverDownloadedForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetSystemUpdateNotificationEventForContentDelivery
				var _return = GetSystemUpdateNotificationEventForContentDelivery();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // NotifySystemUpdateForContentDelivery
				NotifySystemUpdateForContentDelivery();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // PrepareShutdown
				PrepareShutdown();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // DestroySystemUpdateTask
				DestroySystemUpdateTask();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // RequestSendSystemUpdate
				RequestSendSystemUpdate(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x12: { // GetSendSystemUpdateProgress
				GetSendSystemUpdateProgress(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateInterface");
		}
	}
}

public partial class IVulnerabilityManagerInterface : _IVulnerabilityManagerInterface_Base {
	public readonly string ServiceName;
	public IVulnerabilityManagerInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IVulnerabilityManagerInterface_Base : IpcInterface {
	protected virtual void NeedsUpdateVulnerability(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IVulnerabilityManagerInterface.NeedsUpdateVulnerability not implemented");
	protected virtual void UpdateSafeSystemVersionForDebug(byte[] _0) =>
		"Stub hit for Nn.Ns.Detail.IVulnerabilityManagerInterface.UpdateSafeSystemVersionForDebug".Log();
	protected virtual void GetSafeSystemVersion(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ns.Detail.IVulnerabilityManagerInterface.GetSafeSystemVersion not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4B0: { // NeedsUpdateVulnerability
				NeedsUpdateVulnerability(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4B1: { // UpdateSafeSystemVersionForDebug
				UpdateSafeSystemVersionForDebug(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B2: { // GetSafeSystemVersion
				GetSafeSystemVersion(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IVulnerabilityManagerInterface");
		}
	}
}

