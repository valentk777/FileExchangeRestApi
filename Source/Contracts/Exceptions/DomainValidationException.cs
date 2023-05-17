namespace FileExchangeRestApi.Contracts.Exceptions;

public class DomainValidationException : Exception
{
	public DomainValidationException(string message) : base(message)
	{
	}

	public DomainValidationException(Exception innerException) : base(string.Empty, innerException)
	{
	}

	public DomainValidationException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
