
using System.Threading.Tasks;
using FileExchange.Contracts;

namespace FileExchange.Domain.HttpClients;

public interface IDemoHttpClient
{
	Task<DemoDomainFile> GetFile(string fileName);

	Task<bool> UploadFile(DemoDomainFile file);
}
