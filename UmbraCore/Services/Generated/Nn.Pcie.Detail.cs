using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcie.Detail;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterClassDriver
				break;
			case 0x1: // QueryFunctionsUnregistered
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcie.Detail.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // QueryFunctions
				break;
			case 0x1: // AcquireFunction
				break;
			case 0x2: // ReleaseFunction
				break;
			case 0x3: // GetFunctionState
				break;
			case 0x4: // GetBarProfile
				break;
			case 0x5: // ReadConfig
				break;
			case 0x6: // WriteConfig
				break;
			case 0x7: // ReadBarRegion
				break;
			case 0x8: // WriteBarRegion
				break;
			case 0x9: // FindCapability
				break;
			case 0xA: // FindExtendedCapability
				break;
			case 0xB: // MapDma
				break;
			case 0xC: // UnmapDma
				break;
			case 0xD: // UnmapDmaBusAddress
				break;
			case 0xE: // GetDmaBusAddress
				break;
			case 0xF: // GetDmaBusAddressRange
				break;
			case 0x10: // SetDmaEnable
				break;
			case 0x11: // AcquireIrq
				break;
			case 0x12: // ReleaseIrq
				break;
			case 0x13: // SetIrqEnable
				break;
			case 0x14: // SetAspmEnable
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcie.Detail.ISession");
		}
	}
}

