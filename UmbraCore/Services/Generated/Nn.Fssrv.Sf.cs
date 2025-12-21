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
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseMmc");
	protected virtual ulong GetMmcPartitionSize(uint _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcPartitionSize not implemented");
	protected virtual uint GetMmcPatrolCount() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetMmcPatrolCount not implemented");
	protected virtual void GetAndClearMmcErrorInfo(ulong _0, out byte[] _1, out ulong _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetAndClearMmcErrorInfo not implemented");
	protected virtual void GetMmcExtendedCsd(ulong _0, Span<byte> _1) =>
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
	protected virtual void GetGameCardUpdatePartitionInfo(uint _0, out uint version, out ulong tid) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardUpdatePartitionInfo not implemented");
	protected virtual void FinalizeGameCardDriver() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.FinalizeGameCardDriver");
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
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SetVerifyWriteEnalbleFlag");
	protected virtual void GetGameCardImageHash(uint _0, ulong _1, Span<byte> image_hash) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardImageHash not implemented");
	protected virtual void GetGameCardErrorInfo(ulong _0, ulong _1, Span<byte> _2, Span<byte> error_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorInfo not implemented");
	protected virtual void EraseAndWriteParamDirectly(ulong _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.EraseAndWriteParamDirectly");
	protected virtual void ReadParamDirectly(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.ReadParamDirectly not implemented");
	protected virtual void ForceEraseGameCard() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ForceEraseGameCard");
	protected virtual void GetGameCardErrorInfo2(out byte[] error_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorInfo2 not implemented");
	protected virtual void GetGameCardErrorReportInfo(out byte[] error_report_info) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardErrorReportInfo not implemented");
	protected virtual void GetGameCardDeviceId(ulong _0, Span<byte> device_id) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetGameCardDeviceId not implemented");
	protected virtual void SetSpeedEmulationMode(uint emu_mode) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SetSpeedEmulationMode");
	protected virtual uint GetSpeedEmulationMode() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IDeviceOperator.GetSpeedEmulationMode not implemented");
	protected virtual void SuspendSdmmcControl() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.SuspendSdmmcControl");
	protected virtual void ResumeSdmmcControl() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IDeviceOperator.ResumeSdmmcControl");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // IsSdCardInserted
				om.Initialize(0, 0, 1);
				var _return = IsSdCardInserted();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetSdCardSpeedMode
				om.Initialize(0, 0, 8);
				var _return = GetSdCardSpeedMode();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetSdCardCid
				om.Initialize(0, 0, 0);
				GetSdCardCid(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x3: { // GetSdCardUserAreaSize
				om.Initialize(0, 0, 8);
				var _return = GetSdCardUserAreaSize();
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetSdCardProtectedAreaSize
				om.Initialize(0, 0, 8);
				var _return = GetSdCardProtectedAreaSize();
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetAndClearSdCardErrorInfo
				om.Initialize(0, 0, 24);
				GetAndClearSdCardErrorInfo(im.GetData<ulong>(8), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				om.SetData(24, _1);
				break;
			}
			case 0x64: { // GetMmcCid
				om.Initialize(0, 0, 0);
				GetMmcCid(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x65: { // GetMmcSpeedMode
				om.Initialize(0, 0, 8);
				var _return = GetMmcSpeedMode();
				om.SetData(8, _return);
				break;
			}
			case 0x6E: { // EraseMmc
				om.Initialize(0, 0, 0);
				EraseMmc(im.GetData<uint>(8));
				break;
			}
			case 0x6F: { // GetMmcPartitionSize
				om.Initialize(0, 0, 8);
				var _return = GetMmcPartitionSize(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x70: { // GetMmcPatrolCount
				om.Initialize(0, 0, 4);
				var _return = GetMmcPatrolCount();
				om.SetData(8, _return);
				break;
			}
			case 0x71: { // GetAndClearMmcErrorInfo
				om.Initialize(0, 0, 24);
				GetAndClearMmcErrorInfo(im.GetData<ulong>(8), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				om.SetData(24, _1);
				break;
			}
			case 0x72: { // GetMmcExtendedCsd
				om.Initialize(0, 0, 0);
				GetMmcExtendedCsd(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x73: { // SuspendMmcPatrol
				om.Initialize(0, 0, 0);
				SuspendMmcPatrol();
				break;
			}
			case 0x74: { // ResumeMmcPatrol
				om.Initialize(0, 0, 0);
				ResumeMmcPatrol();
				break;
			}
			case 0xC8: { // IsGameCardInserted
				om.Initialize(0, 0, 1);
				var _return = IsGameCardInserted();
				om.SetData(8, _return);
				break;
			}
			case 0xC9: { // EraseGameCard
				om.Initialize(0, 0, 0);
				EraseGameCard(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xCA: { // GetGameCardHandle
				om.Initialize(0, 0, 4);
				var _return = GetGameCardHandle();
				om.SetData(8, _return);
				break;
			}
			case 0xCB: { // GetGameCardUpdatePartitionInfo
				om.Initialize(0, 0, 16);
				GetGameCardUpdatePartitionInfo(im.GetData<uint>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xCC: { // FinalizeGameCardDriver
				om.Initialize(0, 0, 0);
				FinalizeGameCardDriver();
				break;
			}
			case 0xCD: { // GetGameCardAttribute
				om.Initialize(0, 0, 1);
				var _return = GetGameCardAttribute(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xCE: { // GetGameCardDeviceCertificate
				om.Initialize(0, 0, 0);
				GetGameCardDeviceCertificate(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xCF: { // GetGameCardAsicInfo
				om.Initialize(0, 0, 0);
				GetGameCardAsicInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xD0: { // GetGameCardIdSet
				om.Initialize(0, 0, 0);
				GetGameCardIdSet(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xD1: { // WriteToGameCard
				om.Initialize(0, 0, 0);
				WriteToGameCard(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xD2: { // SetVerifyWriteEnalbleFlag
				om.Initialize(0, 0, 0);
				SetVerifyWriteEnalbleFlag(im.GetData<byte>(8));
				break;
			}
			case 0xD3: { // GetGameCardImageHash
				om.Initialize(0, 0, 0);
				GetGameCardImageHash(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xD4: { // GetGameCardErrorInfo
				om.Initialize(0, 0, 0);
				GetGameCardErrorInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xD5: { // EraseAndWriteParamDirectly
				om.Initialize(0, 0, 0);
				EraseAndWriteParamDirectly(im.GetData<ulong>(8), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0xD6: { // ReadParamDirectly
				om.Initialize(0, 0, 0);
				ReadParamDirectly(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xD7: { // ForceEraseGameCard
				om.Initialize(0, 0, 0);
				ForceEraseGameCard();
				break;
			}
			case 0xD8: { // GetGameCardErrorInfo2
				om.Initialize(0, 0, 16);
				GetGameCardErrorInfo2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD9: { // GetGameCardErrorReportInfo
				om.Initialize(0, 0, 64);
				GetGameCardErrorReportInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xDA: { // GetGameCardDeviceId
				om.Initialize(0, 0, 0);
				GetGameCardDeviceId(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x12C: { // SetSpeedEmulationMode
				om.Initialize(0, 0, 0);
				SetSpeedEmulationMode(im.GetData<uint>(8));
				break;
			}
			case 0x12D: { // GetSpeedEmulationMode
				om.Initialize(0, 0, 4);
				var _return = GetSpeedEmulationMode();
				om.SetData(8, _return);
				break;
			}
			case 0x190: { // SuspendSdmmcControl
				om.Initialize(0, 0, 0);
				SuspendSdmmcControl();
				break;
			}
			case 0x191: { // ResumeSdmmcControl
				om.Initialize(0, 0, 0);
				ResumeSdmmcControl();
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
				om.Initialize(0, 0, 8);
				Read(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // GetEntryCount
				om.Initialize(0, 0, 8);
				var _return = GetEntryCount();
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
				om.Initialize(0, 1, 0);
				var _return = GetEventHandle();
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
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFile.Write");
	protected virtual void Flush() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFile.Flush");
	protected virtual void SetSize(ulong size) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFile.SetSize");
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.GetSize not implemented");
	protected virtual void OperateRange(uint _0, ulong _1, ulong _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFile.OperateRange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Read
				om.Initialize(0, 0, 8);
				Read(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), out var _0, im.GetSpan<byte>(0x46, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // Write
				om.Initialize(0, 0, 0);
				Write(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetSpan<byte>(0x45, 0));
				break;
			}
			case 0x2: { // Flush
				om.Initialize(0, 0, 0);
				Flush();
				break;
			}
			case 0x3: { // SetSize
				om.Initialize(0, 0, 0);
				SetSize(im.GetData<ulong>(8));
				break;
			}
			case 0x4: { // GetSize
				om.Initialize(0, 0, 8);
				var _return = GetSize();
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // OperateRange
				om.Initialize(0, 0, 64);
				OperateRange(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), out var _0);
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
	protected virtual void GetFileTimeStampRaw(Span<byte> path, out byte[] timestamp) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.GetFileTimeStampRaw not implemented");
	protected virtual void QueryEntry(uint _0, Span<byte> path, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystem.QueryEntry not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateFile
				om.Initialize(0, 0, 0);
				CreateFile(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x1: { // DeleteFile
				om.Initialize(0, 0, 0);
				DeleteFile(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x2: { // CreateDirectory
				om.Initialize(0, 0, 0);
				CreateDirectory(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x3: { // DeleteDirectory
				om.Initialize(0, 0, 0);
				DeleteDirectory(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x4: { // DeleteDirectoryRecursively
				om.Initialize(0, 0, 0);
				DeleteDirectoryRecursively(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x5: { // RenameFile
				om.Initialize(0, 0, 0);
				RenameFile(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				break;
			}
			case 0x6: { // RenameDirectory
				om.Initialize(0, 0, 0);
				RenameDirectory(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				break;
			}
			case 0x7: { // GetEntryType
				om.Initialize(0, 0, 1);
				var _return = GetEntryType(im.GetSpan<byte>(0x19, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // OpenFile
				om.Initialize(1, 0, 0);
				var _return = OpenFile(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x9: { // OpenDirectory
				om.Initialize(1, 0, 0);
				var _return = OpenDirectory(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // Commit
				om.Initialize(0, 0, 0);
				Commit();
				break;
			}
			case 0xB: { // GetFreeSpaceSize
				om.Initialize(0, 0, 8);
				var _return = GetFreeSpaceSize(im.GetSpan<byte>(0x19, 0));
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // GetTotalSpaceSize
				om.Initialize(0, 0, 8);
				var _return = GetTotalSpaceSize(im.GetSpan<byte>(0x19, 0));
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // CleanDirectoryRecursively
				om.Initialize(0, 0, 0);
				CleanDirectoryRecursively(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0xE: { // GetFileTimeStampRaw
				om.Initialize(0, 0, 32);
				GetFileTimeStampRaw(im.GetSpan<byte>(0x19, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // QueryEntry
				om.Initialize(0, 0, 0);
				QueryEntry(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				break;
			}
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
	protected virtual void CreateSaveDataFileSystem(byte[] save_struct, byte[] ave_create_struct, byte[] _2) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreateSaveDataFileSystem");
	protected virtual void CreateSaveDataFileSystemBySystemSaveDataId(byte[] _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreateSaveDataFileSystemBySystemSaveDataId");
	protected virtual void RegisterSaveDataFileSystemAtomicDeletion(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterSaveDataFileSystemAtomicDeletion");
	protected virtual void DeleteSaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteSaveDataFileSystemBySaveDataSpaceId");
	protected virtual void FormatSdCardDryRun() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.FormatSdCardDryRun");
	protected virtual byte IsExFatSupported() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.IsExFatSupported not implemented");
	protected virtual void DeleteSaveDataFileSystemBySaveDataAttribute(byte _0, byte[] _1) =>
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
	protected virtual Nn.Fssrv.Sf.IFile OpenSaveDataMetaFile(byte _0, uint _1, byte[] _2) =>
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
	protected virtual void VerifySaveDataFileSystem(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.VerifySaveDataFileSystem not implemented");
	protected virtual void CorruptSaveDataFileSystem(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CorruptSaveDataFileSystem");
	protected virtual void CreatePaddingFile(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CreatePaddingFile");
	protected virtual void DeleteAllPaddingFiles() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.DeleteAllPaddingFiles");
	protected virtual void GetRightsId(byte _0, ulong _1, out byte[] rights) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsId not implemented");
	protected virtual void RegisterExternalKey(byte[] _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.RegisterExternalKey");
	protected virtual void UnregisterAllExternalKey() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.UnregisterAllExternalKey");
	protected virtual void GetRightsIdByPath(Span<byte> _0, out byte[] rights) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsIdByPath not implemented");
	protected virtual void GetRightsIdAndKeyGenerationByPath(Span<byte> _0, out byte _1, out byte[] rights) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetRightsIdAndKeyGenerationByPath not implemented");
	protected virtual void SetCurrentPosixTimeWithTimeDifference(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.SetCurrentPosixTimeWithTimeDifference");
	protected virtual ulong GetFreeSpaceSizeForSaveData(byte _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetFreeSpaceSizeForSaveData not implemented");
	protected virtual void VerifySaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.VerifySaveDataFileSystemBySaveDataSpaceId not implemented");
	protected virtual void CorruptSaveDataFileSystemBySaveDataSpaceId(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.CorruptSaveDataFileSystemBySaveDataSpaceId");
	protected virtual void QuerySaveDataInternalStorageTotalSize() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.QuerySaveDataInternalStorageTotalSize");
	protected virtual void SetSdCardEncryptionSeed(byte[] _0) =>
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
	protected virtual void GetAndClearFileSystemProxyErrorInfo(out byte[] error_info) =>
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
	protected virtual void GetAndClearMemoryReportInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IFileSystemProxy.GetAndClearMemoryReportInfo not implemented");
	protected virtual void Unknown1010() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.Unknown1010");
	protected virtual void OverrideSaveDataTransferTokenSignVerificationKey(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IFileSystemProxy.OverrideSaveDataTransferTokenSignVerificationKey");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenFileSystem(im.GetData<Nn.Fssrv.Sf.FileSystemType>(8), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // SetCurrentProcess
				om.Initialize(0, 0, 0);
				SetCurrentProcess(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x2: { // OpenDataFileSystemByCurrentProcess
				om.Initialize(1, 0, 0);
				var _return = OpenDataFileSystemByCurrentProcess();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x7: { // OpenFileSystemWithPatch
				om.Initialize(1, 0, 0);
				var _return = OpenFileSystemWithPatch(im.GetData<Nn.Fssrv.Sf.FileSystemType>(8), im.GetData<ulong>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x8: { // OpenFileSystemWithId
				om.Initialize(1, 0, 0);
				var _return = OpenFileSystemWithId(im.GetData<Nn.Fssrv.Sf.FileSystemType>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x9: { // OpenDataFileSystemByApplicationId
				om.Initialize(1, 0, 0);
				var _return = OpenDataFileSystemByApplicationId(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // OpenBisFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenBisFileSystem(im.GetData<Nn.Fssrv.Sf.Partition>(8), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC: { // OpenBisStorage
				om.Initialize(1, 0, 0);
				var _return = OpenBisStorage(im.GetData<Nn.Fssrv.Sf.Partition>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xD: { // InvalidateBisCache
				om.Initialize(0, 0, 0);
				InvalidateBisCache();
				break;
			}
			case 0x11: { // OpenHostFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenHostFileSystem(im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12: { // OpenSdCardFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenSdCardFileSystem();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x13: { // FormatSdCardFileSystem
				om.Initialize(0, 0, 0);
				FormatSdCardFileSystem();
				break;
			}
			case 0x15: { // DeleteSaveDataFileSystem
				om.Initialize(0, 0, 0);
				DeleteSaveDataFileSystem(im.GetData<ulong>(8));
				break;
			}
			case 0x16: { // CreateSaveDataFileSystem
				om.Initialize(0, 0, 0);
				CreateSaveDataFileSystem(im.GetBytes(8, 0x40), im.GetBytes(72, 0x40), im.GetBytes(136, 0x10));
				break;
			}
			case 0x17: { // CreateSaveDataFileSystemBySystemSaveDataId
				om.Initialize(0, 0, 0);
				CreateSaveDataFileSystemBySystemSaveDataId(im.GetBytes(8, 0x40), im.GetBytes(72, 0x40));
				break;
			}
			case 0x18: { // RegisterSaveDataFileSystemAtomicDeletion
				om.Initialize(0, 0, 0);
				RegisterSaveDataFileSystemAtomicDeletion(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x19: { // DeleteSaveDataFileSystemBySaveDataSpaceId
				om.Initialize(0, 0, 0);
				DeleteSaveDataFileSystemBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1A: { // FormatSdCardDryRun
				om.Initialize(0, 0, 0);
				FormatSdCardDryRun();
				break;
			}
			case 0x1B: { // IsExFatSupported
				om.Initialize(0, 0, 1);
				var _return = IsExFatSupported();
				om.SetData(8, _return);
				break;
			}
			case 0x1C: { // DeleteSaveDataFileSystemBySaveDataAttribute
				om.Initialize(0, 0, 0);
				DeleteSaveDataFileSystemBySaveDataAttribute(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				break;
			}
			case 0x1E: { // OpenGameCardStorage
				om.Initialize(1, 0, 0);
				var _return = OpenGameCardStorage(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F: { // OpenGameCardFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenGameCardFileSystem(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x20: { // ExtendSaveDataFileSystem
				om.Initialize(0, 0, 0);
				ExtendSaveDataFileSystem(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32));
				break;
			}
			case 0x21: { // DeleteCacheStorage
				om.Initialize(0, 0, 0);
				DeleteCacheStorage();
				break;
			}
			case 0x22: { // GetCacheStorageSize
				om.Initialize(0, 0, 0);
				GetCacheStorageSize();
				break;
			}
			case 0x33: { // OpenSaveDataFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenSaveDataFileSystem(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x34: { // OpenSaveDataFileSystemBySystemSaveDataId
				om.Initialize(1, 0, 0);
				var _return = OpenSaveDataFileSystemBySystemSaveDataId(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x35: { // OpenReadOnlySaveDataFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenReadOnlySaveDataFileSystem(im.GetData<byte>(8), im.GetBytes(16, 0x40));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x39: { // ReadSaveDataFileSystemExtraDataBySaveDataSpaceId
				om.Initialize(0, 0, 0);
				ReadSaveDataFileSystemExtraDataBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x3A: { // ReadSaveDataFileSystemExtraData
				om.Initialize(0, 0, 0);
				ReadSaveDataFileSystemExtraData(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x3B: { // WriteSaveDataFileSystemExtraData
				om.Initialize(0, 0, 0);
				WriteSaveDataFileSystemExtraData(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x3C: { // OpenSaveDataInfoReader
				om.Initialize(1, 0, 0);
				var _return = OpenSaveDataInfoReader();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3D: { // OpenSaveDataInfoReaderBySaveDataSpaceId
				om.Initialize(1, 0, 0);
				var _return = OpenSaveDataInfoReaderBySaveDataSpaceId(im.GetData<byte>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E: { // OpenCacheStorageList
				om.Initialize(0, 0, 0);
				OpenCacheStorageList();
				break;
			}
			case 0x40: { // OpenSaveDataInternalStorageFileSystem
				om.Initialize(0, 0, 0);
				OpenSaveDataInternalStorageFileSystem();
				break;
			}
			case 0x41: { // UpdateSaveDataMacForDebug
				om.Initialize(0, 0, 0);
				UpdateSaveDataMacForDebug();
				break;
			}
			case 0x42: { // WriteSaveDataFileSystemExtraData2
				om.Initialize(0, 0, 0);
				WriteSaveDataFileSystemExtraData2();
				break;
			}
			case 0x50: { // OpenSaveDataMetaFile
				om.Initialize(1, 0, 0);
				var _return = OpenSaveDataMetaFile(im.GetData<byte>(8), im.GetData<uint>(12), im.GetBytes(16, 0x40));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x51: { // OpenSaveDataTransferManager
				om.Initialize(1, 0, 0);
				var _return = OpenSaveDataTransferManager();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x52: { // OpenSaveDataTransferManagerVersion2
				om.Initialize(0, 0, 0);
				OpenSaveDataTransferManagerVersion2();
				break;
			}
			case 0x64: { // OpenImageDirectoryFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenImageDirectoryFileSystem(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6E: { // OpenContentStorageFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenContentStorageFileSystem(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // OpenDataStorageByCurrentProcess
				om.Initialize(1, 0, 0);
				var _return = OpenDataStorageByCurrentProcess();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // OpenDataStorageByProgramId
				om.Initialize(1, 0, 0);
				var _return = OpenDataStorageByProgramId(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCA: { // OpenDataStorageByDataId
				om.Initialize(1, 0, 0);
				var _return = OpenDataStorageByDataId(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCB: { // OpenPatchDataStorageByCurrentProcess
				om.Initialize(1, 0, 0);
				var _return = OpenPatchDataStorageByCurrentProcess();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // OpenDeviceOperator
				om.Initialize(1, 0, 0);
				var _return = OpenDeviceOperator();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F4: { // OpenSdCardDetectionEventNotifier
				om.Initialize(1, 0, 0);
				var _return = OpenSdCardDetectionEventNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F5: { // OpenGameCardDetectionEventNotifier
				om.Initialize(1, 0, 0);
				var _return = OpenGameCardDetectionEventNotifier();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1FE: { // OpenSystemDataUpdateEventNotifier
				om.Initialize(0, 0, 0);
				OpenSystemDataUpdateEventNotifier();
				break;
			}
			case 0x1FF: { // NotifySystemDataUpdateEvent
				om.Initialize(0, 0, 0);
				NotifySystemDataUpdateEvent();
				break;
			}
			case 0x258: { // SetCurrentPosixTime
				om.Initialize(0, 0, 0);
				SetCurrentPosixTime(im.GetData<ulong>(8));
				break;
			}
			case 0x259: { // QuerySaveDataTotalSize
				om.Initialize(0, 0, 8);
				var _return = QuerySaveDataTotalSize(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.SetData(8, _return);
				break;
			}
			case 0x25A: { // VerifySaveDataFileSystem
				om.Initialize(0, 0, 0);
				VerifySaveDataFileSystem(im.GetData<ulong>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x25B: { // CorruptSaveDataFileSystem
				om.Initialize(0, 0, 0);
				CorruptSaveDataFileSystem(im.GetData<ulong>(8));
				break;
			}
			case 0x25C: { // CreatePaddingFile
				om.Initialize(0, 0, 0);
				CreatePaddingFile(im.GetData<ulong>(8));
				break;
			}
			case 0x25D: { // DeleteAllPaddingFiles
				om.Initialize(0, 0, 0);
				DeleteAllPaddingFiles();
				break;
			}
			case 0x25E: { // GetRightsId
				om.Initialize(0, 0, 16);
				GetRightsId(im.GetData<byte>(8), im.GetData<ulong>(16), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x25F: { // RegisterExternalKey
				om.Initialize(0, 0, 0);
				RegisterExternalKey(im.GetBytes(8, 0x10), im.GetBytes(24, 0x10));
				break;
			}
			case 0x260: { // UnregisterAllExternalKey
				om.Initialize(0, 0, 0);
				UnregisterAllExternalKey();
				break;
			}
			case 0x261: { // GetRightsIdByPath
				om.Initialize(0, 0, 16);
				GetRightsIdByPath(im.GetSpan<byte>(0x19, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x262: { // GetRightsIdAndKeyGenerationByPath
				om.Initialize(0, 0, 24);
				GetRightsIdAndKeyGenerationByPath(im.GetSpan<byte>(0x19, 0), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetBytes(16, _1);
				break;
			}
			case 0x263: { // SetCurrentPosixTimeWithTimeDifference
				om.Initialize(0, 0, 0);
				SetCurrentPosixTimeWithTimeDifference(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x264: { // GetFreeSpaceSizeForSaveData
				om.Initialize(0, 0, 8);
				var _return = GetFreeSpaceSizeForSaveData(im.GetData<byte>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x265: { // VerifySaveDataFileSystemBySaveDataSpaceId
				om.Initialize(0, 0, 0);
				VerifySaveDataFileSystemBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x266: { // CorruptSaveDataFileSystemBySaveDataSpaceId
				om.Initialize(0, 0, 0);
				CorruptSaveDataFileSystemBySaveDataSpaceId(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x267: { // QuerySaveDataInternalStorageTotalSize
				om.Initialize(0, 0, 0);
				QuerySaveDataInternalStorageTotalSize();
				break;
			}
			case 0x26C: { // SetSdCardEncryptionSeed
				om.Initialize(0, 0, 0);
				SetSdCardEncryptionSeed(im.GetBytes(8, 0x10));
				break;
			}
			case 0x276: { // SetSdCardAccessibility
				om.Initialize(0, 0, 0);
				SetSdCardAccessibility(im.GetData<byte>(8));
				break;
			}
			case 0x277: { // IsSdCardAccessible
				om.Initialize(0, 0, 1);
				var _return = IsSdCardAccessible();
				om.SetData(8, _return);
				break;
			}
			case 0x280: { // IsSignedSystemPartitionOnSdCardValid
				om.Initialize(0, 0, 1);
				var _return = IsSignedSystemPartitionOnSdCardValid();
				om.SetData(8, _return);
				break;
			}
			case 0x2BC: { // OpenAccessFailureResolver
				om.Initialize(0, 0, 0);
				OpenAccessFailureResolver();
				break;
			}
			case 0x2BD: { // GetAccessFailureDetectionEvent
				om.Initialize(0, 0, 0);
				GetAccessFailureDetectionEvent();
				break;
			}
			case 0x2BE: { // IsAccessFailureDetected
				om.Initialize(0, 0, 0);
				IsAccessFailureDetected();
				break;
			}
			case 0x2C6: { // ResolveAccessFailure
				om.Initialize(0, 0, 0);
				ResolveAccessFailure();
				break;
			}
			case 0x2D0: { // AbandonAccessFailure
				om.Initialize(0, 0, 0);
				AbandonAccessFailure();
				break;
			}
			case 0x320: { // GetAndClearFileSystemProxyErrorInfo
				om.Initialize(0, 0, 128);
				GetAndClearFileSystemProxyErrorInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3E8: { // SetBisRootForHost
				om.Initialize(0, 0, 0);
				SetBisRootForHost(im.GetData<uint>(8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x3E9: { // SetSaveDataSize
				om.Initialize(0, 0, 0);
				SetSaveDataSize(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x3EA: { // SetSaveDataRootPath
				om.Initialize(0, 0, 0);
				SetSaveDataRootPath(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x3EB: { // DisableAutoSaveDataCreation
				om.Initialize(0, 0, 0);
				DisableAutoSaveDataCreation();
				break;
			}
			case 0x3EC: { // SetGlobalAccessLogMode
				om.Initialize(0, 0, 0);
				SetGlobalAccessLogMode(im.GetData<uint>(8));
				break;
			}
			case 0x3ED: { // GetGlobalAccessLogMode
				om.Initialize(0, 0, 4);
				var _return = GetGlobalAccessLogMode();
				om.SetData(8, _return);
				break;
			}
			case 0x3EE: { // OutputAccessLogToSdCard
				om.Initialize(0, 0, 0);
				OutputAccessLogToSdCard(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x3EF: { // RegisterUpdatePartition
				om.Initialize(0, 0, 0);
				RegisterUpdatePartition();
				break;
			}
			case 0x3F0: { // OpenRegisteredUpdatePartition
				om.Initialize(1, 0, 0);
				var _return = OpenRegisteredUpdatePartition();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3F1: { // GetAndClearMemoryReportInfo
				om.Initialize(0, 0, 128);
				GetAndClearMemoryReportInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3F2: { // Unknown1010
				om.Initialize(0, 0, 0);
				Unknown1010();
				break;
			}
			case 0x44C: { // OverrideSaveDataTransferTokenSignVerificationKey
				om.Initialize(0, 0, 0);
				OverrideSaveDataTransferTokenSignVerificationKey(im.GetSpan<byte>(0x5, 0));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenCodeFileSystem
				om.Initialize(1, 0, 0);
				var _return = OpenCodeFileSystem(im.GetData<ulong>(8), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // IsArchivedProgram
				om.Initialize(0, 0, 1);
				var _return = IsArchivedProgram(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // SetCurrentProcess
				om.Initialize(0, 0, 0);
				SetCurrentProcess(im.GetData<ulong>(8), im.Pid);
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterProgram
				om.Initialize(0, 0, 0);
				RegisterProgram(im.GetData<byte>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				break;
			}
			case 0x1: { // UnregisterProgram
				om.Initialize(0, 0, 0);
				UnregisterProgram(im.GetData<ulong>(8));
				break;
			}
			case 0x2: { // SetCurrentProcess
				om.Initialize(0, 0, 0);
				SetCurrentProcess(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x100: { // SetEnabledProgramVerification
				om.Initialize(0, 0, 0);
				SetEnabledProgramVerification(im.GetData<byte>(8));
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
				om.Initialize(0, 0, 0);
				Unknown0(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 8);
				var _return = Unknown1();
				om.SetData(8, _return);
				break;
			}
			case 0x10: { // Unknown16
				om.Initialize(0, 0, 8);
				Unknown16(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x11: { // Unknown17
				om.Initialize(0, 0, 0);
				Unknown17(im.GetSpan<byte>(0x6, 0));
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
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.ISaveDataImporter.Unknown16");
	protected virtual void Unknown17() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.ISaveDataImporter.Unknown17");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 8);
				var _return = Unknown1();
				om.SetData(8, _return);
				break;
			}
			case 0x10: { // Unknown16
				om.Initialize(0, 0, 0);
				Unknown16(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x11: { // Unknown17
				om.Initialize(0, 0, 0);
				Unknown17();
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
				om.Initialize(0, 0, 8);
				ReadSaveDataInfo(out var _0, im.GetSpan<byte>(0x6, 0));
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
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown16");
	protected virtual Nn.Fssrv.Sf.ISaveDataExporter Unknown32(byte _0, ulong _1) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown32 not implemented");
	protected virtual void Unknown64(byte _0, byte[] _1, Span<byte> _2, out ulong _3, out Nn.Fssrv.Sf.ISaveDataImporter _4) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.ISaveDataTransferManager.Unknown64 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x10: { // Unknown16
				om.Initialize(0, 0, 0);
				Unknown16(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x20: { // Unknown32
				om.Initialize(1, 0, 0);
				var _return = Unknown32(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x40: { // Unknown64
				om.Initialize(1, 0, 8);
				Unknown64(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
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
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IStorage.Write");
	protected virtual void Flush() =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IStorage.Flush");
	protected virtual void SetSize(ulong size) =>
		Console.WriteLine("Stub hit for Nn.Fssrv.Sf.IStorage.SetSize");
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.GetSize not implemented");
	protected virtual void OperateRange(uint _0, ulong _1, ulong _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Fssrv.Sf.IStorage.OperateRange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Read
				om.Initialize(0, 0, 0);
				Read(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x46, 0));
				break;
			}
			case 0x1: { // Write
				om.Initialize(0, 0, 0);
				Write(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x45, 0));
				break;
			}
			case 0x2: { // Flush
				om.Initialize(0, 0, 0);
				Flush();
				break;
			}
			case 0x3: { // SetSize
				om.Initialize(0, 0, 0);
				SetSize(im.GetData<ulong>(8));
				break;
			}
			case 0x4: { // GetSize
				om.Initialize(0, 0, 8);
				var _return = GetSize();
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // OperateRange
				om.Initialize(0, 0, 64);
				OperateRange(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fssrv.Sf.IStorage");
		}
	}
}

