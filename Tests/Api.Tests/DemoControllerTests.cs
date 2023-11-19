
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;
using FileExchange.Api.Controllers;
using FileExchange.Contracts;
using FileExchange.Domain.HttpClients;
using Xunit;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace FileExchange.Api.Tests;
public class DemoControllerTests
{
	private const string fileName = "testFileName";

	private NullLogger<DemoController> _logger;
	private DemoDomainFile _responseFile;
	private IFormFile _uploadFile;

	public DemoControllerTests()
	{
		_logger = new NullLogger<DemoController>();
		_responseFile = new DemoDomainFile(fileName, new MemoryStream(Encoding.UTF8.GetBytes("pdf text")));
		_uploadFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("pdf text")), 0, 8, fileName, fileName);
	}

	[Fact]
	public async Task GivenDemoController_WhenGetFileCorrectFileInfo_ThenOkResponseAndCorrectFileData()
	{
		var httpClient = new Mock<IDemoHttpClient>();

		httpClient.Setup(x => x.GetFile(fileName)).ReturnsAsync(() => _responseFile);
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response as FileResult;

		Assert.NotNull(result);
		Assert.Equal("application/pdf", result.ContentType);
		Assert.Equal(_responseFile.Name, result.FileDownloadName);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("space between")]
	public async Task GivenDemoController_WhenGetFileWithIncorrectFileName_ThenBadRequestResponseAndNoFileData(string incorrectFileName)
	{
		var httpClient = new Mock<IDemoHttpClient>();
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(incorrectFileName);
		var result = response as ObjectResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
		Assert.NotNull(result.Value);
		Assert.Equal(string.Format(Constants.Logging.ProvidedBadParameterValue, "fileName", incorrectFileName), result.Value);
	}

	[Fact]
	public async Task GivenDemoController_WhenGetFileButEmptyFileReturned_ThenNoContentResponseAndNoFileData()
	{
		Stream fileContent = Stream.Null;
		var httpClient = new Mock<IDemoHttpClient>();
		httpClient.Setup(x => x.GetFile(fileName)).ReturnsAsync(() => new DemoDomainFile(fileName, fileContent));
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response as NoContentResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status204NoContent, result?.StatusCode);
	}

	[Fact]
	public async Task GivenDemoController_WhenGetFileButNonExistingFileReturned_ThenNoContentResponseAndNoFileData()
	{
		Stream fileContent = null;
		var httpClient = new Mock<IDemoHttpClient>();
		httpClient.Setup(x => x.GetFile(fileName)).ReturnsAsync(() => new DemoDomainFile(fileName, fileContent));
		var controller = new DemoController(_logger, httpClient.Object);

		var response = await controller.GetFile(fileName);
		var result = response as NoContentResult;

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
		var result = response as ObjectResult;

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
		var result = response as ObjectResult;

		Assert.NotNull(result);
		Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
		Assert.NotNull(result?.Value);
		Assert.Equal(exceptionMessage, result?.Value);
	}
}