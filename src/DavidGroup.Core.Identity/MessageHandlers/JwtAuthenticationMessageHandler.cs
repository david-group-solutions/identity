using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace DavidGroup.Core.Identity.MessageHandlers;

/// <summary>
/// A <see cref="DelegatingHandler"/> that forwards the <c>Authorization</c>
/// header from the current HTTP request to outgoing HTTP requests.
/// </summary>
public sealed class JwtAuthenticationMessageHandler(IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    /// <summary>
    /// Sends an HTTP request, forwarding the current request's
    /// <c>Authorization</c> header if one is available.
    /// </summary>
    /// <param name="request">The outgoing <see cref="HttpRequestMessage"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The HTTP response.</returns>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains(HeaderNames.Authorization) &&
            httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(
                HeaderNames.Authorization, out StringValues authorization) == true)
        {
            request.Headers.TryAddWithoutValidation(
                HeaderNames.Authorization,
                authorization.ToString()
            );
        }

        return base.SendAsync(request, cancellationToken);
    }
}
