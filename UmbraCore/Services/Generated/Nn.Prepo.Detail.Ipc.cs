using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Prepo.Detail.Ipc;
public partial class IPrepoService : _IPrepoService_Base;
public abstract class _IPrepoService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2774: // SaveReport
				break;
			case 0x2775: // SaveReportWithUser
				break;
			case 0x27D8: // RequestImmediateTransmission
				break;
			case 0x283C: // GetTransmissionStatus
				break;
			case 0x4E84: // SaveSystemReport
				break;
			case 0x4E85: // SaveSystemReportWithUser
				break;
			case 0x4EE8: // SetOperationMode
				break;
			case 0x7594: // ClearStorage
				break;
			case 0x9CA4: // IsUserAgreementCheckEnabled
				break;
			case 0x9CA5: // SetUserAgreementCheckEnabled
				break;
			case 0x15FF4: // GetStorageUsage
				break;
			case 0x16058: // GetStatistics
				break;
			case 0x16059: // GetThroughputHistory
				break;
			case 0x160BC: // GetLastUploadError
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Prepo.Detail.Ipc.IPrepoService");
		}
	}
}

