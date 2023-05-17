namespace FileExchangeRestApi.Contracts.Configs;

public class ApplicationConfig
{
	public const string SectionName = "Application";

	public string DemoClientBaseUrl { get; init; } = default!;
}