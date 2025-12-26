using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lr;
public partial class IAddOnContentLocationResolver : _IAddOnContentLocationResolver_Base;
public abstract class _IAddOnContentLocationResolver_Base : IpcInterface {
	protected virtual void ResolveAddOnContentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.IAddOnContentLocationResolver.ResolveAddOnContentPath not implemented");
	protected virtual void RegisterAddOnContentStorage(byte[] _0) =>
		"Stub hit for Nn.Lr.IAddOnContentLocationResolver.RegisterAddOnContentStorage".Log();
	protected virtual void UnregisterAllAddOnContentPath() =>
		"Stub hit for Nn.Lr.IAddOnContentLocationResolver.UnregisterAllAddOnContentPath".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ResolveAddOnContentPath
				ResolveAddOnContentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RegisterAddOnContentStorage
				RegisterAddOnContentStorage(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // UnregisterAllAddOnContentPath
				UnregisterAllAddOnContentPath();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.IAddOnContentLocationResolver");
		}
	}
}

public partial class ILocationResolver : _ILocationResolver_Base;
public abstract class _ILocationResolver_Base : IpcInterface {
	protected virtual void ResolveProgramPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveProgramPath not implemented");
	protected virtual void RedirectProgramPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.ILocationResolver.RedirectProgramPath".Log();
	protected virtual void ResolveApplicationControlPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationControlPath not implemented");
	protected virtual void ResolveApplicationHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationHtmlDocumentPath not implemented");
	protected virtual void ResolveDataPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveDataPath not implemented");
	protected virtual void RedirectApplicationControlPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationControlPath".Log();
	protected virtual void RedirectApplicationHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationHtmlDocumentPath".Log();
	protected virtual void ResolveApplicationLegalInformationPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationLegalInformationPath not implemented");
	protected virtual void RedirectApplicationLegalInformationPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationLegalInformationPath".Log();
	protected virtual void Refresh() =>
		"Stub hit for Nn.Lr.ILocationResolver.Refresh".Log();
	protected virtual void SetProgramNcaPath2() =>
		"Stub hit for Nn.Lr.ILocationResolver.SetProgramNcaPath2".Log();
	protected virtual void ClearLocationResolver2() =>
		"Stub hit for Nn.Lr.ILocationResolver.ClearLocationResolver2".Log();
	protected virtual void DeleteProgramNcaPath() =>
		"Stub hit for Nn.Lr.ILocationResolver.DeleteProgramNcaPath".Log();
	protected virtual void DeleteControlNcaPath() =>
		"Stub hit for Nn.Lr.ILocationResolver.DeleteControlNcaPath".Log();
	protected virtual void DeleteDocHtmlNcaPath() =>
		"Stub hit for Nn.Lr.ILocationResolver.DeleteDocHtmlNcaPath".Log();
	protected virtual void DeleteInfoHtmlNcaPath() =>
		"Stub hit for Nn.Lr.ILocationResolver.DeleteInfoHtmlNcaPath".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ResolveProgramPath
				ResolveProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RedirectProgramPath
				RedirectProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ResolveApplicationControlPath
				ResolveApplicationControlPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ResolveApplicationHtmlDocumentPath
				ResolveApplicationHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // ResolveDataPath
				ResolveDataPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // RedirectApplicationControlPath
				RedirectApplicationControlPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // RedirectApplicationHtmlDocumentPath
				RedirectApplicationHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // ResolveApplicationLegalInformationPath
				ResolveApplicationLegalInformationPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // RedirectApplicationLegalInformationPath
				RedirectApplicationLegalInformationPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // Refresh
				Refresh();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // SetProgramNcaPath2
				SetProgramNcaPath2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // ClearLocationResolver2
				ClearLocationResolver2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // DeleteProgramNcaPath
				DeleteProgramNcaPath();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // DeleteControlNcaPath
				DeleteControlNcaPath();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // DeleteDocHtmlNcaPath
				DeleteDocHtmlNcaPath();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // DeleteInfoHtmlNcaPath
				DeleteInfoHtmlNcaPath();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.ILocationResolver");
		}
	}
}

public partial class ILocationResolverManager : _ILocationResolverManager_Base {
	public readonly string ServiceName;
	public ILocationResolverManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILocationResolverManager_Base : IpcInterface {
	protected virtual Nn.Lr.ILocationResolver OpenLocationResolver(byte[] _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenLocationResolver not implemented");
	protected virtual Nn.Lr.IRegisteredLocationResolver OpenRegisteredLocationResolver() =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenRegisteredLocationResolver not implemented");
	protected virtual void RefreshLocationResolver(byte[] _0) =>
		"Stub hit for Nn.Lr.ILocationResolverManager.RefreshLocationResolver".Log();
	protected virtual Nn.Lr.IAddOnContentLocationResolver OpenAddOnContentLocationResolver() =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenAddOnContentLocationResolver not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenLocationResolver
				var _return = OpenLocationResolver(im.GetBytes(8, 0x1));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // OpenRegisteredLocationResolver
				var _return = OpenRegisteredLocationResolver();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // RefreshLocationResolver
				RefreshLocationResolver(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // OpenAddOnContentLocationResolver
				var _return = OpenAddOnContentLocationResolver();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.ILocationResolverManager");
		}
	}
}

public partial class IRegisteredLocationResolver : _IRegisteredLocationResolver_Base;
public abstract class _IRegisteredLocationResolver_Base : IpcInterface {
	protected virtual void ResolveProgramPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.IRegisteredLocationResolver.ResolveProgramPath not implemented");
	protected virtual void RegisterProgramPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.IRegisteredLocationResolver.RegisterProgramPath".Log();
	protected virtual void UnregisterProgramPath(byte[] _0) =>
		"Stub hit for Nn.Lr.IRegisteredLocationResolver.UnregisterProgramPath".Log();
	protected virtual void RedirectProgramPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.IRegisteredLocationResolver.RedirectProgramPath".Log();
	protected virtual void ResolveHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.IRegisteredLocationResolver.ResolveHtmlDocumentPath not implemented");
	protected virtual void RegisterHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.IRegisteredLocationResolver.RegisterHtmlDocumentPath".Log();
	protected virtual void UnregisterHtmlDocumentPath(byte[] _0) =>
		"Stub hit for Nn.Lr.IRegisteredLocationResolver.UnregisterHtmlDocumentPath".Log();
	protected virtual void RedirectHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Lr.IRegisteredLocationResolver.RedirectHtmlDocumentPath".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ResolveProgramPath
				ResolveProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RegisterProgramPath
				RegisterProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // UnregisterProgramPath
				UnregisterProgramPath(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // RedirectProgramPath
				RedirectProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // ResolveHtmlDocumentPath
				ResolveHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // RegisterHtmlDocumentPath
				RegisterHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // UnregisterHtmlDocumentPath
				UnregisterHtmlDocumentPath(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // RedirectHtmlDocumentPath
				RedirectHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.IRegisteredLocationResolver");
		}
	}
}

