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
	protected virtual void Finalize(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest.Finalize");
	protected virtual void SetAndWait(uint _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Mmnv.IRequest.SetAndWait");
	protected virtual uint Get(uint _0) =>
		throw new NotImplementedException("Nn.Mmnv.IRequest.Get not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // InitializeOld
				break;
			case 0x1: // FinalizeOld
				break;
			case 0x2: // SetAndWaitOld
				break;
			case 0x3: // GetOld
				break;
			case 0x4: // Initialize
				break;
			case 0x5: // Finalize
				break;
			case 0x6: // SetAndWait
				break;
			case 0x7: // Get
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mmnv.IRequest");
		}
	}
}

