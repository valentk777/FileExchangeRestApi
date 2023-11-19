
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using FileExchange.Contracts;
using FileExchange.Contracts.Configs;
using FileExchange.Domain.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileExchange.Api.Extensions;
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
	public static void ConfigureAllOptions(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<ApplicationConfig>(configuration.GetSection(ApplicationConfig.SectionName));
		services.Configure<LoggingConfig>(configuration.GetSection(LoggingConfig.SectionName));
	}

	public static void AddDemoHttpClient(this IServiceCollection services)
	{
		// Single HTTP client per client name.
		services.AddHttpClient(Constants.HttpClient.HttpClientName).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

		services.AddSingleton<IDemoHttpClient, DemoHttpClient>();
	}
}