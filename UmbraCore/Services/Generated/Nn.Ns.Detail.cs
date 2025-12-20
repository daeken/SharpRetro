using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ns.Detail;
public partial class IAccountProxyInterface : _IAccountProxyInterface_Base;
public abstract class _IAccountProxyInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateUserAccount
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAccountProxyInterface");
		}
	}
}

public partial class IApplicationManagerInterface : _IApplicationManagerInterface_Base;
public abstract class _IApplicationManagerInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ListApplicationRecord
				break;
			case 0x1: // GenerateApplicationRecordCount
				break;
			case 0x2: // GetApplicationRecordUpdateSystemEvent
				break;
			case 0x3: // GetApplicationViewDeprecated
				break;
			case 0x4: // DeleteApplicationEntity
				break;
			case 0x5: // DeleteApplicationCompletely
				break;
			case 0x6: // IsAnyApplicationEntityRedundant
				break;
			case 0x7: // DeleteRedundantApplicationEntity
				break;
			case 0x8: // IsApplicationEntityMovable
				break;
			case 0x9: // MoveApplicationEntity
				break;
			case 0xB: // CalculateApplicationOccupiedSize
				break;
			case 0x10: // PushApplicationRecord
				break;
			case 0x11: // ListApplicationRecordContentMeta
				break;
			case 0x13: // LaunchApplication
				break;
			case 0x15: // GetApplicationContentPath
				break;
			case 0x16: // TerminateApplication
				break;
			case 0x17: // ResolveApplicationContentPath
				break;
			case 0x1A: // BeginInstallApplication
				break;
			case 0x1B: // DeleteApplicationRecord
				break;
			case 0x1E: // RequestApplicationUpdateInfo
				break;
			case 0x20: // CancelApplicationDownload
				break;
			case 0x21: // ResumeApplicationDownload
				break;
			case 0x23: // UpdateVersionList
				break;
			case 0x24: // PushLaunchVersion
				break;
			case 0x25: // ListRequiredVersion
				break;
			case 0x26: // CheckApplicationLaunchVersion
				break;
			case 0x27: // CheckApplicationLaunchRights
				break;
			case 0x28: // GetApplicationLogoData
				break;
			case 0x29: // CalculateApplicationDownloadRequiredSize
				break;
			case 0x2A: // CleanupSdCard
				break;
			case 0x2B: // CheckSdCardMountStatus
				break;
			case 0x2C: // GetSdCardMountStatusChangedEvent
				break;
			case 0x2D: // GetGameCardAttachmentEvent
				break;
			case 0x2E: // GetGameCardAttachmentInfo
				break;
			case 0x2F: // GetTotalSpaceSize
				break;
			case 0x30: // GetFreeSpaceSize
				break;
			case 0x31: // GetSdCardRemovedEvent
				break;
			case 0x34: // GetGameCardUpdateDetectionEvent
				break;
			case 0x35: // DisableApplicationAutoDelete
				break;
			case 0x36: // EnableApplicationAutoDelete
				break;
			case 0x37: // GetApplicationDesiredLanguage
				break;
			case 0x38: // SetApplicationTerminateResult
				break;
			case 0x39: // ClearApplicationTerminateResult
				break;
			case 0x3A: // GetLastSdCardMountUnexpectedResult
				break;
			case 0x3B: // ConvertApplicationLanguageToLanguageCode
				break;
			case 0x3C: // ConvertLanguageCodeToApplicationLanguage
				break;
			case 0x3D: // GetBackgroundDownloadStressTaskInfo
				break;
			case 0x3E: // GetGameCardStopper
				break;
			case 0x3F: // IsSystemProgramInstalled
				break;
			case 0x40: // StartApplyDeltaTask
				break;
			case 0x41: // GetRequestServerStopper
				break;
			case 0x42: // GetBackgroundApplyDeltaStressTaskInfo
				break;
			case 0x43: // CancelApplicationApplyDelta
				break;
			case 0x44: // ResumeApplicationApplyDelta
				break;
			case 0x45: // CalculateApplicationApplyDeltaRequiredSize
				break;
			case 0x46: // ResumeAll
				break;
			case 0x47: // GetStorageSize
				break;
			case 0x50: // RequestDownloadApplication
				break;
			case 0x51: // RequestDownloadAddOnContent
				break;
			case 0x52: // DownloadApplication
				break;
			case 0x53: // CheckApplicationResumeRights
				break;
			case 0x54: // GetDynamicCommitEvent
				break;
			case 0x55: // RequestUpdateApplication2
				break;
			case 0x56: // EnableApplicationCrashReport
				break;
			case 0x57: // IsApplicationCrashReportEnabled
				break;
			case 0x5A: // BoostSystemMemoryResourceLimit
				break;
			case 0x5B: // Unknown91
				break;
			case 0x5C: // Unknown92
				break;
			case 0x5D: // Unknown93
				break;
			case 0x5E: // LaunchApplication2
				break;
			case 0x5F: // Unknown95
				break;
			case 0x60: // Unknown96
				break;
			case 0x61: // Unknown97
				break;
			case 0x62: // Unknown98
				break;
			case 0x64: // ResetToFactorySettings
				break;
			case 0x65: // ResetToFactorySettingsWithoutUserSaveData
				break;
			case 0x66: // ResetToFactorySettingsForRefurbishment
				break;
			case 0xC8: // CalculateUserSaveDataStatistics
				break;
			case 0xC9: // DeleteUserSaveDataAll
				break;
			case 0xD2: // DeleteUserSystemSaveData
				break;
			case 0xD3: // Unknown211
				break;
			case 0xDC: // UnregisterNetworkServiceAccount
				break;
			case 0xDD: // Unknown221
				break;
			case 0x12C: // GetApplicationShellEvent
				break;
			case 0x12D: // PopApplicationShellEventInfo
				break;
			case 0x12E: // LaunchLibraryApplet
				break;
			case 0x12F: // TerminateLibraryApplet
				break;
			case 0x130: // LaunchSystemApplet
				break;
			case 0x131: // TerminateSystemApplet
				break;
			case 0x132: // LaunchOverlayApplet
				break;
			case 0x133: // TerminateOverlayApplet
				break;
			case 0x190: // GetApplicationControlData
				break;
			case 0x191: // InvalidateAllApplicationControlCache
				break;
			case 0x192: // RequestDownloadApplicationControlData
				break;
			case 0x193: // GetMaxApplicationControlCacheCount
				break;
			case 0x194: // InvalidateApplicationControlCache
				break;
			case 0x195: // ListApplicationControlCacheEntryInfo
				break;
			case 0x196: // Unknown406
				break;
			case 0x1F6: // RequestCheckGameCardRegistration
				break;
			case 0x1F7: // RequestGameCardRegistrationGoldPoint
				break;
			case 0x1F8: // RequestRegisterGameCard
				break;
			case 0x1F9: // GetGameCardMountFailureEvent
				break;
			case 0x1FA: // IsGameCardInserted
				break;
			case 0x1FB: // EnsureGameCardAccess
				break;
			case 0x1FC: // GetLastGameCardMountFailureResult
				break;
			case 0x1FD: // ListApplicationIdOnGameCard
				break;
			case 0x258: // CountApplicationContentMeta
				break;
			case 0x259: // ListApplicationContentMetaStatus
				break;
			case 0x25A: // ListAvailableAddOnContent
				break;
			case 0x25B: // GetOwnedApplicationContentMetaStatus
				break;
			case 0x25C: // RegisterContentsExternalKey
				break;
			case 0x25D: // ListApplicationContentMetaStatusWithRightsCheck
				break;
			case 0x25E: // GetContentMetaStorage
				break;
			case 0x25F: // Unknown607
				break;
			case 0x2BC: // PushDownloadTaskList
				break;
			case 0x2BD: // ClearTaskStatusList
				break;
			case 0x2BE: // RequestDownloadTaskList
				break;
			case 0x2BF: // RequestEnsureDownloadTask
				break;
			case 0x2C0: // ListDownloadTaskStatus
				break;
			case 0x2C1: // RequestDownloadTaskListData
				break;
			case 0x320: // RequestVersionList
				break;
			case 0x321: // ListVersionList
				break;
			case 0x322: // RequestVersionListData
				break;
			case 0x384: // GetApplicationRecord
				break;
			case 0x385: // GetApplicationRecordProperty
				break;
			case 0x386: // EnableApplicationAutoUpdate
				break;
			case 0x387: // DisableApplicationAutoUpdate
				break;
			case 0x388: // TouchApplication
				break;
			case 0x389: // RequestApplicationUpdate
				break;
			case 0x38A: // IsApplicationUpdateRequested
				break;
			case 0x38B: // WithdrawApplicationUpdateRequest
				break;
			case 0x38C: // ListApplicationRecordInstalledContentMeta
				break;
			case 0x38D: // WithdrawCleanupAddOnContentsWithNoRightsRecommendation
				break;
			case 0x38E: // Unknown910
				break;
			case 0x38F: // Unknown911
				break;
			case 0x390: // Unknown912
				break;
			case 0x3E8: // RequestVerifyApplicationDeprecated
				break;
			case 0x3E9: // CorruptApplicationForDebug
				break;
			case 0x3EA: // RequestVerifyAddOnContentsRights
				break;
			case 0x3EB: // RequestVerifyApplication
				break;
			case 0x3EC: // CorruptContentForDebug
				break;
			case 0x4B0: // NeedsUpdateVulnerability
				break;
			case 0x514: // IsAnyApplicationEntityInstalled
				break;
			case 0x515: // DeleteApplicationContentEntities
				break;
			case 0x516: // CleanupUnrecordedApplicationEntity
				break;
			case 0x517: // CleanupAddOnContentsWithNoRights
				break;
			case 0x518: // DeleteApplicationContentEntity
				break;
			case 0x51C: // Unknown1308
				break;
			case 0x51D: // Unknown1309
				break;
			case 0x578: // PrepareShutdown
				break;
			case 0x5DC: // FormatSdCard
				break;
			case 0x5DD: // NeedsSystemUpdateToFormatSdCard
				break;
			case 0x5DE: // GetLastSdCardFormatUnexpectedResult
				break;
			case 0x5E0: // InsertSdCard
				break;
			case 0x5E1: // RemoveSdCard
				break;
			case 0x640: // GetSystemSeedForPseudoDeviceId
				break;
			case 0x641: // ResetSystemSeedForPseudoDeviceId
				break;
			case 0x6A4: // ListApplicationDownloadingContentMeta
				break;
			case 0x6A5: // GetApplicationView
				break;
			case 0x6A6: // GetApplicationDownloadTaskStatus
				break;
			case 0x6A7: // GetApplicationViewDownloadErrorContext
				break;
			case 0x708: // IsNotificationSetupCompleted
				break;
			case 0x709: // GetLastNotificationInfoCount
				break;
			case 0x70A: // ListLastNotificationInfo
				break;
			case 0x70B: // ListNotificationTask
				break;
			case 0x76C: // IsActiveAccount
				break;
			case 0x76D: // RequestDownloadApplicationPrepurchasedRights
				break;
			case 0x76E: // GetApplicationTicketInfo
				break;
			case 0x7D0: // GetSystemDeliveryInfo
				break;
			case 0x7D1: // SelectLatestSystemDeliveryInfo
				break;
			case 0x7D2: // VerifyDeliveryProtocolVersion
				break;
			case 0x7D3: // GetApplicationDeliveryInfo
				break;
			case 0x7D4: // HasAllContentsToDeliver
				break;
			case 0x7D5: // CompareApplicationDeliveryInfo
				break;
			case 0x7D6: // CanDeliverApplication
				break;
			case 0x7D7: // ListContentMetaKeyToDeliverApplication
				break;
			case 0x7D8: // NeedsSystemUpdateToDeliverApplication
				break;
			case 0x7D9: // EstimateRequiredSize
				break;
			case 0x7DA: // RequestReceiveApplication
				break;
			case 0x7DB: // CommitReceiveApplication
				break;
			case 0x7DC: // GetReceiveApplicationProgress
				break;
			case 0x7DD: // RequestSendApplication
				break;
			case 0x7DE: // GetSendApplicationProgress
				break;
			case 0x7DF: // CompareSystemDeliveryInfo
				break;
			case 0x7E0: // ListNotCommittedContentMeta
				break;
			case 0x7E1: // CreateDownloadTask
				break;
			case 0x7E2: // Unknown2018
				break;
			case 0x802: // Unknown2050
				break;
			case 0x834: // Unknown2100
				break;
			case 0x835: // Unknown2101
				break;
			case 0x866: // Unknown2150
				break;
			case 0x867: // Unknown2151
				break;
			case 0x868: // Unknown2152
				break;
			case 0x869: // Unknown2153
				break;
			case 0x86A: // Unknown2154
				break;
			case 0x870: // Unknown2160
				break;
			case 0x871: // Unknown2161
				break;
			case 0x87A: // Unknown2170
				break;
			case 0x87B: // Unknown2171
				break;
			case 0x884: // Unknown2180
				break;
			case 0x885: // Unknown2181
				break;
			case 0x886: // Unknown2182
				break;
			case 0x88E: // Unknown2190
				break;
			case 0x897: // Unknown2199
				break;
			case 0x898: // Unknown2200
				break;
			case 0x899: // Unknown2201
				break;
			case 0x8CA: // Unknown2250
				break;
			case 0x8FC: // Unknown2300
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IApplicationManagerInterface");
		}
	}
}

