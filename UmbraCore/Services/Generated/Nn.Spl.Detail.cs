using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Spl.Detail;
public partial class ICryptoInterface : _ICryptoInterface_Base;
public abstract class _ICryptoInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown3");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown7 not implemented");
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown11 not implemented");
	protected virtual void Unknown14(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown15 not implemented");
	protected virtual void Unknown16(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown16 not implemented");
	protected virtual void Unknown21(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown21 not implemented");
	protected virtual void Unknown22(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown23 not implemented");
	protected virtual void Unknown24(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown24");
	protected virtual void Unknown25(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown25 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x20), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(im.GetBytes(8, 0x14), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // Unknown23
				var _return = Unknown23();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.ICryptoInterface");
		}
	}
}

public partial class IEsInterface : _IEsInterface_Base;
public abstract class _IEsInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown3");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown7 not implemented");
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown11 not implemented");
	protected virtual void Unknown13(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown13 not implemented");
	protected virtual void Unknown14(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown15 not implemented");
	protected virtual void Unknown16(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown16 not implemented");
	protected virtual void Unknown17(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown17");
	protected virtual void Unknown18(byte[] _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, out byte[] _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown18 not implemented");
	protected virtual void Unknown20(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown20 not implemented");
	protected virtual void Unknown21(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown21 not implemented");
	protected virtual void Unknown22(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown23 not implemented");
	protected virtual void Unknown24(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown24");
	protected virtual void Unknown25(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown25 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x20), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				Unknown13(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(im.GetBytes(8, 0x14), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x11: { // Unknown17
				Unknown17(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // Unknown18
				Unknown18(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // Unknown20
				Unknown20(im.GetBytes(8, 0x14), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // Unknown23
				var _return = Unknown23();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IEsInterface");
		}
	}
}

public partial class IFsInterface : _IFsInterface_Base;
public abstract class _IFsInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown3");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown7 not implemented");
	protected virtual void Unknown9(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown9");
	protected virtual void Unknown10(Span<byte> _0, Span<byte> _1, Span<byte> _2, out byte[] _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown10 not implemented");
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown11 not implemented");
	protected virtual void Unknown12(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown12 not implemented");
	protected virtual void Unknown14(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown15 not implemented");
	protected virtual void Unknown16(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown16 not implemented");
	protected virtual void Unknown19(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown19");
	protected virtual void Unknown21(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown21 not implemented");
	protected virtual void Unknown22(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown23 not implemented");
	protected virtual void Unknown24(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown24");
	protected virtual void Unknown25(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown25 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x20), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(im.GetBytes(8, 0x14), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // Unknown19
				Unknown19(im.GetBytes(8, 0x14));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // Unknown23
				var _return = Unknown23();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IFsInterface");
		}
	}
}

public partial class IGeneralInterface : _IGeneralInterface_Base {
	public readonly string ServiceName;
	public IGeneralInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IGeneralInterface_Base : IpcInterface {
	protected virtual void GetConfig(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetConfig not implemented");
	protected virtual void UserExpMod(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.UserExpMod not implemented");
	protected virtual void GenerateAesKek(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GenerateAesKek not implemented");
	protected virtual void LoadAesKey(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadAesKey");
	protected virtual void GenerateAesKey(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GenerateAesKey not implemented");
	protected virtual void SetConfig(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.SetConfig");
	protected virtual void GetRandomBytes(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetRandomBytes not implemented");
	protected virtual void LoadSecureExpModKey(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadSecureExpModKey");
	protected virtual void SecureExpMod(Span<byte> _0, Span<byte> _1, Span<byte> _2, out byte[] _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.SecureExpMod not implemented");
	protected virtual void IsDevelopment(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.IsDevelopment not implemented");
	protected virtual void GenerateSpecificAesKey(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GenerateSpecificAesKey not implemented");
	protected virtual void DecryptRsaPrivateKey(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.DecryptRsaPrivateKey not implemented");
	protected virtual void DecryptAesKey(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.DecryptAesKey not implemented");
	protected virtual void DecryptAesCtr(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.DecryptAesCtr not implemented");
	protected virtual void ComputeCmac(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.ComputeCmac not implemented");
	protected virtual void LoadRsaOaepKey(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadRsaOaepKey");
	protected virtual void UnwrapRsaOaepWrappedTitleKey(byte[] _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, out byte[] _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.UnwrapRsaOaepWrappedTitleKey not implemented");
	protected virtual void LoadTitleKey(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadTitleKey");
	protected virtual void UnwrapAesWrappedTitleKey(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.UnwrapAesWrappedTitleKey not implemented");
	protected virtual void LockAesEngine(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.LockAesEngine not implemented");
	protected virtual void UnlockAesEngine(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.UnlockAesEngine");
	protected virtual KObject GetSplWaitEvent() =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetSplWaitEvent not implemented");
	protected virtual void SetSharedData(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.SetSharedData");
	protected virtual void GetSharedData(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetSharedData not implemented");
	protected virtual void ImportSslRsaKey() =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.ImportSslRsaKey");
	protected virtual void SecureExpModWithSslKey() =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.SecureExpModWithSslKey");
	protected virtual void ImportEsRsaKey() =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.ImportEsRsaKey");
	protected virtual void SecureExpModWithEsKey() =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.SecureExpModWithEsKey");
	protected virtual void EncryptManuRsaKeyForImport() =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.EncryptManuRsaKeyForImport");
	protected virtual void GetPackage2Hash() =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.GetPackage2Hash");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetConfig
				GetConfig(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // UserExpMod
				UserExpMod(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GenerateAesKek
				GenerateAesKek(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // LoadAesKey
				LoadAesKey(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GenerateAesKey
				GenerateAesKey(im.GetBytes(8, 0x20), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // SetConfig
				SetConfig(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // GetRandomBytes
				GetRandomBytes(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // LoadSecureExpModKey
				LoadSecureExpModKey(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // SecureExpMod
				SecureExpMod(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // IsDevelopment
				IsDevelopment(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GenerateSpecificAesKey
				GenerateSpecificAesKey(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // DecryptRsaPrivateKey
				DecryptRsaPrivateKey(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // DecryptAesKey
				DecryptAesKey(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // DecryptAesCtr
				DecryptAesCtr(im.GetBytes(8, 0x14), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // ComputeCmac
				ComputeCmac(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x11: { // LoadRsaOaepKey
				LoadRsaOaepKey(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // UnwrapRsaOaepWrappedTitleKey
				UnwrapRsaOaepWrappedTitleKey(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // LoadTitleKey
				LoadTitleKey(im.GetBytes(8, 0x14));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // UnwrapAesWrappedTitleKey
				UnwrapAesWrappedTitleKey(im.GetBytes(8, 0x14), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // LockAesEngine
				LockAesEngine(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // UnlockAesEngine
				UnlockAesEngine(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // GetSplWaitEvent
				var _return = GetSplWaitEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // SetSharedData
				SetSharedData(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // GetSharedData
				GetSharedData(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // ImportSslRsaKey
				ImportSslRsaKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // SecureExpModWithSslKey
				SecureExpModWithSslKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1C: { // ImportEsRsaKey
				ImportEsRsaKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1D: { // SecureExpModWithEsKey
				SecureExpModWithEsKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // EncryptManuRsaKeyForImport
				EncryptManuRsaKeyForImport();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // GetPackage2Hash
				GetPackage2Hash();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IGeneralInterface");
		}
	}
}

public partial class IRandomInterface : _IRandomInterface_Base {
	public readonly string ServiceName;
	public IRandomInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IRandomInterface_Base : IpcInterface {
	protected virtual void GetRandomBytes(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IRandomInterface.GetRandomBytes not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetRandomBytes
				GetRandomBytes(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IRandomInterface");
		}
	}
}

public partial class ISslInterface : _ISslInterface_Base;
public abstract class _ISslInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown3");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown7 not implemented");
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown11 not implemented");
	protected virtual void Unknown13(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown13 not implemented");
	protected virtual void Unknown14(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown15 not implemented");
	protected virtual void Unknown16(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown16 not implemented");
	protected virtual void Unknown21(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown21 not implemented");
	protected virtual void Unknown22(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown23 not implemented");
	protected virtual void Unknown24(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown24");
	protected virtual void Unknown25(out byte[] _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown25 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x9, 1), im.GetSpan<byte>(0x9, 2), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x20), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				Unknown13(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(im.GetBytes(8, 0x14), im.GetSpan<byte>(0x45, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // Unknown16
				Unknown16(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // Unknown23
				var _return = Unknown23();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.ISslInterface");
		}
	}
}

