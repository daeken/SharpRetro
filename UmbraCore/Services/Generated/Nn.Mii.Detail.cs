using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Mii.Detail;
public partial class IDatabaseService : _IDatabaseService_Base;
public abstract class _IDatabaseService_Base : IpcInterface {
	protected virtual byte IsUpdated(uint _0) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.IsUpdated not implemented");
	protected virtual byte IsFullDatabase() =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.IsFullDatabase not implemented");
	protected virtual uint GetCount(uint _0) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.GetCount not implemented");
	protected virtual void Get(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Get not implemented");
	protected virtual void Get1(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Get1 not implemented");
	protected virtual void UpdateLatest(Span<byte> _0, uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.UpdateLatest not implemented");
	protected virtual void BuildRandom(uint _0, uint _1, uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.BuildRandom not implemented");
	protected virtual void BuildDefault(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.BuildDefault not implemented");
	protected virtual void Get2(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Get2 not implemented");
	protected virtual void Get3(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Get3 not implemented");
	protected virtual void UpdateLatest1(Span<byte> _0, uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.UpdateLatest1 not implemented");
	protected virtual uint FindIndex(Span<byte> _0, byte _1) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.FindIndex not implemented");
	protected virtual void Move(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Move");
	protected virtual void AddOrReplace(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.AddOrReplace");
	protected virtual void Delete(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Delete");
	protected virtual void DestroyFile() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.DestroyFile");
	protected virtual void DeleteFile() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.DeleteFile");
	protected virtual void Format() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Format");
	protected virtual void Import(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Import");
	protected virtual void Export(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Export not implemented");
	protected virtual byte IsBrokenDatabaseWithClearFlag() =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.IsBrokenDatabaseWithClearFlag not implemented");
	protected virtual uint GetIndex(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.GetIndex not implemented");
	protected virtual void SetInterfaceVersion() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.SetInterfaceVersion");
	protected virtual void Convert() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Convert");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // IsUpdated
				break;
			}
			case 0x1: { // IsFullDatabase
				break;
			}
			case 0x2: { // GetCount
				break;
			}
			case 0x3: { // Get
				break;
			}
			case 0x4: { // Get1
				break;
			}
			case 0x5: { // UpdateLatest
				break;
			}
			case 0x6: { // BuildRandom
				break;
			}
			case 0x7: { // BuildDefault
				break;
			}
			case 0x8: { // Get2
				break;
			}
			case 0x9: { // Get3
				break;
			}
			case 0xA: { // UpdateLatest1
				break;
			}
			case 0xB: { // FindIndex
				break;
			}
			case 0xC: { // Move
				break;
			}
			case 0xD: { // AddOrReplace
				break;
			}
			case 0xE: { // Delete
				break;
			}
			case 0xF: { // DestroyFile
				break;
			}
			case 0x10: { // DeleteFile
				break;
			}
			case 0x11: { // Format
				break;
			}
			case 0x12: { // Import
				break;
			}
			case 0x13: { // Export
				break;
			}
			case 0x14: { // IsBrokenDatabaseWithClearFlag
				break;
			}
			case 0x15: { // GetIndex
				break;
			}
			case 0x16: { // SetInterfaceVersion
				break;
			}
			case 0x17: { // Convert
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IDatabaseService");
		}
	}
}

public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected virtual Nn.Mii.Detail.IDatabaseService GetDatabaseService(uint _0) =>
		throw new NotImplementedException("Nn.Mii.Detail.IStaticService.GetDatabaseService not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDatabaseService
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IStaticService");
		}
	}
}

public partial class IImageDatabaseService : _IImageDatabaseService_Base;
public abstract class _IImageDatabaseService_Base : IpcInterface {
	protected virtual void Initialize() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.Initialize");
	protected virtual void Reload() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.Reload");
	protected virtual void GetCount() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.GetCount");
	protected virtual void IsEmpty() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.IsEmpty");
	protected virtual void IsFull() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.IsFull");
	protected virtual void GetAttribute() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.GetAttribute");
	protected virtual void LoadImage() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.LoadImage");
	protected virtual void AddOrUpdateImage() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.AddOrUpdateImage");
	protected virtual void DeleteImages() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.DeleteImages");
	protected virtual void DeleteFile() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.DeleteFile");
	protected virtual void DestroyFile() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.DestroyFile");
	protected virtual void ImportFile() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.ImportFile");
	protected virtual void ExportFile() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.ExportFile");
	protected virtual void ForceInitialize() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IImageDatabaseService.ForceInitialize");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				break;
			}
			case 0xA: { // Reload
				break;
			}
			case 0xB: { // GetCount
				break;
			}
			case 0xC: { // IsEmpty
				break;
			}
			case 0xD: { // IsFull
				break;
			}
			case 0xE: { // GetAttribute
				break;
			}
			case 0xF: { // LoadImage
				break;
			}
			case 0x10: { // AddOrUpdateImage
				break;
			}
			case 0x11: { // DeleteImages
				break;
			}
			case 0x64: { // DeleteFile
				break;
			}
			case 0x65: { // DestroyFile
				break;
			}
			case 0x66: { // ImportFile
				break;
			}
			case 0x67: { // ExportFile
				break;
			}
			case 0x68: { // ForceInitialize
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IImageDatabaseService");
		}
	}
}

