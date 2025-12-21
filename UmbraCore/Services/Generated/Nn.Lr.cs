using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lr;
public partial class IAddOnContentLocationResolver : _IAddOnContentLocationResolver_Base;
public abstract class _IAddOnContentLocationResolver_Base : IpcInterface {
	protected virtual void ResolveAddOnContentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.IAddOnContentLocationResolver.ResolveAddOnContentPath not implemented");
	protected virtual void RegisterAddOnContentStorage(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.IAddOnContentLocationResolver.RegisterAddOnContentStorage");
	protected virtual void UnregisterAllAddOnContentPath() =>
		Console.WriteLine("Stub hit for Nn.Lr.IAddOnContentLocationResolver.UnregisterAllAddOnContentPath");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ResolveAddOnContentPath
				om.Initialize(0, 0, 0);
				ResolveAddOnContentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1: { // RegisterAddOnContentStorage
				om.Initialize(0, 0, 0);
				RegisterAddOnContentStorage(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2: { // UnregisterAllAddOnContentPath
				om.Initialize(0, 0, 0);
				UnregisterAllAddOnContentPath();
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
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectProgramPath");
	protected virtual void ResolveApplicationControlPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationControlPath not implemented");
	protected virtual void ResolveApplicationHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationHtmlDocumentPath not implemented");
	protected virtual void ResolveDataPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveDataPath not implemented");
	protected virtual void RedirectApplicationControlPath(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationControlPath");
	protected virtual void RedirectApplicationHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolver.RedirectApplicationHtmlDocumentPath");
	protected virtual void ResolveApplicationLegalInformationPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolver.ResolveApplicationLegalInformationPath not implemented");
	protected virtual void RedirectApplicationLegalInformationPath(byte[] _0, Span<byte> _1) =>
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ResolveProgramPath
				om.Initialize(0, 0, 0);
				ResolveProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1: { // RedirectProgramPath
				om.Initialize(0, 0, 0);
				RedirectProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x2: { // ResolveApplicationControlPath
				om.Initialize(0, 0, 0);
				ResolveApplicationControlPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x3: { // ResolveApplicationHtmlDocumentPath
				om.Initialize(0, 0, 0);
				ResolveApplicationHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x4: { // ResolveDataPath
				om.Initialize(0, 0, 0);
				ResolveDataPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x5: { // RedirectApplicationControlPath
				om.Initialize(0, 0, 0);
				RedirectApplicationControlPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x6: { // RedirectApplicationHtmlDocumentPath
				om.Initialize(0, 0, 0);
				RedirectApplicationHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x7: { // ResolveApplicationLegalInformationPath
				om.Initialize(0, 0, 0);
				ResolveApplicationLegalInformationPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x8: { // RedirectApplicationLegalInformationPath
				om.Initialize(0, 0, 0);
				RedirectApplicationLegalInformationPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x9: { // Refresh
				om.Initialize(0, 0, 0);
				Refresh();
				break;
			}
			case 0xA: { // SetProgramNcaPath2
				om.Initialize(0, 0, 0);
				SetProgramNcaPath2();
				break;
			}
			case 0xB: { // ClearLocationResolver2
				om.Initialize(0, 0, 0);
				ClearLocationResolver2();
				break;
			}
			case 0xC: { // DeleteProgramNcaPath
				om.Initialize(0, 0, 0);
				DeleteProgramNcaPath();
				break;
			}
			case 0xD: { // DeleteControlNcaPath
				om.Initialize(0, 0, 0);
				DeleteControlNcaPath();
				break;
			}
			case 0xE: { // DeleteDocHtmlNcaPath
				om.Initialize(0, 0, 0);
				DeleteDocHtmlNcaPath();
				break;
			}
			case 0xF: { // DeleteInfoHtmlNcaPath
				om.Initialize(0, 0, 0);
				DeleteInfoHtmlNcaPath();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.ILocationResolver");
		}
	}
}

public partial class ILocationResolverManager : _ILocationResolverManager_Base;
public abstract class _ILocationResolverManager_Base : IpcInterface {
	protected virtual Nn.Lr.ILocationResolver OpenLocationResolver(byte[] _0) =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenLocationResolver not implemented");
	protected virtual Nn.Lr.IRegisteredLocationResolver OpenRegisteredLocationResolver() =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenRegisteredLocationResolver not implemented");
	protected virtual void RefreshLocationResolver(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.ILocationResolverManager.RefreshLocationResolver");
	protected virtual Nn.Lr.IAddOnContentLocationResolver OpenAddOnContentLocationResolver() =>
		throw new NotImplementedException("Nn.Lr.ILocationResolverManager.OpenAddOnContentLocationResolver not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenLocationResolver
				om.Initialize(1, 0, 0);
				var _return = OpenLocationResolver(im.GetBytes(8, 0x1));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // OpenRegisteredLocationResolver
				om.Initialize(1, 0, 0);
				var _return = OpenRegisteredLocationResolver();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // RefreshLocationResolver
				om.Initialize(0, 0, 0);
				RefreshLocationResolver(im.GetBytes(8, 0x1));
				break;
			}
			case 0x3: { // OpenAddOnContentLocationResolver
				om.Initialize(1, 0, 0);
				var _return = OpenAddOnContentLocationResolver();
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
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RegisterProgramPath");
	protected virtual void UnregisterProgramPath(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.UnregisterProgramPath");
	protected virtual void RedirectProgramPath(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RedirectProgramPath");
	protected virtual void ResolveHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lr.IRegisteredLocationResolver.ResolveHtmlDocumentPath not implemented");
	protected virtual void RegisterHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RegisterHtmlDocumentPath");
	protected virtual void UnregisterHtmlDocumentPath(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.UnregisterHtmlDocumentPath");
	protected virtual void RedirectHtmlDocumentPath(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Lr.IRegisteredLocationResolver.RedirectHtmlDocumentPath");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ResolveProgramPath
				om.Initialize(0, 0, 0);
				ResolveProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1: { // RegisterProgramPath
				om.Initialize(0, 0, 0);
				RegisterProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x2: { // UnregisterProgramPath
				om.Initialize(0, 0, 0);
				UnregisterProgramPath(im.GetBytes(8, 0x8));
				break;
			}
			case 0x3: { // RedirectProgramPath
				om.Initialize(0, 0, 0);
				RedirectProgramPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x4: { // ResolveHtmlDocumentPath
				om.Initialize(0, 0, 0);
				ResolveHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x5: { // RegisterHtmlDocumentPath
				om.Initialize(0, 0, 0);
				RegisterHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x6: { // UnregisterHtmlDocumentPath
				om.Initialize(0, 0, 0);
				UnregisterHtmlDocumentPath(im.GetBytes(8, 0x8));
				break;
			}
			case 0x7: { // RedirectHtmlDocumentPath
				om.Initialize(0, 0, 0);
				RedirectHtmlDocumentPath(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lr.IRegisteredLocationResolver");
		}
	}
}

