namespace FileExchangeRestApi.Integration.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FileExchangeRestApi.Api;
using FileExchangeRestApi.Contracts;
using Xunit;

public class FileExchangeTests
{
	private HttpClient _httpClient;

	public FileExchangeTests()
	{
		var webAppFactory = new WebApplicationFactory<Program>();
		_httpClient = webAppFactory.CreateClient();
	}

	[Fact]
	public async Task GivenApi_WhenGetFileWithCorrectFileName_ThenReturns200()
	{
		var fileName = "sample.pdf";
		var route = Constants.Routes.GetFileRoute.Replace("{fileName}", fileName);

		var response = await _httpClient.GetAsync(route);

		Assert.True(response.IsSuccessStatusCode);

		var file = await response.Content.ReadAsAsync<DemoDomainFile>();

		Assert.Equal(fileName, file.Name);
		Assert.NotNull(file.Content);
		Assert.Equal(3028, file.Content.Length);

		PdfLoadedDocument loadedDocument = new PdfLoadedDocument(file.Content);
		PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;

		page.ExtractText(out var content);
		Assert.Contains("A Simple PDF File", content.TextLine.First().Text);

		// Note: for local testing
		//File.WriteAllBytes("integration-test-output-pdf.pdf", result);
	}

	[Fact(Skip = "Only when I have actual endpoint")]
	public async Task GivenApi_WhenUploadFileWithCorrectFileNameAndContent_ThenReturns200()
	{
		var fileName = "integration-test-upload-file.pdf";
		var fileBytes = await GetPdfFile(fileName);
		var fileContent = new ByteArrayContent(fileBytes);
		fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
		var formData = new MultipartFormDataContent {
			{ fileContent, "file", fileName }
		};

		var response = await _httpClient.PostAsync(Constants.Routes.UploadFileRoute, formData);

		Assert.True(response.IsSuccessStatusCode);
	}

	private async Task<byte[]> GetPdfFile(string filePath) =>
		await File.ReadAllBytesAsync(filePath);

	private async Task<byte[]> GetBytesFromResponseContent(HttpResponseMessage response)
	{
		var bytes = await response.Content.ReadAsByteArrayAsync();
		var encoded = Encoding.UTF8.GetString(bytes).Replace("\"", "");
		return Convert.FromBase64String(encoded);
	}
}