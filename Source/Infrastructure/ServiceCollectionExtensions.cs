using System.Net.Http;
using FileExchange.Contracts;
using FileExchange.Domain;
using FileExchange.Infrastructure.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace FileExchange.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDemoHttpClient(this IServiceCollection services)
    {
        // Single HTTP client per client name.
        services.AddHttpClient(Constants.HttpClient.HttpClientName).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

        services.AddSingleton<IDemoHttpClient, DemoHttpClient>();

        return services;
    }
}