using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Eth.Sf;
public partial class IEthInterface : _IEthInterface_Base;
public abstract class _IEthInterface_Base : IpcInterface {
	protected virtual KObject Initialize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterface.Initialize not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Eth.Sf.IEthInterface.Cancel");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Eth.Sf.IEthInterface.GetResult");
	protected virtual void GetMediaList() =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterface.GetMediaList not implemented");
	protected virtual void SetMediaType(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Eth.Sf.IEthInterface.SetMediaType");
	protected virtual void GetMediaType() =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterface.GetMediaType not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // GetMediaList
				break;
			case 0x4: // SetMediaType
				break;
			case 0x5: // GetMediaType
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eth.Sf.IEthInterface");
		}
	}
}

public partial class IEthInterfaceGroup : _IEthInterfaceGroup_Base;
public abstract class _IEthInterfaceGroup_Base : IpcInterface {
	protected virtual KObject GetReadableHandle() =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterfaceGroup.GetReadableHandle not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Eth.Sf.IEthInterfaceGroup.Cancel");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Eth.Sf.IEthInterfaceGroup.GetResult");
	protected virtual void GetInterfaceList() =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterfaceGroup.GetInterfaceList not implemented");
	protected virtual void GetInterfaceCount() =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterfaceGroup.GetInterfaceCount not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetReadableHandle
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // GetInterfaceList
				break;
			case 0x4: // GetInterfaceCount
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eth.Sf.IEthInterfaceGroup");
		}
	}
}

