using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lr;
public partial class IAddOnContentLocationResolver : _IAddOnContentLocationResolver_Base;
public abstract class _IAddOnContentLocationResolver_Base : IpcInterface {
	protected virtual void ResolveAddOnContentPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.IAddOnContentLocationResolver.ResolveAddOnContentPath not implemented");
	protected virtual void RegisterAddOnContentStorage(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.IAddOnContentLocationResolver.RegisterAddOnContentStorage");
	protected virtual void UnregisterAllAddOnContentPath() =>
		Console.WriteLine("Stub hit for Nn.Lr.IAddOnContentLocationResolver.UnregisterAllAddOnContentPath");
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
	protected virtual void ResolveProgramPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveProgramPath not implemented");
	protected virtual void RedirectProgramPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectProgramPath");
	protected virtual void ResolveApplicationControlPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationControlPath not implemented");
	protected virtual void ResolveApplicationHtmlDocumentPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationHtmlDocumentPath not implemented");
	protected virtual void ResolveDataPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveDataPath not implemented");
	protected virtual void RedirectApplicationControlPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationControlPath");
	protected virtual void RedirectApplicationHtmlDocumentPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationHtmlDocumentPath");
	protected virtual void ResolveApplicationLegalInformationPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationLegalInformationPath not implemented");
	protected virtual void RedirectApplicationLegalInformationPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationLegalInformationPath");
	protected virtual void Refresh() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.Refresh");
	protected virtual void SetProgramNcaPath2() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.SetProgramNcaPath2");
	protected virtual void ClearLocationResolver2() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.ClearLocationResolver2");
	protected virtual void DeleteProgramNcaPath() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.DeleteProgramNcaPath");
	protected virtual void DeleteControlNcaPath() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.DeleteControlNcaPath");
	protected virtual void DeleteDocHtmlNcaPath() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.DeleteDocHtmlNcaPath");
	protected virtual void DeleteInfoHtmlNcaPath() =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.DeleteInfoHtmlNcaPath");
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
	protected virtual Nn.Lr.ILocationResolver OpenLocationResolver(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenLocationResolver not implemented");
	protected virtual Nn.Lr.IRegisteredLocationResolver OpenRegisteredLocationResolver() =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenRegisteredLocationResolver not implemented");
	protected virtual void RefreshLocationResolver(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolverManager.RefreshLocationResolver");
	protected virtual Nn.Lr.IAddOnContentLocationResolver OpenAddOnContentLocationResolver() =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenAddOnContentLocationResolver not implemented");
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
	protected virtual void ResolveProgramPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.IRegisteredLocationResolver.ResolveProgramPath not implemented");
	protected virtual void RegisterProgramPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RegisterProgramPath");
	protected virtual void UnregisterProgramPath(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.UnregisterProgramPath");
	protected virtual void RedirectProgramPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RedirectProgramPath");
	protected virtual void ResolveHtmlDocumentPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lr.IRegisteredLocationResolver.ResolveHtmlDocumentPath not implemented");
	protected virtual void RegisterHtmlDocumentPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RegisterHtmlDocumentPath");
	protected virtual void UnregisterHtmlDocumentPath(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.UnregisterHtmlDocumentPath");
	protected virtual void RedirectHtmlDocumentPath(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RedirectHtmlDocumentPath");
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

