using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Xcd.Detail;
public partial class ISystemServer : _ISystemServer_Base;
public abstract class _ISystemServer_Base : IpcInterface {
	protected virtual void GetDataFormat(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetDataFormat not implemented");
	protected virtual void SetDataFormat(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.SetDataFormat");
	protected virtual void GetMcuState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetMcuState not implemented");
	protected virtual void SetMcuState(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.SetMcuState");
	protected virtual void GetMcuVersionForNfc(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetMcuVersionForNfc not implemented");
	protected virtual void CheckNfcDevicePower(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.CheckNfcDevicePower");
	protected virtual void SetNfcEvent(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.SetNfcEvent not implemented");
	protected virtual void GetNfcInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetNfcInfo not implemented");
	protected virtual void StartNfcDiscovery(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartNfcDiscovery");
	protected virtual void StopNfcDiscovery(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StopNfcDiscovery");
	protected virtual void StartNtagRead(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartNtagRead");
	protected virtual void StartNtagWrite(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartNtagWrite");
	protected virtual void SendNfcRawData(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.SendNfcRawData");
	protected virtual void RegisterMifareKey(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.RegisterMifareKey");
	protected virtual void ClearMifareKey(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.ClearMifareKey");
	protected virtual void StartMifareRead(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartMifareRead");
	protected virtual void StartMifareWrite(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartMifareWrite");
	protected virtual void GetAwakeTriggerReasonForLeftRail() =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetAwakeTriggerReasonForLeftRail not implemented");
	protected virtual void GetAwakeTriggerReasonForRightRail() =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetAwakeTriggerReasonForRightRail not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDataFormat
				break;
			case 0x1: // SetDataFormat
				break;
			case 0x2: // GetMcuState
				break;
			case 0x3: // SetMcuState
				break;
			case 0x4: // GetMcuVersionForNfc
				break;
			case 0x5: // CheckNfcDevicePower
				break;
			case 0xA: // SetNfcEvent
				break;
			case 0xB: // GetNfcInfo
				break;
			case 0xC: // StartNfcDiscovery
				break;
			case 0xD: // StopNfcDiscovery
				break;
			case 0xE: // StartNtagRead
				break;
			case 0xF: // StartNtagWrite
				break;
			case 0x10: // SendNfcRawData
				break;
			case 0x11: // RegisterMifareKey
				break;
			case 0x12: // ClearMifareKey
				break;
			case 0x13: // StartMifareRead
				break;
			case 0x14: // StartMifareWrite
				break;
			case 0x65: // GetAwakeTriggerReasonForLeftRail
				break;
			case 0x66: // GetAwakeTriggerReasonForRightRail
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Xcd.Detail.ISystemServer");
		}
	}
}

