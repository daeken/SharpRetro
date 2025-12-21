using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nim.Detail;
public partial class IAsyncData : _IAsyncData_Base;
public abstract class _IAsyncData_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncData.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncData.Unknown1");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown3 not implemented");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown5 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0();
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 8);
				Unknown2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 8);
				Unknown3(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 40);
				Unknown4(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncData");
		}
	}
}

public partial class IAsyncProgressResult : _IAsyncProgressResult_Base;
public abstract class _IAsyncProgressResult_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncProgressResult.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncProgressResult.Unknown1");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncProgressResult.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncProgressResult.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0();
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 16);
				Unknown2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncProgressResult");
		}
	}
}

public partial class IAsyncResult : _IAsyncResult_Base;
public abstract class _IAsyncResult_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncResult.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncResult.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncResult.Unknown2 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0();
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncResult");
		}
	}
}

public partial class IAsyncValue : _IAsyncValue_Base;
public abstract class _IAsyncValue_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncValue.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncValue.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncValue.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncValue.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 8);
				Unknown0(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetSpan<byte>(0x16, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncValue");
		}
	}
}

public partial class INetworkInstallManager : _INetworkInstallManager_Base;
public abstract class _INetworkInstallManager_Base : IpcInterface {
	protected virtual void CreateSystemUpdateTask(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CreateSystemUpdateTask not implemented");
	protected virtual void DestroySystemUpdateTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.DestroySystemUpdateTask");
	protected virtual void ListSystemUpdateTask(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListSystemUpdateTask not implemented");
	protected virtual void RequestSystemUpdateTaskRun(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestSystemUpdateTaskRun not implemented");
	protected virtual void GetSystemUpdateTaskInfo(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetSystemUpdateTaskInfo not implemented");
	protected virtual void CommitSystemUpdateTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.CommitSystemUpdateTask");
	protected virtual void CreateNetworkInstallTask(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CreateNetworkInstallTask not implemented");
	protected virtual void DestroyNetworkInstallTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.DestroyNetworkInstallTask");
	protected virtual void ListNetworkInstallTask(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListNetworkInstallTask not implemented");
	protected virtual void RequestNetworkInstallTaskRun(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestNetworkInstallTaskRun not implemented");
	protected virtual void GetNetworkInstallTaskInfo(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetNetworkInstallTaskInfo not implemented");
	protected virtual void CommitNetworkInstallTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.CommitNetworkInstallTask");
	protected virtual void RequestLatestSystemUpdateMeta(out KObject _0, out Nn.Nim.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestLatestSystemUpdateMeta not implemented");
	protected virtual void ListApplicationNetworkInstallTask(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplicationNetworkInstallTask not implemented");
	protected virtual void ListNetworkInstallTaskContentMeta(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListNetworkInstallTaskContentMeta not implemented");
	protected virtual void RequestLatestVersion(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestLatestVersion not implemented");
	protected virtual void SetNetworkInstallTaskAttribute(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.SetNetworkInstallTaskAttribute");
	protected virtual void AddNetworkInstallTaskContentMeta(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.AddNetworkInstallTaskContentMeta");
	protected virtual void GetDownloadedSystemDataPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetDownloadedSystemDataPath not implemented");
	protected virtual void CalculateNetworkInstallTaskRequiredSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CalculateNetworkInstallTaskRequiredSize not implemented");
	protected virtual void IsExFatDriverIncluded(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.IsExFatDriverIncluded not implemented");
	protected virtual void GetBackgroundDownloadStressTaskInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetBackgroundDownloadStressTaskInfo not implemented");
	protected virtual void RequestDeviceAuthenticationToken(out KObject _0, out Nn.Nim.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestDeviceAuthenticationToken not implemented");
	protected virtual void RequestGameCardRegistrationStatus(byte[] _0, Span<byte> _1, Span<byte> _2, out KObject _3, out Nn.Nim.Detail.IAsyncValue _4) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestGameCardRegistrationStatus not implemented");
	protected virtual void RequestRegisterGameCard(byte[] _0, Span<byte> _1, Span<byte> _2, out KObject _3, out Nn.Nim.Detail.IAsyncResult _4) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestRegisterGameCard not implemented");
	protected virtual void RequestRegisterNotificationToken(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestRegisterNotificationToken not implemented");
	protected virtual void RequestDownloadTaskList(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncData _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestDownloadTaskList not implemented");
	protected virtual void RequestApplicationControl(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestApplicationControl not implemented");
	protected virtual void RequestLatestApplicationControl(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestLatestApplicationControl not implemented");
	protected virtual void RequestVersionList(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncData _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestVersionList not implemented");
	protected virtual void CreateApplyDeltaTask(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CreateApplyDeltaTask not implemented");
	protected virtual void DestroyApplyDeltaTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.DestroyApplyDeltaTask");
	protected virtual void ListApplicationApplyDeltaTask(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplicationApplyDeltaTask not implemented");
	protected virtual void RequestApplyDeltaTaskRun(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestApplyDeltaTaskRun not implemented");
	protected virtual void GetApplyDeltaTaskInfo(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetApplyDeltaTaskInfo not implemented");
	protected virtual void ListApplyDeltaTask(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplyDeltaTask not implemented");
	protected virtual void CommitApplyDeltaTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.CommitApplyDeltaTask");
	protected virtual void CalculateApplyDeltaTaskRequiredSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CalculateApplyDeltaTaskRequiredSize not implemented");
	protected virtual void PrepareShutdown() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.PrepareShutdown");
	protected virtual void ListApplyDeltaTask2(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplyDeltaTask2 not implemented");
	protected virtual void ClearNotEnoughSpaceStateOfApplyDeltaTask(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.ClearNotEnoughSpaceStateOfApplyDeltaTask");
	protected virtual void Unknown42(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown42 not implemented");
	protected virtual void Unknown43(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown43 not implemented");
	protected virtual void Unknown44(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown44 not implemented");
	protected virtual void Unknown45(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown45 not implemented");
	protected virtual void Unknown46() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.Unknown46");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateSystemUpdateTask
				om.Initialize(0, 0, 16);
				CreateSystemUpdateTask(im.GetBytes(8, 0x18), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // DestroySystemUpdateTask
				om.Initialize(0, 0, 0);
				DestroySystemUpdateTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2: { // ListSystemUpdateTask
				om.Initialize(0, 0, 4);
				ListSystemUpdateTask(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // RequestSystemUpdateTaskRun
				om.Initialize(1, 1, 0);
				RequestSystemUpdateTaskRun(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x4: { // GetSystemUpdateTaskInfo
				om.Initialize(0, 0, 56);
				GetSystemUpdateTaskInfo(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // CommitSystemUpdateTask
				om.Initialize(0, 0, 0);
				CommitSystemUpdateTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0x6: { // CreateNetworkInstallTask
				om.Initialize(0, 0, 16);
				CreateNetworkInstallTask(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // DestroyNetworkInstallTask
				om.Initialize(0, 0, 0);
				DestroyNetworkInstallTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0x8: { // ListNetworkInstallTask
				om.Initialize(0, 0, 4);
				ListNetworkInstallTask(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // RequestNetworkInstallTaskRun
				om.Initialize(1, 1, 0);
				RequestNetworkInstallTaskRun(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0xA: { // GetNetworkInstallTaskInfo
				om.Initialize(0, 0, 64);
				GetNetworkInstallTaskInfo(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // CommitNetworkInstallTask
				om.Initialize(0, 0, 0);
				CommitNetworkInstallTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0xC: { // RequestLatestSystemUpdateMeta
				om.Initialize(1, 1, 0);
				RequestLatestSystemUpdateMeta(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0xE: { // ListApplicationNetworkInstallTask
				om.Initialize(0, 0, 4);
				ListApplicationNetworkInstallTask(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // ListNetworkInstallTaskContentMeta
				om.Initialize(0, 0, 4);
				ListNetworkInstallTaskContentMeta(im.GetBytes(8, 0x14), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // RequestLatestVersion
				om.Initialize(1, 1, 0);
				RequestLatestVersion(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x11: { // SetNetworkInstallTaskAttribute
				om.Initialize(0, 0, 0);
				SetNetworkInstallTaskAttribute(im.GetBytes(8, 0x18));
				break;
			}
			case 0x12: { // AddNetworkInstallTaskContentMeta
				om.Initialize(0, 0, 0);
				AddNetworkInstallTaskContentMeta(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x13: { // GetDownloadedSystemDataPath
				om.Initialize(0, 0, 0);
				GetDownloadedSystemDataPath(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x14: { // CalculateNetworkInstallTaskRequiredSize
				om.Initialize(0, 0, 8);
				CalculateNetworkInstallTaskRequiredSize(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // IsExFatDriverIncluded
				om.Initialize(0, 0, 1);
				IsExFatDriverIncluded(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // GetBackgroundDownloadStressTaskInfo
				om.Initialize(0, 0, 16);
				GetBackgroundDownloadStressTaskInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // RequestDeviceAuthenticationToken
				om.Initialize(1, 1, 0);
				RequestDeviceAuthenticationToken(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x18: { // RequestGameCardRegistrationStatus
				om.Initialize(1, 1, 0);
				RequestGameCardRegistrationStatus(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x19: { // RequestRegisterGameCard
				om.Initialize(1, 1, 0);
				RequestRegisterGameCard(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1A: { // RequestRegisterNotificationToken
				om.Initialize(1, 1, 0);
				RequestRegisterNotificationToken(im.GetBytes(8, 0x28), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1B: { // RequestDownloadTaskList
				om.Initialize(1, 1, 0);
				RequestDownloadTaskList(im.GetBytes(8, 0x28), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1C: { // RequestApplicationControl
				om.Initialize(1, 1, 0);
				RequestApplicationControl(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1D: { // RequestLatestApplicationControl
				om.Initialize(1, 1, 0);
				RequestLatestApplicationControl(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1E: { // RequestVersionList
				om.Initialize(1, 1, 0);
				RequestVersionList(im.GetBytes(8, 0x28), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F: { // CreateApplyDeltaTask
				om.Initialize(0, 0, 16);
				CreateApplyDeltaTask(im.GetBytes(8, 0x20), im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x20: { // DestroyApplyDeltaTask
				om.Initialize(0, 0, 0);
				DestroyApplyDeltaTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0x21: { // ListApplicationApplyDeltaTask
				om.Initialize(0, 0, 4);
				ListApplicationApplyDeltaTask(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x22: { // RequestApplyDeltaTaskRun
				om.Initialize(1, 1, 0);
				RequestApplyDeltaTaskRun(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x23: { // GetApplyDeltaTaskInfo
				om.Initialize(0, 0, 48);
				GetApplyDeltaTaskInfo(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x24: { // ListApplyDeltaTask
				om.Initialize(0, 0, 4);
				ListApplyDeltaTask(im.GetBytes(8, 0x14), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x25: { // CommitApplyDeltaTask
				om.Initialize(0, 0, 0);
				CommitApplyDeltaTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0x26: { // CalculateApplyDeltaTaskRequiredSize
				om.Initialize(0, 0, 8);
				CalculateApplyDeltaTaskRequiredSize(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x27: { // PrepareShutdown
				om.Initialize(0, 0, 0);
				PrepareShutdown();
				break;
			}
			case 0x28: { // ListApplyDeltaTask2
				om.Initialize(0, 0, 4);
				ListApplyDeltaTask2(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x29: { // ClearNotEnoughSpaceStateOfApplyDeltaTask
				om.Initialize(0, 0, 0);
				ClearNotEnoughSpaceStateOfApplyDeltaTask(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2A: { // Unknown42
				om.Initialize(0, 0, 16);
				Unknown42(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2B: { // Unknown43
				om.Initialize(0, 0, 16);
				Unknown43(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C: { // Unknown44
				om.Initialize(0, 0, 1);
				Unknown44(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2D: { // Unknown45
				om.Initialize(0, 0, 8);
				Unknown45(im.GetBytes(8, 0x28), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2E: { // Unknown46
				om.Initialize(0, 0, 0);
				Unknown46();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.INetworkInstallManager");
		}
	}
}

public partial class IShopServiceManager : _IShopServiceManager_Base;
public abstract class _IShopServiceManager_Base : IpcInterface {
	protected virtual void RequestDeviceAuthenticationToken(out KObject _0, out Nn.Nim.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceAuthenticationToken not implemented");
	protected virtual void RequestCachedDeviceAuthenticationToken(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestCachedDeviceAuthenticationToken not implemented");
	protected virtual void RequestRegisterDeviceAccount(out KObject _0, out Nn.Nim.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestRegisterDeviceAccount not implemented");
	protected virtual void RequestUnregisterDeviceAccount(out KObject _0, out Nn.Nim.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnregisterDeviceAccount not implemented");
	protected virtual void RequestDeviceAccountStatus(out KObject _0, out Nn.Nim.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceAccountStatus not implemented");
	protected virtual void GetDeviceAccountInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.GetDeviceAccountInfo not implemented");
	protected virtual void RequestDeviceRegistrationInfo(out KObject _0, out Nn.Nim.Detail.IAsyncValue _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceRegistrationInfo not implemented");
	protected virtual void RequestTransferDeviceAccount(out KObject _0, out Nn.Nim.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestTransferDeviceAccount not implemented");
	protected virtual void RequestSyncRegistration(out KObject _0, out Nn.Nim.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestSyncRegistration not implemented");
	protected virtual void IsOwnDeviceId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.IsOwnDeviceId not implemented");
	protected virtual void RequestRegisterNotificationToken(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestRegisterNotificationToken not implemented");
	protected virtual void RequestUnlinkDevice(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnlinkDevice not implemented");
	protected virtual void RequestUnlinkDeviceIntegrated(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnlinkDeviceIntegrated not implemented");
	protected virtual void RequestLinkDevice(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestLinkDevice not implemented");
	protected virtual void HasDeviceLink(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.HasDeviceLink not implemented");
	protected virtual void RequestUnlinkDeviceAll(out KObject _0, out Nn.Nim.Detail.IAsyncResult _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnlinkDeviceAll not implemented");
	protected virtual void RequestCreateVirtualAccount(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestCreateVirtualAccount not implemented");
	protected virtual void RequestDeviceLinkStatus(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceLinkStatus not implemented");
	protected virtual void GetAccountByVirtualAccount(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.GetAccountByVirtualAccount not implemented");
	protected virtual void RequestSyncTicket(out KObject _0, out Nn.Nim.Detail.IAsyncProgressResult _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestSyncTicket not implemented");
	protected virtual void RequestDownloadTicket(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncResult _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDownloadTicket not implemented");
	protected virtual void RequestDownloadTicketForPrepurchasedContents(byte[] _0, out KObject _1, out Nn.Nim.Detail.IAsyncValue _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDownloadTicketForPrepurchasedContents not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestDeviceAuthenticationToken
				om.Initialize(1, 1, 0);
				RequestDeviceAuthenticationToken(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1: { // RequestCachedDeviceAuthenticationToken
				om.Initialize(1, 1, 0);
				RequestCachedDeviceAuthenticationToken(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x64: { // RequestRegisterDeviceAccount
				om.Initialize(1, 1, 0);
				RequestRegisterDeviceAccount(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x65: { // RequestUnregisterDeviceAccount
				om.Initialize(1, 1, 0);
				RequestUnregisterDeviceAccount(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x66: { // RequestDeviceAccountStatus
				om.Initialize(1, 1, 0);
				RequestDeviceAccountStatus(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x67: { // GetDeviceAccountInfo
				om.Initialize(0, 0, 32);
				GetDeviceAccountInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x68: { // RequestDeviceRegistrationInfo
				om.Initialize(1, 1, 0);
				RequestDeviceRegistrationInfo(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x69: { // RequestTransferDeviceAccount
				om.Initialize(1, 1, 0);
				RequestTransferDeviceAccount(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x6A: { // RequestSyncRegistration
				om.Initialize(1, 1, 0);
				RequestSyncRegistration(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x6B: { // IsOwnDeviceId
				om.Initialize(0, 0, 1);
				IsOwnDeviceId(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC8: { // RequestRegisterNotificationToken
				om.Initialize(1, 1, 0);
				RequestRegisterNotificationToken(im.GetBytes(8, 0x28), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x12C: { // RequestUnlinkDevice
				om.Initialize(1, 1, 0);
				RequestUnlinkDevice(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x12D: { // RequestUnlinkDeviceIntegrated
				om.Initialize(1, 1, 0);
				RequestUnlinkDeviceIntegrated(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x12E: { // RequestLinkDevice
				om.Initialize(1, 1, 0);
				RequestLinkDevice(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x12F: { // HasDeviceLink
				om.Initialize(0, 0, 1);
				HasDeviceLink(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x130: { // RequestUnlinkDeviceAll
				om.Initialize(1, 1, 0);
				RequestUnlinkDeviceAll(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x131: { // RequestCreateVirtualAccount
				om.Initialize(1, 1, 0);
				RequestCreateVirtualAccount(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x132: { // RequestDeviceLinkStatus
				om.Initialize(1, 1, 0);
				RequestDeviceLinkStatus(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x190: { // GetAccountByVirtualAccount
				om.Initialize(0, 0, 16);
				GetAccountByVirtualAccount(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1F4: { // RequestSyncTicket
				om.Initialize(1, 1, 0);
				RequestSyncTicket(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F5: { // RequestDownloadTicket
				om.Initialize(1, 1, 0);
				RequestDownloadTicket(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1F6: { // RequestDownloadTicketForPrepurchasedContents
				om.Initialize(1, 1, 0);
				RequestDownloadTicketForPrepurchasedContents(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IShopServiceManager");
		}
	}
}

