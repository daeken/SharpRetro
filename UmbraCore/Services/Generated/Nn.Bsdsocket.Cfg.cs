using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bsdsocket.Cfg;
public partial class ServerInterface : _ServerInterface_Base;
public abstract class _ServerInterface_Base : IpcInterface {
	protected virtual void SetIfUp(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.SetIfUp");
	protected virtual KObject SetIfUpWithEvent(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.SetIfUpWithEvent not implemented");
	protected virtual void CancelIf(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.CancelIf");
	protected virtual void SetIfDown(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.SetIfDown");
	protected virtual void GetIfState(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.GetIfState not implemented");
	protected virtual void DhcpRenew(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.DhcpRenew");
	protected virtual void AddStaticArpEntry(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.AddStaticArpEntry");
	protected virtual void RemoveArpEntry(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.RemoveArpEntry");
	protected virtual void LookupArpEntry(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.LookupArpEntry not implemented");
	protected virtual void LookupArpEntry2(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.LookupArpEntry2 not implemented");
	protected virtual void ClearArpEntries() =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.ClearArpEntries");
	protected virtual void ClearArpEntries2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.ClearArpEntries2");
	protected virtual void PrintArpEntries() =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.PrintArpEntries");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetIfUp
				break;
			}
			case 0x1: { // SetIfUpWithEvent
				break;
			}
			case 0x2: { // CancelIf
				break;
			}
			case 0x3: { // SetIfDown
				break;
			}
			case 0x4: { // GetIfState
				break;
			}
			case 0x5: { // DhcpRenew
				break;
			}
			case 0x6: { // AddStaticArpEntry
				break;
			}
			case 0x7: { // RemoveArpEntry
				break;
			}
			case 0x8: { // LookupArpEntry
				break;
			}
			case 0x9: { // LookupArpEntry2
				break;
			}
			case 0xA: { // ClearArpEntries
				break;
			}
			case 0xB: { // ClearArpEntries2
				break;
			}
			case 0xC: { // PrintArpEntries
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bsdsocket.Cfg.ServerInterface");
		}
	}
}

