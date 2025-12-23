// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fssrv.Sf;

class DataStorage : IStorage {
}

class PatchStorage : IStorage {
}

partial class IStorage {
    protected override void Read(ulong offset, ulong length, Span<byte> data) {
        Console.WriteLine($"IStorage::Read -- 0x{offset:X} 0x{length:X}");
    }
}

public partial class IFileSystemProxy {
    protected override IStorage OpenDataStorageByCurrentProcess() => new DataStorage();
    protected override IStorage OpenPatchDataStorageByCurrentProcess() => new PatchStorage();
    protected override uint GetGlobalAccessLogMode() => 0;
}