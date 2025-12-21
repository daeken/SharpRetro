using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nns.Nvdrv;
public partial class INvDrvDebugFSServices : _INvDrvDebugFSServices_Base;
public abstract class _INvDrvDebugFSServices_Base : IpcInterface {
	protected virtual void OpenLog(KObject _0, Span<byte> _1) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.OpenLog not implemented");
	protected virtual void CloseLog(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nns.Nvdrv.INvDrvDebugFSServices.CloseLog");
	protected virtual void ReadLog(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.ReadLog not implemented");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvDebugFSServices.Unknown4 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenLog
				break;
			}
			case 0x1: { // CloseLog
				break;
			}
			case 0x2: { // ReadLog
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
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
	protected virtual uint Close(uint fd) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Close not implemented");
	protected virtual uint Initialize(uint transfer_memory_size, KObject current_process, KObject transfer_memory) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.Initialize not implemented");
	protected virtual void QueryEvent(uint fd, uint event_id, out uint _2, out KObject _3) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.QueryEvent not implemented");
	protected virtual uint MapSharedMem(uint fd, uint nvmap_handle, KObject _2) =>
		throw new NotImplementedException("Nns.Nvdrv.INvDrvServices.MapSharedMem not implemented");
	protected virtual void GetStatus(Span<byte> _0) =>
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
	protected virtual void Unknown13(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nns.Nvdrv.INvDrvServices.Unknown13");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Open
				break;
			}
			case 0x1: { // Ioctl
				break;
			}
			case 0x2: { // Close
				break;
			}
			case 0x3: { // Initialize
				break;
			}
			case 0x4: { // QueryEvent
				break;
			}
			case 0x5: { // MapSharedMem
				break;
			}
			case 0x6: { // GetStatus
				break;
			}
			case 0x7: { // ForceSetClientPID
				break;
			}
			case 0x8: { // SetClientPID
				break;
			}
			case 0x9: { // DumpGraphicsMemoryInfo
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			case 0xB: { // Ioctl2
				break;
			}
			case 0xC: { // Ioctl3
				break;
			}
			case 0xD: { // Unknown13
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Nvdrv.INvDrvServices");
		}
	}
}

