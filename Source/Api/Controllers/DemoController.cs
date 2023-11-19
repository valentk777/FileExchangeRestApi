
using Microsoft.AspNetCore.Mvc;
using FileExchange.Contracts;
using FileExchange.Domain.HttpClients;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using FileExchange.Api.Attributes;

namespace FileExchange.Api.Controllers;
[ApiController]
public class DemoController : ControllerBase
{
	private const long MaxFileSize = 1L * 1024L * 1024L * 1024L; // 1GB, adjust to your need

	private readonly ILogger<DemoController> _logger;
	private readonly IDemoHttpClient _httpClient;

	public DemoController(ILogger<DemoController> logger, IDemoHttpClient holterHttpClient)
	{
		_logger = logger;
		_httpClient = holterHttpClient;
	}

	[HttpGet]
	[Route(Constants.Routes.GetFileRoute)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[DisableFormValueModelBinding]
	[RequestSizeLimit(MaxFileSize)]
	[RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
	public async Task<IActionResult> GetFile([FromRoute] string? fileName)
	{
		// fileName.Contains(" ") - it is used in URL later, so we cannot have spaces.
		if (string.IsNullOrEmpty(fileName) || fileName.Contains(" ")) {
			_logger.LogError(Constants.Logging.ProvidedBadParameterValue, nameof(fileName), fileName);
			return BadRequest(string.Format(Constants.Logging.ProvidedBadParameterValue, nameof(fileName), fileName));
		}

		try {
			var file = await _httpClient.GetFile(fileName);

			if (file.IsValid()) {
				return File(file.Content, Constants.HttpClient.ApplicationPdf, file.Name);

				// NOTE: ASP.NET converts the byte[] to Base64 behind the scenes.
				//return Ok(file);
			}

			_logger.LogInformation(Constants.Logging.FileIsNullOrEmpty);
			return NoContent();

		} catch (HttpRequestException e) {
			_logger.LogError(e.Message);
			return BadRequest(e.Message);
		} catch (Exception e) {
			_logger.LogError(e.Message, e.StackTrace);
			return BadRequest(e.Message);
		}
	}

	[HttpPost]
	[Route(Constants.Routes.UploadFileRoute)]
	[ProducesResponseType(typeof(DemoDomainFile), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[DisableFormValueModelBinding]
	[RequestSizeLimit(MaxFileSize)]
	[RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
	public async Task<IActionResult> UploadFile(IFormFile file)
	{
		using var stream = file.OpenReadStream();
		var domainFile = new DemoDomainFile(file.FileName, stream);

		if (!domainFile.IsValid() || file.Length == 0) {
			_logger.LogError(Constants.Logging.ProvidedBadParameterValue, nameof(file), file);
			return BadRequest(string.Format(Constants.Logging.ProvidedBadParameterValue, nameof(file), file));
		}

		try {
			var result = await _httpClient.UploadFile(domainFile);

			return Ok(result);
		} catch (HttpRequestException e) {
			_logger.LogError(e.Message);
			return BadRequest(e.Message);
		} catch (Exception e) {
			_logger.LogError(e.Message, e.StackTrace);
			return BadRequest(e.Message);
		}
	}
}