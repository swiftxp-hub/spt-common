using System.Collections.Generic;

namespace SwiftXP.SPT.Common.Services.Interfaces;

public interface IFileSearchService
{
    IEnumerable<string> GetFiles(string baseDirectory, IEnumerable<string> pathsToSearch, IEnumerable<string> pathsToExclude);
}