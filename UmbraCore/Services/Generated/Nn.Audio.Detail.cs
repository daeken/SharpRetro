using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audio.Detail;
public enum AudioRendererRenderingDevice : byte {
	Dsp = 0x0,
	Cpu = 0x1,
}
public enum AudioRendererExecutionMode : byte {
	Auto = 0x0,
	Manual = 0x1,
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AudioRendererParameterInternal {
	public int SampleRate;
	public int SampleCount;
	public int MixBufferCount;
	public int SubMixBufferCount;
	public int VoiceCount;
	public int SinkCount;
	public int EffectCount;
	public int PerformanceManagerCount;
	public byte VoiceDropEnable;
	public byte Reserved;
	public Nn.Audio.Detail.AudioRendererRenderingDevice RenderingDevice;
	public Nn.Audio.Detail.AudioRendererExecutionMode ExecutionMode;
	public int SplitterCount;
	public int SplitterDestinationDataCount;
	public int ExternalContextSize;
	public int Revision;
}
public partial class IAudioDebugManager : _IAudioDebugManager_Base;
public abstract class _IAudioDebugManager_Base : IpcInterface {
	protected virtual void Unknown0(uint _0, ulong _1, KObject _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown1".Log();
	protected virtual void Unknown2() =>
		"Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown2".Log();
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown3".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetData<uint>(8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioDebugManager");
		}
	}
}

