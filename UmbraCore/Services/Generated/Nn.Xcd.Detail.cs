using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Xcd.Detail;
public partial class ISystemServer : _ISystemServer_Base {
	public readonly string ServiceName;
	public ISystemServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemServer_Base : IpcInterface {
	protected virtual void GetDataFormat(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetDataFormat not implemented");
	protected virtual void SetDataFormat(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.SetDataFormat".Log();
	protected virtual void GetMcuState(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetMcuState not implemented");
	protected virtual void SetMcuState(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.SetMcuState".Log();
	protected virtual void GetMcuVersionForNfc(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetMcuVersionForNfc not implemented");
	protected virtual void CheckNfcDevicePower(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.CheckNfcDevicePower".Log();
	protected virtual void SetNfcEvent(byte[] _0, out KObject _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.SetNfcEvent not implemented");
	protected virtual void GetNfcInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetNfcInfo not implemented");
	protected virtual void StartNfcDiscovery(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.StartNfcDiscovery".Log();
	protected virtual void StopNfcDiscovery(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.StopNfcDiscovery".Log();
	protected virtual void StartNtagRead(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.StartNtagRead".Log();
	protected virtual void StartNtagWrite(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.StartNtagWrite".Log();
	protected virtual void SendNfcRawData(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.SendNfcRawData".Log();
	protected virtual void RegisterMifareKey(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.RegisterMifareKey".Log();
	protected virtual void ClearMifareKey(byte[] _0) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.ClearMifareKey".Log();
	protected virtual void StartMifareRead(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.StartMifareRead".Log();
	protected virtual void StartMifareWrite(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Xcd.Detail.ISystemServer.StartMifareWrite".Log();
	protected virtual void GetAwakeTriggerReasonForLeftRail(out byte[] _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetAwakeTriggerReasonForLeftRail not implemented");
	protected virtual void GetAwakeTriggerReasonForRightRail(out byte[] _0) =>
		throw new NotImplementedException("Nn.Xcd.Detail.ISystemServer.GetAwakeTriggerReasonForRightRail not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDataFormat
				GetDataFormat(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // SetDataFormat
				SetDataFormat(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetMcuState
				GetMcuState(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // SetMcuState
				SetMcuState(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetMcuVersionForNfc
				GetMcuVersionForNfc(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // CheckNfcDevicePower
				CheckNfcDevicePower(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // SetNfcEvent
				SetNfcEvent(im.GetBytes(8, 0x8), out var _0, out var _1);
				om.Initialize(0, 2, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Copy(1, CreateHandle(_1, copy: true));
				break;
			}
			case 0xB: { // GetNfcInfo
				GetNfcInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // StartNfcDiscovery
				StartNfcDiscovery(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // StopNfcDiscovery
				StopNfcDiscovery(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // StartNtagRead
				StartNtagRead(im.GetBytes(8, 0x30));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // StartNtagWrite
				StartNtagWrite(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // SendNfcRawData
				SendNfcRawData(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // RegisterMifareKey
				RegisterMifareKey(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // ClearMifareKey
				ClearMifareKey(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // StartMifareRead
				StartMifareRead(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // StartMifareWrite
				StartMifareWrite(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // GetAwakeTriggerReasonForLeftRail
				GetAwakeTriggerReasonForLeftRail(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x66: { // GetAwakeTriggerReasonForRightRail
				GetAwakeTriggerReasonForRightRail(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Xcd.Detail.ISystemServer");
		}
	}
}

