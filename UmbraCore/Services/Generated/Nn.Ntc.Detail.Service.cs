using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ntc.Detail.Service;
public partial class IEnsureNetworkClockAvailabilityService : _IEnsureNetworkClockAvailabilityService_Base;
public abstract class _IEnsureNetworkClockAvailabilityService_Base : IpcInterface {
	protected virtual void StartTask() =>
		Console.WriteLine("Stub hit for Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.StartTask");
	protected virtual KObject GetFinishNotificationEvent() =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.GetFinishNotificationEvent not implemented");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.GetResult");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.Cancel");
	protected virtual byte IsProcessing() =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.IsProcessing not implemented");
	protected virtual ulong GetServerTime() =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService.GetServerTime not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // StartTask
				om.Initialize(0, 0, 0);
				StartTask();
				break;
			}
			case 0x1: { // GetFinishNotificationEvent
				om.Initialize(0, 1, 0);
				var _return = GetFinishNotificationEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x3: { // Cancel
				om.Initialize(0, 0, 0);
				Cancel();
				break;
			}
			case 0x4: { // IsProcessing
				om.Initialize(0, 0, 1);
				var _return = IsProcessing();
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetServerTime
				om.Initialize(0, 0, 8);
				var _return = GetServerTime();
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService");
		}
	}
}

public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected virtual Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService OpenEnsureNetworkClockAvailabilityService(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Ntc.Detail.Service.IStaticService.OpenEnsureNetworkClockAvailabilityService not implemented");
	protected virtual void SuspendAutonomicTimeCorrection() =>
		Console.WriteLine("Stub hit for Nn.Ntc.Detail.Service.IStaticService.SuspendAutonomicTimeCorrection");
	protected virtual void ResumeAutonomicTimeCorrection() =>
		Console.WriteLine("Stub hit for Nn.Ntc.Detail.Service.IStaticService.ResumeAutonomicTimeCorrection");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenEnsureNetworkClockAvailabilityService
				om.Initialize(1, 0, 0);
				var _return = OpenEnsureNetworkClockAvailabilityService(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x64: { // SuspendAutonomicTimeCorrection
				om.Initialize(0, 0, 0);
				SuspendAutonomicTimeCorrection();
				break;
			}
			case 0x65: { // ResumeAutonomicTimeCorrection
				om.Initialize(0, 0, 0);
				ResumeAutonomicTimeCorrection();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ntc.Detail.Service.IStaticService");
		}
	}
}

