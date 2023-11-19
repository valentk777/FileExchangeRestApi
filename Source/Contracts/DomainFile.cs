
using System.IO;

namespace FileExchange.Contracts;

public record DemoDomainFile(string Name, Stream Content)
{
	//public Stream Content { get; init; } = Stream.Null;
	//public byte[] Content { get; init; } = Array.Empty<byte>();

	//public DemoDomainFile()
	//{
	//}

	//public DemoDomainFile(string fileName, byte[]? data)
	//{
	//	Name = fileName;
	//	Content = data ?? Array.Empty<byte>();
	//}

	//public DemoDomainFile(string fileName, Stream? data)
	//{
	//	Name = fileName;
	//	Content = data ?? Stream.Null;
	//}

	//public DemoDomainFile(IFormFile? file)
	//{
	//	if (file == null) {
	//		return;
	//	}

	//	if (!string.IsNullOrEmpty(file.FileName) && !file.FileName.Contains(" ")) {
	//		Name = file.FileName;
	//	}

	//	Content = ConvertToByteArray(file) ?? Array.Empty<byte>();
	//}

	public bool IsValid() =>
		!string.IsNullOrEmpty(Name) && !Name.Contains(" ");

	//public bool IsValid() =>
	//	!string.IsNullOrEmpty(Name) && !Name.Contains(" ") && Content != null && Content.Length != 0;

	//private byte[] ConvertToByteArray(IFormFile file)
	//{
	//	using var memoryStream = new MemoryStream();
	//	file.CopyTo(memoryStream);

	//	return memoryStream.ToArray();
	//}
}