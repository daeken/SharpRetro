using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Visrv.Sf;
public partial class IApplicationDisplayService : _IApplicationDisplayService_Base;
public abstract class _IApplicationDisplayService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: // GetRelayService
				break;
			case 0x65: // GetSystemDisplayService
				break;
			case 0x66: // GetManagerDisplayService
				break;
			case 0x67: // GetIndirectDisplayTransactionService
				break;
			case 0x3E8: // ListDisplays
				break;
			case 0x3F2: // OpenDisplay
				break;
			case 0x3F3: // OpenDefaultDisplay
				break;
			case 0x3FC: // CloseDisplay
				break;
			case 0x44D: // SetDisplayEnabled
				break;
			case 0x44E: // GetDisplayResolution
				break;
			case 0x7E4: // OpenLayer
				break;
			case 0x7E5: // CloseLayer
				break;
			case 0x7EE: // CreateStrayLayer
				break;
			case 0x7EF: // DestroyStrayLayer
				break;
			case 0x835: // SetLayerScalingMode
				break;
			case 0x836: // ConvertScalingMode
				break;
			case 0x992: // GetIndirectLayerImageMap
				break;
			case 0x993: // GetIndirectLayerImageCropMap
				break;
			case 0x99C: // GetIndirectLayerImageRequiredMemoryInfo
				break;
			case 0x1452: // GetDisplayVsyncEvent
				break;
			case 0x1453: // GetDisplayVsyncEventForDebug
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IApplicationDisplayService");
		}
	}
}

public partial class IApplicationRootService : _IApplicationRootService_Base;
public abstract class _IApplicationRootService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDisplayService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IApplicationRootService");
		}
	}
}

public partial class IManagerDisplayService : _IManagerDisplayService_Base;
public abstract class _IManagerDisplayService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xC8: // AllocateProcessHeapBlock
				break;
			case 0xC9: // FreeProcessHeapBlock
				break;
			case 0x44E: // GetDisplayResolution
				break;
			case 0x7DA: // CreateManagedLayer
				break;
			case 0x7DB: // DestroyManagedLayer
				break;
			case 0x802: // CreateIndirectLayer
				break;
			case 0x803: // DestroyIndirectLayer
				break;
			case 0x804: // CreateIndirectProducerEndPoint
				break;
			case 0x805: // DestroyIndirectProducerEndPoint
				break;
			case 0x806: // CreateIndirectConsumerEndPoint
				break;
			case 0x807: // DestroyIndirectConsumerEndPoint
				break;
			case 0x8FC: // AcquireLayerTexturePresentingEvent
				break;
			case 0x8FD: // ReleaseLayerTexturePresentingEvent
				break;
			case 0x8FE: // GetDisplayHotplugEvent
				break;
			case 0x962: // GetDisplayHotplugState
				break;
			case 0x9C5: // GetCompositorErrorInfo
				break;
			case 0xA29: // GetDisplayErrorEvent
				break;
			case 0x1069: // SetDisplayAlpha
				break;
			case 0x106B: // SetDisplayLayerStack
				break;
			case 0x106D: // SetDisplayPowerState
				break;
			case 0x106E: // SetDefaultDisplay
				break;
			case 0x1770: // AddToLayerStack
				break;
			case 0x1771: // RemoveFromLayerStack
				break;
			case 0x1772: // SetLayerVisibility
				break;
			case 0x1773: // SetLayerConfig
				break;
			case 0x1774: // AttachLayerPresentationTracer
				break;
			case 0x1775: // DetachLayerPresentationTracer
				break;
			case 0x1776: // StartLayerPresentationRecording
				break;
			case 0x1777: // StopLayerPresentationRecording
				break;
			case 0x1778: // StartLayerPresentationFenceWait
				break;
			case 0x1779: // StopLayerPresentationFenceWait
				break;
			case 0x177A: // GetLayerPresentationAllFencesExpiredEvent
				break;
			case 0x1B58: // SetContentVisibility
				break;
			case 0x1F40: // SetConductorLayer
				break;
			case 0x1FA4: // SetIndirectProducerFlipOffset
				break;
			case 0x2008: // CreateSharedBufferStaticStorage
				break;
			case 0x2009: // CreateSharedBufferTransferMemory
				break;
			case 0x200A: // DestroySharedBuffer
				break;
			case 0x200B: // BindSharedLowLevelLayerToManagedLayer
				break;
			case 0x200C: // BindSharedLowLevelLayerToIndirectLayer
				break;
			case 0x200F: // UnbindSharedLowLevelLayer
				break;
			case 0x2010: // ConnectSharedLowLevelLayerToSharedBuffer
				break;
			case 0x2011: // DisconnectSharedLowLevelLayerFromSharedBuffer
				break;
			case 0x2012: // CreateSharedLayer
				break;
			case 0x2013: // DestroySharedLayer
				break;
			case 0x2018: // AttachSharedLayerToLowLevelLayer
				break;
			case 0x2019: // ForceDetachSharedLayerFromLowLevelLayer
				break;
			case 0x201A: // StartDetachSharedLayerFromLowLevelLayer
				break;
			case 0x201B: // FinishDetachSharedLayerFromLowLevelLayer
				break;
			case 0x201C: // GetSharedLayerDetachReadyEvent
				break;
			case 0x201D: // GetSharedLowLevelLayerSynchronizedEvent
				break;
			case 0x201E: // CheckSharedLowLevelLayerSynchronized
				break;
			case 0x201F: // RegisterSharedBufferImporterAruid
				break;
			case 0x2020: // UnregisterSharedBufferImporterAruid
				break;
			case 0x2023: // CreateSharedBufferProcessHeap
				break;
			case 0x2024: // GetSharedLayerLayerStacks
				break;
			case 0x2025: // SetSharedLayerLayerStacks
				break;
			case 0x2063: // PresentDetachedSharedFrameBufferToLowLevelLayer
				break;
			case 0x2064: // FillDetachedSharedFrameBufferColor
				break;
			case 0x2065: // GetDetachedSharedFrameBufferImage
				break;
			case 0x2066: // SetDetachedSharedFrameBufferImage
				break;
			case 0x2067: // CopyDetachedSharedFrameBufferImage
				break;
			case 0x2068: // SetDetachedSharedFrameBufferSubImage
				break;
			case 0x2069: // GetSharedFrameBufferContentParameter
				break;
			case 0x206A: // ExpandStartupLogoOnSharedFrameBuffer
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IManagerDisplayService");
		}
	}
}

