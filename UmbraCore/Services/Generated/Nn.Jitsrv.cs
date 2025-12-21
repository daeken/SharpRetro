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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Control
				Control();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GenerateCode
				GenerateCode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E8: { // LoadPlugin
				LoadPlugin();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // GetCodeAddress
				GetCodeAddress();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Jitsrv.IJitEnvironment");
		}
	}
}

public partial class IJitService : _IJitService_Base {
	public readonly string ServiceName;
	public IJitService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IJitService_Base : IpcInterface {
	protected virtual void CreateJitEnvironment() =>
		Console.WriteLine("Stub hit for Nn.Jitsrv.IJitService.CreateJitEnvironment");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateJitEnvironment
				CreateJitEnvironment();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Jitsrv.IJitService");
		}
	}
}

