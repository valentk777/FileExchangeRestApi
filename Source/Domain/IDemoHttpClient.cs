using System.Threading.Tasks;

namespace FileExchange.Domain;

public interface IDemoHttpClient
{
    Task<DemoDomainFile> GetFileAsync(string fileName);

    Task<bool> UploadFileAsync(DemoDomainFile file);
}
