using FileExchange.Domain.FileDownloadServices;
using FileExchange.Domain.FileUploadServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace FileExchange.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFileUploadService(this IServiceCollection services)
    {
        services.AddScoped<IFileUploadService, FileUploadService>();

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue;
        });
    }

    public static void AddFileDownloadService(this IServiceCollection services)
    {
        services.AddScoped<IFileDownloadService, FileDownloadService>();

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue;
        });
    }
}
