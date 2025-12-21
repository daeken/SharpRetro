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
		Console.WriteLine("Stub hit for Nn.Apm.IDebugManager.ClearLastThrottlingState");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetThrottlingState
				om.Initialize(0, 0, 40);
				GetThrottlingState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetLastThrottlingState
				om.Initialize(0, 0, 40);
				GetLastThrottlingState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // ClearLastThrottlingState
				om.Initialize(0, 0, 0);
				ClearLastThrottlingState();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.IDebugManager");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Apm.ISession OpenSession() =>
		throw new NotImplementedException("Nn.Apm.IManager.OpenSession not implemented");
	protected virtual uint GetPerformanceMode() =>
		throw new NotImplementedException("Nn.Apm.IManager.GetPerformanceMode not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				om.Initialize(1, 0, 0);
				var _return = OpenSession();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetPerformanceMode
				om.Initialize(0, 0, 4);
				var _return = GetPerformanceMode();
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.IManager");
		}
	}
}

public partial class IManagerPrivileged : _IManagerPrivileged_Base;
public abstract class _IManagerPrivileged_Base : IpcInterface {
	protected virtual Nn.Apm.ISession OpenSession() =>
		throw new NotImplementedException("Nn.Apm.IManagerPrivileged.OpenSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				om.Initialize(1, 0, 0);
				var _return = OpenSession();
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
		Console.WriteLine("Stub hit for Nn.Apm.ISession.SetPerformanceConfiguration");
	protected virtual uint GetPerformanceConfiguration(uint _0) =>
		throw new NotImplementedException("Nn.Apm.ISession.GetPerformanceConfiguration not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPerformanceConfiguration
				om.Initialize(0, 0, 0);
				SetPerformanceConfiguration(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0x1: { // GetPerformanceConfiguration
				om.Initialize(0, 0, 4);
				var _return = GetPerformanceConfiguration(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.ISession");
		}
	}
}

public partial class ISystemManager : _ISystemManager_Base;
public abstract class _ISystemManager_Base : IpcInterface {
	protected virtual void RequestPerformanceMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Apm.ISystemManager.RequestPerformanceMode");
	protected virtual KObject GetPerformanceEvent(uint _0) =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetPerformanceEvent not implemented");
	protected virtual void GetThrottlingState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetThrottlingState not implemented");
	protected virtual void GetLastThrottlingState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetLastThrottlingState not implemented");
	protected virtual void ClearLastThrottlingState() =>
		Console.WriteLine("Stub hit for Nn.Apm.ISystemManager.ClearLastThrottlingState");
	protected virtual void LoadAndApplySettings() =>
		Console.WriteLine("Stub hit for Nn.Apm.ISystemManager.LoadAndApplySettings");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestPerformanceMode
				om.Initialize(0, 0, 0);
				RequestPerformanceMode(im.GetData<uint>(8));
				break;
			}
			case 0x1: { // GetPerformanceEvent
				om.Initialize(0, 1, 0);
				var _return = GetPerformanceEvent(im.GetData<uint>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // GetThrottlingState
				om.Initialize(0, 0, 40);
				GetThrottlingState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // GetLastThrottlingState
				om.Initialize(0, 0, 40);
				GetLastThrottlingState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // ClearLastThrottlingState
				om.Initialize(0, 0, 0);
				ClearLastThrottlingState();
				break;
			}
			case 0x5: { // LoadAndApplySettings
				om.Initialize(0, 0, 0);
				LoadAndApplySettings();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.ISystemManager");
		}
	}
}

