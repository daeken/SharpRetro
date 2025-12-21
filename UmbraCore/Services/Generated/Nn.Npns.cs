using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Npns;
public partial class INpnsSystem : _INpnsSystem_Base;
public abstract class _INpnsSystem_Base : IpcInterface {
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown4 not implemented");
	protected virtual KObject Unknown5() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown5 not implemented");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown6");
	protected virtual KObject Unknown7() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown7 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown11");
	protected virtual void Unknown12(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown12");
	protected virtual void Unknown13(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown13 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown21 not implemented");
	protected virtual void Unknown22(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown22 not implemented");
	protected virtual void Unknown23(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown23");
	protected virtual void Unknown24(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown24");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown25 not implemented");
	protected virtual void Unknown31(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown31");
	protected virtual void Unknown32(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown32");
	protected virtual void Unknown101() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown101");
	protected virtual void Unknown102() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown102");
	protected virtual void Unknown103() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown103 not implemented");
	protected virtual void Unknown104() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown104 not implemented");
	protected virtual KObject Unknown105() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown105 not implemented");
	protected virtual void Unknown111() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown111 not implemented");
	protected virtual void Unknown112() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown112");
	protected virtual void Unknown113() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown113");
	protected virtual void Unknown114(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown114");
	protected virtual void Unknown115() =>
		throw new NotImplementedException("Nn.Npns.INpnsSystem.Unknown115 not implemented");
	protected virtual void Unknown201(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown201");
	protected virtual void Unknown202(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsSystem.Unknown202");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
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
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			case 0x18: // Unknown24
				break;
			case 0x19: // Unknown25
				break;
			case 0x1F: // Unknown31
				break;
			case 0x20: // Unknown32
				break;
			case 0x65: // Unknown101
				break;
			case 0x66: // Unknown102
				break;
			case 0x67: // Unknown103
				break;
			case 0x68: // Unknown104
				break;
			case 0x69: // Unknown105
				break;
			case 0x6F: // Unknown111
				break;
			case 0x70: // Unknown112
				break;
			case 0x71: // Unknown113
				break;
			case 0x72: // Unknown114
				break;
			case 0x73: // Unknown115
				break;
			case 0xC9: // Unknown201
				break;
			case 0xCA: // Unknown202
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Npns.INpnsSystem");
		}
	}
}

public partial class INpnsUser : _INpnsUser_Base;
public abstract class _INpnsUser_Base : IpcInterface {
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsUser.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsUser.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown4 not implemented");
	protected virtual KObject Unknown5() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown5 not implemented");
	protected virtual KObject Unknown7() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown7 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown21 not implemented");
	protected virtual void Unknown23(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsUser.Unknown23");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown25 not implemented");
	protected virtual void Unknown101() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsUser.Unknown101");
	protected virtual void Unknown102() =>
		Console.WriteLine("Stub hit for Nn.Npns.INpnsUser.Unknown102");
	protected virtual void Unknown103() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown103 not implemented");
	protected virtual void Unknown104() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown104 not implemented");
	protected virtual void Unknown111() =>
		throw new NotImplementedException("Nn.Npns.INpnsUser.Unknown111 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
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
			case 0x7: // Unknown7
				break;
			case 0x15: // Unknown21
				break;
			case 0x17: // Unknown23
				break;
			case 0x19: // Unknown25
				break;
			case 0x65: // Unknown101
				break;
			case 0x66: // Unknown102
				break;
			case 0x67: // Unknown103
				break;
			case 0x68: // Unknown104
				break;
			case 0x6F: // Unknown111
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Npns.INpnsUser");
		}
	}
}

