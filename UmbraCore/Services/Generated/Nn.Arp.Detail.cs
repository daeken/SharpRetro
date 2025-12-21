using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Arp.Detail;
public partial class IReader : _IReader_Base {
	public readonly string ServiceName;
	public IReader(string serviceName) => ServiceName = serviceName;
}
public abstract class _IReader_Base : IpcInterface {
	protected virtual void GetApplicationLaunchProperty(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationLaunchProperty not implemented");
	protected virtual void GetApplicationLaunchPropertyWithApplicationId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationLaunchPropertyWithApplicationId not implemented");
	protected virtual void GetApplicationControlProperty(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationControlProperty not implemented");
	protected virtual void GetApplicationControlPropertyWithApplicationId(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Arp.Detail.IReader.GetApplicationControlPropertyWithApplicationId not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetApplicationLaunchProperty
				GetApplicationLaunchProperty(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetApplicationLaunchPropertyWithApplicationId
				GetApplicationLaunchPropertyWithApplicationId(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // GetApplicationControlProperty
				GetApplicationControlProperty(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetApplicationControlPropertyWithApplicationId
				GetApplicationControlPropertyWithApplicationId(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IReader");
		}
	}
}

public partial class IRegistrar : _IRegistrar_Base;
public abstract class _IRegistrar_Base : IpcInterface {
	protected virtual void Issue(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IRegistrar.Issue");
	protected virtual void SetApplicationLaunchProperty(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IRegistrar.SetApplicationLaunchProperty");
	protected virtual void SetApplicationControlProperty(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IRegistrar.SetApplicationControlProperty");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Issue
				Issue(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // SetApplicationLaunchProperty
				SetApplicationLaunchProperty(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SetApplicationControlProperty
				SetApplicationControlProperty(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IRegistrar");
		}
	}
}

public partial class IWriter : _IWriter_Base {
	public readonly string ServiceName;
	public IWriter(string serviceName) => ServiceName = serviceName;
}
public abstract class _IWriter_Base : IpcInterface {
	protected virtual Nn.Arp.Detail.IRegistrar AcquireRegistrar() =>
		throw new NotImplementedException("Nn.Arp.Detail.IWriter.AcquireRegistrar not implemented");
	protected virtual void DeleteProperties(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Arp.Detail.IWriter.DeleteProperties");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // AcquireRegistrar
				var _return = AcquireRegistrar();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // DeleteProperties
				DeleteProperties(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IWriter");
		}
	}
}

