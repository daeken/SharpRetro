using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pctl.Detail.Ipc;
public partial class IParentalControlService : _IParentalControlService_Base;
public abstract class _IParentalControlService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // Initialize
				break;
			case 0x3E9: // CheckFreeCommunicationPermission
				break;
			case 0x3EA: // ConfirmLaunchApplicationPermission
				break;
			case 0x3EB: // ConfirmResumeApplicationPermission
				break;
			case 0x3EC: // ConfirmSnsPostPermission
				break;
			case 0x3ED: // ConfirmSystemSettingsPermission
				break;
			case 0x3EE: // IsRestrictionTemporaryUnlocked
				break;
			case 0x3EF: // RevertRestrictionTemporaryUnlocked
				break;
			case 0x3F0: // EnterRestrictedSystemSettings
				break;
			case 0x3F1: // LeaveRestrictedSystemSettings
				break;
			case 0x3F2: // IsRestrictedSystemSettingsEntered
				break;
			case 0x3F3: // RevertRestrictedSystemSettingsEntered
				break;
			case 0x3F4: // GetRestrictedFeatures
				break;
			case 0x3F5: // ConfirmStereoVisionPermission
				break;
			case 0x3F6: // ConfirmPlayableApplicationVideoOld
				break;
			case 0x3F7: // ConfirmPlayableApplicationVideo
				break;
			case 0x407: // IsRestrictionEnabled
				break;
			case 0x408: // GetSafetyLevel
				break;
			case 0x409: // SetSafetyLevel
				break;
			case 0x40A: // GetSafetyLevelSettings
				break;
			case 0x40B: // GetCurrentSettings
				break;
			case 0x40C: // SetCustomSafetyLevelSettings
				break;
			case 0x40D: // GetDefaultRatingOrganization
				break;
			case 0x40E: // SetDefaultRatingOrganization
				break;
			case 0x40F: // GetFreeCommunicationApplicationListCount
				break;
			case 0x412: // AddToFreeCommunicationApplicationList
				break;
			case 0x413: // DeleteSettings
				break;
			case 0x414: // GetFreeCommunicationApplicationList
				break;
			case 0x415: // UpdateFreeCommunicationApplicationList
				break;
			case 0x416: // DisableFeaturesForReset
				break;
			case 0x417: // NotifyApplicationDownloadStarted
				break;
			case 0x425: // ConfirmStereoVisionRestrictionConfigurable
				break;
			case 0x426: // GetStereoVisionRestriction
				break;
			case 0x427: // SetStereoVisionRestriction
				break;
			case 0x428: // ResetConfirmedStereoVisionPermission
				break;
			case 0x429: // IsStereoVisionPermitted
				break;
			case 0x4B1: // UnlockRestrictionTemporarily
				break;
			case 0x4B2: // UnlockSystemSettingsRestriction
				break;
			case 0x4B3: // SetPinCode
				break;
			case 0x4B4: // GenerateInquiryCode
				break;
			case 0x4B5: // CheckMasterKey
				break;
			case 0x4B6: // GetPinCodeLength
				break;
			case 0x4B7: // GetPinCodeChangedEvent
				break;
			case 0x4B8: // GetPinCode
				break;
			case 0x57B: // IsPairingActive
				break;
			case 0x57E: // GetSettingsLastUpdated
				break;
			case 0x583: // GetPairingAccountInfo
				break;
			case 0x58D: // GetAccountNickname
				break;
			case 0x590: // GetAccountState
				break;
			case 0x598: // GetSynchronizationEvent
				break;
			case 0x5AB: // StartPlayTimer
				break;
			case 0x5AC: // StopPlayTimer
				break;
			case 0x5AD: // IsPlayTimerEnabled
				break;
			case 0x5AE: // GetPlayTimerRemainingTime
				break;
			case 0x5AF: // IsRestrictedByPlayTimer
				break;
			case 0x5B0: // GetPlayTimerSettings
				break;
			case 0x5B1: // GetPlayTimerEventToRequestSuspension
				break;
			case 0x5B2: // IsPlayTimerAlarmDisabled
				break;
			case 0x5BF: // NotifyWrongPinCodeInputManyTimes
				break;
			case 0x5C0: // CancelNetworkRequest
				break;
			case 0x5C1: // GetUnlinkedEvent
				break;
			case 0x5C2: // ClearUnlinkedEvent
				break;
			case 0x641: // DisableAllFeatures
				break;
			case 0x642: // PostEnableAllFeatures
				break;
			case 0x643: // IsAllFeaturesDisabled
				break;
			case 0x76D: // DeleteFromFreeCommunicationApplicationListForDebug
				break;
			case 0x76E: // ClearFreeCommunicationApplicationListForDebug
				break;
			case 0x76F: // GetExemptApplicationListCountForDebug
				break;
			case 0x770: // GetExemptApplicationListForDebug
				break;
			case 0x771: // UpdateExemptApplicationListForDebug
				break;
			case 0x772: // AddToExemptApplicationListForDebug
				break;
			case 0x773: // DeleteFromExemptApplicationListForDebug
				break;
			case 0x774: // ClearExemptApplicationListForDebug
				break;
			case 0x795: // DeletePairing
				break;
			case 0x79F: // SetPlayTimerSettingsForDebug
				break;
			case 0x7A0: // GetPlayTimerSpentTimeForTest
				break;
			case 0x7A1: // SetPlayTimerAlarmDisabledForDebug
				break;
			case 0x7D1: // RequestPairingAsync
				break;
			case 0x7D2: // FinishRequestPairing
				break;
			case 0x7D3: // AuthorizePairingAsync
				break;
			case 0x7D4: // FinishAuthorizePairing
				break;
			case 0x7D5: // RetrievePairingInfoAsync
				break;
			case 0x7D6: // FinishRetrievePairingInfo
				break;
			case 0x7D7: // UnlinkPairingAsync
				break;
			case 0x7D8: // FinishUnlinkPairing
				break;
			case 0x7D9: // GetAccountMiiImageAsync
				break;
			case 0x7DA: // FinishGetAccountMiiImage
				break;
			case 0x7DB: // GetAccountMiiImageContentTypeAsync
				break;
			case 0x7DC: // FinishGetAccountMiiImageContentType
				break;
			case 0x7DD: // SynchronizeParentalControlSettingsAsync
				break;
			case 0x7DE: // FinishSynchronizeParentalControlSettings
				break;
			case 0x7DF: // FinishSynchronizeParentalControlSettingsWithLastUpdated
				break;
			case 0x7E0: // RequestUpdateExemptionListAsync
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlService");
		}
	}
}

public partial class IParentalControlServiceFactory : _IParentalControlServiceFactory_Base;
public abstract class _IParentalControlServiceFactory_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateService
				break;
			case 0x1: // CreateServiceWithoutInitialize
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory");
		}
	}
}

