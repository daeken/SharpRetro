using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fssrv.Sf;
public enum FileSystemType : uint {
	Invalid = 0x0,
	Invalid2 = 0x1,
	Logo = 0x2,
	ContentControl = 0x3,
	ContentManual = 0x4,
	ContentMeta = 0x5,
	ContentData = 0x6,
	ApplicationPackage = 0x7,
}
public enum Partition : uint {
	BootPartition1Root = 0x0,
	BootPartition2Root = 0xA,
	UserDataRoot = 0x14,
	BootConfigAndPackage2Part1 = 0x15,
	BootConfigAndPackage2Part2 = 0x16,
	BootConfigAndPackage2Part3 = 0x17,
	BootConfigAndPackage2Part4 = 0x18,
	BootConfigAndPackage2Part5 = 0x19,
	BootConfigAndPackage2Part6 = 0x1A,
	CalibrationBinary = 0x1B,
	CalibrationFile = 0x1C,
	SafeMode = 0x1D,
	SystemProperEncryption = 0x1E,
	User = 0x1F,
}
public enum DirectoryEntryType : byte {
	Directory = 0x0,
	File = 0x1,
}
public partial class IDeviceOperator : _IDeviceOperator_Base;
public abstract class _IDeviceOperator_Base : IpcInterface {
	protected virtual byte IsSdCardInserted() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.IsSdCardInserted not implemented");
	protected virtual ulong GetSdCardSpeedMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardSpeedMode not implemented");
	protected virtual void GetSdCardCid(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardCid not implemented");
	protected virtual ulong GetSdCardUserAreaSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardUserAreaSize not implemented");
	protected virtual ulong GetSdCardProtectedAreaSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardProtectedAreaSize not implemented");
	protected virtual void GetAndClearSdCardErrorInfo(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetAndClearSdCardErrorInfo not implemented");
	protected virtual void GetMmcCid(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcCid not implemented");
	protected virtual ulong GetMmcSpeedMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcSpeedMode not implemented");
	protected virtual void EraseMmc(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseMmc");
	protected virtual ulong GetMmcPartitionSize(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcPartitionSize not implemented");
	protected virtual uint GetMmcPatrolCount() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcPatrolCount not implemented");
	protected virtual void GetAndClearMmcErrorInfo(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetAndClearMmcErrorInfo not implemented");
	protected virtual void GetMmcExtendedCsd(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcExtendedCsd not implemented");
	protected virtual void SuspendMmcPatrol() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SuspendMmcPatrol");
	protected virtual void ResumeMmcPatrol() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ResumeMmcPatrol");
	protected virtual byte IsGameCardInserted() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.IsGameCardInserted not implemented");
	protected virtual void EraseGameCard(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseGameCard");
	protected virtual uint GetGameCardHandle() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardHandle not implemented");
	protected virtual void GetGameCardUpdatePartitionInfo(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardUpdatePartitionInfo not implemented");
	protected virtual void FinalizeGameCardDriver() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.FinalizeGameCardDriver");
	protected virtual byte GetGameCardAttribute(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardAttribute not implemented");
	protected virtual void GetGameCardDeviceCertificate(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardDeviceCertificate not implemented");
	protected virtual void GetGameCardAsicInfo(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardAsicInfo not implemented");
	protected virtual void GetGameCardIdSet(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardIdSet not implemented");
	protected virtual void WriteToGameCard(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.WriteToGameCard not implemented");
	protected virtual void SetVerifyWriteEnalbleFlag(byte flag) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SetVerifyWriteEnalbleFlag");
	protected virtual void GetGameCardImageHash(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardImageHash not implemented");
	protected virtual void GetGameCardErrorInfo(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorInfo not implemented");
	protected virtual void EraseAndWriteParamDirectly(ulong _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseAndWriteParamDirectly");
	protected virtual void ReadParamDirectly(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.ReadParamDirectly not implemented");
	protected virtual void ForceEraseGameCard() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ForceEraseGameCard");
	protected virtual void GetGameCardErrorInfo2() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorInfo2 not implemented");
	protected virtual void GetGameCardErrorReportInfo() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorReportInfo not implemented");
	protected virtual void GetGameCardDeviceId(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardDeviceId not implemented");
	protected virtual void SetSpeedEmulationMode(uint emu_mode) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SetSpeedEmulationMode");
	protected virtual uint GetSpeedEmulationMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSpeedEmulationMode not implemented");
	protected virtual void SuspendSdmmcControl() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SuspendSdmmcControl");
	protected virtual void ResumeSdmmcControl() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ResumeSdmmcControl");
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
	protected virtual void Read() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDirectory.Read not implemented");
	protected virtual ulong GetEntryCount() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDirectory.GetEntryCount not implemented");
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
	protected virtual KObject GetEventHandle() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IEventNotifier.GetEventHandle not implemented");
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
	protected virtual void Read(uint _0, ulong offset, ulong size) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.Read not implemented");
	protected virtual void Write(uint _0, ulong offset, ulong size, Span<byte> in_buf) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFile.Write");
	protected virtual void Flush() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFile.Flush");
	protected virtual void SetSize(ulong size) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFile.SetSize");
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.GetSize not implemented");
	protected virtual void OperateRange(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.OperateRange not implemented");
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
	protected virtual void CreateFile(uint mode, ulong size, Span<byte> path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.CreateFile");
	protected virtual void DeleteFile(Span<byte> path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.DeleteFile");
	protected virtual void CreateDirectory(Span<byte> path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.CreateDirectory");
	protected virtual void DeleteDirectory(Span<byte> path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.DeleteDirectory");
	protected virtual void DeleteDirectoryRecursively(Span<byte> path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.DeleteDirectoryRecursively");
	protected virtual void RenameFile(Span<byte> old_path, Span<byte> new_path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.RenameFile");
	protected virtual void RenameDirectory(Span<byte> old_path, Span<byte> new_path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.RenameDirectory");
	protected virtual Nn.Fssrv.Sf.DirectoryEntryType GetEntryType(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetEntryType not implemented");
	protected virtual Nn.Fssrv.Sf.IFile OpenFile(uint mode, Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.OpenFile not implemented");
	protected virtual Nn.Fssrv.Sf.IDirectory OpenDirectory(uint filter_flags, Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.OpenDirectory not implemented");
	protected virtual void Commit() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.Commit");
	protected virtual ulong GetFreeSpaceSize(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetFreeSpaceSize not implemented");
	protected virtual ulong GetTotalSpaceSize(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetTotalSpaceSize not implemented");
	protected virtual void CleanDirectoryRecursively(Span<byte> path) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystem.CleanDirectoryRecursively");
	protected virtual void GetFileTimeStampRaw(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetFileTimeStampRaw not implemented");
	protected virtual void QueryEntry(uint _0, Span<byte> path, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.QueryEntry not implemented");
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
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenFileSystem(Nn.Fssrv.Sf.FileSystemType filesystem_type, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenFileSystem not implemented");
	protected virtual void SetCurrentProcess(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentProcess");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenDataFileSystemByCurrentProcess() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenDataFileSystemByCurrentProcess not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenFileSystemWithPatch(Nn.Fssrv.Sf.FileSystemType filesystem_type, ulong id) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenFileSystemWithPatch not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenFileSystemWithId(Nn.Fssrv.Sf.FileSystemType filesystem_type, ulong tid, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenFileSystemWithId not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenDataFileSystemByApplicationId(ulong u64) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenDataFileSystemByApplicationId not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenBisFileSystem(Nn.Fssrv.Sf.Partition partitionId, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenBisFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IStorage OpenBisStorage(Nn.Fssrv.Sf.Partition partitionId) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenBisStorage not implemented");
	protected virtual void InvalidateBisCache() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.InvalidateBisCache");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenHostFileSystem(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenHostFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenSdCardFileSystem() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSdCardFileSystem not implemented");
	protected virtual void FormatSdCardFileSystem() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.FormatSdCardFileSystem");
	protected virtual void DeleteSaveDataFileSystem(ulong tid) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystem");
	protected virtual void CreateSaveDataFileSystem(Span<byte> save_struct, Span<byte> ave_create_struct, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreateSaveDataFileSystem");
	protected virtual void CreateSaveDataFileSystemBySystemSaveDataId(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreateSaveDataFileSystemBySystemSaveDataId");
	protected virtual void RegisterSaveDataFileSystemAtomicDeletion(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterSaveDataFileSystemAtomicDeletion");
	protected virtual void DeleteSaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystemBySaveDataSpaceId");
	protected virtual void FormatSdCardDryRun() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.FormatSdCardDryRun");
	protected virtual byte IsExFatSupported() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsExFatSupported not implemented");
	protected virtual void DeleteSaveDataFileSystemBySaveDataAttribute(byte _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystemBySaveDataAttribute");
	protected virtual Nn.Fssrv.Sf.IStorage OpenGameCardStorage(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenGameCardStorage not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenGameCardFileSystem(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenGameCardFileSystem not implemented");
	protected virtual void ExtendSaveDataFileSystem(byte _0, ulong _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.ExtendSaveDataFileSystem");
	protected virtual void DeleteCacheStorage() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteCacheStorage");
	protected virtual void GetCacheStorageSize() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.GetCacheStorageSize");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenSaveDataFileSystem(byte save_data_space_id, Span<byte> save_struct) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenSaveDataFileSystemBySystemSaveDataId(byte save_data_space_id, Span<byte> save_struct) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataFileSystemBySystemSaveDataId not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenReadOnlySaveDataFileSystem(byte save_data_space_id, Span<byte> save_struct) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenReadOnlySaveDataFileSystem not implemented");
	protected virtual void ReadSaveDataFileSystemExtraDataBySaveDataSpaceId(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.ReadSaveDataFileSystemExtraDataBySaveDataSpaceId not implemented");
	protected virtual void ReadSaveDataFileSystemExtraData(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.ReadSaveDataFileSystemExtraData not implemented");
	protected virtual void WriteSaveDataFileSystemExtraData(byte _0, ulong _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.WriteSaveDataFileSystemExtraData");
	protected virtual Nn.Fssrv.Sf.ISaveDataInfoReader OpenSaveDataInfoReader() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataInfoReader not implemented");
	protected virtual Nn.Fssrv.Sf.ISaveDataInfoReader OpenSaveDataInfoReaderBySaveDataSpaceId(byte _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataInfoReaderBySaveDataSpaceId not implemented");
	protected virtual void OpenCacheStorageList() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenCacheStorageList");
	protected virtual void OpenSaveDataInternalStorageFileSystem() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataInternalStorageFileSystem");
	protected virtual void UpdateSaveDataMacForDebug() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.UpdateSaveDataMacForDebug");
	protected virtual void WriteSaveDataFileSystemExtraData2() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.WriteSaveDataFileSystemExtraData2");
	protected virtual Nn.Fssrv.Sf.IFile OpenSaveDataMetaFile(byte _0, uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataMetaFile not implemented");
	protected virtual Nn.Fssrv.Sf.ISaveDataTransferManager OpenSaveDataTransferManager() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataTransferManager not implemented");
	protected virtual void OpenSaveDataTransferManagerVersion2() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataTransferManagerVersion2");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenImageDirectoryFileSystem(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenImageDirectoryFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenContentStorageFileSystem(uint content_storage_id) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenContentStorageFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IStorage OpenDataStorageByCurrentProcess() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenDataStorageByCurrentProcess not implemented");
	protected virtual Nn.Fssrv.Sf.IStorage OpenDataStorageByProgramId(ulong tid) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenDataStorageByProgramId not implemented");
	protected virtual Nn.Fssrv.Sf.IStorage OpenDataStorageByDataId(byte storage_id, ulong tid) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenDataStorageByDataId not implemented");
	protected virtual Nn.Fssrv.Sf.IStorage OpenPatchDataStorageByCurrentProcess() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenPatchDataStorageByCurrentProcess not implemented");
	protected virtual Nn.Fssrv.Sf.IDeviceOperator OpenDeviceOperator() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenDeviceOperator not implemented");
	protected virtual Nn.Fssrv.Sf.IEventNotifier OpenSdCardDetectionEventNotifier() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSdCardDetectionEventNotifier not implemented");
	protected virtual Nn.Fssrv.Sf.IEventNotifier OpenGameCardDetectionEventNotifier() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenGameCardDetectionEventNotifier not implemented");
	protected virtual void OpenSystemDataUpdateEventNotifier() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenSystemDataUpdateEventNotifier");
	protected virtual void NotifySystemDataUpdateEvent() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.NotifySystemDataUpdateEvent");
	protected virtual void SetCurrentPosixTime(ulong time) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentPosixTime");
	protected virtual ulong QuerySaveDataTotalSize(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.QuerySaveDataTotalSize not implemented");
	protected virtual void VerifySaveDataFileSystem(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.VerifySaveDataFileSystem not implemented");
	protected virtual void CorruptSaveDataFileSystem(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CorruptSaveDataFileSystem");
	protected virtual void CreatePaddingFile(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreatePaddingFile");
	protected virtual void DeleteAllPaddingFiles() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteAllPaddingFiles");
	protected virtual void GetRightsId(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsId not implemented");
	protected virtual void RegisterExternalKey(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterExternalKey");
	protected virtual void UnregisterAllExternalKey() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.UnregisterAllExternalKey");
	protected virtual void GetRightsIdByPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsIdByPath not implemented");
	protected virtual void GetRightsIdAndKeyGenerationByPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsIdAndKeyGenerationByPath not implemented");
	protected virtual void SetCurrentPosixTimeWithTimeDifference(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentPosixTimeWithTimeDifference");
	protected virtual ulong GetFreeSpaceSizeForSaveData(byte _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetFreeSpaceSizeForSaveData not implemented");
	protected virtual void VerifySaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.VerifySaveDataFileSystemBySaveDataSpaceId not implemented");
	protected virtual void CorruptSaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CorruptSaveDataFileSystemBySaveDataSpaceId");
	protected virtual void QuerySaveDataInternalStorageTotalSize() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.QuerySaveDataInternalStorageTotalSize");
	protected virtual void SetSdCardEncryptionSeed(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSdCardEncryptionSeed");
	protected virtual void SetSdCardAccessibility(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSdCardAccessibility");
	protected virtual byte IsSdCardAccessible() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsSdCardAccessible not implemented");
	protected virtual byte IsSignedSystemPartitionOnSdCardValid() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsSignedSystemPartitionOnSdCardValid not implemented");
	protected virtual void OpenAccessFailureResolver() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenAccessFailureResolver");
	protected virtual void GetAccessFailureDetectionEvent() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.GetAccessFailureDetectionEvent");
	protected virtual void IsAccessFailureDetected() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.IsAccessFailureDetected");
	protected virtual void ResolveAccessFailure() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.ResolveAccessFailure");
	protected virtual void AbandonAccessFailure() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.AbandonAccessFailure");
	protected virtual void GetAndClearFileSystemProxyErrorInfo() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetAndClearFileSystemProxyErrorInfo not implemented");
	protected virtual void SetBisRootForHost(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetBisRootForHost");
	protected virtual void SetSaveDataSize(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSaveDataSize");
	protected virtual void SetSaveDataRootPath(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSaveDataRootPath");
	protected virtual void DisableAutoSaveDataCreation() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DisableAutoSaveDataCreation");
	protected virtual void SetGlobalAccessLogMode(uint mode) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetGlobalAccessLogMode");
	protected virtual uint GetGlobalAccessLogMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetGlobalAccessLogMode not implemented");
	protected virtual void OutputAccessLogToSdCard(Span<byte> log_text) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OutputAccessLogToSdCard");
	protected virtual void RegisterUpdatePartition() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterUpdatePartition");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenRegisteredUpdatePartition() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenRegisteredUpdatePartition not implemented");
	protected virtual void GetAndClearMemoryReportInfo() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetAndClearMemoryReportInfo not implemented");
	protected virtual void Unknown1010() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.Unknown1010");
	protected virtual void OverrideSaveDataTransferTokenSignVerificationKey(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OverrideSaveDataTransferTokenSignVerificationKey");
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
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenCodeFileSystem(ulong Tid, Span<byte> content_path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxyForLoader.OpenCodeFileSystem not implemented");
	protected virtual byte IsArchivedProgram(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxyForLoader.IsArchivedProgram not implemented");
	protected virtual void SetCurrentProcess(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxyForLoader.SetCurrentProcess");
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
	protected virtual void RegisterProgram(byte _0, ulong _1, ulong _2, ulong _3, ulong _4, Span<byte> _5, Span<byte> _6) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IProgramRegistry.RegisterProgram");
	protected virtual void UnregisterProgram(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IProgramRegistry.UnregisterProgram");
	protected virtual void SetCurrentProcess(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IProgramRegistry.SetCurrentProcess");
	protected virtual void SetEnabledProgramVerification(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IProgramRegistry.SetEnabledProgramVerification");
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
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown0 not implemented");
	protected virtual ulong Unknown1() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown1 not implemented");
	protected virtual void Unknown16() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown16 not implemented");
	protected virtual void Unknown17() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown17 not implemented");
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
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataImporter.Unknown0 not implemented");
	protected virtual ulong Unknown1() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataImporter.Unknown1 not implemented");
	protected virtual void Unknown16(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.ISaveDataImporter.Unknown16");
	protected virtual void Unknown17() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.ISaveDataImporter.Unknown17");
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
	protected virtual void ReadSaveDataInfo() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataInfoReader.ReadSaveDataInfo not implemented");
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
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown0 not implemented");
	protected virtual void Unknown16(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown16");
	protected virtual Nn.Fssrv.Sf.ISaveDataExporter Unknown32(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown32 not implemented");
	protected virtual void Unknown64(byte _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown64 not implemented");
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
	protected virtual void Read(ulong offset, ulong length) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.Read not implemented");
	protected virtual void Write(ulong offset, ulong length, Span<byte> data) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IStorage.Write");
	protected virtual void Flush() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IStorage.Flush");
	protected virtual void SetSize(ulong size) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IStorage.SetSize");
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.GetSize not implemented");
	protected virtual void OperateRange(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.OperateRange not implemented");
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

