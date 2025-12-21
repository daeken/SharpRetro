using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pl.Detail;
public partial class ISharedFontManager : _ISharedFontManager_Base {
	public readonly string ServiceName;
	public ISharedFontManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISharedFontManager_Base : IpcInterface {
	protected virtual void RequestLoad(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pl.Detail.ISharedFontManager.RequestLoad");
	protected virtual uint GetLoadState(uint _0) =>
		throw new NotImplementedException("Nn.Pl.Detail.ISharedFontManager.GetLoadState not implemented");
	protected virtual uint GetSize(uint _0) =>
		throw new NotImplementedException("Nn.Pl.Detail.ISharedFontManager.GetSize not implemented");
	protected virtual uint GetSharedMemoryAddressOffset(uint _0) =>
		throw new NotImplementedException("Nn.Pl.Detail.ISharedFontManager.GetSharedMemoryAddressOffset not implemented");
	protected virtual KObject GetSharedMemoryNativeHandle() =>
		throw new NotImplementedException("Nn.Pl.Detail.ISharedFontManager.GetSharedMemoryNativeHandle not implemented");
	protected virtual void GetSharedFontInOrderOfPriority(byte[] _0, out byte _1, out uint _2, Span<byte> _3, Span<byte> _4, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Pl.Detail.ISharedFontManager.GetSharedFontInOrderOfPriority not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestLoad
				RequestLoad(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetLoadState
				var _return = GetLoadState(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetSize
				var _return = GetSize(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetSharedMemoryAddressOffset
				var _return = GetSharedMemoryAddressOffset(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetSharedMemoryNativeHandle
				var _return = GetSharedMemoryNativeHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // GetSharedFontInOrderOfPriority
				GetSharedFontInOrderOfPriority(im.GetBytes(8, 0x8), out var _0, out var _1, im.GetSpan<byte>(0x6, 0), im.GetSpan<byte>(0x6, 1), im.GetSpan<byte>(0x6, 2));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pl.Detail.ISharedFontManager");
		}
	}
}

