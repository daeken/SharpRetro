using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ncm;
public partial class IContentManager : _IContentManager_Base;
public abstract class _IContentManager_Base : IpcInterface {
	protected virtual void CreateContentStorage(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CreateContentStorage");
	protected virtual void CreateContentMetaDatabase(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CreateContentMetaDatabase");
	protected virtual void VerifyContentStorage(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.VerifyContentStorage");
	protected virtual void VerifyContentMetaDatabase(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.VerifyContentMetaDatabase");
	protected virtual Nn.Ncm.IContentStorage OpenContentStorage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentManager.OpenContentStorage not implemented");
	protected virtual Nn.Ncm.IContentMetaDatabase OpenContentMetaDatabase(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentManager.OpenContentMetaDatabase not implemented");
	protected virtual void CloseContentStorageForcibly(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentStorageForcibly");
	protected virtual void CloseContentMetaDatabaseForcibly(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentMetaDatabaseForcibly");
	protected virtual void CleanupContentMetaDatabase(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CleanupContentMetaDatabase");
	protected virtual void OpenContentStorage2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.OpenContentStorage2");
	protected virtual void CloseContentStorage(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentStorage");
	protected virtual void OpenContentMetaDatabase2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.OpenContentMetaDatabase2");
	protected virtual void CloseContentMetaDatabase(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentMetaDatabase");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateContentStorage
				break;
			}
			case 0x1: { // CreateContentMetaDatabase
				break;
			}
			case 0x2: { // VerifyContentStorage
				break;
			}
			case 0x3: { // VerifyContentMetaDatabase
				break;
			}
			case 0x4: { // OpenContentStorage
				break;
			}
			case 0x5: { // OpenContentMetaDatabase
				break;
			}
			case 0x6: { // CloseContentStorageForcibly
				break;
			}
			case 0x7: { // CloseContentMetaDatabaseForcibly
				break;
			}
			case 0x8: { // CleanupContentMetaDatabase
				break;
			}
			case 0x9: { // OpenContentStorage2
				break;
			}
			case 0xA: { // CloseContentStorage
				break;
			}
			case 0xB: { // OpenContentMetaDatabase2
				break;
			}
			case 0xC: { // CloseContentMetaDatabase
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentManager");
		}
	}
}

public partial class IContentMetaDatabase : _IContentMetaDatabase_Base;
public abstract class _IContentMetaDatabase_Base : IpcInterface {
	protected virtual void Set(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Set");
	protected virtual void Get(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.Get not implemented");
	protected virtual void Remove(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Remove");
	protected virtual void GetContentIdByType(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetContentIdByType not implemented");
	protected virtual void ListContentInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListContentInfo not implemented");
	protected virtual void List(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.List not implemented");
	protected virtual void GetLatestContentMetaKey(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetLatestContentMetaKey not implemented");
	protected virtual void ListApplication(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListApplication not implemented");
	protected virtual void Has(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.Has not implemented");
	protected virtual void HasAll(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.HasAll not implemented");
	protected virtual void GetSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetSize not implemented");
	protected virtual void GetRequiredSystemVersion(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetRequiredSystemVersion not implemented");
	protected virtual void GetPatchId(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetPatchId not implemented");
	protected virtual void DisableForcibly() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.DisableForcibly");
	protected virtual void LookupOrphanContent(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.LookupOrphanContent not implemented");
	protected virtual void Commit() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Commit");
	protected virtual void HasContent(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.HasContent not implemented");
	protected virtual void ListContentMetaInfo(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListContentMetaInfo not implemented");
	protected virtual void GetAttributes(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetAttributes not implemented");
	protected virtual void GetRequiredApplicationVersion(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetRequiredApplicationVersion not implemented");
	protected virtual void Unknown20() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Unknown20");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Set
				break;
			}
			case 0x1: { // Get
				break;
			}
			case 0x2: { // Remove
				break;
			}
			case 0x3: { // GetContentIdByType
				break;
			}
			case 0x4: { // ListContentInfo
				break;
			}
			case 0x5: { // List
				break;
			}
			case 0x6: { // GetLatestContentMetaKey
				break;
			}
			case 0x7: { // ListApplication
				break;
			}
			case 0x8: { // Has
				break;
			}
			case 0x9: { // HasAll
				break;
			}
			case 0xA: { // GetSize
				break;
			}
			case 0xB: { // GetRequiredSystemVersion
				break;
			}
			case 0xC: { // GetPatchId
				break;
			}
			case 0xD: { // DisableForcibly
				break;
			}
			case 0xE: { // LookupOrphanContent
				break;
			}
			case 0xF: { // Commit
				break;
			}
			case 0x10: { // HasContent
				break;
			}
			case 0x11: { // ListContentMetaInfo
				break;
			}
			case 0x12: { // GetAttributes
				break;
			}
			case 0x13: { // GetRequiredApplicationVersion
				break;
			}
			case 0x14: { // Unknown20
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentMetaDatabase");
		}
	}
}

public partial class IContentStorage : _IContentStorage_Base;
public abstract class _IContentStorage_Base : IpcInterface {
	protected virtual void GeneratePlaceHolderId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GeneratePlaceHolderId not implemented");
	protected virtual void CreatePlaceHolder(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.CreatePlaceHolder");
	protected virtual void DeletePlaceHolder(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.DeletePlaceHolder");
	protected virtual void HasPlaceHolder(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.HasPlaceHolder not implemented");
	protected virtual void WritePlaceHolder(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.WritePlaceHolder");
	protected virtual void Register(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.Register");
	protected virtual void Delete(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.Delete");
	protected virtual void Has(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.Has not implemented");
	protected virtual void GetPath(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetPath not implemented");
	protected virtual void GetPlaceHolderPath(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetPlaceHolderPath not implemented");
	protected virtual void CleanupAllPlaceHolder() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.CleanupAllPlaceHolder");
	protected virtual void ListPlaceHolder(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ListPlaceHolder not implemented");
	protected virtual void GetContentCount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetContentCount not implemented");
	protected virtual void ListContentId(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ListContentId not implemented");
	protected virtual void GetSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetSize not implemented");
	protected virtual void DisableForcibly() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.DisableForcibly");
	protected virtual void RevertToPlaceHolder(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.RevertToPlaceHolder");
	protected virtual void SetPlaceHolderSize(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.SetPlaceHolderSize");
	protected virtual void ReadContentIdFile(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ReadContentIdFile not implemented");
	protected virtual void GetRightsIdFromPlaceHolderId(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetRightsIdFromPlaceHolderId not implemented");
	protected virtual void GetRightsIdFromContentId(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetRightsIdFromContentId not implemented");
	protected virtual void WriteContentForDebug(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.WriteContentForDebug");
	protected virtual void GetFreeSpaceSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetFreeSpaceSize not implemented");
	protected virtual void GetTotalSpaceSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetTotalSpaceSize not implemented");
	protected virtual void FlushStorage() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.FlushStorage");
	protected virtual void Unknown25(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.Unknown25 not implemented");
	protected virtual void Unknown26() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.Unknown26");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GeneratePlaceHolderId
				break;
			}
			case 0x1: { // CreatePlaceHolder
				break;
			}
			case 0x2: { // DeletePlaceHolder
				break;
			}
			case 0x3: { // HasPlaceHolder
				break;
			}
			case 0x4: { // WritePlaceHolder
				break;
			}
			case 0x5: { // Register
				break;
			}
			case 0x6: { // Delete
				break;
			}
			case 0x7: { // Has
				break;
			}
			case 0x8: { // GetPath
				break;
			}
			case 0x9: { // GetPlaceHolderPath
				break;
			}
			case 0xA: { // CleanupAllPlaceHolder
				break;
			}
			case 0xB: { // ListPlaceHolder
				break;
			}
			case 0xC: { // GetContentCount
				break;
			}
			case 0xD: { // ListContentId
				break;
			}
			case 0xE: { // GetSize
				break;
			}
			case 0xF: { // DisableForcibly
				break;
			}
			case 0x10: { // RevertToPlaceHolder
				break;
			}
			case 0x11: { // SetPlaceHolderSize
				break;
			}
			case 0x12: { // ReadContentIdFile
				break;
			}
			case 0x13: { // GetRightsIdFromPlaceHolderId
				break;
			}
			case 0x14: { // GetRightsIdFromContentId
				break;
			}
			case 0x15: { // WriteContentForDebug
				break;
			}
			case 0x16: { // GetFreeSpaceSize
				break;
			}
			case 0x17: { // GetTotalSpaceSize
				break;
			}
			case 0x18: { // FlushStorage
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			case 0x1A: { // Unknown26
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentStorage");
		}
	}
}

