using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bcat.Detail.Ipc;
public partial class IBcatService : _IBcatService_Base;
public abstract class _IBcatService_Base : IpcInterface {
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService RequestSyncDeliveryCache() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCache not implemented");
	protected virtual void RequestSyncDeliveryCacheWithDirectoryName() =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCacheWithDirectoryName".Log();
	protected virtual void CancelSyncDeliveryCacheRequest() =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.CancelSyncDeliveryCacheRequest".Log();
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService RequestSyncDeliveryCacheWithApplicationId(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCacheWithApplicationId not implemented");
	protected virtual void RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName() =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName".Log();
	protected virtual void SetPassphrase(ulong _0, Span<byte> _1) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.SetPassphrase".Log();
	protected virtual void RegisterBackgroundDeliveryTask(uint _0, ulong _1) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.RegisterBackgroundDeliveryTask".Log();
	protected virtual void UnregisterBackgroundDeliveryTask(ulong _0) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.UnregisterBackgroundDeliveryTask".Log();
	protected virtual void BlockDeliveryTask(ulong _0) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.BlockDeliveryTask".Log();
	protected virtual void UnblockDeliveryTask(ulong _0) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.UnblockDeliveryTask".Log();
	protected virtual void EnumerateBackgroundDeliveryTask(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.EnumerateBackgroundDeliveryTask not implemented");
	protected virtual void GetDeliveryList(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.GetDeliveryList not implemented");
	protected virtual void ClearDeliveryCacheStorage(ulong _0) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.ClearDeliveryCacheStorage".Log();
	protected virtual void GetPushNotificationLog(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.GetPushNotificationLog not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: { // RequestSyncDeliveryCache
				var _return = RequestSyncDeliveryCache();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2775: { // RequestSyncDeliveryCacheWithDirectoryName
				RequestSyncDeliveryCacheWithDirectoryName();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x27D8: { // CancelSyncDeliveryCacheRequest
				CancelSyncDeliveryCacheRequest();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4E84: { // RequestSyncDeliveryCacheWithApplicationId
				var _return = RequestSyncDeliveryCacheWithApplicationId(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4E85: { // RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName
				RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7594: { // SetPassphrase
				SetPassphrase(im.GetData<ulong>(8), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x75F8: { // RegisterBackgroundDeliveryTask
				RegisterBackgroundDeliveryTask(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x75F9: { // UnregisterBackgroundDeliveryTask
				UnregisterBackgroundDeliveryTask(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x75FA: { // BlockDeliveryTask
				BlockDeliveryTask(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x75FB: { // UnblockDeliveryTask
				UnblockDeliveryTask(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15FF4: { // EnumerateBackgroundDeliveryTask
				EnumerateBackgroundDeliveryTask(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x16058: { // GetDeliveryList
				GetDeliveryList(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x16059: { // ClearDeliveryCacheStorage
				ClearDeliveryCacheStorage(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x160BC: { // GetPushNotificationLog
				GetPushNotificationLog(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IBcatService");
		}
	}
}

public partial class IDeliveryCacheDirectoryService : _IDeliveryCacheDirectoryService_Base;
public abstract class _IDeliveryCacheDirectoryService_Base : IpcInterface {
	protected virtual void Open(byte[] _0) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService.Open".Log();
	protected virtual void Read(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService.Read not implemented");
	protected virtual uint GetCount() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService.GetCount not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				Open(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Read
				Read(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // GetCount
				var _return = GetCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService");
		}
	}
}

public partial class IDeliveryCacheFileService : _IDeliveryCacheFileService_Base;
public abstract class _IDeliveryCacheFileService_Base : IpcInterface {
	protected virtual void Open(byte[] _0, byte[] _1) =>
		"Stub hit for Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.Open".Log();
	protected virtual void Read(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.Read not implemented");
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.GetSize not implemented");
	protected virtual void GetDigest(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.GetDigest not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				Open(im.GetBytes(8, 0x20), im.GetBytes(40, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Read
				Read(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // GetSize
				var _return = GetSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetDigest
				GetDigest(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService");
		}
	}
}

public partial class IDeliveryCacheProgressService : _IDeliveryCacheProgressService_Base;
public abstract class _IDeliveryCacheProgressService_Base : IpcInterface {
	protected virtual KObject GetEvent() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService.GetEvent not implemented");
	protected virtual void GetImpl(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService.GetImpl not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetEvent
				var _return = GetEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // GetImpl
				GetImpl(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService");
		}
	}
}

public partial class IDeliveryCacheStorageService : _IDeliveryCacheStorageService_Base;
public abstract class _IDeliveryCacheStorageService_Base : IpcInterface {
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService CreateFileService() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService.CreateFileService not implemented");
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService CreateDirectoryService() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService.CreateDirectoryService not implemented");
	protected virtual void EnumerateDeliveryCacheDirectory(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService.EnumerateDeliveryCacheDirectory not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateFileService
				var _return = CreateFileService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateDirectoryService
				var _return = CreateDirectoryService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // EnumerateDeliveryCacheDirectory
				EnumerateDeliveryCacheDirectory(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService");
		}
	}
}

public partial class IServiceCreator : _IServiceCreator_Base {
	public readonly string ServiceName;
	public IServiceCreator(string serviceName) => ServiceName = serviceName;
}
public abstract class _IServiceCreator_Base : IpcInterface {
	protected virtual Nn.Bcat.Detail.Ipc.IBcatService CreateBcatService(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IServiceCreator.CreateBcatService not implemented");
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService CreateDeliveryCacheStorageService(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IServiceCreator.CreateDeliveryCacheStorageService not implemented");
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService CreateDeliveryCacheStorageServiceWithApplicationId(ulong _0) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IServiceCreator.CreateDeliveryCacheStorageServiceWithApplicationId not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateBcatService
				var _return = CreateBcatService(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateDeliveryCacheStorageService
				var _return = CreateDeliveryCacheStorageService(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // CreateDeliveryCacheStorageServiceWithApplicationId
				var _return = CreateDeliveryCacheStorageServiceWithApplicationId(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IServiceCreator");
		}
	}
}

