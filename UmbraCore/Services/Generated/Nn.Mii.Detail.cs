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
	protected virtual void UpdateLatest(byte[] _0, uint _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.UpdateLatest not implemented");
	protected virtual void BuildRandom(uint _0, uint _1, uint _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.BuildRandom not implemented");
	protected virtual void BuildDefault(uint _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.BuildDefault not implemented");
	protected virtual void Get2(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Get2 not implemented");
	protected virtual void Get3(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.Get3 not implemented");
	protected virtual void UpdateLatest1(byte[] _0, uint _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.UpdateLatest1 not implemented");
	protected virtual uint FindIndex(byte[] _0, byte _1) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.FindIndex not implemented");
	protected virtual void Move(byte[] _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Move");
	protected virtual void AddOrReplace(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.AddOrReplace");
	protected virtual void Delete(byte[] _0) =>
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
	protected virtual uint GetIndex(byte[] _0) =>
		throw new NotImplementedException("Nn.Mii.Detail.IDatabaseService.GetIndex not implemented");
	protected virtual void SetInterfaceVersion() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.SetInterfaceVersion");
	protected virtual void Convert() =>
		Console.WriteLine("Stub hit for Nn.Mii.Detail.IDatabaseService.Convert");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // IsUpdated
				om.Initialize(0, 0, 1);
				var _return = IsUpdated(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // IsFullDatabase
				om.Initialize(0, 0, 1);
				var _return = IsFullDatabase();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetCount
				om.Initialize(0, 0, 4);
				var _return = GetCount(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // Get
				om.Initialize(0, 0, 4);
				Get(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x4: { // Get1
				om.Initialize(0, 0, 4);
				Get1(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x5: { // UpdateLatest
				om.Initialize(0, 0, 88);
				UpdateLatest(im.GetBytes(8, 0x58), im.GetData<uint>(96), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // BuildRandom
				om.Initialize(0, 0, 88);
				BuildRandom(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // BuildDefault
				om.Initialize(0, 0, 88);
				BuildDefault(im.GetData<uint>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Get2
				om.Initialize(0, 0, 4);
				Get2(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x9: { // Get3
				om.Initialize(0, 0, 4);
				Get3(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xA: { // UpdateLatest1
				om.Initialize(0, 0, 68);
				UpdateLatest1(im.GetBytes(8, 0x44), im.GetData<uint>(76), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // FindIndex
				om.Initialize(0, 0, 4);
				var _return = FindIndex(im.GetBytes(8, 0x10), im.GetData<byte>(24));
				om.SetData(8, _return);
				break;
			}
			case 0xC: { // Move
				om.Initialize(0, 0, 0);
				Move(im.GetBytes(8, 0x10), im.GetData<uint>(24));
				break;
			}
			case 0xD: { // AddOrReplace
				om.Initialize(0, 0, 0);
				AddOrReplace(im.GetBytes(8, 0x44));
				break;
			}
			case 0xE: { // Delete
				om.Initialize(0, 0, 0);
				Delete(im.GetBytes(8, 0x10));
				break;
			}
			case 0xF: { // DestroyFile
				om.Initialize(0, 0, 0);
				DestroyFile();
				break;
			}
			case 0x10: { // DeleteFile
				om.Initialize(0, 0, 0);
				DeleteFile();
				break;
			}
			case 0x11: { // Format
				om.Initialize(0, 0, 0);
				Format();
				break;
			}
			case 0x12: { // Import
				om.Initialize(0, 0, 0);
				Import(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x13: { // Export
				om.Initialize(0, 0, 0);
				Export(im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x14: { // IsBrokenDatabaseWithClearFlag
				om.Initialize(0, 0, 1);
				var _return = IsBrokenDatabaseWithClearFlag();
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetIndex
				om.Initialize(0, 0, 4);
				var _return = GetIndex(im.GetBytes(8, 0x58));
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // SetInterfaceVersion
				om.Initialize(0, 0, 0);
				SetInterfaceVersion();
				break;
			}
			case 0x17: { // Convert
				om.Initialize(0, 0, 0);
				Convert();
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDatabaseService
				om.Initialize(1, 0, 0);
				var _return = GetDatabaseService(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				om.Initialize(0, 0, 0);
				Initialize();
				break;
			}
			case 0xA: { // Reload
				om.Initialize(0, 0, 0);
				Reload();
				break;
			}
			case 0xB: { // GetCount
				om.Initialize(0, 0, 0);
				GetCount();
				break;
			}
			case 0xC: { // IsEmpty
				om.Initialize(0, 0, 0);
				IsEmpty();
				break;
			}
			case 0xD: { // IsFull
				om.Initialize(0, 0, 0);
				IsFull();
				break;
			}
			case 0xE: { // GetAttribute
				om.Initialize(0, 0, 0);
				GetAttribute();
				break;
			}
			case 0xF: { // LoadImage
				om.Initialize(0, 0, 0);
				LoadImage();
				break;
			}
			case 0x10: { // AddOrUpdateImage
				om.Initialize(0, 0, 0);
				AddOrUpdateImage();
				break;
			}
			case 0x11: { // DeleteImages
				om.Initialize(0, 0, 0);
				DeleteImages();
				break;
			}
			case 0x64: { // DeleteFile
				om.Initialize(0, 0, 0);
				DeleteFile();
				break;
			}
			case 0x65: { // DestroyFile
				om.Initialize(0, 0, 0);
				DestroyFile();
				break;
			}
			case 0x66: { // ImportFile
				om.Initialize(0, 0, 0);
				ImportFile();
				break;
			}
			case 0x67: { // ExportFile
				om.Initialize(0, 0, 0);
				ExportFile();
				break;
			}
			case 0x68: { // ForceInitialize
				om.Initialize(0, 0, 0);
				ForceInitialize();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mii.Detail.IImageDatabaseService");
		}
	}
}

