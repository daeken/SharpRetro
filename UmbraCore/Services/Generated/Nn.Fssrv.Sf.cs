using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fssrv.Sf;
public partial class IDeviceOperator : _IDeviceOperator_Base;
public abstract class _IDeviceOperator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // IsSdCardInserted
				break;
			case 0x1: // GetSdCardSpeedMode
				break;
			case 0x2: // GetSdCardCid
				break;
			case 0x3: // GetSdCardUserAreaSize
				break;
			case 0x4: // GetSdCardProtectedAreaSize
				break;
			case 0x5: // GetAndClearSdCardErrorInfo
				break;
			case 0x64: // GetMmcCid
				break;
			case 0x65: // GetMmcSpeedMode
				break;
			case 0x6E: // EraseMmc
				break;
			case 0x6F: // GetMmcPartitionSize
				break;
			case 0x70: // GetMmcPatrolCount
				break;
			case 0x71: // GetAndClearMmcErrorInfo
				break;
			case 0x72: // GetMmcExtendedCsd
				break;
			case 0x73: // SuspendMmcPatrol
				break;
			case 0x74: // ResumeMmcPatrol
				break;
			case 0xC8: // IsGameCardInserted
				break;
			case 0xC9: // EraseGameCard
				break;
			case 0xCA: // GetGameCardHandle
				break;
			case 0xCB: // GetGameCardUpdatePartitionInfo
				break;
			case 0xCC: // FinalizeGameCardDriver
				break;
			case 0xCD: // GetGameCardAttribute
				break;
			case 0xCE: // GetGameCardDeviceCertificate
				break;
			case 0xCF: // GetGameCardAsicInfo
				break;
			case 0xD0: // GetGameCardIdSet
				break;
			case 0xD1: // WriteToGameCard
				break;
			case 0xD2: // SetVerifyWriteEnalbleFlag
				break;
			case 0xD3: // GetGameCardImageHash
				break;
			case 0xD4: // GetGameCardErrorInfo
				break;
			case 0xD5: // EraseAndWriteParamDirectly
				break;
			case 0xD6: // ReadParamDirectly
				break;
			case 0xD7: // ForceEraseGameCard
				break;
			case 0xD8: // GetGameCardErrorInfo2
				break;
			case 0xD9: // GetGameCardErrorReportInfo
				break;
			case 0xDA: // GetGameCardDeviceId
				break;
			case 0x12C: // SetSpeedEmulationMode
				break;
			case 0x12D: // GetSpeedEmulationMode
				break;
			case 0x190: // SuspendSdmmcControl
				break;
			case 0x191: // ResumeSdmmcControl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IDeviceOperator");
		}
	}
}

public partial class IDirectory : _IDirectory_Base;
public abstract class _IDirectory_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Read
				break;
			case 0x1: // GetEntryCount
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IDirectory");
		}
	}
}

public partial class IEventNotifier : _IEventNotifier_Base;
public abstract class _IEventNotifier_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetEventHandle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IEventNotifier");
		}
	}
}

public partial class IFile : _IFile_Base;
public abstract class _IFile_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Read
				break;
			case 0x1: // Write
				break;
			case 0x2: // Flush
				break;
			case 0x3: // SetSize
				break;
			case 0x4: // GetSize
				break;
			case 0x5: // OperateRange
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFile");
		}
	}
}

public partial class IFileSystem : _IFileSystem_Base;
public abstract class _IFileSystem_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateFile
				break;
			case 0x1: // DeleteFile
				break;
			case 0x2: // CreateDirectory
				break;
			case 0x3: // DeleteDirectory
				break;
			case 0x4: // DeleteDirectoryRecursively
				break;
			case 0x5: // RenameFile
				break;
			case 0x6: // RenameDirectory
				break;
			case 0x7: // GetEntryType
				break;
			case 0x8: // OpenFile
				break;
			case 0x9: // OpenDirectory
				break;
			case 0xA: // Commit
				break;
			case 0xB: // GetFreeSpaceSize
				break;
			case 0xC: // GetTotalSpaceSize
				break;
			case 0xD: // CleanDirectoryRecursively
				break;
			case 0xE: // GetFileTimeStampRaw
				break;
			case 0xF: // QueryEntry
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFileSystem");
		}
	}
}