public partial class IApplicationVersionInterface : _IApplicationVersionInterface_Base;
public abstract class _IApplicationVersionInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x23: // Unknown35
				break;
			case 0x24: // Unknown36
				break;
			case 0x25: // Unknown37
				break;
			case 0x320: // Unknown800
				break;
			case 0x321: // Unknown801
				break;
			case 0x322: // Unknown802
				break;
			case 0x3E8: // Unknown1000
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IApplicationVersionInterface");
		}
	}
}

public partial class IAsyncResult : _IAsyncResult_Base;
public abstract class _IAsyncResult_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAsyncResult");
		}
	}
}

public partial class IAsyncValue : _IAsyncValue_Base;
public abstract class _IAsyncValue_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IAsyncValue");
		}
	}
}

public partial class IContentManagementInterface : _IContentManagementInterface_Base;
public abstract class _IContentManagementInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xB: // CalculateApplicationOccupiedSize
				break;
			case 0x2B: // CheckSdCardMountStatus
				break;
			case 0x2F: // GetTotalSpaceSize
				break;
			case 0x30: // GetFreeSpaceSize
				break;
			case 0x258: // CountApplicationContentMeta
				break;
			case 0x259: // ListApplicationContentMetaStatus
				break;
			case 0x25D: // ListApplicationContentMetaStatusWithRightsCheck
				break;
			case 0x25F: // IsAnyApplicationRunning
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IContentManagementInterface");
		}
	}
}

