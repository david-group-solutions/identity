using DavidGroup.Core.Identity.Extensions;

using Microsoft.AspNetCore.Mvc;

namespace DavidGroup.Core.Identity.Controllers;

/// <summary>
/// Base controller providing helper methods to access the current
/// authenticated user's identity information.
/// </summary>
public class IdentityController : ControllerBase
{
    /// <summary>
    /// Retrieves a required claim value for the current user using
    /// <see cref="ClaimsPrincipalExtensions.GetRequiredClaim"/>.
    /// </summary>
    protected string GetRequiredClaim(string claimType)
        => HttpContext.User.GetRequiredClaim(claimType);

    /// <summary>
    /// Retrieves the specified claim for the current user and parses its value
    /// to the specified type using <see cref="ClaimsPrincipalExtensions.GetRequiredClaim"/>.
    /// </summary>
    protected T GetRequiredClaim<T>(string claimType) where T : IParsable<T>
        => HttpContext.User.GetRequiredClaim<T>(claimType);

    /// <summary>
    /// Attempts to retrieve and parse the specified claim to the specified type
    /// for the current user using <see cref="ClaimsPrincipalExtensions.TryGetClaim"/>.
    /// </summary>
    protected bool TryGetClaim<T>(string claimType, out T? value) where T : IParsable<T>
        => HttpContext.User.TryGetClaim<T>(claimType, out value);
}
