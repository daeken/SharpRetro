using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Am.Service;
public partial class IAllSystemAppletProxiesService : _IAllSystemAppletProxiesService_Base;
public abstract class _IAllSystemAppletProxiesService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: // OpenSystemAppletProxy
				break;
			case 0xC8: // OpenLibraryAppletProxyOld
				break;
			case 0xC9: // OpenLibraryAppletProxy
				break;
			case 0x12C: // OpenOverlayAppletProxy
				break;
			case 0x15E: // OpenSystemApplicationProxy
				break;
			case 0x190: // CreateSelfLibraryAppletCreatorForDevelop
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IAllSystemAppletProxiesService");
		}
	}
}

public partial class IAppletAccessor : _IAppletAccessor_Base;
public abstract class _IAppletAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetAppletStateChangedEvent
				break;
			case 0x1: // IsCompleted
				break;
			case 0xA: // Start
				break;
			case 0x14: // RequestExit
				break;
			case 0x19: // Terminate
				break;
			case 0x1E: // GetResult
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IAppletAccessor");
		}
	}
}

public partial class IApplicationAccessor : _IApplicationAccessor_Base;
public abstract class _IApplicationAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetAppletStateChangedEvent
				break;
			case 0x1: // IsCompleted
				break;
			case 0xA: // Start
				break;
			case 0x14: // RequestExit
				break;
			case 0x19: // Terminate
				break;
			case 0x1E: // GetResult
				break;
			case 0x65: // RequestForApplicationToGetForeground
				break;
			case 0x6E: // TerminateAllLibraryApplets
				break;
			case 0x6F: // AreAnyLibraryAppletsLeft
				break;
			case 0x70: // GetCurrentLibraryApplet
				break;
			case 0x78: // GetApplicationId
				break;
			case 0x79: // PushLaunchParameter
				break;
			case 0x7A: // GetApplicationControlProperty
				break;
			case 0x7B: // GetApplicationLaunchProperty
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationAccessor");
		}
	}
}

public partial class IApplicationCreator : _IApplicationCreator_Base;
public abstract class _IApplicationCreator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateApplication
				break;
			case 0x1: // PopLaunchRequestedApplication
				break;
			case 0xA: // CreateSystemApplication
				break;
			case 0x64: // PopFloatingApplicationForDevelopment
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationCreator");
		}
	}
}

public partial class IApplicationFunctions : _IApplicationFunctions_Base;
public abstract class _IApplicationFunctions_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // PopLaunchParameter
				break;
			case 0xA: // CreateApplicationAndPushAndRequestToStart
				break;
			case 0xB: // CreateApplicationAndPushAndRequestToStartForQuest
				break;
			case 0xC: // CreateApplicationAndRequestToStart
				break;
			case 0xD: // CreateApplicationAndRequestToStartForQuest
				break;
			case 0x14: // EnsureSaveData
				break;
			case 0x15: // GetDesiredLanguage
				break;
			case 0x16: // SetTerminateResult
				break;
			case 0x17: // GetDisplayVersion
				break;
			case 0x18: // GetLaunchStorageInfoForDebug
				break;
			case 0x19: // ExtendSaveData
				break;
			case 0x1A: // GetSaveDataSize
				break;
			case 0x1E: // BeginBlockingHomeButtonShortAndLongPressed
				break;
			case 0x1F: // EndBlockingHomeButtonShortAndLongPressed
				break;
			case 0x20: // BeginBlockingHomeButton
				break;
			case 0x21: // EndBlockingHomeButton
				break;
			case 0x28: // NotifyRunning
				break;
			case 0x32: // GetPseudoDeviceId
				break;
			case 0x3C: // SetMediaPlaybackStateForApplication
				break;
			case 0x41: // IsGamePlayRecordingSupported
				break;
			case 0x42: // InitializeGamePlayRecording
				break;
			case 0x43: // SetGamePlayRecordingState
				break;
			case 0x44: // RequestFlushGamePlayingMovieForDebug
				break;
			case 0x46: // RequestToShutdown
				break;
			case 0x47: // RequestToReboot
				break;
			case 0x50: // ExitAndRequestToShowThanksMessage
				break;
			case 0x5A: // EnableApplicationCrashReport
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationFunctions");
		}
	}
}

