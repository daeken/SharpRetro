using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Erpt.Sf;
public partial class IContext : _IContext_Base;
public abstract class _IContext_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SubmitContext
				break;
			case 0x1: // CreateReport
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IContext");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetReportList
				break;
			case 0x1: // GetEvent
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IManager");
		}
	}
}

public partial class IReport : _IReport_Base;
public abstract class _IReport_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Open
				break;
			case 0x1: // Read
				break;
			case 0x2: // SetFlags
				break;
			case 0x3: // GetFlags
				break;
			case 0x4: // Close
				break;
			case 0x5: // GetSize
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.IReport");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenReport
				break;
			case 0x1: // OpenManager
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Erpt.Sf.ISession");
		}
	}
}

