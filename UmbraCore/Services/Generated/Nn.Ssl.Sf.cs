using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ssl.Sf;
public partial class ISslConnection : _ISslConnection_Base;
public abstract class _ISslConnection_Base : IpcInterface {
	protected virtual uint SetSocketDescriptor(uint _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.SetSocketDescriptor not implemented");
	protected virtual void SetHostName(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.SetHostName");
	protected virtual void SetVerifyOption(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.SetVerifyOption");
	protected virtual void SetIoMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.SetIoMode");
	protected virtual uint GetSocketDescriptor() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetSocketDescriptor not implemented");
	protected virtual void GetHostName() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetHostName not implemented");
	protected virtual uint GetVerifyOption() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetVerifyOption not implemented");
	protected virtual uint GetIoMode() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetIoMode not implemented");
	protected virtual void DoHandshake() =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.DoHandshake");
	protected virtual void DoHandshakeGetServerCert() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.DoHandshakeGetServerCert not implemented");
	protected virtual void Read() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Read not implemented");
	protected virtual uint Write(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Write not implemented");
	protected virtual uint Pending() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Pending not implemented");
	protected virtual void Peek() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Peek not implemented");
	protected virtual uint Poll(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Poll not implemented");
	protected virtual void GetVerifyCertError() =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.GetVerifyCertError");
	protected virtual uint GetNeededServerCertBufferSize() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetNeededServerCertBufferSize not implemented");
	protected virtual void SetSessionCacheMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.SetSessionCacheMode");
	protected virtual uint GetSessionCacheMode() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetSessionCacheMode not implemented");
	protected virtual void FlushSessionCache() =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.FlushSessionCache");
	protected virtual void SetRenegotiationMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.SetRenegotiationMode");
	protected virtual uint GetRenegotiationMode() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetRenegotiationMode not implemented");
	protected virtual void SetOption(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.SetOption");
	protected virtual byte GetOption(uint _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetOption not implemented");
	protected virtual void GetVerifyCertErrors() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetVerifyCertErrors not implemented");
	protected virtual void GetCipherInfo(uint _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetCipherInfo not implemented");
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
	protected virtual void SetOption(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslContext.SetOption");
	protected virtual uint GetOption(uint _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.GetOption not implemented");
	protected virtual Nn.Ssl.Sf.ISslConnection CreateConnection() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.CreateConnection not implemented");
	protected virtual uint GetConnectionCount() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.GetConnectionCount not implemented");
	protected virtual ulong ImportServerPki(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.ImportServerPki not implemented");
	protected virtual ulong ImportClientPki(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.ImportClientPki not implemented");
	protected virtual void RemoveServerPki(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslContext.RemoveServerPki");
	protected virtual void RemoveClientPki(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslContext.RemoveClientPki");
	protected virtual ulong RegisterInternalPki(uint _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.RegisterInternalPki not implemented");
	protected virtual void AddPolicyOid(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslContext.AddPolicyOid");
	protected virtual ulong ImportCrl(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslContext.ImportCrl not implemented");
	protected virtual void RemoveCrl(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslContext.RemoveCrl");
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
	protected virtual Nn.Ssl.Sf.ISslContext CreateContext(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.CreateContext not implemented");
	protected virtual uint GetContextCount() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.GetContextCount not implemented");
	protected virtual void GetCertificates(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.GetCertificates not implemented");
	protected virtual uint GetCertificateBufSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.GetCertificateBufSize not implemented");
	protected virtual void DebugIoctl(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.DebugIoctl not implemented");
	protected virtual void SetInterfaceVersion(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslService.SetInterfaceVersion");
	protected virtual void FlushSessionCache() =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslService.FlushSessionCache");
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

