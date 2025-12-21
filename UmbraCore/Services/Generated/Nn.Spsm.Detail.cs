using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Spsm.Detail;
public partial class IPowerStateInterface : _IPowerStateInterface_Base;
public abstract class _IPowerStateInterface_Base : IpcInterface {
	protected virtual void GetState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.GetState not implemented");
	protected virtual KObject SleepSystemAndWaitAwake() =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.SleepSystemAndWaitAwake not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown3");
	protected virtual KObject GetNotificationMessageEventHandle() =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.GetNotificationMessageEventHandle not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.Unknown6 not implemented");
	protected virtual void Unknown7() =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown7");
	protected virtual void AnalyzePerformanceLogForLastSleepWakeSequence(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spsm.Detail.IPowerStateInterface.AnalyzePerformanceLogForLastSleepWakeSequence not implemented");
	protected virtual void ChangeHomeButtonLongPressingTime(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.ChangeHomeButtonLongPressingTime");
	protected virtual void Unknown10() =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown10");
	protected virtual void Unknown11(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spsm.Detail.IPowerStateInterface.Unknown11");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetState
				break;
			}
			case 0x1: { // SleepSystemAndWaitAwake
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // GetNotificationMessageEventHandle
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0x8: { // AnalyzePerformanceLogForLastSleepWakeSequence
				break;
			}
			case 0x9: { // ChangeHomeButtonLongPressingTime
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spsm.Detail.IPowerStateInterface");
		}
	}
}

