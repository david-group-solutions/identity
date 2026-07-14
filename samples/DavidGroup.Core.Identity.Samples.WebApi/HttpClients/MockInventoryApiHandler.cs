using System.Net;
using System.Net.Http.Headers;

namespace DavidGroup.Core.Identity.Samples.WebApi.HttpClients;

public sealed class MockInventoryApiHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AuthenticationHeaderValue? authorization = request.Headers.Authorization;

        string bookId = request.RequestUri!.Segments[^2].TrimEnd('/');

        var response = new
        {
            BookId = Guid.Parse(bookId),
            Available = true,
            AuthorizationHeader = authorization?.ToString()
        };

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(response)
        });
    }
}
