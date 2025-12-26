using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Npns;
public partial class INpnsSystem : _INpnsSystem_Base {
	public readonly string ServiceName;
	public INpnsSystem(string serviceName) => ServiceName = serviceName;
}
public abstract class _INpnsSystem_Base : IpcInterface {
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown1".Log();
	protected virtual void Unknown2(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown2".Log();
	protected virtual void Unknown3(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown4 not implemented");
	protected virtual KObject Unknown5() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown5 not implemented");
	protected virtual void Unknown6() =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown6".Log();
	protected virtual KObject Unknown7() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown7 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown11".Log();
	protected virtual void Unknown12(Span<byte> _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown12".Log();
	protected virtual void Unknown13(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown13 not implemented");
	protected virtual void Unknown21(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown21 not implemented");
	protected virtual void Unknown22(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown22 not implemented");
	protected virtual void Unknown23(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown23".Log();
	protected virtual void Unknown24(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown24".Log();
	protected virtual void Unknown25(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown25 not implemented");
	protected virtual void Unknown31(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown31".Log();
	protected virtual void Unknown32(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown32".Log();
	protected virtual void Unknown101() =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown101".Log();
	protected virtual void Unknown102() =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown102".Log();
	protected virtual void Unknown103(out byte[] _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown103 not implemented");
	protected virtual void Unknown104(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown104 not implemented");
	protected virtual KObject Unknown105() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown105 not implemented");
	protected virtual void Unknown111(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown111 not implemented");
	protected virtual void Unknown112() =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown112".Log();
	protected virtual void Unknown113() =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown113".Log();
	protected virtual void Unknown114(Span<byte> _0, Span<byte> _1) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown114".Log();
	protected virtual void Unknown115(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown115 not implemented");
	protected virtual void Unknown201(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown201".Log();
	protected virtual void Unknown202(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsSystem.Unknown202".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x2), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x2), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				var _return = Unknown5();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6: { // Unknown6
				Unknown6();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				var _return = Unknown7();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // Unknown13
				Unknown13(im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // Unknown23
				Unknown23(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(im.GetBytes(8, 0x28), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1F: { // Unknown31
				Unknown31(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x20: { // Unknown32
				Unknown32(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // Unknown101
				Unknown101();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // Unknown102
				Unknown102();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // Unknown103
				Unknown103(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x68: { // Unknown104
				Unknown104(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x69: { // Unknown105
				var _return = Unknown105();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6F: { // Unknown111
				Unknown111(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x70: { // Unknown112
				Unknown112();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x71: { // Unknown113
				Unknown113();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x72: { // Unknown114
				Unknown114(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x73: { // Unknown115
				Unknown115(im.GetSpan<byte>(0xA, 0), im.GetSpan<byte>(0xA, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC9: { // Unknown201
				Unknown201(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // Unknown202
				Unknown202(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Npns.INpnsSystem");
		}
	}
}

public partial class INpnsUser : _INpnsUser_Base {
	public readonly string ServiceName;
	public INpnsUser(string serviceName) => ServiceName = serviceName;
}
public abstract class _INpnsUser_Base : IpcInterface {
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Npns.INpnsUser.Unknown1".Log();
	protected virtual void Unknown2(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsUser.Unknown2".Log();
	protected virtual void Unknown3(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown4 not implemented");
	protected virtual KObject Unknown5() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown5 not implemented");
	protected virtual KObject Unknown7() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown7 not implemented");
	protected virtual void Unknown21(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown21 not implemented");
	protected virtual void Unknown23(byte[] _0) =>
		"Stub hit for Nn.Npns.INpnsUser.Unknown23".Log();
	protected virtual void Unknown25(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown25 not implemented");
	protected virtual void Unknown101() =>
		"Stub hit for Nn.Npns.INpnsUser.Unknown101".Log();
	protected virtual void Unknown102() =>
		"Stub hit for Nn.Npns.INpnsUser.Unknown102".Log();
	protected virtual void Unknown103(out byte[] _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown103 not implemented");
	protected virtual void Unknown104(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown104 not implemented");
	protected virtual void Unknown111(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown111 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x2), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x2), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				var _return = Unknown5();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x7: { // Unknown7
				var _return = Unknown7();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // Unknown23
				Unknown23(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(im.GetBytes(8, 0x28), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x65: { // Unknown101
				Unknown101();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // Unknown102
				Unknown102();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // Unknown103
				Unknown103(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x68: { // Unknown104
				Unknown104(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // Unknown111
				Unknown111(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Npns.INpnsUser");
		}
	}
}

