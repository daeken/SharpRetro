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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetData<uint>(8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioDebugManager");
		}
	}
}

public partial class IAudioDevice : _IAudioDevice_Base;
public abstract class _IAudioDevice_Base : IpcInterface {
	protected virtual void Unknown0(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown0 not implemented");
	protected virtual void Unknown1(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDevice.Unknown1");
	protected virtual uint Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown3 not implemented");
	protected virtual KObject Unknown4() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown4 not implemented");
	protected virtual uint Unknown5() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown5 not implemented");
	protected virtual void Unknown6(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown6 not implemented");
	protected virtual void Unknown7(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioDevice.Unknown7");
	protected virtual uint Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown8 not implemented");
	protected virtual void Unknown10(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown10 not implemented");
	protected virtual KObject Unknown11() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown11 not implemented");
	protected virtual KObject Unknown12() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioDevice.Unknown12 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 4);
				Unknown0(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 4);
				var _return = Unknown2(im.GetSpan<byte>(0x5, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 1, 0);
				var _return = Unknown4();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 4);
				var _return = Unknown5();
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 4);
				Unknown6(out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 0);
				Unknown7(im.GetData<uint>(8), im.GetSpan<byte>(0x21, 0));
				break;
			}
			case 0x8: { // Unknown8
				om.Initialize(0, 0, 4);
				var _return = Unknown8(im.GetSpan<byte>(0x21, 0));
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 0);
				Unknown10(im.GetSpan<byte>(0x22, 0));
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 1, 0);
				var _return = Unknown11();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xC: { // Unknown12
				om.Initialize(0, 1, 0);
				var _return = Unknown12();
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
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.StartAudioIn");
	protected virtual void StopAudioIn() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.StopAudioIn");
	protected virtual void AppendAudioInBuffer(ulong tag, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBuffer");
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedAudioInBuffer(out uint count, Span<Nn.Audio.AudioInBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetReleasedAudioInBuffer not implemented");
	protected virtual byte ContainsAudioInBuffer(ulong tag) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.ContainsAudioInBuffer not implemented");
	protected virtual void AppendAudioInBufferWithUserEvent(ulong tag, KObject _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferWithUserEvent");
	protected virtual void AppendAudioInBufferAuto(ulong tag, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferAuto");
	protected virtual void GetReleasedAudioInBufferAuto(out uint count, Span<Nn.Audio.AudioInBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetReleasedAudioInBufferAuto not implemented");
	protected virtual void AppendAudioInBufferWithUserEventAuto(ulong tag, KObject _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.AppendAudioInBufferWithUserEventAuto");
	protected virtual uint GetAudioInBufferCount() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInBufferCount not implemented");
	protected virtual void SetAudioInDeviceGain(uint gain) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioIn.SetAudioInDeviceGain");
	protected virtual uint GetAudioInDeviceGain() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioIn.GetAudioInDeviceGain not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetAudioInState
				om.Initialize(0, 0, 4);
				var _return = GetAudioInState();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartAudioIn
				om.Initialize(0, 0, 0);
				StartAudioIn();
				break;
			}
			case 0x2: { // StopAudioIn
				om.Initialize(0, 0, 0);
				StopAudioIn();
				break;
			}
			case 0x3: { // AppendAudioInBuffer
				om.Initialize(0, 0, 0);
				AppendAudioInBuffer(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x5, 0));
				break;
			}
			case 0x4: { // RegisterBufferEvent
				om.Initialize(0, 1, 0);
				var _return = RegisterBufferEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetReleasedAudioInBuffer
				om.Initialize(0, 0, 4);
				GetReleasedAudioInBuffer(out var _0, im.GetSpan<Nn.Audio.AudioInBuffer>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // ContainsAudioInBuffer
				om.Initialize(0, 0, 1);
				var _return = ContainsAudioInBuffer(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // AppendAudioInBufferWithUserEvent
				om.Initialize(0, 0, 0);
				AppendAudioInBufferWithUserEvent(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<Nn.Audio.AudioInBuffer>(0x5, 0));
				break;
			}
			case 0x8: { // AppendAudioInBufferAuto
				om.Initialize(0, 0, 0);
				AppendAudioInBufferAuto(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x21, 0));
				break;
			}
			case 0x9: { // GetReleasedAudioInBufferAuto
				om.Initialize(0, 0, 4);
				GetReleasedAudioInBufferAuto(out var _0, im.GetSpan<Nn.Audio.AudioInBuffer>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xA: { // AppendAudioInBufferWithUserEventAuto
				om.Initialize(0, 0, 0);
				AppendAudioInBufferWithUserEventAuto(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<Nn.Audio.AudioInBuffer>(0x21, 0));
				break;
			}
			case 0xB: { // GetAudioInBufferCount
				om.Initialize(0, 0, 4);
				var _return = GetAudioInBufferCount();
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // SetAudioInDeviceGain
				om.Initialize(0, 0, 0);
				SetAudioInDeviceGain(im.GetData<uint>(8));
				break;
			}
			case 0xD: { // GetAudioInDeviceGain
				om.Initialize(0, 0, 4);
				var _return = GetAudioInDeviceGain();
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioIn");
		}
	}
}

public partial class IAudioInManager : _IAudioInManager_Base;
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
				om.Initialize(0, 0, 4);
				ListAudioIns(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // OpenAudioIn
				om.Initialize(1, 0, 16);
				OpenAudioIn(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 4);
				Unknown2(out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // OpenAudioInAuto
				om.Initialize(1, 0, 16);
				OpenAudioInAuto(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x21, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			case 0x4: { // ListAudioInsAuto
				om.Initialize(0, 0, 4);
				ListAudioInsAuto(out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioIns
				om.Initialize(0, 0, 0);
				RequestSuspendAudioIns(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1: { // RequestResumeAudioIns
				om.Initialize(0, 0, 0);
				RequestResumeAudioIns(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2: { // GetAudioInsProcessMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetAudioInsProcessMasterVolume(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetAudioInsProcessMasterVolume
				om.Initialize(0, 0, 0);
				SetAudioInsProcessMasterVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioInsForDebug
				om.Initialize(0, 0, 0);
				RequestSuspendAudioInsForDebug(im.GetData<ulong>(8));
				break;
			}
			case 0x1: { // RequestResumeAudioInsForDebug
				om.Initialize(0, 0, 0);
				RequestResumeAudioInsForDebug(im.GetData<ulong>(8));
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
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.StartAudioOut");
	protected virtual void StopAudioOut() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.StopAudioOut");
	protected virtual void AppendAudioOutBuffer(ulong tag, Span<Nn.Audio.AudioOutBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.AppendAudioOutBuffer");
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedAudioOutBuffer(out uint count, Span<Nn.Audio.AudioOutBuffer> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.GetReleasedAudioOutBuffer not implemented");
	protected virtual byte ContainsAudioOutBuffer(ulong tag) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOut.ContainsAudioOutBuffer not implemented");
	protected virtual void AppendAudioOutBufferAuto(ulong tag, Span<Nn.Audio.AudioOutBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioOut.AppendAudioOutBufferAuto");
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
				om.Initialize(0, 0, 4);
				var _return = GetAudioOutState();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartAudioOut
				om.Initialize(0, 0, 0);
				StartAudioOut();
				break;
			}
			case 0x2: { // StopAudioOut
				om.Initialize(0, 0, 0);
				StopAudioOut();
				break;
			}
			case 0x3: { // AppendAudioOutBuffer
				om.Initialize(0, 0, 0);
				AppendAudioOutBuffer(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioOutBuffer>(0x5, 0));
				break;
			}
			case 0x4: { // RegisterBufferEvent
				om.Initialize(0, 1, 0);
				var _return = RegisterBufferEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetReleasedAudioOutBuffer
				om.Initialize(0, 0, 4);
				GetReleasedAudioOutBuffer(out var _0, im.GetSpan<Nn.Audio.AudioOutBuffer>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // ContainsAudioOutBuffer
				om.Initialize(0, 0, 1);
				var _return = ContainsAudioOutBuffer(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // AppendAudioOutBufferAuto
				om.Initialize(0, 0, 0);
				AppendAudioOutBufferAuto(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioOutBuffer>(0x21, 0));
				break;
			}
			case 0x8: { // GetReleasedAudioOutBufferAuto
				om.Initialize(0, 0, 4);
				GetReleasedAudioOutBufferAuto(out var _0, im.GetSpan<Nn.Audio.AudioOutBuffer>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x9: { // GetAudioOutBufferCount
				om.Initialize(0, 0, 4);
				var _return = GetAudioOutBufferCount();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // GetAudioOutPlayedSampleCount
				om.Initialize(0, 0, 8);
				var _return = GetAudioOutPlayedSampleCount();
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // FlushAudioOutBuffers
				om.Initialize(0, 0, 1);
				var _return = FlushAudioOutBuffers();
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IAudioOut");
		}
	}
}

public partial class IAudioOutManager : _IAudioOutManager_Base;
public abstract class _IAudioOutManager_Base : IpcInterface {
	protected virtual void ListAudioOuts(out uint count, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.ListAudioOuts not implemented");
	protected virtual void OpenAudioOut(uint sample_rate, ushort unused, ushort channel_count, ulong _3, ulong _4, KObject _5, Span<byte> name_in, out uint sample_rate_out, out uint channel_count_out, out uint pcm_format, out uint _10, out Nn.Audio.Detail.IAudioOut _11, Span<byte> name_out) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.OpenAudioOut not implemented");
	protected virtual void ListAudioOutsAuto(out uint count, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.ListAudioOutsAuto not implemented");
	protected virtual void OpenAudioOutAuto(uint sample_rate, ushort unused, ushort channel_count, ulong _3, ulong _4, KObject _5, Span<byte> _6, out uint sample_rate_out, out uint channel_count_out, out uint pcm_format, out uint _10, out Nn.Audio.Detail.IAudioOut _11, Span<byte> name_out) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioOutManager.OpenAudioOutAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ListAudioOuts
				om.Initialize(0, 0, 4);
				ListAudioOuts(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // OpenAudioOut
				om.Initialize(1, 0, 16);
				OpenAudioOut(im.GetData<uint>(8), im.GetData<ushort>(12), im.GetData<ushort>(14), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				om.SetData(20, _3);
				om.Move(0, CreateHandle(_4));
				break;
			}
			case 0x2: { // ListAudioOutsAuto
				om.Initialize(0, 0, 4);
				ListAudioOutsAuto(out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // OpenAudioOutAuto
				om.Initialize(1, 0, 16);
				OpenAudioOutAuto(im.GetData<uint>(8), im.GetData<ushort>(12), im.GetData<ushort>(14), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x21, 0), out var _0, out var _1, out var _2, out var _3, out var _4, im.GetSpan<byte>(0x22, 0));
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioOuts
				om.Initialize(0, 0, 0);
				RequestSuspendAudioOuts(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1: { // RequestResumeAudioOuts
				om.Initialize(0, 0, 0);
				RequestResumeAudioOuts(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2: { // GetAudioOutsProcessMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetAudioOutsProcessMasterVolume(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetAudioOutsProcessMasterVolume
				om.Initialize(0, 0, 0);
				SetAudioOutsProcessMasterVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
			case 0x4: { // GetAudioOutsProcessRecordVolume
				om.Initialize(0, 0, 4);
				var _return = GetAudioOutsProcessRecordVolume(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // SetAudioOutsProcessRecordVolume
				om.Initialize(0, 0, 0);
				SetAudioOutsProcessRecordVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioOutsForDebug
				om.Initialize(0, 0, 0);
				RequestSuspendAudioOutsForDebug(im.GetData<ulong>(8));
				break;
			}
			case 0x1: { // RequestResumeAudioOutsForDebug
				om.Initialize(0, 0, 0);
				RequestResumeAudioOutsForDebug(im.GetData<ulong>(8));
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
	protected virtual void RequestUpdateAudioRenderer(Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _0, Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _1, Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _2) =>
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
	protected virtual void RequestUpdateAudioRendererAuto(Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _0, Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _1, Span<Nn.Audio.Detail.AudioRendererUpdateDataHeader> _2) =>
		throw new NotImplementedException("Nn.Audio.Detail.IAudioRenderer.RequestUpdateAudioRendererAuto not implemented");
	protected virtual void ExecuteAudioRendererRendering() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IAudioRenderer.ExecuteAudioRendererRendering");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSampleRate
				om.Initialize(0, 0, 4);
				var _return = GetSampleRate();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetSampleCount
				om.Initialize(0, 0, 4);
				var _return = GetSampleCount();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetMixBufferCount
				om.Initialize(0, 0, 4);
				var _return = GetMixBufferCount();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetState
				om.Initialize(0, 0, 4);
				var _return = GetState();
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // RequestUpdateAudioRenderer
				om.Initialize(0, 0, 0);
				RequestUpdateAudioRenderer(im.GetSpan<Nn.Audio.Detail.AudioRendererUpdateDataHeader>(0x5, 0), im.GetSpan<Nn.Audio.Detail.AudioRendererUpdateDataHeader>(0x6, 0), im.GetSpan<Nn.Audio.Detail.AudioRendererUpdateDataHeader>(0x6, 1));
				break;
			}
			case 0x5: { // Start
				om.Initialize(0, 0, 0);
				Start();
				break;
			}
			case 0x6: { // Stop
				om.Initialize(0, 0, 0);
				Stop();
				break;
			}
			case 0x7: { // QuerySystemEvent
				om.Initialize(0, 1, 0);
				var _return = QuerySystemEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x8: { // SetAudioRendererRenderingTimeLimit
				om.Initialize(0, 0, 0);
				SetAudioRendererRenderingTimeLimit(im.GetData<uint>(8));
				break;
			}
			case 0x9: { // GetAudioRendererRenderingTimeLimit
				om.Initialize(0, 0, 4);
				var _return = GetAudioRendererRenderingTimeLimit();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // RequestUpdateAudioRendererAuto
				om.Initialize(0, 0, 0);
				RequestUpdateAudioRendererAuto(im.GetSpan<Nn.Audio.Detail.AudioRendererUpdateDataHeader>(0x21, 0), im.GetSpan<Nn.Audio.Detail.AudioRendererUpdateDataHeader>(0x22, 0), im.GetSpan<Nn.Audio.Detail.AudioRendererUpdateDataHeader>(0x22, 1));
				break;
			}
			case 0xB: { // ExecuteAudioRendererRendering
				om.Initialize(0, 0, 0);
				ExecuteAudioRendererRendering();
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenAudioRenderer
				om.Initialize(1, 0, 0);
				var _return = OpenAudioRenderer(*(Nn.Audio.Detail.AudioRendererParameterInternal*) im.GetDataPointer(8), im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)), Kernel.Get<KObject>(im.GetCopy(1)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetWorkBufferSize
				om.Initialize(0, 0, 8);
				var _return = GetWorkBufferSize(*(Nn.Audio.Detail.AudioRendererParameterInternal*) im.GetDataPointer(8));
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetAudioDeviceService
				om.Initialize(1, 0, 0);
				var _return = GetAudioDeviceService(im.GetData<ulong>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // OpenAudioRendererAuto
				om.Initialize(1, 0, 0);
				var _return = OpenAudioRendererAuto(*(Nn.Audio.Detail.AudioRendererParameterInternal*) im.GetDataPointer(8), im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetAudioDeviceServiceWithRevisionInfo
				om.Initialize(1, 0, 0);
				var _return = GetAudioDeviceServiceWithRevisionInfo(im.GetData<ulong>(8), im.GetData<uint>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendAudioRenderers
				om.Initialize(0, 0, 0);
				RequestSuspendAudioRenderers(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1: { // RequestResumeAudioRenderers
				om.Initialize(0, 0, 0);
				RequestResumeAudioRenderers(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2: { // GetAudioRenderersProcessMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetAudioRenderersProcessMasterVolume(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetAudioRenderersProcessMasterVolume
				om.Initialize(0, 0, 0);
				SetAudioRenderersProcessMasterVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
			case 0x4: { // RegisterAppletResourceUserId
				om.Initialize(0, 0, 0);
				RegisterAppletResourceUserId(im.GetData<ulong>(8));
				break;
			}
			case 0x5: { // UnregisterAppletResourceUserId
				om.Initialize(0, 0, 0);
				UnregisterAppletResourceUserId(im.GetData<ulong>(8));
				break;
			}
			case 0x6: { // GetAudioRenderersProcessRecordVolume
				om.Initialize(0, 0, 4);
				var _return = GetAudioRenderersProcessRecordVolume(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // SetAudioRenderersProcessRecordVolume
				om.Initialize(0, 0, 0);
				SetAudioRenderersProcessRecordVolume(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendForDebug
				om.Initialize(0, 0, 0);
				RequestSuspendForDebug(im.GetData<ulong>(8));
				break;
			}
			case 0x1: { // RequestResumeForDebug
				om.Initialize(0, 0, 0);
				RequestResumeForDebug(im.GetData<ulong>(8));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeCodecController
				om.Initialize(0, 0, 0);
				InitializeCodecController();
				break;
			}
			case 0x1: { // FinalizeCodecController
				om.Initialize(0, 0, 0);
				FinalizeCodecController();
				break;
			}
			case 0x2: { // SleepCodecController
				om.Initialize(0, 0, 0);
				SleepCodecController();
				break;
			}
			case 0x3: { // WakeCodecController
				om.Initialize(0, 0, 0);
				WakeCodecController();
				break;
			}
			case 0x4: { // SetCodecVolume
				om.Initialize(0, 0, 0);
				SetCodecVolume(im.GetData<uint>(8));
				break;
			}
			case 0x5: { // GetCodecVolumeMax
				om.Initialize(0, 0, 4);
				var _return = GetCodecVolumeMax();
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // GetCodecVolumeMin
				om.Initialize(0, 0, 4);
				var _return = GetCodecVolumeMin();
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // SetCodecActiveTarget
				om.Initialize(0, 0, 0);
				SetCodecActiveTarget(im.GetData<uint>(8));
				break;
			}
			case 0x8: { // GetCodecActiveTarget
				om.Initialize(0, 0, 4);
				var _return = GetCodecActiveTarget();
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // BindCodecHeadphoneMicJackInterrupt
				om.Initialize(0, 1, 0);
				var _return = BindCodecHeadphoneMicJackInterrupt();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // IsCodecHeadphoneMicJackInserted
				om.Initialize(0, 0, 1);
				var _return = IsCodecHeadphoneMicJackInserted();
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // ClearCodecHeadphoneMicJackInterrupt
				om.Initialize(0, 0, 0);
				ClearCodecHeadphoneMicJackInterrupt();
				break;
			}
			case 0xC: { // IsCodecDeviceRequested
				om.Initialize(0, 0, 1);
				var _return = IsCodecDeviceRequested();
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
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.StartFinalOutputRecorder");
	protected virtual void StopFinalOutputRecorder() =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.StopFinalOutputRecorder");
	protected virtual void AppendFinalOutputRecorderBuffer(ulong _0, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.AppendFinalOutputRecorderBuffer");
	protected virtual KObject RegisterBufferEvent() =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.RegisterBufferEvent not implemented");
	protected virtual void GetReleasedFinalOutputRecorderBuffer(out uint _0, out ulong _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetReleasedFinalOutputRecorderBuffer not implemented");
	protected virtual byte ContainsFinalOutputRecorderBuffer(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.ContainsFinalOutputRecorderBuffer not implemented");
	protected virtual ulong Unknown7(ulong _0) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.Unknown7 not implemented");
	protected virtual void AppendFinalOutputRecorderBufferAuto(ulong _0, Span<Nn.Audio.AudioInBuffer> _1) =>
		Console.WriteLine("Stub hit for Nn.Audio.Detail.IFinalOutputRecorder.AppendFinalOutputRecorderBufferAuto");
	protected virtual void GetReleasedFinalOutputRecorderBufferAuto(out uint _0, out ulong _1, Span<Nn.Audio.AudioInBuffer> _2) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorder.GetReleasedFinalOutputRecorderBufferAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetFinalOutputRecorderState
				om.Initialize(0, 0, 4);
				var _return = GetFinalOutputRecorderState();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartFinalOutputRecorder
				om.Initialize(0, 0, 0);
				StartFinalOutputRecorder();
				break;
			}
			case 0x2: { // StopFinalOutputRecorder
				om.Initialize(0, 0, 0);
				StopFinalOutputRecorder();
				break;
			}
			case 0x3: { // AppendFinalOutputRecorderBuffer
				om.Initialize(0, 0, 0);
				AppendFinalOutputRecorderBuffer(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x5, 0));
				break;
			}
			case 0x4: { // RegisterBufferEvent
				om.Initialize(0, 1, 0);
				var _return = RegisterBufferEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetReleasedFinalOutputRecorderBuffer
				om.Initialize(0, 0, 16);
				GetReleasedFinalOutputRecorderBuffer(out var _0, out var _1, im.GetSpan<Nn.Audio.AudioInBuffer>(0x6, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x6: { // ContainsFinalOutputRecorderBuffer
				om.Initialize(0, 0, 1);
				var _return = ContainsFinalOutputRecorderBuffer(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 8);
				var _return = Unknown7(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // AppendFinalOutputRecorderBufferAuto
				om.Initialize(0, 0, 0);
				AppendFinalOutputRecorderBufferAuto(im.GetData<ulong>(8), im.GetSpan<Nn.Audio.AudioInBuffer>(0x21, 0));
				break;
			}
			case 0x9: { // GetReleasedFinalOutputRecorderBufferAuto
				om.Initialize(0, 0, 16);
				GetReleasedFinalOutputRecorderBufferAuto(out var _0, out var _1, im.GetSpan<Nn.Audio.AudioInBuffer>(0x22, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorder");
		}
	}
}

public partial class IFinalOutputRecorderManager : _IFinalOutputRecorderManager_Base;
public abstract class _IFinalOutputRecorderManager_Base : IpcInterface {
	protected virtual void OpenFinalOutputRecorder(byte[] _0, ulong _1, KObject _2, out byte[] _3, out Nn.Audio.Detail.IFinalOutputRecorder _4) =>
		throw new NotImplementedException("Nn.Audio.Detail.IFinalOutputRecorderManager.OpenFinalOutputRecorder not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenFinalOutputRecorder
				om.Initialize(1, 0, 16);
				OpenFinalOutputRecorder(im.GetBytes(8, 0x8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)), out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendFinalOutputRecorders
				om.Initialize(0, 0, 0);
				RequestSuspendFinalOutputRecorders(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1: { // RequestResumeFinalOutputRecorders
				om.Initialize(0, 0, 0);
				RequestResumeFinalOutputRecorders(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestSuspendForDebug
				om.Initialize(0, 0, 0);
				RequestSuspendForDebug(im.GetData<ulong>(8));
				break;
			}
			case 0x1: { // RequestResumeForDebug
				om.Initialize(0, 0, 0);
				RequestResumeForDebug(im.GetData<ulong>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audio.Detail.IFinalOutputRecorderManagerForDebugger");
		}
	}
}

