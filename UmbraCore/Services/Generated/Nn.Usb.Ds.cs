using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Ds;
public partial class IDsEndpoint : _IDsEndpoint_Base;
public abstract class _IDsEndpoint_Base : IpcInterface {
	protected virtual uint PostBufferAsync(uint size, ulong buffer) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsEndpoint.PostBufferAsync not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsEndpoint.Cancel");
	protected virtual KObject GetCompletionEvent() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsEndpoint.GetCompletionEvent not implemented");
	protected virtual void GetReportData() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsEndpoint.GetReportData not implemented");
	protected virtual void Stall() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsEndpoint.Stall");
	protected virtual void SetZlt(bool _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsEndpoint.SetZlt");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PostBufferAsync
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // GetCompletionEvent
				break;
			case 0x3: // GetReportData
				break;
			case 0x4: // Stall
				break;
			case 0x5: // SetZlt
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsEndpoint");
		}
	}
}

public partial class IDsInterface : _IDsInterface_Base;
public abstract class _IDsInterface_Base : IpcInterface {
	protected virtual Nn.Usb.Ds.IDsEndpoint RegisterEndpoint(byte address) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.RegisterEndpoint not implemented");
	protected virtual KObject GetSetupEvent() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetSetupEvent not implemented");
	protected virtual void GetSetupPacket() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetSetupPacket not implemented");
	protected virtual void EnableInterface() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsInterface.EnableInterface");
	protected virtual void DisableInterface() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsInterface.DisableInterface");
	protected virtual uint CtrlInPostBufferAsync(uint size, ulong buffer) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.CtrlInPostBufferAsync not implemented");
	protected virtual uint CtrlOutPostBufferAsync(uint size, ulong buffer) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.CtrlOutPostBufferAsync not implemented");
	protected virtual KObject GetCtrlInCompletionEvent() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlInCompletionEvent not implemented");
	protected virtual void GetCtrlInReportData() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlInReportData not implemented");
	protected virtual KObject GetCtrlOutCompletionEvent() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlOutCompletionEvent not implemented");
	protected virtual void GetCtrlOutReportData() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlOutReportData not implemented");
	protected virtual void StallCtrl() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsInterface.StallCtrl");
	protected virtual void AppendConfigurationData(byte interface_number, Nn.Usb.Usb_device_speed speed_mode, Span<byte> descriptor) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsInterface.AppendConfigurationData");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterEndpoint
				break;
			case 0x1: // GetSetupEvent
				break;
			case 0x2: // GetSetupPacket
				break;
			case 0x3: // EnableInterface
				break;
			case 0x4: // DisableInterface
				break;
			case 0x5: // CtrlInPostBufferAsync
				break;
			case 0x6: // CtrlOutPostBufferAsync
				break;
			case 0x7: // GetCtrlInCompletionEvent
				break;
			case 0x8: // GetCtrlInReportData
				break;
			case 0x9: // GetCtrlOutCompletionEvent
				break;
			case 0xA: // GetCtrlOutReportData
				break;
			case 0xB: // StallCtrl
				break;
			case 0xC: // AppendConfigurationData
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsInterface");
		}
	}
}

public partial class IDsService : _IDsService_Base;
public abstract class _IDsService_Base : IpcInterface {
	protected virtual void BindDevice(uint complexId) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.BindDevice");
	protected virtual void BindClientProcess(KObject _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.BindClientProcess");
	protected virtual Nn.Usb.Ds.IDsInterface RegisterInterface(byte address) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsService.RegisterInterface not implemented");
	protected virtual KObject GetStateChangeEvent() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsService.GetStateChangeEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsService.GetState not implemented");
	protected virtual void ClearDeviceData() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.ClearDeviceData");
	protected virtual byte AddUsbStringDescriptor(Span<byte> string_descriptor) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsService.AddUsbStringDescriptor not implemented");
	protected virtual void DeleteUsbStringDescriptor(byte index) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.DeleteUsbStringDescriptor");
	protected virtual void SetUsbDeviceDescriptor(Nn.Usb.Usb_device_speed speed_mode, Span<Nn.Usb.Usb_device_descriptor> descriptor) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.SetUsbDeviceDescriptor");
	protected virtual void SetBinaryObjectStore(Span<Nn.Usb.Usb_bos_descriptor> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.SetBinaryObjectStore");
	protected virtual void Enable() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.Enable");
	protected virtual void Disable() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsService.Disable");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BindDevice
				break;
			case 0x1: // BindClientProcess
				break;
			case 0x2: // RegisterInterface
				break;
			case 0x3: // GetStateChangeEvent
				break;
			case 0x4: // GetState
				break;
			case 0x5: // ClearDeviceData
				break;
			case 0x6: // AddUsbStringDescriptor
				break;
			case 0x7: // DeleteUsbStringDescriptor
				break;
			case 0x8: // SetUsbDeviceDescriptor
				break;
			case 0x9: // SetBinaryObjectStore
				break;
			case 0xA: // Enable
				break;
			case 0xB: // Disable
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsService");
		}
	}
}

