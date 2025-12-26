namespace UmbraCore.Core;

public interface IVfs {
    IEnumerable<(string Path, bool IsFile, ulong Size)> AllEntries { get; }
    IEnumerable<string> EnumerateDirectories(string dir);
    IEnumerable<string> EnumerateFiles(string dir);
    long GetFileSize(string Path);
    IVfsFile OpenRead(string path);
    IVfsFile OpenWrite(string path);
}

public interface IVfsFile : IDisposable {
    ulong Position { get; set; }
    ulong Read(Span<byte> data);
    ulong Write(Span<byte> data);
}

public class DirectoryBackedVfs(string Root) : IVfs {
    public IEnumerable<(string Path, bool IsFile, ulong Size)> AllEntries =>
        Directory.EnumerateFileSystemEntries(Root, "*", SearchOption.AllDirectories)
            .Where(path => Path.GetFileName(path) != ".DS_Store")
            .Select(path => Directory.Exists(path)
                ? (Path.GetRelativePath(Root, path).Replace('\\', '/'), false, 0)
                : (Path.GetRelativePath(Root, path).Replace('\\', '/'), true, (ulong) new FileInfo(path).Length));

    public IEnumerable<string> EnumerateDirectories(string dir) =>
        Directory.EnumerateDirectories(Path.Join(Root, dir), "*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName);
    public IEnumerable<string> EnumerateFiles(string dir) =>
        Directory.EnumerateFiles(Path.Join(Root, dir), "*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(fn => fn != ".DS_Store");

    public long GetFileSize(string path) => new FileInfo(Path.Join(Root, path)).Length;
    public IVfsFile OpenRead(string path) => throw new NotImplementedException();
    public IVfsFile OpenWrite(string path) => throw new NotImplementedException();
}

public class EmptyVfs : IVfs {
    public IEnumerable<(string Path, bool IsFile, ulong Size)> AllEntries => [];
    public IEnumerable<string> EnumerateDirectories(string dir) => [];
    public IEnumerable<string> EnumerateFiles(string dir) => [];
    public long GetFileSize(string path) => throw new NotImplementedException();
    public IVfsFile OpenRead(string path) => null;
    public IVfsFile OpenWrite(string path) => null;
}
