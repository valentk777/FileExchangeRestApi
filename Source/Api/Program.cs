using FileExchange.Api.Extensions;
using FileExchange.Api.Handlers;
using FileExchange.Api.Middlewares;
using FileExchange.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using FileExchange.Infrastructure;
using FileExchange.Domain.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.ConfigureAllOptions(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddFileUploadService();
builder.Services.AddFileDownloadService();
builder.Services.AddDemoHttpClient();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<CommandLoggingMiddleware>();
app.MapControllers();
app.MapHealthChecks(Constants.Endpoint.Health);

app.Run();