public partial class IFileSystemProxy : _IFileSystemProxy_Base;
public abstract class _IFileSystemProxy_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenFileSystem
				break;
			case 0x1: // SetCurrentProcess
				break;
			case 0x2: // OpenDataFileSystemByCurrentProcess
				break;
			case 0x7: // OpenFileSystemWithPatch
				break;
			case 0x8: // OpenFileSystemWithId
				break;
			case 0x9: // OpenDataFileSystemByApplicationId
				break;
			case 0xB: // OpenBisFileSystem
				break;
			case 0xC: // OpenBisStorage
				break;
			case 0xD: // InvalidateBisCache
				break;
			case 0x11: // OpenHostFileSystem
				break;
			case 0x12: // OpenSdCardFileSystem
				break;
			case 0x13: // FormatSdCardFileSystem
				break;
			case 0x15: // DeleteSaveDataFileSystem
				break;
			case 0x16: // CreateSaveDataFileSystem
				break;
			case 0x17: // CreateSaveDataFileSystemBySystemSaveDataId
				break;
			case 0x18: // RegisterSaveDataFileSystemAtomicDeletion
				break;
			case 0x19: // DeleteSaveDataFileSystemBySaveDataSpaceId
				break;
			case 0x1A: // FormatSdCardDryRun
				break;
			case 0x1B: // IsExFatSupported
				break;
			case 0x1C: // DeleteSaveDataFileSystemBySaveDataAttribute
				break;
			case 0x1E: // OpenGameCardStorage
				break;
			case 0x1F: // OpenGameCardFileSystem
				break;
			case 0x20: // ExtendSaveDataFileSystem
				break;
			case 0x21: // DeleteCacheStorage
				break;
			case 0x22: // GetCacheStorageSize
				break;
			case 0x33: // OpenSaveDataFileSystem
				break;
			case 0x34: // OpenSaveDataFileSystemBySystemSaveDataId
				break;
			case 0x35: // OpenReadOnlySaveDataFileSystem
				break;
			case 0x39: // ReadSaveDataFileSystemExtraDataBySaveDataSpaceId
				break;
			case 0x3A: // ReadSaveDataFileSystemExtraData
				break;
			case 0x3B: // WriteSaveDataFileSystemExtraData
				break;
			case 0x3C: // OpenSaveDataInfoReader
				break;
			case 0x3D: // OpenSaveDataInfoReaderBySaveDataSpaceId
				break;
			case 0x3E: // OpenCacheStorageList
				break;
			case 0x40: // OpenSaveDataInternalStorageFileSystem
				break;
			case 0x41: // UpdateSaveDataMacForDebug
				break;
			case 0x42: // WriteSaveDataFileSystemExtraData2
				break;
			case 0x50: // OpenSaveDataMetaFile
				break;
			case 0x51: // OpenSaveDataTransferManager
				break;
			case 0x52: // OpenSaveDataTransferManagerVersion2
				break;
			case 0x64: // OpenImageDirectoryFileSystem
				break;
			case 0x6E: // OpenContentStorageFileSystem
				break;
			case 0xC8: // OpenDataStorageByCurrentProcess
				break;
			case 0xC9: // OpenDataStorageByProgramId
				break;
			case 0xCA: // OpenDataStorageByDataId
				break;
			case 0xCB: // OpenPatchDataStorageByCurrentProcess
				break;
			case 0x190: // OpenDeviceOperator
				break;
			case 0x1F4: // OpenSdCardDetectionEventNotifier
				break;
			case 0x1F5: // OpenGameCardDetectionEventNotifier
				break;
			case 0x1FE: // OpenSystemDataUpdateEventNotifier
				break;
			case 0x1FF: // NotifySystemDataUpdateEvent
				break;
			case 0x258: // SetCurrentPosixTime
				break;
			case 0x259: // QuerySaveDataTotalSize
				break;
			case 0x25A: // VerifySaveDataFileSystem
				break;
			case 0x25B: // CorruptSaveDataFileSystem
				break;
			case 0x25C: // CreatePaddingFile
				break;
			case 0x25D: // DeleteAllPaddingFiles
				break;
			case 0x25E: // GetRightsId
				break;
			case 0x25F: // RegisterExternalKey
				break;
			case 0x260: // UnregisterAllExternalKey
				break;
			case 0x261: // GetRightsIdByPath
				break;
			case 0x262: // GetRightsIdAndKeyGenerationByPath
				break;
			case 0x263: // SetCurrentPosixTimeWithTimeDifference
				break;
			case 0x264: // GetFreeSpaceSizeForSaveData
				break;
			case 0x265: // VerifySaveDataFileSystemBySaveDataSpaceId
				break;
			case 0x266: // CorruptSaveDataFileSystemBySaveDataSpaceId
				break;
			case 0x267: // QuerySaveDataInternalStorageTotalSize
				break;
			case 0x26C: // SetSdCardEncryptionSeed
				break;
			case 0x276: // SetSdCardAccessibility
				break;
			case 0x277: // IsSdCardAccessible
				break;
			case 0x280: // IsSignedSystemPartitionOnSdCardValid
				break;
			case 0x2BC: // OpenAccessFailureResolver
				break;
			case 0x2BD: // GetAccessFailureDetectionEvent
				break;
			case 0x2BE: // IsAccessFailureDetected
				break;
			case 0x2C6: // ResolveAccessFailure
				break;
			case 0x2D0: // AbandonAccessFailure
				break;
			case 0x320: // GetAndClearFileSystemProxyErrorInfo
				break;
			case 0x3E8: // SetBisRootForHost
				break;
			case 0x3E9: // SetSaveDataSize
				break;
			case 0x3EA: // SetSaveDataRootPath
				break;
			case 0x3EB: // DisableAutoSaveDataCreation
				break;
			case 0x3EC: // SetGlobalAccessLogMode
				break;
			case 0x3ED: // GetGlobalAccessLogMode
				break;
			case 0x3EE: // OutputAccessLogToSdCard
				break;
			case 0x3EF: // RegisterUpdatePartition
				break;
			case 0x3F0: // OpenRegisteredUpdatePartition
				break;
			case 0x3F1: // GetAndClearMemoryReportInfo
				break;
			case 0x3F2: // Unknown1010
				break;
			case 0x44C: // OverrideSaveDataTransferTokenSignVerificationKey
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFileSystemProxy");
		}
	}
}

