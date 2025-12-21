using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Prepo.Detail.Ipc;
public partial class IPrepoService : _IPrepoService_Base;
public abstract class _IPrepoService_Base : IpcInterface {
	protected virtual void SaveReport(ulong _0, ulong _1, Span<byte> _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.SaveReport");
	protected virtual void SaveReportWithUser(Span<byte> _0, ulong _1, ulong _2, Span<byte> _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.SaveReportWithUser");
	protected virtual void RequestImmediateTransmission() =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.RequestImmediateTransmission");
	protected virtual uint GetTransmissionStatus() =>
		throw new NotImplementedException("Nn.Prepo.Detail.Ipc.IPrepoService.GetTransmissionStatus not implemented");
	protected virtual void SaveSystemReport(ulong _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.SaveSystemReport");
	protected virtual void SaveSystemReportWithUser(Span<byte> _0, ulong _1, Span<byte> _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.SaveSystemReportWithUser");
	protected virtual void SetOperationMode(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.SetOperationMode");
	protected virtual void ClearStorage() =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.ClearStorage");
	protected virtual byte IsUserAgreementCheckEnabled() =>
		throw new NotImplementedException("Nn.Prepo.Detail.Ipc.IPrepoService.IsUserAgreementCheckEnabled not implemented");
	protected virtual void SetUserAgreementCheckEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.SetUserAgreementCheckEnabled");
	protected virtual void GetStorageUsage(out ulong _0, out ulong _1) =>
		throw new NotImplementedException("Nn.Prepo.Detail.Ipc.IPrepoService.GetStorageUsage not implemented");
	protected virtual void GetStatistics() =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.GetStatistics");
	protected virtual void GetThroughputHistory() =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.GetThroughputHistory");
	protected virtual void GetLastUploadError() =>
		Console.WriteLine("Stub hit for Nn.Prepo.Detail.Ipc.IPrepoService.GetLastUploadError");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: { // SaveReport
				break;
			}
			case 0x2775: { // SaveReportWithUser
				break;
			}
			case 0x27D8: { // RequestImmediateTransmission
				break;
			}
			case 0x283C: { // GetTransmissionStatus
				break;
			}
			case 0x4E84: { // SaveSystemReport
				break;
			}
			case 0x4E85: { // SaveSystemReportWithUser
				break;
			}
			case 0x4EE8: { // SetOperationMode
				break;
			}
			case 0x7594: { // ClearStorage
				break;
			}
			case 0x9CA4: { // IsUserAgreementCheckEnabled
				break;
			}
			case 0x9CA5: { // SetUserAgreementCheckEnabled
				break;
			}
			case 0x15FF4: { // GetStorageUsage
				break;
			}
			case 0x16058: { // GetStatistics
				break;
			}
			case 0x16059: { // GetThroughputHistory
				break;
			}
			case 0x160BC: { // GetLastUploadError
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Prepo.Detail.Ipc.IPrepoService");
		}
	}
}

