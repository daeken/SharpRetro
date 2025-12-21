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
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.BreakDebugProcess");
	protected virtual void TerminateDebugProcess() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TerminateDebugProcess");
	protected virtual void CloseHandle() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.CloseHandle");
	protected virtual void LoadImage() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.LoadImage");
	protected virtual void GetProcessId() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetProcessId");
	protected virtual void GetProcessHandle() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetProcessHandle");
	protected virtual void WaitSynchronization() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.WaitSynchronization");
	protected virtual void GetDebugEvent() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetDebugEvent");
	protected virtual void GetProcessModuleInfo() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetProcessModuleInfo");
	protected virtual void GetProcessList() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetProcessList");
	protected virtual void GetThreadList() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetThreadList");
	protected virtual void GetDebugThreadContext() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetDebugThreadContext");
	protected virtual void ContinueDebugEvent() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.ContinueDebugEvent");
	protected virtual void ReadDebugProcessMemory() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.ReadDebugProcessMemory");
	protected virtual void WriteDebugProcessMemory() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.WriteDebugProcessMemory");
	protected virtual void SetDebugThreadContext() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.SetDebugThreadContext");
	protected virtual void GetDebugThreadParam() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetDebugThreadParam");
	protected virtual void InitializeThreadInfo() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.InitializeThreadInfo");
	protected virtual void SetHardwareBreakPoint() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.SetHardwareBreakPoint");
	protected virtual void QueryDebugProcessMemory() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.QueryDebugProcessMemory");
	protected virtual void GetProcessMemoryDetails() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetProcessMemoryDetails");
	protected virtual void AttachByProgramId() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.AttachByProgramId");
	protected virtual void AttachOnLaunch() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.AttachOnLaunch");
	protected virtual void GetDebugMonitorProcessId() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetDebugMonitorProcessId");
	protected virtual void GetJitDebugProcessList() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetJitDebugProcessList");
	protected virtual void CreateCoreDump() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.CreateCoreDump");
	protected virtual void GetAllDebugThreadInfo() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.GetAllDebugThreadInfo");
	protected virtual void TargetIO_FileOpen() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileOpen");
	protected virtual void TargetIO_FileClose() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileClose");
	protected virtual void TargetIO_FileRead() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileRead");
	protected virtual void TargetIO_FileWrite() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileWrite");
	protected virtual void TargetIO_FileSetAttributes() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileSetAttributes");
	protected virtual void TargetIO_FileGetInformation() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileGetInformation");
	protected virtual void TargetIO_FileSetTime() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileSetTime");
	protected virtual void TargetIO_FileSetSize() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileSetSize");
	protected virtual void TargetIO_FileDelete() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileDelete");
	protected virtual void TargetIO_FileMove() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_FileMove");
	protected virtual void TargetIO_DirectoryCreate() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryCreate");
	protected virtual void TargetIO_DirectoryDelete() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryDelete");
	protected virtual void TargetIO_DirectoryRename() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryRename");
	protected virtual void TargetIO_DirectoryGetCount() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryGetCount");
	protected virtual void TargetIO_DirectoryOpen() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryOpen");
	protected virtual void TargetIO_DirectoryGetNext() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryGetNext");
	protected virtual void TargetIO_DirectoryClose() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_DirectoryClose");
	protected virtual void TargetIO_GetFreeSpace() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_GetFreeSpace");
	protected virtual void TargetIO_GetVolumeInformation() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.TargetIO_GetVolumeInformation");
	protected virtual void InitiateCoreDump() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.InitiateCoreDump");
	protected virtual void ContinueCoreDump() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.ContinueCoreDump");
	protected virtual void AddTTYToCoreDump() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.AddTTYToCoreDump");
	protected virtual void AddImageToCoreDump() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.AddImageToCoreDump");
	protected virtual void CloseCoreDump() =>
		Console.WriteLine("Stub hit for Nn.Dmnt.IInterface.CloseCoreDump");
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

