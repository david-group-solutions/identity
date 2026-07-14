using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace DavidGroup.Core.Identity.Extensions;

/// <summary>
/// Provides extension methods for working with HTTP request headers.
/// </summary>
public static class HeaderDictionaryExtensions
{
    /// <summary>
    /// Attempts to extract a Bearer token from the <c>Authorization</c> header.
    /// </summary>
    /// <param name="headers">The HTTP request headers.</param>
    /// <param name="token">
    /// When this method returns, contains the extracted Bearer token if one was
    /// successfully parsed; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a valid Bearer token was found; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The authentication scheme is matched case-insensitively, and any leading
    /// or trailing whitespace surrounding the token is ignored.
    /// </remarks>
    public static bool TryGetBearerToken(this IHeaderDictionary headers, out string? token)
    {
        token = null;

        if (!headers.TryGetValue(HeaderNames.Authorization, out StringValues authorization))
            return false;

        const string bearerPrefix = "Bearer ";

        string? authorizationValue = authorization.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorizationValue) ||
            !authorizationValue.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        ReadOnlySpan<char> tokenSpan = authorizationValue.AsSpan(bearerPrefix.Length).Trim();
        if (tokenSpan.IsEmpty)
            return false;

        token = tokenSpan.ToString();

        return true;
    }
}
