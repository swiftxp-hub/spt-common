using global::System;
using global::System.Collections.Generic;
using global::System.IO;
using SwiftXP.SPT.Common.Extensions;
using SwiftXP.SPT.Common.Services.Interfaces;

namespace SwiftXP.SPT.Common.Services;

#if NET9_0_OR_GREATER
[SPTarkov.DI.Annotations.Injectable(SPTarkov.DI.Annotations.InjectionType.Scoped)]
#endif
public class FileSearchService : IFileSearchService
{
    public IEnumerable<string> GetFiles(string baseDirectory, IEnumerable<string> pathsToSearch, IEnumerable<string> pathsToExclude)
    {
        HashSet<string> searchRoots = ResolvePaths(baseDirectory, pathsToSearch);

        var (excludedFiles, excludedDirectories) = ParseExclusions(baseDirectory, pathsToExclude);

        foreach (string searchPath in searchRoots)
        {
            if (File.Exists(searchPath))
            {
                if (!IsExcluded(searchPath, excludedFiles, excludedDirectories))
                {
                    yield return searchPath.ToUnixStylePath();
                }
            }
            else if (Directory.Exists(searchPath))
            {
                IEnumerable<string> files = Directory.EnumerateFiles(searchPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (IsExcluded(file, excludedFiles, excludedDirectories))
                    {
                        continue;
                    }

                    yield return file.ToUnixStylePath();
                }
            }
        }
    }

    private static HashSet<string> ResolvePaths(string baseDirectory, IEnumerable<string> relativePaths)
    {
        HashSet<string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string relativePath in relativePaths)
        {
            string normalizedPath = relativePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            string fullPath = Path.GetFullPath(normalizedPath, baseDirectory);
            
            result.Add(fullPath);
        }

        return result;
    }

    private static (HashSet<string> Files, List<string> Directories) ParseExclusions(string baseDirectory, IEnumerable<string> pathsToExclude)
    {
        HashSet<string> files = new(StringComparer.OrdinalIgnoreCase);
        List<string> directories = [];

        foreach (string path in pathsToExclude)
        {
            string normalizedPath = path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            string fullPath = Path.GetFullPath(normalizedPath, baseDirectory);
            
            if (Directory.Exists(fullPath)) 
            {
                directories.Add(fullPath);
            }
            else
            {
                files.Add(fullPath);
            }
        }

        return (files, directories);
    }

    private static bool IsExcluded(string filePath, HashSet<string> excludedFiles, List<string> excludedDirectories)
    {
        if (excludedFiles.Contains(filePath))
            return true;

        foreach (string dir in excludedDirectories)
        {
            if (filePath.StartsWith(dir, StringComparison.OrdinalIgnoreCase))
            {
                int dirLen = dir.Length;
                if (filePath.Length > dirLen && IsDirectorySeparator(filePath[dirLen]))
                    return true;
            }
        }

        return false;
    }

    private static bool IsDirectorySeparator(char c)
    {
        return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
    }
}