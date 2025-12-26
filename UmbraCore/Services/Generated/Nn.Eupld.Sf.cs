using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Eupld.Sf;
public partial class IControl : _IControl_Base {
	public readonly string ServiceName;
	public IControl(string serviceName) => ServiceName = serviceName;
}
public abstract class _IControl_Base : IpcInterface {
	protected virtual void SetUrl(Span<byte> _0) =>
		"Stub hit for Nn.Eupld.Sf.IControl.SetUrl".Log();
	protected virtual void ImportCrt(Span<byte> _0) =>
		"Stub hit for Nn.Eupld.Sf.IControl.ImportCrt".Log();
	protected virtual void ImportPki(Span<byte> _0, Span<byte> _1) =>
		"Stub hit for Nn.Eupld.Sf.IControl.ImportPki".Log();
	protected virtual void SetAutoUpload(byte[] _0) =>
		"Stub hit for Nn.Eupld.Sf.IControl.SetAutoUpload".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetUrl
				SetUrl(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // ImportCrt
				ImportCrt(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ImportPki
				ImportPki(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // SetAutoUpload
				SetAutoUpload(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eupld.Sf.IControl");
		}
	}
}

public partial class IRequest : _IRequest_Base {
	public readonly string ServiceName;
	public IRequest(string serviceName) => ServiceName = serviceName;
}
public abstract class _IRequest_Base : IpcInterface {
	protected virtual KObject Initialize() =>
		throw new NotImplementedException("Nn.Eupld.Sf.IRequest.Initialize not implemented");
	protected virtual void UploadAll() =>
		"Stub hit for Nn.Eupld.Sf.IRequest.UploadAll".Log();
	protected virtual void UploadSelected(Span<byte> _0) =>
		"Stub hit for Nn.Eupld.Sf.IRequest.UploadSelected".Log();
	protected virtual void GetUploadStatus(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Eupld.Sf.IRequest.GetUploadStatus not implemented");
	protected virtual void CancelUpload() =>
		"Stub hit for Nn.Eupld.Sf.IRequest.CancelUpload".Log();
	protected virtual void GetResult() =>
		"Stub hit for Nn.Eupld.Sf.IRequest.GetResult".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // UploadAll
				UploadAll();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // UploadSelected
				UploadSelected(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetUploadStatus
				GetUploadStatus(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // CancelUpload
				CancelUpload();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eupld.Sf.IRequest");
		}
	}
}