public partial class IDevelopInterface : _IDevelopInterface_Base;
public abstract class _IDevelopInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // LaunchProgram
				break;
			case 0x1: // TerminateProcess
				break;
			case 0x2: // TerminateProgram
				break;
			case 0x4: // GetShellEventHandle
				break;
			case 0x5: // GetShellEventInfo
				break;
			case 0x6: // TerminateApplication
				break;
			case 0x7: // PrepareLaunchProgramFromHost
				break;
			case 0x8: // LaunchApplication
				break;
			case 0x9: // LaunchApplicationWithStorageId
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDevelopInterface");
		}
	}
}

public partial class IDocumentInterface : _IDocumentInterface_Base;
public abstract class _IDocumentInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x15: // GetApplicationContentPath
				break;
			case 0x17: // ResolveApplicationContentPath
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDocumentInterface");
		}
	}
}

public partial class IDownloadTaskInterface : _IDownloadTaskInterface_Base;
public abstract class _IDownloadTaskInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2BD: // ClearTaskStatusList
				break;
			case 0x2BE: // RequestDownloadTaskList
				break;
			case 0x2BF: // RequestEnsureDownloadTask
				break;
			case 0x2C0: // ListDownloadTaskStatus
				break;
			case 0x2C1: // RequestDownloadTaskListData
				break;
			case 0x2C2: // TryCommitCurrentApplicationDownloadTask
				break;
			case 0x2C3: // EnableAutoCommit
				break;
			case 0x2C4: // DisableAutoCommit
				break;
			case 0x2C5: // TriggerDynamicCommitEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IDownloadTaskInterface");
		}
	}
}

