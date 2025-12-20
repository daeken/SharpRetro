using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ncm;
public partial class IContentManager : _IContentManager_Base;
public abstract class _IContentManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateContentStorage
				break;
			case 0x1: // CreateContentMetaDatabase
				break;
			case 0x2: // VerifyContentStorage
				break;
			case 0x3: // VerifyContentMetaDatabase
				break;
			case 0x4: // OpenContentStorage
				break;
			case 0x5: // OpenContentMetaDatabase
				break;
			case 0x6: // CloseContentStorageForcibly
				break;
			case 0x7: // CloseContentMetaDatabaseForcibly
				break;
			case 0x8: // CleanupContentMetaDatabase
				break;
			case 0x9: // OpenContentStorage2
				break;
			case 0xA: // CloseContentStorage
				break;
			case 0xB: // OpenContentMetaDatabase2
				break;
			case 0xC: // CloseContentMetaDatabase
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentManager");
		}
	}
}

public partial class IContentMetaDatabase : _IContentMetaDatabase_Base;
public abstract class _IContentMetaDatabase_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Set
				break;
			case 0x1: // Get
				break;
			case 0x2: // Remove
				break;
			case 0x3: // GetContentIdByType
				break;
			case 0x4: // ListContentInfo
				break;
			case 0x5: // List
				break;
			case 0x6: // GetLatestContentMetaKey
				break;
			case 0x7: // ListApplication
				break;
			case 0x8: // Has
				break;
			case 0x9: // HasAll
				break;
			case 0xA: // GetSize
				break;
			case 0xB: // GetRequiredSystemVersion
				break;
			case 0xC: // GetPatchId
				break;
			case 0xD: // DisableForcibly
				break;
			case 0xE: // LookupOrphanContent
				break;
			case 0xF: // Commit
				break;
			case 0x10: // HasContent
				break;
			case 0x11: // ListContentMetaInfo
				break;
			case 0x12: // GetAttributes
				break;
			case 0x13: // GetRequiredApplicationVersion
				break;
			case 0x14: // Unknown20
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentMetaDatabase");
		}
	}
}

public partial class IContentStorage : _IContentStorage_Base;
public abstract class _IContentStorage_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GeneratePlaceHolderId
				break;
			case 0x1: // CreatePlaceHolder
				break;
			case 0x2: // DeletePlaceHolder
				break;
			case 0x3: // HasPlaceHolder
				break;
			case 0x4: // WritePlaceHolder
				break;
			case 0x5: // Register
				break;
			case 0x6: // Delete
				break;
			case 0x7: // Has
				break;
			case 0x8: // GetPath
				break;
			case 0x9: // GetPlaceHolderPath
				break;
			case 0xA: // CleanupAllPlaceHolder
				break;
			case 0xB: // ListPlaceHolder
				break;
			case 0xC: // GetContentCount
				break;
			case 0xD: // ListContentId
				break;
			case 0xE: // GetSize
				break;
			case 0xF: // DisableForcibly
				break;
			case 0x10: // RevertToPlaceHolder
				break;
			case 0x11: // SetPlaceHolderSize
				break;
			case 0x12: // ReadContentIdFile
				break;
			case 0x13: // GetRightsIdFromPlaceHolderId
				break;
			case 0x14: // GetRightsIdFromContentId
				break;
			case 0x15: // WriteContentForDebug
				break;
			case 0x16: // GetFreeSpaceSize
				break;
			case 0x17: // GetTotalSpaceSize
				break;
			case 0x18: // FlushStorage
				break;
			case 0x19: // Unknown25
				break;
			case 0x1A: // Unknown26
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentStorage");
		}
	}
}

