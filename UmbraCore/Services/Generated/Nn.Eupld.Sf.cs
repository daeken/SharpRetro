using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Eupld.Sf;
public partial class IControl : _IControl_Base;
public abstract class _IControl_Base : IpcInterface {
	protected virtual void SetUrl(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IControl.SetUrl");
	protected virtual void ImportCrt(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IControl.ImportCrt");
	protected virtual void ImportPki(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IControl.ImportPki");
	protected virtual void SetAutoUpload(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IControl.SetAutoUpload");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetUrl
				break;
			case 0x1: // ImportCrt
				break;
			case 0x2: // ImportPki
				break;
			case 0x3: // SetAutoUpload
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eupld.Sf.IControl");
		}
	}
}

public partial class IRequest : _IRequest_Base;
public abstract class _IRequest_Base : IpcInterface {
	protected virtual KObject Initialize() =>
		throw new NotImplementedException("Nn.Eupld.Sf.IRequest.Initialize not implemented");
	protected virtual void UploadAll() =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IRequest.UploadAll");
	protected virtual void UploadSelected(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IRequest.UploadSelected");
	protected virtual void GetUploadStatus() =>
		throw new NotImplementedException("Nn.Eupld.Sf.IRequest.GetUploadStatus not implemented");
	protected virtual void CancelUpload() =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IRequest.CancelUpload");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Eupld.Sf.IRequest.GetResult");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // UploadAll
				break;
			case 0x2: // UploadSelected
				break;
			case 0x3: // GetUploadStatus
				break;
			case 0x4: // CancelUpload
				break;
			case 0x5: // GetResult
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eupld.Sf.IRequest");
		}
	}
}

