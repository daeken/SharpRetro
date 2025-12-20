using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bcat.Detail.Ipc;
public partial class IBcatService : _IBcatService_Base;
public abstract class _IBcatService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: // RequestSyncDeliveryCache
				break;
			case 0x2775: // RequestSyncDeliveryCacheWithDirectoryName
				break;
			case 0x27D8: // CancelSyncDeliveryCacheRequest
				break;
			case 0x4E84: // RequestSyncDeliveryCacheWithApplicationId
				break;
			case 0x4E85: // RequestSyncDeliveryCacheWithApplicationIdAndDirectoryName
				break;
			case 0x7594: // SetPassphrase
				break;
			case 0x75F8: // RegisterBackgroundDeliveryTask
				break;
			case 0x75F9: // UnregisterBackgroundDeliveryTask
				break;
			case 0x75FA: // BlockDeliveryTask
				break;
			case 0x75FB: // UnblockDeliveryTask
				break;
			case 0x15FF4: // EnumerateBackgroundDeliveryTask
				break;
			case 0x16058: // GetDeliveryList
				break;
			case 0x16059: // ClearDeliveryCacheStorage
				break;
			case 0x160BC: // GetPushNotificationLog
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IBcatService");
		}
	}
}

public partial class IDeliveryCacheDirectoryService : _IDeliveryCacheDirectoryService_Base;
public abstract class _IDeliveryCacheDirectoryService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Open
				break;
			case 0x1: // Read
				break;
			case 0x2: // GetCount
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheDirectoryService");
		}
	}
}

public partial class IDeliveryCacheFileService : _IDeliveryCacheFileService_Base;
public abstract class _IDeliveryCacheFileService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Open
				break;
			case 0x1: // Read
				break;
			case 0x2: // GetSize
				break;
			case 0x3: // GetDigest
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheFileService");
		}
	}
}

public partial class IDeliveryCacheProgressService : _IDeliveryCacheProgressService_Base;
public abstract class _IDeliveryCacheProgressService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetEvent
				break;
			case 0x1: // GetImpl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheProgressService");
		}
	}
}

public partial class IDeliveryCacheStorageService : _IDeliveryCacheStorageService_Base;
public abstract class _IDeliveryCacheStorageService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateFileService
				break;
			case 0x1: // CreateDirectoryService
				break;
			case 0xA: // EnumerateDeliveryCacheDirectory
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IDeliveryCacheStorageService");
		}
	}
}

public partial class IServiceCreator : _IServiceCreator_Base;
public abstract class _IServiceCreator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateBcatService
				break;
			case 0x1: // CreateDeliveryCacheStorageService
				break;
			case 0x2: // CreateDeliveryCacheStorageServiceWithApplicationId
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bcat.Detail.Ipc.IServiceCreator");
		}
	}
}

