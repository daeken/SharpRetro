using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bcat.Detail.Ipc;
public partial class IBcatService : _IBcatService_Base;
public abstract class _IBcatService_Base : IpcInterface {
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService RequestSyncDeliveryCache() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCache not implemented");
	protected virtual void RequestSyncDeliveryCacheWithDirectoryName() =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCacheWithDirectoryName");
	protected virtual void CancelSyncDeliveryCacheRequest() =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.CancelSyncDeliveryCacheRequest");
	protected virtual Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService RequestSyncDeliveryCacheWithApplicationId(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCacheWithApplicationId not implemented");
	protected virtual void RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName() =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName");
	protected virtual void SetPassphrase(ulong _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.SetPassphrase");
	protected virtual void RegisterBackgroundDeliveryTask(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.RegisterBackgroundDeliveryTask");
	protected virtual void UnregisterBackgroundDeliveryTask(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.UnregisterBackgroundDeliveryTask");
	protected virtual void BlockDeliveryTask(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.BlockDeliveryTask");
	protected virtual void UnblockDeliveryTask(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.UnblockDeliveryTask");
	protected virtual void EnumerateBackgroundDeliveryTask(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.EnumerateBackgroundDeliveryTask not implemented");
	protected virtual void GetDeliveryList(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.GetDeliveryList not implemented");
	protected virtual void ClearDeliveryCacheStorage(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IBcatService.ClearDeliveryCacheStorage");
	protected virtual void GetPushNotificationLog(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IBcatService.GetPushNotificationLog not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: { // RequestSyncDeliveryCache
				om.Initialize(1, 0, 0);
				var _return = RequestSyncDeliveryCache();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2775: { // RequestSyncDeliveryCacheWithDirectoryName
				om.Initialize(0, 0, 0);
				RequestSyncDeliveryCacheWithDirectoryName();
				break;
			}
			case 0x27D8: { // CancelSyncDeliveryCacheRequest
				om.Initialize(0, 0, 0);
				CancelSyncDeliveryCacheRequest();
				break;
			}
			case 0x4E84: { // RequestSyncDeliveryCacheWithApplicationId
				om.Initialize(1, 0, 0);
				var _return = RequestSyncDeliveryCacheWithApplicationId(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4E85: { // RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName
				om.Initialize(0, 0, 0);
				RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName();
				break;
			}
			case 0x7594: { // SetPassphrase
				om.Initialize(0, 0, 0);
				SetPassphrase(im.GetData<ulong>(8), im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x75F8: { // RegisterBackgroundDeliveryTask
				om.Initialize(0, 0, 0);
				RegisterBackgroundDeliveryTask(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x75F9: { // UnregisterBackgroundDeliveryTask
				om.Initialize(0, 0, 0);
				UnregisterBackgroundDeliveryTask(im.GetData<ulong>(8));
				break;
			}
			case 0x75FA: { // BlockDeliveryTask
				om.Initialize(0, 0, 0);
				BlockDeliveryTask(im.GetData<ulong>(8));
				break;
			}
			case 0x75FB: { // UnblockDeliveryTask
				om.Initialize(0, 0, 0);
				UnblockDeliveryTask(im.GetData<ulong>(8));
				break;
			}
			case 0x15FF4: { // EnumerateBackgroundDeliveryTask
				om.Initialize(0, 0, 4);
				EnumerateBackgroundDeliveryTask(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x16058: { // GetDeliveryList
				om.Initialize(0, 0, 8);
				GetDeliveryList(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x16059: { // ClearDeliveryCacheStorage
				om.Initialize(0, 0, 0);
				ClearDeliveryCacheStorage(im.GetData<ulong>(8));
				break;
			}
			case 0x160BC: { // GetPushNotificationLog
				om.Initialize(0, 0, 4);
				GetPushNotificationLog(out var _0, im.GetSpan<byte>(0x6, 0));
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
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService.Open");
	protected virtual void Read(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService.Read not implemented");
	protected virtual uint GetCount() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService.GetCount not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				om.Initialize(0, 0, 0);
				Open(im.GetBytes(8, 0x20));
				break;
			}
			case 0x1: { // Read
				om.Initialize(0, 0, 4);
				Read(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // GetCount
				om.Initialize(0, 0, 4);
				var _return = GetCount();
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
		Console.WriteLine("Stub hit for Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.Open");
	protected virtual void Read(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.Read not implemented");
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.GetSize not implemented");
	protected virtual void GetDigest(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService.GetDigest not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				om.Initialize(0, 0, 0);
				Open(im.GetBytes(8, 0x20), im.GetBytes(40, 0x20));
				break;
			}
			case 0x1: { // Read
				om.Initialize(0, 0, 8);
				Read(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // GetSize
				om.Initialize(0, 0, 8);
				var _return = GetSize();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetDigest
				om.Initialize(0, 0, 16);
				GetDigest(out var _0);
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
				om.Initialize(0, 1, 0);
				var _return = GetEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // GetImpl
				om.Initialize(0, 0, 0);
				GetImpl(im.GetSpan<byte>(0x1A, 0));
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
				om.Initialize(1, 0, 0);
				var _return = CreateFileService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateDirectoryService
				om.Initialize(1, 0, 0);
				var _return = CreateDirectoryService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // EnumerateDeliveryCacheDirectory
				om.Initialize(0, 0, 4);
				EnumerateDeliveryCacheDirectory(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService");
		}
	}
}

public partial class IServiceCreator : _IServiceCreator_Base;
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
				om.Initialize(1, 0, 0);
				var _return = CreateBcatService(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // CreateDeliveryCacheStorageService
				om.Initialize(1, 0, 0);
				var _return = CreateDeliveryCacheStorageService(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // CreateDeliveryCacheStorageServiceWithApplicationId
				om.Initialize(1, 0, 0);
				var _return = CreateDeliveryCacheStorageServiceWithApplicationId(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IServiceCreator");
		}
	}
}

