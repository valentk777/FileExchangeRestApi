namespace FileExchangeRestApi.Domain.HttpClients;

using System.Threading.Tasks;
using FileExchangeRestApi.Contracts;

public interface IDemoHttpClient
{
	Task<DemoDomainFile> GetFile(string fileName);

	Task<bool> UploadFile(DemoDomainFile file);
}
