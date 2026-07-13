using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationRequirements;

/// <summary>
/// Represents an authorization requirement that ensures the authenticated user
/// has a confirmed email address.
/// </summary>
public sealed class EmailConfirmedRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The name of the authorization policy associated with this requirement.
    /// </summary>
    public const string PolicyName = "EmailConfirmed";
}
