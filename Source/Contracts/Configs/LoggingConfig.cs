using System.Collections.Generic;

namespace FileExchange.Contracts.Configs;

public class LoggingConfig
{
	public const string SectionName = "Logging";

	public LogLevel Level { get; init; } = default!;

	public bool LogRegularFormat { get; set; } = default!;

	public IReadOnlyCollection<string>? FilterExpressions = new List<string>();
}