using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationRequirements;

/// <summary>
/// Represents an authorization requirement that ensures the authenticated user
/// has a confirmed phone number.
/// </summary>
public sealed class PhoneConfirmedRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The name of the authorization policy associated with this requirement.
    /// </summary>
    public const string PolicyName = "PhoneConfirmed";
}
