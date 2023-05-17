namespace FileExchangeRestApi.Contracts.Exceptions;

public class ApplicationValidationException : Exception
{
	public ApplicationValidationException(string message) : base(message)
	{
	}

	public ApplicationValidationException(Exception innerException) : base(string.Empty, innerException)
	{
	}

	public ApplicationValidationException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
