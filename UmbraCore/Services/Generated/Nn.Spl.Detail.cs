using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Spl.Detail;
public partial class ICryptoInterface : _ICryptoInterface_Base;
public abstract class _ICryptoInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x7: // Unknown7
				break;
			case 0xB: // Unknown11
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // Unknown16
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			case 0x18: // Unknown24
				break;
			case 0x19: // Unknown25
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.ICryptoInterface");
		}
	}
}

public partial class IEsInterface : _IEsInterface_Base;
public abstract class _IEsInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x7: // Unknown7
				break;
			case 0xB: // Unknown11
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			case 0x12: // Unknown18
				break;
			case 0x14: // Unknown20
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			case 0x18: // Unknown24
				break;
			case 0x19: // Unknown25
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IEsInterface");
		}
	}
}

public partial class IFsInterface : _IFsInterface_Base;
public abstract class _IFsInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x7: // Unknown7
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // Unknown16
				break;
			case 0x13: // Unknown19
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			case 0x18: // Unknown24
				break;
			case 0x19: // Unknown25
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IFsInterface");
		}
	}
}

public partial class IGeneralInterface : _IGeneralInterface_Base;
public abstract class _IGeneralInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetConfig
				break;
			case 0x1: // UserExpMod
				break;
			case 0x2: // GenerateAesKek
				break;
			case 0x3: // LoadAesKey
				break;
			case 0x4: // GenerateAesKey
				break;
			case 0x5: // SetConfig
				break;
			case 0x7: // GetRandomBytes
				break;
			case 0x9: // LoadSecureExpModKey
				break;
			case 0xA: // SecureExpMod
				break;
			case 0xB: // IsDevelopment
				break;
			case 0xC: // GenerateSpecificAesKey
				break;
			case 0xD: // DecryptRsaPrivateKey
				break;
			case 0xE: // DecryptAesKey
				break;
			case 0xF: // DecryptAesCtr
				break;
			case 0x10: // ComputeCmac
				break;
			case 0x11: // LoadRsaOaepKey
				break;
			case 0x12: // UnwrapRsaOaepWrappedTitleKey
				break;
			case 0x13: // LoadTitleKey
				break;
			case 0x14: // UnwrapAesWrappedTitleKey
				break;
			case 0x15: // LockAesEngine
				break;
			case 0x16: // UnlockAesEngine
				break;
			case 0x17: // GetSplWaitEvent
				break;
			case 0x18: // SetSharedData
				break;
			case 0x19: // GetSharedData
				break;
			case 0x1A: // ImportSslRsaKey
				break;
			case 0x1B: // SecureExpModWithSslKey
				break;
			case 0x1C: // ImportEsRsaKey
				break;
			case 0x1D: // SecureExpModWithEsKey
				break;
			case 0x1E: // EncryptManuRsaKeyForImport
				break;
			case 0x1F: // GetPackage2Hash
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IGeneralInterface");
		}
	}
}

public partial class IRandomInterface : _IRandomInterface_Base;
public abstract class _IRandomInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetRandomBytes
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.IRandomInterface");
		}
	}
}

public partial class ISslInterface : _ISslInterface_Base;
public abstract class _ISslInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x7: // Unknown7
				break;
			case 0xB: // Unknown11
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // Unknown16
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			case 0x18: // Unknown24
				break;
			case 0x19: // Unknown25
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spl.Detail.ISslInterface");
		}
	}
}