public partial class IApplicationProxy : _IApplicationProxy_Base;
public abstract class _IApplicationProxy_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCommonStateGetter
				break;
			case 0x1: // GetSelfController
				break;
			case 0x2: // GetWindowController
				break;
			case 0x3: // GetAudioController
				break;
			case 0x4: // GetDisplayController
				break;
			case 0xA: // GetProcessWindingController
				break;
			case 0xB: // GetLibraryAppletCreator
				break;
			case 0x14: // GetApplicationFunctions
				break;
			case 0x3E8: // GetDebugFunctions
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationProxy");
		}
	}
}

public partial class IApplicationProxyService : _IApplicationProxyService_Base;
public abstract class _IApplicationProxyService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenApplicationProxy
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationProxyService");
		}
	}
}

public partial class IAudioController : _IAudioController_Base;
public abstract class _IAudioController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetExpectedMasterVolume
				break;
			case 0x1: // GetMainAppletExpectedMasterVolume
				break;
			case 0x2: // GetLibraryAppletExpectedMasterVolume
				break;
			case 0x3: // ChangeMainAppletMasterVolume
				break;
			case 0x4: // SetTransparentVolumeRate
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IAudioController");
		}
	}
}

public partial class ICommonStateGetter : _ICommonStateGetter_Base;
public abstract class _ICommonStateGetter_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetEventHandle
				break;
			case 0x1: // ReceiveMessage
				break;
			case 0x2: // GetThisAppletKind
				break;
			case 0x3: // AllowToEnterSleep
				break;
			case 0x4: // DisallowToEnterSleep
				break;
			case 0x5: // GetOperationMode
				break;
			case 0x6: // GetPerformanceMode
				break;
			case 0x7: // GetCradleStatus
				break;
			case 0x8: // GetBootMode
				break;
			case 0x9: // GetCurrentFocusState
				break;
			case 0xA: // RequestToAcquireSleepLock
				break;
			case 0xB: // ReleaseSleepLock
				break;
			case 0xC: // ReleaseSleepLockTransiently
				break;
			case 0xD: // GetAcquiredSleepLockEvent
				break;
			case 0x14: // PushToGeneralChannel
				break;
			case 0x1E: // GetHomeButtonReaderLockAccessor
				break;
			case 0x1F: // GetReaderLockAccessorEx
				break;
			case 0x28: // GetCradleFwVersion
				break;
			case 0x32: // IsVrModeEnabled
				break;
			case 0x33: // SetVrModeEnabled
				break;
			case 0x34: // SetLcdBacklighOffEnabled
				break;
			case 0x37: // IsInControllerFirmwareUpdateSection
				break;
			case 0x3C: // GetDefaultDisplayResolution
				break;
			case 0x3D: // GetDefaultDisplayResolutionChangeEvent
				break;
			case 0x3E: // GetHdcpAuthenticationState
				break;
			case 0x3F: // GetHdcpAuthenticationStateChangeEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ICommonStateGetter");
		}
	}
}

public partial class IDebugFunctions : _IDebugFunctions_Base;
public abstract class _IDebugFunctions_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // NotifyMessageToHomeMenuForDebug
				break;
			case 0x1: // OpenMainApplication
				break;
			case 0xA: // EmulateButtonEvent
				break;
			case 0x14: // InvalidateTransitionLayer
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IDebugFunctions");
		}
	}
}

