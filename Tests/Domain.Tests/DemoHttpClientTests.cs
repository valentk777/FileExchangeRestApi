
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using FileExchange.Contracts;
using FileExchange.Contracts.Configs;
using FileExchange.Contracts.Exceptions;
using FileExchange.Domain.HttpClients;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace FileExchange.Domain.Tests;
public class DemoHttpClientTests
{
	private TestHttpClientMock _testClient;
	private DemoHttpClient _DemoHttpClient;

	private string fileName = "fileName";
	private string responseContent = "Response content";
	private Stream _emptyBody = Stream.Null;
	private Stream _nonEmptyBody = new MemoryStream(Encoding.UTF8.GetBytes("body"));

	public DemoHttpClientTests()
	{
		var config = new ApplicationConfig
		{
			DemoClientBaseUrl = "http://www.test-url.com",
		};

		_testClient = new TestHttpClientMock(Constants.HttpClient.HttpClientName);
		_DemoHttpClient = new DemoHttpClient(Options.Create(config), _testClient.GetHttpClientFactory());
	}

	[Fact]
	public void GivenConfiguration_WhenCreatingDemoHttpClientWithNotValidUrl_ThrowDomainValidationException()
	{
		var config = new ApplicationConfig();

		Assert.Throws<DomainValidationException>(() =>
			new DemoHttpClient(
				Options.Create(config), _testClient.GetHttpClientFactory())
		);
	}

	[Fact]
	public async Task GivenDemoHttpClient_WhenGetPdf_CorrectUrlForRequestConstructed()
	{
		var responseContent = "Response content";
		var responseMessage = new HttpResponseMessage()
		{
			StatusCode = HttpStatusCode.OK,
			Content = new StringContent(responseContent)
		};

		_testClient.SetupSendAsync(responseMessage);

		await _DemoHttpClient.GetFile(fileName);

		_testClient.VerifySendAsync(r => r.RequestUri.AbsolutePath.Contains(fileName));
	}

	[Fact]
	public async Task GivenDemoHttpClient_WhenGetPdfForExistingFile_ReturnCorrectResultBody()
	{
		var responseMessage = new HttpResponseMessage()
		{
			StatusCode = HttpStatusCode.OK,
			Content = new StringContent(responseContent)
		};

		_testClient.SetupSendAsync(responseMessage);

		var file = await _DemoHttpClient.GetFile(fileName);

		Assert.NotNull(file);
		Assert.Equal(fileName, file.Name);

		StreamReader reader = new StreamReader(file.Content);
		var text = reader.ReadToEnd();
		Assert.Equal(responseContent, text);
	}

	[Fact]
	public void GivenDemoHttpClient_WhenGetPdfWithBadResponse_ThrowHttpRequestException()
	{
		var responseMessage = new HttpResponseMessage()
		{
			StatusCode = HttpStatusCode.NotFound,
			Content = new StringContent(responseContent)
		};

		_testClient.SetupSendAsync(responseMessage);

		Assert.ThrowsAsync<HttpRequestException>(async () => await _DemoHttpClient.GetFile(fileName));
	}

	[Fact]
	public async Task GivenDemoHttpClient_WhenUploadPdf_CorrectUrlForRequestConstructed()
	{
		var responseMessage = new HttpResponseMessage()
		{
			StatusCode = HttpStatusCode.OK,
		};

		_testClient.SetupSendAsync(responseMessage);

		var file = new DemoDomainFile(fileName, _nonEmptyBody);

		await _DemoHttpClient.UploadFile(file);

		_testClient.VerifySendAsync(r => r.RequestUri.AbsolutePath.Contains(fileName));
	}

	[Fact]
	public async Task GivenDemoHttpClient_WhenUploadPdfForExistingFile_ReturnResultWithoutErrors()
	{
		var responseMessage = new HttpResponseMessage()
		{
			StatusCode = HttpStatusCode.OK,
		};

		_testClient.SetupSendAsync(responseMessage);

		var file = new DemoDomainFile(fileName, _nonEmptyBody);

		var result = await _DemoHttpClient.UploadFile(file);

		Assert.True(result);
	}

	[Fact]
	public void GivenDemoHttpClient_WhenUploadPdfWithBadResponse_ThrowHttpRequestException()
	{
		var responseMessage = new HttpResponseMessage()
		{
			StatusCode = HttpStatusCode.Unauthorized
		};

		_testClient.SetupSendAsync(responseMessage);
		var file = new DemoDomainFile(fileName, _nonEmptyBody);

		Assert.ThrowsAsync<HttpRequestException>(async () => await _DemoHttpClient.UploadFile(file));
	}
}