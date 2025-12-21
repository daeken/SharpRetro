using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nns.Nvdrv;
public partial class INvDrvDebugFSServices : _INvDrvDebugFSServices_Base;
public abstract class _INvDrvDebugFSServices_Base : IpcInterface {
	protected virtual void OpenLog(KObject _0, out byte[] _1) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.OpenLog not implemented");
	protected virtual void CloseLog(byte[] _0) =>
		Console.WriteLine("Stub hit for Nns.Nvdrv.INvDrvDebugFSServices.CloseLog");
	protected virtual void ReadLog(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.ReadLog not implemented");
	protected virtual void Unknown3(byte[] _0, Span<byte> _1, out byte[] _2, Span<byte> _3) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0, Span<byte> _1, Span<byte> _2, out byte[] _3) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.Unknown4 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenLog
				om.Initialize(0, 0, 4);
				OpenLog(Kernel.Get<KObject>(im.GetCopy(0)), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // CloseLog
				om.Initialize(0, 0, 0);
				CloseLog(im.GetBytes(8, 0x4));
				break;
			}
			case 0x2: { // ReadLog
				om.Initialize(0, 0, 4);
				ReadLog(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 4);
				Unknown3(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 4);
				Unknown4(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Nvdrv.INvDrvDebugFSServices");
		}
	}
}

public partial class INvDrvServices : _INvDrvServices_Base;
public abstract class _INvDrvServices_Base : IpcInterface {
	protected virtual void Open(Span<byte> path, out uint fd, out uint error_code) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Open not implemented");
	protected virtual void Ioctl(uint fd, uint rq_id, Span<byte> _2, out uint error_code, Span<byte> _4) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Ioctl not implemented");
	protected virtual uint _Close(uint fd) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices._Close not implemented");
	protected virtual uint Initialize(uint transfer_memory_size, KObject current_process, KObject transfer_memory) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Initialize not implemented");
	protected virtual void QueryEvent(uint fd, uint event_id, out uint _2, out KObject _3) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.QueryEvent not implemented");
	protected virtual uint MapSharedMem(uint fd, uint nvmap_handle, KObject _2) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.MapSharedMem not implemented");
	protected virtual void GetStatus(out byte[] _0) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.GetStatus not implemented");
	protected virtual uint ForceSetClientPID(ulong pid) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.ForceSetClientPID not implemented");
	protected virtual uint SetClientPID(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.SetClientPID not implemented");
	protected virtual void DumpGraphicsMemoryInfo() =>
		Console.WriteLine("Stub hit for Nns.Nvdrv.INvDrvServices.DumpGraphicsMemoryInfo");
	protected virtual uint Unknown10(uint _0, KObject _1) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Unknown10 not implemented");
	protected virtual void Ioctl2(uint _0, uint _1, Span<byte> _2, Span<byte> _3, out uint _4, Span<byte> _5) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Ioctl2 not implemented");
	protected virtual void Ioctl3(uint _0, uint _1, Span<byte> _2, out uint _3, Span<byte> _4, Span<byte> _5) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Ioctl3 not implemented");
	protected virtual void Unknown13(byte[] _0) =>
		Console.WriteLine("Stub hit for Nns.Nvdrv.INvDrvServices.Unknown13");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				om.Initialize(0, 0, 8);
				Open(im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1: { // Ioctl
				om.Initialize(0, 0, 4);
				Ioctl(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x21, 0), out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // _Close
				om.Initialize(0, 0, 4);
				var _return = _Close(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // Initialize
				om.Initialize(0, 0, 4);
				var _return = Initialize(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)), Kernel.Get<KObject>(im.GetCopy(1)));
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // QueryEvent
				om.Initialize(0, 1, 4);
				QueryEvent(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1);
				om.SetData(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x5: { // MapSharedMem
				om.Initialize(0, 0, 4);
				var _return = MapSharedMem(im.GetData<uint>(8), im.GetData<uint>(12), Kernel.Get<KObject>(im.GetCopy(0)));
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // GetStatus
				om.Initialize(0, 0, 36);
				GetStatus(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // ForceSetClientPID
				om.Initialize(0, 0, 4);
				var _return = ForceSetClientPID(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // SetClientPID
				om.Initialize(0, 0, 4);
				var _return = SetClientPID(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // DumpGraphicsMemoryInfo
				om.Initialize(0, 0, 0);
				DumpGraphicsMemoryInfo();
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 4);
				var _return = Unknown10(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // Ioctl2
				om.Initialize(0, 0, 4);
				Ioctl2(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x21, 0), im.GetSpan<byte>(0x21, 1), out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xC: { // Ioctl3
				om.Initialize(0, 0, 4);
				Ioctl3(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x21, 0), out var _0, im.GetSpan<byte>(0x22, 0), im.GetSpan<byte>(0x22, 1));
				om.SetData(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				om.Initialize(0, 0, 0);
				Unknown13(im.GetBytes(8, 0x8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Nvdrv.INvDrvServices");
		}
	}
}

