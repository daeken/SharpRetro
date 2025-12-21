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
	protected virtual void GetWakeupReason() =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetWakeupReason not implemented");
	protected virtual void GetShutdownReason() =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetShutdownReason not implemented");
	protected virtual void GetAcOk() =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetAcOk not implemented");
	protected virtual KObject GetBoardPowerControlEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetBoardPowerControlEvent not implemented");
	protected virtual void GetSleepButtonState() =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetSleepButtonState not implemented");
	protected virtual KObject GetPowerEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.GetPowerEvent not implemented");
	protected virtual void Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.Unknown8 not implemented");
	protected virtual void Unknown9(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bpc.IBoardPowerControlManager.Unknown9");
	protected virtual void Unknown10() =>
		throw new NotImplementedException("Nn.Bpc.IBoardPowerControlManager.Unknown10 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ShutdownSystem
				break;
			case 0x1: // RebootSystem
				break;
			case 0x2: // GetWakeupReason
				break;
			case 0x3: // GetShutdownReason
				break;
			case 0x4: // GetAcOk
				break;
			case 0x5: // GetBoardPowerControlEvent
				break;
			case 0x6: // GetSleepButtonState
				break;
			case 0x7: // GetPowerEvent
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IBoardPowerControlManager");
		}
	}
}

public partial class IPowerButtonManager : _IPowerButtonManager_Base;
public abstract class _IPowerButtonManager_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Bpc.IPowerButtonManager.Unknown0 not implemented");
	protected virtual KObject Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bpc.IPowerButtonManager.Unknown1 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IPowerButtonManager");
		}
	}
}

public partial class IRtcManager : _IRtcManager_Base;
public abstract class _IRtcManager_Base : IpcInterface {
	protected virtual void GetExternalRtcValue() =>
		throw new NotImplementedException("Nn.Bpc.IRtcManager.GetExternalRtcValue not implemented");
	protected virtual void SetExternalRtcValue(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bpc.IRtcManager.SetExternalRtcValue");
	protected virtual void ReadExternalRtcResetFlag() =>
		throw new NotImplementedException("Nn.Bpc.IRtcManager.ReadExternalRtcResetFlag not implemented");
	protected virtual void ClearExternalRtcResetFlag() =>
		Console.WriteLine("Stub hit for Nn.Bpc.IRtcManager.ClearExternalRtcResetFlag");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetExternalRtcValue
				break;
			case 0x1: // SetExternalRtcValue
				break;
			case 0x2: // ReadExternalRtcResetFlag
				break;
			case 0x3: // ClearExternalRtcResetFlag
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IRtcManager");
		}
	}
}

public partial class IWakeupConfigManager : _IWakeupConfigManager_Base;
public abstract class _IWakeupConfigManager_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bpc.IWakeupConfigManager.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bpc.IWakeupConfigManager.Unknown1");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Bpc.IWakeupConfigManager.Unknown2 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IWakeupConfigManager");
		}
	}
}

