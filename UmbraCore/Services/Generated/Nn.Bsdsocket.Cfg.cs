using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bsdsocket.Cfg;
public partial class ServerInterface : _ServerInterface_Base;
public abstract class _ServerInterface_Base : IpcInterface {
	protected virtual void SetIfUp(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.SetIfUp");
	protected virtual KObject SetIfUpWithEvent(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.SetIfUpWithEvent not implemented");
	protected virtual void CancelIf(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.CancelIf");
	protected virtual void SetIfDown(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.SetIfDown");
	protected virtual void GetIfState(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.GetIfState not implemented");
	protected virtual void DhcpRenew(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.DhcpRenew");
	protected virtual void AddStaticArpEntry(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.AddStaticArpEntry");
	protected virtual void RemoveArpEntry(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.RemoveArpEntry");
	protected virtual void LookupArpEntry(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.LookupArpEntry not implemented");
	protected virtual void LookupArpEntry2(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bsdsocket.Cfg.ServerInterface.LookupArpEntry2 not implemented");
	protected virtual void ClearArpEntries() =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.ClearArpEntries");
	protected virtual void ClearArpEntries2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.ClearArpEntries2");
	protected virtual void PrintArpEntries() =>
		Console.WriteLine("Stub hit for Nn.Bsdsocket.Cfg.ServerInterface.PrintArpEntries");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetIfUp
				om.Initialize(0, 0, 0);
				SetIfUp(im.GetBytes(8, 0x2C), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x1: { // SetIfUpWithEvent
				om.Initialize(0, 1, 0);
				var _return = SetIfUpWithEvent(im.GetBytes(8, 0x2C), im.GetSpan<byte>(0x5, 0));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // CancelIf
				om.Initialize(0, 0, 0);
				CancelIf(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x3: { // SetIfDown
				om.Initialize(0, 0, 0);
				SetIfDown(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x4: { // GetIfState
				om.Initialize(0, 0, 0);
				GetIfState(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x5: { // DhcpRenew
				om.Initialize(0, 0, 0);
				DhcpRenew(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x6: { // AddStaticArpEntry
				om.Initialize(0, 0, 0);
				AddStaticArpEntry(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x7: { // RemoveArpEntry
				om.Initialize(0, 0, 0);
				RemoveArpEntry(im.GetBytes(8, 0x4));
				break;
			}
			case 0x8: { // LookupArpEntry
				om.Initialize(0, 0, 0);
				LookupArpEntry(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x9: { // LookupArpEntry2
				om.Initialize(0, 0, 0);
				LookupArpEntry2(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xA: { // ClearArpEntries
				om.Initialize(0, 0, 0);
				ClearArpEntries();
				break;
			}
			case 0xB: { // ClearArpEntries2
				om.Initialize(0, 0, 0);
				ClearArpEntries2(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0xC: { // PrintArpEntries
				om.Initialize(0, 0, 0);
				PrintArpEntries();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bsdsocket.Cfg.ServerInterface");
		}
	}
}

