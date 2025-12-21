using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Profiler;
public partial class IProfiler : _IProfiler_Base {
	public readonly string ServiceName;
	public IProfiler(string serviceName) => ServiceName = serviceName;
}
public abstract class _IProfiler_Base : IpcInterface {
	protected virtual void GetSystemEvent() =>
		Console.WriteLine("Stub hit for Nn.Profiler.IProfiler.GetSystemEvent");
	protected virtual void StartSignalingEvent() =>
		Console.WriteLine("Stub hit for Nn.Profiler.IProfiler.StartSignalingEvent");
	protected virtual void StopSignalingEvent() =>
		Console.WriteLine("Stub hit for Nn.Profiler.IProfiler.StopSignalingEvent");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSystemEvent
				GetSystemEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // StartSignalingEvent
				StartSignalingEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // StopSignalingEvent
				StopSignalingEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Profiler.IProfiler");
		}
	}
}

