using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using DoubleSharp.Buffers;
using LibSharpRetro;
using UmbraCore.Core;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fssrv.Sf;

enum FileAccessMode {
    Read = 1,
    Write = 2,
    ReadWrite = 3,
    AllowAppend = 4,
}

class RomFSIStorageVfsBacked : IStorage {
    readonly IVfs Vfs;
    readonly ImmutableOffsetTable<long, string> FileOffsets;
    readonly byte[] HeaderData;
    (IVfsFile File, string Path) LastFile = (null, null);

    public RomFSIStorageVfsBacked(IVfs vfs) {
        const uint None = 0xFFFFFFFF;
        
        Vfs = vfs;
        var blob = new Blob();
        var numDirs = Vfs.AllEntries.Count(x => !x.IsFile);
        var dirTableCount = CountHashTables(numDirs + 1);
        var numFiles = Vfs.AllEntries.Count(x => x.IsFile);
        var fileTableCount = CountHashTables(numFiles);

        var dirEntries = new (DirEntry Entry, string Name)[numDirs + 1];
        var fileEntries = new (FileEntry Entry, string Name)[numFiles];
        var dirEntryOff = 0U;
        var fileEntryOff = 0U;
        var filePosOff = 0UL;
        var filePartOffsets = new List<(string Path, ulong Start, ulong Size)>();

        uint BuildTables(string dir, string path) {
            var off = dirEntryOff++;
            var dirEntry = new DirEntry {
                Child = None,
                Sibling = None,
                File = None,
            };

            var lastDir = 0U;
            foreach(var (i, sub) in Vfs.EnumerateDirectories(path).Order().Index()) {
                var noff = BuildTables(sub, $"{path}/{sub}");
                ref var nent = ref dirEntries[noff];
                nent.Entry.Parent = off;
                if(i == 0)
                    dirEntry.Child = noff;
                else
                    dirEntries[lastDir].Entry.Sibling = noff;
                lastDir = noff;
            }
            var lastFile = 0U;
            foreach(var (i, sub) in Vfs.EnumerateFiles(path).Order().Index()) {
                var noff = fileEntryOff++;
                var size = (ulong) Vfs.GetFileSize($"{path}/{sub}");
                var nent = new FileEntry {
                    Parent = off,
                    Sibling = None,
                    NameSize = (uint) sub.Length,
                    Offset = filePosOff,
                    Size = size,
                };
                filePartOffsets.Add(($"{path}/{sub}", filePosOff, size));
                filePosOff += size;
                
                if(i == 0)
                    dirEntry.File = noff;
                else
                    fileEntries[lastFile].Entry.Sibling = noff;
                lastFile = noff;
                fileEntries[noff] = (nent, sub);
            }
            
            dirEntry.NameSize = (uint) (dir == "/" ? 0 : dir.Length);
            dirEntries[off] = (dirEntry, dir == "/" ? "" : dir);
            return off;
        }
        BuildTables("/", "/");
        Debug.Assert(dirEntryOff == dirEntries.Length);
        Debug.Assert(fileEntryOff == fileEntries.Length);
        
        var dirParentIndex = dirEntries.Select(x => x.Entry.Parent).ToArray();
        var fileParentIndex = fileEntries.Select(x => x.Entry.Parent).ToArray();
        
        var dirOffsets = new uint[dirEntries.Length];
        var dirOff = 0U;
        for(var i = 0; i < dirEntries.Length; i++) {
            dirOffsets[i] = dirOff;
            var (_, name) = dirEntries[i];
            dirOff += (uint) (6 * 4 + name.Length);
            while((dirOff & 3) != 0) dirOff++;
        }
        var fileOffsets = new uint[fileEntries.Length];
        var fileOff = 0U;
        for(var i = 0; i < fileEntries.Length; i++) {
            fileOffsets[i] = fileOff;
            var (_, name) = fileEntries[i];
            fileOff += (uint) (8 * 4 + name.Length);
            while((fileOff & 3) != 0) fileOff++;
        }
        for(var i = 0; i < dirEntries.Length; i++) {
            ref var entry = ref dirEntries[i].Entry;
            entry.Parent = entry.Parent == None ? None : dirOffsets[entry.Parent];
            entry.Sibling = entry.Sibling == None ? None : dirOffsets[entry.Sibling];
            entry.Child = entry.Child == None ? None : dirOffsets[entry.Child];
            entry.File = entry.File == None ? None : fileOffsets[entry.File];
        }
        for(var i = 0; i < fileEntries.Length; i++) {
            ref var entry = ref fileEntries[i].Entry;
            entry.Parent = entry.Parent == None ? None : dirOffsets[entry.Parent];
            entry.Sibling = entry.Sibling == None ? None : fileOffsets[entry.Sibling];
        }

        var dirHashTable = new uint[dirTableCount];
        Array.Fill(dirHashTable, None);
        for (var i = 0; i < dirEntries.Length; i++) {
            var entryOff = dirOffsets[i];

            var parentForHash = i == 0 ? 0U : dirOffsets[dirParentIndex[i]];
            var h = CalcPathHash(parentForHash, dirEntries[i].Name);
            var b = h % (uint) dirHashTable.Length;
            
            ref var entry = ref dirEntries[i].Entry;
            entry.Hash = dirHashTable[b];
            dirHashTable[b] = entryOff;
        }
        var fileHashTable = new uint[fileTableCount];
        Array.Fill(fileHashTable, None);
        for (var i = 0; i < fileEntries.Length; i++) {
            var entryOff = fileOffsets[i];

            var parentForHash = dirOffsets[fileParentIndex[i]];

            var h = CalcPathHash(parentForHash, fileEntries[i].Name);
            var b = h % (uint) fileHashTable.Length;

            ref var entry = ref fileEntries[i].Entry;
            entry.Hash = fileHashTable[b];
            fileHashTable[b] = entryOff;
        }

        var dirHashTableOff = 0x50U;
        var fileHashTableOff = dirHashTableOff + (uint) dirHashTable.Length * 4;
        var dirTableOff = fileHashTableOff + (uint) fileHashTable.Length * 4;
        var fileTableOff = dirTableOff + dirOff;
        var partitionOff = fileTableOff + fileOff;
        var header = new Header {
            HeaderSize = 0x48,
            DirHashTableOff = dirHashTableOff,
            DirHashTableSize = (uint) dirHashTable.Length * 4,
            DirTableOff = dirTableOff,
            DirTableSize = dirOff,
            FileHashTableOff = fileHashTableOff,
            FileHashTableSize = (uint) fileHashTable.Length * 4,
            FileTableOff = fileTableOff,
            FileTableSize = fileOff,
            FilePartitionOff = partitionOff,
        };

        blob.Write(0, header);
        blob.Write((int) dirHashTableOff, dirHashTable);
        blob.Write((int) fileHashTableOff, fileHashTable);
        for(var i = 0; i < dirEntries.Length; i++) {
            var offset = dirTableOff + dirOffsets[i];
            var (entry, name) = dirEntries[i];
            blob.Write((int) offset, entry);
            blob.WriteBytes((int) (offset + 6 * 4), name);
        }
        for(var i = 0; i < fileEntries.Length; i++) {
            var offset = fileTableOff + fileOffsets[i];
            var (entry, name) = fileEntries[i];
            blob.Write((int) offset, entry);
            blob.WriteBytes((int) (offset + 8 * 4), name);
        }

        FileOffsets = new(filePartOffsets.Select(x => ((long) (partitionOff + x.Start), (long) x.Size, x.Path)));
        HeaderData = blob.ToArray();
    }

