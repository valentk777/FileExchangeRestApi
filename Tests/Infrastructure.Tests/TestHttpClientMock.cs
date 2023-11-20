using Moq;
using Moq.Protected;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileExchange.Infrastructure.Tests;
internal class TestHttpClientMock
{
    private Mock<HttpMessageHandler> _httpHandlerMock;
    private Mock<IHttpClientFactory> _mockHttpClientFactory;

    public TestHttpClientMock(string httpClientName)
    {
        _httpHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(_httpHandlerMock.Object)
        {
            BaseAddress = new Uri("http://www.test-url.com")
        };

        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(_ => _.CreateClient(httpClientName)).Returns(httpClient);
    }

    public IHttpClientFactory GetHttpClientFactory() =>
        _mockHttpClientFactory.Object;

    public void SetupSendAsync(HttpResponseMessage result) =>
        _httpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(result)
            .Verifiable();

    public void VerifySendAsync(Func<HttpRequestMessage, bool> match) =>
        _httpHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(request => match(request)),
                ItExpr.IsAny<CancellationToken>()
            );
}
