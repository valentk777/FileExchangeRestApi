using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileExchange.Domain.FileUploadServices;

public class FileUploadService : IFileUploadService
{
    private readonly ILogger<FileUploadService> _logger;
    private readonly IDemoHttpClient _demoHttpClient;

    public FileUploadService(ILogger<FileUploadService> logger, IDemoHttpClient demoHttpClient)
    {
        _logger = logger;
        _demoHttpClient = demoHttpClient;
    }

    public async Task<bool> UploadFileAsync(Stream stream, string? contentDisposition)
    {
        try
        {
            var fileName = GetFileName(contentDisposition);
            var file = new DemoDomainFile(fileName, stream);

            var result = await _demoHttpClient.UploadFileAsync(file);

            _logger.LogWarning("Result {result}", result);

            return true;
        } catch (Exception)
        {
            return false;
        }
    }

    private string GetFileName(string? contentDisposition)
    {
        if (string.IsNullOrEmpty(contentDisposition))
        {
            throw new ArgumentNullException(nameof(contentDisposition));
        }

        var fileNameStart = contentDisposition.IndexOf("filename=", StringComparison.OrdinalIgnoreCase);

        if (fileNameStart == -1)
        {
            throw new ArgumentException("Invalid Content-Disposition header", nameof(contentDisposition));
        }

        fileNameStart += "filename=".Length;

        var fileNameEnd = contentDisposition.IndexOf(";", fileNameStart, StringComparison.OrdinalIgnoreCase);

        if (fileNameEnd == -1)
        {
            fileNameEnd = contentDisposition.Length;
        }

        var fileName = contentDisposition.Substring(fileNameStart, fileNameEnd - fileNameStart).Trim('"');

        return fileName;
    }
}
