using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nv.Gemcoredump;
public partial class INvGemCoreDump : _INvGemCoreDump_Base;
public abstract class _INvGemCoreDump_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nv.Gemcoredump.INvGemCoreDump.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nv.Gemcoredump.INvGemCoreDump.Unknown1 not implemented");
	protected virtual void Unknown2(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nv.Gemcoredump.INvGemCoreDump.Unknown2 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 16);
				Unknown1(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 4);
				Unknown2(out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nv.Gemcoredump.INvGemCoreDump");
		}
	}
}

