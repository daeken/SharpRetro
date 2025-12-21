using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Cec;
public partial class CecManagerSubinterface100 : _CecManagerSubinterface100_Base;
public abstract class _CecManagerSubinterface100_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Cec.CecManagerSubinterface100.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Cec.CecManagerSubinterface100.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Cec.CecManagerSubinterface100.Unknown2");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Cec.CecManagerSubinterface100.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 1, 0);
				var _return = Unknown0();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 4);
				Unknown1(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetBytes(8, 0x4));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 4);
				Unknown3(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Cec.CecManagerSubinterface100");
		}
	}
}

public partial class ICecManager : _ICecManager_Base;
public abstract class _ICecManager_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Cec.ICecManager.Unknown2");
	protected virtual void Unknown3(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown4 not implemented");
	protected virtual void Unknown5(out byte[] _0) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown5 not implemented");
	protected virtual void Unknown6(out byte[] _0) =>
		throw new NotImplementedException("Nn.Cec.ICecManager.Unknown6 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 1, 8);
				Unknown0(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 4);
				Unknown1(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetBytes(8, 0x4));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 4);
				Unknown3(im.GetBytes(8, 0x18), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 24);
				Unknown4(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 32);
				Unknown5(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 8);
				Unknown6(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Cec.ICecManager");
		}
	}
}

