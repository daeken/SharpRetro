using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ldr.Detail;
public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base {
	public readonly string ServiceName;
	public IDebugMonitorInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected virtual void AddProcessToDebugLaunchQueue(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IDebugMonitorInterface.AddProcessToDebugLaunchQueue");
	protected virtual void ClearDebugLaunchQueue() =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IDebugMonitorInterface.ClearDebugLaunchQueue");
	protected virtual void GetNsoInfos(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IDebugMonitorInterface.GetNsoInfos not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // AddProcessToDebugLaunchQueue
				AddProcessToDebugLaunchQueue(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // ClearDebugLaunchQueue
				ClearDebugLaunchQueue();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetNsoInfos
				GetNsoInfos(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IProcessManagerInterface : _IProcessManagerInterface_Base {
	public readonly string ServiceName;
	public IProcessManagerInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IProcessManagerInterface_Base : IpcInterface {
	protected virtual IpcInterface CreateProcess(byte[] _0, KObject _1) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IProcessManagerInterface.CreateProcess not implemented");
	protected virtual void GetProgramInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IProcessManagerInterface.GetProgramInfo not implemented");
	protected virtual void RegisterTitle(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IProcessManagerInterface.RegisterTitle not implemented");
	protected virtual void UnregisterTitle(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IProcessManagerInterface.UnregisterTitle");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateProcess
				var _return = CreateProcess(im.GetBytes(8, 0x10), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetProgramInfo
				GetProgramInfo(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // RegisterTitle
				RegisterTitle(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // UnregisterTitle
				UnregisterTitle(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IProcessManagerInterface");
		}
	}
}

public partial class IRoInterface : _IRoInterface_Base {
	public readonly string ServiceName;
	public IRoInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IRoInterface_Base : IpcInterface {
	protected virtual ulong LoadNro(ulong _0, ulong _1, ulong _2, ulong _3, ulong _4, ulong _5) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IRoInterface.LoadNro not implemented");
	protected virtual void UnloadNro(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IRoInterface.UnloadNro");
	protected virtual void LoadNrr(ulong _0, ulong _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IRoInterface.LoadNrr");
	protected virtual void UnloadNrr(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IRoInterface.UnloadNrr");
	protected virtual void Initialize(ulong _0, ulong _1, KObject _2) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IRoInterface.Initialize");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // LoadNro
				var _return = LoadNro(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.Pid);
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // UnloadNro
				UnloadNro(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // LoadNrr
				LoadNrr(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // UnloadNrr
				UnloadNrr(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Initialize
				Initialize(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IRoInterface");
		}
	}
}

public partial class IShellInterface : _IShellInterface_Base {
	public readonly string ServiceName;
	public IShellInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IShellInterface_Base : IpcInterface {
	protected virtual void AddProcessToLaunchQueue(Span<byte> _0, uint size, ulong appID) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IShellInterface.AddProcessToLaunchQueue");
	protected virtual void ClearLaunchQueue() =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IShellInterface.ClearLaunchQueue");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // AddProcessToLaunchQueue
				AddProcessToLaunchQueue(im.GetSpan<byte>(0x9, 0), im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // ClearLaunchQueue
				ClearLaunchQueue();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IShellInterface");
		}
	}
}

