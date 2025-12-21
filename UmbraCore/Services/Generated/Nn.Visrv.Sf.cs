using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Visrv.Sf;
public partial class IApplicationDisplayService : _IApplicationDisplayService_Base;
public abstract class _IApplicationDisplayService_Base : IpcInterface {
	protected virtual Nns.Hosbinder.IHOSBinderDriver GetRelayService() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetRelayService not implemented");
	protected virtual Nn.Visrv.Sf.ISystemDisplayService GetSystemDisplayService() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetSystemDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IManagerDisplayService GetManagerDisplayService() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetManagerDisplayService not implemented");
	protected virtual Nns.Hosbinder.IHOSBinderDriver GetIndirectDisplayTransactionService() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectDisplayTransactionService not implemented");
	protected virtual void ListDisplays() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.ListDisplays not implemented");
	protected virtual ulong OpenDisplay(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.OpenDisplay not implemented");
	protected virtual ulong OpenDefaultDisplay() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.OpenDefaultDisplay not implemented");
	protected virtual void CloseDisplay(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.CloseDisplay");
	protected virtual void SetDisplayEnabled(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.SetDisplayEnabled");
	protected virtual void GetDisplayResolution(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetDisplayResolution not implemented");
	protected virtual void OpenLayer(Span<byte> _0, ulong _1, ulong _2, ulong _3) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.OpenLayer not implemented");
	protected virtual void CloseLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.CloseLayer");
	protected virtual void CreateStrayLayer(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.CreateStrayLayer not implemented");
	protected virtual void DestroyStrayLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.DestroyStrayLayer");
	protected virtual void SetLayerScalingMode(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.SetLayerScalingMode");
	protected virtual void ConvertScalingMode() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.ConvertScalingMode");
	protected virtual void GetIndirectLayerImageMap(ulong _0, ulong _1, ulong _2, ulong _3, ulong _4) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectLayerImageMap not implemented");
	protected virtual void GetIndirectLayerImageCropMap(float _0, float _1, float _2, float _3, ulong _4, ulong _5, ulong _6, ulong _7, ulong _8) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectLayerImageCropMap not implemented");
	protected virtual void GetIndirectLayerImageRequiredMemoryInfo(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectLayerImageRequiredMemoryInfo not implemented");
	protected virtual KObject GetDisplayVsyncEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetDisplayVsyncEvent not implemented");
	protected virtual KObject GetDisplayVsyncEventForDebug(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetDisplayVsyncEventForDebug not implemented");
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
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationRootService.GetDisplayService not implemented");
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
	protected virtual ulong AllocateProcessHeapBlock(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.AllocateProcessHeapBlock not implemented");
	protected virtual void FreeProcessHeapBlock(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.FreeProcessHeapBlock");
	protected virtual void GetDisplayResolution(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetDisplayResolution not implemented");
	protected virtual ulong CreateManagedLayer(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateManagedLayer not implemented");
	protected virtual void DestroyManagedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DestroyManagedLayer");
	protected virtual ulong CreateIndirectLayer() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateIndirectLayer not implemented");
	protected virtual void DestroyIndirectLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DestroyIndirectLayer");
	protected virtual ulong CreateIndirectProducerEndPoint(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateIndirectProducerEndPoint not implemented");
	protected virtual void DestroyIndirectProducerEndPoint(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DestroyIndirectProducerEndPoint");
	protected virtual ulong CreateIndirectConsumerEndPoint(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateIndirectConsumerEndPoint not implemented");
	protected virtual void DestroyIndirectConsumerEndPoint(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DestroyIndirectConsumerEndPoint");
	protected virtual KObject AcquireLayerTexturePresentingEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.AcquireLayerTexturePresentingEvent not implemented");
	protected virtual void ReleaseLayerTexturePresentingEvent(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.ReleaseLayerTexturePresentingEvent");
	protected virtual KObject GetDisplayHotplugEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetDisplayHotplugEvent not implemented");
	protected virtual uint GetDisplayHotplugState(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetDisplayHotplugState not implemented");
	protected virtual void GetCompositorErrorInfo(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetCompositorErrorInfo not implemented");
	protected virtual KObject GetDisplayErrorEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetDisplayErrorEvent not implemented");
	protected virtual void SetDisplayAlpha(float _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDisplayAlpha");
	protected virtual void SetDisplayLayerStack(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDisplayLayerStack");
	protected virtual void SetDisplayPowerState(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDisplayPowerState");
	protected virtual void SetDefaultDisplay(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDefaultDisplay");
	protected virtual void AddToLayerStack(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.AddToLayerStack");
	protected virtual void RemoveFromLayerStack(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.RemoveFromLayerStack");
	protected virtual void SetLayerVisibility(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetLayerVisibility");
	protected virtual void SetLayerConfig() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetLayerConfig");
	protected virtual void AttachLayerPresentationTracer() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.AttachLayerPresentationTracer");
	protected virtual void DetachLayerPresentationTracer() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DetachLayerPresentationTracer");
	protected virtual void StartLayerPresentationRecording() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.StartLayerPresentationRecording");
	protected virtual void StopLayerPresentationRecording() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.StopLayerPresentationRecording");
	protected virtual void StartLayerPresentationFenceWait() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.StartLayerPresentationFenceWait");
	protected virtual void StopLayerPresentationFenceWait() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.StopLayerPresentationFenceWait");
	protected virtual void GetLayerPresentationAllFencesExpiredEvent() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.GetLayerPresentationAllFencesExpiredEvent");
	protected virtual void SetContentVisibility(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetContentVisibility");
	protected virtual void SetConductorLayer(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetConductorLayer");
	protected virtual void SetIndirectProducerFlipOffset(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetIndirectProducerFlipOffset");
	protected virtual ulong CreateSharedBufferStaticStorage(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateSharedBufferStaticStorage not implemented");
	protected virtual ulong CreateSharedBufferTransferMemory(ulong _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateSharedBufferTransferMemory not implemented");
	protected virtual void DestroySharedBuffer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DestroySharedBuffer");
	protected virtual void BindSharedLowLevelLayerToManagedLayer(Span<byte> _0, ulong _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.BindSharedLowLevelLayerToManagedLayer");
	protected virtual void BindSharedLowLevelLayerToIndirectLayer(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.BindSharedLowLevelLayerToIndirectLayer");
	protected virtual void UnbindSharedLowLevelLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.UnbindSharedLowLevelLayer");
	protected virtual void ConnectSharedLowLevelLayerToSharedBuffer(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.ConnectSharedLowLevelLayerToSharedBuffer");
	protected virtual void DisconnectSharedLowLevelLayerFromSharedBuffer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DisconnectSharedLowLevelLayerFromSharedBuffer");
	protected virtual ulong CreateSharedLayer(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateSharedLayer not implemented");
	protected virtual void DestroySharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.DestroySharedLayer");
	protected virtual void AttachSharedLayerToLowLevelLayer(Span<byte> _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.AttachSharedLayerToLowLevelLayer");
	protected virtual void ForceDetachSharedLayerFromLowLevelLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.ForceDetachSharedLayerFromLowLevelLayer");
	protected virtual void StartDetachSharedLayerFromLowLevelLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.StartDetachSharedLayerFromLowLevelLayer");
	protected virtual void FinishDetachSharedLayerFromLowLevelLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.FinishDetachSharedLayerFromLowLevelLayer");
	protected virtual KObject GetSharedLayerDetachReadyEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetSharedLayerDetachReadyEvent not implemented");
	protected virtual KObject GetSharedLowLevelLayerSynchronizedEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetSharedLowLevelLayerSynchronizedEvent not implemented");
	protected virtual ulong CheckSharedLowLevelLayerSynchronized(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CheckSharedLowLevelLayerSynchronized not implemented");
	protected virtual void RegisterSharedBufferImporterAruid(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.RegisterSharedBufferImporterAruid");
	protected virtual void UnregisterSharedBufferImporterAruid(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.UnregisterSharedBufferImporterAruid");
	protected virtual ulong CreateSharedBufferProcessHeap(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.CreateSharedBufferProcessHeap not implemented");
	protected virtual uint GetSharedLayerLayerStacks(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetSharedLayerLayerStacks not implemented");
	protected virtual void SetSharedLayerLayerStacks(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetSharedLayerLayerStacks");
	protected virtual void PresentDetachedSharedFrameBufferToLowLevelLayer(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.PresentDetachedSharedFrameBufferToLowLevelLayer");
	protected virtual void FillDetachedSharedFrameBufferColor(uint _0, uint _1, uint _2, uint _3, uint _4, ulong _5, ulong _6) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.FillDetachedSharedFrameBufferColor");
	protected virtual void GetDetachedSharedFrameBufferImage(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetDetachedSharedFrameBufferImage not implemented");
	protected virtual void SetDetachedSharedFrameBufferImage(uint _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDetachedSharedFrameBufferImage");
	protected virtual void CopyDetachedSharedFrameBufferImage(uint _0, uint _1, ulong _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.CopyDetachedSharedFrameBufferImage");
	protected virtual void SetDetachedSharedFrameBufferSubImage(uint _0, uint _1, uint _2, uint _3, uint _4, uint _5, ulong _6, ulong _7, Span<byte> _8) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDetachedSharedFrameBufferSubImage");
	protected virtual void GetSharedFrameBufferContentParameter(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetSharedFrameBufferContentParameter not implemented");
	protected virtual void ExpandStartupLogoOnSharedFrameBuffer() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.ExpandStartupLogoOnSharedFrameBuffer");
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
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerRootService.GetDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayServiceWithProxyNameExchange(Span<byte> _0, uint _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerRootService.GetDisplayServiceWithProxyNameExchange not implemented");
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
	protected virtual ulong GetZOrderCountMin(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetZOrderCountMin not implemented");
	protected virtual ulong GetZOrderCountMax(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetZOrderCountMax not implemented");
	protected virtual void GetDisplayLogicalResolution(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayLogicalResolution not implemented");
	protected virtual void SetDisplayMagnification(uint _0, uint _1, uint _2, uint _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayMagnification");
	protected virtual void SetLayerPosition(float _0, float _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetLayerPosition");
	protected virtual void SetLayerSize(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetLayerSize");
	protected virtual ulong GetLayerZ(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetLayerZ not implemented");
	protected virtual void SetLayerZ(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetLayerZ");
	protected virtual void SetLayerVisibility(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetLayerVisibility");
	protected virtual void SetLayerAlpha(float _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetLayerAlpha");
	protected virtual void CreateStrayLayer(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.CreateStrayLayer not implemented");
	protected virtual void OpenIndirectLayer(ulong _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.OpenIndirectLayer not implemented");
	protected virtual void CloseIndirectLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.CloseIndirectLayer");
	protected virtual void FlipIndirectLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.FlipIndirectLayer");
	protected virtual void ListDisplayModes(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.ListDisplayModes not implemented");
	protected virtual void ListDisplayRgbRanges(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.ListDisplayRgbRanges not implemented");
	protected virtual void ListDisplayContentTypes(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.ListDisplayContentTypes not implemented");
	protected virtual void GetDisplayMode(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.GetDisplayMode");
	protected virtual void SetDisplayMode(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayMode");
	protected virtual ulong GetDisplayUnderscan(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayUnderscan not implemented");
	protected virtual void SetDisplayUnderscan(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayUnderscan");
	protected virtual uint GetDisplayContentType(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayContentType not implemented");
	protected virtual void SetDisplayContentType(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayContentType");
	protected virtual uint GetDisplayRgbRange(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayRgbRange not implemented");
	protected virtual void SetDisplayRgbRange(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayRgbRange");
	protected virtual uint GetDisplayCmuMode(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayCmuMode not implemented");
	protected virtual void SetDisplayCmuMode(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayCmuMode");
	protected virtual float GetDisplayContrastRatio(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayContrastRatio not implemented");
	protected virtual void SetDisplayContrastRatio(float _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayContrastRatio");
	protected virtual float GetDisplayGamma(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayGamma not implemented");
	protected virtual void SetDisplayGamma(float _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayGamma");
	protected virtual float GetDisplayCmuLuma(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetDisplayCmuLuma not implemented");
	protected virtual void SetDisplayCmuLuma(float _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.SetDisplayCmuLuma");
	protected virtual void GetSharedBufferMemoryHandleId(ulong _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetSharedBufferMemoryHandleId not implemented");
	protected virtual void OpenSharedLayer(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.OpenSharedLayer");
	protected virtual void CloseSharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.CloseSharedLayer");
	protected virtual void ConnectSharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.ConnectSharedLayer");
	protected virtual void DisconnectSharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.DisconnectSharedLayer");
	protected virtual void AcquireSharedFrameBuffer(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.AcquireSharedFrameBuffer not implemented");
	protected virtual void PresentSharedFrameBuffer(Span<byte> _0, Span<byte> _1, uint _2, uint _3, ulong _4, ulong _5) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.PresentSharedFrameBuffer");
	protected virtual KObject GetSharedFrameBufferAcquirableEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetSharedFrameBufferAcquirableEvent not implemented");
	protected virtual void FillSharedFrameBufferColor(uint _0, uint _1, uint _2, uint _3, ulong _4, ulong _5) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.FillSharedFrameBufferColor");
	protected virtual void CancelSharedFrameBuffer() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.CancelSharedFrameBuffer");
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
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemRootService.GetDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayServiceWithProxyNameExchange(Span<byte> _0, uint _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemRootService.GetDisplayServiceWithProxyNameExchange not implemented");
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

