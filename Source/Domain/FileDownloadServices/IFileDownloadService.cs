using System.Threading.Tasks;

namespace FileExchange.Domain.FileDownloadServices;

public interface IFileDownloadService
{
    Task<DemoDomainFile> DownloadFileAsync(string fileName);
}
