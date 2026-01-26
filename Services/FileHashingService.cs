using global::System;
using global::System.Collections.Generic;
using global::System.IO;
using global::System.Security.Cryptography;
using SwiftXP.SPT.Common.Services.Interfaces;

namespace SwiftXP.SPT.Common.Services;

#if NET9_0_OR_GREATER
[SPTarkov.DI.Annotations.Injectable(SPTarkov.DI.Annotations.InjectionType.Scoped)]
#endif
public class FileHashingService : IFileHashingService
{
    public Dictionary<string, string> GetFileHashes(IEnumerable<string> filePathsToHash)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string filePath in filePathsToHash)
        {
            if (!File.Exists(filePath))
                continue;

            string fileHash = GetFileHash(filePath);
            result.Add(filePath, fileHash);
        }

        return result;
    }
    
    private static string GetFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        byte[] hashBytes = sha256.ComputeHash(stream);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}