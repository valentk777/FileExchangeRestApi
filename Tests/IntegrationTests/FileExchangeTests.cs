
using Microsoft.AspNetCore.Mvc.Testing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FileExchange.Contracts;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Reflection.Metadata;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;

namespace FileExchange.Integration.Tests;

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
        Assert.NotNull(response.Content);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

        var byteArray = await response.Content.ReadAsByteArrayAsync();
        Assert.Equal(3028, byteArray.Length);

        PdfLoadedDocument loadedDocument = new PdfLoadedDocument(byteArray);
        PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;

        page.ExtractText(out var content);
        Assert.Equal("A Simple PDF File ", content.TextLine.First().Text);

        // Note: for local testing
        //File.WriteAllBytes("integration-test-output-pdf.pdf", result);
    }

	[Fact]
	public async Task GivenApi_WhenUploadFileWithCorrectFileNameAndContent_ThenReturns200()
	{
		var fileName = "integration-test-upload-file.pdf";
		var fileContent = GetPdfFile(fileName);

		var response = await _httpClient.PostAsync(Constants.Routes.UploadFileRoute, fileContent);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("\"File(s) uploaded successfully\"", await response.Content.ReadAsStringAsync());
	}

    //private async Task<byte[]> GetPdfFile(string filePath) =>
    //	await File.ReadAllBytesAsync(filePath);

    private MultipartFormDataContent GetPdfFile(string fileName)
    {
        var content = new MultipartFormDataContent();
        var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        content.Add(new StreamContent(fileStream), "file", fileName);

        return content;
    }

    //   private async Task<byte[]> GetBytesFromResponseContent(HttpResponseMessage response)
    //{
    //	var bytes = await response.Content.ReadAsByteArrayAsync();
    //	var encoded = Encoding.UTF8.GetString(bytes).Replace("\"", "");
    //	return Convert.FromBase64String(encoded);
    //}
}