using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Arp.Detail;
public partial class IReader : _IReader_Base;
public abstract class _IReader_Base : IpcInterface {
	protected virtual void GetApplicationLaunchProperty(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationLaunchProperty not implemented");
	protected virtual void GetApplicationLaunchPropertyWithApplicationId(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationLaunchPropertyWithApplicationId not implemented");
	protected virtual void GetApplicationControlProperty(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationControlProperty not implemented");
	protected virtual void GetApplicationControlPropertyWithApplicationId(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationControlPropertyWithApplicationId not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetApplicationLaunchProperty
				break;
			}
			case 0x1: { // GetApplicationLaunchPropertyWithApplicationId
				break;
			}
			case 0x2: { // GetApplicationControlProperty
				break;
			}
			case 0x3: { // GetApplicationControlPropertyWithApplicationId
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IReader");
		}
	}
}

public partial class IRegistrar : _IRegistrar_Base;
public abstract class _IRegistrar_Base : IpcInterface {
	protected virtual void Issue(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IRegistrar.Issue");
	protected virtual void SetApplicationLaunchProperty(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IRegistrar.SetApplicationLaunchProperty");
	protected virtual void SetApplicationControlProperty(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IRegistrar.SetApplicationControlProperty");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Issue
				break;
			}
			case 0x1: { // SetApplicationLaunchProperty
				break;
			}
			case 0x2: { // SetApplicationControlProperty
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IRegistrar");
		}
	}
}

public partial class IWriter : _IWriter_Base;
public abstract class _IWriter_Base : IpcInterface {
	protected virtual Nn.Arp.Detail.IRegistrar AcquireRegistrar() =>
		throw new NotImplementedException("Nn.Arp.Detail.IWriter.AcquireRegistrar not implemented");
	protected virtual void DeleteProperties(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IWriter.DeleteProperties");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // AcquireRegistrar
				break;
			}
			case 0x1: { // DeleteProperties
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IWriter");
		}
	}
}

