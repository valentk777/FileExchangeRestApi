using System.IO;

namespace FileExchange.Domain;

public record DemoDomainFile(string Name, Stream Content)
{
    public string? MimeType { get; set; }

    public static DemoDomainFile Empty => new DemoDomainFile(string.Empty, Stream.Null);

    public bool IsValid() =>
        !string.IsNullOrEmpty(Name) && !Name.Contains(" ");
}