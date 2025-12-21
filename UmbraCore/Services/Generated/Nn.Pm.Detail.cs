using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pm.Detail;
public partial class IBootModeInterface : _IBootModeInterface_Base;
public abstract class _IBootModeInterface_Base : IpcInterface {
	protected virtual void GetBootMode() =>
		throw new NotImplementedException("Nn.Pm.Detail.IBootModeInterface.GetBootMode not implemented");
	protected virtual void SetMaintenanceBoot() =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IBootModeInterface.SetMaintenanceBoot");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetBootMode
				break;
			case 0x1: // SetMaintenanceBoot
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IBootModeInterface");
		}
	}
}

public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base;
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected virtual void GetDebugProcesses(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.GetDebugProcesses not implemented");
	protected virtual void StartDebugProcess() =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.StartDebugProcess not implemented");
	protected virtual void GetTitlePid(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IDebugMonitorInterface.GetTitlePid");
	protected virtual void EnableDebugForTitleId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.EnableDebugForTitleId not implemented");
	protected virtual KObject GetApplicationPid(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.GetApplicationPid not implemented");
	protected virtual void EnableDebugForApplication() =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.EnableDebugForApplication not implemented");
	protected virtual KObject DisableDebug() =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.DisableDebug not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDebugProcesses
				break;
			case 0x1: // StartDebugProcess
				break;
			case 0x2: // GetTitlePid
				break;
			case 0x3: // EnableDebugForTitleId
				break;
			case 0x4: // GetApplicationPid
				break;
			case 0x5: // EnableDebugForApplication
				break;
			case 0x6: // DisableDebug
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IInformationInterface : _IInformationInterface_Base;
public abstract class _IInformationInterface_Base : IpcInterface {
	protected virtual void GetTitleId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IInformationInterface.GetTitleId not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetTitleId
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IInformationInterface");
		}
	}
}

public partial class IShellInterface : _IShellInterface_Base;
public abstract class _IShellInterface_Base : IpcInterface {
	protected virtual void LaunchProcess(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.LaunchProcess not implemented");
	protected virtual void TerminateProcessByPid(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.TerminateProcessByPid");
	protected virtual void TerminateProcessByTitleId(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.TerminateProcessByTitleId");
	protected virtual KObject GetProcessEventWaiter() =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.GetProcessEventWaiter not implemented");
	protected virtual void GetProcessEventType() =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.GetProcessEventType not implemented");
	protected virtual void NotifyBootFinished(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.NotifyBootFinished");
	protected virtual void GetApplicationPid_0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.GetApplicationPid_0");
	protected virtual void BoostSystemMemoryResourceLimit_0() =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.BoostSystemMemoryResourceLimit_0");
	protected virtual void GetApplicationPid_1() =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.GetApplicationPid_1 not implemented");
	protected virtual void BoostSystemMemoryResourceLimit_1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.BoostSystemMemoryResourceLimit_1");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // LaunchProcess
				break;
			case 0x1: // TerminateProcessByPid
				break;
			case 0x2: // TerminateProcessByTitleId
				break;
			case 0x3: // GetProcessEventWaiter
				break;
			case 0x4: // GetProcessEventType
				break;
			case 0x5: // NotifyBootFinished
				break;
			case 0x6: // GetApplicationPid_0
				break;
			case 0x7: // BoostSystemMemoryResourceLimit_0
				break;
			case 0x8: // GetApplicationPid_1
				break;
			case 0x9: // BoostSystemMemoryResourceLimit_1
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IShellInterface");
		}
	}
}

