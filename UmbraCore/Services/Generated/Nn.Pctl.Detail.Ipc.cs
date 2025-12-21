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
	protected virtual void GetSafetyLevelSettings(uint _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetSafetyLevelSettings not implemented");
	protected virtual void GetCurrentSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetCurrentSettings not implemented");
	protected virtual void SetCustomSafetyLevelSettings(byte[] _0) =>
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
	protected virtual void GenerateInquiryCode(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GenerateInquiryCode not implemented");
	protected virtual byte CheckMasterKey(byte[] _0, Span<byte> _1) =>
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
	protected virtual void GetPairingAccountInfo(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPairingAccountInfo not implemented");
	protected virtual void GetAccountNickname(byte[] _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountNickname not implemented");
	protected virtual uint GetAccountState(byte[] _0) =>
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
	protected virtual void GetPlayTimerSettings(out byte[] _0) =>
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
	protected virtual void SetPlayTimerSettingsForDebug(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetPlayTimerSettingsForDebug");
	protected virtual ulong GetPlayTimerSpentTimeForTest() =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetPlayTimerSpentTimeForTest not implemented");
	protected virtual void SetPlayTimerAlarmDisabledForDebug(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.SetPlayTimerAlarmDisabledForDebug");
	protected virtual void RequestPairingAsync(Span<byte> _0, out byte[] _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.RequestPairingAsync not implemented");
	protected virtual void FinishRequestPairing(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishRequestPairing not implemented");
	protected virtual void AuthorizePairingAsync(byte[] _0, out byte[] _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.AuthorizePairingAsync not implemented");
	protected virtual void FinishAuthorizePairing(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishAuthorizePairing not implemented");
	protected virtual void RetrievePairingInfoAsync(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.RetrievePairingInfoAsync not implemented");
	protected virtual void FinishRetrievePairingInfo(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishRetrievePairingInfo not implemented");
	protected virtual void UnlinkPairingAsync(byte _0, out byte[] _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.UnlinkPairingAsync not implemented");
	protected virtual void FinishUnlinkPairing(byte _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.FinishUnlinkPairing");
	protected virtual void GetAccountMiiImageAsync(byte[] _0, out byte[] _1, out uint _2, out KObject _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountMiiImageAsync not implemented");
	protected virtual void FinishGetAccountMiiImage(byte[] _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishGetAccountMiiImage not implemented");
	protected virtual void GetAccountMiiImageContentTypeAsync(byte[] _0, out byte[] _1, out uint _2, out KObject _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.GetAccountMiiImageContentTypeAsync not implemented");
	protected virtual void FinishGetAccountMiiImageContentType(byte[] _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishGetAccountMiiImageContentType not implemented");
	protected virtual void SynchronizeParentalControlSettingsAsync(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.SynchronizeParentalControlSettingsAsync not implemented");
	protected virtual void FinishSynchronizeParentalControlSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.FinishSynchronizeParentalControlSettings");
	protected virtual ulong FinishSynchronizeParentalControlSettingsWithLastUpdated(byte[] _0) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlService.FinishSynchronizeParentalControlSettingsWithLastUpdated not implemented");
	protected virtual void RequestUpdateExemptionListAsync() =>
		Console.WriteLine("Stub hit for Nn.Pctl.Detail.Ipc.IParentalControlService.RequestUpdateExemptionListAsync");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Initialize
				Initialize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // CheckFreeCommunicationPermission
				CheckFreeCommunicationPermission();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EA: { // ConfirmLaunchApplicationPermission
				ConfirmLaunchApplicationPermission(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<sbyte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EB: { // ConfirmResumeApplicationPermission
				ConfirmResumeApplicationPermission(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<sbyte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EC: { // ConfirmSnsPostPermission
				ConfirmSnsPostPermission();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3ED: { // ConfirmSystemSettingsPermission
				ConfirmSystemSettingsPermission();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EE: { // IsRestrictionTemporaryUnlocked
				var _return = IsRestrictionTemporaryUnlocked();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3EF: { // RevertRestrictionTemporaryUnlocked
				RevertRestrictionTemporaryUnlocked();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F0: { // EnterRestrictedSystemSettings
				EnterRestrictedSystemSettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F1: { // LeaveRestrictedSystemSettings
				LeaveRestrictedSystemSettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F2: { // IsRestrictedSystemSettingsEntered
				var _return = IsRestrictedSystemSettingsEntered();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3F3: { // RevertRestrictedSystemSettingsEntered
				RevertRestrictedSystemSettingsEntered();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F4: { // GetRestrictedFeatures
				var _return = GetRestrictedFeatures();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3F5: { // ConfirmStereoVisionPermission
				ConfirmStereoVisionPermission();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F6: { // ConfirmPlayableApplicationVideoOld
				ConfirmPlayableApplicationVideoOld();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F7: { // ConfirmPlayableApplicationVideo
				ConfirmPlayableApplicationVideo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x407: { // IsRestrictionEnabled
				var _return = IsRestrictionEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x408: { // GetSafetyLevel
				var _return = GetSafetyLevel();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x409: { // SetSafetyLevel
				SetSafetyLevel(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x40A: { // GetSafetyLevelSettings
				GetSafetyLevelSettings(im.GetData<uint>(8), out var _0);
				om.Initialize(0, 0, 3);
				om.SetBytes(8, _0);
				break;
			}
			case 0x40B: { // GetCurrentSettings
				GetCurrentSettings(out var _0);
				om.Initialize(0, 0, 3);
				om.SetBytes(8, _0);
				break;
			}
			case 0x40C: { // SetCustomSafetyLevelSettings
				SetCustomSafetyLevelSettings(im.GetBytes(8, 0x3));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x40D: { // GetDefaultRatingOrganization
				var _return = GetDefaultRatingOrganization();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x40E: { // SetDefaultRatingOrganization
				SetDefaultRatingOrganization(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x40F: { // GetFreeCommunicationApplicationListCount
				var _return = GetFreeCommunicationApplicationListCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x412: { // AddToFreeCommunicationApplicationList
				AddToFreeCommunicationApplicationList(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x413: { // DeleteSettings
				DeleteSettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x414: { // GetFreeCommunicationApplicationList
				GetFreeCommunicationApplicationList(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x415: { // UpdateFreeCommunicationApplicationList
				UpdateFreeCommunicationApplicationList(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x416: { // DisableFeaturesForReset
				DisableFeaturesForReset();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x417: { // NotifyApplicationDownloadStarted
				NotifyApplicationDownloadStarted(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x425: { // ConfirmStereoVisionRestrictionConfigurable
				ConfirmStereoVisionRestrictionConfigurable();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x426: { // GetStereoVisionRestriction
				var _return = GetStereoVisionRestriction();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x427: { // SetStereoVisionRestriction
				SetStereoVisionRestriction(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x428: { // ResetConfirmedStereoVisionPermission
				ResetConfirmedStereoVisionPermission();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x429: { // IsStereoVisionPermitted
				IsStereoVisionPermitted();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B1: { // UnlockRestrictionTemporarily
				UnlockRestrictionTemporarily(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B2: { // UnlockSystemSettingsRestriction
				UnlockSystemSettingsRestriction(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B3: { // SetPinCode
				SetPinCode(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B4: { // GenerateInquiryCode
				GenerateInquiryCode(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4B5: { // CheckMasterKey
				var _return = CheckMasterKey(im.GetBytes(8, 0x20), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x4B6: { // GetPinCodeLength
				var _return = GetPinCodeLength();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4B7: { // GetPinCodeChangedEvent
				var _return = GetPinCodeChangedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x4B8: { // GetPinCode
				GetPinCode(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x57B: { // IsPairingActive
				var _return = IsPairingActive();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x57E: { // GetSettingsLastUpdated
				var _return = GetSettingsLastUpdated();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x583: { // GetPairingAccountInfo
				GetPairingAccountInfo(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x58D: { // GetAccountNickname
				GetAccountNickname(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x590: { // GetAccountState
				var _return = GetAccountState(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x598: { // GetSynchronizationEvent
				var _return = GetSynchronizationEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5AB: { // StartPlayTimer
				StartPlayTimer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5AC: { // StopPlayTimer
				StopPlayTimer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5AD: { // IsPlayTimerEnabled
				var _return = IsPlayTimerEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x5AE: { // GetPlayTimerRemainingTime
				var _return = GetPlayTimerRemainingTime();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5AF: { // IsRestrictedByPlayTimer
				var _return = IsRestrictedByPlayTimer();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x5B0: { // GetPlayTimerSettings
				GetPlayTimerSettings(out var _0);
				om.Initialize(0, 0, 52);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5B1: { // GetPlayTimerEventToRequestSuspension
				var _return = GetPlayTimerEventToRequestSuspension();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5B2: { // IsPlayTimerAlarmDisabled
				var _return = IsPlayTimerAlarmDisabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x5BF: { // NotifyWrongPinCodeInputManyTimes
				NotifyWrongPinCodeInputManyTimes();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5C0: { // CancelNetworkRequest
				CancelNetworkRequest();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5C1: { // GetUnlinkedEvent
				var _return = GetUnlinkedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5C2: { // ClearUnlinkedEvent
				ClearUnlinkedEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x641: { // DisableAllFeatures
				var _return = DisableAllFeatures();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x642: { // PostEnableAllFeatures
				var _return = PostEnableAllFeatures();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x643: { // IsAllFeaturesDisabled
				IsAllFeaturesDisabled(out var _0, out var _1);
				om.Initialize(0, 0, 2);
				om.SetData(8, _0);
				om.SetData(9, _1);
				break;
			}
			case 0x76D: { // DeleteFromFreeCommunicationApplicationListForDebug
				DeleteFromFreeCommunicationApplicationListForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x76E: { // ClearFreeCommunicationApplicationListForDebug
				ClearFreeCommunicationApplicationListForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x76F: { // GetExemptApplicationListCountForDebug
				GetExemptApplicationListCountForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x770: { // GetExemptApplicationListForDebug
				GetExemptApplicationListForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x771: { // UpdateExemptApplicationListForDebug
				UpdateExemptApplicationListForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x772: { // AddToExemptApplicationListForDebug
				AddToExemptApplicationListForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x773: { // DeleteFromExemptApplicationListForDebug
				DeleteFromExemptApplicationListForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x774: { // ClearExemptApplicationListForDebug
				ClearExemptApplicationListForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x795: { // DeletePairing
				DeletePairing();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x79F: { // SetPlayTimerSettingsForDebug
				SetPlayTimerSettingsForDebug(im.GetBytes(8, 0x34));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7A0: { // GetPlayTimerSpentTimeForTest
				var _return = GetPlayTimerSpentTimeForTest();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x7A1: { // SetPlayTimerAlarmDisabledForDebug
				SetPlayTimerAlarmDisabledForDebug(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D1: { // RequestPairingAsync
				RequestPairingAsync(im.GetSpan<byte>(0x9, 0), out var _0, out var _1);
				om.Initialize(0, 1, 8);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D2: { // FinishRequestPairing
				FinishRequestPairing(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D3: { // AuthorizePairingAsync
				AuthorizePairingAsync(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Initialize(0, 1, 8);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D4: { // FinishAuthorizePairing
				FinishAuthorizePairing(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D5: { // RetrievePairingInfoAsync
				RetrievePairingInfoAsync(out var _0, out var _1);
				om.Initialize(0, 1, 8);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D6: { // FinishRetrievePairingInfo
				FinishRetrievePairingInfo(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D7: { // UnlinkPairingAsync
				UnlinkPairingAsync(im.GetData<byte>(8), out var _0, out var _1);
				om.Initialize(0, 1, 8);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D8: { // FinishUnlinkPairing
				FinishUnlinkPairing(im.GetData<byte>(8), im.GetBytes(12, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D9: { // GetAccountMiiImageAsync
				GetAccountMiiImageAsync(im.GetBytes(8, 0x10), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 1, 12);
				om.SetBytes(8, _0);
				om.SetData(16, _1);
				om.Copy(0, CreateHandle(_2, copy: true));
				break;
			}
			case 0x7DA: { // FinishGetAccountMiiImage
				FinishGetAccountMiiImage(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x7DB: { // GetAccountMiiImageContentTypeAsync
				GetAccountMiiImageContentTypeAsync(im.GetBytes(8, 0x10), out var _0, out var _1, out var _2, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 1, 12);
				om.SetBytes(8, _0);
				om.SetData(16, _1);
				om.Copy(0, CreateHandle(_2, copy: true));
				break;
			}
			case 0x7DC: { // FinishGetAccountMiiImageContentType
				FinishGetAccountMiiImageContentType(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x7DD: { // SynchronizeParentalControlSettingsAsync
				SynchronizeParentalControlSettingsAsync(out var _0, out var _1);
				om.Initialize(0, 1, 8);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7DE: { // FinishSynchronizeParentalControlSettings
				FinishSynchronizeParentalControlSettings(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7DF: { // FinishSynchronizeParentalControlSettingsWithLastUpdated
				var _return = FinishSynchronizeParentalControlSettingsWithLastUpdated(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x7E0: { // RequestUpdateExemptionListAsync
				RequestUpdateExemptionListAsync();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlService");
		}
	}
}

public partial class IParentalControlServiceFactory : _IParentalControlServiceFactory_Base {
	public readonly string ServiceName;
	public IParentalControlServiceFactory(string serviceName) => ServiceName = serviceName;
}
public abstract class _IParentalControlServiceFactory_Base : IpcInterface {
	protected virtual Nn.Pctl.Detail.Ipc.IParentalControlService CreateService(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory.CreateService not implemented");
	protected virtual Nn.Pctl.Detail.Ipc.IParentalControlService CreateServiceWithoutInitialize(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory.CreateServiceWithoutInitialize not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateService
				var _return = CreateService(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateServiceWithoutInitialize
				var _return = CreateServiceWithoutInitialize(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory");
		}
	}
}

