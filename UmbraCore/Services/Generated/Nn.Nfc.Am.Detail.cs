using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfc.Am.Detail;
public partial class IAm : _IAm_Base;
public abstract class _IAm_Base : IpcInterface {
	protected virtual void Initialize() =>
		"Stub hit for Nn.Nfc.Am.Detail.IAm.Initialize".Log();
	protected virtual void _Finalize() =>
		"Stub hit for Nn.Nfc.Am.Detail.IAm._Finalize".Log();
	protected virtual void NotifyForegroundApplet(byte[] _0) =>
		"Stub hit for Nn.Nfc.Am.Detail.IAm.NotifyForegroundApplet".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				Initialize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // NotifyForegroundApplet
				NotifyForegroundApplet(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Am.Detail.IAm");
		}
	}
}

public partial class IAmManager : _IAmManager_Base {
	public readonly string ServiceName;
	public IAmManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAmManager_Base : IpcInterface {
	protected virtual Nn.Nfc.Am.Detail.IAm CreateAmInterface() =>
		throw new NotImplementedException("Nn.Nfc.Am.Detail.IAmManager.CreateAmInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateAmInterface
				var _return = CreateAmInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfc.Am.Detail.IAmManager");
		}
	}
}

