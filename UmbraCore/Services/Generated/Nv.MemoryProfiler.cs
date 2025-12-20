using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nv.MemoryProfiler;
public partial class IMemoryProfiler : _IMemoryProfiler_Base;
public abstract class _IMemoryProfiler_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nv.MemoryProfiler.IMemoryProfiler");
		}
	}
}

