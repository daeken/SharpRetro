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
				var _return = GetRelayService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetSystemDisplayService
				var _return = GetSystemDisplayService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x66: { // GetManagerDisplayService
				var _return = GetManagerDisplayService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x67: { // GetIndirectDisplayTransactionService
				var _return = GetIndirectDisplayTransactionService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3E8: { // ListDisplays
				ListDisplays(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x3F2: { // OpenDisplay
				var _return = OpenDisplay(im.GetBytes(8, 0x40));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x3F3: { // OpenDefaultDisplay
				var _return = OpenDefaultDisplay();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x3FC: { // CloseDisplay
				CloseDisplay(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x44D: { // SetDisplayEnabled
				SetDisplayEnabled(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x44E: { // GetDisplayResolution
				GetDisplayResolution(im.GetData<ulong>(8), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7E4: { // OpenLayer
				OpenLayer(im.GetBytes(8, 0x40), im.GetData<ulong>(72), im.GetData<ulong>(80), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x7E5: { // CloseLayer
				CloseLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7EE: { // CreateStrayLayer
				CreateStrayLayer(im.GetData<uint>(8), im.GetData<ulong>(16), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7EF: { // DestroyStrayLayer
				DestroyStrayLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x835: { // SetLayerScalingMode
				SetLayerScalingMode(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x836: { // ConvertScalingMode
				ConvertScalingMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x992: { // GetIndirectLayerImageMap
				GetIndirectLayerImageMap(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x993: { // GetIndirectLayerImageCropMap
				GetIndirectLayerImageCropMap(im.GetData<float>(8), im.GetData<float>(12), im.GetData<float>(16), im.GetData<float>(20), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.GetData<ulong>(48), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x99C: { // GetIndirectLayerImageRequiredMemoryInfo
				GetIndirectLayerImageRequiredMemoryInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x1452: { // GetDisplayVsyncEvent
				var _return = GetDisplayVsyncEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1453: { // GetDisplayVsyncEventForDebug
				var _return = GetDisplayVsyncEventForDebug(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IApplicationDisplayService");
		}
	}
}

public partial class IApplicationRootService : _IApplicationRootService_Base {
	public readonly string ServiceName;
	public IApplicationRootService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IApplicationRootService_Base : IpcInterface {
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IApplicationRootService.GetDisplayService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDisplayService
				var _return = GetDisplayService(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
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
				var _return = AllocateProcessHeapBlock(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xC9: { // FreeProcessHeapBlock
				FreeProcessHeapBlock(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x44E: { // GetDisplayResolution
				GetDisplayResolution(im.GetData<ulong>(8), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7DA: { // CreateManagedLayer
				var _return = CreateManagedLayer(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x7DB: { // DestroyManagedLayer
				DestroyManagedLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x802: { // CreateIndirectLayer
				var _return = CreateIndirectLayer();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x803: { // DestroyIndirectLayer
				DestroyIndirectLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x804: { // CreateIndirectProducerEndPoint
				var _return = CreateIndirectProducerEndPoint(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x805: { // DestroyIndirectProducerEndPoint
				DestroyIndirectProducerEndPoint(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x806: { // CreateIndirectConsumerEndPoint
				var _return = CreateIndirectConsumerEndPoint(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x807: { // DestroyIndirectConsumerEndPoint
				DestroyIndirectConsumerEndPoint(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8FC: { // AcquireLayerTexturePresentingEvent
				var _return = AcquireLayerTexturePresentingEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x8FD: { // ReleaseLayerTexturePresentingEvent
				ReleaseLayerTexturePresentingEvent(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8FE: { // GetDisplayHotplugEvent
				var _return = GetDisplayHotplugEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x962: { // GetDisplayHotplugState
				var _return = GetDisplayHotplugState(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x9C5: { // GetCompositorErrorInfo
				GetCompositorErrorInfo(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xA29: { // GetDisplayErrorEvent
				var _return = GetDisplayErrorEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1069: { // SetDisplayAlpha
				SetDisplayAlpha(im.GetData<float>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x106B: { // SetDisplayLayerStack
				SetDisplayLayerStack(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x106D: { // SetDisplayPowerState
				SetDisplayPowerState(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x106E: { // SetDefaultDisplay
				SetDefaultDisplay(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1770: { // AddToLayerStack
				AddToLayerStack(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1771: { // RemoveFromLayerStack
				RemoveFromLayerStack(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1772: { // SetLayerVisibility
				SetLayerVisibility(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1773: { // SetLayerConfig
				SetLayerConfig();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1774: { // AttachLayerPresentationTracer
				AttachLayerPresentationTracer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1775: { // DetachLayerPresentationTracer
				DetachLayerPresentationTracer();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1776: { // StartLayerPresentationRecording
				StartLayerPresentationRecording();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1777: { // StopLayerPresentationRecording
				StopLayerPresentationRecording();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1778: { // StartLayerPresentationFenceWait
				StartLayerPresentationFenceWait();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1779: { // StopLayerPresentationFenceWait
				StopLayerPresentationFenceWait();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x177A: { // GetLayerPresentationAllFencesExpiredEvent
				GetLayerPresentationAllFencesExpiredEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B58: { // SetContentVisibility
				SetContentVisibility(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F40: { // SetConductorLayer
				SetConductorLayer(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1FA4: { // SetIndirectProducerFlipOffset
				SetIndirectProducerFlipOffset(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2008: { // CreateSharedBufferStaticStorage
				var _return = CreateSharedBufferStaticStorage(im.GetData<ulong>(8), im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2009: { // CreateSharedBufferTransferMemory
				var _return = CreateSharedBufferTransferMemory(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x200A: { // DestroySharedBuffer
				DestroySharedBuffer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x200B: { // BindSharedLowLevelLayerToManagedLayer
				BindSharedLowLevelLayerToManagedLayer(im.GetBytes(8, 0x40), im.GetData<ulong>(72), im.GetData<ulong>(80), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x200C: { // BindSharedLowLevelLayerToIndirectLayer
				BindSharedLowLevelLayerToIndirectLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x200F: { // UnbindSharedLowLevelLayer
				UnbindSharedLowLevelLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2010: { // ConnectSharedLowLevelLayerToSharedBuffer
				ConnectSharedLowLevelLayerToSharedBuffer(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2011: { // DisconnectSharedLowLevelLayerFromSharedBuffer
				DisconnectSharedLowLevelLayerFromSharedBuffer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2012: { // CreateSharedLayer
				var _return = CreateSharedLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2013: { // DestroySharedLayer
				DestroySharedLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2018: { // AttachSharedLayerToLowLevelLayer
				AttachSharedLayerToLowLevelLayer(im.GetBytes(8, 0x10), im.GetData<ulong>(24), im.GetData<ulong>(32));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2019: { // ForceDetachSharedLayerFromLowLevelLayer
				ForceDetachSharedLayerFromLowLevelLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x201A: { // StartDetachSharedLayerFromLowLevelLayer
				StartDetachSharedLayerFromLowLevelLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x201B: { // FinishDetachSharedLayerFromLowLevelLayer
				FinishDetachSharedLayerFromLowLevelLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x201C: { // GetSharedLayerDetachReadyEvent
				var _return = GetSharedLayerDetachReadyEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x201D: { // GetSharedLowLevelLayerSynchronizedEvent
				var _return = GetSharedLowLevelLayerSynchronizedEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x201E: { // CheckSharedLowLevelLayerSynchronized
				var _return = CheckSharedLowLevelLayerSynchronized(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x201F: { // RegisterSharedBufferImporterAruid
				RegisterSharedBufferImporterAruid(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2020: { // UnregisterSharedBufferImporterAruid
				UnregisterSharedBufferImporterAruid(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2023: { // CreateSharedBufferProcessHeap
				var _return = CreateSharedBufferProcessHeap(im.GetData<ulong>(8), im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2024: { // GetSharedLayerLayerStacks
				var _return = GetSharedLayerLayerStacks(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2025: { // SetSharedLayerLayerStacks
				SetSharedLayerLayerStacks(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2063: { // PresentDetachedSharedFrameBufferToLowLevelLayer
				PresentDetachedSharedFrameBufferToLowLevelLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2064: { // FillDetachedSharedFrameBufferColor
				FillDetachedSharedFrameBufferColor(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<uint>(24), im.GetData<ulong>(32), im.GetData<ulong>(40));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2065: { // GetDetachedSharedFrameBufferImage
				GetDetachedSharedFrameBufferImage(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x2066: { // SetDetachedSharedFrameBufferImage
				SetDetachedSharedFrameBufferImage(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2067: { // CopyDetachedSharedFrameBufferImage
				CopyDetachedSharedFrameBufferImage(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2068: { // SetDetachedSharedFrameBufferSubImage
				SetDetachedSharedFrameBufferSubImage(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<uint>(24), im.GetData<uint>(28), im.GetData<ulong>(32), im.GetData<ulong>(40), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2069: { // GetSharedFrameBufferContentParameter
				GetSharedFrameBufferContentParameter(im.GetData<ulong>(8), im.GetData<ulong>(16), out var _0, out var _1, out var _2, out var _3, out var _4);
				om.Initialize(0, 0, 32);
				om.SetData(8, _0);
				om.SetBytes(12, _1);
				om.SetData(28, _2);
				om.SetData(32, _3);
				om.SetData(36, _4);
				break;
			}
			case 0x206A: { // ExpandStartupLogoOnSharedFrameBuffer
				ExpandStartupLogoOnSharedFrameBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.IManagerDisplayService");
		}
	}
}

public partial class IManagerRootService : _IManagerRootService_Base {
	public readonly string ServiceName;
	public IManagerRootService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManagerRootService_Base : IpcInterface {
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerRootService.GetDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayServiceWithProxyNameExchange(byte[] _0, uint _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.IManagerRootService.GetDisplayServiceWithProxyNameExchange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2: { // GetDisplayService
				var _return = GetDisplayService(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetDisplayServiceWithProxyNameExchange
				var _return = GetDisplayServiceWithProxyNameExchange(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(1, 0, 0);
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
				var _return = GetZOrderCountMin(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x4B2: { // GetZOrderCountMax
				var _return = GetZOrderCountMax(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x4B3: { // GetDisplayLogicalResolution
				GetDisplayLogicalResolution(im.GetData<ulong>(8), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4B4: { // SetDisplayMagnification
				SetDisplayMagnification(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x899: { // SetLayerPosition
				SetLayerPosition(im.GetData<float>(8), im.GetData<float>(12), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x89B: { // SetLayerSize
				SetLayerSize(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x89C: { // GetLayerZ
				var _return = GetLayerZ(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x89D: { // SetLayerZ
				SetLayerZ(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x89F: { // SetLayerVisibility
				SetLayerVisibility(im.GetData<byte>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8A1: { // SetLayerAlpha
				SetLayerAlpha(im.GetData<float>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x908: { // CreateStrayLayer
				CreateStrayLayer(im.GetData<uint>(8), im.GetData<ulong>(16), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x960: { // OpenIndirectLayer
				OpenIndirectLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x961: { // CloseIndirectLayer
				CloseIndirectLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x962: { // FlipIndirectLayer
				FlipIndirectLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xBB8: { // ListDisplayModes
				ListDisplayModes(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0xBB9: { // ListDisplayRgbRanges
				ListDisplayRgbRanges(im.GetData<ulong>(8), out var _0, im.GetSpan<uint>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0xBBA: { // ListDisplayContentTypes
				ListDisplayContentTypes(im.GetData<ulong>(8), out var _0, im.GetSpan<uint>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0xC80: { // GetDisplayMode
				GetDisplayMode(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC81: { // SetDisplayMode
				SetDisplayMode(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC82: { // GetDisplayUnderscan
				var _return = GetDisplayUnderscan(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xC83: { // SetDisplayUnderscan
				SetDisplayUnderscan(im.GetData<ulong>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC84: { // GetDisplayContentType
				var _return = GetDisplayContentType(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC85: { // SetDisplayContentType
				SetDisplayContentType(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC86: { // GetDisplayRgbRange
				var _return = GetDisplayRgbRange(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC87: { // SetDisplayRgbRange
				SetDisplayRgbRange(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC88: { // GetDisplayCmuMode
				var _return = GetDisplayCmuMode(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC89: { // SetDisplayCmuMode
				SetDisplayCmuMode(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC8A: { // GetDisplayContrastRatio
				var _return = GetDisplayContrastRatio(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC8B: { // SetDisplayContrastRatio
				SetDisplayContrastRatio(im.GetData<float>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC8E: { // GetDisplayGamma
				var _return = GetDisplayGamma(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC8F: { // SetDisplayGamma
				SetDisplayGamma(im.GetData<float>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC90: { // GetDisplayCmuLuma
				var _return = GetDisplayCmuLuma(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC91: { // SetDisplayCmuLuma
				SetDisplayCmuLuma(im.GetData<float>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2021: { // GetSharedBufferMemoryHandleId
				GetSharedBufferMemoryHandleId(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x203A: { // OpenSharedLayer
				OpenSharedLayer(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x203B: { // CloseSharedLayer
				CloseSharedLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x203C: { // ConnectSharedLayer
				ConnectSharedLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x203D: { // DisconnectSharedLayer
				DisconnectSharedLayer(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x203E: { // AcquireSharedFrameBuffer
				AcquireSharedFrameBuffer(im.GetData<ulong>(8), out var _0, out var _1, out var _2);
				om.Initialize(0, 0, 64);
				om.SetBytes(8, _0);
				om.SetBytes(44, _1);
				om.SetData(64, _2);
				break;
			}
			case 0x203F: { // PresentSharedFrameBuffer
				PresentSharedFrameBuffer(im.GetBytes(8, 0x24), im.GetBytes(44, 0x10), im.GetData<uint>(60), im.GetData<uint>(64), im.GetData<ulong>(72), im.GetData<ulong>(80));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2040: { // GetSharedFrameBufferAcquirableEvent
				var _return = GetSharedFrameBufferAcquirableEvent(im.GetData<ulong>(8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2041: { // FillSharedFrameBufferColor
				FillSharedFrameBufferColor(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20), im.GetData<ulong>(24), im.GetData<ulong>(32));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2042: { // CancelSharedFrameBuffer
				CancelSharedFrameBuffer();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.ISystemDisplayService");
		}
	}
}

public partial class ISystemRootService : _ISystemRootService_Base {
	public readonly string ServiceName;
	public ISystemRootService(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemRootService_Base : IpcInterface {
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayService(uint _0) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemRootService.GetDisplayService not implemented");
	protected virtual Nn.Visrv.Sf.IApplicationDisplayService GetDisplayServiceWithProxyNameExchange(byte[] _0, uint _1) =>
		throw new NotImplementedException("Nn.Visrv.Sf.ISystemRootService.GetDisplayServiceWithProxyNameExchange not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // GetDisplayService
				var _return = GetDisplayService(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetDisplayServiceWithProxyNameExchange
				var _return = GetDisplayServiceWithProxyNameExchange(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Visrv.Sf.ISystemRootService");
		}
	}
}

