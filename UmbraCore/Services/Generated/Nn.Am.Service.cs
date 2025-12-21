using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Am.Service;
public partial class IAllSystemAppletProxiesService : _IAllSystemAppletProxiesService_Base {
	public readonly string ServiceName;
	public IAllSystemAppletProxiesService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAllSystemAppletProxiesService_Base : IpcInterface {
	protected virtual Nn.Am.Service.ISystemAppletProxy OpenSystemAppletProxy(ulong _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.IAllSystemAppletProxiesService.OpenSystemAppletProxy not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletProxy OpenLibraryAppletProxyOld(ulong _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.IAllSystemAppletProxiesService.OpenLibraryAppletProxyOld not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletProxy OpenLibraryAppletProxy(ulong _0, ulong _1, KObject _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Am.Service.IAllSystemAppletProxiesService.OpenLibraryAppletProxy not implemented");
	protected virtual Nn.Am.Service.IOverlayAppletProxy OpenOverlayAppletProxy(ulong _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.IAllSystemAppletProxiesService.OpenOverlayAppletProxy not implemented");
	protected virtual Nn.Am.Service.IApplicationProxy OpenSystemApplicationProxy(ulong _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.IAllSystemAppletProxiesService.OpenSystemApplicationProxy not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletCreator CreateSelfLibraryAppletCreatorForDevelop(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Am.Service.IAllSystemAppletProxiesService.CreateSelfLibraryAppletCreatorForDevelop not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: { // OpenSystemAppletProxy
				var _return = OpenSystemAppletProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // OpenLibraryAppletProxyOld
				var _return = OpenLibraryAppletProxyOld(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // OpenLibraryAppletProxy
				var _return = OpenLibraryAppletProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x15, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12C: { // OpenOverlayAppletProxy
				var _return = OpenOverlayAppletProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15E: { // OpenSystemApplicationProxy
				var _return = OpenSystemApplicationProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // CreateSelfLibraryAppletCreatorForDevelop
				var _return = CreateSelfLibraryAppletCreatorForDevelop(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IAllSystemAppletProxiesService");
		}
	}
}

public partial class IAppletAccessor : _IAppletAccessor_Base;
public abstract class _IAppletAccessor_Base : IpcInterface {
	protected virtual KObject GetAppletStateChangedEvent() =>
		throw new NotImplementedException("Nn.Am.Service.IAppletAccessor.GetAppletStateChangedEvent not implemented");
	protected virtual byte IsCompleted() =>
		throw new NotImplementedException("Nn.Am.Service.IAppletAccessor.IsCompleted not implemented");
	protected virtual void Start() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAppletAccessor.Start");
	protected virtual void RequestExit() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAppletAccessor.RequestExit");
	protected virtual void Terminate() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAppletAccessor.Terminate");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAppletAccessor.GetResult");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAppletStateChangedEvent
				var _return = GetAppletStateChangedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // IsCompleted
				var _return = IsCompleted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Start
				Start();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // RequestExit
				RequestExit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Terminate
				Terminate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IAppletAccessor");
		}
	}
}

public partial class IApplicationAccessor : _IApplicationAccessor_Base;
public abstract class _IApplicationAccessor_Base : IpcInterface {
	protected virtual KObject GetAppletStateChangedEvent() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.GetAppletStateChangedEvent not implemented");
	protected virtual byte IsCompleted() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.IsCompleted not implemented");
	protected virtual void Start() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.Start");
	protected virtual void RequestExit() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.RequestExit");
	protected virtual void Terminate() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.Terminate");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.GetResult");
	protected virtual void RequestForApplicationToGetForeground() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.RequestForApplicationToGetForeground");
	protected virtual void TerminateAllLibraryApplets() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.TerminateAllLibraryApplets");
	protected virtual byte AreAnyLibraryAppletsLeft() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.AreAnyLibraryAppletsLeft not implemented");
	protected virtual Nn.Am.Service.IAppletAccessor GetCurrentLibraryApplet() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.GetCurrentLibraryApplet not implemented");
	protected virtual ulong GetApplicationId() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.GetApplicationId not implemented");
	protected virtual void PushLaunchParameter(uint _0, Nn.Am.Service.IStorage _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationAccessor.PushLaunchParameter");
	protected virtual void GetApplicationControlProperty(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.GetApplicationControlProperty not implemented");
	protected virtual void GetApplicationLaunchProperty(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationAccessor.GetApplicationLaunchProperty not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAppletStateChangedEvent
				var _return = GetAppletStateChangedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // IsCompleted
				var _return = IsCompleted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Start
				Start();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // RequestExit
				RequestExit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Terminate
				Terminate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // RequestForApplicationToGetForeground
				RequestForApplicationToGetForeground();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6E: { // TerminateAllLibraryApplets
				TerminateAllLibraryApplets();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // AreAnyLibraryAppletsLeft
				var _return = AreAnyLibraryAppletsLeft();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x70: { // GetCurrentLibraryApplet
				var _return = GetCurrentLibraryApplet();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x78: { // GetApplicationId
				var _return = GetApplicationId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x79: { // PushLaunchParameter
				PushLaunchParameter(im.GetData<uint>(8), Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7A: { // GetApplicationControlProperty
				GetApplicationControlProperty(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7B: { // GetApplicationLaunchProperty
				GetApplicationLaunchProperty(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationAccessor");
		}
	}
}

public partial class IApplicationCreator : _IApplicationCreator_Base;
public abstract class _IApplicationCreator_Base : IpcInterface {
	protected virtual Nn.Am.Service.IApplicationAccessor CreateApplication(ulong _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationCreator.CreateApplication not implemented");
	protected virtual Nn.Am.Service.IApplicationAccessor PopLaunchRequestedApplication() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationCreator.PopLaunchRequestedApplication not implemented");
	protected virtual Nn.Am.Service.IApplicationAccessor CreateSystemApplication(ulong _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationCreator.CreateSystemApplication not implemented");
	protected virtual Nn.Am.Service.IApplicationAccessor PopFloatingApplicationForDevelopment() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationCreator.PopFloatingApplicationForDevelopment not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateApplication
				var _return = CreateApplication(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // PopLaunchRequestedApplication
				var _return = PopLaunchRequestedApplication();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // CreateSystemApplication
				var _return = CreateSystemApplication(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x64: { // PopFloatingApplicationForDevelopment
				var _return = PopFloatingApplicationForDevelopment();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationCreator");
		}
	}
}

public partial class IApplicationFunctions : _IApplicationFunctions_Base;
public abstract class _IApplicationFunctions_Base : IpcInterface {
	protected virtual Nn.Am.Service.IStorage PopLaunchParameter(uint _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.PopLaunchParameter not implemented");
	protected virtual void CreateApplicationAndPushAndRequestToStart(ulong _0, Nn.Am.Service.IStorage _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.CreateApplicationAndPushAndRequestToStart");
	protected virtual void CreateApplicationAndPushAndRequestToStartForQuest(uint _0, uint _1, ulong _2, Nn.Am.Service.IStorage _3) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.CreateApplicationAndPushAndRequestToStartForQuest");
	protected virtual void CreateApplicationAndRequestToStart(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.CreateApplicationAndRequestToStart");
	protected virtual void CreateApplicationAndRequestToStartForQuest(uint _0, uint _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.CreateApplicationAndRequestToStartForQuest");
	protected virtual ulong EnsureSaveData(byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.EnsureSaveData not implemented");
	protected virtual void GetDesiredLanguage(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.GetDesiredLanguage not implemented");
	protected virtual void SetTerminateResult(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.SetTerminateResult");
	protected virtual void GetDisplayVersion(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.GetDisplayVersion not implemented");
	protected virtual void GetLaunchStorageInfoForDebug(out byte _0, out byte _1) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.GetLaunchStorageInfoForDebug not implemented");
	protected virtual ulong ExtendSaveData(byte _0, byte[] _1, ulong _2, ulong _3) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.ExtendSaveData not implemented");
	protected virtual void GetSaveDataSize(byte _0, byte[] _1, out ulong _2, out ulong _3) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.GetSaveDataSize not implemented");
	protected virtual void BeginBlockingHomeButtonShortAndLongPressed(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.BeginBlockingHomeButtonShortAndLongPressed");
	protected virtual void EndBlockingHomeButtonShortAndLongPressed() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.EndBlockingHomeButtonShortAndLongPressed");
	protected virtual void BeginBlockingHomeButton(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.BeginBlockingHomeButton");
	protected virtual void EndBlockingHomeButton() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.EndBlockingHomeButton");
	protected virtual byte NotifyRunning() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.NotifyRunning not implemented");
	protected virtual void GetPseudoDeviceId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.GetPseudoDeviceId not implemented");
	protected virtual void SetMediaPlaybackStateForApplication(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.SetMediaPlaybackStateForApplication");
	protected virtual byte IsGamePlayRecordingSupported() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationFunctions.IsGamePlayRecordingSupported not implemented");
	protected virtual void InitializeGamePlayRecording(ulong _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.InitializeGamePlayRecording");
	protected virtual void SetGamePlayRecordingState(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.SetGamePlayRecordingState");
	protected virtual void RequestFlushGamePlayingMovieForDebug() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.RequestFlushGamePlayingMovieForDebug");
	protected virtual void RequestToShutdown() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.RequestToShutdown");
	protected virtual void RequestToReboot() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.RequestToReboot");
	protected virtual void ExitAndRequestToShowThanksMessage() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.ExitAndRequestToShowThanksMessage");
	protected virtual void EnableApplicationCrashReport(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IApplicationFunctions.EnableApplicationCrashReport");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // PopLaunchParameter
				var _return = PopLaunchParameter(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // CreateApplicationAndPushAndRequestToStart
				CreateApplicationAndPushAndRequestToStart(im.GetData<ulong>(8), Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // CreateApplicationAndPushAndRequestToStartForQuest
				CreateApplicationAndPushAndRequestToStartForQuest(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // CreateApplicationAndRequestToStart
				CreateApplicationAndRequestToStart(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // CreateApplicationAndRequestToStartForQuest
				CreateApplicationAndRequestToStartForQuest(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // EnsureSaveData
				var _return = EnsureSaveData(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetDesiredLanguage
				GetDesiredLanguage(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // SetTerminateResult
				SetTerminateResult(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // GetDisplayVersion
				GetDisplayVersion(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // GetLaunchStorageInfoForDebug
				GetLaunchStorageInfoForDebug(out var _0, out var _1);
				om.Initialize(0, 0, 2);
				om.SetData(8, _0);
				om.SetData(9, _1);
				break;
			}
			case 0x19: { // ExtendSaveData
				var _return = ExtendSaveData(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32), im.GetData<ulong>(40));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x1A: { // GetSaveDataSize
				GetSaveDataSize(im.GetData<byte>(8), im.GetBytes(16, 0x10), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x1E: { // BeginBlockingHomeButtonShortAndLongPressed
				BeginBlockingHomeButtonShortAndLongPressed(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // EndBlockingHomeButtonShortAndLongPressed
				EndBlockingHomeButtonShortAndLongPressed();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x20: { // BeginBlockingHomeButton
				BeginBlockingHomeButton(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // EndBlockingHomeButton
				EndBlockingHomeButton();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // NotifyRunning
				var _return = NotifyRunning();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x32: { // GetPseudoDeviceId
				GetPseudoDeviceId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // SetMediaPlaybackStateForApplication
				SetMediaPlaybackStateForApplication(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x41: { // IsGamePlayRecordingSupported
				var _return = IsGamePlayRecordingSupported();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x42: { // InitializeGamePlayRecording
				InitializeGamePlayRecording(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x43: { // SetGamePlayRecordingState
				SetGamePlayRecordingState(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x44: { // RequestFlushGamePlayingMovieForDebug
				RequestFlushGamePlayingMovieForDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x46: { // RequestToShutdown
				RequestToShutdown();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x47: { // RequestToReboot
				RequestToReboot();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x50: { // ExitAndRequestToShowThanksMessage
				ExitAndRequestToShowThanksMessage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5A: { // EnableApplicationCrashReport
				EnableApplicationCrashReport(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationFunctions");
		}
	}
}

public partial class IApplicationProxy : _IApplicationProxy_Base;
public abstract class _IApplicationProxy_Base : IpcInterface {
	protected virtual Nn.Am.Service.ICommonStateGetter GetCommonStateGetter() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetCommonStateGetter not implemented");
	protected virtual Nn.Am.Service.ISelfController GetSelfController() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetSelfController not implemented");
	protected virtual Nn.Am.Service.IWindowController GetWindowController() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetWindowController not implemented");
	protected virtual Nn.Am.Service.IAudioController GetAudioController() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetAudioController not implemented");
	protected virtual Nn.Am.Service.IDisplayController GetDisplayController() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetDisplayController not implemented");
	protected virtual Nn.Am.Service.IProcessWindingController GetProcessWindingController() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetProcessWindingController not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletCreator GetLibraryAppletCreator() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetLibraryAppletCreator not implemented");
	protected virtual Nn.Am.Service.IApplicationFunctions GetApplicationFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetApplicationFunctions not implemented");
	protected virtual Nn.Am.Service.IDebugFunctions GetDebugFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxy.GetDebugFunctions not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCommonStateGetter
				var _return = GetCommonStateGetter();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				var _return = GetSelfController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				var _return = GetWindowController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				var _return = GetAudioController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				var _return = GetDisplayController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				var _return = GetProcessWindingController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				var _return = GetLibraryAppletCreator();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // GetApplicationFunctions
				var _return = GetApplicationFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				var _return = GetDebugFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationProxy");
		}
	}
}

public partial class IApplicationProxyService : _IApplicationProxyService_Base {
	public readonly string ServiceName;
	public IApplicationProxyService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IApplicationProxyService_Base : IpcInterface {
	protected virtual Nn.Am.Service.IApplicationProxy OpenApplicationProxy(ulong _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxyService.OpenApplicationProxy not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenApplicationProxy
				var _return = OpenApplicationProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationProxyService");
		}
	}
}

public partial class IAudioController : _IAudioController_Base;
public abstract class _IAudioController_Base : IpcInterface {
	protected virtual void SetExpectedMasterVolume(float _0, float _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAudioController.SetExpectedMasterVolume");
	protected virtual float GetMainAppletExpectedMasterVolume() =>
		throw new NotImplementedException("Nn.Am.Service.IAudioController.GetMainAppletExpectedMasterVolume not implemented");
	protected virtual float GetLibraryAppletExpectedMasterVolume() =>
		throw new NotImplementedException("Nn.Am.Service.IAudioController.GetLibraryAppletExpectedMasterVolume not implemented");
	protected virtual void ChangeMainAppletMasterVolume(float _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAudioController.ChangeMainAppletMasterVolume");
	protected virtual void SetTransparentVolumeRate(float _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IAudioController.SetTransparentVolumeRate");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetExpectedMasterVolume
				SetExpectedMasterVolume(im.GetData<float>(8), im.GetData<float>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetMainAppletExpectedMasterVolume
				var _return = GetMainAppletExpectedMasterVolume();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetLibraryAppletExpectedMasterVolume
				var _return = GetLibraryAppletExpectedMasterVolume();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // ChangeMainAppletMasterVolume
				ChangeMainAppletMasterVolume(im.GetData<float>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // SetTransparentVolumeRate
				SetTransparentVolumeRate(im.GetData<float>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IAudioController");
		}
	}
}

public partial class ICommonStateGetter : _ICommonStateGetter_Base;
public abstract class _ICommonStateGetter_Base : IpcInterface {
	protected virtual KObject GetEventHandle() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetEventHandle not implemented");
	protected virtual uint ReceiveMessage() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.ReceiveMessage not implemented");
	protected virtual void GetThisAppletKind(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetThisAppletKind not implemented");
	protected virtual void AllowToEnterSleep() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.AllowToEnterSleep");
	protected virtual void DisallowToEnterSleep() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.DisallowToEnterSleep");
	protected virtual byte GetOperationMode() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetOperationMode not implemented");
	protected virtual uint GetPerformanceMode() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetPerformanceMode not implemented");
	protected virtual byte GetCradleStatus() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetCradleStatus not implemented");
	protected virtual byte GetBootMode() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetBootMode not implemented");
	protected virtual byte GetCurrentFocusState() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetCurrentFocusState not implemented");
	protected virtual void RequestToAcquireSleepLock() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.RequestToAcquireSleepLock");
	protected virtual void ReleaseSleepLock() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.ReleaseSleepLock");
	protected virtual void ReleaseSleepLockTransiently() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.ReleaseSleepLockTransiently");
	protected virtual KObject GetAcquiredSleepLockEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetAcquiredSleepLockEvent not implemented");
	protected virtual void PushToGeneralChannel(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.PushToGeneralChannel");
	protected virtual Nn.Am.Service.ILockAccessor GetHomeButtonReaderLockAccessor() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetHomeButtonReaderLockAccessor not implemented");
	protected virtual Nn.Am.Service.ILockAccessor GetReaderLockAccessorEx(uint _0) =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetReaderLockAccessorEx not implemented");
	protected virtual void GetCradleFwVersion(out uint _0, out uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetCradleFwVersion not implemented");
	protected virtual byte IsVrModeEnabled() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.IsVrModeEnabled not implemented");
	protected virtual void SetVrModeEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.SetVrModeEnabled");
	protected virtual void SetLcdBacklighOffEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ICommonStateGetter.SetLcdBacklighOffEnabled");
	protected virtual byte IsInControllerFirmwareUpdateSection() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.IsInControllerFirmwareUpdateSection not implemented");
	protected virtual void GetDefaultDisplayResolution(out uint _0, out uint _1) =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetDefaultDisplayResolution not implemented");
	protected virtual KObject GetDefaultDisplayResolutionChangeEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetDefaultDisplayResolutionChangeEvent not implemented");
	protected virtual uint GetHdcpAuthenticationState() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetHdcpAuthenticationState not implemented");
	protected virtual KObject GetHdcpAuthenticationStateChangeEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ICommonStateGetter.GetHdcpAuthenticationStateChangeEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetEventHandle
				var _return = GetEventHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // ReceiveMessage
				var _return = ReceiveMessage();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetThisAppletKind
				GetThisAppletKind(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // AllowToEnterSleep
				AllowToEnterSleep();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // DisallowToEnterSleep
				DisallowToEnterSleep();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetOperationMode
				var _return = GetOperationMode();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // GetPerformanceMode
				var _return = GetPerformanceMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // GetCradleStatus
				var _return = GetCradleStatus();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // GetBootMode
				var _return = GetBootMode();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // GetCurrentFocusState
				var _return = GetCurrentFocusState();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // RequestToAcquireSleepLock
				RequestToAcquireSleepLock();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // ReleaseSleepLock
				ReleaseSleepLock();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // ReleaseSleepLockTransiently
				ReleaseSleepLockTransiently();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // GetAcquiredSleepLockEvent
				var _return = GetAcquiredSleepLockEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x14: { // PushToGeneralChannel
				PushToGeneralChannel(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // GetHomeButtonReaderLockAccessor
				var _return = GetHomeButtonReaderLockAccessor();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F: { // GetReaderLockAccessorEx
				var _return = GetReaderLockAccessorEx(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x28: { // GetCradleFwVersion
				GetCradleFwVersion(out var _0, out var _1, out var _2, out var _3);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				break;
			}
			case 0x32: { // IsVrModeEnabled
				var _return = IsVrModeEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // SetVrModeEnabled
				SetVrModeEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x34: { // SetLcdBacklighOffEnabled
				SetLcdBacklighOffEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x37: { // IsInControllerFirmwareUpdateSection
				var _return = IsInControllerFirmwareUpdateSection();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3C: { // GetDefaultDisplayResolution
				GetDefaultDisplayResolution(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3D: { // GetDefaultDisplayResolutionChangeEvent
				var _return = GetDefaultDisplayResolutionChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3E: { // GetHdcpAuthenticationState
				var _return = GetHdcpAuthenticationState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3F: { // GetHdcpAuthenticationStateChangeEvent
				var _return = GetHdcpAuthenticationStateChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ICommonStateGetter");
		}
	}
}

public partial class IDebugFunctions : _IDebugFunctions_Base;
public abstract class _IDebugFunctions_Base : IpcInterface {
	protected virtual void NotifyMessageToHomeMenuForDebug(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDebugFunctions.NotifyMessageToHomeMenuForDebug");
	protected virtual Nn.Am.Service.IApplicationAccessor OpenMainApplication() =>
		throw new NotImplementedException("Nn.Am.Service.IDebugFunctions.OpenMainApplication not implemented");
	protected virtual void EmulateButtonEvent(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDebugFunctions.EmulateButtonEvent");
	protected virtual void InvalidateTransitionLayer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDebugFunctions.InvalidateTransitionLayer");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // NotifyMessageToHomeMenuForDebug
				NotifyMessageToHomeMenuForDebug(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // OpenMainApplication
				var _return = OpenMainApplication();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // EmulateButtonEvent
				EmulateButtonEvent(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // InvalidateTransitionLayer
				InvalidateTransitionLayer();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IDebugFunctions");
		}
	}
}

public partial class IDisplayController : _IDisplayController_Base;
public abstract class _IDisplayController_Base : IpcInterface {
	protected virtual void GetLastForegroundCaptureImage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.GetLastForegroundCaptureImage not implemented");
	protected virtual void UpdateLastForegroundCaptureImage() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.UpdateLastForegroundCaptureImage");
	protected virtual void GetLastApplicationCaptureImage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.GetLastApplicationCaptureImage not implemented");
	protected virtual void GetCallerAppletCaptureImage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.GetCallerAppletCaptureImage not implemented");
	protected virtual void UpdateCallerAppletCaptureImage() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.UpdateCallerAppletCaptureImage");
	protected virtual void GetLastForegroundCaptureImageEx(out byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.GetLastForegroundCaptureImageEx not implemented");
	protected virtual void GetLastApplicationCaptureImageEx(out byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.GetLastApplicationCaptureImageEx not implemented");
	protected virtual void GetCallerAppletCaptureImageEx(out byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.GetCallerAppletCaptureImageEx not implemented");
	protected virtual void TakeScreenShotOfOwnLayer(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.TakeScreenShotOfOwnLayer");
	protected virtual KObject AcquireLastApplicationCaptureBuffer() =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireLastApplicationCaptureBuffer not implemented");
	protected virtual void ReleaseLastApplicationCaptureBuffer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ReleaseLastApplicationCaptureBuffer");
	protected virtual KObject AcquireLastForegroundCaptureBuffer() =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireLastForegroundCaptureBuffer not implemented");
	protected virtual void ReleaseLastForegroundCaptureBuffer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ReleaseLastForegroundCaptureBuffer");
	protected virtual KObject AcquireCallerAppletCaptureBuffer() =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireCallerAppletCaptureBuffer not implemented");
	protected virtual void ReleaseCallerAppletCaptureBuffer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ReleaseCallerAppletCaptureBuffer");
	protected virtual void AcquireLastApplicationCaptureBufferEx(out byte _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireLastApplicationCaptureBufferEx not implemented");
	protected virtual void AcquireLastForegroundCaptureBufferEx(out byte _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireLastForegroundCaptureBufferEx not implemented");
	protected virtual void AcquireCallerAppletCaptureBufferEx(out byte _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireCallerAppletCaptureBufferEx not implemented");
	protected virtual void ClearCaptureBuffer(byte _0, uint _1, uint _2) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ClearCaptureBuffer");
	protected virtual void ClearAppletTransitionBuffer(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ClearAppletTransitionBuffer");
	protected virtual void AcquireLastApplicationCaptureSharedBuffer(out byte _0, out uint _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireLastApplicationCaptureSharedBuffer not implemented");
	protected virtual void ReleaseLastApplicationCaptureSharedBuffer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ReleaseLastApplicationCaptureSharedBuffer");
	protected virtual void AcquireLastForegroundCaptureSharedBuffer(out byte _0, out uint _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireLastForegroundCaptureSharedBuffer not implemented");
	protected virtual void ReleaseLastForegroundCaptureSharedBuffer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ReleaseLastForegroundCaptureSharedBuffer");
	protected virtual void AcquireCallerAppletCaptureSharedBuffer(out byte _0, out uint _1) =>
		throw new NotImplementedException("Nn.Am.Service.IDisplayController.AcquireCallerAppletCaptureSharedBuffer not implemented");
	protected virtual void ReleaseCallerAppletCaptureSharedBuffer() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IDisplayController.ReleaseCallerAppletCaptureSharedBuffer");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetLastForegroundCaptureImage
				GetLastForegroundCaptureImage(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // UpdateLastForegroundCaptureImage
				UpdateLastForegroundCaptureImage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetLastApplicationCaptureImage
				GetLastApplicationCaptureImage(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetCallerAppletCaptureImage
				GetCallerAppletCaptureImage(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // UpdateCallerAppletCaptureImage
				UpdateCallerAppletCaptureImage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetLastForegroundCaptureImageEx
				GetLastForegroundCaptureImageEx(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // GetLastApplicationCaptureImageEx
				GetLastApplicationCaptureImageEx(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _0);
				break;
			}
			case 0x7: { // GetCallerAppletCaptureImageEx
				GetCallerAppletCaptureImageEx(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _0);
				break;
			}
			case 0x8: { // TakeScreenShotOfOwnLayer
				TakeScreenShotOfOwnLayer(im.GetData<byte>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // AcquireLastApplicationCaptureBuffer
				var _return = AcquireLastApplicationCaptureBuffer();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xB: { // ReleaseLastApplicationCaptureBuffer
				ReleaseLastApplicationCaptureBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // AcquireLastForegroundCaptureBuffer
				var _return = AcquireLastForegroundCaptureBuffer();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xD: { // ReleaseLastForegroundCaptureBuffer
				ReleaseLastForegroundCaptureBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // AcquireCallerAppletCaptureBuffer
				var _return = AcquireCallerAppletCaptureBuffer();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xF: { // ReleaseCallerAppletCaptureBuffer
				ReleaseCallerAppletCaptureBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // AcquireLastApplicationCaptureBufferEx
				AcquireLastApplicationCaptureBufferEx(out var _0, out var _1);
				om.Initialize(0, 1, 1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x11: { // AcquireLastForegroundCaptureBufferEx
				AcquireLastForegroundCaptureBufferEx(out var _0, out var _1);
				om.Initialize(0, 1, 1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x12: { // AcquireCallerAppletCaptureBufferEx
				AcquireCallerAppletCaptureBufferEx(out var _0, out var _1);
				om.Initialize(0, 1, 1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x14: { // ClearCaptureBuffer
				ClearCaptureBuffer(im.GetData<byte>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // ClearAppletTransitionBuffer
				ClearAppletTransitionBuffer(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // AcquireLastApplicationCaptureSharedBuffer
				AcquireLastApplicationCaptureSharedBuffer(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x17: { // ReleaseLastApplicationCaptureSharedBuffer
				ReleaseLastApplicationCaptureSharedBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // AcquireLastForegroundCaptureSharedBuffer
				AcquireLastForegroundCaptureSharedBuffer(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x19: { // ReleaseLastForegroundCaptureSharedBuffer
				ReleaseLastForegroundCaptureSharedBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // AcquireCallerAppletCaptureSharedBuffer
				AcquireCallerAppletCaptureSharedBuffer(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1B: { // ReleaseCallerAppletCaptureSharedBuffer
				ReleaseCallerAppletCaptureSharedBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IDisplayController");
		}
	}
}

public partial class IGlobalStateController : _IGlobalStateController_Base;
public abstract class _IGlobalStateController_Base : IpcInterface {
	protected virtual void RequestToEnterSleep() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.RequestToEnterSleep");
	protected virtual void EnterSleep() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.EnterSleep");
	protected virtual void StartSleepSequence(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.StartSleepSequence");
	protected virtual void StartShutdownSequence() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.StartShutdownSequence");
	protected virtual void StartRebootSequence() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.StartRebootSequence");
	protected virtual void LoadAndApplyIdlePolicySettings() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.LoadAndApplyIdlePolicySettings");
	protected virtual void NotifyCecSettingsChanged() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.NotifyCecSettingsChanged");
	protected virtual void SetDefaultHomeButtonLongPressTime(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.SetDefaultHomeButtonLongPressTime");
	protected virtual void UpdateDefaultDisplayResolution() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IGlobalStateController.UpdateDefaultDisplayResolution");
	protected virtual byte ShouldSleepOnBoot() =>
		throw new NotImplementedException("Nn.Am.Service.IGlobalStateController.ShouldSleepOnBoot not implemented");
	protected virtual KObject GetHdcpAuthenticationFailedEvent() =>
		throw new NotImplementedException("Nn.Am.Service.IGlobalStateController.GetHdcpAuthenticationFailedEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestToEnterSleep
				RequestToEnterSleep();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // EnterSleep
				EnterSleep();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // StartSleepSequence
				StartSleepSequence(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // StartShutdownSequence
				StartShutdownSequence();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // StartRebootSequence
				StartRebootSequence();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // LoadAndApplyIdlePolicySettings
				LoadAndApplyIdlePolicySettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // NotifyCecSettingsChanged
				NotifyCecSettingsChanged();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // SetDefaultHomeButtonLongPressTime
				SetDefaultHomeButtonLongPressTime(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // UpdateDefaultDisplayResolution
				UpdateDefaultDisplayResolution();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // ShouldSleepOnBoot
				var _return = ShouldSleepOnBoot();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xF: { // GetHdcpAuthenticationFailedEvent
				var _return = GetHdcpAuthenticationFailedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IGlobalStateController");
		}
	}
}

public partial class IHomeMenuFunctions : _IHomeMenuFunctions_Base;
public abstract class _IHomeMenuFunctions_Base : IpcInterface {
	protected virtual void RequestToGetForeground() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IHomeMenuFunctions.RequestToGetForeground");
	protected virtual void LockForeground() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IHomeMenuFunctions.LockForeground");
	protected virtual void UnlockForeground() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IHomeMenuFunctions.UnlockForeground");
	protected virtual Nn.Am.Service.IStorage PopFromGeneralChannel() =>
		throw new NotImplementedException("Nn.Am.Service.IHomeMenuFunctions.PopFromGeneralChannel not implemented");
	protected virtual KObject GetPopFromGeneralChannelEvent() =>
		throw new NotImplementedException("Nn.Am.Service.IHomeMenuFunctions.GetPopFromGeneralChannelEvent not implemented");
	protected virtual Nn.Am.Service.ILockAccessor GetHomeButtonWriterLockAccessor() =>
		throw new NotImplementedException("Nn.Am.Service.IHomeMenuFunctions.GetHomeButtonWriterLockAccessor not implemented");
	protected virtual Nn.Am.Service.ILockAccessor GetWriterLockAccessorEx(uint _0) =>
		throw new NotImplementedException("Nn.Am.Service.IHomeMenuFunctions.GetWriterLockAccessorEx not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: { // RequestToGetForeground
				RequestToGetForeground();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // LockForeground
				LockForeground();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // UnlockForeground
				UnlockForeground();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // PopFromGeneralChannel
				var _return = PopFromGeneralChannel();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15: { // GetPopFromGeneralChannelEvent
				var _return = GetPopFromGeneralChannelEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1E: { // GetHomeButtonWriterLockAccessor
				var _return = GetHomeButtonWriterLockAccessor();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F: { // GetWriterLockAccessorEx
				var _return = GetWriterLockAccessorEx(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IHomeMenuFunctions");
		}
	}
}

public partial class ILibraryAppletAccessor : _ILibraryAppletAccessor_Base;
public abstract class _ILibraryAppletAccessor_Base : IpcInterface {
	protected virtual KObject GetAppletStateChangedEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.GetAppletStateChangedEvent not implemented");
	protected virtual byte IsCompleted() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.IsCompleted not implemented");
	protected virtual void Start() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.Start");
	protected virtual void RequestExit() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.RequestExit");
	protected virtual void Terminate() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.Terminate");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.GetResult");
	protected virtual void SetOutOfFocusApplicationSuspendingEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.SetOutOfFocusApplicationSuspendingEnabled");
	protected virtual void PushInData(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.PushInData");
	protected virtual Nn.Am.Service.IStorage PopOutData() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.PopOutData not implemented");
	protected virtual void PushExtraStorage(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.PushExtraStorage");
	protected virtual void PushInteractiveInData(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.PushInteractiveInData");
	protected virtual Nn.Am.Service.IStorage PopInteractiveOutData() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.PopInteractiveOutData not implemented");
	protected virtual KObject GetPopOutDataEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.GetPopOutDataEvent not implemented");
	protected virtual KObject GetPopInteractiveOutDataEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.GetPopInteractiveOutDataEvent not implemented");
	protected virtual byte NeedsToExitProcess() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.NeedsToExitProcess not implemented");
	protected virtual void GetLibraryAppletInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.GetLibraryAppletInfo not implemented");
	protected virtual void RequestForAppletToGetForeground() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletAccessor.RequestForAppletToGetForeground");
	protected virtual ulong GetIndirectLayerConsumerHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletAccessor.GetIndirectLayerConsumerHandle not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAppletStateChangedEvent
				var _return = GetAppletStateChangedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // IsCompleted
				var _return = IsCompleted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Start
				Start();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // RequestExit
				RequestExit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Terminate
				Terminate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x32: { // SetOutOfFocusApplicationSuspendingEnabled
				SetOutOfFocusApplicationSuspendingEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // PushInData
				PushInData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // PopOutData
				var _return = PopOutData();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // PushExtraStorage
				PushExtraStorage(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // PushInteractiveInData
				PushInteractiveInData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x68: { // PopInteractiveOutData
				var _return = PopInteractiveOutData();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x69: { // GetPopOutDataEvent
				var _return = GetPopOutDataEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6A: { // GetPopInteractiveOutDataEvent
				var _return = GetPopInteractiveOutDataEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6E: { // NeedsToExitProcess
				var _return = NeedsToExitProcess();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x78: { // GetLibraryAppletInfo
				GetLibraryAppletInfo(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x96: { // RequestForAppletToGetForeground
				RequestForAppletToGetForeground();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA0: { // GetIndirectLayerConsumerHandle
				var _return = GetIndirectLayerConsumerHandle(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletAccessor");
		}
	}
}

public partial class ILibraryAppletCreator : _ILibraryAppletCreator_Base;
public abstract class _ILibraryAppletCreator_Base : IpcInterface {
	protected virtual Nn.Am.Service.ILibraryAppletAccessor CreateLibraryApplet(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletCreator.CreateLibraryApplet not implemented");
	protected virtual void TerminateAllLibraryApplets() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletCreator.TerminateAllLibraryApplets");
	protected virtual byte AreAnyLibraryAppletsLeft() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletCreator.AreAnyLibraryAppletsLeft not implemented");
	protected virtual Nn.Am.Service.IStorage CreateStorage(ulong _0) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletCreator.CreateStorage not implemented");
	protected virtual Nn.Am.Service.IStorage CreateTransferMemoryStorage(byte _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletCreator.CreateTransferMemoryStorage not implemented");
	protected virtual Nn.Am.Service.IStorage CreateHandleStorage(ulong _0, KObject _1) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletCreator.CreateHandleStorage not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateLibraryApplet
				var _return = CreateLibraryApplet(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // TerminateAllLibraryApplets
				TerminateAllLibraryApplets();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // AreAnyLibraryAppletsLeft
				var _return = AreAnyLibraryAppletsLeft();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // CreateStorage
				var _return = CreateStorage(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // CreateTransferMemoryStorage
				var _return = CreateTransferMemoryStorage(im.GetData<byte>(8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC: { // CreateHandleStorage
				var _return = CreateHandleStorage(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletCreator");
		}
	}
}

public partial class ILibraryAppletProxy : _ILibraryAppletProxy_Base;
public abstract class _ILibraryAppletProxy_Base : IpcInterface {
	protected virtual Nn.Am.Service.ICommonStateGetter GetCommonStateGetter() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetCommonStateGetter not implemented");
	protected virtual Nn.Am.Service.ISelfController GetSelfController() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetSelfController not implemented");
	protected virtual Nn.Am.Service.IWindowController GetWindowController() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetWindowController not implemented");
	protected virtual Nn.Am.Service.IAudioController GetAudioController() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetAudioController not implemented");
	protected virtual Nn.Am.Service.IDisplayController GetDisplayController() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetDisplayController not implemented");
	protected virtual Nn.Am.Service.IProcessWindingController GetProcessWindingController() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetProcessWindingController not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletCreator GetLibraryAppletCreator() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetLibraryAppletCreator not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletSelfAccessor OpenLibraryAppletSelfAccessor() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.OpenLibraryAppletSelfAccessor not implemented");
	protected virtual Nn.Am.Service.IDebugFunctions GetDebugFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletProxy.GetDebugFunctions not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCommonStateGetter
				var _return = GetCommonStateGetter();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				var _return = GetSelfController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				var _return = GetWindowController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				var _return = GetAudioController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				var _return = GetDisplayController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				var _return = GetProcessWindingController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				var _return = GetLibraryAppletCreator();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // OpenLibraryAppletSelfAccessor
				var _return = OpenLibraryAppletSelfAccessor();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				var _return = GetDebugFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletProxy");
		}
	}
}

public partial class ILibraryAppletSelfAccessor : _ILibraryAppletSelfAccessor_Base;
public abstract class _ILibraryAppletSelfAccessor_Base : IpcInterface {
	protected virtual Nn.Am.Service.IStorage PopInData() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.PopInData not implemented");
	protected virtual void PushOutData(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.PushOutData");
	protected virtual Nn.Am.Service.IStorage PopInteractiveInData() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.PopInteractiveInData not implemented");
	protected virtual void PushInteractiveOutData(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.PushInteractiveOutData");
	protected virtual KObject GetPopInDataEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetPopInDataEvent not implemented");
	protected virtual KObject GetPopInteractiveInDataEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetPopInteractiveInDataEvent not implemented");
	protected virtual void ExitProcessAndReturn() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.ExitProcessAndReturn");
	protected virtual void GetLibraryAppletInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetLibraryAppletInfo not implemented");
	protected virtual void GetMainAppletIdentityInfo() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.GetMainAppletIdentityInfo");
	protected virtual byte CanUseApplicationCore() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.CanUseApplicationCore not implemented");
	protected virtual void GetCallerAppletIdentityInfo() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.GetCallerAppletIdentityInfo");
	protected virtual void GetMainAppletApplicationControlProperty(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetMainAppletApplicationControlProperty not implemented");
	protected virtual byte GetMainAppletStorageId() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetMainAppletStorageId not implemented");
	protected virtual void GetCallerAppletIdentityInfoStack(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetCallerAppletIdentityInfoStack not implemented");
	protected virtual void GetNextReturnDestinationAppletIdentityInfo() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.GetNextReturnDestinationAppletIdentityInfo");
	protected virtual uint GetDesirableKeyboardLayout() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetDesirableKeyboardLayout not implemented");
	protected virtual Nn.Am.Service.IStorage PopExtraStorage() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.PopExtraStorage not implemented");
	protected virtual KObject GetPopExtraStorageEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetPopExtraStorageEvent not implemented");
	protected virtual void UnpopInData(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.UnpopInData");
	protected virtual void UnpopExtraStorage(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.UnpopExtraStorage");
	protected virtual ulong GetIndirectLayerProducerHandle() =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetIndirectLayerProducerHandle not implemented");
	protected virtual void ReportVisibleError(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.ReportVisibleError");
	protected virtual void ReportVisibleErrorWithErrorContext(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILibraryAppletSelfAccessor.ReportVisibleErrorWithErrorContext");
	protected virtual void GetMainAppletApplicationDesiredLanguage(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.GetMainAppletApplicationDesiredLanguage not implemented");
	protected virtual Nn.Grcsrv.IGameMovieTrimmer CreateGameMovieTrimmer(ulong _0, KObject _1) =>
		throw new NotImplementedException("Nn.Am.Service.ILibraryAppletSelfAccessor.CreateGameMovieTrimmer not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PopInData
				var _return = PopInData();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // PushOutData
				PushOutData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // PopInteractiveInData
				var _return = PopInteractiveInData();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // PushInteractiveOutData
				PushInteractiveOutData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetPopInDataEvent
				var _return = GetPopInDataEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6: { // GetPopInteractiveInDataEvent
				var _return = GetPopInteractiveInDataEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // ExitProcessAndReturn
				ExitProcessAndReturn();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // GetLibraryAppletInfo
				GetLibraryAppletInfo(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetMainAppletIdentityInfo
				GetMainAppletIdentityInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // CanUseApplicationCore
				var _return = CanUseApplicationCore();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // GetCallerAppletIdentityInfo
				GetCallerAppletIdentityInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetMainAppletApplicationControlProperty
				GetMainAppletApplicationControlProperty(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetMainAppletStorageId
				var _return = GetMainAppletStorageId();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x11: { // GetCallerAppletIdentityInfoStack
				GetCallerAppletIdentityInfoStack(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x12: { // GetNextReturnDestinationAppletIdentityInfo
				GetNextReturnDestinationAppletIdentityInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // GetDesirableKeyboardLayout
				var _return = GetDesirableKeyboardLayout();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x14: { // PopExtraStorage
				var _return = PopExtraStorage();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x19: { // GetPopExtraStorageEvent
				var _return = GetPopExtraStorageEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1E: { // UnpopInData
				UnpopInData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // UnpopExtraStorage
				UnpopExtraStorage(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // GetIndirectLayerProducerHandle
				var _return = GetIndirectLayerProducerHandle();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x32: { // ReportVisibleError
				ReportVisibleError(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x33: { // ReportVisibleErrorWithErrorContext
				ReportVisibleErrorWithErrorContext(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3C: { // GetMainAppletApplicationDesiredLanguage
				GetMainAppletApplicationDesiredLanguage(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // CreateGameMovieTrimmer
				var _return = CreateGameMovieTrimmer(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILibraryAppletSelfAccessor");
		}
	}
}

public partial class ILockAccessor : _ILockAccessor_Base;
public abstract class _ILockAccessor_Base : IpcInterface {
	protected virtual void TryLock(byte _0, out byte _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.ILockAccessor.TryLock not implemented");
	protected virtual void Unlock() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ILockAccessor.Unlock");
	protected virtual KObject GetEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ILockAccessor.GetEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // TryLock
				TryLock(im.GetData<byte>(8), out var _0, out var _1);
				om.Initialize(0, 1, 1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x2: { // Unlock
				Unlock();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetEvent
				var _return = GetEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ILockAccessor");
		}
	}
}

public partial class IOverlayAppletProxy : _IOverlayAppletProxy_Base;
public abstract class _IOverlayAppletProxy_Base : IpcInterface {
	protected virtual Nn.Am.Service.ICommonStateGetter GetCommonStateGetter() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetCommonStateGetter not implemented");
	protected virtual Nn.Am.Service.ISelfController GetSelfController() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetSelfController not implemented");
	protected virtual Nn.Am.Service.IWindowController GetWindowController() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetWindowController not implemented");
	protected virtual Nn.Am.Service.IAudioController GetAudioController() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetAudioController not implemented");
	protected virtual Nn.Am.Service.IDisplayController GetDisplayController() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetDisplayController not implemented");
	protected virtual Nn.Am.Service.IProcessWindingController GetProcessWindingController() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetProcessWindingController not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletCreator GetLibraryAppletCreator() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetLibraryAppletCreator not implemented");
	protected virtual Nn.Am.Service.IOverlayFunctions GetOverlayFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetOverlayFunctions not implemented");
	protected virtual Nn.Am.Service.IDebugFunctions GetDebugFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayAppletProxy.GetDebugFunctions not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCommonStateGetter
				var _return = GetCommonStateGetter();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				var _return = GetSelfController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				var _return = GetWindowController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				var _return = GetAudioController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				var _return = GetDisplayController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				var _return = GetProcessWindingController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				var _return = GetLibraryAppletCreator();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // GetOverlayFunctions
				var _return = GetOverlayFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				var _return = GetDebugFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IOverlayAppletProxy");
		}
	}
}

public partial class IOverlayFunctions : _IOverlayFunctions_Base;
public abstract class _IOverlayFunctions_Base : IpcInterface {
	protected virtual void BeginToWatchShortHomeButtonMessage() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IOverlayFunctions.BeginToWatchShortHomeButtonMessage");
	protected virtual void EndToWatchShortHomeButtonMessage() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IOverlayFunctions.EndToWatchShortHomeButtonMessage");
	protected virtual ulong GetApplicationIdForLogo() =>
		throw new NotImplementedException("Nn.Am.Service.IOverlayFunctions.GetApplicationIdForLogo not implemented");
	protected virtual void SetGpuTimeSliceBoost(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IOverlayFunctions.SetGpuTimeSliceBoost");
	protected virtual void SetAutoSleepTimeAndDimmingTimeEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IOverlayFunctions.SetAutoSleepTimeAndDimmingTimeEnabled");
	protected virtual void TerminateApplicationAndSetReason(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IOverlayFunctions.TerminateApplicationAndSetReason");
	protected virtual void SetScreenShotPermissionGlobally(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IOverlayFunctions.SetScreenShotPermissionGlobally");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BeginToWatchShortHomeButtonMessage
				BeginToWatchShortHomeButtonMessage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // EndToWatchShortHomeButtonMessage
				EndToWatchShortHomeButtonMessage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetApplicationIdForLogo
				var _return = GetApplicationIdForLogo();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetGpuTimeSliceBoost
				SetGpuTimeSliceBoost(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // SetAutoSleepTimeAndDimmingTimeEnabled
				SetAutoSleepTimeAndDimmingTimeEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // TerminateApplicationAndSetReason
				TerminateApplicationAndSetReason(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // SetScreenShotPermissionGlobally
				SetScreenShotPermissionGlobally(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IOverlayFunctions");
		}
	}
}

public partial class IProcessWindingController : _IProcessWindingController_Base;
public abstract class _IProcessWindingController_Base : IpcInterface {
	protected virtual void GetLaunchReason(out byte[] _0) =>
		throw new NotImplementedException("Nn.Am.Service.IProcessWindingController.GetLaunchReason not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletAccessor OpenCallingLibraryApplet() =>
		throw new NotImplementedException("Nn.Am.Service.IProcessWindingController.OpenCallingLibraryApplet not implemented");
	protected virtual void PushContext(Nn.Am.Service.IStorage _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IProcessWindingController.PushContext");
	protected virtual Nn.Am.Service.IStorage PopContext() =>
		throw new NotImplementedException("Nn.Am.Service.IProcessWindingController.PopContext not implemented");
	protected virtual void CancelWindingReservation() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IProcessWindingController.CancelWindingReservation");
	protected virtual void WindAndDoReserved() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IProcessWindingController.WindAndDoReserved");
	protected virtual void ReserveToStartAndWaitAndUnwindThis(Nn.Am.Service.ILibraryAppletAccessor _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IProcessWindingController.ReserveToStartAndWaitAndUnwindThis");
	protected virtual void ReserveToStartAndWait(Nn.Am.Service.ILibraryAppletAccessor _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IProcessWindingController.ReserveToStartAndWait");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetLaunchReason
				GetLaunchReason(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // OpenCallingLibraryApplet
				var _return = OpenCallingLibraryApplet();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15: { // PushContext
				PushContext(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // PopContext
				var _return = PopContext();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x17: { // CancelWindingReservation
				CancelWindingReservation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // WindAndDoReserved
				WindAndDoReserved();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // ReserveToStartAndWaitAndUnwindThis
				ReserveToStartAndWaitAndUnwindThis(Kernel.Get<Nn.Am.Service.ILibraryAppletAccessor>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x29: { // ReserveToStartAndWait
				ReserveToStartAndWait(Kernel.Get<Nn.Am.Service.ILibraryAppletAccessor>(im.GetMove(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IProcessWindingController");
		}
	}
}

public partial class ISelfController : _ISelfController_Base;
public abstract class _ISelfController_Base : IpcInterface {
	protected virtual void Exit() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.Exit");
	protected virtual void LockExit() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.LockExit");
	protected virtual void UnlockExit() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.UnlockExit");
	protected virtual void EnterFatalSection() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.EnterFatalSection");
	protected virtual void LeaveFatalSection() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.LeaveFatalSection");
	protected virtual KObject GetLibraryAppletLaunchableEvent() =>
		throw new NotImplementedException("Nn.Am.Service.ISelfController.GetLibraryAppletLaunchableEvent not implemented");
	protected virtual void SetScreenShotPermission(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetScreenShotPermission");
	protected virtual void SetOperationModeChangedNotification(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetOperationModeChangedNotification");
	protected virtual void SetPerformanceModeChangedNotification(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetPerformanceModeChangedNotification");
	protected virtual void SetFocusHandlingMode(byte _0, byte _1, byte _2) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetFocusHandlingMode");
	protected virtual void SetRestartMessageEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetRestartMessageEnabled");
	protected virtual void SetScreenShotAppletIdentityInfo() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetScreenShotAppletIdentityInfo");
	protected virtual void SetOutOfFocusSuspendingEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetOutOfFocusSuspendingEnabled");
	protected virtual void SetControllerFirmwareUpdateSection(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetControllerFirmwareUpdateSection");
	protected virtual void SetRequiresCaptureButtonShortPressedMessage(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetRequiresCaptureButtonShortPressedMessage");
	protected virtual void SetScreenShotImageOrientation(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetScreenShotImageOrientation");
	protected virtual void SetDesirableKeyboardLayout(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetDesirableKeyboardLayout");
	protected virtual ulong CreateManagedDisplayLayer() =>
		throw new NotImplementedException("Nn.Am.Service.ISelfController.CreateManagedDisplayLayer not implemented");
	protected virtual void IsSystemBufferSharingEnabled() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.IsSystemBufferSharingEnabled");
	protected virtual void GetSystemSharedLayerHandle(out ulong _0, out ulong _1) =>
		throw new NotImplementedException("Nn.Am.Service.ISelfController.GetSystemSharedLayerHandle not implemented");
	protected virtual void SetHandlesRequestToDisplay(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetHandlesRequestToDisplay");
	protected virtual void ApproveToDisplay() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.ApproveToDisplay");
	protected virtual void OverrideAutoSleepTimeAndDimmingTime(uint _0, uint _1, uint _2, uint _3) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.OverrideAutoSleepTimeAndDimmingTime");
	protected virtual void SetMediaPlaybackState(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetMediaPlaybackState");
	protected virtual void SetIdleTimeDetectionExtension(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetIdleTimeDetectionExtension");
	protected virtual uint GetIdleTimeDetectionExtension() =>
		throw new NotImplementedException("Nn.Am.Service.ISelfController.GetIdleTimeDetectionExtension not implemented");
	protected virtual void SetInputDetectionSourceSet(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetInputDetectionSourceSet");
	protected virtual void ReportUserIsActive() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.ReportUserIsActive");
	protected virtual float GetCurrentIlluminance() =>
		throw new NotImplementedException("Nn.Am.Service.ISelfController.GetCurrentIlluminance not implemented");
	protected virtual byte IsIlluminanceAvailable() =>
		throw new NotImplementedException("Nn.Am.Service.ISelfController.IsIlluminanceAvailable not implemented");
	protected virtual void ReportMultimediaError(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.ReportMultimediaError");
	protected virtual void SetWirelessPriorityMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.ISelfController.SetWirelessPriorityMode");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Exit
				Exit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // LockExit
				LockExit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // UnlockExit
				UnlockExit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // EnterFatalSection
				EnterFatalSection();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // LeaveFatalSection
				LeaveFatalSection();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetLibraryAppletLaunchableEvent
				var _return = GetLibraryAppletLaunchableEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // SetScreenShotPermission
				SetScreenShotPermission(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // SetOperationModeChangedNotification
				SetOperationModeChangedNotification(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // SetPerformanceModeChangedNotification
				SetPerformanceModeChangedNotification(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // SetFocusHandlingMode
				SetFocusHandlingMode(im.GetData<byte>(8), im.GetData<byte>(9), im.GetData<byte>(10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // SetRestartMessageEnabled
				SetRestartMessageEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // SetScreenShotAppletIdentityInfo
				SetScreenShotAppletIdentityInfo();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // SetOutOfFocusSuspendingEnabled
				SetOutOfFocusSuspendingEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // SetControllerFirmwareUpdateSection
				SetControllerFirmwareUpdateSection(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // SetRequiresCaptureButtonShortPressedMessage
				SetRequiresCaptureButtonShortPressedMessage(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // SetScreenShotImageOrientation
				SetScreenShotImageOrientation(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // SetDesirableKeyboardLayout
				SetDesirableKeyboardLayout(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // CreateManagedDisplayLayer
				var _return = CreateManagedDisplayLayer();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x29: { // IsSystemBufferSharingEnabled
				IsSystemBufferSharingEnabled();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2A: { // GetSystemSharedLayerHandle
				GetSystemSharedLayerHandle(out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x32: { // SetHandlesRequestToDisplay
				SetHandlesRequestToDisplay(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x33: { // ApproveToDisplay
				ApproveToDisplay();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3C: { // OverrideAutoSleepTimeAndDimmingTime
				OverrideAutoSleepTimeAndDimmingTime(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3D: { // SetMediaPlaybackState
				SetMediaPlaybackState(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E: { // SetIdleTimeDetectionExtension
				SetIdleTimeDetectionExtension(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3F: { // GetIdleTimeDetectionExtension
				var _return = GetIdleTimeDetectionExtension();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x40: { // SetInputDetectionSourceSet
				SetInputDetectionSourceSet(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x41: { // ReportUserIsActive
				ReportUserIsActive();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x42: { // GetCurrentIlluminance
				var _return = GetCurrentIlluminance();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x43: { // IsIlluminanceAvailable
				var _return = IsIlluminanceAvailable();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x46: { // ReportMultimediaError
				ReportMultimediaError(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x50: { // SetWirelessPriorityMode
				SetWirelessPriorityMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ISelfController");
		}
	}
}

public partial class IStorage : _IStorage_Base;
public abstract class _IStorage_Base : IpcInterface {
	protected virtual Nn.Am.Service.IStorageAccessor Unknown0() =>
		throw new NotImplementedException("Nn.Am.Service.IStorage.Unknown0 not implemented");
	protected virtual Nn.Am.Service.ITransferStorageAccessor Unknown1() =>
		throw new NotImplementedException("Nn.Am.Service.IStorage.Unknown1 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // Unknown1
				var _return = Unknown1();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IStorage");
		}
	}
}

public partial class IStorageAccessor : _IStorageAccessor_Base;
public abstract class _IStorageAccessor_Base : IpcInterface {
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Am.Service.IStorageAccessor.GetSize not implemented");
	protected virtual void Write(ulong _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IStorageAccessor.Write");
	protected virtual void Read(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Am.Service.IStorageAccessor.Read not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSize
				var _return = GetSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Write
				Write(im.GetData<ulong>(8), im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Read
				Read(im.GetData<ulong>(8), im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IStorageAccessor");
		}
	}
}

public partial class ISystemAppletProxy : _ISystemAppletProxy_Base;
public abstract class _ISystemAppletProxy_Base : IpcInterface {
	protected virtual Nn.Am.Service.ICommonStateGetter GetCommonStateGetter() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetCommonStateGetter not implemented");
	protected virtual Nn.Am.Service.ISelfController GetSelfController() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetSelfController not implemented");
	protected virtual Nn.Am.Service.IWindowController GetWindowController() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetWindowController not implemented");
	protected virtual Nn.Am.Service.IAudioController GetAudioController() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetAudioController not implemented");
	protected virtual Nn.Am.Service.IDisplayController GetDisplayController() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetDisplayController not implemented");
	protected virtual Nn.Am.Service.IProcessWindingController GetProcessWindingController() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetProcessWindingController not implemented");
	protected virtual Nn.Am.Service.ILibraryAppletCreator GetLibraryAppletCreator() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetLibraryAppletCreator not implemented");
	protected virtual Nn.Am.Service.IHomeMenuFunctions GetHomeMenuFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetHomeMenuFunctions not implemented");
	protected virtual Nn.Am.Service.IGlobalStateController GetGlobalStateController() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetGlobalStateController not implemented");
	protected virtual Nn.Am.Service.IApplicationCreator GetApplicationCreator() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetApplicationCreator not implemented");
	protected virtual Nn.Am.Service.IDebugFunctions GetDebugFunctions() =>
		throw new NotImplementedException("Nn.Am.Service.ISystemAppletProxy.GetDebugFunctions not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCommonStateGetter
				var _return = GetCommonStateGetter();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				var _return = GetSelfController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				var _return = GetWindowController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				var _return = GetAudioController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				var _return = GetDisplayController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				var _return = GetProcessWindingController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				var _return = GetLibraryAppletCreator();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // GetHomeMenuFunctions
				var _return = GetHomeMenuFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15: { // GetGlobalStateController
				var _return = GetGlobalStateController();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x16: { // GetApplicationCreator
				var _return = GetApplicationCreator();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				var _return = GetDebugFunctions();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ISystemAppletProxy");
		}
	}
}

public partial class ITransferStorageAccessor : _ITransferStorageAccessor_Base;
public abstract class _ITransferStorageAccessor_Base : IpcInterface {
	protected virtual ulong GetSize() =>
		throw new NotImplementedException("Nn.Am.Service.ITransferStorageAccessor.GetSize not implemented");
	protected virtual void GetHandle(out ulong _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Am.Service.ITransferStorageAccessor.GetHandle not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSize
				var _return = GetSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetHandle
				GetHandle(out var _0, out var _1);
				om.Initialize(0, 1, 8);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.ITransferStorageAccessor");
		}
	}
}

public partial class IWindow : _IWindow_Base;
public abstract class _IWindow_Base : IpcInterface {
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IWindow");
		}
	}
}

public partial class IWindowController : _IWindowController_Base;
public abstract class _IWindowController_Base : IpcInterface {
	protected virtual Nn.Am.Service.IWindow CreateWindow(uint _0) =>
		throw new NotImplementedException("Nn.Am.Service.IWindowController.CreateWindow not implemented");
	protected virtual ulong GetAppletResourceUserId() =>
		throw new NotImplementedException("Nn.Am.Service.IWindowController.GetAppletResourceUserId not implemented");
	protected virtual void AcquireForegroundRights() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IWindowController.AcquireForegroundRights");
	protected virtual void ReleaseForegroundRights() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IWindowController.ReleaseForegroundRights");
	protected virtual void RejectToChangeIntoBackground() =>
		Console.WriteLine("Stub hit for Nn.Am.Service.IWindowController.RejectToChangeIntoBackground");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateWindow
				var _return = CreateWindow(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetAppletResourceUserId
				var _return = GetAppletResourceUserId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // AcquireForegroundRights
				AcquireForegroundRights();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // ReleaseForegroundRights
				ReleaseForegroundRights();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // RejectToChangeIntoBackground
				RejectToChangeIntoBackground();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IWindowController");
		}
	}
}