public partial class IFileSystemProxyForLoader : _IFileSystemProxyForLoader_Base;
public abstract class _IFileSystemProxyForLoader_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenCodeFileSystem
				break;
			case 0x1: // IsArchivedProgram
				break;
			case 0x2: // SetCurrentProcess
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFileSystemProxyForLoader");
		}
	}
}

public partial class IProgramRegistry : _IProgramRegistry_Base;
public abstract class _IProgramRegistry_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterProgram
				break;
			case 0x1: // UnregisterProgram
				break;
			case 0x2: // SetCurrentProcess
				break;
			case 0x100: // SetEnabledProgramVerification
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IProgramRegistry");
		}
	}
}

public partial class ISaveDataExporter : _ISaveDataExporter_Base;
public abstract class _ISaveDataExporter_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataExporter");
		}
	}
}

public partial class ISaveDataImporter : _ISaveDataImporter_Base;
public abstract class _ISaveDataImporter_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataImporter");
		}
	}
}

public partial class ISaveDataInfoReader : _ISaveDataInfoReader_Base;
public abstract class _ISaveDataInfoReader_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ReadSaveDataInfo
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataInfoReader");
		}
	}
}

public partial class ISaveDataTransferManager : _ISaveDataTransferManager_Base;
public abstract class _ISaveDataTransferManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x10: // Unknown16
				break;
			case 0x20: // Unknown32
				break;
			case 0x40: // Unknown64
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataTransferManager");
		}
	}
}

public partial class IStorage : _IStorage_Base;
public abstract class _IStorage_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Read
				break;
			case 0x1: // Write
				break;
			case 0x2: // Flush
				break;
			case 0x3: // SetSize
				break;
			case 0x4: // GetSize
				break;
			case 0x5: // OperateRange
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IStorage");
		}
	}
}

