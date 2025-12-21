using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcie.Detail;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void RegisterClassDriver(Span<byte> _0, KObject _1) =>
		throw new NotImplementedException("Nn.Pcie.Detail.IManager.RegisterClassDriver not implemented");
	protected virtual void QueryFunctionsUnregistered() =>
		throw new NotImplementedException("Nn.Pcie.Detail.IManager.QueryFunctionsUnregistered not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterClassDriver
				break;
			case 0x1: // QueryFunctionsUnregistered
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcie.Detail.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void QueryFunctions() =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.QueryFunctions not implemented");
	protected virtual KObject AcquireFunction(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.AcquireFunction not implemented");
	protected virtual void ReleaseFunction(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.ReleaseFunction");
	protected virtual void GetFunctionState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetFunctionState not implemented");
	protected virtual void GetBarProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetBarProfile not implemented");
	protected virtual void ReadConfig(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.ReadConfig not implemented");
	protected virtual void WriteConfig(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.WriteConfig");
	protected virtual void ReadBarRegion(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.ReadBarRegion not implemented");
	protected virtual void WriteBarRegion(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.WriteBarRegion");
	protected virtual void FindCapability(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.FindCapability not implemented");
	protected virtual void FindExtendedCapability(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.FindExtendedCapability not implemented");
	protected virtual void MapDma(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.MapDma not implemented");
	protected virtual void UnmapDma(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.UnmapDma");
	protected virtual void UnmapDmaBusAddress(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.UnmapDmaBusAddress");
	protected virtual void GetDmaBusAddress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetDmaBusAddress not implemented");
	protected virtual void GetDmaBusAddressRange(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.GetDmaBusAddressRange not implemented");
	protected virtual void SetDmaEnable(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.SetDmaEnable");
	protected virtual KObject AcquireIrq(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pcie.Detail.ISession.AcquireIrq not implemented");
	protected virtual void ReleaseIrq(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.ReleaseIrq");
	protected virtual void SetIrqEnable(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.SetIrqEnable");
	protected virtual void SetAspmEnable(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pcie.Detail.ISession.SetAspmEnable");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // QueryFunctions
				break;
			case 0x1: // AcquireFunction
				break;
			case 0x2: // ReleaseFunction
				break;
			case 0x3: // GetFunctionState
				break;
			case 0x4: // GetBarProfile
				break;
			case 0x5: // ReadConfig
				break;
			case 0x6: // WriteConfig
				break;
			case 0x7: // ReadBarRegion
				break;
			case 0x8: // WriteBarRegion
				break;
			case 0x9: // FindCapability
				break;
			case 0xA: // FindExtendedCapability
				break;
			case 0xB: // MapDma
				break;
			case 0xC: // UnmapDma
				break;
			case 0xD: // UnmapDmaBusAddress
				break;
			case 0xE: // GetDmaBusAddress
				break;
			case 0xF: // GetDmaBusAddressRange
				break;
			case 0x10: // SetDmaEnable
				break;
			case 0x11: // AcquireIrq
				break;
			case 0x12: // ReleaseIrq
				break;
			case 0x13: // SetIrqEnable
				break;
			case 0x14: // SetAspmEnable
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcie.Detail.ISession");
		}
	}
}

