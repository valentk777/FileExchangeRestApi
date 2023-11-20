using System.IO;
using System.Threading.Tasks;

namespace FileExchange.Domain.FileUploadServices;

public interface IFileUploadService
{
    Task<bool> UploadFileAsync(Stream stream, string? contentDisposition);
}
