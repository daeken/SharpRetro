using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Mmnv;
public partial class IRequest : _IRequest_Base;
public abstract class _IRequest_Base : IpcInterface {
	protected virtual void InitializeOld(uint _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest.InitializeOld");
	protected virtual void FinalizeOld(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest.FinalizeOld");
	protected virtual void SetAndWaitOld(uint _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest.SetAndWaitOld");
	protected virtual uint GetOld(uint _0) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.GetOld not implemented");
	protected virtual uint Initialize(uint _0, uint _1, uint _2) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.Initialize not implemented");
	protected virtual void _Finalize(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest._Finalize");
	protected virtual void SetAndWait(uint _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest.SetAndWait");
	protected virtual uint Get(uint _0) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.Get not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeOld
				om.Initialize(0, 0, 0);
				InitializeOld(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				break;
			}
			case 0x1: { // FinalizeOld
				om.Initialize(0, 0, 0);
				FinalizeOld(im.GetData<uint>(8));
				break;
			}
			case 0x2: { // SetAndWaitOld
				om.Initialize(0, 0, 0);
				SetAndWaitOld(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				break;
			}
			case 0x3: { // GetOld
				om.Initialize(0, 0, 4);
				var _return = GetOld(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // Initialize
				om.Initialize(0, 0, 4);
				var _return = Initialize(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // _Finalize
				om.Initialize(0, 0, 0);
				_Finalize(im.GetData<uint>(8));
				break;
			}
			case 0x6: { // SetAndWait
				om.Initialize(0, 0, 0);
				SetAndWait(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				break;
			}
			case 0x7: { // Get
				om.Initialize(0, 0, 4);
				var _return = Get(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mmnv.IRequest");
		}
	}
}

