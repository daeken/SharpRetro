using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bsdsocket.Cfg;
public partial class ServerInterface : _ServerInterface_Base;
public abstract class _ServerInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetIfUp
				break;
			case 0x1: // SetIfUpWithEvent
				break;
			case 0x2: // CancelIf
				break;
			case 0x3: // SetIfDown
				break;
			case 0x4: // GetIfState
				break;
			case 0x5: // DhcpRenew
				break;
			case 0x6: // AddStaticArpEntry
				break;
			case 0x7: // RemoveArpEntry
				break;
			case 0x8: // LookupArpEntry
				break;
			case 0x9: // LookupArpEntry2
				break;
			case 0xA: // ClearArpEntries
				break;
			case 0xB: // ClearArpEntries2
				break;
			case 0xC: // PrintArpEntries
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bsdsocket.Cfg.ServerInterface");
		}
	}
}

