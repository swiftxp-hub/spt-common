using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SwiftXP.SPT.Common.Extensions.FileSystem;
using Xunit;

namespace SwiftXP.SPT.Common.Tests.Extensions.FileSystem;

public class FileSystemExtensionsTests
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

        public void CreateFile(string relativePath)
        {
            string fullPath = Path.Combine(DirInfo.FullName, relativePath);
            string? dir = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(fullPath, "dummy content");
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
    public void FindFilesByPatternReturnsEmptyWhenBaseDirectoryDoesNotExist()
    {
        string nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        IEnumerable<FileInfo> result = nonExistentDir.FindFilesByPattern(["*.txt"]);

        Assert.Empty(result);
    }

    [Fact]
    public void FindFilesByPatternReturnsEmptyWhenBaseDirectoryIsNull()
    {
        string? nullDir = null;

        IEnumerable<FileInfo> result = nullDir!.FindFilesByPattern(["*.txt"]);

        Assert.Empty(result);
    }

    [Fact]
    public void FindFilesByPatternFindsFilesWithSimplePattern()
    {
        using TempDirectory temp = new();
        temp.CreateFile("match.txt");
        temp.CreateFile("ignore.json");

        IEnumerable<FileInfo> result = temp.DirInfo.FullName.FindFilesByPattern(["*.txt"]);

        Assert.Single(result);
        Assert.Equal("match.txt", result.First().Name);
    }

    [Fact]
    public void FindFilesByPatternFindsFilesRecursive()
    {
        using TempDirectory temp = new();
        temp.CreateFile("root.txt");
        temp.CreateFile("sub/deep.txt");
        temp.CreateFile("sub/ignore.json");

        IEnumerable<FileInfo> result = temp.DirInfo.FullName.FindFilesByPattern(["**/*.txt"]);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, f => f.Name == "root.txt");
        Assert.Contains(result, f => f.Name == "deep.txt");
    }

    [Fact]
    public void FindFilesByPatternRespectsExcludes()
    {
        using TempDirectory temp = new();
        temp.CreateFile("keep.txt");
        temp.CreateFile("exclude.txt");
        temp.CreateFile("sub/exclude.txt");

        IEnumerable<FileInfo> result = temp.DirInfo.FullName.FindFilesByPattern(
            includePatterns: ["**/*.txt"],
            excludePatterns: ["**/exclude.txt"]
        );

        Assert.Single(result);
        Assert.Equal("keep.txt", result.First().Name);
    }

    [Fact]
    public void GetFileInfoReturnsNullForNullOrEmptyPath()
    {
        Assert.Null(FileSystemExtensions.GetFileInfo(null!));
        Assert.Null(FileSystemExtensions.GetFileInfo(string.Empty));
        Assert.Null(FileSystemExtensions.GetFileInfo("   "));
    }

    [Fact]
    public void GetFileInfoReturnsNullForInvalidPathCharacters()
    {
        if (OperatingSystem.IsWindows())
            Assert.Null(FileSystemExtensions.GetFileInfo("invalidpath|<>.txt"));
    }

    [Fact]
    public void GetFileInfoReturnsNullWhenFileDoesNotExist()
    {
        using TempDirectory temp = new();
        string path = Path.Combine(temp.DirInfo.FullName, "ghost.txt");

        FileInfo? info = path.GetFileInfo();

        Assert.Null(info);
    }

    [Fact]
    public void GetFileInfoReturnsFileInfoWhenFileExists()
    {
        using TempDirectory temp = new();
        temp.CreateFile("exists.txt");
        string path = Path.Combine(temp.DirInfo.FullName, "exists.txt");

        FileInfo? info = path.GetFileInfo();

        Assert.NotNull(info);
        Assert.True(info!.Exists);
        Assert.Equal("exists.txt", info.Name);
    }

    [Fact]
    public void GetWebFriendlyPathReplacesBackslashes()
    {
        string path = @"folder\subfolder\file.txt";
        string expected = "folder/subfolder/file.txt";

        Assert.Equal(expected, path.GetWebFriendlyPath());
    }

    [Fact]
    public void GetWebFriendlyPathKeepsForwardSlashes()
    {
        string path = "folder/subfolder/file.txt";

        Assert.Equal(path, path.GetWebFriendlyPath());
    }

    [Fact]
    public void GetWebFriendlyPathHandlesMixedSlashes()
    {
        string path = @"folder\subfolder/file.txt";
        string expected = "folder/subfolder/file.txt";

        Assert.Equal(expected, path.GetWebFriendlyPath());
    }

    [Theory]
    [InlineData("test.log", new[] { "*.log" }, true)]
    [InlineData("test.txt", new[] { "*.log" }, false)]
    [InlineData("logs/app.log", new[] { "logs/**" }, true)]
    [InlineData("bin/app.dll", new[] { "*.log" }, false)]
    public void IsExcludedByPatternsValidatesCorrectly(string filePath, string[] patterns, bool expected)
    {
        bool result = filePath.IsExcludedByPatterns(patterns);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsExcludedByPatternsHandlesCaseInsensitivity()
    {
        string filePath = "Test.LOG";
        string[] patterns = ["*.log"];

        bool result = filePath.IsExcludedByPatterns(patterns);

        Assert.True(result, "Should match regardless of case");
    }
}