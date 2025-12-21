using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audio.Detail;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AudioRendererUpdateDataHeader {
	public int Revision;
	public int BehaviorSize;
	public int MemoryPoolSize;
	public int VoiceSize;
	public int VoiceResourceSize;
	public int EffectSize;
	public int MixSize;
	public int SinkSize;
	public int PerformanceManagerSize;
	public int Unknown24;
	public int Unknown28;
	public int Unknown2C;
	public int Unknown30;
	public int Unknown34;
	public int Unknown38;
	public int TotalSize;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AudioRendererParameterInternal {
	public int SampleRate;
	public int SampleCount;
	public int Unknown8;
	public int MixCount;
	public int VoiceCount;
	public int SinkCount;
	public int EffectCount;
	public int PerformanceManagerCount;
	public int VoiceDropEnable;
	public int SplitterCount;
	public int SplitterDestinationDataCount;
	public int Unknown2C;
	public int Revision;
}
public partial class IAudioDebugManager : _IAudioDebugManager_Base;
public abstract class _IAudioDebugManager_Base : IpcInterface {
	protected virtual void Unknown0(uint _0, ulong _1, KObject _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown2");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDebugManager.Unknown3");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioDebugManager");
		}
	}
}

public partial class IAudioDevice : _IAudioDevice_Base;
public abstract class _IAudioDevice_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown0 not implemented");
	protected virtual void Unknown1(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDevice.Unknown1");
	protected virtual uint Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown3 not implemented");
	protected virtual KObject Unknown4() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown4 not implemented");
	protected virtual uint Unknown5() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown5 not implemented");
	protected virtual void Unknown6() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown6 not implemented");
	protected virtual void Unknown7(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDevice.Unknown7");
	protected virtual uint Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown8 not implemented");
	protected virtual void Unknown10() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown10 not implemented");
	protected virtual KObject Unknown11() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown11 not implemented");
	protected virtual KObject Unknown12() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown12 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
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
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.StartAudioIn");
	protected virtual void StopAudioIn() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.StopAudioIn");
	protected virtual void AppendAudioInBuffer(ulong tag, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBuffer");
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedAudioInBuffer() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetReleasedAudioInBuffer not implemented");
	protected virtual byte ContainsAudioInBuffer(ulong tag) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.ContainsAudioInBuffer not implemented");
	protected virtual void AppendAudioInBufferWithUserEvent(ulong tag, KObject _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferWithUserEvent");
	protected virtual void AppendAudioInBufferAuto(ulong tag, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferAuto");
	protected virtual void GetReleasedAudioInBufferAuto() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetReleasedAudioInBufferAuto not implemented");
	protected virtual void AppendAudioInBufferWithUserEventAuto(ulong tag, KObject _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferWithUserEventAuto");
	protected virtual uint GetAudioInBufferCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInBufferCount not implemented");
	protected virtual void SetAudioInDeviceGain(uint gain) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.SetAudioInDeviceGain");
	protected virtual uint GetAudioInDeviceGain() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInDeviceGain not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetAudioInState
				break;
			case 0x1: // StartAudioIn
				break;
			case 0x2: // StopAudioIn
				break;
			case 0x3: // AppendAudioInBuffer
				break;
			case 0x4: // RegisterBufferEvent
				break;
			case 0x5: // GetReleasedAudioInBuffer
				break;
			case 0x6: // ContainsAudioInBuffer
				break;
			case 0x7: // AppendAudioInBufferWithUserEvent
				break;
			case 0x8: // AppendAudioInBufferAuto
				break;
			case 0x9: // GetReleasedAudioInBufferAuto
				break;
			case 0xA: // AppendAudioInBufferWithUserEventAuto
				break;
			case 0xB: // GetAudioInBufferCount
				break;
			case 0xC: // SetAudioInDeviceGain
				break;
			case 0xD: // GetAudioInDeviceGain
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioIn");
		}
	}
}

public partial class IAudioInManager : _IAudioInManager_Base;
public abstract class _IAudioInManager_Base : IpcInterface {
	protected virtual void ListAudioIns() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.ListAudioIns not implemented");
	protected virtual void OpenAudioIn(ulong _0, ulong pid_copy, ulong _2, KObject _3, Span<byte> name) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.OpenAudioIn not implemented");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.Unknown2 not implemented");
	protected virtual void OpenAudioInAuto(ulong _0, ulong pid_copy, ulong _2, KObject _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.OpenAudioInAuto not implemented");
	protected virtual void ListAudioInsAuto() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManager.ListAudioInsAuto not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ListAudioIns
				break;
			case 0x1: // OpenAudioIn
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // OpenAudioInAuto
				break;
			case 0x4: // ListAudioInsAuto
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioInManager");
		}
	}
}

public partial class IAudioInManagerForApplet : _IAudioInManagerForApplet_Base;
public abstract class _IAudioInManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendAudioIns(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioInManagerForApplet.RequestSuspendAudioIns");
	protected virtual void RequestResumeAudioIns(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioInManagerForApplet.RequestResumeAudioIns");
	protected virtual uint GetAudioInsProcessMasterVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioInManagerForApplet.GetAudioInsProcessMasterVolume not implemented");
	protected virtual void SetAudioInsProcessMasterVolume(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioInManagerForApplet.SetAudioInsProcessMasterVolume");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendAudioIns
				break;
			case 0x1: // RequestResumeAudioIns
				break;
			case 0x2: // GetAudioInsProcessMasterVolume
				break;
			case 0x3: // SetAudioInsProcessMasterVolume
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioInManagerForApplet");
		}
	}
}

public partial class IAudioInManagerForDebugger : _IAudioInManagerForDebugger_Base;
public abstract class _IAudioInManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendAudioInsForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioInManagerForDebugger.RequestSuspendAudioInsForDebug");
	protected virtual void RequestResumeAudioInsForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioInManagerForDebugger.RequestResumeAudioInsForDebug");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendAudioInsForDebug
				break;
			case 0x1: // RequestResumeAudioInsForDebug
				break;
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
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.StartAudioOut");
	protected virtual void StopAudioOut() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.StopAudioOut");
	protected virtual void AppendAudioOutBuffer(ulong tag, Span<Nn.Audio.AudioOutBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.AppendAudioOutBuffer");
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedAudioOutBuffer() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetReleasedAudioOutBuffer not implemented");
	protected virtual byte ContainsAudioOutBuffer(ulong tag) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.ContainsAudioOutBuffer not implemented");
	protected virtual void AppendAudioOutBufferAuto(ulong tag, Span<Nn.Audio.AudioOutBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.AppendAudioOutBufferAuto");
	protected virtual void GetReleasedAudioOutBufferAuto() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetReleasedAudioOutBufferAuto not implemented");
	protected virtual uint GetAudioOutBufferCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetAudioOutBufferCount not implemented");
	protected virtual ulong GetAudioOutPlayedSampleCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetAudioOutPlayedSampleCount not implemented");
	protected virtual byte FlushAudioOutBuffers() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.FlushAudioOutBuffers not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetAudioOutState
				break;
			case 0x1: // StartAudioOut
				break;
			case 0x2: // StopAudioOut
				break;
			case 0x3: // AppendAudioOutBuffer
				break;
			case 0x4: // RegisterBufferEvent
				break;
			case 0x5: // GetReleasedAudioOutBuffer
				break;
			case 0x6: // ContainsAudioOutBuffer
				break;
			case 0x7: // AppendAudioOutBufferAuto
				break;
			case 0x8: // GetReleasedAudioOutBufferAuto
				break;
			case 0x9: // GetAudioOutBufferCount
				break;
			case 0xA: // GetAudioOutPlayedSampleCount
				break;
			case 0xB: // FlushAudioOutBuffers
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOut");
		}
	}
}

