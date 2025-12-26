using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Erpt.Sf;
public partial class IContext : _IContext_Base {
	public readonly string ServiceName;
	public IContext(string serviceName) => ServiceName = serviceName;
}
public abstract class _IContext_Base : IpcInterface {
	protected virtual void SubmitContext(Span<byte> _0, Span<byte> _1) =>
		"Stub hit for Nn.Erpt.Sf.IContext.SubmitContext".Log();
	protected virtual void CreateReport(byte[] _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		"Stub hit for Nn.Erpt.Sf.IContext.CreateReport".Log();
	protected virtual void Unknown2(byte[] _0) =>
		"Stub hit for Nn.Erpt.Sf.IContext.Unknown2".Log();
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Erpt.Sf.IContext.Unknown3".Log();
	protected virtual void Unknown4() =>
		"Stub hit for Nn.Erpt.Sf.IContext.Unknown4".Log();
	protected virtual void Unknown5() =>
		"Stub hit for Nn.Erpt.Sf.IContext.Unknown5".Log();
	protected virtual void Unknown6() =>
		"Stub hit for Nn.Erpt.Sf.IContext.Unknown6".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SubmitContext
				SubmitContext(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // CreateReport
				CreateReport(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), im.GetSpan<byte>(0x5, 2));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IContext");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void GetReportList(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IManager.GetReportList not implemented");
	protected virtual KObject GetEvent() =>
		throw new NotImplementedException("Nn.Erpt.Sf.IManager.GetEvent not implemented");
	protected virtual void Unknown2() =>
		"Stub hit for Nn.Erpt.Sf.IManager.Unknown2".Log();
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Erpt.Sf.IManager.Unknown3".Log();
	protected virtual void Unknown4() =>
		"Stub hit for Nn.Erpt.Sf.IManager.Unknown4".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetReportList
				GetReportList(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetEvent
				var _return = GetEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IManager");
		}
	}
}

public partial class IReport : _IReport_Base;
public abstract class _IReport_Base : IpcInterface {
	protected virtual void Open(byte[] _0) =>
		"Stub hit for Nn.Erpt.Sf.IReport.Open".Log();
	protected virtual void Read(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IReport.Read not implemented");
	protected virtual void SetFlags(byte[] _0) =>
		"Stub hit for Nn.Erpt.Sf.IReport.SetFlags".Log();
	protected virtual void GetFlags(out byte[] _0) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IReport.GetFlags not implemented");
	protected virtual void _Close() =>
		"Stub hit for Nn.Erpt.Sf.IReport._Close".Log();
	protected virtual void GetSize(out byte[] _0) =>
		throw new NotImplementedException("Nn.Erpt.Sf.IReport.GetSize not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				Open(im.GetBytes(8, 0x14));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Read
				Read(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // SetFlags
				SetFlags(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetFlags
				GetFlags(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // _Close
				_Close();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetSize
				GetSize(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IReport");
		}
	}
}

public partial class ISession : _ISession_Base {
	public readonly string ServiceName;
	public ISession(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISession_Base : IpcInterface {
	protected virtual Nn.Erpt.Sf.IReport OpenReport() =>
		throw new NotImplementedException("Nn.Erpt.Sf.ISession.OpenReport not implemented");
	protected virtual Nn.Erpt.Sf.IManager OpenManager() =>
		throw new NotImplementedException("Nn.Erpt.Sf.ISession.OpenManager not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenReport
				var _return = OpenReport();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // OpenManager
				var _return = OpenManager();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.ISession");
		}
	}
}

