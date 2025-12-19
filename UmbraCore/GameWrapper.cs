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

        Callbacks.NativeReentry = (state, op, a, b, replAddr) => {
            Console.WriteLine($"Native reentry from {0x71_00000000 + replAddr:X} ({op})");
            Core.Kernel.StackTrace((ulong*) state);
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
                    var ip = (uint) a;
                    var op2 = ip & 0b111U;
                    var crm = (ip >> 3) & 0b1111U;
                    var crn = (ip >> 7) & 0b1111U;
                    var op1 = (ip >> 11) & 0b111U;
                    var op0 = (ip >> 14) & 0b1U;
                    Callbacks.WriteSr(op0, op1, crn, crm, op2, *GetRegisterPointer(state, (int) b));
                    break;
                }
                case 3: {
                    var ip = (uint) a;
                    var op2 = ip & 0b111U;
                    var crm = (ip >> 3) & 0b1111U;
                    var crn = (ip >> 7) & 0b1111U;
                    var op1 = (ip >> 11) & 0b111U;
                    var op0 = (ip >> 14) & 0b1U;
                    *GetRegisterPointer(state, (int) b) = Callbacks.ReadSr(op0, op1, crn, crm, op2);
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
public unsafe delegate void NativeReentryDelegate(NativeState* state, uint op, ulong a, ulong b, ulong repl);
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

public unsafe class Callbacks {
    public CallbackTableOffsets CallbackTable;

    public DebugDelegate Debug {
        get;
        set => CallbackTable.debug = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("Debug not implemented");

    public LoadModuleDelegate LoadModule {
        get;
        set => CallbackTable.loadModule = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _, _, _, _) => throw new NotImplementedException("LoadModule not implemented");

    public InitModuleDelegate InitModule {
        get;
        set => CallbackTable.initModule = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("InitModule not implemented");

    public NativeReentryDelegate NativeReentry {
        get;
        set => CallbackTable.nativeReentry = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _) => throw new NotImplementedException("NativeReentry not implemented");

    public ReadSrDelegate ReadSr {
        get;
        set => CallbackTable.readSr = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _) => throw new NotImplementedException("ReadSr not implemented");

    public WriteSrDelegate WriteSr {
        get;
        set => CallbackTable.writeSr = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _) => throw new NotImplementedException("WriteSr not implemented");

    public SetHeapSizeDelegate SetHeapSize {
        get;
        set => CallbackTable.svcSetHeapSize = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("SetHeapSize not implemented");

    public SetMemoryPermissionDelegate SetMemoryPermission {
        get;
        set => CallbackTable.svcSetMemoryPermission = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("SetMemoryPermission not implemented");

    public SetMemoryAttributeDelegate SetMemoryAttribute {
        get;
        set => CallbackTable.svcSetMemoryAttribute = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("SetMemoryAttribute not implemented");

    public MapMemoryDelegate MapMemory {
        get;
        set => CallbackTable.svcMapMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("MapMemory not implemented");

    public UnmapMemoryDelegate UnmapMemory {
        get;
        set => CallbackTable.svcUnmapMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("UnmapMemory not implemented");

    public QueryMemoryDelegate QueryMemory {
        get;
        set => CallbackTable.svcQueryMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("QueryMemory not implemented");

    public ExitProcessDelegate ExitProcess {
        get;
        set => CallbackTable.svcExitProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("ExitProcess not implemented");

    public CreateThreadDelegate CreateThread {
        get;
        set => CallbackTable.svcCreateThread = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, ref _) => throw new NotImplementedException("CreateThread not implemented");

    public StartThreadDelegate StartThread {
        get;
        set => CallbackTable.svcStartThread = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("StartThread not implemented");

    public ExitThreadDelegate ExitThread {
        get;
        set => CallbackTable.svcExitThread = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("ExitThread not implemented");

    public SleepThreadDelegate SleepThread {
        get;
        set => CallbackTable.svcSleepThread = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("SleepThread not implemented");

    public GetThreadPriorityDelegate GetThreadPriority {
        get;
        set => CallbackTable.svcGetThreadPriority = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("GetThreadPriority not implemented");

    public SetThreadPriorityDelegate SetThreadPriority {
        get;
        set => CallbackTable.svcSetThreadPriority = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("SetThreadPriority not implemented");

    public GetThreadCoreMaskDelegate GetThreadCoreMask {
        get;
        set => CallbackTable.svcGetThreadCoreMask = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _, ref _) => throw new NotImplementedException("GetThreadCoreMask not implemented");

    public SetThreadCoreMaskDelegate SetThreadCoreMask {
        get;
        set => CallbackTable.svcSetThreadCoreMask = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("SetThreadCoreMask not implemented");

    public GetCurrentProcessorNumberDelegate GetCurrentProcessorNumber {
        get;
        set => CallbackTable.svcGetCurrentProcessorNumber = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("GetCurrentProcessorNumber not implemented");

    public SignalEventDelegate SignalEvent {
        get;
        set => CallbackTable.svcSignalEvent = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("SignalEvent not implemented");

    public ClearEventDelegate ClearEvent {
        get;
        set => CallbackTable.svcClearEvent = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("ClearEvent not implemented");

    public MapSharedMemoryDelegate MapSharedMemory {
        get;
        set => CallbackTable.svcMapSharedMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("MapSharedMemory not implemented");

    public UnmapSharedMemoryDelegate UnmapSharedMemory {
        get;
        set => CallbackTable.svcUnmapSharedMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("UnmapSharedMemory not implemented");

    public CreateTransferMemoryDelegate CreateTransferMemory {
        get;
        set => CallbackTable.svcCreateTransferMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("CreateTransferMemory not implemented");

    public CloseHandleDelegate CloseHandle {
        get;
        set => CallbackTable.svcCloseHandle = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("CloseHandle not implemented");

    public ResetSignalDelegate ResetSignal {
        get;
        set => CallbackTable.svcResetSignal = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("ResetSignal not implemented");

    public WaitSynchronizationDelegate WaitSynchronization {
        get;
        set => CallbackTable.svcWaitSynchronization = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("WaitSynchronization not implemented");

    public CancelSynchronizationDelegate CancelSynchronization {
        get;
        set => CallbackTable.svcCancelSynchronization = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("CancelSynchronization not implemented");

    public ArbitrateLockDelegate ArbitrateLock {
        get;
        set => CallbackTable.svcArbitrateLock = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("ArbitrateLock not implemented");

    public ArbitrateUnlockDelegate ArbitrateUnlock {
        get;
        set => CallbackTable.svcArbitrateUnlock = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("ArbitrateUnlock not implemented");

    public WaitProcessWideKeyAtomicDelegate WaitProcessWideKeyAtomic {
        get;
        set => CallbackTable.svcWaitProcessWideKeyAtomic = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("WaitProcessWideKeyAtomic not implemented");

    public SignalProcessWideKeyDelegate SignalProcessWideKey {
        get;
        set => CallbackTable.svcSignalProcessWideKey = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("SignalProcessWideKey not implemented");

    public GetSystemTickDelegate GetSystemTick {
        get;
        set => CallbackTable.svcGetSystemTick = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("GetSystemTick not implemented");

    public ConnectToNamedPortDelegate ConnectToNamedPort {
        get;
        set => CallbackTable.svcConnectToNamedPort = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("ConnectToNamedPort not implemented");

    public SendSyncRequestLightDelegate SendSyncRequestLight {
        get;
        set => CallbackTable.svcSendSyncRequestLight = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("SendSyncRequestLight not implemented");

    public SendSyncRequestDelegate SendSyncRequest {
        get;
        set => CallbackTable.svcSendSyncRequest = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("SendSyncRequest not implemented");

    public SendSyncRequestWithUserBufferDelegate SendSyncRequestWithUserBuffer {
        get;
        set => CallbackTable.svcSendSyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("SendSyncRequestWithUserBuffer not implemented");

    public SendAsyncRequestWithUserBufferDelegate SendAsyncRequestWithUserBuffer {
        get;
        set => CallbackTable.svcSendAsyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("SendAsyncRequestWithUserBuffer not implemented");

    public GetProcessIdDelegate GetProcessId {
        get;
        set => CallbackTable.svcGetProcessId = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("GetProcessId not implemented");

    public GetThreadIdDelegate GetThreadId {
        get;
        set => CallbackTable.svcGetThreadId = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("GetThreadId not implemented");

    public BreakDelegate Break {
        get;
        set => CallbackTable.svcBreak = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("Break not implemented");

    public OutputDebugStringDelegate OutputDebugString {
        get;
        set => CallbackTable.svcOutputDebugString = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("OutputDebugString not implemented");

    public ReturnFromExceptionDelegate ReturnFromException {
        get;
        set => CallbackTable.svcReturnFromException = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("ReturnFromException not implemented");

    public GetInfoDelegate GetInfo {
        get;
        set => CallbackTable.svcGetInfo = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("GetInfo not implemented");

    public FlushEntireDataCacheDelegate FlushEntireDataCache {
        get;
        set => CallbackTable.svcFlushEntireDataCache = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("FlushEntireDataCache not implemented");

    public FlushDataCacheDelegate FlushDataCache {
        get;
        set => CallbackTable.svcFlushDataCache = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("FlushDataCache not implemented");

    public MapPhysicalMemoryDelegate MapPhysicalMemory {
        get;
        set => CallbackTable.svcMapPhysicalMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("MapPhysicalMemory not implemented");

    public UnmapPhysicalMemoryDelegate UnmapPhysicalMemory {
        get;
        set => CallbackTable.svcUnmapPhysicalMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("UnmapPhysicalMemory not implemented");

    public GetDebugFutureThreadInfoDelegate GetDebugFutureThreadInfo {
        get;
        set => CallbackTable.svcGetDebugFutureThreadInfo = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _, ref _, ref _, ref _, ref _, ref _) =>
        throw new NotImplementedException("GetDebugFutureThreadInfo not implemented");

    public GetLastThreadInfoDelegate GetLastThreadInfo {
        get;
        set => CallbackTable.svcGetLastThreadInfo = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (ref _, ref _, ref _, ref _, ref _, ref _) =>
        throw new NotImplementedException("GetLastThreadInfo not implemented");

    public GetResourceLimitLimitValueDelegate GetResourceLimitLimitValue {
        get;
        set => CallbackTable.svcGetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("GetResourceLimitLimitValue not implemented");

    public GetResourceLimitCurrentValueDelegate GetResourceLimitCurrentValue {
        get;
        set => CallbackTable.svcGetResourceLimitCurrentValue = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("GetResourceLimitCurrentValue not implemented");

    public SetThreadActivityDelegate SetThreadActivity {
        get;
        set => CallbackTable.svcSetThreadActivity = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("SetThreadActivity not implemented");

    public GetThreadContext3Delegate GetThreadContext3 {
        get;
        set => CallbackTable.svcGetThreadContext3 = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("GetThreadContext3 not implemented");

    public WaitForAddressDelegate WaitForAddress {
        get;
        set => CallbackTable.svcWaitForAddress = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("WaitForAddress not implemented");

    public SignalToAddressDelegate SignalToAddress {
        get;
        set => CallbackTable.svcSignalToAddress = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("SignalToAddress not implemented");

    public SynchronizePreemptionStateDelegate SynchronizePreemptionState {
        get;
        set => CallbackTable.svcSynchronizePreemptionState = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("SynchronizePreemptionState not implemented");

    public GetResourceLimitPeakValueDelegate GetResourceLimitPeakValue {
        get;
        set => CallbackTable.svcGetResourceLimitPeakValue = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("GetResourceLimitPeakValue not implemented");

    public CreateIoPoolDelegate CreateIoPool {
        get;
        set => CallbackTable.svcCreateIoPool = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("CreateIoPool not implemented");

    public CreateIoRegionDelegate CreateIoRegion {
        get;
        set => CallbackTable.svcCreateIoRegion = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("CreateIoRegion not implemented");

    public KernelDebugDelegate KernelDebug {
        get;
        set => CallbackTable.svcKernelDebug = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("KernelDebug not implemented");

    public ChangeKernelTraceStateDelegate ChangeKernelTraceState {
        get;
        set => CallbackTable.svcChangeKernelTraceState = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("ChangeKernelTraceState not implemented");

    public CreateSessionDelegate CreateSession {
        get;
        set => CallbackTable.svcCreateSession = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _, ref _) => throw new NotImplementedException("CreateSession not implemented");

    public AcceptSessionDelegate AcceptSession {
        get;
        set => CallbackTable.svcAcceptSession = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("AcceptSession not implemented");

    public ReplyAndReceiveLightDelegate ReplyAndReceiveLight {
        get;
        set => CallbackTable.svcReplyAndReceiveLight = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("ReplyAndReceiveLight not implemented");

    public ReplyAndReceiveDelegate ReplyAndReceive {
        get;
        set => CallbackTable.svcReplyAndReceive = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, ref _) => throw new NotImplementedException("ReplyAndReceive not implemented");

    public ReplyAndReceiveWithUserBufferDelegate ReplyAndReceiveWithUserBuffer {
        get;
        set => CallbackTable.svcReplyAndReceiveWithUserBuffer = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _, ref _) => throw new NotImplementedException("ReplyAndReceiveWithUserBuffer not implemented");

    public CreateEventDelegate CreateEvent {
        get;
        set => CallbackTable.svcCreateEvent = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (ref _, ref _) => throw new NotImplementedException("CreateEvent not implemented");

    public MapIoRegionDelegate MapIoRegion {
        get;
        set => CallbackTable.svcMapIoRegion = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("MapIoRegion not implemented");

    public UnmapIoRegionDelegate UnmapIoRegion {
        get;
        set => CallbackTable.svcUnmapIoRegion = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("UnmapIoRegion not implemented");

    public MapPhysicalMemoryUnsafeDelegate MapPhysicalMemoryUnsafe {
        get;
        set => CallbackTable.svcMapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("MapPhysicalMemoryUnsafe not implemented");

    public UnmapPhysicalMemoryUnsafeDelegate UnmapPhysicalMemoryUnsafe {
        get;
        set => CallbackTable.svcUnmapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("UnmapPhysicalMemoryUnsafe not implemented");

    public SetUnsafeLimitDelegate SetUnsafeLimit {
        get;
        set => CallbackTable.svcSetUnsafeLimit = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("SetUnsafeLimit not implemented");

    public CreateCodeMemoryDelegate CreateCodeMemory {
        get;
        set => CallbackTable.svcCreateCodeMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("CreateCodeMemory not implemented");

    public ControlCodeMemoryDelegate ControlCodeMemory {
        get;
        set => CallbackTable.svcControlCodeMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _) => throw new NotImplementedException("ControlCodeMemory not implemented");

    public SleepSystemDelegate SleepSystem {
        get;
        set => CallbackTable.svcSleepSystem = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("SleepSystem not implemented");

    public ReadWriteRegisterDelegate ReadWriteRegister {
        get;
        set => CallbackTable.svcReadWriteRegister = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("ReadWriteRegister not implemented");

    public SetProcessActivityDelegate SetProcessActivity {
        get;
        set => CallbackTable.svcSetProcessActivity = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("SetProcessActivity not implemented");

    public CreateSharedMemoryDelegate CreateSharedMemory {
        get;
        set => CallbackTable.svcCreateSharedMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("CreateSharedMemory not implemented");

    public MapTransferMemoryDelegate MapTransferMemory {
        get;
        set => CallbackTable.svcMapTransferMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("MapTransferMemory not implemented");

    public UnmapTransferMemoryDelegate UnmapTransferMemory {
        get;
        set => CallbackTable.svcUnmapTransferMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("UnmapTransferMemory not implemented");

    public CreateInterruptEventDelegate CreateInterruptEvent {
        get;
        set => CallbackTable.svcCreateInterruptEvent = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("CreateInterruptEvent not implemented");

    public QueryPhysicalAddressDelegate QueryPhysicalAddress {
        get;
        set => CallbackTable.svcQueryPhysicalAddress = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _, ref _, ref _) => throw new NotImplementedException("QueryPhysicalAddress not implemented");

    public QueryMemoryMappingDelegate QueryMemoryMapping {
        get;
        set => CallbackTable.svcQueryMemoryMapping = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("QueryMemoryMapping not implemented");

    public CreateDeviceAddressSpaceDelegate CreateDeviceAddressSpace {
        get;
        set => CallbackTable.svcCreateDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("CreateDeviceAddressSpace not implemented");

    public AttachDeviceAddressSpaceDelegate AttachDeviceAddressSpace {
        get;
        set => CallbackTable.svcAttachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("AttachDeviceAddressSpace not implemented");

    public DetachDeviceAddressSpaceDelegate DetachDeviceAddressSpace {
        get;
        set => CallbackTable.svcDetachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("DetachDeviceAddressSpace not implemented");

    public MapDeviceAddressSpaceByForceDelegate MapDeviceAddressSpaceByForce {
        get;
        set => CallbackTable.svcMapDeviceAddressSpaceByForce = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _) => throw new NotImplementedException("MapDeviceAddressSpaceByForce not implemented");

    public MapDeviceAddressSpaceAlignedDelegate MapDeviceAddressSpaceAligned {
        get;
        set => CallbackTable.svcMapDeviceAddressSpaceAligned = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _) => throw new NotImplementedException("MapDeviceAddressSpaceAligned not implemented");

    public MapDeviceAddressSpaceDelegate MapDeviceAddressSpace {
        get;
        set => CallbackTable.svcMapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _, ref _) => throw new NotImplementedException("MapDeviceAddressSpace not implemented");

    public UnmapDeviceAddressSpaceDelegate UnmapDeviceAddressSpace {
        get;
        set => CallbackTable.svcUnmapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _) => throw new NotImplementedException("UnmapDeviceAddressSpace not implemented");

    public InvalidateProcessDataCacheDelegate InvalidateProcessDataCache {
        get;
        set => CallbackTable.svcInvalidateProcessDataCache = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("InvalidateProcessDataCache not implemented");

    public StoreProcessDataCacheDelegate StoreProcessDataCache {
        get;
        set => CallbackTable.svcStoreProcessDataCache = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("StoreProcessDataCache not implemented");

    public FlushProcessDataCacheDelegate FlushProcessDataCache {
        get;
        set => CallbackTable.svcFlushProcessDataCache = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("FlushProcessDataCache not implemented");

    public DebugActiveProcessDelegate DebugActiveProcess {
        get;
        set => CallbackTable.svcDebugActiveProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("DebugActiveProcess not implemented");

    public BreakDebugProcessDelegate BreakDebugProcess {
        get;
        set => CallbackTable.svcBreakDebugProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("BreakDebugProcess not implemented");

    public TerminateDebugProcessDelegate TerminateDebugProcess {
        get;
        set => CallbackTable.svcTerminateDebugProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("TerminateDebugProcess not implemented");

    public GetDebugEventDelegate GetDebugEvent {
        get;
        set => CallbackTable.svcGetDebugEvent = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _) => throw new NotImplementedException("GetDebugEvent not implemented");

    public ContinueDebugEventDelegate ContinueDebugEvent {
        get;
        set => CallbackTable.svcContinueDebugEvent = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("ContinueDebugEvent not implemented");

    public GetProcessListDelegate GetProcessList {
        get;
        set => CallbackTable.svcGetProcessList = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("GetProcessList not implemented");

    public GetThreadListDelegate GetThreadList {
        get;
        set => CallbackTable.svcGetThreadList = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("GetThreadList not implemented");

    public GetDebugThreadContextDelegate GetDebugThreadContext {
        get;
        set => CallbackTable.svcGetDebugThreadContext = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("GetDebugThreadContext not implemented");

    public SetDebugThreadContextDelegate SetDebugThreadContext {
        get;
        set => CallbackTable.svcSetDebugThreadContext = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("SetDebugThreadContext not implemented");

    public QueryDebugProcessMemoryDelegate QueryDebugProcessMemory {
        get;
        set => CallbackTable.svcQueryDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("QueryDebugProcessMemory not implemented");

    public ReadDebugProcessMemoryDelegate ReadDebugProcessMemory {
        get;
        set => CallbackTable.svcReadDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("ReadDebugProcessMemory not implemented");

    public WriteDebugProcessMemoryDelegate WriteDebugProcessMemory {
        get;
        set => CallbackTable.svcWriteDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("WriteDebugProcessMemory not implemented");

    public SetHardwareBreakPointDelegate SetHardwareBreakPoint {
        get;
        set => CallbackTable.svcSetHardwareBreakPoint = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("SetHardwareBreakPoint not implemented");

    public GetDebugThreadParamDelegate GetDebugThreadParam {
        get;
        set => CallbackTable.svcGetDebugThreadParam = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _, ref _) => throw new NotImplementedException("GetDebugThreadParam not implemented");

    public GetSystemInfoDelegate GetSystemInfo {
        get;
        set => CallbackTable.svcGetSystemInfo = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("GetSystemInfo not implemented");

    public CreatePortDelegate CreatePort {
        get;
        set => CallbackTable.svcCreatePort = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _, ref _) => throw new NotImplementedException("CreatePort not implemented");

    public ManageNamedPortDelegate ManageNamedPort {
        get;
        set => CallbackTable.svcManageNamedPort = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("ManageNamedPort not implemented");

    public ConnectToPortDelegate ConnectToPort {
        get;
        set => CallbackTable.svcConnectToPort = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, ref _) => throw new NotImplementedException("ConnectToPort not implemented");

    public SetProcessMemoryPermissionDelegate SetProcessMemoryPermission {
        get;
        set => CallbackTable.svcSetProcessMemoryPermission = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("SetProcessMemoryPermission not implemented");

    public MapProcessMemoryDelegate MapProcessMemory {
        get;
        set => CallbackTable.svcMapProcessMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("MapProcessMemory not implemented");

    public UnmapProcessMemoryDelegate UnmapProcessMemory {
        get;
        set => CallbackTable.svcUnmapProcessMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("UnmapProcessMemory not implemented");

    public QueryProcessMemoryDelegate QueryProcessMemory {
        get;
        set => CallbackTable.svcQueryProcessMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("QueryProcessMemory not implemented");

    public MapProcessCodeMemoryDelegate MapProcessCodeMemory {
        get;
        set => CallbackTable.svcMapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("MapProcessCodeMemory not implemented");

    public UnmapProcessCodeMemoryDelegate UnmapProcessCodeMemory {
        get;
        set => CallbackTable.svcUnmapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("UnmapProcessCodeMemory not implemented");

    public CreateProcessDelegate CreateProcess {
        get;
        set => CallbackTable.svcCreateProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, ref _) => throw new NotImplementedException("CreateProcess not implemented");

    public StartProcessDelegate StartProcess {
        get;
        set => CallbackTable.svcStartProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _) => throw new NotImplementedException("StartProcess not implemented");

    public TerminateProcessDelegate TerminateProcess {
        get;
        set => CallbackTable.svcTerminateProcess = Marshal.GetFunctionPointerForDelegate(field = value);
    } = _ => throw new NotImplementedException("TerminateProcess not implemented");

    public GetProcessInfoDelegate GetProcessInfo {
        get;
        set => CallbackTable.svcGetProcessInfo = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, ref _) => throw new NotImplementedException("GetProcessInfo not implemented");

    public CreateResourceLimitDelegate CreateResourceLimit {
        get;
        set => CallbackTable.svcCreateResourceLimit = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (ref _) => throw new NotImplementedException("CreateResourceLimit not implemented");

    public SetResourceLimitLimitValueDelegate SetResourceLimitLimitValue {
        get;
        set => CallbackTable.svcSetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _) => throw new NotImplementedException("SetResourceLimitLimitValue not implemented");

    public CallSecureMonitorDelegate CallSecureMonitor {
        get;
        set => CallbackTable.svcCallSecureMonitor = Marshal.GetFunctionPointerForDelegate(field = value);
    } = (_, _, _, _, _, _, _, _, ref _, ref _, ref _, ref _, ref _, ref _, ref _) =>
        throw new NotImplementedException("CallSecureMonitor not implemented");

    public SetMemoryAttribute2Delegate SetMemoryAttribute2 {
        get;
        set => CallbackTable.svcSetMemoryAttribute2 = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("SetMemoryAttribute2 not implemented");

    public MapInsecurePhysicalMemoryDelegate MapInsecurePhysicalMemory {
        get;
        set => CallbackTable.svcMapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("MapInsecurePhysicalMemory not implemented");

    public UnmapInsecurePhysicalMemoryDelegate UnmapInsecurePhysicalMemory {
        get;
        set => CallbackTable.svcUnmapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate(field = value);
    } = () => throw new NotImplementedException("UnmapInsecurePhysicalMemory not implemented");
}