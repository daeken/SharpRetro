using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Apm;
public partial class IDebugManager : _IDebugManager_Base;
public abstract class _IDebugManager_Base : IpcInterface {
	protected virtual void GetThrottlingState() =>
		throw new NotImplementedException("Nn.Apm.IDebugManager.GetThrottlingState not implemented");
	protected virtual void GetLastThrottlingState() =>
		throw new NotImplementedException("Nn.Apm.IDebugManager.GetLastThrottlingState not implemented");
	protected virtual void ClearLastThrottlingState() =>
		Console.WriteLine("Stub hit for Nn.Apm.IDebugManager.ClearLastThrottlingState");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetThrottlingState
				break;
			case 0x1: // GetLastThrottlingState
				break;
			case 0x2: // ClearLastThrottlingState
				break;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenSession
				break;
			case 0x1: // GetPerformanceMode
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.IManager");
		}
	}
}

public partial class IManagerPrivileged : _IManagerPrivileged_Base;
public abstract class _IManagerPrivileged_Base : IpcInterface {
	protected virtual Nn.Apm.ISession OpenSession() =>
		throw new NotImplementedException("Nn.Apm.IManagerPrivileged.OpenSession not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenSession
				break;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetPerformanceConfiguration
				break;
			case 0x1: // GetPerformanceConfiguration
				break;
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
	protected virtual void GetThrottlingState() =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetThrottlingState not implemented");
	protected virtual void GetLastThrottlingState() =>
		throw new NotImplementedException("Nn.Apm.ISystemManager.GetLastThrottlingState not implemented");
	protected virtual void ClearLastThrottlingState() =>
		Console.WriteLine("Stub hit for Nn.Apm.ISystemManager.ClearLastThrottlingState");
	protected virtual void LoadAndApplySettings() =>
		Console.WriteLine("Stub hit for Nn.Apm.ISystemManager.LoadAndApplySettings");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestPerformanceMode
				break;
			case 0x1: // GetPerformanceEvent
				break;
			case 0x2: // GetThrottlingState
				break;
			case 0x3: // GetLastThrottlingState
				break;
			case 0x4: // ClearLastThrottlingState
				break;
			case 0x5: // LoadAndApplySettings
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Apm.ISystemManager");
		}
	}
}

