using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using Blake3;

namespace Deep.Toolkit.Helper;

public static class FileHash
{
    public static async Task<string> GetHashAsync(
        HashAlgorithm hashAlgorithm,
        Stream stream,
        byte[] buffer,
        Action<ulong>? progress = default
    )
    {
        ulong totalBytesRead = 0;

        using (hashAlgorithm)
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) != 0)
            {
                totalBytesRead += (ulong)bytesRead;
                hashAlgorithm.TransformBlock(buffer, 0, bytesRead, null, 0);
                progress?.Invoke(totalBytesRead);
            }

            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            var hash = hashAlgorithm.Hash;
            if (hash == null || hash.Length == 0)
                throw new InvalidOperationException("Hash algorithm did not produce a hash.");

            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
        }
    }

    /// <summary>
    ///     Get the Blake3 hash of a span of data with multi-threading.
    /// </summary>
    public static Hash GetBlake3Parallel(ReadOnlySpan<byte> data)
    {
        using var hasher = Hasher.New();
        hasher.UpdateWithJoin(data);
        return hasher.Finalize();
    }

    /// <summary>
    ///     Task.Run wrapped <see cref="GetBlake3Parallel" />
    /// </summary>
    public static Task<Hash> GetBlake3ParallelAsync(ReadOnlyMemory<byte> data)
    {
        return Task.Run(() => GetBlake3Parallel(data.Span));
    }

    /// <summary>
    ///     Get the Blake3 hash of a file as memory-mapped with multi-threading.
    /// </summary>
    public static Hash GetBlake3MemoryMappedParallel(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

        var totalBytes = Convert.ToInt64(new FileInfo(filePath).Length);

        using var hasher = Hasher.New();

        // Memory map
        using var fileStream = File.OpenRead(filePath);
        using var memoryMappedFile = MemoryMappedFile.CreateFromFile(
            fileStream,
            null,
            totalBytes,
            MemoryMappedFileAccess.Read,
            HandleInheritability.None,
            false
        );

        using var accessor =
            memoryMappedFile.CreateViewAccessor(0, totalBytes, MemoryMappedFileAccess.Read);

        Debug.Assert(accessor.Capacity == fileStream.Length);

        var buffer = new byte[accessor.Capacity];
        accessor.ReadArray(0, buffer, 0, buffer.Length);

        hasher.UpdateWithJoin(buffer);
        return hasher.Finalize();
    }

    /// <summary>
    ///     Task.Run wrapped <see cref="GetBlake3MemoryMappedParallel" />
    /// </summary>
    public static Task<Hash> GetBlake3MemoryMappedParallelAsync(string filePath)
    {
        return Task.Run(() => GetBlake3MemoryMappedParallel(filePath));
    }

    /// <summary>
    ///     Determines suitable buffer size for hashing based on stream length.
    /// </summary>
    private static int GetBufferSize(ulong totalBytes)
    {
        return totalBytes switch
        {
            < Size.MiB => 8 * (int)Size.KiB,
            < 500 * Size.MiB => 16 * (int)Size.KiB,
            < Size.GiB => 32 * (int)Size.KiB,
            _ => 64 * (int)Size.KiB
        };
    }
}