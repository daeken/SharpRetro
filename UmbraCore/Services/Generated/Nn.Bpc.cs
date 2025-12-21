using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bpc;
public partial class IBoardPowerControlManager : _IBoardPowerControlManager_Base;
public abstract class _IBoardPowerControlManager_Base : IpcInterface {
	protected virtual void ShutdownSystem() =>
		Console.WriteLine("Stub hit for Nn.Bpc.IBoardPowerControlManager.ShutdownSystem");
	protected virtual void RebootSystem() =>
		Console.WriteLine("Stub hit for Nn.Bpc.IBoardPowerControlManager.RebootSystem");
	protected virtual void GetWakeupReason(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetWakeupReason not implemented");
	protected virtual void GetShutdownReason(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetShutdownReason not implemented");
	protected virtual void GetAcOk(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetAcOk not implemented");
	protected virtual KObject GetBoardPowerControlEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetBoardPowerControlEvent not implemented");
	protected virtual void GetSleepButtonState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetSleepButtonState not implemented");
	protected virtual KObject GetPowerEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetPowerEvent not implemented");
	protected virtual void Unknown8(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.Unknown8 not implemented");
	protected virtual void Unknown9(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bpc.IBoardPowerControlManager.Unknown9");
	protected virtual void Unknown10(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.Unknown10 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ShutdownSystem
				om.Initialize(0, 0, 0);
				ShutdownSystem();
				break;
			}
			case 0x1: { // RebootSystem
				om.Initialize(0, 0, 0);
				RebootSystem();
				break;
			}
			case 0x2: { // GetWakeupReason
				om.Initialize(0, 0, 4);
				GetWakeupReason(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // GetShutdownReason
				om.Initialize(0, 0, 4);
				GetShutdownReason(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // GetAcOk
				om.Initialize(0, 0, 1);
				GetAcOk(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetBoardPowerControlEvent
				om.Initialize(0, 1, 0);
				var _return = GetBoardPowerControlEvent(im.GetBytes(8, 0x4));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6: { // GetSleepButtonState
				om.Initialize(0, 0, 4);
				GetSleepButtonState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // GetPowerEvent
				om.Initialize(0, 1, 0);
				var _return = GetPowerEvent(im.GetBytes(8, 0x4));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x8: { // Unknown8
				om.Initialize(0, 0, 4);
				Unknown8(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				om.Initialize(0, 0, 0);
				Unknown9(im.GetBytes(8, 0x4));
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 12);
				Unknown10(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IBoardPowerControlManager");
		}
	}
}

public partial class IPowerButtonManager : _IPowerButtonManager_Base;
public abstract class _IPowerButtonManager_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IPowerButtonManager.Unknown0 not implemented");
	protected virtual KObject Unknown1(byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IPowerButtonManager.Unknown1 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 1, 0);
				var _return = Unknown1(im.GetBytes(8, 0x4));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IPowerButtonManager");
		}
	}
}

public partial class IRtcManager : _IRtcManager_Base;
public abstract class _IRtcManager_Base : IpcInterface {
	protected virtual void GetExternalRtcValue(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IRtcManager.GetExternalRtcValue not implemented");
	protected virtual void SetExternalRtcValue(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bpc.IRtcManager.SetExternalRtcValue");
	protected virtual void ReadExternalRtcResetFlag(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IRtcManager.ReadExternalRtcResetFlag not implemented");
	protected virtual void ClearExternalRtcResetFlag() =>
		Console.WriteLine("Stub hit for Nn.Bpc.IRtcManager.ClearExternalRtcResetFlag");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetExternalRtcValue
				om.Initialize(0, 0, 8);
				GetExternalRtcValue(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // SetExternalRtcValue
				om.Initialize(0, 0, 0);
				SetExternalRtcValue(im.GetBytes(8, 0x8));
				break;
			}
			case 0x2: { // ReadExternalRtcResetFlag
				om.Initialize(0, 0, 1);
				ReadExternalRtcResetFlag(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // ClearExternalRtcResetFlag
				om.Initialize(0, 0, 0);
				ClearExternalRtcResetFlag();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IRtcManager");
		}
	}
}

public partial class IWakeupConfigManager : _IWakeupConfigManager_Base;
public abstract class _IWakeupConfigManager_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Bpc.IWakeupConfigManager.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bpc.IWakeupConfigManager.Unknown1");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bpc.IWakeupConfigManager.Unknown2 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetBytes(8, 0x4));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 16);
				Unknown2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IWakeupConfigManager");
		}
	}
}

