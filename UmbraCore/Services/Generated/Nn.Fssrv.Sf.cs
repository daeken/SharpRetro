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
	protected virtual void GetSdCardCid(ulong _0, Span<byte> cid) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardCid not implemented");
	protected virtual ulong GetSdCardUserAreaSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardUserAreaSize not implemented");
	protected virtual ulong GetSdCardProtectedAreaSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSdCardProtectedAreaSize not implemented");
	protected virtual void GetAndClearSdCardErrorInfo(ulong _0, out byte[] _1, out ulong _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetAndClearSdCardErrorInfo not implemented");
	protected virtual void GetMmcCid(ulong _0, Span<byte> cid) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcCid not implemented");
	protected virtual ulong GetMmcSpeedMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcSpeedMode not implemented");
	protected virtual void EraseMmc(uint _0) =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseMmc".Log();
	protected virtual ulong GetMmcPartitionSize(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcPartitionSize not implemented");
	protected virtual uint GetMmcPatrolCount() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcPatrolCount not implemented");
	protected virtual void GetAndClearMmcErrorInfo(ulong _0, out byte[] _1, out ulong _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetAndClearMmcErrorInfo not implemented");
	protected virtual void GetMmcExtendedCsd(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcExtendedCsd not implemented");
	protected virtual void SuspendMmcPatrol() =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SuspendMmcPatrol".Log();
	protected virtual void ResumeMmcPatrol() =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ResumeMmcPatrol".Log();
	protected virtual byte IsGameCardInserted() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.IsGameCardInserted not implemented");
	protected virtual void EraseGameCard(uint _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseGameCard".Log();
	protected virtual uint GetGameCardHandle() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardHandle not implemented");
	protected virtual void GetGameCardUpdatePartitionInfo(uint _0, out uint version, out ulong tid) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardUpdatePartitionInfo not implemented");
	protected virtual void FinalizeGameCardDriver() =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.FinalizeGameCardDriver".Log();
	protected virtual byte GetGameCardAttribute(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardAttribute not implemented");
	protected virtual void GetGameCardDeviceCertificate(uint _0, ulong _1, Span<byte> certificate) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardDeviceCertificate not implemented");
	protected virtual void GetGameCardAsicInfo(ulong _0, ulong _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardAsicInfo not implemented");
	protected virtual void GetGameCardIdSet(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardIdSet not implemented");
	protected virtual void WriteToGameCard(ulong _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.WriteToGameCard not implemented");
	protected virtual void SetVerifyWriteEnalbleFlag(byte flag) =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SetVerifyWriteEnalbleFlag".Log();
	protected virtual void GetGameCardImageHash(uint _0, ulong _1, Span<byte> image_hash) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardImageHash not implemented");
	protected virtual void GetGameCardErrorInfo(ulong _0, ulong _1, Span<byte> _2, Span<byte> error_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorInfo not implemented");
	protected virtual void EraseAndWriteParamDirectly(ulong _0, Span<byte> _1) =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseAndWriteParamDirectly".Log();
	protected virtual void ReadParamDirectly(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.ReadParamDirectly not implemented");
	protected virtual void ForceEraseGameCard() =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ForceEraseGameCard".Log();
	protected virtual void GetGameCardErrorInfo2(out byte[] error_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorInfo2 not implemented");
	protected virtual void GetGameCardErrorReportInfo(out byte[] error_report_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorReportInfo not implemented");
	protected virtual void GetGameCardDeviceId(ulong _0, Span<byte> device_id) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardDeviceId not implemented");
	protected virtual void SetSpeedEmulationMode(uint emu_mode) =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SetSpeedEmulationMode".Log();
	protected virtual uint GetSpeedEmulationMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSpeedEmulationMode not implemented");
	protected virtual void SuspendSdmmcControl() =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SuspendSdmmcControl".Log();
	protected virtual void ResumeSdmmcControl() =>
		"Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ResumeSdmmcControl".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // IsSdCardInserted
				var _return = IsSdCardInserted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetSdCardSpeedMode
				var _return = GetSdCardSpeedMode();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetSdCardCid
				GetSdCardCid(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetSdCardUserAreaSize
				var _return = GetSdCardUserAreaSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetSdCardProtectedAreaSize
				var _return = GetSdCardProtectedAreaSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetAndClearSdCardErrorInfo
				GetAndClearSdCardErrorInfo(im.GetData<ulong>(8), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				om.SetData(24, _1);
				break;
			}
			case 0x64: { // GetMmcCid
				GetMmcCid(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // GetMmcSpeedMode
				var _return = GetMmcSpeedMode();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x6E: { // EraseMmc
				EraseMmc(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // GetMmcPartitionSize
				var _return = GetMmcPartitionSize(im.GetData<uint>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x70: { // GetMmcPatrolCount
				var _return = GetMmcPatrolCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x71: { // GetAndClearMmcErrorInfo
				GetAndClearMmcErrorInfo(im.GetData<ulong>(8), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				om.SetData(24, _1);
				break;
			}
			case 0x72: { // GetMmcExtendedCsd
				GetMmcExtendedCsd(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x73: { // SuspendMmcPatrol
				SuspendMmcPatrol();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x74: { // ResumeMmcPatrol
				ResumeMmcPatrol();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC8: { // IsGameCardInserted
				var _return = IsGameCardInserted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xC9: { // EraseGameCard
				EraseGameCard(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // GetGameCardHandle
				var _return = GetGameCardHandle();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xCB: { // GetGameCardUpdatePartitionInfo
				GetGameCardUpdatePartitionInfo(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xCC: { // FinalizeGameCardDriver
				FinalizeGameCardDriver();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCD: { // GetGameCardAttribute
				var _return = GetGameCardAttribute(im.GetData<uint>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xCE: { // GetGameCardDeviceCertificate
				GetGameCardDeviceCertificate(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCF: { // GetGameCardAsicInfo
				GetGameCardAsicInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD0: { // GetGameCardIdSet
				GetGameCardIdSet(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD1: { // WriteToGameCard
				WriteToGameCard(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD2: { // SetVerifyWriteEnalbleFlag
				SetVerifyWriteEnalbleFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD3: { // GetGameCardImageHash
				GetGameCardImageHash(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD4: { // GetGameCardErrorInfo
				GetGameCardErrorInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD5: { // EraseAndWriteParamDirectly
				EraseAndWriteParamDirectly(im.GetData<ulong>(8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD6: { // ReadParamDirectly
				ReadParamDirectly(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD7: { // ForceEraseGameCard
				ForceEraseGameCard();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD8: { // GetGameCardErrorInfo2
				GetGameCardErrorInfo2(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD9: { // GetGameCardErrorReportInfo
				GetGameCardErrorReportInfo(out var _0);
				om.Initialize(0, 0, 64);
				om.SetBytes(8, _0);
				break;
			}
			case 0xDA: { // GetGameCardDeviceId
				GetGameCardDeviceId(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12C: { // SetSpeedEmulationMode
				SetSpeedEmulationMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12D: { // GetSpeedEmulationMode
				var _return = GetSpeedEmulationMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x190: { // SuspendSdmmcControl
				SuspendSdmmcControl();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x191: { // ResumeSdmmcControl
				ResumeSdmmcControl();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IDeviceOperator");
		}
	}
}

public partial class IDirectory : _IDirectory_Base;
public abstract class _IDirectory_Base : IpcInterface {
	protected virtual void Read(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDirectory.Read not implemented");
	protected virtual ulong GetEntryCount() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDirectory.GetEntryCount not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Read
				Read(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // GetEntryCount
				var _return = GetEntryCount();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IDirectory");
		}
	}
}

public partial class IEventNotifier : _IEventNotifier_Base;
public abstract class _IEventNotifier_Base : IpcInterface {
	protected virtual KObject GetEventHandle() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IEventNotifier.GetEventHandle not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetEventHandle
				var _return = GetEventHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IEventNotifier");
		}
	}
}

public partial class IFile : _IFile_Base;
public abstract class _IFile_Base : IpcInterface {
	protected virtual void Read(uint _0, ulong offset, ulong size, out ulong out_size, Span<byte> out_buf) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.Read not implemented");
	protected virtual void Write(uint _0, ulong offset, ulong size, Span<byte> in_buf) =>
		"Stub hit for Nn.Fssrv.Sf.IFile.Write".Log();
	protected virtual void Flush() =>
		"Stub hit for Nn.Fssrv.Sf.IFile.Flush".Log();
	protected virtual void SetSize(ulong size) =>
		"Stub hit for Nn.Fssrv.Sf.IFile.SetSize".Log();
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.GetSize not implemented");
	protected virtual void OperateRange(uint _0, ulong _1, ulong _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.OperateRange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Read
				Read(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // Write
				Write(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetSpan<byte>(0x45, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Flush
				Flush();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // SetSize
				SetSize(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetSize
				var _return = GetSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // OperateRange
				OperateRange(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), out var _0);
				om.Initialize(0, 0, 64);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFile");
		}
	}
}

public partial class IFileSystem : _IFileSystem_Base;
public abstract class _IFileSystem_Base : IpcInterface {
	protected virtual void CreateFile(uint mode, ulong size, Span<byte> path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.CreateFile".Log();
	protected virtual void DeleteFile(Span<byte> path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.DeleteFile".Log();
	protected virtual void CreateDirectory(Span<byte> path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.CreateDirectory".Log();
	protected virtual void DeleteDirectory(Span<byte> path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.DeleteDirectory".Log();
	protected virtual void DeleteDirectoryRecursively(Span<byte> path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.DeleteDirectoryRecursively".Log();
	protected virtual void RenameFile(Span<byte> old_path, Span<byte> new_path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.RenameFile".Log();
	protected virtual void RenameDirectory(Span<byte> old_path, Span<byte> new_path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.RenameDirectory".Log();
	protected virtual Nn.Fssrv.Sf.DirectoryEntryType GetEntryType(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetEntryType not implemented");
	protected virtual Nn.Fssrv.Sf.IFile OpenFile(uint mode, Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.OpenFile not implemented");
	protected virtual Nn.Fssrv.Sf.IDirectory OpenDirectory(uint filter_flags, Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.OpenDirectory not implemented");
	protected virtual void Commit() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.Commit".Log();
	protected virtual ulong GetFreeSpaceSize(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetFreeSpaceSize not implemented");
	protected virtual ulong GetTotalSpaceSize(Span<byte> path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetTotalSpaceSize not implemented");
	protected virtual void CleanDirectoryRecursively(Span<byte> path) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystem.CleanDirectoryRecursively".Log();
	protected virtual void GetFileTimeStampRaw(Span<byte> path, out byte[] timestamp) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetFileTimeStampRaw not implemented");
	protected virtual void QueryEntry(uint _0, Span<byte> path, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.QueryEntry not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateFile
				CreateFile(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // DeleteFile
				DeleteFile(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // CreateDirectory
				CreateDirectory(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // DeleteDirectory
				DeleteDirectory(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // DeleteDirectoryRecursively
				DeleteDirectoryRecursively(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // RenameFile
				RenameFile(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // RenameDirectory
				RenameDirectory(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // GetEntryType
				var _return = GetEntryType(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // OpenFile
				var _return = OpenFile(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x9: { // OpenDirectory
				var _return = OpenDirectory(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // Commit
				Commit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // GetFreeSpaceSize
				var _return = GetFreeSpaceSize(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // GetTotalSpaceSize
				var _return = GetTotalSpaceSize(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // CleanDirectoryRecursively
				CleanDirectoryRecursively(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // GetFileTimeStampRaw
				GetFileTimeStampRaw(im.GetSpan<byte>(0x19, 0), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // QueryEntry
				QueryEntry(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFileSystem");
		}
	}
}

public partial class IFileSystemProxy : _IFileSystemProxy_Base {
	public readonly string ServiceName;
	public IFileSystemProxy(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFileSystemProxy_Base : IpcInterface {
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenFileSystem(Nn.Fssrv.Sf.FileSystemType filesystem_type, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenFileSystem not implemented");
	protected virtual void SetCurrentProcess(ulong _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentProcess".Log();
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
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.InvalidateBisCache".Log();
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenHostFileSystem(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenHostFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenSdCardFileSystem() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSdCardFileSystem not implemented");
	protected virtual void FormatSdCardFileSystem() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.FormatSdCardFileSystem".Log();
	protected virtual void DeleteSaveDataFileSystem(ulong tid) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystem".Log();
	protected virtual void CreateSaveDataFileSystem(byte[] save_struct, byte[] ave_create_struct, byte[] _2) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreateSaveDataFileSystem".Log();
	protected virtual void CreateSaveDataFileSystemBySystemSaveDataId(byte[] _0, byte[] _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreateSaveDataFileSystemBySystemSaveDataId".Log();
	protected virtual void RegisterSaveDataFileSystemAtomicDeletion(Span<byte> _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterSaveDataFileSystemAtomicDeletion".Log();
	protected virtual void DeleteSaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystemBySaveDataSpaceId".Log();
	protected virtual void FormatSdCardDryRun() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.FormatSdCardDryRun".Log();
	protected virtual byte IsExFatSupported() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsExFatSupported not implemented");
	protected virtual void DeleteSaveDataFileSystemBySaveDataAttribute(byte _0, byte[] _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystemBySaveDataAttribute".Log();
	protected virtual Nn.Fssrv.Sf.IStorage OpenGameCardStorage(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenGameCardStorage not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenGameCardFileSystem(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenGameCardFileSystem not implemented");
	protected virtual void ExtendSaveDataFileSystem(byte _0, ulong _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.ExtendSaveDataFileSystem".Log();
	protected virtual void DeleteCacheStorage() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteCacheStorage".Log();
	protected virtual void GetCacheStorageSize() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.GetCacheStorageSize".Log();
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenSaveDataFileSystem(byte save_data_space_id, byte[] save_struct) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataFileSystem not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenSaveDataFileSystemBySystemSaveDataId(byte save_data_space_id, byte[] save_struct) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataFileSystemBySystemSaveDataId not implemented");
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenReadOnlySaveDataFileSystem(byte save_data_space_id, byte[] save_struct) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenReadOnlySaveDataFileSystem not implemented");
	protected virtual void ReadSaveDataFileSystemExtraDataBySaveDataSpaceId(byte _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.ReadSaveDataFileSystemExtraDataBySaveDataSpaceId not implemented");
	protected virtual void ReadSaveDataFileSystemExtraData(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.ReadSaveDataFileSystemExtraData not implemented");
	protected virtual void WriteSaveDataFileSystemExtraData(byte _0, ulong _1, Span<byte> _2) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.WriteSaveDataFileSystemExtraData".Log();
	protected virtual Nn.Fssrv.Sf.ISaveDataInfoReader OpenSaveDataInfoReader() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataInfoReader not implemented");
	protected virtual Nn.Fssrv.Sf.ISaveDataInfoReader OpenSaveDataInfoReaderBySaveDataSpaceId(byte _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataInfoReaderBySaveDataSpaceId not implemented");
	protected virtual void OpenCacheStorageList() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenCacheStorageList".Log();
	protected virtual void OpenSaveDataInternalStorageFileSystem() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataInternalStorageFileSystem".Log();
	protected virtual void UpdateSaveDataMacForDebug() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.UpdateSaveDataMacForDebug".Log();
	protected virtual void WriteSaveDataFileSystemExtraData2() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.WriteSaveDataFileSystemExtraData2".Log();
	protected virtual Nn.Fssrv.Sf.IFile OpenSaveDataMetaFile(byte _0, uint _1, byte[] _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataMetaFile not implemented");
	protected virtual Nn.Fssrv.Sf.ISaveDataTransferManager OpenSaveDataTransferManager() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataTransferManager not implemented");
	protected virtual void OpenSaveDataTransferManagerVersion2() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenSaveDataTransferManagerVersion2".Log();
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
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenSystemDataUpdateEventNotifier".Log();
	protected virtual void NotifySystemDataUpdateEvent() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.NotifySystemDataUpdateEvent".Log();
	protected virtual void SetCurrentPosixTime(ulong time) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentPosixTime".Log();
	protected virtual ulong QuerySaveDataTotalSize(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.QuerySaveDataTotalSize not implemented");
	protected virtual void VerifySaveDataFileSystem(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.VerifySaveDataFileSystem not implemented");
	protected virtual void CorruptSaveDataFileSystem(ulong _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CorruptSaveDataFileSystem".Log();
	protected virtual void CreatePaddingFile(ulong _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreatePaddingFile".Log();
	protected virtual void DeleteAllPaddingFiles() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteAllPaddingFiles".Log();
	protected virtual void GetRightsId(byte _0, ulong _1, out byte[] rights) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsId not implemented");
	protected virtual void RegisterExternalKey(byte[] _0, byte[] _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterExternalKey".Log();
	protected virtual void UnregisterAllExternalKey() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.UnregisterAllExternalKey".Log();
	protected virtual void GetRightsIdByPath(Span<byte> _0, out byte[] rights) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsIdByPath not implemented");
	protected virtual void GetRightsIdAndKeyGenerationByPath(Span<byte> _0, out byte _1, out byte[] rights) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsIdAndKeyGenerationByPath not implemented");
	protected virtual void SetCurrentPosixTimeWithTimeDifference(uint _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentPosixTimeWithTimeDifference".Log();
	protected virtual ulong GetFreeSpaceSizeForSaveData(byte _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetFreeSpaceSizeForSaveData not implemented");
	protected virtual void VerifySaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.VerifySaveDataFileSystemBySaveDataSpaceId not implemented");
	protected virtual void CorruptSaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CorruptSaveDataFileSystemBySaveDataSpaceId".Log();
	protected virtual void QuerySaveDataInternalStorageTotalSize() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.QuerySaveDataInternalStorageTotalSize".Log();
	protected virtual void SetSdCardEncryptionSeed(byte[] _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSdCardEncryptionSeed".Log();
	protected virtual void SetSdCardAccessibility(byte _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSdCardAccessibility".Log();
	protected virtual byte IsSdCardAccessible() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsSdCardAccessible not implemented");
	protected virtual byte IsSignedSystemPartitionOnSdCardValid() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsSignedSystemPartitionOnSdCardValid not implemented");
	protected virtual void OpenAccessFailureResolver() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OpenAccessFailureResolver".Log();
	protected virtual void GetAccessFailureDetectionEvent() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.GetAccessFailureDetectionEvent".Log();
	protected virtual void IsAccessFailureDetected() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.IsAccessFailureDetected".Log();
	protected virtual void ResolveAccessFailure() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.ResolveAccessFailure".Log();
	protected virtual void AbandonAccessFailure() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.AbandonAccessFailure".Log();
	protected virtual void GetAndClearFileSystemProxyErrorInfo(out byte[] error_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetAndClearFileSystemProxyErrorInfo not implemented");
	protected virtual void SetBisRootForHost(uint _0, Span<byte> _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetBisRootForHost".Log();
	protected virtual void SetSaveDataSize(ulong _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSaveDataSize".Log();
	protected virtual void SetSaveDataRootPath(Span<byte> _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetSaveDataRootPath".Log();
	protected virtual void DisableAutoSaveDataCreation() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DisableAutoSaveDataCreation".Log();
	protected virtual void SetGlobalAccessLogMode(uint mode) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetGlobalAccessLogMode".Log();
	protected virtual uint GetGlobalAccessLogMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetGlobalAccessLogMode not implemented");
	protected virtual void OutputAccessLogToSdCard(Span<byte> log_text) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OutputAccessLogToSdCard".Log();
	protected virtual void RegisterUpdatePartition() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterUpdatePartition".Log();
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenRegisteredUpdatePartition() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.OpenRegisteredUpdatePartition not implemented");
	protected virtual void GetAndClearMemoryReportInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetAndClearMemoryReportInfo not implemented");
	protected virtual void Unknown1010() =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.Unknown1010".Log();
	protected virtual void OverrideSaveDataTransferTokenSignVerificationKey(Span<byte> _0) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OverrideSaveDataTransferTokenSignVerificationKey".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenFileSystem
				var _return = OpenFileSystem(im.GetData<Nn.Fssrv.Sf.FileSystemType>(8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // SetCurrentProcess
				SetCurrentProcess(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // OpenDataFileSystemByCurrentProcess
				var _return = OpenDataFileSystemByCurrentProcess();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x7: { // OpenFileSystemWithPatch
				var _return = OpenFileSystemWithPatch(im.GetData<Nn.Fssrv.Sf.FileSystemType>(8), im.GetData<ulong>(16));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x8: { // OpenFileSystemWithId
				var _return = OpenFileSystemWithId(im.GetData<Nn.Fssrv.Sf.FileSystemType>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x9: { // OpenDataFileSystemByApplicationId
				var _return = OpenDataFileSystemByApplicationId(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // OpenBisFileSystem
				var _return = OpenBisFileSystem(im.GetData<Nn.Fssrv.Sf.Partition>(8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC: { // OpenBisStorage
				var _return = OpenBisStorage(im.GetData<Nn.Fssrv.Sf.Partition>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xD: { // InvalidateBisCache
				InvalidateBisCache();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // OpenHostFileSystem
				var _return = OpenHostFileSystem(im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12: { // OpenSdCardFileSystem
				var _return = OpenSdCardFileSystem();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x13: { // FormatSdCardFileSystem
				FormatSdCardFileSystem();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // DeleteSaveDataFileSystem
				DeleteSaveDataFileSystem(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // CreateSaveDataFileSystem
				CreateSaveDataFileSystem(im.GetBytes(8, 0x40), im.GetBytes(72, 0x40), im.GetBytes(136, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // CreateSaveDataFileSystemBySystemSaveDataId
				CreateSaveDataFileSystemBySystemSaveDataId(im.GetBytes(8, 0x40), im.GetBytes(72, 0x40));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // RegisterSaveDataFileSystemAtomicDeletion
				RegisterSaveDataFileSystemAtomicDeletion(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // DeleteSaveDataFileSystemBySaveDataSpaceId
				DeleteSaveDataFileSystemBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // FormatSdCardDryRun
				FormatSdCardDryRun();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // IsExFatSupported
				var _return = IsExFatSupported();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x1C: { // DeleteSaveDataFileSystemBySaveDataAttribute
				DeleteSaveDataFileSystemBySaveDataAttribute(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // OpenGameCardStorage
				var _return = OpenGameCardStorage(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F: { // OpenGameCardFileSystem
				var _return = OpenGameCardFileSystem(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x20: { // ExtendSaveDataFileSystem
				ExtendSaveDataFileSystem(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // DeleteCacheStorage
				DeleteCacheStorage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x22: { // GetCacheStorageSize
				GetCacheStorageSize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x33: { // OpenSaveDataFileSystem
				var _return = OpenSaveDataFileSystem(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x34: { // OpenSaveDataFileSystemBySystemSaveDataId
				var _return = OpenSaveDataFileSystemBySystemSaveDataId(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x35: { // OpenReadOnlySaveDataFileSystem
				var _return = OpenReadOnlySaveDataFileSystem(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x39: { // ReadSaveDataFileSystemExtraDataBySaveDataSpaceId
				ReadSaveDataFileSystemExtraDataBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3A: { // ReadSaveDataFileSystemExtraData
				ReadSaveDataFileSystemExtraData(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3B: { // WriteSaveDataFileSystemExtraData
				WriteSaveDataFileSystemExtraData(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3C: { // OpenSaveDataInfoReader
				var _return = OpenSaveDataInfoReader();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3D: { // OpenSaveDataInfoReaderBySaveDataSpaceId
				var _return = OpenSaveDataInfoReaderBySaveDataSpaceId(im.GetData<byte>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E: { // OpenCacheStorageList
				OpenCacheStorageList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x40: { // OpenSaveDataInternalStorageFileSystem
				OpenSaveDataInternalStorageFileSystem();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x41: { // UpdateSaveDataMacForDebug
				UpdateSaveDataMacForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x42: { // WriteSaveDataFileSystemExtraData2
				WriteSaveDataFileSystemExtraData2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x50: { // OpenSaveDataMetaFile
				var _return = OpenSaveDataMetaFile(im.GetData<byte>(8), im.GetData<uint>(12), im.GetBytes(16, 0x40));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x51: { // OpenSaveDataTransferManager
				var _return = OpenSaveDataTransferManager();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x52: { // OpenSaveDataTransferManagerVersion2
				OpenSaveDataTransferManagerVersion2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // OpenImageDirectoryFileSystem
				var _return = OpenImageDirectoryFileSystem(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // OpenContentStorageFileSystem
				var _return = OpenContentStorageFileSystem(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // OpenDataStorageByCurrentProcess
				var _return = OpenDataStorageByCurrentProcess();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // OpenDataStorageByProgramId
				var _return = OpenDataStorageByProgramId(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCA: { // OpenDataStorageByDataId
				var _return = OpenDataStorageByDataId(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCB: { // OpenPatchDataStorageByCurrentProcess
				var _return = OpenPatchDataStorageByCurrentProcess();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // OpenDeviceOperator
				var _return = OpenDeviceOperator();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F4: { // OpenSdCardDetectionEventNotifier
				var _return = OpenSdCardDetectionEventNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F5: { // OpenGameCardDetectionEventNotifier
				var _return = OpenGameCardDetectionEventNotifier();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1FE: { // OpenSystemDataUpdateEventNotifier
				OpenSystemDataUpdateEventNotifier();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1FF: { // NotifySystemDataUpdateEvent
				NotifySystemDataUpdateEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x258: { // SetCurrentPosixTime
				SetCurrentPosixTime(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x259: { // QuerySaveDataTotalSize
				var _return = QuerySaveDataTotalSize(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x25A: { // VerifySaveDataFileSystem
				VerifySaveDataFileSystem(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25B: { // CorruptSaveDataFileSystem
				CorruptSaveDataFileSystem(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25C: { // CreatePaddingFile
				CreatePaddingFile(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25D: { // DeleteAllPaddingFiles
				DeleteAllPaddingFiles();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25E: { // GetRightsId
				GetRightsId(im.GetData<byte>(8), im.GetData<ulong>(16), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25F: { // RegisterExternalKey
				RegisterExternalKey(im.GetBytes(8, 0x10), im.GetBytes(24, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x260: { // UnregisterAllExternalKey
				UnregisterAllExternalKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x261: { // GetRightsIdByPath
				GetRightsIdByPath(im.GetSpan<byte>(0x19, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x262: { // GetRightsIdAndKeyGenerationByPath
				GetRightsIdAndKeyGenerationByPath(im.GetSpan<byte>(0x19, 0), out var _0, out var _1);
				om.Initialize(0, 0, 24);
				om.SetData(8, _0);
				om.SetBytes(16, _1);
				break;
			}
			case 0x263: { // SetCurrentPosixTimeWithTimeDifference
				SetCurrentPosixTimeWithTimeDifference(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x264: { // GetFreeSpaceSizeForSaveData
				var _return = GetFreeSpaceSizeForSaveData(im.GetData<byte>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x265: { // VerifySaveDataFileSystemBySaveDataSpaceId
				VerifySaveDataFileSystemBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x266: { // CorruptSaveDataFileSystemBySaveDataSpaceId
				CorruptSaveDataFileSystemBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x267: { // QuerySaveDataInternalStorageTotalSize
				QuerySaveDataInternalStorageTotalSize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x26C: { // SetSdCardEncryptionSeed
				SetSdCardEncryptionSeed(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x276: { // SetSdCardAccessibility
				SetSdCardAccessibility(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x277: { // IsSdCardAccessible
				var _return = IsSdCardAccessible();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x280: { // IsSignedSystemPartitionOnSdCardValid
				var _return = IsSignedSystemPartitionOnSdCardValid();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2BC: { // OpenAccessFailureResolver
				OpenAccessFailureResolver();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BD: { // GetAccessFailureDetectionEvent
				GetAccessFailureDetectionEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2BE: { // IsAccessFailureDetected
				IsAccessFailureDetected();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2C6: { // ResolveAccessFailure
				ResolveAccessFailure();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2D0: { // AbandonAccessFailure
				AbandonAccessFailure();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x320: { // GetAndClearFileSystemProxyErrorInfo
				GetAndClearFileSystemProxyErrorInfo(out var _0);
				om.Initialize(0, 0, 128);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3E8: { // SetBisRootForHost
				SetBisRootForHost(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E9: { // SetSaveDataSize
				SetSaveDataSize(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EA: { // SetSaveDataRootPath
				SetSaveDataRootPath(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EB: { // DisableAutoSaveDataCreation
				DisableAutoSaveDataCreation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EC: { // SetGlobalAccessLogMode
				SetGlobalAccessLogMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3ED: { // GetGlobalAccessLogMode
				var _return = GetGlobalAccessLogMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3EE: { // OutputAccessLogToSdCard
				OutputAccessLogToSdCard(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3EF: { // RegisterUpdatePartition
				RegisterUpdatePartition();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F0: { // OpenRegisteredUpdatePartition
				var _return = OpenRegisteredUpdatePartition();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3F1: { // GetAndClearMemoryReportInfo
				GetAndClearMemoryReportInfo(out var _0);
				om.Initialize(0, 0, 128);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3F2: { // Unknown1010
				Unknown1010();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x44C: { // OverrideSaveDataTransferTokenSignVerificationKey
				OverrideSaveDataTransferTokenSignVerificationKey(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFileSystemProxy");
		}
	}
}

public partial class IFileSystemProxyForLoader : _IFileSystemProxyForLoader_Base {
	public readonly string ServiceName;
	public IFileSystemProxyForLoader(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFileSystemProxyForLoader_Base : IpcInterface {
	protected virtual Nn.Fssrv.Sf.IFileSystem OpenCodeFileSystem(ulong Tid, Span<byte> content_path) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxyForLoader.OpenCodeFileSystem not implemented");
	protected virtual byte IsArchivedProgram(ulong _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxyForLoader.IsArchivedProgram not implemented");
	protected virtual void SetCurrentProcess(ulong _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IFileSystemProxyForLoader.SetCurrentProcess".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenCodeFileSystem
				var _return = OpenCodeFileSystem(im.GetData<ulong>(8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // IsArchivedProgram
				var _return = IsArchivedProgram(im.GetData<ulong>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // SetCurrentProcess
				SetCurrentProcess(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IFileSystemProxyForLoader");
		}
	}
}

public partial class IProgramRegistry : _IProgramRegistry_Base {
	public readonly string ServiceName;
	public IProgramRegistry(string serviceName) => ServiceName = serviceName;
}
public abstract class _IProgramRegistry_Base : IpcInterface {
	protected virtual void RegisterProgram(byte _0, ulong _1, ulong _2, ulong _3, ulong _4, Span<byte> _5, Span<byte> _6) =>
		"Stub hit for Nn.Fssrv.Sf.IProgramRegistry.RegisterProgram".Log();
	protected virtual void UnregisterProgram(ulong _0) =>
		"Stub hit for Nn.Fssrv.Sf.IProgramRegistry.UnregisterProgram".Log();
	protected virtual void SetCurrentProcess(ulong _0, ulong _1) =>
		"Stub hit for Nn.Fssrv.Sf.IProgramRegistry.SetCurrentProcess".Log();
	protected virtual void SetEnabledProgramVerification(byte _0) =>
		"Stub hit for Nn.Fssrv.Sf.IProgramRegistry.SetEnabledProgramVerification".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterProgram
				RegisterProgram(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // UnregisterProgram
				UnregisterProgram(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SetCurrentProcess
				SetCurrentProcess(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x100: { // SetEnabledProgramVerification
				SetEnabledProgramVerification(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IProgramRegistry");
		}
	}
}

public partial class ISaveDataExporter : _ISaveDataExporter_Base;
public abstract class _ISaveDataExporter_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown0 not implemented");
	protected virtual ulong Unknown1() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown1 not implemented");
	protected virtual void Unknown16(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown16 not implemented");
	protected virtual void Unknown17(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataExporter.Unknown17 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				var _return = Unknown1();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x11: { // Unknown17
				Unknown17(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataExporter");
		}
	}
}

public partial class ISaveDataImporter : _ISaveDataImporter_Base;
public abstract class _ISaveDataImporter_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataImporter.Unknown0 not implemented");
	protected virtual ulong Unknown1() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataImporter.Unknown1 not implemented");
	protected virtual void Unknown16(Span<byte> _0) =>
		"Stub hit for Nn.Fssrv.Sf.ISaveDataImporter.Unknown16".Log();
	protected virtual void Unknown17() =>
		"Stub hit for Nn.Fssrv.Sf.ISaveDataImporter.Unknown17".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				var _return = Unknown1();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // Unknown17
				Unknown17();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataImporter");
		}
	}
}

public partial class ISaveDataInfoReader : _ISaveDataInfoReader_Base;
public abstract class _ISaveDataInfoReader_Base : IpcInterface {
	protected virtual void ReadSaveDataInfo(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataInfoReader.ReadSaveDataInfo not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ReadSaveDataInfo
				ReadSaveDataInfo(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataInfoReader");
		}
	}
}

public partial class ISaveDataTransferManager : _ISaveDataTransferManager_Base;
public abstract class _ISaveDataTransferManager_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown0 not implemented");
	protected virtual void Unknown16(Span<byte> _0) =>
		"Stub hit for Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown16".Log();
	protected virtual Nn.Fssrv.Sf.ISaveDataExporter Unknown32(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown32 not implemented");
	protected virtual void Unknown64(byte _0, byte[] _1, Span<byte> _2, out ulong _3, out Nn.Fssrv.Sf.ISaveDataImporter _4) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown64 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x20: { // Unknown32
				var _return = Unknown32(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x40: { // Unknown64
				Unknown64(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Initialize(1, 0, 8);
				om.SetData(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.ISaveDataTransferManager");
		}
	}
}

public partial class IStorage : _IStorage_Base;
public abstract class _IStorage_Base : IpcInterface {
	protected virtual void Read(ulong offset, ulong length, Span<byte> data) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.Read not implemented");
	protected virtual void Write(ulong offset, ulong length, Span<byte> data) =>
		"Stub hit for Nn.Fssrv.Sf.IStorage.Write".Log();
	protected virtual void Flush() =>
		"Stub hit for Nn.Fssrv.Sf.IStorage.Flush".Log();
	protected virtual void SetSize(ulong size) =>
		"Stub hit for Nn.Fssrv.Sf.IStorage.SetSize".Log();
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.GetSize not implemented");
	protected virtual void OperateRange(uint _0, ulong _1, ulong _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.OperateRange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Read
				Read(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Write
				Write(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x45, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Flush
				Flush();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // SetSize
				SetSize(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetSize
				var _return = GetSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // OperateRange
				OperateRange(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), out var _0);
				om.Initialize(0, 0, 64);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IStorage");
		}
	}
}

