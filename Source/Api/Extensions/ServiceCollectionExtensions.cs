using FileExchange.Contracts.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileExchange.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureAllOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ApplicationConfig>().Bind(configuration.GetSection(ApplicationConfig.SectionName)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<LoggingConfig>().Bind(configuration.GetSection(LoggingConfig.SectionName)).ValidateDataAnnotations().ValidateOnStart();
    }
}