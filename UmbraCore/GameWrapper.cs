using System.Runtime.InteropServices;
using Aarch64Cpu;
using NxCommon;

namespace UmbraCore;

public delegate void SetupDelegate(ref CallbackTableOffsets callbacks);
public unsafe delegate void RunFromDelegate(CpuState* state, ulong addr, ulong until);

public unsafe class GameWrapper {
	readonly SetupDelegate _Setup;
	readonly RunFromDelegate _RunFrom;
	
	public Callbacks Callbacks = new();
	
	public GameWrapper(string libPath) {
		var lib = NativeLibrary.Load(libPath);
		_Setup = Marshal.GetDelegateForFunctionPointer<SetupDelegate>(NativeLibrary.GetExport(lib, "setup"));
		_RunFrom = Marshal.GetDelegateForFunctionPointer<RunFromDelegate>(NativeLibrary.GetExport(lib, "runFrom"));

        Callbacks.NativeReentry = (state, op, a, b) => {
            switch(op) {
				case 0:
					throw new NotImplementedException($"Brk? 0x{a:X}");
				case 1:
                    switch(a) {
                        case 0x01:
                            state->X0 = Callbacks.SetHeapSize(state->X1, ref state->X1);
                            break;
                        case 0x02:
                            state->X0 = Callbacks.SetMemoryPermission(state->X0, state->X1, state->X2);
                            break;
                        case 0x03:
                            state->X0 = Callbacks.SetMemoryAttribute(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x04:
                            state->X0 = Callbacks.MapMemory(state->X0, state->X1, state->X2);
                            break;
                        case 0x05:
                            state->X0 = Callbacks.UnmapMemory(state->X0, state->X1, state->X2);
                            break;
                        case 0x06:
                            state->X0 = Callbacks.QueryMemory(state->X0, state->X2, ref state->X1);
                            break;
                        case 0x07:
                            Callbacks.ExitProcess();
                            break;
                        case 0x08:
                            state->X0 = Callbacks.CreateThread(state->X1, state->X2, state->X3, state->X4, state->X5,
                                ref state->X1);
                            break;
                        case 0x09:
                            state->X0 = Callbacks.StartThread(state->X0, state->X2, state->X3);
                            break;
                        case 0x0A:
                            Callbacks.ExitThread();
                            break;
                        case 0x0B:
                            Callbacks.SleepThread(state->X0);
                            break;
                        case 0x0C:
                            state->X0 = Callbacks.GetThreadPriority(state->X1, ref state->X1);
                            break;
                        case 0x0D:
                            state->X0 = Callbacks.SetThreadPriority(state->X0, state->X1);
                            break;
                        case 0x0E:
                            state->X0 = Callbacks.GetThreadCoreMask(state->X2, ref state->X1, ref state->X2);
                            break;
                        case 0x0F:
                            state->X0 = Callbacks.SetThreadCoreMask(state->X0, state->X1, state->X2);
                            break;
                        case 0x10:
                            state->X0 = Callbacks.GetCurrentProcessorNumber();
                            break;
                        case 0x11:
                            state->X0 = Callbacks.SignalEvent(state->X0);
                            break;
                        case 0x12:
                            state->X0 = Callbacks.ClearEvent(state->X0);
                            break;
                        case 0x13:
                            state->X0 = Callbacks.MapSharedMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x14:
                            state->X0 = Callbacks.UnmapSharedMemory(state->X0, state->X1, state->X2);
                            break;
                        case 0x15:
                            state->X0 = Callbacks.CreateTransferMemory(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x16:
                            state->X0 = Callbacks.CloseHandle(state->X0);
                            break;
                        case 0x17:
                            state->X0 = Callbacks.ResetSignal(state->X0);
                            break;
                        case 0x18:
                            state->X0 = Callbacks.WaitSynchronization(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x19:
                            state->X0 = Callbacks.CancelSynchronization(state->X0);
                            break;
                        case 0x1A:
                            state->X0 = Callbacks.ArbitrateLock(state->X0, state->X1, state->X2);
                            break;
                        case 0x1B:
                            state->X0 = Callbacks.ArbitrateUnlock(state->X0);
                            break;
                        case 0x1C:
                            state->X0 = Callbacks.WaitProcessWideKeyAtomic(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x1D:
                            state->X0 = Callbacks.SignalProcessWideKey(state->X0, state->X1);
                            break;
                        case 0x1E:
                            state->X0 = Callbacks.GetSystemTick();
                            break;
                        case 0x1F:
                            state->X0 = Callbacks.ConnectToNamedPort(state->X1, ref state->X1);
                            break;
                        case 0x20:
                            state->X0 = Callbacks.SendSyncRequestLight(state->X0);
                            break;
                        case 0x21:
                            state->X0 = Callbacks.SendSyncRequest(state->X0);
                            break;
                        case 0x22:
                            state->X0 = Callbacks.SendSyncRequestWithUserBuffer(state->X0, state->X1, state->X2);
                            break;
                        case 0x23:
                            state->X0 = Callbacks.SendAsyncRequestWithUserBuffer(state->X1, state->X2, state->X3,
                                ref state->X1);
                            break;
                        case 0x24:
                            state->X0 = Callbacks.GetProcessId(state->X1, ref state->X1);
                            break;
                        case 0x25:
                            state->X0 = Callbacks.GetThreadId(state->X1, ref state->X1);
                            break;
                        case 0x26:
                            state->X0 = Callbacks.Break(state->X0, state->X1, state->X2);
                            break;
                        case 0x27:
                            state->X0 = Callbacks.OutputDebugString(state->X0, state->X1);
                            break;
                        case 0x28:
                            Callbacks.ReturnFromException(state->X0);
                            break;
                        case 0x29:
                            state->X0 = Callbacks.GetInfo(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x2A:
                            Callbacks.FlushEntireDataCache();
                            break;
                        case 0x2B:
                            state->X0 = Callbacks.FlushDataCache(state->X0, state->X1);
                            break;
                        case 0x2C:
                            state->X0 = Callbacks.MapPhysicalMemory(state->X0, state->X1);
                            break;
                        case 0x2D:
                            state->X0 = Callbacks.UnmapPhysicalMemory(state->X0, state->X1);
                            break;
                        case 0x2E:
                            state->X0 = Callbacks.GetDebugFutureThreadInfo(state->X3, ref state->X1, ref state->X2,
                                ref state->X3, ref state->X4, ref state->X5, ref state->X6);
                            break;
                        case 0x2F:
                            state->X0 = Callbacks.GetLastThreadInfo(ref state->X1, ref state->X2, ref state->X3,
                                ref state->X4, ref state->X5, ref state->X6);
                            break;
                        case 0x30:
                            state->X0 = Callbacks.GetResourceLimitLimitValue(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x31:
                            state->X0 = Callbacks.GetResourceLimitCurrentValue(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x32:
                            state->X0 = Callbacks.SetThreadActivity(state->X0, state->X1);
                            break;
                        case 0x33:
                            state->X0 = Callbacks.GetThreadContext3(state->X0, state->X1);
                            break;
                        case 0x34:
                            state->X0 = Callbacks.WaitForAddress(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x35:
                            state->X0 = Callbacks.SignalToAddress(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x36:
                            Callbacks.SynchronizePreemptionState();
                            break;
                        case 0x37:
                            state->X0 = Callbacks.GetResourceLimitPeakValue(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x3C:
                            state->X0 = Callbacks.KernelDebug(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x3D:
                            state->X0 = Callbacks.ChangeKernelTraceState(state->X0);
                            break;
                        case 0x40:
                            state->X0 = Callbacks.CreateSession(state->X2, state->X3, ref state->X1, ref state->X2);
                            break;
                        case 0x41:
                            state->X0 = Callbacks.AcceptSession(state->X1, ref state->X1);
                            break;
                        case 0x42:
                            state->X0 = Callbacks.ReplyAndReceiveLight(state->X0);
                            break;
                        case 0x43:
                            state->X0 = Callbacks.ReplyAndReceive(state->X1, state->X2, state->X3, state->X4,
                                ref state->X1);
                            break;
                        case 0x44:
                            state->X0 = Callbacks.ReplyAndReceiveWithUserBuffer(state->X1, state->X2, state->X3,
                                state->X4, state->X5, state->X6, ref state->X1);
                            break;
                        case 0x45:
                            state->X0 = Callbacks.CreateEvent(ref state->X1, ref state->X2);
                            break;
                        case 0x48:
                            state->X0 = Callbacks.MapPhysicalMemoryUnsafe(state->X0, state->X1);
                            break;
                        case 0x49:
                            state->X0 = Callbacks.UnmapPhysicalMemoryUnsafe(state->X0, state->X1);
                            break;
                        case 0x4A:
                            state->X0 = Callbacks.SetUnsafeLimit(state->X0);
                            break;
                        case 0x4B:
                            state->X0 = Callbacks.CreateCodeMemory(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x4C:
                            state->X0 = Callbacks.ControlCodeMemory(state->X0, state->X1, state->X2, state->X3,
                                state->X4);
                            break;
                        case 0x4D:
                            Callbacks.SleepSystem();
                            break;
                        case 0x4E:
                            state->X0 = Callbacks.ReadWriteRegister(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x4F:
                            state->X0 = Callbacks.SetProcessActivity(state->X0, state->X1);
                            break;
                        case 0x50:
                            state->X0 = Callbacks.CreateSharedMemory(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x51:
                            state->X0 = Callbacks.MapTransferMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x52:
                            state->X0 = Callbacks.UnmapTransferMemory(state->X0, state->X1, state->X2);
                            break;
                        case 0x53:
                            state->X0 = Callbacks.CreateInterruptEvent(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x54:
                            state->X0 = Callbacks.QueryPhysicalAddress(state->X1, ref state->X1, ref state->X2,
                                ref state->X3);
                            break;
                        case 0x56:
                            state->X0 = Callbacks.CreateDeviceAddressSpace(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x57:
                            state->X0 = Callbacks.AttachDeviceAddressSpace(state->X0, state->X1);
                            break;
                        case 0x58:
                            state->X0 = Callbacks.DetachDeviceAddressSpace(state->X0, state->X1);
                            break;
                        case 0x59:
                            state->X0 = Callbacks.MapDeviceAddressSpaceByForce(state->X0, state->X1, state->X2,
                                state->X3, state->X4, state->X5);
                            break;
                        case 0x5A:
                            state->X0 = Callbacks.MapDeviceAddressSpaceAligned(state->X0, state->X1, state->X2,
                                state->X3, state->X4, state->X5);
                            break;
                        case 0x5B:
                            state->X0 = Callbacks.MapDeviceAddressSpace(state->X1, state->X2, state->X3, state->X4,
                                state->X5, state->X6, ref state->X1);
                            break;
                        case 0x5C:
                            state->X0 = Callbacks.UnmapDeviceAddressSpace(state->X0, state->X1, state->X2, state->X3,
                                state->X4);
                            break;
                        case 0x5D:
                            state->X0 = Callbacks.InvalidateProcessDataCache(state->X0, state->X1, state->X2);
                            break;
                        case 0x5E:
                            state->X0 = Callbacks.StoreProcessDataCache(state->X0, state->X1, state->X2);
                            break;
                        case 0x5F:
                            state->X0 = Callbacks.FlushProcessDataCache(state->X0, state->X1, state->X2);
                            break;
                        case 0x60:
                            state->X0 = Callbacks.DebugActiveProcess(state->X1, ref state->X1);
                            break;
                        case 0x61:
                            state->X0 = Callbacks.BreakDebugProcess(state->X0);
                            break;
                        case 0x62:
                            state->X0 = Callbacks.TerminateDebugProcess(state->X0);
                            break;
                        case 0x63:
                            state->X0 = Callbacks.GetDebugEvent(state->X0, state->X1);
                            break;
                        case 0x64:
                            state->X0 = Callbacks.ContinueDebugEvent(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x65:
                            state->X0 = Callbacks.GetProcessList(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x66:
                            state->X0 = Callbacks.GetThreadList(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x67:
                            state->X0 = Callbacks.GetDebugThreadContext(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x68:
                            state->X0 = Callbacks.SetDebugThreadContext(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x69:
                            state->X0 = Callbacks.QueryDebugProcessMemory(state->X0, state->X2, state->X3,
                                ref state->X1);
                            break;
                        case 0x6A:
                            state->X0 = Callbacks.ReadDebugProcessMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x6B:
                            state->X0 = Callbacks.WriteDebugProcessMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x6C:
                            state->X0 = Callbacks.SetHardwareBreakPoint(state->X0, state->X1, state->X2);
                            break;
                        case 0x6D:
                            state->X0 = Callbacks.GetDebugThreadParam(state->X2, state->X3, state->X4, ref state->X1,
                                ref state->X2);
                            break;
                        case 0x6F:
                            state->X0 = Callbacks.GetSystemInfo(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x70:
                            state->X0 = Callbacks.CreatePort(state->X2, state->X3, state->X4, ref state->X1,
                                ref state->X2);
                            break;
                        case 0x71:
                            state->X0 = Callbacks.ManageNamedPort(state->X1, state->X2, ref state->X1);
                            break;
                        case 0x72:
                            state->X0 = Callbacks.ConnectToPort(state->X1, ref state->X1);
                            break;
                        case 0x73:
                            state->X0 = Callbacks.SetProcessMemoryPermission(state->X0, state->X1, state->X2,
                                state->X3);
                            break;
                        case 0x74:
                            state->X0 = Callbacks.MapProcessMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x75:
                            state->X0 = Callbacks.UnmapProcessMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x76:
                            state->X0 = Callbacks.QueryProcessMemory(state->X0, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x77:
                            state->X0 = Callbacks.MapProcessCodeMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x78:
                            state->X0 = Callbacks.UnmapProcessCodeMemory(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x79:
                            state->X0 = Callbacks.CreateProcess(state->X1, state->X2, state->X3, ref state->X1);
                            break;
                        case 0x7A:
                            state->X0 = Callbacks.StartProcess(state->X0, state->X1, state->X2, state->X3);
                            break;
                        case 0x7B:
                            state->X0 = Callbacks.TerminateProcess(state->X0);
                            break;
                        case 0x7C:
                            state->X0 = Callbacks.GetProcessInfo(state->X0, state->X1, ref state->X1);
                            break;
                        case 0x7D:
                            state->X0 = Callbacks.CreateResourceLimit(ref state->X1);
                            break;
                        case 0x7E:
                            state->X0 = Callbacks.SetResourceLimitLimitValue(state->X0, state->X1, state->X2);
                            break;
                        case 0x7F:
                            state->X0 = Callbacks.CallSecureMonitor(state->X0, state->X1, state->X2, state->X3,
                                state->X4, state->X5, state->X6, state->X7, ref state->X1, ref state->X2, ref state->X3,
                                ref state->X4, ref state->X5, ref state->X6, ref state->X7);
                            break;
                        default:
                            throw new NotImplementedException($"Unsupported SVC: 0x{a:X02}");
                    }
                    break;
                case 2: {
                    var ip = (uint) state->X2;
                    var op2 = ip & 0b111U;
                    var crm = (ip >> 3) & 0b1111U;
                    var crn = (ip >> 7) & 0b1111U;
                    var op1 = (ip >> 11) & 0b111U;
                    var op0 = (ip >> 14) & 0b1U;
                    Callbacks.WriteSr(op0, op1, crn, crm, op2, *GetRegisterPointer(state, (int) state->X3));
                    break;
                }
                case 3: {
                    var ip = (uint) state->X2;
                    var op2 = ip & 0b111U;
                    var crm = (ip >> 3) & 0b1111U;
                    var crn = (ip >> 7) & 0b1111U;
                    var op1 = (ip >> 11) & 0b111U;
                    var op0 = (ip >> 14) & 0b1U;
                    *GetRegisterPointer(state, (int) state->X3) = Callbacks.ReadSr(op0, op1, crn, crm, op2);
                    break;
                }
                default:
					throw new NotImplementedException($"Unsupported op to nativeReentry: {op}");
			}
		};
	}
    
    static ulong* GetRegisterPointer(NativeState* ptr, int num) => num switch {
        0 => &ptr->X0, 1 => &ptr->X1, 2 => &ptr->X2, 3 => &ptr->X3,
        4 => &ptr->X4, 5 => &ptr->X5, 6 => &ptr->X6, 7 => &ptr->X7,
        8 => &ptr->X8, 9 => &ptr->X9, 10 => &ptr->X10, 11 => &ptr->X11,
        12 => &ptr->X12, 13 => &ptr->X13, 14 => &ptr->X14, 15 => &ptr->X15,
        16 => &ptr->X16, 17 => &ptr->X17, 18 => &ptr->X18, 19 => &ptr->X19,
        20 => &ptr->X20, 21 => &ptr->X21, 22 => &ptr->X22, 23 => &ptr->X23,
        24 => &ptr->X24, 25 => &ptr->X25, 26 => &ptr->X26, 27 => &ptr->X27,
        28 => &ptr->X28, 29 => &ptr->X29, 30 => &ptr->X30,
        _ => throw new NotSupportedException($"Register {num} unsupported"),
    };

	public void Setup() => _Setup(ref Callbacks.CallbackTable);
	public void RunFrom(CpuState* state, ulong addr, ulong until) => _RunFrom(state, addr, until);
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct NativeState {
	public ulong
		X29, X30, X27, X28, X25, X26, X23, X24,
		X21, X22, X19, X20, X17, X18, X15, X16,
		X13, X14, X11, X12, X9 , X10, X7 , X8 ,
		X5, X6, X3, X4, X1, X2, X0, _;
}

public delegate void StubDelegate();
public delegate void DebugDelegate(ulong pc);
public unsafe delegate void LoadModuleDelegate(ulong loadBase, byte* data, ulong size, ulong textStart, ulong textEnd, ulong roStart, ulong roEnd, ulong dataStart, ulong dataEnd);
public delegate void InitModuleDelegate(ulong loadBase, ulong size);
public unsafe delegate void NativeReentryDelegate(NativeState* state, uint op, ulong a, ulong b);
public delegate ulong ReadSrDelegate(uint op0, uint op1, uint crn, uint crm, uint op2);
public delegate void WriteSrDelegate(uint op0, uint op1, uint crn, uint crm, uint op2, ulong value);
public delegate ulong SetHeapSizeDelegate(ulong in_1, ref ulong out_1);
public delegate ulong SetMemoryPermissionDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong SetMemoryAttributeDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong MapMemoryDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong UnmapMemoryDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong QueryMemoryDelegate(ulong in_0, ulong in_2, ref ulong out_1);
public delegate void ExitProcessDelegate();
public delegate ulong CreateThreadDelegate(ulong in_1, ulong in_2, ulong in_3, ulong in_4, ulong in_5, ref ulong out_1);
public delegate ulong StartThreadDelegate(ulong in_0, ulong in_2, ulong in_3);
public delegate void ExitThreadDelegate();
public delegate void SleepThreadDelegate(ulong in_0);
public delegate ulong GetThreadPriorityDelegate(ulong in_1, ref ulong out_1);
public delegate ulong SetThreadPriorityDelegate(ulong in_0, ulong in_1);
public delegate ulong GetThreadCoreMaskDelegate(ulong in_2, ref ulong out_1, ref ulong out_2);
public delegate ulong SetThreadCoreMaskDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong GetCurrentProcessorNumberDelegate();
public delegate ulong SignalEventDelegate(ulong in_0);
public delegate ulong ClearEventDelegate(ulong in_0);
public delegate ulong MapSharedMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong UnmapSharedMemoryDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong CreateTransferMemoryDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong CloseHandleDelegate(ulong in_0);
public delegate ulong ResetSignalDelegate(ulong in_0);
public delegate ulong WaitSynchronizationDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong CancelSynchronizationDelegate(ulong in_0);
public delegate ulong ArbitrateLockDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong ArbitrateUnlockDelegate(ulong in_0);
public delegate ulong WaitProcessWideKeyAtomicDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong SignalProcessWideKeyDelegate(ulong in_0, ulong in_1);
public delegate ulong GetSystemTickDelegate();
public delegate ulong ConnectToNamedPortDelegate(ulong in_1, ref ulong out_1);
public delegate ulong SendSyncRequestLightDelegate(ulong in_0);
public delegate ulong SendSyncRequestDelegate(ulong in_0);
public delegate ulong SendSyncRequestWithUserBufferDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong SendAsyncRequestWithUserBufferDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong GetProcessIdDelegate(ulong in_1, ref ulong out_1);
public delegate ulong GetThreadIdDelegate(ulong in_1, ref ulong out_1);
public delegate ulong BreakDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong OutputDebugStringDelegate(ulong in_0, ulong in_1);
public delegate void ReturnFromExceptionDelegate(ulong in_0);
public delegate ulong GetInfoDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate void FlushEntireDataCacheDelegate();
public delegate ulong FlushDataCacheDelegate(ulong in_0, ulong in_1);
public delegate ulong MapPhysicalMemoryDelegate(ulong in_0, ulong in_1);
public delegate ulong UnmapPhysicalMemoryDelegate(ulong in_0, ulong in_1);
public delegate ulong GetDebugFutureThreadInfoDelegate(ulong in_3, ref ulong out_1, ref ulong out_2, ref ulong out_3, ref ulong out_4, ref ulong out_5, ref ulong out_6);
public delegate ulong GetLastThreadInfoDelegate(ref ulong out_1, ref ulong out_2, ref ulong out_3, ref ulong out_4, ref ulong out_5, ref ulong out_6);
public delegate ulong GetResourceLimitLimitValueDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong GetResourceLimitCurrentValueDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong SetThreadActivityDelegate(ulong in_0, ulong in_1);
public delegate ulong GetThreadContext3Delegate(ulong in_0, ulong in_1);
public delegate ulong WaitForAddressDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong SignalToAddressDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate void SynchronizePreemptionStateDelegate();
public delegate ulong GetResourceLimitPeakValueDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate void CreateIoPoolDelegate();
public delegate void CreateIoRegionDelegate();
public delegate ulong KernelDebugDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong ChangeKernelTraceStateDelegate(ulong in_0);
public delegate ulong CreateSessionDelegate(ulong in_2, ulong in_3, ref ulong out_1, ref ulong out_2);
public delegate ulong AcceptSessionDelegate(ulong in_1, ref ulong out_1);
public delegate ulong ReplyAndReceiveLightDelegate(ulong in_0);
public delegate ulong ReplyAndReceiveDelegate(ulong in_1, ulong in_2, ulong in_3, ulong in_4, ref ulong out_1);
public delegate ulong ReplyAndReceiveWithUserBufferDelegate(ulong in_1, ulong in_2, ulong in_3, ulong in_4, ulong in_5, ulong in_6, ref ulong out_1);
public delegate ulong CreateEventDelegate(ref ulong out_1, ref ulong out_2);
public delegate void MapIoRegionDelegate();
public delegate void UnmapIoRegionDelegate();
public delegate ulong MapPhysicalMemoryUnsafeDelegate(ulong in_0, ulong in_1);
public delegate ulong UnmapPhysicalMemoryUnsafeDelegate(ulong in_0, ulong in_1);
public delegate ulong SetUnsafeLimitDelegate(ulong in_0);
public delegate ulong CreateCodeMemoryDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong ControlCodeMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3, ulong in_4);
public delegate void SleepSystemDelegate();
public delegate ulong ReadWriteRegisterDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong SetProcessActivityDelegate(ulong in_0, ulong in_1);
public delegate ulong CreateSharedMemoryDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong MapTransferMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong UnmapTransferMemoryDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong CreateInterruptEventDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong QueryPhysicalAddressDelegate(ulong in_1, ref ulong out_1, ref ulong out_2, ref ulong out_3);
public delegate void QueryMemoryMappingDelegate();
public delegate ulong CreateDeviceAddressSpaceDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong AttachDeviceAddressSpaceDelegate(ulong in_0, ulong in_1);
public delegate ulong DetachDeviceAddressSpaceDelegate(ulong in_0, ulong in_1);
public delegate ulong MapDeviceAddressSpaceByForceDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3, ulong in_4, ulong in_5);
public delegate ulong MapDeviceAddressSpaceAlignedDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3, ulong in_4, ulong in_5);
public delegate ulong MapDeviceAddressSpaceDelegate(ulong in_1, ulong in_2, ulong in_3, ulong in_4, ulong in_5, ulong in_6, ref ulong out_1);
public delegate ulong UnmapDeviceAddressSpaceDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3, ulong in_4);
public delegate ulong InvalidateProcessDataCacheDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong StoreProcessDataCacheDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong FlushProcessDataCacheDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong DebugActiveProcessDelegate(ulong in_1, ref ulong out_1);
public delegate ulong BreakDebugProcessDelegate(ulong in_0);
public delegate ulong TerminateDebugProcessDelegate(ulong in_0);
public delegate ulong GetDebugEventDelegate(ulong in_0, ulong in_1);
public delegate ulong ContinueDebugEventDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong GetProcessListDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong GetThreadListDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong GetDebugThreadContextDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong SetDebugThreadContextDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong QueryDebugProcessMemoryDelegate(ulong in_0, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong ReadDebugProcessMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong WriteDebugProcessMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong SetHardwareBreakPointDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong GetDebugThreadParamDelegate(ulong in_2, ulong in_3, ulong in_4, ref ulong out_1, ref ulong out_2);
public delegate ulong GetSystemInfoDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong CreatePortDelegate(ulong in_2, ulong in_3, ulong in_4, ref ulong out_1, ref ulong out_2);
public delegate ulong ManageNamedPortDelegate(ulong in_1, ulong in_2, ref ulong out_1);
public delegate ulong ConnectToPortDelegate(ulong in_1, ref ulong out_1);
public delegate ulong SetProcessMemoryPermissionDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong MapProcessMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong UnmapProcessMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong QueryProcessMemoryDelegate(ulong in_0, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong MapProcessCodeMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong UnmapProcessCodeMemoryDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong CreateProcessDelegate(ulong in_1, ulong in_2, ulong in_3, ref ulong out_1);
public delegate ulong StartProcessDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3);
public delegate ulong TerminateProcessDelegate(ulong in_0);
public delegate ulong GetProcessInfoDelegate(ulong in_0, ulong in_1, ref ulong out_1);
public delegate ulong CreateResourceLimitDelegate(ref ulong out_1);
public delegate ulong SetResourceLimitLimitValueDelegate(ulong in_0, ulong in_1, ulong in_2);
public delegate ulong CallSecureMonitorDelegate(ulong in_0, ulong in_1, ulong in_2, ulong in_3, ulong in_4, ulong in_5, ulong in_6, ulong in_7, ref ulong out_1, ref ulong out_2, ref ulong out_3, ref ulong out_4, ref ulong out_5, ref ulong out_6, ref ulong out_7);
public delegate void SetMemoryAttribute2Delegate();
public delegate void MapInsecurePhysicalMemoryDelegate();
public delegate void UnmapInsecurePhysicalMemoryDelegate();

public class Callbacks {
    public CallbackTableOffsets CallbackTable;
    public DebugDelegate Debug { get => debug; set => CallbackTable.debug = Marshal.GetFunctionPointerForDelegate(debug = value); } DebugDelegate debug;
    public LoadModuleDelegate LoadModule { get => loadModule; set => CallbackTable.loadModule = Marshal.GetFunctionPointerForDelegate(loadModule = value); } LoadModuleDelegate loadModule;
    public InitModuleDelegate InitModule { get => initModule; set => CallbackTable.initModule = Marshal.GetFunctionPointerForDelegate(initModule = value); } InitModuleDelegate initModule;
    public NativeReentryDelegate NativeReentry { get => nativeReentry; set => CallbackTable.nativeReentry = Marshal.GetFunctionPointerForDelegate(nativeReentry = value); } NativeReentryDelegate nativeReentry;
    public ReadSrDelegate ReadSr { get => readSr; set => CallbackTable.readSr = Marshal.GetFunctionPointerForDelegate(readSr = value); } ReadSrDelegate readSr;
    public WriteSrDelegate WriteSr { get => writeSr; set => CallbackTable.writeSr = Marshal.GetFunctionPointerForDelegate(writeSr = value); } WriteSrDelegate writeSr;
    public SetHeapSizeDelegate SetHeapSize { get => setHeapSize; set => CallbackTable.svcSetHeapSize = Marshal.GetFunctionPointerForDelegate(setHeapSize = value); } SetHeapSizeDelegate setHeapSize;
    public SetMemoryPermissionDelegate SetMemoryPermission { get => setMemoryPermission; set => CallbackTable.svcSetMemoryPermission = Marshal.GetFunctionPointerForDelegate(setMemoryPermission = value); } SetMemoryPermissionDelegate setMemoryPermission;
    public SetMemoryAttributeDelegate SetMemoryAttribute { get => setMemoryAttribute; set => CallbackTable.svcSetMemoryAttribute = Marshal.GetFunctionPointerForDelegate(setMemoryAttribute = value); } SetMemoryAttributeDelegate setMemoryAttribute;
    public MapMemoryDelegate MapMemory { get => mapMemory; set => CallbackTable.svcMapMemory = Marshal.GetFunctionPointerForDelegate(mapMemory = value); } MapMemoryDelegate mapMemory;
    public UnmapMemoryDelegate UnmapMemory { get => unmapMemory; set => CallbackTable.svcUnmapMemory = Marshal.GetFunctionPointerForDelegate(unmapMemory = value); } UnmapMemoryDelegate unmapMemory;
    public QueryMemoryDelegate QueryMemory { get => queryMemory; set => CallbackTable.svcQueryMemory = Marshal.GetFunctionPointerForDelegate(queryMemory = value); } QueryMemoryDelegate queryMemory;
    public ExitProcessDelegate ExitProcess { get => exitProcess; set => CallbackTable.svcExitProcess = Marshal.GetFunctionPointerForDelegate(exitProcess = value); } ExitProcessDelegate exitProcess;
    public CreateThreadDelegate CreateThread { get => createThread; set => CallbackTable.svcCreateThread = Marshal.GetFunctionPointerForDelegate(createThread = value); } CreateThreadDelegate createThread;
    public StartThreadDelegate StartThread { get => startThread; set => CallbackTable.svcStartThread = Marshal.GetFunctionPointerForDelegate(startThread = value); } StartThreadDelegate startThread;
    public ExitThreadDelegate ExitThread { get => exitThread; set => CallbackTable.svcExitThread = Marshal.GetFunctionPointerForDelegate(exitThread = value); } ExitThreadDelegate exitThread;
    public SleepThreadDelegate SleepThread { get => sleepThread; set => CallbackTable.svcSleepThread = Marshal.GetFunctionPointerForDelegate(sleepThread = value); } SleepThreadDelegate sleepThread;
    public GetThreadPriorityDelegate GetThreadPriority { get => getThreadPriority; set => CallbackTable.svcGetThreadPriority = Marshal.GetFunctionPointerForDelegate(getThreadPriority = value); } GetThreadPriorityDelegate getThreadPriority;
    public SetThreadPriorityDelegate SetThreadPriority { get => setThreadPriority; set => CallbackTable.svcSetThreadPriority = Marshal.GetFunctionPointerForDelegate(setThreadPriority = value); } SetThreadPriorityDelegate setThreadPriority;
    public GetThreadCoreMaskDelegate GetThreadCoreMask { get => getThreadCoreMask; set => CallbackTable.svcGetThreadCoreMask = Marshal.GetFunctionPointerForDelegate(getThreadCoreMask = value); } GetThreadCoreMaskDelegate getThreadCoreMask;
    public SetThreadCoreMaskDelegate SetThreadCoreMask { get => setThreadCoreMask; set => CallbackTable.svcSetThreadCoreMask = Marshal.GetFunctionPointerForDelegate(setThreadCoreMask = value); } SetThreadCoreMaskDelegate setThreadCoreMask;
    public GetCurrentProcessorNumberDelegate GetCurrentProcessorNumber { get => getCurrentProcessorNumber; set => CallbackTable.svcGetCurrentProcessorNumber = Marshal.GetFunctionPointerForDelegate(getCurrentProcessorNumber = value); } GetCurrentProcessorNumberDelegate getCurrentProcessorNumber;
    public SignalEventDelegate SignalEvent { get => signalEvent; set => CallbackTable.svcSignalEvent = Marshal.GetFunctionPointerForDelegate(signalEvent = value); } SignalEventDelegate signalEvent;
    public ClearEventDelegate ClearEvent { get => clearEvent; set => CallbackTable.svcClearEvent = Marshal.GetFunctionPointerForDelegate(clearEvent = value); } ClearEventDelegate clearEvent;
    public MapSharedMemoryDelegate MapSharedMemory { get => mapSharedMemory; set => CallbackTable.svcMapSharedMemory = Marshal.GetFunctionPointerForDelegate(mapSharedMemory = value); } MapSharedMemoryDelegate mapSharedMemory;
    public UnmapSharedMemoryDelegate UnmapSharedMemory { get => unmapSharedMemory; set => CallbackTable.svcUnmapSharedMemory = Marshal.GetFunctionPointerForDelegate(unmapSharedMemory = value); } UnmapSharedMemoryDelegate unmapSharedMemory;
    public CreateTransferMemoryDelegate CreateTransferMemory { get => createTransferMemory; set => CallbackTable.svcCreateTransferMemory = Marshal.GetFunctionPointerForDelegate(createTransferMemory = value); } CreateTransferMemoryDelegate createTransferMemory;
    public CloseHandleDelegate CloseHandle { get => closeHandle; set => CallbackTable.svcCloseHandle = Marshal.GetFunctionPointerForDelegate(closeHandle = value); } CloseHandleDelegate closeHandle;
    public ResetSignalDelegate ResetSignal { get => resetSignal; set => CallbackTable.svcResetSignal = Marshal.GetFunctionPointerForDelegate(resetSignal = value); } ResetSignalDelegate resetSignal;
    public WaitSynchronizationDelegate WaitSynchronization { get => waitSynchronization; set => CallbackTable.svcWaitSynchronization = Marshal.GetFunctionPointerForDelegate(waitSynchronization = value); } WaitSynchronizationDelegate waitSynchronization;
    public CancelSynchronizationDelegate CancelSynchronization { get => cancelSynchronization; set => CallbackTable.svcCancelSynchronization = Marshal.GetFunctionPointerForDelegate(cancelSynchronization = value); } CancelSynchronizationDelegate cancelSynchronization;
    public ArbitrateLockDelegate ArbitrateLock { get => arbitrateLock; set => CallbackTable.svcArbitrateLock = Marshal.GetFunctionPointerForDelegate(arbitrateLock = value); } ArbitrateLockDelegate arbitrateLock;
    public ArbitrateUnlockDelegate ArbitrateUnlock { get => arbitrateUnlock; set => CallbackTable.svcArbitrateUnlock = Marshal.GetFunctionPointerForDelegate(arbitrateUnlock = value); } ArbitrateUnlockDelegate arbitrateUnlock;
    public WaitProcessWideKeyAtomicDelegate WaitProcessWideKeyAtomic { get => waitProcessWideKeyAtomic; set => CallbackTable.svcWaitProcessWideKeyAtomic = Marshal.GetFunctionPointerForDelegate(waitProcessWideKeyAtomic = value); } WaitProcessWideKeyAtomicDelegate waitProcessWideKeyAtomic;
    public SignalProcessWideKeyDelegate SignalProcessWideKey { get => signalProcessWideKey; set => CallbackTable.svcSignalProcessWideKey = Marshal.GetFunctionPointerForDelegate(signalProcessWideKey = value); } SignalProcessWideKeyDelegate signalProcessWideKey;
    public GetSystemTickDelegate GetSystemTick { get => getSystemTick; set => CallbackTable.svcGetSystemTick = Marshal.GetFunctionPointerForDelegate(getSystemTick = value); } GetSystemTickDelegate getSystemTick;
    public ConnectToNamedPortDelegate ConnectToNamedPort { get => connectToNamedPort; set => CallbackTable.svcConnectToNamedPort = Marshal.GetFunctionPointerForDelegate(connectToNamedPort = value); } ConnectToNamedPortDelegate connectToNamedPort;
    public SendSyncRequestLightDelegate SendSyncRequestLight { get => sendSyncRequestLight; set => CallbackTable.svcSendSyncRequestLight = Marshal.GetFunctionPointerForDelegate(sendSyncRequestLight = value); } SendSyncRequestLightDelegate sendSyncRequestLight;
    public SendSyncRequestDelegate SendSyncRequest { get => sendSyncRequest; set => CallbackTable.svcSendSyncRequest = Marshal.GetFunctionPointerForDelegate(sendSyncRequest = value); } SendSyncRequestDelegate sendSyncRequest;
    public SendSyncRequestWithUserBufferDelegate SendSyncRequestWithUserBuffer { get => sendSyncRequestWithUserBuffer; set => CallbackTable.svcSendSyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate(sendSyncRequestWithUserBuffer = value); } SendSyncRequestWithUserBufferDelegate sendSyncRequestWithUserBuffer;
    public SendAsyncRequestWithUserBufferDelegate SendAsyncRequestWithUserBuffer { get => sendAsyncRequestWithUserBuffer; set => CallbackTable.svcSendAsyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate(sendAsyncRequestWithUserBuffer = value); } SendAsyncRequestWithUserBufferDelegate sendAsyncRequestWithUserBuffer;
    public GetProcessIdDelegate GetProcessId { get => getProcessId; set => CallbackTable.svcGetProcessId = Marshal.GetFunctionPointerForDelegate(getProcessId = value); } GetProcessIdDelegate getProcessId;
    public GetThreadIdDelegate GetThreadId { get => getThreadId; set => CallbackTable.svcGetThreadId = Marshal.GetFunctionPointerForDelegate(getThreadId = value); } GetThreadIdDelegate getThreadId;
    public BreakDelegate Break { get => sbreak; set => CallbackTable.svcBreak = Marshal.GetFunctionPointerForDelegate(sbreak = value); } BreakDelegate sbreak;
    public OutputDebugStringDelegate OutputDebugString { get => outputDebugString; set => CallbackTable.svcOutputDebugString = Marshal.GetFunctionPointerForDelegate(outputDebugString = value); } OutputDebugStringDelegate outputDebugString;
    public ReturnFromExceptionDelegate ReturnFromException { get => returnFromException; set => CallbackTable.svcReturnFromException = Marshal.GetFunctionPointerForDelegate(returnFromException = value); } ReturnFromExceptionDelegate returnFromException;
    public GetInfoDelegate GetInfo { get => getInfo; set => CallbackTable.svcGetInfo = Marshal.GetFunctionPointerForDelegate(getInfo = value); } GetInfoDelegate getInfo;
    public FlushEntireDataCacheDelegate FlushEntireDataCache { get => flushEntireDataCache; set => CallbackTable.svcFlushEntireDataCache = Marshal.GetFunctionPointerForDelegate(flushEntireDataCache = value); } FlushEntireDataCacheDelegate flushEntireDataCache;
    public FlushDataCacheDelegate FlushDataCache { get => flushDataCache; set => CallbackTable.svcFlushDataCache = Marshal.GetFunctionPointerForDelegate(flushDataCache = value); } FlushDataCacheDelegate flushDataCache;
    public MapPhysicalMemoryDelegate MapPhysicalMemory { get => mapPhysicalMemory; set => CallbackTable.svcMapPhysicalMemory = Marshal.GetFunctionPointerForDelegate(mapPhysicalMemory = value); } MapPhysicalMemoryDelegate mapPhysicalMemory;
    public UnmapPhysicalMemoryDelegate UnmapPhysicalMemory { get => unmapPhysicalMemory; set => CallbackTable.svcUnmapPhysicalMemory = Marshal.GetFunctionPointerForDelegate(unmapPhysicalMemory = value); } UnmapPhysicalMemoryDelegate unmapPhysicalMemory;
    public GetDebugFutureThreadInfoDelegate GetDebugFutureThreadInfo { get => getDebugFutureThreadInfo; set => CallbackTable.svcGetDebugFutureThreadInfo = Marshal.GetFunctionPointerForDelegate(getDebugFutureThreadInfo = value); } GetDebugFutureThreadInfoDelegate getDebugFutureThreadInfo;
    public GetLastThreadInfoDelegate GetLastThreadInfo { get => getLastThreadInfo; set => CallbackTable.svcGetLastThreadInfo = Marshal.GetFunctionPointerForDelegate(getLastThreadInfo = value); } GetLastThreadInfoDelegate getLastThreadInfo;
    public GetResourceLimitLimitValueDelegate GetResourceLimitLimitValue { get => getResourceLimitLimitValue; set => CallbackTable.svcGetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate(getResourceLimitLimitValue = value); } GetResourceLimitLimitValueDelegate getResourceLimitLimitValue;
    public GetResourceLimitCurrentValueDelegate GetResourceLimitCurrentValue { get => getResourceLimitCurrentValue; set => CallbackTable.svcGetResourceLimitCurrentValue = Marshal.GetFunctionPointerForDelegate(getResourceLimitCurrentValue = value); } GetResourceLimitCurrentValueDelegate getResourceLimitCurrentValue;
    public SetThreadActivityDelegate SetThreadActivity { get => setThreadActivity; set => CallbackTable.svcSetThreadActivity = Marshal.GetFunctionPointerForDelegate(setThreadActivity = value); } SetThreadActivityDelegate setThreadActivity;
    public GetThreadContext3Delegate GetThreadContext3 { get => getThreadContext3; set => CallbackTable.svcGetThreadContext3 = Marshal.GetFunctionPointerForDelegate(getThreadContext3 = value); } GetThreadContext3Delegate getThreadContext3;
    public WaitForAddressDelegate WaitForAddress { get => waitForAddress; set => CallbackTable.svcWaitForAddress = Marshal.GetFunctionPointerForDelegate(waitForAddress = value); } WaitForAddressDelegate waitForAddress;
    public SignalToAddressDelegate SignalToAddress { get => signalToAddress; set => CallbackTable.svcSignalToAddress = Marshal.GetFunctionPointerForDelegate(signalToAddress = value); } SignalToAddressDelegate signalToAddress;
    public SynchronizePreemptionStateDelegate SynchronizePreemptionState { get => synchronizePreemptionState; set => CallbackTable.svcSynchronizePreemptionState = Marshal.GetFunctionPointerForDelegate(synchronizePreemptionState = value); } SynchronizePreemptionStateDelegate synchronizePreemptionState;
    public GetResourceLimitPeakValueDelegate GetResourceLimitPeakValue { get => getResourceLimitPeakValue; set => CallbackTable.svcGetResourceLimitPeakValue = Marshal.GetFunctionPointerForDelegate(getResourceLimitPeakValue = value); } GetResourceLimitPeakValueDelegate getResourceLimitPeakValue;
    public CreateIoPoolDelegate CreateIoPool { get => createIoPool; set => CallbackTable.svcCreateIoPool = Marshal.GetFunctionPointerForDelegate(createIoPool = value); } CreateIoPoolDelegate createIoPool;
    public CreateIoRegionDelegate CreateIoRegion { get => createIoRegion; set => CallbackTable.svcCreateIoRegion = Marshal.GetFunctionPointerForDelegate(createIoRegion = value); } CreateIoRegionDelegate createIoRegion;
    public KernelDebugDelegate KernelDebug { get => kernelDebug; set => CallbackTable.svcKernelDebug = Marshal.GetFunctionPointerForDelegate(kernelDebug = value); } KernelDebugDelegate kernelDebug;
    public ChangeKernelTraceStateDelegate ChangeKernelTraceState { get => changeKernelTraceState; set => CallbackTable.svcChangeKernelTraceState = Marshal.GetFunctionPointerForDelegate(changeKernelTraceState = value); } ChangeKernelTraceStateDelegate changeKernelTraceState;
    public CreateSessionDelegate CreateSession { get => createSession; set => CallbackTable.svcCreateSession = Marshal.GetFunctionPointerForDelegate(createSession = value); } CreateSessionDelegate createSession;
    public AcceptSessionDelegate AcceptSession { get => acceptSession; set => CallbackTable.svcAcceptSession = Marshal.GetFunctionPointerForDelegate(acceptSession = value); } AcceptSessionDelegate acceptSession;
    public ReplyAndReceiveLightDelegate ReplyAndReceiveLight { get => replyAndReceiveLight; set => CallbackTable.svcReplyAndReceiveLight = Marshal.GetFunctionPointerForDelegate(replyAndReceiveLight = value); } ReplyAndReceiveLightDelegate replyAndReceiveLight;
    public ReplyAndReceiveDelegate ReplyAndReceive { get => replyAndReceive; set => CallbackTable.svcReplyAndReceive = Marshal.GetFunctionPointerForDelegate(replyAndReceive = value); } ReplyAndReceiveDelegate replyAndReceive;
    public ReplyAndReceiveWithUserBufferDelegate ReplyAndReceiveWithUserBuffer { get => replyAndReceiveWithUserBuffer; set => CallbackTable.svcReplyAndReceiveWithUserBuffer = Marshal.GetFunctionPointerForDelegate(replyAndReceiveWithUserBuffer = value); } ReplyAndReceiveWithUserBufferDelegate replyAndReceiveWithUserBuffer;
    public CreateEventDelegate CreateEvent { get => createEvent; set => CallbackTable.svcCreateEvent = Marshal.GetFunctionPointerForDelegate(createEvent = value); } CreateEventDelegate createEvent;
    public MapIoRegionDelegate MapIoRegion { get => mapIoRegion; set => CallbackTable.svcMapIoRegion = Marshal.GetFunctionPointerForDelegate(mapIoRegion = value); } MapIoRegionDelegate mapIoRegion;
    public UnmapIoRegionDelegate UnmapIoRegion { get => unmapIoRegion; set => CallbackTable.svcUnmapIoRegion = Marshal.GetFunctionPointerForDelegate(unmapIoRegion = value); } UnmapIoRegionDelegate unmapIoRegion;
    public MapPhysicalMemoryUnsafeDelegate MapPhysicalMemoryUnsafe { get => mapPhysicalMemoryUnsafe; set => CallbackTable.svcMapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate(mapPhysicalMemoryUnsafe = value); } MapPhysicalMemoryUnsafeDelegate mapPhysicalMemoryUnsafe;
    public UnmapPhysicalMemoryUnsafeDelegate UnmapPhysicalMemoryUnsafe { get => unmapPhysicalMemoryUnsafe; set => CallbackTable.svcUnmapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate(unmapPhysicalMemoryUnsafe = value); } UnmapPhysicalMemoryUnsafeDelegate unmapPhysicalMemoryUnsafe;
    public SetUnsafeLimitDelegate SetUnsafeLimit { get => setUnsafeLimit; set => CallbackTable.svcSetUnsafeLimit = Marshal.GetFunctionPointerForDelegate(setUnsafeLimit = value); } SetUnsafeLimitDelegate setUnsafeLimit;
    public CreateCodeMemoryDelegate CreateCodeMemory { get => createCodeMemory; set => CallbackTable.svcCreateCodeMemory = Marshal.GetFunctionPointerForDelegate(createCodeMemory = value); } CreateCodeMemoryDelegate createCodeMemory;
    public ControlCodeMemoryDelegate ControlCodeMemory { get => controlCodeMemory; set => CallbackTable.svcControlCodeMemory = Marshal.GetFunctionPointerForDelegate(controlCodeMemory = value); } ControlCodeMemoryDelegate controlCodeMemory;
    public SleepSystemDelegate SleepSystem { get => sleepSystem; set => CallbackTable.svcSleepSystem = Marshal.GetFunctionPointerForDelegate(sleepSystem = value); } SleepSystemDelegate sleepSystem;
    public ReadWriteRegisterDelegate ReadWriteRegister { get => readWriteRegister; set => CallbackTable.svcReadWriteRegister = Marshal.GetFunctionPointerForDelegate(readWriteRegister = value); } ReadWriteRegisterDelegate readWriteRegister;
    public SetProcessActivityDelegate SetProcessActivity { get => setProcessActivity; set => CallbackTable.svcSetProcessActivity = Marshal.GetFunctionPointerForDelegate(setProcessActivity = value); } SetProcessActivityDelegate setProcessActivity;
    public CreateSharedMemoryDelegate CreateSharedMemory { get => createSharedMemory; set => CallbackTable.svcCreateSharedMemory = Marshal.GetFunctionPointerForDelegate(createSharedMemory = value); } CreateSharedMemoryDelegate createSharedMemory;
    public MapTransferMemoryDelegate MapTransferMemory { get => mapTransferMemory; set => CallbackTable.svcMapTransferMemory = Marshal.GetFunctionPointerForDelegate(mapTransferMemory = value); } MapTransferMemoryDelegate mapTransferMemory;
    public UnmapTransferMemoryDelegate UnmapTransferMemory { get => unmapTransferMemory; set => CallbackTable.svcUnmapTransferMemory = Marshal.GetFunctionPointerForDelegate(unmapTransferMemory = value); } UnmapTransferMemoryDelegate unmapTransferMemory;
    public CreateInterruptEventDelegate CreateInterruptEvent { get => createInterruptEvent; set => CallbackTable.svcCreateInterruptEvent = Marshal.GetFunctionPointerForDelegate(createInterruptEvent = value); } CreateInterruptEventDelegate createInterruptEvent;
    public QueryPhysicalAddressDelegate QueryPhysicalAddress { get => queryPhysicalAddress; set => CallbackTable.svcQueryPhysicalAddress = Marshal.GetFunctionPointerForDelegate(queryPhysicalAddress = value); } QueryPhysicalAddressDelegate queryPhysicalAddress;
    public QueryMemoryMappingDelegate QueryMemoryMapping { get => queryMemoryMapping; set => CallbackTable.svcQueryMemoryMapping = Marshal.GetFunctionPointerForDelegate(queryMemoryMapping = value); } QueryMemoryMappingDelegate queryMemoryMapping;
    public CreateDeviceAddressSpaceDelegate CreateDeviceAddressSpace { get => createDeviceAddressSpace; set => CallbackTable.svcCreateDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(createDeviceAddressSpace = value); } CreateDeviceAddressSpaceDelegate createDeviceAddressSpace;
    public AttachDeviceAddressSpaceDelegate AttachDeviceAddressSpace { get => attachDeviceAddressSpace; set => CallbackTable.svcAttachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(attachDeviceAddressSpace = value); } AttachDeviceAddressSpaceDelegate attachDeviceAddressSpace;
    public DetachDeviceAddressSpaceDelegate DetachDeviceAddressSpace { get => detachDeviceAddressSpace; set => CallbackTable.svcDetachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(detachDeviceAddressSpace = value); } DetachDeviceAddressSpaceDelegate detachDeviceAddressSpace;
    public MapDeviceAddressSpaceByForceDelegate MapDeviceAddressSpaceByForce { get => mapDeviceAddressSpaceByForce; set => CallbackTable.svcMapDeviceAddressSpaceByForce = Marshal.GetFunctionPointerForDelegate(mapDeviceAddressSpaceByForce = value); } MapDeviceAddressSpaceByForceDelegate mapDeviceAddressSpaceByForce;
    public MapDeviceAddressSpaceAlignedDelegate MapDeviceAddressSpaceAligned { get => mapDeviceAddressSpaceAligned; set => CallbackTable.svcMapDeviceAddressSpaceAligned = Marshal.GetFunctionPointerForDelegate(mapDeviceAddressSpaceAligned = value); } MapDeviceAddressSpaceAlignedDelegate mapDeviceAddressSpaceAligned;
    public MapDeviceAddressSpaceDelegate MapDeviceAddressSpace { get => mapDeviceAddressSpace; set => CallbackTable.svcMapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(mapDeviceAddressSpace = value); } MapDeviceAddressSpaceDelegate mapDeviceAddressSpace;
    public UnmapDeviceAddressSpaceDelegate UnmapDeviceAddressSpace { get => unmapDeviceAddressSpace; set => CallbackTable.svcUnmapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(unmapDeviceAddressSpace = value); } UnmapDeviceAddressSpaceDelegate unmapDeviceAddressSpace;
    public InvalidateProcessDataCacheDelegate InvalidateProcessDataCache { get => invalidateProcessDataCache; set => CallbackTable.svcInvalidateProcessDataCache = Marshal.GetFunctionPointerForDelegate(invalidateProcessDataCache = value); } InvalidateProcessDataCacheDelegate invalidateProcessDataCache;
    public StoreProcessDataCacheDelegate StoreProcessDataCache { get => storeProcessDataCache; set => CallbackTable.svcStoreProcessDataCache = Marshal.GetFunctionPointerForDelegate(storeProcessDataCache = value); } StoreProcessDataCacheDelegate storeProcessDataCache;
    public FlushProcessDataCacheDelegate FlushProcessDataCache { get => flushProcessDataCache; set => CallbackTable.svcFlushProcessDataCache = Marshal.GetFunctionPointerForDelegate(flushProcessDataCache = value); } FlushProcessDataCacheDelegate flushProcessDataCache;
    public DebugActiveProcessDelegate DebugActiveProcess { get => debugActiveProcess; set => CallbackTable.svcDebugActiveProcess = Marshal.GetFunctionPointerForDelegate(debugActiveProcess = value); } DebugActiveProcessDelegate debugActiveProcess;
    public BreakDebugProcessDelegate BreakDebugProcess { get => breakDebugProcess; set => CallbackTable.svcBreakDebugProcess = Marshal.GetFunctionPointerForDelegate(breakDebugProcess = value); } BreakDebugProcessDelegate breakDebugProcess;
    public TerminateDebugProcessDelegate TerminateDebugProcess { get => terminateDebugProcess; set => CallbackTable.svcTerminateDebugProcess = Marshal.GetFunctionPointerForDelegate(terminateDebugProcess = value); } TerminateDebugProcessDelegate terminateDebugProcess;
    public GetDebugEventDelegate GetDebugEvent { get => getDebugEvent; set => CallbackTable.svcGetDebugEvent = Marshal.GetFunctionPointerForDelegate(getDebugEvent = value); } GetDebugEventDelegate getDebugEvent;
    public ContinueDebugEventDelegate ContinueDebugEvent { get => continueDebugEvent; set => CallbackTable.svcContinueDebugEvent = Marshal.GetFunctionPointerForDelegate(continueDebugEvent = value); } ContinueDebugEventDelegate continueDebugEvent;
    public GetProcessListDelegate GetProcessList { get => getProcessList; set => CallbackTable.svcGetProcessList = Marshal.GetFunctionPointerForDelegate(getProcessList = value); } GetProcessListDelegate getProcessList;
    public GetThreadListDelegate GetThreadList { get => getThreadList; set => CallbackTable.svcGetThreadList = Marshal.GetFunctionPointerForDelegate(getThreadList = value); } GetThreadListDelegate getThreadList;
    public GetDebugThreadContextDelegate GetDebugThreadContext { get => getDebugThreadContext; set => CallbackTable.svcGetDebugThreadContext = Marshal.GetFunctionPointerForDelegate(getDebugThreadContext = value); } GetDebugThreadContextDelegate getDebugThreadContext;
    public SetDebugThreadContextDelegate SetDebugThreadContext { get => setDebugThreadContext; set => CallbackTable.svcSetDebugThreadContext = Marshal.GetFunctionPointerForDelegate(setDebugThreadContext = value); } SetDebugThreadContextDelegate setDebugThreadContext;
    public QueryDebugProcessMemoryDelegate QueryDebugProcessMemory { get => queryDebugProcessMemory; set => CallbackTable.svcQueryDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(queryDebugProcessMemory = value); } QueryDebugProcessMemoryDelegate queryDebugProcessMemory;
    public ReadDebugProcessMemoryDelegate ReadDebugProcessMemory { get => readDebugProcessMemory; set => CallbackTable.svcReadDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(readDebugProcessMemory = value); } ReadDebugProcessMemoryDelegate readDebugProcessMemory;
    public WriteDebugProcessMemoryDelegate WriteDebugProcessMemory { get => writeDebugProcessMemory; set => CallbackTable.svcWriteDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(writeDebugProcessMemory = value); } WriteDebugProcessMemoryDelegate writeDebugProcessMemory;
    public SetHardwareBreakPointDelegate SetHardwareBreakPoint { get => setHardwareBreakPoint; set => CallbackTable.svcSetHardwareBreakPoint = Marshal.GetFunctionPointerForDelegate(setHardwareBreakPoint = value); } SetHardwareBreakPointDelegate setHardwareBreakPoint;
    public GetDebugThreadParamDelegate GetDebugThreadParam { get => getDebugThreadParam; set => CallbackTable.svcGetDebugThreadParam = Marshal.GetFunctionPointerForDelegate(getDebugThreadParam = value); } GetDebugThreadParamDelegate getDebugThreadParam;
    public GetSystemInfoDelegate GetSystemInfo { get => getSystemInfo; set => CallbackTable.svcGetSystemInfo = Marshal.GetFunctionPointerForDelegate(getSystemInfo = value); } GetSystemInfoDelegate getSystemInfo;
    public CreatePortDelegate CreatePort { get => createPort; set => CallbackTable.svcCreatePort = Marshal.GetFunctionPointerForDelegate(createPort = value); } CreatePortDelegate createPort;
    public ManageNamedPortDelegate ManageNamedPort { get => manageNamedPort; set => CallbackTable.svcManageNamedPort = Marshal.GetFunctionPointerForDelegate(manageNamedPort = value); } ManageNamedPortDelegate manageNamedPort;
    public ConnectToPortDelegate ConnectToPort { get => connectToPort; set => CallbackTable.svcConnectToPort = Marshal.GetFunctionPointerForDelegate(connectToPort = value); } ConnectToPortDelegate connectToPort;
    public SetProcessMemoryPermissionDelegate SetProcessMemoryPermission { get => setProcessMemoryPermission; set => CallbackTable.svcSetProcessMemoryPermission = Marshal.GetFunctionPointerForDelegate(setProcessMemoryPermission = value); } SetProcessMemoryPermissionDelegate setProcessMemoryPermission;
    public MapProcessMemoryDelegate MapProcessMemory { get => mapProcessMemory; set => CallbackTable.svcMapProcessMemory = Marshal.GetFunctionPointerForDelegate(mapProcessMemory = value); } MapProcessMemoryDelegate mapProcessMemory;
    public UnmapProcessMemoryDelegate UnmapProcessMemory { get => unmapProcessMemory; set => CallbackTable.svcUnmapProcessMemory = Marshal.GetFunctionPointerForDelegate(unmapProcessMemory = value); } UnmapProcessMemoryDelegate unmapProcessMemory;
    public QueryProcessMemoryDelegate QueryProcessMemory { get => queryProcessMemory; set => CallbackTable.svcQueryProcessMemory = Marshal.GetFunctionPointerForDelegate(queryProcessMemory = value); } QueryProcessMemoryDelegate queryProcessMemory;
    public MapProcessCodeMemoryDelegate MapProcessCodeMemory { get => mapProcessCodeMemory; set => CallbackTable.svcMapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate(mapProcessCodeMemory = value); } MapProcessCodeMemoryDelegate mapProcessCodeMemory;
    public UnmapProcessCodeMemoryDelegate UnmapProcessCodeMemory { get => unmapProcessCodeMemory; set => CallbackTable.svcUnmapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate(unmapProcessCodeMemory = value); } UnmapProcessCodeMemoryDelegate unmapProcessCodeMemory;
    public CreateProcessDelegate CreateProcess { get => createProcess; set => CallbackTable.svcCreateProcess = Marshal.GetFunctionPointerForDelegate(createProcess = value); } CreateProcessDelegate createProcess;
    public StartProcessDelegate StartProcess { get => startProcess; set => CallbackTable.svcStartProcess = Marshal.GetFunctionPointerForDelegate(startProcess = value); } StartProcessDelegate startProcess;
    public TerminateProcessDelegate TerminateProcess { get => terminateProcess; set => CallbackTable.svcTerminateProcess = Marshal.GetFunctionPointerForDelegate(terminateProcess = value); } TerminateProcessDelegate terminateProcess;
    public GetProcessInfoDelegate GetProcessInfo { get => getProcessInfo; set => CallbackTable.svcGetProcessInfo = Marshal.GetFunctionPointerForDelegate(getProcessInfo = value); } GetProcessInfoDelegate getProcessInfo;
    public CreateResourceLimitDelegate CreateResourceLimit { get => createResourceLimit; set => CallbackTable.svcCreateResourceLimit = Marshal.GetFunctionPointerForDelegate(createResourceLimit = value); } CreateResourceLimitDelegate createResourceLimit;
    public SetResourceLimitLimitValueDelegate SetResourceLimitLimitValue { get => setResourceLimitLimitValue; set => CallbackTable.svcSetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate(setResourceLimitLimitValue = value); } SetResourceLimitLimitValueDelegate setResourceLimitLimitValue;
    public CallSecureMonitorDelegate CallSecureMonitor { get => callSecureMonitor; set => CallbackTable.svcCallSecureMonitor = Marshal.GetFunctionPointerForDelegate(callSecureMonitor = value); } CallSecureMonitorDelegate callSecureMonitor;
    public SetMemoryAttribute2Delegate SetMemoryAttribute2 { get => setMemoryAttribute2; set => CallbackTable.svcSetMemoryAttribute2 = Marshal.GetFunctionPointerForDelegate(setMemoryAttribute2 = value); } SetMemoryAttribute2Delegate setMemoryAttribute2;
    public MapInsecurePhysicalMemoryDelegate MapInsecurePhysicalMemory { get => mapInsecurePhysicalMemory; set => CallbackTable.svcMapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate(mapInsecurePhysicalMemory = value); } MapInsecurePhysicalMemoryDelegate mapInsecurePhysicalMemory;
    public UnmapInsecurePhysicalMemoryDelegate UnmapInsecurePhysicalMemory { get => unmapInsecurePhysicalMemory; set => CallbackTable.svcUnmapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate(unmapInsecurePhysicalMemory = value); } UnmapInsecurePhysicalMemoryDelegate unmapInsecurePhysicalMemory;
}