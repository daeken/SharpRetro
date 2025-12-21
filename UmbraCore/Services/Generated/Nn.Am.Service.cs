using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Am.Service;
public partial class IAllSystemAppletProxiesService : _IAllSystemAppletProxiesService_Base;
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
				om.Initialize(1, 0, 0);
				var _return = OpenSystemAppletProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // OpenLibraryAppletProxyOld
				om.Initialize(1, 0, 0);
				var _return = OpenLibraryAppletProxyOld(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // OpenLibraryAppletProxy
				om.Initialize(1, 0, 0);
				var _return = OpenLibraryAppletProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x15, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12C: { // OpenOverlayAppletProxy
				om.Initialize(1, 0, 0);
				var _return = OpenOverlayAppletProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15E: { // OpenSystemApplicationProxy
				om.Initialize(1, 0, 0);
				var _return = OpenSystemApplicationProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // CreateSelfLibraryAppletCreatorForDevelop
				om.Initialize(1, 0, 0);
				var _return = CreateSelfLibraryAppletCreatorForDevelop(im.GetData<ulong>(8), im.Pid);
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
				om.Initialize(0, 1, 0);
				var _return = GetAppletStateChangedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // IsCompleted
				om.Initialize(0, 0, 1);
				var _return = IsCompleted();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Start
				om.Initialize(0, 0, 0);
				Start();
				break;
			}
			case 0x14: { // RequestExit
				om.Initialize(0, 0, 0);
				RequestExit();
				break;
			}
			case 0x19: { // Terminate
				om.Initialize(0, 0, 0);
				Terminate();
				break;
			}
			case 0x1E: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
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
				om.Initialize(0, 1, 0);
				var _return = GetAppletStateChangedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // IsCompleted
				om.Initialize(0, 0, 1);
				var _return = IsCompleted();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Start
				om.Initialize(0, 0, 0);
				Start();
				break;
			}
			case 0x14: { // RequestExit
				om.Initialize(0, 0, 0);
				RequestExit();
				break;
			}
			case 0x19: { // Terminate
				om.Initialize(0, 0, 0);
				Terminate();
				break;
			}
			case 0x1E: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x65: { // RequestForApplicationToGetForeground
				om.Initialize(0, 0, 0);
				RequestForApplicationToGetForeground();
				break;
			}
			case 0x6E: { // TerminateAllLibraryApplets
				om.Initialize(0, 0, 0);
				TerminateAllLibraryApplets();
				break;
			}
			case 0x6F: { // AreAnyLibraryAppletsLeft
				om.Initialize(0, 0, 1);
				var _return = AreAnyLibraryAppletsLeft();
				om.SetData(8, _return);
				break;
			}
			case 0x70: { // GetCurrentLibraryApplet
				om.Initialize(1, 0, 0);
				var _return = GetCurrentLibraryApplet();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x78: { // GetApplicationId
				om.Initialize(0, 0, 8);
				var _return = GetApplicationId();
				om.SetData(8, _return);
				break;
			}
			case 0x79: { // PushLaunchParameter
				om.Initialize(0, 0, 0);
				PushLaunchParameter(im.GetData<uint>(8), Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x7A: { // GetApplicationControlProperty
				om.Initialize(0, 0, 0);
				GetApplicationControlProperty(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x7B: { // GetApplicationLaunchProperty
				om.Initialize(0, 0, 0);
				GetApplicationLaunchProperty(im.GetSpan<byte>(0x6, 0));
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
				om.Initialize(1, 0, 0);
				var _return = CreateApplication(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // PopLaunchRequestedApplication
				om.Initialize(1, 0, 0);
				var _return = PopLaunchRequestedApplication();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // CreateSystemApplication
				om.Initialize(1, 0, 0);
				var _return = CreateSystemApplication(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x64: { // PopFloatingApplicationForDevelopment
				om.Initialize(1, 0, 0);
				var _return = PopFloatingApplicationForDevelopment();
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
				om.Initialize(1, 0, 0);
				var _return = PopLaunchParameter(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // CreateApplicationAndPushAndRequestToStart
				om.Initialize(0, 0, 0);
				CreateApplicationAndPushAndRequestToStart(im.GetData<ulong>(8), Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0xB: { // CreateApplicationAndPushAndRequestToStartForQuest
				om.Initialize(0, 0, 0);
				CreateApplicationAndPushAndRequestToStartForQuest(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0xC: { // CreateApplicationAndRequestToStart
				om.Initialize(0, 0, 0);
				CreateApplicationAndRequestToStart(im.GetData<ulong>(8));
				break;
			}
			case 0xD: { // CreateApplicationAndRequestToStartForQuest
				om.Initialize(0, 0, 0);
				CreateApplicationAndRequestToStartForQuest(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16));
				break;
			}
			case 0x14: { // EnsureSaveData
				om.Initialize(0, 0, 8);
				var _return = EnsureSaveData(im.GetBytes(8, 0x10));
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetDesiredLanguage
				om.Initialize(0, 0, 8);
				GetDesiredLanguage(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // SetTerminateResult
				om.Initialize(0, 0, 0);
				SetTerminateResult(im.GetData<uint>(8));
				break;
			}
			case 0x17: { // GetDisplayVersion
				om.Initialize(0, 0, 16);
				GetDisplayVersion(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // GetLaunchStorageInfoForDebug
				om.Initialize(0, 0, 2);
				GetLaunchStorageInfoForDebug(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(9, _1);
				break;
			}
			case 0x19: { // ExtendSaveData
				om.Initialize(0, 0, 8);
				var _return = ExtendSaveData(im.GetData<byte>(8), im.GetBytes(16, 0x10), im.GetData<ulong>(32), im.GetData<ulong>(40));
				om.SetData(8, _return);
				break;
			}
			case 0x1A: { // GetSaveDataSize
				om.Initialize(0, 0, 16);
				GetSaveDataSize(im.GetData<byte>(8), im.GetBytes(16, 0x10), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x1E: { // BeginBlockingHomeButtonShortAndLongPressed
				om.Initialize(0, 0, 0);
				BeginBlockingHomeButtonShortAndLongPressed(im.GetData<ulong>(8));
				break;
			}
			case 0x1F: { // EndBlockingHomeButtonShortAndLongPressed
				om.Initialize(0, 0, 0);
				EndBlockingHomeButtonShortAndLongPressed();
				break;
			}
			case 0x20: { // BeginBlockingHomeButton
				om.Initialize(0, 0, 0);
				BeginBlockingHomeButton(im.GetData<ulong>(8));
				break;
			}
			case 0x21: { // EndBlockingHomeButton
				om.Initialize(0, 0, 0);
				EndBlockingHomeButton();
				break;
			}
			case 0x28: { // NotifyRunning
				om.Initialize(0, 0, 1);
				var _return = NotifyRunning();
				om.SetData(8, _return);
				break;
			}
			case 0x32: { // GetPseudoDeviceId
				om.Initialize(0, 0, 16);
				GetPseudoDeviceId(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3C: { // SetMediaPlaybackStateForApplication
				om.Initialize(0, 0, 0);
				SetMediaPlaybackStateForApplication(im.GetData<byte>(8));
				break;
			}
			case 0x41: { // IsGamePlayRecordingSupported
				om.Initialize(0, 0, 1);
				var _return = IsGamePlayRecordingSupported();
				om.SetData(8, _return);
				break;
			}
			case 0x42: { // InitializeGamePlayRecording
				om.Initialize(0, 0, 0);
				InitializeGamePlayRecording(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0x43: { // SetGamePlayRecordingState
				om.Initialize(0, 0, 0);
				SetGamePlayRecordingState(im.GetData<uint>(8));
				break;
			}
			case 0x44: { // RequestFlushGamePlayingMovieForDebug
				om.Initialize(0, 0, 0);
				RequestFlushGamePlayingMovieForDebug();
				break;
			}
			case 0x46: { // RequestToShutdown
				om.Initialize(0, 0, 0);
				RequestToShutdown();
				break;
			}
			case 0x47: { // RequestToReboot
				om.Initialize(0, 0, 0);
				RequestToReboot();
				break;
			}
			case 0x50: { // ExitAndRequestToShowThanksMessage
				om.Initialize(0, 0, 0);
				ExitAndRequestToShowThanksMessage();
				break;
			}
			case 0x5A: { // EnableApplicationCrashReport
				om.Initialize(0, 0, 0);
				EnableApplicationCrashReport(im.GetData<byte>(8));
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
				om.Initialize(1, 0, 0);
				var _return = GetCommonStateGetter();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				om.Initialize(1, 0, 0);
				var _return = GetSelfController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				om.Initialize(1, 0, 0);
				var _return = GetWindowController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				om.Initialize(1, 0, 0);
				var _return = GetAudioController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				om.Initialize(1, 0, 0);
				var _return = GetDisplayController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				om.Initialize(1, 0, 0);
				var _return = GetProcessWindingController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				om.Initialize(1, 0, 0);
				var _return = GetLibraryAppletCreator();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // GetApplicationFunctions
				om.Initialize(1, 0, 0);
				var _return = GetApplicationFunctions();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				om.Initialize(1, 0, 0);
				var _return = GetDebugFunctions();
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IApplicationProxy");
		}
	}
}

public partial class IApplicationProxyService : _IApplicationProxyService_Base;
public abstract class _IApplicationProxyService_Base : IpcInterface {
	protected virtual Nn.Am.Service.IApplicationProxy OpenApplicationProxy(ulong _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Am.Service.IApplicationProxyService.OpenApplicationProxy not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenApplicationProxy
				om.Initialize(1, 0, 0);
				var _return = OpenApplicationProxy(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
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
				om.Initialize(0, 0, 0);
				SetExpectedMasterVolume(im.GetData<float>(8), im.GetData<float>(12));
				break;
			}
			case 0x1: { // GetMainAppletExpectedMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetMainAppletExpectedMasterVolume();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetLibraryAppletExpectedMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetLibraryAppletExpectedMasterVolume();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // ChangeMainAppletMasterVolume
				om.Initialize(0, 0, 0);
				ChangeMainAppletMasterVolume(im.GetData<float>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x4: { // SetTransparentVolumeRate
				om.Initialize(0, 0, 0);
				SetTransparentVolumeRate(im.GetData<float>(8));
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
				om.Initialize(0, 1, 0);
				var _return = GetEventHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // ReceiveMessage
				om.Initialize(0, 0, 4);
				var _return = ReceiveMessage();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetThisAppletKind
				om.Initialize(0, 0, 8);
				GetThisAppletKind(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // AllowToEnterSleep
				om.Initialize(0, 0, 0);
				AllowToEnterSleep();
				break;
			}
			case 0x4: { // DisallowToEnterSleep
				om.Initialize(0, 0, 0);
				DisallowToEnterSleep();
				break;
			}
			case 0x5: { // GetOperationMode
				om.Initialize(0, 0, 1);
				var _return = GetOperationMode();
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // GetPerformanceMode
				om.Initialize(0, 0, 4);
				var _return = GetPerformanceMode();
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // GetCradleStatus
				om.Initialize(0, 0, 1);
				var _return = GetCradleStatus();
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // GetBootMode
				om.Initialize(0, 0, 1);
				var _return = GetBootMode();
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // GetCurrentFocusState
				om.Initialize(0, 0, 1);
				var _return = GetCurrentFocusState();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // RequestToAcquireSleepLock
				om.Initialize(0, 0, 0);
				RequestToAcquireSleepLock();
				break;
			}
			case 0xB: { // ReleaseSleepLock
				om.Initialize(0, 0, 0);
				ReleaseSleepLock();
				break;
			}
			case 0xC: { // ReleaseSleepLockTransiently
				om.Initialize(0, 0, 0);
				ReleaseSleepLockTransiently();
				break;
			}
			case 0xD: { // GetAcquiredSleepLockEvent
				om.Initialize(0, 1, 0);
				var _return = GetAcquiredSleepLockEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x14: { // PushToGeneralChannel
				om.Initialize(0, 0, 0);
				PushToGeneralChannel(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x1E: { // GetHomeButtonReaderLockAccessor
				om.Initialize(1, 0, 0);
				var _return = GetHomeButtonReaderLockAccessor();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F: { // GetReaderLockAccessorEx
				om.Initialize(1, 0, 0);
				var _return = GetReaderLockAccessorEx(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x28: { // GetCradleFwVersion
				om.Initialize(0, 0, 16);
				GetCradleFwVersion(out var _0, out var _1, out var _2, out var _3);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				break;
			}
			case 0x32: { // IsVrModeEnabled
				om.Initialize(0, 0, 1);
				var _return = IsVrModeEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x33: { // SetVrModeEnabled
				om.Initialize(0, 0, 0);
				SetVrModeEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x34: { // SetLcdBacklighOffEnabled
				om.Initialize(0, 0, 0);
				SetLcdBacklighOffEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x37: { // IsInControllerFirmwareUpdateSection
				om.Initialize(0, 0, 1);
				var _return = IsInControllerFirmwareUpdateSection();
				om.SetData(8, _return);
				break;
			}
			case 0x3C: { // GetDefaultDisplayResolution
				om.Initialize(0, 0, 8);
				GetDefaultDisplayResolution(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3D: { // GetDefaultDisplayResolutionChangeEvent
				om.Initialize(0, 1, 0);
				var _return = GetDefaultDisplayResolutionChangeEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3E: { // GetHdcpAuthenticationState
				om.Initialize(0, 0, 4);
				var _return = GetHdcpAuthenticationState();
				om.SetData(8, _return);
				break;
			}
			case 0x3F: { // GetHdcpAuthenticationStateChangeEvent
				om.Initialize(0, 1, 0);
				var _return = GetHdcpAuthenticationStateChangeEvent();
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
				om.Initialize(0, 0, 0);
				NotifyMessageToHomeMenuForDebug(im.GetData<uint>(8));
				break;
			}
			case 0x1: { // OpenMainApplication
				om.Initialize(1, 0, 0);
				var _return = OpenMainApplication();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // EmulateButtonEvent
				om.Initialize(0, 0, 0);
				EmulateButtonEvent(im.GetData<uint>(8));
				break;
			}
			case 0x14: { // InvalidateTransitionLayer
				om.Initialize(0, 0, 0);
				InvalidateTransitionLayer();
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
				om.Initialize(0, 0, 0);
				GetLastForegroundCaptureImage(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x1: { // UpdateLastForegroundCaptureImage
				om.Initialize(0, 0, 0);
				UpdateLastForegroundCaptureImage();
				break;
			}
			case 0x2: { // GetLastApplicationCaptureImage
				om.Initialize(0, 0, 0);
				GetLastApplicationCaptureImage(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x3: { // GetCallerAppletCaptureImage
				om.Initialize(0, 0, 0);
				GetCallerAppletCaptureImage(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x4: { // UpdateCallerAppletCaptureImage
				om.Initialize(0, 0, 0);
				UpdateCallerAppletCaptureImage();
				break;
			}
			case 0x5: { // GetLastForegroundCaptureImageEx
				om.Initialize(0, 0, 1);
				GetLastForegroundCaptureImageEx(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // GetLastApplicationCaptureImageEx
				om.Initialize(0, 0, 1);
				GetLastApplicationCaptureImageEx(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7: { // GetCallerAppletCaptureImageEx
				om.Initialize(0, 0, 1);
				GetCallerAppletCaptureImageEx(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x8: { // TakeScreenShotOfOwnLayer
				om.Initialize(0, 0, 0);
				TakeScreenShotOfOwnLayer(im.GetData<byte>(8), im.GetData<uint>(12));
				break;
			}
			case 0xA: { // AcquireLastApplicationCaptureBuffer
				om.Initialize(0, 1, 0);
				var _return = AcquireLastApplicationCaptureBuffer();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xB: { // ReleaseLastApplicationCaptureBuffer
				om.Initialize(0, 0, 0);
				ReleaseLastApplicationCaptureBuffer();
				break;
			}
			case 0xC: { // AcquireLastForegroundCaptureBuffer
				om.Initialize(0, 1, 0);
				var _return = AcquireLastForegroundCaptureBuffer();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xD: { // ReleaseLastForegroundCaptureBuffer
				om.Initialize(0, 0, 0);
				ReleaseLastForegroundCaptureBuffer();
				break;
			}
			case 0xE: { // AcquireCallerAppletCaptureBuffer
				om.Initialize(0, 1, 0);
				var _return = AcquireCallerAppletCaptureBuffer();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xF: { // ReleaseCallerAppletCaptureBuffer
				om.Initialize(0, 0, 0);
				ReleaseCallerAppletCaptureBuffer();
				break;
			}
			case 0x10: { // AcquireLastApplicationCaptureBufferEx
				om.Initialize(0, 1, 1);
				AcquireLastApplicationCaptureBufferEx(out var _0, out var _1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x11: { // AcquireLastForegroundCaptureBufferEx
				om.Initialize(0, 1, 1);
				AcquireLastForegroundCaptureBufferEx(out var _0, out var _1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x12: { // AcquireCallerAppletCaptureBufferEx
				om.Initialize(0, 1, 1);
				AcquireCallerAppletCaptureBufferEx(out var _0, out var _1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x14: { // ClearCaptureBuffer
				om.Initialize(0, 0, 0);
				ClearCaptureBuffer(im.GetData<byte>(8), im.GetData<uint>(12), im.GetData<uint>(16));
				break;
			}
			case 0x15: { // ClearAppletTransitionBuffer
				om.Initialize(0, 0, 0);
				ClearAppletTransitionBuffer(im.GetData<uint>(8));
				break;
			}
			case 0x16: { // AcquireLastApplicationCaptureSharedBuffer
				om.Initialize(0, 0, 8);
				AcquireLastApplicationCaptureSharedBuffer(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x17: { // ReleaseLastApplicationCaptureSharedBuffer
				om.Initialize(0, 0, 0);
				ReleaseLastApplicationCaptureSharedBuffer();
				break;
			}
			case 0x18: { // AcquireLastForegroundCaptureSharedBuffer
				om.Initialize(0, 0, 8);
				AcquireLastForegroundCaptureSharedBuffer(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x19: { // ReleaseLastForegroundCaptureSharedBuffer
				om.Initialize(0, 0, 0);
				ReleaseLastForegroundCaptureSharedBuffer();
				break;
			}
			case 0x1A: { // AcquireCallerAppletCaptureSharedBuffer
				om.Initialize(0, 0, 8);
				AcquireCallerAppletCaptureSharedBuffer(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1B: { // ReleaseCallerAppletCaptureSharedBuffer
				om.Initialize(0, 0, 0);
				ReleaseCallerAppletCaptureSharedBuffer();
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
				om.Initialize(0, 0, 0);
				RequestToEnterSleep();
				break;
			}
			case 0x1: { // EnterSleep
				om.Initialize(0, 0, 0);
				EnterSleep();
				break;
			}
			case 0x2: { // StartSleepSequence
				om.Initialize(0, 0, 0);
				StartSleepSequence(im.GetData<byte>(8));
				break;
			}
			case 0x3: { // StartShutdownSequence
				om.Initialize(0, 0, 0);
				StartShutdownSequence();
				break;
			}
			case 0x4: { // StartRebootSequence
				om.Initialize(0, 0, 0);
				StartRebootSequence();
				break;
			}
			case 0xA: { // LoadAndApplyIdlePolicySettings
				om.Initialize(0, 0, 0);
				LoadAndApplyIdlePolicySettings();
				break;
			}
			case 0xB: { // NotifyCecSettingsChanged
				om.Initialize(0, 0, 0);
				NotifyCecSettingsChanged();
				break;
			}
			case 0xC: { // SetDefaultHomeButtonLongPressTime
				om.Initialize(0, 0, 0);
				SetDefaultHomeButtonLongPressTime(im.GetData<ulong>(8));
				break;
			}
			case 0xD: { // UpdateDefaultDisplayResolution
				om.Initialize(0, 0, 0);
				UpdateDefaultDisplayResolution();
				break;
			}
			case 0xE: { // ShouldSleepOnBoot
				om.Initialize(0, 0, 1);
				var _return = ShouldSleepOnBoot();
				om.SetData(8, _return);
				break;
			}
			case 0xF: { // GetHdcpAuthenticationFailedEvent
				om.Initialize(0, 1, 0);
				var _return = GetHdcpAuthenticationFailedEvent();
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
				om.Initialize(0, 0, 0);
				RequestToGetForeground();
				break;
			}
			case 0xB: { // LockForeground
				om.Initialize(0, 0, 0);
				LockForeground();
				break;
			}
			case 0xC: { // UnlockForeground
				om.Initialize(0, 0, 0);
				UnlockForeground();
				break;
			}
			case 0x14: { // PopFromGeneralChannel
				om.Initialize(1, 0, 0);
				var _return = PopFromGeneralChannel();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15: { // GetPopFromGeneralChannelEvent
				om.Initialize(0, 1, 0);
				var _return = GetPopFromGeneralChannelEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1E: { // GetHomeButtonWriterLockAccessor
				om.Initialize(1, 0, 0);
				var _return = GetHomeButtonWriterLockAccessor();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F: { // GetWriterLockAccessorEx
				om.Initialize(1, 0, 0);
				var _return = GetWriterLockAccessorEx(im.GetData<uint>(8));
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
				om.Initialize(0, 1, 0);
				var _return = GetAppletStateChangedEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // IsCompleted
				om.Initialize(0, 0, 1);
				var _return = IsCompleted();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Start
				om.Initialize(0, 0, 0);
				Start();
				break;
			}
			case 0x14: { // RequestExit
				om.Initialize(0, 0, 0);
				RequestExit();
				break;
			}
			case 0x19: { // Terminate
				om.Initialize(0, 0, 0);
				Terminate();
				break;
			}
			case 0x1E: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x32: { // SetOutOfFocusApplicationSuspendingEnabled
				om.Initialize(0, 0, 0);
				SetOutOfFocusApplicationSuspendingEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x64: { // PushInData
				om.Initialize(0, 0, 0);
				PushInData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x65: { // PopOutData
				om.Initialize(1, 0, 0);
				var _return = PopOutData();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // PushExtraStorage
				om.Initialize(0, 0, 0);
				PushExtraStorage(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x67: { // PushInteractiveInData
				om.Initialize(0, 0, 0);
				PushInteractiveInData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x68: { // PopInteractiveOutData
				om.Initialize(1, 0, 0);
				var _return = PopInteractiveOutData();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x69: { // GetPopOutDataEvent
				om.Initialize(0, 1, 0);
				var _return = GetPopOutDataEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6A: { // GetPopInteractiveOutDataEvent
				om.Initialize(0, 1, 0);
				var _return = GetPopInteractiveOutDataEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6E: { // NeedsToExitProcess
				om.Initialize(0, 0, 1);
				var _return = NeedsToExitProcess();
				om.SetData(8, _return);
				break;
			}
			case 0x78: { // GetLibraryAppletInfo
				om.Initialize(0, 0, 8);
				GetLibraryAppletInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x96: { // RequestForAppletToGetForeground
				om.Initialize(0, 0, 0);
				RequestForAppletToGetForeground();
				break;
			}
			case 0xA0: { // GetIndirectLayerConsumerHandle
				om.Initialize(0, 0, 8);
				var _return = GetIndirectLayerConsumerHandle(im.GetData<ulong>(8), im.Pid);
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
				om.Initialize(1, 0, 0);
				var _return = CreateLibraryApplet(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // TerminateAllLibraryApplets
				om.Initialize(0, 0, 0);
				TerminateAllLibraryApplets();
				break;
			}
			case 0x2: { // AreAnyLibraryAppletsLeft
				om.Initialize(0, 0, 1);
				var _return = AreAnyLibraryAppletsLeft();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // CreateStorage
				om.Initialize(1, 0, 0);
				var _return = CreateStorage(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // CreateTransferMemoryStorage
				om.Initialize(1, 0, 0);
				var _return = CreateTransferMemoryStorage(im.GetData<byte>(8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC: { // CreateHandleStorage
				om.Initialize(1, 0, 0);
				var _return = CreateHandleStorage(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
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
				om.Initialize(1, 0, 0);
				var _return = GetCommonStateGetter();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				om.Initialize(1, 0, 0);
				var _return = GetSelfController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				om.Initialize(1, 0, 0);
				var _return = GetWindowController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				om.Initialize(1, 0, 0);
				var _return = GetAudioController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				om.Initialize(1, 0, 0);
				var _return = GetDisplayController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				om.Initialize(1, 0, 0);
				var _return = GetProcessWindingController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				om.Initialize(1, 0, 0);
				var _return = GetLibraryAppletCreator();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // OpenLibraryAppletSelfAccessor
				om.Initialize(1, 0, 0);
				var _return = OpenLibraryAppletSelfAccessor();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				om.Initialize(1, 0, 0);
				var _return = GetDebugFunctions();
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
				om.Initialize(1, 0, 0);
				var _return = PopInData();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // PushOutData
				om.Initialize(0, 0, 0);
				PushOutData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x2: { // PopInteractiveInData
				om.Initialize(1, 0, 0);
				var _return = PopInteractiveInData();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // PushInteractiveOutData
				om.Initialize(0, 0, 0);
				PushInteractiveOutData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x5: { // GetPopInDataEvent
				om.Initialize(0, 1, 0);
				var _return = GetPopInDataEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6: { // GetPopInteractiveInDataEvent
				om.Initialize(0, 1, 0);
				var _return = GetPopInteractiveInDataEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // ExitProcessAndReturn
				om.Initialize(0, 0, 0);
				ExitProcessAndReturn();
				break;
			}
			case 0xB: { // GetLibraryAppletInfo
				om.Initialize(0, 0, 8);
				GetLibraryAppletInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetMainAppletIdentityInfo
				om.Initialize(0, 0, 0);
				GetMainAppletIdentityInfo();
				break;
			}
			case 0xD: { // CanUseApplicationCore
				om.Initialize(0, 0, 1);
				var _return = CanUseApplicationCore();
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // GetCallerAppletIdentityInfo
				om.Initialize(0, 0, 0);
				GetCallerAppletIdentityInfo();
				break;
			}
			case 0xF: { // GetMainAppletApplicationControlProperty
				om.Initialize(0, 0, 0);
				GetMainAppletApplicationControlProperty(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x10: { // GetMainAppletStorageId
				om.Initialize(0, 0, 1);
				var _return = GetMainAppletStorageId();
				om.SetData(8, _return);
				break;
			}
			case 0x11: { // GetCallerAppletIdentityInfoStack
				om.Initialize(0, 0, 4);
				GetCallerAppletIdentityInfoStack(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x12: { // GetNextReturnDestinationAppletIdentityInfo
				om.Initialize(0, 0, 0);
				GetNextReturnDestinationAppletIdentityInfo();
				break;
			}
			case 0x13: { // GetDesirableKeyboardLayout
				om.Initialize(0, 0, 4);
				var _return = GetDesirableKeyboardLayout();
				om.SetData(8, _return);
				break;
			}
			case 0x14: { // PopExtraStorage
				om.Initialize(1, 0, 0);
				var _return = PopExtraStorage();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x19: { // GetPopExtraStorageEvent
				om.Initialize(0, 1, 0);
				var _return = GetPopExtraStorageEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1E: { // UnpopInData
				om.Initialize(0, 0, 0);
				UnpopInData(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x1F: { // UnpopExtraStorage
				om.Initialize(0, 0, 0);
				UnpopExtraStorage(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x28: { // GetIndirectLayerProducerHandle
				om.Initialize(0, 0, 8);
				var _return = GetIndirectLayerProducerHandle();
				om.SetData(8, _return);
				break;
			}
			case 0x32: { // ReportVisibleError
				om.Initialize(0, 0, 0);
				ReportVisibleError(im.GetBytes(8, 0x8));
				break;
			}
			case 0x33: { // ReportVisibleErrorWithErrorContext
				om.Initialize(0, 0, 0);
				ReportVisibleErrorWithErrorContext(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x15, 0));
				break;
			}
			case 0x3C: { // GetMainAppletApplicationDesiredLanguage
				om.Initialize(0, 0, 8);
				GetMainAppletApplicationDesiredLanguage(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // CreateGameMovieTrimmer
				om.Initialize(1, 0, 0);
				var _return = CreateGameMovieTrimmer(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
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
				om.Initialize(0, 1, 1);
				TryLock(im.GetData<byte>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x2: { // Unlock
				om.Initialize(0, 0, 0);
				Unlock();
				break;
			}
			case 0x3: { // GetEvent
				om.Initialize(0, 1, 0);
				var _return = GetEvent();
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
				om.Initialize(1, 0, 0);
				var _return = GetCommonStateGetter();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				om.Initialize(1, 0, 0);
				var _return = GetSelfController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				om.Initialize(1, 0, 0);
				var _return = GetWindowController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				om.Initialize(1, 0, 0);
				var _return = GetAudioController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				om.Initialize(1, 0, 0);
				var _return = GetDisplayController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				om.Initialize(1, 0, 0);
				var _return = GetProcessWindingController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				om.Initialize(1, 0, 0);
				var _return = GetLibraryAppletCreator();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // GetOverlayFunctions
				om.Initialize(1, 0, 0);
				var _return = GetOverlayFunctions();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				om.Initialize(1, 0, 0);
				var _return = GetDebugFunctions();
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
				om.Initialize(0, 0, 0);
				BeginToWatchShortHomeButtonMessage();
				break;
			}
			case 0x1: { // EndToWatchShortHomeButtonMessage
				om.Initialize(0, 0, 0);
				EndToWatchShortHomeButtonMessage();
				break;
			}
			case 0x2: { // GetApplicationIdForLogo
				om.Initialize(0, 0, 8);
				var _return = GetApplicationIdForLogo();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetGpuTimeSliceBoost
				om.Initialize(0, 0, 0);
				SetGpuTimeSliceBoost(im.GetData<ulong>(8));
				break;
			}
			case 0x4: { // SetAutoSleepTimeAndDimmingTimeEnabled
				om.Initialize(0, 0, 0);
				SetAutoSleepTimeAndDimmingTimeEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x5: { // TerminateApplicationAndSetReason
				om.Initialize(0, 0, 0);
				TerminateApplicationAndSetReason(im.GetData<uint>(8));
				break;
			}
			case 0x6: { // SetScreenShotPermissionGlobally
				om.Initialize(0, 0, 0);
				SetScreenShotPermissionGlobally(im.GetData<byte>(8));
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
				om.Initialize(0, 0, 4);
				GetLaunchReason(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // OpenCallingLibraryApplet
				om.Initialize(1, 0, 0);
				var _return = OpenCallingLibraryApplet();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15: { // PushContext
				om.Initialize(0, 0, 0);
				PushContext(Kernel.Get<Nn.Am.Service.IStorage>(im.GetMove(0)));
				break;
			}
			case 0x16: { // PopContext
				om.Initialize(1, 0, 0);
				var _return = PopContext();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x17: { // CancelWindingReservation
				om.Initialize(0, 0, 0);
				CancelWindingReservation();
				break;
			}
			case 0x1E: { // WindAndDoReserved
				om.Initialize(0, 0, 0);
				WindAndDoReserved();
				break;
			}
			case 0x28: { // ReserveToStartAndWaitAndUnwindThis
				om.Initialize(0, 0, 0);
				ReserveToStartAndWaitAndUnwindThis(Kernel.Get<Nn.Am.Service.ILibraryAppletAccessor>(im.GetMove(0)));
				break;
			}
			case 0x29: { // ReserveToStartAndWait
				om.Initialize(0, 0, 0);
				ReserveToStartAndWait(Kernel.Get<Nn.Am.Service.ILibraryAppletAccessor>(im.GetMove(0)));
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
				om.Initialize(0, 0, 0);
				Exit();
				break;
			}
			case 0x1: { // LockExit
				om.Initialize(0, 0, 0);
				LockExit();
				break;
			}
			case 0x2: { // UnlockExit
				om.Initialize(0, 0, 0);
				UnlockExit();
				break;
			}
			case 0x3: { // EnterFatalSection
				om.Initialize(0, 0, 0);
				EnterFatalSection();
				break;
			}
			case 0x4: { // LeaveFatalSection
				om.Initialize(0, 0, 0);
				LeaveFatalSection();
				break;
			}
			case 0x9: { // GetLibraryAppletLaunchableEvent
				om.Initialize(0, 1, 0);
				var _return = GetLibraryAppletLaunchableEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // SetScreenShotPermission
				om.Initialize(0, 0, 0);
				SetScreenShotPermission(im.GetData<uint>(8));
				break;
			}
			case 0xB: { // SetOperationModeChangedNotification
				om.Initialize(0, 0, 0);
				SetOperationModeChangedNotification(im.GetData<byte>(8));
				break;
			}
			case 0xC: { // SetPerformanceModeChangedNotification
				om.Initialize(0, 0, 0);
				SetPerformanceModeChangedNotification(im.GetData<byte>(8));
				break;
			}
			case 0xD: { // SetFocusHandlingMode
				om.Initialize(0, 0, 0);
				SetFocusHandlingMode(im.GetData<byte>(8), im.GetData<byte>(9), im.GetData<byte>(10));
				break;
			}
			case 0xE: { // SetRestartMessageEnabled
				om.Initialize(0, 0, 0);
				SetRestartMessageEnabled(im.GetData<byte>(8));
				break;
			}
			case 0xF: { // SetScreenShotAppletIdentityInfo
				om.Initialize(0, 0, 0);
				SetScreenShotAppletIdentityInfo();
				break;
			}
			case 0x10: { // SetOutOfFocusSuspendingEnabled
				om.Initialize(0, 0, 0);
				SetOutOfFocusSuspendingEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x11: { // SetControllerFirmwareUpdateSection
				om.Initialize(0, 0, 0);
				SetControllerFirmwareUpdateSection(im.GetData<byte>(8));
				break;
			}
			case 0x12: { // SetRequiresCaptureButtonShortPressedMessage
				om.Initialize(0, 0, 0);
				SetRequiresCaptureButtonShortPressedMessage(im.GetData<byte>(8));
				break;
			}
			case 0x13: { // SetScreenShotImageOrientation
				om.Initialize(0, 0, 0);
				SetScreenShotImageOrientation(im.GetData<uint>(8));
				break;
			}
			case 0x14: { // SetDesirableKeyboardLayout
				om.Initialize(0, 0, 0);
				SetDesirableKeyboardLayout(im.GetData<uint>(8));
				break;
			}
			case 0x28: { // CreateManagedDisplayLayer
				om.Initialize(0, 0, 8);
				var _return = CreateManagedDisplayLayer();
				om.SetData(8, _return);
				break;
			}
			case 0x29: { // IsSystemBufferSharingEnabled
				om.Initialize(0, 0, 0);
				IsSystemBufferSharingEnabled();
				break;
			}
			case 0x2A: { // GetSystemSharedLayerHandle
				om.Initialize(0, 0, 16);
				GetSystemSharedLayerHandle(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x32: { // SetHandlesRequestToDisplay
				om.Initialize(0, 0, 0);
				SetHandlesRequestToDisplay(im.GetData<byte>(8));
				break;
			}
			case 0x33: { // ApproveToDisplay
				om.Initialize(0, 0, 0);
				ApproveToDisplay();
				break;
			}
			case 0x3C: { // OverrideAutoSleepTimeAndDimmingTime
				om.Initialize(0, 0, 0);
				OverrideAutoSleepTimeAndDimmingTime(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20));
				break;
			}
			case 0x3D: { // SetMediaPlaybackState
				om.Initialize(0, 0, 0);
				SetMediaPlaybackState(im.GetData<byte>(8));
				break;
			}
			case 0x3E: { // SetIdleTimeDetectionExtension
				om.Initialize(0, 0, 0);
				SetIdleTimeDetectionExtension(im.GetData<uint>(8));
				break;
			}
			case 0x3F: { // GetIdleTimeDetectionExtension
				om.Initialize(0, 0, 4);
				var _return = GetIdleTimeDetectionExtension();
				om.SetData(8, _return);
				break;
			}
			case 0x40: { // SetInputDetectionSourceSet
				om.Initialize(0, 0, 0);
				SetInputDetectionSourceSet(im.GetData<uint>(8));
				break;
			}
			case 0x41: { // ReportUserIsActive
				om.Initialize(0, 0, 0);
				ReportUserIsActive();
				break;
			}
			case 0x42: { // GetCurrentIlluminance
				om.Initialize(0, 0, 4);
				var _return = GetCurrentIlluminance();
				om.SetData(8, _return);
				break;
			}
			case 0x43: { // IsIlluminanceAvailable
				om.Initialize(0, 0, 1);
				var _return = IsIlluminanceAvailable();
				om.SetData(8, _return);
				break;
			}
			case 0x46: { // ReportMultimediaError
				om.Initialize(0, 0, 0);
				ReportMultimediaError(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x50: { // SetWirelessPriorityMode
				om.Initialize(0, 0, 0);
				SetWirelessPriorityMode(im.GetData<uint>(8));
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
				om.Initialize(1, 0, 0);
				var _return = Unknown0();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(1, 0, 0);
				var _return = Unknown1();
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
				om.Initialize(0, 0, 8);
				var _return = GetSize();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Write
				om.Initialize(0, 0, 0);
				Write(im.GetData<ulong>(8), im.GetSpan<byte>(0x21, 0));
				break;
			}
			case 0xB: { // Read
				om.Initialize(0, 0, 0);
				Read(im.GetData<ulong>(8), im.GetSpan<byte>(0x22, 0));
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
				om.Initialize(1, 0, 0);
				var _return = GetCommonStateGetter();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSelfController
				om.Initialize(1, 0, 0);
				var _return = GetSelfController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetWindowController
				om.Initialize(1, 0, 0);
				var _return = GetWindowController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetAudioController
				om.Initialize(1, 0, 0);
				var _return = GetAudioController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetDisplayController
				om.Initialize(1, 0, 0);
				var _return = GetDisplayController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // GetProcessWindingController
				om.Initialize(1, 0, 0);
				var _return = GetProcessWindingController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xB: { // GetLibraryAppletCreator
				om.Initialize(1, 0, 0);
				var _return = GetLibraryAppletCreator();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x14: { // GetHomeMenuFunctions
				om.Initialize(1, 0, 0);
				var _return = GetHomeMenuFunctions();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15: { // GetGlobalStateController
				om.Initialize(1, 0, 0);
				var _return = GetGlobalStateController();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x16: { // GetApplicationCreator
				om.Initialize(1, 0, 0);
				var _return = GetApplicationCreator();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // GetDebugFunctions
				om.Initialize(1, 0, 0);
				var _return = GetDebugFunctions();
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
				om.Initialize(0, 0, 8);
				var _return = GetSize();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetHandle
				om.Initialize(0, 1, 8);
				GetHandle(out var _0, out var _1);
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
				om.Initialize(1, 0, 0);
				var _return = CreateWindow(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetAppletResourceUserId
				om.Initialize(0, 0, 8);
				var _return = GetAppletResourceUserId();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // AcquireForegroundRights
				om.Initialize(0, 0, 0);
				AcquireForegroundRights();
				break;
			}
			case 0xB: { // ReleaseForegroundRights
				om.Initialize(0, 0, 0);
				ReleaseForegroundRights();
				break;
			}
			case 0xC: { // RejectToChangeIntoBackground
				om.Initialize(0, 0, 0);
				RejectToChangeIntoBackground();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Am.Service.IWindowController");
		}
	}
}