    static uint CalcPathHash(uint parent, string path) {
        var hash = parent ^ 123456789U;
        foreach(var c in path) {
            hash = (hash >> 5) | (hash << 27);
            Debug.Assert(c <= 0x7F);
            hash ^= c;
        }
        return hash;
    }

    static int CountHashTables(int count) {
        if(count < 3) return 3;
        if(count < 19) return count | 1;
        while (count % 2 == 0 || count % 3 == 0 || count % 5 == 0 || count % 7 == 0 || count % 11 == 0 || count % 13 == 0 || count % 17 == 0)
            count++;
        return count;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Header {
        public ulong HeaderSize;
        public ulong DirHashTableOff, DirHashTableSize;
        public ulong DirTableOff, DirTableSize;
        public ulong FileHashTableOff, FileHashTableSize;
        public ulong FileTableOff, FileTableSize;
        public ulong FilePartitionOff;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DirEntry {
        public uint Parent, Sibling, Child;
        public uint File, Hash;
        public uint NameSize;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileEntry {
        public uint Parent, Sibling;
        public ulong Offset, Size;
        public uint Hash;
        public uint NameSize;
    }
    
    protected override void Read(ulong offset, ulong length, Span<byte> data) {
        $"IStorageVfsBacked::Read -- 0x{offset:X} 0x{length:X} (0x{data.Length:X} byte buffer)".Log();
        if(length == 0) return;
        if(offset >= (ulong) HeaderData.Length) {
            if(FileOffsets.TryGetEntry((long) offset, out var foff, out var size, out var path)) {
                $"Reading from file '{path}' -- 0x{foff:X} 0x{offset:X}".Log();
                if(LastFile.File != null) {
                    if(LastFile.Path == path) {
                        LastFile.File.Position = offset - (ulong) foff;
                        LastFile.File.Read(data);
                        return;
                    }
                    LastFile.File.Dispose();
                    LastFile = (null, null);
                }
                var file = Vfs.OpenRead(path);
                LastFile = (file, path);
                file.Position = offset - (ulong) foff;
                file.Read(data);
                return;
            }
            throw new NotImplementedException();
        }
        HeaderData.AsSpan((int) offset, Math.Min((int) length, HeaderData.Length - (int) offset)).CopyTo(data);
    }}

public class SaveDataFileSystem : IFileSystem {
    protected override IFile OpenFile(uint mode, Span<byte> path) {
        $"Attempting to open save data file '{Encoding.ASCII.GetString(path)}' with mode {mode}!".Log();
        if(mode == (uint) FileAccessMode.Read)
            throw new IpcException(0x80070002); // File not found
        return new IFile();
    }
}

public partial class IFileSystemProxy {
    protected override IStorage OpenDataStorageByCurrentProcess() => new RomFSIStorageVfsBacked(new DirectoryBackedVfs(Kernel.RomFsPath));
    protected override IStorage OpenPatchDataStorageByCurrentProcess() => new RomFSIStorageVfsBacked(new EmptyVfs());
    protected override IFileSystem OpenSaveDataFileSystem(byte save_data_space_id, byte[] save_struct) => new SaveDataFileSystem();
    protected override uint GetGlobalAccessLogMode() => 0;
}