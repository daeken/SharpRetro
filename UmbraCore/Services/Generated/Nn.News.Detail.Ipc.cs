using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.News.Detail.Ipc;
public partial class INewlyArrivedEventHolder : _INewlyArrivedEventHolder_Base;
public abstract class _INewlyArrivedEventHolder_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewlyArrivedEventHolder");
		}
	}
}

public partial class INewsDataService : _INewsDataService_Base;
public abstract class _INewsDataService_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewsDataService");
		}
	}
}

public partial class INewsDatabaseService : _INewsDatabaseService_Base;
public abstract class _INewsDatabaseService_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewsDatabaseService");
		}
	}
}

public partial class INewsService : _INewsService_Base;
public abstract class _INewsService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: // Unknown10100
				break;
			case 0x4E84: // Unknown20100
				break;
			case 0x7594: // Unknown30100
				break;
			case 0x7595: // Unknown30101
				break;
			case 0x75F8: // Unknown30200
				break;
			case 0x765C: // Unknown30300
				break;
			case 0x76C0: // Unknown30400
				break;
			case 0x78B4: // Unknown30900
				break;
			case 0x78B5: // Unknown30901
				break;
			case 0x78B6: // Unknown30902
				break;
			case 0x9CA4: // Unknown40100
				break;
			case 0x9CA5: // Unknown40101
				break;
			case 0x9D08: // Unknown40200
				break;
			case 0x9D09: // Unknown40201
				break;
			case 0x15FF4: // Unknown90100
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewsService");
		}
	}
}

public partial class IOverwriteEventHolder : _IOverwriteEventHolder_Base;
public abstract class _IOverwriteEventHolder_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.IOverwriteEventHolder");
		}
	}
}

public partial class IServiceCreator : _IServiceCreator_Base;
public abstract class _IServiceCreator_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.IServiceCreator");
		}
	}
}

