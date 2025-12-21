using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bluetooth;
public partial class IBluetoothDriver : _IBluetoothDriver_Base {
	public readonly string ServiceName;
	public IBluetoothDriver(string serviceName) => ServiceName = serviceName;
}
public abstract class _IBluetoothDriver_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.Unknown0");
	protected virtual KObject Init() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Init not implemented");
	protected virtual void Enable() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.Enable");
	protected virtual void Disable() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.Disable");
	protected virtual void CleanupAndShutdown() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CleanupAndShutdown");
	protected virtual void GetAdapterProperties(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.GetAdapterProperties not implemented");
	protected virtual void GetAdapterProperty(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.GetAdapterProperty not implemented");
	protected virtual void SetAdapterProperty(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.SetAdapterProperty");
	protected virtual void StartDiscovery() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.StartDiscovery");
	protected virtual void CancelDiscovery() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CancelDiscovery");
	protected virtual void CreateBond(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CreateBond");
	protected virtual void RemoveBond(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.RemoveBond");
	protected virtual void CancelBond(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CancelBond");
	protected virtual void PinReply(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.PinReply");
	protected virtual void SspReply(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.SspReply");
	protected virtual void Unknown15(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Unknown15 not implemented");
	protected virtual KObject InitInterfaces(byte[] _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.InitInterfaces not implemented");
	protected virtual void HidHostInterface_Connect(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_Connect");
	protected virtual void HidHostInterface_Disconnect(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_Disconnect");
	protected virtual void HidHostInterface_SendData(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_SendData");
	protected virtual void HidHostInterface_SendData2(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_SendData2");
	protected virtual void HidHostInterface_SetReport(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_SetReport");
	protected virtual void HidHostInterface_GetReport(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetReport");
	protected virtual void HidHostInterface_WakeController(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_WakeController");
	protected virtual void HidHostInterface_AddPairedDevice(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_AddPairedDevice");
	protected virtual void HidHostInterface_GetPairedDevice(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetPairedDevice not implemented");
	protected virtual void HidHostInterface_CleanupAndShutdown() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_CleanupAndShutdown");
	protected virtual void Unknown27(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Unknown27 not implemented");
	protected virtual void ExtInterface_SetTSI(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetTSI");
	protected virtual void ExtInterface_SetBurstMode(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetBurstMode");
	protected virtual void ExtInterface_SetZeroRetran(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetZeroRetran");
	protected virtual void ExtInterface_SetMcMode(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetMcMode");
	protected virtual void ExtInterface_StartLlrMode() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_StartLlrMode");
	protected virtual void ExtInterface_ExitLlrMode() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_ExitLlrMode");
	protected virtual void ExtInterface_SetRadio(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetRadio");
	protected virtual void ExtInterface_SetVisibility(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetVisibility");
	protected virtual void Unknown36(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.Unknown36");
	protected virtual KObject Unknown37() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Unknown37 not implemented");
	protected virtual void HidHostInterface_GetLatestPlr(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetLatestPlr not implemented");
	protected virtual void ExtInterface_GetPendingConnections(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.ExtInterface_GetPendingConnections not implemented");
	protected virtual void HidHostInterface_GetChannelMap() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetChannelMap");
	protected virtual void SetIsBluetoothBoostEnabled(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.SetIsBluetoothBoostEnabled not implemented");
	protected virtual void GetIsBluetoothBoostEnabled(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.GetIsBluetoothBoostEnabled");
	protected virtual void SetIsBluetoothAfhEnabled(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.SetIsBluetoothAfhEnabled not implemented");
	protected virtual void GetIsBluetoothAfhEnabled(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.GetIsBluetoothAfhEnabled");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Init
				var _return = Init();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // Enable
				Enable();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Disable
				Disable();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // CleanupAndShutdown
				CleanupAndShutdown();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetAdapterProperties
				GetAdapterProperties(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetAdapterProperty
				GetAdapterProperty(im.GetBytes(8, 0x4), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // SetAdapterProperty
				SetAdapterProperty(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // StartDiscovery
				StartDiscovery();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // CancelDiscovery
				CancelDiscovery();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // CreateBond
				CreateBond(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // RemoveBond
				RemoveBond(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // CancelBond
				CancelBond(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // PinReply
				PinReply(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // SspReply
				SspReply(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // InitInterfaces
				var _return = InitInterfaces(im.GetBytes(8, 0x2));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x11: { // HidHostInterface_Connect
				HidHostInterface_Connect(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // HidHostInterface_Disconnect
				HidHostInterface_Disconnect(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // HidHostInterface_SendData
				HidHostInterface_SendData(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // HidHostInterface_SendData2
				HidHostInterface_SendData2(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // HidHostInterface_SetReport
				HidHostInterface_SetReport(im.GetBytes(8, 0xC), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // HidHostInterface_GetReport
				HidHostInterface_GetReport(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // HidHostInterface_WakeController
				HidHostInterface_WakeController(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // HidHostInterface_AddPairedDevice
				HidHostInterface_AddPairedDevice(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // HidHostInterface_GetPairedDevice
				HidHostInterface_GetPairedDevice(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // HidHostInterface_CleanupAndShutdown
				HidHostInterface_CleanupAndShutdown();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // Unknown27
				Unknown27(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1C: { // ExtInterface_SetTSI
				ExtInterface_SetTSI(im.GetBytes(8, 0x7));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1D: { // ExtInterface_SetBurstMode
				ExtInterface_SetBurstMode(im.GetBytes(8, 0x7));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // ExtInterface_SetZeroRetran
				ExtInterface_SetZeroRetran(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // ExtInterface_SetMcMode
				ExtInterface_SetMcMode(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x20: { // ExtInterface_StartLlrMode
				ExtInterface_StartLlrMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // ExtInterface_ExitLlrMode
				ExtInterface_ExitLlrMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x22: { // ExtInterface_SetRadio
				ExtInterface_SetRadio(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // ExtInterface_SetVisibility
				ExtInterface_SetVisibility(im.GetBytes(8, 0x2));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x24: { // Unknown36
				Unknown36(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // Unknown37
				var _return = Unknown37();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x26: { // HidHostInterface_GetLatestPlr
				HidHostInterface_GetLatestPlr(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x27: { // ExtInterface_GetPendingConnections
				ExtInterface_GetPendingConnections(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // HidHostInterface_GetChannelMap
				HidHostInterface_GetChannelMap();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x29: { // SetIsBluetoothBoostEnabled
				SetIsBluetoothBoostEnabled(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2A: { // GetIsBluetoothBoostEnabled
				GetIsBluetoothBoostEnabled(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2B: { // SetIsBluetoothAfhEnabled
				SetIsBluetoothAfhEnabled(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C: { // GetIsBluetoothAfhEnabled
				GetIsBluetoothAfhEnabled(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bluetooth.IBluetoothDriver");
		}
	}
}