public partial class IECommerceInterface : _IECommerceInterface_Base;
public abstract class _IECommerceInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IECommerceInterface");
		}
	}
}

public partial class IFactoryResetInterface : _IFactoryResetInterface_Base;
public abstract class _IFactoryResetInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: // ResetToFactorySettings
				break;
			case 0x65: // ResetToFactorySettingsWithoutUserSaveData
				break;
			case 0x66: // ResetToFactorySettingsForRefurbishment
				break;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IProgressAsyncResult");
		}
	}
}

public partial class IProgressMonitorForDeleteUserSaveDataAll : _IProgressMonitorForDeleteUserSaveDataAll_Base;
public abstract class _IProgressMonitorForDeleteUserSaveDataAll_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0xA: // Unknown10
				break;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F38: // GetECommerceInterface
				break;
			case 0x1F39: // GetApplicationVersionInterface
				break;
			case 0x1F3A: // GetFactoryResetInterface
				break;
			case 0x1F3B: // GetAccountProxyInterface
				break;
			case 0x1F3C: // GetApplicationManagerInterface
				break;
			case 0x1F3D: // GetDownloadTaskInterface
				break;
			case 0x1F3E: // GetContentManagementInterface
				break;
			case 0x1F3F: // GetDocumentInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IServiceGetterInterface");
		}
	}
}

