using System;
using System.IO;
using System.Linq;
using SwiftXP.SPT.Common.Services.Interfaces;

namespace SwiftXP.SPT.Common.Utilities;

public static class BaseDirectoryUtility
{
    private static Lazy<string> s_cachedBaseDirectory = new(ResolvePath);

    public static string GetEftBaseDirectory() => s_cachedBaseDirectory.Value;

    private static string ResolvePath()
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