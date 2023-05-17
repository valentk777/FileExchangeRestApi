namespace FileExchangeRestApi.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using FileExchangeRestApi.Contracts;
using FileExchangeRestApi.Domain.HttpClients;

[ApiController]
public class DemoController : ControllerBase
{
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
	public async Task<ActionResult<DemoDomainFile>> GetFile([FromRoute] string? fileName)
	{
		// fileName.Contains(" ") - it is used in URL later, so we cannot have spaces.
		if (string.IsNullOrEmpty(fileName) || fileName.Contains(" ")) {
			_logger.LogError(Constants.Logging.ProvidedBadParameterValue, nameof(fileName), fileName);
			return BadRequest(string.Format(Constants.Logging.ProvidedBadParameterValue, nameof(fileName), fileName));
		}

		try {
			var file = await _httpClient.GetFile(fileName);

			if (file.IsValid()) {
				// NOTE: ASP.NET converts the byte[] to Base64 behind the scenes.
				return Ok(file);
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
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<bool>> UploadFile(IFormFile file)
	{
		var domainFile = new DemoDomainFile(file);

		if (!domainFile.IsValid()) {
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