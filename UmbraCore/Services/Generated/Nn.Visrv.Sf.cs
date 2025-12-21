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
	protected virtual void ListDisplays(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.ListDisplays not implemented");
	protected virtual ulong OpenDisplay(byte[] _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.OpenDisplay not implemented");
	protected virtual ulong OpenDefaultDisplay() =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.OpenDefaultDisplay not implemented");
	protected virtual void CloseDisplay(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.CloseDisplay");
	protected virtual void SetDisplayEnabled(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.SetDisplayEnabled");
	protected virtual void GetDisplayResolution(ulong _0, out ulong _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetDisplayResolution not implemented");
	protected virtual void OpenLayer(byte[] _0, ulong _1, ulong _2, ulong _3, out ulong _4, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.OpenLayer not implemented");
	protected virtual void CloseLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.CloseLayer");
	protected virtual void CreateStrayLayer(uint _0, ulong _1, out ulong _2, out ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.CreateStrayLayer not implemented");
	protected virtual void DestroyStrayLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.DestroyStrayLayer");
	protected virtual void SetLayerScalingMode(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.SetLayerScalingMode");
	protected virtual void ConvertScalingMode() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IApplicationDisplayService.ConvertScalingMode");
	protected virtual void GetIndirectLayerImageMap(ulong _0, ulong _1, ulong _2, ulong _3, ulong _4, out ulong _5, out ulong _6, Span<byte> _7) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectLayerImageMap not implemented");
	protected virtual void GetIndirectLayerImageCropMap(float _0, float _1, float _2, float _3, ulong _4, ulong _5, ulong _6, ulong _7, ulong _8, out ulong _9, out ulong _10, Span<byte> _11) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectLayerImageCropMap not implemented");
	protected virtual void GetIndirectLayerImageRequiredMemoryInfo(ulong _0, ulong _1, out ulong _2, out ulong _3) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetIndirectLayerImageRequiredMemoryInfo not implemented");
	protected virtual KObject GetDisplayVsyncEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetDisplayVsyncEvent not implemented");
	protected virtual KObject GetDisplayVsyncEventForDebug(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationDisplayService.GetDisplayVsyncEventForDebug not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x64: { // GetRelayService
				om.Initialize(1, 0, 0);
				var _return = GetRelayService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetSystemDisplayService
				om.Initialize(1, 0, 0);
				var _return = GetSystemDisplayService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // GetManagerDisplayService
				om.Initialize(1, 0, 0);
				var _return = GetManagerDisplayService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // GetIndirectDisplayTransactionService
				om.Initialize(1, 0, 0);
				var _return = GetIndirectDisplayTransactionService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // ListDisplays
				om.Initialize(0, 0, 8);
				ListDisplays(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x3F2: { // OpenDisplay
				om.Initialize(0, 0, 8);
				var _return = OpenDisplay(im.GetBytes(8, 0x40));
				om.SetData(8, _return);
				break;
			}
			case 0x3F3: { // OpenDefaultDisplay
				om.Initialize(0, 0, 8);
				var _return = OpenDefaultDisplay();
				om.SetData(8, _return);
				break;
			}
			case 0x3FC: { // CloseDisplay
				om.Initialize(0, 0, 0);
				CloseDisplay(im.GetData<ulong>(8));
				break;
			}
			case 0x44D: { // SetDisplayEnabled
				om.Initialize(0, 0, 0);
				SetDisplayEnabled(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x44E: { // GetDisplayResolution
				om.Initialize(0, 0, 16);
				GetDisplayResolution(im.GetData<ulong>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7E4: { // OpenLayer
				om.Initialize(0, 0, 8);
				OpenLayer(im.GetBytes(8, 0x40), im.GetData<ulong>(72), im.GetData<ulong>(80), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7E5: { // CloseLayer
				om.Initialize(0, 0, 0);
				CloseLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x7EE: { // CreateStrayLayer
				om.Initialize(0, 0, 16);
				CreateStrayLayer(im.GetData<uint>(8), im.GetData<ulong>(16), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7EF: { // DestroyStrayLayer
				om.Initialize(0, 0, 0);
				DestroyStrayLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x835: { // SetLayerScalingMode
				om.Initialize(0, 0, 0);
				SetLayerScalingMode(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x836: { // ConvertScalingMode
				om.Initialize(0, 0, 0);
				ConvertScalingMode();
				break;
			}
			case 0x992: { // GetIndirectLayerImageMap
				om.Initialize(0, 0, 16);
				GetIndirectLayerImageMap(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x46, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x993: { // GetIndirectLayerImageCropMap
				om.Initialize(0, 0, 16);
				GetIndirectLayerImageCropMap(im.GetData<float>(8), im.GetData<float>(12), im.GetData<float>(16), im.GetData<float>(20), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.GetData<ulong>(48), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x46, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x99C: { // GetIndirectLayerImageRequiredMemoryInfo
				om.Initialize(0, 0, 16);
				GetIndirectLayerImageRequiredMemoryInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x1452: { // GetDisplayVsyncEvent
				om.Initialize(0, 1, 0);
				var _return = GetDisplayVsyncEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1453: { // GetDisplayVsyncEventForDebug
				om.Initialize(0, 1, 0);
				var _return = GetDisplayVsyncEventForDebug(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IApplicationDisplayService");
		}
	}
}

public partial class IApplicationRootService : _IApplicationRootService_Base;
public abstract class _IApplicationRootService_Base : IpcInterface {
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationRootService.GetDisplayService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDisplayService
				om.Initialize(1, 0, 0);
				var _return = GetDisplayService(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
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
	protected virtual void GetDisplayResolution(ulong _0, out ulong _1, out ulong _2) =>
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
	protected virtual void GetCompositorErrorInfo(ulong _0, ulong _1, out uint _2, Span<byte> _3) =>
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
	protected virtual void BindSharedLowLevelLayerToManagedLayer(byte[] _0, ulong _1, ulong _2, ulong _3) =>
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
	protected virtual void AttachSharedLayerToLowLevelLayer(byte[] _0, ulong _1, ulong _2) =>
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
	protected virtual void GetDetachedSharedFrameBufferImage(ulong _0, ulong _1, out ulong _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetDetachedSharedFrameBufferImage not implemented");
	protected virtual void SetDetachedSharedFrameBufferImage(uint _0, ulong _1, ulong _2, Span<byte> _3) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDetachedSharedFrameBufferImage");
	protected virtual void CopyDetachedSharedFrameBufferImage(uint _0, uint _1, ulong _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.CopyDetachedSharedFrameBufferImage");
	protected virtual void SetDetachedSharedFrameBufferSubImage(uint _0, uint _1, uint _2, uint _3, uint _4, uint _5, ulong _6, ulong _7, Span<byte> _8) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.SetDetachedSharedFrameBufferSubImage");
	protected virtual void GetSharedFrameBufferContentParameter(ulong _0, ulong _1, out uint _2, out byte[] _3, out uint _4, out uint _5, out uint _6) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerDisplayService.GetSharedFrameBufferContentParameter not implemented");
	protected virtual void ExpandStartupLogoOnSharedFrameBuffer() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.IManagerDisplayService.ExpandStartupLogoOnSharedFrameBuffer");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xC8: { // AllocateProcessHeapBlock
				om.Initialize(0, 0, 8);
				var _return = AllocateProcessHeapBlock(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC9: { // FreeProcessHeapBlock
				om.Initialize(0, 0, 0);
				FreeProcessHeapBlock(im.GetData<ulong>(8));
				break;
			}
			case 0x44E: { // GetDisplayResolution
				om.Initialize(0, 0, 16);
				GetDisplayResolution(im.GetData<ulong>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7DA: { // CreateManagedLayer
				om.Initialize(0, 0, 8);
				var _return = CreateManagedLayer(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.SetData(8, _return);
				break;
			}
			case 0x7DB: { // DestroyManagedLayer
				om.Initialize(0, 0, 0);
				DestroyManagedLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x802: { // CreateIndirectLayer
				om.Initialize(0, 0, 8);
				var _return = CreateIndirectLayer();
				om.SetData(8, _return);
				break;
			}
			case 0x803: { // DestroyIndirectLayer
				om.Initialize(0, 0, 0);
				DestroyIndirectLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x804: { // CreateIndirectProducerEndPoint
				om.Initialize(0, 0, 8);
				var _return = CreateIndirectProducerEndPoint(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.SetData(8, _return);
				break;
			}
			case 0x805: { // DestroyIndirectProducerEndPoint
				om.Initialize(0, 0, 0);
				DestroyIndirectProducerEndPoint(im.GetData<ulong>(8));
				break;
			}
			case 0x806: { // CreateIndirectConsumerEndPoint
				om.Initialize(0, 0, 8);
				var _return = CreateIndirectConsumerEndPoint(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.SetData(8, _return);
				break;
			}
			case 0x807: { // DestroyIndirectConsumerEndPoint
				om.Initialize(0, 0, 0);
				DestroyIndirectConsumerEndPoint(im.GetData<ulong>(8));
				break;
			}
			case 0x8FC: { // AcquireLayerTexturePresentingEvent
				om.Initialize(0, 1, 0);
				var _return = AcquireLayerTexturePresentingEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x8FD: { // ReleaseLayerTexturePresentingEvent
				om.Initialize(0, 0, 0);
				ReleaseLayerTexturePresentingEvent(im.GetData<ulong>(8));
				break;
			}
			case 0x8FE: { // GetDisplayHotplugEvent
				om.Initialize(0, 1, 0);
				var _return = GetDisplayHotplugEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x962: { // GetDisplayHotplugState
				om.Initialize(0, 0, 4);
				var _return = GetDisplayHotplugState(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x9C5: { // GetCompositorErrorInfo
				om.Initialize(0, 0, 4);
				GetCompositorErrorInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xA29: { // GetDisplayErrorEvent
				om.Initialize(0, 1, 0);
				var _return = GetDisplayErrorEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1069: { // SetDisplayAlpha
				om.Initialize(0, 0, 0);
				SetDisplayAlpha(im.GetData<float>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x106B: { // SetDisplayLayerStack
				om.Initialize(0, 0, 0);
				SetDisplayLayerStack(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x106D: { // SetDisplayPowerState
				om.Initialize(0, 0, 0);
				SetDisplayPowerState(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x106E: { // SetDefaultDisplay
				om.Initialize(0, 0, 0);
				SetDefaultDisplay(im.GetData<ulong>(8));
				break;
			}
			case 0x1770: { // AddToLayerStack
				om.Initialize(0, 0, 0);
				AddToLayerStack(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1771: { // RemoveFromLayerStack
				om.Initialize(0, 0, 0);
				RemoveFromLayerStack(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1772: { // SetLayerVisibility
				om.Initialize(0, 0, 0);
				SetLayerVisibility(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1773: { // SetLayerConfig
				om.Initialize(0, 0, 0);
				SetLayerConfig();
				break;
			}
			case 0x1774: { // AttachLayerPresentationTracer
				om.Initialize(0, 0, 0);
				AttachLayerPresentationTracer();
				break;
			}
			case 0x1775: { // DetachLayerPresentationTracer
				om.Initialize(0, 0, 0);
				DetachLayerPresentationTracer();
				break;
			}
			case 0x1776: { // StartLayerPresentationRecording
				om.Initialize(0, 0, 0);
				StartLayerPresentationRecording();
				break;
			}
			case 0x1777: { // StopLayerPresentationRecording
				om.Initialize(0, 0, 0);
				StopLayerPresentationRecording();
				break;
			}
			case 0x1778: { // StartLayerPresentationFenceWait
				om.Initialize(0, 0, 0);
				StartLayerPresentationFenceWait();
				break;
			}
			case 0x1779: { // StopLayerPresentationFenceWait
				om.Initialize(0, 0, 0);
				StopLayerPresentationFenceWait();
				break;
			}
			case 0x177A: { // GetLayerPresentationAllFencesExpiredEvent
				om.Initialize(0, 0, 0);
				GetLayerPresentationAllFencesExpiredEvent();
				break;
			}
			case 0x1B58: { // SetContentVisibility
				om.Initialize(0, 0, 0);
				SetContentVisibility(im.GetData<byte>(8));
				break;
			}
			case 0x1F40: { // SetConductorLayer
				om.Initialize(0, 0, 0);
				SetConductorLayer(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1FA4: { // SetIndirectProducerFlipOffset
				om.Initialize(0, 0, 0);
				SetIndirectProducerFlipOffset(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
			case 0x2008: { // CreateSharedBufferStaticStorage
				om.Initialize(0, 0, 8);
				var _return = CreateSharedBufferStaticStorage(im.GetData<ulong>(8), im.GetSpan<byte>(0x15, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x2009: { // CreateSharedBufferTransferMemory
				om.Initialize(0, 0, 8);
				var _return = CreateSharedBufferTransferMemory(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x15, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x200A: { // DestroySharedBuffer
				om.Initialize(0, 0, 0);
				DestroySharedBuffer(im.GetData<ulong>(8));
				break;
			}
			case 0x200B: { // BindSharedLowLevelLayerToManagedLayer
				om.Initialize(0, 0, 0);
				BindSharedLowLevelLayerToManagedLayer(im.GetBytes(8, 0x40), im.GetData<ulong>(72), im.GetData<ulong>(80), im.Pid);
				break;
			}
			case 0x200C: { // BindSharedLowLevelLayerToIndirectLayer
				om.Initialize(0, 0, 0);
				BindSharedLowLevelLayerToIndirectLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x200F: { // UnbindSharedLowLevelLayer
				om.Initialize(0, 0, 0);
				UnbindSharedLowLevelLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x2010: { // ConnectSharedLowLevelLayerToSharedBuffer
				om.Initialize(0, 0, 0);
				ConnectSharedLowLevelLayerToSharedBuffer(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2011: { // DisconnectSharedLowLevelLayerFromSharedBuffer
				om.Initialize(0, 0, 0);
				DisconnectSharedLowLevelLayerFromSharedBuffer(im.GetData<ulong>(8));
				break;
			}
			case 0x2012: { // CreateSharedLayer
				om.Initialize(0, 0, 8);
				var _return = CreateSharedLayer(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x2013: { // DestroySharedLayer
				om.Initialize(0, 0, 0);
				DestroySharedLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x2018: { // AttachSharedLayerToLowLevelLayer
				om.Initialize(0, 0, 0);
				AttachSharedLayerToLowLevelLayer(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetData<ulong>(32));
				break;
			}
			case 0x2019: { // ForceDetachSharedLayerFromLowLevelLayer
				om.Initialize(0, 0, 0);
				ForceDetachSharedLayerFromLowLevelLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x201A: { // StartDetachSharedLayerFromLowLevelLayer
				om.Initialize(0, 0, 0);
				StartDetachSharedLayerFromLowLevelLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x201B: { // FinishDetachSharedLayerFromLowLevelLayer
				om.Initialize(0, 0, 0);
				FinishDetachSharedLayerFromLowLevelLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x201C: { // GetSharedLayerDetachReadyEvent
				om.Initialize(0, 1, 0);
				var _return = GetSharedLayerDetachReadyEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x201D: { // GetSharedLowLevelLayerSynchronizedEvent
				om.Initialize(0, 1, 0);
				var _return = GetSharedLowLevelLayerSynchronizedEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x201E: { // CheckSharedLowLevelLayerSynchronized
				om.Initialize(0, 0, 8);
				var _return = CheckSharedLowLevelLayerSynchronized(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x201F: { // RegisterSharedBufferImporterAruid
				om.Initialize(0, 0, 0);
				RegisterSharedBufferImporterAruid(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2020: { // UnregisterSharedBufferImporterAruid
				om.Initialize(0, 0, 0);
				UnregisterSharedBufferImporterAruid(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2023: { // CreateSharedBufferProcessHeap
				om.Initialize(0, 0, 8);
				var _return = CreateSharedBufferProcessHeap(im.GetData<ulong>(8), im.GetSpan<byte>(0x15, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x2024: { // GetSharedLayerLayerStacks
				om.Initialize(0, 0, 4);
				var _return = GetSharedLayerLayerStacks(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x2025: { // SetSharedLayerLayerStacks
				om.Initialize(0, 0, 0);
				SetSharedLayerLayerStacks(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2063: { // PresentDetachedSharedFrameBufferToLowLevelLayer
				om.Initialize(0, 0, 0);
				PresentDetachedSharedFrameBufferToLowLevelLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
			case 0x2064: { // FillDetachedSharedFrameBufferColor
				om.Initialize(0, 0, 0);
				FillDetachedSharedFrameBufferColor(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<uint>(24), im.GetData<ulong>(32), im.GetData<ulong>(40));
				break;
			}
			case 0x2065: { // GetDetachedSharedFrameBufferImage
				om.Initialize(0, 0, 8);
				GetDetachedSharedFrameBufferImage(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2066: { // SetDetachedSharedFrameBufferImage
				om.Initialize(0, 0, 0);
				SetDetachedSharedFrameBufferImage(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x2067: { // CopyDetachedSharedFrameBufferImage
				om.Initialize(0, 0, 0);
				CopyDetachedSharedFrameBufferImage(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32));
				break;
			}
			case 0x2068: { // SetDetachedSharedFrameBufferSubImage
				om.Initialize(0, 0, 0);
				SetDetachedSharedFrameBufferSubImage(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<uint>(24), im.GetData<uint>(28), im.GetData<ulong>(32), im.GetData<ulong>(40), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x2069: { // GetSharedFrameBufferContentParameter
				om.Initialize(0, 0, 32);
				GetSharedFrameBufferContentParameter(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, out var _1, out var _2, out var _3, out var _4);
				om.SetData(8, _0);
				om.SetBytes(12, _1);
				om.SetData(28, _2);
				om.SetData(32, _3);
				om.SetData(36, _4);
				break;
			}
			case 0x206A: { // ExpandStartupLogoOnSharedFrameBuffer
				om.Initialize(0, 0, 0);
				ExpandStartupLogoOnSharedFrameBuffer();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IManagerDisplayService");
		}
	}
}

public partial class IManagerRootService : _IManagerRootService_Base;
public abstract class _IManagerRootService_Base : IpcInterface {
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerRootService.GetDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayServiceWithProxyNameExchange(byte[] _0, uint _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerRootService.GetDisplayServiceWithProxyNameExchange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2: { // GetDisplayService
				om.Initialize(1, 0, 0);
				var _return = GetDisplayService(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetDisplayServiceWithProxyNameExchange
				om.Initialize(1, 0, 0);
				var _return = GetDisplayServiceWithProxyNameExchange(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
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
	protected virtual void GetDisplayLogicalResolution(ulong _0, out uint _1, out uint _2) =>
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
	protected virtual void CreateStrayLayer(uint _0, ulong _1, out ulong _2, out ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.CreateStrayLayer not implemented");
	protected virtual void OpenIndirectLayer(ulong _0, ulong _1, ulong _2, out ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.OpenIndirectLayer not implemented");
	protected virtual void CloseIndirectLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.CloseIndirectLayer");
	protected virtual void FlipIndirectLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.FlipIndirectLayer");
	protected virtual void ListDisplayModes(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.ListDisplayModes not implemented");
	protected virtual void ListDisplayRgbRanges(ulong _0, out ulong _1, Span<uint> _2) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.ListDisplayRgbRanges not implemented");
	protected virtual void ListDisplayContentTypes(ulong _0, out ulong _1, Span<uint> _2) =>
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
	protected virtual void GetSharedBufferMemoryHandleId(ulong _0, ulong _1, ulong _2, out uint _3, out ulong _4, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetSharedBufferMemoryHandleId not implemented");
	protected virtual void OpenSharedLayer(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.OpenSharedLayer");
	protected virtual void CloseSharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.CloseSharedLayer");
	protected virtual void ConnectSharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.ConnectSharedLayer");
	protected virtual void DisconnectSharedLayer(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.DisconnectSharedLayer");
	protected virtual void AcquireSharedFrameBuffer(ulong _0, out byte[] _1, out byte[] _2, out ulong _3) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.AcquireSharedFrameBuffer not implemented");
	protected virtual void PresentSharedFrameBuffer(byte[] _0, byte[] _1, uint _2, uint _3, ulong _4, ulong _5) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.PresentSharedFrameBuffer");
	protected virtual KObject GetSharedFrameBufferAcquirableEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemDisplayService.GetSharedFrameBufferAcquirableEvent not implemented");
	protected virtual void FillSharedFrameBufferColor(uint _0, uint _1, uint _2, uint _3, ulong _4, ulong _5) =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.FillSharedFrameBufferColor");
	protected virtual void CancelSharedFrameBuffer() =>
		Console.WriteLine("Stub hit for Nn.Visrv.Sf.ISystemDisplayService.CancelSharedFrameBuffer");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4B0: { // GetZOrderCountMin
				om.Initialize(0, 0, 8);
				var _return = GetZOrderCountMin(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x4B2: { // GetZOrderCountMax
				om.Initialize(0, 0, 8);
				var _return = GetZOrderCountMax(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x4B3: { // GetDisplayLogicalResolution
				om.Initialize(0, 0, 8);
				GetDisplayLogicalResolution(im.GetData<ulong>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4B4: { // SetDisplayMagnification
				om.Initialize(0, 0, 0);
				SetDisplayMagnification(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<ulong>(24));
				break;
			}
			case 0x899: { // SetLayerPosition
				om.Initialize(0, 0, 0);
				SetLayerPosition(im.GetData<float>(8), im.GetData<float>(12), im.GetData<ulong>(16));
				break;
			}
			case 0x89B: { // SetLayerSize
				om.Initialize(0, 0, 0);
				SetLayerSize(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
			case 0x89C: { // GetLayerZ
				om.Initialize(0, 0, 8);
				var _return = GetLayerZ(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x89D: { // SetLayerZ
				om.Initialize(0, 0, 0);
				SetLayerZ(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x89F: { // SetLayerVisibility
				om.Initialize(0, 0, 0);
				SetLayerVisibility(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x8A1: { // SetLayerAlpha
				om.Initialize(0, 0, 0);
				SetLayerAlpha(im.GetData<float>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x908: { // CreateStrayLayer
				om.Initialize(0, 0, 16);
				CreateStrayLayer(im.GetData<uint>(8), im.GetData<ulong>(16), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x960: { // OpenIndirectLayer
				om.Initialize(0, 0, 8);
				OpenIndirectLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x961: { // CloseIndirectLayer
				om.Initialize(0, 0, 0);
				CloseIndirectLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x962: { // FlipIndirectLayer
				om.Initialize(0, 0, 0);
				FlipIndirectLayer(im.GetData<ulong>(8));
				break;
			}
			case 0xBB8: { // ListDisplayModes
				om.Initialize(0, 0, 8);
				ListDisplayModes(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xBB9: { // ListDisplayRgbRanges
				om.Initialize(0, 0, 8);
				ListDisplayRgbRanges(im.GetData<ulong>(8), out var _0, im.GetSpan<uint>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xBBA: { // ListDisplayContentTypes
				om.Initialize(0, 0, 8);
				ListDisplayContentTypes(im.GetData<ulong>(8), out var _0, im.GetSpan<uint>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xC80: { // GetDisplayMode
				om.Initialize(0, 0, 0);
				GetDisplayMode(im.GetData<ulong>(8));
				break;
			}
			case 0xC81: { // SetDisplayMode
				om.Initialize(0, 0, 0);
				SetDisplayMode(im.GetData<ulong>(8));
				break;
			}
			case 0xC82: { // GetDisplayUnderscan
				om.Initialize(0, 0, 8);
				var _return = GetDisplayUnderscan(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC83: { // SetDisplayUnderscan
				om.Initialize(0, 0, 0);
				SetDisplayUnderscan(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xC84: { // GetDisplayContentType
				om.Initialize(0, 0, 4);
				var _return = GetDisplayContentType(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC85: { // SetDisplayContentType
				om.Initialize(0, 0, 0);
				SetDisplayContentType(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xC86: { // GetDisplayRgbRange
				om.Initialize(0, 0, 4);
				var _return = GetDisplayRgbRange(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC87: { // SetDisplayRgbRange
				om.Initialize(0, 0, 0);
				SetDisplayRgbRange(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xC88: { // GetDisplayCmuMode
				om.Initialize(0, 0, 4);
				var _return = GetDisplayCmuMode(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC89: { // SetDisplayCmuMode
				om.Initialize(0, 0, 0);
				SetDisplayCmuMode(im.GetData<uint>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xC8A: { // GetDisplayContrastRatio
				om.Initialize(0, 0, 4);
				var _return = GetDisplayContrastRatio(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC8B: { // SetDisplayContrastRatio
				om.Initialize(0, 0, 0);
				SetDisplayContrastRatio(im.GetData<float>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xC8E: { // GetDisplayGamma
				om.Initialize(0, 0, 4);
				var _return = GetDisplayGamma(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC8F: { // SetDisplayGamma
				om.Initialize(0, 0, 0);
				SetDisplayGamma(im.GetData<float>(8), im.GetData<ulong>(16));
				break;
			}
			case 0xC90: { // GetDisplayCmuLuma
				om.Initialize(0, 0, 4);
				var _return = GetDisplayCmuLuma(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xC91: { // SetDisplayCmuLuma
				om.Initialize(0, 0, 0);
				SetDisplayCmuLuma(im.GetData<float>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x2021: { // GetSharedBufferMemoryHandleId
				om.Initialize(0, 0, 16);
				GetSharedBufferMemoryHandleId(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x203A: { // OpenSharedLayer
				om.Initialize(0, 0, 0);
				OpenSharedLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x203B: { // CloseSharedLayer
				om.Initialize(0, 0, 0);
				CloseSharedLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x203C: { // ConnectSharedLayer
				om.Initialize(0, 0, 0);
				ConnectSharedLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x203D: { // DisconnectSharedLayer
				om.Initialize(0, 0, 0);
				DisconnectSharedLayer(im.GetData<ulong>(8));
				break;
			}
			case 0x203E: { // AcquireSharedFrameBuffer
				om.Initialize(0, 0, 64);
				AcquireSharedFrameBuffer(im.GetData<ulong>(8), out var _0, out var _1, out var _2);
				om.SetBytes(8, _0);
				om.SetBytes(44, _1);
				om.SetData(64, _2);
				break;
			}
			case 0x203F: { // PresentSharedFrameBuffer
				om.Initialize(0, 0, 0);
				PresentSharedFrameBuffer(im.GetBytes(8, 0x24), im.GetBytes(44, 0x10), im.GetData<uint>(60), im.GetData<uint>(64), im.GetData<ulong>(72), im.GetData<ulong>(80));
				break;
			}
			case 0x2040: { // GetSharedFrameBufferAcquirableEvent
				om.Initialize(0, 1, 0);
				var _return = GetSharedFrameBufferAcquirableEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2041: { // FillSharedFrameBufferColor
				om.Initialize(0, 0, 0);
				FillSharedFrameBufferColor(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<ulong>(24), im.GetData<ulong>(32));
				break;
			}
			case 0x2042: { // CancelSharedFrameBuffer
				om.Initialize(0, 0, 0);
				CancelSharedFrameBuffer();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.ISystemDisplayService");
		}
	}
}

public partial class ISystemRootService : _ISystemRootService_Base;
public abstract class _ISystemRootService_Base : IpcInterface {
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemRootService.GetDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayServiceWithProxyNameExchange(byte[] _0, uint _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemRootService.GetDisplayServiceWithProxyNameExchange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // GetDisplayService
				om.Initialize(1, 0, 0);
				var _return = GetDisplayService(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetDisplayServiceWithProxyNameExchange
				om.Initialize(1, 0, 0);
				var _return = GetDisplayServiceWithProxyNameExchange(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.ISystemRootService");
		}
	}
}

