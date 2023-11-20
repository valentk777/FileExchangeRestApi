using System.ComponentModel.DataAnnotations;

namespace FileExchange.Contracts.Configs;

public class ApplicationConfig
{
    public const string SectionName = "Application";

    [Url]
    [MinLength(1)]
    public string DemoClientBaseUrl { get; init; } = default!;
}