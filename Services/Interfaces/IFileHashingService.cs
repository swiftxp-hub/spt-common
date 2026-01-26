using System.Collections.Generic;

namespace SwiftXP.SPT.Common.Services.Interfaces;

public interface IFileHashingService
{
    Dictionary<string, string> GetFileHashes(IEnumerable<string> filePathsToHash);
}