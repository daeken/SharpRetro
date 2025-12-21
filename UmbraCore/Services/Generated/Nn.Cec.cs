using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Cec;
public partial class CecManagerSubinterface100 : _CecManagerSubinterface100_Base;
public abstract class _CecManagerSubinterface100_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Cec.CecManagerSubinterface100.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Cec.CecManagerSubinterface100.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Cec.CecManagerSubinterface100.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Cec.CecManagerSubinterface100.Unknown3 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Cec.CecManagerSubinterface100");
		}
	}
}

public partial class ICecManager : _ICecManager_Base;
public abstract class _ICecManager_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Cec.ICecManager.Unknown2");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown6 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Cec.ICecManager");
		}
	}
}

