using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace FileExchange.Domain.FileDownloadServices;

public class FileDownloadService : IFileDownloadService
{
    private readonly ILogger<FileDownloadService> _logger;
    private readonly IDemoHttpClient _demoHttpClient;

    public FileDownloadService(ILogger<FileDownloadService> logger, IDemoHttpClient demoHttpClient)
    {
        _logger = logger;
        _demoHttpClient = demoHttpClient;
    }

    public async Task<DemoDomainFile> DownloadFileAsync(string fileName)
    {
        try
        {
            var file = await _demoHttpClient.GetFileAsync(fileName);

            _logger.LogInformation("File downloaded for file name {fileName}", fileName);

            return file;
        } catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return DemoDomainFile.Empty;
        }
    }
}