public partial class IDisplayController : _IDisplayController_Base;
public abstract class _IDisplayController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetLastForegroundCaptureImage
				break;
			case 0x1: // UpdateLastForegroundCaptureImage
				break;
			case 0x2: // GetLastApplicationCaptureImage
				break;
			case 0x3: // GetCallerAppletCaptureImage
				break;
			case 0x4: // UpdateCallerAppletCaptureImage
				break;
			case 0x5: // GetLastForegroundCaptureImageEx
				break;
			case 0x6: // GetLastApplicationCaptureImageEx
				break;
			case 0x7: // GetCallerAppletCaptureImageEx
				break;
			case 0x8: // TakeScreenShotOfOwnLayer
				break;
			case 0xA: // AcquireLastApplicationCaptureBuffer
				break;
			case 0xB: // ReleaseLastApplicationCaptureBuffer
				break;
			case 0xC: // AcquireLastForegroundCaptureBuffer
				break;
			case 0xD: // ReleaseLastForegroundCaptureBuffer
				break;
			case 0xE: // AcquireCallerAppletCaptureBuffer
				break;
			case 0xF: // ReleaseCallerAppletCaptureBuffer
				break;
			case 0x10: // AcquireLastApplicationCaptureBufferEx
				break;
			case 0x11: // AcquireLastForegroundCaptureBufferEx
				break;
			case 0x12: // AcquireCallerAppletCaptureBufferEx
				break;
			case 0x14: // ClearCaptureBuffer
				break;
			case 0x15: // ClearAppletTransitionBuffer
				break;
			case 0x16: // AcquireLastApplicationCaptureSharedBuffer
				break;
			case 0x17: // ReleaseLastApplicationCaptureSharedBuffer
				break;
			case 0x18: // AcquireLastForegroundCaptureSharedBuffer
				break;
			case 0x19: // ReleaseLastForegroundCaptureSharedBuffer
				break;
			case 0x1A: // AcquireCallerAppletCaptureSharedBuffer
				break;
			case 0x1B: // ReleaseCallerAppletCaptureSharedBuffer
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IDisplayController");
		}
	}
}

public partial class IGlobalStateController : _IGlobalStateController_Base;
public abstract class _IGlobalStateController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestToEnterSleep
				break;
			case 0x1: // EnterSleep
				break;
			case 0x2: // StartSleepSequence
				break;
			case 0x3: // StartShutdownSequence
				break;
			case 0x4: // StartRebootSequence
				break;
			case 0xA: // LoadAndApplyIdlePolicySettings
				break;
			case 0xB: // NotifyCecSettingsChanged
				break;
			case 0xC: // SetDefaultHomeButtonLongPressTime
				break;
			case 0xD: // UpdateDefaultDisplayResolution
				break;
			case 0xE: // ShouldSleepOnBoot
				break;
			case 0xF: // GetHdcpAuthenticationFailedEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IGlobalStateController");
		}
	}
}

public partial class IHomeMenuFunctions : _IHomeMenuFunctions_Base;
public abstract class _IHomeMenuFunctions_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: // RequestToGetForeground
				break;
			case 0xB: // LockForeground
				break;
			case 0xC: // UnlockForeground
				break;
			case 0x14: // PopFromGeneralChannel
				break;
			case 0x15: // GetPopFromGeneralChannelEvent
				break;
			case 0x1E: // GetHomeButtonWriterLockAccessor
				break;
			case 0x1F: // GetWriterLockAccessorEx
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IHomeMenuFunctions");
		}
	}
}

public partial class ILibraryAppletAccessor : _ILibraryAppletAccessor_Base;
public abstract class _ILibraryAppletAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetAppletStateChangedEvent
				break;
			case 0x1: // IsCompleted
				break;
			case 0xA: // Start
				break;
			case 0x14: // RequestExit
				break;
			case 0x19: // Terminate
				break;
			case 0x1E: // GetResult
				break;
			case 0x32: // SetOutOfFocusApplicationSuspendingEnabled
				break;
			case 0x64: // PushInData
				break;
			case 0x65: // PopOutData
				break;
			case 0x66: // PushExtraStorage
				break;
			case 0x67: // PushInteractiveInData
				break;
			case 0x68: // PopInteractiveOutData
				break;
			case 0x69: // GetPopOutDataEvent
				break;
			case 0x6A: // GetPopInteractiveOutDataEvent
				break;
			case 0x6E: // NeedsToExitProcess
				break;
			case 0x78: // GetLibraryAppletInfo
				break;
			case 0x96: // RequestForAppletToGetForeground
				break;
			case 0xA0: // GetIndirectLayerConsumerHandle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletAccessor");
		}
	}
}

public partial class ILibraryAppletCreator : _ILibraryAppletCreator_Base;
public abstract class _ILibraryAppletCreator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateLibraryApplet
				break;
			case 0x1: // TerminateAllLibraryApplets
				break;
			case 0x2: // AreAnyLibraryAppletsLeft
				break;
			case 0xA: // CreateStorage
				break;
			case 0xB: // CreateTransferMemoryStorage
				break;
			case 0xC: // CreateHandleStorage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletCreator");
		}
	}
}

