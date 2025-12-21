using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pctl.Detail.Ipc;
public partial class IParentalControlService : _IParentalControlService_Base;
public abstract class _IParentalControlService_Base : IpcInterface {
	protected virtual void Initialize() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.Initialize");
	protected virtual void CheckFreeCommunicationPermission() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.CheckFreeCommunicationPermission");
	protected virtual void ConfirmLaunchApplicationPermission(byte _0, ulong _1, Span<sbyte> _2) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmLaunchApplicationPermission");
	protected virtual void ConfirmResumeApplicationPermission(byte _0, ulong _1, Span<sbyte> _2) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmResumeApplicationPermission");
	protected virtual void ConfirmSnsPostPermission() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmSnsPostPermission");
	protected virtual void ConfirmSystemSettingsPermission() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmSystemSettingsPermission");
	protected virtual byte IsRestrictionTemporaryUnlocked() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsRestrictionTemporaryUnlocked not implemented");
	protected virtual void RevertRestrictionTemporaryUnlocked() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.RevertRestrictionTemporaryUnlocked");
	protected virtual void EnterRestrictedSystemSettings() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.EnterRestrictedSystemSettings");
	protected virtual void LeaveRestrictedSystemSettings() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.LeaveRestrictedSystemSettings");
	protected virtual byte IsRestrictedSystemSettingsEntered() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsRestrictedSystemSettingsEntered not implemented");
	protected virtual void RevertRestrictedSystemSettingsEntered() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.RevertRestrictedSystemSettingsEntered");
	protected virtual uint GetRestrictedFeatures() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetRestrictedFeatures not implemented");
	protected virtual void ConfirmStereoVisionPermission() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmStereoVisionPermission");
	protected virtual void ConfirmPlayableApplicationVideoOld() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmPlayableApplicationVideoOld");
	protected virtual void ConfirmPlayableApplicationVideo() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmPlayableApplicationVideo");
	protected virtual byte IsRestrictionEnabled() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsRestrictionEnabled not implemented");
	protected virtual uint GetSafetyLevel() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetSafetyLevel not implemented");
	protected virtual void SetSafetyLevel(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetSafetyLevel");
	protected virtual void GetSafetyLevelSettings(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetSafetyLevelSettings not implemented");
	protected virtual void GetCurrentSettings(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetCurrentSettings not implemented");
	protected virtual void SetCustomSafetyLevelSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetCustomSafetyLevelSettings");
	protected virtual uint GetDefaultRatingOrganization() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetDefaultRatingOrganization not implemented");
	protected virtual void SetDefaultRatingOrganization(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetDefaultRatingOrganization");
	protected virtual uint GetFreeCommunicationApplicationListCount() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetFreeCommunicationApplicationListCount not implemented");
	protected virtual void AddToFreeCommunicationApplicationList(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.AddToFreeCommunicationApplicationList");
	protected virtual void DeleteSettings() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.DeleteSettings");
	protected virtual void GetFreeCommunicationApplicationList(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetFreeCommunicationApplicationList not implemented");
	protected virtual void UpdateFreeCommunicationApplicationList(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.UpdateFreeCommunicationApplicationList");
	protected virtual void DisableFeaturesForReset() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.DisableFeaturesForReset");
	protected virtual void NotifyApplicationDownloadStarted(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.NotifyApplicationDownloadStarted");
	protected virtual void ConfirmStereoVisionRestrictionConfigurable() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ConfirmStereoVisionRestrictionConfigurable");
	protected virtual byte GetStereoVisionRestriction() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetStereoVisionRestriction not implemented");
	protected virtual void SetStereoVisionRestriction(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetStereoVisionRestriction");
	protected virtual void ResetConfirmedStereoVisionPermission() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ResetConfirmedStereoVisionPermission");
	protected virtual void IsStereoVisionPermitted() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.IsStereoVisionPermitted");
	protected virtual void UnlockRestrictionTemporarily(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.UnlockRestrictionTemporarily");
	protected virtual void UnlockSystemSettingsRestriction(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.UnlockSystemSettingsRestriction");
	protected virtual void SetPinCode(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetPinCode");
	protected virtual void GenerateInquiryCode(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GenerateInquiryCode not implemented");
	protected virtual byte CheckMasterKey(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.CheckMasterKey not implemented");
	protected virtual uint GetPinCodeLength() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPinCodeLength not implemented");
	protected virtual KObject GetPinCodeChangedEvent() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPinCodeChangedEvent not implemented");
	protected virtual void GetPinCode(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPinCode not implemented");
	protected virtual byte IsPairingActive() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsPairingActive not implemented");
	protected virtual ulong GetSettingsLastUpdated() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetSettingsLastUpdated not implemented");
	protected virtual void GetPairingAccountInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPairingAccountInfo not implemented");
	protected virtual void GetAccountNickname(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountNickname not implemented");
	protected virtual uint GetAccountState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountState not implemented");
	protected virtual KObject GetSynchronizationEvent() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetSynchronizationEvent not implemented");
	protected virtual void StartPlayTimer() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.StartPlayTimer");
	protected virtual void StopPlayTimer() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.StopPlayTimer");
	protected virtual byte IsPlayTimerEnabled() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsPlayTimerEnabled not implemented");
	protected virtual ulong GetPlayTimerRemainingTime() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPlayTimerRemainingTime not implemented");
	protected virtual byte IsRestrictedByPlayTimer() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsRestrictedByPlayTimer not implemented");
	protected virtual void GetPlayTimerSettings(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPlayTimerSettings not implemented");
	protected virtual KObject GetPlayTimerEventToRequestSuspension() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPlayTimerEventToRequestSuspension not implemented");
	protected virtual byte IsPlayTimerAlarmDisabled() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsPlayTimerAlarmDisabled not implemented");
	protected virtual void NotifyWrongPinCodeInputManyTimes() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.NotifyWrongPinCodeInputManyTimes");
	protected virtual void CancelNetworkRequest() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.CancelNetworkRequest");
	protected virtual KObject GetUnlinkedEvent() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetUnlinkedEvent not implemented");
	protected virtual void ClearUnlinkedEvent() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ClearUnlinkedEvent");
	protected virtual byte DisableAllFeatures() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.DisableAllFeatures not implemented");
	protected virtual byte PostEnableAllFeatures() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.PostEnableAllFeatures not implemented");
	protected virtual void IsAllFeaturesDisabled(out byte _0, out byte _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.IsAllFeaturesDisabled not implemented");
	protected virtual void DeleteFromFreeCommunicationApplicationListForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.DeleteFromFreeCommunicationApplicationListForDebug");
	protected virtual void ClearFreeCommunicationApplicationListForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ClearFreeCommunicationApplicationListForDebug");
	protected virtual void GetExemptApplicationListCountForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.GetExemptApplicationListCountForDebug");
	protected virtual void GetExemptApplicationListForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.GetExemptApplicationListForDebug");
	protected virtual void UpdateExemptApplicationListForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.UpdateExemptApplicationListForDebug");
	protected virtual void AddToExemptApplicationListForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.AddToExemptApplicationListForDebug");
	protected virtual void DeleteFromExemptApplicationListForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.DeleteFromExemptApplicationListForDebug");
	protected virtual void ClearExemptApplicationListForDebug() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.ClearExemptApplicationListForDebug");
	protected virtual void DeletePairing() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.DeletePairing");
	protected virtual void SetPlayTimerSettingsForDebug(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetPlayTimerSettingsForDebug");
	protected virtual ulong GetPlayTimerSpentTimeForTest() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPlayTimerSpentTimeForTest not implemented");
	protected virtual void SetPlayTimerAlarmDisabledForDebug(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetPlayTimerAlarmDisabledForDebug");
	protected virtual void RequestPairingAsync(Span<byte> _0, Span<byte> _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.RequestPairingAsync not implemented");
	protected virtual void FinishRequestPairing(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishRequestPairing not implemented");
	protected virtual void AuthorizePairingAsync(Span<byte> _0, Span<byte> _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.AuthorizePairingAsync not implemented");
	protected virtual void FinishAuthorizePairing(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishAuthorizePairing not implemented");
	protected virtual void RetrievePairingInfoAsync(Span<byte> _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.RetrievePairingInfoAsync not implemented");
	protected virtual void FinishRetrievePairingInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishRetrievePairingInfo not implemented");
	protected virtual void UnlinkPairingAsync(byte _0, Span<byte> _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.UnlinkPairingAsync not implemented");
	protected virtual void FinishUnlinkPairing(byte _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.FinishUnlinkPairing");
	protected virtual void GetAccountMiiImageAsync(Span<byte> _0, Span<byte> _1, out uint _2, out KObject _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountMiiImageAsync not implemented");
	protected virtual void FinishGetAccountMiiImage(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishGetAccountMiiImage not implemented");
	protected virtual void GetAccountMiiImageContentTypeAsync(Span<byte> _0, Span<byte> _1, out uint _2, out KObject _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountMiiImageContentTypeAsync not implemented");
	protected virtual void FinishGetAccountMiiImageContentType(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishGetAccountMiiImageContentType not implemented");
	protected virtual void SynchronizeParentalControlSettingsAsync(Span<byte> _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.SynchronizeParentalControlSettingsAsync not implemented");
	protected virtual void FinishSynchronizeParentalControlSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.FinishSynchronizeParentalControlSettings");
	protected virtual ulong FinishSynchronizeParentalControlSettingsWithLastUpdated(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishSynchronizeParentalControlSettingsWithLastUpdated not implemented");
	protected virtual void RequestUpdateExemptionListAsync() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.RequestUpdateExemptionListAsync");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Initialize
				break;
			}
			case 0x3E9: { // CheckFreeCommunicationPermission
				break;
			}
			case 0x3EA: { // ConfirmLaunchApplicationPermission
				break;
			}
			case 0x3EB: { // ConfirmResumeApplicationPermission
				break;
			}
			case 0x3EC: { // ConfirmSnsPostPermission
				break;
			}
			case 0x3ED: { // ConfirmSystemSettingsPermission
				break;
			}
			case 0x3EE: { // IsRestrictionTemporaryUnlocked
				break;
			}
			case 0x3EF: { // RevertRestrictionTemporaryUnlocked
				break;
			}
			case 0x3F0: { // EnterRestrictedSystemSettings
				break;
			}
			case 0x3F1: { // LeaveRestrictedSystemSettings
				break;
			}
			case 0x3F2: { // IsRestrictedSystemSettingsEntered
				break;
			}
			case 0x3F3: { // RevertRestrictedSystemSettingsEntered
				break;
			}
			case 0x3F4: { // GetRestrictedFeatures
				break;
			}
			case 0x3F5: { // ConfirmStereoVisionPermission
				break;
			}
			case 0x3F6: { // ConfirmPlayableApplicationVideoOld
				break;
			}
			case 0x3F7: { // ConfirmPlayableApplicationVideo
				break;
			}
			case 0x407: { // IsRestrictionEnabled
				break;
			}
			case 0x408: { // GetSafetyLevel
				break;
			}
			case 0x409: { // SetSafetyLevel
				break;
			}
			case 0x40A: { // GetSafetyLevelSettings
				break;
			}
			case 0x40B: { // GetCurrentSettings
				break;
			}
			case 0x40C: { // SetCustomSafetyLevelSettings
				break;
			}
			case 0x40D: { // GetDefaultRatingOrganization
				break;
			}
			case 0x40E: { // SetDefaultRatingOrganization
				break;
			}
			case 0x40F: { // GetFreeCommunicationApplicationListCount
				break;
			}
			case 0x412: { // AddToFreeCommunicationApplicationList
				break;
			}
			case 0x413: { // DeleteSettings
				break;
			}
			case 0x414: { // GetFreeCommunicationApplicationList
				break;
			}
			case 0x415: { // UpdateFreeCommunicationApplicationList
				break;
			}
			case 0x416: { // DisableFeaturesForReset
				break;
			}
			case 0x417: { // NotifyApplicationDownloadStarted
				break;
			}
			case 0x425: { // ConfirmStereoVisionRestrictionConfigurable
				break;
			}
			case 0x426: { // GetStereoVisionRestriction
				break;
			}
			case 0x427: { // SetStereoVisionRestriction
				break;
			}
			case 0x428: { // ResetConfirmedStereoVisionPermission
				break;
			}
			case 0x429: { // IsStereoVisionPermitted
				break;
			}
			case 0x4B1: { // UnlockRestrictionTemporarily
				break;
			}
			case 0x4B2: { // UnlockSystemSettingsRestriction
				break;
			}
			case 0x4B3: { // SetPinCode
				break;
			}
			case 0x4B4: { // GenerateInquiryCode
				break;
			}
			case 0x4B5: { // CheckMasterKey
				break;
			}
			case 0x4B6: { // GetPinCodeLength
				break;
			}
			case 0x4B7: { // GetPinCodeChangedEvent
				break;
			}
			case 0x4B8: { // GetPinCode
				break;
			}
			case 0x57B: { // IsPairingActive
				break;
			}
			case 0x57E: { // GetSettingsLastUpdated
				break;
			}
			case 0x583: { // GetPairingAccountInfo
				break;
			}
			case 0x58D: { // GetAccountNickname
				break;
			}
			case 0x590: { // GetAccountState
				break;
			}
			case 0x598: { // GetSynchronizationEvent
				break;
			}
			case 0x5AB: { // StartPlayTimer
				break;
			}
			case 0x5AC: { // StopPlayTimer
				break;
			}
			case 0x5AD: { // IsPlayTimerEnabled
				break;
			}
			case 0x5AE: { // GetPlayTimerRemainingTime
				break;
			}
			case 0x5AF: { // IsRestrictedByPlayTimer
				break;
			}
			case 0x5B0: { // GetPlayTimerSettings
				break;
			}
			case 0x5B1: { // GetPlayTimerEventToRequestSuspension
				break;
			}
			case 0x5B2: { // IsPlayTimerAlarmDisabled
				break;
			}
			case 0x5BF: { // NotifyWrongPinCodeInputManyTimes
				break;
			}
			case 0x5C0: { // CancelNetworkRequest
				break;
			}
			case 0x5C1: { // GetUnlinkedEvent
				break;
			}
			case 0x5C2: { // ClearUnlinkedEvent
				break;
			}
			case 0x641: { // DisableAllFeatures
				break;
			}
			case 0x642: { // PostEnableAllFeatures
				break;
			}
			case 0x643: { // IsAllFeaturesDisabled
				break;
			}
			case 0x76D: { // DeleteFromFreeCommunicationApplicationListForDebug
				break;
			}
			case 0x76E: { // ClearFreeCommunicationApplicationListForDebug
				break;
			}
			case 0x76F: { // GetExemptApplicationListCountForDebug
				break;
			}
			case 0x770: { // GetExemptApplicationListForDebug
				break;
			}
			case 0x771: { // UpdateExemptApplicationListForDebug
				break;
			}
			case 0x772: { // AddToExemptApplicationListForDebug
				break;
			}
			case 0x773: { // DeleteFromExemptApplicationListForDebug
				break;
			}
			case 0x774: { // ClearExemptApplicationListForDebug
				break;
			}
			case 0x795: { // DeletePairing
				break;
			}
			case 0x79F: { // SetPlayTimerSettingsForDebug
				break;
			}
			case 0x7A0: { // GetPlayTimerSpentTimeForTest
				break;
			}
			case 0x7A1: { // SetPlayTimerAlarmDisabledForDebug
				break;
			}
			case 0x7D1: { // RequestPairingAsync
				break;
			}
			case 0x7D2: { // FinishRequestPairing
				break;
			}
			case 0x7D3: { // AuthorizePairingAsync
				break;
			}
			case 0x7D4: { // FinishAuthorizePairing
				break;
			}
			case 0x7D5: { // RetrievePairingInfoAsync
				break;
			}
			case 0x7D6: { // FinishRetrievePairingInfo
				break;
			}
			case 0x7D7: { // UnlinkPairingAsync
				break;
			}
			case 0x7D8: { // FinishUnlinkPairing
				break;
			}
			case 0x7D9: { // GetAccountMiiImageAsync
				break;
			}
			case 0x7DA: { // FinishGetAccountMiiImage
				break;
			}
			case 0x7DB: { // GetAccountMiiImageContentTypeAsync
				break;
			}
			case 0x7DC: { // FinishGetAccountMiiImageContentType
				break;
			}
			case 0x7DD: { // SynchronizeParentalControlSettingsAsync
				break;
			}
			case 0x7DE: { // FinishSynchronizeParentalControlSettings
				break;
			}
			case 0x7DF: { // FinishSynchronizeParentalControlSettingsWithLastUpdated
				break;
			}
			case 0x7E0: { // RequestUpdateExemptionListAsync
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlService");
		}
	}
}

public partial class IParentalControlServiceFactory : _IParentalControlServiceFactory_Base;
public abstract class _IParentalControlServiceFactory_Base : IpcInterface {
	protected virtual Nn.Pctl.Detail.Ipc.IParentalControlService CreateService(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory.CreateService not implemented");
	protected virtual Nn.Pctl.Detail.Ipc.IParentalControlService CreateServiceWithoutInitialize(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory.CreateServiceWithoutInitialize not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateService
				break;
			}
			case 0x1: { // CreateServiceWithoutInitialize
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory");
		}
	}
}

