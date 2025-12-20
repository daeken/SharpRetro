using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Profiler;
public partial class IProfiler : _IProfiler_Base;
public abstract class _IProfiler_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSystemEvent
				break;
			case 0x1: // StartSignalingEvent
				break;
			case 0x2: // StopSignalingEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Profiler.IProfiler");
		}
	}
}