public partial class ILibraryAppletProxy : _ILibraryAppletProxy_Base;
public abstract class _ILibraryAppletProxy_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCommonStateGetter
				break;
			case 0x1: // GetSelfController
				break;
			case 0x2: // GetWindowController
				break;
			case 0x3: // GetAudioController
				break;
			case 0x4: // GetDisplayController
				break;
			case 0xA: // GetProcessWindingController
				break;
			case 0xB: // GetLibraryAppletCreator
				break;
			case 0x14: // OpenLibraryAppletSelfAccessor
				break;
			case 0x3E8: // GetDebugFunctions
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletProxy");
		}
	}
}

public partial class ILibraryAppletSelfAccessor : _ILibraryAppletSelfAccessor_Base;
public abstract class _ILibraryAppletSelfAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PopInData
				break;
			case 0x1: // PushOutData
				break;
			case 0x2: // PopInteractiveInData
				break;
			case 0x3: // PushInteractiveOutData
				break;
			case 0x5: // GetPopInDataEvent
				break;
			case 0x6: // GetPopInteractiveInDataEvent
				break;
			case 0xA: // ExitProcessAndReturn
				break;
			case 0xB: // GetLibraryAppletInfo
				break;
			case 0xC: // GetMainAppletIdentityInfo
				break;
			case 0xD: // CanUseApplicationCore
				break;
			case 0xE: // GetCallerAppletIdentityInfo
				break;
			case 0xF: // GetMainAppletApplicationControlProperty
				break;
			case 0x10: // GetMainAppletStorageId
				break;
			case 0x11: // GetCallerAppletIdentityInfoStack
				break;
			case 0x12: // GetNextReturnDestinationAppletIdentityInfo
				break;
			case 0x13: // GetDesirableKeyboardLayout
				break;
			case 0x14: // PopExtraStorage
				break;
			case 0x19: // GetPopExtraStorageEvent
				break;
			case 0x1E: // UnpopInData
				break;
			case 0x1F: // UnpopExtraStorage
				break;
			case 0x28: // GetIndirectLayerProducerHandle
				break;
			case 0x32: // ReportVisibleError
				break;
			case 0x33: // ReportVisibleErrorWithErrorContext
				break;
			case 0x3C: // GetMainAppletApplicationDesiredLanguage
				break;
			case 0x64: // CreateGameMovieTrimmer
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletSelfAccessor");
		}
	}
}

public partial class ILockAccessor : _ILockAccessor_Base;
public abstract class _ILockAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // TryLock
				break;
			case 0x2: // Unlock
				break;
			case 0x3: // GetEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILockAccessor");
		}
	}
}

public partial class IOverlayAppletProxy : _IOverlayAppletProxy_Base;
public abstract class _IOverlayAppletProxy_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCommonStateGetter
				break;
			case 0x1: // GetSelfController
				break;
			case 0x2: // GetWindowController
				break;
			case 0x3: // GetAudioController
				break;
			case 0x4: // GetDisplayController
				break;
			case 0xA: // GetProcessWindingController
				break;
			case 0xB: // GetLibraryAppletCreator
				break;
			case 0x14: // GetOverlayFunctions
				break;
			case 0x3E8: // GetDebugFunctions
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IOverlayAppletProxy");
		}
	}
}

public partial class IOverlayFunctions : _IOverlayFunctions_Base;
public abstract class _IOverlayFunctions_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BeginToWatchShortHomeButtonMessage
				break;
			case 0x1: // EndToWatchShortHomeButtonMessage
				break;
			case 0x2: // GetApplicationIdForLogo
				break;
			case 0x3: // SetGpuTimeSliceBoost
				break;
			case 0x4: // SetAutoSleepTimeAndDimmingTimeEnabled
				break;
			case 0x5: // TerminateApplicationAndSetReason
				break;
			case 0x6: // SetScreenShotPermissionGlobally
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IOverlayFunctions");
		}
	}
}

public partial class IProcessWindingController : _IProcessWindingController_Base;
public abstract class _IProcessWindingController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetLaunchReason
				break;
			case 0xB: // OpenCallingLibraryApplet
				break;
			case 0x15: // PushContext
				break;
			case 0x16: // PopContext
				break;
			case 0x17: // CancelWindingReservation
				break;
			case 0x1E: // WindAndDoReserved
				break;
			case 0x28: // ReserveToStartAndWaitAndUnwindThis
				break;
			case 0x29: // ReserveToStartAndWait
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IProcessWindingController");
		}
	}
}

