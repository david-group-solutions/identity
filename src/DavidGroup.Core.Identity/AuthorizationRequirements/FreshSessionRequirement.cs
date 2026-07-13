using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationRequirements;

/// <summary>
/// Represents an authorization requirement that ensures the authenticated user's
/// session is recent.
/// </summary>
/// <param name="tokenIssuanceAgeInSeconds">
/// The maximum allowed age, in seconds.
/// </param>
public class FreshSessionRequirement(long tokenIssuanceAgeInSeconds) : IAuthorizationRequirement
{
    /// <summary>
    /// The prefix used to identify fresh session authorization policies.
    /// </summary>
    public const string Prefix = "FreshSession:";

    /// <summary>
    /// Gets the maximum allowed age, in seconds.
    /// </summary>
    public long TokenIssuanceAgeInSeconds { get; } = tokenIssuanceAgeInSeconds;
}
