using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Es;
public partial class IETicketService : _IETicketService_Base;
public abstract class _IETicketService_Base : IpcInterface {
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
			case 0x8: // GetTitleKey
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
			case 0x1F7: // GetTitleKey
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

