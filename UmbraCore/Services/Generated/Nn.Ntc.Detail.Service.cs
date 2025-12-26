using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ntc.Detail.Service;
public partial class IEnsureNetworkClockAvailabilityService : _IEnsureNetworkClockAvailabilityService_Base;
public abstract class _IEnsureNetworkClockAvailabilityService_Base : IpcInterface {
	protected virtual void StartTask() =>
		"Stub hit for Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.StartTask".Log();
	protected virtual KObject GetFinishNotificationEvent() =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.GetFinishNotificationEvent not implemented");
	protected virtual void GetResult() =>
		"Stub hit for Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.GetResult".Log();
	protected virtual void Cancel() =>
		"Stub hit for Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.Cancel".Log();
	protected virtual byte IsProcessing() =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.IsProcessing not implemented");
	protected virtual ulong GetServerTime() =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.GetServerTime not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // StartTask
				StartTask();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetFinishNotificationEvent
				var _return = GetFinishNotificationEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // IsProcessing
				var _return = IsProcessing();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetServerTime
				var _return = GetServerTime();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService");
		}
	}
}

public partial class IStaticService : _IStaticService_Base {
	public readonly string ServiceName;
	public IStaticService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IStaticService_Base : IpcInterface {
	protected virtual Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService OpenEnsureNetworkClockAvailabilityService(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IStaticService.OpenEnsureNetworkClockAvailabilityService not implemented");
	protected virtual void SuspendAutonomicTimeCorrection() =>
		"Stub hit for Nn.Ntc.Detail.Service.IStaticService.SuspendAutonomicTimeCorrection".Log();
	protected virtual void ResumeAutonomicTimeCorrection() =>
		"Stub hit for Nn.Ntc.Detail.Service.IStaticService.ResumeAutonomicTimeCorrection".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenEnsureNetworkClockAvailabilityService
				var _return = OpenEnsureNetworkClockAvailabilityService(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x64: { // SuspendAutonomicTimeCorrection
				SuspendAutonomicTimeCorrection();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // ResumeAutonomicTimeCorrection
				ResumeAutonomicTimeCorrection();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ntc.Detail.Service.IStaticService");
		}
	}
}

