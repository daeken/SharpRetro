using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.News.Detail.Ipc;
public partial class INewlyArrivedEventHolder : _INewlyArrivedEventHolder_Base;
public abstract class _INewlyArrivedEventHolder_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewlyArrivedEventHolder.Unknown0 not implemented");
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
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDataService.Unknown0");
	protected virtual void Unknown1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDataService.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDataService.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDataService.Unknown3 not implemented");
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
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDatabaseService.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDatabaseService.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDatabaseService.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDatabaseService.Unknown3");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDatabaseService.Unknown4");
	protected virtual void Unknown5(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDatabaseService.Unknown5");
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
	protected virtual void Unknown10100(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown10100");
	protected virtual void Unknown20100(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown20100");
	protected virtual void Unknown30100(Span<byte> _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30100 not implemented");
	protected virtual void Unknown30101(Span<byte> _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30101 not implemented");
	protected virtual void Unknown30200() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30200 not implemented");
	protected virtual void Unknown30300(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown30300");
	protected virtual void Unknown30400(Span<byte> _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30400 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewlyArrivedEventHolder Unknown30900() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30900 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewsDataService Unknown30901() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30901 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewsDatabaseService Unknown30902() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30902 not implemented");
	protected virtual void Unknown40100(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40100");
	protected virtual void Unknown40101(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40101");
	protected virtual void Unknown40200() =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40200");
	protected virtual void Unknown40201() =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40201");
	protected virtual void Unknown90100() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown90100 not implemented");
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
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IOverwriteEventHolder.Unknown0 not implemented");
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
	protected virtual Nn.News.Detail.Ipc.INewsService Unknown0() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IServiceCreator.Unknown0 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewlyArrivedEventHolder Unknown1() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IServiceCreator.Unknown1 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewsDataService Unknown2() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IServiceCreator.Unknown2 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewsDatabaseService Unknown3() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IServiceCreator.Unknown3 not implemented");
	protected virtual Nn.News.Detail.Ipc.IOverwriteEventHolder Unknown4() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IServiceCreator.Unknown4 not implemented");
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

