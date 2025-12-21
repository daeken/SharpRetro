using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pl.Detail;
public partial class ISharedFontManager : _ISharedFontManager_Base;
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
	protected virtual void GetSharedFontInOrderOfPriority(Span<byte> _0, out byte _1, out uint _2, Span<byte> _3, Span<byte> _4, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Pl.Detail.ISharedFontManager.GetSharedFontInOrderOfPriority not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RequestLoad
				break;
			}
			case 0x1: { // GetLoadState
				break;
			}
			case 0x2: { // GetSize
				break;
			}
			case 0x3: { // GetSharedMemoryAddressOffset
				break;
			}
			case 0x4: { // GetSharedMemoryNativeHandle
				break;
			}
			case 0x5: { // GetSharedFontInOrderOfPriority
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pl.Detail.ISharedFontManager");
		}
	}
}

