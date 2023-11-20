using Microsoft.AspNetCore.Mvc;
using FileExchange.Contracts;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using FileExchange.Domain.FileUploadServices;
using FileExchange.Domain.FileDownloadServices;
using System.IO;

namespace FileExchange.Api.Controllers;
[ApiController]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;
    private readonly IFileUploadService _fileUploadService;
    private readonly IFileDownloadService _fileDownloadService;

    public DemoController(ILogger<DemoController> logger, IFileUploadService fileUploadService, IFileDownloadService fileDownloadService)
    {
        _logger = logger;
        _fileUploadService = fileUploadService;
        _fileDownloadService = fileDownloadService;
    }

    [HttpGet]
    [Route(Constants.Routes.GetFileRoute)]
    public async Task<IResult> DownloadFile([FromRoute] string? fileName)
    {
        if (string.IsNullOrEmpty(fileName) || fileName.Contains(" "))
        {
            _logger.LogError(Constants.Logging.ProvidedBadParameterValue, nameof(fileName), fileName);
            return Results.BadRequest(string.Format(Constants.Logging.ProvidedBadParameterValue, nameof(fileName), fileName));
        }

        try
        {
            var file = await _fileDownloadService.DownloadFileAsync(fileName);

            if (file.IsValid())
            {
                return Results.File(file.Content, Constants.HttpClient.ApplicationPdf, file.Name);
            }

            _logger.LogInformation(Constants.Logging.FileIsNullOrEmpty);

            return Results.NoContent();

        } catch (HttpRequestException e)
        {
            _logger.LogError(e.Message);
            return Results.BadRequest(e.Message);
        } catch (Exception e)
        {
            _logger.LogError(e.Message, e.StackTrace);
            return Results.BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route(Constants.Routes.UploadFileRoute)]
    public async Task<IResult> UploadFile()
    {
        try
        {
            var boundary = Request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                await _fileUploadService.UploadFileAsync(section.Body, section.ContentDisposition);

                section = await reader.ReadNextSectionAsync();
            }

            return Results.Ok("File(s) uploaded successfully");
        } catch (HttpRequestException e)
        {
            _logger.LogError(e.Message);
            return Results.BadRequest(e.Message);
        } catch (Exception e)
        {
            _logger.LogError(e.Message, e.StackTrace);
            return Results.BadRequest(e.Message);
        }
    }
}
