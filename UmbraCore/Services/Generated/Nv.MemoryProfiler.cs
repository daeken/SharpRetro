using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nv.MemoryProfiler;
public partial class IMemoryProfiler : _IMemoryProfiler_Base;
public abstract class _IMemoryProfiler_Base : IpcInterface {
	protected virtual void Unknown0() =>
		"Stub hit for Nv.MemoryProfiler.IMemoryProfiler.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nv.MemoryProfiler.IMemoryProfiler.Unknown1".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nv.MemoryProfiler.IMemoryProfiler");
		}
	}
}

