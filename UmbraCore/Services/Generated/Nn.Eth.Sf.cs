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
	protected virtual void GetMediaList(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterface.GetMediaList not implemented");
	protected virtual void SetMediaType(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Eth.Sf.IEthInterface.SetMediaType");
	protected virtual void GetMediaType(out byte[] _0) =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterface.GetMediaType not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				om.Initialize(0, 1, 0);
				var _return = Initialize(im.GetSpan<byte>(0x5, 0));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Cancel
				om.Initialize(0, 0, 0);
				Cancel();
				break;
			}
			case 0x2: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x3: { // GetMediaList
				om.Initialize(0, 0, 0);
				GetMediaList(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x4: { // SetMediaType
				om.Initialize(0, 0, 0);
				SetMediaType(im.GetBytes(8, 0x4));
				break;
			}
			case 0x5: { // GetMediaType
				om.Initialize(0, 0, 12);
				GetMediaType(out var _0);
				om.SetBytes(8, _0);
				break;
			}
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
	protected virtual void GetInterfaceList(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterfaceGroup.GetInterfaceList not implemented");
	protected virtual void GetInterfaceCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Eth.Sf.IEthInterfaceGroup.GetInterfaceCount not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetReadableHandle
				om.Initialize(0, 1, 0);
				var _return = GetReadableHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Cancel
				om.Initialize(0, 0, 0);
				Cancel();
				break;
			}
			case 0x2: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x3: { // GetInterfaceList
				om.Initialize(0, 0, 0);
				GetInterfaceList(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x4: { // GetInterfaceCount
				om.Initialize(0, 0, 4);
				GetInterfaceCount(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eth.Sf.IEthInterfaceGroup");
		}
	}
}

