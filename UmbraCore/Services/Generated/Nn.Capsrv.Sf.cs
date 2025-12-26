using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Capsrv.Sf;
public partial class IAlbumAccessorService : _IAlbumAccessorService_Base {
	public readonly string ServiceName;
	public IAlbumAccessorService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAlbumAccessorService_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown3".Log();
	protected virtual void Unknown4(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown4".Log();
	protected virtual void Unknown5(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown5 not implemented");
	protected virtual void Unknown6(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown6 not implemented");
	protected virtual void Unknown7(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown7 not implemented");
	protected virtual void Unknown8(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8 not implemented");
	protected virtual void Unknown9(byte[] _0, out byte[] _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown9 not implemented");
	protected virtual void Unknown10(byte[] _0, out byte[] _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown10 not implemented");
	protected virtual void Unknown11(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown11 not implemented");
	protected virtual void Unknown12(byte[] _0, out byte[] _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown12 not implemented");
	protected virtual void Unknown13(byte[] _0, out byte[] _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown13 not implemented");
	protected virtual void Unknown14(byte[] _0, out byte[] _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown14 not implemented");
	protected virtual void Unknown301(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown301 not implemented");
	protected virtual void Unknown401(out byte[] _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown401 not implemented");
	protected virtual void Unknown501(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown501 not implemented");
	protected virtual void Unknown1001(byte[] _0, out byte[] _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown1001 not implemented");
	protected virtual void Unknown1002(byte[] _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown1002 not implemented");
	protected virtual void Unknown8001(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8001".Log();
	protected virtual void Unknown8002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8002".Log();
	protected virtual void Unknown8011(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8011".Log();
	protected virtual void Unknown8012(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8012 not implemented");
	protected virtual void Unknown8021(byte[] _0, ulong _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8021 not implemented");
	protected virtual void Unknown10011(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown10011".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x1), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 48);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(im.GetBytes(8, 0x28), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12(im.GetBytes(8, 0x38), out var _0, im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				Unknown13(im.GetBytes(8, 0x38), out var _0, im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetBytes(8, 0x38), out var _0, im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 80);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12D: { // Unknown301
				Unknown301(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x191: { // Unknown401
				Unknown401(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1F5: { // Unknown501
				Unknown501(im.GetBytes(8, 0x2), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3E9: { // Unknown1001
				Unknown1001(im.GetBytes(8, 0x38), out var _0, im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 80);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3EA: { // Unknown1002
				Unknown1002(im.GetBytes(8, 0x38), im.GetSpan<byte>(0x16, 0), im.GetSpan<byte>(0x46, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F41: { // Unknown8001
				Unknown8001(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F42: { // Unknown8002
				Unknown8002(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F4B: { // Unknown8011
				Unknown8011(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F4C: { // Unknown8012
				Unknown8012(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1F55: { // Unknown8021
				Unknown8021(im.GetBytes(8, 0x28), im.Pid, out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x271B: { // Unknown10011
				Unknown10011(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumAccessorService");
		}
	}
}

public partial class IAlbumAccessorSession : _IAlbumAccessorSession_Base;
public abstract class _IAlbumAccessorSession_Base : IpcInterface {
	protected virtual void Unknown2001(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2001 not implemented");
	protected virtual void Unknown2002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2002".Log();
	protected virtual void Unknown2003(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2003 not implemented");
	protected virtual void Unknown2004(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2004 not implemented");
	protected virtual void Unknown2005(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2005".Log();
	protected virtual void Unknown2006(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2006 not implemented");
	protected virtual void Unknown2007(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2007 not implemented");
	protected virtual void Unknown2008(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2008 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: { // Unknown2001
				Unknown2001(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D2: { // Unknown2002
				Unknown2002(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D3: { // Unknown2003
				Unknown2003(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D4: { // Unknown2004
				Unknown2004(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D5: { // Unknown2005
				Unknown2005(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D6: { // Unknown2006
				Unknown2006(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D7: { // Unknown2007
				Unknown2007(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D8: { // Unknown2008
				Unknown2008(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 64);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumAccessorSession");
		}
	}
}

public partial class IAlbumControlService : _IAlbumControlService_Base {
	public readonly string ServiceName;
	public IAlbumControlService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAlbumControlService_Base : IpcInterface {
	protected virtual void Unknown2001(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2001".Log();
	protected virtual void Unknown2002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2002".Log();
	protected virtual void Unknown2011(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2011".Log();
	protected virtual void Unknown2012(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2012".Log();
	protected virtual void Unknown2013(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlService.Unknown2013 not implemented");
	protected virtual void Unknown2014(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2014".Log();
	protected virtual void Unknown2101(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlService.Unknown2101 not implemented");
	protected virtual void Unknown2102(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlService.Unknown2102 not implemented");
	protected virtual void Unknown2201(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2201".Log();
	protected virtual void Unknown2301(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2301".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: { // Unknown2001
				Unknown2001(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D2: { // Unknown2002
				Unknown2002(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7DB: { // Unknown2011
				Unknown2011(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7DC: { // Unknown2012
				Unknown2012(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7DD: { // Unknown2013
				Unknown2013(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7DE: { // Unknown2014
				Unknown2014(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x835: { // Unknown2101
				Unknown2101(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x836: { // Unknown2102
				Unknown2102(im.GetBytes(8, 0x28), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x899: { // Unknown2201
				Unknown2201(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x45, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8FD: { // Unknown2301
				Unknown2301(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x45, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumControlService");
		}
	}
}

public partial class IAlbumControlSession : _IAlbumControlSession_Base;
public abstract class _IAlbumControlSession_Base : IpcInterface {
	protected virtual void Unknown2001(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2001 not implemented");
	protected virtual void Unknown2002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2002".Log();
	protected virtual void Unknown2003(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2003 not implemented");
	protected virtual void Unknown2004(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2004 not implemented");
	protected virtual void Unknown2005(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2005".Log();
	protected virtual void Unknown2006(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2006 not implemented");
	protected virtual void Unknown2007(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2007 not implemented");
	protected virtual void Unknown2008(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2008 not implemented");
	protected virtual void Unknown2401(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2401 not implemented");
	protected virtual void Unknown2402(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2402".Log();
	protected virtual void Unknown2403(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2403".Log();
	protected virtual void Unknown2404(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2404".Log();
	protected virtual void Unknown2405(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2405".Log();
	protected virtual void Unknown2411(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2411".Log();
	protected virtual void Unknown2412(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2412".Log();
	protected virtual void Unknown2413(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2413".Log();
	protected virtual void Unknown2414(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2414".Log();
	protected virtual void Unknown2421(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2421 not implemented");
	protected virtual void Unknown2422(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2422".Log();
	protected virtual void Unknown2424(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2424".Log();
	protected virtual void Unknown2431(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2431".Log();
	protected virtual void Unknown2433(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2433 not implemented");
	protected virtual void Unknown2434(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2434".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: { // Unknown2001
				Unknown2001(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D2: { // Unknown2002
				Unknown2002(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D3: { // Unknown2003
				Unknown2003(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D4: { // Unknown2004
				Unknown2004(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D5: { // Unknown2005
				Unknown2005(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D6: { // Unknown2006
				Unknown2006(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D7: { // Unknown2007
				Unknown2007(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7D8: { // Unknown2008
				Unknown2008(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 64);
				om.SetBytes(8, _0);
				break;
			}
			case 0x961: { // Unknown2401
				Unknown2401(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x962: { // Unknown2402
				Unknown2402(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x963: { // Unknown2403
				Unknown2403(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x964: { // Unknown2404
				Unknown2404(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x965: { // Unknown2405
				Unknown2405(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x96B: { // Unknown2411
				Unknown2411(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x96C: { // Unknown2412
				Unknown2412(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x96D: { // Unknown2413
				Unknown2413(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x96E: { // Unknown2414
				Unknown2414(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x975: { // Unknown2421
				Unknown2421(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x976: { // Unknown2422
				Unknown2422(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x978: { // Unknown2424
				Unknown2424(im.GetBytes(8, 0x20), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x97F: { // Unknown2431
				Unknown2431(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x981: { // Unknown2433
				Unknown2433(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x982: { // Unknown2434
				Unknown2434(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumControlSession");
		}
	}
}

public partial class ICaptureControllerService : _ICaptureControllerService_Base;
public abstract class _ICaptureControllerService_Base : IpcInterface {
	protected virtual void Unknown1(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.ICaptureControllerService.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.ICaptureControllerService.Unknown2 not implemented");
	protected virtual void Unknown1001(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown1001".Log();
	protected virtual void Unknown1002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown1002".Log();
	protected virtual void Unknown1011(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown1011".Log();
	protected virtual void Unknown2001(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown2001".Log();
	protected virtual void Unknown2002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown2002".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x20), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x28), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // Unknown1001
				Unknown1001(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EA: { // Unknown1002
				Unknown1002(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F3: { // Unknown1011
				Unknown1011(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D1: { // Unknown2001
				Unknown2001(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D2: { // Unknown2002
				Unknown2002(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.ICaptureControllerService");
		}
	}
}

public partial class IScreenShotApplicationService : _IScreenShotApplicationService_Base {
	public readonly string ServiceName;
	public IScreenShotApplicationService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IScreenShotApplicationService_Base : IpcInterface {
	protected virtual void SaveScreenShot(uint _0, uint _1, ulong _2, ulong _3, Span<byte> _4, out byte[] _5) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotApplicationService.SaveScreenShot not implemented");
	protected virtual void SaveScreenShotEx0(byte[] _0, uint _1, ulong _2, ulong _3, Span<byte> _4, out byte[] _5) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotApplicationService.SaveScreenShotEx0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xC9: { // SaveScreenShot
				SaveScreenShot(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x45, 0), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xCB: { // SaveScreenShotEx0
				SaveScreenShotEx0(im.GetBytes(8, 0x40), im.GetData<uint>(72), im.GetData<ulong>(80), im.Pid, im.GetSpan<byte>(0x45, 0), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IScreenShotApplicationService");
		}
	}
}

public partial class IScreenShotControlService : _IScreenShotControlService_Base {
	public readonly string ServiceName;
	public IScreenShotControlService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IScreenShotControlService_Base : IpcInterface {
	protected virtual void Unknown1(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown2 not implemented");
	protected virtual void Unknown1001(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1001".Log();
	protected virtual void Unknown1002(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1002".Log();
	protected virtual void Unknown1003(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1003".Log();
	protected virtual void Unknown1011(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1011".Log();
	protected virtual void Unknown1012(byte[] _0) =>
		"Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1012".Log();
	protected virtual void Unknown1201(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown1201 not implemented");
	protected virtual void Unknown1202() =>
		"Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1202".Log();
	protected virtual void Unknown1203(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown1203 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x28), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x30), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // Unknown1001
				Unknown1001(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EA: { // Unknown1002
				Unknown1002(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EB: { // Unknown1003
				Unknown1003(im.GetBytes(8, 0x58));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F3: { // Unknown1011
				Unknown1011(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F4: { // Unknown1012
				Unknown1012(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B1: { // Unknown1201
				Unknown1201(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4B2: { // Unknown1202
				Unknown1202();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B3: { // Unknown1203
				Unknown1203(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IScreenShotControlService");
		}
	}
}

public partial class IScreenShotService : _IScreenShotService_Base {
	public readonly string ServiceName;
	public IScreenShotService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IScreenShotService_Base : IpcInterface {
	protected virtual void Unknown201(byte[] _0, ulong _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown201 not implemented");
	protected virtual void Unknown202(byte[] _0, Span<byte> _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown202 not implemented");
	protected virtual void Unknown203(byte[] _0, ulong _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown203 not implemented");
	protected virtual void Unknown204(byte[] _0, Span<byte> _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown204 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xC9: { // Unknown201
				Unknown201(im.GetBytes(8, 0x10), im.Pid, im.GetSpan<byte>(0x45, 0), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xCA: { // Unknown202
				Unknown202(im.GetBytes(8, 0x38), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x45, 1), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xCB: { // Unknown203
				Unknown203(im.GetBytes(8, 0x50), im.Pid, im.GetSpan<byte>(0x45, 0), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xCC: { // Unknown204
				Unknown204(im.GetBytes(8, 0x78), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x45, 1), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IScreenShotService");
		}
	}
}

public partial class IAlbumAccessorApplicationSession : _IAlbumAccessorApplicationSession_Base;
public abstract class _IAlbumAccessorApplicationSession_Base : IpcInterface {
	protected virtual void OpenAlbumMovieReadStream() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.OpenAlbumMovieReadStream".Log();
	protected virtual void CloseAlbumMovieReadStream() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.CloseAlbumMovieReadStream".Log();
	protected virtual void GetAlbumMovieReadStreamMovieDataSize() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.GetAlbumMovieReadStreamMovieDataSize".Log();
	protected virtual void ReadMovieDataFromAlbumMovieReadStream() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.ReadMovieDataFromAlbumMovieReadStream".Log();
	protected virtual void GetAlbumMovieReadStreamBrokenReason() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.GetAlbumMovieReadStreamBrokenReason".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: { // OpenAlbumMovieReadStream
				OpenAlbumMovieReadStream();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D2: { // CloseAlbumMovieReadStream
				CloseAlbumMovieReadStream();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D3: { // GetAlbumMovieReadStreamMovieDataSize
				GetAlbumMovieReadStreamMovieDataSize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D4: { // ReadMovieDataFromAlbumMovieReadStream
				ReadMovieDataFromAlbumMovieReadStream();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7D5: { // GetAlbumMovieReadStreamBrokenReason
				GetAlbumMovieReadStreamBrokenReason();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumAccessorApplicationSession");
		}
	}
}

public partial class IAlbumApplicationService : _IAlbumApplicationService_Base {
	public readonly string ServiceName;
	public IAlbumApplicationService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAlbumApplicationService_Base : IpcInterface {
	protected virtual void GetAlbumFileListByAruid() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.GetAlbumFileListByAruid".Log();
	protected virtual void DeleteAlbumFileByAruid() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.DeleteAlbumFileByAruid".Log();
	protected virtual void GetAlbumFileSizeByAruid() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.GetAlbumFileSizeByAruid".Log();
	protected virtual void LoadAlbumScreenShotImageByAruid() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.LoadAlbumScreenShotImageByAruid".Log();
	protected virtual void LoadAlbumScreenShotThumbnailImageByAruid() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.LoadAlbumScreenShotThumbnailImageByAruid".Log();
	protected virtual void OpenAccessorSessionForApplication() =>
		"Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.OpenAccessorSessionForApplication".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x66: { // GetAlbumFileListByAruid
				GetAlbumFileListByAruid();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // DeleteAlbumFileByAruid
				DeleteAlbumFileByAruid();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x68: { // GetAlbumFileSizeByAruid
				GetAlbumFileSizeByAruid();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6E: { // LoadAlbumScreenShotImageByAruid
				LoadAlbumScreenShotImageByAruid();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x78: { // LoadAlbumScreenShotThumbnailImageByAruid
				LoadAlbumScreenShotThumbnailImageByAruid();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xEA62: { // OpenAccessorSessionForApplication
				OpenAccessorSessionForApplication();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumApplicationService");
		}
	}
}

