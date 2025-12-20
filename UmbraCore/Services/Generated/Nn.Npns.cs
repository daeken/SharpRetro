using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Npns;
public partial class INpnsSystem : _INpnsSystem_Base;
public abstract class _INpnsSystem_Base : IpcInterface {
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

