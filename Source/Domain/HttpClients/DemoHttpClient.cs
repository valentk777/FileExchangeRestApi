﻿namespace FileExchangeRestApi.Domain.HttpClients;

using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileExchangeRestApi.Contracts;
using FileExchangeRestApi.Contracts.Configs;
using FileExchangeRestApi.Contracts.Exceptions;

public class DemoHttpClient : IDemoHttpClient
{
	private readonly HttpClient _client;

	public DemoHttpClient(IOptions<ApplicationConfig> config, IHttpClientFactory httpClientFactory)
	{
		if (string.IsNullOrEmpty(config.Value.DemoClientBaseUrl)) {
			throw new DomainValidationException(Constants.Logging.InvalidApplicationConfiguration);
		}

		var baseAddress = new Uri($"{config.Value.DemoClientBaseUrl.TrimEnd('/')}/");
		_client = GetHttpClient(httpClientFactory, baseAddress);
	}

	public async Task<DemoDomainFile> GetFile(string fileName)
	{
		var uri = new Uri(_client.BaseAddress + fileName);
		var request = new HttpRequestMessage(HttpMethod.Get, uri);

		using HttpResponseMessage response = await SendRequest(request);

		var fileBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

		return new DemoDomainFile(fileName, fileBytes);
	}

	public async Task<bool> UploadFile(DemoDomainFile file)
	{
		var uri = new Uri(_client.BaseAddress + file.Name);
		var request = new HttpRequestMessage(HttpMethod.Put, uri);

		request.Content = new ByteArrayContent(file.Content);

		using HttpResponseMessage response = await SendRequest(request);

		return true;
	}

	private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
	{
		var response = await _client.SendAsync(request);

		response.EnsureSuccessStatusCode();

		return response;
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