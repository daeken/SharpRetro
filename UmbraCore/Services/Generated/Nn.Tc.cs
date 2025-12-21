using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Tc;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void SetOperatingMode(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.SetOperatingMode");
	protected virtual KObject GetThermalEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tc.IManager.GetThermalEvent not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tc.IManager.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown5");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown6");
	protected virtual void Unknown7() =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown7");
	protected virtual void Unknown8() =>
		throw new NotImplementedException("Nn.Tc.IManager.Unknown8 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetOperatingMode
				break;
			case 0x1: // GetThermalEvent
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tc.IManager");
		}
	}
}

