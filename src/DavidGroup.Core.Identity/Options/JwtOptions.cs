using System.ComponentModel.DataAnnotations;

namespace DavidGroup.Core.Identity.Options;

/// <summary>
/// Represents the configuration options used for JWT token validation.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Gets the expected issuer (<c>iss</c>) claim of issued JWTs.
    /// </summary>
    [Required]
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Gets the expected audience (<c>aud</c>) claim of issued JWTs.
    /// </summary>
    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Gets the secret key used to sign and validate JWTs.
    /// </summary>
    /// <remarks>
    /// This value should be a sufficiently long, cryptographically secure secret
    /// and must be kept confidential.
    /// </remarks>
    [Required]
    public string SecretKey { get; init; } = string.Empty;
}
