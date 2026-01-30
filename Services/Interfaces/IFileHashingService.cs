using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwiftXP.SPT.Common.Services.Interfaces;

public interface IFileHashingService
{
    Task<Dictionary<string, string>> GetFileHashes(IEnumerable<string> filePathsToHash);
}