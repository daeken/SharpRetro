using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Dmnt;
public partial class IInterface : _IInterface_Base;
public abstract class _IInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BreakDebugProcess
				break;
			case 0x1: // TerminateDebugProcess
				break;
			case 0x2: // CloseHandle
				break;
			case 0x3: // LoadImage
				break;
			case 0x4: // GetProcessId
				break;
			case 0x5: // GetProcessHandle
				break;
			case 0x6: // WaitSynchronization
				break;
			case 0x7: // GetDebugEvent
				break;
			case 0x8: // GetProcessModuleInfo
				break;
			case 0x9: // GetProcessList
				break;
			case 0xA: // GetThreadList
				break;
			case 0xB: // GetDebugThreadContext
				break;
			case 0xC: // ContinueDebugEvent
				break;
			case 0xD: // ReadDebugProcessMemory
				break;
			case 0xE: // WriteDebugProcessMemory
				break;
			case 0xF: // SetDebugThreadContext
				break;
			case 0x10: // GetDebugThreadParam
				break;
			case 0x11: // InitializeThreadInfo
				break;
			case 0x12: // SetHardwareBreakPoint
				break;
			case 0x13: // QueryDebugProcessMemory
				break;
			case 0x14: // GetProcessMemoryDetails
				break;
			case 0x15: // AttachByProgramId
				break;
			case 0x16: // AttachOnLaunch
				break;
			case 0x17: // GetDebugMonitorProcessId
				break;
			case 0x19: // GetJitDebugProcessList
				break;
			case 0x1A: // CreateCoreDump
				break;
			case 0x1B: // GetAllDebugThreadInfo
				break;
			case 0x1D: // TargetIO_FileOpen
				break;
			case 0x1E: // TargetIO_FileClose
				break;
			case 0x1F: // TargetIO_FileRead
				break;
			case 0x20: // TargetIO_FileWrite
				break;
			case 0x21: // TargetIO_FileSetAttributes
				break;
			case 0x22: // TargetIO_FileGetInformation
				break;
			case 0x23: // TargetIO_FileSetTime
				break;
			case 0x24: // TargetIO_FileSetSize
				break;
			case 0x25: // TargetIO_FileDelete
				break;
			case 0x26: // TargetIO_FileMove
				break;
			case 0x27: // TargetIO_DirectoryCreate
				break;
			case 0x28: // TargetIO_DirectoryDelete
				break;
			case 0x29: // TargetIO_DirectoryRename
				break;
			case 0x2A: // TargetIO_DirectoryGetCount
				break;
			case 0x2B: // TargetIO_DirectoryOpen
				break;
			case 0x2C: // TargetIO_DirectoryGetNext
				break;
			case 0x2D: // TargetIO_DirectoryClose
				break;
			case 0x2E: // TargetIO_GetFreeSpace
				break;
			case 0x2F: // TargetIO_GetVolumeInformation
				break;
			case 0x30: // InitiateCoreDump
				break;
			case 0x31: // ContinueCoreDump
				break;
			case 0x32: // AddTTYToCoreDump
				break;
			case 0x33: // AddImageToCoreDump
				break;
			case 0x34: // CloseCoreDump
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Dmnt.IInterface");
		}
	}
}

