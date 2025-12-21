using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Xcd.Detail;
public partial class ISystemServer : _ISystemServer_Base;
public abstract class _ISystemServer_Base : IpcInterface {
	protected virtual void GetDataFormat(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetDataFormat not implemented");
	protected virtual void SetDataFormat(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.SetDataFormat");
	protected virtual void GetMcuState(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetMcuState not implemented");
	protected virtual void SetMcuState(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.SetMcuState");
	protected virtual void GetMcuVersionForNfc(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetMcuVersionForNfc not implemented");
	protected virtual void CheckNfcDevicePower(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.CheckNfcDevicePower");
	protected virtual void SetNfcEvent(byte[] _0, out KObject _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.SetNfcEvent not implemented");
	protected virtual void GetNfcInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetNfcInfo not implemented");
	protected virtual void StartNfcDiscovery(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartNfcDiscovery");
	protected virtual void StopNfcDiscovery(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StopNfcDiscovery");
	protected virtual void StartNtagRead(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartNtagRead");
	protected virtual void StartNtagWrite(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartNtagWrite");
	protected virtual void SendNfcRawData(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.SendNfcRawData");
	protected virtual void RegisterMifareKey(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.RegisterMifareKey");
	protected virtual void ClearMifareKey(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.ClearMifareKey");
	protected virtual void StartMifareRead(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartMifareRead");
	protected virtual void StartMifareWrite(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Xcd.Detail.ISystemServer.StartMifareWrite");
	protected virtual void GetAwakeTriggerReasonForLeftRail(out byte[] _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetAwakeTriggerReasonForLeftRail not implemented");
	protected virtual void GetAwakeTriggerReasonForRightRail(out byte[] _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetAwakeTriggerReasonForRightRail not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDataFormat
				om.Initialize(0, 0, 1);
				GetDataFormat(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // SetDataFormat
				om.Initialize(0, 0, 0);
				SetDataFormat(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2: { // GetMcuState
				om.Initialize(0, 0, 1);
				GetMcuState(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // SetMcuState
				om.Initialize(0, 0, 0);
				SetMcuState(im.GetBytes(8, 0x10));
				break;
			}
			case 0x4: { // GetMcuVersionForNfc
				om.Initialize(0, 0, 32);
				GetMcuVersionForNfc(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // CheckNfcDevicePower
				om.Initialize(0, 0, 0);
				CheckNfcDevicePower(im.GetBytes(8, 0x8));
				break;
			}
			case 0xA: { // SetNfcEvent
				om.Initialize(0, 2, 0);
				SetNfcEvent(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Copy(1, CreateHandle(_1, copy: true));
				break;
			}
			case 0xB: { // GetNfcInfo
				om.Initialize(0, 0, 0);
				GetNfcInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0xC: { // StartNfcDiscovery
				om.Initialize(0, 0, 0);
				StartNfcDiscovery(im.GetBytes(8, 0x10));
				break;
			}
			case 0xD: { // StopNfcDiscovery
				om.Initialize(0, 0, 0);
				StopNfcDiscovery(im.GetBytes(8, 0x8));
				break;
			}
			case 0xE: { // StartNtagRead
				om.Initialize(0, 0, 0);
				StartNtagRead(im.GetBytes(8, 0x30));
				break;
			}
			case 0xF: { // StartNtagWrite
				om.Initialize(0, 0, 0);
				StartNtagWrite(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x10: { // SendNfcRawData
				om.Initialize(0, 0, 0);
				SendNfcRawData(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x11: { // RegisterMifareKey
				om.Initialize(0, 0, 0);
				RegisterMifareKey(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x12: { // ClearMifareKey
				om.Initialize(0, 0, 0);
				ClearMifareKey(im.GetBytes(8, 0x10));
				break;
			}
			case 0x13: { // StartMifareRead
				om.Initialize(0, 0, 0);
				StartMifareRead(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x14: { // StartMifareWrite
				om.Initialize(0, 0, 0);
				StartMifareWrite(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x65: { // GetAwakeTriggerReasonForLeftRail
				om.Initialize(0, 0, 8);
				GetAwakeTriggerReasonForLeftRail(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x66: { // GetAwakeTriggerReasonForRightRail
				om.Initialize(0, 0, 8);
				GetAwakeTriggerReasonForRightRail(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Xcd.Detail.ISystemServer");
		}
	}
}