public partial class ISelfController : _ISelfController_Base;
public abstract class _ISelfController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Exit
				break;
			case 0x1: // LockExit
				break;
			case 0x2: // UnlockExit
				break;
			case 0x3: // EnterFatalSection
				break;
			case 0x4: // LeaveFatalSection
				break;
			case 0x9: // GetLibraryAppletLaunchableEvent
				break;
			case 0xA: // SetScreenShotPermission
				break;
			case 0xB: // SetOperationModeChangedNotification
				break;
			case 0xC: // SetPerformanceModeChangedNotification
				break;
			case 0xD: // SetFocusHandlingMode
				break;
			case 0xE: // SetRestartMessageEnabled
				break;
			case 0xF: // SetScreenShotAppletIdentityInfo
				break;
			case 0x10: // SetOutOfFocusSuspendingEnabled
				break;
			case 0x11: // SetControllerFirmwareUpdateSection
				break;
			case 0x12: // SetRequiresCaptureButtonShortPressedMessage
				break;
			case 0x13: // SetScreenShotImageOrientation
				break;
			case 0x14: // SetDesirableKeyboardLayout
				break;
			case 0x28: // CreateManagedDisplayLayer
				break;
			case 0x29: // IsSystemBufferSharingEnabled
				break;
			case 0x2A: // GetSystemSharedLayerHandle
				break;
			case 0x32: // SetHandlesRequestToDisplay
				break;
			case 0x33: // ApproveToDisplay
				break;
			case 0x3C: // OverrideAutoSleepTimeAndDimmingTime
				break;
			case 0x3D: // SetMediaPlaybackState
				break;
			case 0x3E: // SetIdleTimeDetectionExtension
				break;
			case 0x3F: // GetIdleTimeDetectionExtension
				break;
			case 0x40: // SetInputDetectionSourceSet
				break;
			case 0x41: // ReportUserIsActive
				break;
			case 0x42: // GetCurrentIlluminance
				break;
			case 0x43: // IsIlluminanceAvailable
				break;
			case 0x46: // ReportMultimediaError
				break;
			case 0x50: // SetWirelessPriorityMode
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ISelfController");
		}
	}
}

public partial class IStorage : _IStorage_Base;
public abstract class _IStorage_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IStorage");
		}
	}
}

public partial class IStorageAccessor : _IStorageAccessor_Base;
public abstract class _IStorageAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSize
				break;
			case 0xA: // Write
				break;
			case 0xB: // Read
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IStorageAccessor");
		}
	}
}

public partial class ISystemAppletProxy : _ISystemAppletProxy_Base;
public abstract class _ISystemAppletProxy_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCommonStateGetter
				break;
			case 0x1: // GetSelfController
				break;
			case 0x2: // GetWindowController
				break;
			case 0x3: // GetAudioController
				break;
			case 0x4: // GetDisplayController
				break;
			case 0xA: // GetProcessWindingController
				break;
			case 0xB: // GetLibraryAppletCreator
				break;
			case 0x14: // GetHomeMenuFunctions
				break;
			case 0x15: // GetGlobalStateController
				break;
			case 0x16: // GetApplicationCreator
				break;
			case 0x3E8: // GetDebugFunctions
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ISystemAppletProxy");
		}
	}
}

public partial class ITransferStorageAccessor : _ITransferStorageAccessor_Base;
public abstract class _ITransferStorageAccessor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSize
				break;
			case 0x1: // GetHandle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ITransferStorageAccessor");
		}
	}
}

public partial class IWindow : _IWindow_Base;
public abstract class _IWindow_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IWindow");
		}
	}
}

public partial class IWindowController : _IWindowController_Base;
public abstract class _IWindowController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateWindow
				break;
			case 0x1: // GetAppletResourceUserId
				break;
			case 0xA: // AcquireForegroundRights
				break;
			case 0xB: // ReleaseForegroundRights
				break;
			case 0xC: // RejectToChangeIntoBackground
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IWindowController");
		}
	}
}