public partial class IAudioDevice : _IAudioDevice_Base;
public abstract class _IAudioDevice_Base : IpcInterface {
	protected virtual void ListAudioDeviceNames(out uint count, Span<byte> names) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.ListAudioDeviceNames not implemented");
	protected virtual void SetAudioDeviceOutputVolume(float volume, Span<byte> name) =>
		"Stub hit for Nn.Audio.Detail.IAudioDevice.SetAudioDeviceOutputVolume".Log();
	protected virtual float GetAudioDeviceOutputVolume(Span<byte> name) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.GetAudioDeviceOutputVolume not implemented");
	protected virtual void GetActiveAudioDeviceName(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.GetActiveAudioDeviceName not implemented");
	protected virtual KObject QueryAudioDeviceSystemEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.QueryAudioDeviceSystemEvent not implemented");
	protected virtual int GetActiveChannelCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.GetActiveChannelCount not implemented");
	protected virtual void ListAudioDeviceNameAuto(out uint count, Span<byte> names) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.ListAudioDeviceNameAuto not implemented");
	protected virtual void SetAudioDeviceOutputVolumeAuto(float volume, Span<byte> name) =>
		"Stub hit for Nn.Audio.Detail.IAudioDevice.SetAudioDeviceOutputVolumeAuto".Log();
	protected virtual float GetAudioDeviceOutputVolumeAuto(Span<byte> name) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.GetAudioDeviceOutputVolumeAuto not implemented");
	protected virtual void GetActiveAudioDeviceNameAuto(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.GetActiveAudioDeviceNameAuto not implemented");
	protected virtual KObject QueryAudioDeviceInputEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.QueryAudioDeviceInputEvent not implemented");
	protected virtual KObject QueryAudioDeviceOutputEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.QueryAudioDeviceOutputEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListAudioDeviceNames
				ListAudioDeviceNames(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // SetAudioDeviceOutputVolume
				SetAudioDeviceOutputVolume(im.GetData<float>(8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetAudioDeviceOutputVolume
				var _return = GetAudioDeviceOutputVolume(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetActiveAudioDeviceName
				GetActiveAudioDeviceName(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // QueryAudioDeviceSystemEvent
				var _return = QueryAudioDeviceSystemEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetActiveChannelCount
				var _return = GetActiveChannelCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // ListAudioDeviceNameAuto
				ListAudioDeviceNameAuto(out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x7: { // SetAudioDeviceOutputVolumeAuto
				SetAudioDeviceOutputVolumeAuto(im.GetData<float>(8), im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetAudioDeviceOutputVolumeAuto
				var _return = GetAudioDeviceOutputVolumeAuto(im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // GetActiveAudioDeviceNameAuto
				GetActiveAudioDeviceNameAuto(im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // QueryAudioDeviceInputEvent
				var _return = QueryAudioDeviceInputEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xC: { // QueryAudioDeviceOutputEvent
				var _return = QueryAudioDeviceOutputEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioDevice");
		}
	}
}

public partial class IAudioIn : _IAudioIn_Base;
public abstract class _IAudioIn_Base : IpcInterface {
	protected virtual uint GetAudioInState() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInState not implemented");
	protected virtual void StartAudioIn() =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.StartAudioIn".Log();
	protected virtual void StopAudioIn() =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.StopAudioIn".Log();
	protected virtual void AppendAudioInBuffer(ulong tag, Span<Nn.Audio.AudioInBuffer> _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBuffer".Log();
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedAudioInBuffer(out uint count, Span<Nn.Audio.AudioInBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetReleasedAudioInBuffer not implemented");
	protected virtual byte ContainsAudioInBuffer(ulong tag) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.ContainsAudioInBuffer not implemented");
	protected virtual void AppendAudioInBufferWithUserEvent(ulong tag, KObject _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferWithUserEvent".Log();
	protected virtual void AppendAudioInBufferAuto(ulong tag, Span<Nn.Audio.AudioInBuffer> _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferAuto".Log();
	protected virtual void GetReleasedAudioInBufferAuto(out uint count, Span<Nn.Audio.AudioInBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetReleasedAudioInBufferAuto not implemented");
	protected virtual void AppendAudioInBufferWithUserEventAuto(ulong tag, KObject _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferWithUserEventAuto".Log();
	protected virtual uint GetAudioInBufferCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInBufferCount not implemented");
	protected virtual void SetAudioInDeviceGain(uint gain) =>
		"Stub hit for Nn.Audio.Detail.IAudioIn.SetAudioInDeviceGain".Log();
	protected virtual uint GetAudioInDeviceGain() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInDeviceGain not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAudioInState
				var _return = GetAudioInState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartAudioIn
				StartAudioIn();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // StopAudioIn
				StopAudioIn();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // AppendAudioInBuffer
				AppendAudioInBuffer(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // RegisterBufferEvent
				var _return = RegisterBufferEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetReleasedAudioInBuffer
				GetReleasedAudioInBuffer(out var _0, im.GetSpan<Nn.Audio.AudioInBuffer>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // ContainsAudioInBuffer
				var _return = ContainsAudioInBuffer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // AppendAudioInBufferWithUserEvent
				AppendAudioInBufferWithUserEvent(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<Nn.Audio.AudioInBuffer>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // AppendAudioInBufferAuto
				AppendAudioInBufferAuto(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetReleasedAudioInBufferAuto
				GetReleasedAudioInBufferAuto(out var _0, im.GetSpan<Nn.Audio.AudioInBuffer>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xA: { // AppendAudioInBufferWithUserEventAuto
				AppendAudioInBufferWithUserEventAuto(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<Nn.Audio.AudioInBuffer>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // GetAudioInBufferCount
				var _return = GetAudioInBufferCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // SetAudioInDeviceGain
				SetAudioInDeviceGain(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // GetAudioInDeviceGain
				var _return = GetAudioInDeviceGain();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioIn");
		}
	}
}

public partial class IAudioInManager : _IAudioInManager_Base {
	public readonly string ServiceName;
	public IAudioInManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioInManager_Base : IpcInterface {
	protected virtual void ListAudioIns(out uint count, Span<byte> names) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.ListAudioIns not implemented");
	protected virtual void OpenAudioIn(ulong _0, ulong pid_copy, ulong _2, KObject _3, Span<byte> name, out uint sample_rate, out uint channel_count, out uint pcm_format, out uint _8, out Nn.Audio.Detail.IAudioIn _9, Span<byte> name_out) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.OpenAudioIn not implemented");
	protected virtual void Unknown2(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.Unknown2 not implemented");
	protected virtual void OpenAudioInAuto(ulong _0, ulong pid_copy, ulong _2, KObject _3, Span<byte> _4, out uint sample_rate, out uint channel_count, out uint pcm_format, out uint _8, out Nn.Audio.Detail.IAudioIn _9, Span<byte> name) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.OpenAudioInAuto not implemented");
	protected virtual void ListAudioInsAuto(out uint count, Span<byte> names) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.ListAudioInsAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListAudioIns
				ListAudioIns(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // OpenAudioIn
				OpenAudioIn(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x6, 0));
				om.Initialize(1, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // OpenAudioInAuto
				OpenAudioInAuto(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x21, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x22, 0));
				om.Initialize(1, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			case 0x4: { // ListAudioInsAuto
				ListAudioInsAuto(out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioInManager");
		}
	}
}

public partial class IAudioInManagerForApplet : _IAudioInManagerForApplet_Base {
	public readonly string ServiceName;
	public IAudioInManagerForApplet(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioInManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendAudioIns(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioInManagerForApplet.RequestSuspendAudioIns".Log();
	protected virtual void RequestResumeAudioIns(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioInManagerForApplet.RequestResumeAudioIns".Log();
	protected virtual uint GetAudioInsProcessMasterVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManagerForApplet.GetAudioInsProcessMasterVolume not implemented");
	protected virtual void SetAudioInsProcessMasterVolume(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioInManagerForApplet.SetAudioInsProcessMasterVolume".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioIns
				RequestSuspendAudioIns(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeAudioIns
				RequestResumeAudioIns(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetAudioInsProcessMasterVolume
				var _return = GetAudioInsProcessMasterVolume(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetAudioInsProcessMasterVolume
				SetAudioInsProcessMasterVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioInManagerForApplet");
		}
	}
}

public partial class IAudioInManagerForDebugger : _IAudioInManagerForDebugger_Base {
	public readonly string ServiceName;
	public IAudioInManagerForDebugger(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioInManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendAudioInsForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioInManagerForDebugger.RequestSuspendAudioInsForDebug".Log();
	protected virtual void RequestResumeAudioInsForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioInManagerForDebugger.RequestResumeAudioInsForDebug".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioInsForDebug
				RequestSuspendAudioInsForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeAudioInsForDebug
				RequestResumeAudioInsForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioInManagerForDebugger");
		}
	}
}

public partial class IAudioOut : _IAudioOut_Base;
public abstract class _IAudioOut_Base : IpcInterface {
	protected virtual uint GetAudioOutState() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetAudioOutState not implemented");
	protected virtual void StartAudioOut() =>
		"Stub hit for Nn.Audio.Detail.IAudioOut.StartAudioOut".Log();
	protected virtual void StopAudioOut() =>
		"Stub hit for Nn.Audio.Detail.IAudioOut.StopAudioOut".Log();
	protected virtual void AppendAudioOutBuffer(ulong tag, Span<Nn.Audio.AudioOutBuffer> _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioOut.AppendAudioOutBuffer".Log();
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedAudioOutBuffer(out uint count, Span<Nn.Audio.AudioOutBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetReleasedAudioOutBuffer not implemented");
	protected virtual byte ContainsAudioOutBuffer(ulong tag) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.ContainsAudioOutBuffer not implemented");
	protected virtual void AppendAudioOutBufferAuto(ulong tag, Span<Nn.Audio.AudioOutBuffer> _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioOut.AppendAudioOutBufferAuto".Log();
	protected virtual void GetReleasedAudioOutBufferAuto(out uint count, Span<Nn.Audio.AudioOutBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetReleasedAudioOutBufferAuto not implemented");
	protected virtual uint GetAudioOutBufferCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetAudioOutBufferCount not implemented");
	protected virtual ulong GetAudioOutPlayedSampleCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetAudioOutPlayedSampleCount not implemented");
	protected virtual byte FlushAudioOutBuffers() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.FlushAudioOutBuffers not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAudioOutState
				var _return = GetAudioOutState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartAudioOut
				StartAudioOut();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // StopAudioOut
				StopAudioOut();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // AppendAudioOutBuffer
				AppendAudioOutBuffer(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioOutBuffer>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // RegisterBufferEvent
				var _return = RegisterBufferEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetReleasedAudioOutBuffer
				GetReleasedAudioOutBuffer(out var _0, im.GetSpan<Nn.Audio.AudioOutBuffer>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // ContainsAudioOutBuffer
				var _return = ContainsAudioOutBuffer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // AppendAudioOutBufferAuto
				AppendAudioOutBufferAuto(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioOutBuffer>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetReleasedAudioOutBufferAuto
				GetReleasedAudioOutBufferAuto(out var _0, im.GetSpan<Nn.Audio.AudioOutBuffer>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x9: { // GetAudioOutBufferCount
				var _return = GetAudioOutBufferCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // GetAudioOutPlayedSampleCount
				var _return = GetAudioOutPlayedSampleCount();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // FlushAudioOutBuffers
				var _return = FlushAudioOutBuffers();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOut");
		}
	}
}

public partial class IAudioOutManager : _IAudioOutManager_Base {
	public readonly string ServiceName;
	public IAudioOutManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioOutManager_Base : IpcInterface {
	protected virtual void ListAudioOuts(out uint count, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.ListAudioOuts not implemented");
	protected virtual void OpenAudioOut(uint sample_rate, ushort channel_count, ushort unused, ulong _3, ulong _4, KObject _5, Span<byte> name_in, out uint sample_rate_out, out uint channel_count_out, out uint pcm_format, out uint state, out Nn.Audio.Detail.IAudioOut _11, Span<byte> name_out) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.OpenAudioOut not implemented");
	protected virtual void ListAudioOutsAuto(out uint count, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.ListAudioOutsAuto not implemented");
	protected virtual void OpenAudioOutAuto(uint sample_rate, ushort unused, ushort channel_count, ulong _3, ulong _4, KObject _5, Span<byte> _6, out uint sample_rate_out, out uint channel_count_out, out uint pcm_format, out uint _10, out Nn.Audio.Detail.IAudioOut _11, Span<byte> name_out) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.OpenAudioOutAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListAudioOuts
				ListAudioOuts(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // OpenAudioOut
				OpenAudioOut(im.GetData<uint>(8), im.GetData<ushort>(12), im.GetData<ushort>(14), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x6, 0));
				om.Initialize(1, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			case 0x2: { // ListAudioOutsAuto
				ListAudioOutsAuto(out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // OpenAudioOutAuto
				OpenAudioOutAuto(im.GetData<uint>(8), im.GetData<ushort>(12), im.GetData<ushort>(14), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x21, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x22, 0));
				om.Initialize(1, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOutManager");
		}
	}
}

public partial class IAudioOutManagerForApplet : _IAudioOutManagerForApplet_Base {
	public readonly string ServiceName;
	public IAudioOutManagerForApplet(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioOutManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendAudioOuts(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.RequestSuspendAudioOuts".Log();
	protected virtual void RequestResumeAudioOuts(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.RequestResumeAudioOuts".Log();
	protected virtual uint GetAudioOutsProcessMasterVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManagerForApplet.GetAudioOutsProcessMasterVolume not implemented");
	protected virtual void SetAudioOutsProcessMasterVolume(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.SetAudioOutsProcessMasterVolume".Log();
	protected virtual uint GetAudioOutsProcessRecordVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManagerForApplet.GetAudioOutsProcessRecordVolume not implemented");
	protected virtual void SetAudioOutsProcessRecordVolume(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.SetAudioOutsProcessRecordVolume".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioOuts
				RequestSuspendAudioOuts(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeAudioOuts
				RequestResumeAudioOuts(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetAudioOutsProcessMasterVolume
				var _return = GetAudioOutsProcessMasterVolume(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetAudioOutsProcessMasterVolume
				SetAudioOutsProcessMasterVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetAudioOutsProcessRecordVolume
				var _return = GetAudioOutsProcessRecordVolume(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // SetAudioOutsProcessRecordVolume
				SetAudioOutsProcessRecordVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOutManagerForApplet");
		}
	}
}

public partial class IAudioOutManagerForDebugger : _IAudioOutManagerForDebugger_Base {
	public readonly string ServiceName;
	public IAudioOutManagerForDebugger(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioOutManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendAudioOutsForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioOutManagerForDebugger.RequestSuspendAudioOutsForDebug".Log();
	protected virtual void RequestResumeAudioOutsForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioOutManagerForDebugger.RequestResumeAudioOutsForDebug".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioOutsForDebug
				RequestSuspendAudioOutsForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeAudioOutsForDebug
				RequestResumeAudioOutsForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOutManagerForDebugger");
		}
	}
}

public partial class IAudioRenderer : _IAudioRenderer_Base;
public abstract class _IAudioRenderer_Base : IpcInterface {
	protected virtual uint GetSampleRate() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.GetSampleRate not implemented");
	protected virtual uint GetSampleCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.GetSampleCount not implemented");
	protected virtual uint GetMixBufferCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.GetMixBufferCount not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.GetState not implemented");
	protected virtual void RequestUpdateAudioRenderer(Span<byte> input, Span<byte> output, Span<byte> performance) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.RequestUpdateAudioRenderer not implemented");
	protected virtual void Start() =>
		"Stub hit for Nn.Audio.Detail.IAudioRenderer.Start".Log();
	protected virtual void Stop() =>
		"Stub hit for Nn.Audio.Detail.IAudioRenderer.Stop".Log();
	protected virtual KObject QuerySystemEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.QuerySystemEvent not implemented");
	protected virtual void SetAudioRendererRenderingTimeLimit(uint limit) =>
		"Stub hit for Nn.Audio.Detail.IAudioRenderer.SetAudioRendererRenderingTimeLimit".Log();
	protected virtual uint GetAudioRendererRenderingTimeLimit() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.GetAudioRendererRenderingTimeLimit not implemented");
	protected virtual void RequestUpdateAudioRendererAuto(Span<byte> input, Span<byte> output, Span<byte> performance) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.RequestUpdateAudioRendererAuto not implemented");
	protected virtual void ExecuteAudioRendererRendering() =>
		"Stub hit for Nn.Audio.Detail.IAudioRenderer.ExecuteAudioRendererRendering".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSampleRate
				var _return = GetSampleRate();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetSampleCount
				var _return = GetSampleCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetMixBufferCount
				var _return = GetMixBufferCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // RequestUpdateAudioRenderer
				RequestUpdateAudioRenderer(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0), im.GetSpan<byte>(0x6, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Start
				Start();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Stop
				Stop();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // QuerySystemEvent
				var _return = QuerySystemEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x8: { // SetAudioRendererRenderingTimeLimit
				SetAudioRendererRenderingTimeLimit(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetAudioRendererRenderingTimeLimit
				var _return = GetAudioRendererRenderingTimeLimit();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // RequestUpdateAudioRendererAuto
				RequestUpdateAudioRendererAuto(im.GetSpan<byte>(0x21, 0), im.GetSpan<byte>(0x22, 0), im.GetSpan<byte>(0x22, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // ExecuteAudioRendererRendering
				ExecuteAudioRendererRendering();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRenderer");
		}
	}
}

public partial class IAudioRendererManager : _IAudioRendererManager_Base {
	public readonly string ServiceName;
	public IAudioRendererManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioRendererManager_Base : IpcInterface {
	protected virtual Nn.Audio.Detail.IAudioRenderer OpenAudioRenderer(Nn.Audio.Detail.AudioRendererParameterInternal parameter, ulong workBufferSize, ulong _2, ulong _3, KObject workBufferTransferMemory, KObject process) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.OpenAudioRenderer not implemented");
	protected virtual ulong GetWorkBufferSize(Nn.Audio.Detail.AudioRendererParameterInternal parameter) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.GetWorkBufferSize not implemented");
	protected virtual Nn.Audio.Detail.IAudioDevice GetAudioDeviceService(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.GetAudioDeviceService not implemented");
	protected virtual Nn.Audio.Detail.IAudioRenderer OpenAudioRendererAuto(Nn.Audio.Detail.AudioRendererParameterInternal parameter, ulong workBufferAddr, ulong workBufferSize, ulong _3, ulong _4, KObject process) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.OpenAudioRendererAuto not implemented");
	protected virtual Nn.Audio.Detail.IAudioDevice GetAudioDeviceServiceWithRevisionInfo(ulong _0, uint _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.GetAudioDeviceServiceWithRevisionInfo not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenAudioRenderer
				var _return = OpenAudioRenderer(*(Nn.Audio.Detail.AudioRendererParameterInternal*) im.GetDataPointer(8), im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), Kernel.Get<KObject>(im.GetCopy(1)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetWorkBufferSize
				var _return = GetWorkBufferSize(*(Nn.Audio.Detail.AudioRendererParameterInternal*) im.GetDataPointer(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetAudioDeviceService
				var _return = GetAudioDeviceService(im.GetData<ulong>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // OpenAudioRendererAuto
				var _return = OpenAudioRendererAuto(*(Nn.Audio.Detail.AudioRendererParameterInternal*) im.GetDataPointer(8), im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetAudioDeviceServiceWithRevisionInfo
				var _return = GetAudioDeviceServiceWithRevisionInfo(im.GetData<ulong>(8), im.GetData<uint>(16));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRendererManager");
		}
	}
}

public partial class IAudioRendererManagerForApplet : _IAudioRendererManagerForApplet_Base {
	public readonly string ServiceName;
	public IAudioRendererManagerForApplet(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioRendererManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendAudioRenderers(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.RequestSuspendAudioRenderers".Log();
	protected virtual void RequestResumeAudioRenderers(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.RequestResumeAudioRenderers".Log();
	protected virtual uint GetAudioRenderersProcessMasterVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManagerForApplet.GetAudioRenderersProcessMasterVolume not implemented");
	protected virtual void SetAudioRenderersProcessMasterVolume(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.SetAudioRenderersProcessMasterVolume".Log();
	protected virtual void RegisterAppletResourceUserId(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.RegisterAppletResourceUserId".Log();
	protected virtual void UnregisterAppletResourceUserId(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.UnregisterAppletResourceUserId".Log();
	protected virtual uint GetAudioRenderersProcessRecordVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManagerForApplet.GetAudioRenderersProcessRecordVolume not implemented");
	protected virtual void SetAudioRenderersProcessRecordVolume(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.SetAudioRenderersProcessRecordVolume".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioRenderers
				RequestSuspendAudioRenderers(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeAudioRenderers
				RequestResumeAudioRenderers(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetAudioRenderersProcessMasterVolume
				var _return = GetAudioRenderersProcessMasterVolume(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetAudioRenderersProcessMasterVolume
				SetAudioRenderersProcessMasterVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // RegisterAppletResourceUserId
				RegisterAppletResourceUserId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // UnregisterAppletResourceUserId
				UnregisterAppletResourceUserId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetAudioRenderersProcessRecordVolume
				var _return = GetAudioRenderersProcessRecordVolume(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // SetAudioRenderersProcessRecordVolume
				SetAudioRenderersProcessRecordVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRendererManagerForApplet");
		}
	}
}

public partial class IAudioRendererManagerForDebugger : _IAudioRendererManagerForDebugger_Base {
	public readonly string ServiceName;
	public IAudioRendererManagerForDebugger(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioRendererManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForDebugger.RequestSuspendForDebug".Log();
	protected virtual void RequestResumeForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IAudioRendererManagerForDebugger.RequestResumeForDebug".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendForDebug
				RequestSuspendForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeForDebug
				RequestResumeForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRendererManagerForDebugger");
		}
	}
}

public partial class ICodecController : _ICodecController_Base {
	public readonly string ServiceName;
	public ICodecController(string serviceName) => ServiceName = serviceName;
}
public abstract class _ICodecController_Base : IpcInterface {
	protected virtual void InitializeCodecController() =>
		"Stub hit for Nn.Audio.Detail.ICodecController.InitializeCodecController".Log();
	protected virtual void FinalizeCodecController() =>
		"Stub hit for Nn.Audio.Detail.ICodecController.FinalizeCodecController".Log();
	protected virtual void SleepCodecController() =>
		"Stub hit for Nn.Audio.Detail.ICodecController.SleepCodecController".Log();
	protected virtual void WakeCodecController() =>
		"Stub hit for Nn.Audio.Detail.ICodecController.WakeCodecController".Log();
	protected virtual void SetCodecVolume(uint _0) =>
		"Stub hit for Nn.Audio.Detail.ICodecController.SetCodecVolume".Log();
	protected virtual uint GetCodecVolumeMax() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.GetCodecVolumeMax not implemented");
	protected virtual uint GetCodecVolumeMin() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.GetCodecVolumeMin not implemented");
	protected virtual void SetCodecActiveTarget(uint _0) =>
		"Stub hit for Nn.Audio.Detail.ICodecController.SetCodecActiveTarget".Log();
	protected virtual uint GetCodecActiveTarget() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.GetCodecActiveTarget not implemented");
	protected virtual KObject BindCodecHeadphoneMicJackInterrupt() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.BindCodecHeadphoneMicJackInterrupt not implemented");
	protected virtual byte IsCodecHeadphoneMicJackInserted() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.IsCodecHeadphoneMicJackInserted not implemented");
	protected virtual void ClearCodecHeadphoneMicJackInterrupt() =>
		"Stub hit for Nn.Audio.Detail.ICodecController.ClearCodecHeadphoneMicJackInterrupt".Log();
	protected virtual byte IsCodecDeviceRequested() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.IsCodecDeviceRequested not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeCodecController
				InitializeCodecController();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // FinalizeCodecController
				FinalizeCodecController();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SleepCodecController
				SleepCodecController();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // WakeCodecController
				WakeCodecController();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // SetCodecVolume
				SetCodecVolume(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetCodecVolumeMax
				var _return = GetCodecVolumeMax();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // GetCodecVolumeMin
				var _return = GetCodecVolumeMin();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // SetCodecActiveTarget
				SetCodecActiveTarget(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetCodecActiveTarget
				var _return = GetCodecActiveTarget();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // BindCodecHeadphoneMicJackInterrupt
				var _return = BindCodecHeadphoneMicJackInterrupt();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // IsCodecHeadphoneMicJackInserted
				var _return = IsCodecHeadphoneMicJackInserted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // ClearCodecHeadphoneMicJackInterrupt
				ClearCodecHeadphoneMicJackInterrupt();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // IsCodecDeviceRequested
				var _return = IsCodecDeviceRequested();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.ICodecController");
		}
	}
}

public partial class IFinalOutputRecorder : _IFinalOutputRecorder_Base;
public abstract class _IFinalOutputRecorder_Base : IpcInterface {
	protected virtual uint GetFinalOutputRecorderState() =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetFinalOutputRecorderState not implemented");
	protected virtual void StartFinalOutputRecorder() =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.StartFinalOutputRecorder".Log();
	protected virtual void StopFinalOutputRecorder() =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.StopFinalOutputRecorder".Log();
	protected virtual void AppendFinalOutputRecorderBuffer(ulong _0, Span<Nn.Audio.AudioInBuffer> _1) =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.AppendFinalOutputRecorderBuffer".Log();
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedFinalOutputRecorderBuffer(out uint _0, out ulong _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetReleasedFinalOutputRecorderBuffer not implemented");
	protected virtual byte ContainsFinalOutputRecorderBuffer(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.ContainsFinalOutputRecorderBuffer not implemented");
	protected virtual ulong Unknown7(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.Unknown7 not implemented");
	protected virtual void AppendFinalOutputRecorderBufferAuto(ulong _0, Span<Nn.Audio.AudioInBuffer> _1) =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.AppendFinalOutputRecorderBufferAuto".Log();
	protected virtual void GetReleasedFinalOutputRecorderBufferAuto(out uint _0, out ulong _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetReleasedFinalOutputRecorderBufferAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetFinalOutputRecorderState
				var _return = GetFinalOutputRecorderState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartFinalOutputRecorder
				StartFinalOutputRecorder();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // StopFinalOutputRecorder
				StopFinalOutputRecorder();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // AppendFinalOutputRecorderBuffer
				AppendFinalOutputRecorderBuffer(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // RegisterBufferEvent
				var _return = RegisterBufferEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetReleasedFinalOutputRecorderBuffer
				GetReleasedFinalOutputRecorderBuffer(out var _0, out var _1, im.GetSpan<Nn.Audio.AudioInBuffer>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x6: { // ContainsFinalOutputRecorderBuffer
				var _return = ContainsFinalOutputRecorderBuffer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // Unknown7
				var _return = Unknown7(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // AppendFinalOutputRecorderBufferAuto
				AppendFinalOutputRecorderBufferAuto(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetReleasedFinalOutputRecorderBufferAuto
				GetReleasedFinalOutputRecorderBufferAuto(out var _0, out var _1, im.GetSpan<Nn.Audio.AudioInBuffer>(0x22, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorder");
		}
	}
}

public partial class IFinalOutputRecorderManager : _IFinalOutputRecorderManager_Base {
	public readonly string ServiceName;
	public IFinalOutputRecorderManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFinalOutputRecorderManager_Base : IpcInterface {
	protected virtual void OpenFinalOutputRecorder(byte[] _0, ulong _1, KObject _2, out byte[] _3, out Nn.Audio.Detail.IFinalOutputRecorder _4) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorderManager.OpenFinalOutputRecorder not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenFinalOutputRecorder
				OpenFinalOutputRecorder(im.GetBytes(8, 0x8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)), out var _0, out var _1);
				om.Initialize(1, 0, 16);
				om.SetBytes(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManager");
		}
	}
}

public partial class IFinalOutputRecorderManagerForApplet : _IFinalOutputRecorderManagerForApplet_Base {
	public readonly string ServiceName;
	public IFinalOutputRecorderManagerForApplet(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFinalOutputRecorderManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendFinalOutputRecorders(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForApplet.RequestSuspendFinalOutputRecorders".Log();
	protected virtual void RequestResumeFinalOutputRecorders(ulong _0, ulong _1) =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForApplet.RequestResumeFinalOutputRecorders".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendFinalOutputRecorders
				RequestSuspendFinalOutputRecorders(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeFinalOutputRecorders
				RequestResumeFinalOutputRecorders(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManagerForApplet");
		}
	}
}

public partial class IFinalOutputRecorderManagerForDebugger : _IFinalOutputRecorderManagerForDebugger_Base {
	public readonly string ServiceName;
	public IFinalOutputRecorderManagerForDebugger(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFinalOutputRecorderManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger.RequestSuspendForDebug".Log();
	protected virtual void RequestResumeForDebug(ulong _0) =>
		"Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger.RequestResumeForDebug".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendForDebug
				RequestSuspendForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // RequestResumeForDebug
				RequestResumeForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger");
		}
	}
}

