using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfp.Detail;
public partial class IDebug : _IDebug_Base;
public abstract class _IDebug_Base : IpcInterface {
	protected virtual void InitializeDebug(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.InitializeDebug".Log();
	protected virtual void FinalizeDebug() =>
		"Stub hit for Nn.Nfp.Detail.IDebug.FinalizeDebug".Log();
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.ListDevices not implemented");
	protected virtual void StartDetection(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.StartDetection".Log();
	protected virtual void StopDetection(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.StopDetection".Log();
	protected virtual void Mount(byte[] _0, uint _1, uint _2) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Mount".Log();
	protected virtual void Unmount(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unmount".Log();
	protected virtual void OpenApplicationArea(byte[] _0, uint _1) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.OpenApplicationArea".Log();
	protected virtual void GetApplicationArea(byte[] _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetApplicationArea not implemented");
	protected virtual void SetApplicationArea(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.SetApplicationArea".Log();
	protected virtual void Flush(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Flush".Log();
	protected virtual void Restore(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Restore".Log();
	protected virtual void CreateApplicationArea(byte[] _0, uint _1, Span<byte> _2) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.CreateApplicationArea".Log();
	protected virtual void GetTagInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetTagInfo not implemented");
	protected virtual void GetRegisterInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetRegisterInfo not implemented");
	protected virtual void GetCommonInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetCommonInfo not implemented");
	protected virtual void GetModelInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetModelInfo not implemented");
	protected virtual KObject AttachActivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.AttachDeactivateEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetState not implemented");
	protected virtual uint GetDeviceState(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetDeviceState not implemented");
	protected virtual uint GetNpadId(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetNpadId not implemented");
	protected virtual uint GetApplicationArea2(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetApplicationArea2 not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.AttachAvailabilityChangeEvent not implemented");
	protected virtual void RecreateApplicationArea(byte[] _0, uint _1, Span<byte> _2) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.RecreateApplicationArea".Log();
	protected virtual void Format(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Format".Log();
	protected virtual void GetAdminInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetAdminInfo not implemented");
	protected virtual void GetRegisterInfo2(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetRegisterInfo2 not implemented");
	protected virtual void SetRegisterInfo(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.SetRegisterInfo".Log();
	protected virtual void DeleteRegisterInfo(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.DeleteRegisterInfo".Log();
	protected virtual void DeleteApplicationArea(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.DeleteApplicationArea".Log();
	protected virtual byte ExistsApplicationArea(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.ExistsApplicationArea not implemented");
	protected virtual void GetAll(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.GetAll not implemented");
	protected virtual void SetAll(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.SetAll".Log();
	protected virtual void FlushDebug(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.FlushDebug".Log();
	protected virtual void BreakTag(byte[] _0, uint _1) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.BreakTag".Log();
	protected virtual void ReadBackupData(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.ReadBackupData not implemented");
	protected virtual void WriteBackupData(Span<byte> _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.WriteBackupData".Log();
	protected virtual void WriteNtf(byte[] _0, uint _1, Span<byte> _2) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.WriteNtf".Log();
	protected virtual void Unknown300(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unknown300".Log();
	protected virtual void Unknown301() =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unknown301".Log();
	protected virtual void Unknown302(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown302 not implemented");
	protected virtual void Unknown303(byte[] _0, uint _1) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unknown303".Log();
	protected virtual void Unknown304(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unknown304".Log();
	protected virtual void Unknown305(byte[] _0, ulong _1, Span<byte> _2, out uint _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown305 not implemented");
	protected virtual void Unknown306(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown306 not implemented");
	protected virtual KObject Unknown307(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown307 not implemented");
	protected virtual KObject Unknown308(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown308 not implemented");
	protected virtual uint Unknown309() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown309 not implemented");
	protected virtual uint Unknown310(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown310 not implemented");
	protected virtual uint Unknown311(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown311 not implemented");
	protected virtual void Unknown312(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unknown312".Log();
	protected virtual void Unknown313(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IDebug.Unknown313".Log();
	protected virtual KObject Unknown314() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebug.Unknown314 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeDebug
				InitializeDebug(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // FinalizeDebug
				FinalizeDebug();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ListDevices
				ListDevices(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // StartDetection
				StartDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // StopDetection
				StopDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Mount
				Mount(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetData<uint>(20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unmount
				Unmount(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // OpenApplicationArea
				OpenApplicationArea(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetApplicationArea
				GetApplicationArea(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x9: { // SetApplicationArea
				SetApplicationArea(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Flush
				Flush(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Restore
				Restore(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // CreateApplicationArea
				CreateApplicationArea(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // GetTagInfo
				GetTagInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // GetRegisterInfo
				GetRegisterInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetCommonInfo
				GetCommonInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetModelInfo
				GetModelInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // AttachActivateEvent
				var _return = AttachActivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12: { // AttachDeactivateEvent
				var _return = AttachDeactivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x13: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x14: { // GetDeviceState
				var _return = GetDeviceState(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetNpadId
				var _return = GetNpadId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // GetApplicationArea2
				var _return = GetApplicationArea2(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // AttachAvailabilityChangeEvent
				var _return = AttachAvailabilityChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // RecreateApplicationArea
				RecreateApplicationArea(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // Format
				Format(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // GetAdminInfo
				GetAdminInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // GetRegisterInfo2
				GetRegisterInfo2(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // SetRegisterInfo
				SetRegisterInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x68: { // DeleteRegisterInfo
				DeleteRegisterInfo(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x69: { // DeleteApplicationArea
				DeleteApplicationArea(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6A: { // ExistsApplicationArea
				var _return = ExistsApplicationArea(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xC8: { // GetAll
				GetAll(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC9: { // SetAll
				SetAll(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // FlushDebug
				FlushDebug(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCB: { // BreakTag
				BreakTag(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCC: { // ReadBackupData
				ReadBackupData(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xCD: { // WriteBackupData
				WriteBackupData(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCE: { // WriteNtf
				WriteNtf(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12C: { // Unknown300
				Unknown300(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12D: { // Unknown301
				Unknown301();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12E: { // Unknown302
				Unknown302(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x12F: { // Unknown303
				Unknown303(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x130: { // Unknown304
				Unknown304(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x131: { // Unknown305
				Unknown305(im.GetBytes(8, 0x8), im.GetData<ulong>(16), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x132: { // Unknown306
				Unknown306(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x133: { // Unknown307
				var _return = Unknown307(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x134: { // Unknown308
				var _return = Unknown308(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x135: { // Unknown309
				var _return = Unknown309();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x136: { // Unknown310
				var _return = Unknown310(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x137: { // Unknown311
				var _return = Unknown311(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x138: { // Unknown312
				Unknown312(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x139: { // Unknown313
				Unknown313(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13A: { // Unknown314
				var _return = Unknown314();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IDebug");
		}
	}
}

public partial class IDebugManager : _IDebugManager_Base {
	public readonly string ServiceName;
	public IDebugManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IDebugManager_Base : IpcInterface {
	protected virtual Nn.Nfp.Detail.IDebug CreateDebugInterface() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IDebugManager.CreateDebugInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateDebugInterface
				var _return = CreateDebugInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IDebugManager");
		}
	}
}

public partial class ISystem : _ISystem_Base;
public abstract class _ISystem_Base : IpcInterface {
	protected virtual void InitializeSystem(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.InitializeSystem".Log();
	protected virtual void FinalizeSystem() =>
		"Stub hit for Nn.Nfp.Detail.ISystem.FinalizeSystem".Log();
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.ListDevices not implemented");
	protected virtual void StartDetection(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.StartDetection".Log();
	protected virtual void StopDetection(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.StopDetection".Log();
	protected virtual void Mount(byte[] _0, uint _1, uint _2) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.Mount".Log();
	protected virtual void Unmount(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.Unmount".Log();
	protected virtual void Flush(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.Flush".Log();
	protected virtual void Restore(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.Restore".Log();
	protected virtual void GetTagInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetTagInfo not implemented");
	protected virtual void GetRegisterInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetRegisterInfo not implemented");
	protected virtual void GetCommonInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetCommonInfo not implemented");
	protected virtual void GetModelInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetModelInfo not implemented");
	protected virtual KObject AttachActivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.AttachDeactivateEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetState not implemented");
	protected virtual uint GetDeviceState(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetDeviceState not implemented");
	protected virtual uint GetNpadId(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetNpadId not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.AttachAvailabilityChangeEvent not implemented");
	protected virtual void Format(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.Format".Log();
	protected virtual void GetAdminInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetAdminInfo not implemented");
	protected virtual void GetRegisterInfo2(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.GetRegisterInfo2 not implemented");
	protected virtual void SetRegisterInfo(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.SetRegisterInfo".Log();
	protected virtual void DeleteRegisterInfo(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.DeleteRegisterInfo".Log();
	protected virtual void DeleteApplicationArea(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.ISystem.DeleteApplicationArea".Log();
	protected virtual byte ExistsApplicationArea(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystem.ExistsApplicationArea not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // InitializeSystem
				InitializeSystem(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // FinalizeSystem
				FinalizeSystem();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ListDevices
				ListDevices(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // StartDetection
				StartDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // StopDetection
				StopDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Mount
				Mount(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetData<uint>(20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unmount
				Unmount(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Flush
				Flush(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Restore
				Restore(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // GetTagInfo
				GetTagInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // GetRegisterInfo
				GetRegisterInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetCommonInfo
				GetCommonInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetModelInfo
				GetModelInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // AttachActivateEvent
				var _return = AttachActivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12: { // AttachDeactivateEvent
				var _return = AttachDeactivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x13: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x14: { // GetDeviceState
				var _return = GetDeviceState(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetNpadId
				var _return = GetNpadId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // AttachAvailabilityChangeEvent
				var _return = AttachAvailabilityChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x64: { // Format
				Format(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // GetAdminInfo
				GetAdminInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // GetRegisterInfo2
				GetRegisterInfo2(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // SetRegisterInfo
				SetRegisterInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x68: { // DeleteRegisterInfo
				DeleteRegisterInfo(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x69: { // DeleteApplicationArea
				DeleteApplicationArea(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6A: { // ExistsApplicationArea
				var _return = ExistsApplicationArea(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.ISystem");
		}
	}
}

public partial class ISystemManager : _ISystemManager_Base {
	public readonly string ServiceName;
	public ISystemManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemManager_Base : IpcInterface {
	protected virtual Nn.Nfp.Detail.ISystem CreateSystemInterface() =>
		throw new NotImplementedException("Nn.Nfp.Detail.ISystemManager.CreateSystemInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateSystemInterface
				var _return = CreateSystemInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.ISystemManager");
		}
	}
}

public partial class IUser : _IUser_Base;
public abstract class _IUser_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong _1, ulong _2, Span<byte> _3) =>
		"Stub hit for Nn.Nfp.Detail.IUser.Initialize".Log();
	protected virtual void _Finalize() =>
		"Stub hit for Nn.Nfp.Detail.IUser._Finalize".Log();
	protected virtual void ListDevices(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.ListDevices not implemented");
	protected virtual void StartDetection(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IUser.StartDetection".Log();
	protected virtual void StopDetection(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IUser.StopDetection".Log();
	protected virtual void Mount(byte[] _0, uint _1, uint _2) =>
		"Stub hit for Nn.Nfp.Detail.IUser.Mount".Log();
	protected virtual void Unmount(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IUser.Unmount".Log();
	protected virtual void OpenApplicationArea(byte[] _0, uint _1) =>
		"Stub hit for Nn.Nfp.Detail.IUser.OpenApplicationArea".Log();
	protected virtual void GetApplicationArea(byte[] _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetApplicationArea not implemented");
	protected virtual void SetApplicationArea(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Nfp.Detail.IUser.SetApplicationArea".Log();
	protected virtual void Flush(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IUser.Flush".Log();
	protected virtual void Restore(byte[] _0) =>
		"Stub hit for Nn.Nfp.Detail.IUser.Restore".Log();
	protected virtual void CreateApplicationArea(byte[] _0, uint _1, Span<byte> _2) =>
		"Stub hit for Nn.Nfp.Detail.IUser.CreateApplicationArea".Log();
	protected virtual void GetTagInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetTagInfo not implemented");
	protected virtual void GetRegisterInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetRegisterInfo not implemented");
	protected virtual void GetCommonInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetCommonInfo not implemented");
	protected virtual void GetModelInfo(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetModelInfo not implemented");
	protected virtual KObject AttachActivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.AttachActivateEvent not implemented");
	protected virtual KObject AttachDeactivateEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.AttachDeactivateEvent not implemented");
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetState not implemented");
	protected virtual uint GetDeviceState(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetDeviceState not implemented");
	protected virtual uint GetNpadId(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetNpadId not implemented");
	protected virtual uint GetApplicationArea2(byte[] _0) =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.GetApplicationArea2 not implemented");
	protected virtual KObject AttachAvailabilityChangeEvent() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUser.AttachAvailabilityChangeEvent not implemented");
	protected virtual void RecreateApplicationArea(byte[] _0, uint _1, Span<byte> _2) =>
		"Stub hit for Nn.Nfp.Detail.IUser.RecreateApplicationArea".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				Initialize(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ListDevices
				ListDevices(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // StartDetection
				StartDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // StopDetection
				StopDetection(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Mount
				Mount(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetData<uint>(20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unmount
				Unmount(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // OpenApplicationArea
				OpenApplicationArea(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetApplicationArea
				GetApplicationArea(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x9: { // SetApplicationArea
				SetApplicationArea(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Flush
				Flush(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Restore
				Restore(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // CreateApplicationArea
				CreateApplicationArea(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // GetTagInfo
				GetTagInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // GetRegisterInfo
				GetRegisterInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetCommonInfo
				GetCommonInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetModelInfo
				GetModelInfo(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // AttachActivateEvent
				var _return = AttachActivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12: { // AttachDeactivateEvent
				var _return = AttachDeactivateEvent(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x13: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x14: { // GetDeviceState
				var _return = GetDeviceState(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetNpadId
				var _return = GetNpadId(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // GetApplicationArea2
				var _return = GetApplicationArea2(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // AttachAvailabilityChangeEvent
				var _return = AttachAvailabilityChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // RecreateApplicationArea
				RecreateApplicationArea(im.GetBytes(8, 0x8), im.GetData<uint>(16), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IUser");
		}
	}
}

public partial class IUserManager : _IUserManager_Base {
	public readonly string ServiceName;
	public IUserManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IUserManager_Base : IpcInterface {
	protected virtual Nn.Nfp.Detail.IUser CreateUserInterface() =>
		throw new NotImplementedException("Nn.Nfp.Detail.IUserManager.CreateUserInterface not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserInterface
				var _return = CreateUserInterface();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nfp.Detail.IUserManager");
		}
	}
}

