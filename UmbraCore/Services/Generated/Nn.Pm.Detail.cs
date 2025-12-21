using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pm.Detail;
public partial class IBootModeInterface : _IBootModeInterface_Base {
	public readonly string ServiceName;
	public IBootModeInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IBootModeInterface_Base : IpcInterface {
	protected virtual void GetBootMode(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IBootModeInterface.GetBootMode not implemented");
	protected virtual void SetMaintenanceBoot() =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IBootModeInterface.SetMaintenanceBoot");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBootMode
				GetBootMode(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // SetMaintenanceBoot
				SetMaintenanceBoot();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IBootModeInterface");
		}
	}
}

public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base {
	public readonly string ServiceName;
	public IDebugMonitorInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected virtual void GetDebugProcesses(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.GetDebugProcesses not implemented");
	protected virtual void StartDebugProcess(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.StartDebugProcess not implemented");
	protected virtual void GetTitlePid(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IDebugMonitorInterface.GetTitlePid");
	protected virtual void EnableDebugForTitleId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.EnableDebugForTitleId not implemented");
	protected virtual KObject GetApplicationPid(byte[] _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.GetApplicationPid not implemented");
	protected virtual void EnableDebugForApplication(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.EnableDebugForApplication not implemented");
	protected virtual KObject DisableDebug() =>
		throw new NotImplementedException("Nn.Pm.Detail.IDebugMonitorInterface.DisableDebug not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDebugProcesses
				GetDebugProcesses(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // StartDebugProcess
				StartDebugProcess(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // GetTitlePid
				GetTitlePid(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // EnableDebugForTitleId
				EnableDebugForTitleId(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // GetApplicationPid
				var _return = GetApplicationPid(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // EnableDebugForApplication
				EnableDebugForApplication(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // DisableDebug
				var _return = DisableDebug();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IInformationInterface : _IInformationInterface_Base {
	public readonly string ServiceName;
	public IInformationInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IInformationInterface_Base : IpcInterface {
	protected virtual void GetTitleId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pm.Detail.IInformationInterface.GetTitleId not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetTitleId
				GetTitleId(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IInformationInterface");
		}
	}
}

public partial class IShellInterface : _IShellInterface_Base {
	public readonly string ServiceName;
	public IShellInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IShellInterface_Base : IpcInterface {
	protected virtual void LaunchProcess(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.LaunchProcess not implemented");
	protected virtual void TerminateProcessByPid(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.TerminateProcessByPid");
	protected virtual void TerminateProcessByTitleId(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.TerminateProcessByTitleId");
	protected virtual KObject GetProcessEventWaiter() =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.GetProcessEventWaiter not implemented");
	protected virtual void GetProcessEventType(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.GetProcessEventType not implemented");
	protected virtual void NotifyBootFinished(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.NotifyBootFinished");
	protected virtual void GetApplicationPid_0(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.GetApplicationPid_0");
	protected virtual void BoostSystemMemoryResourceLimit_0() =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.BoostSystemMemoryResourceLimit_0");
	protected virtual void GetApplicationPid_1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pm.Detail.IShellInterface.GetApplicationPid_1 not implemented");
	protected virtual void BoostSystemMemoryResourceLimit_1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pm.Detail.IShellInterface.BoostSystemMemoryResourceLimit_1");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // LaunchProcess
				LaunchProcess(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // TerminateProcessByPid
				TerminateProcessByPid(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // TerminateProcessByTitleId
				TerminateProcessByTitleId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetProcessEventWaiter
				var _return = GetProcessEventWaiter();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x4: { // GetProcessEventType
				GetProcessEventType(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // NotifyBootFinished
				NotifyBootFinished(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetApplicationPid_0
				GetApplicationPid_0(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // BoostSystemMemoryResourceLimit_0
				BoostSystemMemoryResourceLimit_0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetApplicationPid_1
				GetApplicationPid_1(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // BoostSystemMemoryResourceLimit_1
				BoostSystemMemoryResourceLimit_1(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IShellInterface");
		}
	}
}

