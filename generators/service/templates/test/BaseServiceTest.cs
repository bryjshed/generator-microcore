using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class BaseServiceTest
    {
        [Fact]
        public async void HttpResonse_OkResponse()
        {
            // Arrange
            Uri getAccountsUi = new Uri("http://localhost/");
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject("Test"));

            Mock<HttpClientHandler> mockHandler = new Mock<HttpClientHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(message => message.RequestUri.AbsoluteUri.Contains(getAccountsUi.AbsoluteUri)),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(response));
            
            HttpClient httpClient = new HttpClient(mockHandler.Object);

            var service = new BaseServiceStub(
                new Mock<ILogger<BaseService>>().Object,
                new AppSettings(),
                httpClient);
            
            // Act
            var result = await service.ProcessResponseAsync<string>(response, "Test");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async void HttpResonse_NotFound()
        {
            // Arrange
            Uri getAccountsUi = new Uri("http://localhost/");
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            response.Content = new StringContent(JsonConvert.SerializeObject("Test"));

            Mock<HttpClientHandler> mockHandler = new Mock<HttpClientHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(message => message.RequestUri.AbsoluteUri.Contains(getAccountsUi.AbsoluteUri)),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(response));
            
            HttpClient httpClient = new HttpClient(mockHandler.Object);

            var service = new BaseServiceStub(
                new Mock<ILogger<BaseService>>().Object,
                new AppSettings(),
                httpClient);
            
            // Act
            var result = await service.ProcessResponseAsync<string>(response, "Test");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async void HttpResonse_BadRequest()
        {
            // Arrange
            Uri getAccountsUi = new Uri("http://localhost/");
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var errors = new Dictionary<string, IList<string>>();
            errors.Add("Error", new List<string>() {"Some random error"});
            response.Content = new StringContent(JsonConvert.SerializeObject(errors));

            Mock<HttpClientHandler> mockHandler = new Mock<HttpClientHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(message => message.RequestUri.AbsoluteUri.Contains(getAccountsUi.AbsoluteUri)),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(response));
            
            HttpClient httpClient = new HttpClient(mockHandler.Object);

            var service = new BaseServiceStub(
                new Mock<ILogger<BaseService>>().Object,
                new AppSettings(),
                httpClient);
            
            // Act
            var result = await service.ProcessResponseAsync<string>(response, "Test");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async void HttpResonse_ServiceUnavailableException()
        {
            // Arrange
            Uri getAccountsUi = new Uri("http://localhost/");
            var response = new HttpResponseMessage();

            Mock<HttpClientHandler> mockHandler = new Mock<HttpClientHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(message => message.RequestUri.AbsoluteUri.Contains(getAccountsUi.AbsoluteUri)),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new ServiceUnavailableException());

            HttpClient httpClient = new HttpClient(mockHandler.Object);

            var service = new BaseServiceStub(
                new Mock<ILogger<BaseService>>().Object,
                new AppSettings(),
                httpClient);

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(async () => await service.ProcessResponseAsync<string>(response, "Test"));
        }
    }
}