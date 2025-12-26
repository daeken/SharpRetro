using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Mmnv;
public partial class IRequest : _IRequest_Base {
	public readonly string ServiceName;
	public IRequest(string serviceName) => ServiceName = serviceName;
}
public abstract class _IRequest_Base : IpcInterface {
	protected virtual void InitializeOld(uint _0, uint _1, uint _2) =>
		"Stub hit for Nn.Mmnv.IRequest.InitializeOld".Log();
	protected virtual void FinalizeOld(uint _0) =>
		"Stub hit for Nn.Mmnv.IRequest.FinalizeOld".Log();
	protected virtual void SetAndWaitOld(uint _0, uint _1, uint _2) =>
		"Stub hit for Nn.Mmnv.IRequest.SetAndWaitOld".Log();
	protected virtual uint GetOld(uint _0) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.GetOld not implemented");
	protected virtual uint Initialize(uint _0, uint _1, uint _2) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.Initialize not implemented");
	protected virtual void _Finalize(uint _0) =>
		"Stub hit for Nn.Mmnv.IRequest._Finalize".Log();
	protected virtual void SetAndWait(uint _0, uint _1, uint _2) =>
		"Stub hit for Nn.Mmnv.IRequest.SetAndWait".Log();
	protected virtual uint Get(uint _0) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.Get not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeOld
				InitializeOld(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // FinalizeOld
				FinalizeOld(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SetAndWaitOld
				SetAndWaitOld(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetOld
				var _return = GetOld(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // Initialize
				var _return = Initialize(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // _Finalize
				_Finalize(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // SetAndWait
				SetAndWait(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Get
				var _return = Get(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mmnv.IRequest");
		}
	}
}

