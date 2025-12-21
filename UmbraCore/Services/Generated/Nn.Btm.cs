using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Btm;
public partial class IBtm : _IBtm_Base;
public abstract class _IBtm_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown1 not implemented");
	protected virtual void RegisterSystemEventForConnectedDeviceConditionImpl(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtm.RegisterSystemEventForConnectedDeviceConditionImpl not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown5");
	protected virtual void Unknown6(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown6");
	protected virtual void Unknown7(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown7");
	protected virtual void RegisterSystemEventForRegisteredDeviceInfoImpl(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtm.RegisterSystemEventForRegisteredDeviceInfoImpl not implemented");
	protected virtual void Unknown9(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown9 not implemented");
	protected virtual void Unknown10(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown10");
	protected virtual void Unknown11(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown11");
	protected virtual void Unknown12(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown12");
	protected virtual void Unknown13(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown13");
	protected virtual void EnableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.EnableRadioImpl");
	protected virtual void DisableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.DisableRadioImpl");
	protected virtual void Unknown16(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown16");
	protected virtual void Unknown17(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown17");
	protected virtual void Unknown18(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown18 not implemented");
	protected virtual void Unknown19(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown19 not implemented");
	protected virtual void Unknown20(out byte[] _0) =>
		throw new NotImplementedException("Nn.Btm.IBtm.Unknown20 not implemented");
	protected virtual void Unknown21(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtm.Unknown21");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 42);
				Unknown1(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // RegisterSystemEventForConnectedDeviceConditionImpl
				om.Initialize(0, 1, 1);
				RegisterSystemEventForConnectedDeviceConditionImpl(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4(im.GetBytes(8, 0x7));
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 0);
				Unknown6(im.GetBytes(8, 0x4));
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 0);
				Unknown7(im.GetBytes(8, 0x4));
				break;
			}
			case 0x8: { // RegisterSystemEventForRegisteredDeviceInfoImpl
				om.Initialize(0, 1, 1);
				RegisterSystemEventForRegisteredDeviceInfoImpl(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x9: { // Unknown9
				om.Initialize(0, 0, 0);
				Unknown9(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 0);
				Unknown10(im.GetBytes(8, 0x60));
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 0, 0);
				Unknown11(im.GetBytes(8, 0x6));
				break;
			}
			case 0xC: { // Unknown12
				om.Initialize(0, 0, 0);
				Unknown12(im.GetBytes(8, 0x6));
				break;
			}
			case 0xD: { // Unknown13
				om.Initialize(0, 0, 0);
				Unknown13(im.GetBytes(8, 0x6));
				break;
			}
			case 0xE: { // EnableRadioImpl
				om.Initialize(0, 0, 0);
				EnableRadioImpl();
				break;
			}
			case 0xF: { // DisableRadioImpl
				om.Initialize(0, 0, 0);
				DisableRadioImpl();
				break;
			}
			case 0x10: { // Unknown16
				om.Initialize(0, 0, 0);
				Unknown16(im.GetBytes(8, 0x6));
				break;
			}
			case 0x11: { // Unknown17
				om.Initialize(0, 0, 0);
				Unknown17(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x12: { // Unknown18
				om.Initialize(0, 1, 1);
				Unknown18(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x13: { // Unknown19
				om.Initialize(0, 1, 1);
				Unknown19(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x14: { // Unknown20
				om.Initialize(0, 0, 1);
				Unknown20(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				om.Initialize(0, 0, 0);
				Unknown21(im.GetBytes(8, 0x1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtm");
		}
	}
}

public partial class IBtmDebug : _IBtmDebug_Base;
public abstract class _IBtmDebug_Base : IpcInterface {
	protected virtual void RegisterSystemEventForDiscoveryImpl(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtmDebug.RegisterSystemEventForDiscoveryImpl not implemented");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Btm.IBtmDebug.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown4");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown5");
	protected virtual void Unknown6(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown6");
	protected virtual void Unknown7(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown7");
	protected virtual void Unknown8(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmDebug.Unknown8");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterSystemEventForDiscoveryImpl
				om.Initialize(0, 1, 1);
				RegisterSystemEventForDiscoveryImpl(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4(im.GetBytes(8, 0x6));
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetBytes(8, 0x6));
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 0);
				Unknown6(im.GetBytes(8, 0xC));
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 0);
				Unknown7(im.GetBytes(8, 0x4));
				break;
			}
			case 0x8: { // Unknown8
				om.Initialize(0, 0, 0);
				Unknown8(im.GetBytes(8, 0x6));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmDebug");
		}
	}
}

public partial class IBtmSystem : _IBtmSystem_Base;
public abstract class _IBtmSystem_Base : IpcInterface {
	protected virtual Nn.Btm.IBtmSystemCore GetCoreImpl() =>
		throw new NotImplementedException("Nn.Btm.IBtmSystem.GetCoreImpl not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCoreImpl
				om.Initialize(1, 0, 0);
				var _return = GetCoreImpl();
				om.Move(0, CreateHandle(_return));
				break;
			}
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
	protected virtual void GetPairedGamepadCountImpl(out byte[] _0) =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.GetPairedGamepadCountImpl not implemented");
	protected virtual void EnableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.EnableRadioImpl");
	protected virtual void DisableRadioImpl() =>
		Console.WriteLine("Stub hit for Nn.Btm.IBtmSystemCore.DisableRadioImpl");
	protected virtual void GetRadioOnOffImpl(out byte[] _0) =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.GetRadioOnOffImpl not implemented");
	protected virtual void AcquireRadioEventImpl(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.AcquireRadioEventImpl not implemented");
	protected virtual void AcquireGamepadPairingEventImpl(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.AcquireGamepadPairingEventImpl not implemented");
	protected virtual void IsGamepadPairingStartedImpl(out byte[] _0) =>
		throw new NotImplementedException("Nn.Btm.IBtmSystemCore.IsGamepadPairingStartedImpl not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // StartGamepadPairingImpl
				om.Initialize(0, 0, 0);
				StartGamepadPairingImpl();
				break;
			}
			case 0x1: { // CancelGamepadPairingImpl
				om.Initialize(0, 0, 0);
				CancelGamepadPairingImpl();
				break;
			}
			case 0x2: { // ClearGamepadPairingDatabaseImpl
				om.Initialize(0, 0, 0);
				ClearGamepadPairingDatabaseImpl();
				break;
			}
			case 0x3: { // GetPairedGamepadCountImpl
				om.Initialize(0, 0, 1);
				GetPairedGamepadCountImpl(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // EnableRadioImpl
				om.Initialize(0, 0, 0);
				EnableRadioImpl();
				break;
			}
			case 0x5: { // DisableRadioImpl
				om.Initialize(0, 0, 0);
				DisableRadioImpl();
				break;
			}
			case 0x6: { // GetRadioOnOffImpl
				om.Initialize(0, 0, 1);
				GetRadioOnOffImpl(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // AcquireRadioEventImpl
				om.Initialize(0, 1, 1);
				AcquireRadioEventImpl(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x8: { // AcquireGamepadPairingEventImpl
				om.Initialize(0, 1, 1);
				AcquireGamepadPairingEventImpl(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x9: { // IsGamepadPairingStartedImpl
				om.Initialize(0, 0, 1);
				IsGamepadPairingStartedImpl(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmSystemCore");
		}
	}
}

