
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileExchange.Contracts;
using FileExchange.Contracts.Configs;
using FileExchange.Contracts.Exceptions;
using FileExchange.Domain;

namespace FileExchange.Infrastructure.HttpClients;

public class DemoHttpClient : IDemoHttpClient
{
    private readonly HttpClient _client;

    public DemoHttpClient(IOptions<ApplicationConfig> config, IHttpClientFactory httpClientFactory)
    {
        if (string.IsNullOrEmpty(config.Value.DemoClientBaseUrl))
        {
            throw new DomainValidationException(Constants.Logging.InvalidApplicationConfiguration);
        }

        var baseAddress = new Uri($"{config.Value.DemoClientBaseUrl.TrimEnd('/')}/");
        _client = GetHttpClient(httpClientFactory, baseAddress);
    }

    public async Task<DemoDomainFile> GetFileAsync(string fileName)
    {
        var uri = new Uri(_client.BaseAddress + fileName);

        // Note: do not use using here, because using will dispose stream and it will be empty in response
        var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationValidationException(response.StatusCode.ToString());
        }

        var contentStream = await response.Content.ReadAsStreamAsync();
        var mimeType = response.Content.Headers.ContentType?.MediaType;

        return new DemoDomainFile(fileName, contentStream) { MimeType = mimeType };
    }

    public async Task<bool> UploadFileAsync(DemoDomainFile file)
    {
        var uri = new Uri(_client.BaseAddress + file.Name);

        using var content = new MultipartFormDataContent
        {
            { new StreamContent(file.Content), "file", file.Name }
        };

        var response = await _client.PostAsync(uri, content);

        return response.IsSuccessStatusCode;
    }

    private HttpClient GetHttpClient(IHttpClientFactory httpClientFactory, Uri baseAddress)
    {
        var httpClient = httpClientFactory.CreateClient(Constants.HttpClient.HttpClientName);

        httpClient.BaseAddress = baseAddress;
        httpClient.DefaultRequestHeaders
          .Accept
          .Add(new MediaTypeWithQualityHeaderValue(Constants.HttpClient.ApplicationPdf));

        return httpClient;
    }
}
