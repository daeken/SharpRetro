using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ssl.Sf;
public partial class ISslConnection : _ISslConnection_Base;
public abstract class _ISslConnection_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetSocketDescriptor
				break;
			case 0x1: // SetHostName
				break;
			case 0x2: // SetVerifyOption
				break;
			case 0x3: // SetIoMode
				break;
			case 0x4: // GetSocketDescriptor
				break;
			case 0x5: // GetHostName
				break;
			case 0x6: // GetVerifyOption
				break;
			case 0x7: // GetIoMode
				break;
			case 0x8: // DoHandshake
				break;
			case 0x9: // DoHandshakeGetServerCert
				break;
			case 0xA: // Read
				break;
			case 0xB: // Write
				break;
			case 0xC: // Pending
				break;
			case 0xD: // Peek
				break;
			case 0xE: // Poll
				break;
			case 0xF: // GetVerifyCertError
				break;
			case 0x10: // GetNeededServerCertBufferSize
				break;
			case 0x11: // SetSessionCacheMode
				break;
			case 0x12: // GetSessionCacheMode
				break;
			case 0x13: // FlushSessionCache
				break;
			case 0x14: // SetRenegotiationMode
				break;
			case 0x15: // GetRenegotiationMode
				break;
			case 0x16: // SetOption
				break;
			case 0x17: // GetOption
				break;
			case 0x18: // GetVerifyCertErrors
				break;
			case 0x19: // GetCipherInfo
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ssl.Sf.ISslConnection");
		}
	}
}

public partial class ISslContext : _ISslContext_Base;
public abstract class _ISslContext_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetOption
				break;
			case 0x1: // GetOption
				break;
			case 0x2: // CreateConnection
				break;
			case 0x3: // GetConnectionCount
				break;
			case 0x4: // ImportServerPki
				break;
			case 0x5: // ImportClientPki
				break;
			case 0x6: // RemoveServerPki
				break;
			case 0x7: // RemoveClientPki
				break;
			case 0x8: // RegisterInternalPki
				break;
			case 0x9: // AddPolicyOid
				break;
			case 0xA: // ImportCrl
				break;
			case 0xB: // RemoveCrl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ssl.Sf.ISslContext");
		}
	}
}

public partial class ISslService : _ISslService_Base;
public abstract class _ISslService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateContext
				break;
			case 0x1: // GetContextCount
				break;
			case 0x2: // GetCertificates
				break;
			case 0x3: // GetCertificateBufSize
				break;
			case 0x4: // DebugIoctl
				break;
			case 0x5: // SetInterfaceVersion
				break;
			case 0x6: // FlushSessionCache
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ssl.Sf.ISslService");
		}
	}
}

