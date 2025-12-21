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
	protected virtual void GetReportData(out Nn.Usb.Usb_report_entry[] entries, out uint report_count) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsEndpoint.GetReportData not implemented");
	protected virtual void Stall() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsEndpoint.Stall");
	protected virtual void SetZlt(bool _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsEndpoint.SetZlt");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // PostBufferAsync
				var _return = PostBufferAsync(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetCompletionEvent
				var _return = GetCompletionEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // GetReportData
				GetReportData(out var _0, out var _2);
				om.Initialize(0, 0, 4);
				var ptr_1 = (Nn.Usb.Usb_report_entry*) om.GetDataPointer(8);
				ptr_1[0] = _0[0];
				ptr_1[1] = _0[1];
				ptr_1[2] = _0[2];
				ptr_1[3] = _0[3];
				ptr_1[4] = _0[4];
				ptr_1[5] = _0[5];
				ptr_1[6] = _0[6];
				ptr_1[7] = _0[7];
				om.SetData(8, _2);
				break;
			}
			case 0x4: { // Stall
				Stall();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // SetZlt
				SetZlt(im.GetData<bool>(8));
				om.Initialize(0, 0, 0);
				break;
			}
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
	protected virtual void GetSetupPacket(Span<byte> _0) =>
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
	protected virtual void GetCtrlInReportData(out Nn.Usb.Usb_report_entry[] entries, out uint report_count) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlInReportData not implemented");
	protected virtual KObject GetCtrlOutCompletionEvent() =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlOutCompletionEvent not implemented");
	protected virtual void GetCtrlOutReportData(out Nn.Usb.Usb_report_entry[] entries, out uint report_count) =>
		throw new NotImplementedException("Nn.Usb.Ds.IDsInterface.GetCtrlOutReportData not implemented");
	protected virtual void StallCtrl() =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsInterface.StallCtrl");
	protected virtual void AppendConfigurationData(byte interface_number, Nn.Usb.Usb_device_speed speed_mode, Span<byte> descriptor) =>
		Console.WriteLine("Stub hit for Nn.Usb.Ds.IDsInterface.AppendConfigurationData");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterEndpoint
				var _return = RegisterEndpoint(im.GetData<byte>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetSetupEvent
				var _return = GetSetupEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // GetSetupPacket
				GetSetupPacket(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // EnableInterface
				EnableInterface();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // DisableInterface
				DisableInterface();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // CtrlInPostBufferAsync
				var _return = CtrlInPostBufferAsync(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // CtrlOutPostBufferAsync
				var _return = CtrlOutPostBufferAsync(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // GetCtrlInCompletionEvent
				var _return = GetCtrlInCompletionEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x8: { // GetCtrlInReportData
				GetCtrlInReportData(out var _0, out var _2);
				om.Initialize(0, 0, 4);
				var ptr_1 = (Nn.Usb.Usb_report_entry*) om.GetDataPointer(8);
				ptr_1[0] = _0[0];
				ptr_1[1] = _0[1];
				ptr_1[2] = _0[2];
				ptr_1[3] = _0[3];
				ptr_1[4] = _0[4];
				ptr_1[5] = _0[5];
				ptr_1[6] = _0[6];
				ptr_1[7] = _0[7];
				om.SetData(8, _2);
				break;
			}
			case 0x9: { // GetCtrlOutCompletionEvent
				var _return = GetCtrlOutCompletionEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xA: { // GetCtrlOutReportData
				GetCtrlOutReportData(out var _0, out var _2);
				om.Initialize(0, 0, 4);
				var ptr_1 = (Nn.Usb.Usb_report_entry*) om.GetDataPointer(8);
				ptr_1[0] = _0[0];
				ptr_1[1] = _0[1];
				ptr_1[2] = _0[2];
				ptr_1[3] = _0[3];
				ptr_1[4] = _0[4];
				ptr_1[5] = _0[5];
				ptr_1[6] = _0[6];
				ptr_1[7] = _0[7];
				om.SetData(8, _2);
				break;
			}
			case 0xB: { // StallCtrl
				StallCtrl();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // AppendConfigurationData
				AppendConfigurationData(im.GetData<byte>(8), im.GetData<Nn.Usb.Usb_device_speed>(12), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsInterface");
		}
	}
}

public partial class IDsService : _IDsService_Base {
	public readonly string ServiceName;
	public IDsService(string serviceName) => ServiceName = serviceName;
}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindDevice
				BindDevice(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // BindClientProcess
				BindClientProcess(Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // RegisterInterface
				var _return = RegisterInterface(im.GetData<byte>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetStateChangeEvent
				var _return = GetStateChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x4: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // ClearDeviceData
				ClearDeviceData();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // AddUsbStringDescriptor
				var _return = AddUsbStringDescriptor(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // DeleteUsbStringDescriptor
				DeleteUsbStringDescriptor(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // SetUsbDeviceDescriptor
				SetUsbDeviceDescriptor(im.GetData<Nn.Usb.Usb_device_speed>(8), im.GetSpan<Nn.Usb.Usb_device_descriptor>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // SetBinaryObjectStore
				SetBinaryObjectStore(im.GetSpan<Nn.Usb.Usb_bos_descriptor>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Enable
				Enable();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Disable
				Disable();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsService");
		}
	}
}

