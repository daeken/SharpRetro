using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audio.Detail;
public partial class IAudioDebugManager : _IAudioDebugManager_Base;
public abstract class _IAudioDebugManager_Base : IpcInterface {
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

