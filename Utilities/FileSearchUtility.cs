using global::System;
using global::System.Collections.Generic;
using global::System.IO;
using global::System.Linq;
using global::Microsoft.Extensions.FileSystemGlobbing;
using global::Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace SwiftXP.SPT.Common.Services;

public class FileSearchUtility
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