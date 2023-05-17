namespace FileExchangeRestApi.Api;

using System.Diagnostics.CodeAnalysis;
using FileExchangeRestApi.Api.Extensions;
using FileExchangeRestApi.Api.Middlewares;
using FileExchangeRestApi.Contracts;

[ExcludeFromCodeCoverage]
public class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.ConfigureAllOptions(builder.Configuration);
		builder.Services.AddHealthChecks();
		builder.Services.AddHttpClient();

		var app = builder.Build();

		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();
		app.UseAuthorization();
		app.UseMiddleware<CommandLoggingMiddleware>();
		app.MapControllers();
		app.MapHealthChecks(Constants.Endpoint.Health);
		app.Run();
	}
}
