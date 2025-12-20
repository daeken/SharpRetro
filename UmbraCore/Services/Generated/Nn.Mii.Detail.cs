using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Mii.Detail;
public partial class IDatabaseService : _IDatabaseService_Base;
public abstract class _IDatabaseService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // IsUpdated
				break;
			case 0x1: // IsFullDatabase
				break;
			case 0x2: // GetCount
				break;
			case 0x3: // Get
				break;
			case 0x4: // Get1
				break;
			case 0x5: // UpdateLatest
				break;
			case 0x6: // BuildRandom
				break;
			case 0x7: // BuildDefault
				break;
			case 0x8: // Get2
				break;
			case 0x9: // Get3
				break;
			case 0xA: // UpdateLatest1
				break;
			case 0xB: // FindIndex
				break;
			case 0xC: // Move
				break;
			case 0xD: // AddOrReplace
				break;
			case 0xE: // Delete
				break;
			case 0xF: // DestroyFile
				break;
			case 0x10: // DeleteFile
				break;
			case 0x11: // Format
				break;
			case 0x12: // Import
				break;
			case 0x13: // Export
				break;
			case 0x14: // IsBrokenDatabaseWithClearFlag
				break;
			case 0x15: // GetIndex
				break;
			case 0x16: // SetInterfaceVersion
				break;
			case 0x17: // Convert
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IDatabaseService");
		}
	}
}

public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDatabaseService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IStaticService");
		}
	}
}

public partial class IImageDatabaseService : _IImageDatabaseService_Base;
public abstract class _IImageDatabaseService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0xA: // Reload
				break;
			case 0xB: // GetCount
				break;
			case 0xC: // IsEmpty
				break;
			case 0xD: // IsFull
				break;
			case 0xE: // GetAttribute
				break;
			case 0xF: // LoadImage
				break;
			case 0x10: // AddOrUpdateImage
				break;
			case 0x11: // DeleteImages
				break;
			case 0x64: // DeleteFile
				break;
			case 0x65: // DestroyFile
				break;
			case 0x66: // ImportFile
				break;
			case 0x67: // ExportFile
				break;
			case 0x68: // ForceInitialize
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IImageDatabaseService");
		}
	}
}

