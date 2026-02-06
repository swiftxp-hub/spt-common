using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using SPT.Common.Models;
using System.Reflection;
using System.Net.Http;
using SPT.Common.Http;

using SptClient = SPT.Common.Http.Client;

namespace SwiftXP.SPT.Common.Http;

public static class SPTClientExtensions
{
    private static readonly FieldInfo s_addressField = typeof(SptClient).GetField("address", BindingFlags.Instance | BindingFlags.NonPublic);

    public static async Task DownloadWithCancellationAsync(
        this SptClient client,
        string path,
        string filePath,
        Action<DownloadProgress>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        string? baseAddress = s_addressField?.GetValue(client) as string;

        if (string.IsNullOrEmpty(baseAddress))
            throw new InvalidOperationException("Could not retrieve base address from Client instance via Reflection.");

        string directoryName = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        using UnityWebRequest request = UnityWebRequest.Get(baseAddress + path);

        request.downloadHandler = new DownloadHandlerFile(filePath)
        {
            removeFileOnAbort = true
        };

        request.certificateHandler = new FakeCertificateHandler();

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        DateTime startTime = DateTime.UtcNow;
        long totalBytes = 0L;

        try
        {
            while (!operation.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    request.Abort();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                DateTime utcNow = DateTime.UtcNow;
                long downloadedBytes = (long)request.downloadedBytes;

                if (totalBytes == 0L && request.GetResponseHeader("Content-Length") != null &&
                    long.TryParse(request.GetResponseHeader("Content-Length"), out long result))
                {
                    totalBytes = result;
                }

                double totalSeconds = (utcNow - startTime).TotalSeconds;
                double bytesPerSecond = (totalSeconds > 0.0) ? (downloadedBytes / totalSeconds) : 0.0;

                progressCallback?.Invoke(new DownloadProgress
                {
                    DownloadSpeed = DownloadProgress.FormatDownloadSpeed(bytesPerSecond),
                    FileSizeInfo = DownloadProgress.FormatFileSize(downloadedBytes) + " / " + DownloadProgress.FormatFileSize(totalBytes)
                });

                await Task.Delay(25, cancellationToken);
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new HttpRequestException($"Download failed: {request.error}");
            }
        }
        catch (OperationCanceledException)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            throw;
        }
    }
}