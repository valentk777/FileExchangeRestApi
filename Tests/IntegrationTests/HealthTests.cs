namespace FileExchangeRestApi.Integration.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using FileExchangeRestApi.Api;
using FileExchangeRestApi.Contracts;
using Xunit;

public class HealthTests
{
	private HttpClient _httpClient;

	public HealthTests()
	{
		var webAppFactory = new WebApplicationFactory<Program>();
		_httpClient = webAppFactory.CreateDefaultClient();
	}

	[Fact]
	public async Task GivenApi_WhenGetHealthRout_ThenReturns200()
	{
		var response = await _httpClient.GetAsync(Constants.Endpoint.Health);
		var stringResult = await response.Content.ReadAsStringAsync();

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal("Healthy", stringResult);
	}
}