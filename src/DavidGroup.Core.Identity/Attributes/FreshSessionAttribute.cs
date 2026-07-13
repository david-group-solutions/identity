using DavidGroup.Core.Identity.AuthorizationRequirements;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Attributes;

/// <summary>
/// Requires the authenticated user's session to be recent.
/// </summary>
/// <remarks>
/// This attribute applies a parameterized authorization policy that validates
/// the age of the access token. Access is granted only if the token was issued
/// no more than the specified number of seconds ago.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class FreshSessionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FreshSessionAttribute"/> class.
    /// </summary>
    /// <param name="tokenIssuanceAgeInSeconds">
    /// The maximum allowed age, in seconds, of the access token since it was issued.
    /// </param>
    public FreshSessionAttribute(long tokenIssuanceAgeInSeconds)
    {
        Policy = $"{FreshSessionRequirement.Prefix}:{tokenIssuanceAgeInSeconds}";
    }
}
