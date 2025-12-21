using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Es;
public partial class IETicketService : _IETicketService_Base;
public abstract class _IETicketService_Base : IpcInterface {
	protected virtual void ImportTicket(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.ImportTicket");
	protected virtual void ImportTicketCertificateSet(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.ImportTicketCertificateSet");
	protected virtual void DeleteTicket(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeleteTicket");
	protected virtual void DeletePersonalizedTicket(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeletePersonalizedTicket");
	protected virtual void DeleteAllCommonTicket() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeleteAllCommonTicket");
	protected virtual void DeleteAllPersonalizedTicket() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeleteAllPersonalizedTicket");
	protected virtual void DeleteAllPersonalizedTicketEx(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeleteAllPersonalizedTicketEx");
	protected virtual void GetTitleKey_0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetTitleKey_0 not implemented");
	protected virtual void CountCommonTicket() =>
		throw new NotImplementedException("Nn.Es.IETicketService.CountCommonTicket not implemented");
	protected virtual void CountPersonalizedTicket() =>
		throw new NotImplementedException("Nn.Es.IETicketService.CountPersonalizedTicket not implemented");
	protected virtual void ListCommonTicket() =>
		throw new NotImplementedException("Nn.Es.IETicketService.ListCommonTicket not implemented");
	protected virtual void ListPersonalizedTicket() =>
		throw new NotImplementedException("Nn.Es.IETicketService.ListPersonalizedTicket not implemented");
	protected virtual void ListMissingPersonalizedTicket(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.ListMissingPersonalizedTicket not implemented");
	protected virtual void GetCommonTicketSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetCommonTicketSize not implemented");
	protected virtual void GetPersonalizedTicketSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetPersonalizedTicketSize not implemented");
	protected virtual void GetCommonTicketData(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetCommonTicketData not implemented");
	protected virtual void GetPersonalizedTicketData(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetPersonalizedTicketData not implemented");
	protected virtual void OwnTicket(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.OwnTicket not implemented");
	protected virtual void GetTicketInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetTicketInfo not implemented");
	protected virtual void ListLightTicketInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.ListLightTicketInfo not implemented");
	protected virtual void SignData(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.SignData not implemented");
	protected virtual void GetCommonTicketAndCertificateSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetCommonTicketAndCertificateSize not implemented");
	protected virtual void GetCommonTicketAndCertificateData(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.GetCommonTicketAndCertificateData not implemented");
	protected virtual void ImportPrepurchaseRecord(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.ImportPrepurchaseRecord");
	protected virtual void DeletePrepurchaseRecord(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeletePrepurchaseRecord");
	protected virtual void DeleteAllPrepurchaseRecord() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.DeleteAllPrepurchaseRecord");
	protected virtual void CountPrepurchaseRecord() =>
		throw new NotImplementedException("Nn.Es.IETicketService.CountPrepurchaseRecord not implemented");
	protected virtual void ListPrepurchaseRecord() =>
		throw new NotImplementedException("Nn.Es.IETicketService.ListPrepurchaseRecord not implemented");
	protected virtual void ListPrepurchaseRecordInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Es.IETicketService.ListPrepurchaseRecordInfo not implemented");
	protected virtual void Unknown30() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown30");
	protected virtual void Unknown31() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown31");
	protected virtual void Unknown32() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown32");
	protected virtual void Unknown33() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown33");
	protected virtual void Unknown34() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown34");
	protected virtual void Unknown35() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown35");
	protected virtual void Unknown36() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown36");
	protected virtual void Unknown501() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown501");
	protected virtual void Unknown502() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown502");
	protected virtual void GetTitleKey_1() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.GetTitleKey_1");
	protected virtual void Unknown504() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown504");
	protected virtual void Unknown508() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown508");
	protected virtual void Unknown509() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown509");
	protected virtual void Unknown510() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown510");
	protected virtual void Unknown1001() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1001");
	protected virtual void Unknown1002() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1002");
	protected virtual void Unknown1003() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1003");
	protected virtual void Unknown1004() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1004");
	protected virtual void Unknown1005() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1005");
	protected virtual void Unknown1006() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1006");
	protected virtual void Unknown1007() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1007");
	protected virtual void Unknown1009() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1009");
	protected virtual void Unknown1010() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1010");
	protected virtual void Unknown1011() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1011");
	protected virtual void Unknown1012() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1012");
	protected virtual void Unknown1013() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1013");
	protected virtual void Unknown1014() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1014");
	protected virtual void Unknown1015() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1015");
	protected virtual void Unknown1016() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1016");
	protected virtual void Unknown1501() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1501");
	protected virtual void Unknown1502() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1502");
	protected virtual void Unknown1503() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1503");
	protected virtual void Unknown1504() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1504");
	protected virtual void Unknown1505() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown1505");
	protected virtual void Unknown2000() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown2000");
	protected virtual void Unknown2501() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown2501");
	protected virtual void Unknown2502() =>
		Console.WriteLine("Stub hit for Nn.Es.IETicketService.Unknown2502");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // ImportTicket
				break;
			case 0x2: // ImportTicketCertificateSet
				break;
			case 0x3: // DeleteTicket
				break;
			case 0x4: // DeletePersonalizedTicket
				break;
			case 0x5: // DeleteAllCommonTicket
				break;
			case 0x6: // DeleteAllPersonalizedTicket
				break;
			case 0x7: // DeleteAllPersonalizedTicketEx
				break;
			case 0x8: // GetTitleKey_0
				break;
			case 0x9: // CountCommonTicket
				break;
			case 0xA: // CountPersonalizedTicket
				break;
			case 0xB: // ListCommonTicket
				break;
			case 0xC: // ListPersonalizedTicket
				break;
			case 0xD: // ListMissingPersonalizedTicket
				break;
			case 0xE: // GetCommonTicketSize
				break;
			case 0xF: // GetPersonalizedTicketSize
				break;
			case 0x10: // GetCommonTicketData
				break;
			case 0x11: // GetPersonalizedTicketData
				break;
			case 0x12: // OwnTicket
				break;
			case 0x13: // GetTicketInfo
				break;
			case 0x14: // ListLightTicketInfo
				break;
			case 0x15: // SignData
				break;
			case 0x16: // GetCommonTicketAndCertificateSize
				break;
			case 0x17: // GetCommonTicketAndCertificateData
				break;
			case 0x18: // ImportPrepurchaseRecord
				break;
			case 0x19: // DeletePrepurchaseRecord
				break;
			case 0x1A: // DeleteAllPrepurchaseRecord
				break;
			case 0x1B: // CountPrepurchaseRecord
				break;
			case 0x1C: // ListPrepurchaseRecord
				break;
			case 0x1D: // ListPrepurchaseRecordInfo
				break;
			case 0x1E: // Unknown30
				break;
			case 0x1F: // Unknown31
				break;
			case 0x20: // Unknown32
				break;
			case 0x21: // Unknown33
				break;
			case 0x22: // Unknown34
				break;
			case 0x23: // Unknown35
				break;
			case 0x24: // Unknown36
				break;
			case 0x1F5: // Unknown501
				break;
			case 0x1F6: // Unknown502
				break;
			case 0x1F7: // GetTitleKey_1
				break;
			case 0x1F8: // Unknown504
				break;
			case 0x1FC: // Unknown508
				break;
			case 0x1FD: // Unknown509
				break;
			case 0x1FE: // Unknown510
				break;
			case 0x3E9: // Unknown1001
				break;
			case 0x3EA: // Unknown1002
				break;
			case 0x3EB: // Unknown1003
				break;
			case 0x3EC: // Unknown1004
				break;
			case 0x3ED: // Unknown1005
				break;
			case 0x3EE: // Unknown1006
				break;
			case 0x3EF: // Unknown1007
				break;
			case 0x3F1: // Unknown1009
				break;
			case 0x3F2: // Unknown1010
				break;
			case 0x3F3: // Unknown1011
				break;
			case 0x3F4: // Unknown1012
				break;
			case 0x3F5: // Unknown1013
				break;
			case 0x3F6: // Unknown1014
				break;
			case 0x3F7: // Unknown1015
				break;
			case 0x3F8: // Unknown1016
				break;
			case 0x5DD: // Unknown1501
				break;
			case 0x5DE: // Unknown1502
				break;
			case 0x5DF: // Unknown1503
				break;
			case 0x5E0: // Unknown1504
				break;
			case 0x5E1: // Unknown1505
				break;
			case 0x7D0: // Unknown2000
				break;
			case 0x9C5: // Unknown2501
				break;
			case 0x9C6: // Unknown2502
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Es.IETicketService");
		}
	}
}

