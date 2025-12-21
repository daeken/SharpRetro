using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Erpt.Sf;
public partial class IContext : _IContext_Base;
public abstract class _IContext_Base : IpcInterface {
	protected virtual void SubmitContext(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.SubmitContext");
	protected virtual void CreateReport(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.CreateReport");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.Unknown2");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.Unknown5");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IContext.Unknown6");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SubmitContext
				break;
			}
			case 0x1: { // CreateReport
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IContext");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void GetReportList(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IManager.GetReportList not implemented");
	protected virtual KObject GetEvent() =>
		throw new NotImplementedException("Nn.Erpt.Sf.IManager.GetEvent not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IManager.Unknown2");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IManager.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IManager.Unknown4");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetReportList
				break;
			}
			case 0x1: { // GetEvent
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IManager");
		}
	}
}

public partial class IReport : _IReport_Base;
public abstract class _IReport_Base : IpcInterface {
	protected virtual void Open(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IReport.Open");
	protected virtual void Read(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IReport.Read not implemented");
	protected virtual void SetFlags(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IReport.SetFlags");
	protected virtual void GetFlags(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IReport.GetFlags not implemented");
	protected virtual void Close() =>
		Console.WriteLine("Stub hit for Nn.Erpt.Sf.IReport.Close");
	protected virtual void GetSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IReport.GetSize not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				break;
			}
			case 0x1: { // Read
				break;
			}
			case 0x2: { // SetFlags
				break;
			}
			case 0x3: { // GetFlags
				break;
			}
			case 0x4: { // Close
				break;
			}
			case 0x5: { // GetSize
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IReport");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual Nn.Erpt.Sf.IReport OpenReport() =>
		throw new NotImplementedException("Nn.Erpt.Sf.ISession.OpenReport not implemented");
	protected virtual Nn.Erpt.Sf.IManager OpenManager() =>
		throw new NotImplementedException("Nn.Erpt.Sf.ISession.OpenManager not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenReport
				break;
			}
			case 0x1: { // OpenManager
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.ISession");
		}
	}
}

