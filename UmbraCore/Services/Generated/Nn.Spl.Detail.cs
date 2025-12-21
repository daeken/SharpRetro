using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Spl.Detail;
public partial class ICryptoInterface : _ICryptoInterface_Base;
public abstract class _ICryptoInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown3");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown7 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown11 not implemented");
	protected virtual void Unknown14(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown14 not implemented");
	protected virtual void Unknown15(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown15 not implemented");
	protected virtual void Unknown16(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown16 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown21 not implemented");
	protected virtual void Unknown22(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown23 not implemented");
	protected virtual void Unknown24(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ICryptoInterface.Unknown24");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ICryptoInterface.Unknown25 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // Unknown16
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			case 0x16: { // Unknown22
				break;
			}
			case 0x17: { // Unknown23
				break;
			}
			case 0x18: { // Unknown24
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.ICryptoInterface");
		}
	}
}

public partial class IEsInterface : _IEsInterface_Base;
public abstract class _IEsInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown3");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown7 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown11 not implemented");
	protected virtual void Unknown13(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown13 not implemented");
	protected virtual void Unknown14(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown14 not implemented");
	protected virtual void Unknown15(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown15 not implemented");
	protected virtual void Unknown16(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown16 not implemented");
	protected virtual void Unknown17(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown17");
	protected virtual void Unknown18(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown18 not implemented");
	protected virtual void Unknown20(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown20 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown21 not implemented");
	protected virtual void Unknown22(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown23 not implemented");
	protected virtual void Unknown24(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IEsInterface.Unknown24");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IEsInterface.Unknown25 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			case 0xD: { // Unknown13
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // Unknown16
				break;
			}
			case 0x11: { // Unknown17
				break;
			}
			case 0x12: { // Unknown18
				break;
			}
			case 0x14: { // Unknown20
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			case 0x16: { // Unknown22
				break;
			}
			case 0x17: { // Unknown23
				break;
			}
			case 0x18: { // Unknown24
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IEsInterface");
		}
	}
}

public partial class IFsInterface : _IFsInterface_Base;
public abstract class _IFsInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown3");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown7 not implemented");
	protected virtual void Unknown9(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown9");
	protected virtual void Unknown10(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown10 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown11 not implemented");
	protected virtual void Unknown12(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown12 not implemented");
	protected virtual void Unknown14(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown14 not implemented");
	protected virtual void Unknown15(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown15 not implemented");
	protected virtual void Unknown16(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown16 not implemented");
	protected virtual void Unknown19(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown19");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown21 not implemented");
	protected virtual void Unknown22(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown23 not implemented");
	protected virtual void Unknown24(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IFsInterface.Unknown24");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IFsInterface.Unknown25 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0x9: { // Unknown9
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			case 0xC: { // Unknown12
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // Unknown16
				break;
			}
			case 0x13: { // Unknown19
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			case 0x16: { // Unknown22
				break;
			}
			case 0x17: { // Unknown23
				break;
			}
			case 0x18: { // Unknown24
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IFsInterface");
		}
	}
}

public partial class IGeneralInterface : _IGeneralInterface_Base;
public abstract class _IGeneralInterface_Base : IpcInterface {
	protected virtual void GetConfig(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetConfig not implemented");
	protected virtual void UserExpMod(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.UserExpMod not implemented");
	protected virtual void GenerateAesKek(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GenerateAesKek not implemented");
	protected virtual void LoadAesKey(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadAesKey");
	protected virtual void GenerateAesKey(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GenerateAesKey not implemented");
	protected virtual void SetConfig(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.SetConfig");
	protected virtual void GetRandomBytes(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetRandomBytes not implemented");
	protected virtual void LoadSecureExpModKey(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadSecureExpModKey");
	protected virtual void SecureExpMod(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.SecureExpMod not implemented");
	protected virtual void IsDevelopment(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.IsDevelopment not implemented");
	protected virtual void GenerateSpecificAesKey(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GenerateSpecificAesKey not implemented");
	protected virtual void DecryptRsaPrivateKey(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.DecryptRsaPrivateKey not implemented");
	protected virtual void DecryptAesKey(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.DecryptAesKey not implemented");
	protected virtual void DecryptAesCtr(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.DecryptAesCtr not implemented");
	protected virtual void ComputeCmac(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.ComputeCmac not implemented");
	protected virtual void LoadRsaOaepKey(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadRsaOaepKey");
	protected virtual void UnwrapRsaOaepWrappedTitleKey(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.UnwrapRsaOaepWrappedTitleKey not implemented");
	protected virtual void LoadTitleKey(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.LoadTitleKey");
	protected virtual void UnwrapAesWrappedTitleKey(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.UnwrapAesWrappedTitleKey not implemented");
	protected virtual void LockAesEngine(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.LockAesEngine not implemented");
	protected virtual void UnlockAesEngine(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.UnlockAesEngine");
	protected virtual KObject GetSplWaitEvent() =>
		throw new NotImplementedException("Nn.Spl.Detail.IGeneralInterface.GetSplWaitEvent not implemented");
	protected virtual void SetSharedData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.IGeneralInterface.SetSharedData");
	protected virtual void GetSharedData(Span<byte> _0) =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetConfig
				break;
			}
			case 0x1: { // UserExpMod
				break;
			}
			case 0x2: { // GenerateAesKek
				break;
			}
			case 0x3: { // LoadAesKey
				break;
			}
			case 0x4: { // GenerateAesKey
				break;
			}
			case 0x5: { // SetConfig
				break;
			}
			case 0x7: { // GetRandomBytes
				break;
			}
			case 0x9: { // LoadSecureExpModKey
				break;
			}
			case 0xA: { // SecureExpMod
				break;
			}
			case 0xB: { // IsDevelopment
				break;
			}
			case 0xC: { // GenerateSpecificAesKey
				break;
			}
			case 0xD: { // DecryptRsaPrivateKey
				break;
			}
			case 0xE: { // DecryptAesKey
				break;
			}
			case 0xF: { // DecryptAesCtr
				break;
			}
			case 0x10: { // ComputeCmac
				break;
			}
			case 0x11: { // LoadRsaOaepKey
				break;
			}
			case 0x12: { // UnwrapRsaOaepWrappedTitleKey
				break;
			}
			case 0x13: { // LoadTitleKey
				break;
			}
			case 0x14: { // UnwrapAesWrappedTitleKey
				break;
			}
			case 0x15: { // LockAesEngine
				break;
			}
			case 0x16: { // UnlockAesEngine
				break;
			}
			case 0x17: { // GetSplWaitEvent
				break;
			}
			case 0x18: { // SetSharedData
				break;
			}
			case 0x19: { // GetSharedData
				break;
			}
			case 0x1A: { // ImportSslRsaKey
				break;
			}
			case 0x1B: { // SecureExpModWithSslKey
				break;
			}
			case 0x1C: { // ImportEsRsaKey
				break;
			}
			case 0x1D: { // SecureExpModWithEsKey
				break;
			}
			case 0x1E: { // EncryptManuRsaKeyForImport
				break;
			}
			case 0x1F: { // GetPackage2Hash
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IGeneralInterface");
		}
	}
}

public partial class IRandomInterface : _IRandomInterface_Base;
public abstract class _IRandomInterface_Base : IpcInterface {
	protected virtual void GetRandomBytes(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.IRandomInterface.GetRandomBytes not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetRandomBytes
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IRandomInterface");
		}
	}
}

public partial class ISslInterface : _ISslInterface_Base;
public abstract class _ISslInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown3");
	protected virtual void Unknown4(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown5");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown7 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown11 not implemented");
	protected virtual void Unknown13(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown13 not implemented");
	protected virtual void Unknown14(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown14 not implemented");
	protected virtual void Unknown15(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown15 not implemented");
	protected virtual void Unknown16(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown16 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown21 not implemented");
	protected virtual void Unknown22(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown22");
	protected virtual KObject Unknown23() =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown23 not implemented");
	protected virtual void Unknown24(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Spl.Detail.ISslInterface.Unknown24");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Spl.Detail.ISslInterface.Unknown25 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			case 0xD: { // Unknown13
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // Unknown16
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			case 0x16: { // Unknown22
				break;
			}
			case 0x17: { // Unknown23
				break;
			}
			case 0x18: { // Unknown24
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.ISslInterface");
		}
	}
}

