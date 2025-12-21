using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Jitsrv;
public partial class IJitEnvironment : _IJitEnvironment_Base;
public abstract class _IJitEnvironment_Base : IpcInterface {
	protected virtual void Control() =>
		Console.WriteLine("Stub hit for Nn.Jitsrv.IJitEnvironment.Control");
	protected virtual void GenerateCode() =>
		Console.WriteLine("Stub hit for Nn.Jitsrv.IJitEnvironment.GenerateCode");
	protected virtual void LoadPlugin() =>
		Console.WriteLine("Stub hit for Nn.Jitsrv.IJitEnvironment.LoadPlugin");
	protected virtual void GetCodeAddress() =>
		Console.WriteLine("Stub hit for Nn.Jitsrv.IJitEnvironment.GetCodeAddress");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Control
				break;
			case 0x1: // GenerateCode
				break;
			case 0x3E8: // LoadPlugin
				break;
			case 0x3E9: // GetCodeAddress
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Jitsrv.IJitEnvironment");
		}
	}
}

public partial class IJitService : _IJitService_Base;
public abstract class _IJitService_Base : IpcInterface {
	protected virtual void CreateJitEnvironment() =>
		Console.WriteLine("Stub hit for Nn.Jitsrv.IJitService.CreateJitEnvironment");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateJitEnvironment
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Jitsrv.IJitService");
		}
	}
}