public partial class IAudioOutManager : _IAudioOutManager_Base;
public abstract class _IAudioOutManager_Base : IpcInterface {
	protected virtual void ListAudioOuts() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.ListAudioOuts not implemented");
	protected virtual void OpenAudioOut(uint sample_rate, ushort unused, ushort channel_count, ulong _3, ulong _4, KObject _5, Span<byte> name_in) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.OpenAudioOut not implemented");
	protected virtual void ListAudioOutsAuto() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.ListAudioOutsAuto not implemented");
	protected virtual void OpenAudioOutAuto(uint sample_rate, ushort unused, ushort channel_count, ulong _3, ulong _4, KObject _5, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.OpenAudioOutAuto not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ListAudioOuts
				break;
			case 0x1: // OpenAudioOut
				break;
			case 0x2: // ListAudioOutsAuto
				break;
			case 0x3: // OpenAudioOutAuto
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOutManager");
		}
	}
}

public partial class IAudioOutManagerForApplet : _IAudioOutManagerForApplet_Base;
public abstract class _IAudioOutManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendAudioOuts(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.RequestSuspendAudioOuts");
	protected virtual void RequestResumeAudioOuts(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.RequestResumeAudioOuts");
	protected virtual uint GetAudioOutsProcessMasterVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManagerForApplet.GetAudioOutsProcessMasterVolume not implemented");
	protected virtual void SetAudioOutsProcessMasterVolume(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.SetAudioOutsProcessMasterVolume");
	protected virtual uint GetAudioOutsProcessRecordVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManagerForApplet.GetAudioOutsProcessRecordVolume not implemented");
	protected virtual void SetAudioOutsProcessRecordVolume(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOutManagerForApplet.SetAudioOutsProcessRecordVolume");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendAudioOuts
				break;
			case 0x1: // RequestResumeAudioOuts
				break;
			case 0x2: // GetAudioOutsProcessMasterVolume
				break;
			case 0x3: // SetAudioOutsProcessMasterVolume
				break;
			case 0x4: // GetAudioOutsProcessRecordVolume
				break;
			case 0x5: // SetAudioOutsProcessRecordVolume
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOutManagerForApplet");
		}
	}
}

public partial class IAudioOutManagerForDebugger : _IAudioOutManagerForDebugger_Base;
public abstract class _IAudioOutManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendAudioOutsForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOutManagerForDebugger.RequestSuspendAudioOutsForDebug");
	protected virtual void RequestResumeAudioOutsForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOutManagerForDebugger.RequestResumeAudioOutsForDebug");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendAudioOutsForDebug
				break;
			case 0x1: // RequestResumeAudioOutsForDebug
				break;
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
	protected virtual void RequestUpdateAudioRenderer(Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.RequestUpdateAudioRenderer not implemented");
	protected virtual void Start() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRenderer.Start");
	protected virtual void Stop() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRenderer.Stop");
	protected virtual KObject QuerySystemEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.QuerySystemEvent not implemented");
	protected virtual void SetAudioRendererRenderingTimeLimit(uint limit) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRenderer.SetAudioRendererRenderingTimeLimit");
	protected virtual uint GetAudioRendererRenderingTimeLimit() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.GetAudioRendererRenderingTimeLimit not implemented");
	protected virtual void RequestUpdateAudioRendererAuto(Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.RequestUpdateAudioRendererAuto not implemented");
	protected virtual void ExecuteAudioRendererRendering() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRenderer.ExecuteAudioRendererRendering");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSampleRate
				break;
			case 0x1: // GetSampleCount
				break;
			case 0x2: // GetMixBufferCount
				break;
			case 0x3: // GetState
				break;
			case 0x4: // RequestUpdateAudioRenderer
				break;
			case 0x5: // Start
				break;
			case 0x6: // Stop
				break;
			case 0x7: // QuerySystemEvent
				break;
			case 0x8: // SetAudioRendererRenderingTimeLimit
				break;
			case 0x9: // GetAudioRendererRenderingTimeLimit
				break;
			case 0xA: // RequestUpdateAudioRendererAuto
				break;
			case 0xB: // ExecuteAudioRendererRendering
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRenderer");
		}
	}
}

