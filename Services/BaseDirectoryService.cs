using System;
using System.IO;
using System.Linq;
using SwiftXP.SPT.Common.Services.Interfaces;

namespace SwiftXP.SPT.Common.Services;

#if NET9_0_OR_GREATER
[SPTarkov.DI.Annotations.Injectable(SPTarkov.DI.Annotations.InjectionType.Scoped)]
#endif
public class BaseDirectoryService : IBaseDirectoryService
{
    private readonly Lazy<string> _cachedBaseDirectory;

    public BaseDirectoryService()
    {
        _cachedBaseDirectory = new Lazy<string>(ResolvePath);
    }

    public string GetEftBaseDirectory() => _cachedBaseDirectory.Value;

    private string ResolvePath()
    {
        string baseDirectory = Path.GetFullPath(AppContext.BaseDirectory);

        if (File.Exists(Path.Combine(baseDirectory, "EscapeFromTarkov.exe")))
        {
            return baseDirectory;
        }

        if (FileExists(baseDirectory, "SPT.Server.exe") ||
            FileExists(baseDirectory, "SPT.Server.Linux"))
        {
            DirectoryInfo directoryInfo = new(baseDirectory);
            if (directoryInfo.Parent == null)
            {
                throw new DirectoryNotFoundException($"Parent directory not found for '{baseDirectory}'.");
            }

            return directoryInfo.Parent.FullName;
        }

        throw new FileNotFoundException(
            "Could not resolve base directory. Neither 'EscapeFromTarkov.exe' nor server executables were found.",
            baseDirectory);
    }

    private static bool FileExists(string directory, string filename)
    {
        if (!Directory.Exists(directory))
            return false;

        return Directory.EnumerateFiles(directory)
                        .Select(Path.GetFileName)
                        .Any(x => x != null && x.Equals(filename, StringComparison.OrdinalIgnoreCase));
    }
}