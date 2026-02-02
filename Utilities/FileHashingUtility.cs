using System.IO.Hashing;
using System.Threading.Tasks;
using global::System;
using global::System.Collections.Generic;
using global::System.IO;

namespace SwiftXP.SPT.Common.Services;

public static class FileHashingUtility
{
    private const int BufferSize = 1024 * 1024;

    public async static Task<Dictionary<string, string>> GetFileHashes(IEnumerable<string> filePathsToHash)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string filePath in filePathsToHash)
        {
            if (!File.Exists(filePath))
                continue;

            string fileHash = await GetXx128FileHash(filePath);
            result.Add(filePath, fileHash);
        }

        return result;
    }

    private async static Task<string> GetXx128FileHash(string filePath)
    {
        XxHash128 xxHash128 = new();
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.Asynchronous);
        await xxHash128.AppendAsync(fileStream);

        byte[] hashBytes = xxHash128.GetCurrentHash();
#if NET5_0_OR_GREATER
        return Convert.ToHexStringLower(hashBytes);
#else
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
#endif
    }
}