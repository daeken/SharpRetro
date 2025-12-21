using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sasbus;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Sasbus.ISession OpenSession(byte[] _0) =>
		throw new NotImplementedException("Nn.Sasbus.IManager.OpenSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				om.Initialize(1, 0, 0);
				var _return = OpenSession(im.GetBytes(8, 0x4));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sasbus.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Sasbus.ISession.Unknown0");
	protected virtual void Unknown1(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Sasbus.ISession.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Sasbus.ISession.Unknown2");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Sasbus.ISession.Unknown3");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x21, 0));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x22, 0));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetBytes(8, 0x18), Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sasbus.ISession");
		}
	}
}

