using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.News.Detail.Ipc;
public partial class INewlyArrivedEventHolder : _INewlyArrivedEventHolder_Base;
public abstract class _INewlyArrivedEventHolder_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewlyArrivedEventHolder.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 1, 0);
				var _return = Unknown0();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewlyArrivedEventHolder");
		}
	}
}

public partial class INewsDataService : _INewsDataService_Base;
public abstract class _INewsDataService_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDataService.Unknown0");
	protected virtual void Unknown1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDataService.Unknown1");
	protected virtual void Unknown2(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDataService.Unknown2 not implemented");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDataService.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetBytes(8, 0x48));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 8);
				Unknown2(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 8);
				Unknown3(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewsDataService");
		}
	}
}

public partial class INewsDatabaseService : _INewsDatabaseService_Base;
public abstract class _INewsDatabaseService_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, Span<byte> _1, Span<byte> _2, out byte[] _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDatabaseService.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDatabaseService.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, Span<byte> _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsDatabaseService.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDatabaseService.Unknown3");
	protected virtual void Unknown4(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDatabaseService.Unknown4");
	protected virtual void Unknown5(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsDatabaseService.Unknown5");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 4);
				Unknown1(im.GetSpan<byte>(0x9, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 4);
				Unknown2(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1));
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewsDatabaseService");
		}
	}
}

public partial class INewsService : _INewsService_Base;
public abstract class _INewsService_Base : IpcInterface {
	protected virtual void Unknown10100(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown10100");
	protected virtual void Unknown20100(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown20100");
	protected virtual void Unknown30100(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30100 not implemented");
	protected virtual void Unknown30101(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30101 not implemented");
	protected virtual void Unknown30200(out byte[] _0) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30200 not implemented");
	protected virtual void Unknown30300(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown30300");
	protected virtual void Unknown30400(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30400 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewlyArrivedEventHolder Unknown30900() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30900 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewsDataService Unknown30901() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30901 not implemented");
	protected virtual Nn.News.Detail.Ipc.INewsDatabaseService Unknown30902() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown30902 not implemented");
	protected virtual void Unknown40100(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40100");
	protected virtual void Unknown40101(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40101");
	protected virtual void Unknown40200() =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40200");
	protected virtual void Unknown40201() =>
		Console.WriteLine("Stub hit for Nn.News.Detail.Ipc.INewsService.Unknown40201");
	protected virtual void Unknown90100(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.INewsService.Unknown90100 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: { // Unknown10100
				om.Initialize(0, 0, 0);
				Unknown10100(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x4E84: { // Unknown20100
				om.Initialize(0, 0, 0);
				Unknown20100(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x7594: { // Unknown30100
				om.Initialize(0, 0, 4);
				Unknown30100(im.GetSpan<byte>(0x9, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7595: { // Unknown30101
				om.Initialize(0, 0, 4);
				Unknown30101(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x75F8: { // Unknown30200
				om.Initialize(0, 0, 1);
				Unknown30200(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x765C: { // Unknown30300
				om.Initialize(0, 0, 0);
				Unknown30300(im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x76C0: { // Unknown30400
				om.Initialize(0, 0, 8);
				Unknown30400(im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x78B4: { // Unknown30900
				om.Initialize(1, 0, 0);
				var _return = Unknown30900();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x78B5: { // Unknown30901
				om.Initialize(1, 0, 0);
				var _return = Unknown30901();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x78B6: { // Unknown30902
				om.Initialize(1, 0, 0);
				var _return = Unknown30902();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x9CA4: { // Unknown40100
				om.Initialize(0, 0, 0);
				Unknown40100(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x9CA5: { // Unknown40101
				om.Initialize(0, 0, 0);
				Unknown40101(im.GetBytes(8, 0x8));
				break;
			}
			case 0x9D08: { // Unknown40200
				om.Initialize(0, 0, 0);
				Unknown40200();
				break;
			}
			case 0x9D09: { // Unknown40201
				om.Initialize(0, 0, 0);
				Unknown40201();
				break;
			}
			case 0x15FF4: { // Unknown90100
				om.Initialize(0, 0, 8);
				Unknown90100(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.INewsService");
		}
	}
}

public partial class IOverwriteEventHolder : _IOverwriteEventHolder_Base;
public abstract class _IOverwriteEventHolder_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.News.Detail.Ipc.IOverwriteEventHolder.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 1, 0);
				var _return = Unknown0();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(1, 0, 0);
				var _return = Unknown0();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(1, 0, 0);
				var _return = Unknown1();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(1, 0, 0);
				var _return = Unknown2();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(1, 0, 0);
				var _return = Unknown3();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(1, 0, 0);
				var _return = Unknown4();
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.News.Detail.Ipc.IServiceCreator");
		}
	}
}