public partial class IAudioRendererManager : _IAudioRendererManager_Base;
public abstract class _IAudioRendererManager_Base : IpcInterface {
	protected virtual Nn.Audio.Detail.IAudioRenderer OpenAudioRenderer(Nn.Audio.Detail.AudioRendererParameterInternal _0, ulong _1, ulong _2, ulong _3, KObject _4, KObject _5) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.OpenAudioRenderer not implemented");
	protected virtual ulong GetWorkBufferSize(Nn.Audio.Detail.AudioRendererParameterInternal _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.GetWorkBufferSize not implemented");
	protected virtual Nn.Audio.Detail.IAudioDevice GetAudioDeviceService(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.GetAudioDeviceService not implemented");
	protected virtual Nn.Audio.Detail.IAudioRenderer OpenAudioRendererAuto(Nn.Audio.Detail.AudioRendererParameterInternal _0, ulong _1, ulong _2, ulong _3, ulong _4, KObject _5) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.OpenAudioRendererAuto not implemented");
	protected virtual Nn.Audio.Detail.IAudioDevice GetAudioDeviceServiceWithRevisionInfo(ulong _0, uint _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManager.GetAudioDeviceServiceWithRevisionInfo not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenAudioRenderer
				break;
			case 0x1: // GetWorkBufferSize
				break;
			case 0x2: // GetAudioDeviceService
				break;
			case 0x3: // OpenAudioRendererAuto
				break;
			case 0x4: // GetAudioDeviceServiceWithRevisionInfo
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRendererManager");
		}
	}
}

public partial class IAudioRendererManagerForApplet : _IAudioRendererManagerForApplet_Base;
public abstract class _IAudioRendererManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendAudioRenderers(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.RequestSuspendAudioRenderers");
	protected virtual void RequestResumeAudioRenderers(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.RequestResumeAudioRenderers");
	protected virtual uint GetAudioRenderersProcessMasterVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManagerForApplet.GetAudioRenderersProcessMasterVolume not implemented");
	protected virtual void SetAudioRenderersProcessMasterVolume(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.SetAudioRenderersProcessMasterVolume");
	protected virtual void RegisterAppletResourceUserId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.RegisterAppletResourceUserId");
	protected virtual void UnregisterAppletResourceUserId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.UnregisterAppletResourceUserId");
	protected virtual uint GetAudioRenderersProcessRecordVolume(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRendererManagerForApplet.GetAudioRenderersProcessRecordVolume not implemented");
	protected virtual void SetAudioRenderersProcessRecordVolume(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForApplet.SetAudioRenderersProcessRecordVolume");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendAudioRenderers
				break;
			case 0x1: // RequestResumeAudioRenderers
				break;
			case 0x2: // GetAudioRenderersProcessMasterVolume
				break;
			case 0x3: // SetAudioRenderersProcessMasterVolume
				break;
			case 0x4: // RegisterAppletResourceUserId
				break;
			case 0x5: // UnregisterAppletResourceUserId
				break;
			case 0x6: // GetAudioRenderersProcessRecordVolume
				break;
			case 0x7: // SetAudioRenderersProcessRecordVolume
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRendererManagerForApplet");
		}
	}
}

public partial class IAudioRendererManagerForDebugger : _IAudioRendererManagerForDebugger_Base;
public abstract class _IAudioRendererManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForDebugger.RequestSuspendForDebug");
	protected virtual void RequestResumeForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRendererManagerForDebugger.RequestResumeForDebug");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendForDebug
				break;
			case 0x1: // RequestResumeForDebug
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioRendererManagerForDebugger");
		}
	}
}

