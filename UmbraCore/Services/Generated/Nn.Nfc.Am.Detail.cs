using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Am.Detail;
public partial class IAm : _IAm_Base;
public abstract class _IAm_Base : IpcInterface {
	protected virtual void Initialize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Am.Detail.IAm.Initialize");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Nfc.Am.Detail.IAm.Finalize");
	protected virtual void NotifyForegroundApplet(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nfc.Am.Detail.IAm.NotifyForegroundApplet");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				break;
			}
			case 0x1: { // Finalize
				break;
			}
			case 0x2: { // NotifyForegroundApplet
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Am.Detail.IAm");
		}
	}
}

public partial class IAmManager : _IAmManager_Base;
public abstract class _IAmManager_Base : IpcInterface {
	protected virtual Nn.Nfc.Am.Detail.IAm CreateAmInterface() =>
		throw new NotImplementedException("Nn.Nfc.Am.Detail.IAmManager.CreateAmInterface not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateAmInterface
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Am.Detail.IAmManager");
		}
	}
}

