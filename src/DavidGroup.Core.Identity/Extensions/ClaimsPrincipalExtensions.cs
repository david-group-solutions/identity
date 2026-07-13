using System.Security.Claims;

namespace DavidGroup.Core.Identity.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the value of the specified claim from the current
    /// <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="user">The claims principal containing the claim.</param>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>
    /// The value of the specified claim.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified claim is not found.
    /// </exception>
    public static string GetRequiredClaim(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirstValue(claimType)
               ?? throw new InvalidOperationException($"Claim '{claimType}' was not found.");
    }

    /// <summary>
    /// Retrieves the specified claim from the current <see cref="ClaimsPrincipal"/>
    /// and parses its value to the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The target type to parse the claim value into. The type must implement
    /// <see cref="IParsable{TSelf}"/>.
    /// </typeparam>
    /// <param name="user">The claims principal containing the claim.</param>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns> The parsed claim value. </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified claim is not found.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when the claim value cannot be parsed as <typeparamref name="T"/>.
    /// </exception>
    public static T GetRequiredClaim<T>(this ClaimsPrincipal user, string claimType)
        where T : IParsable<T>
    {
        string valueStr = user.FindFirstValue(claimType)
                          ?? throw new InvalidOperationException($"Claim '{claimType}' was not found.");;

        return T.Parse(valueStr, provider: null);
    }

    /// <summary>
    /// Attempts to retrieve and parse the claim from the current <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the claim. The type must implement <see cref="IParsable{TSelf}"/>.
    /// </typeparam>
    /// <param name="user">The claims principal containing the user claims.</param>
    /// <param name="claimType">The claim to retrieve.</param>
    /// <param name="value">
    /// When this method returns <see langword="true"/>, contains the parsed value;
    /// otherwise, contains the default value for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the subject claim exists and was successfully parsed;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetClaim<T>(this ClaimsPrincipal user, string claimType, out T? value)
        where T : IParsable<T>
    {
        value = default;

        string? valueStr = user.FindFirstValue(claimType);
        if (string.IsNullOrWhiteSpace(valueStr))
            return false;

        return T.TryParse(valueStr, provider: null, out value);
    }
}
