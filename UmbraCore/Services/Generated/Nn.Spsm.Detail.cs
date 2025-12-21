using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Spsm.Detail;
public partial class IPowerStateInterface : _IPowerStateInterface_Base;
public abstract class _IPowerStateInterface_Base : IpcInterface {
	protected virtual void GetState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.GetState not implemented");
	protected virtual KObject SleepSystemAndWaitAwake() =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.SleepSystemAndWaitAwake not implemented");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown3");
	protected virtual KObject GetNotificationMessageEventHandle() =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.GetNotificationMessageEventHandle not implemented");
	protected virtual void Unknown5(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.Unknown5 not implemented");
	protected virtual void Unknown6(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.Unknown6 not implemented");
	protected virtual void Unknown7() =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown7");
	protected virtual void AnalyzePerformanceLogForLastSleepWakeSequence(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.AnalyzePerformanceLogForLastSleepWakeSequence not implemented");
	protected virtual void ChangeHomeButtonLongPressingTime(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.ChangeHomeButtonLongPressingTime");
	protected virtual void Unknown10() =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown10");
	protected virtual void Unknown11(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown11");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetState
				om.Initialize(0, 0, 4);
				GetState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // SleepSystemAndWaitAwake
				om.Initialize(0, 1, 0);
				var _return = SleepSystemAndWaitAwake();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 4);
				Unknown2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetBytes(8, 0x1));
				break;
			}
			case 0x4: { // GetNotificationMessageEventHandle
				om.Initialize(0, 1, 0);
				var _return = GetNotificationMessageEventHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 4);
				Unknown5(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 80);
				Unknown6(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 0);
				Unknown7();
				break;
			}
			case 0x8: { // AnalyzePerformanceLogForLastSleepWakeSequence
				om.Initialize(0, 0, 0);
				AnalyzePerformanceLogForLastSleepWakeSequence(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x9: { // ChangeHomeButtonLongPressingTime
				om.Initialize(0, 0, 0);
				ChangeHomeButtonLongPressingTime(im.GetBytes(8, 0x8));
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 0);
				Unknown10();
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 0, 0);
				Unknown11(im.GetBytes(8, 0x8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spsm.Detail.IPowerStateInterface");
		}
	}
}

