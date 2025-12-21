using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcie.Detail;
public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManager_Base : IpcInterface {
	protected virtual void RegisterClassDriver(byte[] _0, KObject _1, out KObject _2, out Nn.Pcie.Detail.ISession _3) =>
		throw new NotImplementedException("Nn.Pcie.Detail.IManager.RegisterClassDriver not implemented");
	protected virtual void QueryFunctionsUnregistered(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.IManager.QueryFunctionsUnregistered not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterClassDriver
				RegisterClassDriver(im.GetBytes(8, 0x18), Kernel.Get<KObject>(im.GetCopy(0)), out var _0, out var _1);
				om.Initialize(1, 1, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x1: { // QueryFunctionsUnregistered
				QueryFunctionsUnregistered(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcie.Detail.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void QueryFunctions(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.QueryFunctions not implemented");
	protected virtual KObject AcquireFunction(byte[] _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.AcquireFunction not implemented");
	protected virtual void ReleaseFunction(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.ReleaseFunction");
	protected virtual void GetFunctionState(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetFunctionState not implemented");
	protected virtual void GetBarProfile(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetBarProfile not implemented");
	protected virtual void ReadConfig(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.ReadConfig not implemented");
	protected virtual void WriteConfig(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.WriteConfig");
	protected virtual void ReadBarRegion(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.ReadBarRegion not implemented");
	protected virtual void WriteBarRegion(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.WriteBarRegion");
	protected virtual void FindCapability(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.FindCapability not implemented");
	protected virtual void FindExtendedCapability(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.FindExtendedCapability not implemented");
	protected virtual void MapDma(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.MapDma not implemented");
	protected virtual void UnmapDma(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.UnmapDma");
	protected virtual void UnmapDmaBusAddress(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.UnmapDmaBusAddress");
	protected virtual void GetDmaBusAddress(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetDmaBusAddress not implemented");
	protected virtual void GetDmaBusAddressRange(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetDmaBusAddressRange not implemented");
	protected virtual void SetDmaEnable(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.SetDmaEnable");
	protected virtual KObject AcquireIrq(byte[] _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.AcquireIrq not implemented");
	protected virtual void ReleaseIrq(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.ReleaseIrq");
	protected virtual void SetIrqEnable(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.SetIrqEnable");
	protected virtual void SetAspmEnable(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.SetAspmEnable");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // QueryFunctions
				QueryFunctions(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // AcquireFunction
				var _return = AcquireFunction(im.GetBytes(8, 0x4));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // ReleaseFunction
				ReleaseFunction(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetFunctionState
				GetFunctionState(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetBarProfile
				GetBarProfile(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // ReadConfig
				ReadConfig(im.GetBytes(8, 0xC), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // WriteConfig
				WriteConfig(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // ReadBarRegion
				ReadBarRegion(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // WriteBarRegion
				WriteBarRegion(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // FindCapability
				FindCapability(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // FindExtendedCapability
				FindExtendedCapability(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // MapDma
				MapDma(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // UnmapDma
				UnmapDma(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // UnmapDmaBusAddress
				UnmapDmaBusAddress(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // GetDmaBusAddress
				GetDmaBusAddress(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // GetDmaBusAddressRange
				GetDmaBusAddressRange(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // SetDmaEnable
				SetDmaEnable(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // AcquireIrq
				var _return = AcquireIrq(im.GetBytes(8, 0x8));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12: { // ReleaseIrq
				ReleaseIrq(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // SetIrqEnable
				SetIrqEnable(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // SetAspmEnable
				SetAspmEnable(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcie.Detail.ISession");
		}
	}
}

