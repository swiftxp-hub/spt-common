using global::System;
using global::System.Collections.Generic;
using global::System.IO;
using global::System.Linq;
using global::Microsoft.Extensions.FileSystemGlobbing;
using global::Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using SwiftXP.SPT.Common.Services.Interfaces;

namespace SwiftXP.SPT.Common.Services;

#if NET9_0_OR_GREATER
[SPTarkov.DI.Annotations.Injectable(SPTarkov.DI.Annotations.InjectionType.Scoped)]
#endif
public class FileSearchService : IFileSearchService
{
    public IEnumerable<string> GetFiles(string baseDirectory, IEnumerable<string> pathsToSearch, IEnumerable<string> pathsToExclude)
    {
        if (string.IsNullOrEmpty(baseDirectory) || !Directory.Exists(baseDirectory))
        {
            return [];
        }

        Matcher matcher = new(StringComparison.OrdinalIgnoreCase);
        matcher.AddIncludePatterns(pathsToSearch);
        matcher.AddExcludePatterns(pathsToExclude);

        DirectoryInfo directoryInfo = new(baseDirectory);
        PatternMatchingResult result = matcher.Execute(new DirectoryInfoWrapper(directoryInfo));

        if (!result.HasMatches)
        {
            return [];
        }

        return result.Files.Select(file => Path.GetFullPath(Path.Combine(baseDirectory, file.Path)));
    }
}