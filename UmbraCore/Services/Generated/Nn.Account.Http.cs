using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Http;
public partial class IOAuthProcedure : _IOAuthProcedure_Base;
public abstract class _IOAuthProcedure_Base : IpcInterface {
	protected virtual Nn.Account.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Account.Http.IOAuthProcedure.PrepareAsync not implemented");
	protected virtual void GetRequest(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Account.Http.IOAuthProcedure.GetRequest not implemented");
	protected virtual void ApplyResponse(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Account.Http.IOAuthProcedure.ApplyResponse");
	protected virtual Nn.Account.Detail.IAsyncContext ApplyResponseAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Http.IOAuthProcedure.ApplyResponseAsync not implemented");
	protected virtual void Suspend(out byte[] _0) =>
		throw new NotImplementedException("Nn.Account.Http.IOAuthProcedure.Suspend not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PrepareAsync
				var _return = PrepareAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetRequest
				GetRequest(im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0x1A, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ApplyResponse
				ApplyResponse(im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ApplyResponseAsync
				var _return = ApplyResponseAsync(im.GetSpan<byte>(0x9, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // Suspend
				Suspend(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Http.IOAuthProcedure");
		}
	}
}

