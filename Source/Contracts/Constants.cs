namespace FileExchange.Contracts;

public static class Constants
{
    public static class Logging
    {
        public const string InvalidApplicationConfiguration = "Application configuration is invalid.";

        public const string FileIsNullOrEmpty = "Pdf is null or empty.";

        public const string LogRequestMessage = "Request:\n      Path: {0} -> {1}";

        public const string LogResponseMessage = "Response:\n      Path: {0} -> {1}\n      Status code: {2}";

        public const string ProvidedBadParameterValue = "Provided parameter '{0}' was incorrect. Actual value: '{1}'";
    }

    public static class Endpoint
    {
        public const string Base = "/";

        public const string Health = "/health";

        public const string Swagger = "swagger";
    }

    public static class Routes
    {
        public const string GetFileRoute = "api/demo/file/{fileName}";

        public const string UploadFileRoute = "api/demo/file/upload";
    }

    public static class HttpClient
    {
        public const string HttpClientName = "DEMO-HTTP-CLIENT";

        public const string ContentType = "Content-Type";

        public const string ApplicationPdf = "application/pdf";

        public const string AuthorizationHeader = "Authorization";
    }
}
