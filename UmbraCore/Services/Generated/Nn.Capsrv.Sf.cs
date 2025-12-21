using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Capsrv.Sf;
public partial class IAlbumAccessorService : _IAlbumAccessorService_Base;
public abstract class _IAlbumAccessorService_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown7 not implemented");
	protected virtual void Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8 not implemented");
	protected virtual void Unknown9(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown9 not implemented");
	protected virtual void Unknown10(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown10 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown11 not implemented");
	protected virtual void Unknown12(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown12 not implemented");
	protected virtual void Unknown13(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown13 not implemented");
	protected virtual void Unknown14(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown14 not implemented");
	protected virtual void Unknown301() =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown301 not implemented");
	protected virtual void Unknown401() =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown401 not implemented");
	protected virtual void Unknown501(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown501 not implemented");
	protected virtual void Unknown1001(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown1001 not implemented");
	protected virtual void Unknown1002(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown1002 not implemented");
	protected virtual void Unknown8001(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8001");
	protected virtual void Unknown8002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8002");
	protected virtual void Unknown8011(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8011");
	protected virtual void Unknown8012(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8012 not implemented");
	protected virtual void Unknown8021(Span<byte> _0, ulong _1) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorService.Unknown8021 not implemented");
	protected virtual void Unknown10011(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorService.Unknown10011");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
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
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // Unknown14
				break;
			case 0x12D: // Unknown301
				break;
			case 0x191: // Unknown401
				break;
			case 0x1F5: // Unknown501
				break;
			case 0x3E9: // Unknown1001
				break;
			case 0x3EA: // Unknown1002
				break;
			case 0x1F41: // Unknown8001
				break;
			case 0x1F42: // Unknown8002
				break;
			case 0x1F4B: // Unknown8011
				break;
			case 0x1F4C: // Unknown8012
				break;
			case 0x1F55: // Unknown8021
				break;
			case 0x271B: // Unknown10011
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumAccessorService");
		}
	}
}

public partial class IAlbumAccessorSession : _IAlbumAccessorSession_Base;
public abstract class _IAlbumAccessorSession_Base : IpcInterface {
	protected virtual void Unknown2001(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2001 not implemented");
	protected virtual void Unknown2002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2002");
	protected virtual void Unknown2003(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2003 not implemented");
	protected virtual void Unknown2004(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2004 not implemented");
	protected virtual void Unknown2005(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2005");
	protected virtual void Unknown2006(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2006 not implemented");
	protected virtual void Unknown2007(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2007 not implemented");
	protected virtual void Unknown2008(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumAccessorSession.Unknown2008 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: // Unknown2001
				break;
			case 0x7D2: // Unknown2002
				break;
			case 0x7D3: // Unknown2003
				break;
			case 0x7D4: // Unknown2004
				break;
			case 0x7D5: // Unknown2005
				break;
			case 0x7D6: // Unknown2006
				break;
			case 0x7D7: // Unknown2007
				break;
			case 0x7D8: // Unknown2008
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumAccessorSession");
		}
	}
}

public partial class IAlbumControlService : _IAlbumControlService_Base;
public abstract class _IAlbumControlService_Base : IpcInterface {
	protected virtual void Unknown2001(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2001");
	protected virtual void Unknown2002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2002");
	protected virtual void Unknown2011(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2011");
	protected virtual void Unknown2012(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2012");
	protected virtual void Unknown2013(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlService.Unknown2013 not implemented");
	protected virtual void Unknown2014(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2014");
	protected virtual void Unknown2101(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlService.Unknown2101 not implemented");
	protected virtual void Unknown2102(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlService.Unknown2102 not implemented");
	protected virtual void Unknown2201(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2201");
	protected virtual void Unknown2301(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlService.Unknown2301");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: // Unknown2001
				break;
			case 0x7D2: // Unknown2002
				break;
			case 0x7DB: // Unknown2011
				break;
			case 0x7DC: // Unknown2012
				break;
			case 0x7DD: // Unknown2013
				break;
			case 0x7DE: // Unknown2014
				break;
			case 0x835: // Unknown2101
				break;
			case 0x836: // Unknown2102
				break;
			case 0x899: // Unknown2201
				break;
			case 0x8FD: // Unknown2301
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumControlService");
		}
	}
}

public partial class IAlbumControlSession : _IAlbumControlSession_Base;
public abstract class _IAlbumControlSession_Base : IpcInterface {
	protected virtual void Unknown2001(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2001 not implemented");
	protected virtual void Unknown2002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2002");
	protected virtual void Unknown2003(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2003 not implemented");
	protected virtual void Unknown2004(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2004 not implemented");
	protected virtual void Unknown2005(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2005");
	protected virtual void Unknown2006(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2006 not implemented");
	protected virtual void Unknown2007(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2007 not implemented");
	protected virtual void Unknown2008(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2008 not implemented");
	protected virtual void Unknown2401(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2401 not implemented");
	protected virtual void Unknown2402(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2402");
	protected virtual void Unknown2403(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2403");
	protected virtual void Unknown2404(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2404");
	protected virtual void Unknown2405(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2405");
	protected virtual void Unknown2411(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2411");
	protected virtual void Unknown2412(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2412");
	protected virtual void Unknown2413(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2413");
	protected virtual void Unknown2414(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2414");
	protected virtual void Unknown2421(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2421 not implemented");
	protected virtual void Unknown2422(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2422");
	protected virtual void Unknown2424(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2424");
	protected virtual void Unknown2431(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2431");
	protected virtual void Unknown2433(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IAlbumControlSession.Unknown2433 not implemented");
	protected virtual void Unknown2434(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumControlSession.Unknown2434");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: // Unknown2001
				break;
			case 0x7D2: // Unknown2002
				break;
			case 0x7D3: // Unknown2003
				break;
			case 0x7D4: // Unknown2004
				break;
			case 0x7D5: // Unknown2005
				break;
			case 0x7D6: // Unknown2006
				break;
			case 0x7D7: // Unknown2007
				break;
			case 0x7D8: // Unknown2008
				break;
			case 0x961: // Unknown2401
				break;
			case 0x962: // Unknown2402
				break;
			case 0x963: // Unknown2403
				break;
			case 0x964: // Unknown2404
				break;
			case 0x965: // Unknown2405
				break;
			case 0x96B: // Unknown2411
				break;
			case 0x96C: // Unknown2412
				break;
			case 0x96D: // Unknown2413
				break;
			case 0x96E: // Unknown2414
				break;
			case 0x975: // Unknown2421
				break;
			case 0x976: // Unknown2422
				break;
			case 0x978: // Unknown2424
				break;
			case 0x97F: // Unknown2431
				break;
			case 0x981: // Unknown2433
				break;
			case 0x982: // Unknown2434
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumControlSession");
		}
	}
}

public partial class ICaptureControllerService : _ICaptureControllerService_Base;
public abstract class _ICaptureControllerService_Base : IpcInterface {
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.ICaptureControllerService.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.ICaptureControllerService.Unknown2 not implemented");
	protected virtual void Unknown1001(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown1001");
	protected virtual void Unknown1002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown1002");
	protected virtual void Unknown1011(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown1011");
	protected virtual void Unknown2001(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown2001");
	protected virtual void Unknown2002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.ICaptureControllerService.Unknown2002");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3E9: // Unknown1001
				break;
			case 0x3EA: // Unknown1002
				break;
			case 0x3F3: // Unknown1011
				break;
			case 0x7D1: // Unknown2001
				break;
			case 0x7D2: // Unknown2002
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.ICaptureControllerService");
		}
	}
}

public partial class IScreenShotApplicationService : _IScreenShotApplicationService_Base;
public abstract class _IScreenShotApplicationService_Base : IpcInterface {
	protected virtual void SaveScreenShot(uint _0, uint _1, ulong _2, ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotApplicationService.SaveScreenShot not implemented");
	protected virtual void SaveScreenShotEx0(Span<byte> _0, uint _1, ulong _2, ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotApplicationService.SaveScreenShotEx0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xC9: // SaveScreenShot
				break;
			case 0xCB: // SaveScreenShotEx0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IScreenShotApplicationService");
		}
	}
}

public partial class IScreenShotControlService : _IScreenShotControlService_Base;
public abstract class _IScreenShotControlService_Base : IpcInterface {
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown2 not implemented");
	protected virtual void Unknown1001(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1001");
	protected virtual void Unknown1002(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1002");
	protected virtual void Unknown1003(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1003");
	protected virtual void Unknown1011(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1011");
	protected virtual void Unknown1012(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1012");
	protected virtual void Unknown1201(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown1201 not implemented");
	protected virtual void Unknown1202() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IScreenShotControlService.Unknown1202");
	protected virtual void Unknown1203(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotControlService.Unknown1203 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3E9: // Unknown1001
				break;
			case 0x3EA: // Unknown1002
				break;
			case 0x3EB: // Unknown1003
				break;
			case 0x3F3: // Unknown1011
				break;
			case 0x3F4: // Unknown1012
				break;
			case 0x4B1: // Unknown1201
				break;
			case 0x4B2: // Unknown1202
				break;
			case 0x4B3: // Unknown1203
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IScreenShotControlService");
		}
	}
}

public partial class IScreenShotService : _IScreenShotService_Base;
public abstract class _IScreenShotService_Base : IpcInterface {
	protected virtual void Unknown201(Span<byte> _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown201 not implemented");
	protected virtual void Unknown202(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown202 not implemented");
	protected virtual void Unknown203(Span<byte> _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown203 not implemented");
	protected virtual void Unknown204(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Capsrv.Sf.IScreenShotService.Unknown204 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xC9: // Unknown201
				break;
			case 0xCA: // Unknown202
				break;
			case 0xCB: // Unknown203
				break;
			case 0xCC: // Unknown204
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IScreenShotService");
		}
	}
}

public partial class IAlbumAccessorApplicationSession : _IAlbumAccessorApplicationSession_Base;
public abstract class _IAlbumAccessorApplicationSession_Base : IpcInterface {
	protected virtual void OpenAlbumMovieReadStream() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.OpenAlbumMovieReadStream");
	protected virtual void CloseAlbumMovieReadStream() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.CloseAlbumMovieReadStream");
	protected virtual void GetAlbumMovieReadStreamMovieDataSize() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.GetAlbumMovieReadStreamMovieDataSize");
	protected virtual void ReadMovieDataFromAlbumMovieReadStream() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.ReadMovieDataFromAlbumMovieReadStream");
	protected virtual void GetAlbumMovieReadStreamBrokenReason() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumAccessorApplicationSession.GetAlbumMovieReadStreamBrokenReason");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x7D1: // OpenAlbumMovieReadStream
				break;
			case 0x7D2: // CloseAlbumMovieReadStream
				break;
			case 0x7D3: // GetAlbumMovieReadStreamMovieDataSize
				break;
			case 0x7D4: // ReadMovieDataFromAlbumMovieReadStream
				break;
			case 0x7D5: // GetAlbumMovieReadStreamBrokenReason
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumAccessorApplicationSession");
		}
	}
}

public partial class IAlbumApplicationService : _IAlbumApplicationService_Base;
public abstract class _IAlbumApplicationService_Base : IpcInterface {
	protected virtual void GetAlbumFileListByAruid() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.GetAlbumFileListByAruid");
	protected virtual void DeleteAlbumFileByAruid() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.DeleteAlbumFileByAruid");
	protected virtual void GetAlbumFileSizeByAruid() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.GetAlbumFileSizeByAruid");
	protected virtual void LoadAlbumScreenShotImageByAruid() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.LoadAlbumScreenShotImageByAruid");
	protected virtual void LoadAlbumScreenShotThumbnailImageByAruid() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.LoadAlbumScreenShotThumbnailImageByAruid");
	protected virtual void OpenAccessorSessionForApplication() =>
		Console.WriteLine("Stub hit for Nn.Capsrv.Sf.IAlbumApplicationService.OpenAccessorSessionForApplication");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x66: // GetAlbumFileListByAruid
				break;
			case 0x67: // DeleteAlbumFileByAruid
				break;
			case 0x68: // GetAlbumFileSizeByAruid
				break;
			case 0x6E: // LoadAlbumScreenShotImageByAruid
				break;
			case 0x78: // LoadAlbumScreenShotThumbnailImageByAruid
				break;
			case 0xEA62: // OpenAccessorSessionForApplication
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Capsrv.Sf.IAlbumApplicationService");
		}
	}
}

