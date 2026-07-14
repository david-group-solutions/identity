using System.Net;

using DavidGroup.Core.Identity.MessageHandlers;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DavidGroup.Core.Identity.Tests.MessageHandlers;

/// <summary>
/// Unit tests for <see cref="JwtAuthenticationMessageHandler"/>.
/// </summary>
public static class JwtAuthenticationMessageHandlerTests
{
    /// <summary>
    /// A stub <see cref="DelegatingHandler"/> used as the inner handler so the
    /// outgoing <see cref="HttpRequestMessage"/> can be captured and inspected
    /// without making a real network call.
    /// </summary>
    private sealed class CapturingInnerHandler : DelegatingHandler
    {
        public HttpRequestMessage? CapturedRequest { get; private set; }

        public HttpResponseMessage ResponseToReturn { get; set; } = new(HttpStatusCode.OK);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CapturedRequest = request;
            return Task.FromResult(ResponseToReturn);
        }
    }

    private static (JwtAuthenticationMessageHandler Handler, CapturingInnerHandler InnerHandler) CreateHandler(
        HttpContext? httpContext)
    {
        HttpContextAccessor accessor = new()
        {
            HttpContext = httpContext
        };

        CapturingInnerHandler innerHandler = new();

        JwtAuthenticationMessageHandler handler = new(accessor)
        {
            InnerHandler = innerHandler
        };

        return (handler, innerHandler);
    }

    /// <summary>
    /// Tests for the protected <c>SendAsync</c> override of
    /// <see cref="JwtAuthenticationMessageHandler"/>.
    /// </summary>
    public class SendAsyncTests
    {
        [Fact]
        public async Task SendAsync_CurrentRequestHasAuthorizationHeaderAndOutgoingRequestHasNone_ForwardsHeader()
        {
            // Arrange
            const string expectedAuthorizationValue = "Bearer abc123";
            DefaultHttpContext httpContext = new();
            httpContext.Request.Headers[HeaderNames.Authorization] = expectedAuthorizationValue;

            (JwtAuthenticationMessageHandler handler, CapturingInnerHandler innerHandler) = CreateHandler(httpContext);
            using HttpMessageInvoker invoker = new(handler);
            HttpRequestMessage outgoingRequest = new(HttpMethod.Get, "https://example.com/api");

            // Act
            await invoker.SendAsync(outgoingRequest, CancellationToken.None);

            // Assert
            HttpRequestMessage? capturedRequest = innerHandler.CapturedRequest;
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest.Headers.Contains(HeaderNames.Authorization));
            string actualAuthorizationValue = capturedRequest.Headers.GetValues(HeaderNames.Authorization).Single();
            Assert.Equal(expectedAuthorizationValue, actualAuthorizationValue);
        }

        [Fact]
        public async Task SendAsync_OutgoingRequestAlreadyHasAuthorizationHeader_DoesNotOverwriteIt()
        {
            // Arrange
            const string existingAuthorizationValue = "Bearer original-token";
            DefaultHttpContext httpContext = new();
            httpContext.Request.Headers[HeaderNames.Authorization] = "Bearer from-http-context";

            HttpRequestMessage outgoingRequest = new(HttpMethod.Get, "https://example.com/api");
            outgoingRequest.Headers.TryAddWithoutValidation(HeaderNames.Authorization, existingAuthorizationValue);

            (JwtAuthenticationMessageHandler handler, CapturingInnerHandler innerHandler) = CreateHandler(httpContext);
            using HttpMessageInvoker invoker = new(handler);

            // Act
            await invoker.SendAsync(outgoingRequest, CancellationToken.None);

            // Assert
            HttpRequestMessage? capturedRequest = innerHandler.CapturedRequest;
            Assert.NotNull(capturedRequest);
            string actualAuthorizationValue = capturedRequest.Headers.GetValues(HeaderNames.Authorization).Single();
            Assert.Equal(existingAuthorizationValue, actualAuthorizationValue);
        }

        [Fact]
        public async Task SendAsync_HttpContextIsNull_DoesNotAddAuthorizationHeader()
        {
            // Arrange
            (JwtAuthenticationMessageHandler handler, CapturingInnerHandler innerHandler) = CreateHandler(httpContext: null);
            using HttpMessageInvoker invoker = new(handler);
            HttpRequestMessage outgoingRequest = new(HttpMethod.Get, "https://example.com/api");

            // Act
            await invoker.SendAsync(outgoingRequest, CancellationToken.None);

            // Assert
            HttpRequestMessage? capturedRequest = innerHandler.CapturedRequest;
            Assert.NotNull(capturedRequest);
            Assert.False(capturedRequest.Headers.Contains(HeaderNames.Authorization));
        }

        [Fact]
        public async Task SendAsync_CurrentRequestHasNoAuthorizationHeader_DoesNotAddAuthorizationHeader()
        {
            // Arrange
            DefaultHttpContext httpContext = new();

            (JwtAuthenticationMessageHandler handler, CapturingInnerHandler innerHandler) = CreateHandler(httpContext);
            using HttpMessageInvoker invoker = new(handler);
            HttpRequestMessage outgoingRequest = new(HttpMethod.Get, "https://example.com/api");

            // Act
            await invoker.SendAsync(outgoingRequest, CancellationToken.None);

            // Assert
            HttpRequestMessage? capturedRequest = innerHandler.CapturedRequest;
            Assert.NotNull(capturedRequest);
            Assert.False(capturedRequest.Headers.Contains(HeaderNames.Authorization));
        }

        [Fact]
        public async Task SendAsync_ReturnsResponseProducedByInnerHandler()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            (JwtAuthenticationMessageHandler handler, CapturingInnerHandler innerHandler) = CreateHandler(httpContext);
            HttpResponseMessage expectedResponse = new(HttpStatusCode.NoContent);
            innerHandler.ResponseToReturn = expectedResponse;

            using HttpMessageInvoker invoker = new(handler);
            HttpRequestMessage outgoingRequest = new(HttpMethod.Get, "https://example.com/api");

            // Act
            HttpResponseMessage actualResponse = await invoker.SendAsync(outgoingRequest, CancellationToken.None);

            // Assert
            Assert.Same(expectedResponse, actualResponse);
        }
    }
}