public partial class ISystemUpdateControl : _ISystemUpdateControl_Base;
public abstract class _ISystemUpdateControl_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			case 0x12: // Unknown18
				break;
			case 0x13: // Unknown19
				break;
			case 0x14: // Unknown20
				break;
			case 0x15: // Unknown21
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateControl");
		}
	}
}

public partial class ISystemUpdateInterface : _ISystemUpdateInterface_Base;
public abstract class _ISystemUpdateInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetBackgroundNetworkUpdateState
				break;
			case 0x1: // OpenSystemUpdateControl
				break;
			case 0x2: // NotifyExFatDriverRequired
				break;
			case 0x3: // ClearExFatDriverStatusForDebug
				break;
			case 0x4: // RequestBackgroundNetworkUpdate
				break;
			case 0x5: // NotifyBackgroundNetworkUpdate
				break;
			case 0x6: // NotifyExFatDriverDownloadedForDebug
				break;
			case 0x9: // GetSystemUpdateNotificationEventForContentDelivery
				break;
			case 0xA: // NotifySystemUpdateForContentDelivery
				break;
			case 0xB: // PrepareShutdown
				break;
			case 0x10: // DestroySystemUpdateTask
				break;
			case 0x11: // RequestSendSystemUpdate
				break;
			case 0x12: // GetSendSystemUpdateProgress
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.ISystemUpdateInterface");
		}
	}
}

public partial class IVulnerabilityManagerInterface : _IVulnerabilityManagerInterface_Base;
public abstract class _IVulnerabilityManagerInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4B0: // NeedsUpdateVulnerability
				break;
			case 0x4B1: // UpdateSafeSystemVersionForDebug
				break;
			case 0x4B2: // GetSafeSystemVersion
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ns.Detail.IVulnerabilityManagerInterface");
		}
	}
}

