using System.Security.Claims;

using DavidGroup.Core.Identity.Data;
using DavidGroup.Core.Identity.Extensions;

namespace DavidGroup.Core.Identity.Utilities;

/// <summary>
/// Provides helper methods for working with authenticated users.
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// Determines whether the user authenticated within the specified time period.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <param name="maxAge">
    /// The maximum allowed time elapsed since authentication.
    /// </param>
    /// <param name="now">
    /// The current time used for comparison. If not specified,
    /// <see cref="DateTimeOffset.UtcNow"/> is used.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the user's authentication time exists and falls
    /// within the specified age; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The authentication time is read from the
    /// <see cref="DavidGroupClaimTypes.AuthTime"/> claim and is expected to be
    /// stored as a Unix timestamp in seconds.
    /// </remarks>
    public static bool IsAuthenticatedWithin(ClaimsPrincipal user, TimeSpan maxAge, DateTimeOffset? now = null)
    {
        now ??= DateTimeOffset.UtcNow;

        if (!user.TryGetClaim(DavidGroupClaimTypes.AuthTime, out long autoTimeSeconds))
            return false;

        DateTimeOffset authTime = DateTimeOffset.FromUnixTimeSeconds(autoTimeSeconds);

        TimeSpan age = now.Value - authTime;

        if (age < TimeSpan.Zero) return false;

        return age <= maxAge;
    }
}
