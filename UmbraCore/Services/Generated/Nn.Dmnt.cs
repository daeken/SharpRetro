using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Dmnt;
public partial class IInterface : _IInterface_Base {
	public readonly string ServiceName;
	public IInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IInterface_Base : IpcInterface {
	protected virtual void BreakDebugProcess() =>
		"Stub hit for Nn.Dmnt.IInterface.BreakDebugProcess".Log();
	protected virtual void TerminateDebugProcess() =>
		"Stub hit for Nn.Dmnt.IInterface.TerminateDebugProcess".Log();
	protected virtual void CloseHandle() =>
		"Stub hit for Nn.Dmnt.IInterface.CloseHandle".Log();
	protected virtual void LoadImage() =>
		"Stub hit for Nn.Dmnt.IInterface.LoadImage".Log();
	protected virtual void GetProcessId() =>
		"Stub hit for Nn.Dmnt.IInterface.GetProcessId".Log();
	protected virtual void GetProcessHandle() =>
		"Stub hit for Nn.Dmnt.IInterface.GetProcessHandle".Log();
	protected virtual void WaitSynchronization() =>
		"Stub hit for Nn.Dmnt.IInterface.WaitSynchronization".Log();
	protected virtual void GetDebugEvent() =>
		"Stub hit for Nn.Dmnt.IInterface.GetDebugEvent".Log();
	protected virtual void GetProcessModuleInfo() =>
		"Stub hit for Nn.Dmnt.IInterface.GetProcessModuleInfo".Log();
	protected virtual void GetProcessList() =>
		"Stub hit for Nn.Dmnt.IInterface.GetProcessList".Log();
	protected virtual void GetThreadList() =>
		"Stub hit for Nn.Dmnt.IInterface.GetThreadList".Log();
	protected virtual void GetDebugThreadContext() =>
		"Stub hit for Nn.Dmnt.IInterface.GetDebugThreadContext".Log();
	protected virtual void ContinueDebugEvent() =>
		"Stub hit for Nn.Dmnt.IInterface.ContinueDebugEvent".Log();
	protected virtual void ReadDebugProcessMemory() =>
		"Stub hit for Nn.Dmnt.IInterface.ReadDebugProcessMemory".Log();
	protected virtual void WriteDebugProcessMemory() =>
		"Stub hit for Nn.Dmnt.IInterface.WriteDebugProcessMemory".Log();
	protected virtual void SetDebugThreadContext() =>
		"Stub hit for Nn.Dmnt.IInterface.SetDebugThreadContext".Log();
	protected virtual void GetDebugThreadParam() =>
		"Stub hit for Nn.Dmnt.IInterface.GetDebugThreadParam".Log();
	protected virtual void InitializeThreadInfo() =>
		"Stub hit for Nn.Dmnt.IInterface.InitializeThreadInfo".Log();
	protected virtual void SetHardwareBreakPoint() =>
		"Stub hit for Nn.Dmnt.IInterface.SetHardwareBreakPoint".Log();
	protected virtual void QueryDebugProcessMemory() =>
		"Stub hit for Nn.Dmnt.IInterface.QueryDebugProcessMemory".Log();
	protected virtual void GetProcessMemoryDetails() =>
		"Stub hit for Nn.Dmnt.IInterface.GetProcessMemoryDetails".Log();
	protected virtual void AttachByProgramId() =>
		"Stub hit for Nn.Dmnt.IInterface.AttachByProgramId".Log();
	protected virtual void AttachOnLaunch() =>
		"Stub hit for Nn.Dmnt.IInterface.AttachOnLaunch".Log();
	protected virtual void GetDebugMonitorProcessId() =>
		"Stub hit for Nn.Dmnt.IInterface.GetDebugMonitorProcessId".Log();
	protected virtual void GetJitDebugProcessList() =>
		"Stub hit for Nn.Dmnt.IInterface.GetJitDebugProcessList".Log();
	protected virtual void CreateCoreDump() =>
		"Stub hit for Nn.Dmnt.IInterface.CreateCoreDump".Log();
	protected virtual void GetAllDebugThreadInfo() =>
		"Stub hit for Nn.Dmnt.IInterface.GetAllDebugThreadInfo".Log();
	protected virtual void TargetIO_FileOpen() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileOpen".Log();
	protected virtual void TargetIO_FileClose() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileClose".Log();
	protected virtual void TargetIO_FileRead() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileRead".Log();
	protected virtual void TargetIO_FileWrite() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileWrite".Log();
	protected virtual void TargetIO_FileSetAttributes() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileSetAttributes".Log();
	protected virtual void TargetIO_FileGetInformation() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileGetInformation".Log();
	protected virtual void TargetIO_FileSetTime() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileSetTime".Log();
	protected virtual void TargetIO_FileSetSize() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileSetSize".Log();
	protected virtual void TargetIO_FileDelete() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileDelete".Log();
	protected virtual void TargetIO_FileMove() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_FileMove".Log();
	protected virtual void TargetIO_DirectoryCreate() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryCreate".Log();
	protected virtual void TargetIO_DirectoryDelete() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryDelete".Log();
	protected virtual void TargetIO_DirectoryRename() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryRename".Log();
	protected virtual void TargetIO_DirectoryGetCount() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryGetCount".Log();
	protected virtual void TargetIO_DirectoryOpen() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryOpen".Log();
	protected virtual void TargetIO_DirectoryGetNext() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryGetNext".Log();
	protected virtual void TargetIO_DirectoryClose() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryClose".Log();
	protected virtual void TargetIO_GetFreeSpace() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_GetFreeSpace".Log();
	protected virtual void TargetIO_GetVolumeInformation() =>
		"Stub hit for Nn.Dmnt.IInterface.TargetIO_GetVolumeInformation".Log();
	protected virtual void InitiateCoreDump() =>
		"Stub hit for Nn.Dmnt.IInterface.InitiateCoreDump".Log();
	protected virtual void ContinueCoreDump() =>
		"Stub hit for Nn.Dmnt.IInterface.ContinueCoreDump".Log();
	protected virtual void AddTTYToCoreDump() =>
		"Stub hit for Nn.Dmnt.IInterface.AddTTYToCoreDump".Log();
	protected virtual void AddImageToCoreDump() =>
		"Stub hit for Nn.Dmnt.IInterface.AddImageToCoreDump".Log();
	protected virtual void CloseCoreDump() =>
		"Stub hit for Nn.Dmnt.IInterface.CloseCoreDump".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BreakDebugProcess
				BreakDebugProcess();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // TerminateDebugProcess
				TerminateDebugProcess();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // CloseHandle
				CloseHandle();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // LoadImage
				LoadImage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetProcessId
				GetProcessId();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetProcessHandle
				GetProcessHandle();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // WaitSynchronization
				WaitSynchronization();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // GetDebugEvent
				GetDebugEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetProcessModuleInfo
				GetProcessModuleInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetProcessList
				GetProcessList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // GetThreadList
				GetThreadList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // GetDebugThreadContext
				GetDebugThreadContext();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // ContinueDebugEvent
				ContinueDebugEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // ReadDebugProcessMemory
				ReadDebugProcessMemory();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // WriteDebugProcessMemory
				WriteDebugProcessMemory();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // SetDebugThreadContext
				SetDebugThreadContext();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetDebugThreadParam
				GetDebugThreadParam();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // InitializeThreadInfo
				InitializeThreadInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // SetHardwareBreakPoint
				SetHardwareBreakPoint();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // QueryDebugProcessMemory
				QueryDebugProcessMemory();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // GetProcessMemoryDetails
				GetProcessMemoryDetails();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // AttachByProgramId
				AttachByProgramId();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // AttachOnLaunch
				AttachOnLaunch();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // GetDebugMonitorProcessId
				GetDebugMonitorProcessId();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // GetJitDebugProcessList
				GetJitDebugProcessList();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // CreateCoreDump
				CreateCoreDump();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // GetAllDebugThreadInfo
				GetAllDebugThreadInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1D: { // TargetIO_FileOpen
				TargetIO_FileOpen();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // TargetIO_FileClose
				TargetIO_FileClose();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // TargetIO_FileRead
				TargetIO_FileRead();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x20: { // TargetIO_FileWrite
				TargetIO_FileWrite();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // TargetIO_FileSetAttributes
				TargetIO_FileSetAttributes();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x22: { // TargetIO_FileGetInformation
				TargetIO_FileGetInformation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // TargetIO_FileSetTime
				TargetIO_FileSetTime();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x24: { // TargetIO_FileSetSize
				TargetIO_FileSetSize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // TargetIO_FileDelete
				TargetIO_FileDelete();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x26: { // TargetIO_FileMove
				TargetIO_FileMove();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x27: { // TargetIO_DirectoryCreate
				TargetIO_DirectoryCreate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // TargetIO_DirectoryDelete
				TargetIO_DirectoryDelete();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x29: { // TargetIO_DirectoryRename
				TargetIO_DirectoryRename();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2A: { // TargetIO_DirectoryGetCount
				TargetIO_DirectoryGetCount();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2B: { // TargetIO_DirectoryOpen
				TargetIO_DirectoryOpen();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2C: { // TargetIO_DirectoryGetNext
				TargetIO_DirectoryGetNext();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2D: { // TargetIO_DirectoryClose
				TargetIO_DirectoryClose();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2E: { // TargetIO_GetFreeSpace
				TargetIO_GetFreeSpace();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2F: { // TargetIO_GetVolumeInformation
				TargetIO_GetVolumeInformation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x30: { // InitiateCoreDump
				InitiateCoreDump();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x31: { // ContinueCoreDump
				ContinueCoreDump();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x32: { // AddTTYToCoreDump
				AddTTYToCoreDump();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x33: { // AddImageToCoreDump
				AddImageToCoreDump();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x34: { // CloseCoreDump
				CloseCoreDump();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Dmnt.IInterface");
		}
	}
}

