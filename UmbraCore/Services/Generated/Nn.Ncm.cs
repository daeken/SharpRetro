using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ncm;
public partial class IContentManager : _IContentManager_Base {
	public readonly string ServiceName;
	public IContentManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IContentManager_Base : IpcInterface {
	protected virtual void CreateContentStorage(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CreateContentStorage".Log();
	protected virtual void CreateContentMetaDatabase(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CreateContentMetaDatabase".Log();
	protected virtual void VerifyContentStorage(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.VerifyContentStorage".Log();
	protected virtual void VerifyContentMetaDatabase(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.VerifyContentMetaDatabase".Log();
	protected virtual Nn.Ncm.IContentStorage OpenContentStorage(byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentManager.OpenContentStorage not implemented");
	protected virtual Nn.Ncm.IContentMetaDatabase OpenContentMetaDatabase(byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentManager.OpenContentMetaDatabase not implemented");
	protected virtual void CloseContentStorageForcibly(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CloseContentStorageForcibly".Log();
	protected virtual void CloseContentMetaDatabaseForcibly(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CloseContentMetaDatabaseForcibly".Log();
	protected virtual void CleanupContentMetaDatabase(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CleanupContentMetaDatabase".Log();
	protected virtual void OpenContentStorage2(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.OpenContentStorage2".Log();
	protected virtual void CloseContentStorage(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CloseContentStorage".Log();
	protected virtual void OpenContentMetaDatabase2(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.OpenContentMetaDatabase2".Log();
	protected virtual void CloseContentMetaDatabase(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentManager.CloseContentMetaDatabase".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateContentStorage
				CreateContentStorage(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // CreateContentMetaDatabase
				CreateContentMetaDatabase(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // VerifyContentStorage
				VerifyContentStorage(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // VerifyContentMetaDatabase
				VerifyContentMetaDatabase(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // OpenContentStorage
				var _return = OpenContentStorage(im.GetBytes(8, 0x1));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // OpenContentMetaDatabase
				var _return = OpenContentMetaDatabase(im.GetBytes(8, 0x1));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // CloseContentStorageForcibly
				CloseContentStorageForcibly(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // CloseContentMetaDatabaseForcibly
				CloseContentMetaDatabaseForcibly(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // CleanupContentMetaDatabase
				CleanupContentMetaDatabase(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // OpenContentStorage2
				OpenContentStorage2(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // CloseContentStorage
				CloseContentStorage(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // OpenContentMetaDatabase2
				OpenContentMetaDatabase2(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // CloseContentMetaDatabase
				CloseContentMetaDatabase(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentManager");
		}
	}
}

public partial class IContentMetaDatabase : _IContentMetaDatabase_Base;
public abstract class _IContentMetaDatabase_Base : IpcInterface {
	protected virtual void Set(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Ncm.IContentMetaDatabase.Set".Log();
	protected virtual void Get(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.Get not implemented");
	protected virtual void Remove(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentMetaDatabase.Remove".Log();
	protected virtual void GetContentIdByType(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetContentIdByType not implemented");
	protected virtual void ListContentInfo(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListContentInfo not implemented");
	protected virtual void List(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.List not implemented");
	protected virtual void GetLatestContentMetaKey(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetLatestContentMetaKey not implemented");
	protected virtual void ListApplication(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListApplication not implemented");
	protected virtual void Has(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.Has not implemented");
	protected virtual void HasAll(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.HasAll not implemented");
	protected virtual void GetSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetSize not implemented");
	protected virtual void GetRequiredSystemVersion(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetRequiredSystemVersion not implemented");
	protected virtual void GetPatchId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetPatchId not implemented");
	protected virtual void DisableForcibly() =>
		"Stub hit for Nn.Ncm.IContentMetaDatabase.DisableForcibly".Log();
	protected virtual void LookupOrphanContent(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.LookupOrphanContent not implemented");
	protected virtual void Commit() =>
		"Stub hit for Nn.Ncm.IContentMetaDatabase.Commit".Log();
	protected virtual void HasContent(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.HasContent not implemented");
	protected virtual void ListContentMetaInfo(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListContentMetaInfo not implemented");
	protected virtual void GetAttributes(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetAttributes not implemented");
	protected virtual void GetRequiredApplicationVersion(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetRequiredApplicationVersion not implemented");
	protected virtual void Unknown20() =>
		"Stub hit for Nn.Ncm.IContentMetaDatabase.Unknown20".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Set
				Set(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Get
				Get(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Remove
				Remove(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetContentIdByType
				GetContentIdByType(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // ListContentInfo
				ListContentInfo(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // List
				List(im.GetBytes(8, 0x20), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // GetLatestContentMetaKey
				GetLatestContentMetaKey(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // ListApplication
				ListApplication(im.GetBytes(8, 0x1), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Has
				Has(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // HasAll
				HasAll(im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // GetSize
				GetSize(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // GetRequiredSystemVersion
				GetRequiredSystemVersion(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetPatchId
				GetPatchId(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // DisableForcibly
				DisableForcibly();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // LookupOrphanContent
				LookupOrphanContent(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // Commit
				Commit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // HasContent
				HasContent(im.GetBytes(8, 0x20), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x11: { // ListContentMetaInfo
				ListContentMetaInfo(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12: { // GetAttributes
				GetAttributes(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // GetRequiredApplicationVersion
				GetRequiredApplicationVersion(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // Unknown20
				Unknown20();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentMetaDatabase");
		}
	}
}

public partial class IContentStorage : _IContentStorage_Base;
public abstract class _IContentStorage_Base : IpcInterface {
	protected virtual void GeneratePlaceHolderId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GeneratePlaceHolderId not implemented");
	protected virtual void CreatePlaceHolder(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentStorage.CreatePlaceHolder".Log();
	protected virtual void DeletePlaceHolder(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentStorage.DeletePlaceHolder".Log();
	protected virtual void HasPlaceHolder(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.HasPlaceHolder not implemented");
	protected virtual void WritePlaceHolder(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Ncm.IContentStorage.WritePlaceHolder".Log();
	protected virtual void Register(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentStorage.Register".Log();
	protected virtual void Delete(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentStorage.Delete".Log();
	protected virtual void Has(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.Has not implemented");
	protected virtual void GetPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetPath not implemented");
	protected virtual void GetPlaceHolderPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetPlaceHolderPath not implemented");
	protected virtual void CleanupAllPlaceHolder() =>
		"Stub hit for Nn.Ncm.IContentStorage.CleanupAllPlaceHolder".Log();
	protected virtual void ListPlaceHolder(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ListPlaceHolder not implemented");
	protected virtual void GetContentCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetContentCount not implemented");
	protected virtual void ListContentId(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ListContentId not implemented");
	protected virtual void GetSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetSize not implemented");
	protected virtual void DisableForcibly() =>
		"Stub hit for Nn.Ncm.IContentStorage.DisableForcibly".Log();
	protected virtual void RevertToPlaceHolder(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentStorage.RevertToPlaceHolder".Log();
	protected virtual void SetPlaceHolderSize(byte[] _0) =>
		"Stub hit for Nn.Ncm.IContentStorage.SetPlaceHolderSize".Log();
	protected virtual void ReadContentIdFile(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ReadContentIdFile not implemented");
	protected virtual void GetRightsIdFromPlaceHolderId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetRightsIdFromPlaceHolderId not implemented");
	protected virtual void GetRightsIdFromContentId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetRightsIdFromContentId not implemented");
	protected virtual void WriteContentForDebug(byte[] _0, Span<byte> _1) =>
		"Stub hit for Nn.Ncm.IContentStorage.WriteContentForDebug".Log();
	protected virtual void GetFreeSpaceSize(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetFreeSpaceSize not implemented");
	protected virtual void GetTotalSpaceSize(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetTotalSpaceSize not implemented");
	protected virtual void FlushStorage() =>
		"Stub hit for Nn.Ncm.IContentStorage.FlushStorage".Log();
	protected virtual void Unknown25(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.Unknown25 not implemented");
	protected virtual void Unknown26() =>
		"Stub hit for Nn.Ncm.IContentStorage.Unknown26".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GeneratePlaceHolderId
				GeneratePlaceHolderId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // CreatePlaceHolder
				CreatePlaceHolder(im.GetBytes(8, 0x28));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // DeletePlaceHolder
				DeletePlaceHolder(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // HasPlaceHolder
				HasPlaceHolder(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // WritePlaceHolder
				WritePlaceHolder(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Register
				Register(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Delete
				Delete(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Has
				Has(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // GetPath
				GetPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetPlaceHolderPath
				GetPlaceHolderPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // CleanupAllPlaceHolder
				CleanupAllPlaceHolder();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // ListPlaceHolder
				ListPlaceHolder(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetContentCount
				GetContentCount(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // ListContentId
				ListContentId(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // GetSize
				GetSize(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // DisableForcibly
				DisableForcibly();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // RevertToPlaceHolder
				RevertToPlaceHolder(im.GetBytes(8, 0x30));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // SetPlaceHolderSize
				SetPlaceHolderSize(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // ReadContentIdFile
				ReadContentIdFile(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // GetRightsIdFromPlaceHolderId
				GetRightsIdFromPlaceHolderId(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // GetRightsIdFromContentId
				GetRightsIdFromContentId(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // WriteContentForDebug
				WriteContentForDebug(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // GetFreeSpaceSize
				GetFreeSpaceSize(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // GetTotalSpaceSize
				GetTotalSpaceSize(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // FlushStorage
				FlushStorage();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // Unknown26
				Unknown26();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentStorage");
		}
	}
}

