using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore;

public unsafe class GameWrapper {
    [DllImport("../NxRecompile/libtest.dylib")]
    public static extern void setup(CpuState* state, ref Callbacks callbacks);
    [DllImport("../NxRecompile/libtest.dylib")]
    public static extern void runFrom(ulong addr, ulong until);
}

public delegate void StubDelegate();
public delegate void DebugDelegate(ulong pc, string dasm);
public unsafe delegate void LoadModuleDelegate(ulong loadBase, byte* data, int size);
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

[StructLayout(LayoutKind.Sequential)]
public struct Callbacks() {
    IntPtr debug = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("debug"));
    public DebugDelegate Debug { set => debug = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr loadModule = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("loadModule"));
    public LoadModuleDelegate LoadModule { set => loadModule = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr readSr = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("readSr"));
    public ReadSrDelegate ReadSr { set => readSr = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr writeSr = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("writeSr"));
    public WriteSrDelegate WriteSr { set => writeSr = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetHeapSize = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetHeapSize"));
	public SetHeapSizeDelegate SetHeapSize { set => svcSetHeapSize = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetMemoryPermission = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetMemoryPermission"));
	public SetMemoryPermissionDelegate SetMemoryPermission { set => svcSetMemoryPermission = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetMemoryAttribute = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetMemoryAttribute"));
	public SetMemoryAttributeDelegate SetMemoryAttribute { set => svcSetMemoryAttribute = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapMemory"));
	public MapMemoryDelegate MapMemory { set => svcMapMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapMemory"));
	public UnmapMemoryDelegate UnmapMemory { set => svcUnmapMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcQueryMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcQueryMemory"));
	public QueryMemoryDelegate QueryMemory { set => svcQueryMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcExitProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcExitProcess"));
	public ExitProcessDelegate ExitProcess { set => svcExitProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateThread = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateThread"));
	public CreateThreadDelegate CreateThread { set => svcCreateThread = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcStartThread = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcStartThread"));
	public StartThreadDelegate StartThread { set => svcStartThread = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcExitThread = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcExitThread"));
	public ExitThreadDelegate ExitThread { set => svcExitThread = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSleepThread = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSleepThread"));
	public SleepThreadDelegate SleepThread { set => svcSleepThread = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetThreadPriority = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetThreadPriority"));
	public GetThreadPriorityDelegate GetThreadPriority { set => svcGetThreadPriority = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetThreadPriority = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetThreadPriority"));
	public SetThreadPriorityDelegate SetThreadPriority { set => svcSetThreadPriority = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetThreadCoreMask = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetThreadCoreMask"));
	public GetThreadCoreMaskDelegate GetThreadCoreMask { set => svcGetThreadCoreMask = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetThreadCoreMask = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetThreadCoreMask"));
	public SetThreadCoreMaskDelegate SetThreadCoreMask { set => svcSetThreadCoreMask = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetCurrentProcessorNumber = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetCurrentProcessorNumber"));
	public GetCurrentProcessorNumberDelegate GetCurrentProcessorNumber { set => svcGetCurrentProcessorNumber = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSignalEvent = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSignalEvent"));
	public SignalEventDelegate SignalEvent { set => svcSignalEvent = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcClearEvent = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcClearEvent"));
	public ClearEventDelegate ClearEvent { set => svcClearEvent = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapSharedMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapSharedMemory"));
	public MapSharedMemoryDelegate MapSharedMemory { set => svcMapSharedMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapSharedMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapSharedMemory"));
	public UnmapSharedMemoryDelegate UnmapSharedMemory { set => svcUnmapSharedMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateTransferMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateTransferMemory"));
	public CreateTransferMemoryDelegate CreateTransferMemory { set => svcCreateTransferMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCloseHandle = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCloseHandle"));
	public CloseHandleDelegate CloseHandle { set => svcCloseHandle = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcResetSignal = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcResetSignal"));
	public ResetSignalDelegate ResetSignal { set => svcResetSignal = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcWaitSynchronization = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcWaitSynchronization"));
	public WaitSynchronizationDelegate WaitSynchronization { set => svcWaitSynchronization = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCancelSynchronization = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCancelSynchronization"));
	public CancelSynchronizationDelegate CancelSynchronization { set => svcCancelSynchronization = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcArbitrateLock = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcArbitrateLock"));
	public ArbitrateLockDelegate ArbitrateLock { set => svcArbitrateLock = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcArbitrateUnlock = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcArbitrateUnlock"));
	public ArbitrateUnlockDelegate ArbitrateUnlock { set => svcArbitrateUnlock = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcWaitProcessWideKeyAtomic = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcWaitProcessWideKeyAtomic"));
	public WaitProcessWideKeyAtomicDelegate WaitProcessWideKeyAtomic { set => svcWaitProcessWideKeyAtomic = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSignalProcessWideKey = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSignalProcessWideKey"));
	public SignalProcessWideKeyDelegate SignalProcessWideKey { set => svcSignalProcessWideKey = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetSystemTick = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetSystemTick"));
	public GetSystemTickDelegate GetSystemTick { set => svcGetSystemTick = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcConnectToNamedPort = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcConnectToNamedPort"));
	public ConnectToNamedPortDelegate ConnectToNamedPort { set => svcConnectToNamedPort = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSendSyncRequestLight = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSendSyncRequestLight"));
	public SendSyncRequestLightDelegate SendSyncRequestLight { set => svcSendSyncRequestLight = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSendSyncRequest = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSendSyncRequest"));
	public SendSyncRequestDelegate SendSyncRequest { set => svcSendSyncRequest = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSendSyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSendSyncRequestWithUserBuffer"));
	public SendSyncRequestWithUserBufferDelegate SendSyncRequestWithUserBuffer { set => svcSendSyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSendAsyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSendAsyncRequestWithUserBuffer"));
	public SendAsyncRequestWithUserBufferDelegate SendAsyncRequestWithUserBuffer { set => svcSendAsyncRequestWithUserBuffer = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetProcessId = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetProcessId"));
	public GetProcessIdDelegate GetProcessId { set => svcGetProcessId = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetThreadId = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetThreadId"));
	public GetThreadIdDelegate GetThreadId { set => svcGetThreadId = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcBreak = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcBreak"));
	public BreakDelegate Break { set => svcBreak = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcOutputDebugString = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcOutputDebugString"));
	public OutputDebugStringDelegate OutputDebugString { set => svcOutputDebugString = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcReturnFromException = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcReturnFromException"));
	public ReturnFromExceptionDelegate ReturnFromException { set => svcReturnFromException = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetInfo = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetInfo"));
	public GetInfoDelegate GetInfo { set => svcGetInfo = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcFlushEntireDataCache = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcFlushEntireDataCache"));
	public FlushEntireDataCacheDelegate FlushEntireDataCache { set => svcFlushEntireDataCache = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcFlushDataCache = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcFlushDataCache"));
	public FlushDataCacheDelegate FlushDataCache { set => svcFlushDataCache = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapPhysicalMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapPhysicalMemory"));
	public MapPhysicalMemoryDelegate MapPhysicalMemory { set => svcMapPhysicalMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapPhysicalMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapPhysicalMemory"));
	public UnmapPhysicalMemoryDelegate UnmapPhysicalMemory { set => svcUnmapPhysicalMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetDebugFutureThreadInfo = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetDebugFutureThreadInfo"));
	public GetDebugFutureThreadInfoDelegate GetDebugFutureThreadInfo { set => svcGetDebugFutureThreadInfo = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetLastThreadInfo = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetLastThreadInfo"));
	public GetLastThreadInfoDelegate GetLastThreadInfo { set => svcGetLastThreadInfo = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetResourceLimitLimitValue"));
	public GetResourceLimitLimitValueDelegate GetResourceLimitLimitValue { set => svcGetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetResourceLimitCurrentValue = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetResourceLimitCurrentValue"));
	public GetResourceLimitCurrentValueDelegate GetResourceLimitCurrentValue { set => svcGetResourceLimitCurrentValue = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetThreadActivity = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetThreadActivity"));
	public SetThreadActivityDelegate SetThreadActivity { set => svcSetThreadActivity = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetThreadContext3 = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetThreadContext3"));
	public GetThreadContext3Delegate GetThreadContext3 { set => svcGetThreadContext3 = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcWaitForAddress = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcWaitForAddress"));
	public WaitForAddressDelegate WaitForAddress { set => svcWaitForAddress = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSignalToAddress = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSignalToAddress"));
	public SignalToAddressDelegate SignalToAddress { set => svcSignalToAddress = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSynchronizePreemptionState = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSynchronizePreemptionState"));
	public SynchronizePreemptionStateDelegate SynchronizePreemptionState { set => svcSynchronizePreemptionState = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetResourceLimitPeakValue = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetResourceLimitPeakValue"));
	public GetResourceLimitPeakValueDelegate GetResourceLimitPeakValue { set => svcGetResourceLimitPeakValue = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateIoPool = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateIoPool"));
	public CreateIoPoolDelegate CreateIoPool { set => svcCreateIoPool = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateIoRegion = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateIoRegion"));
	public CreateIoRegionDelegate CreateIoRegion { set => svcCreateIoRegion = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcKernelDebug = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcKernelDebug"));
	public KernelDebugDelegate KernelDebug { set => svcKernelDebug = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcChangeKernelTraceState = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcChangeKernelTraceState"));
	public ChangeKernelTraceStateDelegate ChangeKernelTraceState { set => svcChangeKernelTraceState = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateSession = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateSession"));
	public CreateSessionDelegate CreateSession { set => svcCreateSession = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcAcceptSession = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcAcceptSession"));
	public AcceptSessionDelegate AcceptSession { set => svcAcceptSession = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcReplyAndReceiveLight = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcReplyAndReceiveLight"));
	public ReplyAndReceiveLightDelegate ReplyAndReceiveLight { set => svcReplyAndReceiveLight = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcReplyAndReceive = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcReplyAndReceive"));
	public ReplyAndReceiveDelegate ReplyAndReceive { set => svcReplyAndReceive = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcReplyAndReceiveWithUserBuffer = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcReplyAndReceiveWithUserBuffer"));
	public ReplyAndReceiveWithUserBufferDelegate ReplyAndReceiveWithUserBuffer { set => svcReplyAndReceiveWithUserBuffer = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateEvent = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateEvent"));
	public CreateEventDelegate CreateEvent { set => svcCreateEvent = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapIoRegion = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapIoRegion"));
	public MapIoRegionDelegate MapIoRegion { set => svcMapIoRegion = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapIoRegion = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapIoRegion"));
	public UnmapIoRegionDelegate UnmapIoRegion { set => svcUnmapIoRegion = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapPhysicalMemoryUnsafe"));
	public MapPhysicalMemoryUnsafeDelegate MapPhysicalMemoryUnsafe { set => svcMapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapPhysicalMemoryUnsafe"));
	public UnmapPhysicalMemoryUnsafeDelegate UnmapPhysicalMemoryUnsafe { set => svcUnmapPhysicalMemoryUnsafe = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetUnsafeLimit = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetUnsafeLimit"));
	public SetUnsafeLimitDelegate SetUnsafeLimit { set => svcSetUnsafeLimit = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateCodeMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateCodeMemory"));
	public CreateCodeMemoryDelegate CreateCodeMemory { set => svcCreateCodeMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcControlCodeMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcControlCodeMemory"));
	public ControlCodeMemoryDelegate ControlCodeMemory { set => svcControlCodeMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSleepSystem = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSleepSystem"));
	public SleepSystemDelegate SleepSystem { set => svcSleepSystem = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcReadWriteRegister = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcReadWriteRegister"));
	public ReadWriteRegisterDelegate ReadWriteRegister { set => svcReadWriteRegister = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetProcessActivity = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetProcessActivity"));
	public SetProcessActivityDelegate SetProcessActivity { set => svcSetProcessActivity = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateSharedMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateSharedMemory"));
	public CreateSharedMemoryDelegate CreateSharedMemory { set => svcCreateSharedMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapTransferMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapTransferMemory"));
	public MapTransferMemoryDelegate MapTransferMemory { set => svcMapTransferMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapTransferMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapTransferMemory"));
	public UnmapTransferMemoryDelegate UnmapTransferMemory { set => svcUnmapTransferMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateInterruptEvent = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateInterruptEvent"));
	public CreateInterruptEventDelegate CreateInterruptEvent { set => svcCreateInterruptEvent = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcQueryPhysicalAddress = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcQueryPhysicalAddress"));
	public QueryPhysicalAddressDelegate QueryPhysicalAddress { set => svcQueryPhysicalAddress = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcQueryMemoryMapping = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcQueryMemoryMapping"));
	public QueryMemoryMappingDelegate QueryMemoryMapping { set => svcQueryMemoryMapping = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateDeviceAddressSpace"));
	public CreateDeviceAddressSpaceDelegate CreateDeviceAddressSpace { set => svcCreateDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcAttachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcAttachDeviceAddressSpace"));
	public AttachDeviceAddressSpaceDelegate AttachDeviceAddressSpace { set => svcAttachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcDetachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcDetachDeviceAddressSpace"));
	public DetachDeviceAddressSpaceDelegate DetachDeviceAddressSpace { set => svcDetachDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapDeviceAddressSpaceByForce = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapDeviceAddressSpaceByForce"));
	public MapDeviceAddressSpaceByForceDelegate MapDeviceAddressSpaceByForce { set => svcMapDeviceAddressSpaceByForce = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapDeviceAddressSpaceAligned = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapDeviceAddressSpaceAligned"));
	public MapDeviceAddressSpaceAlignedDelegate MapDeviceAddressSpaceAligned { set => svcMapDeviceAddressSpaceAligned = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapDeviceAddressSpace"));
	public MapDeviceAddressSpaceDelegate MapDeviceAddressSpace { set => svcMapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapDeviceAddressSpace"));
	public UnmapDeviceAddressSpaceDelegate UnmapDeviceAddressSpace { set => svcUnmapDeviceAddressSpace = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcInvalidateProcessDataCache = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcInvalidateProcessDataCache"));
	public InvalidateProcessDataCacheDelegate InvalidateProcessDataCache { set => svcInvalidateProcessDataCache = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcStoreProcessDataCache = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcStoreProcessDataCache"));
	public StoreProcessDataCacheDelegate StoreProcessDataCache { set => svcStoreProcessDataCache = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcFlushProcessDataCache = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcFlushProcessDataCache"));
	public FlushProcessDataCacheDelegate FlushProcessDataCache { set => svcFlushProcessDataCache = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcDebugActiveProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcDebugActiveProcess"));
	public DebugActiveProcessDelegate DebugActiveProcess { set => svcDebugActiveProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcBreakDebugProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcBreakDebugProcess"));
	public BreakDebugProcessDelegate BreakDebugProcess { set => svcBreakDebugProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcTerminateDebugProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcTerminateDebugProcess"));
	public TerminateDebugProcessDelegate TerminateDebugProcess { set => svcTerminateDebugProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetDebugEvent = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetDebugEvent"));
	public GetDebugEventDelegate GetDebugEvent { set => svcGetDebugEvent = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcContinueDebugEvent = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcContinueDebugEvent"));
	public ContinueDebugEventDelegate ContinueDebugEvent { set => svcContinueDebugEvent = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetProcessList = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetProcessList"));
	public GetProcessListDelegate GetProcessList { set => svcGetProcessList = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetThreadList = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetThreadList"));
	public GetThreadListDelegate GetThreadList { set => svcGetThreadList = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetDebugThreadContext = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetDebugThreadContext"));
	public GetDebugThreadContextDelegate GetDebugThreadContext { set => svcGetDebugThreadContext = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetDebugThreadContext = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetDebugThreadContext"));
	public SetDebugThreadContextDelegate SetDebugThreadContext { set => svcSetDebugThreadContext = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcQueryDebugProcessMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcQueryDebugProcessMemory"));
	public QueryDebugProcessMemoryDelegate QueryDebugProcessMemory { set => svcQueryDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcReadDebugProcessMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcReadDebugProcessMemory"));
	public ReadDebugProcessMemoryDelegate ReadDebugProcessMemory { set => svcReadDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcWriteDebugProcessMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcWriteDebugProcessMemory"));
	public WriteDebugProcessMemoryDelegate WriteDebugProcessMemory { set => svcWriteDebugProcessMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetHardwareBreakPoint = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetHardwareBreakPoint"));
	public SetHardwareBreakPointDelegate SetHardwareBreakPoint { set => svcSetHardwareBreakPoint = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetDebugThreadParam = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetDebugThreadParam"));
	public GetDebugThreadParamDelegate GetDebugThreadParam { set => svcGetDebugThreadParam = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetSystemInfo = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetSystemInfo"));
	public GetSystemInfoDelegate GetSystemInfo { set => svcGetSystemInfo = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreatePort = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreatePort"));
	public CreatePortDelegate CreatePort { set => svcCreatePort = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcManageNamedPort = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcManageNamedPort"));
	public ManageNamedPortDelegate ManageNamedPort { set => svcManageNamedPort = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcConnectToPort = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcConnectToPort"));
	public ConnectToPortDelegate ConnectToPort { set => svcConnectToPort = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetProcessMemoryPermission = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetProcessMemoryPermission"));
	public SetProcessMemoryPermissionDelegate SetProcessMemoryPermission { set => svcSetProcessMemoryPermission = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapProcessMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapProcessMemory"));
	public MapProcessMemoryDelegate MapProcessMemory { set => svcMapProcessMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapProcessMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapProcessMemory"));
	public UnmapProcessMemoryDelegate UnmapProcessMemory { set => svcUnmapProcessMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcQueryProcessMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcQueryProcessMemory"));
	public QueryProcessMemoryDelegate QueryProcessMemory { set => svcQueryProcessMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapProcessCodeMemory"));
	public MapProcessCodeMemoryDelegate MapProcessCodeMemory { set => svcMapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapProcessCodeMemory"));
	public UnmapProcessCodeMemoryDelegate UnmapProcessCodeMemory { set => svcUnmapProcessCodeMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateProcess"));
	public CreateProcessDelegate CreateProcess { set => svcCreateProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcStartProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcStartProcess"));
	public StartProcessDelegate StartProcess { set => svcStartProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcTerminateProcess = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcTerminateProcess"));
	public TerminateProcessDelegate TerminateProcess { set => svcTerminateProcess = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcGetProcessInfo = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcGetProcessInfo"));
	public GetProcessInfoDelegate GetProcessInfo { set => svcGetProcessInfo = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCreateResourceLimit = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCreateResourceLimit"));
	public CreateResourceLimitDelegate CreateResourceLimit { set => svcCreateResourceLimit = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetResourceLimitLimitValue"));
	public SetResourceLimitLimitValueDelegate SetResourceLimitLimitValue { set => svcSetResourceLimitLimitValue = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcCallSecureMonitor = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcCallSecureMonitor"));
	public CallSecureMonitorDelegate CallSecureMonitor { set => svcCallSecureMonitor = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcSetMemoryAttribute2 = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcSetMemoryAttribute2"));
	public SetMemoryAttribute2Delegate SetMemoryAttribute2 { set => svcSetMemoryAttribute2 = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcMapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcMapInsecurePhysicalMemory"));
	public MapInsecurePhysicalMemoryDelegate MapInsecurePhysicalMemory { set => svcMapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate(value); }
    IntPtr svcUnmapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate<StubDelegate>(() => throw new NotImplementedException("svcUnmapInsecurePhysicalMemory"));
	public UnmapInsecurePhysicalMemoryDelegate UnmapInsecurePhysicalMemory { set => svcUnmapInsecurePhysicalMemory = Marshal.GetFunctionPointerForDelegate(value); }
}