public partial class IManagerRootService : _IManagerRootService_Base;
public abstract class _IManagerRootService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2: // GetDisplayService
				break;
			case 0x3: // GetDisplayServiceWithProxyNameExchange
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IManagerRootService");
		}
	}
}

public partial class ISystemDisplayService : _ISystemDisplayService_Base;
public abstract class _ISystemDisplayService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4B0: // GetZOrderCountMin
				break;
			case 0x4B2: // GetZOrderCountMax
				break;
			case 0x4B3: // GetDisplayLogicalResolution
				break;
			case 0x4B4: // SetDisplayMagnification
				break;
			case 0x899: // SetLayerPosition
				break;
			case 0x89B: // SetLayerSize
				break;
			case 0x89C: // GetLayerZ
				break;
			case 0x89D: // SetLayerZ
				break;
			case 0x89F: // SetLayerVisibility
				break;
			case 0x8A1: // SetLayerAlpha
				break;
			case 0x908: // CreateStrayLayer
				break;
			case 0x960: // OpenIndirectLayer
				break;
			case 0x961: // CloseIndirectLayer
				break;
			case 0x962: // FlipIndirectLayer
				break;
			case 0xBB8: // ListDisplayModes
				break;
			case 0xBB9: // ListDisplayRgbRanges
				break;
			case 0xBBA: // ListDisplayContentTypes
				break;
			case 0xC80: // GetDisplayMode
				break;
			case 0xC81: // SetDisplayMode
				break;
			case 0xC82: // GetDisplayUnderscan
				break;
			case 0xC83: // SetDisplayUnderscan
				break;
			case 0xC84: // GetDisplayContentType
				break;
			case 0xC85: // SetDisplayContentType
				break;
			case 0xC86: // GetDisplayRgbRange
				break;
			case 0xC87: // SetDisplayRgbRange
				break;
			case 0xC88: // GetDisplayCmuMode
				break;
			case 0xC89: // SetDisplayCmuMode
				break;
			case 0xC8A: // GetDisplayContrastRatio
				break;
			case 0xC8B: // SetDisplayContrastRatio
				break;
			case 0xC8E: // GetDisplayGamma
				break;
			case 0xC8F: // SetDisplayGamma
				break;
			case 0xC90: // GetDisplayCmuLuma
				break;
			case 0xC91: // SetDisplayCmuLuma
				break;
			case 0x2021: // GetSharedBufferMemoryHandleId
				break;
			case 0x203A: // OpenSharedLayer
				break;
			case 0x203B: // CloseSharedLayer
				break;
			case 0x203C: // ConnectSharedLayer
				break;
			case 0x203D: // DisconnectSharedLayer
				break;
			case 0x203E: // AcquireSharedFrameBuffer
				break;
			case 0x203F: // PresentSharedFrameBuffer
				break;
			case 0x2040: // GetSharedFrameBufferAcquirableEvent
				break;
			case 0x2041: // FillSharedFrameBufferColor
				break;
			case 0x2042: // CancelSharedFrameBuffer
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.ISystemDisplayService");
		}
	}
}

public partial class ISystemRootService : _ISystemRootService_Base;
public abstract class _ISystemRootService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // GetDisplayService
				break;
			case 0x3: // GetDisplayServiceWithProxyNameExchange
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.ISystemRootService");
		}
	}
}

