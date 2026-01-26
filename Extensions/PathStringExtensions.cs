using System.IO;

namespace SwiftXP.SPT.Common.Extensions;

public static class PathStringExtensions
{
    public static string ToUnixStylePath(this string str) =>
        str.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
}