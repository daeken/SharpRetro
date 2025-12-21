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
	protected virtual void Suspend(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Account.Http.IOAuthProcedure.Suspend not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PrepareAsync
				break;
			}
			case 0x1: { // GetRequest
				break;
			}
			case 0x2: { // ApplyResponse
				break;
			}
			case 0x3: { // ApplyResponseAsync
				break;
			}
			case 0xA: { // Suspend
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Http.IOAuthProcedure");
		}
	}
}

