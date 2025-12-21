using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Btm;
public partial class IBtm : _IBtm_Base;
public abstract class _IBtm_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown1 not implemented");
	protected virtual void RegisterSystemEventForConnectedDeviceConditionImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtm.RegisterSystemEventForConnectedDeviceConditionImpl not implemented");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown5");
	protected virtual void Unknown6(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown6");
	protected virtual void Unknown7(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown7");
	protected virtual void RegisterSystemEventForRegisteredDeviceInfoImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtm.RegisterSystemEventForRegisteredDeviceInfoImpl not implemented");
	protected virtual void Unknown9() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown9 not implemented");
	protected virtual void Unknown10(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown10");
	protected virtual void Unknown11(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown11");
	protected virtual void Unknown12(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown12");
	protected virtual void Unknown13(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown13");
	protected virtual void EnableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.EnableRadioImpl");
	protected virtual void DisableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.DisableRadioImpl");
	protected virtual void Unknown16(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown16");
	protected virtual void Unknown17(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown17");
	protected virtual void Unknown18() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown18 not implemented");
	protected virtual void Unknown19() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown19 not implemented");
	protected virtual void Unknown20() =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown20 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown21");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // RegisterSystemEventForConnectedDeviceConditionImpl
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
			case 0x8: // RegisterSystemEventForRegisteredDeviceInfoImpl
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // EnableRadioImpl
				break;
			case 0xF: // DisableRadioImpl
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			case 0x12: // Unknown18
				break;
			case 0x13: // Unknown19
				break;
			case 0x14: // Unknown20
				break;
			case 0x15: // Unknown21
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtm");
		}
	}
}

public partial class IBtmDebug : _IBtmDebug_Base;
public abstract class _IBtmDebug_Base : IpcInterface {
	protected virtual void RegisterSystemEventForDiscoveryImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmDebug.RegisterSystemEventForDiscoveryImpl not implemented");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown2");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Btm.IBtmDebug.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown5");
	protected virtual void Unknown6(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown6");
	protected virtual void Unknown7(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown7");
	protected virtual void Unknown8(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown8");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterSystemEventForDiscoveryImpl
				break;
			case 0x1: // Unknown1
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmDebug");
		}
	}
}

public partial class IBtmSystem : _IBtmSystem_Base;
public abstract class _IBtmSystem_Base : IpcInterface {
	protected virtual Nn.Btm.IBtmSystemCore GetCoreImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystem.GetCoreImpl not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCoreImpl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmSystem");
		}
	}
}

public partial class IBtmSystemCore : _IBtmSystemCore_Base;
public abstract class _IBtmSystemCore_Base : IpcInterface {
	protected virtual void StartGamepadPairingImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.StartGamepadPairingImpl");
	protected virtual void CancelGamepadPairingImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.CancelGamepadPairingImpl");
	protected virtual void ClearGamepadPairingDatabaseImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.ClearGamepadPairingDatabaseImpl");
	protected virtual void GetPairedGamepadCountImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.GetPairedGamepadCountImpl not implemented");
	protected virtual void EnableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.EnableRadioImpl");
	protected virtual void DisableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.DisableRadioImpl");
	protected virtual void GetRadioOnOffImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.GetRadioOnOffImpl not implemented");
	protected virtual void AcquireRadioEventImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.AcquireRadioEventImpl not implemented");
	protected virtual void AcquireGamepadPairingEventImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.AcquireGamepadPairingEventImpl not implemented");
	protected virtual void IsGamepadPairingStartedImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.IsGamepadPairingStartedImpl not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // StartGamepadPairingImpl
				break;
			case 0x1: // CancelGamepadPairingImpl
				break;
			case 0x2: // ClearGamepadPairingDatabaseImpl
				break;
			case 0x3: // GetPairedGamepadCountImpl
				break;
			case 0x4: // EnableRadioImpl
				break;
			case 0x5: // DisableRadioImpl
				break;
			case 0x6: // GetRadioOnOffImpl
				break;
			case 0x7: // AcquireRadioEventImpl
				break;
			case 0x8: // AcquireGamepadPairingEventImpl
				break;
			case 0x9: // IsGamepadPairingStartedImpl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmSystemCore");
		}
	}
}

