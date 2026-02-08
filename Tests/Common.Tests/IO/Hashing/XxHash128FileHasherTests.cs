using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Hashing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SwiftXP.SPT.Common.IO.Hashing;
using Xunit;

namespace SwiftXP.SPT.Common.Tests.IO.Hashing;

public class XxHash128FileHasherTests
{
    private sealed class TempDirectory : IDisposable
    {
        public DirectoryInfo DirInfo { get; }

        public TempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            DirInfo = Directory.CreateDirectory(path);
        }

        public FileInfo CreateFile(string fileName, string content)
        {
            string fullPath = Path.Combine(DirInfo.FullName, fileName);
            File.WriteAllText(fullPath, content);

            return new FileInfo(fullPath);
        }

        public void Dispose()
        {
            if (DirInfo.Exists)
                try { DirInfo.Delete(true); } catch { }
        }
    }

    [Fact]
    public async Task GetFileHashAsyncReturnsCorrectHashForExistingFile()
    {
        using TempDirectory temp = new();
        string content = "Hello World";
        FileInfo file = temp.CreateFile("test.txt", content);

        XxHash128FileHasher hasher = new();

        string? result = await hasher.GetFileHashAsync(file);

        Assert.NotNull(result);

        byte[] expectedBytes = XxHash128.Hash(Encoding.UTF8.GetBytes(content));
        string expectedHash = Convert.ToHexStringLower(expectedBytes);

        Assert.Equal(expectedHash, result);
    }

    [Fact]
    public async Task GetFileHashAsyncReturnsNullWhenFileDoesNotExist()
    {
        using TempDirectory temp = new();
        FileInfo nonExistentFile = new(Path.Combine(temp.DirInfo.FullName, "ghost.txt"));

        XxHash128FileHasher hasher = new();

        string? result = await hasher.GetFileHashAsync(nonExistentFile);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetFileHashAsyncWorksWithEmptyFile()
    {
        using TempDirectory temp = new();
        FileInfo file = temp.CreateFile("empty.txt", string.Empty);

        XxHash128FileHasher hasher = new();

        string? result = await hasher.GetFileHashAsync(file);

        byte[] expectedBytes = XxHash128.Hash(Array.Empty<byte>());
        string expectedHash = Convert.ToHexStringLower(expectedBytes);

        Assert.Equal(expectedHash, result);
    }

    [Fact]
    public async Task GetFileHashesAsyncReturnsDictionaryWithOnlyExistingFiles()
    {
        using TempDirectory temp = new();
        FileInfo file1 = temp.CreateFile("file1.txt", "Content A");
        FileInfo file2 = temp.CreateFile("file2.txt", "Content B");
        FileInfo ghostFile = new(Path.Combine(temp.DirInfo.FullName, "ghost.txt"));

        List<FileInfo> filesToHash = [file1, ghostFile, file2];
        XxHash128FileHasher hasher = new();

        Dictionary<string, string> result = await hasher.GetFileHashesAsync(filesToHash);

        Assert.Equal(2, result.Count);
        Assert.Contains(file1.FullName, result.Keys);
        Assert.Contains(file2.FullName, result.Keys);
        Assert.DoesNotContain(ghostFile.FullName, result.Keys);

        string hashA = Convert.ToHexStringLower(XxHash128.Hash(Encoding.UTF8.GetBytes("Content A")));

        Assert.Equal(hashA, result[file1.FullName]);
    }

    [Fact]
    public async Task GetFileHashAsyncThrowsTaskCanceledExceptionWhenCancelled()
    {
        using TempDirectory temp = new();

        string content = new('a', 1024 * 1024);
        FileInfo file = temp.CreateFile("large.txt", content);

        XxHash128FileHasher hasher = new();
        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await hasher.GetFileHashAsync(file, cts.Token);
        });
    }

    [Fact]
    public async Task GetFileHashAsyncReleasesFileHandleAfterHashing()
    {
        using TempDirectory temp = new();
        FileInfo file = temp.CreateFile("locktest.txt", "test");
        XxHash128FileHasher hasher = new();

        await hasher.GetFileHashAsync(file);

        exception = Record.Exception(() =>
        {
            using FileStream fs = file.Open(FileMode.Open, FileAccess.Write, FileShare.None);
        });

        Assert.Null(exception);
    }

    private Exception? exception;
}