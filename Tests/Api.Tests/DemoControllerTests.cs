namespace FileExchangeRestApi.Api.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;
using FileExchangeRestApi.Api.Controllers;
using FileExchangeRestApi.Contracts;
using FileExchangeRestApi.Domain.HttpClients;
using Xunit;

public class DemoControllerTests
{
	private const string fileName = "testFileName";

	private NullLogger<DemoController> _logger;
	private DemoDomainFile _file;

	public DemoControllerTests()
	{
		_logger = new NullLogger<DemoController>();
		_file = new DemoDomainFile(fileName, Encoding.ASCII.GetBytes("pdf text"));
	}

	[Fact]
	public async Task GivenDemoController_WhenGetFileCorrectFileInfo_ThenOkResponseAndCorrectFileData()
	{
		var httpClient = new Mock<IDemoHttpClient>();

		httpClient.Setup(x => x.GetFile(fileName)).ReturnsAsync(() => _file);
		var controller = new DemoController(_logger, httpClient.Object);

		ActionResult<DemoDomainFile> response = await controller.GetFile(fileName);
		var result = response.Result as ObjectResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
		Assert.NotNull(result?.Value);

		var file = result.Value as DemoDomainFile;

		Assert.Equal(_file.Name, file.Name);
		Assert.Equal(_file.Content, file.Content);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task GivenDemoController_WhenGetFileWithIncorrectFileName_ThenBadRequestResponseAndNoFileData(string fileName)
	{
		var httpClient = new Mock<IDemoHttpClient>();
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response.Result as ObjectResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
		Assert.NotNull(result?.Value);
		Assert.Equal(string.Format(Constants.Logging.ProvidedBadParameterValue, "fileName", fileName), result?.Value);
	}

	[Theory]
	[InlineData(null)]
	[InlineData(new byte[0])]
	public async Task GivenDemoController_WhenGetFileButEmptyFileReturned_ThenNoContentResponseAndNoFileData(byte[] fileContent)
	{
		var httpClient = new Mock<IDemoHttpClient>();
		httpClient.Setup(x => x.GetFile(fileName)).ReturnsAsync(() => new DemoDomainFile(fileName, fileContent));

		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response.Result as NoContentResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status204NoContent, result?.StatusCode);
	}

	[Fact]
	public async Task GivenDemoController_WhenGetFileThrowHttpRequestException_ThenBadRequestResponseAndNoFileData()
	{
		var httpClient = new Mock<IDemoHttpClient>();
		var exceptionMessage = "exception message";
		httpClient.Setup(x => x.GetFile(fileName)).ThrowsAsync(new HttpRequestException(exceptionMessage));
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response.Result as ObjectResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
		Assert.NotNull(result?.Value);
		Assert.Equal(exceptionMessage, result?.Value);
	}

	[Fact]
	public async Task GivenDemoController_WhenGetFileThrowUnexpectedException_ThenBadRequestResponseAndNoFileData()
	{
		var httpClient = new Mock<IDemoHttpClient>();
		var exceptionMessage = "exception message";
		httpClient.Setup(x => x.GetFile(fileName)).ThrowsAsync(new Exception(exceptionMessage));
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response.Result as ObjectResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
		Assert.NotNull(result?.Value);
		Assert.Equal(exceptionMessage, result?.Value);
	}
}