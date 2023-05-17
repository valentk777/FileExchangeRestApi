namespace FileExchangeRestApi.Api.Extensions;

using System.Diagnostics.CodeAnalysis;
using FileExchangeRestApi.Contracts;
using FileExchangeRestApi.Contracts.Configs;
using FileExchangeRestApi.Domain.HttpClients;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
	public static void ConfigureAllOptions(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<ApplicationConfig>(configuration.GetSection(ApplicationConfig.SectionName));
		services.Configure<LoggingConfig>(configuration.GetSection(LoggingConfig.SectionName));
	}

	public static void AddHttpClient(this IServiceCollection services)
	{
		// Single HTTP client per client name.
		services.AddHttpClient(Constants.HttpClient.HttpClientName).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

		services.AddSingleton<IDemoHttpClient, DemoHttpClient>();
	}
}