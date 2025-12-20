using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lr;
public partial class IAddOnContentLocationResolver : _IAddOnContentLocationResolver_Base;
public abstract class _IAddOnContentLocationResolver_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ResolveAddOnContentPath
				break;
			case 0x1: // RegisterAddOnContentStorage
				break;
			case 0x2: // UnregisterAllAddOnContentPath
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.IAddOnContentLocationResolver");
		}
	}
}

public partial class ILocationResolver : _ILocationResolver_Base;
public abstract class _ILocationResolver_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ResolveProgramPath
				break;
			case 0x1: // RedirectProgramPath
				break;
			case 0x2: // ResolveApplicationControlPath
				break;
			case 0x3: // ResolveApplicationHtmlDocumentPath
				break;
			case 0x4: // ResolveDataPath
				break;
			case 0x5: // RedirectApplicationControlPath
				break;
			case 0x6: // RedirectApplicationHtmlDocumentPath
				break;
			case 0x7: // ResolveApplicationLegalInformationPath
				break;
			case 0x8: // RedirectApplicationLegalInformationPath
				break;
			case 0x9: // Refresh
				break;
			case 0xA: // SetProgramNcaPath2
				break;
			case 0xB: // ClearLocationResolver2
				break;
			case 0xC: // DeleteProgramNcaPath
				break;
			case 0xD: // DeleteControlNcaPath
				break;
			case 0xE: // DeleteDocHtmlNcaPath
				break;
			case 0xF: // DeleteInfoHtmlNcaPath
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.ILocationResolver");
		}
	}
}

public partial class ILocationResolverManager : _ILocationResolverManager_Base;
public abstract class _ILocationResolverManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenLocationResolver
				break;
			case 0x1: // OpenRegisteredLocationResolver
				break;
			case 0x2: // RefreshLocationResolver
				break;
			case 0x3: // OpenAddOnContentLocationResolver
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.ILocationResolverManager");
		}
	}
}

public partial class IRegisteredLocationResolver : _IRegisteredLocationResolver_Base;
public abstract class _IRegisteredLocationResolver_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ResolveProgramPath
				break;
			case 0x1: // RegisterProgramPath
				break;
			case 0x2: // UnregisterProgramPath
				break;
			case 0x3: // RedirectProgramPath
				break;
			case 0x4: // ResolveHtmlDocumentPath
				break;
			case 0x5: // RegisterHtmlDocumentPath
				break;
			case 0x6: // UnregisterHtmlDocumentPath
				break;
			case 0x7: // RedirectHtmlDocumentPath
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.IRegisteredLocationResolver");
		}
	}
}

