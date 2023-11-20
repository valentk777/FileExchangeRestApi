
using Microsoft.Extensions.Logging;
using FileExchange.Contracts;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FileExchange.Api.Middlewares;

public class CommandLoggingMiddleware
{
    private readonly ILogger<CommandLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public CommandLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<CommandLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        LogRequest(context);
        await _next(context);
        LogResponse(context);
    }

    private void LogRequest(HttpContext context)
    {
        var path = context.Request.Path.ToString();

        if (IsPathNotRootOrSwagger(path))
        {
            _logger.LogInformation(Constants.Logging.LogRequestMessage, context.Request.Method, path);
        }
    }

    private void LogResponse(HttpContext context)
    {
        var path = context.Request.Path.ToString();

        if (!IsPathNotRootOrSwagger(path))
        {
            return;
        }

        var statusCode = context.Response.StatusCode;

        if (statusCode < 300)
        {
            _logger.LogInformation(Constants.Logging.LogResponseMessage, context.Request.Method, path, statusCode);
        } else
        {
            _logger.LogError(Constants.Logging.LogResponseMessage, context.Request.Method, path, statusCode);
        }
    }

    private static bool IsPathNotRootOrSwagger(string path) =>
        path != string.Empty && path != Constants.Endpoint.Base && !path.Contains(Constants.Endpoint.Swagger);
}