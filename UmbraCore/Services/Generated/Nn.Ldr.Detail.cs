using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ldr.Detail;
public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base;
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected virtual void AddProcessToDebugLaunchQueue(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IDebugMonitorInterface.AddProcessToDebugLaunchQueue");
	protected virtual void ClearDebugLaunchQueue() =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IDebugMonitorInterface.ClearDebugLaunchQueue");
	protected virtual void GetNsoInfos(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IDebugMonitorInterface.GetNsoInfos not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // AddProcessToDebugLaunchQueue
				break;
			}
			case 0x1: { // ClearDebugLaunchQueue
				break;
			}
			case 0x2: { // GetNsoInfos
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IProcessManagerInterface : _IProcessManagerInterface_Base;
public abstract class _IProcessManagerInterface_Base : IpcInterface {
	protected virtual IpcInterface CreateProcess(Span<byte> _0, KObject _1) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IProcessManagerInterface.CreateProcess not implemented");
	protected virtual void GetProgramInfo(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IProcessManagerInterface.GetProgramInfo not implemented");
	protected virtual void RegisterTitle(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldr.Detail.IProcessManagerInterface.RegisterTitle not implemented");
	protected virtual void UnregisterTitle(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IProcessManagerInterface.UnregisterTitle");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateProcess
				break;
			}
			case 0x1: { // GetProgramInfo
				break;
			}
			case 0x2: { // RegisterTitle
				break;
			}
			case 0x3: { // UnregisterTitle
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IProcessManagerInterface");
		}
	}
}

public partial class IRoInterface : _IRoInterface_Base;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // LoadNro
				break;
			}
			case 0x1: { // UnloadNro
				break;
			}
			case 0x2: { // LoadNrr
				break;
			}
			case 0x3: { // UnloadNrr
				break;
			}
			case 0x4: { // Initialize
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IRoInterface");
		}
	}
}

public partial class IShellInterface : _IShellInterface_Base;
public abstract class _IShellInterface_Base : IpcInterface {
	protected virtual void AddProcessToLaunchQueue(Span<byte> _0, uint size, ulong appID) =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IShellInterface.AddProcessToLaunchQueue");
	protected virtual void ClearLaunchQueue() =>
		Console.WriteLine("Stub hit for Nn.Ldr.Detail.IShellInterface.ClearLaunchQueue");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // AddProcessToLaunchQueue
				break;
			}
			case 0x1: { // ClearLaunchQueue
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IShellInterface");
		}
	}
}

