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
				om.Initialize(0, 0, 0);
				Initialize();
				break;
			}
			case 0x3E9: { // CheckFreeCommunicationPermission
				om.Initialize(0, 0, 0);
				CheckFreeCommunicationPermission();
				break;
			}
			case 0x3EA: { // ConfirmLaunchApplicationPermission
				om.Initialize(0, 0, 0);
				ConfirmLaunchApplicationPermission(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<sbyte>(0x9, 0));
				break;
			}
			case 0x3EB: { // ConfirmResumeApplicationPermission
				om.Initialize(0, 0, 0);
				ConfirmResumeApplicationPermission(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<sbyte>(0x9, 0));
				break;
			}
			case 0x3EC: { // ConfirmSnsPostPermission
				om.Initialize(0, 0, 0);
				ConfirmSnsPostPermission();
				break;
			}
			case 0x3ED: { // ConfirmSystemSettingsPermission
				om.Initialize(0, 0, 0);
				ConfirmSystemSettingsPermission();
				break;
			}
			case 0x3EE: { // IsRestrictionTemporaryUnlocked
				om.Initialize(0, 0, 1);
				var _return = IsRestrictionTemporaryUnlocked();
				om.SetData(8, _return);
				break;
			}
			case 0x3EF: { // RevertRestrictionTemporaryUnlocked
				om.Initialize(0, 0, 0);
				RevertRestrictionTemporaryUnlocked();
				break;
			}
			case 0x3F0: { // EnterRestrictedSystemSettings
				om.Initialize(0, 0, 0);
				EnterRestrictedSystemSettings();
				break;
			}
			case 0x3F1: { // LeaveRestrictedSystemSettings
				om.Initialize(0, 0, 0);
				LeaveRestrictedSystemSettings();
				break;
			}
			case 0x3F2: { // IsRestrictedSystemSettingsEntered
				om.Initialize(0, 0, 1);
				var _return = IsRestrictedSystemSettingsEntered();
				om.SetData(8, _return);
				break;
			}
			case 0x3F3: { // RevertRestrictedSystemSettingsEntered
				om.Initialize(0, 0, 0);
				RevertRestrictedSystemSettingsEntered();
				break;
			}
			case 0x3F4: { // GetRestrictedFeatures
				om.Initialize(0, 0, 4);
				var _return = GetRestrictedFeatures();
				om.SetData(8, _return);
				break;
			}
			case 0x3F5: { // ConfirmStereoVisionPermission
				om.Initialize(0, 0, 0);
				ConfirmStereoVisionPermission();
				break;
			}
			case 0x3F6: { // ConfirmPlayableApplicationVideoOld
				om.Initialize(0, 0, 0);
				ConfirmPlayableApplicationVideoOld();
				break;
			}
			case 0x3F7: { // ConfirmPlayableApplicationVideo
				om.Initialize(0, 0, 0);
				ConfirmPlayableApplicationVideo();
				break;
			}
			case 0x407: { // IsRestrictionEnabled
				om.Initialize(0, 0, 1);
				var _return = IsRestrictionEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x408: { // GetSafetyLevel
				om.Initialize(0, 0, 4);
				var _return = GetSafetyLevel();
				om.SetData(8, _return);
				break;
			}
			case 0x409: { // SetSafetyLevel
				om.Initialize(0, 0, 0);
				SetSafetyLevel(im.GetData<uint>(8));
				break;
			}
			case 0x40A: { // GetSafetyLevelSettings
				om.Initialize(0, 0, 3);
				GetSafetyLevelSettings(im.GetData<uint>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x40B: { // GetCurrentSettings
				om.Initialize(0, 0, 3);
				GetCurrentSettings(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x40C: { // SetCustomSafetyLevelSettings
				om.Initialize(0, 0, 0);
				SetCustomSafetyLevelSettings(im.GetBytes(8, 0x3));
				break;
			}
			case 0x40D: { // GetDefaultRatingOrganization
				om.Initialize(0, 0, 4);
				var _return = GetDefaultRatingOrganization();
				om.SetData(8, _return);
				break;
			}
			case 0x40E: { // SetDefaultRatingOrganization
				om.Initialize(0, 0, 0);
				SetDefaultRatingOrganization(im.GetData<uint>(8));
				break;
			}
			case 0x40F: { // GetFreeCommunicationApplicationListCount
				om.Initialize(0, 0, 4);
				var _return = GetFreeCommunicationApplicationListCount();
				om.SetData(8, _return);
				break;
			}
			case 0x412: { // AddToFreeCommunicationApplicationList
				om.Initialize(0, 0, 0);
				AddToFreeCommunicationApplicationList(im.GetData<ulong>(8));
				break;
			}
			case 0x413: { // DeleteSettings
				om.Initialize(0, 0, 0);
				DeleteSettings();
				break;
			}
			case 0x414: { // GetFreeCommunicationApplicationList
				om.Initialize(0, 0, 4);
				GetFreeCommunicationApplicationList(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x415: { // UpdateFreeCommunicationApplicationList
				om.Initialize(0, 0, 0);
				UpdateFreeCommunicationApplicationList(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x416: { // DisableFeaturesForReset
				om.Initialize(0, 0, 0);
				DisableFeaturesForReset();
				break;
			}
			case 0x417: { // NotifyApplicationDownloadStarted
				om.Initialize(0, 0, 0);
				NotifyApplicationDownloadStarted(im.GetData<ulong>(8));
				break;
			}
			case 0x425: { // ConfirmStereoVisionRestrictionConfigurable
				om.Initialize(0, 0, 0);
				ConfirmStereoVisionRestrictionConfigurable();
				break;
			}
			case 0x426: { // GetStereoVisionRestriction
				om.Initialize(0, 0, 1);
				var _return = GetStereoVisionRestriction();
				om.SetData(8, _return);
				break;
			}
			case 0x427: { // SetStereoVisionRestriction
				om.Initialize(0, 0, 0);
				SetStereoVisionRestriction(im.GetData<byte>(8));
				break;
			}
			case 0x428: { // ResetConfirmedStereoVisionPermission
				om.Initialize(0, 0, 0);
				ResetConfirmedStereoVisionPermission();
				break;
			}
			case 0x429: { // IsStereoVisionPermitted
				om.Initialize(0, 0, 0);
				IsStereoVisionPermitted();
				break;
			}
			case 0x4B1: { // UnlockRestrictionTemporarily
				om.Initialize(0, 0, 0);
				UnlockRestrictionTemporarily(im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x4B2: { // UnlockSystemSettingsRestriction
				om.Initialize(0, 0, 0);
				UnlockSystemSettingsRestriction(im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x4B3: { // SetPinCode
				om.Initialize(0, 0, 0);
				SetPinCode(im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x4B4: { // GenerateInquiryCode
				om.Initialize(0, 0, 32);
				GenerateInquiryCode(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4B5: { // CheckMasterKey
				om.Initialize(0, 0, 1);
				var _return = CheckMasterKey(im.GetBytes(8, 0x20), im.GetSpan<byte>(0x9, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x4B6: { // GetPinCodeLength
				om.Initialize(0, 0, 4);
				var _return = GetPinCodeLength();
				om.SetData(8, _return);
				break;
			}
			case 0x4B7: { // GetPinCodeChangedEvent
				om.Initialize(0, 1, 0);
				var _return = GetPinCodeChangedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x4B8: { // GetPinCode
				om.Initialize(0, 0, 4);
				GetPinCode(out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x57B: { // IsPairingActive
				om.Initialize(0, 0, 1);
				var _return = IsPairingActive();
				om.SetData(8, _return);
				break;
			}
			case 0x57E: { // GetSettingsLastUpdated
				om.Initialize(0, 0, 8);
				var _return = GetSettingsLastUpdated();
				om.SetData(8, _return);
				break;
			}
			case 0x583: { // GetPairingAccountInfo
				om.Initialize(0, 0, 16);
				GetPairingAccountInfo(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x58D: { // GetAccountNickname
				om.Initialize(0, 0, 4);
				GetAccountNickname(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x590: { // GetAccountState
				om.Initialize(0, 0, 4);
				var _return = GetAccountState(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x598: { // GetSynchronizationEvent
				om.Initialize(0, 1, 0);
				var _return = GetSynchronizationEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5AB: { // StartPlayTimer
				om.Initialize(0, 0, 0);
				StartPlayTimer();
				break;
			}
			case 0x5AC: { // StopPlayTimer
				om.Initialize(0, 0, 0);
				StopPlayTimer();
				break;
			}
			case 0x5AD: { // IsPlayTimerEnabled
				om.Initialize(0, 0, 1);
				var _return = IsPlayTimerEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x5AE: { // GetPlayTimerRemainingTime
				om.Initialize(0, 0, 8);
				var _return = GetPlayTimerRemainingTime();
				om.SetData(8, _return);
				break;
			}
			case 0x5AF: { // IsRestrictedByPlayTimer
				om.Initialize(0, 0, 1);
				var _return = IsRestrictedByPlayTimer();
				om.SetData(8, _return);
				break;
			}
			case 0x5B0: { // GetPlayTimerSettings
				om.Initialize(0, 0, 52);
				GetPlayTimerSettings(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5B1: { // GetPlayTimerEventToRequestSuspension
				om.Initialize(0, 1, 0);
				var _return = GetPlayTimerEventToRequestSuspension();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5B2: { // IsPlayTimerAlarmDisabled
				om.Initialize(0, 0, 1);
				var _return = IsPlayTimerAlarmDisabled();
				om.SetData(8, _return);
				break;
			}
			case 0x5BF: { // NotifyWrongPinCodeInputManyTimes
				om.Initialize(0, 0, 0);
				NotifyWrongPinCodeInputManyTimes();
				break;
			}
			case 0x5C0: { // CancelNetworkRequest
				om.Initialize(0, 0, 0);
				CancelNetworkRequest();
				break;
			}
			case 0x5C1: { // GetUnlinkedEvent
				om.Initialize(0, 1, 0);
				var _return = GetUnlinkedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5C2: { // ClearUnlinkedEvent
				om.Initialize(0, 0, 0);
				ClearUnlinkedEvent();
				break;
			}
			case 0x641: { // DisableAllFeatures
				om.Initialize(0, 0, 1);
				var _return = DisableAllFeatures();
				om.SetData(8, _return);
				break;
			}
			case 0x642: { // PostEnableAllFeatures
				om.Initialize(0, 0, 1);
				var _return = PostEnableAllFeatures();
				om.SetData(8, _return);
				break;
			}
			case 0x643: { // IsAllFeaturesDisabled
				om.Initialize(0, 0, 2);
				IsAllFeaturesDisabled(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(9, _1);
				break;
			}
			case 0x76D: { // DeleteFromFreeCommunicationApplicationListForDebug
				om.Initialize(0, 0, 0);
				DeleteFromFreeCommunicationApplicationListForDebug(im.GetData<ulong>(8));
				break;
			}
			case 0x76E: { // ClearFreeCommunicationApplicationListForDebug
				om.Initialize(0, 0, 0);
				ClearFreeCommunicationApplicationListForDebug();
				break;
			}
			case 0x76F: { // GetExemptApplicationListCountForDebug
				om.Initialize(0, 0, 0);
				GetExemptApplicationListCountForDebug();
				break;
			}
			case 0x770: { // GetExemptApplicationListForDebug
				om.Initialize(0, 0, 0);
				GetExemptApplicationListForDebug();
				break;
			}
			case 0x771: { // UpdateExemptApplicationListForDebug
				om.Initialize(0, 0, 0);
				UpdateExemptApplicationListForDebug();
				break;
			}
			case 0x772: { // AddToExemptApplicationListForDebug
				om.Initialize(0, 0, 0);
				AddToExemptApplicationListForDebug();
				break;
			}
			case 0x773: { // DeleteFromExemptApplicationListForDebug
				om.Initialize(0, 0, 0);
				DeleteFromExemptApplicationListForDebug();
				break;
			}
			case 0x774: { // ClearExemptApplicationListForDebug
				om.Initialize(0, 0, 0);
				ClearExemptApplicationListForDebug();
				break;
			}
			case 0x795: { // DeletePairing
				om.Initialize(0, 0, 0);
				DeletePairing();
				break;
			}
			case 0x79F: { // SetPlayTimerSettingsForDebug
				om.Initialize(0, 0, 0);
				SetPlayTimerSettingsForDebug(im.GetBytes(8, 0x34));
				break;
			}
			case 0x7A0: { // GetPlayTimerSpentTimeForTest
				om.Initialize(0, 0, 8);
				var _return = GetPlayTimerSpentTimeForTest();
				om.SetData(8, _return);
				break;
			}
			case 0x7A1: { // SetPlayTimerAlarmDisabledForDebug
				om.Initialize(0, 0, 0);
				SetPlayTimerAlarmDisabledForDebug(im.GetData<byte>(8));
				break;
			}
			case 0x7D1: { // RequestPairingAsync
				om.Initialize(0, 1, 8);
				RequestPairingAsync(im.GetSpan<byte>(0x9, 0), out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D2: { // FinishRequestPairing
				om.Initialize(0, 0, 16);
				FinishRequestPairing(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D3: { // AuthorizePairingAsync
				om.Initialize(0, 1, 8);
				AuthorizePairingAsync(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D4: { // FinishAuthorizePairing
				om.Initialize(0, 0, 16);
				FinishAuthorizePairing(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D5: { // RetrievePairingInfoAsync
				om.Initialize(0, 1, 8);
				RetrievePairingInfoAsync(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D6: { // FinishRetrievePairingInfo
				om.Initialize(0, 0, 16);
				FinishRetrievePairingInfo(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D7: { // UnlinkPairingAsync
				om.Initialize(0, 1, 8);
				UnlinkPairingAsync(im.GetData<byte>(8), out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7D8: { // FinishUnlinkPairing
				om.Initialize(0, 0, 0);
				FinishUnlinkPairing(im.GetData<byte>(8), im.GetBytes(12, 0x8));
				break;
			}
			case 0x7D9: { // GetAccountMiiImageAsync
				om.Initialize(0, 1, 12);
				GetAccountMiiImageAsync(im.GetBytes(8, 0x10), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				om.SetData(16, _1);
				om.Copy(0, CreateHandle(_2, copy: true));
				break;
			}
			case 0x7DA: { // FinishGetAccountMiiImage
				om.Initialize(0, 0, 4);
				FinishGetAccountMiiImage(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7DB: { // GetAccountMiiImageContentTypeAsync
				om.Initialize(0, 1, 12);
				GetAccountMiiImageContentTypeAsync(im.GetBytes(8, 0x10), out var _0, out var _1, out var _2, im.GetSpan<byte>(0xA, 0));
				om.SetBytes(8, _0);
				om.SetData(16, _1);
				om.Copy(0, CreateHandle(_2, copy: true));
				break;
			}
			case 0x7DC: { // FinishGetAccountMiiImageContentType
				om.Initialize(0, 0, 4);
				FinishGetAccountMiiImageContentType(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7DD: { // SynchronizeParentalControlSettingsAsync
				om.Initialize(0, 1, 8);
				SynchronizeParentalControlSettingsAsync(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7DE: { // FinishSynchronizeParentalControlSettings
				om.Initialize(0, 0, 0);
				FinishSynchronizeParentalControlSettings(im.GetBytes(8, 0x8));
				break;
			}
			case 0x7DF: { // FinishSynchronizeParentalControlSettingsWithLastUpdated
				om.Initialize(0, 0, 8);
				var _return = FinishSynchronizeParentalControlSettingsWithLastUpdated(im.GetBytes(8, 0x8));
				om.SetData(8, _return);
				break;
			}
			case 0x7E0: { // RequestUpdateExemptionListAsync
				om.Initialize(0, 0, 0);
				RequestUpdateExemptionListAsync();
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateService
				om.Initialize(1, 0, 0);
				var _return = CreateService(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateServiceWithoutInitialize
				om.Initialize(1, 0, 0);
				var _return = CreateServiceWithoutInitialize(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pctl.Detail.Ipc.IParentalControlServiceFactory");
		}
	}
}