public partial class ICodecController : _ICodecController_Base;
public abstract class _ICodecController_Base : IpcInterface {
	protected virtual void InitializeCodecController() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.InitializeCodecController");
	protected virtual void FinalizeCodecController() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.FinalizeCodecController");
	protected virtual void SleepCodecController() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.SleepCodecController");
	protected virtual void WakeCodecController() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.WakeCodecController");
	protected virtual void SetCodecVolume(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.SetCodecVolume");
	protected virtual uint GetCodecVolumeMax() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.GetCodecVolumeMax not implemented");
	protected virtual uint GetCodecVolumeMin() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.GetCodecVolumeMin not implemented");
	protected virtual void SetCodecActiveTarget(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.SetCodecActiveTarget");
	protected virtual uint GetCodecActiveTarget() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.GetCodecActiveTarget not implemented");
	protected virtual KObject BindCodecHeadphoneMicJackInterrupt() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.BindCodecHeadphoneMicJackInterrupt not implemented");
	protected virtual byte IsCodecHeadphoneMicJackInserted() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.IsCodecHeadphoneMicJackInserted not implemented");
	protected virtual void ClearCodecHeadphoneMicJackInterrupt() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.ICodecController.ClearCodecHeadphoneMicJackInterrupt");
	protected virtual byte IsCodecDeviceRequested() =>
		throw new NotImplementedException("Nn.Audio.Detail.ICodecController.IsCodecDeviceRequested not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // InitializeCodecController
				break;
			case 0x1: // FinalizeCodecController
				break;
			case 0x2: // SleepCodecController
				break;
			case 0x3: // WakeCodecController
				break;
			case 0x4: // SetCodecVolume
				break;
			case 0x5: // GetCodecVolumeMax
				break;
			case 0x6: // GetCodecVolumeMin
				break;
			case 0x7: // SetCodecActiveTarget
				break;
			case 0x8: // GetCodecActiveTarget
				break;
			case 0x9: // BindCodecHeadphoneMicJackInterrupt
				break;
			case 0xA: // IsCodecHeadphoneMicJackInserted
				break;
			case 0xB: // ClearCodecHeadphoneMicJackInterrupt
				break;
			case 0xC: // IsCodecDeviceRequested
				break;
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
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.StartFinalOutputRecorder");
	protected virtual void StopFinalOutputRecorder() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.StopFinalOutputRecorder");
	protected virtual void AppendFinalOutputRecorderBuffer(ulong _0, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.AppendFinalOutputRecorderBuffer");
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedFinalOutputRecorderBuffer() =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetReleasedFinalOutputRecorderBuffer not implemented");
	protected virtual byte ContainsFinalOutputRecorderBuffer(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.ContainsFinalOutputRecorderBuffer not implemented");
	protected virtual ulong Unknown7(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.Unknown7 not implemented");
	protected virtual void AppendFinalOutputRecorderBufferAuto(ulong _0, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.AppendFinalOutputRecorderBufferAuto");
	protected virtual void GetReleasedFinalOutputRecorderBufferAuto() =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetReleasedFinalOutputRecorderBufferAuto not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetFinalOutputRecorderState
				break;
			case 0x1: // StartFinalOutputRecorder
				break;
			case 0x2: // StopFinalOutputRecorder
				break;
			case 0x3: // AppendFinalOutputRecorderBuffer
				break;
			case 0x4: // RegisterBufferEvent
				break;
			case 0x5: // GetReleasedFinalOutputRecorderBuffer
				break;
			case 0x6: // ContainsFinalOutputRecorderBuffer
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // AppendFinalOutputRecorderBufferAuto
				break;
			case 0x9: // GetReleasedFinalOutputRecorderBufferAuto
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorder");
		}
	}
}

public partial class IFinalOutputRecorderManager : _IFinalOutputRecorderManager_Base;
public abstract class _IFinalOutputRecorderManager_Base : IpcInterface {
	protected virtual void OpenFinalOutputRecorder(Span<byte> _0, ulong _1, KObject _2) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorderManager.OpenFinalOutputRecorder not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenFinalOutputRecorder
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManager");
		}
	}
}

public partial class IFinalOutputRecorderManagerForApplet : _IFinalOutputRecorderManagerForApplet_Base;
public abstract class _IFinalOutputRecorderManagerForApplet_Base : IpcInterface {
	protected virtual void RequestSuspendFinalOutputRecorders(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForApplet.RequestSuspendFinalOutputRecorders");
	protected virtual void RequestResumeFinalOutputRecorders(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForApplet.RequestResumeFinalOutputRecorders");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendFinalOutputRecorders
				break;
			case 0x1: // RequestResumeFinalOutputRecorders
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManagerForApplet");
		}
	}
}

public partial class IFinalOutputRecorderManagerForDebugger : _IFinalOutputRecorderManagerForDebugger_Base;
public abstract class _IFinalOutputRecorderManagerForDebugger_Base : IpcInterface {
	protected virtual void RequestSuspendForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger.RequestSuspendForDebug");
	protected virtual void RequestResumeForDebug(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger.RequestResumeForDebug");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestSuspendForDebug
				break;
			case 0x1: // RequestResumeForDebug
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger");
		}
	}
}

