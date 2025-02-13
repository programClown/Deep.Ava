using System.Collections;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Deep.Toolkit.Converters.Json;
using JetBrains.Annotations;

namespace Deep.Toolkit.Models.FileInterfaces;

[PublicAPI]
[Localizable(false)]
[JsonConverter(typeof(StringJsonConverter<DirectoryPath>))]
public class DirectoryPath : FileSystemPath, IPathObject, IEnumerable<FileSystemPath>
{
    private DirectoryInfo? info;

    public DirectoryPath([Localizable(false)] string path)
        : base(path)
    {
    }

    public DirectoryPath(FileSystemPath path)
        : base(path)
    {
    }

    public DirectoryPath(DirectoryInfo info)
        : base(info.FullName)
    {
        // Additionally set the info field
        this.info = info;
    }

    public DirectoryPath([Localizable(false)] params string[] paths)
        : base(paths)
    {
    }

    [JsonIgnore] public DirectoryInfo Info => info ??= new DirectoryInfo(FullPath);

    /// <summary>
    ///     Get the parent directory.
    /// </summary>
    [JsonIgnore]
    public DirectoryPath? Parent => Info.Parent == null ? null : new DirectoryPath(Info.Parent);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IEnumerator<FileSystemPath> GetEnumerator()
    {
        return Info.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly)
            .Select<FileSystemInfo, FileSystemPath>(
                fsInfo =>
                    fsInfo switch
                    {
                        FileInfo file => new FilePath(file),
                        DirectoryInfo directory => new DirectoryPath(directory),
                        _ => throw new InvalidOperationException("Unknown file system info type")
                    }
            )
            .GetEnumerator();
    }

    [JsonIgnore] FileSystemInfo IPathObject.Info => Info;

    [JsonIgnore]
    public bool IsSymbolicLink
    {
        get
        {
            Info.Refresh();
            return Info.Exists && Info.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the directory exists.
    /// </summary>
    [JsonIgnore]
    public bool Exists => Info.Exists;

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => Info.Name;

    /// <inheritdoc />
    public long GetSize()
    {
        Info.Refresh();
        return Info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }

    void IPathObject.Delete()
    {
        Info.Delete(true);
    }

    Task IPathObject.DeleteAsync()
    {
        return DeleteAsync(true);
    }

    public DirectoryPath RelativeTo(DirectoryPath path)
    {
        return new DirectoryPath(Path.GetRelativePath(path.FullPath, FullPath));
    }

    /// <summary>
    ///     Gets the size of the directory.
    /// </summary>
    /// <param name="includeSymbolicLinks">
    ///     Whether to include files and subdirectories that are symbolic links / reparse points.
    /// </param>
    public long GetSize(bool includeSymbolicLinks)
    {
        if (includeSymbolicLinks)
            return GetSize();

        Info.Refresh();
        var files = Info.GetFiles()
            .Where(file => !file.Attributes.HasFlag(FileAttributes.ReparsePoint))
            .Sum(file => file.Length);

        var subDirs = Info.GetDirectories()
            .Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint))
            .Sum(dir => dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));

        return files + subDirs;
    }

    /// <summary>
    ///     Gets the size of the directory asynchronously.
    /// </summary>
    /// <param name="includeSymbolicLinks">
    ///     Whether to include files and subdirectories that are symbolic links / reparse points.
    /// </param>
    public Task<long> GetSizeAsync(bool includeSymbolicLinks)
    {
        return Task.Run(() => GetSize(includeSymbolicLinks));
    }

    /// <summary>
    ///     Creates the directory.
    /// </summary>
    public void Create()
    {
        Directory.CreateDirectory(FullPath);
    }

    /// <summary>
    ///     Deletes the directory.
    /// </summary>
    public void Delete()
    {
        Info.Delete();
    }

    /// <summary> Deletes the directory asynchronously. </summary>
    public Task DeleteAsync()
    {
        return Task.Run(Delete);
    }

    /// <summary>
    ///     Deletes the directory.
    /// </summary>
    /// <param name="recursive">Whether to delete subdirectories and files.</param>
    public void Delete(bool recursive)
    {
        Info.Delete(recursive);
    }

    /// <summary>
    ///     Deletes the directory asynchronously.
    /// </summary>
    public Task DeleteAsync(bool recursive)
    {
        return Task.Run(() => Delete(recursive));
    }

    private void ThrowIfNotExists()
    {
        if (!Exists) throw new DirectoryNotFoundException($"Directory not found: {FullPath}");
    }

    public void CopyTo(DirectoryPath destinationDir, bool recursive = true)
    {
        ThrowIfNotExists();

        // Cache directories before we start copying
        var dirs = EnumerateDirectories().ToList();

        destinationDir.Create();

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in EnumerateFiles())
        {
            var targetFilePath = destinationDir.JoinFile(file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
            foreach (var subDir in dirs)
            {
                var targetDirectory = destinationDir.JoinDir(subDir.Name);
                subDir.CopyTo(targetDirectory);
            }
    }

    public async Task CopyToAsync(DirectoryPath destinationDir, bool recursive = true)
    {
        ThrowIfNotExists();

        // Cache directories before we start copying
        var dirs = EnumerateDirectories().ToList();

        destinationDir.Create();

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in EnumerateFiles())
        {
            var targetFilePath = destinationDir.JoinFile(file.Name);
            await file.CopyToAsync(targetFilePath).ConfigureAwait(false);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
            foreach (var subDir in dirs)
            {
                var targetDirectory = destinationDir.JoinDir(subDir.Name);
                await subDir.CopyToAsync(targetDirectory).ConfigureAwait(false);
            }
    }

    /// <summary>
    ///     Move the directory to a destination path.
    /// </summary>
    public DirectoryPath MoveTo(DirectoryPath destinationDir)
    {
        Info.MoveTo(destinationDir.FullPath);
        // Return the new path
        return destinationDir;
    }

    /// <summary>
    ///     Move the file to a target path.
    /// </summary>
    public async Task<DirectoryPath> MoveToAsync(DirectoryPath destinationDir)
    {
        await Task.Run(() => Info.MoveTo(destinationDir.FullPath)).ConfigureAwait(false);
        // Return the new path
        return destinationDir;
    }

    /// <summary>
    ///     Move the directory to a destination path as a subfolder with the current name.
    /// </summary>
    public async Task<DirectoryPath> MoveToDirectoryAsync(DirectoryPath destinationParentDir)
    {
        await Task.Run(() => Info.MoveTo(destinationParentDir.JoinDir(Name))).ConfigureAwait(false);
        // Return the new path
        return destinationParentDir.JoinDir(this);
    }

    /// <summary>
    ///     Join with other paths to form a new directory path.
    /// </summary>
    public DirectoryPath JoinDir([Localizable(false)] params DirectoryPath[] paths)
    {
        return new DirectoryPath(Path.Combine(FullPath, Path.Combine(paths.Select(path => path.FullPath).ToArray())));
    }

    /// <summary>
    ///     Join with other paths to form a new file path.
    /// </summary>
    public FilePath JoinFile([Localizable(false)] params FilePath[] paths)
    {
        return new FilePath(Path.Combine(FullPath, Path.Combine(paths.Select(path => path.FullPath).ToArray())));
    }

    /// <summary>
    ///     Returns an enumerable collection of files that matches
    ///     a specified search pattern and search subdirectory option.
    /// </summary>
    public IEnumerable<FilePath> EnumerateFiles(
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly
    )
    {
        return Info.EnumerateFiles(searchPattern, searchOption).Select(file => new FilePath(file));
    }

    /// <summary>
    ///     Returns an enumerable collection of directories that matches
    ///     a specified search pattern and search subdirectory option.
    /// </summary>
    public IEnumerable<DirectoryPath> EnumerateDirectories(
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly
    )
    {
        return Info.EnumerateDirectories(searchPattern, searchOption)
            .Select(directory => new DirectoryPath(directory));
    }

    /// <summary>
    ///     Return a new <see cref="DirectoryPath" /> with the given file name.
    /// </summary>
    public DirectoryPath WithName(string directoryName)
    {
        if (Path.GetDirectoryName(FullPath) is { } directory && !string.IsNullOrWhiteSpace(directory))
            return new DirectoryPath(directory, directoryName);

        return new DirectoryPath(directoryName);
    }

    public override string ToString()
    {
        return FullPath;
    }

    // DirectoryPath + DirectoryPath = DirectoryPath
    public static DirectoryPath operator +(DirectoryPath path, DirectoryPath other)
    {
        return new DirectoryPath(Path.Combine(path, other.FullPath));
    }

    // DirectoryPath + FilePath = FilePath
    public static FilePath operator +(DirectoryPath path, FilePath other)
    {
        return new FilePath(Path.Combine(path, other.FullPath));
    }

    // DirectoryPath + FileInfo = FilePath
    public static FilePath operator +(DirectoryPath path, FileInfo other)
    {
        return new FilePath(Path.Combine(path, other.FullName));
    }

    // DirectoryPath + string = string
    public static string operator +(DirectoryPath path, [Localizable(false)] string other)
    {
        return Path.Combine(path, other);
    }

    // Implicit conversions to and from string
    public static implicit operator string(DirectoryPath path)
    {
        return path.FullPath;
    }

    public static implicit operator DirectoryPath([Localizable(false)] string path)
    {
        return new DirectoryPath(path);
    }

    // Implicit conversions to and from DirectoryInfo
    public static implicit operator DirectoryInfo(DirectoryPath path)
    {
        return path.Info;
    }

    public static implicit operator DirectoryPath(DirectoryInfo path)
    {
        return new DirectoryPath(path);
    }
}