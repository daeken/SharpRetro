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
	protected virtual void GetHostName(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetHostName not implemented");
	protected virtual uint GetVerifyOption() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetVerifyOption not implemented");
	protected virtual uint GetIoMode() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetIoMode not implemented");
	protected virtual void DoHandshake() =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslConnection.DoHandshake");
	protected virtual void DoHandshakeGetServerCert(out uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.DoHandshakeGetServerCert not implemented");
	protected virtual void Read(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Read not implemented");
	protected virtual uint Write(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Write not implemented");
	protected virtual uint Pending() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.Pending not implemented");
	protected virtual void Peek(out uint _0, Span<byte> _1) =>
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
	protected virtual void GetVerifyCertErrors(out uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetVerifyCertErrors not implemented");
	protected virtual void GetCipherInfo(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslConnection.GetCipherInfo not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetSocketDescriptor
				var _return = SetSocketDescriptor(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // SetHostName
				SetHostName(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SetVerifyOption
				SetVerifyOption(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // SetIoMode
				SetIoMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetSocketDescriptor
				var _return = GetSocketDescriptor();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetHostName
				GetHostName(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // GetVerifyOption
				var _return = GetVerifyOption();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // GetIoMode
				var _return = GetIoMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // DoHandshake
				DoHandshake();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // DoHandshakeGetServerCert
				DoHandshakeGetServerCert(out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xA: { // Read
				Read(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xB: { // Write
				var _return = Write(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // Pending
				var _return = Pending();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // Peek
				Peek(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xE: { // Poll
				var _return = Poll(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xF: { // GetVerifyCertError
				GetVerifyCertError();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetNeededServerCertBufferSize
				var _return = GetNeededServerCertBufferSize();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x11: { // SetSessionCacheMode
				SetSessionCacheMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // GetSessionCacheMode
				var _return = GetSessionCacheMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x13: { // FlushSessionCache
				FlushSessionCache();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // SetRenegotiationMode
				SetRenegotiationMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // GetRenegotiationMode
				var _return = GetRenegotiationMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // SetOption
				SetOption(im.GetData<byte>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // GetOption
				var _return = GetOption(im.GetData<uint>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x18: { // GetVerifyCertErrors
				GetVerifyCertErrors(out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x19: { // GetCipherInfo
				GetCipherInfo(im.GetData<uint>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetOption
				SetOption(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetOption
				var _return = GetOption(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // CreateConnection
				var _return = CreateConnection();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetConnectionCount
				var _return = GetConnectionCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // ImportServerPki
				var _return = ImportServerPki(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // ImportClientPki
				var _return = ImportClientPki(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // RemoveServerPki
				RemoveServerPki(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // RemoveClientPki
				RemoveClientPki(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // RegisterInternalPki
				var _return = RegisterInternalPki(im.GetData<uint>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // AddPolicyOid
				AddPolicyOid(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // ImportCrl
				var _return = ImportCrl(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0xB: { // RemoveCrl
				RemoveCrl(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ssl.Sf.ISslContext");
		}
	}
}

public partial class ISslService : _ISslService_Base {
	public readonly string ServiceName;
	public ISslService(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISslService_Base : IpcInterface {
	protected virtual Nn.Ssl.Sf.ISslContext CreateContext(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.CreateContext not implemented");
	protected virtual uint GetContextCount() =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.GetContextCount not implemented");
	protected virtual void GetCertificates(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.GetCertificates not implemented");
	protected virtual uint GetCertificateBufSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.GetCertificateBufSize not implemented");
	protected virtual void DebugIoctl(ulong _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ssl.Sf.ISslService.DebugIoctl not implemented");
	protected virtual void SetInterfaceVersion(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslService.SetInterfaceVersion");
	protected virtual void FlushSessionCache() =>
		Console.WriteLine("Stub hit for Nn.Ssl.Sf.ISslService.FlushSessionCache");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateContext
				var _return = CreateContext(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetContextCount
				var _return = GetContextCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetCertificates
				GetCertificates(im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // GetCertificateBufSize
				var _return = GetCertificateBufSize(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // DebugIoctl
				DebugIoctl(im.GetData<ulong>(8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // SetInterfaceVersion
				SetInterfaceVersion(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // FlushSessionCache
				FlushSessionCache();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ssl.Sf.ISslService");
		}
	}
}

