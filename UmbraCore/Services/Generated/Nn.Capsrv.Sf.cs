using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Capsrv.Sf;
public partial class IAlbumAccessorService : _IAlbumAccessorService_Base;
public abstract class _IAlbumAccessorService_Base : IpcInterface {
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

