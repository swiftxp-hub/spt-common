using System;
using System.IO;
using Xunit;
using SwiftXP.SPT.Common.Environment;

namespace SwiftXP.SPT.Common.Tests.Environment;

public class BaseDirectoryLocatorTests
{
    private sealed class TempDirectory : IDisposable
    {
        public DirectoryInfo DirInfo { get; }

        public TempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            DirInfo = Directory.CreateDirectory(path);
        }

        public string CreateSubdirectory(string name)
        {
            return DirInfo.CreateSubdirectory(name).FullName;
        }

        public void CreateFile(string fileName, string subPath = "")
        {
            string folder = string.IsNullOrEmpty(subPath) ? DirInfo.FullName : Path.Combine(DirInfo.FullName, subPath);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(Path.Combine(folder, fileName), "dummy content");
        }

        public void Dispose()
        {
            if (DirInfo.Exists)
            {
                try { DirInfo.Delete(true); } catch { }
            }
        }
    }

    [Fact]
    public void GetBaseDirectoryReturnsPathWhenEscapeFromTarkovExeIsAtRoot()
    {
        using TempDirectory temp = new();
        temp.CreateFile("EscapeFromTarkov.exe");

        BaseDirectoryLocator locator = new(temp.DirInfo.FullName);
        string result = locator.GetBaseDirectory();

        Assert.Equal(temp.DirInfo.FullName, result);
    }

    [Fact]
    public void GetBaseDirectoryWalksUpTreeToFindEscapeFromTarkovExe()
    {
        using TempDirectory temp = new();
        temp.CreateFile("EscapeFromTarkov.exe");

        string deepPath = temp.CreateSubdirectory("Sub/Deep");

        BaseDirectoryLocator locator = new BaseDirectoryLocator(deepPath);
        string result = locator.GetBaseDirectory();

        Assert.Equal(temp.DirInfo.FullName, result);
    }

    [Fact]
    public void GetBaseDirectoryReturnsParentWhenInServerDirectoryWindows()
    {
        using TempDirectory temp = new();

        string serverPath = temp.CreateSubdirectory("Server");
        temp.CreateFile("SPT.Server.exe", "Server");

        BaseDirectoryLocator locator = new(serverPath);
        string result = locator.GetBaseDirectory();

        Assert.Equal(temp.DirInfo.FullName, result);
    }

    [Fact]
    public void GetBaseDirectoryReturnsParentWhenInServerDirectoryLinux()
    {
        using TempDirectory temp = new();
        string serverPath = temp.CreateSubdirectory("Server");

        temp.CreateFile("SPT.Server.Linux", "Server");

        BaseDirectoryLocator locator = new(serverPath);
        string result = locator.GetBaseDirectory();

        Assert.Equal(temp.DirInfo.FullName, result);
    }

    [Fact]
    public void GetBaseDirectoryUsesScanSpecificDepthToFindExeInSubfolder()
    {
        using TempDirectory temp = new();

        string subPath = temp.CreateSubdirectory("Sub");
        temp.CreateFile("EscapeFromTarkov.exe", "Sub");

        BaseDirectoryLocator locator = new(temp.DirInfo.FullName);
        string result = locator.GetBaseDirectory();

        Assert.Equal(subPath, result);
    }

    [Fact]
    public void GetBaseDirectoryThrowsFileNotFoundWhenNothingIsFound()
    {
        using TempDirectory temp = new();

        BaseDirectoryLocator locator = new(temp.DirInfo.FullName);

        Assert.Throws<FileNotFoundException>(() => locator.GetBaseDirectory());
    }

    [Fact]
    public void GetBaseDirectoryCachesResult()
    {
        using TempDirectory temp = new();
        temp.CreateFile("EscapeFromTarkov.exe");

        BaseDirectoryLocator locator = new(temp.DirInfo.FullName);

        string path1 = locator.GetBaseDirectory();

        File.Delete(Path.Combine(temp.DirInfo.FullName, "EscapeFromTarkov.exe"));

        string path2 = locator.GetBaseDirectory();

        Assert.Equal(path1, path2);
    }
}