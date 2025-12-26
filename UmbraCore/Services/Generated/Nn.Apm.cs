using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Apm;
public partial class IDebugManager : _IDebugManager_Base;
public abstract class _IDebugManager_Base : IpcInterface {
	protected virtual void GetThrottlingState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Apm.IDebugManager.GetThrottlingState not implemented");
	protected virtual void GetLastThrottlingState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Apm.IDebugManager.GetLastThrottlingState not implemented");
	protected virtual void ClearLastThrottlingState() =>
		"Stub hit for Nn.Apm.IDebugManager.ClearLastThrottlingState".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetThrottlingState
				GetThrottlingState(out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetLastThrottlingState
				GetLastThrottlingState(out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // ClearLastThrottlingState
				ClearLastThrottlingState();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.IDebugManager");
		}
	}
}

public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Apm.ISession OpenSession() =>
		throw new NotImplementedException("Nn.Apm.IManager.OpenSession not implemented");
	protected virtual uint GetPerformanceMode() =>
		throw new NotImplementedException("Nn.Apm.IManager.GetPerformanceMode not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				var _return = OpenSession();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetPerformanceMode
				var _return = GetPerformanceMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.IManager");
		}
	}
}

public partial class IManagerPrivileged : _IManagerPrivileged_Base {
	public readonly string ServiceName;
	public IManagerPrivileged(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManagerPrivileged_Base : IpcInterface {
	protected virtual Nn.Apm.ISession OpenSession() =>
		throw new NotImplementedException("Nn.Apm.IManagerPrivileged.OpenSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				var _return = OpenSession();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.IManagerPrivileged");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void SetPerformanceConfiguration(uint _0, uint _1) =>
		"Stub hit for Nn.Apm.ISession.SetPerformanceConfiguration".Log();
	protected virtual uint GetPerformanceConfiguration(uint _0) =>
		throw new NotImplementedException("Nn.Apm.ISession.GetPerformanceConfiguration not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPerformanceConfiguration
				SetPerformanceConfiguration(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetPerformanceConfiguration
				var _return = GetPerformanceConfiguration(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.ISession");
		}
	}
}

public partial class ISystemManager : _ISystemManager_Base {
	public readonly string ServiceName;
	public ISystemManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemManager_Base : IpcInterface {
	protected virtual void RequestPerformanceMode(uint _0) =>
		"Stub hit for Nn.Apm.ISystemManager.RequestPerformanceMode".Log();
	protected virtual KObject GetPerformanceEvent(uint _0) =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetPerformanceEvent not implemented");
	protected virtual void GetThrottlingState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetThrottlingState not implemented");
	protected virtual void GetLastThrottlingState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetLastThrottlingState not implemented");
	protected virtual void ClearLastThrottlingState() =>
		"Stub hit for Nn.Apm.ISystemManager.ClearLastThrottlingState".Log();
	protected virtual void LoadAndApplySettings() =>
		"Stub hit for Nn.Apm.ISystemManager.LoadAndApplySettings".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestPerformanceMode
				RequestPerformanceMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetPerformanceEvent
				var _return = GetPerformanceEvent(im.GetData<uint>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // GetThrottlingState
				GetThrottlingState(out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // GetLastThrottlingState
				GetLastThrottlingState(out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // ClearLastThrottlingState
				ClearLastThrottlingState();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // LoadAndApplySettings
				LoadAndApplySettings();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.ISystemManager");
		}
	}
}

