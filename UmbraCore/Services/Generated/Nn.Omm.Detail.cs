using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Omm.Detail;
public partial class IOperationModeManager : _IOperationModeManager_Base {
	public readonly string ServiceName;
	public IOperationModeManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IOperationModeManager_Base : IpcInterface {
	protected virtual void GetOperationMode(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.GetOperationMode not implemented");
	protected virtual KObject GetOperationModeChangeEvent() =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.GetOperationModeChangeEvent not implemented");
	protected virtual void EnableAudioVisual() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.EnableAudioVisual".Log();
	protected virtual void DisableAudioVisual() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.DisableAudioVisual".Log();
	protected virtual void EnterSleepAndWait(KObject _0) =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.EnterSleepAndWait".Log();
	protected virtual void GetCradleStatus(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.GetCradleStatus not implemented");
	protected virtual void FadeInDisplay() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.FadeInDisplay".Log();
	protected virtual void FadeOutDisplay() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.FadeOutDisplay".Log();
	protected virtual void Unknown8(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown8 not implemented");
	protected virtual void Unknown9() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown9".Log();
	protected virtual void Unknown10(byte[] _0) =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown10".Log();
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown11 not implemented");
	protected virtual KObject Unknown12() =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown12 not implemented");
	protected virtual void Unknown13() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown13".Log();
	protected virtual void Unknown14(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown14 not implemented");
	protected virtual void Unknown15() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown15".Log();
	protected virtual void Unknown16() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown16".Log();
	protected virtual void Unknown17() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown17".Log();
	protected virtual void Unknown18() =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown18".Log();
	protected virtual KObject Unknown19() =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown19 not implemented");
	protected virtual void Unknown20(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown20 not implemented");
	protected virtual void Unknown21(byte[] _0) =>
		"Stub hit for Nn.Omm.Detail.IOperationModeManager.Unknown21".Log();
	protected virtual KObject Unknown22() =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown22 not implemented");
	protected virtual void Unknown23(out byte[] _0) =>
		throw new NotImplementedException("Nn.Omm.Detail.IOperationModeManager.Unknown23 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetOperationMode
				GetOperationMode(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetOperationModeChangeEvent
				var _return = GetOperationModeChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // EnableAudioVisual
				EnableAudioVisual();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // DisableAudioVisual
				DisableAudioVisual();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // EnterSleepAndWait
				EnterSleepAndWait(Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetCradleStatus
				GetCradleStatus(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // FadeInDisplay
				FadeInDisplay();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // FadeOutDisplay
				FadeOutDisplay();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // Unknown12
				var _return = Unknown12();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xD: { // Unknown13
				Unknown13();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // Unknown17
				Unknown17();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // Unknown18
				Unknown18();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // Unknown19
				var _return = Unknown19();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x14: { // Unknown20
				Unknown20(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // Unknown22
				var _return = Unknown22();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x17: { // Unknown23
				Unknown23(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Omm.Detail.IOperationModeManager");
		}
	}
}

