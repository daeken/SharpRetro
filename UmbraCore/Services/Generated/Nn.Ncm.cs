using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ncm;
public partial class IContentManager : _IContentManager_Base;
public abstract class _IContentManager_Base : IpcInterface {
	protected virtual void CreateContentStorage(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CreateContentStorage");
	protected virtual void CreateContentMetaDatabase(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CreateContentMetaDatabase");
	protected virtual void VerifyContentStorage(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.VerifyContentStorage");
	protected virtual void VerifyContentMetaDatabase(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.VerifyContentMetaDatabase");
	protected virtual Nn.Ncm.IContentStorage OpenContentStorage(byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentManager.OpenContentStorage not implemented");
	protected virtual Nn.Ncm.IContentMetaDatabase OpenContentMetaDatabase(byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentManager.OpenContentMetaDatabase not implemented");
	protected virtual void CloseContentStorageForcibly(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentStorageForcibly");
	protected virtual void CloseContentMetaDatabaseForcibly(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentMetaDatabaseForcibly");
	protected virtual void CleanupContentMetaDatabase(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CleanupContentMetaDatabase");
	protected virtual void OpenContentStorage2(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.OpenContentStorage2");
	protected virtual void CloseContentStorage(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentStorage");
	protected virtual void OpenContentMetaDatabase2(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.OpenContentMetaDatabase2");
	protected virtual void CloseContentMetaDatabase(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentManager.CloseContentMetaDatabase");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateContentStorage
				om.Initialize(0, 0, 0);
				CreateContentStorage(im.GetBytes(8, 0x1));
				break;
			}
			case 0x1: { // CreateContentMetaDatabase
				om.Initialize(0, 0, 0);
				CreateContentMetaDatabase(im.GetBytes(8, 0x1));
				break;
			}
			case 0x2: { // VerifyContentStorage
				om.Initialize(0, 0, 0);
				VerifyContentStorage(im.GetBytes(8, 0x1));
				break;
			}
			case 0x3: { // VerifyContentMetaDatabase
				om.Initialize(0, 0, 0);
				VerifyContentMetaDatabase(im.GetBytes(8, 0x1));
				break;
			}
			case 0x4: { // OpenContentStorage
				om.Initialize(1, 0, 0);
				var _return = OpenContentStorage(im.GetBytes(8, 0x1));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // OpenContentMetaDatabase
				om.Initialize(1, 0, 0);
				var _return = OpenContentMetaDatabase(im.GetBytes(8, 0x1));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x6: { // CloseContentStorageForcibly
				om.Initialize(0, 0, 0);
				CloseContentStorageForcibly(im.GetBytes(8, 0x1));
				break;
			}
			case 0x7: { // CloseContentMetaDatabaseForcibly
				om.Initialize(0, 0, 0);
				CloseContentMetaDatabaseForcibly(im.GetBytes(8, 0x1));
				break;
			}
			case 0x8: { // CleanupContentMetaDatabase
				om.Initialize(0, 0, 0);
				CleanupContentMetaDatabase(im.GetBytes(8, 0x1));
				break;
			}
			case 0x9: { // OpenContentStorage2
				om.Initialize(0, 0, 0);
				OpenContentStorage2(im.GetBytes(8, 0x1));
				break;
			}
			case 0xA: { // CloseContentStorage
				om.Initialize(0, 0, 0);
				CloseContentStorage(im.GetBytes(8, 0x1));
				break;
			}
			case 0xB: { // OpenContentMetaDatabase2
				om.Initialize(0, 0, 0);
				OpenContentMetaDatabase2(im.GetBytes(8, 0x1));
				break;
			}
			case 0xC: { // CloseContentMetaDatabase
				om.Initialize(0, 0, 0);
				CloseContentMetaDatabase(im.GetBytes(8, 0x1));
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
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Set");
	protected virtual void Get(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.Get not implemented");
	protected virtual void Remove(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Remove");
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
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.DisableForcibly");
	protected virtual void LookupOrphanContent(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.LookupOrphanContent not implemented");
	protected virtual void Commit() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Commit");
	protected virtual void HasContent(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.HasContent not implemented");
	protected virtual void ListContentMetaInfo(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.ListContentMetaInfo not implemented");
	protected virtual void GetAttributes(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetAttributes not implemented");
	protected virtual void GetRequiredApplicationVersion(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentMetaDatabase.GetRequiredApplicationVersion not implemented");
	protected virtual void Unknown20() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentMetaDatabase.Unknown20");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Set
				om.Initialize(0, 0, 0);
				Set(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x1: { // Get
				om.Initialize(0, 0, 8);
				Get(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Remove
				om.Initialize(0, 0, 0);
				Remove(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3: { // GetContentIdByType
				om.Initialize(0, 0, 16);
				GetContentIdByType(im.GetBytes(8, 0x18), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // ListContentInfo
				om.Initialize(0, 0, 4);
				ListContentInfo(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // List
				om.Initialize(0, 0, 8);
				List(im.GetBytes(8, 0x20), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // GetLatestContentMetaKey
				om.Initialize(0, 0, 16);
				GetLatestContentMetaKey(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // ListApplication
				om.Initialize(0, 0, 8);
				ListApplication(im.GetBytes(8, 0x1), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Has
				om.Initialize(0, 0, 1);
				Has(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // HasAll
				om.Initialize(0, 0, 1);
				HasAll(im.GetSpan<byte>(0x5, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // GetSize
				om.Initialize(0, 0, 8);
				GetSize(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // GetRequiredSystemVersion
				om.Initialize(0, 0, 4);
				GetRequiredSystemVersion(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetPatchId
				om.Initialize(0, 0, 8);
				GetPatchId(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // DisableForcibly
				om.Initialize(0, 0, 0);
				DisableForcibly();
				break;
			}
			case 0xE: { // LookupOrphanContent
				om.Initialize(0, 0, 0);
				LookupOrphanContent(im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xF: { // Commit
				om.Initialize(0, 0, 0);
				Commit();
				break;
			}
			case 0x10: { // HasContent
				om.Initialize(0, 0, 1);
				HasContent(im.GetBytes(8, 0x20), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x11: { // ListContentMetaInfo
				om.Initialize(0, 0, 4);
				ListContentMetaInfo(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x12: { // GetAttributes
				om.Initialize(0, 0, 1);
				GetAttributes(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // GetRequiredApplicationVersion
				om.Initialize(0, 0, 4);
				GetRequiredApplicationVersion(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // Unknown20
				om.Initialize(0, 0, 0);
				Unknown20();
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
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.CreatePlaceHolder");
	protected virtual void DeletePlaceHolder(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.DeletePlaceHolder");
	protected virtual void HasPlaceHolder(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.HasPlaceHolder not implemented");
	protected virtual void WritePlaceHolder(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.WritePlaceHolder");
	protected virtual void Register(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.Register");
	protected virtual void Delete(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.Delete");
	protected virtual void Has(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.Has not implemented");
	protected virtual void GetPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetPath not implemented");
	protected virtual void GetPlaceHolderPath(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetPlaceHolderPath not implemented");
	protected virtual void CleanupAllPlaceHolder() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.CleanupAllPlaceHolder");
	protected virtual void ListPlaceHolder(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ListPlaceHolder not implemented");
	protected virtual void GetContentCount(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetContentCount not implemented");
	protected virtual void ListContentId(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ListContentId not implemented");
	protected virtual void GetSize(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetSize not implemented");
	protected virtual void DisableForcibly() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.DisableForcibly");
	protected virtual void RevertToPlaceHolder(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.RevertToPlaceHolder");
	protected virtual void SetPlaceHolderSize(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.SetPlaceHolderSize");
	protected virtual void ReadContentIdFile(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.ReadContentIdFile not implemented");
	protected virtual void GetRightsIdFromPlaceHolderId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetRightsIdFromPlaceHolderId not implemented");
	protected virtual void GetRightsIdFromContentId(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetRightsIdFromContentId not implemented");
	protected virtual void WriteContentForDebug(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.WriteContentForDebug");
	protected virtual void GetFreeSpaceSize(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetFreeSpaceSize not implemented");
	protected virtual void GetTotalSpaceSize(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.GetTotalSpaceSize not implemented");
	protected virtual void FlushStorage() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.FlushStorage");
	protected virtual void Unknown25(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ncm.IContentStorage.Unknown25 not implemented");
	protected virtual void Unknown26() =>
		Console.WriteLine("Stub hit for Nn.Ncm.IContentStorage.Unknown26");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GeneratePlaceHolderId
				om.Initialize(0, 0, 16);
				GeneratePlaceHolderId(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // CreatePlaceHolder
				om.Initialize(0, 0, 0);
				CreatePlaceHolder(im.GetBytes(8, 0x28));
				break;
			}
			case 0x2: { // DeletePlaceHolder
				om.Initialize(0, 0, 0);
				DeletePlaceHolder(im.GetBytes(8, 0x10));
				break;
			}
			case 0x3: { // HasPlaceHolder
				om.Initialize(0, 0, 1);
				HasPlaceHolder(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // WritePlaceHolder
				om.Initialize(0, 0, 0);
				WritePlaceHolder(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x5: { // Register
				om.Initialize(0, 0, 0);
				Register(im.GetBytes(8, 0x20));
				break;
			}
			case 0x6: { // Delete
				om.Initialize(0, 0, 0);
				Delete(im.GetBytes(8, 0x10));
				break;
			}
			case 0x7: { // Has
				om.Initialize(0, 0, 1);
				Has(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // GetPath
				om.Initialize(0, 0, 0);
				GetPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x9: { // GetPlaceHolderPath
				om.Initialize(0, 0, 0);
				GetPlaceHolderPath(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0xA: { // CleanupAllPlaceHolder
				om.Initialize(0, 0, 0);
				CleanupAllPlaceHolder();
				break;
			}
			case 0xB: { // ListPlaceHolder
				om.Initialize(0, 0, 4);
				ListPlaceHolder(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetContentCount
				om.Initialize(0, 0, 4);
				GetContentCount(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // ListContentId
				om.Initialize(0, 0, 4);
				ListContentId(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // GetSize
				om.Initialize(0, 0, 8);
				GetSize(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // DisableForcibly
				om.Initialize(0, 0, 0);
				DisableForcibly();
				break;
			}
			case 0x10: { // RevertToPlaceHolder
				om.Initialize(0, 0, 0);
				RevertToPlaceHolder(im.GetBytes(8, 0x30));
				break;
			}
			case 0x11: { // SetPlaceHolderSize
				om.Initialize(0, 0, 0);
				SetPlaceHolderSize(im.GetBytes(8, 0x18));
				break;
			}
			case 0x12: { // ReadContentIdFile
				om.Initialize(0, 0, 0);
				ReadContentIdFile(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x13: { // GetRightsIdFromPlaceHolderId
				om.Initialize(0, 0, 24);
				GetRightsIdFromPlaceHolderId(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // GetRightsIdFromContentId
				om.Initialize(0, 0, 24);
				GetRightsIdFromContentId(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // WriteContentForDebug
				om.Initialize(0, 0, 0);
				WriteContentForDebug(im.GetBytes(8, 0x18), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x16: { // GetFreeSpaceSize
				om.Initialize(0, 0, 8);
				GetFreeSpaceSize(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // GetTotalSpaceSize
				om.Initialize(0, 0, 8);
				GetTotalSpaceSize(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // FlushStorage
				om.Initialize(0, 0, 0);
				FlushStorage();
				break;
			}
			case 0x19: { // Unknown25
				om.Initialize(0, 0, 8);
				Unknown25(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // Unknown26
				om.Initialize(0, 0, 0);
				Unknown26();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ncm.IContentStorage");
		}
	}
}

