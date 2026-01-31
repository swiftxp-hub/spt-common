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

        string? eftPath = Directory.EnumerateFiles(baseDirectory, "EscapeFromTarkov.exe", SearchOption.AllDirectories).FirstOrDefault();
        if (eftPath != null)
        {
            return Path.GetDirectoryName(eftPath)!;
        }

        string? serverPath = Directory.EnumerateFiles(baseDirectory, "SPT.Server.exe", SearchOption.AllDirectories).FirstOrDefault();
        serverPath ??= Directory.EnumerateFiles(baseDirectory, "SPT.Server.Linux", SearchOption.AllDirectories).FirstOrDefault();
        serverPath ??= Directory.EnumerateFiles(baseDirectory, "SPT.Server.ARM64", SearchOption.AllDirectories).FirstOrDefault();

        if (serverPath != null)
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
}