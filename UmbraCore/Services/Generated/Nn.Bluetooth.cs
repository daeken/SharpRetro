using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bluetooth;
public partial class IBluetoothDriver : _IBluetoothDriver_Base;
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
	protected virtual void GetAdapterProperties() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.GetAdapterProperties not implemented");
	protected virtual void GetAdapterProperty(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.GetAdapterProperty not implemented");
	protected virtual void SetAdapterProperty(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.SetAdapterProperty");
	protected virtual void StartDiscovery() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.StartDiscovery");
	protected virtual void CancelDiscovery() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CancelDiscovery");
	protected virtual void CreateBond(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CreateBond");
	protected virtual void RemoveBond(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.RemoveBond");
	protected virtual void CancelBond(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.CancelBond");
	protected virtual void PinReply(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.PinReply");
	protected virtual void SspReply(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.SspReply");
	protected virtual void Unknown15() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Unknown15 not implemented");
	protected virtual KObject InitInterfaces(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.InitInterfaces not implemented");
	protected virtual void HidHostInterface_Connect(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_Connect");
	protected virtual void HidHostInterface_Disconnect(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_Disconnect");
	protected virtual void HidHostInterface_SendData(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_SendData");
	protected virtual void HidHostInterface_SendData2(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_SendData2");
	protected virtual void HidHostInterface_SetReport(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_SetReport");
	protected virtual void HidHostInterface_GetReport(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetReport");
	protected virtual void HidHostInterface_WakeController(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_WakeController");
	protected virtual void HidHostInterface_AddPairedDevice(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_AddPairedDevice");
	protected virtual void HidHostInterface_GetPairedDevice(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetPairedDevice not implemented");
	protected virtual void HidHostInterface_CleanupAndShutdown() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_CleanupAndShutdown");
	protected virtual void Unknown27() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Unknown27 not implemented");
	protected virtual void ExtInterface_SetTSI(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetTSI");
	protected virtual void ExtInterface_SetBurstMode(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetBurstMode");
	protected virtual void ExtInterface_SetZeroRetran(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetZeroRetran");
	protected virtual void ExtInterface_SetMcMode(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetMcMode");
	protected virtual void ExtInterface_StartLlrMode() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_StartLlrMode");
	protected virtual void ExtInterface_ExitLlrMode() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_ExitLlrMode");
	protected virtual void ExtInterface_SetRadio(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetRadio");
	protected virtual void ExtInterface_SetVisibility(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.ExtInterface_SetVisibility");
	protected virtual void Unknown36(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.Unknown36");
	protected virtual KObject Unknown37() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.Unknown37 not implemented");
	protected virtual void HidHostInterface_GetLatestPlr() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetLatestPlr not implemented");
	protected virtual void ExtInterface_GetPendingConnections() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.ExtInterface_GetPendingConnections not implemented");
	protected virtual void HidHostInterface_GetChannelMap() =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.HidHostInterface_GetChannelMap");
	protected virtual void SetIsBluetoothBoostEnabled() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.SetIsBluetoothBoostEnabled not implemented");
	protected virtual void GetIsBluetoothBoostEnabled(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.GetIsBluetoothBoostEnabled");
	protected virtual void SetIsBluetoothAfhEnabled() =>
		throw new NotImplementedException("Nn.Bluetooth.IBluetoothDriver.SetIsBluetoothAfhEnabled not implemented");
	protected virtual void GetIsBluetoothAfhEnabled(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bluetooth.IBluetoothDriver.GetIsBluetoothAfhEnabled");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Init
				break;
			case 0x2: // Enable
				break;
			case 0x3: // Disable
				break;
			case 0x4: // CleanupAndShutdown
				break;
			case 0x5: // GetAdapterProperties
				break;
			case 0x6: // GetAdapterProperty
				break;
			case 0x7: // SetAdapterProperty
				break;
			case 0x8: // StartDiscovery
				break;
			case 0x9: // CancelDiscovery
				break;
			case 0xA: // CreateBond
				break;
			case 0xB: // RemoveBond
				break;
			case 0xC: // CancelBond
				break;
			case 0xD: // PinReply
				break;
			case 0xE: // SspReply
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // InitInterfaces
				break;
			case 0x11: // HidHostInterface_Connect
				break;
			case 0x12: // HidHostInterface_Disconnect
				break;
			case 0x13: // HidHostInterface_SendData
				break;
			case 0x14: // HidHostInterface_SendData2
				break;
			case 0x15: // HidHostInterface_SetReport
				break;
			case 0x16: // HidHostInterface_GetReport
				break;
			case 0x17: // HidHostInterface_WakeController
				break;
			case 0x18: // HidHostInterface_AddPairedDevice
				break;
			case 0x19: // HidHostInterface_GetPairedDevice
				break;
			case 0x1A: // HidHostInterface_CleanupAndShutdown
				break;
			case 0x1B: // Unknown27
				break;
			case 0x1C: // ExtInterface_SetTSI
				break;
			case 0x1D: // ExtInterface_SetBurstMode
				break;
			case 0x1E: // ExtInterface_SetZeroRetran
				break;
			case 0x1F: // ExtInterface_SetMcMode
				break;
			case 0x20: // ExtInterface_StartLlrMode
				break;
			case 0x21: // ExtInterface_ExitLlrMode
				break;
			case 0x22: // ExtInterface_SetRadio
				break;
			case 0x23: // ExtInterface_SetVisibility
				break;
			case 0x24: // Unknown36
				break;
			case 0x25: // Unknown37
				break;
			case 0x26: // HidHostInterface_GetLatestPlr
				break;
			case 0x27: // ExtInterface_GetPendingConnections
				break;
			case 0x28: // HidHostInterface_GetChannelMap
				break;
			case 0x29: // SetIsBluetoothBoostEnabled
				break;
			case 0x2A: // GetIsBluetoothBoostEnabled
				break;
			case 0x2B: // SetIsBluetoothAfhEnabled
				break;
			case 0x2C: // GetIsBluetoothAfhEnabled
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bluetooth.IBluetoothDriver");
		}
	}
}

