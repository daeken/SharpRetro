using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Http;
public partial class IOAuthProcedure : _IOAuthProcedure_Base;
public abstract class _IOAuthProcedure_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PrepareAsync
				break;
			case 0x1: // GetRequest
				break;
			case 0x2: // ApplyResponse
				break;
			case 0x3: // ApplyResponseAsync
				break;
			case 0xA: // Suspend
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Http.IOAuthProcedure");
		}
	}
}

